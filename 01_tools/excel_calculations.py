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


def get_monster_name(monster_id: int) -> str:
    """Return PRT DEFAULT Russian name for the given monster_id (from TblMonsters).
    Used by _guard_str_safe for formatting guard strings in aggregator sheets.
    """
    entry = _load_tbl_monsters().get(str(monster_id))
    return entry.get("name", "") if entry else ""


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
# Aggregators — aggregate from merged_objects + heroes_found + towns_found
# to build the "Все Арты" / "Все Заклы" / "Все Навыки" / "Опыт" sheets.
# Called ONLY by excel_export.py (not by parsing modules or JSON/tree exporters).
# ============================================================================

# PRT DEFAULT TblObject names (MainForm.cs:10717+). Used for Object column
# in aggregator sheets to match PRT 0000.xlsx exactly.
_PRT_OBJECT = {
    26: "Событие",       # Event
    34: "Герой",         # Hero
    54: "Монстр",        # Monster
    81: "Ученый",        # Scholar
    83: "Хижина провидца",  # Seer Hut
    98: "Городок",       # Town
    100: "Камень знаний", # Learning Stone
    101: "Сундук с сокровищами",  # Treasure Chest
    113: "Хижина ведьмы",  # Witch Hut
}

def _guard_str_safe(guard) -> Optional[str]:
    """Format a guard list as 'Pikeman 5, Goblin 20' string.
    If guard is already a string, return it as-is.
    If guard is a list of {monster_id, count} dicts, format each.
    """
    if not guard:
        return None
    if isinstance(guard, str):
        return guard
    if isinstance(guard, list):
        parts = []
        for g in guard:
            if isinstance(g, dict):
                mid = g.get("monster_id", -1)
                cnt = g.get("count", 0)
                if isinstance(mid, int) and 0 <= mid < 0xFF and cnt:
                    mname = get_monster_name(mid) or f"M{mid}"
                    parts.append(f"{mname} {cnt}")
        return ", ".join(parts) if parts else None
    return None

