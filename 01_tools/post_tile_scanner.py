#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
post_tile_scanner.py — Parse post-tile sections of .GM1 saves.

Implements the forward structure-walking chain from tile end to hero section,
using the EXACT ProspectorRT scan functions (translated to Python).

The chain (from ProspectorRT GetSenseRegion + Scanner):
  After tiles: u16 num5, then num5 × (u16 + 35) records
  ObjectNumber = u16 at after_num5
  EventBox = after_num5 + ObjectNumber × 5 + 4
  ArtRes = ScanObjectContent(EventBox, ScanEventBoxContent)
  Monstr = ScanObjectContent(ArtRes, ScanArtResContent)
  SeerHut = ScanMonstrContent(Monstr)
  PassGuard = ScanObjectContent(SeerHut, ScanSeerHutContent)
  MapTimedEvent = ScanObjectContent(PassGuard, ScanPassGuardContent)
  TownsTimedEvent = ScanMapTimedEvents(MapTimedEvent)
  BottleSign = ScanTownsTimedEvents(TownsTimedEvent)
  Mine = ScanBottleSignContent(BottleSign)
  Dwelling = Mine + count × 62 + 1
  Garrison = Dwelling + count × 75 + 2
  UnknownVarReg = Garrison + count × 61 + 1
  UnknownFixedReg = UnknownVarReg + count × 28 + 1
  Color = UnknownFixedReg + 49
  Town = Color + 1160
  Hero = ScanTownsContent(Town) + 25 (fixed gap)
  HeroState = ScanHeroesContent(Hero)
  CurrentState = HeroState + HeroCount × 2

