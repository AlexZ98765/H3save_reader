#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3b: Глубокий анализ функции сериализации.

Найденная функция 0x000bc010 — кандидат на "create save header" (генерит H3SVG/H3SVC magic).
Нужно:
1. Найти ВСЕ функции, которые записывают/читают сейв.
2. Рекурсивно пройти по call-graph, восстанавливая структуру.
3. Найти все строки-подсказки и сопоставить их с функциями.
"""
import sys, struct, re, json
from collections import defaultdict, deque
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')
import pefile, capstone

EXE_PATH = "/home/z/my-project/upload/heroes3.exe"
OUT_DIR = "/home/z/my-project/work_disasm"
IMAGE_BASE = 0x400000

pe = pefile.PE(EXE_PATH, fast_load=True)
pe.parse_data_directories()

# Найти .text секцию
text_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.text'):
        text_sec = s
        break

text_data = pe.__data__[text_sec.PointerToRawData:text_sec.PointerToRawData + text_sec.SizeOfRawData]
text_va_base = text_sec.VirtualAddress
text_size = text_sec.Misc_VirtualSize

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

def disasm_range(start_va, end_va):
    """Disassemble [start_va, end_va)."""
    start_off = va_to_offset(start_va)
    end_off = va_to_offset(end_va)
    if start_off is None or end_off is None:
        return []
    chunk = pe.__data__[start_off:end_off]
    return list(md.disasm(chunk, start_va))

def disasm_function(start_va, max_instrs=10000):
    """Disassemble a function, stopping at ret/jmp that exits."""
    instrs = []
    va = start_va
    seen = set()
    while va and len(instrs) < max_instrs:
        if va in seen:  # infinite loop protection
            break
        seen.add(va)
        off = va_to_offset(va)
        if off is None:
            break
        # Дизассемблируем 1 инструкцию
        chunk = pe.__data__[off:off+16]
        ins_list = list(md.disasm(chunk, va, count=1))
        if not ins_list:
            break
        ins = ins_list[0]
        instrs.append(ins)
        # Останавливаемся на ret
        if ins.mnemonic == 'ret':
            break
        # На unconditional jmp (не loop) — следуем
        if ins.mnemonic == 'jmp':
            # Если это jmp к адресу внутри функции — продолжаем
            try:
                target = int(ins.op_str, 0)
                if text_va_base <= target < text_va_base + text_size:
                    va = target
                    continue
            except ValueError:
                pass
            break
        va += ins.size
    return instrs

# === 1. Найдём функции, которые ссылаются на ключевые строки ===

print("=" * 70)
print("ШАГ 1: Поиск всех функций сериализации")
print("=" * 70)

# Все ключевые строки (VA в формате abs = image_base + rva)
KEY_STRINGS = {
    # Magic
    'H3SVG':       IMAGE_BASE + 0x277d38,
    'H3SVC':       IMAGE_BASE + 0x277d40,
    # Save file related strings
    'NEWGAME.gm1': IMAGE_BASE + 0x28338c,
    'AUTOSAVE.':   IMAGE_BASE + 0x2839b8,
    '*.gm?':       IMAGE_BASE + 0x2834dc,
    '*.cgm':       IMAGE_BASE + 0x2834d4,
    '*.h3m':       IMAGE_BASE + 0x2834e4,
    'Autosave':    IMAGE_BASE + 0x27fe2c,
    'loadgame.pcx': IMAGE_BASE + 0x2780f4,
    'Timeout sending save game': IMAGE_BASE + 0x277f34,
    'Receiving save game from': IMAGE_BASE + 0x2780ac,
}

# Найти все xrefs
xref_map = defaultdict(list)  # str_name -> list of xref VAs
for name, target_va in KEY_STRINGS.items():
    target_bytes = struct.pack('<I', target_va)
    pos = 0
    while True:
        idx = text_data.find(target_bytes, pos)
        if idx < 0: break
        xref_va = text_va_base + idx
        xref_map[name].append(xref_va)
        pos = idx + 1

print("\nXrefs по строкам:")
for name, xrefs in xref_map.items():
    print(f"  {name:40s} : {len(xrefs):3d} xrefs")
    for x in xrefs[:3]:
        print(f"      0x{x:08x}")

# === 2. Для каждого xref найдём начало содержащей его функции ===
# Используем эвристику: ищем ближайший предшествующий "push ebp; mov ebp, esp" 
# или alignment pad (nop-ы) перед процедурным прологом.

def find_function_start(target_va, max_back=8192):
    """Find the start of the function containing target_va."""
    # Идём назад, ищем типичный пролог
    for offset in range(0, max_back, 1):
        va = target_va - offset
        if va < text_va_base:
            break
        off = va_to_offset(va)
        if off is None:
            continue
        # 55 8B EC — push ebp; mov ebp, esp
        if off >= 2 and pe.__data__[off] == 0x55 and pe.__data__[off+1] == 0x8B and pe.__data__[off+2] == 0xEC:
            return va
    return None

print("\n" + "=" * 70)
print("ШАГ 2: Определение функций по xref'ам")
print("=" * 70)

functions = {}  # func_start_va -> {'xrefs_to_strings': [...], 'instrs': [...]}
for name, xrefs in xref_map.items():
    for xref_va in xrefs:
        func_start = find_function_start(xref_va)
        if func_start is None:
            print(f"  Не удалось найти начало функции для xref 0x{xref_va:08x} ({name})")
            continue
        if func_start not in functions:
            functions[func_start] = {'xrefs_to_strings': set(), 'instrs': None}
        functions[func_start]['xrefs_to_strings'].add(name)

print(f"\nНайдено {len(functions)} уникальных функций:")
for fv, info in sorted(functions.items()):
    print(f"  0x{fv:08x}: строки={sorted(info['xrefs_to_strings'])}")

# === 3. Дизассемблируем каждую функцию полностью ===
print("\n" + "=" * 70)
print("ШАГ 3: Полное дизассемблирование найденных функций")
print("=" * 70)

for fv in list(functions.keys()):
    print(f"\n[*] Дизассемблируем функцию 0x{fv:08x}...")
    instrs = disasm_function(fv, max_instrs=20000)
    functions[fv]['instrs'] = instrs
    print(f"    {len(instrs)} инструкций")

# === 4. Извлекаем все call-цели (call graph) ===
print("\n" + "=" * 70)
print("ШАГ 4: Извлечение call-graph")
print("=" * 70)

callgraph = defaultdict(set)  # func_va -> set of called function VAs
for fv, info in functions.items():
    for ins in info['instrs']:
        if ins.mnemonic == 'call':
            try:
                target = int(ins.op_str, 0)
                if text_va_base <= target < text_va_base + text_size:
                    callgraph[fv].add(target)
            except ValueError:
                pass

for fv in sorted(callgraph):
    targets = sorted(callgraph[fv])
    print(f"\n0x{fv:08x} вызывает {len(targets)} функций:")
    for t in targets[:20]:
        print(f"    → 0x{t:08x}")
    if len(targets) > 20:
        print(f"    ... и ещё {len(targets) - 20}")

# === 5. Поиск строковых ссылок внутри найденных функций ===
print("\n" + "=" * 70)
print("ШАГ 5: Строковые ссылки в функциях")
print("=" * 70)

# Найти ВСЕ строки (VA + содержимое) и построить индекс
all_strings = {}  # abs_va -> string value
str_regex = re.compile(rb'[\x20-\x7e]{5,}')
for s in pe.sections:
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    sec_name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    for m in str_regex.finditer(sec_data):
        va = IMAGE_BASE + s.VirtualAddress + m.start()
        val = m.group().decode('ascii', errors='replace')
        all_strings[va] = val

print(f"Индекс строк: {len(all_strings)} entries")

# Для каждой функции найти все 4-байтовые immediate, которые указывают в .rdata/.data
print("\nСтроки, на которые ссылается каждая функция:")
for fv, info in functions.items():
    found_strings = []
    for ins in info['instrs']:
        # Ищем 4-байтовые immediate в op_str
        # Например: push 0x6834d4 или mov eax, 0x6834d4
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings:
                    found_strings.append((ins.address, imm, all_strings[imm]))
            except ValueError:
                pass
    if found_strings:
        print(f"\n0x{fv:08x}:")
        for ins_va, str_va, str_val in found_strings[:20]:
            print(f"    0x{ins_va:08x} → 0x{str_va:08x}  {str_val!r}")
        if len(found_strings) > 20:
            print(f"    ... и ещё {len(found_strings) - 20}")

# === 6. Расширенный анализ: какие глобальные переменные используются ===
print("\n" + "=" * 70)
print("ШАГ 6: Глобальные переменные (ссылки на .data)")
print("=" * 70)

# .data section
data_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.data'):
        data_sec = s
        break

data_va_start = IMAGE_BASE + data_sec.VirtualAddress
data_va_end = data_va_start + data_sec.Misc_VirtualSize

for fv, info in functions.items():
    data_refs = []
    for ins in info['instrs']:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if data_va_start <= imm < data_va_end:
                    data_refs.append((ins.address, imm))
            except ValueError:
                pass
    if data_refs:
        print(f"\n0x{fv:08x}: {len(data_refs)} ссылок на .data (unique: {len(set(r[1] for r in data_refs))})")
        unique_refs = sorted(set(r[1] for r in data_refs))
        for ref in unique_refs[:15]:
            print(f"    → 0x{ref:08x} (offset в .data: 0x{ref - data_va_start:08x})")

# === 7. Сохраняем результаты ===
print("\n[*] Сохраняем результаты...")

# Полный дизассемблированный текст
with open(f"{OUT_DIR}/save_functions_disasm.txt", 'w', encoding='utf-8') as f:
    f.write(f"# Полное дизассемблирование функций сериализации сейвов\n")
    f.write(f"# Image base: 0x{IMAGE_BASE:08x}\n")
    f.write(f"# Найдено функций: {len(functions)}\n\n")
    
    for fv, info in sorted(functions.items()):
        f.write(f"\n{'='*80}\n")
        f.write(f"FUNCTION at 0x{fv:08x}\n")
        f.write(f"  Strings referenced: {sorted(info['xrefs_to_strings'])}\n")
        f.write(f"  Calls: {sorted(callgraph[fv])}\n")
        f.write(f"{'='*80}\n\n")
        for ins in info['instrs']:
            f.write(f"  0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}\n")

# JSON-описание функций
funcs_json = []
for fv, info in sorted(functions.items()):
    funcs_json.append({
        'va': f"0x{fv:08x}",
        'strings': sorted(info['xrefs_to_strings']),
        'instr_count': len(info['instrs']),
        'calls': [f"0x{t:08x}" for t in sorted(callgraph[fv])],
    })

with open(f"{OUT_DIR}/save_functions.json", 'w', encoding='utf-8') as f:
    json.dump({
        'image_base': f"0x{IMAGE_BASE:08x}",
        'functions': funcs_json,
        'string_index_size': len(all_strings),
    }, f, indent=2, ensure_ascii=False)

print(f"[*] Сохранили {OUT_DIR}/save_functions_disasm.txt")
print(f"[*] Сохранили {OUT_DIR}/save_functions.json")
print("\nГотово.")
