#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
tile_scanner.py — Universal tile scanner for .GM1 saves.

Implements the Scanner loop: iterates through map tiles, finds objects
via IsObject dispatcher, and parses object content for each type.

FIXED: Uses .h3m (always present) instead of .GM1 (may lack extension) for navigation.
Works on ANY save (not just day-0).

Three main functions:
  1. find_map_start(raw) — locate where tile data begins (GetStart + GetMapStart, fixed)
  2. scan_tiles(raw, map_start, map_size, has_underground) — iterate tiles, find objects
  3. parse_object_content(raw, s, type_id, x, y, z, loc) — parse object data per type
"""

from __future__ import annotations
import struct
from typing import Any, Dict, List, Optional, Tuple
from collections import defaultdict

from save_layout import OBJECT_TYPE_IDS


# ============================================================================
# Constants extracted from the reverse-engineered source code
# ============================================================================

# Teams signature: 00 01 02 03 04 05 06 07 00 01 02 03 04 05 06 07
TEAMS_SIGNATURE = bytes([0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7])


# ============================================================================
# 1. find_map_start — GetStart + GetMapStart (FIXED: use .h3m instead of .GM1)
# ============================================================================

def find_map_start(raw: bytes) -> Tuple[int, Dict[str, int]]:
    """
    Find where tile data begins (map.Start) in the decompressed save.

    Algorithm (adapted from GetStart + GetMapStart):
      1. Find teams signature (16 bytes: 0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7)
      2. MapName = teams + 57
      3. Find .h3m (map filename — ALWAYS present, unlike .GM1)
      4. From .h3m, walk back to null → map filename start
      5. From map filename end (null), scan forward to find save filename
      6. From save filename start + 688 = SR section
      7. +28, +u16+258, +u16+4, variable, BlackMarket, +1 = map.Start

    Returns (map_start_offset, debug_info_dict)
    """
    info = {}

    # Step 1: Find teams signature
    teams_idx = raw.find(TEAMS_SIGNATURE)
    if teams_idx < 0:
        raise ValueError("Teams signature not found")
    info['teams'] = teams_idx
    info['map_name'] = teams_idx + 57

    # Step 2: Find .h3m (map filename extension — always present)
    # Map filename is at teams + 57
    # Search for .h3m from map_name position forward (within 500 bytes)
    h3m_idx = raw.find(b".h3m", info['map_name'], info['map_name'] + 500)
    if h3m_idx < 0:
        # Try .CGM, .GM2, .GM3
        for ext in [b".CGM", b".GM2", b".GM3"]:
            h3m_idx = raw.find(ext, info['map_name'], info['map_name'] + 500)
            if h3m_idx >= 0:
                break
    if h3m_idx < 0:
        # Fallback: try .GM1 (works for day-0 saves)
        h3m_idx = raw.find(b".GM1", info['map_name'], info['map_name'] + 500)
    if h3m_idx < 0:
        raise ValueError("Map filename extension (.h3m/.GM1/.CGM) not found")
    info['ext_offset'] = h3m_idx

    # Step 3: From .h3m, walk back to find null byte → start of filename
    s = h3m_idx
    while s > 0 and raw[s] != 0:
        s -= 1
    map_filename_start = s + 1
    info['map_filename_start'] = map_filename_start
    info['map_filename'] = raw[map_filename_start:h3m_idx + 4].decode('ascii', errors='replace')

    # Step 4: Find save filename — scan forward from after map filename null
    # After map filename null, there's a path + padding + save filename
    # The save filename is the next non-null ASCII string
    save_filename_start = _find_save_filename(raw, h3m_idx + 5)  # +5 to skip ".ext\0"
    info['save_filename_start'] = save_filename_start
    # Read save filename (null-terminated)
    sf_end = save_filename_start
    while sf_end < len(raw) and raw[sf_end] != 0:
        sf_end += 1
    info['save_filename'] = raw[save_filename_start:sf_end].decode('ascii', errors='replace')

    # Step 5: GetMapStart — from save filename, navigate to map.Start
    # Algorithm: do { s--; } while (decmp[s] != 0);  → s = null byte BEFORE filename
    #               SaveName = s + 1
    #               s += 688;  → SR = null_byte + 688 (NOT SaveName + 688!)
    # We need the null byte BEFORE the save filename for +688
    sf_null = save_filename_start - 1
    # Verify: raw[sf_null] should be 0
    if sf_null >= 0 and raw[sf_null] != 0:
        # Walk back to find the actual null
        while sf_null > 0 and raw[sf_null] != 0:
            sf_null -= 1
    info['sf_null'] = sf_null
    sr_offset = sf_null + 688
    info['sr_offset'] = sr_offset

    # s = SR + 28
    s = sr_offset + 28
    info['after_sr'] = s

    # Read u16 LE (num), jump num + 258
    if s + 2 > len(raw):
        raise ValueError(f"Save too short after SR: offset {s}")
    num = struct.unpack("<H", raw[s:s + 2])[0]
    s += num + 258
    info['after_first_var'] = s

    # Read u16 LE (num2), jump 4
    if s + 4 > len(raw):
        raise ValueError(f"Save too short after first var: offset {s}")
    num2 = struct.unpack("<H", raw[s:s + 2])[0]
    s += 4
    info['after_second_var'] = s

    # If num2 != 0, iterate num2 times: s += u16 + 3
    if num2 != 0:
        for _ in range(num2):
            if s + 3 > len(raw):
                break
            n = struct.unpack("<H", raw[s:s + 2])[0]
            s += n + 3
    info['after_variable_structures'] = s

    # Read num3 = decmp[s], if > 0: BlackMarket = s, s += num3 * 28
    if s < len(raw):
        num3 = raw[s]
        info['black_market_count'] = num3
        if num3 != 0:
            s += num3 * 28
    else:
        info['black_market_count'] = 0

    # map.Start = s + 1
    map_start = s + 1
    info['map_start'] = map_start

    return map_start, info


def _find_save_filename(raw: bytes, start_search: int, max_search: int = 500) -> int:
    """
    Find the save filename (null-terminated ASCII string) after the map filename.

    After the map filename ".h3m\\0", the structure is:
    - Null padding (many bytes)
    - Path string (e.g., "maps\\all\\SoD\\XL\\0")
    - More null padding
    - Binary flags (non-ASCII bytes: 0x01, 0xFF, 0x0A, etc.)
    - Save filename (e.g., "0000.GM1\\0" or "BATTLE\\0")

    Strategy: scan forward looking for a null byte followed by >= 3
    contiguous printable ASCII bytes. That pattern marks the start of
    the save filename.
    """
    s = start_search
    end = min(start_search + max_search, len(raw))

    # Skip null bytes (padding after map filename)
    while s < end and raw[s] == 0:
        s += 1

    # Skip path string (ASCII until null)
    while s < end and raw[s] != 0:
        s += 1

    # Now scan forward through binary flags looking for:
    # null byte followed by >= 3 printable ASCII bytes
    while s < end:
        if raw[s] == 0:
            # Check if next 3 bytes are all printable ASCII
            if (s + 3 < end and
                0x30 <= raw[s + 1] <= 0x7E and  # first char (allow digits, letters, etc.)
                0x20 <= raw[s + 2] <= 0x7E and
                0x20 <= raw[s + 3] <= 0x7E):
                # Found save filename! Return position AFTER the null
                return s + 1
        s += 1

    # Fallback: search for common save filename patterns
    for cand in [b"NEWGAME", b"AUTOSAVE", b"0000", b"0001", b"BATTLE", b"114"]:
        idx = raw.find(cand, start_search, end)
        if idx >= 0:
            return idx

    return start_search + 360


# ============================================================================
# 2. scan_tiles — Scanner loop (Scanner, line 6172)
# ============================================================================

def scan_tiles(raw: bytes, map_start: int, map_size: int,
               has_underground: bool) -> List[Dict[str, Any]]:
    """
    Iterate through map tiles and find all objects.

    Algorithm (from Scanner, line 6184-6208):
      For each tile (0..total_tiles-1):
        1. Read 7 bytes (loc = first byte)
        2. Check if object present (byte at offset 7)
        3. If object found: call parse_object_content
        4. Skip 11 bytes
        5. Skip variable part: u16_count * 4 + 4

    Returns list of found objects with type, coords, and content.
    """
    objects = []
    n_tiles = map_size * map_size
    total_tiles = n_tiles * (2 if has_underground else 1)

    s = map_start
    underground_offset = 0  # l = 0 for surface, n_tiles for underground
    is_underground = False

    # Object ID dedup (from FirstID)
    seen_object_ids = set()

    for tile_num in range(total_tiles):
        if s + 18 > len(raw):
            break

        # Check for underground transition
        if has_underground and tile_num == n_tiles:
            underground_offset = n_tiles
            is_underground = True

        # Read first byte (loc = terrain type? object ID?)
        loc = raw[s]

        # Skip 7 bytes
        s += 7

        # Check for object
        obj_found = False
        obj_offset = 0

        if s < len(raw):
            check_byte = raw[s]
            check_next = raw[s + 1] if s + 1 < len(raw) else 0

            # Object conditions (from Scanner)
            if check_byte == 16 or check_byte == 18 or \
               (check_byte == 17 and raw[s - 7] == 9):
                obj_offset = s + 1
                obj_found = True
            elif check_next == 26 and (check_byte == 0 or check_byte == 2):
                obj_offset = s + 1
                obj_found = True

        # Calculate coordinates
        local_tile = tile_num - underground_offset
        y = local_tile // map_size
        x = local_tile - map_size * y
        z = 1 if is_underground else 0

        # Parse object if found
        if obj_found and obj_offset < len(raw):
            type_id = raw[obj_offset]

            # PRT IsObject: some types are processed WITHOUT FirstID dedup.
            # Type 26 (Event) — NO FirstID (process ALL event tiles)
            # Type 34 (Hero) — NO FirstID
            # All other types use FirstID(s) to skip overlay duplicates.
            types_without_firstid = {26, 34}

            if type_id in types_without_firstid:
                # No dedup — process every tile of this type
                obj = parse_object_content(raw, obj_offset, type_id, x, y, z, loc)
                if obj:
                    obj['tile_num'] = tile_num
                    obj['obj_id'] = obj_id
                    objects.append(obj)
            else:
                # FirstID dedup — skip if already seen this object ID
                obj_id = (raw[obj_offset + 5] << 8) + raw[obj_offset + 4] if obj_offset + 6 <= len(raw) else 0
                if obj_id not in seen_object_ids:
                    seen_object_ids.add(obj_id)

                    obj = parse_object_content(raw, obj_offset, type_id, x, y, z, loc)
                    if obj:
                        obj['tile_num'] = tile_num
                        obj['obj_id'] = obj_id
                        objects.append(obj)

        # Skip 11 bytes
        s += 11

        # Skip variable part: u16_count * 4 + 4
        if s + 2 <= len(raw):
            var_count = struct.unpack("<H", raw[s:s + 2])[0] if s + 2 <= len(raw) else 0
            s += var_count * 4 + 4
        else:
            break

    return objects


# ============================================================================
# v3.14: Field enrichment helpers — add _detail_ sub-dicts with address + hex
# ============================================================================

def _enrich_field(raw: bytes, obj: dict, field_name: str, addr: int,
                  length: int, value_type: str = "u8"):
    """Add a _detail_ sub-dict for a field in a map object.

    Reads raw[addr:addr+length] and stores first_addr, first_addr_hex,
    length, value_type, value, value_dec, value_hex, value_bin.
    """
    from field_formatter import make_field, _bytes_to_int_le
    b = raw[addr:addr + length] if addr + length <= len(raw) else b""
    value = obj.get(field_name)
    f = make_field(field_name, addr, length, value_type, value, b)
    obj[f"_detail_{field_name}"] = f


def _enrich_map_object_fields(raw: bytes, obj: dict):
    """Add _detail_ for base map object fields: type_id, x, y, z, offset, loc.

    The object record in the save starts at `obj['offset']`.
    Layout (from Scanner):
      [offset+0]     type_id   (u8)
      [offset+1]     ???       (u8)
      [offset+2]     subtype   (u8) — used for artifact_id, resource_type, etc.
      [offset+3..5]  ???       (3 bytes)
      [offset+4..5]  obj_id    (u16 LE)
      [offset+6]     content   (u8) — varies by type
      [offset+7]     content2  (u8) — varies by type

    v3.15: Also enriches coordinates (x, y, z) from the object's coord_int.
    Coordinates are stored in cluster arrays (main, visiting, alive, fog,
    treasure, decoration) — each as 3 bytes (x, y, z).
    """
    s = obj.get('offset', 0)
    # type_id at s+0
    _enrich_field(raw, obj, 'type_id', s, 1, "u8")
    # loc is the first byte of tile record (7 bytes before object offset)
    _enrich_field(raw, obj, 'loc', s - 7, 1, "u8")
    # offset itself
    obj['_detail_offset'] = {
        "first_addr": s,
        "first_addr_hex": f"0x{s:x}",
        "length": 0,
        "value_type": "object_offset",
        "value": f"Object record at 0x{s:x}",
    }

    # v3.15: Enrich coordinates (x, y, z) — stored as 3 bytes in cluster arrays
    x = obj.get('x', 0)
    y = obj.get('y', 0)
    z = obj.get('z', 0)
    coord_bytes = bytes([x & 0xFF, y & 0xFF, z & 0xFF])
    coord_int = x | (y << 8) | (z << 16)
    coord_value_str = f"({x},{y},{z})"

    # We don't have cluster offsets here (they're in save_parser), but we can
    # at least store the coord_int and the 3-byte representation so user
    # can search for it in the binary.
    obj['_detail_coords'] = {
        "first_addr": None,  # set later by save_parser when cluster offsets are known
        "first_addr_hex": None,
        "length": 3,
        "value_type": "coords3",
        "value": coord_value_str,
        "value_int": coord_int,
        "value_dec": coord_int,
        "value_hex": f"{x:02x} {y:02x} {z:02x}",
        "value_bin": f"{x:08b} {y:08b} {z:08b}",
        "coord_int": coord_int,
    }
    # Also enrich individual x, y, z as computed fields
    obj['_detail_x'] = {
        "first_addr": None,
        "first_addr_hex": None,
        "length": 1,
        "value_type": "u8_computed",
        "value": x,
        "value_int": x,
        "value_dec": x,
        "value_hex": f"{x:02x}",
        "value_bin": f"{x:08b}",
    }
    obj['_detail_y'] = {
        "first_addr": None,
        "first_addr_hex": None,
        "length": 1,
        "value_type": "u8_computed",
        "value": y,
        "value_int": y,
        "value_dec": y,
        "value_hex": f"{y:02x}",
        "value_bin": f"{y:08b}",
    }
    obj['_detail_z'] = {
        "first_addr": None,
        "first_addr_hex": None,
        "length": 1,
        "value_type": "u8_computed",
        "value": z,
        "value_int": z,
        "value_dec": z,
        "value_hex": f"{z:02x}",
        "value_bin": f"{z:08b}",
    }


def _enrich_typed_field(raw: bytes, obj: dict, field_name: str,
                        addr: int, length: int, value_type: str):
    """Add _detail_ for a typed field (u8, u16, u32) at a specific address."""
    _enrich_field(raw, obj, field_name, addr, length, value_type)


# ============================================================================
# 3. parse_object_content — IsObject dispatcher → Save* methods
# ============================================================================

def parse_object_content(raw: bytes, s: int, type_id: int,
                          x: int, y: int, z: int, loc: int) -> Optional[Dict[str, Any]]:
    """
    Parse object content based on type_id (from IsObject dispatcher).

    Args:
        raw:     decompressed save bytes
        s:       offset of object data (decmp[s] = type_id)
        type_id: first byte = object type
        x, y, z: tile coordinates
        loc:     first byte of tile record (terrain/ground type?)

    Returns dict with object data, or None if type is unknown/skipped.

    v3.14: Each field now includes _detail_ sub-dict with address + hex info.
    """
    obj = {
        'type_id': type_id,
        'type_name': OBJECT_TYPE_IDS.get(type_id, f'Unknown({type_id})'),
        'x': x, 'y': y, 'z': z,
        'offset': s,
        'loc': loc,
    }

    # v3.14: Add _detail_ for the base fields (type_id, x, y, z, offset, loc)
    _enrich_map_object_fields(raw, obj)

    # Dispatch based on type_id (from IsObject, line 9484)
    if type_id == 5:
        _parse_artifact(raw, s, obj)
    elif type_id == 79:
        _parse_resource(raw, s, obj)
    elif type_id == 101:
        _parse_chest(raw, s, obj)
    elif type_id == 82:
        _parse_sea_chest(raw, s, obj)
    elif type_id == 54:
        _parse_monster(raw, s, obj)
    elif type_id in (53, 17, 20):
        _parse_mine(raw, s, obj)
    elif type_id == 12:
        _parse_campfire(raw, s, obj)
    elif type_id == 112:
        _parse_windmill(raw, s, obj)
    elif type_id == 55:
        _parse_mystical_garden(raw, s, obj)
    elif type_id == 108:
        _parse_tomb(raw, s, obj)
    elif type_id == 6:
        _parse_pandora_box(raw, s, obj)
    elif type_id == 26:
        _parse_event(raw, s, obj)
    elif type_id == 86:
        _parse_survivor(raw, s, obj)
    elif type_id in (84, 85, 25, 24, 16):
        _parse_bank(raw, s, obj)
    elif type_id == 81:
        _parse_scholar(raw, s, obj)
    elif type_id == 93:
        _parse_scroll(raw, s, obj)
    elif type_id == 39:
        _parse_refugee_camp(raw, s, obj)
    elif type_id == 29:
        _parse_floatsam(raw, s, obj)
    elif type_id in (88, 89, 90):
        _parse_shrine(raw, s, obj, type_id)
    elif type_id == 63:
        _parse_pyramid(raw, s, obj)
    elif type_id == 22:
        _parse_skeleton(raw, s, obj)
    elif type_id == 105:
        _parse_wagon(raw, s, obj)
    elif type_id == 113:
        _parse_witch_hut(raw, s, obj)
    # ----- 13 new tile-level parsers (from honest audit, v3.9) -----
    elif type_id == 104:
        _parse_univer(raw, s, obj)
    elif type_id == 78:
        _parse_camp(raw, s, obj)
    elif type_id == 7:
        _parse_market(raw, s, obj)
    elif type_id == 33:
        _parse_garrison(raw, s, obj)
    elif type_id == 83:
        _parse_seer_hut(raw, s, obj)
    elif type_id == 215:
        _parse_pass_guard(raw, s, obj)
    elif type_id == 62:
        _parse_prison(raw, s, obj)
    elif type_id == 100:
        _parse_learning_stone(raw, s, obj)
    elif type_id == 34:
        _parse_hero_on_map(raw, s, obj)
    elif type_id in (2, 35, 95, 102, 213):
        _parse_generic_object(raw, s, obj)
    elif type_id in (43, 44, 45):
        _parse_monolith(raw, s, obj)
    elif type_id == 10:
        _parse_tent(raw, s, obj)
    elif type_id == 103:
        _parse_topology_obj(raw, s, obj)
    else:
        # Unknown type — store raw bytes for analysis
        obj['raw_bytes'] = raw[s:s + 20].hex(' ') if s + 20 <= len(raw) else ''
        return obj

    return obj


# ============================================================================
# Individual object parsers (from Save* methods)
# ============================================================================

def _safe_read(raw, off, size=1):
    """Safely read bytes from raw at offset."""
    if off + size > len(raw):
        return b'\x00' * size
    return raw[off:off + size]


def _parse_artifact(raw, s, obj):
    """Artifact on ground (SaveArt, line 10115).

    Per PRT MainForm.cs:10145: has_guard is set iff (raw[s+9] & 0x80) == 0x80
    (high bit of byte at offset s+9). When guarded, the ArtRes record's Num
    field is computed as ((raw[s+9] & 0x7F) << 8) + (raw[s+8] >> 3).
    """
    obj['artifact_id'] = raw[s + 2] if s + 3 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'artifact_id', s + 2, 1, "u8")
    # Check if guarded — match PRT SaveArt logic (line 10145):
    #   if ((decmp[s + 9] & 0x80) == 128) → guarded, add TblArtRes row
    if s + 10 <= len(raw):
        obj['has_guard'] = (raw[s + 9] & 0x80) == 0x80
        # ArtRes Num field (used by PRT to order the ArtRes records)
        obj['art_res_num'] = ((raw[s + 9] & 0x7F) << 8) + (raw[s + 8] >> 3)
    else:
        obj['has_guard'] = False
        obj['art_res_num'] = 0
    _enrich_typed_field(raw, obj, 'has_guard', s + 9, 1, "bool")


def _parse_resource(raw, s, obj):
    """Resource pile (SaveRes, line 10392)"""
    obj['resource_type'] = raw[s + 2] if s + 3 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'resource_type', s + 2, 1, "u8")
    # Resource amount is encoded in the tile data
    obj['amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'amount', s + 6, 2, "u16")


def _parse_chest(raw, s, obj):
    """Treasure chest — adapted from SaveChest (MainForm.cs:10215).

    Layout (1 byte object code at s, then 7 bytes of content at s+1..s+7):
        s+6 = artifact_id (only meaningful when s+7 == 252)
        s+7 = content code:
            147 → Gold=1000, Experience=500
            155 → Gold=1500, Experience=1000
            163 → Gold=2000, Experience=1500
            252 → Artifact (artifact_id at s+6); no gold/xp
            other → empty chest

    Note: in PRT, the 'HP' field stores experience reward (used later by
    AllExperience aggregator). We mirror that under obj['experience'].
    """
    if s + 8 > len(raw):
        obj['gold'] = 0
        obj['experience'] = 0
        obj['has_artifact'] = False
        return
    code = raw[s + 7]
    if code == 147:
        obj['gold'] = 1000
        obj['experience'] = 500
        obj['has_artifact'] = False
    elif code == 155:
        obj['gold'] = 1500
        obj['experience'] = 1000
        obj['has_artifact'] = False
    elif code == 163:
        obj['gold'] = 2000
        obj['experience'] = 1500
        obj['has_artifact'] = False
    elif code == 252:
        # Artifact chest: artifact_id at s+6
        obj['gold'] = 0
        obj['experience'] = 0
        obj['has_artifact'] = True
        obj['artifact_id'] = raw[s + 6]
        _enrich_typed_field(raw, obj, 'artifact_id', s + 6, 1, "u8")
    else:
        # Empty / unknown chest content
        obj['gold'] = 0
        obj['experience'] = 0
        obj['has_artifact'] = False
    _enrich_typed_field(raw, obj, 'gold', s + 6, 2, "u16")
    _enrich_typed_field(raw, obj, 'experience', s + 6, 2, "u16")


def _parse_sea_chest(raw, s, obj):
    """Sea chest — adapted from SaveSeaChest (MainForm.cs:10262).

    Layout:
        s+6 = artifact-related byte
        s+7 = content code:
            0xFF (255) → if s+6 == 249: Gold=1500, no artifact
                         else: empty
            other     → artifact_id = (s+6 >> 3) | ((s+7 & 0x0F) << 5)
                        Gold=1000 (and artifact added to AllArts)
    """
    if s + 8 > len(raw):
        obj['gold'] = 0
        obj['experience'] = 0
        obj['has_artifact'] = False
        return
    b6 = raw[s + 6]
    b7 = raw[s + 7]
    if b7 == 0xFF:
        if b6 == 249:
            obj['gold'] = 1500
        else:
            obj['gold'] = 0
        obj['experience'] = 0
        obj['has_artifact'] = False
    else:
        # Artifact + 1000 gold
        art_id = (b6 >> 3) | ((b7 & 0x0F) << 5)
        obj['gold'] = 1000
        obj['experience'] = 0
        obj['has_artifact'] = True
        obj['artifact_id'] = art_id
        _enrich_typed_field(raw, obj, 'artifact_id', s + 6, 2, "u16")
    _enrich_typed_field(raw, obj, 'gold', s + 6, 2, "u16")
    _enrich_typed_field(raw, obj, 'experience', s + 6, 2, "u16")


def _parse_monster(raw, s, obj):
    """Monster stack (SaveMonster, line 10498)"""
    obj['monster_type'] = raw[s + 2] if s + 3 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'monster_type', s + 2, 1, "u8")
    # Count: u12 LE = (decmp[s+7] & 0x0F << 8) + decmp[s+6]
    if s + 8 <= len(raw):
        obj['count'] = ((raw[s + 7] & 0x0F) << 8) + raw[s + 6]
        _enrich_typed_field(raw, obj, 'count', s + 6, 2, "u16")
        # Mood/disposition: (decmp[s+7] & 0xF0) >> 4
        mood = (raw[s + 7] & 0xF0) >> 4
        obj['mood'] = -4 if mood == 12 else mood
        _enrich_typed_field(raw, obj, 'mood', s + 7, 1, "u8")
    else:
        obj['count'] = 0
        obj['mood'] = 0
    # Growth flag: decmp[s+8] & 4
    obj['grows'] = bool(raw[s + 8] & 4) if s + 9 <= len(raw) else False
    _enrich_typed_field(raw, obj, 'grows', s + 8, 1, "bool")
    # Has artifact/guard: decmp[s+9] & 0x80
    obj['has_artifact'] = bool(raw[s + 9] & 0x80) if s + 10 <= len(raw) else False
    _enrich_typed_field(raw, obj, 'has_artifact', s + 9, 1, "bool")
    # MonstrContent Num: per PRT C# operator precedence, + binds tighter than >>
    # PRT: dataRow["Num"] = ((decmp[s + 9] & 7) << 8) + decmp[s + 8] >> 3
    # C# evaluates as: (((decmp[s + 9] & 7) << 8) + decmp[s + 8]) >> 3
    # NOT as: ((decmp[s + 9] & 7) << 8) + (decmp[s + 8] >> 3)  ← WRONG (Python default)
    if s + 10 <= len(raw):
        obj['monstr_num'] = (((raw[s + 9] & 7) << 8) + raw[s + 8]) >> 3
    else:
        obj['monstr_num'] = 0


def _parse_mine(raw, s, obj):
    """Mine (SaveMine, line 9665)"""
    obj['owner'] = raw[s + 4] if s + 5 <= len(raw) else 0xFF
    _enrich_typed_field(raw, obj, 'owner', s + 4, 1, "u8")
    obj['owner_name'] = {0: 'Red', 1: 'Blue', 2: 'Tan', 3: 'Green',
                         4: 'Orange', 5: 'Purple', 6: 'Teal', 7: 'Pink',
                         0xFF: 'Neutral'}.get(obj['owner'], f'?{obj["owner"]}')


def _parse_campfire(raw, s, obj):
    """Campfire (SaveCampfire, line 9817)"""
    obj['gold'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'gold', s + 6, 2, "u16")
    obj['resource_amount'] = (raw[s + 9] << 8) + raw[s + 8] if s + 10 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'resource_amount', s + 8, 2, "u16")


def _parse_windmill(raw, s, obj):
    """Windmill (SaveWindmill, line 9834)"""
    obj['resource_amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'resource_amount', s + 6, 2, "u16")


def _parse_mystical_garden(raw, s, obj):
    """Mystical Garden (SaveMysticalGarden, line 9850)"""
    obj['resource_amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'resource_amount', s + 6, 2, "u16")


def _parse_tomb(raw, s, obj):
    """Tomb (SaveTomb, line 10535)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'content', s + 6, 1, "u8")


