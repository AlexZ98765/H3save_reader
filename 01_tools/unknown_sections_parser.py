#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
unknown_sections_parser.py — Parse all UNKNOWN sections of .GM1 saves.

Closes ALL coverage gaps identified in export_coverage_report.py:

  1. Game setup (680 bytes after save_filename) — player config, victory/loss
  2. SR section (28 bytes) — script/random, 7×u32
  3. Variable structures (355 bytes) — u16 chain
  4. num5/sprites (variable) — .def filenames + 35 bytes metadata
  5. Dwelling (count × 75 bytes) — creature availability
  6. UnknownVarReg (count × 28 bytes) — active objects with coords
  7. UnknownFixedReg (49 bytes) — global game state
  8. BitField (map_size² × 4 bytes) — fog of war bitmask
  9. Motions (variable) — path records / AI movements
"""

from __future__ import annotations
import struct
from dataclasses import dataclass, field
from typing import Any, Dict, List, Optional, Tuple


def _u8(raw, s):
    return raw[s] if s < len(raw) else 0

def _u16(raw, s):
    return (raw[s+1] << 8) | raw[s] if s + 2 <= len(raw) else 0

def _u32(raw, s):
    return (raw[s+3] << 24) | (raw[s+2] << 16) | (raw[s+1] << 8) | raw[s] if s + 4 <= len(raw) else 0

def _bytes_hex(raw, s, length):
    return raw[s:s+length].hex(" ") if s + length <= len(raw) else ""


# ============================================================================
# 1. Game setup (680 bytes after save_filename null terminator)
# ============================================================================

def parse_game_setup(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse game setup block — player config, victory/loss, hero availability.

    This block is 688 bytes from sf_null to SR (ProspectorRT: s += 688).
    After save_filename + null, there are ~680 bytes of game setup.
    Structure mirrors .h3m sections: players, conditions, available_heroes,
    disabled artifacts/spells/skills, rumors, heroes_settings.
    """
    block = raw[start:end]
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": len(block),
        "raw_hex": block.hex(" "),
    }

    # Try to identify sub-sections by looking for patterns
    # The save likely stores these in a similar order to .h3m:
    # 1. Player configs (8 players × some bytes)
    # 2. Victory/loss conditions
    # 3. Available heroes bitmask
    # 4. Disabled artifacts/spells/skills bitmasks
    # 5. Rumors count
    # 6. Hero configured flags

    # Find all non-zero regions
    non_zero_regions = []
    in_nz = False
    for i, b in enumerate(block):
        if b != 0 and not in_nz:
            in_nz = True
            run_start = i
        elif b == 0 and in_nz:
            in_nz = False
            run = block[run_start:i]
            non_zero_regions.append({
                "offset": start + run_start,
                "offset_hex": f"0x{start + run_start:x}",
                "length": i - run_start,
                "bytes_hex": run.hex(" "),
                "values": list(run),
            })
    if in_nz:
        run = block[run_start:]
        non_zero_regions.append({
            "offset": start + run_start,
            "offset_hex": f"0x{start + run_start:x}",
            "length": len(block) - run_start,
            "bytes_hex": run.hex(" "),
            "values": list(run),
        })

    result["non_zero_regions"] = non_zero_regions
    result["non_zero_count"] = len(non_zero_regions)
    result["non_zero_bytes"] = sum(1 for b in block if b != 0)
    result["description"] = (
        "Game setup: player configuration (8 players: human/AI/behavior), "
        "victory/loss conditions, available heroes bitmask, "
        "disabled artifacts/spells/skills, rumors count, hero configured flags. "
        "Corresponds to .h3m sections: players, specific_condition, available_heroes, "
        "disabled_*, rumors, heroes_settings."
    )
    return result


# ============================================================================
# 2. SR section (28 bytes)
# ============================================================================

