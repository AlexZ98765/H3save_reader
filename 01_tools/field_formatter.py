#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
field_formatter.py — Format parser fields with full address + hex info,
mirroring the .h3m.json structure used by the map parser.

Each field gets a dict with:
  - first_addr      (decimal offset in save)
  - first_addr_hex  ("0x...")
  - length          (size in bytes)
  - value_type      ("u8", "u16", "u32", "i32", "cp1251", "bytes", "list", ...)
  - value           (human-readable)
  - value_int        (integer value if applicable)
  - value_hex        (raw bytes as hex string "AA BB CC")
  - value_bin        (binary representation "AAAAAAAA BBBBBBBB")

This is the SAME format used by the .h3m.json map parser — so users get
consistent field structures across both map and save data.
"""

from __future__ import annotations
import struct
from typing import Any, Dict, List, Optional, Tuple


def _bytes_to_hex(b: bytes) -> str:
    """Convert bytes to hex string separated by spaces: 'AA BB CC'."""
    return " ".join(f"{x:02x}" for x in b)


def _bytes_to_bin(b: bytes) -> str:
    """Convert bytes to binary string: 'AAAAAAAA BBBBBBBB'."""
    return " ".join(f"{x:08b}" for x in b)


def _bytes_to_int_le(b: bytes, signed: bool = False) -> Optional[int]:
    """Convert bytes to little-endian integer."""
    if not b:
        return None
    try:
        if len(b) == 1:
            return struct.unpack("<b" if signed else "<B", b)[0]
        elif len(b) == 2:
            return struct.unpack("<h" if signed else "<H", b)[0]
        elif len(b) == 4:
            return struct.unpack("<i" if signed else "<I", b)[0]
        elif len(b) == 8:
            return struct.unpack("<q" if signed else "<Q", b)[0]
        else:
            return int.from_bytes(b, "little", signed=signed)
    except Exception:
        return None


def make_field(name: str, addr: int, length: int, value_type: str,
               value: Any, raw_bytes: bytes,
               value_int: Optional[int] = None,
               value_dec: Optional[int] = None) -> Dict[str, Any]:
    """Build a field dict in the .h3m.json style.

    Args:
        name:        field name (used for context, not stored in dict)
        addr:        absolute byte offset in the save
        length:      field length in bytes
        value_type:  type string ("u8", "u16", "u32", "cp1251", "bytes", etc.)
        value:       human-readable value (string, int, list, etc.)
        raw_bytes:   raw bytes from the save
        value_int:   integer value (if different from value, e.g. for enums)
        value_dec:   decimal value (for display)

    Returns:
        Dict with keys: first_addr, first_addr_hex, length, value_type,
        value, value_int, value_dec, value_hex, value_bin
    """
    field_dict = {
        "first_addr": int(addr),
        "first_addr_hex": f"0x{addr:x}",
        "length": int(length),
        "value_type": value_type,
        "value": value,
    }
    if value_int is not None:
        field_dict["value_int"] = value_int
    elif isinstance(value, int):
        field_dict["value_int"] = value
    else:
        field_dict["value_int"] = _bytes_to_int_le(raw_bytes)
    if value_dec is not None:
        field_dict["value_dec"] = value_dec
    else:
        field_dict["value_dec"] = field_dict.get("value_int")
    field_dict["value_hex"] = _bytes_to_hex(raw_bytes) if raw_bytes else ""
    field_dict["value_bin"] = _bytes_to_bin(raw_bytes) if raw_bytes else ""
    return field_dict


def make_u8_field(name: str, raw: bytes, addr: int) -> Dict[str, Any]:
    """Build a u8 field dict."""
    b = raw[addr:addr + 1]
    v = b[0] if b else 0
    return make_field(name, addr, 1, "u8", v, b)


def make_u16_field(name: str, raw: bytes, addr: int, signed: bool = False) -> Dict[str, Any]:
    """Build a u16/i16 field dict."""
    b = raw[addr:addr + 2]
    if len(b) < 2:
        b = b + b"\x00" * (2 - len(b))
    v = struct.unpack("<h" if signed else "<H", b)[0]
    return make_field(name, addr, 2, "i16" if signed else "u16", v, b)


def make_u32_field(name: str, raw: bytes, addr: int, signed: bool = False) -> Dict[str, Any]:
    """Build a u32/i32 field dict."""
    b = raw[addr:addr + 4]
    if len(b) < 4:
        b = b + b"\x00" * (4 - len(b))
    v = struct.unpack("<i" if signed else "<I", b)[0]
    return make_field(name, addr, 4, "i32" if signed else "u32", v, b)


def make_bool_field(name: str, raw: bytes, addr: int) -> Dict[str, Any]:
    """Build a bool field dict (1 byte, 0 or 1)."""
    b = raw[addr:addr + 1]
    v = bool(b[0]) if b else False
    return make_field(name, addr, 1, "bool", v, b, value_int=int(v))


def make_cp1251_field(name: str, raw: bytes, addr: int, length: int) -> Dict[str, Any]:
    """Build a cp1251 string field dict."""
    b = raw[addr:addr + length]
    try:
        s = b.decode("cp1251", errors="replace").rstrip("\x00")
    except Exception:
        s = b.decode("latin-1", errors="replace").rstrip("\x00")
    return make_field(name, addr, length, "cp1251", s, b)


def make_ascii_field(name: str, raw: bytes, addr: int, length: int) -> Dict[str, Any]:
    """Build an ASCII string field dict."""
    b = raw[addr:addr + length]
    s = b.decode("ascii", errors="replace").rstrip("\x00")
    return make_field(name, addr, length, "ascii", s, b)


def make_bytes_field(name: str, raw: bytes, addr: int, length: int,
                     value_type: str = "bytes") -> Dict[str, Any]:
    """Build a raw bytes field dict (e.g. for bitmasks, unknown data)."""
    b = raw[addr:addr + length]
    return make_field(name, addr, length, value_type, b.hex(), b,
                      value_int=None, value_dec=None)


def make_list_field(name: str, raw: bytes, addr: int, item_size: int, item_count: int,
                    value_type: str = "u32", signed: bool = False) -> Dict[str, Any]:
    """Build a list field dict (array of integers).

    Args:
        name:        field name
        raw:         raw save bytes
        addr:        starting offset
        item_size:   size of each item in bytes (1, 2, 4, 8)
        item_count:  number of items
        value_type:  type string for display ("u32", "u16", "u8")
        signed:      if True, interpret as signed
    """
    total_len = item_size * item_count
    b = raw[addr:addr + total_len]
    items = []
    for i in range(item_count):
        chunk = b[i * item_size:(i + 1) * item_size]
        if len(chunk) < item_size:
            chunk = chunk + b"\x00" * (item_size - len(chunk))
        v = _bytes_to_int_le(chunk, signed=signed)
        items.append(v if v is not None else 0)
    return make_field(name, addr, total_len, f"list_{value_type}", items, b,
                      value_int=None, value_dec=None)


def make_coords_field(name: str, raw: bytes, addr: int) -> Dict[str, Any]:
    """Build a 3-byte coordinates field dict (x, y, z)."""
    b = raw[addr:addr + 3]
    if len(b) < 3:
        b = b + b"\x00" * (3 - len(b))
    x, y, z = b[0], b[1], b[2]
    value_str = f"({x},{y},{z})"
    return make_field(name, addr, 3, "coords3", value_str, b,
                      value_int=None, value_dec=None)


def make_string_field(name: str, raw: bytes, addr: int, length: int,
                      encoding: str = "cp1251") -> Dict[str, Any]:
    """Build a variable-length string field dict (length-prefixed).

    For .GM1 saves, strings are length-prefixed with a u16 length,
    then the string bytes follow.
    """
    # Read length prefix
    len_b = raw[addr:addr + 2]
    if len(len_b) < 2:
        return make_field(name, addr, 2, f"str_{encoding}", "", len_b)
    str_len = struct.unpack("<H", len_b)[0]
    if str_len <= 0 or str_len > length:
        return make_field(name, addr, 2, f"str_{encoding}", "", len_b)
    str_start = addr + 2
    str_end = str_start + str_len
    str_b = raw[str_start:str_end]
    try:
        s = str_b.decode(encoding, errors="replace").rstrip("\x00")
    except Exception:
        s = str_b.decode("latin-1", errors="replace").rstrip("\x00")
    full_b = len_b + str_b
    return make_field(name, addr, 2 + str_len, f"str_{encoding}", s, full_b)


# ============================================================================
# Helpers for building "detailed" field dicts from existing simple fields
# ============================================================================

def enrich_field(name: str, value: Any, addr: int, length: int,
                 value_type: str, raw: bytes) -> Dict[str, Any]:
    """Take an existing simple value + metadata and wrap it in .h3m.json format.

    This is used to enrich fields that were already parsed by parse_hero_block
    / parse_town_block — without re-reading from raw.

    Args:
        name:        field name
        value:       already-parsed value (int, str, list, etc.)
        addr:        absolute offset in save
        length:      length in bytes
        value_type:  type string
        raw:         raw bytes from save at addr (length bytes)
    """
    f = make_field(name, addr, length, value_type, value, raw)
    return f
