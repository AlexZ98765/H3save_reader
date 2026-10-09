#!/usr/bin/env python3
"""
build_objects_flat.py

Build a flat list of ALL 7009 objects (no overlays nesting) from
objects_by_coord.json.

Why:
    objects_by_coord.json uses one entry per (x,y,z) coordinate and
    packs additional objects on the same tile into "overlays[]".
    That's convenient for coord_int lookups, but iterating over every
    object requires recursion.

    objects_flat.json gives a plain array of all 7009 objects, each
    with its own coord_int, type, category, sprite_def, details.
    Useful for bulk iteration, filtering, counting.

Output:
    /home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/
        objects_flat.json         (flat list, sorted by coord_int)
        objects_by_coord.json     (unchanged — primary + overlays)
"""

import json
import os

OC_PATH = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/objects_by_coord.json"
OUT_PATH = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/objects_flat.json"


def main():
    print(f"Loading {OC_PATH} ...")
    oc = json.load(open(OC_PATH, encoding="utf-8"))
    print(f"  {len(oc)} unique coordinate keys (primary records)")

    flat = []
    n_overlays_unpacked = 0
    for ck, rec in oc.items():
        # Primary record (without overlays field — it's implied by tile)
        flat_rec = {
            "coord_key":     rec["coord_key"],
            "coord_int":     rec["coord_int"],
            "x":             rec["x"],
            "y":             rec["y"],
            "z":             rec["z"],
            "object_index":  rec["object_index"],
            "sprite_ref_id": rec.get("sprite_ref_id"),
            "sprite_def":    rec.get("sprite_def"),
            "sprite_base":   rec.get("sprite_base"),
            "type":          rec.get("type", ""),
            "category":      rec.get("category", ""),
            "details":       rec.get("details", {}),
            "is_primary":    True,
        }
        flat.append(flat_rec)

        # Unpack overlays
        for ovl in rec.get("overlays", []):
            flat.append({
                "coord_key":     ck,                # same as parent
                "coord_int":     rec["coord_int"],  # same as parent
                "x":             rec["x"],
                "y":             rec["y"],
                "z":             rec["z"],
                "object_index":  ovl.get("object_index"),
                "sprite_ref_id": ovl.get("sprite_ref_id"),
                "sprite_def":    ovl.get("sprite_def"),
                "sprite_base":   ovl.get("sprite_base") if ovl.get("sprite_def") else None,
                "type":          ovl.get("type", ""),
                "category":      ovl.get("category", ""),
                "details":       ovl.get("details", {}),
                "is_primary":    False,
            })
            n_overlays_unpacked += 1

    # Sort by (coord_int, is_primary DESC, object_index) for stable ordering:
    # primary first on each tile, then overlays by object_index
    flat.sort(key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1, o["object_index"] or 0))

    out = {
        "_meta": {
            "map_name":           "Myth and Legend.h3m",
            "total_objects":      len(flat),
            "primary_objects":    sum(1 for o in flat if o["is_primary"]),
            "overlay_objects":    sum(1 for o in flat if not o["is_primary"]),
            "unique_coords":      len(oc),
            "source_file":        "objects_by_coord.json",
            "coord_encoding":     "coord_int = x | (y << 8) | (z << 16)",
            "sort_order":         "by coord_int, then primary-first, then object_index",
            "schema":             "list of objects; each has x,y,z,type,category,sprite_def,details,is_primary",
        },
        "objects": flat,
    }

    with open(OUT_PATH, "w", encoding="utf-8") as f:
        json.dump(out, f, ensure_ascii=False, indent=1)

    print(f"\nWrote {OUT_PATH}  ({os.path.getsize(OUT_PATH):,} bytes)")
    print(f"  Total objects (flat):    {len(flat)}")
    print(f"    Primary (on own tile): {out['_meta']['primary_objects']}")
    print(f"    Overlays (stacked):    {out['_meta']['overlay_objects']}")
    print(f"  Unique coords:           {out['_meta']['unique_coords']}")

    # Sanity check
    from collections import Counter
    cat_count = Counter()
    type_count = Counter()
    for o in flat:
        cat_count[o["category"]] += 1
        type_count[o["type"]] += 1

    print(f"\nTop 10 categories (flat):")
    for c, n in cat_count.most_common(10):
        print(f"  {c:25s} {n}")
    print(f"\nTotal distinct types: {len(type_count)}")

    # Print first 3 records
    print("\nFirst 3 records:")
    for o in flat[:3]:
        print(f"  ({o['x']:3d},{o['y']:3d},{o['z']}) primary={o['is_primary']} "
              f"type={o['type']!r:25s} sprite={o['sprite_def']}")


if __name__ == "__main__":
    main()
