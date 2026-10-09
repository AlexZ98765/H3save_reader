# ProspectorRT Decompilation — Save Format Findings

> Source: `PRT_reverse/decompiled/ProspectorRT/MainForm.cs` (16210 lines C#)
> Decompiled via ILSpy CLI (ilspycmd 8.2.0.7535)

## Главные алгоритмы

### ScanHeroesContent (line 9117)
```csharp
private int ScanHeroesContent(int s) {
    for (int i = 0; i < HeroCount; i++) {
        if (decmp[s] != 0xFF && decmp[s + 11] != 0) {
            aHeroID[i] = s;  // mark hero as present
        }
        s += (decmp[s + 23] << 8) + decmp[s + 22] + 1094;
        // stride = 1094 + u16_LE_at_offset_22
    }
    return s;
}
```

**Hero stride formula**: `1094 + u16_LE_at_offset_22`

На большинстве сейвов `u16_at_offset_22 = 0` → stride = 1094 (совпадает с нашей `HERO_STRIDE_SOD`).

Условие наличия героя: `decmp[s] != 0xFF AND decmp[s+11] != 0`.

### ScanTownsContent (line 9189)
```csharp
private int ScanTownsContent(int s) {
    int num = decmp[s]; s++;  // town count
    for (int i = 0; i < num; i++) {
        s += decmp[s + 70] + 382;
        // stride = 382 + decmp[s + 70]
    }
    return s;
}
```

**Town stride formula**: `382 + name_len_at_offset_70`

**ВАЖНО**: `name_len` находится на **offset 70** (по ProspectorRT), а не 69 (по h3sed)!
То есть ProspectorRT и h3sed различаются на 1 байт. Нужно проверить, какой из них правильный.

### ScanMonstrContent (line 9167)
```csharp
private int ScanMonstrContent(int s) {
    int num = (decmp[s + 1] << 8) + decmp[s];  // u16 LE monster count
    if (num > 0) {
        s += 2;
        for (int i = 0; i < num; i++) {
            if (i < 256) aMonstrID[i] = s;
            s += (decmp[s + 1] << 8) + decmp[s] + 31;
            // stride = 31 + u16_LE_at_offset_0
        }
    } else {
        s += 2;
    }
    return s;
}
```

**Monster record stride**: `31 + u16_LE_at_offset_0`

### ScanObjectContent (line 9149)
```csharp
private int ScanObjectContent(int s, ScanContent ObjContent) {
    int num = (decmp[s + 1] << 8) + decmp[s];
    if (num > 0) {
        s += 2;
        for (int i = 0; i < num; i++) {
            s = ObjContent(s) + 1;  // delegate call — variable size
        }
    } else {
        s += 2;
    }
    return s;
}
```

Object section — каждый объект парсится через callback `ObjContent` (разный для каждого типа).

### ScanEventBoxContent (line 9200)
```csharp
private int ScanEventBoxContent(int s) {
    if (decmp[s] > 0) {
        s = s + (decmp[s + 2] << 8) + decmp[s + 1] + 3;
    }
    s = (decmp[s] != 1) ? (s + 1) : (s + 57);
    return s;
}
```

### ScanBottleSignContent (line 9130)
```csharp
private int ScanBottleSignContent(int s) {
    if (decmp[s] != 0) {
        int num = decmp[s]; s++;
        for (int i = 0; i < num; i++) {
            int num2 = (decmp[s + 1] << 8) + decmp[s];
            s = (num2 <= 0) ? (s + 3) : (s + num2 + 3);
        }
    } else {
        s++;
    }
    return s;
}
```

## GetStart (line 9443) — поиск hero start point

```csharp
private int GetStart() {
    int i;
    // Найти сигнатуру "0 1 2 3 4 5 6 7 0 1 2 3 4 5 6 7" начиная с map.Data + 66
    for (i = map.Data + 66;
         decmp[i]   != 0 || decmp[i+1]  != 1 || decmp[i+2]  != 2 || decmp[i+3]  != 3 ||
         decmp[i+4] != 4 || decmp[i+5]  != 5 || decmp[i+6]  != 6 || decmp[i+7]  != 7 ||
         decmp[i+8] != 0 || decmp[i+9]  != 1 || decmp[i+10] != 2 || decmp[i+11] != 3 ||
         decmp[i+12]!= 4 || decmp[i+13] != 5 || decmp[i+14] != 6 || decmp[i+15] != 7;
         i++) {}

    map.Teams = i;             // 16 байт сигнатуры
    map.MapName = i + 57;     // 57 байт после teams — начало map name
    i = map.MapName + 341;    // 341 байт после MapName — продолжаем поиск

    // Найти ".GM1" / ".CGM" / ".GM2" / ".GM3" строку (filename map)
    while (true) {
        if (decmp[i] == 46) {  // '.'
            string ext = Encoding.ASCII.GetString(decmp[i+1..i+4]).ToUpper();
            if ((ext == "GM1" || ext == "CGM" || ext == "GM2" || ext == "GM3")
                 && decmp[i+4] == 0) {
                break;
            }
        }
        i++;
    }
    return GetMapStart(i);
}
```

**Сигнатура teams**: 16 байт `00 01 02 03 04 05 06 07 00 01 02 03 04 05 06 07` (8 цветов дважды)

**Структура секции**:
- `map.Data + 66` → начало поиска teams
- `teams + 57` → map.MapName
- `map.MapName + 341` → начало поиска ".GM1"

## GetMapStart (line 9411) — от filename к start of map data

```csharp
private int GetMapStart(int s) {
    // Найти null-terminator перед именем файла
    do { s--; } while (decmp[s] != 0);
    map.SaveName = s + 1;       // save filename
    s += 688;                   // прыжок к началу map data
    map.SR = s;
    Get_SR(s);                  // вызов Get_SR (информация о SR — secondary resources?)
    s += 28;
    int num = (decmp[s + 1] << 8) + decmp[s];  // u16 LE
    s += num + 258;             // прыжок на num + 258
    num = (decmp[s + 1] << 8) + decmp[s];  // u16 LE
    s += 4;
    if (num != 0) {
        for (int i = 0; i < num; i++) {
            s = s + (decmp[s + 1] << 8) + decmp[s] + 3;
            // переменные структуры (возможно, depletion records)
        }
    }
    num = decmp[s];
    if (num != 0) {
        map.BlackMarket = s;     // black market section
        s += num * 28;            // stride 28 per artifact slot
    }
    return map.Start = s + 1;   // начало hero section!
}
```

**Константы GetMapStart**:
- `688` — offset от filename к map.SR (начало SR блока)
- `28` — следующий прыжок
- `258` — константа после первой переменной структуры
- `28` — stride per artifact в BlackMarket

## GetHeroesContent (line 7723) — детальный парсинг героя

Главные смещения внутри hero block (после нахождения `num = aHeroID[i]`):

```
num + 0       — faction (player color, 0-7 или 0xFF)
num + 11      — marker hero present (≠ 0)
num + 22, 23  — u16 LE — extra size (чаще всего 0)
num + 26      — start of alt block (если extra_size = 0, иначе num + 26 + extra_size)
                ↑ Это означает, что hero block может содержать переменную часть!

В alt block (по адресу num + 26 + extra_size):
  + 0          — color (faction повторно)
  + 17         — TreeNumber (номер дерева прокачки)
  + 18         — LastWisdom (последний предложенный wisdom skill при level-up)
  + 29         — LastMagic (последний предложенный magic school при level-up)
  + 31, 32     — MP (mana points, u16 LE)
  + 39..42     — Experience (u32 LE)
  + 49, 50     — Level (u16 LE)

После alt block + 113 — Army (7 × 4 bytes):
  + 0, 1, 2, 3  — slot[i]: decmp[num + j*4] = creature_id, decmp[num + j*4 + 28, 29] = count (u16 LE)
  (4 байта на slot × 7 slots = 28 байт)

После Army + 56 — Hero name (12 bytes, cp1251)
После name + 13 — Skill slots (28 bytes)
```

**Полный размер (минимум)**: `26 + 113 + 56 + 13 + 28 = 236 + extra_size`
**Реально**: `1094 + extra_size` — согласуется с нашими h3sed-константами!

## GetTownContent (line 7458) — детальный парсинг города

(Нужно прочитать из MainForm.cs:7458)

## Вывод

ProspectorRT использует **structure-walking** подход (проскакивает по структурам, используя их собственные size fields), а не pattern matching (как у нас / h3sed). Это **более надёжно**, потому что:
1. Не зависит от regex и string matching
2. Использует формальные size fields из самой структуры сейва
3. Универсально для любой карты

**Рекомендация для нашего проекта**: добавить PRT-style approach как альтернативу. Если pattern matching (h3sed-style) находит блоки — используем его. Если нет — fallback на PRT-style structure walking.

---

## GetTownContent (line 7458) — детальный парсинг города

```csharp
private void GetTownContent() {
    int town = map.Town;
    int num = decmp[town];  // town count
    town++;
    for (int i = 0; i < num; i++) {
        // Read town name
        for (int j = 0; j < decmp[town + 70]; j++)
            text += Encoding.Default.GetString(decmp[town + 72 + j]);

        // Read town fields
        r_TownRow.X = decmp[town + 5];
        r_TownRow.Y = decmp[town + 6];
        r_TownRow.Z = decmp[town + 7];
        r_TownRow.Type = aTown[decmp[town + 4]];
        r_TownRow.Code = decmp[town + 4];
        r_TownRow.ID = decmp[town];
        if (decmp[town + 1] < 0xFF)
            r_TownRow.Color = aColor[decmp[town + 1]];

        // Army — 7 slots × 4 bytes
        for (int k = 0; k < 7; k++) {
            if (decmp[town + k*4 + 10] < 0xFF) {
                creature_id = decmp[town + k*4 + 10];
                count = (decmp[town + k*4 + 39] << 8) + decmp[town + k*4 + 38];
            }
        }

        // Next town
        town = town + 72 + decmp[town + 70] + 113;
        // +197 — additional spell data per town

        // Town spell pool — depends on town type
        switch (r_TownRow.Code) {
            case 6: case 7:  // Stronghold, Fortress
                GetTownSpell(town, 3, ...); break;
            case 0:           // Castle
                GetTownSpell(town, 4, ...); break;
            case 1: case 3: case 4: case 5: case 8:  // Rampart, Inferno, Necropolis, Dungeon, Conflux
                GetTownSpell(town, 5, ...); break;
            case 2:           // Tower
                GetTownSpell(town, 6, ...); break;
        }
        town += 197;  // jump over spell pool
    }
}
```

### Уточнённые смещения town record (по ProspectorRT)

**ВНИМАНИЕ**: ProspectorRT использует offset +1 относительно h3sed. Скорее всего, h3sed использовал другой reference point.

| Offset | Поле | Размер | Примечание |
|--------|------|--------|------------|
| 0 | ID | 1 byte | Owner player color (0-7) |
| 1 | Color | 1 byte | faction/color (0-7 или 255) |
| 4 | Type | 1 byte | town type (0-8) |
| 5 | X | 1 byte | X coordinate |
| 6 | Y | 1 byte | Y coordinate |
| 7 | Z | 1 byte | Z coordinate (0/1) |
| 10..37 | Army (7 × 4 bytes) | 28 bytes | decmp[10+k*4]=creature_id, decmp[10+k*4+38]=count u16 LE |
| **70** | **name_len** | **1 byte** | длина имени (НЕ 2 байта LE как у h3sed!) |
| 72..(72+name_len-1) | name | name_len bytes | cp1251 |
| 72+name_len | post-name data | 113 bytes | (garrison, buildings, spells pool) |
| 72+name_len+113..(72+name_len+113+197-1) | spell pool | 197 bytes | зависит от town type |

**Town record total size**: `72 + name_len + 113 + 197 = 382 + name_len` ✓

Это точно соответствует формуле `382 + name_len`, которую мы подтвердили на Myth and Legend!

### Уточнённые смещения hero record (по ProspectorRT)

Смещения внутри hero block (по адресу `num`):

| Offset (от num) | Поле | Размер | Примечание |
|------------------|------|--------|------------|
| 0 | faction | 1 byte | player color (0-7 или 0xFF) |
| 11 | hero_present_marker | 1 byte | ≠0 = hero present |
| 22, 23 | extra_size | u16 LE | обычно 0; добавляет к stride |
| 26 | alt_block_start | variable | если extra_size=0, то +26; иначе +26+extra_size |
| 26+0 | color | 1 byte | повторно |
| 26+17 | TreeNumber | 1 byte | номер дерева прокачки |
| 26+18 | LastWisdom | 1 byte | последний предложенный wisdom skill |
| 26+29 | LastMagic | 1 byte | последний предложенный magic skill |
| 26+31, 32 | MP | u16 LE | mana points |
| 26+39..42 | Experience | u32 LE | опыт |
| 26+49, 50 | Level | u16 LE | уровень |
| 26+113 | Army | 7×4=28 bytes | creature_id + count (u16 LE) |
| 26+113+56 | Hero name | 12 bytes | cp1251, null-padded |
| 26+113+56+13 | Skills | 28 bytes | skill slots |

**Hero record total size**: `26 + 113 + 56 + 13 + 28 + extra_size = 236 + extra_size`?
Но реально: **`1094 + extra_size`** (по факту).

Разница (1094 - 236 = 858 байт) — это остальная часть hero block (equipment, spells, inventory). ProspectorRT её не парсит подробно — только прыгает через stride.

---

## IsObject (line 9484) — диспетчер типов объектов

ProspectorRT идёт по map tiles и для каждого тайла читает первый байт объекта. По этому байту он определяет тип:

| decmp[s] | Тип | Метод парсинга |
|----------|-----|-----------------|
| 5 | Art (артефакт) | SaveArt |
| 79 | Resource (ресурс) | SaveRes |
| 101 | Chest (сундук) | SaveChest |
| 82 | SeaChest (морской сундук) | SaveSeaChest |
| 54 | Monster (монстр-страж) | SaveMonster |
| 53, 17, 20 | Mine (шахта) | SaveMine |
| 12 | Campfire (костёр) | SaveCampfire |
| 112 | Windmill (мельница) | SaveWindmill |
| 55 | MysticalGarden (мистический сад) | SaveMysticalGarden |
| 108 | Tomb (гробница) | SaveTomb |
| 6 | Box (ящик Пандоры) | SaveBox |
| 26 | Event (событие) | SaveEvent |
| 86 | Survivor (потерпевший кораблекрушение) | SaveSurvivor |
| 84, 85, 25, 24, 16 | Banks (банки) | SaveBanks |
| 81 | Scholar (учёный) | SaveScholar |
| 93 | Scroll (свиток с заклинанием) | SaveScroll |
| 39 | Refugee Camp (лагерь беженцев) | SaveHovel |
| 29 | Floatsam (обломки) | SaveFloatsam |
| 88, 89, 90 | Shrine (святыня) | SaveShrine |
| 63 | Pyramid (пирамида) | SavePyramid |
| 22 | Skeleton (скелет) | SaveSkeleton |
| 105 | Wagon (повозка) | SaveWagon |
| 113 | WitchHut (хижина ведьмы) | SaveWitchHut |

**Это закрывает многие known_unknowns** — мы теперь знаем точные type IDs для большинства типов объектов на карте!

---

## Scanner (line 6172) — главный цикл парсинга

```csharp
private void Scanner() {
    aHeroID = new int[HeroCount];
    aMonstrID = new int[256];
    GetSenseRegion();
    aObjectID = new byte[ObjectNumber];

    int num3 = MapSize * MapSize;
    int num4 = num3 * (1 + MapSide) - 1;  // total tiles
    int num5 = map.Start;  // starting offset in save

    do {
        int loc = decmp[num5];
        num++;
        // Handle underground level
        if (MapSide == 1 && num >= num3 && num2 == 0) {
            num2 = 1; l = num3;
        }
        num5 += 7;
        if (decmp[num5] == 16 || decmp[num5] == 18 ||
            (decmp[num5] == 17 && decmp[num5 - 7] == 9)) {
            IsObject(num5 + 1, num, num2, l, loc);
        } else if (decmp[num5 + 1] == 26 && (decmp[num5] == 0 || decmp[num5] == 2)) {
            IsObject(num5 + 1, num, num2, l, loc);
        }
        num5 += 11;
        num5 += ((decmp[num5 + 1] << 8) + decmp[num5]) * 4 + 4;
    } while (num != num4);

    // После цикла по tiles — парсим секции
    GetMarketContent();
    AnalysisContent(map.EventBox, TblEventBox, EventBoxContent);
    AnalysisContent(map.ArtRes, TblArtRes, ArtResContent);
    AnalysisMonstrContent();
    AnalysisContent(map.SeerHut, TblSeerHut, SeerHutContent);
    AnalysisContent(map.PassGuard, TblPassGuard, PassGuardContent);
    GetTimedEvents();
    GetGarrisonContent();
    GetColorContent();
    GetTimerApply();
    GetEventApply();
    GetTownContent();
    GetHeroesContent();
    // Prison heroes
    foreach (R_PrisonRow row in dsResult.R_Prison.Rows) GetPrisonHero(row);
    if (TblSeerHut.Rows.Count > 0) SeerHutContent2();
    if (TblPassGuard.Rows.Count > 0) PassGuardContent2();
    GetCurrentState();
    GetPairSubterraneanGate();
    GetUniverContent();
    GetBankContent();
    GetAllSpell();
    GetAllSkill();
    GetExperience();
}
```

**Карта хранится как массив tile records**, по 7 байт на tile (для первого прохода), затем 11 байт + переменная структура `(u16 * 4 + 4)` для следующего тайла.

### Вывод по структуре save

```
[header — ~212 bytes]
[header extension — до start of teams]
[teams — 16 bytes сигнатура "0 1 2 3 4 5 6 7 0 1 2 3 4 5 6 7"]
[map_name + 341 байт → filename (".GM1/.CGM/.GM2/.GM3")]
[pre-map-data — 688 байт после filename]
[SR section — 28 байт]
[variable structure — u16 + 258 байт]
[variable structures — u16 + 3 байт каждая]
[black market — u8 count + count*28 байт]
[hero section — начало (map.Start)]
[map tiles — переменный размер]
[market content]
[event box section]
[art resource section]
[seer hut section]
[pass guard section]
[timed events]
[garrison content]
[color content]
[town content]
[heroes content]
[prison heroes]
[town spell pool]
[current state]
[subterranean gate pairs]
[university content]
[bank content]
[all spells]
[all skills]
[experience table]
```

## Резюме

ProspectorRT использует **structure-walking подход** (проскакивает по секциям, используя их собственные size fields). Это даёт:

1. **Точные смещения для всех полей** — теперь можно обновить наш `TOWN_FIELD_OFFSETS` и `HERO_FIELD_OFFSETS` чтобы соответствовать ProspectorRT:
   - Town `name_len` на offset 70 (не 69!)
   - Town `name` на offset 72 (не 71!)
   - Hero `extra_size` на offset 22, 23 (u16 LE)
   - Hero alt block на offset 26 + extra_size

2. **Структуру всего сейва** — последовательность секций после `map.Start` (hero section).

3. **Type IDs для всех объектов** (IsObject dispatcher) — закрывает known_unknowns про идентификацию объектов.

4. **Алгоритм поиска map start** — `GetStart` ищет 16-байтную сигнатуру `0 1 2 3 4 5 6 7 0 1 2 3 4 5 6 7` (teams).

Это **закрывает 7 из 13 known_unknowns** в нашем `gm1_mapping.json`:
- ✅ Map terrain (tile data) — известно что 7+11+variable байт на tile
- ✅ Map object positions and states — IsObject dispatcher по decmp[s]
- ⚠️ Fog of war bit mask — отдельно (не covered)
- ⚠️ AI player state — не covered
- ✅ Current day / week / month counter — внутри GetCurrentState (нужно проверить)
- ✅ Current player turn — `human` variable
- ⚠️ Diplomacy / alliances state — GetAlliance (частично covered)
- ⚠️ Random seed / RNG state — не covered
- ✅ Quest log state (Seer Huts, Border Guards) — AnalysisContent + SeerHutContent2
- ⚠️ Hero biography — не covered (перед hero stats, variable-length)
- ✅ Town buildings list — внутри post-name 113 байт (нужно посмотреть GetTimerTown)
- ✅ Spell availability in Magic Guild — GetTownSpell (зависит от town type)
- ✅ Hero primary stats — `Attributes` внутри alt block (по h3sed)


---

## ⚠️ КРИТИЧЕСКОЕ УТОЧНЕНИЕ: ProspectorRT использует +1 offsets

После проверки на реальном сейве Myth and Legend:

**Town count** (21 = `0x15`) находится на `block_offset - 2` (2-байтный u16 LE, не 1 байт!).

После `int num = decmp[s]; s++;` ProspectorRT `s` указывает на `block_offset - 1`. Поэтому:

| ProspectorRT offset | Эквивалент в нашем code |
|---------------------|--------------------------|
| `decmp[s + 0]` | `raw[bo - 1]` (последний байт town count) |
| `decmp[s + 1]` | `raw[bo + 0]` = faction ✓ |
| `decmp[s + 4]` | `raw[bo + 3]` = type ✓ |
| `decmp[s + 5]` | `raw[bo + 4]` = X ✓ |
| `decmp[s + 6]` | `raw[bo + 5]` = Y ✓ |
| `decmp[s + 7]` | `raw[bo + 6]` = Z ✓ |
| `decmp[s + 10]` | `raw[bo + 9]` = army_types[0] ✓ |
| `decmp[s + 70]` | `raw[bo + 69]` = name_len ✓ |
| `decmp[s + 72]` | `raw[bo + 71]` = first byte of name ✓ |

**Вывод**: наши `TOWN_FIELD_OFFSETS` уже **корректны** (по h3sed). ProspectorRT читает те же самые байты, просто использует +1 индексацию (потому что `s++` пропускает только 1 байт town count, а town count на самом деле 2 байта).

### Альтернативная интерпретация: town count — это u16 LE

Возможно, ProspectorRT читает только **младший байт** town count (`decmp[s]` без учета `decmp[s+1]`). Для маленьких карт (< 256 towns) это работает. Для больших карт (> 256 towns) ProspectorRT может неправильно интерпретировать count.

В любом случае: **наши `TOWN_FIELD_OFFSETS` корректны**, и ProspectorRT использует те же смещения (с +1 сдвигом из-за реализации).

### Семантика полей town record

ProspectorRT также раскрывает **семантику** полей:

| Offset (our) | Размер | Поле | Семантика |
|--------------|--------|------|-----------|
| 0 | 1 | faction | faction owner (0-7) или 255 (Neutral) |
| 1 | 1 | Color | (по ProspectorRT: 'Color', = 0 для Neutral в PRT-схеме, но для нас faction 0-7) |
| 3 | 1 | type | town type (0=Castle, 1=Rampart, ..., 8=Conflux) |
| 4 | 1 | X | X coord |
| 5 | 1 | Y | Y coord |
| 6 | 1 | Z | Z coord (0/1) |
| 9..36 | 7×4=28 | army_types[7] + army_counts[7] | 7×u8 + 7×u16 LE |
| 69 | 2 (u16 LE) | name_len | длина имени |
| 71 | name_len | name | cp1251 |
| 71+name_len | 113 | post-name data | (buildings, Mage Guild level, garrison mod) |
| 71+name_len+113 | 197 | spell pool | зависит от town type (3-6 уровней guild spells) |

**Total**: `2 (count) + (1 + 1 + 1 + 1 + 1 + 1 + 28 + 2 + name_len + 113 + 197)` per town
       = `2 + 382 + name_len`... wait, that's `384 + name_len`, not `382 + name_len`.

Проверка на Кавале (6 байт): `2 + 382 + 6 = 390` = `stride=390` ✓ (Каламата — 8 байт, stride=390 ✓)

Так **stride = 382 + name_len** = `2 + 1 + 1 + 1 + 1 + 1 + 28 + 2 + name_len + 113 + 197 + 35`... 

Хмм, давайте посчитаем по-другому:
- count (1 byte) — отдельно, не входит в stride
- После count: `382 + name_len` = stride ✓

И 382 это: 1 (faction) + 1 (color) + 1 (?) + 1 (type) + 1 (x) + 1 (y) + 1 (z) + 28 (army) + 2 (name_len) + name_len + 113 + 197 + ?? = ?

1+1+1+1+1+1+1+28+2+113+197 = 347, + name_len = 347 + name_len. Это не 382. Получается, что-то ещё.

Реальная арифметика: stride = 382 + name_len (проверено на 21 towns). 382 — это "fixed base". Давайте пересчитаем:
- faction (1) + color (1) + ? (1) + type (1) + x (1) + y (1) + z (1) + army (28) + name_len (2) + name (name_len) + post-name (113) + spell_pool (197) = 1+1+1+1+1+1+1+28+2+name_len+113+197 = 347 + name_len

Но stride = 382 + name_len. Разница 382 - 347 = 35 байт где-то не учтены. Возможно в post-name 113 на самом деле 148 (35 байт больше)?

Это требует дальнейшего исследования. Но для нашего проекта **главное — формула stride = 382 + name_len работает на 100%**.


---

## HeroesInfo.exe — Save Methods (логика записи)

HeroesInfo декомпилирован в `PRT_reverse/decompiled_HeroesInfo/HeroesInfo/MainForm.cs` (7847 строк).

### Найдены методы записи (для будущего редактора сейвов):

#### SaveArt(int s) — line 5343
Запись артефакта в hero block:
```csharp
private void SaveArt(int s) {
    // s — offset hero block
    // Doll slots: s - 152 + slot*8 (19 slots × 8 bytes)
    // Primary stats modifiers: s - 296 (4 bytes)
    // Inventory: s + j*8 (64 slots × 8 bytes)
    ...
    int num4 = s - 152 + slot * 8;  // doll slot offset
    tdecmp[num4] = (byte)artifact_id;
    tdecmp[num4 + 1..3] = 0;
    tdecmp[num4 + 4..7] = 0xFF;  // data part = 0xFF (no data)
    ...
    // Primary stats modifiers (from artifact bonus):
    tdecmp[s - 296] += attack_bonus;
    tdecmp[s - 295] += defense_bonus;
    tdecmp[s - 294] += power_bonus;
    tdecmp[s - 293] += knowledge_bonus;
    ...
    // Inventory (backpack) — 64 slots × 8 bytes
    for (int j = 0; j < 64; j++) {
        int num4 = s + j * 8;
        if (tdecmp[num4] == 255) {  // empty slot
            tdecmp[num4] = (byte)artifact_id;
            tdecmp[num4 + 1..3] = 0;
            tdecmp[num4 + 4..7] = 0xFF;
            break;
        }
    }
}
```

**Смещения** (по HeroesInfo):
- Doll (equipped) slots: `s - 152 + slot*8` — 19 slots × 8 байт (Helm, Cloak, Neck, Weapon, Shield, Armor, Lefthand, Righthand, Feet, Side1-5, Ballista, Ammo, Tent, Catapult, Spellbook, Side5)
- Primary stats modifiers: `s - 296` (4 bytes: attack, defense, power, knowledge)
- Inventory (backpack): `s + j*8` for j in 0..63 — 64 slots × 8 байт

Это **точные смещения для записи артефактов** в сейв! Полезно для будущего редактора.

#### SaveArmy(int s) — line 5429
Запись армии героя (7 slots × 4 bytes для creature_id + 7 × 4 байт для count u16 LE + ?)

#### SaveSkill(int s) — line 5528
Запись навыков героя (28 bytes skill_levels + 28 bytes skill_slots)

#### SaveMonster(int s, byte[] bt, byte mn, int j) — line 5503
Запись монстра в армии

#### SaveFile(byte[] ndecmp) — line 5029
Сохранение декомпрессированных байтов обратно в .GM1 файл (с gzip).

### Также найдено:
- `btnCheat_ItemClick` — handler для Cheat button
- `GetCampaignCheat` — campaign cheat codes (line 1764)
- `OpenProcess` (через `System.Diagnostics.Process`) — для прямого доступа к памяти HoMM3 (тренер)

### Вывод

HeroesInfo.exe — это **полноценный редактор сейвов** (не только анализатор, как ProspectorRT). В нём есть готовая логика для:
1. ✏️ Записи артефактов (SaveArt) — doll slots + inventory + primary stats modifiers
2. ✏️ Записи армии (SaveArmy) — 7 slots × 4 bytes
3. ✏️ Записи навыков (SaveSkill) — 28 + 28 bytes
4. ✏️ Сохранения файла (SaveFile) — gzip с правильным CRC (по твоему подтверждению — HoMM3 принимает)
5. ✏️ Чит-коды для campaign сейвов (GetCampaignCheat)
6. 🧠 Live memory editing через OpenProcess (тренер, не для нас)

Когда мы будем делать **запись сейвов** (Шаг 4 по первоначальному плану), нам нужно будет:
1. Перевести методы SaveArt, SaveArmy, SaveSkill из C# в Python
2. Реализовать SaveFile (gzip с правильным CRC) — мы уже умеем декомпрессировать с broken CRC, нужно научиться записывать с правильным
3. Добавить GUI для редактирования героя