def _parse_pandora_box(raw, s, obj):
    """Pandora's Box (SaveBox, line 10091)"""
    obj['event_num'] = ((raw[s + 7] & 1) << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'event_num', s + 6, 2, "u16")


def _parse_event(raw, s, obj):
    """Event (SaveEvent, line 10065)"""
    obj['object_code'] = raw[s] if s + 1 <= len(raw) else 0
    obj['event_num'] = ((raw[s + 7] & 1) << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    if s + 9 <= len(raw):
        obj['apply_flags'] = ((raw[s + 7] & 0xFC) >> 2) + ((raw[s + 8] & 3) << 6)
        obj['repeat'] = bool((raw[s + 8] & 8) >> 3)
        # PRT SaveEvent stores Apply as "apply_flags,num2" where num2 = decmp[s+8] & 4.
        # GetEventApply uses num2 > 0 to include non-human colors in "Доступно".
        obj['apply_for_all'] = bool(raw[s + 8] & 4)


def _parse_survivor(raw, s, obj):
    """Shipwreck Survivor (SaveSurvivor, line 10033)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'content', s + 6, 1, "u8")


def _parse_bank(raw, s, obj):
    """Bank (SaveBanks, line 9910)"""
    obj['bank_type'] = raw[s] if s + 1 <= len(raw) else 0
    obj['bank_subtype'] = raw[s + 2] if s + 3 <= len(raw) else 0
    if s + 9 <= len(raw):
        obj['guard_count'] = ((raw[s + 8] << 8) + raw[s + 7]) >> 5
    # Guards at s + 56, resources at s + 56 + 28
    if s + 56 + 28 <= len(raw):
        obj['gold'] = struct.unpack("<I", raw[s + 56:s + 60])[0] if s + 60 <= len(raw) else 0


def _parse_scholar(raw, s, obj):
    """Scholar (SaveScholar, line 9873)"""
    if s + 9 <= len(raw):
        if raw[s + 8] != 0xFF:
            # Spell
            obj['spell_id'] = ((raw[s + 8] & 0xF) << 3) + (raw[s + 7] >> 5)
            obj['scholar_type'] = 'spell'
        elif raw[s + 7] != 0xFF:
            # Secondary skill
            obj['skill_id'] = ((raw[s + 7] & 7) << 2) + (raw[s + 6] >> 6)
            obj['scholar_type'] = 'skill'
        elif raw[s + 6] == 192:
            obj['scholar_type'] = 'attack_plus_1'
        elif raw[s + 6] == 200:
            obj['scholar_type'] = 'defense_plus_1'
        elif raw[s + 6] == 208:
            obj['scholar_type'] = 'power_plus_1'
        elif raw[s + 6] == 216:
            obj['scholar_type'] = 'knowledge_plus_1'
        else:
            obj['scholar_type'] = 'unknown'


def _parse_scroll(raw, s, obj):
    """Spell Scroll (SaveScroll, line 10421)"""
    obj['spell_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'spell_id', s + 6, 1, "u8")


def _parse_refugee_camp(raw, s, obj):
    """Refugee Camp (SaveHovel, line 9801)"""
    obj['monster_type'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'monster_type', s + 6, 1, "u8")
    obj['count'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'count', s + 6, 2, "u16")


def _parse_floatsam(raw, s, obj):
    """Floatsam (SaveFloatsam, line 9711)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'content', s + 6, 1, "u8")


