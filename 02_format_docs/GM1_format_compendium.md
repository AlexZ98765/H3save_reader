# Полная сводка формата `.GM1` сейвов Heroes of Might and Magic III

> **Версия документа:** 2.0 (2026-10-08)
> **Метод:** Дифференциальный анализ реальных сейвов (12 пар сейвов)
> **Версия игры:** HotA / SoD 4.2 (version_major=0x2A, version_minor=0x02)
> **Локализовано полей:** ~100 конкретных смещений

---

## 1. Общая структура файла

`.GM1` файл — это **gzip-сжатый бинарный файл**. После разжатия:

| Смещение | Размер | Назначение |
|----------|--------|-----------|
| `0x000000` | ~950 | Заголовок сейва (magic, version, map name, save filename) |
| `0x000400`-`0x06FFFF` | ~450 KB | Hero alt blocks (scattered, variable stride) |
| `0x070000`-`0x0CFFFF` | ~380 KB | Hero blocks (continued) + game state |
| `0x0D0000`-`0x13FFFF` | ~450 KB | Player state, town records, object owners |
| `0x140000`-`0x16FFFF` | ~200 KB | Hero main block + visiting coords |
| `0x170000`-`0x17FFFF` | ~64 KB | Hero skills table + map event counter |
| `0x180000`-`<end-18>` | variable | Path-block (история движений) |
| `<end-18>` | 18 | Footer "HD3\0" + size/CRC + padding |

### Важные особенности:
1. **Hero blocks имеют переменный stride** — они разбросаны по файлу, не подряд
2. **Сжатие gzip с "битым" CRC** — игра пишет неправильный trailer. Парсер должен обходить проверку CRC (аналогично `h3sed`)
3. **Footer "HD3\0"** — маркер конца файла, его позиция = `file_size - 18`
4. **Path-block растёт динамически** — добавляет записи переменной длины

---

## 2. Заголовок файла

| Смещение | Размер | Поле | Тип | Описание |
|----------|--------|------|-----|---------|
| `0x00` | 5 | magic | ASCII | `"H3SVG"` (сценарий) или `"H3SVC"` (кампания) |
| `0x08` | 1 | version_major | u8 | `0x2A`=HotA/SoD, `0x20`=SoD 4.0, `0x1F`=AB, `0x0F`=RoE |
| `0x0C` | 1 | version_minor | u8 | `0x02` для HotA 1.x |
| `0x3B6` | 10 | save_filename | ASCII | Последние 10 символов имени файла, null-terminated |

---

## 3. Версии игры

