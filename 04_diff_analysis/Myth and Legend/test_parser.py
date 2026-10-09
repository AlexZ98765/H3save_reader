#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Тест парсера без GUI — импортирует логику из gm1_parser.py и проверяет
на реальных сейвах.
"""
import sys
import os
import json

# Добавляем директорию с парсером
sys.path.insert(0, '/home/z/my-project/download')

# Импортируем логику (без GUI части)
import importlib.util
spec = importlib.util.spec_from_file_location("gm1_parser", "/home/z/my-project/download/gm1_parser.py")

# Чтобы избежать импорта PySide6 при тестировании логики, подменяем его
import types
for mod_name in ['PySide6', 'PySide6.QtCore', 'PySide6.QtGui', 'PySide6.QtWidgets']:
    m = types.ModuleType(mod_name)
    sys.modules[mod_name] = m
    for attr in ['Qt', 'QFont', 'QAction', 'QColor', 'QPalette', 'QFileInfo',
                 'QApplication', 'QMainWindow', 'QWidget', 'QVBoxLayout', 'QHBoxLayout',
                 'QPushButton', 'QLineEdit', 'QLabel', 'QPlainTextEdit', 'QComboBox',
                 'QSpinBox', 'QCheckBox', 'QFileDialog', 'QGroupBox', 'QFormLayout',
                 'QMessageBox', 'QSplitter', 'QTabWidget', 'QTreeWidget', 'QTreeWidgetItem',
                 'QTextEdit', 'QStatusBar', 'QMenuBar', 'QMenu']:
        setattr(m, attr, type(attr, (), {}))

mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

# Теперь у нас есть функции
load_gm1_file = mod.load_gm1_file
parse_save = mod.parse_save
export_to_json = mod.export_to_json
hex_dump = mod.hex_dump

# Загружаем mapping
with open('/home/z/my-project/download/gm1_mapping.json', 'r', encoding='utf-8') as f:
    mapping = json.load(f)

print(f"[*] Mapping loaded: {len(mapping.get('blocks', []))} blocks")
print(f"[*] Constants: {list(mapping.get('constants', {}).keys())}")

# Тестируем на нескольких сейвах
test_files = [
    '/home/z/my-project/upload/001.GM1',
    '/home/z/my-project/upload/0020_9.GM1',
]

for path in test_files:
    if not os.path.exists(path):
        print(f"\n[!] File not found: {path}")
        continue

    print(f"\n{'='*80}")
    print(f"Testing: {path}")
    print(f"{'='*80}")

    # Load
    raw = load_gm1_file(path)
    print(f"[*] Raw size: {len(raw)} bytes")
    print(f"[*] Magic: {raw[:5]}")

    # Parse
    parsed = parse_save(raw, mapping)

    print(f"\n[*] Parsed blocks: {len(parsed['blocks'])}")
    print(f"[*] Path records: {len(parsed['path_records'])}")
    print(f"[*] Hero blocks found: {len(parsed['hero_blocks_found'])}")
    print(f"[*] Errors: {len(parsed['errors'])}")

    if parsed['errors']:
        print(f"\n[!] Errors:")
        for e in parsed['errors'][:5]:
            print(f"    {e}")

    # Print summary of each block
    print(f"\n[*] Block summary:")
    for block in parsed['blocks']:
        print(f"\n  ─── {block['name']} ───")
        print(f"      {block['description']}")
        for f in block['fields'][:5]:
            offset_str = f"0x{f['offset']:08x}" if isinstance(f['offset'], int) and f['offset'] >= 0 else str(f['offset'])
            print(f"      • {f['name']:30s} = {f['formatted'][:50]:50s}  @ {offset_str}")
        if len(block['fields']) > 5:
            print(f"      ... and {len(block['fields']) - 5} more fields")

    # Test JSON export
    print(f"\n[*] Testing JSON export…")
    json_str = export_to_json(parsed, raw)
    print(f"    JSON size: {len(json_str)} bytes")

    # Save JSON for inspection
    out_path = path.replace('.GM1', '_parsed.json').replace('.gm1', '_parsed.json')
    with open(out_path, 'w', encoding='utf-8') as f:
        f.write(json_str)
    print(f"    Saved to: {out_path}")

print("\n✅ All tests completed!")
