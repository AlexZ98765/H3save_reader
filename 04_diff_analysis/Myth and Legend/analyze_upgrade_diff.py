#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_3 → 0020_4.
Контекст: посетил форт улучшений и улучшил существ в отряде.

Главные находки:
  0x6b77a: hero movement byte (опять обмен значениями)
  0x6b780: hero #2 visit flag (88 → -1) — visit сбросился
  0x6f998 / 0x6f99e: ⭐ НОВЫЙ БЛОК — Hero #3 stats + visit counter!
  0x140b26: ⭐⭐ КООРДИНАТЫ ГОСТЯ ФОРТА УЛУЧШЕНИЙ (появились!)
  0x157ec3: hero coordinates (53,100) → (56,104)
  0x157eec: hero stat counter 2 (+4)
  0x157efc / 0x157f02: movement values
  0x157f4e: u8 0x46 → 0x47 (битовая маска +1?)
  0x1818a2: 6 новых path-записей (движение к форту)
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

A = load('/home/z/my-project/upload/0020_3.GM1')
B = load('/home/z/my-project/upload/0020_4.GM1')
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
    (0x06b77a, 0x06b784, "Hero #2 movement + visit flag"),
    (0x06f998, 0x06f9a2, "⭐ Hero #3 stats (NEW!)"),
    (0x140b26, 0x140b2e, "⭐⭐ Fort visiting coordinates (NEW)"),
    (0x157ec3, 0x157f02, "Hero coords + counters + movement"),
    (0x157f4d, 0x157f4f, "⭐ Visit bonus bitmask"),
    (0x1818a2, 0x18192a, "⭐ Path records (6 moves)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_3, до форта улучшений):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_4, после форта улучшений):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ Hero #3 ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ НОВОГО БЛОКА HERO #3")
print("="*80)
val_a = int.from_bytes(A[0x6f99e:0x6f9a2], 'little')
val_b = int.from_bytes(B[0x6f99e:0x6f9a2], 'little')
print(f"u32 @ 0x6f99e: A=0x{val_a:08x} ({val_a if val_a<0x80000000 else -1}) → B=0x{val_b:08x} ({val_b})")
print(f"u8 @ 0x6f998: A=0x{A[0x6f998]:02x} → B=0x{B[0x6f998]:02x}")
print()
print("Структура hero blocks (по stride 0x280 = 640 байт):")
hero2_addr = 0x6b716
hero3_addr = 0x6f998
stride = hero3_addr - hero2_addr
print(f"  Hero #2 starts at: 0x{hero2_addr:08x}")
print(f"  Hero #3 starts at: 0x{hero3_addr:08x}")
print(f"  Stride = 0x{stride:x} = {stride} байт")
print(f"  ⭐ Stride между hero blocks = {stride} байт (0x{stride:x})")

# === Анализ координат гостя (0x140b26) ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ КООРДИНАТ ГОСТЯ (0x140b26)")
print("="*80)
val1_a = int.from_bytes(A[0x140b26:0x140b2a], 'little')
val2_a = int.from_bytes(A[0x140b2a:0x140b2e], 'little')
val1_b = int.from_bytes(B[0x140b26:0x140b2a], 'little')
val2_b = int.from_bytes(B[0x140b2a:0x140b2e], 'little')
print(f"u32 #1 @ 0x140b26: A=0x{val1_a:08x} ({val1_a if val1_a<0x80000000 else -1}) → B=0x{val1_b:08x} ({val1_b})")
print(f"u32 #2 @ 0x140b2a: A=0x{val2_a:08x} ({val2_a if val2_a<0x80000000 else -1}) → B=0x{val2_b:08x} ({val2_b})")
print(f"  B: ({val1_b}, {val2_b}) — координаты гостя")
print(f"  0x67 = 103, 0x53 = 83 → (103, 83) или (83, 103)?")
print(f"  Hero coords в B: (56, 104) — герой у форта улучшений")
print(f"  103 и 83 — не совпадают напрямую, но 0x67 = 103 — может быть связан")

# === Анализ hero coords ===
print("\n" + "="*80)
print("АНАЛИЗ КООРДИНАТ ГЕРОЯ")
print("="*80)
xa = chr(A[0x157ec3])
ya = chr(A[0x157ec5])
xb = chr(B[0x157ec3])
yb = chr(B[0x157ec5])
print(f"Hero X,Y (UTF-16 LE):")
print(f"  A: '{xa}'{ya}' = ({ord(xa)}, {ord(ya)})")
print(f"  B: '{xb}{yb}' = ({ord(xb)}, {ord(yb)})")
print(f"  → Герой сдвинулся с ({ord(xa)}, {ord(ya)}) на ({ord(xb)}, {ord(yb)})")
print(f"  Это +3/+4 в координатах — долгий путь к форту")

# === Анализ флага @ 0x157f4e ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ ФЛАГА @ 0x157f4e")
print("="*80)
val_a = A[0x157f4e]
val_b = B[0x157f4e]
print(f"u8 @ 0x157f4e: A=0x{val_a:02x} ({val_a}) → B=0x{val_b:02x} ({val_b})")
print(f"Delta = +{val_b - val_a}")
print(f"  A: 0x46 = 01000110 (биты 1, 2, 6)")
print(f"  B: 0x47 = 01000111 (биты 0, 1, 2, 6)")
print(f"  Изменился бит 0 (0x01) — установлен")
print()
print(f"⭐ В предыдущем диффе (0020_2 → 0020_3, фонтан удачи):")
print(f"  0x157f4d: 0x00 → 0x10 (бит 4 = фонтан удачи)")
print(f"  0x157f4e: не изменился (0x46)")
print(f"")
print(f"В этом диффе (форт улучшений):")
print(f"  0x157f4d: не изменился (0x10)")
print(f"  0x157f4e: 0x46 → 0x47 (бит 0 = форт улучшений)")
print(f"")
print(f"⭐ ГИПОТЕЗА: 0x157f4d и 0x157f4e — это битовые маски посещённых объектов")
print(f"  0x157f4d бит 4 (0x10) = фонтан удачи")
print(f"  0x157f4e бит 0 (0x01) = форт улучшений")