def parse_sr_section(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse SR (Script/Random) section — 28 bytes, possibly RNG seed + conditions."""
    block = raw[start:end]
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": len(block),
        "raw_hex": block.hex(" "),
    }

    # Parse as 7 × u32 LE
    u32_values = []
    for i in range(0, 28, 4):
        v = _u32(raw, start + i)
        u32_values.append({
            "offset": start + i,
            "offset_hex": f"0x{start + i:x}",
            "value": v,
            "value_hex": f"0x{v:08X}",
        })
    result["u32_values"] = u32_values

    # Parse as 28 × u8
    u8_values = list(block)
    result["u8_values"] = u8_values

    result["description"] = (
        "SR (Script/Random) section: 28 bytes read by ProspectorRT Get_SR(). "
        "Likely contains RNG seed, handicap settings, and/or victory/loss condition flags. "
        "All zeros on this save — may be populated on other saves."
    )
    result["all_zeros"] = all(b == 0 for b in block)
    return result


# ============================================================================
# 3. Variable structures (after SR)
# ============================================================================

def parse_variable_structures(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse variable structures after SR — u16 chain of length+data."""
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": end - start,
    }

    s = start
    blocks = []

    # First block: u16 length + data
    if s + 2 <= end:
        length1 = _u16(raw, s)
        s += 2
        data1 = raw[s:s + length1] if s + length1 <= end else b""
        blocks.append({
            "block_index": 0,
            "type": "length_prefixed",
            "offset": start,
            "offset_hex": f"0x{start:x}",
            "length": length1,
            "data_hex": data1.hex(" ")[:200],
            "data_size": len(data1),
        })
        s += length1

    # Second block: u16 count + count × (u16 + 3 bytes)
    if s + 2 <= end:
        count = _u16(raw, s)
        s += 2
        entries = []
        for i in range(count):
            if s + 5 <= end:
                entry_len = _u16(raw, s)
                entry_data = raw[s + 2:s + 5]
                entries.append({
                    "index": i,
                    "offset": s,
                    "offset_hex": f"0x{s:x}",
                    "u16_value": entry_len,
                    "data_hex": entry_data.hex(" "),
                })
                s += 5  # 2 + 3
        blocks.append({
            "block_index": 1,
            "type": "count_prefixed",
            "offset": start + length1 + 2 if 'length1' in dir() else 0,
            "count": count,
            "entries": entries,
        })

    # Remaining bytes
    if s < end:
        remaining = raw[s:end]
        blocks.append({
            "block_index": 2,
            "type": "remaining",
            "offset": s,
            "offset_hex": f"0x{s:x}",
            "size": len(remaining),
            "data_hex": remaining.hex(" ")[:200],
        })

    result["blocks"] = blocks
    result["description"] = (
        "Variable structures after SR: victory/loss condition data, "
        "custom hero settings, and other pre-game configuration. "
        "Structure: u16 length + data, then u16 count + count × (u16 + 3 bytes)."
    )
    return result


# ============================================================================
# 4. num5 / sprites (u16 count + count × (u16 msg_len + 35 bytes))
# ============================================================================

@dataclass
class SpriteRecord:
    index: int
    offset: int
    filename: str
    metadata_hex: str
    metadata_bytes: bytes


def parse_sprite_records(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse num5 records — sprite definitions with .def filenames + 35 bytes metadata."""
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": end - start,
    }

    count = _u16(raw, start)
    s = start + 4  # u16 count + 2 padding
    records: List[Dict[str, Any]] = []

    for i in range(count):
        if s + 2 > end:
            break
        msg_len = _u16(raw, s)
        s += 2
        filename_raw = raw[s:s + msg_len] if s + msg_len <= end else b""
        try:
            filename = filename_raw.decode("ascii", errors="replace").rstrip("\x00")
        except Exception:
            filename = filename_raw.hex()
        s += msg_len
        metadata = raw[s:s + 35] if s + 35 <= end else b""
        s += 35

        records.append({
            "index": i,
            "offset": s - msg_len - 35 - 2,
            "offset_hex": f"0x{s - msg_len - 35 - 2:x}",
            "filename": filename,
            "msg_len": msg_len,
            "metadata_hex": metadata.hex(" "),
            "metadata_size": len(metadata),
        })

    result["count"] = count
    result["records"] = records
    result["description"] = (
        f"Sprite definitions: {count} records. Each: u16 filename_length + ASCII .def filename + "
        "35 bytes metadata (object type, render flags, animation data). "
        "Corresponds to .h3m 'sprites' section."
    )
    return result


# ============================================================================
# 5. Dwelling (u8 count + 2 + count × 75 bytes)
# ============================================================================

@dataclass
class DwellingRecord:
    index: int
    offset: int
    creature_id: int
    x: int
    y: int
    z: int
    available_count: int
    growth_count: int
    raw_hex: str


def parse_dwelling_section(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse dwelling section — creature availability records."""
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": end - start,
    }

    count = _u8(raw, start)
    s = start + 3  # u8 count + 2 padding
    records: List[Dict[str, Any]] = []

    for i in range(count):
        if s + 75 > end:
            break
        chunk = raw[s:s + 75]
        creature_id = chunk[0]
        # Try to decode coords — bytes 1-3 might be x, y, z (but z=0xFF on some)
        x = chunk[1]
        y = chunk[2]
        z_raw = chunk[3]
        z = z_raw if z_raw < 2 else -1  # 0xFF = undefined

        # Look for u16 counts in the record
        # Bytes 6-7: possible available count (u16 LE)
        # Bytes 8-9: possible growth count (u16 LE)
        avail = _u16(raw, s + 6) if s + 8 <= end else 0
        growth = _u16(raw, s + 8) if s + 10 <= end else 0

        records.append({
            "index": i,
            "offset": s,
            "offset_hex": f"0x{s:x}",
            "creature_id": creature_id,
            "x": x,
            "y": y,
            "z": z,
            "z_raw": z_raw,
            "available_count": avail,
            "growth_count": growth,
            "raw_hex": chunk.hex(" "),
        })
        s += 75

    result["count"] = count
    result["records"] = records
    result["description"] = (
        f"Dwelling availability: {count} records × 75 bytes. "
        "Each: creature_id (u8), coords, available count, growth count, flags."
    )
    return result


