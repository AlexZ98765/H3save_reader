#!/usr/bin/env python3
"""
build_object_mapping.py

Build a comprehensive {coordinate -> object type + details} mapping
for all 7009 objects on the "Myth and Legend.h3m" map.

Outputs (saved to /home/z/my-project/download/):
  - objects_by_coord.json     : "x:y:z" -> full object record
  - coord_int_lookup.json     : coord_int -> "x:y:z"   (for GM1 save lookups)
  - type_index.json           : type -> [coord_key, ...]
  - category_index.json       : category -> [coord_key, ...]
  - object_mapping_summary.json : top-level stats (counts per type/category)

Coordinate encoding in GM1 saves is a 32-bit packed int:
    coord_int = x | (y << 8) | (z << 16)
(where z=0 surface, z=1 underground).
We pre-compute this for every object so the parser can look up the type
directly from a save's visiting_coords / hero location / town location.

Run:
    python3 /home/z/my-project/scripts/build_object_mapping.py
"""

import json
import os
import sys
from collections import Counter, defaultdict

MAP_JSON = "/home/z/my-project/upload/Myth and Legend.h3m.json"
OUT_DIR  = "/home/z/my-project/download"


# ----------------------------------------------------------------------
# Category classification
# ----------------------------------------------------------------------
# Coarse category buckets, so we can quickly find e.g. "all towns" or
# "all monsters" without enumerating every H3 object type name.

