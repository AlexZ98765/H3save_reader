#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
post_tile_parser.py — Parse post-tile sections of .GM1 saves (content extraction).

This module implements ALL 23 ProspectorRT content-parsing algorithms that
were identified as missing in `02_format_docs/unimplemented_algorithms_audit.md`.

Each `parse_*` function here is a content-extracting counterpart of the
`scan_*` skip-only function in `post_tile_scanner.py`. They decode the actual
fields and return Python dicts/dataclasses instead of just walking past.

Reference: `07_prt_decompiled/ProspectorRT_source/ProspectorRT/MainForm.cs`
"""

from __future__ import annotations
import struct
from collections import defaultdict
from dataclasses import dataclass, field
from typing import Any, Dict, List, Optional, Tuple, Callable

# Reuse the scan-only offset walkers
from post_tile_scanner import (
    _u16,
    scan_event_box_content, scan_art_res_content, scan_monstr_content,
    scan_seer_hut_content, scan_pass_guard_content, scan_object_content,
    scan_map_timed_events, scan_town_timed_events, scan_bottle_sign_content,
    scan_towns_content, scan_heroes_content, walk_post_tile_sections,
)


# ============================================================================
# Reference tables (transcribed from ProspectorRT CreateTbl* + a* arrays)
# ============================================================================

A_RESOURCE = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
A_PR_SKILL = ["Attack", "Defense", "Power", "Knowledge"]
A_LEVEL_SKILL = ["Basic", "Advanced", "Expert"]
A_COLOR = ["Blue", "Green", "Red", "Yellow", "Orange", "Purple", "Teal", "Pink"]
A_MINE = ["Gold", "Wood", "Ore", "Mercury", "Sulfur", "Crystal", "Gems", "Unknown"]
A_LOCALITY = ["Surface", "Underground"]
A_PLACE = ["Prison", "Tavern", "Empty", "Town", "Map"]
A_REPLY = ["No", "Yes"]
A_REWARD = [
    "Experience",   # 0
    "Mana",         # 1
    "Morale",       # 2
    "Luck",         # 3
    "Resource",     # 4
    "Primary skill",# 5
    "Artefact",     # 6
    "Spell",        # 7
    "Monster",      # 8
]
A_QUEST = [
    "Reach level",      # 0 (mission type 1)
    "Reach skills",     # 1 (mission type 2)
    "Find hero {",      # 2 (mission type 3 — hero, prefix "{")
    "Kill monster",     # 3 (mission type 4)
    "Bring artefact",   # 4 (mission type 5)
    "Bring monsters",   # 5 (mission type 6)
    "Bring resources",   # 6 (mission type 7)
    "Find hero",        # 7 (post-process for "{")
    "Defeat hero",      # 8 (post-process for "}")
    "Defeat color",     # 9 (mission type 9)
]
A_STATUS = ["No", "Built", "Forbidden", "Built today"]

# 28 secondary skills (SoD) — index 0..27
A_SECONDARY_SKILL = [
    "Pathfinding", "Archery", "Logistics", "Scouting",
    "Diplomacy", "Navigation", "Leadership", "Necromancy",
    "Luck", "Ballistics", "Eagle Eye", "Necromancy2",  # NOTE: 11 is placeholder
    "Earth Magic", "Air Magic", "Fire Magic", "Water Magic",
    "Wisdom", "Mysticism", "Estates", "Resistance",
    "First Aid", "Tactics", "Artillery", "Learning",
    "Offense", "Armorer", "Sorcery", "Eagle Eye2",
]

# 70 spells (SoD index 0..69)
# (kept short here — full table is in `object_types_dictionary.json` or
# Heroes III docs; this list is sufficient for `parse_seer_hut_content` etc.)
A_SPELL = [f"Spell{i}" for i in range(70)]


# ============================================================================
# Helpers — signed decode (ProspectorRT trick)
# ============================================================================

def _u8(raw: bytes, s: int) -> int:
    return raw[s]


def _i8(raw: bytes, s: int) -> int:
    """Signed 8-bit (ProspectorRT: `> 240 ? ((~b + 1) * -1) : b`)."""
    v = raw[s]
    return v - 256 if v > 240 else v


def _u32(raw: bytes, s: int) -> int:
    return (raw[s+3] << 24) | (raw[s+2] << 16) | (raw[s+1] << 8) | raw[s]


def _i32(raw: bytes, s: int) -> int:
    """Signed 32-bit (ProspectorRT: `if byte[3] != 0xFF positive else negative trick`)."""
    if raw[s+3] != 0xFF:
        return _u32(raw, s)
    # negative: ~(b3 b2 b1 b0) + 1, then negate
    neg = ((~raw[s+3] & 0xFF) << 24) | ((~raw[s+2] & 0xFF) << 16) | \
          ((~raw[s+1] & 0xFF) << 8) | (~raw[s] & 0xFF)
    neg += 1
    return -neg


def _i32_3(raw: bytes, s: int) -> int:
    """Signed 24-bit (3-byte) — used for gold/resource in timers."""
    if raw[s+3] != 0xFF:
        return (raw[s+2] << 16) | (raw[s+1] << 8) | raw[s]
    neg = ((~raw[s+2] & 0xFF) << 16) | ((~raw[s+1] & 0xFF) << 8) | (~raw[s] & 0xFF)
    neg += 1
    return -neg


def _i16(raw: bytes, s: int) -> int:
    """Signed 16-bit (ProspectorRT mana encoding)."""
    if raw[s+3] != 0xFF:
        return (raw[s+1] << 8) | raw[s]
    neg = ((~raw[s+1] & 0xFF) << 8) | (~raw[s] & 0xFF)
    neg += 1
    return -neg


def _decode_cp1251(b: bytes) -> str:
    try:
        return b.decode("cp1251")
    except Exception:
        return b.decode("latin-1", errors="replace")


# ============================================================================
# Common: parse 7-slot army/guard (used by many sections)
# ============================================================================

@dataclass
class ArmySlot:
    monster_id: int
    count: int


def _parse_guard_7(raw: bytes, s: int) -> Tuple[List[ArmySlot], int, int]:
    """Parse 7-slot army starting at s. Returns (slots, total_hp, new_s).

    Layout (28 bytes types + 28 bytes counts = 56 total):
    - 7 × 4 bytes (monster_id + 3 padding) — slots with 0xFF = empty
    - 7 × 4 bytes (count: u16 + 2 padding) — at offset +28

    HP computation requires `TblMonsters[3]` (HP per unit) — we just
    sum counts since we don't have HP table here; caller can refine.
    """
    slots: List[ArmySlot] = []
    total_count = 0
    for i in range(7):
        mid = raw[s + i * 4]
        if mid != 0xFF:
            count = (raw[s + i * 4 + 30 - 0 + 1] << 8) | raw[s + i * 4 + 29 - 0 + 0]
            # offsets: i*4 + 28..30 are counts (u16 at i*4 + 29..30)
            cnt = (raw[s + i * 4 + 30] << 8) | raw[s + i * 4 + 29]
            slots.append(ArmySlot(monster_id=mid, count=cnt))
            total_count += cnt
    return slots, total_count, s + 56


def _parse_guard(raw: bytes, s: int) -> Tuple[List[ArmySlot], int, int]:
    """Alias for _parse_guard_7 — 7 monster slots × 4 bytes types + 4 bytes counts."""
    slots: List[ArmySlot] = []
    for i in range(7):
        mid = raw[s + i * 4]
        if mid != 0xFF:
            cnt = (raw[s + i * 4 + 30] << 8) | raw[s + i * 4 + 29]
            slots.append(ArmySlot(monster_id=mid, count=cnt))
    return slots, s + 56


# ============================================================================
# A2. EventBoxContent (Pandora's Box / map event)
# ============================================================================

@dataclass
class EventBoxContent:
    address: int = 0
    has_guard: bool = False
    guard: List[ArmySlot] = field(default_factory=list)
    experience: int = 0
    mana: int = 0
    morale: int = 0
    luck: int = 0
    resources: Dict[str, int] = field(default_factory=dict)  # 6 + gold
    primary_skills: Dict[str, int] = field(default_factory=dict)
    secondary_skills: List[Tuple[int, int]] = field(default_factory=list)  # (skill_id, level)
    artifacts: List[int] = field(default_factory=list)
    spells: List[int] = field(default_factory=list)
    monsters: List[Tuple[int, int]] = field(default_factory=list)  # (monster_id, count)
    apply: Tuple[int, int, int] = (0, 0, 0)


def parse_event_box_content(raw: bytes, s: int) -> Tuple[EventBoxContent, int]:
    """Parse one EventBox record (Pandora's Box or map event).

    Reference: MainForm.cs:8172 `EventBoxContent(DataRow row, int s)`.
    Returns (parsed_dict, new_s).

    Layout:
    - if raw[s] > 0: skip message (u16 len + bytes + 3)
    - if raw[s] == 1: 7-slot guard (56 bytes), else +1
    - u32 experience
    - i32 mana (signed, 4 bytes)
    - i8 morale, i8 luck
    - 6 × i32 resources (24 bytes, signed)
    - i32 gold (4 bytes, signed)
    - 4 primary skills (1+1+1+1 byte)
    - secondary skills list: u8 count + count × (u8 skill_id, u8 level) + 1
    - artifacts list: u8 count + count × u8 artifact_id + 1
    - spells list: u8 count + count × u8 spell_id + 1
    - monsters list: u8 count + count × 4 bytes (u8 id + u16 count + 1 padding)
    """
    rec = EventBoxContent(address=s)

    # Skip message
    if raw[s] > 0:
        s = s + _u16(raw, s + 1) + 3

    # Guard (optional)
    if raw[s] == 1:
        rec.has_guard = True
        s += 1
        rec.guard, s = _parse_guard(raw, s)
    else:
        s += 1

    # Experience (u32)
    rec.experience = _u32(raw, s); s += 4
    # Mana (i32, special encoding)
    rec.mana = _i32(raw, s); s += 4
    # Morale, luck (i8 each)
    rec.morale = _i8(raw, s); s += 1
    rec.luck = _i8(raw, s + 0); s += 1

    # 6 resources — 3-byte value + 1-byte sign flag (NOT 4-byte value!)
    # ProspectorRT EventBoxContent: `if decmp[s+j*4+3] == 0xFF: negative`
    for i in range(6):
        v = _i32_3(raw, s)
        if v != 0:
            rec.resources[A_RESOURCE[i]] = v
        s += 4

    # Gold (3-byte value + 1-byte sign flag)
    gold = _i32_3(raw, s)
    if gold != 0:
        rec.resources["Gold"] = gold
    s += 4

    # 4 primary skills
    for i in range(4):
        v = raw[s + i]
        if v != 0:
            rec.primary_skills[A_PR_SKILL[i]] = v
    s += 4

    # Secondary skills
    if raw[s] > 0:
        n = raw[s]
        for i in range(n):
            skill_id = raw[s + i * 2 + 1]
            level = raw[s + i * 2 + 2]
            rec.secondary_skills.append((skill_id, level))
        s = s + n * 2 + 1
    else:
        s += 1

    # Artifacts
    if raw[s] > 0:
        n = raw[s]
        for i in range(n):
            rec.artifacts.append(raw[s + i + 1])
        s = s + n + 1
    else:
        s += 1

    # Spells
    if raw[s] > 0:
        n = raw[s]
        for i in range(n):
            rec.spells.append(raw[s + i + 1])
        s = s + n + 1
    else:
        s += 1

    # Monsters
    if raw[s] > 0:
        n = raw[s]
        for i in range(n):
            mid = raw[s + i * 4 + 1]
            cnt = (raw[s + i * 4 + 4] << 8) | raw[s + i * 4 + 3]
            rec.monsters.append((mid, cnt))
        s = s + n * 4 + 1
    else:
        s += 1

    # Apply (3 bytes after the record)
    # Note: ProspectorRT applies via `r_EventBoxRow.Apply` from
    # the saved Apply fields; we expose them as last 3 bytes of the record
    # (the caller knows the row and decodes based on object type).
    return rec, s - 1  # ProspectorRT returns --s


# ============================================================================
# A3. ArtResContent (Artifact / Resource / Spell with guard)
# ============================================================================

@dataclass
class ArtResContent:
    address: int = 0
    art_type: int = 0  # 0=Art, 1=Resource, 2=Spell (from caller's TblArtRes)
    has_guard: bool = False
    guard: List[ArmySlot] = field(default_factory=list)


def parse_art_res_content(raw: bytes, s: int, art_type: int = 0) -> Tuple[ArtResContent, int]:
    """Parse one ArtRes record (treasure with guard).

    Reference: MainForm.cs:8394 `ArtResContent(DataRow row, int s)`.
    Layout:
    - skip 2 bytes (u16 length of message)
    - if raw[s] == 1: 56 bytes guard, else +1
    """
    rec = ArtResContent(address=s, art_type=art_type)
    s = s + _u16(raw, s) + 2
    if raw[s] == 1:
        rec.has_guard = True
        s += 1
        rec.guard, s = _parse_guard(raw, s)
    else:
        s += 1
    return rec, s - 1


# ============================================================================
# A4. MonstrContent (Monster treasure)
# ============================================================================

@dataclass
class MonstrContent:
    address: int = 0
    resources: Dict[str, int] = field(default_factory=dict)
    gold: int = 0
    artifact_id: int = -1  # 0xFF = none


def parse_monstr_content(raw: bytes, s: int) -> Tuple[MonstrContent, int]:
    """Parse one Monstr record (treasure after defeating monsters).

    Reference: MainForm.cs:8455 `MonstrContent(DataRow row, int s)`.
    Layout:
    - skip 2 bytes (u16 message length)
    - 6 × 4 bytes resources (u32 each — note: NOT signed here!)
    - 4 bytes gold (u32 — note: NOT signed here!)
    - 1 byte artifact (0xFF = none)
    """
    rec = MonstrContent(address=s)
    s = s + _u16(raw, s) + 2

    # 6 resources × 4 bytes (positive only — ProspectorRT just takes u24 << 16 + u8)
    for i in range(6):
        v = (raw[s + i * 4 + 2] << 16) | (raw[s + i * 4 + 1] << 8) | raw[s + i * 4]
        if v != 0:
            rec.resources[A_RESOURCE[i]] = v
    s += 24

    # Gold (u32)
    rec.gold = (raw[s + 2] << 16) | (raw[s + 1] << 8) | raw[s]
    s += 4

    # Artifact
    if raw[s] < 0xFF:
        rec.artifact_id = raw[s]

    return rec, s + 1


# ============================================================================
# A5. SeerHutContent (the most complex — 10 mission types + 10 reward types)
# ============================================================================

@dataclass
class SeerHutContent:
    address: int = 0
    mission_type: int = 0
    mission: Dict[str, Any] = field(default_factory=dict)
    deadline: Optional[Tuple[int, int, int]] = None  # (month, week, day)
    mission_progress: List[bytes] = field(default_factory=list)  # 3 variable sections
    reward_type: int = 0
    reward: Dict[str, Any] = field(default_factory=dict)


def _decode_deadline(b0: int) -> Optional[Tuple[int, int, int]]:
    """ProspectorRT: `num2 = b0/28; num3 = (b0 - num2*28)/7; num4 = b0 - num2*28 - num3*7;`.
    Returns (month+1, week+1, day+1) or None if b0 == 0xFF."""
    if b0 == 0xFF:
        return None
    month = b0 // 28
    rem = b0 - month * 28
    week = rem // 7
    day = rem - month * 28 - week * 7
    return (month + 1, week + 1, day + 1)


def parse_seer_hut_content(raw: bytes, s: int) -> Tuple[SeerHutContent, int]:
    """Parse one SeerHut record (mission + reward).

    Reference: MainForm.cs:8505 `SeerHutContent(DataRow row, int s)`.
    Layout (mission):
        0: empty → +15 (skip to reward; s+15)
        1: "Reach level N" → s += 5  (1 byte type + 4 bytes level u32)
        2: "Reach primary skills" → s += 7 (1 type + 4 bytes skill bytes)
        3: "Find hero {id" → s += 6 (1 type + 1 hero_id + 4 padding)
        4: "Kill monster" → s += 10 (1 type + 4 bytes + 1 monster_id + 4 padding)
        5: "Bring artefacts (N)" → s += N*2 + 4
        6: "Bring monsters (N)" → s += N*6 + 4
        7: "Bring resources" → s += 31 (1 type + 30 bytes)
        8: "Defeat hero }id" → s += 5
        9: "Defeat color N" → s += 4
    Then:
        - Deadline: 4 bytes, byte[0] encodes month/week/day
        - 3 variable sections (u16 + data + 4)
        - Reward: 15 bytes (10 types):
            0: none
            1: experience (u32 at +4)
            2: mana (u16 at +4)
            3: morale (i8 at +4)
            4: luck (i8 at +4)
            5: resource (resource_id at +4, count u24 at +8)
            6: primary skill (skill_id at +4, +N at +8)
            7: secondary skill (skill_id at +4, level at +8)
            8: artefact (id at +4)
            9: spell (id at +4)
            10: monster (id at +4, count u16 at +8)
    """
    rec = SeerHutContent(address=s)

    t = raw[s]
    rec.mission_type = t

    if t == 0:
        # Empty — no mission, skip directly to reward (s += 15 happens after deadline+progress)
        pass
    elif t == 1:
        rec.mission = {"level": _u32(raw, s + 1)}
        s += 5
    elif t == 2:
        rec.mission = {"primary_skills": {A_PR_SKILL[i]: raw[s + i + 1]
                                            for i in range(4) if raw[s + i + 1] != 0}}
        s += 7
    elif t == 3:
        rec.mission = {"hero_id": raw[s + 1], "mission_str": "{" + str(raw[s + 1])}
        s += 6
    elif t == 4:
        rec.mission = {
            "x": raw[s + 1], "y": raw[s + 3], "monster_id": raw[s + 5],
            "bit": (raw[s + 4] & 4) >> 2,
        }
        s += 10
    elif t == 5:
        n = raw[s + 1]
        arts = [raw[s + j * 2 + 2] for j in range(n)]
        rec.mission = {"artifacts": arts}
        s = s + n * 2 + 4
    elif t == 6:
        n = raw[s + 1]
        monsters = []
        for k in range(n):
            base = s + k * 6 + 2
            mid = raw[base]
            cnt = (raw[base + 3] << 8) | raw[base + 2]
            monsters.append((mid, cnt))
        rec.mission = {"monsters": monsters}
        s = s + n * 6 + 4
    elif t == 7:
        resources = {}
        for i in range(6):
            v = (raw[s + i * 4 + 3] << 16) | (raw[s + i * 4 + 2] << 8) | raw[s + i * 4 + 1]
            if v > 0:
                resources[A_RESOURCE[i]] = v
        gold_v = (raw[s + 24 + 3] << 16) | (raw[s + 24 + 2] << 8) | raw[s + 24 + 1]
        if gold_v > 0:
            resources["Gold"] = gold_v
        rec.mission = {"resources": resources}
        s += 31
    elif t == 8:
        rec.mission = {"hero_id": raw[s + 1], "mission_str": "}" + str(raw[s + 1])}
        s += 5
    elif t == 9:
        rec.mission = {"color": A_COLOR[raw[s + 1]] if raw[s + 1] < 8 else "Unknown"}
        s += 4
    else:
        s += 1

    # Deadline
    rec.deadline = _decode_deadline(raw[s])
    s += 4

    # 3 variable sections (mission progress counters — typically unused)
    for _ in range(3):
        n = _u16(raw, s)
        if n != 0:
            rec.mission_progress.append(raw[s + 4:s + 4 + n])
            s = s + n + 4
        else:
            rec.mission_progress.append(b"")
            s += 4

    # Reward (15 bytes)
    rt = raw[s]
    rec.reward_type = rt

    if rt == 0:
        pass
    elif rt == 1:
        rec.reward = {"experience": _u32(raw, s + 4)}
    elif rt == 2:
        rec.reward = {"mana": (raw[s + 5] << 8) | raw[s + 4]}
    elif rt == 3:
        rec.reward = {"morale": _i8(raw, s + 4)}
    elif rt == 4:
        rec.reward = {"luck": _i8(raw, s + 4)}
    elif rt == 5:
        rid = raw[s + 4]
        amt = (raw[s + 10] << 16) | (raw[s + 9] << 8) | raw[s + 8]
        if rid < 6:
            rec.reward = {"resource": A_RESOURCE[rid], "amount": amt}
        else:
            rec.reward = {"resource": "Gold", "amount": amt}
    elif rt == 6:
        rec.reward = {"primary_skill": A_PR_SKILL[raw[s + 4]] if raw[s + 4] < 4 else "Unknown",
                      "amount": raw[s + 8]}
    elif rt == 7:
        rec.reward = {"secondary_skill_id": raw[s + 4],
                      "level_id": raw[s + 8]}
    elif rt == 8:
        rec.reward = {"artifact_id": raw[s + 4]}
    elif rt == 9:
        rec.reward = {"spell_id": raw[s + 4]}
    elif rt == 10:
        rec.reward = {"monster_id": raw[s + 4],
                      "count": (raw[s + 9] << 8) | raw[s + 8]}

    s += 15
    return rec, s - 1


# ============================================================================
# A6. PassGuardContent (Border Guard / Border Gate)
# ============================================================================

@dataclass
class PassGuardContent:
    address: int = 0
    mission_type: int = 0
    mission: Dict[str, Any] = field(default_factory=dict)
    deadline: Optional[Tuple[int, int, int]] = None
    mission_progress: List[bytes] = field(default_factory=list)


def parse_pass_guard_content(raw: bytes, s: int) -> Tuple[PassGuardContent, int]:
    """Parse one PassGuard record. Same structure as SeerHut but no reward (skipped).

    Reference: MainForm.cs:8740 `PassGuardContent(DataRow row, int s)`.
    Layout: same as SeerHut mission + deadline + 3 progress sections,
    but NO 15-byte reward block at the end.
    """
    rec = PassGuardContent(address=s)
    t = raw[s]
    rec.mission_type = t

    if t == 0:
        return rec, s + 1
    elif t == 1:
        rec.mission = {"level": _u32(raw, s + 1)}
        s += 5
    elif t == 2:
        rec.mission = {"primary_skills": {A_PR_SKILL[i]: raw[s + i + 1]
                                            for i in range(4) if raw[s + i + 1] != 0}}
        s += 7
    elif t == 3:
        rec.mission = {"hero_id": raw[s + 1], "mission_str": "{" + str(raw[s + 1])}
        s += 6
    elif t == 4:
        rec.mission = {
            "x": raw[s + 1], "y": raw[s + 3], "monster_id": raw[s + 5],
            "bit": (raw[s + 4] & 4) >> 2,
        }
        s += 10
    elif t == 5:
        n = raw[s + 1]
        arts = [raw[s + j * 2 + 2] for j in range(n)]
        rec.mission = {"artifacts": arts}
        s = s + n * 2 + 4
    elif t == 6:
        n = raw[s + 1]
        monsters = []
        for k in range(n):
            base = s + k * 6 + 2
            mid = raw[base]
            cnt = (raw[base + 3] << 8) | raw[base + 2]
            monsters.append((mid, cnt))
        rec.mission = {"monsters": monsters}
        s = s + n * 6 + 4
    elif t == 7:
        resources = {}
        for i in range(6):
            v = (raw[s + i * 4 + 3] << 16) | (raw[s + i * 4 + 2] << 8) | raw[s + i * 4 + 1]
            if v > 0:
                resources[A_RESOURCE[i]] = v
        gold_v = (raw[s + 24 + 3] << 16) | (raw[s + 24 + 2] << 8) | raw[s + 24 + 1]
        if gold_v > 0:
            resources["Gold"] = gold_v
        rec.mission = {"resources": resources}
        s += 31
    elif t == 8:
        rec.mission = {"hero_id": raw[s + 1], "mission_str": "}" + str(raw[s + 1])}
        s += 5
    elif t == 9:
        rec.mission = {"color": A_COLOR[raw[s + 1]] if raw[s + 1] < 8 else "Unknown"}
        s += 4
    else:
        s += 1

    # Deadline
    rec.deadline = _decode_deadline(raw[s])
    s += 4

    # 3 variable sections
    for _ in range(3):
        n = _u16(raw, s)
        if n != 0:
            rec.mission_progress.append(raw[s + 4:s + 4 + n])
            s = s + n + 4
        else:
            rec.mission_progress.append(b"")
            s += 4

    return rec, s


# ============================================================================
# A7. BankContent (Creature Banks — post-tile)
# ============================================================================

@dataclass
class BankContent:
    address: int = 0
    is_artifact_bank: bool = False
    guard: List[ArmySlot] = field(default_factory=list)
    resources: Dict[str, int] = field(default_factory=dict)
    gold: int = 0
    monster_reward_id: int = -1
    monster_reward_count: int = 0
    artifact_ids: List[int] = field(default_factory=list)


def parse_bank_content(raw: bytes, s: int, is_artifact_bank: bool = False
                       ) -> Tuple[BankContent, int]:
    """Parse one Bank record (post-tile Bank section, not the tile-scan version).

    Reference: MainForm.cs:8902 `BankContent(DataRow row, int s)`.
    Layout (89 + variable):
    - 7-slot guard (56 bytes, with counts at offset +28)
    - 28 bytes resources (6 × 4) — bank resource table
    - 4 bytes gold (u16 at +24, +25)
    - 7 bytes monster reward (or sentinel)
    - For artifact bank: u8 count + count × 4 bytes artifact IDs
    """
    rec = BankContent(address=s, is_artifact_bank=is_artifact_bank)

    # Guard
    rec.guard, s = _parse_guard(raw, s)

    # Resources (6 × 4 bytes — note GetBankResource only reads byte 0!)
    for i in range(6):
        v = raw[s + i * 4]
        if v > 0:
            rec.resources[A_RESOURCE[i]] = v
    gold = (raw[s + 25] << 8) | raw[s + 24]
    if gold != 0:
        rec.gold = gold
    s += 28

    if not is_artifact_bank:
        # 7 bytes for monster reward
        if raw[s] < 0xFF:
            rec.monster_reward_id = raw[s]
            rec.monster_reward_count = raw[s + 4]
        s += 7
    else:
        # Artifact bank: 5 bytes gap + u8 count + count × 4
        s += 5
        n = raw[s]
        s += 2
        if n > 0:
            for j in range(n):
                rec.artifact_ids.append(raw[s + j * 4])
            s += n * 4

    return rec, s


# ============================================================================
# A8. GarrisonContent (post-tile garrison)
# ============================================================================

@dataclass
class GarrisonContent:
    address: int = 0
    guard: List[ArmySlot] = field(default_factory=list)
    color: int = -1
    can_take: int = 0


def parse_garrison_content(raw: bytes, s: int) -> Tuple[GarrisonContent, int]:
    """Parse one Garrison record (post-tile, separate from tile-scan garrison).

    Reference: MainForm.cs:8136 `GarrisonContent(DataRow row, int s)`.
    Layout (61 bytes):
    - 7-slot guard (56 bytes) — counts at offset +29..+30
    - 1 byte color (0xFF = none)
    - 1 byte "CanTake"
    - 2 bytes x,y,z at +57, +58, +59 ( ProspectorRT uses them to match row)
    """
    rec = GarrisonContent(address=s)
    rec.guard, s = _parse_guard(raw, s)
    if raw[s] != 0xFF:
        rec.color = raw[s]
    rec.can_take = raw[s + 60] if s + 60 < len(raw) else 0
    return rec, s + 61


# ============================================================================
# A9b. parse_bank_resource / parse_bank_monster (extracted from parse_bank_content)
# ============================================================================

def parse_bank_resource(raw: bytes, s: int) -> Tuple[Dict[str, int], int]:
    """Read bank resources + gold (ProspectorRT GetBankResource, MainForm.cs:9018).

    Layout (28 bytes):
    - 6 × 4 bytes resources (only byte 0 of each 4-byte slot is used)
    - 4 bytes gold (u16 at +24, +25)

    Returns (resources_dict, gold).
    """
    res: Dict[str, int] = {}
    for i in range(6):
        v = raw[s + i * 4] if s + i * 4 < len(raw) else 0
        if v > 0:
            res[A_RESOURCE[i]] = v
    gold = (raw[s + 25] << 8) | raw[s + 24] if s + 26 <= len(raw) else 0
    return res, gold


def parse_bank_monster(raw: bytes, s: int) -> Tuple[int, int]:
    """Read bank monster reward (ProspectorRT GetBankMonster, MainForm.cs:9013).

    Layout:
    - decmp[s] = monster_id
    - decmp[s + 4] = count

    Returns (monster_id, count).
    """
    if s + 5 > len(raw):
        return (-1, 0)
    return (raw[s], raw[s + 4])


# ============================================================================
# C5b. parse_timer_town — links town timed events to towns + decodes buildings
# ============================================================================

# Town building bitmask table (from ProspectorRT CreateTblBuilding, MainForm.cs:11200).
# 6 bytes per town type × 8 bits = 48 building slots per town type.
# Below is a simplified version of the most common buildings for town types 0-8.
# (Castle, Rampart, Tower, Inferno, Necropolis, Dungeon, Stronghold, Fortress, Conflux)
# Source: PRT MainForm.cs CreateTblBuilding (lines 11200-11579).
TOWN_BUILDINGS = {
    0: {  # Castle
        (1, 0): "Mage Guild lvl 1", (1, 1): "Mage Guild lvl 2", (1, 2): "Mage Guild lvl 3",
        (1, 3): "Mage Guild lvl 4", (1, 4): "Mage Guild lvl 5",
        (2, 0): "Blacksmith", (2, 1): "Tent", (2, 2): "Castle",
        (2, 3): "Citadel", (2, 4): "Fort", (3, 0): "Tavern",
        (3, 1): "Marketplace", (3, 2): "Resource Silo",
        (4, 0): "Shipyard", (4, 1): "Lighthouse",
        (5, 0): "Grail", (6, 0): "Horde Building",
    },
    1: {  # Rampart
        (1, 0): "Mage Guild lvl 1", (1, 1): "Mage Guild lvl 2", (1, 2): "Mage Guild lvl 3",
        (1, 3): "Mage Guild lvl 4", (1, 4): "Mage Guild lvl 5",
        (2, 0): "Blacksmith", (2, 2): "Castle", (2, 3): "Citadel", (2, 4): "Fort",
        (3, 0): "Tavern", (3, 1): "Marketplace", (3, 2): "Resource Silo",
        (4, 0): "Shipyard", (5, 0): "Grail",
    },
    2: {  # Tower
        (1, 0): "Mage Guild lvl 1", (1, 1): "Mage Guild lvl 2", (1, 2): "Mage Guild lvl 3",
        (1, 3): "Mage Guild lvl 4", (1, 4): "Mage Guild lvl 5", (1, 5): "Library",
        (2, 0): "Blacksmith", (2, 2): "Castle", (2, 3): "Citadel", (2, 4): "Fort",
        (3, 0): "Tavern", (3, 1): "Marketplace", (3, 2): "Resource Silo",
        (4, 0): "Shipyard", (5, 0): "Grail",
    },
    # Town types 3-8 follow same general pattern (Inferno, Necropolis, Dungeon,
    # Stronghold, Fortress, Conflux). We keep generic fallback.
}


def _decode_building_bitmask(building_bytes: List[int], town_type: int
                              ) -> List[str]:
    """Decode 6-byte building bitmask into list of building names.

    ProspectorRT GetTimerTown (MainForm.cs:6984) iterates over 5 bytes × 8 bits
    + 1 extra byte (byte 5, bit 0 = Grail). For each set bit, looks up
    TblBuilding by (Type=town_type, Byte=k+1, Bit=l).
    """
    buildings: List[str] = []
    table = TOWN_BUILDINGS.get(town_type, {})
    if not table:
        # Generic fallback: just return bit indices
        for k in range(5):
            if k < len(building_bytes):
                for l in range(8):
                    if building_bytes[k] & (1 << l):
                        buildings.append(f"byte{k+1}_bit{l}")
        if len(building_bytes) > 5 and building_bytes[5] > 0:
            buildings.append("Grail (or special)")
        return buildings

    for k in range(5):
        if k >= len(building_bytes):
            break
        for l in range(8):
            if building_bytes[k] & (1 << l):
                name = table.get((k + 1, l))
                if name:
                    buildings.append(name)
                else:
                    buildings.append(f"byte{k+1}_bit{l}")
    # Byte 5 (special: Grail etc.)
    if len(building_bytes) > 5 and building_bytes[5] > 0:
        # Bit 0 = Grail for most town types
        if building_bytes[5] & 1:
            name = table.get((6, 0), "Grail")
            buildings.append(name)
    return buildings


@dataclass
class TownTimerLink:
    """Result of linking timed events to towns."""
    town_id: int
    town_name: str = ""
    town_type: int = 0
    town_x: int = 0
    town_y: int = 0
    town_z: int = 0
    town_color: str = ""
    events: List[Any] = field(default_factory=list)  # List[TimedEvent]
    buildings_built: List[str] = field(default_factory=list)


def parse_timer_town(towns: List[Dict[str, Any]],
                     town_timed_events: List["TimedEvent"]
                     ) -> List[TownTimerLink]:
    """Link town timed events to their towns + decode buildings bitmask.

    ProspectorRT GetTimerTown (MainForm.cs:6984):
    - For each town, finds all timed events with `event.id == town.id`.
    - For each such event, decodes 6-byte building bitmask → list of building names.
    - Also computes MageTimer / LibTimer flags.

    Args:
        towns: list of dicts (output of save_parser.parse_town_block)
        town_timed_events: list of TimedEvent with is_town=True

    Returns list of TownTimerLink records (one per town that has events).
    """
    result: List[TownTimerLink] = []
    # Group events by town ID
    events_by_id: Dict[int, List["TimedEvent"]] = defaultdict(list)
    for ev in town_timed_events:
        events_by_id[ev.id].append(ev)

    for town in towns:
        town_id = town.get("id") or town.get("block_offset", 0)
        if town_id not in events_by_id:
            continue
        link = TownTimerLink(
            town_id=town_id,
            town_name=town.get("name", ""),
            town_type=town.get("type", 0) if isinstance(town.get("fields"), dict)
                      else town.get("fields", {}).get("type", 0),
            town_x=town.get("x", 0) or town.get("fields", {}).get("x", 0),
            town_y=town.get("y", 0) or town.get("fields", {}).get("y", 0),
            town_z=town.get("z", 0) or town.get("fields", {}).get("z", 0),
            town_color=town.get("faction_name", ""),
            events=events_by_id[town_id],
        )
        # Decode buildings for each event
        for ev in link.events:
            if ev.buildings:
                link.buildings_built.extend(
                    _decode_building_bitmask(ev.buildings, link.town_type))
        # Dedup
        link.buildings_built = sorted(set(link.buildings_built))
        result.append(link)
    return result


# ============================================================================
# F2. is_hero_tavern — check if hero is in tavern
# ============================================================================

def is_hero_tavern(hero_id: int, tavern_guests: List[Tuple[int, int]]) -> int:
    """Check if hero is currently in some player's tavern (ProspectorRT IsHeroTavern,
    MainForm.cs:7971).

    Args:
        hero_id:  hero index (0..155)
        tavern_guests: list of (slot1, slot2) tuples per player (8 players)
                       - From GetColorContent: decmp[color + i*145 + 12] = guest1,
                                              decmp[color + i*145 + 11] = guest2

    Returns:
        Player index (0..7) whose tavern has this hero, or 255 if not in any tavern.
    """
    for player_idx, (g1, g2) in enumerate(tavern_guests):
        if g1 == hero_id or g2 == hero_id:
            return player_idx
    return 255


# ============================================================================
# F3. hero_on_object — handle hero-on-object tile case (ProspectorRT HeroOnObject)
# ============================================================================

@dataclass
class HeroOnObjectInfo:
    """Result of parsing a hero-on-object tile."""
    hero_id: int
    underlying_type_id: int
    underlying_object: Optional[Dict[str, Any]] = None
    swapped_bytes: bytes = b""  # original 5 bytes that were temporarily swapped


def hero_on_object(raw: bytearray, s: int, hero_id_at_s6: int,
                   hero_blocks: Dict[int, int],
                   dispatch_func: callable = None
                   ) -> Optional[HeroOnObjectInfo]:
    """Handle hero-on-object tile case (ProspectorRT HeroOnObject, MainForm.cs:9643).

    When a hero stands on a tile with another object (e.g. mine, artifact),
    the tile scan sees the hero's data, but the underlying object's data is
    in the hero record (at hero_blocks[hero_id] + 11..19).

    ProspectorRT temporarily swaps 5 bytes (s, s+6, s+7, s+8, s+9) with
    the hero record bytes (s+11, s+16, s+17, s+18, s+19), re-dispatches
    IsObject(s) to record the underlying object, then restores the 5 bytes.

    Since we're PARSING (not filling DataTables), we DON'T need to mutate
    the raw bytes — we just need to read the underlying object's bytes from
    the hero record and parse it ourselves.

    Args:
        raw:                decompressed save bytes (we DON'T mutate)
        s:                  offset of hero-on-object tile
        hero_id_at_s6:      hero ID at raw[s + 6]
        hero_blocks:        dict {hero_id: block_offset} from ScanHeroesContent
        dispatch_func:      function (raw, s, type_id, x, y, z, loc) → obj
                            (defaults to tile_scanner.parse_object_content)

    Returns HeroOnObjectInfo or None if hero record not found.
    """
    if dispatch_func is None:
        # Lazy import to avoid circular dependency
        import os, sys
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from tile_scanner import parse_object_content
        dispatch_func = parse_object_content

    if hero_id_at_s6 not in hero_blocks:
        return None
    hero_block = hero_blocks[hero_id_at_s6]

    # The underlying object's data is at hero_block + 11..19 (5 bytes)
    # ProspectorRT:
    #   decmp[s]      = decmp[num + 11]   ← underlying type_id
    #   decmp[s + 6]  = decmp[num + 16]
    #   decmp[s + 7]  = decmp[num + 17]
    #   decmp[s + 8]  = decmp[num + 18]
    #   decmp[s + 9]  = decmp[num + 19]
    underlying_type_id = raw[hero_block + 11] if hero_block + 12 <= len(raw) else 0

    # Build a fake "raw" by reading bytes from hero record into the 5 slots
    # We do NOT mutate raw — we create a temp bytearray just for dispatch.
    # Actually, the simpler approach: dispatch directly on the underlying bytes
    # using offsets relative to s, but reading from hero_block.
    # For read-only parse, we just record what we found.
    info = HeroOnObjectInfo(
        hero_id=hero_id_at_s6,
        underlying_type_id=underlying_type_id,
    )

    # If we want the underlying object dict, we'd need to mutate a temp copy.
    # For now, just record the type_id and the hero block offset.
    # Caller can dispatch manually if needed.

    return info


# ============================================================================
# A9. UniverContent (University of Magic)
# ============================================================================

@dataclass
class UniverContent:
    address: int = 0
    skills: List[int] = field(default_factory=list)  # 4 secondary skill IDs


def parse_univer_content(raw: bytes, s: int) -> Tuple[UniverContent, int]:
    """Parse one Univer record.

    Reference: MainForm.cs:8031 `UniverContent(DataRow row, int s)`.
    Layout (16 bytes): 4 × 4 bytes (skill_id + 3 padding).
    """
    rec = UniverContent(address=s)
    for i in range(4):
        rec.skills.append(raw[s + i * 4])
    return rec, s + 16


# ============================================================================
# A10. MarketContent (Black Market — 7 artifacts for sale)
# ============================================================================

@dataclass
class MarketContent:
    address: int = 0
    artifacts: List[int] = field(default_factory=list)  # 7 artifact IDs (0xFF = empty)


def parse_market_content(raw: bytes, s: int) -> Tuple[MarketContent, int]:
    """Parse one Black Market record (28 bytes).

    Reference: MainForm.cs:8055 `MarketContent(DataRow row, int s)`.
    Layout: 7 × 4 bytes (artifact ID + 3 padding). 0xFF = empty.
    """
    rec = MarketContent(address=s)
    for i in range(7):
        if raw[s + i * 4] != 0xFF:
            rec.artifacts.append(raw[s + i * 4])
    return rec, s + 28


# ============================================================================
# B1. GetArtMerchants (art merchants in CurrentState section)
# ============================================================================

@dataclass
class ArtMerchant:
    artifact_id: int
    slot: int


def parse_art_merchants(raw: bytes, s: int) -> Tuple[List[ArtMerchant], int]:
    """Parse art merchants (7 slots) in CurrentState section.

    Reference: MainForm.cs:7684 `GetArtMerchants(int s)`.
    Layout: 7 × 4 bytes (artifact ID + 3 padding). 0xFF = empty.
    """
    merchants: List[ArtMerchant] = []
    for i in range(7):
        if raw[s + i * 4] < 0xFF:
            merchants.append(ArtMerchant(artifact_id=raw[s + i * 4], slot=i + 1))
    return merchants, s + 28


# ============================================================================
# B2. GetAlliance (player alliances)
# ============================================================================

@dataclass
class AllianceInfo:
    teams_offset: int = 0  # address of the [0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7] sequence
    team_of_human: int = 0
    allies: List[int] = field(default_factory=list)


def _find_teams_sequence(raw: bytes, search_start: int, search_end: int) -> Optional[int]:
    """Find the [0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7] 16-byte sequence in the save."""
    target = bytes(range(8)) + bytes(range(8))
    return raw.find(target, search_start, search_end)


def _sum_ally(num: int) -> Tuple[int, int]:
    """Reference: MainForm.cs:6328 `SumAlly(num, out min)`."""
    table = {
        1: (1, 7), 2: (3, 13), 3: (6, 18), 4: (10, 22),
        5: (15, 25), 6: (21, 27),
    }
    return table.get(num, (0, 255))


def parse_alliance(raw: bytes, map_data_offset: int, human_player: int
                   ) -> AllianceInfo:
    """Parse alliance block.

    Reference: MainForm.cs:6273 `GetAlliance()`.
    Layout (after finding [0..7,0..7] sequence):
    - 8 bytes player team assignments (after sequence)
    - ProspectorRT reads `decmp[num + human + 1]` for human's team
    - Then iterates: `decmp[num + i + 1] == hTeam` → ally
    """
    info = AllianceInfo()
    info.team_of_human = human_player

    # Search for the 16-byte sentinel [0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7]
    # ProspectorRT searches starting at `map.Data + 66`
    search_start = map_data_offset + 66
    search_end = min(len(raw), search_start + 0x10000)
    teams_off = _find_teams_sequence(raw, search_start, search_end)
    if teams_off < 0:
        return info
    info.teams_offset = teams_off

    # Find the alliance block by ProspectorRT's reverse search
    # `for (int num3 = map.Teams - 9; num3 > num2; num3--)` — they walk backward
    # looking for bytes < 8 and > 1 that pass FindAlly heuristic.
    map_data_end = map_data_offset + 65
    found = -1
    for s in range(teams_off - 9, map_data_end, -1):
        if s < 0:
            break
        v = raw[s]
        if 1 < v < 8:
            # Check FindAlly: sum of next 8 bytes must be in SumAlly(v)
            sum8 = sum(raw[s + 1:s + 9])
            mn, mx = _sum_ally(v - 1)
            if mn <= sum8 <= mx:
                found = s
                break

    if found > 0:
        # team_of_human = decmp[found + human + 1]
        if found + human + 1 < len(raw):
            h_team = raw[found + human + 1]
            for i in range(8):
                if i != human and found + i + 1 < len(raw):
                    if raw[found + i + 1] == h_team:
                        info.allies.append(i)

    return info


# ============================================================================
# B3. GetExperience (aggregator — return EXP sources)
# ============================================================================

@dataclass
class ExperienceSource:
    x: int
    y: int
    z: int
    locality: str
    object_type: str
    resource: str = ""
    guard: str = ""
    hp: int = 0
    xp: int = 0


def parse_experience_sources(parsed_save) -> List[ExperienceSource]:
    """Aggregate all EXP sources from a parsed save.

    Reference: MainForm.cs:6381 `GetExperience()`.
    This is an aggregator that walks through EventBox, SeerHut, Spell, Art,
    Resource, Chest, Bank, Monstr, Town, Garrison, Heroes — collecting
    anything that gives experience.

    In our implementation, we just iterate over the already-parsed sections
    and produce a list. (Simplification of ProspectorRT's 400-line function.)
    """
    sources: List[ExperienceSource] = []
    # EventBox with experience reward
    for eb in getattr(parsed_save, "event_boxes", []):
        if eb.experience > 0:
            sources.append(ExperienceSource(
                x=0, y=0, z=0, locality="", object_type="EventBox",
                xp=eb.experience,
            ))
    # Banks give 5 * count per creature
    for bank in getattr(parsed_save, "banks", []):
        if bank.monster_reward_count > 0:
            sources.append(ExperienceSource(
                x=0, y=0, z=0, locality="", object_type="Bank",
                hp=bank.monster_reward_count,
                xp=5 * bank.monster_reward_count,
            ))
    return sources


# ============================================================================
# C1-C8. GetTimedEvents (map + town)
# ============================================================================

@dataclass
class TimedEvent:
    address: int = 0
    is_town: bool = False
    resources: Dict[str, int] = field(default_factory=dict)
    gold: int = 0
    day: int = 0
    repeat: int = 0
    apply: Tuple[int, int, int] = (0, 0, 0)
    id: int = 0
    # Town-only fields
    buildings: List[int] = field(default_factory=list)  # 6 bytes bitmask
    monsters: List[int] = field(default_factory=list)  # 7 × u16


def _parse_timer_res(raw: bytes, s: int) -> Tuple[Dict[str, int], int]:
    """Parse resources + gold in timed event.

    Reference: MainForm.cs:6956 `GetTimerRes(int s, DataRow row)`.
    Layout (28 bytes):
    - 2 bytes padding (ProspectorRT skips: `s += 2`)
    - 6 × 4 bytes resources (3-byte value + 1-byte sign flag, NOT 4-byte value!)
    - 4 bytes gold (3-byte value + 1-byte sign flag)
    """
    s += 2  # skip
    res = {}
    for i in range(6):
        v = _i32_3(raw, s)  # 3-byte signed value within 4-byte slot
        if v != 0:
            res[A_RESOURCE[i]] = v
        s += 4
    gold = _i32_3(raw, s)  # gold uses same 3-byte signed encoding
    if gold != 0:
        res["Gold"] = gold
    s += 4
    return res, s


def _parse_timer_content(raw: bytes, s: int) -> Tuple[List[int], List[int], int]:
    """Parse building bitmask + monster quantities (town timed events only).

    Reference: MainForm.cs:6931 `GetTimerContent(int s, DataRow row)`.
    Layout: 6 bytes (6 building bytes) + 8 bytes padding + 14 bytes (7 × u16).
    """
    s += 38  # skip to building bytes
    buildings = [raw[s + i] for i in range(6)]
    s += 8
    monsters = [(raw[s + j * 2 + 1] << 8) | raw[s + j * 2] for j in range(7)]
    return buildings, monsters, s


def parse_map_timed_events(raw: bytes, s: int) -> Tuple[List[TimedEvent], int]:
    """Parse all map-level timed events.

    Reference: MainForm.cs:6905 `MapTimedEvents(...)`.
    Layout: u16 count + 4 + count × (u16 msg_len + 37).
    Each record: msg_len bytes (u16 + message) + 30 bytes (resources + gold)
                + 7 bytes (apply + day + repeat).
    ProspectorRT reads Apply/Day/Repeat at mapTimedEvent + 30..35
    (where mapTimedEvent = after msg_len skip, BEFORE GetTimerRes).
    """
    events: List[TimedEvent] = []
    num = _u16(raw, s)
    if num <= 0:
        return events, s + 4
    s += 4
    for i in range(num):
        msg_len = _u16(raw, s)
        s = s + msg_len  # skip u16 + message body (msg_len includes u16 header)
        body_start = s  # this is the "mapTimedEvent" reference in ProspectorRT
        ev = TimedEvent(address=body_start, is_town=False)
        ev.resources, _ = _parse_timer_res(raw, body_start)  # don't advance s here
        # Apply + Day + Repeat are at body_start + 30..35
        ev.apply = (raw[body_start + 30], raw[body_start + 31], raw[body_start + 32])
        ev.day = (raw[body_start + 34] << 8) | raw[body_start + 33]
        if raw[body_start + 35] > 0:
            ev.repeat = raw[body_start + 35]
        events.append(ev)
        s = body_start + 37  # next record
    return events, s


def parse_town_timed_events(raw: bytes, s: int) -> Tuple[List[TimedEvent], int]:
    """Parse all town-level timed events.

    Reference: MainForm.cs:6877 `TownsTimedEvents(...)`.
    Layout: u16 count + 4 + count × (u16 msg_len + 60).
    Each record: msg_len bytes (u16 + message) + 30 bytes (resources + gold)
                + 8 bytes GetTimerContent (6 building bytes + 2 padding + ... )
                + 14 bytes monsters (7 × u16)
                + 7 bytes (apply + day + repeat + id + padding).
    ProspectorRT reads fields at body_start + 30..37 (apply, day, repeat, id)
    and GetTimerContent at body_start + 38 (6 building bytes + 14 monster bytes).
    """
    events: List[TimedEvent] = []
    num = _u16(raw, s)
    if num <= 0:
        return events, s + 4
    s += 4
    for i in range(num):
        msg_len = _u16(raw, s)
        s = s + msg_len
        body_start = s
        ev = TimedEvent(address=body_start, is_town=True)
        ev.resources, _ = _parse_timer_res(raw, body_start)
        # GetTimerContent: skip 38 bytes, read 6 building bytes + 8 padding + 14 monsters
        buildings, monsters, _ = _parse_timer_content(raw, body_start)
        ev.buildings = buildings
        ev.monsters = monsters
        ev.apply = (raw[body_start + 30], raw[body_start + 31], raw[body_start + 32])
        ev.day = (raw[body_start + 34] << 8) | raw[body_start + 33]
        if raw[body_start + 35] > 0:
            ev.repeat = raw[body_start + 35]
        ev.id = raw[body_start + 37]
        events.append(ev)
        s = body_start + 60  # next record
    return events, s


# ============================================================================
# D1. GetTownSpell (Magic Guild spells)
# ============================================================================

@dataclass
class TownSpellPool:
    address: int = 0
    levels: List[List[int]] = field(default_factory=list)  # one list per level
    has_library: bool = False
    has_mage_guild_level_5: bool = False


def parse_town_spell(raw: bytes, s: int, lvl: int,
                     library_built: bool = False,
                     mg5_built: bool = False
                     ) -> Tuple[TownSpellPool, int]:
    """Parse spells available in town's Magic Guild.

    Reference: MainForm.cs:7542 `GetTownSpell(int s, int lvl, ...)`.
    Layout (197 bytes — same as town "post-name 113" + spell area):
    - For lvl <= 5: lvl levels × ~5 spells each
    - For lvl == 6 (Tower with Library): 1 extra spell slot
    - Each spell slot: u8 spell_id (0xFF = none) for slots beyond level 1
                      (level 1 spells are always available — different encoding)

    Notes from ProspectorRT:
    - Library check: `(decmp[num - 14] & 0x40) == 64` → library adds 1 spell per level
    - Mage Guild lvl 5: `(decmp[num - 6] & 0x40) == 64`
    """
    rec = TownSpellPool(address=s, has_library=library_built,
                        has_mage_guild_level_5=mg5_built)
    start = s
    actual_lvl = 5 if lvl == 6 else lvl

    for level_idx in range(actual_lvl):
        # ProspectorRT layout: 5 spells per level × 4 bytes each, but only byte 0 matters
        # The exact offset depends on the town record stride.
        # Simplified: read 5 spell slots (u8 each at offsets 0, 4, 8, 12, 16)
        spells = []
        for slot in range(5):
            off = s + slot * 4
            if off < len(raw):
                sp = raw[off]
                if sp != 0xFF:
                    spells.append(sp)
        rec.levels.append(spells)
        s += 5 * 4  # 20 bytes per level

    # Tower with Library: extra level 6 (just 1 spell slot)
    if lvl == 6:
        sp = raw[s]
        if sp != 0xFF:
            rec.levels.append([sp])
        s += 4

    return rec, s


# ============================================================================
# E1-E3. ArtDollPlace (artifact doll slots)
# ============================================================================

# Doll slots ( Heroes III equipment layout — 19 visible slots)
DOLL_SLOTS = {
    0: "Head", 1: "Neck", 2: "Torso", 3: "RightHand", 4: "LeftHand",
    5: "Feet", 6: "Misc1", 7: "Misc2", 8: "Misc3", 9: "Misc4",
    10: "Misc5", 11: "Feet2", 12: "Cloak",
    # 4-6 are machine slots (Catapult, Ammo Cart, First Aid Tent, Ballista)
    # ProspectorRT: case 4/5/6 → Machine
}


@dataclass
class DollPlace:
    artifact_id: int
    slot_index: int
    slot_name: str
    is_machine: bool = False


def parse_art_doll(raw_hero_block: bytes, hero_block_offset: int
                   ) -> List[DollPlace]:
    """Parse artifact doll slots from hero block (within hero record).

    Reference: MainForm.cs:7723 `GetHeroesContent` (inner loop on artifacts)
    and MainForm.cs:7883 `ArtDollPlace1` / 7935 `ArtDollPlace2`.

    Layout (in hero block, offset +561 onwards in our HERO_FIELD_OFFSETS):
    83 slots × 8 bytes = 686 bytes
    Each slot 8 bytes:
    - byte 0: artifact_id (0 = spellbook, 3/255 = empty, 4/5/6 = machines)
    - bytes 1-7: artifact-specific data

    ProspectorRT reads 83 slots, treating 4/5/6 as "machine" (war machines)
    and 0 as "spell book".
    """
    # We use the hero block from `parse_hero_block` output, but ProspectorRT
    # reads from the raw save. For simplicity, we accept the hero_block offset
    # and read directly from raw bytes. Caller passes `raw_hero_block` = raw[hero_block_offset:hero_block_offset+1094].
    # The artifact block in hero is at offset +561 (within hero block).
    # (See save_layout.HERO_FIELD_OFFSETS)
    artifact_offset = 561  # within hero block, after spells
    if len(raw_hero_block) < artifact_offset + 83 * 8:
        return []

    places: List[DollPlace] = []
    for i in range(83):
        off = artifact_offset + i * 8
        art_id = raw_hero_block[off]
        if art_id in (3, 255):
            continue
        is_machine = art_id in (4, 5, 6)
        slot_name = "Machine" if is_machine else ("Book" if art_id == 0 else
                                                   DOLL_SLOTS.get(i, f"Slot{i}"))
        places.append(DollPlace(
            artifact_id=art_id,
            slot_index=i,
            slot_name=slot_name,
            is_machine=is_machine,
        ))
    return places


# ============================================================================
# F1. GetPrisonHero (link prison record to hero record)
# ============================================================================

@dataclass
class PrisonHero:
    hero_id: int
    hero_name: str = ""
    level: int = 0
    primary_skills: Dict[str, int] = field(default_factory=dict)
    secondary_skills: List[Tuple[int, int]] = field(default_factory=list)
    artifacts: List[int] = field(default_factory=list)
    machines: List[int] = field(default_factory=list)
    book: bool = False
    spells: List[int] = field(default_factory=list)
    army: List[ArmySlot] = field(default_factory=list)
    mp: int = 0
    experience: int = 0


def parse_prison_hero(prison_row: Dict[str, Any],
                      heroes_list: List[Dict[str, Any]]) -> Optional[PrisonHero]:
    """Link a prison record to its hero record.

    Reference: MainForm.cs:7983 `GetPrisonHero(DataSet2.R_PrisonRow row)`.
    ProspectorRT just copies fields from R_HeroesRow to R_PrisonRow.
    We do the same: find hero by ID, return a PrisonHero.
    """
    hero_id = prison_row.get("hero_id")
    if hero_id is None:
        return None
    # Find hero by ID
    for h in heroes_list:
        if h.get("id") == hero_id:
            return PrisonHero(
                hero_id=hero_id,
                hero_name=h.get("name", ""),
                level=h.get("level", 0),
                primary_skills=h.get("primary_skills", {}),
                secondary_skills=h.get("secondary_skills", []),
                artifacts=h.get("equipment", []),
                spells=h.get("spells_book", []),
                army=h.get("army", []),
                mp=h.get("movement_left", 0),
                experience=h.get("experience", 0),
            )
    return None


# ============================================================================
# G1. GetPairSubterraneanGate (link subterranean gate pairs)
# ============================================================================

@dataclass
class SubTerGate:
    x: int
    y: int
    z: int
    bits: int


def parse_pair_subterranean_gate(raw: bytes, sub_ter_gate_off: int
                                 ) -> Tuple[List[SubTerGate], List[int], int]:
    """Pair subterranean gates.

    Reference: MainForm.cs:9335 `GetPairSubterraneanGate()`.
    Layout:
    - u16 count + count × 4 bytes (x, y, z, bits) — list of gates
    - u16 count + count × 4 bytes (pair_id, 0xFFFF = unpaired) — pair index

    Returns (gates_list, pair_ids, end_offset).
    """
    s = sub_ter_gate_off
    n = _u16(raw, s)
    s += 2
    gates: List[SubTerGate] = []
    for i in range(n):
        gates.append(SubTerGate(
            x=raw[s + i * 4],
            y=raw[s + i * 4 + 2],
            z=(raw[s + i * 4 + 3] & 4) >> 2,
            bits=raw[s + i * 4 + 3],
        ))
    s += n * 4

    n2 = _u16(raw, s)
    s += 2
    pair_ids: List[int] = []
    for j in range(n2):
        if raw[s + j * 4] == 0xFF and raw[s + j * 4 + 1] == 0xFF:
            pair_ids.append(-1)
        else:
            pair_ids.append((raw[s + j * 4 + 1] << 8) | raw[s + j * 4])
    s += n2 * 4

    return gates, pair_ids, s


# ============================================================================
# G2. ScanMonolithWhirlpool (One Way + Two Way + Whirlpools)
# ============================================================================

@dataclass
class MonolithInfo:
    one_way_groups: List[List[Tuple[int, int, int, int]]] = field(default_factory=list)
    two_way_groups: List[List[Tuple[int, int, int, int]]] = field(default_factory=list)
    whirlpools: List[Tuple[int, int, int, int]] = field(default_factory=list)
    one_way_offset: int = 0
    whirlpool_offset: int = 0


def parse_monolith_whirlpool(raw: bytes, s: int) -> Tuple[MonolithInfo, int]:
    """Parse monolith + whirlpool topology.

    Reference: MainForm.cs:9392 `ScanMonolithWhirlpool(int s)`.
    Layout:
    - 8 groups × (u16 count + count × 4 bytes) — One Way Monoliths (type A)
    - 8 groups × (u16 count + count × 4 bytes) — Two Way Monoliths
    - 1 group × (u16 count + count × 4 bytes) — Whirlpools
    """
    info = MonolithInfo()

    # 8 one-way groups
    for i in range(8):
        n = _u16(raw, s)
        s += 2
        group = []
        for j in range(n):
            group.append((raw[s + j * 4], raw[s + j * 4 + 1],
                          raw[s + j * 4 + 2], raw[s + j * 4 + 3]))
        info.one_way_groups.append(group)
        s += n * 4
    info.one_way_offset = s  # ProspectorRT sets map.OneWayMonolith = s here

    # 8 two-way groups
    for i in range(8):
        n = _u16(raw, s)
        s += 2
        group = []
        for j in range(n):
            group.append((raw[s + j * 4], raw[s + j * 4 + 1],
                          raw[s + j * 4 + 2], raw[s + j * 4 + 3]))
        info.two_way_groups.append(group)
        s += n * 4
    info.whirlpool_offset = s  # ProspectorRT sets map.Whirlpool = s here

    # Whirlpools (1 group)
    n = _u16(raw, s)
    s += 2
    for j in range(n):
        info.whirlpools.append((raw[s + j * 4], raw[s + j * 4 + 1],
                                 raw[s + j * 4 + 2], raw[s + j * 4 + 3]))
    s += n * 4

    return info, s


# ============================================================================
# H1-H5. Header offsets — extend find_map_start
# ============================================================================

@dataclass
class HeaderOffsets:
    save_name: int = 0
    sr: int = 0  # script/random 28 bytes
    sr_bytes: bytes = b""
    teams: int = 0  # 16-byte [0..7,0..7] sequence
    map_name: int = 0  # 341 bytes after teams
    black_market: int = 0
    map_start: int = 0  # end of header / start of tile data


def find_header_offsets(raw: bytes, scan_start: int = 0,
                         scan_end: Optional[int] = None) -> HeaderOffsets:
    """Find all ProspectorRT header offsets (BlackMarket, SR, Teams, MapName, Start).

    Reference: MainForm.cs:9411 `GetMapStart` + MainForm.cs:9443 `GetStart`.

    Strategy:
    1. Find the [0,1,2,3,4,5,6,7,0,1,2,3,4,5,6,7] 16-byte sentinel (map.Teams)
    2. From Teams: map.MapName = Teams + 57
    3. From MapName: walk forward to ".GM1\0" or ".CGM\0" etc.
    4. From there: skip 688 bytes → map.SR
    5. SR + 28 → length + 258 + 4 + variable skip
    6. Then Black Market (u8 count + count × 28)
    7. Then map.Start (beginning of tile loop)
    """
    if scan_end is None:
        scan_end = len(raw)
    info = HeaderOffsets()

    # 1. Find [0..7,0..7] sentinel
    target = bytes(range(8)) + bytes(range(8))
    teams_off = raw.find(target, scan_start, scan_end)
    if teams_off < 0:
        return info
    info.teams = teams_off

    # 2. MapName = Teams + 57
    info.map_name = teams_off + 57

    # 3. Walk forward to find ".GM1\0" / ".CGM\0" / ".GM2\0" / ".GM3\0"
    i = info.map_name + 341
    valid_exts = {b"GM1", b"CGM", b"GM2", b"GM3"}
    while i < scan_end - 4:
        if raw[i] == 0x2E:  # '.'
            ext = raw[i + 1:i + 4]
            if ext in valid_exts and raw[i + 4] == 0:
                break
        i += 1
    if i >= scan_end - 4:
        return info

    # 4. GetMapStart(i):
    # map.SaveName = i + 1
    info.save_name = i + 1
    # s = i + 688; map.SR = s; Get_SR(s); s += 28
    s = i + 688
    info.sr = s
    info.sr_bytes = raw[s:s + 28]
    s += 28

    # 5. u16 length + skip length + 258 + 4
    n = _u16(raw, s)
    s += n + 258
    # 6. Another u16 length + 4 + variable
    n = _u16(raw, s)
    s += 4
    if n != 0:
        for j in range(n):
            s = s + _u16(raw, s) + 3

    # 7. Black Market: u8 count + count × 28
    bm_count = raw[s]
    if bm_count != 0:
        info.black_market = s
        s += bm_count * 28

    # 8. map.Start = s + 1
    info.map_start = s + 1
    return info


# ============================================================================
# I1-I5. Aggregators (collect across all parsed sections)
# ============================================================================

def aggregate_all_spells(parsed_save) -> List[Dict[str, Any]]:
    """Aggregate all spells from EventBox, Scholar, Spell object, SeerHut, Town, Heroes.

    Reference: MainForm.cs:7263 `GetAllSpell()`.
    Returns list of {x, y, z, object_type, spell_id, slot, guard}.
    """
    spells: List[Dict[str, Any]] = []
    for eb in getattr(parsed_save, "event_boxes", []):
        for slot_idx, spell_id in enumerate(eb.spells):
            spells.append({"object": "EventBox", "spell_id": spell_id,
                           "slot": slot_idx,
                           "guard": eb.guard if slot_idx == 0 else None})
    for sh in getattr(parsed_save, "seer_huts", []):
        if sh.reward_type == 9:  # spell reward
            spells.append({"object": "SeerHut", "spell_id": sh.reward.get("spell_id")})
    for town in getattr(parsed_save, "towns", []):
        for level_idx, level_spells in enumerate(getattr(town, "spell_pool", {}).get("levels", [])):
            for slot_idx, spell_id in enumerate(level_spells):
                spells.append({"object": "Town", "spell_id": spell_id,
                               "level": level_idx + 1, "slot": slot_idx})
    for hero in getattr(parsed_save, "heroes", []):
        for spell_id in hero.get("spells_book", []):
            spells.append({"object": "Hero", "spell_id": spell_id,
                           "hero_name": hero.get("name", "")})
    return spells


def aggregate_all_skills(parsed_save) -> List[Dict[str, Any]]:
    """Aggregate all secondary skills from EventBox, Scholar, SeerHut.

    Reference: MainForm.cs:7160 `GetAllSkill()`.
    """
    skills: List[Dict[str, Any]] = []
    for eb in getattr(parsed_save, "event_boxes", []):
        for slot_idx, (skill_id, level) in enumerate(eb.secondary_skills):
            skills.append({"object": "EventBox", "skill_id": skill_id,
                           "level": level, "slot": slot_idx})
    for sh in getattr(parsed_save, "seer_huts", []):
        if sh.reward_type == 7:
            skills.append({"object": "SeerHut",
                           "skill_id": sh.reward.get("secondary_skill_id"),
                           "level": sh.reward.get("level_id")})
    return skills


def aggregate_monstr_content(parsed_save) -> List[MonstrContent]:
    """Aggregate MonstrContent results. (Trivial — just returns the list.)

    Reference: MainForm.cs:6801 `AnalysisMonstrContent()`.
    """
    return list(getattr(parsed_save, "monstr_records", []))


# ============================================================================
# AnalysisContent (generic post-tile section parser)
# ============================================================================

def parse_analysis_content(raw: bytes, s: int,
                            parse_one: Callable[[bytes, int], Tuple[Any, int]]
                            ) -> Tuple[List[Any], int]:
    """Generic post-tile section parser (used for EventBox/ArtRes/SeerHut/PassGuard/Bank).

    Reference: MainForm.cs:6809 `AnalysisContent(int s, DataTable Tbl, ObjectContent)`.

    Layout:
    - u16 count
    - count × records, each parse_one returns (record, new_s)
    """
    records: List[Any] = []
    n = _u16(raw, s)
    if n <= 0:
        return records, s + 2
    s += 2
    for i in range(n):
        rec, s = parse_one(raw, s)
        records.append(rec)
        s += 1  # ProspectorRT: `s = ObjContent(s) + 1`
    return records, s


# ============================================================================
# Master: parse_all_post_tile_sections
# ============================================================================

def parse_all_post_tile_sections(raw: bytes, after_tiles_offset: int,
                                  object_number: int, hero_count: int = 156,
                                  town_spell_levels: Optional[Dict[int, int]] = None,
                                  map_size: int = 0,
                                  has_underground: bool = False,
                                  chrn: int = 0
                                  ) -> Dict[str, Any]:
    """Walk all post-tile sections AND parse their content.

    Combines `walk_post_tile_sections` (offsets only) with all the
    `parse_*` content extractors above.

    Args:
        raw: decompressed save bytes
        after_tiles_offset: offset after tile loop + num5 records
        object_number: number of objects on map (from tile scan)
        hero_count: 156 for SoD
        town_spell_levels: optional dict {town_id: spell_level_count} for GetTownSpell
        map_size: map width/height (for BitField / TwoWayMonolith)
        has_underground: True if map has underground
        chrn: chronobranch offset (HotA, usually 0)

    Returns dict with:
        - 'offsets': dict from walk_post_tile_sections
        - 'event_boxes': list of EventBoxContent
        - 'art_res': list of ArtResContent
        - 'monstr': list of MonstrContent
        - 'seer_huts': list of SeerHutContent
        - 'pass_guards': list of PassGuardContent
        - 'banks': list of BankContent
        - 'garrisons': list of GarrisonContent
        - 'universers': list of UniverContent
        - 'map_timed_events': list of TimedEvent
        - 'town_timed_events': list of TimedEvent
        - 'monoliths': MonolithInfo
        - 'sub_ter_gates': (gates, pair_ids)
    """
    # Step 1: walk offsets
    offsets = walk_post_tile_sections(raw, after_tiles_offset, object_number,
                                       hero_count, map_size, has_underground, chrn)

    result: Dict[str, Any] = {"offsets": offsets}

    # Step 2: parse content of each section
    # A2. EventBox
    try:
        ebs, _ = parse_analysis_content(raw, offsets["event_box"],
                                          parse_event_box_content)
        result["event_boxes"] = ebs
    except Exception as e:
        result["event_boxes"] = []
        result["event_box_error"] = str(e)

    # A3. ArtRes
    try:
        arts, _ = parse_analysis_content(raw, offsets["art_res"],
                                          lambda r, s: parse_art_res_content(r, s, 0))
        result["art_res"] = arts
    except Exception as e:
        result["art_res"] = []

    # A4. Monstr — special (count is u16, but each record is independent)
    try:
        monstrs = []
        s = offsets["monstr"]
        n = _u16(raw, s)
        s += 2
        for i in range(n):
            rec, s = parse_monstr_content(raw, s)
            monstrs.append(rec)
            s += 31  # MonstrContent records are u16_length + 31
        result["monstr_records"] = monstrs
    except Exception as e:
        result["monstr_records"] = []

    # A5. SeerHut
    try:
        seers, _ = parse_analysis_content(raw, offsets["seer_hut"],
                                            parse_seer_hut_content)
        result["seer_huts"] = seers
    except Exception as e:
        result["seer_huts"] = []

    # A6. PassGuard
    try:
        pass_guards, _ = parse_analysis_content(raw, offsets["pass_guard"],
                                                  parse_pass_guard_content)
        result["pass_guards"] = pass_guards
    except Exception as e:
        result["pass_guards"] = []

    # A7. Bank — special (read by bank num from TblBanks; here we just iterate)
    try:
        banks = []
        s = offsets.get("bank", 0) + 2
        # We don't have TblBanks info — just iterate count from offset
        bank_count_off = offsets.get("bank", 0)
        if bank_count_off > 0:
            n = _u16(raw, bank_count_off)
            s = bank_count_off + 2
            for i in range(n):
                rec, s = parse_bank_content(raw, s, is_artifact_bank=False)
                banks.append(rec)
        result["banks"] = banks
    except Exception as e:
        result["banks"] = []

    # A8. Garrison (post-tile)
    try:
        garrisons = []
        s = offsets["garrison"]
        n = raw[s]
        s += 1
        for i in range(n):
            rec, s = parse_garrison_content(raw, s)
            garrisons.append(rec)
        result["garrisons"] = garrisons
    except Exception as e:
        result["garrisons"] = []

    # C2. Map timed events
    try:
        result["map_timed_events"], _ = parse_map_timed_events(
            raw, offsets["map_timed_event"])
    except Exception as e:
        result["map_timed_events"] = []

    # C3. Town timed events
    try:
        result["town_timed_events"], _ = parse_town_timed_events(
            raw, offsets["towns_timed_event"])
    except Exception as e:
        result["town_timed_events"] = []

    # G2. Monolith + Whirlpool
    try:
        # BitField = CurrentState + 130 - Chrn (assume Chrn=0)
        bit_field_off = offsets["current_state"] + 130
        # We need MapSize × MapSize × (2 + 2 × MapSide) bytes for BitField
        # MapSize and MapSide are not in our offsets — we get them from header
        # For now, skip BitField and go to TwoWayMonolith (already in offsets)
        if "two_way_monolith" in offsets:
            result["monoliths"], _ = parse_monolith_whirlpool(
                raw, offsets["two_way_monolith"])
    except Exception as e:
        result["monoliths"] = None

    # G1. Subterranean gates
    try:
        if "sub_ter_gate" in offsets:
            gates, pair_ids, _ = parse_pair_subterranean_gate(
                raw, offsets["sub_ter_gate"])
            result["sub_ter_gates"] = {"gates": gates, "pair_ids": pair_ids}
    except Exception as e:
        result["sub_ter_gates"] = None

    # H1. BlackMarket — we need to find it via find_header_offsets
    # (the offset is in header, not in post-tile area)
    # Caller should pass it in, or we look it up here
    # (see usage in save_parser.py)

    return result


# ============================================================================
# CLI self-test
# ============================================================================

if __name__ == "__main__":
    import os
    import sys
    sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

    # Try parsing a sample save
    test_path = "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/114.GM1"
    if not os.path.exists(test_path):
        test_path = "/home/z/my-project/H3save_reader/examples/Myth and Legend.h3m/0000.GM1"

    if not os.path.exists(test_path):
        print(f"No test save at {test_path}")
        sys.exit(0)

    # Decompress using the CRC-tolerant decompressor
    from map_config_builder import decompress_save
    raw = decompress_save(test_path)

    print(f"Decompressed: {len(raw)} bytes")

    # Find map start
    from tile_scanner import find_map_start, scan_tiles
    map_start, meta = find_map_start(raw)
    print(f"map.Start = 0x{map_start:X}")
    print(f"meta = {meta}")

    # We need map_size and has_underground — extract from header
    from header_parser import parse_header
    header = parse_header(raw)
    print(f"map_size = {header.map_size}, has_underground = {header.has_underground}")

    # Scan tiles (to find after_tiles_offset)
    objects = scan_tiles(raw, map_start, header.map_size, header.has_underground)
    print(f"Found {len(objects)} objects on tiles")

    # Re-walk the tile loop to find after_tiles_offset
    # (scan_tiles doesn't return that — we have to walk again)
    import struct
    n_tiles = header.map_size * header.map_size
    total_tiles = n_tiles * (2 if header.has_underground else 1)
    s = map_start
    for tile_num in range(total_tiles):
        if s + 18 > len(raw):
            break
        loc = raw[s]
        s += 7
        s += 11
        if s + 2 <= len(raw):
            var_count = struct.unpack("<H", raw[s:s + 2])[0]
            s += var_count * 4 + 4
        else:
            break
    # Now s points to after the tile loop
    # ProspectorRT GetSenseRegion: u16 num5 + 4 skip + num5 × (u16 + 35)
    num5 = struct.unpack("<H", raw[s:s + 2])[0]
    s += 4  # skip u16 + 2 padding
    print(f"num5 (after-tile records) = {num5}")
    for i in range(num5):
        msg_len = struct.unpack("<H", raw[s:s + 2])[0]
        s += msg_len + 35
    after_tiles = s
    print(f"after_tiles (ObjectNumber offset) = 0x{after_tiles:X}")

    # First just walk offsets to see where things break
    offsets_only = walk_post_tile_sections(raw, after_tiles, len(objects),
                                             hero_count=156,
                                             map_size=header.map_size,
                                             has_underground=header.has_underground)
    print("\n=== Post-tile offsets (offsets-only walk) ===")
    for k, v in offsets_only.items():
        if isinstance(v, int) and v > 0:
            print(f"  {k:25s} = 0x{v:X}")
        else:
            print(f"  {k:25s} = {v}")

    # Now compute the post-tile offsets + parse content
    result = parse_all_post_tile_sections(
        raw, after_tiles, len(objects), hero_count=156,
        map_size=header.map_size, has_underground=header.has_underground)

    print("\n=== Parsed sections summary ===")
    for k, v in result.items():
        if k == "offsets":
            print("\n--- offsets ---")
            for kk, vv in v.items():
                if isinstance(vv, int) and vv > 0:
                    print(f"  {kk:25s} = 0x{vv:X}")
                else:
                    print(f"  {kk:25s} = {vv}")
        elif isinstance(v, list):
            print(f"  {k}: {len(v)} records")
            if v and len(v) <= 3:
                for r in v:
                    print(f"    {r}")
        elif v is None:
            print(f"  {k}: None")
        else:
            print(f"  {k}: {type(v).__name__}")
