using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using DevExpress.XtraEditors;

namespace ProspectorRT;

public class ExportForm : Form
{
	private IContainer components;

	private CheckEdit ce0;

	private CheckEdit ce1;

	private CheckEdit ce2;

	private CheckEdit ce3;

	private CheckEdit ce4;

	private CheckEdit ce5;

	private CheckEdit ce6;

	private CheckEdit ce7;

	private CheckEdit ce8;

	private CheckEdit ce9;

	private CheckEdit ce10;

	private CheckEdit ce11;

	private CheckEdit ce12;

	private CheckEdit ce13;

	private CheckEdit ce14;

	private CheckEdit ce15;

	private CheckEdit ce16;

	private CheckEdit ce17;

	private CheckEdit ce18;

	private CheckEdit ce19;

	private CheckEdit ce20;

	private CheckEdit ce21;

	private CheckEdit ce22;

	private GroupControl grpControl;

	private PanelControl panelControl1;

	private LabelControl labelControl1;

	private SimpleButton btnPath;

	private TextEdit tePath;

	private SimpleButton btnOK;

	private SimpleButton btnCancel;

	private SimpleButton btnClear;

	private SimpleButton btnAdd;

	private SaveFileDialog saveFileDialog1;

	private CheckEdit ce23;

	private string myName = "ProspectorRT - Экспорт в Excel";

