using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraTab;
using ProspectorRT.Properties;

namespace ProspectorRT;

public class MainForm : Form
{
	private delegate void SetLabelCange(string str);

	private delegate void ScanCompleteProc();

	private delegate void ScanErrorProc();

	private delegate int ObjectContent(DataRow row, int s);

	private delegate int ScanContent(int s);

	private struct MapRegion
	{
		public int Size;

		public int Side;

		public int Data;

		public int Teams;

		public int MapName;

		public int SaveName;

		public int SR;

		public int BlackMarket;

		public int Start;

		public int EventBox;

		public int ArtRes;

		public int Monstr;

		public int SeerHut;

		public int PassGuard;

		public int MapTimedEvent;

		public int TownsTimedEvent;

		public int BottleSign;

		public int Mine;

		public int Dwelling;

		public int Garrison;

		public int UnknownVarReg;

		public int UnknownFixedReg;

		public int Color;

		public int Town;

		public int Hero;

		public int HeroState;

		public int CurrentState;

		public int BitField;

		public int TwoWayMonolith;

		public int OneWayMonolith;

		public int Whirlpool;

		public int SubTerGate;

		public int SubTerGatePair;

		public int Univer;

		public int Bank;

		public int Motions;
	}

	private IContainer components;

	private StatusStrip statusStrip1;

	private ToolStripStatusLabel tsLabelStatusL;

	private ToolStripStatusLabel toolStripStatusLabel3;

	private ToolStripStatusLabel tsLabelLink;

	private ToolStripStatusLabel toolStripStatusLabel1;

	private BindingSource rMonstrBindingSource;

	private BindingSource rHeroesBindingSource;

	private BindingSource rAllArtsBindingSource;

	private BindingSource rBindingSource;

	private BindingSource rBankBindingSource;

	private BindingSource rEventBoxBindingSource;

	private BindingSource rScholarBindingSource;

	private BindingSource rResourceBindingSource;

	private BindingSource rChestBindingSource;

	private BindingSource rSpellBindingSource;

	private BindingSource rSkillBindingSource;

	private BindingSource rCampBindingSource;

	private BindingSource rMarketBindingSource;

	private BindingSource rSeerHutBindingSource;

	private BindingSource rPassGuardBindingSource;

	private BindingSource rGarrisonBindingSource;

	private BindingSource rPrisonBindingSource;

	private BindingSource rObjectBindingSource;

	private BindingSource rTownBindingSource;

	private OpenFileDialog openFileDialog1;

	private BarManager barManager1;

	private Bar bar2;

	private BarButtonItem tsButtonFile;

	private BarButtonItem tsButtonStart;

	private BarButtonItem tsButtonStop;

	private BarButtonItem tsButtonRefresh;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private XtraTabControl tabControl;

	private XtraTabPage tabMonster;

	private XtraTabPage tabArt;

	private XtraTabPage tabBank;

	private XtraTabPage tabBoxEvent;

	private XtraTabPage tabScholar;

	private XtraTabPage tabResource;

	private XtraTabPage tabChest;

	private XtraTabPage tabSpell;

	private XtraTabPage tabSkill;

	private XtraTabPage tabCamp;

	private XtraTabPage tabMarket;

	private XtraTabPage tabSeerHut;

	private XtraTabPage tabPassGuard;

	private XtraTabPage tabGarrison;

	private XtraTabPage tabPrison;

	private XtraTabPage tabObject;

	private XtraTabPage tabHero;

	private XtraTabPage tabTown;

	private XtraTabPage tabAllArts;

	private GridControl grdArt;

	private GridView vwArt;

	private GridColumn colX;

	private GridColumn colY;

	private GridColumn colZ;

	private GridColumn colLocality;

	private GridColumn colObject;

	private GridColumn colSlot;

	private GridColumn colName;

	private GridColumn colClass;

	private GridColumn colRelicC;

	private GridColumn colGold;

	private GridColumn colResource;

	private GridColumn colGuard;

	private GridControl grdMonster;

	private GridView vwMonster;

	private GridColumn colX1;

	private GridColumn colY1;

	private GridColumn colZ1;

	private GridColumn colLocality1;

	private GridColumn colName1;

	private GridColumn colNumber;

	private GridColumn colMood;

	private GridColumn colLevel;

	private GridColumn colArt;

	private GridColumn colGold1;

	private GridColumn colResource1;

	private GridControl grdBank;

	private GridView vwBank;

	private GridColumn colX2;

	private GridColumn colY2;

	private GridColumn colZ2;

	private GridColumn colLocality2;

	private GridColumn colName2;

	private GridColumn colGuard1;

	private GridColumn colMonster;

	private GridColumn colGold2;

	private GridColumn colResource2;

	private GridControl grdEventBox;

	private GridView vwEventBox;

	private GridColumn colX3;

	private GridColumn colY3;

	private GridColumn colZ3;

	private GridColumn colLocality3;

	private GridColumn colObject1;

	private GridColumn colGuard2;

	private GridColumn colExperience;

	private GridColumn colMana;

	private GridColumn colMorale;

	private GridColumn colLuck;

	private GridColumn colGold3;

	private GridColumn colResource3;

	private GridColumn colPrimarySkill;

	private GridColumn colSecondarySkill;

	private GridColumn colArtefact;

	private GridColumn colSpell;

	private GridColumn colMonster1;

	private GridControl grdScholar;

	private GridView vwScholar;

	private GridColumn colX4;

	private GridColumn colY4;

	private GridColumn colZ4;

	private GridColumn colLocality4;

	private GridColumn colSpell1;

	private GridColumn colPrimarySkill1;

	private GridColumn colSecondarySkill1;

	private GridControl grdResource;

	private GridView vwResource;

	private GridColumn colX5;

	private GridColumn colY5;

	private GridColumn colZ5;

	private GridColumn colLocality5;

	private GridColumn colObject2;

	private GridColumn colResource4;

	private GridColumn colGold4;

	private GridColumn colGuard3;

	private GridControl grdChest;

	private GridView vwChest;

	private GridColumn colX6;

	private GridColumn colY6;

	private GridColumn colZ6;

	private GridColumn colLocality6;

	private GridColumn colObject3;

	private GridColumn colArt1;

	private GridColumn colClass1;

	private GridColumn colRelicC6;

	private GridColumn colGold5;

	private GridControl grdSpell;

	private GridView vwSpell;

	private GridColumn colX7;

	private GridColumn colY7;

	private GridColumn colZ7;

	private GridColumn colLocality7;

	private GridColumn colObject4;

	private GridColumn colSpell2;

	private GridColumn colGuard4;

	private GridControl grdSkill;

	private GridView vwSkill;

	private GridColumn colX8;

	private GridColumn colY8;

	private GridColumn colZ8;

	private GridColumn colLocality8;

	private GridColumn colObject5;

	private GridColumn colSkill;

	private GridControl grdCamp;

	private GridView vwCamp;

	private GridControl grdTown;

	private GridView vwTown;

	private GridColumn colX10;

	private GridColumn colY10;

	private GridColumn colZ10;

	private GridColumn colName3;

	private GridColumn colType;

	private GridColumn colLevel1;

	private GridColumn colSpell3;

	private GridColumn colColor;

	private GridColumn colGarrison;

	private GridControl grdAllArts;

	private GridView vwAllArts;

	private GridColumn colX11;

	private GridColumn colY11;

	private GridColumn colZ11;

	private GridColumn colLocality10;

	private GridColumn colObject7;

	private GridColumn colArtefact1;

	private GridColumn colClass2;

	private GridColumn gridColumn1;

	private GridColumn colSlot1;

	private GridColumn colPlace;

	private GridColumn colColor1;

	private GridColumn colHero;

	private GridColumn colDoll;

	private GridColumn colMonster3;

	private GridColumn colMission;

	private GridColumn colGuard5;

	private GridControl grdMarket;

	private GridView vwMarket;

	private GridColumn colX12;

	private GridColumn colY12;

	private GridColumn colZ12;

	private GridColumn colLocality11;

	private GridColumn colObject8;

	private GridColumn colSlot2;

	private GridColumn colArt2;

	private GridColumn colClass3;

	private GridColumn gridColumn2;

	private GridColumn colCost;

	private GridControl grdSeerHut;

	private GridView vwSeerHut;

	private GridColumn colX13;

	private GridColumn colY13;

	private GridColumn colZ13;

	private GridColumn colLocality12;

	private GridColumn colMission1;

	private GridColumn colReward;

	private GridColumn colDeadline;

	private GridControl grdPassGuard;

	private GridView vwPassGuard;

	private GridColumn colX14;

	private GridColumn colY14;

	private GridColumn colZ14;

	private GridColumn colLocality13;

	private GridColumn colMission2;

	private GridColumn colDeadline1;

	private GridControl grdGarrison;

	private GridView vwGarrison;

	private GridColumn colX15;

	private GridColumn colY15;

	private GridColumn colZ15;

	private GridColumn colLocality14;

	private GridColumn colAntiMagic;

	private GridColumn colGuard6;

	private GridColumn colColor2;

	private GridColumn colCanTake;

	private GridControl grdPrison;

	private GridView vwPrison;

	private GridColumn colX16;

	private GridColumn colY16;

	private GridColumn colZ16;

	private GridColumn colLocality15;

	private GridColumn colHero1;

	private GridColumn colLevel2;

	private GridColumn colPrimarySkill2;

	private GridColumn colSecondarySkill2;

	private GridColumn colArt3;

	private GridColumn colSpell4;

	private GridColumn colMonster4;

	private GridColumn colMachine;

	private GridColumn colBook;

	private GridColumn colMP;

	private GridControl grdObject;

	private GridView vwObject;

	private GridColumn colX17;

	private GridColumn colY17;

	private GridColumn colZ17;

	private GridColumn colLocality16;

	private GridColumn colObject9;

	private GridColumn colPayment;

	private GridControl grdHero;

	private GridView vwHero;

	private GridColumn colID;

	private GridColumn colHero2;

	private GridColumn colPlace1;

	private GridColumn colColor3;

	private GridColumn colLevel3;

	private GridColumn colPrimarySkill3;

	private GridColumn colSecondarySkill3;

	private GridColumn colArt4;

	private GridColumn colSpell5;

	private GridColumn colMonster5;

	private GridColumn colMachine1;

	private GridColumn colBook1;

	private GridColumn colMP1;

	private GridColumn colX9;

	private GridColumn colY9;

	private GridColumn colZ9;

	private GridColumn colLocality9;

	private GridColumn colObject6;

	private GridColumn colMonster2;

	private GridColumn colLevel4;

	private GridColumn colNumber1;

	private DefaultLookAndFeel defaultLookAndFeel1;

	private ToolTipController toolTipController1;

	private BarDockControl barDockControl3;

	private BarDockControl barDockControl4;

	private BarDockControl barDockControl2;

	private BarDockControl barDockControl1;

	private BarManager barManager2;

	private BarAndDockingController barAndDockingController2;

	private GridColumn colIncrease;

	private GridColumn colBuilt;

	private GridColumn colExperience2;

	private GridColumn colExperience1;

	private GridColumn colHP1;

	private GridColumn colHP2;

	private GridColumn colHP3;

	private GridColumn colHP4;

	private GridColumn colHP5;

	private GridColumn colHP6;

	private GridColumn colHP7;

	private GridColumn colHP8;

	private GridColumn colHP9;

	private GridColumn colHP11;

	private GridColumn colHP10;

	private XtraTabPage tabTopology;

	private GridControl grdTopology;

	private BindingSource rTopologyBindingSource;

	private GridView vwTopology;

	private GridColumn colX18;

	private GridColumn colY18;

	private GridColumn colZ18;

	private GridColumn colLocality17;

	private GridColumn colObject10;

	private GridColumn colType1;

	private GridColumn colColor4;

	private XtraTabPage tabAllSpell;

	private GridControl grdAllSpell;

	private BindingSource rAllSpellBindingSource;

	private GridView vwAllSpell;

	private GridColumn colX19;

	private GridColumn colY19;

	private GridColumn colZ19;

	private GridColumn colObject11;

	private GridColumn colSpell6;

	private GridColumn colSlot3;

	private GridColumn colColor5;

	private GridColumn colName4;

	private GridColumn colBuilt1;

	private GridColumn colGarrison1;

	private GridColumn colID1;

	private GridColumn colHero3;

	private GridColumn colPlace2;

	private GridColumn colMission3;

	private GridColumn colGuard7;

	private GridColumn colLevel9;

	private GridColumn colLibrary;

	private GridColumn colID3;

	private XtraTabPage tabAllTimer;

	private GridControl grdAllTimer;

	private BindingSource rAllTimerBindingSource;

	private GridView vwAllTimer;

	private GridColumn colObject12;

	private GridColumn colDay;

	private GridColumn colRepeat;

	private GridColumn colTown;

	private GridColumn colType2;

	private GridColumn colPlace3;

	private GridColumn colResource5;

	private GridColumn colBuilding;

	private GridColumn colMonster6;

	private GridColumn colGold13;

	private GridColumn colApply;

	private GridColumn colColor13;

	private GridColumn colAvailable;

	private GridColumn colTimer;

	private XtraTabPage tabAllSkill;

	private GridControl grdAllSkill;

	private BindingSource rAllSkillBindingSource;

	private GridView vwAllSkill;

	private GridColumn colX20;

	private GridColumn colY20;

	private GridColumn colZ20;

	private GridColumn colLocality18;

	private GridColumn colObject13;

	private GridColumn colSkill1;

	private GridColumn colMission4;

	private GridColumn colGuard8;

	private GridColumn colLevel13;

	private BarButtonItem tsButtonOT;

	private BarSubItem tsMenuService;

	private BarButtonItem tsButtonExcel;

	private SaveFileDialog saveFileDialog1;

	private GridColumn colHire;

	private GridColumn colSlot13;

	private GridColumn colApply2;

	private GridColumn colRepeat1;

	private XtraTabPage tabExperience;

	private GridControl grdExperience;

	private GridView vwExperience;

	private BindingSource rAllExperienceBindingSource;

	private GridColumn colX21;

	private GridColumn colY21;

	private GridColumn colZ21;

	private GridColumn colLocality19;

	private GridColumn colObject14;

	private GridColumn colMonster7;

	private GridColumn colGuard9;

	private GridColumn colMission5;

	private GridColumn colID2;

	private GridColumn colHero4;

	private GridColumn colExperience3;

	private GridColumn colColor21;

	private GridColumn colHP21;

	private GridColumn colXP;

	private GridColumn colXP1;

	private GridColumn colXP2;

	private GridColumn colXP3;

	private GridColumn colArt21;

	private GridColumn colResource21;

	private GridColumn colTown21;

	private GridColumn colSpell21;

	private PopupMenu ppmFile;

	private BarButtonItem bButtonOpen;

	private BarButtonItem bButtonClear;

	private ToolStripStatusLabel tsLabelGrail;

	private BarSubItem barSubItem1;

	private BarSubItem bsiVerOracle;

	private BarCheckItem chi32EN;

	private BarCheckItem chi40EN;

	private BarCheckItem chi40RU;

	private BarEditItem biDepth;

	private RepositoryItemSpinEdit repositoryItemSpinEdit1;

	private BarButtonItem tsButtonSPT;

	public DataSet2 dsResult;

	private BarCheckItem chiFree;

	private GridColumn colIdeology;

	private BarSubItem barSubItem3;

	private BarCheckItem chiStart;

	private BarEditItem biRecent;

	private RepositoryItemSpinEdit repositoryItemSpinEdit2;

	private BarSubItem mnuButtonHelp;

	private BarButtonItem tsButtonHelp;

	private BarButtonItem tsAbout;

	private BarButtonItem tsButtonObjectName;

	private GridColumn colHeroClass;

	private GridColumn colPair;

	private Timer timer1;

	private Timer timer2;

	private string sBegin = "Начали";

	private string sStop = "Стоп";

	private string sStart = "Старт";

	private string sReady = "Готово";

	private string sChoose = "Выберите файл";

	private string sFile = "Файл";

	private string sError = "Ошибка";

	private string errorExistFile = "не существует.\r\nУдалить из списка?";

	private string titleExistFile = "ошибка открытия файла";

	private string errorMapVer = "Недопустимая версия карты";

	private string titleMapVer = "Ошибка открытия";

	private string errorScaner = " К сожалению, ошибка обработки сейва. \r\n Пожалуйста, вышлите его и карту на e-mail \r\n \r\n               prospectorrt@yandex.ru \r\n \r\n Будет возможность в ответном сообщении \r\n получить исправленный файл программы.";

	private string errorObjectName = "с названиями объектов не существует.\r\nПроверьте пункт меню \"Сервис->Настройка->Названия объектов\"\r\nИспользованы значения по умолчанию.";

	private string errorTemplateTable = "не является шаблоном для этой Таблицы.";

	private string messageExcel = "Экспорт  в  Excel  завершен.\r\nОткрыть  файл";

	private int HeroCount = 156;

	private bool ot3;

	private bool spt;

	private int MaxLevel = 10;

	private byte Version = 2;

	private MapRegion map = default(MapRegion);

	private byte[] aObjectID;

	private int[] aHeroID;

	private int[] aMonstrID;

	private int Chrn;

	private int MapSize;

	private int MapSide;

	private int ObjectNumber;

	private string grail;

	private string days;

	private GridHitInfo hitInfo;

	private GridHitInfo dblHitInfo;

	private Thread myThread;

	private Hashtable hBT = new Hashtable();

	private Hashtable hDS = new Hashtable();

	private Hashtable hFR = new Hashtable();

	private ArrayList aGrid = new ArrayList();

	private BindingSource bsInit = new BindingSource();

	private IList<string> SheetsName;

	private bool fLoadForm = true;

	private bool fRowFlag;

	private bool fStopFlag;

	public string[] aArtClass;

	public string[] aResource = new string[6] { "W", "M", "O", "S", "C", "G" };

	private string[] aPrSkill = new string[4] { "A", "D", "P", "K" };

	private string[] aMine;

	private string[] aLocality;

	private string[] aLevelSkill;

	private string[] aFullLvlSkill;

	private string[] aColor;

	private string[] aReply;

	private string[] aTown;

	private string[] aDoll;

	private string[] aPlace;

	private string[] aTent;

	private string[] aStatus;

	private string[] аQuest;

	private string[] aReward;

	private string[] aHeroClass;

	private string[] aIdeology;

	private byte[] decmp;

	private byte[] aSPTdecmp;

	private int[,] aTavernGuest = new int[8, 2];

	private int[] aExistColor = new int[8];

	private int[] aFoundColor = new int[8];

	private int human;

	private int[] aAlliance = new int[0];

	public DataTable TblMonsters;

	public DataTable TblArt;

	public DataTable TblSpell;

	public DataTable TblSecondarySkill;

	public DataTable TblBuilding;

	public DataTable TblObject;

	private DataTable TblIdeology;

	private DataTable TblDwelling;

	private DataTable TblMonstersComplete = new DataTable();

	private DataTable TblArtComplete = new DataTable();

	private DataTable TblSpellComplete = new DataTable();

	private DataTable TblSecondarySkillComplete = new DataTable();

	private DataTable TblObjectComplete = new DataTable();

	private DataTable TblDwellingComplete = new DataTable();

	private DataTable TblMonstersSoDEn = new DataTable();

	private DataTable TblArtSoDEn = new DataTable();

	private DataTable TblSpellSoDEn = new DataTable();

	private DataTable TblSecondarySkillSoDEn = new DataTable();

	private DataTable TblObjectSoDEn = new DataTable();

	private DataTable TblDwellingSoDEn = new DataTable();

	private DataTable TblMonstersSoDRu = new DataTable();

	private DataTable TblArtSoDRu = new DataTable();

	private DataTable TblSpellSoDRu = new DataTable();

	private DataTable TblSecondarySkillSoDRu = new DataTable();

	private DataTable TblObjectSoDRu = new DataTable();

	private DataTable TblDwellingSoDRu = new DataTable();

	private DataTable TblBanks = new DataTable();

	private DataTable TblUniver = new DataTable();

	private DataTable TblMarket = new DataTable();

	private DataTable TblGarrison = new DataTable();

	private DataTable TblEventBox = new DataTable();

	private DataTable TblMonstr = new DataTable();

	private DataTable TblArtRes = new DataTable();

	private DataTable TblSeerHut = new DataTable();

	private DataTable TblPassGuard = new DataTable();

	private byte[,] _PW = new byte[18, 4];

	private byte[,] _PW10 = new byte[18, 4];

	private byte[,] _SW = new byte[18, 28];

	private byte[] _SR = new byte[28];

	private myToolTip ntoolt;

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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c55: Expected O, but got Unknown
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Expected O, but got Unknown
		//IL_1009: Unknown result type (might be due to invalid IL or missing references)
		//IL_1013: Expected O, but got Unknown
		//IL_10e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ea: Expected O, but got Unknown
		//IL_1180: Unknown result type (might be due to invalid IL or missing references)
		//IL_118a: Expected O, but got Unknown
		//IL_12dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e6: Expected O, but got Unknown
		//IL_12ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Expected O, but got Unknown
		//IL_12fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1308: Expected O, but got Unknown
		//IL_1870: Unknown result type (might be due to invalid IL or missing references)
		//IL_187a: Expected O, but got Unknown
		//IL_18c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cf: Expected O, but got Unknown
		//IL_1972: Unknown result type (might be due to invalid IL or missing references)
		//IL_197c: Expected O, but got Unknown
		//IL_1a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5d: Expected O, but got Unknown
		//IL_21c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cf: Expected O, but got Unknown
		//IL_2e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e11: Expected O, but got Unknown
		//IL_ef1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef25: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
		statusStrip1 = new StatusStrip();
		tsLabelStatusL = new ToolStripStatusLabel();
		tsLabelGrail = new ToolStripStatusLabel();
		toolStripStatusLabel3 = new ToolStripStatusLabel();
		tsLabelLink = new ToolStripStatusLabel();
		toolStripStatusLabel1 = new ToolStripStatusLabel();
		dsResult = new DataSet2();
		rMonstrBindingSource = new BindingSource(components);
		rHeroesBindingSource = new BindingSource(components);
		rAllArtsBindingSource = new BindingSource(components);
		rBindingSource = new BindingSource(components);
		rBankBindingSource = new BindingSource(components);
		rEventBoxBindingSource = new BindingSource(components);
		rScholarBindingSource = new BindingSource(components);
		rResourceBindingSource = new BindingSource(components);
		rChestBindingSource = new BindingSource(components);
		rSpellBindingSource = new BindingSource(components);
		rSkillBindingSource = new BindingSource(components);
		rCampBindingSource = new BindingSource(components);
		rMarketBindingSource = new BindingSource(components);
		rSeerHutBindingSource = new BindingSource(components);
		rPassGuardBindingSource = new BindingSource(components);
		rGarrisonBindingSource = new BindingSource(components);
		rPrisonBindingSource = new BindingSource(components);
		rObjectBindingSource = new BindingSource(components);
		rTownBindingSource = new BindingSource(components);
		openFileDialog1 = new OpenFileDialog();
		barManager1 = new BarManager(components);
		bar2 = new Bar();
		tsButtonFile = new BarButtonItem();
		ppmFile = new PopupMenu(components);
		bButtonOpen = new BarButtonItem();
		bButtonClear = new BarButtonItem();
		tsButtonStart = new BarButtonItem();
		tsButtonStop = new BarButtonItem();
		tsButtonRefresh = new BarButtonItem();
		tsMenuService = new BarSubItem();
		tsButtonExcel = new BarButtonItem();
		barSubItem1 = new BarSubItem();
		chiFree = new BarCheckItem();
		bsiVerOracle = new BarSubItem();
		chi32EN = new BarCheckItem();
		chi40EN = new BarCheckItem();
		chi40RU = new BarCheckItem();
		biDepth = new BarEditItem();
		repositoryItemSpinEdit1 = new RepositoryItemSpinEdit();
		barSubItem3 = new BarSubItem();
		chiStart = new BarCheckItem();
		biRecent = new BarEditItem();
		repositoryItemSpinEdit2 = new RepositoryItemSpinEdit();
		tsButtonObjectName = new BarButtonItem();
		mnuButtonHelp = new BarSubItem();
		tsButtonHelp = new BarButtonItem();
		tsAbout = new BarButtonItem();
		tsButtonOT = new BarButtonItem();
		tsButtonSPT = new BarButtonItem();
		barDockControlTop = new BarDockControl();
		barDockControlBottom = new BarDockControl();
		barDockControlLeft = new BarDockControl();
		barDockControlRight = new BarDockControl();
		tabControl = new XtraTabControl();
		tabArt = new XtraTabPage();
		grdArt = new GridControl();
		vwArt = new GridView();
		colX = new GridColumn();
		colY = new GridColumn();
		colZ = new GridColumn();
		colLocality = new GridColumn();
		colObject = new GridColumn();
		colSlot = new GridColumn();
		colName = new GridColumn();
		colClass = new GridColumn();
		colRelicC = new GridColumn();
		colGold = new GridColumn();
		colResource = new GridColumn();
		colGuard = new GridColumn();
		colHP1 = new GridColumn();
		toolTipController1 = new ToolTipController(components);
		tabMonster = new XtraTabPage();
		grdMonster = new GridControl();
		vwMonster = new GridView();
		colX1 = new GridColumn();
		colY1 = new GridColumn();
		colZ1 = new GridColumn();
		colLocality1 = new GridColumn();
		colName1 = new GridColumn();
		colNumber = new GridColumn();
		colMood = new GridColumn();
		colLevel = new GridColumn();
		colArt = new GridColumn();
		colGold1 = new GridColumn();
		colResource1 = new GridColumn();
		colIncrease = new GridColumn();
		colHP2 = new GridColumn();
		tabBank = new XtraTabPage();
		grdBank = new GridControl();
		vwBank = new GridView();
		colX2 = new GridColumn();
		colY2 = new GridColumn();
		colZ2 = new GridColumn();
		colLocality2 = new GridColumn();
		colName2 = new GridColumn();
		colGuard1 = new GridColumn();
		colMonster = new GridColumn();
		colGold2 = new GridColumn();
		colResource2 = new GridColumn();
		colHP3 = new GridColumn();
		tabBoxEvent = new XtraTabPage();
		grdEventBox = new GridControl();
		vwEventBox = new GridView();
		colX3 = new GridColumn();
		colY3 = new GridColumn();
		colZ3 = new GridColumn();
		colLocality3 = new GridColumn();
		colObject1 = new GridColumn();
		colGuard2 = new GridColumn();
		colExperience = new GridColumn();
		colMana = new GridColumn();
		colMorale = new GridColumn();
		colLuck = new GridColumn();
		colGold3 = new GridColumn();
		colResource3 = new GridColumn();
		colPrimarySkill = new GridColumn();
		colSecondarySkill = new GridColumn();
		colArtefact = new GridColumn();
		colSpell = new GridColumn();
		colMonster1 = new GridColumn();
		colHP4 = new GridColumn();
		colApply2 = new GridColumn();
		colRepeat1 = new GridColumn();
		tabScholar = new XtraTabPage();
		grdScholar = new GridControl();
		vwScholar = new GridView();
		colX4 = new GridColumn();
		colY4 = new GridColumn();
		colZ4 = new GridColumn();
		colLocality4 = new GridColumn();
		colSpell1 = new GridColumn();
		colPrimarySkill1 = new GridColumn();
		colSecondarySkill1 = new GridColumn();
		tabResource = new XtraTabPage();
		grdResource = new GridControl();
		vwResource = new GridView();
		colX5 = new GridColumn();
		colY5 = new GridColumn();
		colZ5 = new GridColumn();
		colLocality5 = new GridColumn();
		colObject2 = new GridColumn();
		colResource4 = new GridColumn();
		colGold4 = new GridColumn();
		colGuard3 = new GridColumn();
		colHP5 = new GridColumn();
		tabChest = new XtraTabPage();
		grdChest = new GridControl();
		vwChest = new GridView();
		colX6 = new GridColumn();
		colY6 = new GridColumn();
		colZ6 = new GridColumn();
		colLocality6 = new GridColumn();
		colObject3 = new GridColumn();
		colArt1 = new GridColumn();
		colClass1 = new GridColumn();
		colRelicC6 = new GridColumn();
		colGold5 = new GridColumn();
		colHP6 = new GridColumn();
		tabSpell = new XtraTabPage();
		grdSpell = new GridControl();
		vwSpell = new GridView();
		colX7 = new GridColumn();
		colY7 = new GridColumn();
		colZ7 = new GridColumn();
		colLocality7 = new GridColumn();
		colObject4 = new GridColumn();
		colSpell2 = new GridColumn();
		colGuard4 = new GridColumn();
		colHP7 = new GridColumn();
		colLevel9 = new GridColumn();
		tabSkill = new XtraTabPage();
		grdSkill = new GridControl();
		vwSkill = new GridView();
		colX8 = new GridColumn();
		colY8 = new GridColumn();
		colZ8 = new GridColumn();
		colLocality8 = new GridColumn();
		colObject5 = new GridColumn();
		colSkill = new GridColumn();
		tabCamp = new XtraTabPage();
		grdCamp = new GridControl();
		vwCamp = new GridView();
		colX9 = new GridColumn();
		colY9 = new GridColumn();
		colZ9 = new GridColumn();
		colLocality9 = new GridColumn();
		colObject6 = new GridColumn();
		colMonster2 = new GridColumn();
		colLevel4 = new GridColumn();
		colNumber1 = new GridColumn();
		tabMarket = new XtraTabPage();
		grdMarket = new GridControl();
		vwMarket = new GridView();
		colX12 = new GridColumn();
		colY12 = new GridColumn();
		colZ12 = new GridColumn();
		colLocality11 = new GridColumn();
		colObject8 = new GridColumn();
		colSlot2 = new GridColumn();
		colArt2 = new GridColumn();
		colClass3 = new GridColumn();
		gridColumn2 = new GridColumn();
		colCost = new GridColumn();
		tabSeerHut = new XtraTabPage();
		grdSeerHut = new GridControl();
		vwSeerHut = new GridView();
		colX13 = new GridColumn();
		colY13 = new GridColumn();
		colZ13 = new GridColumn();
		colLocality12 = new GridColumn();
		colMission1 = new GridColumn();
		colReward = new GridColumn();
		colDeadline = new GridColumn();
		tabPassGuard = new XtraTabPage();
		grdPassGuard = new GridControl();
		vwPassGuard = new GridView();
		colX14 = new GridColumn();
		colY14 = new GridColumn();
		colZ14 = new GridColumn();
		colLocality13 = new GridColumn();
		colMission2 = new GridColumn();
		colDeadline1 = new GridColumn();
		tabGarrison = new XtraTabPage();
		grdGarrison = new GridControl();
		vwGarrison = new GridView();
		colX15 = new GridColumn();
		colY15 = new GridColumn();
		colZ15 = new GridColumn();
		colLocality14 = new GridColumn();
		colAntiMagic = new GridColumn();
		colGuard6 = new GridColumn();
		colColor2 = new GridColumn();
		colCanTake = new GridColumn();
		colHP8 = new GridColumn();
		tabPrison = new XtraTabPage();
		grdPrison = new GridControl();
		vwPrison = new GridView();
		colX16 = new GridColumn();
		colY16 = new GridColumn();
		colZ16 = new GridColumn();
		colLocality15 = new GridColumn();
		colHero1 = new GridColumn();
		colLevel2 = new GridColumn();
		colPrimarySkill2 = new GridColumn();
		colSecondarySkill2 = new GridColumn();
		colArt3 = new GridColumn();
		colSpell4 = new GridColumn();
		colMonster4 = new GridColumn();
		colMachine = new GridColumn();
		colBook = new GridColumn();
		colMP = new GridColumn();
		colExperience2 = new GridColumn();
		colHP11 = new GridColumn();
		tabObject = new XtraTabPage();
		grdObject = new GridControl();
		vwObject = new GridView();
		colX17 = new GridColumn();
		colY17 = new GridColumn();
		colZ17 = new GridColumn();
		colLocality16 = new GridColumn();
		colObject9 = new GridColumn();
		colPayment = new GridColumn();
		tabTopology = new XtraTabPage();
		grdTopology = new GridControl();
		rTopologyBindingSource = new BindingSource(components);
		vwTopology = new GridView();
		colX18 = new GridColumn();
		colY18 = new GridColumn();
		colZ18 = new GridColumn();
		colLocality17 = new GridColumn();
		colObject10 = new GridColumn();
		colType1 = new GridColumn();
		colColor4 = new GridColumn();
		colPair = new GridColumn();
		tabAllTimer = new XtraTabPage();
		grdAllTimer = new GridControl();
		rAllTimerBindingSource = new BindingSource(components);
		vwAllTimer = new GridView();
		colObject12 = new GridColumn();
		colDay = new GridColumn();
		colRepeat = new GridColumn();
		colTown = new GridColumn();
		colType2 = new GridColumn();
		colPlace3 = new GridColumn();
		colResource5 = new GridColumn();
		colBuilding = new GridColumn();
		colMonster6 = new GridColumn();
		colGold13 = new GridColumn();
		colApply = new GridColumn();
		colColor13 = new GridColumn();
		tabHero = new XtraTabPage();
		grdHero = new GridControl();
		vwHero = new GridView();
		colID = new GridColumn();
		colHero2 = new GridColumn();
		colPlace1 = new GridColumn();
		colColor3 = new GridColumn();
		colLevel3 = new GridColumn();
		colPrimarySkill3 = new GridColumn();
		colSecondarySkill3 = new GridColumn();
		colArt4 = new GridColumn();
		colSpell5 = new GridColumn();
		colMonster5 = new GridColumn();
		colMachine1 = new GridColumn();
		colBook1 = new GridColumn();
		colMP1 = new GridColumn();
		colExperience1 = new GridColumn();
		colHP10 = new GridColumn();
		colHire = new GridColumn();
		colIdeology = new GridColumn();
		colHeroClass = new GridColumn();
		tabTown = new XtraTabPage();
		grdTown = new GridControl();
		vwTown = new GridView();
		colX10 = new GridColumn();
		colY10 = new GridColumn();
		colZ10 = new GridColumn();
		colName3 = new GridColumn();
		colType = new GridColumn();
		colLevel1 = new GridColumn();
		colSpell3 = new GridColumn();
		colColor = new GridColumn();
		colGarrison = new GridColumn();
		colBuilt = new GridColumn();
		colHP9 = new GridColumn();
		colLibrary = new GridColumn();
		colID3 = new GridColumn();
		colAvailable = new GridColumn();
		colTimer = new GridColumn();
		tabAllArts = new XtraTabPage();
		grdAllArts = new GridControl();
		vwAllArts = new GridView();
		colX11 = new GridColumn();
		colY11 = new GridColumn();
		colZ11 = new GridColumn();
		colLocality10 = new GridColumn();
		colObject7 = new GridColumn();
		colArtefact1 = new GridColumn();
		colClass2 = new GridColumn();
		gridColumn1 = new GridColumn();
		colSlot1 = new GridColumn();
		colPlace = new GridColumn();
		colColor1 = new GridColumn();
		colHero = new GridColumn();
		colDoll = new GridColumn();
		colMonster3 = new GridColumn();
		colMission = new GridColumn();
		colGuard5 = new GridColumn();
		tabAllSpell = new XtraTabPage();
		grdAllSpell = new GridControl();
		rAllSpellBindingSource = new BindingSource(components);
		vwAllSpell = new GridView();
		colX19 = new GridColumn();
		colY19 = new GridColumn();
		colZ19 = new GridColumn();
		colObject11 = new GridColumn();
		colSpell6 = new GridColumn();
		colSlot3 = new GridColumn();
		colColor5 = new GridColumn();
		colName4 = new GridColumn();
		colBuilt1 = new GridColumn();
		colGarrison1 = new GridColumn();
		colID1 = new GridColumn();
		colHero3 = new GridColumn();
		colPlace2 = new GridColumn();
		colMission3 = new GridColumn();
		colGuard7 = new GridColumn();
		tabAllSkill = new XtraTabPage();
		grdAllSkill = new GridControl();
		rAllSkillBindingSource = new BindingSource(components);
		vwAllSkill = new GridView();
		colX20 = new GridColumn();
		colY20 = new GridColumn();
		colZ20 = new GridColumn();
		colLocality18 = new GridColumn();
		colObject13 = new GridColumn();
		colSkill1 = new GridColumn();
		colMission4 = new GridColumn();
		colGuard8 = new GridColumn();
		colLevel13 = new GridColumn();
		colSlot13 = new GridColumn();
		tabExperience = new XtraTabPage();
		grdExperience = new GridControl();
		rAllExperienceBindingSource = new BindingSource(components);
		vwExperience = new GridView();
		colX21 = new GridColumn();
		colY21 = new GridColumn();
		colZ21 = new GridColumn();
		colLocality19 = new GridColumn();
		colObject14 = new GridColumn();
		colMonster7 = new GridColumn();
		colGuard9 = new GridColumn();
		colMission5 = new GridColumn();
		colID2 = new GridColumn();
		colHero4 = new GridColumn();
		colExperience3 = new GridColumn();
		colColor21 = new GridColumn();
		colHP21 = new GridColumn();
		colXP = new GridColumn();
		colXP1 = new GridColumn();
		colXP2 = new GridColumn();
		colXP3 = new GridColumn();
		colArt21 = new GridColumn();
		colResource21 = new GridColumn();
		colTown21 = new GridColumn();
		colSpell21 = new GridColumn();
		defaultLookAndFeel1 = new DefaultLookAndFeel(components);
		barManager2 = new BarManager(components);
		barAndDockingController2 = new BarAndDockingController(components);
		barDockControl1 = new BarDockControl();
		barDockControl2 = new BarDockControl();
		barDockControl3 = new BarDockControl();
		barDockControl4 = new BarDockControl();
		saveFileDialog1 = new SaveFileDialog();
		timer1 = new Timer(components);
		timer2 = new Timer(components);
		((Control)statusStrip1).SuspendLayout();
		((ISupportInitialize)dsResult).BeginInit();
		((ISupportInitialize)rMonstrBindingSource).BeginInit();
		((ISupportInitialize)rHeroesBindingSource).BeginInit();
		((ISupportInitialize)rAllArtsBindingSource).BeginInit();
		((ISupportInitialize)rBindingSource).BeginInit();
		((ISupportInitialize)rBankBindingSource).BeginInit();
		((ISupportInitialize)rEventBoxBindingSource).BeginInit();
		((ISupportInitialize)rScholarBindingSource).BeginInit();
		((ISupportInitialize)rResourceBindingSource).BeginInit();
		((ISupportInitialize)rChestBindingSource).BeginInit();
		((ISupportInitialize)rSpellBindingSource).BeginInit();
		((ISupportInitialize)rSkillBindingSource).BeginInit();
		((ISupportInitialize)rCampBindingSource).BeginInit();
		((ISupportInitialize)rMarketBindingSource).BeginInit();
		((ISupportInitialize)rSeerHutBindingSource).BeginInit();
		((ISupportInitialize)rPassGuardBindingSource).BeginInit();
		((ISupportInitialize)rGarrisonBindingSource).BeginInit();
		((ISupportInitialize)rPrisonBindingSource).BeginInit();
		((ISupportInitialize)rObjectBindingSource).BeginInit();
		((ISupportInitialize)rTownBindingSource).BeginInit();
		((ISupportInitialize)barManager1).BeginInit();
		((ISupportInitialize)ppmFile).BeginInit();
		((ISupportInitialize)repositoryItemSpinEdit1).BeginInit();
		((ISupportInitialize)repositoryItemSpinEdit2).BeginInit();
		((ISupportInitialize)tabControl).BeginInit();
		((Control)tabControl).SuspendLayout();
		((Control)tabArt).SuspendLayout();
		((ISupportInitialize)grdArt).BeginInit();
		((ISupportInitialize)vwArt).BeginInit();
		((Control)tabMonster).SuspendLayout();
		((ISupportInitialize)grdMonster).BeginInit();
		((ISupportInitialize)vwMonster).BeginInit();
		((Control)tabBank).SuspendLayout();
		((ISupportInitialize)grdBank).BeginInit();
		((ISupportInitialize)vwBank).BeginInit();
		((Control)tabBoxEvent).SuspendLayout();
		((ISupportInitialize)grdEventBox).BeginInit();
		((ISupportInitialize)vwEventBox).BeginInit();
		((Control)tabScholar).SuspendLayout();
		((ISupportInitialize)grdScholar).BeginInit();
		((ISupportInitialize)vwScholar).BeginInit();
		((Control)tabResource).SuspendLayout();
		((ISupportInitialize)grdResource).BeginInit();
		((ISupportInitialize)vwResource).BeginInit();
		((Control)tabChest).SuspendLayout();
		((ISupportInitialize)grdChest).BeginInit();
		((ISupportInitialize)vwChest).BeginInit();
		((Control)tabSpell).SuspendLayout();
		((ISupportInitialize)grdSpell).BeginInit();
		((ISupportInitialize)vwSpell).BeginInit();
		((Control)tabSkill).SuspendLayout();
		((ISupportInitialize)grdSkill).BeginInit();
		((ISupportInitialize)vwSkill).BeginInit();
		((Control)tabCamp).SuspendLayout();
		((ISupportInitialize)grdCamp).BeginInit();
		((ISupportInitialize)vwCamp).BeginInit();
		((Control)tabMarket).SuspendLayout();
		((ISupportInitialize)grdMarket).BeginInit();
		((ISupportInitialize)vwMarket).BeginInit();
		((Control)tabSeerHut).SuspendLayout();
		((ISupportInitialize)grdSeerHut).BeginInit();
		((ISupportInitialize)vwSeerHut).BeginInit();
		((Control)tabPassGuard).SuspendLayout();
		((ISupportInitialize)grdPassGuard).BeginInit();
		((ISupportInitialize)vwPassGuard).BeginInit();
		((Control)tabGarrison).SuspendLayout();
		((ISupportInitialize)grdGarrison).BeginInit();
		((ISupportInitialize)vwGarrison).BeginInit();
		((Control)tabPrison).SuspendLayout();
		((ISupportInitialize)grdPrison).BeginInit();
		((ISupportInitialize)vwPrison).BeginInit();
		((Control)tabObject).SuspendLayout();
		((ISupportInitialize)grdObject).BeginInit();
		((ISupportInitialize)vwObject).BeginInit();
		((Control)tabTopology).SuspendLayout();
		((ISupportInitialize)grdTopology).BeginInit();
		((ISupportInitialize)rTopologyBindingSource).BeginInit();
		((ISupportInitialize)vwTopology).BeginInit();
		((Control)tabAllTimer).SuspendLayout();
		((ISupportInitialize)grdAllTimer).BeginInit();
		((ISupportInitialize)rAllTimerBindingSource).BeginInit();
		((ISupportInitialize)vwAllTimer).BeginInit();
		((Control)tabHero).SuspendLayout();
		((ISupportInitialize)grdHero).BeginInit();
		((ISupportInitialize)vwHero).BeginInit();
		((Control)tabTown).SuspendLayout();
		((ISupportInitialize)grdTown).BeginInit();
		((ISupportInitialize)vwTown).BeginInit();
		((Control)tabAllArts).SuspendLayout();
		((ISupportInitialize)grdAllArts).BeginInit();
		((ISupportInitialize)vwAllArts).BeginInit();
		((Control)tabAllSpell).SuspendLayout();
		((ISupportInitialize)grdAllSpell).BeginInit();
		((ISupportInitialize)rAllSpellBindingSource).BeginInit();
		((ISupportInitialize)vwAllSpell).BeginInit();
		((Control)tabAllSkill).SuspendLayout();
		((ISupportInitialize)grdAllSkill).BeginInit();
		((ISupportInitialize)rAllSkillBindingSource).BeginInit();
		((ISupportInitialize)vwAllSkill).BeginInit();
		((Control)tabExperience).SuspendLayout();
		((ISupportInitialize)grdExperience).BeginInit();
		((ISupportInitialize)rAllExperienceBindingSource).BeginInit();
		((ISupportInitialize)vwExperience).BeginInit();
		((ISupportInitialize)barManager2).BeginInit();
		((ISupportInitialize)barAndDockingController2).BeginInit();
		((Control)this).SuspendLayout();
		((ToolStrip)statusStrip1).BackColor = Color.FromArgb(235, 236, 239);
		((ToolStrip)statusStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[5]
		{
			(ToolStripItem)tsLabelStatusL,
			(ToolStripItem)tsLabelGrail,
			(ToolStripItem)toolStripStatusLabel3,
			(ToolStripItem)tsLabelLink,
			(ToolStripItem)toolStripStatusLabel1
		});
		((Control)statusStrip1).Location = new Point(0, 515);
		((Control)statusStrip1).Name = "statusStrip1";
		((Control)statusStrip1).Size = new Size(974, 22);
		((Control)statusStrip1).TabIndex = 5;
		((Control)statusStrip1).Text = "statusStrip1";
		((ToolStripItem)tsLabelStatusL).Font = new Font("Courier New", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((ToolStripItem)tsLabelStatusL).Name = "tsLabelStatusL";
		((ToolStripItem)tsLabelStatusL).Size = new Size(84, 17);
		((ToolStripItem)tsLabelStatusL).Text = "Choose file";
		((ToolStripItem)tsLabelGrail).Font = new Font("Courier New", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((ToolStripItem)tsLabelGrail).Name = "tsLabelGrail";
		((ToolStripItem)tsLabelGrail).Size = new Size(594, 17);
		tsLabelGrail.Spring = true;
		((ToolStripItem)toolStripStatusLabel3).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripItem)toolStripStatusLabel3).Name = "toolStripStatusLabel3";
		((ToolStripItem)toolStripStatusLabel3).Size = new Size(77, 17);
		((ToolStripItem)toolStripStatusLabel3).Text = "Designed  for";
		((ToolStripItem)toolStripStatusLabel3).TextAlign = (ContentAlignment)64;
		((ToolStripItem)toolStripStatusLabel3).TextImageRelation = (TextImageRelation)8;
		((ToolStripItem)tsLabelLink).Font = new Font("Courier New", 9f, (FontStyle)1, (GraphicsUnit)3, (byte)204);
		((ToolStripLabel)tsLabelLink).IsLink = true;
		((ToolStripLabel)tsLabelLink).LinkBehavior = (LinkBehavior)2;
		((ToolStripLabel)tsLabelLink).LinkColor = Color.FromArgb(72, 118, 186);
		((ToolStripItem)tsLabelLink).Name = "tsLabelLink";
		((ToolStripItem)tsLabelLink).Size = new Size(120, 17);
		((ToolStripItem)tsLabelLink).Tag = "http://HeroesPortal.net";
		((ToolStripItem)tsLabelLink).Text = "HeroesPortal.net";
		((ToolStripItem)tsLabelLink).TextAlign = (ContentAlignment)1024;
		((ToolStripItem)tsLabelLink).TextImageRelation = (TextImageRelation)8;
		((ToolStripItem)tsLabelLink).Click += tsLabelLink_Click;
		toolStripStatusLabel1.BorderStyle = (Border3DStyle)4;
		((ToolStripItem)toolStripStatusLabel1).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripItem)toolStripStatusLabel1).Font = new Font("Courier New", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		((ToolStripItem)toolStripStatusLabel1).Name = "toolStripStatusLabel1";
		((ToolStripItem)toolStripStatusLabel1).Size = new Size(84, 17);
		((ToolStripItem)toolStripStatusLabel1).Text = "   ver. 2.3";
		((ToolStripItem)toolStripStatusLabel1).TextAlign = (ContentAlignment)64;
		((ToolStripItem)toolStripStatusLabel1).TextImageRelation = (TextImageRelation)8;
		dsResult.DataSetName = "DataSet2";
		dsResult.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
		rMonstrBindingSource.DataMember = "R_Monstr";
		rMonstrBindingSource.DataSource = dsResult;
		rHeroesBindingSource.DataMember = "R_Heroes";
		rHeroesBindingSource.DataSource = dsResult;
		rAllArtsBindingSource.DataMember = "R_AllArts";
		rAllArtsBindingSource.DataSource = dsResult;
		rBindingSource.DataMember = "R_Art";
		rBindingSource.DataSource = dsResult;
		rBindingSource.Sort = "";
		rBankBindingSource.DataMember = "R_Bank";
		rBankBindingSource.DataSource = dsResult;
		rEventBoxBindingSource.DataMember = "R_EventBox";
		rEventBoxBindingSource.DataSource = dsResult;
		rScholarBindingSource.DataMember = "R_Scholar";
		rScholarBindingSource.DataSource = dsResult;
		rResourceBindingSource.DataMember = "R_Resource";
		rResourceBindingSource.DataSource = dsResult;
		rChestBindingSource.DataMember = "R_Chest";
		rChestBindingSource.DataSource = dsResult;
		rSpellBindingSource.DataMember = "R_Spell";
		rSpellBindingSource.DataSource = dsResult;
		rSkillBindingSource.DataMember = "R_Skill";
		rSkillBindingSource.DataSource = dsResult;
		rCampBindingSource.DataMember = "R_Camp";
		rCampBindingSource.DataSource = dsResult;
		rMarketBindingSource.DataMember = "R_Market";
		rMarketBindingSource.DataSource = dsResult;
		rSeerHutBindingSource.DataMember = "R_SeerHut";
		rSeerHutBindingSource.DataSource = dsResult;
		rPassGuardBindingSource.DataMember = "R_PassGuard";
		rPassGuardBindingSource.DataSource = dsResult;
		rGarrisonBindingSource.DataMember = "R_Garrison";
		rGarrisonBindingSource.DataSource = dsResult;
		rPrisonBindingSource.DataMember = "R_Prison";
		rPrisonBindingSource.DataSource = dsResult;
		rObjectBindingSource.DataMember = "R_Object";
		rObjectBindingSource.DataSource = dsResult;
		rTownBindingSource.DataMember = "R_Town";
		rTownBindingSource.DataSource = dsResult;
		((FileDialog)openFileDialog1).Filter = "HoMM3 Files|*.CGM;*.GM1;*GM0|All Files|*.*";
		((FileDialog)openFileDialog1).SupportMultiDottedExtensions = true;
		barManager1.AllowCustomization = false;
		barManager1.Bars.AddRange(new Bar[1] { bar2 });
		barManager1.DockControls.Add(barDockControlTop);
		barManager1.DockControls.Add(barDockControlBottom);
		barManager1.DockControls.Add(barDockControlLeft);
		barManager1.DockControls.Add(barDockControlRight);
		barManager1.Form = (Control)(object)this;
		barManager1.Items.AddRange(new BarItem[24]
		{
			tsButtonFile, tsButtonStart, tsButtonStop, tsButtonRefresh, tsButtonOT, tsMenuService, tsButtonExcel, bButtonOpen, bButtonClear, barSubItem1,
			bsiVerOracle, chi32EN, chi40EN, chi40RU, biDepth, tsButtonSPT, chiFree, barSubItem3, chiStart, biRecent,
			mnuButtonHelp, tsButtonHelp, tsAbout, tsButtonObjectName
		});
		barManager1.MainMenu = bar2;
		barManager1.MaxItemId = 37;
		barManager1.RepositoryItems.AddRange(new RepositoryItem[2] { repositoryItemSpinEdit1, repositoryItemSpinEdit2 });
		barManager1.HighlightedLinkChanged += barManager1_HighlightedLinkChanged;
		bar2.BarName = "Главное меню";
		bar2.CanDockStyle = BarCanDockStyle.Top;
		bar2.DockCol = 0;
		bar2.DockRow = 0;
		bar2.DockStyle = BarDockStyle.Top;
		bar2.FloatLocation = new Point(863, 475);
		bar2.LinksPersistInfo.AddRange(new LinkPersistInfo[8]
		{
			new LinkPersistInfo(tsButtonFile),
			new LinkPersistInfo(tsButtonStart),
			new LinkPersistInfo(tsButtonStop),
			new LinkPersistInfo(tsButtonRefresh),
			new LinkPersistInfo(BarLinkUserDefines.PaintStyle, tsMenuService, BarItemPaintStyle.Standard),
			new LinkPersistInfo(mnuButtonHelp),
			new LinkPersistInfo(tsButtonOT),
			new LinkPersistInfo(tsButtonSPT)
		});
		bar2.OptionsBar.AllowQuickCustomization = false;
		bar2.OptionsBar.DisableClose = true;
		bar2.OptionsBar.DisableCustomization = true;
		bar2.OptionsBar.MultiLine = true;
		bar2.OptionsBar.RotateWhenVertical = false;
		bar2.OptionsBar.UseWholeRow = true;
		bar2.Text = "Главное меню";
		tsButtonFile.ButtonStyle = BarButtonStyle.DropDown;
		tsButtonFile.Caption = "        Файл       ";
		tsButtonFile.DropDownControl = ppmFile;
		tsButtonFile.Id = 0;
		tsButtonFile.Name = "tsButtonFile";
		tsButtonFile.ItemClick += tsButtonFile_ItemClick;
		ppmFile.LinksPersistInfo.AddRange(new LinkPersistInfo[2]
		{
			new LinkPersistInfo(bButtonOpen),
			new LinkPersistInfo(bButtonClear, beginGroup: true)
		});
		ppmFile.Manager = barManager1;
		ppmFile.Name = "ppmFile";
		ppmFile.CloseUp += ppmFile_CloseUp;
		ppmFile.BeforePopup += ppmFile_BeforePopup;
		bButtonOpen.Appearance.Font = new Font("Tahoma", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)204);
		bButtonOpen.Appearance.Options.UseFont = true;
		bButtonOpen.Caption = "Открыть";
		bButtonOpen.Id = 19;
		bButtonOpen.ItemShortcut = new BarShortcut((Keys)131151);
		bButtonOpen.Name = "bButtonOpen";
		bButtonOpen.ItemClick += tsButtonFile_ItemClick;
		bButtonClear.Caption = "Очистить";
		bButtonClear.Id = 20;
		bButtonClear.Name = "bButtonClear";
		bButtonClear.ItemClick += bButtonClear_ItemClick;
		tsButtonStart.Caption = "        Старт        ";
		tsButtonStart.Enabled = false;
		tsButtonStart.Id = 1;
		tsButtonStart.Name = "tsButtonStart";
		tsButtonStart.ItemClick += tsButtonStart_ItemClick;
		tsButtonStop.Caption = "        Стоп        ";
		tsButtonStop.Enabled = false;
		tsButtonStop.Id = 2;
		tsButtonStop.Name = "tsButtonStop";
		tsButtonStop.ItemClick += tsButtonStop_ItemClick;
		tsButtonRefresh.Caption = "       Обновить       ";
		tsButtonRefresh.Enabled = false;
		tsButtonRefresh.Id = 3;
		tsButtonRefresh.Name = "tsButtonRefresh";
		tsButtonRefresh.ItemClick += tsButtonRefresh_ItemClick;
		tsMenuService.Caption = "        Сервис        ";
		tsMenuService.Id = 13;
		tsMenuService.LinksPersistInfo.AddRange(new LinkPersistInfo[3]
		{
			new LinkPersistInfo(tsButtonExcel, beginGroup: true),
			new LinkPersistInfo(barSubItem1, beginGroup: true),
			new LinkPersistInfo(barSubItem3, beginGroup: true)
		});
		tsMenuService.Name = "tsMenuService";
		tsMenuService.Popup += tsMenuService_Popup;
		tsButtonExcel.Caption = "Экспорт в Excel";
		tsButtonExcel.Id = 14;
		tsButtonExcel.ItemShortcut = new BarShortcut((Keys)131148);
		tsButtonExcel.Name = "tsButtonExcel";
		tsButtonExcel.ItemClick += tsButtonExcel_ItemClick;
		barSubItem1.Caption = "Оракул";
		barSubItem1.Id = 21;
		barSubItem1.LinksPersistInfo.AddRange(new LinkPersistInfo[3]
		{
			new LinkPersistInfo(chiFree),
			new LinkPersistInfo(bsiVerOracle, beginGroup: true),
			new LinkPersistInfo(biDepth, beginGroup: true)
		});
		barSubItem1.Name = "barSubItem1";
		chiFree.Caption = "Без запуска игры";
		chiFree.Id = 29;
		chiFree.Name = "chiFree";
		chiFree.CheckedChanged += chiFree_CheckedChanged;
		bsiVerOracle.Caption = "Версия";
		bsiVerOracle.Id = 22;
		bsiVerOracle.LinksPersistInfo.AddRange(new LinkPersistInfo[3]
		{
			new LinkPersistInfo(chi32EN),
			new LinkPersistInfo(chi40EN),
			new LinkPersistInfo(chi40RU)
		});
		bsiVerOracle.Name = "bsiVerOracle";
		chi32EN.Caption = "HoMM 3.2 EN";
		chi32EN.Checked = true;
		chi32EN.Id = 24;
		chi32EN.Name = "chi32EN";
		chi32EN.CheckedChanged += chi32EN_CheckedChanged;
		chi40EN.Caption = "HoMM3 4.0 EN";
		chi40EN.Id = 25;
		chi40EN.Name = "chi40EN";
		chi40EN.CheckedChanged += chi40EN_CheckedChanged;
		chi40RU.Caption = "HoMM3 4.0 RU";
		chi40RU.Id = 26;
		chi40RU.Name = "chi40RU";
		chi40RU.CheckedChanged += chi40RU_CheckedChanged;
		biDepth.Caption = "Максимум уровней";
		biDepth.Edit = repositoryItemSpinEdit1;
		biDepth.EditValue = 10;
		biDepth.Id = 27;
		biDepth.Name = "biDepth";
		biDepth.EditValueChanged += biDepth_EditValueChanged;
		repositoryItemSpinEdit1.AutoHeight = false;
		repositoryItemSpinEdit1.Buttons.AddRange(new EditorButton[1]
		{
			new EditorButton()
		});
		repositoryItemSpinEdit1.IsFloatValue = false;
		repositoryItemSpinEdit1.Mask.EditMask = "N00";
		repositoryItemSpinEdit1.MaxValue = new decimal(new int[4] { 20, 0, 0, 0 });
		repositoryItemSpinEdit1.MinValue = new decimal(new int[4] { 3, 0, 0, 0 });
		repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
		barSubItem3.Caption = "Настройка";
		barSubItem3.Id = 30;
		barSubItem3.LinksPersistInfo.AddRange(new LinkPersistInfo[3]
		{
			new LinkPersistInfo(chiStart),
			new LinkPersistInfo(biRecent, beginGroup: true),
			new LinkPersistInfo(tsButtonObjectName, beginGroup: true)
		});
		barSubItem3.Name = "barSubItem3";
		chiStart.Caption = "Кнопка  «Старт»";
		chiStart.Checked = true;
		chiStart.Id = 31;
		chiStart.Name = "chiStart";
		chiStart.CheckedChanged += chiStart_CheckedChanged;
		biRecent.Caption = "Последних открытых файлов";
		biRecent.Edit = repositoryItemSpinEdit2;
		biRecent.EditValue = "8";
		biRecent.Id = 32;
		biRecent.Name = "biRecent";
		biRecent.EditValueChanged += biRecent_EditValueChanged;
		repositoryItemSpinEdit2.AutoHeight = false;
		repositoryItemSpinEdit2.Buttons.AddRange(new EditorButton[1]
		{
			new EditorButton()
		});
		repositoryItemSpinEdit2.IsFloatValue = false;
		repositoryItemSpinEdit2.Mask.EditMask = "N00";
		repositoryItemSpinEdit2.MaxValue = new decimal(new int[4] { 16, 0, 0, 0 });
		repositoryItemSpinEdit2.MinValue = new decimal(new int[4] { 2, 0, 0, 0 });
		repositoryItemSpinEdit2.Name = "repositoryItemSpinEdit2";
		tsButtonObjectName.Caption = "Названия объектов";
		tsButtonObjectName.Id = 36;
		tsButtonObjectName.Name = "tsButtonObjectName";
		tsButtonObjectName.ItemClick += tsButtonObjectName_ItemClick;
		mnuButtonHelp.Caption = "        Справка        ";
		mnuButtonHelp.Id = 33;
		mnuButtonHelp.LinksPersistInfo.AddRange(new LinkPersistInfo[2]
		{
			new LinkPersistInfo(tsButtonHelp),
			new LinkPersistInfo(tsAbout, beginGroup: true)
		});
		mnuButtonHelp.Name = "mnuButtonHelp";
		tsButtonHelp.Caption = "Справка";
		tsButtonHelp.Id = 34;
		tsButtonHelp.ItemShortcut = new BarShortcut((Keys)112);
		tsButtonHelp.Name = "tsButtonHelp";
		tsButtonHelp.ItemClick += tsButtonHelp_ItemClick;
		tsAbout.Caption = "О программе";
		tsAbout.Id = 35;
		tsAbout.Name = "tsAbout";
		tsAbout.ItemClick += tsAbout_ItemClick;
		tsButtonOT.Caption = "     OT-3     ";
		tsButtonOT.Enabled = false;
		tsButtonOT.Id = 12;
		tsButtonOT.Name = "tsButtonOT";
		tsButtonOT.Visibility = BarItemVisibility.Never;
		tsButtonOT.ItemClick += tsButtonOT_ItemClick;
		tsButtonSPT.Caption = "     SPT     ";
		tsButtonSPT.Enabled = false;
		tsButtonSPT.Id = 28;
		tsButtonSPT.Name = "tsButtonSPT";
		tsButtonSPT.Visibility = BarItemVisibility.Never;
		tsButtonSPT.ItemClick += tsButtonSPT_ItemClick;
		((Control)barDockControlTop).CausesValidation = false;
		barDockControlTop.Dock = (DockStyle)1;
		barDockControlTop.Location = new Point(0, 0);
		barDockControlTop.Size = new Size(974, 22);
		((Control)barDockControlBottom).CausesValidation = false;
		barDockControlBottom.Dock = (DockStyle)2;
		barDockControlBottom.Location = new Point(0, 537);
		barDockControlBottom.Size = new Size(974, 0);
		((Control)barDockControlLeft).CausesValidation = false;
		barDockControlLeft.Dock = (DockStyle)3;
		barDockControlLeft.Location = new Point(0, 22);
		barDockControlLeft.Size = new Size(0, 515);
		((Control)barDockControlRight).CausesValidation = false;
		barDockControlRight.Dock = (DockStyle)4;
		barDockControlRight.Location = new Point(974, 22);
		barDockControlRight.Size = new Size(0, 515);
		((Control)tabControl).Dock = (DockStyle)5;
		((Control)tabControl).Location = new Point(0, 22);
		tabControl.LookAndFeel.SkinName = "Blue";
		tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
		tabControl.MultiLine = DefaultBoolean.True;
		((Control)tabControl).Name = "tabControl";
		tabControl.SelectedTabPage = tabArt;
		((Control)tabControl).Size = new Size(974, 493);
		((Control)tabControl).TabIndex = 10;
		tabControl.TabPages.AddRange(new XtraTabPage[24]
		{
			tabArt, tabMonster, tabBank, tabBoxEvent, tabScholar, tabResource, tabChest, tabSpell, tabSkill, tabCamp,
			tabMarket, tabSeerHut, tabPassGuard, tabGarrison, tabPrison, tabObject, tabTopology, tabAllTimer, tabHero, tabTown,
			tabAllArts, tabAllSpell, tabAllSkill, tabExperience
		});
		tabArt.Appearance.Header.Font = new Font("Tahoma", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)204);
		tabArt.Appearance.Header.Options.UseFont = true;
		((Control)tabArt).Controls.Add((Control)(object)grdArt);
		((Control)tabArt).Name = "tabArt";
		tabArt.Size = new Size(969, 448);
		((Control)tabArt).Text = "Артефакты";
		grdArt.DataSource = rBindingSource;
		((Control)grdArt).Dock = (DockStyle)5;
		((Control)grdArt).Location = new Point(0, 0);
		grdArt.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdArt.LookAndFeel.UseDefaultLookAndFeel = false;
		grdArt.MainView = vwArt;
		grdArt.MenuManager = barManager1;
		((Control)grdArt).Name = "grdArt";
		((Control)grdArt).Size = new Size(969, 448);
		((Control)grdArt).TabIndex = 0;
		grdArt.ToolTipController = toolTipController1;
		grdArt.ViewCollection.AddRange(new BaseView[1] { vwArt });
		vwArt.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwArt.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwArt.Appearance.FocusedRow.ForeColor = Color.White;
		vwArt.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwArt.Appearance.FocusedRow.Options.UseBackColor = true;
		vwArt.Appearance.FocusedRow.Options.UseForeColor = true;
		vwArt.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwArt.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwArt.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwArt.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwArt.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwArt.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwArt.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwArt.Appearance.SelectedRow.ForeColor = Color.Black;
		vwArt.Appearance.SelectedRow.Options.UseBackColor = true;
		vwArt.Appearance.SelectedRow.Options.UseForeColor = true;
		vwArt.Columns.AddRange(new GridColumn[13]
		{
			colX, colY, colZ, colLocality, colObject, colSlot, colName, colClass, colRelicC, colGold,
			colResource, colGuard, colHP1
		});
		vwArt.CustomizationFormBounds = new Rectangle(752, 259, 216, 185);
		vwArt.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwArt.GridControl = grdArt;
		vwArt.GroupPanelText = " ";
		vwArt.Name = "vwArt";
		vwArt.OptionsBehavior.AllowIncrementalSearch = true;
		vwArt.OptionsBehavior.Editable = false;
		vwArt.OptionsBehavior.ReadOnly = true;
		vwArt.OptionsFilter.UseNewCustomFilterDialog = true;
		vwArt.OptionsNavigation.UseTabKey = false;
		vwArt.OptionsSelection.MultiSelect = true;
		vwArt.OptionsView.ShowGroupPanel = false;
		colX.FieldName = "X";
		colX.Name = "colX";
		colX.Visible = true;
		colX.VisibleIndex = 0;
		colX.Width = 27;
		colY.FieldName = "Y";
		colY.Name = "colY";
		colY.Visible = true;
		colY.VisibleIndex = 1;
		colY.Width = 27;
		colZ.FieldName = "Z";
		colZ.Name = "colZ";
		colZ.Visible = true;
		colZ.VisibleIndex = 2;
		colZ.Width = 27;
		colLocality.Caption = "Район";
		colLocality.FieldName = "Locality";
		colLocality.Name = "colLocality";
		colLocality.Visible = true;
		colLocality.VisibleIndex = 3;
		colLocality.Width = 32;
		colObject.Caption = "Объект";
		colObject.FieldName = "Object";
		colObject.Name = "colObject";
		colObject.Visible = true;
		colObject.VisibleIndex = 4;
		colObject.Width = 140;
		colSlot.Caption = "Слот";
		colSlot.FieldName = "Slot";
		colSlot.Name = "colSlot";
		colSlot.Visible = true;
		colSlot.VisibleIndex = 5;
		colSlot.Width = 35;
		colName.Caption = "Артефакт";
		colName.FieldName = "Name";
		colName.Name = "colName";
		colName.Visible = true;
		colName.VisibleIndex = 6;
		colName.Width = 200;
		colClass.Caption = "Класс";
		colClass.FieldName = "Class";
		colClass.Name = "colClass";
		colClass.Visible = true;
		colClass.VisibleIndex = 7;
		colClass.Width = 70;
		colRelicC.Caption = "Реликт-С";
		colRelicC.FieldName = "Relic-C";
		colRelicC.Name = "colRelicC";
		colRelicC.Visible = true;
		colRelicC.VisibleIndex = 8;
		colRelicC.Width = 90;
		colGold.Caption = "Золото";
		colGold.FieldName = "Gold";
		colGold.Name = "colGold";
		colGold.Visible = true;
		colGold.VisibleIndex = 9;
		colGold.Width = 50;
		colResource.Caption = "Ресурс";
		colResource.FieldName = "Resource";
		colResource.Name = "colResource";
		colResource.Visible = true;
		colResource.VisibleIndex = 10;
		colResource.Width = 50;
		colGuard.Caption = "Охрана";
		colGuard.FieldName = "Guard";
		colGuard.Name = "colGuard";
		colGuard.Visible = true;
		colGuard.VisibleIndex = 11;
		colGuard.Width = 160;
		colHP1.Caption = "∑ HP";
		colHP1.FieldName = "HP";
		colHP1.Name = "colHP1";
		colHP1.Visible = true;
		colHP1.VisibleIndex = 12;
		colHP1.Width = 40;
		toolTipController1.AutoPopDelay = 60000;
		toolTipController1.ShowBeak = true;
		toolTipController1.ToolTipType = ToolTipType.SuperTip;
		toolTipController1.BeforeShow += toolTipController1_BeforeShow;
		((Control)tabMonster).Controls.Add((Control)(object)grdMonster);
		((Control)tabMonster).Name = "tabMonster";
		tabMonster.Size = new Size(969, 448);
		((Control)tabMonster).Text = "Монстры";
		grdMonster.DataSource = rMonstrBindingSource;
		((Control)grdMonster).Dock = (DockStyle)5;
		((Control)grdMonster).Location = new Point(0, 0);
		grdMonster.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdMonster.LookAndFeel.UseDefaultLookAndFeel = false;
		grdMonster.MainView = vwMonster;
		grdMonster.MenuManager = barManager1;
		((Control)grdMonster).Name = "grdMonster";
		((Control)grdMonster).Size = new Size(969, 448);
		((Control)grdMonster).TabIndex = 0;
		grdMonster.ToolTipController = toolTipController1;
		grdMonster.ViewCollection.AddRange(new BaseView[1] { vwMonster });
		vwMonster.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwMonster.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwMonster.Appearance.FocusedRow.ForeColor = Color.White;
		vwMonster.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwMonster.Appearance.FocusedRow.Options.UseBackColor = true;
		vwMonster.Appearance.FocusedRow.Options.UseForeColor = true;
		vwMonster.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwMonster.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwMonster.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwMonster.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwMonster.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwMonster.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwMonster.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwMonster.Appearance.SelectedRow.ForeColor = Color.Black;
		vwMonster.Appearance.SelectedRow.Options.UseBackColor = true;
		vwMonster.Appearance.SelectedRow.Options.UseForeColor = true;
		vwMonster.Columns.AddRange(new GridColumn[13]
		{
			colX1, colY1, colZ1, colLocality1, colName1, colNumber, colMood, colLevel, colArt, colGold1,
			colResource1, colIncrease, colHP2
		});
		vwMonster.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwMonster.GridControl = grdMonster;
		vwMonster.GroupPanelText = " ";
		vwMonster.Name = "vwMonster";
		vwMonster.OptionsBehavior.AllowIncrementalSearch = true;
		vwMonster.OptionsBehavior.Editable = false;
		vwMonster.OptionsBehavior.ReadOnly = true;
		vwMonster.OptionsFilter.UseNewCustomFilterDialog = true;
		vwMonster.OptionsNavigation.UseTabKey = false;
		vwMonster.OptionsSelection.MultiSelect = true;
		vwMonster.OptionsView.ShowGroupPanel = false;
		colX1.FieldName = "X";
		colX1.Name = "colX1";
		colX1.Visible = true;
		colX1.VisibleIndex = 0;
		colX1.Width = 27;
		colY1.FieldName = "Y";
		colY1.Name = "colY1";
		colY1.Visible = true;
		colY1.VisibleIndex = 1;
		colY1.Width = 27;
		colZ1.FieldName = "Z";
		colZ1.Name = "colZ1";
		colZ1.Visible = true;
		colZ1.VisibleIndex = 2;
		colZ1.Width = 27;
		colLocality1.Caption = "Район";
		colLocality1.FieldName = "Locality";
		colLocality1.Name = "colLocality1";
		colLocality1.Visible = true;
		colLocality1.VisibleIndex = 3;
		colLocality1.Width = 32;
		colName1.Caption = "Монстр";
		colName1.FieldName = "Name";
		colName1.Name = "colName1";
		colName1.Visible = true;
		colName1.VisibleIndex = 4;
		colName1.Width = 249;
		colNumber.Caption = "Количество";
		colNumber.FieldName = "Number";
		colNumber.Name = "colNumber";
		colNumber.Visible = true;
		colNumber.VisibleIndex = 5;
		colNumber.Width = 43;
		colMood.Caption = "Настроение";
		colMood.FieldName = "Mood";
		colMood.Name = "colMood";
		colMood.Visible = true;
		colMood.VisibleIndex = 6;
		colMood.Width = 42;
		colLevel.Caption = "Уровень";
		colLevel.FieldName = "Level";
		colLevel.Name = "colLevel";
		colLevel.Visible = true;
		colLevel.VisibleIndex = 7;
		colLevel.Width = 38;
		colArt.Caption = "Артефакт";
		colArt.FieldName = "Art";
		colArt.Name = "colArt";
		colArt.Visible = true;
		colArt.VisibleIndex = 9;
		colArt.Width = 192;
		colGold1.Caption = "Золото";
		colGold1.FieldName = "Gold";
		colGold1.Name = "colGold1";
		colGold1.Visible = true;
		colGold1.VisibleIndex = 10;
		colResource1.Caption = "Ресурс";
		colResource1.FieldName = "Resource";
		colResource1.Name = "colResource1";
		colResource1.Visible = true;
		colResource1.VisibleIndex = 11;
		colResource1.Width = 64;
		colIncrease.Caption = "Прирост";
		colIncrease.FieldName = "Increase";
		colIncrease.Name = "colIncrease";
		colIncrease.Visible = true;
		colIncrease.VisibleIndex = 12;
		colIncrease.Width = 86;
		colHP2.Caption = "∑ HP";
		colHP2.FieldName = "HP";
		colHP2.Name = "colHP2";
		colHP2.Visible = true;
		colHP2.VisibleIndex = 8;
		colHP2.Width = 46;
		((Control)tabBank).Controls.Add((Control)(object)grdBank);
		((Control)tabBank).Name = "tabBank";
		tabBank.Size = new Size(969, 448);
		((Control)tabBank).Text = "Банки";
		grdBank.DataSource = rBankBindingSource;
		((Control)grdBank).Dock = (DockStyle)5;
		((Control)grdBank).Location = new Point(0, 0);
		grdBank.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdBank.LookAndFeel.UseDefaultLookAndFeel = false;
		grdBank.MainView = vwBank;
		grdBank.MenuManager = barManager1;
		((Control)grdBank).Name = "grdBank";
		((Control)grdBank).Size = new Size(969, 448);
		((Control)grdBank).TabIndex = 0;
		grdBank.ToolTipController = toolTipController1;
		grdBank.ViewCollection.AddRange(new BaseView[1] { vwBank });
		vwBank.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwBank.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwBank.Appearance.FocusedRow.ForeColor = Color.White;
		vwBank.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwBank.Appearance.FocusedRow.Options.UseBackColor = true;
		vwBank.Appearance.FocusedRow.Options.UseForeColor = true;
		vwBank.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwBank.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwBank.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwBank.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwBank.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwBank.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwBank.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwBank.Appearance.SelectedRow.ForeColor = Color.Black;
		vwBank.Appearance.SelectedRow.Options.UseBackColor = true;
		vwBank.Appearance.SelectedRow.Options.UseForeColor = true;
		vwBank.Columns.AddRange(new GridColumn[10] { colX2, colY2, colZ2, colLocality2, colName2, colGuard1, colMonster, colGold2, colResource2, colHP3 });
		vwBank.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwBank.GridControl = grdBank;
		vwBank.GroupPanelText = " ";
		vwBank.Name = "vwBank";
		vwBank.OptionsBehavior.AllowIncrementalSearch = true;
		vwBank.OptionsBehavior.Editable = false;
		vwBank.OptionsBehavior.ReadOnly = true;
		vwBank.OptionsFilter.UseNewCustomFilterDialog = true;
		vwBank.OptionsNavigation.UseTabKey = false;
		vwBank.OptionsSelection.MultiSelect = true;
		vwBank.OptionsView.ShowGroupPanel = false;
		colX2.FieldName = "X";
		colX2.Name = "colX2";
		colX2.Visible = true;
		colX2.VisibleIndex = 0;
		colX2.Width = 27;
		colY2.FieldName = "Y";
		colY2.Name = "colY2";
		colY2.Visible = true;
		colY2.VisibleIndex = 1;
		colY2.Width = 27;
		colZ2.FieldName = "Z";
		colZ2.Name = "colZ2";
		colZ2.Visible = true;
		colZ2.VisibleIndex = 2;
		colZ2.Width = 27;
		colLocality2.Caption = "Район";
		colLocality2.FieldName = "Locality";
		colLocality2.Name = "colLocality2";
		colLocality2.Visible = true;
		colLocality2.VisibleIndex = 3;
		colLocality2.Width = 32;
		colName2.Caption = "Объект";
		colName2.FieldName = "Name";
		colName2.Name = "colName2";
		colName2.Visible = true;
		colName2.VisibleIndex = 4;
		colName2.Width = 221;
		colGuard1.Caption = "Охрана";
		colGuard1.FieldName = "Guard";
		colGuard1.Name = "colGuard1";
		colGuard1.Visible = true;
		colGuard1.VisibleIndex = 5;
		colGuard1.Width = 269;
		colMonster.Caption = "Монстр";
		colMonster.FieldName = "Monster";
		colMonster.Name = "colMonster";
		colMonster.Visible = true;
		colMonster.VisibleIndex = 7;
		colMonster.Width = 131;
		colGold2.Caption = "Золото";
		colGold2.FieldName = "Gold";
		colGold2.Name = "colGold2";
		colGold2.Visible = true;
		colGold2.VisibleIndex = 8;
		colGold2.Width = 53;
		colResource2.Caption = "Ресурс";
		colResource2.FieldName = "Resource";
		colResource2.Name = "colResource2";
		colResource2.Visible = true;
		colResource2.VisibleIndex = 9;
		colResource2.Width = 115;
		colHP3.Caption = "∑ HP";
		colHP3.FieldName = "HP";
		colHP3.Name = "colHP3";
		colHP3.Visible = true;
		colHP3.VisibleIndex = 6;
		colHP3.Width = 46;
		((Control)tabBoxEvent).Controls.Add((Control)(object)grdEventBox);
		((Control)tabBoxEvent).Name = "tabBoxEvent";
		tabBoxEvent.Size = new Size(969, 448);
		((Control)tabBoxEvent).Text = "События и Ящики Пандоры";
		grdEventBox.DataSource = rEventBoxBindingSource;
		((Control)grdEventBox).Dock = (DockStyle)5;
		((Control)grdEventBox).Location = new Point(0, 0);
		grdEventBox.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdEventBox.LookAndFeel.UseDefaultLookAndFeel = false;
		grdEventBox.MainView = vwEventBox;
		grdEventBox.MenuManager = barManager1;
		((Control)grdEventBox).Name = "grdEventBox";
		((Control)grdEventBox).Size = new Size(969, 448);
		((Control)grdEventBox).TabIndex = 0;
		grdEventBox.ToolTipController = toolTipController1;
		grdEventBox.ViewCollection.AddRange(new BaseView[1] { vwEventBox });
		vwEventBox.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwEventBox.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwEventBox.Appearance.FocusedRow.ForeColor = Color.White;
		vwEventBox.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwEventBox.Appearance.FocusedRow.Options.UseBackColor = true;
		vwEventBox.Appearance.FocusedRow.Options.UseForeColor = true;
		vwEventBox.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwEventBox.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwEventBox.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwEventBox.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwEventBox.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwEventBox.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwEventBox.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwEventBox.Appearance.SelectedRow.ForeColor = Color.Black;
		vwEventBox.Appearance.SelectedRow.Options.UseBackColor = true;
		vwEventBox.Appearance.SelectedRow.Options.UseForeColor = true;
		vwEventBox.Columns.AddRange(new GridColumn[20]
		{
			colX3, colY3, colZ3, colLocality3, colObject1, colGuard2, colExperience, colMana, colMorale, colLuck,
			colGold3, colResource3, colPrimarySkill, colSecondarySkill, colArtefact, colSpell, colMonster1, colHP4, colApply2, colRepeat1
		});
		vwEventBox.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwEventBox.GridControl = grdEventBox;
		vwEventBox.GroupPanelText = " ";
		vwEventBox.Name = "vwEventBox";
		vwEventBox.OptionsBehavior.AllowIncrementalSearch = true;
		vwEventBox.OptionsBehavior.Editable = false;
		vwEventBox.OptionsBehavior.ReadOnly = true;
		vwEventBox.OptionsFilter.UseNewCustomFilterDialog = true;
		vwEventBox.OptionsNavigation.UseTabKey = false;
		vwEventBox.OptionsSelection.MultiSelect = true;
		vwEventBox.OptionsView.ShowGroupPanel = false;
		colX3.FieldName = "X";
		colX3.Name = "colX3";
		colX3.Visible = true;
		colX3.VisibleIndex = 0;
		colX3.Width = 27;
		colY3.FieldName = "Y";
		colY3.Name = "colY3";
		colY3.Visible = true;
		colY3.VisibleIndex = 1;
		colY3.Width = 27;
		colZ3.FieldName = "Z";
		colZ3.Name = "colZ3";
		colZ3.Visible = true;
		colZ3.VisibleIndex = 2;
		colZ3.Width = 27;
		colLocality3.Caption = "Район";
		colLocality3.FieldName = "Locality";
		colLocality3.Name = "colLocality3";
		colLocality3.Visible = true;
		colLocality3.VisibleIndex = 3;
		colLocality3.Width = 32;
		colObject1.Caption = "Объект";
		colObject1.FieldName = "Object";
		colObject1.Name = "colObject1";
		colObject1.Visible = true;
		colObject1.VisibleIndex = 4;
		colObject1.Width = 65;
		colGuard2.Caption = "Охрана";
		colGuard2.FieldName = "Guard";
		colGuard2.Name = "colGuard2";
		colGuard2.Visible = true;
		colGuard2.VisibleIndex = 5;
		colGuard2.Width = 98;
		colExperience.Caption = "Опыт";
		colExperience.FieldName = "Experience";
		colExperience.Name = "colExperience";
		colExperience.Visible = true;
		colExperience.VisibleIndex = 7;
		colExperience.Width = 38;
		colMana.Caption = "Мана";
		colMana.FieldName = "Mana";
		colMana.Name = "colMana";
		colMana.Visible = true;
		colMana.VisibleIndex = 8;
		colMana.Width = 36;
		colMorale.Caption = "Мораль";
		colMorale.FieldName = "Morale";
		colMorale.Name = "colMorale";
		colMorale.Visible = true;
		colMorale.VisibleIndex = 9;
		colMorale.Width = 31;
		colLuck.Caption = "Удача";
		colLuck.FieldName = "Luck";
		colLuck.Name = "colLuck";
		colLuck.Visible = true;
		colLuck.VisibleIndex = 10;
		colLuck.Width = 31;
		colGold3.Caption = "Золото";
		colGold3.FieldName = "Gold";
		colGold3.Name = "colGold3";
		colGold3.Visible = true;
		colGold3.VisibleIndex = 11;
		colGold3.Width = 42;
		colResource3.Caption = "Ресурс";
		colResource3.FieldName = "Resource";
		colResource3.Name = "colResource3";
		colResource3.Visible = true;
		colResource3.VisibleIndex = 12;
		colResource3.Width = 45;
		colPrimarySkill.Caption = "Первичный навык";
		colPrimarySkill.FieldName = "PrimarySkill";
		colPrimarySkill.Name = "colPrimarySkill";
		colPrimarySkill.Visible = true;
		colPrimarySkill.VisibleIndex = 13;
		colPrimarySkill.Width = 62;
		colSecondarySkill.Caption = "Вторичный навык";
		colSecondarySkill.FieldName = "SecondarySkill";
		colSecondarySkill.Name = "colSecondarySkill";
		colSecondarySkill.Visible = true;
		colSecondarySkill.VisibleIndex = 14;
		colSecondarySkill.Width = 78;
		colArtefact.Caption = "Артефакт";
		colArtefact.FieldName = "Artefact";
		colArtefact.Name = "colArtefact";
		colArtefact.Visible = true;
		colArtefact.VisibleIndex = 15;
		colSpell.Caption = "Заклинание";
		colSpell.FieldName = "Spell";
		colSpell.Name = "colSpell";
		colSpell.Visible = true;
		colSpell.VisibleIndex = 16;
		colMonster1.Caption = "Монстр";
		colMonster1.FieldName = "Monster";
		colMonster1.Name = "colMonster1";
		colMonster1.Visible = true;
		colMonster1.VisibleIndex = 17;
		colHP4.Caption = "∑ HP";
		colHP4.FieldName = "HP";
		colHP4.Name = "colHP4";
		colHP4.Visible = true;
		colHP4.VisibleIndex = 6;
		colHP4.Width = 40;
		colApply2.Caption = "Доступно";
		colApply2.FieldName = "Apply";
		colApply2.Name = "colApply2";
		colApply2.Visible = true;
		colApply2.VisibleIndex = 18;
		colApply2.Width = 44;
		colRepeat1.Caption = "Повтор";
		colRepeat1.FieldName = "Repeat";
		colRepeat1.Name = "colRepeat1";
		colRepeat1.Width = 35;
		((Control)tabScholar).Controls.Add((Control)(object)grdScholar);
		((Control)tabScholar).Name = "tabScholar";
		tabScholar.Size = new Size(969, 448);
		((Control)tabScholar).Text = "Ученые";
		grdScholar.DataSource = rScholarBindingSource;
		((Control)grdScholar).Dock = (DockStyle)5;
		((Control)grdScholar).Location = new Point(0, 0);
		grdScholar.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdScholar.LookAndFeel.UseDefaultLookAndFeel = false;
		grdScholar.MainView = vwScholar;
		grdScholar.MenuManager = barManager1;
		((Control)grdScholar).Name = "grdScholar";
		((Control)grdScholar).Size = new Size(969, 448);
		((Control)grdScholar).TabIndex = 0;
		grdScholar.ToolTipController = toolTipController1;
		grdScholar.ViewCollection.AddRange(new BaseView[1] { vwScholar });
		vwScholar.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwScholar.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwScholar.Appearance.FocusedRow.ForeColor = Color.White;
		vwScholar.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwScholar.Appearance.FocusedRow.Options.UseBackColor = true;
		vwScholar.Appearance.FocusedRow.Options.UseForeColor = true;
		vwScholar.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwScholar.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwScholar.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwScholar.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwScholar.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwScholar.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwScholar.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwScholar.Appearance.SelectedRow.ForeColor = Color.Black;
		vwScholar.Appearance.SelectedRow.Options.UseBackColor = true;
		vwScholar.Appearance.SelectedRow.Options.UseForeColor = true;
		vwScholar.Columns.AddRange(new GridColumn[7] { colX4, colY4, colZ4, colLocality4, colSpell1, colPrimarySkill1, colSecondarySkill1 });
		vwScholar.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwScholar.GridControl = grdScholar;
		vwScholar.GroupPanelText = " ";
		vwScholar.Name = "vwScholar";
		vwScholar.OptionsBehavior.AllowIncrementalSearch = true;
		vwScholar.OptionsBehavior.Editable = false;
		vwScholar.OptionsBehavior.ReadOnly = true;
		vwScholar.OptionsFilter.UseNewCustomFilterDialog = true;
		vwScholar.OptionsNavigation.UseTabKey = false;
		vwScholar.OptionsSelection.MultiSelect = true;
		vwScholar.OptionsView.ShowGroupPanel = false;
		colX4.FieldName = "X";
		colX4.Name = "colX4";
		colX4.Visible = true;
		colX4.VisibleIndex = 0;
		colX4.Width = 27;
		colY4.FieldName = "Y";
		colY4.Name = "colY4";
		colY4.Visible = true;
		colY4.VisibleIndex = 1;
		colY4.Width = 27;
		colZ4.FieldName = "Z";
		colZ4.Name = "colZ4";
		colZ4.Visible = true;
		colZ4.VisibleIndex = 2;
		colZ4.Width = 27;
		colLocality4.Caption = "Район";
		colLocality4.FieldName = "Locality";
		colLocality4.Name = "colLocality4";
		colLocality4.Visible = true;
		colLocality4.VisibleIndex = 3;
		colLocality4.Width = 32;
		colSpell1.Caption = "Заклинание";
		colSpell1.FieldName = "Spell";
		colSpell1.Name = "colSpell1";
		colSpell1.Visible = true;
		colSpell1.VisibleIndex = 6;
		colSpell1.Width = 350;
		colPrimarySkill1.Caption = "Первичный навык";
		colPrimarySkill1.FieldName = "PrimarySkill";
		colPrimarySkill1.Name = "colPrimarySkill1";
		colPrimarySkill1.Visible = true;
		colPrimarySkill1.VisibleIndex = 4;
		colPrimarySkill1.Width = 123;
		colSecondarySkill1.Caption = "Вторичный навык";
		colSecondarySkill1.FieldName = "SecondarySkill";
		colSecondarySkill1.Name = "colSecondarySkill1";
		colSecondarySkill1.Visible = true;
		colSecondarySkill1.VisibleIndex = 5;
		colSecondarySkill1.Width = 362;
		((Control)tabResource).Controls.Add((Control)(object)grdResource);
		((Control)tabResource).Name = "tabResource";
		tabResource.Size = new Size(969, 448);
		((Control)tabResource).Text = "Ресурсы";
		grdResource.DataSource = rResourceBindingSource;
		((Control)grdResource).Dock = (DockStyle)5;
		((Control)grdResource).Location = new Point(0, 0);
		grdResource.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdResource.LookAndFeel.UseDefaultLookAndFeel = false;
		grdResource.MainView = vwResource;
		grdResource.MenuManager = barManager1;
		((Control)grdResource).Name = "grdResource";
		((Control)grdResource).Size = new Size(969, 448);
		((Control)grdResource).TabIndex = 0;
		grdResource.ToolTipController = toolTipController1;
		grdResource.ViewCollection.AddRange(new BaseView[1] { vwResource });
		vwResource.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwResource.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwResource.Appearance.FocusedRow.ForeColor = Color.White;
		vwResource.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwResource.Appearance.FocusedRow.Options.UseBackColor = true;
		vwResource.Appearance.FocusedRow.Options.UseForeColor = true;
		vwResource.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwResource.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwResource.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwResource.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwResource.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwResource.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwResource.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwResource.Appearance.SelectedRow.ForeColor = Color.Black;
		vwResource.Appearance.SelectedRow.Options.UseBackColor = true;
		vwResource.Appearance.SelectedRow.Options.UseForeColor = true;
		vwResource.Columns.AddRange(new GridColumn[9] { colX5, colY5, colZ5, colLocality5, colObject2, colResource4, colGold4, colGuard3, colHP5 });
		vwResource.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwResource.GridControl = grdResource;
		vwResource.GroupPanelText = " ";
		vwResource.Name = "vwResource";
		vwResource.OptionsBehavior.AllowIncrementalSearch = true;
		vwResource.OptionsBehavior.Editable = false;
		vwResource.OptionsBehavior.ReadOnly = true;
		vwResource.OptionsFilter.UseNewCustomFilterDialog = true;
		vwResource.OptionsNavigation.UseTabKey = false;
		vwResource.OptionsSelection.MultiSelect = true;
		vwResource.OptionsView.ShowGroupPanel = false;
		colX5.FieldName = "X";
		colX5.Name = "colX5";
		colX5.Visible = true;
		colX5.VisibleIndex = 0;
		colX5.Width = 27;
		colY5.FieldName = "Y";
		colY5.Name = "colY5";
		colY5.Visible = true;
		colY5.VisibleIndex = 1;
		colY5.Width = 27;
		colZ5.FieldName = "Z";
		colZ5.Name = "colZ5";
		colZ5.Visible = true;
		colZ5.VisibleIndex = 2;
		colZ5.Width = 27;
		colLocality5.Caption = "Район";
		colLocality5.FieldName = "Locality";
		colLocality5.Name = "colLocality5";
		colLocality5.Visible = true;
		colLocality5.VisibleIndex = 3;
		colLocality5.Width = 32;
		colObject2.Caption = "Объект";
		colObject2.FieldName = "Object";
		colObject2.Name = "colObject2";
		colObject2.Visible = true;
		colObject2.VisibleIndex = 4;
		colObject2.Width = 175;
		colResource4.Caption = "Ресурс";
		colResource4.FieldName = "Resource";
		colResource4.Name = "colResource4";
		colResource4.Visible = true;
		colResource4.VisibleIndex = 5;
		colResource4.Width = 106;
		colGold4.Caption = "Золото";
		colGold4.FieldName = "Gold";
		colGold4.Name = "colGold4";
		colGold4.Visible = true;
		colGold4.VisibleIndex = 6;
		colGold4.Width = 66;
		colGuard3.Caption = "Охрана";
		colGuard3.FieldName = "Guard";
		colGuard3.Name = "colGuard3";
		colGuard3.Visible = true;
		colGuard3.VisibleIndex = 7;
		colGuard3.Width = 419;
		colHP5.Caption = "∑ HP";
		colHP5.FieldName = "HP";
		colHP5.Name = "colHP5";
		colHP5.Visible = true;
		colHP5.VisibleIndex = 8;
		colHP5.Width = 46;
		((Control)tabChest).Controls.Add((Control)(object)grdChest);
		((Control)tabChest).Name = "tabChest";
		tabChest.Size = new Size(969, 448);
		((Control)tabChest).Text = "Сундуки";
		grdChest.DataSource = rChestBindingSource;
		((Control)grdChest).Dock = (DockStyle)5;
		((Control)grdChest).Location = new Point(0, 0);
		grdChest.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdChest.LookAndFeel.UseDefaultLookAndFeel = false;
		grdChest.MainView = vwChest;
		grdChest.MenuManager = barManager1;
		((Control)grdChest).Name = "grdChest";
		((Control)grdChest).Size = new Size(969, 448);
		((Control)grdChest).TabIndex = 0;
		grdChest.ToolTipController = toolTipController1;
		grdChest.ViewCollection.AddRange(new BaseView[1] { vwChest });
		vwChest.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwChest.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwChest.Appearance.FocusedRow.ForeColor = Color.White;
		vwChest.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwChest.Appearance.FocusedRow.Options.UseBackColor = true;
		vwChest.Appearance.FocusedRow.Options.UseForeColor = true;
		vwChest.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwChest.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwChest.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwChest.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwChest.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwChest.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwChest.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwChest.Appearance.SelectedRow.ForeColor = Color.Black;
		vwChest.Appearance.SelectedRow.Options.UseBackColor = true;
		vwChest.Appearance.SelectedRow.Options.UseForeColor = true;
		vwChest.Columns.AddRange(new GridColumn[10] { colX6, colY6, colZ6, colLocality6, colObject3, colArt1, colClass1, colRelicC6, colGold5, colHP6 });
		vwChest.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwChest.GridControl = grdChest;
		vwChest.GroupPanelText = " ";
		vwChest.Name = "vwChest";
		vwChest.OptionsBehavior.AllowIncrementalSearch = true;
		vwChest.OptionsBehavior.Editable = false;
		vwChest.OptionsBehavior.ReadOnly = true;
		vwChest.OptionsFilter.UseNewCustomFilterDialog = true;
		vwChest.OptionsNavigation.UseTabKey = false;
		vwChest.OptionsSelection.MultiSelect = true;
		vwChest.OptionsView.ShowGroupPanel = false;
		colX6.FieldName = "X";
		colX6.Name = "colX6";
		colX6.Visible = true;
		colX6.VisibleIndex = 0;
		colX6.Width = 27;
		colY6.FieldName = "Y";
		colY6.Name = "colY6";
		colY6.Visible = true;
		colY6.VisibleIndex = 1;
		colY6.Width = 27;
		colZ6.FieldName = "Z";
		colZ6.Name = "colZ6";
		colZ6.Visible = true;
		colZ6.VisibleIndex = 2;
		colZ6.Width = 27;
		colLocality6.Caption = "Район";
		colLocality6.FieldName = "Locality";
		colLocality6.Name = "colLocality6";
		colLocality6.Visible = true;
		colLocality6.VisibleIndex = 3;
		colLocality6.Width = 32;
		colObject3.Caption = "Объект";
		colObject3.FieldName = "Object";
		colObject3.Name = "colObject3";
		colObject3.Visible = true;
		colObject3.VisibleIndex = 4;
		colObject3.Width = 156;
		colArt1.Caption = "Артефакт";
		colArt1.FieldName = "Art";
		colArt1.Name = "colArt1";
		colArt1.Visible = true;
		colArt1.VisibleIndex = 7;
		colArt1.Width = 326;
		colClass1.Caption = "Класс";
		colClass1.FieldName = "Class";
		colClass1.Name = "colClass1";
		colClass1.Visible = true;
		colClass1.VisibleIndex = 8;
		colClass1.Width = 91;
		colRelicC6.Caption = "Реликт-С";
		colRelicC6.FieldName = "Relic-C";
		colRelicC6.Name = "colRelicC6";
		colRelicC6.Visible = true;
		colRelicC6.VisibleIndex = 9;
		colRelicC6.Width = 122;
		colGold5.Caption = "Золото";
		colGold5.FieldName = "Gold";
		colGold5.Name = "colGold5";
		colGold5.Visible = true;
		colGold5.VisibleIndex = 5;
		colGold5.Width = 83;
		colHP6.Caption = "Опыт";
		colHP6.FieldName = "HP";
		colHP6.Name = "colHP6";
		colHP6.Visible = true;
		colHP6.VisibleIndex = 6;
		colHP6.Width = 57;
		((Control)tabSpell).Controls.Add((Control)(object)grdSpell);
		((Control)tabSpell).Name = "tabSpell";
		tabSpell.Size = new Size(969, 448);
		((Control)tabSpell).Text = "Заклинания";
		grdSpell.DataSource = rSpellBindingSource;
		((Control)grdSpell).Dock = (DockStyle)5;
		((Control)grdSpell).Location = new Point(0, 0);
		grdSpell.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdSpell.LookAndFeel.UseDefaultLookAndFeel = false;
		grdSpell.MainView = vwSpell;
		grdSpell.MenuManager = barManager1;
		((Control)grdSpell).Name = "grdSpell";
		((Control)grdSpell).Size = new Size(969, 448);
		((Control)grdSpell).TabIndex = 0;
		grdSpell.ToolTipController = toolTipController1;
		grdSpell.ViewCollection.AddRange(new BaseView[1] { vwSpell });
		vwSpell.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwSpell.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwSpell.Appearance.FocusedRow.ForeColor = Color.White;
		vwSpell.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwSpell.Appearance.FocusedRow.Options.UseBackColor = true;
		vwSpell.Appearance.FocusedRow.Options.UseForeColor = true;
		vwSpell.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwSpell.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwSpell.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwSpell.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwSpell.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwSpell.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwSpell.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwSpell.Appearance.SelectedRow.ForeColor = Color.Black;
		vwSpell.Appearance.SelectedRow.Options.UseBackColor = true;
		vwSpell.Appearance.SelectedRow.Options.UseForeColor = true;
		vwSpell.Columns.AddRange(new GridColumn[9] { colX7, colY7, colZ7, colLocality7, colObject4, colSpell2, colGuard4, colHP7, colLevel9 });
		vwSpell.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwSpell.GridControl = grdSpell;
		vwSpell.GroupPanelText = " ";
		vwSpell.Name = "vwSpell";
		vwSpell.OptionsBehavior.AllowIncrementalSearch = true;
		vwSpell.OptionsBehavior.Editable = false;
		vwSpell.OptionsBehavior.ReadOnly = true;
		vwSpell.OptionsFilter.UseNewCustomFilterDialog = true;
		vwSpell.OptionsNavigation.UseTabKey = false;
		vwSpell.OptionsSelection.MultiSelect = true;
		vwSpell.OptionsView.ShowGroupPanel = false;
		colX7.FieldName = "X";
		colX7.Name = "colX7";
		colX7.Visible = true;
		colX7.VisibleIndex = 0;
		colX7.Width = 27;
		colY7.FieldName = "Y";
		colY7.Name = "colY7";
		colY7.Visible = true;
		colY7.VisibleIndex = 1;
		colY7.Width = 27;
		colZ7.FieldName = "Z";
		colZ7.Name = "colZ7";
		colZ7.Visible = true;
		colZ7.VisibleIndex = 2;
		colZ7.Width = 27;
		colLocality7.Caption = "Район";
		colLocality7.FieldName = "Locality";
		colLocality7.Name = "colLocality7";
		colLocality7.Visible = true;
		colLocality7.VisibleIndex = 3;
		colLocality7.Width = 32;
		colObject4.Caption = "Объект";
		colObject4.FieldName = "Object";
		colObject4.Name = "colObject4";
		colObject4.Visible = true;
		colObject4.VisibleIndex = 4;
		colObject4.Width = 182;
		colSpell2.Caption = "Заклинание";
		colSpell2.FieldName = "Spell";
		colSpell2.Name = "colSpell2";
		colSpell2.Visible = true;
		colSpell2.VisibleIndex = 5;
		colSpell2.Width = 200;
		colGuard4.Caption = "Охрана";
		colGuard4.FieldName = "Guard";
		colGuard4.Name = "colGuard4";
		colGuard4.Visible = true;
		colGuard4.VisibleIndex = 7;
		colGuard4.Width = 338;
		colHP7.Caption = "∑ HP";
		colHP7.FieldName = "HP";
		colHP7.Name = "colHP7";
		colHP7.Visible = true;
		colHP7.VisibleIndex = 8;
		colHP7.Width = 46;
		colLevel9.Caption = "Уровень";
		colLevel9.FieldName = "Level";
		colLevel9.Name = "colLevel9";
		colLevel9.Visible = true;
		colLevel9.VisibleIndex = 6;
		colLevel9.Width = 52;
		((Control)tabSkill).Controls.Add((Control)(object)grdSkill);
		((Control)tabSkill).Name = "tabSkill";
		tabSkill.Size = new Size(969, 448);
		((Control)tabSkill).Text = "Навыки";
		grdSkill.DataSource = rSkillBindingSource;
		((Control)grdSkill).Dock = (DockStyle)5;
		((Control)grdSkill).Location = new Point(0, 0);
		grdSkill.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdSkill.LookAndFeel.UseDefaultLookAndFeel = false;
		grdSkill.MainView = vwSkill;
		grdSkill.MenuManager = barManager1;
		((Control)grdSkill).Name = "grdSkill";
		((Control)grdSkill).Size = new Size(969, 448);
		((Control)grdSkill).TabIndex = 0;
		grdSkill.ToolTipController = toolTipController1;
		grdSkill.ViewCollection.AddRange(new BaseView[1] { vwSkill });
		vwSkill.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwSkill.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwSkill.Appearance.FocusedRow.ForeColor = Color.White;
		vwSkill.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwSkill.Appearance.FocusedRow.Options.UseBackColor = true;
		vwSkill.Appearance.FocusedRow.Options.UseForeColor = true;
		vwSkill.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwSkill.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwSkill.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwSkill.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwSkill.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwSkill.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwSkill.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwSkill.Appearance.SelectedRow.ForeColor = Color.Black;
		vwSkill.Appearance.SelectedRow.Options.UseBackColor = true;
		vwSkill.Appearance.SelectedRow.Options.UseForeColor = true;
		vwSkill.Columns.AddRange(new GridColumn[6] { colX8, colY8, colZ8, colLocality8, colObject5, colSkill });
		vwSkill.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwSkill.GridControl = grdSkill;
		vwSkill.GroupPanelText = " ";
		vwSkill.Name = "vwSkill";
		vwSkill.OptionsBehavior.AllowIncrementalSearch = true;
		vwSkill.OptionsBehavior.Editable = false;
		vwSkill.OptionsBehavior.ReadOnly = true;
		vwSkill.OptionsFilter.UseNewCustomFilterDialog = true;
		vwSkill.OptionsNavigation.UseTabKey = false;
		vwSkill.OptionsSelection.MultiSelect = true;
		vwSkill.OptionsView.ShowGroupPanel = false;
		colX8.FieldName = "X";
		colX8.Name = "colX8";
		colX8.Visible = true;
		colX8.VisibleIndex = 0;
		colX8.Width = 27;
		colY8.FieldName = "Y";
		colY8.Name = "colY8";
		colY8.Visible = true;
		colY8.VisibleIndex = 1;
		colY8.Width = 27;
		colZ8.FieldName = "Z";
		colZ8.Name = "colZ8";
		colZ8.Visible = true;
		colZ8.VisibleIndex = 2;
		colZ8.Width = 27;
		colLocality8.Caption = "Район";
		colLocality8.FieldName = "Locality";
		colLocality8.Name = "colLocality8";
		colLocality8.Visible = true;
		colLocality8.VisibleIndex = 3;
		colLocality8.Width = 32;
		colObject5.Caption = "Объект";
		colObject5.FieldName = "Object";
		colObject5.Name = "colObject5";
		colObject5.Visible = true;
		colObject5.VisibleIndex = 4;
		colObject5.Width = 261;
		colSkill.Caption = "Навык";
		colSkill.FieldName = "Skill";
		colSkill.Name = "colSkill";
		colSkill.Visible = true;
		colSkill.VisibleIndex = 5;
		colSkill.Width = 574;
		((Control)tabCamp).Controls.Add((Control)(object)grdCamp);
		((Control)tabCamp).Name = "tabCamp";
		tabCamp.Size = new Size(969, 448);
		((Control)tabCamp).Text = "Лагеря Беженцев";
		grdCamp.DataSource = rCampBindingSource;
		((Control)grdCamp).Dock = (DockStyle)5;
		((Control)grdCamp).Location = new Point(0, 0);
		grdCamp.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdCamp.LookAndFeel.UseDefaultLookAndFeel = false;
		grdCamp.MainView = vwCamp;
		grdCamp.MenuManager = barManager1;
		((Control)grdCamp).Name = "grdCamp";
		((Control)grdCamp).Size = new Size(969, 448);
		((Control)grdCamp).TabIndex = 0;
		grdCamp.ToolTipController = toolTipController1;
		grdCamp.ViewCollection.AddRange(new BaseView[1] { vwCamp });
		vwCamp.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwCamp.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwCamp.Appearance.FocusedRow.ForeColor = Color.White;
		vwCamp.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwCamp.Appearance.FocusedRow.Options.UseBackColor = true;
		vwCamp.Appearance.FocusedRow.Options.UseForeColor = true;
		vwCamp.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwCamp.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwCamp.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwCamp.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwCamp.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwCamp.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwCamp.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwCamp.Appearance.SelectedRow.ForeColor = Color.Black;
		vwCamp.Appearance.SelectedRow.Options.UseBackColor = true;
		vwCamp.Appearance.SelectedRow.Options.UseForeColor = true;
		vwCamp.Columns.AddRange(new GridColumn[8] { colX9, colY9, colZ9, colLocality9, colObject6, colMonster2, colLevel4, colNumber1 });
		vwCamp.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwCamp.GridControl = grdCamp;
		vwCamp.GroupPanelText = " ";
		vwCamp.Name = "vwCamp";
		vwCamp.OptionsBehavior.AllowIncrementalSearch = true;
		vwCamp.OptionsBehavior.Editable = false;
		vwCamp.OptionsBehavior.ReadOnly = true;
		vwCamp.OptionsFilter.UseNewCustomFilterDialog = true;
		vwCamp.OptionsNavigation.UseTabKey = false;
		vwCamp.OptionsSelection.MultiSelect = true;
		vwCamp.OptionsView.ShowGroupPanel = false;
		colX9.FieldName = "X";
		colX9.Name = "colX9";
		colX9.Visible = true;
		colX9.VisibleIndex = 0;
		colX9.Width = 27;
		colY9.FieldName = "Y";
		colY9.Name = "colY9";
		colY9.Visible = true;
		colY9.VisibleIndex = 1;
		colY9.Width = 27;
		colZ9.FieldName = "Z";
		colZ9.Name = "colZ9";
		colZ9.Visible = true;
		colZ9.VisibleIndex = 2;
		colZ9.Width = 27;
		colLocality9.Caption = "Район";
		colLocality9.FieldName = "Locality";
		colLocality9.Name = "colLocality9";
		colLocality9.Visible = true;
		colLocality9.VisibleIndex = 3;
		colLocality9.Width = 32;
		colObject6.Caption = "Объект";
		colObject6.FieldName = "Object";
		colObject6.Name = "colObject6";
		colObject6.Visible = true;
		colObject6.VisibleIndex = 4;
		colObject6.Width = 128;
		colMonster2.Caption = "Монстр";
		colMonster2.FieldName = "Monster";
		colMonster2.Name = "colMonster2";
		colMonster2.Visible = true;
		colMonster2.VisibleIndex = 5;
		colMonster2.Width = 488;
		colLevel4.Caption = "Уровень";
		colLevel4.FieldName = "Level";
		colLevel4.Name = "colLevel4";
		colLevel4.Visible = true;
		colLevel4.VisibleIndex = 7;
		colLevel4.Width = 109;
		colNumber1.Caption = "Количество";
		colNumber1.FieldName = "Number";
		colNumber1.Name = "colNumber1";
		colNumber1.Visible = true;
		colNumber1.VisibleIndex = 6;
		colNumber1.Width = 110;
		((Control)tabMarket).Controls.Add((Control)(object)grdMarket);
		((Control)tabMarket).Name = "tabMarket";
		tabMarket.Size = new Size(969, 448);
		((Control)tabMarket).Text = "Рынки";
		grdMarket.DataSource = rMarketBindingSource;
		((Control)grdMarket).Dock = (DockStyle)5;
		((Control)grdMarket).Location = new Point(0, 0);
		grdMarket.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdMarket.LookAndFeel.UseDefaultLookAndFeel = false;
		grdMarket.MainView = vwMarket;
		grdMarket.MenuManager = barManager1;
		((Control)grdMarket).Name = "grdMarket";
		((Control)grdMarket).Size = new Size(969, 448);
		((Control)grdMarket).TabIndex = 0;
		grdMarket.ToolTipController = toolTipController1;
		grdMarket.ViewCollection.AddRange(new BaseView[1] { vwMarket });
		vwMarket.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwMarket.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwMarket.Appearance.FocusedRow.ForeColor = Color.White;
		vwMarket.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwMarket.Appearance.FocusedRow.Options.UseBackColor = true;
		vwMarket.Appearance.FocusedRow.Options.UseForeColor = true;
		vwMarket.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwMarket.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwMarket.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwMarket.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwMarket.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwMarket.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwMarket.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwMarket.Appearance.SelectedRow.ForeColor = Color.Black;
		vwMarket.Appearance.SelectedRow.Options.UseBackColor = true;
		vwMarket.Appearance.SelectedRow.Options.UseForeColor = true;
		vwMarket.Columns.AddRange(new GridColumn[10] { colX12, colY12, colZ12, colLocality11, colObject8, colSlot2, colArt2, colClass3, gridColumn2, colCost });
		vwMarket.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwMarket.GridControl = grdMarket;
		vwMarket.GroupPanelText = " ";
		vwMarket.Name = "vwMarket";
		vwMarket.OptionsBehavior.AllowIncrementalSearch = true;
		vwMarket.OptionsBehavior.Editable = false;
		vwMarket.OptionsBehavior.ReadOnly = true;
		vwMarket.OptionsFilter.UseNewCustomFilterDialog = true;
		vwMarket.OptionsNavigation.UseTabKey = false;
		vwMarket.OptionsSelection.MultiSelect = true;
		vwMarket.OptionsView.ShowGroupPanel = false;
		colX12.FieldName = "X";
		colX12.Name = "colX12";
		colX12.Visible = true;
		colX12.VisibleIndex = 0;
		colX12.Width = 27;
		colY12.FieldName = "Y";
		colY12.Name = "colY12";
		colY12.Visible = true;
		colY12.VisibleIndex = 1;
		colY12.Width = 27;
		colZ12.FieldName = "Z";
		colZ12.Name = "colZ12";
		colZ12.Visible = true;
		colZ12.VisibleIndex = 2;
		colZ12.Width = 27;
		colLocality11.Caption = "Район";
		colLocality11.FieldName = "Locality";
		colLocality11.Name = "colLocality11";
		colLocality11.Visible = true;
		colLocality11.VisibleIndex = 3;
		colLocality11.Width = 32;
		colObject8.Caption = "Объект";
		colObject8.FieldName = "Object";
		colObject8.Name = "colObject8";
		colObject8.Visible = true;
		colObject8.VisibleIndex = 4;
		colObject8.Width = 136;
		colSlot2.Caption = "Слот";
		colSlot2.FieldName = "Slot";
		colSlot2.Name = "colSlot2";
		colSlot2.Visible = true;
		colSlot2.VisibleIndex = 5;
		colSlot2.Width = 36;
		colArt2.Caption = "Артефакт";
		colArt2.FieldName = "Art";
		colArt2.Name = "colArt2";
		colArt2.Visible = true;
		colArt2.VisibleIndex = 6;
		colArt2.Width = 277;
		colClass3.Caption = "Класс";
		colClass3.FieldName = "Class";
		colClass3.Name = "colClass3";
		colClass3.Visible = true;
		colClass3.VisibleIndex = 7;
		colClass3.Width = 108;
		gridColumn2.Caption = "Реликт-С";
		gridColumn2.FieldName = "Relic-C";
		gridColumn2.Name = "gridColumn2";
		gridColumn2.Visible = true;
		gridColumn2.VisibleIndex = 8;
		gridColumn2.Width = 136;
		colCost.Caption = "Цена на Черном Рынке";
		colCost.FieldName = "Cost";
		colCost.Name = "colCost";
		colCost.Visible = true;
		colCost.VisibleIndex = 9;
		colCost.Width = 142;
		((Control)tabSeerHut).Controls.Add((Control)(object)grdSeerHut);
		((Control)tabSeerHut).Name = "tabSeerHut";
		tabSeerHut.Size = new Size(969, 448);
		((Control)tabSeerHut).Text = "Провидцы";
		grdSeerHut.DataSource = rSeerHutBindingSource;
		((Control)grdSeerHut).Dock = (DockStyle)5;
		((Control)grdSeerHut).Location = new Point(0, 0);
		grdSeerHut.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdSeerHut.LookAndFeel.UseDefaultLookAndFeel = false;
		grdSeerHut.MainView = vwSeerHut;
		grdSeerHut.MenuManager = barManager1;
		((Control)grdSeerHut).Name = "grdSeerHut";
		((Control)grdSeerHut).Size = new Size(969, 448);
		((Control)grdSeerHut).TabIndex = 0;
		grdSeerHut.ToolTipController = toolTipController1;
		grdSeerHut.ViewCollection.AddRange(new BaseView[1] { vwSeerHut });
		vwSeerHut.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwSeerHut.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwSeerHut.Appearance.FocusedRow.ForeColor = Color.White;
		vwSeerHut.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwSeerHut.Appearance.FocusedRow.Options.UseBackColor = true;
		vwSeerHut.Appearance.FocusedRow.Options.UseForeColor = true;
		vwSeerHut.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwSeerHut.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwSeerHut.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwSeerHut.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwSeerHut.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwSeerHut.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwSeerHut.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwSeerHut.Appearance.SelectedRow.ForeColor = Color.Black;
		vwSeerHut.Appearance.SelectedRow.Options.UseBackColor = true;
		vwSeerHut.Appearance.SelectedRow.Options.UseForeColor = true;
		vwSeerHut.Columns.AddRange(new GridColumn[7] { colX13, colY13, colZ13, colLocality12, colMission1, colReward, colDeadline });
		vwSeerHut.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwSeerHut.GridControl = grdSeerHut;
		vwSeerHut.GroupPanelText = " ";
		vwSeerHut.Name = "vwSeerHut";
		vwSeerHut.OptionsBehavior.AllowIncrementalSearch = true;
		vwSeerHut.OptionsBehavior.Editable = false;
		vwSeerHut.OptionsBehavior.ReadOnly = true;
		vwSeerHut.OptionsFilter.UseNewCustomFilterDialog = true;
		vwSeerHut.OptionsNavigation.UseTabKey = false;
		vwSeerHut.OptionsSelection.MultiSelect = true;
		vwSeerHut.OptionsView.ShowGroupPanel = false;
		colX13.FieldName = "X";
		colX13.Name = "colX13";
		colX13.Visible = true;
		colX13.VisibleIndex = 0;
		colX13.Width = 27;
		colY13.FieldName = "Y";
		colY13.Name = "colY13";
		colY13.Visible = true;
		colY13.VisibleIndex = 1;
		colY13.Width = 27;
		colZ13.FieldName = "Z";
		colZ13.Name = "colZ13";
		colZ13.Visible = true;
		colZ13.VisibleIndex = 2;
		colZ13.Width = 27;
		colLocality12.Caption = "Район";
		colLocality12.FieldName = "Locality";
		colLocality12.Name = "colLocality12";
		colLocality12.Visible = true;
		colLocality12.VisibleIndex = 3;
		colLocality12.Width = 32;
		colMission1.Caption = "Задание";
		colMission1.FieldName = "Mission";
		colMission1.Name = "colMission1";
		colMission1.Visible = true;
		colMission1.VisibleIndex = 4;
		colMission1.Width = 372;
		colReward.Caption = "Награда";
		colReward.FieldName = "Reward";
		colReward.Name = "colReward";
		colReward.Visible = true;
		colReward.VisibleIndex = 5;
		colReward.Width = 372;
		colDeadline.Caption = "Срок";
		colDeadline.FieldName = "Deadline";
		colDeadline.Name = "colDeadline";
		colDeadline.Visible = true;
		colDeadline.VisibleIndex = 6;
		colDeadline.Width = 91;
		((Control)tabPassGuard).Controls.Add((Control)(object)grdPassGuard);
		((Control)tabPassGuard).Name = "tabPassGuard";
		tabPassGuard.Size = new Size(969, 448);
		((Control)tabPassGuard).Text = "Стражи Прохода";
		grdPassGuard.DataSource = rPassGuardBindingSource;
		((Control)grdPassGuard).Dock = (DockStyle)5;
		((Control)grdPassGuard).Location = new Point(0, 0);
		grdPassGuard.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdPassGuard.LookAndFeel.UseDefaultLookAndFeel = false;
		grdPassGuard.MainView = vwPassGuard;
		grdPassGuard.MenuManager = barManager1;
		((Control)grdPassGuard).Name = "grdPassGuard";
		((Control)grdPassGuard).Size = new Size(969, 448);
		((Control)grdPassGuard).TabIndex = 0;
		grdPassGuard.ToolTipController = toolTipController1;
		grdPassGuard.ViewCollection.AddRange(new BaseView[1] { vwPassGuard });
		vwPassGuard.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwPassGuard.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwPassGuard.Appearance.FocusedRow.ForeColor = Color.White;
		vwPassGuard.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwPassGuard.Appearance.FocusedRow.Options.UseBackColor = true;
		vwPassGuard.Appearance.FocusedRow.Options.UseForeColor = true;
		vwPassGuard.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwPassGuard.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwPassGuard.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwPassGuard.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwPassGuard.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwPassGuard.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwPassGuard.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwPassGuard.Appearance.SelectedRow.ForeColor = Color.Black;
		vwPassGuard.Appearance.SelectedRow.Options.UseBackColor = true;
		vwPassGuard.Appearance.SelectedRow.Options.UseForeColor = true;
		vwPassGuard.Columns.AddRange(new GridColumn[6] { colX14, colY14, colZ14, colLocality13, colMission2, colDeadline1 });
		vwPassGuard.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwPassGuard.GridControl = grdPassGuard;
		vwPassGuard.GroupPanelText = " ";
		vwPassGuard.Name = "vwPassGuard";
		vwPassGuard.OptionsBehavior.AllowIncrementalSearch = true;
		vwPassGuard.OptionsBehavior.Editable = false;
		vwPassGuard.OptionsBehavior.ReadOnly = true;
		vwPassGuard.OptionsFilter.UseNewCustomFilterDialog = true;
		vwPassGuard.OptionsNavigation.UseTabKey = false;
		vwPassGuard.OptionsSelection.MultiSelect = true;
		vwPassGuard.OptionsView.ShowGroupPanel = false;
		colX14.FieldName = "X";
		colX14.Name = "colX14";
		colX14.Visible = true;
		colX14.VisibleIndex = 0;
		colX14.Width = 27;
		colY14.FieldName = "Y";
		colY14.Name = "colY14";
		colY14.Visible = true;
		colY14.VisibleIndex = 1;
		colY14.Width = 27;
		colZ14.FieldName = "Z";
		colZ14.Name = "colZ14";
		colZ14.Visible = true;
		colZ14.VisibleIndex = 2;
		colZ14.Width = 27;
		colLocality13.Caption = "Район";
		colLocality13.FieldName = "Locality";
		colLocality13.Name = "colLocality13";
		colLocality13.Visible = true;
		colLocality13.VisibleIndex = 3;
		colLocality13.Width = 32;
		colMission2.Caption = "Задание";
		colMission2.FieldName = "Mission";
		colMission2.Name = "colMission2";
		colMission2.Visible = true;
		colMission2.VisibleIndex = 4;
		colMission2.Width = 744;
		colDeadline1.Caption = "Срок";
		colDeadline1.FieldName = "Deadline";
		colDeadline1.Name = "colDeadline1";
		colDeadline1.Visible = true;
		colDeadline1.VisibleIndex = 5;
		colDeadline1.Width = 91;
		((Control)tabGarrison).Controls.Add((Control)(object)grdGarrison);
		((Control)tabGarrison).Name = "tabGarrison";
		tabGarrison.Size = new Size(969, 448);
		((Control)tabGarrison).Text = "Гарнизоны";
		grdGarrison.DataSource = rGarrisonBindingSource;
		((Control)grdGarrison).Dock = (DockStyle)5;
		((Control)grdGarrison).Location = new Point(0, 0);
		grdGarrison.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdGarrison.LookAndFeel.UseDefaultLookAndFeel = false;
		grdGarrison.MainView = vwGarrison;
		grdGarrison.MenuManager = barManager1;
		((Control)grdGarrison).Name = "grdGarrison";
		((Control)grdGarrison).Size = new Size(969, 448);
		((Control)grdGarrison).TabIndex = 0;
		grdGarrison.ToolTipController = toolTipController1;
		grdGarrison.ViewCollection.AddRange(new BaseView[1] { vwGarrison });
		vwGarrison.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwGarrison.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwGarrison.Appearance.FocusedRow.ForeColor = Color.White;
		vwGarrison.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwGarrison.Appearance.FocusedRow.Options.UseBackColor = true;
		vwGarrison.Appearance.FocusedRow.Options.UseForeColor = true;
		vwGarrison.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwGarrison.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwGarrison.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwGarrison.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwGarrison.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwGarrison.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwGarrison.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwGarrison.Appearance.SelectedRow.ForeColor = Color.Black;
		vwGarrison.Appearance.SelectedRow.Options.UseBackColor = true;
		vwGarrison.Appearance.SelectedRow.Options.UseForeColor = true;
		vwGarrison.Columns.AddRange(new GridColumn[9] { colX15, colY15, colZ15, colLocality14, colAntiMagic, colGuard6, colColor2, colCanTake, colHP8 });
		vwGarrison.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwGarrison.GridControl = grdGarrison;
		vwGarrison.GroupPanelText = " ";
		vwGarrison.Name = "vwGarrison";
		vwGarrison.OptionsBehavior.AllowIncrementalSearch = true;
		vwGarrison.OptionsBehavior.Editable = false;
		vwGarrison.OptionsBehavior.ReadOnly = true;
		vwGarrison.OptionsFilter.UseNewCustomFilterDialog = true;
		vwGarrison.OptionsNavigation.UseTabKey = false;
		vwGarrison.OptionsSelection.MultiSelect = true;
		vwGarrison.OptionsView.ShowGroupPanel = false;
		colX15.FieldName = "X";
		colX15.Name = "colX15";
		colX15.Visible = true;
		colX15.VisibleIndex = 0;
		colX15.Width = 27;
		colY15.FieldName = "Y";
		colY15.Name = "colY15";
		colY15.Visible = true;
		colY15.VisibleIndex = 1;
		colY15.Width = 27;
		colZ15.FieldName = "Z";
		colZ15.Name = "colZ15";
		colZ15.Visible = true;
		colZ15.VisibleIndex = 2;
		colZ15.Width = 27;
		colLocality14.Caption = "Район";
		colLocality14.FieldName = "Locality";
		colLocality14.Name = "colLocality14";
		colLocality14.Visible = true;
		colLocality14.VisibleIndex = 3;
		colLocality14.Width = 32;
		colAntiMagic.Caption = "Антимагический";
		colAntiMagic.FieldName = "AntiMagic";
		colAntiMagic.Name = "colAntiMagic";
		colAntiMagic.Visible = true;
		colAntiMagic.VisibleIndex = 4;
		colAntiMagic.Width = 60;
		colGuard6.Caption = "Охрана";
		colGuard6.FieldName = "Guard";
		colGuard6.Name = "colGuard6";
		colGuard6.Visible = true;
		colGuard6.VisibleIndex = 5;
		colGuard6.Width = 511;
		colColor2.Caption = "Флаг";
		colColor2.FieldName = "Color";
		colColor2.Name = "colColor2";
		colColor2.Visible = true;
		colColor2.VisibleIndex = 7;
		colColor2.Width = 105;
		colCanTake.Caption = "Можно забрать";
		colCanTake.FieldName = "CanTake";
		colCanTake.Name = "colCanTake";
		colCanTake.Visible = true;
		colCanTake.VisibleIndex = 8;
		colCanTake.Width = 113;
		colHP8.Caption = "∑ HP";
		colHP8.FieldName = "HP";
		colHP8.Name = "colHP8";
		colHP8.Visible = true;
		colHP8.VisibleIndex = 6;
		colHP8.Width = 46;
		((Control)tabPrison).Controls.Add((Control)(object)grdPrison);
		((Control)tabPrison).Name = "tabPrison";
		tabPrison.Size = new Size(969, 448);
		((Control)tabPrison).Text = "Тюрьмы";
		grdPrison.DataSource = rPrisonBindingSource;
		((Control)grdPrison).Dock = (DockStyle)5;
		((Control)grdPrison).Location = new Point(0, 0);
		grdPrison.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdPrison.LookAndFeel.UseDefaultLookAndFeel = false;
		grdPrison.MainView = vwPrison;
		grdPrison.MenuManager = barManager1;
		((Control)grdPrison).Name = "grdPrison";
		((Control)grdPrison).Size = new Size(969, 448);
		((Control)grdPrison).TabIndex = 0;
		grdPrison.ToolTipController = toolTipController1;
		grdPrison.ViewCollection.AddRange(new BaseView[1] { vwPrison });
		vwPrison.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwPrison.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwPrison.Appearance.FocusedRow.ForeColor = Color.White;
		vwPrison.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwPrison.Appearance.FocusedRow.Options.UseBackColor = true;
		vwPrison.Appearance.FocusedRow.Options.UseForeColor = true;
		vwPrison.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwPrison.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwPrison.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwPrison.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwPrison.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwPrison.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwPrison.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwPrison.Appearance.SelectedRow.ForeColor = Color.Black;
		vwPrison.Appearance.SelectedRow.Options.UseBackColor = true;
		vwPrison.Appearance.SelectedRow.Options.UseForeColor = true;
		vwPrison.Columns.AddRange(new GridColumn[16]
		{
			colX16, colY16, colZ16, colLocality15, colHero1, colLevel2, colPrimarySkill2, colSecondarySkill2, colArt3, colSpell4,
			colMonster4, colMachine, colBook, colMP, colExperience2, colHP11
		});
		vwPrison.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwPrison.GridControl = grdPrison;
		vwPrison.GroupPanelText = " ";
		vwPrison.Name = "vwPrison";
		vwPrison.OptionsBehavior.AllowIncrementalSearch = true;
		vwPrison.OptionsBehavior.Editable = false;
		vwPrison.OptionsBehavior.ReadOnly = true;
		vwPrison.OptionsFilter.UseNewCustomFilterDialog = true;
		vwPrison.OptionsNavigation.UseTabKey = false;
		vwPrison.OptionsSelection.MultiSelect = true;
		vwPrison.OptionsView.ShowGroupPanel = false;
		colX16.FieldName = "X";
		colX16.Name = "colX16";
		colX16.Visible = true;
		colX16.VisibleIndex = 0;
		colX16.Width = 27;
		colY16.FieldName = "Y";
		colY16.Name = "colY16";
		colY16.Visible = true;
		colY16.VisibleIndex = 1;
		colY16.Width = 27;
		colZ16.FieldName = "Z";
		colZ16.Name = "colZ16";
		colZ16.Visible = true;
		colZ16.VisibleIndex = 2;
		colZ16.Width = 27;
		colLocality15.Caption = "Район";
		colLocality15.FieldName = "Locality";
		colLocality15.Name = "colLocality15";
		colLocality15.Visible = true;
		colLocality15.VisibleIndex = 3;
		colLocality15.Width = 32;
		colHero1.Caption = "Герой";
		colHero1.FieldName = "Hero";
		colHero1.Name = "colHero1";
		colHero1.Visible = true;
		colHero1.VisibleIndex = 4;
		colHero1.Width = 71;
		colLevel2.Caption = "Уровень";
		colLevel2.FieldName = "Level";
		colLevel2.Name = "colLevel2";
		colLevel2.Visible = true;
		colLevel2.VisibleIndex = 5;
		colLevel2.Width = 33;
		colPrimarySkill2.Caption = "Первичный навык";
		colPrimarySkill2.FieldName = "PrimarySkill";
		colPrimarySkill2.Name = "colPrimarySkill2";
		colPrimarySkill2.Visible = true;
		colPrimarySkill2.VisibleIndex = 7;
		colPrimarySkill2.Width = 88;
		colSecondarySkill2.Caption = "Вторичный навык";
		colSecondarySkill2.FieldName = "SecondarySkill";
		colSecondarySkill2.Name = "colSecondarySkill2";
		colSecondarySkill2.Visible = true;
		colSecondarySkill2.VisibleIndex = 8;
		colSecondarySkill2.Width = 103;
		colArt3.Caption = "Артефакт";
		colArt3.FieldName = "Art";
		colArt3.Name = "colArt3";
		colArt3.Visible = true;
		colArt3.VisibleIndex = 9;
		colArt3.Width = 103;
		colSpell4.Caption = "Заклинание";
		colSpell4.FieldName = "Spell";
		colSpell4.Name = "colSpell4";
		colSpell4.Visible = true;
		colSpell4.VisibleIndex = 10;
		colSpell4.Width = 103;
		colMonster4.Caption = "Монстр";
		colMonster4.FieldName = "Monster";
		colMonster4.Name = "colMonster4";
		colMonster4.Visible = true;
		colMonster4.VisibleIndex = 11;
		colMonster4.Width = 103;
		colMachine.Caption = "Машина";
		colMachine.FieldName = "Machine";
		colMachine.Name = "colMachine";
		colMachine.Visible = true;
		colMachine.VisibleIndex = 13;
		colMachine.Width = 55;
		colBook.Caption = "Книга заклинаний";
		colBook.FieldName = "Book";
		colBook.Name = "colBook";
		colBook.Visible = true;
		colBook.VisibleIndex = 14;
		colBook.Width = 43;
		colMP.FieldName = "MP";
		colMP.Name = "colMP";
		colMP.Visible = true;
		colMP.VisibleIndex = 15;
		colMP.Width = 48;
		colExperience2.Caption = "Опыт";
		colExperience2.FieldName = "Experience";
		colExperience2.Name = "colExperience2";
		colExperience2.Visible = true;
		colExperience2.VisibleIndex = 6;
		colExperience2.Width = 39;
		colHP11.Caption = "∑ HP";
		colHP11.FieldName = "HP";
		colHP11.Name = "colHP11";
		colHP11.Visible = true;
		colHP11.VisibleIndex = 12;
		colHP11.Width = 46;
		((Control)tabObject).Controls.Add((Control)(object)grdObject);
		((Control)tabObject).Name = "tabObject";
		tabObject.Size = new Size(969, 448);
		((Control)tabObject).Text = "Объекты";
		grdObject.DataSource = rObjectBindingSource;
		((Control)grdObject).Dock = (DockStyle)5;
		((Control)grdObject).Location = new Point(0, 0);
		grdObject.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdObject.LookAndFeel.UseDefaultLookAndFeel = false;
		grdObject.MainView = vwObject;
		grdObject.MenuManager = barManager1;
		((Control)grdObject).Name = "grdObject";
		((Control)grdObject).Size = new Size(969, 448);
		((Control)grdObject).TabIndex = 0;
		grdObject.ToolTipController = toolTipController1;
		grdObject.ViewCollection.AddRange(new BaseView[1] { vwObject });
		vwObject.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwObject.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwObject.Appearance.FocusedRow.ForeColor = Color.White;
		vwObject.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwObject.Appearance.FocusedRow.Options.UseBackColor = true;
		vwObject.Appearance.FocusedRow.Options.UseForeColor = true;
		vwObject.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwObject.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwObject.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwObject.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwObject.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwObject.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwObject.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwObject.Appearance.SelectedRow.ForeColor = Color.Black;
		vwObject.Appearance.SelectedRow.Options.UseBackColor = true;
		vwObject.Appearance.SelectedRow.Options.UseForeColor = true;
		vwObject.Columns.AddRange(new GridColumn[6] { colX17, colY17, colZ17, colLocality16, colObject9, colPayment });
		vwObject.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwObject.GridControl = grdObject;
		vwObject.GroupPanelText = " ";
		vwObject.Name = "vwObject";
		vwObject.OptionsBehavior.AllowIncrementalSearch = true;
		vwObject.OptionsBehavior.Editable = false;
		vwObject.OptionsBehavior.ReadOnly = true;
		vwObject.OptionsFilter.UseNewCustomFilterDialog = true;
		vwObject.OptionsNavigation.UseTabKey = false;
		vwObject.OptionsSelection.MultiSelect = true;
		vwObject.OptionsView.ShowGroupPanel = false;
		colX17.FieldName = "X";
		colX17.Name = "colX17";
		colX17.Visible = true;
		colX17.VisibleIndex = 0;
		colX17.Width = 27;
		colY17.FieldName = "Y";
		colY17.Name = "colY17";
		colY17.Visible = true;
		colY17.VisibleIndex = 1;
		colY17.Width = 27;
		colZ17.FieldName = "Z";
		colZ17.Name = "colZ17";
		colZ17.Visible = true;
		colZ17.VisibleIndex = 2;
		colZ17.Width = 27;
		colLocality16.Caption = "Район";
		colLocality16.FieldName = "Locality";
		colLocality16.Name = "colLocality16";
		colLocality16.Visible = true;
		colLocality16.VisibleIndex = 3;
		colLocality16.Width = 32;
		colObject9.Caption = "Объект";
		colObject9.FieldName = "Object";
		colObject9.Name = "colObject9";
		colObject9.Visible = true;
		colObject9.VisibleIndex = 4;
		colObject9.Width = 744;
		colPayment.Caption = "Плата";
		colPayment.FieldName = "Payment";
		colPayment.Name = "colPayment";
		colPayment.Visible = true;
		colPayment.VisibleIndex = 5;
		colPayment.Width = 91;
		((Control)tabTopology).Controls.Add((Control)(object)grdTopology);
		((Control)tabTopology).Name = "tabTopology";
		tabTopology.Size = new Size(969, 448);
		((Control)tabTopology).Text = "Топология";
		grdTopology.DataSource = rTopologyBindingSource;
		((Control)grdTopology).Dock = (DockStyle)5;
		((Control)grdTopology).Location = new Point(0, 0);
		grdTopology.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdTopology.LookAndFeel.UseDefaultLookAndFeel = false;
		grdTopology.MainView = vwTopology;
		grdTopology.MenuManager = barManager1;
		((Control)grdTopology).Name = "grdTopology";
		((Control)grdTopology).Size = new Size(969, 448);
		((Control)grdTopology).TabIndex = 0;
		grdTopology.ViewCollection.AddRange(new BaseView[1] { vwTopology });
		rTopologyBindingSource.DataMember = "R_Topology";
		rTopologyBindingSource.DataSource = dsResult;
		vwTopology.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwTopology.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwTopology.Appearance.FocusedRow.ForeColor = Color.White;
		vwTopology.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwTopology.Appearance.FocusedRow.Options.UseBackColor = true;
		vwTopology.Appearance.FocusedRow.Options.UseForeColor = true;
		vwTopology.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwTopology.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwTopology.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwTopology.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwTopology.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwTopology.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwTopology.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwTopology.Appearance.SelectedRow.ForeColor = Color.Black;
		vwTopology.Appearance.SelectedRow.Options.UseBackColor = true;
		vwTopology.Appearance.SelectedRow.Options.UseForeColor = true;
		vwTopology.Columns.AddRange(new GridColumn[8] { colX18, colY18, colZ18, colLocality17, colObject10, colType1, colColor4, colPair });
		vwTopology.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwTopology.GridControl = grdTopology;
		vwTopology.Name = "vwTopology";
		vwTopology.OptionsBehavior.AllowIncrementalSearch = true;
		vwTopology.OptionsBehavior.Editable = false;
		vwTopology.OptionsBehavior.ReadOnly = true;
		vwTopology.OptionsFilter.UseNewCustomFilterDialog = true;
		vwTopology.OptionsNavigation.UseTabKey = false;
		vwTopology.OptionsSelection.MultiSelect = true;
		vwTopology.OptionsView.ShowGroupPanel = false;
		colX18.FieldName = "X";
		colX18.Name = "colX18";
		colX18.Visible = true;
		colX18.VisibleIndex = 0;
		colX18.Width = 27;
		colY18.FieldName = "Y";
		colY18.Name = "colY18";
		colY18.Visible = true;
		colY18.VisibleIndex = 1;
		colY18.Width = 27;
		colZ18.FieldName = "Z";
		colZ18.Name = "colZ18";
		colZ18.Visible = true;
		colZ18.VisibleIndex = 2;
		colZ18.Width = 27;
		colLocality17.Caption = "Район";
		colLocality17.FieldName = "Locality";
		colLocality17.Name = "colLocality17";
		colLocality17.Visible = true;
		colLocality17.VisibleIndex = 3;
		colLocality17.Width = 32;
		colObject10.Caption = "Объект";
		colObject10.FieldName = "Object";
		colObject10.Name = "colObject10";
		colObject10.Visible = true;
		colObject10.VisibleIndex = 4;
		colObject10.Width = 491;
		colType1.Caption = "Тип";
		colType1.FieldName = "Type";
		colType1.Name = "colType1";
		colType1.Visible = true;
		colType1.VisibleIndex = 5;
		colType1.Width = 132;
		colColor4.Caption = "Цвет";
		colColor4.FieldName = "Color";
		colColor4.Name = "colColor4";
		colColor4.Visible = true;
		colColor4.VisibleIndex = 6;
		colColor4.Width = 131;
		colPair.Caption = "Пара";
		colPair.FieldName = "Pair";
		colPair.Name = "colPair";
		colPair.Visible = true;
		colPair.VisibleIndex = 7;
		colPair.Width = 81;
		((Control)tabAllTimer).Controls.Add((Control)(object)grdAllTimer);
		((Control)tabAllTimer).Name = "tabAllTimer";
		tabAllTimer.Size = new Size(969, 448);
		((Control)tabAllTimer).Text = "События-Таймеры";
		grdAllTimer.DataSource = rAllTimerBindingSource;
		((Control)grdAllTimer).Dock = (DockStyle)5;
		((Control)grdAllTimer).Location = new Point(0, 0);
		grdAllTimer.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdAllTimer.LookAndFeel.UseDefaultLookAndFeel = false;
		grdAllTimer.MainView = vwAllTimer;
		grdAllTimer.MenuManager = barManager1;
		((Control)grdAllTimer).Name = "grdAllTimer";
		((Control)grdAllTimer).Size = new Size(969, 448);
		((Control)grdAllTimer).TabIndex = 0;
		grdAllTimer.ToolTipController = toolTipController1;
		grdAllTimer.ViewCollection.AddRange(new BaseView[1] { vwAllTimer });
		rAllTimerBindingSource.DataMember = "R_AllTimer";
		rAllTimerBindingSource.DataSource = dsResult;
		vwAllTimer.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwAllTimer.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwAllTimer.Appearance.FocusedRow.ForeColor = Color.White;
		vwAllTimer.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwAllTimer.Appearance.FocusedRow.Options.UseBackColor = true;
		vwAllTimer.Appearance.FocusedRow.Options.UseForeColor = true;
		vwAllTimer.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwAllTimer.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwAllTimer.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwAllTimer.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwAllTimer.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwAllTimer.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwAllTimer.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwAllTimer.Appearance.SelectedRow.ForeColor = Color.Black;
		vwAllTimer.Appearance.SelectedRow.Options.UseBackColor = true;
		vwAllTimer.Appearance.SelectedRow.Options.UseForeColor = true;
		vwAllTimer.Columns.AddRange(new GridColumn[12]
		{
			colObject12, colDay, colRepeat, colTown, colType2, colPlace3, colResource5, colBuilding, colMonster6, colGold13,
			colApply, colColor13
		});
		vwAllTimer.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwAllTimer.GridControl = grdAllTimer;
		vwAllTimer.Name = "vwAllTimer";
		vwAllTimer.OptionsBehavior.AllowIncrementalSearch = true;
		vwAllTimer.OptionsBehavior.Editable = false;
		vwAllTimer.OptionsBehavior.ReadOnly = true;
		vwAllTimer.OptionsFilter.UseNewCustomFilterDialog = true;
		vwAllTimer.OptionsNavigation.UseTabKey = false;
		vwAllTimer.OptionsSelection.MultiSelect = true;
		vwAllTimer.OptionsView.ShowGroupPanel = false;
		colObject12.Caption = "Объект";
		colObject12.FieldName = "Object";
		colObject12.Name = "colObject12";
		colObject12.Visible = true;
		colObject12.VisibleIndex = 0;
		colObject12.Width = 60;
		colDay.Caption = "День";
		colDay.FieldName = "Day";
		colDay.Name = "colDay";
		colDay.Visible = true;
		colDay.VisibleIndex = 1;
		colDay.Width = 42;
		colRepeat.Caption = "Повтор";
		colRepeat.FieldName = "Repeat";
		colRepeat.Name = "colRepeat";
		colRepeat.Visible = true;
		colRepeat.VisibleIndex = 2;
		colRepeat.Width = 47;
		colTown.Caption = "Город";
		colTown.FieldName = "Town";
		colTown.Name = "colTown";
		colTown.Visible = true;
		colTown.VisibleIndex = 3;
		colTown.Width = 90;
		colType2.Caption = "Тип";
		colType2.FieldName = "Type";
		colType2.Name = "colType2";
		colType2.Visible = true;
		colType2.VisibleIndex = 4;
		colType2.Width = 90;
		colPlace3.Caption = "Местоположение";
		colPlace3.FieldName = "Place";
		colPlace3.Name = "colPlace3";
		colPlace3.Visible = true;
		colPlace3.VisibleIndex = 5;
		colPlace3.Width = 58;
		colResource5.Caption = "Ресурс";
		colResource5.FieldName = "Resource";
		colResource5.Name = "colResource5";
		colResource5.Visible = true;
		colResource5.VisibleIndex = 9;
		colResource5.Width = 113;
		colBuilding.Caption = "Постройки";
		colBuilding.FieldName = "Building";
		colBuilding.Name = "colBuilding";
		colBuilding.Visible = true;
		colBuilding.VisibleIndex = 10;
		colBuilding.Width = 113;
		colMonster6.Caption = "Монстр";
		colMonster6.FieldName = "Monster";
		colMonster6.Name = "colMonster6";
		colMonster6.Visible = true;
		colMonster6.VisibleIndex = 11;
		colMonster6.Width = 113;
		colGold13.Caption = "Золото";
		colGold13.FieldName = "Gold";
		colGold13.Name = "colGold13";
		colGold13.Visible = true;
		colGold13.VisibleIndex = 8;
		colGold13.Width = 47;
		colApply.Caption = "Доступно";
		colApply.FieldName = "Apply";
		colApply.Name = "colApply";
		colApply.Visible = true;
		colApply.VisibleIndex = 7;
		colApply.Width = 115;
		colColor13.Caption = "Флаг";
		colColor13.FieldName = "Color";
		colColor13.Name = "colColor13";
		colColor13.Visible = true;
		colColor13.VisibleIndex = 6;
		colColor13.Width = 60;
		((Control)tabHero).Controls.Add((Control)(object)grdHero);
		((Control)tabHero).Name = "tabHero";
		tabHero.Size = new Size(969, 448);
		((Control)tabHero).Text = "Герои";
		grdHero.DataSource = rHeroesBindingSource;
		((Control)grdHero).Dock = (DockStyle)5;
		((Control)grdHero).Location = new Point(0, 0);
		grdHero.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdHero.LookAndFeel.UseDefaultLookAndFeel = false;
		grdHero.MainView = vwHero;
		grdHero.MenuManager = barManager1;
		((Control)grdHero).Name = "grdHero";
		((Control)grdHero).Size = new Size(969, 448);
		((Control)grdHero).TabIndex = 0;
		grdHero.ToolTipController = toolTipController1;
		grdHero.ViewCollection.AddRange(new BaseView[1] { vwHero });
		vwHero.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwHero.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwHero.Appearance.FocusedRow.ForeColor = Color.White;
		vwHero.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwHero.Appearance.FocusedRow.Options.UseBackColor = true;
		vwHero.Appearance.FocusedRow.Options.UseForeColor = true;
		vwHero.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwHero.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwHero.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwHero.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwHero.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwHero.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwHero.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwHero.Appearance.SelectedRow.ForeColor = Color.Black;
		vwHero.Appearance.SelectedRow.Options.UseBackColor = true;
		vwHero.Appearance.SelectedRow.Options.UseForeColor = true;
		vwHero.Columns.AddRange(new GridColumn[18]
		{
			colID, colHero2, colPlace1, colColor3, colLevel3, colPrimarySkill3, colSecondarySkill3, colArt4, colSpell5, colMonster5,
			colMachine1, colBook1, colMP1, colExperience1, colHP10, colHire, colIdeology, colHeroClass
		});
		vwHero.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwHero.GridControl = grdHero;
		vwHero.GroupPanelText = " ";
		vwHero.Name = "vwHero";
		vwHero.OptionsBehavior.AllowIncrementalSearch = true;
		vwHero.OptionsBehavior.Editable = false;
		vwHero.OptionsBehavior.ReadOnly = true;
		vwHero.OptionsFilter.UseNewCustomFilterDialog = true;
		vwHero.OptionsNavigation.UseTabKey = false;
		vwHero.OptionsSelection.MultiSelect = true;
		vwHero.OptionsView.ShowGroupPanel = false;
		colID.Caption = "№";
		colID.FieldName = "ID";
		colID.Name = "colID";
		colID.Visible = true;
		colID.VisibleIndex = 0;
		colID.Width = 25;
		colHero2.Caption = "Герой";
		colHero2.FieldName = "Hero";
		colHero2.Name = "colHero2";
		colHero2.Visible = true;
		colHero2.VisibleIndex = 1;
		colHero2.Width = 73;
		colPlace1.Caption = "Местоположение";
		colPlace1.FieldName = "Place";
		colPlace1.Name = "colPlace1";
		colPlace1.Visible = true;
		colPlace1.VisibleIndex = 2;
		colPlace1.Width = 52;
		colColor3.Caption = "Флаг";
		colColor3.FieldName = "Color";
		colColor3.Name = "colColor3";
		colColor3.Visible = true;
		colColor3.VisibleIndex = 3;
		colColor3.Width = 52;
		colLevel3.Caption = "Уровень";
		colLevel3.FieldName = "Level";
		colLevel3.Name = "colLevel3";
		colLevel3.Visible = true;
		colLevel3.VisibleIndex = 4;
		colLevel3.Width = 30;
		colPrimarySkill3.Caption = "Первичный навык";
		colPrimarySkill3.FieldName = "PrimarySkill";
		colPrimarySkill3.Name = "colPrimarySkill3";
		colPrimarySkill3.Visible = true;
		colPrimarySkill3.VisibleIndex = 6;
		colPrimarySkill3.Width = 93;
		colSecondarySkill3.Caption = "Вторичный навык";
		colSecondarySkill3.FieldName = "SecondarySkill";
		colSecondarySkill3.Name = "colSecondarySkill3";
		colSecondarySkill3.Visible = true;
		colSecondarySkill3.VisibleIndex = 7;
		colSecondarySkill3.Width = 103;
		colArt4.Caption = "Артефакт";
		colArt4.FieldName = "Art";
		colArt4.Name = "colArt4";
		colArt4.Visible = true;
		colArt4.VisibleIndex = 8;
		colArt4.Width = 103;
		colSpell5.Caption = "Заклинание";
		colSpell5.FieldName = "Spell";
		colSpell5.Name = "colSpell5";
		colSpell5.Visible = true;
		colSpell5.VisibleIndex = 9;
		colSpell5.Width = 103;
		colMonster5.Caption = "Монстр";
		colMonster5.FieldName = "Monster";
		colMonster5.Name = "colMonster5";
		colMonster5.Visible = true;
		colMonster5.VisibleIndex = 10;
		colMonster5.Width = 103;
		colMachine1.Caption = "Машина";
		colMachine1.FieldName = "Machine";
		colMachine1.Name = "colMachine1";
		colMachine1.Visible = true;
		colMachine1.VisibleIndex = 12;
		colMachine1.Width = 49;
		colBook1.Caption = "Книга заклинаний";
		colBook1.FieldName = "Book";
		colBook1.Name = "colBook1";
		colBook1.Visible = true;
		colBook1.VisibleIndex = 13;
		colBook1.Width = 35;
		colMP1.FieldName = "MP";
		colMP1.Name = "colMP1";
		colMP1.Visible = true;
		colMP1.VisibleIndex = 14;
		colMP1.Width = 40;
		colExperience1.Caption = "Опыт";
		colExperience1.FieldName = "Experience";
		colExperience1.Name = "colExperience1";
		colExperience1.Visible = true;
		colExperience1.VisibleIndex = 5;
		colExperience1.Width = 41;
		colHP10.Caption = "∑ HP";
		colHP10.FieldName = "HP";
		colHP10.Name = "colHP10";
		colHP10.Visible = true;
		colHP10.VisibleIndex = 11;
		colHP10.Width = 46;
		colHire.Caption = "Найм";
		colHire.FieldName = "Hire";
		colHire.Name = "colHire";
		colHire.Width = 41;
		colIdeology.Caption = "Идеология";
		colIdeology.FieldName = "Ideology";
		colIdeology.Name = "colIdeology";
		colIdeology.Width = 65;
		colHeroClass.Caption = "Класс";
		colHeroClass.FieldName = "Class";
		colHeroClass.Name = "colHeroClass";
		((Control)tabTown).Controls.Add((Control)(object)grdTown);
		((Control)tabTown).Name = "tabTown";
		tabTown.Size = new Size(969, 448);
		((Control)tabTown).Text = "Города";
		grdTown.DataSource = rTownBindingSource;
		((Control)grdTown).Dock = (DockStyle)5;
		((Control)grdTown).Location = new Point(0, 0);
		grdTown.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdTown.LookAndFeel.UseDefaultLookAndFeel = false;
		grdTown.MainView = vwTown;
		grdTown.MenuManager = barManager1;
		((Control)grdTown).Name = "grdTown";
		((Control)grdTown).Size = new Size(969, 448);
		((Control)grdTown).TabIndex = 0;
		grdTown.ToolTipController = toolTipController1;
		grdTown.ViewCollection.AddRange(new BaseView[1] { vwTown });
		vwTown.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwTown.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwTown.Appearance.FocusedRow.ForeColor = Color.White;
		vwTown.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwTown.Appearance.FocusedRow.Options.UseBackColor = true;
		vwTown.Appearance.FocusedRow.Options.UseForeColor = true;
		vwTown.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwTown.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwTown.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwTown.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwTown.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwTown.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwTown.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwTown.Appearance.SelectedRow.ForeColor = Color.Black;
		vwTown.Appearance.SelectedRow.Options.UseBackColor = true;
		vwTown.Appearance.SelectedRow.Options.UseForeColor = true;
		vwTown.Columns.AddRange(new GridColumn[15]
		{
			colX10, colY10, colZ10, colName3, colType, colLevel1, colSpell3, colColor, colGarrison, colBuilt,
			colHP9, colLibrary, colID3, colAvailable, colTimer
		});
		vwTown.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwTown.GridControl = grdTown;
		vwTown.GroupPanelText = " ";
		vwTown.Name = "vwTown";
		vwTown.OptionsBehavior.AllowIncrementalSearch = true;
		vwTown.OptionsBehavior.Editable = false;
		vwTown.OptionsBehavior.ReadOnly = true;
		vwTown.OptionsFilter.UseNewCustomFilterDialog = true;
		vwTown.OptionsNavigation.UseTabKey = false;
		vwTown.OptionsSelection.MultiSelect = true;
		vwTown.OptionsView.ShowGroupPanel = false;
		colX10.FieldName = "X";
		colX10.Name = "colX10";
		colX10.Visible = true;
		colX10.VisibleIndex = 0;
		colX10.Width = 27;
		colY10.FieldName = "Y";
		colY10.Name = "colY10";
		colY10.Visible = true;
		colY10.VisibleIndex = 1;
		colY10.Width = 27;
		colZ10.FieldName = "Z";
		colZ10.Name = "colZ10";
		colZ10.Visible = true;
		colZ10.VisibleIndex = 2;
		colZ10.Width = 27;
		colName3.Caption = "Город";
		colName3.FieldName = "Name";
		colName3.Name = "colName3";
		colName3.Visible = true;
		colName3.VisibleIndex = 3;
		colName3.Width = 76;
		colType.Caption = "Тип";
		colType.FieldName = "Type";
		colType.Name = "colType";
		colType.Visible = true;
		colType.VisibleIndex = 4;
		colType.Width = 64;
		colLevel1.Caption = "Уровень";
		colLevel1.FieldName = "Slot";
		colLevel1.Name = "colLevel1";
		colLevel1.Visible = true;
		colLevel1.VisibleIndex = 6;
		colLevel1.Width = 32;
		colSpell3.Caption = "Заклинание";
		colSpell3.FieldName = "Spell";
		colSpell3.Name = "colSpell3";
		colSpell3.Visible = true;
		colSpell3.VisibleIndex = 7;
		colSpell3.Width = 290;
		colColor.Caption = "Флаг";
		colColor.FieldName = "Color";
		colColor.Name = "colColor";
		colColor.Visible = true;
		colColor.VisibleIndex = 5;
		colColor.Width = 64;
		colGarrison.Caption = "Гарнизон";
		colGarrison.FieldName = "Garrison";
		colGarrison.Name = "colGarrison";
		colGarrison.Visible = true;
		colGarrison.VisibleIndex = 12;
		colGarrison.Width = 95;
		colBuilt.Caption = "Построен";
		colBuilt.FieldName = "Built";
		colBuilt.Name = "colBuilt";
		colBuilt.Visible = true;
		colBuilt.VisibleIndex = 8;
		colBuilt.Width = 55;
		colHP9.Caption = "∑ HP";
		colHP9.FieldName = "HP";
		colHP9.Name = "colHP9";
		colHP9.Visible = true;
		colHP9.VisibleIndex = 13;
		colHP9.Width = 43;
		colLibrary.Caption = "Библиотека";
		colLibrary.FieldName = "Library";
		colLibrary.Name = "colLibrary";
		colLibrary.Visible = true;
		colLibrary.VisibleIndex = 11;
		colLibrary.Width = 45;
		colID3.Caption = "ID";
		colID3.FieldName = "ID";
		colID3.Name = "colID3";
		colID3.OptionsColumn.ShowInCustomizationForm = false;
		colAvailable.Caption = "Доступен";
		colAvailable.FieldName = "Available";
		colAvailable.Name = "colAvailable";
		colAvailable.Visible = true;
		colAvailable.VisibleIndex = 9;
		colAvailable.Width = 55;
		colTimer.Caption = "Таймер";
		colTimer.FieldName = "Timer";
		colTimer.Name = "colTimer";
		colTimer.Visible = true;
		colTimer.VisibleIndex = 10;
		colTimer.Width = 48;
		((Control)tabAllArts).Controls.Add((Control)(object)grdAllArts);
		((Control)tabAllArts).Name = "tabAllArts";
		tabAllArts.Size = new Size(969, 448);
		((Control)tabAllArts).Text = "Все Арты";
		grdAllArts.DataSource = rAllArtsBindingSource;
		((Control)grdAllArts).Dock = (DockStyle)5;
		((Control)grdAllArts).Location = new Point(0, 0);
		grdAllArts.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdAllArts.LookAndFeel.UseDefaultLookAndFeel = false;
		grdAllArts.MainView = vwAllArts;
		grdAllArts.MenuManager = barManager1;
		((Control)grdAllArts).Name = "grdAllArts";
		((Control)grdAllArts).Size = new Size(969, 448);
		((Control)grdAllArts).TabIndex = 0;
		grdAllArts.ToolTipController = toolTipController1;
		grdAllArts.ViewCollection.AddRange(new BaseView[1] { vwAllArts });
		vwAllArts.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwAllArts.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwAllArts.Appearance.FocusedRow.ForeColor = Color.White;
		vwAllArts.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwAllArts.Appearance.FocusedRow.Options.UseBackColor = true;
		vwAllArts.Appearance.FocusedRow.Options.UseForeColor = true;
		vwAllArts.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwAllArts.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwAllArts.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwAllArts.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwAllArts.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwAllArts.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwAllArts.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwAllArts.Appearance.SelectedRow.ForeColor = Color.Black;
		vwAllArts.Appearance.SelectedRow.Options.UseBackColor = true;
		vwAllArts.Appearance.SelectedRow.Options.UseForeColor = true;
		vwAllArts.Columns.AddRange(new GridColumn[16]
		{
			colX11, colY11, colZ11, colLocality10, colObject7, colArtefact1, colClass2, gridColumn1, colSlot1, colPlace,
			colColor1, colHero, colDoll, colMonster3, colMission, colGuard5
		});
		vwAllArts.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwAllArts.GridControl = grdAllArts;
		vwAllArts.GroupPanelText = " ";
		vwAllArts.Name = "vwAllArts";
		vwAllArts.OptionsBehavior.AllowIncrementalSearch = true;
		vwAllArts.OptionsBehavior.Editable = false;
		vwAllArts.OptionsBehavior.ReadOnly = true;
		vwAllArts.OptionsFilter.UseNewCustomFilterDialog = true;
		vwAllArts.OptionsNavigation.UseTabKey = false;
		vwAllArts.OptionsSelection.MultiSelect = true;
		vwAllArts.OptionsView.ShowGroupPanel = false;
		colX11.FieldName = "X";
		colX11.Name = "colX11";
		colX11.Visible = true;
		colX11.VisibleIndex = 0;
		colX11.Width = 27;
		colY11.FieldName = "Y";
		colY11.Name = "colY11";
		colY11.Visible = true;
		colY11.VisibleIndex = 1;
		colY11.Width = 27;
		colZ11.FieldName = "Z";
		colZ11.Name = "colZ11";
		colZ11.Visible = true;
		colZ11.VisibleIndex = 2;
		colZ11.Width = 27;
		colLocality10.Caption = "Район";
		colLocality10.FieldName = "Locality";
		colLocality10.Name = "colLocality10";
		colLocality10.Visible = true;
		colLocality10.VisibleIndex = 3;
		colLocality10.Width = 32;
		colObject7.Caption = "Объект";
		colObject7.FieldName = "Object";
		colObject7.Name = "colObject7";
		colObject7.Visible = true;
		colObject7.VisibleIndex = 4;
		colObject7.Width = 82;
		colArtefact1.Caption = "Артефакт";
		colArtefact1.FieldName = "Artefact";
		colArtefact1.Name = "colArtefact1";
		colArtefact1.Visible = true;
		colArtefact1.VisibleIndex = 6;
		colArtefact1.Width = 212;
		colClass2.Caption = "Класс";
		colClass2.FieldName = "Class";
		colClass2.Name = "colClass2";
		colClass2.Visible = true;
		colClass2.VisibleIndex = 7;
		colClass2.Width = 43;
		gridColumn1.Caption = "Реликт-С";
		gridColumn1.FieldName = "Relic-C";
		gridColumn1.Name = "gridColumn1";
		gridColumn1.Visible = true;
		gridColumn1.VisibleIndex = 8;
		gridColumn1.Width = 59;
		colSlot1.Caption = "Слот";
		colSlot1.FieldName = "Slot";
		colSlot1.Name = "colSlot1";
		colSlot1.Visible = true;
		colSlot1.VisibleIndex = 5;
		colSlot1.Width = 37;
		colPlace.Caption = "Местоположение";
		colPlace.FieldName = "Place";
		colPlace.Name = "colPlace";
		colPlace.Visible = true;
		colPlace.VisibleIndex = 12;
		colPlace.Width = 45;
		colColor1.Caption = "Флаг";
		colColor1.FieldName = "Color";
		colColor1.Name = "colColor1";
		colColor1.Visible = true;
		colColor1.VisibleIndex = 11;
		colColor1.Width = 49;
		colHero.Caption = "Герой";
		colHero.FieldName = "Hero";
		colHero.Name = "colHero";
		colHero.Visible = true;
		colHero.VisibleIndex = 9;
		colHero.Width = 76;
		colDoll.Caption = "Кукла";
		colDoll.FieldName = "Doll";
		colDoll.Name = "colDoll";
		colDoll.Visible = true;
		colDoll.VisibleIndex = 10;
		colDoll.Width = 53;
		colMonster3.Caption = "Монстр";
		colMonster3.FieldName = "Monster";
		colMonster3.Name = "colMonster3";
		colMonster3.Visible = true;
		colMonster3.VisibleIndex = 13;
		colMonster3.Width = 56;
		colMission.Caption = "Задание";
		colMission.FieldName = "Mission";
		colMission.Name = "colMission";
		colMission.Visible = true;
		colMission.VisibleIndex = 14;
		colMission.Width = 56;
		colGuard5.Caption = "Охрана";
		colGuard5.FieldName = "Guard";
		colGuard5.Name = "colGuard5";
		colGuard5.Visible = true;
		colGuard5.VisibleIndex = 15;
		colGuard5.Width = 67;
		((Control)tabAllSpell).Controls.Add((Control)(object)grdAllSpell);
		((Control)tabAllSpell).Name = "tabAllSpell";
		tabAllSpell.Size = new Size(969, 448);
		((Control)tabAllSpell).Text = "Все Заклы";
		grdAllSpell.DataSource = rAllSpellBindingSource;
		((Control)grdAllSpell).Dock = (DockStyle)5;
		((Control)grdAllSpell).Location = new Point(0, 0);
		grdAllSpell.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdAllSpell.LookAndFeel.UseDefaultLookAndFeel = false;
		grdAllSpell.MainView = vwAllSpell;
		grdAllSpell.MenuManager = barManager1;
		((Control)grdAllSpell).Name = "grdAllSpell";
		((Control)grdAllSpell).Size = new Size(969, 448);
		((Control)grdAllSpell).TabIndex = 0;
		grdAllSpell.ToolTipController = toolTipController1;
		grdAllSpell.ViewCollection.AddRange(new BaseView[1] { vwAllSpell });
		rAllSpellBindingSource.DataMember = "R_AllSpell";
		rAllSpellBindingSource.DataSource = dsResult;
		vwAllSpell.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwAllSpell.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwAllSpell.Appearance.FocusedRow.ForeColor = Color.White;
		vwAllSpell.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwAllSpell.Appearance.FocusedRow.Options.UseBackColor = true;
		vwAllSpell.Appearance.FocusedRow.Options.UseForeColor = true;
		vwAllSpell.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwAllSpell.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwAllSpell.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwAllSpell.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwAllSpell.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwAllSpell.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwAllSpell.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwAllSpell.Appearance.SelectedRow.ForeColor = Color.Black;
		vwAllSpell.Appearance.SelectedRow.Options.UseBackColor = true;
		vwAllSpell.Appearance.SelectedRow.Options.UseForeColor = true;
		vwAllSpell.Columns.AddRange(new GridColumn[15]
		{
			colX19, colY19, colZ19, colObject11, colSpell6, colSlot3, colColor5, colName4, colBuilt1, colGarrison1,
			colID1, colHero3, colPlace2, colMission3, colGuard7
		});
		vwAllSpell.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwAllSpell.GridControl = grdAllSpell;
		vwAllSpell.Name = "vwAllSpell";
		vwAllSpell.OptionsBehavior.AllowIncrementalSearch = true;
		vwAllSpell.OptionsBehavior.Editable = false;
		vwAllSpell.OptionsBehavior.ReadOnly = true;
		vwAllSpell.OptionsFilter.UseNewCustomFilterDialog = true;
		vwAllSpell.OptionsNavigation.UseTabKey = false;
		vwAllSpell.OptionsSelection.MultiSelect = true;
		vwAllSpell.OptionsView.ShowGroupPanel = false;
		colX19.FieldName = "X";
		colX19.Name = "colX19";
		colX19.Visible = true;
		colX19.VisibleIndex = 0;
		colX19.Width = 27;
		colY19.FieldName = "Y";
		colY19.Name = "colY19";
		colY19.Visible = true;
		colY19.VisibleIndex = 1;
		colY19.Width = 27;
		colZ19.FieldName = "Z";
		colZ19.Name = "colZ19";
		colZ19.Visible = true;
		colZ19.VisibleIndex = 2;
		colZ19.Width = 27;
		colObject11.Caption = "Объект";
		colObject11.FieldName = "Object";
		colObject11.Name = "colObject11";
		colObject11.Visible = true;
		colObject11.VisibleIndex = 3;
		colObject11.Width = 81;
		colSpell6.Caption = "Заклинание";
		colSpell6.FieldName = "Spell";
		colSpell6.Name = "colSpell6";
		colSpell6.Visible = true;
		colSpell6.VisibleIndex = 4;
		colSpell6.Width = 146;
		colSlot3.Caption = "Уровень";
		colSlot3.FieldName = "Slot";
		colSlot3.Name = "colSlot3";
		colSlot3.Visible = true;
		colSlot3.VisibleIndex = 5;
		colSlot3.Width = 30;
		colColor5.Caption = "Флаг";
		colColor5.FieldName = "Color";
		colColor5.Name = "colColor5";
		colColor5.Visible = true;
		colColor5.VisibleIndex = 6;
		colColor5.Width = 46;
		colName4.Caption = "Город";
		colName4.FieldName = "Name";
		colName4.Name = "colName4";
		colName4.Visible = true;
		colName4.VisibleIndex = 7;
		colName4.Width = 77;
		colBuilt1.Caption = "Построен";
		colBuilt1.FieldName = "Built";
		colBuilt1.Name = "colBuilt1";
		colBuilt1.Visible = true;
		colBuilt1.VisibleIndex = 8;
		colBuilt1.Width = 40;
		colGarrison1.Caption = "Гарнизон";
		colGarrison1.FieldName = "Garrison";
		colGarrison1.Name = "colGarrison1";
		colGarrison1.Visible = true;
		colGarrison1.VisibleIndex = 9;
		colGarrison1.Width = 65;
		colID1.Caption = "№";
		colID1.FieldName = "ID";
		colID1.Name = "colID1";
		colID1.Width = 25;
		colHero3.Caption = "Герой";
		colHero3.FieldName = "Hero";
		colHero3.Name = "colHero3";
		colHero3.Visible = true;
		colHero3.VisibleIndex = 10;
		colHero3.Width = 73;
		colPlace2.Caption = "Местоположение";
		colPlace2.FieldName = "Place";
		colPlace2.Name = "colPlace2";
		colPlace2.Visible = true;
		colPlace2.VisibleIndex = 11;
		colPlace2.Width = 56;
		colMission3.Caption = "Задание";
		colMission3.FieldName = "Mission";
		colMission3.Name = "colMission3";
		colMission3.Visible = true;
		colMission3.VisibleIndex = 12;
		colMission3.Width = 110;
		colGuard7.Caption = "Охрана";
		colGuard7.FieldName = "Guard";
		colGuard7.Name = "colGuard7";
		colGuard7.Visible = true;
		colGuard7.VisibleIndex = 13;
		colGuard7.Width = 143;
		((Control)tabAllSkill).Controls.Add((Control)(object)grdAllSkill);
		((Control)tabAllSkill).Name = "tabAllSkill";
		tabAllSkill.Size = new Size(969, 448);
		((Control)tabAllSkill).Text = "Все Навыки";
		grdAllSkill.DataSource = rAllSkillBindingSource;
		((Control)grdAllSkill).Dock = (DockStyle)5;
		((Control)grdAllSkill).Location = new Point(0, 0);
		grdAllSkill.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdAllSkill.LookAndFeel.UseDefaultLookAndFeel = false;
		grdAllSkill.MainView = vwAllSkill;
		grdAllSkill.MenuManager = barManager1;
		((Control)grdAllSkill).Name = "grdAllSkill";
		((Control)grdAllSkill).Size = new Size(969, 448);
		((Control)grdAllSkill).TabIndex = 0;
		grdAllSkill.ToolTipController = toolTipController1;
		grdAllSkill.ViewCollection.AddRange(new BaseView[1] { vwAllSkill });
		rAllSkillBindingSource.DataMember = "R_AllSkill";
		rAllSkillBindingSource.DataSource = dsResult;
		vwAllSkill.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwAllSkill.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwAllSkill.Appearance.FocusedRow.ForeColor = Color.White;
		vwAllSkill.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwAllSkill.Appearance.FocusedRow.Options.UseBackColor = true;
		vwAllSkill.Appearance.FocusedRow.Options.UseForeColor = true;
		vwAllSkill.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwAllSkill.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwAllSkill.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwAllSkill.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwAllSkill.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwAllSkill.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwAllSkill.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwAllSkill.Appearance.SelectedRow.ForeColor = Color.Black;
		vwAllSkill.Appearance.SelectedRow.Options.UseBackColor = true;
		vwAllSkill.Appearance.SelectedRow.Options.UseForeColor = true;
		vwAllSkill.Columns.AddRange(new GridColumn[10] { colX20, colY20, colZ20, colLocality18, colObject13, colSkill1, colMission4, colGuard8, colLevel13, colSlot13 });
		vwAllSkill.FocusRectStyle = DrawFocusRectStyle.RowFocus;
		vwAllSkill.GridControl = grdAllSkill;
		vwAllSkill.Name = "vwAllSkill";
		vwAllSkill.OptionsBehavior.AllowIncrementalSearch = true;
		vwAllSkill.OptionsBehavior.Editable = false;
		vwAllSkill.OptionsBehavior.ReadOnly = true;
		vwAllSkill.OptionsFilter.UseNewCustomFilterDialog = true;
		vwAllSkill.OptionsNavigation.UseTabKey = false;
		vwAllSkill.OptionsSelection.MultiSelect = true;
		vwAllSkill.OptionsView.ShowGroupPanel = false;
		colX20.FieldName = "X";
		colX20.Name = "colX20";
		colX20.Visible = true;
		colX20.VisibleIndex = 0;
		colX20.Width = 27;
		colY20.FieldName = "Y";
		colY20.Name = "colY20";
		colY20.Visible = true;
		colY20.VisibleIndex = 1;
		colY20.Width = 27;
		colZ20.FieldName = "Z";
		colZ20.Name = "colZ20";
		colZ20.Visible = true;
		colZ20.VisibleIndex = 2;
		colZ20.Width = 27;
		colLocality18.Caption = "Район";
		colLocality18.FieldName = "Locality";
		colLocality18.Name = "colLocality18";
		colLocality18.Visible = true;
		colLocality18.VisibleIndex = 3;
		colLocality18.Width = 32;
		colObject13.Caption = "Объект";
		colObject13.FieldName = "Object";
		colObject13.Name = "colObject13";
		colObject13.Visible = true;
		colObject13.VisibleIndex = 4;
		colObject13.Width = 140;
		colSkill1.Caption = "Навык";
		colSkill1.FieldName = "Skill";
		colSkill1.Name = "colSkill1";
		colSkill1.Visible = true;
		colSkill1.VisibleIndex = 6;
		colSkill1.Width = 230;
		colMission4.Caption = "Задание";
		colMission4.FieldName = "Mission";
		colMission4.Name = "colMission4";
		colMission4.Visible = true;
		colMission4.VisibleIndex = 7;
		colMission4.Width = 192;
		colGuard8.Caption = "Охрана";
		colGuard8.FieldName = "Guard";
		colGuard8.Name = "colGuard8";
		colGuard8.Visible = true;
		colGuard8.VisibleIndex = 8;
		colGuard8.Width = 193;
		colLevel13.Caption = "Уровень";
		colLevel13.FieldName = "Level";
		colLevel13.Name = "colLevel13";
		colLevel13.Visible = true;
		colLevel13.VisibleIndex = 5;
		colLevel13.Width = 80;
		colSlot13.Caption = "Slot";
		colSlot13.FieldName = "Slot";
		colSlot13.Name = "colSlot13";
		colSlot13.OptionsColumn.ShowInCustomizationForm = false;
		((Control)tabExperience).Controls.Add((Control)(object)grdExperience);
		((Control)tabExperience).Name = "tabExperience";
		tabExperience.Size = new Size(969, 448);
		((Control)tabExperience).Text = "Опыт";
		grdExperience.DataSource = rAllExperienceBindingSource;
		((Control)grdExperience).Dock = (DockStyle)5;
		((Control)grdExperience).Location = new Point(0, 0);
		grdExperience.LookAndFeel.SkinName = "MySkin_Lilian1";
		grdExperience.LookAndFeel.UseDefaultLookAndFeel = false;
		grdExperience.MainView = vwExperience;
		grdExperience.MenuManager = barManager1;
		((Control)grdExperience).Name = "grdExperience";
		((Control)grdExperience).Size = new Size(969, 448);
		((Control)grdExperience).TabIndex = 0;
		grdExperience.ToolTipController = toolTipController1;
		grdExperience.ViewCollection.AddRange(new BaseView[1] { vwExperience });
		rAllExperienceBindingSource.DataMember = "R_AllExperience";
		rAllExperienceBindingSource.DataSource = dsResult;
		vwExperience.Appearance.FocusedRow.BackColor = Color.FromArgb(72, 118, 186);
		vwExperience.Appearance.FocusedRow.BackColor2 = Color.FromArgb(169, 189, 226);
		vwExperience.Appearance.FocusedRow.ForeColor = Color.White;
		vwExperience.Appearance.FocusedRow.GradientMode = (LinearGradientMode)1;
		vwExperience.Appearance.FocusedRow.Options.UseBackColor = true;
		vwExperience.Appearance.FocusedRow.Options.UseForeColor = true;
		vwExperience.Appearance.HideSelectionRow.BackColor = Color.FromArgb(169, 189, 226);
		vwExperience.Appearance.HideSelectionRow.BackColor2 = Color.Transparent;
		vwExperience.Appearance.HideSelectionRow.ForeColor = Color.Black;
		vwExperience.Appearance.HideSelectionRow.Options.UseBackColor = true;
		vwExperience.Appearance.HideSelectionRow.Options.UseForeColor = true;
		vwExperience.Appearance.SelectedRow.BackColor = Color.FromArgb(226, 238, 255);
		vwExperience.Appearance.SelectedRow.BackColor2 = Color.Transparent;
		vwExperience.Appearance.SelectedRow.ForeColor = Color.Black;
		vwExperience.Appearance.SelectedRow.Options.UseBackColor = true;
		vwExperience.Appearance.SelectedRow.Options.UseForeColor = true;
		vwExperience.Columns.AddRange(new GridColumn[21]
		{
			colX21, colY21, colZ21, colLocality19, colObject14, colMonster7, colGuard9, colMission5, colID2, colHero4,
			colExperience3, colColor21, colHP21, colXP, colXP1, colXP2, colXP3, colArt21, colResource21, colTown21,
			colSpell21
		});
		vwExperience.GridControl = grdExperience;
		vwExperience.Name = "vwExperience";
		vwExperience.OptionsBehavior.AllowIncrementalSearch = true;
		vwExperience.OptionsBehavior.Editable = false;
		vwExperience.OptionsBehavior.ReadOnly = true;
		vwExperience.OptionsFilter.UseNewCustomFilterDialog = true;
		vwExperience.OptionsNavigation.UseTabKey = false;
		vwExperience.OptionsSelection.MultiSelect = true;
		vwExperience.OptionsView.ShowGroupPanel = false;
		colX21.FieldName = "X";
		colX21.Name = "colX21";
		colX21.Visible = true;
		colX21.VisibleIndex = 0;
		colX21.Width = 27;
		colY21.FieldName = "Y";
		colY21.Name = "colY21";
		colY21.Visible = true;
		colY21.VisibleIndex = 1;
		colY21.Width = 27;
		colZ21.FieldName = "Z";
		colZ21.Name = "colZ21";
		colZ21.Visible = true;
		colZ21.VisibleIndex = 2;
		colZ21.Width = 27;
		colLocality19.Caption = "Район";
		colLocality19.FieldName = "Locality";
		colLocality19.Name = "colLocality19";
		colLocality19.Visible = true;
		colLocality19.VisibleIndex = 3;
		colLocality19.Width = 32;
		colObject14.Caption = "Объект";
		colObject14.FieldName = "Object";
		colObject14.Name = "colObject14";
		colObject14.Visible = true;
		colObject14.VisibleIndex = 4;
		colObject14.Width = 90;
		colMonster7.Caption = "Монстр";
		colMonster7.FieldName = "Monster";
		colMonster7.Name = "colMonster7";
		colMonster7.Visible = true;
		colMonster7.VisibleIndex = 8;
		colMonster7.Width = 90;
		colGuard9.Caption = "Охрана";
		colGuard9.FieldName = "Guard";
		colGuard9.Name = "colGuard9";
		colGuard9.Visible = true;
		colGuard9.VisibleIndex = 11;
		colGuard9.Width = 115;
		colMission5.Caption = "Задание";
		colMission5.FieldName = "Mission";
		colMission5.Name = "colMission5";
		colMission5.Width = 80;
		colID2.Caption = "№";
		colID2.FieldName = "ID";
		colID2.Name = "colID2";
		colID2.Width = 40;
		colHero4.Caption = "Герой";
		colHero4.FieldName = "Hero";
		colHero4.Name = "colHero4";
		colHero4.Visible = true;
		colHero4.VisibleIndex = 6;
		colHero4.Width = 70;
		colExperience3.Caption = "Опыт";
		colExperience3.FieldName = "Experience";
		colExperience3.Name = "colExperience3";
		colExperience3.Visible = true;
		colExperience3.VisibleIndex = 13;
		colExperience3.Width = 55;
		colColor21.Caption = "Флаг";
		colColor21.FieldName = "Color";
		colColor21.Name = "colColor21";
		colColor21.Visible = true;
		colColor21.VisibleIndex = 7;
		colColor21.Width = 80;
		colHP21.Caption = "∑ HP";
		colHP21.FieldName = "HP";
		colHP21.Name = "colHP21";
		colHP21.Visible = true;
		colHP21.VisibleIndex = 12;
		colHP21.Width = 55;
		colXP.Caption = "∑ XP";
		colXP.FieldName = "XP";
		colXP.Name = "colXP";
		colXP.Visible = true;
		colXP.VisibleIndex = 14;
		colXP.Width = 60;
		colXP1.Caption = "∑ XP+5%";
		colXP1.FieldName = "XP5";
		colXP1.Name = "colXP1";
		colXP1.Width = 60;
		colXP2.Caption = "∑ XP+10%";
		colXP2.FieldName = "XP10";
		colXP2.Name = "colXP2";
		colXP2.Width = 60;
		colXP3.Caption = "∑ XP+15%";
		colXP3.FieldName = "XP15";
		colXP3.Name = "colXP3";
		colXP3.Width = 60;
		colArt21.Caption = "Артефакт";
		colArt21.FieldName = "Art";
		colArt21.Name = "colArt21";
		colArt21.Visible = true;
		colArt21.VisibleIndex = 9;
		colArt21.Width = 90;
		colResource21.Caption = "Ресурс";
		colResource21.FieldName = "Resource";
		colResource21.Name = "colResource21";
		colResource21.Visible = true;
		colResource21.VisibleIndex = 10;
		colResource21.Width = 60;
		colTown21.Caption = "Город";
		colTown21.FieldName = "Town";
		colTown21.Name = "colTown21";
		colTown21.Visible = true;
		colTown21.VisibleIndex = 5;
		colTown21.Width = 70;
		colSpell21.Caption = "Заклинание";
		colSpell21.FieldName = "Spell";
		colSpell21.Name = "colSpell21";
		colSpell21.Width = 80;
		barManager2.Controller = barAndDockingController2;
		barManager2.DockControls.Add(barDockControl1);
		barManager2.DockControls.Add(barDockControl2);
		barManager2.DockControls.Add(barDockControl3);
		barManager2.DockControls.Add(barDockControl4);
		barManager2.Form = (Control)(object)this;
		barManager2.MaxItemId = 1;
		barAndDockingController2.LookAndFeel.SkinName = "MySkin_Lilian1";
		barAndDockingController2.LookAndFeel.UseDefaultLookAndFeel = false;
		barAndDockingController2.PropertiesBar.AllowLinkLighting = false;
		((Control)barDockControl1).CausesValidation = false;
		barDockControl1.Dock = (DockStyle)1;
		barDockControl1.Location = new Point(0, 0);
		barDockControl1.Size = new Size(974, 0);
		((Control)barDockControl2).CausesValidation = false;
		barDockControl2.Dock = (DockStyle)2;
		barDockControl2.Location = new Point(0, 537);
		barDockControl2.Size = new Size(974, 0);
		((Control)barDockControl3).CausesValidation = false;
		barDockControl3.Dock = (DockStyle)3;
		barDockControl3.Location = new Point(0, 0);
		barDockControl3.Size = new Size(0, 537);
		((Control)barDockControl4).CausesValidation = false;
		barDockControl4.Dock = (DockStyle)4;
		barDockControl4.Location = new Point(974, 0);
		barDockControl4.Size = new Size(0, 537);
		((FileDialog)saveFileDialog1).Filter = "LM Oracle files|*.lmh|All files|*.*";
		timer1.Interval = 20;
		timer1.Tick += timer1_Tick;
		timer2.Interval = 1000;
		timer2.Tick += timer2_Tick;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(974, 537);
		((Control)this).Controls.Add((Control)(object)tabControl);
		((Control)this).Controls.Add((Control)(object)statusStrip1);
		((Control)this).Controls.Add((Control)(object)barDockControlLeft);
		((Control)this).Controls.Add((Control)(object)barDockControlRight);
		((Control)this).Controls.Add((Control)(object)barDockControlBottom);
		((Control)this).Controls.Add((Control)(object)barDockControlTop);
		((Control)this).Controls.Add((Control)(object)barDockControl3);
		((Control)this).Controls.Add((Control)(object)barDockControl4);
		((Control)this).Controls.Add((Control)(object)barDockControl2);
		((Control)this).Controls.Add((Control)(object)barDockControl1);
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Control)this).MinimumSize = new Size(762, 217);
		((Control)this).Name = "MainForm";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Tag = "ProspectorRT;MainForm;";
		((Control)this).Text = "ProspectorRT  -  HoMM 3  Стартовый сейв";
		((Form)this).Load += MainForm_Load;
		((Control)this).LocationChanged += MainForm_LocationChanged;
		((Control)this).SizeChanged += MainForm_SizeChanged;
		((Control)statusStrip1).ResumeLayout(false);
		((Control)statusStrip1).PerformLayout();
		((ISupportInitialize)dsResult).EndInit();
		((ISupportInitialize)rMonstrBindingSource).EndInit();
		((ISupportInitialize)rHeroesBindingSource).EndInit();
		((ISupportInitialize)rAllArtsBindingSource).EndInit();
		((ISupportInitialize)rBindingSource).EndInit();
		((ISupportInitialize)rBankBindingSource).EndInit();
		((ISupportInitialize)rEventBoxBindingSource).EndInit();
		((ISupportInitialize)rScholarBindingSource).EndInit();
		((ISupportInitialize)rResourceBindingSource).EndInit();
		((ISupportInitialize)rChestBindingSource).EndInit();
		((ISupportInitialize)rSpellBindingSource).EndInit();
		((ISupportInitialize)rSkillBindingSource).EndInit();
		((ISupportInitialize)rCampBindingSource).EndInit();
		((ISupportInitialize)rMarketBindingSource).EndInit();
		((ISupportInitialize)rSeerHutBindingSource).EndInit();
		((ISupportInitialize)rPassGuardBindingSource).EndInit();
		((ISupportInitialize)rGarrisonBindingSource).EndInit();
		((ISupportInitialize)rPrisonBindingSource).EndInit();
		((ISupportInitialize)rObjectBindingSource).EndInit();
		((ISupportInitialize)rTownBindingSource).EndInit();
		((ISupportInitialize)barManager1).EndInit();
		((ISupportInitialize)ppmFile).EndInit();
		((ISupportInitialize)repositoryItemSpinEdit1).EndInit();
		((ISupportInitialize)repositoryItemSpinEdit2).EndInit();
		((ISupportInitialize)tabControl).EndInit();
		((Control)tabControl).ResumeLayout(false);
		((Control)tabArt).ResumeLayout(false);
		((ISupportInitialize)grdArt).EndInit();
		((ISupportInitialize)vwArt).EndInit();
		((Control)tabMonster).ResumeLayout(false);
		((ISupportInitialize)grdMonster).EndInit();
		((ISupportInitialize)vwMonster).EndInit();
		((Control)tabBank).ResumeLayout(false);
		((ISupportInitialize)grdBank).EndInit();
		((ISupportInitialize)vwBank).EndInit();
		((Control)tabBoxEvent).ResumeLayout(false);
		((ISupportInitialize)grdEventBox).EndInit();
		((ISupportInitialize)vwEventBox).EndInit();
		((Control)tabScholar).ResumeLayout(false);
		((ISupportInitialize)grdScholar).EndInit();
		((ISupportInitialize)vwScholar).EndInit();
		((Control)tabResource).ResumeLayout(false);
		((ISupportInitialize)grdResource).EndInit();
		((ISupportInitialize)vwResource).EndInit();
		((Control)tabChest).ResumeLayout(false);
		((ISupportInitialize)grdChest).EndInit();
		((ISupportInitialize)vwChest).EndInit();
		((Control)tabSpell).ResumeLayout(false);
		((ISupportInitialize)grdSpell).EndInit();
		((ISupportInitialize)vwSpell).EndInit();
		((Control)tabSkill).ResumeLayout(false);
		((ISupportInitialize)grdSkill).EndInit();
		((ISupportInitialize)vwSkill).EndInit();
		((Control)tabCamp).ResumeLayout(false);
		((ISupportInitialize)grdCamp).EndInit();
		((ISupportInitialize)vwCamp).EndInit();
		((Control)tabMarket).ResumeLayout(false);
		((ISupportInitialize)grdMarket).EndInit();
		((ISupportInitialize)vwMarket).EndInit();
		((Control)tabSeerHut).ResumeLayout(false);
		((ISupportInitialize)grdSeerHut).EndInit();
		((ISupportInitialize)vwSeerHut).EndInit();
		((Control)tabPassGuard).ResumeLayout(false);
		((ISupportInitialize)grdPassGuard).EndInit();
		((ISupportInitialize)vwPassGuard).EndInit();
		((Control)tabGarrison).ResumeLayout(false);
		((ISupportInitialize)grdGarrison).EndInit();
		((ISupportInitialize)vwGarrison).EndInit();
		((Control)tabPrison).ResumeLayout(false);
		((ISupportInitialize)grdPrison).EndInit();
		((ISupportInitialize)vwPrison).EndInit();
		((Control)tabObject).ResumeLayout(false);
		((ISupportInitialize)grdObject).EndInit();
		((ISupportInitialize)vwObject).EndInit();
		((Control)tabTopology).ResumeLayout(false);
		((ISupportInitialize)grdTopology).EndInit();
		((ISupportInitialize)rTopologyBindingSource).EndInit();
		((ISupportInitialize)vwTopology).EndInit();
		((Control)tabAllTimer).ResumeLayout(false);
		((ISupportInitialize)grdAllTimer).EndInit();
		((ISupportInitialize)rAllTimerBindingSource).EndInit();
		((ISupportInitialize)vwAllTimer).EndInit();
		((Control)tabHero).ResumeLayout(false);
		((ISupportInitialize)grdHero).EndInit();
		((ISupportInitialize)vwHero).EndInit();
		((Control)tabTown).ResumeLayout(false);
		((ISupportInitialize)grdTown).EndInit();
		((ISupportInitialize)vwTown).EndInit();
		((Control)tabAllArts).ResumeLayout(false);
		((ISupportInitialize)grdAllArts).EndInit();
		((ISupportInitialize)vwAllArts).EndInit();
		((Control)tabAllSpell).ResumeLayout(false);
		((ISupportInitialize)grdAllSpell).EndInit();
		((ISupportInitialize)rAllSpellBindingSource).EndInit();
		((ISupportInitialize)vwAllSpell).EndInit();
		((Control)tabAllSkill).ResumeLayout(false);
		((ISupportInitialize)grdAllSkill).EndInit();
		((ISupportInitialize)rAllSkillBindingSource).EndInit();
		((ISupportInitialize)vwAllSkill).EndInit();
		((Control)tabExperience).ResumeLayout(false);
		((ISupportInitialize)grdExperience).EndInit();
		((ISupportInitialize)rAllExperienceBindingSource).EndInit();
		((ISupportInitialize)vwExperience).EndInit();
		((ISupportInitialize)barManager2).EndInit();
		((ISupportInitialize)barAndDockingController2).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	public MainForm()
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		InitializeComponent();
	}

	public MainForm(bool ot, bool st)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		ot3 = ot;
		spt = st;
		InitializeComponent();
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		if (Registry.RegistryKeyExist("MainForm"))
		{
			try
			{
				((Form)this).Location = new Point(int.Parse(Registry.GetRegistryValue("MainForm", "X")), int.Parse(Registry.GetRegistryValue("MainForm", "Y")));
				((Form)this).Size = new Size(int.Parse(Registry.GetRegistryValue("MainForm", "Width")), int.Parse(Registry.GetRegistryValue("MainForm", "Height")));
				switch (Registry.GetRegistryValue("MainForm", "WindowState"))
				{
				case "Maximized":
					((Form)this).WindowState = (FormWindowState)2;
					break;
				case "Normal":
					((Form)this).WindowState = (FormWindowState)0;
					break;
				}
				((Form)this).StartPosition = (FormStartPosition)0;
				if (Registry.GetRegistryValue("MainForm", "StartButton") == "0")
				{
					tsButtonStart.Visibility = BarItemVisibility.Never;
					chiStart.Checked = false;
				}
				if (int.TryParse(Registry.GetRegistryValue("MainForm", "RecentFiles"), out var result))
				{
					biRecent.EditValue = result;
				}
			}
			catch
			{
			}
		}
		else
		{
			fLoadForm = false;
			MainForm_SizeChanged(sender, null);
		}
		fLoadForm = false;
		if (ot3)
		{
			tsButtonOT.Visibility = BarItemVisibility.Always;
		}
		if (spt)
		{
			tsButtonSPT.Visibility = BarItemVisibility.Always;
		}
		InitMembers(MyInit: true, (Control)(object)this, barManager2, null);
		CreateTblEventBox();
		CreateTblMonstr();
		CreateTblArtRes();
		CreateTblSeerHut();
		CreateTblPassGuard();
		CreateTblBanks();
		CreateTblUniver();
		CreateTblMarket();
		CreateTblGarrison();
		Create_PW();
		Create_PW10();
		Create_SW();
		InitOracle();
		string path = "";
		int[] verObjName = GetVerObjName(out path);
		ChangeObjName(verObjName[0], path, verObjName[1]);
		((ToolStripItem)tsLabelStatusL).Text = sChoose;
		((Control)grdArt).Select();
	}

	private void DbClear()
	{
		dsResult.Clear();
		TblEventBox.Clear();
		TblMonstr.Clear();
		TblArtRes.Clear();
		TblSeerHut.Clear();
		TblPassGuard.Clear();
		TblBanks.Clear();
		TblUniver.Clear();
		TblMarket.Clear();
		TblGarrison.Clear();
		((ToolStripItem)tsLabelGrail).Text = "";
	}

	private void BindingControl(bool on)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		if (on)
		{
			foreach (GridControl item in aGrid)
			{
				item.DataSource = (object)(BindingSource)hDS[((Control)item).Name];
			}
			return;
		}
		foreach (GridControl item2 in aGrid)
		{
			item2.DataSource = bsInit;
		}
	}

	private void InitOracle()
	{
		if (Registry.RegistryKeyExist("Oracle"))
		{
			string registryValue = Registry.GetRegistryValue("Oracle", "Version");
			if (byte.TryParse(registryValue, out Version) && (Version == 2 || Version == 1 || Version == 0))
			{
				switch (Version)
				{
				case 2:
					chi32EN.Checked = true;
					LMOracle.Version = 2;
					break;
				case 1:
					chi40EN.Checked = true;
					LMOracle.Version = 1;
					break;
				case 0:
					chi40RU.Checked = true;
					LMOracle.Version = 0;
					break;
				}
			}
			else
			{
				chi32EN.Checked = true;
				LMOracle.Version = 2;
			}
			registryValue = Registry.GetRegistryValue("Oracle", "Depth");
			if (int.TryParse(registryValue, out MaxLevel) && MaxLevel > 0 && MaxLevel < 25)
			{
				biDepth.EditValue = MaxLevel;
			}
			else
			{
				MaxLevel = int.Parse(biDepth.EditValue.ToString());
			}
			LMOracle.MaxLevel = MaxLevel;
			string registryValue2 = Registry.GetRegistryValue("Oracle", "WithoutGame");
			if (int.TryParse(registryValue2, out var result) && (result == 1 || result == 0))
			{
				switch (result)
				{
				case 1:
					chiFree.Checked = true;
					CommonSetting.LMOracleFree = true;
					break;
				case 0:
					chiFree.Checked = false;
					CommonSetting.LMOracleFree = false;
					break;
				}
			}
			else
			{
				bool lMOracleFree = (chiFree.Checked = true);
				CommonSetting.LMOracleFree = lMOracleFree;
			}
		}
		else
		{
			Registry.CreateKey("Oracle");
			Registry.SetRegistryValue("Oracle", "Version", Version.ToString());
			Registry.SetRegistryValue("Oracle", "Depth", MaxLevel.ToString());
			LMOracle.MaxLevel = MaxLevel;
			LMOracle.Version = Version;
		}
		LMOracle.PW = _PW;
		LMOracle.PW10 = _PW10;
		LMOracle.SW = _SW;
		LMOracle.SR = _SR;
	}

	private void tsButtonObjectName_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		string path = "";
		ObjectNameForm objectNameForm = new ObjectNameForm();
		int[] verObjName = GetVerObjName(out path);
		objectNameForm.iVersion = verObjName[0];
		if (objectNameForm.iVersion == 4)
		{
			objectNameForm.sPath = path;
			objectNameForm.iCodePage = verObjName[1];
		}
		if ((int)((Form)objectNameForm).ShowDialog() != 1)
		{
			return;
		}
		try
		{
			Registry.SetRegistryValue("ObjectName", "Version", objectNameForm.iVersion.ToString());
			if (objectNameForm.iVersion == 4)
			{
				Registry.SetRegistryValue("ObjectName", "Path", objectNameForm.sPath);
				Registry.SetRegistryValue("ObjectName", "CodePage", objectNameForm.iCodePage.ToString());
			}
			else
			{
				Registry.SetRegistryValue("ObjectName", "Path", "");
				Registry.SetRegistryValue("ObjectName", "CodePage", "0");
			}
		}
		catch
		{
		}
		ChangeObjName(objectNameForm.iVersion, objectNameForm.sPath, objectNameForm.iCodePage);
	}

	private void MainForm_SizeChanged(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (fLoadForm || (int)((Form)this).WindowState == 1)
		{
			return;
		}
		try
		{
			Registry.SetRegistryValue("MainForm", "WindowState", ((object)((Form)this).WindowState).ToString());
			if ((int)((Form)this).WindowState == 0)
			{
				Registry.SetRegistryValue("MainForm", "X", ((Form)this).Location.X.ToString());
				Registry.SetRegistryValue("MainForm", "Y", ((Form)this).Location.Y.ToString());
				Registry.SetRegistryValue("MainForm", "Width", ((Form)this).Size.Width.ToString());
				Registry.SetRegistryValue("MainForm", "Height", ((Form)this).Size.Height.ToString());
			}
		}
		catch
		{
		}
	}

	private void MainForm_LocationChanged(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (fLoadForm || (int)((Form)this).WindowState == 1)
		{
			return;
		}
		try
		{
			Registry.SetRegistryValue("MainForm", "WindowState", ((object)((Form)this).WindowState).ToString());
			if ((int)((Form)this).WindowState == 0)
			{
				Registry.SetRegistryValue("MainForm", "X", ((Form)this).Location.X.ToString());
				Registry.SetRegistryValue("MainForm", "Y", ((Form)this).Location.Y.ToString());
				Registry.SetRegistryValue("MainForm", "Width", ((Form)this).Size.Width.ToString());
				Registry.SetRegistryValue("MainForm", "Height", ((Form)this).Size.Height.ToString());
			}
		}
		catch
		{
		}
	}

	private void tsButtonExcel_ItemClick(object sender, ItemClickEventArgs e)
	{
		ExportExcel();
	}

	private void tsMenuService_Popup(object sender, EventArgs e)
	{
		tsButtonExcel.Enabled = tsButtonRefresh.Enabled;
	}

	private void tsButtonSPT_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		try
		{
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				if (assembly.GetName().Name == "SPT")
				{
					Obj2Form(assembly, "SPTForm", (Form)(object)this, null);
					return;
				}
			}
			Assembly asm = Assembly.LoadFrom(Application.StartupPath + "\\SPT.dll");
			Obj2Form(asm, "SPTForm", (Form)(object)this, null);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void tsButtonOT_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		try
		{
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				if (assembly.GetName().Name == "OT-3")
				{
					Obj2Form(assembly, "OTForm", (Form)(object)this, GetBoxes());
					return;
				}
			}
			Assembly asm = Assembly.LoadFrom(Application.StartupPath + "\\OT-3.dll");
			Obj2Form(asm, "OTForm", (Form)(object)this, GetBoxes());
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void Obj2Form(Assembly asm, string NameForm, Form frmMain, object tag)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		object obj = asm.CreateInstance(((object)frmMain).GetType().Namespace + "." + NameForm);
		((Form)obj).Owner = frmMain;
		((Control)(Form)obj).Tag = tag;
		((Control)(Form)obj).Text = CommonSetting.MyFile;
		((Form)obj).ShowDialog();
	}

	private DataTable GetBoxes()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("X", Type.GetType("System.Int32"));
		dataTable.Columns.Add("Y", Type.GetType("System.Int32"));
		dataTable.Columns.Add("Z", Type.GetType("System.Int32"));
		dataTable.Columns.Add("Address", Type.GetType("System.Int32"));
		string text = TblObject.Select("Code='6'")[0][1].ToString();
		DataSet2.R_EventBoxRow[] array = (DataSet2.R_EventBoxRow[])dsResult.R_EventBox.Select("Object='" + text + "'");
		DataSet2.R_EventBoxRow[] array2 = array;
		foreach (DataSet2.R_EventBoxRow r_EventBoxRow in array2)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["X"] = r_EventBoxRow.X;
			dataRow["Y"] = r_EventBoxRow.Y;
			dataRow["Z"] = r_EventBoxRow.Z;
			dataRow["Address"] = r_EventBoxRow.Address;
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static byte[] GetSaveFile(string mfile)
	{
		Encoding @default = Encoding.Default;
		byte[] array = File.ReadAllBytes(mfile);
		if (@default.GetString(new byte[4]
		{
			array[0],
			array[1],
			array[2],
			array[3]
		}) == "H3SV")
		{
			return array;
		}
		return Decompress(array);
	}

	private void GetFileInfo()
	{
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		string text2 = "";
		Encoding @default = Encoding.Default;
		Chrn = 0;
		try
		{
			decmp = GetSaveFile(CommonSetting.MyFile);
			if (decmp[48] == 28)
			{
				text = "SoD";
			}
			else if (decmp[48] == 21)
			{
				text = "AB";
			}
			else if (decmp[48] == 14)
			{
				text = "RoE";
			}
			else
			{
				if (decmp[49] != 29)
				{
					MessageBox.Show(errorMapVer + "  (" + decmp[48] + ")", titleMapVer + "  -  " + Path.GetFileName(CommonSetting.MyFile), (MessageBoxButtons)0, (MessageBoxIcon)16);
					((Control)this).Text = CommonSetting.MyName;
					if (ot3)
					{
						tsButtonOT.Enabled = false;
					}
					if (spt)
					{
						tsButtonSPT.Enabled = false;
					}
					tsButtonStart.Enabled = false;
					tsButtonRefresh.Enabled = false;
					return;
				}
				text = "CHRON";
				Chrn++;
			}
			map.Size = 53 + Chrn;
			map.Side = map.Size + 4;
			MapSize = decmp[map.Size];
			MapSide = decmp[map.Side];
			int num = map.Side + 3;
			int num2 = (decmp[map.Side + 2] << 8) + decmp[map.Side + 1];
			for (int i = 0; i < num2; i++)
			{
				text2 += @default.GetString(new byte[1] { decmp[num + i] });
			}
			((Control)this).Text = CommonSetting.MyName + "  -  " + Path.GetFileName(CommonSetting.MyFile) + "   [" + text + " " + MapSize + "x" + MapSize + "x" + MapSide + "]   «" + text2 + "»";
			num += num2;
			num2 = (decmp[num + 1] << 8) + decmp[num];
			map.Data = num + num2 + 2;
			if (ot3)
			{
				tsButtonOT.Enabled = false;
			}
			if (spt)
			{
				tsButtonSPT.Enabled = false;
			}
			tsButtonStart.Enabled = true;
			tsButtonRefresh.Enabled = false;
			((ToolStripItem)tsLabelStatusL).Text = sStart;
			ScanStatus(visible: true);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source + "  -  " + Path.GetFileName(CommonSetting.MyFile), (MessageBoxButtons)0, (MessageBoxIcon)16);
			((Control)this).Text = CommonSetting.MyName;
			if (ot3)
			{
				tsButtonOT.Enabled = false;
			}
			if (spt)
			{
				tsButtonSPT.Enabled = false;
			}
			tsButtonStart.Enabled = false;
			tsButtonRefresh.Enabled = false;
		}
		finally
		{
			fRowFlag = false;
			DbClear();
		}
	}

	private void tsButtonFile_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		if (Registry.RegistryKeyExist("LastDir"))
		{
			((FileDialog)openFileDialog1).InitialDirectory = Registry.GetRegistryValue("LastDir", "Path");
		}
		if ((int)((CommonDialog)openFileDialog1).ShowDialog() == 1)
		{
			Registry.SetRegistryValue("LastDir", "Path", Path.GetDirectoryName(((FileDialog)openFileDialog1).FileName));
			CommonSetting.MyFile = ((FileDialog)openFileDialog1).FileName;
			GetFileInfo();
			if (!chiStart.Checked)
			{
				tsButtonStart_ItemClick(sender, e);
			}
		}
	}

	private void ScanStatus(bool visible)
	{
		if (!visible)
		{
			if (ot3)
			{
				tsButtonOT.Enabled = true;
			}
			if (spt)
			{
				tsButtonSPT.Enabled = SPTEnable();
			}
			tsButtonStop.Enabled = false;
			tsButtonStart.Enabled = false;
			tsButtonRefresh.Enabled = true;
			tsButtonFile.Enabled = true;
			((ToolStripItem)tsLabelStatusL).Text = sReady;
		}
	}

	private bool SPTEnable()
	{
		int num = map.SaveName;
		for (int i = 1; i < 59; i++)
		{
			if (aSPTdecmp[num + i] == 46)
			{
				num += i;
				break;
			}
		}
		Encoding aSCII = Encoding.ASCII;
		if (Path.GetExtension(CommonSetting.MyFile).ToUpper() != ".GM1")
		{
			return false;
		}
		string @string = aSCII.GetString(new byte[3]
		{
			aSPTdecmp[num + 1],
			aSPTdecmp[num + 2],
			aSPTdecmp[num + 3]
		});
		if (@string.ToUpper() != "GM1")
		{
			return false;
		}
		if (aSCII.GetString(new byte[1] { aSPTdecmp[4] }).ToUpper() != "G")
		{
			return false;
		}
		if (ScanMap(num))
		{
			return false;
		}
		if (ScanSave(num))
		{
			return false;
		}
		if (ScanMaps(num))
		{
			return false;
		}
		return true;
	}

	private bool ScanMap(int fn)
	{
		Encoding aSCII = Encoding.ASCII;
		for (int i = 212; i < fn; i++)
		{
			string @string = aSCII.GetString(new byte[3]
			{
				aSPTdecmp[i],
				aSPTdecmp[i + 1],
				aSPTdecmp[i + 2]
			});
			if (@string.ToUpper() == "H3C")
			{
				return true;
			}
		}
		return false;
	}

	private bool ScanSave(int fn)
	{
		Encoding aSCII = Encoding.ASCII;
		for (int i = 212; i < fn; i++)
		{
			string @string = aSCII.GetString(new byte[3]
			{
				aSPTdecmp[i],
				aSPTdecmp[i + 1],
				aSPTdecmp[i + 2]
			});
			if (@string.ToUpper() == "CGM")
			{
				return true;
			}
		}
		return false;
	}

	private bool ScanMaps(int fn)
	{
		int num = StartMap(fn);
		if (num < 212)
		{
			return true;
		}
		for (int i = num; i < fn; i++)
		{
			if (aSPTdecmp[i] == 0 && aSPTdecmp[i + 1] == 109 && aSPTdecmp[i + 2] == 97 && aSPTdecmp[i + 3] == 112 && aSPTdecmp[i + 4] == 115 && aSPTdecmp[i + 5] == 0)
			{
				return false;
			}
		}
		return true;
	}

	private int StartMap(int fn)
	{
		Encoding aSCII = Encoding.ASCII;
		for (int i = 212; i < fn; i++)
		{
			if (aSPTdecmp[i] == 46)
			{
				string @string = aSCII.GetString(new byte[3]
				{
					aSPTdecmp[i + 1],
					aSPTdecmp[i + 2],
					aSPTdecmp[i + 3]
				});
				if (@string.ToUpper() == "H3M")
				{
					return i + 3;
				}
			}
		}
		return -1;
	}

	private void tsButtonStart_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (CommonSetting.MyFile != "")
		{
			if (sender != null)
			{
				SaveRecent();
			}
			else if (sender == null)
			{
				decmp = GetSaveFile(CommonSetting.MyFile);
			}
			tsButtonStart.Enabled = false;
			tsButtonFile.Enabled = false;
			tsButtonStop.Enabled = true;
			BindingControl(on: false);
			fStopFlag = false;
			myThread = new Thread(Scanner);
			myThread.IsBackground = true;
			myThread.Priority = ThreadPriority.BelowNormal;
			myThread.Start();
		}
	}

	private void tsButtonStop_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (myThread == null)
		{
			return;
		}
		fStopFlag = true;
		myThread.Abort();
		if (ot3)
		{
			tsButtonOT.Enabled = false;
		}
		if (spt)
		{
			tsButtonSPT.Enabled = false;
		}
		tsButtonStart.Enabled = true;
		tsButtonFile.Enabled = true;
		tsButtonRefresh.Enabled = false;
		tsButtonStop.Enabled = false;
		((ToolStripItem)tsLabelStatusL).Text = sStop;
		fRowFlag = false;
		try
		{
			DbClear();
		}
		catch
		{
			try
			{
				DbClear();
			}
			catch
			{
			}
		}
	}

	private void tsButtonRefresh_ItemClick(object sender, ItemClickEventArgs e)
	{
		foreach (GridControl item in aGrid)
		{
			string value = null;
			GridView gridView = (GridView)item.FocusedView;
			if (gridView.FocusedRowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(gridView.FocusedRowHandle);
				if (!gridView.Columns.Contains(gridView.Columns.ColumnByFieldName("X")))
				{
					value = ((!(gridView.Name == "vwAllTimer")) ? dataRow["ID"].ToString() : ((dataRow["ID"] == DBNull.Value) ? dataRow["Day"].ToString() : (dataRow["ID"].ToString() + ":" + dataRow["Day"].ToString())));
				}
				else if (dataRow["X"] != DBNull.Value)
				{
					value = ((!gridView.Columns.Contains(gridView.Columns.ColumnByFieldName("Slot"))) ? (dataRow["X"].ToString() + ";" + dataRow["Y"].ToString() + ";" + dataRow["Z"].ToString()) : ((!(gridView.Name == "vwAllSpell")) ? (dataRow["X"].ToString() + ";" + dataRow["Y"].ToString() + ";" + dataRow["Z"].ToString() + ";" + dataRow["Slot"].ToString()) : ((dataRow["Hero"] == DBNull.Value) ? (dataRow["X"].ToString() + ";" + dataRow["Y"].ToString() + ";" + dataRow["Z"].ToString() + ";" + dataRow["Slot"].ToString()) : (dataRow["X"].ToString() + ";" + dataRow["Y"].ToString() + ";" + dataRow["Z"].ToString() + ";" + dataRow["ID"].ToString() + ":" + dataRow["Spell"].ToString()))));
				}
				else if (gridView.Name == "vwAllSpell")
				{
					value = dataRow["ID"].ToString() + ":" + dataRow["Spell"].ToString();
				}
				else if (dataRow["Slot"] != DBNull.Value)
				{
					value = dataRow["X"].ToString() + ";" + dataRow["Slot"].ToString();
				}
				else if (dataRow["Hero"] != DBNull.Value)
				{
					value = dataRow["X"].ToString() + ";" + dataRow["Hero"].ToString() + "," + dataRow["Doll"].ToString();
				}
			}
			if (hFR.Count > 0)
			{
				hFR[gridView.Name] = value;
			}
			else
			{
				hFR.Add(gridView.Name, value);
			}
		}
		fRowFlag = false;
		DbClear();
		if (ot3)
		{
			tsButtonOT.Enabled = false;
		}
		if (spt)
		{
			tsButtonSPT.Enabled = false;
		}
		tsButtonRefresh.Enabled = false;
		ScanStatus(visible: true);
		tsButtonStart_ItemClick(null, e);
		fRowFlag = true;
	}

	private void tsButtonHelp_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		string text = Path.GetDirectoryName(Application.ExecutablePath) + "\\Readme.txt";
		if (File.Exists(text))
		{
			try
			{
				Process.Start(text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}
	}

	private void tsAbout_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		frmAbout frmAbout2 = new frmAbout();
		((Form)frmAbout2).ShowDialog();
	}

	private void SetFocusedRow(object sender, FocusedRowChangedEventArgs e)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (!fRowFlag || e.PrevFocusedRowHandle >= 0)
		{
			return;
		}
		GridView gridView = (GridView)sender;
		if (hFR[gridView.Name] == null)
		{
			return;
		}
		string[] array = ((string)hFR[gridView.Name]).Split(new char[1] { ';' });
		hFR[gridView.Name] = null;
		DataView dataView = (DataView)((BindingSource)gridView.DataSource).SyncRoot;
		int num = -1;
		if (array.Length == 1)
		{
			string[] array2 = array[0].Split(new char[1] { ':' });
			if (array2.Length == 1)
			{
				for (int i = 0; i < dataView.Count; i++)
				{
					DataRowView dataRowView = dataView[i];
					if (dataRowView["ID"] != DBNull.Value)
					{
						if (int.Parse(array[0]) == (int)dataRowView["ID"])
						{
							num = gridView.GetRowHandle(i);
							break;
						}
					}
					else if (gridView.Name == "vwAllTimer" && int.Parse(array[0]) == (int)dataRowView["Day"])
					{
						num = gridView.GetRowHandle(i);
						break;
					}
				}
			}
			else if (array2.Length == 2)
			{
				for (int j = 0; j < dataView.Count; j++)
				{
					DataRowView dataRowView2 = dataView[j];
					if (dataRowView2["ID"] == DBNull.Value)
					{
						continue;
					}
					if (gridView.Name == "vwAllTimer")
					{
						if (int.Parse(array2[0]) == (int)dataRowView2["ID"] && dataRowView2["Day"].ToString() == array2[1])
						{
							num = gridView.GetRowHandle(j);
							break;
						}
					}
					else if (int.Parse(array2[0]) == (int)dataRowView2["ID"] && dataRowView2["Spell"].ToString() == array2[1])
					{
						num = gridView.GetRowHandle(j);
						break;
					}
				}
			}
		}
		else if (array.Length == 2)
		{
			for (int k = 0; k < dataView.Count; k++)
			{
				DataRowView dataRowView3 = dataView[k];
				if (dataRowView3["X"] != DBNull.Value)
				{
					continue;
				}
				if (dataRowView3["Slot"] != DBNull.Value)
				{
					if (int.TryParse(array[1], out var result) && result == (int)dataRowView3["Slot"])
					{
						num = gridView.GetRowHandle(k);
						break;
					}
				}
				else if (dataRowView3["Hero"] != DBNull.Value)
				{
					string[] array3 = array[1].Split(new char[1] { ',' });
					if (dataRowView3["Hero"].ToString() == array3[0] && dataRowView3["Doll"].ToString() == array3[1])
					{
						num = gridView.GetRowHandle(k);
						break;
					}
				}
			}
		}
		else if (array.Length == 3)
		{
			for (int l = 0; l < dataView.Count; l++)
			{
				DataRowView dataRowView4 = dataView[l];
				if (int.Parse(array[0]) == (int)dataRowView4["X"] && int.Parse(array[1]) == (int)dataRowView4["Y"] && int.Parse(array[2]) == (int)dataRowView4["Z"])
				{
					num = gridView.GetRowHandle(l);
					break;
				}
			}
		}
		else if (array.Length == 4)
		{
			for (int m = 0; m < dataView.Count; m++)
			{
				DataRowView dataRowView5 = dataView[m];
				if (dataRowView5["X"] == DBNull.Value || int.Parse(array[0]) != (int)dataRowView5["X"] || int.Parse(array[1]) != (int)dataRowView5["Y"] || int.Parse(array[2]) != (int)dataRowView5["Z"])
				{
					continue;
				}
				if (array[3] != "")
				{
					if (gridView.Name == "vwAllSpell")
					{
						string[] array4 = array[3].Split(new char[1] { ':' });
						if (array4.Length == 2)
						{
							if (dataRowView5["ID"] != DBNull.Value && int.Parse(array4[0]) == (int)dataRowView5["ID"] && dataRowView5["Spell"] != DBNull.Value && array4[1] == (string)dataRowView5["Spell"])
							{
								num = gridView.GetRowHandle(m);
								break;
							}
						}
						else if (dataRowView5["Slot"] != DBNull.Value && int.Parse(array[3]) == (int)dataRowView5["Slot"])
						{
							num = gridView.GetRowHandle(m);
							break;
						}
					}
					else if (dataRowView5["Slot"] != DBNull.Value && int.Parse(array[3]) == (int)dataRowView5["Slot"])
					{
						num = gridView.GetRowHandle(m);
						break;
					}
					continue;
				}
				num = gridView.GetRowHandle(m);
				break;
			}
		}
		if (num >= 0)
		{
			if (e.FocusedRowHandle >= 0)
			{
				gridView.UnselectRow(e.FocusedRowHandle);
			}
			gridView.FocusedRowHandle = num;
			gridView.SelectRow(num);
			gridView.MakeRowVisible(num, invalidate: true);
		}
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

	private void tsLabelLink_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ToolStripLabel val = (ToolStripLabel)sender;
		OpenLink((string)((ToolStripItem)val).Tag);
		val.LinkVisited = true;
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

	private void MenuHandlerRecent(object sender, ItemClickEventArgs e)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and I4
		if (File.Exists(e.Item.Tag.ToString()))
		{
			CommonSetting.MyFile = e.Item.Tag.ToString();
			GetFileInfo();
			if (!chiStart.Checked)
			{
				tsButtonStart_ItemClick(sender, e);
			}
		}
		else if ((int)MessageBox.Show(sFile + " " + e.Item.Tag.ToString() + " " + errorExistFile, Application.ProductName + " - " + titleExistFile, (MessageBoxButtons)4, (MessageBoxIcon)16) == 6)
		{
			RemoveRecentFile(e.Item.Tag.ToString());
		}
	}

	private void SaveRecent()
	{
		if (Registry.RegistryKeyExist("Recent"))
		{
			string registryValue = Registry.GetRegistryValue("Recent", "Files");
			string[] array = registryValue.Split(new string[1] { "," }, StringSplitOptions.None);
			if (array.Length > 0)
			{
				int num = registryValue.IndexOf(CommonSetting.MyFile);
				if (num < 0)
				{
					if (array.Length < int.Parse(biRecent.EditValue.ToString()))
					{
						Registry.SetRegistryValue("Recent", "Files", CommonSetting.MyFile + "," + registryValue);
					}
					else
					{
						Registry.SetRegistryValue("Recent", "Files", CommonSetting.MyFile + "," + registryValue.Replace("," + array[int.Parse(biRecent.EditValue.ToString()) - 1], ""));
					}
				}
				else if (num > 0)
				{
					Registry.SetRegistryValue("Recent", "Files", CommonSetting.MyFile + "," + registryValue.Remove(num - 1, CommonSetting.MyFile.Length + 1));
				}
			}
			else
			{
				Registry.SetRegistryValue("Recent", "Files", CommonSetting.MyFile);
			}
		}
		else
		{
			Registry.SetRegistryValue("Recent", "Files", CommonSetting.MyFile);
		}
	}

	private void ppmFile_BeforePopup(object sender, CancelEventArgs e)
	{
		if (Registry.RegistryKeyExist("Recent"))
		{
			string[] array = Registry.GetRegistryValue("Recent", "Files").Split(new string[1] { "," }, StringSplitOptions.None);
			if (array.Length > 0)
			{
				BarButtonItem[] array2 = new BarButtonItem[array.Length];
				ppmFile.ClearLinks();
				ppmFile.ItemLinks.Add(bButtonOpen);
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = ppmFile.Manager.Items.CreateButton(i + 1 + ".  " + Path.GetFileName(array[i]));
					array2[i].Tag = array[i];
					array2[i].ItemClick += MenuHandlerRecent;
					array2[i].Hint = Path.GetDirectoryName(array[i]);
					array2[i].Id = i;
					if (i == 0)
					{
						ppmFile.ItemLinks.Add(array2[i], beginGroup: true);
					}
					else
					{
						ppmFile.ItemLinks.Add(array2[i]);
					}
				}
				ppmFile.ItemLinks.Add(bButtonClear, beginGroup: true);
			}
			else
			{
				bButtonClear_ItemClick(null, null);
			}
		}
		else
		{
			ppmFile.ClearLinks();
			ppmFile.ItemLinks.Add(bButtonOpen);
		}
	}

	private void RemoveRecentFile(string fname)
	{
		string registryValue = Registry.GetRegistryValue("Recent", "Files");
		string[] array = registryValue.Split(new string[1] { "," }, StringSplitOptions.None);
		int num = registryValue.IndexOf(fname);
		if (num > 0)
		{
			Registry.SetRegistryValue("Recent", "Files", registryValue.Remove(num - 1, fname.Length + 1));
		}
		else if (num == 0)
		{
			if (array.Length == 1)
			{
				Registry.DeleteKey("Recent");
			}
			else
			{
				Registry.SetRegistryValue("Recent", "Files", registryValue.Remove(0, fname.Length + 1));
			}
		}
	}

	private void bButtonClear_ItemClick(object sender, ItemClickEventArgs e)
	{
		Registry.DeleteKey("Recent");
		ppmFile.ClearLinks();
		ppmFile.ItemLinks.Add(bButtonOpen);
	}

	private void chiStart_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (chiStart.Checked)
		{
			tsButtonStart.Visibility = BarItemVisibility.Always;
			Registry.SetRegistryValue("MainForm", "StartButton", "1");
		}
		else
		{
			tsButtonStart.Visibility = BarItemVisibility.Never;
			Registry.SetRegistryValue("MainForm", "StartButton", "0");
		}
	}

	private void biRecent_EditValueChanged(object sender, EventArgs e)
	{
		if (Registry.RegistryKeyExist("Recent"))
		{
			string[] array = Registry.GetRegistryValue("Recent", "Files").Split(new string[1] { "," }, StringSplitOptions.None);
			if (array.Length > int.Parse(biRecent.EditValue.ToString()))
			{
				string text = array[0];
				for (int i = 1; i < int.Parse(biRecent.EditValue.ToString()); i++)
				{
					text = text + "," + array[i];
				}
				Registry.SetRegistryValue("Recent", "Files", text);
			}
		}
		Registry.SetRegistryValue("MainForm", "RecentFiles", biRecent.EditValue.ToString());
	}

	private void SetLabelStatus(string str)
	{
		((ToolStripItem)tsLabelStatusL).Text = str;
	}

	private void ScanComplete()
	{
		dsResult.AcceptChanges();
		BindingControl(on: true);
		ScanStatus(visible: false);
		((ToolStripItem)tsLabelGrail).Text = days + new string(' ', 10) + TblArt.Rows[2]["Name"].ToString() + " - " + grail;
	}

	private void ScanError()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show(errorScaner, "ProspectorRT - " + sError, (MessageBoxButtons)0, (MessageBoxIcon)16);
		RemoveRecentFile(CommonSetting.MyFile);
		tsButtonStop_ItemClick(null, null);
	}

	private void GetSenseRegion()
	{
		int num = -1;
		int mapSize = MapSize;
		int num2 = mapSize * mapSize;
		int num3 = num2 * (1 + MapSide) - 1;
		int num4 = GetStart();
		do
		{
			num++;
			num4 += 18;
			num4 += ((decmp[num4 + 1] << 8) + decmp[num4]) * 4 + 4;
		}
		while (num != num3);
		int num5 = (decmp[num4 + 1] << 8) + decmp[num4];
		num4 += 4;
		for (int i = 0; i < num5; i++)
		{
			num4 += (decmp[num4 + 1] << 8) + decmp[num4] + 35;
		}
		ObjectNumber = (decmp[num4 + 1] << 8) + decmp[num4];
		map.EventBox = num4 + ObjectNumber * 5 + 4;
		map.ArtRes = ScanObjectContent(map.EventBox, ScanEventBoxContent);
		map.Monstr = ScanObjectContent(map.ArtRes, ScanArtResContent);
		map.SeerHut = ScanMonstrContent(map.Monstr);
		map.PassGuard = ScanObjectContent(map.SeerHut, ScanSeerHutContent);
		map.MapTimedEvent = ScanObjectContent(map.PassGuard, ScanPassGuardContent);
		map.TownsTimedEvent = ScanMapTimedEvents(map.MapTimedEvent);
		map.BottleSign = ScanTownsTimedEvents(map.TownsTimedEvent);
		map.Mine = ScanBottleSignContent(map.BottleSign);
		map.Dwelling = map.Mine + decmp[map.Mine] * 62 + 1;
		map.Garrison = map.Dwelling + decmp[map.Dwelling] * 75 + 2;
		map.UnknownVarReg = map.Garrison + decmp[map.Garrison] * 61 + 1;
		map.UnknownFixedReg = map.UnknownVarReg + decmp[map.UnknownVarReg] * 28 + 1;
		map.Color = map.UnknownFixedReg + 49;
		map.Town = map.Color + 1160;
		map.Hero = ScanTownsContent(map.Town);
		map.HeroState = ScanHeroesContent(map.Hero);
		map.CurrentState = map.HeroState + HeroCount * 2;
		map.BitField = map.CurrentState + 130 - Chrn;
		map.TwoWayMonolith = map.BitField + MapSize * MapSize * (2 + 2 * MapSide);
		map.SubTerGate = ScanMonolithWhirlpool(map.TwoWayMonolith);
		map.SubTerGatePair = map.SubTerGate + ((decmp[map.SubTerGate + 1] << 8) + decmp[map.SubTerGate]) * 4 + 2;
		map.Univer = map.SubTerGatePair + ((decmp[map.SubTerGatePair + 1] << 8) + decmp[map.SubTerGatePair]) * 4 + 2;
		map.Bank = map.Univer + decmp[map.Univer] * 16 + 2;
		map.Motions = ScanBankContent(map.Bank);
	}

	private void Scanner()
	{
		try
		{
			((Control)this).Invoke((Delegate)new SetLabelCange(SetLabelStatus), new object[1] { sBegin });
			aHeroID = new int[HeroCount];
			aMonstrID = new int[256];
			GetSenseRegion();
			aObjectID = new byte[ObjectNumber];
			int num = -1;
			int num2 = 0;
			int l = 0;
			int mapSize = MapSize;
			int num3 = mapSize * mapSize;
			int num4 = num3 * (1 + MapSide) - 1;
			int num5 = map.Start;
			do
			{
				int loc = decmp[num5];
				num++;
				if (MapSide == 1 && num >= num3 && num2 == 0)
				{
					num2 = 1;
					l = num3;
				}
				num5 += 7;
				if (decmp[num5] == 16 || decmp[num5] == 18 || (decmp[num5] == 17 && decmp[num5 - 7] == 9))
				{
					IsObject(num5 + 1, num, num2, l, loc);
				}
				else if (decmp[num5 + 1] == 26 && (decmp[num5] == 0 || decmp[num5] == 2))
				{
					IsObject(num5 + 1, num, num2, l, loc);
				}
				num5 += 11;
				num5 += ((decmp[num5 + 1] << 8) + decmp[num5]) * 4 + 4;
			}
			while (num != num4);
			((Control)this).Invoke((Delegate)new SetLabelCange(SetLabelStatus), new object[1] { sBegin + "." });
			GetMarketContent();
			AnalysisContent(map.EventBox, TblEventBox, EventBoxContent);
			AnalysisContent(map.ArtRes, TblArtRes, ArtResContent);
			AnalysisMonstrContent();
			AnalysisContent(map.SeerHut, TblSeerHut, SeerHutContent);
			AnalysisContent(map.PassGuard, TblPassGuard, PassGuardContent);
			((Control)this).Invoke((Delegate)new SetLabelCange(SetLabelStatus), new object[1] { sBegin + ".." });
			GetTimedEvents();
			GetGarrisonContent();
			GetColorContent();
			GetTimerApply();
			GetEventApply();
			GetTownContent();
			GetHeroesContent();
			if (dsResult.R_Prison.Rows.Count > 0)
			{
				foreach (DataSet2.R_PrisonRow row in dsResult.R_Prison.Rows)
				{
					GetPrisonHero(row);
				}
			}
			if (TblSeerHut.Rows.Count > 0)
			{
				SeerHutContent2();
			}
			if (TblPassGuard.Rows.Count > 0)
			{
				PassGuardContent2();
			}
			GetCurrentState();
			GetPairSubterraneanGate();
			GetUniverContent();
			GetBankContent();
			GetAllSpell();
			GetAllSkill();
			((Control)this).Invoke((Delegate)new SetLabelCange(SetLabelStatus), new object[1] { sBegin + "..." });
			GetExperience();
			DataSet2.R_AllArtsRow[] array = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("Artefact IS NULL");
			DataSet2.R_AllArtsRow[] array2 = array;
			foreach (DataSet2.R_AllArtsRow r_AllArtsRow in array2)
			{
				r_AllArtsRow.Delete();
			}
			if (spt)
			{
				aSPTdecmp = decmp;
			}
			((Control)this).Invoke((Delegate)new ScanCompleteProc(ScanComplete));
		}
		catch
		{
			if (!fStopFlag)
			{
				((Control)this).Invoke((Delegate)new ScanErrorProc(ScanError));
			}
			else
			{
				fStopFlag = false;
			}
		}
	}

	private void GetAlliance()
	{
		int num = 0;
		int num2 = map.Data + 65;
		for (int num3 = map.Teams - 9; num3 > num2; num3--)
		{
			if (decmp[num3] < 8 && decmp[num3] > 1 && FindAlly(num3))
			{
				num = num3;
				break;
			}
		}
		if (num > 0)
		{
			int num4 = decmp[num + human + 1];
			aAlliance = new int[NumAlly(num, num4)];
			if (aAlliance.Length <= 0)
			{
				return;
			}
			int num5 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (i != human && decmp[num + i + 1] == num4)
				{
					aAlliance[num5] = i;
					num5++;
				}
			}
		}
		else
		{
			aAlliance = new int[0];
		}
	}

	private bool FindAlly(int s)
	{
		int num = decmp[s];
		int num2 = 0;
		for (int i = 1; i < 9; i++)
		{
			if (decmp[s + i] > num)
			{
				return false;
			}
			num2 += decmp[s + i];
		}
		if (num2 <= SumAlly(num - 1, out var min) && num2 >= min)
		{
			return true;
		}
		return false;
	}

	private int SumAlly(int num, out int min)
	{
		switch (num)
		{
		case 1:
			min = 1;
			return 7;
		case 2:
			min = 3;
			return 13;
		case 3:
			min = 6;
			return 18;
		case 4:
			min = 10;
			return 22;
		case 5:
			min = 15;
			return 25;
		case 6:
			min = 21;
			return 27;
		default:
			min = 0;
			return 255;
		}
	}

	private int NumAlly(int teams, int hTeam)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (i != human && decmp[teams + i + 1] == hTeam)
			{
				num++;
			}
		}
		return num;
	}

	private bool IsEquals(int s, int cnt, int num)
	{
		for (int i = 0; i < cnt; i++)
		{
			if (decmp[s + i] != num)
			{
				return false;
			}
		}
		return true;
	}

	private void GetExperience()
	{
		GetAlliance();
		string text = "";
		string text2 = "";
		DataSet2.R_AllExperienceDataTable r_AllExperienceDataTable = new DataSet2.R_AllExperienceDataTable();
		DataSet2.R_AllExperienceRow[] array = (DataSet2.R_AllExperienceRow[])dsResult.R_AllExperience.Select();
		DataSet2.R_EventBoxRow[] array2 = (DataSet2.R_EventBoxRow[])dsResult.R_EventBox.Select("Guard IS NOT NULL OR Experience IS NOT NULL");
		DataSet2.R_SeerHutRow[] array3 = (DataSet2.R_SeerHutRow[])dsResult.R_SeerHut.Select("Reward LIKE '%" + aReward[0] + "%'");
		DataSet2.R_SpellRow[] array4 = (DataSet2.R_SpellRow[])dsResult.R_Spell.Select("Guard IS NOT NULL");
		DataSet2.R_ArtRow[] array5 = (DataSet2.R_ArtRow[])dsResult.R_Art.Select("Guard IS NOT NULL");
		DataSet2.R_ResourceRow[] array6 = (DataSet2.R_ResourceRow[])dsResult.R_Resource.Select("Guard IS NOT NULL");
		DataSet2.R_ChestRow[] array7 = (DataSet2.R_ChestRow[])dsResult.R_Chest.Select("HP IS NOT NULL");
		DataSet2.R_BankRow[] array8 = (DataSet2.R_BankRow[])dsResult.R_Bank.Select();
		DataSet2.R_MonstrRow[] array9 = (DataSet2.R_MonstrRow[])dsResult.R_Monstr.Select();
		DataSet2.R_TownRow[] array10 = (DataSet2.R_TownRow[])dsResult.R_Town.Select("Garrison IS NOT NULL");
		DataSet2.R_GarrisonRow[] array11 = (DataSet2.R_GarrisonRow[])dsResult.R_Garrison.Select("Guard IS NOT NULL");
		DataSet2.R_HeroesRow[] array12 = (DataSet2.R_HeroesRow[])dsResult.R_Heroes.Select("Monster IS NOT NULL AND Color IS NOT NULL AND Place <> '" + aPlace[1] + "'");
		text = TblObject.Select("Code='53'")[0][1].ToString();
		text2 = TblMonsters.Rows.Find(70)[1].ToString();
		for (int i = map.Mine; i < map.Dwelling; i++)
		{
			if (decmp[i] == 70 && decmp[i - 1] == 1 && decmp[i - 2] > 0 && decmp[i - 2] < 7 && decmp[i - 3] == byte.MaxValue && decmp[i + 1] == 0 && decmp[i + 2] == 0 && decmp[i + 3] == 0 && IsEquals(i + 4, 24, 255) && decmp[i + 28] > 99 && decmp[i + 28] < 250 && IsEquals(i + 29, 27, 0))
			{
				DataSet2.R_AllExperienceRow r_AllExperienceRow = r_AllExperienceDataTable.NewR_AllExperienceRow();
				r_AllExperienceRow.X = decmp[i + 56];
				r_AllExperienceRow.Y = decmp[i + 57];
				r_AllExperienceRow.Z = decmp[i + 58];
				r_AllExperienceRow.Locality = ((DataSet2.R_MineRow)dsResult.R_Mine.Select("X='" + decmp[i + 56] + "' AND Y='" + decmp[i + 57] + "' AND Z='" + decmp[i + 58] + "'")[0]).Locality;
				r_AllExperienceRow.Object = text;
				r_AllExperienceRow.Resource = aMine[decmp[i - 2]];
				r_AllExperienceRow.Guard = text2 + " " + decmp[i + 28];
				int xP = (r_AllExperienceRow.HP = 5 * decmp[i + 28]);
				r_AllExperienceRow.XP = xP;
				GetPercent(r_AllExperienceRow);
				r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow);
				i += 59;
			}
		}
		for (int j = map.Dwelling; j < map.Garrison; j++)
		{
			if (decmp[j] == 17)
			{
				DataRow dataRow = TblDwelling.Rows.Find(decmp[j + 1]);
				if (dataRow != null && (int)dataRow[2] == decmp[j + 2] && (int)dataRow[2] == decmp[j + 17] && decmp[j + 3] == byte.MaxValue && decmp[j + 4] == byte.MaxValue && decmp[j + 5] == byte.MaxValue && decmp[j + 6] > 0 && decmp[j + 6] < 5 && IsEquals(j + 7, 7, 0) && decmp[j + 18] == 0 && decmp[j + 19] == 0 && decmp[j + 20] == 0 && IsEquals(j + 21, 24, 255) && decmp[j + 6] * 3 == decmp[j + 45] && IsEquals(j + 46, 27, 0))
				{
					DataSet2.R_MineRow[] array13 = (DataSet2.R_MineRow[])dsResult.R_Mine.Select("X='" + decmp[j + 14] + "' AND Y='" + decmp[j + 15] + "' AND Z='" + decmp[j + 16] + "'");
					if (array13.Length > 0)
					{
						DataSet2.R_AllExperienceRow r_AllExperienceRow2 = r_AllExperienceDataTable.NewR_AllExperienceRow();
						r_AllExperienceRow2.X = decmp[j + 14];
						r_AllExperienceRow2.Y = decmp[j + 15];
						r_AllExperienceRow2.Z = decmp[j + 16];
						r_AllExperienceRow2.Locality = array13[0].Locality;
						r_AllExperienceRow2.Object = dataRow[1].ToString();
						DataRow dataRow2 = TblMonsters.Rows.Find(decmp[j + 2]);
						r_AllExperienceRow2.Guard = dataRow2[1].ToString() + " " + decmp[j + 45];
						int xP2 = (r_AllExperienceRow2.HP = (int)dataRow2[3] * decmp[j + 45]);
						r_AllExperienceRow2.XP = xP2;
						GetPercent(r_AllExperienceRow2);
						r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow2);
					}
					j += 74;
				}
			}
			else
			{
				if (decmp[j] != 20)
				{
					continue;
				}
				if (decmp[j + 1] == 1)
				{
					if (decmp[j + 2] == 32 && decmp[j + 3] == 33 && decmp[j + 4] == 116 && decmp[j + 5] == 117 && decmp[j + 6] == 6 && decmp[j + 7] == 0 && decmp[j + 8] == 6 && decmp[j + 9] == 0 && decmp[j + 10] == 3 && decmp[j + 11] == 0 && decmp[j + 12] == 2 && decmp[j + 13] == 0 && decmp[j + 17] == 116 && decmp[j + 21] == 117 && IsEquals(j + 25, 20, 255) && decmp[j + 45] == 9 && decmp[j + 49] == 6 && IsEquals(j + 50, 23, 0))
					{
						DataSet2.R_AllExperienceRow r_AllExperienceRow3 = r_AllExperienceDataTable.NewR_AllExperienceRow();
						r_AllExperienceRow3.X = decmp[j + 14];
						r_AllExperienceRow3.Y = decmp[j + 15];
						r_AllExperienceRow3.Z = decmp[j + 16];
						r_AllExperienceRow3.Locality = ((DataSet2.R_MineRow)dsResult.R_Mine.Select("X='" + decmp[j + 14] + "' AND Y='" + decmp[j + 15] + "' AND Z='" + decmp[j + 16] + "'")[0]).Locality;
						r_AllExperienceRow3.Object = TblDwelling.Rows.Find(96)[1].ToString();
						r_AllExperienceRow3.Guard = TblMonsters.Rows.Find(116)[1].ToString() + " 9, " + TblMonsters.Rows.Find(117)[1].ToString() + " 6";
						int xP3 = (r_AllExperienceRow3.HP = 810);
						r_AllExperienceRow3.XP = xP3;
						GetPercent(r_AllExperienceRow3);
						r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow3);
						j += 74;
					}
				}
				else if (decmp[j + 1] == 0 && decmp[j + 2] == 112 && decmp[j + 3] == 114 && decmp[j + 4] == 113 && decmp[j + 5] == 115 && decmp[j + 6] == 6 && decmp[j + 7] == 0 && decmp[j + 8] == 5 && decmp[j + 9] == 0 && decmp[j + 10] == 4 && decmp[j + 11] == 0 && decmp[j + 12] == 6 && decmp[j + 13] == 0 && decmp[j + 17] == 113 && IsEquals(j + 21, 24, 255) && decmp[j + 45] == 12 && IsEquals(j + 46, 27, 0))
				{
					DataSet2.R_AllExperienceRow r_AllExperienceRow4 = r_AllExperienceDataTable.NewR_AllExperienceRow();
					r_AllExperienceRow4.X = decmp[j + 14];
					r_AllExperienceRow4.Y = decmp[j + 15];
					r_AllExperienceRow4.Z = decmp[j + 16];
					r_AllExperienceRow4.Locality = ((DataSet2.R_MineRow)dsResult.R_Mine.Select("X='" + decmp[j + 14] + "' AND Y='" + decmp[j + 15] + "' AND Z='" + decmp[j + 16] + "'")[0]).Locality;
					r_AllExperienceRow4.Object = TblDwelling.Rows.Find(97)[1].ToString();
					r_AllExperienceRow4.Guard = TblMonsters.Rows.Find(113)[1].ToString() + " 12";
					int xP4 = (r_AllExperienceRow4.HP = 480);
					r_AllExperienceRow4.XP = xP4;
					GetPercent(r_AllExperienceRow4);
					r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow4);
					j += 74;
				}
			}
		}
		text = TblObject.Select("Code='100'")[0][1].ToString();
		DataSet2.R_AllExperienceRow[] array14 = array;
		foreach (DataSet2.R_AllExperienceRow r_AllExperienceRow5 in array14)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow6 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow6.X = r_AllExperienceRow5.X;
			r_AllExperienceRow6.Y = r_AllExperienceRow5.Y;
			r_AllExperienceRow6.Z = r_AllExperienceRow5.Z;
			r_AllExperienceRow6.Locality = r_AllExperienceRow5.Locality;
			r_AllExperienceRow6.Object = text;
			int xP = (r_AllExperienceRow6.Experience = r_AllExperienceRow5.Experience);
			r_AllExperienceRow6.XP = xP;
			GetPercent(r_AllExperienceRow6);
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow6);
		}
		DataSet2.R_ChestRow[] array15 = array7;
		foreach (DataSet2.R_ChestRow r_ChestRow in array15)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow7 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow7.X = r_ChestRow.X;
			r_AllExperienceRow7.Y = r_ChestRow.Y;
			r_AllExperienceRow7.Z = r_ChestRow.Z;
			r_AllExperienceRow7.Locality = r_ChestRow.Locality;
			r_AllExperienceRow7.Object = r_ChestRow.Object;
			int xP = (r_AllExperienceRow7.Experience = r_ChestRow.HP);
			r_AllExperienceRow7.XP = xP;
			GetPercent(r_AllExperienceRow7);
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow7);
		}
		text = TblObject.Select("Code='26'")[0][1].ToString();
		DataSet2.R_EventBoxRow[] array16 = array2;
		foreach (DataSet2.R_EventBoxRow r_EventBoxRow in array16)
		{
			if ((r_EventBoxRow.Object == text && r_EventBoxRow.IsApplyNull()) || (r_EventBoxRow.Object == text && r_EventBoxRow.Apply.IndexOf(aColor[human]) < 0))
			{
				continue;
			}
			DataSet2.R_AllExperienceRow r_AllExperienceRow8 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow8.X = r_EventBoxRow.X;
			r_AllExperienceRow8.Y = r_EventBoxRow.Y;
			r_AllExperienceRow8.Z = r_EventBoxRow.Z;
			r_AllExperienceRow8.Locality = r_EventBoxRow.Locality;
			r_AllExperienceRow8.Object = r_EventBoxRow.Object;
			if (!r_EventBoxRow.IsGuardNull())
			{
				r_AllExperienceRow8.Guard = r_EventBoxRow.Guard;
				int xP = (r_AllExperienceRow8.HP = r_EventBoxRow.HP);
				r_AllExperienceRow8.XP = xP;
			}
			if (!r_EventBoxRow.IsExperienceNull())
			{
				r_AllExperienceRow8.Experience = r_EventBoxRow.Experience;
				if (!r_AllExperienceRow8.IsXPNull())
				{
					r_AllExperienceRow8.XP += r_EventBoxRow.Experience;
				}
				else
				{
					r_AllExperienceRow8.XP = r_EventBoxRow.Experience;
				}
			}
			GetPercent(r_AllExperienceRow8);
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow8);
		}
		text = TblObject.Select("Code='5'")[0][1].ToString();
		DataSet2.R_ArtRow[] array17 = array5;
		foreach (DataSet2.R_ArtRow r_ArtRow in array17)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow9 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow9.X = r_ArtRow.X;
			r_AllExperienceRow9.Y = r_ArtRow.Y;
			r_AllExperienceRow9.Z = r_ArtRow.Z;
			r_AllExperienceRow9.Locality = r_ArtRow.Locality;
			r_AllExperienceRow9.Object = r_ArtRow.Object;
			if (r_ArtRow.Object == text)
			{
				r_AllExperienceRow9.Art = r_ArtRow.Name;
			}
			if (!r_ArtRow.IsGuardNull())
			{
				r_AllExperienceRow9.Guard = r_ArtRow.Guard;
				int xP = (r_AllExperienceRow9.HP = r_ArtRow.HP);
				r_AllExperienceRow9.XP = xP;
				GetPercent(r_AllExperienceRow9);
			}
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow9);
		}
		DataSet2.R_ResourceRow[] array18 = array6;
		foreach (DataSet2.R_ResourceRow r_ResourceRow in array18)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow10 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow10.X = r_ResourceRow.X;
			r_AllExperienceRow10.Y = r_ResourceRow.Y;
			r_AllExperienceRow10.Z = r_ResourceRow.Z;
			r_AllExperienceRow10.Locality = r_ResourceRow.Locality;
			r_AllExperienceRow10.Object = r_ResourceRow.Object;
			if (!r_ResourceRow.IsResourceNull())
			{
				r_AllExperienceRow10.Resource = r_ResourceRow.Resource;
			}
			else if (!r_ResourceRow.IsGoldNull())
			{
				r_AllExperienceRow10.Resource = r_ResourceRow.Gold.ToString();
			}
			if (!r_ResourceRow.IsGuardNull())
			{
				r_AllExperienceRow10.Guard = r_ResourceRow.Guard;
				int xP = (r_AllExperienceRow10.HP = r_ResourceRow.HP);
				r_AllExperienceRow10.XP = xP;
				GetPercent(r_AllExperienceRow10);
			}
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow10);
		}
		DataSet2.R_BankRow[] array19 = array8;
		foreach (DataSet2.R_BankRow r_BankRow in array19)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow11 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow11.X = r_BankRow.X;
			r_AllExperienceRow11.Y = r_BankRow.Y;
			r_AllExperienceRow11.Z = r_BankRow.Z;
			r_AllExperienceRow11.Locality = r_BankRow.Locality;
			r_AllExperienceRow11.Object = r_BankRow.Name;
			if (!r_BankRow.IsGuardNull())
			{
				r_AllExperienceRow11.Guard = r_BankRow.Guard;
				int xP = (r_AllExperienceRow11.HP = r_BankRow.HP);
				r_AllExperienceRow11.XP = xP;
				GetPercent(r_AllExperienceRow11);
			}
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow11);
		}
		text = TblObject.Select("Code='54'")[0][1].ToString();
		DataSet2.R_MonstrRow[] array20 = array9;
		foreach (DataSet2.R_MonstrRow r_MonstrRow in array20)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow12 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow12.X = r_MonstrRow.X;
			r_AllExperienceRow12.Y = r_MonstrRow.Y;
			r_AllExperienceRow12.Z = r_MonstrRow.Z;
			r_AllExperienceRow12.Locality = r_MonstrRow.Locality;
			r_AllExperienceRow12.Object = text;
			int xP = (r_AllExperienceRow12.HP = r_MonstrRow.HP);
			r_AllExperienceRow12.XP = xP;
			r_AllExperienceRow12.Monster = r_MonstrRow.Name + " " + r_MonstrRow.Number;
			GetPercent(r_AllExperienceRow12);
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow12);
		}
		DataSet2.R_SpellRow[] array21 = array4;
		foreach (DataSet2.R_SpellRow r_SpellRow in array21)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow13 = r_AllExperienceDataTable.NewR_AllExperienceRow();
			r_AllExperienceRow13.X = r_SpellRow.X;
			r_AllExperienceRow13.Y = r_SpellRow.Y;
			r_AllExperienceRow13.Z = r_SpellRow.Z;
			r_AllExperienceRow13.Locality = r_SpellRow.Locality;
			r_AllExperienceRow13.Object = r_SpellRow.Object;
			r_AllExperienceRow13.Spell = r_SpellRow.Spell;
			if (!r_SpellRow.IsGuardNull())
			{
				r_AllExperienceRow13.Guard = r_SpellRow.Guard;
				int xP = (r_AllExperienceRow13.HP = r_SpellRow.HP);
				r_AllExperienceRow13.XP = xP;
				GetPercent(r_AllExperienceRow13);
			}
			r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow13);
		}
		text = TblObject.Select("Code='83'")[0][1].ToString();
		DataSet2.R_SeerHutRow[] array22 = array3;
		foreach (DataSet2.R_SeerHutRow r_SeerHutRow in array22)
		{
			if (r_SeerHutRow.Mission.IndexOf(аQuest[6]) != 0 || r_SeerHutRow.Mission.IndexOf(aColor[human]) >= 0)
			{
				DataSet2.R_AllExperienceRow r_AllExperienceRow14 = r_AllExperienceDataTable.NewR_AllExperienceRow();
				r_AllExperienceRow14.X = r_SeerHutRow.X;
				r_AllExperienceRow14.Y = r_SeerHutRow.Y;
				r_AllExperienceRow14.Z = r_SeerHutRow.Z;
				r_AllExperienceRow14.Locality = r_SeerHutRow.Locality;
				r_AllExperienceRow14.Object = text;
				r_AllExperienceRow14.Mission = r_SeerHutRow.Mission;
				int xP = (r_AllExperienceRow14.Experience = int.Parse(r_SeerHutRow.Reward.Replace(aReward[0] + " ", "")));
				r_AllExperienceRow14.XP = xP;
				GetPercent(r_AllExperienceRow14);
				r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow14);
			}
		}
		text = TblObject.Select("Code='33'")[0][1].ToString();
		DataSet2.R_GarrisonRow[] array23 = array11;
		foreach (DataSet2.R_GarrisonRow r_GarrisonRow in array23)
		{
			if (r_GarrisonRow.IsColorNull() || (!(r_GarrisonRow.Color == aColor[human]) && !IsAlly(r_GarrisonRow.Color)))
			{
				DataSet2.R_AllExperienceRow r_AllExperienceRow15 = r_AllExperienceDataTable.NewR_AllExperienceRow();
				r_AllExperienceRow15.X = r_GarrisonRow.X;
				r_AllExperienceRow15.Y = r_GarrisonRow.Y;
				r_AllExperienceRow15.Z = r_GarrisonRow.Z;
				r_AllExperienceRow15.Locality = r_GarrisonRow.Locality;
				r_AllExperienceRow15.Object = text;
				if (!r_GarrisonRow.IsColorNull())
				{
					r_AllExperienceRow15.Color = r_GarrisonRow.Color;
				}
				if (!r_GarrisonRow.IsGuardNull())
				{
					r_AllExperienceRow15.Guard = r_GarrisonRow.Guard;
					int xP = (r_AllExperienceRow15.HP = r_GarrisonRow.HP);
					r_AllExperienceRow15.XP = xP;
					GetPercent(r_AllExperienceRow15);
				}
				r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow15);
			}
		}
		text = TblObject.Select("Code='34'")[0][1].ToString();
		DataSet2.R_HeroesRow[] array24 = array12;
		foreach (DataSet2.R_HeroesRow r_HeroesRow in array24)
		{
			if (r_HeroesRow.IsColorNull() || (!(r_HeroesRow.Color == aColor[human]) && !IsAlly(r_HeroesRow.Color)))
			{
				DataSet2.R_AllExperienceRow r_AllExperienceRow16 = r_AllExperienceDataTable.NewR_AllExperienceRow();
				int num6 = -1;
				int num7 = -1;
				if (!r_HeroesRow.IsPlaceNull())
				{
					num6 = r_HeroesRow.Place.IndexOf(".");
					num7 = r_HeroesRow.Place.LastIndexOf(".");
					r_AllExperienceRow16.X = int.Parse(r_HeroesRow.Place.Substring(0, num6));
					r_AllExperienceRow16.Y = int.Parse(r_HeroesRow.Place.Substring(num6 + 1, num7 - num6 - 1));
					r_AllExperienceRow16.Z = int.Parse(r_HeroesRow.Place.Substring(num7 + 1, r_HeroesRow.Place.Length - num7 - 1));
				}
				r_AllExperienceRow16.Locality = aLocality[0];
				r_AllExperienceRow16.Object = text;
				r_AllExperienceRow16.Hero = r_HeroesRow.Hero;
				r_AllExperienceRow16.ID = r_HeroesRow.ID;
				if (!r_HeroesRow.IsColorNull())
				{
					r_AllExperienceRow16.Color = r_HeroesRow.Color;
				}
				if (!r_HeroesRow.IsMonsterNull())
				{
					r_AllExperienceRow16.Monster = r_HeroesRow.Monster;
					r_AllExperienceRow16.HP = r_HeroesRow.HP;
					r_AllExperienceRow16.Experience = 500;
					r_AllExperienceRow16.XP = r_AllExperienceRow16.HP + r_AllExperienceRow16.Experience;
					GetPercent(r_AllExperienceRow16);
				}
				r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow16);
			}
		}
		text = TblObject.Select("Code='98'")[0][1].ToString();
		DataSet2.R_TownRow[] array25 = array10;
		foreach (DataSet2.R_TownRow r_TownRow in array25)
		{
			if (r_TownRow.IsColorNull() || (!(r_TownRow.Color == aColor[human]) && !IsAlly(r_TownRow.Color)))
			{
				DataSet2.R_AllExperienceRow r_AllExperienceRow17 = r_AllExperienceDataTable.NewR_AllExperienceRow();
				r_AllExperienceRow17.X = r_TownRow.X;
				r_AllExperienceRow17.Y = r_TownRow.Y;
				r_AllExperienceRow17.Z = r_TownRow.Z;
				r_AllExperienceRow17.Locality = aLocality[0];
				r_AllExperienceRow17.Object = text;
				r_AllExperienceRow17.Town = r_TownRow.Name;
				if (!r_TownRow.IsColorNull())
				{
					r_AllExperienceRow17.Color = r_TownRow.Color;
				}
				if (!r_TownRow.IsGarrisonNull())
				{
					r_AllExperienceRow17.Guard = r_TownRow.Garrison;
					r_AllExperienceRow17.HP = r_TownRow.HP;
					r_AllExperienceRow17.Experience = 500;
					r_AllExperienceRow17.XP = r_AllExperienceRow17.HP + r_AllExperienceRow17.Experience;
					GetPercent(r_AllExperienceRow17);
				}
				r_AllExperienceDataTable.Rows.Add(r_AllExperienceRow17);
			}
		}
		dsResult.R_AllExperience.Clear();
		DataView defaultView = r_AllExperienceDataTable.DefaultView;
		defaultView.Sort = "Z, Y, X";
		foreach (DataRowView item in defaultView)
		{
			DataSet2.R_AllExperienceRow r_AllExperienceRow18 = dsResult.R_AllExperience.NewR_AllExperienceRow();
			r_AllExperienceRow18.ItemArray = item.Row.ItemArray;
			dsResult.R_AllExperience.Rows.Add(r_AllExperienceRow18);
		}
	}

	private bool IsAlly(string color)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (aColor[i] == color)
			{
				num = i;
				break;
			}
		}
		for (int j = 0; j < aAlliance.Length; j++)
		{
			if (aAlliance[j] == num)
			{
				return true;
			}
		}
		return false;
	}

	private void GetPercent(DataSet2.R_AllExperienceRow row)
	{
		row.XP5 = (int)Math.Round((double)row.XP * 1.05, 0);
		row.XP10 = (int)Math.Round((double)row.XP * 1.1, 0);
		row.XP15 = (int)Math.Round((double)row.XP * 1.15, 0);
	}

	private void AnalysisMonstrContent()
	{
		foreach (DataRow row in TblMonstr.Rows)
		{
			MonstrContent(row, aMonstrID[(int)row["Num"]]);
		}
	}

	private void AnalysisContent(int s, DataTable Tbl, ObjectContent ObjContent)
	{
		int num = 0;
		int num2 = (decmp[s + 1] << 8) + decmp[s];
		if (num2 <= 0)
		{
			return;
		}
		s += 2;
		DataView defaultView = Tbl.DefaultView;
		defaultView.Sort = "Num";
		if (num2 == Tbl.Rows.Count)
		{
			foreach (DataRowView item in defaultView)
			{
				s = ObjContent(item.Row, s) + 1;
			}
			return;
		}
		if (num2 <= Tbl.Rows.Count)
		{
			return;
		}
		foreach (DataRowView item2 in defaultView)
		{
			if (num == (int)item2.Row["Num"])
			{
				s = ObjContent(item2.Row, s) + 1;
				num++;
				continue;
			}
			for (int i = num; i < (int)item2.Row["Num"]; i++)
			{
				s = ObjContent(null, s) + 1;
				num++;
			}
			s = ObjContent(item2.Row, s) + 1;
			num++;
		}
		if (num2 > num)
		{
			for (int j = 0; j < num2 - num; j++)
			{
				s = ObjContent(null, s) + 1;
			}
		}
	}

	private void GetTimedEvents()
	{
		DataSet2.R_AllTimerDataTable tblTE = new DataSet2.R_AllTimerDataTable();
		MapTimedEvents(tblTE);
		TownsTimedEvents(tblTE);
		MapTownsTimedEvents(tblTE);
	}

	private void MapTownsTimedEvents(DataSet2.R_AllTimerDataTable tblTE)
	{
		DataView defaultView = tblTE.DefaultView;
		defaultView.Sort = "Day, Object, ID";
		foreach (DataRowView item in defaultView)
		{
			DataSet2.R_AllTimerRow r_AllTimerRow = dsResult.R_AllTimer.NewR_AllTimerRow();
			r_AllTimerRow.ItemArray = item.Row.ItemArray;
			dsResult.R_AllTimer.Rows.Add(r_AllTimerRow);
		}
	}

	private void TownsTimedEvents(DataSet2.R_AllTimerDataTable tblTE)
	{
		int townsTimedEvent = map.TownsTimedEvent;
		int num = (decmp[townsTimedEvent + 1] << 8) + decmp[townsTimedEvent];
		if (num <= 0)
		{
			return;
		}
		townsTimedEvent += 4;
		for (int i = 0; i < num; i++)
		{
			townsTimedEvent += (decmp[townsTimedEvent + 1] << 8) + decmp[townsTimedEvent];
			DataSet2.R_AllTimerRow r_AllTimerRow = tblTE.NewR_AllTimerRow();
			GetTimerRes(townsTimedEvent, r_AllTimerRow);
			GetTimerContent(townsTimedEvent, r_AllTimerRow);
			r_AllTimerRow.Day = (decmp[townsTimedEvent + 34] << 8) + decmp[townsTimedEvent + 33];
			if (decmp[townsTimedEvent + 35] > 0)
			{
				r_AllTimerRow.Repeat = decmp[townsTimedEvent + 35];
			}
			r_AllTimerRow.Object = aPlace[3];
			r_AllTimerRow.Apply = decmp[townsTimedEvent + 30] + "," + decmp[townsTimedEvent + 31] + "," + decmp[townsTimedEvent + 32];
			r_AllTimerRow.ID = decmp[townsTimedEvent + 37];
			tblTE.Rows.Add(r_AllTimerRow);
			townsTimedEvent += 60;
		}
	}

	private void MapTimedEvents(DataSet2.R_AllTimerDataTable tblTE)
	{
		int mapTimedEvent = map.MapTimedEvent;
		int num = (decmp[mapTimedEvent + 1] << 8) + decmp[mapTimedEvent];
		if (num <= 0)
		{
			return;
		}
		mapTimedEvent += 4;
		for (int i = 0; i < num; i++)
		{
			mapTimedEvent = mapTimedEvent + (decmp[mapTimedEvent + 1] << 8) + decmp[mapTimedEvent];
			DataSet2.R_AllTimerRow r_AllTimerRow = tblTE.NewR_AllTimerRow();
			GetTimerRes(mapTimedEvent, r_AllTimerRow);
			r_AllTimerRow.Day = (decmp[mapTimedEvent + 34] << 8) + decmp[mapTimedEvent + 33];
			if (decmp[mapTimedEvent + 35] > 0)
			{
				r_AllTimerRow.Repeat = decmp[mapTimedEvent + 35];
			}
			r_AllTimerRow.Object = aPlace[4];
			r_AllTimerRow.Apply = decmp[mapTimedEvent + 30] + "," + decmp[mapTimedEvent + 31] + "," + decmp[mapTimedEvent + 32];
			tblTE.Rows.Add(r_AllTimerRow);
			mapTimedEvent += 37;
		}
	}

	private void GetTimerContent(int s, DataRow row)
	{
		s += 38;
		string text = "";
		for (int i = 0; i < 6; i++)
		{
			int num = decmp[s + i];
			text = text + "," + num;
		}
		s += 8;
		string text2 = "";
		for (int j = 0; j < 7; j++)
		{
			text2 = text2 + "," + ((decmp[s + j * 2 + 1] << 8) + decmp[s + j * 2]);
		}
		if (text != "")
		{
			row["Building"] = text.Remove(0, 1);
		}
		if (text2 != "")
		{
			row["Monster"] = text2.Remove(0, 1);
		}
	}

	private void GetTimerRes(int s, DataRow row)
	{
		string text = "";
		int num = 0;
		s += 2;
		for (int i = 0; i < 6; i++)
		{
			if (decmp[s + i * 4 + 3] == byte.MaxValue)
			{
				text = text + ", " + aResource[i] + (((byte)(~decmp[s + i * 4 + 2]) << 16) + ((byte)(~decmp[s + i * 4 + 1]) << 8) + (byte)(~decmp[s + i * 4]) + 1) * -1;
			}
			else if (decmp[s + i * 4 + 2] != 0 || decmp[s + i * 4 + 1] != 0 || decmp[s + i * 4] != 0)
			{
				text = text + ", " + aResource[i] + ((decmp[s + i * 4 + 2] << 16) + (decmp[s + i * 4 + 1] << 8) + decmp[s + i * 4]);
			}
		}
		s += 24;
		num = ((decmp[s + 3] != byte.MaxValue) ? ((decmp[s + 2] << 16) + (decmp[s + 1] << 8) + decmp[s]) : ((((byte)(~decmp[s + 2]) << 16) + ((byte)(~decmp[s + 1]) << 8) + (byte)(~decmp[s]) + 1) * -1));
		if (num != 0)
		{
			row["Gold"] = num;
		}
		if (text != "")
		{
			row["Resource"] = text.Remove(0, 2);
		}
	}

	private void GetTimerTown(DataSet2.R_TownRow trow)
	{
		int num = 0;
		int num2 = 0;
		string townType = GetTownType(trow.Type);
		DataSet2.R_AllTimerRow[] array = (DataSet2.R_AllTimerRow[])dsResult.R_AllTimer.Select("ID='" + trow.ID + "'");
		DataSet2.R_AllTimerRow[] array2 = array;
		foreach (DataSet2.R_AllTimerRow r_AllTimerRow in array2)
		{
			r_AllTimerRow.Town = trow.Name;
			r_AllTimerRow.Type = trow.Type;
			r_AllTimerRow.Place = trow.X + "." + trow.Y + "." + trow.Z;
			if (!trow.IsColorNull())
			{
				r_AllTimerRow.Color = trow.Color;
			}
			string text = "";
			string[] array3 = r_AllTimerRow.Monster.Split(new string[1] { "," }, StringSplitOptions.None);
			for (int j = 0; j < 7; j++)
			{
				if (int.Parse(array3[j]) > 0)
				{
					DataRow dataRow = TblMonsters.Select("Town='" + townType + "' AND Level='" + (j + 1) + "'")[0];
					text = string.Concat(text, ", ", dataRow["Name"], " ", array3[j]);
				}
			}
			if (text != "")
			{
				r_AllTimerRow.Monster = text.Remove(0, 2);
			}
			else
			{
				r_AllTimerRow.SetMonsterNull();
			}
			string text2 = "";
			string[] array4 = r_AllTimerRow.Building.Split(new string[1] { "," }, StringSplitOptions.None);
			for (int k = 0; k < 5; k++)
			{
				int num3 = int.Parse(array4[k]);
				if (k == 1)
				{
					num |= num3;
				}
				else if (townType == "2" && k == 2)
				{
					num2 |= num3;
				}
				if (num3 <= 0)
				{
					continue;
				}
				for (int l = 0; l < 8; l++)
				{
					if ((num3 & (1 << l)) > 0)
					{
						string filterExpression = "Type='" + townType + "' AND Byte='" + (k + 1) + "' AND Bit='" + l + "'";
						DataRow dataRow2 = TblBuilding.Select(filterExpression)[0];
						if ((string)dataRow2["Building"] != "")
						{
							text2 = text2 + ", " + dataRow2["Building"];
						}
					}
				}
			}
			if (int.Parse(array4[5]) > 0)
			{
				DataRow dataRow3 = TblBuilding.Select("Type='" + townType + "' AND Byte='6' AND Bit='0'")[0];
				if ((string)dataRow3["Building"] != "")
				{
					text2 = text2 + ", " + dataRow3["Building"];
				}
			}
			if (text2 != "")
			{
				r_AllTimerRow.Building = text2.Remove(0, 2);
			}
			else
			{
				r_AllTimerRow.SetBuildingNull();
			}
			if (r_AllTimerRow.IsApplyNull())
			{
				num2 = (num = 0);
			}
		}
		trow.MageTimer = num;
		if (townType == "2")
		{
			trow.LibTimer = (num2 & 4) >> 2;
		}
	}

	private string GetTownType(string type)
	{
		for (int i = 0; i < aTown.Length; i++)
		{
			if (aTown[i] == type)
			{
				return i.ToString();
			}
		}
		return "";
	}

	private void GetEventApply()
	{
		string text = TblObject.Select("Code='26'")[0][1].ToString();
		DataSet2.R_EventBoxRow[] array = (DataSet2.R_EventBoxRow[])dsResult.R_EventBox.Select("Object='" + text + "'");
		DataSet2.R_EventBoxRow[] array2 = array;
		foreach (DataSet2.R_EventBoxRow r_EventBoxRow in array2)
		{
			string text2 = "";
			string[] array3 = r_EventBoxRow.Apply.Split(new string[1] { "," }, StringSplitOptions.None);
			int num = int.Parse(array3[0]);
			int num2 = int.Parse(array3[1]);
			for (int j = 0; j < 8; j++)
			{
				if ((num & (1 << j)) > 0 && aExistColor[j] > 0)
				{
					if (j == human)
					{
						text2 = text2 + ", " + aColor[j];
					}
					else if (j != human && num2 > 0)
					{
						text2 = text2 + ", " + aColor[j];
					}
				}
			}
			if (text2 != "")
			{
				r_EventBoxRow.Apply = text2.Remove(0, 2);
			}
			else
			{
				r_EventBoxRow.SetApplyNull();
			}
		}
	}

	private void GetTimerApply()
	{
		DataSet2.R_AllTimerRow[] array = (DataSet2.R_AllTimerRow[])dsResult.R_AllTimer.Select();
		DataSet2.R_AllTimerRow[] array2 = array;
		foreach (DataSet2.R_AllTimerRow r_AllTimerRow in array2)
		{
			string text = "";
			string[] array3 = r_AllTimerRow.Apply.Split(new string[1] { "," }, StringSplitOptions.None);
			int num = int.Parse(array3[0]);
			int num2 = int.Parse(array3[1]);
			int num3 = int.Parse(array3[2]);
			for (int j = 0; j < 8; j++)
			{
				if ((num & (1 << j)) > 0 && aExistColor[j] > 0)
				{
					if (j == human && num2 > 0)
					{
						text = text + ", " + aColor[j];
					}
					else if (j != human && num3 > 0)
					{
						text = text + ", " + aColor[j];
					}
				}
			}
			if (text != "")
			{
				r_AllTimerRow.Apply = text.Remove(0, 2);
			}
			else
			{
				r_AllTimerRow.SetApplyNull();
			}
		}
	}

	private void GetAllSkill()
	{
		string text = "";
		DataSet2.R_AllSkillDataTable r_AllSkillDataTable = new DataSet2.R_AllSkillDataTable();
		DataSet2.R_EventBoxRow[] array = (DataSet2.R_EventBoxRow[])dsResult.R_EventBox.Select("SecondarySkill IS NOT NULL");
		DataSet2.R_SkillRow[] array2 = (DataSet2.R_SkillRow[])dsResult.R_Skill.Select("Skill IS NOT NULL");
		DataSet2.R_ScholarRow[] array3 = (DataSet2.R_ScholarRow[])dsResult.R_Scholar.Select("SecondarySkill IS NOT NULL");
		DataSet2.R_SeerHutRow[] array4 = (DataSet2.R_SeerHutRow[])dsResult.R_SeerHut.Select("Reward LIKE '%.%'");
		DataSet2.R_SkillRow[] array5 = array2;
		foreach (DataSet2.R_SkillRow r_SkillRow in array5)
		{
			string[] array6 = r_SkillRow.Skill.Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int j = 0; j < array6.Length; j++)
			{
				DataSet2.R_AllSkillRow r_AllSkillRow = r_AllSkillDataTable.NewR_AllSkillRow();
				r_AllSkillRow.X = r_SkillRow.X;
				r_AllSkillRow.Y = r_SkillRow.Y;
				r_AllSkillRow.Z = r_SkillRow.Z;
				r_AllSkillRow.Locality = r_SkillRow.Locality;
				r_AllSkillRow.Object = r_SkillRow.Object;
				r_AllSkillRow.Skill = array6[j];
				r_AllSkillRow.Slot = j;
				r_AllSkillDataTable.Rows.Add(r_AllSkillRow);
			}
		}
		DataSet2.R_EventBoxRow[] array7 = array;
		foreach (DataSet2.R_EventBoxRow r_EventBoxRow in array7)
		{
			string[] array8 = r_EventBoxRow.SecondarySkill.Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int l = 0; l < array8.Length; l++)
			{
				DataSet2.R_AllSkillRow r_AllSkillRow2 = r_AllSkillDataTable.NewR_AllSkillRow();
				r_AllSkillRow2.X = r_EventBoxRow.X;
				r_AllSkillRow2.Y = r_EventBoxRow.Y;
				r_AllSkillRow2.Z = r_EventBoxRow.Z;
				r_AllSkillRow2.Locality = r_EventBoxRow.Locality;
				r_AllSkillRow2.Object = r_EventBoxRow.Object;
				r_AllSkillRow2.Slot = l;
				string[] array9 = array8[l].Split(new string[1] { "." }, StringSplitOptions.None);
				if (array9.Length > 0)
				{
					r_AllSkillRow2.Skill = array9[1];
					r_AllSkillRow2.Level = GetFullLvlSkill(array9[0] + ".");
				}
				else
				{
					r_AllSkillRow2.Skill = array8[l];
				}
				if (l == 0 && !r_EventBoxRow.IsGuardNull())
				{
					r_AllSkillRow2.Guard = r_EventBoxRow.Guard;
				}
				r_AllSkillDataTable.Rows.Add(r_AllSkillRow2);
			}
		}
		text = TblObject.Select("Code='81'")[0][1].ToString();
		DataSet2.R_ScholarRow[] array10 = array3;
		foreach (DataSet2.R_ScholarRow r_ScholarRow in array10)
		{
			DataSet2.R_AllSkillRow r_AllSkillRow3 = r_AllSkillDataTable.NewR_AllSkillRow();
			r_AllSkillRow3.X = r_ScholarRow.X;
			r_AllSkillRow3.Y = r_ScholarRow.Y;
			r_AllSkillRow3.Z = r_ScholarRow.Z;
			r_AllSkillRow3.Object = text;
			r_AllSkillRow3.Locality = r_ScholarRow.Locality;
			r_AllSkillRow3.Skill = r_ScholarRow.SecondarySkill;
			r_AllSkillRow3.Slot = 1;
			r_AllSkillDataTable.Rows.Add(r_AllSkillRow3);
		}
		text = TblObject.Select("Code='83'")[0][1].ToString();
		DataSet2.R_SeerHutRow[] array11 = array4;
		foreach (DataSet2.R_SeerHutRow r_SeerHutRow in array11)
		{
			DataSet2.R_AllSkillRow r_AllSkillRow4 = r_AllSkillDataTable.NewR_AllSkillRow();
			r_AllSkillRow4.X = r_SeerHutRow.X;
			r_AllSkillRow4.Y = r_SeerHutRow.Y;
			r_AllSkillRow4.Z = r_SeerHutRow.Z;
			r_AllSkillRow4.Object = text;
			r_AllSkillRow4.Locality = r_SeerHutRow.Locality;
			r_AllSkillRow4.Mission = r_SeerHutRow.Mission;
			r_AllSkillRow4.Slot = 1;
			string[] array12 = r_SeerHutRow.Reward.Replace(aReward[5] + " ", "").Split(new string[1] { "." }, StringSplitOptions.None);
			if (array12.Length > 0)
			{
				r_AllSkillRow4.Skill = array12[1];
				r_AllSkillRow4.Level = GetFullLvlSkill(array12[0] + ".");
			}
			else
			{
				r_AllSkillRow4.Skill = array12[0];
			}
			r_AllSkillDataTable.Rows.Add(r_AllSkillRow4);
		}
		DataView defaultView = r_AllSkillDataTable.DefaultView;
		defaultView.Sort = "Z, Y, X, Slot";
		foreach (DataRowView item in defaultView)
		{
			DataSet2.R_AllSkillRow r_AllSkillRow5 = dsResult.R_AllSkill.NewR_AllSkillRow();
			r_AllSkillRow5.ItemArray = item.Row.ItemArray;
			dsResult.R_AllSkill.Rows.Add(r_AllSkillRow5);
		}
	}

	private void GetAllSpell()
	{
		string text = "";
		DataSet2.R_AllSpellDataTable r_AllSpellDataTable = new DataSet2.R_AllSpellDataTable();
		DataSet2.R_AllSpellDataTable r_AllSpellDataTable2 = new DataSet2.R_AllSpellDataTable();
		DataSet2.R_EventBoxRow[] array = (DataSet2.R_EventBoxRow[])dsResult.R_EventBox.Select("Spell IS NOT NULL");
		DataSet2.R_ScholarRow[] array2 = (DataSet2.R_ScholarRow[])dsResult.R_Scholar.Select("Spell IS NOT NULL");
		DataSet2.R_SpellRow[] array3 = (DataSet2.R_SpellRow[])dsResult.R_Spell.Select("Spell IS NOT NULL");
		DataSet2.R_SeerHutRow[] array4 = (DataSet2.R_SeerHutRow[])dsResult.R_SeerHut.Select("Reward LIKE 'Заклинание%'");
		DataSet2.R_TownRow[] array5 = (DataSet2.R_TownRow[])dsResult.R_Town.Select("Spell IS NOT NULL");
		DataSet2.R_HeroesRow[] array6 = (DataSet2.R_HeroesRow[])dsResult.R_Heroes.Select("Spell IS NOT NULL AND (Place <> '" + aPlace[2] + "' OR Place IS NULL)");
		DataSet2.R_EventBoxRow[] array7 = array;
		foreach (DataSet2.R_EventBoxRow r_EventBoxRow in array7)
		{
			string[] array8 = r_EventBoxRow.Spell.Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int j = 0; j < array8.Length; j++)
			{
				DataSet2.R_AllSpellRow r_AllSpellRow = r_AllSpellDataTable.NewR_AllSpellRow();
				r_AllSpellRow.X = r_EventBoxRow.X;
				r_AllSpellRow.Y = r_EventBoxRow.Y;
				r_AllSpellRow.Z = r_EventBoxRow.Z;
				r_AllSpellRow.Object = r_EventBoxRow.Object;
				r_AllSpellRow.Spell = array8[j];
				r_AllSpellRow.Slot = (int)TblSpell.Select("Name='" + array8[j] + "'")[0][2];
				if (j == 0 && !r_EventBoxRow.IsGuardNull())
				{
					r_AllSpellRow.Guard = r_EventBoxRow.Guard;
				}
				r_AllSpellDataTable.Rows.Add(r_AllSpellRow);
			}
		}
		DataSet2.R_SpellRow[] array9 = array3;
		foreach (DataSet2.R_SpellRow r_SpellRow in array9)
		{
			DataSet2.R_AllSpellRow r_AllSpellRow2 = r_AllSpellDataTable.NewR_AllSpellRow();
			r_AllSpellRow2.X = r_SpellRow.X;
			r_AllSpellRow2.Y = r_SpellRow.Y;
			r_AllSpellRow2.Z = r_SpellRow.Z;
			r_AllSpellRow2.Object = r_SpellRow.Object;
			r_AllSpellRow2.Spell = r_SpellRow.Spell;
			r_AllSpellRow2.Slot = (int)TblSpell.Select("Name='" + r_SpellRow.Spell + "'")[0][2];
			if (!r_SpellRow.IsGuardNull())
			{
				r_AllSpellRow2.Guard = r_SpellRow.Guard;
			}
			r_AllSpellDataTable.Rows.Add(r_AllSpellRow2);
		}
		text = TblObject.Select("Code='81'")[0][1].ToString();
		DataSet2.R_ScholarRow[] array10 = array2;
		foreach (DataSet2.R_ScholarRow r_ScholarRow in array10)
		{
			DataSet2.R_AllSpellRow r_AllSpellRow3 = r_AllSpellDataTable.NewR_AllSpellRow();
			r_AllSpellRow3.X = r_ScholarRow.X;
			r_AllSpellRow3.Y = r_ScholarRow.Y;
			r_AllSpellRow3.Z = r_ScholarRow.Z;
			r_AllSpellRow3.Object = text;
			r_AllSpellRow3.Spell = r_ScholarRow.Spell;
			r_AllSpellRow3.Slot = (int)TblSpell.Select("Name='" + r_ScholarRow.Spell + "'")[0][2];
			r_AllSpellDataTable.Rows.Add(r_AllSpellRow3);
		}
		text = TblObject.Select("Code='83'")[0][1].ToString();
		DataSet2.R_SeerHutRow[] array11 = array4;
		foreach (DataSet2.R_SeerHutRow r_SeerHutRow in array11)
		{
			DataSet2.R_AllSpellRow r_AllSpellRow4 = r_AllSpellDataTable.NewR_AllSpellRow();
			r_AllSpellRow4.X = r_SeerHutRow.X;
			r_AllSpellRow4.Y = r_SeerHutRow.Y;
			r_AllSpellRow4.Z = r_SeerHutRow.Z;
			r_AllSpellRow4.Object = text;
			r_AllSpellRow4.Spell = r_SeerHutRow.Reward.Replace("Заклинание ", "");
			r_AllSpellRow4.Slot = (int)TblSpell.Select("Name='" + r_AllSpellRow4.Spell + "'")[0][2];
			r_AllSpellRow4.Mission = r_SeerHutRow.Mission;
			r_AllSpellDataTable.Rows.Add(r_AllSpellRow4);
		}
		text = TblObject.Select("Code='98'")[0][1].ToString();
		DataSet2.R_TownRow[] array12 = array5;
		foreach (DataSet2.R_TownRow r_TownRow in array12)
		{
			string[] array13 = r_TownRow.Spell.Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int num = 0; num < array13.Length; num++)
			{
				DataSet2.R_AllSpellRow r_AllSpellRow5 = r_AllSpellDataTable.NewR_AllSpellRow();
				r_AllSpellRow5.X = r_TownRow.X;
				r_AllSpellRow5.Y = r_TownRow.Y;
				r_AllSpellRow5.Z = r_TownRow.Z;
				r_AllSpellRow5.Object = text;
				r_AllSpellRow5.Spell = array13[num];
				r_AllSpellRow5.Slot = r_TownRow.Slot;
				r_AllSpellRow5.Name = r_TownRow.Name;
				if (num == 0 && !r_TownRow.IsGarrisonNull())
				{
					r_AllSpellRow5.Garrison = r_TownRow.Garrison;
				}
				if (!r_TownRow.IsColorNull())
				{
					r_AllSpellRow5.Color = r_TownRow.Color;
				}
				if (!r_TownRow.IsBuiltNull())
				{
					r_AllSpellRow5.Built = aReply[1];
				}
				r_AllSpellDataTable.Rows.Add(r_AllSpellRow5);
			}
		}
		text = TblObject.Select("Code='34'")[0][1].ToString();
		DataSet2.R_HeroesRow[] array14 = array6;
		foreach (DataSet2.R_HeroesRow r_HeroesRow in array14)
		{
			string[] array15 = r_HeroesRow.Spell.Split(new string[1] { ", " }, StringSplitOptions.None);
			int num3 = -1;
			int num4 = -1;
			if (!r_HeroesRow.IsPlaceNull())
			{
				num3 = r_HeroesRow.Place.IndexOf(".");
				num4 = r_HeroesRow.Place.LastIndexOf(".");
			}
			if (num3 < 0)
			{
				for (int num5 = 0; num5 < array15.Length; num5++)
				{
					DataSet2.R_AllSpellRow r_AllSpellRow6 = r_AllSpellDataTable2.NewR_AllSpellRow();
					if (!r_HeroesRow.IsPlaceNull())
					{
						r_AllSpellRow6.Place = r_HeroesRow.Place;
					}
					r_AllSpellRow6.Object = text;
					r_AllSpellRow6.Spell = array15[num5];
					r_AllSpellRow6.Slot = (int)TblSpell.Select("Name='" + array15[num5] + "'")[0][2];
					r_AllSpellRow6.ID = r_HeroesRow.ID;
					r_AllSpellRow6.Hero = r_HeroesRow.Hero;
					if (!r_HeroesRow.IsColorNull())
					{
						r_AllSpellRow6.Color = r_HeroesRow.Color;
					}
					r_AllSpellDataTable2.Rows.Add(r_AllSpellRow6);
				}
				continue;
			}
			for (int num6 = 0; num6 < array15.Length; num6++)
			{
				DataSet2.R_AllSpellRow r_AllSpellRow7 = r_AllSpellDataTable.NewR_AllSpellRow();
				r_AllSpellRow7.X = int.Parse(r_HeroesRow.Place.Substring(0, num3));
				r_AllSpellRow7.Y = int.Parse(r_HeroesRow.Place.Substring(num3 + 1, num4 - num3 - 1));
				r_AllSpellRow7.Z = int.Parse(r_HeroesRow.Place.Substring(num4 + 1, r_HeroesRow.Place.Length - num4 - 1));
				r_AllSpellRow7.Object = text;
				r_AllSpellRow7.Spell = array15[num6];
				r_AllSpellRow7.Slot = (int)TblSpell.Select("Name='" + array15[num6] + "'")[0][2];
				r_AllSpellRow7.ID = r_HeroesRow.ID;
				r_AllSpellRow7.Hero = r_HeroesRow.Hero;
				if (!r_HeroesRow.IsColorNull())
				{
					r_AllSpellRow7.Color = r_HeroesRow.Color;
				}
				else
				{
					r_AllSpellRow7.Place = aPlace[0];
				}
				r_AllSpellDataTable.Rows.Add(r_AllSpellRow7);
			}
		}
		DataView defaultView = r_AllSpellDataTable.DefaultView;
		defaultView.Sort = "Z, Y, X, Slot";
		foreach (DataRowView item in defaultView)
		{
			DataSet2.R_AllSpellRow r_AllSpellRow8 = dsResult.R_AllSpell.NewR_AllSpellRow();
			r_AllSpellRow8.ItemArray = item.Row.ItemArray;
			dsResult.R_AllSpell.Rows.Add(r_AllSpellRow8);
		}
		defaultView = r_AllSpellDataTable2.DefaultView;
		defaultView.Sort = "Place DESC, Color, ID";
		foreach (DataRowView item2 in defaultView)
		{
			DataSet2.R_AllSpellRow r_AllSpellRow9 = dsResult.R_AllSpell.NewR_AllSpellRow();
			r_AllSpellRow9.ItemArray = item2.Row.ItemArray;
			dsResult.R_AllSpell.Rows.Add(r_AllSpellRow9);
		}
	}

	private string GetFullLvlSkill(string skl)
	{
		if (aLevelSkill[0] == skl)
		{
			return aFullLvlSkill[0];
		}
		if (aLevelSkill[1] == skl)
		{
			return aFullLvlSkill[1];
		}
		if (aLevelSkill[2] == skl)
		{
			return aFullLvlSkill[2];
		}
		return "";
	}

	private void GetTownContent()
	{
		int town = map.Town;
		int num = decmp[town];
		town++;
		Encoding @default = Encoding.Default;
		DataSet2.R_TownDataTable r_TownDataTable = new DataSet2.R_TownDataTable();
		for (int i = 0; i < num; i++)
		{
			string text = "";
			string text2 = "";
			int num2 = 0;
			for (int j = 0; j < decmp[town + 70]; j++)
			{
				text += @default.GetString(new byte[1] { decmp[town + 72 + j] });
			}
			DataSet2.R_TownRow r_TownRow = r_TownDataTable.NewR_TownRow();
			r_TownRow.X = decmp[town + 5];
			r_TownRow.Y = decmp[town + 6];
			r_TownRow.Z = decmp[town + 7];
			r_TownRow.Type = aTown[decmp[town + 4]];
			r_TownRow.Code = decmp[town + 4];
			r_TownRow.Name = text;
			r_TownRow.ID = decmp[town];
			if (decmp[town + 1] < byte.MaxValue)
			{
				r_TownRow.Color = aColor[decmp[town + 1]];
			}
			for (int k = 0; k < 7; k++)
			{
				if (decmp[town + k * 4 + 10] < byte.MaxValue)
				{
					DataRow dataRow = TblMonsters.Rows.Find(decmp[town + k * 4 + 10]);
					int num3 = (decmp[town + k * 4 + 39] << 8) + decmp[town + k * 4 + 38];
					text2 = string.Concat(text2, ", ", dataRow[1], " ", num3.ToString());
					num2 += (int)dataRow[3] * num3;
				}
			}
			if (text2 != "")
			{
				r_TownRow.Garrison = text2.Remove(0, 2);
				r_TownRow.HP = num2;
			}
			town = town + 72 + decmp[town + 70] + 113;
			if (!r_TownRow.IsCodeNull())
			{
				GetTimerTown(r_TownRow);
				switch (r_TownRow.Code)
				{
				case 6:
				case 7:
					GetTownSpell(town, 3, r_TownRow, r_TownDataTable);
					break;
				case 0:
					GetTownSpell(town, 4, r_TownRow, r_TownDataTable);
					break;
				case 1:
				case 3:
				case 4:
				case 5:
				case 8:
					GetTownSpell(town, 5, r_TownRow, r_TownDataTable);
					break;
				case 2:
					GetTownSpell(town, 6, r_TownRow, r_TownDataTable);
					break;
				}
			}
			if (text != "")
			{
				r_TownDataTable.Rows.Add(r_TownRow);
			}
			town += 197;
		}
		DataView defaultView = r_TownDataTable.DefaultView;
		defaultView.Sort = "Z, Y, X, Slot";
		foreach (DataRowView item in defaultView)
		{
			DataSet2.R_TownRow r_TownRow2 = dsResult.R_Town.NewR_TownRow();
			r_TownRow2.ItemArray = item.Row.ItemArray;
			dsResult.R_Town.Rows.Add(r_TownRow2);
		}
	}

	private void GetTownSpell(int s, int lvl, DataSet2.R_TownRow row, DataSet2.R_TownDataTable tbl)
	{
		int num = s;
		string text = "";
		int num2 = ((lvl == 6) ? 5 : lvl);
		int num3 = 0;
		int num4;
		int num5 = (num4 = ((lvl == 6) ? 1 : 0));
		if (num5 == 1)
		{
			if ((decmp[num - 14] & 0x40) == 64)
			{
				num3 |= 4;
				row.Library = aStatus[3];
			}
			if ((decmp[num - 6] & 0x40) == 64)
			{
				num3 |= 2;
				if ((num3 & 4) == 0)
				{
					row.Library = aStatus[2];
				}
			}
			if (row.LibTimer > 0)
			{
				num3 |= row.LibTimer;
				if ((num3 & 4) == 0)
				{
					if ((num3 & 2) == 0)
					{
						row.Library = aStatus[1];
					}
					else
					{
						row.Library = row.Library + ", " + aStatus[1];
					}
				}
			}
			if (num3 == 0)
			{
				row.Library = aStatus[0];
				num5 = 0;
			}
		}
		for (int i = 0; i < num2; i++)
		{
			if (row.MageTimer >> i + 3 > 0 || ((decmp[num - 8] >> i) & 1) > 0 || ((decmp[num - 16] >> i) & 1) > 0)
			{
				if (row.MageTimer >> i + 3 > 0)
				{
					row.Timer = aReply[1];
				}
				if (((decmp[num - 8] >> i) & 1) > 0)
				{
					row.Available = aReply[1];
				}
				else
				{
					row.Available = aReply[0];
				}
				if (((decmp[num - 16] >> i) & 1) > 0)
				{
					row.Built = aReply[1];
				}
				for (int j = 0; j < 5 + num4 - i; j++)
				{
					if (decmp[s + j * 4] < byte.MaxValue && j < 5 + num5 - i)
					{
						text = text + ", " + TblSpell.Rows[decmp[s + j * 4]][1];
					}
				}
				if (text != "")
				{
					row.Spell = text.Remove(0, 2);
				}
				row.Slot = i + 1;
				text = "";
				if (i < num2 - 1)
				{
					DataSet2.R_TownRow r_TownRow = tbl.NewR_TownRow();
					r_TownRow.ItemArray = row.ItemArray;
					r_TownRow.SetSpellNull();
					r_TownRow.SetBuiltNull();
					r_TownRow.SetAvailableNull();
					r_TownRow.SetTimerNull();
					r_TownRow.SetGarrisonNull();
					r_TownRow.SetHPNull();
					r_TownRow.SetLibraryNull();
					tbl.Rows.Add(r_TownRow);
					row = r_TownRow;
					s += 24;
				}
				continue;
			}
			if (i == 0)
			{
				row.Available = aReply[0];
				row.Slot = 1;
				if (lvl == 6)
				{
					row.Library = aReply[0];
				}
			}
			else
			{
				row.Delete();
			}
			break;
		}
	}

	private void GetColorContent()
	{
		int color = map.Color;
		for (int i = 0; i < 8; i++)
		{
			aExistColor[i] = decmp[color + i * 145 + 24] + decmp[color + i * 145 + 1];
			aTavernGuest[i, 0] = decmp[color + i * 145 + 12];
			aTavernGuest[i, 1] = decmp[color + i * 145 + 11];
			if (decmp[color + i * 145 + 14] == 3)
			{
				human = i;
			}
		}
	}

	private void GetCurrentState()
	{
		int currentState = map.CurrentState;
		if (decmp[currentState + 2] != byte.MaxValue)
		{
			grail = decmp[currentState + 2] + "." + decmp[currentState + 4] + "." + decmp[currentState + 6];
		}
		else
		{
			grail = aReply[0];
		}
		currentState -= Chrn;
		days = "Дата - " + decmp[currentState + 15] + decmp[currentState + 13] + decmp[currentState + 11];
		GetArtMerchants(currentState + 49);
	}

	private void GetArtMerchants(int s)
	{
		if (dsResult.R_Town.Select("Slot='1' AND (Code='2' OR Code='5' OR Code='8')").Length <= 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			if (decmp[s + i * 4] < byte.MaxValue)
			{
				DataSet2.R_MarketRow r_MarketRow = dsResult.R_Market.NewR_MarketRow();
				DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
				DataRow dataRow = TblArt.Rows.Find(decmp[s + i * 4]);
				if (spt && i == 0)
				{
					r_MarketRow.Address = s;
				}
				string artefact = (r_MarketRow.Art = (string)dataRow[1]);
				r_AllArtsRow.Artefact = artefact;
				string @class = (r_MarketRow.Class = (string)dataRow[2]);
				r_AllArtsRow.Class = @class;
				if (dataRow[3] != DBNull.Value)
				{
					string relic_C = (r_MarketRow._Relic_C = (string)dataRow[3]);
					r_AllArtsRow._Relic_C = relic_C;
				}
				r_MarketRow.Cost = (int)dataRow[4];
				int slot = (r_MarketRow.Slot = i + 1);
				r_AllArtsRow.Slot = slot;
				string @object = (r_MarketRow.Object = TblObject.Select("Code='255'")[0][1].ToString());
				r_AllArtsRow.Object = @object;
				dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow, num);
				dsResult.R_Market.Rows.InsertAt(r_MarketRow, num);
				num++;
			}
		}
	}

	private void GetHeroesContent()
	{
		int num = map.Hero;
		int heroState = map.HeroState;
		int pos = 0;
		for (int i = 0; i < HeroCount; i++)
		{
			bool flag = false;
			bool flag2 = false;
			DataSet2.R_HeroesRow r_HeroesRow = dsResult.R_Heroes.NewR_HeroesRow();
			r_HeroesRow.ID = i;
			r_HeroesRow.Ideology = TblIdeology.Rows[i][1].ToString();
			r_HeroesRow.Class = TblIdeology.Rows[i][2].ToString();
			r_HeroesRow.ClassID = (int)TblIdeology.Rows[i][3];
			if (spt)
			{
				r_HeroesRow.Address = num;
			}
			if (decmp[num] != byte.MaxValue)
			{
				r_HeroesRow.Place = decmp[num] + "." + decmp[num + 2] + "." + decmp[num + 4];
				flag = true;
			}
			else if (decmp[heroState + i] == 64)
			{
				int num2 = IsHeroTavern(i);
				if (num2 == 255)
				{
					r_HeroesRow.Place = aPlace[2];
					flag2 = true;
				}
				else
				{
					r_HeroesRow.Color = aColor[num2];
					r_HeroesRow.Place = aPlace[1];
				}
			}
			if (!flag2 && (decmp[heroState + i + HeroCount] & (1 << human)) > 0)
			{
				r_HeroesRow.Hire = aReply[1];
			}
			else
			{
				r_HeroesRow.Hire = aReply[0];
			}
			num = (((decmp[num + 23] << 8) + decmp[num + 22] == 0) ? (num + 26) : (num + 26 + ((decmp[num + 23] << 8) + decmp[num + 22])));
			if (decmp[num] < 8)
			{
				r_HeroesRow.Color = aColor[decmp[num]];
			}
			r_HeroesRow.TreeNumber = decmp[num + 17];
			r_HeroesRow.LastWisdom = decmp[num + 18];
			r_HeroesRow.LastMagic = decmp[num + 29];
			r_HeroesRow.MP = (decmp[num + 32] << 8) + decmp[num + 31];
			r_HeroesRow.Experience = (decmp[num + 42] << 24) + (decmp[num + 41] << 16) + (decmp[num + 40] << 8) + decmp[num + 39];
			r_HeroesRow.Level = (decmp[num + 50] << 8) + decmp[num + 49];
			num += 113;
			_ = 27;
			string text = "";
			int num3 = 0;
			for (int j = 0; j < 7; j++)
			{
				if (decmp[num + j * 4] < byte.MaxValue)
				{
					DataRow dataRow = TblMonsters.Rows.Find(decmp[num + j * 4]);
					int num4 = (decmp[num + j * 4 + 29] << 8) + decmp[num + j * 4 + 28];
					text = string.Concat(text, ", ", dataRow[1], " ", num4.ToString());
					num3 += (int)dataRow[3] * num4;
				}
			}
			if (!r_HeroesRow.IsPlaceNull() && r_HeroesRow.Place != aPlace[2] && text != "")
			{
				r_HeroesRow.Monster = text.Remove(0, 2);
				r_HeroesRow.HP = num3;
			}
			num += 56;
			Encoding @default = Encoding.Default;
			string text2 = "";
			for (int k = 0; k < 12 && decmp[num + k] != 0; k++)
			{
				text2 += @default.GetString(new byte[1] { decmp[num + k] });
			}
			r_HeroesRow.Hero = text2;
			num += 13;
			string text3 = "";
			string[] array = new string[8];
			for (int l = 0; l < 28; l++)
			{
				if (decmp[num + l] != 0)
				{
					array[decmp[num + l + 28] - 1] = aLevelSkill[decmp[num + l] - 1] + TblSecondarySkill.Rows[l][1];
				}
			}
			for (int m = 0; m < 8 && array[m] != null; m++)
			{
				text3 = text3 + ", " + array[m];
			}
			r_HeroesRow.SecondarySkill = ((text3 != "") ? text3.Remove(0, 2) : text3);
			num += 56;
			r_HeroesRow.PrimarySkill = aPrSkill[0] + decmp[num] + "-" + aPrSkill[1] + decmp[num + 1] + "-" + aPrSkill[2] + decmp[num + 2] + "-" + aPrSkill[3] + decmp[num + 3];
			num += 4;
			string text4 = "";
			for (int n = 0; n < 70; n++)
			{
				if (decmp[num + n] == 1)
				{
					text4 = text4 + ", " + TblSpell.Rows[n][1];
				}
			}
			if (text4 != "")
			{
				r_HeroesRow.Spell = text4.Remove(0, 2);
			}
			num += 140;
			int pos2 = 0;
			string text5 = "";
			string text6 = "";
			string loc = "";
			string plc = "";
			for (int num5 = 0; num5 < 83; num5++)
			{
				int num6 = decmp[num + num5 * 8];
				switch (num6)
				{
				case 4:
				case 5:
				case 6:
					text5 = text5 + ", " + TblArt.Rows.Find(num6)[1];
					continue;
				case 0:
					r_HeroesRow.Book = aReply[1];
					continue;
				case 3:
				case 255:
					continue;
				}
				DataRow dataRow2 = TblArt.Rows.Find(num6);
				text6 = text6 + ", " + dataRow2[1].ToString();
				if (flag)
				{
					pos2 = ArtDollPlace1(r_HeroesRow, dataRow2, num5, pos2, ref loc, ref plc);
				}
				else if (!flag2)
				{
					pos = ArtDollPlace2(r_HeroesRow, dataRow2, num5, pos);
				}
			}
			if (text6 != "")
			{
				r_HeroesRow.Art = text6.Remove(0, 2);
			}
			if (text5 != "")
			{
				r_HeroesRow.Machine = text5.Remove(0, 2);
			}
			num += 686;
			dsResult.R_Heroes.Rows.Add(r_HeroesRow);
		}
	}

	private int ArtDollPlace1(DataSet2.R_HeroesRow hrow, DataRow arow, int pl, int pos, ref string loc, ref string plc)
	{
		int num = hrow.Place.IndexOf(".");
		int num2 = hrow.Place.LastIndexOf(".");
		if (pos == 0)
		{
			DataSet2.R_AllArtsRow[] array = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("X='" + hrow.Place.Substring(0, num) + "' AND Y='" + hrow.Place.Substring(num + 1, num2 - num - 1) + "' AND Z='" + hrow.Place.Substring(num2 + 1, hrow.Place.Length - num2 - 1) + "'");
			if (array.Length > 0)
			{
				DataSet2.R_AllArtsRow r_AllArtsRow = array[0];
				loc = r_AllArtsRow.Locality;
				if (!DBNull.Value.Equals(r_AllArtsRow["Place"]))
				{
					plc = r_AllArtsRow.Place;
				}
				GetDollPlace(hrow, arow, r_AllArtsRow, pl);
				pos = dsResult.R_AllArts.Rows.IndexOf(r_AllArtsRow);
			}
			else
			{
				DataSet2.R_AllArtsRow r_AllArtsRow2 = dsResult.R_AllArts.NewR_AllArtsRow();
				r_AllArtsRow2.X = int.Parse(hrow.Place.Substring(0, num));
				r_AllArtsRow2.Y = int.Parse(hrow.Place.Substring(num + 1, num2 - num - 1));
				r_AllArtsRow2.Z = int.Parse(hrow.Place.Substring(num2 + 1, hrow.Place.Length - num2 - 1));
				r_AllArtsRow2.Object = TblObject.Select("Code='34'")[0][1].ToString();
				r_AllArtsRow2.Locality = loc;
				if (plc != "")
				{
					r_AllArtsRow2.Place = plc;
				}
				GetDollPlace(hrow, arow, r_AllArtsRow2, pl);
				dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow2, pos);
			}
		}
		else
		{
			DataSet2.R_AllArtsRow r_AllArtsRow3 = dsResult.R_AllArts.NewR_AllArtsRow();
			r_AllArtsRow3.X = int.Parse(hrow.Place.Substring(0, num));
			r_AllArtsRow3.Y = int.Parse(hrow.Place.Substring(num + 1, num2 - num - 1));
			r_AllArtsRow3.Z = int.Parse(hrow.Place.Substring(num2 + 1, hrow.Place.Length - num2 - 1));
			r_AllArtsRow3.Object = TblObject.Select("Code='34'")[0][1].ToString();
			r_AllArtsRow3.Locality = loc;
			if (plc != "")
			{
				r_AllArtsRow3.Place = plc;
			}
			GetDollPlace(hrow, arow, r_AllArtsRow3, pl);
			dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow3, pos);
		}
		return ++pos;
	}

	private int ArtDollPlace2(DataSet2.R_HeroesRow hrow, DataRow arow, int pl, int pos)
	{
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		r_AllArtsRow.Object = TblObject.Select("Code='34'")[0][1].ToString();
		if (!DBNull.Value.Equals(hrow["Place"]))
		{
			r_AllArtsRow.Place = hrow.Place;
		}
		GetDollPlace(hrow, arow, r_AllArtsRow, pl);
		dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow, pos);
		return ++pos;
	}

	private void GetDollPlace(DataSet2.R_HeroesRow hrow, DataRow arow, DataSet2.R_AllArtsRow nrow, int pl)
	{
		nrow.Artefact = (string)arow[1];
		nrow.Class = (string)arow[2];
		if (arow[3] != DBNull.Value)
		{
			nrow._Relic_C = (string)arow[3];
		}
		if (!DBNull.Value.Equals(hrow["Color"]))
		{
			nrow.Color = hrow.Color;
		}
		nrow.Hero = hrow.Hero;
		if (pl < 13 || pl == 18)
		{
			nrow.Doll = aDoll[pl];
		}
		else
		{
			nrow.Doll = aDoll[19];
		}
	}

	private int IsHeroTavern(int hero)
	{
		for (int i = 0; i < aTavernGuest.GetLength(0); i++)
		{
			if (aTavernGuest[i, 0] == hero || aTavernGuest[i, 1] == hero)
			{
				return i;
			}
		}
		return 255;
	}

	private void GetPrisonHero(DataSet2.R_PrisonRow row)
	{
		DataSet2.R_HeroesRow r_HeroesRow = (DataSet2.R_HeroesRow)dsResult.R_Heroes.Select("ID='" + row.Hero + "'")[0];
		row.Hero = r_HeroesRow.Hero;
		row.Level = r_HeroesRow.Level;
		row.PrimarySkill = r_HeroesRow.PrimarySkill;
		row.SecondarySkill = r_HeroesRow.SecondarySkill;
		if (!r_HeroesRow.IsArtNull())
		{
			row.Art = r_HeroesRow.Art;
		}
		if (!r_HeroesRow.IsMachineNull())
		{
			row.Machine = r_HeroesRow.Machine;
		}
		if (!r_HeroesRow.IsBookNull())
		{
			row.Book = r_HeroesRow.Book;
		}
		if (!r_HeroesRow.IsSpellNull())
		{
			row.Spell = r_HeroesRow.Spell;
		}
		if (!r_HeroesRow.IsMonsterNull())
		{
			row.Monster = r_HeroesRow.Monster;
		}
		if (!r_HeroesRow.IsHPNull())
		{
			row.HP = r_HeroesRow.HP;
		}
		row.MP = r_HeroesRow.MP;
		row.Experience = r_HeroesRow.Experience;
	}

	private void GetUniverContent()
	{
		if (TblUniver.Rows.Count > 0)
		{
			int s = map.Univer + 2;
			for (int i = 0; i < TblUniver.Rows.Count; i++)
			{
				DataRow row = TblUniver.Rows[i];
				s = UniverContent(row, s);
			}
		}
	}

	private int UniverContent(DataRow row, int s)
	{
		DataSet2.R_SkillRow r_SkillRow = (DataSet2.R_SkillRow)dsResult.R_Skill.Select("X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'")[0];
		if (spt)
		{
			r_SkillRow.Address = s;
		}
		r_SkillRow.Skill = (string)TblSecondarySkill.Rows[decmp[s]][1] + ", " + (string)TblSecondarySkill.Rows[decmp[s + 4]][1] + ", " + (string)TblSecondarySkill.Rows[decmp[s + 8]][1] + ", " + (string)TblSecondarySkill.Rows[decmp[s + 12]][1];
		return s + 16;
	}

	private void GetMarketContent()
	{
		if (TblMarket.Rows.Count > 0)
		{
			int s = map.BlackMarket + 1;
			for (int i = 0; i < TblMarket.Rows.Count; i++)
			{
				DataRow row = TblMarket.Rows[i];
				s = MarketContent(row, s);
			}
		}
	}

	private int MarketContent(DataRow row, int s)
	{
		int num = 0;
		string filterExpression = "X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'";
		DataSet2.R_MarketRow r_MarketRow = (DataSet2.R_MarketRow)dsResult.R_Market.Select(filterExpression)[0];
		DataSet2.R_AllArtsRow r_AllArtsRow = (DataSet2.R_AllArtsRow)dsResult.R_AllArts.Select(filterExpression)[0];
		if (spt)
		{
			r_MarketRow.Address = s;
		}
		for (int i = 1; i < 7; i++)
		{
			if (decmp[s + i * 4] != byte.MaxValue)
			{
				num++;
				DataSet2.R_MarketRow r_MarketRow2 = dsResult.R_Market.NewR_MarketRow();
				DataSet2.R_AllArtsRow r_AllArtsRow2 = dsResult.R_AllArts.NewR_AllArtsRow();
				r_MarketRow2.ItemArray = r_MarketRow.ItemArray;
				r_AllArtsRow2.ItemArray = r_AllArtsRow.ItemArray;
				DataRow dataRow = TblArt.Rows.Find(decmp[s + i * 4]);
				string artefact = (r_MarketRow2.Art = (string)dataRow[1]);
				r_AllArtsRow2.Artefact = artefact;
				string @class = (r_MarketRow2.Class = (string)dataRow[2]);
				r_AllArtsRow2.Class = @class;
				if (dataRow[3] != DBNull.Value)
				{
					string relic_C = (r_MarketRow2._Relic_C = (string)dataRow[3]);
					r_AllArtsRow2._Relic_C = relic_C;
				}
				r_MarketRow2.Cost = (int)dataRow[4];
				int slot = (r_MarketRow2.Slot = i + 1);
				r_AllArtsRow2.Slot = slot;
				dsResult.R_Market.Rows.InsertAt(r_MarketRow2, dsResult.R_Market.Rows.IndexOf(r_MarketRow) + i);
				dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow2, dsResult.R_AllArts.Rows.IndexOf(r_AllArtsRow) + i);
			}
		}
		if (decmp[s] == byte.MaxValue)
		{
			if (num > 0)
			{
				r_MarketRow.Delete();
				r_AllArtsRow.Delete();
			}
			else
			{
				r_AllArtsRow.Delete();
			}
		}
		else
		{
			DataRow dataRow = TblArt.Rows.Find(decmp[s]);
			string artefact2 = (r_MarketRow.Art = (string)dataRow[1]);
			r_AllArtsRow.Artefact = artefact2;
			string class2 = (r_MarketRow.Class = (string)dataRow[2]);
			r_AllArtsRow.Class = class2;
			if (dataRow[3] != DBNull.Value)
			{
				string relic_C2 = (r_MarketRow._Relic_C = (string)dataRow[3]);
				r_AllArtsRow._Relic_C = relic_C2;
			}
			r_MarketRow.Cost = (int)dataRow[4];
			int slot2 = (r_MarketRow.Slot = 1);
			r_AllArtsRow.Slot = slot2;
		}
		return s + 28;
	}

	private void GetGarrisonContent()
	{
		if (TblGarrison.Rows.Count > 0)
		{
			int garrison = map.Garrison;
			garrison++;
			for (int i = 0; i < TblGarrison.Rows.Count; i++)
			{
				DataRow row = TblGarrison.Rows[i];
				garrison = GarrisonContent(row, garrison);
			}
		}
	}

	private int GarrisonContent(DataRow row, int s)
	{
		string text = "";
		int num = 0;
		DataSet2.R_GarrisonRow r_GarrisonRow = (DataSet2.R_GarrisonRow)dsResult.R_Garrison.Rows.Find(new object[3]
		{
			decmp[s + 57],
			decmp[s + 58],
			decmp[s + 59]
		});
		if (r_GarrisonRow != null)
		{
			for (int i = 0; i < 7; i++)
			{
				if (decmp[s + i * 4 + 1] < byte.MaxValue)
				{
					DataRow dataRow = TblMonsters.Rows.Find(decmp[s + i * 4 + 1]);
					int num2 = (decmp[s + i * 4 + 30] << 8) + decmp[s + i * 4 + 29];
					text = string.Concat(text, ", ", dataRow[1], " ", num2.ToString());
					num += (int)dataRow[3] * num2;
				}
			}
			if (text != "")
			{
				r_GarrisonRow.Guard = text.Remove(0, 2);
				r_GarrisonRow.HP = num;
			}
			if (decmp[s] != byte.MaxValue)
			{
				r_GarrisonRow.Color = aColor[decmp[s]];
			}
			r_GarrisonRow.CanTake = aReply[decmp[s + 60]];
		}
		return s + 61;
	}

	private int EventBoxContent(DataRow row, int s)
	{
		if (row == null)
		{
			return ScanEventBoxContent(s);
		}
		if (decmp[s] > 0)
		{
			s = s + (decmp[s + 2] << 8) + decmp[s + 1] + 3;
		}
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int[] array = new int[0];
		int[] array2 = new int[0];
		int[,] array3 = new int[0, 0];
		if (decmp[s] == 1)
		{
			s++;
			for (int i = 0; i < 7; i++)
			{
				if (decmp[s + i * 4] < byte.MaxValue)
				{
					DataRow dataRow = TblMonsters.Rows.Find(decmp[s + i * 4]);
					int num7 = (decmp[s + i * 4 + 29] << 8) + decmp[s + i * 4 + 28];
					text = string.Concat(text, ", ", dataRow[1], " ", num7.ToString());
					num6 += (int)dataRow[3] * num7;
				}
			}
			s += 56;
		}
		else
		{
			s++;
		}
		num = (decmp[s + 3] << 24) + (decmp[s + 2] << 16) + (decmp[s + 1] << 8) + decmp[s];
		s += 4;
		num2 = ((decmp[s + 3] != byte.MaxValue) ? ((decmp[s + 1] << 8) + decmp[s]) : ((((byte)(~decmp[s + 1]) << 8) + (byte)(~decmp[s]) + 1) * -1));
		s += 4;
		num4 = ((decmp[s] > 240) ? (((byte)(~decmp[s]) + 1) * -1) : decmp[s]);
		num5 = ((decmp[s + 1] > 240) ? (((byte)(~decmp[s + 1]) + 1) * -1) : decmp[s + 1]);
		s += 2;
		int address = s;
		for (int j = 0; j < 6; j++)
		{
			if (decmp[s + j * 4 + 3] == byte.MaxValue)
			{
				text2 = text2 + ", " + aResource[j] + (((byte)(~decmp[s + j * 4 + 2]) << 16) + ((byte)(~decmp[s + j * 4 + 1]) << 8) + (byte)(~decmp[s + j * 4]) + 1) * -1;
			}
			else if (decmp[s + j * 4 + 2] != 0 || decmp[s + j * 4 + 1] != 0 || decmp[s + j * 4] != 0)
			{
				text2 = text2 + ", " + aResource[j] + ((decmp[s + j * 4 + 2] << 16) + (decmp[s + j * 4 + 1] << 8) + decmp[s + j * 4]);
			}
		}
		s += 24;
		num3 = ((decmp[s + 3] != byte.MaxValue) ? ((decmp[s + 2] << 16) + (decmp[s + 1] << 8) + decmp[s]) : ((((byte)(~decmp[s + 2]) << 16) + ((byte)(~decmp[s + 1]) << 8) + (byte)(~decmp[s]) + 1) * -1));
		s += 4;
		for (int k = 0; k < 4; k++)
		{
			if (decmp[s + k] != 0)
			{
				text3 = text3 + ", " + aPrSkill[k] + decmp[s + k];
			}
		}
		s += 4;
		if (decmp[s] > 0)
		{
			for (int l = 0; l < decmp[s]; l++)
			{
				text4 = text4 + ", " + aLevelSkill[decmp[s + l * 2 + 2] - 1] + TblSecondarySkill.Rows[decmp[s + l * 2 + 1]][1];
			}
			s = s + decmp[s] * 2 + 1;
		}
		else
		{
			s++;
		}
		if (decmp[s] > 0)
		{
			array = new int[decmp[s]];
			for (int m = 0; m < decmp[s]; m++)
			{
				array[m] = decmp[s + m + 1];
			}
			s = s + decmp[s] + 1;
		}
		else
		{
			s++;
		}
		if (decmp[s] > 0)
		{
			array2 = new int[decmp[s]];
			for (int n = 0; n < decmp[s]; n++)
			{
				array2[n] = decmp[s + n + 1];
			}
			s = s + decmp[s] + 1;
		}
		else
		{
			s++;
		}
		if (decmp[s] > 0)
		{
			array3 = new int[decmp[s], 2];
			for (int num8 = 0; num8 < decmp[s]; num8++)
			{
				array3[num8, 0] = decmp[s + num8 * 4 + 1];
				array3[num8, 1] = (decmp[s + num8 * 4 + 4] << 8) + decmp[s + num8 * 4 + 3];
			}
			s = s + decmp[s] * 4 + 1;
		}
		else
		{
			s++;
		}
		DataSet2.R_EventBoxRow r_EventBoxRow = (DataSet2.R_EventBoxRow)dsResult.R_EventBox.Select("X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'")[0];
		if (ot3)
		{
			r_EventBoxRow.Address = address;
		}
		if (text != "")
		{
			r_EventBoxRow.Guard = text.Remove(0, 2);
			r_EventBoxRow.HP = num6;
		}
		if (num != 0)
		{
			r_EventBoxRow.Experience = num;
		}
		if (num2 != 0)
		{
			r_EventBoxRow.Mana = num2;
		}
		if (num4 != 0)
		{
			r_EventBoxRow.Morale = ((num4 > 0) ? ("+" + num4) : num4.ToString());
		}
		if (num5 != 0)
		{
			r_EventBoxRow.Luck = ((num5 > 0) ? ("+" + num5) : num5.ToString());
		}
		if (num3 != 0)
		{
			r_EventBoxRow.Gold = num3;
		}
		if (text2 != "")
		{
			r_EventBoxRow.Resource = text2.Remove(0, 2);
		}
		if (text3 != "")
		{
			r_EventBoxRow.PrimarySkill = text3.Remove(0, 2);
		}
		if (text4 != "")
		{
			r_EventBoxRow.SecondarySkill = text4.Remove(0, 2);
		}
		if (array.Length > 0)
		{
			string text5 = "";
			for (int num9 = 0; num9 < array.Length; num9++)
			{
				text5 = text5 + ", " + TblArt.Rows.Find(array[num9])[1];
			}
			r_EventBoxRow.Artefact = text5.Remove(0, 2);
		}
		if (array2.Length > 0)
		{
			string text6 = "";
			for (int num10 = 0; num10 < array2.Length; num10++)
			{
				text6 = text6 + ", " + TblSpell.Rows[array2[num10]][1];
			}
			r_EventBoxRow.Spell = text6.Remove(0, 2);
		}
		if (array3.GetLength(0) > 0)
		{
			string text7 = "";
			for (int num11 = 0; num11 < array3.GetLength(0); num11++)
			{
				text7 = string.Concat(text7, ", ", TblMonsters.Rows.Find(array3[num11, 0])[1], " ", array3[num11, 1].ToString());
			}
			r_EventBoxRow.Monster = text7.Remove(0, 2);
		}
		DataSet2.R_AllArtsRow r_AllArtsRow = (DataSet2.R_AllArtsRow)dsResult.R_AllArts.Select("X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'")[0];
		if (array.Length > 0)
		{
			for (int num12 = 1; num12 < array.Length; num12++)
			{
				DataSet2.R_AllArtsRow r_AllArtsRow2 = dsResult.R_AllArts.NewR_AllArtsRow();
				r_AllArtsRow2.ItemArray = r_AllArtsRow.ItemArray;
				DataRow dataRow2 = TblArt.Rows.Find(array[num12]);
				r_AllArtsRow2.Artefact = (string)dataRow2[1];
				r_AllArtsRow2.Class = (string)dataRow2[2];
				if (dataRow2[3] != DBNull.Value)
				{
					r_AllArtsRow2._Relic_C = (string)dataRow2[3];
				}
				dsResult.R_AllArts.Rows.InsertAt(r_AllArtsRow2, dsResult.R_AllArts.Rows.IndexOf(r_AllArtsRow) + num12);
			}
			GetArt(r_AllArtsRow, array[0]);
			if (text != "")
			{
				r_AllArtsRow.Guard = text.Remove(0, 2);
			}
		}
		else
		{
			r_AllArtsRow.Delete();
		}
		return --s;
	}

	private int ArtResContent(DataRow row, int s)
	{
		if (row == null)
		{
			return ScanArtResContent(s);
		}
		s = s + (decmp[s + 1] << 8) + decmp[s] + 2;
		if (decmp[s] == 1)
		{
			string text = "";
			int num = 0;
			s++;
			for (int i = 0; i < 7; i++)
			{
				if (decmp[s + i * 4] < byte.MaxValue)
				{
					DataRow dataRow = TblMonsters.Rows.Find(decmp[s + i * 4]);
					int num2 = (decmp[s + i * 4 + 29] << 8) + decmp[s + i * 4 + 28];
					text = string.Concat(text, ", ", dataRow[1], " ", num2.ToString());
					num += (int)dataRow[3] * num2;
				}
			}
			string filterExpression = "X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'";
			if ((int)row["Type"] == 0)
			{
				DataSet2.R_ArtRow r_ArtRow = (DataSet2.R_ArtRow)dsResult.R_Art.Select(filterExpression)[0];
				DataSet2.R_AllArtsRow r_AllArtsRow = (DataSet2.R_AllArtsRow)dsResult.R_AllArts.Select(filterExpression)[0];
				if (text != "")
				{
					string guard = (r_ArtRow.Guard = text.Remove(0, 2));
					r_AllArtsRow.Guard = guard;
					r_ArtRow.HP = num;
				}
			}
			else if ((int)row["Type"] == 1)
			{
				DataSet2.R_ResourceRow r_ResourceRow = (DataSet2.R_ResourceRow)dsResult.R_Resource.Select(filterExpression)[0];
				if (text != "")
				{
					r_ResourceRow.Guard = text.Remove(0, 2);
					r_ResourceRow.HP = num;
				}
			}
			else if ((int)row["Type"] == 2)
			{
				DataSet2.R_SpellRow r_SpellRow = (DataSet2.R_SpellRow)dsResult.R_Spell.Select(filterExpression)[0];
				if (text != "")
				{
					r_SpellRow.Guard = text.Remove(0, 2);
					r_SpellRow.HP = num;
				}
			}
			s += 56;
		}
		else
		{
			s++;
		}
		return --s;
	}

	private void MonstrContent(DataRow row, int s)
	{
		s += (decmp[s + 1] << 8) + decmp[s] + 2;
		string text = "";
		int num = 0;
		DataSet2.R_MonstrRow r_MonstrRow = (DataSet2.R_MonstrRow)dsResult.R_Monstr.Rows.Find(new object[3]
		{
			row[0],
			row[1],
			row[2]
		});
		DataSet2.R_AllArtsRow[] array = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'");
		for (int i = 0; i < 6; i++)
		{
			if (decmp[s + i * 4 + 2] != 0 || decmp[s + i * 4 + 1] != 0 || decmp[s + i * 4] != 0)
			{
				text = text + ", " + aResource[i] + ((decmp[s + i * 4 + 2] << 16) + (decmp[s + i * 4 + 1] << 8) + decmp[s + i * 4]);
			}
		}
		if (text != "")
		{
			r_MonstrRow.Resource = text.Remove(0, 2);
		}
		s += 24;
		num = (decmp[s + 2] << 16) + (decmp[s + 1] << 8) + decmp[s];
		if (num != 0)
		{
			r_MonstrRow.Gold = num;
		}
		s += 4;
		if (decmp[s] < byte.MaxValue)
		{
			DataRow dataRow = TblArt.Rows.Find(decmp[s]);
			r_MonstrRow.Art = (string)dataRow[1];
			if (array.Length > 0)
			{
				array[0].Artefact = (string)dataRow[1];
				array[0].Class = (string)dataRow[2];
				if (dataRow[3] != DBNull.Value)
				{
					array[0]._Relic_C = (string)dataRow[3];
				}
			}
		}
		else if (array.Length > 0)
		{
			array[0].Delete();
		}
	}

	private int SeerHutContent(DataRow row, int s)
	{
		if (row == null)
		{
			return ScanSeerHutContent(s);
		}
		DataSet2.R_AllArtsRow r_AllArtsRow = null;
		DataSet2.R_SeerHutRow r_SeerHutRow = (DataSet2.R_SeerHutRow)dsResult.R_SeerHut.Rows.Find(new object[3]
		{
			(int)row[0] - 1,
			row[1],
			row[2]
		});
		DataSet2.R_AllArtsRow[] array = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("X='" + ((int)row[0] - 1) + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'");
		if (array.Length > 0)
		{
			r_AllArtsRow = array[0];
			r_AllArtsRow.Artefact = "";
		}
		if (decmp[s] == 0)
		{
			r_AllArtsRow?.Delete();
			return s + 15;
		}
		if (decmp[s] == 1)
		{
			r_SeerHutRow.Mission = аQuest[0] + " " + decmp[s + 1];
			s += 5;
		}
		else if (decmp[s] == 2)
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				if (decmp[s + i + 1] != 0)
				{
					text = text + ", " + aPrSkill[i] + decmp[s + i + 1];
				}
			}
			if (text != "")
			{
				r_SeerHutRow.Mission = аQuest[1] + " " + text.Remove(0, 2);
			}
			s += 7;
		}
		else if (decmp[s] == 3)
		{
			r_SeerHutRow.Mission = "{" + decmp[s + 1];
			s += 6;
		}
		else if (decmp[s] == 4)
		{
			r_SeerHutRow.Mission = string.Concat(аQuest[2], " ", TblMonsters.Rows.Find(decmp[s + 5])[1], " (", decmp[s + 1].ToString(), ".", decmp[s + 3].ToString(), ".", ((decmp[s + 4] & 4) >> 2).ToString(), ")");
			s += 10;
		}
		else if (decmp[s] == 5)
		{
			string text2 = "";
			for (int j = 0; j < decmp[s + 1] * 2; j += 2)
			{
				text2 = text2 + ", " + TblArt.Rows.Find(decmp[s + j + 2])[1];
			}
			if (text2 != "")
			{
				r_SeerHutRow.Mission = аQuest[3] + " " + text2.Remove(0, 2);
			}
			s = s + decmp[s + 1] * 2 + 4;
		}
		else if (decmp[s] == 6)
		{
			string text3 = "";
			for (int k = 0; k < decmp[s + 1] * 6; k += 6)
			{
				text3 = string.Concat(text3, ", ", TblMonsters.Rows.Find(decmp[s + k + 2])[1], " ", ((decmp[s + k + 5] << 8) + decmp[s + k + 4]).ToString());
			}
			if (text3 != "")
			{
				r_SeerHutRow.Mission = аQuest[4] + " " + text3.Remove(0, 2);
			}
			s = s + decmp[s + 1] * 6 + 4;
		}
		else if (decmp[s] == 7)
		{
			string text4 = "";
			int num = 0;
			for (int l = 0; l < 6; l++)
			{
				num = (decmp[s + l * 4 + 3] << 16) + (decmp[s + l * 4 + 2] << 8) + decmp[s + l * 4 + 1];
				if (num > 0)
				{
					text4 = text4 + ", " + aResource[l] + num;
				}
			}
			num = (decmp[s + 24 + 3] << 16) + (decmp[s + 24 + 2] << 8) + decmp[s + 24 + 1];
			if (num > 0)
			{
				text4 = text4 + ", " + aMine[6] + " " + num;
			}
			if (text4 != "")
			{
				r_SeerHutRow.Mission = аQuest[5] + " " + text4.Remove(0, 2);
			}
			s += 31;
		}
		else if (decmp[s] == 8)
		{
			r_SeerHutRow.Mission = "}" + decmp[s + 1];
			s += 5;
		}
		else if (decmp[s] == 9)
		{
			r_SeerHutRow.Mission = аQuest[6] + " " + aColor[decmp[s + 1]];
			s += 4;
		}
		if (decmp[s] != byte.MaxValue && decmp[s + 1] != byte.MaxValue)
		{
			int num2 = decmp[s] / 28;
			int num3 = (decmp[s] - num2 * 28) / 7;
			int num4 = decmp[s] - num2 * 28 - num3 * 7;
			r_SeerHutRow.Deadline = (num2 + 1).ToString() + (num3 + 1) + (num4 + 1);
		}
		s += 4;
		for (int m = 0; m < 3; m++)
		{
			s = (((decmp[s + 1] << 8) + decmp[s] != 0) ? (s + (decmp[s + 1] << 8) + decmp[s] + 4) : (s + 4));
		}
		if (decmp[s] == 0)
		{
			s += 15;
		}
		else if (decmp[s] == 1)
		{
			r_SeerHutRow.Reward = aReward[0] + " " + ((decmp[s + 7] << 24) + (decmp[s + 6] << 16) + (decmp[s + 5] << 8) + decmp[s + 4]);
			s += 15;
		}
		else if (decmp[s] == 2)
		{
			r_SeerHutRow.Reward = aReward[1] + " " + ((decmp[s + 5] << 8) + decmp[s + 4]);
			s += 15;
		}
		else if (decmp[s] == 3)
		{
			r_SeerHutRow.Reward = aReward[2] + " +" + decmp[s + 4];
			s += 15;
		}
		else if (decmp[s] == 4)
		{
			r_SeerHutRow.Reward = aReward[3] + " +" + decmp[s + 4];
			s += 15;
		}
		else if (decmp[s] == 5)
		{
			if (decmp[s + 4] < 6)
			{
				r_SeerHutRow.Reward = aReward[4] + " " + aResource[decmp[s + 4]] + ((decmp[s + 10] << 16) + (decmp[s + 9] << 8) + decmp[s + 8]);
			}
			else
			{
				r_SeerHutRow.Reward = aReward[4] + " " + aMine[6] + " " + ((decmp[s + 10] << 16) + (decmp[s + 9] << 8) + decmp[s + 8]);
			}
			s += 15;
		}
		else if (decmp[s] == 6)
		{
			r_SeerHutRow.Reward = aReward[5] + " " + aPrSkill[decmp[s + 4]] + "+" + decmp[s + 8];
			s += 15;
		}
		else if (decmp[s] == 7)
		{
			r_SeerHutRow.Reward = aReward[5] + " " + aLevelSkill[decmp[s + 8] - 1] + TblSecondarySkill.Rows[decmp[s + 4]][1];
			s += 15;
		}
		else if (decmp[s] == 8)
		{
			DataRow dataRow = TblArt.Rows.Find(decmp[s + 4]);
			r_SeerHutRow.Reward = aReward[6] + " " + (string)dataRow[1];
			if (r_AllArtsRow != null)
			{
				r_AllArtsRow.Artefact = (string)dataRow[1];
				r_AllArtsRow.Class = (string)dataRow[2];
				if (dataRow[3] != DBNull.Value)
				{
					r_AllArtsRow._Relic_C = (string)dataRow[3];
				}
				r_AllArtsRow.Mission = r_SeerHutRow.Mission;
			}
			s += 15;
		}
		else if (decmp[s] == 9)
		{
			r_SeerHutRow.Reward = aReward[7] + " " + TblSpell.Rows[decmp[s + 4]][1];
			s += 15;
		}
		else if (decmp[s] == 10)
		{
			r_SeerHutRow.Reward = string.Concat(aReward[8], " ", TblMonsters.Rows.Find(decmp[s + 4])[1], " ", ((decmp[s + 9] << 8) + decmp[s + 8]).ToString());
			s += 15;
		}
		if (r_AllArtsRow != null && r_AllArtsRow.Artefact == "")
		{
			r_AllArtsRow.Delete();
		}
		return --s;
	}

	private void SeerHutContent2()
	{
		DataSet2.R_SeerHutRow[] array = (DataSet2.R_SeerHutRow[])dsResult.R_SeerHut.Select("Mission LIKE '{*'");
		DataSet2.R_SeerHutRow[] array2 = (DataSet2.R_SeerHutRow[])dsResult.R_SeerHut.Select("Mission LIKE '}*'");
		DataSet2.R_AllArtsRow[] array3 = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("Mission LIKE '{*'");
		DataSet2.R_AllArtsRow[] array4 = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select("Mission LIKE '}*'");
		DataSet2.R_SeerHutRow[] array5 = array;
		foreach (DataSet2.R_SeerHutRow r_SeerHutRow in array5)
		{
			DataSet2.R_HeroesRow r_HeroesRow = (DataSet2.R_HeroesRow)dsResult.R_Heroes.Select("ID='" + r_SeerHutRow.Mission.Remove(0, 1) + "'")[0];
			r_SeerHutRow.Mission = аQuest[7] + " " + r_HeroesRow.Hero + " (" + r_HeroesRow.Place + ")";
		}
		DataSet2.R_SeerHutRow[] array6 = array2;
		foreach (DataSet2.R_SeerHutRow r_SeerHutRow2 in array6)
		{
			r_SeerHutRow2.Mission = аQuest[8] + " " + dsResult.R_Heroes.Select("ID='" + r_SeerHutRow2.Mission.Remove(0, 1) + "'")[0][1];
		}
		DataSet2.R_AllArtsRow[] array7 = array3;
		foreach (DataSet2.R_AllArtsRow r_AllArtsRow in array7)
		{
			DataSet2.R_HeroesRow r_HeroesRow2 = (DataSet2.R_HeroesRow)dsResult.R_Heroes.Select("ID='" + r_AllArtsRow.Mission.Remove(0, 1) + "'")[0];
			r_AllArtsRow.Mission = аQuest[7] + " " + r_HeroesRow2.Hero + " (" + r_HeroesRow2.Place + ")";
		}
		DataSet2.R_AllArtsRow[] array8 = array4;
		foreach (DataSet2.R_AllArtsRow r_AllArtsRow2 in array8)
		{
			r_AllArtsRow2.Mission = аQuest[8] + " " + dsResult.R_Heroes.Select("ID='" + r_AllArtsRow2.Mission.Remove(0, 1) + "'")[0][1];
		}
	}

	private int PassGuardContent(DataRow row, int s)
	{
		if (row == null)
		{
			return ScanPassGuardContent(s);
		}
		DataSet2.R_PassGuardRow r_PassGuardRow = (DataSet2.R_PassGuardRow)dsResult.R_PassGuard.Rows.Find(new object[3]
		{
			row[0],
			row[1],
			row[2]
		});
		if (decmp[s] == 0)
		{
			return s + 1;
		}
		if (decmp[s] == 1)
		{
			r_PassGuardRow.Mission = аQuest[0] + " " + decmp[s + 1];
			s += 5;
		}
		else if (decmp[s] == 2)
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				if (decmp[s + i + 1] != 0)
				{
					text = text + ", " + aPrSkill[i] + decmp[s + i + 1];
				}
			}
			if (text != "")
			{
				r_PassGuardRow.Mission = аQuest[1] + " " + text.Remove(0, 2);
			}
			s += 7;
		}
		else if (decmp[s] == 3)
		{
			r_PassGuardRow.Mission = "{" + decmp[s + 1];
			s += 6;
		}
		else if (decmp[s] == 4)
		{
			r_PassGuardRow.Mission = string.Concat(аQuest[2], " ", TblMonsters.Rows.Find(decmp[s + 5])[1], " (", decmp[s + 1].ToString(), ".", decmp[s + 3].ToString(), ".", ((decmp[s + 4] & 4) >> 2).ToString(), ")");
			s += 10;
		}
		else if (decmp[s] == 5)
		{
			string text2 = "";
			for (int j = 0; j < decmp[s + 1] * 2; j += 2)
			{
				text2 = text2 + ", " + TblArt.Rows.Find(decmp[s + j + 2])[1];
			}
			if (text2 != "")
			{
				r_PassGuardRow.Mission = аQuest[3] + " " + text2.Remove(0, 2);
			}
			s = s + decmp[s + 1] * 2 + 4;
		}
		else if (decmp[s] == 6)
		{
			string text3 = "";
			for (int k = 0; k < decmp[s + 1] * 6; k += 6)
			{
				text3 = string.Concat(text3, ", ", TblMonsters.Rows.Find(decmp[s + k + 2])[1], " ", ((decmp[s + k + 5] << 8) + decmp[s + k + 4]).ToString());
			}
			if (text3 != "")
			{
				r_PassGuardRow.Mission = аQuest[4] + " " + text3.Remove(0, 2);
			}
			s = s + decmp[s + 1] * 6 + 4;
		}
		else if (decmp[s] == 7)
		{
			string text4 = "";
			int num = 0;
			for (int l = 0; l < 6; l++)
			{
				num = (decmp[s + l * 4 + 3] << 16) + (decmp[s + l * 4 + 2] << 8) + decmp[s + l * 4 + 1];
				if (num > 0)
				{
					text4 = text4 + ", " + aResource[l] + num;
				}
			}
			num = (decmp[s + 24 + 3] << 16) + (decmp[s + 24 + 2] << 8) + decmp[s + 24 + 1];
			if (num > 0)
			{
				text4 = text4 + ", " + aMine[6] + " " + num;
			}
			if (text4 != "")
			{
				r_PassGuardRow.Mission = аQuest[5] + " " + text4.Remove(0, 2);
			}
			s += 31;
		}
		else if (decmp[s] == 8)
		{
			r_PassGuardRow.Mission = "}" + decmp[s + 1];
			s += 5;
		}
		else if (decmp[s] == 9)
		{
			r_PassGuardRow.Mission = аQuest[6] + " " + aColor[decmp[s + 1]];
			s += 4;
		}
		if (decmp[s] != byte.MaxValue && decmp[s + 1] != byte.MaxValue)
		{
			int num2 = decmp[s] / 28;
			int num3 = (decmp[s] - num2 * 28) / 7;
			int num4 = decmp[s] - num2 * 28 - num3 * 7;
			r_PassGuardRow.Deadline = (num2 + 1).ToString() + (num3 + 1) + (num4 + 1);
		}
		s += 4;
		for (int m = 0; m < 3; m++)
		{
			s += (decmp[s + 1] << 8) + decmp[s] + 4;
		}
		return s;
	}

	private void PassGuardContent2()
	{
		DataSet2.R_PassGuardRow[] array = (DataSet2.R_PassGuardRow[])dsResult.R_PassGuard.Select("Mission LIKE '{*'");
		DataSet2.R_PassGuardRow[] array2 = (DataSet2.R_PassGuardRow[])dsResult.R_PassGuard.Select("Mission LIKE '}*'");
		DataSet2.R_PassGuardRow[] array3 = array;
		foreach (DataSet2.R_PassGuardRow r_PassGuardRow in array3)
		{
			DataSet2.R_HeroesRow r_HeroesRow = (DataSet2.R_HeroesRow)dsResult.R_Heroes.Select("ID='" + r_PassGuardRow.Mission.Remove(0, 1) + "'")[0];
			r_PassGuardRow.Mission = аQuest[7] + " " + r_HeroesRow.Hero + " (" + r_HeroesRow.Place + ")";
		}
		DataSet2.R_PassGuardRow[] array4 = array2;
		foreach (DataSet2.R_PassGuardRow r_PassGuardRow2 in array4)
		{
			r_PassGuardRow2.Mission = аQuest[8] + " " + dsResult.R_Heroes.Select("ID='" + r_PassGuardRow2.Mission.Remove(0, 1) + "'")[0][1];
		}
	}

	private void GetBankContent()
	{
		int num = 0;
		int s = map.Bank + 2;
		DataView defaultView = TblBanks.DefaultView;
		defaultView.Sort = "Num";
		foreach (DataRowView item in defaultView)
		{
			if (num == (int)item.Row["Num"])
			{
				s = BankContent(item.Row, s);
				num++;
				continue;
			}
			for (int i = num; i < (int)item.Row["Num"]; i++)
			{
				s = BankContent(null, s);
				num++;
			}
			s = BankContent(item.Row, s);
			num++;
		}
	}

	private int BankContent(DataRow row, int s)
	{
		if (row == null)
		{
			s += 89;
			s += decmp[s] * 4 + 2;
			return s;
		}
		if (!(bool)row[4])
		{
			DataSet2.R_BankRow r_BankRow = (DataSet2.R_BankRow)dsResult.R_Bank.Select("X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'")[0];
			if (spt)
			{
				r_BankRow.Address = s;
			}
			r_BankRow.Guard = GetBankGuard(s, out var hp);
			r_BankRow.HP = hp;
			s += 56;
			int gold;
			string bankResource = GetBankResource(s, out gold);
			if (bankResource != "")
			{
				r_BankRow.Resource = bankResource.Remove(0, 2);
			}
			if (gold != 0)
			{
				r_BankRow.Gold = gold;
			}
			s += 28;
			if (decmp[s] < byte.MaxValue)
			{
				r_BankRow.Monster = GetBankMonster(s);
			}
			s += 7;
		}
		else
		{
			string filterExpression = "X='" + row[0].ToString() + "' AND Y='" + row[1].ToString() + "' AND Z='" + row[2].ToString() + "'";
			DataSet2.R_ArtRow[] array = (DataSet2.R_ArtRow[])dsResult.R_Art.Select(filterExpression);
			DataSet2.R_AllArtsRow[] array2 = (DataSet2.R_AllArtsRow[])dsResult.R_AllArts.Select(filterExpression);
			if (spt)
			{
				array[0].Address = s;
			}
			array[0].Guard = GetBankGuard(s, out var hp2);
			array[0].HP = hp2;
			s += 56;
			int gold2;
			string bankResource2 = GetBankResource(s, out gold2);
			if (bankResource2 != "")
			{
				array[0].Resource = bankResource2.Remove(0, 2);
			}
			if (gold2 != 0)
			{
				array[0].Gold = gold2;
			}
			s = s + 28 + 5;
			int num = decmp[s];
			if (num == 0)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Delete();
					if (i > 0)
					{
						array[i].Delete();
					}
				}
				s += 2;
			}
			else
			{
				s += 2;
				array2[0].Guard = array[0].Guard;
				for (int j = 0; j < array2.Length; j++)
				{
					if (j < num)
					{
						DataRow dataRow = TblArt.Rows.Find(decmp[s + j * 4]);
						DataSet2.R_AllArtsRow obj = array2[j];
						string artefact = (array[j].Name = (string)dataRow[1]);
						obj.Artefact = artefact;
						DataSet2.R_AllArtsRow obj2 = array2[j];
						string @class = (array[j].Class = (string)dataRow[2]);
						obj2.Class = @class;
						if (dataRow[3] != DBNull.Value)
						{
							DataSet2.R_AllArtsRow obj3 = array2[j];
							string relic_C = (array[j]._Relic_C = (string)dataRow[3]);
							obj3._Relic_C = relic_C;
						}
						if (num > 1)
						{
							DataSet2.R_ArtRow obj4 = array[j];
							int slot = (array2[j].Slot = j + 1);
							obj4.Slot = slot;
						}
					}
					else
					{
						array2[j].Delete();
						array[j].Delete();
					}
				}
				s += num * 4;
			}
		}
		return s;
	}

	private string GetBankMonster(int s)
	{
		return (string)TblMonsters.Rows.Find(decmp[s])[1] + " " + decmp[s + 4];
	}

	private string GetBankResource(int s, out int gold)
	{
		string text = "";
		for (int i = 0; i < 6; i++)
		{
			int num = decmp[s + i * 4];
			if (num > 0)
			{
				text = text + ", " + aResource[i] + num;
			}
		}
		gold = (decmp[s + 25] << 8) + decmp[s + 24];
		return text;
	}

	private string GetBankGuard(int s, out int hp)
	{
		int[,] array = new int[7, 2];
		int num = 0;
		string text = "";
		hp = 0;
		for (int i = 0; i < 7; i++)
		{
			int num2 = decmp[s + i * 4];
			if (num2 != 255)
			{
				if (ItWas(array, num2, num, out var p))
				{
					array[p, 1] += decmp[s + i * 4 + 28];
					continue;
				}
				array[num, 0] = decmp[s + i * 4];
				array[num, 1] = decmp[s + i * 4 + 28];
				num++;
			}
		}
		for (int j = 0; j < num; j++)
		{
			DataRow dataRow = TblMonsters.Rows.Find(array[j, 0]);
			text = string.Concat(text, ", ", dataRow[1], " ", array[j, 1].ToString());
			hp += (int)dataRow[3] * array[j, 1];
		}
		if (!(text != ""))
		{
			return "";
		}
		return text.Remove(0, 2);
	}

	private bool ItWas(int[,] aGuard, int guard, int count, out int p)
	{
		for (int i = 0; i < count; i++)
		{
			if (aGuard[i, 0] == guard)
			{
				p = i;
				return true;
			}
		}
		p = 0;
		return false;
	}

	private int ScanTownsTimedEvents(int s)
	{
		int num = (decmp[s + 1] << 8) + decmp[s];
		if (num > 0)
		{
			s += 4;
			for (int i = 0; i < num; i++)
			{
				s += (decmp[s + 1] << 8) + decmp[s] + 60;
			}
		}
		else
		{
			s += 4;
		}
		return s;
	}

	private int ScanMapTimedEvents(int s)
	{
		int num = (decmp[s + 1] << 8) + decmp[s];
		if (num > 0)
		{
			s += 4;
			for (int i = 0; i < num; i++)
			{
				s += (decmp[s + 1] << 8) + decmp[s] + 37;
			}
		}
		else
		{
			s += 4;
		}
		return s;
	}

	private int ScanHeroesContent(int s)
	{
		for (int i = 0; i < HeroCount; i++)
		{
			if (decmp[s] != byte.MaxValue && decmp[s + 11] != 0)
			{
				aHeroID[i] = s;
			}
			s += (decmp[s + 23] << 8) + decmp[s + 22] + 1094;
		}
		return s;
	}

	private int ScanBottleSignContent(int s)
	{
		if (decmp[s] != 0)
		{
			int num = decmp[s];
			s++;
			for (int i = 0; i < num; i++)
			{
				int num2 = (decmp[s + 1] << 8) + decmp[s];
				s = ((num2 <= 0) ? (s + 3) : (s + (num2 + 3)));
			}
		}
		else
		{
			s++;
		}
		return s;
	}

	private int ScanObjectContent(int s, ScanContent ObjContent)
	{
		int num = (decmp[s + 1] << 8) + decmp[s];
		if (num > 0)
		{
			s += 2;
			for (int i = 0; i < num; i++)
			{
				s = ObjContent(s) + 1;
			}
		}
		else
		{
			s += 2;
		}
		return s;
	}

	private int ScanMonstrContent(int s)
	{
		int num = (decmp[s + 1] << 8) + decmp[s];
		if (num > 0)
		{
			s += 2;
			for (int i = 0; i < num; i++)
			{
				if (i < 256)
				{
					aMonstrID[i] = s;
				}
				s += (decmp[s + 1] << 8) + decmp[s] + 31;
			}
		}
		else
		{
			s += 2;
		}
		return s;
	}

	private int ScanTownsContent(int s)
	{
		int num = decmp[s];
		s++;
		for (int i = 0; i < num; i++)
		{
			s += decmp[s + 70] + 382;
		}
		return s;
	}

	private int ScanEventBoxContent(int s)
	{
		if (decmp[s] > 0)
		{
			s = s + (decmp[s + 2] << 8) + decmp[s + 1] + 3;
		}
		s = ((decmp[s] != 1) ? (s + 1) : (s + 57));
		s += 42;
		s = ((decmp[s] <= 0) ? (s + 1) : (s + decmp[s] * 2 + 1));
		s = ((decmp[s] <= 0) ? (s + 1) : (s + decmp[s] + 1));
		s = ((decmp[s] <= 0) ? (s + 1) : (s + decmp[s] + 1));
		s = ((decmp[s] <= 0) ? (s + 1) : (s + decmp[s] * 4 + 1));
		return --s;
	}

	private int ScanArtResContent(int s)
	{
		s = s + (decmp[s + 1] << 8) + decmp[s] + 2;
		s = ((decmp[s] != 1) ? (s + 1) : (s + 57));
		return --s;
	}

	private int ScanSeerHutContent(int s)
	{
		if (decmp[s] == 0)
		{
			return s + 15;
		}
		if (decmp[s] == 1)
		{
			s += 5;
		}
		else if (decmp[s] == 2)
		{
			s += 7;
		}
		else if (decmp[s] == 3)
		{
			s += 6;
		}
		else if (decmp[s] == 4)
		{
			s += 10;
		}
		else if (decmp[s] == 5)
		{
			s = s + decmp[s + 1] * 2 + 4;
		}
		else if (decmp[s] == 6)
		{
			s = s + decmp[s + 1] * 6 + 4;
		}
		else if (decmp[s] == 7)
		{
			s += 31;
		}
		else if (decmp[s] == 8)
		{
			s += 5;
		}
		else if (decmp[s] == 9)
		{
			s += 4;
		}
		s += 4;
		for (int i = 0; i < 3; i++)
		{
			s = (((decmp[s + 1] << 8) + decmp[s] != 0) ? (s + (decmp[s + 1] << 8) + decmp[s] + 4) : (s + 4));
		}
		s += 15;
		return --s;
	}

	private int ScanPassGuardContent(int s)
	{
		if (decmp[s] == 0)
		{
			return s + 1;
		}
		if (decmp[s] == 1)
		{
			s += 5;
		}
		else if (decmp[s] == 2)
		{
			s += 7;
		}
		else if (decmp[s] == 3)
		{
			s += 6;
		}
		else if (decmp[s] == 4)
		{
			s += 10;
		}
		else if (decmp[s] == 5)
		{
			s = s + decmp[s + 1] * 2 + 4;
		}
		else if (decmp[s] == 6)
		{
			s = s + decmp[s + 1] * 6 + 4;
		}
		else if (decmp[s] == 7)
		{
			s += 31;
		}
		else if (decmp[s] == 8)
		{
			s += 5;
		}
		else if (decmp[s] == 9)
		{
			s += 4;
		}
		s += 4;
		for (int i = 0; i < 3; i++)
		{
			s += (decmp[s + 1] << 8) + decmp[s] + 4;
		}
		return s;
	}

	private int ScanBankContent(int s)
	{
		int num = (decmp[s + 1] << 8) + decmp[s];
		s += 2;
		for (int i = 0; i < num; i++)
		{
			s += 89;
			s += decmp[s] * 4 + 2;
		}
		return s;
	}

	private int GetPairSubterraneanGate()
	{
		int subTerGate = map.SubTerGate;
		int num = (decmp[subTerGate + 1] << 8) + decmp[subTerGate];
		subTerGate += 2;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = subTerGate + i * 4;
		}
		subTerGate += num * 4;
		num = (decmp[subTerGate + 1] << 8) + decmp[subTerGate];
		subTerGate += 2;
		int[] array2 = new int[num];
		for (int j = 0; j < num; j++)
		{
			if (decmp[subTerGate + j * 4] == byte.MaxValue && decmp[subTerGate + j * 4 + 1] == byte.MaxValue)
			{
				array2[j] = -1;
			}
			else
			{
				array2[j] = (decmp[subTerGate + j * 4 + 1] << 8) + decmp[subTerGate + j * 4];
			}
		}
		subTerGate += num * 4;
		int num2 = array2.Length - 1;
		for (int k = 0; k < array2.Length && k < num2; k++)
		{
			for (int num3 = array2.Length - 1; num3 > k; num3--)
			{
				if (array2[num3] == k)
				{
					num2--;
					try
					{
						DataSet2.R_TopologyRow r_TopologyRow = (DataSet2.R_TopologyRow)dsResult.R_Topology.Select("X='" + decmp[array[array2[k]]] + "' AND Y='" + decmp[array[array2[k]] + 2] + "' AND Z='" + ((decmp[array[array2[k]] + 3] & 4) >> 2) + "'")[0];
						r_TopologyRow.Pair = k;
					}
					catch
					{
					}
					try
					{
						DataSet2.R_TopologyRow r_TopologyRow2 = (DataSet2.R_TopologyRow)dsResult.R_Topology.Select("X='" + decmp[array[k]] + "' AND Y='" + decmp[array[k] + 2] + "' AND Z='" + ((decmp[array[k] + 3] & 4) >> 2) + "'")[0];
						r_TopologyRow2.Pair = k;
					}
					catch
					{
					}
					break;
				}
			}
		}
		return subTerGate;
	}

	private int ScanMonolithWhirlpool(int s)
	{
		for (int i = 0; i < 8; i++)
		{
			int num = (decmp[s + 1] << 8) + decmp[s];
			s = s + num * 4 + 2;
		}
		map.OneWayMonolith = s;
		for (int j = 0; j < 8; j++)
		{
			int num2 = (decmp[s + 1] << 8) + decmp[s];
			s = s + num2 * 4 + 2;
		}
		map.Whirlpool = s;
		int num3 = (decmp[s + 1] << 8) + decmp[s];
		s = s + num3 * 4 + 2;
		return s;
	}

	private int GetMapStart(int s)
	{
		do
		{
			s--;
		}
		while (decmp[s] != 0);
		map.SaveName = s + 1;
		s += 688;
		map.SR = s;
		Get_SR(s);
		s += 28;
		int num = (decmp[s + 1] << 8) + decmp[s];
		s += num + 258;
		num = (decmp[s + 1] << 8) + decmp[s];
		s += 4;
		if (num != 0)
		{
			for (int i = 0; i < num; i++)
			{
				s = s + (decmp[s + 1] << 8) + decmp[s] + 3;
			}
		}
		num = decmp[s];
		if (num != 0)
		{
			map.BlackMarket = s;
			s += num * 28;
		}
		return map.Start = s + 1;
	}

	private int GetStart()
	{
		Encoding aSCII = Encoding.ASCII;
		int i;
		for (i = map.Data + 66; decmp[i] != 0 || decmp[i + 1] != 1 || decmp[i + 2] != 2 || decmp[i + 3] != 3 || decmp[i + 4] != 4 || decmp[i + 5] != 5 || decmp[i + 6] != 6 || decmp[i + 7] != 7 || decmp[i + 8] != 0 || decmp[i + 9] != 1 || decmp[i + 10] != 2 || decmp[i + 11] != 3 || decmp[i + 12] != 4 || decmp[i + 13] != 5 || decmp[i + 14] != 6 || decmp[i + 15] != 7; i++)
		{
		}
		map.Teams = i;
		map.MapName = i + 57;
		i = map.MapName + 341;
		while (true)
		{
			if (decmp[i] == 46)
			{
				string @string = aSCII.GetString(new byte[3]
				{
					decmp[i + 1],
					decmp[i + 2],
					decmp[i + 3]
				});
				if ((@string.ToUpper() == "GM1" || @string.ToUpper() == "CGM" || @string.ToUpper() == "GM2" || @string.ToUpper() == "GM3") && decmp[i + 4] == 0)
				{
					break;
				}
			}
			i++;
		}
		return GetMapStart(i);
	}

	private bool FirstID(int s)
	{
		int num = (decmp[s + 5] << 8) + decmp[s + 4];
		if (aObjectID[num] == 0)
		{
			aObjectID[num]++;
			return true;
		}
		return false;
	}

	private void IsObject(int s, int c, int z, int l, int loc)
	{
		int mapSize = MapSize;
		int num = (c - l) / mapSize;
		int x = c - l - mapSize * num;
		if (decmp[s] == 5 && FirstID(s))
		{
			SaveArt(x, num, z, s, loc);
		}
		else if (decmp[s] == 79 && FirstID(s))
		{
			SaveRes(x, num, z, s, loc);
		}
		else if (decmp[s] == 101 && FirstID(s))
		{
			SaveChest(x, num, z, s, loc);
		}
		else if (decmp[s] == 82 && FirstID(s))
		{
			SaveSeaChest(x, num, z, s, loc);
		}
		else if (decmp[s] == 54 && FirstID(s))
		{
			SaveMonster(x, num, z, s, loc);
		}
		else if (decmp[s] == 53 && FirstID(s))
		{
			SaveMine(x, num, z, s, loc);
		}
		else if ((decmp[s] == 17 || decmp[s] == 20) && FirstID(s))
		{
			SaveMine(x, num, z, s, loc);
		}
		else if (decmp[s] == 12 && FirstID(s))
		{
			SaveCampfire(x, num, z, s, loc);
		}
		else if (decmp[s] == 112 && FirstID(s))
		{
			SaveWindmill(x, num, z, s, loc);
		}
		else if (decmp[s] == 55 && FirstID(s))
		{
			SaveMysticalGarden(x, num, z, s, loc);
		}
		else if (decmp[s] == 108 && FirstID(s))
		{
			SaveTomb(x, num, z, s, loc);
		}
		else if (decmp[s] == 6 && FirstID(s))
		{
			SaveBox(x, num, z, s, loc);
		}
		else if (decmp[s] == 26)
		{
			SaveEvent(x, num, z, s, loc);
		}
		else if (decmp[s] == 86 && FirstID(s))
		{
			SaveSurvivor(x, num, z, s, loc);
		}
		else if ((decmp[s] == 84 || decmp[s] == 85 || decmp[s] == 25 || decmp[s] == 24 || decmp[s] == 16) && FirstID(s))
		{
			SaveBanks(x, num, z, s, loc);
		}
		else if (decmp[s] == 81 && FirstID(s))
		{
			SaveScholar(x, num, z, s, loc);
		}
		else if (decmp[s] == 93 && FirstID(s))
		{
			SaveScroll(x, num, z, s, loc);
		}
		else if (decmp[s] == 39 && FirstID(s))
		{
			SaveHovel(x, num, z, s, loc);
		}
		else if (decmp[s] == 29 && FirstID(s))
		{
			SaveFloatsam(x, num, z, s, loc);
		}
		else if ((decmp[s] == 88 || decmp[s] == 89 || decmp[s] == 90) && FirstID(s))
		{
			SaveShrine(x, num, z, s, loc);
		}
		else if (decmp[s] == 63 && FirstID(s))
		{
			SavePyramid(x, num, z, s, loc);
		}
		else if (decmp[s] == 22 && FirstID(s))
		{
			SaveSkeleton(x, num, z, s, loc);
		}
		else if (decmp[s] == 105 && FirstID(s))
		{
			SaveWagon(x, num, z, s, loc);
		}
		else if (decmp[s] == 113 && FirstID(s))
		{
			SaveWitchHut(x, num, z, s, loc);
		}
		else if (decmp[s] == 104 && FirstID(s))
		{
			SaveUniver(x, num, z, s, loc);
		}
		else if (decmp[s] == 78 && FirstID(s))
		{
			SaveCamp(x, num, z, s, loc);
		}
		else if (decmp[s] == 7 && FirstID(s))
		{
			SaveMarket(x, num, z, s, loc);
		}
		else if (decmp[s] == 33 && FirstID(s))
		{
			SaveGarrison(x, num, z, s, loc);
		}
		else if (decmp[s] == 83 && FirstID(s))
		{
			SaveSeerHut(x, num, z, s, loc);
		}
		else if (decmp[s] == 215 && FirstID(s))
		{
			SavePassDuard(x, num, z, s, loc);
		}
		else if (decmp[s] == 62 && FirstID(s))
		{
			SavePrison(x, num, z, s, loc);
		}
		else if (decmp[s] == 100 && FirstID(s))
		{
			SaveLearningStone(x, num, z, s, loc);
		}
		else if (decmp[s] == 34)
		{
			if (decmp[s + 2] != byte.MaxValue && decmp[s + 3] != byte.MaxValue && decmp[s + 4] != byte.MaxValue && decmp[s + 5] != byte.MaxValue)
			{
				HeroOnObject(s, c, z, l, loc);
			}
			SaveHero(x, num, z, s, loc, c, l);
		}
		else if ((decmp[s] == 2 || decmp[s] == 35 || decmp[s] == 95 || decmp[s] == 102 || decmp[s] == 213) && FirstID(s))
		{
			SaveObject(x, num, z, s, loc);
		}
		else if ((decmp[s] == 43 || decmp[s] == 44 || decmp[s] == 45) && FirstID(s))
		{
			SaveMonolith(x, num, z, s, loc);
		}
		else if (decmp[s] == 10 && FirstID(s))
		{
			SaveTent(x, num, z, s, loc);
		}
		else if (decmp[s] == 103 && FirstID(s))
		{
			SaveTopologyObj(x, num, z, s, loc);
		}
	}

	private void HeroOnObject(int s, int c, int z, int l, int loc)
	{
		byte[] array = new byte[5];
		int num = aHeroID[decmp[s + 6]];
		array[0] = decmp[s];
		array[1] = decmp[s + 6];
		array[2] = decmp[s + 7];
		array[3] = decmp[s + 8];
		array[4] = decmp[s + 9];
		decmp[s] = decmp[num + 11];
		decmp[s + 6] = decmp[num + 16];
		decmp[s + 7] = decmp[num + 17];
		decmp[s + 8] = decmp[num + 18];
		decmp[s + 9] = decmp[num + 19];
		IsObject(s, c, z, l, loc);
		decmp[s] = array[0];
		decmp[s + 6] = array[1];
		decmp[s + 7] = array[2];
		decmp[s + 8] = array[3];
		decmp[s + 9] = array[4];
	}

	private void SaveMine(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_MineRow r_MineRow = dsResult.R_Mine.NewR_MineRow();
		r_MineRow.X = x;
		r_MineRow.Y = y;
		r_MineRow.Z = z;
		r_MineRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		dsResult.R_Mine.Rows.Add(r_MineRow);
	}

	private void SaveHero(int x, int y, int z, int s, int loc, int c, int l)
	{
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		r_AllArtsRow.X = x;
		r_AllArtsRow.Y = y;
		r_AllArtsRow.Z = z;
		r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_AllArtsRow.Object = TblObject.Select("Code='34'")[0][1].ToString();
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		if (decmp[s + 4] != byte.MaxValue && decmp[s + 5] != byte.MaxValue)
		{
			int num = decmp[s + 6];
			int num2 = decmp[s + 7];
			int num3 = aHeroID[num];
			decmp[s] = decmp[num3 + 11];
			decmp[s + 6] = decmp[num3 + 16];
			decmp[s + 7] = decmp[num3 + 17];
			IsObject(s, c, z, l, loc);
			decmp[s] = 34;
			decmp[s + 6] = (byte)num;
			decmp[s + 7] = (byte)num2;
		}
	}

	private void SaveLearningStone(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_AllExperienceRow r_AllExperienceRow = dsResult.R_AllExperience.NewR_AllExperienceRow();
		r_AllExperienceRow.X = x;
		r_AllExperienceRow.Y = y;
		r_AllExperienceRow.Z = z;
		r_AllExperienceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_AllExperienceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_AllExperienceRow.Experience = 1000;
		dsResult.R_AllExperience.Rows.Add(r_AllExperienceRow);
	}

	private void SaveFloatsam(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		if (spt)
		{
			r_ResourceRow.Address = s;
		}
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 6] == 1)
		{
			r_ResourceRow.Resource = "W5";
		}
		else if (decmp[s + 6] == 2)
		{
			r_ResourceRow.Resource = "W5";
			r_ResourceRow.Gold = 200;
		}
		else if (decmp[s + 6] == 3)
		{
			r_ResourceRow.Resource = "W10";
			r_ResourceRow.Gold = 500;
		}
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
	}

	private void SaveObject(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ObjectRow r_ObjectRow = dsResult.R_Object.NewR_ObjectRow();
		r_ObjectRow.X = x;
		r_ObjectRow.Y = y;
		r_ObjectRow.Z = z;
		r_ObjectRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ObjectRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s] == 102)
		{
			if (spt)
			{
				r_ObjectRow.Address = s;
			}
			if (decmp[s + 7] == 32)
			{
				r_ObjectRow.Payment = "2000";
			}
			else if (decmp[s + 7] == 64)
			{
				r_ObjectRow.Payment = "G10";
			}
		}
		dsResult.R_Object.Rows.Add(r_ObjectRow);
	}

	private void SaveMonolith(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_TopologyRow r_TopologyRow = dsResult.R_Topology.NewR_TopologyRow();
		r_TopologyRow.X = x;
		r_TopologyRow.Y = y;
		r_TopologyRow.Z = z;
		r_TopologyRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_TopologyRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_TopologyRow.Type = decmp[s + 2].ToString();
		dsResult.R_Topology.Rows.Add(r_TopologyRow);
	}

	private void SaveTent(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_TopologyRow r_TopologyRow = dsResult.R_Topology.NewR_TopologyRow();
		r_TopologyRow.X = x;
		r_TopologyRow.Y = y;
		r_TopologyRow.Z = z;
		r_TopologyRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_TopologyRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_TopologyRow.Color = aTent[decmp[s + 2]];
		dsResult.R_Topology.Rows.Add(r_TopologyRow);
	}

	private void SaveTopologyObj(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_TopologyRow r_TopologyRow = dsResult.R_Topology.NewR_TopologyRow();
		r_TopologyRow.X = x;
		r_TopologyRow.Y = y;
		r_TopologyRow.Z = z;
		r_TopologyRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_TopologyRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		dsResult.R_Topology.Rows.Add(r_TopologyRow);
	}

	private void SaveHovel(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		if (spt)
		{
			r_ResourceRow.Address = s;
		}
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_ResourceRow.Resource = aResource[(decmp[s + 7] ^ (decmp[s + 7] & 0xC1)) / 4] + (((decmp[s + 7] & 1) << 2) + ((decmp[s + 6] >> 6) & 3));
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
	}

	private void SaveCampfire(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		if (spt)
		{
			r_ResourceRow.Address = s;
		}
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_ResourceRow.Resource = aResource[decmp[s + 6] & 0xF] + ((decmp[s + 6] & 0xF0) >> 4);
		r_ResourceRow.Gold = ((decmp[s + 6] & 0xF0) >> 4) * 100;
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
	}

	private void SaveWindmill(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		if (spt)
		{
			r_ResourceRow.Address = s;
		}
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_ResourceRow.Resource = aResource[decmp[s + 6] & 0xF] + ((decmp[s + 7] & 0xF0) >> 5);
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
	}

	private void SaveMysticalGarden(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		if (spt)
		{
			r_ResourceRow.Address = s;
		}
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if ((decmp[s + 6] & 0x80) > 0)
		{
			r_ResourceRow.Gold = 500;
		}
		else
		{
			r_ResourceRow.Resource = "G5";
		}
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
	}

	private void SaveScholar(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ScholarRow r_ScholarRow = dsResult.R_Scholar.NewR_ScholarRow();
		r_ScholarRow.X = x;
		r_ScholarRow.Y = y;
		r_ScholarRow.Z = z;
		r_ScholarRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		if (decmp[s + 8] != byte.MaxValue)
		{
			r_ScholarRow.Spell = (string)TblSpell.Rows[((decmp[s + 8] & 0xF) << 3) + (decmp[s + 7] >> 5)][1];
		}
		else if (decmp[s + 7] != byte.MaxValue && decmp[s + 8] == byte.MaxValue)
		{
			r_ScholarRow.SecondarySkill = (string)TblSecondarySkill.Rows[((decmp[s + 7] & 7) << 2) + (decmp[s + 6] >> 6)][1];
		}
		else if (decmp[s + 7] == byte.MaxValue && decmp[s + 8] == byte.MaxValue)
		{
			if (decmp[s + 6] == 192)
			{
				r_ScholarRow.PrimarySkill = aPrSkill[0] + "+1";
			}
			if (decmp[s + 6] == 200)
			{
				r_ScholarRow.PrimarySkill = aPrSkill[1] + "+1";
			}
			if (decmp[s + 6] == 208)
			{
				r_ScholarRow.PrimarySkill = aPrSkill[2] + "+1";
			}
			if (decmp[s + 6] == 216)
			{
				r_ScholarRow.PrimarySkill = aPrSkill[3] + "+1";
			}
		}
		dsResult.R_Scholar.Rows.Add(r_ScholarRow);
	}

	private void SaveBanks(int x, int y, int z, int s, int loc)
	{
		DataRow dataRow = TblBanks.NewRow();
		dataRow["Num"] = (decmp[s + 8] << 8) + decmp[s + 7] >> 5;
		if (decmp[s] == 16)
		{
			DataSet2.R_BankRow r_BankRow = dsResult.R_Bank.NewR_BankRow();
			int num2 = (r_BankRow.X = x);
			dataRow[0] = num2;
			int num4 = (r_BankRow.Y = y);
			dataRow[1] = num4;
			int num6 = (r_BankRow.Z = z);
			dataRow[2] = num6;
			r_BankRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_BankRow.Name = TblObject.Select("Code='" + decmp[s] + "' AND Type='" + decmp[s + 2] + "'")[0][1].ToString();
			dataRow[3] = decmp[s];
			dataRow[4] = false;
			dsResult.R_Bank.Rows.Add(r_BankRow);
		}
		else
		{
			DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			int num8 = (r_ArtRow.X = x);
			int num10 = (r_AllArtsRow.X = num8);
			dataRow[0] = num10;
			int num12 = (r_ArtRow.Y = y);
			int num14 = (r_AllArtsRow.Y = num12);
			dataRow[1] = num14;
			int num16 = (r_ArtRow.Z = z);
			int num18 = (r_AllArtsRow.Z = num16);
			dataRow[2] = num18;
			string locality = (r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
			r_AllArtsRow.Locality = locality;
			if (decmp[s] == 84 || decmp[s] == 85 || decmp[s] == 24)
			{
				string @object = (r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
				r_AllArtsRow.Object = @object;
				dataRow[3] = decmp[s];
				dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
				dsResult.R_Art.Rows.Add(r_ArtRow);
			}
			else if (decmp[s] == 25)
			{
				string object2 = (r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
				r_AllArtsRow.Object = object2;
				dataRow[3] = decmp[s];
				string artefact = (r_ArtRow.Name = "1");
				r_AllArtsRow.Artefact = artefact;
				dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
				dsResult.R_Art.Rows.Add(r_ArtRow);
				for (int i = 2; i < 5; i++)
				{
					DataSet2.R_ArtRow r_ArtRow2 = dsResult.R_Art.NewR_ArtRow();
					DataSet2.R_AllArtsRow r_AllArtsRow2 = dsResult.R_AllArts.NewR_AllArtsRow();
					object[] itemArray2 = (r_ArtRow2.ItemArray = r_ArtRow.ItemArray);
					r_AllArtsRow2.ItemArray = itemArray2;
					string artefact2 = (r_ArtRow2.Name = i.ToString());
					r_AllArtsRow2.Artefact = artefact2;
					dsResult.R_AllArts.Rows.Add(r_AllArtsRow2);
					dsResult.R_Art.Rows.Add(r_ArtRow2);
				}
			}
			dataRow[4] = true;
		}
		TblBanks.Rows.Add(dataRow);
	}

	private void SaveUniver(int x, int y, int z, int s, int loc)
	{
		DataRow dataRow = TblUniver.NewRow();
		DataSet2.R_SkillRow r_SkillRow = dsResult.R_Skill.NewR_SkillRow();
		int num2 = (r_SkillRow.X = x);
		dataRow[0] = num2;
		int num4 = (r_SkillRow.Y = y);
		dataRow[1] = num4;
		int num6 = (r_SkillRow.Z = z);
		dataRow[2] = num6;
		r_SkillRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_SkillRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		dsResult.R_Skill.Rows.Add(r_SkillRow);
		TblUniver.Rows.Add(dataRow);
	}

	private void SaveMarket(int x, int y, int z, int s, int loc)
	{
		DataRow dataRow = TblMarket.NewRow();
		DataSet2.R_MarketRow r_MarketRow = dsResult.R_Market.NewR_MarketRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		int num2 = (r_MarketRow.X = x);
		int num4 = (r_AllArtsRow.X = num2);
		dataRow[0] = num4;
		int num6 = (r_MarketRow.Y = y);
		int num8 = (r_AllArtsRow.Y = num6);
		dataRow[1] = num8;
		int num10 = (r_MarketRow.Z = z);
		int num12 = (r_AllArtsRow.Z = num10);
		dataRow[2] = num12;
		string locality = (r_MarketRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		string @object = (r_MarketRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		dsResult.R_Market.Rows.Add(r_MarketRow);
		TblMarket.Rows.Add(dataRow);
	}

	private void SaveGarrison(int x, int y, int z, int s, int loc)
	{
		DataRow dataRow = TblGarrison.NewRow();
		DataSet2.R_GarrisonRow r_GarrisonRow = dsResult.R_Garrison.NewR_GarrisonRow();
		int num2 = (r_GarrisonRow.X = x);
		dataRow[0] = num2;
		int num4 = (r_GarrisonRow.Y = y);
		dataRow[1] = num4;
		int num6 = (r_GarrisonRow.Z = z);
		dataRow[2] = num6;
		r_GarrisonRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_GarrisonRow.AntiMagic = aReply[decmp[s + 2]];
		dsResult.R_Garrison.Rows.Add(r_GarrisonRow);
		TblGarrison.Rows.Add(dataRow);
	}

	private void SaveSurvivor(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		if (spt)
		{
			r_ArtRow.Address = s;
		}
		int x2 = (r_ArtRow.X = x);
		r_AllArtsRow.X = x2;
		int y2 = (r_ArtRow.Y = y);
		r_AllArtsRow.Y = y2;
		int z2 = (r_ArtRow.Z = z);
		r_AllArtsRow.Z = z2;
		string locality = (r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		DataRow dataRow = TblArt.Rows.Find(decmp[s + 6]);
		string artefact = (r_ArtRow.Name = (string)dataRow[1]);
		r_AllArtsRow.Artefact = artefact;
		string @class = (r_ArtRow.Class = (string)dataRow[2]);
		r_AllArtsRow.Class = @class;
		if (dataRow[3] != DBNull.Value)
		{
			string relic_C = (r_ArtRow._Relic_C = (string)dataRow[3]);
			r_AllArtsRow._Relic_C = relic_C;
		}
		string @object = (r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		dsResult.R_Art.Rows.Add(r_ArtRow);
	}

	private void SaveEvent(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_EventBoxRow r_EventBoxRow = dsResult.R_EventBox.NewR_EventBoxRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		DataRow dataRow = TblEventBox.NewRow();
		int num2 = (r_EventBoxRow.X = x);
		int num4 = (r_AllArtsRow.X = num2);
		dataRow[0] = num4;
		int num6 = (r_EventBoxRow.Y = y);
		int num8 = (r_AllArtsRow.Y = num6);
		dataRow[1] = num8;
		int num10 = (r_EventBoxRow.Z = z);
		int num12 = (r_AllArtsRow.Z = num10);
		dataRow[2] = num12;
		dataRow["Num"] = ((decmp[s + 7] & 1) << 8) + decmp[s + 6];
		string locality = (r_EventBoxRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		r_EventBoxRow.Apply = ((decmp[s + 7] & 0xFC) >> 2) + ((decmp[s + 8] & 3) << 6) + "," + (decmp[s + 8] & 4);
		r_EventBoxRow.Repeat = aReply[((decmp[s + 8] & 8) >> 3) ^ 1];
		string @object = (r_EventBoxRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_EventBox.Rows.Add(r_EventBoxRow);
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		TblEventBox.Rows.Add(dataRow);
	}

	private void SaveBox(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_EventBoxRow r_EventBoxRow = dsResult.R_EventBox.NewR_EventBoxRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		DataRow dataRow = TblEventBox.NewRow();
		int num2 = (r_EventBoxRow.X = x);
		int num4 = (r_AllArtsRow.X = num2);
		dataRow[0] = num4;
		int num6 = (r_EventBoxRow.Y = y);
		int num8 = (r_AllArtsRow.Y = num6);
		dataRow[1] = num8;
		int num10 = (r_EventBoxRow.Z = z);
		int num12 = (r_AllArtsRow.Z = num10);
		dataRow[2] = num12;
		dataRow["Num"] = ((decmp[s + 7] & 1) << 8) + decmp[s + 6];
		string locality = (r_EventBoxRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		string @object = (r_EventBoxRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_EventBox.Rows.Add(r_EventBoxRow);
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		TblEventBox.Rows.Add(dataRow);
	}

	private void SaveArt(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		if (spt)
		{
			r_ArtRow.Address = s;
		}
		int x2 = (r_ArtRow.X = x);
		r_AllArtsRow.X = x2;
		int y2 = (r_ArtRow.Y = y);
		r_AllArtsRow.Y = y2;
		int z2 = (r_ArtRow.Z = z);
		r_AllArtsRow.Z = z2;
		string locality = (r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		DataRow dataRow = TblArt.Rows.Find(decmp[s + 2]);
		string artefact = (r_ArtRow.Name = (string)dataRow[1]);
		r_AllArtsRow.Artefact = artefact;
		string @class = (r_ArtRow.Class = (string)dataRow[2]);
		r_AllArtsRow.Class = @class;
		if (dataRow[3] != DBNull.Value)
		{
			string relic_C = (r_ArtRow._Relic_C = (string)dataRow[3]);
			r_AllArtsRow._Relic_C = relic_C;
		}
		string @object = (r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_Art.Rows.Add(r_ArtRow);
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		if ((decmp[s + 9] & 0x80) == 128)
		{
			DataRow dataRow2 = TblArtRes.NewRow();
			dataRow2[0] = x;
			dataRow2[1] = y;
			dataRow2[2] = z;
			dataRow2["Num"] = ((decmp[s + 9] & 0x7F) << 8) + decmp[s + 8] >> 3;
			dataRow2["Type"] = 0;
			TblArtRes.Rows.Add(dataRow2);
		}
	}

	private void SaveSeerHut(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_SeerHutRow r_SeerHutRow = dsResult.R_SeerHut.NewR_SeerHutRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		DataRow dataRow = TblSeerHut.NewRow();
		int x2 = (r_SeerHutRow.X = x);
		r_AllArtsRow.X = x2;
		dataRow[0] = x + 1;
		int num3 = (r_SeerHutRow.Y = y);
		int num5 = (r_AllArtsRow.Y = num3);
		dataRow[1] = num5;
		int num7 = (r_SeerHutRow.Z = z);
		int num9 = (r_AllArtsRow.Z = num7);
		dataRow[2] = num9;
		dataRow["Num"] = decmp[s + 6];
		string locality = (r_SeerHutRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		r_AllArtsRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		dsResult.R_SeerHut.Rows.Add(r_SeerHutRow);
		TblSeerHut.Rows.Add(dataRow);
	}

	private void SavePrison(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_PrisonRow r_PrisonRow = dsResult.R_Prison.NewR_PrisonRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		int x2 = (r_PrisonRow.X = x);
		r_AllArtsRow.X = x2;
		int y2 = (r_PrisonRow.Y = y);
		r_AllArtsRow.Y = y2;
		int z2 = (r_PrisonRow.Z = z);
		r_AllArtsRow.Z = z2;
		string locality = (r_PrisonRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		r_AllArtsRow.Object = TblObject.Select("Code='34'")[0][1].ToString();
		r_AllArtsRow.Place = aPlace[0];
		r_PrisonRow.Hero = decmp[s + 6].ToString();
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		dsResult.R_Prison.Rows.Add(r_PrisonRow);
	}

	private void SavePassDuard(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_PassGuardRow r_PassGuardRow = dsResult.R_PassGuard.NewR_PassGuardRow();
		DataRow dataRow = TblPassGuard.NewRow();
		int num2 = (r_PassGuardRow.X = x);
		dataRow[0] = num2;
		int num4 = (r_PassGuardRow.Y = y);
		dataRow[1] = num4;
		int num6 = (r_PassGuardRow.Z = z);
		dataRow[2] = num6;
		dataRow["Num"] = (decmp[s + 7] << 8) + decmp[s + 6];
		r_PassGuardRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		dsResult.R_PassGuard.Rows.Add(r_PassGuardRow);
		TblPassGuard.Rows.Add(dataRow);
	}

	private void SaveChest(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ChestRow r_ChestRow = dsResult.R_Chest.NewR_ChestRow();
		if (spt)
		{
			r_ChestRow.Address = s;
		}
		r_ChestRow.X = x;
		r_ChestRow.Y = y;
		r_ChestRow.Z = z;
		r_ChestRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ChestRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 7] == 147)
		{
			r_ChestRow.Gold = 1000;
			r_ChestRow.HP = 500;
		}
		else if (decmp[s + 7] == 155)
		{
			r_ChestRow.Gold = 1500;
			r_ChestRow.HP = 1000;
		}
		else if (decmp[s + 7] == 163)
		{
			r_ChestRow.Gold = 2000;
			r_ChestRow.HP = 1500;
		}
		else if (decmp[s + 7] == 252)
		{
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			r_AllArtsRow.X = x;
			r_AllArtsRow.Y = y;
			r_AllArtsRow.Z = z;
			r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_AllArtsRow.Object = r_ChestRow.Object;
			GetArt(r_AllArtsRow, decmp[s + 6]);
			r_ChestRow.Art = r_AllArtsRow.Artefact;
			r_ChestRow.Class = r_AllArtsRow.Class;
			if (!r_AllArtsRow.Is_Relic_CNull())
			{
				r_ChestRow._Relic_C = r_AllArtsRow._Relic_C;
			}
			dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		}
		dsResult.R_Chest.Rows.Add(r_ChestRow);
	}

	private void SaveSeaChest(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ChestRow r_ChestRow = dsResult.R_Chest.NewR_ChestRow();
		if (spt)
		{
			r_ChestRow.Address = s;
		}
		r_ChestRow.X = x;
		r_ChestRow.Y = y;
		r_ChestRow.Z = z;
		r_ChestRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ChestRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 7] == byte.MaxValue)
		{
			if (decmp[s + 6] == 249)
			{
				r_ChestRow.Gold = 1500;
			}
		}
		else
		{
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			r_AllArtsRow.X = x;
			r_AllArtsRow.Y = y;
			r_AllArtsRow.Z = z;
			r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_AllArtsRow.Object = r_ChestRow.Object;
			GetArt(r_AllArtsRow, (decmp[s + 6] >> 3) + ((decmp[s + 7] & 0xF) << 5));
			r_ChestRow.Art = r_AllArtsRow.Artefact;
			r_ChestRow.Class = r_AllArtsRow.Class;
			if (!r_AllArtsRow.Is_Relic_CNull())
			{
				r_ChestRow._Relic_C = r_AllArtsRow._Relic_C;
			}
			r_ChestRow.Gold = 1000;
			dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		}
		dsResult.R_Chest.Rows.Add(r_ChestRow);
	}

	private void SaveSkeleton(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
		if (spt)
		{
			r_ArtRow.Address = s;
		}
		r_ArtRow.X = x;
		r_ArtRow.Y = y;
		r_ArtRow.Z = z;
		r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 8] == byte.MaxValue)
		{
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			r_AllArtsRow.X = x;
			r_AllArtsRow.Y = y;
			r_AllArtsRow.Z = z;
			r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_AllArtsRow.Object = r_ArtRow.Object;
			DataRow dataRow = TblArt.Rows.Find((decmp[s + 7] << 2) + ((decmp[s + 6] & 0xC0) >> 6));
			string artefact = (r_ArtRow.Name = (string)dataRow[1]);
			r_AllArtsRow.Artefact = artefact;
			string @class = (r_ArtRow.Class = (string)dataRow[2]);
			r_AllArtsRow.Class = @class;
			if (dataRow[3] != DBNull.Value)
			{
				string relic_C = (r_ArtRow._Relic_C = (string)dataRow[3]);
				r_AllArtsRow._Relic_C = relic_C;
			}
			dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		}
		dsResult.R_Art.Rows.Add(r_ArtRow);
	}

	private void SaveWagon(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
		if (spt)
		{
			r_ArtRow.Address = s;
		}
		r_ArtRow.X = x;
		r_ArtRow.Y = y;
		r_ArtRow.Z = z;
		r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 7] == 160 && decmp[s + 8] == byte.MaxValue)
		{
			r_ArtRow.Resource = aResource[(decmp[s + 9] & 0xF) >> 1] + decmp[s + 6];
		}
		else if (decmp[s + 7] != 128 && decmp[s + 8] != byte.MaxValue)
		{
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			r_AllArtsRow.X = x;
			r_AllArtsRow.Y = y;
			r_AllArtsRow.Z = z;
			r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_AllArtsRow.Object = r_ArtRow.Object;
			DataRow dataRow = TblArt.Rows.Find((decmp[s + 8] << 1) + (decmp[s + 7] >> 7));
			string artefact = (r_ArtRow.Name = (string)dataRow[1]);
			r_AllArtsRow.Artefact = artefact;
			string @class = (r_ArtRow.Class = (string)dataRow[2]);
			r_AllArtsRow.Class = @class;
			if (dataRow[3] != DBNull.Value)
			{
				string relic_C = (r_ArtRow._Relic_C = (string)dataRow[3]);
				r_AllArtsRow._Relic_C = relic_C;
			}
			dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		}
		dsResult.R_Art.Rows.Add(r_ArtRow);
	}

	private void SaveWitchHut(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_SkillRow r_SkillRow = dsResult.R_Skill.NewR_SkillRow();
		r_SkillRow.X = x;
		r_SkillRow.Y = y;
		r_SkillRow.Z = z;
		r_SkillRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_SkillRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		int num = ((decmp[s + 8] & 0xF) << 3) + (decmp[s + 7] >> 5);
		if (num < 28)
		{
			r_SkillRow.Skill = (string)TblSecondarySkill.Rows[num][1];
		}
		dsResult.R_Skill.Rows.Add(r_SkillRow);
	}

	private void SaveRes(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ResourceRow r_ResourceRow = dsResult.R_Resource.NewR_ResourceRow();
		r_ResourceRow.X = x;
		r_ResourceRow.Y = y;
		r_ResourceRow.Z = z;
		r_ResourceRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_ResourceRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		if (decmp[s + 2] < 6)
		{
			r_ResourceRow.Resource = aResource[decmp[s + 2]] + (((decmp[s + 8] & 1) << 16) + (decmp[s + 7] << 8) + decmp[s + 6]);
		}
		else if (decmp[s + 2] == 6)
		{
			r_ResourceRow.Gold = (((decmp[s + 8] & 1) << 16) + (decmp[s + 7] << 8) + decmp[s + 6]) * 100;
		}
		dsResult.R_Resource.Rows.Add(r_ResourceRow);
		if ((decmp[s + 9] & 0x80) == 128)
		{
			DataRow dataRow = TblArtRes.NewRow();
			dataRow[0] = x;
			dataRow[1] = y;
			dataRow[2] = z;
			dataRow["Num"] = ((decmp[s + 9] & 0x7F) << 8) + decmp[s + 8] >> 3;
			dataRow["Type"] = 1;
			TblArtRes.Rows.Add(dataRow);
		}
	}

	private void SaveScroll(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_SpellRow r_SpellRow = dsResult.R_Spell.NewR_SpellRow();
		r_SpellRow.X = x;
		r_SpellRow.Y = y;
		r_SpellRow.Z = z;
		r_SpellRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_SpellRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		DataRow dataRow = TblSpell.Rows[decmp[s + 6]];
		r_SpellRow.Spell = (string)dataRow[1];
		r_SpellRow.Level = (int)dataRow[2];
		dsResult.R_Spell.Rows.Add(r_SpellRow);
		if ((decmp[s + 9] & 0x80) == 128)
		{
			DataRow dataRow2 = TblArtRes.NewRow();
			dataRow2[0] = x;
			dataRow2[1] = y;
			dataRow2[2] = z;
			dataRow2["Num"] = ((decmp[s + 9] & 0x7F) << 8) + decmp[s + 8] >> 3;
			dataRow2["Type"] = 2;
			TblArtRes.Rows.Add(dataRow2);
		}
	}

	private void SavePyramid(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_SpellRow r_SpellRow = dsResult.R_Spell.NewR_SpellRow();
		if (spt)
		{
			r_SpellRow.Address = s;
		}
		r_SpellRow.X = x;
		r_SpellRow.Y = y;
		r_SpellRow.Z = z;
		r_SpellRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_SpellRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		DataRow dataRow = TblSpell.Rows[((decmp[s + 8] & 0xF) << 3) + (decmp[s + 7] >> 5)];
		r_SpellRow.Spell = (string)dataRow[1];
		r_SpellRow.Level = (int)dataRow[2];
		r_SpellRow.Guard = string.Concat(TblMonsters.Rows.Find(116)[1], " 40, ", TblMonsters.Rows.Find(117)[1], " 20");
		r_SpellRow.HP = 3200;
		dsResult.R_Spell.Rows.Add(r_SpellRow);
	}

	private void SaveCamp(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_CampRow r_CampRow = dsResult.R_Camp.NewR_CampRow();
		DataRow dataRow = TblMonsters.Rows.Find(decmp[s + 2]);
		if (spt)
		{
			r_CampRow.Address = s;
		}
		r_CampRow.X = x;
		r_CampRow.Y = y;
		r_CampRow.Z = z;
		r_CampRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		r_CampRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		r_CampRow.Monster = (string)dataRow[1];
		r_CampRow.Level = (int)dataRow[2];
		r_CampRow.Number = decmp[s + 6];
		dsResult.R_Camp.Rows.Add(r_CampRow);
	}

	private void SaveShrine(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_SpellRow r_SpellRow = dsResult.R_Spell.NewR_SpellRow();
		r_SpellRow.X = x;
		r_SpellRow.Y = y;
		r_SpellRow.Z = z;
		r_SpellRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		DataRow dataRow = TblSpell.Rows[((decmp[s + 8] & 0xF) << 3) + (decmp[s + 7] >> 5)];
		r_SpellRow.Spell = (string)dataRow[1];
		r_SpellRow.Level = (int)dataRow[2];
		r_SpellRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
		dsResult.R_Spell.Rows.Add(r_SpellRow);
	}

	private void SaveMonster(int x, int y, int z, int s, int loc)
	{
		int num = (decmp[s + 7] & 0xF0) >> 4;
		DataSet2.R_MonstrRow r_MonstrRow = dsResult.R_Monstr.NewR_MonstrRow();
		if (spt)
		{
			r_MonstrRow.Address = s;
		}
		r_MonstrRow.X = x;
		r_MonstrRow.Y = y;
		r_MonstrRow.Z = z;
		r_MonstrRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
		GetMonster(r_MonstrRow, decmp[s + 2]);
		r_MonstrRow.Mood = ((num == 12) ? (-4) : num);
		r_MonstrRow.Number = ((decmp[s + 7] & 0xF) << 8) + decmp[s + 6];
		r_MonstrRow.HP *= r_MonstrRow.Number;
		r_MonstrRow.Increase = (((decmp[s + 8] & 4) > 0) ? aReply[0] : aReply[1]);
		dsResult.R_Monstr.Rows.Add(r_MonstrRow);
		if ((decmp[s + 9] & 0x80) == 128)
		{
			DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
			DataRow dataRow = TblMonstr.NewRow();
			int num3 = (r_AllArtsRow.X = x);
			dataRow[0] = num3;
			int num5 = (r_AllArtsRow.Y = y);
			dataRow[1] = num5;
			int num7 = (r_AllArtsRow.Z = z);
			dataRow[2] = num7;
			dataRow["Num"] = ((decmp[s + 9] & 7) << 8) + decmp[s + 8] >> 3;
			r_AllArtsRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]);
			r_AllArtsRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString();
			r_AllArtsRow.Monster = r_MonstrRow.Name + " " + r_MonstrRow.Number;
			dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
			TblMonstr.Rows.Add(dataRow);
		}
	}

	private void SaveTomb(int x, int y, int z, int s, int loc)
	{
		DataSet2.R_ArtRow r_ArtRow = dsResult.R_Art.NewR_ArtRow();
		DataSet2.R_AllArtsRow r_AllArtsRow = dsResult.R_AllArts.NewR_AllArtsRow();
		if (spt)
		{
			r_ArtRow.Address = s;
		}
		int x2 = (r_ArtRow.X = x);
		r_AllArtsRow.X = x2;
		int y2 = (r_ArtRow.Y = y);
		r_AllArtsRow.Y = y2;
		int z2 = (r_ArtRow.Z = z);
		r_AllArtsRow.Z = z2;
		string locality = (r_ArtRow.Locality = ((loc == 8) ? aLocality[1] : aLocality[0]));
		r_AllArtsRow.Locality = locality;
		if (decmp[s + 6] == 31)
		{
			DataRow dataRow = TblArt.Rows.Find(((decmp[s + 8] & 0x7F) << 3) + (decmp[s + 7] >> 5));
			string artefact = (r_ArtRow.Name = (string)dataRow[1]);
			r_AllArtsRow.Artefact = artefact;
			string @class = (r_ArtRow.Class = (string)dataRow[2]);
			r_AllArtsRow.Class = @class;
			if (dataRow[3] != DBNull.Value)
			{
				string relic_C = (r_ArtRow._Relic_C = (string)dataRow[3]);
				r_AllArtsRow._Relic_C = relic_C;
			}
		}
		string @object = (r_ArtRow.Object = TblObject.Select("Code='" + decmp[s] + "'")[0][1].ToString());
		r_AllArtsRow.Object = @object;
		dsResult.R_AllArts.Rows.Add(r_AllArtsRow);
		dsResult.R_Art.Rows.Add(r_ArtRow);
	}

	private void GetArt(DataSet2.R_AllArtsRow row, int code)
	{
		DataRow dataRow = TblArt.Rows.Find(code);
		row.Artefact = (string)dataRow[1];
		row.Class = (string)dataRow[2];
		if (dataRow[3] != DBNull.Value)
		{
			row._Relic_C = (string)dataRow[3];
		}
	}

	private void GetMonster(DataSet2.R_MonstrRow row, int code)
	{
		DataRow dataRow = TblMonsters.Rows.Find(code);
		row.Name = (string)dataRow[1];
		row.Level = (int)dataRow[2];
		row.HP = (int)dataRow[3];
	}

	private void CreateTblMarket()
	{
		TblMarket.Columns.Add("X", Type.GetType("System.Int32"));
		TblMarket.Columns.Add("Y", Type.GetType("System.Int32"));
		TblMarket.Columns.Add("Z", Type.GetType("System.Int32"));
		TblMarket.Columns.Add("Code", Type.GetType("System.Int32"));
	}

	private void CreateTblUniver()
	{
		TblUniver.Columns.Add("X", Type.GetType("System.Int32"));
		TblUniver.Columns.Add("Y", Type.GetType("System.Int32"));
		TblUniver.Columns.Add("Z", Type.GetType("System.Int32"));
		TblUniver.Columns.Add("Code", Type.GetType("System.Int32"));
	}

	private void CreateTblBanks()
	{
		TblBanks.Columns.Add("X", Type.GetType("System.Int32"));
		TblBanks.Columns.Add("Y", Type.GetType("System.Int32"));
		TblBanks.Columns.Add("Z", Type.GetType("System.Int32"));
		TblBanks.Columns.Add("Code", Type.GetType("System.Int32"));
		TblBanks.Columns.Add("Art", Type.GetType("System.Boolean"));
		TblBanks.Columns.Add("Num", Type.GetType("System.Int32"));
	}

	private void CreateTblGarrison()
	{
		TblGarrison.Columns.Add("X", Type.GetType("System.Int32"));
		TblGarrison.Columns.Add("Y", Type.GetType("System.Int32"));
		TblGarrison.Columns.Add("Z", Type.GetType("System.Int32"));
		TblGarrison.Columns.Add("Code", Type.GetType("System.Int32"));
	}

	private void CreateTblEventBox()
	{
		TblEventBox.Columns.Add("X", Type.GetType("System.Int32"));
		TblEventBox.Columns.Add("Y", Type.GetType("System.Int32"));
		TblEventBox.Columns.Add("Z", Type.GetType("System.Int32"));
		TblEventBox.Columns.Add("Num", Type.GetType("System.Int32"));
	}

	private void CreateTblMonstr()
	{
		TblMonstr.Columns.Add("X", Type.GetType("System.Int32"));
		TblMonstr.Columns.Add("Y", Type.GetType("System.Int32"));
		TblMonstr.Columns.Add("Z", Type.GetType("System.Int32"));
		TblMonstr.Columns.Add("Num", Type.GetType("System.Int32"));
	}

	private void CreateTblSeerHut()
	{
		TblSeerHut.Columns.Add("X", Type.GetType("System.Int32"));
		TblSeerHut.Columns.Add("Y", Type.GetType("System.Int32"));
		TblSeerHut.Columns.Add("Z", Type.GetType("System.Int32"));
		TblSeerHut.Columns.Add("Num", Type.GetType("System.Int32"));
	}

	private void CreateTblPassGuard()
	{
		TblPassGuard.Columns.Add("X", Type.GetType("System.Int32"));
		TblPassGuard.Columns.Add("Y", Type.GetType("System.Int32"));
		TblPassGuard.Columns.Add("Z", Type.GetType("System.Int32"));
		TblPassGuard.Columns.Add("Num", Type.GetType("System.Int32"));
	}

	private void CreateTblArtRes()
	{
		TblArtRes.Columns.Add("X", Type.GetType("System.Int32"));
		TblArtRes.Columns.Add("Y", Type.GetType("System.Int32"));
		TblArtRes.Columns.Add("Z", Type.GetType("System.Int32"));
		TblArtRes.Columns.Add("Num", Type.GetType("System.Int32"));
		TblArtRes.Columns.Add("Type", Type.GetType("System.Int32"));
	}

	private void CreateTblDwelling()
	{
		TblDwelling.Columns.Add("Code", Type.GetType("System.Int32"));
		TblDwelling.Columns.Add("Name", Type.GetType("System.String"));
		TblDwelling.Columns.Add("Monster", Type.GetType("System.Int32"));
		TblDwelling.PrimaryKey = new DataColumn[1] { TblDwelling.Columns["Code"] };
		TblDwelling.Rows.Add(1, "Утес чудищ", 96);
		TblDwelling.Rows.Add(3, "Залы тьмы", 66);
		TblDwelling.Rows.Add(4, "Свод драконов", 68);
		TblDwelling.Rows.Add(5, "Тренировочная площадка", 10);
		TblDwelling.Rows.Add(8, "Портал Славы", 12);
		TblDwelling.Rows.Add(9, "Пещера циклопов", 94);
		TblDwelling.Rows.Add(10, "Покинутый дворец", 54);
		TblDwelling.Rows.Add(13, "Разлом элементалей земли", 113);
		TblDwelling.Rows.Add(14, "Огненное озеро", 52);
		TblDwelling.Rows.Add(18, "Алтарь желаний", 36);
		TblDwelling.Rows.Add(23, "Логово горгон", 102);
		TblDwelling.Rows.Add(24, "Утес драконов", 26);
		TblDwelling.Rows.Add(28, "Пруд гидр", 110);
		TblDwelling.Rows.Add(32, "Берлога мантикор", 80);
		TblDwelling.Rows.Add(34, "Лабиринт", 78);
		TblDwelling.Rows.Add(35, "Монастырь", 8);
		TblDwelling.Rows.Add(36, "Золотой павильон", 38);
		TblDwelling.Rows.Add(40, "Адская дыра", 50);
		TblDwelling.Rows.Add(41, "Пещера драконов", 82);
		TblDwelling.Rows.Add(42, "Гнездо на утесе", 92);
		TblDwelling.Rows.Add(44, "Заоблачный храм", 40);
		TblDwelling.Rows.Add(45, "Арка дендроидов", 22);
		TblDwelling.Rows.Add(49, "Гнездо вивернов", 108);
		TblDwelling.Rows.Add(51, "Опушка единорогов", 24);
		TblDwelling.Rows.Add(52, "Мавзолей", 64);
		TblDwelling.Rows.Add(60, "Алтарь мыслей", 120);
		TblDwelling.Rows.Add(61, "Погребальный костер", 130);
		TblDwelling.Rows.Add(62, "Ледяной утес", 132);
		TblDwelling.Rows.Add(63, "Кристаллическая пещера", 133);
		TblDwelling.Rows.Add(64, "Магический лес", 134);
		TblDwelling.Rows.Add(65, "Серная берлога", 135);
		TblDwelling.Rows.Add(66, "Ложбина чародеев", 136);
		TblDwelling.Rows.Add(68, "Опушка единорогов", 24);
		TblDwelling.Rows.Add(70, "Алтарь земли", 113);
		TblDwelling.Rows.Add(79, "Мост троллей", 144);
		TblDwelling.Rows.Add(96, "Фабрика големов", 116);
		TblDwelling.Rows.Add(97, "Сопряжение", 113);
	}

	private void CreateTblObject()
	{
		TblObject.Columns.Add("Code", Type.GetType("System.Int32"));
		TblObject.Columns.Add("Name", Type.GetType("System.String"));
		TblObject.Columns.Add("Type", Type.GetType("System.Int32"));
		TblObject.Rows.Add(2, "Жертвенный алтарь");
		TblObject.Rows.Add(5, "Артефакт");
		TblObject.Rows.Add(6, "Ящик Пандоры");
		TblObject.Rows.Add(7, "Черный рынок");
		TblObject.Rows.Add(10, "Палатка ключника");
		TblObject.Rows.Add(12, "Кострище покинутого лагеря");
		TblObject.Rows.Add(16, "Склады циклопов", 0);
		TblObject.Rows.Add(16, "Сокровищница гномов", 1);
		TblObject.Rows.Add(16, "Консерватория грифонов", 2);
		TblObject.Rows.Add(16, "Тайник бесов", 3);
		TblObject.Rows.Add(16, "Хранилище медуз", 4);
		TblObject.Rows.Add(16, "Банк Наг", 5);
		TblObject.Rows.Add(16, "Улей змиев", 6);
		TblObject.Rows.Add(22, "Труп");
		TblObject.Rows.Add(24, "Ветхий корабль");
		TblObject.Rows.Add(25, "Утопия драконов");
		TblObject.Rows.Add(26, "Событие");
		TblObject.Rows.Add(29, "Обломки");
		TblObject.Rows.Add(33, "Гарнизон");
		TblObject.Rows.Add(34, "Герой");
		TblObject.Rows.Add(35, "Форт на холме");
		TblObject.Rows.Add(39, "Чей-то погреб");
		TblObject.Rows.Add(43, "Монолит входа");
		TblObject.Rows.Add(44, "Монолит выхода");
		TblObject.Rows.Add(45, "Двухсторонний монолит");
		TblObject.Rows.Add(53, "Заброшенная шахта");
		TblObject.Rows.Add(54, "Монстр");
		TblObject.Rows.Add(55, "Мистический сад");
		TblObject.Rows.Add(62, "Тюрьма");
		TblObject.Rows.Add(63, "Пирамида");
		TblObject.Rows.Add(78, "Лагерь беженцев");
		TblObject.Rows.Add(79, "Ресурс");
		TblObject.Rows.Add(81, "Ученый");
		TblObject.Rows.Add(82, "Морской сундук");
		TblObject.Rows.Add(83, "Хижина провидца");
		TblObject.Rows.Add(84, "Склеп");
		TblObject.Rows.Add(85, "Кораблекрушение");
		TblObject.Rows.Add(86, "Потерпевший кораблекрушение");
		TblObject.Rows.Add(88, "Святыня магического воплощения");
		TblObject.Rows.Add(89, "Святыня магического жеста");
		TblObject.Rows.Add(90, "Святыня магической мысли");
		TblObject.Rows.Add(93, "Свиток с заклинанием");
		TblObject.Rows.Add(95, "Таверна");
		TblObject.Rows.Add(98, "Городок");
		TblObject.Rows.Add(100, "Камень знаний");
		TblObject.Rows.Add(101, "Сундук с сокровищами");
		TblObject.Rows.Add(102, "Древо знаний");
		TblObject.Rows.Add(103, "Врата подземного мира");
		TblObject.Rows.Add(104, "Университет");
		TblObject.Rows.Add(105, "Телега");
		TblObject.Rows.Add(108, "Могила воина");
		TblObject.Rows.Add(111, "Водоворот");
		TblObject.Rows.Add(112, "Ветряная мельница");
		TblObject.Rows.Add(113, "Хижина ведьмы");
		TblObject.Rows.Add(213, "Гильдия наемников");
		TblObject.Rows.Add(215, "Страж прохода");
		TblObject.Rows.Add(255, "Торговцы Артефактами");
	}

	private void CreateTblSpell()
	{
		TblSpell.Columns.Add("Code", Type.GetType("System.Int32"));
		TblSpell.Columns.Add("Name", Type.GetType("System.String"));
		TblSpell.Columns.Add("Level", Type.GetType("System.Int32"));
		TblSpell.Rows.Add(0, "Вызвать корабль", 1);
		TblSpell.Rows.Add(1, "Затопить корабль", 2);
		TblSpell.Rows.Add(2, "Видения", 2);
		TblSpell.Rows.Add(3, "Просмотр земли", 1);
		TblSpell.Rows.Add(4, "Маскировка", 2);
		TblSpell.Rows.Add(5, "Просмотр воздуха", 1);
		TblSpell.Rows.Add(6, "Полет", 5);
		TblSpell.Rows.Add(7, "Хождение по воде", 4);
		TblSpell.Rows.Add(8, "Дверь измерений", 5);
		TblSpell.Rows.Add(9, "Городской портал", 4);
		TblSpell.Rows.Add(10, "Зыбучие пески", 2);
		TblSpell.Rows.Add(11, "Минное поле", 3);
		TblSpell.Rows.Add(12, "Силовое поле", 3);
		TblSpell.Rows.Add(13, "Стена огня", 2);
		TblSpell.Rows.Add(14, "Землетрясение", 3);
		TblSpell.Rows.Add(15, "Волшебная стрела", 1);
		TblSpell.Rows.Add(16, "Ледяная молния", 2);
		TblSpell.Rows.Add(17, "Удар молнии", 2);
		TblSpell.Rows.Add(18, "Взрыв", 5);
		TblSpell.Rows.Add(19, "Цепная молния", 4);
		TblSpell.Rows.Add(20, "Кольцо холода", 3);
		TblSpell.Rows.Add(21, "Огненный шар", 3);
		TblSpell.Rows.Add(22, "Инферно", 4);
		TblSpell.Rows.Add(23, "Метеоритный дождь", 4);
		TblSpell.Rows.Add(24, "Волна смерти", 2);
		TblSpell.Rows.Add(25, "Уничтожить нечисть", 3);
		TblSpell.Rows.Add(26, "Армагеддон", 4);
		TblSpell.Rows.Add(27, "Щит", 1);
		TblSpell.Rows.Add(28, "Воздушный щит", 3);
		TblSpell.Rows.Add(29, "Огненный щит", 4);
		TblSpell.Rows.Add(30, "Защита от воздуха", 2);
		TblSpell.Rows.Add(31, "Защита от огня", 1);
		TblSpell.Rows.Add(32, "Защита от воды", 1);
		TblSpell.Rows.Add(33, "Защита от земли", 3);
		TblSpell.Rows.Add(34, "Анти-Магия", 3);
		TblSpell.Rows.Add(35, "Снятие заклинания", 1);
		TblSpell.Rows.Add(36, "Волшебное зеркало", 5);
		TblSpell.Rows.Add(37, "Лечение", 1);
		TblSpell.Rows.Add(38, "Восстановление", 4);
		TblSpell.Rows.Add(39, "Оживление мертвецов", 3);
		TblSpell.Rows.Add(40, "Жертва", 5);
		TblSpell.Rows.Add(41, "Благословление", 1);
		TblSpell.Rows.Add(42, "Проклятье", 1);
		TblSpell.Rows.Add(43, "Жажда крови", 1);
		TblSpell.Rows.Add(44, "Точность", 2);
		TblSpell.Rows.Add(45, "Слабость", 2);
		TblSpell.Rows.Add(46, "Каменная кожа", 1);
		TblSpell.Rows.Add(47, "Разрушающий луч", 2);
		TblSpell.Rows.Add(48, "Молитва", 4);
		TblSpell.Rows.Add(49, "Радость", 3);
		TblSpell.Rows.Add(50, "Печаль", 4);
		TblSpell.Rows.Add(51, "Удача", 2);
		TblSpell.Rows.Add(52, "Неудача", 3);
		TblSpell.Rows.Add(53, "Ускорение", 1);
		TblSpell.Rows.Add(54, "Медлительность", 1);
		TblSpell.Rows.Add(55, "Палач", 4);
		TblSpell.Rows.Add(56, "Бешенство", 4);
		TblSpell.Rows.Add(57, "Гром титанов");
		TblSpell.Rows.Add(58, "Контрудар", 4);
		TblSpell.Rows.Add(59, "Берсерк", 4);
		TblSpell.Rows.Add(60, "Гипноз", 3);
		TblSpell.Rows.Add(61, "Забывчивость", 3);
		TblSpell.Rows.Add(62, "Слепота", 2);
		TblSpell.Rows.Add(63, "Телепорт", 3);
		TblSpell.Rows.Add(64, "Устранение преград", 2);
		TblSpell.Rows.Add(65, "Клон", 4);
		TblSpell.Rows.Add(66, "Огненный Элементаль", 5);
		TblSpell.Rows.Add(67, "Земляной Элементаль", 5);
		TblSpell.Rows.Add(68, "Водный Элементаль", 5);
		TblSpell.Rows.Add(69, "Воздушный Элементаль", 5);
		TblSpell.Rows.Add(70, "Взгляд, превращяющий в камень", 0);
	}

	private void CreateTblSecondarySkill()
	{
		TblSecondarySkill.Columns.Add("Code", Type.GetType("System.Int32"));
		TblSecondarySkill.Columns.Add("Name", Type.GetType("System.String"));
		TblSecondarySkill.Rows.Add(0, "Поиск пути");
		TblSecondarySkill.Rows.Add(1, "Меткость");
		TblSecondarySkill.Rows.Add(2, "Логистика");
		TblSecondarySkill.Rows.Add(3, "Разведка");
		TblSecondarySkill.Rows.Add(4, "Дипломатия");
		TblSecondarySkill.Rows.Add(5, "Навигация");
		TblSecondarySkill.Rows.Add(6, "Лидерство");
		TblSecondarySkill.Rows.Add(7, "Мудрость");
		TblSecondarySkill.Rows.Add(8, "Мистицизм");
		TblSecondarySkill.Rows.Add(9, "Удача");
		TblSecondarySkill.Rows.Add(10, "Баллистика");
		TblSecondarySkill.Rows.Add(11, "Орлиный глаз");
		TblSecondarySkill.Rows.Add(12, "Чародейство");
		TblSecondarySkill.Rows.Add(13, "Поместья");
		TblSecondarySkill.Rows.Add(14, "Огонь");
		TblSecondarySkill.Rows.Add(15, "Воздух");
		TblSecondarySkill.Rows.Add(16, "Вода");
		TblSecondarySkill.Rows.Add(17, "Земля");
		TblSecondarySkill.Rows.Add(18, "Грамотность");
		TblSecondarySkill.Rows.Add(19, "Тактика");
		TblSecondarySkill.Rows.Add(20, "Артиллерия");
		TblSecondarySkill.Rows.Add(21, "Обучаемость");
		TblSecondarySkill.Rows.Add(22, "Нападение");
		TblSecondarySkill.Rows.Add(23, "Защита");
		TblSecondarySkill.Rows.Add(24, "Интеллект");
		TblSecondarySkill.Rows.Add(25, "Волшебство");
		TblSecondarySkill.Rows.Add(26, "Сопротивление");
		TblSecondarySkill.Rows.Add(27, "Первая помощь");
	}

	private void CreateTblArt()
	{
		TblArt.Columns.Add("Code", Type.GetType("System.Int32"));
		TblArt.Columns.Add("Name", Type.GetType("System.String"));
		TblArt.Columns.Add("Class", Type.GetType("System.String"));
		TblArt.Columns.Add("Relic-C", Type.GetType("System.String"));
		TblArt.Columns.Add("Cost", Type.GetType("System.Int32"));
		TblArt.Columns.Add("Num", Type.GetType("System.Int32"));
		TblArt.PrimaryKey = new DataColumn[1] { TblArt.Columns["Code"] };
		TblArt.Rows.Add(0, "Книга заклинаний", "", DBNull.Value, 500);
		TblArt.Rows.Add(1, "Свиток с заклинанием", "", DBNull.Value, 0);
		TblArt.Rows.Add(2, "Грааль", "", DBNull.Value, 0);
		TblArt.Rows.Add(3, "Катапульта", "0", DBNull.Value, 0);
		TblArt.Rows.Add(4, "Баллиста", "0", DBNull.Value, 2500);
		TblArt.Rows.Add(5, "Подвода с боеприпасами", "0", DBNull.Value, 1000);
		TblArt.Rows.Add(6, "Палатка первой помощи", "0", DBNull.Value, 750);
		TblArt.Rows.Add(7, "Секира Кентавра", "1", DBNull.Value, 5000);
		TblArt.Rows.Add(8, "Черный клинок мертвого рыцаря", "2", "132", 7500);
		TblArt.Rows.Add(9, "Великий Гноллий кистень", "2", DBNull.Value, 10000);
		TblArt.Rows.Add(10, "Карающая дубина Огра", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(11, "Адский меч", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(12, "Гладиус Титана", "4", "135", 25000);
		TblArt.Rows.Add(13, "Щит гномьих Героев", "1", DBNull.Value, 5000);
		TblArt.Rows.Add(14, "Щит тоскующих мертвецов", "2", "132", 7500);
		TblArt.Rows.Add(15, "Щит короля Гноллов", "2", DBNull.Value, 10000);
		TblArt.Rows.Add(16, "Щит яростного Огра", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(17, "Щит проклятых", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(18, "Щит стража", "4", "135", 25000);
		TblArt.Rows.Add(19, "Шлем белого Единорога", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(20, "Шлем-череп", "1", "132", 7500);
		TblArt.Rows.Add(21, "Шлем хаоса", "2", DBNull.Value, 10000);
		TblArt.Rows.Add(22, "Корона верховного волхва", "2", DBNull.Value, 12500);
		TblArt.Rows.Add(23, "Шлем сатанинской ярости", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(24, "Шлем небесного грома", "4", "135", 25000);
		TblArt.Rows.Add(25, "Нагрудник из окаменелого дерева", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(26, "Доспехи из ребер", "2", "132", 7500);
		TblArt.Rows.Add(27, "Кольчуга Великого Василиска", "2", DBNull.Value, 10000);
		TblArt.Rows.Add(28, "Туника Короля Циклопов", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(29, "Нагрудник из серного камня", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(30, "Латы Титана", "4", "135", 25000);
		TblArt.Rows.Add(31, "Магические доспехи", "2", "129", 10000);
		TblArt.Rows.Add(32, "Сандалии святых", "4", "129", 20000);
		TblArt.Rows.Add(33, "Ожерелье божественной благодати", "4", "129", 30000);
		TblArt.Rows.Add(34, "Щит львиной храбрости", "4", "129", 40000);
		TblArt.Rows.Add(35, "Меч правосудия", "4", "129", 50000);
		TblArt.Rows.Add(36, "Шлем божественного просвещения", "4", "129", 60000);
		TblArt.Rows.Add(37, "Неподвижный глаз Дракона", "1", "134", 5000);
		TblArt.Rows.Add(38, "Языки пламени Красного Дракона", "2", "134", 10000);
		TblArt.Rows.Add(39, "Щит из чешуи Дракона", "3", "134", 15000);
		TblArt.Rows.Add(40, "Доспехи из чешуи Дракона", "4", "134", 20000);
		TblArt.Rows.Add(41, "Наколенники из Драконьей кости", "1", "134", 5000);
		TblArt.Rows.Add(42, "Плащ из Драконьих крыльев", "2", "134", 10000);
		TblArt.Rows.Add(43, "Ожерелье из зубов Дракона", "3", "134", 15000);
		TblArt.Rows.Add(44, "Корона из зубов Дракона", "4", "134", 20000);
		TblArt.Rows.Add(45, "Застывший глаз Дракона", "1", "134", 5000);
		TblArt.Rows.Add(46, "Клевер удачи", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(47, "Карты пророчества", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(48, "Голубка удачи", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(49, "Значок смелости", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(50, "Герб доблести", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(51, "Знак отваги", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(52, "Телескоп", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(53, "Подзорная труба", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(54, "Амулет гробовщика", "1", "130", 5000);
		TblArt.Rows.Add(55, "Мантия Вампира", "2", "130", 10000);
		TblArt.Rows.Add(56, "Сапоги Мертвеца", "3", "130", 15000);
		TblArt.Rows.Add(57, "Колье неприступности", "3", DBNull.Value, 5000);
		TblArt.Rows.Add(58, "Мантия равновесия", "3", DBNull.Value, 10000);
		TblArt.Rows.Add(59, "Сапоги противодействия", "4", DBNull.Value, 15000);
		TblArt.Rows.Add(60, "Эльфийский лук из вишневого дерева", "1", "137", 5000);
		TblArt.Rows.Add(61, "Тетива из волос гривы Единорога", "2", "137", 10000);
		TblArt.Rows.Add(62, "Стрелы из Ангельских перьев", "3", "137", 15000);
		TblArt.Rows.Add(63, "Птица познания", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(64, "Бесстрашный хранитель", "1", DBNull.Value, 5000);
		TblArt.Rows.Add(65, "Символ знаний", "2", DBNull.Value, 7500);
		TblArt.Rows.Add(66, "Медаль дипломата", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(67, "Кольцо дипломата", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(68, "Лента посла", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(69, "Кольцо странника", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(70, "Перчатки всадника", "2", DBNull.Value, 7500);
		TblArt.Rows.Add(71, "Ожерелье морского проведения", "3", "136", 25000);
		TblArt.Rows.Add(72, "Крылья ангела", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(73, "Амулет маны", "1", "138", 1250);
		TblArt.Rows.Add(74, "Талисман маны", "1", "138", 2500);
		TblArt.Rows.Add(75, "Магическая медаль маны", "1", "138", 3750);
		TblArt.Rows.Add(76, "Магическое ожерелье", "1", "139", 1250);
		TblArt.Rows.Add(77, "Магическое кольцо", "1", "139", 2500);
		TblArt.Rows.Add(78, "Магическая накидка", "1", "139", 3750);
		TblArt.Rows.Add(79, "Сфера небесного свода", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(80, "Сфера илистого озера", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(81, "Сфера бушующего огня", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(82, "Сфера проливного дождя", "3", DBNull.Value, 15000);
		TblArt.Rows.Add(83, "Плащ отречения", "3", DBNull.Value, 20000);
		TblArt.Rows.Add(84, "Дух уныния", "1", DBNull.Value, 5000);
		TblArt.Rows.Add(85, "Песочные часы недоброго часа", "1", DBNull.Value, 5000);
		TblArt.Rows.Add(86, "Фолиант магии огня", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(87, "Фолиант магии воздуха", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(88, "Фолиант магии воды", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(89, "Фолиант магии земли", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(90, "Сапоги левитации", "4", DBNull.Value, 25000);
		TblArt.Rows.Add(91, "Золотой лук", "3", DBNull.Value, 20000);
		TblArt.Rows.Add(92, "Шар постоянства", "3", DBNull.Value, 18750);
		TblArt.Rows.Add(93, "Медаль уязвимости", "4", DBNull.Value, 62500);
		TblArt.Rows.Add(94, "Кольцо жизненной силы", "1", "131", 12500);
		TblArt.Rows.Add(95, "Кольцо жизни", "2", "131", 12500);
		TblArt.Rows.Add(96, "Склянка жизненой силы", "3", "131", 25000);
		TblArt.Rows.Add(97, "Ожерелье стремительности", "1", DBNull.Value, 12500);
		TblArt.Rows.Add(98, "Сапоги-скороходы", "2", DBNull.Value, 15000);
		TblArt.Rows.Add(99, "Накидка скорости", "3", DBNull.Value, 25000);
		TblArt.Rows.Add(100, "Брелок бесстрастия", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(101, "Брелок ясновидения", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(102, "Священный брелок", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(103, "Брелок жизни", "1", DBNull.Value, 6250);
		TblArt.Rows.Add(104, "Брелок смерти", "1", DBNull.Value, 6250);
		TblArt.Rows.Add(105, "Брелок свободы", "1", DBNull.Value, 2500);
		TblArt.Rows.Add(106, "Брелок отрицательности", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(107, "Брелок абсолютной памяти", "1", DBNull.Value, 7500);
		TblArt.Rows.Add(108, "Брелок смелости", "3", DBNull.Value, 17500);
		TblArt.Rows.Add(109, "Плащ бесконечных кристаллов", "3", "140", 12500);
		TblArt.Rows.Add(110, "Кольцо драгоценных камней", "3", "140", 12500);
		TblArt.Rows.Add(111, "Неиссякаемая склянка ртути", "3", "140", 12500);
		TblArt.Rows.Add(112, "Неистощимая подвода с рудой", "2", DBNull.Value, 12500);
		TblArt.Rows.Add(113, "Вечное кольцо серы", "3", "140", 12500);
		TblArt.Rows.Add(114, "Неистощимая подвода леса", "2", DBNull.Value, 12500);
		TblArt.Rows.Add(115, "Неиссякаемый мешок золота", "4", DBNull.Value, 25000);
		TblArt.Rows.Add(116, "Неиссякаемая сума золота", "3", DBNull.Value, 18750);
		TblArt.Rows.Add(117, "Неиссякаемая мошна золота", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(118, "Ноги легиона", "1", "133", 12500);
		TblArt.Rows.Add(119, "Поясница легиона", "2", "133", 12500);
		TblArt.Rows.Add(120, "Туловище легиона", "2", "133", 12500);
		TblArt.Rows.Add(121, "Руки легиона", "3", "133", 12500);
		TblArt.Rows.Add(122, "Голова легиона", "3", "133", 12500);
		TblArt.Rows.Add(123, "Шляпа морского капитана", "4", "136", 37500);
		TblArt.Rows.Add(124, "Шляпа оратора", "4", DBNull.Value, 75000);
		TblArt.Rows.Add(125, "Оковы войны", "3", DBNull.Value, 12500);
		TblArt.Rows.Add(126, "Сфера запрещения", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(127, "Пузырек с кровью Дракона", "4", DBNull.Value, 50000);
		TblArt.Rows.Add(128, "Клинок армагеддона", "4", DBNull.Value, 125000);
		TblArt.Rows.Add(129, "Альянс Ангелов", "5", DBNull.Value, 210000);
		TblArt.Rows.Add(130, "Плащ короля нежити", "5", DBNull.Value, 30000);
		TblArt.Rows.Add(131, "Эликсир жизни", "5", DBNull.Value, 50000);
		TblArt.Rows.Add(132, "Доспехи проклятого", "5", DBNull.Value, 30000);
		TblArt.Rows.Add(133, "Статуя легиона", "5", DBNull.Value, 62500);
		TblArt.Rows.Add(134, "Мощь отца Драконов", "5", DBNull.Value, 105000);
		TblArt.Rows.Add(135, "Грохот Титана", "5", DBNull.Value, 100000);
		TblArt.Rows.Add(136, "Шляпа адмирала", "5", DBNull.Value, 62500);
		TblArt.Rows.Add(137, "Лук Снайпера", "5", DBNull.Value, 30000);
		TblArt.Rows.Add(138, "Колодец волшебника", "5", DBNull.Value, 7500);
		TblArt.Rows.Add(139, "Кольцо Мага", "5", DBNull.Value, 7500);
		TblArt.Rows.Add(140, "Рог изобилия", "5", DBNull.Value, 50000);
		TblArt.Rows.Add(141, "Волшебная палочка", "4", DBNull.Value, 0);
		TblArt.Rows.Add(142, "Золотая стрела", "4", DBNull.Value, 0);
		TblArt.Rows.Add(143, "Сила монстра", "4", DBNull.Value, 0);
	}

	private void CreateTblMonster()
	{
		TblMonsters.Columns.Add("Code", Type.GetType("System.Int32"));
		TblMonsters.Columns.Add("Name", Type.GetType("System.String"));
		TblMonsters.Columns.Add("Level", Type.GetType("System.Int32"));
		TblMonsters.Columns.Add("HP", Type.GetType("System.Int32"));
		TblMonsters.Columns.Add("Altar", Type.GetType("System.Int32"));
		TblMonsters.Columns.Add("Town", Type.GetType("System.Int32"));
		TblMonsters.PrimaryKey = new DataColumn[1] { TblMonsters.Columns["Code"] };
		TblMonsters.Rows.Add(0, "Копейщик", 1, 10, 10, 0);
		TblMonsters.Rows.Add(1, "Алебардщик", 1, 10, 10, 0);
		TblMonsters.Rows.Add(2, "Арбалетчик", 2, 10, 15, 0);
		TblMonsters.Rows.Add(3, "Тяжелый арбалетчик", 2, 10, 20, 0);
		TblMonsters.Rows.Add(4, "Грифон", 3, 25, 40, 0);
		TblMonsters.Rows.Add(5, "Королевский грифон", 3, 25, 55, 0);
		TblMonsters.Rows.Add(6, "Мечник", 4, 35, 55, 0);
		TblMonsters.Rows.Add(7, "Крестоносец", 4, 35, 70, 0);
		TblMonsters.Rows.Add(8, "Монах", 5, 30, 60, 0);
		TblMonsters.Rows.Add(9, "Фанатик", 5, 30, 90, 0);
		TblMonsters.Rows.Add(10, "Кавалерист", 6, 100, 240, 0);
		TblMonsters.Rows.Add(11, "Чемпион", 6, 100, 260, 0);
		TblMonsters.Rows.Add(12, "Ангел", 7, 200, 625, 0);
		TblMonsters.Rows.Add(13, "Архангел", 7, 250, 1095, 0);
		TblMonsters.Rows.Add(14, "Кентавр", 1, 8, 10, 1);
		TblMonsters.Rows.Add(15, "Капитан кентавров", 1, 10, 15, 1);
		TblMonsters.Rows.Add(16, "Гном", 2, 20, 15, 1);
		TblMonsters.Rows.Add(17, "Боевой гном", 2, 20, 25, 1);
		TblMonsters.Rows.Add(18, "Лесной эльф", 3, 15, 25, 1);
		TblMonsters.Rows.Add(19, "Благородный эльф", 3, 15, 40, 1);
		TblMonsters.Rows.Add(20, "Пегас", 4, 30, 60, 1);
		TblMonsters.Rows.Add(21, "Серебряный пегас", 4, 30, 65, 1);
		TblMonsters.Rows.Add(22, "Дендроид охранник", 5, 55, 60, 1);
		TblMonsters.Rows.Add(23, "Дендроид солдат", 5, 65, 100, 1);
		TblMonsters.Rows.Add(24, "Единорог", 6, 90, 225, 1);
		TblMonsters.Rows.Add(25, "Боевой единорог", 6, 110, 250, 1);
		TblMonsters.Rows.Add(26, "Зеленый дракон", 7, 180, 605, 1);
		TblMonsters.Rows.Add(27, "Золотой дракон", 7, 250, 1075, 1);
		TblMonsters.Rows.Add(28, "Гремлин", 1, 4, 5, 2);
		TblMonsters.Rows.Add(29, "Мастер-гремлин", 1, 4, 5, 2);
		TblMonsters.Rows.Add(30, "Каменная Горгулья", 2, 16, 20, 2);
		TblMonsters.Rows.Add(31, "Обсидиановая горгулья", 2, 16, 25, 2);
		TblMonsters.Rows.Add(32, "Каменный голем", 3, 30, 30, 2);
		TblMonsters.Rows.Add(33, "Стальной голем", 3, 35, 50, 2);
		TblMonsters.Rows.Add(34, "Маг", 4, 25, 70, 2);
		TblMonsters.Rows.Add(35, "Архи-маг", 4, 30, 85, 2);
		TblMonsters.Rows.Add(36, "Джинн", 5, 40, 110, 2);
		TblMonsters.Rows.Add(37, "Мастер-джинн", 5, 40, 115, 2);
		TblMonsters.Rows.Add(38, "Нага", 6, 110, 250, 2);
		TblMonsters.Rows.Add(39, "Королева нага", 6, 110, 355, 2);
		TblMonsters.Rows.Add(40, "Гигант", 7, 150, 460, 2);
		TblMonsters.Rows.Add(41, "Титан", 7, 300, 935, 2);
		TblMonsters.Rows.Add(42, "Бес", 1, 4, 5, 3);
		TblMonsters.Rows.Add(43, "Черт", 1, 4, 5, 3);
		TblMonsters.Rows.Add(44, "Гог", 2, 13, 15, 3);
		TblMonsters.Rows.Add(45, "Магог", 2, 13, 30, 3);
		TblMonsters.Rows.Add(46, "Адская гончая", 3, 25, 40, 3);
		TblMonsters.Rows.Add(47, "Цербер", 3, 25, 45, 3);
		TblMonsters.Rows.Add(48, "Демон", 4, 35, 55, 3);
		TblMonsters.Rows.Add(49, "Рогатый демон", 4, 40, 60, 3);
		TblMonsters.Rows.Add(50, "Порождение зла", 5, 45, 95, 3);
		TblMonsters.Rows.Add(51, "Адское отродье", 5, 45, 150, 3);
		TblMonsters.Rows.Add(52, "Ифрит", 6, 90, 205, 3);
		TblMonsters.Rows.Add(53, "Ифрит-султан", 6, 90, 230, 3);
		TblMonsters.Rows.Add(54, "Дьявол", 7, 160, 635, 3);
		TblMonsters.Rows.Add(55, "Архидьявол", 7, 200, 885, 3);
		TblMonsters.Rows.Add(56, "Скелет", 1, 6, 5, 4);
		TblMonsters.Rows.Add(57, "Воин скелет", 1, 6, 10, 4);
		TblMonsters.Rows.Add(58, "Живой мертвец", 2, 15, 10, 4);
		TblMonsters.Rows.Add(59, "Зомби", 2, 20, 15, 4);
		TblMonsters.Rows.Add(60, "Страж", 3, 18, 30, 4);
		TblMonsters.Rows.Add(61, "Привидение", 3, 18, 35, 4);
		TblMonsters.Rows.Add(62, "Вампир", 4, 30, 65, 4);
		TblMonsters.Rows.Add(63, "Вампир лорд", 4, 40, 95, 4);
		TblMonsters.Rows.Add(64, "Лич", 5, 30, 105, 4);
		TblMonsters.Rows.Add(65, "Могущественный лич", 5, 40, 130, 4);
		TblMonsters.Rows.Add(66, "Черный рыцарь", 6, 120, 260, 4);
		TblMonsters.Rows.Add(67, "Рыцарь смерти", 6, 120, 295, 4);
		TblMonsters.Rows.Add(68, "Костяной дракон", 7, 150, 420, 4);
		TblMonsters.Rows.Add(69, "Дракон-привидение", 7, 200, 585, 4);
		TblMonsters.Rows.Add(70, "Троглодит", 1, 5, 5, 5);
		TblMonsters.Rows.Add(71, "Адский троглодит", 1, 6, 10, 5);
		TblMonsters.Rows.Add(72, "Гарпия", 2, 14, 15, 5);
		TblMonsters.Rows.Add(73, "Гарпия-ведьма", 2, 14, 25, 5);
		TblMonsters.Rows.Add(74, "Бехолдер", 3, 22, 40, 5);
		TblMonsters.Rows.Add(75, "Злой глаз", 3, 22, 45, 5);
		TblMonsters.Rows.Add(76, "Медуза", 4, 25, 60, 5);
		TblMonsters.Rows.Add(77, "Королева медуза", 4, 30, 70, 5);
		TblMonsters.Rows.Add(78, "Минотавр", 5, 50, 100, 5);
		TblMonsters.Rows.Add(79, "Король минотавр", 5, 50, 130, 5);
		TblMonsters.Rows.Add(80, "Мантикора", 6, 80, 190, 5);
		TblMonsters.Rows.Add(81, "Скорпикора", 6, 80, 195, 5);
		TblMonsters.Rows.Add(82, "Красный дракон", 7, 180, 585, 5);
		TblMonsters.Rows.Add(83, "Черный дракон", 7, 300, 1090, 5);
		TblMonsters.Rows.Add(84, "Гоблин", 1, 5, 5, 6);
		TblMonsters.Rows.Add(85, "Хобгоблин", 1, 5, 5, 6);
		TblMonsters.Rows.Add(86, "Наездник на волках", 2, 10, 15, 6);
		TblMonsters.Rows.Add(87, "Налетчик", 2, 10, 25, 6);
		TblMonsters.Rows.Add(88, "Орк", 3, 15, 20, 6);
		TblMonsters.Rows.Add(89, "Орк-вождь", 3, 20, 30, 6);
		TblMonsters.Rows.Add(90, "Огр", 4, 40, 50, 6);
		TblMonsters.Rows.Add(91, "Огр-шаман", 4, 60, 80, 6);
		TblMonsters.Rows.Add(92, "Птица рух", 5, 60, 125, 6);
		TblMonsters.Rows.Add(93, "Птица-гром", 5, 60, 135, 6);
		TblMonsters.Rows.Add(94, "Циклоп", 6, 70, 155, 6);
		TblMonsters.Rows.Add(95, "Король циклопов", 6, 70, 180, 6);
		TblMonsters.Rows.Add(96, "Чудище", 7, 160, 395, 6);
		TblMonsters.Rows.Add(97, "Древнее чудище", 7, 300, 770, 6);
		TblMonsters.Rows.Add(98, "Гнолл", 1, 6, 5, 7);
		TblMonsters.Rows.Add(99, "Гнолл-мародер", 1, 6, 10, 7);
		TblMonsters.Rows.Add(100, "Ящер", 2, 14, 15, 7);
		TblMonsters.Rows.Add(101, "Ящер-воин", 2, 15, 15, 7);
		TblMonsters.Rows.Add(102, "Горгона", 5, 70, 110, 7);
		TblMonsters.Rows.Add(103, "Могучая горгона", 5, 70, 125, 7);
		TblMonsters.Rows.Add(104, "Змий", 3, 20, 30, 7);
		TblMonsters.Rows.Add(105, "Змий-дракон", 3, 20, 35, 7);
		TblMonsters.Rows.Add(106, "Василиск", 4, 35, 65, 7);
		TblMonsters.Rows.Add(107, "Великий василиск", 4, 40, 85, 7);
		TblMonsters.Rows.Add(108, "Виверн", 6, 70, 165, 7);
		TblMonsters.Rows.Add(109, "Виверн-монарх", 6, 70, 185, 7);
		TblMonsters.Rows.Add(110, "Гидра", 7, 175, 515, 7);
		TblMonsters.Rows.Add(111, "Гидра хаоса", 7, 250, 740, 7);
		TblMonsters.Rows.Add(112, "Воздушный элементаль", 2, 25, 40, 8);
		TblMonsters.Rows.Add(113, "Элементаль земли", 5, 40, 40, 8);
		TblMonsters.Rows.Add(114, "Огненный элементаль", 4, 35, 40, 8);
		TblMonsters.Rows.Add(115, "Элементаль воды", 3, 30, 35, 8);
		TblMonsters.Rows.Add(116, "Золотой голем", 5, 50, 75);
		TblMonsters.Rows.Add(117, "Алмазный голем", 6, 60, 95);
		TblMonsters.Rows.Add(118, "Маленькая фея", 1, 3, 5, 8);
		TblMonsters.Rows.Add(119, "Фея", 1, 3, 10, 8);
		TblMonsters.Rows.Add(120, "Психический элементаль", 6, 75, 205, 8);
		TblMonsters.Rows.Add(121, "Магический элементаль", 6, 80, 250, 8);
		TblMonsters.Rows.Add(122, "НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonsters.Rows.Add(123, "Ледяной элементаль", 3, 30, 45, 8);
		TblMonsters.Rows.Add(124, "НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonsters.Rows.Add(125, "Элементаль магмы", 5, 40, 60, 8);
		TblMonsters.Rows.Add(126, "НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonsters.Rows.Add(127, "Элементаль шторма", 2, 25, 60, 8);
		TblMonsters.Rows.Add(128, "НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonsters.Rows.Add(129, "Энергетический элементаль", 4, 35, 55, 8);
		TblMonsters.Rows.Add(130, "Огненная птица", 7, 150, 565, 8);
		TblMonsters.Rows.Add(131, "Феникс", 7, 200, 840, 8);
		TblMonsters.Rows.Add(132, "Лазурный дракон", 7, 1000, 9855);
		TblMonsters.Rows.Add(133, "Кристальный дракон", 7, 800, 4915);
		TblMonsters.Rows.Add(134, "Сказочный дракон", 7, 500, 2445);
		TblMonsters.Rows.Add(135, "Ржавый дракон", 7, 750, 3300);
		TblMonsters.Rows.Add(136, "Чародей", 6, 30, 150);
		TblMonsters.Rows.Add(137, "Снайпер", 4, 15, 70);
		TblMonsters.Rows.Add(138, "Хоббит", 1, 4, 5);
		TblMonsters.Rows.Add(139, "Крестьянин", 1, 1, 0);
		TblMonsters.Rows.Add(140, "Орк на кабане", 2, 15, 15);
		TblMonsters.Rows.Add(141, "Мумия", 3, 30, 30);
		TblMonsters.Rows.Add(142, "Кочевник", 3, 30, 40);
		TblMonsters.Rows.Add(143, "Вор", 2, 10, 15);
		TblMonsters.Rows.Add(144, "Тролль", 5, 40, 125);
		TblMonsters.Rows.Add(145, "Катапульта", 0, 0, 0);
		TblMonsters.Rows.Add(146, "Баллиста", 0, 0, 0);
		TblMonsters.Rows.Add(147, "Палатка первой помощи", 0, 0, 0);
		TblMonsters.Rows.Add(148, "Подвода с боеприпасами", 0, 0, 0);
	}

	private void CreateTblBuilding()
	{
		TblBuilding.Columns.Add("Type", Type.GetType("System.Int32"));
		TblBuilding.Columns.Add("Byte", Type.GetType("System.Int32"));
		TblBuilding.Columns.Add("Bit", Type.GetType("System.Int32"));
		TblBuilding.Columns.Add("Building", Type.GetType("System.String"));
		TblBuilding.Columns.Add("Complete", Type.GetType("System.String"));
		TblBuilding.Columns.Add("SoDEn", Type.GetType("System.String"));
		TblBuilding.Rows.Add(0, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(0, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(0, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(0, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(0, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(0, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(0, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(0, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(0, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(0, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(0, 2, 2, "", "", "");
		TblBuilding.Rows.Add(0, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(0, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(0, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(0, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(0, 2, 7, "", "", "");
		TblBuilding.Rows.Add(0, 3, 0, "Верфь", "", "Shipyard");
		TblBuilding.Rows.Add(0, 3, 1, "Колосс", "", "Colossus");
		TblBuilding.Rows.Add(0, 3, 2, "Маяк", "", "Lighthouse");
		TblBuilding.Rows.Add(0, 3, 3, "Братство меча", "", "Brotherhood");
		TblBuilding.Rows.Add(0, 3, 4, "Конюшни", "", "Stables");
		TblBuilding.Rows.Add(0, 3, 5, "", "", "");
		TblBuilding.Rows.Add(0, 3, 6, "Сторожевой пост", "Караулка", "Guardhouse");
		TblBuilding.Rows.Add(0, 3, 7, "Ул.сторожевой пост", "Сторожевая", "Upg. Guardhouse");
		TblBuilding.Rows.Add(0, 4, 0, "", "", "");
		TblBuilding.Rows.Add(0, 4, 1, "Башня лучников", "Башня стрелков", "Archers' Tower");
		TblBuilding.Rows.Add(0, 4, 2, "Ул.башня лучников", "Башня лучников", "Upg. Archers' Tower");
		TblBuilding.Rows.Add(0, 4, 3, "", "", "");
		TblBuilding.Rows.Add(0, 4, 4, "Башня Грифонов", "", "Griffin Tower");
		TblBuilding.Rows.Add(0, 4, 5, "Ул.башня грифонов", "Крепость грифонов", "Upg. Griffin Tower");
		TblBuilding.Rows.Add(0, 4, 6, "Бастион грифонов", "", "Griffin Bastion");
		TblBuilding.Rows.Add(0, 4, 7, "Казармы", "Бараки", "Barracks");
		TblBuilding.Rows.Add(0, 5, 0, "Ул.казармы", "Казармы", "Upg. Barracks");
		TblBuilding.Rows.Add(0, 5, 1, "", "", "");
		TblBuilding.Rows.Add(0, 5, 2, "Монастырь", "", "Monastery");
		TblBuilding.Rows.Add(0, 5, 3, "Ул. монастырь", "Храм", "Upg. Monastery");
		TblBuilding.Rows.Add(0, 5, 4, "", "", "");
		TblBuilding.Rows.Add(0, 5, 5, "Ипподром", "Арена", "Training Grounds");
		TblBuilding.Rows.Add(0, 5, 6, "Ул.ипподром", "Ристалище", "Upg. Training Grounds");
		TblBuilding.Rows.Add(0, 5, 7, "Портал славы", "", "Portal of Glory");
		TblBuilding.Rows.Add(0, 6, 0, "Ул.портал славы", "Портал доблести", "Upg. Portal of Glory");
		TblBuilding.Rows.Add(1, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(1, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(1, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(1, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(1, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(1, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(1, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(1, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(1, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(1, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(1, 2, 2, "", "", "");
		TblBuilding.Rows.Add(1, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(1, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(1, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(1, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(1, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(1, 3, 0, "", "", "");
		TblBuilding.Rows.Add(1, 3, 1, "Хранитель духа", "Дух-хранитель", "Spirit Guardian");
		TblBuilding.Rows.Add(1, 3, 2, "Таинственный пруд", "", "Mystic Pond");
		TblBuilding.Rows.Add(1, 3, 3, "Фонтан удачи", "", "Fountain of Fortune");
		TblBuilding.Rows.Add(1, 3, 4, "Сокровищница", "", "Treasury");
		TblBuilding.Rows.Add(1, 3, 5, "", "", "");
		TblBuilding.Rows.Add(1, 3, 6, "Конюшни кентавров", "", "Centaur Stables");
		TblBuilding.Rows.Add(1, 3, 7, "Ул.конюшни кентавров", "Двор кентавров", "Upg. Centaur Stables");
		TblBuilding.Rows.Add(1, 4, 0, "", "", "");
		TblBuilding.Rows.Add(1, 4, 1, "Коттедж гномов", "Избушка гномов", "Dwarf Cottage");
		TblBuilding.Rows.Add(1, 4, 2, "Ул.коттедж гномов", "Хоромы гномов", "Upg. Dwarf Cottage");
		TblBuilding.Rows.Add(1, 4, 3, "Гильдия горняков", "", "Miners' Guild");
		TblBuilding.Rows.Add(1, 4, 4, "Усадьба", "", "Homestead");
		TblBuilding.Rows.Add(1, 4, 5, "Ул.усадьба", "Большая усадьба", "Upg. Homestead");
		TblBuilding.Rows.Add(1, 4, 6, "", "", "");
		TblBuilding.Rows.Add(1, 4, 7, "Заколдованный ручей", "Волшебный родник", "Enchanted Spring");
		TblBuilding.Rows.Add(1, 5, 0, "Ул.заколдованный ручей", "Волшебный источник", "Upg. Enchanted Spring");
		TblBuilding.Rows.Add(1, 5, 1, "", "", "");
		TblBuilding.Rows.Add(1, 5, 2, "Арка дендроидов", "Роща дендроидов", "Dendroid Arches");
		TblBuilding.Rows.Add(1, 5, 3, "Ул.арка дендроидов", "Лес дендроидов", "Upg. Dendroid Arches");
		TblBuilding.Rows.Add(1, 5, 4, "Молодые дендроиды", "Роща дендроидов", "Dendroid Saplings");
		TblBuilding.Rows.Add(1, 5, 5, "Лужайка единорогов", "Поляна единорогов", "Unicorn Glade");
		TblBuilding.Rows.Add(1, 5, 6, "Ул.лужайка единорогов", "Лужайка единорогов", "Upg. Unicorn Glade");
		TblBuilding.Rows.Add(1, 5, 7, "Драконьи скалы", "Утесы дракона", "Dragon Cliffs");
		TblBuilding.Rows.Add(1, 6, 0, "Ул.драконьи скалы", "Скала дракона", "Upg. Dragon Cliffs");
		TblBuilding.Rows.Add(2, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(2, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(2, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(2, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(2, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(2, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(2, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(2, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(2, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(2, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(2, 2, 2, "Торговцы артефактами", "Торговец артефактами", "Artifact Merchants");
		TblBuilding.Rows.Add(2, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(2, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(2, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(2, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(2, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(2, 3, 0, "", "", "");
		TblBuilding.Rows.Add(2, 3, 1, "Небесный корабль", "", "Skyship");
		TblBuilding.Rows.Add(2, 3, 2, "Библиотека", "", "Library");
		TblBuilding.Rows.Add(2, 3, 3, "Стена знаний", "", "Wall of Knowledge");
		TblBuilding.Rows.Add(2, 3, 4, "Смотровая башня", "Сторожевая башня", "Lookout Tower");
		TblBuilding.Rows.Add(2, 3, 5, "", "", "");
		TblBuilding.Rows.Add(2, 3, 6, "Мастерская", "", "Workshop");
		TblBuilding.Rows.Add(2, 3, 7, "Ул.мастерская", "Мануфактура", "Upg. Workshop");
		TblBuilding.Rows.Add(2, 4, 0, "", "", "");
		TblBuilding.Rows.Add(2, 4, 1, "Парапет", "", "Parapet");
		TblBuilding.Rows.Add(2, 4, 2, "Ул.парапет", "Часовня", "Upg. Parapet");
		TblBuilding.Rows.Add(2, 4, 3, "Крылья ваятеля", "Мастерская скульптора", "Sculptor's Wings");
		TblBuilding.Rows.Add(2, 4, 4, "Фабрика големов", "", "Golem Factory");
		TblBuilding.Rows.Add(2, 4, 5, "Ул.фабрика големов", "Завод големов", "Upg. Golem Factory");
		TblBuilding.Rows.Add(2, 4, 6, "", "", "");
		TblBuilding.Rows.Add(2, 4, 7, "Башня магов", "", "Mage Tower");
		TblBuilding.Rows.Add(2, 5, 0, "Ул.башня магов", "Оплот магов", "Upg. Mage Tower");
		TblBuilding.Rows.Add(2, 5, 1, "", "", "");
		TblBuilding.Rows.Add(2, 5, 2, "Алтарь желаний", "Алтарь грез", "Altar of Wishes");
		TblBuilding.Rows.Add(2, 5, 3, "Ул.алтарь желаний", "Алтарь мечты", "Upg. Altar of Wishes");
		TblBuilding.Rows.Add(2, 5, 4, "", "", "");
		TblBuilding.Rows.Add(2, 5, 5, "Золотой павильон", "", "Golden Pavilion");
		TblBuilding.Rows.Add(2, 5, 6, "Ул.золотой павильон", "Золотой сад", "Upg. Golden Pavilion");
		TblBuilding.Rows.Add(2, 5, 7, "Заоблачный храм", "Небесный храм", "Cloud Temple");
		TblBuilding.Rows.Add(2, 6, 0, "Ул.заоблачный храм", "Небесный чертог", "Upg. Cloud Temple");
		TblBuilding.Rows.Add(3, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(3, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(3, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(3, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(3, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(3, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(3, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(3, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(3, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(3, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(3, 2, 2, "", "", "");
		TblBuilding.Rows.Add(3, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(3, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(3, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(3, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(3, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(3, 3, 0, "", "", "");
		TblBuilding.Rows.Add(3, 3, 1, "Бог Огня", "", "Deity of Fire");
		TblBuilding.Rows.Add(3, 3, 2, "Серные тучи", "Облака серы", "Brimstone Stormclouds");
		TblBuilding.Rows.Add(3, 3, 3, "Врата замка", "Врата города", "Castle Gate");
		TblBuilding.Rows.Add(3, 3, 4, "Орден огня", "", "Order of Fire");
		TblBuilding.Rows.Add(3, 3, 5, "", "", "");
		TblBuilding.Rows.Add(3, 3, 6, "Котел бесов", "Тигель бесов", "Imp Crucible");
		TblBuilding.Rows.Add(3, 3, 7, "Ул.котел бесов", "Котел бесов", "Upg. Imp Crucible");
		TblBuilding.Rows.Add(3, 4, 0, "Инкубатор", "", "Birthing Pools");
		TblBuilding.Rows.Add(3, 4, 1, "Дворец пророков", "Зал грехов", "Hall of Sins");
		TblBuilding.Rows.Add(3, 4, 2, "Ул.дворец пророков", "Храм грехов", "Upg. Hall of Sins");
		TblBuilding.Rows.Add(3, 4, 3, "", "", "");
		TblBuilding.Rows.Add(3, 4, 4, "Псарни", "", "Kennels");
		TblBuilding.Rows.Add(3, 4, 5, "Ул.псарни", "Большие псарни", "Upg. Kennels");
		TblBuilding.Rows.Add(3, 4, 6, "Клетки", "", "Cages");
		TblBuilding.Rows.Add(3, 4, 7, "Врата демонов", "", "Demon Gate");
		TblBuilding.Rows.Add(3, 5, 0, "Ул.врата демонов", "Врата Ада", "Upg. Demon Gate");
		TblBuilding.Rows.Add(3, 5, 1, "", "", "");
		TblBuilding.Rows.Add(3, 5, 2, "Провал", "Геенна", "Hell Hole");
		TblBuilding.Rows.Add(3, 5, 3, "Ул.провал", "Преисподняя", "Upg. Hell Hole");
		TblBuilding.Rows.Add(3, 5, 4, "", "", "");
		TblBuilding.Rows.Add(3, 5, 5, "Огненное озеро", "", "Fire Lake");
		TblBuilding.Rows.Add(3, 5, 6, "Ул.огненное озеро", "Огненное море", "Upg. Fire Lake");
		TblBuilding.Rows.Add(3, 5, 7, "Покинутый дворец", "Заброшенный дворец", "Forsaken Palace");
		TblBuilding.Rows.Add(3, 6, 0, "Ул.покинутый дворец", "Заброшенный замок", "Upg. Forsaken Palace");
		TblBuilding.Rows.Add(4, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(4, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(4, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(4, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(4, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(4, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(4, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(4, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(4, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(4, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(4, 2, 2, "", "", "");
		TblBuilding.Rows.Add(4, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(4, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(4, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(4, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(4, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(4, 3, 0, "Верфь", "", "Shipyard");
		TblBuilding.Rows.Add(4, 3, 1, "Темница душ", "", "Soul Prison");
		TblBuilding.Rows.Add(4, 3, 2, "Вуаль тьмы", "Завеса тьмы", "Cover of Darkness");
		TblBuilding.Rows.Add(4, 3, 3, "Усилитель черной магии", "Усилитель некромантии", "Necromancy Amplifier");
		TblBuilding.Rows.Add(4, 3, 4, "Преобразователь скелетов", "Машина скелетов", "Skeleton Transformer");
		TblBuilding.Rows.Add(4, 3, 5, "", "", "");
		TblBuilding.Rows.Add(4, 3, 6, "Проклятый замок", "", "Cursed Temple");
		TblBuilding.Rows.Add(4, 3, 7, "Ул.проклятый замок", "Проклятый чертог", "Upg. Cursed Temple");
		TblBuilding.Rows.Add(4, 4, 0, "Разрытые могилы", "", "Unearthed Graves");
		TblBuilding.Rows.Add(4, 4, 1, "Кладбище", "", "Graveyard");
		TblBuilding.Rows.Add(4, 4, 2, "Ул.кладбище", "Курган", "Upg. Graveyard");
		TblBuilding.Rows.Add(4, 4, 3, "", "", "");
		TblBuilding.Rows.Add(4, 4, 4, "Пристанище душ", "Гробница душ", "Tomb of Souls");
		TblBuilding.Rows.Add(4, 4, 5, "Ул.пристанище душ", "Склеп душ", "Upg. Tomb of Souls");
		TblBuilding.Rows.Add(4, 4, 6, "", "", "");
		TblBuilding.Rows.Add(4, 4, 7, "Поместье", "", "Estate");
		TblBuilding.Rows.Add(4, 5, 0, "Ул.поместье", "Большое поместье", "Upg. Estate");
		TblBuilding.Rows.Add(4, 5, 1, "", "", "");
		TblBuilding.Rows.Add(4, 5, 2, "Мавзолей", "", "Mausoleum");
		TblBuilding.Rows.Add(4, 5, 3, "Ул.мавзолей", "Старый мавзолей", "Upg. Mausoleum");
		TblBuilding.Rows.Add(4, 5, 4, "", "", "");
		TblBuilding.Rows.Add(4, 5, 5, "Дворец тьмы", "Чертог Тьмы", "Hall of Darkness");
		TblBuilding.Rows.Add(4, 5, 6, "Ул.дворец тьмы", "Храм Тьмы", "Upg. Hall of Darkness");
		TblBuilding.Rows.Add(4, 5, 7, "Склеп драконов", "Склеп дракона", "Dragon Vault");
		TblBuilding.Rows.Add(4, 6, 0, "Ул.склеп драконов", "Гробница дракона", "Upg. Dragon Vault");
		TblBuilding.Rows.Add(5, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(5, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(5, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(5, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(5, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(5, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(5, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(5, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(5, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(5, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(5, 2, 2, "Торговцы артефактами", "Торговец артефактами", "Artifact Merchants");
		TblBuilding.Rows.Add(5, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(5, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(5, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(5, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(5, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(5, 3, 0, "", "", "");
		TblBuilding.Rows.Add(5, 3, 1, "Хранитель земли", "Страж земли", "Guardian of Earth");
		TblBuilding.Rows.Add(5, 3, 2, "Вихрь маны", "", "Mana Vortex");
		TblBuilding.Rows.Add(5, 3, 3, "Портал вызова", "Портал", "Portal of Summoning");
		TblBuilding.Rows.Add(5, 3, 4, "Академия боевых искусств", "Военная академия", "Battle Scholar Academy");
		TblBuilding.Rows.Add(5, 3, 5, "", "", "");
		TblBuilding.Rows.Add(5, 3, 6, "Загон", "Садок", "Warren");
		TblBuilding.Rows.Add(5, 3, 7, "Ул.загон", "Роща", "Upg. Warren");
		TblBuilding.Rows.Add(5, 4, 0, "Грибные кольца", "Кольца грибов", "Mushroom Rings");
		TblBuilding.Rows.Add(5, 4, 1, "Чердак гарпий", "", "Harpy Loft");
		TblBuilding.Rows.Add(5, 4, 2, "Ул.чердак гарпий", "Хоры гарпий", "Upg. Harpy Loft");
		TblBuilding.Rows.Add(5, 4, 3, "", "", "");
		TblBuilding.Rows.Add(5, 4, 4, "Камень глаз", "Глазница", "Pillar of Eyes");
		TblBuilding.Rows.Add(5, 4, 5, "Ул.камень глаз", "Великая глазница", "Upg. Pillar of Eyes");
		TblBuilding.Rows.Add(5, 4, 6, "", "", "");
		TblBuilding.Rows.Add(5, 4, 7, "Часовня безмолвия", "Капелла безмолвия", "Chapel of Stilled Voices");
		TblBuilding.Rows.Add(5, 5, 0, "Ул.часовня безмолвия", "Капелла тишины", "Upg. Stilled Voices");
		TblBuilding.Rows.Add(5, 5, 1, "", "", "");
		TblBuilding.Rows.Add(5, 5, 2, "Лабиринт", "", "Labyrinth");
		TblBuilding.Rows.Add(5, 5, 3, "Ул.лабиринт", "Великий лабиринт", "Upg. Labyrinth");
		TblBuilding.Rows.Add(5, 5, 4, "", "", "");
		TblBuilding.Rows.Add(5, 5, 5, "Логово мантикор", "", "Manticore Lair");
		TblBuilding.Rows.Add(5, 5, 6, "Ул.логово мантикор", "Гнездо мантикор", "Upg. Manticore Lair");
		TblBuilding.Rows.Add(5, 5, 7, "Пещера драконов", "", "Dragon Cave");
		TblBuilding.Rows.Add(5, 6, 0, "Ул.пещера драконов", "Логово драконов", "Upg. Dragon Cave");
		TblBuilding.Rows.Add(6, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(6, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(6, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(6, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(6, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(6, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(6, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(6, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(6, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(6, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(6, 2, 2, "", "", "");
		TblBuilding.Rows.Add(6, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(6, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(6, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(6, 2, 6, "", "", "");
		TblBuilding.Rows.Add(6, 2, 7, "", "", "");
		TblBuilding.Rows.Add(6, 3, 0, "", "", "");
		TblBuilding.Rows.Add(6, 3, 1, "Памятник богам войны", "Монумент полководца", "Warlords' Monument");
		TblBuilding.Rows.Add(6, 3, 2, "Черный ход", "Подземный ход", "Escape Tunnel");
		TblBuilding.Rows.Add(6, 3, 3, "Гильдия наемников", "", "Freelancer's Guild");
		TblBuilding.Rows.Add(6, 3, 4, "Двор баллист", "", "Ballista Yard");
		TblBuilding.Rows.Add(6, 3, 5, "Храм валгаллы", "Валгалла", "Hall of Valhalla");
		TblBuilding.Rows.Add(6, 3, 6, "Казармы гоблинов", "Бараки гоблинов", "Goblin Barracks");
		TblBuilding.Rows.Add(6, 3, 7, "Ул.казармы гоблинов", "Казармы гоблинов", "Upg. Goblin Barracks");
		TblBuilding.Rows.Add(6, 4, 0, "Столовая", "Трапезная", "Mess Hall");
		TblBuilding.Rows.Add(6, 4, 1, "Волчий загон", "Волчье логово", "Wolf Pen");
		TblBuilding.Rows.Add(6, 4, 2, "Ул.волчий загон", "Волчий загон", "Upg. Wolf Pen");
		TblBuilding.Rows.Add(6, 4, 3, "", "", "");
		TblBuilding.Rows.Add(6, 4, 4, "Башня орков", "", "Orc Tower");
		TblBuilding.Rows.Add(6, 4, 5, "Ул.башня орков", "Крепость орков", "Upg. Orc Tower");
		TblBuilding.Rows.Add(6, 4, 6, "", "", "");
		TblBuilding.Rows.Add(6, 4, 7, "Форт огров", "", "Ogre Fort");
		TblBuilding.Rows.Add(6, 5, 0, "Ул.форт огров", "Крепость огров", "Upg. Ogre Fort");
		TblBuilding.Rows.Add(6, 5, 1, "", "", "");
		TblBuilding.Rows.Add(6, 5, 2, "Гнездо на скале", "Гнездо на утесе", "Cliff Nest");
		TblBuilding.Rows.Add(6, 5, 3, "Ул.гнездо на скале", "Гнездо на скале", "Upg. Cliff Nest");
		TblBuilding.Rows.Add(6, 5, 4, "", "", "");
		TblBuilding.Rows.Add(6, 5, 5, "Пещера циклопов", "", "Cyclops Cave");
		TblBuilding.Rows.Add(6, 5, 6, "Ул.пещера циклопов", "Логово циклопов", "Upg. Cyclops Cave");
		TblBuilding.Rows.Add(6, 5, 7, "Утес чудищ", "Обиталище чудищ", "Behemoth Lair");
		TblBuilding.Rows.Add(6, 6, 0, "Ул.утес чудищ", "Логово чудищ", "Upg. Behemoth Lair");
		TblBuilding.Rows.Add(7, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(7, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(7, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(7, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(7, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(7, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(7, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(7, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(7, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(7, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(7, 2, 2, "", "", "");
		TblBuilding.Rows.Add(7, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(7, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(7, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(7, 2, 6, "", "", "");
		TblBuilding.Rows.Add(7, 2, 7, "", "", "");
		TblBuilding.Rows.Add(7, 3, 0, "Верфь", "", "Shipyard");
		TblBuilding.Rows.Add(7, 3, 1, "Плотоядное растение", "", "Carnivorous Plant");
		TblBuilding.Rows.Add(7, 3, 2, "Клетка богов войны", "Клеть воителя", "Cage of Warlords");
		TblBuilding.Rows.Add(7, 3, 3, "Знаки страха", "Глифы Ужаса", "Glyphs of Fear");
		TblBuilding.Rows.Add(7, 3, 4, "Обелиск крови", "Кровавый обелиск", "Blood Obelisk");
		TblBuilding.Rows.Add(7, 3, 5, "", "", "");
		TblBuilding.Rows.Add(7, 3, 6, "Хижина гноллов", "Лачуга гноллов", "Gnoll Hut");
		TblBuilding.Rows.Add(7, 3, 7, "Ул.хижина гноллов", "Хибара гноллов", "Upg. Gnoll Hut");
		TblBuilding.Rows.Add(7, 4, 0, "Квартира капитана", "Жилье капитана", "Captain's Quarters");
		TblBuilding.Rows.Add(7, 4, 1, "Логово ящериц", "Пещера ящеров", "Lizard Den");
		TblBuilding.Rows.Add(7, 4, 2, "Ул.логово ящериц", "Логово ящеров", "Upg. Lizard Den");
		TblBuilding.Rows.Add(7, 4, 3, "", "", "");
		TblBuilding.Rows.Add(7, 4, 4, "Улей змиев", "Змеиное гнездо", "Serpent Fly Hive");
		TblBuilding.Rows.Add(7, 4, 5, "Ул.улей змиев", "Змеиное логово", "Upg. Serpent Fly Hive");
		TblBuilding.Rows.Add(7, 4, 6, "", "", "");
		TblBuilding.Rows.Add(7, 4, 7, "Яма василиска", "Яма василисков", "Basilisk Pit");
		TblBuilding.Rows.Add(7, 5, 0, "Ул.яма василиска", "Логово василисков", "Upg. Basilisk Pit");
		TblBuilding.Rows.Add(7, 5, 1, "", "", "");
		TblBuilding.Rows.Add(7, 5, 2, "Логово горгон", "Гнездо горгон", "Gorgon Lair");
		TblBuilding.Rows.Add(7, 5, 3, "Ул.логово горгон", "Логово горгон", "Upg. Gorgon Lair");
		TblBuilding.Rows.Add(7, 5, 4, "", "", "");
		TblBuilding.Rows.Add(7, 5, 5, "Гнездо виверн", "", "Wyvern Nest");
		TblBuilding.Rows.Add(7, 5, 6, "Ул.гнездо виверн", "Насест виверн", "Upg. Wyvern Nest");
		TblBuilding.Rows.Add(7, 5, 7, "Пруд гидр", "", "Hydra Pond");
		TblBuilding.Rows.Add(7, 6, 0, "Ул.пруд гидр", "Озеро гидр", "Upg. Hydra Pond");
		TblBuilding.Rows.Add(8, 1, 0, "Префектура", "Ратуша", "Town Hall");
		TblBuilding.Rows.Add(8, 1, 1, "Муниципалитет", "Магистрат", "City Hall");
		TblBuilding.Rows.Add(8, 1, 2, "Капитолий", "", "Capitol");
		TblBuilding.Rows.Add(8, 1, 3, "Форт", "", "Fort");
		TblBuilding.Rows.Add(8, 1, 4, "Цитадель", "", "Citadel");
		TblBuilding.Rows.Add(8, 1, 5, "Замок", "", "Castle");
		TblBuilding.Rows.Add(8, 1, 6, "Таверна", "", "Tavern");
		TblBuilding.Rows.Add(8, 1, 7, "Кузница", "", "Blacksmith");
		TblBuilding.Rows.Add(8, 2, 0, "Рынок", "", "Marketplace");
		TblBuilding.Rows.Add(8, 2, 1, "Хранилище ресурсов", "Склад ресурсов", "Resource Silo");
		TblBuilding.Rows.Add(8, 2, 2, "Торговцы артефактами", "Торговец артефактами", "Artifact Merchants");
		TblBuilding.Rows.Add(8, 2, 3, "Гильдия магов 1-го уровня", "1 этаж Гильдии магов", "Mage Guild Level 1");
		TblBuilding.Rows.Add(8, 2, 4, "Гильдия магов 2-го уровня", "2 этаж Гильдии магов", "Mage Guild Level 2");
		TblBuilding.Rows.Add(8, 2, 5, "Гильдия магов 3-го уровня", "3 этаж Гильдии магов", "Mage Guild Level 3");
		TblBuilding.Rows.Add(8, 2, 6, "Гильдия магов 4-го уровня", "4 этаж Гильдии магов", "Mage Guild Level 4");
		TblBuilding.Rows.Add(8, 2, 7, "Гильдия магов 5-го уровня", "5 этаж Гильдии магов", "Mage Guild Level 5");
		TblBuilding.Rows.Add(8, 3, 0, "Верфь", "", "Shipyard");
		TblBuilding.Rows.Add(8, 3, 1, "Радуга", "Полярное сияние", "Aurora Borealias");
		TblBuilding.Rows.Add(8, 3, 2, "Университет магии", "Магический университет", "Magic University");
		TblBuilding.Rows.Add(8, 3, 3, "", "", "");
		TblBuilding.Rows.Add(8, 3, 4, "", "", "");
		TblBuilding.Rows.Add(8, 3, 5, "", "", "");
		TblBuilding.Rows.Add(8, 3, 6, "Волшебный фонарь", "", "Magic Lantern");
		TblBuilding.Rows.Add(8, 3, 7, "Ул.волшебный фонарь", "Волшебный омут", "Upg. Magic Lantern");
		TblBuilding.Rows.Add(8, 4, 0, "Сад жизни", "Роща Жизни", "Garden of Life");
		TblBuilding.Rows.Add(8, 4, 1, "Алтарь воздуха", "", "Altar of Air");
		TblBuilding.Rows.Add(8, 4, 2, "Ул.алтарь воздуха", "Храм грома", "Upg. Altar of Air");
		TblBuilding.Rows.Add(8, 4, 3, "", "", "");
		TblBuilding.Rows.Add(8, 4, 4, "Алтарь воды", "", "Altar of Water");
		TblBuilding.Rows.Add(8, 4, 5, "Ул.алтарь воды", "Храм льда", "Upg. Altar of Water");
		TblBuilding.Rows.Add(8, 4, 6, "", "", "");
		TblBuilding.Rows.Add(8, 4, 7, "Алтарь огня", "", "Altar of Fire");
		TblBuilding.Rows.Add(8, 5, 0, "Ул.алтарь огня", "Храм энергии", "Upg. Altar of Fire");
		TblBuilding.Rows.Add(8, 5, 1, "", "", "");
		TblBuilding.Rows.Add(8, 5, 2, "Алтарь земли", "", "Altar of Earth");
		TblBuilding.Rows.Add(8, 5, 3, "Ул.алтарь земли", "Храм магмы", "Upg. Altar of Earth");
		TblBuilding.Rows.Add(8, 5, 4, "", "", "");
		TblBuilding.Rows.Add(8, 5, 5, "Алтарь мысли", "Алтарь разума", "Altar of Thought");
		TblBuilding.Rows.Add(8, 5, 6, "Ул.алтарь мысли", "Храм магии", "Upg. Altar of Thought");
		TblBuilding.Rows.Add(8, 5, 7, "Костер", "Погребальный костер", "Pyre");
		TblBuilding.Rows.Add(8, 6, 0, "Ул.костер", "Похоронный костер", "Upg. Pyre");
	}

	private void CreateTblIdeology()
	{
		TblIdeology.Columns.Add("ID", Type.GetType("System.Int32"));
		TblIdeology.Columns.Add("Ideology", Type.GetType("System.String"));
		TblIdeology.Columns.Add("Class", Type.GetType("System.String"));
		TblIdeology.Columns.Add("ClassID", Type.GetType("System.Int32"));
		TblIdeology.Rows.Add(0, "0", "", 0);
		TblIdeology.Rows.Add(1, "0", "", 0);
		TblIdeology.Rows.Add(2, "0", "", 0);
		TblIdeology.Rows.Add(3, "0", "", 0);
		TblIdeology.Rows.Add(4, "0", "", 0);
		TblIdeology.Rows.Add(5, "0", "", 0);
		TblIdeology.Rows.Add(6, "0", "", 0);
		TblIdeology.Rows.Add(7, "0", "", 0);
		TblIdeology.Rows.Add(8, "0", "", 1);
		TblIdeology.Rows.Add(9, "0", "", 1);
		TblIdeology.Rows.Add(10, "0", "", 1);
		TblIdeology.Rows.Add(11, "0", "", 1);
		TblIdeology.Rows.Add(12, "0", "", 1);
		TblIdeology.Rows.Add(13, "0", "", 1);
		TblIdeology.Rows.Add(14, "0", "", 1);
		TblIdeology.Rows.Add(15, "0", "", 1);
		TblIdeology.Rows.Add(16, "0", "", 2);
		TblIdeology.Rows.Add(17, "0", "", 2);
		TblIdeology.Rows.Add(18, "0", "", 2);
		TblIdeology.Rows.Add(19, "0", "", 2);
		TblIdeology.Rows.Add(20, "0", "", 2);
		TblIdeology.Rows.Add(21, "0", "", 2);
		TblIdeology.Rows.Add(22, "0", "", 2);
		TblIdeology.Rows.Add(23, "0", "", 2);
		TblIdeology.Rows.Add(24, "0", "", 3);
		TblIdeology.Rows.Add(25, "0", "", 3);
		TblIdeology.Rows.Add(26, "0", "", 3);
		TblIdeology.Rows.Add(27, "0", "", 3);
		TblIdeology.Rows.Add(28, "0", "", 3);
		TblIdeology.Rows.Add(29, "0", "", 3);
		TblIdeology.Rows.Add(30, "0", "", 3);
		TblIdeology.Rows.Add(31, "0", "", 3);
		TblIdeology.Rows.Add(32, "0", "", 4);
		TblIdeology.Rows.Add(33, "0", "", 4);
		TblIdeology.Rows.Add(34, "0", "", 4);
		TblIdeology.Rows.Add(35, "0", "", 4);
		TblIdeology.Rows.Add(36, "0", "", 4);
		TblIdeology.Rows.Add(37, "0", "", 4);
		TblIdeology.Rows.Add(38, "0", "", 4);
		TblIdeology.Rows.Add(39, "0", "", 4);
		TblIdeology.Rows.Add(40, "0", "", 5);
		TblIdeology.Rows.Add(41, "0", "", 5);
		TblIdeology.Rows.Add(42, "0", "", 5);
		TblIdeology.Rows.Add(43, "0", "", 5);
		TblIdeology.Rows.Add(44, "0", "", 5);
		TblIdeology.Rows.Add(45, "0", "", 5);
		TblIdeology.Rows.Add(46, "0", "", 5);
		TblIdeology.Rows.Add(47, "0", "", 5);
		TblIdeology.Rows.Add(48, "1", "", 6);
		TblIdeology.Rows.Add(49, "1", "", 6);
		TblIdeology.Rows.Add(50, "1", "", 6);
		TblIdeology.Rows.Add(51, "1", "", 6);
		TblIdeology.Rows.Add(52, "1", "", 6);
		TblIdeology.Rows.Add(53, "1", "", 6);
		TblIdeology.Rows.Add(54, "1", "", 6);
		TblIdeology.Rows.Add(55, "1", "", 6);
		TblIdeology.Rows.Add(56, "1", "", 7);
		TblIdeology.Rows.Add(57, "1", "", 7);
		TblIdeology.Rows.Add(58, "1", "", 7);
		TblIdeology.Rows.Add(59, "1", "", 7);
		TblIdeology.Rows.Add(60, "1", "", 7);
		TblIdeology.Rows.Add(61, "1", "", 7);
		TblIdeology.Rows.Add(62, "1", "", 7);
		TblIdeology.Rows.Add(63, "1", "", 7);
		TblIdeology.Rows.Add(64, "1", "", 8);
		TblIdeology.Rows.Add(65, "1", "", 8);
		TblIdeology.Rows.Add(66, "1", "", 8);
		TblIdeology.Rows.Add(67, "1", "", 8);
		TblIdeology.Rows.Add(68, "1", "", 8);
		TblIdeology.Rows.Add(69, "1", "", 8);
		TblIdeology.Rows.Add(70, "1", "", 8);
		TblIdeology.Rows.Add(71, "1", "", 8);
		TblIdeology.Rows.Add(72, "1", "", 9);
		TblIdeology.Rows.Add(73, "1", "", 9);
		TblIdeology.Rows.Add(74, "1", "", 9);
		TblIdeology.Rows.Add(75, "1", "", 9);
		TblIdeology.Rows.Add(76, "1", "", 9);
		TblIdeology.Rows.Add(77, "1", "", 9);
		TblIdeology.Rows.Add(78, "1", "", 9);
		TblIdeology.Rows.Add(79, "1", "", 9);
		TblIdeology.Rows.Add(80, "1", "", 10);
		TblIdeology.Rows.Add(81, "1", "", 10);
		TblIdeology.Rows.Add(82, "1", "", 10);
		TblIdeology.Rows.Add(83, "1", "", 10);
		TblIdeology.Rows.Add(84, "1", "", 10);
		TblIdeology.Rows.Add(85, "1", "", 10);
		TblIdeology.Rows.Add(86, "1", "", 10);
		TblIdeology.Rows.Add(87, "1", "", 10);
		TblIdeology.Rows.Add(88, "1", "", 11);
		TblIdeology.Rows.Add(89, "1", "", 11);
		TblIdeology.Rows.Add(90, "1", "", 11);
		TblIdeology.Rows.Add(91, "1", "", 11);
		TblIdeology.Rows.Add(92, "1", "", 11);
		TblIdeology.Rows.Add(93, "1", "", 11);
		TblIdeology.Rows.Add(94, "1", "", 11);
		TblIdeology.Rows.Add(95, "1", "", 11);
		TblIdeology.Rows.Add(96, "2", "", 12);
		TblIdeology.Rows.Add(97, "2", "", 12);
		TblIdeology.Rows.Add(98, "2", "", 12);
		TblIdeology.Rows.Add(99, "2", "", 12);
		TblIdeology.Rows.Add(100, "2", "", 12);
		TblIdeology.Rows.Add(101, "2", "", 12);
		TblIdeology.Rows.Add(102, "2", "", 12);
		TblIdeology.Rows.Add(103, "2", "", 12);
		TblIdeology.Rows.Add(104, "2", "", 13);
		TblIdeology.Rows.Add(105, "2", "", 13);
		TblIdeology.Rows.Add(106, "2", "", 13);
		TblIdeology.Rows.Add(107, "2", "", 13);
		TblIdeology.Rows.Add(108, "2", "", 13);
		TblIdeology.Rows.Add(109, "2", "", 13);
		TblIdeology.Rows.Add(110, "2", "", 13);
		TblIdeology.Rows.Add(111, "2", "", 13);
		TblIdeology.Rows.Add(112, "2", "", 14);
		TblIdeology.Rows.Add(113, "2", "", 14);
		TblIdeology.Rows.Add(114, "2", "", 14);
		TblIdeology.Rows.Add(115, "2", "", 14);
		TblIdeology.Rows.Add(116, "2", "", 14);
		TblIdeology.Rows.Add(117, "2", "", 14);
		TblIdeology.Rows.Add(118, "2", "", 14);
		TblIdeology.Rows.Add(119, "2", "", 14);
		TblIdeology.Rows.Add(120, "2", "", 15);
		TblIdeology.Rows.Add(121, "2", "", 15);
		TblIdeology.Rows.Add(122, "2", "", 15);
		TblIdeology.Rows.Add(123, "2", "", 15);
		TblIdeology.Rows.Add(124, "2", "", 15);
		TblIdeology.Rows.Add(125, "2", "", 15);
		TblIdeology.Rows.Add(126, "2", "", 15);
		TblIdeology.Rows.Add(127, "2", "", 15);
		TblIdeology.Rows.Add(128, "2", "", 16);
		TblIdeology.Rows.Add(129, "2", "", 16);
		TblIdeology.Rows.Add(130, "2", "", 16);
		TblIdeology.Rows.Add(131, "2", "", 16);
		TblIdeology.Rows.Add(132, "2", "", 16);
		TblIdeology.Rows.Add(133, "2", "", 16);
		TblIdeology.Rows.Add(134, "2", "", 16);
		TblIdeology.Rows.Add(135, "2", "", 16);
		TblIdeology.Rows.Add(136, "2", "", 17);
		TblIdeology.Rows.Add(137, "2", "", 17);
		TblIdeology.Rows.Add(138, "2", "", 17);
		TblIdeology.Rows.Add(139, "2", "", 17);
		TblIdeology.Rows.Add(140, "2", "", 17);
		TblIdeology.Rows.Add(141, "2", "", 17);
		TblIdeology.Rows.Add(142, "2", "", 17);
		TblIdeology.Rows.Add(143, "2", "", 17);
		TblIdeology.Rows.Add(144, "0", "", 0);
		TblIdeology.Rows.Add(145, "2", "", 15);
		TblIdeology.Rows.Add(146, "0", "", 0);
		TblIdeology.Rows.Add(147, "0", "", 5);
		TblIdeology.Rows.Add(148, "0", "", 2);
		TblIdeology.Rows.Add(149, "2", "", 12);
		TblIdeology.Rows.Add(150, "1", "", 8);
		TblIdeology.Rows.Add(151, "1", "", 10);
		TblIdeology.Rows.Add(152, "0", "", 0);
		TblIdeology.Rows.Add(153, "1", "", 10);
		TblIdeology.Rows.Add(154, "2", "", 12);
		TblIdeology.Rows.Add(155, "1", "", 6);
	}

	private void CreateDefaultArray()
	{
		aArtClass = new string[6] { "Машина", "Сокровище", "Малый", "Большой", "Реликвия", "Реликвия-С" };
		aMine = new string[7] { "Древесина", "Ртуть", "Руда", "Сера", "Кристаллы", "Самоцветы", "Золото" };
		aLocality = new string[2] { "суша", "море" };
		aLevelSkill = new string[3] { "Б.", "П.", "Э." };
		aFullLvlSkill = new string[3] { "Базовый", "Продвинутый", "Экспертный" };
		aColor = new string[8] { "Красный", "Синий", "Коричневый", "Зеленый", "Оранжевый", "Пурпурный", "Бирюзовый", "Розовый" };
		aReply = new string[2] { "Нет", "Да" };
		aTown = new string[9] { "Замок", "Оплот", "Башня", "Инферно", "Некрополис", "Темница", "Цитадель", "Крепость", "Сопряжение" };
		aDoll = new string[20]
		{
			"Голова", "Плечи", "Шея", "Правая рука", "Левая рука", "Торс", "Правое кольцо", "Левое кольцо", "Ноги", "Разное1",
			"Разное2", "Разное3", "Разное4", "", "", "", "", "", "Разное5", "Рюкзак"
		};
		aPlace = new string[6] { "Тюрьма", "Таверна", "Запрещен", "Город", "Карта", "Лодка" };
		aTent = new string[8] { "Светло-голубая", "Зеленая", "Красная", "Темно-синяя", "Коричневая", "Пурпурная", "Белая", "Черная" };
		aStatus = new string[4] { "Нет", "Таймер", "Доступно", "Построено" };
		аQuest = new string[9] { "Достичь уровня", "Получить навык", "Победить монстра", "Принести артефакты", "Принести монстров", "Принести ресурсы", "Иметь цвет флага", "Победить героя", "Быть героем" };
		aReward = new string[9] { "Опыт", "Мана", "Мораль", "Удача", "Ресурс", "Навык", "Артефакт", "Заклинание", "Монстр" };
		aHeroClass = new string[18]
		{
			"Рыцарь", "Клирик", "Следопыт", "Друид", "Алхимик", "Колдун", "Демон", "Еретик", "Рыцарь смерти", "Некромант",
			"Лорд", "Чернокнижник", "Варвар", "Боевой маг", "Хозяин зверей", "Ведьма", "Странник", "Элементалист"
		};
		aIdeology = new string[3] { "Добро", "Зло", "Нейтрал" };
	}

	private void CreateTblArtComplete()
	{
		TblArtComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblArtComplete.Rows.Add("Волшебная книга");
		TblArtComplete.Rows.Add("Свиток заклинания");
		TblArtComplete.Rows.Add("Грааль");
		TblArtComplete.Rows.Add("Катапульта");
		TblArtComplete.Rows.Add("Баллиста");
		TblArtComplete.Rows.Add("Обоз");
		TblArtComplete.Rows.Add("Палатка первой помощи");
		TblArtComplete.Rows.Add("Топор кентавра");
		TblArtComplete.Rows.Add("Черный меч мертвого рыцаря");
		TblArtComplete.Rows.Add("Боевой цеп гнолла");
		TblArtComplete.Rows.Add("Огрова дубина разрушения");
		TblArtComplete.Rows.Add("Меч адского пламени");
		TblArtComplete.Rows.Add("Боевой меч титана");
		TblArtComplete.Rows.Add("Щит королей гномов");
		TblArtComplete.Rows.Add("Щит неупокоенных");
		TblArtComplete.Rows.Add("Щит короля гноллов");
		TblArtComplete.Rows.Add("Щит неистового огра");
		TblArtComplete.Rows.Add("Щит проклятых");
		TblArtComplete.Rows.Add("Щит часового");
		TblArtComplete.Rows.Add("Шлем белого единорога");
		TblArtComplete.Rows.Add("Шлем-череп");
		TblArtComplete.Rows.Add("Шлем Хаоса");
		TblArtComplete.Rows.Add("Корона верховного мага");
		TblArtComplete.Rows.Add("Шлем адской бури");
		TblArtComplete.Rows.Add("Громовой шлем");
		TblArtComplete.Rows.Add("Доспехи из окаменевшего древа");
		TblArtComplete.Rows.Add("Каркас");
		TblArtComplete.Rows.Add("Чешуя большого василиска");
		TblArtComplete.Rows.Add("Туника короля циклопов");
		TblArtComplete.Rows.Add("Доспехи самородной серы");
		TblArtComplete.Rows.Add("Кираса титана");
		TblArtComplete.Rows.Add("Дивный доспех");
		TblArtComplete.Rows.Add("Сандалии святого");
		TblArtComplete.Rows.Add("Ожерелье небесного блаженства");
		TblArtComplete.Rows.Add("Щит львиной храбрости");
		TblArtComplete.Rows.Add("Меч правосудия");
		TblArtComplete.Rows.Add("Шлем небесного просветления");
		TblArtComplete.Rows.Add("Безмолвный глаз дракона");
		TblArtComplete.Rows.Add("Огненный язык красного дракона");
		TblArtComplete.Rows.Add("Щит дракона");
		TblArtComplete.Rows.Add("Доспех черного дракона");
		TblArtComplete.Rows.Add("Поножи из кости дракона");
		TblArtComplete.Rows.Add("Плащ из крыла дракона");
		TblArtComplete.Rows.Add("Ожерелье из зубов дракона");
		TblArtComplete.Rows.Add("Корона дракона");
		TblArtComplete.Rows.Add("Неподвижный глаз дракона");
		TblArtComplete.Rows.Add("Клевер Фортуны");
		TblArtComplete.Rows.Add("Карты пророчества");
		TblArtComplete.Rows.Add("Птица счастья");
		TblArtComplete.Rows.Add("Знак мужества");
		TblArtComplete.Rows.Add("Крест отваги");
		TblArtComplete.Rows.Add("Глиф доблести");
		TblArtComplete.Rows.Add("Телескоп");
		TblArtComplete.Rows.Add("Подзорная труба");
		TblArtComplete.Rows.Add("Амулет некроманта");
		TblArtComplete.Rows.Add("Плащ вампира");
		TblArtComplete.Rows.Add("Башмаки мертвеца");
		TblArtComplete.Rows.Add("Наследный доспех");
		TblArtComplete.Rows.Add("Плащ равновесия");
		TblArtComplete.Rows.Add("Башмаки полярности");
		TblArtComplete.Rows.Add("Эльфийский лук из вишневого дерева");
		TblArtComplete.Rows.Add("Тетива из гривы единорога");
		TblArtComplete.Rows.Add("Стрелы с перьями ангела");
		TblArtComplete.Rows.Add("Птица проницательности");
		TblArtComplete.Rows.Add("Стойкий часовой");
		TblArtComplete.Rows.Add("Символ знания");
		TblArtComplete.Rows.Add("Медаль чиновника");
		TblArtComplete.Rows.Add("Кольцо дипломата");
		TblArtComplete.Rows.Add("Лента посла");
		TblArtComplete.Rows.Add("Кольцо странника");
		TblArtComplete.Rows.Add("Перчатки всадника");
		TblArtComplete.Rows.Add("Ожерелье навигатора");
		TblArtComplete.Rows.Add("Крылья ангела");
		TblArtComplete.Rows.Add("Амулет маны");
		TblArtComplete.Rows.Add("Талисман маны");
		TblArtComplete.Rows.Add("Волшебная сфера маны");
		TblArtComplete.Rows.Add("Колье заклинателя");
		TblArtComplete.Rows.Add("Кольцо заклинателя");
		TblArtComplete.Rows.Add("Накидка заклинателя");
		TblArtComplete.Rows.Add("Сфера небесного свода");
		TblArtComplete.Rows.Add("Сфера тверди земной");
		TblArtComplete.Rows.Add("Сфера буйного пламени");
		TblArtComplete.Rows.Add("Сфера проливного дождя");
		TblArtComplete.Rows.Add("Накидка отречения");
		TblArtComplete.Rows.Add("Дух угнетения");
		TblArtComplete.Rows.Add("Часы недоброго часа");
		TblArtComplete.Rows.Add("Книга магии Огня");
		TblArtComplete.Rows.Add("Книга магии Воздуха");
		TblArtComplete.Rows.Add("Книга магии Воды");
		TblArtComplete.Rows.Add("Книга магии Земли");
		TblArtComplete.Rows.Add("Башмаки левитации");
		TblArtComplete.Rows.Add("Золотой лук");
		TblArtComplete.Rows.Add("Сфера постоянства");
		TblArtComplete.Rows.Add("Сфера уязвимости");
		TblArtComplete.Rows.Add("Кольцо здоровья");
		TblArtComplete.Rows.Add("Кольцо жизни");
		TblArtComplete.Rows.Add("Сосуд с кровью жизни");
		TblArtComplete.Rows.Add("Ожерелье скорости");
		TblArtComplete.Rows.Add("Башмаки скороходы");
		TblArtComplete.Rows.Add("Накидка скорости");
		TblArtComplete.Rows.Add("Кулон бесстрастия");
		TblArtComplete.Rows.Add("Кулон внутреннего зрения");
		TblArtComplete.Rows.Add("Кулон святости");
		TblArtComplete.Rows.Add("Кулон жизни");
		TblArtComplete.Rows.Add("Кулон смерти");
		TblArtComplete.Rows.Add("Кулон свободной воли");
		TblArtComplete.Rows.Add("Кулон отрицания");
		TblArtComplete.Rows.Add("Кулон твердой памяти");
		TblArtComplete.Rows.Add("Кулон мужества");
		TblArtComplete.Rows.Add("Изобильная накидка кристаллов");
		TblArtComplete.Rows.Add("Нескончаемое кольцо самоцветов");
		TblArtComplete.Rows.Add("Бездонный сосуд ртути");
		TblArtComplete.Rows.Add("Неисчерпаемая вагонетка руды");
		TblArtComplete.Rows.Add("Неиссякаемое кольцо серы");
		TblArtComplete.Rows.Add("Бесконечная повозка дров");
		TblArtComplete.Rows.Add("Бездонный мешок золота");
		TblArtComplete.Rows.Add("Бездонная сума золота");
		TblArtComplete.Rows.Add("Бездонный кошель золота");
		TblArtComplete.Rows.Add("Ноги Легионера");
		TblArtComplete.Rows.Add("Поясница Легионера");
		TblArtComplete.Rows.Add("Торс Легионера");
		TblArtComplete.Rows.Add("Руки Легионера");
		TblArtComplete.Rows.Add("Голова Легионера");
		TblArtComplete.Rows.Add("Шляпа капитана");
		TblArtComplete.Rows.Add("Шляпа заклинателя");
		TblArtComplete.Rows.Add("Оковы войны");
		TblArtComplete.Rows.Add("Сфера подавления");
		TblArtComplete.Rows.Add("Фиал драконьей крови");
		TblArtComplete.Rows.Add("Клинок Армагеддона");
		TblArtComplete.Rows.Add("Ангельский союз");
		TblArtComplete.Rows.Add("Накидка Мертвого короля");
		TblArtComplete.Rows.Add("Фиал с кровью жизни");
		TblArtComplete.Rows.Add("Доспех проклятого");
		TblArtComplete.Rows.Add("Статуя Легионера");
		TblArtComplete.Rows.Add("Сила Отца драконов");
		TblArtComplete.Rows.Add("Гром титана");
		TblArtComplete.Rows.Add("Шляпа адмирала");
		TblArtComplete.Rows.Add("Лук снайпера");
		TblArtComplete.Rows.Add("Источник чародея");
		TblArtComplete.Rows.Add("Кольцо мага");
		TblArtComplete.Rows.Add("Рог изобилия");
		TblArtComplete.Rows.Add("Волшебная палочка");
		TblArtComplete.Rows.Add("Золотая стрела");
		TblArtComplete.Rows.Add("Сила монстра");
	}

	private void CreateTblArtSoDEn()
	{
		TblArtSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblArtSoDEn.Rows.Add("Spell Book");
		TblArtSoDEn.Rows.Add("Spell Scroll");
		TblArtSoDEn.Rows.Add("The Grail");
		TblArtSoDEn.Rows.Add("Catapult");
		TblArtSoDEn.Rows.Add("Ballista");
		TblArtSoDEn.Rows.Add("Ammo Cart");
		TblArtSoDEn.Rows.Add("First Aid Tent");
		TblArtSoDEn.Rows.Add("Centaurs Axe");
		TblArtSoDEn.Rows.Add("Blackshard of the Dead Knight");
		TblArtSoDEn.Rows.Add("Greater Gnoll's Flail");
		TblArtSoDEn.Rows.Add("Ogre's Club of Havoc");
		TblArtSoDEn.Rows.Add("Sword of Hellfire");
		TblArtSoDEn.Rows.Add("Titan's Gladius");
		TblArtSoDEn.Rows.Add("Shield of the Dwarven Lords");
		TblArtSoDEn.Rows.Add("Shield of the Yawning Dead");
		TblArtSoDEn.Rows.Add("Buckler of the Gnoll King");
		TblArtSoDEn.Rows.Add("Targ of the Rampaging Ogre");
		TblArtSoDEn.Rows.Add("Shield of the Damned");
		TblArtSoDEn.Rows.Add("Sentinel's Shield");
		TblArtSoDEn.Rows.Add("Helm of the Alabaster Unicorn");
		TblArtSoDEn.Rows.Add("Skull Helmet");
		TblArtSoDEn.Rows.Add("Helm of Chaos");
		TblArtSoDEn.Rows.Add("Crown of the Supreme Magi");
		TblArtSoDEn.Rows.Add("Hellstorm Helmet");
		TblArtSoDEn.Rows.Add("Thunder Helmet");
		TblArtSoDEn.Rows.Add("Breastplate of Petrified Wood");
		TblArtSoDEn.Rows.Add("Rib Cage");
		TblArtSoDEn.Rows.Add("Scales of the Greater Basilisk");
		TblArtSoDEn.Rows.Add("Tunic of the Cyclops King");
		TblArtSoDEn.Rows.Add("Breastplate of Brimstone");
		TblArtSoDEn.Rows.Add("Titan's Cuirass");
		TblArtSoDEn.Rows.Add("Armor of Wonder");
		TblArtSoDEn.Rows.Add("Sandals of the Saint");
		TblArtSoDEn.Rows.Add("Celestial Necklace of Bliss");
		TblArtSoDEn.Rows.Add("Lion's Shield of Courage");
		TblArtSoDEn.Rows.Add("Sword of Judgement");
		TblArtSoDEn.Rows.Add("Helm of Heavenly Enlightenment");
		TblArtSoDEn.Rows.Add("Quiet Eye of the Dragon");
		TblArtSoDEn.Rows.Add("Red Dragon Flame Tongue");
		TblArtSoDEn.Rows.Add("Dragon Scale Shield");
		TblArtSoDEn.Rows.Add("Dragon Scale Armor");
		TblArtSoDEn.Rows.Add("Dragonbone Greaves");
		TblArtSoDEn.Rows.Add("Dragon Wing Tabard");
		TblArtSoDEn.Rows.Add("Necklace of Dragonteeth");
		TblArtSoDEn.Rows.Add("Crown of Dragontooth");
		TblArtSoDEn.Rows.Add("Still Eye of the Dragon");
		TblArtSoDEn.Rows.Add("Clover of Fortune");
		TblArtSoDEn.Rows.Add("Cards of Prophecy");
		TblArtSoDEn.Rows.Add("Ladybird of Luck");
		TblArtSoDEn.Rows.Add("Badge of Courage");
		TblArtSoDEn.Rows.Add("Crest of Valor");
		TblArtSoDEn.Rows.Add("Glyph of Gallantry");
		TblArtSoDEn.Rows.Add("Speculum");
		TblArtSoDEn.Rows.Add("Spyglass");
		TblArtSoDEn.Rows.Add("Amulet of the Undertaker");
		TblArtSoDEn.Rows.Add("Vampire's Cowl");
		TblArtSoDEn.Rows.Add("Dead Man's Boots");
		TblArtSoDEn.Rows.Add("Garniture of Interference");
		TblArtSoDEn.Rows.Add("Surcoat of Counterpoise");
		TblArtSoDEn.Rows.Add("Boots of Polarity");
		TblArtSoDEn.Rows.Add("Bow of Elven Cherrywood");
		TblArtSoDEn.Rows.Add("Bowstring of the Unicorn's Mane");
		TblArtSoDEn.Rows.Add("Angel Feather Arrows");
		TblArtSoDEn.Rows.Add("Bird of Perception");
		TblArtSoDEn.Rows.Add("Stoic Watchman");
		TblArtSoDEn.Rows.Add("Emblem of Cognizance");
		TblArtSoDEn.Rows.Add("Statesman's Medal");
		TblArtSoDEn.Rows.Add("Diplomat's Ring");
		TblArtSoDEn.Rows.Add("Ambassador's Sash");
		TblArtSoDEn.Rows.Add("Ring of the Wayfarer");
		TblArtSoDEn.Rows.Add("Equestrian's Gloves");
		TblArtSoDEn.Rows.Add("Necklace of Ocean Guidance");
		TblArtSoDEn.Rows.Add("Angel Wings");
		TblArtSoDEn.Rows.Add("Charm of Mana");
		TblArtSoDEn.Rows.Add("Talisman of Mana");
		TblArtSoDEn.Rows.Add("Mystic Orb of Mana");
		TblArtSoDEn.Rows.Add("Collar of Conjuring");
		TblArtSoDEn.Rows.Add("Ring of Conjuring");
		TblArtSoDEn.Rows.Add("Cape of Conjuring");
		TblArtSoDEn.Rows.Add("Orb of the Firmament");
		TblArtSoDEn.Rows.Add("Orb of Silt");
		TblArtSoDEn.Rows.Add("Orb of Tempestuous Fire");
		TblArtSoDEn.Rows.Add("Orb of Driving Rain");
		TblArtSoDEn.Rows.Add("Recanter's Cloak");
		TblArtSoDEn.Rows.Add("Spirit of Oppression");
		TblArtSoDEn.Rows.Add("Hourglass of the Evil Hour");
		TblArtSoDEn.Rows.Add("Tome of Fire Magic");
		TblArtSoDEn.Rows.Add("Tome of Air Magic");
		TblArtSoDEn.Rows.Add("Tome of Water Magic");
		TblArtSoDEn.Rows.Add("Tome of Earth Magic");
		TblArtSoDEn.Rows.Add("Boots of Levitation");
		TblArtSoDEn.Rows.Add("Golden Bow");
		TblArtSoDEn.Rows.Add("Sphere of Permanence");
		TblArtSoDEn.Rows.Add("Orb of Vulnerability");
		TblArtSoDEn.Rows.Add("Ring of Vitality");
		TblArtSoDEn.Rows.Add("Ring of Life");
		TblArtSoDEn.Rows.Add("Vial of Lifeblood");
		TblArtSoDEn.Rows.Add("Necklace of Swiftness");
		TblArtSoDEn.Rows.Add("Boots of Speed");
		TblArtSoDEn.Rows.Add("Cape of Velocity");
		TblArtSoDEn.Rows.Add("Pendant of Dispassion");
		TblArtSoDEn.Rows.Add("Pendant of Second Sight");
		TblArtSoDEn.Rows.Add("Pendant of Holiness");
		TblArtSoDEn.Rows.Add("Pendant of Life");
		TblArtSoDEn.Rows.Add("Pendant of Death");
		TblArtSoDEn.Rows.Add("Pendant of Free Will");
		TblArtSoDEn.Rows.Add("Pendant of Negativity");
		TblArtSoDEn.Rows.Add("Pendant of Total Recall");
		TblArtSoDEn.Rows.Add("Pendant of Courage");
		TblArtSoDEn.Rows.Add("Everflowing Crystal Cloak");
		TblArtSoDEn.Rows.Add("Ring of Infinite Gems");
		TblArtSoDEn.Rows.Add("Everpouring Vial of Mercury");
		TblArtSoDEn.Rows.Add("Inexhaustible Cart of Ore");
		TblArtSoDEn.Rows.Add("Eversmoking Ring of Sulfur");
		TblArtSoDEn.Rows.Add("Inexhaustible Cart of Lumber");
		TblArtSoDEn.Rows.Add("Endless Sack of Gold");
		TblArtSoDEn.Rows.Add("Endless Bag of Gold");
		TblArtSoDEn.Rows.Add("Endless Purse of Gold");
		TblArtSoDEn.Rows.Add("Legs of Legion");
		TblArtSoDEn.Rows.Add("Loins of Legion");
		TblArtSoDEn.Rows.Add("Torso of Legion");
		TblArtSoDEn.Rows.Add("Arms of Legion");
		TblArtSoDEn.Rows.Add("Head of Legion");
		TblArtSoDEn.Rows.Add("Sea Captain's Hat");
		TblArtSoDEn.Rows.Add("Spellbinder's Hat");
		TblArtSoDEn.Rows.Add("Shackles of War");
		TblArtSoDEn.Rows.Add("Orb of Inhibition");
		TblArtSoDEn.Rows.Add("Vial of Dragon Blood");
		TblArtSoDEn.Rows.Add("Armageddon's Blade");
		TblArtSoDEn.Rows.Add("Angelic Alliance");
		TblArtSoDEn.Rows.Add("Cloak of the Undead King");
		TblArtSoDEn.Rows.Add("Elixir of Life");
		TblArtSoDEn.Rows.Add("Armor of the Damned");
		TblArtSoDEn.Rows.Add("Statue of Legion");
		TblArtSoDEn.Rows.Add("Power of the Dragon Father");
		TblArtSoDEn.Rows.Add("Titan's Thunder");
		TblArtSoDEn.Rows.Add("Admiral's Hat");
		TblArtSoDEn.Rows.Add("Bow of the Sharpshooter");
		TblArtSoDEn.Rows.Add("Wizard's Well");
		TblArtSoDEn.Rows.Add("Ring of the Magi");
		TblArtSoDEn.Rows.Add("Cornucopia");
		TblArtSoDEn.Rows.Add("Magic Stick");
		TblArtSoDEn.Rows.Add("Golden Arrow");
		TblArtSoDEn.Rows.Add("Monster Strength");
	}

	private void CreateTblArtSoDRu()
	{
		TblArtSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblArtSoDRu.Rows.Add("Книга Заклинаний");
		TblArtSoDRu.Rows.Add("Свиток с Заклинаниями");
		TblArtSoDRu.Rows.Add("Грааль");
		TblArtSoDRu.Rows.Add("Катапульта");
		TblArtSoDRu.Rows.Add("Баллиста");
		TblArtSoDRu.Rows.Add("Подвода с Боеприпасами");
		TblArtSoDRu.Rows.Add("Санитарная Палатка");
		TblArtSoDRu.Rows.Add("Секира Кентавра");
		TblArtSoDRu.Rows.Add("Блэкшард Мертвого Рыцаря");
		TblArtSoDRu.Rows.Add("Великий Гномий Кистень");
		TblArtSoDRu.Rows.Add("Карающая Дубина Орга");
		TblArtSoDRu.Rows.Add("Адский Меч");
		TblArtSoDRu.Rows.Add("Гладиус Титана");
		TblArtSoDRu.Rows.Add("Щит Гномьих Богов");
		TblArtSoDRu.Rows.Add("Щит Тоскующих Мертвецов");
		TblArtSoDRu.Rows.Add("Щит Короля Гноллов");
		TblArtSoDRu.Rows.Add("Щит Яростного Орга");
		TblArtSoDRu.Rows.Add("Щит Проклятых");
		TblArtSoDRu.Rows.Add("Щит Часового");
		TblArtSoDRu.Rows.Add("Шлем Белого Единорога");
		TblArtSoDRu.Rows.Add("Шлем-Череп");
		TblArtSoDRu.Rows.Add("Шлем Хаоса");
		TblArtSoDRu.Rows.Add("Корона Главного Мага");
		TblArtSoDRu.Rows.Add("Шлем Сатанинской Ярости ");
		TblArtSoDRu.Rows.Add("Шлем Небесного Грома");
		TblArtSoDRu.Rows.Add("Нагрудник из Окаменелого Дерева");
		TblArtSoDRu.Rows.Add("Ребра");
		TblArtSoDRu.Rows.Add("Кольчуга Великого Василиска");
		TblArtSoDRu.Rows.Add("Туника Короля Циклопов");
		TblArtSoDRu.Rows.Add("Нагрудник из Серного Камня");
		TblArtSoDRu.Rows.Add("Латы Титана");
		TblArtSoDRu.Rows.Add("Магические Доспехи");
		TblArtSoDRu.Rows.Add("Сандалии Святых");
		TblArtSoDRu.Rows.Add("Ожерелье Божественной Благодати");
		TblArtSoDRu.Rows.Add("Щит Львиной Храбрости");
		TblArtSoDRu.Rows.Add("Меч Правосудия");
		TblArtSoDRu.Rows.Add("Шлем Божественного Просвещения");
		TblArtSoDRu.Rows.Add("Неподвижный Глаз Дракона");
		TblArtSoDRu.Rows.Add("Языки Пламени Красного Дракона");
		TblArtSoDRu.Rows.Add("Щит из Чешуи Дракона");
		TblArtSoDRu.Rows.Add("Доспехи из Чешуи Дракона");
		TblArtSoDRu.Rows.Add("Наколенники из Драконьей Кости");
		TblArtSoDRu.Rows.Add("Плащ из Драконьих Крыльев");
		TblArtSoDRu.Rows.Add("Ожерелье из Зубов Дракона");
		TblArtSoDRu.Rows.Add("Корона из Зубов Дракона");
		TblArtSoDRu.Rows.Add("Застывшей Глаз Дракона");
		TblArtSoDRu.Rows.Add("Клевер Удачи");
		TblArtSoDRu.Rows.Add("Карты Пророчества");
		TblArtSoDRu.Rows.Add("Голубка Удачи");
		TblArtSoDRu.Rows.Add("Значок Смелости");
		TblArtSoDRu.Rows.Add("Герб Доблести");
		TblArtSoDRu.Rows.Add("Знак Отваги");
		TblArtSoDRu.Rows.Add("Зеркало");
		TblArtSoDRu.Rows.Add("Подзорная Труба");
		TblArtSoDRu.Rows.Add("Амулет Гробовщика");
		TblArtSoDRu.Rows.Add("Мантия Вампира");
		TblArtSoDRu.Rows.Add("Сапоги Мертвеца");
		TblArtSoDRu.Rows.Add("Колье Неприступности");
		TblArtSoDRu.Rows.Add("Мантия Равновесия");
		TblArtSoDRu.Rows.Add("Сапоги Противодействия");
		TblArtSoDRu.Rows.Add("Лук из Вишневого Дерева Эльфов");
		TblArtSoDRu.Rows.Add("Тетива из Волоса Гривы Единорога");
		TblArtSoDRu.Rows.Add("Стрелы из Ангельских Перьев");
		TblArtSoDRu.Rows.Add("Птица Познания");
		TblArtSoDRu.Rows.Add("Бесстрашный Хранитель");
		TblArtSoDRu.Rows.Add("Символ Знаний");
		TblArtSoDRu.Rows.Add("Медаль Дипломата");
		TblArtSoDRu.Rows.Add("Кольцо Дипломата");
		TblArtSoDRu.Rows.Add("Лента Посла");
		TblArtSoDRu.Rows.Add("Кольцо Странника");
		TblArtSoDRu.Rows.Add("Перчатки Всадника");
		TblArtSoDRu.Rows.Add("Ожерелье Морского Проведения");
		TblArtSoDRu.Rows.Add("Крылья Ангела");
		TblArtSoDRu.Rows.Add("Амулет Маны");
		TblArtSoDRu.Rows.Add("Талисман Маны");
		TblArtSoDRu.Rows.Add("Магическая Медаль Маны");
		TblArtSoDRu.Rows.Add("Магический Ошейник");
		TblArtSoDRu.Rows.Add("Магическое Кольцо");
		TblArtSoDRu.Rows.Add("Магическая Накидка");
		TblArtSoDRu.Rows.Add("Сфера Небесного Свода");
		TblArtSoDRu.Rows.Add("Сфера Илистого Озера");
		TblArtSoDRu.Rows.Add("Сфера Бушующего Огня");
		TblArtSoDRu.Rows.Add("Сфера Проливного Дождя");
		TblArtSoDRu.Rows.Add("Плащ Отречения");
		TblArtSoDRu.Rows.Add("Дух Уныния");
		TblArtSoDRu.Rows.Add("Песочные Часы Недоброго Часа");
		TblArtSoDRu.Rows.Add("Книга Магии Огня");
		TblArtSoDRu.Rows.Add("Книга Магии Воздуха");
		TblArtSoDRu.Rows.Add("Книга Магии Воды");
		TblArtSoDRu.Rows.Add("Книга Магии Земли");
		TblArtSoDRu.Rows.Add("Сапоги Левитации");
		TblArtSoDRu.Rows.Add("Золотой Лук");
		TblArtSoDRu.Rows.Add("Шар Постоянства");
		TblArtSoDRu.Rows.Add("Медаль Уязвимости");
		TblArtSoDRu.Rows.Add("Кольцо Жизненной Силы");
		TblArtSoDRu.Rows.Add("Кольцо Жизни");
		TblArtSoDRu.Rows.Add("Склянка Жизненной Силы");
		TblArtSoDRu.Rows.Add("Ожерелье Стремительности");
		TblArtSoDRu.Rows.Add("Сапоги-Скороходы");
		TblArtSoDRu.Rows.Add("Накидка Скорости");
		TblArtSoDRu.Rows.Add("Брелок Бесстрастия");
		TblArtSoDRu.Rows.Add("Брелок Ясновидения");
		TblArtSoDRu.Rows.Add("Священный Брелок");
		TblArtSoDRu.Rows.Add("Брелок Жизни");
		TblArtSoDRu.Rows.Add("Брелок Смерти");
		TblArtSoDRu.Rows.Add("Брелок Свободы");
		TblArtSoDRu.Rows.Add("Брелок Отрицательности");
		TblArtSoDRu.Rows.Add("Брелок Абсолютной Памяти");
		TblArtSoDRu.Rows.Add("Брелок Смелости");
		TblArtSoDRu.Rows.Add("Плащ Бесконечных Кристаллов");
		TblArtSoDRu.Rows.Add("Кольцо Драгоценных Камней");
		TblArtSoDRu.Rows.Add("Неиссякаемая Склянка Ртути");
		TblArtSoDRu.Rows.Add("Неистощимая Подвода с Рудой");
		TblArtSoDRu.Rows.Add("Вечное Кольцо Серы");
		TblArtSoDRu.Rows.Add("Неистощимая Подвода Леса");
		TblArtSoDRu.Rows.Add("Неиссякаемый Мешок Золота");
		TblArtSoDRu.Rows.Add("Неиссякаемая Сума Золота");
		TblArtSoDRu.Rows.Add("Неиссякаемая Мошна Золота");
		TblArtSoDRu.Rows.Add("Ноги Легиона");
		TblArtSoDRu.Rows.Add("Поясница Легиона");
		TblArtSoDRu.Rows.Add("Туловище Легиона");
		TblArtSoDRu.Rows.Add("Руки Легиона");
		TblArtSoDRu.Rows.Add("Голова Легиона");
		TblArtSoDRu.Rows.Add("Шляпа Морского Капитана");
		TblArtSoDRu.Rows.Add("Шляпа Оратора");
		TblArtSoDRu.Rows.Add("Оковы Войны");
		TblArtSoDRu.Rows.Add("Сфера Запрещения");
		TblArtSoDRu.Rows.Add("Пузырек с Кровью Дракона");
		TblArtSoDRu.Rows.Add("Клинок Армагеддона");
		TblArtSoDRu.Rows.Add("Альянс Ангелов");
		TblArtSoDRu.Rows.Add("Плащ Короля Нечисти");
		TblArtSoDRu.Rows.Add("Эликсир Жизни");
		TblArtSoDRu.Rows.Add("Доспехи Проклятого");
		TblArtSoDRu.Rows.Add("Статуя Легиона");
		TblArtSoDRu.Rows.Add("Мощь отца Драконов");
		TblArtSoDRu.Rows.Add("Грохот Титана");
		TblArtSoDRu.Rows.Add("Шляпа Адмирала");
		TblArtSoDRu.Rows.Add("Лук Снайпера");
		TblArtSoDRu.Rows.Add("Колодец Волшебника");
		TblArtSoDRu.Rows.Add("Кольцо Мага");
		TblArtSoDRu.Rows.Add("Рог изобилия");
		TblArtSoDRu.Rows.Add("Волшебная палочка");
		TblArtSoDRu.Rows.Add("Золотая стрела");
		TblArtSoDRu.Rows.Add("Сила монстра");
	}

	private void CreateTblSpellComplete()
	{
		TblSpellComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblSpellComplete.Rows.Add("Призвать корабль");
		TblSpellComplete.Rows.Add("Затопить корабль");
		TblSpellComplete.Rows.Add("Виденье");
		TblSpellComplete.Rows.Add("Земное око");
		TblSpellComplete.Rows.Add("Маскировка");
		TblSpellComplete.Rows.Add("Небесное око");
		TblSpellComplete.Rows.Add("Полет");
		TblSpellComplete.Rows.Add("Движение по воде");
		TblSpellComplete.Rows.Add("Пространственные врата");
		TblSpellComplete.Rows.Add("Портал города");
		TblSpellComplete.Rows.Add("Зыбучие пески");
		TblSpellComplete.Rows.Add("Мины");
		TblSpellComplete.Rows.Add("Силовое поле");
		TblSpellComplete.Rows.Add("Стена огня");
		TblSpellComplete.Rows.Add("Землетрясение");
		TblSpellComplete.Rows.Add("Волшебная стрела");
		TblSpellComplete.Rows.Add("Ледяная стрела");
		TblSpellComplete.Rows.Add("Молния");
		TblSpellComplete.Rows.Add("Удушение");
		TblSpellComplete.Rows.Add("Цепь молний");
		TblSpellComplete.Rows.Add("Кольцо холода");
		TblSpellComplete.Rows.Add("Огненный шар");
		TblSpellComplete.Rows.Add("Инферно");
		TblSpellComplete.Rows.Add("Звездопад");
		TblSpellComplete.Rows.Add("Дрожь смерти");
		TblSpellComplete.Rows.Add("Уничтожить нежить");
		TblSpellComplete.Rows.Add("Армагеддон");
		TblSpellComplete.Rows.Add("Щит");
		TblSpellComplete.Rows.Add("Воздушный щит");
		TblSpellComplete.Rows.Add("Огненный щит");
		TblSpellComplete.Rows.Add("Оберег Воздуха");
		TblSpellComplete.Rows.Add("Оберег Огня");
		TblSpellComplete.Rows.Add("Оберег Воды");
		TblSpellComplete.Rows.Add("Оберег Земли");
		TblSpellComplete.Rows.Add("Антимагия");
		TblSpellComplete.Rows.Add("Снять чары");
		TblSpellComplete.Rows.Add("Волшебное зеркало");
		TblSpellComplete.Rows.Add("Лечение");
		TblSpellComplete.Rows.Add("Воскрешение");
		TblSpellComplete.Rows.Add("Поднять мертвецов");
		TblSpellComplete.Rows.Add("Жертвоприношение");
		TblSpellComplete.Rows.Add("Благословение");
		TblSpellComplete.Rows.Add("Проклятие");
		TblSpellComplete.Rows.Add("Жажда крови");
		TblSpellComplete.Rows.Add("Меткость");
		TblSpellComplete.Rows.Add("Слабость");
		TblSpellComplete.Rows.Add("Каменная кожа");
		TblSpellComplete.Rows.Add("Разрушительный луч");
		TblSpellComplete.Rows.Add("Молитва");
		TblSpellComplete.Rows.Add("Радость");
		TblSpellComplete.Rows.Add("Печаль");
		TblSpellComplete.Rows.Add("Фортуна");
		TblSpellComplete.Rows.Add("Неудача");
		TblSpellComplete.Rows.Add("Ускорение");
		TblSpellComplete.Rows.Add("Замедление");
		TblSpellComplete.Rows.Add("Убийца");
		TblSpellComplete.Rows.Add("Бешенство");
		TblSpellComplete.Rows.Add("Молния титана");
		TblSpellComplete.Rows.Add("Ответный удар");
		TblSpellComplete.Rows.Add("Берсерк");
		TblSpellComplete.Rows.Add("Гипноз");
		TblSpellComplete.Rows.Add("Забывчивость");
		TblSpellComplete.Rows.Add("Ослепление");
		TblSpellComplete.Rows.Add("Телепортация");
		TblSpellComplete.Rows.Add("Убрать препятствие");
		TblSpellComplete.Rows.Add("Фантом");
		TblSpellComplete.Rows.Add("Огненный элементал");
		TblSpellComplete.Rows.Add("Земной элементал");
		TblSpellComplete.Rows.Add("Водный элементал");
		TblSpellComplete.Rows.Add("Воздушный элементал");
		TblSpellComplete.Rows.Add("Взгляд, превращяющий в камень");
	}

	private void CreateTblSpellSoDEn()
	{
		TblSpellSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblSpellSoDEn.Rows.Add("Summon Boat");
		TblSpellSoDEn.Rows.Add("Scuttle Boat");
		TblSpellSoDEn.Rows.Add("Visions");
		TblSpellSoDEn.Rows.Add("View Earth");
		TblSpellSoDEn.Rows.Add("Disguise");
		TblSpellSoDEn.Rows.Add("View Air");
		TblSpellSoDEn.Rows.Add("Fly");
		TblSpellSoDEn.Rows.Add("Water Walk");
		TblSpellSoDEn.Rows.Add("Dimension Door");
		TblSpellSoDEn.Rows.Add("Town Portal");
		TblSpellSoDEn.Rows.Add("Quicksand");
		TblSpellSoDEn.Rows.Add("Land Mine");
		TblSpellSoDEn.Rows.Add("Force Field");
		TblSpellSoDEn.Rows.Add("Fire Wall");
		TblSpellSoDEn.Rows.Add("Earthquake");
		TblSpellSoDEn.Rows.Add("Magic Arrow");
		TblSpellSoDEn.Rows.Add("Ice Bolt");
		TblSpellSoDEn.Rows.Add("Lightning Bolt");
		TblSpellSoDEn.Rows.Add("Implosion");
		TblSpellSoDEn.Rows.Add("Chain Lightning");
		TblSpellSoDEn.Rows.Add("Frost Ring");
		TblSpellSoDEn.Rows.Add("Fireball");
		TblSpellSoDEn.Rows.Add("Inferno");
		TblSpellSoDEn.Rows.Add("Meteor Shower");
		TblSpellSoDEn.Rows.Add("Death Ripple");
		TblSpellSoDEn.Rows.Add("Destroy Undead");
		TblSpellSoDEn.Rows.Add("Armageddon");
		TblSpellSoDEn.Rows.Add("Shield");
		TblSpellSoDEn.Rows.Add("Air Shield");
		TblSpellSoDEn.Rows.Add("Fire Shield");
		TblSpellSoDEn.Rows.Add("Protection from Air");
		TblSpellSoDEn.Rows.Add("Protection from Fire");
		TblSpellSoDEn.Rows.Add("Protection from Water");
		TblSpellSoDEn.Rows.Add("Protection from Earth");
		TblSpellSoDEn.Rows.Add("Anti-Magic");
		TblSpellSoDEn.Rows.Add("Dispel");
		TblSpellSoDEn.Rows.Add("Magic Mirror");
		TblSpellSoDEn.Rows.Add("Cure");
		TblSpellSoDEn.Rows.Add("Resurrection");
		TblSpellSoDEn.Rows.Add("Animate Dead");
		TblSpellSoDEn.Rows.Add("Sacrifice");
		TblSpellSoDEn.Rows.Add("Bless");
		TblSpellSoDEn.Rows.Add("Curse");
		TblSpellSoDEn.Rows.Add("Bloodlust");
		TblSpellSoDEn.Rows.Add("Precision");
		TblSpellSoDEn.Rows.Add("Weakness");
		TblSpellSoDEn.Rows.Add("Stone Skin");
		TblSpellSoDEn.Rows.Add("Disrupting Ray");
		TblSpellSoDEn.Rows.Add("Prayer");
		TblSpellSoDEn.Rows.Add("Mirth");
		TblSpellSoDEn.Rows.Add("Sorrow");
		TblSpellSoDEn.Rows.Add("Fortune");
		TblSpellSoDEn.Rows.Add("Misfortune");
		TblSpellSoDEn.Rows.Add("Haste");
		TblSpellSoDEn.Rows.Add("Slow");
		TblSpellSoDEn.Rows.Add("Slayer");
		TblSpellSoDEn.Rows.Add("Frenzy");
		TblSpellSoDEn.Rows.Add("Titan's Lightning Bolt");
		TblSpellSoDEn.Rows.Add("Counterstrike");
		TblSpellSoDEn.Rows.Add("Berserk");
		TblSpellSoDEn.Rows.Add("Hypnotize");
		TblSpellSoDEn.Rows.Add("Forgetfulness");
		TblSpellSoDEn.Rows.Add("Blind");
		TblSpellSoDEn.Rows.Add("Teleport");
		TblSpellSoDEn.Rows.Add("Remove Obstacle");
		TblSpellSoDEn.Rows.Add("Clone");
		TblSpellSoDEn.Rows.Add("Fire Elemental");
		TblSpellSoDEn.Rows.Add("Earth Elemental");
		TblSpellSoDEn.Rows.Add("Water Elemental");
		TblSpellSoDEn.Rows.Add("Air Elemental");
		TblSpellSoDEn.Rows.Add("Stone Gaze");
	}

	private void CreateTblSpellSoDRu()
	{
		TblSpellSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblSpellSoDRu.Rows.Add("Вызвать Корабль");
		TblSpellSoDRu.Rows.Add("Затопить корабль");
		TblSpellSoDRu.Rows.Add("Видения");
		TblSpellSoDRu.Rows.Add("Просмотр Земли");
		TblSpellSoDRu.Rows.Add("Маскировка");
		TblSpellSoDRu.Rows.Add("Просмотр Воздуха");
		TblSpellSoDRu.Rows.Add("Полет");
		TblSpellSoDRu.Rows.Add("Хождение по воде");
		TblSpellSoDRu.Rows.Add("Дверь измерений");
		TblSpellSoDRu.Rows.Add("Городской портал");
		TblSpellSoDRu.Rows.Add("Зыбучие пески");
		TblSpellSoDRu.Rows.Add("Минное поле");
		TblSpellSoDRu.Rows.Add("Силовое поле");
		TblSpellSoDRu.Rows.Add("Стена Огня");
		TblSpellSoDRu.Rows.Add("Землетрясение");
		TblSpellSoDRu.Rows.Add("Волшебная стрела");
		TblSpellSoDRu.Rows.Add("Ледяная молния");
		TblSpellSoDRu.Rows.Add("Удар молнии");
		TblSpellSoDRu.Rows.Add("Взрыв");
		TblSpellSoDRu.Rows.Add("Цепная молния");
		TblSpellSoDRu.Rows.Add("Кольцо холода");
		TblSpellSoDRu.Rows.Add("Огненный Шар");
		TblSpellSoDRu.Rows.Add("Инферно");
		TblSpellSoDRu.Rows.Add("Метеоритный Дождь");
		TblSpellSoDRu.Rows.Add("Волна Смерти");
		TblSpellSoDRu.Rows.Add("Уничтожить Нечисть");
		TblSpellSoDRu.Rows.Add("Армагеддон");
		TblSpellSoDRu.Rows.Add("Щит");
		TblSpellSoDRu.Rows.Add("Воздушный Щит");
		TblSpellSoDRu.Rows.Add("Огненный щит");
		TblSpellSoDRu.Rows.Add("Защита от воздуха");
		TblSpellSoDRu.Rows.Add("Защита от огня");
		TblSpellSoDRu.Rows.Add("Защита от воды");
		TblSpellSoDRu.Rows.Add("Защита от земли");
		TblSpellSoDRu.Rows.Add("Анти-Магия");
		TblSpellSoDRu.Rows.Add("Снятие Заклинаний");
		TblSpellSoDRu.Rows.Add("Волшебное зеркало");
		TblSpellSoDRu.Rows.Add("Лечение");
		TblSpellSoDRu.Rows.Add("Восстановление");
		TblSpellSoDRu.Rows.Add("Оживление мертвецов");
		TblSpellSoDRu.Rows.Add("Жертва");
		TblSpellSoDRu.Rows.Add("Благословление");
		TblSpellSoDRu.Rows.Add("Проклятье");
		TblSpellSoDRu.Rows.Add("Жажда крови");
		TblSpellSoDRu.Rows.Add("Точность");
		TblSpellSoDRu.Rows.Add("Слабость");
		TblSpellSoDRu.Rows.Add("Каменная кожа");
		TblSpellSoDRu.Rows.Add("Разрушающий луч");
		TblSpellSoDRu.Rows.Add("Молитва");
		TblSpellSoDRu.Rows.Add("Радость");
		TblSpellSoDRu.Rows.Add("Печаль");
		TblSpellSoDRu.Rows.Add("Удача");
		TblSpellSoDRu.Rows.Add("Неудача");
		TblSpellSoDRu.Rows.Add("Ускорение");
		TblSpellSoDRu.Rows.Add("Медлительность");
		TblSpellSoDRu.Rows.Add("Палач");
		TblSpellSoDRu.Rows.Add("Бешенство");
		TblSpellSoDRu.Rows.Add("Гром Титанов");
		TblSpellSoDRu.Rows.Add("Контрудар");
		TblSpellSoDRu.Rows.Add("Берсерк");
		TblSpellSoDRu.Rows.Add("Гипноз");
		TblSpellSoDRu.Rows.Add("Забывчивость");
		TblSpellSoDRu.Rows.Add("Слепота");
		TblSpellSoDRu.Rows.Add("Телепорт");
		TblSpellSoDRu.Rows.Add("Устранение преград");
		TblSpellSoDRu.Rows.Add("Клон");
		TblSpellSoDRu.Rows.Add("Огенный Элементаль");
		TblSpellSoDRu.Rows.Add("Земляной Элементаль");
		TblSpellSoDRu.Rows.Add("Водный Элементаль");
		TblSpellSoDRu.Rows.Add("Воздушный Элементаль");
		TblSpellSoDRu.Rows.Add("Взгляд, превращяющий в камень");
	}

	private void CreateTblMonsterComplete()
	{
		TblMonstersComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblMonstersComplete.Rows.Add("Копейщик");
		TblMonstersComplete.Rows.Add("Алебардщик");
		TblMonstersComplete.Rows.Add("Стрелок");
		TblMonstersComplete.Rows.Add("Лучник");
		TblMonstersComplete.Rows.Add("Грифон");
		TblMonstersComplete.Rows.Add("Королевский грифон");
		TblMonstersComplete.Rows.Add("Мечник");
		TblMonstersComplete.Rows.Add("Крестоносец");
		TblMonstersComplete.Rows.Add("Монах");
		TblMonstersComplete.Rows.Add("Фанатик");
		TblMonstersComplete.Rows.Add("Всадник");
		TblMonstersComplete.Rows.Add("Чемпион");
		TblMonstersComplete.Rows.Add("Ангел");
		TblMonstersComplete.Rows.Add("Архангел");
		TblMonstersComplete.Rows.Add("Кентавр");
		TblMonstersComplete.Rows.Add("Капитан кентавров");
		TblMonstersComplete.Rows.Add("Гном");
		TblMonstersComplete.Rows.Add("Боевой гном");
		TblMonstersComplete.Rows.Add("Лесной эльф");
		TblMonstersComplete.Rows.Add("Высокий эльф");
		TblMonstersComplete.Rows.Add("Пегас");
		TblMonstersComplete.Rows.Add("Серебряный пегас");
		TblMonstersComplete.Rows.Add("Дендроид страж");
		TblMonstersComplete.Rows.Add("Дендроид воин");
		TblMonstersComplete.Rows.Add("Единорог");
		TblMonstersComplete.Rows.Add("Боевой единорог");
		TblMonstersComplete.Rows.Add("Зеленый дракон");
		TblMonstersComplete.Rows.Add("Золотой дракон");
		TblMonstersComplete.Rows.Add("Гремлин");
		TblMonstersComplete.Rows.Add("Мастер гремлин");
		TblMonstersComplete.Rows.Add("Каменная горгулья");
		TblMonstersComplete.Rows.Add("Обсидиановая горгулья");
		TblMonstersComplete.Rows.Add("Каменный голем");
		TblMonstersComplete.Rows.Add("Железный голем");
		TblMonstersComplete.Rows.Add("Маг");
		TblMonstersComplete.Rows.Add("Архимаг");
		TblMonstersComplete.Rows.Add("Джинн");
		TblMonstersComplete.Rows.Add("Владыка джиннов");
		TblMonstersComplete.Rows.Add("Нага");
		TblMonstersComplete.Rows.Add("Королева наг");
		TblMonstersComplete.Rows.Add("Гигант");
		TblMonstersComplete.Rows.Add("Титан");
		TblMonstersComplete.Rows.Add("Бес");
		TblMonstersComplete.Rows.Add("Черт");
		TblMonstersComplete.Rows.Add("Гог");
		TblMonstersComplete.Rows.Add("Магог");
		TblMonstersComplete.Rows.Add("Гончая ада");
		TblMonstersComplete.Rows.Add("Цербер");
		TblMonstersComplete.Rows.Add("Демон");
		TblMonstersComplete.Rows.Add("Рогатый демон");
		TblMonstersComplete.Rows.Add("Демон бездны");
		TblMonstersComplete.Rows.Add("Владыка бездны");
		TblMonstersComplete.Rows.Add("Ифрит");
		TblMonstersComplete.Rows.Add("Султан ифритов");
		TblMonstersComplete.Rows.Add("Дьявол");
		TblMonstersComplete.Rows.Add("Архидьявол");
		TblMonstersComplete.Rows.Add("Скелет");
		TblMonstersComplete.Rows.Add("Скелет воин");
		TblMonstersComplete.Rows.Add("Ходячий мертвец");
		TblMonstersComplete.Rows.Add("Зомби");
		TblMonstersComplete.Rows.Add("Призрак");
		TblMonstersComplete.Rows.Add("Привидение");
		TblMonstersComplete.Rows.Add("Вампир");
		TblMonstersComplete.Rows.Add("Лорд вампиров");
		TblMonstersComplete.Rows.Add("Лич");
		TblMonstersComplete.Rows.Add("Могучий лич");
		TblMonstersComplete.Rows.Add("Черный рыцарь");
		TblMonstersComplete.Rows.Add("Зловещий рыцарь");
		TblMonstersComplete.Rows.Add("Костяной дракон");
		TblMonstersComplete.Rows.Add("Призрачный дракон");
		TblMonstersComplete.Rows.Add("Троглодит");
		TblMonstersComplete.Rows.Add("Адский троглодит");
		TblMonstersComplete.Rows.Add("Гарпия");
		TblMonstersComplete.Rows.Add("Гарпия ведьма");
		TblMonstersComplete.Rows.Add("Бехолдер");
		TblMonstersComplete.Rows.Add("Злобоглаз");
		TblMonstersComplete.Rows.Add("Медуза");
		TblMonstersComplete.Rows.Add("Королева медуз");
		TblMonstersComplete.Rows.Add("Минотавр");
		TblMonstersComplete.Rows.Add("Королевский минотавров");
		TblMonstersComplete.Rows.Add("Мантикора");
		TblMonstersComplete.Rows.Add("Скорпикора");
		TblMonstersComplete.Rows.Add("Красный дракон");
		TblMonstersComplete.Rows.Add("Черный дракон");
		TblMonstersComplete.Rows.Add("Гоблин");
		TblMonstersComplete.Rows.Add("Хобгоблин");
		TblMonstersComplete.Rows.Add("Наездник на волке");
		TblMonstersComplete.Rows.Add("Разбойник на волке");
		TblMonstersComplete.Rows.Add("Орк");
		TblMonstersComplete.Rows.Add("Вождь орков");
		TblMonstersComplete.Rows.Add("Огр");
		TblMonstersComplete.Rows.Add("Огр-маг");
		TblMonstersComplete.Rows.Add("Рух");
		TblMonstersComplete.Rows.Add("Громовая птица");
		TblMonstersComplete.Rows.Add("Циклоп");
		TblMonstersComplete.Rows.Add("Королевский циклоп");
		TblMonstersComplete.Rows.Add("Чудище");
		TblMonstersComplete.Rows.Add("Древнее чудище");
		TblMonstersComplete.Rows.Add("Гнолл");
		TblMonstersComplete.Rows.Add("Гнолл мародер");
		TblMonstersComplete.Rows.Add("Ящер");
		TblMonstersComplete.Rows.Add("Ящер воин");
		TblMonstersComplete.Rows.Add("Горгон");
		TblMonstersComplete.Rows.Add("Могучий горгон");
		TblMonstersComplete.Rows.Add("Летучий змей");
		TblMonstersComplete.Rows.Add("Летучий змий");
		TblMonstersComplete.Rows.Add("Василиск");
		TblMonstersComplete.Rows.Add("Великий василиск");
		TblMonstersComplete.Rows.Add("Виверна");
		TblMonstersComplete.Rows.Add("Виверна-монарх");
		TblMonstersComplete.Rows.Add("Гидра");
		TblMonstersComplete.Rows.Add("Гидра хаоса");
		TblMonstersComplete.Rows.Add("Воздушный элементал");
		TblMonstersComplete.Rows.Add("Земной элементал");
		TblMonstersComplete.Rows.Add("Огненный элементал");
		TblMonstersComplete.Rows.Add("Водный элементал");
		TblMonstersComplete.Rows.Add("Золотой голем");
		TblMonstersComplete.Rows.Add("Алмазный голем");
		TblMonstersComplete.Rows.Add("Пикси");
		TblMonstersComplete.Rows.Add("Фея");
		TblMonstersComplete.Rows.Add("Элементал мысли");
		TblMonstersComplete.Rows.Add("Волшебный элементал");
		TblMonstersComplete.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersComplete.Rows.Add("Ледяной элементал");
		TblMonstersComplete.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersComplete.Rows.Add("Элементал магмы");
		TblMonstersComplete.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersComplete.Rows.Add("Штормовой элементал");
		TblMonstersComplete.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersComplete.Rows.Add("Ракшас");
		TblMonstersComplete.Rows.Add("Жар-птица");
		TblMonstersComplete.Rows.Add("Феникс");
		TblMonstersComplete.Rows.Add("Лазурный дракон");
		TblMonstersComplete.Rows.Add("Кристальный дракон");
		TblMonstersComplete.Rows.Add("Сказочный дракон");
		TblMonstersComplete.Rows.Add("Ржавый дракон");
		TblMonstersComplete.Rows.Add("Колдун");
		TblMonstersComplete.Rows.Add("Снайпер");
		TblMonstersComplete.Rows.Add("Полурослик");
		TblMonstersComplete.Rows.Add("Крестьянин");
		TblMonstersComplete.Rows.Add("Боров");
		TblMonstersComplete.Rows.Add("Мумия");
		TblMonstersComplete.Rows.Add("Кочевник");
		TblMonstersComplete.Rows.Add("Разбойник");
		TblMonstersComplete.Rows.Add("Тролль");
		TblMonstersComplete.Rows.Add("Катапульта");
		TblMonstersComplete.Rows.Add("Баллиста");
		TblMonstersComplete.Rows.Add("Палатка первой помощи");
		TblMonstersComplete.Rows.Add("Обоз");
	}

	private void CreateTblMonsterSoDEn()
	{
		TblMonstersSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblMonstersSoDEn.Rows.Add("Pikeman");
		TblMonstersSoDEn.Rows.Add("Halberdier");
		TblMonstersSoDEn.Rows.Add("Archer");
		TblMonstersSoDEn.Rows.Add("Marksman");
		TblMonstersSoDEn.Rows.Add("Griffin");
		TblMonstersSoDEn.Rows.Add("Royal Griffin");
		TblMonstersSoDEn.Rows.Add("Swordsman");
		TblMonstersSoDEn.Rows.Add("Crusader");
		TblMonstersSoDEn.Rows.Add("Monk");
		TblMonstersSoDEn.Rows.Add("Zealot");
		TblMonstersSoDEn.Rows.Add("Cavalier");
		TblMonstersSoDEn.Rows.Add("Champion");
		TblMonstersSoDEn.Rows.Add("Angel");
		TblMonstersSoDEn.Rows.Add("Archangel");
		TblMonstersSoDEn.Rows.Add("Centaur");
		TblMonstersSoDEn.Rows.Add("Centaur Captain");
		TblMonstersSoDEn.Rows.Add("Dwarf");
		TblMonstersSoDEn.Rows.Add("Battle Dwarf");
		TblMonstersSoDEn.Rows.Add("Wood Elf");
		TblMonstersSoDEn.Rows.Add("Grand Elf");
		TblMonstersSoDEn.Rows.Add("Pegasus");
		TblMonstersSoDEn.Rows.Add("Silver Pegasus");
		TblMonstersSoDEn.Rows.Add("Dendroid Guard");
		TblMonstersSoDEn.Rows.Add("Dendroid Soldier");
		TblMonstersSoDEn.Rows.Add("Unicorn");
		TblMonstersSoDEn.Rows.Add("War Unicorn");
		TblMonstersSoDEn.Rows.Add("Green Dragon");
		TblMonstersSoDEn.Rows.Add("Gold Dragon");
		TblMonstersSoDEn.Rows.Add("Gremlin");
		TblMonstersSoDEn.Rows.Add("Master Gremlin");
		TblMonstersSoDEn.Rows.Add("Stone Gargoyle");
		TblMonstersSoDEn.Rows.Add("Obsidian Gargoyle");
		TblMonstersSoDEn.Rows.Add("Stone Golem");
		TblMonstersSoDEn.Rows.Add("Iron Golem");
		TblMonstersSoDEn.Rows.Add("Mage");
		TblMonstersSoDEn.Rows.Add("Arch Mage");
		TblMonstersSoDEn.Rows.Add("Genie");
		TblMonstersSoDEn.Rows.Add("Master Genie");
		TblMonstersSoDEn.Rows.Add("Naga");
		TblMonstersSoDEn.Rows.Add("Naga Queen");
		TblMonstersSoDEn.Rows.Add("Giant");
		TblMonstersSoDEn.Rows.Add("Titan");
		TblMonstersSoDEn.Rows.Add("Imp");
		TblMonstersSoDEn.Rows.Add("Familiar");
		TblMonstersSoDEn.Rows.Add("Gog");
		TblMonstersSoDEn.Rows.Add("Magog");
		TblMonstersSoDEn.Rows.Add("Hell Hound");
		TblMonstersSoDEn.Rows.Add("Cerberus");
		TblMonstersSoDEn.Rows.Add("Demon");
		TblMonstersSoDEn.Rows.Add("Horned Demon");
		TblMonstersSoDEn.Rows.Add("Pit Fiend");
		TblMonstersSoDEn.Rows.Add("Pit Lord");
		TblMonstersSoDEn.Rows.Add("Efreeti");
		TblMonstersSoDEn.Rows.Add("Efreet Sultan");
		TblMonstersSoDEn.Rows.Add("Devil");
		TblMonstersSoDEn.Rows.Add("Arch Devil");
		TblMonstersSoDEn.Rows.Add("Skeleton");
		TblMonstersSoDEn.Rows.Add("Skeleton Warrior");
		TblMonstersSoDEn.Rows.Add("Walking Dead");
		TblMonstersSoDEn.Rows.Add("Zombie");
		TblMonstersSoDEn.Rows.Add("Wight");
		TblMonstersSoDEn.Rows.Add("Wraith");
		TblMonstersSoDEn.Rows.Add("Vampire");
		TblMonstersSoDEn.Rows.Add("Vampire Lord");
		TblMonstersSoDEn.Rows.Add("Lich");
		TblMonstersSoDEn.Rows.Add("Power Lich");
		TblMonstersSoDEn.Rows.Add("Black Knight");
		TblMonstersSoDEn.Rows.Add("Dread Knight");
		TblMonstersSoDEn.Rows.Add("Bone Dragon");
		TblMonstersSoDEn.Rows.Add("Ghost Dragon");
		TblMonstersSoDEn.Rows.Add("Troglodyte");
		TblMonstersSoDEn.Rows.Add("Infernal Troglodyte");
		TblMonstersSoDEn.Rows.Add("Harpy");
		TblMonstersSoDEn.Rows.Add("Harpy Hag");
		TblMonstersSoDEn.Rows.Add("Beholder");
		TblMonstersSoDEn.Rows.Add("Evil Eye");
		TblMonstersSoDEn.Rows.Add("Medusa");
		TblMonstersSoDEn.Rows.Add("Medusa Queen");
		TblMonstersSoDEn.Rows.Add("Minotaur");
		TblMonstersSoDEn.Rows.Add("Minotaur King");
		TblMonstersSoDEn.Rows.Add("Manticore");
		TblMonstersSoDEn.Rows.Add("Scorpicore");
		TblMonstersSoDEn.Rows.Add("Red Dragon");
		TblMonstersSoDEn.Rows.Add("Black Dragon");
		TblMonstersSoDEn.Rows.Add("Goblin");
		TblMonstersSoDEn.Rows.Add("Hobgoblin");
		TblMonstersSoDEn.Rows.Add("Wolf Rider");
		TblMonstersSoDEn.Rows.Add("Wolf Raider");
		TblMonstersSoDEn.Rows.Add("Orc");
		TblMonstersSoDEn.Rows.Add("Orc Chieftain");
		TblMonstersSoDEn.Rows.Add("Ogre");
		TblMonstersSoDEn.Rows.Add("Ogre Mage");
		TblMonstersSoDEn.Rows.Add("Roc");
		TblMonstersSoDEn.Rows.Add("Thunderbird");
		TblMonstersSoDEn.Rows.Add("Cyclops");
		TblMonstersSoDEn.Rows.Add("Cyclops King");
		TblMonstersSoDEn.Rows.Add("Behemoth");
		TblMonstersSoDEn.Rows.Add("Ancient Behemoth");
		TblMonstersSoDEn.Rows.Add("Gnoll");
		TblMonstersSoDEn.Rows.Add("Gnoll Marauder");
		TblMonstersSoDEn.Rows.Add("Lizardman");
		TblMonstersSoDEn.Rows.Add("Lizard Warrior");
		TblMonstersSoDEn.Rows.Add("Gorgon");
		TblMonstersSoDEn.Rows.Add("Mighty Gorgon");
		TblMonstersSoDEn.Rows.Add("Serpent Fly");
		TblMonstersSoDEn.Rows.Add("Dragon Fly");
		TblMonstersSoDEn.Rows.Add("Basilisk");
		TblMonstersSoDEn.Rows.Add("Greater Basilisk");
		TblMonstersSoDEn.Rows.Add("Wyvern");
		TblMonstersSoDEn.Rows.Add("Wyvern Monarch");
		TblMonstersSoDEn.Rows.Add("Hydra");
		TblMonstersSoDEn.Rows.Add("Chaos Hydra");
		TblMonstersSoDEn.Rows.Add("Air Elemental");
		TblMonstersSoDEn.Rows.Add("Earth Elemental");
		TblMonstersSoDEn.Rows.Add("Fire Elemental");
		TblMonstersSoDEn.Rows.Add("Water Elemental");
		TblMonstersSoDEn.Rows.Add("Gold Golem");
		TblMonstersSoDEn.Rows.Add("Diamond Golem");
		TblMonstersSoDEn.Rows.Add("Pixie");
		TblMonstersSoDEn.Rows.Add("Sprite");
		TblMonstersSoDEn.Rows.Add("Psychic Elemental");
		TblMonstersSoDEn.Rows.Add("Magic Elemental");
		TblMonstersSoDEn.Rows.Add("NOT USED");
		TblMonstersSoDEn.Rows.Add("Ice Elemental");
		TblMonstersSoDEn.Rows.Add("NOT USED");
		TblMonstersSoDEn.Rows.Add("Magma Elemental");
		TblMonstersSoDEn.Rows.Add("NOT USED");
		TblMonstersSoDEn.Rows.Add("Storm Elemental");
		TblMonstersSoDEn.Rows.Add("NOT USED");
		TblMonstersSoDEn.Rows.Add("Energy Elemental");
		TblMonstersSoDEn.Rows.Add("Firebird");
		TblMonstersSoDEn.Rows.Add("Phoenix");
		TblMonstersSoDEn.Rows.Add("Azure Dragon");
		TblMonstersSoDEn.Rows.Add("Crystal Dragon");
		TblMonstersSoDEn.Rows.Add("Faerie Dragon");
		TblMonstersSoDEn.Rows.Add("Rust Dragon");
		TblMonstersSoDEn.Rows.Add("Enchanter");
		TblMonstersSoDEn.Rows.Add("Sharpshooter");
		TblMonstersSoDEn.Rows.Add("Halfling");
		TblMonstersSoDEn.Rows.Add("Peasant");
		TblMonstersSoDEn.Rows.Add("Boar");
		TblMonstersSoDEn.Rows.Add("Mummy");
		TblMonstersSoDEn.Rows.Add("Nomad");
		TblMonstersSoDEn.Rows.Add("Rogue");
		TblMonstersSoDEn.Rows.Add("Troll");
		TblMonstersSoDEn.Rows.Add("Catapult");
		TblMonstersSoDEn.Rows.Add("Ballista");
		TblMonstersSoDEn.Rows.Add("First Aid Tent");
		TblMonstersSoDEn.Rows.Add("Ammo Cart");
	}

	private void CreateTblMonsterSoDRu()
	{
		TblMonstersSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblMonstersSoDRu.Rows.Add("Копейщик");
		TblMonstersSoDRu.Rows.Add("Алебардщик");
		TblMonstersSoDRu.Rows.Add("Лучник");
		TblMonstersSoDRu.Rows.Add("Стрелок");
		TblMonstersSoDRu.Rows.Add("Грифон");
		TblMonstersSoDRu.Rows.Add("Королевский Грифон");
		TblMonstersSoDRu.Rows.Add("Рыцарь");
		TblMonstersSoDRu.Rows.Add("Крестоносец");
		TblMonstersSoDRu.Rows.Add("Монах");
		TblMonstersSoDRu.Rows.Add("Фанатик");
		TblMonstersSoDRu.Rows.Add("Кавалерист");
		TblMonstersSoDRu.Rows.Add("Чемпион");
		TblMonstersSoDRu.Rows.Add("Ангел");
		TblMonstersSoDRu.Rows.Add("Архангел");
		TblMonstersSoDRu.Rows.Add("Кентавр");
		TblMonstersSoDRu.Rows.Add("Капитан Кентавров");
		TblMonstersSoDRu.Rows.Add("Гном");
		TblMonstersSoDRu.Rows.Add("Боевой Гном");
		TblMonstersSoDRu.Rows.Add("Лесной Ельф");
		TblMonstersSoDRu.Rows.Add("Великий Эльф");
		TblMonstersSoDRu.Rows.Add("Пегас");
		TblMonstersSoDRu.Rows.Add("Серебрянный Пегас");
		TblMonstersSoDRu.Rows.Add("Дендроид Охранник");
		TblMonstersSoDRu.Rows.Add("Дендроид Солдат");
		TblMonstersSoDRu.Rows.Add("Единорог");
		TblMonstersSoDRu.Rows.Add("Боевой Единорог");
		TblMonstersSoDRu.Rows.Add("Зеленый Дракон");
		TblMonstersSoDRu.Rows.Add("Золотой Дракон");
		TblMonstersSoDRu.Rows.Add("Гремли");
		TblMonstersSoDRu.Rows.Add("Мастер-Гремлин");
		TblMonstersSoDRu.Rows.Add("Каменная Горгулья");
		TblMonstersSoDRu.Rows.Add("Обсидиановая Горгулья");
		TblMonstersSoDRu.Rows.Add("Каменный Голем");
		TblMonstersSoDRu.Rows.Add("Стальной Голем");
		TblMonstersSoDRu.Rows.Add("Маг");
		TblMonstersSoDRu.Rows.Add("Архи-Маг");
		TblMonstersSoDRu.Rows.Add("Джин");
		TblMonstersSoDRu.Rows.Add("Мастер-Джин");
		TblMonstersSoDRu.Rows.Add("Нага");
		TblMonstersSoDRu.Rows.Add("Королева Нага");
		TblMonstersSoDRu.Rows.Add("Гигант");
		TblMonstersSoDRu.Rows.Add("Титан");
		TblMonstersSoDRu.Rows.Add("Бес");
		TblMonstersSoDRu.Rows.Add("Чёрт");
		TblMonstersSoDRu.Rows.Add("Гога");
		TblMonstersSoDRu.Rows.Add("Магога");
		TblMonstersSoDRu.Rows.Add("Адская Гончая");
		TblMonstersSoDRu.Rows.Add("Цербер");
		TblMonstersSoDRu.Rows.Add("Демон");
		TblMonstersSoDRu.Rows.Add("Рогатый Демон");
		TblMonstersSoDRu.Rows.Add("Порождение Зла");
		TblMonstersSoDRu.Rows.Add("Адское Отродье");
		TblMonstersSoDRu.Rows.Add("Эфрит");
		TblMonstersSoDRu.Rows.Add("Эфрит-Султан");
		TblMonstersSoDRu.Rows.Add("Дьявол");
		TblMonstersSoDRu.Rows.Add("Архидьявол");
		TblMonstersSoDRu.Rows.Add("Скелет");
		TblMonstersSoDRu.Rows.Add("Воин Скелет");
		TblMonstersSoDRu.Rows.Add("Живой Мертвец");
		TblMonstersSoDRu.Rows.Add("Зомби");
		TblMonstersSoDRu.Rows.Add("Страж");
		TblMonstersSoDRu.Rows.Add("Привидение");
		TblMonstersSoDRu.Rows.Add("Вампир");
		TblMonstersSoDRu.Rows.Add("Вампир Лорд");
		TblMonstersSoDRu.Rows.Add("Лич");
		TblMonstersSoDRu.Rows.Add("Могущественный Лич");
		TblMonstersSoDRu.Rows.Add("Черный Рыцарь");
		TblMonstersSoDRu.Rows.Add("Рыцарь Смерти");
		TblMonstersSoDRu.Rows.Add("Костяной Дракон");
		TblMonstersSoDRu.Rows.Add("Дракон-Привидение");
		TblMonstersSoDRu.Rows.Add("Троглодит");
		TblMonstersSoDRu.Rows.Add("Адский Троглодит");
		TblMonstersSoDRu.Rows.Add("Гарпия");
		TblMonstersSoDRu.Rows.Add("Гарпия-Ведьма");
		TblMonstersSoDRu.Rows.Add("Созерцатель");
		TblMonstersSoDRu.Rows.Add("Дурной Глаз");
		TblMonstersSoDRu.Rows.Add("Медуза");
		TblMonstersSoDRu.Rows.Add("Королева Медуза");
		TblMonstersSoDRu.Rows.Add("Минотавр");
		TblMonstersSoDRu.Rows.Add("Король Минотавр");
		TblMonstersSoDRu.Rows.Add("Мантикора");
		TblMonstersSoDRu.Rows.Add("Скорпикора");
		TblMonstersSoDRu.Rows.Add("Красный Дракон");
		TblMonstersSoDRu.Rows.Add("Черный Дракон");
		TblMonstersSoDRu.Rows.Add("Гоблин");
		TblMonstersSoDRu.Rows.Add("Хобгоблин");
		TblMonstersSoDRu.Rows.Add("Наездник на волках");
		TblMonstersSoDRu.Rows.Add("Налетчик");
		TblMonstersSoDRu.Rows.Add("Орк");
		TblMonstersSoDRu.Rows.Add("Орк-Вождь");
		TblMonstersSoDRu.Rows.Add("Людоед");
		TblMonstersSoDRu.Rows.Add("Людоед Маг");
		TblMonstersSoDRu.Rows.Add("Птица Рух");
		TblMonstersSoDRu.Rows.Add("Птица Грома");
		TblMonstersSoDRu.Rows.Add("Циклоп");
		TblMonstersSoDRu.Rows.Add("Король Циклопов");
		TblMonstersSoDRu.Rows.Add("Чудище");
		TblMonstersSoDRu.Rows.Add("Древнее Чудище");
		TblMonstersSoDRu.Rows.Add("Гнолл");
		TblMonstersSoDRu.Rows.Add("Гнолл-Мародер");
		TblMonstersSoDRu.Rows.Add("Ящер");
		TblMonstersSoDRu.Rows.Add("Ящер-Воин");
		TblMonstersSoDRu.Rows.Add("Горгона");
		TblMonstersSoDRu.Rows.Add("Могучая Горгона");
		TblMonstersSoDRu.Rows.Add("Змей");
		TblMonstersSoDRu.Rows.Add("Стрекоза");
		TblMonstersSoDRu.Rows.Add("Василиск");
		TblMonstersSoDRu.Rows.Add("Великий Василиск");
		TblMonstersSoDRu.Rows.Add("Виверн");
		TblMonstersSoDRu.Rows.Add("Виверн-Монарх");
		TblMonstersSoDRu.Rows.Add("Гидра");
		TblMonstersSoDRu.Rows.Add("Гидра Хаоса");
		TblMonstersSoDRu.Rows.Add("Воздушный Элементаль");
		TblMonstersSoDRu.Rows.Add("Элементаль Земли");
		TblMonstersSoDRu.Rows.Add("Огненный Элементаль");
		TblMonstersSoDRu.Rows.Add("Элементаль Воды");
		TblMonstersSoDRu.Rows.Add("Золотой Голем");
		TblMonstersSoDRu.Rows.Add("Алмазный Голем");
		TblMonstersSoDRu.Rows.Add("Маленькая Фея");
		TblMonstersSoDRu.Rows.Add("Фея");
		TblMonstersSoDRu.Rows.Add("Психический Элементаль");
		TblMonstersSoDRu.Rows.Add("Магический Элементаль");
		TblMonstersSoDRu.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersSoDRu.Rows.Add("Ледяной Элементаль");
		TblMonstersSoDRu.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersSoDRu.Rows.Add("Элементаль Магмы");
		TblMonstersSoDRu.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersSoDRu.Rows.Add("Элементаль Шторма");
		TblMonstersSoDRu.Rows.Add("НЕ ИСПОЛЬЗУЕТСЯ");
		TblMonstersSoDRu.Rows.Add("Энергетический Элементаль");
		TblMonstersSoDRu.Rows.Add("Огненная Птица");
		TblMonstersSoDRu.Rows.Add("Феникс");
		TblMonstersSoDRu.Rows.Add("Лазурный Дракон");
		TblMonstersSoDRu.Rows.Add("Кристальный Дракон");
		TblMonstersSoDRu.Rows.Add("Сказочный Дракон");
		TblMonstersSoDRu.Rows.Add("Ржавый Дракон");
		TblMonstersSoDRu.Rows.Add("Чародей");
		TblMonstersSoDRu.Rows.Add("Снайпер");
		TblMonstersSoDRu.Rows.Add("Хоббит");
		TblMonstersSoDRu.Rows.Add("Крестьянин");
		TblMonstersSoDRu.Rows.Add("Кабан");
		TblMonstersSoDRu.Rows.Add("Мумия");
		TblMonstersSoDRu.Rows.Add("Кочевник");
		TblMonstersSoDRu.Rows.Add("Вор");
		TblMonstersSoDRu.Rows.Add("Тролль");
		TblMonstersSoDRu.Rows.Add("Катапульта");
		TblMonstersSoDRu.Rows.Add("Баллиста");
		TblMonstersSoDRu.Rows.Add("Палатка Первой Помощи");
		TblMonstersSoDRu.Rows.Add("Тележка с Боеприпасами");
	}

	private void CreateTblObjectComplete()
	{
		TblObjectComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblObjectComplete.Rows.Add("Жертвенник");
		TblObjectComplete.Rows.Add("Артефакт");
		TblObjectComplete.Rows.Add("Ящик Пандоры");
		TblObjectComplete.Rows.Add("Черный рынок");
		TblObjectComplete.Rows.Add("Шатры хранителя ключей");
		TblObjectComplete.Rows.Add("Костер");
		TblObjectComplete.Rows.Add("Хранилище циклопов");
		TblObjectComplete.Rows.Add("Сокровищница гномов");
		TblObjectComplete.Rows.Add("Консерватория грифонов");
		TblObjectComplete.Rows.Add("Яма бесов");
		TblObjectComplete.Rows.Add("Склады медуз");
		TblObjectComplete.Rows.Add("Хранилище наг");
		TblObjectComplete.Rows.Add("Улей летучих змиев");
		TblObjectComplete.Rows.Add("Скелет");
		TblObjectComplete.Rows.Add("Заброшенный корабль");
		TblObjectComplete.Rows.Add("Утопия драконов");
		TblObjectComplete.Rows.Add("Событие");
		TblObjectComplete.Rows.Add("Обломки");
		TblObjectComplete.Rows.Add("Гарнизон");
		TblObjectComplete.Rows.Add("Герой");
		TblObjectComplete.Rows.Add("Форт на холме");
		TblObjectComplete.Rows.Add("Навес");
		TblObjectComplete.Rows.Add("Портал входа");
		TblObjectComplete.Rows.Add("Портал выхода");
		TblObjectComplete.Rows.Add("Портал");
		TblObjectComplete.Rows.Add("Шахта");
		TblObjectComplete.Rows.Add("Монстр");
		TblObjectComplete.Rows.Add("Таинственный сад");
		TblObjectComplete.Rows.Add("Тюрьма");
		TblObjectComplete.Rows.Add("Пирамида");
		TblObjectComplete.Rows.Add("Лагерь беженцев");
		TblObjectComplete.Rows.Add("Ресурс");
		TblObjectComplete.Rows.Add("Ученый книжник");
		TblObjectComplete.Rows.Add("Сундук");
		TblObjectComplete.Rows.Add("Хижина предсказателя");
		TblObjectComplete.Rows.Add("Склеп");
		TblObjectComplete.Rows.Add("Разбившийся корабль");
		TblObjectComplete.Rows.Add("Потерпевший кораблекрушение");
		TblObjectComplete.Rows.Add("Святилище магических песнопений");
		TblObjectComplete.Rows.Add("Святилище магических жестов");
		TblObjectComplete.Rows.Add("Святилище магических мыслей");
		TblObjectComplete.Rows.Add("Волшебный свиток");
		TblObjectComplete.Rows.Add("Таверна");
		TblObjectComplete.Rows.Add("Город");
		TblObjectComplete.Rows.Add("Камень учености");
		TblObjectComplete.Rows.Add("Ларец с сокровищами");
		TblObjectComplete.Rows.Add("Древо познания");
		TblObjectComplete.Rows.Add("Подземные врата");
		TblObjectComplete.Rows.Add("Университет");
		TblObjectComplete.Rows.Add("Повозка");
		TblObjectComplete.Rows.Add("Гробница воина");
		TblObjectComplete.Rows.Add("Водоворот");
		TblObjectComplete.Rows.Add("Ветряная мельница");
		TblObjectComplete.Rows.Add("Хижина ведьмы");
		TblObjectComplete.Rows.Add("Гильдия наемников");
		TblObjectComplete.Rows.Add("Страж задания");
		TblObjectComplete.Rows.Add("Торговцы Артефактами");
	}

	private void CreateTblObjectSoDEn()
	{
		TblObjectSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblObjectSoDEn.Rows.Add("Altar of Sacrifice");
		TblObjectSoDEn.Rows.Add("Artifact");
		TblObjectSoDEn.Rows.Add("Pandora's Box");
		TblObjectSoDEn.Rows.Add("Black Market");
		TblObjectSoDEn.Rows.Add("Keymaster's Tent");
		TblObjectSoDEn.Rows.Add("Campfire");
		TblObjectSoDEn.Rows.Add("Cyclops Stockpile");
		TblObjectSoDEn.Rows.Add("Dwarven Treasury");
		TblObjectSoDEn.Rows.Add("Griffin Conservatory");
		TblObjectSoDEn.Rows.Add("Imp Cache");
		TblObjectSoDEn.Rows.Add("Medusa Stores");
		TblObjectSoDEn.Rows.Add("Naga Bank");
		TblObjectSoDEn.Rows.Add("Dragon Fly Hive");
		TblObjectSoDEn.Rows.Add("Corpse");
		TblObjectSoDEn.Rows.Add("Derelict Ship");
		TblObjectSoDEn.Rows.Add("Dragon Utopia");
		TblObjectSoDEn.Rows.Add("Event");
		TblObjectSoDEn.Rows.Add("Flotsam");
		TblObjectSoDEn.Rows.Add("Garrison");
		TblObjectSoDEn.Rows.Add("Hero");
		TblObjectSoDEn.Rows.Add("Hill Fort");
		TblObjectSoDEn.Rows.Add("Lean To");
		TblObjectSoDEn.Rows.Add("Monolith One Way Entrance");
		TblObjectSoDEn.Rows.Add("Monolith One Way Exit");
		TblObjectSoDEn.Rows.Add("Monolith Two Way");
		TblObjectSoDEn.Rows.Add("Mine");
		TblObjectSoDEn.Rows.Add("Monster");
		TblObjectSoDEn.Rows.Add("Mystical Garden");
		TblObjectSoDEn.Rows.Add("Prison");
		TblObjectSoDEn.Rows.Add("Pyramid");
		TblObjectSoDEn.Rows.Add("Refugee Camp");
		TblObjectSoDEn.Rows.Add("Resource");
		TblObjectSoDEn.Rows.Add("Scholar");
		TblObjectSoDEn.Rows.Add("Sea Chest");
		TblObjectSoDEn.Rows.Add("Seer's Hut");
		TblObjectSoDEn.Rows.Add("Crypt");
		TblObjectSoDEn.Rows.Add("Shipwreck");
		TblObjectSoDEn.Rows.Add("Shipwreck Survivor");
		TblObjectSoDEn.Rows.Add("Shrine of Magic Incantation");
		TblObjectSoDEn.Rows.Add("Shrine of Magic Gesture");
		TblObjectSoDEn.Rows.Add("Shrine of Magic Thought");
		TblObjectSoDEn.Rows.Add("Spell Scroll");
		TblObjectSoDEn.Rows.Add("Tavern");
		TblObjectSoDEn.Rows.Add("Town");
		TblObjectSoDEn.Rows.Add("Learning Stone");
		TblObjectSoDEn.Rows.Add("Treasure Chest");
		TblObjectSoDEn.Rows.Add("Tree of Knowledge");
		TblObjectSoDEn.Rows.Add("Subterranean Gate");
		TblObjectSoDEn.Rows.Add("University");
		TblObjectSoDEn.Rows.Add("Wagon");
		TblObjectSoDEn.Rows.Add("Warrior's Tomb");
		TblObjectSoDEn.Rows.Add("Whirlpool");
		TblObjectSoDEn.Rows.Add("Windmill");
		TblObjectSoDEn.Rows.Add("Witch Hut");
		TblObjectSoDEn.Rows.Add("Freelancer's Guild");
		TblObjectSoDEn.Rows.Add("Quest Guard");
		TblObjectSoDEn.Rows.Add("Artifact Merchants");
	}

	private void CreateTblObjectSoDRu()
	{
		TblObjectSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblObjectSoDRu.Rows.Add("Алтарь Жертвоприношиния");
		TblObjectSoDRu.Rows.Add("Артефакт");
		TblObjectSoDRu.Rows.Add("Ящик Пандоры");
		TblObjectSoDRu.Rows.Add("Черный Рынок");
		TblObjectSoDRu.Rows.Add("Палатка Ключника");
		TblObjectSoDRu.Rows.Add("Бивачный Костер");
		TblObjectSoDRu.Rows.Add("Склады Циклопов");
		TblObjectSoDRu.Rows.Add("Сокровищница Гномов");
		TblObjectSoDRu.Rows.Add("Консерватория Грифонов");
		TblObjectSoDRu.Rows.Add("Тайник Бесов");
		TblObjectSoDRu.Rows.Add("Хранилище Медуз");
		TblObjectSoDRu.Rows.Add("Нага Банк");
		TblObjectSoDRu.Rows.Add("Улей Змиев");
		TblObjectSoDRu.Rows.Add("Труп");
		TblObjectSoDRu.Rows.Add("Ветхий Корабль");
		TblObjectSoDRu.Rows.Add("Утопия Драконов");
		TblObjectSoDRu.Rows.Add("Событие");
		TblObjectSoDRu.Rows.Add("Обломки");
		TblObjectSoDRu.Rows.Add("Гарнизон");
		TblObjectSoDRu.Rows.Add("Герой");
		TblObjectSoDRu.Rows.Add("Форт на холме");
		TblObjectSoDRu.Rows.Add("Прислониться к");
		TblObjectSoDRu.Rows.Add("Монолит Входа");
		TblObjectSoDRu.Rows.Add("Монолит Выхода");
		TblObjectSoDRu.Rows.Add("Двухсторонний Монолит");
		TblObjectSoDRu.Rows.Add("Шахта");
		TblObjectSoDRu.Rows.Add("Монстр");
		TblObjectSoDRu.Rows.Add("Мистический сад");
		TblObjectSoDRu.Rows.Add("Тюрьма");
		TblObjectSoDRu.Rows.Add("Пирамида");
		TblObjectSoDRu.Rows.Add("Лагерь Беженцов");
		TblObjectSoDRu.Rows.Add("Ресурс");
		TblObjectSoDRu.Rows.Add("Ученый");
		TblObjectSoDRu.Rows.Add("Морской сундук");
		TblObjectSoDRu.Rows.Add("Хижина Провидца");
		TblObjectSoDRu.Rows.Add("Склеп");
		TblObjectSoDRu.Rows.Add("Кораблекрушение");
		TblObjectSoDRu.Rows.Add("Потерпевший Кораблекрушение");
		TblObjectSoDRu.Rows.Add("Святыня Магического Воплощения");
		TblObjectSoDRu.Rows.Add("Святыня Магического Жеста");
		TblObjectSoDRu.Rows.Add("Святыня Магической Мысли");
		TblObjectSoDRu.Rows.Add("Свиток с Заклинанием");
		TblObjectSoDRu.Rows.Add("Таверна");
		TblObjectSoDRu.Rows.Add("Городок");
		TblObjectSoDRu.Rows.Add("Камень Знаний");
		TblObjectSoDRu.Rows.Add("Сундук Сокровищ");
		TblObjectSoDRu.Rows.Add("Древо Знаний");
		TblObjectSoDRu.Rows.Add("Врата Подземного Мира");
		TblObjectSoDRu.Rows.Add("Университет");
		TblObjectSoDRu.Rows.Add("Телега");
		TblObjectSoDRu.Rows.Add("Могила Воина");
		TblObjectSoDRu.Rows.Add("Водоворот");
		TblObjectSoDRu.Rows.Add("Ветряная мельница");
		TblObjectSoDRu.Rows.Add("Хижина Ведьмы");
		TblObjectSoDRu.Rows.Add("Гильдия свободных работников");
		TblObjectSoDRu.Rows.Add("Хранитель Вопроса");
		TblObjectSoDRu.Rows.Add("Торговцы Артефактами");
	}

	private void CreateTblDwellingComplete()
	{
		TblDwellingComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblDwellingComplete.Rows.Add("Логово чудищ");
		TblDwellingComplete.Rows.Add("Чертог Тьмы");
		TblDwellingComplete.Rows.Add("Склеп дракона");
		TblDwellingComplete.Rows.Add("Арена");
		TblDwellingComplete.Rows.Add("Портал славы");
		TblDwellingComplete.Rows.Add("Пещера циклопов");
		TblDwellingComplete.Rows.Add("Заброшенный дворец");
		TblDwellingComplete.Rows.Add("Колония земных элементалов");
		TblDwellingComplete.Rows.Add("Огненное озеро");
		TblDwellingComplete.Rows.Add("Алтарь грез");
		TblDwellingComplete.Rows.Add("Логово горгон");
		TblDwellingComplete.Rows.Add("Утесы дракона");
		TblDwellingComplete.Rows.Add("Пруд гидр");
		TblDwellingComplete.Rows.Add("Логово мантикор");
		TblDwellingComplete.Rows.Add("Лабиринт");
		TblDwellingComplete.Rows.Add("Монастырь");
		TblDwellingComplete.Rows.Add("Золотой павильон");
		TblDwellingComplete.Rows.Add("Геенна");
		TblDwellingComplete.Rows.Add("Пещера драконов");
		TblDwellingComplete.Rows.Add("Гнездо на утесе");
		TblDwellingComplete.Rows.Add("Небесный храм");
		TblDwellingComplete.Rows.Add("Своды дендроидов");
		TblDwellingComplete.Rows.Add("Гнездо виверн");
		TblDwellingComplete.Rows.Add("Поляна единорогов");
		TblDwellingComplete.Rows.Add("Мавзолей");
		TblDwellingComplete.Rows.Add("Алтарь Разума");
		TblDwellingComplete.Rows.Add("Погребальный костер");
		TblDwellingComplete.Rows.Add("Мерзлые утесы");
		TblDwellingComplete.Rows.Add("Кристальная пещера");
		TblDwellingComplete.Rows.Add("Волшебный лес");
		TblDwellingComplete.Rows.Add("Сернистое логово");
		TblDwellingComplete.Rows.Add("Яма заклинателей");
		TblDwellingComplete.Rows.Add("Поляна единорогов");
		TblDwellingComplete.Rows.Add("Алтарь Земли");
		TblDwellingComplete.Rows.Add("Мост троллей");
		TblDwellingComplete.Rows.Add("Фабрика големов");
		TblDwellingComplete.Rows.Add("Колония элементалов");
	}

	private void CreateTblDwellingSoDEn()
	{
		TblDwellingSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblDwellingSoDEn.Rows.Add("Behemoth Crag");
		TblDwellingSoDEn.Rows.Add("Hall of Darkness");
		TblDwellingSoDEn.Rows.Add("Dragon Vault");
		TblDwellingSoDEn.Rows.Add("Training Grounds");
		TblDwellingSoDEn.Rows.Add("Portal of Glory");
		TblDwellingSoDEn.Rows.Add("Cyclops Cave");
		TblDwellingSoDEn.Rows.Add("Forsaken Palace");
		TblDwellingSoDEn.Rows.Add("Earth Elemental Conflux");
		TblDwellingSoDEn.Rows.Add("Fire Lake");
		TblDwellingSoDEn.Rows.Add("Altar of Wishes");
		TblDwellingSoDEn.Rows.Add("Gorgon Lair");
		TblDwellingSoDEn.Rows.Add("Dragon Cliffs");
		TblDwellingSoDEn.Rows.Add("Hydra Pond");
		TblDwellingSoDEn.Rows.Add("Manticore Lair");
		TblDwellingSoDEn.Rows.Add("Labyrinth");
		TblDwellingSoDEn.Rows.Add("Monastery");
		TblDwellingSoDEn.Rows.Add("Golden Pavilion");
		TblDwellingSoDEn.Rows.Add("Hell Hole");
		TblDwellingSoDEn.Rows.Add("Dragon Cave");
		TblDwellingSoDEn.Rows.Add("Cliff Nest");
		TblDwellingSoDEn.Rows.Add("Cloud Temple");
		TblDwellingSoDEn.Rows.Add("Dendroid Arches");
		TblDwellingSoDEn.Rows.Add("Wyvern Nest");
		TblDwellingSoDEn.Rows.Add("Unicorn Glade");
		TblDwellingSoDEn.Rows.Add("Mausoleum");
		TblDwellingSoDEn.Rows.Add("Altar of Thought");
		TblDwellingSoDEn.Rows.Add("Pyre");
		TblDwellingSoDEn.Rows.Add("Frozen Cliffs");
		TblDwellingSoDEn.Rows.Add("Crystal Cavern");
		TblDwellingSoDEn.Rows.Add("Magic Forest");
		TblDwellingSoDEn.Rows.Add("Sulfurous Lair");
		TblDwellingSoDEn.Rows.Add("Enchanter's Hollow");
		TblDwellingSoDEn.Rows.Add("Unicorn Glade");
		TblDwellingSoDEn.Rows.Add("Altar of Earth");
		TblDwellingSoDEn.Rows.Add("Troll Bridge");
		TblDwellingSoDEn.Rows.Add("Golem Factory");
		TblDwellingSoDEn.Rows.Add("Elemental Conflux");
	}

	private void CreateTblDwellingSoDRu()
	{
		TblDwellingSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblDwellingSoDRu.Rows.Add("Утес Чудищ");
		TblDwellingSoDRu.Rows.Add("Залы Тьмы");
		TblDwellingSoDRu.Rows.Add("Свод Драконов");
		TblDwellingSoDRu.Rows.Add("Тренировочная площадка");
		TblDwellingSoDRu.Rows.Add("Портал Славы");
		TblDwellingSoDRu.Rows.Add("Пещера Циклопов");
		TblDwellingSoDRu.Rows.Add("Покинутый дворец");
		TblDwellingSoDRu.Rows.Add("Разлом Элементалей Земли");
		TblDwellingSoDRu.Rows.Add("Огненное озеро");
		TblDwellingSoDRu.Rows.Add("Алтарь желаний");
		TblDwellingSoDRu.Rows.Add("Логово Горгон");
		TblDwellingSoDRu.Rows.Add("Утес Драконов");
		TblDwellingSoDRu.Rows.Add("Пруд Гидр");
		TblDwellingSoDRu.Rows.Add("Берлога Мантикор");
		TblDwellingSoDRu.Rows.Add("Лабиринт");
		TblDwellingSoDRu.Rows.Add("Монастырь");
		TblDwellingSoDRu.Rows.Add("Золотой Павильон");
		TblDwellingSoDRu.Rows.Add("Адская дыра");
		TblDwellingSoDRu.Rows.Add("Пещера Драконов");
		TblDwellingSoDRu.Rows.Add("Гнездовой утес");
		TblDwellingSoDRu.Rows.Add("Облачный Храм");
		TblDwellingSoDRu.Rows.Add("Арка Дендроидов");
		TblDwellingSoDRu.Rows.Add("Гнездо Вивернов");
		TblDwellingSoDRu.Rows.Add("Опушка Единорогов");
		TblDwellingSoDRu.Rows.Add("Мавзолей");
		TblDwellingSoDRu.Rows.Add("Алтарь Мыслей");
		TblDwellingSoDRu.Rows.Add("Погребальный костер");
		TblDwellingSoDRu.Rows.Add("Замороженный утес");
		TblDwellingSoDRu.Rows.Add("Кристаллическая пещера");
		TblDwellingSoDRu.Rows.Add("Магический лес");
		TblDwellingSoDRu.Rows.Add("Серная Берлога");
		TblDwellingSoDRu.Rows.Add("Ложбина Чародеев");
		TblDwellingSoDRu.Rows.Add("Опушка Единорогов");
		TblDwellingSoDRu.Rows.Add("Алтарь Земли");
		TblDwellingSoDRu.Rows.Add("Мост Троллей");
		TblDwellingSoDRu.Rows.Add("Фабрика Големов");
		TblDwellingSoDRu.Rows.Add("Сопряжение");
	}

	private void CreateTblSecondarySkillComplete()
	{
		TblSecondarySkillComplete.Columns.Add("Name", Type.GetType("System.String"));
		TblSecondarySkillComplete.Rows.Add("Следопыт");
		TblSecondarySkillComplete.Rows.Add("Стрелок");
		TblSecondarySkillComplete.Rows.Add("Логистика");
		TblSecondarySkillComplete.Rows.Add("Разведка");
		TblSecondarySkillComplete.Rows.Add("Дипломатия");
		TblSecondarySkillComplete.Rows.Add("Навигация");
		TblSecondarySkillComplete.Rows.Add("Лидерство");
		TblSecondarySkillComplete.Rows.Add("Мудрость");
		TblSecondarySkillComplete.Rows.Add("Мистицизм");
		TblSecondarySkillComplete.Rows.Add("Удача");
		TblSecondarySkillComplete.Rows.Add("Баллистика");
		TblSecondarySkillComplete.Rows.Add("Орлиный глаз");
		TblSecondarySkillComplete.Rows.Add("Некромантия");
		TblSecondarySkillComplete.Rows.Add("Казначей");
		TblSecondarySkillComplete.Rows.Add("Огонь");
		TblSecondarySkillComplete.Rows.Add("Воздух");
		TblSecondarySkillComplete.Rows.Add("Вода");
		TblSecondarySkillComplete.Rows.Add("Земля");
		TblSecondarySkillComplete.Rows.Add("Книжник");
		TblSecondarySkillComplete.Rows.Add("Тактика");
		TblSecondarySkillComplete.Rows.Add("Боевая машина");
		TblSecondarySkillComplete.Rows.Add("Обучение");
		TblSecondarySkillComplete.Rows.Add("Атака");
		TblSecondarySkillComplete.Rows.Add("Защита");
		TblSecondarySkillComplete.Rows.Add("Разум");
		TblSecondarySkillComplete.Rows.Add("Ворожба");
		TblSecondarySkillComplete.Rows.Add("Устойчивость к магии");
		TblSecondarySkillComplete.Rows.Add("Лечение");
	}

	private void CreateTblSecondarySkillSoDEn()
	{
		TblSecondarySkillSoDEn.Columns.Add("Name", Type.GetType("System.String"));
		TblSecondarySkillSoDEn.Rows.Add("Pathfinding");
		TblSecondarySkillSoDEn.Rows.Add("Archery");
		TblSecondarySkillSoDEn.Rows.Add("Logistics");
		TblSecondarySkillSoDEn.Rows.Add("Scouting");
		TblSecondarySkillSoDEn.Rows.Add("Diplomacy");
		TblSecondarySkillSoDEn.Rows.Add("Navigation");
		TblSecondarySkillSoDEn.Rows.Add("Leadership");
		TblSecondarySkillSoDEn.Rows.Add("Wisdom");
		TblSecondarySkillSoDEn.Rows.Add("Mysticism");
		TblSecondarySkillSoDEn.Rows.Add("Luck");
		TblSecondarySkillSoDEn.Rows.Add("Ballistics");
		TblSecondarySkillSoDEn.Rows.Add("Eagle Eye");
		TblSecondarySkillSoDEn.Rows.Add("Necromancy");
		TblSecondarySkillSoDEn.Rows.Add("Estates");
		TblSecondarySkillSoDEn.Rows.Add("Fire Magic");
		TblSecondarySkillSoDEn.Rows.Add("Air Magic");
		TblSecondarySkillSoDEn.Rows.Add("Water Magic");
		TblSecondarySkillSoDEn.Rows.Add("Earth Magic");
		TblSecondarySkillSoDEn.Rows.Add("Scholar");
		TblSecondarySkillSoDEn.Rows.Add("Tactics");
		TblSecondarySkillSoDEn.Rows.Add("Artillery");
		TblSecondarySkillSoDEn.Rows.Add("Learning");
		TblSecondarySkillSoDEn.Rows.Add("Offense");
		TblSecondarySkillSoDEn.Rows.Add("Armorer");
		TblSecondarySkillSoDEn.Rows.Add("Intelligence");
		TblSecondarySkillSoDEn.Rows.Add("Sorcery");
		TblSecondarySkillSoDEn.Rows.Add("Resistance");
		TblSecondarySkillSoDEn.Rows.Add("First Aid");
	}

	private void CreateTblSecondarySkillSoDRu()
	{
		TblSecondarySkillSoDRu.Columns.Add("Name", Type.GetType("System.String"));
		TblSecondarySkillSoDRu.Rows.Add("Поиск Пути");
		TblSecondarySkillSoDRu.Rows.Add("Стрельба");
		TblSecondarySkillSoDRu.Rows.Add("Логистика");
		TblSecondarySkillSoDRu.Rows.Add("Разведка");
		TblSecondarySkillSoDRu.Rows.Add("Дипломатия");
		TblSecondarySkillSoDRu.Rows.Add("Навигация");
		TblSecondarySkillSoDRu.Rows.Add("Лидерство");
		TblSecondarySkillSoDRu.Rows.Add("Мудрость");
		TblSecondarySkillSoDRu.Rows.Add("Мистицизм");
		TblSecondarySkillSoDRu.Rows.Add("Удача");
		TblSecondarySkillSoDRu.Rows.Add("Баллистика");
		TblSecondarySkillSoDRu.Rows.Add("Зоркость");
		TblSecondarySkillSoDRu.Rows.Add("Чародейство");
		TblSecondarySkillSoDRu.Rows.Add("Имущество");
		TblSecondarySkillSoDRu.Rows.Add("Огонь");
		TblSecondarySkillSoDRu.Rows.Add("Воздух");
		TblSecondarySkillSoDRu.Rows.Add("Вода");
		TblSecondarySkillSoDRu.Rows.Add("Земля");
		TblSecondarySkillSoDRu.Rows.Add("Грамотность");
		TblSecondarySkillSoDRu.Rows.Add("Тактика");
		TblSecondarySkillSoDRu.Rows.Add("Артиллерия");
		TblSecondarySkillSoDRu.Rows.Add("Обучение");
		TblSecondarySkillSoDRu.Rows.Add("Нападение");
		TblSecondarySkillSoDRu.Rows.Add("Доспехи");
		TblSecondarySkillSoDRu.Rows.Add("Интеллект");
		TblSecondarySkillSoDRu.Rows.Add("Волшебство");
		TblSecondarySkillSoDRu.Rows.Add("Сопротивление");
		TblSecondarySkillSoDRu.Rows.Add("Первая помощь");
	}

	private void CreateArrayComplete()
	{
		aArtClass = new string[6] { "Машина", "Ценный", "Малый", "Великий", "Реликвия", "Реликвия-С" };
		aLevelSkill = new string[3] { "1.", "2.", "3." };
		aFullLvlSkill = new string[3] { "1 ступени", "2 ступени", "3 ступени" };
		aColor = new string[8] { "Красный", "Синий", "Серый", "Зеленый", "Оранжевый", "Лиловый", "Сизый", "Розовый" };
		aTown = new string[9] { "Замок", "Бастион", "Башня", "Инферно", "Некрополь", "Подземелье", "Цитадель", "Крепость", "Колония" };
		aDoll = new string[20]
		{
			"Голова", "Плечи", "Шея", "Правая рука", "Левая рука", "Тело", "Пальцы правой руки", "Пальцы левой руки", "Ноги", "Разное1",
			"Разное2", "Разное3", "Разное4", "", "", "", "", "", "Разное5", "Рюкзак"
		};
		aTent = new string[8] { "Голубая", "Зеленая", "Красная", "Синяя", "Серая", "Лиловая", "Белая", "Черная" };
		aHeroClass = new string[18]
		{
			"Рыцарь", "Клерик", "Рейнджер", "Друид", "Алхимик", "Чародей", "Одержимый", "Еретик", "Рыцарь смерти", "Некромант",
			"Верховный лорд", "Чернокнижник", "Варвар", "Боевой маг", "Зверолов", "Ведьма", "Странник", "Элементалист"
		};
	}

	private void CreateArraySoDEn()
	{
		aArtClass = new string[6] { "Machine", "Treasure", "Minor", "Major", "Relic", "Relic-C" };
		aMine = new string[7] { "Wood", "Mercury", "Ore", "Sulfur", "Crystal", "Gems", "Gold" };
		aLocality = new string[2] { "land", "sea" };
		aLevelSkill = new string[3] { "B.", "A.", "E." };
		aFullLvlSkill = new string[3] { "Basic", "Advanced", "Expert" };
		aColor = new string[8] { "Red", "Blue", "Tan", "Green", "Orange", "Purple", "Teal", "Pink" };
		aReply = new string[2] { "No", "Yes" };
		aTown = new string[9] { "Castle", "Rampart", "Tower", "Inferno", "Necropolis", "Dungeon", "Stronghold", "Fortress", "Conflux" };
		aDoll = new string[20]
		{
			"Head", "Shoulders", "Neck", "Right Hand", "Left Hand", "Torso", "Right Ring", "Left Ring", "Feet", "Misc1",
			"Misc2", "Misc3", "Misc4", "", "", "", "", "", "Misc5", "BackPack"
		};
		aPlace = new string[6] { "Prison", "Tavern", "Disable", "Town", "Map", "Boat" };
		aTent = new string[8] { "Light Blue", "Green", "Red", "Dark Blue", "Brown", "Purple", "White", "Black" };
		aStatus = new string[4] { "No", "Timer", "Available", "Constructed" };
		аQuest = new string[9] { "Achieve level", "Achieve skill", "Defeat monster", "Return with artifacts", "Return with creatures", "Return with resources", "Belong to player", "Defeat Hero", "Be Hero" };
		aReward = new string[9] { "Experience", "Spell points", "Morale", "Luck", "Resource", "Skill", "Artifact", "Spell", "Creatures" };
		aHeroClass = new string[18]
		{
			"Knight", "Cleric", "Ranger", "Druid", "Alchemist", "Wizard", "Demoniac", "Heretic", "Death Knight", "Necromancer",
			"Overlord", "Warlock", "Barbarian", "Battle Mage", "Beastmaster", "Witch", "Planeswalker", "Elementalist"
		};
		aIdeology = new string[3] { "Good", "Evil", "Neutral" };
	}

	private void CreateArraySoDRu()
	{
		aArtClass = new string[6] { "Машина", "Сокровище", "Малый", "Большой", "Реликвия", "Реликвия-С" };
		aColor = new string[8] { "Красный", "Синий", "Коричневый", "Зеленый", "Оранжевый", "Багровый", "Чайный", "Розовый" };
		aTown = new string[9] { "Замок", "Оплот", "Башня", "Инферно", "Некрополис", "Подземелье", "Цитадель", "Крепость", "Сопряжение" };
		aDoll = new string[20]
		{
			"Голова", "Плечи", "Шея", "Правая рука", "Левая рука", "Торс", "Правое кольцо", "Левое кольцо", "Ноги", "Разное1",
			"Разное2", "Разное3", "Разное4", "", "", "", "", "", "Разное5", "Рюкзак"
		};
		aTent = new string[8] { "Светло-голубая", "Зеленая", "Красная", "Темно-синяя", "Коричневая", "Пурпурная", "Белая", "Черная" };
		aHeroClass = new string[18]
		{
			"Рыцарь", "Священник", "Рейнджер", "Друид", "Алхимик", "Маг", "Демон", "Еретик", "Рыцарь смерти", "Некромант",
			"Лорд", "Чернокнижник", "Варвар", "Боевой маг", "Хозяин зверей", "Ведьма", "Путешественник", "Элементалист"
		};
	}

	private void Create_PW()
	{
		_PW[0, 0] = 35;
		_PW[0, 1] = 45;
		_PW[0, 2] = 10;
		_PW[0, 3] = 10;
		_PW[1, 0] = 20;
		_PW[1, 1] = 15;
		_PW[1, 2] = 30;
		_PW[1, 3] = 35;
		_PW[2, 0] = 35;
		_PW[2, 1] = 45;
		_PW[2, 2] = 10;
		_PW[2, 3] = 10;
		_PW[3, 0] = 10;
		_PW[3, 1] = 20;
		_PW[3, 2] = 35;
		_PW[3, 3] = 35;
		_PW[4, 0] = 30;
		_PW[4, 1] = 30;
		_PW[4, 2] = 20;
		_PW[4, 3] = 20;
		_PW[5, 0] = 10;
		_PW[5, 1] = 10;
		_PW[5, 2] = 40;
		_PW[5, 3] = 40;
		_PW[6, 0] = 35;
		_PW[6, 1] = 35;
		_PW[6, 2] = 15;
		_PW[6, 3] = 15;
		_PW[7, 0] = 15;
		_PW[7, 1] = 15;
		_PW[7, 2] = 35;
		_PW[7, 3] = 35;
		_PW[8, 0] = 30;
		_PW[8, 1] = 25;
		_PW[8, 2] = 20;
		_PW[8, 3] = 25;
		_PW[9, 0] = 15;
		_PW[9, 1] = 15;
		_PW[9, 2] = 35;
		_PW[9, 3] = 35;
		_PW[10, 0] = 35;
		_PW[10, 1] = 35;
		_PW[10, 2] = 15;
		_PW[10, 3] = 15;
		_PW[11, 0] = 10;
		_PW[11, 1] = 10;
		_PW[11, 2] = 50;
		_PW[11, 3] = 30;
		_PW[12, 0] = 55;
		_PW[12, 1] = 35;
		_PW[12, 2] = 5;
		_PW[12, 3] = 5;
		_PW[13, 0] = 30;
		_PW[13, 1] = 20;
		_PW[13, 2] = 25;
		_PW[13, 3] = 25;
		_PW[14, 0] = 30;
		_PW[14, 1] = 50;
		_PW[14, 2] = 10;
		_PW[14, 3] = 10;
		_PW[15, 0] = 5;
		_PW[15, 1] = 15;
		_PW[15, 2] = 40;
		_PW[15, 3] = 40;
		_PW[16, 0] = 45;
		_PW[16, 1] = 25;
		_PW[16, 2] = 15;
		_PW[16, 3] = 15;
		_PW[17, 0] = 15;
		_PW[17, 1] = 15;
		_PW[17, 2] = 35;
		_PW[17, 3] = 35;
	}

	private void Create_PW10()
	{
		_PW10[0, 0] = 30;
		_PW10[0, 1] = 30;
		_PW10[0, 2] = 20;
		_PW10[0, 3] = 20;
		_PW10[1, 0] = 20;
		_PW10[1, 1] = 20;
		_PW10[1, 2] = 30;
		_PW10[1, 3] = 30;
		_PW10[2, 0] = 30;
		_PW10[2, 1] = 30;
		_PW10[2, 2] = 20;
		_PW10[2, 3] = 20;
		_PW10[3, 0] = 20;
		_PW10[3, 1] = 20;
		_PW10[3, 2] = 30;
		_PW10[3, 3] = 30;
		_PW10[4, 0] = 30;
		_PW10[4, 1] = 30;
		_PW10[4, 2] = 20;
		_PW10[4, 3] = 20;
		_PW10[5, 0] = 30;
		_PW10[5, 1] = 20;
		_PW10[5, 2] = 20;
		_PW10[5, 3] = 30;
		_PW10[6, 0] = 30;
		_PW10[6, 1] = 30;
		_PW10[6, 2] = 20;
		_PW10[6, 3] = 20;
		_PW10[7, 0] = 20;
		_PW10[7, 1] = 20;
		_PW10[7, 2] = 30;
		_PW10[7, 3] = 30;
		_PW10[8, 0] = 25;
		_PW10[8, 1] = 25;
		_PW10[8, 2] = 25;
		_PW10[8, 3] = 25;
		_PW10[9, 0] = 25;
		_PW10[9, 1] = 25;
		_PW10[9, 2] = 25;
		_PW10[9, 3] = 25;
		_PW10[10, 0] = 30;
		_PW10[10, 1] = 30;
		_PW10[10, 2] = 20;
		_PW10[10, 3] = 20;
		_PW10[11, 0] = 20;
		_PW10[11, 1] = 20;
		_PW10[11, 2] = 30;
		_PW10[11, 3] = 30;
		_PW10[12, 0] = 30;
		_PW10[12, 1] = 30;
		_PW10[12, 2] = 20;
		_PW10[12, 3] = 20;
		_PW10[13, 0] = 25;
		_PW10[13, 1] = 25;
		_PW10[13, 2] = 25;
		_PW10[13, 3] = 25;
		_PW10[14, 0] = 30;
		_PW10[14, 1] = 30;
		_PW10[14, 2] = 20;
		_PW10[14, 3] = 20;
		_PW10[15, 0] = 20;
		_PW10[15, 1] = 20;
		_PW10[15, 2] = 30;
		_PW10[15, 3] = 30;
		_PW10[16, 0] = 30;
		_PW10[16, 1] = 30;
		_PW10[16, 2] = 20;
		_PW10[16, 3] = 20;
		_PW10[17, 0] = 25;
		_PW10[17, 1] = 25;
		_PW10[17, 2] = 25;
		_PW10[17, 3] = 25;
	}

	private void Create_SW()
	{
		_SW[0, 0] = 4;
		_SW[0, 1] = 5;
		_SW[0, 2] = 5;
		_SW[0, 3] = 4;
		_SW[0, 4] = 4;
		_SW[0, 5] = 8;
		_SW[0, 6] = 10;
		_SW[0, 7] = 3;
		_SW[0, 8] = 2;
		_SW[0, 9] = 3;
		_SW[0, 10] = 8;
		_SW[0, 11] = 2;
		_SW[0, 12] = 0;
		_SW[0, 13] = 6;
		_SW[0, 14] = 1;
		_SW[0, 15] = 3;
		_SW[0, 16] = 4;
		_SW[0, 17] = 2;
		_SW[0, 18] = 1;
		_SW[0, 19] = 7;
		_SW[0, 20] = 5;
		_SW[0, 21] = 4;
		_SW[0, 22] = 7;
		_SW[0, 23] = 5;
		_SW[0, 24] = 1;
		_SW[0, 25] = 1;
		_SW[0, 26] = 5;
		_SW[0, 27] = 2;
		_SW[1, 0] = 2;
		_SW[1, 1] = 3;
		_SW[1, 2] = 4;
		_SW[1, 3] = 3;
		_SW[1, 4] = 7;
		_SW[1, 5] = 5;
		_SW[1, 6] = 2;
		_SW[1, 7] = 7;
		_SW[1, 8] = 4;
		_SW[1, 9] = 5;
		_SW[1, 10] = 4;
		_SW[1, 11] = 6;
		_SW[1, 12] = 0;
		_SW[1, 13] = 3;
		_SW[1, 14] = 2;
		_SW[1, 15] = 4;
		_SW[1, 16] = 4;
		_SW[1, 17] = 3;
		_SW[1, 18] = 6;
		_SW[1, 19] = 2;
		_SW[1, 20] = 2;
		_SW[1, 21] = 4;
		_SW[1, 22] = 4;
		_SW[1, 23] = 3;
		_SW[1, 24] = 6;
		_SW[1, 25] = 5;
		_SW[1, 26] = 2;
		_SW[1, 27] = 10;
		_SW[2, 0] = 7;
		_SW[2, 1] = 8;
		_SW[2, 2] = 5;
		_SW[2, 3] = 7;
		_SW[2, 4] = 4;
		_SW[2, 5] = 3;
		_SW[2, 6] = 6;
		_SW[2, 7] = 3;
		_SW[2, 8] = 3;
		_SW[2, 9] = 6;
		_SW[2, 10] = 4;
		_SW[2, 11] = 2;
		_SW[2, 12] = 0;
		_SW[2, 13] = 2;
		_SW[2, 14] = 0;
		_SW[2, 15] = 1;
		_SW[2, 16] = 3;
		_SW[2, 17] = 3;
		_SW[2, 18] = 1;
		_SW[2, 19] = 5;
		_SW[2, 20] = 6;
		_SW[2, 21] = 4;
		_SW[2, 22] = 5;
		_SW[2, 23] = 8;
		_SW[2, 24] = 2;
		_SW[2, 25] = 2;
		_SW[2, 26] = 9;
		_SW[2, 27] = 3;
		_SW[3, 0] = 5;
		_SW[3, 1] = 5;
		_SW[3, 2] = 5;
		_SW[3, 3] = 2;
		_SW[3, 4] = 4;
		_SW[3, 5] = 2;
		_SW[3, 6] = 2;
		_SW[3, 7] = 8;
		_SW[3, 8] = 6;
		_SW[3, 9] = 9;
		_SW[3, 10] = 4;
		_SW[3, 11] = 7;
		_SW[3, 12] = 0;
		_SW[3, 13] = 3;
		_SW[3, 14] = 1;
		_SW[3, 15] = 2;
		_SW[3, 16] = 4;
		_SW[3, 17] = 4;
		_SW[3, 18] = 8;
		_SW[3, 19] = 1;
		_SW[3, 20] = 1;
		_SW[3, 21] = 4;
		_SW[3, 22] = 1;
		_SW[3, 23] = 3;
		_SW[3, 24] = 7;
		_SW[3, 25] = 6;
		_SW[3, 26] = 1;
		_SW[3, 27] = 7;
		_SW[4, 0] = 4;
		_SW[4, 1] = 5;
		_SW[4, 2] = 6;
		_SW[4, 3] = 4;
		_SW[4, 4] = 3;
		_SW[4, 5] = 3;
		_SW[4, 6] = 3;
		_SW[4, 7] = 6;
		_SW[4, 8] = 4;
		_SW[4, 9] = 2;
		_SW[4, 10] = 6;
		_SW[4, 11] = 3;
		_SW[4, 12] = 0;
		_SW[4, 13] = 4;
		_SW[4, 14] = 1;
		_SW[4, 15] = 4;
		_SW[4, 16] = 2;
		_SW[4, 17] = 3;
		_SW[4, 18] = 3;
		_SW[4, 19] = 4;
		_SW[4, 20] = 4;
		_SW[4, 21] = 10;
		_SW[4, 22] = 6;
		_SW[4, 23] = 8;
		_SW[4, 24] = 4;
		_SW[4, 25] = 3;
		_SW[4, 26] = 5;
		_SW[4, 27] = 2;
		_SW[5, 0] = 2;
		_SW[5, 1] = 2;
		_SW[5, 2] = 2;
		_SW[5, 3] = 2;
		_SW[5, 4] = 4;
		_SW[5, 5] = 1;
		_SW[5, 6] = 4;
		_SW[5, 7] = 10;
		_SW[5, 8] = 8;
		_SW[5, 9] = 4;
		_SW[5, 10] = 4;
		_SW[5, 11] = 8;
		_SW[5, 12] = 0;
		_SW[5, 13] = 5;
		_SW[5, 14] = 2;
		_SW[5, 15] = 6;
		_SW[5, 16] = 3;
		_SW[5, 17] = 3;
		_SW[5, 18] = 9;
		_SW[5, 19] = 1;
		_SW[5, 20] = 1;
		_SW[5, 21] = 4;
		_SW[5, 22] = 1;
		_SW[5, 23] = 1;
		_SW[5, 24] = 10;
		_SW[5, 25] = 8;
		_SW[5, 26] = 0;
		_SW[5, 27] = 7;
		_SW[6, 0] = 4;
		_SW[6, 1] = 6;
		_SW[6, 2] = 10;
		_SW[6, 3] = 5;
		_SW[6, 4] = 4;
		_SW[6, 5] = 4;
		_SW[6, 6] = 3;
		_SW[6, 7] = 4;
		_SW[6, 8] = 2;
		_SW[6, 9] = 2;
		_SW[6, 10] = 7;
		_SW[6, 11] = 3;
		_SW[6, 12] = 0;
		_SW[6, 13] = 3;
		_SW[6, 14] = 4;
		_SW[6, 15] = 2;
		_SW[6, 16] = 1;
		_SW[6, 17] = 3;
		_SW[6, 18] = 2;
		_SW[6, 19] = 6;
		_SW[6, 20] = 5;
		_SW[6, 21] = 4;
		_SW[6, 22] = 8;
		_SW[6, 23] = 7;
		_SW[6, 24] = 2;
		_SW[6, 25] = 3;
		_SW[6, 26] = 6;
		_SW[6, 27] = 2;
		_SW[7, 0] = 4;
		_SW[7, 1] = 4;
		_SW[7, 2] = 3;
		_SW[7, 3] = 3;
		_SW[7, 4] = 3;
		_SW[7, 5] = 2;
		_SW[7, 6] = 2;
		_SW[7, 7] = 8;
		_SW[7, 8] = 10;
		_SW[7, 9] = 2;
		_SW[7, 10] = 6;
		_SW[7, 11] = 4;
		_SW[7, 12] = 0;
		_SW[7, 13] = 2;
		_SW[7, 14] = 5;
		_SW[7, 15] = 3;
		_SW[7, 16] = 2;
		_SW[7, 17] = 4;
		_SW[7, 18] = 5;
		_SW[7, 19] = 4;
		_SW[7, 20] = 4;
		_SW[7, 21] = 4;
		_SW[7, 22] = 4;
		_SW[7, 23] = 4;
		_SW[7, 24] = 6;
		_SW[7, 25] = 6;
		_SW[7, 26] = 3;
		_SW[7, 27] = 5;
		_SW[8, 0] = 4;
		_SW[8, 1] = 5;
		_SW[8, 2] = 5;
		_SW[8, 3] = 4;
		_SW[8, 4] = 2;
		_SW[8, 5] = 8;
		_SW[8, 6] = 0;
		_SW[8, 7] = 6;
		_SW[8, 8] = 4;
		_SW[8, 9] = 1;
		_SW[8, 10] = 7;
		_SW[8, 11] = 4;
		_SW[8, 12] = 10;
		_SW[8, 13] = 0;
		_SW[8, 14] = 1;
		_SW[8, 15] = 2;
		_SW[8, 16] = 3;
		_SW[8, 17] = 4;
		_SW[8, 18] = 2;
		_SW[8, 19] = 5;
		_SW[8, 20] = 5;
		_SW[8, 21] = 4;
		_SW[8, 22] = 7;
		_SW[8, 23] = 5;
		_SW[8, 24] = 5;
		_SW[8, 25] = 4;
		_SW[8, 26] = 5;
		_SW[8, 27] = 0;
		_SW[9, 0] = 6;
		_SW[9, 1] = 2;
		_SW[9, 2] = 4;
		_SW[9, 3] = 2;
		_SW[9, 4] = 4;
		_SW[9, 5] = 5;
		_SW[9, 6] = 0;
		_SW[9, 7] = 8;
		_SW[9, 8] = 6;
		_SW[9, 9] = 1;
		_SW[9, 10] = 5;
		_SW[9, 11] = 7;
		_SW[9, 12] = 10;
		_SW[9, 13] = 3;
		_SW[9, 14] = 2;
		_SW[9, 15] = 3;
		_SW[9, 16] = 3;
		_SW[9, 17] = 8;
		_SW[9, 18] = 6;
		_SW[9, 19] = 2;
		_SW[9, 20] = 3;
		_SW[9, 21] = 4;
		_SW[9, 22] = 3;
		_SW[9, 23] = 2;
		_SW[9, 24] = 6;
		_SW[9, 25] = 6;
		_SW[9, 26] = 1;
		_SW[9, 27] = 0;
		_SW[10, 0] = 5;
		_SW[10, 1] = 6;
		_SW[10, 2] = 8;
		_SW[10, 3] = 5;
		_SW[10, 4] = 3;
		_SW[10, 5] = 4;
		_SW[10, 6] = 8;
		_SW[10, 7] = 3;
		_SW[10, 8] = 3;
		_SW[10, 9] = 1;
		_SW[10, 10] = 7;
		_SW[10, 11] = 2;
		_SW[10, 12] = 0;
		_SW[10, 13] = 4;
		_SW[10, 14] = 2;
		_SW[10, 15] = 1;
		_SW[10, 16] = 0;
		_SW[10, 17] = 3;
		_SW[10, 18] = 1;
		_SW[10, 19] = 10;
		_SW[10, 20] = 8;
		_SW[10, 21] = 4;
		_SW[10, 22] = 8;
		_SW[10, 23] = 6;
		_SW[10, 24] = 1;
		_SW[10, 25] = 2;
		_SW[10, 26] = 6;
		_SW[10, 27] = 1;
		_SW[11, 0] = 2;
		_SW[11, 1] = 2;
		_SW[11, 2] = 2;
		_SW[11, 3] = 2;
		_SW[11, 4] = 4;
		_SW[11, 5] = 4;
		_SW[11, 6] = 3;
		_SW[11, 7] = 10;
		_SW[11, 8] = 8;
		_SW[11, 9] = 2;
		_SW[11, 10] = 6;
		_SW[11, 11] = 8;
		_SW[11, 12] = 0;
		_SW[11, 13] = 5;
		_SW[11, 14] = 5;
		_SW[11, 15] = 2;
		_SW[11, 16] = 2;
		_SW[11, 17] = 5;
		_SW[11, 18] = 8;
		_SW[11, 19] = 1;
		_SW[11, 20] = 1;
		_SW[11, 21] = 4;
		_SW[11, 22] = 1;
		_SW[11, 23] = 1;
		_SW[11, 24] = 8;
		_SW[11, 25] = 10;
		_SW[11, 26] = 0;
		_SW[11, 27] = 6;
		_SW[12, 0] = 8;
		_SW[12, 1] = 7;
		_SW[12, 2] = 7;
		_SW[12, 3] = 8;
		_SW[12, 4] = 1;
		_SW[12, 5] = 2;
		_SW[12, 6] = 5;
		_SW[12, 7] = 2;
		_SW[12, 8] = 3;
		_SW[12, 9] = 3;
		_SW[12, 10] = 8;
		_SW[12, 11] = 2;
		_SW[12, 12] = 0;
		_SW[12, 13] = 2;
		_SW[12, 14] = 2;
		_SW[12, 15] = 3;
		_SW[12, 16] = 0;
		_SW[12, 17] = 3;
		_SW[12, 18] = 1;
		_SW[12, 19] = 8;
		_SW[12, 20] = 8;
		_SW[12, 21] = 4;
		_SW[12, 22] = 10;
		_SW[12, 23] = 6;
		_SW[12, 24] = 1;
		_SW[12, 25] = 1;
		_SW[12, 26] = 6;
		_SW[12, 27] = 1;
		_SW[13, 0] = 4;
		_SW[13, 1] = 4;
		_SW[13, 2] = 9;
		_SW[13, 3] = 4;
		_SW[13, 4] = 3;
		_SW[13, 5] = 0;
		_SW[13, 6] = 4;
		_SW[13, 7] = 6;
		_SW[13, 8] = 4;
		_SW[13, 9] = 2;
		_SW[13, 10] = 6;
		_SW[13, 11] = 5;
		_SW[13, 12] = 0;
		_SW[13, 13] = 1;
		_SW[13, 14] = 3;
		_SW[13, 15] = 3;
		_SW[13, 16] = 3;
		_SW[13, 17] = 3;
		_SW[13, 18] = 4;
		_SW[13, 19] = 5;
		_SW[13, 20] = 4;
		_SW[13, 21] = 4;
		_SW[13, 22] = 8;
		_SW[13, 23] = 4;
		_SW[13, 24] = 5;
		_SW[13, 25] = 6;
		_SW[13, 26] = 4;
		_SW[13, 27] = 4;
		_SW[14, 0] = 8;
		_SW[14, 1] = 7;
		_SW[14, 2] = 8;
		_SW[14, 3] = 7;
		_SW[14, 4] = 1;
		_SW[14, 5] = 8;
		_SW[14, 6] = 5;
		_SW[14, 7] = 2;
		_SW[14, 8] = 2;
		_SW[14, 9] = 2;
		_SW[14, 10] = 7;
		_SW[14, 11] = 1;
		_SW[14, 12] = 0;
		_SW[14, 13] = 1;
		_SW[14, 14] = 0;
		_SW[14, 15] = 1;
		_SW[14, 16] = 2;
		_SW[14, 17] = 3;
		_SW[14, 18] = 1;
		_SW[14, 19] = 6;
		_SW[14, 20] = 8;
		_SW[14, 21] = 4;
		_SW[14, 22] = 5;
		_SW[14, 23] = 10;
		_SW[14, 24] = 1;
		_SW[14, 25] = 1;
		_SW[14, 26] = 5;
		_SW[14, 27] = 6;
		_SW[15, 0] = 2;
		_SW[15, 1] = 3;
		_SW[15, 2] = 3;
		_SW[15, 3] = 2;
		_SW[15, 4] = 2;
		_SW[15, 5] = 6;
		_SW[15, 6] = 1;
		_SW[15, 7] = 8;
		_SW[15, 8] = 8;
		_SW[15, 9] = 4;
		_SW[15, 10] = 8;
		_SW[15, 11] = 10;
		_SW[15, 12] = 0;
		_SW[15, 13] = 1;
		_SW[15, 14] = 3;
		_SW[15, 15] = 3;
		_SW[15, 16] = 3;
		_SW[15, 17] = 3;
		_SW[15, 18] = 7;
		_SW[15, 19] = 1;
		_SW[15, 20] = 1;
		_SW[15, 21] = 4;
		_SW[15, 22] = 2;
		_SW[15, 23] = 4;
		_SW[15, 24] = 7;
		_SW[15, 25] = 8;
		_SW[15, 26] = 0;
		_SW[15, 27] = 8;
		_SW[16, 0] = 6;
		_SW[16, 1] = 8;
		_SW[16, 2] = 8;
		_SW[16, 3] = 6;
		_SW[16, 4] = 2;
		_SW[16, 5] = 5;
		_SW[16, 6] = 3;
		_SW[16, 7] = 2;
		_SW[16, 8] = 3;
		_SW[16, 9] = 2;
		_SW[16, 10] = 8;
		_SW[16, 11] = 2;
		_SW[16, 12] = 0;
		_SW[16, 13] = 3;
		_SW[16, 14] = 3;
		_SW[16, 15] = 2;
		_SW[16, 16] = 2;
		_SW[16, 17] = 3;
		_SW[16, 18] = 1;
		_SW[16, 19] = 8;
		_SW[16, 20] = 8;
		_SW[16, 21] = 8;
		_SW[16, 22] = 9;
		_SW[16, 23] = 5;
		_SW[16, 24] = 1;
		_SW[16, 25] = 1;
		_SW[16, 26] = 2;
		_SW[16, 27] = 1;
		_SW[17, 0] = 2;
		_SW[17, 1] = 2;
		_SW[17, 2] = 2;
		_SW[17, 3] = 2;
		_SW[17, 4] = 4;
		_SW[17, 5] = 4;
		_SW[17, 6] = 3;
		_SW[17, 7] = 8;
		_SW[17, 8] = 8;
		_SW[17, 9] = 2;
		_SW[17, 10] = 4;
		_SW[17, 11] = 8;
		_SW[17, 12] = 0;
		_SW[17, 13] = 3;
		_SW[17, 14] = 6;
		_SW[17, 15] = 6;
		_SW[17, 16] = 6;
		_SW[17, 17] = 6;
		_SW[17, 18] = 8;
		_SW[17, 19] = 1;
		_SW[17, 20] = 1;
		_SW[17, 21] = 4;
		_SW[17, 22] = 1;
		_SW[17, 23] = 1;
		_SW[17, 24] = 8;
		_SW[17, 25] = 8;
		_SW[17, 26] = 0;
		_SW[17, 27] = 4;
	}

	private void Get_SR(int s)
	{
		for (int i = 0; i < 28; i++)
		{
			_SR[i] = decmp[s + i];
		}
	}

	public static byte[] Decompress(byte[] input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		using GZipStream zip = new GZipStream(new MemoryStream(input), CompressionMode.Decompress, leaveOpen: false);
		return ReadAllBytes(zip);
	}

	private static byte[] ReadAllBytes(GZipStream zip)
	{
		if (zip == null)
		{
			throw new ArgumentNullException("zip");
		}
		int num = 1;
		byte[] array = new byte[num];
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		do
		{
			if (array.Length < num4 + num)
			{
				byte[] array2 = new byte[array.Length * 2];
				Array.Copy(array, array2, array.Length);
				array = array2;
			}
			try
			{
				num3 = zip.Read(array, num2, num);
			}
			catch
			{
				num3 = 0;
			}
			num2 += num;
			num4 += num3;
		}
		while (num3 == num);
		byte[] array3 = new byte[num4];
		Array.Copy(array, array3, num4);
		return array3;
	}

	public void InitMembers(bool MyInit, Control MyForm, BarManager MyManager, Font MyFont)
	{
		Type type = ((object)MyForm).GetType();
		MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
		foreach (MemberInfo memberInfo in members)
		{
			if (memberInfo.MemberType != MemberTypes.Field)
			{
				continue;
			}
			FieldInfo field = memberInfo.ReflectedType.GetField(memberInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if ((object)field == null || (object)field.FieldType != typeof(GridControl))
			{
				continue;
			}
			GridControl grid = (GridControl)type.InvokeMember(memberInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, MyForm, null);
			if (MyInit)
			{
				InitGridAndViews(grid, MyForm, MyManager);
				if (GridRegistryExist(grid, MyForm))
				{
					GridViewsDefaultLayout(grid, MyForm);
				}
				else if (MyFont != null)
				{
					GridChangeFont(grid, MyFont);
				}
			}
			else if (MyFont != null)
			{
				GridChangeFont(grid, MyFont);
			}
		}
	}

	private void InitGridAndViews(GridControl grid, Control MyForm, BarManager MyManager)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		((Control)grid).DoubleClick += GridControl_DoubleClick;
		if (!object.Equals(grid.MenuManager, MyManager))
		{
			grid.MenuManager = MyManager;
		}
		aGrid.Add(grid);
		hDS.Add(((Control)grid).Name, grid.DataSource);
		PopupMenu popupMenu = null;
		if (((Control)grid).Tag != null && ((Control)grid).Tag.ToString() != "")
		{
			popupMenu = (PopupMenu)((object)MyForm).GetType().InvokeMember((string)((Control)grid).Tag, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, MyForm, null);
			MyManager.SetPopupContextMenu((Control)(object)grid, popupMenu);
		}
		foreach (GridView view in grid.Views)
		{
			view.MouseDown += new MouseEventHandler(GridView_MouseDown);
			view.KeyDown += new KeyEventHandler(GridView_KeyDown);
			view.FocusedRowChanged += SetFocusedRow;
			view.PopupMenuShowing += GridView_PopupMenuShowing;
			MemoryStream memoryStream = new MemoryStream();
			view.SaveLayoutToStream(memoryStream, OptionsLayoutBase.FullLayout);
			hBT.Add(view.Name, memoryStream.GetBuffer());
			memoryStream.Close();
		}
	}

	private void GridViewsDefaultLayout(GridControl grid, Control frm)
	{
		foreach (GridView view in grid.Views)
		{
			string text = "Templates\\" + ((object)frm).GetType().Assembly.GetName().Name + "." + frm.Name + "." + view.Name;
			view.RestoreLayoutFromRegistry(CommonSetting.m_SoftwareKey + "\\" + text);
			Font font = GetFont(text, "Font");
			if (font != null)
			{
				GridChangeFont(view.GridControl, font);
			}
		}
	}

	private bool GridRegistryExist(GridControl grid, Control frm)
	{
		string text = "Templates\\" + ((object)frm).GetType().Assembly.GetName().Name + "." + frm.Name;
		foreach (GridView view in grid.Views)
		{
			if (Registry.RegistryKeyExist(text + "." + view.Name))
			{
				return true;
			}
		}
		return false;
	}

	private void PrintControl(IPrintable ctrl)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		PrintForm printForm = new PrintForm();
		PrintingSystem printingSystem = printForm.printingSystem1;
		PrintableComponentLink printableComponentLink = new PrintableComponentLink();
		printableComponentLink.Component = ctrl;
		printableComponentLink.Margins = new Margins(59, 59, 59, 59);
		printableComponentLink.PaperKind = (PaperKind)9;
		printingSystem.Links.Add(printableComponentLink);
		printableComponentLink.CreateDocument();
		try
		{
			((Form)printForm).ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void ShowCell(GridView gView)
	{
		if (gView.FocusedValue != null && gView.FocusedValue != DBNull.Value && gView.FocusedValue.ToString() != "")
		{
			ShowForm showForm = new ShowForm();
			((Form)showForm).Owner = (Form)(object)this;
			((Control)showForm.reCell).Text = gView.FocusedValue.ToString();
			if (gView.Name == "vwHero" || gView.Name == "vwPrison")
			{
				((Control)showForm).Text = gView.GetFocusedDataRow()["Hero"].ToString() + " - " + ((gView.FocusedColumn.Caption == "") ? gView.FocusedColumn.FieldName : gView.FocusedColumn.Caption);
			}
			else if (gView.Name == "vwEventBox")
			{
				((Control)showForm).Text = gView.GetFocusedDataRow()["X"].ToString() + "." + gView.GetFocusedDataRow()["Y"].ToString() + "." + gView.GetFocusedDataRow()["Z"].ToString() + " - " + ((gView.FocusedColumn.Caption == "") ? gView.FocusedColumn.FieldName : gView.FocusedColumn.Caption);
			}
			else
			{
				((Control)showForm).Text = ((gView.FocusedColumn.Caption == "") ? gView.FocusedColumn.FieldName : gView.FocusedColumn.Caption);
			}
			((Control)showForm).Show();
		}
	}

	private void popupMenu_CloseUp(object sender, EventArgs e)
	{
		PopupMenu popupMenu = (PopupMenu)sender;
		foreach (BarItemLink itemLink in popupMenu.ItemLinks)
		{
			itemLink.Visible = false;
		}
	}

	private void GridView_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Invalid comparison between Unknown and I4
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Invalid comparison between Unknown and I4
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Invalid comparison between Unknown and I4
		GridView gridView = (GridView)sender;
		BarManager barManager = (BarManager)gridView.GridControl.MenuManager;
		if ((int)e.KeyCode == 93)
		{
			if (barManager.GetPopupContextMenu((Control)(object)gridView.GridControl) == null)
			{
				PopupCreateGrid(gridView, null);
			}
			PopupMenu popupContextMenu = barManager.GetPopupContextMenu((Control)(object)gridView.GridControl);
			foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
			{
				itemLink.Visible = true;
			}
			popupContextMenu.ShowPopup(barManager, ((Control)gridView.GridControl).PointToScreen(new Point(0, 0)));
		}
		else if (e.Control && (int)e.KeyCode == 68)
		{
			GenItemClick(gridView, "Дерево прокачки");
		}
		else if (e.Control && (int)e.KeyCode == 87)
		{
			GenItemClick(gridView, "Показать ячейку");
		}
		else if (e.Control && (int)e.KeyCode == 80)
		{
			GenItemClick(gridView, "Печать");
		}
		else if (e.Control && (int)e.KeyCode == 69)
		{
			GenItemClick(gridView, "Копировать ячейку");
		}
		else if (e.Control && (int)e.KeyCode == 82)
		{
			GenItemClick(gridView, "Восстановить шаблон");
		}
	}

	private void GridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		if (e.HitInfo.InColumnPanel)
		{
			DXMenuCheckItem dXMenuCheckItem = new DXMenuCheckItem();
			if (e.HitInfo.Column.OptionsFilter.FilterPopupMode == FilterPopupMode.CheckedList)
			{
				dXMenuCheckItem.Checked = true;
			}
			else
			{
				dXMenuCheckItem.Checked = false;
			}
			dXMenuCheckItem.Tag = e.HitInfo.Column;
			dXMenuCheckItem.Click += GridMenuItemClick;
			dXMenuCheckItem.Caption = "Фильтр с помечаемым списком";
			dXMenuCheckItem.BeginGroup = true;
			e.Menu.Items.Add(dXMenuCheckItem);
		}
	}

	private void GridMenuItemClick(object sender, EventArgs e)
	{
		DXMenuCheckItem dXMenuCheckItem = (DXMenuCheckItem)sender;
		GridColumn gridColumn = (GridColumn)dXMenuCheckItem.Tag;
		if (gridColumn.OptionsFilter.FilterPopupMode == FilterPopupMode.CheckedList)
		{
			gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
		}
		else
		{
			gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		}
	}

	private void GridControl_DoubleClick(object sender, EventArgs e)
	{
		GridControl gridControl = (GridControl)sender;
		if (((GridView)gridControl.FocusedView).Name == "vwHero" && dblHitInfo != null && dblHitInfo.RowHandle >= 0)
		{
			Cursor.Current = Cursors.WaitCursor;
			LMOracle.fOwner = ((Control)gridControl).FindForm();
			if (CommonSetting.LMOracleFree)
			{
				LMOracle.ShowSkillTreeA((GridView)gridControl.FocusedView);
			}
			else
			{
				LMOracle.ShowSkillTree((GridView)gridControl.FocusedView);
			}
			Cursor.Current = Cursors.Default;
		}
		dblHitInfo = null;
	}

	private void GenItemClick(GridView gView, string cp)
	{
		BarButtonItem barButtonItem = ((BarManager)gView.GridControl.MenuManager).Items.CreateButton(cp);
		barButtonItem.Tag = gView;
		ItemClickEventArgs e = new ItemClickEventArgs(barButtonItem, null);
		MenuHandlerGrid(barButtonItem, e);
	}

	private void GridView_MouseDown(object sender, MouseEventArgs e)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		dblHitInfo = (hitInfo = ((GridView)sender).CalcHitInfo(new Point(e.X, e.Y)));
		if (hitInfo.RowHandle < 0)
		{
			hitInfo = null;
		}
		if ((int)e.Button != 2097152)
		{
			return;
		}
		GridView gridView = (GridView)sender;
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
		PopupMenu popupContextMenu = ((BarManager)gridView.GridControl.MenuManager).GetPopupContextMenu((Control)(object)gridView.GridControl);
		bool visible = !gridHitInfo.InColumnPanel && !gridHitInfo.InGroupPanel && gridHitInfo.HitTest != GridHitTest.Footer && gridHitInfo.HitTest != GridHitTest.RowFooter;
		bool flag = false;
		if (popupContextMenu == null)
		{
			PopupCreateGrid(gridView, null);
			popupContextMenu = ((BarManager)gridView.GridControl.MenuManager).GetPopupContextMenu((Control)(object)gridView.GridControl);
		}
		else
		{
			foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
			{
				if (itemLink.Caption == "Настройка")
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				PopupCreateGrid(gridView, popupContextMenu);
			}
		}
		if (popupContextMenu != null)
		{
			foreach (BarItemLink itemLink2 in popupContextMenu.ItemLinks)
			{
				itemLink2.Visible = visible;
			}
		}
		((Control)gridView.GridControl).Focus();
	}

	private void PopupCreateGrid(GridView view, PopupMenu pm)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		BarButtonItem barButtonItem = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Дерево прокачки");
		barButtonItem.ItemClick += MenuHandlerGrid;
		barButtonItem.ShortcutKeyDisplayString = "Ctrl+D";
		barButtonItem.Appearance.Font = new Font(barButtonItem.Appearance.Font, (FontStyle)1);
		barButtonItem.Tag = view;
		BarButtonItem barButtonItem2 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Показать ячейку");
		barButtonItem2.ItemClick += MenuHandlerGrid;
		barButtonItem2.ShortcutKeyDisplayString = "Ctrl+W";
		barButtonItem2.Tag = view;
		BarButtonItem barButtonItem3 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Копировать");
		barButtonItem3.ItemClick += MenuHandlerGrid;
		barButtonItem3.ShortcutKeyDisplayString = "Ctrl+C";
		barButtonItem3.Tag = view;
		BarButtonItem barButtonItem4 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Копировать ячейку");
		barButtonItem4.ItemClick += MenuHandlerGrid;
		barButtonItem4.ShortcutKeyDisplayString = "Ctrl+E";
		barButtonItem4.Tag = view;
		BarSubItem barSubItem = ((BarManager)view.GridControl.MenuManager).Items.CreateMenu("Настройка");
		BarSubItem barSubItem2 = ((BarManager)view.GridControl.MenuManager).Items.CreateMenu("Экспорт в LMOracle");
		barSubItem2.Popup += tsExpotr_Popup;
		BarButtonItem barButtonItem5 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Общие итоги");
		barButtonItem5.ItemClick += MenuHandlerGrid;
		barButtonItem5.ButtonStyle = BarButtonStyle.Check;
		barButtonItem5.Down = view.OptionsView.ShowFooter;
		barButtonItem5.Tag = view;
		BarButtonItem barButtonItem6 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Итоги по группам");
		barButtonItem6.ItemClick += MenuHandlerGrid;
		barButtonItem6.ButtonStyle = BarButtonStyle.Check;
		barButtonItem6.Down = view.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways;
		barButtonItem6.Tag = view;
		BarButtonItem barButtonItem7 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Объединить ячейки");
		barButtonItem7.ItemClick += MenuHandlerGrid;
		barButtonItem7.ButtonStyle = BarButtonStyle.Check;
		barButtonItem7.Down = view.OptionsView.AllowCellMerge;
		barButtonItem7.Tag = view;
		BarButtonItem barButtonItem8 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Автоширина столбцов");
		barButtonItem8.ItemClick += MenuHandlerGrid;
		barButtonItem8.ButtonStyle = BarButtonStyle.Check;
		barButtonItem8.Down = view.OptionsView.ColumnAutoWidth;
		barButtonItem8.Tag = view;
		BarButtonItem barButtonItem9 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Шрифт");
		barButtonItem9.ItemClick += MenuHandlerGrid;
		barButtonItem9.Tag = view;
		BarButtonItem barButtonItem10 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Печать");
		barButtonItem10.ItemClick += MenuHandlerGrid;
		barButtonItem10.ShortcutKeyDisplayString = "Ctrl+P";
		barButtonItem10.Tag = view;
		BarButtonItem barButtonItem11 = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Восстановить шаблон");
		barButtonItem11.ItemClick += MenuHandlerGrid;
		barButtonItem11.ShortcutKeyDisplayString = "Ctrl+R";
		barButtonItem11.Tag = view;
		barSubItem.ItemLinks.Add(barButtonItem11);
		BarButtonItem item = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Создать шаблон по умолчанию");
		btnTemplateInit(view, item);
		barSubItem.ItemLinks.Add(item, beginGroup: true);
		item = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Загрузить шаблон");
		item.ItemClick += MenuHandlerGrid;
		item.Tag = view;
		barSubItem.ItemLinks.Add(item, beginGroup: true);
		item = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Сохранить шаблон");
		item.ItemClick += MenuHandlerGrid;
		item.Tag = view;
		barSubItem.ItemLinks.Add(item);
		item = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Копировать ");
		item.ItemClick += MenuHandlerGrid;
		item.Tag = view;
		barSubItem2.ItemLinks.Add(item);
		item = ((BarManager)view.GridControl.MenuManager).Items.CreateButton("Сохранить");
		item.ItemClick += MenuHandlerGrid;
		item.Tag = view;
		barSubItem2.ItemLinks.Add(item, beginGroup: true);
		if (pm == null)
		{
			pm = ((BarManager)view.GridControl.MenuManager).Items.CreatePopupMenu();
			if (view.Name == "vwHero")
			{
				pm.ItemLinks.Add(barButtonItem);
				pm.ItemLinks.Add(barButtonItem2, beginGroup: true);
			}
			if (view.Name == "vwPrison" || view.Name == "vwEventBox" || view.Name == "vwAllTimer")
			{
				pm.ItemLinks.Add(barButtonItem2);
			}
			pm.ItemLinks.Add(barButtonItem3, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem4);
			if (view.Name == "vwHero")
			{
				pm.ItemLinks.Add(barSubItem2, beginGroup: true);
			}
			pm.ItemLinks.Add(barButtonItem5, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem6);
			pm.ItemLinks.Add(barButtonItem7, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem8);
			pm.ItemLinks.Add(barButtonItem9, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem10);
			pm.ItemLinks.Add(barSubItem, beginGroup: true);
			((BarManager)view.GridControl.MenuManager).SetPopupContextMenu((Control)(object)view.GridControl, pm);
		}
		else
		{
			if (view.Name == "vwHero")
			{
				pm.ItemLinks.Add(barButtonItem);
				pm.ItemLinks.Add(barButtonItem2, beginGroup: true);
			}
			if (view.Name == "vwPrison" || view.Name == "vwEventBox" || view.Name == "vwAllTimer")
			{
				pm.ItemLinks.Add(barButtonItem2);
			}
			pm.ItemLinks.Add(barButtonItem3, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem4);
			if (view.Name == "vwHero")
			{
				pm.ItemLinks.Add(barSubItem2, beginGroup: true);
			}
			pm.ItemLinks.Add(barButtonItem5, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem6);
			pm.ItemLinks.Add(barButtonItem7, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem8);
			pm.ItemLinks.Add(barButtonItem9, beginGroup: true);
			pm.ItemLinks.Add(barButtonItem10);
			pm.ItemLinks.Add(barSubItem, beginGroup: true);
		}
		pm.CloseUp += popupMenu_CloseUp;
	}

	private void btnTemplateInit(GridView View, BarButtonItem item)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		Form val = (Form)((BarManager)View.GridControl.MenuManager).Form;
		ArrayList arrayList = new ArrayList();
		arrayList.AddRange(new object[4] { val, View, null, null });
		item.Tag = arrayList;
		item.ItemClick += btnTemplate_ItemClick;
		if (Registry.RegistryKeyExist("Templates\\" + ((object)val).GetType().Assembly.GetName().Name + "." + ((Control)val).Name + "." + View.Name))
		{
			item.Caption = "Удалить шаблон по умолчанию";
		}
		else
		{
			item.Caption = "Создать шаблон по умолчанию";
		}
	}

	private void btnTemplate_ItemClick(object sender, ItemClickEventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		BarButtonItem barButtonItem = (BarButtonItem)e.Item;
		Form val = (Form)((ArrayList)barButtonItem.Tag)[0];
		GridView gridView = (GridView)((ArrayList)barButtonItem.Tag)[1];
		string text = "Templates\\" + ((object)val).GetType().Assembly.GetName().Name + "." + ((Control)val).Name + "." + gridView.Name;
		if (barButtonItem.Caption == "Удалить шаблон по умолчанию")
		{
			Registry.DeleteKey(text);
			barButtonItem.Caption = "Создать шаблон по умолчанию";
		}
		else
		{
			gridView.SaveLayoutToRegistry(CommonSetting.m_SoftwareKey + "\\" + text, OptionsLayoutBase.FullLayout);
			barButtonItem.Caption = "Удалить шаблон по умолчанию";
			SetFont(gridView.Appearance.FilterPanel.Font, text, "Font");
		}
	}

	private void RestoreLayout(GridView gView)
	{
		Control form = ((BarManager)gView.GridControl.MenuManager).Form;
		string text = "Templates\\" + ((object)form).GetType().Assembly.GetName().Name + "." + form.Name + "." + gView.Name;
		if (Registry.RegistryKeyExist(text))
		{
			gView.RestoreLayoutFromRegistry(CommonSetting.m_SoftwareKey + "\\" + text, OptionsLayoutBase.FullLayout);
		}
		else
		{
			MemoryStream memoryStream = new MemoryStream((byte[])hBT[gView.Name]);
			gView.RestoreLayoutFromStream(memoryStream, OptionsLayoutBase.FullLayout);
			memoryStream.Close();
		}
		RestoreItemDown(gView);
		if (gView.FocusedRowHandle >= 0)
		{
			gView.SelectRow(gView.FocusedRowHandle);
			gView.MakeRowVisible(gView.FocusedRowHandle, invalidate: true);
		}
	}

	private void RestoreItemDown(GridView gView)
	{
		BarManager barManager = (BarManager)gView.GridControl.MenuManager;
		PopupMenu popupContextMenu = barManager.GetPopupContextMenu((Control)(object)gView.GridControl);
		if (popupContextMenu == null)
		{
			return;
		}
		foreach (BarItemLink itemLink in popupContextMenu.ItemLinks)
		{
			switch (itemLink.Caption)
			{
			case "Объединить ячейки":
				((BarButtonItem)itemLink.Item).Down = gView.OptionsView.AllowCellMerge;
				break;
			case "Автоширина столбцов":
				((BarButtonItem)itemLink.Item).Down = gView.OptionsView.ColumnAutoWidth;
				break;
			case "Итоги по группам":
				((BarButtonItem)itemLink.Item).Down = gView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways;
				break;
			case "Общие итоги":
				((BarButtonItem)itemLink.Item).Down = gView.OptionsView.ShowFooter;
				break;
			}
		}
	}

	private void MenuHandlerGrid(object sender, ItemClickEventArgs e)
	{
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Expected O, but got Unknown
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Invalid comparison between Unknown and I4
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Expected O, but got Unknown
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Invalid comparison between Unknown and I4
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Invalid comparison between Unknown and I4
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		switch (e.Item.Caption)
		{
		case "Дерево прокачки":
		{
			GridView gridView = (GridView)e.Item.Tag;
			if (gridView.Name == "vwHero")
			{
				Cursor.Current = Cursors.WaitCursor;
				LMOracle.fOwner = ((Control)gridView.GridControl).FindForm();
				if (CommonSetting.LMOracleFree)
				{
					LMOracle.ShowSkillTreeA(gridView);
				}
				else
				{
					LMOracle.ShowSkillTree(gridView);
				}
				Cursor.Current = Cursors.Default;
			}
			break;
		}
		case "Показать ячейку":
			ShowCell((GridView)e.Item.Tag);
			break;
		case "Копировать":
			((GridView)e.Item.Tag).CopyToClipboard();
			break;
		case "Копировать ячейку":
		{
			GridView gridView = (GridView)e.Item.Tag;
			if (gridView.FocusedValue != null && gridView.FocusedValue != DBNull.Value && gridView.FocusedValue.ToString() != "")
			{
				try
				{
					Clipboard.SetData(DataFormats.GetFormat(DataFormats.StringFormat).Name, gridView.FocusedValue);
					break;
				}
				catch
				{
					break;
				}
			}
			break;
		}
		case "Восстановить шаблон":
			RestoreLayout((GridView)e.Item.Tag);
			break;
		case "Печать":
			PrintControl(((GridView)e.Item.Tag).GridControl);
			break;
		case "Объединить ячейки":
			if (((BarButtonItem)e.Item).Down)
			{
				((GridView)e.Item.Tag).OptionsView.AllowCellMerge = true;
			}
			else
			{
				((GridView)e.Item.Tag).OptionsView.AllowCellMerge = false;
			}
			break;
		case "Автоширина столбцов":
			if (((BarButtonItem)e.Item).Down)
			{
				((GridView)e.Item.Tag).OptionsView.ColumnAutoWidth = true;
			}
			else
			{
				((GridView)e.Item.Tag).OptionsView.ColumnAutoWidth = false;
			}
			break;
		case "Общие итоги":
			if (((BarButtonItem)e.Item).Down)
			{
				((GridView)e.Item.Tag).OptionsView.ShowFooter = true;
			}
			else
			{
				((GridView)e.Item.Tag).OptionsView.ShowFooter = false;
			}
			break;
		case "Итоги по группам":
			if (((BarButtonItem)e.Item).Down)
			{
				((GridView)e.Item.Tag).GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
			}
			else
			{
				((GridView)e.Item.Tag).GroupFooterShowMode = GroupFooterShowMode.Hidden;
			}
			break;
		case "Шрифт":
		{
			FontDialog val3 = new FontDialog();
			val3.Font = ((GridView)e.Item.Tag).Appearance.Row.Font;
			try
			{
				if ((int)((CommonDialog)val3).ShowDialog() == 1)
				{
					GridChangeFont(((GridView)e.Item.Tag).GridControl, val3.Font);
				}
				break;
			}
			catch (Exception ex3)
			{
				MessageBox.Show(ex3.Message, ex3.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
				break;
			}
		}
		case "Загрузить шаблон":
		{
			OpenFileDialog val2 = new OpenFileDialog();
			((FileDialog)val2).Filter = "XML Files(*.XML)|*.XML";
			if ((int)((CommonDialog)val2).ShowDialog() != 1)
			{
				break;
			}
			if (CheckViewName(((FileDialog)val2).FileName, ((GridView)e.Item.Tag).Name))
			{
				try
				{
					((GridView)e.Item.Tag).RestoreLayoutFromXml(((FileDialog)val2).FileName, OptionsLayoutBase.FullLayout);
					RestoreItemDown((GridView)e.Item.Tag);
					break;
				}
				catch (Exception ex2)
				{
					MessageBox.Show(ex2.Message, ex2.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
					break;
				}
			}
			MessageBox.Show(sFile + " " + Path.GetFileName(((FileDialog)val2).FileName) + " " + errorTemplateTable, CommonSetting.MyName, (MessageBoxButtons)0, (MessageBoxIcon)16);
			break;
		}
		case "Сохранить шаблон":
		{
			SaveFileDialog val = new SaveFileDialog();
			((FileDialog)val).Filter = "XML Files(*.XML)|*.XML";
			((FileDialog)val).FileName = ((GridView)e.Item.Tag).Name.Remove(0, 2) + "Template.XML";
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				try
				{
					((GridView)e.Item.Tag).SaveLayoutToXml(((FileDialog)val).FileName, OptionsLayoutBase.FullLayout);
					break;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
					break;
				}
			}
			break;
		}
		case "Копировать ":
			LMOracleCopy((GridView)e.Item.Tag);
			break;
		case "Сохранить":
			LMOracleSave((GridView)e.Item.Tag);
			break;
		}
	}

	private void LMOracleCopy(GridView view)
	{
		if (view.RowCount > 0)
		{
			int[] selectedRows = view.GetSelectedRows();
			string text = "";
			for (int i = 0; i < selectedRows.Length; i++)
			{
				DataRow dataRow = view.GetDataRow(selectedRows[i]);
				text = text + dataRow[0].ToString() + ";";
			}
			try
			{
				Clipboard.SetData(DataFormats.GetFormat(DataFormats.Text).Name, (object)text);
			}
			catch
			{
			}
		}
	}

	private void LMOracleSave(GridView view)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		if (view.RowCount <= 0)
		{
			return;
		}
		string fileName = Path.GetFileName(CommonSetting.MyFile).Replace(Path.GetExtension(CommonSetting.MyFile), "");
		((FileDialog)saveFileDialog1).FileName = fileName;
		((FileDialog)saveFileDialog1).AddExtension = true;
		if ((int)((CommonDialog)saveFileDialog1).ShowDialog() == 1)
		{
			int[] selectedRows = view.GetSelectedRows();
			byte[] array = new byte[selectedRows.Length + 1];
			array[0] = (byte)selectedRows.Length;
			for (int i = 0; i < selectedRows.Length; i++)
			{
				DataRow dataRow = view.GetDataRow(selectedRows[i]);
				int num = (int)dataRow[0];
				array[i + 1] = (byte)num;
			}
			try
			{
				File.WriteAllBytes(((FileDialog)saveFileDialog1).FileName, array);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source + "  -  " + ((FileDialog)saveFileDialog1).FileName, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}
	}

	private bool CheckViewName(string file, string name)
	{
		StreamReader streamReader = new StreamReader(file);
		while (!streamReader.EndOfStream)
		{
			string text = streamReader.ReadLine();
			if (text.IndexOf(name) > 0)
			{
				streamReader.Close();
				return true;
			}
		}
		streamReader.Close();
		return false;
	}

	private void tsExpotr_Popup(object sender, EventArgs e)
	{
		BarSubItem barSubItem = (BarSubItem)sender;
		foreach (LinkPersistInfo item in barSubItem.LinksPersistInfo)
		{
			item.Item.Enabled = tsButtonRefresh.Enabled;
		}
	}

	private void GridChangeFont(GridControl grid, Font fnt)
	{
		foreach (GridView view in grid.Views)
		{
			view.Appearance.FilterPanel.Font = fnt;
			view.Appearance.FilterPanel.Options.UseFont = true;
			view.Appearance.FocusedCell.Font = fnt;
			view.Appearance.FocusedCell.Options.UseFont = true;
			view.Appearance.FocusedRow.Font = fnt;
			view.Appearance.FocusedRow.Options.UseFont = true;
			view.Appearance.FooterPanel.Font = fnt;
			view.Appearance.FooterPanel.Options.UseFont = true;
			view.Appearance.GroupFooter.Font = fnt;
			view.Appearance.GroupFooter.Options.UseFont = true;
			view.Appearance.GroupPanel.Font = fnt;
			view.Appearance.GroupPanel.Options.UseFont = true;
			view.Appearance.GroupRow.Font = fnt;
			view.Appearance.GroupRow.Options.UseFont = true;
			view.Appearance.HeaderPanel.Font = fnt;
			view.Appearance.HeaderPanel.Options.UseFont = true;
			view.Appearance.HideSelectionRow.Font = fnt;
			view.Appearance.HideSelectionRow.Options.UseFont = true;
			view.Appearance.Row.Font = fnt;
			view.Appearance.Row.Options.UseFont = true;
			view.Appearance.SelectedRow.Font = fnt;
			view.Appearance.SelectedRow.Options.UseFont = true;
		}
	}

	private void SetFont(Font fnt, string Key, string Name)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Registry.SetRegistryValue(Key, Name, fnt.Name + ";" + fnt.SizeInPoints + ";" + ((object)fnt.Style).ToString());
	}

	private bool GetFontStyle(string fstr, ref FontStyle fstl)
	{
		bool result = false;
		switch (fstr)
		{
		case "Regular":
			fstl = (FontStyle)0;
			result = true;
			break;
		case "Bold":
			fstl = (FontStyle)1;
			result = true;
			break;
		case "Italic":
			fstl = (FontStyle)2;
			result = true;
			break;
		case "Strikeout":
			fstl = (FontStyle)8;
			result = true;
			break;
		case "Underline":
			fstl = (FontStyle)4;
			result = true;
			break;
		}
		return result;
	}

	private Font GetFont(string Key, string Name)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		string registryValue = Registry.GetRegistryValue(Key, Name);
		if (registryValue != "")
		{
			string[] array = registryValue.Split(new string[1] { ";" }, StringSplitOptions.None);
			string[] array2 = array[2].Split(new string[1] { "," }, StringSplitOptions.None);
			FontStyle val = (FontStyle)0;
			string[] array3 = array2;
			foreach (string text in array3)
			{
				FontStyle fstl = (FontStyle)0;
				val = (FontStyle)((!GetFontStyle(text.Trim(), ref fstl)) ? ((FontStyle)0) : (val | fstl));
			}
			try
			{
				return new Font(array[0], float.Parse(array[1]), val);
			}
			catch
			{
				Registry.SetRegistryValue(Key, Name, "");
				return null;
			}
		}
		return null;
	}

	private void ExportExcel()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Invalid comparison between Unknown and I4
		ExportForm exportForm = new ExportForm();
		SheetsName = GetSheetsName();
		((Control)exportForm).Tag = SheetsName;
		((Control)exportForm).Text = CommonSetting.MyFile;
		if ((int)((Form)exportForm).ShowDialog() == 1)
		{
			string text = ((Control)exportForm).Text;
			string randomFileName = Path.GetRandomFileName();
			string tempPath = Path.GetTempPath();
			string text2 = Path.Combine(tempPath, randomFileName.Replace(Path.GetExtension(randomFileName), ""));
			try
			{
				Directory.CreateDirectory(text2);
				File.WriteAllBytes(Path.Combine(text2, "Template.zip"), Resources.Template);
				ZipStorer zipStorer = ZipStorer.Open(Path.Combine(text2, "Template.zip"), FileAccess.Read);
				List<ZipStorer.ZipFileEntry> list = zipStorer.ReadCentralDir();
				foreach (ZipStorer.ZipFileEntry item in list)
				{
					string filename = Path.Combine(text2, item.FilenameInZip);
					zipStorer.ExtractFile(item, filename);
				}
				zipStorer.Close();
				Directory.CreateDirectory(Path.Combine(text2, "xl\\_rels"));
				Directory.CreateDirectory(Path.Combine(text2, "xl\\worksheets"));
				SaveXML(text2);
				zipStorer = ZipStorer.Create(text, "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\theme\\theme1.xml"), "xl\\theme\\theme1.xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\styles.xml"), "xl\\styles.xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "docProps\\core.xml"), "docProps\\core.xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "_rels\\.rels"), "_rels\\.rels", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "[Content_Types].xml"), "[Content_Types].xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\workbook.xml"), "xl\\workbook.xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "docProps\\app.xml"), "docProps\\app.xml", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\_rels\\workbook.xml.rels"), "xl\\_rels\\workbook.xml.rels", "");
				zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\sharedStrings.xml"), "xl\\sharedStrings.xml", "");
				for (int i = 1; i <= SheetsName.Count; i++)
				{
					zipStorer.AddFile(ZipStorer.Compression.Deflate, Path.Combine(text2, "xl\\worksheets\\sheet" + i + ".xml"), "xl\\worksheets\\sheet" + i + ".xml", "");
				}
				zipStorer.Close();
				if ((int)MessageBox.Show(messageExcel + "  " + Path.GetFileName(text) + "?", "ProspectorRT", (MessageBoxButtons)4, (MessageBoxIcon)64) == 6 && File.Exists(text))
				{
					try
					{
						Process.Start(text);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
					}
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message, ex2.Source, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			finally
			{
				try
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text2);
					directoryInfo.Delete(recursive: true);
				}
				catch
				{
				}
			}
		}
		((Component)(object)exportForm).Dispose();
	}

	private GridView[] GetViews()
	{
		GridView[] array = new GridView[SheetsName.Count];
		int num = 0;
		foreach (XtraTabPage tabPage in tabControl.TabPages)
		{
			if (SheetsName.IndexOf(((Control)tabPage).Text) >= 0)
			{
				GridControl gridControl = (GridControl)(object)((Control)tabPage).GetNextControl((Control)(object)tabPage, true);
				GridView gridView = (GridView)gridControl.FocusedView;
				array[num] = gridView;
				num++;
			}
		}
		return array;
	}

	private IList<string> GetSheetsName()
	{
		List<string> list = new List<string>();
		foreach (XtraTabPage tabPage in tabControl.TabPages)
		{
			GridControl gridControl = (GridControl)(object)((Control)tabPage).GetNextControl((Control)(object)tabPage, true);
			GridView gridView = (GridView)gridControl.FocusedView;
			if (gridView.DataRowCount > 0)
			{
				list.Add(((Control)tabPage).Text);
			}
			else
			{
				list.Add("~" + ((Control)tabPage).Text);
			}
		}
		return list;
	}

	private void SaveXML(string myPath)
	{
		GridView[] views = GetViews();
		using (FileStream output = new FileStream(Path.Combine(myPath, "[Content_Types].xml"), FileMode.Create))
		{
			WriteContentTypes(output, SheetsName);
		}
		using (FileStream output2 = new FileStream(Path.Combine(myPath, "xl\\workbook.xml"), FileMode.Create))
		{
			WriteWorkBook(output2, SheetsName);
		}
		using (FileStream output3 = new FileStream(Path.Combine(myPath, "docProps\\app.xml"), FileMode.Create))
		{
			WriteDocPropsApp(output3, SheetsName);
		}
		using (FileStream output4 = new FileStream(Path.Combine(myPath, "xl\\_rels\\workbook.xml.rels"), FileMode.Create))
		{
			WriteXlRels(output4, SheetsName);
		}
		IDictionary<string, int> lookupTable;
		IList<string> stringTable = CreateStringTables(views, out lookupTable);
		using (FileStream output5 = new FileStream(Path.Combine(myPath, "xl\\sharedStrings.xml"), FileMode.Create))
		{
			WriteStringTable(output5, stringTable);
		}
		for (int i = 0; i < views.Length; i++)
		{
			using FileStream output6 = new FileStream(Path.Combine(myPath, "xl\\worksheets\\sheet" + (i + 1) + ".xml"), FileMode.Create);
			WriteWorksheet(output6, views[i], lookupTable);
		}
	}

	private void WriteDocPropsApp(Stream output, IList<string> strSheetsName)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(output, Encoding.UTF8);
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("Properties");
		xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
		xmlTextWriter.WriteAttributeString("xmlns:vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
		xmlTextWriter.WriteElementString("Application", "Microsoft Excel");
		xmlTextWriter.WriteElementString("DocSecurity", "0");
		xmlTextWriter.WriteElementString("ScaleCrop", "false");
		xmlTextWriter.WriteStartElement("HeadingPairs");
		xmlTextWriter.WriteStartElement("vt:vector");
		xmlTextWriter.WriteAttributeString("size", "2");
		xmlTextWriter.WriteAttributeString("baseType", "variant");
		xmlTextWriter.WriteStartElement("vt:variant");
		xmlTextWriter.WriteElementString("vt:lpstr", "Worksheets");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("vt:variant");
		xmlTextWriter.WriteElementString("vt:i4", strSheetsName.Count.ToString());
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("TitlesOfParts");
		xmlTextWriter.WriteStartElement("vt:vector");
		xmlTextWriter.WriteAttributeString("size", strSheetsName.Count.ToString());
		xmlTextWriter.WriteAttributeString("baseType", "lpstr");
		for (int i = 1; i <= strSheetsName.Count; i++)
		{
			xmlTextWriter.WriteElementString("vt:lpstr", "Sheet" + i);
		}
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteElementString("Company", "");
		xmlTextWriter.WriteElementString("LinksUpToDate", "false");
		xmlTextWriter.WriteElementString("SharedDoc", "false");
		xmlTextWriter.WriteElementString("HyperlinksChanged", "false");
		xmlTextWriter.WriteElementString("AppVersion", "12.0000");
		xmlTextWriter.WriteEndElement();
	}

	private void WriteContentTypes(Stream output, IList<string> strSheetsName)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(output, Encoding.UTF8);
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("Types");
		xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/xl/theme/theme1.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.theme+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/xl/styles.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Default");
		xmlTextWriter.WriteAttributeString("Extension", "rels");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Default");
		xmlTextWriter.WriteAttributeString("Extension", "xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/xl/workbook.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/docProps/app.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.extended-properties+xml");
		xmlTextWriter.WriteEndElement();
		for (int num = strSheetsName.Count; num > 0; num--)
		{
			xmlTextWriter.WriteStartElement("Override");
			xmlTextWriter.WriteAttributeString("PartName", "/xl/worksheets/sheet" + num + ".xml");
			xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/xl/sharedStrings.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Override");
		xmlTextWriter.WriteAttributeString("PartName", "/docProps/core.xml");
		xmlTextWriter.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.core-properties+xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
	}

	private void WriteXlRels(Stream output, IList<string> strSheetsName)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(output, Encoding.UTF8);
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("Relationships");
		xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");
		for (int num = strSheetsName.Count; num > 0; num--)
		{
			xmlTextWriter.WriteStartElement("Relationship");
			xmlTextWriter.WriteAttributeString("Id", "rId" + num);
			xmlTextWriter.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
			xmlTextWriter.WriteAttributeString("Target", "worksheets/sheet" + num + ".xml");
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteStartElement("Relationship");
		xmlTextWriter.WriteAttributeString("Id", "rId" + (strSheetsName.Count + 3));
		xmlTextWriter.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings");
		xmlTextWriter.WriteAttributeString("Target", "sharedStrings.xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Relationship");
		xmlTextWriter.WriteAttributeString("Id", "rId" + (strSheetsName.Count + 2));
		xmlTextWriter.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
		xmlTextWriter.WriteAttributeString("Target", "styles.xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("Relationship");
		xmlTextWriter.WriteAttributeString("Id", "rId" + (strSheetsName.Count + 1));
		xmlTextWriter.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
		xmlTextWriter.WriteAttributeString("Target", "theme/theme1.xml");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
	}

	private void WriteWorkBook(Stream output, IList<string> strSheetsName)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(output, Encoding.UTF8);
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("workbook");
		xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlTextWriter.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
		xmlTextWriter.WriteStartElement("fileVersion");
		xmlTextWriter.WriteAttributeString("rupBuild", "4505");
		xmlTextWriter.WriteAttributeString("lowestEdited", "4");
		xmlTextWriter.WriteAttributeString("lastEdited", "4");
		xmlTextWriter.WriteAttributeString("appName", "xl");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("workbookPr");
		xmlTextWriter.WriteAttributeString("defaultThemeVersion", "124226");
		xmlTextWriter.WriteAttributeString("filterPrivacy", "1");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("bookViews");
		xmlTextWriter.WriteStartElement("workbookView");
		xmlTextWriter.WriteAttributeString("windowHeight", "8010");
		xmlTextWriter.WriteAttributeString("windowWidth", "14805");
		xmlTextWriter.WriteAttributeString("yWindow", "105");
		xmlTextWriter.WriteAttributeString("xWindow", "240");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("sheets");
		int num = 0;
		foreach (string item in strSheetsName)
		{
			num++;
			xmlTextWriter.WriteStartElement("sheet");
			xmlTextWriter.WriteAttributeString("name", item);
			xmlTextWriter.WriteAttributeString("sheetId", num.ToString());
			xmlTextWriter.WriteAttributeString("r:id", "rId" + num);
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("calcPr");
		xmlTextWriter.WriteAttributeString("calcId", "124519");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
	}

	private IList<string> CreateStringTables(GridView[] vws, out IDictionary<string, int> lookupTable)
	{
		List<string> list = new List<string>();
		lookupTable = new Dictionary<string, int>();
		foreach (GridView gridView in vws)
		{
			GridColumnReadOnlyCollection visibleColumns = gridView.VisibleColumns;
			for (int j = 0; j < visibleColumns.Count; j++)
			{
				string text = ((!string.IsNullOrEmpty(visibleColumns[j].Caption)) ? visibleColumns[j].Caption : visibleColumns[j].FieldName);
				if (!lookupTable.ContainsKey(text))
				{
					lookupTable.Add(text, list.Count);
					list.Add(text);
				}
			}
			for (int k = 0; k < gridView.DataRowCount; k++)
			{
				DataRowView dataRowView = (DataRowView)gridView.GetRow(k);
				for (int l = 0; l < visibleColumns.Count; l++)
				{
					if ((object)dataRowView[visibleColumns[l].FieldName].GetType() == typeof(string) && dataRowView[visibleColumns[l].FieldName] != DBNull.Value)
					{
						string text2 = (string)dataRowView[visibleColumns[l].FieldName];
						if (!lookupTable.ContainsKey(text2))
						{
							lookupTable.Add(text2, list.Count);
							list.Add(text2);
						}
					}
				}
			}
		}
		return list;
	}

	private void WriteStringTable(Stream output, IList<string> stringTable)
	{
		using XmlWriter xmlWriter = XmlWriter.Create(output);
		xmlWriter.WriteStartDocument(standalone: true);
		xmlWriter.WriteStartElement("sst", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlWriter.WriteAttributeString("count", stringTable.Count.ToString());
		xmlWriter.WriteAttributeString("uniqueCount", stringTable.Count.ToString());
		foreach (string item in stringTable)
		{
			xmlWriter.WriteStartElement("si");
			xmlWriter.WriteElementString("t", item);
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteEndElement();
	}

	private string RowColumnToPosition(int row, int column)
	{
		return ColumnIndexToName(column) + RowIndexToName(row);
	}

	private string ColumnIndexToName(int columnIndex)
	{
		char c = (char)(65 + columnIndex % 26);
		columnIndex /= 26;
		if (columnIndex == 0)
		{
			return c.ToString();
		}
		return ((char)(64 + columnIndex)).ToString() + c;
	}

	private string RowIndexToName(int rowIndex)
	{
		return (rowIndex + 1).ToString();
	}

	private void WriteWorksheet(Stream output, GridView vw, IDictionary<string, int> lookupTable)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(output, Encoding.UTF8);
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("worksheet");
		xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlTextWriter.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
		xmlTextWriter.WriteStartElement("dimension");
		string text = RowColumnToPosition(vw.DataRowCount - 1, vw.VisibleColumns.Count - 1);
		xmlTextWriter.WriteAttributeString("ref", "A1:" + text);
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("sheetViews");
		xmlTextWriter.WriteStartElement("sheetView");
		xmlTextWriter.WriteAttributeString("tabSelected", "1");
		xmlTextWriter.WriteAttributeString("workbookViewId", "0");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("sheetFormatPr");
		xmlTextWriter.WriteAttributeString("defaultRowHeight", "15");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("sheetData");
		WriteWorksheetData(xmlTextWriter, vw, lookupTable);
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteStartElement("pageMargins");
		xmlTextWriter.WriteAttributeString("left", "0.7");
		xmlTextWriter.WriteAttributeString("right", "0.7");
		xmlTextWriter.WriteAttributeString("top", "0.75");
		xmlTextWriter.WriteAttributeString("bottom", "0.75");
		xmlTextWriter.WriteAttributeString("header", "0.3");
		xmlTextWriter.WriteAttributeString("footer", "0.3");
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
	}

	private void WriteWorksheetData(XmlTextWriter writer, GridView vw, IDictionary<string, int> lookupTable)
	{
		GridColumnReadOnlyCollection visibleColumns = vw.VisibleColumns;
		int dataRowCount = vw.DataRowCount;
		int count = visibleColumns.Count;
		writer.WriteStartElement("row");
		string value = RowIndexToName(0);
		writer.WriteAttributeString("r", value);
		writer.WriteAttributeString("spans", "1:" + count);
		for (int i = 0; i < count; i++)
		{
			object obj = ((!string.IsNullOrEmpty(visibleColumns[i].Caption)) ? visibleColumns[i].Caption : visibleColumns[i].FieldName);
			writer.WriteStartElement("c");
			value = RowColumnToPosition(0, i);
			writer.WriteAttributeString("r", value);
			if (obj is string key)
			{
				writer.WriteAttributeString("t", "s");
				obj = lookupTable[key];
			}
			writer.WriteElementString("v", obj.ToString());
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
		for (int j = 1; j < dataRowCount + 1; j++)
		{
			writer.WriteStartElement("row");
			value = RowIndexToName(j);
			writer.WriteAttributeString("r", value);
			writer.WriteAttributeString("spans", "1:" + count);
			DataRowView dataRowView = (DataRowView)vw.GetRow(j - 1);
			for (int k = 0; k < count; k++)
			{
				object obj2 = dataRowView[visibleColumns[k].FieldName];
				writer.WriteStartElement("c");
				value = RowColumnToPosition(j, k);
				writer.WriteAttributeString("r", value);
				if (obj2 is string key2)
				{
					writer.WriteAttributeString("t", "s");
					obj2 = lookupTable[key2];
				}
				writer.WriteElementString("v", obj2.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}

	private void chi32EN_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (chi32EN.Checked)
		{
			LMOracle.Version = (Version = 2);
			Registry.SetRegistryValue("Oracle", "Version", "2");
			chi40EN.Checked = false;
			chi40RU.Checked = false;
		}
		else if (!chi40EN.Checked && !chi40RU.Checked)
		{
			chi32EN.Checked = true;
		}
	}

	private void chi40EN_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (chi40EN.Checked)
		{
			LMOracle.Version = (Version = 1);
			Registry.SetRegistryValue("Oracle", "Version", "1");
			chi32EN.Checked = false;
			chi40RU.Checked = false;
		}
		else if (!chi32EN.Checked && !chi40RU.Checked)
		{
			chi40EN.Checked = true;
		}
	}

	private void chi40RU_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (chi40RU.Checked)
		{
			LMOracle.Version = (Version = 0);
			Registry.SetRegistryValue("Oracle", "Version", "0");
			chi32EN.Checked = false;
			chi40EN.Checked = false;
		}
		else if (!chi40EN.Checked && !chi32EN.Checked)
		{
			chi40RU.Checked = true;
		}
	}

	private void biDepth_EditValueChanged(object sender, EventArgs e)
	{
		LMOracle.MaxLevel = (MaxLevel = int.Parse(biDepth.EditValue.ToString()));
		Registry.SetRegistryValue("Oracle", "Depth", MaxLevel.ToString());
	}

	private void chiFree_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		CommonSetting.LMOracleFree = chiFree.Checked;
		if (chiFree.Checked)
		{
			Registry.SetRegistryValue("Oracle", "WithoutGame", "1");
			bsiVerOracle.Enabled = false;
		}
		else
		{
			Registry.SetRegistryValue("Oracle", "WithoutGame", "0");
			bsiVerOracle.Enabled = true;
		}
	}

	private int[] GetVerObjName(out string path)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[2];
		path = "";
		int num = 3;
		int num2 = 0;
		if (Registry.RegistryKeyExist("ObjectName"))
		{
			try
			{
				num = int.Parse(Registry.GetRegistryValue("ObjectName", "Version"));
				if (num < 0 || num > 4)
				{
					num = 3;
				}
				if (num == 4)
				{
					num2 = int.Parse(Registry.GetRegistryValue("ObjectName", "CodePage"));
					if (num2 < 0 || num2 > 2)
					{
						num2 = 0;
					}
					path = Registry.GetRegistryValue("ObjectName", "Path");
					if (path != "" && !File.Exists(path))
					{
						if (TblArt == null)
						{
							MessageBox.Show(sFile + " " + path + " " + errorObjectName, CommonSetting.MyName, (MessageBoxButtons)0, (MessageBoxIcon)16);
						}
						path = "";
					}
				}
			}
			catch
			{
			}
		}
		array[0] = num;
		array[1] = num2;
		return array;
	}

	private void ChangeObjName(int ver, string path, int CodePg)
	{
		switch (ver)
		{
		case 0:
			ObjNameSoDEn();
			break;
		case 1:
			ObjNameSoDRu();
			break;
		case 2:
			ObjNameComplete();
			break;
		case 3:
			ObjNameDefault();
			break;
		case 4:
			if (path != "")
			{
				ObjNameFromFile(path, CodePg);
			}
			else
			{
				ObjNameDefault();
			}
			break;
		}
		foreach (DataRow row in TblIdeology.Rows)
		{
			row["Class"] = aHeroClass[(int)row["ClassID"]];
			row["Ideology"] = aIdeology[int.Parse(row["Ideology"].ToString())];
		}
		LMOracle.aLevelSkill = aLevelSkill;
		LMOracle.TblSecondarySkill = TblSecondarySkill;
		if (tsButtonRefresh.Enabled)
		{
			tsButtonRefresh_ItemClick(new object(), null);
		}
	}

	private void CreateDefaultTable()
	{
		TblMonsters = new DataTable();
		TblArt = new DataTable();
		TblSpell = new DataTable();
		TblSecondarySkill = new DataTable();
		TblObject = new DataTable();
		TblDwelling = new DataTable();
		TblBuilding = new DataTable();
		TblIdeology = new DataTable();
		CreateTblMonster();
		CreateTblArt();
		CreateTblSpell();
		CreateTblSecondarySkill();
		CreateTblObject();
		CreateTblBuilding();
		CreateTblIdeology();
		CreateTblDwelling();
	}

	private void ModifyArtTbl()
	{
		foreach (DataRow row in TblArt.Rows)
		{
			if (row["Relic-C"] != DBNull.Value)
			{
				row["Relic-C"] = TblArt.Rows.Find(row["Relic-C"])["Name"];
			}
			if (row["Class"].ToString() != "")
			{
				row["Class"] = aArtClass[int.Parse(row["Class"].ToString())];
			}
		}
	}

	private void ModifyArtTblRelic()
	{
		foreach (DataRow row in TblArt.Rows)
		{
			if (row["Relic-C"] != DBNull.Value)
			{
				row["Relic-C"] = TblArt.Rows.Find(row["Relic-C"])["Name"];
			}
		}
	}

	private void ModifyArtTblClass()
	{
		foreach (DataRow row in TblArt.Rows)
		{
			if (row["Class"].ToString() != "")
			{
				row["Class"] = aArtClass[int.Parse(row["Class"].ToString())];
			}
		}
	}

	private void ObjNameFromFile(string path, int CodePg)
	{
		bool flag = false;
		bool flag2 = false;
		CreateDefaultTable();
		CreateDefaultArray();
		string[] array = File.ReadAllLines(path, CodePg switch
		{
			0 => Encoding.GetEncoding(1251), 
			1 => Encoding.GetEncoding(866), 
			_ => Encoding.UTF8, 
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == "[Art]")
			{
				if (!ArtSection(i + 1, array))
				{
					ModifyArtTblRelic();
				}
				flag = true;
			}
			if (array[i] == "[Monstr]")
			{
				MonstrSection(i + 1, array);
			}
			if (array[i] == "[Spell]")
			{
				SpellSection(i + 1, array);
			}
			if (array[i] == "[Skill]")
			{
				SkillSection(i + 1, array);
			}
			if (array[i] == "[Dwelling]")
			{
				DwellingSection(i + 1, array);
			}
			if (array[i] == "[Object]")
			{
				ObjectSection(i + 1, array);
			}
			if (array[i] == "[ArtClass]")
			{
				if (!ArtClassSection(i + 1, array))
				{
					ModifyArtTblClass();
				}
				flag2 = true;
			}
			if (array[i] == "[Mine]")
			{
				MineSection(i + 1, array);
			}
			if (array[i] == "[Locality]")
			{
				LocalitySection(i + 1, array);
			}
			if (array[i] == "[LevelSkill]")
			{
				LevelSkillSection(i + 1, array);
			}
			if (array[i] == "[FullLevelSkill]")
			{
				FullLevelSkillSection(i + 1, array);
			}
			if (array[i] == "[Color]")
			{
				ColorSection(i + 1, array);
			}
			if (array[i] == "[Reply]")
			{
				ReplySection(i + 1, array);
			}
			if (array[i] == "[Town]")
			{
				TownSection(i + 1, array);
			}
			if (array[i] == "[Doll]")
			{
				DollSection(i + 1, array);
			}
			if (array[i] == "[Place]")
			{
				PlaceSection(i + 1, array);
			}
			if (array[i] == "[Tent]")
			{
				TentSection(i + 1, array);
			}
			if (array[i] == "[Status]")
			{
				StatusSection(i + 1, array);
			}
			if (array[i] == "[Quest]")
			{
				QuestSection(i + 1, array);
			}
			if (array[i] == "[Reward]")
			{
				RewardSection(i + 1, array);
			}
			if (array[i] == "[HeroClass]")
			{
				HeroClassSection(i + 1, array);
			}
			if (array[i] == "[Ideology]")
			{
				IdeologySection(i + 1, array);
			}
		}
		if (!flag)
		{
			ModifyArtTblRelic();
		}
		if (!flag2)
		{
			ModifyArtTblClass();
		}
		TblMonsters.Rows[145]["Name"] = TblArt.Rows[3]["Name"];
		TblMonsters.Rows[146]["Name"] = TblArt.Rows[4]["Name"];
		TblMonsters.Rows[147]["Name"] = TblArt.Rows[6]["Name"];
		TblMonsters.Rows[148]["Name"] = TblArt.Rows[5]["Name"];
	}

	private bool ArtClassSection(int start, string[] strs)
	{
		bool result = false;
		int num = aArtClass.Length;
		string[] array = new string[num];
		int num2 = 0;
		for (int i = start; i < strs.Length; i++)
		{
			string text = strs[i];
			if (text.IndexOf("[") > -1 && text.IndexOf("]") > -1)
			{
				break;
			}
			if (text != "")
			{
				int num3 = text.IndexOf(" ");
				if (num3 < 0)
				{
					break;
				}
				array[num2] = text.Substring(num3).Trim();
				if (++num2 == num)
				{
					break;
				}
			}
		}
		if (num2 == num)
		{
			for (int j = 0; j < num; j++)
			{
				aArtClass[j] = array[j];
			}
			ModifyArtTblClass();
			result = true;
		}
		return result;
	}

	private void MineSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aMine);
	}

	private void LocalitySection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aLocality);
	}

	private void LevelSkillSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aLevelSkill);
	}

	private void FullLevelSkillSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aFullLvlSkill);
	}

	private void ColorSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aColor);
	}

	private void ReplySection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aReply);
	}

	private void TownSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aTown);
	}

	private void DollSection(int start, string[] strs)
	{
		string[] array = new string[11];
		SectionProcArray(start, strs, array);
		if (array[0] != null)
		{
			for (int i = 0; i < 9; i++)
			{
				aDoll[i] = array[i];
			}
			aDoll[9] = array[9] + "1";
			aDoll[10] = array[9] + "2";
			aDoll[11] = array[9] + "3";
			aDoll[12] = array[9] + "4";
			aDoll[18] = array[9] + "5";
			aDoll[19] = array[10];
		}
	}

	private void PlaceSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aPlace);
	}

	private void TentSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aTent);
	}

	private void StatusSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aStatus);
	}

	private void QuestSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, аQuest);
	}

	private void RewardSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aReward);
	}

	private void HeroClassSection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aHeroClass);
	}

	private void IdeologySection(int start, string[] strs)
	{
		SectionProcArray(start, strs, aIdeology);
	}

	private bool ArtSection(int start, string[] strs)
	{
		bool result = false;
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Name", Type.GetType("System.String"));
		for (int i = start; i < strs.Length; i++)
		{
			string text = strs[i];
			if (text.IndexOf("[") > -1 && text.IndexOf("]") > -1)
			{
				break;
			}
			if (text != "")
			{
				int num = text.IndexOf(" ");
				if (num < 0)
				{
					break;
				}
				dataTable.Rows.Add(text.Substring(num).Trim());
				if (dataTable.Rows.Count == 141)
				{
					break;
				}
			}
		}
		if (dataTable.Rows.Count == 141)
		{
			ReplaceName(TblArt, dataTable);
			ModifyArtTblRelic();
			result = true;
		}
		return result;
	}

	private void MonstrSection(int start, string[] strs)
	{
		SectionProcTable(start, strs, TblMonsters, 145);
	}

	private void SpellSection(int start, string[] strs)
	{
		SectionProcTable(start, strs, TblSpell, 71);
	}

	private void SkillSection(int start, string[] strs)
	{
		SectionProcTable(start, strs, TblSecondarySkill, 28);
	}

	private void DwellingSection(int start, string[] strs)
	{
		SectionProcTable(start, strs, TblDwelling, 37);
	}

	private void ObjectSection(int start, string[] strs)
	{
		SectionProcTable(start, strs, TblObject, 57);
	}

	private void SectionProcArray(int start, string[] strs, string[] ArrObj)
	{
		int num = ArrObj.Length;
		string[] array = new string[num];
		int num2 = 0;
		for (int i = start; i < strs.Length; i++)
		{
			string text = strs[i];
			if (text.IndexOf("[") > -1 && text.IndexOf("]") > -1)
			{
				break;
			}
			if (text != "")
			{
				int num3 = text.IndexOf(" ");
				if (num3 < 0)
				{
					break;
				}
				array[num2] = text.Substring(num3).Trim();
				if (++num2 == num)
				{
					break;
				}
			}
		}
		if (num2 == num)
		{
			for (int j = 0; j < num; j++)
			{
				ArrObj[j] = array[j];
			}
		}
	}

	private void SectionProcTable(int start, string[] strs, DataTable TblObj, int count)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Name", Type.GetType("System.String"));
		for (int i = start; i < strs.Length; i++)
		{
			string text = strs[i];
			if (text.IndexOf("[") > -1 && text.IndexOf("]") > -1)
			{
				break;
			}
			if (text != "")
			{
				int num = text.IndexOf(" ");
				if (num < 0)
				{
					break;
				}
				dataTable.Rows.Add(text.Substring(num).Trim());
				if (dataTable.Rows.Count == count)
				{
					break;
				}
			}
		}
		if (dataTable.Rows.Count == count)
		{
			ReplaceName(TblObj, dataTable);
		}
	}

	private void ObjNameDefault()
	{
		CreateDefaultArray();
		CreateDefaultTable();
		ModifyArtTbl();
	}

	private void ObjNameSoDEn()
	{
		CreateDefaultTable();
		CreateArraySoDEn();
		if (TblArtSoDEn.Columns.Count == 0)
		{
			CreateTblArtSoDEn();
			CreateTblMonsterSoDEn();
			CreateTblSpellSoDEn();
			CreateTblSecondarySkillSoDEn();
			CreateTblDwellingSoDEn();
			CreateTblObjectSoDEn();
		}
		ReplaceName(TblArt, TblArtSoDEn);
		ModifyArtTbl();
		ReplaceName(TblMonsters, TblMonstersSoDEn);
		ReplaceName(TblSpell, TblSpellSoDEn);
		ReplaceName(TblSecondarySkill, TblSecondarySkillSoDEn);
		ReplaceName(TblDwelling, TblDwellingSoDEn);
		ReplaceName(TblObject, TblObjectSoDEn);
		foreach (DataRow row in TblBuilding.Rows)
		{
			row["Building"] = row["SoDEn"];
		}
	}

	private void ObjNameComplete()
	{
		CreateDefaultTable();
		CreateDefaultArray();
		CreateArrayComplete();
		if (TblArtComplete.Columns.Count == 0)
		{
			CreateTblArtComplete();
			CreateTblMonsterComplete();
			CreateTblSpellComplete();
			CreateTblSecondarySkillComplete();
			CreateTblDwellingComplete();
			CreateTblObjectComplete();
		}
		ReplaceName(TblArt, TblArtComplete);
		ModifyArtTbl();
		ReplaceName(TblMonsters, TblMonstersComplete);
		ReplaceName(TblSpell, TblSpellComplete);
		ReplaceName(TblSecondarySkill, TblSecondarySkillComplete);
		ReplaceName(TblDwelling, TblDwellingComplete);
		ReplaceName(TblObject, TblObjectComplete);
		foreach (DataRow row in TblBuilding.Rows)
		{
			if (row["Complete"].ToString() != "")
			{
				row["Building"] = row["Complete"];
			}
		}
	}

	private void ObjNameSoDRu()
	{
		CreateDefaultTable();
		CreateDefaultArray();
		CreateArraySoDRu();
		if (TblArtSoDRu.Columns.Count == 0)
		{
			CreateTblArtSoDRu();
			CreateTblMonsterSoDRu();
			CreateTblSpellSoDRu();
			CreateTblSecondarySkillSoDRu();
			CreateTblDwellingSoDRu();
			CreateTblObjectSoDRu();
		}
		ReplaceName(TblArt, TblArtSoDRu);
		ModifyArtTbl();
		ReplaceName(TblMonsters, TblMonstersSoDRu);
		ReplaceName(TblSpell, TblSpellSoDRu);
		ReplaceName(TblSecondarySkill, TblSecondarySkillSoDRu);
		ReplaceName(TblDwelling, TblDwellingSoDRu);
		ReplaceName(TblObject, TblObjectSoDRu);
	}

	private void ReplaceName(DataTable s, DataTable r)
	{
		int count = r.Rows.Count;
		for (int i = 0; i < count; i++)
		{
			s.Rows[i]["Name"] = r.Rows[i]["Name"];
		}
	}

	private void barManager1_HighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
	{
		if (e.Link == null)
		{
			return;
		}
		timer2.Stop();
		if (e.Link.Item.Tag != null)
		{
			if (ntoolt != null)
			{
				((Control)ntoolt).Hide();
			}
			else
			{
				ntoolt = new myToolTip();
			}
			((Control)ntoolt).Tag = e.Link.Item.Id + "|" + e.Link.Item.Hint;
			timer2.Start();
		}
		else if (ntoolt != null)
		{
			((Control)ntoolt).Hide();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (ntoolt != null && ((Control)ntoolt).Visible)
		{
			if ((int)timer1.Tag <= 100)
			{
				timer1.Tag = (int)timer1.Tag + 1;
				return;
			}
			timer1.Stop();
			((Control)ntoolt).Hide();
		}
		else
		{
			timer1.Stop();
		}
	}

	private void ppmFile_CloseUp(object sender, EventArgs e)
	{
		timer1.Stop();
		timer2.Stop();
		if (ntoolt != null)
		{
			((Control)ntoolt).Hide();
			((Form)ntoolt).Close();
			ntoolt = null;
		}
	}

	private void timer2_Tick(object sender, EventArgs e)
	{
		timer2.Stop();
		if (ntoolt != null)
		{
			BarButtonItemLink barButtonItemLink = (BarButtonItemLink)ppmFile.Activator;
			string[] array = ((Control)ntoolt).Tag.ToString().Split(new char[1] { '|' });
			int num = int.Parse(array[0]);
			Point point = new Point(barButtonItemLink.Item.DropDownControl.Bounds.X + barButtonItemLink.Item.DropDownControl.Bounds.Width, barButtonItemLink.Item.DropDownControl.Bounds.Y + barButtonItemLink.Item.DropDownControl.IPopup.PopupOwnerRectangle.Height * num);
			((Control)ntoolt.label1).Text = array[1];
			((Form)ntoolt).Location = new Point(point.X + 3, point.Y + barButtonItemLink.Item.DropDownControl.IPopup.PopupOwnerRectangle.Height * 2 + num * 2);
			((Control)ntoolt).Show();
			timer1.Tag = 0;
			timer1.Start();
		}
	}
}
