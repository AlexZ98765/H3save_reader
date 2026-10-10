# ЧЕСТНЫЙ АУДИТ: ProspectorRT vs наша реализация — алгоритмы чтения

> **Дата:** 2026-10-10
> **Метод:** Построчное сравнение MainForm.cs (16 210 строк) с `01_tools/*.py`
> **Критерий:** Только алгоритмы ЧТЕНИЯ (байты сейва → Python objects)
> **Что НЕ считается:** агрегаторы, форматирование для UI, helpers

## Почему я был неправ раньше

Я говорил "все 23 алгоритма реализованы" — это касалось **только post-tile парсеров**
из аудита `02_format_docs/unimplemented_algorithms_audit.md`. Я не проверил
**tile-level парсеры** (Save* методы в `IsObject` диспетчере) и не учёл
**inline-реализации** (например, `ScanBankContent` встроен в `walk_post_tile_sections`).

Сейчас я честно прошёл по всему `MainForm.cs`, и вот реальная картина.

---

## ⭐ ИТОГ: 18 алгоритмов чтения не реализованы

| Категория | Всего PRT | Реализовано | Не реализовано |
|---|---|---|---|
| **A. Tile-level парсеры** (Save* методы, IsObject диспетчер) | 37 | 24 | **13** |
| **B. Post-tile парсеры** (Get/Scan/Content/Analysis) | 56 | 51 | **5** |
| C. Агрегаторы (игнорируем по требованию) | 3 | 0 | (не считаем) |
| D. UI форматирование + helpers (игнорируем) | 14 | 0 | (не считаем) |

---

## ❌ 13 отсутствующих tile-level парсеров

Из 37 Save* методов в `IsObject` диспетчере (MainForm.cs:9484) у нас есть 24,
отсутствуют 13. Большинство тривиальны — записывают x,y,z и читают 1-2 байта.

| type_id | PRT метод | Что читает из сейва | Сложность |
|---|---|---|---|
| 104 | `SaveUniver` | Только x,y,z (контент в `parse_univer_content`) | тривиально (5 строк) |
| 78 | `SaveCamp` | `decmp[s+6]` (monster_id), `decmp[s+7]` (count) | просто (10 строк) |
| 7 | `SaveMarket` | Только x,y,z (контент в `parse_market_content`) | тривиально (5 строк) |
| 33 | `SaveGarrison` | Только x,y,z (контент в `parse_garrison_content`) | тривиально (5 строк) |
| 83 | `SaveSeerHut` | Только x,y,z + `decmp[s+6]` (Num) (контент в `parse_seer_hut_content`) | просто (10 строк) |
| 215 | `SavePassDuard` | `decmp[s+6,7]` u16 (pass_id) | просто (8 строк) |
| 62 | `SavePrison` | `decmp[s+6]` (hero_id) | просто (8 строк) |
| 100 | `SaveLearningStone` | Только x,y,z + фиксированный Experience=1000 | тривиально (5 строк) |
| 34 | `SaveHero` | 4 байта + вызывает `HeroOnObject` (см. ниже) | средне (40 строк) |
| 2,35,95,102,213 | `SaveObject` | Для type 102 (Hill Fort): `decmp[s+7]` (32=2000, 64=G10) | просто (15 строк) |
| 43,44,45 | `SaveMonolith` | Только x,y,z | тривиально (5 строк) |
| 10 | `SaveTent` | Цвет палатки ключника | просто (8 строк) |
| 103 | `SaveTopologyObj` | Только x,y,z | тривиально (5 строк) |

**Важно:** Для типов 7, 33, 83, 104, 103 — основной контент читается в post-tile
парсерах, которые у нас ✅ есть. На tile-level нужно только записать координаты
и, возможно, ID для связывания.

---

## ❌ 5 отсутствующих post-tile парсеров

| PRT метод | Строка | Что делает | Сложность |
|---|---|---|---|
| `GetTimerTown` | 6984 | Связывает timed events с town по ID + декодирует buildings bitmask (5 байт × 8 битов = 40 buildings) | средне (~80 строк) |
| `IsHeroTavern` | 7971 | Маленький helper: ищет героя в таверне (используется в `GetHeroesContent` для `aPlace[2]`) | просто (~10 строк) |
| `GetBankResource` | 9018 | Читает 6 ресурсов × 1 байт + gold u16 в Bank record | просто (~15 строк) |
| `GetBankMonster` | 9013 | Читает monster_id (1 байт) + count (1 байт) в Bank record | просто (~10 строк) |
| `HeroOnObject` | 9643 | Обрабатывает tile где hero стоит на объекте: 5 байт swap, повторный вызов `IsObject`, restore | средне (~25 строк) |

**Важно:** `GetBankResource` и `GetBankMonster` функционально уже включены в
наш `parse_bank_content` (читает ресурсы и monster_reward), но не как
отдельные helper-функции. Это cosmetic, не функциональный пробел.

`HeroOnObject` — единственный реально сложный — это tile-scan сценарий,
когда герой стоит на тайле с другим объектом (artifact, mine, и т.д.).
PRT временно меняет байты, вызывает `IsObject` повторно, восстанавливает.

---

## ✅ Что у нас есть (51 из 56 post-tile + 24 из 37 tile = 75 всего)

