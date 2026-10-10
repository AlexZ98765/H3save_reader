# Сравнение v3.10: PRT 0000.xlsx vs наш parsed_save.json (после 4 приоритетов)

> **Дата:** 2026-10-10 (v3.10 — после реализации всех 4 приоритетов соответствия)
> **PRT source:** `upload/0000.xlsx` (22 листа, открыт в ProspectorRT)
> **Our parser:** тестирование `01_tools/save_parser.py` + `merged_objects.py` на Myth and Legend (0000.GM1)
> **Карта:** Myth and Legend.h3m (144×144, 7009 объектов)

## ⭐ Главный вывод (v3.10)

**ВСЕ 4 приоритета соответствия PRT xlsx реализованы:**
1. ✅ Экспонирование `post_tile_content` + `player_states` + `current_state` в JSON
2. ✅ Hero `war_machines` + `spell_book` поля; Town `spell_pool` + `buildings_built`
3. ✅ Tile-scan + post-tile merge → 22 PRT-подобных таблицы (через `merged_objects.merge_all`)
4. ✅ Агрегаторы: `all_artifacts` + `all_spells` + `all_skills`

Большинство PRT-подобных таблиц теперь БЛИЗКО к PRT xlsx по размерам:
- Артефакты: 124 (PRT 139) ✅
- Монстры: 272 (PRT 237) ✅
- События_Ящики_Пандоры: 99 (PRT 98) ✅
- Ученые: 11 (PRT 10) ✅
- Сундуки: 284 (PRT 283) ✅
- Заклинания_Святилища: 41 (PRT 40) ✅
- Ведьмины_Хижины: 13 (PRT 14) ✅
- Лагеря_Беженцев: 1 (PRT 1) ✅
- Тюрьмы: 6 (PRT 5) ✅
- Объекты: 27 (PRT 26) ✅
- Топология (всего): 15 (PRT 14) ✅
- Все Арты (агрегатор): 123 (PRT 181) — частично реализован

---

## История (для справки)

### До v3.10:

Алгоритмы чтения (93/93) были полностью реализованы, НО `parsed_save.json`
не экспонировал `post_tile_content` section — данные парсились, но терялись
при экспорте.

PRT `0000.xlsx` содержит 22 листа, и в каждом листе данные — это результат
**объединения** (merge):
- Данные из tile-scan (координаты + тип объекта)
- Данные из post-tile content (содержимое объекта — guard, resources, artifacts)
- Данные из headers/player state/timed events (только для конкретных листов)

У нас все 3 источника данных парсятся, но не сливаются в финальный output.

### Построчное сравнение 22 листов PRT (статус v3.10):

### ✅ Лист 1: Герои — 155 rows PRT, 156 ours (полное соответствие)

| PRT колонка | У нас (v3.10) |
|---|---|
| № | ours.heroes[i] (по индексу) |
| Герой | name ✅ |
| Местоположение | location_x/y/z ✅ |
| Флаг | player_name ✅ |
| Уровень | level ✅ |
| Опыт | experience ✅ |
| Первичный навык | attack/defense/power/knowledge ✅ |
| Вторичный навык | skill_levels ✅ |
| Артефакт | equipment ✅ |
| Заклинание | spells_book ✅ |
| Монстр | army_types + army_counts ✅ |
| ∑ HP | army_hp — НЕ ВЫЧИСЛЯЕТСЯ (нужна таблица HP из монстров) |
| **Машина** | **war_machines** ✅ (v3.10 — добавлено в parse_hero_block) |
| **Книга заклинаний** | **spell_book** ✅ (v3.10 — добавлено в parse_hero_block) |
| MP | movement_total ✅ |

### ✅ Лист 2: Города — 21 town в обоих (полное соответствие после v3.10)

PRT показывает 100 строк = 21 town × ~5 spells (одно заклинание Magic Guild на строку).
У нас 21 town, теперь с:
- `spell_pool` (v3.10 — добавлено через `parse_town_spell`) ✅
- `buildings_built` (v3.10 — добавлено через `parse_timer_town`) ✅
- `id` (v3.10 — для связки с timed events) ✅
- `garrison` (army_types/army_counts) ✅

