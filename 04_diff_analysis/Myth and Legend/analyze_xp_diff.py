#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Детальный анализ 0012 → 0013 (герой получил 1000 опыта, новый уровень,
вторичный навык).
"""
import gzip, io, contextlib

@contextlib.contextmanager
def patch_gzip():
    reader = getattr(gzip, '_GzipReader', None)
    if reader and hasattr(reader, '_read_eof'):
        orig = reader._read_eof
        def patched(self):
            try: self._fp.read(8)
            except: pass
        reader._read_eof = patched
        try: yield
        finally: reader._read_eof = orig
    else:
        yield

def load(p):
    with open(p, 'rb') as f:
        data = f.read()
    with patch_gzip():
        with gzip.GzipFile(fileobj=io.BytesIO(data)) as gf:
            return gf.read()

A = load('/home/z/my-project/upload/0012.GM1')
B = load('/home/z/my-project/upload/0013.GM1')
print(f"A: {len(A)}, B: {len(B)}, diff: {len(B)-len(A)}")

RANGES = [
    (0x000003b6, 0x000003b7),
    (0x00068467, 0x00068469),
    (0x0006846e, 0x00068472),
    (0x0006d7a4, 0x0006d7a5),
    (0x0006d7aa, 0x0006d7ab),
    (0x00130b71, 0x00130b75),
    (0x00130b7e, 0x00130b80),
    (0x00130b89, 0x00130bea),
    (0x00130bf6, 0x00130c12),
    (0x0013e409, 0x0013e40a),
    (0x0013e420, 0x0013e421),
    (0x00157ec3, 0x00157ed7),
    (0x00157eec, 0x00157eed),
    (0x00157efa, 0x00157efb),
    (0x00157f00, 0x00157f09),
    (0x00157f0e, 0x00157f14),
    (0x00157fa4, 0x00157fa5),
    (0x00157fc0, 0x00157fc1),
    (0x00157fcd, 0x00157fce),
    (0x00170d31, 0x00170d32),
    (0x00170e4f, 0x00170e52),
    (0x00170f6d, 0x00170f72),
    (0x0017108d, 0x00171092),
    (0x001711ad, 0x001711b2),
    (0x001712cd, 0x001712d2),
    (0x001713ed, 0x001713f4),
    (0x0017150d, 0x00171516),
    (0x0017162f, 0x00171638),
    # plus tail
]

def hex_dump(data, start, end, prefix="    "):
    chunk = data[start:end]
    lines = []
    for i in range(0, len(chunk), 16):
        s = chunk[i:i+16]
        hex_part = ' '.join(f'{b:02x}' for b in s)
        ascii_part = ''.join(chr(b) if 32 <= b < 127 else '.' for b in s)
        lines.append(f"{prefix}{start+i:08x}  {hex_part:<48s}  |{ascii_part}|")
    return '\n'.join(lines)

# Кластеризация
clusters = []
cs, ce = RANGES[0]
for s, e in RANGES[1:]:
    if s - ce < 1024:
        ce = max(ce, e)
    else:
        clusters.append((cs, ce))
        cs, ce = s, e
clusters.append((cs, ce))

print(f"\nКластеров: {len(clusters)}")
for i, (s, e) in enumerate(clusters):
    print(f"  К{i+1}: 0x{s:08x}..0x{e:08x} ({e-s}b)")

# === Детальный дамп каждого кластера ===
print("\n" + "="*80)
print("ДЕТАЛЬНЫЙ ДАМП")
print("="*80)

for i, (cs, ce) in enumerate(clusters):
    ctx_start = max(0, cs - 64)
    ctx_end = min(len(A), ce + 96)
    
    print(f"\n{'='*80}")
    print(f"К{cs:08x}: 0x{cs:08x}..0x{ce:08x} (изм {ce-cs}b)")
    print(f"{'='*80}")
    print("A (0012):")
    print(hex_dump(A, ctx_start, ctx_end))
    print("B (0013):")
    print(hex_dump(B, ctx_start, ctx_end))
