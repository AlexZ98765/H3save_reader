#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
localizer.py — Language-aware name lookup for HoMM3 save objects.

Reads the structured multilingual dictionary from
  02_format_docs/localization_dictionary.json
which is generated from the original editor's Data/*.txt files
(4 language variants: en, ru_sod, ru_wog, ru_complete).

Usage
-----
    from localizer import Localizer

    loc = Localizer()                        # default = ru_sod
    loc = Localizer(language='en')           # English
    loc = Localizer(language='ru_wog')       # WoG Russian

    loc.art(2)        # → 'Грааль' / 'The Grail' / ...
    loc.monstr(0)     # → 'Копейщик' / 'Pikeman' / ...
    loc.spell(11)     # → 'Ускорение' / 'Haste' / ...
    loc.skill(3)      # → 'Разведка' / 'Scouting' / ...
    loc.town(0)       # → 'Замок' / 'Castle' / ...
    loc.color(0)      # → 'Красный' / 'Red' / ...
    loc.hero_class(0) # → 'Рыцарь' / 'Knight' / ...

    loc.section('Art', 42)              # generic lookup by section name + index
    loc.section_list('Spell')           # full list for a section

Languages
---------
  - 'en'           English (SoD)
  - 'ru_sod'       Русский (SoD)         — DEFAULT
  - 'ru_wog'       Русский (WoG)
  - 'ru_complete'  Русский (Complete)

