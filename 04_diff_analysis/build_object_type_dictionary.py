#!/usr/bin/env python3
"""
build_object_type_dictionary.py

Build an editable dictionary of all HoMM3 map editor object types
from the LazyLlama wiki page:
    https://heroes.thelazy.net/index.php/Map_Editor_Objects

Also cross-reference with the 204 distinct object type names found
on the "Myth and Legend.h3m" map (from objects_by_coord.json) to
attach matching metadata.

Output:
    /home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/
        object_types_dictionary.json   (editable, human-readable)
"""

import json
import os
import re
import sys
from collections import defaultdict

WIKI_HTML    = "/tmp/lazymap.html"
OC_PATH      = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/objects_by_coord.json"
OUT_PATH     = "/home/z/my-project/download/homm3_gm1_toolkit/03_object_mapping/object_types_dictionary.json"


def clean_text(s: str) -> str:
    """Strip HTML tags and decode common entities."""
    if s is None:
        return ""
    # replace <br> with newline
    s = re.sub(r'<br\s*/?>', '\n', s, flags=re.IGNORECASE)
    # remove all other tags
    s = re.sub(r'<[^>]+>', '', s)
    # decode entities
    s = s.replace('&amp;', '&')
    s = s.replace('&nbsp;', ' ')
    s = s.replace('&quot;', '"')
    s = s.replace('&lt;', '<')
    s = s.replace('&gt;', '>')
    s = s.replace('&#039;', "'")
    s = re.sub(r'&#(\d+);', lambda m: chr(int(m.group(1))), s)
    # collapse whitespace
    s = re.sub(r'[ \t]+', ' ', s)
    s = re.sub(r'\n{3,}', '\n\n', s)
    return s.strip()


def parse_wiki_table(html: str):
    """Parse the wikitable into a list of row dicts."""
    m = re.search(r'<table[^>]*wikitable[^>]*>(.*?)</table>', html, re.DOTALL)
    if not m:
        return [], []
    table_html = m.group(1)
    rows = re.findall(r'<tr[^>]*>(.*?)</tr>', table_html, re.DOTALL)
    if not rows:
        return [], []

    # First row = headers
    headers = []
    header_cells = re.findall(r'<th[^>]*>(.*?)</th>', rows[0], re.DOTALL)
    for c in header_cells:
        headers.append(clean_text(c))

    # Remaining rows = data
    records = []
    for r in rows[1:]:
        cells = re.findall(r'<t[dh][^>]*>(.*?)</t[dh]>', r, re.DOTALL)
        if len(cells) < len(headers):
            continue
        rec = {}
        for i, h in enumerate(headers):
            rec[h] = clean_text(cells[i]) if i < len(cells) else ""
        records.append(rec)
    return headers, records


def normalize_for_match(name: str) -> str:
    """Lowercase + strip non-alphanumerics for fuzzy name matching."""
    s = name.lower().strip()
    s = re.sub(r'[^a-z0-9]+', '', s)
    return s


