# HoMM3 GM1 Toolkit — подробное описание файлов

> **Версия документа:** 2.7 (2026-10-08)
> **Проект:** Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD / HotA)
> **Цель:** Создать полноценный редактор сейвов (аналог редактора карт с возможностью модификации всех полей)

---

## 📁 Структура архива `homm3_gm1_toolkit.zip`

Архив организован по смыслу — каждая папка содержит файлы одной логической категории. Внутри архива:

```
homm3_gm1_toolkit/
├── README.md                          Точка входа (краткая навигация)
├── FILES_DESCRIPTION.md               Этот файл (подробное описание)
│
├── 01_tools/                          Готовые инструменты для работы с сейвами
│   ├── gm1_parser.py                  PySide6 GUI парсер сейвов ⭐⭐⭐ (включает объекты карты в экспорт)
│   ├── map_json_loader.py             ⭐ НОВОЕ: загрузчик JSON-парсинга карты (делает парсер универсальным)
│   ├── gm1_diff.py                    CLI компаратор сейвов
│   └── gm1_diff_gui.py                GUI версия компаратора (PySide6)
│
├── 02_format_docs/                    Документация по формату .GM1
│   ├── GM1_format_compendium.md       Человекочитаемая справка ⭐⭐
│   ├── gm1_mapping.json               Машиночитаемый конф смещений ⭐⭐⭐
│   └── diff_interpretation.json       Сводка всех 30 дифф-анализов
│
├── 03_object_mapping/                 Маппинг 7009 объектов карты → координаты ⭐
│   ├── README.md                      Описание маппинга
│   ├── objects_by_coord.json          (1.8 MB) "x:y:z" → полная запись объекта (primary + overlays[])
│   ├── objects_flat.json              (2.1 MB) плоский список всех 7009 объектов (без вложений)
│   ├── objects_with_offsets.json      (2.0 MB) ⭐ НОВОЕ: адреса каждого объекта в сейве (main_offset, visiting_offset, fog_offset и др.)
│   ├── coord_int_lookup.json          coord_int → "x:y:z" (для парсера сейвов)
│   ├── type_index.json                тип → [список координат]
│   ├── category_index.json            категория → [список координат]
│   ├── object_mapping_summary.json    Сводка статистики
│   ├── coord_mapping_verification.json (556 KB) верификация против 312.GM1
│   ├── save_offset_clusters.json      Кластеры смещений в сейве
│   ├── save_offset_clusters.md        Человекочитаемая карта кластеров
│   ├── object_types_dictionary.json   ⭐ 2037 типов объектов из LazyLlama wiki (редактируемый)
│   └── map_types_xref.json            ⭐ кросс-референс типов карты с wiki
│
├── 04_diff_analysis/                  Скрипты дифференциального анализа
│   ├── analyze_diff.py                Общий фреймворк (первый анализ)
│   ├── analyze_path.py                Path-записи (типы 01 03, 0b 03)
│   ├── analyze_path_dest.py           0017→0018: queued_path_destination
│   ├── analyze_build_diff.py          0011→0012: постройка здания
│   ├── analyze_xp_diff.py             0012→0013: опыт + уровень
│   ├── analyze_xp_focus.py            Детализация XP/level
│   ├── analyze_artifact_diff.py       0013→0014: покупка артефакта
│   ├── analyze_hire_diff.py           0014→0015: найм существ
│   ├── analyze_transfer_diff.py       0015→0016: передача армии
│   ├── analyze_spellbook_diff.py      0016→0017: книга магии
│   ├── analyze_ballista_diff.py       0018→0019: покупка баллисты
│   ├── analyze_stables_diff.py        0019→0020: конюшня
│   ├── analyze_cheat_diff.py          0020→0020_1: чит-код
│   ├── analyze_cheat2_diff.py         0020_5→0020_6: чит-коды
│   ├── analyze_mine_diff.py           0020_1→0020_2: захват шахты
│   ├── analyze_fountain_diff.py       0020_2→0020_3: фонтан удачи
│   ├── analyze_upgrade_diff.py        0020_3→0020_4: форт улучшений
│   ├── analyze_disembark_diff.py      0020_4→0020_5: высадка из лодки
│   ├── analyze_battle_diff.py         0020_6→0020_7: битва за город
│   ├── analyze_battle2_diff.py        0020_8→0020_9: победа над врагом
│   ├── analyze_moves_diff.py          0020_7→0020_8: движение 2 героев
│   ├── analyze_mercenary_diff.py      Доп. анализ наёмника
│   ├── analyze_447_diff.py            ⭐ 447_1→447_2: army swap + artifact equip
│   ├── analyze_447_diff_report.json   ⭐ машинно-читаемый отчёт 447_1→447_2
│   ├── analyze_447_diff_report.md     ⭐ человекочитаемый отчёт 447_1→447_2
│   ├── analyze_447_2_to_3_diff.py     ⭐ НОВОЕ: 447_2→447_3: построена Magic Guild Lv3
│   ├── analyze_447_2_to_3_diff_report.json ⭐ НОВОЕ: машинно-читаемый отчёт
│   ├── analyze_447_2_to_3_diff_report.md  ⭐ НОВОЕ: человекочитаемый отчёт
│   ├── build_object_mapping.py        Построение маппинга из .h3m.json
│   ├── build_object_type_dictionary.py ⭐ построение словаря типов из LazyLlama wiki
│   ├── build_objects_flat.py          ⭐ построение плоского списка 7009 объектов из objects_by_coord.json
│   ├── build_objects_with_offsets.py  ⭐ НОВОЕ: вычисление адресов объектов в сейве (main_offset, visiting_offset и др.)
│   ├── verify_coord_mapping.py        Верификация маппинга против .GM1
│   ├── analyze_save_clusters.py       Кластеризация смещений в сейве
│   ├── regenerate_mapping.py          Генератор gm1_mapping.json
│   └── test_parser.py                 Тест парсера без GUI
│
├── 05_disasm/                         Результаты дизассемблирования heroes3.exe
│   ├── disasm_summary.json            Сводка PE-структуры
│   ├── all_strings.txt                Все 9680 ASCII-строк из PE
│   ├── interesting_strings.txt        Отфильтрованные строки по сейвам
│   ├── imports.txt                    Импорты PE (DLL + функции)
│   ├── save_io_funcs.json             Функции save/load (SAVE_WRITER, SAVE_READER, ...)
│   ├── save_callgraph.json            Call-graph 362 функций сериализации
│   ├── save_functions.json            Описания 18 функций сериализации
│   ├── save_functions_disasm.txt      Полный дизассемблер функций
│   ├── magic_xrefs_disasm.txt         Xref'ы на H3SVG/H3SVC/AUTOSAVE
│   ├── rtti_strings.txt               RTTI-строки (минимальные)
│   ├── rtti_xrefs.json                RTTI xref'ы
│   └── func_*.asm                     15 файлов дизассемблированного кода
│
├── 06_disasm_scripts/                 Скрипты, которые создали 05_disasm/
│   ├── disasm_h3.py                   Главный дизассемблер (PE→JSON+TXT)
│   ├── disasm_save_io.py              Поиск функций save I/O в EXE
│   ├── disasm_serialize_content.py    Дизассемблирование SAVE_WRITER_CONTENT
│   ├── disasm_serialize_methods.py    Дизассемблирование методов сериализации
│   ├── find_main_serializer.py        Поиск главного сериализатора
│   ├── find_save_io.py                Поиск точек входа в save/load
│   ├── find_source_paths.py           Поиск путей к исходникам в EXE
│   ├── find_vtables.py                Извлечение vtable-ов
│   ├── find_xrefs.py                  Поиск xref'ов к символам
│   ├── callgraph_analysis.py          Анализ call-graph
│   ├── analyze_main_func.py           Анализ главной функции
│   └── analyze_save_funcs.py          Анализ найденных save-функций
│
├── examples/                          ⭐ НОВОЕ: примеры использования
│   ├── parsed_save_312_with_objects.json  (6.9 MB) экспорт сейва 312.GM1 с objects_on_map + словарём типов
│   └── homm3_map_objects.xlsx         ⭐ НОВОЕ: Excel-экспорт всех 7009 объектов карты (4 листа)
│
└── _obsolete/                         Устаревшие файлы (см. ниже)
    ├── README.md                      Почему каждый файл устарел
    ├── old_root/
    │   ├── README_old.md              Старый README (пустой)
    │   └── gm1_essential.zip          Старый частичный архив
    ├── old_scripts/
    │   └── gm1_diff_old.py            Старая версия gm1_diff.py (35 KB)
    └── old_disasm/
        ├── diff_interpretation_old.json   Старая версия (8 KB, 19 диффов)
        └── gm1_mapping_old.json           Старая версия mapping (до hex-формата)
```

