"""excel_compare.py — Compare two Excel files and output differences.

Used by the Tools → Compare... dialog in gm1_parser.py. Also usable as a
standalone CLI script.

Input: two .xlsx files (e.g. OUR export vs PRT export).
Output: a new .xlsx file (compare_save_<map_name>.xlsx) with the same sheet
names, but each sheet contains ONLY the rows that differ between the two files.

ARCHITECTURE (v3 — column-aware ID-based, no fuzzy string matching):

  The previous version used fuzzy string matching (whole-cell + phrase-level +
  token-level + ё→е normalization) to collapse translation synonyms. This was
  WRONG — it was over-engineered and produced over-merges.

  The CORRECT approach (per user direction):
    1. We know what each column contains (e.g. "Охрана" = monster names+counts,
       "Флаг" = player color, "Тип" = town type).
    2. We know which localization section covers each column's values
       (e.g. "Охрана" → Monstr section, "Флаг" → Color section).
    3. For each cell value, we reverse-lookup in the appropriate section →
       get the ID(s) that this value is a translation of.
    4. The ID is the canonical comparison key. All translations of the same
       ID (across en/ru_sod/ru_wog/ru_complete) collapse to the same canonical.
    5. No fuzzy matching, no ё→е conversion, no phrase splitting.

  Compound columns (guard strings, spell lists, etc.) are parsed into
  components first, then each component is reverse-looked-up by ID.

  Columnar section mapping is built from column HEADER + sheet context
  (some columns like "Герой" have different meanings per sheet).

  Reverse-lookup returns a SET of IDs (some values are ambiguous — e.g.
  "Стрелок" is both ru_complete for Archer (ID=2) AND ru_sod for Marksman
  (ID=3)). For deterministic comparison, we pick the LOWEST ID. This works
  for OUR vs PRT comparison because both use the same localizer (ru_sod),
  so both resolve to the same lowest ID.

Each output sheet has the original columns plus a "Source" column:
  - "A-only" = row only in file A (multiplicity difference)
  - "B-only" = row only in file B (multiplicity difference)
  - "A≠B (rows: A=N, B=M)" = same signature in both, different multiplicities
"""
from __future__ import annotations
import os
import re
import sys
import json
from typing import Any, Dict, List, Optional, Tuple
from collections import Counter

import openpyxl
from openpyxl import Workbook
from openpyxl.styles import Font, PatternFill, Alignment


# ============================================================================
# Localization reverse-lookup — built once from localization_dictionary.json
# ============================================================================

_LOC_DATA: Optional[Dict] = None
_REVERSE_LOOKUP: Optional[Dict[str, Dict[str, set]]] = None
_LANGS_ORDER = ["en", "ru_sod", "ru_wog", "ru_complete"]


def _load_localization() -> Dict:
    """Load localization_dictionary.json (the single source of truth)."""
    global _LOC_DATA
    if _LOC_DATA is None:
        try:
            _here = os.path.dirname(os.path.abspath(__file__))
            _docs = os.path.normpath(os.path.join(_here, "..", "02_format_docs"))
            path = os.path.join(_docs, "localization_dictionary.json")
            with open(path, "r", encoding="utf-8") as f:
                _LOC_DATA = json.load(f)
        except Exception:
            _LOC_DATA = {}
    return _LOC_DATA


