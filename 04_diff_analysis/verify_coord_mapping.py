#!/usr/bin/env python3
"""
verify_coord_mapping.py

Cross-verify the object mapping against the actual decompressed GM1 save:
for each non-decoration object (town, hero, monster, resource, artifact,
mine, sign, ...), search for its 3-byte coordinate (x|y<<8|z<<16) in the
save and report all offsets. This proves that the coord_int encoding we
use to look up object types from save data is correct.

Outputs:
  - coord_mapping_verification.json : per-object verification table
  - stdout: high-level stats + sample matches
"""

import json
import os
import struct
import gzip
import io

SAVE_PATH = "/home/z/my-project/upload/312.GM1"
DECOMP_PATH = "/tmp/312_dec.bin"
OC_PATH = "/home/z/my-project/download/objects_by_coord.json"
OUT_PATH = "/home/z/my-project/download/coord_mapping_verification.json"


def decompress_save():
    if os.path.exists(DECOMP_PATH) and \
       os.path.getsize(DECOMP_PATH) > 1_000_000:
        return open(DECOMP_PATH, "rb").read()
    raw = open(SAVE_PATH, "rb").read()
    gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
    out = b""
    try:
        while True:
            chunk = gz.read(65536)
            if not chunk:
                break
            out += chunk
    except Exception as e:
        print(f"(decompress stopped at: {e})")
    open(DECOMP_PATH, "wb").write(out)
    return out


def find_all(data: bytes, needle: bytes):
    """Yield all offsets where needle appears in data."""
    i = 0
    while True:
        j = data.find(needle, i)
        if j < 0:
            return
        yield j
        i = j + 1


def main():
    print("Decompressing save ...")
    data = decompress_save()
    print(f"Decompressed size: {len(data):,} bytes (0x{len(data):X})")

    print(f"Loading {OC_PATH} ...")
    oc = json.load(open(OC_PATH, "r", encoding="utf-8"))
    print(f"  {len(oc)} unique coordinate keys")

    # We will check every non-decoration tile.
    # Decorations are mostly just graphics; they don't appear in the save
    # as coordinates.
    interesting_categories = {
        "town", "hero", "mine", "dwelling", "monster", "bank",
        "resource", "artifact", "treasure", "visit_skill", "visit_magic",
        "visit_other", "quest", "teleport", "garrison", "shipyard",
        "boat", "sign", "event", "observatory", "terrain_modifier",
    }

    results = {}
    matched = 0
    unmatched = 0
    sample_matches = []

    for coord_key, rec in oc.items():
        if rec["category"] not in interesting_categories:
            continue
        x = rec["x"]; y = rec["y"]; z = rec["z"]
        # 3-byte form (most common in H3 saves)
        needle3 = bytes([x & 0xFF, y & 0xFF, z & 0xFF])
        offs3 = list(find_all(data, needle3))
        # 4-byte form (rare, used in some arrays)
        needle4 = struct.pack("<I", x | (y << 8) | (z << 16))
        offs4 = list(find_all(data, needle4))

        # Filter 4-byte matches that aren't part of a 3-byte match
        set3 = set(offs3)
        offs4_only = [o for o in offs4 if o not in set3 and (o-1) not in set3]

        rec_out = {
            "type":     rec["type"],
            "category": rec["category"],
            "sprite_def": rec.get("sprite_def"),
            "coord_int": rec["coord_int"],
            "matches_3byte": [f"0x{o:X}" for o in offs3[:20]],
            "n_matches_3byte": len(offs3),
            "matches_4byte_only": [f"0x{o:X}" for o in offs4_only[:10]],
            "n_matches_4byte_only": len(offs4_only),
            "details": rec.get("details", {}),
        }
        results[coord_key] = rec_out

        if offs3 or offs4_only:
            matched += 1
            if len(sample_matches) < 30:
                sample_matches.append((coord_key, rec["type"], rec["category"],
                                       len(offs3), len(offs4_only),
                                       offs3[:3], offs4_only[:2]))
        else:
            unmatched += 1

    print(f"\nMatched:   {matched} objects")
    print(f"Unmatched: {unmatched} objects")
    print()
    print("Sample matches (coord | type | 3-byte # | 4-byte # | first 3-byte offs):")
    for ck, t, cat, n3, n4, o3, o4 in sample_matches[:25]:
        o3s = ", ".join(f"0x{o:X}" for o in o3) if o3 else "-"
        print(f"  {ck:14s} {t:30s} 3b:{n3:3d}  4b:{n4:2d}  offs3=[{o3s}]")

    # Save full results
    with open(OUT_PATH, "w", encoding="utf-8") as f:
        json.dump(results, f, ensure_ascii=False, indent=1)
    print(f"\nWrote {OUT_PATH}  ({os.path.getsize(OUT_PATH):,} bytes)")

    # Now, group unmatched by category to see what we're missing
    cat_unmatched = {}
    cat_matched   = {}
    for ck, r in results.items():
        c = r["category"]
        if r["n_matches_3byte"] == 0 and r["n_matches_4byte_only"] == 0:
            cat_unmatched[c] = cat_unmatched.get(c, 0) + 1
        else:
            cat_matched[c] = cat_matched.get(c, 0) + 1

    print("\nPer-category stats (matched / unmatched):")
    all_cats = set(cat_matched) | set(cat_unmatched)
    for c in sorted(all_cats):
        m = cat_matched.get(c, 0)
        u = cat_unmatched.get(c, 0)
        print(f"  {c:25s} matched={m:4d}  unmatched={u:4d}")


if __name__ == "__main__":
    main()
