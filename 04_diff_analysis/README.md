# 04_diff_analysis — Differential Analysis Scripts

Эта папка содержит только **универсальный** скрипт парсинга LazyLlama wiki.

**Исторические разовые diff-скрипты** (использовавшиеся для ручного локализования
полей формата `.GM1` на карте "Myth and Legend") перенесены в
[`../_obsolete/04_diff_analysis/Myth and Legend/`](../_obsolete/04_diff_analysis/Myth%20and%20Legend/)
— это исследовательский архив, не нужный в runtime.

## Структура (актуальная v3.10)

```
04_diff_analysis/
├── README.md                         (этот файл)
└── build_object_type_dictionary.py   ⭐ универсальный — парсит LazyLlama wiki
                                      (не привязан к карте)
```

## Универсальные скрипты

| Скрипт | Назначение |
|--------|------------|
| `build_object_type_dictionary.py` | Парсит HTML-страницу LazyLlama wiki (`Map_Editor_Objects`) и строит `03_object_mapping/object_types_dictionary.json` (2037 типов объектов). Не привязан к конкретной карте. |

## Исторические подпапки

Все исторические diff-скрипты (для карты "Myth and Legend") перенесены в
`_obsolete/04_diff_analysis/Myth and Legend/` — 25 `analyze_*.py` скриптов
+ `build_*.py` + `find_*.py` + `verify_*.py` + `regenerate_*.py` + отчёты.
Эти скрипты НЕ вызываются в runtime и оставлены как исследовательский архив.
