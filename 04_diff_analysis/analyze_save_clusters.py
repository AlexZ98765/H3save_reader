#!/usr/bin/env python3
"""
analyze_save_clusters.py

Using the verified coord-mapping, group all 3-byte coordinate matches
in the decompressed save into offset clusters. This reveals where the
save file stores different logical sections:
  - object array (one entry per map object)
  - visiting_coords array (per-hero / per-town visited)
  - fog-of-war bitmap references
  - town records
  - hero records
  - etc.

Output: a Markdown report + JSON with cluster metadata.
"""

import json
import os
import struct
from collections import Counter, defaultdict

DECOMP_PATH = "/tmp/312_dec.bin"
VERIF_PATH  = "/home/z/my-project/download/coord_mapping_verification.json"
OC_PATH     = "/home/z/my-project/download/objects_by_coord.json"
OUT_JSON    = "/home/z/my-project/download/save_offset_clusters.json"
OUT_MD      = "/home/z/my-project/download/save_offset_clusters.md"


def find_all(data, needle):
    i = 0
    while True:
        j = data.find(needle, i)
        if j < 0:
            return
        yield j
        i = j + 1


def main():
    data = open(DECOMP_PATH, "rb").read()
    print(f"Save size: {len(data):,} bytes (0x{len(data):X})")
    oc = json.load(open(OC_PATH, "r", encoding="utf-8"))

    # For each tile (including decorations), find all offsets where its
    # 3-byte coord appears in the save.
    all_offsets = []  # list of (offset, coord_key, type, category)
    for coord_key, rec in oc.items():
        x, y, z = rec["x"], rec["y"], rec["z"]
        needle = bytes([x & 0xFF, y & 0xFF, z & 0xFF])
        for o in find_all(data, needle):
            all_offsets.append((o, coord_key, rec["type"], rec["category"]))

    print(f"Total coord-bytes occurrences in save: {len(all_offsets):,}")

    # Sort by offset
    all_offsets.sort()

    # Cluster offsets into ranges (gaps > 256 bytes start a new cluster)
    clusters = []
    if all_offsets:
        cur_start = all_offsets[0][0]
        cur_end   = all_offsets[0][0]
        cur_items = [all_offsets[0]]
        for it in all_offsets[1:]:
            o = it[0]
            if o - cur_end <= 256:
                cur_end = o
                cur_items.append(it)
            else:
                clusters.append((cur_start, cur_end, cur_items))
                cur_start = o
                cur_end   = o
                cur_items = [it]
        clusters.append((cur_start, cur_end, cur_items))

    print(f"Number of clusters (gap > 256): {len(clusters)}")

    # Build per-cluster summary
    cluster_summary = []
    for c_start, c_end, items in clusters:
        cat_count = Counter()
        type_count = Counter()
        for o, ck, t, c in items:
            cat_count[c] += 1
            type_count[t] += 1
        size = c_end - c_start + 3
        cluster_summary.append({
            "offset_start": f"0x{c_start:X}",
            "offset_end":   f"0x{c_end:X}",
            "offset_start_int": c_start,
            "offset_end_int":   c_end,
            "n_hits":       len(items),
            "span_bytes":   size,
            "top_categories": cat_count.most_common(5),
            "top_types":      type_count.most_common(5),
        })

    # Print largest clusters
    print("\nTop 25 clusters by number of coord-hits:")
    print(f"{'Offset':14s} {'Hits':>6s} {'Span':>8s}  Top categories")
    for c in sorted(cluster_summary, key=lambda x: -x["n_hits"])[:25]:
        cats = ", ".join(f"{n}({c})" for c, n in c["top_categories"][:3])
        print(f"{c['offset_start']:14s} {c['n_hits']:6d} {c['span_bytes']:8d}  {cats}")

    print(f"\nAll clusters (sorted by offset):")
    print(f"{'Offset':14s} {'Hits':>6s} {'Span':>8s}  Top categories")
    for c in cluster_summary:
        cats = ", ".join(f"{n}({c})" for c, n in c["top_categories"][:3])
        print(f"{c['offset_start']:14s} {c['n_hits']:6d} {c['span_bytes']:8d}  {cats}")

    # Write JSON
    with open(OUT_JSON, "w", encoding="utf-8") as f:
        json.dump(cluster_summary, f, ensure_ascii=False, indent=1)
    print(f"\nWrote {OUT_JSON}")

    # Write Markdown
    md = []
    md.append("# GM1 Save — Coordinate-Cluster Map\n")
    md.append(f"Save: `312.GM1` decompressed ({len(data):,} bytes, 0x{len(data):X})\n")
    md.append(f"Total coord-bytes occurrences: {len(all_offsets):,}\n")
    md.append(f"Total clusters (gap > 256 bytes): {len(clusters)}\n\n")
    md.append("## Clusters sorted by offset\n")
    md.append("| Offset start | Offset end | Hits | Span (bytes) | Top categories |\n")
    md.append("|---|---|---|---|---|\n")
    for c in cluster_summary:
        cats = ", ".join(f"{n} {cc}" for cc, n in c["top_categories"][:3])
        md.append(f"| {c['offset_start']} | {c['offset_end']} | {c['n_hits']} | "
                  f"{c['span_bytes']} | {cats} |\n")
    md.append("\n## Clusters sorted by hits (descending)\n")
    md.append("| Offset start | Hits | Span | Top categories |\n")
    md.append("|---|---|---|---|\n")
    for c in sorted(cluster_summary, key=lambda x: -x["n_hits"]):
        cats = ", ".join(f"{n} {cc}" for cc, n in c["top_categories"][:3])
        md.append(f"| {c['offset_start']} | {c['n_hits']} | {c['span_bytes']} | {cats} |\n")

    with open(OUT_MD, "w", encoding="utf-8") as f:
        f.writelines(md)
    print(f"Wrote {OUT_MD}")

    # Also: for each category, where does it appear?
    print("\nPer-category offset distribution (top 5 clusters per category):")
    cat_to_clusters = defaultdict(list)
    for c in cluster_summary:
        for cat, n in c["top_categories"]:
            cat_to_clusters[cat].append((c["offset_start"], n, c["n_hits"]))
    for cat in sorted(cat_to_clusters):
        lst = sorted(cat_to_clusters[cat], key=lambda x: -x[1])[:5]
        print(f"  {cat:25s}: {', '.join(f'{o}({n}/{t})' for o,n,t in lst)}")


if __name__ == "__main__":
    main()
