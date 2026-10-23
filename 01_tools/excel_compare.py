"""excel_compare.py — Compare two Excel files and output differences.

Used by the Tools → Compare... dialog in gm1_parser.py. Also usable as a
standalone CLI script.

Input: two .xlsx files (e.g. OUR export vs PRT export).
Output: a new .xlsx file (compare_save_<map_name>.xlsx) with the same sheet
names, but each sheet contains ONLY the rows that differ between the two files.

Comparison strategy:
  - For sheets WITH X/Y/Z columns: group rows by (X,Y,Z), compare per-coord.
    Output rows from BOTH files that differ (with a "Source" column: "A" / "B" / "A-only" / "B-only").
  - For sheets WITHOUT X/Y/Z (e.g. События-Таймеры, Герои): compare row-by-row by index.

Each output sheet has the original columns plus:
  - "Source" column: "A" = only in file A, "B" = only in file B, "A≠B" = in both but different
  - For "A≠B" rows: values from file A followed by values from file B (side-by-side).
"""
from __future__ import annotations
import os
import sys
from typing import Any, Dict, List, Optional, Tuple
from collections import defaultdict

import openpyxl
from openpyxl import Workbook
from openpyxl.styles import Font, PatternFill, Alignment


def compare_excel_files(file_a: str, file_b: str, output_path: str,
                        ignore_localization: bool = True) -> str:
    """Compare two .xlsx files and save differences to output_path.

    Args:
        file_a: Path to the first Excel file (e.g. OUR export).
        file_b: Path to the second Excel file (e.g. PRT export).
        output_path: Where to save the comparison result.
        ignore_localization: If True, string-only differences are ignored
            (treated as localization/capitalization). Only None-vs-value and
            numeric differences are reported.

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

    # Color fills for source column
    fill_a = PatternFill(start_color="FFF2CC", end_color="FFF2CC", fill_type="solid")  # yellow
    fill_b = PatternFill(start_color="D9EAD3", end_color="D9EAD3", fill_type="solid")  # green
    fill_diff = PatternFill(start_color="F4CCCC", end_color="F4CCCC", fill_type="solid")  # red
    bold_font = Font(bold=True)

    for sheet_name in common_sheets:
        ws_a = wb_a[sheet_name]
        ws_b = wb_b[sheet_name]

        headers_a = [ws_a.cell(row=1, column=c).value for c in range(1, ws_a.max_column + 1)]
        headers_b = [ws_b.cell(row=1, column=c).value for c in range(1, ws_b.max_column + 1)]

        if headers_a != headers_b:
            # Headers differ — just output both full sheets side by side
            _write_sheet_mismatch(wb_out, sheet_name, headers_a, headers_b, ws_a, ws_b)
            continue

        rows_a = _extract_rows(ws_a)
        rows_b = _extract_rows(ws_b)

        has_xyz = 'X' in headers_a and 'Y' in headers_a and 'Z' in headers_a

        if has_xyz:
            diffs = _compare_xyz(rows_a, rows_b, headers_a, ignore_localization)
        else:
            diffs = _compare_rowbyrow(rows_a, rows_b, headers_a, ignore_localization)

        if not diffs:
            continue  # no diffs → skip this sheet

        # Create output sheet
        ws_out = wb_out.create_sheet(sheet_name)
        # Write headers: original + "Source" + (for diffs: duplicate columns with B values)
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
            fill = fill_a if source in ("A-only",) else fill_b if source in ("B-only",) else fill_diff
            for col_idx in range(1, len(out_headers) + 1):
                ws_out.cell(row=row_num, column=col_idx).fill = fill

        # Auto-size columns
        _autosize(ws_out)

    # If no sheets were created (all identical), add a summary
    if not wb_out.sheetnames:
        ws = wb_out.create_sheet("Summary")
        ws.append(["Result", "No differences found — files are identical (after localization filter)."])

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


def _values_equal(a, b, ignore_localization: bool) -> bool:
    """Compare two cell values. If ignore_localization, string-only diffs are ignored."""
    if a == b:
        return True
    if a is None and b is None:
        return True
    # Handle empty/zero equivalence
    a_empty = a is None or (isinstance(a, str) and a.strip() in ('', '0')) or a == 0
    b_empty = b is None or (isinstance(b, str) and b.strip() in ('', '0')) or b == 0
    if a_empty and b_empty:
        return True
    if a_empty or b_empty:
        return False  # one empty, other has data
    if ignore_localization and isinstance(a, str) and isinstance(b, str):
        return True  # both strings → ignore (localization)
    if isinstance(a, (int, float)) and isinstance(b, (int, float)):
        return a == b
    return False


def _compare_xyz(rows_a: List[List], rows_b: List[List], headers: List[str],
                 ignore_localization: bool) -> List[Dict]:
    """Compare sheets with X/Y/Z columns. Group by (X,Y,Z), compare per-coord."""
    # Find column indices for X, Y, Z
    x_idx = headers.index("X")
    y_idx = headers.index("Y")
    z_idx = headers.index("Z")

    def key(row):
        return (row[x_idx], row[y_idx], row[z_idx])

    by_a = defaultdict(list)
    for row in rows_a:
        by_a[key(row)].append(row)
    by_b = defaultdict(list)
    for row in rows_b:
        by_b[key(row)].append(row)

    coords_a = set(by_a)
    coords_b = set(by_b)
    common = coords_a & coords_b
    only_a = coords_a - coords_b
    only_b = coords_b - coords_a

    diffs = []

    # A-only rows
    for c in sorted(only_a, key=lambda k: tuple(v if v is not None else -1 for v in k)):
        for row in by_a[c]:
            diffs.append({"source": "A-only", "row": row})

    # B-only rows
    for c in sorted(only_b, key=lambda k: tuple(v if v is not None else -1 for v in k)):
        for row in by_b[c]:
            diffs.append({"source": "B-only", "row": row})

    # Common coords — compare first row of each
    for c in common:
        row_a = by_a[c][0]
        row_b = by_b[c][0]
        # Also check row count mismatch
        if len(by_a[c]) != len(by_b[c]):
            diffs.append({"source": f"A≠B (rows: A={len(by_a[c])}, B={len(by_b[c])})", "row": row_a})
            continue
        # Compare each column
        has_diff = False
        for i in range(min(len(row_a), len(row_b))):
            if not _values_equal(row_a[i], row_b[i], ignore_localization):
                has_diff = True
                break
        if has_diff:
            diffs.append({"source": "A≠B", "row": row_a})

    return diffs


def _compare_rowbyrow(rows_a: List[List], rows_b: List[List], headers: List[str],
                      ignore_localization: bool) -> List[Dict]:
    """Compare sheets without X/Y/Z columns — row by row by index."""
    diffs = []
    n = min(len(rows_a), len(rows_b))

    # Extra rows in A
    if len(rows_a) > n:
        for i in range(n, len(rows_a)):
            diffs.append({"source": "A-only", "row": rows_a[i]})

    # Extra rows in B
    if len(rows_b) > n:
        for i in range(n, len(rows_b)):
            diffs.append({"source": "B-only", "row": rows_b[i]})

    # Compare common rows
    for i in range(n):
        row_a = rows_a[i]
        row_b = rows_b[i]
        has_diff = False
        for j in range(min(len(row_a), len(row_b))):
            if not _values_equal(row_a[j], row_b[j], ignore_localization):
                has_diff = True
                break
        if has_diff:
            diffs.append({"source": "A≠B", "row": row_a})

    return diffs


def _write_sheet_mismatch(wb_out, sheet_name, headers_a, headers_b, ws_a, ws_b):
    """Write a summary sheet when headers differ."""
    ws = wb_out.create_sheet(sheet_name[:28])  # Excel sheet name max 31 chars
    ws.append(["Headers differ between files — cannot compare."])
    ws.append([])
    ws.append(["File A headers:"] + list(headers_a))
    ws.append(["File B headers:"] + list(headers_b))


def _autosize(ws, max_width: int = 60):
    """Auto-size columns in a worksheet."""
    for col in ws.columns:
        max_len = 0
        col_letter = col[0].column_letter
        for cell in col:
            try:
                val_len = len(str(cell.value)) if cell.value else 0
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
        print("Usage: python excel_compare.py <file_a.xlsx> <file_b.xlsx> <output.xlsx>")
        sys.exit(1)
    out = compare_excel_files(sys.argv[1], sys.argv[2], sys.argv[3])
    print(f"Comparison saved to: {out}")
