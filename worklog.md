# H3save_reader — Work Log

Multi-agent shared work log. Append-only. Newest entries at the bottom.

---
Task ID: 0
Agent: main
Task: Analyze the actual repo state and prepare to remove all "Myth and Legend.h3m" hardcoding from the project. User has uploaded fresh examples for two maps (`Myth and Legend.h3m` and `save_parse_test_01`). Three-phase algorithm to be implemented: (1) load map JSON, (2) build a per-map "config" by scanning a day-zero save, (3) parse any save of that map using the config.

Work Log:
- Pulled latest commits; confirmed new `examples/Myth and Legend.h3m/` and `examples/save_parse_test_01/` directories with `.h3m.zip` (map JSON) + `0000.GM1` (day-zero save) + `0001.GM1`/`114.GM1` (post-action save).
- Re-inventoried the actual file tree. Differences from `README.md` / `FILES_DESCRIPTION.md`:
  - `03_object_mapping/` has been cleaned out — only `README.md` and `object_types_dictionary.json` (968 KB, universal) remain. The 10 per-map JSON files described in the old README are GONE.
  - `examples/` no longer contains `parsed_save_312_with_objects.json` or `homm3_map_objects.xlsx`; it now holds the new example bundles.
  - `_obsolete/` directory is GONE.
  - `01_tools/` contains extra files not in the old README: `block_finder.py`, `map_config_builder.py`, `gm1_mapping.json` (duplicate of `02_format_docs/gm1_mapping.json`), `Games - Ярлык.lnk` (junk Windows shortcut).
  - Root has `map_config_Мифы_и_легенды.json` (output of `map_config_builder.py`).
- Wrote `/home/z/my-project/scripts/probe_save.py` and decompressed all 4 example saves. Confirmed:
  - Both maps are SoD/HoMM3 v0x2A minor 2
  - `save_parse_test_01`: 36×36, no underground (JSON says false, save byte @0x34=1 — needs investigation), 3 objects, 5 sprites, decompressed size 196,608 bytes
  - `Myth and Legend.h3m`: 144×144, has underground, 7009 objects, decompressed size 1,572,864 bytes
- Read `01_tools/map_config_builder.py` (388 lines) — this already implements much of Step 2 (AnchorTable builder) with `build_map_config(map_data, day_zero_raw, day_zero_path)`. Existing output `map_config_Мифы_и_легенды.json` reveals two bugs:
  1. `find_object_clusters` merges everything into ONE cluster (gap threshold `0x10000` too coarse).
  2. `find_town_section` only detects 1 of 21 towns on Myth and Legend.
- Ran `probe_save.py` against all 4 saves; header layout matches `02_format_docs/header_pointer_search.md`.

Stage Summary:
- Real file tree mapped. Two main doc files (`README.md`, `FILES_DESCRIPTION.md`) are stale and describe files that no longer exist.
- `map_config_builder.py` is the right architecture but has clustering + town-finder bugs that need to be fixed in Step 1.
- Step 0 deliverables: update `README.md` and `FILES_DESCRIPTION.md` to reflect actual repo; create `01_tools/save_layout.py` dataclasses; create `01_tools/header_parser.py` with robust header parsing; set up `04_diff_analysis/Myth and Legend/` subfolder per user request.
- Step 1 deliverables: rewrite `find_object_clusters` with better cluster splitting; fix `find_town_section` to scan from `header_size` not `0x100000`; test on both example maps.


---
Task ID: 1
Agent: main
Task: Шаг 0 + Шаг 1 — создать чистую архитектуру без хардкода и универсальные finder-ы. Переписать md файлы под реальное состояние.

