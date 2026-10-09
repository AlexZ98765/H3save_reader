#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Поиск путей к исходным файлам и других debug-строк в heroes3.exe.

Разработчики оставили строки вида "C:\Dev\Heroes 3 Exp 2\Game\WINGRAPH.CPP" —
это даёт нам имена исходных файлов. Если найдём много таких путей, сможем
сопоставить функции с исходными файлами (по xref'ам).
"""
import sys, struct, re, json
from collections import Counter, defaultdict
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')
import pefile, capstone

EXE_PATH = "/home/z/my-project/upload/heroes3.exe"
OUT_DIR = "/home/z/my-project/work_disasm"
IMAGE_BASE = 0x400000

pe = pefile.PE(EXE_PATH, fast_load=True)
pe.parse_data_directories()

text_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.text'):
        text_sec = s

text_data = pe.__data__[text_sec.PointerToRawData:text_sec.PointerToRawData + text_sec.SizeOfRawData]
text_va_base = text_sec.VirtualAddress

# === 1. Найти все строки-пути к файлам ===
print("=" * 70)
print("ШАГ 1: Поиск путей к исходным файлам")
print("=" * 70)

str_regex = re.compile(rb'[\x20-\x7e]{5,}')
all_strings = {}
for s in pe.sections:
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    for m in str_regex.finditer(sec_data):
        va = IMAGE_BASE + s.VirtualAddress + m.start()
        all_strings[va] = m.group().decode('ascii', errors='replace')

# Паттерны путей
path_patterns = [
    re.compile(r'^[A-Za-z]:\\[^<>:"|?*\x00-\x1f]+\.(cpp|c|hpp|h|cxx|hxx|inl)$', re.IGNORECASE),
    re.compile(r'^[A-Za-z]:\\[^<>:"|?*\x00-\x1f]+\.(def|txt|pcx|bmp|lod|pac|snd|vid|pak)$', re.IGNORECASE),
    re.compile(r'^[A-Za-z]:\\', re.IGNORECASE),  # Любой путь
    re.compile(r'\\Heroes\s*3', re.IGNORECASE),
    re.compile(r'Heroes\s*3\s*Exp', re.IGNORECASE),
    re.compile(r'\.cpp$', re.IGNORECASE),
    re.compile(r'\.h$', re.IGNORECASE),
]

# Найти все пути
cpp_paths = []  # .cpp/.h files
other_paths = []  # другие пути
for va, val in all_strings.items():
    if re.search(r'^[A-Za-z]:\\', val) or re.search(r'\\Heroes\s*3', val, re.IGNORECASE):
        if re.search(r'\.(cpp|c|hpp|h|cxx|hxx|inl)$', val, re.IGNORECASE):
            cpp_paths.append((va, val))
        else:
            other_paths.append((va, val))

print(f"\nНайдено путей к .cpp/.h файлам: {len(cpp_paths)}")
print(f"Найдено других путей: {len(other_paths)}")

# Уникальные имена файлов
cpp_files = Counter()
for va, val in cpp_paths:
    # Извлечь имя файла из пути
    fname = val.rsplit('\\', 1)[-1] if '\\' in val else val.rsplit('/', 1)[-1]
    cpp_files[fname] += 1

print(f"\nУникальных .cpp/.h файлов: {len(cpp_files)}")
print(f"\nТоп-30 .cpp/.h файлов по частоте упоминания:")
for fname, cnt in cpp_files.most_common(30):
    print(f"  {cnt:3d}x  {fname}")

# Сохранить все пути
with open(f"{OUT_DIR}/source_file_paths.txt", 'w', encoding='utf-8') as f:
    f.write(f"# Все пути к .cpp/.h файлам из heroes3.exe\n")
    f.write(f"# Всего: {len(cpp_paths)}\n\n")
    for va, val in sorted(cpp_paths):
        f.write(f"0x{va:08x}  {val}\n")
    f.write(f"\n# Другие пути (всего {len(other_paths)}):\n\n")
    for va, val in sorted(other_paths):
        f.write(f"0x{va:08x}  {val}\n")

print(f"\n[*] Сохранено: {OUT_DIR}/source_file_paths.txt")

# === 2. Найти xref'ы на эти .cpp-файлы ===
print("\n" + "=" * 70)
print("ШАГ 2: Поиск xref'ов на .cpp-файлы")
print("=" * 70)

# Для каждого .cpp-пути найти xref в .text
def find_xrefs_to_va(target_va):
    target_bytes = struct.pack('<I', target_va)
    pos = 0
    xrefs = []
    while True:
        idx = text_data.find(target_bytes, pos)
        if idx < 0: break
        xrefs.append(text_va_base + idx)
        pos = idx + 1
    return xrefs

cpp_xrefs = {}  # (va, filename) -> [xref VAs]
for va, val in cpp_paths:
    fname = val.rsplit('\\', 1)[-1] if '\\' in val else val
    xrefs = find_xrefs_to_va(va)
    if xrefs:
        cpp_xrefs[(va, fname)] = xrefs

print(f"Файлов с xref'ами: {len(cpp_xrefs)}/{len(cpp_paths)}")

# Группировка по имени файла
by_filename = defaultdict(list)
for (va, fname), xrefs in cpp_xrefs.items():
    by_filename[fname].extend(xrefs)

print(f"\nТоп-30 файлов по количеству xref'ов (вероятно, главные модули):")
for fname, xrefs in sorted(by_filename.items(), key=lambda x: -len(x[1]))[:30]:
    print(f"  {len(xrefs):4d}x  {fname}")

# === 3. Сопоставить xref'ы с функциями ===
print("\n" + "=" * 70)
print("ШАГ 3: Сопоставление функций с .cpp-файлами")
print("=" * 70)

def find_func_start(target_va, max_back=16384):
    for offset in range(0, max_back, 1):
        va = target_va - offset
        if va < text_va_base:
            break
        off = None
        for s in pe.sections:
            if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
                off = s.PointerToRawData + (va - s.VirtualAddress)
                break
        if off is None or off + 2 >= len(pe.__data__):
            continue
        if pe.__data__[off] == 0x55 and pe.__data__[off+1] == 0x8B and pe.__data__[off+2] == 0xEC:
            return va
    return None

func_to_files = defaultdict(set)  # func_va -> set of cpp filenames
file_to_funcs = defaultdict(set)  # cpp filename -> set of func VAs

for (va, fname), xrefs in cpp_xrefs.items():
    for xva in xrefs:
        fs = find_func_start(xva)
        if fs:
            func_to_files[fs].add(fname)
            file_to_funcs[fname].add(fs)

print(f"\nФункций, которые ссылаются на .cpp-файлы: {len(func_to_files)}")
print(f"\nТоп-30 функций по количеству различных .cpp-файлов (вероятно, dispatch-функции):")
for fv, files in sorted(func_to_files.items(), key=lambda x: -len(x[1]))[:30]:
    print(f"  0x{fv:08x}  {len(files):3d} файлов: {sorted(files)[:5]}")

# === 4. Особо интересующие нас .cpp-файлы ===
print("\n" + "=" * 70)
print("ШАГ 4: Поиск функций из .cpp-файлов с интересными именами")
print("=" * 70)

INTERESTING_KEYWORDS = [
    'serialize', 'seriali', 'save', 'load', 'gamestate', 'game_state',
    'player', 'hero', 'town', 'map', 'object', 'turn', 'round',
    'fog', 'path', 'ai', 'battle', 'artifact', 'spell', 'creature',
    'scenario', 'campaign', 'mapobject', 'gameinfo', 'game_info',
]

for fname, funcs in sorted(file_to_funcs.items(), key=lambda x: -len(x[1])):
    fn_lower = fname.lower()
    for kw in INTERESTING_KEYWORDS:
        if kw in fn_lower:
            print(f"\n  📄 {fname}  ({len(funcs)} функций)")
            for fv in sorted(funcs)[:10]:
                print(f"      0x{fv:08x}")
            break

# === 5. Сохранить соответствие функций и файлов ===
result = {
    'total_cpp_files': len(cpp_paths),
    'unique_cpp_files': len(cpp_files),
    'total_xrefs': sum(len(x) for x in cpp_xrefs.values()),
    'funcs_with_xrefs': len(func_to_files),
    'top_30_files_by_xrefs': [
        {'file': fname, 'xref_count': len(xrefs)}
        for fname, xrefs in sorted(by_filename.items(), key=lambda x: -len(x[1]))[:30]
    ],
    'top_30_funcs_by_file_count': [
        {'va': f"0x{fv:08x}", 'file_count': len(files), 'files': sorted(files)[:10]}
        for fv, files in sorted(func_to_files.items(), key=lambda x: -len(x[1]))[:30]
    ],
    'interesting_files': {
        fname: [f"0x{x:08x}" for x in sorted(funcs)]
        for fname, funcs in file_to_funcs.items()
        if any(kw in fname.lower() for kw in INTERESTING_KEYWORDS)
    },
}

with open(f"{OUT_DIR}/cpp_file_mapping.json", 'w', encoding='utf-8') as f:
    json.dump(result, f, indent=2, ensure_ascii=False)

print(f"\n[*] Сохранено: {OUT_DIR}/cpp_file_mapping.json")

# === 6. Сохранить все имена .cpp-файлов (уникальные) ===
with open(f"{OUT_DIR}/unique_cpp_files.txt", 'w', encoding='utf-8') as f:
    f.write(f"# Уникальные .cpp/.h файлы из heroes3.exe\n")
    f.write(f"# Всего: {len(cpp_files)}\n\n")
    for fname, cnt in sorted(cpp_files.items()):
        f.write(f"{cnt:4d}x  {fname}\n")

print(f"[*] Сохранено: {OUT_DIR}/unique_cpp_files.txt")

print("\nГотово.")