---

## 🆕 Основные файлы (главные инструменты)

### 1. `01_tools/gm1_parser.py` — PySide6 GUI парсер сейвов ⭐⭐⭐

**Размер:** 75 KB
**Назначение:** Главный инструмент для просмотра и анализа `.GM1` сейвов.

**Возможности:**
- Открывает `.GM1` файлы (с обходом битого gzip CRC, специфичного для HoMM3)
- Парсит сейв согласно `02_format_docs/gm1_mapping.json`
- Показывает дерево всех блоков и полей с их значениями
- При клике на поле показывает детали (имя, значение, тип, смещение, размер) и hex-дамп с контекстом (±16 байт)
- **Экспортирует в JSON** (v2.1+) с тремя секциями: `objects_on_map`, `object_types_dictionary`, `coord_int_lookup`
- **⭐ НОВОЕ в v2.6: Адреса объектов в JSON** — каждый объект теперь содержит:
  - `main_offset` — смещение в главном массиве состояний объектов (`0x120C8C..`)
  - `visiting_offset` — смещение в visiting-objects array
  - `fog_offset` — смещение в fog-of-war array
  - `alive_offset` — смещение в alive-objects overlay
  - `treasure_offset` — смещение в treasure/visit_other cluster
  - `decoration_offset` — смещение в decoration-shadow bitmask
  - `save_offsets` — полный список всех смещений в сейве
  - `verified` — был ли объект верифицирован через coord_bytes (true для 1751 недекоративных объектов)
- **⭐ НОВОЕ в v2.6: Таблица "Map Objects" в GUI** — новая вкладка с таблицей всех 7009 объектов:
  - Колонки: X, Y, Z, coord_int, is_primary, type, category, sprite_def, main_offset, visiting_offset, fog_offset, decoration_offset, all_save_offsets
  - **Фильтр по тексту** (type, category, sprite_def, coord_key)
  - **Фильтр по категории** (dropdown с 21 категорией)
  - **Чекбокс "Verified only"** — показывать только объекты с подтверждёнными адресами
  - **Сортировка** по любой колонке (клик по заголовку)
  - **Двойной клик по строке** → открывает детали объекта + hex-дамп вокруг `main_offset`
- **Экспорт в Excel** (v2.5+) — кнопка "Export Objects to Excel…" (Ctrl+X)
- Поддерживает hot-reload mapping
- Ищет hero blocks через regex-паттерны

**Запуск:**
```bash
python3 01_tools/gm1_parser.py
python3 01_tools/gm1_parser.py /path/to/save.gm1
```

**Структура JSON-экспорта** (v2.2):
```jsonc
{
  "file_info":   { "raw_size": ..., "magic": "H3SVG", "version_major": 42, "version_minor": 2 },
  "summary":     {
    "total_blocks": 21, "heroes_found": ..., "towns_found": ...,
    "objects_total":         7009,           // ⭐ ВСЕ объекты карты
    "objects_primary":       6006,           // главные на своих тайлах
    "objects_overlay":       1003,           // дополнительные (стопки)
    "objects_unique_coords": 6006,           // уникальных координат
    "wiki_object_types":     2037
  },
  "blocks":      [ ... ],                  // 21 блок сейва с полями
  "heroes_found": [ ... ],
  "towns_found":  [ ... ],
  "objects_on_map": {                       // ⭐ ДВА варианта в одной секции
    "_meta": {
      "map_name":        "Myth and Legend.h3m",
      "total_objects":   7009,
      "primary_objects": 6006,
      "overlay_objects": 1003,
      "unique_coords":   6006,
      "source_files":    { "by_coord": "...", "flat": "..." },
      "notes": [
        "objects_by_coord: one entry per (x,y,z), additional objects on the same tile in 'overlays[]'.",
        "objects_flat:     plain list of ALL 7009 objects, each with its own is_primary flag."
      ]
    },
    "coord_int_lookup": { "57": "57:0:0", "879": "111:3:0", ... },
    "objects_by_coord": [ {x,y,z,type,category,sprite_def,details,overlays[]}, ... ],  // 6006 primary
    "objects_flat":     [ {x,y,z,type,category,sprite_def,details,is_primary}, ... ]   // 7009 flat
  },
  "object_types_dictionary": {
    "_meta":       { "source": "https://heroes.thelazy.net/...", "total_object_types": 2086 },
    "_categories": { "Artifacts": [...], "Monsters": [...], "Towns": [...], ... },
    "_name_index": { "Mountain": ["avlmtd01.def", ...], "Tower": [...], ... },
    "_id_index":   { "5:136": "ava0137.def", ... },
    "objects":     { "ava0137.def": { wiki_name, category, description, ... }, ... }
  }
}
```

---

### 2. `02_format_docs/gm1_mapping.json` — Машиночитаемый конфиг смещений ⭐⭐⭐

**Размер:** 40 KB
**Назначение:** Карта всех известных смещений в `.GM1` сейвах. Используется парсером.

**Содержит:**
- `constants` — константы формата (magic bytes, player colors, version codes)
- `blocks` — массив из 20 блоков с полями (header, player_state, town, hero_main_block, path_block, ...)
- `path_block.path_record_types` — 7 типов path-записей (01 03, 03 03, 04 03, 06 03, 08 03, 09 03, 0b 03)
- `search_patterns` — regex-паттерны для поиска hero blocks
- `known_unknowns` — список того, что ещё не локализовано

**Всего:** 110+ полей в 20 блоках.

**Важно:** Все смещения заданы как hex-строки. Смещения **валидны только для сейвов одной карты** ("Myth and Legend.h3m").

