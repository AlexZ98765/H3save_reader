# HoMM3 GM1 Toolkit — подробное описание файлов

> **Версия документа:** 3.0-dev (2026-10-10)
> **Проект:** Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD / HotA)
> **Цель:** Универсальный редактор сейвов, работающий с любой картой без хардкода абсолютных смещений

---

## 📁 Структура репозитория (актуальная)

```
H3save_reader/
├── README.md                          Точка входа (краткая навигация)
├── FILES_DESCRIPTION.md               Этот файл (подробное описание)
│
├── 01_tools/                          Готовые инструменты для работы с сейвами
│   ├── gm1_parser.py                  PySide6 GUI парсер сейвов (требует рефакторинга для использования MapConfig)
│   ├── map_json_loader.py             ⭐ Фаза 1: загрузчик JSON-парсинга карты (универсальный)
│   ├── map_config_builder.py          ⭐ Фаза 2: построитель per-map config из day-0 сейва
│   ├── save_parser.py                 ⭐ Фаза 3: универсальный парсер сейва через MapConfig
│   ├── cluster_finder.py              ⭐ Универсальный, gap-based поиск кластеров объектов в сейве
│   ├── header_parser.py               ⭐ Универсальный парсер заголовка .GM1
│   ├── save_layout.py                 ⭐ Dataclasses (HeaderInfo, ObjectCluster, HeroSection, TownSection, MapConfig) + формат-константы (HERO_FIELD_OFFSETS, TOWN_FIELD_OFFSETS)
│   ├── block_finder.py                Старый универсальный поиск hero/town блоков (regex + имена)
│   ├── gm1_diff.py                    CLI компаратор сейвов
│   └── gm1_diff_gui.py                GUI версия компаратора
│
├── 02_format_docs/                    Документация по формату .GM1
│   ├── GM1_format_compendium.md       Человекочитаемая справка по формату
│   ├── gm1_mapping.json               ⚠️ Конфиг смещений (требует чистки: удалить blocks и alternative_offsets)
│   ├── header_pointer_search.md       Стратегия универсального поиска секций
│   └── diff_interpretation.json       Сводка 32 дифф-анализов (историческая)
│
├── 03_object_mapping/                 Универсальные словари типов объектов
│   ├── README.md                      Описание
│   └── object_types_dictionary.json   ⭐ 2037 типов из LazyLlama wiki (универсальный)
│
├── 04_diff_analysis/                  Исторические скрипты дифф-анализа (разовые)
│   ├── README.md                      Структура папки
│   ├── build_object_type_dictionary.py ⭐ Универсальный — парсит LazyLlama wiki
│   └── Myth and Legend/               Подпапка для карты Myth and Legend
│       ├── README.md                  Описание скриптов
│       ├── analyze_*.py (25 скриптов) Анализ пар сейвов
│       ├── build_*.py (3 скрипта)     Построение маппингов
│       ├── find_header_pointers.py    Поиск указателей в заголовке
│       ├── verify_coord_mapping.py    Верификация против 312.GM1
│       ├── regenerate_mapping.py      Генератор gm1_mapping.json (decimal→hex)
│       ├── test_parser.py             Тест парсера без GUI
│       ├── analyze_save_clusters.py   Кластеризация смещений
│       ├── header_pointer_search_report.json
│       └── *.md / *.json отчёты      Отчёты дифф-анализов
│
├── 05_disasm/                         Результаты дизассемблирования heroes3.exe
│   ├── disasm_summary.json            Сводка PE-структуры
│   ├── all_strings.txt                Все 9680 ASCII-строк из PE
│   ├── interesting_strings.txt        Отфильтрованные строки (52)
│   ├── imports.txt                    Импорты PE (DLL + функции)
│   ├── save_io_funcs.json             Функции save/load
│   ├── save_callgraph.json            Call-graph 362 функций сериализации
│   ├── save_functions.json            18 функций сериализации
│   ├── save_functions_disasm.txt      Полный дизассемблер этих 18 функций
│   ├── magic_xrefs_disasm.txt         Xref-ы на H3SVG/H3SVC/AUTOSAVE
│   ├── rtti_strings.txt / rtti_xrefs.json  RTTI данные
│   └── func_*.asm (15 файлов)         Дизассемблированный код ключевых функций
│
├── 06_disasm_scripts/                 Скрипты, создавших 05_disasm/
│   ├── disasm_h3.py                   Главный дизассемблер (PE→JSON+TXT)
│   ├── disasm_save_io.py              Поиск функций save I/O
│   ├── disasm_serialize_content.py    Дизассемблирование SAVE_WRITER_CONTENT
│   ├── disasm_serialize_methods.py    Дизассемблирование методов сериализации
│   ├── find_main_serializer.py        Поиск главного сериализатора
│   ├── find_save_io.py                Поиск точек входа save/load
│   ├── find_source_paths.py           Поиск путей к исходникам в EXE
│   ├── find_vtables.py                Извлечение vtable-ов
│   ├── find_xrefs.py                  Поиск xref-ов к символам
│   ├── callgraph_analysis.py          Анализ call-graph
│   ├── analyze_main_func.py           Анализ главной функции игры
│   └── analyze_save_funcs.py          Анализ найденных save-функций
```

