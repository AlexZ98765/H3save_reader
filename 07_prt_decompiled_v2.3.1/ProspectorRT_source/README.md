# ProspectorRT v2.3.1 — Source + Build

## Structure
```
ProspectorRT_source/
├── app.ico                    ← application icon
├── ProspectorRT.csproj        ← Visual Studio project file
├── build.sh                   ← Linux build script
├── README.md                  ← this file
├── source/                    ← C# source files
│   ├── ProspectorRT/          ← MainForm.cs (patched), TownNameValidator.cs (new), etc.
│   ├── ProspectorRT.Properties/
│   ├── Properties/            ← AssemblyInfo.cs (v2.3.1.0)
│   └── System.IO.Compression/
├── resources/                 ← .resources + .resx files (embedded into exe)
└── dlls/                      ← DevExpress v11.1 DLLs (INCLUDED, no download needed)
```

## Build (Linux)
```bash
# 1. Install .NET SDK 6.0
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version 6.0.428

# 2. Install Mono
sudo apt install mono-devel

# 3. Build
export DOTNET_SDK=~/dotnet/dotnet
cd ProspectorRT_source
./build.sh

# Result: ProspectorRT.exe (with icon, ~824 KB)
```

## Build (Windows / Visual Studio)
1. Open `ProspectorRT.csproj` in Visual Studio 2022
2. Set target framework to .NET Framework 2.0
3. Build → produces ProspectorRT.exe

## Build (Windows / command line)
```
dotnet build ProspectorRT.csproj
```

## DLLs included
All DevExpress v11.1 DLLs are in `dlls/` — no need to download from another repo:
- DevExpress.Data.v11.1.dll
- DevExpress.Utils.v11.1.dll
- DevExpress.XtraBars.v11.1.dll
- DevExpress.XtraEditors.v11.1.dll
- DevExpress.XtraGrid.v11.1.dll
- DevExpress.XtraLayout.v11.1.dll
- DevExpress.XtraPrinting.v11.1.dll
- DevExpress.Printing.v11.1.Core.dll
- DevExpress.RichEdit.v11.1.Core.dll
- DevExpress.XtraRichEdit.v11.1.dll
- DevExpress.XtraTreeList.v11.1.dll
- SkinLib.dll

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
Extended SoD: version 31/32 accepted
Debug: try/catch each scanner + WriteDebugLog

## Verify
```bash
monodis --assembly ProspectorRT.exe | grep Version    # 2.3.1.0
monodis --method ProspectorRT.exe | grep -E "FindTownSection|WriteDebugLog|SafeSpellSlot"
```
