# Object Mapping — `Myth and Legend.h3m`

> Mapping of all **7009 map objects** to their `(x, y, z)` coordinates, types, sprite `.def` files, and per-type details, ready to be used by the GM1 save parser.

## Result

- **Map:** `Myth and Legend.h3m`
- **Total objects on map:** 7009 (6006 unique coords + 1003 overlays stacked on top)
- **Unique coordinate keys:** 6006
- **Tiles with multiple stacked objects:** 795 (1003 collisions total — handled via overlays)
- **Distinct object types:** 204
- **Sprites used:** 546 / 548

## Coordinate encoding (matches the GM1 save format)

In the decompressed save, every tile reference is encoded as a packed 24-bit word:

```text
coord_int = x | (y << 8) | (z << 16)
```

where `z = 0` is the surface, `z = 1` is the underground.

In the save file these 3 bytes are followed by a 4th "context" byte (e.g. `01` in town-record section, `00` in object-state array, `ff` for unused slots).

The script `verify_coord_mapping.py` confirms that **all 1751 non-decoration objects** (town / hero / mine / dwelling / monster / resource / artifact / treasure / visit_* / quest / teleport / garrison / sign / event / observatory / terrain_modifier) are found in the decompressed `312.GM1` save via this encoding — **100% match**.

## Files produced

| File | Description |
|------|-------------|
| `objects_by_coord.json` (1.8 MB) | `"x:y:z" → {coord_int, type, category, sprite_def, sprite_ref_id, object_index, details, overlays[]}`. The primary lookup. **6006 records** (one per unique tile). |
| `objects_flat.json` (2.1 MB) ⭐ | **Flat list of ALL 7009 objects** (no nesting). Each record has `is_primary` flag (true=main object on tile, false=overlay). Use for bulk iteration. |
| `coord_int_lookup.json` (128 KB) | `coord_int → "x:y:z"`. Use when you only have the 4-byte coord from the save and need to find the object record. |
| `type_index.json` (98 KB) | `type_name → [coord_key, ...]`. Find all towns / prisons / monoliths / etc. on the map. |
| `category_index.json` (95 KB) | `category → [coord_key, ...]`. Coarse grouping (21 buckets). |
| `object_mapping_summary.json` (5 KB) | Top-level counts. |
| `coord_mapping_verification.json` (556 KB) | Per-object verification: every match offset in the save. |
| `save_offset_clusters.json` (≈ 12 KB) | Offset clusters where coord-bytes appear in the decompressed save — reveals save sections. |
| `save_offset_clusters.md` | Human-readable cluster map. |
| `object_types_dictionary.json` ⭐ | **Editable** dictionary of 2037 object types from LazyLlama wiki (keyed by `.def` file name). Includes `user_edits` field for your customizations, preserved on rebuild. |
| `map_types_xref.json` ⭐ | Cross-reference: each map type name → its wiki entry (190/203 matched, 13 unmatched listed). |

## Two object-list formats

In HoMM3, a single tile can host multiple stacked objects (decorations layered over a "real" object). We provide two representations:

**`objects_by_coord.json`** — keyed by `"x:y:z"`. The primary object on each tile is the top-level record; additional objects on the same tile are listed in its `overlays[]` field. **6006 records total.** Use this for tile lookups by coord_int.

```python
import json
oc = json.load(open('objects_by_coord.json'))
# Lookup by coord_int
rec = oc[ci_lookup['879']]   # "111:3:0" — Tower "Кавала"
print(rec['type'], rec['overlays'])   # primary + list of stacked objects
```

**`objects_flat.json`** — plain array of all 7009 objects, each with `is_primary` flag. **No nesting.** Use this for bulk iteration/filtering.

```python
import json
flat = json.load(open('objects_flat.json'))['objects']
# All 7009 objects, no recursion needed
for o in flat:
    if o['category'] == 'town':
        print(o['x'], o['y'], o['type'], o['details'].get('town_name'))
```

## Category buckets

21 coarse categories, in priority order (used to choose the "primary" object on a tile with collisions):

