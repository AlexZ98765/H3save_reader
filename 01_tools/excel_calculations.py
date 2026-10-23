"""excel_calculations.py — Excel-only calculations and aggregations.

This module contains ALL computations that are needed for the Excel report
(∑ HP, future aggregators like Все Арты / Все Заклы / Все Навыки / Опыт)
but should NOT pollute the JSON output or the tree view shown in the GUI.

Architecture rule:
  - Parsing modules (save_parser, merged_objects, tile_scanner, post_tile_parser,
    art_metadata) must NOT compute these derived values — they only parse raw
    bytes and store raw data (including name lookups, which are parsing utilities).
  - This module reads raw data + loads tbl_monsters_mapping.json and computes
    Excel-only derived fields (∑ HP, aggregations).
  - Only `excel_export.py` imports this module. JSON exporters (`export_to_json`,
    `parsed_save_to_dict`) and tree view (`_populate_tree` in gm1_parser) must
    NOT use it.

What goes HERE (calculations/aggregations):
  - get_monster_hp(monster_id) — HP per unit (a CALCULATION-related lookup,
    used only to compute ∑ HP — NOT used by parsing modules for naming).
  - compute_army_hp(army_types, army_counts) — ∑ HP for 7-slot army.
  - compute_guard_hp(guard_list) — ∑ HP for guard slots.
  - hero_sum_hp, town_sum_hp, prison_sum_hp, artifact_sum_hp,
    resource_sum_hp, bank_sum_hp, event_box_sum_hp, monster_sum_hp
    (sheet-specific helpers used by excel_export only).

What STAYS in art_metadata.py (parsing utilities — name lookups):
  - get_monster_info, get_monster_name, get_monster_level — used by
    merge_monsters to populate JSON output with monster names.
  - All artifact metadata (TblArt lookups).

Data files loaded:
  - 02_format_docs/tbl_monsters_mapping.json — 145 monsters: {id → {name, level, hp, ...}}
    (extracted from PRT MainForm.cs TblMonsters.Rows.Add calls; column 3 = HP)

PRT references for ∑ HP computation:
  - GetTownContent (MainForm.cs:7493): `num2 += (int)dataRow[3] * num3`
  - GetHeroesContent (MainForm.cs:7790): `num3 += (int)dataRow[3] * num4`
  - ArtResContent (MainForm.cs:8413): `num += (int)dataRow[3] * num2`
  - EventBoxContent (MainForm.cs:8205): `num6 += (int)dataRow[3] * num7`
  - BankContent (line 8918): `r_BankRow.HP = hp`
  Column 3 of TblMonsters = HP per unit.
"""
from __future__ import annotations
import json
import os
from typing import Any, Dict, List, Optional

_HERE = os.path.dirname(os.path.abspath(__file__))
_DOCS = os.path.normpath(os.path.join(_HERE, "..", "02_format_docs"))
TBL_MONSTERS_PATH = os.path.join(_DOCS, "tbl_monsters_mapping.json")

# Lazy-loaded cache
_TBL_MONSTERS: Optional[Dict[str, Dict[str, Any]]] = None


def _load_tbl_monsters() -> Dict[str, Dict[str, Any]]:
    """Load and cache tbl_monsters_mapping.json (PRT TblMonsters table).
    Note: art_metadata.py also loads the same file for get_monster_info name
    lookups (used by parsing modules). This is a small duplication, kept
    separate so this module is self-contained for Excel-only computations.
    """
    global _TBL_MONSTERS
    if _TBL_MONSTERS is None:
        try:
            with open(TBL_MONSTERS_PATH, "r", encoding="utf-8") as f:
                _TBL_MONSTERS = {
                    k: v for k, v in json.load(f).items() if not k.startswith("_")
                }
        except Exception:
            _TBL_MONSTERS = {}
    return _TBL_MONSTERS


# ============================================================================
# Monster HP lookup (PRT TblMonsters column 3) — Excel-only
# Used to compute ∑ HP; not used by parsing modules.
# ============================================================================

