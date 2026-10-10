# Audit: алгоритмы ProspectorRT — статус реализации

> **Дата:** 2026-10-10 (оригинал), обновлено 2026-10-10 (v3.8 — все реализованы)
> **Source:** `07_prt_decompiled/ProspectorRT_source/ProspectorRT/MainForm.cs`
> **Реализация:** `01_tools/post_tile_parser.py` (1,779 строк)

## TL;DR

ProspectorRT `Scanner()` (MainForm.cs:6172) после `GetSenseRegion()` вызывает **32 функции парсинга**.

✅ **Все 23 нереализованных алгоритма ProspectorRT теперь реализованы в `01_tools/post_tile_parser.py`.**
Этот документ изначально был todo-списком отсутствующих парсеров; теперь он сохранён как
историческая справка по соответствию `MainForm.cs` ↔ `post_tile_parser.py`.

Осталось нереализованным только: **write-back pipeline** (редактирование сейва, Шаг 4).

---

## ⭐ Статус реализации по категориям (v3.8)

| Категория | Парсеры | Реализация в `post_tile_parser.py` | Статус |
|---|---|---|---|
| **A. Post-tile content** | `parse_event_box_content`, `parse_art_res_content`, `parse_monstr_content`, `parse_seer_hut_content`, `parse_pass_guard_content`, `parse_bank_content`, `parse_garrison_content`, `parse_univer_content`, `parse_market_content` | `EventBoxContent`, `ArtResContent`, `MonstrContent`, `SeerHutContent`, `PassGuardContent`, `BankContent`, `GarrisonContent`, `UniverContent`, `MarketContent` dataclasses | ✅ |
| **B. Alliance + ArtMerchants + EXP** | `parse_alliance`, `parse_art_merchants`, `parse_experience_sources` | `AllianceInfo`, `ArtMerchant`, `ExperienceSource` dataclasses | ✅ |
| **C. Timed events** | `parse_map_timed_events`, `parse_town_timed_events`, `_parse_timer_res`, `_parse_timer_content` | `TimedEvent` dataclass + helpers | ✅ |
| **D. Town spells** | `parse_town_spell` | `TownSpellPool` dataclass | ✅ |
| **E. ArtDollPlace** | `parse_art_doll` | `DollPlace` dataclass | ✅ |
| **F. Prison heroes** | `parse_prison_hero` | `PrisonHero` dataclass | ✅ |
| **G. Topology** | `parse_pair_subterranean_gate`, `parse_monolith_whirlpool` | `SubTerGate`, `MonolithInfo` dataclasses | ✅ |
| **H. Header offsets** | `find_header_offsets` | `HeaderOffsets` dataclass | ✅ |
| **I. Aggregators** | `aggregate_all_spells`, `aggregate_all_skills`, `aggregate_monstr_content` | helper functions | ✅ |
| **J. Editing / write-back** | (отсутствует) | НЕ реализовано — следующий шаг | ❌ |
| **K. Reference tables** | (заменено) | `03_object_mapping/object_types_dictionary.json` (2037 типов из LazyLlama) | ✅ (другой подход) |

---

## Что уже реализовано ✅ (оригинальная таблица — сохранена для исторической справки)

| ProspectorRT функция | Строка | Эквивалент в `01_tools/` |
|---|---|---|
| `GetStart()` | 9443 | `tile_scanner.find_map_start()` + `post_tile_parser.find_header_offsets()` (полная версия с Teams, MapName, BlackMarket, SR) |
| `GetMapStart()` | 9411 | `tile_scanner.find_map_start()` + `post_tile_parser.find_header_offsets()` |
| tile loop в `Scanner()` | 6211-6267 | `tile_scanner.scan_tiles()` |
| `IsObject()` (31 type dispatcher) | 9484 | `tile_scanner.parse_object_content()` |
| `GetSenseRegion()` | 6124 | `post_tile_scanner.walk_post_tile_sections()` (offsets + расширенная цепочка BitField/Monolith/SubTerGate/Bank/Motions) |
| `Scan*Content()` (10 функций) | 9081-9335 | `post_tile_scanner.scan_*` (offsets only) + `post_tile_parser.parse_*` (content extraction) |
| `GetColorContent()` | 7653 | `save_parser.parse_hero_block()` + `PLAYER_STATE_OFFSETS` |
| `GetCurrentState()` | 7668 | `save_parser` + `CURRENT_STATE_OFFSETS` + `post_tile_parser.parse_art_merchants()` |
| `GetTownContent()` | 7458 | `save_parser.parse_town_block()` (базовые поля) + `post_tile_parser.parse_town_spell()` (Magic Guild) |
| `GetHeroesContent()` | 7723 | `save_parser.parse_hero_block()` (базовые поля) + `post_tile_parser.parse_art_doll()` (doll slots) |

