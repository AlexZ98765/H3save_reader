# Diff Analysis: 447_1.GM1 → 447_2.GM1

## User action between saves

1. **Swapped creatures in the army** — creatures moved between two of the 7 hero army slots
2. **Equipped an artifact from backpack onto the hero's doll** — backpack lost 1 artifact, doll gained 1 slot

**Total changed ranges:** 49

**Total changed bytes:** 274

---

## 1. Army swap (hero's 7 slots)

Hero army structure:
- `army_types[7]` @ `0x00143E52` (28 bytes, 7 × 4-byte creature IDs)
- `army_counts[7]` @ `0x00143E6E` (28 bytes, 7 × 4-byte creature counts)

| slot | type offset | count offset | A (447_1) | B (447_2) | Δ |
|------:|-------------|--------------|-----------|-----------|---|
| **0** | `0x00143E52` | `0x00143E6E` | type=empty, count=0 | type=id=9 (Centaur), count=200 | SWAP |
| 1 | `0x00143E56` | `0x00143E72` | type=id=3 (Wolf Rider), count=237 | (unchanged) | — |
| 2 | `0x00143E5A` | `0x00143E76` | type=id=11 (Dwarf), count=94 | (unchanged) | — |
| 3 | `0x00143E5E` | `0x00143E7A` | type=id=13 (Wood Elf), count=62 | (unchanged) | — |
| **4** | `0x00143E62` | `0x00143E7E` | type=id=9 (Centaur), count=200 | type=empty, count=0 | SWAP |
| 5 | `0x00143E66` | `0x00143E82` | type=id=443 (creature_id_443), count=3857903808 | (unchanged) | — |
| 6 | `0x00143E6A` | `0x00143E86` | type=id=175 (creature_id_175), count=3773098220 | (unchanged) | — |

**Interpretation:** creatures from slot 4 (creature_id=9, count=200) were moved to slot 0 (which was previously empty). This is the army swap.

## 2. Artifact equip (backpack → doll)

### Doll slot fill

- **Doll slot offset:** `0x00143FA7`
- **A (447_1):** `0xFFFFFFFF` (empty)
- **B (447_2):** `0x00000034` (Speculum)

### Backpack count decrement

- **Backpack count offset:** `0x00143DDD`
- **A (447_1):** 29 (29 artifacts in backpack)
- **B (447_2):** 28 (28 artifacts in backpack — one moved to doll)

### Backpack slot shift (cascade)

- **Range:** `0x0014400F .. 0x001440D3`
- **Stride:** 8 bytes per backpack slot (4-byte artifact_id + 4-byte data)
- **25 slots shifted** (first byte of each slot's artifact_id changed)

When an artifact is removed from the middle of the backpack, all subsequent slots shift by one position. Since artifact IDs fit in 1 byte (max 0xFF), only the first byte of each 4-byte ID changes:

| position | offset | A byte (artifact) | B byte (artifact) |
|----------:|--------|-------------------|-------------------|
| 0 | `0x0014400F` | 0x34 (Speculum) | 0x41 (Emblem of Cognizance) |
| 1 | `0x00144017` | 0x41 (Emblem of Cognizance) | 0x45 (Ring of the Wayfarer) |
| 2 | `0x0014401F` | 0x45 (Ring of the Wayfarer) | 0x55 (Hourglass of the Evil Hour) |
| 3 | `0x00144027` | 0x55 (Hourglass of the Evil Hour) | 0x3F (Bird of Perception) |
| 4 | `0x0014402F` | 0x3F (Bird of Perception) | 0x47 (Necklace of Ocean Guidance) |
| 5 | `0x00144037` | 0x47 (Necklace of Ocean Guidance) | 0x20 (Sandals of the Saint) |
| 6 | `0x0014403F` | 0x20 (Sandals of the Saint) | 0x26 (Red Dragon Flame Tongue) |
| 7 | `0x00144047` | 0x26 (Red Dragon Flame Tongue) | 0x2D (Still Eye of the Dragon) |
| 8 | `0x0014404F` | 0x2D (Still Eye of the Dragon) | 0x27 (Dragon Scale Shield) |
| 9 | `0x00144057` | 0x27 (Dragon Scale Shield) | 0x29 (Dragonbone Greaves) |
| ... | ... | ... | ... |

## 3. Side effects

### Day/step counter

| offset | A | B | Δ |
|---|---|---|---|
| `0x000003B7` | 49 | 50 | +1 |

### Hero alt blocks (stride 0x1E0 = 480 bytes)

- **10 hero alt-block changes** at stride [24, 264, 286]
- **Value:** 0x04 → 0x0C (delta=+8)
- **Interpretation:** 0x04 -> 0x0c (bit 2 set, bit 3 set) — possibly 'has been moved this turn' flag for each hero
- **First 5 offsets:** 0x001789AB, 0x00178AC9, 0x00178AE1, 0x00178BE9, 0x00178C01

### Path records added

### Replay log updated (HotA base64)

- **Range:** `0x00130B07 .. 0x00130B68` (98 bytes)
- **Interpretation:** HotA replay log (base64-encoded action sequence) updated for new turn

- **Range:** `0x00130B73 .. 0x00130B8F` (29 bytes)
- **Interpretation:** HotA replay log (base64-encoded action sequence) updated for new turn

### HD3 trailer zeroed

---

## Summary of localized fields

| Field | Offset | Size | Description |
|---|---|---:|---|
| `army_types[7]` | `0x00143E52` | 28 B | Hero army creature IDs (7 × 4-byte), 0xFFFFFFFF = empty |
| `army_counts[7]` | `0x00143E6E` | 28 B | Hero army creature counts (7 × 4-byte) |
| `doll_slot[N]` | `0x00143FA7` | 4 B | Equipped artifact slot on hero doll (0xFFFFFFFF = empty) |
| `backpack_count` | `0x00143DDD` | 1 B | Number of artifacts currently in backpack |
| `backpack_slots[N]` | `0x0014400F+` | 8 B each | Backpack artifact slots (4-byte ID + 4-byte data), shifted on remove |
| `day_or_step_counter` | `0x000003B7` | 1 B | Incremented by 1 per user action (49→50) |
| `hero_alt_flag` | `0x001789AB+0x1E0*N` | 1 B | Per-hero flag (0x04→0x0c, bits 2+3 set) |
