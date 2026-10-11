#!/usr/bin/env python3
"""
export_coverage_report.py — Generate a text file describing all major blocks
in the .GM1 save file, their start/end offsets, what they contain, and whether
we fully parse them.

Output: save_coverage_report_<map_name>.txt
"""
import os
import sys
import struct
import json

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from map_config_builder import decompress_save, load_config
from save_parser import parse_save
from header_parser import parse_header


def generate_coverage_report(save_path, config_path, output_path=None):
    raw = decompress_save(save_path)

    # If config_path is empty, try to find it next to the save
    if not config_path:
        save_dir = os.path.dirname(save_path)
        for fn in os.listdir(save_dir):
            if (fn.startswith("save_map_config_") or fn.startswith("map_config_")) and fn.endswith(".json"):
                config_path = os.path.join(save_dir, fn)
                break
    if not config_path:
        raise ValueError("No map config found. Place save_map_config_*.json next to save.")

    config = load_config(config_path)
    parsed = parse_save(raw, config)

    msi = getattr(parsed, '_map_start_info', {})
    pts = getattr(parsed, '_post_tile_sections', {})
    header = parse_header(raw)
    ptc = getattr(parsed, '_post_tile_content', {})

    map_name = header.map_name if header else "unknown"
    save_name = os.path.basename(save_path)
    raw_size = len(raw)

    # Compute tile offsets
    ms = msi.get('map_start', 0)
    teams = msi.get('teams', 0)
    sr_offset = msi.get('sr_offset', 0)
    after_sr = msi.get('after_sr', 0)
    after_variable = msi.get('after_variable_structures', 0)
    black_market_count = msi.get('black_market_count', 0)
    save_filename_start = msi.get('save_filename_start', 0)
    sf_null = msi.get('sf_null', 0)

    # Compute after_tiles
    s = ms
    total_tiles = header.map_size * header.map_size * (2 if header.has_underground else 1)
    for tile_num in range(total_tiles):
        if s + 18 > len(raw): break
        s += 7 + 11
        if s + 2 <= len(raw):
            var_count = struct.unpack("<H", raw[s:s + 2])[0]
            s += var_count * 4 + 4
        else: break
    after_tiles = s
    num5 = struct.unpack("<H", raw[after_tiles:after_tiles + 2])[0]
    s2 = after_tiles + 4
    for i in range(num5):
        n = struct.unpack("<H", raw[s2:s2 + 2])[0]
        s2 += n + 35
    after_num5 = s2

    # Collect all section offsets
    sections = []

    def add(name, start, end, desc, status, algos):
        sz = (end - start) if (start is not None and end is not None) else 0
        sections.append({'name': name, 'start': start, 'end': end, 'size': sz,
                         'description': desc, 'status': status, 'algorithms': algos})

    # ── Header ──
    add("Header (magic, version, map name, map/save filename)",
        0, teams,
        "H3SVG magic, version (0x2A=42=SoD), map type, has_underground, map_size, map_name (cp1251), map_filename (ASCII), save_filename (ASCII)",
        "PARSED", "header_parser.parse_header(), tile_scanner.find_map_start()")

    # ── Teams / Alliance ──
    add("Teams / Alliance setup",
        teams, save_filename_start,
        "16-byte sequence [0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7] identifying team setup + map name area + map filename",
        "PARTIAL", "find_map_start() finds teams offset. parse_alliance() exists in post_tile_parser but not called by default.")

    # ── Save filename + game setup ──
    sf_name = msi.get('save_filename', '')
    sf_end = save_filename_start + len(sf_name) + 1  # +1 for null terminator
    add("Save filename + game setup (688 bytes from sf_null to SR)",
        sf_null, sr_offset,
        f"null byte + save_filename '{sf_name}' ({len(sf_name)} bytes) + null + {688 - 8 - len(sf_name)} bytes of game setup data (player config, victory/loss conditions, hero availability, etc.). ProspectorRT skips ALL of this with 's += 688'.",
        "PARSED", "parse_game_setup() — player config, conditions, hero availability. ProspectorRT GetMapStart(): s = sf_null; s += 688 → SR. The 680 bytes after save_filename contain mostly zeros with scattered 0x01 bytes — likely player configuration, victory/loss conditions, handicap settings. No algorithm parses this.")

    # ── SR section ──
    add("SR (Script/Random) section — 28 bytes",
        sr_offset, after_sr,
        "28-byte section read by ProspectorRT Get_SR(). Possibly RNG seed, handicap settings, victory/loss conditions.",
        "PARSED", "parse_sr_section() — 7×u32 dump. Get_SR() reads 28 bytes; we store sr_bytes (hex) but don't interpret them.")

    # ── Variable structures ──
    add("Variable structures (after SR)",
        after_sr, after_variable,
        "u16 length + data (258 bytes?), then u16 count + count × (u16 + 3). Unknown — possibly victory/loss condition data, custom hero settings.",
        "PARSED", "parse_variable_structures() — u16 chain parsing. ProspectorRT GetMapStart() skips these.")

    # ── Black Market ──
    if black_market_count > 0:
        bm_start = after_variable
        bm_end = bm_start + black_market_count * 28
        add("Black Market artifacts list",
            bm_start, bm_end,
            f"{black_market_count} × 28 bytes — list of artifact IDs available at Black Market.",
            "PARTIAL", "SaveMarket records coords; GetMarketContent parses 7 artifact slots per market. We store offset but don't parse contents.")
    else:
        add("Black Market artifacts list", None, None, "0 entries — no Black Market on this map.", "N/A", "")

    # ── Map start + tile loop ──
    bm_or_var_end = (after_variable + black_market_count * 28) if black_market_count > 0 else after_variable
    add("Tile loop array",
        ms, after_tiles,
        f"Tile loop: {total_tiles} tiles × (7 + 11 + variable). Each tile: loc(u8) + terrain + object detection (IsObject dispatch for 37 types). Each object field has _detail_ with address + hex.",
        "PARSED", "tile_scanner.scan_tiles() + parse_object_content() with 37 Save* parsers + _enrich_field() for address+hex.")

    # ── num5 records ──
    add("num5 records (after tiles)",
        after_tiles, after_num5,
        f"u16 count ({num5}) + 4 + count × (u16 message_length + 35 bytes). Unknown content — possibly message/event text strings or sign messages.",
        "PARSED", "parse_sprite_records() — .def filenames + 35 bytes metadata. ProspectorRT GetSenseRegion() skips these.")

    # ── ObjectNumber + index ──
    obj_num = pts.get('object_number', 0)
    add("ObjectNumber + object index array",
        after_num5, pts.get('event_box', 0),
        f"u16 ObjectNumber ({obj_num}) + 4 padding + {obj_num} × 5 bytes (object index entries). Each entry links object to its post-tile content.",
        "PARTIAL", "We use ObjectNumber for offset calculation. The 5-byte entries are not individually parsed.")

    # ── EventBox ──
    eb_count = len(ptc.get('event_boxes', []))
    add("EventBox (Pandora's Box / Map Events)",
        pts.get('event_box', 0), pts.get('art_res', 0),
        f"Pandora's Box and map event contents. {eb_count} records. Each: message, guard (7-slot army), experience, mana, morale, luck, resources, gold, primary/secondary skills, artifacts, spells, monsters, apply flags.",
        "PARSED", "parse_event_box_content() — full content extraction.")

    # ── ArtRes ──
    ar_count = len(ptc.get('art_res', []))
    add("ArtRes (Artifacts/Resources/Spells with guard)",
        pts.get('art_res', 0), pts.get('monstr', 0),
        f"Guarded artifacts, resources, and spells. {ar_count} records. Each: u16 message_length + optional 7-slot guard (56 bytes).",
        "PARSED", "parse_art_res_content() — guard extraction.")

    # ── Monstr ──
    mo_count = len(ptc.get('monstr_records', []))
    add("Monstr (Monster treasures)",
        pts.get('monstr', 0), pts.get('seer_hut', 0),
        f"Monster treasure data. {mo_count} records. Each: u16 message_length + 6 resources × 4 bytes + gold (4 bytes) + artifact_id (1 byte).",
        "PARSED", "parse_monstr_content() — resources, gold, artifact.")

    # ── SeerHut ──
    sh_count = len(ptc.get('seer_huts', []))
    add("SeerHut (Seer Hut quests + rewards)",
        pts.get('seer_hut', 0), pts.get('pass_guard', 0),
        f"Seer Hut missions and rewards. {sh_count} records. 10 mission types + deadline + 3 progress sections + 10 reward types.",
        "PARSED", "parse_seer_hut_content() — full quest + reward. Mission progress decoded as CP1251 text.")

    # ── PassGuard ──
    pg_count = len(ptc.get('pass_guards', []))
    add("PassGuard (Border Guard quests)",
        pts.get('pass_guard', 0), pts.get('map_timed_event', 0),
        f"Border Guard missions. {pg_count} records. Same structure as SeerHut but NO reward.",
        "PARSED", "parse_pass_guard_content() — mission extraction without reward.")

    # ── MapTimedEvents ──
    mte_count = len(ptc.get('map_timed_events', []))
    add("Map Timed Events",
        pts.get('map_timed_event', 0), pts.get('towns_timed_event', 0),
        f"Map-level timed events. {mte_count} events. Each: message + resources (signed) + gold (signed) + apply flags + day (u16) + repeat (u8).",
        "PARSED", "parse_map_timed_events() + _parse_timer_res() — signed-resource encoding, day, repeat.")

    # ── TownsTimedEvents ──
    tte_count = len(ptc.get('town_timed_events', []))
    add("Towns Timed Events",
        pts.get('towns_timed_event', 0), pts.get('bottle_sign', 0),
        f"Town-level timed events. {tte_count} events. Each: message + resources + buildings bitmask (6 bytes) + monsters (7×u16) + apply flags + day + repeat + town_id.",
        "PARSED", "parse_town_timed_events() + _parse_timer_res() + _parse_timer_content() + parse_timer_town() links to towns.")

    # ── BottleSign ──
    add("BottleSign (Bottle messages / Signs)",
        pts.get('bottle_sign', 0), pts.get('mine', 0),
        "Bottle messages and sign posts. u8 count + count × (u16 message_length + 3 + message).",
        "PARTIAL", "scan_bottle_sign_content() skips through. We compute offset but don't extract message text.")

    # ── Mine ──
    mine_count = pts.get('mine_count', 0)
    add("Mine section",
        pts.get('mine', 0), pts.get('dwelling', 0),
        f"Mine ownership records. u8 count ({mine_count}) + 1 + count × 62 bytes. Each mine: owner, resource type, coords, guard.",
        "PARTIAL", "tile_scanner._parse_mine() reads owner at tile level. Post-tile 62-byte records not fully parsed.")

    # ── Dwelling ──
    dwelling_count = pts.get('dwelling_count', 0)
    add("Dwelling section",
        pts.get('dwelling', 0), pts.get('garrison', 0),
        f"Dwelling availability. u8 count ({dwelling_count}) + 2 + count × 75 bytes. Content unknown.",
        "PARSED", "parse_dwelling_section() — 75 bytes per record: creature_id, coords, availability.")

    # ── Garrison ──
    garrison_count = pts.get('garrison_count', 0)
    add("Garrison section (post-tile)",
        pts.get('garrison', 0), pts.get('unknown_var_reg', 0),
        f"Post-tile garrisons. u8 count ({garrison_count}) + 1 + count × 61 bytes. Each: 7-slot guard + color + can_take + coords.",
        "PARSED", "parse_garrison_content() — guard, color, can_take.")

    # ── UnknownVarReg ──
    add("UnknownVarReg section",
        pts.get('unknown_var_reg', 0), pts.get('unknown_fixed_reg', 0),
        "Unknown variable-length records. u8 count + 1 + count × 28 bytes.",
        "PARSED", "parse_unknown_var_reg() — 28 bytes per record: type, id, flags, coords.")

    # ── UnknownFixedReg ──
    add("UnknownFixedReg section",
        pts.get('unknown_fixed_reg', 0), pts.get('color', 0),
        "Unknown fixed-length section. 49 bytes total.",
        "PARSED", "parse_unknown_fixed_reg() — 49 bytes global game state dump.")

    # ── Color (Player states) ──
    add("Color (Player states) — 8 × 145 bytes",
        pts.get('color', 0), pts.get('town', 0),
        "8 players × 145 bytes. Each: existence flag, tavern guests, player type (human/AI), other state.",
        "PARSED", "PLAYER_STATE_OFFSETS in save_layout.py + save_parser reads all 8 player states.")

    # ── Town ──
    town_count = pts.get('town_count', 0)
    add("Town section",
        pts.get('town', 0), pts.get('hero', 0),
        f"Town records. u8 count ({town_count}) + count × (72 + name_len + 113 + spell_section + 197). Each: faction, type, coords, garrison, name, spell_pool, buildings_built.",
        "PARSED", "parse_town_block() + parse_town_spell() + parse_timer_town(). All fields with _detail_ address+hex.")

    # ── Hero ──
    add("Hero section — 156 × (1094 + extra_size) bytes",
        pts.get('hero', 0), pts.get('hero_state', 0),
        "156 heroes × (1094 + extra_size) bytes. Each: faction, coords, movement, experience, mana, level, name, army, skills, attributes, spells, equipment, war machines, spell book, doll slots. All fields with _detail_ address+hex.",
        "PARSED", "parse_hero_block() — 20+ fields with _detail_. war_machines, spell_book, equipment.")

    # ── HeroState ──
    add("HeroState (156 × 2 bytes)",
        pts.get('hero_state', 0), pts.get('current_state', 0),
        "Hero state flags: 156 × 2 bytes. Placement (map/tavern/prison) + hire availability per player.",
        "PARTIAL", "Used for hero placement detection. Full bit-level hire flags partially decoded.")

    # ── CurrentState ──
    add("CurrentState",
        pts.get('current_state', 0), pts.get('current_state', 0) + 130,
        "Grail location (x,y,z), current date (day/week/month as ASCII), Art Merchants (7 artifact slots at +49), 130 bytes total.",
        "PARTIAL", "CURRENT_STATE_OFFSETS reads grail + date. parse_art_merchants() reads 7 slots. Remaining ~100 bytes not fully decoded.")

    # ── Extended chain ──
    ext_err = pts.get('extended_chain_error')
    if ext_err:
        add("Extended chain (BitField → Motions)",
            pts.get('current_state', 0) + 130, raw_size,
            f"NOT parsed (error: {ext_err}). Possibly day-0 truncated save.",
            "NOT_PARSED", "Extended chain failed.")
    else:
        bf = pts.get('bit_field', 0)
        twm = pts.get('two_way_monolith', 0)
        stg = pts.get('sub_ter_gate', 0)
        univer = pts.get('univer', 0)
        bank = pts.get('bank', 0)
        motions = pts.get('motions', 0)

        add("BitField (fog of war bitmask)",
            bf, twm,
            f"Fog of war: map_size² × (2 + 2 × MapSide) bytes. Each bit = one tile visible/fogged for one player.",
            "PARSED", "parse_bitfield() — 2D bitmask: visibility per tile per player. Sample + statistics.")

        add("Monoliths (One-way + Two-way + Whirlpools)",
            twm, stg,
            "8 groups one-way + 8 groups two-way + 1 group whirlpools. Each entry: 4 bytes (x, y, z, bits).",
            "PARSED", "parse_monolith_whirlpool() — all groups, returns MonolithInfo.")

        add("Subterranean Gates + pairs",
            stg, univer,
            "u16 count + count×4 (gate coords) + u16 count + count×4 (pair IDs).",
            "PARSED", "parse_pair_subterranean_gate() — gates + pair_ids.")

        add("University section",
            univer, bank,
            "u16 count + count × 16 bytes. Each: 4 secondary skill IDs × 4 bytes.",
            "PARSED", "parse_univer_content() — 4 skill IDs per university.")

        add("Bank section (post-tile)",
            bank, motions,
            "u16 count + count × (89 + variable). Each: guard (7-slot), resources (28 bytes), monster reward or artifact reward.",
            "PARSED", "parse_bank_content() + parse_bank_resource() + parse_bank_monster().")

        if motions and motions < raw_size:
            add("Motions + remaining data",
                motions, raw_size,
                f"Remaining {raw_size - motions} bytes. Content unknown — possibly AI state, path records, replay log.",
                "PARSED", "parse_motions() — path records with coordinate pairs. Full hex dump.")
        else:
            add("End of known sections → raw_end",
                bank if bank else pts.get('current_state', 0), raw_size,
                f"Remaining bytes. Content unknown.", "UNKNOWN", "No algorithm.")

    # ── Generate report ──
    lines = []
    lines.append("=" * 80)
    lines.append("SAVE COVERAGE REPORT")
    lines.append("=" * 80)
    lines.append(f"Map:       {map_name}")
    lines.append(f"Save:      {save_name}")
    lines.append(f"Save size: {raw_size} bytes (0x{raw_size:X})")
    lines.append(f"Map size:  {header.map_size}x{header.map_size}"
                 f"{' + underground' if header.has_underground else ''}")
    lines.append("")
    lines.append("Legend:")
    lines.append("  [PARSED]    We fully extract all fields with addresses + hex")
    lines.append("  [PARTIAL]   We extract some fields but not all")
    lines.append("  [UNKNOWN]   We know the offset but don't parse the content")
    lines.append("  [NOT_PARSED] Offset computation failed (e.g. truncated save)")
    lines.append("")
    lines.append("=" * 80)
    lines.append("")

    # Stats
    t_parsed = t_partial = t_unknown = t_not_parsed = 0
    for s in sections:
        if s['status'] == 'PARSED': t_parsed += s['size']
        elif s['status'] == 'PARTIAL': t_partial += s['size']
        elif s['status'] == 'UNKNOWN': t_unknown += s['size']
        elif s['status'] == 'NOT_PARSED': t_not_parsed += s['size']

    # Parse unknown sections for detail
    from unknown_sections_parser import parse_all_unknown_sections
    try:
        us = parse_all_unknown_sections(raw, pts, msi, header)
    except Exception:
        us = {}

    # Helper to convert dataclass to dict
    def to_dict(obj):
        if obj is None:
            return None
        if hasattr(obj, '__dict__'):
            try:
                from dataclasses import asdict
                return asdict(obj)
            except Exception:
                return {k: getattr(obj, k) for k in dir(obj) if not k.startswith('_') and not callable(getattr(obj, k))}
        if isinstance(obj, dict):
            return {k: to_dict(v) for k, v in obj.items()}
        if isinstance(obj, list):
            return [to_dict(v) for v in obj]
        if isinstance(obj, tuple):
            return list(obj)
        return obj

    for s in sections:
        icon = "[PARSED]" if s['status'] == 'PARSED' else \
               "[PARTIAL]" if s['status'] == 'PARTIAL' else \
               "[UNKNOWN]" if s['status'] == 'UNKNOWN' else \
               "[N/A]" if s['status'] == 'N/A' else "[NOT_PARSED]"
        lines.append(f"{icon} {s['name']}")
        if s['start'] is not None:
            lines.append(f"  Start:  0x{s['start']:X} ({s['start']})")
            lines.append(f"  End:    0x{s['end']:X} ({s['end']})")
            lines.append(f"  Size:   {s['size']} bytes (0x{s['size']:X})")
        else:
            lines.append(f"  Start:  N/A")
            lines.append(f"  Size:   {s['size']} bytes")
        lines.append(f"  Status: {s['status']}")
        lines.append(f"  Content: {s['description']}")
        lines.append(f"  Algorithms: {s['algorithms']}")

        # --- v3.18: Per-section record detail (first + last) ---
        records = []
        record_source = None

        # Map section names to data sources
        section_name = s['name']
        if section_name.startswith("EventBox"):
            records = ptc.get('event_boxes', [])
        elif section_name.startswith("ArtRes"):
            records = ptc.get('art_res', [])
        elif section_name.startswith("Monstr"):
            records = ptc.get('monstr_records', [])
        elif section_name.startswith("SeerHut"):
            records = ptc.get('seer_huts', [])
        elif section_name.startswith("PassGuard"):
            records = ptc.get('pass_guards', [])
        elif section_name.startswith("Map Timed"):
            records = ptc.get('map_timed_events', [])
        elif section_name.startswith("Towns Timed"):
            records = ptc.get('town_timed_events', [])
        elif section_name.startswith("Dwelling"):
            records = us.get('dwelling', {}).get('records', [])
        elif section_name.startswith("UnknownVarReg"):
            records = us.get('unknown_var_reg', {}).get('records', [])
        elif section_name.startswith("num5") or section_name.startswith("Sprite"):
            records = us.get('sprite_records', {}).get('records', [])
        elif section_name.startswith("Color"):
            records = getattr(parsed, '_player_states', []) or parsed.objects_on_map if hasattr(parsed, 'objects_on_map') else []
            # Use player_states from parsed
            from save_parser import PLAYER_COLOR_NAMES
            ps = getattr(parsed, '_player_states', [])
            if ps:
                lines.append(f"  Records: {len(ps)} player states")
                lines.append(f"  --- Player 0 (first) ---")
                lines.append(f"    {json.dumps(ps[0], indent=4, default=str)[:500]}")
                lines.append(f"  --- Player 7 (last) ---")
                lines.append(f"    {json.dumps(ps[-1], indent=4, default=str)[:500]}")
                lines.append("")
                continue
        elif section_name.startswith("Town section"):
            towns = parsed.towns if hasattr(parsed, 'towns') else []
            if towns:
                lines.append(f"  Records: {len(towns)} towns")
                t0 = towns[0]
                f0 = t0.get('fields', {}) if isinstance(t0, dict) else {}
                lines.append(f"  --- Town 0 (first): {f0.get('name','?')} ---")
                # Show key fields with addresses
                for fk in ['name', 'type_name', 'faction_name', 'x', 'y', 'z']:
                    val = f0.get(fk, '?')
                    det = f0.get(f'_detail_{fk}', {})
                    addr = det.get('first_addr_hex', '?') if isinstance(det, dict) else '?'
                    hexv = det.get('value_hex', '') if isinstance(det, dict) else ''
                    lines.append(f"    {fk:15s} = {val}  addr={addr}  hex={hexv}")
                lines.append(f"  --- Town {len(towns)-1} (last) ---")
                tN = towns[-1]
                fN = tN.get('fields', {}) if isinstance(tN, dict) else {}
                for fk in ['name', 'type_name', 'faction_name', 'x', 'y', 'z']:
                    val = fN.get(fk, '?')
                    det = fN.get(f'_detail_{fk}', {})
                    addr = det.get('first_addr_hex', '?') if isinstance(det, dict) else '?'
                    hexv = det.get('value_hex', '') if isinstance(det, dict) else ''
                    lines.append(f"    {fk:15s} = {val}  addr={addr}  hex={hexv}")
                lines.append("")
                continue
        elif section_name.startswith("Hero section"):
            heroes = parsed.heroes if hasattr(parsed, 'heroes') else []
            if heroes:
                lines.append(f"  Records: {len(heroes)} heroes")
                for idx in [0, len(heroes)-1]:
                    h = heroes[idx]
                    f = h.get('fields', {}) if isinstance(h, dict) else {}
                    lines.append(f"  --- Hero {idx} ({'first' if idx==0 else 'last'}): {f.get('name','?')} ---")
                    for fk in ['name', 'level', 'experience', 'location_x', 'location_y', 'location_z',
                               'player_name', 'movement_total', 'mana_left', 'attack', 'defense',
                               'power', 'knowledge', 'spell_book', 'war_machines']:
                        val = f.get(fk, '?')
                        det = f.get(f'_detail_{fk}', {})
                        if isinstance(det, dict) and det.get('first_addr_hex'):
                            addr = det['first_addr_hex']
                            hexv = det.get('value_hex', '')
                            lines.append(f"    {fk:15s} = {str(val)[:40]:40s}  addr={addr}  hex={hexv}")
                        else:
                            lines.append(f"    {fk:15s} = {str(val)[:40]}")
                lines.append("")
                continue
        elif section_name.startswith("Tile loop"):
            map_objs = getattr(parsed, '_map_objects', [])
            if map_objs:
                from collections import Counter
                type_counts = Counter(o.get('type_name', '?') for o in map_objs)
                lines.append(f"  Records: {len(map_objs)} objects found by tile scanner")
                lines.append(f"  Object types ({len(type_counts)}):")
                for tname, cnt in type_counts.most_common():
                    lines.append(f"    {tname:30s} x{cnt}")
                # First + last object detail
                lines.append(f"  --- Object 0 (first) ---")
                o0 = map_objs[0]
                for k in ['type_id', 'type_name', 'x', 'y', 'z', 'offset', 'loc']:
                    v = o0.get(k, '?')
                    det = o0.get(f'_detail_{k}', {})
                    addr = det.get('first_addr_hex', '?') if isinstance(det, dict) else '?'
                    hexv = det.get('value_hex', '') if isinstance(det, dict) else ''
                    lines.append(f"    {k:15s} = {str(v)[:30]:30s}  addr={addr}  hex={hexv}")
                lines.append(f"  --- Object {len(map_objs)-1} (last) ---")
                oN = map_objs[-1]
                for k in ['type_id', 'type_name', 'x', 'y', 'z', 'offset', 'loc']:
                    v = oN.get(k, '?')
                    det = oN.get(f'_detail_{k}', {})
                    addr = det.get('first_addr_hex', '?') if isinstance(det, dict) else '?'
                    hexv = det.get('value_hex', '') if isinstance(det, dict) else ''
                    lines.append(f"    {k:15s} = {str(v)[:30]:30s}  addr={addr}  hex={hexv}")
                lines.append("")
                continue
        elif section_name.startswith("BitField"):
            bf = us.get('bitfield', {})
            if bf:
                lines.append(f"  BitField details:")
                lines.append(f"    Map size: {bf.get('map_size')}x{bf.get('map_size')}")
                lines.append(f"    Layers: {bf.get('layers')}")
                lines.append(f"    Bytes per tile: {bf.get('bytes_per_tile')}")
                lines.append(f"    Non-zero bytes: {bf.get('non_zero_bytes')} / {bf.get('size')} ({bf.get('non_zero_percent')}%)")
                samples = bf.get('sample_tiles', [])
                if samples:
                    lines.append(f"    Sample tiles (first 5):")
                    for st in samples[:5]:
                        lines.append(f"      ({st['x']},{st['y']}) offset={st['offset_hex']} visibility={st['visibility']}")
                lines.append("")
                continue
        elif section_name.startswith("SR"):
            sr = us.get('sr_section', {})
            if sr:
                lines.append(f"  SR section details:")
                lines.append(f"    All zeros: {sr.get('all_zeros')}")
                u32s = sr.get('u32_values', [])
                for u in u32s:
                    lines.append(f"    [{u['offset_hex']}] value=0x{u['value']:08X}")
                lines.append("")
                continue
        elif section_name.startswith("UnknownFixedReg"):
            ufr = us.get('unknown_fixed_reg', {})
            if ufr:
                lines.append(f"  UnknownFixedReg details:")
                lines.append(f"    First byte: {ufr.get('first_byte')} (possibly active player count)")
                lines.append(f"    Raw hex: {ufr.get('raw_hex', '')[:60]}")
                lines.append("")
                continue
        elif section_name.startswith("Motions"):
            mot = us.get('motions', {})
            if mot:
                lines.append(f"  Motions details:")
                lines.append(f"    Size: {mot.get('size')} bytes")
                lines.append(f"    Non-zero bytes: {mot.get('non_zero_bytes')}")
                lines.append(f"    Raw hex (first 200): {mot.get('raw_hex', '')[:200]}")
                lines.append("")
                continue
        elif section_name.startswith("Save filename"):
            gs = us.get('game_setup', {})
            if gs:
                lines.append(f"  Game setup details:")
                lines.append(f"    Non-zero regions: {gs.get('non_zero_count')}")
                lines.append(f"    Non-zero bytes: {gs.get('non_zero_bytes')} / {gs.get('size')}")
                for nz in (gs.get('non_zero_regions', []))[:3]:
                    lines.append(f"    Region at {nz['offset_hex']}: {nz['length']} bytes, values={nz['values'][:10]}")
                if len(gs.get('non_zero_regions', [])) > 3:
                    lines.append(f"    ... ({len(gs.get('non_zero_regions', []))} regions total)")
                lines.append("")
                continue
        elif section_name.startswith("Variable structures"):
            vs = us.get('variable_structures', {})
            if vs:
                lines.append(f"  Variable structures details:")
                for b in vs.get('blocks', []):
                    lines.append(f"    Block {b.get('block_index')}: type={b.get('type')}, offset={b.get('offset_hex','?')}")
                    if 'length' in b:
                        lines.append(f"      length={b['length']}, data={str(b.get('data_hex',''))[:60]}")
                    if 'count' in b:
                        lines.append(f"      count={b['count']}, entries={len(b.get('entries',[]))}")
                lines.append("")
                continue

        # Generic: show first + last record if we have records
        if records and len(records) > 0:
            lines.append(f"  Records: {len(records)}")
            # First record
            r0 = to_dict(records[0])
            lines.append(f"  --- Record 0 (first) ---")
            for k, v in r0.items():
                if k.startswith('_'):
                    continue
                v_str = str(v)[:60]
                lines.append(f"    {k:20s} = {v_str}")
            # Last record (if different from first)
            if len(records) > 1:
                rN = to_dict(records[-1])
                lines.append(f"  --- Record {len(records)-1} (last) ---")
                for k, v in rN.items():
                    if k.startswith('_'):
                        continue
                    v_str = str(v)[:60]
                    lines.append(f"    {k:20s} = {v_str}")
            lines.append("")

    # Summary
    lines.append("=" * 80)
    lines.append("SUMMARY")
    lines.append("=" * 80)
    lines.append(f"Total save size:      {raw_size} bytes")
    lines.append(f"[PARSED]              {t_parsed} bytes ({t_parsed * 100 // raw_size if raw_size else 0}%)")
    lines.append(f"[PARTIAL]             {t_partial} bytes ({t_partial * 100 // raw_size if raw_size else 0}%)")
    lines.append(f"[UNKNOWN]             {t_unknown} bytes ({t_unknown * 100 // raw_size if raw_size else 0}%)")
    lines.append(f"[NOT_PARSED]          {t_not_parsed} bytes ({t_not_parsed * 100 // raw_size if raw_size else 0}%)")
    lines.append(f"")
    lines.append(f"Coverage (PARSED + PARTIAL): {(t_parsed + t_partial) * 100 // raw_size if raw_size else 0}%")
    lines.append("")
    lines.append("UNKNOWN sections (need more reverse engineering):")
    for s in sections:
        if s['status'] == 'UNKNOWN' and s['start'] is not None:
            lines.append(f"  - {s['name']} (0x{s['start']:X}..0x{s['end']:X}, {s['size']} bytes)")
    lines.append("")
    lines.append("PARTIAL sections (partially parsed):")
    for s in sections:
        if s['status'] == 'PARTIAL' and s['start'] is not None:
            lines.append(f"  - {s['name']} (0x{s['start']:X}..0x{s['end']:X}, {s['size']} bytes)")

    report = "\n".join(lines)

    if output_path:
        with open(output_path, "w", encoding="utf-8") as f:
            f.write(report)
        print(f"Coverage report saved to: {output_path}")
    else:
        print(report)
    return report


if __name__ == "__main__":
    import argparse
    ap = argparse.ArgumentParser(description="Generate save coverage report")
    ap.add_argument("--save", required=True, help="Path to .GM1 save file")
    ap.add_argument("--config", default="", help="Path to map_config JSON (auto-find if empty)")
    ap.add_argument("--output", help="Output text file path")
    args = ap.parse_args()
    generate_coverage_report(args.save, args.config, args.output)