---

## ⭐ ⭐ ⭐ Категории A-I (теперь все ✅ реализованы)

См. ниже оригинальные описания категорий A-K — они сохранены для справки.
Каждый пункт имеет заголовок "✅ Реализовано в ..." для указания текущего статуса.

### A1. `AnalysisContent` (строка 6809) — генерический post-tile парсер

✅ **Реализовано в `post_tile_parser.parse_analysis_content()`** — generic helper для EventBox/ArtRes/SeerHut/PassGuard.

Универсальная функция, которая по DataTable + делегату парсит все записи секции:
```csharp
private void AnalysisContent(int s, DataTable Tbl, ObjectContent ObjContent)
```
Используется для `EventBox`, `ArtRes`, `SeerHut`, `PassGuard`. У нас её нет.

### A2. `EventBoxContent` (строка 8172) — ⭐ высокий приоритет

Парсит одну Pandorabox / event-коробку:
- Позиция (x,y,z)
- Гард (7 монстров × 4 байта + counts)
- Опыт (4 байта int32)
- Мана (4 байта, signed)
- Мораль, удача (1+1 byte, signed)
- 6 ресурсов × 4 байта (signed, 0xFFFFFF=-1)
- Золото (4 байта, signed)
- 4 primary skills (1+1+1+1 byte)
- Список secondary skills (variable)
- Список артефактов (variable, u8 count + u8[] ids)
- Список заклинаний (variable, u8 count + u8[] ids)
- Список монстров (variable, u8 count + 4 байта × count)
- Apply flags (3 байта: human/non-human/computer)

У нас: только `scan_event_box_content` (skip-only, не извлекает поля).

### A3. `ArtResContent` (строка 8394) — ⭐ высокий приоритет

Парсит секцию `ArtRes` (артефакты/ресурсы/заклинания с гардом):
- Skip 2 bytes (length)
- Type (0=Art, 1=Resource, 2=Spell) — берётся из TblArtRes
- 7 монстров гарда (×4 байта + counts)
- Типа-специфичная запись в R_Art / R_Resource / R_Spell

У нас: только `scan_art_res_content` (skip-only).

### A4. `MonstrContent` (строка 8455) — ⭐ высокий приоритет

Парсит секцию `Monstr` (монстры на карте с сокровищами):
- Skip 2 bytes
- 6 ресурсов × 4 байта (signed encoding!)
- Золото (4 байта, signed)
- Артефакт (1 byte, 0xFF = нет)

У нас: только `scan_monstr_content` (skip-only).

### A5. `SeerHutContent` (строка 8505) — ⭐ высокий приоритет

Парсит одну Seer Hut с миссией и наградой. Сложная функция (10 mission types + 10 reward types + deadline + 3 variable sections):
- **Mission** (10 типов):
  0: пусто → +15
  1: "Принести опыт N" → +5
  2: "Достичь primary skills" → +7
  3: "{" + hero_id (special) → +6
  4: "Убить монстра N" → +10
  5: "Принести артефакты (N шт)" → +N*2+4
  6: "Принести монстров (N типов)" → +N*6+4
  7: "Принести ресурсы" → +31
  8: "}" + hero_id (special) → +5
  9: "Победить цвет N" → +4