> **Примечание:** Папка `examples/` с примерами сейвов и парсингами карт (`.h3m.zip`, `0000.GM1`, `114.GM1`) больше не включается в архив. Пользователь должен предоставлять свои `.h3m.json` / `.GM1` файлы. См. README.md "С чего начать" для команды построения per-map config.

---

## 🆕 Главные файлы (текущее состояние)

### 1. `01_tools/save_layout.py` — Dataclasses ⭐⭐⭐

**Размер:** 290 строк
**Назначение:** Определяет дата-классы для архитектуры парсера и формат-константы.

**Содержимое:**
- `HeaderInfo` — распарсенный заголовок сейва (magic, version, map_name, header_size, ...)
- `ObjectCluster` — кластер объектов в сейве (name, start, end, hits, distinct_coords, peak_density, ...)
- `HeroBlockInfo`, `HeroSection` — найденные hero-блоки
- `TownBlockInfo`, `TownSection` — найденные town-блоки
- `ObjectOffsets` — смещения объекта в разных кластерах (main, visiting, fog, ...)
- `MapConfig` — полная per-map конфигурация (meta + clusters + hero_section + town_section + object_offsets + field_offsets)
- `ParsedField`, `ParsedBlock`, `ParsedSave` — для будущей Фазы 3

**Формат-константы (НЕ зависят от карты — проверены против h3sed/vlucas/cysun):**
- `HERO_FIELD_OFFSETS` — относительные смещения внутри hero-блока (Player=-169, CoordinatesX=-195, Experience=-130, HeroLevel=-120, Inventory=+365, ...)
- `TOWN_FIELD_OFFSETS` — то же для town-блока (faction=0, type=3, x=4, y=5, z=6, name_len=69, name=71, ...)
- `HERO_BLOCK_SIZE = 1122`, `HERO_STRIDE_SOD = 0x446 = 1094`
- `HERO_NAME_OFFSET_FROM_BLOCK_START = 169`, `TOWN_NAME_OFFSET_FROM_BLOCK_START = 71`

**Статус:** ✅ Готов.

---

### 2. `01_tools/header_parser.py` — Парсер заголовка ⭐⭐⭐

**Размер:** 165 строк
**Назначение:** Универсальный парсер заголовка `.GM1` сейва. Возвращает `HeaderInfo` с `header_size` — смещением, откуда начинать сканирование следующих секций.

**Возможности:**
- Парсит magic, version_major/minor, map_type, has_underground, map_size, is_playable
- Парсит map_name (cp1251) и description (cp1251)
- Находит map_filename (ASCII) и save_filename (ASCII) после них
- Вычисляет `header_size` — конец заголовка (round up до 4-байтной границы)

**Программное использование:**
```python
from header_parser import parse_header
header = parse_header(decompressed_save_bytes)
print(f"map_name: {header.map_name}, header_size: {header.header_size:#X}")
```

**Статус:** ✅ Готов. Проверен на обеих картах (save_parse_test_01 и Myth and Legend).

---

### 3. `01_tools/cluster_finder.py` — Поиск кластеров объектов ⭐⭐⭐

**Размер:** 470 строк
**Назначение:** Универсальный, density-based finder для per-object data секций в сейве.

