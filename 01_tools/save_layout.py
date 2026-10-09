#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
save_layout.py — Dataclasses describing the layout of a decompressed
HoMM3 `.GM1` save and the per-map "anchor table" (config) that lets us
parse any save of a given map without hardcoding absolute offsets.

Three-phase architecture (see README.md):

  Phase 1: Map JSON  -> MapData           (see map_json_loader.py)
  Phase 2: Day-0 save + MapData -> MapConfig  (see map_config_builder.py)
  Phase 3: Any save + MapConfig -> ParsedSave  (see save_parser.py, planned)

This module only defines the data shapes — no parsing logic.
"""

from __future__ import annotations
from dataclasses import dataclass, field, asdict
from typing import Dict, List, Optional, Tuple, Any
import json


# ============================================================================
# Phase 2 output: HeaderInfo, ObjectCluster, HeroSection, TownSection, MapConfig
# ============================================================================

@dataclass
class HeaderInfo:
    """Parsed header of a decompressed .GM1 save.

    Field layout (verified against `02_format_docs/header_pointer_search.md`):

      0x00..0x04  magic           5 bytes ASCII  "H3SVG" or "H3SVC"
      0x05..0x07  padding         3 bytes
      0x08..0x0B  version_major   u32 LE        (e.g. 0x2A = 42 = SoD/HotA)
      0x0C..0x0F  version_minor   u32 LE        (e.g. 2)
      0x10..0x2F  game_state      32 bytes      (day/week/month/difficulty)
      0x30..0x33  map_type        u32 LE        (28 = SoD)
      0x34        has_underground u8
      0x35..0x38  map_size        u32 LE
      0x39        is_playable     u8
      0x3A..0x3B  name_length     u16 LE
      0x3C..      map_name        name_length bytes, cp1251
      ..          description_len u16 LE
      ..          description     cp1251
      ..          player setup, map filename, save filename, ...

    The actual end of the header (where game-state sections begin) is
    exposed via `header_size` so other parsers can start scanning from
    that offset.
    """

    magic: str
    version_major: int
    version_minor: int
    map_type: int
    has_underground: bool
    map_size: int
    is_playable: bool
    map_name: str
    description: str
    map_filename: str          # e.g. "Myth and Legend.h3m" — empty if not found
    save_filename: str         # e.g. "0000.GM1" — empty if not found
    header_size: int           # byte offset where game-state sections begin

    # Additional raw fields for diagnostics
    game_state_32: bytes = b""  # bytes 0x10..0x2F
    raw_size: int = 0          # decompressed save size

    def to_dict(self) -> dict:
        d = asdict(self)
        d["game_state_32"] = self.game_state_32.hex(" ")
        return d


@dataclass
class ObjectCluster:
    """A dense region of 3-byte coord_int matches in the save.

    Each cluster represents one "section" of the save that stores per-object
    data (one record per object). Different clusters store different kinds
    of state (main, visiting, fog-of-war, alive, treasure, decoration).
    """

    name: str                  # "main", "visiting", "fog", "alive",
                               # "treasure", "decoration", "other_N"
    start: int                 # byte offset (inclusive)
    end: int                   # byte offset (inclusive; last hit)
    hits: int                  # total coord_int matches in [start, end]
    distinct_coords: int = 0   # how many distinct coord_ints hit (coverage)
    stride_min: int = 0        # min gap between consecutive hits (heuristic)
    stride_median: int = 0     # median gap
    peak_density: int = 0      # max hits in any 4KB window inside the cluster

    @property
    def start_hex(self) -> str:
        return f"0x{self.start:X}"

    @property
    def end_hex(self) -> str:
        return f"0x{self.end:X}"

    def to_dict(self) -> dict:
        return {
            "name":             self.name,
            "start":            self.start,
            "start_hex":        self.start_hex,
            "end":              self.end,
            "end_hex":          self.end_hex,
            "hits":             self.hits,
            "distinct_coords":  self.distinct_coords,
            "stride_min":       self.stride_min,
            "stride_median":    self.stride_median,
            "peak_density":     self.peak_density,
        }


@dataclass
class HeroBlockInfo:
    """One hero block found in the save."""

    block_offset: int          # byte offset of faction byte (block start)
    name_offset: int          # byte offset of name (block_offset + 169)
    name: str
    player: int               # 0-7 or 255 (neutral)
    level: int
    x: int
    y: int
    z: int
    is_active: bool           # player <= 7

    @property
    def block_offset_hex(self) -> str:
        return f"0x{self.block_offset:X}"


@dataclass
class HeroSection:
    """All hero blocks found in the save."""

    stride: int = 0
    blocks: List[HeroBlockInfo] = field(default_factory=list)

    @property
    def count(self) -> int:
        return len(self.blocks)

    @property
    def start(self) -> Optional[int]:
        return self.blocks[0].block_offset if self.blocks else None

    @property
    def start_hex(self) -> Optional[str]:
        return f"0x{self.start:X}" if self.start is not None else None

    @property
    def stride_hex(self) -> str:
        return f"0x{self.stride:X}"

    def to_dict(self) -> dict:
        return {
            "start":         self.start,
            "start_hex":     self.start_hex,
            "stride":        self.stride,
            "stride_hex":    self.stride_hex,
            "count":         self.count,
            "all_offsets":   [b.block_offset_hex for b in self.blocks[:50]],
            "blocks":        [
                {
                    "block_offset":  b.block_offset,
                    "block_offset_hex": b.block_offset_hex,
                    "name":          b.name,
                    "player":        b.player,
                    "level":         b.level,
                    "x": b.x, "y": b.y, "z": b.z,
                    "is_active":     b.is_active,
                }
                for b in self.blocks
            ],
        }


@dataclass
class TownBlockInfo:
    """One town block found in the save."""

    block_offset: int          # faction byte (block start)
    name_offset: int
    name: str
    faction: int               # 0-7 or 255
    town_type: int             # 0-8
    x: int
    y: int
    z: int

    @property
    def block_offset_hex(self) -> str:
        return f"0x{self.block_offset:X}"


@dataclass
class TownSection:
    """All town blocks found in the save."""

    blocks: List[TownBlockInfo] = field(default_factory=list)

    @property
    def count(self) -> int:
        return len(self.blocks)

    @property
    def offsets(self) -> List[str]:
        return [b.block_offset_hex for b in self.blocks]

    def to_dict(self) -> dict:
        return {
            "count":    self.count,
            "offsets":  self.offsets,
            "blocks":   [
                {
                    "block_offset":       b.block_offset,
                    "block_offset_hex":    b.block_offset_hex,
                    "name":               b.name,
                    "faction":           b.faction,
                    "type":              b.town_type,
                    "x": b.x, "y": b.y, "z": b.z,
                }
                for b in self.blocks
            ],
        }


@dataclass
class ObjectOffsets:
    """Per-object offsets in the save (computed from clusters)."""

    main_offset:       Optional[int] = None
    visiting_offset:    Optional[int] = None
    fog_offset:         Optional[int] = None
    alive_offset:       Optional[int] = None
    treasure_offset:    Optional[int] = None
    decoration_offset:  Optional[int] = None
    save_offsets:       List[int] = field(default_factory=list)  # all hits

    def to_dict(self) -> dict:
        def h(v: Optional[int]) -> Optional[str]:
            return f"0x{v:X}" if v is not None else None
        return {
            "main_offset":       h(self.main_offset),
            "visiting_offset":    h(self.visiting_offset),
            "fog_offset":         h(self.fog_offset),
            "alive_offset":       h(self.alive_offset),
            "treasure_offset":    h(self.treasure_offset),
            "decoration_offset":  h(self.decoration_offset),
            "save_offsets":       [f"0x{o:X}" for o in self.save_offsets],
        }


@dataclass
class MapConfig:
    """Per-map anchor table produced by Phase 2.

    Lets us parse ANY save of the same map (Phase 3) without
    hardcoding absolute offsets — they're recomputed from the
    day-0 save.
    """

    meta: Dict[str, Any] = field(default_factory=dict)
    clusters: List[ObjectCluster] = field(default_factory=list)
    hero_section: HeroSection = field(default_factory=HeroSection)
    town_section: TownSection = field(default_factory=TownSection)
    object_offsets: Dict[int, ObjectOffsets] = field(default_factory=dict)
    # Per-field relative offsets inside a block (format constants, not
    # map-specific — populated from 02_format_docs/gm1_mapping.json)
    field_offsets: Dict[str, Dict[str, Any]] = field(default_factory=dict)

    def to_dict(self) -> dict:
        return {
            "_meta":          self.meta,
            "clusters":       [c.to_dict() for c in self.clusters],
            "hero_section":   self.hero_section.to_dict(),
            "town_section":   self.town_section.to_dict(),
            "object_offsets": {
                str(ci): off.to_dict()
                for ci, off in self.object_offsets.items()
            },
            "field_offsets":  self.field_offsets,
        }

    def save(self, path: str) -> None:
        with open(path, "w", encoding="utf-8") as f:
            json.dump(self.to_dict(), f, ensure_ascii=False, indent=2)

    @classmethod
    def from_dict(cls, d: dict) -> "MapConfig":
        mc = cls()
        mc.meta = d.get("_meta", {})
        mc.clusters = [
            ObjectCluster(
                name=c["name"],
                start=c["start"],
                end=c["end"],
                hits=c["hits"],
                distinct_coords=c.get("distinct_coords", 0),
                stride_min=c.get("stride_min", 0),
                stride_median=c.get("stride_median", 0),
                peak_density=c.get("peak_density", 0),
            )
            for c in d.get("clusters", [])
        ]
        # Hero section
        hs = d.get("hero_section", {})
        mc.hero_section = HeroSection(
            stride=int(hs.get("stride", 0) or 0),
            blocks=[
                HeroBlockInfo(
                    block_offset=b["block_offset"],
                    name_offset=b["block_offset"] + 169,
                    name=b["name"],
                    player=b["player"],
                    level=b["level"],
                    x=b["x"], y=b["y"], z=b["z"],
                    is_active=b["is_active"],
                )
                for b in hs.get("blocks", [])
            ],
        )
        # Town section
        ts = d.get("town_section", {})
        mc.town_section = TownSection(
            blocks=[
                TownBlockInfo(
                    block_offset=b["block_offset"],
                    name_offset=b["block_offset"] + 71,
                    name=b["name"],
                    faction=b["faction"],
                    town_type=b["type"],
                    x=b["x"], y=b["y"], z=b["z"],
                )
                for b in ts.get("blocks", [])
            ],
        )
        # Object offsets
        mc.object_offsets = {
            int(ci): ObjectOffsets(
                main_offset=oo.get("main_offset_int"),
                visiting_offset=oo.get("visiting_offset_int"),
                fog_offset=oo.get("fog_offset_int"),
                alive_offset=oo.get("alive_offset_int"),
                treasure_offset=oo.get("treasure_offset_int"),
                decoration_offset=oo.get("decoration_offset_int"),
                save_offsets=oo.get("save_offsets_int", []),
            )
            for ci, oo in d.get("object_offsets", {}).items()
        }
        # Field offsets
        mc.field_offsets = d.get("field_offsets", {})
        return mc


# ============================================================================
# Phase 3 output (planned): ParsedSave
# ============================================================================

@dataclass
class ParsedField:
    """One parsed field from a save block."""

    name: str
    offset: int
    size: int
    type: str           # "u8", "u16", "u32", "ascii", "bytes", "bitmask"
    value: Any
    raw_bytes: bytes
    description: str = ""

    @property
    def offset_hex(self) -> str:
        return f"0x{self.offset:X}"


@dataclass
class ParsedBlock:
    """One block of the save with all parsed fields."""

    name: str
    start: int
    end: int
    fields: List[ParsedField] = field(default_factory=list)
    description: str = ""


@dataclass
class ParsedSave:
    """Phase 3 output: fully parsed save.

    Built from a MapConfig + a decompressed save.
    """

    header: HeaderInfo
    blocks: List[ParsedBlock] = field(default_factory=list)
    heroes: List[dict] = field(default_factory=list)
    towns: List[dict] = field(default_factory=list)
    objects_on_map: List[dict] = field(default_factory=list)


# ============================================================================
# Field-offset constants (relative to block start) — format constants,
# verified against h3sed / vlucas / cysun / svetoslav save editors.
# These do NOT depend on the map.
# ============================================================================

HERO_FIELD_OFFSETS = {
    # All offsets RELATIVE TO NAME position (h3sed uses faction byte = name_pos - 169)
    "Player":               -169,
    "CoordinatesX":         -195,
    "CoordinatesY":         -193,
    "CoordinatesZ":         -191,
    "CoordinatesXMarker":   -150,
    "CoordinatesYMarker":  -146,
    "Experience":           -130,
    "MaxMovementPoints":    -138,
    "CurrentMovementPoints": -134,
    "ManaPoints":           -122,
    "HeroLevel":            -120,
    "NumOfSkills":          -126,
    "Creatures":            -56,
    "CreatureAmounts":      -28,
    "Attributes":           69,
    "Skills":               13,
    "SkillSlots":           41,
    "Spells":               73,
    "SpellBook":            143,
    "Helm":                 213,
    "Weapon":               237,
    "Shield":               245,
    "Armor":                253,
    "Inventory":            365,
}

TOWN_FIELD_OFFSETS = {
    "faction":     0,
    "type":        3,
    "x":           4,
    "y":           5,
    "z":           6,
    "army_types":  9,
    "army_counts": 37,
    "name_len":    69,
    "name":        71,
}

HERO_BLOCK_SIZE = 1122          # approximate (h3sed)
HERO_STRIDE_SOD = 0x446         # 1094 bytes (verified on Myth and Legend, confirmed by ProspectorRT IL)
HERO_NAME_OFFSET_FROM_BLOCK_START = 169  # h3sed
TOWN_NAME_OFFSET_FROM_BLOCK_START = 71   # h3sed

# Town record base size — verified by ProspectorRT IL analysis (ScanTownsContent uses 0x17E = 382)
# Total town record size = TOWN_RECORD_BASE_SIZE + name_len_in_cp1251
# (the 382 bytes include the 2-byte name_len prefix at offset 69)
# On Myth and Legend: 21 towns with strides 386-390 (382 + name_len 4-8 bytes)
TOWN_RECORD_BASE_SIZE = 382


if __name__ == "__main__":
    # Smoke test: just print constants
    print("HERO_FIELD_OFFSETS:")
    for k, v in HERO_FIELD_OFFSETS.items():
        print(f"  {k:30s} {v:+5d}")
    print()
    print("TOWN_FIELD_OFFSETS:")
    for k, v in TOWN_FIELD_OFFSETS.items():
        print(f"  {k:30s} {v:+5d}")
    print()
    print(f"HERO_BLOCK_SIZE  = {HERO_BLOCK_SIZE}")
    print(f"HERO_STRIDE_SOD  = 0x{HERO_STRIDE_SOD:X}")
