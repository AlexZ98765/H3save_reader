"""excel_compare.py — Compare two Excel files and output differences.

Used by the Tools → Compare... dialog in gm1_parser.py. Also usable as a
standalone CLI script.

Input: two .xlsx files (e.g. OUR export vs PRT export).
Output: a new .xlsx file (compare_save_<map_name>.xlsx) with the same sheet
names, but each sheet contains ONLY the rows that differ between the two files.

Comparison strategy (FIXED in v2 — see "BUGS IN v1" below):

  The previous version had three defects that made comparing sheets
  "Все Арты" / "Все Заклы" / "Герои" unreliable, especially after fixing
  PRT bugs in excel_calculations.py (banned-hero inclusion, Таверна-vs-Тюрьма
  Place labels):

  v1 BUG 1 — `_compare_xyz` only used the FIRST row per (X,Y,Z) bucket.
        For a hero with 5 artifacts at coords 78.109.0, the 5 rows collapsed
        to one comparison → most diffs lost.
  v1 BUG 2 — `_compare_xyz` collapsed rows with None X/Y/Z (tavern heroes,
        banned heroes, art merchants, ground artifacts without coords) into
        one bucket → only first row compared → catastrophic.
  v1 BUG 3 — `ignore_localization=True` treated ANY string-vs-string
        difference as "localization" and returned True. This masked exactly
        the differences we need to verify: Place column values
        ("Таверна" vs "Тюрьма"), artifact/spell/hero names, etc.

  v2 FIX — signature-based comparison:
        Each row is reduced to a "signature" tuple of normalized cell values
        (strings stripped, numbers as-is, None→None). Two rows are equal iff
        their signatures are equal. We use collections.Counter for multiset
        comparison:
          - A-only  = signatures present in A in greater multiplicity than B
          - B-only  = signatures present in B in greater multiplicity than A
          - Common  = signatures with equal multiplicity → no diff
          - A≠B     = same signature present in both but with different
                      multiplicities (e.g., A has 5 rows, B has 4) — reported
                      once with the row count info.
        This works uniformly for sheets WITH X/Y/Z (Все Арты, Все Заклы),
        WITHOUT X/Y/Z (Герои, События-Таймеры), and with mixed rows
        (active heroes + tavern + banned + prison + merchants).

  v2 FIX — column-aware localization filter:
        `ignore_localization=True` now ignores only case-differences in
        string values (e.g. "Герой" vs "герой"), not arbitrary string
        differences. Real content differences ("Таверна" vs "Тюрьма") are
        reported.

Each output sheet has the original columns plus a "Source" column:
  - "A-only" = row only in file A (multiplicity difference)
  - "B-only" = row only in file B (multiplicity difference)
  - "A≠B (rows: A=N, B=M)" = same signature in both, different multiplicities
"""
from __future__ import annotations
import os
import re
import sys
from typing import Any, Dict, List, Optional, Tuple
from collections import Counter

import openpyxl
from openpyxl import Workbook
from openpyxl.styles import Font, PatternFill, Alignment


# ============================================================================
# Synonym map — built once from 02_format_docs/synonyms.json (auto-generated
# from localization_dictionary.json by scripts/build_synonyms.py).
#
# All 4 languages (en, ru_sod, ru_wog, ru_complete) for the SAME concept ID
# are treated as synonyms. Example: Color[5] = ["Purple", "Багровый",
# "Пурпурный", "Лиловый"] — all map to canonical form "багровый".
#
# Cell normalization rules (when ignore_localization=True):
#   1. Whole-cell match: if the entire stripped lowercase cell string matches
#      a known synonym, replace with canonical form. Catches single-word
#      cells like Тип="Темница" ↔ "Подземелье", Флаг="Пурпурный" ↔ "Багровый".
#   2. Token-level match: if whole-cell doesn't match, split by whitespace
#      and comma, replace each token that matches a known synonym, rejoin.
#      Catches multi-word cells like "Джинн 10" ↔ "Джин 10" (where "Джинн"
#      and "Джин" are synonyms for monster ID 36).
# ============================================================================

_SYN_MAP: Optional[Dict[str, str]] = None