**Алгоритм (gap-based с iterative masking):**
1. `find_coord_hits(raw, coord_ints, scan_start)` — находит все 3-байтные совпадения `coord_int = x | (y<<8) | (z<<16)` в сейве
2. `cluster_hits(...)` — группирует хиты в кластеры по адаптивному gap threshold (max(0x400, 8 × median_intra_gap))
3. `build_clusters(...)` — итеративно извлекает лучший кластер (по distinct_coords + peak_density), маскирует его хиты, повторяет
4. `assign_object_offsets(clusters, hits_by_ci)` — для каждого объекта находит его смещения в каждом кластере

**Качество кластеров:**
- Фильтр `min_distinct_ratio = 0.10` (10%) — отбрасывает шум-полы с большим hits, но низким distinct_coords
- Главный массив (main) — самый плотный кластер с ~100% покрытием (6006 distinct для Myth and Legend)
- visiting, decoration, alive, fog, treasure — следующие по качеству

**Программное использование:**
```python
from cluster_finder import find_all_object_clusters
clusters, object_offsets = find_all_object_clusters(
    raw, coord_ints, scan_start=header.header_size)
print(f"clusters: {len(clusters)}, object_offsets: {len(object_offsets)}")
for c in clusters:
    print(f"  {c.name}: {c.start_hex}..{c.end_hex}  hits={c.hits}  distinct={c.distinct_coords}")
```

**Статус:** ✅ Готов. Проверен на обеих картах:
- save_parse_test_01: 1 кластер (main, 3 hits, 3 distinct)
- Myth and Legend: 10 кластеров (main=7267 hits / 6006 distinct — 100% покрытие)

---

### 4. `01_tools/map_config_builder.py` — Построитель per-map config ⭐⭐⭐ (Фаза 2)

**Размер:** 460 строк
**Назначение:** Сравнивает `MapData` (из Фазы 1) с сейвом нулевого дня, находит динамические смещения всех секций сейва. Сохраняет результат в `map_config_<mapname>.json`.

**Возможности:**
- `parse_header(raw)` → HeaderInfo (делегирует в `header_parser`)
- `find_object_clusters(...)` (делегирует в `cluster_finder`)
- `find_hero_blocks(raw, scan_start, map_hero_names)` — поиск hero-блоков
  - Method 1: h3sed-style regex + name validation (принимает Latin/Cyrillic буквы + пробел/точка/апостроф — работает для русской локализации с cp1251 именами вроде "Одиссей", "Лорд Хаарт")
  - Method 2: name-based поиск (для English saves) — стандартные 156 имён + map-specific Prison герои
- `find_town_blocks(raw, scan_start, town_coords, town_names)` — поиск town-блоков
  - Method 1: coord-based (ищет 3-byte (x,y,z) needle, валидирует faction/type/name_len)
  - Method 2: name-based (ищет town_name как cp1251 строку)
- `build_map_config(map_data, day0_raw, day0_path)` → MapConfig
- `save_config(config, output_dir)` → JSON файл

**CLI:**
```bash
python3 01_tools/map_config_builder.py \
  --map-json /path/to/MyMap.h3m.zip \
  --day0-save /path/to/0000.GM1 \
  --output-dir /tmp/
```

**Программное использование:**
```python
from map_json_loader import load_map_json_safely
from map_config_builder import build_map_config, save_config, decompress_save

md, err = load_map_json_safely("/path/to/MyMap.h3m.zip")
day0 = decompress_save("/path/to/0000.GM1")
config = build_map_config(md, day0, "/path/to/0000.GM1")
save_config(config, "/tmp/")
```

**Статус:** ✅ Готов. Проверен на обеих картах:
- save_parse_test_01: 1 кластер, 1 town, 150 героев в пуле
- Myth and Legend: 10 кластеров, 21 town, 156 героев в пуле (стандарт SoD)

---

### 4. `01_tools/save_parser.py` — Универсальный парсер сейва ⭐⭐⭐ (Фаза 3)

**Размер:** 350 строк
**Назначение:** Парсит любой `.GM1` сейв через `MapConfig` (построенный в Фазе 2). Без хардкода абсолютных смещений.

