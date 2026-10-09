#!/usr/bin/env python3
"""Find xrefs to save-related strings — corrected with image base."""
import sys, struct
sys.path.insert(0, '/home/z/.local/lib/python3.13/site-packages')
import pefile, capstone

pe = pefile.PE('/home/z/my-project/upload/heroes3.exe', fast_load=True)
pe.parse_data_directories()

IMAGE_BASE = pe.OPTIONAL_HEADER.ImageBase
print(f"Image base: 0x{IMAGE_BASE:08x}")

data = pe.__data__

# Target strings with their RVAs (file offset == RVA here since these are in .data)
targets = [
    (b'H3SVG',       0x277d38),
    (b'H3SVC',       0x277d40),
    (b'*.gm?',       0x2834dc),
    (b'*.cgm',       0x2834d4),
    (b'NEWGAME.gm1', 0x28338c),
    (b'AUTOSAVE.',   0x2839b8),
]

text_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.text'):
        text_sec = s
        break

text_data = pe.__data__[text_sec.PointerToRawData:text_sec.PointerToRawData + text_sec.SizeOfRawData]
text_va_base = text_sec.VirtualAddress  # = 0x1000 typically

print(f".text: VA=0x{text_va_base:08x}, size={len(text_data)}")

# Ищем absolute VA (image base + RVA) в .text
print("\n[*] Searching for absolute VA references (image_base + RVA)...")
all_xrefs = []
for s_bytes, rva in targets:
    abs_va = IMAGE_BASE + rva
    target_bytes = struct.pack('<I', abs_va)
    pos = 0
    xrefs = []
    while True:
        idx = text_data.find(target_bytes, pos)
        if idx < 0: break
        xref_va = text_va_base + idx
        xrefs.append(xref_va)
        pos = idx + 1
    print(f"  {s_bytes!r} (abs VA 0x{abs_va:08x}): {len(xrefs)} xrefs in .text")
    for x in xrefs[:5]:
        print(f"    at 0x{x:08x}")
        all_xrefs.append((s_bytes, x))

# Дизассемблируем окрестности каждого xref
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail = True

def disasm_around(va, before=128, after=256):
    start_va = max(va - before, text_sec.VirtualAddress)
    end_va = min(va + after, text_sec.VirtualAddress + text_sec.Misc_VirtualSize)
    start_off = pe.sections.__getitem__ if False else None
    # Compute file offset
    for s in pe.sections:
        if s.VirtualAddress <= start_va < s.VirtualAddress + s.Misc_VirtualSize:
            start_off = s.PointerToRawData + (start_va - s.VirtualAddress)
        if s.VirtualAddress <= end_va < s.VirtualAddress + s.Misc_VirtualSize:
            end_off = s.PointerToRawData + (end_va - s.VirtualAddress)
    chunk = pe.__data__[start_off:end_off]
    return list(md.disasm(chunk, start_va))

print("\n[*] Writing disassembly of xref neighborhoods...")
with open('/home/z/my-project/work_disasm/magic_xrefs_disasm.txt', 'w', encoding='utf-8') as f:
    f.write(f"# Disassembly around xrefs to savegame magic strings\n")
    f.write(f"# Image base: 0x{IMAGE_BASE:08x}\n")
    f.write(f"# Total xref locations: {len(all_xrefs)}\n\n")
    for s_bytes, xref_va in all_xrefs:
        f.write(f"=== Xref at 0x{xref_va:08x} (refs to {s_bytes!r}) ===\n")
        instrs = disasm_around(xref_va, before=128, after=384)
        for ins in instrs:
            marker = "  <=== XREF" if ins.address == xref_va else ""
            f.write(f"  0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}{marker}\n")
        f.write("\n")
        # Дизассемблируем вперёд на 1KB — ищем, какие функции вызываются рядом
        f.write(f"=== Forward trace from 0x{xref_va:08x} ===\n")
        instrs = disasm_around(xref_va, before=0, after=2048)
        for ins in instrs:
            f.write(f"  0x{ins.address:08x}: {ins.mnemonic:8s} {ins.op_str}\n")
            # Останавливаемся на ret/long jmp
            if ins.mnemonic == 'ret' or (ins.mnemonic == 'jmp' and not ins.op_str.startswith('0x')):
                break
        f.write("\n")

print(f"[*] Disasm saved to /home/z/my-project/work_disasm/magic_xrefs_disasm.txt")