- **Deadline** (4 байта — закодированная дата)
- **3 variable sections** (u16 + data, для mission items quantities)
- **Reward** (10 типов):
  0: нет → +15
  1: опыт → +15
  2: мана → +15
  3: мораль → +15
  4: удача → +15
  5: ресурс → +15
  6: primary skill → +15
  7: secondary skill → +15
  8: артефакт → +15 (плюс `r_AllArtsRow` запись)
  9: заклинание → +15
  10: монстр → +15

У нас: только `scan_seer_hut_content` (skip-only). Это самый сложный парсер в ProspectorRT.

### A6. `PassGuardContent` (строка 8740) — ⭐ средний приоритет

Парсит один Border Guard / Border Gate. Структура похожа на SeerHut (та же самая функция skip).
У нас: только `scan_pass_guard_content` (skip-only).

### A7. `BankContent` (строка 8902) — ⭐ высокий приоритет

Парсит одну Creature Bank (post-tile секция `map.Bank`, а НЕ тайловая):
- Гард (7 монстров × 4 байта + counts + hp)
- 28 байт ресурсы + золото
- Тип банка (u8): 0xFF = нет награды
- 7 байт для non-artifact bank
- Для artifact bank: u8 count, count × 4 байта (artifact IDs)

Суб-функции:
- `GetBankGuard` (строка 9033) — парсит гард, возвращает строку + HP
- `GetBankResource` (строка 9018) — парсит ресурсы, возвращает строку + gold
- `GetBankMonster` (строка 9013) — парсит монстра-награду

У нас: `_parse_bank` в `tile_scanner.py` (другая структура — там только тип/позиция, не содержимое).

### A8. `GarrisonContent` (строка 8136) — ⭐ средний приоритет

Парсит одну секцию Garrison (post-tile, отдельная от тайлов):
- Гард (7 монстров × 4 байта + counts + hp)
- Цвет владельца (1 байт, 0xFF = нет)
- "CanTake" флаг (1 байт)

У нас: нет отдельного парсера post-tile garrison (только `_parse_garrison` для tile-level, а там другая структура).

### A9. `UniverContent` (строка 8031) — ⭐ низкий приоритет

Парсит University of Magic (4 secondary skills на выбор):
- 4 × 4 байта (skill ID × 4)

У нас: `_parse_scholar` есть, но University — нет.

### A10. `MarketContent` (строка 8055) — ⭐ средний приоритет

Парсит Black Market (7 артефактов на продажу):
- 7 × 4 байта (artifact IDs, 0xFF = пусто)

У нас: Black Market offset (`map.BlackMarket`) НЕ вычисляется в `find_map_start`.
ProspectorRT вычисляет его в `GetMapStart` (строка 9430).

---

## Категория B: Текущие события (current state + art merchants)

### B1. `GetArtMerchants` (строка 7684) — ⭐ средний приоритет

Парсит Art Merchants в секции CurrentState (offset +49):
- 7 × 4 байта (artifact IDs)
- Вызывается из `GetCurrentState` после `currentState -= Chrn`

У нас: `CURRENT_STATE_OFFSETS` покрывает `current_state + 2` (grail) и `+ 11/13/15` (date),
но не Art Merchants (`+ 49`).

### B2. `GetAlliance` (строка 6273) — ⭐ средний приоритет

Парсит alliance команд:
- 8 игроков, ищет последовательность `[0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7]` в сейве
- Определяет `aAlliance[]` — союзники human игрока

Суб-функции:
- `FindAlly` (строка 6309)
- `SumAlly` (строка 6328) — таблица возможных составов команд
- `NumAlly` (строка 6356)
- `IsEquals` (строка 6369)

У нас: alliance/teams парсинг НЕ реализован.

### B3. `GetExperience` (строка 6381) — ⭐ средний приоритет

Агрегатор всей доступной EXP на карте (для каждого объекта вычисляет HP и XP):
- Mine → 5 × count (`text2 + " " + count`)
- Dwelling → (тоже computed)
- Garrison
- EventBox (guard + experience)
- SeerHut (rewards)
- Spell (guard)
- Art (guard)
- Resource (guard)
- Chest (gold + XP)
- Bank
- Monstr (treasure guards)
- Town garrison
- Heroes army

