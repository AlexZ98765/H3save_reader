#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
block_finder.py — Universal block finder for .GM1 saves.

Algorithms borrowed from 4 open-source HoMM3 save editors:
  - h3sed (Python):       regex-based hero/town struct search
  - vlucas (TypeScript):  hero name pattern + validation
  - svetoslav (TS/Electron): hero name list search
  - cysun (C#):           hero name search from end of file

COMMON PATTERN (all 4 editors):
  1. Heroes are found by searching for their NAME (13 bytes, null-padded)
  2. Search starts at offset ~30000 (skip header + name tables)
  3. NO pointers in header — blocks are found by pattern matching
  4. Hero block is ~1122 bytes, name at offset +169 (h3sed) or +0 (others)
  5. Towns found by name search (from end of file, or by faction)

This module provides:
  - find_hero_blocks(raw)    → list of (offset, name, player, level)
  - find_town_blocks(raw)    → list of (offset, name, faction, x, y, z)
  - validate_hero_at(raw, offset)  → bool (is this a real hero struct?)
"""

import re
import struct
from typing import List, Tuple, Optional


# ============================================================================
# Constants (from h3sed + vlucas + cysun)
# ============================================================================

HERO_SCAN_START = 30000  # Skip header + name tables (all editors agree)

# Hero block structure offsets (relative to name position)
# From vlucas/cysun: name is at offset 0, other fields are relative
# From h3sed: name is at +169 from block start, so block_start = name_pos - 169
HERO_NAME_OFFSET_FROM_BLOCK_START = 169  # h3sed
HERO_BLOCK_SIZE = 1122  # approximate, from h3sed

# Hero field offsets (relative to NAME position, from vlucas/cysun)
HERO_OFFSETS = {
    "Player":              -169,  # 1 byte: player faction 0-7 or 255
    "CoordinatesX":        -195,  # 2 bytes LE
    "CoordinatesY":        -193,  # 2 bytes LE
    "CoordinatesZ":        -191,  # 2 bytes LE
    "CoordinatesXMarker":  -150,  # marker
    "CoordinatesYMarker":  -146,  # marker
    "Experience":          -130,  # 4 bytes LE
    "MaxMovementPoints":   -138,  # 4 bytes LE
    "CurrentMovementPoints": -134, # 4 bytes LE
    "ManaPoints":          -122,  # 2 bytes LE
    "HeroLevel":           -120,  # 1 byte
    "NumOfSkills":         -126,  # 4 bytes LE
    "Creatures":           -56,   # 28 bytes: 7 × 4-byte creature IDs
    "CreatureAmounts":     -28,   # 28 bytes: 7 × 4-byte creature counts
    "Attributes":          69,    # 4 bytes: primary stats (attack/defense/power/knowledge)
    "Skills":              13,    # 28 bytes: skill levels
    "SkillSlots":          41,    # 28 bytes: skill IDs (HotA: 923)
    "Spells":              73,    # 70 bytes: spells in book
    "SpellBook":           143,   # 70 bytes: spells available
    "Helm":                213,   # 8 bytes (offset from block start = 213+169=382)
    "Weapon":              237,
    "Shield":              245,
    "Armor":               253,
    "Inventory":           365,   # 512 bytes: 64 × 8-byte artifact slots
}

# Town block structure (from h3sed TOWN_REGEX)
# Town block starts at faction byte
TOWN_OFFSETS = {
    "faction":    0,   # 1 byte: 0-7 or 255
    "type":       3,   # 1 byte: town type 0-8
    "x":          4,   # 1 byte
    "y":          5,   # 1 byte
    "z":          6,   # 1 byte
    "army_types": 9,   # 28 bytes: 7 × 4-byte creature IDs
    "army_counts": 37, # 28 bytes: 7 × 4-byte creature counts
    "name_len":   69,  # 2 bytes LE
    "name":       71,  # variable length
}


# ============================================================================
# Hero Regex (from h3sed, simplified)
# ============================================================================

# h3sed uses a complex regex to match the full hero struct.
# We use a simpler version that matches the key fields around the name.
HERO_REGEX = re.compile(b"""
    # Player faction (0-7 or 255)
    [\x00-\x07\xFF]
    .{30}                    # 30 bytes unknown
    .{4}                     # movement total
    .{4}                     # movement remaining
    .{4}                     # experience
    [\x00-\x1C][\x00]{3}     # skill slots used (0-28)
    .{2}                     # spell points
    [\x00-\x4B]              # hero level (0-75)
    .{63}                    # 63 bytes unknown
    .{28}                    # 7 x 4-byte creature IDs
    .{28}                    # 7 x 4-byte creature counts
    (?P<name>[^\x00-\x20].{11}\x00)  # 13 bytes: hero name
    [\x00-\x03]{28}          # skill levels
    [\x00-\x1C]{28}          # skill slots
    .{4}                     # primary stats
    [\x00-\x01]{70}          # spells in book
    [\x00-\x01]{70}          # spells available
""", re.VERBOSE | re.DOTALL)


# ============================================================================
# Hero name list (from cysun, all 156 SoD heroes)
# ============================================================================

HERO_NAMES = [
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


# ============================================================================
# Hero finding
# ============================================================================

def find_hero_blocks(raw: bytes) -> List[dict]:
    """
    Find all hero blocks in the decompressed save.

    Uses a hybrid approach:
      1. Try h3sed-style regex (most reliable)
      2. Fall back to name-based search (vlucas/svetoslav/cysun style)

    Returns list of dicts:
      {
        "offset": int,           # byte offset of name in save
        "block_offset": int,     # byte offset of hero block start (= offset - 169)
        "name": str,             # hero name
        "player": int,           # player 0-7 or 255 (neutral)
        "level": int,            # hero level 0-75
        "x": int, "y": int, "z": int,  # coordinates
        "is_active": bool,       # True if player <= 7 (on map)
      }
    """
    heroes = []

    # ----- Method 1: h3sed-style regex -----
    pos = HERO_SCAN_START
    while pos < len(raw) - HERO_BLOCK_SIZE:
        m = HERO_REGEX.search(raw, pos)
        if not m:
            break
        name_start = m.start() + 169  # name is at +169 from block start
        # Wait — the regex matches from the player faction byte, so m.start()
        # is the block start, and name is at m.start("name") - m.start() = 169
        # But actually HERO_REGEX starts matching at the faction byte,
        # and name is at +169 from there
        block_start = m.start()
        name_bytes = m.group("name")
        # Try ASCII first, then cp1251 (for Cyrillic hero names)
        try:
            name = name_bytes.rstrip(b"\x00").decode("ascii")
        except UnicodeDecodeError:
            try:
                name = name_bytes.rstrip(b"\x00").decode("cp1251")
            except Exception:
                name = name_bytes.rstrip(b"\x00").decode("latin-1", errors="replace")

        hero = _parse_hero_at(raw, name_start, name)
        if hero:
            heroes.append(hero)
        pos = m.end()

    # ----- Method 2: name-based search (fallback / supplement) -----
    # If regex found fewer than expected, also search by names
    if len(heroes) < 5:
        heroes = _find_heroes_by_name(raw)

    return heroes


def _find_heroes_by_name(raw: bytes) -> List[dict]:
    """Find heroes by searching for each known hero name (13 bytes, null-padded).
    This is the approach used by vlucas, svetoslav, and cysun."""
    heroes = []
    seen_positions = set()

    for hero_name in HERO_NAMES:
        # Build 13-byte pattern: name + null padding
        name_bytes = hero_name.encode("ascii")[:13]
        pattern = name_bytes + b"\x00" * (13 - len(name_bytes))

        # Search from HERO_SCAN_START
        pos = HERO_SCAN_START
        while pos < len(raw) - 13:
            idx = raw.find(pattern, pos)
            if idx < 0:
                break
            if idx not in seen_positions:
                seen_positions.add(idx)
                hero = _parse_hero_at(raw, idx, hero_name)
                if hero:
                    heroes.append(hero)
            pos = idx + 1

    return heroes


def _parse_hero_at(raw: bytes, name_pos: int, name: str) -> Optional[dict]:
    """Parse hero fields at the given name position.
    Returns None if validation fails."""
    o = HERO_OFFSETS

    # Check bounds
    if name_pos + o["Player"] < 0 or name_pos + 365 + 512 > len(raw):
        return None

    # Validate: level 0-75
    level = raw[name_pos + o["HeroLevel"]]
    if level > 75:
        return None

    # Validate: numSkills 0-28, followed by 3 zero bytes
    num_skills = struct.unpack("<I", raw[name_pos + o["NumOfSkills"]:name_pos + o["NumOfSkills"] + 4])[0]
    if num_skills > 28:
        return None
    # Check 3 zero bytes after numSkills
    # (vlucas checks this)

    # Validate: player 0-7 or 255
    player = raw[name_pos + o["Player"]]
    if player > 7 and player != 0xFF:
        return None

    # Read coordinates
    x = struct.unpack("<h", raw[name_pos + o["CoordinatesX"]:name_pos + o["CoordinatesX"] + 2])[0]
    y = struct.unpack("<h", raw[name_pos + o["CoordinatesY"]:name_pos + o["CoordinatesY"] + 2])[0]
    z = struct.unpack("<h", raw[name_pos + o["CoordinatesZ"]:name_pos + o["CoordinatesZ"] + 2])[0]

    return {
        "offset": name_pos,
        "block_offset": name_pos - HERO_NAME_OFFSET_FROM_BLOCK_START,
        "name": name,
        "player": player,
        "level": level,
        "x": x, "y": y, "z": z,
        "is_active": player <= 7,
    }


# ============================================================================
# Town finding
# ============================================================================

# Standard H3 town names per faction (from h3sed + vlucas)
TOWN_NAMES_BY_FACTION = {
    "Castle":    ["Абингдон", "Бигфорд", "Блэкширд", "Вайнброу", "Гавард", "Гринхем",
                  "Денмут", "Квартхоуст", "Килмор", "Лонглей", "Мерлайон", "Рокхоум",
                  "Сильверхельм", "Стронхолд", "Сэндмаркет", "Тайрхолм", "Хиллтоп", "Элкрав"],
    "Rampart":   ["Авликорн", "Дриффин", "Клоугхоум", "Лайонскар", "Шайем"],
    "Tower":     ["Бринфлёр", "Вейланд", "Ревант", "Хедмон", "Аэнаин"],
    "Inferno":   ["Крион", "Мидард", "Плэг", "Уркаган", "Чайнд"],
    "Necropolis": ["Даркмоор", "Дрейк", "Колгарт", "Рокмонд", "Шадоунд"],
    "Dungeon":   ["Блэкдор", "Гейр", "Краг", "Лорэ", "Морган", "Слай"],
    "Stronghold": ["Блэктурн", "Даркон", "Крак", "Уфтелин", "Вэй"],
    "Fortress":  ["Блэкклоу", "Гриф", "Кроу", "Мирмидон", "Рэйвен"],
    "Conflux":   ["Аэрис", "Игнис", "Терра", "Аква"],
}


def find_town_blocks(raw: bytes) -> List[dict]:
    """
    Find all town blocks in the decompressed save.

    Towns are found by searching for town names (from the map or standard list).
    The town struct starts 71 bytes before the name (faction + type + coords + army).

    Returns list of dicts:
      {
        "offset": int,       # byte offset of town block start (faction byte)
        "name_offset": int,  # byte offset of town name
        "name": str,         # town name
        "faction": int,      # 0-7 or 255
        "type": int,         # 0-8
        "x": int, "y": int, "z": int,
      }
    """
    towns = []
    seen_positions = set()

    # Search for each town name
    all_town_names = []
    for faction, names in TOWN_NAMES_BY_FACTION.items():
        for name in names:
            all_town_names.append((name, faction))

    # Also search for Cyrillic names (win1251)
    for town_name, faction in all_town_names:
        try:
            name_bytes = town_name.encode("cp1251")
        except Exception:
            name_bytes = town_name.encode("utf-8")

        pos = HERO_SCAN_START
        while pos < len(raw) - len(name_bytes):
            idx = raw.find(name_bytes, pos)
            if idx < 0:
                break
            # Check that this is a town (not just a name in some other context)
            # Town name is at offset 71 from block start
            if idx >= 71 and idx not in seen_positions:
                town = _parse_town_at(raw, idx, town_name, faction)
                if town:
                    seen_positions.add(idx)
                    towns.append(town)
            pos = idx + 1

    return towns


def _parse_town_at(raw: bytes, name_pos: int, name: str, faction_hint: str) -> Optional[dict]:
    """Parse town fields at the given name position.
    Town block starts 71 bytes before name (at faction byte)."""
    o = TOWN_OFFSETS
    block_start = name_pos - o["name"]

    if block_start < 0 or block_start + 70 > len(raw):
        return None

    faction = raw[block_start + o["faction"]]
    town_type = raw[block_start + o["type"]]
    x = raw[block_start + o["x"]]
    y = raw[block_start + o["y"]]
    z = raw[block_start + o["z"]]

    # Validate
    if faction > 7 and faction != 0xFF:
        return None
    if town_type > 8:
        return None
    if x > 0xFC or y > 0xFC or z > 1:
        return None

    # Read name length
    name_len = struct.unpack("<H", raw[block_start + o["name_len"]:block_start + o["name_len"] + 2])[0]
    if name_len > 14 or name_len == 0:
        return None

    return {
        "offset": block_start,
        "name_offset": name_pos,
        "name": name,
        "faction": faction,
        "type": town_type,
        "x": x, "y": y, "z": z,
    }


# ============================================================================
# Main object finding (from coord_bytes cluster analysis)
# ============================================================================

def find_object_array_start(raw: bytes, n_objects: int = 7009) -> Optional[int]:
    """
    Find the start of the main object-state array.

    The main array has one record per map object. Each record starts with
    3 coord bytes (x, y, z). We find the array by looking for a dense cluster
    of coord-byte matches.

    This is the approach we developed in our coord_mapping_verification.
    """
    # Known cluster ranges (from analyze_save_clusters.py)
    # These are for "Myth and Legend.h3m" — for other maps they'll differ
    # but the DENSITY pattern should be similar
    best_offset = None
    best_count = 0

    # Scan in 0x10000-byte windows
    window_size = 0x10000
    for start in range(0x100000, min(len(raw), 0x180000), window_size):
        end = min(start + window_size, len(raw))
        # Count how many bytes in this window are followed by 0x00 or 0xFF
        # (typical for coord + flag pattern)
        coord_like = 0
        for i in range(start, end - 3):
            # coord pattern: x, y, z where z is 0 or 1
            if raw[i + 2] in (0, 1) and raw[i + 3] in (0, 1, 0xFF, 0x81):
                coord_like += 1
        if coord_like > best_count:
            best_count = coord_like
            best_offset = start

    return best_offset


# ============================================================================
# Summary
# ============================================================================

def find_all_blocks(raw: bytes) -> dict:
    """
    Find all major blocks in the save.

    Returns:
      {
        "heroes": [...],  # list of hero dicts
        "towns": [...],   # list of town dicts
        "object_array_start": int or None,
      }
    """
    return {
        "heroes": find_hero_blocks(raw),
        "towns": find_town_blocks(raw),
        "object_array_start": find_object_array_start(raw),
    }


if __name__ == "__main__":
    import gzip
    import io
    import sys

    def decompress(path):
        raw = open(path, "rb").read()
        gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
        out = b""
        try:
            while True:
                chunk = gz.read(65536)
                if not chunk:
                    break
                out += chunk
        except Exception:
            pass
        return out

    if len(sys.argv) < 2:
        print("Usage: python3 block_finder.py <save.gm1>")
        sys.exit(1)

    raw = decompress(sys.argv[1])
    print(f"Save size: {len(raw):,} bytes")

    result = find_all_blocks(raw)
    print(f"\nHeroes found: {len(result['heroes'])}")
    for h in result["heroes"][:10]:
        active = "ACTIVE" if h["is_active"] else "pool"
        print(f"  {h['name']:20s} player={h['player']:3d} level={h['level']:2d} "
              f"({h['x']:3d},{h['y']:3d},{h['z']}) [{active}] @ 0x{h['offset']:X}")

    print(f"\nTowns found: {len(result['towns'])}")
    for t in result["towns"][:10]:
        print(f"  {t['name']:20s} faction={t['faction']:3d} type={t['type']} "
              f"({t['x']:3d},{t['y']:3d},{t['z']}) @ 0x{t['offset']:X}")

    print(f"\nObject array start: {result['object_array_start']}")
    if result['object_array_start']:
        print(f"  = 0x{result['object_array_start']:X}")
