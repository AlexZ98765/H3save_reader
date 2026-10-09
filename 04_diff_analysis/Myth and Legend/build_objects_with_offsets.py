#!/usr/bin/env python3
"""
build_objects_with_offsets.py

For each of the 7009 map objects, compute their save-file offsets
across all known save clusters. This enriches the object mapping
with concrete byte addresses where each object appears in the
decompressed .GM1 save (312.GM1 by default).

Output:
    03_object_mapping/objects_with_offsets.json
        key = coord_int (string)
        value = {
            "coord_key":           "x:y:z",
            "type":                "Tower",
            "category":            "town",
            "sprite_def":          "AVLholg0.def",
            "save_offsets":        ["0x1FD60", "0x120C8C", ...],
            "main_offset":         "0x120C8C",   # in main object-state array
            "visiting_offset":     "0x118C1C",   # in visiting-objects array
            "fog_offset":          null,           # in fog-of-war / discovered
            "alive_offset":        null,           # in alive-objects overlay
            "treasure_offset":     null,           # in treasure/visit_other cluster
            "decoration_offset":   null,           # in decoration-shadow bitmask
        }
"""

import json
import os

VERIF_PATH = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/coord_mapping_verification.json"
OC_PATH    = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/objects_by_coord.json"
OUT_PATH   = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/objects_with_offsets.json"


# Cluster ranges (from analyze_save_clusters.py output for 312.GM1)
# These are approximate — based on dominant offset clusters where
# object coord bytes appear in the decompressed save.
CLUSTERS = [
    # (name, start, end, description)
    ("visiting",     0x118C1C, 0x11FFFF, "Visiting-objects array (per-hero / per-town visited)"),
    ("main",         0x120C8C, 0x12FFFF, "Main per-object state array (~7009 objects)"),
    ("treasure",     0xADB820 & 0xFFFFF0, 0xADB820 & 0xFFFFF0 + 0x10000, "Treasure / visit_other heavy cluster"),
    ("alive",        0x9D800,  0x9E000,  "Alive-objects overlay cluster"),
    ("fog",          0x7E900,  0x7F000,  "Fog-of-war / discovered-objects cluster"),
    ("decoration",   0x172100, 0x173FFF, "Decoration-shadow bitmask cluster"),
]


def find_cluster(offset: int):
    """Return cluster name for a given offset, or None if outside all clusters."""
    for name, start, end, _ in CLUSTERS:
        if start <= offset < end:
            return name
    return None


def main():
    print(f"Loading {VERIF_PATH} ...")
    verif = json.load(open(VERIF_PATH, encoding="utf-8"))
    print(f"  {len(verif)} non-decoration objects with verification data")

    print(f"\nLoading {OC_PATH} ...")
    oc = json.load(open(OC_PATH, encoding="utf-8"))
    print(f"  {len(oc)} total unique coordinate keys")

    out = {}
    n_with_main = 0
    n_with_visiting = 0
    n_with_fog = 0
    n_with_decoration = 0
    n_total_offsets = 0

    # Process verified objects (1751 non-decoration)
    for ck, rec in oc.items():
        ci_str = str(rec["coord_int"])
        v = verif.get(ck)
        if v is None:
            # Decoration (not in verification file) — has decoration-cluster offset only
            out[ci_str] = {
                "coord_key":         ck,
                "type":              rec.get("type", ""),
                "category":          rec.get("category", ""),
                "sprite_def":        rec.get("sprite_def", ""),
                "save_offsets":      [],
                "main_offset":       None,
                "visiting_offset":   None,
                "fog_offset":        None,
                "alive_offset":      None,
                "treasure_offset":   None,
                "decoration_offset": None,
                "verified":          False,
            }
            continue

        # Categorize each 3-byte match offset by cluster
        save_offsets = []
        clusters_hits = {
            "main":       [],
            "visiting":   [],
            "fog":        [],
            "alive":      [],
            "treasure":   [],
            "decoration": [],
            "other":      [],
        }
        for off_str in v.get("matches_3byte", []):
            off = int(off_str, 16)
            save_offsets.append(off_str)
            cluster = find_cluster(off)
            if cluster:
                clusters_hits[cluster].append(off_str)
            else:
                clusters_hits["other"].append(off_str)
        for off_str in v.get("matches_4byte_only", []):
            save_offsets.append(off_str)
            clusters_hits["other"].append(off_str)

        out[ci_str] = {
            "coord_key":         ck,
            "type":              rec.get("type", ""),
            "category":          rec.get("category", ""),
            "sprite_def":        rec.get("sprite_def", ""),
            "save_offsets":      save_offsets,
            "main_offset":       clusters_hits["main"][0]       if clusters_hits["main"]       else None,
            "visiting_offset":   clusters_hits["visiting"][0]   if clusters_hits["visiting"]   else None,
            "fog_offset":        clusters_hits["fog"][0]        if clusters_hits["fog"]        else None,
            "alive_offset":      clusters_hits["alive"][0]      if clusters_hits["alive"]      else None,
            "treasure_offset":   clusters_hits["treasure"][0]   if clusters_hits["treasure"]   else None,
            "decoration_offset": clusters_hits["decoration"][0] if clusters_hits["decoration"] else None,
            "verified":          True,
        }

        n_total_offsets += len(save_offsets)
        if clusters_hits["main"]:       n_with_main += 1
        if clusters_hits["visiting"]:   n_with_visiting += 1
        if clusters_hits["fog"]:        n_with_fog += 1
        if clusters_hits["decoration"]: n_with_decoration += 1

    # Save
    with open(OUT_PATH, "w", encoding="utf-8") as f:
        json.dump(out, f, ensure_ascii=False, indent=1)
    print(f"\nWrote {OUT_PATH}  ({os.path.getsize(OUT_PATH):,} bytes)")
    print(f"  Total objects: {len(out)}")
    print(f"  Verified (non-decoration): {sum(1 for v in out.values() if v['verified'])}")
    print(f"  With main_offset:        {n_with_main}")
    print(f"  With visiting_offset:    {n_with_visiting}")
    print(f"  With fog_offset:         {n_with_fog}")
    print(f"  With decoration_offset:  {n_with_decoration}")
    print(f"  Total coord-bytes occurrences: {n_total_offsets}")

    # Print samples
    print("\nSample records:")
    samples = ["879", "4649", "9264"]  # Tower Кавала, Prison, Event
    for ci in samples:
        if ci in out:
            print(f"\n  coord_int={ci}:")
            for k, v in out[ci].items():
                if k == "save_offsets":
                    print(f"    {k}: {v[:5]}{'...' if len(v)>5 else ''}")
                else:
                    print(f"    {k}: {v}")


if __name__ == "__main__":
    main()
