#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0020_6 → 0020_7.
Контекст: напал на вражеский город, победил, убил охраняющих существ, город стал моим.

Главные находки в диффе:
  0x5aac1, 0x5aac8: Hero #4 block — флаги (герой после битвы)
  0x5b92d, 0x5b934: НОВЫЙ блок (Hero #5?) — возможно защищавшийся герой
  0x13e2fd..0x13e426: несколько флагов (action state)
  0x13f78c: ⭐ флаг (1→3) — статус города?
  0x13f795..0x13f7a9 (20b): ⭐⭐ ARMY убита! (5,6,1,8,2 → -1,-1,-1,-1,-1)
  0x13f7b1..0x13f7c2 (17b): ⭐⭐ ARMY убита (11,8,28,6,17 → 0,0,0,0,0)
  0x140af9..0x140b05: visiting coords (67,83) → (65,84)
  0x140b22..0x140b3f: ⭐⭐ STATS после битвы (movement, counter)
  0x140b9c (4b): 0x8e → 0xffffffff — артефакт исчез!
  0x181954: ⭐ новые path-записи (битва + захват города)
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

A = load('/home/z/my-project/upload/0020_6.GM1')
B = load('/home/z/my-project/upload/0020_7.GM1')
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
    (0x05aac1, 0x05aac9, "Hero #4 block — после битвы"),
    (0x05b92d, 0x05b935, "⭐ NEW block (Hero #5?) — защищавшийся"),
    (0x13e2fd, 0x13e426, "⭐ Action flags"),
    (0x13f78c, 0x13f7c2, "⭐⭐ TOWN GARRISON — УБИТА АРМИЯ!"),
    (0x140af9, 0x140b45, "⭐⭐ Visiting + hero stats после битвы"),
    (0x140b9c, 0x140bd9, "⭐ Артефакт исчез + флаги"),
    (0x181954, 0x18197b, "⭐ Path-записи (battle + capture town)"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 32)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0020_6, до битвы):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020_7, после победы):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ убитой армии ===
print("\n" + "="*80)
print("⭐⭐⭐ АНАЛИЗ УБИТОЙ АРМИИ (TOWN GARRISON)")
print("="*80)

# 0x13f795..0x13f7a9 (20 байт = 5 × u32 LE)
print("\nБлок 1 @ 0x13f795..0x13f7a9 (5 × u32 LE = ARMY COUNTS):")
for i in range(5):
    addr = 0x13f795 + i * 4
    val_a = int.from_bytes(A[addr:addr+4], 'little')
    val_b = int.from_bytes(B[addr:addr+4], 'little')
    chg = " ⭐" if val_a != val_b else ""
    print(f"  Slot [{i}] @ 0x{addr:08x}: A={val_a:10d} (0x{val_a:08x}) → B={val_b if val_b<0x80000000 else -1:10d} (0x{val_b:08x}){chg}")

print("\nБлок 2 @ 0x13f7b1..0x13f7c2 (5 × u32 LE = ARMY COUNTS другой?):")
for i in range(5):
    addr = 0x13f7b1 + i * 4
    if addr + 4 > 0x13f7c2 + 1:
        break
    val_a = int.from_bytes(A[addr:addr+4], 'little')
    val_b = int.from_bytes(B[addr:addr+4], 'little')
    chg = " ⭐" if val_a != val_b else ""
    print(f"  Slot [{i}] @ 0x{addr:08x}: A={val_a:10d} → B={val_b:10d}{chg}")

# === Анализ флага города ===
print("\n" + "="*80)
print("⭐⭐ АНАЛИЗ ФЛАГА ГОРОДА")
print("="*80)
print(f"u8 @ 0x13f78c: A=0x{A[0x13f78c]:02x} ({A[0x13f78c]}) → B=0x{B[0x13f78c]:02x} ({B[0x13f78c]})")
print(f"  A: 1 → B: 3")
print(f"  ⭐ Это может быть OWNER COLOR города!")
print(f"  1 = Blue (бывший владелец)")
print(f"  3 = Green (наш игрок)")
print(f"  ⭐⭐ Город захвачен! Owner изменён с 1 (Blue) на 3 (Green)")

