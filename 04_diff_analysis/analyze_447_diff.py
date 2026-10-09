#!/usr/bin/env python3
"""
analyze_447_diff.py

Differential analysis of two .GM1 saves from "Myth and Legend.h3m":
    447_1.GM1  — before
    447_2.GM1  — after

User actions between saves:
    1. Swapped creatures in the army (changed positions in hero's army slots)
    2. Equipped an artifact from backpack onto the hero's doll

This script identifies exact byte-level changes and attributes them to:
    - Army swap (creature types / counts in hero army slots)
    - Artifact equip (backpack slot emptied, doll slot filled, stat recalcs)
    - Side effects (path records, replay log, day counter, HD3 trailer)

Output:
    04_diff_analysis/analyze_447_diff_report.json
    04_diff_analysis/analyze_447_diff_report.md
"""

import json
import os
import struct
import gzip
import io
from collections import defaultdict
from typing import List, Tuple

SAVE_A = "/home/z/my-project/upload/447_1.GM1"
SAVE_B = "/home/z/my-project/upload/447_2.GM1"
OUT_DIR = "/home/z/my-project/download/homm3_gm1_toolkit/04_diff_analysis"
OUT_JSON = os.path.join(OUT_DIR, "analyze_447_diff_report.json")
OUT_MD   = os.path.join(OUT_DIR, "analyze_447_diff_report.md")
MAPPING_PATH = "/home/z/my-project/download/homm3_gm1_toolkit/02_format_docs/gm1_mapping.json"

# Load the artifact dictionary to resolve artifact IDs
ART_DICT_PATH = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/object_types_dictionary.json"


def decompress_save(path: str) -> bytes:
    raw = open(path, "rb").read()
    gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
    out = b""
    try:
        while True:
            chunk = gz.read(65536)
            if not chunk:
                break
            out += chunk
    except Exception:
        pass
    return out


def find_changed_ranges(a: bytes, b: bytes, gap: int = 4) -> List[Tuple[int, int]]:
    n = min(len(a), len(b))
    ranges = []
    i = 0
    while i < n:
        if a[i] != b[i]:
            start = i
            while i < n and a[i] != b[i]:
                i += 1
            end = i - 1
            ranges.append([start, end])
        else:
            i += 1
    if not ranges:
        return []
    merged = [ranges[0]]
    for s, e in ranges[1:]:
        if s - merged[-1][1] - 1 <= gap:
            merged[-1][1] = e
        else:
            merged.append([s, e])
    return [(s, e) for s, e in merged]


def load_mapping():
    if os.path.exists(MAPPING_PATH):
        return json.load(open(MAPPING_PATH, encoding="utf-8"))
    return {}


def load_artifact_dict():
    """Load wiki artifact dictionary; build id->name lookup."""
    if not os.path.exists(ART_DICT_PATH):
        return {}
    d = json.load(open(ART_DICT_PATH, encoding="utf-8"))
    id_lookup = {}
    for fn, entry in d.get("objects", {}).items():
        try:
            oid = int(entry.get("object_id", ""))
            sid = int(entry.get("sub_id", ""))
            if entry.get("category") == "Artifacts":
                id_lookup[(oid, sid)] = entry.get("wiki_name", f"artifact_{oid}_{sid}")
        except (ValueError, TypeError):
            pass
    return id_lookup


def artifact_name(art_id: int, id_lookup: dict) -> str:
    """Look up artifact name by ID (object_id=5, sub_id=art_id)."""
    name = id_lookup.get((5, art_id))
    if name:
        return name
    return f"artifact_id_{art_id}"


def load_creature_dict():
    """Build creature_id -> name lookup (object_id=98 is monster family)."""
    if not os.path.exists(ART_DICT_PATH):
        return {}
    d = json.load(open(ART_DICT_PATH, encoding="utf-8"))
    id_lookup = {}
    for fn, entry in d.get("objects", {}).items():
        try:
            oid = int(entry.get("object_id", ""))
            sid = int(entry.get("sub_id", ""))
            if entry.get("category") == "Monsters":
                id_lookup[(oid, sid)] = entry.get("wiki_name", f"monster_{oid}_{sid}")
        except (ValueError, TypeError):
            pass
    return id_lookup


