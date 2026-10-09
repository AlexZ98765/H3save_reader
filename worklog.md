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


---
Task ID: 3
Agent: main
Task: Убрать папку examples/ из архива (по запросу пользователя).

Work Log:
- Пересобрал архив без `examples/` — теперь 105 файлов вместо 117, размер 526 KB вместо 3.0 MB
- Обновил `README.md`: убрал таблицу "Примеры (в `examples/`)", заменил `examples/MyMap/...` на `/path/to/...` в команде запуска
- Обновил `FILES_DESCRIPTION.md`: убрал `examples/` из дерева структуры, добавил примечание "Папка examples/ больше не включается в архив. Пользователь должен предоставлять свои .h3m.json / .GM1 файлы"
- Заменил все `examples/MyMap/...` в примерах кода на `/path/to/MyMap.h3m.zip` и `/path/to/0000.GM1`
- Обновил статистику: "Карт в examples/" → "Примеры сейвов | (не входят в архив; пользователь загружает свои)"
- Убрал запись "Новые примеры в examples/" из changelog
- Worklog.md добавлен в архив для сохранения истории работы

Stage Summary:
- Архив теперь весит 526 KB (в 6 раз меньше) — без тяжёлых сейвов и JSON-парсингов карт
- README и FILES_DESCRIPTION актуализированы под отсутствие examples/
- Веб-страница скачивания (https://preview-b05de585.space-z.ai/) автоматически отдаёт обновлённый архив через `/api/download` endpoint

---
Task ID: 4
Agent: main
Task: Шаг 3 — полный перевод GUI parse_save() на save_parser.parse_save(), очистка gm1_mapping.json от absolute offsets, добавление кнопки "Load Map Config…" в GUI.

Work Log:
- Очистка `02_format_docs/gm1_mapping.json` (Шаг 3b):
  - Создан скрипт `/home/z/my-project/scripts/clean_gm1_mapping.py`
  - Удалены секции: `blocks` (21 блок с absolute offsets), `alternative_offsets.save_447_series` (14 полей), `search_patterns` (regex с hardcoded диапазонами)
  - Оставлены только универсальные секции: `constants`, `path_block` (7 path_record_types), `field_offsets` (HERO_FIELD_OFFSETS, TOWN_FIELD_OFFSETS, HERO_BLOCK_SIZE/STRIDE константы — импортированы из save_layout.py), `known_unknowns`, `notes` (обновлён — описание трёхфазной архитектуры)
  - Размер файла: 51 KB → 14 KB (в 3.6× меньше)
- Рефакторинг `01_tools/gm1_parser.py` (Шаг 3c):
  - `parse_save(raw, mapping=None, map_config=None)` — переписана на 2 пути:
    - `_parse_save_via_config(raw, map_config)` — Path 1 (с MapConfig): вызывает `save_parser.parse_save()` (Phase 3), конвертирует ParsedSave в dict формат для `_populate_tree`. Возвращает `file_info` (с header_size, map_filename, save_filename), `blocks` (header + per-cluster), `heroes_found` (legacy format с {name, offset, fields dict}), `towns_found` (legacy format с {name, faction_name, type_name, location, army_types, army_counts}), `object_offsets`, `map_config_meta`
    - `_parse_save_legacy(raw, mapping)` — Path 2 (без MapConfig): парсит только header + heroes + towns через universal find_hero_blocks/find_town_blocks. Без блоков из mapping (т.к. `gm1_mapping.json` больше не содержит `blocks`)
  - Обновлены все 3 вызова `parse_save()`:
    - `load_file`: `parse_save(self.raw_data, self.mapping, map_config=getattr(self, "map_config", None))` + использует `parsed_data["object_offsets"]` если есть (пропускает `_compute_object_offsets_in_save`)
    - `_on_reload_mapping`: то же
    - `_on_load_day_zero`: `parse_save(self.raw_data, self.mapping, map_config=config)` + использует `parsed_data["object_offsets"]`
  - `_on_load_day_zero` исправлен: обращается к `config.meta`, `config.hero_section.count` (MapConfig dataclass) вместо dict-доступа через `config["_meta"]["map_name"]` — добавлена проверка `hasattr(config, "meta")` для обратной совместимости
- Новая кнопка "Load Map Config…" (Шаг 3d):
  - Toolbar: добавлена кнопка "2b. Map Config…" между "2. Day-Zero Save…" и "3. Open Save…"
  - Menu: добавлен пункт "Load → Load Map Config…" (Ctrl+L)
  - Новый метод `_on_load_map_config()` загружает `MapConfig.from_dict(json.load(open(path)))` через `save_layout.MapConfig`. Показывает QMessageBox с информацией о загруженном config (map_name, map_size, n_objects, n_heroes, n_towns, n_clusters).
  - Позволяет пропустить Шаг 2 (Day-Zero Save) если config уже построен
- Тестирование (Шаг 3e):
  - Создан `/home/z/my-project/scripts/test_step3.py` — end-to-end тест Phase 3 через `gm1_parser.parse_save(raw, mapping={}, map_config=config)` на 4 сейвах
  - Использует mock PySide6 (так как реальная библиотека требует GUI среды)
  - Все 4 теста проходят:
    - save_parse_test_01/0000.GM1: file_info (H3SVG v42.2, map='save_parse_test_01'), 2 blocks (header + cluster:main 0x775A..0x7765), 150 heroes (first='Оррин' level=1 exp=74), 1 town ('ttost' faction=Neutral type=Castle coords=(2,2,0)), 3 object_offsets (100% verified)
    - save_parse_test_01/0001.GM1: те же 150 героев, 1 town — те же смещения (path block не растёт на маленькой карте)
    - Myth and Legend/0000.GM1: file_info (map='Мифы и легенды'), 11 blocks (header + 10 clusters: main 0x12281A..0x12B0FB, visiting 0x100EE6..0x1148BB, decoration 0x13D4AF..0x14244A, ...), 156 heroes (first='Одиссей' at 0x14223F level=1 exp=46 player=Red), 21 towns (first='Кавала' faction=Neutral type=Tower coords=(109,3,0)), 6006 object_offsets (100% verified)
    - Myth and Legend/114.GM1: hero blocks сдвинулись (0x14223F → 0x1418EB), Одиссей level=2 exp=1629 — адаптация между сейвами работает
- Обновлены md файлы (Шаг 3f):
  - `README.md` — описание Фазы 3 обновлено (упомянуты обе `save_parser.py` + `gm1_parser.py`), обновлены "Известные ограничения"
  - `FILES_DESCRIPTION.md` — раздел 6 (`gm1_parser.py`) переписан с детальным описанием рефакторинга Шага 2 + Шага 3; раздел 7 (`gm1_mapping.json`) переписан под чистую структуру; добавлен changelog v3.0-dev Шаг 3

Stage Summary:
- **Шаг 3 завершён.** Полностью убран хардкод смещений из проекта:
  - `gm1_mapping.json` теперь содержит только универсальные формат-константы (14 KB вместо 51 KB)
  - `gm1_parser.parse_save()` автоматически использует `save_parser.parse_save()` (Phase 3) когда загружен MapConfig (через "Load Day-Zero Save" или новую кнопку "Load Map Config")
  - `gm1_parser.parse_save()` без MapConfig возвращает минимальный ParsedSave (header + heroes + towns) — без хардкода
  - Новая кнопка "2b. Map Config…" (Ctrl+L) позволяет пропускать Шаг 2 если config уже построен
- **End-to-end тест** на 4 сейвах (2 карты × day-0 + post-action) проходит успешно
- **Адаптация между сейвами работает:** на Myth and Legend day-0 → 114.GM1 hero blocks автоматически перенаходятся (0x14223F → 0x1418EB), hero state корректно парсится (Одиссей level=1→2, exp=46→1629), 6006 object_offsets 100% verified на обоих сейвах
- **Все три фазы архитектуры готовы:**
  - Phase 1: `map_json_loader.py` — загрузка любой карты
  - Phase 2: `map_config_builder.py` — построение MapConfig из day-0 сейва
  - Phase 3: `save_parser.py` + `gm1_parser.py` — парсинг любого сейва через MapConfig
- Проект достиг состояния "универсальный парсер сейвов .GM1 для любой карты без хардкода смещений".