def aggregate_all_artifacts(parsed: Dict[str, Any], localizer=None) -> List[Dict[str, Any]]:
    """Aggregate ALL artifacts from every source on the map → "Все Арты" sheet.

    Matches PRT R_AllArts table, populated during:
      1. Art Merchants (GetArtMerchants) — 7 rows, no X/Y/Z
      2. Black Market (GetMarketContent + SaveMarket) — 3 markets × 7 slots
      3. Ground artifacts (SaveArt/ArtResContent) — Tomb/Skeleton/Survivor/Ground
      4. Chests (SaveChest when artifact_id != 0xFF)
      5. Event/Pandora boxes (EventBoxContent artifacts list)
      6. Creature Banks (BankContent for artifact-banks, type 24/25/84/85)
      7. Seer Huts (SeerHutContent reward_type == artifact)
      8. Heroes (GetHeroesContent — doll slot artifacts, excluding book/machines)

    Each row has: x, y, z, locality, object, slot, artifact_name, class_, relic,
    hero, doll_slot, flag, place, monster, mission, guard.

    Args:
        parsed: the parsed dict (from parsed_save_to_dict or gm1_parser).
        localizer: Localizer instance for localized artifact names/classes.

    Returns:
        List of dicts, one per artifact row.
    """
    mo = parsed.get("merged_objects", {}) or {}
    rows: List[Dict[str, Any]] = []

    # Lazy-load art_metadata for name/class/relic lookups.
    try:
        from art_metadata import (
            get_artifact_name, get_artifact_class_id, get_artifact_relic_c_id,
        )
        _has_art_meta = True
    except Exception:
        _has_art_meta = False

    def _art_info(aid):
        """Return dict {name, class, relic} for the given artifact_id."""
        if not _has_art_meta or aid is None or aid < 0:
            return {}
        info = {}
        # Use localizer (ru_sod) for NAME — matches PRT's binary output exactly.
        # tbl_art_mapping.json (via get_artifact_name with localizer=None) has
        # ru_wog-style names which differ from PRT's actual output.
        if localizer:
            info["name"] = localizer.art(aid) or get_artifact_name(aid, localizer=None)
        else:
            info["name"] = get_artifact_name(aid, localizer=None)
        cls_id = get_artifact_class_id(aid)
        if localizer:
            info["class"] = localizer.art_class(cls_id)
        relic_id = get_artifact_relic_c_id(aid)
        if relic_id is not None:
            # Use localizer for relic name (matches PRT binary output)
            if localizer:
                info["relic"] = localizer.art(relic_id) or get_artifact_name(relic_id, localizer=None)
            else:
                info["relic"] = get_artifact_name(relic_id, localizer=None)
        return info

    # ── 1. Art Merchants (no X/Y/Z) ──
    for rec in mo.get("Рынки", []):
        if rec.get("object") == "Торговцы Артефактами":
            aid = rec.get("artifact_id", -1)
            if aid is not None and aid >= 0:
                info = _art_info(aid)
                rows.append({
                    "x": None, "y": None, "z": None,
                    "locality": None,
                    "object": rec.get("object"),
                    "slot": rec.get("slot"),
                    "artifact_name": info.get("name"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": None, "doll_slot": None, "flag": None,
                    "place": None, "monster": None, "mission": None, "guard": None,
                })

    # ── 2. Black Market (with X/Y/Z) ──
    for rec in mo.get("Рынки", []):
        if rec.get("object") == "Черный рынок":
            aid = rec.get("artifact_id", -1)
            if aid is not None and aid >= 0:
                info = _art_info(aid)
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object"),
                    "slot": rec.get("slot"),
                    "artifact_name": info.get("name"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": None, "doll_slot": None, "flag": None,
                    "place": None, "monster": None, "mission": None, "guard": None,
                })

    # ── 3. Ground artifacts (Tomb, Skeleton, Survivor, Ground Art) ──
    for rec in mo.get("Артефакты", []):
        aid = rec.get("artifact_id", -1)
        if aid is not None and aid >= 0:
            info = _art_info(aid)
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),
                # Preserve slot from rec — bank rows (e.g., Dragon Utopia)
                # have slot=1,2,3,4; ground artifacts have slot=None.
                # Previously hardcoded None which lost bank slot info.
                "slot": rec.get("slot"),
                "artifact_name": info.get("name") or rec.get("artifact_name"),
                "class_": info.get("class") or rec.get("class_"),
                "relic": info.get("relic") or rec.get("relic"),
                "hero": None, "doll_slot": None, "flag": None,
                "place": None, "monster": None, "mission": None,
                "guard": rec.get("guard"),
            })

    # ── 4. Chests with artifacts ──
    for rec in mo.get("Сундуки", []):
        aid = rec.get("artifact_id", -1)
        if aid is not None and aid >= 0 and aid < 0xFF:
            info = _art_info(aid)
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),
                "slot": None,
                "artifact_name": info.get("name") or rec.get("artifact"),
                "class_": info.get("class"),
                "relic": info.get("relic"),
                "hero": None, "doll_slot": None, "flag": None,
                "place": None, "monster": None, "mission": None, "guard": None,
            })

    # ── 4b. Monsters with artifacts ──
    # PRT MonstrContent (MainForm.cs ~9167): for guarded monsters with an
    # artifact_id (u8 byte), the monster is shown on the "Все Арты" sheet
    # with Object="Монстр", the artifact name in the Артефакт column, and the
    # monster name + count in the Монстр column. Each monster-with-artifact
    # emits ONE row.
    # Source: mo["Монстры"] — each monster record has `artifact_id` (raw int)
    # and `artifact` (localized name) fields. We use artifact_id to look up
    # class and relic-C via art_metadata.
    for rec in mo.get("Монстры", []):
        aid = rec.get("artifact_id", -1)
        if aid is None or aid < 0:
            continue  # monster has no artifact
        info = _art_info(aid)
        # Monster column: "<name> <count>" (PRT format, e.g. "Циклоп 2")
        mon_name = rec.get("monster", "")
        mon_count = rec.get("count", 0)
        mon_str = f"{mon_name} {mon_count}" if mon_count and mon_name else mon_name
        rows.append({
            "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
            "locality": rec.get("locality"),
            "object": _PRT_OBJECT.get(54, "Монстр"),  # PRT TblObject code 54 = "Монстр"
            "slot": None,
            "artifact_name": info.get("name") or rec.get("artifact"),
            "class_": info.get("class"),
            "relic": info.get("relic"),
            "hero": None, "doll_slot": None, "flag": None,
            "place": None, "monster": mon_str, "mission": None,
            "guard": None,
        })

    # ── 5. Event/Pandora box artifacts ──
    # PRT splits event artifacts into separate rows (one per artifact).
    # Each row gets the artifact's class and relic-C from art_metadata.
    for rec in mo.get("События_Ящики_Пандоры", []):
        art_str = rec.get("artifact")
        # Prefer artifact_ids list (more reliable for class/relic lookup)
        art_ids = rec.get("artifact_ids", [])
        if art_ids:
            # Iterate individual artifact IDs (one row per artifact)
            for aid in art_ids:
                if aid is None or aid < 0:
                    continue
                info = _art_info(aid)
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object"),
                    "slot": None,
                    "artifact_name": info.get("name"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": None, "doll_slot": None, "flag": None,
                    "place": None, "monster": None, "mission": None,
                    "guard": rec.get("guard"),
                })
        elif art_str:
            # Fallback: art_str is a formatted string like "Art1, Art2"
            # (older path — no per-artifact IDs available; emit single row).
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),
                "slot": None,
                "artifact_name": art_str,
                "class_": None,
                "relic": None,
                "hero": None, "doll_slot": None, "flag": None,
                "place": None, "monster": None, "mission": None,
                "guard": rec.get("guard"),
            })

    # ── 6. Creature Banks (artifact-banks only: type 24/25/84/85) ──
    for rec in mo.get("Банки", []):
        if rec.get("is_artifact_bank", False):
            aids = rec.get("artifact_ids", [])
            for slot_idx, aid in enumerate(aids):
                if aid is not None and aid >= 0 and aid < 0xFF:
                    info = _art_info(aid)
                    rows.append({
                        "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                        "locality": rec.get("locality"),
                        "object": rec.get("object"),
                        "slot": slot_idx + 1,
                        "artifact_name": info.get("name"),
                        "class_": info.get("class"),
                        "relic": info.get("relic"),
                        "hero": None, "doll_slot": None, "flag": None,
                        "place": None, "monster": None, "mission": None,
                        "guard": rec.get("guard"),
                    })

    # ── 7. Seer Huts with artifact reward OR artifact mission ──
    # PRT also fills the Задание (mission) column with the formatted mission string.
    # We format it the same way as _fmt_seer_mission in excel_export.py (using localizer).
    _A_RES_LETTERS = ["W", "M", "O", "S", "C", "G"]
    _A_RES_NAMES = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
    _PSKILL_LETTERS = {"Attack": "A", "Defense": "D", "Power": "P", "Knowledge": "K"}

    def _fmt_mission_str(rec):
        """Format seer hut mission string (same logic as _fmt_seer_mission)."""
        mtype = rec.get("mission_type", 0)
        mission = rec.get("mission", {}) or {}
        if not mtype:
            return None
        if mtype == 1:
            return f"{localizer.quest(0)} {mission.get('level', 0)}" if localizer else f"Достичь уровня {mission.get('level', 0)}"
        elif mtype == 2:
            ps = mission.get("primary_skills", {})
            if isinstance(ps, dict) and ps:
                parts = [f"{_PSKILL_LETTERS.get(k, k[:1].upper())}{v}" for k, v in ps.items()]
                return f"{localizer.quest(1)} {', '.join(parts)}" if localizer else f"Получить навык {', '.join(parts)}"
            return localizer.quest(1) if localizer else "Получить навык"
        elif mtype == 3:
            hero_id = mission.get("hero_id", -1)
            heroes_list = parsed.get("heroes_found", []) or []
            hero = heroes_list[hero_id] if 0 <= hero_id < len(heroes_list) else {}
            hero_name = hero.get("name", "") or f"H{hero_id}"
            hero_place = hero.get("place") or ""
            if hero_place and "." in str(hero_place):
                return f"{localizer.quest(7)} {hero_name} ({hero_place})" if localizer else f"Победить героя {hero_name} ({hero_place})"
            return f"{localizer.quest(7)} {hero_name}" if localizer else f"Победить героя {hero_name}"
        elif mtype == 4:
            mid = mission.get("monster_id", -1)
            mx = mission.get("x", 0)
            my = mission.get("y", 0)
            mz = mission.get("bit", 0)
            mname = localizer.monstr(mid) if localizer and mid >= 0 else f"M{mid}"
            return f"{localizer.quest(2)} {mname} ({mx}.{my}.{mz})" if localizer else f"Победить монстра {mname} ({mx}.{my}.{mz})"
        elif mtype == 5:
            arts = mission.get("artifacts", [])
            if isinstance(arts, list) and arts:
                art_names = [localizer.art(a) if localizer and isinstance(a, int) and a >= 0 else str(a) for a in arts]
                art_names = [n for n in art_names if n]
                if art_names:
                    return f"{localizer.quest(3)} {', '.join(art_names)}" if localizer else f"Принести артефакты {', '.join(art_names)}"
            return localizer.quest(3) if localizer else "Принести артефакты"
        elif mtype == 6:
            mons = mission.get("monsters", [])
            if isinstance(mons, list) and mons:
                mon_parts = []
                for m in mons:
                    if isinstance(m, (list, tuple)) and len(m) >= 2:
                        mid, cnt = m[0], m[1]
                        mname = localizer.monstr(mid) if localizer and isinstance(mid, int) and mid >= 0 else f"M{mid}"
                        mon_parts.append(f"{mname} {cnt}")
                if mon_parts:
                    return f"{localizer.quest(4)} {', '.join(mon_parts)}" if localizer else f"Принести монстров {', '.join(mon_parts)}"
            return localizer.quest(4) if localizer else "Принести монстров"
        elif mtype == 7:
            res = mission.get("resources", {})
            if isinstance(res, dict) and res:
                res_parts = []
                for rkey, rcount in res.items():
                    if rcount:
                        try:
                            ridx = _A_RES_NAMES.index(rkey)
                            res_parts.append(f"{_A_RES_LETTERS[ridx]}{rcount}")
                        except (ValueError, IndexError):
                            if rkey == "Gold":
                                res_parts.append(f"Золото {rcount}")
                            else:
                                res_parts.append(f"{rkey} {rcount}")
                if res_parts:
                    return f"{localizer.quest(5)} {', '.join(res_parts)}" if localizer else f"Принести ресурсы {', '.join(res_parts)}"
            return localizer.quest(5) if localizer else "Принести ресурсы"
        elif mtype == 8:
            hero_id = mission.get("hero_id", -1)
            heroes_list = parsed.get("heroes_found", []) or []
            hero = heroes_list[hero_id] if 0 <= hero_id < len(heroes_list) else {}
            hero_name = hero.get("name", "") or f"H{hero_id}"
            return f"{localizer.quest(8)} {hero_name}" if localizer else f"Быть героем {hero_name}"
        elif mtype == 9:
            color_id = mission.get("color_id")
            color_name = localizer.color(color_id) if localizer and color_id is not None else (mission.get("color") or "?")
            return f"{localizer.quest(6)} {color_name}" if localizer else f"Иметь цвет флага {color_name}"
        return None

    # Use localizer for seer hut object name (matches PRT binary exactly)
    _seer_hut_name = localizer.object(83) if localizer else "Хижина Провидца"

    for rec in mo.get("Провидцы", []):
        # Format mission string for Задание column
        mission_str = _fmt_mission_str(rec)
        # Check if reward is an artifact
        reward = rec.get("reward", {})
        if isinstance(reward, dict) and "artifact_id" in reward:
            aid = reward.get("artifact_id", -1)
            if aid is not None and aid >= 0:
                info = _art_info(aid)
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": _seer_hut_name,
                    "slot": None,
                    "artifact_name": info.get("name"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": None, "doll_slot": None, "flag": None,
                    "place": None, "monster": None, "mission": mission_str,
                    "guard": None,
                })
        # Check if mission is to bring artifacts (mission_type=5)
        mission = rec.get("mission", {})
        if isinstance(mission, dict) and "artifacts" in mission:
            for aid in mission.get("artifacts", []):
                if isinstance(aid, int) and aid >= 0:
                    info = _art_info(aid)
                    rows.append({
                        "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                        "locality": rec.get("locality"),
                        "object": _seer_hut_name,
                        "slot": None,
                        "artifact_name": info.get("name"),
                        "class_": info.get("class"),
                        "relic": info.get("relic"),
                        "hero": None, "doll_slot": None, "flag": None,
                        "place": None, "monster": None, "mission": mission_str,
                        "guard": None,
                    })

    # ── 8. Heroes (artifact string from doll slots) ──
    # PRT reads 83 doll slots; our parser produces a formatted `artifact` string.
    #
    # NOTE on PRT bug fixed here:
    #   PRT (MainForm.cs:7842-7868) only calls ArtDollPlace1 (for heroes with X.Y.Z
    #   loc byte != 255, "flag") OR ArtDollPlace2 (for non-banned, "else if !flag2").
    #   Banned heroes (flag2=true, Place="Запрещен") are completely SKIPPED — their
    #   artifacts never reach R_AllArts. We fix this: include ALL heroes regardless
    #   of Place.
    #
    # Местоположение (Place column) per hero state — matches PRT's intent WITHOUT
    # the missing-hero bug:
    #   - "Таверна"      → tavern hero (in player's tavern, hireable)
    #   - "Запрещен"     → banned hero (cannot be hired this scenario)
    #   - "Тюрьма"       → prison-on-map hero (decmp[num]!=255 but no player color)
    #   - None           → active map hero with color (PRT shows empty Place column)
    for h in parsed.get("heroes_found", []):
        art_str = h.get("artifact")
        if not art_str:
            continue
        hero_name = h.get("name", "?")
        flag = h.get("flag")
        place = h.get("place")
        tavern_owner = h.get("tavern_owner")

        # Determine Местоположение column + (X,Y,Z) coords per hero state
        hx, hy, hz = None, None, None
        place_col = None
        if place == "Таверна":
            place_col = "Таверна"
        elif place == "Запрещен":
            place_col = "Запрещен"
        elif isinstance(place, str) and "." in place:
            parts = place.split(".")
            if len(parts) >= 3:
                try:
                    hx, hy, hz = int(parts[0]), int(parts[1]), int(parts[2])
                except ValueError:
                    pass
            # Hero is on the map (loc byte != 0xFF). Three cases:
            #   - Has flag/color → active hero, no Place label (PRT behavior)
            #   - No flag/color + in a tavern → "escaped" hero: record keeps
            #     map (prison) coords, but the hero waits in a tavern → "Таверна"
            #   - No flag/color, not in tavern → prison-on-map hero → "Тюрьма"
            if flag:
                place_col = None
            elif tavern_owner is not None and tavern_owner >= 0:
                place_col = "Таверна"
            else:
                place_col = "Тюрьма"
        # else: place is None — regular hireable-but-not-on-map hero; leave None

        art_names = [a.strip() for a in art_str.split(",") if a.strip()]
        # Prefer doll_artifacts list (has artifact_id for class/relic lookup)
        doll_arts = h.get("doll_artifacts", [])
        if doll_arts:
            # Iterate (slot_index, artifact_id) tuples.
            # PRT GetDollPlace uses `pl` (slot index) DIRECTLY with aDoll[pl]
            # (line 7963): `nrow.Doll = aDoll[pl]` (no +1 offset).
            # OUR's loc.doll(idx) is also 0-based — so doll_slot = slot_idx
            # (NOT slot_idx + 1, which was the previous bug causing Кукла to
            # show "Разное" instead of "Ноги" for slot_idx=8).
            for slot_idx, art_id in doll_arts:
                info = _art_info(art_id)
                rows.append({
                    "x": hx, "y": hy, "z": hz,
                    "locality": "суша",
                    "object": _PRT_OBJECT.get(34, "Герой"),  # PRT "Герой"
                    "slot": None,  # PRT shows None for hero artifact Слот
                    "artifact_name": info.get("name") or (localizer.art(art_id) if localizer else f"A{art_id}"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": hero_name,
                    # PRT GetDollPlace: aDoll[pl] — direct index, no +1 offset
                    "doll_slot": slot_idx,
                    "flag": flag,
                    "place": place_col,
                    "monster": None, "mission": None, "guard": None,
                })
        else:
            # Fallback: parse art_str (no class/relic available)
            for slot_idx, art_name in enumerate(art_names):
                rows.append({
                    "x": hx, "y": hy, "z": hz,
                    "locality": "суша",
                    "object": _PRT_OBJECT.get(34, "Герой"),  # PRT "Герой"
                    "slot": None,  # PRT shows None for hero artifact Слот
                    "artifact_name": art_name,
                    "class_": None,
                    "relic": None,
                    "hero": hero_name,
                    "doll_slot": slot_idx,  # PRT shows doll slot NAME
                    "flag": flag,
                    "place": place_col,
                    "monster": None, "mission": None, "guard": None,
                })

    return rows


def aggregate_all_spells(parsed: Dict[str, Any], localizer=None, raw: Optional[bytes] = None) -> List[Dict[str, Any]]:
    """Aggregate ALL spells from every source → "Все Заклы" sheet.

    Sources (matching PRT GetAllSpell, MainForm.cs:7263):
      1. Event/Pandora boxes (EventBoxContent spells list)
      2. Shrines (ShrineContent)
      3. Heroes (spells_book)
      4. Towns (spell pool per level)
      5. Seer Huts (if spell reward)
      6. Scholars (if spell reward)

    Returns list of dicts with: x, y, z, object, spell_name, level, flag,
    town, built, garrison, hero, place, mission, guard, num.
    """
    mo = parsed.get("merged_objects", {}) or {}
    rows: List[Dict[str, Any]] = []

    # Build spell reverse-lookup: lowercased name → spell_id
    _spell_rev = {}
    if localizer:
        try:
            import json as _json
            import os as _os
            _here = _os.path.dirname(_os.path.abspath(__file__))
            _docs = _os.path.normpath(_os.path.join(_here, "..", "02_format_docs"))
            _path = _os.path.join(_docs, "localization_dictionary.json")
            with open(_path, "r", encoding="utf-8") as _f:
                _loc = _json.load(_f)
            for _sid_str, _sname in _loc.get("sections", {}).get("Spell", {}).get("languages", {}).get("ru_sod", {}).items():
                if _sname:
                    _spell_rev[_sname.lower()] = int(_sid_str)
        except Exception:
            pass

    def _spl_name(sid):
        if localizer and isinstance(sid, int) and sid >= 0:
            return localizer.spell(sid)
        return None

    def _find_spell_id(name):
        """Reverse-lookup spell name → ID (case-insensitive)."""
        if name and isinstance(name, str):
            return _spell_rev.get(name.lower())
        return None

    # ── 1. Event/Pandora box spells ──
    for rec in mo.get("События_Ящики_Пандоры", []):
        spell_str = rec.get("spell")
        if spell_str:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "object": rec.get("object"),
                "spell_name": spell_str,
                "level": None, "flag": None, "town": None, "built": None,
                "garrison": None, "hero": None, "place": None,
                "mission": None, "guard": rec.get("guard"),
            })

    # ── 2. Shrine spells — use rec's localized object name ──
    for rec in mo.get("Заклинания_Святилища", []):
        sid = rec.get("spell_id")
        if sid is not None and sid >= 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "object": rec.get("object") or localizer.object(81) if localizer else "Святыня",
                "spell_name": _spl_name(sid) or str(sid),
                "level": rec.get("spell_level"),
                "flag": None, "town": None, "built": None,
                "garrison": None, "hero": None, "place": None,
                "mission": None, "guard": None, "num": None,
            })

    # ── 3. Hero spells — include ALL heroes, split spells per row, label Place ──
    #
    # NOTE on PRT bugs fixed here:
    #   1. PRT filter (MainForm.cs:7273) `Place <> 'Запрещен' OR Place IS NULL`
    #      EXCLUDES banned heroes — their spells never reach R_AllSpell. We
    #      include ALL heroes regardless of Place.
    #   2. PRT GetAllSpell line 7418: for heroes with X.Y.Z Place and no Color
    #      (prison-on-map heroes), PRT writes `Place = "Тюрьма"` for ALL such
    #      heroes, mislabeling any "escaped" hero placed on map AND listed in a
    #      tavern — PRT never re-checks tavern membership once loc byte != 0xFF.
    #      We fix this via the hero's `tavern_owner` field (mirrors the
    #      IsHeroTavern check added in MainForm_fixed1.cs FIX1-C).
    #   3. PRT splits spell string by ", " → one row per spell (line 7277, 7381,
    #      7401). We do the same.
    for hero_idx, h in enumerate(parsed.get("heroes_found", [])):
        spell_str = h.get("spell")
        if not spell_str:
            continue
        hero_name = h.get("name", "?")
        flag = h.get("flag")
        place = h.get("place")
        tavern_owner = h.get("tavern_owner")

        # Determine Местоположение column + (X,Y,Z) coords per hero state
        hx, hy, hz = None, None, None
        place_col = None
        if place == "Таверна":
            place_col = "Таверна"
        elif place == "Запрещен":
            place_col = "Запрещен"
        elif isinstance(place, str) and "." in place:
            parts = place.split(".")
            if len(parts) >= 3:
                try:
                    hx, hy, hz = int(parts[0]), int(parts[1]), int(parts[2])
                except ValueError:
                    pass
            if flag:
                place_col = None
            elif tavern_owner is not None and tavern_owner >= 0:
                place_col = "Таверна"
            else:
                place_col = "Тюрьма"

        # Split spells by ", " → one row per spell (matches PRT GetAllSpell)
        spell_names = [s.strip() for s in spell_str.split(",") if s.strip()]
        for spell_name in spell_names:
            # Look up spell level via localizer (PRT GetAllSpell reads it from TblSpell)
            spell_level = None
            if localizer:
                # Try to find spell_id by reverse-lookup (spell_name → id)
                # PRT stores spells as IDs in hero data, then looks up name + level
                # We have the name (from hero's spell string) — need level
                # Use localizer.spell_level() if we can find the ID
                pass  # spell_level lookup is complex; PRT gets it from TblSpell directly
            rows.append({
                "x": hx, "y": hy, "z": hz,
                "object": _PRT_OBJECT.get(34, "Герой"),
                "spell_name": spell_name,
                "level": localizer.spell_level(_find_spell_id(spell_name)) if localizer else None,
                "flag": flag, "town": None, "built": None,
                "garrison": None, "hero": hero_name,
                "place": place_col,
                "mission": None, "guard": None, "num": hero_idx,
            })

    # ── 4. Town spells (from spell pool per level) ──
    # PRT GetAllSpell for towns: reads spell pool + garrison + built flag.
    # Garrison: formatted as "<name> <count>, ..." from army_types/counts.
    # Built: "Да" if mage guild level is built (bit in built_byte at sp_addr-16).
    for t in parsed.get("towns_found", []):
        f = t.get("fields", {}) or {}
        sp = f.get("spell_pool", {})
        levels = sp.get("levels", []) if isinstance(sp, dict) else []
        town_name = f.get("name", "?")
        ttype = f.get("type", 0) or 0
        faction = f.get("faction", -1)
        flag = localizer.color(faction) if localizer and isinstance(faction, int) and 0 <= faction < 8 else None

        # Garrison: format from army_types + army_counts
        at = f.get("army_types", []) or []
        ac = f.get("army_counts", []) or []
        garrison_parts = []
        for mt, ct in zip(at, ac):
            if isinstance(mt, int) and mt >= 0 and ct:
                mname = localizer.monstr(mt) if localizer else f"M{mt}"
                garrison_parts.append(f"{mname} {ct}")
        garrison_str = ", ".join(garrison_parts) if garrison_parts else None

        # Built: check built_byte (similar to Города sheet logic)
        sp_addr = sp.get("address", 0) if isinstance(sp, dict) else 0
        built_byte = 0
        if raw and sp_addr and sp_addr - 16 >= 0 and sp_addr - 16 < len(raw):
            built_byte = raw[sp_addr - 16]

        for level_idx, level_spells in enumerate(levels):
            if isinstance(level_spells, list):
                for sid in level_spells:
                    if sid is not None and sid >= 0:
                        built_str = "Да" if (built_byte >> level_idx) & 1 else None
                        rows.append({
                            "x": f.get("x"), "y": f.get("y"), "z": f.get("z"),
                            "object": _PRT_OBJECT.get(98, "Городок"),
                            "spell_name": _spl_name(sid) or str(sid),
                            "level": level_idx + 1,
                            "flag": flag, "town": town_name, "built": built_str,
                            "garrison": garrison_str if level_idx == 0 else None,
                            "hero": None, "place": None,
                            "mission": None, "guard": None, "num": None,
                        })

    # ── 5. Scholar spells (scholar_type='spell', has spell_id) ──
    for rec in mo.get("Ученые", []):
        sid = rec.get("spell_id")
        if sid is not None and sid >= 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "object": _PRT_OBJECT.get(81, "Ученый"),
                "spell_name": _spl_name(sid) or str(sid),
                "level": None, "flag": None, "town": None, "built": None,
                "garrison": None, "hero": None, "place": None,
                "mission": None, "guard": None,
            })

    # ── 6. Seer Huts with spell reward ──
    for rec in mo.get("Провидцы", []):
        reward = rec.get("reward", {})
        if isinstance(reward, dict) and "spell_id" in reward:
            sid = reward.get("spell_id", -1)
            if sid is not None and sid >= 0:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "object": _PRT_OBJECT.get(81, "Ученый"),
                    "spell_name": _spl_name(sid) or str(sid),
                    "level": None, "flag": None, "town": None, "built": None,
                    "garrison": None, "hero": None, "place": None,
                    "mission": rec.get("quest"), "guard": None,
                })

    return rows