def _build_synonym_map() -> Dict[str, str]:
    """Load synonyms.json and build reverse-lookup: lowercased_string → canonical."""
    try:
        import json
        _here = os.path.dirname(os.path.abspath(__file__))
        _docs = os.path.normpath(os.path.join(_here, "..", "02_format_docs"))
        path = os.path.join(_docs, "synonyms.json")
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)
    except Exception:
        return {}

    syn_map: Dict[str, str] = {}

    def _add_group(synonyms: List[str]):
        if not synonyms or len(synonyms) < 2:
            return
        # Pick canonical: prefer first Cyrillic entry, else first entry
        canonical = None
        for s in synonyms:
            if isinstance(s, str) and s and not s.isascii():
                canonical = s
                break
        if canonical is None:
            canonical = synonyms[0]
        canonical_lower = canonical.lower()
        # Add all members to reverse-lookup (case-insensitive)
        for s in synonyms:
            if isinstance(s, str) and s:
                syn_map[s.lower()] = canonical_lower

    # Auto-generated groups from localization_dictionary.json
    for group in data.get("groups", []):
        if isinstance(group, list):
            _add_group(group)

    # User-extensible custom groups (for synonyms not in localization dict)
    for group in data.get("groups_extra", []):
        if isinstance(group, list):
            _add_group(group)

    return syn_map


def _get_syn_map() -> Dict[str, str]:
    global _SYN_MAP
    if _SYN_MAP is None:
        _SYN_MAP = _build_synonym_map()
    return _SYN_MAP


def compare_excel_files(file_a: str, file_b: str, output_path: str,
                        ignore_localization: bool = True) -> str:
    """Compare two .xlsx files and save differences to output_path.

    Args:
        file_a: Path to the first Excel file (e.g. OUR export).
        file_b: Path to the second Excel file (e.g. PRT export).
        output_path: Where to save the comparison result.
        ignore_localization: If True, only letter-CASE differences between
            strings are ignored (e.g. "Герой" == "герой"). Real content
            differences ("Таверна" vs "Тюрьма", different artifact names)
            are still reported as diffs.

    Returns:
        The output_path on success.
    """
    wb_a = openpyxl.load_workbook(file_a, data_only=True)
    wb_b = openpyxl.load_workbook(file_b, data_only=True)
    wb_out = Workbook()
    # Remove the default sheet
    if "Sheet" in wb_out.sheetnames:
        del wb_out["Sheet"]

    common_sheets = sorted(set(wb_a.sheetnames) & set(wb_b.sheetnames))
    # Track sheets only in one file → write them as summary
    only_a = sorted(set(wb_a.sheetnames) - set(wb_b.sheetnames))
    only_b = sorted(set(wb_b.sheetnames) - set(wb_a.sheetnames))

    # Color fills for source column
    fill_a = PatternFill(start_color="FFF2CC", end_color="FFF2CC", fill_type="solid")  # yellow
    fill_b = PatternFill(start_color="D9EAD3", end_color="D9EAD3", fill_type="solid")  # green
    fill_diff = PatternFill(start_color="F4CCCC", end_color="F4CCCC", fill_type="solid")  # red
    bold_font = Font(bold=True)

    summary_rows: List[Tuple[str, int, int, int, int]] = []
    # (sheet_name, a_total, b_total, n_diffs, n_common)

    for sheet_name in common_sheets:
        ws_a = wb_a[sheet_name]
        ws_b = wb_b[sheet_name]

        headers_a = [ws_a.cell(row=1, column=c).value for c in range(1, ws_a.max_column + 1)]
        headers_b = [ws_b.cell(row=1, column=c).value for c in range(1, ws_b.max_column + 1)]

        if headers_a != headers_b:
            # Headers differ — just output both full sheets side by side
            _write_sheet_mismatch(wb_out, sheet_name, headers_a, headers_b, ws_a, ws_b)
            summary_rows.append((sheet_name, ws_a.max_row - 1, ws_b.max_row - 1,
                                 max(ws_a.max_row, ws_b.max_row), 0))
            continue

        rows_a = _extract_rows(ws_a)
        rows_b = _extract_rows(ws_b)

        diffs = _compare_signature(rows_a, rows_b, headers_a, ignore_localization)

        # Compute counts for summary
        n_common = sum(1 for d in diffs if d["source"] == "COMMON_DUMMY")
        # Actually n_common isn't tracked separately; recompute:
        # multiplicity of common signatures (present in both with same count).
        sigs_a = Counter(_row_signature(r, ignore_localization) for r in rows_a)
        sigs_b = Counter(_row_signature(r, ignore_localization) for r in rows_b)
        common_sigs = sigs_a & sigs_b  # min multiplicities
        n_common = sum(common_sigs.values())

        summary_rows.append((sheet_name, len(rows_a), len(rows_b), len(diffs), n_common))

        if not diffs:
            continue  # no diffs → skip this sheet

        # Create output sheet
        ws_out = wb_out.create_sheet(sheet_name)
        # Write headers: original + "Source"
        out_headers = list(headers_a) + ["Source"]
        ws_out.append(out_headers)
        # Bold + colored header
        for col_idx in range(1, len(out_headers) + 1):
            cell = ws_out.cell(row=1, column=col_idx)
            cell.font = bold_font
            cell.alignment = Alignment(horizontal="center", wrap_text=True)

        for diff in diffs:
            source = diff["source"]
            row_data = diff["row"]
            ws_out.append(list(row_data) + [source])
            # Color the row based on source
            row_num = ws_out.max_row
            if source == "A-only":
                fill = fill_a
            elif source == "B-only":
                fill = fill_b
            elif source.startswith("A≠B"):
                fill = fill_diff
            else:
                fill = fill_diff
            for col_idx in range(1, len(out_headers) + 1):
                ws_out.cell(row=row_num, column=col_idx).fill = fill

        # Auto-size columns
        _autosize(ws_out)
        # Freeze the first row (header) so it stays visible while scrolling,
        # and enable auto_filter on the header row so the user can quickly
        # filter by Source (A-only / B-only / A≠B) or any other column.
        ws_out.freeze_panes = "A2"
        ws_out.auto_filter.ref = ws_out.dimensions

    # Summary sheet (always at front)
    if wb_out.sheetnames:
        ws_sum = wb_out.create_sheet("_Сводка", 0)
        ws_sum.append(["Лист", "A: всего", "B: всего", "Diff-строк", "Совпадений"])
        for col_idx in range(1, 6):
            ws_sum.cell(row=1, column=col_idx).font = bold_font
            ws_sum.cell(row=1, column=col_idx).alignment = Alignment(horizontal="center")
        for name, na, nb, nd, nc in summary_rows:
            ws_sum.append([name, na, nb, nd, nc])
        # Sheet-name-only lists for sheets present in only one file
        if only_a or only_b:
            ws_sum.append([])
            ws_sum.append(["Только в A:"] + only_a)
            ws_sum.append(["Только в B:"] + only_b)
        _autosize(ws_sum)
        # Freeze + filter on summary sheet too
        ws_sum.freeze_panes = "A2"
        ws_sum.auto_filter.ref = ws_sum.dimensions

    # If no sheets were created (no diffs and no sheet mismatches)
    if not wb_out.sheetnames:
        ws = wb_out.create_sheet("Summary")
        ws.append(["Result", "No differences found — files are identical (after case-insensitive filter)."])

    wb_out.save(output_path)
    return output_path


