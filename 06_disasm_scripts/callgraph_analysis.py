#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3d: Реконструкция call-graph для функций сейвов.

Идея:
- magic_funcs (0xbbda0, 0xbc010, 0xbca60, 0xbe0b0) — генерят H3SVG/H3SVC и формируют buffer
- file_io_funcs (0x200210, 0x2207a4, 0x222cdb) — пишут через WriteFile
- Нужно найти путь: magic_func → ??? → file_io_func

Это и будет цепочка сериализации.
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

def find_func_start(target_va, max_back=8192):
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

# === 1. Построить глобальный call-graph всех функций ===
# Для каждой инструкции call в .text определим цель и функцию-вызывателя
print("[*] Строим глобальный call-graph...")

# Сначала найдём все функции (по прологам 55 8B EC)
all_funcs = set()
print("[*] Сканирование .text на прологи 55 8B EC...")
for i in range(len(text_data) - 3):
    if text_data[i] == 0x55 and text_data[i+1] == 0x8B and text_data[i+2] == 0xEC:
        all_funcs.add(text_va_base + i)

print(f"[*] Найдено функций: {len(all_funcs)}")

# Построим call-graph
callgraph = defaultdict(set)  # func_va -> set of called VAs
caller_graph = defaultdict(set)  # func_va -> set of callers
calls_in_func = defaultdict(list)  # func_va -> [(call_va, target_va), ...]

# Пройдём по всем call-инструкциям
# FF 15 imm32 → call dword ptr [imm32]  (косвенный, через IAT)
# E8 rel32 → call rel32  (прямой)
print("[*] Анализ call-инструкций...")
for i in range(len(text_data) - 5):
    op = text_data[i]
    if op == 0xE8:
        # Прямой call rel32
        rel = struct.unpack('<i', text_data[i+1:i+5])[0]
        call_va = text_va_base + i
        target_va = call_va + 5 + rel
        if text_va_base <= target_va < text_va_base + text_size:
            caller_func = find_func_start(call_va)
            if caller_func:
                callgraph[caller_func].add(target_va)
                caller_graph[target_va].add(caller_func)
                calls_in_func[caller_func].append((call_va, target_va))

print(f"[*] Call-graph содержит {len(callgraph)} функций с вызовами")

# === 2. Найти путь от magic_funcs к file_io_funcs через BFS ===
magic_funcs = {0xbbda0, 0xbc010, 0xbca60, 0xbe0b0}
file_io_funcs = {0x200210, 0x2207a4, 0x222cdb, 0x200240, 0x221326, 0x200180, 0x21a164, 0x183440, 0x1987a0}

print(f"\n[*] Ищем пути от magic_funcs к file_io_funcs (BFS, глубина до 10)...")
print(f"    Источники: {[hex(x) for x in magic_funcs]}")
print(f"    Цели:     {[hex(x) for x in file_io_funcs]}")

def bfs_path(start, targets, max_depth=10):
    """BFS find shortest path from start to any target."""
    visited = {start}
    queue = deque([(start, [start])])
    while queue:
        node, path = queue.popleft()
        if len(path) > max_depth:
            continue
        for nxt in callgraph.get(node, []):
            if nxt in targets:
                return path + [nxt]
            if nxt not in visited:
                visited.add(nxt)
                queue.append((nxt, path + [nxt]))
    return None

paths_found = []
for src in magic_funcs:
    for tgt in file_io_funcs:
        path = bfs_path(src, {tgt}, max_depth=8)
        if path:
            paths_found.append((src, tgt, path))
            print(f"  ✓ 0x{src:08x} → 0x{tgt:08x}: {' → '.join(f'0x{x:08x}' for x in path)}")

if not paths_found:
    print("  Прямых путей не найдено. Ищем в более широком диапазоне...")
    # Ищем любой путь к любой цели
    for src in magic_funcs:
        path = bfs_path(src, file_io_funcs, max_depth=10)
        if path:
            print(f"  ✓ 0x{src:08x} → 0x{path[-1]:08x}: {' → '.join(f'0x{x:08x}' for x in path)}")
            paths_found.append((src, path[-1], path))

# === 3. Для каждой magic-функции показать её прямые call'ы ===
print("\n[*] Прямые call'ы из magic_funcs:")
for fv in sorted(magic_funcs):
    targets = sorted(callgraph.get(fv, []))
    print(f"\n0x{fv:08x} вызывает {len(targets)} функций:")
    for t in targets:
        marker = " ⭐ FILE_IO" if t in file_io_funcs else ""
        print(f"    → 0x{t:08x}{marker}")

# === 4. Найти, кто вызывает magic_funcs ===
print("\n[*] Кто вызывает magic_funcs (callers):")
for fv in sorted(magic_funcs):
    callers = sorted(caller_graph.get(fv, []))
    print(f"\n0x{fv:08x} вызывается из {len(callers)} функций:")
    for c in callers[:15]:
        print(f"    ← 0x{c:08x}")

# === 5. Найти общие функции, вызываемые всеми magic_funcs ===
print("\n[*] Общие call-цели для magic_funcs:")
common_targets = None
for fv in magic_funcs:
    targets = set(callgraph.get(fv, []))
    if common_targets is None:
        common_targets = targets
    else:
        common_targets &= targets
if common_targets:
    for t in sorted(common_targets):
        marker = " ⭐ FILE_IO" if t in file_io_funcs else ""
        print(f"  → 0x{t:08x}{marker}")

# === 6. Сохранить call-graph целиком для magic_funcs и file_io_funcs ===
# Сохранить подсетевой граф: magic_funcs + их соседи 2-го уровня + file_io_funcs
interesting_set = set()
for fv in magic_funcs | file_io_funcs:
    interesting_set.add(fv)
    for t in callgraph.get(fv, []):
        interesting_set.add(t)
        for tt in callgraph.get(t, []):
            interesting_set.add(tt)
    for c in caller_graph.get(fv, []):
        interesting_set.add(c)
        for cc in caller_graph.get(c, []):
            interesting_set.add(cc)

print(f"\n[*] Подсетевой граф содержит {len(interesting_set)} функций")

# Сохранить в JSON
subgraph = {}
for fv in interesting_set:
    subgraph[f"0x{fv:08x}"] = {
        'calls': [f"0x{x:08x}" for x in sorted(callgraph.get(fv, []))],
        'called_by': [f"0x{x:08x}" for x in sorted(caller_graph.get(fv, []))],
        'is_magic_func': fv in magic_funcs,
        'is_file_io_func': fv in file_io_funcs,
    }

with open(f"{OUT_DIR}/save_callgraph.json", 'w', encoding='utf-8') as f:
    json.dump({
        'image_base': f"0x{IMAGE_BASE:08x}",
        'magic_funcs': [f"0x{x:08x}" for x in sorted(magic_funcs)],
        'file_io_funcs': [f"0x{x:08x}" for x in sorted(file_io_funcs)],
        'paths_found': [
            {'from': f"0x{s:08x}", 'to': f"0x{t:08x}",
             'path': [f"0x{x:08x}" for x in p]}
            for s, t, p in paths_found
        ],
        'subgraph_size': len(interesting_set),
        'subgraph': subgraph,
    }, f, indent=2, ensure_ascii=False)

print(f"[*] Сохранено: {OUT_DIR}/save_callgraph.json")
