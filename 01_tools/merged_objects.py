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
            "hp": None,  # ∑ HP — calc, not computed (in calculations_todo.txt)
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

    return artifacts


def merge_monsters(tile_objects: List[Dict[str, Any]],
                   post_tile_content: Dict[str, Any],
                   obj_dict: Optional[Dict] = None
                   ) -> List[Dict[str, Any]]:
    """Merge monster tile objects with Monstr content (sheet "Монстры").

    Returns records with:
      x, y, z, locality, monster_id, count, mood, level, hp, grows,
      artifact_id, gold, resources
    """
    monsters = []
    tile_map = _build_tile_object_map(tile_objects)

    # From tile scan: monsters have type_id 54 (Monster)
    for obj in tile_objects:
        if obj.get('type_id') != 54:
            continue
        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Monster",
            "monster_type": obj.get('monster_type', 0),
            "count": obj.get('count', 0),
            "mood": obj.get('mood', 0),
            "grows": obj.get('grows', False),
            "has_artifact": obj.get('has_artifact', False),
        }
        monsters.append(rec)

    # From post-tile Monstr content (treasure)
    monstr_records = post_tile_content.get("monstr_records", [])
    for mr in monstr_records:
        mr = _to_dict(mr)
        monsters.append({
            "address": mr.get("address"),
            "resources": mr.get("resources", {}),
            "gold": mr.get("gold", 0),
            "artifact_id": mr.get("artifact_id", -1),
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
        linked_guard = None
        if res_info.get("has_guard") and res_info.get("art_res_num", 0) < len(art_res_records):
            ar = art_res_records[res_info["art_res_num"]]
            # Verify this ArtRes is a resource (Type=1), not artifact (Type=0)
            if ar.get("art_type", 0) == 1:
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
            "hp": None,  # ∑ HP — calc, not computed (in calculations_todo.txt)
        }
        resources.append(rec)

    return resources


def merge_event_boxes(tile_objects: List[Dict[str, Any]],
                      post_tile_content: Dict[str, Any]
                      ) -> List[Dict[str, Any]]:
    """Merge event tile objects + Pandora boxes with EventBox content
    (sheet "События и Ящики Пандоры").

    Returns records with ALL fields from EventBoxContent:
      x, y, z, locality, object, guard, experience, mana, morale, luck,
      resources, primary_skills, secondary_skills, artifacts, spells,
      monsters, apply
    """
    tile_map = _build_tile_object_map(tile_objects)
    event_boxes = []

    # EventBox records from post_tile_content have the content but no coords
    # We match them to tile objects by their address (offset in save)
    ebs = post_tile_content.get("event_boxes", [])
    for eb in ebs:
        eb = _to_dict(eb)
        rec = {
            "address": eb.get("address"),
            "has_guard": eb.get("has_guard", False),
            "guard": _serialize_guard(eb.get("guard", [])),
            "experience": eb.get("experience", 0),
            "mana": eb.get("mana", 0),
            "morale": eb.get("morale", 0),
            "luck": eb.get("luck", 0),
            "resources": eb.get("resources", {}),
            "primary_skills": eb.get("primary_skills", {}),
            "secondary_skills": eb.get("secondary_skills", []),
            "artifacts": eb.get("artifacts", []),
            "spells": eb.get("spells", []),
            "monsters": eb.get("monsters", []),
            "apply": eb.get("apply", (0, 0, 0)),
        }
        event_boxes.append(rec)
    return event_boxes


def merge_banks(tile_objects: List[Dict[str, Any]],
                post_tile_content: Dict[str, Any]
                ) -> List[Dict[str, Any]]:
    """Merge bank tile objects + post-tile Bank content (sheet "Банки").

    Returns records with:
      x, y, z, locality, object, guard, monster_reward, gold, resources, artifact_ids
    """
    banks = []
    # Tile scan: banks have type_id 16, 24, 25, 84, 85
    tile_banks = [obj for obj in tile_objects
                  if obj.get('type_id') in (16, 24, 25, 84, 85)]
    # Post-tile content
    post_banks = post_tile_content.get("banks", [])

    # Zip them up — both should be in same order (bank_num)
    for i, pb in enumerate(post_banks):
        pb = _to_dict(pb)
        tb = tile_banks[i] if i < len(tile_banks) else {}
        banks.append({
            "x": tb.get('x'),
            "y": tb.get('y'),
            "z": tb.get('z'),
            "locality": "море" if tb.get('loc') == 8 else "суша",
            "object": "Bank",
            "address": pb.get("address"),
            "is_artifact_bank": pb.get("is_artifact_bank", False),
            "guard": _serialize_guard(pb.get("guard", [])),
            "resources": pb.get("resources", {}),
            "gold": pb.get("gold", 0),
            "monster_reward_id": pb.get("monster_reward_id", -1),
            "monster_reward_count": pb.get("monster_reward_count", 0),
            "artifact_ids": pb.get("artifact_ids", []),
        })
    return banks


def merge_garrisons(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any]
                    ) -> List[Dict[str, Any]]:
    """Merge garrison tile objects + post-tile Garrison content (sheet "Garrisons").

    Returns records with: x, y, z, locality, object, guard, color, can_take
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
            "address": pg.get("address"),
            "guard": _serialize_guard(pg.get("guard", [])),
            "color": pg.get("color", -1),
            "can_take": pg.get("can_take", 0),
        })
    return garrisons


def merge_seer_huts(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any]
                    ) -> List[Dict[str, Any]]:
    """Merge seer hut tile objects + post-tile SeerHut content (sheet "Провидцы").

    Returns records with:
      x, y, z, locality, object, num (quest ID), mission_type, mission,
      deadline, reward_type, reward
    """
    seer_huts = []
    tile_seer = [obj for obj in tile_objects if obj.get('type_id') == 83]
    post_seer = post_tile_content.get("seer_huts", [])

    for i, ps in enumerate(post_seer):
        ps = _to_dict(ps)
        ts = tile_seer[i] if i < len(tile_seer) else {}
        seer_huts.append({
            "x": ts.get('x'),
            "y": ts.get('y'),
            "z": ts.get('z'),
            "locality": "море" if ts.get('loc') == 8 else "суша",
            "object": "Seer Hut",
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
    """Merge border guard tile objects + post-tile PassGuard content."""
    pass_guards = []
    tile_pg = [obj for obj in tile_objects if obj.get('type_id') == 215]
    post_pg = post_tile_content.get("pass_guards", [])

    for i, pp in enumerate(post_pg):
        pp = _to_dict(pp)
        tp = tile_pg[i] if i < len(tile_pg) else {}
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
    """Merge shrine tile objects (sheet "Заклинания" — shrines only).

    Returns records with:
      x, y, z, locality, object (localized shrine name), shrine_type, spell_id, spell_level
    """
    # Localized shrine names: code 88/89/90 → Святыня магического воплощения/жеста/мысли
    try:
        from localizer import get_localizer
        loc = get_localizer()
        shrine_names = {88: loc.object(88), 89: loc.object(89), 90: loc.object(90)}
    except Exception:
        shrine_names = {88: "Святыня", 89: "Святыня", 90: "Святыня"}

    out = []
    for obj in tile_objects:
        if obj.get('type_id') not in (88, 89, 90):
            continue
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

        # Look up monster name and level from TblMonsters (PRT-exact names)
        monster_info = get_monster_info(monster_id)
        if monster_info:
            # PRT-style name (e.g., "Скелет", "Крестоносец" — matches PRT 0000.xlsx)
            monster_name = monster_info.get("name", f"M{monster_id}")
            level = int(monster_info.get("level", 0))
        else:
            # Fallback to localizer for name (will differ from PRT but still readable)
            monster_name = localizer.monstr(monster_id) if localizer else f"M{monster_id}"
            level = 0

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


def merge_keymaster_tents(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge keymaster tent tile objects (sheet "Топология")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Keymaster Tent",
            "color_id": obj.get('color_id', 0),
            "color_name": obj.get('color_name', '?'),
        }
        for obj in tile_objects if obj.get('type_id') == 10
    ]


def merge_monoliths(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any]) -> List[Dict[str, Any]]:
    """Merge monolith tile objects + post-tile MonolithInfo (sheet "Топология")."""
    out = []
    # Tile scan monoliths: types 43, 44, 45
    for obj in tile_objects:
        if obj.get('type_id') in (43, 44, 45):
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "море" if obj.get('loc') == 8 else "суша",
                "object": "Monolith",
                "monolith_subtype": obj.get('monolith_subtype', 0),
            })
    # Post-tile monoliths (with pairing info)
    monos = post_tile_content.get("monoliths")
    if monos:
        monos = _to_dict(monos)
        # Add one_way_groups + two_way_groups + whirlpools
        for group_idx, group in enumerate(monos.get("one_way_groups", [])):
            for entry in group:
                # entry is a tuple (x, y, z, bits) — not a dataclass
                if isinstance(entry, (tuple, list)) and len(entry) >= 3:
                    x, y, z = entry[0], entry[1], entry[2]
                else:
                    x = y = z = None
                out.append({
                    "object": "One-Way Monolith",
                    "group": group_idx,
                    "x": x, "y": y, "z": z,
                })
        for group_idx, group in enumerate(monos.get("two_way_groups", [])):
            for entry in group:
                if isinstance(entry, (tuple, list)) and len(entry) >= 3:
                    x, y, z = entry[0], entry[1], entry[2]
                else:
                    x = y = z = None
                out.append({
                    "object": "Two-Way Monolith",
                    "group": group_idx,
                    "x": x, "y": y, "z": z,
                })
        for entry in monos.get("whirlpools", []):
            if isinstance(entry, (tuple, list)) and len(entry) >= 3:
                x, y, z = entry[0], entry[1], entry[2]
            else:
                x = y = z = None
            out.append({
                "object": "Whirlpool",
                "x": x, "y": y, "z": z,
            })
    return out


def merge_subterranean_gates(tile_objects: List[Dict[str, Any]],
                             post_tile_content: Dict[str, Any]
                             ) -> List[Dict[str, Any]]:
    """Merge subterranean gate tile objects + post-tile pairing (sheet "Топология")."""
    out = []
    # Tile scan: type 103 (topology_object)
    for obj in tile_objects:
        if obj.get('type_id') == 103:
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "море" if obj.get('loc') == 8 else "суша",
                "object": "Subterranean Gate",
            })
    # Post-tile pairing
    stg = post_tile_content.get("sub_ter_gates")
    if stg and isinstance(stg, dict):
        gates = stg.get("gates", [])
        pair_ids = stg.get("pair_ids", [])
        for i, gate in enumerate(gates):
            gate = _to_dict(gate)
            pair_id = pair_ids[i] if i < len(pair_ids) else -1
            out.append({
                "object": "Subterranean Gate (paired)",
                "x": gate.get('x'),
                "y": gate.get('y'),
                "z": gate.get('z'),
                "pair_id": pair_id,
            })
    return out


def merge_market(tile_objects: List[Dict[str, Any]],
                 post_tile_content: Dict[str, Any]
                 ) -> List[Dict[str, Any]]:
    """Merge black market tile objects + post-tile Market content (sheet "Рынки")."""
    markets = []
    tile_markets = [obj for obj in tile_objects if obj.get('type_id') == 7]
    # Post-tile content might have market data (we have parse_market_content,
    # but it's not currently in post_tile_content output unless we add it)
    # For now, just use tile objects
    for obj in tile_markets:
        markets.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "море" if obj.get('loc') == 8 else "суша",
            "object": "Black Market",
        })
    return markets


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
              raw: Optional[bytes] = None
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
        "Монстры": merge_monsters(tile_objects, post_tile_content, obj_dict),
        "Банки": merge_banks(tile_objects, post_tile_content),
        "События_Ящики_Пандоры": merge_event_boxes(tile_objects, post_tile_content),
        "Ученые": merge_scholars(tile_objects),
        "Ресурсы": merge_resources(tile_objects, post_tile_content,
                                    localizer=localizer, raw=raw),
        "Сундуки": merge_chests(tile_objects, obj_dict),
        "Заклинания_Святилища": merge_shrines(tile_objects),
        "Ведьмины_Хижины": merge_witch_huts(tile_objects, post_tile_content,
                                              localizer=localizer, raw=raw),
        "Лагеря_Беженцев": merge_refugee_camps(tile_objects, localizer=localizer, raw=raw),
        "Рынки": merge_market(tile_objects, post_tile_content),
        "Провидцы": merge_seer_huts(tile_objects, post_tile_content),
        "Border_Guards": merge_pass_guards(tile_objects, post_tile_content),
        "Тюрьмы": merge_prisons(tile_objects, heroes_parsed),
        "Объекты": merge_generic_objects(tile_objects),
        "Университеты": merge_universities(tile_objects, post_tile_content),
        "Learning_Stones": merge_learning_stones(tile_objects),
        "Топология_Палатки": merge_keymaster_tents(tile_objects),
        "Топология_Монолиты": merge_monoliths(tile_objects, post_tile_content),
        "Топология_Подземные_Врата": merge_subterranean_gates(tile_objects, post_tile_content),
        "Герои_на_карте": merge_heroes_on_map(tile_objects, heroes_parsed),
        "Гарнизоны": merge_garrisons(tile_objects, post_tile_content),
    }