---

### 3. `02_format_docs/GM1_format_compendium.md` — Справка по формату ⭐⭐

**Размер:** 19 KB
**Назначение:** Человекочитаемая документация по формату `.GM1`.

**Содержит:**
- Общая структура файла (диапазоны блоков)
- Заголовок и версии игры
- Состояние игрока (ресурсы, флаги)
- Карта объектов (владельцы)
- Город (структура с 5 слотами гарнизона)
- Герой — основной блок и alt блоки
- Path-block (с детальным описанием 7 типов записей)
- Movement cost — детали (100/141 + модификаторы)
- Список неизвестных блоков

---

### 4. `02_format_docs/diff_interpretation.json` — Сводка дифференциальных анализов

**Размер:** 16 KB
**Назначение:** Машиночитаемая сводка всех 30 дифференциальных анализов.

---

## 🗺️ Маппинг объектов карты

### 5. `03_object_mapping/objects_by_coord.json` — Основной маппинг (primary + overlays) ⭐

**Размер:** 1.8 MB
**Назначение:** Словарь `"x:y:z"` → полная запись объекта карты `Myth and Legend.h3m`. На тайлах с несколькими объектами (декорации на стопке) primary выбирается по приоритету категории, остальные упакованы в `overlays[]`.

**Структура записи:**
```json
{
  "coord_key":     "111:3:0",
  "coord_int":     879,
  "x": 111, "y": 3, "z": 0,
  "object_index":  1,
  "sprite_ref_id": 2,
  "sprite_def":    "AVLholg0.def",
  "sprite_base":   "AVLholg0",
  "type":          "Tower",
  "category":      "town",
  "details":       {"town_name": "Кавала", "player_color": "None", ...},
  "overlays":      [   // ⭐ дополнительные объекты на том же тайле
    {"type": "Pine Trees", "category": "decoration", "sprite_def": "AVLautr6.def", ...}
  ]
}
```

**Статистика:**
- Уникальных координат (primary): **6006**
- Overlays (доп. объекты): **1003**
- **Всего объектов: 7009** (6006 primary + 1003 overlays)
- Различных типов: 204
- Спрайтов использовано: 546/548

**Когда использовать:**
- Lookup по `coord_int` (один результат на тайл)
- Получение primary объекта тайла
- Анализ overlays на тайле

---

### 5a. `03_object_mapping/objects_flat.json` — Плоский список всех 7009 объектов ⭐ НОВОЕ

**Размер:** 2.1 MB
**Назначение:** Плоский массив всех 7009 объектов карты (без вложений `overlays[]`). Каждый объект имеет поле `is_primary` (true — главный объект тайла, false — overlay).

**Структура:**
```jsonc
{
  "_meta": {
    "map_name":         "Myth and Legend.h3m",
    "total_objects":    7009,           // ← ВСЕ объекты
    "primary_objects":  6006,           // главные на своих тайлах
    "overlay_objects":  1003,           // дополнительные (стопки декораций)
    "unique_coords":    6006,
    "source_file":      "objects_by_coord.json",
    "coord_encoding":   "coord_int = x | (y << 8) | (z << 16)",
    "sort_order":       "by coord_int, then primary-first, then object_index"
  },
  "objects": [
    {
      "coord_key":    "57:0:0",
      "coord_int":    57,
      "x": 57, "y": 0, "z": 0,
      "object_index": 7,
      "sprite_def":   "AvLRD02.def",
      "type":         "Crater",
      "category":     "decoration",
      "details":      {},
      "is_primary":   true
    },
    {
      "coord_key":    "111:3:0",
      "coord_int":    879,
      "x": 111, "y": 3, "z": 0,
      "object_index": 1,
      "sprite_def":   "AVLholg0.def",
      "type":         "Tower",
      "category":     "town",
      "details":      {"town_name": "Кавала", ...},
      "is_primary":   true
    },
    // 1003 overlay-записей имеют is_primary=false и тот же coord_int, что и primary:
    {
      "coord_key":    "41:18:0",
      "coord_int":    4649,
      "x": 41, "y": 18, "z": 0,
      "object_index": 3922,
      "sprite_def":   "AVLautr6.def",
      "type":         "Pine Trees",
      "category":     "decoration",
      "details":      {},
      "is_primary":   false           // ⭐ overlay на тайле Prison
    },
    ...
  ]
}
```

**Когда использовать:**
- Перебор всех 7009 объектов без рекурсии
- Фильтрация/подсчёт по типу/категории (без отдельной обработки overlays)
- Итерация в скриптах: `for o in objects: ...`

**Скрипт-генератор:** `04_diff_analysis/build_objects_flat.py` (читает `objects_by_coord.json`, разворачивает overlays в плоский массив)

---

### 6. `03_object_mapping/coord_int_lookup.json` — Быстрый lookup по coord_int

**Размер:** 128 KB
**Назначение:** Словарь `coord_int → "x:y:z"`. Используется парсером сейвов, чтобы по 4-байтной координате из сейва быстро находить тип объекта.

**Кодировка:** `coord_int = x | (y << 8) | (z << 16)` (z=0 surface, z=1 underground)

---

### 7. `03_object_mapping/object_types_dictionary.json` — Словарь типов объектов ⭐ НОВОЕ

**Размер:** 361 KB
**Назначение:** Редактируемый словарь всех типов объектов HoMM3, собранный с LazyLlama wiki.

**Источник:** https://heroes.thelazy.net/index.php/Map_Editor_Objects (revision 194767, 2086 строк таблицы)

**Структура:**
```jsonc
{
  "_meta": {
    "source": "https://heroes.thelazy.net/index.php/Map_Editor_Objects",
    "page_revision": 194767,
    "fetched_at": "2026-10-08",
    "total_object_types": 2086,
    "categories": { "Monsters": 183, "Artifacts": 164, "Swamp": 131, ... }
  },
  "_categories": { "Artifacts": ["Spell Scroll", ...], "Monsters": [...], ... },
  "_name_index": { "Mountain": ["avlmtd01.def", ...], ... },   // Object Name → [file_name]
  "_id_index":   { "5:136": "ava0137.def", ... },               // objID:subID → file_name
  "objects": {
    "ava0001.def": {
      "wiki_name":    "Spell Scroll",
      "file_name":    "AVA0001.def",
      "category":     "Artifacts",
      "generates_on": "All except Rock",
      "restrictions": "Rock",
      "object_id":    "93",
      "sub_id":       "0",
      "layering":     "-",
      "description":  "This scroll contains a spell...",
      "user_edits":   {                    // ⭐ РЕДАКТИРУЕМО
        "user_notes":    "",
        "user_category": "",
        "user_alias":    ""
      }
    },
    ...
  }
}
```

**Ключевые особенности:**
- **Первичный ключ** — `.def` file name (lowercase), уникальный для каждой строки wiki
- **Все 2086 строк** сохранены (многие Object Names дублируются — например 87 "Mountain" с разными .def)
- **`user_edits`** — пользовательские поля, которые **сохраняются при пересоздании словаря**. Скрипт `build_object_type_dictionary.py` при перегенерации загружает существующие `user_edits` и не затирает их
- **Три индекса** для быстрого поиска:
  - `_name_index`: по имени объекта (Object Name → list of .def files)
  - `_id_index`: по паре ObjectID:SubID → .def file
  - `_categories`: по категории (Category → list of Object Names)