def main():
    if not os.path.exists(WIKI_HTML):
        print(f"FATAL: {WIKI_HTML} not found. Run curl first.", file=sys.stderr)
        sys.exit(1)
    html = open(WIKI_HTML, encoding='utf-8').read()
    print(f"Wiki HTML size: {len(html):,} bytes")

    headers, records = parse_wiki_table(html)
    print(f"Parsed {len(records)} object types from wiki table.")
    print(f"Headers: {headers}")
    print()

    # Build a name-normalized lookup for cross-matching
    wiki_by_norm = {}
    for r in records:
        n = normalize_for_match(r.get('Object Name', ''))
        if n:
            wiki_by_norm[n] = r

    # Load object map to find which types are actually used
    if os.path.exists(OC_PATH):
        oc = json.load(open(OC_PATH, encoding='utf-8'))
        types_used = {}
        for ck, rec in oc.items():
            t = rec.get('type')
            if t and t != '(empty)':
                types_used[t] = types_used.get(t, 0) + 1
        print(f"Map uses {len(types_used)} distinct type names (from {len(oc)} objects).")
    else:
        types_used = {}
        print(f"WARN: {OC_PATH} not found - skipping cross-reference.")

    # Match: try to find each map-type in the wiki (by normalized name)
    matches = 0
    unmatched_map_types = []
    for t in types_used:
        n = normalize_for_match(t)
        if n in wiki_by_norm:
            matches += 1
        else:
            unmatched_map_types.append((t, types_used[t]))

    print(f"Matched {matches}/{len(types_used)} map types to wiki entries.")
    print(f"Unmatched: {len(unmatched_map_types)}")
    if unmatched_map_types:
        print("Top 20 unmatched map types (with their count):")
        for t, c in sorted(unmatched_map_types, key=lambda x: -x[1])[:20]:
            print(f"  {c:4d}  {t}")

    # Build category index from wiki
    cat_index = defaultdict(list)
    for r in records:
        cat = r.get('Category', '(uncategorized)')
        cat_index[cat].append(r.get('Object Name', ''))

    # Build def-file index (will be used to match our sprite_def names)
    def_index = {}
    for r in records:
        fn = r.get('File Name', '').strip()
        if fn:
            def_index[fn.lower()] = r

    # Cross-reference: how many of our map's sprite_def files match wiki File Names?
    if os.path.exists(OC_PATH):
        oc = json.load(open(OC_PATH, encoding='utf-8'))
        sprite_defs_used = set()
        for ck, rec in oc.items():
            sd = rec.get('sprite_def')
            if sd:
                sprite_defs_used.add(sd.lower())
        matched_defs   = sum(1 for sd in sprite_defs_used if sd in def_index)
        unmatched_defs = [sd for sd in sprite_defs_used if sd not in def_index]
        print()
        print(f"Sprite .def files used on map: {len(sprite_defs_used)}")
        print(f"  Matched to wiki:    {matched_defs}")
        print(f"  Unmatched in wiki:  {len(unmatched_defs)}")
        if unmatched_defs:
            print("  Sample unmatched .def files:")
            for sd in sorted(unmatched_defs)[:15]:
                print(f"    {sd}")

    # ----- Build the output dictionary -----
    # The output is editable JSON. Primary key is the .def file name
    # (which is unique per wiki row). We also keep a name->file_names
    # index for easy lookup by object name.
    #
    # User-editable fields are kept under "user_edits" and prefixed with
    # "user_". On rebuild, the script will preserve these by merging
    # with the previous file's user_edits.

    # Try to load existing file's user_edits to preserve them
    existing_user_edits = {}
    if os.path.exists(OUT_PATH):
        try:
            existing = json.load(open(OUT_PATH, encoding='utf-8'))
            for fn, rec in existing.get('objects', {}).items():
                ue = rec.get('user_edits', {})
                if ue:
                    existing_user_edits[fn.lower()] = ue
            print(f"Loaded {len(existing_user_edits)} existing user-edits for preservation.")
        except Exception as e:
            print(f"WARN: could not load existing user-edits: {e}")

    out = {
        "_meta": {
            "source": "https://heroes.thelazy.net/index.php/Map_Editor_Objects",
            "page_revision": 194767,
            "fetched_at": "2026-10-08",
            "total_object_types": len(records),
            "categories": dict(sorted({k: len(v) for k, v in cat_index.items()}.items(),
                                       key=lambda x: -x[1])),
            "notes": [
                "Primary key = .def file name (lowercase), which is unique.",
                "The 'objects' dict preserves ALL 2086 wiki rows — many Object Names",
                "appear multiple times because each visual variant gets its own row",
                "(e.g. 87 'Mountain' rows with different .def files).",
                "Use the 'name_index' to look up by Object Name → list of .def files.",
                "Use 'id_index' to look up by 'ObjectID:SubID' → .def file.",
                "User-edits under 'user_edits' in each entry are preserved on rebuild.",
            ],
        },
        "_categories": dict(cat_index),
        "_name_index": defaultdict(list),  # Object Name -> [file_name, ...]
        "_id_index":   {},                  # "objID:subID" -> file_name (first match)
        "objects":     {},                  # file_name (lowercase) -> entry
    }

    # Sort records by Category then Object Name then File Name for stable ordering
    sorted_records = sorted(records, key=lambda r: (
        r.get('Category', '').lower(),
        r.get('Object Name', '').lower(),
        r.get('File Name', '').lower()
    ))

    for r in sorted_records:
        name = r.get('Object Name', '').strip()
        fn   = r.get('File Name', '').strip().lower()
        if not name or not fn:
            continue
        oid  = r.get('Object ID', '').strip()
        sid  = r.get('Sub ID', '').strip()
        id_key = f"{oid}:{sid}"

        entry = {
            "wiki_name":      name,
            "file_name":      r.get('File Name', '').strip(),
            "category":       r.get('Category', ''),
            "generates_on":   r.get('Generates on', ''),
            "restrictions":   r.get('Restrictions', ''),
            "object_id":      oid,
            "sub_id":         sid,
            "layering":       r.get('Layering', ''),
            "description":    r.get('Description', ''),
            "user_edits":     existing_user_edits.get(fn, {
                "user_notes":     "",
                "user_category":  "",
                "user_alias":     "",
            }),
        }
        out["objects"][fn] = entry
        out["_name_index"][name].append(fn)
        if id_key not in out["_id_index"]:
            out["_id_index"][id_key] = fn

    # Convert defaultdict to plain dict for JSON serialization
    out["_name_index"] = dict(out["_name_index"])

    # Save
    os.makedirs(os.path.dirname(OUT_PATH), exist_ok=True)
    with open(OUT_PATH, "w", encoding="utf-8") as f:
        json.dump(out, f, ensure_ascii=False, indent=1)
    print()
    print(f"Wrote {OUT_PATH}  ({os.path.getsize(OUT_PATH):,} bytes)")
    print(f"  {len(out['objects'])} object types")
    print(f"  {len(out['_categories'])} categories")
    print(f"  {len(out['_name_index'])} distinct Object Names")
    print(f"  {len(out['_id_index'])} distinct ObjectID:SubID pairs")

    # ----- Build a smaller "map-types-to-wiki" cross-reference -----
    # So users can see, for each map type name, the corresponding wiki entry
    xref_path = OUT_PATH.replace('object_types_dictionary.json',
                                 'map_types_xref.json')
    xref = {}
    if os.path.exists(OC_PATH):
        oc = json.load(open(OC_PATH, encoding='utf-8'))
        for ck, rec in oc.items():
            t = rec.get('type')
            if not t or t == '(empty)':
                continue
            if t not in xref:
                n = normalize_for_match(t)
                wiki_match = wiki_by_norm.get(n)
                xref[t] = {
                    "map_count":   0,
                    "wiki_match":  wiki_match.get('Object Name', '') if wiki_match else '',
                    "wiki_id":     wiki_match.get('Object ID',  '') if wiki_match else '',
                    "wiki_sub_id": wiki_match.get('Sub ID',     '') if wiki_match else '',
                    "wiki_file":   wiki_match.get('File Name',  '') if wiki_match else '',
                    "wiki_category": wiki_match.get('Category', '') if wiki_match else '',
                }
            xref[t]['map_count'] += 1
    with open(xref_path, "w", encoding="utf-8") as f:
        json.dump(xref, f, ensure_ascii=False, indent=1)
    print(f"Wrote {xref_path}  ({os.path.getsize(xref_path):,} bytes)")
    print(f"  {len(xref)} distinct map-type names cross-referenced")

    # Print samples
    print()
    print("Sample entries:")
    for name in list(out["objects"].keys())[:3]:
        print(json.dumps(out["objects"][name], indent=2, ensure_ascii=False)[:600])
        print()


if __name__ == "__main__":
    main()