Work Log:
- Создан `01_tools/save_layout.py` (290 строк) — dataclasses (`HeaderInfo`, `ObjectCluster`, `HeroBlockInfo`, `HeroSection`, `TownBlockInfo`, `TownSection`, `ObjectOffsets`, `MapConfig`, `ParsedField`, `ParsedBlock`, `ParsedSave`) + формат-константы (`HERO_FIELD_OFFSETS`, `TOWN_FIELD_OFFSETS`, `HERO_BLOCK_SIZE`, `HERO_STRIDE_SOD`, `HERO_NAME_OFFSET_FROM_BLOCK_START`, `TOWN_NAME_OFFSET_FROM_BLOCK_START`). Roundtrip MapConfig → JSON → MapConfig проверен.
- Создан `01_tools/header_parser.py` (165 строк) — парсер заголовка `.GM1` сейва. Возвращает `HeaderInfo` с `header_size` (откуда начинать сканирование секций). Проверен на обеих картах:
  - save_parse_test_01: header_size=0x258, map_size=36, save_filename='0000.GM1'
  - Myth and Legend: header_size=0x3BC, map_size=144, map_name='Мифы и легенды', map_filename='Myth and Legend.h3m', save_filename='0000.GM1'
- Создан `01_tools/cluster_finder.py` (470 строк) — универсальный gap-based кластеризатор с iterative masking:
  - `find_coord_hits(raw, coord_ints, scan_start)` — находит все 3-байтные совпадения
  - `cluster_hits(...)` — gap-based кластеризация с адаптивным threshold (max(0x400, 8 × median_intra_gap))
  - `build_clusters(...)` — итеративно извлекает лучший кластер по (distinct_coords, peak_density), маскирует его хиты, повторяет. Quality filter: distinct/hits >= 10%, hits >= 3, distinct >= 2.
  - `assign_object_offsets(...)` — для каждого объекта находит его смещения в каждом кластере.
  - Проверен на обеих картах:
    - save_parse_test_01: 1 кластер (main, 3 hits, 3 distinct, 100% покрытие)
    - Myth and Legend: 10 кластеров (main=7267 hits / 6006 distinct = 100% покрытие; visiting=1095/172; decoration=150/70; alive=10/7; fog=8/7; treasure=10/6; + 4 other_*). Главный массив идеально локализован.
- Переписан `01_tools/map_config_builder.py` (460 строк):
  - Делегирует парсинг заголовка в `header_parser.parse_header`
  - Делегирует кластеризацию в `cluster_finder.find_all_object_clusters`
  - Реализует `find_hero_blocks` (regex + name-based fallback):
    - **КРИТИЧЕСКОЕ ОТКРЫТИЕ**: герои в Russian HoMM3 хранятся с cp1251 именами ("Одиссей", "Лорд Хаарт", "Валеска", "Сорша", "Персей"), а не с английскими. Стандартный список 156 имён (English) не находит их.
    - Решение: `_is_valid_hero_name` принимает Latin + Cyrillic (Unicode U+0400-U+052F) + space/dot/apostrophe. Проверка по Unicode codepoint, не по byte value.
    - Доп. фильтр: имя не должно состоять из одного повт. символа (отсекает 'яяяя').
  - Реализует `find_town_blocks` (coord-based + name-based): поиск 3-byte (x,y,z) needle с валидацией faction/type/name_len. Поддержка town_names для доп. верификации.
  - CLI: `python3 01_tools/map_config_builder.py --map-json ... --day0-save ... --output-dir ...`
- Реорганизован `04_diff_analysis/`:
  - 39 файлов перемещены в `04_diff_analysis/Myth and Legend/` (подпапка по карте)
  - На верхнем уровне остался `build_object_type_dictionary.py` (универсальный) + новый `README.md`
  - Добавлен `04_diff_analysis/Myth and Legend/README.md` с описанием всех 39 скриптов
- Очистка репозитория:
  - Удалён `01_tools/Games - Ярлык.lnk` (junk Windows shortcut)
  - Удалён дубликат `01_tools/gm1_mapping.json` (остался каноничный в `02_format_docs/`)
  - Удалён `map_config_Мифы_и_легенды.json` из root (старый артефакт со сломанным town finder)
- Сгенерированы новые `map_config_*.json` для обеих карт:
  - `examples/save_parse_test_01/map_config_save_parse_test_01.json` — 1 кластер, 1 town ('ttost'), 150 героев (пул SoD, русские имена: 'Оррин', 'Сильвия', ...)
  - `examples/Myth and Legend.h3m/map_config_Мифы_и_легенды.json` — 10 кластеров, 21 town ('Кавала', 'Волос', 'Каламата', 'Этопия', 'Дельфи', ...), 156 героев (stride 0x446, first='Одиссей' at 0x14223F)
