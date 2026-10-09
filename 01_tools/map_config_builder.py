#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
map_config_builder.py — Build a per-map config (anchor table) by
comparing a map JSON parse with a "day-zero" save (save immediately
after loading the map, before any player action).

THREE-PHASE ARCHITECTURE (see README.md):

  Phase 1: Map JSON  -> MapData            (see map_json_loader.py)
  Phase 2: Day-0 save + MapData -> MapConfig  (THIS module)
  Phase 3: Any save + MapConfig -> ParsedSave  (planned: save_parser.py)

Algorithm (Phase 2):

  1. Parse the save header → HeaderInfo (gives map_name, map_size,
     has_underground, header_size = where to start scanning).
  2. Find all 3-byte coord_int matches in the save → cluster them
     into per-object data sections (main, visiting, fog, alive, ...).
  3. Find hero blocks by scanning from header_size using:
       a. Regex-based search (h3sed-style)
       b. Name-based search (using standard hero names + map-specific
          Prison hero names from MapData)
  4. Find town blocks by scanning from header_size using:
       a. Search for 3-byte (x, y, z) coord patterns from MapData towns
       b. Validate by faction byte (0-7 or 255) and town_type byte (0-8)
       c. Cross-reference with town names from MapData when available
  5. Assemble everything into MapConfig dataclass.
  6. Save MapConfig to map_config_<mapname>.json.

The resulting config has ZERO hardcoded offsets — every section
boundary is computed from the day-zero save + map data.
"""

from __future__ import annotations
import gzip
import io
import json
import os
import re
import struct
from typing import Any, Dict, List, Optional, Tuple, Set
from collections import defaultdict

from save_layout import (
    HeaderInfo, ObjectCluster, HeroBlockInfo, HeroSection,
    TownBlockInfo, TownSection, ObjectOffsets, MapConfig,
    HERO_FIELD_OFFSETS, TOWN_FIELD_OFFSETS,
    HERO_BLOCK_SIZE, HERO_STRIDE_SOD, HERO_NAME_OFFSET_FROM_BLOCK_START,
    TOWN_NAME_OFFSET_FROM_BLOCK_START,
)
from header_parser import parse_header
from cluster_finder import find_all_object_clusters


# ============================================================================
# Decompression (gzip with broken CRC — HoMM3 saves)
# ============================================================================

def decompress_save(path: str) -> bytes:
    """Decompress a .GM1 save. HoMM3 saves are gzip with broken CRC
    (and sometimes truncated). We read until the gzip stream fails,
    then return what we got."""
    raw = open(path, "rb").read()
    if raw[:2] != b"\x1f\x8b":
        return raw  # already decompressed
    out = b""
    gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
    try:
        while True:
            chunk = gz.read(65536)
            if not chunk:
                break
            out += chunk
    except Exception:
        pass
    return out


# ============================================================================
# Hero finding (regex + name-based)
# ============================================================================

# Hero regex (h3sed-style) — simplified
HERO_REGEX = re.compile(b"""
    [\\x00-\\x07\\xFF]            # Player faction (0-7 or 255)
    .{30}                        # 30 bytes unknown
    .{4}                         # movement total
    .{4}                         # movement remaining
    .{4}                         # experience
    [\\x00-\\x1C][\\x00]{3}       # skill slots used (0-28)
    .{2}                         # spell points
    [\\x00-\\x4B]                 # hero level (0-75)
    .{63}                        # 63 bytes unknown
    .{28}                        # 7 x 4-byte creature IDs
    .{28}                        # 7 x 4-byte creature counts
    (?P<name>[^\\x00-\\x20].{11}\\x00)  # 13 bytes: hero name
    [\\x00-\\x03]{28}            # skill levels
    [\\x00-\\x1C]{28}            # skill slots
    .{4}                         # primary stats
    [\\x00-\\x01]{70}            # spells in book
    [\\x00-\\x01]{70}            # spells available
