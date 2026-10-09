#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Поиск главного сериализатора CGameState::Serialize.

Стратегия:
1. Найти все функции, вызываемые из SAVE_WRITER_DRIVER (0xbeb60)
   и SAVE_READER_DRIVER (0xbeff0) — кроме уже известных writers/readers.
2. Найти самые большие функции в .text — главный сериализатор должен быть в топе.
3. Найти функции с аномальным количеством виртуальных call-ов
   (call dword ptr [eax+8] / [edx+8] / [ecx+8] и т.п.) — это вызовы
   виртуального метода serialize.
4. Найти функции, которые вызывают helper 0x4130/0x4180 много раз
   (это базовые CLoadFile::read/CSaveFile::write).
5. Пересечение всех кандидатов даст главный сериализатор.
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
text_size = text_sec.Misc_VirtualSize

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

def find_func_start(target_va, max_back=16384):
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

def disasm_function(start_va, max_instrs=50000):
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

# Строки
str_regex = re.compile(rb'[\x20-\x7e]{5,}')
all_strings = {}
for s in pe.sections:
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    for m in str_regex.finditer(sec_data):
        va = IMAGE_BASE + s.VirtualAddress + m.start()
        all_strings[va] = m.group().decode('ascii', errors='replace')

# Импорты
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

# === 1. Найти все функции в .text ===
print("=" * 70)
print("ШАГ 1: Поиск всех функций (по прологу 55 8B EC)")
print("=" * 70)

all_funcs = []
for i in range(len(text_data) - 3):
    if text_data[i] == 0x55 and text_data[i+1] == 0x8B and text_data[i+2] == 0xEC:
        all_funcs.append(text_va_base + i)

print(f"Найдено функций: {len(all_funcs)}")

# === 2. Для каждой функции определить размер ===
print("\n" + "=" * 70)
print("ШАГ 2: Вычисление размеров функций")
print("=" * 70)

func_sizes = {}  # va -> (size, instr_count)
print("[*] Дизассемблируем все функции для подсчёта размера (это займёт ~минуту)...")

for i, fv in enumerate(all_funcs):
    if i % 5000 == 0:
        print(f"  обработано {i}/{len(all_funcs)} функций...")
    instrs = disasm_function(fv, max_instrs=20000)
    if instrs:
        last_instr = instrs[-1]
        size = last_instr.address + last_instr.size - fv
        func_sizes[fv] = (size, len(instrs))

print(f"[*] Получены размеры {len(func_sizes)} функций")

# === 3. Топ-20 самых больших функций ===
print("\n" + "=" * 70)
print("ШАГ 3: Топ-30 самых больших функций")
print("=" * 70)

sorted_by_size = sorted(func_sizes.items(), key=lambda x: -x[1][0])
print(f"\n{'VA':12s} {'size':>8s} {'instrs':>8s}  annotations")
print("-" * 80)
for fv, (sz, ic) in sorted_by_size[:30]:
    # Аннотируем — какие строки содержит
    instrs = disasm_function(fv, max_instrs=20000)
    strs = set()
    for ins in instrs:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings and len(all_strings[imm]) < 80:
                    strs.add(all_strings[imm])
            except ValueError:
                pass
    str_preview = ', '.join(list(strs)[:3])
    print(f"0x{fv:08x}   {sz:8d} {ic:8d}  {str_preview[:60]}")

# === 4. Найти функции с аномальным количеством виртуальных call-ов ===
# Признак виртуального call-а: FF 50 NN (call dword ptr [eax+NN8])
# или FF 51 NN, FF 52 NN и т.п. — где смещение 0x8 (типично для vtable[2])
print("\n" + "=" * 70)
print("ШАГ 4: Поиск функций с большим количеством виртуальных call-ов")
print("=" * 70)

# Для каждой функции посчитаем количество виртуальных call-ов
func_vcall_count = {}
func_helper_count = {}  # вызовы helper 0x4130/0x4180
func_call_to_helper = defaultdict(int)

print("[*] Подсчёт виртуальных call-ов во всех функциях...")
for fv, (sz, ic) in func_sizes.items():
    instrs = disasm_function(fv, max_instrs=20000)
    vcall_count = 0
    helper_count = 0
    for ins in instrs:
        if ins.mnemonic == 'call' and '[' in ins.op_str:
            # Косвенный вызов
            vcall_count += 1
        if ins.mnemonic == 'call':
            try:
                target = int(ins.op_str, 0)
                if target in (0x4130, 0x4180):
                    helper_count += 1
            except ValueError:
                pass
    func_vcall_count[fv] = vcall_count
    func_helper_count[fv] = helper_count

# Топ-30 по количеству виртуальных call-ов
print(f"\nТоп-30 функций по количеству виртуальных call-ов (call [reg+N]):")
sorted_by_vcall = sorted(func_vcall_count.items(), key=lambda x: -x[1])
print(f"{'VA':12s} {'vcalls':>8s} {'size':>8s}  annotations")
print("-" * 80)
for fv, vc in sorted_by_vcall[:30]:
    if vc < 10:
        break
    sz, ic = func_sizes[fv]
    # Аннотация
    instrs = disasm_function(fv, max_instrs=20000)
    strs = set()
    for ins in instrs:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings and len(all_strings[imm]) < 80:
                    strs.add(all_strings[imm])
            except ValueError:
                pass
    str_preview = ', '.join(list(strs)[:3])
    print(f"0x{fv:08x}   {vc:8d} {sz:8d}  {str_preview[:60]}")

