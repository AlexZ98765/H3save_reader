#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
tile_scanner.py — Universal tile scanner for .GM1 saves.

Implements ProspectorRT's Scanner loop: iterates through map tiles, finds objects
via IsObject dispatcher, and parses object content for each type.

FIXED: Uses .h3m (always present) instead of .GM1 (may lack extension) for navigation.
Works on ANY save (not just day-0), unlike ProspectorRT.

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
# Constants from ProspectorRT decompilation
# ============================================================================

# Teams signature: 00 01 02 03 04 05 06 07 00 01 02 03 04 05 06 07
TEAMS_SIGNATURE = bytes([0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7])


# ============================================================================
# 1. find_map_start — GetStart + GetMapStart (FIXED: use .h3m instead of .GM1)
# ============================================================================

def find_map_start(raw: bytes) -> Tuple[int, Dict[str, int]]:
    """
    Find where tile data begins (map.Start) in the decompressed save.

    Algorithm (adapted from ProspectorRT GetStart + GetMapStart):
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
        # Fallback: try .GM1 (ProspectorRT's original approach — works for day-0 saves)
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
    # ProspectorRT: do { s--; } while (decmp[s] != 0);  → s = null byte BEFORE filename
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
# 2. scan_tiles — Scanner loop (ProspectorRT Scanner, line 6172)
# ============================================================================

def scan_tiles(raw: bytes, map_start: int, map_size: int,
               has_underground: bool) -> List[Dict[str, Any]]:
    """
    Iterate through map tiles and find all objects.

    Algorithm (from ProspectorRT Scanner, line 6184-6208):
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

            # Object conditions (from ProspectorRT Scanner)
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
# 3. parse_object_content — IsObject dispatcher → Save* methods
# ============================================================================

def parse_object_content(raw: bytes, s: int, type_id: int,
                          x: int, y: int, z: int, loc: int) -> Optional[Dict[str, Any]]:
    """
    Parse object content based on type_id (from ProspectorRT IsObject dispatcher).

    Args:
        raw:     decompressed save bytes
        s:       offset of object data (decmp[s] = type_id)
        type_id: first byte = object type
        x, y, z: tile coordinates
        loc:     first byte of tile record (terrain/ground type?)

    Returns dict with object data, or None if type is unknown/skipped.
    """
    obj = {
        'type_id': type_id,
        'type_name': OBJECT_TYPE_IDS.get(type_id, f'Unknown({type_id})'),
        'x': x, 'y': y, 'z': z,
        'offset': s,
        'loc': loc,
    }

    # Dispatch based on type_id (from ProspectorRT IsObject, line 9484)
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
    else:
        # Unknown type — store raw bytes for analysis
        obj['raw_bytes'] = raw[s:s + 20].hex(' ') if s + 20 <= len(raw) else ''
        return obj

    return obj


# ============================================================================
# Individual object parsers (from ProspectorRT Save* methods)
# ============================================================================

def _safe_read(raw, off, size=1):
    """Safely read bytes from raw at offset."""
    if off + size > len(raw):
        return b'\x00' * size
    return raw[off:off + size]


def _parse_artifact(raw, s, obj):
    """Artifact on ground (ProspectorRT SaveArt, line 10115)"""
    obj['artifact_id'] = raw[s + 2] if s + 3 <= len(raw) else 0
    # Check if guarded (from ArtResContent)
    obj['has_guard'] = raw[s + 8] != 0xFF if s + 9 <= len(raw) else False


