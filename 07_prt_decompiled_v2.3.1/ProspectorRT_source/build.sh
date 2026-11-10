#!/bin/bash
set -e
D="$(cd "$(dirname "$0")" && pwd)"

DOTNET="${DOTNET_SDK:-$HOME/dotnet/dotnet}"
CSC="$(find "$(dirname "$DOTNET")" -name csc.dll -path "*/Roslyn/bincore/*" 2>/dev/null | head -1)"
SYSREF="${MONO_PREFIX:-/usr}/lib/mono/2.0-api"
DLLS="$D/dlls"
RES="$D/resources"
OUT="$D/ProspectorRT.exe"

[ ! -f "$DOTNET" ] && echo "ERROR: .NET SDK 6.0+ not found. Install: curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version 6.0.428" && exit 1
[ ! -f "$CSC" ] && echo "ERROR: csc.dll not found" && exit 1
[ ! -f "$SYSREF/mscorlib.dll" ] && echo "ERROR: Mono not found. Install: sudo apt install mono-devel" && exit 1

echo "=== Building ProspectorRT v2.3.1 ==="

CS="$D/ProspectorRT/CommonSetting.cs $D/ProspectorRT/DataSet2.cs $D/ProspectorRT/Registry.cs $D/ProspectorRT/myToolTip.cs $D/ProspectorRT/LMOracle.cs $D/ProspectorRT/SkillTreeForm.cs $D/ProspectorRT/ExportForm.cs $D/ProspectorRT/PrintForm.cs $D/ProspectorRT/ShowForm.cs $D/ProspectorRT/ObjectNameForm.cs $D/ProspectorRT/frmAbout.cs $D/ProspectorRT/MainForm.cs $D/ProspectorRT/TownNameValidator.cs $D/ProspectorRT/Program.cs $D/ProspectorRT.Properties/Resources.cs $D/ProspectorRT.Properties/Settings.cs $D/Properties/AssemblyInfo.cs $D/System.IO.Compression/ZipStorer.cs"

"$DOTNET" "$CSC" \
    -target:winexe -out:"$OUT" -langversion:10.0 \
    -nostdlib -noconfig -win32icon:"$D/app.ico" \
    -r:"$SYSREF/mscorlib.dll" -r:"$SYSREF/System.dll" -r:"$SYSREF/System.Core.dll" \
    -r:"$SYSREF/System.Data.dll" -r:"$SYSREF/System.Xml.dll" \
    -r:"$SYSREF/System.Drawing.dll" -r:"$SYSREF/System.Windows.Forms.dll" \
    -r:"$DLLS/DevExpress.Data.v11.1.dll" -r:"$DLLS/DevExpress.Utils.v11.1.dll" \
    -r:"$DLLS/DevExpress.XtraBars.v11.1.dll" -r:"$DLLS/DevExpress.XtraEditors.v11.1.dll" \
    -r:"$DLLS/DevExpress.XtraGrid.v11.1.dll" -r:"$DLLS/DevExpress.XtraLayout.v11.1.dll" \
    -r:"$DLLS/DevExpress.XtraPrinting.v11.1.dll" -r:"$DLLS/DevExpress.Printing.v11.1.Core.dll" \
    -r:"$DLLS/DevExpress.RichEdit.v11.1.Core.dll" -r:"$DLLS/DevExpress.XtraRichEdit.v11.1.dll" \
    -r:"$DLLS/DevExpress.XtraTreeList.v11.1.dll" -r:"$DLLS/SkinLib.dll" \
    -resource:"$RES/ProspectorRT.MainForm.resources,ProspectorRT.MainForm.resources" \
    -resource:"$RES/ProspectorRT.PrintForm.resources,ProspectorRT.PrintForm.resources" \
    -resource:"$RES/ProspectorRT.frmAbout.resources,ProspectorRT.frmAbout.resources" \
    -resource:"$RES/ProspectorRT.Properties.Resources.resources,ProspectorRT.Properties.Resources.resources" \
    -resource:"$RES/ProspectorRT.exe.licenses" \
    -resource:"$RES/ProspectorRT.ExportForm.resources,ProspectorRT.ExportForm.resources" \
    -resource:"$RES/ProspectorRT.myToolTip.resources,ProspectorRT.myToolTip.resources" \
    -resource:"$RES/ProspectorRT.SkillTreeForm.resources,ProspectorRT.SkillTreeForm.resources" \
    -resource:"$RES/ProspectorRT.ObjectNameForm.resources,ProspectorRT.ObjectNameForm.resources" \
    -resource:"$RES/ProspectorRT.ShowForm.resources,ProspectorRT.ShowForm.resources" \
    $CS

echo ""
echo "=== Build complete ==="
ls -la "$OUT"