def aggregate_all_skills(parsed: Dict[str, Any], localizer=None) -> List[Dict[str, Any]]:
    """Aggregate ALL skills from every source → "Все Навыки" sheet.

    Sources (matching PRT GetAllSkill, MainForm.cs:7160):
      1. Witch Huts (WitchHutContent)
      2. Event/Pandora boxes (EventBoxContent secondary_skills list)
      3. Scholars (ScholarContent)
      4. Seer Huts (if skill reward)

    Returns list of dicts with: x, y, z, locality, object, level_name,
    skill_name, mission, guard.
    """
    mo = parsed.get("merged_objects", {}) or {}
    rows: List[Dict[str, Any]] = []

    def _skl_name(sid):
        if localizer and isinstance(sid, int) and sid >= 0:
            return localizer.skill(sid)
        return None

    # ── 1. Witch Huts + Universities — stored in mo["Ведьмины_Хижины"] ──
    # Each record has a `skill` field which is a comma-separated skill string.
    # Witch huts have 1 skill; universities have 4 skills (e.g. "Вода, Чародейство, ...").
    # PRT GetAllSkill splits each R_Skill.Skill by ", " — one row per skill.
    for rec in mo.get("Ведьмины_Хижины", []):
        skill_str = rec.get("skill")
        if skill_str:
            skills = [s.strip() for s in skill_str.split(",") if s.strip()]
            for skill_name in skills:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object"),
                    "level_name": None,
                    "skill_name": skill_name,
                    "mission": None, "guard": None,
                })

    # ── 2. Event/Pandora box skills — parse level prefix (Б./П./Е.) ──
    for rec in mo.get("События_Ящики_Пандоры", []):
        skill_str = rec.get("secondary_skill")
        if skill_str:
            # Parse level prefix:
            #   ru_sod/ru_wog: "Б."/"П."/"Э." → "Базовый"/"Продвинутый"/"Экспертный"
            #   en: "B."/"A."/"E." → fallback to ru_sod full names
            #   ru_complete: "1."/"2."/"3." → fallback to ru_sod full names
            # All map to the same 3 levels (Basic/Advanced/Expert).
            level_map = {
                "Б.": "Базовый", "П.": "Продвинутый", "Е.": "Экспертный",
                "B.": "Базовый", "A.": "Продвинутый", "E.": "Экспертный",
                "1.": "Базовый", "2.": "Продвинутый", "3.": "Экспертный",
            }
            level_name = None
            parsed_skill = skill_str
            for prefix, lname in level_map.items():
                if skill_str.startswith(prefix):
                    level_name = lname
                    parsed_skill = skill_str[len(prefix):]
                    break
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),
                "level_name": level_name,
                "skill_name": parsed_skill,
                "mission": None, "guard": rec.get("guard"),
            })

    # ── 3. Scholars — use skill_id (only scholar_type='skill') ──
    for rec in mo.get("Ученые", []):
        sid = rec.get("skill_id")
        if sid is not None and sid >= 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": _PRT_OBJECT.get(81, "Ученый"),
                "level_name": None,
                "skill_name": _skl_name(sid) or str(sid),
                "mission": None, "guard": None,
            })

    # (Universities are NOT included separately — they're already in
    # mo["Ведьмины_Хижины"] with 4-skill comma-separated string, matching
    # PRT's R_Skill table which is populated by both SaveWitchHut + SaveUniver.)

    # ── 4. Seer Huts with skill reward ──
    # PRT SeerHutContent reward_type=7 (Secondary skill) populates R_Skill table
    # with one row per hut: level prefix + skill name. Reward dict has keys
    # `secondary_skill_id` and `level_id` (1-based: 1=Basic, 2=Advanced, 3=Expert).
    # Also include the mission string (formatted like PRT "Принести ресурсы M5, ...").
    # PRT DEFAULT arrays: aQuest[5]="Принести ресурсы", aResource letters W/M/O/S/C/G.
    _A_RESOURCE_LETTERS = ["W", "M", "O", "S", "C", "G"]
    _A_RESOURCE_NAMES = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
    _QUEST_NAME_FOR_MT = {
        # mission_type → PRT aQuest index (mission_type 1-based, aQuest 0-based)
        1: 0,  # Achieve level
        2: 1,  # Achieve skill (PRT only — Kill hero etc)
        3: 2,  # Defeat monster
        4: 3,  # Return with artifacts
        5: 4,  # Return with creatures
        6: 5,  # Return with resources
        7: 5,  # Bring resources — PRT also maps to "Принести ресурсы"
        8: 6,  # Belong to player
        9: 7,  # Defeat Hero
    }

    def _fmt_seer_mission(rec):
        """Format seer hut mission string (matches PRT R_AllSkill.Mission column).

        PRT format examples:
          "Принести ресурсы M5, Золото 9"  (resource letter + amount, comma-separated)
          "Принести артефакты ..."
          "Принести существ ..."
        """
        mtype = rec.get("mission_type", 0)
        mission = rec.get("mission", {}) or {}
        if not mtype:
            return None
        quest_idx = _QUEST_NAME_FOR_MT.get(mtype, mtype - 1)
        # Get Russian quest name from localizer (or fallback)
        quest_name = None
        if localizer:
            quest_name = localizer.quest(quest_idx)
        if not quest_name:
            return None
        parts = [quest_name]
        # Add specifics based on mission content
        if "level" in mission:
            parts.append(str(mission['level']))
        if "artifacts" in mission:
            arts = mission['artifacts']
            if isinstance(arts, list):
                art_names = []
                for a in arts:
                    if isinstance(a, int) and a >= 0:
                        if localizer:
                            n = localizer.art(a)
                            if n:
                                art_names.append(n)
                        if not (localizer and localizer.art(a)):
                            art_names.append(str(a))
                if art_names:
                    parts.append(", ".join(art_names))
        if "resources" in mission:
            res = mission['resources']
            if isinstance(res, dict):
                res_parts = []
                for rkey, rcount in res.items():
                    if rcount:
                        # Find resource letter
                        try:
                            ridx = _A_RESOURCE_NAMES.index(rkey)
                            letter = _A_RESOURCE_LETTERS[ridx]
                            res_parts.append(f"{letter}{rcount}")
                        except (ValueError, IndexError):
                            # Gold special case (idx 6, not in our 6-letter array)
                            if rkey == "Gold":
                                res_parts.append(f"Золото {rcount}")
                            else:
                                res_parts.append(f"{rkey} {rcount}")
                if res_parts:
                    parts.append(", ".join(res_parts))
        if "monsters" in mission:
            mons = mission['monsters']
            if isinstance(mons, list):
                mon_parts = []
                for m in mons:
                    if isinstance(m, dict):
                        mid = m.get('monster_id', -1)
                        cnt = m.get('count', 0)
                        if mid >= 0 and localizer:
                            mname = localizer.monstr(mid)
                            if mname:
                                mon_parts.append(f"{mname} {cnt}")
                            else:
                                mon_parts.append(f"M{mid} {cnt}")
                if mon_parts:
                    parts.append(", ".join(mon_parts))
        return " ".join(parts)

    for rec in mo.get("Провидцы", []):
        reward = rec.get("reward", {})
        if not isinstance(reward, dict):
            continue
        # Check both possible key names: skill_id (older format) OR secondary_skill_id
        sid = reward.get("secondary_skill_id")
        if sid is None:
            sid = reward.get("skill_id", -1)
        if sid is not None and isinstance(sid, int) and sid >= 0:
            lvl = reward.get("level_id", 0)
            # level_id is 1-based (1=Basic, 2=Advanced, 3=Expert) per PRT SeerHut
            # Convert to 0-based index for localizer.full_level_skill
            level_name = None
            if isinstance(lvl, int) and lvl >= 1 and localizer:
                level_name = localizer.full_level_skill(lvl - 1)
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),  # localized "Хижина Провидца"
                "level_name": level_name,
                "skill_name": _skl_name(sid) or str(sid),
                "mission": _fmt_seer_mission(rec),
                "guard": None,
            })

    return rows