**Возможности:**
- `parse_save(raw, config)` → `ParsedSave` — главная функция
- `parse_hero_block(raw, block_offset)` → словарь с полями героя (использует `HERO_FIELD_OFFSETS`):
  - `location_x/y/z`, `player` + `player_name`, `movement_total/left`, `experience`, `mana_left`, `level`, `num_skills`, `name` (cp1251)
  - `army_types[7]`, `army_counts[7]`, `skill_levels[28]`, `skill_slots[28]`
  - `attack`, `defense`, `power`, `knowledge` (атрибуты)
  - `spells_book[70]`, `spells_available[70]`
  - `equipment[19]` — 19 × (artifact_id, data)
- `parse_town_block(raw, block_offset)` → словарь с полями города (использует `TOWN_FIELD_OFFSETS`):
  - `faction` + `faction_name`, `type` + `type_name`, `x/y/z`, `army_types[7]`, `army_counts[7]`, `name`
- `adapt_config_to_save(raw, config)` → `(hero_section, town_section, object_offsets)` — **АДАПТАЦИЯ** между сейвами:
  - Пересоздаёт hero blocks, town blocks, object clusters для ТЕКУЩЕГО сейва (они могли сдвинуться из-за роста path-block/replay log)
  - Использует `find_hero_blocks`, `find_town_blocks` из `map_config_builder.py`
  - Использует `find_all_object_clusters` из `cluster_finder.py`
- `parsed_save_to_dict(parsed)` → JSON-serializable dict

**Программное использование (Фазы 1+2+3 — полный pipeline):**
```python
from map_json_loader import load_map_json_safely
from map_config_builder import build_map_config, decompress_save
from save_parser import parse_save, parsed_save_to_dict
import json

# Phase 1: load map JSON
md, err = load_map_json_safely("/path/to/MyMap.h3m.zip")

# Phase 2: build config from day-0 save
day0_raw = decompress_save("/path/to/0000.GM1")
config = build_map_config(md, day0_raw, "/path/to/0000.GM1")
config.save("/tmp/map_config_MyMap.json")

# Phase 3: parse ANY save of the same map
target_raw = decompress_save("/path/to/114.GM1")
parsed = parse_save(target_raw, config)

# Access parsed data
print(f"Map: {parsed.header.map_name}, save_filename: {parsed.header.save_filename}")
print(f"Heroes: {len(parsed.heroes)}")
for h in parsed.heroes[:3]:
    print(f"  {h['block_offset_hex']}  {h['fields']['name']!r}  "
          f"level={h['fields']['level']}  exp={h['fields']['experience']}")

# Serialize to JSON
data = parsed_save_to_dict(parsed)
json.dump(data, open("/tmp/parsed.json", "w"), ensure_ascii=False, indent=2)
```

**Адаптация между сейвами (проверено):**
- Myth and Legend: day-0 → 114.GM1 (день 1-1-4)
  - Hero blocks сдвинулись: `0x14223F` → `0x1418EB` (~0xA54 байт)
  - Hero state изменился: Одиссей level=1 exp=46 → level=2 exp=1629
  - Активных героев: 16 → 26 (игрок нанял 10 новых)
  - Town blocks сдвинулись: `0x140250` → `0x13F8FC`
- save_parse_test_01: day-0 → 0001.GM1 (после ходов)
  - Размер сейва идентичен (path block не растёт на малой карте)
  - Герой переместился: (5,3,0) → (5,1,0)

**Статус:** ✅ Готов. Проверен на обеих картах (day-0 + post-action сейвы).

---

### 5. `01_tools/map_json_loader.py` — Загрузчик JSON карты ⭐⭐⭐ (Фаза 1)

**Размер:** 398 строк
**Назначение:** Универсальный загрузчик JSON-парсинга `.h3m` карты. Строит `MapData` для ЛЮБОЙ карты.

**Статус:** ✅ Готов. Без изменений с v2.7.

---

### 6. `01_tools/gm1_parser.py` — PySide6 GUI парсер ⭐ (частично отрефакторен)

**Размер:** 2829 строк
**Назначение:** Главный GUI инструмент для просмотра и анализа `.GM1` сейвов.