def get_monster_hp(monster_id: int) -> int:
    """Return HP per unit for the given monster_id. 0 if unknown/unused.

    PRT TblMonsters column 3 = HP per unit (per MainForm.cs:7493 GetTownContent:
    `num2 += (int)dataRow[3] * num3` where dataRow[3] is HP and num3 is count).

    Monster IDs 0..148 are valid (excluding 122/124/126/128 which are
    "НЕ ИСПОЛЬЗУЕТСЯ" placeholders). IDs ≥ 149 don't exist.
    """
    entry = _load_tbl_monsters().get(str(monster_id))
    if entry is None:
        return 0
    return int(entry.get("hp", 0))


# ============================================================================
# ∑ HP computation for various army / guard formats
# ============================================================================

def compute_army_hp(army_types, army_counts) -> int:
    """Compute ∑ HP for an army (7 slots): sum of (monster_hp × count) for each
    non-empty slot. Matches PRT GetTownContent/GetHeroesContent logic.

    Args:
        army_types:  list of 7 monster IDs (or -1 / 0xFFFFFFFF for empty slot)
        army_counts: list of 7 counts (0 for empty slot)

    Returns:
        int total HP, or 0 if no valid monsters.
    """
    if not army_types or not army_counts:
        return 0
    total = 0
    for i in range(min(len(army_types), len(army_counts))):
        mid = army_types[i]
        cnt = army_counts[i]
        # Treat -1, 0xFFFFFFFF, 0xFF, None as empty slot
        if mid is None or not isinstance(mid, int) or mid < 0 or mid >= 0xFF:
            continue
        if not cnt or cnt <= 0:
            continue
        total += get_monster_hp(mid) * cnt
    return total


def compute_guard_hp(guard_list) -> int:
    """Compute ∑ HP for a guard (list of {monster_id, count} slots).

    Used for: ArtRes guard (artifact/resource tiles with guard),
    EventBox guard (Pandora/event guard), Bank guard.

    PRT ArtResContent (MainForm.cs:8413):
        `num += (int)dataRow[3] * num2` for each non-empty guard slot.
    PRT EventBoxContent (MainForm.cs:8205):
        `num6 += (int)dataRow[3] * num7` for each non-empty guard slot.
    """
    if not guard_list:
        return 0
    total = 0
    for g in guard_list:
        # Guard slots may be dicts OR dataclasses (ArmySlot)
        if isinstance(g, dict):
            mid = g.get("monster_id", -1)
            cnt = g.get("count", 0)
        else:
            mid = getattr(g, "monster_id", -1)
            cnt = getattr(g, "count", 0)
        if isinstance(mid, int) and 0 <= mid < 0xFF and cnt:
            total += get_monster_hp(mid) * cnt
    return total


# ============================================================================
# Sheet-specific helpers — used by excel_export.py only.
# Each function takes a record (from parsed_data["merged_objects"][sheet]
# or parsed_data["heroes_found"][i] / parsed_data["towns_found"][i]) and
# returns the ∑ HP value to write to the Excel cell.
# ============================================================================