CATEGORY_RULES = [
    # (category, set of object type names that belong to it)
    ("town", {
        "Castle", "Tower", "Rampart", "Inferno", "Necropolis", "Dungeon",
        "Stronghold", "Fortress", "Conflux", "Random Town",
    }),

    # mines / resource producers
    ("mine", {
        "Sawmill", "Ore Pit", "Gold Mine", "Alchemist's Lab", "Gem Pond",
        "Crystal Cavern", "Sulfur Dune", "Abandoned Mine",
    }),

    # raw resource piles on the map
    ("resource", {
        "Random Resource", "Gold", "Wood", "Ore", "Mercury", "Sulfur",
        "Crystal", "Gems",
    }),

    # treasure (gold + maybe other)
    ("treasure", {
        "Treasure Chest", "Campfire", "Flotsam", "Shipwreck Survivor",
        "Sea Chest", "Daemon Cave Loot", "Leaning Tower of Pisa",
    }),

    # artifacts lying on the ground
    ("artifact", {
        "Random Minor Artifact", "Random Major Artifact",
        "Random Treasure Artifact", "Random Relic Artifact",
        "Necklace of Ocean Guidance", "Endless Purse of Gold",
        "Endless Bag of Gold", "Endless Sack of Gold",
        "Endless String of Beads", "Endless Cart of Ore",
        "Endless Pouch of Crystal", "Endless Pouch of Sulfur",
        "Endless Vial of Mercury", "Endless Pouch of Gems",
        "Endless Rod of Gems", "Endless Sack of Ore",
        "Sandals of the Saint", "Diplomat's Ring",
        "Book of Earth", "Book of Air", "Book of Fire", "Book of Water",
        "Sphere of Permanence", "Sphere of Negation",
    }),

    # monsters (all standalone stacks guarding / wandering)
    ("monster", {
        "Random Monster 1", "Random Monster 2", "Random Monster 3",
        "Random Monster 4", "Random Monster 5", "Random Monster 6",
        "Random Monster 7",
        # named creature stacks
        "Azure Dragon", "Black Dragon", "Bone Dragon", "Chimera",
        "Crusader", "Cyclops", "Cyclops Stockpile", "Daemon", "Death Knight",
        "Dead Knight", "Druid", "Elven Archer", "Enchanter", "Familiar",
        "Genie", "Giant", "Gnoll Marauder", "Goblin", "Gog", "Golem",
        "Gorgon", "Griffin", "Halberdier", "Hell Hound", "Hobgoblin",
        "Hydra", "Imp", "Iron Golem", "Knight", "Lich", "Lizardman",
        "Mage", "Marksman", "Master Genie", "Medusa Queen",
        "Minotaur", "Minotaur King", "Monk", "Naga", "Necromancer",
        "Ogre", "Ogre Mage", "Paladin", "Peasant", "Pegasus",
        "Pikeman", "Pit Fiend", "Power Lich", "Ranger", "Roc",
        "Skeleton", "Skeleton Warrior", "Silver Pegasi", "Sorceress",
        "Stone Golem", "Vampire", "Vampire Lord", "Walking Dead",
        "Warlock", "Warrior", "Wight", "Wraith", "Wyvern",
        "Wolf", "Wood Elf", "Zealot", "Centaur", "Centaur Captain",
        "Centaur Stables", "Cerberus", "Champion", "Cleric",
        "Dendroid Guard", "Dragon", "Dragon Fly", "Dragon Utopia",
        "Dread Knight", "Dwarf", "Earth Elemental", "Efreet",
        "Efreet Sultan", "Efreets", "Evil Eye", "Fire Elemental",
        "Ghost", "Ghost Dragon", "Gnoll", "Goblin Knight",
        "Grand Elf", "Green Dragon", "Gremlin", "Harpies",
        "Hell Steed", "Hobbit", "Horned Demon", "Horned Minotaur",
        "Ice Elemental", "Implosion", "Infernal Trooper",
        "Ironclad Behemoth", "Lich King", "Lord of the Pit",
        "Mage Killer", "Magic Elemental", "Mantis", "Master Gremlin",
        "Medusa", "Megadragon", "Mummy", "Naga Queen",
        "Necrodragon", "Nightmare", "Nix", "Nix Warrior",
        "Obsidian Gargoyle", "Orc", "Orc Chieftain", "Overlord",
        "Paladin", "Peasants", "Pegasus Knight", "Phoenix",
        "Red Dragon", "Royal Griffin", "Satyr", "Shadow Witch",
        "Skeleton Mage", "Skink", "Spirit Elemental", "Sprite",
        "Storm Elemental", "Suicide Dragon", "Thief", "Titan",
        "Troll", "Unicorn", "Vampire Lord", "Vermin", "Water Elemental",
        "Wight", "Wraith", "Wyrm",
        # creature banks (still mostly monster fights)
        "Naga Bank", "Dragon Fly Hive", "Dragon Utopia",
        "Cyclops Stockpile", "Griffin Cache", "Hydra Pond",
        "Imp Cache", "Medusa Stores", "Roc Nest", "Troll Bridge",
        "Wolf Raider Pit",
    }),

    # creature banks (banks of monsters with rewards)
    ("bank", {
        "Naga Bank", "Dragon Fly Hive", "Dragon Utopia",
        "Cyclops Stockpile", "Derelict Ship", "Crypt", "Dragon Cave",
        "Dwarven Treasury", "Golem Factory", "Griffin Cache",
        "Hydra Pond", "Imp Cache", "Medusa Stores", "Roc Nest",
        "Shipwreck", "Troll Bridge", "Wolf Raider Pit",
    }),

    # dwellings (recruit units on adventure map)
    ("dwelling", {
        "Dwarf Cottage", "Gnoll Hut", "Homestead", "Lizard Den",
        "Parapet", "Workshop", "Barracks", "Training Grounds",
        "Hall of Sins", "Kennels", "Imp Crucible", "Dendroid Arches",
        "Unicorn Glade", "Dragon Cliffs", "Centaur Capitan",
        "Refugee Camp", "Wight", "Wraight",
    }),

    # skill-boosting visitable objects
    ("visit_skill", {
        "Learning Stone", "Marletto Tower", "Mercenary Camp",
        "Star Axis", "Garden of Revelation", "Tree of Knowledge",
        "School of Magic", "School of War", "Arena", "Library of Enlightenment",
    }),

    # magic-boosting / spell-granting visitable objects
    ("visit_magic", {
        "Shrine of Magic Gesture", "Shrine of Magic Thought",
        "Shrine of Magic Incantation", "Magic Well", "Magic Spring",
        "Witch Hut", "Scholar", "Altar of Sacrifice", "Sanctuary",
        "Enchanted Spring",
    }),

    # other visitable (one-shot / permanent buffs)
    ("visit_other", {
        "Stables", "Fountain of Youth", "Fountain of Fortune",
        "Faerie Ring", "Idol of Fortune", "Swan Pond", "Temple",
        "Rally Flag", "Oasis", "Watering Hole", "Mystical Garden",
        "Sirens", "Corral", "Centaur Stables", "Black Market",
        "Trading Post", "University",
        "Arena", "Boat", "Cartographer", "Eye of the Magi",
        "Hut of the Magi", "Redwood Observatory", "Lighthouse",
    }),

    # hero-related
    ("hero", {
        "Prison", "Random Hero", "Hero",
    }),

    # teleports
    ("teleport", {
        "Monolith Two Way", "Monolith One Way Entrance",
        "Monolith One Way Exit", "Subterranean Gate", "Whirlpool",
    }),

    # sign / event
    ("sign",   {"Sign"}),
    ("event",  {"Event"}),

    # special / quest
    ("quest", {
        "Seer's Hut", "Border Guard", "Border Gate", "Quest Guard",
        "Hill Fort", "Den of Thieves",
    }),

    # garrison (defensive structure with stationed units)
    ("garrison", {
        "Rampart", "Garrison", "Anti-Magic Garrison",
    }),

    # terrain modifiers
    ("terrain_modifier", {
        "Cursed Ground", "Magic Plains", "Clover Field",
        "Evil Fog", "Favorable Winds", "Lucid Pool",
        "Magic Clouds", "Cursed Lands", "Holy Ground", "Fairy Ring",
    }),

    # observatories (reveal map)
    ("observatory", {
        "Redwood Observatory", "Hut of the Magi", "Eye of the Magi",
        "Cartographer",
    }),

    # water / boat
    ("boat", {"Boat"}),

    # shipyard
    ("shipyard", {"Shipyard"}),
]