**Возможности:**
- Открывает `.GM1` файлы (с обходом битого gzip CRC)
- Парсит сейв согласно `02_format_docs/gm1_mapping.json`
- Загрузка JSON карты (Ctrl+M) — динамическое построение objects_by_coord для любой карты
- Кнопка "Load Day-Zero Save…" (Ctrl+D) — строит `MapConfig` через `map_config_builder.build_map_config` и сохраняет в `map_config_<mapname>.json`
- Вкладка "Map Objects" с фильтрами и сортировкой
- Экспорт в JSON и Excel

**Рефакторинг в v3.0-dev (Шаг 2):**
- ✅ `_compute_object_offsets_in_save()` переписан — использует `cluster_finder.find_all_object_clusters` (без хардкода `0x118000`/`0x120000`/`0x170000`), `scan_start = header.header_size` (без хардкода `0x10000`)
- ✅ `find_hero_blocks(raw, scan_start=None)` — `scan_start` теперь по умолчанию берётся из `header_parser.parse_header(raw).header_size` (без хардкода `0x100000`)
- ✅ `find_town_blocks(raw, ..., scan_start=None)` — то же самое (без хардкода `30000`)
- ✅ `_show_map_object_details` — классификация смещений по кластерам из `MapConfig` (без хардкода `0x118C1C`/`0x120C8C`/`0x172100`)
- ⚠️ `parse_save()` (GUI функция) всё ещё читает `mapping["blocks"]` из `02_format_docs/gm1_mapping.json` (с absolute offsets для Myth and Legend) — это для отображения в дереве блоков. Для программного парсинга используйте `save_parser.parse_save(raw, config)`.

**Статус:** ⚠️ Частично отрефакторен. GUI `parse_save()` всё ещё зависит от `gm1_mapping.json`. Полный перевод GUI на `save_parser.parse_save()` — следующий шаг (Шаг 3).

---

### 7. `02_format_docs/gm1_mapping.json` — Машиночитаемый конф ⚠️

**Размер:** 1669 строк
**Назначение:** Карта всех известных смещений в `.GM1` сейвах.

**Содержимое:**
- `constants` — формат-константы (magic bytes, player colors, version codes) — ✅ универсальные
- `field_offsets` (заложено) — относительные смещения внутри блоков — ✅ универсальные (теперь дублированы в `save_layout.py`)
- `blocks` — массив из 20 блоков с ABSOLUTE смещениями — ❌ валидны только для "Myth and Legend.h3m"
- `alternative_offsets.save_447_series` — 14 полей для сейвов серии 447 — ❌ хардкод
- `path_block.path_record_types` — 7 типов path-записей — ✅ универсальные
- `known_unknowns` — список не-локализованных полей

**Статус:** ⚠️ Требует чистки: удалить секции `blocks` и `alternative_offsets`, оставить только `constants`, `path_block`, `known_unknowns`. Абсолютные смещения теперь вычисляются динамически через `map_config_builder.py`. Поле `field_offsets` дублировано в `save_layout.py` (более каноничное место).

---

### 8. `03_object_mapping/object_types_dictionary.json` — Словарь типов ⭐

**Размер:** 968 KB
**Назначение:** Редактируемый словарь всех 2037 типов объектов HoMM3, собранный с LazyLlama wiki.

**Структура:**
```jsonc
{
  "_meta": {
    "source": "https://heroes.thelazy.net/index.php/Map_Editor_Objects",
    "page_revision": 194767,
    "total_object_types": 2086
  },
  "_categories": { "Artifacts": [...], "Monsters": [...], ... },
  "_name_index": { "Mountain": ["avlmtd01.def", ...], ... },
  "_id_index":   { "5:136": "ava0137.def", ... },
  "objects": {
    "ava0001.def": {
      "wiki_name":    "Spell Scroll",
      "category":     "Artifacts",
      "description":  "This scroll contains a spell...",
      "user_edits":   { "user_notes": "", "user_category": "", "user_alias": "" }
    },
    ...
  }
}
```

**Статус:** ✅ Универсальный. Единственный файл в `03_object_mapping/`.

---

## 🔬 Дизассемблирование (05_disasm/)

Результаты анализа `heroes3.exe` (PE32, SoD build). Созданы скриптами из `06_disasm_scripts/`. Не зависят от конкретной карты.