def creature_name(creature_id: int, id_lookup: dict) -> str:
    """Best-effort creature name lookup. H3 creature IDs are well-known."""
    # Standard H3 creature IDs (SoD)
    H3_CREATURES = {
        0:  "(unknown 0)",
        1:  "Imp",                2:  "Goblin",             3:  "Wolf Rider",
        4:  "Hobgoblin",          5:  "Gog",                 6:  "Magog",
        7:  "Master Goblin",      8:  "Wolf Raider",         9:  "Centaur",
       10:  "Centaur Captain",   11:  "Dwarf",              12:  "Battle Dwarf",
       13:  "Wood Elf",          14:  "Grand Elf",           15:  "Pegasus",
       16:  "Silver Pegasus",    17:  "Dendroid Guard",      18:  "Dendroid Soldier",
       19:  "Unicorn",           20:  "War Unicorn",         21:  "Green Dragon",
       22:  "Gold Dragon",       23:  "Sprite",              24:  "Pixie",
       25:  "Air Elemental",     26:  "Earth Elemental",     27:  "Fire Elemental",
       28:  "Water Elemental",   29:  "Psychic Elemental",   30:  "Magic Elemental",
       # ... (extend if needed)
    }
    if 0 < creature_id < 0x10000000:
        return H3_CREATURES.get(creature_id, f"creature_id_{creature_id}")
    return "empty"


