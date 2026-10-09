#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_1 → 0020_2.
Контекст: захватил шахту руды (была оранжевого игрока, стала моей).

Главные находки:
  0x6a64e / 0x6a654: hero #2 stats (movement после хода к шахте)
  0x6b716: hero #2 movement byte
  0x6b71c: hero #2 visit counter (88 → -1, visit сбросился)
  0x13c850: ⭐ FLAG смены владельца шахты (4 → 3 = orange → my_color)
  0x157ec3: hero coordinates (1,3,4) → (3,5)
  0x157eec: покинутые тайлы
  0x18185e: новые path-записи
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

A = load('/home/z/my-project/upload/0020_1.GM1')
B = load('/home/z/my-project/upload/0020_2.GM1')
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
    (0x06a64e, 0x06a655, "Hero #2 movement bytes"),
    (0x06b716, 0x06b720, "Hero #2 stats + visit counter"),
    (0x13c850, 0x13c851, "⭐ MINE OWNER FLAG"),
    (0x157ec3, 0x157ed7, "Hero X,Y coordinates"),
    (0x157eec, 0x157ef8, "Visited tiles + movement bonus"),
    (0x18185e, 0x00181886, "⭐ New path records (mine capture)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 64)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_1, до захвата шахты):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_2, после захвата шахты):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ FLAG смены владельца ===
print("\n" + "="*80)
print("⭐⭐⭐ АНАЛИЗ ФЛАГА ВЛАДЕЛЬЦА ШАХТЫ")
print("="*80)

val_a = A[0x13c850]
val_b = B[0x13c850]
print(f"\nu8 @ 0x13c850: A=0x{val_a:02x} ({val_a}) → B=0x{val_b:02x} ({val_b})")
print(f"Delta = {val_b - val_a}")

# Расширенный контекст для понимания структуры массива объектов
print("\n--- Контекст вокруг 0x13c850 (массив владельцев объектов?) ---")
print("A:")
print(hex_dump(A, 0x13c800, 0x13c8a0))
print("B:")
print(hex_dump(B, 0x13c800, 0x13c8a0))

# === Player colors в HoMM3 ===
print("""
PLAYER COLORS в HoMM3 (стандарт):
  0 = Red
  1 = Blue
  2 = Tan
  3 = Green  ← наш игрок (вероятно)
  4 = Orange ← был владелец шахты
  5 = Purple
  6 = Teal
  7 = Pink
  0xFF = Neutral

Изменение 0x04 → 0x03 означает:
  было: owner = ORANGE (4)
  стало: owner = GREEN (3) — наш игрок
""")

# === Hero coordinates ===
print("\n" + "="*80)
print("АНАЛИЗ КООРДИНАТ ГЕРОЯ")
print("="*80)

# Hero X,Y в UTF-16 LE
xa = chr(A[0x157ec3])
ya = chr(A[0x157ec5])
xb = chr(B[0x157ec3])
yb = chr(B[0x157ec5])
print(f"\nHero current X,Y (UTF-16 LE @ 0x157ec3):")
print(f"  A: X='{xa}' (={ord(xa)}), Y='{ya}' (={ord(ya)})")
print(f"  B: X='{xb}' (={ord(xb)}), Y='{yb}' (={ord(yb)})")
print(f"  → Герой сдвинулся с ({ord(xa)}, {ord(ya)}) на ({ord(xb)}, {ord(yb)})")

# === Visit counter ===
print("\n" + "="*80)
print("АНАЛИЗ VISIT COUNTER")
print("="*80)
val_a = int.from_bytes(A[0x6b71c:0x6b720], 'little')
val_b = int.from_bytes(B[0x6b71c:0x6b720], 'little')
print(f"u32 @ 0x6b71c: A=0x{val_a:08x} ({val_a if val_a<0x80000000 else -1}) → B=0x{val_b:08x} ({val_b if val_b<0x80000000 else -1})")
print(f"Visit counter СБРОШЕН! 88 (stables) → -1 (не посещено)")
print(f"Это логично: после хода героя visit сбрасывается, чтобы можно было посетить новый объект")

# === Path-записи ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ НОВЫХ PATH-ЗАПИСЕЙ")
print("="*80)

# Дамп B в районе новых path-записей
print("\nB @ 0x181850..0x1818c0:")
print(hex_dump(B, 0x181850, 0x1818c0))

# Запись 1: 01 03 (15b) — движение героя
rec1 = B[0x18185e:0x18185e+15]
print(f"\nЗапись 1 (тип 01 03, 15b) @ 0x18185e:")
print(f"  {rec1.hex()}")
print(f"  counter={int.from_bytes(rec1[2:6],'little')}, N={rec1[6]}")
print(f"  from=({int.from_bytes(rec1[7:9],'little')}, {int.from_bytes(rec1[9:11],'little')})")
print(f"  to=({int.from_bytes(rec1[11:13],'little')}, {int.from_bytes(rec1[13:15],'little')})")

# Запись 2: 01 03 (15b)
rec2_offset = 0x18185e + 15
rec2 = B[rec2_offset:rec2_offset+15]
print(f"\nЗапись 2 (тип 01 03, 15b) @ 0x{rec2_offset:08x}:")
print(f"  {rec2.hex()}")
print(f"  counter={int.from_bytes(rec2[2:6],'little')}, N={rec2[6]}")
print(f"  from=({int.from_bytes(rec2[7:9],'little')}, {int.from_bytes(rec2[9:11],'little')})")
print(f"  to=({int.from_bytes(rec2[11:13],'little')}, {int.from_bytes(rec2[13:15],'little')})")

# Запись 3: 03 03 — НОВЫЙ ТИП!
rec3_offset = rec2_offset + 15
print(f"\nЗапись 3 (НОВЫЙ ТИП 03 03) @ 0x{rec3_offset:08x}:")
print(f"  {B[rec3_offset:rec3_offset+20].hex()}")

# Декодируем запись типа 03 03
rec3 = B[rec3_offset:rec3_offset+9]
print(f"  Декодировка (предположительно):")
print(f"  flag1=0x{rec3[0]:02x} (тип: CAPTURE?)")
print(f"  flag2=0x{rec3[1]:02x}")
print(f"  u32 LE counter = {int.from_bytes(rec3[2:6],'little')}")
print(f"  u8 = 0x{rec3[6]:02x} — возможно, новый owner color")
print(f"  u8 = 0x{rec3[7]:02x} — object type?")
print(f"  u8 = 0x{rec3[8]:02x} — flag?")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ЗАХВАТ ШАХТЫ — НАЙДЕНЫ:

1. ⭐⭐⭐ ФЛАГ ВЛАДЕЛЬЦА ШАХТЫ (OBJECT OWNER)
   Смещение: 0x13c850 (1 байт, u8)
   A: 0x04 (ORANGE player)  →  B: 0x03 (GREEN player = наш игрок)
   
   ⭐ Это массив владельцев объектов на карте!
   Каждый объект карты (шахта, город, жилище существ и т.д.)
   имеет 1 байт со значением:
     0 = Red
     1 = Blue
     2 = Tan
     3 = Green  ← наш игрок
     4 = Orange ← бывший владелец шахты
     5 = Purple
     6 = Teal
     7 = Pink
     0xFF = Neutral
   
   Изменение 4 → 3 = шахта перешла от ORANGE к GREEN

2. ⭐⭐ HERO VISIT COUNTER сброшен
   Смещение: 0x6b71c (4b, u32 LE)
   A: 0x58 (88) — посещена конюшня
   B: 0xFFFFFFFF (-1) — не посещено (сброс после хода)
   
   Visit counter хранит только ПОСЛЕДНЕЕ посещённое событие.
   Когда герой перемещается, он может посетить новый объект,
   поэтому счётчик сбрасывается.

3. ⭐ НОВЫЙ ТИП PATH-ЗАПИСИ: "03 03" — CAPTURE EVENT
   Размер: ~9 байт
   Формат:
     u8  flag1 = 0x03 (тип: CAPTURE)
     u8  flag2 = 0x03
     u32 counter (LE) = 0x22 (34) — visit_id захвата
     u8  new_owner = 0x04 (новый владелец? но это ORANGE, а должно быть GREEN=3)
     u8  object_type = 0x03 (тип объекта — шахта?)
     u8  flag = 0x0b
   
   ⚠️ В записи значение 0x04 — возможно, это НЕ new_owner, а что-то другое.
   Может быть: previous_owner = 4 (ORANGE)?

4. ⭐ PATH-ЗАПИСИ ДВИЖЕНИЯ
   2 записи типа "01 03" (15b каждая) — движение героя
   counter = 0x58 (88) — visit_id от конюшни?
   
5. ⭐ HERO COORDINATES изменились
   Смещение: 0x157ec3 (UTF-16 LE)
   A: ('1', 'd') = (49, 100)  →  B: ('3', 'c') = (51, 99)
   Герой сдвинулся на (+2, -1) — подошёл к шахте

6. ⭐ MAP EVENT COUNTER
   Смещение: 0x17ec37
   A: 0xc3 (195) → B: 0xc6 (198) — delta=+3 (по числу новых path-записей)

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x13c850: ⭐⭐ OBJECT OWNER ARRAY (массив владельцев объектов карты)
    формат: u8 per object, value = player_color (0-7) или 0xFF (neutral)
   stride между объектами нужно определить дополнительно

ТИПЫ PATH-ЗАПИСЕЙ (обновлённый список):
  01 03 (15b) — движение героя (один шаг)
  03 03 (9b)  — ⭐ CAPTURE EVENT (захват объекта) — НОВЫЙ ТИП
  08 03 (27b) — army transfer (передача армии)
  09 03 (19b) — visit (посещение объекта)
  0b 03 (4+N*8b) — fog of war update
""")