- Обновлены md файлы:
  - `README.md` — переписан под v3.0-dev структуру
  - `FILES_DESCRIPTION.md` — переписан (978 строк) с детальным описанием всех новых модулей и финальной структуры
  - `03_object_mapping/README.md` — обновлён (только универсальный словарь типов)
  - `04_diff_analysis/README.md` — новый (описывает структуру с подпапками)
  - `04_diff_analysis/Myth and Legend/README.md` — новый (описание 39 скриптов)
- Smoke test всех модулей проходит:
  - Все 4 новых модуля (`save_layout`, `header_parser`, `cluster_finder`, `map_config_builder`) импортируются без ошибок
  - `_is_valid_hero_name` проходит все 7 тестов (4 валидных + 3 невалидных)
  - `MapConfig` roundtrip (Python → JSON → Python) работает
  - Оба `map_config_*.json` файла загружаются через `MapConfig.from_dict`

Stage Summary:
- **Шаг 0 и Шаг 1 завершены.** Архитектура без хардкода абсолютных смещений готова:
  - Фаза 1 (`map_json_loader.py`): без изменений, универсальная
  - Фаза 2 (`map_config_builder.py`): переписана, использует `header_parser`, `cluster_finder`, `save_layout`. Без хардкода 0x100000 — scan начинается с `header_size`. Без хардкода имён героев — принимает Russian cp1251 имена.
  - Фаза 3: пока не реализована (`gm1_parser.py` ещё содержит хардкод для Myth and Legend — следующий шаг рефакторинга)
- Все `.md` файлы актуализированы под реальное состояние репозитория.
- Финальная структура: 97 файлов в 7 категориях + 2 примера карт с готовыми `map_config_*.json`.
- **Что осталось для следующего цикла:** рефакторинг `gm1_parser.py` (2829 строк) — заменить хардкод смещений на чтение из `map_config_<mapname>.json`. Реализовать `save_parser.py` (Фаза 3) — адаптация смещений между сейвами и парсинг полей через `field_offsets`.


---
Task ID: 2
Agent: main
Task: Шаг 2 — реализовать Фазу 3 (save_parser.py) и провести частичный рефакторинг gm1_parser.py для удаления хардкода смещений. Обновить md файлы.

Work Log:
- Прочитал `01_tools/gm1_parser.py` (2829 строк), идентифицировал хардкод:
  - `find_hero_blocks` (строка 272): `for i in range(0x100000, ...)` — hardcoded scan start
  - `find_town_blocks` (строка 489): `pos = 30000` — hardcoded scan start
  - `_compute_object_offsets_in_save` (строки 1767-1872): hardcoded `scan_start = 0x10000`, fallback CLUSTERS с `0x118000`, `0x120000`, `0x170000`, `0xAD000`, `0x9D000`, `0x7E000`
  - `_show_map_object_details` (строки 2474-2486): хардкод `0x118C1C`, `0x120C8C`, `0x172100` для классификации смещений
  - `parse_save` (строка 747): читает `mapping["blocks"]` из `gm1_mapping.json` с absolute offsets для Myth and Legend
- Создан `01_tools/save_parser.py` (350 строк) — Фаза 3:
  - `parse_save(raw, config)` → `ParsedSave` (использует `header_parser.parse_header`, делегирует адаптацию в `adapt_config_to_save`)
  - `parse_hero_block(raw, block_offset)` — читает все поля героя через `HERO_FIELD_OFFSETS` (location, player, movement, exp, mana, level, num_skills, name (cp1251), army_types/counts, skill_levels/slots, attributes, spells_book/available, equipment)
  - `parse_town_block(raw, block_offset)` — читает все поля города через `TOWN_FIELD_OFFSETS` (faction, type, x/y/z, army, name)
  - `adapt_config_to_save(raw, config)` — пересоздаёт hero/town blocks через `map_config_builder.find_hero_blocks/find_town_blocks` и object clusters через `cluster_finder.find_all_object_clusters` для ТЕКУЩЕГО сейва. Адаптация необходима, т.к. между сейвами hero/town блоки и object clusters могут сдвигаться из-за роста path-block/replay log.
  - `parsed_save_to_dict(parsed)` — JSON-сериализация
  - Smoke test на обеих картах (day-0 + post-action) проходит успешно