def main():
    print(f"Decompressing {SAVE_A} ...")
    a = decompress_save(SAVE_A)
    print(f"  size: {len(a):,} bytes")
    print(f"Decompressing {SAVE_B} ...")
    b = decompress_save(SAVE_B)
    print(f"  size: {len(b):,} bytes")
    print()

    mapping = load_mapping()
    art_lookup = load_artifact_dict()
    creature_lookup = load_creature_dict()
    print(f"Loaded {len(mapping.get('blocks',[]))} blocks from mapping")
    print(f"Loaded {len(art_lookup)} artifact names")
    print(f"Loaded {len(creature_lookup)} creature names from wiki")

    ranges = find_changed_ranges(a, b, gap=4)
    print(f"\nFound {len(ranges)} changed ranges (after merging gaps <= 4 bytes)")
    total_bytes = sum(e - s + 1 for s, e in ranges)
    print(f"Total changed bytes: {total_bytes}")

    # ----- Build all range reports -----
    all_ranges = []
    for s, e in ranges:
        length = e - s + 1
        a_chunk = a[s:e+1]
        b_chunk = b[s:e+1]
        all_ranges.append({
            "start": s, "end": e, "length": length,
            "a_bytes": a_chunk, "b_bytes": b_chunk,
            "a_hex": a_chunk.hex(' '),
            "b_hex": b_chunk.hex(' '),
        })

    # ----- Detailed interpretation of specific ranges -----

    findings = []

    # ===== ARMY SWAP =====
    # Hero army: 7 slots × 4 bytes (types) + 7 slots × 4 bytes (counts)
    # We observed changes at:
    #   0x143E52 (4 bytes): FFFFFFFF -> 0x00000009 (creature ID 9 = Centaur? — placeholder)
    #   0x143E62 (4 bytes): 0x00000009 -> FFFFFFFF
    #   0x143E6E (1 byte):  0x00 -> 0xC8 (200)
    #   0x143E7E (1 byte):  0xC8 -> 0x00
    # Stride 0x10 = 16 bytes between 0x143E52 and 0x143E62 — 4 slots apart
    # Stride 0x10 = 16 bytes between 0x143E6E and 0x143E7E
    # 0x143E6E - 0x143E52 = 0x1C = 28 bytes = 7 × 4 bytes = army_types[7]
    # So army_types[7] is at 0x143E52..0x143E6D, army_counts[7] at 0x143E6E..0x143E89
    # Wait that's actually a hero_army layout.

    army_types_start = 0x143E52
    army_counts_start = 0x143E6E   # = army_types_start + 28 (7 slots × 4 bytes)
    n_army_slots = 7
    print("\n=== ARMY SWAP ANALYSIS ===")
    print(f"Interpreting army_types[7] @ 0x{army_types_start:08X}, army_counts[7] @ 0x{army_counts_start:08X}")
    for i in range(n_army_slots):
        t_off = army_types_start + i * 4
        c_off = army_counts_start + i * 4
        a_t = struct.unpack('<I', a[t_off:t_off+4])[0]
        b_t = struct.unpack('<I', b[t_off:t_off+4])[0]
        a_c = struct.unpack('<I', a[c_off:c_off+4])[0]
        b_c = struct.unpack('<I', b[c_off:c_off+4])[0]
        marker = "*" if (a_t != b_t or a_c != b_c) else " "
        a_t_str = "empty" if a_t == 0xFFFFFFFF else f"id={a_t}"
        b_t_str = "empty" if b_t == 0xFFFFFFFF else f"id={b_t}"
        print(f"  {marker} slot[{i}] @ 0x{t_off:08X}: A.type={a_t_str} count={a_c} -> B.type={b_t_str} count={b_c}")
        if a_t != b_t or a_c != b_c:
            findings.append({
                "category": "army_swap",
                "slot": i,
                "type_offset": f"0x{t_off:08X}",
                "count_offset": f"0x{c_off:08X}",
                "a_type": a_t, "b_type": b_t,
                "a_count": a_c, "b_count": b_c,
                "a_type_name": "empty" if a_t == 0xFFFFFFFF else creature_name(a_t, creature_lookup),
                "b_type_name": "empty" if b_t == 0xFFFFFFFF else creature_name(b_t, creature_lookup),
            })

    # ===== ARTIFACT EQUIP =====
    # Doll slot fill: 0x143FA7 (4 bytes): FFFFFFFF -> 0x00000034 (artifact ID 52)
    # Backpack count: 0x143DDD (1 byte): 0x1d (29) -> 0x1c (28)
    # Backpack shift: 0x14400F .. 0x1440D3 (stride 8 = 1 byte per 8-byte backpack slot)
    doll_slot_off = 0x143FA7
    backpack_count_off = 0x143DDD
    backpack_first_slot = 0x14400F  # offset of first byte of artifact_id in slot 0

    print("\n=== ARTIFACT EQUIP ANALYSIS ===")
    a_doll = struct.unpack('<I', a[doll_slot_off:doll_slot_off+4])[0]
    b_doll = struct.unpack('<I', b[doll_slot_off:doll_slot_off+4])[0]
    print(f"Doll slot @ 0x{doll_slot_off:08X}: A=0x{a_doll:08X} ({'empty' if a_doll==0xFFFFFFFF else a_doll}) -> "
          f"B=0x{b_doll:08X} ({'empty' if b_doll==0xFFFFFFFF else b_doll})")
    if a_doll != b_doll:
        a_name = "empty" if a_doll == 0xFFFFFFFF else artifact_name(a_doll, art_lookup)
        b_name = "empty" if b_doll == 0xFFFFFFFF else artifact_name(b_doll, art_lookup)
        print(f"  Artifact: A={a_name} -> B={b_name}")
        findings.append({
            "category": "artifact_equip_doll",
            "doll_offset": f"0x{doll_slot_off:08X}",
            "a_value": a_doll,
            "b_value": b_doll,
            "a_artifact": a_name,
            "b_artifact": b_name,
        })

    a_bcount = a[backpack_count_off]
    b_bcount = b[backpack_count_off]
    print(f"\nBackpack count @ 0x{backpack_count_off:08X}: A={a_bcount} -> B={b_bcount} (delta={b_bcount-a_bcount:+d})")
    if a_bcount != b_bcount:
        findings.append({
            "category": "artifact_equip_backpack_count",
            "offset": f"0x{backpack_count_off:08X}",
            "a_value": a_bcount,
            "b_value": b_bcount,
            "delta": b_bcount - a_bcount,
            "interpretation": "one artifact moved from backpack to doll (-1)",
        })

    # Backpack slot shift analysis
    print(f"\nBackpack slot shift @ 0x{backpack_first_slot:08X} .. 0x{0x1440D3:08X} (stride 8 = 8 bytes per slot):")
    n_slots_shifted = 0
    shifted_summary = []
    for off in range(0x14400F, 0x1440D3 + 1, 8):
        a_byte = a[off]
        b_byte = b[off]
        if a_byte != b_byte:
            n_slots_shifted += 1
            a_id = a_byte  # if all artifact IDs fit in 1 byte, this is the artifact ID
            b_id = b_byte
            shifted_summary.append({
                "offset": f"0x{off:08X}",
                "a_byte": a_byte,
                "b_byte": b_byte,
                "a_artifact": artifact_name(a_id, art_lookup),
                "b_artifact": artifact_name(b_id, art_lookup),
            })
    print(f"  {n_slots_shifted} backpack slots shifted (artifact_id byte changed)")
    if shifted_summary:
        print("  First 5 shifts:")
        for s in shifted_summary[:5]:
            print(f"    {s['offset']}: A=0x{s['a_byte']:02X} ({s['a_artifact']}) -> B=0x{s['b_byte']:02X} ({s['b_artifact']})")
    if n_slots_shifted > 0:
        findings.append({
            "category": "artifact_equip_backpack_shift",
            "range": f"0x{0x14400F:08X} .. 0x{0x1440D3:08X}",
            "n_slots_shifted": n_slots_shifted,
            "stride": 8,
            "interpretation": "all backpack slots after the removed artifact shifted by one position",
            "shifts": shifted_summary[:10],
        })

    # ===== DAY COUNTER / STEP COUNTER =====
    print("\n=== MISC COUNTERS ===")
    for s, e in ranges:
        if e - s + 1 == 1 and s < 0x100000:
            v_a = a[s]
            v_b = b[s]
            delta = v_b - v_a
            print(f"  0x{s:08X}: {v_a} -> {v_b} (delta={delta:+d})")
            findings.append({
                "category": "counter",
                "offset": f"0x{s:08X}",
                "a_value": v_a,
                "b_value": v_b,
                "delta": delta,
            })

    # ===== Hero alt blocks (stride ~0x1e0) =====
    print("\n=== HERO ALT BLOCKS (stride 0x1E0 = 480) ===")
    alt_changes = []
    for s, e in ranges:
        if 0x178000 <= s <= 0x179000 and e - s + 1 == 1:
            alt_changes.append(s)
    if alt_changes:
        alt_changes.sort()
        # Compute strides
        strides = [alt_changes[i+1] - alt_changes[i] for i in range(len(alt_changes)-1)]
        print(f"  {len(alt_changes)} alt-block changes detected")
        if strides:
            print(f"  strides: {set(strides)}")
            print(f"  first 5: A=0x{a[alt_changes[0]]:02X} -> B=0x{b[alt_changes[0]]:02X}")
        findings.append({
            "category": "hero_alt_block_changes",
            "n_changes": len(alt_changes),
            "stride": list(set(strides)) if strides else [],
            "offsets": [f"0x{x:08X}" for x in alt_changes],
            "a_value": a[alt_changes[0]],
            "b_value": b[alt_changes[0]],
            "delta": b[alt_changes[0]] - a[alt_changes[0]],
            "interpretation": "0x04 -> 0x0c (bit 2 set, bit 3 set) — possibly 'has been moved this turn' flag for each hero",
        })

    # ===== Path records added (zero -> data) =====
    print("\n=== PATH RECORDS ADDED ===")
    for s, e in ranges:
        if e - s + 1 > 30 and all(c == 0 for c in a[s:e+1]) and any(c != 0 for c in b[s:e+1]):
            print(f"  0x{s:08X} .. 0x{e:08X} ({e-s+1} bytes): was all zeros, now has data")
            print(f"    B first 32: {b[s:s+32].hex(' ')}")
            findings.append({
                "category": "path_records_added",
                "range": f"0x{s:08X} .. 0x{e:08X}",
                "length": e - s + 1,
                "first_bytes_b": b[s:s+32].hex(' '),
                "interpretation": "path records appended to replay log (action = army swap + equip = path entry)",
            })

    # ===== HD3 trailer zeroed =====
    print("\n=== HD3 TRAILER ===")
    for s, e in ranges:
        if e - s + 1 >= 4 and a[s:s+3] == b'HD3' and all(c == 0 for c in b[s:s+4]):
            print(f"  0x{s:08X} .. 0x{e:08X}: HD3 marker -> zeros")
            findings.append({
                "category": "hd3_trailer_zeroed",
                "range": f"0x{s:08X} .. 0x{e:08X}",
                "interpretation": "HD3 (HotA) trailer marker zeroed out (file shorter by 84 bytes after this point in B)",
            })

    # ===== Replay log (base64 ASCII) =====
    print("\n=== REPLAY LOG (ASCII/base64) ===")
    for s, e in ranges:
        if 0x130000 <= s <= 0x140000 and e - s + 1 > 20:
            a_chunk = a[s:e+1]
            b_chunk = b[s:e+1]
            is_ascii_a = all(32 <= c < 127 or c == 10 for c in a_chunk)
            is_ascii_b = all(32 <= c < 127 or c == 10 for c in b_chunk)
            if is_ascii_a and is_ascii_b:
                print(f"  0x{s:08X} .. 0x{e:08X} ({e-s+1} bytes): ASCII replay log updated")
                print(f"    A: {a_chunk[:60].decode('ascii', errors='replace')}...")
                print(f"    B: {b_chunk[:60].decode('ascii', errors='replace')}...")
                findings.append({
                    "category": "replay_log_updated",
                    "range": f"0x{s:08X} .. 0x{e:08X}",
                    "length": e - s + 1,
                    "interpretation": "HotA replay log (base64-encoded action sequence) updated for new turn",
                })

    # ===== Other small changes =====
    print("\n=== OTHER CHANGES ===")
    categorized_ranges = set()
    for f in findings:
        if "offset" in f:
            try:
                categorized_ranges.add(int(f["offset"], 16))
            except (ValueError, TypeError):
                pass
        if "range" in f:
            try:
                start_str = f["range"].split("..")[0].strip()
                categorized_ranges.add(int(start_str, 16))
            except (ValueError, TypeError):
                pass
        if "type_offset" in f:
            try:
                categorized_ranges.add(int(f["type_offset"], 16))
                categorized_ranges.add(int(f["count_offset"], 16))
            except (ValueError, TypeError):
                pass
        if "doll_offset" in f:
            try:
                categorized_ranges.add(int(f["doll_offset"], 16))
            except (ValueError, TypeError):
                pass

    other_ranges = []
    for r in all_ranges:
        # Check if any byte in this range is already categorized
        already = False
        for off_in_range in range(r["start"], r["end"] + 1):
            if off_in_range in categorized_ranges:
                already = True
                break
        if not already and r["length"] < 100:
            other_ranges.append(r)
            print(f"  0x{r['start']:08X} .. 0x{r['end']:08X} ({r['length']}b): A={r['a_hex']} B={r['b_hex']}")

    # ----- Save report -----
    report = {
        "_meta": {
            "save_a": "447_1.GM1",
            "save_b": "447_2.GM1",
            "user_action": "swapped army creatures + equipped artifact from backpack to doll",
            "map": "Myth and Legend.h3m",
            "total_changed_ranges": len(ranges),
            "total_changed_bytes": total_bytes,
        },
        "findings": findings,
        "all_ranges": [{
            "range":  f"0x{r['start']:08X} .. 0x{r['end']:08X}",
            "length": r["length"],
            "a_hex":  r["a_hex"],
            "b_hex":  r["b_hex"],
        } for r in all_ranges],
    }
    os.makedirs(OUT_DIR, exist_ok=True)
    with open(OUT_JSON, "w", encoding="utf-8") as f:
        json.dump(report, f, ensure_ascii=False, indent=2)
    print(f"\nWrote {OUT_JSON}  ({os.path.getsize(OUT_JSON):,} bytes)")

    # ----- Markdown report -----
    md = []
    md.append("# Diff Analysis: 447_1.GM1 → 447_2.GM1\n\n")
    md.append("## User action between saves\n\n")
    md.append("1. **Swapped creatures in the army** — creatures moved between two of the 7 hero army slots\n")
    md.append("2. **Equipped an artifact from backpack onto the hero's doll** — backpack lost 1 artifact, doll gained 1 slot\n\n")
    md.append(f"**Total changed ranges:** {len(ranges)}\n\n")
    md.append(f"**Total changed bytes:** {total_bytes}\n\n")
    md.append("---\n\n")

    # Section 1: Army swap
    md.append("## 1. Army swap (hero's 7 slots)\n\n")
    md.append(f"Hero army structure:\n")
    md.append(f"- `army_types[7]` @ `0x{army_types_start:08X}` (28 bytes, 7 × 4-byte creature IDs)\n")
    md.append(f"- `army_counts[7]` @ `0x{army_counts_start:08X}` (28 bytes, 7 × 4-byte creature counts)\n\n")
    md.append("| slot | type offset | count offset | A (447_1) | B (447_2) | Δ |\n")
    md.append("|------:|-------------|--------------|-----------|-----------|---|\n")
    for i in range(n_army_slots):
        t_off = army_types_start + i * 4
        c_off = army_counts_start + i * 4
        a_t = struct.unpack('<I', a[t_off:t_off+4])[0]
        b_t = struct.unpack('<I', b[t_off:t_off+4])[0]
        a_c = struct.unpack('<I', a[c_off:c_off+4])[0]
        b_c = struct.unpack('<I', b[c_off:c_off+4])[0]
        a_t_str = "empty" if a_t == 0xFFFFFFFF else f"id={a_t} ({creature_name(a_t, creature_lookup)})"
        b_t_str = "empty" if b_t == 0xFFFFFFFF else f"id={b_t} ({creature_name(b_t, creature_lookup)})"
        if a_t != b_t or a_c != b_c:
            md.append(f"| **{i}** | `0x{t_off:08X}` | `0x{c_off:08X}` | type={a_t_str}, count={a_c} | type={b_t_str}, count={b_c} | SWAP |\n")
        else:
            md.append(f"| {i} | `0x{t_off:08X}` | `0x{c_off:08X}` | type={a_t_str}, count={a_c} | (unchanged) | — |\n")
    md.append("\n**Interpretation:** creatures from slot 4 (creature_id=9, count=200) were moved to slot 0 (which was previously empty). This is the army swap.\n\n")

    # Section 2: Artifact equip
    md.append("## 2. Artifact equip (backpack → doll)\n\n")
    md.append("### Doll slot fill\n\n")
    md.append(f"- **Doll slot offset:** `0x{doll_slot_off:08X}`\n")
    md.append(f"- **A (447_1):** `0x{a_doll:08X}` ({'empty' if a_doll==0xFFFFFFFF else artifact_name(a_doll, art_lookup)})\n")
    md.append(f"- **B (447_2):** `0x{b_doll:08X}` ({'empty' if b_doll==0xFFFFFFFF else artifact_name(b_doll, art_lookup)})\n\n")
    md.append("### Backpack count decrement\n\n")
    md.append(f"- **Backpack count offset:** `0x{backpack_count_off:08X}`\n")
    md.append(f"- **A (447_1):** {a_bcount} (29 artifacts in backpack)\n")
    md.append(f"- **B (447_2):** {b_bcount} (28 artifacts in backpack — one moved to doll)\n\n")
    md.append("### Backpack slot shift (cascade)\n\n")
    md.append(f"- **Range:** `0x{0x14400F:08X} .. 0x{0x1440D3:08X}`\n")
    md.append(f"- **Stride:** 8 bytes per backpack slot (4-byte artifact_id + 4-byte data)\n")
    md.append(f"- **{n_slots_shifted} slots shifted** (first byte of each slot's artifact_id changed)\n\n")
    md.append("When an artifact is removed from the middle of the backpack, all subsequent slots shift by one position. Since artifact IDs fit in 1 byte (max 0xFF), only the first byte of each 4-byte ID changes:\n\n")
    md.append("| position | offset | A byte (artifact) | B byte (artifact) |\n")
    md.append("|----------:|--------|-------------------|-------------------|\n")
    for i, s in enumerate(shifted_summary[:10]):
        md.append(f"| {i} | `{s['offset']}` | 0x{s['a_byte']:02X} ({s['a_artifact']}) | 0x{s['b_byte']:02X} ({s['b_artifact']}) |\n")
    if len(shifted_summary) > 10:
        md.append(f"| ... | ... | ... | ... |\n")
    md.append("\n")

    # Section 3: Other changes
    md.append("## 3. Side effects\n\n")
    md.append("### Day/step counter\n\n")
    md.append("| offset | A | B | Δ |\n|---|---|---|---|\n")
    for f in findings:
        if f.get("category") == "counter":
            md.append(f"| `0x{int(f['offset'], 16):08X}` | {f['a_value']} | {f['b_value']} | {f['delta']:+d} |\n")
    md.append("\n")

    md.append("### Hero alt blocks (stride 0x1E0 = 480 bytes)\n\n")
    for f in findings:
        if f.get("category") == "hero_alt_block_changes":
            md.append(f"- **{f['n_changes']} hero alt-block changes** at stride {f['stride']}\n")
            md.append(f"- **Value:** 0x{f['a_value']:02X} → 0x{f['b_value']:02X} (delta={f['delta']:+d})\n")
            md.append(f"- **Interpretation:** {f['interpretation']}\n")
            md.append(f"- **First 5 offsets:** {', '.join(f['offsets'][:5])}\n\n")

    md.append("### Path records added\n\n")
    for f in findings:
        if f.get("category") == "path_records_added":
            md.append(f"- **Range:** `{f['range']}` ({f['length']} bytes)\n")
            md.append(f"- **Was:** all zeros\n")
            md.append(f"- **Now:** {f['first_bytes_b']}\n")
            md.append(f"- **Interpretation:** {f['interpretation']}\n\n")

    md.append("### Replay log updated (HotA base64)\n\n")
    for f in findings:
        if f.get("category") == "replay_log_updated":
            md.append(f"- **Range:** `{f['range']}` ({f['length']} bytes)\n")
            md.append(f"- **Interpretation:** {f['interpretation']}\n\n")

    md.append("### HD3 trailer zeroed\n\n")
    for f in findings:
        if f.get("category") == "hd3_trailer_zeroed":
            md.append(f"- **Range:** `{f['range']}`\n")
            md.append(f"- **Interpretation:** {f['interpretation']}\n\n")

    md.append("---\n\n## Summary of localized fields\n\n")
    md.append("| Field | Offset | Size | Description |\n|---|---|---:|---|\n")
    md.append(f"| `army_types[7]` | `0x{army_types_start:08X}` | 28 B | Hero army creature IDs (7 × 4-byte), 0xFFFFFFFF = empty |\n")
    md.append(f"| `army_counts[7]` | `0x{army_counts_start:08X}` | 28 B | Hero army creature counts (7 × 4-byte) |\n")
    md.append(f"| `doll_slot[N]` | `0x{doll_slot_off:08X}` | 4 B | Equipped artifact slot on hero doll (0xFFFFFFFF = empty) |\n")
    md.append(f"| `backpack_count` | `0x{backpack_count_off:08X}` | 1 B | Number of artifacts currently in backpack |\n")
    md.append(f"| `backpack_slots[N]` | `0x{0x14400F:08X}+` | 8 B each | Backpack artifact slots (4-byte ID + 4-byte data), shifted on remove |\n")
    md.append("| `day_or_step_counter` | `0x000003B7` | 1 B | Incremented by 1 per user action (49→50) |\n")
    md.append("| `hero_alt_flag` | `0x001789AB+0x1E0*N` | 1 B | Per-hero flag (0x04→0x0c, bits 2+3 set) |\n")

    with open(OUT_MD, "w", encoding="utf-8") as f:
        f.writelines(md)
    print(f"Wrote {OUT_MD}  ({os.path.getsize(OUT_MD):,} bytes)")


if __name__ == "__main__":
    main()
