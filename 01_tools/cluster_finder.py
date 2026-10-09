#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
cluster_finder.py — Universal, density-based finder for the per-object
data sections in a decompressed `.GM1` save.

PROBLEM with the old algorithm (map_config_builder.find_object_clusters):
  - Skipped the first 0x10000 bytes (small maps store object arrays there)
  - Used a 0x10000-byte (64 KB) gap threshold to separate clusters, which
    merged all 6 distinct per-object arrays into ONE cluster on large maps.

NEW ALGORITHM (this module) — two-pass gap-based clustering:

  Pass 1: `find_coord_hits(raw, coord_ints, scan_start)`
       Single pass through the save, recording every offset where the
       3 bytes starting there match a known `coord_int` from the map.

  Pass 2: gap-based clustering with adaptive threshold.
       Within a real cluster (e.g. the main object array), consecutive
       hits are very close together (typical intra-cluster gap is a few
       bytes — the record stride). Between clusters, consecutive hits
       are far apart (many KB).
       
       Algorithm:
         a. Sort all hits by offset
         b. Compute median intra-cluster gap (after dropping the top
            10% largest gaps as inter-cluster)
         c. Threshold = max(0x400, 8 * median_intra_gap)
            — at least 1 KB, and at most ~8× typical intra-cluster gap
         d. Group consecutive hits with gap < threshold into clusters
         e. Filter clusters with < min_cluster_hits hits

  Pass 3: `classify_clusters(clusters)`
       The densest cluster (highest peak_density) is the **main object
       array**. The next densest are visiting / fog / alive / treasure /
       decoration / other_N, in that order.

  Pass 4: `assign_object_offsets(clusters, hits_by_ci)`
       For each cluster and each coord_int, find the FIRST hit offset of
       that coord_int that falls inside the cluster. That's the offset
       of that object's record in that cluster.

