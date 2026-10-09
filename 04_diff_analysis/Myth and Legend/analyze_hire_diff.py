#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0014 → 0015.
Контекст: нанял 5 существ 6-го уровня в городе, оставил их в городе.

Главные находки в диффе:
  - 0x13e481: u16 LE 0x709d → 0x6003 (delta = -4250)  ← золото игрока?
  - 0x13f60f: u32 LE 0xffffffff → 0x50 (80)             ← ID существа?
  - 0x13f62b: u8 0x00 → 0x05                            ← COUNT = 5! ✅
  - 0x13f65f: u8 0x08 → 0x03                            ← slot flag
"""
import gzip, io, contextlib

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

A = load('/home/z/my-project/upload/0014.GM1')
B = load('/home/z/my-project/upload/0015.GM1')
print(f"A: {len(A)}, B: {len(B)}, diff: {len(B)-len(A)}")

def hex_dump(data, start, end, prefix="    "):
    chunk = data[start:end]
    lines = []
    for i in range(0, len(chunk), 16):
        s = chunk[i:i+16]
        hex_part = ' '.join(f'{b:02x}' for b in s)
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in s)
        lines.append(f"{prefix}{start+i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return '\n'.join(lines)

CLUSTERS = [
    (0x13e481, 0x13e483, "Player resource (gold)"),
    (0x13f60f, 0x13f613, "⭐ Creature ID appeared"),
    (0x13f62b, 0x13f62c, "⭐ Creature COUNT = 5"),
    (0x13f65f, 0x13f660, "Slot flag"),
]

print("\n" + "="*80)
print("ДЕТАЛЬНЫЙ ДАМП КАЖДОГО КЛАСТЕРА")
print("="*80)

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 80)
    ctx_end = min(len(A), ce + 80)
    
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x} (изм {ce-cs}b)")
    print(f"{'='*80}")
    print("A (0014, до найма):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0015, нанял 5 существ 6 ур.):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ структуры гарнизона ===
print("\n" + "="*80)
print("АНАЛИЗ СТРУКТУРЫ ГАРНИЗОНА ГОРОДА")
print("="*80)

# Покажем широкий контекст вокруг 0x13f60f, чтобы увидеть весь стек существ
print("\n— Широкий контекст 0x13f5f0..0x13f6a0 (вероятно массив гарнизона):")
print("A:")
print(hex_dump(A, 0x13f5f0, 0x13f6a0))
print("B:")
print(hex_dump(B, 0x13f5f0, 0x13f6a0))

# === Интерпретация ===
print("\n" + "="*80)
print("ИНТЕРПРЕТАЦИЯ")
print("="*80)

print("""
[1] 0x000003b6: '4' → '5'  — последний символ имени файла (0014 → 0015)

[2] 0x00130b71..0x00130c11 (base64) — save metadata, всегда меняется

[3] ⭐ 0x0013e481 (2b, u16 LE):
   A: 9d 70 = 0x709d = 28829
   B: 03 60 = 0x6003 = 24579
   Delta = -4250
   → Игрок потратил 4250 золота на найм 5 существ 6-го уровня
   → Цена 1 существа = 850 золота (типичная цена для существ 6 ур. в HoMM3)
   ⭐ Смещение 0x13e481 — это ЗОЛОТО ИГРОКА (u16 LE)

[4] ⭐⭐⭐ 0x0013f60f (4b, u32 LE):
   A: ff ff ff ff  (слот пуст)
   B: 50 00 00 00  (0x50 = 80)
   → Появилось значение 80 в слоте гарнизона!
   → 80 (0x50) — это ID существа 6-го уровня (в HoMM3 ID существ 6 ур. ~ 70-100)
   ⭐ Смещение 0x13f60f — это СЛОТ ГАРНИЗОНА (creature ID, u32 LE)

[5] ⭐⭐⭐ 0x0013f62b (1b, u8):
   A: 00  (слот пуст)
   B: 05  (5 существ!)
   → COUNT = 5! Точное совпадение с количеством нанятых существ!
   ⭐ Смещение 0x13f62b — это COUNT существ в слоте гарнизона (u8)

