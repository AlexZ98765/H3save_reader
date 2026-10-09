#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3e: Детальный дизассемблинг save reader (0xbca60) и save writer (0xbe0b0).

Это ключевые функции сериализации. У них десятки call-целей —
каждая вызываемая функция читает/пишет одну секцию сейва.
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
rdata_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.text'):
        text_sec = s
    elif s.Name.startswith(b'.rdata'):
        rdata_sec = s

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

def disasm_function_complete(start_va, max_instrs=50000):
    """Disassemble until ret, following jmps."""
    instrs = []
    va = start_va
    visited = set()
    while va and len(instrs) < max_instrs:
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

# === Извлечь строки ===
str_regex = re.compile(rb'[\x20-\x7e]{5,}')
all_strings = {}
for s in pe.sections:
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    for m in str_regex.finditer(sec_data):
        va = IMAGE_BASE + s.VirtualAddress + m.start()
        all_strings[va] = m.group().decode('ascii', errors='replace')

# === Извлечь импорты ===
import_table = {}
if hasattr(pe, 'DIRECTORY_ENTRY_IMPORT'):
    for entry in pe.DIRECTORY_ENTRY_IMPORT:
        dll = entry.dll.decode('utf-8', errors='replace').lower()
        for imp in entry.imports:
            if imp.name:
                func_name = imp.name.decode('utf-8', errors='replace')
                clean = re.sub(r'@\d+$', '', func_name)
                clean = re.sub(r'^\?|@@.*$', '', clean)
                import_table[imp.address] = f"{dll}!{clean}"

# === Анализ главных функций ===
TARGETS = [
    (0x000be0b0, "SAVE_WRITER"),  # главный save writer
    (0x000bca60, "SAVE_READER"),  # главный save reader (40 call-целей)
    (0x000beb60, "SAVE_WRITER_DRIVER"),  # вызывает save writer
    (0x000beff0, "SAVE_READER_DRIVER"),  # вызывает save reader
    (0x000bbda0, "HEADER_WRITER"),  # пишет H3SVG/H3SVC magic + header
]

for func_va, label in TARGETS:
    print(f"\n{'='*80}")
    print(f"=== {label} at 0x{func_va:08x} ===")
    print(f"{'='*80}")
    
    instrs = disasm_function_complete(func_va)
    print(f"Инструкций: {len(instrs)}")
    
    # Найти все call-цели
    direct_calls = []  # (call_va, target_va, label)
    indirect_calls = []  # (call_va, op_str) — косвенные через указатель
    
    for ins in instrs:
        if ins.mnemonic == 'call':
            # Прямой call (E8 rel32)
            try:
                target = int(ins.op_str, 0)
                direct_calls.append((ins.address, target))
            except ValueError:
                # Косвенный — call dword ptr [reg+off] или call dword ptr [imm32]
                indirect_calls.append((ins.address, ins.op_str))
                # Проверим, если это IAT-ссылка
                m = re.search(r'\[(0x[0-9a-fA-F]+)\]', ins.op_str)
                if m:
                    iat_va = int(m.group(1), 0)
                    if iat_va in import_table:
                        indirect_calls[-1] = (ins.address, f"{import_table[iat_va]} (IAT 0x{iat_va:08x})")
    
    print(f"\nПрямые call'ы: {len(direct_calls)}")
    print(f"Косвенные call'ы: {len(indirect_calls)}")
    
    # Подсчитать частоту вызовов
    call_counter = Counter(t for _, t in direct_calls)
    print(f"\nТоп-30 вызываемых функций:")
    for tgt, cnt in call_counter.most_common(30):
        # Аннотируем
        ann = ""
        # Проверим, не содержит ли target функция ссылок на строки
        tgt_instrs = disasm_function_complete(tgt, max_instrs=200)
        tgt_strs = []
        for ti in tgt_instrs[:50]:
            for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ti.op_str):
                try:
                    imm = int(m.group(), 0)
                    if imm in all_strings and len(all_strings[imm]) < 80:
                        tgt_strs.append(all_strings[imm])
                except ValueError:
                    pass
        if tgt_strs:
            ann = f"  ; strings: {tgt_strs[:3]}"
        print(f"  {cnt:3d}x → 0x{tgt:08x}{ann}")
    
    # Найти строки в самой функции
    print(f"\nСтроки в {label}:")
    seen = set()
    for ins in instrs:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings:
                    s = all_strings[imm]
                    if s not in seen and len(s) < 80:
                        seen.add(s)
                        print(f"  0x{ins.address:08x} → {s!r}")
            except ValueError:
                pass
    
    # Сохранить полное дизассемблирование с аннотациями
    with open(f"{OUT_DIR}/func_{label}_0x{func_va:08x}.asm", 'w', encoding='utf-8') as f:
        f.write(f"; Function: {label} at 0x{func_va:08x}\n")
        f.write(f"; Image base: 0x{IMAGE_BASE:08x}\n")
        f.write(f"; Instructions: {len(instrs)}\n")
        f.write(f"; Direct calls: {len(direct_calls)}\n")
        f.write(f"; Indirect calls: {len(indirect_calls)}\n\n")
        
        for ins in instrs:
            # Аннотации
            annotation = ""
            
            # Строки
            for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
                try:
                    imm = int(m.group(), 0)
                    if imm in all_strings:
                        annotation += f"  ; \"{all_strings[imm]}\""
                        break
                except ValueError:
                    pass
            
            # Импорты (если это call [imm32] и imm32 в IAT)
            if ins.mnemonic == 'call':
                m = re.search(r'\[(0x[0-9a-fA-F]+)\]', ins.op_str)
                if m:
                    iat_va = int(m.group(1), 0)
                    if iat_va in import_table:
                        annotation = f"  ; {import_table[iat_va]}"
            
            # Аннотация для прямых call-ов — показать целевую функцию
            if ins.mnemonic == 'call':
                try:
                    tgt = int(ins.op_str, 0)
                    annotation += f"  ; → sub_{tgt:08x}"
                except ValueError:
                    pass
            
            f.write(f"0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}{annotation}\n")
    
    print(f"\n[*] Сохранено: {OUT_DIR}/func_{label}_0x{func_va:08x}.asm")

print("\nГотово.")
