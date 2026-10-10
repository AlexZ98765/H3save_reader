using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace ProspectorRT;

public class LMOracle
{
	public struct THero
	{
		public byte Class;

		public byte Level;

		public byte TreeNumber;

		public byte LastWisdom;

		public byte LastMagic;

		public byte Attack;

		public byte Defense;

		public byte SpellPower;

		public byte Knowledge;

		public byte SkillCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
		public byte[] Skill;
	}

	public struct TSecondary
	{
		public byte Left;

		public byte Right;
	}

	public struct TSkillOffer
	{
		public byte Primary;

		public TSecondary Secondary;
	}

	public struct TWeights
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] PW;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] PW10;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
		public byte[] SW;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
		public byte[] SR;
	}

	public struct TSkillExcept
	{
		public byte Last_Wisdom;

		public byte Last_Magic;

		public byte Delta_Wisdom;

		public byte Delta_Magic;
	}

	public enum PrimarySkill
	{
		ATTACK,
		DEFENSE,
		SPELL_POWER,
		KNOWLEDGE
	}

	public enum SecondarySkill
	{
		PATHFINDING,
		ARCHERY,
		LOGISTICS,
		SCOUTING,
		DIPLOMACY,
		NAVIGATION,
		LEADERSHIP,
		WISDOM,
		MYSTICISM,
		LUCK,
		BALLISTICS,
		EAGLE_EYE,
		NECROMANCY,
		ESTATES,
		FIRE_MAGIC,
		AIR_MAGIC,
		WATER_MAGIC,
		EARTH_MAGIC,
		SCHOLAR,
		TACTICS,
		ARTILLERY,
		LEARNING,
		OFFENSE,
		ARMORER,
		INTELLIGENCE,
		SORCERY,
		RESISTANCE,
		FIRST_AID,
		NONE
	}

	public enum LevelSkill
	{
		BASIC = 1,
		ADVANCED,
		EXPERT
	}

	private static byte[,] _PW;

	private static byte[,] _PW10;

	private static byte[,] _SW;

	private static byte[] _SR;

	private static int _MaxLevel;

	private static int maxlvl;

	private static byte _Version;

	private static bool LoadDll = false;

	private static int BBFlag = 1;

	private static string[] _aLevelSkill;

	private static DataTable _TblSecondarySkill;

	private static Form _fOwner;

	public static byte[,] PW
	{
		set
		{
			_PW = value;
		}
	}

	public static byte[,] PW10
	{
		set
		{
			_PW10 = value;
		}
	}

	public static byte[,] SW
	{
		set
		{
			_SW = value;
		}
	}

	public static byte[] SR
	{
		set
		{
			_SR = value;
		}
	}

	public static int MaxLevel
	{
		set
		{
			maxlvl = (_MaxLevel = value - 1);
		}
	}

	public static byte Version
	{
		set
		{
			_Version = value;
		}
	}

	public static string[] aLevelSkill
	{
		set
		{
			_aLevelSkill = value;
		}
	}

	public static DataTable TblSecondarySkill
	{
		set
		{
			_TblSecondarySkill = value;
		}
	}

	public static Form fOwner
	{
		set
		{
			_fOwner = value;
		}
	}

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern uint GetProcessID(string ProcName);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern uint GetHStartPoint(IntPtr hProcess, byte Version);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern void RAMHero(IntPtr hProcess, uint HStartPoint, int ID, ref THero Hero);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern int RAMWeights(IntPtr hProcess, uint HStartPoint, byte Version, byte Class, ref TWeights Weights);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern int LevelUp(ref THero Hero, ref TWeights Weights, ref TSkillOffer SkillOffer, int Option);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern byte isStandardWeights(IntPtr hProcess, uint HStartPoint, byte Version, int ID);

	[DllImport("LMOracle.SkillTreeAPI.dll", CharSet = CharSet.Unicode)]
	public static extern int RAMBBWeights(IntPtr hProcess, uint HStartPoint, byte Version, byte Class, ref TWeights Weights);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
	public static extern IntPtr LoadLibrary(string FileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
	public static extern IntPtr OpenProcess(int dwAccess, bool bInheritHandle, uint dwProcessId);

	public static int Rand(uint Seed, ref uint R)
	{
		R = 214013 * Seed + 2531011;
		return (int)((R >> 16) & 0x7FFF);
	}

	public static int LevelUpA(THero Hero, TWeights Weights, TSkillOffer SkillOffer, byte Option)
	{
		byte level = Hero.Level;
		byte b = (byte)(Option & 3u);
		level = ((b == 0) ? ((byte)(level + 1)) : (++Hero.Level));
		uint R = 0u;
		int num = Rand((uint)(214013 * level + 156823 * Hero.TreeNumber + 154079), ref R);
		num %= 100;
		num++;
		if ((Option & 4u) != 0)
		{
			int num2 = ((level > 9) ? (Weights.PW10[0] + Weights.PW10[1]) : (Weights.PW[0] + Weights.PW[1]));
			if (num2 > 1)
			{
				num = Rand(R, ref R);
				num %= num2;
				num++;
			}
			else
			{
				num = num2;
			}
		}
		int num3 = 0;
		while (true)
		{
			int num4 = ((level > 9) ? Weights.PW10[num3] : Weights.PW[num3]);
			if (num <= num4)
			{
				break;
			}
			num -= num4;
			num3++;
		}
		SkillOffer.Primary = (byte)num3;
		if (b != 0)
		{
			switch (num3)
			{
			case 0:
				Hero.Attack++;
				break;
			case 1:
				Hero.Defense++;
				break;
			case 2:
				Hero.SpellPower++;
				break;
			case 3:
				Hero.Knowledge++;
				break;
			}
		}
		TSkillExcept skillExcept = default(TSkillExcept);
		skillExcept.Last_Wisdom = Hero.LastWisdom;
		skillExcept.Last_Magic = Hero.LastMagic;
		skillExcept.Delta_Wisdom = 6;
		skillExcept.Delta_Magic = 4;
		byte minLevel = ((Hero.SkillCount > 7) ? ((byte)1) : ((byte)0));
		if ((Hero.Class & 0x11) == 1)
		{
			skillExcept.Delta_Wisdom = (skillExcept.Delta_Magic = 3);
		}
		byte skillOffer = GetSkillOffer(Hero, Weights, level, skillExcept, 1, 3, 28, ref R);
		if (skillOffer == 28)
		{
			skillOffer = GetSkillOffer(Hero, Weights, level, skillExcept, minLevel, 3, 28, ref R);
		}
		if (skillOffer == 7)
		{
			skillExcept.Last_Wisdom = level;
		}
		if (skillOffer >= 14 && skillOffer <= 17)
		{
			skillExcept.Last_Magic = level;
		}
		byte skillOffer2 = GetSkillOffer(Hero, Weights, level, skillExcept, minLevel, 1, skillOffer, ref R);
		if (skillOffer2 == 28)
		{
			skillOffer2 = GetSkillOffer(Hero, Weights, level, skillExcept, minLevel, 3, skillOffer, ref R);
		}
		if (skillOffer2 == 7)
		{
			skillExcept.Last_Wisdom = level;
		}
		if (skillOffer2 >= 14 && skillOffer2 <= 17)
		{
			skillExcept.Last_Magic = level;
		}
		SkillOffer.Secondary.Left = skillOffer;
		SkillOffer.Secondary.Right = skillOffer2;
		if (b != 0)
		{
			Hero.LastWisdom = skillExcept.Last_Wisdom;
			Hero.LastMagic = skillExcept.Last_Magic;
			byte b2 = ((b == 1) ? skillOffer : skillOffer2);
			if (b2 < 28)
			{
				if (Hero.Skill[b2] == 0)
				{
					Hero.SkillCount++;
				}
				Hero.Skill[b2]++;
			}
			return 0;
		}
		return 0;
	}

	public static byte GetSkillOffer(THero Hero, TWeights Weights, byte Level, TSkillExcept SkillExcept, byte MinLevel, byte MaxLevel, byte LeftSkill, ref uint R)
	{
		if (MinLevel >= MaxLevel)
		{
			return 28;
		}
		if (SkillExcept.Last_Wisdom + SkillExcept.Delta_Wisdom <= Level && Hero.Skill[7] >= MinLevel && Hero.Skill[7] < MaxLevel && Weights.SR[7] == 0 && LeftSkill != 7)
		{
			return 7;
		}
		int num = 0;
		if (SkillExcept.Last_Magic + SkillExcept.Delta_Magic <= Level && LeftSkill != 14 && LeftSkill != 15 && LeftSkill != 16 && LeftSkill != 17)
		{
			for (int i = 14; i <= 17; i++)
			{
				if (Hero.Skill[i] >= MinLevel && Hero.Skill[i] < MaxLevel && Weights.SR[i] == 0)
				{
					num = ((Hero.Skill[i] == 0) ? (num + Weights.SW[i]) : (num + 1));
				}
			}
			if (num != 0)
			{
				int num2;
				if (num > 1)
				{
					num2 = Rand(R, ref R);
					num2 %= num;
					num2++;
				}
				else
				{
					num2 = 1;
				}
				num = num2;
				for (int j = 14; j <= 17; j++)
				{
					if (Hero.Skill[j] >= MinLevel && Hero.Skill[j] < MaxLevel && Weights.SR[j] == 0)
					{
						num = ((Hero.Skill[j] == 0) ? (num - Weights.SW[j]) : (num - 1));
					}
					if (num <= 0)
					{
						return (byte)j;
					}
				}
			}
		}
		num = 0;
		for (int k = 0; k < 28; k++)
		{
			if (Hero.Skill[k] >= MinLevel && Hero.Skill[k] < MaxLevel && k != LeftSkill)
			{
				if (Weights.SR[k] == 0 && Weights.SW[k] > 0)
				{
					num += Weights.SW[k];
				}
				else if (Hero.Skill[k] > 0)
				{
					num++;
				}
			}
		}
		if (num != 0)
		{
			int num2;
			if (num > 1)
			{
				num2 = Rand(R, ref R);
				num2 %= num;
				num2++;
			}
			else
			{
				num2 = 1;
			}
			num = num2;
			for (int l = 0; l < 28; l++)
			{
				if (Hero.Skill[l] >= MinLevel && Hero.Skill[l] < MaxLevel && l != LeftSkill)
				{
					if (Weights.SR[l] == 0 && Weights.SW[l] > 0)
					{
						num -= Weights.SW[l];
					}
					else if (Hero.Skill[l] > 0)
					{
						num--;
					}
				}
				if (num <= 0)
				{
					return (byte)l;
				}
			}
		}
		return 28;
	}

	public static void ShowSkillTreeA(GridView view)
	{
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (view.RowCount <= 0 || view.GetDataRow(0)[0] == DBNull.Value)
			{
				return;
			}
			if (!LoadDll)
			{
				if (!File.Exists(Application.StartupPath + "\\LMOracle.SkillTreeAPI.dll"))
				{
					MessageBox.Show("Библиотека LMOracle.SkillTreeAPI.dll не найдена.\r\nПостроение дерева прокачки невозможно.", "Ошибка загрузки LMOracle.SkillTreeAPI.dll", (MessageBoxButtons)0, (MessageBoxIcon)16);
					return;
				}
				LoadLibrary(Application.StartupPath + "\\LMOracle.SkillTreeAPI.dll");
				LoadDll = true;
			}
			BBFlag = 1;
			_MaxLevel = maxlvl;
			THero Hero = default(THero);
			TWeights Weights = default(TWeights);
			TSkillOffer skillOffer = default(TSkillOffer);
			GetNewStruct(ref Hero, ref Weights);
			DataRow focusedDataRow = view.GetFocusedDataRow();
			GetNewHero(ref Hero, focusedDataRow);
			GetNewWeights(ref Weights, Hero.Class);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 28; i++)
			{
				num += Hero.Skill[i];
				if (_SR[i] == 0 && _SW[Hero.Class, i] > 0)
				{
					num2 += 3 - Hero.Skill[i];
				}
				else if (Hero.Skill[i] > 0)
				{
					num2 += 3 - Hero.Skill[i];
				}
			}
			if (num2 < _MaxLevel)
			{
				_MaxLevel = num2;
			}
			if (24 - num <= _MaxLevel)
			{
				_MaxLevel = 24 - num - 1;
			}
			SkillTreeForm skillTreeForm = new SkillTreeForm();
			skillTreeForm.treeSkill.BeginUnboundLoad();
			TreeList treeSkill = skillTreeForm.treeSkill;
			object[] nodeData = new object[1];
			TreeListNode treeListNode = treeSkill.AppendNode(nodeData, -1);
			treeListNode.SetValue(0, focusedDataRow[1].ToString() + "  " + Hero.Level + "  [" + Hero.Attack.ToString() + "-" + Hero.Defense.ToString() + "-" + Hero.SpellPower.ToString() + "-" + Hero.Knowledge.ToString() + "]  " + focusedDataRow["SecondarySkill"].ToString());
			treeListNode.Tag = Hero;
			if (_MaxLevel >= 0)
			{
				GetNextLevel(treeListNode, Weights, skillOffer, skillTreeForm);
				if (_MaxLevel > 1)
				{
					if (treeListNode.Nodes.Count > 0)
					{
						GetNextNode(treeListNode.Nodes[0], Weights, skillOffer, skillTreeForm);
					}
					if (treeListNode.Nodes.Count > 1)
					{
						GetNextNode(treeListNode.Nodes[1], Weights, skillOffer, skillTreeForm);
					}
				}
			}
			skillTreeForm.treeSkill.EndUnboundLoad();
			if (treeListNode.Nodes.Count > 0)
			{
				skillTreeForm.treeSkill.MakeNodeVisible(treeListNode.Nodes[0]);
			}
			((Control)skillTreeForm).Text = ((Control)skillTreeForm).Text;
			skillTreeForm.fOwner = _fOwner;
			_fOwner = null;
			((Form)skillTreeForm).ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	public static void ShowSkillTree(GridView view)
	{
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (view.RowCount <= 0 || view.GetDataRow(0)[0] == DBNull.Value)
			{
				return;
			}
			if (!LoadDll)
			{
				if (!File.Exists(Application.StartupPath + "\\LMOracle.SkillTreeAPI.dll"))
				{
					MessageBox.Show("Библиотека LMOracle.SkillTreeAPI.dll не найдена.\r\nПостроение дерева прокачки невозможно.", "Ошибка загрузки LMOracle.SkillTreeAPI.dll", (MessageBoxButtons)0, (MessageBoxIcon)16);
					return;
				}
				LoadLibrary(Application.StartupPath + "\\LMOracle.SkillTreeAPI.dll");
				LoadDll = true;
			}
			string text = ((_Version != 0) ? "Heroes of Might and Magic III" : "Герои Меча и Магии III");
			uint num = 0u;
			Process[] processes = Process.GetProcesses();
			Process[] array = processes;
			foreach (Process process in array)
			{
				if (process.MainWindowTitle == text)
				{
					num = (uint)process.Id;
					break;
				}
			}
			if (num == 0)
			{
				MessageBox.Show("Процесс \"Heroes of Might and Magic 3\" не найден.\r\nЗапустите игру или выберите соответствующую версию.", "Ошибка доступа к процессу", (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
			IntPtr hProcess = OpenProcess(16, bInheritHandle: false, num);
			uint hStartPoint = GetHStartPoint(hProcess, _Version);
			_MaxLevel = maxlvl;
			THero Hero = default(THero);
			TWeights Weights = default(TWeights);
			TSkillOffer skillOffer = default(TSkillOffer);
			DataRow focusedDataRow = view.GetFocusedDataRow();
			int iD = (int)focusedDataRow[0];
			RAMHero(hProcess, hStartPoint, iD, ref Hero);
			if (Hero.Level == 0)
			{
				MessageBox.Show("Выберите соответствующую версию или в \"Heroes of Might and Magic 3\" загрузите сейв \"" + Path.GetFileName(CommonSetting.MyFile) + "\".", "Ошибка построения дерева прокачки", (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
			BBFlag = isStandardWeights(hProcess, hStartPoint, _Version, iD);
			if (BBFlag == 1)
			{
				RAMWeights(hProcess, hStartPoint, _Version, Hero.Class, ref Weights);
			}
			else
			{
				RAMBBWeights(hProcess, hStartPoint, _Version, Hero.Class, ref Weights);
			}
			int num2 = 0;
			for (int j = 0; j < 28; j++)
			{
				num2 += Hero.Skill[j];
			}
			if (24 - num2 <= _MaxLevel)
			{
				_MaxLevel = 24 - num2 - 1;
			}
			SkillTreeForm skillTreeForm = new SkillTreeForm();
			skillTreeForm.treeSkill.BeginUnboundLoad();
			TreeList treeSkill = skillTreeForm.treeSkill;
			object[] nodeData = new object[1];
			TreeListNode treeListNode = treeSkill.AppendNode(nodeData, -1);
			treeListNode.SetValue(0, focusedDataRow[1].ToString() + "  " + Hero.Level + "  [" + Hero.Attack.ToString() + "-" + Hero.Defense.ToString() + "-" + Hero.SpellPower.ToString() + "-" + Hero.Knowledge.ToString() + "]  " + focusedDataRow["SecondarySkill"].ToString());
			treeListNode.Tag = Hero;
			if (_MaxLevel >= 0)
			{
				GetNextLevel(treeListNode, Weights, skillOffer, skillTreeForm);
				if (_MaxLevel > 1)
				{
					if (treeListNode.Nodes.Count > 0)
					{
						GetNextNode(treeListNode.Nodes[0], Weights, skillOffer, skillTreeForm);
					}
					if (treeListNode.Nodes.Count > 1)
					{
						GetNextNode(treeListNode.Nodes[1], Weights, skillOffer, skillTreeForm);
					}
				}
			}
			skillTreeForm.treeSkill.EndUnboundLoad();
			if (treeListNode.Nodes.Count > 0)
			{
				skillTreeForm.treeSkill.MakeNodeVisible(treeListNode.Nodes[0]);
			}
			if (_Version == 2)
			{
				((Control)skillTreeForm).Text = ((Control)skillTreeForm).Text + "  HoMM3 3.2 EN";
			}
			else if (_Version == 1)
			{
				((Control)skillTreeForm).Text = ((Control)skillTreeForm).Text + "  HoMM3 4.0 EN";
			}
			else if (_Version == 0)
			{
				((Control)skillTreeForm).Text = ((Control)skillTreeForm).Text + "  HoMM3 4.0 RU";
			}
			skillTreeForm.fOwner = _fOwner;
			_fOwner = null;
			((Form)skillTreeForm).ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private static void GetNextNode(TreeListNode nodeP, TWeights Weights, TSkillOffer SkillOffer, SkillTreeForm frm)
	{
		GetNextLevel(nodeP, Weights, SkillOffer, frm);
		if (nodeP.Level > 1 && nodeP.ParentNode.Nodes.Count == 2)
		{
			GetNextLevel(nodeP.ParentNode.Nodes[1], Weights, SkillOffer, frm);
			if (_MaxLevel - nodeP.Level > 0)
			{
				GetNextNode(nodeP.ParentNode.Nodes[1].Nodes[0], Weights, SkillOffer, frm);
			}
		}
		if (nodeP.Level < _MaxLevel && nodeP.Nodes.Count > 0)
		{
			GetNextNode(nodeP.Nodes[0], Weights, SkillOffer, frm);
		}
	}

	private static void GetNextLevel(TreeListNode nodeP, TWeights Weights, TSkillOffer SkillOffer, SkillTreeForm frm)
	{
		THero Hero = (THero)nodeP.Tag;
		LevelUp(ref Hero, ref Weights, ref SkillOffer, (BBFlag == 1) ? 1 : 5);
		if (SkillOffer.Secondary.Left < 28)
		{
			TreeList treeSkill = frm.treeSkill;
			object[] nodeData = new object[1];
			TreeListNode treeListNode = treeSkill.AppendNode(nodeData, nodeP);
			treeListNode.SetValue(0, string.Concat(_aLevelSkill[Hero.Skill[SkillOffer.Secondary.Left] - 1], _TblSecondarySkill.Rows[SkillOffer.Secondary.Left][1], "  [", Hero.Attack.ToString(), "-", Hero.Defense.ToString(), "-", Hero.SpellPower.ToString(), "-", Hero.Knowledge.ToString(), "]  ", Hero.Level.ToString()));
			treeListNode.Tag = Hero;
		}
		Hero = (THero)nodeP.Tag;
		LevelUp(ref Hero, ref Weights, ref SkillOffer, (BBFlag == 1) ? 2 : 6);
		if (SkillOffer.Secondary.Right < 28)
		{
			TreeList treeSkill2 = frm.treeSkill;
			object[] nodeData2 = new object[1];
			TreeListNode treeListNode = treeSkill2.AppendNode(nodeData2, nodeP);
			treeListNode.SetValue(0, string.Concat(_aLevelSkill[Hero.Skill[SkillOffer.Secondary.Right] - 1], _TblSecondarySkill.Rows[SkillOffer.Secondary.Right][1], "  [", Hero.Attack.ToString(), "-", Hero.Defense.ToString(), "-", Hero.SpellPower.ToString(), "-", Hero.Knowledge.ToString(), "]  ", Hero.Level.ToString()));
			treeListNode.Tag = Hero;
		}
	}

	private static void GetNewStruct(ref THero Hero, ref TWeights Weights)
	{
		Hero.Skill = new byte[28];
		Weights.PW = new byte[4];
		Weights.PW10 = new byte[4];
		Weights.SW = new byte[28];
		Weights.SR = new byte[28];
	}

	private static void GetNewHero(ref THero Hero, DataRow row)
	{
		Hero.Level = (byte)(int)row["Level"];
		Hero.Class = (byte)(int)row["ClassID"];
		Hero.TreeNumber = (byte)(int)row["TreeNumber"];
		Hero.LastWisdom = (byte)(int)row["LastWisdom"];
		Hero.LastMagic = (byte)(int)row["LastMagic"];
		string[] array = row["PrimarySkill"].ToString().Split(new string[1] { "-" }, StringSplitOptions.None);
		Hero.Attack = (byte)int.Parse(array[0].Substring(1));
		Hero.Defense = (byte)int.Parse(array[1].Substring(1));
		Hero.SpellPower = (byte)int.Parse(array[2].Substring(1));
		Hero.Knowledge = (byte)int.Parse(array[3].Substring(1));
		string[] array2 = row["SecondarySkill"].ToString().Split(new string[1] { ", " }, StringSplitOptions.None);
		if (array2.Length == 1 && array2[0] == "")
		{
			array2 = new string[0];
		}
		Hero.SkillCount = (byte)array2.Length;
		for (int i = 0; i < 28; i++)
		{
			Hero.Skill[i] = 0;
		}
		for (int j = 0; j < array2.Length; j++)
		{
			int num = (int)_TblSecondarySkill.Select("Name = '" + array2[j].Substring(2) + "'")[0][0];
			int sSkillLevel = GetSSkillLevel(array2[j].Substring(0, 2));
			Hero.Skill[num] = (byte)sSkillLevel;
		}
	}

	private static void GetNewWeights(ref TWeights Weights, int HeroClass)
	{
		for (int i = 0; i < 4; i++)
		{
			Weights.PW[i] = _PW[HeroClass, i];
			Weights.PW10[i] = _PW10[HeroClass, i];
		}
		for (int j = 0; j < 28; j++)
		{
			Weights.SW[j] = _SW[HeroClass, j];
			Weights.SR[j] = _SR[j];
		}
	}

	private static int GetSSkillLevel(string lvl)
	{
		for (int i = 0; i < 3; i++)
		{
			if (lvl == _aLevelSkill[i])
			{
				return ++i;
			}
		}
		return -1;
	}
}