DEFAULT_CATEGORY = "decoration"


def categorize(type_name: str) -> str:
    """Map an H3 object-type name to a coarse category bucket."""
    for cat, names in CATEGORY_RULES:
        if type_name in names:
            # NOTE: 'monster' rule also lists some banks; 'bank' rule
            # overrides AFTER because we check 'bank' first in the order
            # above. To handle the overlap cleanly, we re-check here.
            return cat
    # Heuristics on the type name
    n = type_name.lower()
    if "mountain" in n or "rock" in n or "trees" in n or "vegetation" in n \
       or "cactus" in n or "flowers" in n or "shrub" in n or "moss" in n \
       or "kelp" in n or "mushrooms" in n or "mandrake" in n \
       or "volcano" in n or "lava" in n or "lake" in n or "reef" in n \
       or "crater" in n or "outcropping" in n or "stump" in n \
       or "log" in n or "mound" in n or "canyon" in n or "skull" in n \
       or "hole" in n or "lean to" in n or "sand dune" in n \
       or "river delta" in n or "oasis" in n:
        return "decoration"
    if "mine" in n or "sawmill" in n or "ore pit" in n \
       or "alchemist" in n or "gem pond" in n \
       or "crystal cavern" in n or "sulfur dune" in n:
        return "mine"
    return DEFAULT_CATEGORY


# ----------------------------------------------------------------------
# Helpers to extract the most useful fields from an object_condition
# ----------------------------------------------------------------------

def _val(field):
    """Return the 'value' field of a parsed-map node, or None."""
    if isinstance(field, dict):
        v = field.get("value")
        if v is not None:
            return v
        # fall back to value_int for numeric cases
        return field.get("value_int")
    return field