def _build_reverse_lookup() -> Dict[str, Dict[str, set]]:
    """For each section, build reverse-lookup: lowercased_value → set of IDs.

    All 4 languages are indexed. Keys are lowercased for case-insensitive
    matching. Values are SETS of IDs (a value may be a translation of
    multiple IDs — e.g. "Стрелок" is both Archer (ID=2) ru_complete and
    Marksman (ID=3) ru_sod).
    """
    global _REVERSE_LOOKUP
    if _REVERSE_LOOKUP is not None:
        return _REVERSE_LOOKUP
    loc = _load_localization()
    rev: Dict[str, Dict[str, set]] = {}
    for sec_name, sec in loc.get("sections", {}).items():
        if not isinstance(sec, dict):
            continue
        sec_rev: Dict[str, set] = {}
        langs_data = sec.get("languages", {})
        if not isinstance(langs_data, dict):
            continue
        for lang in _LANGS_ORDER:
            lang_dict = langs_data.get(lang, {})
            if not isinstance(lang_dict, dict):
                continue
            for id_str, value in lang_dict.items():
                if not isinstance(value, str):
                    continue
                v = value.strip()
                if not v:
                    continue
                # Case-insensitive key (lowercase only — NO ё→е conversion)
                key = v.lower()
                sec_rev.setdefault(key, set()).add(int(id_str))
        rev[sec_name] = sec_rev
    _REVERSE_LOOKUP = rev
    return rev


def _reverse_lookup(section: str, value: str) -> Optional[set]:
    """Reverse-lookup a value in a localization section → set of IDs.

    Returns None if section doesn't exist or value not found.
    Returns set of IDs (possibly multiple) if found.
    Case-insensitive (value is lowercased before lookup).
    """
    rev = _build_reverse_lookup().get(section)
    if not rev:
        return None
    return rev.get(value.lower())


# ============================================================================
# Column → section mapping (column-aware comparison)
# ============================================================================

# Default section per column header (when not in SHEET_COLUMN_OVERRIDES).
# Special types prefixed with "_" are compound (parsed into components).
_COLUMN_SECTION_DEFAULT: Dict[str, str] = {
    # Direct-lookup columns (single value, reverse-lookup → ID)
    "Объект": "Object",
    "Класс": "ArtClass",
    "Реликт-С": "Art",
    "Флаг": "Color",
    "Тип": "Town",
    "Навык": "Skill",
    "Кукла": "Doll",
    "Местоположение": "Place",
    "Задание": "Quest",
    "Цвет": "Color",
    "Построен": "Reply",  # Да/Нет
    "Доступен": "Reply",
    "Доступно": "Reply",  # might be Color list in some sheets — override below
    "Таймер": "Reply",
    "Библиотека": "Status",
    "Можно забрать": "Reply",
    "Антимагический": "Reply",
    "Прирост": "Reply",
    "Повтор": "Reply",

    # Compound columns (parsed)
    "Охрана": "_guard_monsters",  # "Name1 count1, Name2 count2, ..."
    "Гарнизон": "_guard_monsters",
    "Монстр": "_monster_with_count",  # could be just name or "Name count, Name count"
    "Артефакт": "_art_list",  # could be single or "Art1, Art2, ..."
    "Заклинание": "_spell_list",  # could be single or "Spell1, Spell2, ..."
    "Машина": "_art_list",
    "Вторичный навык": "_skill_with_level",  # "Б.Навык, П.Навык2, ..."
    "Первичный навык": "_primary_skill",  # "K1, A2, ..."
    "Ресурс": "_resource_compound",  # "W5, G3, ..."
    "Плата": "_resource_compound",
    "Награда": "_reward_compound",  # "Опыт 500", "Навык 6 Attack +6", ...
}

# Sheet-specific column overrides (some columns have different meanings
# depending on the sheet).
# Format: {(sheet_name, column_name): section_or_special_type}
_SHEET_COLUMN_OVERRIDES: Dict[Tuple[str, str], str] = {
    # "Доступно" in События-Таймеры = list of colors (e.g. "Красный, Синий")
    ("События-Таймеры", "Доступно"): "_color_list",
    # Same for "События и Ящики Пандоры" — "Доступно" column lists colors
    ("События и Ящики Пандоры", "Доступно"): "_color_list",
    # "Монстр" in Монстры/Лагеря Беженцев = single monster name (no count)
    ("Монстры", "Монстр"): "Monstr",
    ("Лагеря Беженцев", "Монстр"): "Monstr",
    # "Герой" in События-Таймеры is actually "Город" (town name), not hero name
    # But the header there is "Город" already, not "Герой".
    # "Герой" column = hero name from save file binary (NOT in localization_dictionary)
    # → treated as plain string (no section lookup)
    ("Герои", "Герой"): "_plain_string",
    ("Все Арты", "Герой"): "_plain_string",
    ("Все Заклы", "Герой"): "_plain_string",
    ("Опыт", "Герой"): "_plain_string",
    # "Город" = town name from save file (NOT in localization_dictionary) → plain string
    ("События-Таймеры", "Город"): "_plain_string",
    ("Города", "Город"): "_plain_string",
    ("Все Заклы", "Город"): "_plain_string",
    ("Опыт", "Город"): "_plain_string",
}