def aggregate_all_experience(parsed: Dict[str, Any], localizer=None) -> List[Dict[str, Any]]:
    """Aggregate ALL XP sources → "Опыт" sheet.

    Sources (matching PRT GetAllExperience, MainForm.cs:6400+):
      1. Learning Stones (1000 XP each)
      2. Chests (gold or XP)
      3. Events (experience field)
      4. Artifacts (guard HP = XP)
      5. Resources (guard HP = XP)
      6. Banks (guard HP = XP)
      7. Monsters (count × HP = XP)
      8. Spells (guard HP = XP)
      9. Seer Huts (if XP reward)
      10. Heroes (500 + army HP)
      11. Towns (500 + garrison HP)

    Returns list of dicts with: x, y, z, locality, object, town, hero, flag,
    monster, artifact, resource, guard, hp, experience, xp_total.
    """
    mo = parsed.get("merged_objects", {}) or {}
    rows: List[Dict[str, Any]] = []

    # ── 1. Learning Stones ──
    for rec in mo.get("Learning_Stones", []):
        xp = rec.get("experience", 1000)
        rows.append({
            "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
            "locality": rec.get("locality"),
            "object": _PRT_OBJECT.get(100, "Камень знаний"),
            "town": None, "hero": None, "flag": None,
            "monster": None, "artifact": None, "resource": None,
            "guard": None, "hp": None,
            "experience": xp, "xp_total": xp,
        })

    # ── 2. Chests ──
    for rec in mo.get("Сундуки", []):
        xp = rec.get("experience")
        if xp and xp > 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": rec.get("object"),
                "town": None, "hero": None, "flag": None,
                "monster": None, "artifact": rec.get("artifact"),
                "resource": None, "guard": None, "hp": None,
                "experience": xp, "xp_total": xp,
            })

    # ── 3. Events (experience field) ──
    for rec in mo.get("События_Ящики_Пандоры", []):
        xp = rec.get("experience")
        if xp and xp > 0:
            hp_val = event_box_sum_hp(rec) if event_box_sum_hp else None
            total_xp = (hp_val or 0) + xp
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": _PRT_OBJECT.get(83, "Хижина провидца"),
                "town": None, "hero": None, "flag": None,
                "monster": None, "artifact": None, "resource": None,
                "guard": rec.get("guard"), "hp": hp_val,
                "experience": xp, "xp_total": total_xp,
            })

    # ── 4-8. Guarded tiles (Artifacts, Resources, Banks, Spells) ──
    # For each, HP = guard HP = XP
    for sheet_key, obj_name in [
        ("Артефакты", "Артефакт"),
        ("Ресурсы", "Ресурс"),
        ("Банки", "Банк"),
        ("Заклинания_Святилища", "Святыня"),
    ]:
        for rec in mo.get(sheet_key, []):
            hp_func = {
                "Артефакты": artifact_sum_hp,
                "Ресурсы": resource_sum_hp,
                "Банки": bank_sum_hp,
                "Заклинания_Святилища": None,
            }.get(sheet_key)
            hp_val = hp_func(rec) if hp_func else None
            if hp_val and hp_val > 0:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object", obj_name),
                    "town": None, "hero": None, "flag": None,
                    "monster": None, "artifact": rec.get("artifact"),
                    "resource": rec.get("resource"),
                    "guard": _guard_str_safe(rec.get("guard")),
                    "hp": hp_val,
                    "experience": None, "xp_total": hp_val,
                })

    # ── 9. Monsters ──
    for rec in mo.get("Монстры", []):
        hp_val = monster_sum_hp(rec) if monster_sum_hp else None
        if hp_val and hp_val > 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "locality": rec.get("locality"),
                "object": _PRT_OBJECT.get(54, "Монстр"),
                "town": None, "hero": None, "flag": None,
                "monster": rec.get("monster"), "artifact": None,
                "resource": None, "guard": None, "hp": hp_val,
                "experience": None, "xp_total": hp_val,
            })

    # ── 10. Garrisons (guard HP = XP) ──
    for rec in mo.get("Гарнизоны", []):
        guard = rec.get("guard")
        if guard:
            hp_val = compute_guard_hp(guard)
            if hp_val and hp_val > 0:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object", "Гарнизон"),
                    "town": None, "hero": None, "flag": rec.get("flag"),
                    "monster": None, "artifact": None,
                    "resource": None,
                    "guard": _guard_str_safe(guard),
                    "hp": hp_val,
                    "experience": None, "xp_total": hp_val,
                })

    # ── 11. SeerHut XP rewards ──
    for rec in mo.get("Провидцы", []):
        reward = rec.get("reward", {})
        if isinstance(reward, dict) and "experience" in reward:
            xp = reward.get("experience", 0)
            if xp and xp > 0:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": _PRT_OBJECT.get(83, "Хижина провидца"),  # PRT name for SeerHut
                    "town": None, "hero": None, "flag": None,
                    "monster": None, "artifact": None, "resource": None,
                    "guard": None, "hp": None,
                    "experience": xp, "xp_total": xp,
                })

    # ── 12. Heroes ──
    for h in parsed.get("heroes_found", []):
        place = h.get("place")
        if not place or place == "Запрещен":
            continue
        hp_val = hero_sum_hp(h) if hero_sum_hp else None
        if hp_val and hp_val > 0:
            xp = hp_val + 500
            # Parse X/Y/Z from place string
            hx, hy, hz = None, None, None
            if isinstance(place, str) and "." in place:
                parts = place.split(".")
                if len(parts) >= 3:
                    try:
                        hx, hy, hz = int(parts[0]), int(parts[1]), int(parts[2])
                    except ValueError:
                        pass
            rows.append({
                "x": hx, "y": hy, "z": hz,
                "locality": "суша",
                "object": _PRT_OBJECT.get(34, "Герой"),  # PRT "Герой"
                "town": None, "hero": h.get("name"), "flag": h.get("flag"),
                "monster": h.get("monster"), "artifact": None,
                "resource": None, "guard": None, "hp": hp_val,
                "experience": 500, "xp_total": xp,
            })

    # ── 11. Towns ──
    for t in parsed.get("towns_found", []):
        f = t.get("fields", {}) or {}
        hp_val = town_sum_hp(t) if town_sum_hp else None
        if hp_val and hp_val > 0:
            xp = hp_val + 500
            rows.append({
                "x": f.get("x"), "y": f.get("y"), "z": f.get("z"),
                "locality": "суша",
                "object": _PRT_OBJECT.get(98, "Городок"),  # PRT "Городок" for Town
                "town": f.get("name"), "hero": None,
                "flag": None,
                "monster": None, "artifact": None, "resource": None,
                "guard": None, "hp": hp_val,
                "experience": 500, "xp_total": xp,
            })

    return rows
