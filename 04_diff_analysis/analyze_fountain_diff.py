#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_2 → 0020_3.
Контекст: посетил фонтан удачи (Fountain of Fortune).
Должен установиться флаг посещения до какого-то события для этого героя.

Главные находоки:
  0x06a64e / 0x06a654: hero #2 movement bytes (после хода)
  0x06b77a: hero #2 movement byte (другой)
  0x06b780: u32 LE 0xfffe403f → 0x00000058 — visit flag reset
  0x157ec3: hero coordinates (51,99) → (53,100)
  0x157ee0: u8 0x00 → 0x02 — счетчик +2
  0x157eec: u8 0x01 → 0x02 — счетчик +1
  0x157f4d: u8 0x00 → 0x10 (16) — ⭐ НОВОЕ поле! Возможно luck bonus
  0x181884: новые path-записи (движение героя)
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

A = load('/home/z/my-project/upload/0020_2.GM1')
B = load('/home/z/my-project/upload/0020_3.GM1')
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
    (0x06b77a, 0x06b784, "⭐ Hero #2 movement + visit counter"),
    (0x157ec3, 0x157ed7, "Hero X,Y coordinates"),
    (0x157ee0, 0x157f02, "⭐ Hero stats counters + movement"),
    (0x157f4d, 0x157f4e, "⭐⭐ NEW FIELD (luck bonus?)"),
    (0x181884, 0x1818a3, "New path records"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_2, до фонтана):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_3, после фонтана):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ visit counter ===
print("\n" + "="*80)
print("АНАЛИЗ VISIT COUNTER (Hero #2)")
print("="*80)

val_a = int.from_bytes(A[0x6b780:0x6b784], 'little')
val_b = int.from_bytes(B[0x6b780:0x6b784], 'little')
print(f"u32 @ 0x6b780: A=0x{val_a:08x} ({val_a if val_a<0x80000000 else -1}) → B=0x{val_b:08x} ({val_b})")
print(f"  A: 0xFFFE403F — отрицательное число или -1 + extra")
print(f"  B: 0x00000058 = 88 — visit_id (тот же, что был у конюшни)")
print(f"  ⭐ Видимо 0x58 = visit_id для ВСЕХ посещаемых объектов")
print(f"     (не специфичный для конюшни, а общий event counter)")

# === Анализ нового поля 0x157f4d ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ НОВОГО ПОЛЯ @ 0x157f4d")
print("="*80)

val_a = A[0x157f4d]
val_b = B[0x157f4d]
print(f"u8 @ 0x157f4d: A=0x{val_a:02x} ({val_a}) → B=0x{val_b:02x} ({val_b})")
print(f"Delta = +{val_b - val_a}")
print()
print("Контекст вокруг 0x157f4d (hero stats block):")
print("A:")
print(hex_dump(A, 0x157f00, 0x157f70))
print("B:")
print(hex_dump(B, 0x157f00, 0x157f70))

# === Hero coordinates ===
print("\n" + "="*80)
print("АНАЛИЗ КООРДИНАТ ГЕРОЯ")
print("="*80)
xa = chr(A[0x157ec3])
ya = chr(A[0x157ec5])
xb = chr(B[0x157ec3])
yb = chr(B[0x157ec5])
print(f"Hero current X,Y (UTF-16 LE @ 0x157ec3):")
print(f"  A: X='{xa}' (={ord(xa)}), Y='{ya}' (={ord(ya)})")
print(f"  B: X='{xb}' (={ord(xb)}), Y='{yb}' (={ord(yb)})")
print(f"  → Герой сдвинулся с ({ord(xa)}, {ord(ya)}) на ({ord(xb)}, {ord(yb)})")

# === Hero stat counters ===
print("\n" + "="*80)
print("АНАЛИЗ СЧЁТЧИКОВ @ 0x157ee0, 0x157eec")
print("="*80)

val_a1 = A[0x157ee0]
val_b1 = B[0x157ee0]
val_a2 = A[0x157eec]
val_b2 = B[0x157eec]
print(f"u8 @ 0x157ee0: A={val_a1} → B={val_b1} (delta={val_b1-val_a1:+d})")
print(f"u8 @ 0x157eec: A={val_a2} → B={val_b2} (delta={val_b2-val_a2:+d:+d})".replace(":+d:+d", ""))
print(f"u8 @ 0x157eec: A={val_a2} → B={val_b2} (delta={val_b2-val_a2})")

# === Path-записи ===
print("\n" + "="*80)
print("АНАЛИЗ PATH-ЗАПИСЕЙ")
print("="*80)
print("\nB @ 0x181880..0x1818b0:")
print(hex_dump(B, 0x181880, 0x1818b0))

