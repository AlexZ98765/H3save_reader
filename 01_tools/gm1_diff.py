#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
gm1_diff.py — Дифференциальный компаратор .GM1 сохранений Heroes III
====================================================================

Утилита для реверс-инжиниринга .GM1 файлов сохранений Heroes of Might
and Magic III: Shadow of Death (а также .CGM кампаний и HotA-сохранений).
Файлы .GM1 — это gzip-compressed бинарные данные, начинающиеся с магии
"H3SVG" (одиночный сценарий) или "H3SVC" (кампания).

Идея дифференциального подхода: сохраняем игру дважды с минимальным
различием (герой сдвинут на 1 тайл, +1 к атаке и т.д.), сравниваем байты.
Так локализуются "неизвестные" секции — fog of war, пути героев,
состояние объектов и т.п.

====================================================================
Режимы работы (флаги)
====================================================================
  (по умолчанию)        Байтовый diff: список изменённых диапазонов.
  --byte-diff           То же, что и режим по умолчанию.
  --section-map         Гистограмма стабильных/изменённых 1КБ-блоков.
  --annotated           Diff с аннотациями из --annotations FILE.
  --annotations FILE    JSON-файл с описанием известных полей.

Вспомогательные флаги (применяются к одному файлу):
  --info FILE           Только заголовок: магия, версия, имя карты, размер.
  --strings N           Все ASCII-строки длиной >= N (по умолчанию 6).
  --hexdump A B         Hex+ASCII дамп байтов [A, B).
  --find PATTERN        Поиск байтовой маски (hex или строка), все смещения.

Прочие флаги:
  --gap N               Допустимый разрыв между соседними изменениями
                        (в байтах; по умолчанию 4) — позволяет объединять
                        связанные поля в один диапазон.
  --limit N             Ограничение числа выводимых диапазонов (по умолчанию 200).
  --no-color            Отключить ANSI-цвета.

====================================================================
Примеры
====================================================================
  python3 gm1_diff.py --info save.gm1
  python3 gm1_diff.py save1.gm1 save2.gm1
  python3 gm1_diff.py a.gm1 b.gm1 --annotations offsets.json --limit 50
  python3 gm1_diff.py a.gm1 b.gm1 --section-map
  python3 gm1_diff.py save.gm1 --find "Christian"
  python3 gm1_diff.py save.gm1 --hexdump 0x1000 0x1100
  python3 gm1_diff.py save.gm1 --strings 8

====================================================================
Формат JSON-аннотаций
====================================================================
Поддерживаются два стиля (можно комбинировать в одном файле):

1) Плоский список полей:
   {
     "fields": [
       {"name": "hero[0].attack", "offset": 1472, "size": 1, "type": "u8"},
       {"name": "hero[0].exp",    "offset": 1511, "size": 4, "type": "u32"}
     ]
   }

2) Список секций с относительными полями и опциональным repeat:
   {
     "sections": [
       {"name": "hero", "start": 1000, "repeat": 8, "stride": 500,
        "fields": {"attack": {"offset": 238, "size": 1, "type": "u8"},
                   "exp":    {"offset": 239, "size": 4, "type": "u32"}}}
     ]
   }

Допустимые типы: u8, u16, u32, u64, i8, i16, i32, i64, f32, str, bytes.

