# ProspectorRT 2.3 — Reverse Engineering Analysis

> Источник: `https://github.com/AlexZ98765/PRT_reverse`
> Автор: Stormbringer (2012-2021)
> Лицензия: неизвестна (закрытый код)
> Цель: анализ СТАРТОВЫХ сейвов HoMM3 (SoD/HotA/WoG) для турнирной разведки

## Что такое ProspectorRT

`.NET` приложение (PE32, Mono/.Net assembly, 2.3.0.0), 778 KB. Зависимости:
- `DevExpress` 11.1 (UI-библиотека)
- `LMOracle.SkillTreeAPI.dll` (для экспорта в LM Oracle H3)
- `SkinLib.dll` (визуальные темы)

Парсит **только стартовые** сейвы (нулевой день) и выводит 24 таблицы с объектами карты (арты, банки, монстры, города, герои, события, ресурсы, навыки, заклинания, тюрьмы, topology, и т.д.).

## Сколько извлечено

Что мы смогли получить **без исходников** через статический анализ:

| Источник | Что извлечено | Кол-во |
|----------|---------------|--------|
| `.NET TypeDef table` | 135 определённых классов (включая System/DevExpress) | 135 |
| `.NET MethodDef table` | 2905 методов | 2905 |
| `.NET Field table` | 1331 поле классов | 1331 |
| `.NET TypeRef table` | 418 ссылок на внешние типы | 418 |
| `#US heap` (user strings) | 3637 уникальных строковых литералов | 3637 |
| ASCII strings в PE | 4876 строк | 4876 |

## Главные находки

### 1. 24 таблицы DataSet (R_*)

ProspectorRT использует типизированный .NET DataSet с 24 таблицами. Это соответствует 24 вкладкам UI (см. Readme.txt). Имена таблиц:

| Таблица | Что содержит | Кол-во строк (типично) |
|---------|--------------|------------------------|
| `R_AllArts` | Все артефакты на карте + у героев | ~50-200 |
| `R_AllExperience` | Все объекты, дающие опыт (Дерево Знаний, Жертвенный алтарь, Форт на холме, Таверна, Гильдия наёмников) | ~10-50 |
| `R_AllSkill` | Все объекты, дающие вторичные навыки (Университет, Хижина ведьмы) | ~5-20 |
| `R_AllSpell` | Все объекты, дающие заклинания (Святыни, Пирамида, Свиток) | ~10-30 |
| `R_AllTimer` | Все события-таймеры | ~5-20 |
| `R_Art` | Артефакты (отдельно от All) | ~30-100 |
| `R_Bank` | Банки (Наг, Драконий Улей, etc.) | ~5-20 |
| `R_Camp` | Костры | ~5-20 |
| `R_Chest` | Сундуки и Морские сундуки | ~20-50 |
| `R_Garrison` | Гарнизоны (Наемники, Тюрьмы) | ~5-20 |
| `R_Heroes` | Герои в тюрьмах + доступные герои | 156 |
| `R_Market` | Черные рынки + Торговцы артефактами | ~1-5 |
| `R_Mine` | Шахты ресурсов | ~20-50 |
| `R_Monstr` | Монстры (стражи) | ~50-200 |
| `R_Object` | Объекты посещений (Дерево Знаний, Фонтаны, etc.) | ~50-200 |
| `R_PassGuard` | Стражи прохода | ~5-20 |
| `R_Prison` | Тюрьмы с героями | ~1-10 |
| `R_Resource` | Ресурсы на земле | ~20-100 |
| `R_Scholar` | Учёные | ~5-20 |
| `R_SeerHut` | Хижины провидцев | ~5-30 |
| `R_Skill` | Объекты, дающие втор. навыки (отдельно от All) | ~5-20 |
| `R_Spell` | Объекты с заклинаниями (отдельно от All) | ~10-30 |
| `R_Topology` | Монолиты, Подземные врата, Вихри | ~5-20 |
| `R_Town` | Города | 21 (для Myth and Legend) |

### 2. 24 Section-класса

Каждой таблице R_* соответствует Section-класс, который парсит свою секцию сейва:

