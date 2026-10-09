#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Дизассемблируем кандидатов на serialize-методы напрямую,
без проверки пролога 55 8B EC.
"""
import sys, struct, re, json
from collections import Counter
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
    """va — абсолютный VA (с учётом IMAGE_BASE)."""
    rva = va - IMAGE_BASE
    for s in pe.sections:
        if s.VirtualAddress <= rva < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (rva - s.VirtualAddress)
    return None

def disasm_from(va, max_instrs=2000):
    """Disassemble starting from absolute VA, follow jmps, stop on ret."""
    instrs = []
    cur_va = va
    visited = set()
    while cur_va and len(instrs) < max_instrs:
        if cur_va in visited:
            break
        visited.add(cur_va)
        off = va_to_offset(cur_va)
        if off is None or off + 16 > len(pe.__data__):
            break
        chunk = pe.__data__[off:off+16]
        ins_list = list(md.disasm(chunk, cur_va, count=1))
        if not ins_list:
            break
        ins = ins_list[0]
        instrs.append(ins)
        if ins.mnemonic == 'ret':
            break
        if ins.mnemonic == 'jmp':
            try:
                target = int(ins.op_str, 0)
                if IMAGE_BASE + text_sec.VirtualAddress <= target < IMAGE_BASE + text_sec.VirtualAddress + text_sec.Misc_VirtualSize:
                    cur_va = target
                    continue
            except ValueError:
                pass
            break
        cur_va += ins.size
    return instrs

# Загрузить список кандидатов из vtable_analysis.json
with open(f"{OUT_DIR}/vtable_analysis.json") as f:
    vt_analysis = json.load(f)

# Уникальные метод-кандидаты (vtable[2])
candidates = set()
for c in vt_analysis['top_30_vtables_by_size']:
    candidates.add(int(c['method_at_offset_8'], 16))
for c in vt_analysis['top_30_method2_functions']:
    candidates.add(int(c['va'], 16))

print(f"[*] Анализируем {len(candidates)} уникальных кандидатов serialize-методов...")

# Анализируем каждый
results = []
for fv in sorted(candidates):
    instrs = disasm_from(fv, max_instrs=3000)
    if not instrs:
        # Возможно, это не функция, а данные. Проверим первые 32 байта.
        off = va_to_offset(fv)
        if off is None:
            continue
        first_bytes = pe.__data__[off:off+32].hex()
        results.append({
            'va': fv,
            'instrs': 0,
            'size': 0,
            'direct_calls': 0,
            'indirect_calls': 0,
            'helper_calls': 0,
            'first_bytes': first_bytes,
            'note': 'no disassembly possible',
        })
        continue
    
    size = sum(ins.size for ins in instrs)
    direct_calls = sum(1 for ins in instrs if ins.mnemonic == 'call' and '[' not in ins.op_str)
    indirect_calls = sum(1 for ins in instrs if ins.mnemonic == 'call' and '[' in ins.op_str)
    # helper calls = вызовы 0x4130/0x4180 (это base serialize helpers)
    helper_calls = 0
    for ins in instrs:
        if ins.mnemonic == 'call':
            try:
                t = int(ins.op_str, 0)
                if t in (0x4130, 0x4180):
                    helper_calls += 1
            except ValueError:
                pass
    
    results.append({
        'va': fv,
        'instrs': len(instrs),
        'size': size,
        'direct_calls': direct_calls,
        'indirect_calls': indirect_calls,
        'helper_calls': helper_calls,
        'first_bytes': '',
    })

# Отсортировать по размеру
results.sort(key=lambda x: -x['size'])

print(f"\n{'VA':12s} {'instrs':>7s} {'size':>6s} {'d_call':>7s} {'i_call':>7s} {'help':>5s}  first_bytes")
print("-" * 100)
for r in results[:30]:
    fb = r.get('first_bytes', '')[:32]
    print(f"0x{r['va']:08x}   {r['instrs']:7d} {r['size']:6d} {r['direct_calls']:7d} {r['indirect_calls']:7d} {r['helper_calls']:5d}  {fb}")

# Сохранить самый интересный кандидат на дизассемблирование
# Критерий: много инструкций + много косвенных call-ов (vtable-вызовы)
# Это будет самый "глубокий" serialize-метод
best_candidates = [r for r in results if r['size'] > 100 and r['indirect_calls'] > 3]
print(f"\n[*] Кандидаты с size>100 и indirect_calls>3: {len(best_candidates)}")
for r in best_candidates[:10]:
    print(f"  ⭐ 0x{r['va']:08x}  size={r['size']}, indirect_calls={r['indirect_calls']}, helper_calls={r['helper_calls']}")

# Дизассемблировать топ-5 кандидатов полностью и сохранить
print("\n[*] Дизассемблируем топ-5 кандидатов полностью...")
for r in best_candidates[:5]:
    fv = r['va']
    instrs = disasm_from(fv, max_instrs=50000)
    
    with open(f"{OUT_DIR}/serialize_method_0x{fv:08x}.asm", 'w', encoding='utf-8') as f:
        f.write(f"; Serialize method candidate at 0x{fv:08x}\n")
        f.write(f"; Instructions: {len(instrs)}\n")
        f.write(f"; Size: {sum(i.size for i in instrs)} bytes\n")
        f.write(f"; Direct calls: {sum(1 for i in instrs if i.mnemonic == 'call' and '[' not in i.op_str)}\n")
        f.write(f"; Indirect calls: {sum(1 for i in instrs if i.mnemonic == 'call' and '[' in i.op_str)}\n\n")
        for ins in instrs:
            f.write(f"0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}\n")
    
    print(f"  Saved: {OUT_DIR}/serialize_method_0x{fv:08x}.asm ({len(instrs)} instrs)")

# Также сохраним результаты в JSON
with open(f"{OUT_DIR}/serialize_method_analysis.json", 'w', encoding='utf-8') as f:
    json.dump([
        {**r, 'va': f"0x{r['va']:08x}"}
        for r in results[:30]
    ], f, indent=2, ensure_ascii=False)

print(f"\n[*] Saved: {OUT_DIR}/serialize_method_analysis.json")
