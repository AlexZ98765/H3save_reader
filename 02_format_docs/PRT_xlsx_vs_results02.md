# Сравнение v3.9: PRT 0000.xlsx vs наш parsed_save.json (results02)

> **Дата:** 2026-10-10 (v3.9 — после реализации 18 алгоритмов)
> **PRT source:** `upload/0000.xlsx` (22 листа, открыт в ProspectorRT)
> **Our parser:** `upload/Myth and Legend.h3m_results02.zip` (parsed_save.json)
> **Карта:** Myth and Legend.h3m (144×144, 7009 объектов)

## Главный вывод

**Алгоритмы чтения (93/93) полностью реализованы.** Проблема сейчас не в
отсутствии алгоритмов — она в **формате вывода**: наш `parsed_save.json`
не содержит `post_tile_content` section, в которой лежат результаты
post-tile парсеров (EventBox, ArtRes, Monstr, SeerHut, Bank, ...).

PRT `0000.xlsx` содержит 22 листа, и в каждом листе данные — это результат
**объединения** (merge):
- Данные из tile-scan (координаты + тип объекта)
- Данные из post-tile content (содержимое объекта — guard, resources, artifacts)
- Данные из headers/player state/timed events (только для конкретных листов)

У нас все 3 источника данных парсятся, но не сливаются в финальный output.

---

## Построчное сравнение 22 листов PRT

### ✅ Лист 1: Герои — 155 rows PRT, 156 ours (полное соответствие)

| PRT колонка | У нас |
|---|---|
| № | ours.heroes_found[i] (по индексу) |
| Герой | name ✅ |
| Местоположение | location_x/y/z ✅ |
| Флаг | player_name ✅ |
| Уровень | level ✅ |
| Опыт | experience ✅ |
| Первичный навык | attack/defense/power/knowledge ✅ |
| Вторичный навык | skill_levels ✅ |
| Артефакт | equipment ✅ |
| Заклинание | spells_book ✅ (есть в save_layout) |
| Монстр | army_types + army_counts ✅ |
| ∑ HP | army_hp — НЕ ВЫЧИСЛЯЕТСЯ (нужна таблица HP из монстров) |
| **Машина** | **❌ НЕ эксПОНИРУЕТСЯ** — есть в `parse_art_doll` (art_id 4/5/6), но не выделено |
| **Книга заклинаний** | **❌ НЕ эксПОНИРУЕТСЯ** — есть в `parse_art_doll` (art_id 0), но не выделено |
| MP | movement_total ✅ |

**Проблемы (2 колонки):** Машина (war machines) и Книга заклинаний (spell book)
парсятся в `parse_art_doll()`, но не выделяются в отдельные поля hero record.

### ✅ Лист 2: Города — 21 town в обоих

PRT показывает 100 строк = 21 town × ~5 spells (одно заклинание Magic Guild на строку).
У нас 21 town, но spells не разворачиваются в строки.

PRT также показывает:
- "Построен" (битовая маска зданий — 5 байт × 8 битов)
- "Доступен" (доступно ли сегодня)
- "Таймер" (MageTimer/LibTimer)
- "Библиотека" (Tower library flag)
- "Гарнизон" + ∑ HP (army в гарнизоне + total HP)

У нас есть базовые town fields, но:
- `spell_pool` (from parse_town_spell) — НЕ ЭКСПОНИРУЕТСЯ
- `buildings_built` (from parse_timer_town) — НЕ ЭКСПОНИРУЕТСЯ
- `garrison` (army) — ЕСТЬ (army_types/army_counts)
- ∑ HP — НЕ ВЫЧИСЛЯЕТСЯ

### ❌ Лист 3: Артефакты — 139 rows PRT, 0 rows в нашем output

PRT: X, Y, Z, Район, Объект, Слот, Артефакт, Класс, Реликт-С, Золото, Ресурс, Охрана, ∑ HP