# === Path-записи ===
print("\n" + "="*80)
print("АНАЛИЗ PATH-ЗАПИСЕЙ (6 движений)")
print("="*80)
print("\nB @ 0x1818a0..0x181940:")
print(hex_dump(B, 0x1818a0, 0x181940))

# Декодировать все 01 03 записи
print("\n--- Декодировка path-записей ---")
offset = 0x1818a2
for i in range(6):
    if B[offset:offset+2] != b'\x01\x03':
        break
    rec = B[offset:offset+15]
    counter = int.from_bytes(rec[2:6], 'little')
    n = rec[6]
    fx, fy = int.from_bytes(rec[7:9],'little'), int.from_bytes(rec[9:11],'little')
    tx, ty = int.from_bytes(rec[11:13],'little'), int.from_bytes(rec[13:15],'little')
    print(f"  [{i+1}] @ 0x{offset:08x}: counter={counter}, N={n}, ({fx},{fy}) → ({tx},{ty})")
    offset += 15

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ФОРТ УЛУЧШЕНИЙ (UPGRADE FORT) — НАЙДЕНЫ:

1. ⭐⭐⭐ НОВЫЙ БЛОК HERO #3
   Смещение: 0x6f998 (movement byte), 0x6f99e (visit counter)
   Stride между hero blocks: 0x6f998 - 0x6b716 = 0x482 = 1154 байта
   ⚠️ Это stride между hero #2 и hero #3, но:
      - Hero #1: ~0x6d7a4 (movement)
      - Hero #2: ~0x6b716 (movement) ← отличается!
      - Hero #3: ~0x6f998 (movement) ← отличается!
   
   Возможно hero blocks расположены не подряд, а с переменным stride
   (в зависимости от количества данных в каждом блоке).

2. ⭐⭐ HERO #2 VISIT FLAG сброшен
   Смещение: 0x6b780 (u32 LE)
   A: 0x00000058 (88) — посещено
   B: 0xFFFE403F — "не посещено" (encoded)
   ⭐ Подтверждает: visit flag сбрасывается при перемещении героя

3. ⭐⭐ HERO #3 VISIT COUNTER установлен
   Смещение: 0x6f99e (u32 LE)
   A: 0xFFFFFFFF — не посещено
   B: 0x00000058 (88) — visit_id
   ⭐ Это ПОДТВЕРЖДАЕТ гипотезу: 88 = универсальный event counter
   Hero #3 теперь посетил объект (форт улучшений).

4. ⭐⭐ КООРДИНАТЫ ГОСТЯ ФОРТА УЛУЧШЕНИЙ
   Смещение: 0x140b26 (8b = 2 × u32 LE)
   A: 0xFFFFFFFF 0xFFFFFFFF (не задано)
   B: 0x00000067 0x00000053 (X=103, Y=83)
   ⭐ Это аналог visiting hero coordinates (0x143e6e для города)
   Но для объектов карты — отдельный массив координат гостей!

5. ⭐⭐ БИТОВАЯ МАСКА ПОСЕЩЁННЫХ ОБЪЕКТОВ
   Смещение: 0x157f4e (u8)
   A: 0x46 (01000110) → B: 0x47 (01000111)
   Delta = +1 (бит 0 установлен)
   
   ⭐⭐ Это подтверждает гипотезу о битовой маске!
   0x157f4d + 0x157f4e = 16-битная маска посещённых объектов:
     бит 0  (0x01 в 0x157f4e) = ⭐ UPGRADE FORT
     бит 4  (0x10 в 0x157f4d) = FOUNTAIN OF FORTUNE
   
   Видимо, есть и другие биты для других объектов:
     бит 1  = ?
     бит 2  = ?
     бит 6  = ?
     ...

6. ⭐ HERO COORDINATES изменились
   Смещение: 0x157ec3 (UTF-16 LE)
   A: ('5', 'd') = (53, 100) → B: ('8', 'h') = (56, 104)
   Герой сдвинулся на (+3, +4) — прошёл долгий путь

7. ⭐ HERO STAT COUNTERS
   0x157eec: 0x02 → 0x06 (+4) — счётчик ходов героя увеличился на 4

8. ⭐ 6 PATH-ЗАПИСЕЙ ДВИЖЕНИЯ (тип 01 03)
   Герой сделал 6 шагов к форту:
   (53,100) → (54,101) → (55,101) → (56,102) → ...

9. ⭐ MAP EVENT COUNTER
   0x17ec37: 0xc8 (200) → 0xd1 (209) — delta=+9
   (больше, чем количество path-записей — 6; видимо ещё что-то произошло)

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x6f998: hero #3 movement byte (u8) — stride между hero blocks ≈ 0x482
  0x6f99e: hero #3 visit counter (u32 LE)
  0x140b26: ⭐⭐ MAP OBJECT VISITING COORDS (8b = 2 × u32 LE)
            — отдельный массив для гостей объектов карты
            — отличается от 0x143e6e (visiting hero for town)
  0x157f4e: ⭐⭐ UPGRADE FORT visit bit (бит 0)
            — дополняет 0x157f4d (FOUNTAIN OF FORTUNE = бит 4)
""")