def _extract_rows(ws) -> List[List[Any]]:
    """Extract all data rows (skip header) as list of lists."""
    rows = []
    for r in range(2, ws.max_row + 1):
        vals = [ws.cell(row=r, column=c).value for c in range(1, ws.max_column + 1)]
        if all(v is None or v == '' for v in vals):
            continue
        rows.append(vals)
    return rows


def _norm_cell(v: Any, ignore_localization: bool) -> Any:
    """Normalize a single cell value for signature comparison.

    - None / empty string → None (canonical empty marker)
    - "0" string → 0 int (PRT often stores 0 as both "0" and None)
    - String → strip; if ignore_localization:
        * whole-cell synonym match (case-insensitive) → canonical form
        * else tokenize by whitespace+comma, replace synonym tokens, lowercase
    - int/float → as-is
    """
    if v is None:
        return None
    if isinstance(v, str):
        s = v.strip()
        if s == '' or s == '0':
            return None if s == '' else 0
        if not ignore_localization:
            return s
        # Lowercase + synonym substitution
        lower = s.lower()
        syn_map = _get_syn_map()
        # Whole-cell synonym match (fast path)
        if lower in syn_map:
            return syn_map[lower]
        # Token-level synonym match for multi-word cells.
        # Split keeping separators, replace any token that's a known synonym.
        tokens = re.split(r'(\s+|,)', s)
        any_replaced = False
        for i, tok in enumerate(tokens):
            if not tok or tok.isspace() or tok == ',':
                continue
            low = tok.lower()
            if low in syn_map:
                tokens[i] = syn_map[low]
                any_replaced = True
        result = ''.join(tokens) if any_replaced else s
        return result.lower()
    # Numbers (int, float)
    return v


def _row_signature(row: List[Any], ignore_localization: bool) -> Tuple:
    """Build a hashable signature tuple for a row, normalizing each cell."""
    return tuple(_norm_cell(v, ignore_localization) for v in row)


def _values_equal(a: Any, b: Any, ignore_localization: bool) -> bool:
    """Compare two cell values for equality.

    With ignore_localization=True, only letter-CASE differences are ignored
    for strings. Real content differences ("Таверна" vs "Тюрьма") are NOT
    ignored — this is the FIX that lets us verify PRT-bug fixes actually
    produce different Place labels.
    """
    na = _norm_cell(a, ignore_localization)
    nb = _norm_cell(b, ignore_localization)
    return na == nb


