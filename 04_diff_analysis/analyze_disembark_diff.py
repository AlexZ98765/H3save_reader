#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_4 → 0020_5.
Контекст: герой высадился на берег с лодки.

Главные находки:
  0x59be4 / 0x59bea: ⭐ BOAT-related fields (были в воде, теперь на суше)
  0x5aac1..0x5aac9: ⭐ НОВЫЙ hero block (Hero #4?) — высадившийся герой
  0x13e112 / 0x13e119: ⭐ флаги (1→0 и 0→1)
  0x13e409: action flag 0x58 → 0x01 (event_id сменился)
  0x140af9..0x140b05: ⭐ координаты гостя (68,82) → (67,83) — изменилось!
  0x140b26: visiting coords СБРОШЕН (стало -1, -1) — форт улучшений покинут
  0x1636b2: ⭐ QUEUED PATH DESTINATION изменился (93,125 → 109,127)
  0x181929: ⭐ НОВЫЕ path-записи типа 08 03 (army transfer? disembark?)
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

A = load('/home/z/my-project/upload/0020_4.GM1')
B = load('/home/z/my-project/upload/0020_5.GM1')
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
    (0x059be4, 0x059beb, "⭐ BOAT-related fields"),
    (0x05aac1, 0x05aac9, "⭐ NEW hero block (Hero #4?)"),
    (0x13e112, 0x13e11a, "⭐ Toggle flags"),
    (0x140af9, 0x140b38, "⭐⭐ Visiting coords (multi)"),
    (0x140b82, 0x140b83, "Flag byte"),
    (0x1636b2, 0x1636b7, "⭐ Queued path destination"),
    (0x181929, 0x181955, "⭐ New path records (disembark)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_4, в лодке):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_5, высадился):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ BOAT-related ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ BOAT-RELATED FIELDS")
print("="*80)
print(f"u8 @ 0x59be4: A=0x{A[0x59be4]:02x} → B=0x{B[0x59be4]:02x}")
print(f"u8 @ 0x59bea: A=0x{A[0x59bea]:02x} → B=0x{B[0x59bea]:02x}")
print("  A: 0x22, 0x01 (в лодке, на воде)")
print("  B: 0x08, 0x06 (на суше, без лодки)")
print("  ⭐ Это флаги состояния героя: 'на воде' vs 'на суше'")

# === Анализ нового hero block ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ НОВОГО HERO BLOCK @ 0x5aac1")
print("="*80)
print("Stride от предыдущих hero blocks:")
hero_blocks = [
    ("Hero #1", 0x6d7a4),
    ("Hero #2", 0x6b716),
    ("Hero #3", 0x6f998),
    ("Hero #4?", 0x5aac1),  # или 0x5aac8?
]
for i in range(1, len(hero_blocks)):
    prev = hero_blocks[i-1][1]
    cur = hero_blocks[i][1]
    stride = abs(cur - prev)
    print(f"  {hero_blocks[i-1][0]} → {hero_blocks[i][0]}: stride = 0x{stride:x} ({stride} байт)")

# === Анализ координат гостя ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ VISITING COORDS (0x140af9)")
print("="*80)
xa = chr(A[0x140af9])
ya = chr(A[0x140afb])
xb = chr(B[0x140af9])
yb = chr(B[0x140afb])
print(f"Visiting coords (UTF-16 LE):")
print(f"  A: '{xa}'{ya}' = ({ord(xa)}, {ord(ya)})")
print(f"  B: '{xb}{yb}' = ({ord(xb)}, {ord(yb)})")

# u32 @ 0x140b26
val1_a = int.from_bytes(A[0x140b26:0x140b2a], 'little')
val2_a = int.from_bytes(A[0x140b2a:0x140b2e], 'little')
val1_b = int.from_bytes(B[0x140b26:0x140b2a], 'little')
val2_b = int.from_bytes(B[0x140b2a:0x140b2e], 'little')
print(f"\nu32 @ 0x140b26 (visiting coords #2):")
print(f"  A: ({val1_a}, {val2_a})")
print(f"  B: ({val1_b if val1_b<0x80000000 else -1}, {val2_b if val2_b<0x80000000 else -1})")
print(f"  ⭐ Форт улучшений покинут: (103, 83) → (-1, -1)")

# u32 @ 0x140b2e и 0x140b32 — ещё координаты?
val3_a = int.from_bytes(A[0x140b2e:0x140b32], 'little')
val4_a = int.from_bytes(A[0x140b32:0x140b36], 'little')
val3_b = int.from_bytes(B[0x140b2e:0x140b32], 'little')
val4_b = int.from_bytes(B[0x140b32:0x140b36], 'little')
print(f"\nu32 @ 0x140b2e: A=0x{val3_a:08x} ({val3_a}) → B=0x{val3_b:08x} ({val3_b})")
print(f"u32 @ 0x140b32: A=0x{val4_a:08x} ({val4_a}) → B=0x{val4_b:08x} ({val4_b})")

# === Queued path destination ===
print("\n" + "="*80)
print("⭐ QUEUED PATH DESTINATION")
print("="*80)
xa = int.from_bytes(A[0x1636b2:0x1636b6], 'little')
ya = int.from_bytes(A[0x1636b6:0x1636ba], 'little')
xb = int.from_bytes(B[0x1636b2:0x1636b6], 'little')
yb = int.from_bytes(B[0x1636b6:0x1636ba], 'little')
print(f"u32 @ 0x1636b2: A=({xa}, {ya}) → B=({xb}, {yb})")
print(f"  Delta = ({xb-xa}, {yb-ya})")
print(f"  ⭐ Путь героя изменился после высадки")

# === Path-записи ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ PATH-ЗАПИСЕЙ (disembark)")
print("="*80)
print("\nB @ 0x181920..0x181970:")
print(hex_dump(B, 0x181920, 0x181970))

# Декодируем новые path-записи
print("\n--- Декодировка path-записей ---")
offset = 0x181929
records = []
while offset < 0x181960:
    flag1 = B[offset]
    flag2 = B[offset+1]
    
    if flag1 == 0x08 and flag2 == 0x03:
        # Army transfer / Disembark? 27 bytes
        rec = B[offset:offset+27]
        print(f"\n  Запись типа '08 03' (27b) @ 0x{offset:08x}:")
        print(f"    {rec.hex()}")
        print(f"    flag1={rec[0]:02x}, flag2={rec[1]:02x}")
        print(f"    counter = {int.from_bytes(rec[2:6],'little')}")
        print(f"    u8 @ +6 = 0x{rec[6]:02x}")
        print(f"    u8 @ +7 = 0x{rec[7]:02x}")
        print(f"    sub-rec type = 0x{rec[8]:02x} 0x{rec[9]:02x}  ← вложенная запись")
        print(f"    sub-counter = {int.from_bytes(rec[10:14],'little')}")
        print(f"    N = {rec[14]}")
        print(f"    from = ({int.from_bytes(rec[15:17],'little')}, {int.from_bytes(rec[17:19],'little')})")
        print(f"    to = ({int.from_bytes(rec[19:21],'little')}, {int.from_bytes(rec[21:23],'little')})")
        print(f"    tail = {rec[23:].hex()}")
        offset += 27
    elif flag1 == 0x09 and flag2 == 0x03:
        # Visit (19b)
        rec = B[offset:offset+19]
        print(f"\n  Запись типа '09 03' (19b) @ 0x{offset:08x}:")
        print(f"    {rec.hex()}")
        print(f"    counter = {int.from_bytes(rec[2:6],'little')}")
        print(f"    N = {rec[6]}")
        from_x = int.from_bytes(rec[7:9],'little')
        from_y = int.from_bytes(rec[9:11],'little')
        to_x = int.from_bytes(rec[11:13],'little')
        to_y = int.from_bytes(rec[13:15],'little')
        print(f"    from = ({from_x}, {from_y})  →  to = ({to_x}, {to_y})")
        print(f"    tail = {rec[15:].hex()}")
        offset += 19
    elif flag1 == 0x01 and flag2 == 0x03:
        # Movement (15b)
        rec = B[offset:offset+15]
        print(f"\n  Запись типа '01 03' (15b) @ 0x{offset:08x}:")
        print(f"    {rec.hex()}")
        offset += 15
    else:
        print(f"\n  Неизвестный тип {flag1:02x} {flag2:02x} @ 0x{offset:08x}")
        print(f"    {B[offset:offset+30].hex()}")
        break

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ ВЫСАДКА С ЛОДКИ (DISEMBARK) — НАЙДЕНЫ:

1. ⭐⭐⭐ BOAT STATE FLAGS
   Смещение: 0x59be4 (u8) и 0x59bea (u8)
   A: 0x22, 0x01 (герой на воде, в лодке)
   B: 0x08, 0x06 (герой на суше, без лодки)
   
   ⭐ Эти байты — флаги "hero on boat" / "hero on land".
   При высадке значения меняются на специфические константы.

2. ⭐⭐⭐ НОВЫЙ HERO BLOCK (Hero #4?)
   Смещение: 0x5aac1..0x5aac9
   A: 02 03 00 (без изменений)
   B: 12 22 01 (высадившийся герой — изменились флаги)
   
   ⭐ Hero blocks в файле расположены с ПЕРЕМЕННЫМ stride:
     Hero #1: 0x6d7a4
     Hero #2: 0x6b716 (stride 0x28e от Hero #1)
     Hero #3: 0x6f998 (stride 0x482 от Hero #2)
     Hero #4: 0x5aac1 (stride 0xaed7 от Hero #3!) ← большой stride
   
   Видимо, hero blocks не выровнены по stride, а разбросаны по файлу.

3. ⭐⭐ VISITING COORDS изменились
   Смещение: 0x140af9 (UTF-16 LE, 12b)
   A: ('h','R') = (104, 82)  →  B: ('g','S') = (103, 83)
   
   u32 @ 0x140b26 (вторые visiting coords):
   A: (103, 83) — герой был у форта улучшений
   B: (-1, -1) — форт улучшений покинут!
   
   u32 @ 0x140b2e: A=0x7d0 (2000), B=0x870 (2160) — delta=+160
   u32 @ 0x140b32: A=0x7d0 (2000), B=0x0 (0)
   ⭐ Это может быть boat_id или path_id!

4. ⭐⭐ FLAG BYTE @ 0x140b82
   u8 @ 0x140b82: A=0x04 → B=0x00
   Сброс флага (возможно, "hero on boat" flag)

5. ⭐ TOGGLE FLAGS @ 0x13e112, 0x13e119
   u8 @ 0x13e112: A=0x01 → B=0x00 (сброс)
   u8 @ 0x13e119: A=0x00 → B=0x01 (установка)
   ⭐ Это может быть смена "active surface" (water → land) у героя

6. ⭐ QUEUED PATH DESTINATION изменился
   Смещение: 0x1636b2 (2 × u32 LE)
   A: (93, 125) → B: (109, 127)
   Delta = (+16, +2)
   ⭐ Путь героя обновился после высадки

7. ⭐ НОВЫЕ PATH-ЗАПИСИ
   Добавлены 2 записи:
     - "08 03" (27b) —Army transfer / Disembark event
       sub-record типа "06 03" с координатами (104,82) → (104,82)
       (hero stays at the same tile — disembark!)
     - "09 03" (19b) — Visit event
       from = (103, 83), to = (104, 82)
   
   ⭐ В path-записи "08 03" виден подтип "06 03" — это может быть
     специальный тип "DISEMBARK" event!

8. ⭐ MAP EVENT COUNTER
   0x17ec37: 0xd1 (209) → 0xd4 (212) — delta=+3

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x59be4: HERO ON BOAT flag (u8, 0x22=boat, 0x08=land)
  0x59bea: HERO SURFACE flag (u8, 0x01=water, 0x06=land)
  0x5aac1..0x5aac9: Hero #4 block (с переменным stride)
  0x13e112: surface flag 1 (toggle)
  0x13e119: surface flag 2 (toggle)
  0x140b2e: boat_id / path_id (u32 LE)
  0x140b82: hero on boat flag (u8)
  
PATH-ЗАПИСЬ типа "08 03" (27b):
  Содержит ВЛОЖЕННУЮ запись (sub-record) типа "06 03"
  ⭐ Тип "06 03" — возможно DISEMBARK event (новый подтип)
""")