### Файлы с метаданными
- `disasm_summary.json` — сводка PE-структуры
- `all_strings.txt` (9680 строк) — все ASCII-строки из PE
- `interesting_strings.txt` (52 строки) — отфильтрованные по сейвам
- `imports.txt` — все импорты PE

### Функции сериализации
- `save_io_funcs.json` — найденные функции save/load
- `save_callgraph.json` — call-graph 362 функций
- `save_functions.json` — описания 18 функций сериализации
- `save_functions_disasm.txt` — полный дизассемблер
- `magic_xrefs_disasm.txt` — xref-ы к H3SVG/H3SVC/AUTOSAVE

### 15 файлов `func_*.asm`
Дизассемблированный код ключевых функций:
- `func_SAVE_WRITER_0x000be0b0.asm` — главный save writer
- `func_SAVE_READER_0x000bca60.asm` — главный save reader
- `func_SAVE_WRITER_CONTENT_0x000bc290.asm` — виртуальный сериализатор
- `func_HEADER_WRITER_0x000bbda0.asm` — пишет H3SVG magic + версию
- и др.

---

## 📜 Скрипты дифференциального анализа (04_diff_analysis/)

**Важно:** Эти скрипты — **исторические разовые исследования**. Они использовались для ручного локализования полей формата и не вызываются в runtime. Результаты собраны в `02_format_docs/diff_interpretation.json` (32 дифф-анализа).

Подробнее см. `04_diff_analysis/README.md` и `04_diff_analysis/Myth and Legend/README.md`.

---

## ⚠️ Известные проблемы (work in progress)

### `gm1_parser.py` — хардкод смещений

**Проблема:** Прямые смещения `0x157F1E`, `0x120C8C`, `0x118C1C`, `0x13F4C3`, `0x172129` в коде. Действительны только для сейвов "Myth and Legend.h3m".

**План:** Заменить на чтение из `map_config_<mapname>.json` (построенного `map_config_builder.py`). Это следующий шаг рефакторинга.

### `gm1_mapping.json` — абсолютные смещения

**Проблема:** Секции `blocks` и `alternative_offsets` содержат absolute смещения для одной карты.

**План:** Удалить эти секции. Оставить только `constants`, `path_block`, `known_unknowns`. Поле `field_offsets` уже дублировано в `save_layout.py` (более каноничное место).

---

## 📊 Статистика

| Метрика | Значение |
|---------|----------|
| Дифференциальных анализов выполнено (исторически) | 32 |
| Полей локализовано | 124+ |
| Блоков описано | 20 |
| Типов path-записей | 7 |
| Функций сериализации найдено в EXE | 18 |
| Строк в дизассемблере | 9680 |
| Скриптов анализа (в `04_diff_analysis/Myth and Legend/`) | 39 |
| Типов объектов в словаре wiki | 2037 (универсальный) |
| Примеры сейвов | (не входят в архив; пользователь загружает свои) |

---

## 🎯 Рекомендуемый порядок использования

### Сценарий 1: Распарсить сейв новой карты

1. Получить `.h3m.json` (парсинг карты через твой внешний парсер)
2. Запустить `map_config_builder.py` на сейве нулевого дня:
   ```bash
   python3 01_tools/map_config_builder.py \
     --map-json /path/to/MyMap.h3m.zip \
     --day0-save /path/to/0000.GM1 \
     --output-dir /tmp/
   ```
3. Получить `map_config_MyMap.json` — конфиг с динамическими смещениями
4. Использовать конфиг в `gm1_parser.py` для парсинга любого сейва той же карты

### Сценарий 2: Программное использование (Фазы 1+2; Фаза 3 — planned)

```python
# Phase 1: load map JSON
from map_json_loader import load_map_json_safely
md, err = load_map_json_safely("/path/to/MyMap.h3m.zip")

# Phase 2: build config from day-0 save
from map_config_builder import build_map_config, decompress_save
day0_raw = decompress_save("/path/to/0000.GM1")
config = build_map_config(md, day0_raw, "/path/to/0000.GM1")
# config.save("/tmp/map_config_MyMap.json")

# Phase 3 (planned): parse any save
# from save_parser import parse_save
# target_raw = decompress_save("/path/to/114.GM1")
# parsed = parse_save(target_raw, config, md)
```

---

## 📝 Журнал изменений