def _get_column_section(sheet_name: str, column_name: str) -> Optional[str]:
    """Get the localization section (or special type) for a column.

    Returns None if no mapping known (column will be compared as plain
    lowercased string).
    """
    # Sheet-specific override takes priority
    sec = _SHEET_COLUMN_OVERRIDES.get((sheet_name, column_name))
    if sec is not None:
        return sec
    # Default mapping by header
    return _COLUMN_SECTION_DEFAULT.get(column_name)


# ============================================================================
# Cell normalization (column-aware, ID-based)
# ============================================================================

def _normalize_direct(value: str, section: str) -> str:
    """Normalize a cell value for a direct-lookup section (single value).

    Reverse-lookups the value in the section → gets set of IDs.
    Returns canonical string "Section:ID" (lowest ID for determinism).
    If value not found in section → returns lowercased value as-is
    (no canonical ID assigned; compared as plain string).
    """
    ids = _reverse_lookup(section, value)
    if ids:
        # Pick lowest ID for deterministic comparison
        return f"{section}:{min(ids)}"
    # Not found in section — fall back to plain lowercased string
    return value.lower()


def _parse_guard_components(value: str) -> List[Tuple[str, Optional[str]]]:
    """Parse a guard string into list of (name, count_str) pairs.

    Examples:
      "Тяжелый арбалетчик 5, Мечник 8" → [("Тяжелый арбалетчик", "5"), ("Мечник", "8")]
      "Маг 10" → [("Маг", "10")]
      "Скелет" → [("Скелет", None)]
    """
    parts = []
    # Split by comma
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        # Try to split name from trailing count
        # Pattern: "<name> <count>" where count is digits
        m = re.match(r"^(.*?)\s+(\d+)\s*$", chunk)
        if m:
            name = m.group(1).strip()
            count = m.group(2)
            parts.append((name, count))
        else:
            # Just a name, no count
            parts.append((chunk, None))
    return parts


def _normalize_guard_monsters(value: str) -> str:
    """Normalize a guard string by reverse-looking-up each monster name → ID.

    Returns canonical: sorted tuple of ("Monstr:ID", count) strings.
    """
    components = _parse_guard_components(value)
    canon_parts = []
    for name, count in components:
        ids = _reverse_lookup("Monstr", name)
        if ids:
            canon = f"Monstr:{min(ids)}"
        else:
            canon = f"Monstr:?:{name.lower()}"
        canon_parts.append((canon, count))
    # Sort by (canonical, count) for stable comparison
    canon_parts.sort()
    return "|".join(f"{c}:{cnt}" for c, cnt in canon_parts)


def _normalize_list_compound(value: str, section: str) -> str:
    """Normalize a comma-separated list of values from a section.

    Returns canonical: sorted tuple of "Section:ID" strings.
    """
    parts = []
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        ids = _reverse_lookup(section, chunk)
        if ids:
            parts.append(f"{section}:{min(ids)}")
        else:
            parts.append(f"{section}:?:{chunk.lower()}")
    parts.sort()
    return "|".join(parts)


