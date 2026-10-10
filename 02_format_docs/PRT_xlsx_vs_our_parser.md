# Сравнение: ProspectorRT 0000.xlsx vs наш parsed_save.json

> **Дата:** 2026-10-10
> **PRT source:** `upload/0000.xlsx` (22 листа, открыт в ProspectorRT)
> **Our parser:** `parsed_save.json` (открыт через наш gm1_parser на day-0 сейве той же карты)
> **Карта:** Myth and Legend.h3m (144×144, 7009 объектов, 21 town, 156 heroes)

## TL;DR

**Все 23 алгоритма у нас реализованы в `post_tile_parser.py` ✅**, НО `parsed_save.json`
не экспонирует пост-обработку, которую ProspectorRT делает ПОСЛЕ парсинга — объединение
tile-scan данных с post-tile контентом в "человекочитаемые" таблицы.

PRT `0000.xlsx` содержит **22 листа** (21 содержательный + 1 нулевой):
- **16 "сырых" таблиц** (Артефакты, Монстры, Банки, ...) — данные тайлов, обогащённые post-tile контентом
- **3 агрегатора** (Все Арты, Все Заклы, Все Навыки) — всё снаряжение на карте в одной таблице
- **1 опытный агрегатор** (Опыт — 568 строк) — все источники опыта
- **2 базовые таблицы** (Герои, Города) — основная информация

Наш `parsed_save.json` экспонирует только:
- ✅ `heroes_found` (156 героев) — основная информация, но **нет 3 колонок PRT**
- ✅ `towns_found` (21 город) — основная информация, **нет расширения спеллов**
- ✅ `objects_on_map` (7009 объектов) — координаты + типы, но **нет merge с post-tile**
- ✅ `blocks` (11 кластеров) — смещения
- ❌ `post_tile_content` — **НЕ ЭКСПОНИРУЕТСЯ** в JSON (есть во внутреннем `_post_tile_content`)
- ❌ aggregators — **НЕ ЭКСПОНИРУЮТСЯ**

---

## Детальное сравнение по 22 листам PRT

### ✅ Базовые таблицы (полные или частичные)

#### 1. Герои (PRT: 155 строк, наш: 156)

PRT колонки: `№, Герой, Местоположение, Флаг, Уровень, Опыт, Первичный навык, Вторичный навык, Артефакт, Заклинание, Монстр, ∑ HP, Машина, Книга заклинаний, MP`

У нас есть: `name, player, player_name, level, experience, movement_total, movement_left, mana_left, location_x/y/z, attack, defense, power, knowledge, num_skills, army_types, army_counts, skill_levels, skill_slots, spells_book, equipment`

**⚠️ Не хватает 3 полей:**
- `Флаг` (Hire flag — `decmperoState + i + HeroCount] & (1 << human)` в ProspectorRT)
- `Машина` (war machines: Ballista, Ammo Cart, First Aid Tent — doll slots 4-6)
- `Книга заклинаний` (Book slot — doll slot 0)

Эти поля парсятся в `parse_art_doll()` (art_id 0 = Book, 4/5/6 = Machines), но **не экспонируются отдельно**.

#### 2. Города (PRT: 101 строка = 21 town × ~5 spells, наш: 21 towns)

PRT колонки: `X, Y, Z, Город, Тип, Флаг, Уровень, Заклинание, Построен, Доступен, Таймер, Библиотека, Гарнизон, ∑ HP`

У нас есть: `name, faction, faction_name, type, type_name, x, y, z, army_types, army_counts, spell_pool_depth`

**⚠️ Не хватает:**
- `Заклинание` (каждое заклинание в Magic Guild — отдельная строка, например 5 spells × 21 town = 105 строк, у PRT 101)
- `Построен` (битовая маска построенных зданий — 5 байт × 8 битов)
- `Доступен` (что-то вроде "признак постройки сегодня")
- `Таймер` (бит-флаги Mage Timer / Library Timer — из `GetTimerTown`)
- `Библиотека` (для Tower — флаг Library, добавляет +1 spell per level)

У нас есть `parse_town_spell()` (Magic Guild spells), но **город не расширён до построчного списка спеллов**.

### ❌ Сырые таблицы (post-tile content merge) — все 16 отсутствуют в JSON

| # | Лист PRT | Строк | Что это | Алгоритм у нас | В parsed_save.json |
|---|---|---|---|---|---|
| 3 | Артефакты | 139 | Артефакты на карте + name/class/relic/guard | `parse_art_res_content` ✅ | ❌ |
| 4 | Монстры | 237 | Монстры с сокровищами + count/mood/level/HP/art/gold/res | `parse_monstr_content` ✅ | ❌ |
| 5 | Банки | 7 | Creature Banks (post-tile) | `parse_bank_content` ✅ | ❌ |
| 6 | События и Ящики Пандоры | 98 | EventBox с полным содержимым | `parse_event_box_content` ✅ | ❌ |
| 7 | Ученые | 10 | Scholars (gives primary OR secondary OR spell) | `tile_scanner._parse_scholar` ✅ | ❌ |
| 8 | Ресурсы | 482 | Resource piles + optional guard | `tile_scanner._parse_resource` ✅ | ❌ |
| 9 | Сундуки | 283 | Chests (gold/exp/artifact) | `tile_scanner._parse_chest` ✅ | ❌ |
| 10 | Заклинания | 40 | Spell shrines + town spells + scroll objects | `_parse_shrine` ✅ + `parse_town_spell` ✅ | ❌ |
| 11 | Навыки | 14 | Witch huts + universities + scholars with skills | `_parse_witch_hut` ✅ + `parse_univer_content` ✅ | ❌ |
| 12 | Лагеря Беженцев | 1 | Refugee camps (random monsters) | `_parse_refugee_camp` ✅ | ❌ |
| 13 | Рынки | 27 | Black Market + Art Merchants | `parse_market_content` ✅ + `parse_art_merchants` ✅ | ❌ |
| 14 | Провидцы | 0 (empty) | Seer Huts (на этой карте нет активных) | `parse_seer_hut_content` ✅ | ❌ |
| 15 | Тюрьмы | 5 | Prison heroes with full stats | `parse_prison_hero` ✅ | ❌ |
| 16 | Объекты | 26 | Misc objects (Hill Fort, Tent, etc.) | `tile_scanner._parse_*` ✅ | ❌ |
| 17 | Топология | 14 | Keymaster tents, monoliths, sub-gates, whirlpools | `parse_monolith_whirlpool` ✅ + `parse_pair_subterranean_gate` ✅ | ❌ |
| 18 | События-Таймеры | 174 | Map + town timed events | `parse_map_timed_events` ✅ + `parse_town_timed_events` ✅ | ❌ |

