#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
excel_export.py — Excel export of HoMM3 save contents.

This module is split out from gm1_parser.py so it can be imported without PySide6
(e.g. for headless CLI processing). It uses only openpyxl + standard library.

Main entry point: export_objects_to_excel(parsed, raw, output_path, ...)

See gm1_parser.py:_on_export_objects_excel for the GUI usage.
"""
from __future__ import annotations
import os
import json
from typing import Any, Dict, List, Optional

def export_objects_to_excel(parsed: Dict, raw: bytes, output_path: str,
                            objects_by_coord: Dict = None,
                            computed_offsets: Dict = None,
                            language: str = None) -> str:
    """
    Export all 7009 map objects to an Excel workbook with multiple sheets:
      - All Objects       (flat list of 7009)
      - By Category       (grouped by 21 categories)
      - Summary           (counts per type/category)
      - Wiki Dictionary   (2037 wiki types from object_types_dictionary.json)

    Plus standardized save-content sheets (Артефакты, Монстры, Банки, ...)
    populated with localized names using the `language` parameter.

    Language codes: 'en', 'ru_sod' (default), 'ru_wog', 'ru_complete'.
    Alias 'ru' → 'ru_sod'.

    Returns the path written.
    """
    try:
        from openpyxl import Workbook
        from openpyxl.styles import Font, PatternFill, Alignment, Border, Side
        from openpyxl.utils import get_column_letter
    except ImportError:
        raise RuntimeError(
            "openpyxl is required for Excel export. Install with: pip install openpyxl"
        )

    # ----- Localizer for game-object name lookup -----
    try:
        from localizer import Localizer
        loc = Localizer(language)
    except Exception:
        loc = None  # graceful degradation — fall back to integer IDs

    # ----- Use data from caller (no static file loading) -----
    script_dir = os.path.dirname(os.path.abspath(__file__))
    obj_map_dir = os.path.normpath(os.path.join(script_dir, "..", "03_object_mapping"))
    oc = objects_by_coord or {}

    if not oc:
        raise RuntimeError(
            "No map objects loaded. Load a Map JSON first (Ctrl+M)."
        )

    # Build flat list on-the-fly from objects_by_coord
    flat_objects = []
    for ck, rec in oc.items():
        flat_objects.append({
            "coord_key":    rec["coord_key"],
            "coord_int":    rec["coord_int"],
            "x":            rec["x"], "y": rec["y"], "z": rec["z"],
            "object_index": rec.get("object_index"),
            "sprite_ref_id": rec.get("sprite_ref_id"),
            "sprite_def":   rec.get("sprite_def", ""),
            "type":         rec.get("type", ""),
            "category":     rec.get("category", ""),
            "details":      rec.get("details", {}),
            "is_primary":   True,
        })
        for ovl in rec.get("overlays", []):
            flat_objects.append({
                "coord_key":    ck,
                "coord_int":    rec["coord_int"],
                "x":            rec["x"], "y": rec["y"], "z": rec["z"],
                "object_index": ovl.get("object_index"),
                "sprite_ref_id": ovl.get("sprite_ref_id"),
                "sprite_def":   ovl.get("sprite_def", ""),
                "type":         ovl.get("type", ""),
                "category":     ovl.get("category", ""),
                "details":      ovl.get("details", {}),
                "is_primary":   False,
            })
    flat_objects.sort(key=lambda o: (o["coord_int"], 0 if o["is_primary"] else 1))

    # ----- Load wiki dictionary (universal, not map-specific) + use passed offsets -----
    wiki_dict = None
    obj_dict_path = os.path.join(obj_map_dir, "object_types_dictionary.json")
    if os.path.exists(obj_dict_path):
        try:
            with open(obj_dict_path, "r", encoding="utf-8") as f:
                wiki_dict = json.load(f)
        except Exception:
            pass

    # Use offsets passed by caller (dynamically computed, not from file)
    objects_with_offsets = computed_offsets or {}

    # ----- Build workbook -----
    wb = Workbook()

    # Styling helpers
    header_font  = Font(bold=True, color="FFFFFF", size=11)
    header_fill  = PatternFill(start_color="305496", end_color="305496", fill_type="solid")
    header_align = Alignment(horizontal="center", vertical="center", wrap_text=True)
    thin_border  = Border(
        left=Side(style="thin", color="BBBBBB"),
        right=Side(style="thin", color="BBBBBB"),
        top=Side(style="thin", color="BBBBBB"),
        bottom=Side(style="thin", color="BBBBBB"),
    )

    def style_header(ws, row=1):
        for cell in ws[row]:
            cell.font = header_font
            cell.fill = header_fill
            cell.alignment = header_align
            cell.border = thin_border

    def autosize_columns(ws, max_widths=None):
        for col_cells in ws.columns:
            try:
                col_letter = get_column_letter(col_cells[0].column)
            except Exception:
                continue
            max_len = 0
            for cell in col_cells:
                if cell.value is None:
                    continue
                s = str(cell.value)
                if len(s) > max_len:
                    max_len = len(s)
            width = min(max(max_len + 2, 8), (max_widths or {}).get(col_letter, 60))
            ws.column_dimensions[col_letter].width = width

    # ----- Sheet 1: All Objects (flat 7009) -----
    ws1 = wb.active
    ws1.title = "All Objects"
    headers1 = [
        "#", "X", "Y", "Z", "coord_int", "coord_key", "is_primary",
        "object_index", "sprite_ref_id", "sprite_def",
        "type", "category",
        "details (JSON)",
        # Save-file offsets (per-object addresses in decompressed .GM1)
        "main_offset", "visiting_offset", "fog_offset",
        "alive_offset", "treasure_offset", "decoration_offset",
        "all_save_offsets", "verified",
        # Wiki info
        "wiki_name", "wiki_category", "wiki_object_id", "wiki_sub_id",
        "wiki_description",
    ]
    ws1.append(headers1)
    style_header(ws1)

    # Pre-build wiki lookup
    wiki_by_def = {}
    if wiki_dict:
        for fn, entry in wiki_dict.get("objects", {}).items():
            wiki_by_def[fn.lower()] = entry

    for i, o in enumerate(flat_objects, start=1):
        sd = (o.get("sprite_def") or "").lower()
        wiki = wiki_by_def.get(sd, {})
        # Get offsets from objects_with_offsets (by coord_int string)
        ci_str = str(o.get("coord_int", ""))
        offs = (objects_with_offsets or {}).get(ci_str, {})
        row = [
            i,
            o["x"], o["y"], o["z"],
            o["coord_int"], o["coord_key"],
            "YES" if o["is_primary"] else "no",
            o.get("object_index"),
            o.get("sprite_ref_id"),
            o.get("sprite_def", ""),
            o.get("type", ""),
            o.get("category", ""),
            json.dumps(o.get("details", {}), ensure_ascii=False)[:500],  # truncate long details
            # Save-file offsets
            offs.get("main_offset", "")       or "",
            offs.get("visiting_offset", "")   or "",
            offs.get("fog_offset", "")        or "",
            offs.get("alive_offset", "")      or "",
            offs.get("treasure_offset", "")   or "",
            offs.get("decoration_offset", "") or "",
            ", ".join(offs.get("save_offsets", []))[:300],
            "YES" if offs.get("verified") else "no",
            # Wiki info
            wiki.get("wiki_name", ""),
            wiki.get("category", ""),
            wiki.get("object_id", ""),
            wiki.get("sub_id", ""),
            (wiki.get("description", "") or "").replace("\n", " ")[:300],
        ]
        ws1.append(row)

    # Freeze top row + filter
    ws1.freeze_panes = "A2"
    ws1.auto_filter.ref = ws1.dimensions
    autosize_columns(ws1, max_widths={"M": 60, "T": 50, "Y": 80})

    # ----- Sheet 2: By Category (one row per object, grouped by category) -----
    ws2 = wb.create_sheet("By Category")
    headers2 = ["category", "#", "X", "Y", "Z", "type", "sprite_def",
                "is_primary", "object_index", "main_offset", "all_save_offsets", "details"]
    ws2.append(headers2)
    style_header(ws2)
    # Sort by category then coord_int
    sorted_by_cat = sorted(flat_objects, key=lambda o: (o["category"], o["coord_int"]))
    for o in sorted_by_cat:
        ci_str = str(o.get("coord_int", ""))
        offs = (objects_with_offsets or {}).get(ci_str, {})
        ws2.append([
            o["category"], "", o["x"], o["y"], o["z"],
            o["type"], o.get("sprite_def", ""),
            "YES" if o["is_primary"] else "no",
            o.get("object_index"),
            offs.get("main_offset", "") or "",
            ", ".join(offs.get("save_offsets", []))[:300],
            json.dumps(o.get("details", {}), ensure_ascii=False)[:300],
        ])
    ws2.freeze_panes = "A2"
    ws2.auto_filter.ref = ws2.dimensions
    autosize_columns(ws2, max_widths={"L": 60})

    # ----- Sheet 3: Summary (counts per type and category) -----
    ws3 = wb.create_sheet("Summary")
    from collections import Counter
    type_counter  = Counter(o["type"]   for o in flat_objects)
    cat_counter   = Counter(o["category"] for o in flat_objects)
    primary_count = sum(1 for o in flat_objects if o["is_primary"])
    overlay_count = sum(1 for o in flat_objects if not o["is_primary"])

    # Top section: meta info
    ws3.append(["Metric", "Value"])
    style_header(ws3)
    ws3.append(["Map name", parsed["file_info"].get("map_name", "(unknown)")])
    ws3.append(["Total objects (flat)", len(flat_objects)])
    ws3.append(["Primary objects", primary_count])
    ws3.append(["Overlay objects (stacked)", overlay_count])
    ws3.append(["Unique coordinates", len({o["coord_key"] for o in flat_objects})])
    ws3.append(["Distinct types", len(type_counter)])
    ws3.append(["Distinct categories", len(cat_counter)])
    ws3.append([])
    ws3.append([])

    # Categories breakdown
    ws3.append(["Category", "Count"])
    style_header(ws3, row=ws3.max_row)
    for cat, n in cat_counter.most_common():
        ws3.append([cat, n])
    ws3.append([])
    ws3.append([])

    # Types breakdown
    ws3.append(["Type", "Category", "Count"])
    style_header(ws3, row=ws3.max_row)
    type_to_cat = {o["type"]: o["category"] for o in flat_objects}
    for t, n in type_counter.most_common():
        ws3.append([t, type_to_cat.get(t, ""), n])

    autosize_columns(ws3, max_widths={"A": 50, "B": 30, "C": 12})

    # ----- Sheet 4: Wiki Dictionary (2037 types) -----
    if wiki_dict:
        ws4 = wb.create_sheet("Wiki Dictionary")
        headers4 = ["file_name", "wiki_name", "category", "object_id", "sub_id",
                    "generates_on", "restrictions", "layering", "description",
                    "user_notes", "user_category", "user_alias"]
        ws4.append(headers4)
        style_header(ws4)
        # Sort by category, then wiki_name
        sorted_wiki = sorted(wiki_dict.get("objects", {}).items(),
                             key=lambda kv: (kv[1].get("category", "").lower(),
                                             kv[1].get("wiki_name", "").lower()))
        for fn, entry in sorted_wiki:
            ue = entry.get("user_edits", {}) or {}
            ws4.append([
                entry.get("file_name", ""),
                entry.get("wiki_name", ""),
                entry.get("category", ""),
                entry.get("object_id", ""),
                entry.get("sub_id", ""),
                entry.get("generates_on", ""),
                entry.get("restrictions", ""),
                entry.get("layering", ""),
                (entry.get("description", "") or "").replace("\n", " ")[:500],
                ue.get("user_notes", ""),
                ue.get("user_category", ""),
                ue.get("user_alias", ""),
            ])
        ws4.freeze_panes = "A2"
        ws4.auto_filter.ref = ws4.dimensions
        autosize_columns(ws4, max_widths={"I": 80})

    # ----- Save -----
    # v3.17: Add standardized sheets from merged_objects + post_tile_content + extra_sections
    mo = parsed.get("merged_objects", {})
    ptc = parsed.get("post_tile_content", {})

    # Localizer helpers (graceful when no loc)
    def _art_name(idx):
        return loc.art(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _spl_name(idx):
        return loc.spell(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _skl_name(idx):
        return loc.skill(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _cls_name(aid):
        """Return localized artifact class name. `aid` is artifact_id (NOT class_id).
        Looks up class_id from art_metadata, then localizes."""
        if not loc or not isinstance(aid, int) or aid < 0:
            return ""
        try:
            from art_metadata import get_artifact_class_id
            cls_id = get_artifact_class_id(aid)
            return loc.art_class(cls_id)
        except Exception:
            return ""
    def _town_name(idx):
        return loc.town(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _color_name(idx):
        return loc.color(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _tent_name(idx):
        return loc.tent(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _quest_name(idx):
        return loc.quest(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _reward_name(idx):
        return loc.reward(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _mine_name(idx):
        return loc.mine(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _place_name(idx):
        return loc.place(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _hero_class_name(idx):
        return loc.hero_class(idx) if loc and isinstance(idx, int) and idx >= 0 else ""

    # ----- Monster name lookup (PRT TblMonsters, then localizer fallback) -----
    # PRT uses hardcoded TblMonsters table (e.g., "Джинн", "Скелет", "Змий-дракон").
    # Our localizer (ru_sod) has slightly different names ("Джин", "Стрекоза").
    # Use TblMonsters first for PRT-exact matching; fall back to localizer.
    try:
        from art_metadata import get_monster_name as _prt_monster_name
        _has_prt_names = True
    except Exception:
        _has_prt_names = False

    # ----- Excel-only calculations (∑ HP) — imported from excel_calculations -----
    # This module is NOT imported by parsing modules (save_parser, merged_objects,
    # art_metadata) or JSON/tree exporters. Only excel_export uses it.
    try:
        from excel_calculations import (
            hero_sum_hp, town_sum_hp, prison_sum_hp,
            artifact_sum_hp, resource_sum_hp, bank_sum_hp,
            event_box_sum_hp, monster_sum_hp,
            aggregate_all_artifacts, aggregate_all_spells,
            aggregate_all_skills, aggregate_all_experience,
        )
    except Exception:
        hero_sum_hp = town_sum_hp = prison_sum_hp = None
        artifact_sum_hp = resource_sum_hp = bank_sum_hp = None
        event_box_sum_hp = monster_sum_hp = None
        aggregate_all_artifacts = aggregate_all_spells = None
        aggregate_all_skills = aggregate_all_experience = None

    def _mon_name(idx):
        """Return localized monster name. Tries PRT TblMonsters first (for
        PRT-exact match), falls back to localizer (ru_sod) if not found.
        Returns empty string for invalid indices.
        """
        if not isinstance(idx, int) or idx < 0:
            return ""
        if _has_prt_names:
            name = _prt_monster_name(idx)
            if name:
                return name
        return loc.monstr(idx) if loc else ""

    def _sanitize(val):
        """Convert any list/dict/tuple to string for safe Excel output."""
        if isinstance(val, (list, dict, tuple)):
            return str(val)
        return val

    def _guard_str(guard_list):
        """Format guard list as 'Pikeman 5, Goblin 20' — matching PRT format.

        Per PRT MainForm.cs:9033-9065 (GetBankGuard):
          - 7 slots can have duplicate monster_ids
          - ItWas() deduplicates: if monster_id was already seen, sum the count
            into the existing entry instead of adding a new one
          - Final format: "<name> <count>, <name> <count>, ..." (no 'x' prefix)
          - Monster names come from TblMonsters (PRT-exact, e.g. "Змий-дракон")

        Example: 3 slots of Скелет (count 10 each) + 2 slots of Живой мертвец
        (count 10 each) → "Скелет 30, Живой мертвец 20"
        """
        if not guard_list:
            return None
        # If guard is already a formatted string (from aggregator), return as-is.
        if isinstance(guard_list, str):
            return guard_list
        # Deduplicate by monster_id, summing counts (PRT ItWas logic)
        seen: dict = {}  # monster_id → total_count
        order: list = []  # preserve first-seen order
        for g in guard_list:
            if isinstance(g, dict):
                mid = g.get('monster_id', -1)
                cnt = g.get('count', 0)
                if mid is None or mid < 0:
                    continue
                if mid in seen:
                    seen[mid] += cnt  # ItWas: sum count into existing entry
                else:
                    seen[mid] = cnt
                    order.append(mid)
            else:
                # Non-dict entries (shouldn't happen, but be safe)
                order.append(("raw", str(g)))
                seen[("raw", str(g))] = 0
        parts = []
        for mid in order:
            if isinstance(mid, tuple):
                parts.append(mid[1])
            else:
                mname = _mon_name(mid) or f"M{mid}"
                cnt = seen[mid]
                # PRT format: "<name> <count>" (no 'x' prefix)
                parts.append(f"{mname} {cnt}")
        return ", ".join(parts) if parts else None

    def _art_list_str(art_ids):
        """Convert list of artifact IDs to comma-separated names."""
        if not art_ids:
            return None
        parts = []
        for aid in art_ids:
            if isinstance(aid, int) and aid >= 0:
                n = _art_name(aid)
                parts.append(n if n else f"A{aid}")
        return ", ".join(parts) if parts else None

    def _spl_list_str(spl_ids):
        """Convert list of spell IDs to comma-separated names."""
        if not spl_ids:
            return None
        parts = []
        for sid in spl_ids:
            if isinstance(sid, int) and sid >= 0:
                n = _spl_name(sid)
                parts.append(n if n else f"S{sid}")
        return ", ".join(parts) if parts else None

    def _skl_list_str(skl_ids):
        """Convert list of skill IDs to comma-separated names."""
        if not skl_ids:
            return None
        parts = []
        for skid in skl_ids:
            if isinstance(skid, int) and skid >= 0:
                n = _skl_name(skid)
                parts.append(n if n else f"Sk{skid}")
        return ", ".join(parts) if parts else None

    if isinstance(mo, dict) and mo and "error" not in mo:

        # ── Артефакты ──
        # Universal: writes all populated fields from merge_artifacts() output.
        #   rec keys: x, y, z, locality, object, slot, artifact_id, artifact_name,
        #             class_, relic, gold, resource, guard, hp
        # No hardcoding — class/relic come from art_metadata lookups done in merge.
        ws = wb.create_sheet("Артефакты")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Слот", "Артефакт", "Класс", "Реликт-С", "Золото", "Ресурс", "Охрана", "∑ HP"])
        for rec in mo.get("Артефакты", []):
            aid = rec.get("artifact_id", -1)
            # Localized artifact name: prefer rec's stored name (already localized
            # by merge_artifacts); fallback to localizer lookup
            art_name = rec.get("artifact_name")
            if not art_name and aid is not None and aid >= 0:
                art_name = _art_name(aid) or aid
            # Guard formatting (list of dicts → 'Pikeman x5, Goblin x20' string)
            guard_val = rec.get("guard")
            guard_str = _guard_str(guard_val) if guard_val else None
            # Slot: PRT shows 1..N for banks; None for tile-level artifacts
            slot_val = rec.get("slot")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Артефакт"),
                       slot_val,
                       art_name,
                       rec.get("class_"),
                       rec.get("relic"),
                       rec.get("gold"),
                       rec.get("resource"),
                       guard_str,
                       artifact_sum_hp(rec) if artifact_sum_hp else None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Монстры ──
        # Universal: writes all populated fields from merge_monsters() output.
        #   rec keys: x, y, z, locality, monster (PRT name), count, mood,
        #             level (1..7), hp (∑ HP), artifact (name), gold (int),
        #             resource (PRT "G8, S5"), grows ('Да' or None)
        ws = wb.create_sheet("Монстры")
        ws.append(["X", "Y", "Z", "Район", "Монстр", "Количество", "Настроение", "Уровень", "∑ HP", "Артефакт", "Золото", "Ресурс", "Прирост"])
        for rec in mo.get("Монстры", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("monster"),
                       rec.get("count"),
                       rec.get("mood"),
                       rec.get("level"),
                       monster_sum_hp(rec) if monster_sum_hp else None,
                       rec.get("artifact"),
                       rec.get("gold"),
                       rec.get("resource"),
                       rec.get("grows")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Банки ──
        # Universal: writes all populated fields from merge_banks() output.
        #   rec keys: x, y, z, locality, object (localized bank name),
        #             guard (list), hp (∑ HP), monster_reward ("<name> <count>"),
        #             gold (int), resources (PRT "G8, S5" string)
        # No hardcoding — all formatted in merge_banks via PRT logic.
        ws = wb.create_sheet("Банки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Охрана", "∑ HP", "Монстр", "Золото", "Ресурс"])
        for rec in mo.get("Банки", []):
            # Guard: list of {monster_id, count} → formatted string (PRT format
            # with ItWas dedup, e.g. "Нага 8, Королева нага 2")
            guard_val = rec.get("guard")
            guard_str = _guard_str(guard_val) if guard_val else None
            # ∑ HP: computed by excel_calculations.bank_sum_hp (Excel-only)
            hp_val = bank_sum_hp(rec) if bank_sum_hp else None
            # Monster reward: PRT-formatted "<name> <count>" (e.g. "Виверн 4")
            monster_val = rec.get("monster_reward")
            # Gold: int or None
            gold_val = rec.get("gold")
            # Resources: PRT-formatted "G8, S5" string
            res_val = rec.get("resources")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Bank"),
                       guard_str,
                       hp_val,
                       monster_val,
                       gold_val,
                       res_val])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── События и Ящики Пандоры ──
        # Universal: writes all populated fields from merge_event_boxes() output.
        #   rec keys: x, y, z, locality, object, guard, hp, experience, mana,
        #             morale, luck, gold, resource, primary_skill, secondary_skill,
        #             artifact, spell, monster, available
        ws = wb.create_sheet("События и Ящики Пандоры")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Охрана", "∑ HP", "Опыт", "Мана",
                   "Мораль", "Удача", "Золото", "Ресурс", "Первичный навык",
                   "Вторичный навык", "Артефакт", "Заклинание", "Монстр", "Доступно"])
        for rec in mo.get("События_Ящики_Пандоры", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Событие"),
                       rec.get("guard"),
                       event_box_sum_hp(rec) if event_box_sum_hp else None,
                       rec.get("experience"),
                       rec.get("mana"),
                       rec.get("morale"),
                       rec.get("luck"),
                       rec.get("gold"),
                       rec.get("resource"),
                       rec.get("primary_skill"),
                       rec.get("secondary_skill"),
                       rec.get("artifact"),
                       rec.get("spell"),
                       rec.get("monster"),
                       rec.get("available")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Ученые ──
        ws = wb.create_sheet("Ученые")
        ws.append(["X", "Y", "Z", "Район", "Первичный навык", "Вторичный навык", "Заклинание"])
        for rec in mo.get("Ученые", []):
            # Primary skill: 'A+1' / 'D+1' / 'P+1' / 'K+1' (matches PRT aPrSkill[k]+"+1")
            # Computed in merge_scholars; fallback to None if not a primary-skill scholar.
            primary_str = rec.get("primary_skill")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       primary_str,
                       _skl_name(rec.get("skill_id", -1)) if rec.get("skill_id") is not None else None,
                       _spl_name(rec.get("spell_id", -1)) if rec.get("spell_id") is not None else None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Ресурсы ──
        # Universal: writes all populated fields from merge_resources() output.
        #   rec keys: x, y, z, locality, object, resource (PRT format like "S4"), gold, guard, hp
        # No hardcoding — resource letter format comes from art_metadata via merge.
        ws = wb.create_sheet("Ресурсы")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Ресурс", "Золото", "Охрана", "∑ HP"])
        for rec in mo.get("Ресурсы", []):
            # Resource: PRT format like "S4", "W10", "G5", or None for gold-only rows
            resource_val = rec.get("resource")
            # Gold: integer or None
            gold_val = rec.get("gold")
            # Guard: list of dicts → 'Pikeman 5, Goblin 20' (PRT format with ItWas dedup)
            guard_val = rec.get("guard")
            guard_str = _guard_str(guard_val) if guard_val else None
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Ресурс"),
                       resource_val,
                       gold_val,
                       guard_str,
                       resource_sum_hp(rec) if resource_sum_hp else None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Сундуки ──
        ws = wb.create_sheet("Сундуки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Золото", "Опыт", "Артефакт", "Класс", "Реликт-С"])
        for rec in mo.get("Сундуки", []):
            aid = rec.get("artifact_id", -1) if rec.get("has_artifact") else -1
            # Look up relic-C from art_metadata (PRT DEFAULT TblArt)
            relic_name = None
            if aid >= 0:
                try:
                    from art_metadata import get_artifact_relic_c_id, get_artifact_name
                    relic_id = get_artifact_relic_c_id(aid)
                    if relic_id is not None:
                        relic_name = get_artifact_name(relic_id, localizer=None)
                except Exception:
                    pass
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Сундук"), rec.get("gold"), rec.get("experience"),
                       _art_name(aid) if aid >= 0 else None,
                       _cls_name(aid) if aid >= 0 else None,
                       relic_name])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Заклинания ──
        # Reads from mo["Заклинания_Святилища"] — now includes BOTH shrines (88/89/90)
        # and pyramids (63) per PRT R_Spell table (SaveShrine + SavePyramid both
        # populate the SAME table). Pyramids carry a hardcoded guard
        # "Золотой Голем 40, Алмазный Голем 20" + HP 3200.
        ws = wb.create_sheet("Заклинания")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Заклинание", "Уровень", "Охрана", "∑ HP"])
        for rec in mo.get("Заклинания_Святилища", []):
            sid = rec.get("spell_id", 0) or 0
            shrine_name = rec.get("object") or "Святыня"
            spell_name = _spl_name(sid) or sid
            spell_level = rec.get("spell_level")
            if spell_level is None and loc:
                spell_level = loc.spell_level(sid)
            # Guard + HP — only populated for pyramids; shrines leave them None
            guard_str = rec.get("guard")
            hp_val = rec.get("hp")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       shrine_name, spell_name, spell_level, guard_str, hp_val])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Навыки ──
        # Universal: writes both Witch Huts (single skill) and Universities
        # (4 skills joined as comma-separated string) into Навыки sheet.
        # PRT puts BOTH types in Навыки sheet (MainForm.cs SaveWitchHut+SaveUniver).
        # No hardcoding — uses merge_witch_huts() output which dispatches
        # per art_metadata config.
        ws = wb.create_sheet("Навыки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Навык"])
        for rec in mo.get("Ведьмины_Хижины", []):
            # Skill: localized name (WitchHut) or comma-joined names (University)
            skill_val = rec.get("skill")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Хижина ведьмы"),
                       skill_val])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Лагеря Беженцев ──
        # Universal: writes all populated fields from merge_refugee_camps() output.
        #   rec keys: x, y, z, locality, object, monster (PRT name), count (u8), level (1..7)
        # No hardcoding — uses merge output directly.
        ws = wb.create_sheet("Лагеря Беженцев")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Монстр", "Количество", "Уровень"])
        for rec in mo.get("Лагеря_Беженцев", []):
            # Monster name: PRT-style from TblMonsters (e.g., "Скелет", "Крестоносец")
            monster_name = rec.get("monster")
            # Count: u8 value (max 255)
            count_val = rec.get("count")
            # Level: 1..7 from TblMonsters lookup
            level_val = rec.get("level")
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Лагерь беженцев"),
                       monster_name,
                       count_val,
                       level_val])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Рынки ──
        # PRT layout: Art Merchants rows first (no X/Y/Z), then Black Market rows
        # (with X/Y/Z). Each row: Объект, Слот 1..7, Артефакт, Класс, Реликт-С, Цена.
        ws = wb.create_sheet("Рынки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Слот", "Артефакт", "Класс", "Реликт-С", "Цена на Черном Рынке"])
        for rec in mo.get("Рынки", []):
            ws.append([
                rec.get("x"), rec.get("y"), rec.get("z"),
                rec.get("locality") if rec.get("locality") else None,
                rec.get("object", "Black Market"),
                rec.get("slot"),
                rec.get("artifact_name"),
                rec.get("class"),
                rec.get("relic_c"),
                rec.get("cost"),
            ])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Провидцы ──
        ws = wb.create_sheet("Провидцы")
        ws.append(["X", "Y", "Z", "Район", "Задание", "Награда", "Срок"])

        # Mapping from our post_tile_parser mission_type (0=empty, 1..9) to Quest[] index
        # in the data dictionary (0..8). This is a direct mapping based on what each
        # mission_type means in our parser (post_tile_parser.py:417).
        QUEST_TYPE_TO_DICT_IDX = {
            1: 0,  # t=1 "Reach level" → Quest[0]="Достичь уровня"
            2: 1,  # t=2 "Reach primary skills" → Quest[1]="Получить навык"
            3: 7,  # t=3 "Find hero {" → Quest[7]="Победить героя" (or similar)
            4: 2,  # t=4 "Kill monster" → Quest[2]="Победить монстра"
            5: 3,  # t=5 "Bring artefacts" → Quest[3]="Принести артефакты"
            6: 4,  # t=6 "Bring monsters" → Quest[4]="Принести монстров"
            7: 5,  # t=7 "Bring resources" → Quest[5]="Принести ресурсы"
            8: 7,  # t=8 "Defeat hero }" → Quest[7]="Победить героя"
            9: 6,  # t=9 "Defeat color" → Quest[6]="Иметь цвет флага"
        }
        # Mapping from our reward_type (0=none, 1..10) to Reward[] index in dictionary.
        # Our parser order (post_tile_parser.py:535) matches Reward[] in dictionary:
        # 1=experience→0, 2=mana→1, 3=morale→2, 4=luck→3, 5=resource→4, 6=primary→5(skill),
        # 7=secondary→5(skill), 8=artifact→6, 9=spell→7, 10=monster→8
        REWARD_TYPE_TO_DICT_IDX = {
            1: 0,  # experience
            2: 1,  # mana
            3: 2,  # morale
            4: 3,  # luck
            5: 4,  # resource
            6: 5,  # primary skill (matches "Навык" in dictionary)
            7: 5,  # secondary skill (matches "Навык" in dictionary)
            8: 6,  # artifact
            9: 7,  # spell
            10: 8, # monster
        }

        for rec in mo.get("Провидцы", []):
            # Mission
            mtype = rec.get("mission_type")
            mission = rec.get("mission", {}) or {}
            if mtype is None or mtype == 0:
                mission_str = "—" if mtype == 0 else str(mission)
            else:
                quest_id = QUEST_TYPE_TO_DICT_IDX.get(mtype, mtype - 1)
                mission_str = _quest_name(quest_id) or str(mtype)
                if "level" in mission:
                    mission_str += f" {mission['level']}"
                if "artifacts" in mission:
                    arts = mission['artifacts']
                    if isinstance(arts, list):
                        art_names = [_art_name(a) or str(a) for a in arts if isinstance(a, int) and a >= 0]
                        if art_names:
                            mission_str += f" [{', '.join(art_names)}]"
                if "monsters" in mission:
                    mons = mission['monsters']
                    if isinstance(mons, list):
                        mon_parts = []
                        for m in mons:
                            if isinstance(m, dict):
                                mid = m.get('monster_id', -1)
                                cnt = m.get('count', 0)
                                if mid >= 0:
                                    mon_parts.append(f"{_mon_name(mid) or mid} x{cnt}")
                            elif isinstance(m, (list, tuple)) and len(m) >= 2:
                                mon_parts.append(f"{_mon_name(m[0]) or m[0]} x{m[1]}")
                        if mon_parts:
                            mission_str += f" [{', '.join(mon_parts)}]"
                if "resources" in mission:
                    res = mission['resources']
                    if isinstance(res, dict):
                        res_parts = []
                        for rkey, rcount in res.items():
                            if rcount:
                                try:
                                    ridx = ['Wood', 'Mercury', 'Ore', 'Sulfur', 'Crystal', 'Gems'].index(rkey)
                                    rname = _mine_name(ridx) or rkey
                                except (ValueError, IndexError):
                                    rname = rkey
                                res_parts.append(f"{rname} x{rcount}")
                        if res_parts:
                            mission_str += f" [{', '.join(res_parts)}]"
            # Reward
            rtype = rec.get("reward_type")
            reward = rec.get("reward", {}) or {}
            if rtype is None or rtype == 0:
                reward_str = "—" if rtype == 0 else str(reward)
            else:
                reward_id = REWARD_TYPE_TO_DICT_IDX.get(rtype, rtype - 1)
                reward_str = _reward_name(reward_id) or str(rtype)
                if "experience" in reward:
                    reward_str += f" {reward['experience']}"
                if "mana" in reward:
                    reward_str += f" {reward['mana']}"
                if "morale" in reward:
                    reward_str += f" {reward['morale']}"
                if "luck" in reward:
                    reward_str += f" {reward['luck']}"
                if "amount" in reward:
                    reward_str += f" {reward['amount']}"
                if "artifact_id" in reward or "artifacts" in reward:
                    aids = reward.get('artifacts', [reward.get('artifact_id', -1)])
                    if isinstance(aids, list):
                        art_names = [_art_name(a) or str(a) for a in aids if isinstance(a, int) and a >= 0]
                        if art_names:
                            reward_str += f" [{', '.join(art_names)}]"
                if "spell_id" in reward or "spells" in reward:
                    sids = reward.get('spells', [reward.get('spell_id', -1)])
                    if isinstance(sids, list):
                        spl_names = [_spl_name(s) or str(s) for s in sids if isinstance(s, int) and s >= 0]
                        if spl_names:
                            reward_str += f" [{', '.join(spl_names)}]"
                if "monster_id" in reward or "monsters" in reward:
                    mons = reward.get('monsters', [reward.get('monster_id', -1)])
                    if isinstance(mons, list):
                        mon_parts = []
                        for m in mons:
                            if isinstance(m, dict):
                                mid = m.get('monster_id', -1)
                                cnt = m.get('count', 0)
                                if mid >= 0:
                                    mon_parts.append(f"{_mon_name(mid) or mid} x{cnt}")
                            elif isinstance(m, int) and m >= 0:
                                mon_parts.append(_mon_name(m) or str(m))
                        if mon_parts:
                            reward_str += f" [{', '.join(mon_parts)}]"
                if "resource" in reward and "amount" in reward:
                    rt = reward['resource']
                    reward_str += f" {rt} x{reward['amount']}"
                if "resource_type" in reward and "amount" in reward:
                    rt = reward['resource_type']
                    rn = _mine_name(rt) if isinstance(rt, int) else str(rt)
                    reward_str += f" {rn} x{reward['amount']}"
                # Secondary skill (reward_type 7)
                if "secondary_skill_id" in reward:
                    sid = reward.get("secondary_skill_id", -1)
                    lvl = reward.get("level_id", 0)
                    if isinstance(sid, int) and sid >= 0:
                        skn = _skl_name(sid) or str(sid)
                        reward_str += f" {skn}"
                        if isinstance(lvl, int) and lvl >= 0:
                            lvl_name = loc.full_level_skill(lvl) if loc else None
                            reward_str += f" ({lvl_name or 'L' + str(lvl)})"
                # Primary skill (reward_type 6)
                if "primary_skill" in reward:
                    psk = reward.get("primary_skill", "")
                    amt = reward.get("amount", 0)
                    reward_str += f" {psk}"
                    if amt:
                        reward_str += f" +{amt}"
            ws.append([_sanitize(rec.get("x")), _sanitize(rec.get("y")), _sanitize(rec.get("z")),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       _sanitize(mission_str), _sanitize(reward_str), _sanitize(rec.get("deadline"))])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Тюрьмы ──
        ws = wb.create_sheet("Тюрьмы")
        ws.append(["X", "Y", "Z", "Район", "Герой", "Уровень", "Опыт", "Первичный навык", "Вторичный навык", "Артефакт", "Заклинание", "Монстр", "∑ HP", "Машина", "Книга заклинаний", "MP"])
        for rec in mo.get("Тюрьмы", []):
            # All fields are pre-formatted in merge_prisons to match PRT
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("hero_name"),
                       rec.get("hero_level"),
                       rec.get("hero_experience"),
                       rec.get("primary_skill"),      # "A2-D2-P1-K1"
                       rec.get("secondary_skill"),    # "Б.Лидерство, Б.Тактика"
                       None,                          # Артефакт (not in prison per PRT)
                       rec.get("spell"),              # single starting spell name
                       rec.get("monster"),            # "Копейщик 12, Арбалетчик 7"
                       prison_sum_hp(rec) if prison_sum_hp else None,  # ∑ HP (Excel-only)
                       rec.get("machine"),            # "Палатка первой помощи" or None
                       rec.get("book"),               # "Да" or None
                       rec.get("mp")])                # 1560
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Объекты ──
        ws = wb.create_sheet("Объекты")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Плата"])
        for rec in mo.get("Объекты", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object"),
                       rec.get("payment")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Гарнизоны ──
        ws = wb.create_sheet("Гарнизоны")
        ws.append(["X", "Y", "Z", "Район", "Антимагический", "Охрана", "∑ HP", "Флаг", "Можно забрать"])
        for rec in mo.get("Гарнизоны", []):
            guard = rec.get("guard", [])
            guard_str = _guard_str(guard) if guard else None
            hp_val = 0
            try:
                from excel_calculations import compute_guard_hp
                hp_val = compute_guard_hp(guard)
            except Exception:
                pass
            color_idx = rec.get("color", -1)
            color_str = _color_name(color_idx) if isinstance(color_idx, int) and 0 <= color_idx < 8 else None
            can_take = "Да" if rec.get("can_take") else "Нет"
            anti_magic = "Да" if rec.get("anti_magic") else "Нет"
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "море" if rec.get("locality") == "море" else "суша",
                       anti_magic,
                       guard_str,
                       hp_val if hp_val > 0 else None,
                       color_str,
                       can_take])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Стражи Прохода ──
        ws = wb.create_sheet("Стражи Прохода")
        ws.append(["X", "Y", "Z", "Район", "Задание", "Срок"])
        for rec in mo.get("Border_Guards", []):
            mission = rec.get("mission", {})
            mission_str = None
            if isinstance(mission, dict):
                if "level" in mission:
                    mission_str = f"Достичь уровня {mission['level']}"
                elif "primary_skills" in mission:
                    ps = mission["primary_skills"]
                    if isinstance(ps, dict):
                        parts = [f"{k} {v}" for k, v in ps.items()]
                        mission_str = "Иметь первичные навыки " + ", ".join(parts)
                elif "hero_id" in mission:
                    mission_str = f"Победить героя ({mission.get('mission_str', '')})"
                elif "artifacts" in mission:
                    arts = mission["artifacts"]
                    if isinstance(arts, list):
                        mission_str = f"Принести артефакты {arts}"
            deadline = rec.get("deadline")
            if isinstance(deadline, (list, dict, tuple)):
                deadline = str(deadline)
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "море" if rec.get("locality") == "море" else "суша",
                       _sanitize(mission_str), deadline])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Топология ──
        # PRT: 1 row per topology object from tile scan (no duplicates).
        # Tent: Object (localized) + Color (from aTent[]).
        # Monolith: Object (localized) + Type (subtype as string).
        # Subterranean Gate: Object (localized) + Pair (from post-tile pairing).
        ws = wb.create_sheet("Топология")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Тип", "Цвет", "Пара"])
        for rec in mo.get("Топология_Палатки", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Палатка ключника"),
                       None,
                       rec.get("color"),
                       None])
        for rec in mo.get("Топология_Монолиты", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Монолит"),
                       rec.get("type"),
                       None,
                       None])
        for rec in mo.get("Топология_Подземные_Врата", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Врата подземного мира"),
                       None,
                       None,
                       rec.get("pair")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── События-Таймеры ──
        # PRT GetTimedEvents: MapTimedEvents + TownsTimedEvents + MapTownsTimedEvents
        # Sort: DataView.Sort = "Day, Object, ID" (ascending).
        # Then GetTimerTown() enriches town events with Name/Type/Place/Color from
        # the matching town (matched by town index byte).
        # Then GetTimerApply() computes "Доступно" from apply = (X, Y, Z):
        #   for each color j: if (X bit j set) AND aExistColor[j]>0:
        #     - if j == human AND Y > 0: include color
        #     - elif j != human AND Z > 0: include color
        ws = wb.create_sheet("События-Таймеры")
        ws.append(["Объект", "День", "Повтор", "Город", "Тип", "Местоположение", "Флаг", "Доступно", "Золото", "Ресурс", "Постройки", "Монстр"])

        # PRT aResource / aColor / aTown (DEFAULT array, MainForm.cs:11750/11752/1055).
        # PRT 0000.xlsx was generated in ObjNameDefault mode, so use DEFAULT arrays.
        _A_RESOURCE_LETTERS = ["W", "M", "O", "S", "C", "G"]
        _A_RESOURCE_NAMES = ["Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems"]
        _A_COLOR = ["Красный", "Синий", "Коричневый", "Зеленый",
                    "Оранжевый", "Пурпурный", "Бирюзовый", "Розовый"]
        _A_TOWN = ["Замок", "Оплот", "Башня", "Инферно", "Некрополис",
                   "Темница", "Цитадель", "Крепость", "Сопряжение"]

        # Towns sorted by block_offset (matches PRT's iteration order).
        # PRT's `town` pointer starts 1 byte before OUR's `block_offset`,
        # so PRT's R_Town.ID = raw[block_offset - 1] = TOWN INDEX in this list.
        # Two sources of town list:
        #   - parsed["towns"]              (from parsed_save_to_dict — full dicts)
        #   - parsed["towns_found"]        (from gm1_parser GUI — simplified dicts
        #     with "offset" instead of "block_offset", and "location" tuple)
        # Use whichever is non-empty.
        _towns_source = parsed.get("towns") or parsed.get("towns_found") or []
        # Normalize each town dict — both sources have name/type/faction/fields,
        # but `towns_found` uses `offset` instead of `block_offset` and packs
        # X/Y/Z into a `location` tuple. Use fields[] as the source of truth.
        def _town_block_offset(t):
            return t.get("block_offset") or t.get("offset") or \
                   (t.get("fields", {}) or {}).get("_detail_block_offset", {}).get("first_addr", 0)
        _towns_sorted = sorted(_towns_source, key=_town_block_offset)

        # Compute human_idx and aExistColor[] (PRT GetColorContent, line 7653).
        human_idx = -1
        a_exist_color = [0] * 8
        offsets = ptc.get("offsets", {}) if isinstance(ptc, dict) else {}
        color_off = offsets.get("color", 0) if isinstance(offsets, dict) else 0
        if raw is not None and color_off > 0:
            for i in range(8):
                off_human = color_off + i * 145 + 14
                off24 = color_off + i * 145 + 24
                off1 = color_off + i * 145 + 1
                if off_human < len(raw) and raw[off_human] == 3:
                    human_idx = i
                if off24 < len(raw) and off1 < len(raw):
                    a_exist_color[i] = raw[off24] + raw[off1]

        # Helper: format resources as PRT GetTimerRes does.
        # PRT format: "W11287806, M11022488, ..." (single letter + amount, no space, no "x").
        def _fmt_timer_res(res_dict):
            if not isinstance(res_dict, dict):
                return None
            parts = []
            for i, rname in enumerate(_A_RESOURCE_NAMES):
                amount = res_dict.get(rname, 0)
                if amount and amount != 0:
                    letter = _A_RESOURCE_LETTERS[i]
                    parts.append(f"{letter}{amount}")
            return ", ".join(parts) if parts else None

        # Helper: format buildings per PRT GetTimerTown (line 7018-7063).
        # PRT decodes the 6-byte building bitmask array using TblBuilding
        # lookup table (Type, Byte, Bit → Building name).
        #   - For k in 0..4 (5 bytes): each byte's 8 bits → building name lookup
        #     via (Type=town_type, Byte=k+1, Bit=l) where bit l is set
        #   - For k=5 (byte 6): single bit (Bit=0) → one building lookup
        # Empty Building ("") entries in TblBuilding are skipped by PRT.
        #
        # Load TblBuilding from 02_format_docs/tbl_building_mapping.json
        # (extracted from MainForm.cs CreateTblBuilding).
        _TBL_BUILDING_CACHE = None
        def _load_tbl_building():
            nonlocal _TBL_BUILDING_CACHE
            if _TBL_BUILDING_CACHE is not None:
                return _TBL_BUILDING_CACHE
            try:
                import json
                import os
                _here = os.path.dirname(os.path.abspath(__file__))
                _docs = os.path.normpath(os.path.join(_here, "..", "02_format_docs"))
                path = os.path.join(_docs, "tbl_building_mapping.json")
                with open(path, "r", encoding="utf-8") as f:
                    _TBL_BUILDING_CACHE = json.load(f)
            except Exception:
                _TBL_BUILDING_CACHE = {}
            return _TBL_BUILDING_CACHE

        # tbl_monsters_mapping.json — already loaded as _tbl_monsters at top
        # of this module. Has "town" field; we need (town, level) → name.
        _TBL_MONSTERS_CACHE = None
        def _load_tbl_monsters():
            nonlocal _TBL_MONSTERS_CACHE
            if _TBL_MONSTERS_CACHE is not None:
                return _TBL_MONSTERS_CACHE
            try:
                import json
                import os
                _here = os.path.dirname(os.path.abspath(__file__))
                _docs = os.path.normpath(os.path.join(_here, "..", "02_format_docs"))
                path = os.path.join(_docs, "tbl_monsters_mapping.json")
                with open(path, "r", encoding="utf-8") as f:
                    _TBL_MONSTERS_CACHE = json.load(f)
            except Exception:
                _TBL_MONSTERS_CACHE = {}
            return _TBL_MONSTERS_CACHE

        def _fmt_timer_buildings(buildings, town_type_idx):
            """Decode 6-byte building bitmask array → PRT-style building names.

            Args:
              buildings: list of 6 ints (one per Byte 1..6, last is single-bit)
              town_type_idx: int 0..8 (PRT aTown index: 0=Замок, 1=Оплот, 2=Башня,
                              3=Инферно, 4=Некрополис, 5=Темница, 6=Цитадель,
                              7=Крепость, 8=Сопряжение)

            PRT logic (MainForm.cs:7018-7063):
              for k in 0..4: (5 bytes)
                num3 = int(array4[k])
                for l in 0..7:
                  if num3 & (1<<l):
                    find TblBuilding[type=town_type, byte=k+1, bit=l]
                    if Building != "": append
              if int(array4[5]) > 0:
                find TblBuilding[type=town_type, byte=6, bit=0]
                if Building != "": append
            """
            if not buildings or not isinstance(buildings, (list, tuple)):
                return None
            if len(buildings) < 6:
                return None
            if all(b == 0 for b in buildings):
                return None
            tbl = _load_tbl_building()
            parts = []
            # First 5 bytes — 8 bits each
            for k in range(5):
                num3 = int(buildings[k])
                if num3 <= 0:
                    continue
                for l in range(8):
                    if (num3 & (1 << l)) > 0:
                        key = f"{town_type_idx}:{k + 1}:{l}"
                        entry = tbl.get(key)
                        if entry and entry.get("building"):
                            parts.append(entry["building"])
            # 6th byte — only bit 0
            if int(buildings[5]) > 0:
                key = f"{town_type_idx}:6:0"
                entry = tbl.get(key)
                if entry and entry.get("building"):
                    parts.append(entry["building"])
            return ", ".join(parts) if parts else None

        # Helper: format monsters per PRT GetTimerTown (line 7000-7017).
        # PRT decodes the 7-element monster count array using TblMonsters lookup
        # (Town=town_type, Level=j+1) → monster name. Output format:
        #   "{Monster Name} {count}, ..." (matches PRT DEFAULT "Монах 6" style).
        def _fmt_timer_monsters(monsters, town_type_idx):
            """Decode 7-element monster count array → PRT-style monster names.

            Args:
              monsters: list of 7 ints (one per level 1..7)
              town_type_idx: int 0..8 (PRT aTown index)

            PRT logic (MainForm.cs:7001-7009):
              for j in 0..6:
                if int(array3[j]) > 0:
                  dataRow = TblMonsters.Select("Town=town_type AND Level=j+1")[0]
                  text += ", " + dataRow["Name"] + " " + array3[j]
            """
            if not monsters or not isinstance(monsters, (list, tuple)):
                return None
            if len(monsters) < 7:
                return None
            if all(m == 0 for m in monsters):
                return None
            tbl_m = _load_tbl_monsters()
            # Build (town, level) → name index for fast lookup
            parts = []
            for j in range(7):
                count = int(monsters[j])
                if count <= 0:
                    continue
                # Find monster with matching (town, level)
                # PRT: TblMonsters.Select("Town=town_type AND Level=j+1")[0]
                # — first match wins. In tbl_monsters_mapping.json:
                #   "116": {"name": "Золотой Голем", "level": 6, "town": 2}, ...
                name = None
                for code, info in tbl_m.items():
                    if (isinstance(info, dict) and
                            info.get("town") == town_type_idx and
                            info.get("level") == j + 1):
                        name = info.get("name")
                        break
                if name:
                    parts.append(f"{name} {count}")
            return ", ".join(parts) if parts else None

        # Helper: compute "Доступно" per PRT GetTimerApply (line 7124).
        def _fmt_timer_apply(apply_tuple):
            if not apply_tuple or not isinstance(apply_tuple, (list, tuple)) or len(apply_tuple) < 3:
                return None
            x_flags, y_num, z_num = apply_tuple[0], apply_tuple[1], apply_tuple[2]
            if not x_flags or human_idx < 0:
                return None
            avail = []
            for j in range(8):
                if (x_flags & (1 << j)) > 0 and a_exist_color[j] > 0:
                    if j == human_idx:
                        if y_num > 0:
                            avail.append(_A_COLOR[j])
                    else:
                        if z_num > 0:
                            avail.append(_A_COLOR[j])
            return ", ".join(avail) if avail else None

        # Helper: normalize an event object (dataclass OR dict) to a plain dict.
        # When called from gm1_parser.py GUI, `ptc["map_timed_events"]` and
        # `ptc["town_timed_events"]` contain TimedEvent dataclass instances
        # (NOT dicts). When called from regenerate_repo_xlsx.py (which uses
        # parsed_save_to_dict), they are already plain dicts. We must handle
        # BOTH formats so the timers sheet works in all entry points.
        def _ev_to_dict(ev):
            if isinstance(ev, dict):
                return ev
            if hasattr(ev, "__dict__"):
                # Dataclass — convert via asdict (handles nested dataclasses/tuples).
                try:
                    from dataclasses import asdict
                    return asdict(ev)
                except Exception:
                    # Fallback: pull public attributes into a dict.
                    return {k: getattr(ev, k) for k in vars(ev) if not k.startswith("_")}
            # Last resort: object with __dict__-like accessor.
            try:
                return dict(ev)
            except Exception:
                return {}

        # Collect all timer events with metadata for sorting.
        # Each entry: (day, object_name, id_for_sort, raw_ev_dict, is_map_event)
        all_events = []
        for ev in ptc.get("map_timed_events", []):
            ev_dict = _ev_to_dict(ev)
            if ev_dict:
                day = ev_dict.get("day") or 0
                # PRT MapTimedEvents does NOT set ID; default to 0 (DBNull sorts first)
                ev_id = ev_dict.get("id") or 0
                all_events.append((day, "Карта", ev_id, ev_dict, True))
        for ev in ptc.get("town_timed_events", []):
            ev_dict = _ev_to_dict(ev)
            if ev_dict:
                day = ev_dict.get("day") or 0
                # PRT R_AllTimerRow.ID = byte[body_start+37] = town_index
                ev_id = ev_dict.get("id") or 0
                all_events.append((day, "Город", ev_id, ev_dict, False))

        # Sort by (Day, Object, ID) — PRT DataView.Sort.
        # "Город" (Г=U+0413) < "Карта" (К=U+041A) alphabetically.
        all_events.sort(key=lambda e: (e[0], e[1], e[2]))

        # Emit rows in PRT-sorted order.
        for day, obj_name, ev_id, ev, is_map_event in all_events:
            res = ev.get("resources", {}) or {}
            res_str = _fmt_timer_res(res)
            gold_val = res.get("Gold") if isinstance(res, dict) else None
            if not gold_val:
                gold_val = None
            apply_tuple = ev.get("apply")
            available = _fmt_timer_apply(apply_tuple)
            repeat_val = ev.get("repeat") or None

            if is_map_event:
                # Map event: no town info, no buildings/monsters (PRT MapTimedEvents
                # does NOT call GetTimerContent — only GetTimerRes).
                # PRT outputs Day explicitly (even when 0), so use `day` not `day or None`.
                ws.append([obj_name, day, repeat_val,
                           None, None, None, None, available,
                           gold_val, res_str, None, None])
            else:
                # Town event: look up town by index, format buildings/monsters.
                town_idx = ev_id
                town = _towns_sorted[town_idx] if 0 <= town_idx < len(_towns_sorted) else None
                if town:
                    t_fields = town.get("fields", {}) or {}
                    t_name = town.get("name") or t_fields.get("name", "")
                    t_type_idx = town.get("type")
                    if t_type_idx is None:
                        t_type_idx = t_fields.get("type", 0)
                    t_type = _A_TOWN[t_type_idx] if 0 <= t_type_idx < len(_A_TOWN) else None
                    # Coords: `towns` has x/y/z as separate keys; `towns_found`
                    # packs them into a `location` tuple (x, y, z).
                    t_loc_tuple = town.get("location")
                    if isinstance(t_loc_tuple, (list, tuple)) and len(t_loc_tuple) >= 3:
                        tx, ty, tz = t_loc_tuple[0], t_loc_tuple[1], t_loc_tuple[2]
                    else:
                        tx = town.get("x") or t_fields.get("x", 0)
                        ty = town.get("y") or t_fields.get("y", 0)
                        tz = town.get("z") or t_fields.get("z", 0)
                    t_loc = f"{tx}.{ty}.{tz}"
                    # PRT Color = aColor[decmp[town+1]] if byte < 255.
                    # OUR's faction = raw[block_offset] = byte[town+1] in PRT's terms.
                    faction = town.get("faction")
                    if faction is None:
                        faction = t_fields.get("faction", 255)
                    t_flag = _A_COLOR[faction] if faction is not None and faction < 255 else None
                else:
                    t_name = t_type = t_loc = t_flag = None
                    t_type_idx = None

                bld_str = _fmt_timer_buildings(ev.get("buildings"),
                                               t_type_idx if t_type_idx is not None else 0)
                mon_str = _fmt_timer_monsters(ev.get("monsters"),
                                               t_type_idx if t_type_idx is not None else 0)

                ws.append([obj_name, day, repeat_val,
                           t_name, t_type, t_loc, t_flag, available,
                           gold_val, res_str, bld_str, mon_str])

        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Герои ──
        ws = wb.create_sheet("Герои")
        ws.append(["№", "Герой", "Местоположение", "Флаг", "Уровень", "Опыт", "Первичный навык", "Вторичный навык", "Артефакт", "Заклинание", "Монстр", "∑ HP", "Машина", "Книга заклинаний", "MP"])
        for i, h in enumerate(parsed.get("heroes_found", [])):
            f = h.get("fields", {})

            # Use pre-formatted fields from JSON
            place_str = h.get("place")
            flag_str = h.get("flag")
            primary_str = h.get("primary_skill")
            secondary_str = h.get("secondary_skill")

            # Artifact: from doll slots (pre-formatted in JSON)
            equip_str = h.get("artifact")

            # Spell: from JSON
            spells_str = h.get("spell")

            # Monster: in JSON it's ALWAYS present (full dump).
            # In Excel (report): only show if Place is set AND != "Запрещен"
            # (matches PRT MainForm.cs:7793 condition: !IsPlaceNull && Place != aPlace[2]
            # where aPlace[2] = "Запрещен"). Heroes with Place = None (not on map,
            # not in tavern) get NO monster/HP.
            monster_str = h.get("monster")
            # ∑ HP: Excel-only — computed from army_types/counts via excel_calculations.
            # Same filter as monster: only show for active heroes (Place set + != "Запрещен").
            is_active = bool(place_str) and place_str != "Запрещен"
            if not is_active:
                monster_str = None
                hero_hp_val = None
            else:
                hero_hp_val = hero_sum_hp(h) if hero_sum_hp else None

            # Machine: from JSON
            machines_str = h.get("machine")

            # Book: from JSON
            book_str = h.get("book")

            # MP: movement_total
            mp = f.get("movement_total")

            ws.append([i, f.get("name", "?"), place_str, flag_str,
                       f.get("level"), f.get("experience"),
                       primary_str, secondary_str, equip_str, spells_str,
                       monster_str, hero_hp_val, machines_str, book_str, mp])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Города ──
        # PRT GetTownContent (line 7458) creates 1 row per spell level (1..N).
        # First row: Garrison + ∑ HP + Library. Subsequent rows: Spell + Available.
        # Empty levels are DELETED (not shown). Sorted by Z, Y, X, Slot.
        ws = wb.create_sheet("Города")
        ws.append(["X", "Y", "Z", "Город", "Тип", "Флаг", "Уровень", "Заклинание",
                   "Построен", "Доступен", "Таймер", "Библиотека", "Гарнизон", "∑ HP"])

        # Pre-compute MageTimer per town (PRT GetTimerTown line 7069):
        #   MageTimer = OR of byte[1] of buildings array across all timed events
        #   for this town, EXCLUDING events with null Apply (which contribute 0).
        # The "Таймер" column on Города sheet shows "Да" if MageTimer != 0,
        # else None. This indicates the town has at least one timed event that
        # changes buildings via byte[1] bitmask (which contains Mage Guild
        # levels and similar "magic-related" buildings).
        town_timer_links = parsed.get("town_timer_links", []) or []
        mage_timer_by_town: Dict[int, int] = {}  # town_id → MageTimer bitmask
        for link in town_timer_links:
            link_dict = link if isinstance(link, dict) else {}
            town_id = link_dict.get("town_id", 0)
            mage_timer = 0
            for ev in link_dict.get("events", []):
                ev_dict = ev if isinstance(ev, dict) else {}
                # Check Apply — if null/zero, this event doesn't contribute
                # (PRT line 7064-7067: if IsApplyNull, reset num=0 for this event)
                apply_tuple = ev_dict.get("apply")
                if not apply_tuple or not isinstance(apply_tuple, (list, tuple)):
                    continue
                # Apply is (x_flags, y_num, z_num). Considered non-null if any
                # of the three is non-zero. (PRT checks IsApplyNull which is
                # True when the field was never set; we approximate by checking
                # all-zero tuple as "null".)
                if apply_tuple == (0, 0, 0):
                    continue
                buildings = ev_dict.get("buildings", [])
                if isinstance(buildings, (list, tuple)) and len(buildings) >= 2:
                    # byte[1] is at index 1 (0-based) — Mage Guild levels etc.
                    mage_timer |= int(buildings[1]) if buildings[1] else 0
            mage_timer_by_town[town_id] = mage_timer

        for town_idx, t in enumerate(parsed.get("towns_found", [])):
            f = t.get("fields", {})
            sp = f.get("spell_pool", {})
            levels = sp.get("levels", []) if isinstance(sp, dict) else []

            # Town type name
            ttype = f.get("type", 0) or 0
            ttype_name = _town_name(ttype) or f.get("type_name") or str(ttype)

            # Faction/color
            faction_id = f.get("faction", -1)
            faction_name = _color_name(faction_id) if faction_id is not None and faction_id >= 0 else (f.get("faction_name") or "")

            # Garrison: format as PRT "<name> <count>, ..." (with dedup, no "x" prefix)
            at = f.get("army_types", []) or []
            ac = f.get("army_counts", []) or []
            garrison_list = []
            for mt, ct in zip(at, ac):
                if isinstance(mt, int) and mt >= 0 and ct:
                    garrison_list.append({"monster_id": mt, "count": ct})
            garrison_str = _guard_str(garrison_list) if garrison_list else None

            # ∑ HP: computed by excel_calculations.town_sum_hp (Excel-only)
            sum_hp = town_sum_hp(t) if town_sum_hp else None

            # MageTimer — PRT GetTimerTown sets trow.MageTimer = bitmask.
            # The "Таймер" column shows "Да" if MageTimer != 0, else None.
            # town_timer_links.town_id = town's positional index in towns list
            # (matches `town_idx` from enumerate here, since both iterate
            # parsed.towns in the same order).
            mage_timer = mage_timer_by_town.get(town_idx, 0)
            timer_str = "Да" if mage_timer else None

            # Library: Tower (type=2) has Library by default → "Доступно"
            # PRT GetTownSpell: num5=1 for Tower, Library defaults to "Доступно" (aStatus[0])
            # unless bit 6 of (spell_off-14) is set → "Недоступно"
            library_str = None
            sp_addr = sp.get("address", 0) if isinstance(sp, dict) else 0
            if ttype == 2:
                if sp_addr and sp_addr - 14 >= 0 and sp_addr - 14 < len(raw):
                    lib_byte = raw[sp_addr - 14]
                    library_str = "Недоступно" if (lib_byte & 0x40) else "Доступно"
                else:
                    library_str = "Доступно"  # default for Tower

            # Read "Built" flags from byte at spell_off - 16
            # PRT: if (((decmp[num-16] >> i) & 1) > 0) row.Built = aReply[1] = "Да"
            built_byte = 0
            if sp_addr and sp_addr - 16 >= 0 and sp_addr - 16 < len(raw):
                built_byte = raw[sp_addr - 16]

            # Create 1 row per spell level
            level_num = 0
            for level_idx, level_spells in enumerate(levels):
                if not isinstance(level_spells, list) or not level_spells:
                    # Empty level — PRT deletes it, but if it's the first level,
                    # PRT shows it with Available="Да"
                    if level_idx == 0:
                        ws.append([f.get("x"), f.get("y"), f.get("z"),
                                   f.get("name", "?"), ttype_name, faction_name,
                                   1, None, None, "Да", None, None,
                                   garrison_str, sum_hp])
                    continue
                level_num = level_idx + 1
                # Spell names for this level
                spell_names = [_spl_name(sid) or str(sid) for sid in level_spells
                               if sid is not None and sid >= 0]
                spells_str = ", ".join(spell_names) if spell_names else None
                # First row gets Garrison + ∑ HP + Library; subsequent rows get None
                is_first = (level_idx == 0)
                # Built: bit i of built_byte → "Да" if set, else None
                built_str = "Да" if (built_byte >> level_idx) & 1 else None
                ws.append([f.get("x"), f.get("y"), f.get("z"),
                           f.get("name", "?"), ttype_name, faction_name,
                           level_num, spells_str,
                           built_str,
                           "Да",  # Доступен — default "Да" for existing levels
                           timer_str,  # Таймер — "Да" if MageTimer != 0, else None
                           library_str if is_first else None,
                           garrison_str if is_first else None,
                           sum_hp if is_first else None])
            # If no levels at all, emit one row with Available="Да"
            if level_num == 0:
                ws.append([f.get("x"), f.get("y"), f.get("z"),
                           f.get("name", "?"), ttype_name, faction_name,
                           1, None, None, "Да", timer_str, None,
                           garrison_str, sum_hp])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Арты (aggregator) ──
        ws = wb.create_sheet("Все Арты")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Слот", "Артефакт", "Класс", "Реликт-С", "Герой", "Кукла", "Флаг", "Местоположение", "Монстр", "Задание", "Охрана"])
        if aggregate_all_artifacts:
            for rec in aggregate_all_artifacts(parsed, localizer=loc):
                # For Hero artifacts: Кукла = doll slot name (e.g. "Разное1"), Слот = None.
                # For Market/Bank artifacts: Слот = numeric (1..7), Кукла = None.
                obj = rec.get("object")
                slot_val = rec.get("slot")
                doll_val = rec.get("doll_slot")
                if obj == "Герой":  # PRT name for hero artifacts
                    # Hero artifact: PRT shows doll slot NAME in Кукла, None in Слот
                    slot_name = None
                    doll_name = loc.doll(doll_val) if isinstance(doll_val, int) and doll_val >= 0 and loc else None
                elif isinstance(slot_val, int) and slot_val >= 0:
                    slot_name = slot_val  # numeric for markets/banks
                    doll_name = None
                else:
                    slot_name = None
                    doll_name = None
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           rec.get("locality"),
                           rec.get("object"), slot_name,
                           rec.get("artifact_name"),
                           rec.get("class_"), rec.get("relic"),
                           rec.get("hero"), doll_name, rec.get("flag"),
                           rec.get("place"), None, rec.get("mission"),
                           _guard_str(rec.get("guard")) if rec.get("guard") else None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Заклы (aggregator) ──
        ws = wb.create_sheet("Все Заклы")
        ws.append(["X", "Y", "Z", "Объект", "Заклинание", "Уровень", "Флаг", "Город", "Построен", "Гарнизон", "Герой", "Местоположение", "Задание", "Охрана"])
        if aggregate_all_spells:
            for rec in aggregate_all_spells(parsed, localizer=loc):
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           rec.get("object"), rec.get("spell_name"),
                           rec.get("level"), rec.get("flag"),
                           rec.get("town"), rec.get("built"),
                           rec.get("garrison"), rec.get("hero"),
                           rec.get("place"), rec.get("mission"),
                           _guard_str(rec.get("guard")) if rec.get("guard") else None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Навыки (aggregator) ──
        ws = wb.create_sheet("Все Навыки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Уровень", "Навык", "Задание", "Охрана"])
        if aggregate_all_skills:
            for rec in aggregate_all_skills(parsed, localizer=loc):
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           rec.get("locality"),
                           rec.get("object"), rec.get("level_name"),
                           rec.get("skill_name"), rec.get("mission"),
                           rec.get("guard")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Опыт (aggregator) ──
        ws = wb.create_sheet("Опыт")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Город", "Герой", "Флаг", "Монстр", "Артефакт", "Ресурс", "Охрана", "∑ HP", "Опыт", "∑ XP"])
        if aggregate_all_experience:
            for rec in aggregate_all_experience(parsed, localizer=loc):
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           rec.get("locality"),
                           rec.get("object"), rec.get("town"),
                           rec.get("hero"), rec.get("flag"),
                           rec.get("monster"), rec.get("artifact"),
                           rec.get("resource"), _guard_str(rec.get("guard")) if rec.get("guard") else None,
                           rec.get("hp"), rec.get("experience"),
                           rec.get("xp_total")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

    # ----- Save -----
    wb.save(output_path)
    return output_path
