#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Финальный анализ path-записей героя.
Известно:
- Файл B длиннее на 43 байта
- HD3 footer сместился на 43 байта (0x18255c → 0x182587)
- В кластере 9 (0x181552) появились новые path-записи

Цель: точно восстановить формат path-записей.
"""
import gzip, io, contextlib

@contextlib.contextmanager
def patch():
    original = getattr(gzip, '_GzipReader', None)
    if original is not None and hasattr(original, '_read_eof'):
        orig_read_eof = original._read_eof
        def patched(self):
            try: self._fp.read(8)
            except: pass
        original._read_eof = patched
        try: yield
        finally: original._read_eof = orig_read_eof
    else:
        yield

def load(p):
    with open(p, 'rb') as f:
        data = f.read()
    with patch():
        with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
            return gf.read()

A = load('/home/z/my-project/upload/001.GM1')
B = load('/home/z/my-project/upload/0011.GM1')

# === 1. Анализ path-записей ===
# В A последняя запись в path-блоке (перед нулями) была по адресу 0x181542:
# 01 03 0d 00 00 00 02 4f 00 6b 00 50 00 6b 00
# Затем нули. В B здесь появились новые записи.
# Размер stride = 15 байт на запись

print("=" * 80)
print("АНАЛИЗ PATH-ЗАПИСЕЙ ГЕРОЯ")
print("=" * 80)

# Найдём все path-записи — они начинаются с 01 03 или 0b 03
# Каждая запись: flag1(1b) flag2(1b) counter(4b LE) N(1b) X1(2b) Y1(2b) X2(2b) Y2(2b) = 15 байт
# (для записей с 01 03)

# Сегмент A
print("\n--- Сегмент A (0x1814d0 .. 0x1815b0, до изменений) ---")
seg_a = A[0x1814d0:0x1815b0]
print(f"Длина: {len(seg_a)} байт")

# Разбиваем на записи по 15 байт, начиная с известной позиции
# Найдём начало path-записей — ищем 01 03
i = 0
records_a = []
while i < len(seg_a) - 15:
    if seg_a[i] == 0x01 and seg_a[i+1] == 0x03:
        rec = seg_a[i:i+15]
        flag1, flag2 = rec[0], rec[1]
        counter = int.from_bytes(rec[2:6], 'little')
        N = rec[6]
        x1 = int.from_bytes(rec[7:9], 'little')
        y1 = int.from_bytes(rec[9:11], 'little')
        x2 = int.from_bytes(rec[11:13], 'little')
        y2 = int.from_bytes(rec[13:15], 'little')
        records_a.append({
            'addr': 0x1814d0 + i,
            'flag1': flag1, 'flag2': flag2, 'counter': counter, 'N': N,
            'from': (x1, y1), 'to': (x2, y2),
        })
        i += 15
    else:
        i += 1

print(f"\nНайдено path-записей в A: {len(records_a)}")
for r in records_a[-10:]:  # последние 10
    print(f"  @0x{r['addr']:08x}: flags={r['flag1']:02x},{r['flag2']:02x} counter={r['counter']:3d} N={r['N']:2d} "
          f"from=({r['from'][0]},{r['from'][1]}) → to=({r['to'][0]},{r['to'][1]})")

# Сегмент B
print("\n--- Сегмент B (0x1814d0 .. 0x1815d0) ---")
seg_b = B[0x1814d0:0x1815d0]
print(f"Длина: {len(seg_b)} байт (на {len(seg_b) - len(seg_a)} байт больше)")

# Аналогичный поиск
i = 0
records_b = []
while i < len(seg_b) - 15:
    if seg_b[i] == 0x01 and seg_b[i+1] == 0x03:
        rec = seg_b[i:i+15]
        flag1, flag2 = rec[0], rec[1]
        counter = int.from_bytes(rec[2:6], 'little')
        N = rec[6]
        x1 = int.from_bytes(rec[7:9], 'little')
        y1 = int.from_bytes(rec[9:11], 'little')
        x2 = int.from_bytes(rec[11:13], 'little')
        y2 = int.from_bytes(rec[13:15], 'little')
        records_b.append({
            'addr': 0x1814d0 + i,
            'flag1': flag1, 'flag2': flag2, 'counter': counter, 'N': N,
            'from': (x1, y1), 'to': (x2, y2),
        })
        i += 15
    elif seg_b[i] == 0x0b and seg_b[i+1] == 0x03:
        # Другой тип записи — попробуем другой формат
        # 0b 03 03 00 2f 00 61 00 13 00 1b 00 2f 00 62 00 11 00 19 00 2f 00 63 00 11 00 19 00 0b
        # 28 байт? Тогда 0b = начало, 0b в конце = конец следующей?
        # Или: 0b — тип, 03 — подтип, далее N + N пар координат
        # 03 00 — N=3? (3 пары координат)
        # 2f 00 61 00 — (47, 97)
        # 13 00 1b 00 — (19, 27) ?
        # 2f 00 62 00 — (47, 98)
        # 11 00 19 00 — (17, 25)
        # 2f 00 63 00 — (47, 99)
        # 11 00 19 00 — (17, 25)
        # Это другая структура: {flag1=0b flag2=03 N=03 pair1 pair2 pair3} - 2+4+N*8 = 30 байт?
        # Но у нас 28 байт до следующего 0b. N=03 → 3 пары по 8 байт = 24 + 4 header = 28 байт. Сходится!
        rec = seg_b[i:i+28]
        if len(rec) == 28:
            flag1, flag2 = rec[0], rec[1]
            N = rec[2]  # 3 пары
            pairs = []
            for j in range(N):
                x = int.from_bytes(rec[4+j*4:6+j*4], 'little')
                y = int.from_bytes(rec[6+j*4:8+j*4], 'little')
                pairs.append((x, y))
            records_b.append({
                'addr': 0x1814d0 + i,
                'flag1': flag1, 'flag2': flag2,
                'N': N, 'pairs': pairs,
                'format': 'multi-pair',
            })
            i += 28
        else:
            i += 1
    else:
        i += 1

print(f"\nНайдено path-записей в B: {len(records_b)}")
for r in records_b[-15:]:
    if r.get('format') == 'multi-pair':
        print(f"  @0x{r['addr']:08x}: flags={r['flag1']:02x},{r['flag2']:02x} N={r['N']} pairs={r['pairs']}")
    else:
        print(f"  @0x{r['addr']:08x}: flags={r['flag1']:02x},{r['flag2']:02x} counter={r['counter']:3d} N={r['N']:2d} "
              f"from=({r['from'][0]},{r['from'][1]}) → to=({r['to'][0]},{r['to'][1]})")

# === 2. Сравнение ===
print("\n" + "=" * 80)
print("СРАВНЕНИЕ PATH-ЗАПИСЕЙ")
print("=" * 80)

print(f"\nВ A: {len(records_a)} записей")
print(f"В B: {len(records_b)} записей (разница: {len(records_b) - len(records_a)})")

# Найдём новые записи в B
new_records = []
for rb in records_b:
    found = False
    for ra in records_a:
        if rb['addr'] == ra['addr']:
            found = True
            break
    if not found:
        new_records.append(rb)

print(f"\nНОВЫХ ЗАПИСЕЙ В B: {len(new_records)}")
for r in new_records:
    if r.get('format') == 'multi-pair':
        print(f"  @0x{r['addr']:08x}: flags={r['flag1']:02x},{r['flag2']:02x} N={r['N']} pairs={r['pairs']}")
    else:
        print(f"  @0x{r['addr']:08x}: flags={r['flag1']:02x},{r['flag2']:02x} counter={r['counter']:3d} N={r['N']:2d} "
              f"from=({r['from'][0]},{r['from'][1]}) → to=({r['to'][0]},{r['to'][1]})")

# === 3. ИТОГ ===
print("\n" + "=" * 80)
print("ВОССТАНОВЛЕННАЯ СТРУКТУРА PATH-ЗАПИСЕЙ")
print("=" * 80)
print("""
Обнаружено 2 типа записей в path-блоке (начинается ~0x1814d0):

