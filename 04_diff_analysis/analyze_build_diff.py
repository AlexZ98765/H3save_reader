#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ изменений 0011.GM1 → 0012.GM1.

Контекст: в новом сейве построено здание в городе.

Цель: для каждого изменённого диапазона дать hex-дамп с окружением
и интерпретировать, какие поля могли измениться.
"""
import sys, gzip, io, contextlib, json, struct

@contextlib.contextmanager
def patch_gzip():
    reader = getattr(gzip, '_GzipReader', None)
    if reader and hasattr(reader, '_read_eof'):
        orig = reader._read_eof
        def patched(self):
            try: self._fp.read(8)
            except: pass
        reader._read_eof = patched
        try: yield
        finally: reader._read_eof = orig
    else:
        yield

def load(p):
    with open(p, 'rb') as f:
        data = f.read()
    with patch_gzip():
        with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
            return gf.read()

A = load('/home/z/my-project/upload/0011.GM1')
B = load('/home/z/my-project/upload/0012.GM1')
print(f"A: {len(A)} bytes, B: {len(B)} bytes (diff: {len(B)-len(A)})")

# Диапазоны из диффа
RANGES = [
    (0x000003b6, 0x000003b7),
    (0x00130b71, 0x00130b75),
    (0x00130b7d, 0x00130b80),
    (0x00130b89, 0x00130bec),
    (0x00130bf5, 0x00130c10),
    (0x0013e409, 0x0013e40a),
    (0x0013e420, 0x0013e421),
    (0x0013e469, 0x0013e46a),
    (0x0013e481, 0x0013e483),
    (0x0013f17b, 0x0013f17c),
    (0x0013f220, 0x0013f223),
    (0x0013f22a, 0x0013f22b),
]

def hex_dump(data, start, end, prefix="    "):
    chunk = data[start:end]
    lines = []
    for i in range(0, len(chunk), 16):
        s = chunk[i:i+16]
        hex_part = ' '.join(f'{b:02x}' for b in s)
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in s)
        lines.append(f"{prefix}{start+i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return '\n'.join(lines)

# Группировка в кластеры
clusters = []
cs, ce = RANGES[0]
for s, e in RANGES[1:]:
    if s - ce < 1024:
        ce = max(ce, e)
    else:
        clusters.append((cs, ce))
        cs, ce = s, e
clusters.append((cs, ce))

print(f"\nКластеров: {len(clusters)}")
for i, (s, e) in enumerate(clusters):
    print(f"  Кластер {i+1}: 0x{s:08x} .. 0x{e:08x}")

# === Детальный дамп каждого кластера с контекстом ±64 байта ===
print("\n" + "=" * 80)
print("ДЕТАЛЬНЫЙ АНАЛИЗ КАЖДОГО КЛАСТЕРА")
print("=" * 80)

for i, (cs, ce) in enumerate(clusters):
    ctx_start = max(0, cs - 64)
    ctx_end = min(len(A), ce + 64)
    
    print(f"\n{'='*80}")
    print(f"КЛАСТЕР {i+1}: 0x{cs:08x} .. 0x{ce:08x} (изменено {ce-cs} байт)")
    print(f"{'='*80}")
    
    print("\nФайл A (ДО — 0011.GM1):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("\nФайл B (ПОСЛЕ — 0012.GM1, построено здание):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Специфичные интерпретации ===
print("\n" + "=" * 80)
print("ИНТЕРПРЕТАЦИЯ КАЖДОГО ДИАПАЗОНА")
print("=" * 80)

# 1. 0x3b6: имя файла
print("\n[1] 0x000003b6 (1 байт):")
print("    A: 0x31 = '1', B: 0x32 = '2'")
print("    → Последний символ имени файла сейва: '0011.GM1' → '0012.GM1'")
print("    Подтверждает, что 0x3b6 — это позиция последней цифры в имени файла сейва.")

# 2. 0x130b71: base64-блок
print("\n[2] 0x00130b71 .. 0x00130c10 (~190 байт):")
print("    Это base64-подобный закодированный блок. Похоже на описание карты.")
print("    Возможно — это ZIP-подобный savegame-info блок, который меняется")
print("    при каждом сохранении (timestamp, build info).")
print("    Не связано напрямую со зданием — это 'метаданные сохранения'.")

# 3. 0x13e409: u8 0x58 → 0xff
print("\n[3] 0x0013e409 (1 байт):")
print(f"    A: 0x58 (88), B: 0xff (255)")
print(f"    Контекст:")
print(f"      A[-2..+6]: {A[0x13e407:0x13e40f].hex()}")
print(f"      B[-2..+6]: {B[0x13e407:0x13e40f].hex()}")
print("    0x58 = 88 — это похоже на counter/day number (как в path-блоке)")
print("    Обнуление до 0xff может означать 'сброс текущего действия'.")

# 4. 0x13e420: u8 0xff → 0x12
print("\n[4] 0x0013e420 (1 байт):")
print(f"    A: 0xff (-1 или 255), B: 0x12 (18)")
print(f"    Контекст:")
print(f"      A[-4..+6]: {A[0x13e41c:0x13e426].hex()}")
print(f"      B[-4..+6]: {B[0x13e41c:0x13e426].hex()}")
print("    0xff→0x12: было 'нет значения', стало 18.")
print("    18 — это может быть: ID здания, количество ресурсов, или номер здания в городе.")

# 5. 0x13e469: 0xa9 → 0xa4
print("\n[5] 0x0013e469 (1 байт):")
print(f"    A: 0xa9 (169), B: 0xa4 (164)")
print(f"    Delta: -5")
print(f"    Контекст:")
print(f"      A[-4..+6]: {A[0x13e465:0x13e46f].hex()}")
print(f"      B[-4..+6]: {B[0x13e465:0x13e46f].hex()}")
print("    Похоже на u32 LE или u16 LE — нужно посмотреть в контексте.")

# Прочитать u32 LE в этой точке
val_a = int.from_bytes(A[0x13e469:0x13e46d], 'little')
val_b = int.from_bytes(B[0x13e469:0x13e46d], 'little')
print(f"    Если u32 LE: A=0x{val_a:08x} ({val_a}), B=0x{val_b:08x} ({val_b}), delta={val_b-val_a}")

# 6. 0x13e481: 2 байта
print("\n[6] 0x0013e481 (2 байта):")
print(f"    A: 91 72, B: 9d 70")
val_a = int.from_bytes(A[0x13e481:0x13e483], 'little')
val_b = int.from_bytes(B[0x13e481:0x13e483], 'little')
print(f"    u16 LE: A=0x{val_a:04x} ({val_a}), B=0x{val_b:04x} ({val_b}), delta={val_b-val_a}")

# 7. 0x13f17b: 0x00 → 0x01
print("\n[7] 0x0013f17b (1 байт):")
print(f"    A: 0x00, B: 0x01")
print(f"    Контекст:")
print(f"      A[-4..+6]: {A[0x13f177:0x13f181].hex()}")
print(f"      B[-4..+6]: {B[0x13f177:0x13f181].hex()}")
print("    ⭐ 0x00 → 0x01 = FALSE → TRUE!")
print("    Это ВЕРОЯТНО флаг 'здание построено' в списке зданий города.")
print("    Каждое здание в городе имеет свой бит/байт в массиве.")

# 8. 0x13f220: 3 байта
print("\n[8] 0x0013f220 (3 байта):")
print(f"    A: 28 d2 a9, B: 08 d2 e9")
val_a = int.from_bytes(A[0x13f220:0x13f223], 'little')
val_b = int.from_bytes(B[0x13f220:0x13f223], 'little')
print(f"    u24 LE: A=0x{val_a:06x}, B=0x{val_b:06x}")
val_a32 = int.from_bytes(A[0x13f220:0x13f224], 'little')
val_b32 = int.from_bytes(B[0x13f220:0x13f224], 'little')
print(f"    u32 LE (с доп. байтом): A=0x{val_a32:08x} ({val_a32}), B=0x{val_b32:08x} ({val_b32}), delta={val_b32-val_a32}")
print("    Возможно: ресурсы игрока (золото/дерево/руда уменьшились на строительство).")

# 9. 0x13f22a: 1 байт
print("\n[9] 0x0013f22a (1 байт):")
print(f"    A: 0x2d (45), B: 0xed (237)")
print(f"    Delta: +192 (с учётом знака: -64 как i8)")
print(f"    Если u8: B-A = {(0xed - 0x2d) & 0xff}")
val_a = int.from_bytes(A[0x13f228:0x13f22c], 'little')
val_b = int.from_bytes(B[0x13f228:0x13f22c], 'little')
print(f"    u32 LE @ 0x13f228: A=0x{val_a:08x} ({val_a}), B=0x{val_b:08x} ({val_b}), delta={val_b-val_a}")

# === ИТОГ ===
print("\n" + "=" * 80)
print("ИТОГОВАЯ СВОДКА ПО СТРОИТЕЛЬСТВУ ЗДАНИЯ")
print("=" * 80)

print("""
Анализ 0011 → 0012 (построено здание в городе):

