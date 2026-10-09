# Myth and Legend — Diff Analysis Scripts

Эти скрипты — **исторические разовые исследования**, использовавшиеся
для ручного локализования 124+ полей формата `.GM1` на карте
"Myth and Legend.h3m" (144×144, 7009 объектов, 21 town, 156 heroes
в пуле). Результаты собраны в `02_format_docs/diff_interpretation.json`
и `02_format_docs/gm1_mapping.json`.

**Эти скрипты НЕ вызываются в runtime** и не нужны для работы парсера.
Они оставлены как исследовательский архив.

## Зависимости от хардкода

Эти скрипты содержат **захардкоженные пути** к сейвам серии 0011–0020_9
и 312/447 (которые не загружены в репозиторий — слишком большие).
Пути вида `/home/z/my-project/upload/...` и `KNOWN_OFFSETS` словари
жёстко прописаны для сейвов карты Myth and Legend.

## Что локализовано

### Серия 001 (старт игры → ключевые действия)

| Скрипт | Пара сейвов | Что локализовано |
|--------|-------------|------------------|
| `analyze_diff.py` | 001→0011 (движение героя) | Общий фреймворк, кластеризация изменений |
| `analyze_path.py` | — | Path-записи: типы `01 03` (движение), `0b 03` (fog of war) |
| `analyze_build_diff.py` | 0011→0012 (постройка здания) | `0x13F17B` (флаг постройки), `0x13E420` (ID здания) |
| `analyze_xp_diff.py` | 0012→0013 (опыт + уровень) | experience, level |
| `analyze_artifact_diff.py` | 0013→0014 (покупка артефакта) | Hero artifact inventory, Black Market |
| `analyze_hire_diff.py` | 0014→0015 (найм существ) | Town garrison, player gold |
| `analyze_transfer_diff.py` | 0015→0016 (передача армии) | Hero army, новый path-record `08 03` |
| `analyze_spellbook_diff.py` | 0016→0017 (книга магии) | has_spell_book, spell_bit_array, spell_book_slot |
| `analyze_path_dest.py` | 0017→0018 (изменение пути) | queued_path_destination |
| `analyze_ballista_diff.py` | 0018→0019 (покупка баллисты) | ballista/ammo_cart/tent/catapult slots |
| `analyze_stables_diff.py` | 0019→0020 (конюшня) | hero visit counter, visit flag |

### Серия 0020 (читы и сложные сценарии)

| Скрипт | Что локализовано |
|--------|------------------|
| `analyze_cheat_diff.py` | cheater_flag, current_movement_points |
| `analyze_mine_diff.py` | object_owner (захват шахты) |
| `analyze_fountain_diff.py` | visited_objects bitmask |
| `analyze_upgrade_diff.py` | map_object_visiting_coords |
| `analyze_disembark_diff.py` | hero_on_boat_flag, hero_surface_flag. Новый path-record `06 03` |
| `analyze_cheat2_diff.py` | alt movement_points, army_count_slot_alt |
| `analyze_battle_diff.py` | town_owner_color, town_garrison_army. Новый path-record `04 03` |
| `analyze_moves_diff.py` | hero visit_id swap, global turn counters |
| `analyze_battle2_diff.py` | enemy_hero_state, enemy_hero_defeated_block |
| `analyze_mercenary_diff.py` | Наёмник — детали |

### Серия 447 (army swap, artifact equip, Magic Guild upgrade)

| Скрипт | Пара сейвов | Что локализовано |
|--------|-------------|------------------|
| `analyze_447_diff.py` | 447_1→447_2 | Army swap (slot 0 ↔ slot 4), artifact equip, 7 полей |
| `analyze_447_2_to_3_diff.py` | 447_2→447_3 | Magic Guild Lv3 (level counter 2→3, building bitmask, 7 player resources deducted) |

**Отчёты:**
- `analyze_447_diff_report.{json,md}` — машинно- и человекочитаемый отчёт 447_1→447_2
- `analyze_447_2_to_3_diff_report.{json,md}` — то же для 447_2→447_3
- `analyze_hero_hire_diff_report.{json,md}` — отчёт найма героя

### Скрипты для маппинга объектов (для этой карты)

| Скрипт | Назначение |
|--------|------------|
| `build_object_mapping.py` | Построение `objects_by_coord.json` и индексов для Myth and Legend (имеет хардкод `/home/z/my-project/upload/Myth and Legend.h3m.json`) |
| `build_objects_flat.py` | Плоский список 7009 объектов из `objects_by_coord.json` |
| `build_objects_with_offsets.py` | Вычисление адресов объектов в сейве (main_offset, visiting_offset, ...) |
| `verify_coord_mapping.py` | Верификация маппинга против `312.GM1` (100% совпадение: 1751/1751) |
| `analyze_save_clusters.py` | Кластеризация смещений в сейве |
| `find_header_pointers.py` | Поиск указателей в заголовке (с хардкодом KNOWN_OFFSETS для 312/447) |
| `header_pointer_search_report.json` | Результаты поиска |
| `regenerate_mapping.py` | Генератор `gm1_mapping.json` (decimal→hex) |
| `test_parser.py` | Тест парсера без GUI (с заглушками PySide6) |

## Что делать с этими скриптами

- **Не запускать** — пути захардкожены, целевые сейвы не в репозитории
- **Читать как справочник** — если нужно понять, как было локализовано конкретное поле
- **Результаты уже учтены** в `02_format_docs/gm1_mapping.json` (поле `field_offsets` для героев/городов) и `02_format_docs/diff_interpretation.json` (32 дифф-анализа)

## Если делаем дифф-анализ для НОВОЙ карты

1. Создать подпапку `04_diff_analysis/NewMap/`
2. Положить туда сейвы (до/после действия) — в папку `examples/NewMap/`
3. Использовать новый `01_tools/map_config_builder.py` для построения config
4. Использовать `01_tools/gm1_diff.py` для сравнения сейвов
5. Новый скрипт положить в подпапку `04_diff_analysis/NewMap/`
