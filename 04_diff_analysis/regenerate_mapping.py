#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Генератор mapping JSON. Переписывает gm1_mapping.json с hex-строковыми смещениями,
чтобы избежать ошибок конвертации hex→decimal.
"""
import json

MAPPING = {
    "format_version": "2.0",
    "last_updated": "2026-10-08",
    "format_name": "Heroes of Might and Magic III - Shadow of Death / HotA save file (.GM1)",
    "notes": [
        "All offsets are RELATIVE TO THE START of the decompressed (gzip) save data.",
        "All offsets are HEX STRINGS (e.g. '0x157EC3') to avoid decimal conversion errors.",
        "All multi-byte integers are LITTLE-ENDIAN unless stated otherwise.",
        "",
        "⚠️ IMPORTANT: All concrete offsets below are VALID ONLY for the same map!",
        "Offsets were derived via differential analysis of saves from the map",
        "'Myth and Legend.h3m'. Different maps will have DIFFERENT absolute offsets",
        "because hero/town/object counts differ, which shifts all subsequent data.",
        "",
        "Hero blocks have VARIABLE STRIDE — they are scattered across the file.",
        "Town garrison uses 5 slots (not 7 like hero army).",
        "",
        "Movement cost: 100 per horizontal/vertical step, 141 per diagonal step.",
        "Movement cost is ALSO modified by: terrain, roads, artifacts, hero class",
        "(native terrain bonus), building bonuses (e.g. Stables).",
        "",
        "To find blocks in a NEW save (different map), use the 'Find Hero Blocks'",
        "feature in the parser GUI, or extend the search_patterns section below."
    ],
    "constants": {
        "MAGIC_SCENARIO": "H3SVG",
        "MAGIC_CAMPAIGN": "H3SVC",
        "FOOTER_MAGIC": "HD3",
        "PLAYER_COLORS": {
            "0": "Red",
            "1": "Blue",
            "2": "Tan",
            "3": "Green",
            "4": "Orange",
            "5": "Purple",
            "6": "Teal",
            "7": "Pink",
            "255": "Neutral"
        },
        "VERSION_MAJOR": {
            "0x0E": "RoE 1.0",
            "0x0F": "RoE",
            "0x1E": "AB 1.0",
            "0x1F": "AB",
            "0x20": "SoD 4.0",
            "0x21": "SoD 4.x",
            "0x2A": "HotA",
            "0x2B": "HotA"
        }
    },

    "blocks": [
        {
            "name": "header",
            "description": "Save file header (magic, version, map name, filename)",
            "fields": [
                {"name": "magic", "offset": "0x0", "size": 5, "type": "ascii",
                 "description": "Magic bytes: H3SVG (scenario) or H3SVC (campaign)"},
                {"name": "version_major", "offset": "0x8", "size": 1, "type": "u8_hex",
                 "description": "Major version byte (0x2A = SoD/HotA)"},
                {"name": "version_minor", "offset": "0xC", "size": 1, "type": "u8_hex",
                 "description": "Minor version byte"},
                {"name": "save_filename", "offset": "0x3B6", "size": 10, "type": "ascii",
                 "description": "Save file name (last 10 chars of filename, ASCII null-terminated)"}
            ]
        },

        {
            "name": "player_state",
            "description": "Player global state (action flags, resources)",
            "fields": [
                {"name": "surface_flag_1", "offset": "0x13E112", "size": 1, "type": "u8",
                 "description": "Surface flag 1 (toggles at disembark: 1=water→0, 0=land→1)"},
                {"name": "surface_flag_2", "offset": "0x13E119", "size": 1, "type": "u8",
                 "description": "Surface flag 2 (toggles at disembark: 0=water→1, 1=land→0)"},
                {"name": "action_state", "offset": "0x13E2FD", "size": 6, "type": "bytes",
                 "description": "Action state block (6 bytes)"},
                {"name": "action_counter", "offset": "0x13E408", "size": 1, "type": "u8",
                 "description": "Action counter"},
                {"name": "last_visited_object_id", "offset": "0x13E409", "size": 1, "type": "u8",
                 "description": "Last visited object ID / action flag (changes on visit)"},
                {"name": "action_counter_2", "offset": "0x13E41F", "size": 1, "type": "u8",
                 "description": "Action counter 2"},
                {"name": "last_built_building_id", "offset": "0x13E420", "size": 1, "type": "u8",
                 "description": "ID of last built building"},
                {"name": "owner_change_flag", "offset": "0x13E425", "size": 1, "type": "u8",
                 "description": "Owner change flag"},
                {"name": "player_wood", "offset": "0x13E469", "size": 4, "type": "u32_le",
                 "description": "Player wood resource (u32 LE)"},
                {"name": "player_gold", "offset": "0x13E481", "size": 2, "type": "u16_le",
                 "description": "Player gold (u16 LE)"},
                {"name": "building_built_flag", "offset": "0x13F17B", "size": 1, "type": "u8_bool",
                 "description": "Building built flag (bool)"},
                {"name": "player_resource_alt", "offset": "0x13F220", "size": 4, "type": "u32_le",
                 "description": "Player resource (alt storage)"},
                {"name": "player_resource_swap", "offset": "0x13F647", "size": 2, "type": "u16_le",
                 "description": "Player resource (byte-swapped storage)"}
            ]
        },

        {
            "name": "enemy_hero_state",
            "description": "Enemy hero state (for tracking defeated/fled enemies)",
            "fields": [
                {"name": "enemy_hero_state", "offset": "0x13E2E6", "size": 1, "type": "u8_enum",
                 "values": {"6": "active", "5": "defeated"},
                 "description": "Enemy hero state (6=active, 5=defeated)"},
                {"name": "enemy_hero_swap", "offset": "0x13E2ED", "size": 4, "type": "bytes",
                 "description": "Enemy hero swap field (bytes reorder on defeat)"}
            ]
        },

        {
            "name": "object_owners",
            "description": "Map object owner array (mines, dwellings, etc.)",
            "fields": [
                {"name": "object_owner_example", "offset": "0x13C850", "size": 1, "type": "u8_enum",
                 "values": "PLAYER_COLORS",
                 "description": "Example slot of object owner array. Each map object (mine, dwelling) has 1 byte with player color (0-7) or 0xFF=neutral. Stride between objects needs further investigation."}
            ]
        },

        {
            "name": "town",
            "description": "Town record (garrison, owner, name, available units)",
            "fields": [
                {"name": "town_garrison_army_types_alt", "offset": "0x13F60F", "size": 28, "type": "u32_le_array",
                 "array_count": 7,
                 "description": "Town garrison army_types (alt 7-slot version, 7 × u32 LE creature IDs, 0xFFFFFFFF=empty)"},
                {"name": "town_garrison_army_counts_alt", "offset": "0x13F62B", "size": 28, "type": "u32_le_array",
                 "array_count": 7,
                 "description": "Town garrison army_counts (alt 7-slot version, 7 × u32 LE creature counts)"},
                {"name": "available_for_hire", "offset": "0x13F65F", "size": 1, "type": "u8",
                 "description": "Available for hire counter (decreases when units are hired)"},
                {"name": "town_name", "offset": "0x13F648", "size": 8, "type": "cp1251",
                 "description": "Town name (cp1251 encoded, ~8 bytes)"},
                {"name": "town_owner_color", "offset": "0x13F78C", "size": 1, "type": "u8_enum",
                 "values": "PLAYER_COLORS",
                 "description": "Town owner color (0-7=player, 0xFF=neutral). Changes when town is captured."},
                {"name": "town_garrison_army_counts_5", "offset": "0x13F795", "size": 20, "type": "u32_le_array",
                 "array_count": 5,
                 "description": "Town garrison army_counts[5] (5 × u32 LE). Confirmed: towns have 5 slots, not 7!"},
                {"name": "town_garrison_army_types_5", "offset": "0x13F7B1", "size": 20, "type": "u32_le_array",
                 "array_count": 5,
                 "description": "Town garrison army_types[5] (5 × u32 LE creature IDs, 0=empty)"}
            ]
        },

        {
            "name": "town_visiting_hero",
            "description": "Visiting hero coordinates for town",
            "fields": [
                {"name": "visiting_hero_coords", "offset": "0x143E6E", "size": 8, "type": "u32_le_pair",
                 "description": "Visiting hero coordinates (X, Y) as 2 × u32 LE. (0xFFFFFFFF, 0xFFFFFFFF) = no visitor."},
                {"name": "visiting_hero_flag", "offset": "0x16368B", "size": 1, "type": "u8_bool",
                 "description": "Visiting hero flag (true if hero is visiting town)"},
                {"name": "hero_slot_in_town", "offset": "0x1636AE", "size": 1, "type": "u8",
                 "description": "Hero slot in town (2=guest, 7=inside garrison)"}
            ]
        },

        {
            "name": "hero_main_block",
            "description": "HERO #1 (main active hero) block — full stats around 0x157EC3..0x164000",
            "fields": [
                {"name": "hero_xy_current", "offset": "0x157EC3", "size": 10, "type": "utf16_le_pair",
                 "description": "Hero current X,Y coordinates as UTF-16 LE single chars (e.g. '4','a' = (4,10))"},
                {"name": "hero_stat_counter_1", "offset": "0x157EE0", "size": 1, "type": "u8",
                 "description": "Hero stat counter 1 (increases on actions)"},
                {"name": "hero_stat_counter_2", "offset": "0x157EEC", "size": 1, "type": "u8",
                 "description": "Hero stat counter 2"},
                {"name": "movement_bonus_values", "offset": "0x157EF0", "size": 8, "type": "u32_le_pair",
                 "description": "Movement bonus values (2 × u32 LE). E.g. (51, 99) — possibly Stables bonus etc."},
                {"name": "current_movement_points", "offset": "0x157F00", "size": 4, "type": "u32_le",
                 "description": "Current movement points (u32 LE). Cheat code sets to 299999. Normal ~500-2000. Cost: 100/step horizontal/vertical, 141/step diagonal, modified by terrain/roads/artifacts/class."},
                {"name": "experience", "offset": "0x157F04", "size": 4, "type": "u32_le",
                 "description": "Hero experience (u32 LE). +1000 from Learning Stone etc."},
                {"name": "level", "offset": "0x157F08", "size": 1, "type": "u8",
                 "description": "Hero level (u8)"},
                {"name": "skill_count", "offset": "0x157F0E", "size": 4, "type": "u32_le",
                 "description": "Number of secondary skills known"},
                {"name": "new_skill_data", "offset": "0x157F12", "size": 2, "type": "u16_le",
                 "description": "New skill data (skill_id + level encoded)"},
                {"name": "visited_objects_low", "offset": "0x157F4D", "size": 1, "type": "u8_bitmask",
                 "bits": {"4": "Fountain of Fortune"},
                 "description": "Visited bonus-objects bitmask (low byte, bits 0-7). Bit 4 = Fountain of Fortune. NOTE: Not all visited objects set bits — only those with boolean weekly modifiers (luck/morale/etc)."},
                {"name": "visited_objects_high", "offset": "0x157F4E", "size": 1, "type": "u8_bitmask",
                 "bits": {"0": "Upgrade Fort"},
                 "description": "Visited bonus-objects bitmask (high byte, bits 8-15). Bit 0 (i.e. bit 8 overall) = Upgrade Fort."},
                {"name": "skill_slot_flag", "offset": "0x157FA4", "size": 1, "type": "u8",
                 "description": "Skill slot flag"},
                {"name": "skill_level", "offset": "0x157FC0", "size": 1, "type": "u8",
                 "description": "Skill level byte"},
                {"name": "skill_counter", "offset": "0x157FCD", "size": 1, "type": "u8",
                 "description": "Skill counter"},
                {"name": "queued_path_destination", "offset": "0x1636B2", "size": 8, "type": "u32_le_pair",
                 "description": "Queued path destination (X, Y) as 2 × u32 LE. (0xFFFFFFFF, 0xFFFFFFFF) = no path queued."},
                {"name": "hero_army_types", "offset": "0x163718", "size": 28, "type": "u32_le_array",
                 "array_count": 7,
                 "description": "Hero army creature IDs (7 × u32 LE, 0xFFFFFFFF=empty slot)"},
                {"name": "hero_army_counts", "offset": "0x163734", "size": 28, "type": "u32_le_array",
                 "array_count": 7,
                 "description": "Hero army creature counts (7 × u32 LE)"},
                {"name": "has_spell_book", "offset": "0x163794", "size": 1, "type": "u8_bool",
                 "description": "Hero has spell book flag (bool)"},
                {"name": "spell_bit_array", "offset": "0x1637A0", "size": 146, "type": "bit_array",
                 "description": "Spell bit array (~146 bytes). Each bit = 1 spell known. Structure: 5 sections × 9 bytes (one per spell level)."},
                {"name": "ballista_slot", "offset": "0x163885", "size": 4, "type": "u32_le",
                 "description": "Ballista slot (u32 LE artifact ID, 0xFFFFFFFF=empty). ID=4 = Ballista."},
                {"name": "ammo_cart_slot", "offset": "0x163889", "size": 4, "type": "u32_le",
                 "description": "Ammo Cart slot (u32 LE artifact ID)"},
                {"name": "first_aid_tent_slot", "offset": "0x16388D", "size": 4, "type": "u32_le",
                 "description": "First Aid Tent slot (u32 LE artifact ID)"},
                {"name": "catapult_slot", "offset": "0x163891", "size": 4, "type": "u32_le",
                 "description": "Catapult slot (u32 LE artifact ID)"},
                {"name": "spell_book_slot", "offset": "0x1638A5", "size": 4, "type": "u32_le",
                 "description": "Spell Book artifact slot (u32 LE artifact ID)"}
            ]
        },

        {
            "name": "hero_main_movement_alt",
            "description": "Hero #1 movement bytes (alt block, smaller mirror)",
            "fields": [
                {"name": "movement_remaining", "offset": "0x6D7A4", "size": 1, "type": "u8",
                 "description": "Hero #1 movement remaining (low byte)"},
                {"name": "movement_total", "offset": "0x6D7AA", "size": 1, "type": "u8",
                 "description": "Hero #1 movement total/other"}
            ]
        },

        {
            "name": "hero_2_block",
            "description": "Hero #2 alt block (stats + visit counter)",
            "fields": [
                {"name": "movement_remaining", "offset": "0x6B716", "size": 1, "type": "u8",
                 "description": "Hero #2 movement remaining (low byte)"},
                {"name": "visit_counter_short", "offset": "0x6B71C", "size": 4, "type": "u32_le",
                 "description": "Hero #2 short-term visit counter (u32 LE, 0xFFFFFFFF=not visited)"},
                {"name": "movement_byte_3", "offset": "0x6B77A", "size": 1, "type": "u8",
                 "description": "Hero #2 movement byte 3"},
                {"name": "visit_flag", "offset": "0x6B780", "size": 4, "type": "u32_le",
                 "description": "Hero #2 visit flag (encoded). 0xFFFE403F=not visited, 0x00000058 (88)=visited (event_id)."}
            ]
        },

        {
            "name": "hero_3_block",
            "description": "Hero #3 alt block",
            "fields": [
                {"name": "movement_byte", "offset": "0x6F998", "size": 1, "type": "u8",
                 "description": "Hero #3 movement byte"},
                {"name": "visit_counter", "offset": "0x6F99E", "size": 4, "type": "u32_le",
                 "description": "Hero #3 visit counter (u32 LE, 0xFFFFFFFF=not visited, 88=event_id)"}
            ]
        },

        {
            "name": "hero_4_block",
            "description": "Hero #4 alt block (one of multiple scattered blocks)",
            "fields": [
                {"name": "movement_byte", "offset": "0x5AAC1", "size": 1, "type": "u8",
                 "description": "Hero #4 movement byte. NOTE: Hero blocks have variable stride across the file."}
            ]
        },

        {
            "name": "hero_5_block",
            "description": "Hero #5 (defender/in boat) block",
            "fields": [
                {"name": "movement_byte", "offset": "0x5B92D", "size": 1, "type": "u8",
                 "description": "Hero #5 movement byte (defender position)"},
                {"name": "hero_xy", "offset": "0x141385", "size": 10, "type": "utf16_le_pair",
                 "description": "Hero #5 X,Y coordinates (UTF-16 LE, in boat)"},
                {"name": "movement_counter", "offset": "0x14138AE", "size": 1, "type": "u8",
                 "description": "Hero #5 movement counter"},
                {"name": "movement", "offset": "0x14138C2", "size": 2, "type": "u16_le",
                 "description": "Hero #5 movement (u16 LE)"}
            ]
        },

        {
            "name": "hero_alt_block_general",
            "description": "Hero alt block fields (multiple heroes share these offsets in different blocks)",
            "fields": [
                {"name": "hero_on_boat_flag", "offset": "0x59BE4", "size": 1, "type": "u8_enum",
                 "values": {"0x22": "on boat", "0x08": "on land"},
                 "description": "Hero on boat flag (0x22=on boat/water, 0x08=on land)"},
                {"name": "hero_surface_flag", "offset": "0x59BEA", "size": 1, "type": "u8_enum",
                 "values": {"0x01": "water surface", "0x06": "land surface"},
                 "description": "Hero surface flag (0x01=water, 0x06=land)"},
                {"name": "visiting_coords_current", "offset": "0x140AF9", "size": 12, "type": "utf16_le_pair",
                 "description": "Visiting coords (current, UTF-16 LE)"},
                {"name": "hero_state_byte", "offset": "0x140B22", "size": 1, "type": "u8",
                 "description": "Hero state byte (alt)"},
                {"name": "map_object_visiting_coords", "offset": "0x140B26", "size": 8, "type": "u32_le_pair",
                 "description": "Map object visiting coords (2 × u32 LE). Set when hero visits fort/shrine/etc."},
                {"name": "hero_level_counter", "offset": "0x140B30", "size": 4, "type": "u32_le",
                 "description": "Hero level counter (alt block)"},
                {"name": "movement_points_alt_1", "offset": "0x140B34", "size": 4, "type": "u32_le",
                 "description": "Movement points (alt block 1, u32 LE)"},
                {"name": "movement_points_alt_2", "offset": "0x140B36", "size": 4, "type": "u32_le",
                 "description": "Movement points (alt block 2, u32 LE)"},
                {"name": "movement_total_alt", "offset": "0x140B3C", "size": 4, "type": "u32_le",
                 "description": "Movement total (alt block)"},
                {"name": "hero_level_alt", "offset": "0x140B44", "size": 1, "type": "u8",
                 "description": "Hero level (alt block). +2 after winning a battle."},
                {"name": "hero_on_boat_flag_2", "offset": "0x140B82", "size": 1, "type": "u8",
                 "description": "Hero on boat flag 2"},
                {"name": "hero_coords_alt", "offset": "0x140B84", "size": 8, "type": "u32_le_pair",
                 "description": "Hero coords (alt block, 2 × u32 LE)"},
                {"name": "hero_artifact_slot_defender", "offset": "0x140B9C", "size": 4, "type": "u32_le",
                 "description": "Hero artifact slot (defender side). Cleared after defeat."},
                {"name": "army_count_slot_alt", "offset": "0x140BA0", "size": 5, "type": "bytes",
                 "description": "Army count slot (alt block, u32 count + u8 slot_index)"},
                {"name": "skill_count_alt", "offset": "0x140BD9", "size": 1, "type": "u8",
                 "description": "Skill count (alt block). +1 when learning new skill."},
                {"name": "skill_improvement_flag", "offset": "0x140BE7", "size": 1, "type": "u8",
                 "description": "Skill improvement flag"},
                {"name": "skill_level_alt", "offset": "0x140C01", "size": 2, "type": "u8_pair",
                 "description": "Skill level (alt, 2 × u8). +1 when improving existing skill."}
            ]
        },

        {
            "name": "global_hero_counters",
            "description": "Global hero turn counters (incremented per hero turn)",
            "fields": [
                {"name": "turn_counter_1", "offset": "0x143E41", "size": 1, "type": "u8",
                 "description": "Global hero turn counter 1 (+1 per turn)"},
                {"name": "turn_counter_2", "offset": "0x143E48", "size": 1, "type": "u8",
                 "description": "Global hero turn counter 2 (+1 per turn)"},
                {"name": "visiting_coords_reset", "offset": "0x143E6E", "size": 8, "type": "u32_le_pair",
                 "description": "Visiting coords (reset to -1, -1 when leaving)"},
                {"name": "hero_movement_alt", "offset": "0x143E7E", "size": 2, "type": "u16_le",
                 "description": "Hero movement (alt, u16 LE)"}
            ]
        },

        {
            "name": "enemy_hero_defeated_block",
            "description": "Enemy hero block (large cleared block on defeat)",
            "fields": [
                {"name": "defeated_flag", "offset": "0x151C7F", "size": 1, "type": "u8_bool",
                 "description": "Enemy hero defeated flag (1=active, 0=defeated)"},
                {"name": "army_slots_cleared", "offset": "0x151C93", "size": 4, "type": "bytes",
                 "description": "Enemy army slots (cleared on defeat)"},
                {"name": "visiting_coords", "offset": "0x151CA6", "size": 8, "type": "u32_le_pair",
                 "description": "Enemy visiting coords (reset to -1, -1 on defeat)"},
                {"name": "hero_state", "offset": "0x151CC2", "size": 1, "type": "u8_enum",
                 "values": {"7": "active", "2": "banished"},
                 "description": "Enemy hero state (7=active, 2=banished)"},
                {"name": "big_block_cleared", "offset": "0x151D02", "size": 43, "type": "bytes",
                 "description": "Enemy hero big block (43 bytes, fully cleared on defeat)"}
            ]
        },

        {
            "name": "artifacts_transfer",
            "description": "Artifact slots for transfer (winner/loser)",
            "fields": [
                {"name": "artifact_slot_winner", "offset": "0x16A19C", "size": 1, "type": "u8",
                 "description": "Artifact slot (winner side) — receives artifact after victory"},
                {"name": "artifact_slot_loser", "offset": "0x16A1B4", "size": 1, "type": "u8",
                 "description": "Artifact slot (loser side) — cleared after defeat"}
            ]
        },

        {
            "name": "black_market",
            "description": "Black Market artifact slot",
            "fields": [
                {"name": "black_market_artifact", "offset": "0x16A2D4", "size": 4, "type": "u32_le",
                 "description": "Black Market artifact slot (u32 LE artifact ID, 0xFFFFFFFF=empty)"}
            ]
        },

        {
            "name": "cheater_flag",
            "description": "Cheater flag (set when any cheat code is used)",
            "fields": [
                {"name": "cheater_flag", "offset": "0x16A29D", "size": 1, "type": "u8_bool",
                 "description": "Cheater flag (0=normal, 1=cheated). Set when ANY cheat code is used."}
            ]
        },

        {
            "name": "path_block",
            "description": "Path block — replay log of all hero movements",
            "fields": [
                {"name": "base64_metadata", "offset": "0x130B71", "size": 190, "type": "ascii",
                 "description": "Base64 metadata block (changes on every save, ~190 bytes)"},
                {"name": "map_event_counter", "offset": "0x17EC37", "size": 1, "type": "u8",
                 "description": "Map event counter (increments by number of path-records added)"},
                {"name": "path_block_main", "offset": "0x1814D0", "size": 256, "type": "path_records",
                 "description": "Path block (history of movements, ~256 bytes base, expands with new records)"},
                {"name": "path_block_extension", "offset": "0x18179D", "size": -1, "type": "path_records",
                 "description": "Path block extension (variable size, fills as new path-records are added)"}
            ],
            "path_record_types": {
                "01 03": {
                    "name": "hero_movement",
                    "size": 15,
                    "description": "Hero movement (one step)",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x01"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le",
                         "description": "visit_id (per-hero counter)"},
                        {"name": "step_n", "offset": 6, "size": 1, "type": "u8",
                         "description": "Step number in current multi-step move"},
                        {"name": "from_x", "offset": 7, "size": 2, "type": "u16_le"},
                        {"name": "from_y", "offset": 9, "size": 2, "type": "u16_le"},
                        {"name": "to_x", "offset": 11, "size": 2, "type": "u16_le"},
                        {"name": "to_y", "offset": 13, "size": 2, "type": "u16_le"}
                    ]
                },
                "03 03": {
                    "name": "capture_event",
                    "size": 9,
                    "description": "Object capture event (e.g. mine captured)",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le",
                         "description": "visit_id of capture"},
                        {"name": "prev_owner", "offset": 6, "size": 1, "type": "u8",
                         "description": "Previous owner color (4=Orange etc.)"},
                        {"name": "object_type", "offset": 7, "size": 1, "type": "u8"},
                        {"name": "terminator", "offset": 8, "size": 1, "type": "u8", "value": "0x0b"}
                    ]
                },
                "04 03": {
                    "name": "battle_event_town",
                    "size": 10,
                    "description": "Battle event (e.g. attacking enemy town)",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x04"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le",
                         "description": "visit_id of battle"},
                        {"name": "data", "offset": 6, "size": 4, "type": "bytes",
                         "description": "Battle info (result, losses, etc.)"}
                    ]
                },
                "06 03": {
                    "name": "disembark_subrecord",
                    "size": "variable",
                    "description": "Disembark event subrecord (nested inside 08 03)",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x06"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"}
                    ]
                },
                "08 03": {
                    "name": "battle_outcome_or_army_transfer",
                    "size": "9 or 27",
                    "description": "Battle outcome (9b compact) OR Army transfer / Disembark (27b with subrecords)",
                    "fields_compact_9b": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x08"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le",
                         "description": "visit_id of battle"},
                        {"name": "result", "offset": 6, "size": 1, "type": "u8",
                         "description": "Encoded result (0xff seen)"},
                        {"name": "outcome", "offset": 7, "size": 1, "type": "u8_enum",
                         "values": {"1": "victory"}, "description": "Battle outcome (1=victory)"},
                        {"name": "terminator", "offset": 8, "size": 1, "type": "u8", "value": "0x0b"}
                    ],
                    "fields_full_27b": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x08"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le"},
                        {"name": "n1", "offset": 6, "size": 1, "type": "u8"},
                        {"name": "n2", "offset": 7, "size": 1, "type": "u8"},
                        {"name": "subrecord", "offset": 8, "size": 19, "type": "subrecord",
                         "description": "Nested subrecord (e.g. 06 03 for disembark)"}
                    ]
                },
                "09 03": {
                    "name": "visit_event",
                    "size": 19,
                    "description": "Visit event (visiting map object)",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x09"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "counter", "offset": 2, "size": 4, "type": "u32_le",
                         "description": "visit_id"},
                        {"name": "n", "offset": 6, "size": 1, "type": "u8"},
                        {"name": "from_x", "offset": 7, "size": 2, "type": "u16_le"},
                        {"name": "from_y", "offset": 9, "size": 2, "type": "u16_le"},
                        {"name": "to_x", "offset": 11, "size": 2, "type": "u16_le"},
                        {"name": "to_y", "offset": 13, "size": 2, "type": "u16_le"},
                        {"name": "tail", "offset": 15, "size": 4, "type": "bytes"}
                    ]
                },
                "0b 03": {
                    "name": "fog_of_war_update",
                    "size": "variable",
                    "description": "Fog of war update — newly visible tiles",
                    "fields": [
                        {"name": "flag1", "offset": 0, "size": 1, "type": "u8", "value": "0x0b"},
                        {"name": "flag2", "offset": 1, "size": 1, "type": "u8", "value": "0x03"},
                        {"name": "n", "offset": 2, "size": 1, "type": "u8",
                         "description": "Number of coordinate pairs"},
                        {"name": "pad", "offset": 3, "size": 1, "type": "u8", "value": "0x00"},
                        {"name": "pairs", "offset": 4, "size": "n*8", "type": "u16_le_pair_array",
                         "description": "Array of N (X, Y) coordinate pairs (4 bytes each)"}
                    ]
                }
            }
        },

        {
            "name": "footer",
            "description": "File footer (last 18 bytes)",
            "fields": [
                {"name": "footer_magic", "offset": -18, "size": 4, "type": "ascii",
                 "value": "HD3\\0",
                 "description": "Footer magic (always 'HD3\\0')"},
                {"name": "footer_size_or_crc", "offset": -14, "size": 3, "type": "bytes",
                 "description": "Footer size or CRC (3 bytes, value changes with file size)"},
                {"name": "footer_padding", "offset": -11, "size": 11, "type": "bytes",
                 "description": "Footer padding (always zeros)"}
            ]
        }
    ],

    "search_patterns": {
        "hero_blocks": {
            "description": "Hero blocks have variable stride, use these patterns to find them",
            "patterns": [
                {"name": "hero_coords_pattern", "regex": "[\\x20-\\x7e]\\x00[\\x20-\\x7e]\\x00\\x00\\x00\\x01",
                 "description": "Hero X,Y coords in UTF-16 LE followed by u32 LE counter=1"},
                {"name": "hero_visit_id_pattern", "regex": "\\x10\\x22|\\x12\\x22|\\x09\\x22",
                 "description": "Common visit_id values (0x2210, 0x2212, 0x2209) appearing in hero blocks"}
            ]
        },
        "path_records": {
            "description": "Path records start with flag1 flag2 bytes",
            "patterns": [
                {"type": "01 03", "size": 15, "name": "hero_movement"},
                {"type": "03 03", "size": 9, "name": "capture_event"},
                {"type": "04 03", "size": 10, "name": "battle_event_town"},
                {"type": "08 03", "size": "9 or 27", "name": "battle_outcome_or_army_transfer"},
                {"type": "09 03", "size": 19, "name": "visit_event"},
                {"type": "0b 03", "size": "variable", "name": "fog_of_war_update"}
            ]
        }
    },

    "known_unknowns": [
        "Map terrain (tile data) — not yet localized",
        "Map object positions and states (except owner color at 0x13C850)",
        "Fog of war bit mask (separate from path-records)",
        "AI player state and decision memory",
        "Current day / week / month counter",
        "Current player turn",
        "Diplomacy / alliances state",
        "Random seed / RNG state",
        "Quest log state (Seer Huts, Border Guards)",
        "Hero biography (variable-length, precedes hero stats in hero block)",
        "Town buildings list (only 'last built' and 'built flag' known)",
        "Spell availability in Magic Guild",
        "Hero primary stats (attack/defense/power/knowledge) — partially known from h3sed but not verified in our diffs"
    ]
}

# Сохраняем
with open('/home/z/my-project/download/gm1_mapping.json', 'w', encoding='utf-8') as f:
    json.dump(MAPPING, f, indent=2, ensure_ascii=False)

print("✅ Mapping saved with hex-string offsets")
print(f"   Total blocks: {len(MAPPING['blocks'])}")
print(f"   Total fields: {sum(len(b.get('fields', [])) for b in MAPPING['blocks'])}")
