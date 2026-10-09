#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Анализ 312.GM1 → 312_1.GM1.
Контекст: 
  - Герой посещает лагерь наёмников (Mercenary Camp) → +1 атака (разовый бонус)
  - День 3-1-2 (день 3, неделя 1, месяц 2)
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

A = load('/home/z/my-project/upload/312.GM1')
B = load('/home/z/my-project/upload/312_1.GM1')
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

# Главные кластеры
CLUSTERS = [
    (0x0651b5, 0x065239, "⭐ Hero visit flags + movement"),
    (0x157f00, 0x157f70, "⭐⭐ Hero stats: experience, level, attack?!"),
    (0x15800c, 0x15800d, "⭐ New flag (visited mercenary camp?)"),
    (0x17ec78, 0x17ec79, "Map event counter / day counter?"),
    (0x1835cd, 0x18360a, "⭐ Path records (movement to camp)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (312.GM1, до лагеря наёмников):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (312_1.GM1, после лагеря +attack):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Детальный анализ hero stats ===
print("\n" + "="*80)
print("⭐⭐⭐ ДЕТАЛЬНЫЙ АНАЛИЗ HERO STATS")
print("="*80)

# Experience (u32 LE @ 0x157F04)
exp_a = int.from_bytes(A[0x157F04:0x157F08], 'little')
exp_b = int.from_bytes(B[0x157F04:0x157F08], 'little')
print(f"\nExperience @ 0x157F04 (u32 LE):")
print(f"  A = {exp_a} (0x{exp_a:08x})")
print(f"  B = {exp_b} (0x{exp_b:08x})")
print(f"  Delta = {exp_b - exp_a}")

# Level (u8 @ 0x157F08)
print(f"\nLevel @ 0x157F08 (u8):")
print(f"  A = {A[0x157F08]}")
print(f"  B = {B[0x157F08]}")

# Check bytes around 0x157F0B (10 bytes changed)
print(f"\nBytes @ 0x157F0B (10 bytes changed):")
print(f"  A: {A[0x157F0B:0x157F15].hex()}")
print(f"  B: {B[0x157F0B:0x157F15].hex()}")

# Decode as u32 LE pairs
for off in range(0x157F0B, 0x157F15, 4):
    if off + 4 <= 0x157F15:
        va = int.from_bytes(A[off:off+4], 'little')
        vb = int.from_bytes(B[off:off+4], 'little')
        print(f"  u32 @ 0x{off:08x}: A=0x{va:08x} ({va}) → B=0x{vb:08x} ({vb}), delta={vb-va}")

# Check 0x157F2D (1 byte: 0x07 → 0x01)
print(f"\n⭐ u8 @ 0x157F2D: A=0x{A[0x157F2D]:02x} ({A[0x157F2D]}) → B=0x{B[0x157F2D]:02x} ({B[0x157F2D]})")
print(f"  Delta = {B[0x157F2D] - A[0x157F2D]}")
print(f"  ⚠️ Уменьшение на 6 — это НЕ +1 атака")

# Check 0x157F41 (2 bytes: 0xf0 0x01 → 0x00 0x00)
va = int.from_bytes(A[0x157F41:0x157F43], 'little')
vb = int.from_bytes(B[0x157F41:0x157F43], 'little')
print(f"\nu16 LE @ 0x157F41: A=0x{va:04x} ({va}) → B=0x{vb:04x} ({vb})")
print(f"  A=496 → B=0 — обнуление (возможно, movement leftover)")

# Check 0x157F5F (1 byte: 0x00 → 0x20)
print(f"\n⭐ u8 @ 0x157F5F: A=0x{A[0x157F5F]:02x} → B=0x{B[0x157F5F]:02x}")
print(f"  0x20 = бит 5 установлен")
print(f"  Это может быть флаг 'visited Mercenary Camp' (permanent bonus)!")

# Check 0x15800C (1 byte: 0x00 → 0x01)
print(f"\n⭐ u8 @ 0x15800C: A=0x{A[0x15800C]:02x} → B=0x{B[0x15800C]:02x}")
print(f"  0x00 → 0x01 — НОВЫЙ флаг установлен")
print(f"  Это тоже может быть 'visited Mercenary Camp'!")

# Check 0x17EC78 (1 byte: 0x00 → 0x04)
print(f"\nu8 @ 0x17EC78: A=0x{A[0x17EC78]:02x} → B=0x{B[0x17EC78]:02x}")
print(f"  0x00 → 0x04 — счётчик +4 (возможно, day/week/month)")

# === Поиск ATTACK STAT ===
print("\n" + "="*80)
print("⭐ ПОИСК ATTACK STAT (+1 от Mercenary Camp)")
print("="*80)

# В h3sed primary stats (attack/defense/power/knowledge) — это 4 байта
# Они должны быть где-то в hero block
# Проверим все однобайтовые изменения в диапазоне 0x157F00..0x158020
print("\nВсе изменённые байты в hero block (0x157F00..0x158020):")
for addr in range(0x157F00, 0x158020):
    if A[addr] != B[addr]:
        delta = B[addr] - A[addr]
        print(f"  0x{addr:08x}: A=0x{A[addr]:02x} ({A[addr]}) → B=0x{B[addr]:02x} ({B[addr]}), delta={delta:+d}")

# === Анализ path-записей ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ PATH-ЗАПИСЕЙ")
print("="*80)

# Декодируем path-записи из B @ 0x1835CD
offset = 0x1835CD
for i in range(5):
    if B[offset:offset+2] != b'\x01\x03':
        # Check if it's 0b 03 (fog of war)
        if B[offset:offset+2] == b'\x0b\x03':
            n = B[offset+2]
            size = 4 + n * 8
            print(f"  [{i+1}] 0b 03 (fog of war) @ 0x{offset:08x}, N={n}, size={size}")
            offset += size
            continue
        break
    rec = B[offset:offset+15]
    counter = int.from_bytes(rec[2:6], 'little')
    n = rec[6]
    fx, fy = int.from_bytes(rec[7:9],'little'), int.from_bytes(rec[9:11],'little')
    tx, ty = int.from_bytes(rec[11:13],'little'), int.from_bytes(rec[13:15],'little')
    print(f"  [{i+1}] 01 03 @ 0x{offset:08x}: counter={counter}, N={n}, ({fx},{fy})→({tx},{ty})  delta=({tx-fx:+d},{ty-fy:+d})")
    offset += 15

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ЛАГЕРЬ НАЁМНИКОВ (MERCENARY CAMP) + СМЕНА ДНЯ

1. ⭐ EXPERIENCE изменился
   Смещение: 0x157F04 (u32 LE)
   A = 0x005e3000  →  B = 0x005e3400
   Delta = +1024 (0x400)
   ⚠️ 1024 XP — это опыт за ход (суточный доход?) или за посещение?

2. ⭐⭐ НОВЫЕ ФЛАГИ (кандидаты на "visited Mercenary Camp"):
   
   a) 0x157F5F: 0x00 → 0x20 (бит 5)
      — может быть флагом "Mercenary Camp visited" (permanent +1 attack)
   
   b) 0x15800C: 0x00 → 0x01
      — может быть флагом "Mercenary Camp visited" (alternative)
   
   Оба установились одновременно — один из них (или оба) —
   флаг посещения лагеря наёмников.

