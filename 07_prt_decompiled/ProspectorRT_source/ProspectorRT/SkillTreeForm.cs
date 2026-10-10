using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace ProspectorRT;

public class SkillTreeForm : Form
{
	private Form _fOwner;

	private IContainer components;

	private TreeListColumn colSSkill;

	public TreeList treeSkill;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem btnExpandAll;

	private BarButtonItem btnCollapseAll;

	private PopupMenu popupMenu1;

	private ToolTipController toolTipController1;

	public Form fOwner
	{
		set
		{
			_fOwner = value;
		}
	}

	public SkillTreeForm()
	{
		InitializeComponent();
	}

	private void btnExpandAll_ItemClick(object sender, ItemClickEventArgs e)
	{
		treeSkill.ExpandAll();
	}

	private void btnCollapseAll_ItemClick(object sender, ItemClickEventArgs e)
	{
		treeSkill.CollapseAll();
		if (treeSkill.Nodes[0].Nodes.Count > 0)
		{
			treeSkill.MakeNodeVisible(treeSkill.Nodes[0].Nodes[0]);
		}
	}

	private void SkillTreeForm_Load(object sender, EventArgs e)
	{
		((BarManager)treeSkill.MenuManager).SetPopupContextMenu((Control)(object)treeSkill, popupMenu1);
	}

