#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
header_parser.py — Robust parser for the .GM1 save header.

The header layout (verified against `02_format_docs/header_pointer_search.md`
and probe_save.py diagnostics on both example maps) is:

  0x00..0x04  magic           5 bytes ASCII  "H3SVG" (scenario) or "H3SVC" (campaign)
  0x05..0x07  padding         3 bytes (usually zeros)
  0x08..0x0B  version_major   u32 LE         (0x2A = 42 = SoD / HotA)
  0x0C..0x0F  version_minor   u32 LE         (e.g. 2)
  0x10..0x2F  game_state      32 bytes       (day/week/month/difficulty)
  0x30..0x33  map_type        u32 LE         (28 = SoD)
  0x34        has_underground u8
  0x35..0x38  map_size        u32 LE
  0x39        is_playable     u8
  0x3A..0x3B  name_length     u16 LE
  0x3C..      map_name        name_length bytes, cp1251
  ..          description_len u16 LE
  ..          description     cp1251
  ..          player setup    (variable — 8 player records, each variable)
  ..          map_filename    ASCII, terminated by null
  ..          path            ASCII
  ..          save_filename   ASCII

We compute `header_size` = byte offset where the game-state sections
begin. After the header, we expect to find player resource blocks,
object owners array, town records, hero blocks, visiting array,
main object array, path block, decoration bitmask.

The header_size heuristic: find the LAST of (map_filename, save_filename)
end + some padding. If we cannot find them, fall back to a heuristic
based on map_size (small maps have smaller headers).
"""

from __future__ import annotations
import struct
from typing import Optional
from save_layout import HeaderInfo


def _find_ascii_string(raw: bytes, start: int, max_len: int = 256) -> tuple[int, str]:
    """
    Find the next null-terminated ASCII string starting at `start`.
    Returns (end_offset, string) — end_offset is AFTER the null terminator
    (or after max_len chars if no null found).
    """
    end = start
    while end < len(raw) and end - start < max_len:
        if raw[end] == 0:
            return end + 1, raw[start:end].decode("ascii", errors="replace")
        if not (0x20 <= raw[end] <= 0x7e) and raw[end] not in (0x09, 0x0A, 0x0D):
            # non-printable, non-null — end the string here
            break
        end += 1
    return end, raw[start:end].decode("ascii", errors="replace")


def parse_header(raw: bytes) -> HeaderInfo:
    """
    Parse the header of a decompressed .GM1 save.

    Raises ValueError if `raw` is too short or magic bytes are wrong.
    """
    if len(raw) < 0x40:
        raise ValueError(f"Save too small ({len(raw)} bytes) — cannot parse header")

    magic = raw[:5].decode("ascii", errors="replace")
    if magic not in ("H3SVG", "H3SVC"):
        raise ValueError(f"Bad magic: {magic!r} (expected 'H3SVG' or 'H3SVC')")

    version_major = struct.unpack("<I", raw[0x08:0x0C])[0]
    version_minor = struct.unpack("<I", raw[0x0C:0x10])[0]
    game_state = raw[0x10:0x30]

    map_type = struct.unpack("<I", raw[0x30:0x34])[0]
    has_underground = raw[0x34] != 0
    map_size = struct.unpack("<I", raw[0x35:0x39])[0]
    is_playable = raw[0x39] != 0

    name_len = struct.unpack("<H", raw[0x3A:0x3C])[0]
    off = 0x3C
    if name_len > 0 and off + name_len <= len(raw):
        map_name = raw[off:off + name_len].decode("cp1251", errors="replace")
    else:
        map_name = ""
    off += name_len

    description = ""
    if off + 2 <= len(raw):
        desc_len = struct.unpack("<H", raw[off:off + 2])[0]
        off += 2
        if desc_len > 0 and off + desc_len <= len(raw):
            description = raw[off:off + desc_len].decode("cp1251", errors="replace")
        off += desc_len

    # Find map_filename: ASCII string ending in ".h3m"
    # Search from `off` (current position) up to a reasonable limit
    map_filename = ""
    save_filename = ""

    # Look for ".h3m" string between current offset and 0x1000
    h3m_idx = raw.find(b".h3m", off, off + 0x800)
    if h3m_idx > 0:
        # Walk back to find start of the filename
        end = h3m_idx + 4
        start = h3m_idx
        while start > off and 0x20 <= raw[start - 1] <= 0x7e:
            start -= 1
        map_filename = raw[start:end].decode("ascii", errors="replace")

    # Save filename: typically appears shortly after map filename
    # Look for a common pattern: a short ASCII string (1-15 chars) followed by ".GM1"
    # Or sometimes the save filename doesn't have an extension at all (e.g. "0000", "114")
    # Search from after map_filename to +0x400
    search_start = h3m_idx + 4 if h3m_idx > 0 else off
    save_idx = raw.find(b".GM1", search_start, search_start + 0x400)
    if save_idx > 0:
        # Walk back
        end = save_idx + 4
        start = save_idx
        while start > search_start and 0x20 <= raw[start - 1] <= 0x7e:
            start -= 1
        save_filename = raw[start:end].decode("ascii", errors="replace")
    else:
        # Try common save names
        for cand in (b"NEWGAME.gm1", b"AUTOSAVE", b"0000", b"0001", b"114"):
            idx = raw.find(cand, search_start, search_start + 0x400)
            if idx > 0:
                save_filename = cand.decode("ascii", errors="replace")
                break

    # Compute header_size: end of save_filename (or fallback heuristic)
    if save_filename:
        # Find the position of save_filename in raw
        sf_idx = raw.find(save_filename.encode("ascii", errors="replace"), off, off + 0x1000)
        if sf_idx > 0:
            header_size = sf_idx + len(save_filename)
            # Round up to next 4-byte boundary
            header_size = (header_size + 3) & ~3
        else:
            header_size = 0x400  # fallback
    elif h3m_idx > 0:
        # Use end of map_filename + some padding
        header_size = h3m_idx + 4 + 0x100
        header_size = (header_size + 3) & ~3
    else:
        # Last resort: heuristic based on map_size
        # Small maps (e.g. 36x36) tend to have header ~0x250..0x300
        # XL maps (e.g. 144x144) tend to have header ~0x400
        header_size = 0x400 if map_size > 64 else 0x300

    # Ensure header_size is at least some minimum (so cluster scanning
    # doesn't trip over header bytes)
    if header_size < 0x100:
        header_size = 0x100

    return HeaderInfo(
        magic=magic,
        version_major=version_major,
        version_minor=version_minor,
        map_type=map_type,
        has_underground=has_underground,
        map_size=map_size,
        is_playable=is_playable,
        map_name=map_name,
        description=description,
        map_filename=map_filename,
        save_filename=save_filename,
        header_size=header_size,
        game_state_32=game_state,
        raw_size=len(raw),
    )


if __name__ == "__main__":
    import sys
    import gzip
    import io

    def decompress(path: str) -> bytes:
        raw = open(path, "rb").read()
        if raw[:2] != b"\x1f\x8b":
            return raw
        out = b""
        gz = gzip.GzipFile(fileobj=io.BytesIO(raw))
        try:
            while True:
                chunk = gz.read(65536)
                if not chunk:
                    break
                out += chunk
        except Exception:
            pass
        return out

    for p in sys.argv[1:]:
        raw = decompress(p)
        h = parse_header(raw)
        print(f"\n--- {p} ---")
        for k, v in h.to_dict().items():
            if k == "game_state_32":
                print(f"  {k}: {v}")
            else:
                print(f"  {k}: {v!r}")