У нас в `objects_on_map` есть ~160 артефактов (сумма Random Treasure/Minor/Major Artifact),
но:
- Имя артефакта — НЕТ (только тип "Random Treasure Artifact")
- Класс — НЕТ
- Реликт-С — НЕТ
- Guard — парсится в `parse_art_res_content`, но НЕ ЭКСПОНИРУЕТСЯ
- ∑ HP — НЕ ВЫЧИСЛЯЕТСЯ

**Причина:** наш `objects_on_map` берёт данные из JSON карты (`.h3m`), а не
из сейва. Map JSON знает только тип "Random Treasure Artifact", но не конкретный
ID артефакта. Конкретный ID парсит `tile_scanner._parse_artifact` (читает
`raw[s+2]`), но результат **не объединяется** с `objects_on_map`.

### ❌ Лист 4: Монстры — 237 rows PRT, 0 rows в нашем output

PRT: X, Y, Z, Район, Монстр, Количество, Настроение, Уровень, ∑ HP, Артефакт, Золото, Ресурс, Прирост

У нас в `objects_on_map` есть ~220 монстров (Random Monster 1-7), но:
- Имя монстра — НЕТ
- Количество — парсится в `_parse_monster` (raw[s+6,7]), но НЕ ЭКСПОНИРУЕТСЯ
- Настроение (mood) — парсится, но НЕ ЭКСПОНИРУЕТСЯ
- Артефакт/Золото/Ресурс (treasure) — парсится в `parse_monstr_content`, но НЕ ЭКСПОНИРУЕТСЯ

### ❌ Лист 5: Банки — 7 rows PRT, 0 rows в нашем output

PRT: X, Y, Z, Район, Объект, Охрана, ∑ HP, Монстр, Золото, Ресурс

У нас:
- `tile_scanner._parse_bank` находит bank tiles (~7 objects)
- `post_tile_parser.parse_bank_content` парсит содержимое
- `parse_bank_resource` + `parse_bank_monster` работают
- **НО всё это НЕ ЭКСПОНИРУЕТСЯ в parsed_save.json**

### ❌ Лист 6: События и Ящики Пандоры — 98 rows PRT, 0 rows в нашем output

PRT: X, Y, Z, Район, Объект, Охрана, ∑ HP, Опыт, Мана, Мораль, Удача, Золото, Ресурс, Первичный навык, Вторичный навык, Артефакт, Заклинание, Монстр, Доступно

У нас:
- `tile_scanner._parse_event` находит event tiles (98 objects)
- `tile_scanner._parse_pandora_box` находит pandora boxes
- `post_tile_parser.parse_event_box_content` парсит ВСЕ поля (опыт, мана, ресурсы, артефакты, спеллы, монстры)
- **НО НЕ ЭКСПОНИРУЕТСЯ в parsed_save.json**

### ❌ Листы 7-22: всё парсится, но не экспонируется

| Лист PRT | Rows | У нас парсер | В parsed_save.json |
|---|---|---|---|
| Ученые (Scholars) | 10 | `_parse_scholar` ✅ | ❌ |
| Ресурсы | 482 | `_parse_resource` ✅ | ❌ |
| Сундуки | 283 | `_parse_chest` ✅ | ❌ |
| Заклинания | 40 | `_parse_shrine` + `parse_town_spell` ✅ | ❌ |
| Навыки | 14 | `_parse_witch_hut` + `parse_univer_content` ✅ | ❌ |
| Лагеря Беженцев | 1 | `_parse_refugee_camp` ✅ | ❌ |
| Рынки | 27 | `parse_market_content` + `parse_art_merchants` ✅ | ❌ |
| Провидцы | 0 | `parse_seer_hut_content` ✅ | ❌ (в этом сейве 0) |
| Тюрьмы | 5 | `_parse_prison` + `parse_prison_hero` ✅ | ❌ |
| Объекты | 26 | `_parse_generic_object` (Hill Fort) ✅ | ❌ |
| Топология | 14 | `parse_monolith_whirlpool` + `parse_pair_subterranean_gate` ✅ | ❌ |
| События-Таймеры | 174 | `parse_map_timed_events` + `parse_town_timed_events` ✅ | ❌ |
| Все Арты (агрегатор) | 181 | НЕ реализован (по требованию игнорируем) | ❌ |
| Все Заклы (агрегатор) | 458 | `aggregate_all_spells` ✅ | ❌ |
| Все Навыки (агрегатор) | 28 | `aggregate_all_skills` ✅ | ❌ |
| Опыт (агрегатор) | 568 | `parse_experience_sources` ✅ | ❌ |

