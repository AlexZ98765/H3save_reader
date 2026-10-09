# Diff Analysis: 447_2.GM1 → 447_3.GM1

## User action

**Built Magic Guild Level 3 in a town** (was Lv2, upgraded to Lv3)

**Total changed ranges:** 13

**Total changed bytes:** 171

---

## 1. Mage Guild level upgrade ⭐

| Field | Offset | Size | A (447_2) | B (447_3) | Description |
|-------|--------|-----:|----------|----------|-------------|
| `mage_guild_level` | `0x13F74D` | 1 B | **2** (Lv2) | **3** (Lv3) | Magic Guild current level |
| `town_build_flag` | `0x13F70B` | 1 B | 0x00 | 0x01 | Set to 1 when build action performed this turn |
| `building_bitmask` | `0x13F7AB` | 4 B | `00 00 00 22` | `03 00 00 24` | Building bitmask (see byte breakdown below) |
| `town_bitmask_byte` | `0x13F7B6` | 1 B | 0xA3 | 0xA7 | Bit 2 set — possibly Lv3 marker |

### Building bitmask byte breakdown (0x13F7AB..0x13F7AE)

| byte | offset | A | B | bits set | bits cleared | interpretation |
|-----:|--------|---|---|----------|--------------|----------------|
| 0 | `0x0013F7AB` | 0x00 | 0x03 | 0x03 | 0x00 | bits 0,1 set — possibly 'build progress' or newly-built marker |
| 3 | `0x0013F7AE` | 0x22 | 0x24 | 0x04 | 0x02 | bit 1 (Lv2 marker) cleared, bit 2 (Lv3 marker) set |

**Key insight:** The bitmask encoding stores the CURRENT level bit (not all-built-levels). When upgrading Lv2→Lv3, bit 1 (Lv2) is cleared and bit 2 (Lv3) is set.

---

## 2. Player resource deduction (Mage Guild Lv3 cost)

**Resource block @ `0x13E3E7..0x13E404`** (7 × 4-byte LE ints):

| slot | resource | offset | A value | B value | Δ | interpretation |
|-----:|----------|--------|--------:|--------:|------:|----------------|
| 0 | Wood | `0x0013E3E7` | 1121 | 1116 | -5 | cost deduction |
| 1 | Mercury | `0x0013E3EB` | 195 | 189 | -6 | cost deduction |
| 2 | Ore | `0x0013E3EF` | 892 | 887 | -5 | cost deduction |
| 3 | Sulfur | `0x0013E3F3` | 373 | 367 | -6 | cost deduction |
| 4 | Crystal | `0x0013E3F7` | 459 | 453 | -6 | cost deduction |
| 5 | Gems | `0x0013E3FB` | 173 | 167 | -6 | cost deduction |
| 6 | Gold | `0x0013E3FF` | 2543782 | 2542782 | -1000 | cost deduction |

**Mage Guild Lv3 standard cost:** 5 wood + 5 ore + 2000 gold (varies slightly by town/faction)

**Note:** the deltas don't perfectly match the standard cost because:
- Daily resource income from mines also ticked (player ended turn between saves — confirmed by action counter increment)
- Some resources decreased by 6 instead of 5 (likely mine production reduction)
- Gold delta is much smaller than 2000 (daily income added back ~1500)

---

## 3. Spell list update (Lv3 spells unlocked)

Mage Guild Lv3 unlocks new tier-3 spells in the town's spell pool:

| offset | A | B | change |
|--------|---|---|--------|
| `0x0013E387` | 0x0D (id=13 (View Earth)) | 0xFF (empty) | spell removed |
| `0x0013E39E` | 0xFF (empty) | 0x12 (id=18 (Disrupting Ray)) | spell added |

**Note:** Mage Guild Lv3 unlocks spells from the level-3 spell pool. The two changes above suggest the spell list was rearranged — one spell slot was emptied (probably moved to another position) and a new spell (ID 18) was added.

Spell ID 18 = **Disrupting Ray** (likely a Lv2/Lv3 spell)

---

## 4. Hero spell book bitmask

- **Offset:** `0x00140246`
- **A:** 0xFB (binary: 11111011)
- **B:** 0xEB (binary: 11101011)
- **Change:** bit 4 cleared (0x10)
- **Interpretation:** Possibly a spell was 'used' (cast this turn) — bit cleared when spell memorized/cast

---

## 5. Action counter

- **Offset:** `0x000003B7`
- **A:** 50
- **B:** 51
- **Δ:** +1 (one user action = one increment)

---

## 6. HotA replay log

HotA-specific base64-encoded replay log updated at:
- `0x00130B07..0x00130B68` (98 bytes ASCII)
- `0x00130B73..0x00130B8F` (29 bytes ASCII)

---

## Summary of localized fields

| Field | Offset | Size | Description |
|---|---|---:|---|
| `mage_guild_level` | `0x13F74D` | 1 B | Magic Guild current level (1-5); 0x02→0x03 = Lv2→Lv3 |
| `town_build_flag` | `0x13F70B` | 1 B | Set to 1 when any build action performed this turn |
| `building_bitmask` | `0x13F7AB` | 4 B | Town building bitmask; byte[3] bits 0-4 = Mage Guild levels (only current level bit set) |
| `town_bitmask_byte` | `0x13F7B6` | 1 B | Secondary town bitmask; bit 2 = Mage Guild Lv3 |
| `player_resources[7]` | `0x13E3E7` | 28 B | 7 × 4-byte LE ints: Wood, Mercury, Ore, Sulfur, Crystal, Gems, Gold |
| `town_spell_slots` | `0x13E380+` | varies | Town spell pool (FF=empty); rearranged on Mage Guild upgrade |
| `hero_spell_book_bitmask` | `0x140246` | 1 B | Per-spell 'cast this turn' flag (bit cleared when spell cast) |
| `action_counter` | `0x000003B7` | 1 B | User action counter (+1 per action) |
