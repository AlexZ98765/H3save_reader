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
    def _mon_name(idx):
        return loc.monstr(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _spl_name(idx):
        return loc.spell(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _skl_name(idx):
        return loc.skill(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
    def _cls_name(idx):
        return loc.art_class(idx) if loc and isinstance(idx, int) and idx >= 0 else ""
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

    def _guard_str(guard_list):
        """Format guard list as 'Pikeman 5, Goblin 20' — matching PRT format.

        Per PRT MainForm.cs:9033-9065 (GetBankGuard):
          - 7 slots can have duplicate monster_ids
          - ItWas() deduplicates: if monster_id was already seen, sum the count
            into the existing entry instead of adding a new one
          - Final format: "<name> <count>, <name> <count>, ..." (no 'x' prefix)

        Example: 3 slots of Скелет (count 10 each) + 2 slots of Живой мертвец
        (count 10 each) → "Скелет 30, Живой мертвец 20"
        """
        if not guard_list:
            return None
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
                       rec.get("hp")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Монстры ──
        ws = wb.create_sheet("Монстры")
        ws.append(["X", "Y", "Z", "Район", "Монстр", "Количество", "Настроение", "Уровень", "∑ HP", "Артефакт", "Золото", "Ресурс", "Прирост"])
        for rec in mo.get("Монстры", []):
            mid = rec.get("monster_type", 0) or 0
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       _mon_name(mid) or mid, rec.get("count"),
                       rec.get("mood"), None, None,
                       _art_name(rec.get("artifact_id", -1)) if rec.get("has_artifact") else None,
                       None, None, rec.get("grows")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Банки ──
        ws = wb.create_sheet("Банки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Охрана", "∑ HP", "Монстр", "Золото", "Ресурс"])
        for rec in mo.get("Банки", []):
            guard_str = _guard_str(rec.get("guard", []))
            res_str = str(rec.get("resources", {})) if rec.get("resources") else None
            mid = rec.get("monster_reward_id", -1)
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Bank"), guard_str, None,
                       _mon_name(mid) if mid >= 0 else None,
                       rec.get("gold"), res_str])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── События и Ящики Пандоры ──
        ws = wb.create_sheet("События и Ящики Пандоры")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Охрана", "∑ HP", "Опыт", "Мана", "Мораль", "Удача", "Золото", "Ресурс", "Первичный навык", "Вторичный навык", "Артефакт", "Заклинание", "Монстр", "Доступно"])
        for rec in ptc.get("event_boxes", []):
            if isinstance(rec, dict):
                guard_str = _guard_str(rec.get("guard", []))
                res_str = str(rec.get("resources", {})) if rec.get("resources") else None
                ps_str = str(rec.get("primary_skills", {})) if rec.get("primary_skills") else None
                ss_str = _skl_list_str(rec.get("secondary_skills", [])) if rec.get("secondary_skills") else None
                # If secondary_skills is a list of dicts (skill_id+level), format them
                if rec.get("secondary_skills") and isinstance(rec.get("secondary_skills")[0], dict):
                    parts = []
                    for s in rec.get("secondary_skills", []):
                        if isinstance(s, dict):
                            sid = s.get('skill_id', -1)
                            lvl = s.get('level', 0)
                            if sid >= 0:
                                parts.append(f"{_skl_name(sid) or f'Sk{sid}'} (L{lvl})")
                    ss_str = ", ".join(parts) if parts else None
                art_str = _art_list_str(rec.get("artifacts", [])) if rec.get("artifacts") else None
                # If artifacts is list of dicts with artifact_id
                if rec.get("artifacts") and isinstance(rec.get("artifacts")[0], dict):
                    parts = []
                    for a in rec.get("artifacts", []):
                        if isinstance(a, dict):
                            aid = a.get('artifact_id', -1)
                            if aid >= 0:
                                n = _art_name(aid) or f"A{aid}"
                                parts.append(n)
                    art_str = ", ".join(parts) if parts else None
                spl_str = _spl_list_str(rec.get("spells", [])) if rec.get("spells") else None
                mon_str_parts = []
                if rec.get("monsters"):
                    for m in rec.get("monsters", []):
                        if isinstance(m, dict):
                            mid = m.get('monster_id', -1)
                            cnt = m.get('count', 0)
                            if mid >= 0:
                                mon_str_parts.append(f"{_mon_name(mid) or f'M{mid}'} x{cnt}")
                        elif isinstance(m, (list, tuple)) and len(m) >= 2:
                            mid, cnt = m[0], m[1]
                            if mid >= 0:
                                mon_str_parts.append(f"{_mon_name(mid) or f'M{mid}'} x{cnt}")
                mon_str = ", ".join(mon_str_parts) if mon_str_parts else None
                apply_str = str(rec.get("apply", ())) if rec.get("apply") else None
                ws.append([None, None, None, None, "Событие", guard_str, None,
                           rec.get("experience"), rec.get("mana"), rec.get("morale"), rec.get("luck"),
                           rec.get("resources", {}).get("Gold"), res_str,
                           ps_str, ss_str, art_str, spl_str, mon_str, apply_str])
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
                       rec.get("hp")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Сундуки ──
        ws = wb.create_sheet("Сундуки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Золото", "Опыт", "Артефакт", "Класс", "Реликт-С"])
        for rec in mo.get("Сундуки", []):
            aid = rec.get("artifact_id", -1) if rec.get("has_artifact") else -1
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Сундук"), rec.get("gold"), rec.get("experience"),
                       _art_name(aid) if aid >= 0 else None,
                       _cls_name(aid) if aid >= 0 else None, None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Заклинания ──
        ws = wb.create_sheet("Заклинания")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Заклинание", "Уровень", "Охрана", "∑ HP"])
        for rec in mo.get("Заклинания_Святилища", []):
            sid = rec.get("spell_id", 0) or 0
            shrine_name = rec.get("object") or "Святыня"
            spell_name = _spl_name(sid) or sid
            spell_level = rec.get("spell_level")
            if spell_level is None and loc:
                spell_level = loc.spell_level(sid)
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       shrine_name, spell_name, spell_level, None, None])
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
        ws = wb.create_sheet("Рынки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Слот", "Артефакт", "Класс", "Реликт-С", "Цена на Черном Рынке"])
        for rec in mo.get("Рынки", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Black Market"), None, None, None, None, None])
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
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       mission_str, reward_str, rec.get("deadline")])
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
                       None,                          # ∑ HP (calculation — see calculations_todo.txt)
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

        # ── Топология ──
        ws = wb.create_sheet("Топология")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Тип", "Цвет", "Пара"])
        for rec in mo.get("Топология_Палатки", []):
            cid = rec.get("color_id", 0) or 0
            # Always use localized tent name (it has the correct color order for tents)
            color_name = _tent_name(cid) or rec.get("color_name") or cid
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       "Палатка ключника", None, color_name, None])
        for rec in mo.get("Топология_Монолиты", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Монолит"), None, None, None])
        for rec in mo.get("Топология_Подземные_Врата", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       rec.get("object", "Врата"), None, None, rec.get("pair_id")])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── События-Таймеры ──
        ws = wb.create_sheet("События-Таймеры")
        ws.append(["Объект", "День", "Повтор", "Город", "Тип", "Местоположение", "Флаг", "Доступно", "Золото", "Ресурс", "Постройки", "Монстр"])
        for ev in ptc.get("map_timed_events", []):
            if isinstance(ev, dict):
                res = ev.get("resources", {}) or {}
                res_parts = []
                for rkey, rcount in res.items():
                    if rcount and rkey != "Gold":
                        # rkey may be like 'Wood', 'Mercury', etc. — try to lookup
                        try:
                            ridx = ['Wood', 'Mercury', 'Ore', 'Sulfur', 'Crystal', 'Gems'].index(rkey)
                            rname = _mine_name(ridx) or rkey
                        except ValueError:
                            rname = rkey
                        res_parts.append(f"{rname} x{rcount}")
                res_str = ", ".join(res_parts) if res_parts else None
                ws.append(["Карта", ev.get("day"), ev.get("repeat"), None, None, None, None, None,
                           res.get("Gold"), res_str, None, None])
        for ev in ptc.get("town_timed_events", []):
            if isinstance(ev, dict):
                res = ev.get("resources", {}) or {}
                res_parts = []
                for rkey, rcount in res.items():
                    if rcount and rkey != "Gold":
                        try:
                            ridx = ['Wood', 'Mercury', 'Ore', 'Sulfur', 'Crystal', 'Gems'].index(rkey)
                            rname = _mine_name(ridx) or rkey
                        except ValueError:
                            rname = rkey
                        res_parts.append(f"{rname} x{rcount}")
                res_str = ", ".join(res_parts) if res_parts else None
                bld_str = str(ev.get("buildings", [])) if ev.get("buildings") else None
                # Monsters may be list of dicts with monster_id+count
                mons = ev.get("monsters", []) or []
                if mons and isinstance(mons[0], dict):
                    mon_parts = []
                    for m in mons:
                        if isinstance(m, dict):
                            mid = m.get('monster_id', -1)
                            cnt = m.get('count', 0)
                            if mid >= 0:
                                mon_parts.append(f"{_mon_name(mid) or mid} x{cnt}")
                    mon_str = ", ".join(mon_parts) if mon_parts else None
                else:
                    mon_str = str(mons) if mons else None
                ws.append(["Город", ev.get("day"), ev.get("repeat"), ev.get("id"), None, None, None, None,
                           res.get("Gold"), res_str,
                           bld_str, mon_str])
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
            # In Excel (report): only show if Place != "Запрещен" (matches PRT).
            monster_str = h.get("monster")
            # Excel filter: hide monster for disabled heroes
            if place_str == "Запрещен":
                monster_str = None

            # Machine: from JSON
            machines_str = h.get("machine")

            # Book: from JSON
            book_str = h.get("book")

            # MP: movement_total
            mp = f.get("movement_total")

            ws.append([i, f.get("name", "?"), place_str, flag_str,
                       f.get("level"), f.get("experience"),
                       primary_str, secondary_str, equip_str, spells_str,
                       monster_str, None, machines_str, book_str, mp])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Города ──
        ws = wb.create_sheet("Города")
        ws.append(["X", "Y", "Z", "Город", "Тип", "Флаг", "Уровень", "Заклинание", "Построен", "Доступен", "Таймер", "Библиотека", "Гарнизон", "∑ HP"])
        for t in parsed.get("towns_found", []):
            f = t.get("fields", {})
            sp = f.get("spell_pool", {})
            levels = sp.get("levels", []) if isinstance(sp, dict) else []
            # Spell pool levels is list of lists of spell IDs
            spell_names_per_level = []
            for lvl in levels:
                if isinstance(lvl, list):
                    lvl_names = [_spl_name(sid) or str(sid) for sid in lvl if sid is not None and sid >= 0]
                    if lvl_names:
                        spell_names_per_level.append(", ".join(lvl_names))
            spells_str = " | ".join(spell_names_per_level) if spell_names_per_level else None
            # Army: types+counts as 'Pikeman x10, Archer x5'
            at = f.get("army_types", []) or []
            ac = f.get("army_counts", []) or []
            army_parts = []
            for mt, ct in zip(at, ac):
                if isinstance(mt, int) and mt >= 0 and ct:
                    army_parts.append(f"{_mon_name(mt) or mt} x{ct}")
            army_str = ", ".join(army_parts) if army_parts else None
            # Town type name: prefer localized name, fall back to existing type_name
            ttype = f.get("type", 0) or 0
            loc_town = _town_name(ttype)
            ttype_name = loc_town or f.get("type_name") or str(ttype)
            # Faction name: prefer localized name, fall back to existing faction_name
            faction_id = f.get("faction", -1)
            loc_color = _color_name(faction_id) if faction_id is not None and faction_id >= 0 else ""
            faction_name = loc_color or f.get("faction_name") or ""
            ws.append([f.get("x"), f.get("y"), f.get("z"),
                       f.get("name", "?"), ttype_name,
                       faction_name, None, spells_str,
                       None, None, None, None, army_str, None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Арты (aggregator) ──
        ws = wb.create_sheet("Все Арты")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Слот", "Артефакт", "Класс", "Реликт-С", "Герой", "Кукла", "Флаг", "Местоположение", "Монстр", "Задание", "Охрана"])
        aggr = parsed.get("aggregators", {})
        for rec in aggr.get("all_artifacts", []):
            if isinstance(rec, dict):
                aid = rec.get("artifact_id", -1) or -1
                slot = rec.get("slot", -1)
                # Doll slot name lookup
                slot_name = None
                if isinstance(slot, int) and slot >= 0:
                    slot_name = loc.doll(slot) if loc else None
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           "суша" if rec.get("locality", "суша") != "море" else "море",
                           rec.get("object"), slot_name,
                           _art_name(aid) or aid if aid >= 0 else None,
                           _cls_name(aid) if aid >= 0 else None, None,
                           rec.get("hero"), None, rec.get("flag"),
                           _place_name(rec.get("place", -1)) if rec.get("place") is not None else None,
                           None, rec.get("mission"), None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Заклы (aggregator) ──
        ws = wb.create_sheet("Все Заклы")
        ws.append(["X", "Y", "Z", "Объект", "Заклинание", "Уровень", "Флаг", "Город", "Построен", "Гарнизон", "Герой", "Местоположение", "Задание", "Охрана"])
        for rec in aggr.get("all_spells", []):
            if isinstance(rec, dict):
                sid = rec.get("spell_id", -1) or -1
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           rec.get("object"), _spl_name(sid) or sid if sid >= 0 else None, None, None,
                           None, None, None, None, None, None, None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Все Навыки (aggregator) ──
        ws = wb.create_sheet("Все Навыки")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Уровень", "Навык", "Задание", "Охрана"])
        for rec in aggr.get("all_skills", []):
            if isinstance(rec, dict):
                skid = rec.get("skill_id", -1) or -1
                lvl = rec.get("level", -1)
                level_name = loc.full_level_skill(lvl) if loc and isinstance(lvl, int) and lvl >= 0 else None
                ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                           "суша" if rec.get("locality", "суша") != "море" else "море",
                           rec.get("object"), level_name,
                           _skl_name(skid) or skid if skid >= 0 else None, None, None])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

        # ── Опыт (aggregator) ──
        ws = wb.create_sheet("Опыт")
        ws.append(["X", "Y", "Z", "Район", "Объект", "Город", "Герой", "Флаг", "Монстр", "Артефакт", "Ресурс", "Охрана", "∑ HP", "Опыт", "∑ XP"])
        # Experience aggregator is optional — populate from Learning Stones
        for rec in mo.get("Learning_Stones", []):
            ws.append([rec.get("x"), rec.get("y"), rec.get("z"),
                       "суша" if rec.get("locality", "суша") != "море" else "море",
                       "Камень знаний", None, None, None, None, None, None, None, None,
                       rec.get("experience", 1000), rec.get("experience", 1000)])
        ws.freeze_panes = "A2"
        ws.auto_filter.ref = ws.dimensions
        autosize_columns(ws)

    # ----- Save -----
    wb.save(output_path)
    return output_path