def _parse_resource(raw, s, obj):
    """Resource pile (ProspectorRT SaveRes, line 10392)"""
    obj['resource_type'] = raw[s + 2] if s + 3 <= len(raw) else 0
    # Resource amount is encoded in the tile data
    obj['amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0


def _parse_chest(raw, s, obj):
    """Treasure chest (ProspectorRT SaveChest, line 10215)"""
    obj['gold'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    obj['experience'] = (raw[s + 9] << 8) + raw[s + 8] if s + 10 <= len(raw) else 0
    # If decmp[s+8] != 0xFF, it's an artifact instead of XP
    obj['has_artifact'] = raw[s + 8] != 0xFF if s + 9 <= len(raw) else False
    if obj['has_artifact']:
        obj['artifact_id'] = raw[s + 8]
        obj['experience'] = 0


def _parse_sea_chest(raw, s, obj):
    """Sea chest (ProspectorRT SaveSeaChest, line 10262)"""
    obj['gold'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    obj['experience'] = (raw[s + 9] << 8) + raw[s + 8] if s + 10 <= len(raw) else 0


def _parse_monster(raw, s, obj):
    """Monster stack (ProspectorRT SaveMonster, line 10498)"""
    obj['monster_type'] = raw[s + 2] if s + 3 <= len(raw) else 0
    # Count: u16 LE = (decmp[s+7] & 0x0F << 8) + decmp[s+6]
    if s + 8 <= len(raw):
        obj['count'] = ((raw[s + 7] & 0x0F) << 8) + raw[s + 6]
        # Mood/disposition: (decmp[s+7] & 0xF0) >> 4
        mood = (raw[s + 7] & 0xF0) >> 4
        obj['mood'] = -4 if mood == 12 else mood
    else:
        obj['count'] = 0
        obj['mood'] = 0
    # Growth flag: decmp[s+8] & 4
    obj['grows'] = bool(raw[s + 8] & 4) if s + 9 <= len(raw) else False
    # Has artifact: decmp[s+9] & 0x80
    obj['has_artifact'] = bool(raw[s + 9] & 0x80) if s + 10 <= len(raw) else False


def _parse_mine(raw, s, obj):
    """Mine (ProspectorRT SaveMine, line 9665)"""
    obj['owner'] = raw[s + 4] if s + 5 <= len(raw) else 0xFF
    obj['owner_name'] = {0: 'Red', 1: 'Blue', 2: 'Tan', 3: 'Green',
                         4: 'Orange', 5: 'Purple', 6: 'Teal', 7: 'Pink',
                         0xFF: 'Neutral'}.get(obj['owner'], f'?{obj["owner"]}')


def _parse_campfire(raw, s, obj):
    """Campfire (ProspectorRT SaveCampfire, line 9817)"""
    obj['gold'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    obj['resource_amount'] = (raw[s + 9] << 8) + raw[s + 8] if s + 10 <= len(raw) else 0


def _parse_windmill(raw, s, obj):
    """Windmill (ProspectorRT SaveWindmill, line 9834)"""
    obj['resource_amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0


def _parse_mystical_garden(raw, s, obj):
    """Mystical Garden (ProspectorRT SaveMysticalGarden, line 9850)"""
    obj['resource_amount'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0


def _parse_tomb(raw, s, obj):
    """Tomb (ProspectorRT SaveTomb, line 10535)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_pandora_box(raw, s, obj):
    """Pandora's Box (ProspectorRT SaveBox, line 10091)"""
    obj['event_num'] = ((raw[s + 7] & 1) << 8) + raw[s + 6] if s + 8 <= len(raw) else 0


def _parse_event(raw, s, obj):
    """Event (ProspectorRT SaveEvent, line 10065)"""
    obj['object_code'] = raw[s] if s + 1 <= len(raw) else 0
    obj['event_num'] = ((raw[s + 7] & 1) << 8) + raw[s + 6] if s + 8 <= len(raw) else 0
    if s + 9 <= len(raw):
        obj['apply_flags'] = ((raw[s + 7] & 0xFC) >> 2) + ((raw[s + 8] & 3) << 6)
        obj['repeat'] = bool((raw[s + 8] & 8) >> 3)


def _parse_survivor(raw, s, obj):
    """Shipwreck Survivor (ProspectorRT SaveSurvivor, line 10033)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_bank(raw, s, obj):
    """Bank (ProspectorRT SaveBanks, line 9910)"""
    obj['bank_type'] = raw[s] if s + 1 <= len(raw) else 0
    obj['bank_subtype'] = raw[s + 2] if s + 3 <= len(raw) else 0
    if s + 9 <= len(raw):
        obj['guard_count'] = ((raw[s + 8] << 8) + raw[s + 7]) >> 5
    # Guards at s + 56, resources at s + 56 + 28
    if s + 56 + 28 <= len(raw):
        obj['gold'] = struct.unpack("<I", raw[s + 56:s + 60])[0] if s + 60 <= len(raw) else 0


def _parse_scholar(raw, s, obj):
    """Scholar (ProspectorRT SaveScholar, line 9873)"""
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
    """Spell Scroll (ProspectorRT SaveScroll, line 10421)"""
    obj['spell_id'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_refugee_camp(raw, s, obj):
    """Refugee Camp (ProspectorRT SaveHovel, line 9801)"""
    obj['monster_type'] = raw[s + 6] if s + 7 <= len(raw) else 0
    obj['count'] = (raw[s + 7] << 8) + raw[s + 6] if s + 8 <= len(raw) else 0


def _parse_floatsam(raw, s, obj):
    """Floatsam (ProspectorRT SaveFloatsam, line 9711)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_shrine(raw, s, obj, type_id):
    """Shrine (ProspectorRT SaveShrine, line 10484)"""
    obj['shrine_type'] = type_id  # 88=gesture, 89=thought, 90=incantation
    obj['spell_id'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_pyramid(raw, s, obj):
    """Pyramid (ProspectorRT SavePyramid, line 10445)"""
    obj['spell_id'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_skeleton(raw, s, obj):
    """Skeleton (ProspectorRT SaveSkeleton, line 10302)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_wagon(raw, s, obj):
    """Wagon (ProspectorRT SaveWagon, line 10337)"""
    obj['content'] = raw[s + 6] if s + 7 <= len(raw) else 0


def _parse_witch_hut(raw, s, obj):
    """Witch Hut (ProspectorRT SaveWitchHut, line 10376)"""
    obj['skill_id'] = raw[s + 6] if s + 7 <= len(raw) else 0