def _normalize_skill_with_level(value: str) -> str:
    """Normalize "Б.Навык, П.Навык2, ..." → sorted tuple of (level_id, skill_id).

    PRT prefix codes:
      ru_sod/ru_wog: "Б." (Basic), "П." (Advanced), "Э." (Expert)
      en: "B.", "A.", "E."
      ru_complete: "1.", "2.", "3."
    All map to levels 0/1/2.
    """
    LEVEL_PREFIXES = {
        "Б.": 0, "П.": 1, "Е.": 2,  # Russian
        "B.": 0, "A.": 1, "E.": 2,  # English
        "1.": 0, "2.": 1, "3.": 2,  # Complete
    }
    parts = []
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        # Try to extract level prefix
        level_id = None
        for prefix, lvl in LEVEL_PREFIXES.items():
            if chunk.startswith(prefix):
                level_id = lvl
                chunk = chunk[len(prefix):]
                break
        # Reverse-lookup skill name → ID
        ids = _reverse_lookup("Skill", chunk)
        if ids:
            skill_canon = f"Skill:{min(ids)}"
        else:
            skill_canon = f"Skill:?:{chunk.lower()}"
        # Use -1 as level_id sort key when None (so None sorts last)
        level_sort = level_id if level_id is not None else -1
        parts.append((level_sort, level_id, skill_canon))
    # Sort by (level_sort, skill_canon) — level_sort handles None case
    parts.sort(key=lambda p: (p[0], p[2]))
    return "|".join(f"L{lvl}:{sc}" if lvl is not None else sc for _, lvl, sc in parts)


def _normalize_primary_skill(value: str) -> str:
    """Normalize "K1, A2, ..." → sorted tuple of (skill_code, value)."""
    # PRT primary skill codes: A=Attack, D=Defense, P=Power, K=Knowledge
    # Same across all languages.
    parts = []
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        # Format: "<letter><number>"
        m = re.match(r"^([AaDdPpKk])(\d+)$", chunk)
        if m:
            code = m.group(1).upper()
            val = m.group(2)
            parts.append(f"{code}:{val}")
        else:
            parts.append(chunk.lower())
    parts.sort()
    return "|".join(parts)


def _normalize_resource_compound(value: str) -> str:
    """Normalize "W5, G3, ..." → sorted tuple of (resource_letter, amount)."""
    parts = []
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        # Format: "<letter><number>" or "<letter><sign><number>" (e.g. "G-7")
        m = re.match(r"^([WMOSCGwmoscg])(-?\d+)$", chunk)
        if m:
            letter = m.group(1).upper()
            amount = m.group(2)
            parts.append(f"{letter}:{amount}")
        else:
            parts.append(chunk.lower())
    parts.sort()
    return "|".join(parts)


def _normalize_reward_compound(value: str) -> str:
    """Normalize "Награда X" → canonical.

    PRT format: "<reward_type_name> <amount>" or just "<reward_type_name>".
    Reverse-lookup the reward type name in Reward section → ID.
    """
    # Try to extract reward type name (first word(s)) and amount
    value = value.strip()
    if not value:
        return ""
    # Common patterns: "Опыт 500", "Навык 6 Attack +6", "Артефакт 'Name'", etc.
    # For simplicity: just lowercased string. Reverse-lookup is complex due to
    # varying formats.
    return value.lower()