| Major | Minor | Расширение |
|-------|-------|-----------|
| `0x0E` | - | RoE 1.0 (Restoration of Erathia) |
| `0x0F` | - | RoE |
| `0x1E` | - | AB 1.0 (Armageddon's Blade) |
| `0x1F` | - | AB |
| `0x20` | - | SoD 4.0 (Shadow of Death) |
| `0x21` | - | SoD 4.x |
| `0x2A` | `0x02` | HotA (Horn of the Abyss) |
| `0x2B` | - | HotA (новее) |

---

## 4. Состояние игрока (Player State)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x13E112` | 1 | u8 | surface_flag_1 | Surface flag 1 (toggles at disembark) |
| `0x13E119` | 1 | u8 | surface_flag_2 | Surface flag 2 (toggles at disembark) |
| `0x13E2FD` | 6 | bytes | action_state | Action state block |
| `0x13E408` | 1 | u8 | action_counter | Action counter |
| `0x13E409` | 1 | u8 | last_visited_object_id | Last visited object ID / action flag |
| `0x13E41F` | 1 | u8 | action_counter_2 | Action counter 2 |
| `0x13E420` | 1 | u8 | last_built_building_id | ID последнего построенного здания |
| `0x13E425` | 1 | u8 | owner_change_flag | Owner change flag |
| `0x13E469` | 4 | u32 LE | player_wood | Дерево игрока |
| `0x13E481` | 2 | u16 LE | player_gold | Золото игрока |
| `0x13F17B` | 1 | bool | building_built_flag | Флаг постройки здания |
| `0x13F220` | 4 | u32 LE | player_resource_alt | Ресурс игрока (alt storage) |
| `0x13F647` | 2 | u16 LE | player_resource_swap | Ресурс игрока (byte-swapped) |

---

## 5. Вражеский герой (Enemy Hero State)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x13E2E6` | 1 | u8 enum | enemy_hero_state | 6=active, 5=defeated |
| `0x13E2ED` | 4 | bytes | enemy_hero_swap | Swap field (bytes reorder on defeat) |
| `0x151C7F` | 1 | bool | defeated_flag | 1=active, 0=defeated |
| `0x151C93` | 4 | bytes | army_slots_cleared | Army slots (cleared on defeat) |
| `0x151CA6` | 8 | 2×u32 LE | visiting_coords | Visiting coords (reset to -1, -1) |
| `0x151CC2` | 1 | u8 enum | hero_state | 7=active, 2=banished |
| `0x151D02` | 43 | bytes | big_block_cleared | Big block (cleared on defeat) |

---

## 6. Карта объектов (Object Owners)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x13C850` | 1 | u8 enum | object_owner | Player color (0-7) или 0xFF=neutral. **Stride между объектами требует уточнения.** |

### Player colors:
```
0=Red, 1=Blue, 2=Tan, 3=Green, 4=Orange, 5=Purple, 6=Teal, 7=Pink, 0xFF=Neutral
```

---

## 7. Город (Town Record) — верифицировано h3sed

Структура из h3sed `TOWN_BYTE_POSITIONS` (offset = относительно начала town record):

| Offset | Размер | Тип | Поле | Описание |
|--------|--------|-----|------|---------|
| 0 | 1 | u8 enum | **faction** | Владелец (0-7 или 0xFF=neutral). Меняется при захвате! |
| 1 | 2 | bytes | unknown | Неизвестно |
| 3 | 1 | u8 enum | **type** | Тип города (0-8 SoD, 0-11 HotA). НЕ меняется при захвате. |
| 4 | 1 | u8 | **location_x** | X координата |
| 5 | 1 | u8 | **location_y** | Y координата |
| 6 | 1 | u8 | **location_z** | Z координата (0=overworld, 1=underground) |
| 7 | 2 | bytes | unknown | Неизвестно |
| 9 | 28 | 7×u32 LE | **army_types** | ID существ гарнизона (7 слотов, 0xFFFFFFFF=пусто) |
| 37 | 28 | 7×u32 LE | **army_counts** | Количество существ (7 слотов) |
| 65 | 4 | bytes | unknown | Неизвестно |
| 69 | 2 | u16 LE | **name_len** | Длина имени города |
| 71 | N | cp1251 | **name** | Имя города (переменная длина, max 14 символов) |

### Town types:
```
SoD: 0=Castle, 1=Rampart, 2=Tower, 3=Inferno, 4=Necropolis, 5=Dungeon,
     6=Stronghold, 7=Fortress, 8=Conflux
HotA: + 9=Cove, 10=Factory, 11=Bulwark
```

### Пример (из диффа 312_2→312_3, захват города "Патрас"):
- faction: 4 (Orange) → **3 (Green)** ⭐
- type: 0 (Castle) — без изменений
- location: (40, 103, 0) — без изменений
- army_types: [9, 1, -1, -1, -1, -1, -1] → **[-1, -1, -1, -1, -1, -1, -1]** ⭐ (очищено)
- army_counts: [12, 56, 0, 0, 0, 0, 0] → **[0, 0, 0, 0, 0, 0, 0]** ⭐ (очищено)
- name: "Патрас" — без изменений

### Размер town record:
- SoD: 382-396 байт (зависит от длины имени)
- HotA: 546-581 байт
- Максимальное количество городов: 48

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x13F60F` | 28 | 7×u32 LE | town_garrison_army_types_alt | Alt 7-слотовая версия |
| `0x13F62B` | 28 | 7×u32 LE | town_garrison_army_counts_alt | Alt 7-слотовая версия |
| `0x13F648` | ~8 | cp1251 | town_name | Имя города (cp1251) |
| `0x13F65F` | 1 | u8 | available_for_hire | Доступно для найма (уменьшается при найме) |
| `0x13F78C` | 1 | u8 enum | town_owner_color | Цвет владельца (0-7 или 0xFF) |
| `0x13F795` | 20 | 5×u32 LE | town_garrison_army_counts[5] | ⭐ Основной формат: 5 слотов! |
| `0x13F7B1` | 20 | 5×u32 LE | town_garrison_army_types[5] | ID существ (0=пусто) |
| `0x16A2D4` | 4 | u32 LE | black_market_artifact | Артефакт в Black Market |

### ⭐ ВАЖНО: У городов 7 слотов гарнизона (как и у героев)!

⚠️ Ранее я ошибочно писал про 5 слотов — на самом деле их 7, просто в диффе были заполнены только 5.

---

## 8. Visiting Hero (для города)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x143E6E` | 8 | 2×u32 LE | visiting_hero_coords | Координаты гостя (X, Y), (-1,-1)=нет гостя |
| `0x16368B` | 1 | bool | visiting_hero_flag | Флаг наличия гостя |
| `0x1636AE` | 1 | u8 | hero_slot_in_town | 2=guest, 7=inside garrison |

---

## 9. Герой — основной блок (Hero #1)

**Диапазон:** `0x157EC3` .. `0x164000`

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x157EC3` | 10 | UTF-16 LE | hero_xy_current | Текущие координаты (X, Y как односимвольные строки) |
| `0x157EE0` | 1 | u8 | hero_stat_counter_1 | Счётчик 1 |
| `0x157EEC` | 1 | u8 | hero_stat_counter_2 | Счётчик 2 |
| `0x157EF0` | 8 | 2×u32 LE | movement_bonus_values | Бонусы движения (напр. 51, 99) |
| `0x157F00` | 4 | u32 LE | current_movement_points | Текущее движение (чит=299999) |
| `0x157F04` | 4 | u32 LE | experience | Опыт (+1000 от Learning Stone) |
| `0x157F08` | 1 | u8 | level | Уровень |
| `0x157F0E` | 4 | u32 LE | skill_count | Количество навыков |
| `0x157F12` | 2 | u16 LE | new_skill_data | Данные нового навыка |
| `0x157F4D` | 1 | bitmask | visited_objects_low | Битовая маска посещённых объектов (low byte) |
| `0x157F4E` | 1 | bitmask | visited_objects_high | Битовая маска (high byte) |
| `0x157FA4` | 1 | u8 | skill_slot_flag | Флаг слота навыка |
| `0x157FC0` | 1 | u8 | skill_level | Уровень навыка |
| `0x157FCD` | 1 | u8 | skill_counter | Счётчик навыков |
| `0x1636B2` | 8 | 2×u32 LE | queued_path_destination | Целевая точка пути (X, Y) |
| `0x163718` | 28 | 7×u32 LE | hero_army_types | ID существ (7 слотов, 0xFFFFFFFF=пусто) |
| `0x163734` | 28 | 7×u32 LE | hero_army_counts | Количество существ (7 слотов) |
| `0x163794` | 1 | bool | has_spell_book | Наличие книги магии |
| `0x1637A0` | ~146 | bit array | spell_bit_array | Битовая маска известных заклинаний |
| `0x163885` | 4 | u32 LE | ballista_slot | Слот баллисты (ID=4) |
| `0x163889` | 4 | u32 LE | ammo_cart_slot | Слот Ammo Cart |
| `0x16388D` | 4 | u32 LE | first_aid_tent_slot | Слот First Aid Tent |
| `0x163891` | 4 | u32 LE | catapult_slot | Слот Catapult |
| `0x1638A5` | 4 | u32 LE | spell_book_slot | Слот Spell Book |

### Битовая маска посещённых объектов (`0x157F4D` + `0x157F4E`)

Это **16-битная маска** для бонус-объектов с **boolean weekly modifiers**:

| Бит | Смещение | Объект | Тип бонуса |
|-----|----------|--------|-----------|
| 4 | `0x157F4D` | ⭐ Fountain of Fortune | Weekly (luck) |
| 0 | `0x157F4E` | ⭐ Upgrade Fort | Weekly (upgrade) |
| 3 | `0x157F57` | ⭐ **Marletto Tower** | Permanent (+1 defense) |
| 4 | `0x157F6F` | ⭐ **Arena** | Permanent (+2 defense) |
| 5 | `0x157F5F` | ⭐ **Mercenary Camp** | Permanent (+1 attack) |

⚠️ Битовая маска **разбросана по разным байтам** (не непрерывна)! Каждый тип бонус-объекта имеет свой байт.

⚠️ **Не все посещения устанавливают биты:**
- ❌ Learning Stone (даёт +1000 XP один раз) — отслеживается per-object
- ❌ Stables (даёт +movement) — отслеживается через visit_counter
- ✅ Fountain of Fortune, Upgrade Fort — boolean модификаторы

---


## 🎯 ПОЛНАЯ СТРУКТУРА HERO BLOCK (верифицирована h3sed)

Все смещения ниже — **относительно faction byte** (offset 0 в h3sed).

В сейвах "Myth and Legend.h3m" faction byte = `0x157F1E`.

| h3sed offset | Абсолютное смещение | Размер | Поле | Тип |
|--------------|---------------------|--------|------|-----|
| -26 | `0x157F04` | 1 | location_x | u8 |
| -25 | `0x157F05` | 1 | location_y | u8 |
| -24 | `0x157F06` | 1 | location_z | u8 |
| -20 | `0x157F0A` | 1 | on_map | u8 bool |
| 0 | `0x157F1E` | 1 | faction | u8 enum |
| 31 | `0x157F3D` | 4 | movement_total | u32 LE |
| 35 | `0x157F41` | 4 | movement_left | u32 LE |
| 39 | `0x157F45` | 4 | experience | u32 LE |
| 43 | `0x157F49` | 4 | skills_count | u32 LE |
| 47 | `0x157F4D` | 2 | mana_left | u16 LE |
| 49 | `0x157F4F` | 1 | level | u8 |
| 113 | `0x157F8F` | 28 | army_types[7] | 7×u32 LE |
| 141 | `0x157FAB` | 28 | army_counts[7] | 7×u32 LE |
| 169 | `0x157F87` | 13 | hero_name | ASCII |
| 182 | `0x157F90` | 30 | skill_levels | bytes |
| 238 | `0x15800C` | 1 | attack | u8 |
| 239 | `0x15800D` | 1 | defense | u8 |
| 240 | `0x15800E` | 1 | power | u8 |
| 241 | `0x15800F` | 1 | knowledge | u8 |
| 242 | `0x158010` | 70 | spells_book | bit array |
| 312 | `0x158058` | 70 | spells_available | bit array |
| 382 | `0x15809C` | 152 | equipment (19 slots × 8b) | bytes |
| 534 | `0x158134` | 512 | inventory (64 slots × 8b) | bytes |
| 1092 | `0x158362` | 30 | skills_slot (HotA) | bytes |

### Дополнительные поля (не в h3sed, найдены нами):
| Абсолютное смещение | Размер | Поле | Описание |
|---------------------|--------|------|-----------|
| `0x157F31` | 8 | queued_path_destination | 2×u32 LE, цель пути |
| `0x157F57` | 1 | visited_marletto_tower | bitmask бит 3 |
| `0x157F6F` | 1 | visited_arena | bitmask бит 4 |
| `0x157F5F` | 1 | visited_mercenary_camp | bitmask бит 5 |

## 10. Герой — alt блоки (Hero #2-#5)

⚠️ **Hero blocks имеют переменный stride** — они разбросаны по файлу!

| Hero | Movement byte addr | Visit counter addr |
|------|---------------------|---------------------|
| Hero #1 (main) | `0x06D7A4` | `0x06D7AA` |
| Hero #2 | `0x06B716` | `0x06B71C`, `0x06B780` |
| Hero #3 | `0x06F998` | `0x06F99E` |
| Hero #4 | `0x05AAC1` | - |
| Hero #5 (defender, boat) | `0x05B92D` | - |

### Hero alt block (общие поля)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x059BE4` | 1 | u8 enum | hero_on_boat_flag | 0x22=boat, 0x08=land |
| `0x059BEA` | 1 | u8 enum | hero_surface_flag | 0x01=water, 0x06=land |
| `0x140AF9` | 12 | UTF-16 LE | visiting_coords_current | Текущие visiting coords |
| `0x140B22` | 1 | u8 | hero_state_byte | State byte |
| `0x140B26` | 8 | 2×u32 LE | map_object_visiting_coords | Координаты гостя объекта |
| `0x140B30` | 4 | u32 LE | hero_level_counter | Счётчик уровней (alt) |
| `0x140B34` | 4 | u32 LE | movement_points_alt_1 | Movement (alt 1) |
| `0x140B36` | 4 | u32 LE | movement_points_alt_2 | Movement (alt 2) |
| `0x140B3C` | 4 | u32 LE | movement_total_alt | Movement total |
| `0x140B44` | 1 | u8 | hero_level_alt | Уровень героя (alt), +2 за победу |
| `0x140B82` | 1 | u8 | hero_on_boat_flag_2 | Флаг на лодке (alt) |
| `0x140B84` | 8 | 2×u32 LE | hero_coords_alt | Координаты героя (alt) |
| `0x140B9C` | 4 | u32 LE | hero_artifact_slot_defender | Слот артефакта (защитник) |
| `0x140BA0` | 5 | bytes | army_count_slot_alt | Count + slot_index |
| `0x140BD9` | 1 | u8 | skill_count_alt | Количество навыков (alt) |
| `0x140BE7` | 1 | u8 | skill_improvement_flag | Флаг улучшения навыка |
| `0x140C01` | 2 | 2×u8 | skill_level_alt | Уровень навыка (alt) |
| `0x141385` | 10 | UTF-16 LE | hero_5_xy | Координаты Hero #5 (в лодке) |
| `0x14138AE` | 1 | u8 | hero_5_movement_counter | Счётчик движения |
| `0x14138C2` | 2 | u16 LE | hero_5_movement | Movement (u16 LE) |

---

## 11. Глобальные счётчики ходов

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x143E41` | 1 | u8 | turn_counter_1 | +1 за ход |
| `0x143E48` | 1 | u8 | turn_counter_2 | +1 за ход |
| `0x143E6E` | 8 | 2×u32 LE | visiting_coords_reset | Сбрасывается в -1,-1 |
| `0x143E7E` | 2 | u16 LE | hero_movement_alt | Movement (alt) |

---

## 12. Артефакты

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x16A19C` | 1 | u8 | artifact_slot_winner | Слот победителя (получает трофей) |
| `0x16A1B4` | 1 | u8 | artifact_slot_loser | Слот проигравшего (очищается) |
| `0x16A2D4` | 4 | u32 LE | black_market_artifact | Артефакт в Black Market |

---

## 13. Cheater flag

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x16A29D` | 1 | bool | cheater_flag | 0=normal, 1=cheated |

Устанавливается при использовании **любого** чит-кода (nwcneo, nwcoracle, и т.д.)

---

## 14. Path-block (история движений)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x130B71` | ~190 | ASCII | base64_metadata | Меняется при каждом сохранении |
| `0x17EC37` | 1 | u8 | map_event_counter | +N при добавлении path-записей |
| `0x1814D0` | ~256 | path records | path_block_main | Основной блок истории |
| `0x18179D` | variable | path records | path_block_extension | Расширение (растёт с новыми записями) |

### 14.1. Типы path-записей

#### `01 03` — Движение героя (15 байт)
```
u8  flag1   = 0x01
u8  flag2   = 0x03
u32 counter (LE)  — visit_id
u8  step_n        — номер шага в multi-step move
u16 from_X (LE)
u16 from_Y (LE)
u16 to_X   (LE)
u16 to_Y   (LE)
```

#### `03 03` — Capture event (9 байт)
```
u8  flag1   = 0x03
u8  flag2   = 0x03
u32 counter (LE)  — visit_id захвата
u8  prev_owner    — бывший владелец (4=Orange, и т.д.)
u8  object_type
u8  terminator = 0x0b
```

#### `04 03` — Battle event с городом (~10 байт)
```
u8  flag1   = 0x04
u8  flag2   = 0x03
u32 counter (LE)  — visit_id битвы
u8[4] data        — информация о битве
```

#### `06 03` — Disembark subrecord (переменный)
Вложенная запись внутри `08 03`. Содержит координаты `(from_X, from_Y) → (to_X, to_Y)`.

#### `08 03` — Battle outcome / Army transfer / Disembark

**Компактная форма (9 байт) — battle outcome:**
```
u8  flag1      = 0x08
u8  flag2      = 0x03
u32 counter    (LE) — visit_id битвы
u8  result     — encoded result (0xff)
u8  outcome    — 1 = победа
u8  terminator = 0x0b
```

**Полная форма (27 байт) — army transfer / disembark:**
```
u8  flag1   = 0x08
u8  flag2   = 0x03
u32 counter (LE)
u8  n1
u8  n2
u8[19] subrecord — вложенная запись (например 06 03 для disembark)
```

#### `09 03` — Visit event (19 байт)
```
u8  flag1   = 0x09
u8  flag2   = 0x03
u32 counter (LE)  — visit_id
u8  n
u16 from_X (LE)
u16 from_Y (LE)
u16 to_X   (LE)
u16 to_Y   (LE)
u8[4] tail
```

#### `0b 03` — Fog of war update (4 + N×8 байт)
```
u8  flag1   = 0x0b
u8  flag2   = 0x03
u8  n             — количество пар координат
u8  pad = 0x00
u16[n][2] pairs (LE) — массив N пар (X, Y), каждая 4 байта
```

---

## 15. Footer (последние 18 байт)

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `<end-18>` | 4 | ASCII | footer_magic | `"HD3\0"` |
| `<end-14>` | 3 | bytes | footer_size_or_crc | Size/CRC (меняется с размером файла) |
| `<end-11>` | 11 | bytes | footer_padding | Padding (всегда нули) |

---

## 16. Movement cost — детали

Базовая стоимость движения (без модификаторов):

| Тип движения | Cost (movement points) |
|--------------|------------------------|
| По горизонтали | **100** |
| По вертикали | **100** |
| По диагонали | **141** (≈ 100 × √2) |

### Модификаторы стоимости:
- **Тип почвы** — разные terrain types дают разный penalty
- **Дороги** — снижают стоимость до ~75-50% от базовой
- **Артефакты** — некоторые дают bonus к movement (например, Boots of Speed)
- **Класс героя** — родная почва даёт бонус (например, Ranger на траве)
- **Бонусы от зданий** — Stables даёт +movement до конца недели
- **Лодка** — передвижение по воде имеет свою стоимость

⚠️ Это **не** разная стоимость "на воде vs на суше" — это **разные базовые стоимости** для разных направлений (orthogonal vs diagonal).

---

## 17. Что НЕИЗВЕСТНО / НЕОПИСАНО

Эти блоки пока не локализованы и требуют дальнейших дифференциальных анализов:

- ❌ Карта мира (террайн, тайлы) — не локализована
- ❌ Полная битовая маска fog of war (отдельно от path-records)
- ❌ Состояние всех объектов карты (открытые сундуки, посещённые хижины)
- ❌ Состояние AI игроков (стратегии, память)
- ❌ Текущий день / неделя / месяц
- ❌ Текущий ход игрока
- ❌ Дипломатия / альянсы
- ❌ Random seed / состояние RNG
- ❌ Quest log (Seer Huts, Border Guards)
- ❌ Биография героя (переменная длина, перед hero stats)
- ❌ Полный список зданий города (только last_built + flag известны)
- ❌ Доступные заклинания в Magic Guild
- ❌ Hero primary stats (attack/defense/power/knowledge) — частично из h3sed

---

## 18. Источники

- Дифференциальный анализ 12 пар реальных сейвов (001.GM1 → 0020_9.GM1)
- Дизассемблирование `heroes3.exe` (PE32, SoD build)
- `h3sed` (suurjaak) — для базовой структуры hero block
- `AlexSnowLeo/Heroes3Editor`, `vlucas/homm3-savegame-editor` — для общих паттернов

## 19. Файлы проекта

- `gm1_mapping.json` — машина-читаемый конфиг всех известных смещений
- `gm1_parser.py` — PySide6 GUI парсер
- `gm1_diff.py` / `gm1_diff_gui.py` — дифференциальный компаратор сейвов
- `disasm/` — результаты дизассемблирования heroes3.exe


## 20. Кросс-репозиторная верификация

Все 4 публичных save editor'а сравнены:

| Репозиторий | Базовая точка | Полей | Уникальных находок |
|-------------|---------------|-------|---------------------|
| h3sed (suurjaak) | faction byte (offset 0) | 35+ | Town structure, path records, HotA skills_slot |
| vlucas | hero name (offset 169) | 28 | CoordinatesXMarker, CoordinatesYMarker, BlockedSlots |
| cysun | hero name (offset 169) | 25 | — (subset of vlucas) |
| svetoslav | hero name (offset 169) | 12 | — (minimal subset) |

### Новые поля из vlucas (не в h3sed):
| h3sed offset | Поле | Описание |
|--------------|------|-----------|
| 19 | coordinates_x_marker | Маркер X (возможно queued path X) |
| 23 | coordinates_y_marker | Маркер Y (возможно queued path Y) |
| 1047 | blocked_slots | Заблокированные слоты комбинированных артефактов |

### Расхождение:
- vlucas: CoordinatesZ = h3sed offset -22
- h3sed: location_z = offset -24
- Разница: 2 байта. h3sed вероятно правильный (более строгая валидация).


## 21. Game State — Day Counter

| Смещение | Размер | Тип | Поле | Описание |
|----------|--------|-----|------|---------|
| `0x17F494` | 1 | u8 | **DAY COUNTER** | Текущий день (1-7 внутри недели). Сбрасывается в 1 при новой неделе. |

### Подтверждено:
- End Turn: день 2 → день 3 (дифф 312_6→313_1)
- `movement_left` сбрасывается к `movement_total` при наступлении нового дня


## 22. Расшифровка "unknown" секции hero block (offsets 1-30)

h3sed помечает offsets 1-30 как "unknown". Дифференциальный анализ заполняет пробелы:

| h3sed offset | Абсолютное (312/xx) | Поле | Описание |
|--------------|---------------------|------|----------|
| +3 | hero_start+3 | battle_outcome_flag | 253=before battle, 0=after victory |
| +4 | hero_start+4 | artifacts_collected_counter | +1 при подборе артефакта |
| +15 | hero_start+15 | hero_action_counter | +1 за каждое действие (move, fight, pickup) |
| +29 | hero_start+29 | mana_bonus_or_spellpoints | +2 при level up |

### Подтверждено в диффах xx_01→xx_05 (герой Артемида):
- exp +726 (убийство монстра), +750 (монстр), +1500 (сундук), 0 (артефакт)
- level 16→17 (+1 от сундука)
- defense 12→13 (+1 выбран при level up)
- skills_count 7→8 (новый навык при level up)
- movement: -141 (диагональ), -100 (orthogonal), -341 (2 шага)


## 23. Карта "Myth and Legend.h3m" — связь с сейвом

### Координаты: исправленный порядок байтов

h3sed помечает: `location_x=-26, location_y=-25, location_z=-24`

**Реальный порядок**: **X, Z, Y** (не X, Y, Z)!

| Байт | h3sed метка | Реальное значение |
|------|-------------|-------------------|
| raw[-26] | location_x | **X** ✅ |
| raw[-25] | location_y | **Z** (0=surface, 1=underground) ⚠️ |
| raw[-24] | location_z | **Y** ⚠️ |

### Смещение координат карта → сейв

| Объект | Координаты на карте | Координаты в сейве | Δ X |
|--------|---------------------|---------------------|-----|
| Town "Патрас" | (42, 103) | (40, 103) | -2 |
| Town "Спарта" | (67, 101) | (65, 101) | -2 |
| Town @ (102, 83) | (102, 83) | (100, 83) | -2 |

**Сейв использует X-2 от карты** (города занимают 2×2 тайла, сейв = верхний-левый угол).

### Карта:
- Размер: 144×144 тайлов
- Underground: да
- Тип: SoD
- Объектов: 7009
- Городов: 21 (Tower, Castle, Rampart, Dungeon, Inferno)
- Героев: 155 настроек
