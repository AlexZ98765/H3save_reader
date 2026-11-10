using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview;

namespace ProspectorRT;

public class PrintForm : Form
{
	private IContainer components;

	private PreviewBar previewBar1;

	private PrintPreviewBarItem printPreviewBarItem2;

	private PrintPreviewBarItem printPreviewBarItem3;

	private PrintPreviewBarItem printPreviewBarItem4;

	private PrintPreviewBarItem printPreviewBarItem5;

	private PrintPreviewBarItem printPreviewBarItem6;

	private PrintPreviewBarItem printPreviewBarItem7;

	private PrintPreviewBarItem printPreviewBarItem8;

	private PrintPreviewBarItem printPreviewBarItem9;

	private PrintPreviewBarItem printPreviewBarItem10;

	private PrintPreviewBarItem printPreviewBarItem11;

	private PrintPreviewBarItem printPreviewBarItem12;

	private PrintPreviewBarItem printPreviewBarItem13;

	private PrintPreviewBarItem printPreviewBarItem14;

	private PrintPreviewBarItem printPreviewBarItem15;

	private ZoomBarEditItem zoomBarEditItem1;

	private PrintPreviewRepositoryItemComboBox printPreviewRepositoryItemComboBox1;

	private PrintPreviewBarItem printPreviewBarItem16;

	private PrintPreviewBarItem printPreviewBarItem17;

	private PrintPreviewBarItem printPreviewBarItem18;

	private PrintPreviewBarItem printPreviewBarItem19;

	private PrintPreviewBarItem printPreviewBarItem20;

	private PrintPreviewBarItem printPreviewBarItem21;

	private PrintPreviewBarItem printPreviewBarItem22;

	private PrintPreviewBarItem printPreviewBarItem23;

	private PrintPreviewBarItem printPreviewBarItem24;

	private PrintPreviewBarItem printPreviewBarItem25;

	private PrintPreviewBarItem printPreviewBarItem26;

	private PreviewBar previewBar2;

	private PrintPreviewStaticItem printPreviewStaticItem1;

	private BarStaticItem barStaticItem1;

	private ProgressBarEditItem progressBarEditItem1;

	private RepositoryItemProgressBar repositoryItemProgressBar1;

	private PrintPreviewBarItem printPreviewBarItem1;

	private BarButtonItem barButtonItem1;

	private PrintPreviewStaticItem printPreviewStaticItem2;

	private ZoomTrackBarEditItem zoomTrackBarEditItem1;

	private RepositoryItemZoomTrackBar repositoryItemZoomTrackBar1;

	private PreviewBar previewBar3;

	private PrintPreviewSubItem printPreviewSubItem1;

	private PrintPreviewSubItem printPreviewSubItem2;

	private PrintPreviewSubItem printPreviewSubItem4;

	private PrintPreviewBarItem printPreviewBarItem27;

	private PrintPreviewBarItem printPreviewBarItem28;

	private BarToolbarsListItem barToolbarsListItem1;

	private PrintPreviewSubItem printPreviewSubItem3;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private PrintControl printControl1;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem1;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem2;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem3;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem4;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem5;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem6;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem7;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem8;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem9;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem10;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem11;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem12;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem13;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem14;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem15;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem16;

	private PrintPreviewBarCheckItem printPreviewBarCheckItem17;

	public PrintingSystem printingSystem1;