Все ключевые алгоритмы чтения реализованы:
- Header walking (GetStart, GetMapStart, Get_SR)
- Tile loop (Scanner, IsObject, FirstID)
- Structure walk (GetSenseRegion, walk_post_tile_sections)
- All Content parsers (EventBox, ArtRes, Monstr, SeerHut, PassGuard, Bank, Garrison, Univer, Market)
- All Scan* skip-only functions
- Town content (GetTownContent, GetTownSpell)
- Hero content (GetHeroesContent, ArtDollPlace, GetPrisonHero)
- Player state (GetColorContent, GetCurrentState)
- Art merchants (GetArtMerchants)
- Timed events (Map/Towns/Res/Content)
- Topology (GetPairSubterraneanGate, ScanMonolithWhirlpool)

---

## Дорожная карта реализации (что нужно дописать)

### Этап 1: Tile-level парсеры (13 функций, ~150 строк)

Простые — добавить в `tile_scanner.py` `parse_object_content` dispatcher:

```python
elif type_id == 104:  # SaveUniver — University
    obj['object_kind'] = 'university'
    # content parsed in post_tile_parser.parse_univer_content

elif type_id == 78:  # SaveCamp — Mercenary camp
    obj['object_kind'] = 'mercenary_camp'
    obj['monster_id'] = raw[s + 6]
    obj['monster_count'] = raw[s + 7]

elif type_id == 7:  # SaveMarket — Black Market
    obj['object_kind'] = 'black_market'

elif type_id == 33:  # SaveGarrison
    obj['object_kind'] = 'garrison'

elif type_id == 83:  # SaveSeerHut
    obj['object_kind'] = 'seer_hut'
    obj['num'] = raw[s + 6]

elif type_id == 215:  # SavePassDuard — Border Guard
    obj['object_kind'] = 'border_guard'
    obj['pass_id'] = (raw[s + 7] << 8) | raw[s + 6]

elif type_id == 62:  # SavePrison
    obj['object_kind'] = 'prison'
    obj['hero_id'] = raw[s + 6]

elif type_id == 100:  # SaveLearningStone
    obj['object_kind'] = 'learning_stone'
    obj['experience'] = 1000  # fixed

elif type_id == 34:  # SaveHero — hero on map
    obj['object_kind'] = 'hero_on_map'
    # HeroOnObject case (see HeroOnObject below)
    obj['hero_id'] = raw[s + 6]
    obj['hero_id_2'] = raw[s + 7]

elif type_id in (2, 35, 95, 213):  # SaveObject — generic
    obj['object_kind'] = 'generic_object'

elif type_id == 102:  # Hill Fort
    obj['object_kind'] = 'hill_fort'
    if raw[s + 7] == 32:
        obj['upgrade_cost'] = '2000 gold'
    elif raw[s + 7] == 64:
        obj['upgrade_cost'] = '10 gems'

elif type_id in (43, 44, 45):  # SaveMonolith
    obj['object_kind'] = 'monolith'

elif type_id == 10:  # SaveTent — keymaster tent
    obj['object_kind'] = 'keymaster_tent'
    obj['tent_color'] = raw[s + 6]

elif type_id == 103:  # SaveTopologyObj
    obj['object_kind'] = 'topology_object'
```

### Этап 2: 5 post-tile парсеров (~140 строк)

1. `parse_timer_town(town_id, timed_events)` — ~80 строк: для каждого town найти
   все timed events с этим ID, декодировать 5 building bytes × 8 битов через
   `object_types_dictionary.json` (раздел Buildings).

2. `is_hero_tavern(hero_id, hero_state, hero_count, human_player)` — ~10 строк:
   проверяет биты `decmperoState + i] == 64` (tavern flag).

3. `parse_bank_resource(raw, s)` — ~15 строк: 6 × 1 байт ресурсов + u16 gold.
   (Функционально уже есть в `parse_bank_content`, выделяем в отдельную функцию.)

4. `parse_bank_monster(raw, s)` — ~10 строк: monster_id + count.
   (Аналогично — уже есть в `parse_bank_content`.)

5. `hero_on_object(raw, s, ...)` — ~25 строк: сложный сценарий hero-on-object.

---

## Окончательный ответ

**18 алгоритмов чтения отсутствуют** (13 tile + 5 post-tile).
**75 алгоритмов чтения реализованы** (24 tile + 51 post-tile).

Не реализованы:
- 10 тривиальных tile-парсеров (по 5-10 строк каждый — записать координаты + 1 байт)
- 3 средних tile-парсера (SaveHero + HeroOnObject + SaveObject для Hill Fort)
- 1 средний post-tile (GetTimerTown — buildings bitmask decoding)
- 2 тривиальных post-tile (GetBankResource + GetBankMonster — уже есть в parse_bank_content)
- 1 средний post-tile (HeroOnObject — 25 строк)

**Объём работы:** ~290 строк кода для полной реализации. Все алгоритмы
описаны в `07_prt_decompiled/ProspectorRT_source/ProspectorRT/MainForm.cs`,
никакого нового реверс-инжиниринга не нужно.

**Что НЕ считается отсутствующим:**
- 17 агрегаторов / UI форматирования / helpers — игнорируем по требованию