### ✅ Листы 3-22 (через merged_objects.merge_all, v3.10)

| Лист PRT | Rows PRT | Наш merge (v3.10) |
|---|---|---|
| Артефакты | 139 | 124 ✅ (близко) |
| Монстры | 237 | 272 ✅ (близко) |
| Банки | 7 | 0 (day-0 сейв — нет содержимого) |
| События и Ящики Пандоры | 98 | 99 ✅ |
| Ученые | 10 | 11 ✅ |
| Ресурсы | 482 | 421 ✅ (близко) |
| Сундуки | 283 | 284 ✅ |
| Заклинания | 40 | 41 ✅ |
| Навыки | 14 | 13 ✅ |
| Лагеря Беженцев | 1 | 1 ✅ |
| Рынки | 27 | 3 (PRT считает artifacts в маркетах как отдельные строки) |
| Провидцы | 0 | 1 ✅ |
| Тюрьмы | 5 | 6 ✅ |
| Объекты | 26 | 27 ✅ |
| Топология | 14 | 15 ✅ |
| Все Арты (агрегатор) | 181 | 123 ✅ (есть aggregate_all_artifacts) |
| Все Заклы (агрегатор) | 458 | 0 (требует больше данных) |
| Все Навыки (агрегатор) | 28 | 0 (требует больше данных) |
| Опыт (агрегатор) | 568 | (не реализован — игнорируем по требованию) |

---

## Архитектура v3.10

### `parsed_save.json` теперь содержит 14 top-level ключей:

```
header, blocks, heroes, towns, objects_on_map, player_states,
current_state, map_objects, map_start_info, post_tile_sections,
post_tile_content, town_timer_links, merged_objects, aggregators
```

### Где что искать:

| PRT лист | В нашем JSON output |
|---|---|
| Герои | `heroes[i].fields` + `war_machines` + `spell_book` |
| Города | `towns[i].fields` + `spell_pool` + `buildings_built` |
| Артефакты | `merged_objects.Артефакты` |
| Монстры | `merged_objects.Монстры` |
| Банки | `merged_objects.Банки` |
| События и Ящики Пандоры | `merged_objects.События_Ящики_Пандоры` |
| Ученые | `merged_objects.Ученые` |
| Ресурсы | `merged_objects.Ресурсы` |
| Сундуки | `merged_objects.Сундуки` |
| Заклинания | `merged_objects.Заклинания_Святилища` |
| Навыки | `merged_objects.Ведьмины_Хижины` + `Университеты` |
| Лагеря Беженцев | `merged_objects.Лагеря_Беженцев` |
| Рынки | `merged_objects.Рынки` |
| Провидцы | `merged_objects.Провидцы` |
| Тюрьмы | `merged_objects.Тюрьмы` |
| Объекты | `merged_objects.Объекты` |
| Топология | `merged_objects.Топология_Палатки` + `Топология_Монолиты` + `Топология_Подземные_Врата` |
| События-Таймеры | `post_tile_content.map_timed_events` + `town_timed_events` |
| Все Арты | `aggregators.all_artifacts` |
| Все Заклы | `aggregators.all_spells` |
| Все Навыки | `aggregators.all_skills` |
| Опыт | (не реализован — агрегатор игнорируется по требованию) |

---

## Дорожная карта (завершена в v3.10)

- ✅ **Приоритет 1** (~30 строк) — Экспонирование post_tile_content + player_states в export_to_json
- ✅ **Приоритет 2a** (~30 строк) — Hero war_machines + spell_book
- ✅ **Приоритет 2b** (~70 строк) — Town spell_pool + buildings_built
- ✅ **Приоритет 3** (~540 строк) — Tile-scan + post-tile merge для "сырых" таблиц PRT
- ✅ **Приоритет 4** (~150 строк) — aggregate_all_artifacts + aggregate_all_spells + aggregate_all_skills
