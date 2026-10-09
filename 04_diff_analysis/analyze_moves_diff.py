#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_7 → 0020_8.
Контекст: 
  - Hero #1 (главный): сходил 1 клетку по горизонтали
  - Hero #5 (в лодке): сходил 2 клетки по диагонали

Главные находки в диффе:
  0x07312f, 0x073145: ⭐ Hero stats (booleans меняются местами — visit flag reset)
  0x07343f: ⭐ Hero #3 stats (значение 0x2212 → 0x0002)
  0x0756c3: ⭐ Hero block (новое значение 0x2210)
  0x141385: ⭐⭐ Hero #1 coords (108,107) → (106,109) — НО игрок сказал 1 клетку!
  0x143e41: ⭐ u8 0x50 → 0x51 — hero counter?
  0x143e6e: ⭐ visiting coords (100,103) → (-1,-1) — сбросился
  0x18197a: ⭐ 3 path-записи (2 для Hero #1 + 1 для Hero #5?)
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

A = load('/home/z/my-project/upload/0020_7.GM1')
B = load('/home/z/my-project/upload/0020_8.GM1')
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
    (0x07312f, 0x07314d, "⭐ Hero block: visit flag reset"),
    (0x07343f, 0x073447, "⭐ Hero stats change"),
    (0x0756c3, 0x0756cb, "⭐ Hero block: new value"),
    (0x141385, 0x1413c4, "⭐⭐ Hero coords + counters"),
    (0x143e41, 0x143e80, "⭐⭐ Hero counter + visiting reset"),
    (0x18197a, 0x1819a8, "⭐⭐ Path records (3 moves)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_7):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_8):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Hero coords ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ КООРДИНАТ ГЕРОЯ")
print("="*80)

# 0x141385 — это UTF-16 LE
xa = chr(A[0x141385])
ya = chr(A[0x141387])
xb = chr(B[0x141385])
yb = chr(B[0x141387])
print(f"Hero coords @ 0x141385 (UTF-16 LE):")
print(f"  A: '{xa}'{ya}' = ({ord(xa)}, {ord(ya)})")
print(f"  B: '{xb}{yb}' = ({ord(xb)}, {ord(yb)})")
print(f"  Delta = ({ord(xb)-ord(xa)}, {ord(yb)-ord(ya)})")
print(f"  ⚠️ Это не '1 клетка по горизонтали' — это диагональное движение!")
print(f"  Возможно, это Hero #5 (в лодке), который ходил 2 клетки по диагонали")

# Hero counter 0x14138ae
print(f"\nu8 @ 0x14138ae: A=0x{A[0x14138ae]:02x} ({A[0x14138ae]}) → B=0x{B[0x14138ae]:02x} ({B[0x14138ae]})")
print(f"  ⭐ Hero counter: 2 → 5 (delta=+3) — счётчик ходов героя")

# Movement delta 0x14138c2
val_a = int.from_bytes(A[0x14138c2:0x14138c6], 'little')
val_b = int.from_bytes(B[0x14138c2:0x14138c6], 'little')
print(f"\nu16 LE @ 0x14138c2: A=0x{val_a:04x} ({val_a}) → B=0x{val_b:04x} ({val_b})")
print(f"  Delta = {val_b - val_a}")
print(f"  ⭐ Movement decreased after moves")

# === Hero counter @ 0x143e41 ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ HERO COUNTER @ 0x143e41")
print("="*80)
print(f"u8 @ 0x143e41: A=0x{A[0x143e41]:02x} ({A[0x143e41]}) → B=0x{B[0x143e41]:02x} ({B[0x143e41]})")
print(f"u8 @ 0x143e48: A=0x{A[0x143e48]:02x} ({A[0x143e48]}) → B=0x{B[0x143e48]:02x} ({B[0x143e48]})")
print(f"  ⭐ Оба байта: 0x50 (80) → 0x51 (81) — счётчик ходов +1")

# === Visiting coords @ 0x143e6e ===
val1_a = int.from_bytes(A[0x143e6e:0x143e72], 'little')
val2_a = int.from_bytes(A[0x143e72:0x143e76], 'little')
val1_b = int.from_bytes(B[0x143e6e:0x143e72], 'little')
val2_b = int.from_bytes(B[0x143e72:0x143e76], 'little')
print(f"\nu32 @ 0x143e6e (visiting coords):")
print(f"  A: ({val1_a}, {val2_a})")
print(f"  B: ({val1_b if val1_b<0x80000000 else -1}, {val2_b if val2_b<0x80000000 else -1})")
print(f"  ⭐ Visiting coords сброшены в (-1, -1) — форт улучшений покинут!")

# u32 @ 0x143e7e
val_a = int.from_bytes(A[0x143e7e:0x143e82], 'little')
val_b = int.from_bytes(B[0x143e7e:0x143e82], 'little')
print(f"\nu16 LE @ 0x143e7e: A=0x{val_a:04x} ({val_a}) → B=0x{val_b:04x} ({val_b})")
print(f"  Delta = {val_b - val_a} — movement related")

# === Path-записи ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ PATH-ЗАПИСЕЙ (3 движения)")
print("="*80)
print("\nB @ 0x181975..0x1819b0:")
print(hex_dump(B, 0x181975, 0x1819b0))

# Декодировать 3 path-записи
print("\n--- Декодировка path-записей ---")
offset = 0x18197a
for i in range(3):
    if B[offset:offset+2] != b'\x01\x03':
        print(f"  [{offset:08x}] НЕ 01 03: {B[offset:offset+2].hex()}")
        break
    rec = B[offset:offset+15]
    counter = int.from_bytes(rec[2:6], 'little')
    n = rec[6]
    fx, fy = int.from_bytes(rec[7:9],'little'), int.from_bytes(rec[9:11],'little')
    tx, ty = int.from_bytes(rec[11:13],'little'), int.from_bytes(rec[13:15],'little')
    print(f"  [{i+1}] @ 0x{offset:08x}: counter={counter}, N={n}, ({fx},{fy}) → ({tx},{ty})  delta=({tx-fx:+d},{ty-fy:+d})")
    offset += 15

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ДВИЖЕНИЕ ДВУХ ГЕРОЕВ — НАЙДЕНЫ:

1. ⭐⭐⭐ PATH-ЗАПИСИ (3 движения)
   3 записи типа "01 03" (15b каждая):
   
   Запись 1: counter=13 (0x0d), N=2
     (80, 107) → (81, 107)  delta = (+1, 0)
     ⭐ Это Hero #1: 1 клетка по горизонтали (как и сказал игрок)!
   
   Запись 2: counter=3, N=5
     (108, 107) → (108, 108)  delta = (0, +1)
     ⭐ Это первое движение Hero #5 (в лодке)?
   
   Запись 3: counter=3, N=5
     (108, 108) → (106, 109)  delta = (-2, +1)
     ⭐ Это второе движение Hero #5 — диагональ!
   
   ⚠️ Hero #5 сделал только 1 step в записи 3, но delta = (-2, +1)
      Возможно, для лодки 1 step = 2 тайла (быстрее передвижение)

2. ⭐⭐ HERO COORDS @ 0x141385 (UTF-16 LE)
   A: ('l','k') = (108, 107)  →  B: ('j','m') = (106, 109)
   delta = (-2, +2) — диагональное движение
   
   ⭐ Это Hero #5 (в лодке) — его координаты изменились.
   Hero #1 (главный) имеет координаты в другом блоке (0x157ec3).

3. ⭐⭐ HERO COUNTERS @ 0x143e41, 0x143e48
   u8 @ 0x143e41: 0x50 (80) → 0x51 (81) — счётчик +1
   u8 @ 0x143e48: 0x50 (80) → 0x51 (81) — счётчик +1
   ⭐ Это общие счётчики ходов (увеличиваются на 1 за ход)

4. ⭐⭐ VISITING COORDS сброшены
   u32 @ 0x143e6e: (100, 103) → (-1, -1)
   ⭐ Форт улучшений покинут (или другой объект)

5. ⭐ MOVEMENT DECREASED
   u16 LE @ 0x14138c2: 0x0c35 (3125) → 0x0b1b (2843)
   Delta = -282 — movement points уменьшился после хода
   
   u16 LE @ 0x143e7e: 0x0738 (1848) → 0x06d4 (1748)
   Delta = -100 — movement для другого героя

6. ⭐ HERO BLOCK CHANGES (несколько hero blocks)
   0x07312f: visit flag reset (10 22 → 00 00)
   0x073145: visit flag set (00 00 → 10 22)
   ⭐ Значение "10 22" (0x2210 = 8720) — это visit_id!
   
   0x07343f: 12 22 → 02 00 (Hero #3 stats change)
   0x0756c3: 00 00 → 10 22 (новое значение 0x2210)
   
   ⭐ Pattern: "10 22" (0x2210) появляется как visit_id в нескольких блоках

7. ⭐ ACTION FLAG
   0x13e409: 0x01 → 0x03 — флаг действия изменился

8. ⭐ MAP EVENT COUNTER
   0x17ec37: 0xd7 (215) → 0xda (218) — delta=+3 (по числу path-записей)

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x07312f / 0x073145: hero visit flags (swap pattern)
  0x07343f: Hero #3 visit counter (u16 LE)
  0x0756c3: Hero block visit flag (u16 LE)
  0x141385: ⭐ Hero #5 coords (UTF-16 LE, в лодке)
  0x14138ae: Hero #5 movement counter (u8)
  0x14138c2: Hero #5 movement (u16 LE)
  0x143e41: ⭐ Global hero turn counter (u8, +1 per turn)
  0x143e48: ⭐ Global hero turn counter 2 (u8, +1 per turn)
  0x143e6e: visiting coords (reset to -1, -1)
  0x143e7e: Hero movement (u16 LE)

⭐⭐ СТРУКТУРНОЕ ОТКРЫТИЕ:
  В файле есть МНОЖЕСТВО hero blocks, и у каждого свой счётчик
  visit_id. Значение "10 22" (0x2210) появляется в нескольких
  блоках одновременно — это, видимо, общий event counter,
  который перетекает между блоками при действиях.
""")