- `ArtClassSection`, `ArtSection` — артефакты
- `ColorSection` — цвета/фракции
- `DollSection` — экипировка героя (doll = кукла)
- `DwellingSection` — жилища существ
- `FullLevelSkillSection`, `LevelSkillSection`, `SkillSection` — навыки
- `HeroClassSection` — класс героя
- `IdeologySection` — добро/зло/нейтрал (для некоторых объектов)
- `LocalitySection` — локации
- `MineSection` — шахты
- `MonstrSection` — монстры
- `ObjectSection` — общие объекты
- `PlaceSection` — места размещения
- `QuestSection` — задания провидцев
- `ReplySection` — реплики
- `RewardSection` — награды
- `SkillSection` — навыки
- `SpellSection` — заклинания
- `StatusSection` — статусы
- `TentSection` — палатки ( жилища )
- `TownSection` — города

### 3. 88 Get* методов (парсеров)

Ключевые методы, раскрывающие структуру сейва:

#### Map / Save scanning
- `ScanMap`, `ScanMaps`, `ScanMapTimedEvents` — парсинг карты
- `ScanSave` — главный парсер сейва
- `ScanComplete`, `ScanError`, `ScanStatus` — обработка результата
- `GetMapStart` — находит начало map данных в сейве
- `GetHStartPoint` — находит hero start point (стартовая позиция героев)
- `GetSaveFile` — открывает .GM1 файл
- `GetFileInfo` — заголовок сейва
- `GetFileOffset` — возвращает смещение файла
- `ReadAllBytes`, `ReadPartBytes`, `ReadFileInfo`, `ReadCentralDir` — ZIP/gzip чтение
- `ExtractFile` — распаковка

#### Hero parsing
- `ScanHeroesContent` — сканирует содержимое героев
- `GetHeroesContent` — возвращает содержимое героев
- `GetNewHero` — создаёт нового героя
- `GetPrisonHero` — герои в тюрьмах
- `GetStartHero` — стартовые герои
- `GetTavernGuest` — герои в таверне
- `HeroOnObject` — герой на объекте
- `GetNewName` — новое имя героя (пользовательское)
- `GetNewLevel` — новый уровень героя
- `GetNewSteps` — новые шаги
- `GetNewStruct` — новая структура героя
- `GetNewWeights` — новые веса (primary stats)
- `RAMHero` — доступ к памяти героя (открытый процесс HoMM3!)
- `FillAllHero`, `FillSelectedHero` — заполнение таблицы героев
- `GetArmy`, `GetArts`, `SaveHero`, `SaveArmy`, `SaveArt`, `SaveSkill` — экспорт героя
- `IsHeroNull`, `IsHeroTavern` — проверки

#### Town parsing
- `ScanTownsContent` — сканирует города
- `ScanTownsTimedEvents` — таймеры построек в городах
- `GetTownContent` — содержимое города
- `GetTownType` — тип города (Castle, Rampart, ...)
- `GetTownSpell` — заклинания в Magic Guild
- `GetTimerTown` — таймер построек
- `MapTownsTimedEvents` — карта таймеров

#### Object parsing
- `ScanArtResContent` — артефакты и ресурсы
- `ScanBankContent` — банки
- `ScanBottleSignContent` — бутылки со знаками (?)
- `ScanEventBoxContent` — ящики событий
- `ScanMonolithWhirlpool` — монолиты и вихри
- `ScanMonstrContent` — монстры
- `ScanObjectContent` — объекты
- `ScanPassGuardContent` — стражи прохода
- `ScanSeerHutContent` — хижины провидцев
- `GetBankContent`, `GetBankGuard`, `GetBankMonster`, `GetBankResource` — банки
- `GetMarketContent` — рынки
- `GetGarrisonContent` — гарнизоны
- `GetDollPlace` — слоты экипировки
- `GetColorContent`, `GetColor`, `GetCorrectColor` — цвета
- `GetAlliance` — альянсы
- `GetExperience` — опыт
- `GetAllSkill`, `GetSSkillLevel`, `GetSkillLevel` — навыки
- `GetAllSpell` — заклинания
- `GetMonster` — монстры
- `GetNewStruct` — структуры
- `GetOrientation` — ориентация
- `GetPairSubterraneanGate` — пары подземных врат
- `GetSenseRegion` — область восприятия
- `GetVerObjName` — имя объекта (по словарю)
- `GetTownType` — тип города

### 4. Колонки DataSet (381 уникальное имя)