def hero_sum_hp(hero_dict: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a hero from parsed_data["heroes_found"][i].

    Returns None for non-active heroes (those with Place=None or "Запрещен"),
    matching PRT MainForm.cs:7793 condition:
        `if (!r_HeroesRow.IsPlaceNull() && r_HeroesRow.Place != aPlace[2] && text != "")`

    Reads `army_types` and `army_counts` from hero's `fields` dict.
    """
    if not isinstance(hero_dict, dict):
        return None
    place = hero_dict.get("place")
    # PRT: only show when Place is set AND != "Запрещен"
    if not place or place == "Запрещен":
        return None
    f = hero_dict.get("fields", {}) or {}
    at = f.get("army_types", []) or []
    ac = f.get("army_counts", []) or []
    hp = compute_army_hp(at, ac)
    return hp if hp > 0 else None


def town_sum_hp(town_dict: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a town's garrison.

    Reads `army_types` and `army_counts` from town's `fields` dict
    (or top-level if it's a `towns` dict, not `towns_found`).
    Matches PRT GetTownContent (MainForm.cs:7493).
    """
    if not isinstance(town_dict, dict):
        return None
    f = town_dict.get("fields", {}) or {}
    # towns_found stores army_types/counts in fields; towns stores them at top-level
    at = f.get("army_types", []) or town_dict.get("army_types", []) or []
    ac = f.get("army_counts", []) or town_dict.get("army_counts", []) or []
    hp = compute_army_hp(at, ac)
    return hp if hp > 0 else None


def prison_sum_hp(prison_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a prison's hero army.

    Reads `army_types`/`army_counts` from prison rec (if stored by merge_prisons)
    OR from prison's `fields` sub-dict (if it wraps hero data).

    Matches PRT MainForm.cs:7790 GetHeroesContent logic.
    """
    if not isinstance(prison_rec, dict):
        return None
    at = prison_rec.get("army_types", []) or []
    ac = prison_rec.get("army_counts", []) or []
    if not at:
        # Try fields sub-dict
        f = prison_rec.get("fields", {}) or {}
        at = f.get("army_types", []) or []
        ac = f.get("army_counts", []) or []
    hp = compute_army_hp(at, ac)
    return hp if hp > 0 else None


def artifact_sum_hp(artifact_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for an artifact tile's guard.

    merge_artifacts stores `guard` field (list of {monster_id, count} dicts).
    Matches PRT ArtResContent (MainForm.cs:8413).
    """
    if not isinstance(artifact_rec, dict):
        return None
    guard = artifact_rec.get("guard")
    if not guard:
        return None
    hp = compute_guard_hp(guard)
    return hp if hp > 0 else None


def resource_sum_hp(resource_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a resource tile's guard (same logic as artifact)."""
    return artifact_sum_hp(resource_rec)


def bank_sum_hp(bank_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a bank's guard.

    merge_banks stores `guard` field (list of {monster_id, count} dicts).
    Matches PRT BankContent (MainForm.cs:8918).
    """
    return artifact_sum_hp(bank_rec)


def event_box_sum_hp(event_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for an event/Pandora's box guard.

    merge_event_boxes stores `guard_slots` field (raw list of {monster_id, count})
    alongside the formatted `guard` string. We use `guard_slots` for HP computation.
    Matches PRT EventBoxContent (MainForm.cs:8205).
    """
    if not isinstance(event_rec, dict):
        return None
    # Prefer guard_slots (raw list); fall back to guard if it's a list (not a string)
    guard = event_rec.get("guard_slots")
    if not guard:
        guard = event_rec.get("guard")
        if isinstance(guard, str):
            return None  # guard is a formatted string, not a list — can't compute HP
    if not guard:
        return None
    hp = compute_guard_hp(guard)
    return hp if hp > 0 else None


def monster_sum_hp(monster_rec: Dict[str, Any]) -> Optional[int]:
    """Compute ∑ HP for a monster tile (e.g. wandering monster stack).

    merge_monsters stores `monster_type` (raw int) and `count` (raw int) —
    both raw data, NOT calculations. HP = monster_hp(monster_type) * count.
    Matches PRT r_MonstrRow.HP logic.
    """
    if not isinstance(monster_rec, dict):
        return None
    mid = monster_rec.get("monster_type")
    cnt = monster_rec.get("count", 0)
    if not isinstance(mid, int) or mid < 0 or mid >= 0xFF:
        return None
    if not cnt or cnt <= 0:
        return None
    hp = get_monster_hp(mid) * cnt
    return hp if hp > 0 else None


# ============================================================================
# Future aggregators (stubs — to be implemented in next steps)
# ============================================================================
# GetAllArt (MainForm.cs): aggregate all artifacts from EventBox, Scholar,
#   SeerHut, Spell, Art, Monstr, Bank, Garrison, Heroes, Towns → "Все Арты" sheet.
# GetAllSpell (MainForm.cs): aggregate all spells → "Все Заклы" sheet.
# GetAllSkill (MainForm.cs): aggregate all skills → "Все Навыки" sheet.
# GetAllExperience (MainForm.cs): aggregate XP from all sources → "Опыт" sheet.
