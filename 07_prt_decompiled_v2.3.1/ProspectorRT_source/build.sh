#!/bin/bash
set -e
D="$(cd "$(dirname "$0")" && pwd)"
DOTNET="${DOTNET_SDK:-$HOME/dotnet/dotnet}"
CSC="$(find "$(dirname "$DOTNET")" -name csc.dll -path "*/Roslyn/bincore/*" 2>/dev/null | head -1)"
SYSREF="${MONO_PREFIX:-/usr}/lib/mono/2.0-api"
[ ! -f "$DOTNET" ] && echo "ERROR: dotnet not found" && exit 1
[ ! -f "$CSC" ] && echo "ERROR: csc.dll not found" && exit 1
[ ! -f "$SYSREF/mscorlib.dll" ] && echo "ERROR: mono-devel not installed" && exit 1
[ ! -d "$D/dlls" ] && echo "ERROR: dlls/ not found. Clone PRT_reverse, copy *.dll" && exit 1
CS="$D/source/ProspectorRT/CommonSetting.cs $D/source/ProspectorRT/DataSet2.cs $D/source/ProspectorRT/Registry.cs $D/source/ProspectorRT/myToolTip.cs $D/source/ProspectorRT/LMOracle.cs $D/source/ProspectorRT/SkillTreeForm.cs $D/source/ProspectorRT/ExportForm.cs $D/source/ProspectorRT/PrintForm.cs $D/source/ProspectorRT/ShowForm.cs $D/source/ProspectorRT/ObjectNameForm.cs $D/source/ProspectorRT/frmAbout.cs $D/source/ProspectorRT/MainForm.cs $D/source/ProspectorRT/TownNameValidator.cs $D/source/ProspectorRT/Program.cs $D/source/ProspectorRT.Properties/Resources.cs $D/source/ProspectorRT.Properties/Settings.cs $D/source/Properties/AssemblyInfo.cs $D/source/System.IO.Compression/ZipStorer.cs"
"$DOTNET" "$CSC" -target:winexe -out:"$D/ProspectorRT.exe" -langversion:10.0 -nostdlib -noconfig -win32icon:"$D/app.ico" \
 -r:"$SYSREF/mscorlib.dll" -r:"$SYSREF/System.dll" -r:"$SYSREF/System.Core.dll" -r:"$SYSREF/System.Data.dll" -r:"$SYSREF/System.Xml.dll" -r:"$SYSREF/System.Drawing.dll" -r:"$SYSREF/System.Windows.Forms.dll" \
 -r:"$D/dlls/DevExpress.Data.v11.1.dll" -r:"$D/dlls/DevExpress.Utils.v11.1.dll" -r:"$D/dlls/DevExpress.XtraBars.v11.1.dll" -r:"$D/dlls/DevExpress.XtraEditors.v11.1.dll" -r:"$D/dlls/DevExpress.XtraGrid.v11.1.dll" -r:"$D/dlls/DevExpress.XtraLayout.v11.1.dll" -r:"$D/dlls/DevExpress.XtraPrinting.v11.1.dll" -r:"$D/dlls/DevExpress.Printing.v11.1.Core.dll" -r:"$D/dlls/DevExpress.RichEdit.v11.1.Core.dll" -r:"$D/dlls/DevExpress.XtraRichEdit.v11.1.dll" -r:"$D/dlls/DevExpress.XtraTreeList.v11.1.dll" -r:"$D/dlls/SkinLib.dll" \
 -resource:"$D/resources/ProspectorRT.MainForm.resources,ProspectorRT.MainForm.resources" -resource:"$D/resources/ProspectorRT.PrintForm.resources,ProspectorRT.PrintForm.resources" -resource:"$D/resources/ProspectorRT.frmAbout.resources,ProspectorRT.frmAbout.resources" -resource:"$D/resources/ProspectorRT.Properties.Resources.resources,ProspectorRT.Properties.Resources.resources" -resource:"$D/resources/ProspectorRT.exe.licenses" -resource:"$D/resources/ProspectorRT.ExportForm.resources,ProspectorRT.ExportForm.resources" -resource:"$D/resources/ProspectorRT.myToolTip.resources,ProspectorRT.myToolTip.resources" -resource:"$D/resources/ProspectorRT.SkillTreeForm.resources,ProspectorRT.SkillTreeForm.resources" -resource:"$D/resources/ProspectorRT.ObjectNameForm.resources,ProspectorRT.ObjectNameForm.resources" -resource:"$D/resources/ProspectorRT.ShowForm.resources,ProspectorRT.ShowForm.resources" \
 $CS
echo "Done: $(ls -la "$D/ProspectorRT.exe")"
