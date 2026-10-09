#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0013 → 0014.

Контекст:
  - Герой достал из замка (был внутри, сейчас снаружи, как гость)
  - Купил артефакт за ресурсы (дерево)

Интересные кластеры:
  - 0x820d2 / 0x820d8: hero state in town?
  - 0x13e408: action flag changed (0x04 → 0x05, 0x58 → 0x83)
  - 0x13e469: counter changed (0xa4 → 0x32)
  - 0x13f647: 2 bytes swapped
  - 0x143e6e: 8 bytes — appeared two u32 LE: 100 (0x64) and 103 (0x67)
  - 0x16368b / 0x1636ae: flag changes
  - 0x163865: appeared u32 = 0x72 (114)
  - 0x16a2d4: u32 disappeared (0x72 → 0xffffffff)
  - 0x17ec37: counter +1
  - 0x18179d: new path-like record appeared (19 bytes)
  - 0x1827a7: HD3 footer shifted
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

A = load('/home/z/my-project/upload/0013.GM1')
B = load('/home/z/my-project/upload/0014.GM1')
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
    (0x000820d2, 0x000820d9, "Hero town state"),
    (0x0013e408, 0x0013e40f, "Action flag block"),
    (0x0013e469, 0x0013e46a, "Counter (day?)"),
    (0x0013f647, 0x0013f649, "Player resource (wood?)"),
    (0x00143e6e, 0x00143e76, "⭐ Hero coordinates appeared"),
    (0x0016368b, 0x0016368c, "Hero/town flag"),
    (0x001636ae, 0x001636af, "Hero/town flag"),
    (0x00163865, 0x00163869, "u32 appeared (artifact ID?)"),
    (0x0016a2d4, 0x0016a2d8, "u32 disappeared (artifact moved?)"),
    (0x0017ec37, 0x0017ec38, "Counter"),
    (0x0018179d, 0x001817b0, "⭐ New path-like record"),
    (0x001817e1, 0x001817f4, "Path block shifts"),
]

print("\n" + "="*80)
print("ДЕТАЛЬНЫЙ ДАМП КАЖДОГО КЛАСТЕРА")
print("="*80)

for cs, ce, label in CLUSTERS:
    ctx_start = max(0, cs - 48)
    ctx_end = min(len(A), ce + 64)
    
    print(f"\n{'='*80}")
    print(f"📍 {label}: 0x{cs:08x}..0x{ce:08x} (изм {ce-cs}b)")
    print(f"{'='*80}")
    print("A (0013, герой внутри замка):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0014, герой снаружи как гость, купил артефакт):")
    print(hex_dump(B, ctx_start, ctx_end))

# === Специфичные интерпретации ===
print("\n" + "="*80)
print("ИНТЕРПРЕТАЦИЯ")
print("="*80)

print("""
[1] 0x000003b6: '3' → '4'  — последний символ имени файла (0013 → 0014)

[2] 0x000820d2: 0x62 → 0x22  — hero state in town changed
   0x000820d8: 0x0a → 0x83  — движение/movement flag
   Это блок состояния героя ВНУТРИ замка. При выходе из замка значения меняются.

[3] 0x0013e408..0x0013e40f (7 байт):
   A: 04 58 03 01 58 0d ff
   B: 05 83 03 01 58 0d 83
   Изменились 2 байта:
     0x13e408: 0x04 → 0x05 (+1) — счётчик действий
     0x13e409: 0x58 → 0x83 (+43) — флаг текущего действия (покупка артефакта?)
     0x13e40f: 0xff → 0x83     — связанный флаг

[4] 0x0013e469: 0xa4 → 0x32
   u32 LE: A=0xa4 (164), B=0x32 (50), delta=-114
   Возможно: количество дерева у игрока (было 164, стало 50 — потратили 114)

[5] ⭐ 0x0013f647 (2b): 83 ff → ff 83
   Похоже на свап байтов или смещение в массиве

[6] ⭐ 0x00143e6e (8 байт):
   A: ff ff ff ff ff ff ff ff   (нет значения)
   B: 64 00 00 00 67 00 00 00   (появились u32 LE: 100 и 103)
   
   100 = 0x64, 103 = 0x67
   Это КООРДИНАТЫ! Герой теперь "гость города" — его позиция (100, 103)
   записалась в массиве "visiting heroes" или "garrison heroes"
   
   8 байт = 2 × u32 LE = (X=100, Y=103) или (hero_id=100, town_id=103)

[7] 0x0016368b: 0x00 → 0x01  — флаг "герой посетил город" установлен
   0x001636ae: 0x07 → 0x02  — счётчик/индекс героя в городе изменился

[8] ⭐ 0x00163865 (4b): ff ff ff ff → 72 00 00 00
   u32 LE: появилось значение 0x72 (114)
   Это может быть ID артефакта, купленного героем!
   
   ⭐ 0x0016a2d4 (4b): 72 00 00 00 → ff ff ff ff
   u32 LE: исчезло значение 0x72 (114) — АРТЕФАКТ ПЕРЕМЕЩЁН!
   
   ВЫВОД: Артефакт с ID=114 был в массиве "артефакты в продаже" (Black Market?)
   и перешёл в инвентарь героя. Магазин → герой.

[9] 0x0017ec37: 0xb8 → 0xb9 (+1) — общий счётчик событий +1

[10] ⭐ 0x0018179d (19 байт):
    A: 0b 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  (были нули)
    B: 09 03 83 00 00 00 03 03 59 00 7a 00 59 00 7a 00 00 00 0b
    
    Это НОВАЯ PATH-ЗАПИСЬ! Формат как в блоке 0x1814d0:
      09 03       — flags (type 2: "0b 03"?) — но здесь "09 03"
      83 00 00 00 — u32 LE = 0x83 = 131 (counter?)
      03          — N=3 (3 пары координат)
      59 00 7a 00 — (89, 122) — первая пара
      59 00 7a 00 — (89, 122) — вторая пара (дублирует)
      00 00 0b    — завершение?
    
    Возможно: hero посетил город (89, 122) — путь к городу.

[11] 0x001817e1 / 0x001817f3: '0' (0x30) перемещается на 18 байт вперёд
    Структура path-блока сдвинулась — добавилась новая запись.

[12] 0x001827a7..0x001827cb: HD3 footer снова сместился на +18 байт
    (размер файла вырос на 18 байт = размер новой path-записи)
""")