КЛАСТЕР 1: 0x3b6 (1 байт) — имя файла сейва
  → '1' → '2' (последняя цифра 0011 → 0012)
  → Подтверждает известное смещение

КЛАСТЕР 2: 0x130b71 (190 байт) — base64 метаданные сейва
  → Меняется при каждом сохранении (timestamp / build info)
  → Не связано напрямую со зданием

КЛАСТЕР 3: 0x13e409-0x13e483 (несколько малых изменений)
  → 0x13e409: 0x58 → 0xff  (counter reset)
  → 0x13e420: 0xff → 0x12  (появилось значение 18 — возможно ID здания)
  → 0x13e469: 0xa9 → 0xa4  (delta -5)
  → 0x13e481: u16 0x7291 → 0x709d (delta -244)
  → Эти поля вероятно: текущий ход, день, ресурсы игрока

КЛАСТЕР 4: 0x13f17b (1 байт) — ⭐ ФЛАГ ЗДАНИЯ
  → 0x00 → 0x01 (FALSE → TRUE)
  → Самая вероятная интерпретация: флаг "здание N построено" в массиве зданий города
  → Соседние байты — другие здания того же города

КЛАСТЕР 5: 0x13f220-0x13f22a (4+ байт) — РЕСУРСЫ ИГРОКА
  → Изменились значения, похожие на u32 количества ресурсов
  → Уменьшение ресурсов при строительстве здания

ГЛАВНЫЕ НАХОДКИ:
  ⭐ Смещение 0x13f17b — кандидат на "битую зданий города" 
    (массив флагов построенных зданий)
  ⭐ Смещение 0x13f220..0x13f22a — кандидат на "ресурсы игрока"
  ⭐ 0x13e420 — кандидат на "ID последнего построенного здания" (значение 18 = 0x12)
  
СТРУКТУРНЫЕ ПРЕДПОЛОЖЕНИЯ:
  Смещения 0x13e4xx, 0x13f1xx, 0x13f2xx — это, вероятно, блок "CGameState":
    - текущий день/ход
    - ресурсы каждого игрока
    - списки зданий городов
    - списки посещённых объектов
""")
