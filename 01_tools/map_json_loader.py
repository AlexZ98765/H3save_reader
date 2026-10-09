#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
map_json_loader.py — Load a pre-parsed .h3m map JSON and build
the same data structures that 03_object_mapping/ provides, but
dynamically for ANY map.

This makes the GM1 parser universal: instead of relying on
hardcoded files for "Myth and Legend.h3m", the user loads the
map JSON (produced by their own map parser tool) and the parser
builds the object mapping on the fly.

Usage:
    from map_json_loader import load_map_json, MapData
    md = load_map_json("/path/to/MyMap.h3m.json")
    # or from a .zip:
    md = load_map_json("/path/to/MyMap.h3m.zip")

    md.map_name         # "Myth and Legend"
    md.map_size         # 144
    md.has_underground  # True
    md.n_objects        # 7009
    md.objects_by_coord # {"111:3:0": {coord_int, type, category, ...}, ...}
    md.coord_int_lookup # {879: "111:3:0", ...}
    md.type_index       # {"Tower": ["111:3:0", ...], ...}
    md.category_index   # {"town": ["111:3:0", ...], ...}
"""

import json
import os
import re
import zipfile
import io
from typing import Any, Dict, List, Optional, Tuple
from collections import Counter, defaultdict


# Reuse the category rules from build_object_mapping.py
# (duplicated here so this module is self-contained)
_CATEGORY_RULES = [
    ("town", {
        "Castle", "Tower", "Rampart", "Inferno", "Necropolis", "Dungeon",
        "Stronghold", "Fortress", "Conflux", "Random Town",
    }),
    ("mine", {
        "Sawmill", "Ore Pit", "Gold Mine", "Alchemist's Lab", "Gem Pond",
        "Crystal Cavern", "Sulfur Dune", "Abandoned Mine",
    }),
    ("resource", {
        "Random Resource", "Gold", "Wood", "Ore", "Mercury", "Sulfur",
        "Crystal", "Gems",
    }),
    ("treasure", {
        "Treasure Chest", "Campfire", "Flotsam", "Shipwreck Survivor",
        "Sea Chest",
    }),
    ("artifact", {
        "Random Minor Artifact", "Random Major Artifact",
        "Random Treasure Artifact", "Random Relic Artifact",
    }),
    ("monster", {
        "Random Monster 1", "Random Monster 2", "Random Monster 3",
        "Random Monster 4", "Random Monster 5", "Random Monster 6",
        "Random Monster 7",
    }),
    ("visit_skill", {
        "Learning Stone", "Marletto Tower", "Mercenary Camp",
        "Star Axis", "Garden of Revelation", "Tree of Knowledge",
        "School of Magic", "School of War", "Arena", "Library of Enlightenment",
    }),
    ("visit_magic", {
        "Shrine of Magic Gesture", "Shrine of Magic Thought",
        "Shrine of Magic Incantation", "Magic Well", "Witch Hut",
        "Scholar", "Altar of Sacrifice", "Sanctuary", "Enchanted Spring",
    }),
    ("visit_other", {
        "Stables", "Fountain of Youth", "Fountain of Fortune",
        "Faerie Ring", "Idol of Fortune", "Swan Pond", "Temple",
        "Rally Flag", "Oasis", "Mystical Garden", "Sirens",
        "Black Market", "Trading Post", "University",
        "Cartographer", "Eye of the Magi", "Hut of the Magi",
        "Redwood Observatory", "Lighthouse",
    }),
    ("hero", {"Prison", "Random Hero", "Hero"}),
    ("teleport", {
        "Monolith Two Way", "Monolith One Way Entrance",
        "Monolith One Way Exit", "Subterranean Gate", "Whirlpool",
    }),
    ("sign", {"Sign"}),
    ("event", {"Event"}),
    ("quest", {
        "Seer's Hut", "Border Guard", "Border Gate", "Quest Guard",
        "Hill Fort", "Den of Thieves",
    }),
    ("boat", {"Boat"}),
    ("terrain_modifier", {"Cursed Ground", "Magic Plains"}),
]

_DEFAULT_CATEGORY = "decoration"


def _categorize(type_name: str) -> str:
    for cat, names in _CATEGORY_RULES:
        if type_name in names:
            return cat
    n = type_name.lower()
    if any(k in n for k in ("mountain", "rock", "trees", "vegetation", "cactus",
                            "flowers", "shrub", "moss", "kelp", "mushrooms",
                            "mandrake", "volcano", "lava", "lake", "reef",
                            "crater", "outcropping", "stump", "log", "mound",
                            "canyon", "skull", "hole", "sand dune", "river delta")):
        return "decoration"
    return _DEFAULT_CATEGORY


def _val(field):
    if isinstance(field, dict):
        v = field.get("value")
        return v if v is not None else field.get("value_int")
    return field


def _text_value(node):
    if not isinstance(node, dict):
        return node
    v = node.get("value")
    if v is not None and not isinstance(v, (dict, list)):
        return v
    tv = node.get("text_value")
    if isinstance(tv, dict):
        return _val(tv)
    nm = node.get("name")
    if isinstance(nm, dict):
        return _text_value(nm)
    se = node.get("setting_exists")
    if isinstance(se, dict):
        sub = {k: v for k, v in node.items() if k != "setting_exists"}
        if sub:
            if "name" in sub:
                return _text_value(sub["name"])
            for k, v in sub.items():
                if isinstance(v, dict):
                    return _text_value(v)
        return _val(se)
    return None


def _extract_details(cond: dict) -> dict:
    if not cond:
        return {}
    if len(cond) == 1 and isinstance(next(iter(cond.values())), dict):
        actual = next(iter(cond.values()))
    else:
        actual = cond
    out = {}
    if "town_structure" in actual:
        ts = actual["town_structure"]
        out["player_color"] = _val(ts.get("player_color"))
        out["town_name"] = _text_value(ts.get("town_name"))
    if "hero_settings" in actual:
        hs = actual["hero_settings"]
        out["player_color"] = _val(hs.get("player_color"))
        out["hero_id"] = _val(hs.get("hero_id"))
        out["name"] = _text_value(hs.get("name"))
    if "resource_quantity" in actual:
        out["resource_quantity"] = _val(actual["resource_quantity"])
    if "monster_settings" in actual:
        ms = actual["monster_settings"]
        out["monster_quantity"] = _val(ms.get("monster_quantity"))
    if "guard" in actual:
        g = actual["guard"]
        se = g.get("setting_exists") if isinstance(g, dict) else None
        if isinstance(se, dict):
            out["guarded"] = bool(_val(se))
    if "message" in actual:
        out["message"] = _text_value(actual["message"])
    if "player_color" in actual and "player_color" not in out:
        out["player_color"] = _val(actual["player_color"])
    return out


class MapData:
    """Holds all parsed map data needed by the GM1 parser."""

    def __init__(self):
        self.map_name: str = ""
        self.map_size: int = 0
        self.has_underground: bool = False
        self.description: str = ""
        self.n_objects: int = 0
        self.n_sprites: int = 0
        self.n_towns: int = 0
        self.n_heroes: int = 0
        self.n_players: int = 0

        self.objects_by_coord: Dict[str, dict] = {}
        self.coord_int_lookup: Dict[int, str] = {}
        self.type_index: Dict[str, list] = defaultdict(list)
        self.category_index: Dict[str, list] = defaultdict(list)

        self.source_path: str = ""
        self.source_type: str = ""  # "json" or "zip"

    def summary(self) -> str:
        return (f"Map: {self.map_name} ({self.map_size}×{self.map_size}"
                f"{' + underground' if self.has_underground else ''}), "
                f"{self.n_objects} objects, {self.n_towns} towns, "
                f"{self.n_heroes} heroes, {self.n_players} players")


def _parse_coords(coord_str: str):
    s = coord_str.strip().lstrip("(").rstrip(")")
    return tuple(int(p.strip()) for p in s.split(","))


def _coord_int_from_xyz(x, y, z):
    return (x & 0xFF) | ((y & 0xFF) << 8) | ((z & 0xFF) << 16)


def load_map_json(path: str) -> MapData:
    """
    Load a pre-parsed .h3m map JSON from a .json or .zip file.

    The JSON should have the same structure as produced by the
    h3m map parser (top-level key = filename, containing map_info,
    objects, sprites, players, etc.).
    """
    md = MapData()
    md.source_path = path

    # Load JSON (from .json or .zip)
    if path.lower().endswith(".zip"):
        md.source_type = "zip"
        with zipfile.ZipFile(path, "r") as zf:
            # Find the first .json file in the archive
            json_names = [n for n in zf.namelist() if n.lower().endswith(".json")]
            if not json_names:
                raise ValueError(f"No .json file found in {path}")
            with zf.open(json_names[0]) as f:
                data = json.load(f)
    else:
        md.source_type = "json"
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)

    # Find the top-level map key (usually the filename without extension)
    # The structure is { "MapName.h3m": { map_info, objects, ... } }
    if len(data) == 1:
        map_key = next(iter(data.keys()))
        root = data[map_key]
        md.map_name = map_key.rsplit(".", 1)[0] if "." in map_key else map_key
    else:
        # Maybe the data IS the root (no wrapper key)
        root = data
        md.map_name = os.path.basename(path).rsplit(".", 1)[0]

    # ----- map_info -----
    mi = root.get("map_info", {})
    md.map_name = _val(mi.get("name")) or md.map_name
    md.map_size = _val(mi.get("map_size")) or 0
    md.has_underground = bool(_val(mi.get("has_underground")))
    md.description = _val(mi.get("description")) or ""

    # ----- players -----
    players = root.get("players", {})
    md.n_players = len(players) if isinstance(players, dict) else 0

    # ----- objects -----
    obj_node = root.get("objects", {})
    obj_list = obj_node.get("object_list") if isinstance(obj_node, dict) else None
    if isinstance(obj_list, dict):
        obj_items = list(obj_list.values())
    elif isinstance(obj_list, list):
        obj_items = obj_list
    else:
        obj_items = []
    md.n_objects = len(obj_items)

    # ----- sprites (for .def name lookup) -----
    sprite_node = root.get("sprites", {})
    sprite_list = sprite_node.get("sprite_list") if isinstance(sprite_node, dict) else None
    sprite_lookup = {}
    if isinstance(sprite_list, dict):
        for k, sp in sprite_list.items():
            name_node = sp.get("name", {}) if isinstance(sp, dict) else {}
            name = _val(name_node.get("text_value")) if isinstance(name_node, dict) else None
            try:
                idx1 = int(k.replace("sprite_item_", ""))
                sprite_lookup[idx1] = name
            except (ValueError, TypeError):
                pass
    md.n_sprites = len(sprite_lookup)

    # ----- Build objects_by_coord -----
    overlays = defaultdict(list)
    PRIORITY = {
        "town": 0, "hero": 1, "mine": 2, "resource": 3, "artifact": 4,
        "treasure": 5, "monster": 6, "visit_skill": 7, "visit_magic": 8,
        "visit_other": 9, "quest": 10, "teleport": 11, "boat": 12,
        "sign": 13, "event": 14, "terrain_modifier": 15, "decoration": 99,
    }

    # Iterate as (key, value) pairs regardless of dict/list
    if isinstance(obj_list, dict):
        obj_iter = obj_list.items()
    elif isinstance(obj_list, list):
        obj_iter = enumerate(obj_list)
    else:
        obj_iter = []

    for k, obj in obj_iter:
        if not isinstance(obj, dict):
            continue
        coord_node = obj.get("coords", {})
        ref_node = obj.get("sprite_ref_id", {})
        cond = obj.get("object_condition", {})

        coord_str = _val(coord_node) if isinstance(coord_node, dict) else None
        if not coord_str:
            continue
        try:
            x, y, z = _parse_coords(coord_str)
        except Exception:
            continue

        ref_id = _val(ref_node) if isinstance(ref_node, dict) else None
        if cond and isinstance(cond, dict) and len(cond) > 0:
            type_name = next(iter(cond.keys()))
        else:
            type_name = "(empty)"

        sprite_def = sprite_lookup.get(ref_id)
        category = _categorize(type_name)
        coord_key = f"{x}:{y}:{z}"
        ci = _coord_int_from_xyz(x, y, z)

        details = _extract_details(cond)

        record = {
            "coord_key": coord_key,
            "coord_int": ci,
            "x": x, "y": y, "z": z,
            "object_index": (int(k.replace("object_item_", ""))
                             if isinstance(k, str) and k.startswith("object_item_")
                             else (k + 1) if isinstance(k, int) else 0),
            "sprite_ref_id": ref_id,
            "sprite_def": sprite_def or "",
            "type": type_name,
            "category": category,
            "details": details,
        }

        if coord_key in md.objects_by_coord:
            existing = md.objects_by_coord[coord_key]
            if PRIORITY.get(category, 50) < PRIORITY.get(existing["category"], 50):
                overlays[coord_key].append(existing)
                md.objects_by_coord[coord_key] = record
            else:
                overlays[coord_key].append(record)
        else:
            md.objects_by_coord[coord_key] = record
            md.coord_int_lookup[ci] = coord_key

        md.type_index[type_name].append(coord_key)
        md.category_index[category].append(coord_key)

    # Attach overlays
    for ck, rec in md.objects_by_coord.items():
        if ck in overlays:
            rec["overlays"] = [
                {"type": o["type"], "category": o["category"],
                 "sprite_def": o["sprite_def"], "details": o["details"]}
                for o in overlays[ck]
            ]

    # Count towns and heroes
    md.n_towns = len(md.category_index.get("town", []))
    md.n_heroes = len(md.category_index.get("hero", []))

    # Convert defaultdicts to plain dicts
    md.type_index = dict(md.type_index)
    md.category_index = dict(md.category_index)

    return md


def load_map_json_safely(path: str) -> Tuple[Optional[MapData], Optional[str]]:
    """
    Load a map JSON, returning (MapData, None) on success or
    (None, error_message) on failure.
    """
    try:
        md = load_map_json(path)
        if md.n_objects == 0:
            return None, f"No objects found in {path} — is this a valid map JSON?"
        return md, None
    except Exception as e:
        return None, str(e)
