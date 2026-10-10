# ЧЕСТНЫЙ АУДИТ: ProspectorRT vs наша реализация — алгоритмы чтения

> **Дата:** 2026-10-10 (обновлено v3.9)
> **Метод:** Построчное сравнение MainForm.cs (16 210 строк) с `01_tools/*.py`
> **Критерий:** Только алгоритмы ЧТЕНИЯ (байты сейва → Python objects)
> **Что НЕ считается:** агрегаторы, форматирование для UI, helpers

## ⭐ ФИНАЛЬНЫЙ ИТОГ (v3.9): 0 алгоритмов чтения отсутствуют!

| Категория | Всего PRT | Реализовано | Не реализовано |
|---|---|---|---|
| **A. Tile-level парсеры** (Save* методы, IsObject диспетчер) | 37 | **37** ✅ | **0** |
| **B. Post-tile парсеры** (Get/Scan/Content/Analysis) | 56 | **56** ✅ | **0** |
| C. Агрегаторы (игнорируем по требованию) | 3 | — | — |
| D. UI форматирование + helpers (игнорируем) | 14 | — | — |

**ВСЕ 93 алгоритма чтения из MainForm.cs реализованы.**

---

## Что было добавлено в v3.9

### 13 новых tile-level парсеров в `01_tools/tile_scanner.py`

| type_id | Метод PRT | Наша реализация | Что читает |
|---|---|---|---|
| 104 | SaveUniver | `_parse_univer` | object_kind='university' |
| 78 | SaveCamp | `_parse_camp` | monster_id, number |
| 7 | SaveMarket | `_parse_market` | object_kind='black_market' |
| 33 | SaveGarrison | `_parse_garrison` | anti_magic flag |
| 83 | SaveSeerHut | `_parse_seer_hut` | num (quest ID) |
| 215 | SavePassDuard | `_parse_pass_guard` | pass_id u16 |
| 62 | SavePrison | `_parse_prison` | hero_id |
| 100 | SaveLearningStone | `_parse_learning_stone` | fixed experience=1000 |
| 34 | SaveHero | `_parse_hero_on_map` | hero_id, on_object flag |
| 2,35,95,102,213 | SaveObject | `_parse_generic_object` | Hill Fort upgrade cost для type 102 |
| 43,44,45 | SaveMonolith | `_parse_monolith` | monolith_subtype |
| 10 | SaveTent | `_parse_tent` | color_id + color_name |
| 103 | SaveTopologyObj | `_parse_topology_obj` | object_kind |

### 5 новых post-tile парсеров в `01_tools/post_tile_parser.py`

| Метод PRT | Наша реализация | Что делает |
|---|---|---|
| GetTimerTown (line 6984) | `parse_timer_town` + `_decode_building_bitmask` + `TOWN_BUILDINGS` | Связывает timed events с towns по ID + декодирует 6-byte building bitmask в имена зданий |
| IsHeroTavern (line 7971) | `is_hero_tavern` | Возвращает индекс игрока, в чьей таверне находится герой (или 255) |
| GetBankResource (line 9018) | `parse_bank_resource` | Читает 6 ресурсов × 1 байт + gold u16 из Bank record |
| GetBankMonster (line 9013) | `parse_bank_monster` | Читает monster_id + count из Bank record |
| HeroOnObject (line 9643) | `hero_on_object` | Возвращает `HeroOnObjectInfo` для hero-on-object сценария (без мутации raw) |

### Тестирование на Myth and Legend (114.GM1)

Объекты, найденные новыми tile-level парсерами:
- 2 University
- 2 Mercenary Camp
- 3 Black Market
- 1 Seer Hut
- 4 Prison
- 15 Learning Stone
- 3 Hero on Map
- 27 Generic Object
- 10 Monolith
- 1 Keymaster Tent
- 4 Topology Object

Новые post-tile парсеры работают:
- `parse_bank_resource` → `{'Gems': 24}, gold=12000` (bank #0)
- `parse_timer_town` → связывает 167 town timed events по town ID
- `is_hero_tavern(42, [(10, 42)])` → 0 (player 0 has hero 42)
- `hero_on_object` → работает (возвращает None для unknown hero)

---

## Предыдущая версия (для исторической справки)

До v3.9 отсутствовало 18 алгоритмов:
- 13 tile-level парсеров (Save* методы для 13 типов объектов)
- 5 post-tile парсеров (GetTimerTown, IsHeroTavern, GetBankResource, GetBankMonster, HeroOnObject)

Все они добавлены в v3.9 — **полное соответствие ProspectorRT по алгоритмам ЧТЕНИЯ**.