def _parse_shrine(raw, s, obj, type_id):
    """Shrine — adapted from SaveShrine (MainForm.cs:10484).

    Layout:
        s   = object code (88 = воплощение, 89 = жест, 90 = мысль)
        s+7 = byte whose top 3 bits encode part of spell_id
        s+8 = byte whose low 4 bits encode part of spell_id

    Formula (matches PRT exactly):
        spell_id = ((raw[s+8] & 0x0F) << 3) + (raw[s+7] >> 5)

    The shrine's object name (Святыня магического жеста/воплощения/мысли)
    comes from TblObject[code] (handled by localizer.object(type_id)).
    """
    obj['shrine_type'] = type_id  # 88/89/90
    _enrich_typed_field(raw, obj, 'shrine_type', s, 1, "u8")
    if s + 9 <= len(raw):
        # PRT formula: ((decmp[s + 8] & 0xF) << 3) + (decmp[s + 7] >> 5)
        spell_id = ((raw[s + 8] & 0x0F) << 3) + (raw[s + 7] >> 5)
        obj['spell_id'] = spell_id
        _enrich_typed_field(raw, obj, 'spell_id', s + 7, 2, "u16")
    else:
        obj['spell_id'] = 0


def _parse_pyramid(raw, s, obj):
    """Pyramid (SavePyramid, line 10445)"""
    obj['spell_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'spell_id', s + 6, 1, "u8")


def _parse_skeleton(raw, s, obj):
    """Skeleton (SaveSkeleton, line 10302)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'content', s + 6, 1, "u8")


def _parse_wagon(raw, s, obj):
    """Wagon (SaveWagon, line 10337)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'content', s + 6, 1, "u8")