У нас: нет — это чисто агрегационная функция для UI.

---

## Категория C: Timed Events — полностью отсутствует

### C1. `GetTimedEvents` (строка 6857) — ⭐ высокий приоритет

Главный парсер timed events. Подфункции:

### C2. `MapTimedEvents` (строка 6905) — ⭐ высокий приоритет

Парсит map-level timed events:
- u16 count + 4 skip + count × (u16 + 37)
- В каждом:
  - 6 ресурсов × 4 байта (signed, 0xFFFFFF=-1)
  - Золото (4 байта, signed)
  - 4 байта (padding?)
  - 2 байта (Day)
  - 1 байт (Repeat)
  - 3 байта (Apply: human/non-human/computer)
  - 1 байт (ID)

### C3. `TownsTimedEvents` (строка 6877) — ⭐ высокий приоритет

Парсит town-level timed events (более длинная структура — u16 + 60):
- Аналогично Map, но с:
  - 6 байт buildings bitmask (×8 битов = 48 buildings)
  - 14 байт monsters (7 × 2 байта)
  - Apply + ID

### C4. `GetTimerRes` (строка 6956) — ⭐ высокий приоритет

Парсит ресурсы в timed event (negative encoding через `~byte`):
```csharp
if (decmp[s + i * 4 + 3] != 0xFF)  // positive
    num = (decmp[s + i * 4 + 2] << 16) + (decmp[s + i * 4 + 1] << 8) + decmp[s + i * 4];
else  // negative — bit-not + 1 trick
    num = ((~decmp[s + i * 4 + 2] << 16) + (~decmp[s + i * 4 + 1] << 8) + ~decmp[s + i * 4] + 1) * -1;
```

### C5. `GetTimerContent` (строка 6931) — ⭐ средний приоритет

Парсит building bitmask + monster quantities в town timed event:
- 6 байт (6 building bytes) — но ProspectorRT читает только первые 6
- 14 байт (7 × 2 байта) — monsters count

### C6. `GetTimerTown` (строка 6984) — ⭐ средний приоритет

Связывает timed events с towns по ID, декодирует buildings через `TblBuilding` (по 5 байт × 8 битов + byte 6).

### C7. `GetTimerApply` (строка 7124) — ⭐ средний приоритет

Декодирует Apply flags (3 числа) в имена игроков, которые получат timed event.

### C8. `GetEventApply` (строка 7088) — ⭐ средний приоритет

Декодирует Apply flags в EventBox (2 числа → players, кто получит).

У нас: **полностью отсутствует** весь блок timed events (C1-C8).

---

## Категория D: Town spell pool — частично отсутствует

### D1. `GetTownSpell` (строка 7542) — ⭐ высокий приоритет

Сложная функция (200 строк) — парсит спеллы в Magic Guild конкретного town:
- 6 уровней гильдии (Castle=4, Tower=6, Stronghold/Fortress=3, остальные=5) — У НАС ЕСТЬ в `TOWN_SPELL_POOL_DEPTH`
- Проверка Library (Tower): бит 0x40 в `decmp[s - 6]`
- Проверка Mage Guild lvl 5: бит 0x40 в `decmp[s - 14]`
- 6 спеллов на уровень (максимум)
- Спец-логика для уровня 6 (только Library доп. спелл)

У нас: `TOWN_SPELL_POOL_DEPTH` константа есть в `save_layout.py`,
но **парсера спеллов нет**. Только hero spellbook в `parse_hero_block`.

---

## Категория E: Hero artifacts — doll placement

### E1. `ArtDollPlace1` (строка 7883) — ⭐ средний приоритет

Для героев на карте (с известной позицией x.y.z):
- Создаёт/обновляет запись в `R_AllArts`
- Прописывает artifact position на doll (head/neck/armor и т.д.)

### E2. `ArtDollPlace2` (строка 7935) — ⭐ средний приоритет

Для героев в таверне / prison — та же логика, но без привязки к карте.