3. ⭐ ATTACK STAT — где +1?
   Не найдено прямого +1 к attack в проверенных смещениях.
   0x157F2D: 0x07 → 0x01 (delta=-6) — это НЕ атака
   
   Возможно attack хранится в другом месте:
   - В h3sed primary stats (attack/defense/power/knowledge) — 4 отдельных байта
   - Нужно проверить полный hero block на предмет +1 к какому-то байту
   
   ⚠️ Ни один байт в диапазоне 0x157F00..0x158020 не изменился на +1!
   Возможно attack хранится ВНЕ этого диапазона, или
   бонус хранится как флаг (0x157F5F или 0x15800C), а фактический
   attack вычисляется динамически = base_attack + bonuses.

4. ⭐ MOVEMENT RESET (смена дня)
   0x157F41: 0xf0 0x01 (496) → 0x00 0x00 (0) — movement сброшен (новый день)
   0x157F04: experience +1024 — возможно суточный доход опыта

5. ⭐ VISIT FLAGS (hero alt block)
   0x0651B5: 0x10 0x22 → 0x00 0x00 — visit flag сброшен
   0x0651BC: 0x58 → 0x00 — visit counter сброшен
   0x065232: 0x33 → 0x22 — movement byte (уменьшился)
   0x065238: 0x05 → 0x58 — visit counter установлен (88 = event_id)

6. ⭐ MAP EVENT COUNTER / DAY COUNTER
   0x17EC78: 0x00 → 0x04
   Возможно, это счётчик дня (day 3 = 0x03, но +4 = что-то другое)
   
   Или это счётчик событий (4 новых path-записи)

7. ⭐ PATH-ЗАПИСИ (4 движения)
   4 записи типа 01 03 (движение героя):
   (48, 94) → (49, 95) → (50, 95) → (51, 95) → (52, 94)
   counter = 88 (0x58) во всех записях

8. ⭐ ИМЯ ФАЙЛА
   0x3B3: "BATTLE\0\0\0" → "312_1.GM1"
   Имя файла сейва изменилось

ОТКРЫТЫЕ ВОПРОСЫ:
  - Где хранится ATTACK stat? (не найдено +1 ни в одном байте)
  - Что означает 0x157F5F (бит 5) — Mercenary Camp или другой объект?
  - Что означает 0x15800C — Mercenary Camp или другой объект?
  - 0x17EC78 — это day counter или event counter?
""")