| # | Category | Count | Examples |
|---|----------|------:|----------|
| 0 | town | 21 | Castle, Tower, Rampart, Inferno, Necropolis, Dungeon, Stronghold, Fortress, Conflux |
| 1 | hero | 6 | Prison |
| 2 | mine | 90 | Sawmill, Ore Pit, Gold Mine, Alchemist's Lab, Gem Pond, Crystal Cavern, Sulfur Dune |
| 3 | dwelling | 31 | Dwarf Cottage, Gnoll Hut, Homestead, Lizard Den, Parapet, Workshop, Barracks, ... |
| 4 | monster | 251 | Random Monster 1-7, named creature stacks (Crusader, Knight, Naga, ...) |
| 5 | bank | 7 | Naga Bank, Dragon Fly Hive, Dragon Utopia, Cyclops Stockpile, Shipwreck, ... |
| 6 | resource | 421 | Random Resource, Gold, Wood, Ore, Mercury, Sulfur, Crystal, Gems |
| 7 | artifact | 114 | Random Minor/Major/Treasure/Relic Artifact, Endless * resources, named artifacts |
| 8 | treasure | 319 | Treasure Chest, Campfire, Flotsam, Shipwreck Survivor |
| 9 | visit_skill | 103 | Learning Stone, Marletto Tower, Mercenary Camp, Star Axis, Garden of Revelation, Tree of Knowledge, School of Magic, School of War, Arena, Library of Enlightenment |
| 10 | visit_magic | 89 | Shrine of Magic Gesture/Thought/Incantation, Magic Well, Witch Hut, Scholar, Altar of Sacrifice, Sanctuary, Enchanted Spring |
| 11 | visit_other | 134 | Stables, Fountain of Youth, Fountain of Fortune, Faerie Ring, Idol of Fortune, Swan Pond, Temple, Rally Flag, Oasis, Mystical Garden, Sirens, Black Market, Trading Post, University, Cartographer, Eye of the Magi, Hut of the Magi, Redwood Observatory, Lighthouse |
| 12 | quest | 13 | Seer's Hut, Border Guard, Border Gate, Quest Guard, Hill Fort, Den of Thieves |
| 13 | teleport | 16 | Monolith Two Way, Monolith One Way Entrance/Exit, Subterranean Gate, Whirlpool |
| 14 | garrison | (none on this map) | Rampart, Garrison, Anti-Magic Garrison |
| 15 | shipyard | (none on this map) | Shipyard |
| 16 | boat | 15 | Boat |
| 17 | sign | 17 | Sign |
| 18 | event | 99 | Event (one-shot map event with rewards) |
| 19 | observatory | (subset of visit_other) | Redwood Observatory, Hut of the Magi, Eye of the Magi, Cartographer |
| 20 | terrain_modifier | 38 | Cursed Ground, Magic Plains |
| 99 | decoration | 5240 | Mountain, Trees, Rock, Cactus, Reef, Lava Flow, ... |

## Per-type `details` extraction

The `details` field of each record carries the most useful per-type fields from the `.h3m` `object_condition`:

| Object type | Extracted fields |
|-------------|------------------|
| Town (Castle/Tower/...) | `player_color`, `formation`, `town_name`, `building_set`, `spells_must`, `spells_disabled`, `as_player_color` |
| Prison | `player_color`, `hero_id`, `face`, `name`, `experience` |
| Resource / Random Resource | `resource_quantity`, `guarded` |
| Monster / Random Monster | `monster_quantity`, `disposition`, `monster_text` |
| Artifact / Treasure / guarded | `guarded`, `guard_monster_type`, `guard_monster_count`, `guard_character` |
| Witch Hut | `secondary_skills` (bitmask) |
| Scholar | `scholar_choice` |
| Sign | `message` (text) |
| Event | `experience`, `spell_points`, `morale`, `luck`, `resources`, `secondary_skills_gained`, `artifacts_gained`, `monsters_gained`, `players_applied`, `human_trigger` |
| Sawmill / Lighthouse / mine | `player_color` (top-level) |

## Save-section discovery (decompressed `312.GM1`)

The verification script finds **all 1751 non-decoration objects** in the decompressed save. The match offsets cluster into well-defined sections:

| Offset cluster | Hits | Likely save-section |
|---|---:|---|
| **`0x120C8C`** | **7269** | Map object-state array (one record per map object, including decorations) |
| `0x118C1C` | 616 | Visiting-objects array (per-hero / per-town visited list) |
| `0x172129` | 576 | Passability / decoration-shadow bitmask (decorations only) |
| `0x9D892` | 283 | Mixed — possibly "alive objects" overlay |
| `0x7E9F7` | 293 | Mixed — possibly fog-of-war / discovered-objects array |
| `0xADB82` | 226 | Mixed — treasure/visit_other heavy |
| `0x17F812`–`0x180ADD` | 230 | End-of-save object-state patch |

The dominant cluster at `0x120C8C` (≈ 7269 hits, very close to 7009 unique objects) is the **per-object state array** — this is where the save stores runtime state (visited flag, owner override, garrison, etc.) for every map object.

## Usage examples

```python
import json

# Load mappings (paths relative to this toolkit archive)
oc        = json.load(open('03_object_mapping/objects_by_coord.json'))
ci_lookup = json.load(open('03_object_mapping/coord_int_lookup.json'))
type_idx  = json.load(open('03_object_mapping/type_index.json'))
cat_idx   = json.load(open('03_object_mapping/category_index.json'))

# 1) Decode a coord_int read from a save (e.g. 0x36F = 879 = "Кавала" town)
coord_key = ci_lookup['879']   # "111:3:0"
print(oc[coord_key])
# -> {type: "Tower", category: "town", details: {town_name: "Кавала", ...}}

# 2) Find all towns on the map
for ck in cat_idx['town']:
    print(ck, oc[ck]['type'], oc[ck]['details'].get('town_name'))

# 3) Find all Prisons
for ck in type_idx.get('Prison', []):
    print(ck, oc[ck]['details'].get('hero_id'))

# 4) Iterate over every monster on the map
for ck in cat_idx['monster']:
    rec = oc[ck]
    print(f"{ck}  {rec['type']:25s}  count={rec['details'].get('monster_quantity')}")
```

## Scripts

| Script | Description |
|--------|-------------|
| `04_diff_analysis/build_object_mapping.py` | Build all mapping JSONs from `Myth and Legend.h3m.json`. |
| `04_diff_analysis/verify_coord_mapping.py` | Verify the mapping against the decompressed `312.GM1` save. |
| `04_diff_analysis/analyze_save_clusters.py` | Group coord-bytes occurrences in the save into offset clusters — reveals save sections. |