**Статистика:**
- 2037 уникальных .def файлов
- 819 уникальных Object Names
- 857 уникальных пар ObjectID:SubID
- 53 категории (Artifacts, Monsters, Towns, Dwellings, Terrains, ...)

**Как редактировать:**
1. Откройте `object_types_dictionary.json` в любом JSON-редакторе
2. Найдите нужный объект по `.def` файлу или через `_name_index` по имени
3. Заполните поля в `user_edits`:
   - `user_notes` — ваши заметки
   - `user_category` — своя категория (переопределяет wiki)
   - `user_alias` — альтернативное имя (для поиска)
4. Сохраните файл
5. При пересоздании словаря (новая версия wiki) — `user_edits` будут сохранены

**Скрипт-генератор:** `04_diff_analysis/build_object_type_dictionary.py` (читает `/tmp/lazymap.html`, который нужно скачать отдельно: `curl -o /tmp/lazymap.html https://heroes.thelazy.net/index.php/Map_Editor_Objects`)

---

### 8. `03_object_mapping/map_types_xref.json` — Кросс-референс типов карты ⭐ НОВОЕ

**Размер:** 37 KB
**Назначение:** Кросс-референс типов объектов, найденных на карте "Myth and Legend.h3m", с записями wiki.

**Структура:**
```jsonc
{
  "Tower": {
    "map_count":    3,           // сколько раз встречается на карте
    "wiki_match":   "Tower",     // имя в wiki (если найдено)
    "wiki_id":      "98",
    "wiki_sub_id":  "0",
    "wiki_file":    "avltowr0.def",
    "wiki_category": "Towns"
  },
  "Wraight": {                   // ⚠ нет совпадения в wiki
    "map_count":    7,
    "wiki_match":   "",
    "wiki_id":      "",
    ...
  },
  ...
}
```

**Статистика соответствия:**
- 190 из 203 типов карты найдены в wiki (94%)
- 13 типов не найдены (опечатки в карте или альтернативные названия):
  - `Wraight` (7) — должно быть `Wraith`
  - `Monolith Two Way` (6) — в wiki пишется как `Two-way Monolith`
  - `Centaur Capitan` (2) — должно быть `Centaur Captain`
  - `Light blue` (2) — вероятно, цвет игрока
  - `Monolith One Way Entrance`/`Exit` — варианты написания
  - `Daemon` (1) — должно быть `Demon` или `Devil`
  - `Air/Earth/Water/Fire Elemental Conflux` — варианты elemental conflux
  - `Monastery` (1) — название из HotA
  - `Dead Knight` (1) — должно быть `Death Knight`

Эти расхождения можно исправить в `user_edits.user_alias` словаря типов.

---

### 9. `03_object_mapping/coord_mapping_verification.json` — Верификация против сейва

**Размер:** 556 KB
**Назначение:** Доказательство, что маппинг корректен.

**Результат верификации:**
- Все 1751 недекоративных объектов найдены в декомпрессированном `312.GM1`
- 100% совпадение

---

### 10. `03_object_mapping/save_offset_clusters.{json,md}` — Карта кластеров сейва

**Размер:** 266 KB / 74 KB
**Назначение:** Группировка всех смещений coord-байтов в кластеры — показывает структуру секций сейва.

**Главные кластеры в декомпрессированном `312.GM1`:**
| Offset | Hits | Что это |
|---|---:|---|
| `0x120C8C` | 7269 | Главный массив состояний объектов (~все 7009 объектов) |
| `0x118C1C` | 616 | Visiting-objects array (герои/города у объектов) |
| `0x172129` | 576 | Passability / decoration-shadow bitmask |
| `0x9D892` | 283 | "Alive objects" overlay |
| `0x7E9F7` | 293 | Fog-of-war / discovered-objects array |

---

### 11. `03_object_mapping/object_mapping_summary.json` — Сводка статистики

**Размер:** 5 KB
**Назначение:** Top-level counts для быстрой проверки.

---

## 🔧 Инструменты для дифференциального анализа

### 12. `01_tools/gm1_diff.py` — CLI компаратор сейвов

**Размер:** 37 KB
**Назначение:** Дифференциальный анализатор двух `.GM1` сейвов.

**Возможности:**
- `--info FILE` — базовая информация о сейве (magic, version, map name)
- `FILE_A FILE_B` — байт-уровневый diff (по умолчанию)
- `--section-map` — гистограмма стабильных/изменённых блоков
- `--annotated --annotations FILE` — атрибуция изменений к известным полям
- `--strings MIN_LEN` — поиск всех ASCII-строк
- `--hexdump START END` — hex-дамп диапазона
- `--find PATTERN` — поиск байт-паттерна

---

### 13. `01_tools/gm1_diff_gui.py` — GUI версия компаратора

**Размер:** 28 KB
**Назначение:** PySide6 GUI обёртка над `gm1_diff.py`. Те же возможности, но с графическим интерфейсом.

---

## 📦 Примеры (examples/)

### 14. `examples/parsed_save_312_with_objects.json` — пример полного экспорта ⭐

**Размер:** 6.9 MB
**Назначение:** Полный JSON-экспорт сейва `312.GM1` (карта "Myth and Legend.h3m"), сгенерированный обновлённым `gm1_parser.py` (v2.2).

**Содержит:**
- `file_info` — magic, version, размер
- `summary` — счётчики: 21 блок, 205 heroes, 64 towns, **7009 объектов карты** (6006 primary + 1003 overlays), 2037 типов в словаре
- `blocks` — 21 блок сейва с полями (offset, size, type, value, raw_bytes)
- `heroes_found` — 205 кандидатов в hero blocks (regex-поиск)
- `towns_found` — 64 кандидата в town records
- **`objects_on_map`** — два варианта представления объектов карты:
  - `_meta` — total/primary/overlay/unique_coords, source_files, notes
  - `coord_int_lookup` — словарь `coord_int → "x:y:z"` (6006 записей)
  - `objects_by_coord` — список 6006 primary-записей с overlays[] для каждого тайла
  - `objects_flat` — список всех **7009** объектов (без вложений), каждый с `is_primary` флагом
- **`object_types_dictionary`** — словарь 2037 типов объектов из LazyLlama wiki