====================================================================
Зависимости
====================================================================
Только стандартная библиотека Python 3 (gzip, argparse, json, re, struct).
"""

import argparse
import gzip
import json
import re
import struct
import sys
from typing import List, Tuple, Optional, Dict, Any

# ============================================================================
# Константы
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

# ANSI-коды для цветного вывода (можно отключить через --no-color).
ANSI = {
    "reset":   "\x1b[0m",
    "red":     "\x1b[31m",
    "green":   "\x1b[32m",
    "yellow":  "\x1b[33m",
    "blue":    "\x1b[34m",
    "magenta": "\x1b[35m",
    "cyan":    "\x1b[36m",
    "gray":    "\x1b[90m",
    "bold":    "\x1b[1m",
}


# ============================================================================
# Загрузка и базовый парсинг .GM1
# ============================================================================

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
        # HoMM3 иногда пишет gzip с неправильным CRC (или обрезает trailing
        # байты). Патчим gzip._GzipReader._read_eof, чтобы он не падал на этом.
        # Аналогично h3sed.patch_gzip_for_partial().
        import contextlib, io

        @contextlib.contextmanager
        def patch_gzip_for_partial():
            original = getattr(gzip, '_GzipReader', None)
            if original is not None and hasattr(original, '_read_eof'):
                orig_read_eof = original._read_eof
                def patched_read_eof(self):
                    # Потребляем 8 байт CRC + size, но не проверяем их
                    try:
                        self._fp.read(8)
                    except Exception:
                        pass
                original._read_eof = patched_read_eof
                try:
                    yield
                finally:
                    original._read_eof = orig_read_eof
            else:
                # Python без _GzipReader — просто yield
                yield

        try:
            with patch_gzip_for_partial():
                with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
                    raw = gf.read()
        except Exception:
            # Последняя попытка: zlib напрямую (DEFLATE без gzip-обёртки)
            # Берём содержимое после 10-байтного gzip-заголовка.
            try:
                import zlib
                # Пропускаем gzip-заголовок (минимум 10 байт) и используем
                # raw DEFLATE (wbits = -15).
                raw = zlib.decompress(data[10:], -15)
            except Exception:
                raw = data  # откатываемся к сырым данным
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
# Цветной вывод
# ============================================================================

def color(s: str, c: str, enable: bool = True) -> str:
    """Оборачивает строку s в ANSI-цвет c (если enable)."""
    return f"{ANSI[c]}{s}{ANSI['reset']}" if enable else s


def hex_dump_line(buf: bytes, ascii_color: str = "",
                  enable_color: bool = True) -> str:
    """Форматирует байтовую строку как hex с ASCII-представлением."""
    hexs = " ".join(f"{b:02x}" for b in buf)
    asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in buf)
    if ascii_color and enable_color:
        asciis = color(asciis, ascii_color)
    return f"{hexs}  |{asciis}|"


def format_range(a: bytes, b: bytes, start: int, end: int,
                 enable_color: bool, max_show: int = 64) -> str:
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

    off_s = color(f"0x{start:08x}", "cyan", enable_color)
    off_e = color(f"0x{end:08x}", "cyan", enable_color)
    sz = color(f"{size}b", "gray", enable_color)
    line_a = color("-", "red", enable_color) + " " + hex_dump_line(a_part, "red", enable_color)
    line_b = color("+", "green", enable_color) + " " + hex_dump_line(b_part, "green", enable_color)
    out = f"  [{off_s} .. {off_e}] ({sz})\n{line_a}\n{line_b}"
    if truncated:
        out += f"\n  ... {size - max_show} more bytes"
    return out


# ============================================================================
# Заголовок метаданных
# ============================================================================

def print_metadata_header(path_a: Optional[str], path_b: Optional[str],
                          a_info: Dict, b_info: Optional[Dict]):
    """Печатает стандартный заголовок с информацией о файлах."""
    enable = sys.stdout.isatty()
    print("=== GM1 DIFF ===")
    if path_b and b_info is not None:
        print(f"File A: {path_a}  ({a_info['compressed_size']} bytes compressed -> "
              f"{a_info['raw_size']} bytes raw, {a_info['magic']}, {a_info['version_name']})")
        print(f"File B: {path_b}  ({b_info['compressed_size']} bytes compressed -> "
              f"{b_info['raw_size']} bytes raw, {b_info['magic']}, {b_info['version_name']})")
        # Имя карты: если совпадают — одна строка, иначе — две
        if a_info.get("map_name") and a_info["map_name"] == b_info.get("map_name"):
            print(f'Map name: "{a_info["map_name"]}"')
        else:
            if a_info.get("map_name"):
                print(f'Map name A: "{a_info["map_name"]}"')
            if b_info.get("map_name"):
                print(f'Map name B: "{b_info["map_name"]}"')
    else:
        print(f"File: {path_a}  ({a_info['compressed_size']} bytes compressed -> "
              f"{a_info['raw_size']} bytes raw, {a_info['magic']}, {a_info['version_name']})")
        if a_info.get("map_name"):
            print(f'Map name: "{a_info["map_name"]}"')
    print()


# ============================================================================
# Режим A: байтовый diff (по умолчанию)
# ============================================================================

def cmd_byte_diff(a_raw, b_raw, a_info, b_info, args):
    print_metadata_header(args.file_a, args.file_b, a_info, b_info)

    ranges = find_changed_ranges(a_raw, b_raw, gap=args.gap)
    total_changed = sum(e - s for s, e in ranges)
    print(f"Total changed bytes: {total_changed} in {len(ranges)} ranges\n")

    if not ranges:
        print("Files are identical.")
        return

    enable_color = sys.stdout.isatty() and not args.no_color
    for i, (s, e) in enumerate(ranges[:args.limit]):
        print(format_range(a_raw, b_raw, s, e, enable_color))
        print()
    if len(ranges) > args.limit:
        print(f"... and {len(ranges) - args.limit} more ranges "
              f"(use --limit to see more)")


# ============================================================================
# Режим B: section map — гистограмма 1КБ-блоков
# ============================================================================

def cmd_section_map(a_raw, b_raw, a_info, b_info, args):
    print_metadata_header(args.file_a, args.file_b, a_info, b_info)

    block = 1024
    n = max(len(a_raw), len(b_raw))
    n_blocks = (n + block - 1) // block
    print(f"Section map (block size = {block} bytes, total {n} bytes / {n_blocks} blocks):")
    print()

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

    enable_color = sys.stdout.isatty() and not args.no_color
    per_line = 64
    for i in range(0, len(marks), per_line):
        offset = i * block
        label = color(f"0x{offset:08x}", "cyan", enable_color)
        chunk = "".join(marks[i:i + per_line])
        if enable_color:
            chunk = (chunk.replace("=", color("=", "gray"))
                          .replace("*", color("*", "red"))
                          .replace("?", color("?", "yellow")))
        print(f"{label}  {chunk}")

    print()
    print(f"Legend: {color('=', 'gray', enable_color)} unchanged   "
          f"{color('*', 'red', enable_color)} changed   "
          f"{color('?', 'yellow', enable_color)} one side longer")

    # Топ-20 самых длинных стабильных отрезков (вероятно, структура/паддинг).
    print()
    print("Top-20 longest unchanged runs (likely stable sections / padding):")
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
        print(f"  0x{s:08x} .. 0x{e:08x}  ({l} bytes)")


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


def cmd_annotated(a_raw, b_raw, a_info, b_info, args):
    print_metadata_header(args.file_a, args.file_b, a_info, b_info)
    fields = load_annotations(args.annotations)
    print(f"Loaded {len(fields)} annotated fields from {args.annotations}\n")

    idx = build_field_index(fields)

    ranges = find_changed_ranges(a_raw, b_raw, gap=args.gap)
    total_changed = sum(e - s for s, e in ranges)
    print(f"Total changed bytes: {total_changed} in {len(ranges)} ranges\n")

    enable_color = sys.stdout.isatty() and not args.no_color
    unknown_count = 0
    known_count = 0
    printed_ranges = 0
    last_field_printed: Optional[int] = None  # offset последнего показанного поля

    for s, e in ranges:
        if printed_ranges >= args.limit:
            print(f"\n... output truncated at {args.limit} ranges "
                  f"({len(ranges) - printed_ranges} more)")
            break
        # Идём побайтово внутри диапазона, группируя по полям
        i = s
        while i < e:
            field = idx.get(i)
            if field is None:
                # Неизвестный байт
                off = color(f"0x{i:08x}", "cyan", enable_color)
                tag = color("UNKNOWN_SECTION", "yellow", enable_color)
                oldv = color(f"{a_raw[i]:02x}", "red", enable_color)
                newv = color(f"{b_raw[i]:02x}", "green", enable_color)
                print(f"  [{off}] {tag}: {oldv} -> {newv}")
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
                    off = color(f"0x{field['offset']:08x}", "cyan", enable_color)
                    name = color(field["name"], "magenta", enable_color)
                    print(f"  [{off}] {name}: {oldv} -> {newv}")
                    known_count += 1
                    last_field_printed = field["offset"]
                # Если поле уходит за пределы текущего range — пропускаем весь остаток
                i = max(full_end, i + 1)
                if full_end > e:
                    # Поле выходит за диапазон; прерываем этот range
                    break
        printed_ranges += 1

    print()
    print(f"Summary: {known_count} attributed changes, "
          f"{unknown_count} in unknown regions")


# ============================================================================
# Вспомогательные команды (для одного файла)
# ============================================================================

def cmd_info(path, args):
    raw, info = load_save(path)
    print_metadata_header(path, None, info, None)
    print(f"Magic:           {info['magic']} ({info['type']})")
    print(f"Version major:   {info['version_major']} (0x{info['version_major']:X})")
    print(f"Version minor:   {info['version_minor']} (0x{info['version_minor']:X})")
    print(f"Version name:    {info['version_name']}")
    print(f"Compressed size: {info['compressed_size']}")
    print(f"Raw size:        {info['raw_size']}")
    print(f"Map name:        {info['map_name']!r}")


def cmd_strings(path, args):
    raw, info = load_save(path)
    min_len = args.strings
    print(f"=== Strings in {path} (min length {min_len}) ===")
    pattern = re.compile(rb"[\x20-\x7e]{%d,}" % min_len)
    count = 0
    enable_color = sys.stdout.isatty() and not args.no_color
    for m in pattern.finditer(raw):
        s = m.group(0).decode("ascii", errors="ignore")
        off = color(f"0x{m.start():08x}", "cyan", enable_color)
        # Обрежем длинные строки до 200 символов
        disp = s if len(s) <= 200 else s[:200] + "..."
        print(f"{off}  {disp}")
        count += 1
    print(f"\nTotal: {count} strings")


def cmd_hexdump(path, args):
    raw, info = load_save(path)
    start, end = args.hexdump
    if start < 0:
        start = 0
    if end > len(raw):
        end = len(raw)
    print(f"=== Hex dump {path} [0x{start:x} .. 0x{end:x}] ===")
    enable_color = sys.stdout.isatty() and not args.no_color
    for i in range(start, end, 16):
        chunk = raw[i:i + 16]
        hexs = " ".join(f"{b:02x}" for b in chunk)
        hexs = hexs.ljust(16 * 3 - 1)
        asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in chunk)
        off = color(f"{i:08x}", "cyan", enable_color)
        print(f"{off}  {hexs}  |{asciis}|")


def cmd_find(path, args):
    raw, info = load_save(path)
    pat = args.find
    # Интерпретация: hex если длина чётная и все символы hex/пробелы, иначе строка
    cleaned = pat.replace(" ", "")
    is_hex = (len(cleaned) >= 2 and len(cleaned) % 2 == 0
              and all(c in "0123456789abcdefABCDEF" for c in cleaned))
    if is_hex:
        needle = bytes.fromhex(cleaned)
        print(f"Searching for hex pattern: {needle.hex()} ({len(needle)} bytes)")
    else:
        needle = pat.encode("utf-8")
        print(f"Searching for string: {pat!r} ({len(needle)} bytes)")
    print(f"In file: {path}  (raw size {len(raw)})")
    print()
    enable_color = sys.stdout.isatty() and not args.no_color
    count = 0
    i = raw.find(needle)
    while i != -1:
        ctx_start = max(0, i - 8)
        ctx_end = min(len(raw), i + len(needle) + 8)
        ctx = raw[ctx_start:ctx_end]
        off = color(f"0x{i:08x}", "cyan", enable_color)
        hexs = " ".join(f"{b:02x}" for b in ctx)
        asciis = "".join(chr(b) if 32 <= b < 127 else "." for b in ctx)
        print(f"{off}  {hexs}  |{asciis}|")
        count += 1
        i = raw.find(needle, i + 1)
    print(f"\nTotal: {count} matches")


# ============================================================================
# Точка входа
# ============================================================================

def main():
    parser = argparse.ArgumentParser(
        description="Дифференциальный компаратор .GM1 сохранений Heroes III.",
        epilog=(
            "Примеры:\n"
            "  gm1_diff.py --info save.gm1\n"
            "  gm1_diff.py save1.gm1 save2.gm1\n"
            "  gm1_diff.py a.gm1 b.gm1 --annotations offsets.json --limit 50\n"
            "  gm1_diff.py a.gm1 b.gm1 --section-map\n"
            "  gm1_diff.py save.gm1 --find Christian\n"
            "  gm1_diff.py save.gm1 --hexdump 0x1000 0x1100\n"
            "  gm1_diff.py save.gm1 --strings 8\n"
        ),
        formatter_class=argparse.RawDescriptionHelpFormatter,
    )
    # Позиционные: один или два файла
    parser.add_argument("file_a", nargs="?",
                        help="первый файл (или единственный для --info/--strings/...)")
    parser.add_argument("file_b", nargs="?", help="второй файл для сравнения")

    # Режимы diff (для двух файлов)
    parser.add_argument("--byte-diff", action="store_true",
                        help="режим A (по умолчанию): байтовый diff")
    parser.add_argument("--section-map", action="store_true",
                        help="режим B: гистограмма 1КБ-блоков")
    parser.add_argument("--annotated", action="store_true",
                        help="режим C: diff с аннотациями из --annotations FILE")
    parser.add_argument("--annotations", metavar="FILE",
                        help="JSON-файл с известными смещениями полей")
    parser.add_argument("--gap", type=int, default=4,
                        help="допустимый разрыв между соседними изменениями (по умолчанию 4)")
    parser.add_argument("--limit", type=int, default=200,
                        help="ограничение числа выводимых диапазонов (по умолчанию 200)")
    parser.add_argument("--no-color", action="store_true",
                        help="отключить ANSI-цвета")

    # Вспомогательные команды (для одного файла)
    parser.add_argument("--info", metavar="FILE",
                        help="только заголовок файла и выход")
    parser.add_argument("--strings", type=int, nargs="?", const=6, default=None,
                        help="все ASCII-строки длиной >= N (по умолчанию 6)")
    parser.add_argument("--hexdump", nargs=2, metavar=("START", "END"),
                        help="hex+ASCII дамп байтов [START, END)")
    parser.add_argument("--find", metavar="PATTERN",
                        help="поиск байтового паттерна (hex или строка)")

    args = parser.parse_args()

    # --- Команды одного файла ---
    if args.info:
        cmd_info(args.info, args)
        return
    if args.strings is not None:
        if not args.file_a:
            print("ERROR: --strings требует имени файла", file=sys.stderr)
            sys.exit(2)
        cmd_strings(args.file_a, args)
        return
    if args.hexdump:
        if not args.file_a:
            print("ERROR: --hexdump требует имени файла", file=sys.stderr)
            sys.exit(2)
        try:
            args.hexdump = [int(args.hexdump[0], 0), int(args.hexdump[1], 0)]
        except ValueError:
            print("ERROR: смещения должны быть целыми (0x... или decimal)", file=sys.stderr)
            sys.exit(2)
        cmd_hexdump(args.file_a, args)
        return
    if args.find:
        if not args.file_a:
            print("ERROR: --find требует имени файла", file=sys.stderr)
            sys.exit(2)
        cmd_find(args.file_a, args)
        return

    # --- С этого момента нужен file_a ---
    if not args.file_a:
        parser.print_help()
        sys.exit(2)

    # Если файл один и нет спец-команды — показываем info
    if not args.file_b:
        cmd_info(args.file_a, args)
        return

    # --- Загружаем оба файла ---
    a_raw, a_info = load_save(args.file_a)
    b_raw, b_info = load_save(args.file_b)

    # --- Маршрутизация по режимам ---
    if args.section_map:
        cmd_section_map(a_raw, b_raw, a_info, b_info, args)
    elif args.annotated or args.annotations:
        if not args.annotations:
            print("ERROR: --annotated требует --annotations FILE", file=sys.stderr)
            sys.exit(2)
        cmd_annotated(a_raw, b_raw, a_info, b_info, args)
    else:
        # Режим по умолчанию: byte diff
        cmd_byte_diff(a_raw, b_raw, a_info, b_info, args)


if __name__ == "__main__":
    main()