# ============================================================================
# 6. UnknownVarReg (u8 count + 1 + count × 28 bytes)
# ============================================================================

def parse_unknown_var_reg(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse UnknownVarReg — active objects with coordinates."""
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": end - start,
    }

    count = _u8(raw, start)
    s = start + 1  # u8 count + 1
    records: List[Dict[str, Any]] = []

    for i in range(count):
        if s + 28 > end:
            break
        chunk = raw[s:s + 28]
        rec_type = chunk[0]
        rec_id = chunk[1]
        flags = (chunk[2] << 8) | chunk[3]

        # Coords appear twice in the record
        # Bytes 8-10: x, padding, y
        # Bytes 14-16: x again, padding, y again
        x1 = chunk[8]
        y1 = chunk[10]
        x2 = chunk[14]
        y2 = chunk[16]

        # Also try u16 LE for coords
        coord_x = _u16(chunk, 8) if len(chunk) >= 10 else 0
        coord_y = _u16(chunk, 10) if len(chunk) >= 12 else 0

        records.append({
            "index": i,
            "offset": s,
            "offset_hex": f"0x{s:x}",
            "type": rec_type,
            "id": rec_id,
            "flags": flags,
            "flags_hex": f"0x{flags:04X}",
            "x1": x1,
            "y1": y1,
            "x2": x2,
            "y2": y2,
            "coord_x_u16": coord_x,
            "coord_y_u16": coord_y,
            "raw_hex": chunk.hex(" "),
        })
        s += 28

    result["count"] = count
    result["records"] = records
    result["description"] = (
        f"UnknownVarReg: {count} records × 28 bytes. "
        "Each: type (u8), id (u8), flags (u16), coordinates (x, y repeated), "
        "and padding. Likely a list of active map objects (heroes, towns) with their positions."
    )
    return result


# ============================================================================
# 7. UnknownFixedReg (49 bytes)
# ============================================================================

def parse_unknown_fixed_reg(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse UnknownFixedReg — global game state (49 bytes)."""
    block = raw[start:end]
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": len(block),
        "raw_hex": block.hex(" "),
    }

    # Parse individual bytes with offsets
    bytes_list = []
    for i, b in enumerate(block):
        bytes_list.append({
            "offset": start + i,
            "offset_hex": f"0x{start + i:x}",
            "value": b,
            "value_hex": f"0x{b:02X}",
        })
    result["bytes"] = bytes_list

    # Try to interpret first few bytes
    result["first_byte"] = block[0] if block else 0  # 8 = number of players?
    result["second_byte"] = block[1] if len(block) > 1 else 0
    result["third_byte"] = block[2] if len(block) > 2 else 0

    result["description"] = (
        f"UnknownFixedReg: 49 bytes of global game state. "
        f"First byte = {block[0] if block else 0} (possibly active player count), "
        f"then flags and padding. Only {sum(1 for b in block if b != 0)} non-zero bytes."
    )
    return result


# ============================================================================
# 8. BitField / fog of war (map_size² × (2 + 2×MapSide) bytes)
# ============================================================================

