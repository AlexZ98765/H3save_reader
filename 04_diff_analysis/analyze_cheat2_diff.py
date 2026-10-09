#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_5 → 0020_6.
Контекст: применил чит-коды для героя:
  - добавил существ
  - дал много ходов (movement)

Главные находки в диффе:
  0x140b26..0x140b2e (8b): ff ff ff ff → 64 00 00 00 53 00 00 00
                          Появились значения 0x64 (100), 0x53 (83) — visiting coords?
  0x140b36..0x140b39 (3b): 00 00 00 → df 93 04
                          u24/u32 LE: появилось 0x0493df = 299999 — MOVEMENT!
  0x140b84..0x140b8c (8b): ff ff ff ff → 84 00 00 00 87 00 00 00
                          Появились значения 0x84 (132), 0x87 (135) — координаты?
  0x140ba0..0x140ba5 (5b): 00 00 00 00 00 → 05 00 00 00 05
                          Появились значения 5, 5 — COUNT = 5!
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

A = load('/home/z/my-project/upload/0020_5.GM1')
B = load('/home/z/my-project/upload/0020_6.GM1')
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
    (0x140b26, 0x140b39, "⭐⭐ Visiting coords + MOVEMENT cheat"),
    (0x140b84, 0x140ba5, "⭐⭐ NEW coords + COUNT = 5 (creatures!)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 64)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_5, до читов):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_6, с читами):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ MOVEMENT cheat ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ MOVEMENT CHEAT")
print("="*80)

val_a = int.from_bytes(A[0x140b36:0x140b3a], 'little')
val_b = int.from_bytes(B[0x140b36:0x140b3a], 'little')
print(f"u32 @ 0x140b36: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  ⭐ B = 299999 = 0x0493DF — это MOVEMENT POINTS (как в диффе 0020→0020_1)!")
print(f"  Тот же чит-код 'nwcneo' = бесконечные ходы")

# === Анализ COUNT = 5 ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ COUNT = 5 (CREATURES)")
print("="*80)

val_a = int.from_bytes(A[0x140ba0:0x140ba5], 'little')
val_b = int.from_bytes(B[0x140ba0:0x140ba5], 'little')
print(f"u32 @ 0x140ba0: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  ⭐ Появились значения 5 — это COUNT добавленных существ!")
print(f"  B = 0x00000005 = 5 существ (по словам игрока)")

# === Анализ новых координат ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ НОВЫХ КООРДИНАТ (0x140b84)")
print("="*80)

val1_a = int.from_bytes(A[0x140b84:0x140b88], 'little')
val2_a = int.from_bytes(A[0x140b88:0x140b8c], 'little')
val1_b = int.from_bytes(B[0x140b84:0x140b88], 'little')
val2_b = int.from_bytes(B[0x140b88:0x140b8c], 'little')
print(f"u32 #1 @ 0x140b84: A=0x{val1_a:08x} ({val1_a if val1_a<0x80000000 else -1}) → B=0x{val1_b:08x} ({val1_b})")
print(f"u32 #2 @ 0x140b88: A=0x{val2_a:08x} ({val2_a if val2_a<0x80000000 else -1}) → B=0x{val2_b:08x} ({val2_b})")
print(f"  ⭐ B: (132, 135) — координаты героя после чита")
print(f"  Hero moved to a new location")

# === Анализ visiting coords (0x140b26) ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ VISITING COORDS (0x140b26)")
print("="*80)
val1_a = int.from_bytes(A[0x140b26:0x140b2a], 'little')
val2_a = int.from_bytes(A[0x140b2a:0x140b2e], 'little')
val1_b = int.from_bytes(B[0x140b26:0x140b2a], 'little')
val2_b = int.from_bytes(B[0x140b2a:0x140b2e], 'little')
print(f"u32 #1 @ 0x140b26: A=0x{val1_a:08x} ({val1_a if val1_a<0x80000000 else -1}) → B=0x{val1_b:08x} ({val1_b})")
print(f"u32 #2 @ 0x140b2a: A=0x{val2_a:08x} ({val2_a if val2_a<0x80000000 else -1}) → B=0x{val2_b:08x} ({val2_b})")
print(f"  ⭐ B: (100, 83) — visiting coords героя (новая позиция)")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ЧИТ-КОДЫ: ДОБАВЛЕНИЕ СУЩЕСТВ + MOVEMENT — НАЙДЕНЫ:

1. ⭐⭐⭐ MOVEMENT POINTS (чит-код)
   Смещение: 0x140b36 (4b, u32 LE)
   A: 0x00000000 (0) → B: 0x000493df (299999)
   
   ⭐ Это АНАЛОГ current_movement_points, но в ДРУГОМ блоке!
   Ранее (0020→0020_1) мы видели movement на смещении 0x157f00.
   Сейчас movement установлен на 0x140b36 — это другой блок.
   
   ⭐ ВЫВОД: в файле есть НЕСКОЛЬКО копий movement points!
     - 0x157f00 — для главного активного героя (Hero #1?)
     - 0x140b36 — для героя-гостя (Hero #2 или Hero #4?)
   
   Это подтверждает, что hero blocks разбросаны по файлу
   с переменным stride.

2. ⭐⭐⭐ COUNT = 5 (добавленные существа)
   Смещение: 0x140ba0 (4b, u32 LE)
   A: 0x00000000 (0) → B: 0x00000005 (5)
   
   ⭐ Это COUNT существ в слоте армии!
   Чит-код добавил 5 существ (как и сказал игрок).
   
   По структуре это army_counts[slot] — аналог 0x163734,
   но для другого героя.

3. ⭐⭐ НОВЫЕ КООРДИНАТЫ (0x140b84)
   Смещение: 0x140b84 (8b, 2 × u32 LE)
   A: 0xFFFFFFFF 0xFFFFFFFF (-1, -1)
   B: 0x00000084 0x00000087 (132, 135)
   
   ⭐ Это КОординаты героя после чита.
   Ранее мы видели coords на:
     - 0x140b26 — visiting coords (старые)
     - 0x140b84 — НОВЫЕ coords для другого блока героя
   Возможно, это массив "последних известных позиций" нескольких героев.

4. ⭐ VISITING COORDS обновились (0x140b26)
   Смещение: 0x140b26 (8b, 2 × u32 LE)
   A: (-1, -1) → B: (100, 83)
   ⭐ Visiting coords героя обновились (он посетил что-то)

5. ⭐⭐ СТРУКТУРНОЕ ОТКРЫТИЕ
   В файле есть несколько БЛОКОВ ДАННЫХ ГЕРОЯ, расположенных
   в разных местах файла (не подряд):
   
   БЛОК 1 (Hero #1, активный):
     0x157f00 — current movement points (u32 LE)
     0x157f04 — experience (u32 LE)
     0x157f08 — level (u8)
     ...
     0x163718 — army_types (28b)
     0x163734 — army_counts (28b)
   
   БЛОК 2 (Hero #2, гость):
     0x140b26 — visiting coords (8b)
     0x140b36 — movement points (u32 LE)  ← НОВОЕ
     0x140b84 — coords (8b)               ← НОВОЕ
     0x140ba0 — army_counts (4b)          ← НОВОЕ
   
   ⭐ Каждый герой имеет свой блок с похожей структурой,
     но блоки расположены с переменным stride.

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x140b36: hero movement points (alt block) (u32 LE)
            — 299999 = cheat code for unlimited movement
  0x140b84: hero coords (alt block) (2 × u32 LE)
            — координаты героя в альтернативном блоке
  0x140ba0: hero army count slot (alt block) (u32 LE)
            — count of creatures in army slot (5 в нашем случае)
""")
