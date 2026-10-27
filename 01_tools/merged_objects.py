#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
merged_objects.py — Merge tile-scan results with post-tile content.

This module produces standardized "raw" tables (Артефакты, Монстры, Банки, etc.)
by combining:
  - tile_scanner.scan_tiles() — gives coords + type for each object
  - post_tile_parser.parse_all_post_tile_sections() — gives content (guard, resources, artifacts)

The merge logic:
  1. Build coord_int → tile_object map (from scan_tiles)
  2. For each post-tile section (EventBox, ArtRes, Monstr, Bank, Garrison, SeerHut,
     PassGuard, Univer, Market, Prison), find the corresponding tile object by
     matching coordinates
  3. Merge into a single record with both type and content

The output is a list of dicts — one per object on the map — ready for JSON export.
"""
from __future__ import annotations
from collections import defaultdict
from typing import Any, Dict, List, Optional, Tuple


# ============================================================================
# Build coord_int → tile_object map
# ============================================================================

def _build_tile_object_map(tile_objects: List[Dict[str, Any]]
                            ) -> Dict[int, List[Dict[str, Any]]]:
    """Build a dict coord_int → list of tile_objects at that coord.

    Multiple objects can be on the same coord (overlays).
    """
    m: Dict[int, List[Dict[str, Any]]] = defaultdict(list)
    for obj in tile_objects:
        ci = obj.get('coord_int') if 'coord_int' in obj else None
        if ci is None:
            x, y, z = obj.get('x', 0), obj.get('y', 0), obj.get('z', 0)
            ci = x | (y << 8) | (z << 16)
        m[ci].append(obj)
    return m


def _find_tile_at(tile_map: Dict[int, List[Dict[str, Any]]],
                  x: int, y: int, z: int,
                  type_filter: Optional[str] = None) -> Optional[Dict[str, Any]]:
    """Find a tile_object at (x,y,z), optionally filtering by object_kind or type_name."""
    ci = x | (y << 8) | (z << 16)
    candidates = tile_map.get(ci, [])
    if not candidates:
        return None
    if type_filter is None:
        return candidates[0]
    # Try to find one matching the filter
    for c in candidates:
        kind = c.get('object_kind') or c.get('type_name', '')
        if type_filter.lower() in kind.lower():
            return c
    return candidates[0]  # fallback to first


# ============================================================================
# Merge functions — one per sheet
# ============================================================================

# Resource name lookup (aResource[])
A_RESOURCE = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]


def _serialize_guard(guard) -> List[Dict[str, Any]]:
    """Convert ArmySlot list (dataclasses OR dicts) to plain dicts.

    post_tile_content stores guard as List[ArmySlot] (dataclass).
    However, by the time merge_*() runs in our pipeline, _post_tile_content
    has already been serialized to dicts via _serialize_post_tile_content.
    So guard may be either:
      - List[ArmySlot] (when called from internal save_parser before serialization)
      - List[Dict] (when called after serialization)
    """
    if not guard:
        return []
    out = []
    for s in guard:
        if isinstance(s, dict):
            out.append({"monster_id": s.get("monster_id", -1), "count": s.get("count", 0)})
        elif hasattr(s, "monster_id"):
            out.append({"monster_id": s.monster_id, "count": s.count})
        else:
            out.append({"raw": str(s)})
    return out


def _to_dict(obj):
    """Convert a dataclass or dict to plain dict (for safe iteration)."""
    if hasattr(obj, '__dict__') and not isinstance(obj, dict):
        from dataclasses import asdict
        try:
            return asdict(obj)
        except Exception:
            return {k: getattr(obj, k) for k in dir(obj) if not k.startswith('_')}
    return obj


def merge_artifacts(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any],
                    obj_dict: Optional[Dict] = None,
                    localizer=None,
                    raw: Optional[bytes] = None
                    ) -> List[Dict[str, Any]]:
    """Merge artifacts tile objects with ArtRes content (sheet "Артефакты").

    Universal: no hardcoding — uses:
      - art_metadata.get_artifact_tile_parser(type_id) for tile-level extraction
      - art_metadata.get_bank_config(type_id) for bank artifact-bank detection
      - art_metadata.get_artifact_class_id/relic_c_id for TblArt lookups
      - localizer.object(type_id) for localized object names
      - localizer.art_class(class_id) for localized class names
      - localizer.art(relic_c_id) for localized relic names

    Sources (per PRT MainForm.cs SaveArt/SaveSkeleton/SaveSurvivor/SaveTomb/SaveBanks):
      - type_id=5  (SaveArt): 1 row per ground artifact, artifact_id from raw[s+2]
      - type_id=22 (SaveSkeleton): 1 row per corpse, artifact_id present iff raw[s+8]==0xFF
      - type_id=86 (SaveSurvivor): 1 row per survivor, artifact_id from raw[s+6] directly
      - type_id=108 (SaveTomb): 1 row per tomb, artifact_id present iff raw[s+6]==31
      - type_id=16 (SaveBanks): no row in Артефакты (plain bank)
      - type_id=24, 84, 85 (SaveBanks): 1 row per bank, slot=1, artifact_id from bank's artifact_ids[0]
      - type_id=25 (SaveBanks / Dragon Utopia): 4 rows per bank, slots=1..4, artifact_id from bank's artifact_ids[i]

    Returns a list of records with fields:
      x, y, z, locality, object, slot, artifact_id, artifact_name, class, relic,
      gold, resource, guard, hp
    """
    # Lazy imports (so module loads even if art_metadata/localizer unavailable)
    try:
        from art_metadata import (
            get_artifact_tile_parser,
            extract_artifact_id_from_tile,
            get_bank_config,
            is_artifact_bank,
            get_bank_default_slots,
            is_bank_type,
            get_artifact_class_id,
            get_artifact_relic_c_id,
            get_all_artifact_bearing_type_ids,
            links_art_res,
            get_art_res_num,
            has_artifact_guard_check,
        )
        _has_art_meta = True
    except Exception:
        _has_art_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    # Localized "Артефакт" string for ground artifacts (type_id=5)
    ARTIFACT_OBJ_NAME = localizer.object(5) if localizer else "Артефакт"

    artifacts: List[Dict[str, Any]] = []

    def _build_rec(x, y, z, locality, object_name, slot, artifact_id,
                   gold=None, resource=None, guard=None):
        """Build a single artifact record with class/relic lookups."""
        rec = {
            "x": x, "y": y, "z": z,
            "locality": locality,
            "object": object_name,
            "slot": slot,
            "artifact_id": artifact_id if artifact_id is not None else -1,
            "artifact_name": "",
            "class_": None,
            "relic": None,
            "gold": gold,
            "resource": resource,
            "guard": guard,
            # ∑ HP is computed by excel_export via excel_calculations.artifact_sum_hp
            # (Excel-only — NOT stored in JSON).
        }
        if artifact_id is not None and artifact_id >= 0 and _has_art_meta:
            # Localized artifact name
            if localizer:
                rec["artifact_name"] = localizer.art(artifact_id)
            # Class (e.g., "Большой", "Малый", "Сокровище", "Реликвия")
            class_id = get_artifact_class_id(artifact_id)
            if localizer:
                rec["class_"] = localizer.art_class(class_id)
            # Combination relic (if part of one)
            relic_c_id = get_artifact_relic_c_id(artifact_id)
            if relic_c_id is not None and localizer:
                rec["relic"] = localizer.art(relic_c_id)
        return rec

    # ----- 1. Tile-level artifacts (type 5, 22, 86, 108) -----
    # For each tile object with a known artifact-tile parser, extract artifact_id
    # from the raw bytes (using PRT Save* logic) and emit a row.
    #
    # For ground artifacts (type_id=5) with has_guard=True, also link the
    # corresponding ArtRes record (matched by art_res_num) to attach guard info.
    # This preserves PRT behavior where guarded ground artifacts show their
    # guard string in the Охрана column.
    #
    # Matching logic (per PRT AnalysisContent, MainForm.cs:6809):
    #   - PRT sorts TblArtRes rows by "Num" field
    #   - Then iterates post-tile ArtRes records in that sorted order
    #   - So ArtRes[i] corresponds to ground artifact with art_res_num == i
    if _has_art_meta and raw is not None:
        # Build a dict: art_res_num → ArtRes record (by index = Num)
        art_res_records = []
        if post_tile_content and isinstance(post_tile_content, dict):
            for ar in post_tile_content.get("art_res", []):
                art_res_records.append(_to_dict(ar))
        # art_res_records[i] corresponds to art_res_num == i (sorted by Num per PRT)

        # For ground artifacts (type_id=5), look up ArtRes by art_res_num field
        # (which PRT computes from bytes: ((raw[s+9] & 0x7F) << 8) + (raw[s+8] >> 3))
        for obj in tile_objects:
            type_id = int(obj.get('type_id', 0))
            parser_cfg = get_artifact_tile_parser(type_id)
            if parser_cfg is None:
                continue  # not an artifact-bearing tile type

            # Tile offset where the object data starts
            s = obj.get('offset')
            if s is None or not isinstance(s, int):
                continue

            # Extract artifact_id via the type-specific parser (data-driven)
            artifact_id = extract_artifact_id_from_tile(raw, s, type_id)

            # Localized object name (e.g., "Артефакт", "Труп", "Потерпевший кораблекрушение", "Могила воина")
            if localizer:
                object_name = localizer.object(type_id) or f"Object {type_id}"
            else:
                object_name = obj.get('type_name') or f"Object {type_id}"

            locality = "море" if obj.get('loc') == 8 else "суша"

            # For tile types that link to ArtRes (data-driven check via
            # art_metadata.links_art_res), look up the linked ArtRes record
            # by art_res_num field (per PRT AnalysisContent logic).
            linked_guard = None
            if _has_art_meta and links_art_res(type_id) and has_artifact_guard_check(raw, s, type_id):
                arn = get_art_res_num(raw, s, type_id)
                if 0 <= arn < len(art_res_records):
                    ar = art_res_records[arn]
                    linked_guard = _serialize_guard(ar.get("guard", [])) or None

            # Always emit a row (per PRT SaveTomb/SaveSkeleton/SaveSurvivor/SaveArt behavior)
            rec = _build_rec(
                x=obj.get('x'), y=obj.get('y'), z=obj.get('z'),
                locality=locality, object_name=object_name,
                slot=None,  # tile-level artifacts have no slot
                artifact_id=artifact_id,
                guard=linked_guard,
            )
            artifacts.append(rec)

    # ----- 2. Ground artifacts fallback (when raw bytes unavailable) -----
    # If raw is None, fall back to using tile_object's stored artifact_id field
    # (set by _parse_artifact in tile_scanner). This works only for tile types
    # whose tile_scanner parser stores artifact_id directly (currently type 5,
    # but the check is data-driven: just look at whether the obj has the field).
    elif raw is None:
        for obj in tile_objects:
            type_id = int(obj.get('type_id', 0))
            # Skip types that don't have a tile-level artifact parser
            if not (_has_art_meta and get_artifact_tile_parser(type_id) is not None):
                continue
            # Use stored artifact_id if available (only set by tile_scanner for
            # types whose parser stores it directly, e.g. type 5 SaveArt).
            # For other types (Tomb, Corpse, Survivor), tile_scanner stores 'content'
            # raw byte but NOT the decoded artifact_id — those need raw bytes to
            # extract via art_metadata.extract_artifact_id_from_tile formula.
            stored_aid = obj.get('artifact_id')
            if stored_aid is None:
                continue  # can't decode without raw bytes
            # Localized object name
            if localizer:
                object_name = localizer.object(type_id) or f"Object {type_id}"
            else:
                object_name = obj.get('type_name') or f"Object {type_id}"
            rec = _build_rec(
                x=obj.get('x'), y=obj.get('y'), z=obj.get('z'),
                locality="море" if obj.get('loc') == 8 else "суша",
                object_name=object_name,
                slot=None,
                artifact_id=stored_aid,
            )
            artifacts.append(rec)

    # ----- 3. Bank artifacts (type 16, 24, 25, 84, 85) -----
    # Banks produce rows in Артефакты sheet iff is_artifact_bank(type_id) is True.
    # For each artifact bank, emit `default_slots` rows (or actual count from
    # bank's artifact_ids list, whichever is larger).
    if _has_art_meta and post_tile_content and isinstance(post_tile_content, dict):
        tile_map = _build_tile_object_map(tile_objects)
        post_banks = post_tile_content.get("banks", [])

        # Build a list of (tile_bank, post_bank) pairs by index (positional matching)
        # The order of post_banks matches the order of tile_banks in tile scan.
        tile_banks = [obj for obj in tile_objects
                      if is_bank_type(int(obj.get('type_id', 0)))]

        for i, pb in enumerate(post_banks):
            pb = _to_dict(pb)
            tb = tile_banks[i] if i < len(tile_banks) else {}
            type_id = int(tb.get('type_id', 0))

            if not is_artifact_bank(type_id):
                continue  # plain bank (e.g., code 16) — no row in Артефакты

            # Localized bank object name (e.g., "Утопия драконов", "Склеп", "Кораблекрушение", "Ветхий корабль")
            if localizer:
                object_name = localizer.object(type_id) or f"Bank {type_id}"
            else:
                object_name = f"Bank {type_id}"

            x, y, z = tb.get('x'), tb.get('y'), tb.get('z')
            locality = "море" if tb.get('loc') == 8 else "суша"

            # Artifact IDs from bank content (parsed by parse_bank_content with is_artifact_bank=True)
            artifact_ids = pb.get("artifact_ids", [])
            default_slots = get_bank_default_slots(type_id)

            # Number of rows to emit: max(default_slots, len(artifact_ids))
            # Per PRT: Dragon Utopia (type 25) always emits 4 rows (slots 1-4),
            # even if some are empty.
            n_slots = max(default_slots, len(artifact_ids)) if default_slots else len(artifact_ids)

            # Bank-wide fields (gold + guard) — attached to first row only
            # (matches PRT where Dragon Utopia's gold=20000 is on slot 1)
            bank_gold = pb.get("gold", 0) or None
            bank_guard = _serialize_guard(pb.get("guard", [])) or None

            for slot_idx in range(n_slots):
                # PRT BankContent (MainForm.cs:8994): slot is set ONLY when num > 1
                # (i.e., multi-slot banks like Dragon Utopia get slots 1..N; single-slot
                # banks like Crypt/Shipwreck/Derelict Ship get slot=None)
                slot_num = (slot_idx + 1) if n_slots > 1 else None
                art_id = artifact_ids[slot_idx] if slot_idx < len(artifact_ids) else None

                # Attach gold + guard to first row only (PRT behavior)
                gold = bank_gold if slot_idx == 0 else None
                guard = bank_guard if slot_idx == 0 else None

                rec = _build_rec(
                    x=x, y=y, z=z,
                    locality=locality,
                    object_name=object_name,
                    slot=slot_num,
                    artifact_id=art_id,
                    gold=gold,
                    guard=guard,
                )
                artifacts.append(rec)

    # ----- 4. (REMOVED) ArtRes records -----
    # Previously, raw ArtRes records (with only address + guard) were appended
    # here, producing 10+ "Книга Заклинаний" rows with None coords. PRT does NOT
    # include these in Артефакты sheet. Removed.

    # ----- 5. Wagons (type 105 = "Телега") — SaveWagon, MainForm.cs:10337 -----
    # PRT emits a R_Art row for EVERY wagon (regardless of whether it has an
    # artifact, a resource, or is empty). Only artifact-bearing wagons ALSO
    # get a R_AllArts row (handled in excel_calculations.aggregate_all_artifacts
    # via the heroes/banks path — wagons are NOT yet duplicated there because
    # they don't appear in tile scan heroes/banks; but the Артефакты sheet
    # just needs the R_Art row).
    #
    # _parse_wagon already classified each wagon as 'artifact'/'resource'/'empty'
    # and extracted artifact_id + resource_type + resource_amount per PRT formula:
    #   artifact case: artifact_id = (raw[s+8] << 1) + (raw[s+7] >> 7)
    #   resource case: Resource = aResource[(raw[s+9] & 0xF) >> 1] + raw[s+6]
    if localizer:
        wagon_name = localizer.object(105)  # "Телега"
    else:
        wagon_name = "Телега"
    # PRT aResource[] = ["Дерево", "Ртуть", "Руда", "Сера", "Кристалл", "Самоцвет"]
    # (DEFAULT Russian names). For Артефакты sheet "Ресурс" column PRT outputs
    # the resource letter+amount: e.g. "G2" (2 gems), "M5" (5 mercury).
    # See MainForm.cs SaveWagon line 10351:
    #   r_ArtRow.Resource = aResource[(decmp[s+9] & 0xF) >> 1] + decmp[s+6]
    # aResource DEFAULT array (MainForm.cs:11762) — using FIRST LETTER only
    # since PRT DEFAULT display uses single-letter codes:
    #   ["Дерево", "Ртуть", "Руда", "Сера", "Кристалл", "Самоцвет"]
    # But the actual PRT output uses "G2"/"M5" style — that's because aResource
    # contains full names but PRT display concatenates first letter + amount.
    # Actually checking: aResource is itself full names ("Gold", "Wood"...) and
    # PRT writes them AS-IS + the amount. We use the DEFAULT resource letter
    # convention matching PRT's actual 0000.xlsx output: "W"/"M"/"O"/"S"/"C"/"G".
    _A_RESOURCE_LETTERS = ["W", "M", "O", "S", "C", "G"]
    for obj in tile_objects:
        if int(obj.get('type_id', 0)) != 105:
            continue
        mode = obj.get('mode', 'empty')
        artifact_id = obj.get('artifact_id')
        resource_type = obj.get('resource_type')
        resource_amount = obj.get('resource_amount')

        # Format Resource column (matches PRT DEFAULT "G2" style)
        resource_str = None
        if mode == 'resource' and resource_type is not None and resource_amount is not None:
            if 0 <= resource_type < len(_A_RESOURCE_LETTERS):
                resource_str = f"{_A_RESOURCE_LETTERS[resource_type]}{resource_amount}"

        # Always emit a row (matches PRT SaveWagon's always-added R_Art row)
        rec = _build_rec(
            x=obj.get('x'), y=obj.get('y'), z=obj.get('z'),
            locality="море" if obj.get('loc') == 8 else "суша",
            object_name=wagon_name,
            slot=None,
            artifact_id=artifact_id if (mode == 'artifact' and artifact_id is not None) else None,
            resource=resource_str,
        )
        artifacts.append(rec)

    return artifacts


def merge_monsters(tile_objects: List[Dict[str, Any]],
                   post_tile_content: Dict[str, Any],
                   obj_dict: Optional[Dict] = None,
                   localizer=None,
                   raw: Optional[bytes] = None
                   ) -> List[Dict[str, Any]]:
    """Merge monster tile objects + post-tile Monstr content (sheet "Монстры").

    Universal: no hardcoding — uses art_metadata.get_monster_info for
    TblMonsters lookups (name, level, hp). PRT MainForm.cs SaveMonster +
    MonstrContent logic.

    PRT behavior:
      - SaveMonster (tile scan): reads monster_id at s+2, count at s+6..s+7
        (u12), mood at s+7 (high nibble), grows at s+8 bit 2, has_guard at
        s+9 bit 7. Looks up Name + Level + HP from TblMonsters.
        Sum HP = monster_hp * count.
        Прирост = "Да" if grows, else None.
      - MonstrContent (post-tile): for each guarded monster (has_guard=True),
        reads resources (6 x 4 bytes), gold (24-bit LE), artifact_id (u8).
        Linked by monstr_num field (sorted index).

    Returns records with:
      x, y, z, locality, monster (name), count, mood, level, hp (Sum HP),
      artifact (name), gold, resource (PRT "G8, S5"), grows ('Да' or None)
    """
    try:
        from art_metadata import (
            get_monster_info,
            get_monster_name,
            get_monster_level,
        )
        _has_meta = True
    except Exception:
        _has_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    monsters: List[Dict[str, Any]] = []

    # Build MonstrContent records index (by monstr_num) for guarded monster linking.
    # Per PRT AnalysisMonstrContent: TblMonstr is sorted by Num, and aMonstrID[Num]
    # gives the offset. Our post_tile_content["monstr_records"] is already in
    # parse order (which corresponds to sorted-by-Num order). So monstr_records[i]
    # corresponds to the guarded monster with monstr_num == i.
    monstr_records: List[Dict[str, Any]] = []
    if post_tile_content and isinstance(post_tile_content, dict):
        for mr in post_tile_content.get("monstr_records", []):
            monstr_records.append(_to_dict(mr))

    # Resource letter mapping (matches PRT aResource[])
    A_RESOURCE_LETTERS = ["W", "M", "O", "S", "C", "G"]
    A_RESOURCE_NAMES = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
    name_to_letter = dict(zip(A_RESOURCE_NAMES, A_RESOURCE_LETTERS))

    for obj in tile_objects:
        if obj.get('type_id') != 54:
            continue

        # Monster name + Level + HP.
        # Use localizer.monstr() (ru_sod) for NAME — matches PRT's binary output
        # exactly. Use get_monster_info() only for metadata (level, hp, altar,
        # town) — those are structural, not name translations.
        monster_id = obj.get('monster_type', 0)
        monster_name = localizer.monstr(monster_id) if localizer else None
        level = 0
        base_hp = 0
        if _has_meta:
            info = get_monster_info(monster_id)
            if info:
                # Only use metadata from tbl_monsters_mapping — NOT the name
                # (tbl_monsters_mapping has ru_wog names which differ from
                # PRT's actual ru_sod binary output).
                if not monster_name:
                    monster_name = info.get("name", f"M{monster_id}")
                level = int(info.get("level", 0))
                base_hp = int(info.get("hp", 0))
            else:
                if not monster_name:
                    monster_name = f"M{monster_id}"
        else:
            if not monster_name:
                monster_name = f"M{monster_id}"

        count = obj.get('count', 0)
        mood = obj.get('mood', 0)
        grows = obj.get('grows', False)

        # Sum HP = monster_hp * count (PRT: r_MonstrRow.HP *= Number)
        # NOTE: ∑ HP is computed by excel_export via excel_calculations.monster_sum_hp
        # (Excel-only — NOT stored in JSON to avoid polluting the JSON/tree output).
        # We store `monster_type` (raw int from save) so excel_export can compute HP.

        # прирост: bit set = "doesn't grow" (override), bit clear = "grows" (default)
        # PRT MainForm.cs:10514 — Increase = ((decmp[s+8] & 4) > 0) ? aReply[0] : aReply[1]
        # aReply[0]="Нет" (bit set = override = no growth), aReply[1]="Да" (bit clear = default = grows)
        grows_str = "Нет" if grows else "Да"

        # For guarded monsters, link MonstrContent by monstr_num
        artifact_name = None
        gold = None
        resource_str = None
        art_id = None  # initialize before the if block (so it's always defined)

        if obj.get('has_artifact') and obj.get('monstr_num', 0) < len(monstr_records):
            mr = monstr_records[obj['monstr_num']]

            # Resources: format as PRT "G8, S5" string
            resources_dict = mr.get("resources", {})
            if isinstance(resources_dict, dict):
                res_parts = []
                for rname in A_RESOURCE_NAMES:
                    amount = resources_dict.get(rname, 0)
                    if amount and amount > 0:
                        letter = name_to_letter.get(rname, "?")
                        res_parts.append(f"{letter}{amount}")
                resource_str = ", ".join(res_parts) if res_parts else None

            # Gold (24-bit LE from parse_monstr_content)
            gold_val = mr.get("gold", 0)
            gold = gold_val if gold_val > 0 else None

            # Artifact (u8, 0xFF = none)
            art_id = mr.get("artifact_id", -1)
            if art_id is not None and 0 <= art_id < 0xFF:
                artifact_name = localizer.art(art_id) if localizer else f"A{art_id}"
            else:
                art_id = None

        # Store artifact_id for aggregate_all_artifacts to look up class/relic
        # (PRT MonstrContent gives u8 artifact_id; we keep raw int separately
        # from the localized name field `artifact`).
        artifact_id_field = art_id if art_id is not None else -1

        monsters.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "monster": monster_name,
            "monster_type": monster_id,  # raw int — used by excel_calculations.monster_sum_hp
            "count": count,
            "mood": mood,
            "level": level,
            "artifact": artifact_name,
            "artifact_id": artifact_id_field,  # raw int (or -1 if no artifact)
            "gold": gold,
            "resource": resource_str,
            "grows": grows_str,
        })

    return monsters


def merge_chests(tile_objects: List[Dict[str, Any]],
                 obj_dict: Optional[Dict] = None
                 ) -> List[Dict[str, Any]]:
    """Merge chest tile objects (sheet "Сундуки").

    Returns records with:
      x, y, z, locality, object, gold, experience, artifact_id, has_artifact
    """
    # Code 101 = "Сундук с сокровищами" / "Treasure Chest"
    # Code 82  = "Морской сундук" / "Sea Chest"
    # Use localized names from the localizer (matches PRT TblObject[code][1]).
    try:
        from localizer import get_localizer
        loc = get_localizer()
        chest_name = loc.object(101) or "Сундук с сокровищами"
        sea_chest_name = loc.object(82) or "Морской сундук"
    except Exception:
        chest_name = "Сундук с сокровищами"
        sea_chest_name = "Морской сундук"

    chests = []
    for obj in tile_objects:
        if obj.get('type_id') not in (101, 82):  # Chest + Sea Chest
            continue
        is_sea = obj.get('type_id') == 82
        chests.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": sea_chest_name if is_sea else chest_name,
            "gold": obj.get('gold', 0),
            "experience": obj.get('experience', 0),
            "has_artifact": obj.get('has_artifact', False),
            "artifact_id": obj.get('artifact_id', -1) if obj.get('has_artifact') else -1,
        })
    return chests


def merge_resources(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Optional[Dict[str, Any]] = None,
                    localizer=None,
                    raw: Optional[bytes] = None
                    ) -> List[Dict[str, Any]]:
    """Merge resource tile objects (sheet "Ресурсы").

    Universal: no hardcoding — uses art_metadata.extract_resource_from_tile
    to compute resource letter+amount and/or gold for ALL 6 resource-bearing
    object types via data-driven JSON config (resource_tile_parsers.json).

    PRT behavior (MainForm.cs SaveRes/SaveCampfire/SaveWindmill/SaveMysticalGarden/
    SaveFloatsam/SaveHovel) — all 6 types emit a row in R_Resource sheet.

    Returns records with:
      x, y, z, locality, object (localized name), resource (e.g. "S4", "W10"), gold,
      guard (from ArtRes, only for guarded Resource piles), hp
    """
    try:
        from art_metadata import (
            extract_resource_from_tile,
            is_resource_bearing_tile_type,
            get_resource_tile_parser,
        )
        _has_res_meta = True
    except Exception:
        _has_res_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    resources: List[Dict[str, Any]] = []

    # Build ArtRes records index (by Num field) for guard linking — same logic
    # as merge_artifacts but for Resource tiles (Type=1 in TblArtRes).
    art_res_records: List[Dict[str, Any]] = []
    if post_tile_content and isinstance(post_tile_content, dict):
        for ar in post_tile_content.get("art_res", []):
            art_res_records.append(_to_dict(ar))

    for obj in tile_objects:
        type_id = int(obj.get('type_id', 0))
        if not (_has_res_meta and is_resource_bearing_tile_type(type_id)):
            continue  # not a resource-bearing tile type

        s = obj.get('offset')
        if s is None or not isinstance(s, int) or raw is None:
            continue

        # Extract resource info via data-driven config (per PRT Save* logic)
        res_info = extract_resource_from_tile(raw, s, type_id)

        # Localized object name (e.g., "Ресурс", "Бивачный Костер",
        # "Ветряная мельница", "Мистический сад", "Обломки", "Прислониться к")
        if localizer:
            object_name = localizer.object(type_id) or f"Object {type_id}"
        else:
            object_name = obj.get('type_name') or f"Object {type_id}"

        locality = "море" if obj.get('loc') == 8 else "суша"

        # For guarded Resource piles (type 79), link to ArtRes by art_res_num
        # to attach guard info (matches PRT AnalysisContent logic).
        # NOTE: PRT's ArtResContent checks row["Type"] == 1 to ensure the
        # ArtRes record is a resource-type entry (not artifact). OUR's
        # post_tile_parser doesn't track art_type (hardcoded to 0), so we
        # can't check it. But we trust that art_res_num (computed per PRT
        # formula from raw bytes) points to the correct ArtRes record — each
        # tile has its own unique art_res_num, so there's no ambiguity.
        # We just link the guard unconditionally when has_guard is True.
        linked_guard = None
        if res_info.get("has_guard") and res_info.get("art_res_num", 0) < len(art_res_records):
            ar = art_res_records[res_info["art_res_num"]]
            linked_guard = _serialize_guard(ar.get("guard", [])) or None

        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": locality,
            "object": object_name,
            "resource": res_info.get("resource"),
            "gold": res_info.get("gold"),
            "guard": linked_guard,
            # ∑ HP is computed by excel_export via excel_calculations.resource_sum_hp
            # (Excel-only — NOT stored in JSON).
        }
        resources.append(rec)

    return resources


def merge_event_boxes(tile_objects: List[Dict[str, Any]],
                      post_tile_content: Optional[Dict[str, Any]] = None,
                      localizer=None,
                      raw: Optional[bytes] = None
                      ) -> List[Dict[str, Any]]:
    """Merge event tile objects + Pandora boxes with EventBox content
    (sheet "События и Ящики Пандоры").

    PRT logic:
      - SaveEvent (line 10065): creates R_EventBox row with X/Y/Z/Num from tile scan
      - AnalysisContent: iterates TblEventBox (sorted by Num) + calls EventBoxContent
        to fill guard/experience/mana/morale/luck/resources/skills/artifacts/spells/monsters

    Universal: sort tile events by event_num (matching PRT's TblEventBox.Sort = "Num"),
    match positionally to post_tile event_boxes (same order as parse = sorted by Num).
    """
    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    # Get tile events (type 26) and Pandora boxes (type 6) — both go to this sheet
    events = [o for o in tile_objects if o.get('type_id') in (26, 6)]
    # Sort by event_num (PRT TblEventBox.Sort = "Num")
    events_sorted = sorted(events, key=lambda e: e.get('event_num', 0))

    # Get post_tile event_boxes (in parse order = sorted by Num order)
    ebs = []
    if post_tile_content and isinstance(post_tile_content, dict):
        for eb in post_tile_content.get("event_boxes", []):
            ebs.append(_to_dict(eb))

    # Get human player color for "Доступно" column
    # PRT line 7658: aExistColor[i] = decmp[color + i*145 + 24] + decmp[color + i*145 + 1]
    # PRT line 7661: human = i where decmp[color_off + i*145 + 14] == 3
    human_color = None
    human_idx = -1
    a_exist_color = [0] * 8
    if raw is not None and post_tile_content and isinstance(post_tile_content, dict):
        offsets = post_tile_content.get("offsets", {})
        color_off = offsets.get("color", 0)
        if color_off > 0 and localizer:
            for i in range(8):
                off_human = color_off + i * 145 + 14
                if off_human < len(raw) and raw[off_human] == 3:
                    human_idx = i
                    human_color = localizer.color(i)
                # aExistColor: sum of two bytes within the player section
                off24 = color_off + i * 145 + 24
                off1 = color_off + i * 145 + 1
                if off24 < len(raw) and off1 < len(raw):
                    a_exist_color[i] = raw[off24] + raw[off1]

    # PRT aColor[] order (Myth and Legend ru_sod localization): Red, Blue, Tan, Green,
    # Orange, Purple, Teal, Pink. Use localizer.color(i) to match the active localization.
    # We use localizer.color() for each existing color index to build the name list.

    # Resource letter mapping (matches PRT aResource[])
    A_RESOURCE_LETTERS = ["W", "M", "O", "S", "C", "G"]
    A_RESOURCE_NAMES = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
    name_to_letter = dict(zip(A_RESOURCE_NAMES, A_RESOURCE_LETTERS))

    # Primary skill short codes (PRT aPrSkill)
    A_PRSKILL = ["A", "D", "P", "K"]  # Attack, Defense, Power, Knowledge
    PRSKILL_NAMES = ["Attack", "Defense", "Power", "Knowledge"]
    name_to_prskill = dict(zip(PRSKILL_NAMES, A_PRSKILL))

    # Secondary skill level prefixes (PRT aLevelSkill)
    # Use localizer if available (returns "Б."/"П."/"Э." for ru_sod, ru_wog;
    # "1."/"2."/"3." for ru_complete; "B."/"A."/"E." for en).
    # Previously was hardcoded as ["Б.", "А.", "Е."] — the "А." was wrong
    # (English "Advanced" first letter, not Russian "П" = "Продвинутый").
    # PRT 0000.xlsx output uses ru_sod = "Б."/"П."/"Э.", so we match it.
    if localizer:
        A_LEVELSKILL = [localizer.level_skill(i) for i in range(3)]
        # Fallback if localizer returned empty strings
        if not all(A_LEVELSKILL):
            A_LEVELSKILL = ["Б.", "П.", "Э."]
    else:
        A_LEVELSKILL = ["Б.", "П.", "Э."]

    records = []
    for i, event_tile in enumerate(events_sorted):
        # Get coordinates from tile scan
        x = event_tile.get('x')
        y = event_tile.get('y')
        z = event_tile.get('z')
        loc = event_tile.get('loc', 0)

        # Localized object name
        type_id = event_tile.get('type_id', 26)
        object_name = localizer.object(type_id) if localizer else "Событие"

        # Get content from post_tile event_box (matched positionally)
        eb = ebs[i] if i < len(ebs) else {}

        # Guard: PRT EventBoxContent (line 8198) does NOT use ItWas dedup.
        # Each guard slot is listed separately (e.g., "Нага 3, Нага 3, Нага 3").
        guard_list = _serialize_guard(eb.get("guard", []))
        guard_str = None
        # ∑ HP for guard is computed by excel_export via excel_calculations.event_box_sum_hp
        # (Excel-only — NOT stored in JSON to avoid polluting the JSON/tree output).
        # We store `guard_slots` (raw list of {monster_id, count}) so excel_calculations
        # can compute HP; `guard` field keeps the formatted display string.
        if guard_list:
            # Format guard string WITHOUT dedup (each slot shown separately)
            parts = []
            for g in guard_list:
                mid = g.get("monster_id", -1)
                cnt = g.get("count", 0)
                mname = localizer.monstr(mid) if localizer else f"M{mid}"
                parts.append(f"{mname} {cnt}")
            guard_str = ", ".join(parts) if parts else None

        # Experience
        experience = eb.get("experience", 0) or None

        # Mana
        mana = eb.get("mana", 0) or None

        # Morale (PRT format: "+2" for positive, "-2" for negative)
        morale_val = eb.get("morale", 0)
        morale_str = None
        if morale_val and morale_val != 0:
            morale_str = f"+{morale_val}" if morale_val > 0 else str(morale_val)

        # Luck (same format as morale)
        luck_val = eb.get("luck", 0)
        luck_str = None
        if luck_val and luck_val != 0:
            luck_str = f"+{luck_val}" if luck_val > 0 else str(luck_val)

        # Gold (from resources dict)
        resources_dict = eb.get("resources", {})
        gold = resources_dict.get("Gold", 0) if isinstance(resources_dict, dict) else 0
        gold_str = gold if gold else None

        # Resource (PRT format: "W-10, G-5" — letter + signed amount)
        res_parts = []
        if isinstance(resources_dict, dict):
            for rname in A_RESOURCE_NAMES:
                amount = resources_dict.get(rname, 0)
                if amount and amount != 0:
                    letter = name_to_letter.get(rname, "?")
                    res_parts.append(f"{letter}{amount}")
        resource_str = ", ".join(res_parts) if res_parts else None

        # Primary skills (PRT format: "K1" — no "+" sign, just code+value)
        ps_dict = eb.get("primary_skills", {})
        ps_parts = []
        if isinstance(ps_dict, dict):
            for pname, pval in ps_dict.items():
                if pval and pval != 0:
                    code = name_to_prskill.get(pname, pname[:1].upper())
                    ps_parts.append(f"{code}{pval}")
        ps_str = ", ".join(ps_parts) if ps_parts else None

        # Secondary skills (PRT format: "Б.Мудрость, А.Меткость")
        ss_list = eb.get("secondary_skills", [])
        ss_parts = []
        if ss_list:
            for ss in ss_list:
                if isinstance(ss, dict):
                    sid = ss.get("skill_id", -1)
                    lvl = ss.get("level", 0)
                    if sid >= 0 and lvl > 0:
                        skill_name = localizer.skill(sid) if localizer else f"Sk{sid}"
                        prefix = A_LEVELSKILL[lvl - 1] if lvl <= 3 else f"L{lvl}."
                        ss_parts.append(f"{prefix}{skill_name}")
                elif isinstance(ss, (list, tuple)) and len(ss) >= 2:
                    sid, lvl = ss[0], ss[1]
                    if sid >= 0 and lvl > 0:
                        skill_name = localizer.skill(sid) if localizer else f"Sk{sid}"
                        prefix = A_LEVELSKILL[lvl - 1] if lvl <= 3 else f"L{lvl}."
                        ss_parts.append(f"{prefix}{skill_name}")
        ss_str = ", ".join(ss_parts) if ss_parts else None

        # Artifacts (PRT format: "Shield of Dwarven Lords, Sword of Justice")
        art_list = eb.get("artifacts", [])
        art_parts = []
        if art_list:
            for aid in art_list:
                if isinstance(aid, int) and aid >= 0:
                    art_parts.append(localizer.art(aid) if localizer else f"A{aid}")
        art_str = ", ".join(art_parts) if art_parts else None

        # Spells (PRT format: "Haste, Bless")
        spl_list = eb.get("spells", [])
        spl_parts = []
        if spl_list:
            for sid in spl_list:
                if isinstance(sid, int) and sid >= 0:
                    spl_parts.append(localizer.spell(sid) if localizer else f"S{sid}")
        spl_str = ", ".join(spl_parts) if spl_parts else None

        # Monsters (PRT format: "Pikeman 10, Archer 5" — no "x")
        mon_list = eb.get("monsters", [])
        mon_parts = []
        if mon_list:
            for m in mon_list:
                if isinstance(m, dict):
                    mid = m.get("monster_id", -1)
                    cnt = m.get("count", 0)
                elif isinstance(m, (list, tuple)) and len(m) >= 2:
                    mid, cnt = m[0], m[1]
                else:
                    continue
                if mid >= 0:
                    mname = localizer.monstr(mid) if localizer else f"M{mid}"
                    mon_parts.append(f"{mname} {cnt}")
        mon_str = ", ".join(mon_parts) if mon_parts else None

        # "Доступно" — PRT GetEventApply (MainForm.cs:7088).
        # For each color j (0..7):
        #   if (apply_flags bit j set) AND (color j exists on map):
        #     - if j == human: include color name (always)
        #     - else if apply_for_all (num2 = decmp[s+8] & 4) > 0: include color name
        # For Pandora's Box (type 6), apply_flags is not set (no SaveEvent tile record),
        # so available stays None (PRT does not call GetEventApply for Pandora's boxes).
        #
        # PRT's aColor[] array is set by CreateDefaultArray (MainForm.cs:11750),
        # which is the array used by ObjNameDefault — the mode PRT actually runs in
        # for the Myth and Legend save (per 0000.xlsx output). The DEFAULT aColor
        # uses "Пурпурный"/"Бирюзовый" for indices 5/6, NOT the SoD-RU array values
        # ("Багровый"/"Чайный") which the localizer's ru_sod mode would return.
        # We hardcode the DEFAULT aColor list to match PRT's actual output.
        _A_COLOR = ["Красный", "Синий", "Коричневый", "Зеленый",
                    "Оранжевый", "Пурпурный", "Бирюзовый", "Розовый"]
        apply_flags = event_tile.get('apply_flags', 0)
        apply_for_all = event_tile.get('apply_for_all', False)
        available = None
        if apply_flags and human_idx >= 0:
            avail_colors = []
            for j in range(8):
                if (apply_flags & (1 << j)) > 0 and a_exist_color[j] > 0:
                    if j == human_idx:
                        avail_colors.append(_A_COLOR[j])
                    elif apply_for_all:
                        avail_colors.append(_A_COLOR[j])
            if avail_colors:
                available = ", ".join(avail_colors)

        records.append({
            "x": x, "y": y, "z": z,
            "locality": "море" if loc == 8 else "суша",
            "object": object_name,
            "guard": guard_str,
            "guard_slots": guard_list,  # raw list for excel_calculations.event_box_sum_hp
            "experience": experience,
            "mana": mana,
            "morale": morale_str,
            "luck": luck_str,
            "gold": gold_str,
            "resource": resource_str,
            "primary_skill": ps_str,
            "secondary_skill": ss_str,
            "artifact": art_str,
            # Also store raw artifact IDs (used by aggregate_all_artifacts to
            # look up class/relic per-artifact for "Все Арты" sheet).
            "artifact_ids": list(art_list) if art_list else [],
            "spell": spl_str,
            "monster": mon_str,
            "available": available,
            # Repeat: PRT aReply[((decmp[s+8] & 8) >> 3) ^ 1] — "Нет" if bit 3 set, "Да" if clear
            "repeat": "Нет" if event_tile.get("repeat", False) else "Да",
        })

    return records


def merge_banks(tile_objects: List[Dict[str, Any]],
                post_tile_content: Dict[str, Any],
                localizer=None,
                raw: Optional[bytes] = None
                ) -> List[Dict[str, Any]]:
    """Merge bank tile objects + post-tile Bank content (sheet "Банки").

    Universal: no hardcoding — uses art_metadata.is_bank_type and
    is_artifact_bank to determine which types produce rows. Per PRT logic
    (MainForm.cs SaveBanks + BankContent): only PLAIN banks (code 16)
    produce rows in Банки sheet. Artifact-banks (24, 25, 84, 85) go to
    Артефакты sheet (handled by merge_artifacts).

    Returns records with:
      x, y, z, locality, object (localized bank name), guard (list),
      monster_reward (formatted "<name> <count>"), gold (int), resources
      (PRT-formatted "G8, S5" string), hp (∑ HP)
    """
    try:
        from art_metadata import (
            is_bank_type,
            is_artifact_bank,
            get_monster_info,
            get_monster_name,
            get_monster_level,
        )
        _has_meta = True
    except Exception:
        _has_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    banks = []
    # Tile scan: banks have type_id 16, 24, 25, 84, 85 (we still match all for
    # positional pairing with post_banks). But we only EMIT rows for plain
    # banks (type_id 16) — artifact-banks go to Артефакты sheet.
    tile_banks = [obj for obj in tile_objects
                  if _has_meta and is_bank_type(int(obj.get('type_id', 0)))]
    post_banks = post_tile_content.get("banks", []) if post_tile_content else []

    # Resource letter mapping (matches PRT aResource[])
    A_RESOURCE = ["W", "M", "O", "S", "C", "G"]  # Wood, Mercury, Ore, Sulfur, Crystal, Gems

    for i, pb in enumerate(post_banks):
        pb = _to_dict(pb)
        tb = tile_banks[i] if i < len(tile_banks) else {}
        type_id = int(tb.get('type_id', 0))

        # Skip artifact-banks — they go to Артефакты sheet, not Банки
        if _has_meta and is_artifact_bank(type_id):
            continue

        # Skip non-bank types (defensive)
        if _has_meta and not is_bank_type(type_id):
            continue

        # Localized bank name using subtype (e.g., "Склады Циклопов", "Банк Наг")
        # PRT MainForm.cs:9924 uses TblObject.Select("Code='16' AND Type='decmp[s+2]'")
        bank_subtype = None
        s = tb.get('offset')
        if s is not None and isinstance(s, int) and raw is not None and s + 3 <= len(raw):
            bank_subtype = raw[s + 2]

        if localizer and bank_subtype is not None:
            object_name = localizer.object(type_id, bank_subtype) or f"Bank {type_id}"
        elif localizer:
            object_name = localizer.object(type_id) or f"Bank {type_id}"
        else:
            object_name = f"Bank {type_id}"

        # Guard: list of {monster_id, count} (already parsed correctly with
        # u8 count at offset s+i*4+28 in Step 7 fix to _parse_guard)
        guard_list = pb.get("guard", [])

        # ∑ HP is computed by excel_export via excel_calculations.bank_sum_hp
        # (Excel-only — NOT stored in JSON to avoid polluting the JSON/tree output).
        # The bank's guard is already stored as `guard` (list of {monster_id, count})
        # in the rec, so excel_calculations can compute HP from it.

        # Format Monster reward as "<name> <count>" (PRT GetBankMonster:9015)
        # PRT: TblMonsters[decmp[s]][1] + " " + decmp[s+4]
        # In our parse_bank_content, monster_reward_id is at offset +0, count at +4
        # (within the 7-byte monster reward section after the 28-byte resource/gold)
        monster_reward_id = pb.get("monster_reward_id", -1)
        monster_reward_count = pb.get("monster_reward_count", 0)
        monster_str = None
        if monster_reward_id is not None and 0 <= monster_reward_id < 255:
            # Primary: localizer (ru_sod) — matches PRT's binary output exactly.
            # Fallback: tbl_monsters_mapping only if localizer missing.
            if localizer:
                mname = localizer.monstr(monster_reward_id) or (
                    get_monster_name(monster_reward_id) if _has_meta else f"M{monster_reward_id}"
                )
            else:
                mname = (get_monster_name(monster_reward_id) if _has_meta
                         else f"M{monster_reward_id}")
            # PRT format: "<name> <count>" (e.g., "Виверн 4")
            monster_str = f"{mname} {monster_reward_count}" if monster_reward_count > 0 else mname

        # Format Resources as PRT "G8, S5" string (GetBankResource:9018)
        # Resources stored as dict {'Gems': 8, 'Wood': 4, ...} in OUR parse
        resources_dict = pb.get("resources", {})
        res_parts = []
        if isinstance(resources_dict, dict):
            # Iterate in canonical order: W, M, O, S, C, G
            # Map English name → letter
            name_to_letter = {"Wood": "W", "Mercury": "M", "Ore": "O",
                              "Sulfur": "S", "Crystal": "C", "Gems": "G"}
            for rname in ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]:
                amount = resources_dict.get(rname, 0)
                if amount and amount > 0:
                    letter = name_to_letter.get(rname, "?")
                    res_parts.append(f"{letter}{amount}")
        resource_str = ", ".join(res_parts) if res_parts else None

        # Gold (u16 from parse_bank_content)
        gold = pb.get("gold", 0) or None

        banks.append({
            "x": tb.get('x'),
            "y": tb.get('y'),
            "z": tb.get('z'),
            "locality": "море" if tb.get('loc') == 8 else "суша",
            "object": object_name,
            "guard": guard_list,  # keep as list for _guard_str formatting in excel
            "monster_reward": monster_str,
            "gold": gold,
            "resources": resource_str,  # PRT-formatted string
            # Keep raw fields for backward compat / debugging
            "is_artifact_bank": pb.get("is_artifact_bank", False),
            "address": pb.get("address"),
            "artifact_ids": pb.get("artifact_ids", []),
        })
    return banks


def merge_garrisons(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any]
                    ) -> List[Dict[str, Any]]:
    """Merge garrison tile objects + post-tile Garrison content (sheet "Garrisons").

    Returns records with: x, y, z, locality, object, anti_magic, guard, color, can_take

    PRT layout (SaveGarrison + GarrisonContent):
      - Tile scan (SaveGarrison, line 10017): reads anti_magic from decmp[s+2]
      - Post-tile (GarrisonContent, line 8136): reads guard/color/can_take from
        the 61-byte record (color at offset 0, monster_ids at i*4+1, counts at
        i*4+29,30, X/Y/Z at 57..59, CanTake at 60)
    """
    garrisons = []
    tile_garrisons = [obj for obj in tile_objects if obj.get('type_id') == 33]
    post_garrisons = post_tile_content.get("garrisons", [])

    for i, pg in enumerate(post_garrisons):
        pg = _to_dict(pg)
        tg = tile_garrisons[i] if i < len(tile_garrisons) else {}
        garrisons.append({
            "x": tg.get('x'),
            "y": tg.get('y'),
            "z": tg.get('z'),
            "locality": "море" if tg.get('loc') == 8 else "суша",
            "object": "Garrison",
            "anti_magic": tg.get('anti_magic', False),
            "address": pg.get("address"),
            "guard": _serialize_guard(pg.get("guard", [])),
            "color": pg.get("color", -1),
            "can_take": pg.get("can_take", 0),
        })
    return garrisons


def merge_seer_huts(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any],
                    localizer=None
                    ) -> List[Dict[str, Any]]:
    """Merge seer hut tile objects + post-tile SeerHut content (sheet "Провидцы").

    Returns records with:
      x, y, z, locality, object, num (quest ID), mission_type, mission,
      deadline, reward_type, reward
    """
    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None
    # Localized object name (PRT TblObject code 83 = "Хижина Провидца")
    seer_hut_name = localizer.object(83) if localizer else "Хижина Провидца"

    seer_huts = []
    tile_seer = [obj for obj in tile_objects if obj.get('type_id') == 83]
    post_seer = post_tile_content.get("seer_huts", [])

    # PRT AnalysisContent sorts TblSeerHut by Num, then iterates in that order.
    # post_seer[i] corresponds to the SeerHut with Num == i.
    # OUR's tile scan stores `num` field per tile object. Match by num.
    for ts in tile_seer:
        num = ts.get("num")
        if num is not None and 0 <= num < len(post_seer):
            ps = _to_dict(post_seer[num])
        else:
            # Fallback: positional matching (if num missing)
            i = tile_seer.index(ts)
            ps = _to_dict(post_seer[i]) if i < len(post_seer) else {}
        seer_huts.append({
            "x": ts.get('x'),
            "y": ts.get('y'),
            "z": ts.get('z'),
            "locality": "море" if ts.get('loc') == 8 else "суша",
            "object": seer_hut_name,
            "address": ps.get("address"),
            "num": ts.get("num", 0),
            "mission_type": ps.get("mission_type", 0),
            "mission": ps.get("mission", {}),
            "deadline": ps.get("deadline"),
            "reward_type": ps.get("reward_type", 0),
            "reward": ps.get("reward", {}),
        })
    return seer_huts


def merge_pass_guards(tile_objects: List[Dict[str, Any]],
                      post_tile_content: Dict[str, Any]
                      ) -> List[Dict[str, Any]]:
    """Merge border guard tile objects + post-tile PassGuard content.

    IMPORTANT: PRT AnalysisContent (MainForm.cs:6818) sorts TblPassGuard by
    Num field, then iterates in that order to parse save bytes. This means
    post_tile_content["pass_guards"][i] corresponds to the PassGuard with
    Num == i.

    OUR's tile scan stores `num` field for each tile object. To match
    tile_pg[i] with the correct post_pg entry, we use tile_pg[i].num as
    index into post_pg (which is already in Num-sorted order).

    Without this fix, positional matching (tile_pg[i] ↔ post_pg[i]) produces
    SHIFTED results because tile scan order != Num order.
    """
    pass_guards = []
    tile_pg = [obj for obj in tile_objects if obj.get('type_id') == 215]
    post_pg = post_tile_content.get("pass_guards", [])

    for i, tp in enumerate(tile_pg):
        # Use `num` field to find the matching post_pg entry
        num = tp.get("num")
        if num is not None and 0 <= num < len(post_pg):
            pp = _to_dict(post_pg[num])
        elif i < len(post_pg):
            # Fallback: positional matching (only if num is missing/invalid)
            pp = _to_dict(post_pg[i])
        else:
            pp = {}
        pass_guards.append({
            "x": tp.get('x'),
            "y": tp.get('y'),
            "z": tp.get('z'),
            "locality": "море" if tp.get('loc') == 8 else "суша",
            "object": "Border Guard",
            "address": pp.get("address"),
            "num": tp.get("num", 0),
            "mission_type": pp.get("mission_type", 0),
            "mission": pp.get("mission", {}),
            "deadline": pp.get("deadline"),
        })
    return pass_guards


def merge_universities(tile_objects: List[Dict[str, Any]],
                       post_tile_content: Dict[str, Any]
                       ) -> List[Dict[str, Any]]:
    """Merge university tile objects + post-tile Univer content (sheet "Навыки")."""
    unis = []
    tile_uni = [obj for obj in tile_objects if obj.get('type_id') == 104]
    post_uni = post_tile_content.get("universers", [])

    for i, pu in enumerate(post_uni):
        pu = _to_dict(pu)
        tu = tile_uni[i] if i < len(tile_uni) else {}
        unis.append({
            "x": tu.get('x'),
            "y": tu.get('y'),
            "z": tu.get('z'),
            "locality": "море" if tu.get('loc') == 8 else "суша",
            "object": "University",
            "address": pu.get("address"),
            "skills": pu.get("skills", []),
        })
    return unis


def merge_scholars(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge scholar tile objects (sheet "Ученые").

    Each scholar gives ONE of:
      - Spell (scholar_type='spell', spell_id set)
      - Secondary skill (scholar_type='skill', skill_id set)
      - Primary skill +1 (scholar_type='attack_plus_1'/'defense_plus_1'/
                          'power_plus_1'/'knowledge_plus_1')

    PRT stores PrimarySkill as 'A+1' / 'D+1' / 'P+1' / 'K+1' (using aPrSkill
    array = ['A','D','P','K'] concatenated with '+1').
    """
    # Mapping scholar_type → primary skill short code (matches PRT aPrSkill[k]+"+1")
    PRIMARY_SKILL_STR = {
        'attack_plus_1':     'A+1',
        'defense_plus_1':    'D+1',
        'power_plus_1':      'P+1',
        'knowledge_plus_1':  'K+1',
    }

    out = []
    for obj in tile_objects:
        if obj.get('type_id') != 81:
            continue
        scholar_type = obj.get('scholar_type', 'unknown')
        out.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Scholar",
            "scholar_type": scholar_type,
            "skill_id": obj.get('skill_id'),
            "spell_id": obj.get('spell_id'),
            # Primary skill as PRT-style string ('A+1' / 'D+1' / 'P+1' / 'K+1' or None)
            "primary_skill": PRIMARY_SKILL_STR.get(scholar_type),
        })
    return out


