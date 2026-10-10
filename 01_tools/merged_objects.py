#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
merged_objects.py — Merge tile-scan results with post-tile content.

This module produces PRT-like "raw" tables (Артефакты, Монстры, Банки, etc.)
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
# Merge functions — one per PRT sheet
# ============================================================================

# Resource name lookup (PRT aResource[])
A_RESOURCE = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]


def _serialize_guard(guard) -> List[Dict[str, Any]]:
    """Convert ArmySlot list (dataclasses OR dicts) to plain dicts.

    PRT post_tile_content stores guard as List[ArmySlot] (dataclass).
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
                    obj_dict: Optional[Dict] = None
                    ) -> List[Dict[str, Any]]:
    """Merge artifacts tile objects with ArtRes content (PRT sheet "Артефакты").

    Returns a list of records with fields:
      x, y, z, locality, object, slot, artifact_id, artifact_name, class, relic,
      gold, resource, guard, hp

    PRT row example (139 rows):
      (56, 8, 0, 'суша', 'Артефакт', None, 'Кольцо драгоценных камней', 'Большой',
       'Рог изобилия', None, None, None, None)
    """
    tile_map = _build_tile_object_map(tile_objects)
    artifacts = []

    # From tile scan: artifacts have type_id 5 (Artifact)
    for obj in tile_objects:
        if obj.get('type_id') != 5:
            continue
        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Artifact",
            "artifact_id": obj.get('artifact_id', 0),
            "has_guard": obj.get('has_guard', False),
        }
        artifacts.append(rec)

    # From post-tile content: ArtRes records have guard details
    art_res = post_tile_content.get("art_res", [])
    for ar in art_res:
        ar = _to_dict(ar)  # convert dataclass to dict
        artifacts.append({
            "address": ar.get("address"),
            "art_type": ar.get("art_type"),  # 0=Art, 1=Resource, 2=Spell
            "has_guard": ar.get("has_guard"),
            "guard": ar.get("guard", []),
        })

    return artifacts


def merge_monsters(tile_objects: List[Dict[str, Any]],
                   post_tile_content: Dict[str, Any],
                   obj_dict: Optional[Dict] = None
                   ) -> List[Dict[str, Any]]:
    """Merge monster tile objects with Monstr content (PRT sheet "Монстры").

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
            "locality": "underground" if obj.get('loc') == 8 else "surface",
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
    """Merge chest tile objects (PRT sheet "Сундуки").

    Returns records with:
      x, y, z, locality, object, gold, experience, artifact_id, has_artifact
    """
    chests = []
    for obj in tile_objects:
        if obj.get('type_id') not in (101, 82):  # Chest + Sea Chest
            continue
        chests.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Sea Chest" if obj.get('type_id') == 82 else "Treasure Chest",
            "gold": obj.get('gold', 0),
            "experience": obj.get('experience', 0),
            "has_artifact": obj.get('has_artifact', False),
            "artifact_id": obj.get('artifact_id', -1) if obj.get('has_artifact') else -1,
        })
    return chests


