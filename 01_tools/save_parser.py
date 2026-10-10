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

    v3.12: Each field now includes full address + hex info (like .h3m.json format).
    The "fields" dict contains BOTH the simple value AND a "_detail" sub-dict
    with first_addr, first_addr_hex, length, value_type, value_hex, value_bin.
    """
    from field_formatter import (
        make_u8_field, make_u16_field, make_u32_field, make_cp1251_field,
        make_list_field, make_bytes_field, enrich_field,
    )

    name_offset = block_offset + HERO_NAME_OFFSET_FROM_BLOCK_START
    o = HERO_FIELD_OFFSETS

    fields: Dict[str, Any] = {}

    # Coordinates (i16 LE each)
    try:
        x_addr = name_offset + o["CoordinatesX"]
        x_b = raw[x_addr:x_addr + 2]
        x_val = struct.unpack("<h", x_b)[0]
        fields["location_x"] = x_val
        fields["_detail_location_x"] = enrich_field(
            "location_x", x_val, x_addr, 2, "i16", x_b)

        y_addr = name_offset + o["CoordinatesY"]
        y_b = raw[y_addr:y_addr + 2]
        y_val = struct.unpack("<h", y_b)[0]
        fields["location_y"] = y_val
        fields["_detail_location_y"] = enrich_field(
            "location_y", y_val, y_addr, 2, "i16", y_b)

        z_addr = name_offset + o["CoordinatesZ"]
        z_b = raw[z_addr:z_addr + 2]
        z_val = struct.unpack("<h", z_b)[0]
        fields["location_z"] = z_val
        fields["_detail_location_z"] = enrich_field(
            "location_z", z_val, z_addr, 2, "i16", z_b)
    except struct.error:
        fields["location_x"] = fields["location_y"] = fields["location_z"] = 0

    # Player faction (u8)
    p_addr = name_offset + o["Player"]
    fields["player"] = raw[p_addr]
    fields["player_name"] = PLAYER_COLOR_NAMES.get(fields["player"], f"?{fields['player']}")
    fields["_detail_player"] = make_u8_field("player", raw, p_addr)
    fields["_detail_player"]["value"] = fields["player_name"]  # override with human-readable

    # Movement (u32 LE)
    mt_addr = name_offset + o["MaxMovementPoints"]
    mt_b = raw[mt_addr:mt_addr + 4]
    fields["movement_total"] = struct.unpack("<I", mt_b)[0]
    fields["_detail_movement_total"] = enrich_field(
        "movement_total", fields["movement_total"], mt_addr, 4, "u32", mt_b)

    ml_addr = name_offset + o["CurrentMovementPoints"]
    ml_b = raw[ml_addr:ml_addr + 4]
    fields["movement_left"] = struct.unpack("<I", ml_b)[0]
    fields["_detail_movement_left"] = enrich_field(
        "movement_left", fields["movement_left"], ml_addr, 4, "u32", ml_b)

    # Experience (u32 LE)
    exp_addr = name_offset + o["Experience"]
    exp_b = raw[exp_addr:exp_addr + 4]
    fields["experience"] = struct.unpack("<I", exp_b)[0]
    fields["_detail_experience"] = enrich_field(
        "experience", fields["experience"], exp_addr, 4, "u32", exp_b)

    # Mana (u16 LE)
    mana_addr = name_offset + o["ManaPoints"]
    mana_b = raw[mana_addr:mana_addr + 2]
    fields["mana_left"] = struct.unpack("<H", mana_b)[0]
    fields["_detail_mana_left"] = enrich_field(
        "mana_left", fields["mana_left"], mana_addr, 2, "u16", mana_b)

    # Level (u8)
    lvl_addr = name_offset + o["HeroLevel"]
    lvl_b = raw[lvl_addr:lvl_addr + 1]
    fields["level"] = lvl_b[0] if lvl_b else 0
    fields["_detail_level"] = make_u8_field("level", raw, lvl_addr)

    # Num skills (u32 LE)
    ns_addr = name_offset + o["NumOfSkills"]
    ns_b = raw[ns_addr:ns_addr + 4]
    fields["num_skills"] = struct.unpack("<I", ns_b)[0]
    fields["_detail_num_skills"] = enrich_field(
        "num_skills", fields["num_skills"], ns_addr, 4, "u32", ns_b)

    # Name (13 bytes, cp1251)
    name_b = raw[name_offset:name_offset + 13]
    fields["name"] = _decode_name(name_b)
    fields["_detail_name"] = make_cp1251_field("name", raw, name_offset, 13)

    # Army — 7 × u32 creature IDs + 7 × u32 counts
    army_types_addr = name_offset + o["Creatures"]
    army_types_b = raw[army_types_addr:army_types_addr + 28]
    army_types = []
    for i in range(7):
        v = struct.unpack("<I", army_types_b[i * 4:(i + 1) * 4])[0]
        army_types.append(v if v != 0xFFFFFFFF else -1)
    fields["army_types"] = army_types
    fields["_detail_army_types"] = make_list_field(
        "army_types", raw, army_types_addr, 4, 7, "u32")

    army_counts_addr = name_offset + o["CreatureAmounts"]
    army_counts_b = raw[army_counts_addr:army_counts_addr + 28]
    army_counts = []
    for i in range(7):
        v = struct.unpack("<I", army_counts_b[i * 4:(i + 1) * 4])[0]
        army_counts.append(v if v else 0)
    fields["army_counts"] = army_counts
    fields["_detail_army_counts"] = make_list_field(
        "army_counts", raw, army_counts_addr, 4, 7, "u32")

    # Skills (28 bytes: skill levels + skill IDs)
    skills_addr = name_offset + o["Skills"]
    skills_b = raw[skills_addr:skills_addr + 28]
    fields["skill_levels"] = list(skills_b)
    fields["_detail_skill_levels"] = make_list_field(
        "skill_levels", raw, skills_addr, 1, 28, "u8")

    skill_slots_addr = name_offset + o["SkillSlots"]
    skill_slots_b = raw[skill_slots_addr:skill_slots_addr + 28]
    fields["skill_slots"] = list(skill_slots_b)
    fields["_detail_skill_slots"] = make_list_field(
        "skill_slots", raw, skill_slots_addr, 1, 28, "u8")

    # Attributes (4 bytes: attack/defense/power/knowledge)
    attrs_addr = name_offset + o["Attributes"]
    attrs_b = raw[attrs_addr:attrs_addr + 4]
    fields["attack"]    = attrs_b[0]
    fields["defense"]   = attrs_b[1]
    fields["power"]     = attrs_b[2]
    fields["knowledge"] = attrs_b[3]
    fields["_detail_attributes"] = make_bytes_field(
        "attributes", raw, attrs_addr, 4, "bytes_u8x4")

    # Spells (70 bytes: spells_book + spells_available)
    spells_addr = name_offset + o["Spells"]
    spells_b = raw[spells_addr:spells_addr + 70]
    fields["spells_book"] = list(spells_b)
    fields["_detail_spells_book"] = make_list_field(
        "spells_book", raw, spells_addr, 1, 70, "u8")

    spellbook_addr = name_offset + o["SpellBook"]
    spellbook_b = raw[spellbook_addr:spellbook_addr + 70]
    fields["spells_available"] = list(spellbook_b)
    fields["_detail_spells_available"] = make_list_field(
        "spells_available", raw, spellbook_addr, 1, 70, "u8")

    # Equipment (19 × 8-byte slots, 152 bytes total)
    equip_addr = name_offset + o["Inventory"]
    equipment = []
    for i in range(19):
        slot = raw[equip_addr + i * 8:equip_addr + i * 8 + 8]
        artifact_id = struct.unpack("<I", slot[:4])[0]
        data = struct.unpack("<I", slot[4:])[0]
        equipment.append((artifact_id, data))
    fields["equipment"] = equipment
    fields["_detail_equipment"] = make_bytes_field(
        "equipment", raw, equip_addr, 152, "bytes_equipment_19x8")

    # ----- v3.10: War machines + Spell Book (PRT columns "Машина" and "Книга заклинаний") -----
    # PRT reads 83 doll slots × 8 bytes from offset +561 onwards (see MainForm.cs:7723).
    # Slot index = 0 → Spell Book (artifact_id == 0)
    # Slot index 4/5/6 → War machines (Ballista=4, Ammo Cart=5, First Aid Tent=6)
    # We extract these as separate fields for clarity.
    doll_start = name_offset + 561  # 83 × 8 = 686 bytes
    war_machines = []  # list of (slot_index, artifact_id, data)
    spell_book = False
    for i in range(83):
        slot_off = doll_start + i * 8
        if slot_off + 8 > len(raw):
            break
        art_id = raw[slot_off]
        if art_id == 0:
            spell_book = True  # has spell book
        elif art_id in (4, 5, 6):
            machine_name = {4: "Ballista", 5: "Ammo Cart", 6: "First Aid Tent"}.get(art_id, f"Unknown({art_id})")
            war_machines.append({
                "slot_index": i,
                "machine_id": art_id,
                "machine_name": machine_name,
                "data": struct.unpack("<I", raw[slot_off + 4:slot_off + 8])[0],
            })
    fields["war_machines"] = war_machines
    fields["spell_book"] = spell_book

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

    # v3.12: Add block_offset detail at top level
    fields["_detail_block_offset"] = {
        "first_addr": block_offset,
        "first_addr_hex": f"0x{block_offset:x}",
        "length": 0,  # variable
        "value_type": "hero_block",
        "value": f"Hero block at 0x{block_offset:x}",
    }

    return fields


# ============================================================================
# Town block parsing (read all known fields using TOWN_FIELD_OFFSETS)
# ============================================================================

def parse_town_block(raw: bytes, block_offset: int) -> Dict[str, Any]:
    """
    Parse a town block at the given block_offset (faction byte = offset 0).
    Returns a dict of all known fields, using TOWN_FIELD_OFFSETS.

    v3.12: Each field now includes full address + hex info (like .h3m.json format).
    The "fields" dict contains BOTH the simple value AND a "_detail" sub-dict
    with first_addr, first_addr_hex, length, value_type, value_hex, value_bin.
    """
    from field_formatter import (
        make_u8_field, make_list_field, make_bytes_field,
        make_cp1251_field, enrich_field, _bytes_to_hex,
    )

    o = TOWN_FIELD_OFFSETS
    fields: Dict[str, Any] = {}

    # v3.12: Add block_offset detail at top level
    fields["_detail_block_offset"] = {
        "first_addr": block_offset,
        "first_addr_hex": f"0x{block_offset:x}",
        "length": 0,  # variable
        "value_type": "town_block",
        "value": f"Town block at 0x{block_offset:x}",
    }

    # Town ID
    fields["id"] = raw[block_offset] if block_offset < len(raw) else 0
    fields["_detail_id"] = make_u8_field("id", raw, block_offset)

    # Faction
    f_addr = block_offset + o["faction"]
    fields["faction"] = raw[f_addr]
    fields["faction_name"] = PLAYER_COLOR_NAMES.get(fields["faction"], f"?{fields['faction']}")
    fields["_detail_faction"] = make_u8_field("faction", raw, f_addr)
    fields["_detail_faction"]["value"] = fields["faction_name"]

    # Type
    t_addr = block_offset + o["type"]
    fields["type"] = raw[t_addr]
    fields["type_name"] = TOWN_TYPE_NAMES.get(fields["type"], f"?{fields['type']}")
    fields["_detail_type"] = make_u8_field("type", raw, t_addr)
    fields["_detail_type"]["value"] = fields["type_name"]

    # Coords
    x_addr = block_offset + o["x"]
    y_addr = block_offset + o["y"]
    z_addr = block_offset + o["z"]
    fields["x"] = raw[x_addr]
    fields["y"] = raw[y_addr]
    fields["z"] = raw[z_addr]
    fields["_detail_x"] = make_u8_field("x", raw, x_addr)
    fields["_detail_y"] = make_u8_field("y", raw, y_addr)
    fields["_detail_z"] = make_u8_field("z", raw, z_addr)

    # Army — 7 × u32 creature IDs + 7 × u32 counts
    army_types_addr = block_offset + o["army_types"]
    army_types = []
    for i in range(7):
        v = struct.unpack("<I", raw[army_types_addr + i * 4:
                                     army_types_addr + i * 4 + 4])[0]
        army_types.append(v if v != 0xFFFFFFFF else -1)
    fields["army_types"] = army_types
    fields["_detail_army_types"] = make_list_field(
        "army_types", raw, army_types_addr, 4, 7, "u32")

    army_counts_addr = block_offset + o["army_counts"]
    army_counts = []
    for i in range(7):
        v = struct.unpack("<I", raw[army_counts_addr + i * 4:
                                     army_counts_addr + i * 4 + 4])[0]
        army_counts.append(v if v else 0)
    fields["army_counts"] = army_counts
    fields["_detail_army_counts"] = make_list_field(
        "army_counts", raw, army_counts_addr, 4, 7, "u32")

    # Name (variable length, cp1251)
    name_len_addr = block_offset + o["name_len"]
    name_len = struct.unpack("<H", raw[name_len_addr:name_len_addr + 2])[0]
    if 0 < name_len <= 14:
        name_start = block_offset + o["name"]
        fields["name"] = _decode_cp1251(raw[name_start:name_start + name_len])
        fields["_detail_name"] = make_cp1251_field(
            "name", raw, name_start, name_len)
    else:
        fields["name"] = "?"
        fields["_detail_name"] = {
            "first_addr": name_len_addr,
            "first_addr_hex": f"0x{name_len_addr:x}",
            "length": 2,
            "value_type": "cp1251",
            "value": "?",
            "value_int": name_len,
            "value_dec": name_len,
            "value_hex": _bytes_to_hex(raw[name_len_addr:name_len_addr + 2]),
            "value_bin": "",
        }

    # ----- v3.10: Spell Pool (Magic Guild spells) -----
    # ProspectorRT GetTownContent (line 7458) reads spell_pool after each town.
    # The spell pool starts at block_offset + 72 + name_len + 113 (after town name
    # and a 113-byte gap before spell slots).
    # Town spell pool depth depends on town type (TOWN_SPELL_POOL_DEPTH).
    # We call post_tile_parser.parse_town_spell to extract spells per level.
    try:
        # Lazy import to avoid circular dependency
        import os, sys
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from post_tile_parser import parse_town_spell
        from save_layout import TOWN_SPELL_POOL_DEPTH

        # Spell section starts after town record + 113 bytes gap
        # block_offset + 72 (name_len_offset) + name_len + 113
        # but ProspectorRT calls GetTownSpell AFTER skipping town record
        # (town stride = 72 + name_len + 113 + spell_section + 197)
        # Let's compute: end of "town main body" = block_offset + 72 + name_len + 113
        spell_section_offset = block_offset + 72 + name_len + 113
        # Library check: if Tower (type=2), library built = bit 0x40 in some byte
        # For simplicity, assume library = False unless we can read it
        lvl = TOWN_SPELL_POOL_DEPTH.get(fields["type"], 5)
        # parse_town_spell expects (raw, s, lvl, library_built, mg5_built)
        # We pass library_built = False (will need to fix later if needed)
        spell_pool, _ = parse_town_spell(raw, spell_section_offset, lvl)
        # spell_pool is TownSpellPool dataclass with .levels (list of lists)
        # Convert to dict for JSON
        fields["spell_pool"] = {
            "address": spell_pool.address,
            "has_library": spell_pool.has_library,
            "has_mage_guild_level_5": spell_pool.has_mage_guild_level_5,
            "levels": spell_pool.levels,  # list of [spell_id, ...]
        }
    except Exception as e:
        # If post_tile_parser fails (e.g., out of bounds), don't break the whole town
        fields["spell_pool"] = {"error": str(e)}

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

    # v3.10: Link town timed events to towns (parse_timer_town)
    # For each town, find all timed events with matching town ID and decode
    # the 6-byte building bitmask into building names.
    try:
        import os, sys
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from post_tile_parser import parse_timer_town
        tte = post_tile_content.get("town_timed_events", []) if isinstance(post_tile_content, dict) else []
        if tte and towns_parsed:
            # Build town list for parse_timer_town (it expects dicts with 'id'/'fields' structure)
            town_dicts = [
                {
                    "id": t.get("fields", {}).get("id", 0),
                    "name": t.get("name") or t.get("fields", {}).get("name", ""),
                    "fields": t.get("fields", {}),
                }
                for t in towns_parsed
            ]
            town_links = parse_timer_town(town_dicts, tte)
            # Convert TownTimerLink dataclasses to dicts and attach to each town
            from dataclasses import asdict
            links_by_id = {link.town_id: link for link in town_links}
            for town in towns_parsed:
                town_id = town.get("fields", {}).get("id", 0)
                if town_id in links_by_id:
                    link = links_by_id[town_id]
                    town["timed_events_count"] = len(link.events)
                    town["buildings_built"] = link.buildings_built
            # Also store the links themselves
            parsed._town_timer_links = [asdict(l) for l in town_links]
    except Exception as e:
        parsed._town_timer_links = {"error": str(e)}

    # v3.10: Merge tile-scan results with post-tile content (Priority 3)
    # Produce PRT-like "raw" tables: Артефакты, Монстры, Банки, etc.
    try:
        import os, sys
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from merged_objects import merge_all
        if map_objects and isinstance(post_tile_content, dict):
            heroes_for_merge = [
                {"fields": h.get("fields", {}), "name": h.get("name")}
                for h in heroes_parsed
            ]
            parsed._merged_objects = merge_all(
                tile_objects=map_objects,
                post_tile_content=post_tile_content,
                heroes_parsed=heroes_for_merge,
            )
        else:
            parsed._merged_objects = {}
    except Exception as e:
        parsed._merged_objects = {"error": str(e)}

    # v3.10: Aggregators (Priority 4) — "Все Арты", "Все Заклы", "Все Навыки"
    # These collect artifacts/spells/skills from ALL sources on the map.
    try:
        import os, sys
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from post_tile_parser import (
            aggregate_all_artifacts,
            aggregate_all_spells,
            aggregate_all_skills,
        )
        parsed._aggregate_all_artifacts = aggregate_all_artifacts(parsed)
        parsed._aggregate_all_spells = aggregate_all_spells(parsed)
        parsed._aggregate_all_skills = aggregate_all_skills(parsed)
    except Exception as e:
        parsed._aggregate_all_artifacts = []
        parsed._aggregate_all_spells = []
        parsed._aggregate_all_skills = []
        parsed._aggregator_error = str(e)

    return parsed


# ============================================================================
# Convenience: parsed_save_to_dict (for JSON serialization)
# ============================================================================

def _serialize_post_tile_content(content):
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


def _serialize_merged_objects(merged):
    """Convert merged_objects dict (output of merged_objects.merge_all) to JSON-serializable.

    merged_objects has lists of dicts, but some dicts contain dataclass
    instances (e.g., SubTerGate from post_tile_content.sub_ter_gates.gates).
    We need to convert those to plain dicts.
    """
    if not merged or not isinstance(merged, dict):
        return merged
    if "error" in merged:
        return merged
    out = {}
    for k, v in merged.items():
        if isinstance(v, list):
            new_list = []
            for item in v:
                if isinstance(item, dict):
                    new_item = {}
                    for kk, vv in item.items():
                        # If value is a dataclass or has __dict__, convert
                        if hasattr(vv, "__dict__") and not isinstance(vv, (int, str, bool, list, dict)):
                            try:
                                from dataclasses import asdict
                                new_item[kk] = asdict(vv)
                            except Exception:
                                new_item[kk] = str(vv)
                        elif isinstance(vv, tuple):
                            new_item[kk] = list(vv)
                        else:
                            new_item[kk] = vv
                    new_list.append(new_item)
                elif hasattr(item, "__dict__"):
                    try:
                        from dataclasses import asdict
                        new_list.append(asdict(item))
                    except Exception:
                        new_list.append(str(item))
                else:
                    new_list.append(item)
            out[k] = new_list
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
        "town_timer_links": getattr(parsed, "_town_timer_links", []),
        "merged_objects": _serialize_merged_objects(
            getattr(parsed, "_merged_objects", {})),
        "aggregators": {
            "all_artifacts": getattr(parsed, "_aggregate_all_artifacts", []),
            "all_spells": getattr(parsed, "_aggregate_all_spells", []),
            "all_skills": getattr(parsed, "_aggregate_all_skills", []),
            "error": getattr(parsed, "_aggregator_error", None),
        },
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
