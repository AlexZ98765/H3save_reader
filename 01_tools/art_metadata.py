#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
art_metadata.py — Universal artifact metadata loader.

Loads three data files (all extracted from PRT MainForm.cs source code):
  1. 02_format_docs/tbl_art_mapping.json         — 144 artifacts: id → {name, class_id, relic_c_id, cost}
  2. 02_format_docs/artifact_tile_parsers.json   — per-type_id tile-level artifact extraction config
  3. 02_format_docs/bank_artifact_config.json     — per-bank-type_id is_artifact_bank + default_slots

This module is data-driven: no Python hardcodes any artifact_id → class/relic mapping,
no Python hardcodes any type_id → behavior. All lookups come from JSON config files.

Universal: works for any save of any map. Adding a new artifact-bearing object type
just requires adding an entry to the appropriate JSON file.
"""
from __future__ import annotations
import json
import os
from typing import Any, Dict, List, Optional


_HERE = os.path.dirname(os.path.abspath(__file__))
_DOCS = os.path.normpath(os.path.join(_HERE, "..", "02_format_docs"))

TBL_ART_PATH = os.path.join(_DOCS, "tbl_art_mapping.json")
TILE_PARSERS_PATH = os.path.join(_DOCS, "artifact_tile_parsers.json")
BANK_CONFIG_PATH = os.path.join(_DOCS, "bank_artifact_config.json")


# ----------------------------------------------------------------------------
# Lazy loaders (cached after first call)
# ----------------------------------------------------------------------------

_TBL_ART: Optional[Dict[str, Dict[str, Any]]] = None
_TILE_PARSERS: Optional[Dict[str, Dict[str, Any]]] = None
_BANK_CONFIG: Optional[Dict[str, Dict[str, Any]]] = None


def _load_json(path: str) -> Dict[str, Any]:
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)


def _load_tbl_art() -> Dict[str, Dict[str, Any]]:
    global _TBL_ART
    if _TBL_ART is None:
        data = _load_json(TBL_ART_PATH)
        # Strip _meta key if present
        _TBL_ART = {k: v for k, v in data.items() if not k.startswith("_")}
    return _TBL_ART


def _load_tile_parsers() -> Dict[str, Dict[str, Any]]:
    global _TILE_PARSERS
    if _TILE_PARSERS is None:
        data = _load_json(TILE_PARSERS_PATH)
        _TILE_PARSERS = {k: v for k, v in data.items() if not k.startswith("_")}
    return _TILE_PARSERS


def _load_bank_config() -> Dict[str, Dict[str, Any]]:
    global _BANK_CONFIG
    if _BANK_CONFIG is None:
        data = _load_json(BANK_CONFIG_PATH)
        _BANK_CONFIG = {k: v for k, v in data.items() if not k.startswith("_")}
    return _BANK_CONFIG


# ----------------------------------------------------------------------------
# TblArt mapping (artifact_id → class/relic/cost)
# ----------------------------------------------------------------------------

def get_artifact_class_id(artifact_id: int) -> int:
    """Return class_id (0..5) for the given artifact_id.

    Class IDs (from PRT MainForm.cs aArtClass):
      0 = Machine, 1 = Treasure, 2 = Minor, 3 = Major, 4 = Relic, 5 = Relic-C
    Returns 0 (Machine) if artifact_id not in TblArt.
    """
    tbl = _load_tbl_art()
    entry = tbl.get(str(artifact_id))
    if entry is None:
        return 0
    return int(entry.get("class_id", 0))


def get_artifact_relic_c_id(artifact_id: int) -> Optional[int]:
    """Return relic_c_id (combination relic ID) if this artifact is part of one.

    Returns None if the artifact is not part of any combination relic.
    The returned ID can be passed to localizer.art(id) to get the relic name.
    """
    tbl = _load_tbl_art()
    entry = tbl.get(str(artifact_id))
    if entry is None:
        return None
    v = entry.get("relic_c_id")
    if v is None:
        return None
    return int(v)


def get_artifact_cost(artifact_id: int) -> int:
    """Return gold cost (from PRT TblArt) for the given artifact_id. 0 if unknown."""
    tbl = _load_tbl_art()
    entry = tbl.get(str(artifact_id))
    if entry is None:
        return 0
    return int(entry.get("cost", 0))


def get_artifact_name(artifact_id: int, localizer=None) -> str:
    """Return localized artifact name via the provided localizer.

    If localizer is None, returns the raw name from TblArt (which is the PRT
    source's hardcoded Russian name; the actual save-specific name should
    come through the localizer).
    """
    if localizer is not None:
        return localizer.art(artifact_id)
    tbl = _load_tbl_art()
    entry = tbl.get(str(artifact_id))
    return entry.get("name", "") if entry else ""


def is_relic_combo_artifact(artifact_id: int) -> bool:
    """Return True if this artifact is itself a combination relic (class_id == 5)."""
    return get_artifact_class_id(artifact_id) == 5


# ----------------------------------------------------------------------------
# Tile-level artifact extraction (per type_id)
# ----------------------------------------------------------------------------

def get_artifact_tile_parser(type_id: int) -> Optional[Dict[str, Any]]:
    """Return the tile-level artifact extraction config for the given type_id.

    Returns None if this type_id does not bear artifacts at the tile level
    (e.g., type_id=5 returns its config; type_id=99 returns None).

    Config schema (see 02_format_docs/artifact_tile_parsers.json):
      save_method: str        — name of the PRT Save* method
      always_row: bool        — does this type always create an R_Art row?
      artifact_id_encoding:
        "direct_u8"           — artifact_id is raw[base + offset]
        "formula"             — artifact_id is computed via artifact_id_formula
      artifact_id_offset: int     — (direct_u8 only) byte offset
      artifact_id_offsets: [int]  — (formula only) byte offsets used in formula
      artifact_id_formula: str   — (formula only) Python expression using raw[s+X]
      has_artifact_check: dict   — {offset, value} if a byte must equal `value`
                                    for an artifact to be present
    """
    parsers = _load_tile_parsers()
    return parsers.get(str(type_id))


def get_artifact_tile_type_ids() -> List[int]:
    """Return list of type_ids that have tile-level artifact extraction configs."""
    parsers = _load_tile_parsers()
    return sorted(int(k) for k in parsers.keys())


def is_artifact_bearing_tile_type(type_id: int) -> bool:
    """Return True if this type_id has a tile-level artifact parser."""
    return str(type_id) in _load_tile_parsers()


def links_art_res(type_id: int) -> bool:
    """Return True if this tile type links to a post-tile ArtRes record for guard info.

    Per PRT MainForm.cs SaveArt (line 10145): only type_id=5 (SaveArt) adds a
    TblArtRes row when (raw[s+9] & 0x80) == 0x80. Other tile-level parsers
    (Corpse, Survivor, Tomb) do NOT link to ArtRes records.

    This is data-driven: the JSON config has `"links_art_res": true` for type 5.
    """
    cfg = get_artifact_tile_parser(type_id)
    if cfg is None:
        return False
    return bool(cfg.get("links_art_res", False))


def get_art_res_num(raw: bytes, s: int, type_id: int) -> int:
    """Compute the ArtRes "Num" field for this tile object.

    Per PRT SaveArt (line 10151): Num = ((raw[s+9] & 0x7F) << 8) + (raw[s+8] >> 3).
    Used to link ground artifact to its corresponding ArtRes record by index.

    Returns 0 if the type doesn't have an art_res_num_formula.
    """
    cfg = get_artifact_tile_parser(type_id)
    if cfg is None:
        return 0
    formula = cfg.get("art_res_num_formula")
    if not formula:
        return 0
    # Replace raw[s+X] with actual values
    expr = formula
    for off in [6, 7, 8, 9]:
        expr = expr.replace(f"raw[s+{off}]", str(raw[s + off]) if s + off < len(raw) else "0")
    try:
        return int(eval(expr, {"__builtins__": {}}, {}))
    except Exception:
        return 0


def has_artifact_guard_check(raw: bytes, s: int, type_id: int) -> bool:
    """Check if this tile object has a guard (per has_guard_check config).

    Per PRT SaveArt (line 10145): has_guard iff (raw[s+9] & 0x80) == 0x80.

    Returns False if the type has no has_guard_check config.
    """
    cfg = get_artifact_tile_parser(type_id)
    if cfg is None:
        return False
    check = cfg.get("has_guard_check")
    if check is None:
        return False
    off = check.get("offset", 0)
    mask = check.get("mask", 0)
    if s + off >= len(raw):
        return False
    return (raw[s + off] & mask) == mask


def extract_artifact_id_from_tile(raw: bytes, s: int,
                                  type_id: int) -> Optional[int]:
    """Extract the artifact_id from tile bytes for the given type_id.

    Returns:
      int artifact_id (0..143) — if the tile has an artifact
      None — if the tile has no artifact (e.g., empty Corpse, empty Tomb)

    Returns None if the type_id has no parser, or if the has_artifact_check
    byte doesn't match the expected value.
    """
    cfg = get_artifact_tile_parser(type_id)
    if cfg is None:
        return None

    # Check has_artifact_check (e.g., raw[s+8] == 0xFF for Corpse, raw[s+6] == 31 for Tomb)
    check = cfg.get("has_artifact_check")
    if check is not None:
        off = check["offset"]
        expected = check["value"]
        if s + off >= len(raw):
            return None
        if raw[s + off] != expected:
            return None

    # Extract artifact_id
    encoding = cfg.get("artifact_id_encoding")
    if encoding == "direct_u8":
        off = cfg.get("artifact_id_offset", 0)
        if s + off >= len(raw):
            return None
        v = raw[s + off]
        # For Survivor, raw[s+6] is always the artifact_id directly
        return int(v) if v != 0xFF else None

    if encoding == "formula":
        # Build a local namespace with raw[s+X] for each offset
        offsets = cfg.get("artifact_id_offsets", [])
        local_ns = {}
        for off in offsets:
            if s + off < len(raw):
                local_ns[f"raw_s_plus_{off}"] = raw[s + off]
        # The formula uses raw[s+X] notation — translate to our namespace
        formula = cfg.get("artifact_id_formula", "")
        # Replace "raw[s+X]" with the value
        expr = formula
        for off in offsets:
            expr = expr.replace(f"raw[s+{off}]", str(raw[s + off]) if s + off < len(raw) else "0")
        try:
            v = eval(expr, {"__builtins__": {}}, {})
            return int(v) if v is not None else None
        except Exception:
            return None

    return None


# ----------------------------------------------------------------------------
# Bank artifact config (per bank type_id)
# ----------------------------------------------------------------------------

def get_bank_config(type_id: int) -> Optional[Dict[str, Any]]:
    """Return the bank artifact config for the given bank type_id. None if not a bank."""
    return _load_bank_config().get(str(type_id))


def is_artifact_bank(type_id: int) -> bool:
    """Return True if the bank with this type_id is an artifact bank (has artifact slots).

    Based on PRT SaveBanks logic: code==16 → false; code in {24, 25, 84, 85} → true.
    """
    cfg = get_bank_config(type_id)
    if cfg is None:
        return False
    return bool(cfg.get("is_artifact_bank", False))


def get_bank_default_slots(type_id: int) -> int:
    """Return the default number of artifact slots for this bank type_id.

    PRT source: code 25 (Dragon Utopia) = 4 slots; code 24, 84, 85 = 1 slot; code 16 = 0.
    Note: the actual slot count is read dynamically from bank content's `n` byte;
    this is just the default expected count for tile-level row generation.
    """
    cfg = get_bank_config(type_id)
    if cfg is None:
        return 0
    return int(cfg.get("default_slots", 0))


def get_bank_type_ids() -> List[int]:
    """Return list of all bank type_ids (16, 24, 25, 84, 85)."""
    return sorted(int(k) for k in _load_bank_config().keys())


def is_bank_type(type_id: int) -> bool:
    """Return True if this type_id is a bank (any kind, artifact or not)."""
    return str(type_id) in _load_bank_config()


# ----------------------------------------------------------------------------
# Combined: all artifact-bearing type_ids (tile-level + banks)
# ----------------------------------------------------------------------------

def get_all_artifact_bearing_type_ids() -> List[int]:
    """Return list of all type_ids that produce rows in the Артефакты sheet.

    Includes both tile-level artifact objects (5, 22, 86, 108) and
    artifact banks (16, 24, 25, 84, 85).
    """
    tile_ids = get_artifact_tile_type_ids()
    bank_ids = get_bank_type_ids()
    return sorted(set(tile_ids) | set(bank_ids))


# ----------------------------------------------------------------------------
# Self-test
# ----------------------------------------------------------------------------

if __name__ == "__main__":
    print("=== TblArt mapping ===")
    print(f"  Loaded {len(_load_tbl_art())} artifacts")
    print(f"  art[110] class_id={get_artifact_class_id(110)}, relic_c_id={get_artifact_relic_c_id(110)}, cost={get_artifact_cost(110)}")
    print(f"  art[7]   class_id={get_artifact_class_id(7)}, relic_c_id={get_artifact_relic_c_id(7)}, cost={get_artifact_cost(7)}")
    print(f"  art[140] is_relic_combo={is_relic_combo_artifact(140)} (should be True)")

    print("\n=== Tile parsers ===")
    for tid in get_artifact_tile_type_ids():
        cfg = get_artifact_tile_parser(tid)
        print(f"  type {tid}: {cfg.get('save_method')} (always_row={cfg.get('always_row')})")

    print("\n=== Bank config ===")
    for tid in get_bank_type_ids():
        cfg = get_bank_config(tid)
        print(f"  type {tid}: is_artifact_bank={is_artifact_bank(tid)}, default_slots={get_bank_default_slots(tid)}")

    print("\n=== All artifact-bearing type_ids ===")
    print(f"  {get_all_artifact_bearing_type_ids()}")

    # Test extract_artifact_id_from_tile with a dummy byte sequence
    # For Tomb (type 108): if raw[s+6] == 31, has artifact
    # artifact_id = ((raw[s+8] & 0x7F) << 3) + (raw[s+7] >> 5)
    # If we want artifact_id = 110 (Кольцо драгоценных камней):
    #   110 = ((s8 & 0x7F) << 3) + (s7 >> 5)
    #   110 / 8 = 13 r 6 → s8 = 13, s7 >> 5 = 6 → s7 = 6 << 5 = 192
    #   So raw[s+6]=31, raw[s+7]=192, raw[s+8]=13
    test_raw = bytes([108, 0, 0, 0, 0, 0, 31, 192, 13, 0, 0, 0])
    aid = extract_artifact_id_from_tile(test_raw, 0, 108)
    print(f"\n  Test Tomb (type 108) with raw[6]=31, raw[7]=192, raw[8]=13 → artifact_id={aid} (expected 110)")

    # Test Corpse (type 22): if raw[s+8] == 0xFF, has artifact
    # artifact_id = (raw[s+7] << 2) + ((raw[s+6] & 0xC0) >> 6)
    # For artifact_id = 55 (Мантия Вампира):
    #   55 = (s7 << 2) + ((s6 & 0xC0) >> 6)
    #   55 / 4 = 13 r 3 → s7 = 13, (s6 & 0xC0) >> 6 = 3 → s6 = 3 << 6 = 192
    test_raw = bytes([22, 0, 0, 0, 0, 0, 192, 13, 255, 0, 0, 0])
    aid = extract_artifact_id_from_tile(test_raw, 0, 22)
    print(f"  Test Corpse (type 22) with raw[6]=192, raw[7]=13, raw[8]=255 → artifact_id={aid} (expected 55)")

    # Test Corpse without artifact (raw[s+8] != 0xFF)
    test_raw = bytes([22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0])
    aid = extract_artifact_id_from_tile(test_raw, 0, 22)
    print(f"  Test Corpse (type 22) empty → artifact_id={aid} (expected None)")

    # Test Survivor (type 86): raw[s+6] is direct artifact_id
    test_raw = bytes([86, 0, 0, 0, 0, 0, 47, 0, 0, 0, 0, 0])
    aid = extract_artifact_id_from_tile(test_raw, 0, 86)
    print(f"  Test Survivor (type 86) raw[6]=47 → artifact_id={aid} (expected 47)")

    # Test ground artifact (type 5): raw[s+2] is direct artifact_id
    test_raw = bytes([5, 0, 110, 0, 0, 0, 0, 0, 0, 0, 0, 0])
    aid = extract_artifact_id_from_tile(test_raw, 0, 5)
    print(f"  Test Artifact (type 5) raw[2]=110 → artifact_id={aid} (expected 110)")