**Как использовать:**
- Откройте в любом JSON-вьюере (VSCode, jq, http://jsonviewer.stack.hu/) для интерактивного изучения
- Или загружите в Python скриптом:
  ```python
  import json
  data = json.load(open('examples/parsed_save_312_with_objects.json'))

  # Перебор ВСЕХ 7009 объектов карты (включая overlays):
  for o in data['objects_on_map']['objects_flat']:
      if o['category'] == 'town':
          print(f"({o['x']:3d},{o['y']:3d},{o['z']}) {o['type']:10s} "
                f"{o['details'].get('town_name','')}")

  # Или только primary (без декораций-оверлеев):
  for o in data['objects_on_map']['objects_by_coord']:
      if o['category'] == 'town':
          print(f"({o['x']:3d},{o['y']:3d},{o['z']}) {o['type']:10s}")
          for ovl in o.get('overlays', []):
              print(f"  └ overlay: {ovl['type']}")
  ```

**Как воспроизвести:**
```bash
python3 01_tools/gm1_parser.py /path/to/312.GM1
# Затем в GUI: File → Export JSON…
```

---

### 15. `examples/homm3_map_objects.xlsx` — Excel-экспорт всех объектов ⭐ НОВОЕ

**Размер:** 1.1 MB
**Назначение:** Excel-файл со всеми **7009 объектами** карты `Myth and Legend.h3m`, сгенерированный функцией `export_objects_to_excel()` из `gm1_parser.py` (v2.5).

**Содержит 4 листа:**

| Лист | Строк | Колонок | Описание |
|------|------:|--------:|----------|
| **All Objects** | 7010 | 18 | Все 7009 объектов (плоский список, без вложений). Колонки: #, X, Y, Z, coord_int, coord_key, is_primary, object_index, sprite_ref_id, sprite_def, type, category, details (JSON), wiki_name, wiki_category, wiki_object_id, wiki_sub_id, wiki_description |
| **By Category** | 7010 | 10 | Те же 7009 объектов, отсортированные по 21 категории. Колонки: category, #, X, Y, Z, type, sprite_def, is_primary, object_index, details |
| **Summary** | 236 | 3 | Статистика: meta-info (8 строк), категории (21 строка), типы (204 строки) |
| **Wiki Dictionary** | 2038 | 12 | Все 2037 типов объектов из LazyLlama wiki. Колонки: file_name, wiki_name, category, object_id, sub_id, generates_on, restrictions, layering, description, user_notes, user_category, user_alias |

**Особенности:**
- Заголовки окрашены синим (#305496), белый жирный шрифт
- Все колонки авто-подогнаны по ширине (с ограничением 60 символов для длинных описаний)
- Включён **AutoFilter** на листах "All Objects", "By Category" и "Wiki Dictionary" — можно фильтровать по любому полю
- Закреплён верхний ряд (freeze_panes) — заголовки видны при скролле

**Как воспроизвести:**
```bash
python3 01_tools/gm1_parser.py
# В GUI: File → Export Objects to Excel… (или Ctrl+X)
# Или программно:
python3 -c "
import sys, os
sys.path.insert(0, '01_tools')
# Заглушки PySide6 не нужны, если вызывать напрямую:
from gm1_parser import export_objects_to_excel
export_objects_to_excel({}, b'', '/tmp/output.xlsx')
"
```

**Требования:** `pip install openpyxl`

---

## 📜 Скрипты дифференциального анализа (04_diff_analysis/)

Каждый скрипт анализирует конкретную пару сейвов (до/после действия). Скрипты независимы друг от друга.

### Серия 001 (старт игры → ключевые действия)

| Скрипт | Пара сейвов | Что локализовано |
|--------|-------------|------------------|
| `analyze_diff.py` | 001→0011 (движение героя) | Общий фреймворк, кластеризация изменений |
| `analyze_path.py` | — | Path-записи: типы `01 03` (движение) и `0b 03` (fog of war) |
| `analyze_build_diff.py` | 0011→0012 (постройка здания) | `0x13F17B` (флаг постройки), `0x13E420` (ID здания) |
| `analyze_xp_diff.py` | 0012→0013 (опыт + уровень) | experience (`0x157F04`), level (`0x157F08`) |
| `analyze_artifact_diff.py` | 0013→0014 (покупка артефакта) | Hero artifact inventory (`0x163865`), Black Market (`0x16A2D4`) |
| `analyze_hire_diff.py` | 0014→0015 (найм существ) | Town garrison army_types/counts, player gold |
| `analyze_transfer_diff.py` | 0015→0016 (передача армии) | Hero army_types/counts. Новый path-record `08 03` |
| `analyze_spellbook_diff.py` | 0016→0017 (книга магии) | has_spell_book, spell_bit_array, spell_book_slot |
| `analyze_path_dest.py` | 0017→0018 (изменение пути) | queued_path_destination (`0x1636B2`) |
| `analyze_ballista_diff.py` | 0018→0019 (покупка баллисты) | ballista_slot, ammo_cart_slot, first_aid_tent_slot, catapult_slot |
| `analyze_stables_diff.py` | 0019→0020 (конюшня) | hero visit counter, visit flag, last_visited_object_id |

### Серия 0020 (читы и сложные сценарии)

| Скрипт | Пара сейвов | Что локализовано |
|--------|-------------|------------------|
| `analyze_cheat_diff.py` | 0020→0020_1 (чит-код) | cheater_flag, current_movement_points |
| `analyze_mine_diff.py` | 0020_1→0020_2 (захват шахты) | object_owner. Новый path-record `03 03` |
| `analyze_fountain_diff.py` | 0020_2→0020_3 (фонтан удачи) | visited_objects bitmask |
| `analyze_upgrade_diff.py` | 0020_3→0020_4 (форт улучшений) | map_object_visiting_coords, Hero #3 block |
| `analyze_disembark_diff.py` | 0020_4→0020_5 (высадка из лодки) | hero_on_boat_flag, hero_surface_flag. Новый path-record `06 03` |
| `analyze_cheat2_diff.py` | 0020_5→0020_6 (чит-коды) | alt movement_points, army_count_slot_alt |
| `analyze_battle_diff.py` | 0020_6→0020_7 (битва за город) | town_owner_color, town_garrison_army. Новый path-record `04 03` |
| `analyze_moves_diff.py` | 0020_7→0020_8 (движение 2 героев) | hero visit_id swap, global turn counters |
| `analyze_battle2_diff.py` | 0020_8→0020_9 (победа над врагом) | enemy_hero_state, enemy_hero_defeated_block |
| `analyze_mercenary_diff.py` | (доп. анализ) | Наёмник — детали |
| `analyze_447_diff.py` | 447_1→447_2 ⭐ | army swap (slot 0 ↔ slot 4), artifact equip (Speculum id=52 from backpack to doll), 7 новых полей локализовано |
| `analyze_447_2_to_3_diff.py` | 447_2→447_3 ⭐ НОВОЕ | построена Magic Guild Lv3 (level counter 2→3 @ 0x13F74D, building bitmask bit 1→2, 7 player resources deducted, spell slots rearranged) |

### Серия 447 (army swap, artifact equip, Magic Guild upgrade) ⭐

| Скрипт | Пара сейвов | Что локализовано |
|--------|-------------|------------------|
| `analyze_447_diff.py` | 447_1→447_2 | Army swap + artifact equip из backpack на doll |

**Подробности находок (447_1 → 447_2):**

Пользователь сделал два действия:
1. Поменял местами существ в армии героя
2. Надел артефакт из рюкзака на куклу героя

**Локализованные поля (7 новых):**

| Поле | Смещение | Размер | Описание |
|------|----------|-------:|----------|
| `hero_army_types[5]` | `0x143E52` | 20 B | Hero army creature IDs (5 slots × 4 bytes); `0xFFFFFFFF` = пусто |
| `hero_army_counts[5]` | `0x143E6E` | 20 B | Hero army creature counts (5 slots × 4 bytes) |
| `hero_backpack_count` | `0x143DDD` | 1 B | Количество артефактов в рюкзаке героя (29 → 28 после equip) |
| `hero_doll_slot_N` | `0x143FA7` | 4 B | Слот equipped артефакта на кукле (`0xFFFFFFFF` = пусто) |
| `hero_backpack_slots` | `0x14400F` | 8×N | Backpack slots: 4-байтный artifact_id + 4-байтный data. Каскадный сдвиг при удалении |
| `day_or_step_counter` | `0x000003B7` | 1 B | Счётчик действий (+1 за каждое действие пользователя) |
| `hero_alt_flag_per_hero` | `0x1789AB + 0x1E0*N` | 1 B | Флаг на каждого героя (0x04 → 0x0c после действия) |

**Ключевые инсайты:**

1. **Структура backpack slot**: 8 байт на слот (4-байтный artifact_id + 4-байтный data). Поскольку ID артефактов < 256, при каскадном сдвиге меняется только первый байт каждого slot.
2. **Hero army в этом сейве имеет 5 активных слотов** (slot[0]..slot[4]); slots 5+ заняты текстом имени героя.
3. **Equip артефакта НЕ вызывает пересчёт статов** в hero block — только doll slot, backpack count и сам backpack сдвигаются. (В отличие от некоторых других игр.)
4. **Счётчик `0x000003B7` увеличивается на 1 за каждое действие** (army swap + equip = 1 действие, не 2).
5. **HotA replay log** (base64-encoded ASCII) обновляется при каждом действии (смещения 0x130B07-0x130B8F).
6. **Hero alt blocks** имеют stride 0x1E0 = 480 байт — для каждого из 10 героев флаг 0x04 → 0x0c (бит 2 и 3 установлены).
7. **HD3 trailer** в конце файла (HotA extension marker) может быть обнулён при определённых условиях.

**Отчёты:**
- `04_diff_analysis/analyze_447_diff_report.json` — машинно-читаемый отчёт (12 KB)
- `04_diff_analysis/analyze_447_diff_report.md` — человекочитаемый отчёт (5 KB)

---

### 447_2 → 447_3: построена Magic Guild Lv3 ⭐ НОВОЕ

**Пользовательское действие:** построил гильдию магов 3 уровня в замке (был Lv2 → стал Lv3).

**Локализованные поля (7 новых):**

| Поле | Смещение | Размер | Описание |
|------|----------|-------:|----------|
| `mage_guild_level` | `0x13F74D` | 1 B | Текущий уровень Magic Guild (1-5). **2 → 3** = уровень повышен ⭐ |
| `town_build_flag` | `0x13F70B` | 1 B | Флаг "в этом ходу построено здание" (0 → 1) |
| `building_bitmask` | `0x13F7AB` | 4 B | Town building bitmask. byte[3] хранит ТЕКУЩИЙ уровень guild (0x22→0x24: bit 1 cleared, bit 2 set) |
| `town_bitmask_byte` | `0x13F7B6` | 1 B | Дополнительный town bitmask (0xA3→0xA7: bit 2 set = Lv3 marker) |
| `player_resources[7]` | `0x13E3E7` | 28 B | 7 × 4-байтных LE int: Wood, Mercury, Ore, Sulfur, Crystal, Gems, Gold |
| `town_spell_slots` | `0x13E380+` | variable | Town spell pool (FF=empty). Mage Guild upgrade → spell list rearranged |
| `hero_spell_book_bitmask` | `0x140246` | 1 B | Per-spell "cast this turn" flag bitmask (0xFB→0xEB: bit 4 cleared) |

**Стоимость Magic Guild Lv3 — идеально подтверждена диффом:**

| Ресурс | До (447_2) | После (447_3) | Δ | Ожидаемая стоимость | Совпадение? |
|--------|----------:|--------------:|------:|---------------------:|:-----------:|
| Wood   | 1,121     | 1,116         | **-5** | 5 | ✅ |
| Ore    | 892       | 887           | **-5** | 5 | ✅ |
| Gold   | 2,543,782 | 2,542,782     | **-1000** | 1000 | ✅ |

Остальные ресурсы (Mercury -6, Sulfur -6, Crystal -6, Gems -6) — вероятно, от daily mine income tick (между сейвами прошёл ход или другое событие).

**Spell list rearrangement:**

При апгрейде Magic Guild Lv3 — spell pool обновляется:
- Spell ID 13 (**View Earth**) удалён из слота `0x13E387` → теперь 0xFF (empty)
- Spell ID 18 (**Disrupting Ray**) добавлен в слот `0x13E39E` (раньше 0xFF = empty)

**Ключевые инсайты:**

1. **Mage Guild level counter** — простой 1-байтный счётчик на `0x13F74D`. Не путать с building bitmask!
2. **Building bitmask encoding** хранит только бит ТЕКУЩЕГО уровня (не все построенные уровни). При апгрейде Lv2→Lv3: bit 1 cleared, bit 2 set.
3. **Mage Guild upgrade** перестраивает spell pool — старые spells могут удалиться, новые добавиться (не только добавить).
4. **Hero spell book bitmask** на `0x140246` хранит per-spell "cast this turn" флаги (бит сброшен = spell был использован).
5. **Player resources layout**: 7 × 4-byte LE ints в порядке `[Wood, Mercury, Ore, Sulfur, Crystal, Gems, Gold]` начиная с `0x13E3E7`.
6. **Только 13 byte ranges изменилось** — постройка здания один из самых "чистых" диффов в формате H3.
7. **Building bitmask** и **Mage Guild level counter** — отдельные поля (не одно и то же).

**Отчёты:**
- `04_diff_analysis/analyze_447_2_to_3_diff_report.json` — машинно-читаемый отчёт (7 KB)
- `04_diff_analysis/analyze_447_2_to_3_diff_report.md` — человекочитаемый отчёт (5 KB)

---

### Скрипты для маппинга объектов

| Скрипт | Назначение |
|--------|------------|
| `build_object_mapping.py` | Построение `03_object_mapping/objects_by_coord.json` + индексов из `.h3m.json` (7009 объектов → 6006 unique coords + 1003 overlays) |
| `build_objects_flat.py` | ⭐ Построение `03_object_mapping/objects_flat.json` — плоский список всех 7009 объектов (без вложений overlays) |
| `build_object_type_dictionary.py` | ⭐ Построение `object_types_dictionary.json` из LazyLlama wiki HTML (2037 типов с user_edits) |
| `verify_coord_mapping.py` | Верификация маппинга против декомпрессированного `.GM1` (100% match: 1751/1751) |
| `analyze_save_clusters.py` | Группировка coord-байтов в кластеры смещений в сейве |

### Вспомогательные скрипты

| Скрипт | Назначение |
|--------|------------|
| `regenerate_mapping.py` | Генератор `gm1_mapping.json` (использовался для перехода decimal→hex) |
| `test_parser.py` | Тест парсера без GUI (заглушки PySide6) на реальных сейвах |

---

## 🔬 Дизассемблирование (05_disasm/)

Результаты анализа `heroes3.exe` (PE32, SoD build). Созданы скриптами из `06_disasm_scripts/`.

### Файлы с метаданными

| Файл | Назначение |
|------|------------|
| `disasm_summary.json` | Сводка PE-структуры: image base, entry point, секции |
| `all_strings.txt` | Все 9680 ASCII-строк из PE с VA и секциями |
| `interesting_strings.txt` | Отфильтрованные строки, релевантные сейвам |
| `imports.txt` | Все импорты PE (DLL + функции) |

### Файлы с функциями сериализации

| Файл | Назначение |
|------|------------|
| `save_io_funcs.json` | Найденные функции save/load |
| `save_callgraph.json` | Call-graph 362 функций сериализации |
| `save_functions.json` | Описания 18 функций сериализации |
| `save_functions_disasm.txt` | Полный дизассемблер этих 18 функций |
| `magic_xrefs_disasm.txt` | Дизассемблер вокруг xref'ов на H3SVG/H3SVC/AUTOSAVE |

### `func_*.asm` (15 файлов)

Дизассемблированный код ключевых функций:
- `func_SAVE_WRITER_0x000be0b0.asm` — главный save writer
- `func_SAVE_READER_0x000bca60.asm` — главный save reader
- `func_SAVE_WRITER_CONTENT_0x000bc290.asm` — виртуальный сериализатор
- `func_HEADER_WRITER_0x000bbda0.asm` — пишет H3SVG magic + версию
- и др.

---

## 🛠️ Скрипты реверса EXE (06_disasm_scripts/)

Эти скрипты создавали содержимое `05_disasm/`. Запускаются на `heroes3.exe` (SoD build, PE32).

| Скрипт | Назначение |
|--------|------------|
| `disasm_h3.py` | Главный дизассемблер: PE → JSON + TXT файлы |
| `disasm_save_io.py` | Поиск функций save I/O по magic-строкам H3SVG/H3SVC |
| `disasm_serialize_content.py` | Дизассемблирование виртуального сериализатора |
| `disasm_serialize_methods.py` | Дизассемблирование методов сериализации (vtable) |
| `find_main_serializer.py` | Поиск главного сериализатора по call-graph |
| `find_save_io.py` | Поиск точек входа в save/load |
| `find_source_paths.py` | Поиск путей к исходникам в EXE |
| `find_vtables.py` | Извлечение vtable-ов из RTTI |
| `find_xrefs.py` | Поиск xref'ов к заданным символам |
| `callgraph_analysis.py` | Анализ call-graph, поиск подсети сериализации |
| `analyze_main_func.py` | Анализ главной функции игры |
| `analyze_save_funcs.py` | Анализ найденных save-функций, классификация |

---

## 🗑️ Устаревшие файлы (_obsolete/)

| Файл | Почему устарел |
|------|----------------|
| `old_root/README_old.md` | Старый пустой README (34 байта). Заменён на `README.md`. |
| `old_root/gm1_essential.zip` | Старый частичный архив (52 KB). Заменён на полный `homm3_gm1_toolkit.zip`. |
| `old_scripts/gm1_diff_old.py` | Старая версия `gm1_diff.py` (35 KB). Заменена новой (37 KB) с `--annotated`/`--section-map`. |
| `old_disasm/diff_interpretation_old.json` | Старая версия (8 KB, 19 диффов). Заменена новой (16 KB, 30 диффов). |
| `old_disasm/gm1_mapping_old.json` | Старая версия с decimal смещениями. Заменена на hex-строки. |

---

## 🎯 Рекомендуемый порядок использования

### Сценарий 1: Исследовать новый сейв
1. Открыть `01_tools/gm1_parser.py` → File → Open .GM1…
2. Изучить дерево блоков
3. **Export JSON** — теперь включает `objects_on_map` со всеми 6006 объектами карты + `object_types_dictionary` с описаниями

### Сценарий 2: Найти тип объекта по координате из сейва
1. Прочитать 3-байтную координату по нужному смещению в сейве
2. Вычислить `coord_int = x | (y << 8) | (z << 16)`
3. Найти объект в `03_object_mapping/coord_int_lookup.json` → `objects_by_coord.json`
4. Получить тип, категорию, .def файл
5. Посмотреть детали в `03_object_mapping/object_types_dictionary.json` по .def файлу

### Сценарий 3: Найти новое смещение
1. Сделать два сейва (до/после действия)
2. Запустить `01_tools/gm1_diff.py A.gm1 B.gm1`
3. Проанализировать изменённые диапазоны
4. Добавить находку в `02_format_docs/gm1_mapping.json`

### Сценарий 4: Поправить словарь типов объектов
1. Открыть `03_object_mapping/object_types_dictionary.json`
2. Найти нужный объект (по .def файлу или через `_name_index` по имени)
3. Заполнить поля в `user_edits`:
   - `user_notes` — заметки
   - `user_category` — своя категория
   - `user_alias` — альтернативное имя (для устранения опечаток map → wiki)
4. Сохранить файл
5. При пересоздании словаря (новая версия wiki) — `user_edits` будут сохранены

### Сценарий 5: Продолжить реверс-инжиниринг EXE
1. Посмотреть `02_format_docs/diff_interpretation.json` → `open_questions`
2. Изучить дизассемблер в `05_disasm/save_functions_disasm.txt`
3. Использовать `06_disasm_scripts/find_*.py` для поиска новых функций

---

## 📊 Статистика по проекту

| Метрика | Значение |
|---------|----------|
| Дифференциальных анализов выполнено | 32 |
| Полей локализовано | 124+ |
| Блоков описано | 20 |
| Типов path-записей | 7 |
| Функций сериализации найдено в EXE | 18 |
| Строк в дизассемблере | 9680 |
| Скриптов анализа | 34 |
| Объектов карты замапплено | 7009 (6006 уникальных координат, 204 типа, 21 категория) |
| Типов объектов в словаре wiki ⭐ | 2037 (из 2086 строк, 819 уникальных имён, 53 категории) |
| Верификация маппинга против сейва | 100% (1751/1751) |
| Покрытие формата (оценка) | ~35-45% (основные структуры + town buildings) |

---

## 📝 Что НЕОБХОДИМО для дальнейшего прогресса

1. **Декодировать массив объектов `0x120C8C`** в сейве — фиксированный шаг между координатами даст размер записи
2. **Каждому городу/герою в сейве сопоставить тип** через `coord_int_lookup` — это раскроет, какой именно город привязан к town record
3. **Найти указатели в заголовке сейва** на начало hero blocks, town records, object array — это позволит парсить сейвы с разных карт
4. **Локализовать террайн** (тайлы карты)
5. **Найти fog of war bit mask** отдельно от path-records
6. **Локализовать текущий день/неделю/ход**
7. **Локализовать AI state**

---

## 📜 Журнал изменений архива

### v2.7 (2026-10-08)
- **Добавлено**: `01_tools/map_json_loader.py` — модуль загрузки JSON-парсинга карты (из .json или .zip). Делает парсер **универсальным для любой карты**: вместо жёстко заданных файлов `03_object_mapping/` парсер динамически строит `objects_by_coord`, `coord_int_lookup`, `type_index`, `category_index` из загруженного JSON
- **Обновлено**: `01_tools/gm1_parser.py` — добавлена кнопка **"Load Map JSON…"** (Ctrl+M) в тулбар и меню `File → Load Map JSON…`. При загрузке:
  - Парсится JSON карты (любой структуры, совместимой с h3m-парсером)
  - Извлекаются: map_name, map_size, has_underground, objects, sprites, players
  - Динамически строятся objects_by_coord (с overlays), coord_int_lookup, type_index, category_index
  - Перестраивается дерево объектов и таблица "Map Objects"
  - Показывается диалог с информацией о карте (размер, кол-во объектов, городов, героев)
- **Обновлено**: `FILES_DESCRIPTION.md` — добавлено описание `map_json_loader.py`

### v2.6 (2026-10-08)
- **Обновлено**: `01_tools/gm1_parser.py` — добавлены:
  - **Адреса объектов в JSON-экспорте** — каждый объект теперь содержит `main_offset`, `visiting_offset`, `fog_offset`, `alive_offset`, `treasure_offset`, `decoration_offset`, `save_offsets`, `verified`
  - **Новая вкладка "Map Objects" в GUI** — таблица всех 7009 объектов с фильтром по тексту/категории, сортировкой, и двойным кликом для показа деталей + hex-дампа вокруг `main_offset`
  - **Адреса в Excel-экспорте** — лист "All Objects" теперь имеет 26 колонок (включая 7 колонок с адресами)
- **Добавлено**: `03_object_mapping/objects_with_offsets.json` (2.0 MB) — словарь адресов каждого объекта в сейве (1725 verified из 6006 primary)
- **Добавлено**: `04_diff_analysis/build_objects_with_offsets.py` — скрипт-генератор адресов объектов
- **Обновлено**: `examples/homm3_map_objects.xlsx` (1.4 MB) — 26 колонок с адресами
- **Обновлено**: `examples/parsed_save_312_with_objects.json` (10.3 MB) — включает адреса объектов

### v2.5 (2026-10-08)
- **Обновлено**: `01_tools/gm1_parser.py` — добавлена функция `export_objects_to_excel()` и кнопка "Export Objects to Excel…" (горячая клавиша `Ctrl+X`). Создаёт `.xlsx` с 4 листами: All Objects (7009×18), By Category (7009×10), Summary (236×3), Wiki Dictionary (2038×12). Требует `openpyxl`.
- **Добавлено**: `examples/homm3_map_objects.xlsx` (1.1 MB) — пример Excel-экспорта всех 7009 объектов карты
- **Обновлено**: `FILES_DESCRIPTION.md` — добавлена секция 15 (Excel-экспорт), обновлено описание парсера с упоминанием новой кнопки

### v2.4 (2026-10-08)
- **Добавлено**: `04_diff_analysis/analyze_447_2_to_3_diff.py` — анализ диффа 447_2→447_3 (построена Magic Guild Lv3)
- **Добавлено**: `04_diff_analysis/analyze_447_2_to_3_diff_report.json` (7 KB) — машинно-читаемый отчёт
- **Добавлено**: `04_diff_analysis/analyze_447_2_to_3_diff_report.md` (5 KB) — человекочитаемый отчёт
- **Обновлено**: `02_format_docs/gm1_mapping.json` — секция `alternative_offsets.save_447_series` расширена с 7 до 14 полей (добавлены mage_guild_level, town_build_flag, building_bitmask, town_bitmask_byte, player_resources[7], town_spell_slots, hero_spell_book_bitmask)
- **Обновлено**: `02_format_docs/diff_interpretation.json` — добавлен 32-й дифф-анализ (447_2 vs 447_3), 7 новых полей (всего 124+)
- **Обновлено**: `FILES_DESCRIPTION.md` — добавлена подсекция "447_2 → 447_3: построена Magic Guild Lv3" с детальным описанием находок

### v2.3 (2026-10-08)
- **Добавлено**: `04_diff_analysis/analyze_447_diff.py` — анализ диффа 447_1→447_2 (army swap + artifact equip)
- **Добавлено**: `04_diff_analysis/analyze_447_diff_report.json` — машинно-читаемый отчёт (12 KB)
- **Добавлено**: `04_diff_analysis/analyze_447_diff_report.md` — человекочитаемый отчёт (5 KB)
- **Обновлено**: `02_format_docs/gm1_mapping.json` — добавлена секция `alternative_offsets.save_447_series` с 7 новыми полями
- **Обновлено**: `02_format_docs/diff_interpretation.json` — добавлен 31-й дифф-анализ (447_1 vs 447_2), 7 новых полей (всего 117+)
- **Обновлено**: `FILES_DESCRIPTION.md` — добавлена секция "Серия 447" с детальным описанием находок

### v2.2 (2026-10-08)
- **Обновлено**: `01_tools/gm1_parser.py` — теперь в JSON-экспорте объекты карты представлены в ДВУХ вариантах:
  - `objects_on_map.objects_by_coord` — 6006 primary-записей с overlays[] (как раньше)
  - `objects_on_map.objects_flat` — плоский список всех **7009** объектов с `is_primary` флагом
  - `summary` теперь показывает `objects_total: 7009` (а не 6006 как в v2.1)
- **Добавлено**: `03_object_mapping/objects_flat.json` (2.1 MB) — плоский список всех 7009 объектов
- **Добавлено**: `04_diff_analysis/build_objects_flat.py` — скрипт-генератор плоского списка из `objects_by_coord.json`
- **Обновлено**: `examples/parsed_save_312_with_objects.json` — теперь 6.9 MB с обоими вариантами объектов
- **Обновлено**: `FILES_DESCRIPTION.md` — добавлены секции 5a (objects_flat), обновлена структура JSON-экспорта v2.2

### v2.1 (2026-10-08)
- **Обновлено**: `01_tools/gm1_parser.py` — экспортирует в JSON три новые секции (objects_on_map, object_types_dictionary, coord_int_lookup)
- **Добавлено**: `03_object_mapping/object_types_dictionary.json` — редактируемый словарь 2037 типов объектов из LazyLlama wiki (с `user_edits` для правок)
- **Добавлено**: `03_object_mapping/map_types_xref.json` — кросс-референс 203 типов карты с wiki (190/203 совпадений)
- **Добавлено**: `04_diff_analysis/build_object_type_dictionary.py` — скрипт-генератор словаря из HTML wiki
- **Добавлено**: `examples/parsed_save_312_with_objects.json` — пример полного экспорта сейва 312.GM1

### v2.0 (2026-10-08)
- **Реструктурировано**: разрозненные файлы и `scripts/` объединены в 7 смысловых папок
- **Добавлена новая подсистема**: `03_object_mapping/` — маппинг 7009 объектов карты → координаты (с верификацией 100%)
- **Расширено**: `04_diff_analysis/` — 27 скриптов (включая 4 новых)
- **Добавлено**: `_obsolete/` — устаревшие дубликаты с объяснением

### v1.0 (2026-10-07)
- Первая версия архива `homm3_gm1_toolkit.zip` (400 KB)
- 19 дифф-анализов, 104 поля, базовая структура с `scripts/` и `disasm/`