def merge_resources(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge resource tile objects (PRT sheet "Ресурсы").

    Returns records with:
      x, y, z, locality, object, resource_type, amount
    """
    resources = []
    for obj in tile_objects:
        if obj.get('type_id') not in (79,):  # Resource pile
            continue
        resources.append({
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Resource",
            "resource_type": obj.get('resource_type', 0),
            "resource_name": A_RESOURCE[obj.get('resource_type', 0)] if obj.get('resource_type', 0) < 6 else f"R{obj.get('resource_type', 0)}",
            "amount": obj.get('amount', 0),
        })
    return resources


def merge_event_boxes(tile_objects: List[Dict[str, Any]],
                      post_tile_content: Dict[str, Any]
                      ) -> List[Dict[str, Any]]:
    """Merge event tile objects + Pandora boxes with EventBox content
    (PRT sheet "События и Ящики Пандоры").

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
    """Merge bank tile objects + post-tile Bank content (PRT sheet "Банки").

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
            "locality": "underground" if tb.get('loc') == 8 else "surface",
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
    """Merge garrison tile objects + post-tile Garrison content (PRT sheet "Garrisons").

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
            "locality": "underground" if tg.get('loc') == 8 else "surface",
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
    """Merge seer hut tile objects + post-tile SeerHut content (PRT sheet "Провидцы").

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
            "locality": "underground" if ts.get('loc') == 8 else "surface",
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
            "locality": "underground" if tp.get('loc') == 8 else "surface",
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
    """Merge university tile objects + post-tile Univer content (PRT sheet "Навыки")."""
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
            "locality": "underground" if tu.get('loc') == 8 else "surface",
            "object": "University",
            "address": pu.get("address"),
            "skills": pu.get("skills", []),
        })
    return unis


def merge_scholars(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge scholar tile objects (PRT sheet "Ученые")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Scholar",
            "scholar_type": obj.get('scholar_type', 'unknown'),
            "skill_id": obj.get('skill_id'),
            "spell_id": obj.get('spell_id'),
        }
        for obj in tile_objects if obj.get('type_id') == 81
    ]


def merge_shrines(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge shrine tile objects (PRT sheet "Заклинания" — shrines only)."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Shrine",
            "shrine_type": obj.get('shrine_type', 0),
            "spell_id": obj.get('spell_id', 0),
        }
        for obj in tile_objects if obj.get('type_id') in (88, 89, 90)
    ]


def merge_witch_huts(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge witch hut tile objects (PRT sheet "Навыки" — witch huts)."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Witch Hut",
            "skill_id": obj.get('skill_id', 0),
        }
        for obj in tile_objects if obj.get('type_id') == 113
    ]


def merge_refugee_camps(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge refugee camp tile objects (PRT sheet "Лагеря Беженцев")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Refugee Camp",
            "monster_type": obj.get('monster_type', 0),
            "count": obj.get('count', 0),
        }
        for obj in tile_objects if obj.get('type_id') == 39
    ]


def merge_learning_stones(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge learning stone tile objects (PRT sheet "Опыт")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Learning Stone",
            "experience": obj.get('experience', 1000),
        }
        for obj in tile_objects if obj.get('type_id') == 100
    ]


def merge_prisons(tile_objects: List[Dict[str, Any]],
                  heroes_parsed: List[Dict[str, Any]] = None
                  ) -> List[Dict[str, Any]]:
    """Merge prison tile objects (PRT sheet "Тюрьмы").

    If heroes_parsed is given, look up the hero by ID and include full stats.
    """
    prisons = []
    # Build hero_id → hero dict
    heroes_by_id = {}
    if heroes_parsed:
        for h in heroes_parsed:
            f = h.get('fields', {})
            # The hero's "id" is the index in heroes list
            pass  # We need to know the hero's actual ID
        # Heroes list is in order — index = ID
        heroes_by_id = {i: h for i, h in enumerate(heroes_parsed)}

    for obj in tile_objects:
        if obj.get('type_id') != 62:
            continue
        hero_id = obj.get('hero_id', 0)
        rec = {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Prison",
            "hero_id": hero_id,
        }
        if hero_id in heroes_by_id:
            hero = heroes_by_id[hero_id]
            f = hero.get('fields', {})
            rec["hero_name"] = f.get('name', '?')
            rec["hero_level"] = f.get('level', 0)
            rec["hero_experience"] = f.get('experience', 0)
            rec["hero_primary_skills"] = {
                "attack": f.get('attack', 0),
                "defense": f.get('defense', 0),
                "power": f.get('power', 0),
                "knowledge": f.get('knowledge', 0),
            }
            rec["hero_army_types"] = f.get('army_types', [])
            rec["hero_army_counts"] = f.get('army_counts', [])
            rec["hero_spells_book"] = f.get('spells_book', [])
            rec["hero_equipment"] = f.get('equipment', [])
        prisons.append(rec)
    return prisons


def merge_keymaster_tents(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge keymaster tent tile objects (PRT sheet "Топология")."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Keymaster Tent",
            "color_id": obj.get('color_id', 0),
            "color_name": obj.get('color_name', '?'),
        }
        for obj in tile_objects if obj.get('type_id') == 10
    ]


def merge_monoliths(tile_objects: List[Dict[str, Any]],
                    post_tile_content: Dict[str, Any]) -> List[Dict[str, Any]]:
    """Merge monolith tile objects + post-tile MonolithInfo (PRT sheet "Топология")."""
    out = []
    # Tile scan monoliths: types 43, 44, 45
    for obj in tile_objects:
        if obj.get('type_id') in (43, 44, 45):
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "underground" if obj.get('loc') == 8 else "surface",
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
    """Merge subterranean gate tile objects + post-tile pairing (PRT sheet "Топология")."""
    out = []
    # Tile scan: type 103 (topology_object)
    for obj in tile_objects:
        if obj.get('type_id') == 103:
            out.append({
                "x": obj.get('x'),
                "y": obj.get('y'),
                "z": obj.get('z'),
                "locality": "underground" if obj.get('loc') == 8 else "surface",
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
    """Merge black market tile objects + post-tile Market content (PRT sheet "Рынки")."""
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
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Black Market",
        })
    return markets


def merge_generic_objects(tile_objects: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Merge generic object tile objects (PRT sheet "Объекты" — Hill Fort etc.)."""
    return [
        {
            "x": obj.get('x'),
            "y": obj.get('y'),
            "z": obj.get('z'),
            "locality": "underground" if obj.get('loc') == 8 else "surface",
            "object": "Generic Object",
            "type_id": obj.get('type_id'),
            "object_subtype": obj.get('object_subtype', ''),
            "upgrade_cost": obj.get('upgrade_cost'),
        }
        for obj in tile_objects if obj.get('type_id') in (2, 35, 95, 102, 213)
    ]


def merge_heroes_on_map(tile_objects: List[Dict[str, Any]],
                        heroes_parsed: List[Dict[str, Any]] = None
                        ) -> List[Dict[str, Any]]:
    """Merge hero-on-map tile objects (PRT sheet "Герои" — heroes on map)."""
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
            "locality": "underground" if obj.get('loc') == 8 else "surface",
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
# Master: merge_all_objects — produce all PRT-like tables
# ============================================================================

def merge_all(tile_objects: List[Dict[str, Any]],
              post_tile_content: Dict[str, Any],
              heroes_parsed: List[Dict[str, Any]] = None,
              obj_dict: Optional[Dict] = None
              ) -> Dict[str, List[Dict[str, Any]]]:
    """Produce all PRT-like merged tables in one call.

    Returns a dict with keys matching PRT sheet names (in Russian for clarity):
      Артефакты, Монстры, Банки, События_Ящики, Ученые, Ресурсы, Сундуки,
      Заклинания, Навыки (witch huts), Лагеря_Беженцев, Рынки, Провидцы,
      Тюрьмы, Объекты, Топология, Герои_на_карте, Гарнизоны, Университеты,
      Learning_Stones

    Each value is a list of dicts (one per row in PRT xlsx).
    """
    return {
        "Артефакты": merge_artifacts(tile_objects, post_tile_content, obj_dict),
        "Монстры": merge_monsters(tile_objects, post_tile_content, obj_dict),
        "Банки": merge_banks(tile_objects, post_tile_content),
        "События_Ящики_Пандоры": merge_event_boxes(tile_objects, post_tile_content),
        "Ученые": merge_scholars(tile_objects),
        "Ресурсы": merge_resources(tile_objects),
        "Сундуки": merge_chests(tile_objects, obj_dict),
        "Заклинания_Святилища": merge_shrines(tile_objects),
        "Ведьмины_Хижины": merge_witch_huts(tile_objects),
        "Лагеря_Беженцев": merge_refugee_camps(tile_objects),
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
