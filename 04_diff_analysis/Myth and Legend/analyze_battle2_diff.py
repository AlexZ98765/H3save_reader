#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_8 → 0020_9.
Контекст: 
  - Напал на героя врага
  - Враг проиграл и сбежал
  - Герой получил 2 уровня опыта → 2 навыка (1 новый + 1 улучшение)

Главные находки:
  0x05c7b7: hero visit flag (10 22 → 00 00)
  0x05c7be: u8 0x41 (65) → 0x00 — counter reset
  0x13e2e6..0x13e2f1: ⭐ enemy hero state (враг сбежал)
  0x140b22..0x140c03: ⭐⭐ Hero stats (movement, level, experience, skills!)
  0x151c7f..0x151d39: ⭐⭐⭐ ENEMY HERO DEFEATED (большой блок изменений)
  0x16a19c / 0x16a1b4: artifact movement
  0x1819a7: ⭐ path-запись типа 08 03 (battle)
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

A = load('/home/z/my-project/upload/0020_8.GM1')
B = load('/home/z/my-project/upload/0020_9.GM1')
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
    (0x05c7b7, 0x05c7bf, "Hero visit flag"),
    (0x13e2e6, 0x13e2f1, "⭐⭐ ENEMY HERO state (fled!)"),
    (0x140b22, 0x140c03, "⭐⭐⭐ HERO STATS (level +2, exp +N, skills)"),
    (0x151c7f, 0x151d39, "⭐⭐⭐ ENEMY HERO DEFEATED (large block)"),
    (0x16a19c, 0x16a1b5, "⭐ Artifact transfer"),
    (0x1819a7, 0x1819b0, "⭐ Path record (08 03 battle)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_8, до битвы):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_9, после победы):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ HERO STATS ===
print("\n" + "="*80)
print("⭐⭐⭐ АНАЛИЗ HERO STATS (уровни + опыт + навыки)")
print("="*80)

# 0x140b22: u8 0x07 → 0x05
print(f"u8 @ 0x140b22: A=0x{A[0x140b22]:02x} ({A[0x140b22]}) → B=0x{B[0x140b22]:02x} ({B[0x140b22]})")

# 0x140b30: u32 LE
val_a = int.from_bytes(A[0x140b30:0x140b34], 'little')
val_b = int.from_bytes(B[0x140b30:0x140b34], 'little')
print(f"u32 LE @ 0x140b30: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  ⭐ Счётчик: 2 → 4 (delta=+2) — это может быть 2 новых уровня!")

# 0x140b34: u32 LE
val_a = int.from_bytes(A[0x140b34:0x140b38], 'little')
val_b = int.from_bytes(B[0x140b34:0x140b38], 'little')
print(f"u32 LE @ 0x140b34: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  Delta = {val_b - val_a} — это может быть EXPERIENCE!")

# 0x140b38: u32 LE
val_a = int.from_bytes(A[0x140b38:0x140b3c], 'little')
val_b = int.from_bytes(B[0x140b38:0x140b3c], 'little')
print(f"u32 LE @ 0x140b38: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  Delta = {val_b - val_a} — movement related?")

# 0x140b3c: u32 LE
val_a = int.from_bytes(A[0x140b3c:0x140b40], 'little')
val_b = int.from_bytes(B[0x140b3c:0x140b40], 'little')
print(f"u32 LE @ 0x140b3c: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")
print(f"  Delta = {val_b - val_a}")

# 0x140b44: u8
print(f"\nu8 @ 0x140b44: A=0x{A[0x140b44]:02x} ({A[0x140b44]}) → B=0x{B[0x140b44]:02x} ({B[0x140b44]})")
print(f"  ⭐ 0x02 → 0x04 (delta=+2) — это LEVEL UP на 2 уровня!")

# 0x140bcb: u8
print(f"u8 @ 0x140bcb: A=0x{A[0x140bcb]:02x} → B=0x{B[0x140bcb]:02x} — visit flag")

# 0x140bd9: u8
print(f"u8 @ 0x140bd9: A=0x{A[0x140bd9]:02x} ({A[0x140bd9]}) → B=0x{B[0x140bd9]:02x} ({B[0x140bd9]})")
print(f"  0x01 → 0x02 (delta=+1) — skill count +1 (новый навык)!")

# 0x140be7: u8
print(f"u8 @ 0x140be7: A=0x{A[0x140be7]:02x} ({A[0x140be7]}) → B=0x{B[0x140be7]:02x} ({B[0x140be7]})")
print(f"  0x00 → 0x04 — improvement flag?")

# 0x140c01: u8 ×2
print(f"u8 @ 0x140c01: A=0x{A[0x140c01]:02x} → B=0x{B[0x140c01]:02x}")
print(f"u8 @ 0x140c02: A=0x{A[0x140c02]:02x} → B=0x{B[0x140c02]:02x}")
print(f"  0x02 0x02 → 0x03 0x03 (delta=+1+1) — это может быть SKILL LEVEL +1!")

# === Анализ ENEMY HERO DEFEATED ===
print("\n" + "="*80)
print("⭐⭐⭐ АНАЛИЗ ENEMY HERO (ПРОИГРАВШИЙ, СБЕЖАЛ)")
print("="*80)

# 0x13e2e6: u8 0x06 → 0x05
print(f"u8 @ 0x13e2e6: A=0x{A[0x13e2e6]:02x} ({A[0x13e2e6]}) → B=0x{B[0x13e2e6]:02x} ({B[0x13e2e6]})")
print(f"  ⭐ Enemy hero state: 6 (active) → 5 (defeated/fled)")

# 0x13e2ed..0x13e2f1 (4 bytes)
val_a = int.from_bytes(A[0x13e2ed:0x13e2f1], 'little')
val_b = int.from_bytes(B[0x13e2ed:0x13e2f1], 'little')
print(f"\nu32 LE @ 0x13e2ed: A=0x{val_a:08x} → B=0x{val_b:08x}")
print(f"  A: 0x59FFFF41 (encoded) → B: 0x41FFFFFF")
print(f"  ⭐ Байты 'переместились' — это swap значений!")
print(f"  Возможно: '41' = enemy_id, swap to indicate defeat")

# 0x13e409: u8 0x03 → 0x01
print(f"\nu8 @ 0x13e409: A=0x{A[0x13e409]:02x} ({A[0x13e409]}) → B=0x{B[0x13e409]:02x} ({B[0x13e409]})")
print(f"  Action flag изменился")

# === Большой блок 0x151c7f..0x151d39 (вражеский герой) ===
print("\n" + "="*80)
print("⭐⭐⭐ БОЛЬШОЙ БЛОК ВРАГА (0x151c7f..0x151d39)")
print("="*80)

# 0x151c7f: u8 0x01 → 0x00
print(f"u8 @ 0x151c7f: A=0x{A[0x151c7f]:02x} → B=0x{B[0x151c7f]:02x}")
print(f"  ⭐ 1 → 0 — hero defeated flag (был активен, теперь побеждён)")

# 0x151c93..0x151c97: 4 bytes
print(f"\nBytes @ 0x151c93: A={A[0x151c93:0x151c97].hex()} → B={B[0x151c93:0x151c97].hex()}")
print(f"  A: 01 ff 01 01 → B: ff ff 00 00")
print(f"  ⭐ Enemy army slots обнулены (враг потерял армию)")

# 0x151ca6: u32 (visiting coords)
val1_a = int.from_bytes(A[0x151ca6:0x151caa], 'little')
val2_a = int.from_bytes(A[0x151caa:0x151cae], 'little')
val1_b = int.from_bytes(B[0x151ca6:0x151caa], 'little')
val2_b = int.from_bytes(B[0x151caa:0x151cae], 'little')
print(f"\nu32 @ 0x151ca6: A=({val1_a}, {val2_a}) → B=({val1_b if val1_b<0x80000000 else -1}, {val2_b if val2_b<0x80000000 else -1})")
print(f"  ⭐ Enemy visiting coords сброшены в (-1, -1) — враг изгнан")

# 0x151cc2: u8
print(f"\nu8 @ 0x151cc2: A=0x{A[0x151cc2]:02x} ({A[0x151cc2]}) → B=0x{B[0x151cc2]:02x} ({B[0x151cc2]})")
print(f"  ⭐ 0x07 → 0x02 — hero state changed (был активен=7, теперь изгнан=2)")

# 0x151d02..0x151d2d (43 bytes — большой блок!)
print(f"\nБлок 43 bytes @ 0x151d02..0x151d2d:")
print(f"  A: {A[0x151d02:0x151d2d].hex()}")
print(f"  B: {B[0x151d02:0x151d2d].hex()}")
# Декодируем A
print(f"\n  Декодировка A (враг до битвы):")
for i in range(0, 43, 4):
    if 0x151d02 + i + 4 <= 0x151d2d:
        val = int.from_bytes(A[0x151d02+i:0x151d02+i+4], 'little')
        print(f"    +0x{i:02x}: {val}")

# 0x151d34..0x151d39
print(f"\nBytes @ 0x151d34: A={A[0x151d34:0x151d39].hex()} → B={B[0x151d34:0x151d39].hex()}")

# === Path-запись ===
print("\n" + "="*80)
print("⭐ PATH-ЗАПИСЬ типа 08 03 (battle with enemy hero)")
print("="*80)
rec = B[0x1819a7:0x1819a7+9]
print(f"Record @ 0x1819a7: {rec.hex()}")
print(f"  flag1={rec[0]:02x} (тип 08 = battle/transfer)")
print(f"  flag2={rec[1]:02x}")
print(f"  u32 counter = {int.from_bytes(rec[2:6],'little')} (0x{int.from_bytes(rec[2:6],'little'):x})")
print(f"  u8 @ +6 = 0x{rec[6]:02x}")
print(f"  u8 @ +7 = 0x{rec[7]:02x} (результат? 01 = победа)")
print(f"  u8 @ +8 = 0x{rec[8]:02x} (0x0b = terminator)")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ БИТВА С ВРАГОМ + ПОБЕДА + 2 УРОВНЯ — НАЙДЕНЫ:

1. ⭐⭐⭐ HERO LEVEL UP (+2)
   Смещение: 0x140b44 (u8)
   A: 0x02 → B: 0x04 (delta=+2)
   ⭐ Герой получил 2 уровня (как и сказал игрок)!

2. ⭐⭐⭐ SKILL COUNT (+1)
   Смещение: 0x140bd9 (u8)
   A: 0x01 → B: 0x02 (delta=+1)
   ⭐ Добавлен 1 новый навык!

3. ⭐⭐⭐ SKILL LEVEL UP (+1)
   Смещение: 0x140c01..0x140c02 (2 × u8)
   A: 0x02 0x02 → B: 0x03 0x03 (delta=+1+1)
   ⭐ Существующий навык улучшен (level +1)!
   Возможно 2 байта = skill_id + skill_level

4. ⭐⭐⭐ EXPERIENCE GAINED
   Смещение: 0x140b34 (u32 LE)
   A: 0x049261 → B: 0x0491d4
   ⚠️ Опыт УМЕНЬШИЛСЯ? Это странно — возможно это movement
   или другая метрика. Скорее всего, это не опыт.

5. ⭐⭐⭐ COUNTER +2
   Смещение: 0x140b30 (u32 LE)
   A: 0x02 → B: 0x04 (delta=+2)
   ⭐ Это может быть:
     - количество уровней, полученных за битву
     - счётчик побед
     - skill points remaining

6. ⭐⭐⭐ ENEMY HERO STATE CHANGED
   Смещение: 0x13e2e6 (u8)
   A: 0x06 → B: 0x05
   ⭐ Enemy hero state: 6 (active) → 5 (defeated)
   
   Смещение: 0x13e2ed (4b)
   A: 0x59FFFF41 → B: 0x41FFFFFF
   ⭐ Swap значений — враг изгнан
   
7. ⭐⭐⭐ ENEMY HERO BLOCK (большой блок 0x151c7f..0x151d39)
   0x151c7f: 0x01 → 0x00 — hero defeated flag
   0x151c93..0x151c97: 01 ff 01 01 → ff ff 00 00 — army slots cleared
   0x151ca6..0x151cae: (100, 83) → (-1, -1) — visiting coords сброшены
   0x151cc2: 0x07 → 0x02 — hero state changed (active=7 → banished=2)
   0x151d02..0x151d2d (43b): ⭐ большой блок — army + stats обнулены
   
   ⭐⭐ Это полная очистка вражеского героя после побега!
   
8. ⭐ PATH-ЗАПИСЬ типа 08 03 (battle event)
   Размер: 9 байт (компактный формат)
   counter = 0x41 (65) — visit_id битвы
   flag = 0x01 — результат (1 = победа)
   terminator = 0x0b
   
   ⭐ Это новый компактный формат path-записи 08 03!
   (раньше видели 27-байтный вариант для army transfer)

9. ⭐ ARTIFACT TRANSFER
   0x16a19c: 0x01 → 0x40 — артефакт появился
   0x16a1b4: 0x40 → 0xff — артефакт исчез
   ⭐ Артефакт 0x40 (64) перешёл от врага к победителю

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x140b30: hero level counter (u32 LE) — +2 за битву
  0x140b44: ⭐⭐ HERO LEVEL (u8) — +2 за битву
  0x140bd9: ⭐⭐ SKILL COUNT (u8) — +1 за битву (новый навык)
  0x140be7: skill improvement flag (u8)
  0x140c01..0x140c02: ⭐⭐ SKILL LEVEL (2 × u8) — +1 за битву (улучшение)
  0x13e2e6: ⭐ ENEMY HERO STATE (u8, 6=active, 5=defeated)
  0x13e2ed: enemy hero swap field (4b)
  0x151c7f: enemy hero defeated flag (u8)
  0x151c93..0x151c97: enemy army slots (4b)
  0x151ca6..0x151cae: enemy visiting coords (2 × u32 LE)
  0x151cc2: enemy hero state (u8, 7=active, 2=banished)
  0x151d02..0x151d2d: enemy hero big block (43b)
  0x16a19c: artifact slot (winner side)
  0x16a1b4: artifact slot (loser side)

PATH-ЗАПИСЬ типа "08 03" (9b компактный):
  u8  flag1   = 0x08
  u8  flag2   = 0x03
  u32 counter = visit_id битвы
  u8  result  = 0x01 (победа)
  u8  terminator = 0x0b
  
  ⭐ Это BATTLE EVENT в компактной форме (без вложенных sub-records)
""")