ТИП 1: "01 03" — движение героя (один шаг)
  Размер: 15 байт
  Формат:
    u8  flag1     = 0x01
    u8  flag2     = 0x03
    u32 counter   (LE) — порядковый номер хода
    u8  N         — всегда 2 (2 координаты: from, to)
    u16 from_X    (LE)
    u16 from_Y    (LE)
    u16 to_X      (LE)
    u16 to_Y      (LE)
  
  Пример: 01 03 0d 00 00 00 03 4b 00 67 00 4c 00 68 00
    → ход #13, движение с (75, 103) на (76, 104)  [+1, +1]

ТИП 2: "0b 03" — составная запись (несколько пар координат)
  Размер: 4 + N*8 байт
  Формат:
    u8  flag1     = 0x0b
    u8  flag2     = 0x03
    u8  N         — количество пар координат
    u8  pad       = 0x00
    u16[N] pairs  (LE) — массив пар (X, Y)
  
  Пример: 0b 03 03 00 2f 00 61 00 13 00 1b 00 2f 00 62 00 11 00 19 00 2f 00 63 00 11 00 19 00
    → N=3 пары: (47,97)→(19,27), (47,98)→(17,25), (47,99)→(17,25)
    Возможно: fog of war updates — какие тайлы стали видимыми

ЛОКАЛИЗАЦИЯ:
  Hero position (X, Y) UTF-16 строки:        ~0x157ec3 (одиночные символы '5','b')
  Hero path (текущий):                       ~0x157eec (struct с counter, X, Y)
  Hero path (история движений):              ~0x1814d0 .. 0x1815d0 (path-записи)
  Footer маркер "HD3\\0" + CRC24/size:       последние ~18 байт файла
""")
