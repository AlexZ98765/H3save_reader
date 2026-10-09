#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Фокус-анализ: что именно изменилось у героя при получении опыта.

Главные кластеры:
  - 0x6d7a4 / 0x6d7aa: hero stats (movement?)
  - 0x157ec3: hero coordinates
  - 0x157f00..0x157f14: hero fields (level/exp/skills)
  - 0x157fa4 / 0x157fc0 / 0x157fcd: secondary skills
  - 0x170d31..0x1717xx: огромный массив secondary skills всех героев
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

A = load('/home/z/my-project/upload/0012.GM1')
B = load('/home/z/my-project/upload/0013.GM1')

def hex_dump(data, start, end, prefix="    "):
    chunk = data[start:end]
    lines = []
    for i in range(0, len(chunk), 16):
        s = chunk[i:i+16]
        hex_part = ' '.join(f'{b:02x}' for b in s)
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in s)
        lines.append(f"{prefix}{start+i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return '\n'.join(lines)

# === КЛАСТЕР: 0x157ec3 (hero position + something else) ===
print("=" * 80)
print("КЛАСТЕР: 0x157ec3 — позиция героя + его состояние")
print("=" * 80)
print("\nA (0012, до движения и получения опыта):")
print(hex_dump(A, 0x157ea0, 0x158000))
print("\nB (0013, после движения, посещения объекта, получения 1000 XP, уровня, навыка):")
print(hex_dump(B, 0x157ea0, 0x158000))

# === КЛАСТЕР: 0x6d7a4 (hero stats — experience/level?) ===
print("\n" + "=" * 80)
print("КЛАСТЕР: 0x6d7a4 — hero stats block (experience? level?)")
print("=" * 80)
print("\nA:")
print(hex_dump(A, 0x6d780, 0x6d7c0))
print("\nB:")
print(hex_dump(B, 0x6d780, 0x6d7c0))

# Извлечём конкретные значения
print("\n--- Интерпретация ---")
print("0x6d7a4 (1b): A=0x64 (100), B=0x22 (34)  ← движение героя (movement points)")
print("0x6d7aa (1b): A=0x08 (8),   B=0x58 (88)  ← это опыт / level?")

# u32 @ 0x6d7a8: experience?
exp_a = int.from_bytes(A[0x6d7a8:0x6d7ac], 'little')
exp_b = int.from_bytes(B[0x6d7a8:0x6d7ac], 'little')
print(f"\nu32 LE @ 0x6d7a8: A=0x{exp_a:08x} ({exp_a}), B=0x{exp_b:08x} ({exp_b}), delta={exp_b-exp_a}")

# u32 @ 0x6d7a4
val_a = int.from_bytes(A[0x6d7a4:0x6d7a8], 'little')
val_b = int.from_bytes(B[0x6d7a4:0x6d7a8], 'little')
print(f"u32 LE @ 0x6d7a4: A=0x{val_a:08x} ({val_a}), B=0x{val_b:08x} ({val_b}), delta={val_b-val_a}")

# === КЛАСТЕР: 0x157f00 — большой блок состояния героя ===
print("\n" + "=" * 80)
print("КЛАСТЕР: 0x157f00..0x157f14 — hero current-state block")
print("=" * 80)
print("\nA:")
print(hex_dump(A, 0x157ef0, 0x158020))
print("\nB:")
print(hex_dump(B, 0x157ef0, 0x158020))

print("\n--- Интерпретация ---")
print("Изменения:")
print("  0x157f00: 1b 07 00 00 → 80 03 00 00 (u32: 0x71b=1819 → 0x380=896, delta=-923)")
print("  0x157f04: 30 00 00 00 → 18 04 00 00 (u32: 0x30=48 → 0x418=1048, delta=+1000) ← ОПЫТ!")
print("  0x157f08: 02 → 03 (1 byte, +1) ← УРОВЕНЬ!")
print("  0x157f0e: 01 00 00 00 → 02 00 00 00 (u32: 1→2) ← skill count?")
print("  0x157f12: 00 00 → 00 01 (новый навык появился, ID=1, level=0?)")

# === КЛАСТЕР: 0x157fa4, 0x157fc0, 0x157fcd — навыки героя ===
print("\n" + "=" * 80)
print("КЛАСТЕР: 0x157fa4 / 0x157fc0 / 0x157fcd — secondary skills of this hero")
print("=" * 80)
print("\nA:")
print(hex_dump(A, 0x157f90, 0x158010))
print("\nB:")
print(hex_dump(B, 0x157f90, 0x158010))

print("\n--- Интерпретация ---")
print("0x157fa4 (1b): A=0x00, B=0x01  ← НОВЫЙ навык добавлен (skill ID?)")
print("0x157fc0 (1b): A=0x00, B=0x03  ← skill level = 3? или skill ID?")
print("0x157fcd (1b): A=0x03, B=0x04  ← counter+1")

# === КЛАСТЕР: 0x171xxx — большая таблица secondary skills всех героев ===
print("\n" + "=" * 80)
print("КЛАСТЕР: 0x171xxx — таблица secondary skills всех героев")
print("=" * 80)

# Найдём stride — расстояние между изменениями
changes_in_table = [
    (0x170d31, 0x170d32),  # 1b: 0x13 → 0x1b (+8)
    (0x170e4f, 0x170e52),  # 3b: 13 00 13 → 1b 00 1b (+8 per byte)
    (0x170f6d, 0x170f72),  # 5b: 93 00 13 00 13 → 9b 00 1b 00 1b
    (0x17108d, 0x171092),  # 5b: same
    (0x1711ad, 0x1711b2),  # 5b: same
    (0x1712cd, 0x1712d2),  # 5b: 91 00 11 00 11 → 99 00 19 00 19
    (0x1713ed, 0x1713f4),  # 7b
    (0x17150d, 0x171516),  # 9b
    (0x17162f, 0x171638),  # 9b
]

# Расстояния между началами изменений
strides = []
for i in range(1, len(changes_in_table)):
    s = changes_in_table[i][0] - changes_in_table[i-1][0]
    strides.append(s)
print(f"\nStride между началами изменений: {strides}")
print(f"  Средний stride: {sum(strides)/len(strides):.1f}")
print(f"  Все stride: {[hex(s) for s in strides]}")
print(f"  → Размер записи одного героя: 0x120 = {0x120} = 288 байт")

# === Финальная сводка ===
print("\n" + "=" * 80)
print("ИТОГОВАЯ СВОДКА — ГЕРОЙ ПОЛУЧИЛ 1000 XP, НОВЫЙ УРОВЕНЬ, НАВЫК")
print("=" * 80)
print("""
ГЛАВНЫЕ НАХОДКИ:

1. ⭐ EXPERIENCE (опыт героя)
   Смещение: 0x157f04 (относительно начала блока состояния героя)
   Формат: u32 LE
   Значение: 48 → 1048 (delta = +1000) ✅
   Точное место в файле: внутри hero current-state block (~0x157f00)

2. ⭐ LEVEL (уровень героя)
   Смещение: 0x157f08 (1 байт после опыта)
   Формат: u8
   Значение: 2 → 3 (delta = +1) ✅

3. ⭐ SECONDARY SKILL COUNT
   Смещение: 0x157f0e
   Формат: u32 LE
   Значение: 1 → 2 (появился новый навык) ✅

4. ⭐ NEW SECONDARY SKILL
   Смещение: 0x157f12
   Формат: u16 (skill ID + level?)
   Значение: 0x0000 → 0x0100 (новый навык ID=0, level=1? или наоборот)

5. ⭐ SECONDARY SKILLS TABLE (для всех героев)
   Начало: ~0x170d00
   Размер записи на героя: 288 байт (0x120)
   Каждая запись содержит массив из ~28 навыков (по 2 байта: skill_id + level)
   Изменились 9 байт у одного героя — все +8 (0x13→0x1b, 0x11→0x19, 0x91→0x99, 0x93→0x9b)
   Это значит: битовая маска secondary skills сдвинулась на 1 (логический сдвиг влево)

6. ⭐ MOVEMENT POINTS (поход героя)
   Смещение: 0x6d7a4 (u8: 100 → 34)
   Смещение: 0x6d7aa (u8: 8 → 88)
   Это "current movement" героя — уменьшилось после перемещения

7. ⭐ HERO POSITION
   Смещение: 0x157ec3 (UTF-16 LE строки X,Y)
   A: X='4', Y='a' (4, 10) → B: X='2', Y='f' (2, 15)
   Герой сдвинулся на (-2, +5)

8. ⭐ PATH BLOCK
   Адрес: ~0x1814d0..0x1815d0
   Добавлены новые path-записи (формат восстановлен в предыдущем анализе)

ЛОКАЛИЗОВАННЫЕ СМЕЩЕНИЯ ГЕРОЯ:
  Hero stats block (movement, exp, level):    ~0x6d7a4..0x6d7ac
  Hero current-state block (skills, exp):     ~0x157f00..0x158000
  Hero coordinates (UTF-16 X,Y):              ~0x157ec3
  Hero secondary skills table (all heroes):   ~0x170d00..0x171800
    stride = 288 bytes per hero
""")