	public PrintBarManager printBarManager1;

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_245c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2466: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PrintForm));
		printingSystem1 = new PrintingSystem(components);
		printBarManager1 = new PrintBarManager(components);
		previewBar1 = new PreviewBar();
		printPreviewBarItem2 = new PrintPreviewBarItem();
		printPreviewBarItem3 = new PrintPreviewBarItem();
		printPreviewBarItem24 = new PrintPreviewBarItem();
		printPreviewBarItem6 = new PrintPreviewBarItem();
		printPreviewBarItem7 = new PrintPreviewBarItem();
		printPreviewBarItem8 = new PrintPreviewBarItem();
		printPreviewBarItem10 = new PrintPreviewBarItem();
		printPreviewBarItem11 = new PrintPreviewBarItem();
		printPreviewBarItem12 = new PrintPreviewBarItem();
		printPreviewBarItem4 = new PrintPreviewBarItem();
		printPreviewBarItem13 = new PrintPreviewBarItem();
		printPreviewBarItem14 = new PrintPreviewBarItem();
		printPreviewBarItem15 = new PrintPreviewBarItem();
		zoomBarEditItem1 = new ZoomBarEditItem();
		printPreviewRepositoryItemComboBox1 = new PrintPreviewRepositoryItemComboBox();
		printPreviewBarItem16 = new PrintPreviewBarItem();
		printPreviewBarItem17 = new PrintPreviewBarItem();
		printPreviewBarItem18 = new PrintPreviewBarItem();
		printPreviewBarItem19 = new PrintPreviewBarItem();
		printPreviewBarItem20 = new PrintPreviewBarItem();
		printPreviewBarItem21 = new PrintPreviewBarItem();
		printPreviewBarItem23 = new PrintPreviewBarItem();
		printPreviewBarItem26 = new PrintPreviewBarItem();
		previewBar2 = new PreviewBar();
		printPreviewStaticItem1 = new PrintPreviewStaticItem();
		barStaticItem1 = new BarStaticItem();
		progressBarEditItem1 = new ProgressBarEditItem();
		repositoryItemProgressBar1 = new RepositoryItemProgressBar();
		printPreviewBarItem1 = new PrintPreviewBarItem();
		barButtonItem1 = new BarButtonItem();
		printPreviewStaticItem2 = new PrintPreviewStaticItem();
		zoomTrackBarEditItem1 = new ZoomTrackBarEditItem();
		repositoryItemZoomTrackBar1 = new RepositoryItemZoomTrackBar();
		previewBar3 = new PreviewBar();
		printPreviewSubItem1 = new PrintPreviewSubItem();
		printPreviewSubItem2 = new PrintPreviewSubItem();
		printPreviewSubItem4 = new PrintPreviewSubItem();
		printPreviewBarItem27 = new PrintPreviewBarItem();
		printPreviewBarItem28 = new PrintPreviewBarItem();
		barToolbarsListItem1 = new BarToolbarsListItem();
		printPreviewSubItem3 = new PrintPreviewSubItem();
		barDockControlTop = new BarDockControl();
		barDockControlBottom = new BarDockControl();
		barDockControlLeft = new BarDockControl();
		barDockControlRight = new BarDockControl();
		printPreviewBarItem5 = new PrintPreviewBarItem();
		printPreviewBarItem9 = new PrintPreviewBarItem();
		printPreviewBarItem22 = new PrintPreviewBarItem();
		printPreviewBarItem25 = new PrintPreviewBarItem();
		printPreviewBarCheckItem1 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem2 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem3 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem4 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem5 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem6 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem7 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem8 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem9 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem10 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem11 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem12 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem13 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem14 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem15 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem16 = new PrintPreviewBarCheckItem();
		printPreviewBarCheckItem17 = new PrintPreviewBarCheckItem();
		printControl1 = new PrintControl();
		((ISupportInitialize)printingSystem1).BeginInit();
		((ISupportInitialize)printBarManager1).BeginInit();
		((ISupportInitialize)printPreviewRepositoryItemComboBox1).BeginInit();
		((ISupportInitialize)repositoryItemProgressBar1).BeginInit();
		((ISupportInitialize)repositoryItemZoomTrackBar1).BeginInit();
		((Control)this).SuspendLayout();
		printBarManager1.AllowCustomization = false;
		printBarManager1.AllowQuickCustomization = false;
		printBarManager1.Bars.AddRange(new Bar[3] { previewBar1, previewBar2, previewBar3 });
		printBarManager1.DockControls.Add(barDockControlTop);
		printBarManager1.DockControls.Add(barDockControlBottom);
		printBarManager1.DockControls.Add(barDockControlLeft);
		printBarManager1.DockControls.Add(barDockControlRight);
		printBarManager1.Form = (Control)(object)this;
		printBarManager1.ImageStream = (ImageCollectionStreamer)componentResourceManager.GetObject("printBarManager1.ImageStream");
		printBarManager1.Items.AddRange(new BarItem[57]
		{
			printPreviewStaticItem1, barStaticItem1, progressBarEditItem1, printPreviewBarItem1, barButtonItem1, printPreviewStaticItem2, zoomTrackBarEditItem1, printPreviewBarItem2, printPreviewBarItem3, printPreviewBarItem4,
			printPreviewBarItem5, printPreviewBarItem6, printPreviewBarItem7, printPreviewBarItem8, printPreviewBarItem9, printPreviewBarItem10, printPreviewBarItem11, printPreviewBarItem12, printPreviewBarItem13, printPreviewBarItem14,
			printPreviewBarItem15, zoomBarEditItem1, printPreviewBarItem16, printPreviewBarItem17, printPreviewBarItem18, printPreviewBarItem19, printPreviewBarItem20, printPreviewBarItem21, printPreviewBarItem22, printPreviewBarItem23,
			printPreviewBarItem24, printPreviewBarItem25, printPreviewBarItem26, printPreviewSubItem1, printPreviewSubItem2, printPreviewSubItem3, printPreviewSubItem4, printPreviewBarItem27, printPreviewBarItem28, barToolbarsListItem1,
			printPreviewBarCheckItem1, printPreviewBarCheckItem2, printPreviewBarCheckItem3, printPreviewBarCheckItem4, printPreviewBarCheckItem5, printPreviewBarCheckItem6, printPreviewBarCheckItem7, printPreviewBarCheckItem8, printPreviewBarCheckItem9, printPreviewBarCheckItem10,
			printPreviewBarCheckItem11, printPreviewBarCheckItem12, printPreviewBarCheckItem13, printPreviewBarCheckItem14, printPreviewBarCheckItem15, printPreviewBarCheckItem16, printPreviewBarCheckItem17
		});
		printBarManager1.MainMenu = previewBar3;
		printBarManager1.MaxItemId = 58;
		printBarManager1.PreviewBar = previewBar1;
		printBarManager1.PrintControl = printControl1;
		printBarManager1.RepositoryItems.AddRange(new RepositoryItem[3] { repositoryItemProgressBar1, repositoryItemZoomTrackBar1, printPreviewRepositoryItemComboBox1 });
		printBarManager1.StatusBar = previewBar2;
		printBarManager1.TransparentEditors = true;
		previewBar1.BarName = "Toolbar";
		previewBar1.CanDockStyle = BarCanDockStyle.Top;
		previewBar1.DockCol = 0;
		previewBar1.DockRow = 1;
		previewBar1.DockStyle = BarDockStyle.Top;
		previewBar1.LinksPersistInfo.AddRange(new LinkPersistInfo[22]
		{
			new LinkPersistInfo(printPreviewBarItem2),
			new LinkPersistInfo(printPreviewBarItem3),
			new LinkPersistInfo(printPreviewBarItem24),
			new LinkPersistInfo(printPreviewBarItem6, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem7),
			new LinkPersistInfo(printPreviewBarItem8, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem10),
			new LinkPersistInfo(printPreviewBarItem11),
			new LinkPersistInfo(printPreviewBarItem12),
			new LinkPersistInfo(printPreviewBarItem4, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem13, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem14),
			new LinkPersistInfo(printPreviewBarItem15, beginGroup: true),
			new LinkPersistInfo(zoomBarEditItem1),
			new LinkPersistInfo(printPreviewBarItem16),
			new LinkPersistInfo(printPreviewBarItem17, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem18),
			new LinkPersistInfo(printPreviewBarItem19),
			new LinkPersistInfo(printPreviewBarItem20),
			new LinkPersistInfo(printPreviewBarItem21, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem23, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem26, beginGroup: true)
		});
		previewBar1.OptionsBar.AllowQuickCustomization = false;
		previewBar1.OptionsBar.DisableClose = true;
		previewBar1.OptionsBar.DisableCustomization = true;
		previewBar1.OptionsBar.UseWholeRow = true;
		previewBar1.Text = "Toolbar";
		printPreviewBarItem2.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem2.Caption = "Document Map";
		printPreviewBarItem2.Command = PrintingSystemCommand.DocumentMap;
		printPreviewBarItem2.Enabled = false;
		printPreviewBarItem2.Hint = "Document Map";
		printPreviewBarItem2.Id = 7;
		printPreviewBarItem2.ImageIndex = 19;
		printPreviewBarItem2.Name = "printPreviewBarItem2";
		printPreviewBarItem2.Visibility = BarItemVisibility.Never;
		printPreviewBarItem3.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem3.Caption = "Parameters";
		printPreviewBarItem3.Command = PrintingSystemCommand.Parameters;
		printPreviewBarItem3.Enabled = false;
		printPreviewBarItem3.Hint = "Parameters";
		printPreviewBarItem3.Id = 8;
		printPreviewBarItem3.ImageIndex = 22;
		printPreviewBarItem3.Name = "printPreviewBarItem3";
		printPreviewBarItem3.Visibility = BarItemVisibility.Never;
		printPreviewBarItem24.ButtonStyle = BarButtonStyle.DropDown;
		printPreviewBarItem24.Caption = "&Экспорт...";
		printPreviewBarItem24.Command = PrintingSystemCommand.ExportFile;
		printPreviewBarItem24.Enabled = false;
		printPreviewBarItem24.Hint = "Экспорт...";
		printPreviewBarItem24.Id = 30;
		printPreviewBarItem24.ImageIndex = 18;
		printPreviewBarItem24.Name = "printPreviewBarItem24";
		printPreviewBarItem6.Caption = "Открыть";
		printPreviewBarItem6.Command = PrintingSystemCommand.Open;
		printPreviewBarItem6.Hint = "Открыть";
		printPreviewBarItem6.Id = 11;
		printPreviewBarItem6.ImageIndex = 23;
		printPreviewBarItem6.Name = "printPreviewBarItem6";
		printPreviewBarItem7.Caption = "Сохранить";
		printPreviewBarItem7.Command = PrintingSystemCommand.Save;
		printPreviewBarItem7.Enabled = false;
		printPreviewBarItem7.Hint = "Сохранить";
		printPreviewBarItem7.Id = 12;
		printPreviewBarItem7.ImageIndex = 24;
		printPreviewBarItem7.Name = "printPreviewBarItem7";
		printPreviewBarItem8.Caption = "&Печать...";
		printPreviewBarItem8.Command = PrintingSystemCommand.Print;
		printPreviewBarItem8.Enabled = false;
		printPreviewBarItem8.Hint = "Печать...";
		printPreviewBarItem8.Id = 13;
		printPreviewBarItem8.ImageIndex = 1;
		printPreviewBarItem8.Name = "printPreviewBarItem8";
		printPreviewBarItem10.Caption = "Па&раметры страницы...";
		printPreviewBarItem10.Command = PrintingSystemCommand.PageSetup;
		printPreviewBarItem10.Enabled = false;
		printPreviewBarItem10.Hint = "Параметры страницы...";
		printPreviewBarItem10.Id = 15;
		printPreviewBarItem10.ImageIndex = 2;
		printPreviewBarItem10.Name = "printPreviewBarItem10";
		printPreviewBarItem11.Caption = "Колонтитулы";
		printPreviewBarItem11.Command = PrintingSystemCommand.EditPageHF;
		printPreviewBarItem11.Enabled = false;
		printPreviewBarItem11.Hint = "Колонтитулы";
		printPreviewBarItem11.Id = 16;
		printPreviewBarItem11.ImageIndex = 15;
		printPreviewBarItem11.Name = "printPreviewBarItem11";
		printPreviewBarItem12.ActAsDropDown = true;
		printPreviewBarItem12.ButtonStyle = BarButtonStyle.DropDown;
		printPreviewBarItem12.Caption = "Масштаб";
		printPreviewBarItem12.Command = PrintingSystemCommand.Scale;
		printPreviewBarItem12.Enabled = false;
		printPreviewBarItem12.Hint = "Масштаб";
		printPreviewBarItem12.Id = 17;
		printPreviewBarItem12.ImageIndex = 25;
		printPreviewBarItem12.Name = "printPreviewBarItem12";
		printPreviewBarItem4.Caption = "Найти";
		printPreviewBarItem4.Command = PrintingSystemCommand.Find;
		printPreviewBarItem4.Enabled = false;
		printPreviewBarItem4.Hint = "Найти";
		printPreviewBarItem4.Id = 9;
		printPreviewBarItem4.ImageIndex = 20;
		printPreviewBarItem4.Name = "printPreviewBarItem4";
		printPreviewBarItem13.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem13.Caption = "Перемещать";
		printPreviewBarItem13.Command = PrintingSystemCommand.HandTool;
		printPreviewBarItem13.Enabled = false;
		printPreviewBarItem13.Hint = "Перемещать";
		printPreviewBarItem13.Id = 18;
		printPreviewBarItem13.ImageIndex = 16;
		printPreviewBarItem13.Name = "printPreviewBarItem13";
		printPreviewBarItem14.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem14.Caption = "Лупа";
		printPreviewBarItem14.Command = PrintingSystemCommand.Magnifier;
		printPreviewBarItem14.Enabled = false;
		printPreviewBarItem14.Hint = "Лупа";
		printPreviewBarItem14.Id = 19;
		printPreviewBarItem14.ImageIndex = 3;
		printPreviewBarItem14.Name = "printPreviewBarItem14";
		printPreviewBarItem15.Caption = "Уменьшить";
		printPreviewBarItem15.Command = PrintingSystemCommand.ZoomOut;
		printPreviewBarItem15.Enabled = false;
		printPreviewBarItem15.Hint = "Уменьшить";
		printPreviewBarItem15.Id = 20;
		printPreviewBarItem15.ImageIndex = 5;
		printPreviewBarItem15.Name = "printPreviewBarItem15";
		zoomBarEditItem1.Caption = "Изменить";
		zoomBarEditItem1.Edit = printPreviewRepositoryItemComboBox1;
		zoomBarEditItem1.EditValue = "100%";
		zoomBarEditItem1.Enabled = false;
		zoomBarEditItem1.Hint = "Изменить";
		zoomBarEditItem1.Id = 21;
		zoomBarEditItem1.Name = "zoomBarEditItem1";
		zoomBarEditItem1.Width = 70;
		printPreviewRepositoryItemComboBox1.AutoComplete = false;
		printPreviewRepositoryItemComboBox1.Buttons.AddRange(new EditorButton[1]
		{
			new EditorButton(ButtonPredefines.Combo)
		});
		printPreviewRepositoryItemComboBox1.DropDownRows = 11;
		printPreviewRepositoryItemComboBox1.Name = "printPreviewRepositoryItemComboBox1";
		printPreviewRepositoryItemComboBox1.UseParentBackground = true;
		printPreviewBarItem16.Caption = "Увеличить";
		printPreviewBarItem16.Command = PrintingSystemCommand.ZoomIn;
		printPreviewBarItem16.Enabled = false;
		printPreviewBarItem16.Hint = "Увеличить";
		printPreviewBarItem16.Id = 22;
		printPreviewBarItem16.ImageIndex = 4;
		printPreviewBarItem16.Name = "printPreviewBarItem16";
		printPreviewBarItem17.Caption = "Первая страница";
		printPreviewBarItem17.Command = PrintingSystemCommand.ShowFirstPage;
		printPreviewBarItem17.Enabled = false;
		printPreviewBarItem17.Hint = "Первая страница";
		printPreviewBarItem17.Id = 23;
		printPreviewBarItem17.ImageIndex = 7;
		printPreviewBarItem17.Name = "printPreviewBarItem17";
		printPreviewBarItem18.Caption = "Предыдущая страница";
		printPreviewBarItem18.Command = PrintingSystemCommand.ShowPrevPage;
		printPreviewBarItem18.Enabled = false;
		printPreviewBarItem18.Hint = "Предыдущая страница";
		printPreviewBarItem18.Id = 24;
		printPreviewBarItem18.ImageIndex = 8;
		printPreviewBarItem18.Name = "printPreviewBarItem18";
		printPreviewBarItem19.Caption = "Следующая страница";
		printPreviewBarItem19.Command = PrintingSystemCommand.ShowNextPage;
		printPreviewBarItem19.Enabled = false;
		printPreviewBarItem19.Hint = "Следующая страница";
		printPreviewBarItem19.Id = 25;
		printPreviewBarItem19.ImageIndex = 9;
		printPreviewBarItem19.Name = "printPreviewBarItem19";
		printPreviewBarItem20.Caption = "Последняя страница";
		printPreviewBarItem20.Command = PrintingSystemCommand.ShowLastPage;
		printPreviewBarItem20.Enabled = false;
		printPreviewBarItem20.Hint = "Последняя страница";
		printPreviewBarItem20.Id = 26;
		printPreviewBarItem20.ImageIndex = 10;
		printPreviewBarItem20.Name = "printPreviewBarItem20";
		printPreviewBarItem21.ButtonStyle = BarButtonStyle.DropDown;
		printPreviewBarItem21.Caption = "Несколько страниц";
		printPreviewBarItem21.Command = PrintingSystemCommand.MultiplePages;
		printPreviewBarItem21.Enabled = false;
		printPreviewBarItem21.Hint = "Несколько страниц";
		printPreviewBarItem21.Id = 27;
		printPreviewBarItem21.ImageIndex = 11;
		printPreviewBarItem21.Name = "printPreviewBarItem21";
		printPreviewBarItem23.Caption = "&Подложка...";
		printPreviewBarItem23.Command = PrintingSystemCommand.Watermark;
		printPreviewBarItem23.Enabled = false;
		printPreviewBarItem23.Hint = "Подложка...";
		printPreviewBarItem23.Id = 29;
		printPreviewBarItem23.ImageIndex = 21;
		printPreviewBarItem23.Name = "printPreviewBarItem23";
		printPreviewBarItem26.Caption = "В&ыход";
		printPreviewBarItem26.Command = PrintingSystemCommand.ClosePreview;
		printPreviewBarItem26.Hint = "Выход";
		printPreviewBarItem26.Id = 32;
		printPreviewBarItem26.ImageIndex = 13;
		printPreviewBarItem26.Name = "printPreviewBarItem26";
		previewBar2.BarName = "Строка статуса";
		previewBar2.CanDockStyle = BarCanDockStyle.Bottom;
		previewBar2.DockCol = 0;
		previewBar2.DockRow = 0;
		previewBar2.DockStyle = BarDockStyle.Bottom;
		previewBar2.LinksPersistInfo.AddRange(new LinkPersistInfo[7]
		{
			new LinkPersistInfo(printPreviewStaticItem1),
			new LinkPersistInfo(barStaticItem1, beginGroup: true),
			new LinkPersistInfo(progressBarEditItem1),
			new LinkPersistInfo(printPreviewBarItem1),
			new LinkPersistInfo(barButtonItem1),
			new LinkPersistInfo(printPreviewStaticItem2, beginGroup: true),
			new LinkPersistInfo(zoomTrackBarEditItem1)
		});
		previewBar2.OptionsBar.AllowQuickCustomization = false;
		previewBar2.OptionsBar.DisableCustomization = true;
		previewBar2.OptionsBar.DrawDragBorder = false;
		previewBar2.OptionsBar.UseWholeRow = true;
		previewBar2.Text = "Строка статуса";
		printPreviewStaticItem1.Border = BorderStyles.NoBorder;
		printPreviewStaticItem1.Caption = "Nothing";
		printPreviewStaticItem1.Id = 0;
		printPreviewStaticItem1.LeftIndent = 1;
		printPreviewStaticItem1.Name = "printPreviewStaticItem1";
		printPreviewStaticItem1.RightIndent = 1;
		printPreviewStaticItem1.TextAlignment = (StringAlignment)0;
		printPreviewStaticItem1.Type = "PageOfPages";
		barStaticItem1.Border = BorderStyles.NoBorder;
		barStaticItem1.Id = 1;
		barStaticItem1.Name = "barStaticItem1";
		barStaticItem1.TextAlignment = (StringAlignment)0;
		barStaticItem1.Visibility = BarItemVisibility.OnlyInRuntime;
		progressBarEditItem1.Edit = repositoryItemProgressBar1;
		progressBarEditItem1.EditHeight = 12;
		progressBarEditItem1.Id = 2;
		progressBarEditItem1.Name = "progressBarEditItem1";
		progressBarEditItem1.Visibility = BarItemVisibility.Never;
		progressBarEditItem1.Width = 150;
		repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
		repositoryItemProgressBar1.UseParentBackground = true;
		printPreviewBarItem1.Caption = "Stop";
		printPreviewBarItem1.Command = PrintingSystemCommand.StopPageBuilding;
		printPreviewBarItem1.Enabled = false;
		printPreviewBarItem1.Hint = "Stop";
		printPreviewBarItem1.Id = 3;
		printPreviewBarItem1.Name = "printPreviewBarItem1";
		printPreviewBarItem1.Visibility = BarItemVisibility.Never;
		barButtonItem1.Alignment = BarItemLinkAlignment.Left;
		barButtonItem1.Enabled = false;
		barButtonItem1.Id = 4;
		barButtonItem1.Name = "barButtonItem1";
		barButtonItem1.Visibility = BarItemVisibility.OnlyInRuntime;
		printPreviewStaticItem2.Alignment = BarItemLinkAlignment.Right;
		printPreviewStaticItem2.Border = BorderStyles.NoBorder;
		printPreviewStaticItem2.Caption = "100%";
		printPreviewStaticItem2.Id = 5;
		printPreviewStaticItem2.Name = "printPreviewStaticItem2";
		printPreviewStaticItem2.TextAlignment = (StringAlignment)2;
		printPreviewStaticItem2.Type = "ZoomFactor";
		printPreviewStaticItem2.Width = 40;
		zoomTrackBarEditItem1.Alignment = BarItemLinkAlignment.Right;
		zoomTrackBarEditItem1.Edit = repositoryItemZoomTrackBar1;
		zoomTrackBarEditItem1.EditValue = 90;
		zoomTrackBarEditItem1.Enabled = false;
		zoomTrackBarEditItem1.Id = 6;
		zoomTrackBarEditItem1.Name = "zoomTrackBarEditItem1";
		zoomTrackBarEditItem1.Range = new int[2] { 10, 500 };
		zoomTrackBarEditItem1.Width = 140;
		repositoryItemZoomTrackBar1.Alignment = VertAlignment.Center;
		repositoryItemZoomTrackBar1.AllowFocused = false;
		repositoryItemZoomTrackBar1.BorderStyle = BorderStyles.NoBorder;
		repositoryItemZoomTrackBar1.Maximum = 180;
		repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
		repositoryItemZoomTrackBar1.ScrollThumbStyle = ScrollThumbStyle.ArrowDownRight;
		repositoryItemZoomTrackBar1.UseParentBackground = true;
		previewBar3.BarName = "Main Menu";
		previewBar3.CanDockStyle = BarCanDockStyle.Top;
		previewBar3.DockCol = 0;
		previewBar3.DockRow = 0;
		previewBar3.DockStyle = BarDockStyle.Top;
		previewBar3.FloatLocation = new Point(391, 170);
		previewBar3.LinksPersistInfo.AddRange(new LinkPersistInfo[3]
		{
			new LinkPersistInfo(printPreviewSubItem1),
			new LinkPersistInfo(printPreviewSubItem2),
			new LinkPersistInfo(printPreviewSubItem3)
		});
		previewBar3.OptionsBar.AllowQuickCustomization = false;
		previewBar3.OptionsBar.DisableClose = true;
		previewBar3.OptionsBar.DisableCustomization = true;
		previewBar3.OptionsBar.MultiLine = true;
		previewBar3.OptionsBar.UseWholeRow = true;
		previewBar3.Text = "Main Menu";
		printPreviewSubItem1.Caption = "&Файл";
		printPreviewSubItem1.Command = PrintingSystemCommand.File;
		printPreviewSubItem1.Id = 33;
		printPreviewSubItem1.LinksPersistInfo.AddRange(new LinkPersistInfo[4]
		{
			new LinkPersistInfo(printPreviewBarItem10),
			new LinkPersistInfo(printPreviewBarItem8),
			new LinkPersistInfo(printPreviewBarItem24, beginGroup: true),
			new LinkPersistInfo(printPreviewBarItem26, beginGroup: true)
		});
		printPreviewSubItem1.Name = "printPreviewSubItem1";
		printPreviewSubItem2.Caption = "&Вид";
		printPreviewSubItem2.Command = PrintingSystemCommand.View;
		printPreviewSubItem2.Id = 34;
		printPreviewSubItem2.LinksPersistInfo.AddRange(new LinkPersistInfo[2]
		{
			new LinkPersistInfo(printPreviewSubItem4, beginGroup: true),
			new LinkPersistInfo(barToolbarsListItem1, beginGroup: true)
		});
		printPreviewSubItem2.Name = "printPreviewSubItem2";
		printPreviewSubItem4.Caption = "&Макет страницы";
		printPreviewSubItem4.Command = PrintingSystemCommand.PageLayout;
		printPreviewSubItem4.Id = 36;
		printPreviewSubItem4.LinksPersistInfo.AddRange(new LinkPersistInfo[2]
		{
			new LinkPersistInfo(printPreviewBarItem27),
			new LinkPersistInfo(printPreviewBarItem28)
		});
		printPreviewSubItem4.Name = "printPreviewSubItem4";
		printPreviewBarItem27.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem27.Caption = "&Скрыть пробелы";
		printPreviewBarItem27.Command = PrintingSystemCommand.PageLayoutFacing;
		printPreviewBarItem27.Enabled = false;
		printPreviewBarItem27.GroupIndex = 100;
		printPreviewBarItem27.Id = 37;
		printPreviewBarItem27.Name = "printPreviewBarItem27";
		printPreviewBarItem28.ButtonStyle = BarButtonStyle.Check;
		printPreviewBarItem28.Caption = "&Показать пробелы";
		printPreviewBarItem28.Command = PrintingSystemCommand.PageLayoutContinuous;
		printPreviewBarItem28.Down = true;
		printPreviewBarItem28.Enabled = false;
		printPreviewBarItem28.GroupIndex = 100;
		printPreviewBarItem28.Id = 38;
		printPreviewBarItem28.Name = "printPreviewBarItem28";
		barToolbarsListItem1.Caption = "Bars";
		barToolbarsListItem1.Id = 39;
		barToolbarsListItem1.Name = "barToolbarsListItem1";
		printPreviewSubItem3.Caption = "Ф&он";
		printPreviewSubItem3.Command = PrintingSystemCommand.Background;
		printPreviewSubItem3.Id = 35;
		printPreviewSubItem3.LinksPersistInfo.AddRange(new LinkPersistInfo[1]
		{
			new LinkPersistInfo(printPreviewBarItem23)
		});
		printPreviewSubItem3.Name = "printPreviewSubItem3";
		((Control)barDockControlTop).CausesValidation = false;
		barDockControlTop.Dock = (DockStyle)1;
		barDockControlTop.Location = new Point(0, 0);
		barDockControlTop.Size = new Size(854, 53);
		((Control)barDockControlBottom).CausesValidation = false;
		barDockControlBottom.Dock = (DockStyle)2;
		barDockControlBottom.Location = new Point(0, 404);
		barDockControlBottom.Size = new Size(854, 28);
		((Control)barDockControlLeft).CausesValidation = false;
		barDockControlLeft.Dock = (DockStyle)3;
		barDockControlLeft.Location = new Point(0, 53);
		barDockControlLeft.Size = new Size(0, 351);
		((Control)barDockControlRight).CausesValidation = false;
		barDockControlRight.Dock = (DockStyle)4;
		barDockControlRight.Location = new Point(854, 53);
		barDockControlRight.Size = new Size(0, 351);
		printPreviewBarItem5.Caption = "Customize";
		printPreviewBarItem5.Command = PrintingSystemCommand.Customize;
		printPreviewBarItem5.Enabled = false;
		printPreviewBarItem5.Hint = "Customize";
		printPreviewBarItem5.Id = 10;
		printPreviewBarItem5.ImageIndex = 14;
		printPreviewBarItem5.Name = "printPreviewBarItem5";
		printPreviewBarItem9.Caption = "P&rint";
		printPreviewBarItem9.Command = PrintingSystemCommand.PrintDirect;
		printPreviewBarItem9.Enabled = false;
		printPreviewBarItem9.Hint = "Quick Print";
		printPreviewBarItem9.Id = 14;
		printPreviewBarItem9.ImageIndex = 1;
		printPreviewBarItem9.Name = "printPreviewBarItem9";
		printPreviewBarItem22.ButtonStyle = BarButtonStyle.DropDown;
		printPreviewBarItem22.Caption = "&Color...";
		printPreviewBarItem22.Command = PrintingSystemCommand.FillBackground;
		printPreviewBarItem22.Enabled = false;
		printPreviewBarItem22.Hint = "Background";
		printPreviewBarItem22.Id = 28;
		printPreviewBarItem22.ImageIndex = 12;
		printPreviewBarItem22.Name = "printPreviewBarItem22";
		printPreviewBarItem25.ButtonStyle = BarButtonStyle.DropDown;
		printPreviewBarItem25.Caption = "Send via E-Mail...";
		printPreviewBarItem25.Command = PrintingSystemCommand.SendFile;
		printPreviewBarItem25.Enabled = false;
		printPreviewBarItem25.Hint = "Send via E-Mail...";
		printPreviewBarItem25.Id = 31;
		printPreviewBarItem25.ImageIndex = 17;
		printPreviewBarItem25.Name = "printPreviewBarItem25";
		printPreviewBarCheckItem1.Caption = "PDF File";
		printPreviewBarCheckItem1.Checked = true;
		printPreviewBarCheckItem1.Command = PrintingSystemCommand.ExportPdf;
		printPreviewBarCheckItem1.Enabled = false;
		printPreviewBarCheckItem1.GroupIndex = 2;
		printPreviewBarCheckItem1.Hint = "PDF File";
		printPreviewBarCheckItem1.Id = 40;
		printPreviewBarCheckItem1.Name = "printPreviewBarCheckItem1";
		printPreviewBarCheckItem2.Caption = "HTML File";
		printPreviewBarCheckItem2.Command = PrintingSystemCommand.ExportHtm;
		printPreviewBarCheckItem2.Enabled = false;
		printPreviewBarCheckItem2.GroupIndex = 2;
		printPreviewBarCheckItem2.Hint = "HTML File";
		printPreviewBarCheckItem2.Id = 41;
		printPreviewBarCheckItem2.Name = "printPreviewBarCheckItem2";
		printPreviewBarCheckItem3.Caption = "MHT File";
		printPreviewBarCheckItem3.Command = PrintingSystemCommand.ExportMht;
		printPreviewBarCheckItem3.Enabled = false;
		printPreviewBarCheckItem3.GroupIndex = 2;
		printPreviewBarCheckItem3.Hint = "MHT File";
		printPreviewBarCheckItem3.Id = 42;
		printPreviewBarCheckItem3.Name = "printPreviewBarCheckItem3";
		printPreviewBarCheckItem4.Caption = "RTF File";
		printPreviewBarCheckItem4.Command = PrintingSystemCommand.ExportRtf;
		printPreviewBarCheckItem4.Enabled = false;
		printPreviewBarCheckItem4.GroupIndex = 2;
		printPreviewBarCheckItem4.Hint = "RTF File";
		printPreviewBarCheckItem4.Id = 43;
		printPreviewBarCheckItem4.Name = "printPreviewBarCheckItem4";
		printPreviewBarCheckItem5.Caption = "XLS File";
		printPreviewBarCheckItem5.Command = PrintingSystemCommand.ExportXls;
		printPreviewBarCheckItem5.Enabled = false;
		printPreviewBarCheckItem5.GroupIndex = 2;
		printPreviewBarCheckItem5.Hint = "XLS File";
		printPreviewBarCheckItem5.Id = 44;
		printPreviewBarCheckItem5.Name = "printPreviewBarCheckItem5";
		printPreviewBarCheckItem6.Caption = "XLSX File";
		printPreviewBarCheckItem6.Command = PrintingSystemCommand.ExportXlsx;
		printPreviewBarCheckItem6.Enabled = false;
		printPreviewBarCheckItem6.GroupIndex = 2;
		printPreviewBarCheckItem6.Hint = "XLSX File";
		printPreviewBarCheckItem6.Id = 45;
		printPreviewBarCheckItem6.Name = "printPreviewBarCheckItem6";
		printPreviewBarCheckItem7.Caption = "CSV File";
		printPreviewBarCheckItem7.Command = PrintingSystemCommand.ExportCsv;
		printPreviewBarCheckItem7.Enabled = false;
		printPreviewBarCheckItem7.GroupIndex = 2;
		printPreviewBarCheckItem7.Hint = "CSV File";
		printPreviewBarCheckItem7.Id = 46;
		printPreviewBarCheckItem7.Name = "printPreviewBarCheckItem7";
		printPreviewBarCheckItem8.Caption = "Text File";
		printPreviewBarCheckItem8.Command = PrintingSystemCommand.ExportTxt;
		printPreviewBarCheckItem8.Enabled = false;
		printPreviewBarCheckItem8.GroupIndex = 2;
		printPreviewBarCheckItem8.Hint = "Text File";
		printPreviewBarCheckItem8.Id = 47;
		printPreviewBarCheckItem8.Name = "printPreviewBarCheckItem8";
		printPreviewBarCheckItem9.Caption = "Image File";
		printPreviewBarCheckItem9.Command = PrintingSystemCommand.ExportGraphic;
		printPreviewBarCheckItem9.Enabled = false;
		printPreviewBarCheckItem9.GroupIndex = 2;
		printPreviewBarCheckItem9.Hint = "Image File";
		printPreviewBarCheckItem9.Id = 48;
		printPreviewBarCheckItem9.Name = "printPreviewBarCheckItem9";
		printPreviewBarCheckItem10.Caption = "PDF File";
		printPreviewBarCheckItem10.Checked = true;
		printPreviewBarCheckItem10.Command = PrintingSystemCommand.SendPdf;
		printPreviewBarCheckItem10.Enabled = false;
		printPreviewBarCheckItem10.GroupIndex = 1;
		printPreviewBarCheckItem10.Hint = "PDF File";
		printPreviewBarCheckItem10.Id = 49;
		printPreviewBarCheckItem10.Name = "printPreviewBarCheckItem10";
		printPreviewBarCheckItem11.Caption = "MHT File";
		printPreviewBarCheckItem11.Command = PrintingSystemCommand.SendMht;
		printPreviewBarCheckItem11.Enabled = false;
		printPreviewBarCheckItem11.GroupIndex = 1;
		printPreviewBarCheckItem11.Hint = "MHT File";
		printPreviewBarCheckItem11.Id = 50;
		printPreviewBarCheckItem11.Name = "printPreviewBarCheckItem11";
		printPreviewBarCheckItem12.Caption = "RTF File";
		printPreviewBarCheckItem12.Command = PrintingSystemCommand.SendRtf;
		printPreviewBarCheckItem12.Enabled = false;
		printPreviewBarCheckItem12.GroupIndex = 1;
		printPreviewBarCheckItem12.Hint = "RTF File";
		printPreviewBarCheckItem12.Id = 51;
		printPreviewBarCheckItem12.Name = "printPreviewBarCheckItem12";
		printPreviewBarCheckItem13.Caption = "XLS File";
		printPreviewBarCheckItem13.Command = PrintingSystemCommand.SendXls;
		printPreviewBarCheckItem13.Enabled = false;
		printPreviewBarCheckItem13.GroupIndex = 1;
		printPreviewBarCheckItem13.Hint = "XLS File";
		printPreviewBarCheckItem13.Id = 52;
		printPreviewBarCheckItem13.Name = "printPreviewBarCheckItem13";
		printPreviewBarCheckItem14.Caption = "XLSX";
		printPreviewBarCheckItem14.Command = PrintingSystemCommand.SendXlsx;
		printPreviewBarCheckItem14.Enabled = false;
		printPreviewBarCheckItem14.GroupIndex = 1;
		printPreviewBarCheckItem14.Hint = "XLSX";
		printPreviewBarCheckItem14.Id = 53;
		printPreviewBarCheckItem14.Name = "printPreviewBarCheckItem14";
		printPreviewBarCheckItem15.Caption = "CSV File";
		printPreviewBarCheckItem15.Command = PrintingSystemCommand.SendCsv;
		printPreviewBarCheckItem15.Enabled = false;
		printPreviewBarCheckItem15.GroupIndex = 1;
		printPreviewBarCheckItem15.Hint = "CSV File";
		printPreviewBarCheckItem15.Id = 54;
		printPreviewBarCheckItem15.Name = "printPreviewBarCheckItem15";
		printPreviewBarCheckItem16.Caption = "Text File";
		printPreviewBarCheckItem16.Command = PrintingSystemCommand.SendTxt;
		printPreviewBarCheckItem16.Enabled = false;
		printPreviewBarCheckItem16.GroupIndex = 1;
		printPreviewBarCheckItem16.Hint = "Text File";
		printPreviewBarCheckItem16.Id = 55;
		printPreviewBarCheckItem16.Name = "printPreviewBarCheckItem16";
		printPreviewBarCheckItem17.Caption = "Image File";
		printPreviewBarCheckItem17.Command = PrintingSystemCommand.SendGraphic;
		printPreviewBarCheckItem17.Enabled = false;
		printPreviewBarCheckItem17.GroupIndex = 1;
		printPreviewBarCheckItem17.Hint = "Image File";
		printPreviewBarCheckItem17.Id = 56;
		printPreviewBarCheckItem17.Name = "printPreviewBarCheckItem17";
		printControl1.BackColor = Color.FromArgb(245, 245, 245);
		((Control)printControl1).Dock = (DockStyle)5;
		printControl1.ForeColor = Color.Empty;
		printControl1.IsMetric = true;
		((Control)printControl1).Location = new Point(0, 53);
		((Control)printControl1).Name = "printControl1";
		printControl1.PrintingSystem = printingSystem1;
		((Control)printControl1).Size = new Size(854, 351);
		((Control)printControl1).TabIndex = 4;
		printControl1.TooltipFont = new Font("Tahoma", 8.25f);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(854, 432);
		((Control)this).Controls.Add((Control)(object)printControl1);
		((Control)this).Controls.Add((Control)(object)barDockControlLeft);
		((Control)this).Controls.Add((Control)(object)barDockControlRight);
		((Control)this).Controls.Add((Control)(object)barDockControlBottom);
		((Control)this).Controls.Add((Control)(object)barDockControlTop);
		((Control)this).Name = "PrintForm";
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "Просмотр";
		((ISupportInitialize)printingSystem1).EndInit();
		((ISupportInitialize)printBarManager1).EndInit();
		((ISupportInitialize)printPreviewRepositoryItemComboBox1).EndInit();
		((ISupportInitialize)repositoryItemProgressBar1).EndInit();
		((ISupportInitialize)repositoryItemZoomTrackBar1).EndInit();
		((Control)this).ResumeLayout(false);
	}

	public PrintForm()
	{
		InitializeComponent();
	}
}
