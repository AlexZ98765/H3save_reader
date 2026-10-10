using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.UserSkins;

namespace ProspectorRT;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
		Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
		SkinManager.Default.RegisterAssembly(typeof(SkinProject1).Assembly);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		bool st = false;
		bool ot = false;
		if (args.Length > 0)
		{
			if (args.Length == 1)
			{
				args[0] = args[0].Replace("/", "");
				if (args[0].ToUpper() == "OT3" || args[0].ToUpper() == "OT-3")
				{
					ot = true;
				}
				else if (args[0].ToUpper() == "SPT")
				{
					st = true;
				}
			}
			else if (args.Length == 2)
			{
				args[0] = args[0].Replace("/", "");
				args[1] = args[1].Replace("/", "");
				if (args[0].ToUpper() == "OT3" || args[0].ToUpper() == "OT-3")
				{
					ot = true;
				}
				if (args[1].ToUpper() == "OT3" || args[1].ToUpper() == "OT-3")
				{
					ot = true;
				}
				if (args[0].ToUpper() == "SPT")
				{
					st = true;
				}
				if (args[1].ToUpper() == "SPT")
				{
					st = true;
				}
			}
		}
		Application.Run((Form)(object)new MainForm(ot, st));
	}
}