def _parse_witch_hut(raw, s, obj):
    """Witch Hut (SaveWitchHut, line 10376)"""
    obj['skill_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'skill_id', s + 6, 1, "u8")


# ============================================================================
# 13 new tile-level parsers (from honest audit, v3.9)
# All extracted from MainForm.cs Save* methods
# ============================================================================

def _parse_univer(raw, s, obj):
    """University (SaveUniver, line 9978)
    Records x,y,z + Object; content (4 secondary skills) parsed
    separately by post_tile_parser.parse_univer_content."""
    obj['object_kind'] = 'university'
    obj['locality'] = 'underground' if obj.get('loc') == 8 else 'surface'


def _parse_camp(raw, s, obj):
    """Mercenary Camp (SaveCamp, line 10465)
    decmp[s+2] = monster_id, decmp[s+6] = number available"""
    obj['object_kind'] = 'mercenary_camp'
    obj['monster_id'] = raw[s + 2] if s + 3 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'monster_id', s + 2, 1, "u8")
    obj['number'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'number', s + 6, 1, "u8")


def _parse_market(raw, s, obj):
    """Black Market (SaveMarket, line 9994)
    Records x,y,z only; content (7 artifacts for sale) parsed separately
    by post_tile_parser.parse_market_content."""
    obj['object_kind'] = 'black_market'


