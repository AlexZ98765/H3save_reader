# HoMM3 GM1 Toolkit

> Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD / HotA).
> **Версия архива:** 3.0-dev (work in progress — universal parsing without hardcoded offsets).

## Что внутри

Архив организован по 6 смысловым категориям:

| Папка | Что содержит | Главный файл(ы) |
|-------|--------------|------------------|
| **`01_tools/`** | Готовые инструменты для работы с сейвами | `gm1_parser.py` (GUI), `map_json_loader.py` (Фаза 1), `map_config_builder.py` (Фаза 2), `cluster_finder.py`, `header_parser.py`, `save_layout.py`, `save_parser.py` (Фаза 3), `block_finder.py`, `gm1_diff.py`, `gm1_diff_gui.py` |
| **`02_format_docs/`** | Документация по формату `.GM1` | `GM1_format_compendium.md`, `gm1_mapping.json`, `header_pointer_search.md`, `diff_interpretation.json` |
| **`03_object_mapping/`** | Универсальный словарь типов объектов (2037 типов из LazyLlama wiki) | `object_types_dictionary.json` (единственный файл) |
| **`04_diff_analysis/`** | Скрипты дифференциального анализа сейвов (исторические, разовые) | `build_object_type_dictionary.py` (универсальный), `Myth and Legend/` (подпапка для карты) |
| **`05_disasm/`** | Дизассемблированный `heroes3.exe` | `func_*.asm`, `save_functions_disasm.txt`, `all_strings.txt` |
| **`06_disasm_scripts/`** | Скрипты, создавших `05_disasm/` | `disasm_h3.py`, `find_*.py`, `callgraph_analysis.py` |

## Цель проекта

Создать **универсальный** редактор сейвов `.GM1`, работающий с любой картой, без хардкода абсолютных смещений и без предрассчитанных таблиц объектов для конкретной карты.

## Трёхфазный алгоритм (текущая архитектура)

1. **Фаза 1 — Map JSON** (`map_json_loader.py`): загружает JSON-парсинг карты `.h3m` (или `.zip` с ним), строит `MapData` (`objects_by_coord`, `coord_int_lookup`, `type_index`, `category_index`) для ЛЮБОЙ карты.
2. **Фаза 2 — Day-0 anchor** (`map_config_builder.py`): сравнивает `MapData` с сейвом нулевого дня, находит динамические смещения всех секций (`main object array`, `visiting array`, `hero blocks`, `town records`, `decoration bitmask`, ...). Сохраняет результат в `map_config_<mapname>.json`.
3. **Фаза 3 — Any save** (`save_parser.py`): открывает любой сейв той же карты, **адаптирует** смещения из config (hero/town блоки и кластеры объектов могут сдвигаться между сейвами из-за роста path-block/replay log) и распарсивает все поля героя/города/объекта через `field_offsets` (формат-константы из `save_layout.py`). GUI `gm1_parser.py` теперь использует `cluster_finder` напрямую — без хардкода `0x100000`, `0x118000`, `0x120000`, `0x170000`.

## С чего начать

1. **Прочитать** `FILES_DESCRIPTION.md` — подробное описание всех файлов.
2. **Прочитать** `02_format_docs/header_pointer_search.md` — стратегия универсального поиска секций.
3. **Построить per-map config** для своей карты:
   ```bash
   python3 01_tools/map_config_builder.py \
     --map-json /path/to/MyMap.h3m.zip \
     --day0-save /path/to/0000.GM1 \
     --output-dir /tmp/
   ```
4. **Запустить GUI парсер** (требует доработки для использования config):
   ```bash
   python3 01_tools/gm1_parser.py
   ```

## Координатная кодировка

В сейве и в карте объекты адресуются 3 байтами:

```text
coord_int = x | (y << 8) | (z << 16)
```

где `z = 0` — surface, `z = 1` — underground. Это позволяет соотносить объекты карты с байт-паттернами в сейве.

**Важно:** SAVE координаты могут отличаться от MAP координат на +2 (towns занимают 2×2 тайла, сейв использует top-left corner). См. `02_format_docs/GM1_format_compendium.md` для деталей.

## Известные ограничения текущей версии

- `gm1_parser.py` (GUI) — `parse_save()` всё ещё читает блоки из `gm1_mapping.json` (секция `blocks` с absolute offsets для Myth and Legend). Это используется для отображения в дереве; для программного парсинга используйте `save_parser.parse_save(raw, config)`. Очистка `gm1_mapping.json` и перевод GUI на `save_parser` — следующий шаг.
- `04_diff_analysis/Myth and Legend/` — исторические разовые скрипты, использовавшиеся для ручного локализования полей. Не используются в runtime. Пути к сейвам в них захардкожены (сейвы не в репозитории).

## Подробная документация

См. **`FILES_DESCRIPTION.md`** — полное описание всех файлов.
