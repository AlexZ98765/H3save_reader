#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Поиск vtable-ов и реконструкция иерархии классов HoMM3.

Стратегия:
1. Найти все возможные vtable'ы (участки .rdata с последовательностями
   указателей на функции в .text).
2. Найти конструкторы (где vtable записывается в объект: mov [reg], offset vtable).
3. Группировать vtable'ы по "похожести" (если 2 vtable'а разделяют методы —
   это, вероятно, родитель и наследник).
4. Для каждого vtable проанализировать методы — особенно метод [vtable+8]
   (вероятно, serialize).
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
data_sec = None
for s in pe.sections:
    name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    if name == '.text':
        text_sec = s
    elif name == '.rdata':
        rdata_sec = s
    elif name == '.data':
        data_sec = s

text_data = pe.__data__[text_sec.PointerToRawData:text_sec.PointerToRawData + text_sec.SizeOfRawData]
text_va_base = text_sec.VirtualAddress
text_size = text_sec.Misc_VirtualSize

rdata_data = pe.__data__[rdata_sec.PointerToRawData:rdata_sec.PointerToRawData + rdata_sec.SizeOfRawData]
rdata_va_start = IMAGE_BASE + rdata_sec.VirtualAddress
rdata_va_end = rdata_va_start + rdata_sec.Misc_VirtualSize

data_data = pe.__data__[data_sec.PointerToRawData:data_sec.PointerToRawData + data_sec.SizeOfRawData]
data_va_start = IMAGE_BASE + data_sec.VirtualAddress

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

# === 1. Найти все функции ===
def va_to_offset(va):
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

all_funcs = set()
for i in range(len(text_data) - 3):
    if text_data[i] == 0x55 and text_data[i+1] == 0x8B and text_data[i+2] == 0xEC:
        all_funcs.add(text_va_base + i)

print(f"[*] Функций: {len(all_funcs)}")

# === 2. Найти все "vtable-ы" в .rdata ===
# Vtable — это последовательность 4-байтовых указателей на функции в .text
print("\n[*] Поиск vtable-ов в .rdata...")

vtables = []  # list of {va, size, methods: [func_va, ...]}

i = 0
while i + 4 <= len(rdata_data):
    # Прочитать 4-байтовое значение
    val = struct.unpack('<I', rdata_data[i:i+4])[0]
    abs_val = IMAGE_BASE + rdata_sec.VirtualAddress + i  # абсолютный VA этого указателя
    
    # Проверить, указывает ли значение в .text
    if text_va_base + IMAGE_BASE <= val < text_va_base + IMAGE_BASE + text_size:
        # Найдена последовательность — посчитаем длину
        methods = [val]
        j = i + 4
        while j + 4 <= len(rdata_data):
            m_val = struct.unpack('<I', rdata_data[j:j+4])[0]
            if text_va_base + IMAGE_BASE <= m_val < text_va_base + IMAGE_BASE + text_size:
                methods.append(m_val)
                j += 4
            else:
                break
        # Vtable должен иметь хотя бы 2 метода (обычно 3+)
        if len(methods) >= 2:
            vtable_va = IMAGE_BASE + rdata_sec.VirtualAddress + i
            vtables.append({
                'va': vtable_va,
                'rva': rdata_sec.VirtualAddress + i,
                'size': len(methods) * 4,
                'methods': methods,
                'method_count': len(methods),
            })
            i = j  # пропускаем найденный vtable
            continue
    i += 4

print(f"[*] Найдено vtable-ов: {len(vtables)}")
print(f"[*] Общее количество методов во всех vtable: {sum(v['method_count'] for v in vtables)}")

# Распределение по размеру
size_dist = Counter(v['method_count'] for v in vtables)
print(f"\nРаспределение vtable по количеству методов:")
for cnt, num in sorted(size_dist.items())[:30]:
    print(f"  {cnt:3d} методов: {num} vtable-ов")

# === 3. Найти xref'ы на vtable-ы в .text (это конструкторы) ===
print("\n[*] Поиск конструкторов (mov [reg], offset vtable)...")

# Паттерны записи vtable в объект:
# C7 01 <imm32>      mov dword ptr [ecx], imm32
# C7 00 <imm32>      mov dword ptr [eax], imm32
# C7 06 <imm32>      mov dword ptr [esi], imm32
# C7 07 <imm32>      mov dword ptr [edi], imm32
# 89 0D <imm32>      mov dword ptr [imm32], ecx (для static/singletons)
# A3 <imm32>         mov [imm32], eax

vtable_xrefs = defaultdict(list)  # vtable_va -> list of (caller_va, instruction_text)
patterns = [
    (b'\xc7\x01', 'mov dword ptr [ecx], imm32'),
    (b'\xc7\x00', 'mov dword ptr [eax], imm32'),
    (b'\xc7\x06', 'mov dword ptr [esi], imm32'),
    (b'\xc7\x07', 'mov dword ptr [edi], imm32'),
    (b'\xc7\x02', 'mov dword ptr [edx], imm32'),
    (b'\xc7\x03', 'mov dword ptr [ebx], imm32'),
    (b'\xc7\x05', 'mov dword ptr [ebp], imm32'),  # rare
]