# === ГЛАВНЫЕ ВЫВОДЫ ===
print("="*80)
print("ИТОГ — КЛЮЧЕВЫЕ НАХОДКИ")
print("="*80)

print("""
✅ НАЙДЕНО:

1. ⭐ КООРДИНАТЫ ГОСТЯ ГОРОДА
   Смещение: 0x143e6e..0x143e76 (8 байт = 2 × u32 LE)
   A: 0xffffffff 0xffffffff (пусто — герой не гость)
   B: 0x00000064 0x00000067 (X=100, Y=103)
   → Это массив visiting/guest hero координат!
   Когда герой посещает город, сюда записываются его координаты.

2. ⭐ АРТЕФАКТ В ИНВЕНТАРЕ
   Смещение: 0x163865 (4b, u32 LE)
   A: 0xffffffff (нет артефакта)  →  B: 0x72 (114) — ID артефакта
   → Герой получил артефакт в инвентарь.

3. ⭐ АРТЕФАКТ ИЗ МАГАЗИНА
   Смещение: 0x16a2d4 (4b, u32 LE)
   A: 0x72 (114) — артефакт был в магазине
   B: 0xffffffff — артефакт ушёл из магазина
   → Подтверждение: артефакт ID=114 переехал из магазина в героя.

4. ⭐ ДЕРЕВО ИГРОКА
   Смещение: 0x13e469 (1 байт в u32 LE)
   A: 0xa4 (164)  →  B: 0x32 (50)
   Delta: -114 (потрачено 114 дерева на артефакт)
   ⚠️ 114 = ID артефакта? Или совпадение?
   Скорее: цена артефакта = 114 дерева, и ID артефакта = 114 — совпадение.

5. ⭐ ФЛАГ ГОСТЯ ГОРОДА
   Смещение: 0x16368b: 0x00 → 0x01  (герой теперь гость города)
   Смещение: 0x1636ae: 0x07 → 0x02  (slot hero changed)

6. ⭐ PATH-БЛОК РАСШИРИЛСЯ
   Добавилась новая запись в 0x18179d (19 байт):
   Тип "09 03" — новый подтип path-записи (отличается от "01 03" и "0b 03")
   
   Формат (предположительный):
     u8  flag1 = 0x09 (новый тип)
     u8  flag2 = 0x03
     u32 counter (LE) = 0x83 (131)
     u8  N = 3
     u16[N] pairs: (89, 122), (89, 122)  (дубликат?)
     ... padding/ending

ЛОКАЛИЗОВАННЫЕ НОВЫЕ СМЕЩЕНИЯ:
  0x143e6e: visiting hero coordinates (u32 LE × 2)
  0x16368b: visiting hero flag
  0x1636ae: hero slot in town
  0x163865: hero artifact inventory slot (u32 LE, artifact ID)
  0x16a2d4: black market / shop artifacts (u32 LE, artifact ID)
  0x18179d: path-block расширяемый (новый тип записи "09 03")
""")