# Топ-30 по количеству вызовов helper 0x4130/0x4180 (базовые serialize helpers)
print(f"\nТоп-30 функций по количеству вызовов helpers (0x4130/0x4180):")
sorted_by_helper = sorted(func_helper_count.items(), key=lambda x: -x[1])
print(f"{'VA':12s} {'helpers':>8s} {'vcalls':>8s} {'size':>8s}  annotations")
print("-" * 80)
for fv, hc in sorted_by_helper[:30]:
    if hc < 5:
        break
    sz, ic = func_sizes[fv]
    vc = func_vcall_count[fv]
    # Аннотация
    instrs = disasm_function(fv, max_instrs=20000)
    strs = set()
    for ins in instrs:
        for m in re.finditer(r'0x[0-9a-fA-F]{6,8}', ins.op_str):
            try:
                imm = int(m.group(), 0)
                if imm in all_strings and len(all_strings[imm]) < 80:
                    strs.add(all_strings[imm])
            except ValueError:
                pass
    str_preview = ', '.join(list(strs)[:3])
    print(f"0x{fv:08x}   {hc:8d} {vc:8d} {sz:8d}  {str_preview[:60]}")

# === 5. Найти пересечение: top-50 по размеру ∩ top-50 по helper_count ∩ top-50 по vcall ===
print("\n" + "=" * 70)
print("ШАГ 5: Пересечение кандидатов")
print("=" * 70)

top_size = set(fv for fv, _ in sorted_by_size[:50])
top_vcall = set(fv for fv, _ in sorted_by_vcall[:50] if _ >= 10)
top_helper = set(fv for fv, _ in sorted_by_helper[:50] if _ >= 5)

print(f"Топ-50 по размеру: {len(top_size)} функций")
print(f"Топ-50 по виртуальным вызовам (>=10): {len(top_vcall)} функций")
print(f"Топ-50 по вызовам helpers (>=5): {len(top_helper)} функций")

intersection_3 = top_size & top_vcall & top_helper
intersection_2 = (top_size & top_vcall) | (top_size & top_helper) | (top_vcall & top_helper)

print(f"\nПересечение всех трёх (наиболее вероятные кандидаты): {len(intersection_3)}")
for fv in sorted(intersection_3, key=lambda x: -func_sizes[x][0]):
    sz, ic = func_sizes[fv]
    vc = func_vcall_count[fv]
    hc = func_helper_count[fv]
    print(f"  ⭐ 0x{fv:08x}  size={sz} instrs={ic} vcalls={vc} helpers={hc}")

print(f"\nПересечение любых двух (расширенный список): {len(intersection_2)}")
for fv in sorted(intersection_2, key=lambda x: -func_sizes[x][0])[:20]:
    sz, ic = func_sizes[fv]
    vc = func_vcall_count[fv]
    hc = func_helper_count[fv]
    print(f"  • 0x{fv:08x}  size={sz} instrs={ic} vcalls={vc} helpers={hc}")

# === 6. Сохранить JSON-отчёт ===
report = {
    'image_base': f"0x{IMAGE_BASE:08x}",
    'total_functions': len(all_funcs),
    'top_30_by_size': [
        {'va': f"0x{fv:08x}", 'size': sz, 'instrs': ic}
        for fv, (sz, ic) in sorted_by_size[:30]
    ],
    'top_30_by_vcall_count': [
        {'va': f"0x{fv:08x}", 'vcalls': vc, 'size': func_sizes[fv][0]}
        for fv, vc in sorted_by_vcall[:30] if vc >= 10
    ],
    'top_30_by_helper_count': [
        {'va': f"0x{fv:08x}", 'helpers': hc, 'vcalls': func_vcall_count[fv],
         'size': func_sizes[fv][0]}
        for fv, hc in sorted_by_helper[:30] if hc >= 5
    ],
    'intersection_all_three': [
        {'va': f"0x{fv:08x}", 'size': func_sizes[fv][0],
         'instrs': func_sizes[fv][1], 'vcalls': func_vcall_count[fv],
         'helpers': func_helper_count[fv]}
        for fv in sorted(intersection_3, key=lambda x: -func_sizes[x][0])
    ],
    'intersection_any_two': [
        {'va': f"0x{fv:08x}", 'size': func_sizes[fv][0],
         'instrs': func_sizes[fv][1], 'vcalls': func_vcall_count[fv],
         'helpers': func_helper_count[fv]}
        for fv in sorted(intersection_2, key=lambda x: -func_sizes[x][0])[:20]
    ],
}

with open(f"{OUT_DIR}/serialize_candidates.json", 'w', encoding='utf-8') as f:
    json.dump(report, f, indent=2, ensure_ascii=False)

print(f"\n[*] Сохранено: {OUT_DIR}/serialize_candidates.json")
print("\nГотово.")