# Также: 89 0D <imm32> — mov [imm32], ecx (singleton init)
# B8 <imm32> — mov eax, imm32 (load vtable ptr)

# Список всех vtable VA-ов для быстрого поиска
vtable_va_set = set(v['va'] for v in vtables)

# Найти все C7 XX <imm32> где imm32 — это VA vtable
for pat_idx in range(len(text_data) - 6):
    b0 = text_data[pat_idx]
    b1 = text_data[pat_idx + 1]
    if b0 == 0xC7 and b1 in (0x00, 0x01, 0x02, 0x03, 0x06, 0x07):
        # Считать imm32
        imm = struct.unpack('<I', text_data[pat_idx+2:pat_idx+6])[0]
        if imm in vtable_va_set:
            caller_va = text_va_base + pat_idx
            vtable_xrefs[imm].append(caller_va)

# Также ищем mov [reg+imm8], vtable — C7 40-47 NN imm32
for pat_idx in range(len(text_data) - 7):
    b0 = text_data[pat_idx]
    b1 = text_data[pat_idx + 1]
    if b0 == 0xC7 and 0x40 <= b1 <= 0x47:
        # offset = 1 byte
        imm = struct.unpack('<I', text_data[pat_idx+3:pat_idx+7])[0]
        if imm in vtable_va_set:
            caller_va = text_va_base + pat_idx
            vtable_xrefs[imm].append(caller_va)

# Также ищем mov dword ptr [reg+imm32], vtable — C7 80-87 imm32 imm32
for pat_idx in range(len(text_data) - 10):
    b0 = text_data[pat_idx]
    b1 = text_data[pat_idx + 1]
    if b0 == 0xC7 and 0x80 <= b1 <= 0x87:
        imm = struct.unpack('<I', text_data[pat_idx+6:pat_idx+10])[0]
        if imm in vtable_va_set:
            caller_va = text_va_base + pat_idx
            vtable_xrefs[imm].append(caller_va)

# Также mov eax, imm32 (B8) — часто встречается в конструкторах перед mov [ecx], eax
# Но это может быть и другое. Пропускаем — нужно слишком много false positives.

print(f"\n[*] Vtable-ов с xref'ами в .text: {len(vtable_xrefs)}/{len(vtables)}")

# === 4. Анализ: для каждого vtable найдём функцию-конструктор ===
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

vtable_to_constructors = {}  # vtable_va -> [func_va, ...]
for vtable_va, callers in vtable_xrefs.items():
    ctors = set()
    for cv in callers:
        fs = find_func_start(cv)
        if fs:
            ctors.add(fs)
    vtable_to_constructors[vtable_va] = sorted(ctors)

# === 5. Ранжировать vtable-ы по "интересности" ===
# Главный критерий: vtable должен иметь как минимум 3-4 метода,
# один из которых вызывается из SAVE_WRITER_CONTENT (через [eax+8])

# Найдём все функции, которые вызываются из SAVE_WRITER_CONTENT и SAVE_READER_CONTENT
# Эти функции — candidate serialize-методы

# Из предыдущего анализа:
# SAVE_WRITER_CONTENT (0xbc290) делает: call dword ptr [eax + 8]
# Где eax = [ebx] = vtable. Значит, метод [vtable+8] — это serialize!

# Найдём все возможные serialize-методы — это все функции, которые
# часто вызываются через call dword ptr [eax+8] (FF 50 08) или
# call dword ptr [edx+8] (FF 52 08) или call dword ptr [ecx+8] (FF 51 08)

print("\n[*] Поиск call-ов через [reg+0x8] (виртуальный метод vtable[2])...")
vtable_method_2_callers = []  # list of (caller_va, register)
for i in range(len(text_data) - 3):
    if text_data[i] == 0xFF and text_data[i+1] in (0x50, 0x51, 0x52, 0x53, 0x56, 0x57) and text_data[i+2] == 0x08:
        reg_names = {0x50:'eax', 0x51:'ecx', 0x52:'edx', 0x53:'ebx', 0x56:'esi', 0x57:'edi'}
        caller_va = text_va_base + i
        vtable_method_2_callers.append((caller_va, reg_names[text_data[i+1]]))

print(f"  Найдено вызовов call [reg+0x8]: {len(vtable_method_2_callers)}")

# === 6. Для каждого vtable проанализировать метод [2] (смещение +8) ===
# Это кандидат на serialize
print("\n[*] Анализ методов vtable[2] (потенциальные serialize-методы)...")

