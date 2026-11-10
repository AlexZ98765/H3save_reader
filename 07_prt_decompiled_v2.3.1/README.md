# 07_prt_decompiled — ProspectorRT v2.3.1 реверс-код

> **Версия:** 2.3.1 (2026-11-10)
> **Источник:** `ProspectorRT.exe` (823 808 байт, .NET 2.0)
> **Декомпилятор:** ILSpy 8.2 (C# source) + ikdasm (IL dump) + monodis (metadata)
> **Статус:** ✅ Патчи применены, exe пересобран и работает

Эта папка содержит полные результаты дизасемблирования утилиты ProspectorRT v2.3.1
с применёнными патчами для чтения расширенных SoD-карт.

## Структура

```
07_prt_decompiled/
├── ProspectorRT_source/          C# исходник + DLLs + build.sh
│   ├── ProspectorRT/             MainForm.cs (патчи A-J), TownNameValidator.cs, ...
│   ├── dlls/                     12 DevExpress v11.1 DLLs (включены)
│   ├── resources/                10 .resources + .resx файлов
│   ├── ProspectorRT.csproj       для Visual Studio
│   ├── build.sh                  для Linux сборки
│   └── app.ico                   иконка приложения
├── ProspectorRT_IL/              полный IL дамп (ikdasm)
│   └── full_il_dump.txt
├── ProspectorRT_metadata/        метаданные сборки (monodis)
│   ├── ProspectorRT.exe_strings.txt
│   ├── ProspectorRT_fields.txt
│   ├── ProspectorRT_methods.txt
│   ├── ProspectorRT_typedefs.txt
│   ├── ProspectorRT_typerefs.txt
│   └── ProspectorRT_user_strings.txt
├── Data/                         игровые данные (SoD_Ru.txt, SoD_En.txt, ...)
├── README.md                     этот файл
└── Readme.txt                    оригинальный Readme от ProspectorRT
```

## Патчи v2.3.1 (всё в MainForm.cs)

| Патч | Метод | Описание |
|------|-------|----------|
| A | ScanTownsContent | bounds-checked, cap count 80 |
| B | GetTownContent | bounds-checked + name char validation |
| C | GetSenseRegion | chain bounds-checked (Mine→...→Town) |
| D | ScanHeroesContent | bounds-checked, cap 200 |
| E | ScanMonolithWhirlpool | bounds-checked, cap 256 |
| H | FindTownSection | town section fallback scan (ключевой фикс!) |
| I | SaveScroll | safe casts (ToString/Convert вместо (string)/(int)) |
| J | SafeSpellSlot | helper для безопасного lookup spell slot |
| FIXED1 1 | GetHeroesContent | R_AllArts: banned heroes артефакты |
| FIXED1 2 | GetAllSpell | R_AllSpell: banned heroes в фильтре |
| FIXED1 3 | GetAllSpell | tavern/тюрьма mislabel fix |
| Ext SoD | GetFileInfo | version 31/32 accepted as "SoD" |
| Debug | Scanner | try/catch + WriteDebugLog |

## Новый файл
- `TownNameValidator.cs` — `IsPrintableTownChar(byte)` helper

## Сборка
См. `ProspectorRT_source/README.md` и `ProspectorRT_source/build.sh`.
DLLs включены в `ProspectorRT_source/dlls/` — ничего скачивать не нужно.
