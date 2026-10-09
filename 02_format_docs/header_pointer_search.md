# Header Pointer Search — Findings & Strategy

## Goal
Find how to locate hero blocks, town records, and object array in ANY save file (not just "Myth and Legend.h3m" saves).

## Key Discovery: NO direct pointers in header

**Step 1 result:** Searched for known save offsets (hero_block=0x157F1E, object_array=0x120C8C, visiting_array=0x118C1C) as 4-byte LE values in the first 2048 bytes of decompressed saves. **Zero matches.**

**Conclusion:** The save format does NOT store absolute pointers to sections. The structure is **sequential** — each section follows the previous one, and section sizes must be computed from earlier data.

## Decoded header structure (312.GM1)

```
0x000..0x004: "H3SVG" magic (5 bytes)
0x005..0x007: padding (3 bytes)
0x008..0x00B: version_major = 42 (u32 LE)
0x00C..0x00F: version_minor = 2 (u32 LE)
0x010..0x02F: 32 bytes — game state (day/week/month/difficulty, mostly zeros in this save)
0x030..0x033: map_type = 28 (SoD) (u32 LE)  ← matches .h3m map_type
0x034:       has_underground = 1 (u8)
0x035..0x038: map_size = 144 (u32 LE)
0x039:       is_playable = 1 (u8)
0x03A..0x03B: name_length = 14 (u16 LE)
0x03C..0x049: map name "Мифы и легенды" (14 bytes, cp1251)
0x04A..0x04B: description_length = 281 (u16 LE)
0x04C..0x160: description (281 bytes, cp1251)
0x165..0x235: player setup data (player names, factions, etc.)
0x236..0x249: "Myth and Legend.h3m" (map filename, 19 bytes)
0x24A..0x3B2: zeros + "maps\all\SoD\XL" path + zeros (fixed per map)
0x3B3..0x3BC: save filename (variable, e.g. "BATTLE" or "447_1.GM1")
0x3BD..:      game state sections begin
```

## Section offsets (312.GM1, "Myth and Legend.h3m")

| Section | Start offset | How determined |
|---------|-------------|----------------|
| Header | 0x000 | Fixed (H3SVG magic) |
| Map filename | 0x236 | Found via string search |
| Save filename | 0x3B3 | First divergence between 312 vs 447 |
| Player resources | ~0x13E3E7 | From 447_2→447_3 diff (Mage Guild cost) |
| Object owners | ~0x13C850 | From 0020_1→0020_2 diff (mine capture) |
| Town records (447 series) | ~0x13F700 | From 447_2→447_3 diff (Mage Guild Lv3) |
| Visiting array | 0x118C1C | From coord-bytes cluster analysis |
| Main object array | 0x120C8C | From coord-bytes cluster analysis (7269 hits ≈ 7009 objects) |
| Hero block (312 series) | 0x157F1E | From h3sed (faction byte) |
| Hero block (447 series) | 0x143DC0 | From 447_1→447_2 diff (army swap) |
| Decoration bitmask | 0x172129 | From coord-bytes cluster analysis |

## Why hero block start differs between 312 and 447

- 312.GM1: hero block at 0x157F1E
- 447_1.GM1: hero block at 0x143DC0
- Difference: 0x1415E bytes

This means sections between header and hero block have **variable size** that depends on game state. The size difference is likely due to:
- Different number of path records (replay log)
- Different amount of fog-of-war data discovered
- Different visiting-objects history

## Strategy for universal parsing

Since there are no direct pointers, we must **sequentially parse** each section:

1. **Parse header** (fixed structure, see above)
2. **Parse player state** (need to determine size — likely fixed per map_size)
3. **Parse object owners array** (size = n_objects × 1 byte? n_objects not in header)
4. **Parse town records** (size = n_towns × town_record_size)
5. **Parse hero blocks** (size = n_heroes × hero_block_size)
6. **Parse visiting array** (size = ?)
7. **Parse main object array** (size = ?)
8. **Parse path block** (variable, replay log)
9. **Parse decoration bitmask** (size = map_size² × 2 levels / 8 bits)

## What we need to determine sizes

### Option A: Differential analysis (recommended)
Make saves before/after specific actions:
- **Hire a hero** → hero block count changes → find size delta
- **Build a town structure** → town record changes → find size delta
- **Capture a mine** → object owner changes → find size of owner array
- **Visit an object** → visiting array changes → find size of visiting array

### Option B: Statistical analysis
Compare saves from DIFFERENT maps:
- Different map_size → sections that scale with map_size
- Different n_objects → sections that scale with n_objects
- Different n_heroes → sections that scale with n_heroes

### Option C: Disassembly analysis
Read `func_SAVE_READER_CONTENT` more carefully to understand the order of `ReadFile` calls and what sizes they read.

## Next small steps (evolutionary)

### Step 4 (next): Determine player state section size
- Compare header region 0x3BD..0x10000 between 312 and 447
- Find where player state ends and next section begins
- Player resources are at 0x13E3E7 (deep in save), so player state is large

### Step 5: Determine object owners array size
- We know object owners start ~0x13C850 (from mine capture diff)
- We know main object array starts at 0x120C8C
- Object owners array is BETWEEN these? Or before?
- Need to find the boundary

### Step 6: Determine visiting array size
- Visiting array starts at 0x118C1C
- Need to find where it ends (before main object array at 0x120C8C)
- Size = 0x120C8C - 0x118C1C = 0x8070 = 32880 bytes
- If 7009 objects × 4 bytes = 28036 — doesn't match
- If 41472 tiles (2×144²) × 4 bytes = 165888 — too big
- Need to investigate structure

### Step 7: Determine main object array structure
- Starts at 0x120C8C
- Each record starts with 3 coord bytes + 1 flag byte
- Need to determine record size (variable per object type?)

## What the user can do to help

1. **Make a save before/after hiring a hero** — this will reveal hero block size
2. **Make a save from a DIFFERENT map** — even if "жёстко по-другому", comparing headers will show which fields are map-dependent
3. **Make a save before/after dismissing a hero** — opposite of hiring, will confirm hero block size

## Files
- `04_diff_analysis/header_pointer_search_report.json` — raw search results
- This document: `02_format_docs/header_pointer_search.md`