The result is a set of `ObjectCluster` dataclasses plus a per-object
mapping of {coord_int -> {main_offset, visiting_offset, ...}}.
"""

from __future__ import annotations
from typing import Dict, List, Tuple, Set, Optional
from collections import defaultdict
from save_layout import ObjectCluster, ObjectOffsets


# ============================================================================
# Step 1: scan all hits
# ============================================================================

def find_coord_hits(raw: bytes,
                    coord_ints: Set[int],
                    scan_start: int = 0,
                    scan_end: Optional[int] = None) -> Dict[int, List[int]]:
    """
    Scan `raw` for every offset where the 3 bytes starting there match
    a known `coord_int` from the map.

    Args:
        raw:           decompressed save bytes
        coord_ints:    set of valid coord_ints (from MapData.coord_int_lookup)
        scan_start:    byte offset to start scanning (default: 0)
        scan_end:      byte offset to stop scanning (default: len(raw)-2)

    Returns:
        Dict[coord_int, List[offset]] — every hit offset for each coord_int.
    """
    if scan_end is None:
        scan_end = len(raw) - 2
    if scan_end < scan_start:
        return {}

    hits_by_ci: Dict[int, List[int]] = defaultdict(list)
    for i in range(scan_start, scan_end):
        ci = raw[i] | (raw[i + 1] << 8) | (raw[i + 2] << 16)
        if ci in coord_ints:
            hits_by_ci[ci].append(i)

    return dict(hits_by_ci)


def all_hits(hits_by_ci: Dict[int, List[int]]) -> List[Tuple[int, int]]:
    """
    Flatten into a sorted list of (offset, coord_int) tuples.
    """
    flat = []
    for ci, offs in hits_by_ci.items():
        for o in offs:
            flat.append((o, ci))
    flat.sort()
    return flat


# ============================================================================
# Step 2: gap-based clustering with adaptive threshold
# ============================================================================

def _compute_intra_cluster_gap(hits_sorted: List[int]) -> int:
    """
    Estimate the typical intra-cluster gap (the gap you'd see between
    consecutive hits WITHIN the same dense cluster).

    Algorithm:
      1. Compute gaps between consecutive sorted hits
      2. The largest 10% of gaps are almost certainly inter-cluster
         (between different clusters), so drop them
      3. Take the median of the remaining gaps as the "typical" intra-cluster gap
    """
    if len(hits_sorted) < 2:
        return 0
    gaps = [hits_sorted[i + 1] - hits_sorted[i]
            for i in range(len(hits_sorted) - 1)]
    gaps.sort()
    # Drop the top 10% as inter-cluster
    keep = max(1, int(len(gaps) * 0.9))
    intra_gaps = gaps[:keep]
    # Median
    return intra_gaps[len(intra_gaps) // 2]


def cluster_hits(raw_size: int,
                 hits_by_ci: Dict[int, List[int]],
                 window_size: int = 4096,
                 density_threshold: Optional[int] = None,
                 min_cluster_hits: int = 3,
                 gap_multiplier: int = 8,
                 min_gap_threshold: int = 0x400) -> List[Tuple[int, int, int]]:
    """
    Group all hits into clusters using adaptive gap-based threshold.

    Algorithm:
      1. Sort all hits by offset
      2. Compute median intra-cluster gap (drop top 10% largest gaps)
      3. Threshold = max(min_gap_threshold, gap_multiplier * median_intra_gap)
      4. Group consecutive hits where each gap < threshold into clusters
      5. Filter clusters with < min_cluster_hits hits

    Args:
        raw_size:            decompressed save size (bytes)
        hits_by_ci:          output of find_coord_hits
        window_size:         kept for API compat; ignored in this algorithm
        density_threshold:   kept for API compat; ignored
        min_cluster_hits:    discard clusters with fewer hits
        gap_multiplier:      threshold = max(min_gap_threshold, mult × median)
        min_gap_threshold:   floor for the threshold (1 KB by default)

    Returns:
        List of (cluster_start, cluster_end, cluster_hit_count) tuples,
        sorted by hit count descending.
    """
    flat = all_hits(hits_by_ci)
    if not flat:
        return []

    offs = [o for o, _ in flat]
    median_intra = _compute_intra_cluster_gap(offs)
    threshold = max(min_gap_threshold, gap_multiplier * median_intra)
    # Sanity cap: never larger than 0x40000 (256 KB)
    threshold = min(threshold, 0x40000)

    # Group consecutive hits where each gap < threshold
    clusters: List[Tuple[int, int, int]] = []
    if not offs:
        return clusters
    cur_start = offs[0]
    cur_end = offs[0]
    cur_hits = 1
    for i in range(1, len(offs)):
        gap = offs[i] - cur_end
        if gap < threshold:
            cur_hits += 1
            cur_end = offs[i]
        else:
            if cur_hits >= min_cluster_hits:
                clusters.append((cur_start, cur_end + 1, cur_hits))
            cur_start = offs[i]
            cur_end = offs[i]
            cur_hits = 1
    if cur_hits >= min_cluster_hits:
        clusters.append((cur_start, cur_end + 1, cur_hits))

    # Sort by hit count descending
    clusters.sort(key=lambda c: -c[2])
    return clusters


# ============================================================================
# Step 3: classify clusters + compute per-cluster metrics
# ============================================================================

# Cluster names in order of density (most dense first)
CLUSTER_NAMES = [
    "main",       # main object-state array — always the densest
    "visiting",   # visiting array (heroes/towns at objects)
    "decoration", # decoration-shadow bitmask (decorations)
    "alive",      # alive-objects overlay
    "fog",        # fog-of-war / discovered-objects array
    "treasure",   # treasure / visit_other cluster
    "other_1", "other_2", "other_3", "other_4", "other_5",
]


def refine_cluster_bounds(start: int, end: int,
                           hits_in_cluster: List[Tuple[int, int]],
                           padding: int = 64) -> Tuple[int, int, int, int]:
    """
    Tighten cluster bounds to actual first/last hit and compute peak
    density. Returns (tight_start, tight_end, hits_count, peak_density).
    """
    if not hits_in_cluster:
        return (start, end, 0, 0)
    # Tight bounds = first hit .. last hit
    tight_start = max(start, hits_in_cluster[0][0] - padding)
    tight_end = min(end, hits_in_cluster[-1][0] + padding)
    # Peak density: max hits in any 4 KB window within the cluster
    if tight_end > tight_start:
        window = 4096
        n_w = max(1, (tight_end - tight_start) // window)
        peak = 0
        for w in range(n_w + 1):
            ws = tight_start + w * window
            we = min(tight_end, ws + window)
            cnt = sum(1 for off, _ in hits_in_cluster if ws <= off < we)
            peak = max(peak, cnt)
    else:
        peak = len(hits_in_cluster)
    return (tight_start, tight_end, len(hits_in_cluster), peak)


def build_clusters(raw_size: int,
                   hits_by_ci: Dict[int, List[int]],
                   window_size: int = 4096,
                   density_threshold: Optional[int] = None,
                   min_cluster_hits: int = 3,
                   gap_multiplier: int = 8,
                   min_gap_threshold: int = 0x400,
                   min_distinct_ratio: float = 0.10,
                   max_clusters: int = 10) -> List[ObjectCluster]:
    """
    Iterative cluster extraction. For each iteration:
      1. Run gap-based clustering on remaining hits
      2. Pick the cluster with the highest (distinct_coords, peak_density)
         — this is the "best" cluster for this iteration (main, then
         visiting, then decoration, ...)
      3. Mask out all hits inside this cluster (so the next iteration
         can find the next-best cluster without being swamped by
         byte-pattern noise inside the previous one)
      4. Stop when no cluster passes the quality filter or max_clusters
         is reached.

    Quality filter:
      - distinct_coords / hits >= min_distinct_ratio (default 5%)
      - hits >= min_cluster_hits
      - distinct_coords >= 2 (need at least 2 distinct coords)

    Returns List[ObjectCluster] sorted by extraction order (main first).
    """
    flat = all_hits(hits_by_ci)
    if not flat:
        return []

    # Track which hits are still "active" (not yet claimed by a cluster)
    # We use a set of offsets to mask out
    active_offsets = set(o for o, _ in flat)
    total_distinct = len({ci for _, ci in flat})

    # Build a per-offset map for quick lookup
    hits_by_offset: Dict[int, int] = {o: ci for o, ci in flat}

    result: List[ObjectCluster] = []

    for iteration in range(max_clusters):
        # Build hits_by_ci for remaining active offsets
        remaining_hits: Dict[int, List[int]] = defaultdict(list)
        for o in sorted(active_offsets):
            ci = hits_by_offset.get(o)
            if ci is not None:
                remaining_hits[ci].append(o)
        if not remaining_hits:
            break

        # Cluster the remaining hits
        raw_clusters = cluster_hits(raw_size, dict(remaining_hits),
                                      window_size, density_threshold,
                                      min_cluster_hits,
                                      gap_multiplier, min_gap_threshold)
        if not raw_clusters:
            break

        # Pick the best cluster for this iteration
        # Score: (distinct_coords, peak_density) — but we need to compute
        # these for each candidate first.
        best: Optional[ObjectCluster] = None
        best_score = (0, 0)
        for cstart, cend, hits_count in raw_clusters:
            hits_in = [(o, hits_by_offset[o]) for o in sorted(active_offsets)
                       if cstart <= o < cend]
            if not hits_in:
                continue
            tight_start, tight_end, hits, peak = refine_cluster_bounds(
                cstart, cend, hits_in)
            distinct = len(set(ci for _, ci in hits_in))
            # Quality filter
            if hits < min_cluster_hits:
                continue
            if distinct < 2:
                continue
            if distinct / max(hits, 1) < min_distinct_ratio:
                continue
            # Strides
            offs = [o for o, _ in hits_in]
            if len(offs) >= 2:
                strides = [offs[i + 1] - offs[i] for i in range(len(offs) - 1)]
                strides.sort()
                stride_min = strides[0]
                stride_median = strides[len(strides) // 2]
            else:
                stride_min = 0
                stride_median = 0
            score = (distinct, peak)
            if score > best_score:
                best_score = score
                best = ObjectCluster(
                    name="",
                    start=tight_start,
                    end=tight_end,
                    hits=hits,
                    distinct_coords=distinct,
                    stride_min=stride_min,
                    stride_median=stride_median,
                    peak_density=peak,
                )

        if best is None:
            break

        # Assign name
        idx = len(result)
        best.name = CLUSTER_NAMES[idx] if idx < len(CLUSTER_NAMES) else f"other_{idx + 1}"
        result.append(best)

        # Mask out hits inside this cluster
        masked_count = 0
        to_remove = set()
        for o in active_offsets:
            if best.start <= o < best.end:
                to_remove.add(o)
                masked_count += 1
        active_offsets -= to_remove

        if not active_offsets:
            break

    return result


# ============================================================================
# Step 4: assign per-object offsets
# ============================================================================

def assign_object_offsets(clusters: List[ObjectCluster],
                           hits_by_ci: Dict[int, List[int]]) -> Dict[int, ObjectOffsets]:
    """
    For each coord_int, find its FIRST hit offset inside each cluster.
    These are the offsets of that object's record in that cluster.

    Returns Dict[coord_int, ObjectOffsets].
    """
    if not clusters:
        return {}

    result: Dict[int, ObjectOffsets] = defaultdict(ObjectOffsets)

    # Sort hits per ci (they should already be sorted from find_coord_hits)
    for ci, offs in hits_by_ci.items():
        all_offs = list(offs)
        result[ci].save_offsets = all_offs
        # For each cluster, find first hit inside it
        for c in clusters:
            for o in offs:
                if c.start <= o < c.end:
                    if c.name == "main" and result[ci].main_offset is None:
                        result[ci].main_offset = o
                    elif c.name == "visiting" and result[ci].visiting_offset is None:
                        result[ci].visiting_offset = o
                    elif c.name == "fog" and result[ci].fog_offset is None:
                        result[ci].fog_offset = o
                    elif c.name == "alive" and result[ci].alive_offset is None:
                        result[ci].alive_offset = o
                    elif c.name == "treasure" and result[ci].treasure_offset is None:
                        result[ci].treasure_offset = o
                    elif c.name == "decoration" and result[ci].decoration_offset is None:
                        result[ci].decoration_offset = o
                    break  # only the first hit per cluster

    return dict(result)


# ============================================================================
# Convenience: full pipeline
# ============================================================================

def find_all_object_clusters(raw: bytes,
                              coord_ints: Set[int],
                              scan_start: int = 0,
                              window_size: int = 4096,
                              density_threshold: Optional[int] = None,
                              min_cluster_hits: int = 3,
                              gap_multiplier: int = 8,
                              min_gap_threshold: int = 0x400,
                              min_distinct_ratio: float = 0.10,
                              max_clusters: int = 10) -> Tuple[
                                  List[ObjectCluster],
                                  Dict[int, ObjectOffsets]]:
    """
    Run the full pipeline. Returns (clusters, object_offsets).

    Args:
        raw:                decompressed save bytes
        coord_ints:         set of valid coord_ints from MapData
        scan_start:         byte offset to start scanning (header_size)
        window_size:        kept for API compat (unused in gap-based algo)
        density_threshold:  kept for API compat (unused)
        min_cluster_hits:   clusters with fewer hits are discarded
        gap_multiplier:     threshold = max(min_gap_threshold, mult × median)
        min_gap_threshold:  floor for the threshold (1 KB by default)
        min_distinct_ratio: quality filter — drop clusters where
                            distinct/hits < this ratio (default 5%)
        max_clusters:       stop after finding this many clusters

    Returns:
        clusters:       List[ObjectCluster], sorted by quality (main first)
        object_offsets: Dict[coord_int, ObjectOffsets]
    """
    hits_by_ci = find_coord_hits(raw, coord_ints, scan_start)
    clusters = build_clusters(len(raw), hits_by_ci, window_size,
                               density_threshold, min_cluster_hits,
                               gap_multiplier, min_gap_threshold,
                               min_distinct_ratio, max_clusters)
    object_offsets = assign_object_offsets(clusters, hits_by_ci)
    return clusters, object_offsets


# ============================================================================
# Self-test
# ============================================================================

if __name__ == "__main__":
    import sys
    import gzip
    import io
    sys.path.insert(0, "/home/z/my-project/H3save_reader/01_tools")
    from map_json_loader import load_map_json_safely
    from header_parser import parse_header

    def decompress(path: str) -> bytes:
        raw = open(path, "rb").read()
        if raw[:2] != b"\x1f\x8b":
            return raw
        out = b""
        gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
        try:
            while True:
                chunk = gz.read(65536)
                if not chunk:
                    break
                out += chunk
        except Exception:
            pass
        return out

    cases = [
        ("save_parse_test_01",
         "/home/z/my-project/H3save_reader/examples/save_parse_test_01/save_parse_test_01.h3m.zip",
         "/home/z/my-project/H3save_reader/examples/save_parse_test_01/0000.GM1"),
        ("Myth and Legend",
         "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/Myth and Legend.h3m.zip",
         "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/0000.GM1"),
    ]
    for name, mzip, d0 in cases:
        print(f"\n{'='*72}\n{name}\n{'='*72}")
        md, err = load_map_json_safely(mzip)
        if err:
            print(f"  map JSON error: {err}")
            continue
        raw = decompress(d0)
        h = parse_header(raw)
        print(f"  header_size: {h.header_size:#X}  (map_size={h.map_size}, UG={h.has_underground})")
        coord_ints = set(md.coord_int_lookup.keys())
        print(f"  coord_ints: {len(coord_ints)}")
        clusters, obj_off = find_all_object_clusters(raw, coord_ints,
                                                       scan_start=h.header_size)
        print(f"  clusters: {len(clusters)}")
        for c in clusters:
            print(f"    {c.name:15s} {c.start_hex}..{c.end_hex}  hits={c.hits:5d}  "
                  f"distinct={c.distinct_coords:5d}  stride_min={c.stride_min:4d}  "
                  f"stride_med={c.stride_median:5d}  peak={c.peak_density}")
        print(f"  object_offsets: {len(obj_off)} entries")
        # Sample
        if obj_off:
            sample = list(obj_off.items())[:3]
            for ci, oo in sample:
                print(f"    ci={ci}: {oo.to_dict()}")