### E3. `GetDollPlace` (строка 7948) — ⭐ средний приоритет

Заполняет `R_AllArtsRow` для артефакта с конкретной позицией doll (slot, slot_id).

У нас: `parse_hero_block` возвращает `equipment[19]` (19 слотов × artifact_id + data),
но **doll placement не декодируется** — артефакты идут в общем списке без мест.

---

## Категория F: Prison heroes

### F1. `GetPrisonHero` (строка 7983) — ⭐ средний приоритет

Парсит prison hero (герой в тюрьме на карте):
- Берёт `R_PrisonRow` (созданный в `SavePrison` при tile scan)
- Ищет `R_HeroesRow` по ID
- Копирует hero → prison row (level, primary skills, secondary, art, machine, book, spells, monster, MP, exp)

У нас: `_parse_prison` есть в tile_scanner.py (читает только hero_id),
но **связки prison → hero record нет**.

---

## Категория G: Pairing / Topology

### G1. `GetPairSubterraneanGate` (строка 9335) — ⭐ низкий приоритет

Сопоставляет пары Subterranean Gates (вход ↔ выход):
- Читает `map.SubTerGate` — u16 count + count × 4 байта (x,y,z,bits)
- Читает `map.SubTerGatePair` — u16 count + count × 4 байта (pair_id или 0xFFFF)
- Двойной цикл — находит пары и записывает их в `R_Topology.Pair`

### G2. `ScanMonolithWhirlpool` (строка 9392) — ⭐ средний приоритет

Пропускает секцию монолитов/водоворотов (3 подсекции × 8 объектов):
- 8 × u16 count + count × 4 байта — One Way Monoliths (типы A)
- 8 × u16 count + count × 4 байта — Two Way Monoliths
- 1 × u16 count + count × 4 байта — Whirlpools
- Вычисляет `map.OneWayMonolith` и `map.Whirlpool`

У нас: `walk_post_tile_sections` только вычисляет `map.TwoWayMonolith` как `BitField + size² × (2 + 2×MapSide)`,
но `OneWayMonolith` / `Whirlpool` / `SubTerGate` / `SubTerGatePair` **не парсятся**.

---

## Категория H: Header — недостающие смещения

### H1. `map.BlackMarket` (строка 9430 в `GetMapStart`) — ⭐ высокий приоритет

В `GetMapStart` ProspectorRT находит Black Market section:
```csharp
num = decmp[s];
if (num != 0) {
    map.BlackMarket = s;
    s += num * 28;
}
```
Смещение нужно для `GetMarketContent` (Black Market парсер).

У нас: `find_map_start` в `tile_scanner.py` находит только `map.Start`, не `BlackMarket`.

### H2. `map.SaveName` (строка 9416) — ⭐ низкий приоритет

Сохранённое имя сейва (строка перед `map.SR`).

### H3. `map.SR` (строка 9419) — ⭐ низкий приоритет

Script/Random секция. Суб-функция `Get_SR` (строка 14045) парсит 28 байт какой-то мета-информации (возможно RNG seed / handicap / victory conditions).

У нас: не реализовано.

### H4. `map.Teams` (строка 9460) — ⭐ средний приоритет

Смещение sequence `[0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7]` — это alliance setup. Нужно для `GetAlliance`.

### H5. `map.MapName` (строка 9461) — ⭐ низкий приоритет

Имя карты в сейве (341 байт после `map.Teams`).

---

## Категория I: Aggregators (низкий приоритет, нужны только для UI)

### I1. `GetAllSpell` (строка 7263) — ⭐ низкий приоритет

Агрегирует ВСЕ спеллы на карте (из EventBox, Scholar, Spell, SeerHut, Town, Heroes) в единую `R_AllSpell` таблицу. Нужна для отображения в GUI.

### I2. `GetAllSkill` (строка 7160) — ⭐ низкий приоритет

Агрегирует ВСЕ secondary skills (из EventBox, Scholar, SeerHut, Skill objects) в `R_AllSkill`.

### I3. `AnalysisMonstrContent` (строка 6801) — ⭐ средний приоритет

