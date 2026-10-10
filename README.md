# HoMM3 GM1 Toolkit

> Реверс-инжиниринг формата `.GM1` сейвов Heroes of Might and Magic III (SoD / HotA).
> **Версия архива:** 3.8 (10 Октября 2026)

## Что внутри

Архив организован по 7 смысловым категориям:

| Папка | Что содержит | Главный файл(ы) |
|-------|--------------|------------------|
| **`01_tools/`** | Готовые инструменты для работы с сейвами | `gm1_parser.py` (GUI), `map_json_loader.py` (Фаза 1), `map_config_builder.py` (Фаза 2), `cluster_finder.py`, `header_parser.py`, `save_layout.py`, `save_parser.py` (Фаза 3), `tile_scanner.py` (сканер тайлов), **`post_tile_scanner.py`** (offset-walker), **`post_tile_parser.py`** (23 content-парсера ProspectorRT), `gm1_diff.py`, `gm1_diff_gui.py` |
| **`02_format_docs/`** | Документация по формату `.GM1` | `GM1_format_compendium.md`, `gm1_mapping.json`, `header_pointer_search.md`, `diff_interpretation.json`, `unimplemented_algorithms_audit.md` (статус 23 парсеров) |
| **`03_object_mapping/`** | Универсальный словарь типов объектов (2037 типов из LazyLlama wiki) | `object_types_dictionary.json` (единственный файл) |
| **`04_diff_analysis/`** | Скрипты дифференциального анализа сейвов (исторические, разовые) | `build_object_type_dictionary.py` (универсальный), `Myth and Legend/` (подпапка для карты) |
| **`05_disasm/`** | Дизассемблированный `heroes3.exe` (нативный код) | `func_*.asm`, `save_functions_disasm.txt`, `all_strings.txt` |
| **`06_disasm_scripts/`** | Скрипты, создавших `05_disasm/` | `disasm_h3.py`, `find_*.py`, `callgraph_analysis.py` |
| **`07_prt_decompiled/`** | ⭐ Полный реверс-код ProspectorRT.exe (ILSpy C# + raw IL + metadata) | `ProspectorRT_source/ProspectorRT/MainForm.cs` (16K строк scanner), `ProspectorRT_IL/full_il_dump.txt` (4.6 МБ, 2 775 методов), `ProspectorRT_metadata/*` |

## Цель проекта

Создать **универсальный** редактор сейвов `.GM1`, работающий с любой картой, без хардкода абсолютных смещений и без предрассчитанных таблиц объектов для конкретной карты.

## Трёхфазный алгоритм (текущая архитектура)

1. **Фаза 1 — Map JSON** (`map_json_loader.py`): загружает JSON-парсинг карты `.h3m` (или `.zip` с ним), строит `MapData` (`objects_by_coord`, `coord_int_lookup`, `type_index`, `category_index`) для ЛЮБОЙ карты.
2. **Фаза 2 — Day-0 anchor** (`map_config_builder.py`): сравнивает `MapData` с сейвом нулевого дня, находит динамические смещения всех секций (`main object array`, `visiting array`, `hero blocks`, `town records`, `decoration bitmask`, ...). Сохраняет результат в `map_config_<mapname>.json`.
3. **Фаза 3 — Any save** (`save_parser.py` + `post_tile_parser.py`): открывает любой сейв той же карты, **адаптирует** смещения из config (hero/town блоки и кластеры объектов могут сдвигаться между сейвами из-за роста path-block/replay log), парсит все поля героя/города/объекта через `field_offsets` (формат-константы из `save_layout.py`), и декодирует содержимое **всех post-tile секций** ProspectorRT (EventBox, ArtRes, Monstr, SeerHut, PassGuard, Bank, Garrison, Univer, Market, Alliance, ArtMerchants, Timed Events, TownSpell, ArtDollPlace, PrisonHero, SubTerGate, MonolithWhirlpool) через `post_tile_parser.py` — без хардкода смещений.

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

## Текущая реализация post-tile парсеров (v3.8)

`01_tools/post_tile_parser.py` (1,779 строк) реализует **все 23 алгоритма ProspectorRT**:
- **A2-A10** Content-парсеры: EventBox, ArtRes, Monstr, SeerHut (10 mission + 10 reward types), PassGuard, Bank, Garrison, Univer, Market
- **B1-B3** Alliance, ArtMerchants, Experience aggregator
- **C1-C8** Map + Town Timed Events с signed-resource encoding
- **D1** GetTownSpell (Magic Guild spells)
- **E1-E3** ArtDollPlace (раскладка артефактов по слотам)
- **F1** GetPrisonHero (связка prison → hero record)
- **G1-G2** GetPairSubterraneanGate + ScanMonolithWhirlpool
- **H1-H5** Header offsets (BlackMarket, SR, Teams, MapName, Start)
- **I1-I5** Aggregators (GetAllSpell, GetAllSkill, AnalysisMonstrContent, SeerHutContent2, PassGuardContent2)

Тестирование на Myth and Legend (114.GM1): 99 EventBox, 10 ArtRes, 34 Monstr, 1 SeerHut (с русской миссией), 12 Banks, 8 Map Timed Events, 167 Town Timed Events, 8+8 групп монолитов, 12 водоворотов, 4 пары подземных врат.

## Известные ограничения текущей версии

- `gm1_mapping.json` теперь содержит только универсальные формат-константы (`constants`, `path_block`, `field_offsets`, `known_unknowns`). Все absolute offsets удалены в v3.0 (Шаг 3b) — они вычисляются динамически через `map_config_builder.py`.
- `04_diff_analysis/Myth and Legend/` — исторические разовые скрипты, использовавшиеся для ручного локализования полей. Не используются в runtime. Пути к сейвам в них захардкожены (сейвы не в репозитории).
- Write-back (редактирование сейва) — НЕ реализовано. Чтение полностью готово, запись в планах (Шаг 4).
- Day-0 сейвы: расширенная цепочка после CurrentState (BitField, Monolith, SubTerGate, Bank) может отсутствовать/обрываться (файл укорочен). Обрабатывается gracefully через try/except.

## Подробная документация

См. **`FILES_DESCRIPTION.md`** — полное описание всех файлов.
См. **`02_format_docs/unimplemented_algorithms_audit.md`** — аудит 23 алгоритмов ProspectorRT (все ✅ реализованы).