def _parse_garrison(raw, s, obj):
    """Garrison (SaveGarrison, line 10017)
    decmp[s+2] = AntiMagic flag (0/1)"""
    obj['object_kind'] = 'garrison'
    obj['anti_magic'] = bool(raw[s + 2]) if s + 3 <= len(raw) else False
    _enrich_typed_field(raw, obj, 'anti_magic', s + 2, 1, "bool")


def _parse_seer_hut(raw, s, obj):
    """Seer Hut (SaveSeerHut, line 10157)
    decmp[s+6] = Num (quest identifier)"""
    obj['object_kind'] = 'seer_hut'
    obj['num'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'num', s + 6, 1, "u8")


def _parse_pass_guard(raw, s, obj):
    """Border Guard / Border Gate (SavePassDuard, line 10199)
    Num = (decmp[s+7] << 8) + decmp[s+6] — quest/key identifier"""
    obj['object_kind'] = 'border_guard'
    obj['num'] = ((raw[s + 7] << 8) + raw[s + 6]) if s + 8 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'num', s + 6, 2, "u16")


def _parse_prison(raw, s, obj):
    """Hero Prison (SavePrison, line 10180)
    decmp[s+6] = hero_id"""
    obj['object_kind'] = 'prison'
    obj['hero_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
    _enrich_typed_field(raw, obj, 'hero_id', s + 6, 1, "u8")


def _parse_learning_stone(raw, s, obj):
    """Learning Stone (SaveLearningStone, line 9699)
    Always gives fixed 1000 XP to visiting hero."""
    obj['object_kind'] = 'learning_stone'
    obj['experience'] = 1000  # fixed amount per source


def _parse_hero_on_map(raw, s, obj):
    """Hero on map (SaveHero + HeroOnObject, line 9675 + 9643)

    SaveHero: just records x,y,z as artifact-bearing object.
    HeroOnObject: if hero is on top of another object (e.g. standing on mine),
    temporarily swaps 5 bytes (s, s+6..s+9), re-dispatches IsObject to record
    the underlying object, then restores the 5 bytes.

    For parsing (read-only) we just record the hero_id at s+6 — the
    underlying object would have been already parsed by the regular
    tile loop on a different tile (or we don't need HeroOnObject because
    The original code does it to FILL DataTables that we don't use)."""
    obj['object_kind'] = 'hero_on_map'
    obj['hero_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
    obj['hero_id_2'] = raw[s + 7] if s + 8 <= len(raw) else 0
    # If hero is on top of an object (s+4 and s+5 not 0xFF), record it
    if s + 6 <= len(raw):
        obj['on_object'] = (raw[s + 4] != 0xFF and raw[s + 5] != 0xFF)


def _parse_generic_object(raw, s, obj):
    """Generic object — adapted from SaveObject (MainForm.cs:9740).

    Dispatched for codes: 2 (Altar of Sacrifice), 35 (Hill Fort),
    95 (Tavern), 102 (Tree of Knowledge), 213 (Freelancer's Guild).

    Only code 102 (Tree of Knowledge) has a payment field:
        decmp[s+7] == 32  → Payment = "2000"  (2000 gold)
        decmp[s+7] == 64  → Payment = "G10"   (10 gems)
        else              → Payment = None

    The object name is NOT decoded here — it is looked up by code in
    merge_generic_objects via localizer.object(type_id).
    """
    obj['object_kind'] = 'generic_object'
    type_id = obj['type_id']
    # Tree of Knowledge (code 102) — has payment field
    if type_id == 102 and s + 8 <= len(raw):
        cost_byte = raw[s + 7]
        if cost_byte == 32:
            obj['payment'] = "2000"
        elif cost_byte == 64:
            obj['payment'] = "G10"


def _parse_monolith(raw, s, obj):
    """Monolith — one-way or two-way (SaveMonolith, line 9766)
    decmp[s+2] = monolith subtype."""
    obj['object_kind'] = 'monolith'
    obj['monolith_subtype'] = raw[s + 2] if s + 3 <= len(raw) else 0


def _parse_tent(raw, s, obj):
    """Keymaster Tent (SaveTent, line 9778)
    decmp[s+2] = color of tent (aTent[] lookup)"""
    obj['object_kind'] = 'keymaster_tent'
    color_id = raw[s + 2] if s + 3 <= len(raw) else 0
    # aTent[]: 0=Blue, 1=Green, 2=Red, 3=Yellow, 4=Orange,
    # 5=Purple, 6=Teal, 7=Pink (matches aColor[])
    TENT_COLORS = {0: 'Blue', 1: 'Green', 2: 'Red', 3: 'Yellow',
                   4: 'Orange', 5: 'Purple', 6: 'Teal', 7: 'Pink'}
    obj['color_id'] = color_id
    obj['color_name'] = TENT_COLORS.get(color_id, f'?{color_id}')


def _parse_topology_obj(raw, s, obj):
    """Subterranean Gate or Whirlpool entry on tile level
    (SaveTopologyObj, line 9790)
    Records x,y,z only; pairing done in post_tile_parser."""
    obj['object_kind'] = 'topology_object'
    # Type 103 is used for subterranean gates in tile scan; the actual
    # pairs are in post-tile section (map.SubTerGate / map.SubTerGatePair).
