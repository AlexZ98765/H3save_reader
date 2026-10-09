# Diff Analysis: 0020_9 → 0020_10 — Hero Hire in Tavern

## User action
**Hired a hero in a tavern** (hero appears at town, gold deducted)

## Key Findings

### 1. File size is THE SAME (1,572,864 bytes)

⭐ **CRITICAL DISCOVERY:** The save file has **PRE-ALLOCATED SPACE** for all possible heroes.

- 0020_9:  1,572,864 bytes
- 0020_10: 1,572,864 bytes
- Delta: **0 bytes**

**Implication:** Hiring a hero does NOT add new data to the save. Instead, it **fills an existing empty slot** (marked with `0xFF` bytes). This means:
- Hero blocks are at **fixed positions** in the save
- Empty hero slots are filled with `0xFF`
- The number of hero slots is fixed per map (not per save)
- This makes parsing MUCH easier — hero block positions are predictable!

### 2. New hero appears at coords (109, 106, 0)

The new hero block at `0x162DF9`:
```
Before (0020_9):  ff ff ff ff ff ff 00 00 00 00 00 00 00 00 00 00 00
After  (0020_10): 6d 00 6a 00 00 00 01 6d 00 6a 00 62 00 00 00 01 12
```

Decoded:
- `6d 00 6a 00 00 00 01` = coords (109, 106, 0) + flag `01` (hero is on map)
- `6d 00 6a 00 62 00 00 00 01 12` = second coord reference (109, 106, 98) + flag

**Town at (111, 106, 0)** = Castle "Афина" — the hero was hired here and appears just outside the town at (109, 106, 0).

### 3. Gold deducted by 2500 (standard hero hire cost)

```
0x13E481: u32 LE: 21579 -> 19079 (delta = -2500)
```

**Standard hero hire cost = 2500 gold** ✅ confirmed exactly.

### 4. Hero ID array changed at 0x14B232

```
Before: ... 1c 00 00 00 1e 00 00 00 20 00 00 00 ff ff ff ff ...
After:  ... 1c 00 00 00 ff ff ff ff ff ff ff ff ff ff ff ff ...
                                          ↑ cleared (hero ID 0x1e=30 removed)
```

And at `0x14B24A`:
```
Before: 21 00 00 00 04 00 00 00 02
After:  01 00 00 00 00 00 00 00 00
```

This appears to be a **hero availability array** — when a hero is hired, their ID is removed from the available pool.

### 5. Path record added (visit tavern)

At `0x1819AF`:
```
Before: 0b 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
After:  09 03 81 00 00 00 03 ff 6d 00 6a 00 ff 03 ff 3f 00 00 0b
```

**Path record type `09 03`** = "visit object" (tavern)
- Coords: `6d 00 6a 00` = (109, 106) — matches the town location

### 6. Hero spell book bitmask changed

At `0x140263`: `ff` → `81` — new hero's spell book slot initialized

### 7. Hero alt-block flags changed

At `0x16A183`: `ff` → `40` and `0x16A1DC`: `40` → `03`
- These are in the hero alt-block area (per-hero flags at stride 0x1E0)

## Localized Fields Summary

| Field | Offset | Size | Change | Description |
|-------|--------|------|--------|-------------|
| `hero_coords[N]` | `0x162DF9` | 6 | `ff ff ff ff ff ff` → `6d 00 6a 00 00 00` | Hero coords (x=109, y=106, z=0) as u16 pairs |
| `hero_on_map_flag[N]` | `0x162DFF` | 1 | `00` → `01` | Hero is now on the map (1=on map, 0=not hired) |
| `hero_secondary_coord[N]` | `0x163000` | 7 | `00 00...` → `6d 00 6a 00 62 00 00` | Secondary coord reference (town entrance?) |
| `hero_id_array` | `0x14B232` | 8 | `1e 00 00 00 20 00 00 00` → `ff ff ff ff ff ff ff ff` | Hero ID 0x1E removed from available pool |
| `hero_availability` | `0x14B24A` | 9 | `21 00 00 00 04 00 00 00 02` → `01 00 00 00 00 00 00 00 00` | Hero availability flags updated |
| `player_gold` | `0x13E481` | 4 | `21579` → `19079` (u32 LE) | Gold decreased by 2500 (hero hire cost) |
| `hero_spell_book[N]` | `0x140263` | 1 | `ff` → `81` | New hero's spell book initialized |
| `hero_alt_flag[N]` | `0x16A183` | 1 | `ff` → `40` | Hero alt-block flag set |
| `hero_alt_flag[N+1]` | `0x16A1DC` | 1 | `40` → `03` | Adjacent hero alt-block flag updated |
| `path_record_visit` | `0x1819AF` | 19 | `00...` → `09 03 81 00...` | Path record: visit tavern at (109, 106) |
| `movement_points[N]` | `0x162E32` | 4 | `0x6E0` → `0x870` (1760 → 2160) | New hero's movement points |
| `movement_points[N]` | `0x163015` | 2 | `0x6E0` → `0x870` | Mirror of movement points |

## Critical Insight for Universal Parsing

### The save has FIXED-SIZE slots for heroes

This is a **game-changer** for universal parsing:

1. **Hero blocks are at fixed positions** — the save pre-allocates space for ALL possible heroes (likely 156 slots = max heroes per map)
2. **Empty slots are filled with `0xFF`** — easy to detect
3. **Hiring a hero fills a slot** — no data shifting needed
4. **Hero block positions are predictable** — can be computed from map data

### What this means for the parser

To find hero blocks in ANY save:
1. Scan the hero area (approximately `0x140000..0x170000` for this map)
2. Look for non-`0xFF` blocks — these are hired heroes
3. Each hero block starts with 6 bytes of coords (3 × u16: x, y, z) + 1 byte flag
4. If flag = `0x01`, hero is on map; if `0xFF`, slot is empty

### Next steps

To determine the EXACT hero block size and stride:
- Need to find TWO filled hero slots and measure the distance between them
- Or: find the start of the hero section and count slots
- The h3sed tool reports hero block size ~1122 bytes — need to verify

## Files

- `04_diff_analysis/analyze_hero_hire_diff_report.json` — machine-readable report
- `04_diff_analysis/analyze_hero_hire_diff_report.md` — this document
