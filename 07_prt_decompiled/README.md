# 07_prt_decompiled — ProspectorRT полный реверс-код

> **Версия:** 1.0 (2026-10-10)
> **Источник:** `ProspectorRT.exe` (796 672 байт, .NET 4.x)
> **Декомпилятор:** ILSpy 8.2 (C# source) + dnfile + custom IL disassembler

Эта папка содержит **полные результаты дизасемблирования / восстановления исходного кода**
утилиты **ProspectorRT** — единственного известного стороннего редактора сейвов
Heroes of Might and Magic III (SoD/HotA), который фактически открывает `.GM1` без карты.

ProspectorRT служит эталонной реализацией для парсера `01_tools/save_parser.py`:
мы используем его алгоритмы и константы, но убираем искусственное ограничение
«только day-0 сейв» (см. `02_format_docs/PRT_reverse_analysis.md` и
`02_format_docs/PRT_offset_findings.md`).

---

## 📁 Структура

```
07_prt_decompiled/
├── README.md                          ← этот файл
│
├── ProspectorRT_source/               C#-декомпиляция (ILSpy 8.2)
│   ├── ProspectorRT/
│   │   ├── MainForm.cs          ⭐ 16 210 строк — главное окно + весь scanner
│   │   ├── DataSet2.cs                 21 383 строк — типизированный датасет
│   │   ├── PrintForm.cs                 829 строк — окно печати сейва
│   │   ├── LMOracle.cs                  781 строк — работа с oracle.dll
│   │   ├── ExportForm.cs                555 строк — окно экспорта
│   │   ├── SkillTreeForm.cs             309 строк — окно дерева навыков
│   │   ├── frmAbout.cs                  293 строк — about box
│   │   ├── ObjectNameForm.cs            258 строк — окно выбора имени объекта
│   │   ├── myToolTip.cs                 170 строк — кастомный tooltip
│   │   ├── Registry.cs                   78 строк — лицензия
│   │   ├── ShowForm.cs                   71 строк — общее окно
│   │   ├── Program.cs                    60 строк — точка входа
│   │   └── CommonSetting.cs              14 строк — глобальные настройки
│   ├── ProspectorRT.Properties/
│   │   ├── Resources.cs
│   │   └── Settings.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── System.IO.Compression/
│   │   └── ZipStorer.cs                  535 строк — встроенный zip writer
│   ├── ProspectorRT.csproj              проектный файл ILSpy
│   ├── *.resx                           Windows Forms ресурсы (4 файла)
│   ├── ProspectorRT.exe.licenses
│   └── app.ico
│
├── ProspectorRT_IL/                   IL-дизассемблирование (raw .NET IL)
│   └── full_il_dump.txt          ⭐ 4.6 МБ, 2 775 методов в 122 типах
│                                       Каждый метод: RVA, размер, max_stack,
│                                       все инструкции IL с разрешением токенов
│                                       Field/Method/Type/UserString
│
└── ProspectorRT_metadata/             .NET metadata извлечённая через dnfile
    ├── ProspectorRT_typedefs.txt        135 TypeDef (полный список типов)
    ├── ProspectorRT_methods.txt       2 905 MethodDef (все методы)
    ├── ProspectorRT_fields.txt          876 FieldDef (все поля)
    ├── ProspectorRT_typerefs.txt      1 000 TypeRef (внешние ссылки)
    ├── ProspectorRT.exe_strings.txt     ASCII строки из PE (86 КБ)
    └── ProspectorRT_user_strings.txt    #US heap (94 КБ — все строковые литералы C#)
```

---

## Ключевые находки из ProspectorRT (используются в нашем парсере)

| Константа / алгоритм | Значение | Подтверждено из |
|---|---|---|
| `HERO_STRIDE_SOD` | 0x446 (1094) | MainForm.cs (SearchHeroes) + IL disasm |
| `TOWN_RECORD_BASE_SIZE` | 382 байт | MainForm.cs (SaveTown/OpenTown) |
| Town stride | 382 + name_len | MainForm.cs (SaveTown/OpenTown) |
| Player state block | 8 × 145 байт | MainForm.cs (SaveColor/OpenColor) |
| `map.Color = map.Town - 1160` | формула | MainForm.cs (GetColorOffset) |
| Hero alt block @ +26 | TreeNumber, LastWisdom, LastMagic, MP, XP, Level | MainForm.cs (SaveArt/OpenArt) |
| Town spell pool depth | Castle=4, Tower=6, Stronghold/Fortress=3, остальные=5 | MainForm.cs (SaveSpell) |
| `IsObject` dispatch | 31 type_id | MainForm.cs (IsObject) |
| Tile scan loop | `for i=0..map.Objects-1 { tile_rec = read 8 bytes; if IsObject(...) dispatch }` | MainForm.cs (Scanner) |
| Section order (chain) | Start → tiles → EventBox → ArtRes → Monstr → SeerHut → PassGuard → MapTimedEvents → TownsTimedEvents → BottleSign → Mine → Dwelling → Garrison → UnknownVarReg → UnknownFixedReg → Color → Town → Hero → HeroState → CurrentState → BitField → TwoWayMonolith → SubTerGate → SubTerGatePair → Univer → Bank → Motions | MainForm.cs (GetSenseRegion) |

Сводка всех находок с физическими смещениями в `ProspectorRT.exe` —
см. `02_format_docs/PRT_offset_findings.md` и `02_format_docs/asm_prt_correlation.md`.

---

## Как использовать

### 1. C#-исходники (ProspectorRT_source/)
Открывать любым текстовым редактором или IDE. Главное — `MainForm.cs`.
В нём все методы сканера и редактора сейва:
- `Scanner` (главный цикл сканирования)
- `GetSenseRegion` (структура секций после tile loop)
- `Open*` / `Save*` (чтение/запись каждой секции: `OpenHero`/`SaveHero`,
  `OpenTown`/`SaveTown`, `OpenColor`/`SaveColor`, `OpenArt`/`SaveArt`,
  `OpenMonstr`/`SaveMonstr`, `OpenSeerHut`/`SaveSeerHut`, `OpenBank`, ...)
- `IsObject` (dispatch по type_id)
- `AnalysisContent` (универсальный парсер post-tile секций)
- `FindHero*`, `FindTown*` (поиск блоков)

### 2. IL-дамп (ProspectorRT_IL/full_il_dump.txt)
Полный текстовый дамп .NET IL. Полезен, когда C#-декомпиляция ILSpy даёт
неточный результат (иногда ILSpy выбирает неверный цикл или неправильный
`if`-else синтаксис). IL — это ground truth.

Структура каждой записи:
```
========== TYPE #N: Full.Type.Name  (methods: M) ==========
  --- Method [idx] MethodName  RVA=0x...  size=N  type=fat  max_stack=K ---
    IL_0000: ldarg.0
    IL_0001: ldfld  Field:someField
    IL_0007: callvirt  Method:SomeMethod
    IL_000C: ret
```

### 3. Metadata (ProspectorRT_metadata/)
Списки всех типов, методов, полей, строк — извлечены через `dnfile` для
быстрой навигации без открытия 4.6 МБ IL-файла.

---

## Как получено

### C# декомпиляция
ILSpy 8.2 (GUI или CLI) над `PRT_reverse/ProspectorRT.exe`.
Ассемблия — .NET 4.x, C#, Windows Forms, использует DevExpress v11.1
(DevExpress.Data, DevExpress.Utils, DevExpress.XtraEditors, DevExpress.XtraGrid,
DevExpress.XtraTreeList, DevExpress.XtraBars, DevExpress.XtraLayout,
DevExpress.XtraPrinting, DevExpress.XtraRichEdit, DevExpress.RichEdit.Core).

### IL дизассемблирование
Скрипт `scripts/extract_full_il.py` — парсит PE/CLI metadata через `dnfile`,
читает method bodies через `RVA → file offset`, декодирует IL-опкоды
(полная таблица 0x00..0xE0 + префикс 0xFE), разрешает токены в имена.

Запуск:
```bash
python3 /home/z/my-project/scripts/extract_full_il.py
# Output: H3save_reader/07_prt_decompiled/ProspectorRT_IL/full_il_dump.txt
```

### Metadata extraction
`scripts/decompile_prt.py` — использует `dnfile` для всех таблиц TypeDef,
MethodDef, FieldDef, TypeRef. Строки PE и #US heap — через
`scripts/extract_prt_strings.py` и `scripts/extract_user_strings.py`.

---

## Замечания

- `DataSet2.cs` (21 383 строк) — автосгенерированный код типизированного
  DataSet для grid-представлений. **Не содержит** логики формата сейва.
- `myToolTip.cs` (170 строк) — кастомный tooltip отрисовки, **не содержит**
  логики сейва.
- Все "интересные" методы находятся в `MainForm.cs` и `LMOracle.cs`.
- Поддержка HotA в ProspectorRT сделана через `LMOracle.SkillTreeAPI.dll`
  — см. `LMOracle.cs` и `LMOracle.SkillTreeAPI.dll` в `PRT_reverse/`.
