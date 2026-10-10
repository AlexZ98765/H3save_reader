#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
gm1_parser.py — PySide6 GUI парсер .GM1 сейвов Heroes of Might and Magic III.

Версия 2.0 — обновлена с полной структурой hero block и town record из h3sed.

Возможности:
  - Открытие .GM1 файлов (с обходом битного gzip CRC)
  - Парсинг согласно gm1_mapping.json
  - Tree view блоков и полей
  - Hex дамп выделенного поля/блока
  - Экспорт в JSON с полной раскладкой
  - Поиск hero blocks через h3sed-style regex
  - Декодирование path-records (7 типов)

Запуск:
    python3 gm1_parser.py [path_to_gm1_mapping.json] [path_to_save.gm1]
"""

import sys
import os
import io
import gzip
import json
import struct
import re
import contextlib
from typing import Any, Dict, List, Optional, Tuple, Union

from PySide6.QtCore import Qt
from PySide6.QtGui import QFont, QAction
from PySide6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QPushButton, QLineEdit, QLabel, QPlainTextEdit, QComboBox,
    QSpinBox, QCheckBox, QFileDialog, QGroupBox, QFormLayout,
    QMessageBox, QSplitter, QTabWidget, QTreeWidget, QTreeWidgetItem,
    QTextEdit, QStatusBar, QMenuBar,
    QTableWidget, QTableWidgetItem, QHeaderView,
    QProgressBar, QFrame, QGraphicsOpacityEffect, QSizePolicy,
)


# ============================================================================
# КОНСТАНТЫ
# ============================================================================

DEFAULT_MAPPING_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "02_format_docs", "gm1_mapping.json")

# h3sed HERO_BYTE_POSITIONS (SoD base + HotA overrides)
# NOTE: location_x/y/z are 2-byte LE (u16), stored as pairs:
#   x at offset -26 (2 bytes: -26, -25)
#   y at offset -24 (2 bytes: -24, -23)
#   z at offset -22 (2 bytes: -22, -21)
#   on_map flag at offset -20 (1 byte)
# This matches the hex pattern: XX 00 YY 00 ZZ 00 01
HERO_BYTE_POSITIONS = {
    "location_x":      -26,  # u16 LE (2 bytes: -26, -25)
    "location_y":      -24,  # u16 LE (2 bytes: -24, -23)
    "location_z":      -22,  # u16 LE (2 bytes: -22, -21)
    "on_map":          -20,  # u8 (1 byte)
    "faction":           0,
    "movement_total":   31,
    "movement_left":    35,
    "experience":       39,
    "skills_count":     43,
    "mana_left":        47,
    "level":            49,
    "army_types":      113,
    "army_counts":     141,
    "hero_name":       169,
    "skill_levels":    182,
    "attack":          238,
    "defense":         239,
    "power":           240,
    "knowledge":       241,
    "spells_book":     242,
    "spells_available":312,
    "helm":            382,
    "cloak":           390,
    "neck":            398,
    "weapon":          406,
    "shield":          414,
    "armor":           422,
    "lefthand":        430,
    "righthand":       438,
    "feet":            446,
    "side1":           454,
    "side2":           462,
    "side3":           470,
    "side4":           478,
    "ballista":        486,
    "ammo":            494,
    "tent":            502,
    "catapult":        510,
    "spellbook":       518,
    "side5":           526,
    "inventory":       534,
    "skills_slot":    1092,  # HotA
}

# h3sed TOWN_BYTE_POSITIONS
TOWN_BYTE_POSITIONS = {
    "faction":      0,
    "location_x":   4,
    "location_y":   5,
    "location_z":   6,
    "army_types":   9,
    "army_counts": 37,
}

# h3sed HERO_REGEX (HotA version)
HERO_REGEX = re.compile(b"""
    .                        #   1 byte:  player faction 0-7 or 255            000-000
    .{30}                    #  30 bytes: unknown                              001-031
    .{4}                     #   4 bytes: movement points in total             031-034
    .{4}                     #   4 bytes: movement points remaining            035-038
    .{4}                     #   4 bytes: experience                           039-042
    [\\x00-\\x1C][\\x00]{3}     #   4 bytes: skill slots used                     043-046
    .{2}                     #   2 bytes: spell points remaining               047-048
    .{1}                     #   1 byte:  hero level                           049-049
    .{63}                    #  63 bytes: unknown                              050-112
    .{28}                    #  28 bytes: 7 4-byte creature IDs                113-140
    .{28}                    #  28 bytes: 7 4-byte creature counts             141-168
                             #  13 bytes: hero name, null-padded               169-181
    (?P<name>[^\\x00-\\x20].{11}\\x00)
    [\\x00-\\x03]{30}          #  30 bytes: skill levels (HotA)                  182-211
    .{26}                    #  26 bytes: skill slots (legacy, unused)          212-237
    .{4}                     #   4 bytes: primary stats                        238-241
    [\\x00-\\x01]{70}          #  70 bytes: spells in book                       242-311
    [\\x00-\\x01]{70}          #  70 bytes: spells available                     312-381
    (?P<equipment>(          # 152 bytes: 19 8-byte equipments worn            382-533
      (\\xFF{4} .{4}) | (.\\x00{3} .{4})
    ){19})
                             # 512 bytes: 64 8-byte artifacts in inventory     534-1045
    ( ((.\\x00{3}) | \\xFF{4}){2} ){64}
    .{10}                    # 10 bytes: slots taken by combination artifacts 1046-1055
    .{36}                    #  36 bytes: unknown                             1056-1091
    [\\x00-\\x1C]{29,30}       #  30 bytes: skill slots (HotA)                  1092-1121
""", re.VERBOSE | re.DOTALL)

# h3sed TOWN_REGEX (HotA version)
TOWN_REGEX = re.compile(b"""
    (?P<faction>[\\x00-\\x07,\\xFF])  #   1 byte:  town faction 0-7 or 255              000-000
    .{2}                           #   3 bytes: unknown                              001-002
    (?P<type>[\\x00-\\x0B])          #   1 byte:  town type                            003-003
    (?P<x>[\\x00-\\xFC])             #   1 byte:  X coordinate                         004-004
    (?P<y>[\\x00-\\xFC])             #   1 byte:  Y coordinate                         005-005
    (?P<z>[\\x00-\\x01])             #   1 byte:  Z coordinate                         006-006
    .{2}                           #   2 bytes: unknown                              007-008
    (?P<army_names>(               #  28 bytes: 7 4-byte creature IDs                009-036
      (.[\\x00,\\xFF]{3})
    ){7})
    (?P<army_counts>.{28})         #  28 bytes: 7 4-byte creature counts             037-064
    .{4}                           #   4 bytes: unknown                              065-068
    (?P<name_len>[^\\x00]\\x00)      #   2 bytes: name length                          069-070
    (?P<name>                      #   X bytes: name; not 0-terminated               071-
      [^\\x00-\\x20,^\\xFF][^\\x00-\\x1F,^\\xFF]{0,13}
    )