def merge_shrines(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge shrine + pyramid tile objects (sheet "Заклинания").

    PRT puts both Shrines (SaveShrine, code 88/89/90) and Pyramids
    (SavePyramid, code 63) into the SAME R_Spell table → both appear
    on the "Заклинания" sheet. Pyramids carry a hardcoded guard
    "Золотой Голем 40, Алмазный Голем 20" + HP 3200.

    Returns records with:
      x, y, z, locality, object (localized name), shrine_type, spell_id, spell_level,
      guard (str, pyramids only), hp (int, pyramids only)
    """
    # Localized shrine names: code 88/89/90 → Святыня магического воплощения/жеста/мысли
    try:
        from localizer import get_localizer
        loc = get_localizer()
        shrine_names = {88: loc.object(88), 89: loc.object(89), 90: loc.object(90)}
        pyramid_name = loc.object(63)  # "Пирамида"
    except Exception:
        shrine_names = {88: "Святыня", 89: "Святыня", 90: "Святыня"}
        pyramid_name = "Пирамида"

    out = []
    for obj in tile_objects:
        type_id = obj.get('type_id')
        # ---- Shrines (88/89/90) ----
        if type_id in (88, 89, 90):
            shrine_type = obj.get('shrine_type', 0) or obj.get('type_id', 0)
            spell_id = obj.get('spell_id', 0)
            # Spell level (1..5) — structural attribute from CreateTblSpell
            try:
                spell_level = loc.spell_level(spell_id) if loc else 0
            except Exception:
                spell_level = 0
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "море" if obj.get('loc') == 8 else "суша",
                "object": shrine_names.get(shrine_type, "Святыня"),
                "shrine_type": shrine_type,
                "spell_id": spell_id,
                "spell_level": spell_level,
                "guard": None,
                "hp": None,
            })
        # ---- Pyramid (63) — same spell_id formula as shrine ----
        elif type_id == 63:
            spell_id = obj.get('spell_id', 0)
            try:
                spell_level = loc.spell_level(spell_id) if loc else 0
            except Exception:
                spell_level = 0
            # PRT hardcodes pyramid guard + HP (MainForm.cs:10460-10461):
            #   r_SpellRow.Guard = "Золотой Голем 40, Алмазный Голем 20"
            #   r_SpellRow.HP = 3200
            # TblMonsters IDs 116 (Gold Golem) + 117 (Diamond Golem).
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "море" if obj.get('loc') == 8 else "суша",
                "object": pyramid_name,
                "shrine_type": 63,
                "spell_id": spell_id,
                "spell_level": spell_level,
                "guard": "Золотой Голем 40, Алмазный Голем 20",
                "hp": 3200,
            })
    return out


def merge_witch_huts(tile_objects: List[Dict[str, Any]],
                     post_tile_content: Optional[Dict[str, Any]] = None,
                     localizer=None,
                     raw: Optional[bytes] = None
                     ) -> List[Dict[str, Any]]:
    """Merge Witch Huts + Universities into Навыки sheet.

    Universal: no hardcoding — uses art_metadata.get_skill_tile_parser to
    determine which types produce rows in Навыки sheet (currently WitchHut=113
    and Univer=104).

    PRT behavior:
      - WitchHut (113): tile-level extraction via formula
        skill_id = ((raw[s+8] & 0xF) << 3) + (raw[s+7] >> 5); 1 skill per row.
      - Univer (104): tile scan creates an empty row (no skill), post-tile
        UniverContent fills 4 skills (at offsets 0, 4, 8, 12 within 16-byte
        record), joined as "skill1, skill2, skill3, skill4".

    Returns records with:
      x, y, z, locality, object (localized), skill (str), skill_id (int or None)
    """
    try:
        from art_metadata import (
            extract_skill_id_from_tile,
            get_skill_post_tile_info,
            is_skill_bearing_tile_type,
            get_skill_tile_parser,
        )
        _has_skill_meta = True
    except Exception:
        _has_skill_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    # Get post-tile univer records (for Universities)
    univer_records = []
    if post_tile_content and isinstance(post_tile_content, dict):
        for u in post_tile_content.get("universers", []):
            univer_records.append(_to_dict(u))

    records: List[Dict[str, Any]] = []
    univer_idx = 0  # position in univer_records (matches tile-scan order)

    for obj in tile_objects:
        type_id = int(obj.get('type_id', 0))
        if not (_has_skill_meta and is_skill_bearing_tile_type(type_id)):
            continue

        # Localized object name (e.g., "Хижина ведьмы", "Университет")
        if localizer:
            object_name = localizer.object(type_id) or f"Object {type_id}"
        else:
            object_name = obj.get('type_name') or f"Object {type_id}"

        locality = "море" if obj.get('loc') == 8 else "суша"

        # Data-driven dispatch via config fields (no type_id hardcoding)
        cfg = get_skill_tile_parser(type_id)
        if cfg is None:
            continue

        # Determine extraction mode from config:
        # - tile_skill_extraction: True (default) → use skill_id_formula at tile level
        # - tile_skill_extraction: False → use post_tile_section + offsets
        use_tile_extraction = cfg.get("tile_skill_extraction", True)

        if use_tile_extraction:
            # WitchHut-style: tile-level extraction via formula
            s = obj.get('offset')
            if s is None or not isinstance(s, int) or raw is None:
                continue
            skill_id = extract_skill_id_from_tile(raw, s, type_id)
            if skill_id is None:
                skill_str = None
            else:
                skill_str = localizer.skill(skill_id) if localizer else f"Skill {skill_id}"
            records.append({
                "x": obj.get('x'), "y": obj.get('y'), "z": obj.get('z'),
                "locality": locality,
                "object": object_name,
                "skill": skill_str,
                "skill_id": skill_id,
            })
        else:
            # Univer-style: post-tile section parsing
            info = get_skill_post_tile_info(type_id)
            if info is None:
                continue
            separator = info.get("skill_join_separator", ", ")
            # Get the corresponding post-tile record by scan order index
            section_key = info.get("post_tile_section")
            if not section_key or univer_idx >= len(univer_records):
                continue
            u_rec = univer_records[univer_idx]
            univer_idx += 1
            # Read skill_ids from the post-tile record
            skills_list = u_rec.get("skills", [])
            skill_names = []
            for sid in skills_list:
                if isinstance(sid, int) and 0 <= sid < 28:
                    skill_names.append(localizer.skill(sid) if localizer else f"Skill {sid}")
            skill_str = separator.join(skill_names) if skill_names else None
            records.append({
                "x": obj.get('x'), "y": obj.get('y'), "z": obj.get('z'),
                "locality": locality,
                "object": object_name,
                "skill": skill_str,
                "skill_id": None,
                "skills_list": skills_list,
            })

    return records


# Backward-compat alias
merge_skills = merge_witch_huts


def merge_refugee_camps(tile_objects: List[Dict[str, Any]],
                        localizer=None,
                        raw: Optional[bytes] = None
                        ) -> List[Dict[str, Any]]:
    """Merge refugee camp tile objects (sheet "Лагеря Беженцев").

    Universal: no hardcoding — uses art_metadata.is_refugee_camp_type to
    determine which type_ids produce rows (currently type 78 in SoD).

    PRT behavior (MainForm.cs SaveCamp, line 10465):
      - Reads monster_id at byte s+2 (u8)
      - Reads count at byte s+6 (u8, max 255)
      - Looks up Monster name and Level from TblMonsters table
      - Each Refugee Camp row contains: X, Y, Z, Locality, Object (localized),
        Monster (name), Number (count), Level (1..7)

    Returns records with:
      x, y, z, locality, object (localized), monster (name), count (int), level (int)
    """
    try:
        from art_metadata import (
            is_refugee_camp_type,
            get_refugee_camp_config,
            get_monster_info,
            get_monster_name,
            get_monster_level,
        )
        _has_meta = True
    except Exception:
        _has_meta = False

    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    records: List[Dict[str, Any]] = []

    for obj in tile_objects:
        type_id = int(obj.get('type_id', 0))
        if not (_has_meta and is_refugee_camp_type(type_id)):
            continue

        # Localized object name (e.g., "Лагерь Беженцов" in ru_sod)
        if localizer:
            object_name = localizer.object(type_id) or f"Object {type_id}"
        else:
            object_name = obj.get('type_name') or f"Object {type_id}"

        locality = "море" if obj.get('loc') == 8 else "суша"

        # Get config (data-driven offsets and lookup table)
        cfg = get_refugee_camp_config(type_id)
        if cfg is None:
            continue

        # Read monster_id from tile bytes (per config: offset 2)
        s = obj.get('offset')
        if s is None or not isinstance(s, int) or raw is None:
            continue

        # Get monster_id and count from tile_object (already parsed by _parse_camp)
        # OR re-read from raw bytes for robustness
        monster_id_off = cfg.get("monster_id_offset", 2)
        count_off = cfg.get("count_offset", 6)
        if s + max(monster_id_off, count_off) + 1 > len(raw):
            continue
        monster_id = raw[s + monster_id_off]
        count = raw[s + count_off]

        # Look up monster name (localizer first — matches PRT binary output)
        # and level/HP metadata from TblMonsters.
        monster_info = get_monster_info(monster_id) if _has_meta else None
        monster_name = localizer.monstr(monster_id) if localizer else None
        if not monster_name:
            if monster_info:
                monster_name = monster_info.get("name", f"M{monster_id}")
            else:
                monster_name = f"M{monster_id}"
        level = int(monster_info.get("level", 0)) if monster_info else 0

        records.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": locality,
            "object": object_name,
            "monster": monster_name,
            "count": count,
            "level": level,
        })

    return records


def merge_learning_stones(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge learning stone tile objects (sheet "Опыт")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Learning Stone",
            "experience": obj.get('experience', 1000),
        }
        for obj in tile_objects if obj.get('type_id') == 100
    ]


def merge_prisons(tile_objects: List[Dict[str, Any]],
                  heroes_parsed: List[Dict[str, Any]] = None
                  ) -> List[Dict[str, Any]]:
    """Merge prison tile objects (sheet "Тюрьмы").

    Each prison contains a hero (with hero_id pointing into the heroes list).
    PRT copies hero fields into the prison row via GetPrisonHero (MainForm.cs:7983).

    Output fields (matching PRT R_PrisonRow):
      hero_name, level, experience, primary_skill (e.g. "A2-D2-P1-K1"),
      secondary_skill (e.g. "Б.Лидерство, Б.Тактика"),
      spell (single starting spell name, or None),
      monster (e.g. "Копейщик 12, Арбалетчик 7"),
      machine (war machine name, or None),
      book ("Да" if spell book present, else None),
      mp (movement points, e.g. 1560)
    """
    try:
        from localizer import get_localizer
        loc = get_localizer()
    except Exception:
        loc = None

    # Build hero_id → hero dict (heroes list is in order — index = ID)
    heroes_by_id = {i: h for i, h in enumerate(heroes_parsed or [])}

    prisons = []
    for obj in tile_objects:
        if obj.get('type_id') != 62:
            continue
        hero_id = obj.get('hero_id', 0)
        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Prison",
            "hero_id": hero_id,
        }
        if hero_id in heroes_by_id:
            hero = heroes_by_id[hero_id]
            f = hero.get('fields', {})

            # Hero name
            rec["hero_name"] = f.get('name', '?')

            # Level + Experience
            rec["hero_level"] = f.get('level', 0)
            rec["hero_experience"] = f.get('experience', 0)

            # Primary skill: "A2-D2-P1-K1" (matches PRT aPrSkill[k] + value)
            atk = f.get('attack', 0) or 0
            dfn = f.get('defense', 0) or 0
            pwr = f.get('power', 0) or 0
            knw = f.get('knowledge', 0) or 0
            rec["primary_skill"] = f"A{atk}-D{dfn}-P{pwr}-K{knw}"

            # Secondary skill: "Б.Лидерство, Б.Тактика"
            # PRT logic (MainForm.cs:7807-7820):
            #   For each of 28 skills (l=0..27):
            #     if skill_levels[l] != 0:
            #       array[skill_slots[l] - 1] = aLevelSkill[level-1] + TblSecondarySkill[l]
            #   Then join non-null entries with ", "
            # aLevelSkill = ["Б.", "П.", "Э."] (Basic/Advanced/Expert)
            skill_levels = f.get('skill_levels', []) or []
            skill_slots = f.get('skill_slots', []) or []
            LEVEL_PREFIX = ["Б.", "П.", "Э."]  # Basic / Advanced / Expert (SoD_Ru)
            array = [None] * 8
            for l in range(min(28, len(skill_levels), len(skill_slots))):
                lvl = skill_levels[l] if l < len(skill_levels) else 0
                slot = skill_slots[l] if l < len(skill_slots) else 0
                if lvl != 0 and 1 <= slot <= 8:
                    level_prefix = LEVEL_PREFIX[lvl - 1] if 1 <= lvl <= 3 else f"L{lvl}"
                    skill_name = loc.skill(l) if loc else f"Skill{l}"
                    array[slot - 1] = level_prefix + skill_name
            sec_parts = [s for s in array if s]
            rec["secondary_skill"] = ", ".join(sec_parts) if sec_parts else None

            # Spell (single starting spell — PRT uses r_HeroesRow.Spell)
            # TODO: PRT reads a specific spell field; for now use spells_book if exactly 1 spell
            # For now, leave as None — will be addressed in heroes sheet
            sb = f.get('spells_book', []) or []
            # spells_book is a 70-bit array; find first set bit
            spell_indices = [i for i, b in enumerate(sb) if b]
            if spell_indices:
                # PRT shows only ONE spell (the starting spell) — first one
                first_spell_id = spell_indices[0]
                rec["spell"] = loc.spell(first_spell_id) if loc else str(first_spell_id)
            else:
                rec["spell"] = None

            # Monster: "Копейщик 12, Арбалетчик 7"
            # PRT (MainForm.cs:7783-7795): for each of 7 slots, if monster_id < 0xFF:
            #   text += ", " + TblMonsters[name] + " " + count
            at = f.get('army_types', []) or []
            ac = f.get('army_counts', []) or []
            mon_parts = []
            for j in range(min(7, len(at))):
                mid = at[j] if j < len(at) else -1
                cnt = ac[j] if j < len(ac) else 0
                if isinstance(mid, int) and 0 <= mid < 0xFF and cnt:
                    mname = loc.monstr(mid) if loc else str(mid)
                    mon_parts.append(f"{mname} {cnt}")
            rec["monster"] = ", ".join(mon_parts) if mon_parts else None
            # Store raw army data for excel_calculations.prison_sum_hp (Excel-only).
            rec["army_types"] = at
            rec["army_counts"] = ac

            # Machine (war machine name)
            # war_machines is list of dicts {slot_id, machine_id, machine_name, ...}
            # PRT (MainForm.cs:7847-7851): for doll slots with artifact_id 4,5,6:
            #   text5 += ", " + TblArt.Rows.Find(num6)[1]
            # Use loc.art(machine_id) for localized name (Catapult=3, Ballista=4,
            # Ammo Cart=5, First Aid Tent=6)
            wm = f.get('war_machines', []) or []
            machine_parts = []
            for m in wm:
                if isinstance(m, dict):
                    mid = m.get('machine_id', -1)
                    if isinstance(mid, int) and mid >= 0:
                        mname = loc.art(mid) if loc else m.get('machine_name', str(mid))
                        if mname:
                            machine_parts.append(mname)
            rec["machine"] = ", ".join(machine_parts) if machine_parts else None

            # Book: "Да" if spell_book present (PRT r_HeroesRow.Book)
            # spell_book field is truthy if hero has spell book
            rec["book"] = "Да" if f.get('spell_book') else None

            # MP (movement points)
            rec["mp"] = f.get('movement_total', 0)

        prisons.append(rec)
    return prisons


def merge_keymaster_tents(tile_objects: List[Dict[str, Any]],
                           localizer=None
                           ) -> List[Dict[str, Any]]:
    """Merge keymaster tent tile objects (sheet "Топология").

    PRT SaveTent (line 9778): creates 1 row per tent with:
      X, Y, Z, Locality, Object (localized name), Color (from aTent[])
    """
    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    out = []
    for obj in tile_objects:
        if obj.get('type_id') != 10:
            continue
        object_name = localizer.object(10) if localizer else "Палатка ключника"
        color_id = obj.get('color_id', 0)
        color_name = localizer.tent(color_id) if localizer else f"Color {color_id}"
        out.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": object_name,
            "color": color_name,
        })
    return out


def merge_monoliths(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Optional[Dict[str, Any]] = None,
                    localizer=None
                    ) -> List[Dict[str, Any]]:
    """Merge monolith tile objects (sheet "Топология").

    PRT SaveMonolith (line 9766): creates 1 row per monolith from tile scan only.
    Post-tile monolith data is used for pairing computation, NOT for additional rows.
    Type column = monolith subtype (decmp[s+2] as string).
    """
    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    out = []
    for obj in tile_objects:
        type_id = obj.get('type_id')
        if type_id not in (43, 44, 45):
            continue
        object_name = localizer.object(type_id) if localizer else f"Monolith {type_id}"
        subtype = obj.get('monolith_subtype', 0)
        out.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": object_name,
            "type": str(subtype) if subtype is not None else None,
        })
    # REMOVED: post-tile one_way_groups, two_way_groups, whirlpools appending
    # These were creating 26 duplicate rows with wrong coordinates.
    # PRT uses post-tile data ONLY for pair computation, not for new rows.
    return out


def merge_subterranean_gates(tile_objects: List[Dict[str, Any]],
                             post_tile_content: Optional[Dict[str, Any]] = None,
                             localizer=None
                             ) -> List[Dict[str, Any]]:
    """Merge subterranean gate tile objects (sheet "Топология").

    PRT SaveTopologyObj (line 9790): creates 1 row per gate from tile scan.
    Pair info is set on EXISTING rows from post-tile pairing computation.
    """
    if localizer is None:
        try:
            from localizer import get_localizer
            localizer = get_localizer()
        except Exception:
            localizer = None

    out = []
    for obj in tile_objects:
        if obj.get('type_id') != 103:
            continue
        object_name = localizer.object(103) if localizer else "Врата подземного мира"
        out.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": object_name,
        })

    # Set Pair field on existing rows from post-tile sub_ter_gates pairing.
    # PRT GetPairSubterraneanGate (line 9335-9388):
    #   pair_ids[i] = index of gate that gate[i] is paired with (both directions stored)
    #   Algorithm: for each k, search for num3 > k where pair_ids[num3] == k.
    #   When found: set Pair=k on BOTH gate k and gate pair_ids[k].
    #   This gives paired gates the SAME pair index (the lower gate index).
    if post_tile_content and isinstance(post_tile_content, dict):
        stg = post_tile_content.get("sub_ter_gates")
        if stg and isinstance(stg, dict):
            stg_dict = stg if isinstance(stg, dict) else {}
            gates = stg_dict.get("gates", [])
            pair_ids = stg_dict.get("pair_ids", [])
            n = len(gates)

            # Compute pair values: pair_values[i] = Pair for gate i (or None)
            pair_values = [None] * n
            for k in range(n):
                # Search for num3 > k where pair_ids[num3] == k
                for num3 in range(n - 1, k, -1):
                    if num3 < len(pair_ids) and pair_ids[num3] == k:
                        # Found: gate k and gate pair_ids[k] are paired
                        j = pair_ids[k] if k < len(pair_ids) and pair_ids[k] >= 0 else None
                        pair_values[k] = k
                        if j is not None and 0 <= j < n:
                            pair_values[j] = k
                        break

            # Apply pair values to existing rows by matching coords
            for i, gate in enumerate(gates):
                gate = _to_dict(gate)
                gx = gate.get('x')
                gy = gate.get('y')
                gz = gate.get('z')
                pair_val = pair_values[i] if i < len(pair_values) else None
                if pair_val is not None:
                    for row in out:
                        if row.get('x') == gx and row.get('y') == gy and row.get('z') == gz:
                            row['pair'] = pair_val
                            break

    return out


def merge_market(tile_objects: List[Dict[str, Any]],
                 post_tile_content: Optional[Dict[str, Any]] = None,
                 localizer=None,
                 raw: Optional[bytes] = None,
                 map_start_info: Optional[Dict[str, Any]] = None,
                 towns_parsed: Optional[List[Dict[str, Any]]] = None,
                 ) -> List[Dict[str, Any]]:
    """Merge Black Market tile objects + post-tile Market content (sheet "Рынки").

    PRT logic:
      - SaveMarket (MainForm.cs:9994): for each Black Market tile (code 7),
        creates R_Market row with X/Y/Z/Locality + Object (from TblObject).
      - GetArtMerchants (MainForm.cs:7684): if any town has Code in {2,5,8}
        AND Slot=1, parse 7 slots from `current_state + 49`. Each non-empty
        slot creates a row with Object="Торговцы Артефактами", no X/Y/Z.
      - GetMarketContent (MainForm.cs:8042): for each TblMarket row, parse
        28 bytes (7 × 4-byte slots). Each non-empty slot creates a row with
        Slot=1..7, artifact name/class/relic-C/cost from TblArt.

    Output order matches PRT 0000.xlsx: Art Merchants rows first (no X/Y/Z),
    then Black Market rows (3 markets × 7 slots = 21).
    """
    # PRT DEFAULT arrays (matching PRT 0000.xlsx output via ObjNameDefault mode).
    _BLACK_MARKET_NAME = "Черный рынок"
    _ART_MERCHANTS_NAME = "Торговцы Артефактами"

    # Lazy-load art_metadata helpers (used for class/relic/cost lookup by artifact_id).
    try:
        from art_metadata import (
            get_artifact_class_id, get_artifact_relic_c_id,
            get_artifact_cost, get_artifact_name,
        )
    except Exception:
        get_artifact_class_id = get_artifact_relic_c_id = get_artifact_cost = None
        get_artifact_name = None

    # Helper to look up artifact info.
    def _art_info(aid):
        """Return dict {name, class, relic_c, cost} for the given artifact ID.

        Uses tbl_art_mapping.json (PRT DEFAULT TblArt source) for names — NOT
        the localizer — so capitalization matches PRT 0000.xlsx (e.g.
        "Ожерелье стремительности", not "Ожерелье Стремительности").
        """
        if get_artifact_name is None:
            return {}
        info = {}
        info["name"] = get_artifact_name(aid, localizer=None)  # PRT-style capitalization
        cls_id = get_artifact_class_id(aid)
        if localizer is not None:
            info["class"] = localizer.art_class(cls_id)
        relic_id = get_artifact_relic_c_id(aid)
        if relic_id is not None:
            info["relic_c"] = get_artifact_name(relic_id, localizer=None)
        info["cost"] = get_artifact_cost(aid)
        return info

    records: List[Dict[str, Any]] = []

    # ── A. Art Merchants (PRT GetArtMerchants, MainForm.cs:7684) ──
    # Condition: any town with Code in {2, 5, 8} AND Slot=1.
    # PRT's "Slot='1'" matches towns where Slot field = 1 (built on day 1).
    # OUR's towns_parsed has fields["type"] (= PRT's Code) — we don't have a
    # "Slot" field, so we approximate by checking any town with Code in {2,5,8}.
    has_art_merchants_town = False
    if towns_parsed:
        for t in towns_parsed:
            t_type = t.get("type") if t.get("type") is not None else \
                     t.get("fields", {}).get("type", -1)
            if t_type in (2, 5, 8):
                has_art_merchants_town = True
                break

    if has_art_merchants_town and raw is not None and post_tile_content:
        offsets = post_tile_content.get("offsets", {}) if isinstance(post_tile_content, dict) else {}
        current_state = offsets.get("current_state", 0) if isinstance(offsets, dict) else 0
        if current_state > 0:
            am_start = current_state + 49  # PRT GetArtMerchants(currentState + 49)
            # PRT iterates 7 × 4 bytes; artifact_id is the first byte of each slot.
            for i in range(7):
                off = am_start + i * 4
                if off >= len(raw):
                    break
                aid = raw[off]
                if aid < 0xFF:
                    info = _art_info(aid)
                    records.append({
                        "x": None, "y": None, "z": None,
                        "locality": None,
                        "object": _ART_MERCHANTS_NAME,
                        "slot": i + 1,
                        "artifact_id": aid,
                        "artifact_name": info.get("name"),
                        "class": info.get("class"),
                        "relic_c": info.get("relic_c"),
                        "cost": info.get("cost"),
                    })

    # ── B. Black Markets (PRT SaveMarket + GetMarketContent, MainForm.cs:8042) ──
    # Get Black Market section offset from map_start_info:
    # - PRT reads byte at `after_variable_structures` (= num3 = black_market_count)
    # - Black market data starts at `after_variable_structures + 1`
    bm_count = 0
    bm_data_start = 0
    if isinstance(map_start_info, dict):
        bm_count = map_start_info.get("black_market_count", 0)
        avs = map_start_info.get("after_variable_structures", 0)
        if avs > 0:
            bm_data_start = avs + 1  # skip count byte

    # Get Black Market tile objects (code 7) — in tile-scan order.
    tile_markets = [obj for obj in tile_objects if obj.get('type_id') == 7]

    # PRT iterates TblMarket.Rows in order they were added by SaveMarket (= tile-scan order).
    # For each market, parse 28 bytes (7 × 4-byte slots).
    for i in range(min(bm_count, len(tile_markets))):
        if bm_data_start <= 0 or bm_data_start + i * 28 + 28 > len(raw):
            break
        market_addr = bm_data_start + i * 28
        tile = tile_markets[i]
        x = tile.get('x')
        y = tile.get('y')
        z = tile.get('z')
        loc = tile.get('loc', 0)
        # PRT: read 7 slots (artifact_id at byte[market_addr + j*4] for j=0..6).
        # Slot numbering: 1..7 (slot 1 = byte[0], slot 2 = byte[4], ..., slot 7 = byte[24]).
        for slot_idx in range(7):
            off = market_addr + slot_idx * 4
            if off >= len(raw):
                break
            aid = raw[off]
            if aid < 0xFF:
                info = _art_info(aid)
                records.append({
                    "x": x, "y": y, "z": z,
                    "locality": "море" if loc == 8 else "суша",
                    "object": _BLACK_MARKET_NAME,
                    "slot": slot_idx + 1,
                    "artifact_id": aid,
                    "artifact_name": info.get("name"),
                    "class": info.get("class"),
                    "relic_c": info.get("relic_c"),
                    "cost": info.get("cost"),
                })

    return records


def merge_generic_objects(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge generic object tile objects (sheet "Объекты").

    Dispatched for codes: 2 (Altar of Sacrifice), 35 (Hill Fort),
    95 (Tavern), 102 (Tree of Knowledge), 213 (Freelancer's Guild).

    Object name comes from localizer.object(type_id) — same as PRT
    TblObject.Select("Code='" + decmp[s] + "'")[0][1].

    Payment is set only for code 102 (Tree of Knowledge):
        "2000" (2000 gold) or "G10" (10 gems) or None.
    """
    try:
        from localizer import get_localizer
        loc = get_localizer()
    except Exception:
        loc = None

    out = []
    for obj in tile_objects:
        type_id = obj.get('type_id')
        if type_id not in (2, 35, 95, 102, 213):
            continue
        # Localized object name from localizer (matches PRT TblObject lookup by code)
        object_name = loc.object(type_id) if loc else f"Object {type_id}"
        out.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": object_name,
            "type_id": type_id,
            "payment": obj.get('payment'),  # "2000" / "G10" / None (only for code 102)
        })
    return out