	private void treeSkill_MouseDown(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)e.Button != 2097152)
		{
			return;
		}
		TreeListHitInfo treeListHitInfo = ((TreeList)sender).CalcHitInfo(new Point(e.X, e.Y));
		bool visible = treeListHitInfo.HitInfoType != HitInfoType.Column && treeListHitInfo.HitInfoType != HitInfoType.SummaryFooter && treeListHitInfo.HitInfoType != HitInfoType.RowFooter;
		PopupMenu popupContextMenu = ((BarManager)((TreeList)sender).MenuManager).GetPopupContextMenu((Control)(object)(TreeList)sender);
		foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
		{
			itemLink.Visible = visible;
		}
	}

	private void treeSkill_MouseUp(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)e.Button != 2097152)
		{
			return;
		}
		PopupMenu popupContextMenu = ((BarManager)((TreeList)sender).MenuManager).GetPopupContextMenu((Control)(object)(TreeList)sender);
		foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
		{
			itemLink.Visible = true;
		}
	}

	private void treeSkill_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		TreeList treeList = (TreeList)sender;
		BarManager barManager = (BarManager)treeList.MenuManager;
		if ((int)e.KeyCode != 93)
		{
			return;
		}
		PopupMenu popupContextMenu = barManager.GetPopupContextMenu((Control)(object)treeList);
		foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
		{
			itemLink.Visible = true;
		}
		popupContextMenu.ShowPopup(barManager, ((Control)treeList).PointToScreen(new Point(0, 0)));
	}

	private void toolTipController1_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		string[] array = e.ToolTip.Split(new string[1] { ", " }, StringSplitOptions.None);
		if (array.Length > 1)
		{
			string text = "";
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = text + " " + string.Format("{0} \r\n", text2 + ",");
			}
			text = text.Remove(text.Length - 4);
			if (text.Length > 571)
			{
				text = text.Remove(567) + " ...";
			}
			e.ToolTip = text;
		}
	}

	private void SkillTreeForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		((Control)_fOwner).Select();
		_fOwner = null;
		((Component)(object)this).Dispose(disposing: true);
		GC.Collect();
	}

	private void SkillTreeForm_Shown(object sender, EventArgs e)
	{
		((Form)this).Activate();
	}

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
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Expected O, but got Unknown
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Expected O, but got Unknown
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Expected O, but got Unknown
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Expected O, but got Unknown
		components = new Container();
		treeSkill = new TreeList();
		colSSkill = new TreeListColumn();
		barManager1 = new BarManager(components);
		barDockControlTop = new BarDockControl();
		barDockControlBottom = new BarDockControl();
		barDockControlLeft = new BarDockControl();
		barDockControlRight = new BarDockControl();
		btnExpandAll = new BarButtonItem();
		btnCollapseAll = new BarButtonItem();
		toolTipController1 = new ToolTipController(components);
		popupMenu1 = new PopupMenu(components);
		((ISupportInitialize)treeSkill).BeginInit();
		((ISupportInitialize)barManager1).BeginInit();
		((ISupportInitialize)popupMenu1).BeginInit();
		((Control)this).SuspendLayout();
		treeSkill.Appearance.FocusedCell.BackColor = Color.FromArgb(72, 118, 186);
		treeSkill.Appearance.FocusedCell.BackColor2 = Color.FromArgb(169, 189, 226);
		treeSkill.Appearance.FocusedCell.ForeColor = Color.White;
		treeSkill.Appearance.FocusedCell.Options.UseBackColor = true;
		treeSkill.Appearance.FocusedCell.Options.UseForeColor = true;
		treeSkill.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		treeSkill.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		treeSkill.Appearance.FocusedRow.ForeColor = Color.White;
		treeSkill.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		treeSkill.Appearance.FocusedRow.Options.UseBackColor = true;
		treeSkill.Appearance.FocusedRow.Options.UseForeColor = true;
		treeSkill.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		treeSkill.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		treeSkill.Appearance.HideSelectionRow.ForeColor = Color.Black;
		treeSkill.Appearance.HideSelectionRow.Options.UseBackColor = true;
		treeSkill.Appearance.HideSelectionRow.Options.UseForeColor = true;
		treeSkill.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		treeSkill.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		treeSkill.Appearance.SelectedRow.ForeColor = Color.Black;
		treeSkill.Appearance.SelectedRow.Options.UseBackColor = true;
		treeSkill.Appearance.SelectedRow.Options.UseForeColor = true;
		treeSkill.Columns.AddRange(new TreeListColumn[1] { colSSkill });
		((Control)treeSkill).Dock = (DockStyle)5;
		((Control)treeSkill).Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		treeSkill.KeyFieldName = "";
		((Control)treeSkill).Location = new Point(0, 0);
		treeSkill.LookAndFeel.SkinName = "MySkin_Lilian1";
		treeSkill.LookAndFeel.UseDefaultLookAndFeel = false;
		treeSkill.MenuManager = barManager1;
		((Control)treeSkill).Name = "treeSkill";
		treeSkill.OptionsBehavior.AllowIncrementalSearch = true;
		treeSkill.OptionsBehavior.Editable = false;
		treeSkill.OptionsMenu.EnableColumnMenu = false;
		treeSkill.OptionsMenu.EnableFooterMenu = false;
		treeSkill.ParentFieldName = "";
		((Control)treeSkill).Size = new Size(764, 404);
		((Control)treeSkill).TabIndex = 0;
		treeSkill.ToolTipController = toolTipController1;
		treeSkill.TreeLineStyle = LineStyle.Solid;
		((Control)treeSkill).KeyDown += new KeyEventHandler(treeSkill_KeyDown);
		((Control)treeSkill).MouseDown += new MouseEventHandler(treeSkill_MouseDown);
		((Control)treeSkill).MouseUp += new MouseEventHandler(treeSkill_MouseUp);
		colSSkill.Caption = " ";
		colSSkill.FieldName = " ";
		colSSkill.Name = "colSSkill";
		colSSkill.OptionsColumn.AllowEdit = false;
		colSSkill.OptionsColumn.AllowMove = false;
		colSSkill.OptionsColumn.AllowMoveToCustomizationForm = false;
		colSSkill.OptionsColumn.ReadOnly = true;
		colSSkill.OptionsColumn.ShowInCustomizationForm = false;
		colSSkill.Visible = true;
		colSSkill.VisibleIndex = 0;
		barManager1.DockControls.Add(barDockControlTop);
		barManager1.DockControls.Add(barDockControlBottom);
		barManager1.DockControls.Add(barDockControlLeft);
		barManager1.DockControls.Add(barDockControlRight);
		barManager1.Form = (Control)(object)this;
		barManager1.Items.AddRange(new BarItem[2] { btnExpandAll, btnCollapseAll });
		barManager1.MaxItemId = 2;
		((Control)barDockControlTop).CausesValidation = false;
		barDockControlTop.Dock = (DockStyle)1;
		barDockControlTop.Location = new Point(0, 0);
		barDockControlTop.Size = new Size(764, 0);
		((Control)barDockControlBottom).CausesValidation = false;
		barDockControlBottom.Dock = (DockStyle)2;
		barDockControlBottom.Location = new Point(0, 404);
		barDockControlBottom.Size = new Size(764, 0);
		((Control)barDockControlLeft).CausesValidation = false;
		barDockControlLeft.Dock = (DockStyle)3;
		barDockControlLeft.Location = new Point(0, 0);
		barDockControlLeft.Size = new Size(0, 404);
		((Control)barDockControlRight).CausesValidation = false;
		barDockControlRight.Dock = (DockStyle)4;
		barDockControlRight.Location = new Point(764, 0);
		barDockControlRight.Size = new Size(0, 404);
		btnExpandAll.Caption = "Развернуть все";
		btnExpandAll.Id = 0;
		btnExpandAll.Name = "btnExpandAll";
		btnExpandAll.ItemClick += btnExpandAll_ItemClick;
		btnCollapseAll.Caption = "Свернуть все";
		btnCollapseAll.Id = 1;
		btnCollapseAll.Name = "btnCollapseAll";
		btnCollapseAll.ItemClick += btnCollapseAll_ItemClick;
		toolTipController1.AllowHtmlText = true;
		toolTipController1.AutoPopDelay = 60000;
		toolTipController1.ShowBeak = true;
		toolTipController1.ToolTipType = ToolTipType.SuperTip;
		toolTipController1.BeforeShow += toolTipController1_BeforeShow;
		popupMenu1.LinksPersistInfo.AddRange(new LinkPersistInfo[2]
		{
			new LinkPersistInfo(btnExpandAll),
			new LinkPersistInfo(btnCollapseAll, beginGroup: true)
		});
		popupMenu1.Manager = barManager1;
		popupMenu1.Name = "popupMenu1";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(764, 404);
		((Control)this).Controls.Add((Control)(object)treeSkill);
		((Control)this).Controls.Add((Control)(object)barDockControlLeft);
		((Control)this).Controls.Add((Control)(object)barDockControlRight);
		((Control)this).Controls.Add((Control)(object)barDockControlBottom);
		((Control)this).Controls.Add((Control)(object)barDockControlTop);
		((Control)this).Name = "SkillTreeForm";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "Level-Up Oracle";
		((Form)this).FormClosed += new FormClosedEventHandler(SkillTreeForm_FormClosed);
		((Form)this).Load += SkillTreeForm_Load;
		((Form)this).Shown += SkillTreeForm_Shown;
		((ISupportInitialize)treeSkill).EndInit();
		((ISupportInitialize)barManager1).EndInit();
		((ISupportInitialize)popupMenu1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