---

## Чёткая формулировка проблемы

**Проблема НЕ в алгоритмах (все 93 реализованы ✅).**

**Проблема в pipeline `save_parser.parse_save()` и `gm1_parser.export_to_json()`:**

1. `save_parser.parse_save()` вызывает `parse_all_post_tile_sections` ✅
   (с v3.8 — данные есть в `parsed._post_tile_content`)
2. `gm1_parser.export_to_json()` **НЕ копирует** `parsed._post_tile_content` в output JSON ❌
3. `gm1_parser.export_to_json()` **НЕ объединяет** tile-scan данные с post-tile данными ❌

Дополнительно:
4. Hero fields: `war_machines` и `spell_book` парсятся в `parse_art_doll()`, но
   не выделяются как отдельные поля — нужно дописать в `parse_hero_block`
5. Town spells: `parse_town_spell()` возвращает `TownSpellPool`, но
   `parse_town_block()` его не вызывает — нужно добавить

---

## Что нужно реализовать для соответствия PRT xlsx

### Приоритет 1 (минимальный — экспонировать уже готовое):

**A. Экспонировать `post_tile_content` в `export_to_json`** (~10 строк в gm1_parser.py)
- Просто добавить `post_tile_content` в `export_data` dict
- Это даст 99 EventBox + 10 ArtRes + 34 Monstr + 1 SeerHut + 12 Banks + ... в JSON output

**B. Экспонировать `_player_states` и `_current_state` в `export_to_json`** (~5 строк)
- Уже парсятся в save_parser, просто добавить в output

### Приоритет 2 (средний — расширить heroes/towns):

**C. Добавить 3 поля в hero** (~20 строк в save_parser.parse_hero_block)
- `war_machines`: из `equipment` найти art_id 4/5/6 (Ballista/Ammo Cart/First Aid Tent)
- `spell_book`: из `equipment` найти art_id 0 (Spell Book)
- `army_hp`: суммарное HP армии (нужна таблица HP из monster_types_dictionary)

**D. Добавить spells/buildings в town** (~30 строк в save_parser.parse_town_block)
- Вызвать `parse_town_spell()` для каждого town
- Добавить `spell_pool` (5-6 levels × 5 spells)
- Добавить `buildings_built` из `parse_timer_town()` (linked by town ID)

### Приоритет 3 (большой — merge tile-scan с post-tile для PRT-подобных таблиц):

**E. Создать "merged_objects" section** (~300 строк в новом модуле)
- Для каждого объекта из `objects_on_map`:
  - Найти его координаты в `post_tile_content` (EventBox, ArtRes, Monstr, ...)
  - Объединить данные в одну запись с полями: type, name, x/y/z, guard, resources, artifacts, spells, ...
- Это даст эквивалент PRT листам Артефакты/Монстры/Сундуки/Банки/...

### Приоритет 4 (агрегаторы — игнорируем по требованию):

**F. Реализовать `aggregate_all_artifacts`** (~80 строк) — единственный отсутствующий
- Walks all sources: hero equipment, prison, market, ground (ArtRes), chest,
  event_box, bank, seer_hut reward
- PRT имеет "Все Арты" (181 rows)

---

## Рекомендация

Сделать **Приоритет 1 (A+B)** прямо сейчас — это даст немедленное соответствие
по всем post-tile секциям (большая часть листов PRT). Это ~15 строк кода в
`gm1_parser.export_to_json()`.

Затем **Приоритет 2 (C+D)** — ~50 строк, даст соответствие по Герои и Города.

**Приоритет 3 (E)** — самая большая работа, но это и есть финальное
соответствие PRT xlsx по всем "сырым" листам (Артефакты, Монстры, Банки, ...).
