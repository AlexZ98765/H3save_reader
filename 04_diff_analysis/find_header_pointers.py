#!/usr/bin/env python3
"""
find_header_pointers.py — Step 1-3 of the header pointer search.

Strategy:
  1. Search for known save offsets (hero_block, object_array, etc.) as
     4-byte LE values in the first N bytes of decompressed saves.
  2. Compare headers of 312.GM1 vs 447_1.GM1 (same map, different state)
     to find fields that differ — candidates for pointers/sizes.
  3. Statistical diff across 10 saves of the same map — find stable
     bytes (format constants) vs changing bytes (counters/pointers/state).

Output: prints findings + saves JSON report.
"""

import gzip
import io
import json
import os
import struct
from collections import defaultdict
from typing import List, Tuple

SAVES_DIR = "/home/z/my-project/upload"

# Known save offsets (per-save, for "Myth and Legend.h3m")
KNOWN_OFFSETS = {
    "312.GM1": {
        "hero_block_faction": 0x157F1E,
        "object_array_main":  0x120C8C,
        "visiting_array":     0x118C1C,
        "decoration_bitmask": 0x172129,
    },
    "447_1.GM1": {
        "hero_block_faction": 0x143DC0,  # approximate
        "object_array_main":  0x120C8C,  # same (object array position is stable per map)
        "town_records":       0x13F700,  # approximate (where town build flag is)
    },
}

# Saves to compare (all from "Myth and Legend.h3m")
SERIES_SAVES = [
    "001.GM1", "0011.GM1", "0019.GM1", "0020_9.GM1",
    "312.GM1", "312_1.GM1",
    "447_1.GM1", "447_2.GM1", "447_3.GM1",
    "xx_01.GM1", "xx_05.GM1",
]

HEADER_SIZE = 2048  # bytes to analyze as "header"


def decompress(path: str) -> bytes:
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


def find_all(data: bytes, needle: bytes, max_results: int = 50):
    """Yield all offsets where needle appears in data."""
    results = []
    i = 0
    while len(results) < max_results:
        j = data.find(needle, i)
        if j < 0:
            break
        results.append(j)
        i = j + 1
    return results


def step1_search_known_offsets(saves_data: dict):
    """Step 1: Search for known save offsets as 4-byte LE in the header."""
    print("=" * 72)
    print("STEP 1: Search for known save offsets in headers")
    print("=" * 72)

    for save_name, known in KNOWN_OFFSETS.items():
        if save_name not in saves_data:
            print(f"\n--- {save_name}: (not available) ---")
            continue
        data = saves_data[save_name]
        header = data[:HEADER_SIZE]
        print(f"\n--- {save_name} (size: {len(data):,} bytes) ---")
        for label, offset in known.items():
            # Search as 4-byte LE
            needle = struct.pack("<I", offset)
            hits = find_all(header, needle)
            # Also search as 3-byte LE (in case it's stored as 24-bit)
            needle3 = struct.pack("<I", offset)[:3]
            hits3 = find_all(header, needle3)
            # Also search the entire decompressed save (not just header)
            full_hits = find_all(data, needle, max_results=20)
            print(f"  {label} (0x{offset:X}):")
            print(f"    as 4-byte LE in header: {len(hits)} hits → {[f'0x{h:X}' for h in hits[:5]]}")
            print(f"    as 3-byte LE in header: {len(hits3)} hits → {[f'0x{h:X}' for h in hits3[:5]]}")
            print(f"    as 4-byte LE in full save (first 20): {[f'0x{h:X}' for h in full_hits[:10]]}")


def step2_compare_headers(saves_data: dict):
    """Step 2: Compare headers of 312.GM1 vs 447_1.GM1 (same map, different state)."""
    print("\n" + "=" * 72)
    print("STEP 2: Compare headers of 312.GM1 vs 447_1.GM1")
    print("=" * 72)

    a = saves_data.get("312.GM1")
    b = saves_data.get("447_1.GM1")
    if not a or not b:
        print("Missing saves")
        return

    ha = a[:HEADER_SIZE]
    hb = b[:HEADER_SIZE]
    n = min(len(ha), len(hb))

    diff_ranges = []
    i = 0
    while i < n:
        if ha[i] != hb[i]:
            start = i
            while i < n and ha[i] != hb[i]:
                i += 1
            end = i - 1
            diff_ranges.append((start, end))
        else:
            i += 1

    print(f"\nTotal differing byte ranges in first {HEADER_SIZE} bytes: {len(diff_ranges)}")
    print(f"Total differing bytes: {sum(e - s + 1 for s, e in diff_ranges)}")
    print()
    print("Differing ranges (offset, A-bytes, B-bytes, A-as-u32LE, B-as-u32LE):")
    for s, e in diff_ranges:
        a_chunk = ha[s:e + 1]
        b_chunk = hb[s:e + 1]
        # Interpret as u32 LE if 4 bytes
        a_u32 = struct.unpack("<I", a_chunk[:4].ljust(4, b"\x00"))[0] if len(a_chunk) >= 1 else 0
        b_u32 = struct.unpack("<I", b_chunk[:4].ljust(4, b"\x00"))[0] if len(b_chunk) >= 1 else 0
        print(f"  0x{s:04X}..0x{e:04X} ({e - s + 1}b): "
              f"A={a_chunk.hex(' ')}  B={b_chunk.hex(' ')}  "
              f"A_u32=0x{a_u32:08X}  B_u32=0x{b_u32:08X}")

    # Check if any diff values match known offsets
    print("\nChecking if diff values match known offsets:")
    for s, e in diff_ranges:
        a_chunk = ha[s:e + 1]
        b_chunk = hb[s:e + 1]
        if len(a_chunk) >= 4:
            a_u32 = struct.unpack("<I", a_chunk[:4])[0]
            b_u32 = struct.unpack("<I", b_chunk[:4])[0]
            for save_name, known in KNOWN_OFFSETS.items():
                for label, offset in known.items():
                    if a_u32 == offset or b_u32 == offset:
                        print(f"  ⭐ MATCH at 0x{s:04X}: {label} (0x{offset:X}) "
                              f"A=0x{a_u32:08X} B=0x{b_u32:08X}")
        # Also check 2-byte values
        if len(a_chunk) >= 2:
            a_u16 = struct.unpack("<H", a_chunk[:2])[0]
            b_u16 = struct.unpack("<H", b_chunk[:2])[0]
            # Check if these are sizes/counters
            # (we'll just print notable ones)


