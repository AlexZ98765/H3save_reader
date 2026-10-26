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

# Object code → positional index in PRT's CreateTblObject table.
# Loaded from 02_format_docs/object_code_order.json (extracted from MainForm.cs).
# This is structural data (which code goes in which row of TblObject), NOT
# hardcoded names. The actual names come from .txt [Object] section, which
# PRT's ReplaceName() applies by position. For code 16 (banks), 7 subtypes
# exist; subtype selects which of the 7 positions (6..12) is used.
_OBJECT_CODE_ORDER_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    '..', '02_format_docs', 'object_code_order.json'
)


def _load_object_code_order():
    """Load the positional code order from JSON. Returns list of dicts:
    [{'position': 0, 'code': 2, 'subtype': None, 'default_name': '...'}, ...]
    """
    try:
        with open(_OBJECT_CODE_ORDER_PATH, 'r', encoding='utf-8') as f:
            data = json.load(f)
        return data.get('entries', [])
    except Exception:
        return []


_OBJECT_CODE_ORDER_ENTRIES = _load_object_code_order()
# Build lookup: (code, subtype) → position
# For codes without subtype, subtype=None is the key.
_OBJECT_POSITION_BY_CODE = {}
for _entry in _OBJECT_CODE_ORDER_ENTRIES:
    _key = (_entry['code'], _entry.get('subtype'))
    _OBJECT_POSITION_BY_CODE.setdefault(_key, _entry['position'])


# Spell levels (structural attribute, same across all languages).
# Loaded from 02_format_docs/spell_levels.json (extracted from MainForm.cs::CreateTblSpell).
_SPELL_LEVELS_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    '..', '02_format_docs', 'spell_levels.json'
)


def _load_spell_levels():
    """Load spell levels from JSON. Returns dict {code: level}."""
    try:
        with open(_SPELL_LEVELS_PATH, 'r', encoding='utf-8') as f:
            data = json.load(f)
        return {e['code']: e['level'] for e in data.get('entries', [])}
    except Exception:
        return {}


_SPELL_LEVELS = _load_spell_levels()


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

        Uses DICT access on the language dict (keyed by string IDs):
            langs[language][str(idx)]

        Returns "" if section does not exist or ID is missing.
        Falls back to FALLBACK_LANGUAGE (ru_sod) when the entry is empty for
        the active language.
        """
        section = self._sections.get(section_name)
        if not section:
            return ''
        langs = section.get('languages', {})
        lang_dict = langs.get(self.language, {})
        if not isinstance(lang_dict, dict):
            # Backward-compat: legacy list-format dict
            arr = lang_dict
            if 0 <= idx < len(arr) and arr[idx]:
                return arr[idx]
        else:
            v = lang_dict.get(str(idx))
            if v:
                return v
        # Fallback to default Russian
        fb = langs.get(FALLBACK_LANGUAGE, {})
        if isinstance(fb, dict):
            v = fb.get(str(idx))
            if v:
                return v
        else:
            if 0 <= idx < len(fb) and fb[idx]:
                return fb[idx]
        return ''

    def section_list(self, section_name: str) -> List[str]:
        """Return the entire list of names for a section (active language).

        Iterates IDs 0..max_id (across all languages) in order. For each ID,
        returns the active-language value, falling back to FALLBACK_LANGUAGE
        when missing.

        Missing entries are returned as empty strings.
        """
        section = self._sections.get(section_name)
        if not section:
            return []
        langs = section.get('languages', {})
        # Compute max ID across all languages (handles sparse dicts too)
        max_id = -1
        for v in langs.values():
            if isinstance(v, dict):
                for k in v.keys():
                    try:
                        ki = int(k)
                        if ki > max_id:
                            max_id = ki
                    except ValueError:
                        pass
            elif isinstance(v, list):
                if len(v) - 1 > max_id:
                    max_id = len(v) - 1
        if max_id < 0:
            return []
        active = langs.get(self.language, {})
        fb = langs.get(FALLBACK_LANGUAGE, {})
        out = []
        for i in range(max_id + 1):
            v = ''
            if isinstance(active, dict):
                v = active.get(str(i), '')
            elif isinstance(active, list) and 0 <= i < len(active):
                v = active[i] or ''
            if not v:
                if isinstance(fb, dict):
                    v = fb.get(str(i), '')
                elif isinstance(fb, list) and 0 <= i < len(fb):
                    v = fb[i] or ''
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

    def spell_level(self, idx: int) -> int:
        """Spell level (1..5) by spell ID. Returns 0 for unknown spells.

        Spell levels are structural attributes (not localized names), extracted
        from CreateTblSpell in MainForm.cs. They are the same across all
        languages — only the spell names change via .txt ReplaceName.
        """
        return _SPELL_LEVELS.get(idx, 0)

    def skill(self, idx: int) -> str:
        """Secondary skill name by ID (0..27)."""
        return self.section('Skill', idx)

    def dwelling(self, idx: int) -> str:
        """Dwelling name by ID (0..36)."""
        return self.section('Dwelling', idx)

    def object(self, code: int, subtype: Optional[int] = None) -> str:
        """Map object code → localized name.

        PRT's CreateTblObject adds 57 rows to TblObject in a fixed positional
        order (extracted into 02_format_docs/object_code_order.json). The .txt
        [Object] section has 57 names that ReplaceName() applies by POSITION,
        not by code. So to get the name for code X, we find X's positional
        index in CreateTblObject and return .txt [Object][position].

        For code 16 (banks), 7 subtypes exist (0..6). Pass subtype to select
        the right position. If subtype is None for code 16, position 0
        (Склады циклопов / Cyclops Warehouse) is returned.

        Args:
            code: object code as stored in save file (e.g. 101 for chest)
            subtype: bank subtype (0..6) for code 16; None otherwise

        Returns:
            Localized name from .txt [Object] section, or '' if not found.
        """
        # Find positional index for this (code, subtype)
        # First try exact match with subtype
        position = _OBJECT_POSITION_BY_CODE.get((code, subtype))
        if position is None and subtype is not None:
            # Fallback: try without subtype (for code 16 with unknown subtype)
            position = _OBJECT_POSITION_BY_CODE.get((code, None))
        if position is None:
            # Code not in CreateTblObject — try direct .txt lookup
            # (works for codes 0..56 if .txt has them indexed by code)
            return self.section('Object', code)
        # Get name from .txt [Object] section at this position
        return self.section('Object', position)

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