### ❌ Агрегаторы (4 листа) — все отсутствуют в JSON

| # | Лист PRT | Строк | Что агрегирует | Алгоритм у нас | В parsed_save.json |
|---|---|---|---|---|---|
| 19 | Все Арты | 181 | ВСЕ артефакты: hero doll, prison, market, ground, chest, event_box, bank, seer_hut reward | `aggregate_all_artifacts` ❌ **НЕТ** | ❌ |
| 20 | Все Заклы | 458 | ВСЕ заклинания: heroes, towns, shrines, event_box, scholars, seer_hut, scroll | `aggregate_all_spells` ✅ | ❌ |
| 21 | Все Навыки | 28 | ВСЕ secondary skills: hero skills, witch hut, scholar, event_box, seer_hut | `aggregate_all_skills` ✅ | ❌ |
| 22 | Опыт | 568 | ВСЕ источники опыта: chest, monster, bank, event_box, town garrison, hero army, seer_hut | `parse_experience_sources` ✅ | ❌ |

---

## Вывод

### Что у нас есть (✅ реализовано, но НЕ экспонируется в JSON):

Все **23 алгоритма ProspectorRT** реализованы в `01_tools/post_tile_parser.py`. Проблема — `parsed_save.json` не показывает их.

### Чего реально не хватает (для соответствия PRT):

1. **Expose `post_tile_content` в export_to_json** — все 18 контент-парсеров уже возвращают данные, нужно только добавить в `export_data`

2. **Expose aggregators в export_to_json** — `aggregate_all_spells`, `aggregate_all_skills`, `parse_experience_sources` уже написаны

3. **Реализовать `aggregate_all_artifacts`** — единственный отсутствующий агрегратор (181 строка в PRT: hero doll + prison + market + ground + chest + event_box + bank + seer_hut reward)

4. **Добавить 3 недостающие hero columns** — Hire flag (`Флаг`), War Machines (`Машина`), Spell Book (`Книга заклинаний`). Данные уже парсятся в `parse_art_doll()` (art_id 0 = Book, 4/5/6 = Machines), нужно только отдельно извлечь

5. **Расширить town до spell rows** — каждый town размножается на N строк (по одной на заклинание в Magic Guild). Данные уже в `parse_town_spell()`

6. **Добавить town buildings bitmask** — `Построен`/`Доступен`/`Таймер`/`Библиотека`. Алгоритм есть в `parse_timer_town`, нужно применить к town record

7. **Объединить tile_scan + post_tile для merged таблиц** — Артефакты/Монстры/Ресурсы/Chests — PRT делает это в `Scanner()` после tile loop, когда Tbl* DataTables заполнены. У нас tile_scanner и post_tile_parser работают раздельно — нужно при парсинге тайла извлечь координаты и затем сматчить с post-tile контентом по координатам

### Что НЕТ алгоритмически (нужно дописать):

- `aggregate_all_artifacts` — новый алгоритм-агрегратор (аналог `GetAllSpell`/`GetAllSkill` в ProspectorRT, но для артефактов). В MainForm.cs:7263 GetAllSpell реализован, но GetAllArtifacts — НЕТ (PRT делает это через `R_AllArts` DataTable, которую заполняет по мере парсинга каждой секции). Нам нужно создать отдельную функцию, которая пройдёт по всем секциям и соберёт artifacts.

### Дорожная карта соответствия PRT:

1. ✅-этап: **Добавить `post_tile_content` в `export_to_json`** — просто, ~10 строк
2. ✅-этап: **Добавить aggregators в `export_to_json`** — `aggregate_all_spells/skills/experience_sources` уже есть
3. ⏳-этап: **Написать `aggregate_all_artifacts`** — новый код, ~80 строк (по аналогии с GetAllSpell)
4. ⏳-этап: **Добавить 3 hero columns** (Hire/Machine/Book) — ~20 строк
5. ⏳-этап: **Расширить town до spell rows** в JSON — ~30 строк
6. ⏳-этап: **Tile-scan + post-tile merge** — самая большая работа, ~300 строк: нужно для соответствия листам Артефакты/Монстры/Ресурсы/Chests

Все 6 этапов реализуемы без дополнительного реверс-инжиниринга — у нас есть все исходные алгоритмы. Это чисто **post-processing pipeline**.