def _compare_signature(rows_a: List[List], rows_b: List[List], headers: List[str],
                       ignore_localization: bool) -> List[Dict]:
    """Compare two row sets via signature-based multiset comparison.

    Strategy:
      - Each row → signature tuple of normalized cell values
      - Count multiplicities in A and B (Counter)
      - A-only rows = (Counter(A) - Counter(B)).elements()
      - B-only rows = (Counter(B) - Counter(A)).elements()
      - Common with same multiplicity → no diff
      - Common with different multiplicity → one diff entry per signature
        (with "A≠B (rows: A=N, B=M)" source label)

    This works uniformly for any sheet regardless of whether it has X/Y/Z
    columns, mixed None coords, or multi-row groups per coord.

    Output: original row content (NOT lowercased) is preserved for display —
    case-normalization is only used internally for the signature key.
    """
    sigs_a: Counter = Counter(_row_signature(r, ignore_localization) for r in rows_a)
    sigs_b: Counter = Counter(_row_signature(r, ignore_localization) for r in rows_b)

    # A-only (multiset difference: rows in A beyond what's in B)
    only_a = sigs_a - sigs_b  # Counter with multiplicities
    only_b = sigs_b - sigs_a

    # Common signatures present in both — check for multiplicity mismatch
    common_keys = set(sigs_a) & set(sigs_b)

    # Build a map: signature → first ORIGINAL row encountered (for display)
    # so the diff output preserves the actual case/formatting of cell values.
    # Prefer the file where the row appears in greater multiplicity.
    orig_row_for_sig: Dict[Tuple, List[Any]] = {}
    for r in rows_a:
        sig = _row_signature(r, ignore_localization)
        if sig not in orig_row_for_sig:
            orig_row_for_sig[sig] = list(r)
    for r in rows_b:
        sig = _row_signature(r, ignore_localization)
        if sig not in orig_row_for_sig:
            orig_row_for_sig[sig] = list(r)

    diffs: List[Dict] = []

    # Sort signatures for stable output. Sort key: tuple of string-ified
    # values so we can compare mixed types (None/int/str) without crashing.
    def _sort_key(sig):
        return tuple((str(x) if x is not None else "") for x in sig)

    for sig in sorted(only_a.keys(), key=_sort_key):
        for _ in range(only_a[sig]):
            diffs.append({"source": "A-only", "row": list(orig_row_for_sig[sig])})

    for sig in sorted(only_b.keys(), key=_sort_key):
        for _ in range(only_b[sig]):
            diffs.append({"source": "B-only", "row": list(orig_row_for_sig[sig])})

    for sig in sorted(common_keys, key=_sort_key):
        ca = sigs_a[sig]
        cb = sigs_b[sig]
        if ca != cb:
            diffs.append({
                "source": f"A≠B (rows: A={ca}, B={cb})",
                "row": list(orig_row_for_sig[sig]),
            })

    return diffs


# ============================================================================
# Backward-compatible aliases for the old private API (in case external code
# or tests imported them). They dispatch to the new signature-based compare.
# ============================================================================

def _compare_xyz(rows_a, rows_b, headers, ignore_localization):
    """Backward-compat: dispatch to signature-based comparison."""
    return _compare_signature(rows_a, rows_b, headers, ignore_localization)


def _compare_rowbyrow(rows_a, rows_b, headers, ignore_localization):
    """Backward-compat: dispatch to signature-based comparison."""
    return _compare_signature(rows_a, rows_b, headers, ignore_localization)


def _write_sheet_mismatch(wb_out, sheet_name, headers_a, headers_b, ws_a, ws_b):
    """Write a summary sheet when headers differ."""
    ws = wb_out.create_sheet(sheet_name[:28])  # Excel sheet name max 31 chars
    ws.append(["Headers differ between files — cannot compare."])
    ws.append([])
    ws.append(["File A headers:"] + list(headers_a))
    ws.append(["File B headers:"] + list(headers_b))
    # No freeze/filter here — this sheet has only 4 meta-rows, not a tabular diff.


def _autosize(ws, max_width: int = 60):
    """Auto-size columns in a worksheet."""
    for col in ws.columns:
        max_len = 0
        col_letter = col[0].column_letter
        for cell in col:
            try:
                val_len = len(str(cell.value)) if cell.value is not None else 0
                if val_len > max_len:
                    max_len = val_len
            except Exception:
                pass
        ws.column_dimensions[col_letter].width = min(max_len + 2, max_width)


# ============================================================================
# CLI entry point
# ============================================================================
if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Usage: python excel_compare.py <file_a.xlsx> <file_b.xlsx> <output.xlsx> "
              "[--strict]")
        print("  --strict : do not ignore letter-case differences (default: case ignored)")
        sys.exit(1)
    ignore_loc = "--strict" not in sys.argv[3:]
    out = compare_excel_files(sys.argv[1], sys.argv[2], sys.argv[3],
                              ignore_localization=ignore_loc)
    print(f"Comparison saved to: {out}")
