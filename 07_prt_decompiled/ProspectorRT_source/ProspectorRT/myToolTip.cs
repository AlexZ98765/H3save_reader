using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProspectorRT;

public class myToolTip : Form
{
	private const int CS_DROPSHADOW = 131072;

	private IContainer components;

	public Label label1;

	private Timer timer1;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = ((Form)this).CreateParams;
			createParams.ClassStyle |= 0x20000;
			return createParams;
		}
	}

	protected override bool ShowWithoutActivation => true;

	public myToolTip()
	{
		InitializeComponent();
	}

	private void myToolTip_Load(object sender, EventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		((Control)this).Paint += new PaintEventHandler(dropShadow);
		timer1.Tag = 0;
	}

	private void dropShadow(object sender, PaintEventArgs e)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		Color[] array = new Color[3]
		{
			Color.FromArgb(181, 181, 181),
			Color.FromArgb(195, 195, 195),
			Color.FromArgb(211, 211, 211)
		};
		Pen val = new Pen(array[0]);
		Pen val2 = val;
		try
		{
			Point location = ((Form)this).Location;
			location.Y += ((Control)this).Height;
			for (int i = 0; i < 3; i++)
			{
				val.Color = array[i];
				e.Graphics.DrawLine(val, location.X, location.Y, location.X + ((Control)this).Width - 1, location.Y);
				location.Y++;
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void label1_Paint(object sender, PaintEventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		Rectangle rectangle = new Rectangle(0, 0, ((Control)this).ClientRectangle.Width - 1, ((Control)this).ClientRectangle.Height - 1);
		Pen val = new Pen(Color.FromArgb(157, 160, 170), 1f);
		e.Graphics.DrawRectangle(val, rectangle);
	}

	private void label1_MouseHover(object sender, EventArgs e)
	{
		((Control)this).Hide();
	}

	private void label1_MouseMove(object sender, MouseEventArgs e)
	{
		if ((int)timer1.Tag == 0)
		{
			timer1.Start();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if ((int)timer1.Tag > 5)
		{
			timer1.Stop();
			timer1.Tag = 0;
			((Control)this).Hide();
		}
		else
		{
			timer1.Tag = (int)timer1.Tag + 1;
		}
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		components = new Container();
		label1 = new Label();
		timer1 = new Timer(components);
		((Control)this).SuspendLayout();
		((Control)label1).AutoSize = true;
		((Control)label1).BackColor = Color.FromArgb(233, 235, 243);
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)label1).ForeColor = SystemColors.ControlText;
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).MaximumSize = new Size(1000, 20);
		((Control)label1).MinimumSize = new Size(10, 20);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(10, 20);
		((Control)label1).TabIndex = 0;
		label1.TextAlign = (ContentAlignment)32;
		((Control)label1).Paint += new PaintEventHandler(label1_Paint);
		((Control)label1).MouseHover += label1_MouseHover;
		((Control)label1).MouseMove += new MouseEventHandler(label1_MouseMove);
		timer1.Interval = 10;
		timer1.Tick += timer1_Tick;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).AutoSize = true;
		((Form)this).AutoSizeMode = (AutoSizeMode)0;
		((Form)this).ClientSize = new Size(114, 20);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)label1);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).MaximizeBox = false;
		((Control)this).MaximumSize = new Size(1000, 20);
		((Form)this).MinimizeBox = false;
		((Control)this).MinimumSize = new Size(10, 20);
		((Control)this).Name = "myToolTip";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)0;
		((Form)this).Load += myToolTip_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
