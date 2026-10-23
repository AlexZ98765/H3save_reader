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
        info["name"] = get_artifact_name(aid, localizer=None)  # PRT-style name
        cls_id = get_artifact_class_id(aid)
        if localizer:
            info["class"] = localizer.art_class(cls_id)
        relic_id = get_artifact_relic_c_id(aid)
        if relic_id is not None:
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
                "slot": None,
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

    # ── 5. Event/Pandora box artifacts ──
    for rec in mo.get("События_Ящики_Пандоры", []):
        art_str = rec.get("artifact")
        if art_str:
            # art_str is a formatted string like "Shield of Dwarven Lords, Sword of Justice"
            # For now, include as a single row with the formatted string.
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
    for rec in mo.get("Провидцы", []):
        # Check if reward is an artifact
        reward = rec.get("reward", {})
        if isinstance(reward, dict) and "artifact_id" in reward:
            aid = reward.get("artifact_id", -1)
            if aid is not None and aid >= 0:
                info = _art_info(aid)
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": _PRT_OBJECT.get(83, "Хижина провидца"),
                    "slot": None,
                    "artifact_name": info.get("name"),
                    "class_": info.get("class"),
                    "relic": info.get("relic"),
                    "hero": None, "doll_slot": None, "flag": None,
                    "place": None, "monster": None, "mission": rec.get("quest"),
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
                        "object": _PRT_OBJECT.get(83, "Хижина провидца"),
                        "slot": None,
                        "artifact_name": info.get("name"),
                        "class_": info.get("class"),
                        "relic": info.get("relic"),
                        "hero": None, "doll_slot": None, "flag": None,
                        "place": None, "monster": None, "mission": rec.get("quest"),
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
                "doll_slot": slot_idx + 1,  # PRT shows doll slot NAME (e.g. "Разное1")
                "flag": flag,
                "place": place_col,
                "monster": None, "mission": None, "guard": None,
            })

    return rows


def aggregate_all_spells(parsed: Dict[str, Any], localizer=None) -> List[Dict[str, Any]]:
    """Aggregate ALL spells from every source → "Все Заклы" sheet.

    Sources (matching PRT GetAllSpell, MainForm.cs:7263):
      1. Event/Pandora boxes (EventBoxContent spells list)
      2. Shrines (ShrineContent)
      3. Heroes (spells_book)
      4. Towns (spell pool per level)
      5. Seer Huts (if spell reward)
      6. Scholars (if spell reward)

    Returns list of dicts with: x, y, z, object, spell_name, level, flag,
    town, built, garrison, hero, place, mission, guard.
    """
    mo = parsed.get("merged_objects", {}) or {}
    rows: List[Dict[str, Any]] = []

    def _spl_name(sid):
        if localizer and isinstance(sid, int) and sid >= 0:
            return localizer.spell(sid)
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

    # ── 2. Shrine spells — shrines store spell_id + spell_level, NOT spell string ──
    for rec in mo.get("Заклинания_Святилища", []):
        sid = rec.get("spell_id")
        if sid is not None and sid >= 0:
            rows.append({
                "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                "object": _PRT_OBJECT.get(81, "Ученый"),
                "spell_name": _spl_name(sid) or str(sid),
                "level": rec.get("spell_level"),
                "flag": None, "town": None, "built": None,
                "garrison": None, "hero": None, "place": None,
                "mission": None, "guard": None,
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
    for h in parsed.get("heroes_found", []):
        spell_str = h.get("spell")
        if not spell_str:
            continue
        hero_name = h.get("name", "?")
        flag = h.get("flag")
        place = h.get("place")
        tavern_owner = h.get("tavern_owner")

        # Determine Местоположение column + (X,Y,Z) coords per hero state
        # (same logic as aggregate_all_artifacts — see notes there)
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
            # Same three cases as in aggregate_all_artifacts: active hero (has
            # color) → None; colorless but in a tavern (escaped) → "Таверна";
            # colorless prison-on-map → "Тюрьма"
            if flag:
                place_col = None
            elif tavern_owner is not None and tavern_owner >= 0:
                place_col = "Таверна"
            else:
                place_col = "Тюрьма"

        # Split spells by ", " → one row per spell (matches PRT GetAllSpell)
        spell_names = [s.strip() for s in spell_str.split(",") if s.strip()]
        for spell_name in spell_names:
            rows.append({
                "x": hx, "y": hy, "z": hz,
                "object": _PRT_OBJECT.get(34, "Герой"),
                "spell_name": spell_name,
                "level": None, "flag": flag, "town": None, "built": None,
                "garrison": None, "hero": hero_name,
                "place": place_col,
                "mission": None, "guard": None,
            })

    # ── 4. Town spells (from spell pool per level) ──
    for t in parsed.get("towns_found", []):
        f = t.get("fields", {}) or {}
        sp = f.get("spell_pool", {})
        levels = sp.get("levels", []) if isinstance(sp, dict) else []
        town_name = f.get("name", "?")
        ttype = f.get("type", 0) or 0
        town_type_name = localizer.town(ttype) if localizer and isinstance(ttype, int) else str(ttype)
        faction = f.get("faction", -1)
        flag = localizer.color(faction) if localizer and isinstance(faction, int) and 0 <= faction < 8 else None
        for level_idx, level_spells in enumerate(levels):
            if isinstance(level_spells, list):
                for sid in level_spells:
                    if sid is not None and sid >= 0:
                        rows.append({
                            "x": f.get("x"), "y": f.get("y"), "z": f.get("z"),
                            "object": _PRT_OBJECT.get(98, "Городок"),  # PRT uses "Городок" for all town spells
                            "spell_name": _spl_name(sid) or str(sid),
                            "level": level_idx + 1,
                            "flag": flag, "town": town_name, "built": None,
                            "garrison": None, "hero": None, "place": None,
                            "mission": None, "guard": None,
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
            # Parse level prefix: "Б.Лидерство" → level="Базовый", skill="Лидерство"
            level_map = {"Б.": "Базовый", "П.": "Продвинутый", "Е.": "Экспертный"}
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
    for rec in mo.get("Провидцы", []):
        reward = rec.get("reward", {})
        if isinstance(reward, dict) and "skill_id" in reward:
            sid = reward.get("skill_id", -1)
            if sid is not None and sid >= 0:
                rows.append({
                    "x": rec.get("x"), "y": rec.get("y"), "z": rec.get("z"),
                    "locality": rec.get("locality"),
                    "object": rec.get("object"),
                    "level_name": None,
                    "skill_name": _skl_name(sid) or str(sid),
                    "mission": rec.get("quest"), "guard": None,
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