""", re.VERBOSE | re.DOTALL)

PLAYER_COLORS = {
    "0": "Red", "1": "Blue", "2": "Tan", "3": "Green",
    "4": "Orange", "5": "Purple", "6": "Teal", "7": "Pink",
    "255": "Neutral",
}

TOWN_TYPES = {
    "0": "Castle", "1": "Rampart", "2": "Tower", "3": "Inferno",
    "4": "Necropolis", "5": "Dungeon", "6": "Stronghold", "7": "Fortress",
    "8": "Conflux", "9": "Cove", "10": "Factory", "11": "Bulwark",
}


# ============================================================================
# ЛОГИКА ПАРСИНГА
# ============================================================================

@contextlib.contextmanager
def _patch_gzip_for_partial():
    reader_cls = getattr(gzip, "_GzipReader", None)
    if reader_cls is not None and hasattr(reader_cls, "_read_eof"):
        orig_read_eof = reader_cls._read_eof
        def patched_read_eof(self):
            try:
                self._fp.read(8)
            except Exception:
                pass
        reader_cls._read_eof = patched_read_eof
        try:
            yield
        finally:
            reader_cls._read_eof = orig_read_eof
    else:
        yield


def load_gm1_file(path: str) -> bytes:
    with open(path, "rb") as f:
        data = f.read()
    if data[:2] == b"\x1f\x8b":
        try:
            with _patch_gzip_for_partial():
                with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
                    raw = gf.read()
        except Exception:
            try:
                import zlib
                raw = zlib.decompress(data[10:], -15)
            except Exception:
                raw = data
    else:
        raw = data
    return raw


def parse_offset(offset_val) -> int:
    if isinstance(offset_val, int):
        return offset_val
    if isinstance(offset_val, str):
        return int(offset_val, 16) if offset_val.startswith("0x") else int(offset_val)
    raise ValueError(f"Invalid offset type: {type(offset_val)}")


def read_u8(raw, off):
    return raw[off] if 0 <= off < len(raw) else None

def read_u16_le(raw, off):
    if 0 <= off + 2 <= len(raw):
        return struct.unpack("<H", raw[off:off+2])[0]
    return None

def read_u32_le(raw, off):
    if 0 <= off + 4 <= len(raw):
        return struct.unpack("<I", raw[off:off+4])[0]
    return None

def read_ascii(raw, off, size):
    if 0 <= off + size <= len(raw):
        return raw[off:off+size].rstrip(b"\x00").decode("ascii", errors="replace")
    return None

def read_cp1251(raw, off, size):
    if 0 <= off + size <= len(raw):
        return raw[off:off+size].rstrip(b"\x00").decode("cp1251", errors="replace")
    return None

def format_player_color(val):
    if val is None:
        return "?"
    return PLAYER_COLORS.get(str(val), f"Unknown ({val})")

def format_town_type(val):
    if val is None:
        return "?"
    return TOWN_TYPES.get(str(val), f"Unknown ({val})")


# ============================================================================
# HERO BLOCK PARSER (h3sed-style)
# ============================================================================

def find_hero_blocks(raw: bytes, scan_start: int = None) -> List[Dict]:
    """
    Ищет hero blocks используя эвристический поиск (h3sed-style, но с cp1251 name support).
    Проверяет валидность полей: faction, movement, level, attack/defense/power/knowledge,
    equipment pattern, spell bitmaps, skill levels.

    Args:
        raw:        decompressed save bytes
        scan_start: byte offset to start scanning. If None, use header_size
                    from header_parser (no hardcoded 0x100000).
    """
    heroes = []
    seen_offsets = set()

    if scan_start is None:
        try:
            from header_parser import parse_header
            scan_start = parse_header(raw).header_size
        except Exception:
            scan_start = 0x400  # safe fallback (small maps have header_size ~0x258)
    scan_start = max(scan_start, 0x100)  # at least 256 bytes

    for i in range(scan_start, len(raw) - 1122):
        faction = raw[i]
        if faction > 7 and faction != 255:
            continue

        # movement_total (offset 31)
        mt = int.from_bytes(raw[i+31:i+35], 'little')
        if mt > 10000 or mt < 100:
            continue

        # movement_left (offset 35)
        ml = int.from_bytes(raw[i+35:i+39], 'little')
        if ml > 300000 and ml != 299999:
            continue

        # level (offset 49)
        level = raw[i+49]
        if level > 74:
            continue

        # skills_count (offset 43) + 3 zero bytes after (vlucas check)
        sc = int.from_bytes(raw[i+43:i+47], 'little')
        if sc > 28:
            continue
        # vlucas: bytes after numSkills must be 0x00 0x00 0x00
        if raw[i+44] != 0 or raw[i+45] != 0 or raw[i+46] != 0:
            continue

        # attack/defense/power/knowledge (offset 238-241)
        attack = raw[i+238]
        defense = raw[i+239]
        power = raw[i+240]
        knowledge = raw[i+241]
        if attack > 99 or defense > 99 or power > 99 or knowledge > 99:
            continue
        if power == 0 and knowledge == 0 and attack == 0 and defense == 0:
            continue

        # experience (offset 39)
        exp = int.from_bytes(raw[i+39:i+43], 'little')
        if exp > 100000000:
            continue

        # ⭐ SPELL BITMAP validation (offset 242-311 = 70 bytes, each 0x00 or 0x01)
        spells_book = raw[i+242:i+312]
        if any(b > 1 for b in spells_book):
            continue

        # ⭐ SPELLS AVAILABLE validation (offset 312-381 = 70 bytes, each 0x00 or 0x01)
        spells_avail = raw[i+312:i+382]
        if any(b > 1 for b in spells_avail):
            continue

        # ⭐ EQUIPMENT validation (offset 382-533 = 152 bytes = 19 × 8-byte slots)
        # Each slot must be one of:
        #   FF FF FF FF XX XX XX XX  (empty slot)
        #   XX 00 00 00 FF FF FF FF  (artifact, no data)
        #   XX 00 00 00 00 00 00 00  (scroll)
        #   XX 00 00 00 XX XX 00 00  (catapult etc)
        equipment = raw[i+382:i+534]
        equip_valid = True
        has_non_blank = False  # at least one non-empty, non-zero slot
        for slot_idx in range(19):
            slot = equipment[slot_idx*8 : slot_idx*8 + 8]
            # Pattern 1: FF FF FF FF XX XX XX XX (empty)
            if slot[0] == 0xFF and slot[1] == 0xFF and slot[2] == 0xFF and slot[3] == 0xFF:
                continue
            # Pattern 2: XX 00 00 00 (artifact/scroll/war machine)
            if slot[1] == 0 and slot[2] == 0 and slot[3] == 0:
                has_non_blank = True
                # Data part: either 00 00 00 00, FF FF FF FF, or XX XX 00 00
                data = slot[4:8]
                if data == b'\x00\x00\x00\x00' or data == b'\xFF\xFF\xFF\xFF':
                    continue
                if data[2] == 0 and data[3] == 0:
                    continue  # catapult etc: XX XX 00 00
                equip_valid = False
                break
            equip_valid = False
            break
        if not equip_valid:
            continue
        # Reject all-blank equipment (h3sed EQUIPMENT_REGEX negative lookahead)
        if not has_non_blank:
            continue

        # Name (offset 169, 13 bytes, cp1251) — first byte must be printable
        name_bytes = raw[i+169:i+169+13]
        if name_bytes[0] == 0x00 or name_bytes[0] == 0xFF:
            continue
        if name_bytes[0] < 0x20:  # control character
            continue

        # Decode name as cp1251
        name = name_bytes.rstrip(b"\x00").decode("cp1251", errors="replace")

        # ⭐ STRICTER name validation
        clean_name = name.strip()
        if len(clean_name) < 2:
            continue
        # Reject names with control characters (0x01-0x1F)
        if any(0x01 <= ord(c) <= 0x1F for c in clean_name):
            continue
        # Check that name contains at least one letter (alpha)
        has_letter = any(c.isalpha() for c in clean_name)
        if not has_letter:
            continue
        # Filter out names that are all the same character (like "яяяяяяяяяяяя")
        if len(set(clean_name)) == 1:
            continue
        # Reject names with too many non-alpha characters (< 50% alpha)
        alpha_count = sum(1 for c in clean_name if c.isalpha())
        if alpha_count < len(clean_name) * 0.5:
            continue

        # ⭐ COORDINATE validation
        # Coords are 2-byte LE (u16) at offsets -26 (X), -24 (Y), -22 (Z)
        # Pool heroes have coords (-1, -1, -1) = 0xFFFF
        # Active heroes have coords (0-143, 0-143, 0-1)
        coord_x = struct.unpack('<h', raw[i-26:i-24])[0]  # signed for -1 check
        coord_y = struct.unpack('<h', raw[i-24:i-22])[0]
        coord_z = struct.unpack('<h', raw[i-22:i-20])[0]
        if faction <= 7:  # active hero
            if not (-1 <= coord_x <= 255) or not (-1 <= coord_y <= 255) or not (-1 <= coord_z <= 1):
                continue

        # Avoid duplicates — only skip if within 400 bytes of a REAL hero
        if i in seen_offsets:
            continue
        for j in range(i, min(i + 400, len(raw))):
            seen_offsets.add(j)

        # Parse all fields
        hero = {"offset": i, "name": name, "fields": {}}

        for field_name, field_off in HERO_BYTE_POSITIONS.items():
            abs_off = i + field_off
            if field_name in ("location_x", "location_y", "location_z"):
                # Coords are 2-byte LE (u16)
                val = read_u16_le(raw, abs_off)
                hero["fields"][field_name] = (val, str(val) if val is not None else "?")
            elif field_name in ("on_map",
                              "faction", "level", "attack", "defense", "power", "knowledge"):
                val = read_u8(raw, abs_off)
                if field_name == "faction":
                    hero["fields"][field_name] = (val, format_player_color(val) if val is not None else "?")
                else:
                    hero["fields"][field_name] = (val, str(val) if val is not None else "?")
            elif field_name in ("movement_total", "movement_left", "experience", "skills_count"):
                val = read_u32_le(raw, abs_off)
                hero["fields"][field_name] = (val, str(val) if val is not None else "?")
            elif field_name == "mana_left":
                val = read_u16_le(raw, abs_off)
                hero["fields"][field_name] = (val, str(val) if val is not None else "?")
            elif field_name == "hero_name":
                val = read_cp1251(raw, abs_off, 13)
                hero["fields"][field_name] = (val, repr(val) if val else "?")
            elif field_name in ("army_types", "army_counts"):
                slots = []
                for k in range(7):
                    v = read_u32_le(raw, abs_off + k * 4)
                    if v is not None and v < 0x80000000:
                        slots.append(v)
                    else:
                        slots.append(-1)
                hero["fields"][field_name] = (slots, str(slots))
            elif field_name in ("spells_book", "spells_available"):
                if 0 <= abs_off + 70 <= len(raw):
                    bits_set = sum(bin(b).count("1") for b in raw[abs_off:abs_off+70])
                    hero["fields"][field_name] = (bits_set, f"{bits_set} spells")
                else:
                    hero["fields"][field_name] = (None, "?")
            elif field_name in ("skill_levels", "skills_slot"):
                size = 30
                if 0 <= abs_off + size <= len(raw):
                    vals = list(raw[abs_off:abs_off+size])
                    hero["fields"][field_name] = (vals, str(vals[:10]) + "...")
                else:
                    hero["fields"][field_name] = (None, "?")
            elif field_name == "inventory":
                hero["fields"][field_name] = ("512 bytes", "64 slots")
            else:
                val = read_u32_le(raw, abs_off)
                if val is not None and val >= 0x80000000:
                    hero["fields"][field_name] = (val, "empty")
                elif val is not None:
                    hero["fields"][field_name] = (val, f"ID={val}")
                else:
                    hero["fields"][field_name] = (None, "?")

        heroes.append(hero)

    return heroes


# ============================================================================
# TOWN BLOCK PARSER (h3sed-style)
# ============================================================================

def find_town_blocks(raw: bytes, max_pos: int = None,
                      scan_start: int = None) -> List[Dict]:
    """
    Ищет town blocks через h3sed TOWN_REGEX.

    Args:
        raw:        decompressed save bytes
        max_pos:    upper bound for scan (default: len(raw))
        scan_start: byte offset to start scanning. If None, use header_size
                    from header_parser (no hardcoded 30000).
    """
    towns = []
    if scan_start is None:
        try:
            from header_parser import parse_header
            scan_start = parse_header(raw).header_size
        except Exception:
            scan_start = 0x400  # safe fallback
    scan_start = max(scan_start, 0x100)
    pos = scan_start
    if max_pos is None:
        max_pos = len(raw)

    while pos < max_pos:
        m = TOWN_REGEX.search(raw[pos:max_pos])
        if not m:
            break

        town_start = pos + m.start()
        if town_start + 71 > len(raw):
            break

        # Parse fields
        faction = raw[town_start + TOWN_BYTE_POSITIONS["faction"]]
        town_type = raw[town_start + 3]
        loc_x = raw[town_start + TOWN_BYTE_POSITIONS["location_x"]]
        loc_y = raw[town_start + TOWN_BYTE_POSITIONS["location_y"]]
        loc_z = raw[town_start + TOWN_BYTE_POSITIONS["location_z"]]

        # Army
        army_types = []
        army_counts = []
        for i in range(7):
            t = read_u32_le(raw, town_start + 9 + i * 4)
            c = read_u32_le(raw, town_start + 37 + i * 4)
            army_types.append(t if t and t < 0x80000000 else -1)
            army_counts.append(c if c else 0)

        # Name
        name_len = read_u16_le(raw, town_start + 69)
        if name_len and 0 < name_len <= 14:
            name = read_cp1251(raw, town_start + 71, name_len)
        else:
            name = "?"

        # Validate: skip if faction is invalid AND type is invalid
        if faction not in range(8) and faction != 255:
            pos = town_start + 1
            continue
        if town_type > 11:
            pos = town_start + 1
            continue

        # Check if army is valid (all empty or has valid IDs)
        has_creatures = any(t >= 0 for t in army_types)
        if has_creatures:
            # Verify at least one creature count > 0
            if not any(c > 0 for c in army_counts):
                pos = town_start + 1
                continue

        town = {
            "offset": town_start,
            "faction": faction,
            "faction_name": format_player_color(faction),
            "type": town_type,
            "type_name": format_town_type(town_type),
            "location": (loc_x, loc_y, loc_z),
            "army_types": army_types,
            "army_counts": army_counts,
            "name": name,
            "name_len": name_len,
        }
        towns.append(town)
        pos = town_start + max(71 + (name_len or 0), 100)

    return towns


# ============================================================================
# PATH RECORDS PARSER
# ============================================================================

PATH_RECORD_TYPES = {
    "01 03": {"name": "hero_movement", "size": 15},
    "03 03": {"name": "capture_event", "size": 9},
    "04 03": {"name": "battle_event_town", "size": 10},
    "06 03": {"name": "disembark_subrecord", "size": "variable"},
    "08 03": {"name": "battle_outcome_or_army_transfer", "size": "9 or 27"},
    "09 03": {"name": "visit_event", "size": 19},
    "0b 03": {"name": "fog_of_war_update", "size": "variable"},
}


def parse_path_records(raw: bytes, start: int, max_size: int) -> List[Dict]:
    records = []
    pos = start
    end = min(start + max_size, len(raw)) if max_size > 0 else len(raw)

    # Skip leading zeros — find first valid record
    valid_keys = set(PATH_RECORD_TYPES.keys())
    found_start = -1
    for scan_pos in range(start, min(start + 1024, end - 2)):
        type_key = f"{raw[scan_pos]:02x} {raw[scan_pos+1]:02x}"
        if type_key in valid_keys:
            found_start = scan_pos
            break
    if found_start < 0:
        return []
    pos = found_start

    while pos + 2 <= end:
        flag1, flag2 = raw[pos], raw[pos + 1]
        if flag1 == 0x00 and flag2 == 0x00:
            if pos + 8 <= end and raw[pos:pos + 8] == b"\x00" * 8:
                break
            pos += 1
            continue

        type_key = f"{flag1:02x} {flag2:02x}"
        type_def = PATH_RECORD_TYPES.get(type_key)
        if type_def is None:
            pos += 1
            continue

        size_str = type_def["size"]
        if size_str == "variable":
            if pos + 4 > end:
                break
            n = raw[pos + 2]
            size = 4 + n * 8
        elif isinstance(size_str, str) and "or" in size_str:
            if pos + 9 <= end and raw[pos + 8] == 0x0b:
                size = 9
            else:
                size = 27
        elif isinstance(size_str, str):
            size = 15
        else:
            size = int(size_str)

        if pos + size > end:
            break

        record = {
            "type": type_key,
            "type_name": type_def["name"],
            "offset": pos,
            "size": size,
            "raw": raw[pos:pos + size].hex(),
            "fields": {},
        }

        # Parse 01 03 (hero movement)
        if type_key == "01 03" and size >= 15:
            record["fields"]["counter"] = read_u32_le(raw, pos + 2)
            record["fields"]["step_n"] = read_u8(raw, pos + 6)
            record["fields"]["from_x"] = read_u16_le(raw, pos + 7)
            record["fields"]["from_y"] = read_u16_le(raw, pos + 9)
            record["fields"]["to_x"] = read_u16_le(raw, pos + 11)
            record["fields"]["to_y"] = read_u16_le(raw, pos + 13)

        # Parse 09 03 (visit event)
        elif type_key == "09 03" and size >= 15:
            record["fields"]["counter"] = read_u32_le(raw, pos + 2)
            record["fields"]["n"] = read_u8(raw, pos + 6)
            record["fields"]["from_x"] = read_u16_le(raw, pos + 7)
            record["fields"]["from_y"] = read_u16_le(raw, pos + 9)
            record["fields"]["to_x"] = read_u16_le(raw, pos + 11)
            record["fields"]["to_y"] = read_u16_le(raw, pos + 13)

        # Parse 03 03 (capture event)
        elif type_key == "03 03" and size >= 9:
            record["fields"]["counter"] = read_u32_le(raw, pos + 2)
            record["fields"]["prev_owner"] = read_u8(raw, pos + 6)

        # Parse 08 03 (battle outcome, compact 9b)
        elif type_key == "08 03" and size == 9:
            record["fields"]["counter"] = read_u32_le(raw, pos + 2)
            record["fields"]["result"] = read_u8(raw, pos + 6)
            record["fields"]["outcome"] = read_u8(raw, pos + 7)

        # Parse 0b 03 (fog of war)
        elif type_key == "0b 03":
            n = raw[pos + 2]
            record["fields"]["n"] = n
            pairs = []
            for i in range(n):
                pair_off = pos + 4 + i * 4
                if pair_off + 4 <= end:
                    x = read_u16_le(raw, pair_off)
                    y = read_u16_le(raw, pair_off + 2)
                    pairs.append((x, y))
            record["fields"]["pairs"] = pairs

        records.append(record)
        pos += size

    return records


# ============================================================================
# MAPPING-BASED PARSER
# ============================================================================

def read_field(raw: bytes, offset: int, size: int, type_: str,
               values=None, array_count=None, bits=None) -> Tuple[Any, str]:
    if offset < 0:
        offset = len(raw) + offset
    if offset + size > len(raw) or offset < 0:
        return None, f"<out of range: offset=0x{offset:x}, size={size}>"

    chunk = raw[offset:offset + size]

    if type_ == "ascii":
        s = chunk.rstrip(b"\x00").decode("ascii", errors="replace")
        return s, repr(s)
    if type_ == "cp1251":
        s = chunk.rstrip(b"\x00").decode("cp1251", errors="replace")
        return s, repr(s)
    if type_ in ("u8", "u8_hex"):
        v = chunk[0]
        return v, f"0x{v:02x} ({v})" if type_ == "u8_hex" else str(v)
    if type_ == "u8_enum":
        v = chunk[0]
        if isinstance(values, dict):
            label = values.get(str(v), values.get(v, f"0x{v:02x}"))
            return v, f"{v} = {label}"
        return v, f"0x{v:02x} ({v})"
    if type_ == "u8_bool":
        v = chunk[0]
        return bool(v), "true" if v else "false"
    if type_ == "u8_bitmask":
        v = chunk[0]
        bits_set = []
        if bits:
            for bit_str, label in bits.items():
                bit = int(bit_str)
                if v & (1 << bit):
                    bits_set.append(f"bit{bit}={label}")
        return v, f"0x{v:02x} [{', '.join(bits_set) if bits_set else 'none'}]"
    if type_ == "u8_pair":
        return (chunk[0], chunk[1]), f"({chunk[0]}, {chunk[1]})"
    if type_ == "u16_le":
        v = struct.unpack("<H", chunk[:2])[0]
        return v, f"{v} (0x{v:04x})"
    if type_ == "u16_le_pair":
        x, y = struct.unpack("<HH", chunk[:4])
        return (x, y), f"({x}, {y})"
    if type_ == "u32_le":
        v = struct.unpack("<I", chunk[:4])[0]
        if v >= 0x80000000:
            return v, f"{v} (0x{v:08x}) [-1 / empty]"
        return v, f"{v} (0x{v:08x})"
    if type_ == "u32_le_pair":
        x, y = struct.unpack("<II", chunk[:8])
        x_s = str(x) if x < 0x80000000 else "-1"
        y_s = str(y) if y < 0x80000000 else "-1"
        return (x, y), f"({x_s}, {y_s})"
    if type_ == "u32_le_array":
        if array_count is None:
            array_count = size // 4
        vals = list(struct.unpack(f"<{array_count}I", chunk[:array_count * 4]))
        formatted = []
        for i, v in enumerate(vals):
            if v >= 0x80000000:
                formatted.append(f"[{i}]=-1(empty)")
            else:
                formatted.append(f"[{i}]={v}")
        return vals, ", ".join(formatted)
    if type_ == "bytes":
        return chunk, chunk.hex()
    if type_ == "bit_array":
        bits_set = sum(bin(b).count("1") for b in chunk)
        return chunk, f"<bit array {size}b, {bits_set} bits set>"
    if type_ == "path_records":
        return None, f"<path records, size={size}>"
    return chunk, f"<unknown type {type_}>"


def parse_save(raw: bytes, mapping: Dict = None,
                map_config: Any = None) -> Dict:
    """
    Parse a decompressed .GM1 save.

    v3.0 architecture:
      - If `map_config` (a MapConfig) is provided, uses save_parser.parse_save()
        (Phase 3) — universal, no hardcoded offsets.
      - If only `mapping` (gm1_mapping.json) is provided, falls back to the
        legacy path that reads mapping["blocks"] (only path_block is universal
        now; other blocks were removed in v3.0).
      - If neither is provided, returns a minimal ParsedSave with only
        header + heroes_found + towns_found (no blocks).

    Args:
        raw:        decompressed save bytes
        mapping:    gm1_mapping.json dict (legacy; only `path_block` and
                    `constants` are now universal — `blocks` was removed in v3.0)
        map_config: MapConfig object (preferred — from Phase 2 = map_config_builder)

    Returns a dict with the same shape as before (for _populate_tree):
      {
        "file_info": {...},
        "blocks": [...],
        "path_records": [...],
        "heroes_found": [...],
        "towns_found": [...],
        "errors": [...],
      }
    """
    # ----- Path 1: use save_parser.parse_save() if MapConfig is provided -----
    if map_config is not None:
        return _parse_save_via_config(raw, map_config)

    # ----- Path 2: legacy / minimal parse -----
    return _parse_save_legacy(raw, mapping or {})


def _parse_save_via_config(raw: bytes, map_config: Any) -> Dict:
    """Parse via save_parser.parse_save() (Phase 3) — universal, no hardcoded offsets.

    Converts the ParsedSave dataclass to the dict format expected by
    _populate_tree() and export_to_json().
    """
    # Import save_parser lazily (so gm1_parser.py doesn't depend on it at module load)
    try:
        from save_parser import (
            parse_save as sp_parse_save,
            parse_hero_block as sp_parse_hero_block,
            parse_town_block as sp_parse_town_block,
            adapt_config_to_save,
            parsed_save_to_dict,
            PLAYER_COLOR_NAMES, TOWN_TYPE_NAMES,
        )
    except ImportError:
        import importlib.util
        tools_dir = os.path.dirname(os.path.abspath(__file__))
        spec = importlib.util.spec_from_file_location(
            "save_parser", os.path.join(tools_dir, "save_parser.py"))
        mod = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(mod)
        sp_parse_save = mod.parse_save
        sp_parse_hero_block = mod.parse_hero_block
        sp_parse_town_block = mod.parse_town_block
        adapt_config_to_save = mod.adapt_config_to_save
        parsed_save_to_dict = mod.parsed_save_to_dict
        PLAYER_COLOR_NAMES = mod.PLAYER_COLOR_NAMES
        TOWN_TYPE_NAMES = mod.TOWN_TYPE_NAMES

    # Run Phase 3 parse
    parsed = sp_parse_save(raw, map_config)

    # Build the dict format for _populate_tree
    # ----- file_info -----
    file_info = {
        "raw_size":      parsed.header.raw_size,
        "magic":         parsed.header.magic,
        "version_major": parsed.header.version_major,
        "version_minor": parsed.header.version_minor,
        "map_name":      parsed.header.map_name,
        "map_filename":  parsed.header.map_filename,
        "save_filename": parsed.header.save_filename,
        "header_size":   parsed.header.header_size,
        "has_underground": parsed.header.has_underground,
        "map_size":      parsed.header.map_size,
    }

    # ----- blocks: header + per-cluster -----
    blocks = []
    # Header block
    header_block = {
        "name":        "header",
        "description": "Save header (H3SVG magic, version, map name, ...)",
        "fields": [
            {"name": "magic", "offset": 0x0, "size": 5, "type": "ascii",
             "value": parsed.header.magic, "formatted": parsed.header.magic,
             "description": "Magic bytes (H3SVG or H3SVC)"},
            {"name": "version_major", "offset": 0x08, "size": 4, "type": "u32",
             "value": parsed.header.version_major,
             "formatted": f"0x{parsed.header.version_major:02X}",
             "description": "Save version major (0x2A = 42 = SoD/HotA)"},
            {"name": "version_minor", "offset": 0x0C, "size": 4, "type": "u32",
             "value": parsed.header.version_minor,
             "formatted": f"0x{parsed.header.version_minor:02X}"},
            {"name": "map_type", "offset": 0x30, "size": 4, "type": "u32",
             "value": parsed.header.map_type, "formatted": str(parsed.header.map_type),
             "description": "28 = SoD"},
            {"name": "has_underground", "offset": 0x34, "size": 1, "type": "u8",
             "value": int(parsed.header.has_underground),
             "formatted": str(int(parsed.header.has_underground))},
            {"name": "map_size", "offset": 0x35, "size": 4, "type": "u32",
             "value": parsed.header.map_size,
             "formatted": str(parsed.header.map_size)},
            {"name": "map_name", "offset": 0x3C,
             "size": len(parsed.header.map_name.encode("cp1251", errors="replace")),
             "type": "cp1251", "value": parsed.header.map_name,
             "formatted": parsed.header.map_name},
            {"name": "map_filename", "offset": 0,
             "size": len(parsed.header.map_filename),
             "type": "ascii", "value": parsed.header.map_filename,
             "formatted": parsed.header.map_filename},
            {"name": "save_filename", "offset": 0,
             "size": len(parsed.header.save_filename),
             "type": "ascii", "value": parsed.header.save_filename,
             "formatted": parsed.header.save_filename},
            {"name": "header_size", "offset": 0, "size": 0, "type": "int",
             "value": parsed.header.header_size,
             "formatted": f"0x{parsed.header.header_size:X}",
             "description": "Byte offset where game-state sections begin"},
        ],
    }
    blocks.append(header_block)

    # Per-cluster blocks (from parsed.blocks[1:] — first is "header")
    for cluster_block in parsed.blocks[1:]:
        if cluster_block.name.startswith("cluster:"):
            cname = cluster_block.name.split(":", 1)[1]
            blocks.append({
                "name":        f"cluster:{cname}",
                "description":  cluster_block.description,
                "fields": [
                    {"name": "start", "offset": cluster_block.start, "size": 0,
                     "type": "int", "value": cluster_block.start,
                     "formatted": f"0x{cluster_block.start:X}"},
                    {"name": "end", "offset": cluster_block.end, "size": 0,
                     "type": "int", "value": cluster_block.end,
                     "formatted": f"0x{cluster_block.end:X}"},
                ],
            })

    # ----- heroes_found (convert to legacy dict format) -----
    heroes_found = []
    for h in parsed.heroes:
        f = h.get("fields", {})
        # Convert to legacy format: name, offset, fields dict with (val, formatted) tuples
        hero_dict = {
            "name":   h.get("name") or f.get("name", "?"),
            "offset": h["block_offset"],
        }
        # Build fields dict — legacy expected {field_name: (value, formatted_str)}
        hero_fields_dict = {}
        for fname in ["name", "player", "player_name", "level", "experience",
                       "movement_total", "movement_left", "mana_left",
                       "location_x", "location_y", "location_z",
                       "attack", "defense", "power", "knowledge",
                       "num_skills"]:
            if fname in f:
                v = f[fname]
                hero_fields_dict[fname] = (v, str(v))
        # Army, skills, spells, equipment as summary lines
        if "army_types" in f:
            hero_fields_dict["army_types"] = (f["army_types"], str(f["army_types"]))
        if "army_counts" in f:
            hero_fields_dict["army_counts"] = (f["army_counts"], str(f["army_counts"]))
        if "skill_levels" in f:
            hero_fields_dict["skill_levels"] = (f["skill_levels"], f"<{sum(1 for x in f['skill_levels'] if x)} skills>")
        if "equipment" in f:
            equipped = sum(1 for aid, _ in f["equipment"] if aid != 0xFFFFFFFF)
            hero_fields_dict["equipment"] = (f["equipment"], f"<{equipped} equipped>")
        hero_dict["fields"] = hero_fields_dict
        heroes_found.append(hero_dict)

    # ----- towns_found (convert to legacy dict format) -----
    towns_found = []
    for t in parsed.towns:
        f = t.get("fields", {})
        towns_found.append({
            "name":         t.get("name") or f.get("name", "?"),
            "offset":       t["block_offset"],
            "faction":      f.get("faction", 255),
            "faction_name": f.get("faction_name", "?"),
            "type":         f.get("type", 0),
            "type_name":    f.get("type_name", "?"),
            "location":     (f.get("x", 0), f.get("y", 0), f.get("z", 0)),
            "army_types":   f.get("army_types", [-1]*7),
            "army_counts":  f.get("army_counts", [0]*7),
        })

    # ----- object_offsets (from parsed.objects_on_map) -----
    # Convert to the format used by _compute_object_offsets_in_save
    object_offsets = {}
    for o in parsed.objects_on_map:
        ci = o["coord_int"]
        def h(v): return f"0x{v:X}" if v is not None else None
        object_offsets[str(ci)] = {
            "main_offset":       h(o.get("main_offset")),
            "visiting_offset":   h(o.get("visiting_offset")),
            "fog_offset":        h(o.get("fog_offset")),
            "alive_offset":      h(o.get("alive_offset")),
            "treasure_offset":   h(o.get("treasure_offset")),
            "decoration_offset": h(o.get("decoration_offset")),
            "save_offsets":      [f"0x{x:X}" for x in o.get("save_offsets", [])],
            "verified":          bool(o.get("save_offsets")),
        }

    return {
        "file_info":       file_info,
        "blocks":          blocks,
        "path_records":    [],  # Path records not yet implemented in save_parser
        "heroes_found":    heroes_found,
        "towns_found":     towns_found,
        "errors":          [],
        "object_offsets":  object_offsets,  # bonus: from Phase 3
        "map_config_meta": parsed.header.to_dict(),  # bonus: header info
        "map_objects":     getattr(parsed, "_map_objects", []),  # tile scanner results
        "map_start_info":  getattr(parsed, "_map_start_info", {}),  # find_map_start debug
        "player_states":   getattr(parsed, "_player_states", []),
        "current_state":   getattr(parsed, "_current_state", {}),
        "post_tile_sections": getattr(parsed, "_post_tile_sections", {}),  # structure-walking chain
    }


def _parse_save_legacy(raw: bytes, mapping: Dict) -> Dict:
    """Legacy parse path — used when no MapConfig is provided.

    Uses gm1_mapping.json (only `path_block` is universal now; `blocks`
    was removed in v3.0). Always parses header + heroes + towns using
    the universal find_hero_blocks / find_town_blocks.
    """
    # Parse map name from header (at offset 0x3A: 2-byte len + name bytes, cp1251)
    map_name = ""
    try:
        name_len = struct.unpack("<H", raw[0x3A:0x3C])[0]
        if 0 < name_len < 256:
            map_name = raw[0x3C:0x3C + name_len].decode("cp1251", errors="replace")
    except Exception:
        pass

    result = {
        "file_info": {
            "raw_size": len(raw),
            "magic": raw[:5].decode("ascii", errors="replace"),
            "version_major": raw[8],
            "version_minor": raw[12],
            "map_name": map_name,
        },
        "blocks": [],
        "path_records": [],
        "heroes_found": [],
        "towns_found": [],
        "errors": [],
    }

    # Parse path_block from mapping (universal — only this block remains)
    path_record_types = {}
    path_block_def = mapping.get("path_block") or {}
    if isinstance(path_block_def, dict):
        path_record_types = path_block_def.get("path_record_types", {})
    # Also check legacy `blocks` array (still supported if user has old mapping)
    for b in mapping.get("blocks", []):
        if b.get("name") == "path_block":
            path_record_types = b.get("path_record_types", {})
            break

    # If there's a `path_block` with explicit start offset, parse it.
    # In v3.0 we don't have absolute offsets, so skip path record parsing
    # unless mapping provides a path_block with `fields` containing offset.
    path_block_fields = path_block_def.get("fields", []) if isinstance(path_block_def, dict) else []
    for fdef in path_block_fields:
        if fdef.get("type") == "path_records":
            foff = parse_offset(fdef.get("offset", -1))
            if foff >= 0 and foff < len(raw):
                max_size = len(raw) - foff - 18
                records = parse_path_records(raw, foff, max_size)
                result["path_records"].extend(records)
                result["blocks"].append({
                    "name": "path_block",
                    "description": path_block_def.get("description", ""),
                    "fields": [{
                        "name": fdef.get("name", "path_records"),
                        "offset": foff, "size": -1,
                        "type": "path_records", "value": None,
                        "formatted": f"<{len(records)} path records>",
                        "description": fdef.get("description", ""),
                        "path_records": records,
                    }],
                })
            break

    # Find heroes using universal find_hero_blocks
    result["heroes_found"] = find_hero_blocks(raw)

    # Find towns using universal find_town_blocks
    result["towns_found"] = find_town_blocks(raw)

    return result


def export_to_json(parsed: Dict, raw: bytes,
                   objects_by_coord: Dict = None,
                   coord_int_lookup: Dict = None,
                   obj_dict: Dict = None,
                   computed_offsets: Dict = None) -> str:
    def serialize(obj):
        if isinstance(obj, (bytes, bytearray)):
            return obj.hex()
        if isinstance(obj, dict):
            return {k: serialize(v) for k, v in obj.items()}
        if isinstance(obj, list):
            return [serialize(v) for v in obj]
        if isinstance(obj, tuple):
            return list(serialize(v) for v in obj)
        return obj

    # Use data passed by caller (from loaded Map JSON + save scan)
    # NO loading from static files
    objects_on_map = objects_by_coord or {}
    coord_lookup = coord_int_lookup or {}
    obj_dict = obj_dict  # wiki dictionary (still loaded from file, it's universal)
    objects_with_offsets = computed_offsets or {}

    # Load wiki dictionary if not provided (it's universal, not map-specific)
    if not obj_dict:
        script_dir = os.path.dirname(os.path.abspath(__file__))
        obj_map_dir = os.path.normpath(os.path.join(script_dir, "..", "03_object_mapping"))
        obj_dict_path = os.path.join(obj_map_dir, "object_types_dictionary.json")
        if os.path.exists(obj_dict_path):
            try:
                with open(obj_dict_path, "r", encoding="utf-8") as f:
                    obj_dict = json.load(f)
            except Exception:
                pass

    # ----- Attach object list -----
    # Build flat list from objects_on_map (loaded by caller, not from file)
    flat_path = ""  # no static file
    objects_flat = None
    # Always build on-the-fly from objects_on_map (no static file loading)

    if objects_on_map:
        # Compact list of all 6006 primary objects with overlays
        objects_list = []
        for ck, rec in objects_on_map.items():
            ci_str = str(rec["coord_int"])
            offs = (objects_with_offsets or {}).get(ci_str, {})
            objects_list.append({
                "coord_key":   ck,
                "coord_int":   rec["coord_int"],
                "x":           rec["x"],
                "y":           rec["y"],
                "z":           rec["z"],
                "type":        rec.get("type", ""),
                "category":    rec.get("category", ""),
                "sprite_def":  rec.get("sprite_def", ""),
                "details":     rec.get("details", {}),
                "overlays":    rec.get("overlays", []),
                # Save-file offsets (per-object addresses in decompressed .GM1)
                "save_offsets":      offs.get("save_offsets",      []),
                "main_offset":       offs.get("main_offset",       None),
                "visiting_offset":   offs.get("visiting_offset",   None),
                "fog_offset":        offs.get("fog_offset",        None),
                "alive_offset":      offs.get("alive_offset",      None),
                "treasure_offset":   offs.get("treasure_offset",   None),
                "decoration_offset": offs.get("decoration_offset", None),
                "verified":          offs.get("verified", False),
            })
        # Sort by coord_int for stable ordering
        objects_list.sort(key=lambda o: o["coord_int"])

        # Build flat list on-the-fly if not loaded from file
        if objects_flat is None:
            objects_flat = []
            for rec in objects_list:
                objects_flat.append({
                    "coord_key":    rec["coord_key"],
                    "coord_int":    rec["coord_int"],
                    "x":            rec["x"],
                    "y":            rec["y"],
                    "z":            rec["z"],
                    "object_index": None,  # not available here without re-loading
                    "sprite_def":   rec["sprite_def"],
                    "type":         rec["type"],
                    "category":     rec["category"],
                    "details":      rec["details"],
                    "is_primary":   True,
                    # offsets (same as primary)
                    "save_offsets":      rec["save_offsets"],
                    "main_offset":       rec["main_offset"],
                    "visiting_offset":   rec["visiting_offset"],
                    "fog_offset":        rec["fog_offset"],
                    "alive_offset":      rec["alive_offset"],
                    "treasure_offset":   rec["treasure_offset"],
                    "decoration_offset": rec["decoration_offset"],
                    "verified":          rec["verified"],
                })
                for ovl in rec.get("overlays", []):
                    # Overlays share the same coord_int as primary, so they
                    # share the same save_offsets.
                    objects_flat.append({
                        "coord_key":    rec["coord_key"],
                        "coord_int":    rec["coord_int"],
                        "x":            rec["x"],
                        "y":            rec["y"],
                        "z":            rec["z"],
                        "object_index": ovl.get("object_index"),
                        "sprite_def":   ovl.get("sprite_def", ""),
                        "type":         ovl.get("type", ""),
                        "category":     ovl.get("category", ""),
                        "details":      ovl.get("details", {}),
                        "is_primary":   False,
                        # Offsets are tile-level, so same as primary
                        "save_offsets":      rec["save_offsets"],
                        "main_offset":       rec["main_offset"],
                        "visiting_offset":   rec["visiting_offset"],
                        "fog_offset":        rec["fog_offset"],
                        "alive_offset":      rec["alive_offset"],
                        "treasure_offset":   rec["treasure_offset"],
                        "decoration_offset": rec["decoration_offset"],
                        "verified":          rec["verified"],
                    })
            objects_flat.sort(key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1))

        n_primary = len(objects_list)
        n_overlay = len(objects_flat) - n_primary
    else:
        n_primary = 0
        n_overlay = 0
        objects_list = []
        objects_flat = []

    export_data = {
        "file_info": parsed["file_info"],
        "summary": {
            "total_blocks": len(parsed["blocks"]),
            "total_path_records": len(parsed["path_records"]),
            "heroes_found": len(parsed["heroes_found"]),
            "towns_found": len(parsed["towns_found"]),
            "errors_count": len(parsed["errors"]),
            "objects_total":         n_primary + n_overlay,
            "objects_primary":       n_primary,
            "objects_overlay":       n_overlay,
            "objects_unique_coords": n_primary,
            "wiki_object_types":     len(obj_dict.get("objects", {})) if obj_dict else 0,
        },
        "blocks": [],
        "path_records": serialize(parsed["path_records"]),
        "heroes_found": serialize(parsed["heroes_found"]),
        "towns_found": serialize(parsed["towns_found"]),
        "errors": parsed["errors"],
    }

    if objects_on_map:
        export_data["objects_on_map"] = {
            "_meta": {
                "map_name":          parsed["file_info"].get("map_name", "(unknown)"),
                "total_objects":     len(objects_flat),
                "primary_objects":   n_primary,
                "overlay_objects":   n_overlay,
                "unique_coords":     n_primary,
                "source_files": {
                    "by_coord": "03_object_mapping/objects_by_coord.json",
                    "flat":     "03_object_mapping/objects_flat.json",
                },
                "coord_encoding":    "coord_int = x | (y << 8) | (z << 16)",
                "notes": [
                    "objects_by_coord: one entry per (x,y,z), additional objects on the same tile in 'overlays[]'.",
                    "objects_flat:     plain list of ALL 7009 objects, each with its own is_primary flag.",
                    "Use 'objects_by_coord' for tile lookups by coord_int; use 'objects_flat' for bulk iteration.",
                ],
            },
            "coord_int_lookup": coord_lookup or {},
            "objects_by_coord": objects_list,    # 6006 primary records, with overlays[]
            "objects_flat":     objects_flat,    # 7009 flat records
        }
    else:
        export_data["objects_on_map"] = {
            "_meta": {
                "map_name":      parsed["file_info"].get("map_name", "(unknown)"),
                "total_objects": 0,
                "source_file":   "03_object_mapping/objects_by_coord.json",
                "error":         "object mapping directory not found. Place "
                                 "03_object_mapping/ next to 01_tools/gm1_parser.py.",
            },
            "coord_int_lookup": {},
            "objects_by_coord": [],
            "objects_flat":     [],
        }

    # ----- Attach wiki object-type dictionary (compact, name -> entry) -----
    if obj_dict:
        export_data["object_types_dictionary"] = {
            "_meta":         obj_dict.get("_meta", {}),
            "_categories":   obj_dict.get("_categories", {}),
            "_name_index":   obj_dict.get("_name_index", {}),
            "_id_index":     obj_dict.get("_id_index", {}),
            "objects":       obj_dict.get("objects", {}),
            "source_file":   "03_object_mapping/object_types_dictionary.json",
        }

    for block in parsed["blocks"]:
        block_export = {"name": block["name"], "description": block["description"], "fields": []}
        for f in block["fields"]:
            field_export = {
                "name": f["name"],
                "offset": f"0x{f['offset']:08x}" if isinstance(f["offset"], int) and f["offset"] >= 0 else f["offset"],
                "size": f["size"], "type": f["type"],
                "value": serialize(f.get("value")),
                "formatted": f["formatted"], "description": f["description"],
            }
            if isinstance(f["offset"], int) and f["offset"] >= 0 and isinstance(f["size"], int) and f["size"] > 0:
                off, sz = f["offset"], f["size"]
                if off + sz <= len(raw):
                    field_export["raw_bytes"] = raw[off:off + sz].hex()
            block_export["fields"].append(field_export)
        export_data["blocks"].append(block_export)

    return json.dumps(export_data, indent=2, ensure_ascii=False)


def export_objects_to_excel(parsed: Dict, raw: bytes, output_path: str,
                            objects_by_coord: Dict = None,
                            computed_offsets: Dict = None) -> str:
    """
    Export all 7009 map objects to an Excel workbook with multiple sheets:
      - All Objects       (flat list of 7009)
      - By Category       (grouped by 21 categories)
      - Summary           (counts per type/category)
      - Wiki Dictionary   (2037 wiki types from object_types_dictionary.json)

    Returns the path written.
    """
    try:
        from openpyxl import Workbook
        from openpyxl.styles import Font, PatternFill, Alignment, Border, Side
        from openpyxl.utils import get_column_letter
    except ImportError:
        raise RuntimeError(
            "openpyxl is required for Excel export. Install with: pip install openpyxl"
        )

    # ----- Use data from caller (no static file loading) -----
    script_dir = os.path.dirname(os.path.abspath(__file__))
    obj_map_dir = os.path.normpath(os.path.join(script_dir, "..", "03_object_mapping"))
    oc = objects_by_coord or {}

    if not oc:
        raise RuntimeError(
            "No map objects loaded. Load a Map JSON first (Ctrl+M)."
        )

    # Build flat list on-the-fly from objects_by_coord
    flat_objects = []
    for ck, rec in oc.items():
        flat_objects.append({
            "coord_key":    rec["coord_key"],
            "coord_int":    rec["coord_int"],
            "x":            rec["x"], "y": rec["y"], "z": rec["z"],
            "object_index": rec.get("object_index"),
            "sprite_ref_id": rec.get("sprite_ref_id"),
            "sprite_def":   rec.get("sprite_def", ""),
            "type":         rec.get("type", ""),
            "category":     rec.get("category", ""),
            "details":      rec.get("details", {}),
            "is_primary":   True,
        })
        for ovl in rec.get("overlays", []):
            flat_objects.append({
                "coord_key":    ck,
                "coord_int":    rec["coord_int"],
                "x":            rec["x"], "y": rec["y"], "z": rec["z"],
                "object_index": ovl.get("object_index"),
                "sprite_ref_id": ovl.get("sprite_ref_id"),
                "sprite_def":   ovl.get("sprite_def", ""),
                "type":         ovl.get("type", ""),
                "category":     ovl.get("category", ""),
                "details":      ovl.get("details", {}),
                "is_primary":   False,
            })
    flat_objects.sort(key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1))

    # ----- Load wiki dictionary (universal, not map-specific) + use passed offsets -----
    wiki_dict = None
    obj_dict_path = os.path.join(obj_map_dir, "object_types_dictionary.json")
    if os.path.exists(obj_dict_path):
        try:
            with open(obj_dict_path, "r", encoding="utf-8") as f:
                wiki_dict = json.load(f)
        except Exception:
            pass

    # Use offsets passed by caller (dynamically computed, not from file)
    objects_with_offsets = computed_offsets or {}

    # ----- Build workbook -----
    wb = Workbook()

    # Styling helpers
    header_font  = Font(bold=True, color="FFFFFF", size=11)
    header_fill  = PatternFill(start_color="305496", end_color="305496", fill_type="solid")
    header_align = Alignment(horizontal="center", vertical="center", wrap_text=True)
    thin_border  = Border(
        left=Side(style="thin", color="BBBBBB"),
        right=Side(style="thin", color="BBBBBB"),
        top=Side(style="thin", color="BBBBBB"),
        bottom=Side(style="thin", color="BBBBBB"),
    )

    def style_header(ws, row=1):
        for cell in ws[row]:
            cell.font = header_font
            cell.fill = header_fill
            cell.alignment = header_align
            cell.border = thin_border

    def autosize_columns(ws, max_widths=None):
        for col_cells in ws.columns:
            try:
                col_letter = get_column_letter(col_cells[0].column)
            except Exception:
                continue
            max_len = 0
            for cell in col_cells:
                if cell.value is None:
                    continue
                s = str(cell.value)
                if len(s) > max_len:
                    max_len = len(s)
            width = min(max(max_len + 2, 8), (max_widths or {}).get(col_letter, 60))
            ws.column_dimensions[col_letter].width = width

    # ----- Sheet 1: All Objects (flat 7009) -----
    ws1 = wb.active
    ws1.title = "All Objects"
    headers1 = [
        "#", "X", "Y", "Z", "coord_int", "coord_key", "is_primary",
        "object_index", "sprite_ref_id", "sprite_def",
        "type", "category",
        "details (JSON)",
        # Save-file offsets (per-object addresses in decompressed .GM1)
        "main_offset", "visiting_offset", "fog_offset",
        "alive_offset", "treasure_offset", "decoration_offset",
        "all_save_offsets", "verified",
        # Wiki info
        "wiki_name", "wiki_category", "wiki_object_id", "wiki_sub_id",
        "wiki_description",
    ]
    ws1.append(headers1)
    style_header(ws1)

    # Pre-build wiki lookup
    wiki_by_def = {}
    if wiki_dict:
        for fn, entry in wiki_dict.get("objects", {}).items():
            wiki_by_def[fn.lower()] = entry

    for i, o in enumerate(flat_objects, start=1):
        sd = (o.get("sprite_def") or "").lower()
        wiki = wiki_by_def.get(sd, {})
        # Get offsets from objects_with_offsets (by coord_int string)
        ci_str = str(o.get("coord_int", ""))
        offs = (objects_with_offsets or {}).get(ci_str, {})
        row = [
            i,
            o["x"], o["y"], o["z"],
            o["coord_int"], o["coord_key"],
            "YES" if o["is_primary"] else "no",
            o.get("object_index"),
            o.get("sprite_ref_id"),
            o.get("sprite_def", ""),
            o.get("type", ""),
            o.get("category", ""),
            json.dumps(o.get("details", {}), ensure_ascii=False)[:500],  # truncate long details
            # Save-file offsets
            offs.get("main_offset", "")       or "",
            offs.get("visiting_offset", "")   or "",
            offs.get("fog_offset", "")        or "",
            offs.get("alive_offset", "")      or "",
            offs.get("treasure_offset", "")   or "",
            offs.get("decoration_offset", "") or "",
            ", ".join(offs.get("save_offsets", []))[:300],
            "YES" if offs.get("verified") else "no",
            # Wiki info
            wiki.get("wiki_name", ""),
            wiki.get("category", ""),
            wiki.get("object_id", ""),
            wiki.get("sub_id", ""),
            (wiki.get("description", "") or "").replace("\n", " ")[:300],
        ]
        ws1.append(row)

    # Freeze top row + filter
    ws1.freeze_panes = "A2"
    ws1.auto_filter.ref = ws1.dimensions
    autosize_columns(ws1, max_widths={"M": 60, "T": 50, "Y": 80})

    # ----- Sheet 2: By Category (one row per object, grouped by category) -----
    ws2 = wb.create_sheet("By Category")
    headers2 = ["category", "#", "X", "Y", "Z", "type", "sprite_def",
                "is_primary", "object_index", "main_offset", "all_save_offsets", "details"]
    ws2.append(headers2)
    style_header(ws2)
    # Sort by category then coord_int
    sorted_by_cat = sorted(flat_objects, key=lambda o: (o["category"], o["coord_int"]))
    for o in sorted_by_cat:
        ci_str = str(o.get("coord_int", ""))
        offs = (objects_with_offsets or {}).get(ci_str, {})
        ws2.append([
            o["category"], "", o["x"], o["y"], o["z"],
            o["type"], o.get("sprite_def", ""),
            "YES" if o["is_primary"] else "no",
            o.get("object_index"),
            offs.get("main_offset", "") or "",
            ", ".join(offs.get("save_offsets", []))[:300],
            json.dumps(o.get("details", {}), ensure_ascii=False)[:300],
        ])
    ws2.freeze_panes = "A2"
    ws2.auto_filter.ref = ws2.dimensions
    autosize_columns(ws2, max_widths={"L": 60})

    # ----- Sheet 3: Summary (counts per type and category) -----
    ws3 = wb.create_sheet("Summary")
    from collections import Counter
    type_counter  = Counter(o["type"]   for o in flat_objects)
    cat_counter   = Counter(o["category"] for o in flat_objects)
    primary_count = sum(1 for o in flat_objects if o["is_primary"])
    overlay_count = sum(1 for o in flat_objects if not o["is_primary"])

    # Top section: meta info
    ws3.append(["Metric", "Value"])
    style_header(ws3)
    ws3.append(["Map name", parsed["file_info"].get("map_name", "(unknown)")])
    ws3.append(["Total objects (flat)", len(flat_objects)])
    ws3.append(["Primary objects", primary_count])
    ws3.append(["Overlay objects (stacked)", overlay_count])
    ws3.append(["Unique coordinates", len({o["coord_key"] for o in flat_objects})])
    ws3.append(["Distinct types", len(type_counter)])
    ws3.append(["Distinct categories", len(cat_counter)])
    ws3.append([])
    ws3.append([])

    # Categories breakdown
    ws3.append(["Category", "Count"])
    style_header(ws3, row=ws3.max_row)
    for cat, n in cat_counter.most_common():
        ws3.append([cat, n])
    ws3.append([])
    ws3.append([])

    # Types breakdown
    ws3.append(["Type", "Category", "Count"])
    style_header(ws3, row=ws3.max_row)
    type_to_cat = {o["type"]: o["category"] for o in flat_objects}
    for t, n in type_counter.most_common():
        ws3.append([t, type_to_cat.get(t, ""), n])

    autosize_columns(ws3, max_widths={"A": 50, "B": 30, "C": 12})

    # ----- Sheet 4: Wiki Dictionary (2037 types) -----
    if wiki_dict:
        ws4 = wb.create_sheet("Wiki Dictionary")
        headers4 = ["file_name", "wiki_name", "category", "object_id", "sub_id",
                    "generates_on", "restrictions", "layering", "description",
                    "user_notes", "user_category", "user_alias"]
        ws4.append(headers4)
        style_header(ws4)
        # Sort by category, then wiki_name
        sorted_wiki = sorted(wiki_dict.get("objects", {}).items(),
                             key=lambda kv: (kv[1].get("category", "").lower(),
                                             kv[1].get("wiki_name", "").lower()))
        for fn, entry in sorted_wiki:
            ue = entry.get("user_edits", {}) or {}
            ws4.append([
                entry.get("file_name", ""),
                entry.get("wiki_name", ""),
                entry.get("category", ""),
                entry.get("object_id", ""),
                entry.get("sub_id", ""),
                entry.get("generates_on", ""),
                entry.get("restrictions", ""),
                entry.get("layering", ""),
                (entry.get("description", "") or "").replace("\n", " ")[:500],
                ue.get("user_notes", ""),
                ue.get("user_category", ""),
                ue.get("user_alias", ""),
            ])
        ws4.freeze_panes = "A2"
        ws4.auto_filter.ref = ws4.dimensions
        autosize_columns(ws4, max_widths={"I": 80})

    # ----- Save -----
    wb.save(output_path)
    return output_path


def hex_dump(raw: bytes, start: int, length: int, prefix: str = "    ") -> str:
    if start < 0:
        start = len(raw) + start
    end = min(start + length, len(raw))
    if start >= len(raw) or start >= end:
        return f"<out of range: start=0x{start:x}, length={length}>"
    lines = []
    chunk = raw[start:end]
    for i in range(0, len(chunk), 16):
        s = chunk[i:i + 16]
        hex_part = " ".join(f"{b:02x}" for b in s)
        ascii_part = "".join(chr(b) if 32 <= b < 127 else "." for b in s)
        lines.append(f"{prefix}{start + i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return "\n".join(lines)


# ============================================================================
# GUI (PySide6)
# ============================================================================

class GM1ParserWindow(QMainWindow):
    def __init__(self, mapping_path: str = None):
        super().__init__()
        self.setWindowTitle("HoMM3 .GM1 Parser v2.0 (h3sed-verified)")
        self.resize(1400, 900)

        self.mapping_path = mapping_path or DEFAULT_MAPPING_PATH
        self.mapping = {}
        self.raw_data = b""
        self.parsed_data = None
        self.map_data = None  # set when user loads a map JSON via "Load Map JSON…"
        self.map_config = None  # set when user loads a day-zero save
        self.day_zero_raw = None  # decompressed day-zero save bytes
        self.current_file = ""

        self._load_mapping()
        self._build_ui()

    def _load_mapping(self):
        try:
            with open(self.mapping_path, "r", encoding="utf-8") as f:
                self.mapping = json.load(f)
        except Exception as e:
            # v3.6: mapping is optional — parser works without it (Phase 3)
            # Show warning but don't crash
            QMessageBox.warning(self, "Mapping not loaded",
                f"Could not load gm1_mapping.json:\n{e}\n\n"
                "The parser will still work for heroes, towns, and object clusters.\n"
                "Only path-record parsing requires the mapping file.")
            self.mapping = {"blocks": [], "constants": {}, "path_block": {"path_record_types": {}}}

    def _build_ui(self):
        central = QWidget()
        self.setCentralWidget(central)
        layout = QVBoxLayout(central)

        # Toolbar — grouped by function
        toolbar = QHBoxLayout()

        # Group 1: Loading (workflow steps 1→2→3)
        load_group = QGroupBox("Load")
        load_layout = QHBoxLayout(load_group)
        load_layout.setContentsMargins(4, 2, 4, 2)
        load_map_btn = QPushButton("1. Map JSON…")
        load_map_btn.clicked.connect(self._on_load_map_json)
        load_layout.addWidget(load_map_btn)
        load_dz_btn = QPushButton("2. Day-Zero Save…")
        load_dz_btn.clicked.connect(self._on_load_day_zero)
        load_layout.addWidget(load_dz_btn)
        load_cfg_btn = QPushButton("2b. Map Config…")
        load_cfg_btn.setToolTip("Load a previously-saved map_config_<mapname>.json (produced by step 2).\n"
                                 "Lets you skip step 2 (Day-Zero Save) if a config already exists.")
        load_cfg_btn.clicked.connect(self._on_load_map_config)
        load_layout.addWidget(load_cfg_btn)
        open_btn = QPushButton("3. Open Save…")
        open_btn.clicked.connect(self._on_open_file)
        load_layout.addWidget(open_btn)
        info_btn = QPushButton("ℹ Info")
        info_btn.setToolTip("Show available workflows and what each one provides")
        info_btn.clicked.connect(self._on_info)
        load_layout.addWidget(info_btn)
        toolbar.addWidget(load_group)

        # Group 2: Export
        export_group = QGroupBox("Export")
        export_layout = QHBoxLayout(export_group)
        export_layout.setContentsMargins(4, 2, 4, 2)
        export_btn = QPushButton("JSON…")
        export_btn.clicked.connect(self._on_export_json)
        export_layout.addWidget(export_btn)
        export_xlsx_btn = QPushButton("Excel…")
        export_xlsx_btn.clicked.connect(self._on_export_objects_excel)
        export_layout.addWidget(export_xlsx_btn)
        export_bin_btn = QPushButton("Binary…")
        export_bin_btn.clicked.connect(self._on_export_binary)
        export_layout.addWidget(export_bin_btn)
        toolbar.addWidget(export_group)

        # Group 3: Tools
        tools_group = QGroupBox("Tools")
        tools_layout = QHBoxLayout(tools_group)
        tools_layout.setContentsMargins(4, 2, 4, 2)
        reload_btn = QPushButton("Reload Mapping")
        reload_btn.clicked.connect(self._on_reload_mapping)
        tools_layout.addWidget(reload_btn)
        toolbar.addWidget(tools_group)

        # toolbar.addStretch()
        # self.file_label = QLabel("No file loaded")
        # toolbar.addWidget(self.file_label)
        # layout.addLayout(toolbar)

        toolbar.addStretch()
        self.file_label = QLabel("No file loaded")
        toolbar.addWidget(self.file_label)

        toolbar_widget = QWidget()
        toolbar_widget.setLayout(toolbar)
        toolbar_widget.setSizePolicy(QSizePolicy.Preferred, QSizePolicy.Fixed)
        toolbar_widget.setMaximumHeight(toolbar_widget.sizeHint().height())
        layout.addWidget(toolbar_widget)

        # Splitter
        splitter = QSplitter(Qt.Horizontal)

        # Left: tree
        self.tree = QTreeWidget()
        self.tree.setHeaderLabels(["Field", "Value", "Type", "Offset", "Size"])
        self.tree.setColumnWidth(0, 280)
        self.tree.setColumnWidth(1, 350)
        self.tree.setColumnWidth(2, 100)
        self.tree.setColumnWidth(3, 110)
        self.tree.setColumnWidth(4, 60)
        self.tree.itemClicked.connect(self._on_tree_item_clicked)
        splitter.addWidget(self.tree)

        # Right: tabs
        self.tabs = QTabWidget()

        self.details_text = QTextEdit()
        self.details_text.setReadOnly(True)
        self.details_text.setFont(QFont("Courier New", 10))
        self.tabs.addTab(self.details_text, "Field Details")

        self.hex_text = QTextEdit()
        self.hex_text.setReadOnly(True)
        self.hex_text.setFont(QFont("Courier New", 10))
        self.tabs.addTab(self.hex_text, "Hex Dump")

        self.json_text = QTextEdit()
        self.json_text.setReadOnly(True)
        self.json_text.setFont(QFont("Courier New", 10))
        self.tabs.addTab(self.json_text, "JSON Preview")

        self.mapping_text = QTextEdit()
        self.mapping_text.setReadOnly(True)
        self.mapping_text.setFont(QFont("Courier New", 10))
        self.tabs.addTab(self.mapping_text, "Mapping")

        # ----- Map Objects tab (table with all 7009 objects + save offsets) -----
        objects_tab = QWidget()
        objects_layout = QVBoxLayout(objects_tab)
        objects_layout.setContentsMargins(4, 4, 4, 4)

        # Filter row
        filter_row = QHBoxLayout()
        filter_row.addWidget(QLabel("Filter:"))
        self.obj_filter_edit = QLineEdit()
        self.obj_filter_edit.setPlaceholderText(
            "Type to filter by type, category, sprite_def, or coord_key (e.g. 'town', 'Tower', '111:3:0')"
        )
        self.obj_filter_edit.textChanged.connect(self._on_objects_filter_changed)
        filter_row.addWidget(self.obj_filter_edit, 1)
        self.obj_category_combo = QComboBox()
        self.obj_category_combo.addItem("(all categories)")
        for cat in sorted({rec.get("category", "") for rec in (self._load_objects_by_coord() or {}).values()}):
            self.obj_category_combo.addItem(cat)
        self.obj_category_combo.currentTextChanged.connect(self._on_objects_filter_changed)
        filter_row.addWidget(QLabel("Category:"))
        filter_row.addWidget(self.obj_category_combo)
        self.obj_verified_only = QCheckBox("Verified only")
        self.obj_verified_only.toggled.connect(self._on_objects_filter_changed)
        filter_row.addWidget(self.obj_verified_only)
        objects_layout.addLayout(filter_row)

        # Table
        self.objects_table = QTableWidget(0, 13)
        self.objects_table.setHorizontalHeaderLabels([
            "X", "Y", "Z", "coord_int", "is_primary",
            "type", "category", "sprite_def",
            "main_offset", "visiting_offset", "fog_offset",
            "decoration_offset", "all_save_offsets",
        ])
        self.objects_table.horizontalHeader().setSectionResizeMode(QHeaderView.Interactive)
        self.objects_table.horizontalHeader().setStretchLastSection(True)
        self.objects_table.setColumnWidth(0, 50);   self.objects_table.setColumnWidth(1, 50)
        self.objects_table.setColumnWidth(2, 40);   self.objects_table.setColumnWidth(3, 80)
        self.objects_table.setColumnWidth(4, 70);   self.objects_table.setColumnWidth(5, 180)
        self.objects_table.setColumnWidth(6, 110);  self.objects_table.setColumnWidth(7, 130)
        self.objects_table.setColumnWidth(8, 100);  self.objects_table.setColumnWidth(9, 110)
        self.objects_table.setColumnWidth(10, 100); self.objects_table.setColumnWidth(11, 110)
        self.objects_table.setColumnWidth(12, 250)
        self.objects_table.setAlternatingRowColors(True)
        self.objects_table.setEditTriggers(QTableWidget.NoEditTriggers)
        self.objects_table.setSelectionBehavior(QTableWidget.SelectRows)
        self.objects_table.setSelectionMode(QTableWidget.SingleSelection)
        self.objects_table.setSortingEnabled(True)
        self.objects_table.itemDoubleClicked.connect(self._on_object_double_clicked)
        objects_layout.addWidget(self.objects_table)

        # Status label
        self.objects_count_label = QLabel("No objects loaded")
        objects_layout.addWidget(self.objects_count_label)

        self.tabs.addTab(objects_tab, "Map Objects")

        # Load objects into table
        self._populate_objects_table()

        splitter.addWidget(self.tabs)
        splitter.setStretchFactor(0, 2)
        splitter.setStretchFactor(1, 3)
        layout.addWidget(splitter)

        # ----- Load status panel (vertical checkboxes, minimal height) -----
        status_group = QGroupBox("Loaded Data")
        status_layout = QVBoxLayout(status_group)
        status_layout.setContentsMargins(8, 2, 8, 2)
        status_layout.setSpacing(1)

        self.chk_map_json = QCheckBox("Map JSON")
        self.chk_map_json.setEnabled(False)
        status_layout.addWidget(self.chk_map_json)

        self.chk_day_zero = QCheckBox("Day-Zero Save")
        self.chk_day_zero.setEnabled(False)
        status_layout.addWidget(self.chk_day_zero)

        self.chk_map_config = QCheckBox("Map Config")
        self.chk_map_config.setEnabled(False)
        status_layout.addWidget(self.chk_map_config)

        self.chk_save_file = QCheckBox("Save File")
        self.chk_save_file.setEnabled(False)
        status_layout.addWidget(self.chk_save_file)

        self.chk_objects = QCheckBox("Objects Located")
        self.chk_objects.setEnabled(False)
        status_layout.addWidget(self.chk_objects)

        self.lbl_map_name = QLabel("")
        self.lbl_map_name.setStyleSheet("color: gray; font-style: italic; font-size: 10px;")
        status_layout.addWidget(self.lbl_map_name)

        # Force minimal height
        status_group.setMaximumHeight(140)
        status_group.setSizePolicy(status_group.sizePolicy().Policy.Fixed, status_group.sizePolicy().Policy.Fixed)
        layout.addWidget(status_group)

        self.status = QStatusBar()
        self.setStatusBar(self.status)
        self.status.showMessage("Ready")

        # ----- Loading overlay (centered "Загрузка…" with busy progress bar) -----
        self._loading_overlay = QFrame(self)
        self._loading_overlay.setStyleSheet("""
            QFrame {
                background-color: rgba(40, 40, 40, 220);
                border-radius: 12px;
                border: 1px solid #555;
            }
        """)
        ol_layout = QVBoxLayout(self._loading_overlay)
        ol_layout.setAlignment(Qt.AlignCenter)
        ol_layout.setSpacing(12)
        ol_layout.setContentsMargins(40, 30, 40, 30)

        self._loading_label = QLabel("⏳ Загрузка…")
        self._loading_label.setStyleSheet("color: white; font-size: 18px; font-weight: bold;")
        self._loading_label.setAlignment(Qt.AlignCenter)
        ol_layout.addWidget(self._loading_label)

        self._loading_bar = QProgressBar()
        self._loading_bar.setRange(0, 0)  # indeterminate / busy mode
        self._loading_bar.setFixedSize(300, 8)
        self._loading_bar.setTextVisible(False)
        ol_layout.addWidget(self._loading_bar, alignment=Qt.AlignCenter)

        self._loading_overlay.hide()
        self._loading_overlay.raise_()

        # Menu
        menubar = self.menuBar()


        # --- File menu (exit) ---
        file_menu = menubar.addMenu("&File")
        file_menu.addSeparator()
        exit_action = QAction("Exit", self)
        exit_action.setShortcut("Ctrl+Q")
        exit_action.triggered.connect(self.close)
        file_menu.addAction(exit_action)

        # --- Load menu ---
        load_menu = menubar.addMenu("&Load")
        load_map_action = QAction("Load Map JSON…", self)
        load_map_action.setShortcut("Ctrl+M")
        load_map_action.triggered.connect(self._on_load_map_json)
        load_menu.addAction(load_map_action)
        load_dz_action = QAction("Load Day-Zero Save…", self)
        load_dz_action.setShortcut("Ctrl+D")
        load_dz_action.triggered.connect(self._on_load_day_zero)
        load_menu.addAction(load_dz_action)
        load_cfg_action = QAction("Load Map Config…", self)
        load_cfg_action.setShortcut("Ctrl+L")
        load_cfg_action.triggered.connect(self._on_load_map_config)
        load_menu.addAction(load_cfg_action)
        open_action = QAction("Open Save…", self)
        open_action.setShortcut("Ctrl+O")
        open_action.triggered.connect(self._on_open_file)
        load_menu.addAction(open_action)

        # --- Export menu ---
        export_menu = menubar.addMenu("&Export")
        export_action = QAction("Export JSON…", self)
        export_action.setShortcut("Ctrl+E")
        export_action.triggered.connect(self._on_export_json)
        export_menu.addAction(export_action)
        export_xlsx_action = QAction("Export Objects to Excel…", self)
        export_xlsx_action.setShortcut("Ctrl+X")
        export_xlsx_action.triggered.connect(self._on_export_objects_excel)
        export_menu.addAction(export_xlsx_action)
        export_bin_action = QAction("Export Binary…", self)
        export_bin_action.setShortcut("Ctrl+B")
        export_bin_action.triggered.connect(self._on_export_binary)
        export_menu.addAction(export_bin_action)

        # --- Tools menu ---
        tools_menu = menubar.addMenu("&Tools")
        reload_action = QAction("Reload Mapping", self)
        reload_action.triggered.connect(self._on_reload_mapping)
        tools_menu.addAction(reload_action)

        # --- Help menu ---
        help_menu = menubar.addMenu("&Help")
        info_action = QAction("Workflows & Modes Info", self)
        info_action.setShortcut("Ctrl+I")
        info_action.triggered.connect(self._on_info)
        help_menu.addAction(info_action)

        self._refresh_mapping_text()

    def _refresh_mapping_text(self):
        self.mapping_text.setPlainText(json.dumps(self.mapping, indent=2, ensure_ascii=False))

    # ----- Loading overlay helpers -----
    def _set_loading(self, text: str = "Загрузка…"):
        """Show the loading overlay with busy indicator and wait cursor."""
        self._loading_label.setText(f"⏳ {text}")
        self._loading_overlay.resize(380, 120)
        # Center on the main window
        cx = self.width() // 2 - 190
        cy = self.height() // 2 - 60
        self._loading_overlay.move(cx, cy)
        self._loading_overlay.show()
        self._loading_overlay.raise_()
        # QApplication.setOverrideCursor(Qt.WaitCursor)  # TODO: fix
        QApplication.processEvents()

    def _set_idle(self):
        """Hide the loading overlay and restore cursor."""
        self._loading_overlay.hide()
        QApplication.restoreOverrideCursor()
        QApplication.processEvents()

    def resizeEvent(self, event):
        """Reposition the loading overlay when window is resized."""
        super().resizeEvent(event)
        if self._loading_overlay.isVisible():
            cx = self.width() // 2 - 190
            cy = self.height() // 2 - 60
            self._loading_overlay.move(cx, cy)

    def _update_load_status(self):
        """Update the load status checkboxes based on what's loaded.
        Called automatically after each load operation."""
        # Map JSON
        if getattr(self, "map_data", None):
            self.chk_map_json.setChecked(True)
            self.chk_map_json.setText(f"Map JSON: {self.map_data.n_objects} objects, "
                                       f"{self.map_data.n_towns} towns")
        else:
            self.chk_map_json.setChecked(False)
            self.chk_map_json.setText("Map JSON")

        # Day-Zero Save
        if getattr(self, "day_zero_raw", None):
            self.chk_day_zero.setChecked(True)
            self.chk_day_zero.setText(f"Day-Zero Save: {len(self.day_zero_raw):,} bytes")
        else:
            self.chk_day_zero.setChecked(False)
            self.chk_day_zero.setText("Day-Zero Save")

        # Map Config (built from day-zero)
        if getattr(self, "map_config", None):
            self.chk_map_config.setChecked(True)
            c = self.map_config
            # MapConfig is a dataclass, not a dict — use attribute access
            if hasattr(c, "hero_section"):
                # MapConfig dataclass
                n_heroes = c.hero_section.count
                n_towns = c.town_section.count
                n_clusters = len(c.clusters)
            else:
                # Legacy dict (shouldn't happen, but defensive)
                n_heroes = c.get("hero_section", {}).get("count", 0)
                n_towns = c.get("town_section", {}).get("count", 0)
                n_clusters = len(c.get("clusters", []))
            self.chk_map_config.setText(
                f"Map Config: {n_heroes} heroes, "
                f"{n_towns} towns, "
                f"{n_clusters} clusters"
            )
        else:
            self.chk_map_config.setChecked(False)
            self.chk_map_config.setText("Map Config")

        # Save File (currently loaded save for viewing/editing)
        if self.raw_data and self.current_file:
            self.chk_save_file.setChecked(True)
            self.chk_save_file.setText(f"Save File: {os.path.basename(self.current_file)} "
                                        f"({len(self.raw_data):,} bytes)")
        else:
            self.chk_save_file.setChecked(False)
            self.chk_save_file.setText("Save File")

        # Objects Located (addresses computed in current save)
        n_off = len(getattr(self, "_computed_offsets", {}))
        n_ver = sum(1 for v in getattr(self, "_computed_offsets", {}).values()
                    if v.get("verified"))
        if n_ver > 0:
            self.chk_objects.setChecked(True)
            self.chk_objects.setText(f"Objects Located: {n_ver}/{n_off}")
        else:
            self.chk_objects.setChecked(False)
            self.chk_objects.setText("Objects Located")

        # Map name label
        if getattr(self, "map_data", None):
            self.lbl_map_name.setText(f"📋 {self.map_data.map_name}")
        elif self.parsed_data and self.parsed_data.get("file_info", {}).get("map_name"):
            self.lbl_map_name.setText(f"📋 {self.parsed_data['file_info']['map_name']}")
        else:
            self.lbl_map_name.setText("")

    # ----- Map Objects tab helpers -----
    def _load_objects_by_coord(self) -> Dict:
        """Return map objects. Requires a Map JSON to be loaded first.
        Returns empty dict if no Map JSON loaded — NO fallback to static files."""
        return getattr(self, "_cached_objects_by_coord", {})

    def _load_objects_with_offsets(self) -> Dict:
        """Return object offsets — dynamically computed from the loaded save.
        Returns empty dict if no save loaded or no Map JSON loaded."""
        return getattr(self, "_computed_offsets", {})

    def _compute_object_offsets_in_save(self):
        """Dynamically compute save-file offsets for EVERY object by scanning
        the decompressed save for 3-byte coordinate sequences.

        Uses `cluster_finder.find_all_object_clusters` (universal gap-based
        clustering with iterative masking) — NO hardcoded offset ranges.

        Algorithm:
          1. Build a lookup set of all coord_int values from objects_by_coord
          2. Use header_parser to compute header_size (scan starts there)
          3. Use cluster_finder to find clusters + per-object offsets
          4. If a MapConfig is loaded, prefer its clusters (from day-0 save)
             but recompute object_offsets for the CURRENT save
          5. Store result in self._computed_offsets

        Works for ANY map + ANY save. No hardcoded `0x118000`, `0x120000`,
        `0x170000`, `0x10000` etc.
        """
        if not self.raw_data:
            return
        oc = self._load_objects_by_coord()
        if not oc:
            return

        # Build coord_int set
        coord_ints = set()
        coord_lookup = {}
        for ck, rec in oc.items():
            ci = rec["coord_int"]
            coord_ints.add(ci)
            if ci not in coord_lookup:
                coord_lookup[ci] = ck

        if not coord_ints:
            return

        # Use header_parser to compute scan_start (no hardcoded 0x10000)
        try:
            from header_parser import parse_header
            from cluster_finder import find_all_object_clusters
        except ImportError:
            # Fallback to relative imports
            import importlib.util
            tools_dir = os.path.dirname(os.path.abspath(__file__))
            for mod_name in ("header_parser", "cluster_finder"):
                if mod_name not in sys.modules:
                    spec = importlib.util.spec_from_file_location(
                        mod_name, os.path.join(tools_dir, mod_name + ".py"))
                    mod = importlib.util.module_from_spec(spec)
                    spec.loader.exec_module(mod)
                    sys.modules[mod_name] = mod
            from header_parser import parse_header
            from cluster_finder import find_all_object_clusters

        header = parse_header(self.raw_data)
        scan_start = header.header_size

        # Use cluster_finder — universal, no hardcoded ranges
        clusters, object_offsets = find_all_object_clusters(
            self.raw_data, coord_ints, scan_start=scan_start)

        # Convert to the format expected by the GUI
        result = {}
        for ci, ck in coord_lookup.items():
            oo = object_offsets.get(ci)
            if oo is None:
                result[str(ci)] = {
                    "coord_key": ck,
                    "save_offsets": [],
                    "main_offset": None,
                    "visiting_offset": None,
                    "fog_offset": None,
                    "alive_offset": None,
                    "treasure_offset": None,
                    "decoration_offset": None,
                    "verified": False,
                }
            else:
                def h(v): return f"0x{v:X}" if v is not None else None
                result[str(ci)] = {
                    "coord_key": ck,
                    "save_offsets": [f"0x{o:X}" for o in oo.save_offsets],
                    "main_offset": h(oo.main_offset),
                    "visiting_offset": h(oo.visiting_offset),
                    "fog_offset": h(oo.fog_offset),
                    "alive_offset": h(oo.alive_offset),
                    "treasure_offset": h(oo.treasure_offset),
                    "decoration_offset": h(oo.decoration_offset),
                    "verified": len(oo.save_offsets) > 0,
                }

        self._computed_offsets = result
        self._cached_objects_with_offsets = result

    def _build_flat_objects_for_table(self) -> List[Dict]:
        """Build flat list of all 7009 objects with offsets, sorted by coord_int."""
        oc = self._load_objects_by_coord()
        offs = self._load_objects_with_offsets()
        flat = []
        for ck, rec in oc.items():
            ci_str = str(rec["coord_int"])
            o = offs.get(ci_str, {})
            flat.append({
                "x": rec["x"], "y": rec["y"], "z": rec["z"],
                "coord_int": rec["coord_int"],
                "coord_key": ck,
                "is_primary": True,
                "type": rec.get("type", ""),
                "category": rec.get("category", ""),
                "sprite_def": rec.get("sprite_def", ""),
                "main_offset": o.get("main_offset", "") or "",
                "visiting_offset": o.get("visiting_offset", "") or "",
                "fog_offset": o.get("fog_offset", "") or "",
                "alive_offset": o.get("alive_offset", "") or "",
                "treasure_offset": o.get("treasure_offset", "") or "",
                "decoration_offset": o.get("decoration_offset", "") or "",
                "save_offsets": o.get("save_offsets", []),
                "verified": o.get("verified", False),
                "details": rec.get("details", {}),
            })
            for ovl in rec.get("overlays", []):
                flat.append({
                    "x": rec["x"], "y": rec["y"], "z": rec["z"],
                    "coord_int": rec["coord_int"],
                    "coord_key": ck,
                    "is_primary": False,
                    "type": ovl.get("type", ""),
                    "category": ovl.get("category", ""),
                    "sprite_def": ovl.get("sprite_def", ""),
                    # Offsets are tile-level (same coord_int)
                    "main_offset": o.get("main_offset", "") or "",
                    "visiting_offset": o.get("visiting_offset", "") or "",
                    "fog_offset": o.get("fog_offset", "") or "",
                    "alive_offset": o.get("alive_offset", "") or "",
                    "treasure_offset": o.get("treasure_offset", "") or "",
                    "decoration_offset": o.get("decoration_offset", "") or "",
                    "save_offsets": o.get("save_offsets", []),
                    "verified": o.get("verified", False),
                    "details": ovl.get("details", {}),
                })
        flat.sort(key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1))
        return flat

    def _populate_objects_table(self):
        """Populate the Map Objects table with all 7009 objects (or filtered subset)."""
        if not hasattr(self, "objects_table"):
            return
        flat = self._build_flat_objects_for_table()
        self._all_flat_objects = flat  # cache for filtering

        # Apply filter
        filter_text = self.obj_filter_edit.text().lower().strip() if hasattr(self, "obj_filter_edit") else ""
        cat_filter = self.obj_category_combo.currentText() if hasattr(self, "obj_category_combo") else "(all categories)"
        verified_only = self.obj_verified_only.isChecked() if hasattr(self, "obj_verified_only") else False

        filtered = []
        for o in flat:
            if verified_only and not o["verified"]:
                continue
            if cat_filter != "(all categories)" and o["category"] != cat_filter:
                continue
            if filter_text:
                haystack = " ".join(str(v) for v in [
                    o["type"], o["category"], o["sprite_def"], o["coord_key"],
                    o["main_offset"], o["visiting_offset"], o["fog_offset"],
                    o["decoration_offset"],
                ]).lower()
                if filter_text not in haystack:
                    continue
            filtered.append(o)

        # Populate table
        self.objects_table.setSortingEnabled(False)
        self.objects_table.setRowCount(len(filtered))
        for row, o in enumerate(filtered):
            # Numeric cells (for sorting)
            x_item = QTableWidgetItem()
            x_item.setData(Qt.DisplayRole, o["x"])
            y_item = QTableWidgetItem()
            y_item.setData(Qt.DisplayRole, o["y"])
            z_item = QTableWidgetItem()
            z_item.setData(Qt.DisplayRole, o["z"])
            ci_item = QTableWidgetItem()
            ci_item.setData(Qt.DisplayRole, o["coord_int"])
            self.objects_table.setItem(row, 0, x_item)
            self.objects_table.setItem(row, 1, y_item)
            self.objects_table.setItem(row, 2, z_item)
            self.objects_table.setItem(row, 3, ci_item)
            self.objects_table.setItem(row, 4, QTableWidgetItem("YES" if o["is_primary"] else "no"))
            self.objects_table.setItem(row, 5, QTableWidgetItem(o["type"]))
            self.objects_table.setItem(row, 6, QTableWidgetItem(o["category"]))
            self.objects_table.setItem(row, 7, QTableWidgetItem(o["sprite_def"]))
            self.objects_table.setItem(row, 8, QTableWidgetItem(o["main_offset"]))
            self.objects_table.setItem(row, 9, QTableWidgetItem(o["visiting_offset"]))
            self.objects_table.setItem(row, 10, QTableWidgetItem(o["fog_offset"]))
            self.objects_table.setItem(row, 11, QTableWidgetItem(o["decoration_offset"]))
            self.objects_table.setItem(row, 12, QTableWidgetItem(", ".join(o["save_offsets"])))
            # Store the full object for double-click
            for col in range(13):
                item = self.objects_table.item(row, col)
                if item:
                    item.setData(Qt.UserRole, row)
        self.objects_table.setSortingEnabled(True)

        # Update count label
        total = len(flat)
        shown = len(filtered)
        if shown == total:
            self.objects_count_label.setText(f"Showing all {shown} objects")
        else:
            self.objects_count_label.setText(f"Showing {shown} of {total} objects (filtered)")

    def _on_objects_filter_changed(self):
        """Called when filter text or category changes — repopulate table."""
        self._populate_objects_table()

    def _on_object_double_clicked(self, item: QTableWidgetItem):
        """When user double-clicks a row, show object details + hex dump of main_offset."""
        row = item.row()
        # Get the coord_int from column 3
        ci_item = self.objects_table.item(row, 3)
        if not ci_item:
            return
        coord_int = ci_item.data(Qt.DisplayRole)
        if coord_int is None:
            return
        # Find the object in our flat list
        if not hasattr(self, "_all_flat_objects"):
            return
        # Get the actual row index (sorting may have reordered)
        # Easier: scan by coord_int + is_primary
        is_primary_item = self.objects_table.item(row, 4)
        is_primary = (is_primary_item.text() == "YES") if is_primary_item else True
        # Find first matching object
        target = None
        for o in self._all_flat_objects:
            if o["coord_int"] == coord_int and o["is_primary"] == is_primary:
                target = o
                break
        if target is None:
            # Fall back to first by coord_int
            for o in self._all_flat_objects:
                if o["coord_int"] == coord_int:
                    target = o
                    break
        if target is None:
            return

        # Build details text
        lines = [
            f"=== Object at {target['coord_key']} (coord_int={target['coord_int']}) ===",
            f"Type:        {target['type']}",
            f"Category:    {target['category']}",
            f"Sprite:      {target['sprite_def']}",
            f"Is primary:  {target['is_primary']}",
            f"Verified:    {target['verified']}",
            "",
            "=== Save-file offsets (in decompressed .GM1) ===",
            f"  main_offset:       {target['main_offset'] or '(none — decoration)'}",
            f"  visiting_offset:   {target['visiting_offset'] or '(none)'}",
            f"  fog_offset:        {target['fog_offset'] or '(none)'}",
            f"  alive_offset:      {target['alive_offset'] or '(none)'}",
            f"  treasure_offset:   {target['treasure_offset'] or '(none)'}",
            f"  decoration_offset: {target['decoration_offset'] or '(none)'}",
            f"  all_save_offsets:  {target['save_offsets']}",
            "",
            "=== Details (from .h3m map) ===",
        ]
        for k, v in target["details"].items():
            lines.append(f"  {k}: {v}")

        # Hex dump around main_offset (if available and we have raw_data)
        if target["main_offset"] and self.raw_data:
            try:
                off = int(target["main_offset"], 16)
                lines.append("")
                lines.append(f"=== Hex dump around main_offset 0x{off:X} (±16 bytes) ===")
                lines.append(hex_dump(self.raw_data, off - 16, 48))
            except Exception as e:
                lines.append(f"(hex dump failed: {e})")

        # Show in Field Details tab
        self.details_text.setPlainText("\n".join(lines))
        self.tabs.setCurrentWidget(self.details_text)

    def _on_info(self):
        """Show a dialog explaining all available workflows and what each provides."""
        has_map_json = "✅ loaded" if self.map_data and getattr(self.map_data, 'n_sprites', 0) > 0 else "❌ not loaded"
        has_config = "✅ loaded" if getattr(self, "map_config", None) else "❌ not loaded"
        has_day0 = "✅ loaded" if getattr(self, "day_zero_raw", None) else "❌ not loaded"
        has_save = "✅ loaded" if self.raw_data else "❌ not loaded"

        info_text = f"""HoMM3 .GM1 Save Parser — Режимы работы

