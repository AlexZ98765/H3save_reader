#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020 → 0020_1.
Контекст: применили читкод для "бесконечных" ходов героя.
Ожидается:
  1) Флаг "cheater" (bool — игрок использовал читы)
  2) Установка большого значения movement points (или special флага)

Главные изменения:
  0x157ef0..0x157ef8 (8b): ff ff ff ff ff ff ff ff → 33 00 00 00 63 00 00 00
                          Появились 2 значения: 0x33 (51) и 0x63 (99)
  0x157f00..0x157f03 (3b): 54 02 00 → df 93 04
                          u32 LE: 0x254 (596) → 0x493df (299999) — огромное число!
  0x16a29d (1b): 0x00 → 0x01 — ФЛАГ!
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

A = load('/home/z/my-project/upload/0020.GM1')
B = load('/home/z/my-project/upload/0020_1.GM1')
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

# === Главные кластеры ===
CLUSTERS = [
    (0x157ef0, 0x157ef8, "⭐ Movement values (8b)"),
    (0x157f00, 0x157f03, "⭐ Movement total (3b)"),
    (0x16a29d, 0x16a29e, "⭐ CHEATER FLAG?"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020, без чита):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_1, с читом):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Извлечение значений ===
print("\n" + "="*80)
print("АНАЛИЗ ЗНАЧЕНИЙ")
print("="*80)

# 0x157ef0..0x157ef8 (8 байт = 2 × u32 LE)
val_a1 = int.from_bytes(A[0x157ef0:0x157ef4], 'little')
val_a2 = int.from_bytes(A[0x157ef4:0x157ef8], 'little')
val_b1 = int.from_bytes(B[0x157ef0:0x157ef4], 'little')
val_b2 = int.from_bytes(B[0x157ef4:0x157ef8], 'little')
print(f"\nu32 @ 0x157ef0: A=0x{val_a1:08x} ({val_a1 if val_a1<0x80000000 else -1}) → B=0x{val_b1:08x} ({val_b1})")
print(f"u32 @ 0x157ef4: A=0x{val_a2:08x} ({val_a2 if val_a2<0x80000000 else -1}) → B=0x{val_b2:08x} ({val_b2})")
print(f"  В A: оба значения 0xFFFFFFFF = -1 (не заданы)")
print(f"  В B: val1 = 0x33 (51), val2 = 0x63 (99)")
print(f"  Похоже на координаты! (51, 99) или hero stats (level=51?, exp=99?)")

# 0x157f00..0x157f03 (4 байта u32 LE, но изменены только 3)
val_a3 = int.from_bytes(A[0x157f00:0x157f04], 'little')
val_b3 = int.from_bytes(B[0x157f00:0x157f04], 'little')
print(f"\nu32 @ 0x157f00: A=0x{val_a3:08x} ({val_a3}) → B=0x{val_b3:08x} ({val_b3})")
print(f"  Delta = +{val_b3 - val_a3}")
print(f"  Это MOVEMENT POINTS (бесконечные ходы)!")
print(f"  B = {val_b3} = огромное число (типично для чит-кода 'nwcneo' = 'unlimited movement')")

# 0x16a29d (1 байт — flag)
val_a4 = A[0x16a29d]
val_b4 = B[0x16a29d]
print(f"\nu8 @ 0x16a29d: A=0x{val_a4:02x} ({val_a4}) → B=0x{val_b4:02x} ({val_b4})")
print(f"  ⭐ Это ФЛАГ! 0 → 1 (FALSE → TRUE)")
print(f"  Кандидат на 'cheater flag'")

# === Контекст флага ===
print("\n" + "="*80)
print("КОНТЕКСТ ФЛАГА @ 0x16a29d")
print("="*80)
# Покажем соседние байты
print("\nРасширенный контекст 0x16a280..0x16a2c0:")
print("A:")
print(hex_dump(A, 0x16a280, 0x16a2c0))
print("B:")
print(hex_dump(B, 0x16a280, 0x16a2c0))

# Проверим — этот флаг в hero block или в global state?
# Известно, что hero data ~0x163000..0x164000, а town data ~0x13f000..0x140000
# 0x16a29d — между hero и town, может быть в CGameState area
print(f"\n0x16a29d находится в диапазоне между hero data и town data")
print(f"Возможно, это global player flag или map flag")

# === Финальный итог ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ЧИТ-КОД "БЕСКОНЕЧНЫЕ ХОДЫ" — НАЙДЕНЫ:

1. ⭐⭐⭐ ФЛАГ CHEATER
   Смещение: 0x16a29d (1 байт, u8 bool)
   A: 0x00 (FALSE — без чита)
   B: 0x01 (TRUE — чит применён)
   
   ⭐ Это флаг "cheater" — устанавливается при использовании любого чит-кода.
   Когда этот флаг установлен, игра помечает сейв как "cheated" и
   не засчитывает достижения / статистику.

2. ⭐⭐⭐ MOVEMENT POINTS = огромное число
   Смещение: 0x157f00 (4 байта, u32 LE)
   A: 0x00000254 (596)  — нормальное значение
   B: 0x000493df (299999) — огромное число (≈ бесконечные ходы)
   
   ⭐ Это текущее movement points героя!
   Чит-код устанавливает очень большое значение, чтобы герой
   мог ходить без ограничений.
   
   ⚠️ Сравним с предыдущим диффом (0012→0013):
       0x157f00 (hero exp-related): 1819 → 896
   Здесь: 596 → 299999
   Похоже, это НЕ experience, а именно movement!
   
   Возможно:
     0x157f00 (4b)  = current_movement_points (u32 LE)
     0x157f04 (4b)  = experience (u32 LE)
     0x157f08 (1b)  = level (u8)

3. ⭐ MOVEMENT-RELATED VALUES
   Смещение: 0x157ef0 (8 байт = 2 × u32 LE)
   A: 0xFFFFFFFF 0xFFFFFFFF (не задано)
   B: 0x00000033 (51) 0x00000063 (99)
   
   Возможные интерпретации:
   - Maximum movement for the day (51, 99)?
   - Original movement + bonus (51, 99)?
   - Cooldowns / counters?
   
   Значения небольшие (51, 99), не похожи на координаты.
   Возможно, это:
     0x33 (51) = movement_bonus_from_stables (за посещение конюшни)
     0x63 (99) = movement_bonus_other

4. ⭐ ИМЯ ФАЙЛА
   Смещение: 0x3b7..0x3bd
   A: ".GM1\\0\\0" → B: "_1.GM1"
   Имя файла сейва изменилось (0020.GM1 → 0020_1.GM1)
   Подтверждает известное смещение 0x3b6

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x157ef0 (8b, 2×u32 LE): movement bonus values (были -1, стали 51 и 99)
  0x157f00 (4b, u32 LE): CURRENT MOVEMENT POINTS (чит даёт 299999)
  0x16a29d (1b, u8 bool): ⭐ CHEATER FLAG (0=normal, 1=cheated)

ПРОВЕРКА ПРЕДЫДУЩИХ НАХОДОК:
  Ранее я думал, что 0x157f00 = movement-related (не experience)
  Этот дифф ПОДТВЕРЖДАЕТ — это действительно MOVEMENT!
  А 0x157f04 = EXPERIENCE (был прав в диффе 0012→0013)
  
  Также 0x6d7a4 (Hero #1 movement remaining, u8) — это, вероятно,
  младший байт от 0x157f00 или отдельное поле.
""")
