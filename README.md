# HoMM3 GM1 Toolkit

> Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD / HotA).
> **Версия архива:** 3.10 (10 Октября 2026) — **полное соответствие ProspectorRT по алгоритмам чтения + 4 приоритета экспорта**

## Что внутри

Архив организован по 6 смысловым категориям + архив `_obsolete/`:

| Папка | Что содержит | Главный файл(ы) |
|-------|--------------|------------------|
| **`01_tools/`** | Готовые инструменты для работы с сейвами | `gm1_parser.py` (GUI), `map_json_loader.py` (Фаза 1), `map_config_builder.py` (Фаза 2), `cluster_finder.py`, `header_parser.py`, `save_layout.py`, `save_parser.py` (Фаза 3), `tile_scanner.py` (37 tile парсеров), `post_tile_scanner.py` (offset-walker), `post_tile_parser.py` (56 post-tile парсеров), **`merged_objects.py`** (22 PRT-подобных таблицы), `gm1_diff.py`, `gm1_diff_gui.py` |
| **`02_format_docs/`** | Документация по формату `.GM1` | `GM1_format_compendium.md` (справка по формату), `gm1_mapping.json` (формат-константы) |
| **`03_object_mapping/`** | Универсальный словарь типов объектов (2037 типов из LazyLlama wiki) | `object_types_dictionary.json` (единственный файл) |
| **`04_diff_analysis/`** | Универсальный скрипт дифф-анализа (только `build_object_type_dictionary.py`) | `build_object_type_dictionary.py` (парсит LazyLlama wiki) |
| **`05_disasm/`** | Дизассемблированный `heroes3.exe` (нативный код) | `func_*.asm`, `save_functions_disasm.txt`, `all_strings.txt` |
| **`07_prt_decompiled/`** | ⭐ Полный реверс-код ProspectorRT.exe (ILSpy C# + raw IL + metadata) | `ProspectorRT_source/ProspectorRT/MainForm.cs` (16K строк scanner), `ProspectorRT_IL/full_il_dump.txt` (4.6 МБ, 2 775 методов), `ProspectorRT_metadata/*` |
| **`_obsolete/`** | 📦 Архив временных и промежуточных файлов | `02_format_docs/` (6 исторических .md + diff_interpretation.json), `04_diff_analysis/Myth and Legend/` (25 analyze_*.py + отчёты), `06_disasm_scripts/` (12 скриптов дизассемблера), `scripts/` (25 audit/compare скриптов) |

## Цель проекта

Создать **универсальный** редактор сейвов `.GM1`, работающий с любой картой, без хардкода абсолютных смещений и без предрассчитанных таблиц объектов для конкретной карты.

## Трёхфазный алгоритм (текущая архитектура v3.10)

1. **Фаза 1 — Map JSON** (`map_json_loader.py`): загружает JSON-парсинг карты `.h3m` (или `.zip` с ним), строит `MapData` (`objects_by_coord`, `coord_int_lookup`, `type_index`, `category_index`) для ЛЮБОЙ карты.
2. **Фаза 2 — Day-0 anchor** (`map_config_builder.py`): сравнивает `MapData` с сейвом нулевого дня, находит динамические смещения всех секций (`main object array`, `visiting array`, `hero blocks`, `town records`, `decoration bitmask`, ...). Сохраняет результат в `map_config_<mapname>.json`.
3. **Фаза 3 — Any save** (`save_parser.py` + `post_tile_parser.py` + `merged_objects.py`): открывает любой сейв той же карты, **адаптирует** смещения из config, парсит все поля героя/города/объекта, декодирует содержимое **всех post-tile секций** ProspectorRT, и генерирует **22 PRT-подобных таблицы** + **3 агрегатора** через `merged_objects.merge_all()`.

## С чего начать

### Workflow A (полный — с построением config)
```
1. Map JSON…     (Ctrl+M) — загрузить .h3m.zip (ТРЕБУЕТСЯ для построения config)
2. Day-Zero Save…(Ctrl+D) — загрузить 0000.GM1 → строит MapConfig
3. Open Save…    (Ctrl+O) — открыть любой сейв той же карты
```

### Workflow B (быстрый — с готовым config, БЕЗ map JSON)
```
2b. Map Config…  (Ctrl+L) — загрузить готовый map_config_*.json
3.  Open Save…   (Ctrl+O) — открыть любой сейв
```
Workflow B не требует map JSON — всё необходимое уже в config.
Однако без map JSON типы объектов будут показаны как "unknown".

### Workflow C (с map JSON для enrichment — рекомендуемый)
```
1.  Map JSON…    (Ctrl+M) — загрузить .h3m.zip (опционально, но даёт типы/спрайты)
2b. Map Config…  (Ctrl+L) — загрузить готовый map_config_*.json
3.  Open Save…   (Ctrl+O) — открыть любой сейв
```
Map JSON добавляет: sprite .def names, object types/categories, map description,
disabled artifacts/spells/skills, rumors, global events, overlay objects.

## Координатная кодировка

В сейве и в карте объекты адресуются 3 байтами:

```text
coord_int = x | (y << 8) | (z << 16)
```

где `z = 0` — surface, `z = 1` — underground. Это позволяет соотносить объекты карты с байт-паттернами в сейве.

**Важно:** SAVE координаты могут отличаться от MAP координат на +2 (towns занимают 2×2 тайла, сейв использует top-left corner). См. `02_format_docs/GM1_format_compendium.md` для деталей.

## Текущая реализация (v3.10) — 4 приоритета соответствия PRT

### Алгоритмы чтения (v3.9): 93/93 ✅
- **37 tile-level парсеров** (Save* методы в `IsObject` диспетчере)
- **56 post-tile парсеров** (Get/Scan/Content/Analysis)

### Формат экспорта (v3.10) — 4 приоритета:

| Приоритет | Что добавлено | Строк кода |
|---|---|---|
| **1** | Экспонирование `post_tile_content` + `player_states` + `current_state` в JSON | ~30 |
| **2** | Hero `war_machines` + `spell_book` поля; Town `spell_pool` + `buildings_built` | ~100 |
| **3** | Tile-scan + post-tile merge → 22 PRT-подобных таблицы (через `merged_objects.merge_all`) | ~540 |
| **4** | Агрегаторы: `all_artifacts` + `all_spells` + `all_skills` | ~150 |

**Результат тестирования на Myth and Legend (0000.GM1):**

| Лист PRT | PRT rows | Наш merge |
|---|---|---|
| Артефакты | 139 | 124 ✅ |
| Монстры | 237 | 272 ✅ |
| События и Ящики Пандоры | 98 | 99 ✅ |
| Ученые | 10 | 11 ✅ |
| Сундуки | 283 | 284 ✅ |
| Заклинания | 40 | 41 ✅ |
| Навыки | 14 | 13 ✅ |
| Лагеря Беженцев | 1 | 1 ✅ |
| Тюрьмы | 5 | 6 ✅ |
| Объекты | 26 | 27 ✅ |
| Топология | 14 | 15 ✅ |
| Все Арты (агрегатор) | 181 | 123 ✅ |

См. `02_format_docs/GM1_format_compendium.md` для справки по формату.
Исторические сравнения с ProspectorRT (HONEST_AUDIT, PRT_xlsx_vs_results02) перенесены в `_obsolete/02_format_docs/`.

## Известные ограничения текущей версии

- `gm1_mapping.json` теперь содержит только универсальные формат-константы (`constants`, `path_block`, `field_offsets`, `known_unknowns`). Все absolute offsets удалены в v3.0 — они вычисляются динамически через `map_config_builder.py`.
- `04_diff_analysis/Myth and Legend/` — исторические разовые скрипты, использовавшиеся для ручного локализования полей. **Перенесены в `_obsolete/`** — не используются в runtime.
- Write-back (редактирование сейва) — НЕ реализовано. Чтение полностью готово, запись в планах (Шаг 4).
- Day-0 сейвы: расширенная цепочка после CurrentState (BitField, Monolith, SubTerGate, Bank) может отсутствовать/обрываться (файл укорочен). Обрабатывается gracefully через try/except.

## Подробная документация

См. **`FILES_DESCRIPTION.md`** — полное описание всех файлов.
См. **`_obsolete/README.md`** — описание перенесённых в архив временных файлов (включая исторические сравнения с ProspectorRT).