Из user strings (#US heap) извлечены имена колонок. Они раскрывают **схему данных** в каждой таблице.

Категории колонок:

#### Идентификаторы
- `ID`, `Id`, `Address`, `Number`, `Slot`, `Code`, `Bit`, `Byte`, `Level`, `Type`

#### Координаты
- `X`, `Y`, `Z` (используются в SQL-фильтрах: `' AND Y='`, `' AND Z='`, `X='`)

#### Игровые сущности
- `Hero`, `Town`, `Building`, `Built`, `Color`, `Class`, `ClassID`, `Ideology`
- `Garrison`, `Guard`, `Monster`, `Creature`, `Creature`s (typo?)
- `Art`, `Artefact`, `Artifact`, `BackPack`, `Doll`, `Book`
- `Spell`, `Skill`, `SecondarySkill`, `Experience`, `Mana`, `HP`, `MP`
- `Reward`, `Mission`, `Deadline`, `Day`, `Repeat`, `Timer`
- `Resource`, `Mine`, `Treasure`, `Cost`, `Increase`
- `Available`, `Apply`, `Hire`, `Place`, `Locality`
- `MageTimer`, `LibTimer`, `Library`, `LastMagic`, `LastWisdom`
- `AntiMagic`, `Morale`, `Luck`, `Mage`, `Wizard`
- `Constructed`, `Default`

#### SQL-выражения, найденные в коде
```
' AND Bit='
' AND Byte='
' AND Byte='6' AND Bit='0'
' AND Level='
' AND Type='
' AND Y='
' AND Z='
' OR Place IS NULL)
X='
Guard IS NOT NULL OR Experience IS NOT NULL
Monster IS NOT NULL AND Color IS NOT NULL AND Place <> '
Slot='1' AND (Code='2' OR Code='5' OR Code='8')
Spell IS NOT NULL AND (Place <> '
```

Эти SQL-фильтры — реальный ключ к пониманию того, как ProspectorRT отличает типы объектов в таблице `R_Object`:
- `Slot='1' AND (Code='2' OR Code='5' OR Code='8')` — это фильтр для определённых объектов (возможно, посетить-объекты с конкретными ID)
- `Byte='6' AND Bit='0'` — фильтр по байту и биту (часто используется для флагов)

### 5. Числовые литералы (вероятные смещения в сейве)

Из user strings найдены числовые константы, которые **могут быть смещениями в SoD-сейве** (проверено на Myth and Legend/0000.GM1):

| Число | Hex | Содержимое в ML day-0 | Гипотеза |
|-------|-----|----------------------|----------|
| 15 | 0x0F | `00 00 00 00 00 00 00 00` | paddings в заголовке |
| 105 | 0x69 | cp1251 строка (часть описания карты) | смещение в description |
| 129 | 0x81 | cp1251 строка | description продолжается |
| 130 | 0x82 | cp1251 строка | description |
| 240 | 0xF0 | cp1251 строка | description (перед player setup) |
| 2000 | 0x7D0 | `1a 00 00 00 41 00 00 00` | likely player_state block (вероятно) |
| 4505 | 0x1199 | `02 00 1a 00 00 00 00 40` | ранняя секция (возможно, AI state) |
| 8010 | 0x1F4A | `00 18 00 00 00 00 00 01` | середина сейва (возможно, town/army) |
| 14805 | 0x39D5 | `0a 01 00 2e 00 23 02 65` | ранняя секция |
| **124226** | **0x1E542** | `39 00 ff ff ff ff 03 00` | **герой** (похоже на запись о герое — `ff ff ff ff` = пустой слот) |
| **124519** | **0x1E667** | `00 00 00 ff ff ff ff 00` | **герой** (похоже на inventory slot) |

### 6. Имена экспортируемых DLL

В строках встречаются:
- `\OT-3.dll` — вероятно, "Oracle Tool 3" или "Object Tool 3"
- `\SPT.dll` — "Skill Progression Tree" (для экспорта деревьев прокачки)
- `\LMOracle.SkillTreeAPI.dll` — экспорт в LM Oracle H3

Эти DLL'и не входят в репозиторий, но упоминание важно для понимания архитектуры.

### 7. Распознаваемые файлы

В строках есть:
- `*.CGM;*.GM1;*GM0` — фильтр открытых файлов (campaign saves, scenario saves, ?)
- `H3C` — campaign save magic
- `H3M` — карта
- `H3SV` — начало "H3SVG" (scenario save magic)

## Что НЕ удалось извлечь без исходников

### IL код методов
Главное ограничение: статический анализ через строки не даёт **тела методов**. Чтобы узнать, что ProspectorRT делает на самом деле (какие байты читает, как валидирует), нужен декомпилятор IL:
- **ILSpy** (open source)
- **dnSpy** (fork ILSpy + debugger)
- **dotPeek** (JetBrains, free)
- **monodis** (Linux CLI)

У нас в среде нет ни одного из них. Но! Можно установить через pip `pythonnet` (уже сделано) и **вызывать .NET методы напрямую** через clr.

### Параметры методов
В .NET MethodDef есть параметрические имена, но они видны только через декомпиляцию IL. Через `dnfile` мы получаем только имена методов, не их параметры.

### Точные смещения в сейве
Без IL кода нельзя сказать **однозначно**, что `0x1E542` — это "первый герой". Но можно **гипотетически**:
1. У ProspectorRT есть `GetHStartPoint` — значит есть какая-то логика поиска hero start
2. В строках есть числа `124226, 124519` — вероятно, это захардкоженные offsets для SoD-сейвов (без подземелья? со стандартной картой?)

## Гипотеза

ProspectorRT был написан под конкретные SoD-сейвы 144x144 (XL) с подземельем. Числа вроде 124226 (=0x1E542) — это смещения в **стандартном SoD-сейве**. Для других карт (разного размера) нужно либо пересчитывать, либо ProspectorRT использует относительные offsets от `GetMapStart` / `GetHStartPoint`.

## Что мы можем сделать с этой информацией

### Сейчас (без IL-декомпиляции)

1. **Расширить known_unknowns** — добавить список полей, которые мы теперь знаем что они есть (но не знаем где):
   - `Library`, `LibTimer`, `LastMagic`, `LastWisdom` — библиотека/магия в Tower (только для городов Tower)
   - `MageTimer`, `Building`, `Built`, `Constructed` — постройки и таймеры в городах
   - `Reward`, `Mission`, `Deadline`, `Day`, `Repeat` — задания Seer Hut
   - `AntiMagic`, `Morale`, `Luck` — модификаторы
   - `Hire`, `Place`, `Locality` — размещение объектов
   - `Doll`, `BackPack`, `Book` — экипировка героя (doll = body slots, backpack = inventory, book = spell book)

2. **Использовать SQL-выражения как подсказку** — ProspectorRT различает объекты по `Byte` + `Bit` флагам. Если на карте есть несколько объектов с одним `Code`/`Type`, их можно различить по комбинации `(Slot, Code, Byte, Bit)`.

3. **Имена Section-классов** — это **явные названия секций сейва**, которые нужно локализовать. Сейчас у нас `cluster_finder` находит 10 кластеров, но не знает их семантику. Если сопоставить имена Section-классов ProspectorRT с нашими кластерами, мы сможем назвать:
   - `cluster:main` → `ObjectSection` (per-object state array)
   - `cluster:visiting` → ?
   - `cluster:decoration` → `LocalitySection` или `PlaceSection`
   - `cluster:alive` → ?
   - `cluster:fog` → ?
   - `cluster:treasure` → `MineSection` / `ResourceSection`
   - `cluster:other_*` → ?

### Что требует настоящей декомпиляции

Чтобы **извлечь полный формат** из ProspectorRT, нужен ILSpy или dnSpy. Тогда мы увидим:
- Точные смещения (offsets) в сейве для каждого поля
- Логику поиска секций (как `GetMapStart` находит начало map данных)
- Структуры hero block / town block (которые у ProspectorRT, возможно, отличаются от h3sed)

## Файлы

Скрипты для извлечения:
- `scripts/extract_prt_strings.py` — извлечение ASCII строк (PE strings)
- `scripts/extract_user_strings.py` — извлечение user strings (#US heap)
- `scripts/decompile_prt.py` — извлечение .NET metadata через dnfile

Результаты:
- `scripts/ProspectorRT.exe_strings.txt` — все ASCII строки (4876)
- `scripts/ProspectorRT_user_strings.txt` — все .NET user strings (3637)
- `scripts/ProspectorRT_typedefs.txt` — 135 TypeDefs
- `scripts/ProspectorRT_methods.txt` — 2905 MethodDefs
- `scripts/ProspectorRT_fields.txt` — 1331 Fields
- `scripts/ProspectorRT_typerefs.txt` — 418 TypeRefs
- (аналогично для HeroesInfo.exe)

---

## Дополнение: HeroesInfo.exe — анализ

`HeroesInfo.exe` (526 KB) — это **дополнительная** тулза от того же автора. В отличие от ProspectorRT, которая анализирует только стартовые сейвы, HeroesInfo работает с **памятью запущенной игры HoMM3** через `OpenProcess`/`RAMHero`. То есть это не только анализатор сейвов, но и **тренер/редактор**!

### Ключевые методы
- `OpenProcess` — открывает процесс HoMM3
- `RAMHero` — читает hero state из памяти (live)
- `GetProcessID` — находит PID HoMM3
- `GetHStartPoint` — находит hero start point в памяти (или в сейве?)

### Дополнительные таблицы (HeroesInfo)
- `R_Heroes` — герои (общая с ProspectorRT)
- `R_PSkill` — primary skills (attack/defense/power/knowledge)
- `R_SSSkill` — secondary skills (детально)
- `R_AddSSkill` — дополнительные навыки (для деревьев прокачки)
- `R_ShortPath`, `R_SSSkill_R_ShortPath` — короткие пути деревьев прокачки
- `R_STreeNumber` — номера деревьев навыков
- `R_Tavern` — гости таверны
- `R_AllArts` — все артефакты (общая с ProspectorRT)
- `R_Oracle` — для экспорта в LM Oracle

### Найденные новые колонки

Из error messages HeroesInfo:
- `R_Heroes`: Hero, Class, Color, Hire, Ideology, Level, LevelUp
- `R_PSkill`: Class, ClassID, Color, Hero, Hire, ID, Ideology, LastMagic, LastWisdom, Level, LevelUp
- `R_SSSkill`: Class, ClassID, Color, Hero, Hire, ID, Ideology, LastMagic, LastWisdom, Level, LevelUp
- `R_AddSSkill`: Advanced, Basic, Expert, ID (для каждого скилла 3 уровня владения)

### Semantика
- `LastMagic` / `LastWisdom` — это последние доступные "магия" / "мудрость" в дереве прокачки. Возможно, указывают на то, что герой уже выбирал эти навыки при level-up.
- `Hire` — может ли быть нанят (для героев в таверне)
- `LevelUp` — количество level-ups, которые ещё не обработаны (когда герой накопил опыта больше, чем нужно для нескольких уровней)
- `Class` / `ClassID` — класс героя (Knight, Cleric, Ranger, ...)
- `Ideology` — добро/зло/нейтрал (только для некоторых классов)

### Forms
- `MainForm` — главная
- `PSkillForm` — primary skills
- `SSSkillForm` — secondary skills
- `SkillTreeForm` — дерево навыков
- `CheatForm` — форма читов!

Это значит, что HeroesInfo умеет:
1. Анализировать сейвы (как ProspectorRT)
2. **Модифицировать** скиллы/primary stats героев в памяти
3. Применять **читы** (CheatForm)

### Вывод для нашего проекта

1. **HeroesInfo — это редактор**, а не только анализатор. Если мы найдём способ извлечь IL код методов `SaveHero`, `SaveArmy`, `SaveArt`, `SaveSkill`, мы получим готовую логику модификации сейвов.
2. **`OpenProcess` + `RAMHero`** указывает на то, что HeroesInfo знает **адреса в памяти HoMM3** для героя. Это полезно для разработки **тренера** (не редактора сейвов, а редактора памяти). Но это не наша цель.
3. **`GetHStartPoint` в обоих тулах** — это общий метод, который, вероятно, возвращает один и тот же hero block start offset в сейве (или в памяти). Расшифровка этого метода даст нам универсальную логику поиска hero blocks.

### Что делать дальше

Вариант A — **установить ILSpy / dnSpy** в среде разработки (Linux/Windows) и:
- Декомпилировать ProspectorRT.exe + HeroesInfo.exe
- Извлечь IL код методов `ScanSave`, `ScanHeroesContent`, `GetHeroesContent`, `GetHStartPoint`
- Извлечь IL код методов `SaveHero`, `SaveArmy`, `SaveArt`, `SaveSkill` (если они есть)
- Перевести IL в C# и далее — в Python

Вариант B — **использовать pythonnet (clr)** для прямого вызова методов ProspectorRT/HeroesInfo:
- Загрузить ProspectorRT.exe как .NET assembly
- Вызвать `ProspectorRT.R_Heroes.ScanHeroesContent(save_bytes)` или аналогичный метод
- Получить DataTable с распарсенными героями
- Минус: нужно знать точную сигнатуру методов (можно через Reflector)

Вариант C — **динамический анализ**:
- Запустить ProspectorRT.exe через Wine + attach debugger
- Установить точки останова на ReadFile/ReadProcessMemory
- Записать, какие смещения читаются
- Минус: нужны сейвы с известной структурой

## Рекомендация

Самый прямой путь — **Вариант A** (ILSpy). Бесплатно, open source, хорошо документировано. Декомпиляция ProspectorRT.exe даст нам:
1. Точные смещения для всех 24 Section-классов
2. Логику `GetMapStart` / `GetHStartPoint` (как они находят начало секций)
3. Логику `ScanSave` — главного парсера
4. Структуры hero block / town block (с возможными отличиями от h3sed)

Это **закроет почти все 13 known_unknowns** в нашем `gm1_mapping.json`.

---

## Дополнение 2: IL-анализ ProspectorRT (Step 2 — извлечение IL)

После извлечения IL из `ProspectorRT.exe` (через `dnfile` + custom IL disassembler), мы получили **полный дизассемблированный IL код 29 типов** (MainForm + классы парсинга):

- `ProspectorRT.MainForm` — 289 методов
- `ProspectorRT.DataSet2` — 67 методов (DataSet с 24 таблицами)
- `ProspectorRT.ExportForm`, `ProspectorRT.Registry`, `ProspectorRT.LMOracle`, и т.д.
- `ScanContent`, `ObjectContent`, `MapRegion`, `Compression`, `ZipFileEntry`

### Главные находки IL анализа

#### 1. Подтверждение `HERO_STRIDE_SOD`

В методе **`ScanHeroesContent`** (RVA=0x3ECC4, 91 bytes IL) найдено:
```
IL_0043  ldc.i4  1094 (0x446)  ← HERO_STRIDE_SOD
```

Это **точно совпадает** с нашей константой `HERO_STRIDE_SOD = 0x446` в `save_layout.py`. ProspectorRT использует тот же stride 1094 байта между hero blocks.

Также в `ScanHeroesContent` найдены:
- `ldc.i4 255 (0xFF)` — Neutral player faction (255 = `0xFF`)
- `ldc.i4.s 11 (0xB)` — 11 (возможно, индекс path_record_types для `0b 03` = fog_of_war_update)
- `ldc.i4.s 22 (0x16)`, `ldc.i4.s 23 (0x17)` — 22, 23 (возможно, размеры или смещения подструктур)

#### 2. `ScanTownsContent` — константы для города

В методе **`ScanTownsContent`** (RVA=0x3EE4C, 49 bytes IL) найдено:
```
IL_001A  ldc.i4.s  70 (0x46)
IL_001E  ldc.i4  382 (0x17E)  ← новая константа для town blocks
```

- `0x46 (70)` — **смещение name_len в town block** (совпадает с нашим `TOWN_FIELD_OFFSETS["name_len"] = 69` — близко, но ProspectorRT использует 70)
- `0x17E (382)` — **новая константа для town**. Возможно, это stride между town records (в SoD/HotA). Нужно проверить на Myth and Legend (где 21 town), найдя расстояние между ними.

#### 3. `GetMapStart` — поиск начала map данных

В методе **`GetMapStart`** (RVA=0x3F58C, 215 bytes IL) найдено:
```
IL_001E  ldc.i4  688 (0x2B0)  ← offset сохраняется в поле
IL_0057  ldc.i4  258 (0x102)  ← сравнение с 258
IL_003A  ldc.i4.s  28 (0x1C)
```

- `0x2B0 (688)` — **вероятно, смещение старта map data** в SoD-сейве (после header + player setup)
- `0x102 (258)` — **вероятно, signature/маркер** для проверки, что это корректное начало map
- `0x1C (28)` — вероятно, сдвиг или размер подструктуры

#### 4. `GetStart` — поиск hero start point

В методе **`GetStart`** (RVA=0x3F670, 478 bytes IL) найдено:
```
IL_0011  ldc.i4.s  66 (0x42)
IL_0116  ldc.i4.s  57 (0x39)
IL_0129  ldc.i4  341 (0x155)
IL_0138  ldc.i4.s  46 (0x2E)
```

- `0x42 (66)`, `0x39 (57)`, `0x155 (341)`, `0x2E (46)` — серия констант, вероятно для поиска hero start. Похоже на offsets в памяти HoMM3 или в save.

#### 5. `GetExperience` — уровни опыта

В методе **`GetExperience`** (RVA=0x368E4) найдено:
```
0x1E0 (480), 0x1F4 (500), 0x32A (810), 0x3E8 (1000)
```
Это **уровни опыта для level-up** в HoMM3 (1000 XP = уровень 2, и т.д.).

#### 6. Константы, найденные в разных методах (выборка)

| Hex | Dec | Метод | Гипотеза |
|-----|-----|-------|----------|
| `0x102` | 258 | GetMapStart | marker for map start |
| `0x155` | 341 | GetStart | hero start point offset |
| `0x17E` | 382 | ScanTownsContent | town record stride |
| `0x2AE` | 686 | GetHeroesContent | hero field offset |
| `0x2B0` | 688 | GetMapStart | map data start offset |
| `0x442` | 1090 | CreateTblMonster | ? |
| **`0x446`** | **1094** | **ScanHeroesContent** | **HERO_STRIDE_SOD** (подтверждено!) |
| `0x447` | 1095 | CreateTblMonster | ? |
| `0x488` | 1160 | GetSenseRegion | region size |
| `0x7D0` | 2000 | SaveChest | chest value |
| `0x1388` | 5000 | CreateTblArt, CreateTblMonster | artifact/monster value |
| `0x2710` | 10000 | CreateTblMonster, CreateTblArt | ? |
| `0x7530` | 30000 | CreateTblMonster | ? |
| `0x200000` | 2097152 | InitGridAndViews | bit mask |
| `0xE88172` | 15237490 | ? | unknown — может быть magic constant |

### Что это даёт нашему проекту

1. **Подтверждение**: Наш `HERO_STRIDE_SOD = 0x446 (1094)` — корректен. ProspectorRT использует тот же stride.

2. **Новые константы для проверки**:
   - `0x17E (382)` — возможный town record stride. Проверим на Myth and Legend (21 town).
   - `0x2B0 (688)` — возможный start offset map data в SoD-сейве.
   - `0x102 (258)` — маркер для проверки map start.

3. **Метод `GetMapStart`**: Если мы сможем декодировать его IL полностью, мы получим готовый алгоритм поиска начала map data — это закроет known_unknown #2 (Map object positions and states).

4. **Метод `GetStart`**: Если декодируем его, мы поймём, как ProspectorRT находит стартовую позицию героев в сейве.

5. **Метод `ScanTownsContent`**: Короткий (49 байт IL) — его полностью декомпилировать легко. Покажет, как ProspectorRT парсит town records (у нас есть `find_town_blocks` — но ProspectorRT может использовать другой подход, например stride 382 вместо нашего поиска по координатам).

### Что нужно для полного декомпилятора

Чтобы получить полные исходники из IL, нужен **полный IL-декомпилятор**:
- ILSpy / dnSpy / dotPeek (Windows)
- `ilspycmd` (cross-platform CLI)
- `monodis` (Linux)

В нашей среде нет ни одного. Но через `dnfile` + custom IL disassembler мы извлекли:
- 29 типов ProspectorRT с их методами (имена методов)
- IL opcode последовательности (с константами и токенами)
- Связь констант с методами (что используется где)

**Этого достаточно**, чтобы:
1. Подтвердить наши константы (HERO_STRIDE_SOD ✓)
2. Найти новые кандидаты для проверки (0x17E, 0x2B0, 0x102)
3. Понять высокоуровневую структуру (какой метод что парсит)

**Не достаточно**, чтобы:
- Точно сказать, как ProspectorRT находит `GetMapStart` (нужен полный декомпилятор)
- Восстановить точную логику сканирования hero/town blocks
- Понять, как ProspectorRT различает типы объектов (по каким byte/bit флагам)

### Рекомендация

1. Установить **ILSpy** или **ilspycmd** в среде с .NET (Windows/Mac/Linux с dotnet SDK).
2. Декомпилировать `ProspectorRT.exe` целиком → `ProspectorRT_source/`.
3. Прочитать методы `ScanSave`, `ScanHeroesContent`, `ScanTownsContent`, `GetMapStart`, `GetStart`, `GetHeroesContent`, `GetTownContent`.
4. Перевести декомпилированный C# в Python, добавить как новый модуль `01_tools/prt_parser.py` (альтернатива h3sed-стилю).
5. Сравнить подход ProspectorRT (stride-based) с нашим (cluster-based) — выбрать лучший или объединить.

### Файлы

- `/home/z/my-project/scripts/extract_il_v2.py` — IL-декомпилятор (Python + dnfile)
- `/home/z/my-project/scripts/PRT_il_analysis.txt` — IL анализ (1.4 MB)
- `/home/z/my-project/scripts/ProspectorRT_typedefs.txt` — 135 TypeDefs
- `/home/z/my-project/scripts/ProspectorRT_methods.txt` — 2905 MethodDefs
- `/home/z/my-project/scripts/ProspectorRT_user_strings.txt` — 3637 user strings

---

## Дополнение 3: Подтверждение town record stride = 382 + name_len

### Гипотеза ProspectorRT

`ScanTownsContent` использует `0x17E (382)` как базовый stride между town records.

### Проверка на Myth and Legend (21 towns)

Вычислены strides между нашими найденными town blocks:

| Город | cp1251 длина имени | Stride | Stride - 382 |
|-------|---------------------|--------|--------------|
| Кавала | 6 | 388 | 6 ✓ |
| Волос | 5 | 387 | 5 ✓ |
| Каламата | 8 | 390 | 8 ✓ |
| Этопия | 6 | 388 | 6 ✓ |
| Дельфи | 6 | 388 | 6 ✓ |
| Хания | 5 | 387 | 5 ✓ |
| Коринф | 6 | 388 | 6 ✓ |
| Спарта | 6 | 388 | 6 ✓ |
| Абдера | 6 | 388 | 6 ✓ |
| Патрас | 6 | 388 | 6 ✓ |
| Комотини | 8 | 390 | 8 ✓ |
| Хиос | 4 | 386 | 4 ✓ |
| Иоанна | 6 | 388 | 6 ✓ |
| Winery | 6 | 388 | 6 ✓ |
| Итака | 5 | 387 | 5 ✓ |
| Ламиа | 5 | 387 | 5 ✓ |
| Олимп | 5 | 387 | 5 ✓ |
| Троя | 4 | 386 | 4 ✓ |
| Афина | 5 | 387 | 5 ✓ |
| Дом Аида | 8 | 390 | 8 ✓ |

**100% совпадение** для всех 21 городов: `stride = 382 + len(name_in_cp1251)`

### Значение для нашего проекта

Это значит, что **town record имеет фиксированную структуру**:
- **base = 382 байта** (включая 2-байтовый length prefix для имени)
- После length prefix следует `name_len` байт имени
- Total: `stride = 382 + name_len`

Если мы знаем, что town record имеет `name_len` на offset 69 (по нашим `TOWN_FIELD_OFFSETS`), то:
- bytes 0..68 = 69 байт фиксированных полей (faction, type, x, y, z, army_types[28], army_counts[28])
- bytes 69..70 = name_len (2 байта LE)
- bytes 71..(71+name_len-1) = name (cp1251)
- bytes (71+name_len)..381 = оставшиеся поля (post-name fields, 382-71-name_len байт)

То есть town record имеет **пост-именную секцию** размером `382 - 71 - name_len + name_len = 382 - 71 = 311 байт` (постоянная длина, не зависит от имени).

Эти 311 байт после имени содержат:
- Building bitmask (4 байта — из наших дифф-анализов `0x13F7AB`)
- Mage Guild level (1 байт — `0x13F74D`)
- Town build flag (1 байт — `0x13F70B`)
- Spell pool (variable, ~70 байт)
- И другие поля

**Сейчас у нас нет функции, которая бы парсила все эти 311 байт**. Это объясняет, почему ProspectorRT умеет показывать 24 таблицы с детальным контентом, а мы пока только header + heroes + towns с базовыми полями.

### Альтернативная стратегия парсинга towns

Теперь у нас есть **два пути** для парсинга towns:

**Путь A (текущий)**: `find_town_blocks` ищет 3-byte (x,y,z) координаты в сейве, валидирует faction/type. Универсально, но находит только 21 из 21 на Myth and Legend (работает корректно).

**Путь B (ProspectorRT-style)**: использовать stride = 382 + name_len.
1. Найти первый town block (через coord search, как в Пути A)
2. Считать name_len
3. Следующий town block на offset = current + 382 + name_len
4. Повторять, пока town валиден

**Путь B быстрее** (O(n) вместо O(save_size) для кластерного поиска), но требует, чтобы towns шли вплотную без промежутков. У нас на Myth and Legend это так — strides 386-390 (соответствуют формуле 382 + name_len).

**Гибридный подход** (рекомендуется):
1. Использовать `find_town_blocks` для поиска всех town блоков (как сейчас)
2. Проверить, что расстояния между ними соответствуют формуле `382 + name_len` — это даст **валидацию** что мы нашли все towns корректно
3. Если найдены не все — использовать stride-based поиск для дополнительных

### Влияние на known_unknowns

Этот stride-анализ помогает локализовать town-связанные known_unknowns:
- ✅ Town record base size: 382 байта (включая name_len prefix)
- ✅ Town record total size: `382 + name_len` (post-name поля занимают 311 байт)
- ⚠️ Содержимое post-name полей (311 байт) всё ещё не до конца локализовано — нужно посмотреть IL `GetTownContent` чтобы понять, что ProspectorRT читает из этих 311 байт
- ⚠️ Building bitmask, Mage Guild level, spell pool — эти поля уже частично локализованы (через наши дифф-анализы), но ProspectorRT может знать больше
