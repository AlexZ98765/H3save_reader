using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;

namespace ProspectorRT;

public class ShowForm : Form
{
	private IContainer components;

	public RichEditControl reCell;

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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		reCell = new RichEditControl();
		((Control)this).SuspendLayout();
		reCell.ActiveViewType = RichEditViewType.Simple;
		reCell.Appearance.Text.Font = new Font("Microsoft Sans Serif", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		reCell.Appearance.Text.Options.UseFont = true;
		reCell.Appearance.Text.Options.UseTextOptions = true;
		reCell.Appearance.Text.TextOptions.WordWrap = WordWrap.Wrap;
		((Control)reCell).Dock = (DockStyle)5;
		((Control)reCell).Location = new Point(0, 0);
		((Control)reCell).Name = "reCell";
		reCell.ReadOnly = true;
		((Control)reCell).Size = new Size(334, 316);
		((Control)reCell).TabIndex = 1;
		((Control)reCell).KeyDown += new KeyEventHandler(reCell_KeyDown);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(334, 316);
		((Control)this).Controls.Add((Control)(object)reCell);
		((Form)this).FormBorderStyle = (FormBorderStyle)6;
		((Control)this).Name = "ShowForm";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Form)this).TopMost = true;
		((Control)this).ResumeLayout(false);
	}

	public ShowForm()
	{
		InitializeComponent();
	}

	private void reCell_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.KeyCode == 27)
		{
			((Form)this).Close();
		}
	}
}
