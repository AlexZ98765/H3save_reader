#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0017 → 0018.
Контекст: изменил трейс пути игрока (queued path / movement destination).

Главное изменение:
  0x1636b2..0x1636ba (8 байт):
    A: ff ff ff ff ff ff ff ff  (нет пути)
    B: 5d 00 00 00 7d 00 00 00  (X=93, Y=125)
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

A = load('/home/z/my-project/upload/0017.GM1')
B = load('/home/z/my-project/upload/0018.GM1')
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

# Главный кластер: 0x1636b2..0x1636ba
print("=" * 80)
print("📍 ГЛАВНОЕ ИЗМЕНЕНИЕ: 0x1636b2..0x1636ba (8 байт)")
print("=" * 80)
print("\nA (0017, до изменения пути):")
print(hex_dump(A, 0x163680, 0x1636e0))
print("\nB (0018, после изменения пути):")
print(hex_dump(B, 0x163680, 0x1636e0))

# Извлечём значения
val_a1 = int.from_bytes(A[0x1636b2:0x1636b6], 'little')
val_a2 = int.from_bytes(A[0x1636b6:0x1636ba], 'little')
val_b1 = int.from_bytes(B[0x1636b2:0x1636b6], 'little')
val_b2 = int.from_bytes(B[0x1636b6:0x1636ba], 'little')

print(f"\n--- Значения ---")
print(f"u32 LE @ 0x1636b2: A=0x{val_a1:08x} ({val_a1 if val_a1 < 0x80000000 else -1})")
print(f"                  B=0x{val_b1:08x} ({val_b1})")
print(f"u32 LE @ 0x1636b6: A=0x{val_a2:08x} ({val_a2 if val_a2 < 0x80000000 else -1})")
print(f"                  B=0x{val_b2:08x} ({val_b2})")

print(f"\n--- Интерпретация ---")
print(f"A: 0xFFFFFFFF 0xFFFFFFFF = (-1, -1) = путь не задан (нет трейса)")
print(f"B: 0x0000005D 0x0000007D = (93, 125)")
print(f"")
print(f"⭐ Это КООРДИНАТЫ ЦЕЛИ ПУТИ ГЕРОЯ (queued path destination)!")
print(f"   Когда игрок кликает по карте, чтобы проложить путь герою,")
print(f"   сюда записывается целевая точка (X=93, Y=125).")
print(f"   Значение 0xFFFFFFFF = путь не задан.")

# === Сравнение с предыдущими находками ===
print("\n" + "=" * 80)
print("СРАВНЕНИЕ С ПРЕДЫДУЩИМИ КООРДИНАТАМИ")
print("=" * 80)

print("""
Типы координат героя в save-файле:

1. ТЕКУЩАЯ ПОЗИЦИЯ ГЕРОЯ
   Смещение: 0x157ec3 (UTF-16 LE строки, по 2 байта на X и Y)
   Пример: '4' 'a' → (4, 10)
   Это где герой стоит прямо сейчас.

2. ЦЕЛЕВАЯ ТОЧКА ПУТИ (QUEUED PATH DESTINATION)  ← НОВАЯ НАХОДКА
   Смещение: 0x1636b2 (2 × u32 LE)
   Пример: (93, 125) или (-1, -1) если путь не задан
   Это куда игрок приказал герою идти.

3. КООРДИНАТЫ ГОСТЯ ГОРОДА
   Смещение: 0x143e6e (2 × u32 LE)
   Пример: (100, 103)
   Это позиция героя, когда он посещает город.

4. PATH-БЛОК (ИСТОРИЯ ДВИЖЕНИЙ)
   Смещение: ~0x1814d0..0x1815d0
   Формат: записи переменной длины (15/19/27 байт)
   Это лог всех движений героя (для реплея).

5. ПОКИНУТЫЕ ТАЙЛЫ
   Смещение: ~0x157eec (struct с counter, X, Y)
   Это тайлы, которые герой посетил и покинул.
""")

# === ИТОГ ===
print("=" * 80)
print("ИТОГ — КЛЮЧЕВАЯ НАХОДКА")
print("=" * 80)
print("""
⭐⭐⭐ НАЙДЕНО: ЦЕЛЕВАЯ ТОЧКА ПУТИ ГЕРОЯ (QUEUED PATH DESTINATION)

Смещение: 0x1636b2..0x1636ba (8 байт = 2 × u32 LE)
Формат:
  u32 LE  X  — целевая X-координата (0xFFFFFFFF = путь не задан)
  u32 LE  Y  — целевая Y-координата (0xFFFFFFFF = путь не задан)

A (0017): (-1, -1) — путь не задан
B (0018): (93, 125) — игрок проложил путь к точке (93, 125)

Структура около этого поля (0x1636b2):
  Контекст в дампе B:
    0x1636a0: ... ff ff ff ff ff ff ff ff
    0x1636a8: ... 
    0x1636b0: ... [padding 2 байта]
    0x1636b2: 5d 00 00 00 7d 00 00 00  ← X=93, Y=125 (НОВОЕ ЗНАЧЕНИЕ)
    0x1636ba: ff ff ff ff ff ff ff ff  ← следующий слот (пустой)

Похоже, это массив из нескольких точек пути (waypoints):
  Каждая точка = 8 байт (2 × u32 LE)
  Массив может содержать до N точек (multi-tile path)
  Пустые слоты = 0xFFFFFFFF 0xFFFFFFFF

Это позволяет хранить ПРОМЕЖУТОЧНЫЕ ТОЧКИ пути, а не только конечную.
""")

# Проверим — есть ли другие занятые слоты рядом (вдруг путь из 2+ точек)
print("\n--- Расширенный дамп 0x163680..0x163720 (возможный массив waypoints) ---")
print("A:")
print(hex_dump(A, 0x163680, 0x163720))
print("B:")
print(hex_dump(B, 0x163680, 0x163720))

# Проверим количество занятых слотов в массиве
print("\n--- Анализ массива waypoints в B ---")
for offset in range(0x163680, 0x163720, 8):
    x = int.from_bytes(B[offset:offset+4], 'little')
    y = int.from_bytes(B[offset+4:offset+8], 'little')
    is_empty = (x == 0xFFFFFFFF and y == 0xFFFFFFFF)
    marker = " ← ЗАНЯТ" if not is_empty else ""
    if not is_empty or offset in (0x1636b2,):
        print(f"  0x{offset:08x}: X=0x{x:08x}, Y=0x{y:08x}{marker}")