══════════════════════════════════════════════════════════════
ТЕКУЩЕЕ СОСТОЯНИЕ:
  Map JSON:       {has_map_json}
  Map Config:     {has_config}
  Day-0 Save:     {has_day0}
  Open Save:      {has_save}
══════════════════════════════════════════════════════════════

ДОСТУПНЫЕ РЕЖИМЫ (от простого к полному):

┌─────────────────────────────────────────────────────────────┐
│ РЕЖИМ 1: Минимальный — только сейв                           │
│                                                             │
│   Шаг: 3. Open Save… (Ctrl+O)                               │
│                                                             │
│   Что нужно:  только .GM1 файл                              │
│   Что даёт:   • Заголовок (magic, version, map_name)        │
│              • Герои (156, с именами, статы, армия)          │
│   Чего нет:   • Города (нужны координаты)                   │
│              • Кластеры объектов (нужны координаты)         │
│              • Типы объектов (нужен map JSON)                │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ РЕЖИМ 2: Map JSON + сейв (РЕКОМЕНДУЕМЫЙ)                    │
│                                                             │
│   Шаги: 1. Map JSON… → 3. Open Save…                       │
│                                                             │
│   Что нужно:  .h3m.zip (парсинг карты) + .GM1 сейв           │
│   Что даёт:   • Всё из режима 1                             │
│              • Города (21, с именами, фракциями)            │
│              • Кластеры объектов (6006, 100% verified)      │
│              • Типы объектов (town, mine, monster, ...)     │
│              • Спрайты (.def names)                          │
│              • Описание карты, слухи, глобальные события      │
│              • Запрещённые артефакты/заклинания/навыки      │
│   Чего нет:   • Начальное состояние (сколько было в         │
│                сундуках ДО действий игрока)                 │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ РЕЖИМ 3: Map Config + сейв                                  │
│                                                             │
│   Шаги: 2b. Map Config… → 3. Open Save…                    │
│                                                             │
│   Что нужно:  map_config_*.json + .GM1 сейв                 │
│   Что даёт:   • Всё из режима 2 (кроме типов/спрайтов)       │
│              • Готовые смещения кластеров (из day-0)        │
│   Чего нет:   • Типы объектов (нужен map JSON)              │
│   Плюс:       Быстрее (config уже построен)                 │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ РЕЖИМ 4: Полный (Map JSON + Day-0 + сейв)                   │
│                                                             │
│   Шаги: 1. Map JSON… → 2. Day-Zero Save… → 3. Open Save…   │
│   ИЛИ:  1. Map JSON… → 2b. Map Config… → 3. Open Save…     │
│                                                             │
│   Что нужно:  .h3m.zip + 0000.GM1 (или config) + .GM1      │
│   Что даёт:   • Всё из режима 2                             │
│              • Начальное состояние объектов                 │
│              • Валидация (что изменилось с day-0)           │
│              • Готовый map_config_*.json для будущих сейвов  │
└─────────────────────────────────────────────────────────────┘

