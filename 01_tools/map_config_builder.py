#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
map_config_builder.py — Build a per-map configuration by comparing
a map JSON parse with a "day-zero" save (save immediately after
loading the map, before any player action).

Algorithm:
  1. User loads Map JSON  → builds objects_by_coord (all objects with coords)
  2. User loads Day-Zero Save → we scan the save and build a config:
     a. Object cluster ranges (main array, visiting, fog, decoration)
     b. Hero section: start offset, stride, count
     c. Town section: offsets of all town records
     d. Player resource offsets
     e. Map name, size, version
  3. Config is saved to a file (map_config_<mapname>.json)
  4. Any subsequent save from the SAME map can now be parsed using this config

The key insight: the day-zero save has a KNOWN initial state. By scanning
it with the map's known objects, we can find exact section boundaries.
These boundaries are stable for all saves of the same map (the save has
pre-allocated fixed-size sections — verified by hero-hire diff showing
zero file size change).
"""

import json
import os
import struct
import gzip
import io
from typing import Any, Dict, List, Optional, Tuple
from collections import defaultdict


def decompress_save(path: str) -> bytes:
    """Decompress a .GM1 save (gzip with broken CRC)."""
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


def parse_header(raw: bytes) -> dict:
    """Parse the save header to extract map name, version, etc."""
    info = {
        "magic": raw[:5].decode("ascii", errors="replace"),
        "version_major": raw[8],
        "version_minor": raw[12],
        "map_type": struct.unpack("<I", raw[0x30:0x34])[0],
        "has_underground": raw[0x34],
        "map_size": struct.unpack("<I", raw[0x35:0x39])[0],
        "map_name": "",
    }
    try:
        name_len = struct.unpack("<H", raw[0x3A:0x3C])[0]
        if 0 < name_len < 256:
            info["map_name"] = raw[0x3C:0x3C + name_len].decode("cp1251", errors="replace")
    except Exception:
        pass
    return info


def find_object_clusters(raw: bytes, coord_ints: set) -> dict:
    """
    Scan the save for all 3-byte coordinate sequences that match known objects.
    Group matches into clusters by offset density.

    Returns:
      {
        "clusters": [
          {"name": "main", "start": 0x120C8C, "end": 0x17xxxx, "hits": 7269},
          {"name": "visiting", "start": 0x118C1C, ...},
          ...
        ],
        "object_offsets": {coord_int: [offset, ...], ...}
      }
    """
    n = len(raw)
    scan_start = 0x10000  # skip header

    # Single pass: collect all offsets where 3-byte coord matches
    all_hits = []  # list of (offset, coord_int)
    for i in range(scan_start, n - 2):
        ci = raw[i] | (raw[i + 1] << 8) | (raw[i + 2] << 16)
        if ci in coord_ints:
            all_hits.append((i, ci))

    if not all_hits:
        return {"clusters": [], "object_offsets": {}}

    # Group hits into clusters (gaps > 0x10000 = new cluster)
    clusters = []
    current_start = all_hits[0][0]
    current_end = all_hits[0][0]
    current_hits = 1
    for off, ci in all_hits[1:]:
        if off - current_end > 0x10000:
            clusters.append({
                "start": current_start,
                "end": current_end,
                "hits": current_hits,
            })
            current_start = off
            current_hits = 1
        else:
            current_hits += 1
        current_end = off
    clusters.append({"start": current_start, "end": current_end, "hits": current_hits})

    # Classify clusters by hit density and position
    # The main object array has the MOST hits (one per object)
    # Sort by hit count descending
    clusters.sort(key=lambda c: -c["hits"])

    classified = []
    names = ["main", "visiting", "decoration", "alive", "fog", "treasure",
             "other_1", "other_2", "other_3", "other_4", "other_5"]
    for i, c in enumerate(clusters):
        c["name"] = names[i] if i < len(names) else f"other_{i+1}"
        c["start_hex"] = f"0x{c['start']:X}"
        c["end_hex"] = f"0x{c['end']:X}"
        classified.append(c)

    # Build object_offsets
    object_offsets = defaultdict(list)
    for off, ci in all_hits:
        object_offsets[ci].append(off)

    return {
        "clusters": classified,
        "object_offsets": dict(object_offsets),
    }


def find_hero_section(raw: bytes) -> dict:
    """
    Find the hero section by scanning for hero block patterns.
    Returns start offset, stride, and count.
    """
    # Use the hero finder from gm1_parser (imported dynamically)
    # We look for the pattern: faction(0-7 or 255) + 30 bytes + movement + ... + name
    # The simplest marker: find the first hero at offset >= 0x100000
    # and measure stride between consecutive heroes

    # Actually, let's use a simpler approach: find all positions where
    # a valid hero name (13 bytes, null-padded) appears, then validate

    # For now, use the known stride 0x446 = 1094 bytes
    # Find the first hero block by looking for the pattern:
    #   byte[0] = 0x00-0x07 or 0xFF (faction)
    #   byte[31..34] = movement_total (100-10000)
    #   byte[49] = level (0-74)

    hero_offsets = []
    i = 0x100000
    while i < len(raw) - 1122:
        faction = raw[i]
        if faction > 7 and faction != 0xFF:
            i += 1
            continue

        # Quick checks
        mt = struct.unpack("<I", raw[i+31:i+35])[0]
        if mt > 10000 or mt < 100:
            i += 1
            continue

        level = raw[i + 49]
        if level > 74:
            i += 1
            continue

        # Check name at offset 169 — first byte printable
        name_byte = raw[i + 169]
        if name_byte == 0 or name_byte == 0xFF or name_byte < 0x20:
            i += 1
            continue

        # Check equipment at offset 382 — at least one non-blank slot
        equip = raw[i+382:i+534]
        has_non_blank = False
        equip_valid = True
        for slot_idx in range(19):
            slot = equip[slot_idx*8:slot_idx*8+8]
            if slot[0] == 0xFF and slot[1] == 0xFF and slot[2] == 0xFF and slot[3] == 0xFF:
                continue
            if slot[1] == 0 and slot[2] == 0 and slot[3] == 0:
                has_non_blank = True
                continue
            equip_valid = False
            break
        if not equip_valid or not has_non_blank:
            i += 1
            continue

        # Spell bitmaps
        if any(b > 1 for b in raw[i+242:i+382]):
            i += 1
            continue

        hero_offsets.append(i)
        i += 1094  # skip to next potential hero block

    if len(hero_offsets) >= 2:
        strides = [hero_offsets[j+1] - hero_offsets[j] for j in range(min(5, len(hero_offsets)-1))]
        from collections import Counter
        stride_counts = Counter(strides)
        stride = stride_counts.most_common(1)[0][0]
    elif hero_offsets:
        stride = 1094
    else:
        stride = 1094

    return {
        "start": hero_offsets[0] if hero_offsets else None,
        "start_hex": f"0x{hero_offsets[0]:X}" if hero_offsets else None,
        "stride": stride,
        "stride_hex": f"0x{stride:X}",
        "count": len(hero_offsets),
        "all_offsets": [f"0x{o:X}" for o in hero_offsets[:20]],
    }


def find_town_section(raw: bytes, town_coords: list) -> dict:
    """
    Find town records by searching for town coordinates.
    town_coords = [(x, y, z), ...] from the map JSON.

    Town record structure (from h3sed):
      offset +0: faction (1 byte)
      offset +3: type (1 byte)
      offset +4: x (1 byte)
      offset +5: y (1 byte)
      offset +6: z (1 byte)
      offset +69: name_len (2 bytes LE)
      offset +71: name (variable)
    """
    town_offsets = []

    for x, y, z in town_coords:
        # Search for the 3-byte coord pattern (x, y, z) in the save
        # Town coords are at offset +4,+5,+6 from town block start
        needle = bytes([x, y, z])
        pos = 0x100000
        while pos < len(raw) - 100:
            idx = raw.find(needle, pos)
            if idx < 0:
                break
            # Check if this looks like a town record
            # faction at idx-4 should be 0-7 or 255
            if idx >= 4:
                faction = raw[idx - 4]
                town_type = raw[idx - 1]
                if (faction <= 7 or faction == 0xFF) and town_type <= 8:
                    town_offsets.append(idx - 4)  # block start = idx - 4
            pos = idx + 1

    # Deduplicate
    town_offsets = sorted(set(town_offsets))

    return {
        "count": len(town_offsets),
        "offsets": [f"0x{o:X}" for o in town_offsets],
    }


def build_map_config(map_data, day_zero_raw: bytes, day_zero_path: str) -> dict:
    """
    Build a complete map configuration by comparing the map JSON
    with the day-zero save.

    Args:
      map_data: MapData object from map_json_loader.py
      day_zero_raw: decompressed day-zero save bytes
      day_zero_path: path to the day-zero .GM1 file

    Returns:
      config dict with all section offsets, cluster ranges, etc.
    """
    header = parse_header(day_zero_raw)

    # Build coord_int set from map objects
    coord_ints = set()
    town_coords = []
    for ck, rec in map_data.objects_by_coord.items():
        coord_ints.add(rec["coord_int"])
        if rec.get("category") == "town":
            town_coords.append((rec["x"], rec["y"], rec["z"]))

    # Find object clusters
    cluster_data = find_object_clusters(day_zero_raw, coord_ints)

    # Find hero section
    hero_section = find_hero_section(day_zero_raw)

    # Find town section
    town_section = find_town_section(day_zero_raw, town_coords)

    # Build config
    config = {
        "_meta": {
            "map_name": header["map_name"],
            "map_size": header["map_size"],
            "has_underground": bool(header["has_underground"]),
            "save_version_major": header["version_major"],
            "save_version_minor": header["version_minor"],
            "day_zero_save": os.path.basename(day_zero_path),
            "day_zero_size": len(day_zero_raw),
            "n_objects": map_data.n_objects,
            "n_unique_coords": len(map_data.objects_by_coord),
            "n_towns": len(town_coords),
            "n_heroes_total": hero_section["count"],
        },
        "clusters": cluster_data["clusters"],
        "hero_section": hero_section,
        "town_section": town_section,
        "object_offsets": {},  # coord_int -> {main_offset, visiting_offset, ...}
    }

    # Build per-object offsets (classified by cluster)
    cluster_ranges = [(c["name"], c["start"], c["end"]) for c in cluster_data["clusters"]]

    for ci_str, offsets in cluster_data["object_offsets"].items():
        ci = int(ci_str)
        main_off = None
        visiting_off = None
        fog_off = None
        alive_off = None
        treasure_off = None
        decoration_off = None

        for off in offsets:
            for name, cstart, cend in cluster_ranges:
                if cstart <= off < cend:
                    if name == "main" and not main_off:
                        main_off = f"0x{off:X}"
                    elif name == "visiting" and not visiting_off:
                        visiting_off = f"0x{off:X}"
                    elif name == "fog" and not fog_off:
                        fog_off = f"0x{off:X}"
                    elif name == "alive" and not alive_off:
                        alive_off = f"0x{off:X}"
                    elif name == "treasure" and not treasure_off:
                        treasure_off = f"0x{off:X}"
                    elif name == "decoration" and not decoration_off:
                        decoration_off = f"0x{off:X}"
                    break

        config["object_offsets"][ci_str] = {
            "main_offset": main_off,
            "visiting_offset": visiting_off,
            "fog_offset": fog_off,
            "alive_offset": alive_off,
            "treasure_offset": treasure_off,
            "decoration_offset": decoration_off,
            "save_offsets": [f"0x{o:X}" for o in offsets],
        }

    return config


def save_config(config: dict, output_dir: str) -> str:
    """Save config to a JSON file. Returns the path."""
    map_name = config["_meta"]["map_name"]
    # Sanitize map name for filename
    safe_name = "".join(c if c.isalnum() or c in "-_" else "_" for c in map_name)
    if not safe_name:
        safe_name = "unknown_map"
    filename = f"map_config_{safe_name}.json"
    path = os.path.join(output_dir, filename)
    with open(path, "w", encoding="utf-8") as f:
        json.dump(config, f, ensure_ascii=False, indent=2)
    return path


def load_config(path: str) -> dict:
    """Load a previously saved map config."""
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)