def parse_bitfield(raw: bytes, start: int, end: int,
                   map_size: int, has_underground: bool) -> Dict[str, Any]:
    """Parse fog of war BitField — visibility bitmask per tile per player."""
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": end - start,
        "map_size": map_size,
        "has_underground": has_underground,
    }

    layers = 2 if has_underground else 1
    bytes_per_tile = 2 + 2 * (1 if has_underground else 0)  # 4 with underground, 2 without
    total_tiles = map_size * map_size * layers
    expected_size = map_size * map_size * bytes_per_tile

    result["expected_size"] = expected_size
    result["bytes_per_tile"] = bytes_per_tile
    result["total_tiles"] = total_tiles
    result["layers"] = layers

    # Don't parse ALL tiles into dict (too much data for JSON)
    # Instead: parse a sample + statistics
    block = raw[start:end]
    non_zero = sum(1 for b in block if b != 0)
    result["non_zero_bytes"] = non_zero
    result["non_zero_percent"] = non_zero * 100 // len(block) if block else 0

    # Sample: first 10 tiles of surface layer
    sample_tiles = []
    for i in range(min(10, map_size * map_size)):
        offset = start + i * bytes_per_tile
        if offset + bytes_per_tile <= end:
            tile_bytes = raw[offset:offset + bytes_per_tile]
            # Each 2-byte group = visibility bitmask for that layer
            # Bit N = player N can see this tile
            visibility = []
            for l in range(layers):
                v = _u16(raw, offset + l * 2)
                players_visible = [p for p in range(8) if v & (1 << p)]
                visibility.append({
                    "layer": l,
                    "bitmask": v,
                    "bitmask_hex": f"0x{v:04X}",
                    "players_visible": players_visible,
                })
            x = i % map_size
            y = i // map_size
            sample_tiles.append({
                "tile_index": i,
                "x": x,
                "y": y,
                "offset": offset,
                "offset_hex": f"0x{offset:x}",
                "visibility": visibility,
            })

    result["sample_tiles"] = sample_tiles
    result["description"] = (
        f"Fog of war bitmask: {map_size}×{map_size} × {layers} layers × {bytes_per_tile} bytes = "
        f"{expected_size} bytes. Each tile has a {bytes_per_tile}-byte visibility bitmask: "
        f"bit N = player N can see this tile. {non_zero} non-zero bytes ({non_zero*100//len(block) if block else 0}% visible)."
    )
    return result


# ============================================================================
# 9. Motions / path records (variable length after Banks)
# ============================================================================

def parse_motions(raw: bytes, start: int, end: int) -> Dict[str, Any]:
    """Parse Motions section — path records / AI movements at end of save."""
    block = raw[start:end]
    result = {
        "start": start,
        "start_hex": f"0x{start:x}",
        "end": end,
        "end_hex": f"0x{end:x}",
        "size": len(block),
    }

    # Try to parse as a sequence of path records
    # Looking at the data: it contains repeated patterns of coords + type bytes
    # e.g.: 37 00 6a 00 = x=55, y=106 (u16 LE coords)
    # d2 07 = some type/flag
    # 04 00 00 00 = some count/size

    s = start
    records: List[Dict[str, Any]] = []
    record_count = 0

    # Try parsing as variable-length records
    # Each record seems to start with a u16 or u8 type indicator
    while s < end - 4 and record_count < 100:  # safety limit
        # Try to identify record structure
        # Look for patterns: u8 type + u8 flags + u16 x + u16 y + ...
        rec_type = _u8(raw, s)
        rec_flags = _u8(raw, s + 1) if s + 1 < end else 0

        # Some records have u16 at start
        u16_val = _u16(raw, s)

        # Try to find coord pairs (u16 x, u16 y)
        if s + 6 <= end:
            x1 = _u16(raw, s + 2)
            y1 = _u16(raw, s + 4)
            x2 = _u16(raw, s + 6) if s + 8 <= end else 0
            y2 = _u16(raw, s + 8) if s + 10 <= end else 0

            records.append({
                "index": record_count,
                "offset": s,
                "offset_hex": f"0x{s:x}",
                "type": rec_type,
                "flags": rec_flags,
                "u16_start": u16_val,
                "coord1": (x1, y1) if x1 < map_size_max and y1 < map_size_max else None,
                "coord2": (x2, y2) if x2 < map_size_max and y2 < map_size_max else None,
                "raw_hex": raw[s:s+min(20, end-s)].hex(" "),
            })
            record_count += 1

        # Advance by variable amount — try to find next record
        # This is heuristic: look for the next non-zero byte after zeros
        s += 1  # simple step — we'll refine later

    result["record_count"] = record_count
    result["records"] = records[:20]  # only first 20 for JSON
    result["total_records_found"] = record_count
    result["raw_hex"] = block.hex(" ")  # full dump — it's only 427 bytes
    result["non_zero_bytes"] = sum(1 for b in block if b != 0)
    result["description"] = (
        f"Motions / path records: {len(block)} bytes at end of save file. "
        "Contains coordinate pairs (u16 x, u16 y) with movement types — "
        "likely hero path records or AI decision data. "
        f"{record_count} potential records identified. Full hex dump included."
    )
    return result


