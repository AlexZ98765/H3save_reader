#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Шаг 3a: Дизассемблирование heroes3.exe для поиска функций сериализации сейвов.

Стратегия:
1. Парсим PE-структуру (секции, импорты, экспорты).
2. Извлекаем все строки (string-extraction), ищем ключевые:
   - "H3SVG", "H3SVC" (magic сейвов)
   - "*.GM1", "*.CGM"
   - "SaveGame", "LoadGame", "Serialize"
   - Имена классов: "CGameState", "CMap", "CHero", "CPlayer"
3. Ищем ссылки на эти строки (xrefs) — там и находятся функции сериализации.
4. Дизассемблируем найденные функции через capstone.
5. Анализируем вызовы CLoadFile / CSaveFile / CSerializer методов.
"""

import sys
import os
import struct
import re
import json
from collections import defaultdict, Counter

# Добавляем user-site для pefile/capstone
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')

import pefile
import capstone

EXE_PATH = "/home/z/my-project/upload/heroes3.exe"
OUT_DIR = "/home/z/my-project/work_disasm"
os.makedirs(OUT_DIR, exist_ok=True)

print(f"[*] Loading PE: {EXE_PATH}")
pe = pefile.PE(EXE_PATH, fast_load=True)
pe.parse_data_directories()

# === 1. Базовая информация о PE ===
print("\n=== PE Info ===")
print(f"Image base:    0x{pe.OPTIONAL_HEADER.ImageBase:08x}")
print(f"Entry point:   0x{pe.OPTIONAL_HEADER.AddressOfEntryPoint:08x}")
print(f"Size of code:  {pe.OPTIONAL_HEADER.SizeOfCode} bytes")
print(f"Number of sections: {len(pe.sections)}")

print("\n=== Sections ===")
sections_info = []
for s in pe.sections:
    name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    info = {
        'name': name,
        'va_start': s.VirtualAddress,
        'va_end': s.VirtualAddress + s.Misc_VirtualSize,
        'raw_size': s.SizeOfRawData,
        'raw_ptr': s.PointerToRawData,
        'entropy': s.get_entropy(),
    }
    sections_info.append(info)
    print(f"  {name:8s} VA=0x{s.VirtualAddress:08x}-0x{s.VirtualAddress + s.Misc_VirtualSize:08x} "
          f"raw=0x{s.PointerToRawData:08x} sz={s.SizeOfRawData:8d} ent={s.get_entropy():.2f}")

# Сохраняем отображение VA → file offset
def va_to_offset(va):
    """Convert VA (relative to image base) to file offset."""
    for s in pe.sections:
        if s.VirtualAddress <= va < s.VirtualAddress + s.Misc_VirtualSize:
            return s.PointerToRawData + (va - s.VirtualAddress)
    return None

# === 2. Извлекаем все ASCII строки >= 5 символов ===
print("\n[*] Extracting ASCII strings...")
data = pe.__data__
ascii_strings = []
str_regex = re.compile(rb'[\x20-\x7e]{5,}')
text_section = None
for s in pe.sections:
    name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    if name in ('.text', '.rdata', '.data'):
        if text_section is None or name == '.rdata':
            text_section = s

# Сканируем все секции (строки могут быть в .rdata)
for s in pe.sections:
    sec_name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    sec_data = pe.__data__[s.PointerToRawData:s.PointerToRawData + s.SizeOfRawData]
    for m in str_regex.finditer(sec_data):
        file_off = s.PointerToRawData + m.start()
        va = s.VirtualAddress + m.start()
        ascii_strings.append({
            'file_off': file_off,
            'va': va,
            'section': sec_name,
            'value': m.group().decode('ascii', errors='replace'),
            'length': len(m.group()),
        })

print(f"[*] Found {len(ascii_strings)} ASCII strings total")

# === 3. Ищем ключевые строки, связанные с сериализацией ===
print("\n[*] Searching for savegame-related strings...")

SAVEGAME_KEYWORDS = [
    # Magic
    'H3SVG', 'H3SVC', 'HCHRONSVG',
    # File extensions
    '.GM1', '.CGM', '.gm1', '.cgm',
    # Function/class names (RTTI / debug strings)
    'SaveGame', 'LoadGame', 'savegame', 'saveGame',
    'CGameState', 'CGameInfo', 'CMap', 'CHero', 'CPlayer', 'CCreature',
    'Serialize', 'serializer', 'CLoadFile', 'CSaveFile', 'CSerializer',
    'Save', 'Load',
    'GameState', 'PlayerState', 'HeroState',
    # Section/field names
    'heroes', 'towns', 'objects', 'map', 'player',
    'fog', 'FoW', 'visible',
    # Error messages often contain useful context
    'Cannot open', 'Cannot save', 'Cannot load', 'corrupt',
    'Invalid save', 'Invalid format', 'magic',
    'version', 'Version',
]

interesting_strings = []
for s in ascii_strings:
    val = s['value']
    for kw in SAVEGAME_KEYWORDS:
        if kw in val and len(val) < 200:
            interesting_strings.append(s)
            break

# Дедуп по значению, оставляя первое вхождение
seen = set()
deduped = []
for s in interesting_strings:
    if s['value'] not in seen:
        seen.add(s['value'])
        deduped.append(s)
interesting_strings = deduped

print(f"[*] Found {len(interesting_strings)} interesting strings")

# Сохраняем все интересующие строки
with open(f"{OUT_DIR}/interesting_strings.txt", 'w', encoding='utf-8') as f:
    f.write("# Strings of interest from heroes3.exe\n")
    f.write(f"# Total: {len(interesting_strings)}\n\n")
    for s in sorted(interesting_strings, key=lambda x: x['va']):
        f.write(f"0x{s['va']:08x} [{s['section']}] {s['value']}\n")

# Показываем топ-50
print("\n[*] Top 50 interesting strings:")
for s in sorted(interesting_strings, key=lambda x: x['va'])[:50]:
    print(f"  0x{s['va']:08x} [{s['section']:7s}] {s['value'][:100]}")

# === 4. Сохраняем все строки в файл для дальнейшего поиска ===
print(f"\n[*] Saving all {len(ascii_strings)} strings to {OUT_DIR}/all_strings.txt")
with open(f"{OUT_DIR}/all_strings.txt", 'w', encoding='utf-8') as f:
    for s in sorted(ascii_strings, key=lambda x: x['va']):
        f.write(f"0x{s['va']:08x} [{s['section']:7s}] len={s['length']:4d}  {s['value']}\n")

# === 5. Ищем xrefs на ключевые строки ===
print("\n[*] Searching for xrefs to savegame magic strings...")

# Магические строки для поиска
magic_strings = ['H3SVG', 'H3SVC', 'HCHRONSVG']
magic_xrefs = []

# Поищем прямые ссылки на эти строки (через push offset / mov reg, offset)
# В .text секции ищем patern: push <imm32> где imm32 — VA строки
text_sec = None
for s in pe.sections:
    name = s.Name.decode('utf-8', errors='replace').rstrip('\x00')
    if name == '.text':
        text_sec = s
        break

if text_sec:
    text_data = pe.__data__[text_sec.PointerToRawData:text_sec.PointerToRawData + text_sec.SizeOfRawData]
    text_va_base = text_sec.VirtualAddress
    
    for ms in magic_strings:
        ms_bytes = ms.encode('ascii')
        # Найдём все вхождения строки в файле
        for s_info in ascii_strings:
            if s_info['value'] == ms:
                target_va = s_info['va']
                # Ищем 4-байтовое значение target_va (little-endian) в .text
                target_bytes = struct.pack('<I', target_va)
                pos = 0
                while True:
                    idx = text_data.find(target_bytes, pos)
                    if idx == -1:
                        break
                    xref_va = text_va_base + idx
                    magic_xrefs.append({
                        'magic': ms,
                        'string_va': target_va,
                        'xref_va': xref_va,
                    })
                    pos = idx + 1
        print(f"  '{ms}': {sum(1 for x in magic_xrefs if x['magic']==ms)} xrefs")

# Дедуплицируем (один и тот же xref может входить в паттерн push+mov)
unique_xrefs = list({x['xref_va'] for x in magic_xrefs})
print(f"[*] {len(unique_xrefs)} unique xref locations")

# === 6. Дизассемблируем окрестности каждого xref ===
print("\n[*] Disassembling around xref locations...")

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def disasm_at(va, before=64, after=128):
    """Disassemble instructions in window [va-before, va+after)."""
    start_va = max(va - before, text_sec.VirtualAddress)
    end_va = min(va + after, text_sec.VirtualAddress + text_sec.Misc_VirtualSize)
    start_off = va_to_offset(start_va)
    end_off = va_to_offset(end_va)
    if start_off is None or end_off is None:
        return []
    chunk = pe.__data__[start_off:end_off]
    return list(md.disasm(chunk, start_va))

# Сохраняем дизассемблированный контекст для каждого xref
with open(f"{OUT_DIR}/magic_xrefs_disasm.txt", 'w', encoding='utf-8') as f:
    f.write(f"# Disassembly around xrefs to magic strings\n")
    f.write(f"# Total xref locations: {len(unique_xrefs)}\n\n")
    
    for i, xref_va in enumerate(sorted(unique_xrefs)):
        magic_here = [x['magic'] for x in magic_xrefs if x['xref_va'] == xref_va]
        f.write(f"=== Xref #{i} at 0x{xref_va:08x} (refs: {', '.join(set(magic_here))}) ===\n")
        instrs = disasm_at(xref_va, before=128, after=256)
        for ins in instrs:
            marker = "  <== XREF" if ins.address == xref_va else ""
            f.write(f"  0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}{marker}\n")
        f.write("\n")

print(f"[*] Disassembly saved to {OUT_DIR}/magic_xrefs_disasm.txt")

# === 7. Анализируем RTTI (Run-Time Type Information) ===
print("\n[*] Searching for RTTI class names...")
rtti_keywords = [
    'CGameState', 'CGameInfo', 'CMap', 'CMapHeader', 'CHero', 'CPlayer',
    'CCreature', 'CArtifact', 'CSpell', 'CTown', 'CBuilding', 'CQuest',
    'CPlayerState', 'CHeroState', 'CObjectList', 'CMapObject',
    'CSaveFile', 'CLoadFile', 'CSerializer', 'CLoadIntrospection',
    'CMapLogic', 'CGameHandler',
]
rtti_strings = []
for s in ascii_strings:
    for kw in rtti_keywords:
        if kw in s['value'] and len(s['value']) < 200:
            rtti_strings.append(s)
            break

# Дедуп
seen = set()
rtti_dedup = []
for s in rtti_strings:
    if s['value'] not in seen:
        seen.add(s['value'])
        rtti_dedup.append(s)
rtti_strings = rtti_dedup

print(f"[*] Found {len(rtti_strings)} RTTI-related strings")
with open(f"{OUT_DIR}/rtti_strings.txt", 'w', encoding='utf-8') as f:
    for s in sorted(rtti_strings, key=lambda x: x['va']):
        f.write(f"0x{s['va']:08x} [{s['section']:7s}] {s['value']}\n")

# === 8. Ищем ссылки на RTTI-строки в .text ===
print("\n[*] Searching xrefs to RTTI class names...")
rtti_xrefs = []
if text_sec:
    for s_info in rtti_strings:
        target_va = s_info['va']
        target_bytes = struct.pack('<I', target_va)
        pos = 0
        while True:
            idx = text_data.find(target_bytes, pos)
            if idx == -1:
                break
            xref_va = text_va_base + idx
            rtti_xrefs.append({
                'string': s_info['value'],
                'string_va': target_va,
                'xref_va': xref_va,
            })
            pos = idx + 1

# Группируем по строкам
by_class = defaultdict(list)
for x in rtti_xrefs:
    # Извлекаем имя класса из строки (например, "CGameState" из ".?AVCGameState@@")
    m = re.search(r'\.?AV?([A-Za-z_][A-Za-z0-9_]+)@@?', x['string'])
    class_name = m.group(1) if m else x['string'][:40]
    by_class[class_name].append(x['xref_va'])

print(f"[*] Found {len(rtti_xrefs)} xrefs to RTTI strings across {len(by_class)} unique class names")

print("\n[*] Top 30 classes by xref count:")
for class_name, xrefs in sorted(by_class.items(), key=lambda x: -len(x[1]))[:30]:
    print(f"  {class_name:40s} {len(xrefs):4d} xrefs")

# Сохраняем
with open(f"{OUT_DIR}/rtti_xrefs.json", 'w', encoding='utf-8') as f:
    json.dump({
        'total_xrefs': len(rtti_xrefs),
        'total_classes': len(by_class),
        'by_class': {
            cls: [f"0x{v:08x}" for v in sorted(set(vlist))]
            for cls, vlist in by_class.items()
        }
    }, f, indent=2, ensure_ascii=False)

# === 9. Импорты ===
print("\n[*] Imported DLLs and key functions:")
imports_summary = defaultdict(list)
if hasattr(pe, 'DIRECTORY_ENTRY_IMPORT'):
    for entry in pe.DIRECTORY_ENTRY_IMPORT:
        dll_name = entry.dll.decode('utf-8', errors='replace')
        funcs = [imp.name.decode('utf-8', errors='replace') if imp.name else f"ord_{imp.ordinal}"
                 for imp in entry.imports]
        imports_summary[dll_name] = funcs
        print(f"  {dll_name}: {len(funcs)} functions")
        # Подсветка интересных
        interesting = [f for f in funcs if any(kw in f.lower() for kw in
                       ['file', 'read', 'write', 'create', 'open', 'close', 'gzip', 'zlib', 'compress'])]
        if interesting:
            print(f"    Interesting: {interesting[:10]}")

# Сохраняем
with open(f"{OUT_DIR}/imports.txt", 'w', encoding='utf-8') as f:
    for dll, funcs in imports_summary.items():
        f.write(f"=== {dll} ===\n")
        for fn in funcs:
            f.write(f"  {fn}\n")
        f.write("\n")

# === 10. Итоговый JSON-отчёт ===
summary = {
    'exe_path': EXE_PATH,
    'exe_size': os.path.getsize(EXE_PATH),
    'md5': '15885af0254a3357913aecaab8c6453c',
    'image_base': f"0x{pe.OPTIONAL_HEADER.ImageBase:08x}",
    'entry_point': f"0x{pe.OPTIONAL_HEADER.AddressOfEntryPoint:08x}",
    'sections': [{'name': s['name'], 'va': f"0x{s['va_start']:08x}",
                  'size': s['raw_size']} for s in sections_info],
    'total_ascii_strings': len(ascii_strings),
    'interesting_strings_count': len(interesting_strings),
    'magic_string_xrefs_count': len(unique_xrefs),
    'rtti_class_xrefs_count': len(rtti_xrefs),
    'rtti_classes_count': len(by_class),
    'top_classes_by_xrefs': [
        {'class': cls, 'xref_count': len(xrefs),
         'first_xref': f"0x{min(xrefs):08x}"}
        for cls, xrefs in sorted(by_class.items(), key=lambda x: -len(x[1]))[:30]
    ],
    'output_dir': OUT_DIR,
    'artifacts': [
        'interesting_strings.txt',
        'all_strings.txt',
        'magic_xrefs_disasm.txt',
        'rtti_strings.txt',
        'rtti_xrefs.json',
        'imports.txt',
    ],
}

with open(f"{OUT_DIR}/disasm_summary.json", 'w', encoding='utf-8') as f:
    json.dump(summary, f, indent=2, ensure_ascii=False)

print(f"\n[*] Summary saved to {OUT_DIR}/disasm_summary.json")
print(f"[*] All artifacts in {OUT_DIR}/")
print("\nDone.")
