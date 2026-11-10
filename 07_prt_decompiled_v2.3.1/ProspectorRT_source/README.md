# ProspectorRT v2.3.1 Source

## Patches in MainForm.cs
A: ScanTownsContent bounds-checked
B: GetTownContent bounds-checked + name validation
C: GetSenseRegion chain bounds-checked
D: ScanHeroesContent bounds-checked
E: ScanMonolithWhirlpool bounds-checked
H: Town section fallback scan (key fix for extended SoD maps)
I: SaveScroll safe casts
J: SafeSpellSlot helper (6 unsafe casts replaced)
FIXED1 1/2/3: R_AllArts + R_AllSpell + tavern fixes
Extended SoD: v31/32 accepted
Debug: try/catch each scanner + WriteDebugLog

## New file
TownNameValidator.cs

## Build
1. .NET SDK 6.0+ + Mono 6.12+ (mono-devel)
2. git clone https://github.com/AlexZ98765/PRT_reverse && cp PRT_reverse/*.dll dlls/
3. ./build.sh
