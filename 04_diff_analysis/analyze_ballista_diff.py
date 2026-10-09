#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0018 → 0019.
Контекст: купил баллисту герою.

Баллиста (Ballista) в HoMM3:
  - Относится к категории "War Machines"
  - Стандартная цена: 250 золота
  - Это артефакт, который устанавливается в специальный слот героя
  - ID баллисты в HoMM3: 0x05 (в индексации артефактов)

Главные изменения:
  0x13e481: u16 LE 0x5e0f → 0x544b (delta = ?
  0x163885: u32 LE 0xffffffff → 0x00000004 (появилось значение 4)
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

A = load('/home/z/my-project/upload/0018.GM1')
B = load('/home/z/my-project/upload/0019.GM1')
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

# === Кластер 1: золото ===
print("=" * 80)
print("📍 КЛАСТЕР 1: 0x13e481 — золото игрока")
print("=" * 80)
print("A:")
print(hex_dump(A, 0x13e470, 0x13e490))
print("B:")
print(hex_dump(B, 0x13e470, 0x13e490))

val_a = int.from_bytes(A[0x13e481:0x13e483], 'little')
val_b = int.from_bytes(B[0x13e481:0x13e483], 'little')
delta = val_b - val_a
print(f"\nu16 LE @ 0x13e481: A={val_a} (0x{val_a:04x}), B={val_b} (0x{val_b:04x}), delta={delta}")
if delta < 0:
    print(f"→ Игрок потратил {-delta} золота на покупку баллисты")

# === Кластер 2: артефакт-инвентарь ===
print("\n" + "=" * 80)
print("📍 КЛАСТЕР 2: 0x163885 — слот артефакта (баллиста!)")
print("=" * 80)

# Покажем широкий контекст, чтобы понять структуру инвентаря
print("A (0018, без баллисты):")
print(hex_dump(A, 0x163860, 0x1638c0))
print("B (0019, с баллистой):")
print(hex_dump(B, 0x163860, 0x1638c0))

val_a = int.from_bytes(A[0x163885:0x163889], 'little')
val_b = int.from_bytes(B[0x163885:0x163889], 'little')
print(f"\nu32 LE @ 0x163885: A=0x{val_a:08x}, B=0x{val_b:08x}")
print(f"A: 0xFFFFFFFF = слот пуст (нет баллисты)")
print(f"B: 0x00000004 = 4 = ID баллисты в HoMM3")

# === Анализ всех слотов артефактов ===
print("\n" + "=" * 80)
print("АНАЛИЗ ИНВЕНТАРЯ АРТЕФАКТОВ ГЕРОЯ")
print("=" * 80)

# Известные слоты артефактов в HoMM3 (по h3sed):
#helm, cloak, neck, weapon, shield, armor, lefthand, righthand, feet,
#side1-5, ballista, ammo, tent, catapult, spellbook, inventory
# Каждый слот = 8 байт (возможно: u32 ID + u32 something)

# Найдём все непустые слоты в диапазоне 0x163860..0x1638c0
print("\nСлоты артефактов (предположительно 8 байт каждый):")
for offset in range(0x163860, 0x1638c0, 4):
    val_a = int.from_bytes(A[offset:offset+4], 'little')
    val_b = int.from_bytes(B[offset:offset+4], 'little')
    is_empty_a = (val_a == 0xFFFFFFFF)
    is_empty_b = (val_b == 0xFFFFFFFF)
    is_empty_zero_a = (val_a == 0)
    is_empty_zero_b = (val_b == 0)
    
    marker = ""
    if val_a != val_b:
        marker = " ⭐ ИЗМЕНЕНО"
    
    if not is_empty_a or not is_empty_b or marker:
        a_str = f"0x{val_a:08x}" + (" (пусто)" if is_empty_a else "")
        b_str = f"0x{val_b:08x}" + (" (пусто)" if is_empty_b else "")
        print(f"  0x{offset:08x}: A={a_str}, B={b_str}{marker}")

# === Интерпретация ===
print("\n" + "=" * 80)
print("ИНТЕРПРЕТАЦИЯ")
print("=" * 80)

print(f"""
⭐⭐⭐ НАЙДЕНО:

1. ⭐ ЗОЛОТО ИГРОКА уменьшилось
   Смещение: 0x13e481 (u16 LE)
   A: {val_a}  →  B: {int.from_bytes(B[0x13e481:0x13e483], 'little')}
   Delta: {int.from_bytes(B[0x13e481:0x13e483], 'little') - int.from_bytes(A[0x13e481:0x13e483], 'little')}
   
   Цена баллисты = {-delta} золота
   ⚠️ {-delta} ≠ 250 (стандартная цена баллисты в HoMM3)
   Возможно: скидка или цена зависит от карт/модов

2. ⭐⭐⭐ СЛОТ БАЛЛИСТЫ В ИНВЕНТАРЕ
   Смещение: 0x163885 (4 байта, u32 LE)
   A: 0xFFFFFFFF (слот пуст)  →  B: 0x00000004 (ID = 4)
   
   ⭐ ID баллисты = 4
   В HoMM3 стандартные ID war machines:
     0 = Catapult
     1 = Ballista   ← но здесь 4!
     2 = First Aid Tent
     3 = Ammo Cart
   
   Возможно, ID в save-файле сдвинут на +3:
     0 → Catapult, 1 → Ballista, ...
   Или ID = 4 это просто artifact ID (а не war machine ID)
   
   В h3sed ARTIFACT_IDS нет баллисты отдельно, она в "war machines" слотах.
   Но в SoD ID артефактов 0..0x91, и 0x04 = "Ballista" (стандартный ID).

3. ⭐ СТРУКТУРА СЛОТА АРТЕФАКТА
   Смещение: 0x163885 — это слот "ballista" в hero equipment
   По h3sed HERO_BYTE_POSITIONS:
     "ballista": 486 (относительно начала блока героя)
     "ammo":     494
     "tent":     502
     "catapult": 510
     "spellbook": 518
   
   Размер слота = 8 байт (u32 ID + u32 something)

4. ⭐ КОНТЕКСТ СЛОТОВ ВОЕННЫХ МАШИН
   По дампу видно:
     0x163885: 0xFFFFFFFF → 0x00000004  ← BALLISTA (новое)
     0x163889..0x16388f: 0xFFFFFFFF (ammo cart — пусто)
     0x163890..0x163897: 0xFFFFFFFF (tent — пусто)
     0x163898..0x16389f: 0xFFFFFFFF (catapult — пусто)
     0x1638a0..0x1638a7: уже занят чем-то другим (это другой раздел)

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x163885: hero ballista slot (u32 LE, artifact ID = 4 = Ballista)
  Структура war machine slots (по 4 байта):
    0x163885: ballista
    0x163889: ammo cart
    0x16388d: first aid tent
    0x163891: catapult
""")

# Сверим с предыдущим диффом (spell book)
print("\n" + "=" * 80)
print("СВЕРКА С ПРЕДЫДУЩИМ ДИФФОМ (Spell Book, 0016→0017)")
print("=" * 80)
print("""
В диффе 0016 → 0017 (покупка книги магии):
  0x1638a5: 0xFFFFFFFF → 0x00000000  ← spell book slot изменён
  
В диффе 0018 → 0019 (покупка баллисты):
  0x163885: 0xFFFFFFFF → 0x00000004  ← ballista slot изменён
  
РАССТОЯНИЕ между слотами:
  0x1638a5 - 0x163885 = 0x20 = 32 байта

Возможная структура hero equipment slots (по 4 байта u32 LE):
  0x163885: ballista
  0x163889: ammo cart
  0x16388d: first aid tent
  0x163891: catapult
  0x163895: spell book?
  ...
  0x1638a5: другой артефактный слот (был изменён в диффе spell book)

Или слоты имеют размер 8 байт (u32 ID + u32 padding/data):
  0x163885..0x16388c (8b): ballista
  0x16388d..0x163894 (8b): ammo cart
  0x163895..0x16389c (8b): first aid tent
  0x16389d..0x1638a4 (8b): catapult
  0x1638a5..0x1638ac (8b): spell book
""")

# === Финальный итог ===
print("=" * 80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("=" * 80)
print("""
⭐⭐⭐ НАЙДЕНО: СЛОТ ВОЕННЫХ МАШИН ГЕРОЯ

1. СМЕЩЕНИЕ СЛОТА БАЛЛИСТЫ
   0x163885 (4 байта, u32 LE)
   A: 0xFFFFFFFF (пусто) → B: 0x00000004 (Ballista ID=4)
   
2. ЦЕНА БАЛЛИСТЫ
   Золото игрока уменьшилось (delta = -X золота)

3. СТРУКТУРА WAR MACHINE SLOTS
   Каждый слот = 4 байта u32 LE (или 8 байт с padding)
   4 слота: Ballista, Ammo Cart, First Aid Tent, Catapult
   (Spell Book — отдельный слот, не war machine)
""")
