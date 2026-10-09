# HoMM3 GM1 Toolkit

> Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD/HotA)
> **Версия архива:** 2.7 (2026-10-08)

## Что внутри

Архив организован по 8 смысловым категориям:

| Папка | Что содержит | Главный файл(ы) |
|-------|--------------|------------------|
| **`01_tools/`** | Готовые инструменты для работы с сейвами | `gm1_parser.py` (v2.7), `map_json_loader.py` ⭐, `gm1_diff.py`, `gm1_diff_gui.py` |
| **`02_format_docs/`** | Документация по формату `.GM1` | `GM1_format_compendium.md`, `gm1_mapping.json`, `diff_interpretation.json` (32 диффа) |
| **`03_object_mapping/`** | Маппинг 7009 объектов + словарь типов + адреса в сейве | `objects_by_coord.json`, `objects_flat.json`, `objects_with_offsets.json`, `object_types_dictionary.json` |
| **`04_diff_analysis/`** | 36 скриптов дифференциального анализа сейвов | `analyze_*.py`, `build_object_*.py`, `build_objects_with_offsets.py` |
| **`05_disasm/`** | Дизассемблированный `heroes3.exe` | `func_*.asm`, `save_functions_disasm.txt` |
| **`06_disasm_scripts/`** | 12 скриптов, создавших `05_disasm/` | `disasm_h3.py`, `find_*.py` |
| **`examples/`** | Примеры: JSON + Excel со всеми объектами и адресами | `parsed_save_312_with_objects.json` (10.3 MB), `homm3_map_objects.xlsx` (1.4 MB) |
| **`_obsolete/`** | Устаревшие файлы (с объяснением) | см. `_obsolete/README.md` |

## С чего начать

1. **Прочитать** `FILES_DESCRIPTION.md` — подробное описание всех файлов и сценариев использования.
2. **Запустить парсер сейвов**: `python3 01_tools/gm1_parser.py`
3. **Загрузить JSON карты** (кнопка "Load Map JSON…" или Ctrl+M) — это делает парсер универсальным для сейвов любой карты
4. **Открыть .GM1 сейв** — File → Open
5. **Изучить дерево** (слева) — блоки сейва + Heroes + Towns + **Map Objects** (все объекты с координатами и адресами)
6. **Перейти на вкладку "Map Objects"** — таблица всех объектов с фильтром и сортировкой
7. **Экспорт в Excel** — File → Export Objects to Excel… (Ctrl+X)

## Универсальность: работа с любой картой ⭐ НОВОЕ в v2.7

Парсер больше не привязан к карте "Myth and Legend.h3m"! Кнопка **"Load Map JSON…"** (Ctrl+M) позволяет загрузить JSON-парсинг любой карты:

```bash
# В GUI:
python3 01_tools/gm1_parser.py
# Нажать кнопку "Load Map JSON…" (или File → Load Map JSON… / Ctrl+M)
# Выбрать .json или .zip файл с парсингом карты
```

После загрузки:
- Парсер динамически строит `objects_by_coord`, `coord_int_lookup`, `type_index`, `category_index` из JSON карты
- Дерево объектов и таблица "Map Objects" перестраиваются
- Парсер работает с сейвами **любой** карты (а не только "Myth and Legend.h3m")

**Программное использование:**
```python
from map_json_loader import load_map_json_safely

md, err = load_map_json_safely("/path/to/AnyMap.h3m.json")
if err:
    print(f"Error: {err}")
else:
    print(f"Map: {md.map_name} ({md.map_size}×{md.map_size})")
    print(f"Objects: {md.n_objects}, Towns: {md.n_towns}, Heroes: {md.n_heroes}")
    # md.objects_by_coord  — {"x:y:z": {type, category, details, ...}}
    # md.coord_int_lookup  — {coord_int: "x:y:z"}
    # md.type_index        — {"Tower": ["111:3:0", ...]}
    # md.category_index    — {"town": ["111:3:0", ...]}
```

JSON карты может быть как отдельным `.json` файлом, так и внутри `.zip` архива.

## Главные достижения v2.6

- **124+ полей** локализовано в 20 блоках сейва
- **32 дифференциальных анализа**
- **7009 объектов карты** замапплено (100% верификация против сейва)
- **1725 объектов имеют подтверждённые адреса в сейве** (main_offset + visiting_offset + др.)
- **2037 типов объектов** из LazyLlama wiki
- **18 функций сериализации** найдено в `heroes3.exe`

### Новое в v2.6 ⭐

**1. Адреса объектов в JSON-экспорте** — каждый объект теперь содержит:
- `main_offset` — смещение в главном массиве состояний объектов (`0x120C8C..`)
- `visiting_offset` — смещение в visiting-objects array
- `fog_offset` — смещение в fog-of-war array
- `alive_offset`, `treasure_offset`, `decoration_offset` — другие кластеры
- `save_offsets` — полный список всех смещений в сейве
- `verified` — был ли объект верифицирован (true для 1751 недекоративных объектов)

**2. Таблица "Map Objects" в GUI** — новая вкладка с таблицей всех 7009 объектов:
- Колонки: X, Y, Z, coord_int, is_primary, type, category, sprite_def, **main_offset**, visiting_offset, fog_offset, decoration_offset, all_save_offsets
- **Фильтр по тексту** (type, category, sprite_def, coord_key)
- **Фильтр по категории** (dropdown с 21 категорией)
- **Чекбокс "Verified only"** — только объекты с подтверждёнными адресами
- **Сортировка** по любой колонке (клик по заголовку)
- **Двойной клик по строке** → детали объекта + hex-дамп вокруг `main_offset`