def _normalize_cell(value: Any, sheet_name: str, column_name: Optional[str]) -> Any:
    """Normalize a cell value based on its column's localization section.

    - None / empty string → None
    - "0" string → 0 int
    - String with known column section:
        * Direct section (Color, Town, etc.): reverse-lookup → "Section:ID"
        * Compound section (_guard_monsters, _spell_list, etc.): parse →
          sorted tuple of canonical strings
    - String with unknown section: lowercase (case-insensitive plain compare)
    - int/float → as-is
    """
    if value is None:
        return None
    if isinstance(value, str):
        s = value.strip()
        if s == '' or s == '0':
            return None if s == '' else 0
        if column_name is None:
            return s.lower()
        section = _get_column_section(sheet_name, column_name)
        if section is None:
            # Unknown column — plain lowercased string
            return s.lower()
        if section == "_plain_string":
            return s.lower()
        if section == "_guard_monsters":
            return _normalize_guard_monsters(s)
        if section == "_monster_with_count":
            # Heuristic: if contains digits → guard format, else single name
            if re.search(r"\d", s):
                return _normalize_guard_monsters(s)
            # Single monster name — direct lookup
            return _normalize_direct(s, "Monstr")
        if section == "_art_list":
            # If contains comma → list, else single
            if "," in s:
                return _normalize_list_compound(s, "Art")
            return _normalize_direct(s, "Art")
        if section == "_spell_list":
            if "," in s:
                return _normalize_list_compound(s, "Spell")
            return _normalize_direct(s, "Spell")
        if section == "_color_list":
            if "," in s:
                return _normalize_list_compound(s, "Color")
            return _normalize_direct(s, "Color")
        if section == "_skill_with_level":
            return _normalize_skill_with_level(s)
        if section == "_primary_skill":
            return _normalize_primary_skill(s)
        if section == "_resource_compound":
            return _normalize_resource_compound(s)
        if section == "_reward_compound":
            return _normalize_reward_compound(s)
        # Direct section (Color, Town, Art, Monstr, Spell, Skill, etc.)
        return _normalize_direct(s, section)
    # Numbers (int, float)
    return value


def _row_signature(row: List[Any], sheet_name: str, headers: List[str]) -> Tuple:
    """Build a hashable signature tuple for a row, normalizing each cell
    based on its column's localization section.
    """
    sig_parts = []
    for i, v in enumerate(row):
        col_name = headers[i] if i < len(headers) else None
        sig_parts.append(_normalize_cell(v, sheet_name, col_name))
    return tuple(sig_parts)


# ============================================================================
# Compare logic (multiset-based, Counter of row signatures)
# ============================================================================