# Global for coordinate validation
map_size_max = 256


# ============================================================================
# Master: parse_all_unknown_sections
# ============================================================================

def parse_all_unknown_sections(raw: bytes, post_tile_sections: Dict[str, int],
                               map_start_info: Dict[str, int],
                               header) -> Dict[str, Any]:
    """Parse ALL unknown sections and return results.

    Args:
        raw: decompressed save bytes
        post_tile_sections: dict from walk_post_tile_sections
        map_start_info: dict from find_map_start
        header: HeaderInfo from parse_header

    Returns dict with keys for each unknown section.
    """
    global map_size_max
    map_size_max = header.map_size if header else 256

    results: Dict[str, Any] = {}

    sr_offset = map_start_info.get("sr_offset", 0)
    after_sr = map_start_info.get("after_sr", 0)
    after_variable = map_start_info.get("after_variable_structures", 0)
    sf_null = map_start_info.get("sf_null", 0)
    black_market_count = map_start_info.get("black_market_count", 0)

    # 1. Game setup (sf_null to SR, 688 bytes)
    if sf_null and sr_offset:
        results["game_setup"] = parse_game_setup(raw, sf_null, sr_offset)

    # 2. SR section (28 bytes)
    if sr_offset and after_sr:
        results["sr_section"] = parse_sr_section(raw, sr_offset, after_sr)

    # 3. Variable structures
    if after_sr and after_variable:
        results["variable_structures"] = parse_variable_structures(raw, after_sr, after_variable)

    # 4. num5 / sprites — need to compute start/end
    # after_tiles is computed by caller, but we can compute from map_start
    ms = map_start_info.get("map_start", 0)
    import struct as _struct
    s = ms
    total_tiles = header.map_size * header.map_size * (2 if header.has_underground else 1)
    for tile_num in range(total_tiles):
        if s + 18 > len(raw): break
        s += 7 + 11
        if s + 2 <= len(raw):
            var_count = _struct.unpack("<H", raw[s:s + 2])[0]
            s += var_count * 4 + 4
        else: break
    num5_start = s
    num5_count = _u16(raw, num5_start)
    s2 = num5_start + 4
    for i in range(num5_count):
        if s2 + 2 > len(raw): break
        n = _u16(raw, s2)
        s2 += n + 35
    num5_end = s2
    if num5_start and num5_end > num5_start:
        results["sprite_records"] = parse_sprite_records(raw, num5_start, num5_end)

    # 5. Dwelling
    dwelling = post_tile_sections.get("dwelling", 0)
    garrison = post_tile_sections.get("garrison", 0)
    if dwelling and garrison:
        results["dwelling"] = parse_dwelling_section(raw, dwelling, garrison)

    # 6. UnknownVarReg
    unknown_var_reg = post_tile_sections.get("unknown_var_reg", 0)
    unknown_fixed_reg = post_tile_sections.get("unknown_fixed_reg", 0)
    if unknown_var_reg and unknown_fixed_reg:
        results["unknown_var_reg"] = parse_unknown_var_reg(raw, unknown_var_reg, unknown_fixed_reg)

    # 7. UnknownFixedReg
    if unknown_fixed_reg:
        color = post_tile_sections.get("color", 0)
        if color:
            results["unknown_fixed_reg"] = parse_unknown_fixed_reg(raw, unknown_fixed_reg, color)

    # 8. BitField
    bit_field = post_tile_sections.get("bit_field", 0)
    two_way_monolith = post_tile_sections.get("two_way_monolith", 0)
    if bit_field and two_way_monolith:
        results["bitfield"] = parse_bitfield(raw, bit_field, two_way_monolith,
                                             header.map_size, header.has_underground)

    # 9. Motions
    motions = post_tile_sections.get("motions", 0)
    if motions and motions < len(raw):
        results["motions"] = parse_motions(raw, motions, len(raw))

    return results
