#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ изменений между 001.GM1 и 0011.GM1.

Известный контекст:
- Зелёный герой чуть прошёл по карте и открыл туман войны
- Карта: "Myth and Legend.h3m"
- Версия сейва: SoD 42.2

Цель: для каждого изменённого диапазона дать интерпретацию —
что это за поле, как оно кодируется, какая семантика.
"""
import sys, struct, re, json, gzip, io
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')

A_PATH = '/home/z/my-project/upload/001.GM1'
B_PATH = '/home/z/my-project/upload/0011.GM1'

def load(path):
    with open(path, 'rb') as f:
        data = f.read()
    # patch gzip _read_eof для HoMM3 saves
    import contextlib
    @contextlib.contextmanager
    def patch():
        original = getattr(gzip, '_GzipReader', None)
        if original is not None and hasattr(original, '_read_eof'):
            orig_read_eof = original._read_eof
            def patched_read_eof(self):
                try: self._fp.read(8)
                except: pass
            original._read_eof = patched_read_eof
            try: yield
            finally: original._read_eof = orig_read_eof
        else:
            yield
    with patch():
        with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
            raw = gf.read()
    return raw

A = load(A_PATH)
B = load(B_PATH)
print(f"A: {len(A)} bytes, B: {len(B)} bytes, diff: {len(B)-len(A)}")

# Все диапазоны изменений из диффа
RANGES = [
    (0x000003b6, 0x000003bb, "1.GM1 in filename"),
    (0x00068467, 0x00068469, ""),
    (0x0006846e, 0x00068472, ""),
    (0x00069599, 0x0006959b, ""),
    (0x000695a0, 0x000695a1, ""),
    (0x00130b71, 0x00130b80, ""),
    (0x00130b89, 0x00130bec, ""),
    (0x00130bf5, 0x00130c12, ""),
    (0x0013e409, 0x0013e40a, ""),
    (0x00157ec3, 0x00157ecd, ""),
    (0x00157ed3, 0x00157ed7, ""),
    (0x00157eec, 0x00157ef8, ""),
    (0x00157f00, 0x00157f01, ""),
    (0x00171093, 0x00171094, ""),
    (0x001711b3, 0x001711b4, ""),
    (0x001712d3, 0x001712d4, ""),
    (0x0017ec37, 0x0017ec38, ""),
    (0x00181552, 0x0018157e, ""),
    (0x00181596, 0x00181597, ""),
    (0x001815c1, 0x001815c2, ""),
    (0x0018255c, 0x00182563, ""),
    (0x0018256e, 0x00182599, ""),
]

def hex_dump(data, start, end, prefix="  "):
    """Печать hex+ASCII диапазона."""
    chunk = data[start:end]
    lines = []
    for i in range(0, len(chunk), 16):
        chunk_slice = chunk[i:i+16]
        hex_part = ' '.join(f'{b:02x}' for b in chunk_slice)
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in chunk_slice)
        lines.append(f"{prefix}{start+i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return '\n'.join(lines)

def interp_u32_le(data, off):
    return int.from_bytes(data[off:off+4], 'little', signed=False)

def interp_i32_le(data, off):
    return int.from_bytes(data[off:off+4], 'little', signed=True)

# === Группировка диапазонов в кластеры ===
print("\n" + "=" * 80)
print("ГРУППИРОВКА ИЗМЕНЕНИЙ В КЛАСТЕРЫ")
print("=" * 80)

# Объединим близкие диапазоны (< 1KB между ними)
clusters = []
cur_start, cur_end = RANGES[0][0], RANGES[0][1]
for s, e, _ in RANGES[1:]:
    if s - cur_end < 1024:
        cur_end = max(cur_end, e)
    else:
        clusters.append((cur_start, cur_end))
        cur_start, cur_end = s, e
clusters.append((cur_start, cur_end))

print(f"Кластеров: {len(clusters)}")
for i, (s, e) in enumerate(clusters):
    print(f"  Кластер {i+1}: 0x{s:08x} .. 0x{e:08x} (размер {e-s} байт)")

# === Анализ каждого кластера с окружением ===
print("\n" + "=" * 80)
print("ДЕТАЛЬНЫЙ АНАЛИЗ КАЖДОГО КЛАСТЕРА")
print("=" * 80)

for i, (cs, ce) in enumerate(clusters):
    # Окружение: ±128 байт
    ctx_start = max(0, cs - 128)
    ctx_end = min(len(A), ce + 128)
    
    print(f"\n{'='*80}")
    print(f"КЛАСТЕР {i+1}: 0x{cs:08x} .. 0x{ce:08x} (изменено {ce-cs} байт)")
    print(f"Контекст: 0x{ctx_start:08x} .. 0x{ctx_end:08x}")
    print(f"{'='*80}")
    
    print("\nФайл A (ДО):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("\nФайл B (ПОСЛЕ):")
    print(hex_dump(B, ctx_start, ctx_end))
    
    # Специфичные интерпретации
    print("\nИнтерпретация:")
    
    if cs == 0x000003b6:
        print("  → Заголовок сейва: имя файла (.GM1)")
        print("    Было: '...001.GM1\\0' → Стало: '...0011.GM1\\0'")
        print("    ВЫВОД: смещение 0x3b6 — это имя файла сейва в заголовке (длина ~9 байт)")
    
    elif cs == 0x00068467:
        print("  → Кандидат: КОПИРОВАНИЕ ДАННЫХ ИЗ ОДНОГО БЛОКА В ДРУГОЙ")
        print("    A (0x68467): 00 00          B: 10 22      ← записалось значение 0x2210")
        print("    A (0x6846e): ff ff ff ff    B: 58 00 00 00  ← записалось 0x58 (88)")
        print("    A (0x69599): 10 22          B: 00 00      ← обнулилось 0x2210")
        print("    A (0x695a0): 58             B: 00         ← обнулилось 0x58")
        print("    ВЫВОД: значение 'переехало' из блока 0x69599.. в блок 0x68467..")
        print("    0x2210 = 8720, 0x58 = 88 — может быть experience, ID, или смещение")
    
    elif cs == 0x00130b71:
        print("  → ASCII/base64 блок (кодированные данные)")
        a_str = A[cs:ce].decode('ascii', errors='replace')
        b_str = B[cs:ce].decode('ascii', errors='replace')
        print(f"    A: {a_str[:80]!r}")
        print(f"    B: {b_str[:80]!r}")
        # Проверим base64
        import base64
        try:
            a_dec = base64.b64decode(a_str + '==', validate=False)
            print(f"    A decoded: {a_dec[:60]}")
        except: pass
        print("    ВЫВОД: закодированный блок (возможно описание карты или сценарий)")
    
    elif cs == 0x0013e409:
        print("  → Единичный байт 0x0d → 0x58")
        print("    В контексте: возможно это указатель/индекс в каком-то массиве")
    
    elif cs == 0x00157ec3:
        print("  → КООРДИНАТЫ ГЕРОЯ (UTF-16 строки!)")
        print("    Было: '5\\0b\\0' + '\\0\\0\\0\\0\\1' + '5\\0b\\0'  (X='5', Y='b'=11)")
        print("    Стало: '4\\0a\\0' + '\\0\\0\\0\\0\\1' + '4\\0a\\0'  (X='4', Y='a'=10)")
        print("    ВЫВОД: ГЕРОЙ СДВИНУЛСЯ: X 5→4, Y b→a (т.е. на 1 влево и на 1 вверх)")
        print("    Эти строки записаны как UTF-16 LE (каждый символ + \\x00)")
    
    elif cs == 0x00157ed3:
        a_val = interp_u32_le(A, cs)
        b_val = interp_u32_le(B, cs)
        print(f"  → 4 байта: A=0x{a_val:08x}, B=0x{b_val:08x}")
        print("    A=0 (нет значения), B=0xFFFFFFFF (-1)")
        print("    ВЫВОД: Появилось значение -1 (например, 'нет целевой точки' или 'unknown')")
    
    elif cs == 0x00157eec:
        print("  → ПУТЬ ГЕРОЯ / ПЛАН ХОДОВ (12 байт)")
        print("    A: 06 02 f3 00 [32 00 00 00] [61 00 00 00]   <- 6, ?, 0x2f3, X=0x32=50, Y=0x61=97")
        print("    B: 07 02 f3 00 [ff ff ff ff] [ff ff ff ff]   <- 7, ?, 0x2f3, X=-1, Y=-1")
        print("    ВЫВОД: Герой ПОКИНУЛ координату (50, 97). Изменился счётчик (6→7).")
        print("    Похоже на массив 'посещённых тайлов' или 'текущего пути'")
    
    elif cs == 0x00157f00:
        a_val = A[cs]
        b_val = B[cs]
        print(f"  → Единичный байт: A=0x{a_val:02x}, B=0x{b_val:02x}")
        print("    Может быть length-counter или flags")
    
    elif cs in (0x00171093, 0x001711b3, 0x001712d3):
        a_val = A[cs]
        b_val = B[cs]
        delta = b_val - a_val
        print(f"  → Единичный байт: A=0x{a_val:02x} ({a_val}), B=0x{b_val:02x} ({b_val}), delta={delta}")
        # Расстояние между изменениями
        if cs == 0x00171093:
            stride = 0x001711b3 - 0x00171093
            print(f"    Расстояние до следующего изменения: 0x{stride:x} = {stride} байт")
            print(f"    ВЫВОД: это массив с шагом {stride} байт (вероятно, ТАБЛИЦА ИГРОКОВ или ГОРОДОВ)")
    
    elif cs == 0x0017ec37:
        a_val = A[cs]
        b_val = B[cs]
        print(f"  → Единичный байт: A=0x{a_val:02x}, B=0x{b_val:02x}, delta={b_val-a_val}")
    
    elif cs == 0x00181552:
        print("  → БОЛЬШОЙ БЛОК (44 байта) — ПУТЬ ГЕРОЯ В UTF-16!")
        print("    Были нули → появились записи вида:")
        print("      01 03 [58 00 00 00] [07] [35 00 62 00] [34 00 61 00] [0b] ...")
        print("    Похоже на записи пути: 'X Y' UTF-16 строки: '5b' '4a' '5b' '4a'...")
        print("    '/a' '/b' '/c' — разделители")
        print("    ВЫВОД: Это ПУТЬ ГЕРОЯ (hero path) — список координат для анимации движения")
    
    elif cs == 0x00181596:
        a_val = A[cs]
        b_val = B[cs]
        print(f"  → '0' (0x30) → 0x00 — символ '0' убран (строка сократилась на 1 символ)")
    
    elif cs == 0x001815c1:
        a_val = A[cs]
        b_val = B[cs]
        print(f"  → 0x00 → '0' (0x30) — добавлен символ '0' (строка удлинилась)")
        print("    ВЫВОД: вместе с предыдущим — символ '0' ПЕРЕМЕЩЁН внутри строки")
    
    elif cs == 0x0018255c:
        print("  → БЛОК 'HD3' — Magic-сигнатура!")
        print("    A: 'HD3\\0\\x1c\\\\L'  ← сигнатура блока 'HD3' + size/offset")
        print("    B: 7 нулей         ← блок стёрт с этого места")
        print("    ВЫВОД: Секция 'HD3' (Heroes 3 Data?) ПЕРЕМЕСТИЛАСЬ!")
    
    elif cs == 0x0018256e:
        print("  → ПЕРЕМЕЩЁННАЯ СЕКЦИЯ 'HD3'")
        print("    B содержит новую 'HD3\\0\\xe7gL' — блок переехал на новый offset")
        print("    ВЫВОД: Подтверждается — секция 'HD3' сдвинулась из-за роста предыдущих данных")
        print("           (вероятно, добавились новые тайлы в fog of war)")

print("\n" + "=" * 80)
print("ИТОГОВАЯ СВОДКА НАХОДОК")
print("=" * 80)

FINDINGS = [
    {
        'offset': '0x000003b6',
        'size': 5,
        'interpretation': 'Имя файла сейва в заголовке',
        'format': 'ASCII строка, null-terminated',
        'value_A': '.GM1\\0',
        'value_B': '1.GM1',
    },
    {
        'offset': '0x00068467 / 0x00069599',
        'size': '6 байт (пара)',
        'interpretation': 'Перемещение значения между двумя блоками (возможно hero ID или experience)',
        'format': 'u16 LE + u32 LE',
        'value': '0x2210 (8720) + 0x58 (88)',
    },
    {
        'offset': '0x00130b71',
        'size': '~250 байт',
        'interpretation': 'Base64-подобный закодированный блок (вероятно описание карты)',
        'format': 'ASCII, строки разделены \\n',
    },
    {
        'offset': '0x00157ec3',
        'size': 10,
        'interpretation': '⭐ КООРДИНАТЫ ГЕРОЯ в UTF-16 LE',
        'format': 'UTF-16 LE строки: X(1 char) + Y(1 char)',
        'value_A': 'X=5, Y=b (11)',
        'value_B': 'X=4, Y=a (10)',
        'movement': 'герой сдвинулся на (-1, -1) — влево и вверх',
    },
    {
        'offset': '0x00157eec',
        'size': 12,
        'interpretation': '⭐ ПУТЬ ГЕРОЯ / посещённые тайлы',
        'format': 'struct {u8 counter; u8 flag; u16 step; i32 X; i32 Y}',
        'value_A': 'counter=6, X=50, Y=97',
        'value_B': 'counter=7, X=-1, Y=-1 (точка покинута)',
    },
    {
        'offset': '0x00171093, 0x001711b3, 0x001712d3',
        'size': '3 × 1 байт, stride=0x120 (288)',
        'interpretation': 'Массив с шагом 288 байт — вероятно ТАБЛИЦА ГОРОДОВ или ИГРОКОВ',
        'format': 'u8 field (movement points? resource count?)',
        'delta_A_to_B': '+8, +8, +8 (однообразное приращение)',
    },
    {
        'offset': '0x00181552',
        'size': 44,
        'interpretation': '⭐ ПУТЬ ГЕРОЯ (hero path) — список тайлов для анимации движения',
        'format': 'записи с UTF-16 строками координат X,Y + разделители /a,/b,/c',
    },
    {
        'offset': '0x0018255c → 0x0018256e',
        'size': '~50 байт',
        'interpretation': '⭐ СЕКЦИЯ "HD3" — ПЕРЕМЕЩЕНА (Heroes 3 Data)',
        'format': 'magic "HD3\\0" + u32 LE offset/size',
        'reason': 'секция сдвинулась из-за роста предыдущих данных',
    },
]

for i, f in enumerate(FINDINGS):
    print(f"\n[{i+1}] {f['offset']}")
    print(f"    Размер: {f['size']}")
    print(f"    Интерпретация: {f['interpretation']}")
    print(f"    Формат: {f['format']}")
    for k, v in f.items():
        if k.startswith('value_') or k == 'movement' or k == 'delta_A_to_B' or k == 'reason':
            print(f"    {k}: {v}")

# Сохраним в JSON
with open('/home/z/my-project/work_disasm/diff_interpretation.json', 'w', encoding='utf-8') as f:
    json.dump(FINDINGS, f, indent=2, ensure_ascii=False)

print("\n[*] Сохранили: /home/z/my-project/work_disasm/diff_interpretation.json")