serialize_candidates = []
for vt in vtables:
    if vt['method_count'] >= 2:
        method_2 = vt['methods'][1]  # vtable[1] = смещение +4? Нет, vtable[2] = смещение +8
        # Подождите: vtable[0] = смещение 0, vtable[1] = смещение 4, vtable[2] = смещение 8
        # Так что метод по смещению +8 — это methods[2]
        if vt['method_count'] >= 3:
            method_at_8 = vt['methods'][2]
            serialize_candidates.append({
                'vtable_va': vt['va'],
                'vtable_size': vt['method_count'],
                'method_at_offset_8': method_at_8,
                'constructors': vtable_to_constructors.get(vt['va'], []),
                'xref_count': len(vtable_xrefs.get(vt['va'], [])),
            })

print(f"\nВсего кандидатов на serialize-методы (vtable[2]): {len(serialize_candidates)}")

# Ранжировать: топ по размеру vtable и количеству конструкторов
serialize_candidates.sort(key=lambda x: (-x['vtable_size'], -len(x['constructors'])))

print(f"\nТоп-30 vtable-ов по размеру (с методом vtable[2]):")
print(f"{'VTABLE VA':12s} {'methods':>8s} {'ctors':>6s} {'xrefs':>6s}  method_2_va")
print("-" * 80)
for c in serialize_candidates[:30]:
    print(f"0x{c['vtable_va']:08x}   {c['vtable_size']:8d} {len(c['constructors']):6d} {c['xref_count']:6d}  0x{c['method_at_offset_8']:08x}")

# === 7. Найти функции, которые часто вызываются через [reg+0x8] ===
# Это покажет "самые популярные" serialize-методы
method_2_call_count = Counter()
for vt in vtables:
    if vt['method_count'] >= 3:
        method = vt['methods'][2]
        method_2_call_count[method] += 1

print(f"\nТоп-30 функций, являющихся vtable[2] (потенциальные serialize-методы):")
print(f"{'METHOD VA':12s} {'vtbl_count':>11s}")
print("-" * 50)
for fv, cnt in method_2_call_count.most_common(30):
    if cnt >= 1:
        print(f"0x{fv:08x}   {cnt:11d}")

# === 8. Найти пересечение: функции, которые:
#   - Являются vtable[2]
#   - Вызываются из SAVE_WRITER_CONTENT или SAVE_READER_CONTENT
#   - Имеют тело, состоящее из множества call dword ptr [eax+8]

# Дизассемблируем SAVE_WRITER_CONTENT и найдём, какие методы он вызывает
print("\n[*] Анализ SAVE_WRITER_CONTENT (0xbc290) — какие методы вызывает...")

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

# === 9. Сохранить результаты ===
result = {
    'total_vtables': len(vtables),
    'vtables_with_xrefs': len(vtable_xrefs),
    'total_method2_calls': len(vtable_method_2_callers),
    'top_30_vtables_by_size': [
        {
            'vtable_va': f"0x{c['vtable_va']:08x}",
            'method_count': c['vtable_size'],
            'method_at_offset_8': f"0x{c['method_at_offset_8']:08x}",
            'constructor_count': len(c['constructors']),
            'constructors': [f"0x{x:08x}" for x in c['constructors'][:5]],
            'xref_count': c['xref_count'],
        }
        for c in serialize_candidates[:30]
    ],
    'top_30_method2_functions': [
        {'va': f"0x{fv:08x}", 'in_vtable_count': cnt}
        for fv, cnt in method_2_call_count.most_common(30)
    ],
}

with open(f"{OUT_DIR}/vtable_analysis.json", 'w', encoding='utf-8') as f:
    json.dump(result, f, indent=2, ensure_ascii=False)

print(f"\n[*] Сохранено: {OUT_DIR}/vtable_analysis.json")

# === 10. Вывести короткий список наиболее вероятных serialize-методов ===
print("\n" + "=" * 70)
print("Наиболее вероятные методы serialize() (vtable[2] с наибольшим размером vtable)")
print("=" * 70)

# Возьмём топ-20 по размеру vtable — самые "большие" классы
top_serialize_candidates = sorted(serialize_candidates,
                                   key=lambda x: (-x['vtable_size'], -len(x['constructors'])))[:20]

for c in top_serialize_candidates:
    method = c['method_at_offset_8']
    # Дизассемблируем метод
    instrs = disasm_function(method, max_instrs=5000)
    size = sum(ins.size for ins in instrs)
    # Посчитаем call-ы
    direct_calls = sum(1 for ins in instrs if ins.mnemonic == 'call' and not '[' in ins.op_str)
    indirect_calls = sum(1 for ins in instrs if ins.mnemonic == 'call' and '[' in ins.op_str)
    helper_calls = sum(1 for ins in instrs if ins.mnemonic == 'call' and '0x4130' in ins.op_str or '0x4180' in ins.op_str)
    
    print(f"\n  vtable 0x{c['vtable_va']:08x} (size={c['vtable_size']}) → method@+8 = 0x{method:08x}")
    print(f"    Body: {len(instrs)} instrs, {size} bytes, {direct_calls} direct calls, {indirect_calls} indirect calls, {helper_calls} helper calls")
    print(f"    Constructors: {len(c['constructors'])} (e.g. {[hex(x) for x in c['constructors'][:3]]})")

print("\nГотово.")