The 25-byte gap between towns and heroes is a FIXED constant
(verified on both day-0 and post-action saves of Myth and Legend).
"""

from __future__ import annotations
import struct
from typing import Any, Dict, List, Optional, Tuple


def _u16(raw: bytes, s: int) -> int:
    return (raw[s + 1] << 8) + raw[s]


# ============================================================================
# Scan functions (exact ProspectorRT translation)
# ============================================================================

def scan_event_box_content(raw, s):
    """ScanEventBoxContent — skip one event box record."""
    if raw[s] > 0:
        s = s + _u16(raw, s + 1) + 3
    s = (s + 57) if raw[s] == 1 else (s + 1)
    s += 42
    s = (s + raw[s] * 2 + 1) if raw[s] > 0 else (s + 1)
    s = (s + raw[s] + 1) if raw[s] > 0 else (s + 1)
    s = (s + raw[s] + 1) if raw[s] > 0 else (s + 1)
    s = (s + raw[s] * 4 + 1) if raw[s] > 0 else (s + 1)
    return s - 1


def scan_art_res_content(raw, s):
    """ScanArtResContent — skip one art/resource record."""
    s = s + _u16(raw, s) + 2
    s = (s + 57) if raw[s] == 1 else (s + 1)
    return s - 1


def scan_monstr_content(raw, s):
    """ScanMonstrContent — skip entire monster section."""
    num = _u16(raw, s)
    if num > 0:
        s += 2
        for i in range(num):
            s += _u16(raw, s) + 31
    else:
        s += 2
    return s


def scan_seer_hut_content(raw, s):
    """ScanSeerHutContent — skip one seer hut record (quest + reward)."""
    t = raw[s]
    if t == 0:
        return s + 15  # type 0 = empty
    elif t == 1: s += 5
    elif t == 2: s += 7
    elif t == 3: s += 6
    elif t == 4: s += 10
    elif t == 5: s = s + raw[s + 1] * 2 + 4
    elif t == 6: s = s + raw[s + 1] * 6 + 4
    elif t == 7: s += 31
    elif t == 8: s += 5
    elif t == 9: s += 4
    else: s += 1
    # Deadline (4 bytes)
    s += 4
    # 3 variable sections
    for i in range(3):
        if _u16(raw, s) != 0:
            s = s + _u16(raw, s) + 4
        else:
            s += 4
    # Reward (15 bytes)
    s += 15
    return s - 1


def scan_pass_guard_content(raw, s):
    """ScanPassGuardContent — same structure as SeerHut."""
    return scan_seer_hut_content(raw, s)


def scan_object_content(raw, s, scan_func):
    """ScanObjectContent — generic: u16 count + iterate scan_func."""
    num = _u16(raw, s)
    if num > 0:
        s += 2
        for i in range(num):
            s = scan_func(raw, s) + 1
    else:
        s += 2
    return s


def scan_map_timed_events(raw, s):
    """ScanMapTimedEvents — u16 count + 4 skip + count × (u16 + 37)."""
    num = _u16(raw, s)
    if num > 0:
        s += 4
        for i in range(num):
            s += _u16(raw, s) + 37
    else:
        s += 4
    return s


def scan_town_timed_events(raw, s):
    """ScanTownsTimedEvents — u16 count + 4 skip + count × (u16 + 60)."""
    num = _u16(raw, s)
    if num > 0:
        s += 4
        for i in range(num):
            s += _u16(raw, s) + 60
    else:
        s += 4
    return s


def scan_bottle_sign_content(raw, s):
    """ScanBottleSignContent — u8 count + variable records."""
    if raw[s] != 0:
        num = raw[s]; s += 1
        for i in range(num):
            n = _u16(raw, s)
            s = (s + 3) if n <= 0 else (s + n + 3)
    else:
        s += 1
    return s


def scan_towns_content(raw, s):
    """ScanTownsContent — u8 count + count × (name_len + 382).
    
    Uses PRT offset +70 for name_len (s = block_offset - 1 after u8 skip,
    so +70 = block_offset + 69 in 0-based indexing).
    """
    num = raw[s]; s += 1
    for i in range(num):
        s += raw[s + 70] + 382
    return s


def scan_heroes_content(raw, s, hero_count=156):
    """ScanHeroesContent — count × (extra_size + 1094)."""
    for i in range(hero_count):
        s += _u16(raw, s + 22) + 1094
    return s


# ============================================================================
# Main: walk_post_tile_sections
# ============================================================================

TOWN_HERO_GAP = 26  # Fixed constant (u8 skip: 26, u16 skip: 25)


def walk_post_tile_sections(raw: bytes, after_tiles_offset: int,
                             object_number: int, hero_count: int = 156
                             ) -> Dict[str, int]:
    """
    Walk post-tile sections forward from after tile data.

    Args:
        raw:                decompressed save bytes
        after_tiles_offset:  offset after tile loop + num5 records
        object_number:      number of objects on map (from tile scan)
        hero_count:         number of heroes (156 for SoD)

    Returns dict with all section offsets.
    """
    s = after_tiles_offset

    # ObjectNumber (u16)
    obj_num = _u16(raw, s)
    # EventBox = s + ObjectNumber * 5 + 4
    event_box = s + obj_num * 5 + 4

    # ArtRes = ScanObjectContent(EventBox, ScanEventBoxContent)
    art_res = scan_object_content(raw, event_box, scan_event_box_content)

    # Monstr = ScanObjectContent(ArtRes, ScanArtResContent)
    monstr = scan_object_content(raw, art_res, scan_art_res_content)

    # SeerHut = ScanMonstrContent(Monstr)
    seer_hut = scan_monstr_content(raw, monstr)

    # PassGuard = ScanObjectContent(SeerHut, ScanSeerHutContent)
    pass_guard = scan_object_content(raw, seer_hut, scan_seer_hut_content)

    # MapTimedEvent = ScanObjectContent(PassGuard, ScanPassGuardContent)
    map_timed = scan_object_content(raw, pass_guard, scan_pass_guard_content)

    # TownsTimedEvent = ScanMapTimedEvents(MapTimedEvent)
    towns_timed = scan_map_timed_events(raw, map_timed)

    # BottleSign = ScanTownsTimedEvents(TownsTimedEvent)
    bottle_sign = scan_town_timed_events(raw, towns_timed)

    # Mine = ScanBottleSignContent(BottleSign)
    mine = scan_bottle_sign_content(raw, bottle_sign)

    # Dwelling = Mine + count × 62 + 1
    dwelling = mine + raw[mine] * 62 + 1

    # Garrison = Dwelling + count × 75 + 2
    garrison = dwelling + raw[dwelling] * 75 + 2

    # UnknownVarReg = Garrison + count × 61 + 1
    unknown_var_reg = garrison + raw[garrison] * 61 + 1

    # UnknownFixedReg = UnknownVarReg + count × 28 + 1
    unknown_fixed_reg = unknown_var_reg + raw[unknown_var_reg] * 28 + 1

    # Color = UnknownFixedReg + 49
    color = unknown_fixed_reg + 49

    # Town = Color + 1160
    town = color + 1160

    # Hero = ScanTownsContent(Town) + TOWN_HERO_GAP
    hero = scan_towns_content(raw, town) + TOWN_HERO_GAP

    # HeroState = ScanHeroesContent(Hero)
    hero_state = scan_heroes_content(raw, hero, hero_count)

    # CurrentState = HeroState + HeroCount × 2
    current_state = hero_state + hero_count * 2

    return {
        "event_box": event_box,
        "art_res": art_res,
        "monstr": monstr,
        "seer_hut": seer_hut,
        "pass_guard": pass_guard,
        "map_timed_event": map_timed,
        "towns_timed_event": towns_timed,
        "bottle_sign": bottle_sign,
        "mine": mine,
        "dwelling": dwelling,
        "garrison": garrison,
        "unknown_var_reg": unknown_var_reg,
        "unknown_fixed_reg": unknown_fixed_reg,
        "color": color,
        "town": town,
        "hero": hero,
        "hero_state": hero_state,
        "current_state": current_state,
        "object_number": obj_num,
        "town_count": raw[town],
        "mine_count": raw[mine],
        "dwelling_count": raw[dwelling],
        "garrison_count": raw[garrison],
    }