Вызывает `MonstrContent` для каждой записи в TblMonstr (заполненной во время tile scan).

### I4. `SeerHutContent2()` — ⭐ средний приоритет

Вторая фаза SeerHut (post-processing — связывает artifacts в SeerHut rewards с `R_AllArts`).

### I5. `PassGuardContent2()` — ⭐ средний приоритет

Вторая фаза PassGuard (post-processing).

---

## Категория J: Editing / Save writing — полностью отсутствует

В `MainForm.cs` ProspectorRT:
- `Save*` функции (с 9665 по 10570) — НЕ для записи в сейв, а для регистрации объектов в DataTable (read-only parsing на самом деле)
- Реальная запись в сейв — через direct editing `decmp[]` array, затем gzip-compress обратно

В `HeroesInfo.exe` (другой декомпилированный редактор):
- `SaveArt` (точное имя?) — запись артефакта в hero block
- `SaveArmy` — запись армии героя/города
- `SaveSkill` — запись secondary skills героя

У нас: **полностью отсутствует** функциональность записи в сейв. Только чтение.

---

## Категория K: Reference tables — не нужны

Все `CreateTbl*` функции (lines 10589-13385):
- `CreateTblArt`, `CreateTblSpell`, `CreateTblMonster`, `CreateTblSecondarySkill`
- `CreateTblBuilding`, `CreateTblDwelling`, `CreateTblIdeology`, `CreateTblObject`
- `Create_PW`, `Create_SW` (стандартные SoD/HotA таблицы)
- `CreateTblMarket`, `CreateTblUniver`, `CreateTblBanks`, `CreateTblGarrison`
- `CreateTblEventBox`, `CreateTblMonstr`, `CreateTblSeerHut`, `CreateTblPassGuard`
- `CreateTblArtRes`, `CreateTblDwelling`

У нас: это заменено статическим `03_object_mapping/object_types_dictionary.json` (2037 типов из LazyLlama wiki).

---

## Сводка приоритетов

### ⭐ Высокий (нужно для базового редактора сейвов):
1. **A2 `EventBoxContent`** — Pandorabox contents
2. **A3 `ArtResContent`** — Art/Resource/Spell с гардом
3. **A4 `MonstrContent`** — Monster treasures
4. **A5 `SeerHutContent`** — Seer Hut missions & rewards (10+10 типов)
5. **A7 `BankContent`** — Creature Banks
6. **C2 `MapTimedEvents`** + **C3 `TownsTimedEvents`** + **C4 `GetTimerRes`**
7. **D1 `GetTownSpell`** — Magic Guild spells (константа уже есть)
8. **H1 `map.BlackMarket`** — offset для A10

### ⭐ Средний (нужно для полного просмотра):
- A6, A8, A10, B1, B2, B3, C5-C8, E1-E3, F1, G2, H4, I3-I5

### ⭐ Низкий (UI niceties):
- A9, G1, H2, H3, H5, I1, I2

### ❌ Совсем не нужно (заменено другим подходом):
- Категория K (CreateTbl*) — у нас есть `object_types_dictionary.json`
- Категория J (Editing) — нужна только для шага 4 (write)

---

## Как использовать этот документ

Каждый пункт имеет:
1. **Ссылку на строку** в `07_prt_decompiled/ProspectorRT_source/ProspectorRT/MainForm.cs`
2. **Сравнение** с тем, что уже есть в `01_tools/`
3. **Приоритет** для планирования следующих шагов

Реализация каждого парсера — это в среднем 30-80 строк Python (перевод с C# на Python):
- Скопировать функцию из MainForm.cs
- Заменить `decmp[s+i]` на `raw[s+i]`
- Заменить DataTable rows на dict / dataclass
- Сохранить результат в `parsed_save`

Главный кандидат на реализацию — `02_format_docs/PRT_offset_findings.md` уже содержит
ключевые смещения. Новые парсеры добавляем в `01_tools/post_tile_scanner.py` (он уже имеет
`scan_*` версии — нужно добавить `parse_*` версии рядом).