[6] 0x0013f65f (1b, u8):
   A: 08  →  B: 03
   → Возможно: позиция слота в гарнизоне (slot index 8 → 3?)
   Или: флаг "свободный слот" сменился на "занятый слот 3"

ВОССТАНОВЛЕННАЯ СТРУКТУРА СЛОТА ГАРНИЗОНА:
  Смещение  +0: u32 LE — creature ID (0xFFFFFFFF = пусто)
  Смещение +4: ?       — возможно u32 alignment
  ...
  Смещение +28: u8     — count (количество существ в слоте)

Размер слота: 0x13f62b - 0x13f60f = 0x1c = 28 байт
Каждый слот гарнизона = 28 байт
Гарнизон = 7 слотов × 28 байт = 196 байт

Или альтернативно: creature ID и count хранятся в разных массивах:
  Массив creature_ids[7]: 0x13f60f, +4, +8, +12, +16, +20, +24
  Массив counts[7]:       0x13f60f + 28, +29, +30, ...

Похоже на структуру h3sed:
  HERO_BYTE_POSITIONS = {
    "army_types":  113,  # 7 × u32 creature IDs (28 байт)
    "army_counts": 141,  # 7 × u32 creature counts (28 байт)
  }
То есть IDs и counts в разных массивах, по 28 байт каждый.

В городе аналогично:
  0x13f60f — army_types[7] массив (28 байт)
  0x13f60f + 28 = 0x13f62b — army_counts[7] массив (28 байт)  ← НАШЛИ COUNT!

КЛАСТЕР 0x13f60f..0x13f65f — это армия города:
  +0x00..0x1c (28b): 7 × u32 creature IDs
  +0x1c..0x38 (28b): 7 × u32 creature counts
  +0x38: u8 slot flag или available-for-hire флаг
""")

# === ИТОГ ===
print("="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
✅ НАЙДЕНО:

1. ⭐⭐⭐ ID СУЩЕСТВА В ГАРНИЗОНЕ ГОРОДА
   Смещение: 0x13f60f (u32 LE)
   A: 0xFFFFFFFF (пустой слот)
   B: 0x50 = 80 (ID существа 6-го уровня)
   → Артефактный формат: 7 слотов по u32 LE

2. ⭐⭐⭐ КОЛИЧЕСТВО СУЩЕСТВ В СЛОТЕ
   Смещение: 0x13f62b (= 0x13f60f + 28) (u8)
   A: 0x00 → B: 0x05 (5 существ!)
   → Точное совпадение с заявленным количеством
   → Структура: army_counts[7] массив, по 1 байту на слот

3. ⭐⭐ ЗОЛОТО ИГРОКА
   Смещение: 0x13e481 (u16 LE)
   A: 28829 → B: 24579 (delta = -4250)
   → 4250 / 5 = 850 золотых за одно существо 6-го уровня
   → Подтверждает типичную цену найма в HoMM3

4. ⭐ СЛОТ-ФЛАГ ГАРНИЗОНА
   Смещение: 0x13f65f (u8)
   A: 0x08 → B: 0x03
   → Флаг "доступно для найма" изменился

СТРУКТУРНОЕ ОТКРЫТИЕ:
   0x13f60f..0x13f62b (28b) — массив creature IDs (7 × u32 LE)  ← ARMY_TYPES
   0x13f62b..0x13f647 (28b) — массив creature counts (7 × u8 + padding)
   (Stride между слотами = 4 байта для IDs, 1 байт для counts)

   Это СТРУКТУРА АРМИИ города — аналог hero army в h3sed:
     HERO_BYTE_POSITIONS["army_types"]  = 113  # 7 × u32
     HERO_BYTE_POSITIONS["army_counts"] = 141  # 7 × u32
   Но в городе counts хранятся компактнее (по 1 байту на слот, не 4).
""")