""", re.VERBOSE | re.DOTALL)


# Standard 156 SoD heroes (from cysun / h3sed)
STANDARD_HERO_NAMES = [
    "Christian", "Edric", "Orrin", "Sylvia", "Valeska", "Sorsha", "Tyris", "Lord Haart", "Catherine",
    "Roland", "Sir Mullich", "Adela", "Adelaide", "Caitlin", "Cuthbert", "Ingham", "Loynis", "Rion",
    "Sanya", "Jenova", "Kyrre", "Ivor", "Ufretin", "Clancy", "Thorgrim", "Ryland", "Mephala", "Gelu",
    "Aeris", "Alagar", "Coronius", "Elleshar", "Malcom", "Melodia", "Gem", "Uland", "Fafner", "Iona",
    "Josephine", "Neela", "Piquedram", "Rissa", "Thane", "Torosar", "Aine", "Astral", "Cyra", "Daremyth",
    "Halon", "Serena", "Solmyr", "Theodorus", "Dracon", "Calh", "Fiona", "Ignatius", "Marius", "Nymus",
    "Octavia", "Pyre", "Rashka", "Xeron", "Ash", "Axsis", "Ayden", "Calid", "Olema", "Xyron", "Xarfax",
    "Zydar", "Charna", "Clavius", "Galthran", "Isra", "Moandor", "Straker", "Tamika", "Vokial", "Aislinn",
    "Nagash", "Nimbus", "Sandro", "Septienna", "Thant", "Vidomina", "Xsi", "Ajit", "Arlach", "Dace",
    "Damacon", "Gunnar", "Lorelei", "Shakti", "Synca", "Mutare", "Mutare Drake", "Alamar", "Darkstorn",
    "Deemer", "Geon", "Jaegar", "Jeddite", "Malekith", "Sephinroth", "Crag Hack", "Gretchin", "Gurnisson",
    "Jabarkas", "Krellion", "Shiva", "Tyraxor", "Yog", "Boragus", "Kilgor", "Dessa", "Gird", "Gundula",
    "Oris", "Saurug", "Terek", "Vey", "Zubin", "Alkin", "Broghild", "Bron", "Drakon", "Gerwulf", "Korbac",
    "Tazar", "Wystan", "Andra", "Merist", "Mirlanda", "Rosic", "Styg", "Tiva", "Verdish", "Voy", "Adrienne",
    "Erdamon", "Fiur", "Ignissa", "Kalt", "Lacus", "Monere", "Pasis", "Thunar", "Aenain", "Brissa", "Ciele",
    "Gelare", "Grindan", "Inteus", "Labetha", "Luna", "Gen. Kendal", "Anabel", "Cassiopeia", "Corkes", "Derek",
    "Elmore", "Illor", "Leena", "Miriam", "Andal", "Astra", "Dargem", "Eovacius", "Manfred", "Zilare",
    "Jeremy", "Bidley", "Spint", "Casmetra", "Tark",
]


def _decode_name(name_bytes: bytes) -> str:
    """Decode a hero name: try ASCII first, then cp1251."""
    s = name_bytes.rstrip(b"\x00")
    try:
        return s.decode("ascii")
    except UnicodeDecodeError:
        try:
            return s.decode("cp1251")
        except Exception:
            return s.decode("latin-1", errors="replace")


def _is_valid_hero_name(name: str) -> bool:
    """
    Validate that a hero name is reasonable.

    Accepts names that consist of:
      - Latin letters (A-Z, a-z) — Unicode codepoints U+0041..U+005A, U+0061..U+007A
      - Cyrillic letters — Unicode codepoints U+0400..U+04FF (Cyrillic block),
        plus U+0500..U+052F (Cyrillic supplement) for extended characters
      - Spaces (for multi-word names like "Lord Haart" / "Лорд Хаарт")
      - Dots, apostrophes (for names like "Gen. Kendal")

    Rejects:
      - Strings of length 0 or 1
      - Strings containing control characters or non-letter symbols
      - Strings that are all the same character (likely byte-pattern noise)

    Note: this function takes a Python `str` (Unicode), so checks are by
    Unicode codepoint — NOT by cp1251 byte value. The cp1251 byte 0xCE
    becomes the Unicode codepoint U+041E (1054), not the byte value 0xCE.
    """
    if not name or len(name) < 2:
        return False
    # All-same character (e.g. "яяяя") = likely noise
    if len(set(name)) == 1:
        return False
    for c in name:
        o = ord(c)
        if o == 0x20:  # space
            continue
        if 0x41 <= o <= 0x5A:  # A-Z
            continue
        if 0x61 <= o <= 0x7A:  # a-z
            continue
        if 0x400 <= o <= 0x52F:  # Cyrillic (basic + supplement)
            continue
        if o in (0x2E, 0x27, 0x60):  # . ' `
            continue
        return False
    return True


def find_hero_blocks(raw: bytes,
                    scan_start: int,
                    map_hero_names: Optional[List[str]] = None) -> HeroSection:
    """
    Find hero blocks in the decompressed save using:
      1. h3sed-style regex (with name validation — accept names that
         consist of Latin or Cyrillic letters, spaces, dots, apostrophes.
         This works for English, Russian, and custom HotA hero names.)
      2. Name-based search (using standard English hero names + map-specific
         Prison hero names) as a supplement for heroes the regex misses.

    Args:
        raw:            decompressed save bytes
        scan_start:     byte offset to start scanning (header_size)
        map_hero_names:  hero names from MapData (e.g. Prison heroes)

    Returns HeroSection.
    """
    # Build set of all valid hero names for name-based search
    valid_names: Set[str] = set()
    for n in STANDARD_HERO_NAMES:
        if n:
            valid_names.add(n)
    if map_hero_names:
        for n in map_hero_names:
            if n:
                valid_names.add(n)

    hero_offsets: List[Tuple[int, str]] = []  # (name_offset, name)
    seen_positions: Set[int] = set()

    # ----- Method 1: h3sed-style regex (with permissive name validation) -----
    pos = scan_start
    while pos < len(raw) - HERO_BLOCK_SIZE:
        m = HERO_REGEX.search(raw, pos)
        if not m:
            break
        name_start = m.start() + HERO_NAME_OFFSET_FROM_BLOCK_START
        if name_start not in seen_positions:
            name = _decode_name(m.group("name"))
            if _is_valid_hero_name(name):
                seen_positions.add(name_start)
                hero_offsets.append((name_start, name))
        pos = m.end()

    # ----- Method 2: name-based search (supplement) -----
    # Search for English hero names (standard 156) — works for English
    # localized saves. For Russian saves, this finds nothing; method 1
    # is the primary path.
    for hero_name in valid_names:
        if not hero_name:
            continue
        try:
            name_bytes = hero_name.encode("ascii")[:13]
        except UnicodeEncodeError:
            try:
                name_bytes = hero_name.encode("cp1251")[:13]
            except Exception:
                continue
        if not name_bytes:
            continue
        pattern = name_bytes + b"\x00" * (13 - len(name_bytes))

        pos = scan_start
        while pos < len(raw) - 13:
            idx = raw.find(pattern, pos)
            if idx < 0:
                break
            if idx not in seen_positions:
                seen_positions.add(idx)
                hero_offsets.append((idx, hero_name))
            pos = idx + 1

    # Sort by offset
    hero_offsets.sort()

    # Parse each hero block
    blocks: List[HeroBlockInfo] = []
    for name_pos, name in hero_offsets:
        hero = _parse_hero_at(raw, name_pos, name)
        if hero:
            blocks.append(hero)

    # Compute stride
    if len(blocks) >= 2:
        strides = [blocks[i + 1].block_offset - blocks[i].block_offset
                   for i in range(min(5, len(blocks) - 1))]
        from collections import Counter
        stride = Counter(strides).most_common(1)[0][0]
    elif blocks:
        stride = HERO_STRIDE_SOD
    else:
        stride = 0

    return HeroSection(stride=stride, blocks=blocks)


def _parse_hero_at(raw: bytes, name_pos: int, name: str) -> Optional[HeroBlockInfo]:
    """Parse hero fields at the given name position.
    Returns None if validation fails."""
    o = HERO_FIELD_OFFSETS
    block_offset = name_pos - HERO_NAME_OFFSET_FROM_BLOCK_START

    if block_offset < 0 or name_pos + 365 + 512 > len(raw):
        return None

    # Validate: level 0-75
    level = raw[name_pos + o["HeroLevel"]]
    if level > 75:
        return None

    # Validate: numSkills 0-28
    try:
        num_skills = struct.unpack("<I",
            raw[name_pos + o["NumOfSkills"]:name_pos + o["NumOfSkills"] + 4])[0]
    except struct.error:
        return None
    if num_skills > 28:
        return None

    # Validate: player 0-7 or 255
    player = raw[name_pos + o["Player"]]
    if player > 7 and player != 0xFF:
        return None

    # Read coordinates
    try:
        x = struct.unpack("<h", raw[name_pos + o["CoordinatesX"]:name_pos + o["CoordinatesX"] + 2])[0]
        y = struct.unpack("<h", raw[name_pos + o["CoordinatesY"]:name_pos + o["CoordinatesY"] + 2])[0]
        z = struct.unpack("<h", raw[name_pos + o["CoordinatesZ"]:name_pos + o["CoordinatesZ"] + 2])[0]
    except struct.error:
        return None

    return HeroBlockInfo(
        block_offset=block_offset,
        name_offset=name_pos,
        name=name,
        player=player,
        level=level,
        x=x, y=y, z=z,
        is_active=player <= 7,
    )


# ============================================================================
# Town finding (coord-based + name-based)
# ============================================================================

def find_town_blocks(raw: bytes,
                     scan_start: int,
                     town_coords: List[Tuple[int, int, int]],
                     town_names: Optional[List[str]] = None) -> TownSection:
    """
    Find town blocks in the decompressed save.

    Town record structure (from h3sed):
      offset +0: faction (1 byte)
      offset +3: type (1 byte)
      offset +4: x (1 byte)
      offset +5: y (1 byte)
      offset +6: z (1 byte)
      offset +9..37: 7 × 4-byte creature IDs (28 bytes)
      offset +37..65: 7 × 4-byte creature counts (28 bytes)
      offset +69: name_len (2 bytes LE)
      offset +71: name (variable, cp1251)

    Algorithm:
      1. For each (x, y, z) from town_coords:
         - Search the save for the 3-byte (x, y, z) pattern
         - For each match at offset `idx`:
           - Check `idx - 4` (block_start): faction byte must be 0-7 or 255
           - Check `idx - 1` (town_type): byte 0-8
           - Check `idx + 65..67` (name_len): 0 < name_len <= 14
         - If all valid, this is a town block at idx-4
      2. If town_names given, also try searching for the name strings

    Args:
        raw:         decompressed save bytes
        scan_start:  byte offset to start scanning (header_size)
        town_coords: list of (x, y, z) for towns from MapData
        town_names:  list of town names (optional, cp1251 strings)

    Returns TownSection.
    """
    blocks: List[TownBlockInfo] = []
    seen_positions: Set[int] = set()

    # ----- Method 1: coord-based search -----
    for x, y, z in town_coords:
        needle = bytes([x, y, z])
        pos = scan_start
        while pos < len(raw) - 100:
            idx = raw.find(needle, pos)
            if idx < 0:
                break
            # Check town block start = idx - 4 (faction byte)
            if idx >= 4:
                block_start = idx - 4
                if block_start not in seen_positions:
                    town = _parse_town_at(raw, block_start)
                    if town:
                        seen_positions.add(block_start)
                        blocks.append(town)
            pos = idx + 1

    # ----- Method 2: name-based search (supplement) -----
    if town_names:
        for town_name in town_names:
            if not town_name:
                continue
            try:
                name_bytes = town_name.encode("cp1251")
            except Exception:
                continue
            pos = scan_start
            while pos < len(raw) - len(name_bytes):
                idx = raw.find(name_bytes, pos)
                if idx < 0:
                    break
                # Town block start = idx - 71
                block_start = idx - TOWN_NAME_OFFSET_FROM_BLOCK_START
                if block_start >= scan_start and block_start not in seen_positions:
                    town = _parse_town_at(raw, block_start)
                    if town and town.name == town_name:
                        seen_positions.add(block_start)
                        blocks.append(town)
                pos = idx + 1

    # Sort by offset
    blocks.sort(key=lambda b: b.block_offset)
    return TownSection(blocks=blocks)


def _parse_town_at(raw: bytes, block_start: int) -> Optional[TownBlockInfo]:
    """Parse a town block at the given offset.
    Returns None if validation fails."""
    o = TOWN_FIELD_OFFSETS
    if block_start < 0 or block_start + 80 > len(raw):
        return None

    faction = raw[block_start + o["faction"]]
    town_type = raw[block_start + o["type"]]
    x = raw[block_start + o["x"]]
    y = raw[block_start + o["y"]]
    z = raw[block_start + o["z"]]

    if faction > 7 and faction != 0xFF:
        return None
    if town_type > 8:
        return None
    if x > 0xFC or y > 0xFC or z > 1:
        return None

    # Read name length
    try:
        name_len = struct.unpack("<H",
            raw[block_start + o["name_len"]:block_start + o["name_len"] + 2])[0]
    except struct.error:
        return None
    if name_len == 0 or name_len > 14:
        return None

    # Read name
    name_start = block_start + o["name"]
    name_end = name_start + name_len
    if name_end > len(raw):
        return None
    try:
        name = raw[name_start:name_end].decode("cp1251")
    except Exception:
        name = ""

    return TownBlockInfo(
        block_offset=block_start,
        name_offset=name_start,
        name=name,
        faction=faction,
        town_type=town_type,
        x=x, y=y, z=z,
    )


# ============================================================================
# MapConfig builder
# ============================================================================

def build_map_config(map_data, day_zero_raw: bytes,
                     day_zero_path: str,
                     include_field_offsets: bool = True) -> MapConfig:
    """
    Build a complete per-map config by comparing map JSON (Phase 1)
    with the day-zero save (Phase 2 input).

    Args:
        map_data:             MapData object from map_json_loader.py
        day_zero_raw:         decompressed day-zero save bytes
        day_zero_path:        path to the day-zero .GM1 file (for metadata)
        include_field_offsets: include HERO_FIELD_OFFSETS and TOWN_FIELD_OFFSETS
                               in the config (useful for downstream parsing)

    Returns MapConfig.
    """
    # ----- Header -----
    header = parse_header(day_zero_raw)

    # ----- Cluster finding -----
    coord_ints = set(map_data.coord_int_lookup.keys())
    clusters, object_offsets = find_all_object_clusters(
        day_zero_raw, coord_ints, scan_start=header.header_size)

    # ----- Hero finding -----
    # Get hero names from map_data (Prison heroes)
    map_hero_names = []
    for ck, rec in map_data.objects_by_coord.items():
        if rec.get("category") == "hero":
            # Get hero name from details
            hero_name = rec.get("details", {}).get("name")
            if hero_name:
                map_hero_names.append(hero_name)

    hero_section = find_hero_blocks(day_zero_raw, header.header_size, map_hero_names)

    # ----- Town finding -----
    # Get town coords and names from map_data
    town_coords: List[Tuple[int, int, int]] = []
    town_names: List[str] = []
    for ck, rec in map_data.objects_by_coord.items():
        if rec.get("category") == "town":
            town_coords.append((rec["x"], rec["y"], rec["z"]))
            town_name = rec.get("details", {}).get("town_name")
            if town_name:
                town_names.append(town_name)

    town_section = find_town_blocks(day_zero_raw, header.header_size,
                                     town_coords, town_names)

    # ----- Assemble MapConfig -----
    meta = {
        "map_name":            header.map_name,
        "map_size":            header.map_size,
        "has_underground":     header.has_underground,
        "save_version_major":  header.version_major,
        "save_version_minor":  header.version_minor,
        "day_zero_save":       os.path.basename(day_zero_path),
        "day_zero_size":       len(day_zero_raw),
        "n_objects":           map_data.n_objects,
        "n_unique_coords":     len(map_data.objects_by_coord),
        "n_towns":             len(town_coords),
        "n_heroes_total":      hero_section.count,
        "header_size":          header.header_size,
        "header_size_hex":      f"0x{header.header_size:X}",
        "map_filename":        header.map_filename,
        "save_filename":       header.save_filename,
    }

    field_offsets = {}
    if include_field_offsets:
        field_offsets["hero"] = HERO_FIELD_OFFSETS
        field_offsets["town"] = TOWN_FIELD_OFFSETS
        field_offsets["constants"] = {
            "HERO_BLOCK_SIZE": HERO_BLOCK_SIZE,
            "HERO_STRIDE_SOD": HERO_STRIDE_SOD,
            "HERO_NAME_OFFSET_FROM_BLOCK_START": HERO_NAME_OFFSET_FROM_BLOCK_START,
            "TOWN_NAME_OFFSET_FROM_BLOCK_START": TOWN_NAME_OFFSET_FROM_BLOCK_START,
        }

    config = MapConfig(
        meta=meta,
        clusters=clusters,
        hero_section=hero_section,
        town_section=town_section,
        object_offsets=object_offsets,
        field_offsets=field_offsets,
    )
    return config


def save_config(config: MapConfig, output_dir: str) -> str:
    """Save MapConfig to a JSON file. Returns the path."""
    map_name = config.meta.get("map_name", "unknown_map")
    # Sanitize map name for filename
    safe_name = "".join(c if c.isalnum() or c in "-_" else "_" for c in map_name)
    if not safe_name:
        safe_name = "unknown_map"
    filename = f"map_config_{safe_name}.json"
    path = os.path.join(output_dir, filename)
    config.save(path)
    return path


def load_config(path: str) -> MapConfig:
    """Load a previously saved MapConfig."""
    with open(path, "r", encoding="utf-8") as f:
        d = json.load(f)
    return MapConfig.from_dict(d)


# ============================================================================
# CLI
# ============================================================================

def _main():
    import sys
    import argparse
    sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
    from map_json_loader import load_map_json_safely

    ap = argparse.ArgumentParser(description="Build per-map config from a day-0 save.")
    ap.add_argument("--map-json", required=True,
                    help="Path to .h3m.json or .h3m.zip with map JSON inside")
    ap.add_argument("--day0-save", required=True,
                    help="Path to day-zero .GM1 save")
    ap.add_argument("--output-dir", default="/tmp/",
                    help="Where to write map_config_<mapname>.json")
    ap.add_argument("--no-field-offsets", action="store_true",
                    help="Don't include HERO_FIELD_OFFSETS / TOWN_FIELD_OFFSETS in output")
    args = ap.parse_args()

    # Phase 1: load map JSON
    md, err = load_map_json_safely(args.map_json)
    if err:
        print(f"ERROR loading map JSON: {err}", file=sys.stderr)
        sys.exit(1)
    print(f"Map: {md.summary()}")

    # Phase 2: build config
    day0_raw = decompress_save(args.day0_save)
    print(f"Day0 decompressed: {len(day0_raw):,} bytes")
    config = build_map_config(md, day0_raw, args.day0_save,
                              include_field_offsets=not args.no_field_offsets)

    # Print summary
    print(f"\n--- Config summary ---")
    print(f"  map_name:          {config.meta['map_name']}")
    print(f"  header_size:       {config.meta['header_size_hex']}")
    print(f"  clusters:          {len(config.clusters)}")
    for c in config.clusters:
        print(f"    {c.name:15s} {c.start_hex}..{c.end_hex}  "
              f"hits={c.hits:5d}  distinct={c.distinct_coords:5d}")
    print(f"  hero_section:      count={config.hero_section.count}  "
          f"stride={config.hero_section.stride_hex}")
    if config.hero_section.blocks:
        print(f"  first hero:       {config.hero_section.blocks[0].block_offset_hex} "
              f"name={config.hero_section.blocks[0].name!r}")
    print(f"  town_section:      count={config.town_section.count}")
    for t in config.town_section.blocks[:5]:
        print(f"    {t.block_offset_hex} name={t.name!r} faction={t.faction} type={t.town_type} "
              f"({t.x},{t.y},{t.z})")
    print(f"  object_offsets:    {len(config.object_offsets)} entries")

    # Save
    out_path = save_config(config, args.output_dir)
    print(f"\nSaved: {out_path}")


if __name__ == "__main__":
    _main()
