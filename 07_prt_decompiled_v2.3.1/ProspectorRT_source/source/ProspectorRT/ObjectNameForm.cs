using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace ProspectorRT;

public class ObjectNameForm : Form
{
	private IContainer components;

	private SimpleButton btnPath;

	private TextEdit tePath;

	private SimpleButton btnOK;

	private SimpleButton btnCancel;

	private RadioGroup rgObjectName;

	private OpenFileDialog openFileDialog1;

	private ComboBoxEdit cbCodePg;

	private LabelControl labelControl1;

	private string _Path = "";

	private int _Ver = 3;

	private int _CodePg;

	public string sPath
	{
		get
		{
			return _Path;
		}
		set
		{
			_Path = value;
		}
	}

	public int iVersion
	{
		get
		{
			return _Ver;
		}
		set
		{
			_Ver = value;
		}
	}

	public int iCodePage
	{
		get
		{
			return _CodePg;
		}
		set
		{
			_CodePg = value;
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		btnPath = new SimpleButton();
		tePath = new TextEdit();
		btnOK = new SimpleButton();
		btnCancel = new SimpleButton();
		rgObjectName = new RadioGroup();
		openFileDialog1 = new OpenFileDialog();
		cbCodePg = new ComboBoxEdit();
		labelControl1 = new LabelControl();
		((ISupportInitialize)tePath.Properties).BeginInit();
		((ISupportInitialize)rgObjectName.Properties).BeginInit();
		((ISupportInitialize)cbCodePg.Properties).BeginInit();
		((Control)this).SuspendLayout();
		btnPath.Appearance.Font = new Font("Tahoma", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)204);
		btnPath.Appearance.Options.UseFont = true;
		((Control)btnPath).Enabled = false;
		((Control)btnPath).Location = new Point(375, 130);
		((Control)btnPath).Name = "btnPath";
		((Control)btnPath).Size = new Size(35, 23);
		((Control)btnPath).TabIndex = 2;
		((Control)btnPath).Text = "...";
		((Control)btnPath).Click += btnPath_Click;
		((Control)tePath).Enabled = false;
		((Control)tePath).Location = new Point(93, 132);
		((Control)tePath).Name = "tePath";
		tePath.Properties.ReadOnly = true;
		((Control)tePath).Size = new Size(269, 20);
		((Control)tePath).TabIndex = 1;
		((Control)btnOK).Location = new Point(204, 163);
		((Control)btnOK).Name = "btnOK";
		((Control)btnOK).Size = new Size(100, 23);
		((Control)btnOK).TabIndex = 4;
		((Control)btnOK).Text = "OK";
		((Control)btnOK).Click += btnOK_Click;
		btnCancel.DialogResult = (DialogResult)2;
		((Control)btnCancel).Location = new Point(310, 163);
		((Control)btnCancel).Name = "btnCancel";
		((Control)btnCancel).Size = new Size(100, 23);
		((Control)btnCancel).TabIndex = 5;
		((Control)btnCancel).Text = "Отмена";
		((Control)btnCancel).Click += btnCancel_Click;
		((Control)rgObjectName).Location = new Point(12, 2);
		((Control)rgObjectName).Name = "rgObjectName";
		rgObjectName.Properties.Appearance.BackColor = SystemColors.Control;
		rgObjectName.Properties.Appearance.Options.UseBackColor = true;
		rgObjectName.Properties.BorderStyle = BorderStyles.NoBorder;
		rgObjectName.Properties.Items.AddRange(new RadioGroupItem[5]
		{
			new RadioGroupItem(null, "The Shadow of Death"),
			new RadioGroupItem(null, "Дыхание Смерти"),
			new RadioGroupItem(null, "Полное Собрание"),
			new RadioGroupItem(null, "По умолчанию"),
			new RadioGroupItem(null, "Из файла")
		});
		((Control)rgObjectName).Size = new Size(141, 162);
		((Control)rgObjectName).TabIndex = 0;
		rgObjectName.SelectedIndexChanged += rgObjectName_SelectedIndexChanged;
		((FileDialog)openFileDialog1).Filter = "Text Files|*.TXT|All Files|*.*";
		((FileDialog)openFileDialog1).SupportMultiDottedExtensions = true;
		cbCodePg.EditValue = "ANSI (1251)";
		((Control)cbCodePg).Enabled = false;
		((Control)cbCodePg).Location = new Point(93, 164);
		((Control)cbCodePg).Name = "cbCodePg";
		cbCodePg.Properties.Buttons.AddRange(new EditorButton[1]
		{
			new EditorButton(ButtonPredefines.Combo)
		});
		cbCodePg.Properties.Items.AddRange(new object[3] { "ANSI (1251)", "OEM (866)", "UTF8" });
		cbCodePg.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
		((Control)cbCodePg).Size = new Size(87, 20);
		((Control)cbCodePg).TabIndex = 3;
		((Control)labelControl1).Location = new Point(34, 167);
		((Control)labelControl1).Name = "labelControl1";
		((Control)labelControl1).Size = new Size(56, 13);
		((Control)labelControl1).TabIndex = 6;
		((Control)labelControl1).Text = "Кодировка";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(423, 195);
		((Control)this).Controls.Add((Control)(object)labelControl1);
		((Control)this).Controls.Add((Control)(object)cbCodePg);
		((Control)this).Controls.Add((Control)(object)tePath);
		((Control)this).Controls.Add((Control)(object)rgObjectName);
		((Control)this).Controls.Add((Control)(object)btnPath);
		((Control)this).Controls.Add((Control)(object)btnOK);
		((Control)this).Controls.Add((Control)(object)btnCancel);
		((Form)this).FormBorderStyle = (FormBorderStyle)5;
		((Control)this).Name = "ObjectNameForm";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "Названия Объектов";
		((Form)this).Load += ObjectNameForm_Load;
		((ISupportInitialize)tePath.Properties).EndInit();
		((ISupportInitialize)rgObjectName.Properties).EndInit();
		((ISupportInitialize)cbCodePg.Properties).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	public ObjectNameForm()
	{
		InitializeComponent();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		((Form)this).Close();
	}

	private void btnPath_Click(object sender, EventArgs e)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		if (((Control)tePath).Text != "")
		{
			((FileDialog)openFileDialog1).InitialDirectory = Path.GetDirectoryName(((Control)tePath).Text);
		}
		else
		{
			((FileDialog)openFileDialog1).InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Data";
		}
		if ((int)((CommonDialog)openFileDialog1).ShowDialog() == 1)
		{
			((Control)tePath).Text = ((FileDialog)openFileDialog1).FileName;
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		_Ver = rgObjectName.SelectedIndex;
		if (_Ver == 4)
		{
			_Path = ((Control)tePath).Text;
			_CodePg = cbCodePg.SelectedIndex;
			if (_Path == "")
			{
				MessageBox.Show("Не указан файл с названиями объектов", ((Control)this).Text, (MessageBoxButtons)0, (MessageBoxIcon)64);
				return;
			}
		}
		((Form)this).DialogResult = (DialogResult)1;
		((Form)this).Close();
	}

	private void rgObjectName_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (rgObjectName.SelectedIndex == 4)
		{
			((Control)tePath).Enabled = true;
			((Control)btnPath).Enabled = true;
			((Control)cbCodePg).Enabled = true;
		}
		else
		{
			((Control)tePath).Enabled = false;
			((Control)btnPath).Enabled = false;
			((Control)cbCodePg).Enabled = false;
		}
	}

	private void ObjectNameForm_Load(object sender, EventArgs e)
	{
		rgObjectName.SelectedIndex = _Ver;
		if (_Ver == 4)
		{
			((Control)tePath).Text = _Path;
			cbCodePg.SelectedIndex = _CodePg;
		}
	}
}
