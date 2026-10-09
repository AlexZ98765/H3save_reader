# pip install PySide6

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
gm1_diff_gui.py — GUI (PySide6) для дифференциального компаратора .GM1
=====================================================================

Графическая обёртка над логикой gm1_diff.py. Вся логика парсинга,
диффа, аннотаций и вспомогательных команд сохранена без изменений.

Запуск:
    python3 gm1_diff_gui.py
"""

import sys
import io
import re
import json
import gzip
import struct
from typing import List, Tuple, Optional, Dict, Any

from PySide6.QtCore import Qt
from PySide6.QtGui import QFont, QAction
from PySide6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QPushButton, QLineEdit, QLabel, QPlainTextEdit, QComboBox,
    QSpinBox, QCheckBox, QFileDialog, QGroupBox, QFormLayout,
    QMessageBox, QSplitter, QTabWidget,
)

# ============================================================================
# ЛОГИКА (идентична оригинальному gm1_diff.py, кроме ANSI/цветов)
# ============================================================================

MAGIC_SCENARIO = b"H3SVG"   # одиночный сценарий
MAGIC_CAMPAIGN = b"H3SVC"   # кампания

# Карта значений version_major (uint32 LE по смещению 8) -> человекочитаемое имя.
# Источники: h3sed metadata.py, документация сообщества.
VERSION_NAMES = {
    0x0E: "RoE 1.0",       # Restoration of Erathia
    0x0F: "RoE",
    0x1E: "AB 1.0",        # Armageddon's Blade (30)
    0x1F: "AB",
    0x20: "SoD 4.0",       # Shadow of Death (32) — основной целевой формат
    0x21: "SoD 4.x",
    0x2A: "HotA",          # Horn of the Abyss (42)
    0x2B: "HotA",          # (43)
}


def load_save(path: str) -> Tuple[bytes, Dict[str, Any]]:
    """
    Читает файл .GM1/.CGM, разжимает gzip (если нужно), возвращает
    кортеж (raw_bytes, info_dict).

    info_dict содержит поля:
      compressed_size, raw_size, magic, type,
      version_major, version_minor, version_name, map_name
    """
    with open(path, "rb") as f:
        data = f.read()
    compressed_size = len(data)

    # H3-сохранения обычно gzip-сжаты (первые 2 байта 0x1f 0x8b).
    # Если это не gzip — считаем, что файл уже распакован (HotA иногда
    # хранит .gm1 без сжатия).
    if data[:2] == b"\x1f\x8b":
        # HoMM3 сейвы часто имеют "битый" gzip CRC (игра пишет trailer
        # неправильно). Стандартный gzip.decompress падает с
        # "CRC check failed". Патчим _GzipReader._read_eof, чтобы
        # потреблять trailing-байты без проверки (как делает h3sed).
        import contextlib

        @contextlib.contextmanager
        def _patch_gzip_for_partial():
            reader_cls = getattr(gzip, "_GzipReader", None)
            if reader_cls is not None and hasattr(reader_cls, "_read_eof"):
                orig_read_eof = reader_cls._read_eof

                def patched_read_eof(self):
                    # Потребляем 8 байт trailer (CRC + ISIZE), но не проверяем.
                    try:
                        self._fp.read(8)
                    except Exception:
                        pass

                reader_cls._read_eof = patched_read_eof
                try:
                    yield
                finally:
                    reader_cls._read_eof = orig_read_eof
            else:
                # В старых Python без _GzipReader — просто yield.
                yield

        try:
            with _patch_gzip_for_partial():
                with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
                    raw = gf.read()
        except Exception:
            # Запасной вариант: raw DEFLATE (zlib wbits=-15),
            # пропуская 10-байтный gzip-заголовок.
            try:
                import zlib
                raw = zlib.decompress(data[10:], -15)
            except Exception:
                raw = data
    else:
        raw = data

    info = {"compressed_size": compressed_size, "raw_size": len(raw)}

    # Магика: первые 4 байта "H3SV", 5-й байт 'G' (сценарий) или 'C' (кампания).
    # Таким образом полная магика — 5 байт: "H3SVG" или "H3SVC".
    if raw[:4] == b"H3SV":
        fifth = raw[4:5]
        if fifth == b"G":
            info["magic"] = "H3SVG"
            info["type"] = "scenario"
        elif fifth == b"C":
            info["magic"] = "H3SVC"
            info["type"] = "campaign"
        else:
            ch = fifth.decode("ascii", errors="replace") if fifth else "?"
            info["magic"] = f"H3SV{ch}"
            info["type"] = "unknown"
    else:
        info["magic"] = raw[:4].hex()
        info["type"] = "unknown"

    # Версия: смещения 8 (major) и 12 (minor) как uint32 LE
    if len(raw) >= 16:
        info["version_major"] = int.from_bytes(raw[8:12], "little")
        info["version_minor"] = int.from_bytes(raw[12:16], "little")
        info["version_name"] = VERSION_NAMES.get(
            info["version_major"],
            "unknown (0x%X)" % info["version_major"])
    else:
        info["version_major"] = info["version_minor"] = 0
        info["version_name"] = "unknown"

    # Имя карты: пытаемся найти длину-префиксную строку после заголовка.
    # Это эвристика — точная структура .gm1 заголовка сложнее.
    info["map_name"] = _guess_map_name(raw)
    return raw, info


def _guess_map_name(raw: bytes, max_search: int = 4096) -> str:
    """
    Эвристический поиск имени карты в начале сохранения.
    Стратегии:
      1) Длина-префиксная строка по смещению 16, 20 или 24
         (uint32 LE длина, затем байты в cp1251/latin1).
      2) Первый длинный (>= 4) printable ASCII отрезок в первых 4 КБ.
    Возвращаем "" если ничего не нашли.
    """
    # Стратегия 1: пробуем несколько стартовых смещений
    for start in (16, 20, 24, 28):
        if start + 4 > len(raw):
            continue
        n = int.from_bytes(raw[start:start + 4], "little")
        if 1 <= n <= 256:
            s = raw[start + 4 : start + 4 + n]
            try:
                txt = s.decode("cp1251")
            except Exception:
                try:
                    txt = s.decode("latin1")
                except Exception:
                    continue
            # Фильтруем: должны быть только печатные ASCII
            clean = "".join(c if 32 <= ord(c) < 127 else "" for c in txt).strip()
            if len(clean) >= 1:
                return clean

    # Стратегия 2: первый длинный printable отрезок после заголовка
    m = re.search(rb"[\x20-\x7e]{4,}", raw[16:max_search])
    if m:
        return m.group(0).decode("ascii", errors="ignore")
    return ""


# ============================================================================
# Байтовый diff: поиск изменённых диапазонов
# ============================================================================

def find_changed_ranges(a: bytes, b: bytes, gap: int = 4) -> List[Tuple[int, int]]:
    """
    Сравнивает два байтовых массива, возвращает список (start, end)
    полуоткрытых диапазонов изменённых байт.

    Подряд идущие изменённые байты объединяются в один диапазон.
    Разрывы из <= gap неизменённых байт между двумя изменениями
    также включаются в тот же диапазон (это помогает группировать
    связанные поля, расположенные близко друг к другу).

    Если один файл длиннее другого — хвост считается одним изменением.
    """
    n = min(len(a), len(b))
    changed_idx = [i for i in range(n) if a[i] != b[i]]

    tail: Optional[Tuple[int, int]] = None
    if len(a) != len(b):
        tail = (n, max(len(a), len(b)))

    if not changed_idx and tail is None:
        return []

    ranges: List[Tuple[int, int]] = []
    if changed_idx:
        start = prev = changed_idx[0]
        for i in changed_idx[1:]:
            # i-prev это разница индексов. Если i-prev <= gap+1, между ними
            # не более gap неизменённых байт — склеиваем.
            if i - prev <= gap + 1:
                prev = i
            else:
                ranges.append((start, prev + 1))
                start = prev = i
        ranges.append((start, prev + 1))

    # Склеиваем хвостовой диапазон с последним, если он близко
    if tail is not None:
        if ranges and tail[0] - ranges[-1][1] <= gap:
            ranges[-1] = (ranges[-1][0], tail[1])
        else:
            ranges.append(tail)
    return ranges


# ============================================================================
# Форматирование вывода (без ANSI-цветов — они бесполезны в QPlainTextEdit)
# ============================================================================

def hex_dump_line(buf: bytes) -> str:
    hexs = " ".join(f"{b:02x}" for b in buf)
    asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in buf)
    return f"{hexs}  |{asciis}|"


def format_range(a: bytes, b: bytes, start: int, end: int,
                 max_show: int = 64) -> str:
    """
    Форматирует один изменённый диапазон для вывода.
    Если диапазон длиннее max_show байт — показываем только первый
    max_show кусок с пометкой (... N more bytes).
    """
    size = end - start
    a_part = a[start:end]
    b_part = b[start:end]

    truncated = size > max_show
    if truncated:
        a_part = a_part[:max_show]
        b_part = b_part[:max_show]

    line_a = "- " + hex_dump_line(a_part)
    line_b = "+ " + hex_dump_line(b_part)
    out = f"  [0x{start:08x} .. 0x{end:08x}] ({size}b)\n{line_a}\n{line_b}"
    if truncated:
        out += f"\n  ... {size - max_show} more bytes"
    return out


# ============================================================================
# Заголовок метаданных
# ============================================================================

def format_metadata_header(path_a: Optional[str], path_b: Optional[str],
                           a_info: Dict, b_info: Optional[Dict]) -> str:
    """Печатает стандартный заголовок с информацией о файлах."""
    lines = ["=== GM1 DIFF ==="]
    if path_b and b_info is not None:
        lines.append(f"File A: {path_a}  ({a_info['compressed_size']} bytes compressed -> "
                     f"{a_info['raw_size']} bytes raw, {a_info['magic']}, {a_info['version_name']})")
        lines.append(f"File B: {path_b}  ({b_info['compressed_size']} bytes compressed -> "
                     f"{b_info['raw_size']} bytes raw, {b_info['magic']}, {b_info['version_name']})")
        # Имя карты: если совпадают — одна строка, иначе — две
        if a_info.get("map_name") and a_info["map_name"] == b_info.get("map_name"):
            lines.append(f'Map name: "{a_info["map_name"]}"')
        else:
            if a_info.get("map_name"):
                lines.append(f'Map name A: "{a_info["map_name"]}"')
            if b_info.get("map_name"):
                lines.append(f'Map name B: "{b_info["map_name"]}"')
    else:
        lines.append(f"File: {path_a}  ({a_info['compressed_size']} bytes compressed -> "
                     f"{a_info['raw_size']} bytes raw, {a_info['magic']}, {a_info['version_name']})")
        if a_info.get("map_name"):
            lines.append(f'Map name: "{a_info["map_name"]}"')
    lines.append("")
    return "\n".join(lines)


# ============================================================================
# Режим A: байтовый diff (по умолчанию)
# ============================================================================

def cmd_byte_diff(a_raw, b_raw, a_info, b_info, path_a, path_b, gap, limit) -> str:
    out = [format_metadata_header(path_a, path_b, a_info, b_info)]

    ranges = find_changed_ranges(a_raw, b_raw, gap=gap)
    total_changed = sum(e - s for s, e in ranges)
    out.append(f"Total changed bytes: {total_changed} in {len(ranges)} ranges\n")

    if not ranges:
        out.append("Files are identical.")
        return "\n".join(out)

    for s, e in ranges[:limit]:
        out.append(format_range(a_raw, b_raw, s, e))
        out.append("")
    if len(ranges) > limit:
        out.append(f"... and {len(ranges) - limit} more ranges (use limit to see more)")
    return "\n".join(out)


# ============================================================================
# Режим B: section map — гистограмма 1КБ-блоков
# ============================================================================

def cmd_section_map(a_raw, b_raw, a_info, b_info, path_a, path_b) -> str:
    out = [format_metadata_header(path_a, path_b, a_info, b_info)]

    block = 1024
    n = max(len(a_raw), len(b_raw))
    n_blocks = (n + block - 1) // block
    out.append(f"Section map (block size = {block} bytes, total {n} bytes / {n_blocks} blocks):")
    out.append("")

    # Для каждого блока определяем метку: = * ?
    marks: List[str] = []
    for i in range(0, n, block):
        a_chunk = a_raw[i:i + block]
        b_chunk = b_raw[i:i + block]
        if len(a_chunk) != len(b_chunk):
            marks.append("?")
        elif a_chunk == b_chunk:
            marks.append("=")
        else:
            marks.append("*")

    per_line = 64
    for i in range(0, len(marks), per_line):
        offset = i * block
        chunk = "".join(marks[i:i + per_line])
        out.append(f"0x{offset:08x}  {chunk}")

    out.append("")
    out.append("Legend: = unchanged   * changed   ? one side longer")

    # Топ-20 самых длинных стабильных отрезков (вероятно, структура/паддинг).
    out.append("")
    out.append("Top-20 longest unchanged runs (likely stable sections / padding):")
    runs: List[Tuple[int, int]] = []
    i = 0
    while i < len(marks):
        if marks[i] == "=":
            j = i
            while j < len(marks) and marks[j] == "=":
                j += 1
            runs.append((i * block, (j - i) * block))
            i = j
        else:
            i += 1
    runs.sort(key=lambda x: -x[1])
    for s, l in runs[:20]:
        e = s + l
        out.append(f"  0x{s:08x} .. 0x{e:08x}  ({l} bytes)")
    return "\n".join(out)


# ============================================================================
# Режим C: annotated diff
# ============================================================================

def load_annotations(path: str) -> List[Dict]:
    """
    Грузит JSON-аннотации известных полей. Возвращает плоский список
    {name, offset, size, type}.

    Поддерживаемые форматы (можно комбинировать в одном JSON):

    1) Плоский список:
       {"fields": [{"name":..., "offset":..., "size":..., "type":...}, ...]}

    2) Список секций с относительными полями и опциональным repeat/stride
       для массивов однотипных записей (герои, города):
       {"sections": [
           {"name": "hero", "start": 1000, "repeat": 8, "stride": 500,
            "fields": {"attack": {"offset": 238, "size": 1, "type": "u8"}}}
       ]}

    3) Словарь секций (для совместимости с выводом Task 1):
       {"hero_block": {"start": 1000, "fields": {...}}}
       В этом случае start обязателен; repeat/stride опциональны.
    """
    with open(path, "r", encoding="utf-8") as f:
        data = json.load(f)

    fields: List[Dict] = []

    # 1) Плоский список
    if "fields" in data and isinstance(data["fields"], list):
        for f in data["fields"]:
            fields.append({
                "name":  f["name"],
                "offset": int(f["offset"]),
                "size":  int(f.get("size", 1)),
                "type":  f.get("type", "u8"),
            })

    # 2) Список секций
    if "sections" in data and isinstance(data["sections"], list):
        for sec in data["sections"]:
            _emit_section(fields, sec)

    # 3) Словарь секций (имя секции -> её описание)
    for key, val in data.items():
        if key in ("fields", "sections"):
            continue
        if isinstance(val, dict) and ("fields" in val or "start" in val):
            sec = dict(val)
            sec.setdefault("name", key)
            _emit_section(fields, sec)

    fields.sort(key=lambda f: f["offset"])
    return fields


def _emit_section(out: List[Dict], sec: Dict):
    """Раскрывает одну секцию (с repeat/stride) в плоский список полей."""
    base = int(sec.get("start", 0))
    name = sec.get("name", "section")
    repeat = int(sec.get("repeat", 1))
    stride = int(sec.get("stride", 0))
    sec_fields = sec.get("fields", {})
    # sec_fields может быть dict {name: {offset, size, type}}
    # или list [{name, offset, size, type}]
    if isinstance(sec_fields, dict):
        items = [(k, v) for k, v in sec_fields.items()]
    elif isinstance(sec_fields, list):
        items = [(f.get("name", "?"), f) for f in sec_fields]
    else:
        items = []

    for r in range(repeat):
        off_base = base + r * stride
        for fname, finfo in items:
            f = {
                "name":   f"{name}[{r}].{fname}" if repeat > 1 else f"{name}.{fname}",
                "offset": off_base + int(finfo["offset"]),
                "size":   int(finfo.get("size", 1)),
                "type":   finfo.get("type", "u8"),
            }
            out.append(f)


def build_field_index(fields: List[Dict]) -> Dict[int, Dict]:
    """
    Строит словарь byte_offset -> field для O(1) поиска.
    Если поля перекрываются — побеждает последнее добавленное.
    """
    idx: Dict[int, Dict] = {}
    for f in fields:
        for o in range(f["offset"], f["offset"] + f["size"]):
            idx[o] = f
    return idx


def format_field_value(raw: bytes, field: Dict) -> str:
    """Декодирует значение поля по типу."""
    start = field["offset"]
    end = min(start + field["size"], len(raw))
    if start >= len(raw):
        return "<out of range>"
    chunk = raw[start:end]
    t = field["type"]
    try:
        if t in ("u8", "u16", "u32", "u64"):
            return str(int.from_bytes(chunk, "little"))
        if t in ("i8", "i16", "i32", "i64"):
            return str(int.from_bytes(chunk, "little", signed=True))
        if t == "f32":
            return repr(struct.unpack("<f", chunk[:4])[0])
        if t == "f64":
            return repr(struct.unpack("<d", chunk[:8])[0])
        if t in ("str", "string"):
            return chunk.rstrip(b"\x00\xff").decode("cp1251", errors="replace")
    except Exception as e:
        return f"<decode error: {e}>"
    return chunk.hex()


def cmd_annotated(a_raw, b_raw, a_info, b_info, path_a, path_b,
                  annotations_path, gap, limit) -> str:
    out = [format_metadata_header(path_a, path_b, a_info, b_info)]
    fields = load_annotations(annotations_path)
    out.append(f"Loaded {len(fields)} annotated fields from {annotations_path}\n")

    idx = build_field_index(fields)

    ranges = find_changed_ranges(a_raw, b_raw, gap=gap)
    total_changed = sum(e - s for s, e in ranges)
    out.append(f"Total changed bytes: {total_changed} in {len(ranges)} ranges\n")

    unknown_count = 0
    known_count = 0
    printed_ranges = 0
    last_field_printed: Optional[int] = None  # offset последнего показанного поля

    for s, e in ranges:
        if printed_ranges >= limit:
            out.append(f"\n... output truncated at {limit} ranges "
                       f"({len(ranges) - printed_ranges} more)")
            break
        # Идём побайтово внутри диапазона, группируя по полям
        i = s
        while i < e:
            field = idx.get(i)
            if field is None:
                out.append(f"  [0x{i:08x}] UNKNOWN_SECTION: {a_raw[i]:02x} -> {b_raw[i]:02x}")
                unknown_count += 1
                i += 1
                last_field_printed = None
            else:
                # Поле; продвигаемся до конца поля (но не дальше e, чтобы
                # не вылезти за пределы текущего range)
                full_end = field["offset"] + field["size"]
                # Печатаем только если ещё не показывали это конкретное поле
                # (по началу offset) — иначе повтор для multi-byte range.
                if last_field_printed != field["offset"]:
                    oldv = format_field_value(a_raw, field)
                    newv = format_field_value(b_raw, field)
                    out.append(f"  [0x{field['offset']:08x}] {field['name']}: {oldv} -> {newv}")
                    known_count += 1
                    last_field_printed = field["offset"]
                # Если поле уходит за пределы текущего range — пропускаем весь остаток
                i = max(full_end, i + 1)
                if full_end > e:
                    # Поле выходит за диапазон; прерываем этот range
                    break
        printed_ranges += 1

    out.append("")
    out.append(f"Summary: {known_count} attributed changes, "
               f"{unknown_count} in unknown regions")
    return "\n".join(out)


# ============================================================================
# Вспомогательные команды (для одного файла)
# ============================================================================

def cmd_info(path) -> str:
    raw, info = load_save(path)
    lines = [format_metadata_header(path, None, info, None)]
    lines.append(f"Magic:           {info['magic']} ({info['type']})")
    lines.append(f"Version major:   {info['version_major']} (0x{info['version_major']:X})")
    lines.append(f"Version minor:   {info['version_minor']} (0x{info['version_minor']:X})")
    lines.append(f"Version name:    {info['version_name']}")
    lines.append(f"Compressed size: {info['compressed_size']}")
    lines.append(f"Raw size:        {info['raw_size']}")
    lines.append(f"Map name:        {info['map_name']!r}")
    return "\n".join(lines)


def cmd_strings(path, min_len) -> str:
    raw, info = load_save(path)
    out = [f"=== Strings in {path} (min length {min_len}) ==="]
    pattern = re.compile(rb"[\x20-\x7e]{%d,}" % min_len)
    count = 0
    for m in pattern.finditer(raw):
        s = m.group(0).decode("ascii", errors="ignore")
        # Обрежем длинные строки до 200 символов
        disp = s if len(s) <= 200 else s[:200] + "..."
        out.append(f"0x{m.start():08x}  {disp}")
        count += 1
    out.append(f"\nTotal: {count} strings")
    return "\n".join(out)


def cmd_hexdump(path, start, end) -> str:
    raw, info = load_save(path)
    if start < 0:
        start = 0
    if end > len(raw):
        end = len(raw)
    out = [f"=== Hex dump {path} [0x{start:x} .. 0x{end:x}] ==="]
    for i in range(start, end, 16):
        chunk = raw[i:i + 16]
        hexs = " ".join(f"{b:02x}" for b in chunk)
        hexs = hexs.ljust(16 * 3 - 1)
        asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in chunk)
        out.append(f"{i:08x}  {hexs}  |{asciis}|")
    return "\n".join(out)


def cmd_find(path, pat) -> str:
    raw, info = load_save(path)
    # Интерпретация: hex если длина чётная и все символы hex/пробелы, иначе строка
    cleaned = pat.replace(" ", "")
    is_hex = (len(cleaned) >= 2 and len(cleaned) % 2 == 0
              and all(c in "0123456789abcdefABCDEF" for c in cleaned))
    out = []
    if is_hex:
        needle = bytes.fromhex(cleaned)
        out.append(f"Searching for hex pattern: {needle.hex()} ({len(needle)} bytes)")
    else:
        needle = pat.encode("utf-8")
        out.append(f"Searching for string: {pat!r} ({len(needle)} bytes)")
    out.append(f"In file: {path}  (raw size {len(raw)})")
    out.append("")
    count = 0
    i = raw.find(needle)
    while i != -1:
        ctx_start = max(0, i - 8)
        ctx_end = min(len(raw), i + len(needle) + 8)
        ctx = raw[ctx_start:ctx_end]
        hexs = " ".join(f"{b:02x}" for b in ctx)
        asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in ctx)
        out.append(f"0x{i:08x}  {hexs}  |{asciis}|")
        count += 1
        i = raw.find(needle, i + 1)
    out.append(f"\nTotal: {count} matches")
    return "\n".join(out)


# ============================================================================
# GUI
# ============================================================================

class Gm1DiffWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("GM1 Diff — Heroes III save comparator (PySide6)")
        self.resize(1100, 750)

        # --- Центральный виджет ---
        central = QWidget()
        self.setCentralWidget(central)
        root = QVBoxLayout(central)

        # ============ Блок выбора файлов ============
        files_group = QGroupBox("Files")
        files_layout = QFormLayout(files_group)

        # File A
        row_a = QHBoxLayout()
        self.edit_a = QLineEdit()
        self.edit_a.setPlaceholderText("Path to first .gm1 file…")
        btn_a = QPushButton("Browse…")
        btn_a.clicked.connect(lambda: self._pick_file(self.edit_a))
        row_a.addWidget(self.edit_a)
        row_a.addWidget(btn_a)
        files_layout.addRow("File A:", row_a)

        # File B
        row_b = QHBoxLayout()
        self.edit_b = QLineEdit()
        self.edit_b.setPlaceholderText("Path to second .gm1 file (optional for info/strings/…)")
        btn_b = QPushButton("Browse…")
        btn_b.clicked.connect(lambda: self._pick_file(self.edit_b))
        row_b.addWidget(self.edit_b)
        row_b.addWidget(btn_b)
        files_layout.addRow("File B:", row_b)

        # Annotations file
        row_ann = QHBoxLayout()
        self.edit_ann = QLineEdit()
        self.edit_ann.setPlaceholderText("Path to annotations .json (for --annotated)")
        btn_ann = QPushButton("Browse…")
        btn_ann.clicked.connect(self._pick_annotations)
        row_ann.addWidget(self.edit_ann)
        row_ann.addWidget(btn_ann)
        files_layout.addRow("Annotations:", row_ann)

        root.addWidget(files_group)

        # ============ Блок параметров ============
        opts_group = QGroupBox("Options")
        opts_layout = QHBoxLayout(opts_group)

        # Режим
        mode_layout = QFormLayout()
        self.combo_mode = QComboBox()
        self.combo_mode.addItems([
            "Byte diff (default)",
            "Section map",
            "Annotated diff",
            "Info (single file)",
            "Strings (single file)",
            "Hexdump (single file)",
            "Find pattern (single file)",
        ])
        self.combo_mode.currentIndexChanged.connect(self._on_mode_changed)
        mode_layout.addRow("Mode:", self.combo_mode)
        opts_layout.addLayout(mode_layout)

        # Gap
        gap_layout = QFormLayout()
        self.spin_gap = QSpinBox()
        self.spin_gap.setRange(0, 1_000_000)
        self.spin_gap.setValue(4)
        gap_layout.addRow("Gap (bytes):", self.spin_gap)
        opts_layout.addLayout(gap_layout)

        # Limit
        limit_layout = QFormLayout()
        self.spin_limit = QSpinBox()
        self.spin_limit.setRange(1, 1_000_000)
        self.spin_limit.setValue(200)
        limit_layout.addRow("Limit ranges:", self.spin_limit)
        opts_layout.addLayout(limit_layout)

        # Strings N
        str_layout = QFormLayout()
        self.spin_strlen = QSpinBox()
        self.spin_strlen.setRange(1, 4096)
        self.spin_strlen.setValue(6)
        str_layout.addRow("Strings min len:", self.spin_strlen)
        opts_layout.addLayout(str_layout)

        # Hexdump range
        hex_layout = QFormLayout()
        self.edit_hex_start = QLineEdit("0x0")
        self.edit_hex_end = QLineEdit("0x100")
        self.edit_hex_start.setMaximumWidth(100)
        self.edit_hex_end.setMaximumWidth(100)
        hex_row = QHBoxLayout()
        hex_row.addWidget(self.edit_hex_start)
        hex_row.addWidget(QLabel(".."))
        hex_row.addWidget(self.edit_hex_end)
        hex_layout.addRow("Hexdump:", hex_row)
        opts_layout.addLayout(hex_layout)

        # Find pattern
        find_layout = QFormLayout()
        self.edit_find = QLineEdit()
        self.edit_find.setPlaceholderText("hex or string")
        find_layout.addRow("Find:", self.edit_find)
        opts_layout.addLayout(find_layout)

        root.addWidget(opts_group)

        # ============ Кнопки ============
        btn_row = QHBoxLayout()
        self.btn_run = QPushButton("▶ Run")
        self.btn_run.clicked.connect(self.on_run)
        self.btn_run.setDefault(True)
        btn_clear = QPushButton("Clear output")
        btn_clear.clicked.connect(lambda: self.output.clear())
        btn_row.addWidget(self.btn_run)
        btn_row.addWidget(btn_clear)
        btn_row.addStretch(1)
        root.addLayout(btn_row)

        # ============ Вывод ============
        self.output = QPlainTextEdit()
        self.output.setReadOnly(True)
        self.output.setFont(QFont("Menlo, Consolas, monospace", 10))
        root.addWidget(self.output, 1)

        self._on_mode_changed(0)

    # ---------- вспомогательные ----------

    def _pick_file(self, target_edit: QLineEdit):
        path, _ = QFileDialog.getOpenFileName(
            self, "Select .gm1 file", "",
            "Heroes III saves (*.gm1 *.GM1 *.cgm *.CGM);;All files (*)")
        if path:
            target_edit.setText(path)

    def _pick_annotations(self):
        path, _ = QFileDialog.getOpenFileName(
            self, "Select annotations JSON", "",
            "JSON files (*.json);;All files (*)")
        if path:
            self.edit_ann.setText(path)

    def _on_mode_changed(self, idx: int):
        """Включаем/выключаем поля в зависимости от выбранного режима."""
        mode = self.combo_mode.currentText()
        is_single = mode.startswith(("Info", "Strings", "Hexdump", "Find"))
        # File B не нужен для одиночных режимов
        self.edit_b.setEnabled(not is_single)

        # annotations нужен только для annotated diff
        need_ann = mode.startswith("Annotated")
        self.edit_ann.setEnabled(need_ann)

        # strings N
        self.spin_strlen.setEnabled(mode.startswith("Strings"))
        # hexdump
        self.edit_hex_start.setEnabled(mode.startswith("Hexdump"))
        self.edit_hex_end.setEnabled(mode.startswith("Hexdump"))
        # find
        self.edit_find.setEnabled(mode.startswith("Find"))

    # ---------- запуск ----------

    def on_run(self):
        mode = self.combo_mode.currentText()
        path_a = self.edit_a.text().strip()
        path_b = self.edit_b.text().strip()

        try:
            if mode.startswith("Info"):
                if not path_a:
                    raise ValueError("Укажите File A")
                text = cmd_info(path_a)

            elif mode.startswith("Strings"):
                if not path_a:
                    raise ValueError("Укажите File A")
                text = cmd_strings(path_a, self.spin_strlen.value())

            elif mode.startswith("Hexdump"):
                if not path_a:
                    raise ValueError("Укажите File A")
                try:
                    start = int(self.edit_hex_start.text(), 0)
                    end = int(self.edit_hex_end.text(), 0)
                except ValueError:
                    raise ValueError("Hexdump: смещения должны быть целыми (0x… или decimal)")
                text = cmd_hexdump(path_a, start, end)

            elif mode.startswith("Find"):
                if not path_a:
                    raise ValueError("Укажите File A")
                pat = self.edit_find.text().strip()
                if not pat:
                    raise ValueError("Укажите паттерн для поиска")
                text = cmd_find(path_a, pat)

            else:
                # режимы для двух файлов
                if not path_a or not path_b:
                    raise ValueError("Укажите оба файла (File A и File B)")
                a_raw, a_info = load_save(path_a)
                b_raw, b_info = load_save(path_b)

                gap = self.spin_gap.value()
                limit = self.spin_limit.value()

                if mode.startswith("Section map"):
                    text = cmd_section_map(a_raw, b_raw, a_info, b_info,
                                           path_a, path_b)
                elif mode.startswith("Annotated"):
                    ann = self.edit_ann.text().strip()
                    if not ann:
                        raise ValueError("Для Annotated diff укажите JSON-файл аннотаций")
                    text = cmd_annotated(a_raw, b_raw, a_info, b_info,
                                         path_a, path_b, ann, gap, limit)
                else:  # Byte diff
                    text = cmd_byte_diff(a_raw, b_raw, a_info, b_info,
                                         path_a, path_b, gap, limit)

            self.output.setPlainText(text)

        except FileNotFoundError as e:
            QMessageBox.critical(self, "File not found", str(e))
        except Exception as e:
            QMessageBox.critical(self, "Error", f"{type(e).__name__}: {e}")


def main():
    app = QApplication(sys.argv)
    w = Gm1DiffWindow()
    w.show()
    sys.exit(app.exec())


if __name__ == "__main__":
    main()
