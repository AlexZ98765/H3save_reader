#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3b-2: Углублённый анализ главной функции сериализации 0x179ce0.

Цель: разобрать поток данных — какие поля и в каком порядке записываются/читаются.
Стратегия:
1. Дизассемблировать функцию полностью.
2. Найти все call'ы — особенно к библиотечным функциям чтения/записи.
3. Отследить смещения [ecx + N], [edx + N] — это поля объекта.
4. Выявить последовательность серийных чтений/записей.
"""
import sys, struct, re, json
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')
import pefile, capstone

EXE_PATH = "/home/z/my-project/upload/heroes3.exe"
OUT_DIR = "/home/z/my-project/work_disasm"
IMAGE_BASE = 0x400000

pe = pefile.PE(EXE_PATH, fast_load=True)
pe.parse_data_directories()

text_sec = None
data_sec = None
rdata_sec = None
for s in pe.sections:
    name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    if name == '.text':
        text_sec = s
    elif name == '.data':
        data_sec = s
    elif name == '.rdata':
        rdata_sec = s

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

def disasm_function_complete(start_va, max_bytes=65536):
    """Дизассемблируем до ret или до max_bytes, следуя jmp."""
    instrs = []
    va = start_va
    end_va = start_va + max_bytes
    visited = set()
    while va and va < end_va and len(instrs) < 50000:
        if va in visited:
            break
        visited.add(va)
        off = va_to_offset(va)
        if off is None:
            break
        chunk = pe.__data__[off:off+16]
        ins_list = list(md.disasm(chunk, va, count=1))
        if not ins_list:
            break
        ins = ins_list[0]
        instrs.append(ins)
        if ins.mnemonic == 'ret':
            break
        if ins.mnemonic == 'jmp':
            try:
                target = int(ins.op_str, 0)
                if text_sec.VirtualAddress <= target < text_sec.VirtualAddress + text_sec.Misc_VirtualSize:
                    va = target
                    continue
            except ValueError:
                pass
            break
        va += ins.size
    return instrs

# === Извлечём все строки ===
str_regex = re.compile(rb'[\x20-\x7e]{5,}')
all_strings = {}
for s in pe.sections:
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    for m in str_regex.finditer(sec_data):
        va = IMAGE_BASE + s.VirtualAddress + m.start()
        val = m.group().decode('ascii', errors='replace')
        all_strings[va] = val

# === Извлечём все импорты ===
import_table = {}  # va_of_iat_entry -> "dll!func"
if hasattr(pe, 'DIRECTORY_ENTRY_IMPORT'):
    for entry in pe.DIRECTORY_ENTRY_IMPORT:
        dll = entry.dll.decode('utf-8', errors='replace').lower()
        for imp in entry.imports:
            if imp.name:
                func_name = imp.name.decode('utf-8', errors='replace')
                # Уберём декорацию stdcall (@N) и манглинг C++
                clean = re.sub(r'@\d+$', '', func_name)
                clean = re.sub(r'^\?|@@.*$', '', clean)
                import_table[imp.address] = f"{dll}!{clean}"

print(f"[*] Импортов: {len(import_table)}")

# === Анализируем главные функции сериализации ===
TARGET_FUNCS = [
    (0x179ce0, "BIG_SERIALIZE_FUNC"),  # 189 ссылок на .data
    (0x17f600, "NEWGAME_HANDLER_1"),
    (0x183160, "NEWGAME_HANDLER_2"),
    (0x1857d0, "AUTOSAVE_HANDLER"),
    (0x1776e0, "MAP_INIT_FUNC"),
]

for func_va, label in TARGET_FUNCS:
    print(f"\n{'='*80}")
    print(f"Анализ функции {label} at 0x{func_va:08x}")
    print(f"{'='*80}")
    
    instrs = disasm_function_complete(func_va, max_bytes=200000)
    print(f"Дизассемблировано {len(instrs)} инструкций")
    
    # Найти все call'ы
    calls = []
    for ins in instrs:
        if ins.mnemonic == 'call':
            try:
                target = int(ins.op_str, 0)
                target_label = import_table.get(IMAGE_BASE + target) if False else None
                # Импорты в PE IAT — используются через косвенную адресацию
                # call dword ptr [0xNNNN] — где 0xNNNN это IAT entry
                calls.append((ins.address, target, target_label))
            except ValueError:
                # Косвенный вызов (call dword ptr [eax+4] и т.п.)
                calls.append((ins.address, None, ins.op_str))
    
    print(f"\nCall'ы ({len(calls)} всего):")
    unique_targets = Counter = {}
    from collections import Counter
    target_counter = Counter()
    for ca, tgt, lbl in calls:
        if tgt is not None:
            target_counter[tgt] += 1
        else:
            target_counter[lbl] += 1
    
    for tgt, cnt in target_counter.most_common(30):
        if isinstance(tgt, int):
            # Проверим, импорт ли это
            if IMAGE_BASE + tgt in import_table:  # вряд ли, т.к. IAT обычно в .rdata
                label = import_table[IMAGE_BASE + tgt]
            else:
                label = f"sub_{tgt:08x}"
        else:
            label = tgt
        print(f"  {cnt:4d}x → {label}")
    
    # Найти все строки, на которые ссылается функция
    print(f"\nСтроковые ссылки:")
    seen_strs = set()
    for ins in instrs:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings:
                    s = all_strings[imm]
                    if s not in seen_strs and len(s) < 100:
                        seen_strs.add(s)
                        print(f"  0x{ins.address:08x} → {s!r}")
            except ValueError:
                pass
    
    # Сохраняем полное дизассемблирование
    with open(f"{OUT_DIR}/func_{label}_0x{func_va:08x}.asm", 'w', encoding='utf-8') as f:
        f.write(f"; Function: {label} at 0x{func_va:08x}\n")
        f.write(f"; Image base: 0x{IMAGE_BASE:08x}\n")
        f.write(f"; Instructions: {len(instrs)}\n\n")
        for ins in instrs:
            # Аннотируем ссылки на строки
            annotation = ""
            for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
                try:
                    imm = int(m.group(), 0)
                    if imm in all_strings:
                        annotation += f"  ; {all_strings[imm]!r}"
                        break
                except ValueError:
                    pass
            f.write(f"0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}{annotation}\n")
    
    print(f"\n[*] Сохранено: {OUT_DIR}/func_{label}_0x{func_va:08x}.asm")

print("\nГотово.")
