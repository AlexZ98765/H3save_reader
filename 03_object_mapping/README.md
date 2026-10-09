# Object Mapping — Universal Dictionary

> Эта папка содержит **только универсальный словарь типов объектов** HoMM3.
> Все данные, специфичные для конкретной карты (`objects_by_coord.json`, `coord_int_lookup.json`,
> `type_index.json`, `category_index.json`, и т. д.), больше НЕ хранятся в этом репозитории —
> они строятся на лету при загрузке JSON карты (см. `01_tools/map_json_loader.py`).

## Файлы в этой папке

| Файл | Описание |
|------|----------|
| `object_types_dictionary.json` (968 KB) | Универсальный словарь 2037 типов объектов HoMM3 из LazyLlama wiki. Ключ — `.def` file name (lowercase). Содержит `_categories`, `_name_index`, `_id_index` для поиска. Поле `user_edits` сохраняется при пересоздании словаря. |
| `README.md` | Этот файл |

## Источник словаря

[https://heroes.thelazy.net/index.php/Map_Editor_Objects](https://heroes.thelazy.net/index.php/Map_Editor_Objects) (revision 194767, 2086 строк таблицы).

Для обновления словаря:
```bash
curl -o /tmp/lazymap.html https://heroes.thelazy.net/index.php/Map_Editor_Objects
python3 04_diff_analysis/build_object_type_dictionary.py
```

## Структура `object_types_dictionary.json`

```jsonc
{
  "_meta": {
    "source": "https://heroes.thelazy.net/index.php/Map_Editor_Objects",
    "page_revision": 194767,
    "fetched_at": "2026-10-08",
    "total_object_types": 2086
  },
  "_categories": { "Artifacts": [...], "Monsters": [...], "Towns": [...], ... },
  "_name_index": { "Mountain": ["avlmtd01.def", ...], ... },
  "_id_index":   { "5:136": "ava0137.def", ... },
  "objects": {
    "ava0001.def": {
      "wiki_name":    "Spell Scroll",
      "category":     "Artifacts",
      "description":  "This scroll contains a spell...",
      "user_edits":   {
        "user_notes":    "",
        "user_category": "",
        "user_alias":    ""
      }
    },
    ...
  }
}
```

## Статистика

- 2037 уникальных `.def` файлов
- 819 уникальных Object Names
- 857 уникальных пар ObjectID:SubID
- 53 категории (Artifacts, Monsters, Towns, Dwellings, Terrains, ...)

## Что было удалено из этой папки

В предыдущих версиях репозитория (v2.0–v2.7) здесь также хранились предрассчитанные
JSON-файлы для карты "Myth and Legend.h3m":

| Удалённый файл | Замена |
|----------------|--------|
| `objects_by_coord.json` (1.8 MB) | `MapData.objects_by_coord` — строится динамически из `.h3m.json` через `map_json_loader.py` |
| `objects_flat.json` (2.1 MB) | `MapData.objects_by_coord` + `MapData.category_index` — плоская иерархия строится при необходимости |
| `coord_int_lookup.json` (128 KB) | `MapData.coord_int_lookup` — строится динамически |
| `type_index.json` (98 KB) | `MapData.type_index` — строится динамически |
| `category_index.json` (95 KB) | `MapData.category_index` — строится динамически |
| `object_mapping_summary.json` | Считается из `MapData` (метод `summary()`) |
| `coord_mapping_verification.json` (556 KB) | Разовый артефакт проверки для "Myth and Legend.h3m" — не нужен в runtime |
| `save_offset_clusters.json` / `.md` | Разовый артефакт. Кластеризация теперь делается `map_config_builder.py::find_object_clusters` |
| `map_types_xref.json` | Кросс-референс одной карты с wiki. Строится на лету через `MapData.type_index` + `_name_index` словаря |

**Причина:** Все эти файлы были **хардкодом для одной конкретной карты**. Универсальный парсер
теперь строит их динамически из `.h3m.json` для любой карты (см. `01_tools/map_json_loader.py`).

## Использование

```python
import json
from map_json_loader import load_map_json_safely

# Загрузить словарь типов (универсальный)
types_dict = json.load(open('03_object_mapping/object_types_dictionary.json'))

# Загрузить конкретную карту
md, err = load_map_json_safely('examples/MyMap.h3m.zip')

# Lookup объекта по coord_int (например, 879 = "111:3:0")
coord_key = md.coord_int_lookup[879]
obj = md.objects_by_coord[coord_key]
print(f"({obj['x']},{obj['y']},{obj['z']}) {obj['type']} / {obj['category']}")

# Если нужно описание типа из wiki:
def_file = obj.get('sprite_def', '').lower()
if def_file in types_dict['objects']:
    info = types_dict['objects'][def_file]
    print(f"  Wiki: {info['wiki_name']} — {info['description'][:80]}")
```