def compare_excel_files(file_a: str, file_b: str, output_path: str,
                        ignore_localization: bool = True) -> str:
    """Compare two .xlsx files and save differences to output_path.

    When headers differ between A and B for a sheet:
      - Comparison is done using ONLY common columns (intersection of headers)
      - Header mismatch info is written at the top of the output sheet
      - _Сводка sheet lists which columns are A-only / B-only per sheet

    Returns the output_path on success.
    """
    wb_a = openpyxl.load_workbook(file_a, data_only=True)
    wb_b = openpyxl.load_workbook(file_b, data_only=True)
    wb_out = Workbook()
    if "Sheet" in wb_out.sheetnames:
        del wb_out["Sheet"]

    common_sheets = sorted(set(wb_a.sheetnames) & set(wb_b.sheetnames))
    only_a = sorted(set(wb_a.sheetnames) - set(wb_b.sheetnames))
    only_b = sorted(set(wb_b.sheetnames) - set(wb_a.sheetnames))

    fill_a = PatternFill(start_color="FFF2CC", end_color="FFF2CC", fill_type="solid")
    fill_b = PatternFill(start_color="D9EAD3", end_color="D9EAD3", fill_type="solid")
    fill_diff = PatternFill(start_color="F4CCCC", end_color="F4CCCC", fill_type="solid")
    fill_info = PatternFill(start_color="D9E1F3", end_color="D9E1F3", fill_type="solid")  # blue-ish for info rows
    bold_font = Font(bold=True)

    # summary_rows: (sheet_name, a_total, b_total, n_diffs, n_common, a_only_cols, b_only_cols)
    summary_rows: List[Tuple] = []

    for sheet_name in common_sheets:
        ws_a = wb_a[sheet_name]
        ws_b = wb_b[sheet_name]

        headers_a = [ws_a.cell(row=1, column=c).value for c in range(1, ws_a.max_column + 1)]
        headers_b = [ws_b.cell(row=1, column=c).value for c in range(1, ws_b.max_column + 1)]

        headers_match = (headers_a == headers_b)

        if headers_match:
            # Headers identical — full comparison
            headers = headers_a
            rows_a = _extract_rows(ws_a)
            rows_b = _extract_rows(ws_b)
            a_only_cols = []
            b_only_cols = []
        else:
            # Headers differ — compare using ONLY common columns
            # Find common headers (preserving A's order for consistency)
            common_headers = [h for h in headers_a if h in headers_b]
            a_only_cols = [h for h in headers_a if h not in headers_b]
            b_only_cols = [h for h in headers_b if h not in headers_a]

            # Extract column indices for common headers
            a_indices = [headers_a.index(h) for h in common_headers]
            b_indices = [headers_b.index(h) for h in common_headers]

            # Extract rows using only common columns
            raw_a = _extract_rows(ws_a)
            raw_b = _extract_rows(ws_b)
            rows_a = [[row[i] if i < len(row) else None for i in a_indices] for row in raw_a]
            rows_b = [[row[i] if i < len(row) else None for i in b_indices] for row in raw_b]
            headers = common_headers

        # Build signatures using column-aware normalization
        sigs_a = Counter(_row_signature(r, sheet_name, headers) for r in rows_a)
        sigs_b = Counter(_row_signature(r, sheet_name, headers) for r in rows_b)

        diffs = _compare_signature(sigs_a, sigs_b, rows_a, rows_b, headers, sheet_name)

        common_sigs = sigs_a & sigs_b
        n_common = sum(common_sigs.values())

        summary_rows.append((sheet_name, len(rows_a), len(rows_b), len(diffs), n_common,
                             a_only_cols, b_only_cols))

        if not diffs and headers_match:
            continue  # no diffs and headers match → skip sheet entirely

        # Create output sheet
        ws_out = wb_out.create_sheet(sheet_name)

        start_row = 1
        if not headers_match:
            # Write header mismatch info at the top
            ws_out.append([f"Колонки только в A: {', '.join(a_only_cols)}" if a_only_cols
                           else "Колонок только в A нет"])
            ws_out.cell(row=1, column=1).fill = fill_info
            ws_out.cell(row=1, column=1).font = bold_font
            ws_out.append([f"Колонки только в B: {', '.join(b_only_cols)}" if b_only_cols
                           else "Колонок только в B нет"])
            ws_out.cell(row=2, column=1).fill = fill_info
            ws_out.cell(row=2, column=1).font = bold_font
            ws_out.append([f"Сравнение по общим колонкам ({len(headers)}): {', '.join(headers)}"])
            ws_out.cell(row=3, column=1).fill = fill_info
            ws_out.cell(row=3, column=1).font = bold_font
            ws_out.append([])  # empty separator row
            start_row = 5  # data headers start at row 5

        # Write data headers + Source
        out_headers = list(headers) + ["Source"]
        ws_out.append(out_headers)
        header_row = ws_out.max_row
        for col_idx in range(1, len(out_headers) + 1):
            cell = ws_out.cell(row=header_row, column=col_idx)
            cell.font = bold_font
            cell.alignment = Alignment(horizontal="center", wrap_text=True)

        for diff in diffs:
            source = diff["source"]
            row_data = diff["row"]
            ws_out.append(list(row_data) + [source])
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

        # Auto-filter on the data header row (not the info rows)
        if not headers_match:
            ws_out.auto_filter.ref = f"A{header_row}:{openpyxl.utils.get_column_letter(len(out_headers))}{ws_out.max_row}"
        else:
            ws_out.auto_filter.ref = ws_out.dimensions
        # Freeze panes at the data header row
        ws_out.freeze_panes = ws_out.cell(row=header_row + 1, column=1).coordinate

        _autosize(ws_out)

    # ── _Сводка (summary) sheet ──
    if wb_out.sheetnames:
        ws_sum = wb_out.create_sheet("_Сводка", 0)
        # File paths
        ws_sum.append(["File A:", file_a])
        ws_sum.cell(row=1, column=1).font = bold_font
        ws_sum.append(["File B:", file_b])
        ws_sum.cell(row=2, column=1).font = bold_font
        ws_sum.append([])  # empty separator

        # Summary table header
        sum_headers = ["Лист", "A: всего", "B: всего", "Diff", "Совп.",
                        "Колонок только в A", "Колонок только в B"]
        ws_sum.append(sum_headers)
        hdr_row = ws_sum.max_row
        for col_idx in range(1, len(sum_headers) + 1):
            ws_sum.cell(row=hdr_row, column=col_idx).font = bold_font
            ws_sum.cell(row=hdr_row, column=col_idx).alignment = Alignment(horizontal="center")

        for name, na, nb, nd, nc, a_cols, b_cols in summary_rows:
            a_cols_str = ", ".join(a_cols) if a_cols else ""
            b_cols_str = ", ".join(b_cols) if b_cols else ""
            ws_sum.append([name, na, nb, nd, nc, a_cols_str, b_cols_str])

        if only_a or only_b:
            ws_sum.append([])
            ws_sum.append(["Только в A:"] + only_a)
            ws_sum.append(["Только в B:"] + only_b)

        _autosize(ws_sum)
        ws_sum.freeze_panes = f"A{hdr_row + 1}"
        ws_sum.auto_filter.ref = f"A{hdr_row}:{openpyxl.utils.get_column_letter(len(sum_headers))}{ws_sum.max_row}"

    if not wb_out.sheetnames:
        ws = wb_out.create_sheet("Summary")
        ws.append(["Result", "No differences found — files are identical."])

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