def step3_statistical_diff(saves_data: dict):
    """Step 3: Statistical diff across multiple saves of the same map.
    Find bytes that are stable (format constants) vs changing (state/counters)."""
    print("\n" + "=" * 72)
    print("STEP 3: Statistical diff across saves (find stable vs changing bytes)")
    print("=" * 72)

    # Get header bytes for all available saves
    headers = {}
    for name in SERIES_SAVES:
        if name in saves_data:
            headers[name] = saves_data[name][:HEADER_SIZE]

    if len(headers) < 2:
        print("Not enough saves for statistical analysis")
        return

    print(f"\nComparing {len(headers)} saves: {list(headers.keys())}")
    n = min(len(h) for h in headers.values())

    # For each byte position, count how many distinct values across saves
    distinct_values = [0] * n
    byte_values = [defaultdict(int) for _ in range(n)]
    for name, h in headers.items():
        for i in range(n):
            byte_values[i][h[i]] += 1
            distinct_values[i] = len(byte_values[i])

    # Classify bytes
    n_saves = len(headers)
    stable_bytes = []      # 1 distinct value (same in all saves)
    binary_bytes = []      # 2 distinct values
    changing_bytes = []    # > 2 distinct values (likely counters/pointers)
    for i in range(n):
        if distinct_values[i] == 1:
            stable_bytes.append(i)
        elif distinct_values[i] == 2:
            binary_bytes.append(i)
        else:
            changing_bytes.append(i)

    print(f"\nByte classification (first {HEADER_SIZE} bytes, {n_saves} saves):")
    print(f"  Stable (same in all):    {len(stable_bytes)} bytes")
    print(f"  Binary (2 values):       {len(binary_bytes)} bytes")
    print(f"  Changing (>2 values):    {len(changing_bytes)} bytes")

    # Group changing bytes into ranges (candidates for counters/pointers)
    if changing_bytes:
        print(f"\nChanging byte ranges (candidates for counters/pointers/sizes):")
        ranges = []
        start = changing_bytes[0]
        prev = start
        for i in changing_bytes[1:]:
            if i - prev > 4:
                ranges.append((start, prev))
                start = i
            prev = i
        ranges.append((start, prev))

        for s, e in ranges:
            size = e - s + 1
            # Show values across saves
            vals_per_save = {}
            for name, h in headers.items():
                chunk = h[s:e + 1]
                vals_per_save[name] = chunk.hex(' ')
            # Try to interpret as u32 LE
            sample_vals = []
            for name, h in headers.items():
                chunk = h[s:e + 1]
                if len(chunk) >= 4:
                    u32 = struct.unpack("<I", chunk[:4])[0]
                    sample_vals.append((name, u32))
                elif len(chunk) >= 2:
                    u16 = struct.unpack("<H", chunk[:2])[0]
                    sample_vals.append((name, u16))

            print(f"\n  0x{s:04X}..0x{e:04X} ({size}b):")
            for name, val in sample_vals[:5]:
                print(f"    {name}: {val}")

    # Show binary bytes (likely flags)
    if binary_bytes:
        print(f"\nBinary byte positions (likely flags): {[f'0x{b:X}' for b in binary_bytes[:30]]}")
        for i in binary_bytes[:15]:
            vals = list(byte_values[i].keys())
            counts = list(byte_values[i].values())
            print(f"  0x{i:04X}: values {[f'0x{v:02X}' for v in vals]} counts {counts}")


def main():
    print("Loading saves...")
    saves_data = {}
    for name in SERIES_SAVES + list(KNOWN_OFFSETS.keys()):
        path = os.path.join(SAVES_DIR, name)
        if os.path.exists(path):
            try:
                saves_data[name] = decompress(path)
                print(f"  {name}: {len(saves_data[name]):,} bytes")
            except Exception as e:
                print(f"  {name}: ERROR {e}")
        else:
            print(f"  {name}: not found")

    step1_search_known_offsets(saves_data)
    step2_compare_headers(saves_data)
    step3_statistical_diff(saves_data)

    # Save report
    report = {
        "step1_known_offsets_searched": len(KNOWN_OFFSETS),
        "step2_diff_312_vs_447": "see stdout",
        "step3_saves_compared": list(saves_data.keys()),
    }
    out_path = "/home/z/my-project/download/homm3_gm1_toolkit/04_diff_analysis/header_pointer_search_report.json"
    with open(out_path, "w") as f:
        json.dump(report, f, indent=2)
    print(f"\nReport saved to {out_path}")


if __name__ == "__main__":
    main()
