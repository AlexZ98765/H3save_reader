#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
save_parser.py — Phase 3: parse ANY save of a given map using a MapConfig
(produced by Phase 2 = map_config_builder.py).

THREE-PHASE ARCHITECTURE (see README.md):
  Phase 1: Map JSON  -> MapData            (map_json_loader.py)
  Phase 2: Day-0 save + MapData -> MapConfig  (map_config_builder.py)
  Phase 3: Any save + MapConfig -> ParsedSave  (THIS module)

This module provides:

  `parse_save(raw, config, map_data)` -> ParsedSave

The ParsedSave contains:
  - HeaderInfo (parsed by header_parser)
  - List[ParsedBlock] — header + per-object data sections with their clusters
  - List[HeroBlockInfo] — heroes with parsed fields
  - List[TownBlockInfo] — towns with parsed fields
  - List[object offsets] — per-object offsets across clusters (from config)
  - Path records (variable, replay log)

The KEY idea: all absolute offsets come from `config.clusters`, `config.hero_section`,
`config.town_section`, `config.object_offsets`. We NEVER hardcode `0x157F1E`,
`0x120C8C`, `0x118C1C` etc.

ADAPTIVE OFFSETS (between day-0 and current save):
  In Phase 2 we scanned the day-0 save. In Phase 3 we open a DIFFERENT save of the
  same map. The absolute offsets of the main object array, visiting array, hero
  blocks, town records may shift because:
    - Path records block (replay log) grows as the player takes actions
    - Fog-of-war data grows as the player discovers the map
    - Visiting-objects array grows as heroes visit objects

  To adapt: we re-find the FIRST hero block by name (heroes have stable names
  from the pool) and the FIRST town block by name (towns have stable names from
  the .h3m). The delta between the day-0 offset and the current save offset
  gives us the shift of the hero/town sections. The main object array shift
  is computed similarly by re-running cluster_finder on the current save.