ВСЕ ВХОДЫ ОПЦИОНАЛЬНЫ — парсер работает на любом уровне.
Map JSON и Day-0 сейв — enrichment, не requirement.

ОГРАНИЧЕНИЕ ProspectorRT: работает только со стартовыми сейвами,
потому что ищет ".GM1" в имени файла (которое есть только у
автосейвов с числовым именем, но не у ручных сохранений).
Наш парсер НЕ имеет этого ограничения — работает с любыми сейвами."""

        # Create a read-only text dialog
        from PySide6.QtWidgets import QDialog, QDialogButtonBox, QTextEdit, QVBoxLayout
        dialog = QDialog(self)
        dialog.setWindowTitle("Режимы работы парсера")
        dialog.resize(700, 600)
        layout = QVBoxLayout(dialog)
        text_edit = QTextEdit()
        text_edit.setReadOnly(True)
        text_edit.setPlainText(info_text)
        text_edit.setFont(QFont("Consolas", 10))
        layout.addWidget(text_edit)
        buttons = QDialogButtonBox(QDialogButtonBox.Ok)
        buttons.accepted.connect(dialog.accept)
        layout.addWidget(buttons)
        dialog.exec()

    def _on_open_file(self):
        # v3.5: ALL inputs are optional. Parsing works at every level:
        #   - No map JSON, no config → header + heroes only (no towns, no clusters)
        #   - Map JSON only            → heroes + towns + clusters (with types)
        #   - MapConfig only            → heroes + towns + clusters (types unknown)
        #   - Map JSON + MapConfig      → everything (recommended)
        if not self.map_data and not self.map_config:
            reply = QMessageBox.question(
                self, "No map data loaded",
                "Neither Map JSON nor Map Config is loaded.\n\n"
                "You can still open the save — the parser will find\n"
                "heroes and header, but NOT towns or object clusters\n"
                "(those require coordinate data from a map).\n\n"
                "For full parsing, load one of:\n"
                "  • Map JSON (Ctrl+M) — gives types, sprites, description\n"
                "  • Map Config (Ctrl+L) — gives pre-built offsets\n"
                "  • Day-Zero Save (Ctrl+D) — builds config from scratch\n\n"
                "Continue anyway?",
                QMessageBox.Yes | QMessageBox.No, QMessageBox.Yes
            )
            if reply != QMessageBox.Yes:
                return
        path, _ = QFileDialog.getOpenFileName(
            self, "Open .GM1 file", "", "GM1 files (*.gm1 *.GM1 *.cgm *.CGM);;All files (*)")
        if not path:
            return
        self.load_file(path)

    def _build_config_from_map_data(self):
        """Build a temporary MapConfig from MapData (without day-0 save).

        Uses map_data.coord_int_lookup for object coordinate set
        and map_data.objects_by_coord for town coords (with X-2 correction).

        This enables parsing ANY save with just Map JSON — no day-0 save needed.
        """
        from save_layout import MapConfig, ObjectOffsets

        # Build coord_ints set from map_data
        coord_ints = set(self.map_data.coord_int_lookup.keys())

        # Build town coords with X-2 correction (map X = save X + 2)
        town_coords = []
        town_names = []
        for ck, rec in self.map_data.objects_by_coord.items():
            if rec.get("category") == "town":
                save_x = rec["x"] - 2
                save_y = rec["y"]
                save_z = rec["z"]
                town_coords.append((save_x, save_y, save_z))
                town_name = rec.get("details", {}).get("town_name", "")
                town_names.append(town_name)

        # Build a minimal MapConfig
        # We don't have clusters or hero_section from a day-0 save,
        # but save_parser.adapt_config_to_save will re-find everything
        # from the current save using these coord_ints and town_coords.
        config = MapConfig()
        config.meta = {
            "map_name": self.map_data.map_name,
            "map_size": self.map_data.map_size,
            "has_underground": self.map_data.has_underground,
            "n_objects": self.map_data.n_objects,
            "n_unique_coords": len(coord_ints),
            "n_towns": len(town_coords),
            "n_heroes_total": 0,  # unknown — will be found by adapt
            "source": "built_from_map_data (no day-0 save)",
        }

        # Populate object_offsets with empty offsets — adapt_config_to_save
        # only needs the coord_int keys, not the actual offset values
        config.object_offsets = {ci: ObjectOffsets() for ci in coord_ints}

        # Populate town_section with coords/names (for find_town_blocks hints)
        from save_layout import TownBlockInfo, TownSection
        town_blocks = []
        for (tx, ty, tz), tname in zip(town_coords, town_names):
            town_blocks.append(TownBlockInfo(
                block_offset=0,  # unknown — will be found
                name_offset=0,
                name=tname,
                faction=255,
                town_type=0,
                x=tx, y=ty, z=tz,
            ))
        config.town_section = TownSection(blocks=town_blocks)

        # clusters and hero_section are empty — adapt_config_to_save
        # will find them fresh from the save
        config.clusters = []

        return config

    def load_file(self, path: str):
        self._set_loading("Декомпрессия сейва…")
        try:
            self.raw_data = load_gm1_file(path)
        except Exception as e:
            self._set_idle()
            QMessageBox.critical(self, "Error", f"Failed to load file:\n{e}")
            return

        self.current_file = path
        filename = os.path.basename(path)
        self.file_label.setText(f"{filename} ({len(self.raw_data)} bytes raw)")

        self._set_loading("Парсинг сейва…")
        # v3.5: If no MapConfig loaded but map_data is available,
        # build a temporary MapConfig from map_data (coord_ints + town_coords)
        map_config = getattr(self, "map_config", None)
        if map_config is None and self.map_data:
            map_config = self._build_config_from_map_data()

        # v3.0: prefer MapConfig (Phase 3) if available; fall back to legacy mapping
        self.parsed_data = parse_save(self.raw_data, self.mapping,
                                       map_config=map_config)

        self._set_loading("Поиск объектов в сейве…")
        self._computed_offsets = {}
        # If parse_save() returned object_offsets (Phase 3 path), use them directly
        if "object_offsets" in self.parsed_data and self.parsed_data["object_offsets"]:
            self._computed_offsets = self.parsed_data["object_offsets"]
            self._cached_objects_with_offsets = self._computed_offsets
        else:
            self._compute_object_offsets_in_save()

        n_verified = sum(1 for v in self._computed_offsets.values() if v.get("verified"))
        self.status.showMessage(
            f"Loaded: {path} | {n_verified} objects located in save", 5000
        )

        self._set_loading("Построение дерева…")
        self._populate_tree()
        self._update_load_status()
        self._populate_objects_table()

        self._set_loading("Генерация JSON…")
        # v3.8: If Phase 3 was used (has map_objects), use parsed_save_to_dict for full JSON
        if "map_objects" in self.parsed_data:
            # Phase 3 path — use save_parser.parsed_save_to_dict
            try:
                from save_parser import parsed_save_to_dict
            except ImportError:
                import importlib.util
                tools_dir = os.path.dirname(os.path.abspath(__file__))
                spec = importlib.util.spec_from_file_location(
                    "save_parser", os.path.join(tools_dir, "save_parser.py"))
                mod = importlib.util.module_from_spec(spec)
                spec.loader.exec_module(mod)
                parsed_save_to_dict = mod.parsed_save_to_dict
            # Build a minimal ParsedSave-like object from parsed_data
            full_json = json.dumps(self.parsed_data, ensure_ascii=False, indent=2,
                                   default=lambda o: o.hex() if isinstance(o, (bytes, bytearray)) else str(o))
            self.json_text.setPlainText(full_json)
        else:
            json_str = export_to_json(self.parsed_data, self.raw_data,
                                    objects_by_coord=self._load_objects_by_coord(),
                                    coord_int_lookup=getattr(self.map_data, "coord_int_lookup", {}) if self.map_data else {},
                                    computed_offsets=self._load_objects_with_offsets())
            self.json_text.setPlainText(json_str)
        self._set_idle()

    def _populate_tree(self):
        self.tree.clear()

        # File info
        info_item = QTreeWidgetItem(["File Info", "", "", "", ""])
        fi = self.parsed_data["file_info"]
        info_item.addChild(QTreeWidgetItem(["raw_size", str(fi["raw_size"]), "int", "0x0", str(fi["raw_size"])]))
        info_item.addChild(QTreeWidgetItem(["magic", fi["magic"], "ascii", "0x0", "5"]))
        info_item.addChild(QTreeWidgetItem(["version_major", f"0x{fi['version_major']:02x}", "u8", "0x8", "1"]))
        info_item.addChild(QTreeWidgetItem(["version_minor", f"0x{fi['version_minor']:02x}", "u8", "0xC", "1"]))
        self.tree.addTopLevelItem(info_item)

        # Mapping blocks
        for block in self.parsed_data["blocks"]:
            block_item = QTreeWidgetItem([block["name"], "", "block", "", ""])
            block_item.setToolTip(0, block.get("description", ""))
            for f in block["fields"]:
                off_str = f"0x{f['offset']:08x}" if isinstance(f["offset"], int) and f["offset"] >= 0 else str(f["offset"])
                field_item = QTreeWidgetItem([f["name"], f["formatted"], f["type"], off_str, str(f["size"])])
                field_item.setToolTip(0, f.get("description", ""))
                if "path_records" in f:
                    for i, rec in enumerate(f["path_records"]):
                        rec_item = QTreeWidgetItem([f"Record {i+1}: {rec['type_name']}", rec["type"], "path_record", f"0x{rec['offset']:08x}", str(rec["size"])])
                        for fname, fval in rec["fields"].items():
                            rec_item.addChild(QTreeWidgetItem([fname, str(fval), "", "", ""]))
                        field_item.addChild(rec_item)
                block_item.addChild(field_item)
            self.tree.addTopLevelItem(block_item)

        # Heroes found (h3sed regex)
        if self.parsed_data["heroes_found"]:
            hero_parent = QTreeWidgetItem([f"Heroes Found ({len(self.parsed_data['heroes_found'])})", "", "summary", "", ""])
            for h in self.parsed_data["heroes_found"]:
                h_item = QTreeWidgetItem([f"{h['name']} @ 0x{h['offset']:08x}", "", "hero", f"0x{h['offset']:08x}", "~1122"])
                for fname, (fval, fstr) in h["fields"].items():
                    h_item.addChild(QTreeWidgetItem([fname, fstr, "", "", ""]))
                hero_parent.addChild(h_item)
            self.tree.addTopLevelItem(hero_parent)

        # Towns found (h3sed regex)
        if self.parsed_data["towns_found"]:
            town_parent = QTreeWidgetItem([f"Towns Found ({len(self.parsed_data['towns_found'])})", "", "summary", "", ""])
            for t in self.parsed_data["towns_found"]:
                t_item = QTreeWidgetItem([f"{t['name']} ({t['type_name']}) @ 0x{t['offset']:08x}",
                                          f"{t['faction_name']}", "town", f"0x{t['offset']:08x}", "~100"])
                t_item.addChild(QTreeWidgetItem(["faction", f"{t['faction']} = {t['faction_name']}", "u8", f"0x{t['offset']:08x}", "1"]))
                t_item.addChild(QTreeWidgetItem(["type", f"{t['type']} = {t['type_name']}", "u8", f"0x{t['offset']+3:08x}", "1"]))
                t_item.addChild(QTreeWidgetItem(["location", f"({t['location'][0]}, {t['location'][1]}, {t['location'][2]})", "3×u8", f"0x{t['offset']+4:08x}", "3"]))
                t_item.addChild(QTreeWidgetItem(["army_types", str(t['army_types']), "7×u32", f"0x{t['offset']+9:08x}", "28"]))
                t_item.addChild(QTreeWidgetItem(["army_counts", str(t['army_counts']), "7×u32", f"0x{t['offset']+37:08x}", "28"]))
                town_parent.addChild(t_item)
            self.tree.addTopLevelItem(town_parent)

        # Map Objects (7009 objects grouped by category)
        self._add_map_objects_to_tree()

        # Map Objects from tile scanner (objects found by IsObject dispatcher)
        map_objs = self.parsed_data.get("map_objects", [])
        if map_objs:
            from collections import Counter
            tile_root = QTreeWidgetItem([
                f"Tile Objects ({len(map_objs)} found by tile scanner)",
                "", "tile_objects_root", "", ""
            ])
            tile_root.setToolTip(0, "Objects found by scan_tiles + IsObject dispatcher (ProspectorRT-style)")
            type_counts = Counter(o["type_name"] for o in map_objs)
            for type_name, count in type_counts.most_common():
                type_item = QTreeWidgetItem([f"{type_name} ({count})", "", "tile_object_type", "", ""])
                # Add first 10 objects of this type
                for o in [obj for obj in map_objs if obj["type_name"] == type_name][:10]:
                    coord = f"({o['x']},{o['y']},{o['z']})"
                    detail = ", ".join(f"{k}={v}" for k, v in o.items()
                                        if k not in ("type_id", "type_name", "x", "y", "z",
                                                     "offset", "loc", "tile_num", "obj_id"))
                    child = QTreeWidgetItem([coord, detail, "tile_object", f"0x{o['offset']:X}", ""])
                    type_item.addChild(child)
                if count > 10:
                    type_item.addChild(QTreeWidgetItem([f"... {count - 10} more", "", "", "", ""]))
                tile_root.addChild(type_item)
            self.tree.addTopLevelItem(tile_root)

        # Errors
        if self.parsed_data["errors"]:
            err_item = QTreeWidgetItem([f"Errors ({len(self.parsed_data['errors'])})", "", "errors", "", ""])
            for e in self.parsed_data["errors"]:
                err_item.addChild(QTreeWidgetItem(["error", e, "", "", ""]))
            self.tree.addTopLevelItem(err_item)

    def _add_map_objects_to_tree(self):
        """Add a 'Map Objects' tree node with all 7009 objects grouped by category."""
        oc = self._load_objects_by_coord()
        offs = self._load_objects_with_offsets()
        if not oc:
            return

        # Build flat list (primary + overlays) — same as the table
        flat = self._build_flat_objects_for_table()

        # Group by category
        from collections import defaultdict
        by_cat = defaultdict(list)
        for o in flat:
            by_cat[o["category"]].append(o)

        # Top-level "Map Objects" node
        objects_root = QTreeWidgetItem([
            f"Map Objects ({len(flat)} total: {sum(1 for o in flat if o['is_primary'])} primary + {sum(1 for o in flat if not o['is_primary'])} overlays)",
            "", "map_objects_root", "", ""
        ])
        objects_root.setToolTip(0, "All 7009 objects on the map, grouped by category. Click an object to see its save-file offsets and hex dump.")

        # Sort categories by count descending
        for cat in sorted(by_cat.keys(), key=lambda c: -len(by_cat[c])):
            cat_items = by_cat[cat]
            cat_item = QTreeWidgetItem([
                f"{cat} ({len(cat_items)})",
                "", "map_object_category", "", ""
            ])
            cat_item.setToolTip(0, f"{len(cat_items)} objects in category '{cat}'")

            # Sort by coord_int within category
            for o in sorted(cat_items, key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1)):
                # Choose label: prefer town_name / hero_id / message if available
                label_extra = ""
                details = o.get("details", {}) or {}
                if details.get("town_name"):
                    label_extra = f" [{details['town_name']}]"
                elif details.get("hero_id"):
                    label_extra = f" [{details['hero_id']}]"
                elif details.get("message"):
                    msg = str(details["message"])[:30]
                    label_extra = f" [{msg}…]" if len(str(details["message"])) > 30 else f" [{msg}]"

                main_off = o.get("main_offset", "")
                obj_label = f"({o['x']:3d},{o['y']:3d},{o['z']}) {o['type']}{label_extra}"
                if not o["is_primary"]:
                    obj_label = f"  ↳ {o['type']} (overlay)"

                obj_item = QTreeWidgetItem([
                    obj_label,
                    main_off or "(no addr)",
                    "map_object",
                    main_off or "",
                    "8+",
                ])
                # Store the full object as data on column 0 for click handler
                obj_item.setData(0, Qt.UserRole, o)
                obj_item.setToolTip(0, (
                    f"Object: {o['type']}\n"
                    f"Category: {o['category']}\n"
                    f"Coords: ({o['x']}, {o['y']}, {o['z']})\n"
                    f"coord_int: {o['coord_int']}\n"
                    f"Sprite: {o['sprite_def']}\n"
                    f"Main offset (in save): {main_off or '(none — decoration)'}\n"
                    f"Click to see hex dump and full details"
                ))
                cat_item.addChild(obj_item)
            objects_root.addChild(cat_item)

        self.tree.addTopLevelItem(objects_root)
        # Note: don't auto-expand objects_root — too many items (7009).
        # User can expand manually.

    def _on_tree_item_clicked(self, item, column):
        name = item.text(0)
        value = item.text(1)
        type_ = item.text(2)
        offset_str = item.text(3)
        size_str = item.text(4)

        # Special handling for map objects (with full object data attached)
        if type_ == "map_object":
            self._show_map_object_details(item)
            return

        # Build detailed field info
        details = [f"Field:    {name}", f"Value:    {value}", f"Type:     {type_}",
                   f"Offset:   {offset_str}", f"Size:     {size_str}"]
        tooltip = item.toolTip(0)
        if tooltip:
            details += ["", "Description:", f"  {tooltip}"]

        # For hero blocks, show all parsed fields
        if type_ == "hero":
            # Find this hero in parsed_data
            hero_off = int(offset_str, 16)
            for h in self.parsed_data.get("heroes_found", []):
                if h["offset"] == hero_off:
                    details += ["", "Hero Fields:"]
                    for fname, (fval, fstr) in h["fields"].items():
                        details.append(f"  {fname:25s} = {fstr}")
                    break

        # For town blocks, show town details
        if type_ == "town":
            town_off = int(offset_str, 16)
            for t in self.parsed_data.get("towns_found", []):
                if t["offset"] == town_off:
                    details += ["", "Town Fields:"]
                    details.append(f"  name:        {t['name']}")
                    details.append(f"  faction:     {t['faction']} ({t['faction_name']})")
                    details.append(f"  type:        {t['type']} ({t['type_name']})")
                    details.append(f"  location:    ({t['location'][0]}, {t['location'][1]}, {t['location'][2]})")
                    details.append(f"  army_types:  {t['army_types']}")
                    details.append(f"  army_counts: {t['army_counts']}")
                    break

        # Show child fields if any
        child_count = item.childCount()
        if child_count > 0 and type_ not in ("hero", "town"):
            details += ["", "Sub-fields:"]
            for i in range(child_count):
                child = item.child(i)
                details.append(f"  {child.text(0):25s} = {child.text(1)}")

        # Hex dump
        if offset_str.startswith("0x"):
            try:
                offset = int(offset_str, 16)
                # Parse size — handle "~1122", "1122+", "variable", etc.
                import re as _re
                nums = _re.findall(r'\d+', size_str)
                if nums:
                    size = int(nums[0])
                else:
                    size = 16
                # Known block sizes by type
                if type_ == "hero":
                    size = 1122  # full hero block
                elif type_ == "town":
                    size = 100   # town record (approximate)
                elif type_ == "path_record":
                    size = max(size, 30)
                # Clamp to reasonable range
                if size <= 0:
                    size = 16
                if size > 5000:
                    size = 5000  # safety limit

                # Hex dump with context (in Field Details)
                ctx_start = max(0, offset - 16)
                hex_text = hex_dump(self.raw_data, ctx_start, min(size + 32, 5000))
                details += ["", "Hex Dump (with context):", hex_text]

                # Full hex dump in the Hex Dump tab
                self.hex_text.setPlainText(hex_dump(self.raw_data, offset, size))
            except ValueError:
                pass

        self.details_text.setPlainText("\n".join(details))

    def _show_map_object_details(self, item: QTreeWidgetItem):
        """Show detailed information for a map object, including save-file offsets
        and the structure of the object record at main_offset."""
        o = item.data(0, Qt.UserRole)
        if not o:
            return

        lines = []
        lines.append("=" * 72)
        lines.append(f"  OBJECT: {o['type']}  (category: {o['category']})")
        lines.append("=" * 72)
        lines.append("")
        lines.append("--- Coordinates ---")
        lines.append(f"  x, y, z:           ({o['x']}, {o['y']}, {o['z']})")
        lines.append(f"  coord_key:         {o['coord_key']}")
        lines.append(f"  coord_int:         {o['coord_int']}  (= x | (y<<8) | (z<<16))")
        lines.append(f"  is_primary:        {o['is_primary']}  (YES = main object on tile; no = overlay)")
        lines.append(f"  sprite_def:        {o['sprite_def']}")
        lines.append(f"  verified in save:  {o.get('verified', False)}")
        lines.append("")

        # ----- Where the coordinates are stored in the save -----
        lines.append("--- Where coordinates are stored in the save ---")
        main_off_str = o.get("main_offset", "")
        if main_off_str:
            try:
                main_off = int(main_off_str, 16)
                lines.append(f"  ⭐ main_offset:    {main_off_str}  (in main object-state array)")
                lines.append(f"     At this offset, the save stores 3 bytes for this object's coords:")
                if self.raw_data and main_off + 3 <= len(self.raw_data):
                    bx, by, bz = self.raw_data[main_off], self.raw_data[main_off+1], self.raw_data[main_off+2]
                    lines.append(f"     bytes[0..2]:  {bx:02X} {by:02X} {bz:02X}  =  x={bx}, y={by}, z={bz}")
                    lines.append(f"     (matches object coords: x={o['x']}, y={o['y']}, z={o['z']})")
            except ValueError:
                pass
        else:
            lines.append(f"  main_offset:    (none — this is a decoration; decorations have no per-object state record)")

        # Other cluster offsets
        for label, key in [
            ("visiting_offset",   "visiting_offset"),
            ("fog_offset",        "fog_offset"),
            ("alive_offset",      "alive_offset"),
            ("treasure_offset",   "treasure_offset"),
            ("decoration_offset", "decoration_offset"),
        ]:
            v = o.get(key, "")
            if v:
                lines.append(f"  {label:20s} {v}")
        lines.append("")

        # ----- Object record structure (decoded) -----
        if main_off_str and self.raw_data:
            try:
                main_off = int(main_off_str, 16)
                lines.append("--- Object record structure at main_offset ---")
                lines.append(f"  Each object record in the main array starts with 3 coord bytes,")
                lines.append(f"  followed by per-object state (flags, owner, etc.). The exact")
                lines.append(f"  layout depends on the object type and is still being decoded.")
                lines.append("")
                lines.append(f"  Hex dump of first 64 bytes at {main_off_str}:")
                dump_size = min(64, len(self.raw_data) - main_off)
                for i in range(0, dump_size, 16):
                    row = self.raw_data[main_off + i:main_off + i + 16]
                    hex_part = " ".join(f"{b:02x}" for b in row)
                    ascii_part = "".join(chr(b) if 32 <= b < 127 else "." for b in row)
                    lines.append(f"    {main_off_str}+{i:02X}:  {hex_part:<48s}  |{ascii_part}|")

                # Try to decode known fields based on object category
                lines.append("")
                lines.append("  Decoded fields (best-effort):")
                lines.append(f"    [offset +0..2]  coords:    ({self.raw_data[main_off]}, {self.raw_data[main_off+1]}, {self.raw_data[main_off+2]})")
                if main_off + 4 <= len(self.raw_data):
                    flag_byte = self.raw_data[main_off+3]
                    lines.append(f"    [offset +3]     flag byte: 0x{flag_byte:02X} ({flag_byte})  — type/owner/state flag")
                if main_off + 8 <= len(self.raw_data):
                    val32 = int.from_bytes(self.raw_data[main_off+4:main_off+8], 'little')
                    lines.append(f"    [offset +4..7]  u32 LE:    0x{val32:08X} ({val32})  — possible object_ref_id or data")
                if main_off + 12 <= len(self.raw_data):
                    val32b = int.from_bytes(self.raw_data[main_off+8:main_off+12], 'little')
                    lines.append(f"    [offset +8..11] u32 LE:    0x{val32b:08X} ({val32b})")
                if main_off + 16 <= len(self.raw_data):
                    val32c = int.from_bytes(self.raw_data[main_off+12:main_off+16], 'little')
                    lines.append(f"    [offset +12..15] u32 LE:   0x{val32c:08X} ({val32c})")
                lines.append(f"    (See 'Hex Dump' tab for 256 bytes of context around this offset)")
            except (ValueError, IndexError) as e:
                lines.append(f"  (failed to decode: {e})")
        lines.append("")

        # ----- Map-defined settings (from .h3m) -----
        lines.append("--- Settings from .h3m map (initial state) ---")
        details = o.get("details", {}) or {}
        if details:
            for k, v in details.items():
                lines.append(f"  {k}: {v}")
        else:
            lines.append("  (no per-object settings in .h3m — defaults apply)")
        lines.append("")

        # ----- Explanation of all_save_offsets -----
        save_offsets = o.get("save_offsets", [])
        lines.append("--- What does 'all_save_offsets' mean? ---")
        lines.append(f"  all_save_offsets is the list of ALL places in the decompressed save")
        lines.append(f"  where the 3-byte coordinate sequence (x, y, z) = "
                     f"({o['x']:02X}, {o['y']:02X}, {o['z']:02X}) appears as a substring.")
        lines.append(f"  These are NOT all 'settings' of the object — they are just references")
        lines.append(f"  to this object's coordinates from various save sections (main array,")
        lines.append(f"  visiting-heroes list, fog-of-war bitmap, etc.).")
        lines.append(f"")
        lines.append(f"  Total references found: {len(save_offsets)}")
        if save_offsets:
            lines.append(f"  Offsets:")
            # Build cluster lookup from current MapConfig if available
            # (so we can identify which cluster each offset belongs to)
            cluster_ranges = []
            if hasattr(self, "map_config") and self.map_config:
                if hasattr(self.map_config, "clusters"):
                    # MapConfig dataclass
                    for c in self.map_config.clusters:
                        cluster_ranges.append((c.start, c.end, c.name))
                elif isinstance(self.map_config, dict):
                    # Legacy dict
                    for c in self.map_config.get("clusters", []):
                        cluster_ranges.append((c["start"], c["end"], c["name"]))
            # Also use freshly-computed clusters from _compute_object_offsets_in_save
            # (which are the actual clusters for THIS save, not day-0)
            # We can rebuild them by re-using cluster_finder, but for the display
            # we just use the config clusters as a hint.
            for so in save_offsets[:20]:
                # Identify cluster
                cluster = "other"
                try:
                    soff = int(so, 16)
                    # Check against all known cluster ranges
                    for cs, ce, cname in cluster_ranges:
                        if cs <= soff < ce:
                            cluster = cname
                            break
                    else:
                        # Fallback heuristic — classify by rough location
                        if soff < 0x10000:
                            cluster = "header-area"
                        elif soff < 0x100000:
                            cluster = "early-section"
                except ValueError:
                    pass
                lines.append(f"    {so}  ({cluster})")
            if len(save_offsets) > 20:
                lines.append(f"    ... and {len(save_offsets) - 20} more")
        lines.append("")

        # ----- Full hex dump in the Hex Dump tab -----
        if main_off_str and self.raw_data:
            try:
                main_off = int(main_off_str, 16)
                # Show 256 bytes centered on main_offset (128 before + 128 after)
                ctx_start = max(0, main_off - 128)
                self.hex_text.setPlainText(hex_dump(self.raw_data, ctx_start, 256))
            except (ValueError, IndexError):
                pass

        self.details_text.setPlainText("\n".join(lines))

    def _on_export_json(self):
        if not self.parsed_data:
            QMessageBox.warning(self, "Warning", "No file loaded")
            return
        path, _ = QFileDialog.getSaveFileName(self, "Export JSON", "parsed_save.json", "JSON files (*.json)")
        if not path:
            return
        try:
            self._set_loading("Экспорт JSON…")
            json_str = export_to_json(self.parsed_data, self.raw_data,
                                objects_by_coord=self._load_objects_by_coord(),
                                coord_int_lookup=getattr(self.map_data, "coord_int_lookup", {}) if self.map_data else {},
                                computed_offsets=self._load_objects_with_offsets())
            with open(path, "w", encoding="utf-8") as f:
                f.write(json_str)
            self._set_idle()
            self.status.showMessage(f"Exported to: {path}")
        except Exception as e:
            self._set_idle()
            QMessageBox.critical(self, "Error", f"Failed to export:\n{e}")

    def _on_export_objects_excel(self):
        """Export all 7009 map objects (with types + coords) to an Excel workbook."""
        # We don't actually need a save loaded — the object map is static per map.
        # But we use parsed_data to know which map name to put in the file.
        if not self.parsed_data:
            reply = QMessageBox.question(
                self, "No save loaded",
                "No save is loaded, but the object map is static (per-map). "
                "Export anyway?",
                QMessageBox.Yes | QMessageBox.No, QMessageBox.Yes
            )
            if reply != QMessageBox.Yes:
                return
        path, _ = QFileDialog.getSaveFileName(
            self, "Export Objects to Excel",
            "homm3_map_objects.xlsx",
            "Excel files (*.xlsx)"
        )
        if not path:
            return
        try:
            self._set_loading("Создание Excel…")
            export_objects_to_excel(self.parsed_data or {}, self.raw_data or b"", path,
                                    objects_by_coord=self._load_objects_by_coord(),
                                    computed_offsets=self._load_objects_with_offsets())
            self._set_idle()
            self.status.showMessage(f"Exported {path}")
            QMessageBox.information(
                self, "Export Complete",
                f"All map objects exported to:\n{path}\n\n"
                "Sheets:\n"
                "  • All Objects     — 7009 objects with coords, types, wiki info\n"
                "  • By Category     — same objects grouped by 21 categories\n"
                "  • Summary         — counts per type/category + meta\n"
                "  • Wiki Dictionary — 2037 types from LazyLlama wiki"
            )
        except Exception as e:
            self._set_idle()
            self.status.showMessage("Export failed")
            QMessageBox.critical(self, "Error", f"Failed to export Excel:\n{e}")

    def _on_export_binary(self):
        """Export the decompressed save as a .bin file for hex-editor analysis."""
        if not self.raw_data:
            QMessageBox.warning(self, "Warning", "No save file loaded")
            return
        # Suggest a filename based on the loaded save
        if self.current_file:
            base = os.path.splitext(os.path.basename(self.current_file))[0]
            suggested = f"{base}_decompressed.bin"
        else:
            suggested = "save_decompressed.bin"
        path, _ = QFileDialog.getSaveFileName(
            self, "Export Binary", suggested, "Binary files (*.bin);;All files (*)"
        )
        if not path:
            return
        try:
            with open(path, "wb") as f:
                f.write(self.raw_data)
            self._set_idle()
            self.status.showMessage(f"Exported binary: {path}")
            QMessageBox.information(
                self, "Export Complete",
                f"Decompressed save exported to:\n{path}\n\n"
                f"Size: {len(self.raw_data):,} bytes (0x{len(self.raw_data):X})\n\n"
                f"You can now open this file in a hex editor (HxD, 010 Editor, etc.)\n"
                f"to inspect the raw bytes.\n\n"
                f"Tip: use the offsets shown in the 'Map Objects' tab to jump\n"
                f"directly to any object's location in the binary."
            )
        except Exception as e:
            self._set_idle()
            self.status.showMessage("Export failed")
            QMessageBox.critical(self, "Error", f"Failed to export binary:\n{e}")

    def _on_reload_mapping(self):
        self._load_mapping()
        self._refresh_mapping_text()
        if self.raw_data and self.mapping:
            self.parsed_data = parse_save(self.raw_data, self.mapping,
                                          map_config=getattr(self, "map_config", None))
            self._populate_tree()
            self._update_load_status()
            self.json_text.setPlainText(export_to_json(self.parsed_data, self.raw_data,
                                objects_by_coord=self._load_objects_by_coord(),
                                coord_int_lookup=getattr(self.map_data, "coord_int_lookup", {}) if self.map_data else {},
                                computed_offsets=self._load_objects_with_offsets()))
        self.status.showMessage("Mapping reloaded")

    # ----- Map JSON loader (makes parser universal for any map) -----
    def _on_load_map_json(self):
        """Open a dialog to load a pre-parsed .h3m map JSON file.

        This replaces the hardcoded 03_object_mapping/ files with
        dynamically-built object mapping from the user's map parser.
        After loading, the parser works for saves from ANY map.
        """
        path, _ = QFileDialog.getOpenFileName(
            self, "Load Map JSON",
            "",
            "Map JSON files (*.json);;Map ZIP files (*.zip);;All files (*)"
        )
        if not path:
            return

        try:
            from map_json_loader import load_map_json_safely
        except ImportError:
            # Try relative import
            import importlib.util
            spec = importlib.util.spec_from_file_location(
                "map_json_loader",
                os.path.join(os.path.dirname(os.path.abspath(__file__)),
                             "map_json_loader.py")
            )
            mod = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(mod)
            load_map_json_safely = mod.load_map_json_safely

        md, err = load_map_json_safely(path)
        if err:
            QMessageBox.critical(self, "Load Error",
                                 f"Failed to load map JSON:\n{err}")
            return

        # Store the loaded MapData
        self.map_data = md
        # Invalidate cached object data so _load_objects_by_coord returns the new data
        self._cached_objects_by_coord = md.objects_by_coord
        # Clear old offsets — will be recomputed if a save is loaded
        self._computed_offsets = {}
        self._cached_objects_with_offsets = {}

        # If a save is already loaded, recompute object offsets with the new map data
        if self.raw_data:
            self._set_loading("Вычисление адресов объектов…")
            self._compute_object_offsets_in_save()
            n_verified = sum(1 for v in self._computed_offsets.values() if v.get("verified"))

        # Rebuild the objects table and tree
        self._populate_objects_table()
        if self.parsed_data:
            self._populate_tree()
            self._update_load_status()

        # Update status
        n_ver = sum(1 for v in getattr(self, "_computed_offsets", {}).values() if v.get("verified")) if self.raw_data else 0
        self._set_idle()
        self.status.showMessage(
            f"Map JSON loaded: {md.summary()}"
            + (f" | {n_ver} objects located in save" if self.raw_data else ""),
            10000
        )
        QMessageBox.information(
            self, "Map JSON Loaded",
            f"Successfully loaded map JSON:\n\n"
            f"  Map:      {md.map_name}\n"
            f"  Size:     {md.map_size}×{md.map_size}"
            f"{' + underground' if md.has_underground else ''}\n"
            f"  Objects:  {md.n_objects}\n"
            f"  Towns:    {md.n_towns}\n"
            f"  Heroes:   {md.n_heroes}\n"
            f"  Players:  {md.n_players}\n"
            f"  Sprites:  {md.n_sprites}\n"
            f"  Unique coords: {len(md.objects_by_coord)}\n"
            + (f"\n  Objects located in save: {n_ver}\n" if self.raw_data else "")
            + f"\nThe parser now uses this map's object data.\n"
            f"Object tree and table have been rebuilt."
        )

    def _on_load_day_zero(self):
        """Load a day-zero save (save immediately after loading the map).
        Builds a per-map config by comparing the map JSON with this save.
        This config is then used to parse ANY subsequent save from the same map.

        Workflow:
          1. Load Map JSON (Ctrl+M) — required first
          2. Load Day-Zero Save (Ctrl+D) — this step, builds config
          3. Open any save (Ctrl+O) — parsed using the config
        """
        # Step 1: check that Map JSON is loaded
        if not self.map_data:
            QMessageBox.warning(
                self, "No Map JSON loaded",
                "You must load a Map JSON first (Ctrl+M).\n\n"
                "Workflow:\n"
                "  1. Load Map JSON… (Ctrl+M)\n"
                "  2. Load Day-Zero Save… (Ctrl+D)  ← you are here\n"
                "  3. Open any save (Ctrl+O)"
            )
            return

        # Step 2: open day-zero save
        path, _ = QFileDialog.getOpenFileName(
            self, "Load Day-Zero Save",
            "",
            "GM1 saves (*.gm1 *.GM1);;All files (*)"
        )
        if not path:
            return

        try:
            from map_config_builder import (
                decompress_save, build_map_config, save_config
            )
        except ImportError:
            import importlib.util
            spec = importlib.util.spec_from_file_location(
                "map_config_builder",
                os.path.join(os.path.dirname(os.path.abspath(__file__)),
                             "map_config_builder.py")
            )
            mod = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(mod)
            decompress_save = mod.decompress_save
            build_map_config = mod.build_map_config
            save_config = mod.save_config

        self._set_loading("Декомпрессия нулевого сейва…")

        try:
            dz_raw = decompress_save(path)
        except Exception as e:
            QMessageBox.critical(self, "Error", f"Failed to decompress save:\n{e}")
            return

        self._set_loading("Построение конфига карты…")

        try:
            config = build_map_config(self.map_data, dz_raw, path)
        except Exception as e:
            QMessageBox.critical(self, "Error", f"Failed to build config:\n{e}")
            return

        # Save config to file
        script_dir = os.path.dirname(os.path.abspath(__file__))
        toolkit_dir = os.path.normpath(os.path.join(script_dir, ".."))
        config_path = save_config(config, toolkit_dir)

        # Store config and day-zero raw data
        self.map_config = config
        self.day_zero_raw = dz_raw

        # Also use the day-zero save as the current raw_data (so user can browse it)
        self.raw_data = dz_raw
        self.current_file = path
        # v3.0: use the freshly-built MapConfig for parsing
        self.parsed_data = parse_save(self.raw_data, self.mapping,
                                       map_config=config)
        self.file_label.setText(f"{os.path.basename(path)} (day-zero, {len(dz_raw):,} bytes)")

        # Compute object offsets — Phase 3 returns them in parsed_data
        self._computed_offsets = {}
        if "object_offsets" in self.parsed_data and self.parsed_data["object_offsets"]:
            self._computed_offsets = self.parsed_data["object_offsets"]
            self._cached_objects_with_offsets = self._computed_offsets
        else:
            self._compute_object_offsets_in_save()

        n_verified = sum(1 for v in self._computed_offsets.values() if v.get("verified"))
        # config is now a MapConfig dataclass (v3.0), not a dict
        if hasattr(config, "meta"):
            # MapConfig dataclass
            n_heroes = config.hero_section.count
            n_towns = config.town_section.count
            n_clusters = len(config.clusters)
            map_name = config.meta.get("map_name", "?")
            day_zero_size = config.meta.get("day_zero_size", 0)
            hero_stride_hex = config.hero_section.stride_hex
            hero_stride = config.hero_section.stride
        else:
            # Legacy dict (shouldn't happen, but defensive)
            n_heroes = config["hero_section"]["count"]
            n_towns = config["town_section"]["count"]
            n_clusters = len(config["clusters"])
            map_name = config["_meta"]["map_name"]
            day_zero_size = config["_meta"]["day_zero_size"]
            hero_stride_hex = config["hero_section"]["stride_hex"]
            hero_stride = config["hero_section"]["stride"]

        self._populate_tree()
        self._update_load_status()
        self._populate_objects_table()

        self._set_idle()
        self.status.showMessage(
            f"Day-zero config built: {n_verified} objects, {n_heroes} heroes, "
            f"{n_towns} towns, {n_clusters} clusters", 10000
        )

        QMessageBox.information(
            self, "Day-Zero Config Built",
            f"Map config built and saved to:\n{config_path}\n\n"
            f"  Map:              {map_name}\n"
            f"  Save size:        {day_zero_size:,} bytes\n"
            f"  Objects located:  {n_verified}\n"
            f"  Heroes found:     {n_heroes}\n"
            f"  Towns found:      {n_towns}\n"
            f"  Clusters:         {n_clusters}\n"
            f"  Hero stride:      {hero_stride_hex} "
            f"({hero_stride} bytes)\n\n"
            f"You can now open ANY save from this map (Ctrl+O).\n"
            f"The parser will use this config for universal parsing."
        )

    def _on_load_map_config(self):
        """Load a previously-saved map_config_<mapname>.json (Phase 3 input).

        Lets the user skip Phase 2 (Day-Zero Save) when a config already exists.
        After loading, the user can directly open any save of this map (Ctrl+O)
        and it will be parsed using Phase 3 (save_parser.parse_save).

        Workflow:
          Option A (full):
            1. Load → Map JSON… (Ctrl+M)
            2. Load → Day-Zero Save… (Ctrl+D)  ← builds config
            3. Open any save (Ctrl+O)

          Option B (with pre-built config):
            1. Load → Map JSON… (Ctrl+M)        ← required for objects_on_map
            2b. Load → Map Config… (Ctrl+L)     ← you are here
            3. Open any save (Ctrl+O)
        """
        # Step 1: ask for the config file
        path, _ = QFileDialog.getOpenFileName(
            self, "Load Map Config",
            "",
            "Map Config JSON (*.json);;All files (*)"
        )
        if not path:
            return

        # Load the config via MapConfig.from_dict
        try:
            from save_layout import MapConfig
        except ImportError:
            import importlib.util
            tools_dir = os.path.dirname(os.path.abspath(__file__))
            spec = importlib.util.spec_from_file_location(
                "save_layout", os.path.join(tools_dir, "save_layout.py"))
            mod = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(mod)
            MapConfig = mod.MapConfig

        try:
            import json
            with open(path, "r", encoding="utf-8") as f:
                config_dict = json.load(f)
            config = MapConfig.from_dict(config_dict)
        except Exception as e:
            QMessageBox.critical(self, "Load Error",
                                 f"Failed to load map config:\n{e}")
            return

        # Store the loaded MapConfig
        self.map_config = config

        # If no Map JSON loaded yet, build objects_by_coord from config
        # (config contains object_offsets → coord_int → save_offset mapping,
        #  but no type/category/sprite info — that comes from map JSON)
        if not self.map_data:
            # Build minimal objects_by_coord from config
            # We have coord_int → save_offsets, but no type/category
            # Still useful for cluster verification and object table display
            from collections import defaultdict
            oc = {}
            for ci_str, off_data in config.object_offsets.items():
                ci = int(ci_str)
                # Decode coord_int → x:y:z
                x = ci & 0xFF
                y = (ci >> 8) & 0xFF
                z = (ci >> 16) & 0xFF
                ck = f"{x}:{y}:{z}"
                oc[ck] = {
                    "coord_key": ck,
                    "coord_int": ci,
                    "x": x, "y": y, "z": z,
                    "type": "(unknown — load Map JSON for type info)",
                    "category": "unknown",
                    "sprite_def": "",
                    "details": {},
                    "overlays": [],
                }
            if oc:
                self._cached_objects_by_coord = oc
                # Build minimal MapData-like object
                class _MinimalMapData:
                    def __init__(self, config):
                        self.map_name = config.meta.get("map_name", "?")
                        self.map_size = config.meta.get("map_size", 0)
                        self.has_underground = config.meta.get("has_underground", False)
                        self.n_objects = config.meta.get("n_objects", 0)
                        self.n_towns = config.meta.get("n_towns", 0)
                        self.n_heroes = config.meta.get("n_heroes_total", 0)
                        self.n_players = 8
                        self.n_sprites = 0
                        self.objects_by_coord = {}
                        self.coord_int_lookup = {}
                        self.type_index = defaultdict(list)
                        self.category_index = defaultdict(list)
                        # Populate from config
                        for ci_str, off in config.object_offsets.items():
                            ci = int(ci_str)
                            x = ci & 0xFF
                            y = (ci >> 8) & 0xFF
                            z = (ci >> 16) & 0xFF
                            ck = f"{x}:{y}:{z}"
                            rec = {
                                "coord_key": ck, "coord_int": ci,
                                "x": x, "y": y, "z": z,
                                "type": "(unknown)", "category": "unknown",
                                "sprite_def": "", "details": {},
                            }
                            self.objects_by_coord[ck] = rec
                            self.coord_int_lookup[ci] = ck
                    def summary(self):
                        return (f"Map: {self.map_name} ({self.map_size}×{self.map_size}"
                                f"{' + underground' if self.has_underground else ''}), "
                                f"{self.n_objects} objects, {self.n_towns} towns, "
                                f"{self.n_heroes} heroes (from MapConfig, no Map JSON)")

                self.map_data = _MinimalMapData(config)
                self._cached_objects_by_coord = self.map_data.objects_by_coord

        # Update status
        n_heroes = config.hero_section.count
        n_towns = config.town_section.count
        n_clusters = len(config.clusters)
        map_name = config.meta.get("map_name", "?")
        map_size = config.meta.get("map_size", 0)
        has_ug = config.meta.get("has_underground", False)
        n_objects = config.meta.get("n_objects", 0)
        has_map_json = "Map JSON" if self.map_data and hasattr(self.map_data, 'n_sprites') and self.map_data.n_sprites > 0 else "NO Map JSON"

        self._set_idle()
        self.status.showMessage(
            f"Map Config loaded: {map_name} | "
            f"{n_objects} objects, {n_heroes} heroes, {n_towns} towns, "
            f"{n_clusters} clusters ({has_map_json})", 10000
        )

        QMessageBox.information(
            self, "Map Config Loaded",
            f"Successfully loaded map config:\n\n"
            f"  Config file:  {os.path.basename(path)}\n"
            f"  Map:         {map_name}\n"
            f"  Size:        {map_size}×{map_size}"
            f"{' + underground' if has_ug else ''}\n"
            f"  Objects:     {n_objects}\n"
            f"  Heroes:       {n_heroes}\n"
            f"  Towns:        {n_towns}\n"
            f"  Clusters:     {n_clusters}\n\n"
            f"  Map JSON:     {has_map_json}\n"
            + ("" if has_map_json == "Map JSON" else
               "\n⚠ No Map JSON loaded — object types/categories are unknown.\n"
               "  Load Map JSON (Ctrl+M) for type info, sprite names, and\n"
               "  map description. Parsing still works without it.\n")
            + f"\nYou can now open ANY save from this map (Ctrl+O).\n"
            f"The parser will use Phase 3 (save_parser) for universal parsing."
        )


def main():
    app = QApplication(sys.argv)
    mapping_path = DEFAULT_MAPPING_PATH
    save_path = None
    if len(sys.argv) >= 2:
        if sys.argv[1].endswith(".json"):
            mapping_path = sys.argv[1]
            if len(sys.argv) >= 3:
                save_path = sys.argv[2]
        else:
            save_path = sys.argv[1]
    window = GM1ParserWindow(mapping_path=mapping_path)
    window.show()
    if save_path and os.path.exists(save_path):
        window.load_file(save_path)
    sys.exit(app.exec())


if __name__ == "__main__":
    main()