def _text_value(node):
    """
    Pull a human-readable text out of a parsed-map node which may have
    several shapes:
      - {"text_value": {"value": "..."}, "text_value_length": {...}}
      - {"setting_exists": {"value": true}, "name": {...}}
      - {"value": "..."}  (already resolved)
    """
    if not isinstance(node, dict):
        return node
    # Direct value
    v = node.get("value")
    if v is not None and not isinstance(v, (dict, list)):
        return v
    # text_value subkey
    tv = node.get("text_value")
    if isinstance(tv, dict):
        return _val(tv)
    # name subkey (used inside setting_exists)
    nm = node.get("name")
    if isinstance(nm, dict):
        return _text_value(nm)
    # setting_exists wrapper for optional fields
    se = node.get("setting_exists")
    if isinstance(se, dict):
        # If the optional field is actually present, dig deeper
        sub = {k: v for k, v in node.items() if k != "setting_exists"}
        if sub:
            # try name first
            if "name" in sub:
                return _text_value(sub["name"])
            # else, just return any string in sub
            for k, v in sub.items():
                if isinstance(v, dict):
                    return _text_value(v)
        # else, return the bool
        return _val(se)
    return None


def _bool(field):
    """Resolve a guard/setting_exists flag to True/False/None."""
    if isinstance(field, dict):
        se = field.get("setting_exists")
        if isinstance(se, dict):
            v = se.get("value")
            if isinstance(v, bool):
                return v
            vi = se.get("value_int")
            if vi is not None:
                return bool(vi)
        v = field.get("value")
        if isinstance(v, bool):
            return v
        vi = field.get("value_int")
        if vi is not None:
            return bool(vi)
    return None


def _extract_details(type_name: str, cond: dict) -> dict:
    """
    Pull the most useful per-type fields out of the parsed object_condition.
    Keeps the output small and parseable for the GM1 save parser.

    NOTE: in the parsed .h3m.json, cond has shape {<TypeName>: {...}}.
    So we first unwrap the single type-key to get the actual fields.
    """
    if not cond:
        return {}

    # Unwrap {<TypeName>: {actual_fields}} -> actual_fields
    if len(cond) == 1 and isinstance(next(iter(cond.values())), dict):
        actual = next(iter(cond.values()))
    else:
        # Some shapes (very rare) might already be the inner dict
        actual = cond

    out = {}

    # Town / dwelling-like: look for town_structure subkey
    if "town_structure" in actual:
        ts = actual["town_structure"]
        out["player_color"]   = _val(ts.get("player_color"))
        out["formation"]      = _val(ts.get("formation"))
        out["town_name"]      = _text_value(ts.get("town_name"))
        # building bitmask
        bs = ts.get("building_set")
        if isinstance(bs, dict):
            out["building_set"] = _val(bs)
        # spells
        sm = ts.get("spells_must")
        if isinstance(sm, dict):
            out["spells_must"] = _val(sm)
        sd = ts.get("spells_disabled")
        if isinstance(sd, dict):
            out["spells_disabled"] = _val(sd)
        # as_player_color (owner override)
        apc = ts.get("as_player_color")
        if apc is not None:
            out["as_player_color"] = _val(apc)
        # creature_set (initial garrison) - skip if too complex

    # Hero settings (Prison, hero placeholders)
    if "hero_settings" in actual:
        hs = actual["hero_settings"]
        out["player_color"] = _val(hs.get("player_color"))
        out["hero_id"]     = _val(hs.get("hero_id"))
        out["face"]        = _val(hs.get("face"))
        out["name"]        = _text_value(hs.get("name"))
        # hero experience / skills / army may be present
        exp = hs.get("experience")
        if isinstance(exp, dict):
            out["experience"] = _val(exp)

    # Resource piles / random resource
    if "resource_quantity" in actual:
        out["resource_quantity"] = _val(actual["resource_quantity"])

    # Monsters
    if "monster_settings" in actual:
        ms = actual["monster_settings"]
        out["monster_quantity"] = _val(ms.get("monster_quantity"))
        out["disposition"]      = _val(ms.get("disposition"))
        out["monster_text"]     = _val(ms.get("monster_text"))

    # Artifacts / chests / guarded objects
    if "guard" in actual:
        g = actual["guard"]
        out["guarded"] = _bool(g)
        # If guarded, try to extract the guard monster stack
        gs = g.get("guard_settings") if isinstance(g, dict) else None
        if isinstance(gs, dict) and gs:
            out["guard_monster_type"]  = _val(gs.get("monster_type"))
            out["guard_monster_count"] = _val(gs.get("monster_count"))
            out["guard_character"]     = _val(gs.get("character"))

    # Witch hut / Scholar: skills
    if "secondary_skills" in actual:
        out["secondary_skills"] = _val(actual["secondary_skills"])

    if "scholar_choice" in actual:
        out["scholar_choice"] = _val(actual["scholar_choice"])

    # Signs / messages
    if "message" in actual:
        m = actual["message"]
        out["message"] = _text_value(m)

    # Events (have rich payload)
    if "hidden_set" in actual:
        hs = actual["hidden_set"]
        out["experience"]   = _val(hs.get("experience"))
        out["spell_points"] = _val(hs.get("spell_points"))
        out["morale"]       = _val(hs.get("morale"))
        out["luck"]         = _val(hs.get("luck"))
        # resources
        res = hs.get("resources")
        if isinstance(res, dict):
            out["resources"] = {k: _val(v) for k, v in res.items()
                                if isinstance(v, dict)}
        sk = hs.get("secondary_skills_gained")
        if sk:
            out["secondary_skills_gained"] = _val(sk)
        ag = hs.get("artifacts_gained")
        if ag:
            out["artifacts_gained"] = _val(ag)
        mg = hs.get("monsters_gained")
        if mg:
            out["monsters_gained"] = _val(mg)
        pa = hs.get("players_applied")
        if pa:
            out["players_applied"] = _val(pa)
        out["human_trigger"] = _val(hs.get("human_trigger"))

    # Sawmill / Lighthouse / mine-style objects have a top-level player_color
    if "player_color" in actual and "player_color" not in out:
        out["player_color"] = _val(actual["player_color"])

    # Boat / teleporter / treasure chest have empty conditions - skip

    return out


