#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3c: Анализ функций записи/чтения сейва через IAT-импорты.

Стратегия:
- Find imports for: WriteFile, ReadFile, CreateFileA (kernel32)
- Find gzlib/zlib functions if present
- Find all xrefs to these imports → those are the actual save I/O functions
- Then trace BACKWARDS from those calls to find the function that builds the save buffer
"""
import sys, struct, re, json
from collections import defaultdict, Counter
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

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

# === 1. Получить все импорты с их IAT адресами ===
import_table = {}  # iat_va -> "dll!func"
import_by_name = {}  # clean_func_name -> iat_va

if hasattr(pe, 'DIRECTORY_ENTRY_IMPORT'):
    for entry in pe.DIRECTORY_ENTRY_IMPORT:
        dll = entry.dll.decode('utf-8', errors='replace').lower()
        for imp in entry.imports:
            if imp.name:
                func_name = imp.name.decode('utf-8', errors='replace')
                clean = re.sub(r'@\d+$', '', func_name)
                clean = re.sub(r'^\?|@@.*$', '', clean)
                iat_va = imp.address  # абсолютный VA в IAT
                import_table[iat_va] = f"{dll}!{clean}"
                import_by_name.setdefault(clean, []).append(iat_va)

print(f"[*] Импортов: {len(import_table)}")

# Интересные импорты для savegame
INTERESTING = ['WriteFile', 'ReadFile', 'CreateFileA', 'CloseHandle', 'DeleteFileA',
               'GetFileAttributesA', 'SetFilePointer', 'GetFileSize', 'GetFileTime',
               'malloc', 'free', 'realloc', 'new', 'delete',
               'memcpy', 'memmove', 'memset',
               # Miles audio (smackw32) — для видео-сейвов
               # bink — для видео
               ]

print("\n[*] Интересные импорты:")
for name in INTERESTING:
    if name in import_by_name:
        for iat in import_by_name[name]:
            print(f"  {name:25s} IAT @ 0x{iat:08x}")

# === 2. Найти все call'ы к этим IAT-ам (call dword ptr [0xNNNN]) ===
# Формат: FF 15 <imm32>  →  call dword ptr [imm32]
# Или FF 25 для jmp (thunks)

print("\n[*] Поиск косвенных call-ов к импортам...")
import_callers = defaultdict(list)  # iat_va -> list of caller VAs

for i in range(len(text_data) - 6):
    if text_data[i] == 0xFF and text_data[i+1] == 0x15:
        # call dword ptr [imm32]
        imm = struct.unpack('<I', text_data[i+2:i+6])[0]
        if imm in import_table:
            caller_va = text_va_base + i
            import_callers[imm].append(caller_va)
    elif text_data[i] == 0xFF and text_data[i+1] == 0x25:
        # jmp dword ptr [imm32] — thunk
        imm = struct.unpack('<I', text_data[i+2:i+6])[0]
        if imm in import_table:
            caller_va = text_va_base + i
            import_callers[imm].append(caller_va)

# Показать статистику
print("\n[*] Топ импортов по числу вызовов:")
for iat_va, callers in sorted(import_callers.items(), key=lambda x: -len(x[1]))[:30]:
    print(f"  {import_table[iat_va]:40s} {len(callers):4d} вызовов")

# === 2.5. Helper: найти начало функции ===
def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

def find_func_start(target_va, max_back=8192):
    """Find function start by looking for 'push ebp; mov ebp, esp' prolog."""
    for offset in range(0, max_back, 1):
        va = target_va - offset
        if va < text_va_base:
            break
        off = va_to_offset(va)
        if off is None or off + 2 >= len(pe.__data__):
            continue
        if pe.__data__[off] == 0x55 and pe.__data__[off+1] == 0x8B and pe.__data__[off+2] == 0xEC:
            return va
    return None

# === 3. Для записи сейва нас интересуют WriteFile и CreateFileA ===
print("\n" + "=" * 70)
print("Анализ вызывающих WriteFile / CreateFileA / ReadFile")
print("=" * 70)

KEY_IMPS = ['WriteFile', 'ReadFile', 'CreateFileA', 'DeleteFileA', 'GetFileSize']

for imp_name in KEY_IMPS:
    if imp_name not in import_by_name:
        continue
    print(f"\n--- {imp_name} ---")
    for iat_va in import_by_name[imp_name]:
        callers = import_callers.get(iat_va, [])
        print(f"\nIAT 0x{iat_va:08x}: {len(callers)} вызовов")
        for caller_va in callers[:20]:
            func_start = find_func_start(caller_va)
            print(f"  call at 0x{caller_va:08x}  →  func 0x{func_start:08x}" if func_start else f"  call at 0x{caller_va:08x}  →  (функция не найдена)")

# === 4. Найти функции, которые содержат call'ы к WriteFile + magic H3SVG ===
print("\n" + "=" * 70)
print("Функции, которые и пишут файл и используют H3SVG/H3SVC")
print("=" * 70)

# Все уникальные функции, вызывающие WriteFile
write_caller_funcs = set()
for iat_va in import_by_name.get('WriteFile', []):
    for caller_va in import_callers.get(iat_va, []):
        fs = find_func_start(caller_va)
        if fs:
            write_caller_funcs.add(fs)

# Все функции, которые ссылаются на H3SVG или H3SVC
def find_string_xrefs(target_str_va):
    target_bytes = struct.pack('<I', target_str_va)
    pos = 0
    xrefs = []
    while True:
        idx = text_data.find(target_bytes, pos)
        if idx < 0: break
        xrefs.append(text_va_base + idx)
        pos = idx + 1
    return xrefs

magic_xrefs = []
for magic_str_va in [IMAGE_BASE + 0x277d38,  # H3SVG
                     IMAGE_BASE + 0x277d40]:  # H3SVC
    magic_xrefs.extend(find_string_xrefs(magic_str_va))

magic_funcs = set()
for xva in magic_xrefs:
    fs = find_func_start(xva)
    if fs:
        magic_funcs.add(fs)

print(f"\nФункции, вызывающие WriteFile: {len(write_caller_funcs)}")
for f in sorted(write_caller_funcs):
    print(f"  0x{f:08x}")

print(f"\nФункции, ссылающиеся на H3SVG/H3SVC: {len(magic_funcs)}")
for f in sorted(magic_funcs):
    print(f"  0x{f:08x}")

# Пересечение — это функции записи сейва!
save_writers = write_caller_funcs & magic_funcs if write_caller_funcs and magic_funcs else set()
print(f"\nПересечение (функции записи сейва): {len(save_writers)}")
for f in sorted(save_writers):
    print(f"  ⭐ 0x{f:08x}")

# === 5. То же для ReadFile (функции чтения сейва) ===
read_caller_funcs = set()
for iat_va in import_by_name.get('ReadFile', []):
    for caller_va in import_callers.get(iat_va, []):
        fs = find_func_start(caller_va)
        if fs:
            read_caller_funcs.add(fs)

save_readers = read_caller_funcs & magic_funcs if read_caller_funcs and magic_funcs else set()
print(f"\nПересечение (функции чтения сейва): {len(save_readers)}")
for f in sorted(save_readers):
    print(f"  ⭐ 0x{f:08x}")

# === 6. Найти CreateFileA-callers рядом с magic ===
print("\n[*] CreateFileA callers (всего):")
cf_funcs = set()
for iat_va in import_by_name.get('CreateFileA', []):
    for caller_va in import_callers.get(iat_va, []):
        fs = find_func_start(caller_va)
        if fs:
            cf_funcs.add(fs)
print(f"  {len(cf_funcs)} уникальных функций")

# === 7. Расширенный анализ: функция, которая содержит magic + CreateFileA + WriteFile ===
# Это и есть главный save writer!
all_save_funcs = (write_caller_funcs | read_caller_funcs | magic_funcs | cf_funcs)
print(f"\nВсе функции, связанные с сейвами: {len(all_save_funcs)}")

# === 8. Сохранить ===
result = {
    'image_base': f"0x{IMAGE_BASE:08x}",
    'save_writer_funcs': [f"0x{x:08x}" for x in sorted(save_writers)],
    'save_reader_funcs': [f"0x{x:08x}" for x in sorted(save_readers)],
    'magic_funcs': [f"0x{x:08x}" for x in sorted(magic_funcs)],
    'writefile_caller_funcs': [f"0x{x:08x}" for x in sorted(write_caller_funcs)],
    'readfile_caller_funcs': [f"0x{x:08x}" for x in sorted(read_caller_funcs)],
    'createfilea_caller_funcs': [f"0x{x:08x}" for x in sorted(cf_funcs)],
    'all_save_related_funcs': [f"0x{x:08x}" for x in sorted(all_save_funcs)],
}

with open(f"{OUT_DIR}/save_io_funcs.json", 'w', encoding='utf-8') as f:
    json.dump(result, f, indent=2, ensure_ascii=False)

print(f"\n[*] Сохранено: {OUT_DIR}/save_io_funcs.json")
print("Готово.")