### v3.0-dev — Шаг 2 (2026-10-10)
- **Новый модуль** `01_tools/save_parser.py` (350 строк) — **Фаза 3**: универсальный парсер сейва через `MapConfig`. Главные функции:
  - `parse_save(raw, config)` → `ParsedSave` (header + clusters + heroes + towns + object_offsets)
  - `parse_hero_block(raw, block_offset)` — читает все поля героя (location, level, exp, army, skills, attributes, spells, equipment)
  - `parse_town_block(raw, block_offset)` — читает все поля города (faction, type, coords, army, name)
  - `adapt_config_to_save(raw, config)` — **АДАПТАЦИЯ** между сейвами: пересоздаёт hero/town blocks и object clusters для текущего сейва (они могут сдвигаться из-за роста path-block)
  - `parsed_save_to_dict(parsed)` — JSON-сериализация
- **Рефакторинг** `01_tools/gm1_parser.py`:
  - ✅ `_compute_object_offsets_in_save()` переписан — использует `cluster_finder.find_all_object_clusters` (без хардкода `0x118000`/`0x120000`/`0x170000`/`0x10000`)
  - ✅ `find_hero_blocks(raw, scan_start=None)` — scan_start из `header_parser.parse_header(raw).header_size` (без хардкода `0x100000`)
  - ✅ `find_town_blocks(raw, ..., scan_start=None)` — то же (без хардкода `30000`)
  - ✅ `_show_map_object_details` — классификация смещений по кластерам из `MapConfig` (без хардкода `0x118C1C`/`0x120C8C`/`0x172100`)
  - ⚠️ `parse_save()` (GUI) всё ещё читает блоки из `gm1_mapping.json` — перевод на `save_parser.parse_save()` следующий шаг
- **Проверено** на обеих картах: day-0 + post-action сейвы. Адаптация между сейвами работает (hero blocks сдвигаются, hero state меняется, town records сдвигаются — всё корректно находится заново)
- **Обновлены** `README.md`, `FILES_DESCRIPTION.md`

### v3.0-dev — Шаг 0+1 (2026-10-10)
- **Новые модули** в `01_tools/`:
  - `save_layout.py` — dataclasses (`HeaderInfo`, `ObjectCluster`, `HeroSection`, `TownSection`, `MapConfig`) + формат-константы (`HERO_FIELD_OFFSETS`, `TOWN_FIELD_OFFSETS`)
  - `header_parser.py` — парсер заголовка `.GM1` (вычисляет `header_size`)
  - `cluster_finder.py` — универсальный gap-based кластеризатор объектов в сейве (итеративный с masking)
  - `map_config_builder.py` — переписан полностью: использует новые модули, убран хардкод `0x100000` (теперь scan от `header_size`), добавлена поддержка русских hero-имён (cp1251)
- **Реорганизация** `04_diff_analysis/`: 39 файлов перемещены в `04_diff_analysis/Myth and Legend/` (подпапка по карте); на верхнем уровне остался только универсальный `build_object_type_dictionary.py` + новый `README.md`
- **Очистка репозитория:**
  - Удалены `_obsolete/` (устаревшие файлы)
  - Удалены все предрассчитанные JSON для "Myth and Legend.h3m" из `03_object_mapping/` (остался только универсальный `object_types_dictionary.json`)
  - Удалён `01_tools/Games - Ярлык.lnk` (junk Windows shortcut)
  - Удалён дубликат `01_tools/gm1_mapping.json` (остался каноничный в `02_format_docs/`)
  - Удалён `map_config_Мифы_и_легенды.json` (старый артефакт со сломанным town finder)
- **Обновлены** `README.md`, `FILES_DESCRIPTION.md`, `03_object_mapping/README.md` под реальное состояние репозитория

### v2.7 (2026-10-08)
- Добавлен `01_tools/map_json_loader.py` (Фаза 1)
- Кнопка "Load Map JSON…" в `gm1_parser.py`

### v2.0–v2.6 (2026-10-07/08)
- 32 дифф-анализа, 124+ полей локализовано, 7009 объектов карты замапплено, 100% верификация против сейва
- Перевод смещений в hex-формат, секция `alternative_offsets.save_447_series`

### v1.0 (2026-10-07)
- Первая версия: 19 дифф-анализов, 104 поля