# ----------------------------------------------------------------------
# Main
# ----------------------------------------------------------------------

def parse_coords(coord_str: str):
    """Parse '(x,y,z)' -> (x, y, z) ints."""
    s = coord_str.strip().lstrip("(").rstrip(")")
    parts = [int(p.strip()) for p in s.split(",")]
    return tuple(parts)


def coord_int_from_xyz(x: int, y: int, z: int) -> int:
    """Encode (x, y, z) the way GM1 saves do."""
    return (x & 0xFF) | ((y & 0xFF) << 8) | ((z & 0xFF) << 16)


def main():
    if not os.path.exists(MAP_JSON):
        print(f"FATAL: map JSON not found at {MAP_JSON}", file=sys.stderr)
        sys.exit(1)

    print(f"Loading {MAP_JSON} ...")
    with open(MAP_JSON, "r", encoding="utf-8") as f:
        data = json.load(f)

    root = data["Myth and Legend.h3m"]
    objects = root["objects"]["object_list"]
    sprites = root["sprites"]["sprite_list"]

    # Pre-build sprite index -> .def name lookup
    sprite_lookup = {}
    for k, sp in sprites.items():
        # the sprite name is the .def filename
        name_node = sp.get("name", {})
        name = None
        if isinstance(name_node, dict):
            name = _val(name_node.get("text_value"))
        idx_str = k.replace("sprite_item_", "")
        try:
            idx = int(idx_str) - 1  # sprite_item_1 -> index 0?
        except ValueError:
            idx = -1
        # NOTE: in the h3m format sprite_ref_id is 1-based?
        # We'll resolve by both 0-based and 1-based just in case.
        sprite_lookup[idx] = name

    # Also build 1-based lookup (sprite_item_1 -> def name)
    sprite_lookup_1based = {}
    for k, sp in sprites.items():
        name_node = sp.get("name", {})
        name = _val(name_node.get("text_value")) if isinstance(name_node, dict) else None
        idx_str = k.replace("sprite_item_", "")
        try:
            idx1 = int(idx_str)
            sprite_lookup_1based[idx1] = name
        except ValueError:
            pass

    # Also map sprite name -> base type (e.g. "AVWmrnd0" without extension)
    def base_type(def_name: str) -> str:
        if not def_name:
            return ""
        return def_name.rsplit(".", 1)[0]

    # ----- Build records -----
    by_coord      = {}     # "x:y:z" -> primary record (first object on tile)
    overlays      = defaultdict(list)  # "x:y:z" -> [additional records on the same tile]
    coord_int_map = {}     # coord_int -> "x:y:z"
    type_index    = defaultdict(list)
    cat_index     = defaultdict(list)
    sprite_index  = defaultdict(list)  # sprite_ref_id (1-based) -> [coord_key]
    type_counter  = Counter()
    cat_counter   = Counter()
    coord_collisions = []

    n_objs = 0
    for k, obj in objects.items():
        n_objs += 1
        coord_node  = obj.get("coords", {})
        ref_node    = obj.get("sprite_ref_id", {})
        cond        = obj.get("object_condition", {})

        coord_str = _val(coord_node) if isinstance(coord_node, dict) else None
        if not coord_str:
            continue
        try:
            x, y, z = parse_coords(coord_str)
        except Exception as e:
            print(f"WARN: cannot parse coords {coord_str!r}: {e}", file=sys.stderr)
            continue

        ref_id = _val(ref_node) if isinstance(ref_node, dict) else None

        # Determine object type name (the single key inside object_condition)
        if cond and isinstance(cond, dict) and len(cond) > 0:
            type_name = next(iter(cond.keys()))
        else:
            type_name = "(empty)"

        # Resolve sprite .def name (try 1-based first)
        sprite_def = sprite_lookup_1based.get(ref_id) or sprite_lookup.get(ref_id)

        category = categorize(type_name)

        coord_key = f"{x}:{y}:{z}"
        ci = coord_int_from_xyz(x, y, z)

        details = _extract_details(type_name, cond)

        record = {
            "coord_key":     coord_key,
            "coord_int":     ci,
            "x": x, "y": y, "z": z,
            "object_index":  int(k.replace("object_item_", "")),
            "sprite_ref_id": ref_id,
            "sprite_def":    sprite_def,
            "sprite_base":   base_type(sprite_def) if sprite_def else None,
            "type":          type_name,
            "category":      category,
            "details":       details,
        }

        # Handle collisions (multiple objects on the same tile).
        # H3 stacks decoration on top of "real" objects; we keep the
        # most informative object as the primary, the rest as overlays.
        # Priority: town > hero > mine > dwelling > monster > resource >
        # artifact > treasure > visit_* > quest > teleport > garrison >
        # boat > sign > event > observatory > terrain_modifier > decoration
        PRIORITY = {
            "town": 0, "hero": 1, "mine": 2, "dwelling": 3,
            "monster": 4, "bank": 5, "resource": 6, "artifact": 7,
            "treasure": 8, "visit_skill": 9, "visit_magic": 10,
            "visit_other": 11, "quest": 12, "teleport": 13,
            "garrison": 14, "shipyard": 15, "boat": 16,
            "sign": 17, "event": 18, "observatory": 19,
            "terrain_modifier": 20, "decoration": 99,
        }

        if coord_key in by_coord:
            existing = by_coord[coord_key]
            coord_collisions.append((coord_key, existing["type"], type_name))
            # If the new record is "more important" than the existing one,
            # swap them.
            if PRIORITY.get(category, 50) < PRIORITY.get(existing["category"], 50):
                overlays[coord_key].append(existing)
                by_coord[coord_key] = record
            else:
                overlays[coord_key].append(record)
        else:
            by_coord[coord_key] = record
            coord_int_map[ci] = coord_key

        type_index[type_name].append(coord_key)
        cat_index[category].append(coord_key)
        if ref_id is not None:
            sprite_index[ref_id].append(coord_key)
        type_counter[type_name] += 1
        cat_counter[category]  += 1

    print(f"Processed {n_objs} objects.")
    print(f"Unique coordinate keys: {len(by_coord)}")
    print(f"Collisions on same tile: {len(coord_collisions)}")
    print(f"Tiles with overlays: {len(overlays)}")

    print()
    print("Top categories:")
    for cat, c in cat_counter.most_common():
        print(f"  {cat:25s} {c}")

    print()
    print(f"Distinct types: {len(type_counter)}")

    # ----- Attach overlays to the primary record -----
    for ck, rec in by_coord.items():
        if ck in overlays:
            rec["overlays"] = [
                {"type": o["type"], "category": o["category"],
                 "sprite_def": o["sprite_def"], "sprite_ref_id": o["sprite_ref_id"],
                 "object_index": o["object_index"], "details": o["details"]}
                for o in overlays[ck]
            ]

    # ----- Write outputs -----
    os.makedirs(OUT_DIR, exist_ok=True)

    out1 = os.path.join(OUT_DIR, "objects_by_coord.json")
    with open(out1, "w", encoding="utf-8") as f:
        json.dump(by_coord, f, ensure_ascii=False, indent=1)
    print(f"Wrote {out1}  ({os.path.getsize(out1):,} bytes)")

    out2 = os.path.join(OUT_DIR, "coord_int_lookup.json")
    with open(out2, "w", encoding="utf-8") as f:
        json.dump({str(k): v for k, v in coord_int_map.items()},
                  f, ensure_ascii=False, indent=1)
    print(f"Wrote {out2}  ({os.path.getsize(out2):,} bytes)")

    out3 = os.path.join(OUT_DIR, "type_index.json")
    with open(out3, "w", encoding="utf-8") as f:
        json.dump(type_index, f, ensure_ascii=False, indent=1)
    print(f"Wrote {out3}  ({os.path.getsize(out3):,} bytes)")

    out4 = os.path.join(OUT_DIR, "category_index.json")
    with open(out4, "w", encoding="utf-8") as f:
        json.dump(cat_index, f, ensure_ascii=False, indent=1)
    print(f"Wrote {out4}  ({os.path.getsize(out4):,} bytes)")

    summary = {
        "map_name":        "Myth and Legend.h3m",
        "total_objects":   n_objs,
        "unique_coords":   len(by_coord),
        "coord_collisions": len(coord_collisions),
        "tiles_with_overlays": len(overlays),
        "categories":      dict(cat_counter.most_common()),
        "types":           dict(type_counter.most_common()),
        "n_distinct_types": len(type_counter),
        "n_distinct_sprites_used": len(sprite_index),
        "n_distinct_sprites_total": len(sprites),
    }
    out5 = os.path.join(OUT_DIR, "object_mapping_summary.json")
    with open(out5, "w", encoding="utf-8") as f:
        json.dump(summary, f, ensure_ascii=False, indent=2)
    print(f"Wrote {out5}  ({os.path.getsize(out5):,} bytes)")

    # Print a small example for sanity check
    print("\nExample records (first 3):")
    for ck in list(by_coord.keys())[:3]:
        print(json.dumps(by_coord[ck], ensure_ascii=False)[:400])

    print("\nExample town record:")
    for ck, r in by_coord.items():
        if r["category"] == "town":
            print(json.dumps(r, ensure_ascii=False, indent=2)[:1500])
            break

    print("\nExample resource record:")
    for ck, r in by_coord.items():
        if r["category"] == "resource" and r["details"]:
            print(json.dumps(r, ensure_ascii=False, indent=2)[:800])
            break

    print("\nExample monster record:")
    for ck, r in by_coord.items():
        if r["category"] == "monster" and r["details"]:
            print(json.dumps(r, ensure_ascii=False, indent=2)[:800])
            break

    print("\nExample hero (Prison) record:")
    for ck, r in by_coord.items():
        if r["category"] == "hero":
            print(json.dumps(r, ensure_ascii=False, indent=2)[:1500])
            break

    print("\nExample sign record:")
    for ck, r in by_coord.items():
        if r["category"] == "sign":
            print(json.dumps(r, ensure_ascii=False, indent=2)[:1500])
            break

    print("\nExample event record:")
    for ck, r in by_coord.items():
        if r["category"] == "event":
            print(json.dumps(r, ensure_ascii=False, indent=2)[:1500])
            break

    print("\nExample tile with overlays:")
    for ck, r in by_coord.items():
        if r.get("overlays"):
            print(f"  coord {ck}: primary={r['type']}, overlays={[o['type'] for o in r['overlays']]}")
            break


if __name__ == "__main__":
    main()
