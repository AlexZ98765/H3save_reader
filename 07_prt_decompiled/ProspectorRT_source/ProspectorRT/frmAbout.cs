using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ProspectorRT;

internal class frmAbout : Form
{
	private IContainer components;

	private Button okButton;

	private LabelControl labelControl1;

	private LabelControl labelControl2;

	private PictureBox pictureBox1;

	private LabelControl labelControl3;

	private LabelControl labelControl4;

	private LabelControl labelControl5;

	private LabelControl labelControl6;

	private LabelControl labelControl7;

	private LabelControl labelControl10;

	private LabelControl labelControl11;

	private LabelControl labelControl13;

	private LinkLabel linkLabel1;

	private LinkLabel linkLabel2;

	private TextBox textBox1;

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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Expected O, but got Unknown
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Expected O, but got Unknown
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Expected O, but got Unknown
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Expected O, but got Unknown
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Expected O, but got Unknown
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Expected O, but got Unknown
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Expected O, but got Unknown
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Expected O, but got Unknown
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Expected O, but got Unknown
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Expected O, but got Unknown
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Expected O, but got Unknown
		//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmAbout));
		okButton = new Button();
		labelControl1 = new LabelControl();
		labelControl2 = new LabelControl();
		pictureBox1 = new PictureBox();
		labelControl3 = new LabelControl();
		labelControl4 = new LabelControl();
		labelControl5 = new LabelControl();
		labelControl6 = new LabelControl();
		labelControl7 = new LabelControl();
		labelControl10 = new LabelControl();
		labelControl11 = new LabelControl();
		labelControl13 = new LabelControl();
		linkLabel1 = new LinkLabel();
		linkLabel2 = new LinkLabel();
		textBox1 = new TextBox();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)okButton).Anchor = (AnchorStyles)10;
		okButton.DialogResult = (DialogResult)2;
		((Control)okButton).Location = new Point(201, 218);
		((Control)okButton).Name = "okButton";
		((Control)okButton).Size = new Size(75, 21);
		((Control)okButton).TabIndex = 0;
		((Control)okButton).Text = "&ОК";
		labelControl1.Appearance.Font = new Font("Tahoma", 18f, (FontStyle)2, (GraphicsUnit)3, (byte)204);
		((Control)labelControl1).Location = new Point(76, 12);
		((Control)labelControl1).Name = "labelControl1";
		((Control)labelControl1).Size = new Size(149, 29);
		((Control)labelControl1).TabIndex = 25;
		((Control)labelControl1).Text = "ProspectorRT";
		labelControl2.Appearance.Font = new Font("Tahoma", 15.75f, (FontStyle)2, (GraphicsUnit)3, (byte)204);
		((Control)labelControl2).Location = new Point(102, 41);
		((Control)labelControl2).Name = "labelControl2";
		((Control)labelControl2).Size = new Size(74, 25);
		((Control)labelControl2).TabIndex = 26;
		((Control)labelControl2).Text = "ver. 2.3";
		pictureBox1.ErrorImage = null;
		pictureBox1.Image = (Image)componentResourceManager.GetObject("pictureBox1.Image");
		pictureBox1.InitialImage = null;
		((Control)pictureBox1).Location = new Point(13, 18);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(35, 35);
		pictureBox1.TabIndex = 27;
		pictureBox1.TabStop = false;
		labelControl3.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl3).Location = new Point(12, 83);
		((Control)labelControl3).Name = "labelControl3";
		((Control)labelControl3).Size = new Size(35, 13);
		((Control)labelControl3).TabIndex = 28;
		((Control)labelControl3).Text = "Автор:";
		labelControl4.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl4).Location = new Point(12, 132);
		((Control)labelControl4).Name = "labelControl4";
		((Control)labelControl4).Size = new Size(29, 13);
		((Control)labelControl4).TabIndex = 29;
		((Control)labelControl4).Text = "Сайт:";
		labelControl5.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl5).Location = new Point(12, 181);
		((Control)labelControl5).Name = "labelControl5";
		((Control)labelControl5).Size = new Size(32, 13);
		((Control)labelControl5).TabIndex = 30;
		((Control)labelControl5).Text = "E-mail:";
		labelControl6.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl6).Location = new Point(57, 83);
		((Control)labelControl6).Name = "labelControl6";
		((Control)labelControl6).Size = new Size(62, 13);
		((Control)labelControl6).TabIndex = 31;
		((Control)labelControl6).Text = "Stormbringer";
		labelControl7.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl7).Location = new Point(6, 224);
		((Control)labelControl7).Name = "labelControl7";
		((Control)labelControl7).Size = new Size(186, 13);
		((Control)labelControl7).TabIndex = 32;
		((Control)labelControl7).Text = "© Copyright  Stormbringer  2012-2021";
		labelControl10.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl10).Location = new Point(103, 100);
		((Control)labelControl10).Name = "labelControl10";
		((Control)labelControl10).Size = new Size(97, 13);
		((Control)labelControl10).TabIndex = 36;
		((Control)labelControl10).Text = "AlexSpl, AmberSoler";
		labelControl11.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl11).Location = new Point(12, 100);
		((Control)labelControl11).Name = "labelControl11";
		((Control)labelControl11).Size = new Size(81, 13);
		((Control)labelControl11).TabIndex = 35;
		((Control)labelControl11).Text = "Благодарности:";
		labelControl13.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((Control)labelControl13).Location = new Point(12, 149);
		((Control)labelControl13).Name = "labelControl13";
		((Control)labelControl13).Size = new Size(68, 13);
		((Control)labelControl13).TabIndex = 37;
		((Control)labelControl13).Text = "Обсуждение:";
		((Control)linkLabel1).AutoSize = true;
		linkLabel1.LinkBehavior = (LinkBehavior)2;
		linkLabel1.LinkColor = Color.FromArgb(72, 118, 186);
		((Control)linkLabel1).Location = new Point(51, 132);
		((Control)linkLabel1).Name = "linkLabel1";
		((Control)linkLabel1).Size = new Size(199, 13);
		((Control)linkLabel1).TabIndex = 39;
		((Label)linkLabel1).TabStop = true;
		((Control)linkLabel1).Tag = "http://sites.google.com/site/prospectorrt";
		((Control)linkLabel1).Text = "http://sites.google.com/site/prospectorrt";
		linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
		((Control)linkLabel2).AutoSize = true;
		linkLabel2.LinkBehavior = (LinkBehavior)2;
		linkLabel2.LinkColor = Color.FromArgb(72, 118, 186);
		((Control)linkLabel2).Location = new Point(90, 149);
		((Control)linkLabel2).Name = "linkLabel2";
		((Control)linkLabel2).Size = new Size(141, 13);
		((Control)linkLabel2).TabIndex = 40;
		((Label)linkLabel2).TabStop = true;
		((Control)linkLabel2).Tag = "http://heroesportal.net/tavern/?id=367658";
		((Control)linkLabel2).Text = "http://www.heroesportal.net";
		linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
		((Control)textBox1).BackColor = SystemColors.Control;
		((TextBoxBase)textBox1).BorderStyle = (BorderStyle)0;
		((Control)textBox1).Location = new Point(54, 181);
		((Control)textBox1).Name = "textBox1";
		((TextBoxBase)textBox1).ReadOnly = true;
		((Control)textBox1).Size = new Size(123, 13);
		((Control)textBox1).TabIndex = 41;
		((Control)textBox1).TabStop = false;
		((Control)textBox1).Text = "prospectorrt@yandex.ru";
		((Form)this).AcceptButton = (IButtonControl)(object)okButton;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(278, 245);
		((Control)this).Controls.Add((Control)(object)textBox1);
		((Control)this).Controls.Add((Control)(object)linkLabel2);
		((Control)this).Controls.Add((Control)(object)linkLabel1);
		((Control)this).Controls.Add((Control)(object)labelControl13);
		((Control)this).Controls.Add((Control)(object)labelControl10);
		((Control)this).Controls.Add((Control)(object)labelControl11);
		((Control)this).Controls.Add((Control)(object)labelControl7);
		((Control)this).Controls.Add((Control)(object)labelControl6);
		((Control)this).Controls.Add((Control)(object)labelControl5);
		((Control)this).Controls.Add((Control)(object)labelControl4);
		((Control)this).Controls.Add((Control)(object)labelControl3);
		((Control)this).Controls.Add((Control)(object)pictureBox1);
		((Control)this).Controls.Add((Control)(object)labelControl2);
		((Control)this).Controls.Add((Control)(object)labelControl1);
		((Control)this).Controls.Add((Control)(object)okButton);
		((Form)this).FormBorderStyle = (FormBorderStyle)5;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "frmAbout";
		((Control)this).Padding = new Padding(9);
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "О программе";
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	public frmAbout()
	{
		InitializeComponent();
	}

	public void OpenLink(string sUrl)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Process.Start(sUrl);
		}
		catch
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe", sUrl);
				Process.Start(startInfo);
				startInfo = null;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}
	}

	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		LinkLabel val = (LinkLabel)sender;
		OpenLink((string)((Control)val).Tag);
		linkLabel1.LinkVisited = true;
	}

	private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		LinkLabel val = (LinkLabel)sender;
		OpenLink((string)((Control)val).Tag);
		linkLabel2.LinkVisited = true;
	}
}
