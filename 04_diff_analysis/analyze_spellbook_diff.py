#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0016 → 0017.
Контекст: купил книгу магии герою → у него появились заклинания 1 и 2 уровня.

Ожидаемые изменения:
  - Золото игрока уменьшилось (книга магии стоит 500 золота в HoMM3)
  - Появился артефакт "Spell Book" в инвентаре героя (slot = 0x11)
  - Появились биты заклинаний в массиве spells героя
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

A = load('/home/z/my-project/upload/0016.GM1')
B = load('/home/z/my-project/upload/0017.GM1')
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
    (0x13e481, 0x13e483, "⭐ Золото игрока"),
    (0x163794, 0x163795, "Spell book flag?"),
    (0x1637a0, 0x1637a3, "Биты заклинаний (3 bytes)"),
    (0x1637af, 0x1637b7, "Биты заклинаний (8 bytes)"),
    (0x1637c0, 0x1637c1, "Spell bit"),
    (0x1637c6, 0x1637c7, "Spell bit"),
    (0x1637cf, 0x1637d0, "Spell bit"),
    (0x1637da, 0x1637db, "Spell bit"),
    (0x1637e6, 0x1637e9, "Биты заклинаний (3 bytes)"),
    (0x1637f5, 0x1637fd, "Биты заклинаний (8 bytes)"),
    (0x163806, 0x163807, "Spell bit"),
    (0x16380c, 0x16380d, "Spell bit"),
    (0x163815, 0x163816, "Spell bit"),
    (0x1638a5, 0x1638ad, "⭐ Инвентарь артефактов"),
]

print("\n" + "="*80)
print("ДЕТАЛЬНЫЙ ДАМП")
print("="*80)

# Покажем полный контекст spell bit array
print("\n📍 ⭐⭐⭐ SPELL BIT ARRAY (0x163790..0x163820):")
print("A (0016, без книги магии):")
print(hex_dump(A, 0x163790, 0x163820))
print("\nB (0017, с книгой магии и заклинаниями):")
print(hex_dump(B, 0x163790, 0x163820))

# === Инвентарь артефактов ===
print("\n📍 ⭐ АРТЕФАКТ-ИНВЕНТАРЬ (0x1638a0..0x1638c0):")
print("A:")
print(hex_dump(A, 0x1638a0, 0x1638c0))
print("B:")
print(hex_dump(B, 0x1638a0, 0x1638c0))

# === Золото ===
print("\n📍 ⭐ Золото игрока (0x13e481):")
print("A:")
print(hex_dump(A, 0x13e470, 0x13e490))
print("B:")
print(hex_dump(B, 0x13e470, 0x13e490))

val_a = int.from_bytes(A[0x13e481:0x13e483], 'little')
val_b = int.from_bytes(B[0x13e481:0x13e483], 'little')
print(f"\nu16 LE @ 0x13e481: A={val_a} (0x{val_a:04x}), B={val_b} (0x{val_b:04x}), delta={val_b-val_a}")

# === Структурный анализ spell bits ===
print("\n" + "="*80)
print("СТРУКТУРНЫЙ АНАЛИЗ SPELL BIT ARRAY")
print("="*80)

# Найдём все изменённые биты в диапазоне 0x163794..0x163816
print("\nИзменённые байты в spell bit array:")
spell_changes = []
for addr in range(0x163794, 0x163816):
    a = A[addr]
    b = B[addr]
    if a != b:
        delta_offset = addr - 0x163794
        # Какие биты установились
        new_bits = b & ~a
        spell_changes.append((addr, delta_offset, a, b, new_bits))
        print(f"  0x{addr:08x} (+0x{delta_offset:02x}): A=0x{a:02x} B=0x{b:02x}  new bits = 0x{new_bits:02x} = {bin(new_bits)[2:].zfill(8)}")

# В HoMM3 заклинания пронумерованы 0..70
# Если каждое заклинание = 1 бит, то 70 бит = 9 байт
# Если биты идут по уровням (5 уровней по ~14 заклинаний = 70 бит), то 5 секций по 14 бит = 5 × 2 байта = 10 байт

print(f"\nВсего изменённых байт: {len(spell_changes)}")

# === Структура spell bit array — попробуем угадать ===
print("\nГипотеза о структуре spell bit array:")
print("HoMM3 содержит 70 заклинаний (id 0..69)")
print("Каждый бит = 1 заклинание (0 = нет, 1 = есть в книге)")
print("70 бит = 9 байт (8 байт по 8 бит + 1 байт с 6 битами)")
print()
print("Изменённые байты имеют stride = ~9 (0x09):")
print("  0x1637a0 (+0x0c): bits 0x01 0x00 0x01 = 1, 0, 1")
print("  0x1637af (+0x1b): bits 0x01 0x00 0x00 0x00 0x00 0x01 0x00 0x01")
print("  0x1637c0 (+0x2c): bit 0x01")
print("  0x1637c6 (+0x32): bit 0x01")
print("  0x1637cf (+0x3b): bit 0x01")
print("  0x1637da (+0x46): bit 0x01")
print("  0x1637e6 (+0x52): bits 0x01 0x00 0x01")
print("  0x1637f5 (+0x61): bits 0x01 0x00 0x00 0x00 0x00 0x01 0x00 0x01")
print("  0x163806 (+0x72): bit 0x01")
print("  0x16380c (+0x78): bit 0x01")
print("  0x163815 (+0x81): bit 0x01")