def _compare_signature(sigs_a: Counter, sigs_b: Counter,
                       rows_a: List[List], rows_b: List[List],
                       headers: List[str], sheet_name: str) -> List[Dict]:
    """Compare two row sets via signature-based multiset comparison.

    Returns list of diffs (A-only / B-only / A≠B with multiplicity mismatch).
    Preserves original row content for display (looks up by signature).
    """
    # Build signature → original row map (first occurrence wins for display)
    row_for_sig: Dict[Tuple, List[Any]] = {}
    for r in rows_a:
        sig = _row_signature(r, sheet_name, headers)
        if sig not in row_for_sig:
            row_for_sig[sig] = r
    for r in rows_b:
        sig = _row_signature(r, sheet_name, headers)
        if sig not in row_for_sig:
            row_for_sig[sig] = r

    only_a = sigs_a - sigs_b  # multiplicity diff
    only_b = sigs_b - sigs_a
    common_keys = set(sigs_a) & set(sigs_b)

    diffs: List[Dict] = []

    def _sort_key(sig):
        return tuple((str(x) if x is not None else "") for x in sig)

    for sig in sorted(only_a.keys(), key=_sort_key):
        for _ in range(only_a[sig]):
            diffs.append({"source": "A-only", "row": list(row_for_sig[sig])})

    for sig in sorted(only_b.keys(), key=_sort_key):
        for _ in range(only_b[sig]):
            diffs.append({"source": "B-only", "row": list(row_for_sig[sig])})

    for sig in sorted(common_keys, key=_sort_key):
        ca = sigs_a[sig]
        cb = sigs_b[sig]
        if ca != cb:
            diffs.append({
                "source": f"A≠B (rows: A={ca}, B={cb})",
                "row": list(row_for_sig[sig]),
            })

    return diffs


def _write_sheet_mismatch(wb_out, sheet_name, headers_a, headers_b, ws_a, ws_b):
    """Write a summary sheet when headers differ."""
    ws = wb_out.create_sheet(sheet_name[:28])
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
        print("Usage: python excel_compare.py <file_a.xlsx> <file_b.xlsx> <output.xlsx>")
        sys.exit(1)
    out = compare_excel_files(sys.argv[1], sys.argv[2], sys.argv[3])
    print(f"Comparison saved to: {out}")