	private string myFile = "";

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
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_1011: Unknown result type (might be due to invalid IL or missing references)
		//IL_101b: Expected O, but got Unknown
		ce0 = new CheckEdit();
		ce1 = new CheckEdit();
		ce2 = new CheckEdit();
		ce3 = new CheckEdit();
		ce4 = new CheckEdit();
		ce5 = new CheckEdit();
		ce6 = new CheckEdit();
		ce7 = new CheckEdit();
		ce8 = new CheckEdit();
		ce9 = new CheckEdit();
		ce10 = new CheckEdit();
		ce11 = new CheckEdit();
		ce12 = new CheckEdit();
		ce13 = new CheckEdit();
		ce14 = new CheckEdit();
		ce15 = new CheckEdit();
		ce16 = new CheckEdit();
		ce17 = new CheckEdit();
		ce18 = new CheckEdit();
		ce19 = new CheckEdit();
		ce20 = new CheckEdit();
		ce21 = new CheckEdit();
		ce22 = new CheckEdit();
		grpControl = new GroupControl();
		ce23 = new CheckEdit();
		panelControl1 = new PanelControl();
		labelControl1 = new LabelControl();
		btnPath = new SimpleButton();
		tePath = new TextEdit();
		btnOK = new SimpleButton();
		btnCancel = new SimpleButton();
		btnClear = new SimpleButton();
		btnAdd = new SimpleButton();
		saveFileDialog1 = new SaveFileDialog();
		((ISupportInitialize)ce0.Properties).BeginInit();
		((ISupportInitialize)ce1.Properties).BeginInit();
		((ISupportInitialize)ce2.Properties).BeginInit();
		((ISupportInitialize)ce3.Properties).BeginInit();
		((ISupportInitialize)ce4.Properties).BeginInit();
		((ISupportInitialize)ce5.Properties).BeginInit();
		((ISupportInitialize)ce6.Properties).BeginInit();
		((ISupportInitialize)ce7.Properties).BeginInit();
		((ISupportInitialize)ce8.Properties).BeginInit();
		((ISupportInitialize)ce9.Properties).BeginInit();
		((ISupportInitialize)ce10.Properties).BeginInit();
		((ISupportInitialize)ce11.Properties).BeginInit();
		((ISupportInitialize)ce12.Properties).BeginInit();
		((ISupportInitialize)ce13.Properties).BeginInit();
		((ISupportInitialize)ce14.Properties).BeginInit();
		((ISupportInitialize)ce15.Properties).BeginInit();
		((ISupportInitialize)ce16.Properties).BeginInit();
		((ISupportInitialize)ce17.Properties).BeginInit();
		((ISupportInitialize)ce18.Properties).BeginInit();
		((ISupportInitialize)ce19.Properties).BeginInit();
		((ISupportInitialize)ce20.Properties).BeginInit();
		((ISupportInitialize)ce21.Properties).BeginInit();
		((ISupportInitialize)ce22.Properties).BeginInit();
		((ISupportInitialize)grpControl).BeginInit();
		((Control)grpControl).SuspendLayout();
		((ISupportInitialize)ce23.Properties).BeginInit();
		((ISupportInitialize)panelControl1).BeginInit();
		((Control)panelControl1).SuspendLayout();
		((ISupportInitialize)tePath.Properties).BeginInit();
		((Control)this).SuspendLayout();
		((Control)ce0).Location = new Point(18, 34);
		((Control)ce0).Name = "ce0";
		ce0.Properties.Caption = "";
		((Control)ce0).Size = new Size(170, 19);
		((Control)ce0).TabIndex = 0;
		((Control)ce1).Location = new Point(18, 59);
		((Control)ce1).Name = "ce1";
		ce1.Properties.Caption = "";
		((Control)ce1).Size = new Size(170, 19);
		((Control)ce1).TabIndex = 1;
		((Control)ce2).Location = new Point(18, 84);
		((Control)ce2).Name = "ce2";
		ce2.Properties.Caption = "";
		((Control)ce2).Size = new Size(170, 19);
		((Control)ce2).TabIndex = 2;
		((Control)ce3).Location = new Point(18, 109);
		((Control)ce3).Name = "ce3";
		ce3.Properties.Caption = "";
		((Control)ce3).Size = new Size(170, 19);
		((Control)ce3).TabIndex = 3;
		((Control)ce4).Location = new Point(18, 134);
		((Control)ce4).Name = "ce4";
		ce4.Properties.Caption = "";
		((Control)ce4).Size = new Size(170, 19);
		((Control)ce4).TabIndex = 4;
		((Control)ce5).Location = new Point(18, 159);
		((Control)ce5).Name = "ce5";
		ce5.Properties.Caption = "";
		((Control)ce5).Size = new Size(170, 19);
		((Control)ce5).TabIndex = 5;
		((Control)ce6).Location = new Point(18, 184);
		((Control)ce6).Name = "ce6";
		ce6.Properties.Caption = "";
		((Control)ce6).Size = new Size(170, 19);
		((Control)ce6).TabIndex = 6;
		((Control)ce7).Location = new Point(18, 209);
		((Control)ce7).Name = "ce7";
		ce7.Properties.Caption = "";
		((Control)ce7).Size = new Size(170, 19);
		((Control)ce7).TabIndex = 7;
		((Control)ce8).Location = new Point(203, 34);
		((Control)ce8).Name = "ce8";
		ce8.Properties.Caption = "";
		((Control)ce8).Size = new Size(170, 19);
		((Control)ce8).TabIndex = 8;
		((Control)ce9).Location = new Point(203, 59);
		((Control)ce9).Name = "ce9";
		ce9.Properties.Caption = "";
		((Control)ce9).Size = new Size(170, 19);
		((Control)ce9).TabIndex = 9;
		((Control)ce10).Location = new Point(203, 84);
		((Control)ce10).Name = "ce10";
		ce10.Properties.Caption = "";
		((Control)ce10).Size = new Size(170, 19);
		((Control)ce10).TabIndex = 10;
		((Control)ce11).Location = new Point(203, 109);
		((Control)ce11).Name = "ce11";
		ce11.Properties.Caption = "";
		((Control)ce11).Size = new Size(170, 19);
		((Control)ce11).TabIndex = 11;
		((Control)ce12).Location = new Point(203, 134);
		((Control)ce12).Name = "ce12";
		ce12.Properties.Caption = "";
		((Control)ce12).Size = new Size(170, 19);
		((Control)ce12).TabIndex = 12;
		((Control)ce13).Location = new Point(203, 159);
		((Control)ce13).Name = "ce13";
		ce13.Properties.Caption = "";
		((Control)ce13).Size = new Size(170, 19);
		((Control)ce13).TabIndex = 13;
		((Control)ce14).Location = new Point(203, 184);
		((Control)ce14).Name = "ce14";
		ce14.Properties.Caption = "";
		((Control)ce14).Size = new Size(170, 19);
		((Control)ce14).TabIndex = 14;
		((Control)ce15).Location = new Point(203, 209);
		((Control)ce15).Name = "ce15";
		ce15.Properties.Caption = "";
		((Control)ce15).Size = new Size(170, 19);
		((Control)ce15).TabIndex = 15;
		((Control)ce16).Location = new Point(388, 34);
		((Control)ce16).Name = "ce16";
		ce16.Properties.Caption = "";
		((Control)ce16).Size = new Size(170, 19);
		((Control)ce16).TabIndex = 16;
		((Control)ce17).Location = new Point(388, 59);
		((Control)ce17).Name = "ce17";
		ce17.Properties.Caption = "";
		((Control)ce17).Size = new Size(170, 19);
		((Control)ce17).TabIndex = 17;
		((Control)ce18).Location = new Point(388, 84);
		((Control)ce18).Name = "ce18";
		ce18.Properties.Caption = "";
		((Control)ce18).Size = new Size(170, 19);
		((Control)ce18).TabIndex = 18;
		((Control)ce19).Location = new Point(388, 109);
		((Control)ce19).Name = "ce19";
		ce19.Properties.Caption = "";
		((Control)ce19).Size = new Size(170, 19);
		((Control)ce19).TabIndex = 19;
		((Control)ce20).Location = new Point(388, 134);
		((Control)ce20).Name = "ce20";
		ce20.Properties.Caption = "";
		((Control)ce20).Size = new Size(170, 19);
		((Control)ce20).TabIndex = 20;
		((Control)ce21).Location = new Point(388, 159);
		((Control)ce21).Name = "ce21";
		ce21.Properties.Caption = "";
		((Control)ce21).Size = new Size(170, 19);
		((Control)ce21).TabIndex = 21;
		((Control)ce22).Location = new Point(388, 184);
		((Control)ce22).Name = "ce22";
		ce22.Properties.Caption = "";
		((Control)ce22).Size = new Size(170, 19);
		((Control)ce22).TabIndex = 22;
		((Control)grpControl).Controls.Add((Control)(object)ce23);
		((Control)grpControl).Controls.Add((Control)(object)ce21);
		((Control)grpControl).Controls.Add((Control)(object)ce22);
		((Control)grpControl).Controls.Add((Control)(object)ce0);
		((Control)grpControl).Controls.Add((Control)(object)ce1);
		((Control)grpControl).Controls.Add((Control)(object)ce20);
		((Control)grpControl).Controls.Add((Control)(object)ce2);
		((Control)grpControl).Controls.Add((Control)(object)ce19);
		((Control)grpControl).Controls.Add((Control)(object)ce3);
		((Control)grpControl).Controls.Add((Control)(object)ce18);
		((Control)grpControl).Controls.Add((Control)(object)ce4);
		((Control)grpControl).Controls.Add((Control)(object)ce17);
		((Control)grpControl).Controls.Add((Control)(object)ce5);
		((Control)grpControl).Controls.Add((Control)(object)ce16);
		((Control)grpControl).Controls.Add((Control)(object)ce6);
		((Control)grpControl).Controls.Add((Control)(object)ce15);
		((Control)grpControl).Controls.Add((Control)(object)ce7);
		((Control)grpControl).Controls.Add((Control)(object)ce14);
		((Control)grpControl).Controls.Add((Control)(object)ce8);
		((Control)grpControl).Controls.Add((Control)(object)ce13);
		((Control)grpControl).Controls.Add((Control)(object)ce9);
		((Control)grpControl).Controls.Add((Control)(object)ce12);
		((Control)grpControl).Controls.Add((Control)(object)ce10);
		((Control)grpControl).Controls.Add((Control)(object)ce11);
		((Control)grpControl).Dock = (DockStyle)5;
		((Control)grpControl).Location = new Point(0, 0);
		((Control)grpControl).Name = "grpControl";
		((Control)grpControl).Size = new Size(574, 325);
		((Control)grpControl).TabIndex = 23;
		((Control)grpControl).Text = "Выберите таблицы для экспорта в Excel";
		((Control)ce23).Location = new Point(388, 209);
		((Control)ce23).Name = "ce23";
		ce23.Properties.Caption = "";
		((Control)ce23).Size = new Size(170, 19);
		((Control)ce23).TabIndex = 23;
		((Control)panelControl1).Controls.Add((Control)(object)labelControl1);
		((Control)panelControl1).Controls.Add((Control)(object)btnPath);
		((Control)panelControl1).Controls.Add((Control)(object)tePath);
		((Control)panelControl1).Controls.Add((Control)(object)btnOK);
		((Control)panelControl1).Controls.Add((Control)(object)btnCancel);
		((Control)panelControl1).Controls.Add((Control)(object)btnClear);
		((Control)panelControl1).Controls.Add((Control)(object)btnAdd);
		((Control)panelControl1).Dock = (DockStyle)2;
		((Control)panelControl1).Location = new Point(0, 241);
		((Control)panelControl1).Name = "panelControl1";
		((Control)panelControl1).Size = new Size(574, 84);
		((Control)panelControl1).TabIndex = 24;
		((Control)labelControl1).Location = new Point(17, 16);
		((Control)labelControl1).Name = "labelControl1";
		((Control)labelControl1).Size = new Size(54, 13);
		((Control)labelControl1).TabIndex = 26;
		((Control)labelControl1).Text = "Имя файла";
		btnPath.Appearance.Font = new Font("Tahoma", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)204);
		btnPath.Appearance.Options.UseFont = true;
		((Control)btnPath).Location = new Point(529, 11);
		((Control)btnPath).Name = "btnPath";
		((Control)btnPath).Size = new Size(35, 23);
		((Control)btnPath).TabIndex = 25;
		((Control)btnPath).Text = "...";
		((Control)btnPath).Click += btnPath_Click;
		((Control)tePath).Location = new Point(80, 13);
		((Control)tePath).Name = "tePath";
		((Control)tePath).Size = new Size(437, 20);
		((Control)tePath).TabIndex = 0;
		((Control)btnOK).Location = new Point(358, 49);
		((Control)btnOK).Name = "btnOK";
		((Control)btnOK).Size = new Size(100, 23);
		((Control)btnOK).TabIndex = 4;
		((Control)btnOK).Text = "OK";
		((Control)btnOK).Click += btnOK_Click;
		btnCancel.DialogResult = (DialogResult)2;
		((Control)btnCancel).Location = new Point(464, 49);
		((Control)btnCancel).Name = "btnCancel";
		((Control)btnCancel).Size = new Size(100, 23);
		((Control)btnCancel).TabIndex = 3;
		((Control)btnCancel).Text = "Отмена";
		((Control)btnCancel).Click += btnCancel_Click;
		((Control)btnClear).Location = new Point(118, 49);
		((Control)btnClear).Name = "btnClear";
		((Control)btnClear).Size = new Size(100, 23);
		((Control)btnClear).TabIndex = 2;
		((Control)btnClear).Text = "Очистить все";
		((Control)btnClear).Click += btnClear_Click;
		((Control)btnAdd).Location = new Point(12, 49);
		((Control)btnAdd).Name = "btnAdd";
		((Control)btnAdd).Size = new Size(100, 23);
		((Control)btnAdd).TabIndex = 1;
		((Control)btnAdd).Text = "Выбрать все";
		((Control)btnAdd).Click += btnAdd_Click;
		((FileDialog)saveFileDialog1).Filter = "Excel файлы|*.xlsx|Все файлы|*.*";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).CancelButton = (IButtonControl)(object)btnCancel;
		((Form)this).ClientSize = new Size(574, 325);
		((Control)this).Controls.Add((Control)(object)panelControl1);
		((Control)this).Controls.Add((Control)(object)grpControl);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "ExportForm";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "Экспорт в Excel";
		((Form)this).Load += ExportForm_Load;
		((ISupportInitialize)ce0.Properties).EndInit();
		((ISupportInitialize)ce1.Properties).EndInit();
		((ISupportInitialize)ce2.Properties).EndInit();
		((ISupportInitialize)ce3.Properties).EndInit();
		((ISupportInitialize)ce4.Properties).EndInit();
		((ISupportInitialize)ce5.Properties).EndInit();
		((ISupportInitialize)ce6.Properties).EndInit();
		((ISupportInitialize)ce7.Properties).EndInit();
		((ISupportInitialize)ce8.Properties).EndInit();
		((ISupportInitialize)ce9.Properties).EndInit();
		((ISupportInitialize)ce10.Properties).EndInit();
		((ISupportInitialize)ce11.Properties).EndInit();
		((ISupportInitialize)ce12.Properties).EndInit();
		((ISupportInitialize)ce13.Properties).EndInit();
		((ISupportInitialize)ce14.Properties).EndInit();
		((ISupportInitialize)ce15.Properties).EndInit();
		((ISupportInitialize)ce16.Properties).EndInit();
		((ISupportInitialize)ce17.Properties).EndInit();
		((ISupportInitialize)ce18.Properties).EndInit();
		((ISupportInitialize)ce19.Properties).EndInit();
		((ISupportInitialize)ce20.Properties).EndInit();
		((ISupportInitialize)ce21.Properties).EndInit();
		((ISupportInitialize)ce22.Properties).EndInit();
		((ISupportInitialize)grpControl).EndInit();
		((Control)grpControl).ResumeLayout(false);
		((ISupportInitialize)ce23.Properties).EndInit();
		((ISupportInitialize)panelControl1).EndInit();
		((Control)panelControl1).ResumeLayout(false);
		((Control)panelControl1).PerformLayout();
		((ISupportInitialize)tePath.Properties).EndInit();
		((Control)this).ResumeLayout(false);
	}

	public ExportForm()
	{
		InitializeComponent();
	}

	private void ExportForm_Load(object sender, EventArgs e)
	{
		myFile = ((Control)this).Text;
		((Control)this).Text = myName;
		List<string> list = (List<string>)((Control)this).Tag;
		ControlCollection controls = ((Control)grpControl).Controls;
		for (int i = 0; i < list.Count; i++)
		{
			CheckEdit checkEdit = (CheckEdit)(object)controls.Find("ce" + i, false)[0];
			if (list[i].Substring(0, 1) == "~")
			{
				((Control)checkEdit).Text = list[i].Replace("~", "");
				((Control)checkEdit).Enabled = false;
			}
			else
			{
				((Control)checkEdit).Text = list[i];
				checkEdit.Checked = true;
			}
		}
		MakeDisable();
		((Control)tePath).Select();
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = (List<string>)((Control)this).Tag;
		ControlCollection controls = ((Control)grpControl).Controls;
		if (IsChecked())
		{
			if (((Control)tePath).Text != "")
			{
				for (int i = 0; i < ((ArrangedElementCollection)controls).Count; i++)
				{
					CheckEdit checkEdit = (CheckEdit)(object)controls[i];
					if (!checkEdit.Checked)
					{
						if (!((Control)checkEdit).Enabled && !string.IsNullOrEmpty(((Control)checkEdit).Text))
						{
							((Control)checkEdit).Text = "~" + ((Control)checkEdit).Text;
						}
						int num = list.IndexOf(((Control)checkEdit).Text);
						if (num >= 0)
						{
							list.RemoveAt(num);
						}
					}
				}
				((Control)this).Text = ((Control)tePath).Text;
				((Form)this).DialogResult = (DialogResult)1;
				((Form)this).Close();
			}
			else
			{
				MessageBox.Show("Не указан файл экспорта.", myName, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}
		else
		{
			MessageBox.Show("Нет выбранных таблиц для экспорта.", myName, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void btnPath_Click(object sender, EventArgs e)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		((FileDialog)saveFileDialog1).FileName = Path.GetFileName(myFile).Replace(Path.GetExtension(myFile), "") + ".xlsx";
		if ((int)((CommonDialog)saveFileDialog1).ShowDialog() == 1)
		{
			((Control)tePath).Text = ((FileDialog)saveFileDialog1).FileName;
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		((Form)this).Close();
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		ControlCollection controls = ((Control)grpControl).Controls;
		for (int i = 0; i < ((ArrangedElementCollection)controls).Count; i++)
		{
			CheckEdit checkEdit = (CheckEdit)(object)controls[i];
			if (((Control)checkEdit).Text != "" && ((Control)checkEdit).Enabled)
			{
				checkEdit.Checked = true;
			}
		}
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		ControlCollection controls = ((Control)grpControl).Controls;
		for (int i = 0; i < ((ArrangedElementCollection)controls).Count; i++)
		{
			CheckEdit checkEdit = (CheckEdit)(object)controls[i];
			checkEdit.Checked = false;
		}
	}

	private bool IsChecked()
	{
		ControlCollection controls = ((Control)grpControl).Controls;
		for (int i = 0; i < ((ArrangedElementCollection)controls).Count; i++)
		{
			CheckEdit checkEdit = (CheckEdit)(object)controls[i];
			if (checkEdit.Checked)
			{
				return true;
			}
		}
		return false;
	}

	private void MakeDisable()
	{
		ControlCollection controls = ((Control)grpControl).Controls;
		for (int i = 0; i < ((ArrangedElementCollection)controls).Count; i++)
		{
			CheckEdit checkEdit = (CheckEdit)(object)controls[i];
			if (((Control)checkEdit).Text == "")
			{
				((Control)checkEdit).Enabled = false;
			}
		}
	}
}