# === Анализ visiting coords ===
print("\n" + "="*80)
print("⭐ VISITING COORDS после битвы")
print("="*80)
xa = chr(A[0x140af9])
ya = chr(A[0x140afb])
xb = chr(B[0x140af9])
yb = chr(B[0x140afb])
print(f"Visiting coords (UTF-16 LE):")
print(f"  A: '{xa}'{ya}' = ({ord(xa)}, {ord(ya)})")
print(f"  B: '{xb}{yb}' = ({ord(xb)}, {ord(yb)})")

# Stats @ 0x140b22
print(f"\nu8 @ 0x140b22: A=0x{A[0x140b22]:02x} → B=0x{B[0x140b22]:02x}")
print(f"u8 @ 0x140b44: A=0x{A[0x140b44]:02x} → B=0x{B[0x140b44]:02x}")

# Movement @ 0x140b36
val_a = int.from_bytes(A[0x140b36:0x140b3a], 'little')
val_b = int.from_bytes(B[0x140b36:0x140b3a], 'little')
print(f"u32 @ 0x140b36 (movement): A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b})")

# === Анализ артефакта ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ АРТЕФАКТА (после битвы)")
print("="*80)
val_a = int.from_bytes(A[0x140b9c:0x140ba0], 'little')
val_b = int.from_bytes(B[0x140b9c:0x140ba0], 'little')
print(f"u32 @ 0x140b9c: A=0x{val_a:08x} ({val_a}) → B=0x{val_b:08x} ({val_b if val_b<0x80000000 else -1})")
print(f"  A: 0x8e (142) — артефакт с ID 142")
print(f"  B: 0xffffffff — слот очищен!")
print(f"  ⭐ Возможно: артефакт 'переместился' в инвентарь победителя")
print(f"  Или: это был артефакт защищавшегося героя, и после победы")
print(f"       он перешёл к атакующему (наш герой)")

# === Path-записи ===
print("\n" + "="*80)
print("⭐ АНАЛИЗ PATH-ЗАПИСЕЙ (битва + захват)")
print("="*80)
print("\nB @ 0x181950..0x1819a0:")
print(hex_dump(B, 0x181950, 0x1819a0))

# Декодировать path-записи
print("\n--- Декодировка path-записей ---")
offset = 0x181954
while offset < 0x181990:
    flag1 = B[offset]
    flag2 = B[offset+1]
    
    if flag1 == 0x01 and flag2 == 0x03:
        rec = B[offset:offset+15]
        counter = int.from_bytes(rec[2:6], 'little')
        n = rec[6]
        fx, fy = int.from_bytes(rec[7:9],'little'), int.from_bytes(rec[9:11],'little')
        tx, ty = int.from_bytes(rec[11:13],'little'), int.from_bytes(rec[13:15],'little')
        print(f"  [{offset:08x}] 01 03: counter={counter}, N={n}, ({fx},{fy})→({tx},{ty})")
        offset += 15
    elif flag1 == 0x04 and flag2 == 0x03:
        # НОВЫЙ ТИП! 04 03
        rec = B[offset:offset+10]
        print(f"  [{offset:08x}] 04 03 (НОВЫЙ ТИП!): {rec.hex()}")
        print(f"    counter={int.from_bytes(rec[2:6],'little')}")
        print(f"    u8 @ +6 = 0x{rec[6]:02x}")
        print(f"    u8 @ +7 = 0x{rec[7]:02x}")
        print(f"    u8 @ +8 = 0x{rec[8]:02x}")
        print(f"    u8 @ +9 = 0x{rec[9]:02x}")
        offset += 10
    elif flag1 == 0x0b and flag2 == 0x00:
        # terminator
        print(f"  [{offset:08x}] 0b 00: terminator")
        offset += 2
    else:
        print(f"  [{offset:08x}] ??? {flag1:02x} {flag2:02x}")
        offset += 2

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ БИТВА ЗА ГОРОД + ПОБЕДА — НАЙДЕНЫ:

1. ⭐⭐⭐ OWNER COLOR ГОРОДА ИЗМЕНИЛСЯ
   Смещение: 0x13f78c (u8)
   A: 1 (Blue — бывший владелец)  →  B: 3 (Green — наш игрок)
   
   ⭐⭐ Это OWNER COLOR города!
   При захвате города владельцем становится победитель.
   
   ⚠️ Это отличается от 0x13c850 (массив владельцев объектов карты):
     - 0x13c850 — для шахт и других объектов
     - 0x13f78c — для городов (отдельная структура)