**3. Адреса в Excel-экспорте** — лист "All Objects" теперь имеет **26 колонок** (включая 7 колонок с адресами):
- main_offset, visiting_offset, fog_offset, alive_offset, treasure_offset, decoration_offset, all_save_offsets, verified

**Пример записи (Town Кавала в 111:3:0):**
```jsonc
{
  "coord_int": 879,
  "type": "Tower",
  "category": "town",
  "main_offset": "0x120C8C",          // ⭐ адрес в сейве
  "save_offsets": ["0x1FD60", "0x120C8C"],
  "verified": true,
  "details": { "town_name": "Кавала", ... }
}
```

### Новое в v2.5 — экспорт всех объектов в Excel

В парсер `gm1_parser.py` добавлена кнопка **"Export Objects to Excel…"** (горячая клавиша `Ctrl+X`).

Создаёт `.xlsx` файл с 4 листами: All Objects (7010×26), By Category (7010×12), Summary (236×3), Wiki Dictionary (2038×12).

**Требования:** `pip install openpyxl`

### Новое в v2.4 (дифф 447_2 → 447_3 — построена Magic Guild Lv3)

Локализованы 7 новых полей:
- `mage_guild_level` @ `0x13F74D` (1 byte, 2→3) — **главная находка: уровень Magic Guild!** ⭐
- `town_build_flag` @ `0x13F70B` (1 byte, 0→1) — флаг постройки
- `building_bitmask` @ `0x13F7AB` (4 bytes) — building bitmask
- `town_bitmask_byte` @ `0x13F7B6` (1 byte) — secondary bitmask
- `player_resources[7]` @ `0x13E3E7` (28 bytes) — 7 × 4-byte LE: Wood, Mercury, Ore, Sulfur, Crystal, Gems, Gold
- `town_spell_slots` @ `0x13E380+` — spell pool (rearranged on guild upgrade)
- `hero_spell_book_bitmask` @ `0x140246` — per-spell "cast this turn" flags

### Новое в v2.3 (дифф 447_1 → 447_2 — army swap + artifact equip)

Локализованы 7 новых полей (см. FILES_DESCRIPTION.md для деталей):
- `hero_army_types[5]` @ `0x143E52`, `hero_army_counts[5]` @ `0x143E6E`
- `hero_backpack_count` @ `0x143DDD`, `hero_doll_slot_N` @ `0x143FA7`
- `hero_backpack_slots` @ `0x14400F` (cascade shift on remove)
- `day_or_step_counter` @ `0x000003B7`, `hero_alt_flag_per_hero` @ `0x1789AB + 0x1E0*N`

## Карта "Myth and Legend.h3m"

Все сейвы серии 312, xx, 447 — с одной карты. Hero block start различается между сейвами:
- Серия 312/xx: hero block start ≈ `0x157F1E` (faction byte)
- Серия 447: hero block start ≈ `0x143DC0`

**Важное ограничение:** абсолютные смещения валидны только для конкретного сейва. Серия `alternative_offsets` в `gm1_mapping.json` документирует смещения для разных сейвов (включая `save_447_series` с 14 полями).

## Координатная кодировка

В сейве координаты объектов хранятся как 3 байта:

```text
coord_int = x | (y << 8) | (z << 16)
```

где `z = 0` — surface, `z = 1` — underground. Для быстрого lookup-а типа объекта по coord_int используйте `03_object_mapping/coord_int_lookup.json`.

## Адреса объектов в сейве

Для каждого из 1725 верифицированных объектов (недекоративных) вычислены адреса в декомпрессированном сейве `312.GM1`:

| Поле | Описание |
|------|----------|
| `main_offset` | Смещение в главном массиве состояний объектов (`0x120C8C..0x180000`) — здесь хранятся owner, visited-flag, garrison |
| `visiting_offset` | Смещение в visiting-objects array (`0x118C1C..`) — какой герой сейчас у объекта |
| `fog_offset` | Смещение в fog-of-war / discovered-objects array |
| `alive_offset` | Смещение в alive-objects overlay |
| `treasure_offset` | Смещение в treasure/visit_other cluster |
| `decoration_offset` | Смещение в decoration-shadow bitmask |
| `save_offsets` | Полный список всех смещений (для отладки) |

Источник: `03_object_mapping/objects_with_offsets.json` (2.0 MB, 6006 записей, из них 1725 verified).

## Два варианта представления объектов

- **`objects_by_coord.json`** (1.8 MB) — словарь `"x:y:z" → {primary, overlays[]}`. **6006 записей**.
- **`objects_flat.json`** (2.1 MB) — плоский список всех **7009 объектов** с полем `is_primary`.

В JSON-экспорте парсера (v2.2+) присутствуют **оба варианта** в `objects_on_map` секции.

## Словарь типов объектов (редактируемый)

`03_object_mapping/object_types_dictionary.json` — словарь всех 2037 типов объектов HoMM3, собранный с LazyLlama wiki.

- **Первичный ключ** — `.def` file name (уникальный)
- **`user_edits`** — поле для правок (notes, custom_category, alias), **сохраняется при пересоздании словаря**
- **3 индекса** для поиска: `_name_index`, `_id_index`, `_categories`

Для обновления словаря:
```bash
curl -o /tmp/lazymap.html https://heroes.thelazy.net/index.php/Map_Editor_Objects
python3 04_diff_analysis/build_object_type_dictionary.py
```

## Подробная документация

См. **`FILES_DESCRIPTION.md`** — полное описание всех файлов с указанием размеров, назначений, форматов и сценариев использования.