# Запись 1: 01 03 (15b) — движение
rec1 = B[0x181884:0x181884+15]
print(f"\nЗапись 1 (тип 01 03, 15b) @ 0x181884:")
print(f"  {rec1.hex()}")
print(f"  counter={int.from_bytes(rec1[2:6],'little')}, N={rec1[6]}")
print(f"  from=({int.from_bytes(rec1[7:9],'little')}, {int.from_bytes(rec1[9:11],'little')})")
print(f"  to=({int.from_bytes(rec1[11:13],'little')}, {int.from_bytes(rec1[13:15],'little')})")

# Запись 2: 01 03 (15b) — движение
rec2 = B[0x181884+15:0x181884+30]
print(f"\nЗапись 2 (тип 01 03, 15b) @ 0x{0x181884+15:08x}:")
print(f"  {rec2.hex()}")
print(f"  counter={int.from_bytes(rec2[2:6],'little')}, N={rec2[6]}")
print(f"  from=({int.from_bytes(rec2[7:9],'little')}, {int.from_bytes(rec2[9:11],'little')})")
print(f"  to=({int.from_bytes(rec2[11:13],'little')}, {int.from_bytes(rec2[13:15],'little')})")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ФОНТАН УДАЧИ (FOUNTAIN OF FORTUNE) — НАЙДЕНЫ:

1. ⭐⭐⭐ VISIT FLAG (Hero #2)
   Смещение: 0x6b780 (4b, u32 LE)
   A: 0xFFFE403F — "не посещено" (encoded)
   B: 0x00000058 (88) — visit_id
   
   ⭐ 0x58 (88) — это УНИВЕРСАЛЬНЫЙ visit_id для всех объектов!
   Тот же 0x58 использовался при посещении конюшни (0020 → 0020_1).
   
   ВЫВОД: 88 — это не ID конюшни, а GLOBAL EVENT COUNTER.
   Каждое посещение объекта увеличивает этот счётчик.
   (Хотя в нашем случае значение осталось 88 — может, это
   weekly visit counter или что-то другое)

2. ⭐⭐⭐ НОВОЕ ПОЛЕ @ 0x157f4d — LUCK BONUS?
   Смещение: 0x157f4d (1b, u8)
   A: 0x00 → B: 0x10 (16)
   Delta = +16
   
   Фонтан удачи даёт +1 luck до конца недели.
   Значение 16 = 0x10 — может быть:
     - luck modifier (16 в какой-то кодировке)
     - visited-objects bitmask (бит 4 = фонтан удачи)
     - что-то другое
   
   ⭐ Кандидат на "luck bonus until end of week"

3. ⭐ HERO STAT COUNTERS изменились
   Смещение: 0x157ee0 (u8): 0 → 2 (+2)
   Смещение: 0x157eec (u8): 1 → 2 (+1)
   
   Это могут быть:
     - счетчик ходов героя
     - счетчик посещённых объектов
     - day counter

4. ⭐ HERO COORDINATES изменились
   Смещение: 0x157ec3 (UTF-16 LE)
   A: '3' 'c' = (51, 99) → B: '5' 'd' = (53, 100)
   Герой сдвинулся на (+2, +1) — подошёл к фонтану

5. ⭐ PATH-ЗАПИСИ ДВИЖЕНИЯ (2 записи типа 01 03)
   Запись 1: (51, 99) → (52, 100)
   Запись 2: (52, 100) → (53, 100)
   counter = 88 в обеих записях (visit_id от предыдущего события)

6. ⭐ MAP EVENT COUNTER
   Смещение: 0x17ec37: 0xc6 (198) → 0xc8 (200) — delta=+2 (по числу path-записей)

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x6b780: hero #2 visit flag (u32 LE)
           0xFFFE403F = "encoded не посещено"
           0x00000058 = visit event counter
  0x157ee0: hero stat counter 1 (u8) — +2 при действии
  0x157eec: hero stat counter 2 (u8) — +1 при действии
  0x157f4d: ⭐⭐ LUCK BONUS field? (u8) — +16 при посещении фонтана удачи

ВОЗМОЖНАЯ СВЯЗЬ:
  При посещении фонтана удачи (luck modifier):
    - Visit flag @ 0x6b780 устанавливается в 0x58 (event_id)
    - Luck bonus @ 0x157f4d увеличивается на 16 (0x10)
    - Visit counter @ 0x13e409 устанавливается в 0x58 (последний посещённый)
    
  16 = 0x10 в двоичном виде = 00010000
  Возможно, бит 4 = "visited luck-modifying object this week"
""")