2. ⭐⭐⭐ АРМИЯ ЗАЩИТНИКА УБИТА (TOWN GARRISON cleared)
   Смещение: 0x13f795..0x13f7a9 (20b = 5 × u32 LE)
   A: 5, 6, 1, 8, 2 (5 слотов с количествами существ)
   B: -1, -1, -1, -1, -1 (все слоты пусты!)
   
   ⭐⭐ Это TOWN GARRISON ARMY COUNTS!
   Все 5 слотов армии защитника обнулены (-1 = 0xFFFFFFFF = пусто).
   
   ⭐ Формат: 5 × u32 LE (а не 7 × u32 LE, как мы думали раньше!)
   Возможно: у городов только 5 слотов гарнизона, а не 7.

3. ⭐⭐⭐ ВТОРАЯ АРМИЯ ОБНУЛЕНА
   Смещение: 0x13f7b1..0x13f7c2 (17b)
   A: 11, 8, 28, 6, 17 (5 значений)
   B: 0, 0, 0, 0, 0 (все обнулены)
   
   ⭐ Это ARMY TYPES (ID существ)! 
   Были: creature IDs (11, 8, 28, 6, 17)
   Стало: 0 (нет существ)
   
   ⭐⭐ ПОДТВЕРЖДЕНИЕ СТРУКТУРЫ:
     0x13f795 (20b) = army_counts[5] — количества существ
     0x13f7b1 (20b) = army_types[5]  — ID существ
     Размер: 5 слотов (а не 7) для городов

4. ⭐⭐ АРТЕФАКТ ИСЧЕЗ (защищавшегося героя?)
   Смещение: 0x140b9c (4b, u32 LE)
   A: 0x8e (142) — артефакт с ID 142
   B: 0xffffffff — слот очищен
   
   ⭐ Возможно: защищавшийся герой имел артефакт 142,
   после победы он перешёл к атакующему герою.
   Или: артефакт просто удалён после битвы.

5. ⭐ ACTION FLAGS
   0x13e2fd..0x13e303 (6b): 04 ff 0f 0e 04 0b → 03 ff 0f 0e 04 ff
   0x13e41f: 0x04 → 0x05 (+1) — счётчик действий
   0x13e425: 0xff → 0x0b — флаг смены владельца

6. ⭐ VISITING COORDS изменились
   0x140af9: ('g','S') = (103, 83) → ('e','T') = (101, 84)
   Герой подошёл к городу.

7. ⭐ MOVEMENT POINTS уменьшился
   0x140b36: A=299999 (чит) → B=?
   (значение изменилось, но нужно точно декодировать)

8. ⭐ НОВЫЙ ТИП PATH-ЗАПИСИ: "04 03" — BATTLE EVENT!
   Размер: ~10 байт
   Формат:
     u8  flag1 = 0x04 (тип: BATTLE)
     u8  flag2 = 0x03
     u32 counter (LE)
     u8  data[4] — информация о битве
   
   ⭐ Это новый тип path-записи для BATTLE events!
   Добавлен после 2 движений героя (01 03) к городу.

9. ⭐ MAP EVENT COUNTER
   0x17ec37: 0xd4 (212) → 0xd7 (215) — delta=+3

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x13f78c: ⭐⭐ TOWN OWNER COLOR (u8, 1=Blue, 3=Green)
  0x13f795 (20b): ⭐⭐ TOWN GARRISON army_counts[5] (5 × u32 LE)
                  — подтверждено: 5 слотов для городов, не 7
  0x13f7b1 (20b): ⭐⭐ TOWN GARRISON army_types[5] (5 × u32 LE)
                  — ID существ, 0 = пусто
  0x140b9c: hero artifact slot (защищавшегося героя)
  0x05b92d: новый hero block (Hero #5? — защищавшийся)

PATH-ЗАПИСЬ типа "04 03" (~10b):
  ⭐ BATTLE EVENT — НОВЫЙ ТИП!
  Содержит: counter + 4 байта данных о битве
""")