Alias 'ru' is accepted and resolves to 'ru_sod'.
Missing strings for a language fall back to ru_sod, then to the raw integer ID.
"""
from __future__ import annotations
import json
import os
from typing import List, Optional


# Path to the multilingual dictionary
_HERE = os.path.dirname(os.path.abspath(__file__))
DICT_PATH = os.path.join(_HERE, "..", "02_format_docs", "localization_dictionary.json")

# Alias map (short → canonical key)
_LANGUAGE_ALIASES = {
    'en': 'en',
    'ru': 'ru_sod',
    'ru_sod': 'ru_sod',
    'ru_wog': 'ru_wog',
    'ru_complete': 'ru_complete',
}

# Friendly display names (used in UI dropdown)
LANGUAGE_DISPLAY_NAMES = {
    'en':          'English (SoD)',
    'ru_sod':      'Русский (SoD)',
    'ru_wog':      'Русский (WoG)',
    'ru_complete': 'Русский (Complete)',
}

DEFAULT_LANGUAGE = 'ru_sod'
FALLBACK_LANGUAGE = 'ru_sod'

# Canonical set of section names available in the dictionary
_ALL_SECTIONS = [
    'Art', 'Monstr', 'Spell', 'Skill', 'Dwelling', 'Object', 'ArtClass',
    'Mine', 'Locality', 'LevelSkill', 'FullLevelSkill', 'Color', 'Reply',
    'Town', 'Doll', 'Place', 'Tent', 'Status', 'Quest', 'Reward',
    'HeroClass', 'Ideology',
]


def available_languages() -> List[str]:
    """Return list of canonical language keys (e.g. ['en', 'ru_sod', 'ru_wog', 'ru_complete'])."""
    return list(LANGUAGE_DISPLAY_NAMES.keys())


def language_display_name(lang: str) -> str:
    """Return human-readable name (e.g. 'ru_sod' → 'Русский (SoD)')."""
    return LANGUAGE_DISPLAY_NAMES.get(lang, lang)


def normalize_language(lang: str) -> str:
    """Resolve user-supplied language (e.g. 'ru') to canonical key (e.g. 'ru_sod')."""
    if lang is None:
        return DEFAULT_LANGUAGE
    return _LANGUAGE_ALIASES.get(lang, lang)


class Localizer:
    """Language-aware lookup of game object names by integer ID."""

    def __init__(self, language: Optional[str] = None,
                 dict_path: Optional[str] = None):
        self.language = normalize_language(language)
        self.dict_path = dict_path or DICT_PATH
        with open(self.dict_path, 'r', encoding='utf-8') as f:
            self._data = json.load(f)
        self._sections = self._data.get('sections', {})

    # ---------- public API ----------

    @staticmethod
    def available_languages() -> List[str]:
        return list(LANGUAGE_DISPLAY_NAMES.keys())

    @staticmethod
    def language_display_name(lang: str) -> str:
        return LANGUAGE_DISPLAY_NAMES.get(lang, lang)

    def set_language(self, language: str) -> None:
        self.language = normalize_language(language)

    def section(self, section_name: str, idx: int) -> str:
        """Generic lookup by section name + integer ID.

        Returns "" if section does not exist.
        Returns "" if index is out of range.
        Falls back to FALLBACK_LANGUAGE (ru_sod) when the entry is empty for
        the active language.
        """
        section = self._sections.get(section_name)
        if not section:
            return ''
        langs = section.get('languages', {})
        arr = langs.get(self.language, [])
        if 0 <= idx < len(arr) and arr[idx]:
            return arr[idx]
        # Fallback to default Russian
        fb_arr = langs.get(FALLBACK_LANGUAGE, [])
        if 0 <= idx < len(fb_arr) and fb_arr[idx]:
            return fb_arr[idx]
        return ''

    def section_list(self, section_name: str) -> List[str]:
        """Return the entire list of names for a section (active language).

        Missing entries are filled with fallback language. Out-of-range
        entries are returned as empty strings.
        """
        section = self._sections.get(section_name)
        if not section:
            return []
        langs = section.get('languages', {})
        arr = langs.get(self.language, [])
        fb_arr = langs.get(FALLBACK_LANGUAGE, [])
        out = []
        n = max(len(arr), len(fb_arr))
        for i in range(n):
            v = arr[i] if i < len(arr) else ''
            if not v:
                v = fb_arr[i] if i < len(fb_arr) else ''
            out.append(v)
        return out

    # ---------- convenience wrappers ----------

    def art(self, idx: int) -> str:
        """Artifact name by ID (0..140)."""
        return self.section('Art', idx)

    def monstr(self, idx: int) -> str:
        """Monster name by ID (0..144)."""
        return self.section('Monstr', idx)

    def spell(self, idx: int) -> str:
        """Spell name by ID (0..70)."""
        return self.section('Spell', idx)

    def skill(self, idx: int) -> str:
        """Secondary skill name by ID (0..27)."""
        return self.section('Skill', idx)

    def dwelling(self, idx: int) -> str:
        """Dwelling name by ID (0..36)."""
        return self.section('Dwelling', idx)

    def object(self, idx: int) -> str:
        """Map object name by ID (0..56)."""
        return self.section('Object', idx)

    def art_class(self, idx: int) -> str:
        """Artifact class (Machine/Treasure/Minor/Major/Relic/Special)."""
        return self.section('ArtClass', idx)

    def mine(self, idx: int) -> str:
        """Mine resource name (Wood/Mercury/Ore/Sulfur/Crystal/Gems/Gold)."""
        return self.section('Mine', idx)

    def locality(self, idx: int) -> str:
        """Surface/Underground."""
        return self.section('Locality', idx)

    def level_skill(self, idx: int) -> str:
        """Short skill level: B./A./E. (or 1./2./3. for ru_complete)."""
        return self.section('LevelSkill', idx)

    def full_level_skill(self, idx: int) -> str:
        """Full skill level name: Basic/Advanced/Expert."""
        return self.section('FullLevelSkill', idx)

    def color(self, idx: int) -> str:
        """Player color name by ID (0..7). Index 255 = Neutral (handled specially)."""
        if idx == 255:
            # Localized neutral names
            return {
                'en':          'Neutral',
                'ru_sod':      'Нейтральный',
                'ru_wog':      'Нейтральный',
                'ru_complete': 'Нейтральный',
            }.get(self.language, 'Neutral')
        return self.section('Color', idx)

    def mine(self, idx: int) -> str:
        """Mine resource name (Wood/Mercury/Ore/Sulfur/Crystal/Gems/Gold).

        Index 6 is "Gold" — handled as a special fallback because the source
        data files only have 6 entries (Wood, Mercury, Ore, Sulfur, Crystal, Gems)
        and Gold is conventionally index 6 (the 7th element).
        """
        if idx == 6:
            return {
                'en':          'Gold',
                'ru_sod':      'Золото',
                'ru_wog':      'Золото',
                'ru_complete': 'Золото',
            }.get(self.language, 'Gold')
        return self.section('Mine', idx)

    def reply(self, idx: int) -> str:
        """Yes/No."""
        return self.section('Reply', idx)

    def town(self, idx: int) -> str:
        """Town/faction name by ID (0..8)."""
        return self.section('Town', idx)

    def doll(self, idx: int) -> str:
        """Hero doll slot (Head/Shoulders/Neck/...)."""
        return self.section('Doll', idx)

    def place(self, idx: int) -> str:
        """Hero place (Prison/Tavern/Disable/Town/Map)."""
        return self.section('Place', idx)

    def tent(self, idx: int) -> str:
        """Keymaster tent color name."""
        return self.section('Tent', idx)

    def status(self, idx: int) -> str:
        """Build status (No/Timer/Available)."""
        return self.section('Status', idx)

    def quest(self, idx: int) -> str:
        """Seer hut mission type."""
        return self.section('Quest', idx)

    def reward(self, idx: int) -> str:
        """Seer hut reward type."""
        return self.section('Reward', idx)

    def hero_class(self, idx: int) -> str:
        """Hero class name (Knight/Cleric/Ranger/...)."""
        return self.section('HeroClass', idx)

    def ideology(self, idx: int) -> str:
        """Good/Evil/Neutral."""
        return self.section('Ideology', idx)

    # ---------- backward compatibility helpers ----------

    def resolve_resource(self, idx: int) -> str:
        """Map our internal A_RESOURCE index (0..5) to Mine[] index (0..5).

        Internal order:  Wood, Mercury, Ore, Sulfur, Crystal, Gems
        Same as Mine[] order in the data files, so direct lookup works.
        """
        return self.section('Mine', idx)


# ============================================================================
# Singleton accessor for convenience
# ============================================================================

_DEFAULT_LOC: Optional[Localizer] = None


def get_localizer(language: Optional[str] = None) -> Localizer:
    """Get a process-wide Localizer. Created lazily on first call.

    If `language` is provided, replaces the singleton's language setting.
    """
    global _DEFAULT_LOC
    if _DEFAULT_LOC is None:
        _DEFAULT_LOC = Localizer(language)
    elif language is not None:
        _DEFAULT_LOC.set_language(language)
    return _DEFAULT_LOC


# ============================================================================
# Self-test
# ============================================================================

if __name__ == '__main__':
    import sys

    print('Available languages:')
    for lang in Localizer.available_languages():
        print(f'  {lang:15s} → {Localizer.language_display_name(lang)}')
    print()

    for lang in ['en', 'ru_sod', 'ru_wog', 'ru_complete']:
        loc = Localizer(lang)
        print(f'\n=== {lang} ({loc.language_display_name(lang)}) ===')
        # Test sample lookups
        print(f'  Art[2]      = {loc.art(2)!r}')
        print(f'  Monstr[0]   = {loc.monstr(0)!r}')
        print(f'  Spell[11]   = {loc.spell(11)!r}')
        print(f'  Skill[3]    = {loc.skill(3)!r}')
        print(f'  Town[0]     = {loc.town(0)!r}')
        print(f'  Color[0]    = {loc.color(0)!r}')
        print(f'  HeroClass[0]= {loc.hero_class(0)!r}')

    print('\n=== Out-of-range / missing test ===')
    loc = Localizer('en')
    print(f'  Art[9999]   = {loc.art(9999)!r}  (should be empty string)')
    print(f'  Art[-1]     = {loc.art(-1)!r}    (should be empty string)')