def merge_heroes_on_map(tile_objects: List[Dict[str, Any]],
                        heroes_parsed: List[Dict[str, Any]] = None
                        ) -> List[Dict[str, Any]]:
    """Merge hero-on-map tile objects (sheet "Герои" — heroes on map)."""
    out = []
    heroes_by_id = {i: h for i, h in enumerate(heroes_parsed or [])}
    for obj in tile_objects:
        if obj.get('type_id') != 34:
            continue
        hero_id = obj.get('hero_id', 0)
        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Hero on Map",
            "hero_id": hero_id,
            "on_object": obj.get('on_object', False),
        }
        if hero_id in heroes_by_id:
            hero = heroes_by_id[hero_id]
            f = hero.get('fields', {})
            rec["hero_name"] = f.get('name', '?')
            rec["hero_level"] = f.get('level', 0)
            rec["hero_experience"] = f.get('experience', 0)
        out.append(rec)
    return out


# ============================================================================
# Master: merge_all_objects — produce all standardized tables
# ============================================================================

def merge_all(tile_objects: List[Dict[str, Any]],
              post_tile_content: Dict[str, Any],
              heroes_parsed: List[Dict[str, Any]] = None,
              obj_dict: Optional[Dict] = None,
              localizer=None,
              raw: Optional[bytes] = None,
              map_start_info: Optional[Dict[str, Any]] = None,
              towns_parsed: Optional[List[Dict[str, Any]]] = None,
              ) -> Dict[str, List[Dict[str, Any]]]:
    """Produce all standardized merged tables in one call.

    Returns a dict with keys matching sheet names (in Russian for clarity):
      Артефакты, Монстры, Банки, События_Ящики, Ученые, Ресурсы, Сундуки,
      Заклинания, Навыки (witch huts), Лагеря_Беженцев, Рынки, Провидцы,
      Тюрьмы, Объекты, Топология, Герои_на_карте, Гарнизоны, Университеты,
      Learning_Stones

    Each value is a list of dicts (one per row in the output xlsx).

    Args:
        tile_objects: list of tile-scan dicts (from scan_tiles)
        post_tile_content: dict from parse_all_post_tile_sections
        heroes_parsed: optional list of parsed hero dicts (for prison/hero tables)
        obj_dict: optional object_types_dictionary lookup
        localizer: optional Localizer instance (used by merge_artifacts for
            localized names of "Объект", "Класс", "Реликт-С" columns)
        raw: optional decompressed save bytes (used by merge_artifacts to
            extract artifact_id from tile bytes via PRT Save* logic)
    """
    return {
        "Артефакты": merge_artifacts(tile_objects, post_tile_content, obj_dict,
                                      localizer=localizer, raw=raw),
        "Монстры": merge_monsters(tile_objects, post_tile_content, obj_dict,
                                    localizer=localizer, raw=raw),
        "Банки": merge_banks(tile_objects, post_tile_content,
                              localizer=localizer, raw=raw),
        "События_Ящики_Пандоры": merge_event_boxes(tile_objects, post_tile_content,
                                                        localizer=localizer, raw=raw),
        "Ученые": merge_scholars(tile_objects),
        "Ресурсы": merge_resources(tile_objects, post_tile_content,
                                    localizer=localizer, raw=raw),
        "Сундуки": merge_chests(tile_objects, obj_dict),
        "Заклинания_Святилища": merge_shrines(tile_objects),
        "Ведьмины_Хижины": merge_witch_huts(tile_objects, post_tile_content,
                                              localizer=localizer, raw=raw),
        "Лагеря_Беженцев": merge_refugee_camps(tile_objects, localizer=localizer, raw=raw),
        "Рынки": merge_market(tile_objects, post_tile_content,
                              localizer=localizer, raw=raw,
                              map_start_info=map_start_info,
                              towns_parsed=towns_parsed),
        "Провидцы": merge_seer_huts(tile_objects, post_tile_content, localizer=localizer),
        "Border_Guards": merge_pass_guards(tile_objects, post_tile_content),
        "Тюрьмы": merge_prisons(tile_objects, heroes_parsed),
        "Объекты": merge_generic_objects(tile_objects),
        "Университеты": merge_universities(tile_objects, post_tile_content),
        "Learning_Stones": merge_learning_stones(tile_objects),
        "Топология_Палатки": merge_keymaster_tents(tile_objects, localizer=localizer),
        "Топология_Монолиты": merge_monoliths(tile_objects, post_tile_content,
                                                localizer=localizer),
        "Топология_Подземные_Врата": merge_subterranean_gates(tile_objects, post_tile_content,
                                                                localizer=localizer),
        "Герои_на_карте": merge_heroes_on_map(tile_objects, heroes_parsed),
        "Гарнизоны": merge_garrisons(tile_objects, post_tile_content),
    }