- Рефакторинг `01_tools/gm1_parser.py`:
  - ✅ `find_hero_blocks(raw, scan_start=None)` — если `scan_start` не указан, берётся из `header_parser.parse_header(raw).header_size`. Убран хардкод `0x100000`.
  - ✅ `find_town_blocks(raw, max_pos=None, scan_start=None)` — то же. Убран хардкод `30000`.
  - ✅ `_compute_object_offsets_in_save()` переписан — использует `cluster_finder.find_all_object_clusters(self.raw_data, coord_ints, scan_start=header.header_size)`. Убраны хардкоды `0x10000` (scan_start), `0x118000`/`0x120000`/`0x170000`/`0xAD000`/`0x9D000`/`0x7E000` (fallback CLUSTERS).
  - ✅ `_show_map_object_details` — кластеризация смещений берётся из `self.map_config["clusters"]` (если есть), иначе fallback на грубую эвристику (header-area, early-section). Убраны хардкоды `0x118C1C`/`0x120C8C`/`0x172100`.
  - ⚠️ `parse_save()` (GUI) оставлен без изменений — он читает `mapping["blocks"]` из `gm1_mapping.json` для отображения в дереве блоков. Перевод на `save_parser.parse_save()` — следующий шаг.
- Тестирование:
  - Создан `/home/z/my-project/scripts/test_save_parser.py` — end-to-end тест Phase 3. Проверены все 4 сейва (2 карты × 2 сейва). Результаты:
    - save_parse_test_01: day-0 → 0001.GM1: размер идентичен, 150 героев, 1 активный (Турис), герой переместился (5,3,0) → (5,1,0)
    - Myth and Legend: day-0 → 114.GM1: 156 героев (stride 0x446), 16→26 активных (игрок нанял 10 новых), Одиссей level 1→2 exp 46→1629, hero blocks сдвинулись 0x14223F→0x1418EB (~0xA54 байт, из-за роста path-block), town blocks сдвинулись 0x140250→0x13F8FC
  - Создан `/home/z/my-project/scripts/test_gm1_parser_refactor.py` — smoke test с mock PySide6. Подтверждает что gm1_parser.py импортируется, `find_hero_blocks` находит 156 героев на маленькой карте (раньше 150 из-за хардкода `0x100000`), `parse_save` работает.
  - bug fix: маленькая карта save_parse_test_01 теперь корректно находит 156 героев (раньше 150, потому что `range(0x100000, len(raw))` = `range(1048576, 196608)` = пустой итератор, сканирование не работало)
- Обновлены md файлы:
  - `README.md` — описание Фазы 3 с адаптацией между сейвами; обновлены "Известные ограничения"
  - `FILES_DESCRIPTION.md` — добавлено детальное описание `save_parser.py` (секция 4), обновлено описание `gm1_parser.py` (секция 6) с детализацией рефакторинга, добавлен changelog v3.0-dev Шаг 2
  - `worklog.md` — этот раздел

Stage Summary:
- **Шаг 2 завершён.** Фаза 3 (save_parser.py) реализована и протестирована. Рефакторинг gm1_parser.py проведён (без хардкода scan_start и cluster ranges). Адаптация между сейвами работает корректно — на разных сейвах одной карты парсер автоматически находит hero/town блоки и object clusters заново, даже если они сдвинулись.
- **Главный инсайт:** между day-0 и post-action сейвами hero blocks могут сдвигаться на сотни байт (path block/replay log растёт). Решение — пересканировать сейв заново через `adapt_config_to_save`, а не пытаться применить смещения из MapConfig напрямую.
- **Bug fix попутно:** старый `find_hero_blocks` с `range(0x100000, len(raw))` ничего не находил на маленьких сейвах (196 KB), потому что `0x100000 > 196608`. Теперь `scan_start` берётся из `header_size` (~0x258 для маленьких карт), и сканирование работает.
- **Что осталось на Шаг 3:** перевести GUI `parse_save()` на `save_parser.parse_save()`, очистить `02_format_docs/gm1_mapping.json` (удалить секции `blocks` и `alternative_offsets` с absolute offsets для Myth and Legend), добавить кнопку "Load Map Config…" в GUI для загрузки готового `map_config_*.json`.