"""

from __future__ import annotations
import struct
from typing import Any, Dict, List, Optional, Tuple
from collections import defaultdict

from save_layout import (
    HeaderInfo, ObjectCluster, HeroBlockInfo, HeroSection,
    TownBlockInfo, TownSection, ObjectOffsets, MapConfig,
    ParsedField, ParsedBlock, ParsedSave,
    HERO_FIELD_OFFSETS, TOWN_FIELD_OFFSETS,
    HERO_BLOCK_SIZE, HERO_STRIDE_SOD,
    HERO_NAME_OFFSET_FROM_BLOCK_START,
    TOWN_NAME_OFFSET_FROM_BLOCK_START,
    TOWN_RECORD_BASE_SIZE,
    HERO_ALT_BLOCK_OFFSET, HERO_ALT_FIELD_OFFSETS,
    PLAYER_STATE_COUNT, PLAYER_STATE_SIZE, PLAYER_STATE_OFFSETS,
    CURRENT_STATE_OFFSETS, OBJECT_TYPE_IDS, TOWN_SPELL_POOL_DEPTH,
    SAVE_SECTION_ORDER,
)
from header_parser import parse_header
from cluster_finder import find_all_object_clusters
from tile_scanner import find_map_start, scan_tiles
from post_tile_scanner import walk_post_tile_sections
from post_tile_parser import parse_all_post_tile_sections


# ============================================================================
# Hero / Town field readers (use field_offsets from config or save_layout)
# ============================================================================

PLAYER_COLOR_NAMES = {
    0: "Red", 1: "Blue", 2: "Tan", 3: "Green",
    4: "Orange", 5: "Purple", 6: "Teal", 7: "Pink",
    255: "Neutral",
}

TOWN_TYPE_NAMES = {
    0: "Castle", 1: "Rampart", 2: "Tower", 3: "Inferno",
    4: "Necropolis", 5: "Dungeon", 6: "Stronghold", 7: "Fortress",
    8: "Conflux",
}


def _decode_cp1251(b: bytes) -> str:
    try:
        return b.decode("cp1251")
    except Exception:
        return b.decode("latin-1", errors="replace")


def _decode_name(name_bytes: bytes) -> str:
    s = name_bytes.rstrip(b"\x00")
    try:
        return s.decode("ascii")
    except UnicodeDecodeError:
        try:
            return s.decode("cp1251")
        except Exception:
            return s.decode("latin-1", errors="replace")


# ============================================================================
# Hero block parsing (read all known fields using HERO_FIELD_OFFSETS)
# ============================================================================

def parse_hero_block(raw: bytes, block_offset: int) -> Dict[str, Any]:
    """
    Parse a hero block at the given block_offset (faction byte = offset 0).
    Returns a dict of all known fields, using HERO_FIELD_OFFSETS.

    All offsets are RELATIVE TO NAME POSITION (h3sed convention). To get the
    actual save offset of a field, add (block_offset + 169) to the relative offset.
    """
    name_offset = block_offset + HERO_NAME_OFFSET_FROM_BLOCK_START
    o = HERO_FIELD_OFFSETS

    fields: Dict[str, Any] = {}

    # Coordinates (u16 LE each)
    try:
        fields["location_x"] = struct.unpack("<h", raw[name_offset + o["CoordinatesX"]:
                                                   name_offset + o["CoordinatesX"] + 2])[0]
        fields["location_y"] = struct.unpack("<h", raw[name_offset + o["CoordinatesY"]:
                                                   name_offset + o["CoordinatesY"] + 2])[0]
        fields["location_z"] = struct.unpack("<h", raw[name_offset + o["CoordinatesZ"]:
                                                   name_offset + o["CoordinatesZ"] + 2])[0]
    except struct.error:
        fields["location_x"] = fields["location_y"] = fields["location_z"] = 0

    # Player faction (u8)
    fields["player"] = raw[name_offset + o["Player"]]
    fields["player_name"] = PLAYER_COLOR_NAMES.get(fields["player"], f"?{fields['player']}")

    # Movement (u32 LE)
    fields["movement_total"] = struct.unpack("<I", raw[name_offset + o["MaxMovementPoints"]:
                                                    name_offset + o["MaxMovementPoints"] + 4])[0]
    fields["movement_left"] = struct.unpack("<I", raw[name_offset + o["CurrentMovementPoints"]:
                                                    name_offset + o["CurrentMovementPoints"] + 4])[0]

    # Experience (u32 LE)
    fields["experience"] = struct.unpack("<I", raw[name_offset + o["Experience"]:
                                                    name_offset + o["Experience"] + 4])[0]

    # Mana (u16 LE)
    fields["mana_left"] = struct.unpack("<H", raw[name_offset + o["ManaPoints"]:
                                                   name_offset + o["ManaPoints"] + 2])[0]

    # Level (u8)
    fields["level"] = raw[name_offset + o["HeroLevel"]]

    # Num skills (u32 LE)
    fields["num_skills"] = struct.unpack("<I", raw[name_offset + o["NumOfSkills"]:
                                                     name_offset + o["NumOfSkills"] + 4])[0]

    # Name (13 bytes, cp1251)
    fields["name"] = _decode_name(raw[name_offset:name_offset + 13])

    # Army — 7 × u32 creature IDs + 7 × u32 counts
    army_types = []
    for i in range(7):
        v = struct.unpack("<I", raw[name_offset + o["Creatures"] + i * 4:
                                     name_offset + o["Creatures"] + i * 4 + 4])[0]
        army_types.append(v if v != 0xFFFFFFFF else -1)
    fields["army_types"] = army_types

    army_counts = []
    for i in range(7):
        v = struct.unpack("<I", raw[name_offset + o["CreatureAmounts"] + i * 4:
                                     name_offset + o["CreatureAmounts"] + i * 4 + 4])[0]
        army_counts.append(v if v else 0)
    fields["army_counts"] = army_counts

    # Skills (28 bytes: skill levels + skill IDs)
    fields["skill_levels"] = list(raw[name_offset + o["Skills"]:
                                       name_offset + o["Skills"] + 28])
    fields["skill_slots"] = list(raw[name_offset + o["SkillSlots"]:
                                       name_offset + o["SkillSlots"] + 28])

    # Attributes (4 bytes: attack/defense/power/knowledge)
    fields["attack"]    = raw[name_offset + o["Attributes"]]
    fields["defense"]   = raw[name_offset + o["Attributes"] + 1]
    fields["power"]     = raw[name_offset + o["Attributes"] + 2]
    fields["knowledge"] = raw[name_offset + o["Attributes"] + 3]

    # Spells (70 bytes: spells_book + spells_available)
    fields["spells_book"]      = list(raw[name_offset + o["Spells"]:
                                            name_offset + o["Spells"] + 70])
    fields["spells_available"] = list(raw[name_offset + o["SpellBook"]:
                                            name_offset + o["SpellBook"] + 70])

    # Equipment (19 × 8-byte slots, 152 bytes total)
    equipment = []
    for i in range(19):
        slot = raw[name_offset + o["Inventory"] + i * 8:
                    name_offset + o["Inventory"] + i * 8 + 8]
        artifact_id = struct.unpack("<I", slot[:4])[0]
        data = struct.unpack("<I", slot[4:])[0]
        equipment.append((artifact_id, data))
    fields["equipment"] = equipment

    # ----- Alt block fields (from ProspectorRT GetHeroesContent) -----
    # Alt block starts at block_offset + HERO_ALT_BLOCK_OFFSET (26)
    # These fields are DUPLICATED in the alt block (also exist in pre-alt)
    # ProspectorRT reads from alt block; h3sed reads from pre-alt.
    # We read both for verification.
    alt_start = block_offset + HERO_ALT_BLOCK_OFFSET
    ao = HERO_ALT_FIELD_OFFSETS
    try:
        # Extra size (u16 LE at block_offset + 22) — determines variable part
        fields["extra_size"] = struct.unpack("<H", raw[block_offset + 22:block_offset + 24])[0]

        # Alt block fields
        fields["alt_color"] = raw[alt_start + ao["Color"]]
        fields["tree_number"] = raw[alt_start + ao["TreeNumber"]]
        fields["last_wisdom"] = raw[alt_start + ao["LastWisdom"]]
        fields["last_magic"] = raw[alt_start + ao["LastMagic"]]
        fields["alt_mp"] = struct.unpack("<H", raw[alt_start + ao["MP"]:alt_start + ao["MP"] + 2])[0]
        fields["alt_experience"] = struct.unpack("<I", raw[alt_start + ao["Experience"]:alt_start + ao["Experience"] + 4])[0]
        fields["alt_level"] = struct.unpack("<H", raw[alt_start + ao["Level"]:alt_start + ao["Level"] + 2])[0]
    except (struct.error, IndexError):
        # Alt block might be out of bounds for some heroes
        pass

    # Town spell pool depth for this hero's town (if hero is in a town)
    # Not directly applicable here, but stored for reference
    fields["spell_pool_depth_hint"] = TOWN_SPELL_POOL_DEPTH

    return fields


# ============================================================================
# Town block parsing (read all known fields using TOWN_FIELD_OFFSETS)
# ============================================================================

def parse_town_block(raw: bytes, block_offset: int) -> Dict[str, Any]:
    """
    Parse a town block at the given block_offset (faction byte = offset 0).
    Returns a dict of all known fields, using TOWN_FIELD_OFFSETS.
    """
    o = TOWN_FIELD_OFFSETS
    fields: Dict[str, Any] = {}

    fields["faction"] = raw[block_offset + o["faction"]]
    fields["faction_name"] = PLAYER_COLOR_NAMES.get(fields["faction"], f"?{fields['faction']}")

    fields["type"] = raw[block_offset + o["type"]]
    fields["type_name"] = TOWN_TYPE_NAMES.get(fields["type"], f"?{fields['type']}")

    fields["x"] = raw[block_offset + o["x"]]
    fields["y"] = raw[block_offset + o["y"]]
    fields["z"] = raw[block_offset + o["z"]]

    # Army — 7 × u32 creature IDs + 7 × u32 counts
    army_types = []
    for i in range(7):
        v = struct.unpack("<I", raw[block_offset + o["army_types"] + i * 4:
                                     block_offset + o["army_types"] + i * 4 + 4])[0]
        army_types.append(v if v != 0xFFFFFFFF else -1)
    fields["army_types"] = army_types

    army_counts = []
    for i in range(7):
        v = struct.unpack("<I", raw[block_offset + o["army_counts"] + i * 4:
                                     block_offset + o["army_counts"] + i * 4 + 4])[0]
        army_counts.append(v if v else 0)
    fields["army_counts"] = army_counts

    # Name (variable length, cp1251)
    name_len = struct.unpack("<H", raw[block_offset + o["name_len"]:
                                        block_offset + o["name_len"] + 2])[0]
    if 0 < name_len <= 14:
        name_start = block_offset + o["name"]
        fields["name"] = _decode_cp1251(raw[name_start:name_start + name_len])
    else:
        fields["name"] = "?"

    return fields


# ============================================================================
# Adaptation: re-find heroes and towns in a non-day-0 save, compute section
# shifts relative to the day-0 config.
# ============================================================================

def adapt_config_to_save(raw: bytes, config: MapConfig) -> Tuple[
    Optional[HeroSection], Optional[TownSection], Dict[int, ObjectOffsets]
]:
    """
    Re-find heroes, towns, and object clusters in the CURRENT save (which may
    differ from the day-0 save used to build the config).

    Returns:
      (hero_section, town_section, object_offsets) — all freshly computed
      for the current save.

    Algorithm:
      - For heroes: reuse map_config_builder.find_hero_blocks (regex + names)
      - For towns: reuse map_config_builder.find_town_blocks (coord-based +
        name-based, using town coords/names from config)
      - For object clusters: reuse cluster_finder.find_all_object_clusters
        (the per-object offsets may have shifted between saves)
    """
    from map_config_builder import find_hero_blocks, find_town_blocks

    header = parse_header(raw)
    scan_start = header.header_size

    # Heroes
    hero_section = find_hero_blocks(raw, scan_start, map_hero_names=None)

    # Towns — extract coords and names from config
    town_coords: List[Tuple[int, int, int]] = []
    town_names: List[str] = []
    for tb in config.town_section.blocks:
        town_coords.append((tb.x, tb.y, tb.z))
        town_names.append(tb.name)
    town_section = find_town_blocks(raw, scan_start, town_coords, town_names)

    # Object clusters
    coord_ints = set(int(ci) for ci in config.object_offsets.keys())
    clusters, object_offsets = find_all_object_clusters(
        raw, coord_ints, scan_start=scan_start)

    return hero_section, town_section, object_offsets


# ============================================================================
# Main entry: parse_save
# ============================================================================

def parse_save(raw: bytes, config: MapConfig) -> ParsedSave:
    """
    Parse a .GM1 save using a MapConfig (from Phase 2).

    Args:
        raw:    decompressed save bytes
        config: MapConfig built from day-0 save of this map

    Returns ParsedSave with header, blocks, heroes, towns, object offsets.
    """
    # ----- 1. Header -----
    header = parse_header(raw)

    blocks: List[ParsedBlock] = []

    # ----- 2. Header block -----
    header_block = ParsedBlock(
        name="header",
        start=0,
        end=header.header_size,
        description="Save header (H3SVG magic, version, map name, ...)",
    )
    header_block.fields.append(ParsedField(
        name="magic", offset=0, size=5, type="ascii",
        value=header.magic, raw_bytes=raw[:5],
        description="Magic bytes: H3SVG (scenario) or H3SVC (campaign)",
    ))
    header_block.fields.append(ParsedField(
        name="version_major", offset=0x08, size=4, type="u32",
        value=header.version_major, raw_bytes=raw[0x08:0x0C],
        description="Save version major (0x2A = 42 = SoD / HotA)",
    ))
    header_block.fields.append(ParsedField(
        name="version_minor", offset=0x0C, size=4, type="u32",
        value=header.version_minor, raw_bytes=raw[0x0C:0x10],
    ))
    header_block.fields.append(ParsedField(
        name="map_type", offset=0x30, size=4, type="u32",
        value=header.map_type, raw_bytes=raw[0x30:0x34],
        description="28 = SoD",
    ))
    header_block.fields.append(ParsedField(
        name="has_underground", offset=0x34, size=1, type="u8",
        value=int(header.has_underground), raw_bytes=raw[0x34:0x35],
    ))
    header_block.fields.append(ParsedField(
        name="map_size", offset=0x35, size=4, type="u32",
        value=header.map_size, raw_bytes=raw[0x35:0x39],
    ))
    header_block.fields.append(ParsedField(
        name="map_name", offset=0x3C, size=len(header.map_name.encode("cp1251", errors="replace")),
        type="cp1251",
        value=header.map_name,
        raw_bytes=raw[0x3C:0x3C + len(header.map_name.encode("cp1251", errors="replace"))],
    ))
    header_block.fields.append(ParsedField(
        name="map_filename", offset=0, size=len(header.map_filename),
        type="ascii", value=header.map_filename,
        raw_bytes=header.map_filename.encode("ascii", errors="replace"),
    ))
    header_block.fields.append(ParsedField(
        name="save_filename", offset=0, size=len(header.save_filename),
        type="ascii", value=header.save_filename,
        raw_bytes=header.save_filename.encode("ascii", errors="replace"),
    ))
    header_block.fields.append(ParsedField(
        name="header_size", offset=0, size=0, type="int",
        value=header.header_size, raw_bytes=b"",
        description="Byte offset where game-state sections begin",
    ))
    blocks.append(header_block)

    # ----- 3. Adapt config to current save (re-find heroes/towns/clusters) -----
    hero_section, town_section, object_offsets = adapt_config_to_save(raw, config)

    # ----- 4. Per-cluster blocks -----
    for cluster in [c for c in config.clusters]:
        cluster_block = ParsedBlock(
            name=f"cluster:{cluster.name}",
            start=cluster.start,
            end=cluster.end,
            description=f"{cluster.name} object array — {cluster.hits} hits, "
                        f"{cluster.distinct_coords} distinct coords, "
                        f"peak density {cluster.peak_density} per 4 KB",
        )
        blocks.append(cluster_block)

    # ----- 5. Hero blocks -----
    heroes_parsed: List[Dict[str, Any]] = []
    for hb in hero_section.blocks:
        try:
            fields = parse_hero_block(raw, hb.block_offset)
        except Exception as e:
            fields = {"error": str(e)}
        heroes_parsed.append({
            "block_offset": hb.block_offset,
            "block_offset_hex": hb.block_offset_hex,
            "name": hb.name,
            "player": hb.player,
            "level": hb.level,
            "is_active": hb.is_active,
            "fields": fields,
        })

    # ----- 6. Town blocks -----
    towns_parsed: List[Dict[str, Any]] = []
    for tb in town_section.blocks:
        try:
            fields = parse_town_block(raw, tb.block_offset)
            # Add spell pool depth for this town type (from ProspectorRT)
            ttype = fields.get("type", -1)
            fields["spell_pool_depth"] = TOWN_SPELL_POOL_DEPTH.get(ttype, 5)
        except Exception as e:
            fields = {"error": str(e)}
        towns_parsed.append({
            "block_offset": tb.block_offset,
            "block_offset_hex": tb.block_offset_hex,
            "name": tb.name,
            "faction": tb.faction,
            "type": tb.town_type,
            "x": tb.x, "y": tb.y, "z": tb.z,
            "fields": fields,
        })

    # ----- 6b. Player state (from ProspectorRT GetColorContent) -----
    # Player state section starts 1160 bytes before town section
    # (map.Town = map.Color + 1160 → map.Color = first_town_offset - 1160)
    player_states: List[Dict[str, Any]] = []
    if towns_parsed:
        first_town_off = towns_parsed[0]["block_offset"]
        # Town count is 2 bytes before first town; player state ends 2 bytes before town count
        color_section_start = first_town_off - 1160 - 2  # -2 for u16 town count
        if color_section_start > 0:
            ps_block = ParsedBlock(
                name="player_states",
                start=color_section_start,
                end=color_section_start + PLAYER_STATE_COUNT * PLAYER_STATE_SIZE,
                description=f"Player state section — {PLAYER_STATE_COUNT} players × "
                            f"{PLAYER_STATE_SIZE} bytes (from ProspectorRT GetColorContent)",
            )
            for pidx in range(PLAYER_STATE_COUNT):
                base = color_section_start + pidx * PLAYER_STATE_SIZE
                if base + PLAYER_STATE_SIZE > len(raw):
                    break
                ps = {
                    "player_index": pidx,
                    "offset": base,
                }
                for fname, foff in PLAYER_STATE_OFFSETS.items():
                    ps[fname] = raw[base + foff]
                # Identify human player
                ps["is_human"] = (ps.get("player_type", 0) == 3)
                ps["color_name"] = PLAYER_COLOR_NAMES.get(pidx, f"?{pidx}")
                player_states.append(ps)
            ps_block.fields = [ParsedField(
                name=f"player_{pidx}", offset=color_section_start + pidx * PLAYER_STATE_SIZE,
                size=PLAYER_STATE_SIZE, type="bytes",
                value=ps, raw_bytes=raw[color_section_start + pidx * PLAYER_STATE_SIZE:
                                        color_section_start + (pidx + 1) * PLAYER_STATE_SIZE],
                description=f"Player {pidx} ({ps.get('color_name', '?')}): "
                            f"type={'human' if ps.get('is_human') else 'AI'}, "
                            f"tavern=({ps.get('tavern_guest_1', '?')}, "
                            f"{ps.get('tavern_guest_2', '?')})",
            ) for pidx, ps in enumerate(player_states)]
            blocks.append(ps_block)

    # ----- 6c. Current state (from ProspectorRT GetCurrentState) -----
    # CurrentState = HeroState + HeroCount * 2
    # HeroState is right after hero blocks (= after last hero block + stride)
    current_state_parsed: Dict[str, Any] = {}
    if hero_section.blocks and hero_section.count > 0:
        # HeroState = after last hero block (approx: last block + stride)
        # More accurately: HeroState = first_hero_block + HeroCount * stride
        # But HeroCount may differ from actual found blocks (some may be inactive)
        # We use the ProspectorRT formula: HeroState = first_hero_block + 156 * 1094
        first_hero_off = hero_section.blocks[0].block_offset
        hero_count = hero_section.count
        hero_state_off = first_hero_off + hero_count * HERO_STRIDE_SOD
        current_state_off = hero_state_off + hero_count * 2

        if current_state_off + 50 < len(raw):
            cs = CURRENT_STATE_OFFSETS
            current_state_parsed = {
                "offset": current_state_off,
            }
            # Grail location
            current_state_parsed["grail_x"] = raw[current_state_off + cs["grail_x"]]
            current_state_parsed["grail_y"] = raw[current_state_off + cs["grail_y"]]
            current_state_parsed["grail_z"] = raw[current_state_off + cs["grail_z"]]
            current_state_parsed["has_grail"] = (current_state_parsed["grail_x"] != 0xFF)

            # Day / week / month (stored as ASCII digit bytes)
            day_byte = raw[current_state_off + cs["day"]]
            week_byte = raw[current_state_off + cs["week"]]
            month_byte = raw[current_state_off + cs["month"]]
            current_state_parsed["day"] = day_byte - 0x30 if 0x30 <= day_byte <= 0x39 else day_byte
            current_state_parsed["week"] = week_byte - 0x30 if 0x30 <= week_byte <= 0x39 else week_byte
            current_state_parsed["month"] = month_byte - 0x30 if 0x30 <= month_byte <= 0x39 else month_byte

            cs_block = ParsedBlock(
                name="current_state",
                start=current_state_off,
                end=current_state_off + 50,
                description="Current game state — day/week/month, Grail location "
                            "(from ProspectorRT GetCurrentState)",
            )
            cs_block.fields = [ParsedField(
                name=fname, offset=current_state_off + foff, size=1, type="u8",
                value=fval, raw_bytes=bytes([fval]) if isinstance(fval, int) else b"",
                description=desc,
            ) for fname, foff, fval, desc in [
                ("grail_x", cs["grail_x"], current_state_parsed["grail_x"],
                 f"Grail X (0xFF = no grail) = {current_state_parsed['grail_x']}"),
                ("grail_y", cs["grail_y"], current_state_parsed["grail_y"], "Grail Y"),
                ("grail_z", cs["grail_z"], current_state_parsed["grail_z"], "Grail Z"),
                ("day", cs["day"], current_state_parsed["day"],
                 f"Day = {current_state_parsed['day']}"),
                ("week", cs["week"], current_state_parsed["week"],
                 f"Week = {current_state_parsed['week']}"),
                ("month", cs["month"], current_state_parsed["month"],
                 f"Month = {current_state_parsed['month']}"),
            ]]
            blocks.append(cs_block)

    # ----- 6d. Map tiles (tile_scanner) — scan tiles for objects -----
    map_objects = []
    map_start_info = {}
    post_tile_sections = {}
    post_tile_content = {}
    try:
        ms, map_start_info = find_map_start(raw)
        map_objects = scan_tiles(raw, ms, header.map_size,
                                  header.has_underground)
        
        # ----- 6e. Post-tile sections (structure-walking chain) -----
        # Compute after_tiles_offset (same logic as tile_scanner internal)
        s = ms
        total_tiles = header.map_size * header.map_size * (2 if header.has_underground else 1)
        for tile_num in range(total_tiles):
            if s + 18 > len(raw): break
            s += 18
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
        obj_num = struct.unpack("<H", raw[after_num5:after_num5 + 2])[0]
        
        hero_count_for_walk = hero_section.count if hero_section else 156
        post_tile_sections = walk_post_tile_sections(
            raw, after_num5, obj_num,
            hero_count=hero_count_for_walk,
            map_size=header.map_size,
            has_underground=header.has_underground,
        )

        # ----- 6f. Post-tile content (parse_event_box, parse_seer_hut, etc.) -----
        try:
            post_tile_content = parse_all_post_tile_sections(
                raw, after_num5, obj_num,
                hero_count=hero_count_for_walk,
                map_size=header.map_size,
                has_underground=header.has_underground,
            )
        except Exception as e:
            post_tile_content = {"error": str(e)}
    except Exception as e:
        # Tile/post-tile scanning is optional — parser still works without it
        if not map_objects:
            map_start_info = {"error": str(e)}
        post_tile_sections = {"error": str(e)}
        post_tile_content = {"error": str(e)}

    # ----- 7. Build ParsedSave -----
    parsed = ParsedSave(
        header=header,
        blocks=blocks,
        heroes=heroes_parsed,
        towns=towns_parsed,
        objects_on_map=[
            {
                "coord_int": ci,
                "main_offset": off.main_offset,
                "visiting_offset": off.visiting_offset,
                "fog_offset": off.fog_offset,
                "alive_offset": off.alive_offset,
                "treasure_offset": off.treasure_offset,
                "decoration_offset": off.decoration_offset,
                "save_offsets": list(off.save_offsets),
            }
            for ci, off in object_offsets.items()
        ],
    )
    # Attach extra parsed data (not in ParsedSave dataclass yet)
    parsed._player_states = player_states
    parsed._current_state = current_state_parsed
    parsed._map_objects = map_objects
    parsed._map_start_info = map_start_info
    parsed._post_tile_sections = post_tile_sections
    parsed._post_tile_content = post_tile_content
    return parsed


# ============================================================================
# Convenience: parsed_save_to_dict (for JSON serialization)
# ============================================================================

def _serialize_post_tile_content(content):
    """Convert post_tile_content dataclasses to JSON-serializable dicts."""
    if not content or not isinstance(content, dict):
        return content
    out = {}
    for k, v in content.items():
        if k == "offsets":
            out[k] = v
        elif isinstance(v, list):
            serialized_list = []
            for item in v:
                if hasattr(item, "__dict__"):
                    # dataclass — use asdict or __dict__
                    try:
                        from dataclasses import asdict
                        serialized_list.append(asdict(item))
                    except Exception:
                        serialized_list.append(vars(item) if not isinstance(item, type) else str(item))
                else:
                    serialized_list.append(item)
            out[k] = serialized_list
        elif hasattr(v, "__dict__"):
            try:
                from dataclasses import asdict
                out[k] = asdict(v)
            except Exception:
                out[k] = str(v)
        else:
            out[k] = v
    return out


def parsed_save_to_dict(parsed: ParsedSave) -> Dict[str, Any]:
    """Convert ParsedSave to a JSON-serializable dict."""
    return {
        "header": parsed.header.to_dict(),
        "blocks": [
            {
                "name":        b.name,
                "start":       b.start,
                "end":         b.end,
                "description": b.description,
                "fields": [
                    {
                        "name":        f.name,
                        "offset":      f.offset,
                        "size":        f.size,
                        "type":        f.type,
                        "value":       f.value,
                        "description": f.description,
                    }
                    for f in b.fields
                ],
            }
            for b in parsed.blocks
        ],
        "heroes": parsed.heroes,
        "towns":  parsed.towns,
        "objects_on_map": parsed.objects_on_map,
        "player_states": getattr(parsed, "_player_states", []),
        "current_state": getattr(parsed, "_current_state", {}),
        "map_objects": getattr(parsed, "_map_objects", []),
        "map_start_info": getattr(parsed, "_map_start_info", {}),
        "post_tile_sections": getattr(parsed, "_post_tile_sections", {}),
        "post_tile_content": _serialize_post_tile_content(
            getattr(parsed, "_post_tile_content", {})),
    }


# ============================================================================
# Self-test
# ============================================================================

if __name__ == "__main__":
    import sys
    import os
    import json
    import gzip
    import io

    sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
    from map_json_loader import load_map_json_safely
    from map_config_builder import decompress_save

    cases = [
        ("save_parse_test_01",
         "/home/z/my-project/H3save_reader/examples/save_parse_test_01/save_parse_test_01.h3m.zip",
         "/home/z/my-project/H3save_reader/examples/save_parse_test_01/0000.GM1",
         "/home/z/my-project/H3save_reader/examples/save_parse_test_01/map_config_save_parse_test_01.json"),
        ("Myth and Legend",
         "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/Myth and Legend.h3m.zip",
         "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/0000.GM1",
         "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/map_config_Мифы_и_легенды.json"),
    ]

    for label, map_zip, save_path, config_path in cases:
        print(f"\n{'='*72}\n{label}\n{'='*72}")

        # Load config
        with open(config_path, "r", encoding="utf-8") as f:
            config_dict = json.load(f)
        config = MapConfig.from_dict(config_dict)
        print(f"Config: {len(config.clusters)} clusters, "
              f"{config.hero_section.count} heroes (day-0), "
              f"{config.town_section.count} towns (day-0)")

        # Load and parse save
        raw = decompress_save(save_path)
        print(f"Save decompressed: {len(raw):,} bytes")
        parsed = parse_save(raw, config)

        # Summary
        print(f"\n--- ParsedSave ---")
        print(f"  Header:")
        print(f"    magic:          {parsed.header.magic}")
        print(f"    map_name:       {parsed.header.map_name!r}")
        print(f"    map_size:       {parsed.header.map_size}")
        print(f"    header_size:    {parsed.header.header_size:#X}")
        print(f"    map_filename:   {parsed.header.map_filename!r}")
        print(f"    save_filename:  {parsed.header.save_filename!r}")
        print(f"  Blocks: {len(parsed.blocks)}")
        for b in parsed.blocks[:3]:
            print(f"    {b.name:25s}  {b.start:#X}..{b.end:#X}")
        print(f"  Heroes (parsed): {len(parsed.heroes)}")
        for h in parsed.heroes[:3]:
            f_ = h["fields"]
            print(f"    {h['block_offset_hex']}  name={f_.get('name')!r}  "
                  f"player={f_.get('player_name')}  level={f_.get('level')}  "
                  f"coords=({f_.get('location_x')},{f_.get('location_y')},{f_.get('location_z')})  "
                  f"exp={f_.get('experience')}")
        print(f"  Towns (parsed): {len(parsed.towns)}")
        for t in parsed.towns[:5]:
            f_ = t["fields"]
            print(f"    {t['block_offset_hex']}  name={f_.get('name')!r}  "
                  f"faction={f_.get('faction_name')}  type={f_.get('type_name')}  "
                  f"coords=({f_.get('x')},{f_.get('y')},{f_.get('z')})")
        print(f"  Object offsets: {len(parsed.objects_on_map)}")
        if parsed.objects_on_map:
            sample = parsed.objects_on_map[0]
            print(f"    sample: ci={sample['coord_int']}  main_offset={sample['main_offset']}")
        # Map objects from tile scanner
        map_objs = getattr(parsed, "_map_objects", [])
        if map_objs:
            from collections import Counter
            type_counts = Counter(o["type_name"] for o in map_objs)
            print(f"  Map objects (tile scanner): {len(map_objs)}")
            for tname, cnt in type_counts.most_common(5):
                print(f"    {tname:25s}: {cnt}")
        else:
            print(f"  Map objects: (none — tile scanner not available)")
        ms_info = getattr(parsed, "_map_start_info", {})
        if ms_info and "error" not in ms_info:
            print(f"  map.Start: 0x{ms_info.get('map_start', 0):X}")