# === Анализ артефакт-инвентаря ===
print("\n" + "="*80)
print("АНАЛИЗ ИНВЕНТАРЯ АРТЕФАКТОВ")
print("="*80)

# Покажем широкий диапазон инвентаря
print("\nПолный дамп 0x163860..0x1638c0:")
print("A:")
print(hex_dump(A, 0x163860, 0x1638c0))
print("B:")
print(hex_dump(B, 0x163860, 0x1638c0))

# Изменилось 0x1638a5..0x1638ad (8 байт)
# A: ff ff ff ff 5e 06 00 00
# B: 00 00 00 00 ff ff ff ff

print("\n0x1638a5..0x1638ad (8 bytes = 2 × u32 LE):")
val_a1 = int.from_bytes(A[0x1638a5:0x1638a9], 'little')
val_a2 = int.from_bytes(A[0x1638a9:0x1638ad], 'little')
val_b1 = int.from_bytes(B[0x1638a5:0x1638a9], 'little')
val_b2 = int.from_bytes(B[0x1638a9:0x1638ad], 'little')
print(f"  u32 #1 @ 0x1638a5: A=0x{val_a1:08x}, B=0x{val_b1:08x}")
print(f"  u32 #2 @ 0x1638a9: A=0x{val_a2:08x}, B=0x{val_b2:08x}")
print(f"\nИнтерпретация:")
print(f"  В A: slot1=0xffffffff (пусто), slot2=0x65e (artifact ID = 0x65e = 1630)")
print(f"  В B: slot1=0x00000000 (???), slot2=0xffffffff (пусто)")
print(f"  Видимо артефакт 0x65e (spell book!) переехал.")
print(f"  Проверим: spell book в HoMM3 имеет ID = 0..0x12, но в SoD ID = 0x0c?")

# === ИТОГ ===
print("\n" + "="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)
print("""
⭐⭐⭐ НАЙДЕНО:

1. ⭐ ЗОЛОТО ИГРОКА уменьшилось
   Смещение: 0x13e481 (u16 LE)
   A: 24579  →  B: 24335
   Delta = -244
   Внимание: 244 ≠ 500 (стандартная цена книги магии)
   Возможно: книга магии куплена за 244 золота (со скидкой?) или цена другой

2. ⭐⭐⭐ SPELL BIT ARRAY ГЕРОЯ
   Начало: 0x163794 (или 0x1637a0)
   Размер: ~146 байт (до 0x163826)
   Формат: массив битов, каждый бит = одно заклинание
   
   После покупки книги магии установились биты для заклинаний:
   - 11 байт изменились (с 0x00 на 0x01 или 0x01 0x00 0x01)
   - Заклинания 1 и 2 уровня (по словам игрока)
   
   Структура (предположительно):
     По 14 бит на уровень × 5 уровней = 70 бит = 9 байт
     Но stride между изменениями = 0x09 (9 байт) — это и есть stride одного уровня!
     
   Изменения в "блоках" по 9 байт:
     0x1637a0 + 0 = 0x1637a0: 01 00 01 (3 байта)
     0x1637a0 + 9 = 0x1637a9: ... (нет изменений)
     0x1637a0 + 18 = 0x1637b2: ... (нет изменений)
     ...
   
   Альтернативная гипотеза: spell bit array имеет структуру
     5 секций (по уровням 1-5), каждая по 14 бит = 2 байта + 6 бит padding
   
3. ⭐⭐ АРТЕФАКТ "SPELL BOOK" В ИНВЕНТАРЕ
   Смещение: 0x1638a5..0x1638ad (8 байт = 2 × u32 LE)
   A: 0xffffffff 0x0000065e  (slot1 пуст, slot2 = артефакт ID 0x65e)
   B: 0x00000000 0xffffffff  (slot1 = 0, slot2 пуст)
   
   ⚠️ Странное поведение: вместо "появился артефакт" мы видим "перемещение"
   Видимо, книга магии уже была в каком-то "промежуточном" слоте,
   а теперь переехала в основной слот.
   
   Или 0x00000000 ≠ "пусто", а "spell book placeholder".
   
   ID 0x65e = 1630 — это точно не HoMM3 ID артефакта (они 0..0x91)
   Возможно, это индекс в каком-то другом массиве (Black Market?)
   или ID артефакта в другой системе кодирования.

4. ⭐ SPELL BOOK OWNED FLAG
   Смещение: 0x163794 (1 байт, u8)
   A: 0x00 (нет книги)  →  B: 0x01 (есть книга)
   Это простой флаг "у героя есть Spell Book"

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x163794: hero has_spell_book flag (u8 bool)
  0x1637a0..0x163826: spell bit array (~146 байт)
    структура: 5 секций по уровням, каждая ~9 байт
    каждый бит = одно заклинание (70 заклинаний всего)
  
  0x1638a5: artifact slot (u32 LE) — перемещение spell book
""")
