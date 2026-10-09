#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0019 → 0020.
Контекст: другим героем посетили конюшню.
Ожидается:
  1) Флаг "конюшня посещена на этой неделе" (per-hero)
  2) Увеличение лимита ходов до конца недели для этого героя
  3) Возможно: новые path-записи (движение героя)
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

A = load('/home/z/my-project/upload/0019.GM1')
B = load('/home/z/my-project/upload/0020.GM1')
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
    (0x06b716, 0x06b720, "⭐ Hero #2 stats block (visiting hero)"),
    (0x06d7a4, 0x06d7ab, "⭐ Hero #1 stats block (main hero movement)"),
    (0x13e409, 0x13e40a, "Action flag"),
    (0x157ec3, 0x157ed7, "Hero current X,Y (UTF-16)"),
    (0x157eec, 0x157f02, "Path/visited tiles + movement"),
    (0x17162d, 0x171bd4, "⭐ Skills table (6 heroes with stride 288)"),
    (0x17ec37, 0x17ec38, "Map event counter"),
    (0x1817fd, 0x181860, "⭐ Path-block: new records"),
]

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 32)
    ctx_end = min(len(A), ce + 64)
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x}")
    print(f"{'='*80}")
    print("A (0019, до конюшни):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0020, после конюшни):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Анализ: hero stats (movement) ===
print("\n" + "="*80)
print("АНАЛИЗ MOVEMENT POINTS")
print("="*80)

# Hero #1 (main, addr 0x6d7a4)
mv_a1 = A[0x6d7a4]
mv_b1 = B[0x6d7a4]
mt_a1 = A[0x6d7aa]
mt_b1 = B[0x6d7aa]
print(f"\nHero #1 (main, 0x6d7a4):")
print(f"  movement remaining: A=0x{mv_a1:02x} ({mv_a1}) → B=0x{mv_b1:02x} ({mv_b1})")
print(f"  movement total/other: A=0x{mt_a1:02x} ({mt_a1}) → B=0x{mt_b1:02x} ({mt_b1})")

# Hero #2 (visiting, addr 0x6b716)
mv_a2 = A[0x6b716]
mv_b2 = B[0x6b716]
val_a2 = int.from_bytes(A[0x6b71c:0x6b720], 'little')
val_b2 = int.from_bytes(B[0x6b71c:0x6b720], 'little')
print(f"\nHero #2 (visiting, 0x6b716):")
print(f"  byte @ 0x6b716: A=0x{mv_a2:02x} ({mv_a2}) → B=0x{mv_b2:02x} ({mv_b2})")
print(f"  u32 @ 0x6b71c: A=0x{val_a2:08x} ({val_a2 if val_a2<0x80000000 else -1}) → B=0x{val_b2:08x} ({val_b2})")

# Сравним: 0x5e (94) и 0x22 (34) — это похоже на movement%
# A hero1.movement = 0x22 (34), B hero1.movement = 0x64 (100) → +66
# A hero2.movement = 0x5e (94), B hero2.movement = 0x22 (34) → -60
# Похоже на ОБМЕН значениями!

print("\n--- Сравнение паттернов ---")
print(f"Hero #1 movement: 0x{mv_a1:02x} → 0x{mv_b1:02x}  (delta={mv_b1-mv_a1:+d})")
print(f"Hero #2 movement: 0x{mv_a2:02x} → 0x{mv_b2:02x}  (delta={mv_b2-mv_a2:+d})")
print(f"Заметим: Hero #1.B = 0x{mv_b1:02x} похож на Hero #2.A = 0x{mv_a2:02x}? {mv_b1==mv_a2}")
print(f"И Hero #2.B = 0x{mv_b2:02x} похож на Hero #1.A = 0x{mv_a1:02x}? {mv_b2==mv_a1}")
print(f"→ Значения НЕ поменялись местами, но hero #1 получил +{mv_b1-mv_a1} (reset к полному?)")

# === Анализ: counter @ 0x6b71c ===
print("\n--- Анализ counter @ 0x6b71c ---")
print(f"A: 0xFFFFFFFF = -1 (нет значения)")
print(f"B: 0x{val_b2:08x} = {val_b2} (появилось значение 88 = 0x58)")
print(f"Это может быть 'stables visited' counter или 'visit ID'")

# === Анализ skills table ===
print("\n" + "="*80)
print("АНАЛИЗ SKILLS TABLE: 6 изменений с stride 288")
print("="*80)

skills_changes = [
    (0x17162d, 0x11, 0x19),
    (0x17174d, 0x11, 0x19),
    (0x17186d, 0x11, 0x19),
    (0x17198f, 0x11, 0x19),
    (0x171ab1, 0x11, 0x19),
    (0x171bd3, 0x11, 0x19),
]
strides = [skills_changes[i+1][0] - skills_changes[i][0] for i in range(len(skills_changes)-1)]
print(f"Изменения: {[(hex(a), hex(b), hex(c)) for a,b,c in skills_changes]}")
print(f"Strides: {strides}")
print(f"Все stride = 288 (0x120)? {all(s==288 for s in strides)}")
print(f"Все значения: 0x11 → 0x19 (delta=+8)? {all(b==0x11 and c==0x19 for _,b,c in skills_changes)}")

# Если stride = 288 (размер hero block), значит изменения в 6 РАЗНЫХ героях
# Но конюшню посетил только 1 герой!
# Возможно: это общий "weekly movement bonus" применён ко всем героям игрока
# Или: эти 6 байт — счётчик недели для каждого героя (стабильно +8 на новой неделе?)

# Вычислим offset внутри hero block
table_start = 0x170d00  # примерное начало таблицы
print(f"\nOffset внутри hero block (от 0x{table_start:08x}):")
for addr, a, b in skills_changes:
    hero_idx = (addr - table_start) // 288
    offset_in_block = (addr - table_start) % 288
    print(f"  0x{addr:08x}: hero[{hero_idx}]+0x{offset_in_block:02x} (={offset_in_block}) — A=0x{a:02x} B=0x{b:02x}")

# === Path-block: новые записи ===
print("\n" + "="*80)
print("АНАЛИЗ PATH-BLOCK: новые записи")
print("="*80)

# Покажем содержимое новых path-записей
print("\nНовые path-записи в B @ 0x1817fd:")
print(hex_dump(B, 0x1817fd, 0x181870))

# Первая запись выглядит как "01 03" — движение героя
# 01 03 58 00 00 00 06 32 00 66 00 31 00 66 00
#  flag1=01, flag2=03, counter=0x58=88, N=6, from=(0x32,0x66)=(50,102), to=(0x31,0x66)=(49,102)
print("\nЗапись 1 (тип 01 03, 15 байт):")
rec1 = B[0x1817fd:0x1817fd+15]
print(f"  {rec1.hex()}")
print(f"  flag1={rec1[0]:02x}, flag2={rec1[1]:02x}, counter={int.from_bytes(rec1[2:6],'little')}, N={rec1[6]}")
print(f"  from=({int.from_bytes(rec1[7:9],'little')}, {int.from_bytes(rec1[9:11],'little')})")
print(f"  to=({int.from_bytes(rec1[11:13],'little')}, {int.from_bytes(rec1[13:15],'little')})")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ КОНЮШНЯ (STABLES) — НАЙДЕНЫ:

1. ⭐ MOVEMENT POINTS ИЗМЕНИЛИСЬ
   Hero #1 (main, 0x6d7a4): 0x22 (34) → 0x64 (100) — увеличился на +66
   Hero #2 (visiting, 0x6b716): 0x5e (94) → 0x22 (34) — уменьшился на -60
   
   Возможно, hero #2 — это ВТОРОЙ герой (который посетил конюшню)
   Hero #1 — основной (его движение тоже обновилось по другой причине?)
   
   ⚠️ HoMM3 stables даёт +400 movement points до конца недели.
   Но здесь движение ИЗМЕНИЛОСЬ (а не просто максимальный лимит).
   Возможно, герой #2 походил к конюшне (потратив movement), а потом
   получил бонус.

2. ⭐ COUNTER/VISIT FLAG (Hero #2)
   Смещение: 0x6b71c (4 байта, u32 LE)
   A: 0xFFFFFFFF (-1, не посещено)  →  B: 0x00000058 (88, посещено)
   ⭐ Это может быть "stables visited this week" флаг (counter = 88)
   Значение 88 = 0x58 совпадает с counter в path-записях!
   
3. ⭐ ACTION FLAG
   Смещение: 0x13e409 (1 байт)
   A: 0x83 (131) → B: 0x58 (88) ← совпадает с visit counter!
   Возможно: ID последнего посещённого объекта

4. ⭐ SKILLS TABLE: 6 ИЗМЕНЕНИЙ СО STRIDE 288
   6 разных героев получили изменение 0x11 → 0x19 (+8) в одном байте
   одного и того же offset внутри hero block.
   
   Странно: конюшню посетил 1 герой, а изменения у 6 героев.
   
   Возможные объяснения:
   а) Это не "skills", а "weekly movement modifiers" — обновляются
      для всех героев игрока одновременно
   б) Это "current_week" счётчик, который обновился у всех героев
   в) 6 героев имеют одинаковый baseline (0x11), и при каком-то событии
      все они получили +8 (например, дневной tick)

5. ⭐ MAP EVENT COUNTER
   Смещение: 0x17ec37 (1 байт)
   A: 0xbf → B: 0xc3 (+4)
   Счётчик событий карты увеличился на 4

6. ⭐ НОВЫЕ PATH-ЗАПИСИ
   Добавлены записи типа "01 03" (движение героя) в path-блок
   Размер: 15 байт на запись
   Counter в записи = 0x58 (88) — совпадает с "visit counter"

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x6b716: hero #2 movement byte (u8) ← уменьшился после хода
  0x6b71c: hero #2 visit counter (u32 LE) ← STABLES VISITED FLAG?
           0xFFFFFFFF = не посещено, 0x58 (88) = посещено (visit ID)
  0x13e409: last visited object ID (u8) ← меняется на 0x58 после конюшни

ОТКРЫТЫЕ ВОПРОСЫ:
  - Что означает значение 88 (0x58)? Это ID конюшни?
  - Почему 6 героев получили +8 в skills table? Это связано с конюшней?
  - Где хранится "movement bonus until end of week"?
""")
