using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ProspectorRT;

[Serializable]
[HelpKeyword("vs.data.DataSet")]
[DesignerCategory("code")]
[XmlRoot("DataSet2")]
[XmlSchemaProvider("GetTypedDataSetSchema")]
[ToolboxItem(true)]
public class DataSet2 : DataSet
{
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_MonstrRowChangeEventHandler(object sender, R_MonstrRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_ArtRowChangeEventHandler(object sender, R_ArtRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_BankRowChangeEventHandler(object sender, R_BankRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_EventBoxRowChangeEventHandler(object sender, R_EventBoxRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_ScholarRowChangeEventHandler(object sender, R_ScholarRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_ResourceRowChangeEventHandler(object sender, R_ResourceRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_SpellRowChangeEventHandler(object sender, R_SpellRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_ChestRowChangeEventHandler(object sender, R_ChestRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_SkillRowChangeEventHandler(object sender, R_SkillRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_CampRowChangeEventHandler(object sender, R_CampRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_MarketRowChangeEventHandler(object sender, R_MarketRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_GarrisonRowChangeEventHandler(object sender, R_GarrisonRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_SeerHutRowChangeEventHandler(object sender, R_SeerHutRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_PrisonRowChangeEventHandler(object sender, R_PrisonRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_ObjectRowChangeEventHandler(object sender, R_ObjectRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_PassGuardRowChangeEventHandler(object sender, R_PassGuardRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_HeroesRowChangeEventHandler(object sender, R_HeroesRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_TownRowChangeEventHandler(object sender, R_TownRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_AllArtsRowChangeEventHandler(object sender, R_AllArtsRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_TopologyRowChangeEventHandler(object sender, R_TopologyRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_AllSpellRowChangeEventHandler(object sender, R_AllSpellRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_AllTimerRowChangeEventHandler(object sender, R_AllTimerRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_AllSkillRowChangeEventHandler(object sender, R_AllSkillRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_AllExperienceRowChangeEventHandler(object sender, R_AllExperienceRowChangeEvent e);

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public delegate void R_MineRowChangeEventHandler(object sender, R_MineRowChangeEvent e);

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_MonstrDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnName;

		private DataColumn columnNumber;

		private DataColumn columnMood;

		private DataColumn columnLevel;

		private DataColumn columnArt;

		private DataColumn columnGold;

		private DataColumn columnResource;

		private DataColumn columnIncrease;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn NameColumn => columnName;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn NumberColumn => columnNumber;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MoodColumn => columnMood;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LevelColumn => columnLevel;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ArtColumn => columnArt;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn IncreaseColumn => columnIncrease;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MonstrRow this[int index] => (R_MonstrRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MonstrRowChangeEventHandler R_MonstrRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MonstrRowChangeEventHandler R_MonstrRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MonstrRowChangeEventHandler R_MonstrRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MonstrRowChangeEventHandler R_MonstrRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MonstrDataTable()
		{
			base.TableName = "R_Monstr";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_MonstrDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_MonstrDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_MonstrRow(R_MonstrRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MonstrRow AddR_MonstrRow(int X, int Y, int Z, string Locality, string Name, int Number, int Mood, int Level, string Art, int Gold, string Resource, string Increase, int Address, int HP)
		{
			R_MonstrRow r_MonstrRow = (R_MonstrRow)NewRow();
			object[] itemArray = new object[14]
			{
				X, Y, Z, Locality, Name, Number, Mood, Level, Art, Gold,
				Resource, Increase, Address, HP
			};
			r_MonstrRow.ItemArray = itemArray;
			base.Rows.Add(r_MonstrRow);
			return r_MonstrRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MonstrRow FindByXYZ(int X, int Y, int Z)
		{
			return (R_MonstrRow)base.Rows.Find(new object[3] { X, Y, Z });
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_MonstrDataTable r_MonstrDataTable = (R_MonstrDataTable)base.Clone();
			r_MonstrDataTable.InitVars();
			return r_MonstrDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_MonstrDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnName = base.Columns["Name"];
			columnNumber = base.Columns["Number"];
			columnMood = base.Columns["Mood"];
			columnLevel = base.Columns["Level"];
			columnArt = base.Columns["Art"];
			columnGold = base.Columns["Gold"];
			columnResource = base.Columns["Resource"];
			columnIncrease = base.Columns["Increase"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnName = new DataColumn("Name", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnName);
			columnNumber = new DataColumn("Number", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnNumber);
			columnMood = new DataColumn("Mood", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnMood);
			columnLevel = new DataColumn("Level", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLevel);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnIncrease = new DataColumn("Increase", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnIncrease);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			base.Constraints.Add(new UniqueConstraint("R_MonstrPrKey", new DataColumn[3] { columnX, columnY, columnZ }, isPrimaryKey: true));
			columnX.AllowDBNull = false;
			columnY.AllowDBNull = false;
			columnZ.AllowDBNull = false;
			columnLocality.Caption = "Object";
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MonstrRow NewR_MonstrRow()
		{
			return (R_MonstrRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_MonstrRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_MonstrRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_MonstrRowChanged != null)
			{
				this.R_MonstrRowChanged(this, new R_MonstrRowChangeEvent((R_MonstrRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_MonstrRowChanging != null)
			{
				this.R_MonstrRowChanging(this, new R_MonstrRowChangeEvent((R_MonstrRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_MonstrRowDeleted != null)
			{
				this.R_MonstrRowDeleted(this, new R_MonstrRowChangeEvent((R_MonstrRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_MonstrRowDeleting != null)
			{
				this.R_MonstrRowDeleting(this, new R_MonstrRowChangeEvent((R_MonstrRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_MonstrRow(R_MonstrRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_MonstrDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_ArtDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnSlot;

		private DataColumn columnName;

		private DataColumn columnClass;

		private DataColumn _columnRelic_C;

		private DataColumn columnGold;

		private DataColumn columnResource;

		private DataColumn columnGuard;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SlotColumn => columnSlot;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn NameColumn => columnName;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ClassColumn => columnClass;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn _Relic_CColumn => _columnRelic_C;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GoldColumn => columnGold;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ResourceColumn => columnResource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[DebuggerNonUserCode]
		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ArtRow this[int index] => (R_ArtRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ArtRowChangeEventHandler R_ArtRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ArtRowChangeEventHandler R_ArtRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ArtRowChangeEventHandler R_ArtRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ArtRowChangeEventHandler R_ArtRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ArtDataTable()
		{
			base.TableName = "R_Art";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ArtDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_ArtDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_ArtRow(R_ArtRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ArtRow AddR_ArtRow(int X, int Y, int Z, string Locality, string Object, int Slot, string Name, string Class, string _Relic_C, int Gold, string Resource, string Guard, int Address, int HP)
		{
			R_ArtRow r_ArtRow = (R_ArtRow)NewRow();
			object[] itemArray = new object[14]
			{
				X, Y, Z, Locality, Object, Slot, Name, Class, _Relic_C, Gold,
				Resource, Guard, Address, HP
			};
			r_ArtRow.ItemArray = itemArray;
			base.Rows.Add(r_ArtRow);
			return r_ArtRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_ArtDataTable r_ArtDataTable = (R_ArtDataTable)base.Clone();
			r_ArtDataTable.InitVars();
			return r_ArtDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_ArtDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnSlot = base.Columns["Slot"];
			columnName = base.Columns["Name"];
			columnClass = base.Columns["Class"];
			_columnRelic_C = base.Columns["Relic-C"];
			columnGold = base.Columns["Gold"];
			columnResource = base.Columns["Resource"];
			columnGuard = base.Columns["Guard"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnName = new DataColumn("Name", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnName);
			columnClass = new DataColumn("Class", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnClass);
			_columnRelic_C = new DataColumn("Relic-C", typeof(string), null, MappingType.Element);
			_columnRelic_C.ExtendedProperties.Add("Generator_ColumnVarNameInTable", "_columnRelic_C");
			_columnRelic_C.ExtendedProperties.Add("Generator_UserColumnName", "Relic-C");
			base.Columns.Add(_columnRelic_C);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ArtRow NewR_ArtRow()
		{
			return (R_ArtRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_ArtRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_ArtRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_ArtRowChanged != null)
			{
				this.R_ArtRowChanged(this, new R_ArtRowChangeEvent((R_ArtRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_ArtRowChanging != null)
			{
				this.R_ArtRowChanging(this, new R_ArtRowChangeEvent((R_ArtRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_ArtRowDeleted != null)
			{
				this.R_ArtRowDeleted(this, new R_ArtRowChangeEvent((R_ArtRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_ArtRowDeleting != null)
			{
				this.R_ArtRowDeleting(this, new R_ArtRowChangeEvent((R_ArtRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_ArtRow(R_ArtRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_ArtDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_BankDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnName;

		private DataColumn columnGuard;

		private DataColumn columnMonster;

		private DataColumn columnGold;

		private DataColumn columnResource;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn NameColumn => columnName;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GuardColumn => columnGuard;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MonsterColumn => columnMonster;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_BankRow this[int index] => (R_BankRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_BankRowChangeEventHandler R_BankRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_BankRowChangeEventHandler R_BankRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_BankRowChangeEventHandler R_BankRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_BankRowChangeEventHandler R_BankRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_BankDataTable()
		{
			base.TableName = "R_Bank";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_BankDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_BankDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_BankRow(R_BankRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_BankRow AddR_BankRow(int X, int Y, int Z, string Locality, string Name, string Guard, string Monster, int Gold, string Resource, int Address, int HP)
		{
			R_BankRow r_BankRow = (R_BankRow)NewRow();
			object[] itemArray = new object[11]
			{
				X, Y, Z, Locality, Name, Guard, Monster, Gold, Resource, Address,
				HP
			};
			r_BankRow.ItemArray = itemArray;
			base.Rows.Add(r_BankRow);
			return r_BankRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_BankDataTable r_BankDataTable = (R_BankDataTable)base.Clone();
			r_BankDataTable.InitVars();
			return r_BankDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_BankDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnName = base.Columns["Name"];
			columnGuard = base.Columns["Guard"];
			columnMonster = base.Columns["Monster"];
			columnGold = base.Columns["Gold"];
			columnResource = base.Columns["Resource"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnName = new DataColumn("Name", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnName);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_BankRow NewR_BankRow()
		{
			return (R_BankRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_BankRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_BankRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_BankRowChanged != null)
			{
				this.R_BankRowChanged(this, new R_BankRowChangeEvent((R_BankRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_BankRowChanging != null)
			{
				this.R_BankRowChanging(this, new R_BankRowChangeEvent((R_BankRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_BankRowDeleted != null)
			{
				this.R_BankRowDeleted(this, new R_BankRowChangeEvent((R_BankRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_BankRowDeleting != null)
			{
				this.R_BankRowDeleting(this, new R_BankRowChangeEvent((R_BankRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_BankRow(R_BankRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_BankDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_EventBoxDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnGuard;

		private DataColumn columnExperience;

		private DataColumn columnMana;

		private DataColumn columnMorale;

		private DataColumn columnLuck;

		private DataColumn columnGold;

		private DataColumn columnResource;

		private DataColumn columnPrimarySkill;

		private DataColumn columnSecondarySkill;

		private DataColumn columnArtefact;

		private DataColumn columnSpell;

		private DataColumn columnMonster;

		private DataColumn columnHP;

		private DataColumn columnAddress;

		private DataColumn columnApply;

		private DataColumn columnRepeat;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GuardColumn => columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ExperienceColumn => columnExperience;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ManaColumn => columnMana;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MoraleColumn => columnMorale;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LuckColumn => columnLuck;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PrimarySkillColumn => columnPrimarySkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SecondarySkillColumn => columnSecondarySkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ArtefactColumn => columnArtefact;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SpellColumn => columnSpell;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MonsterColumn => columnMonster;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ApplyColumn => columnApply;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn RepeatColumn => columnRepeat;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_EventBoxRow this[int index] => (R_EventBoxRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_EventBoxRowChangeEventHandler R_EventBoxRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_EventBoxRowChangeEventHandler R_EventBoxRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_EventBoxRowChangeEventHandler R_EventBoxRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_EventBoxRowChangeEventHandler R_EventBoxRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_EventBoxDataTable()
		{
			base.TableName = "R_EventBox";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_EventBoxDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_EventBoxDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_EventBoxRow(R_EventBoxRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_EventBoxRow AddR_EventBoxRow(int X, int Y, int Z, string Locality, string Object, string Guard, int Experience, int Mana, string Morale, string Luck, int Gold, string Resource, string PrimarySkill, string SecondarySkill, string Artefact, string Spell, string Monster, int HP, int Address, string Apply, string Repeat)
		{
			R_EventBoxRow r_EventBoxRow = (R_EventBoxRow)NewRow();
			object[] itemArray = new object[21]
			{
				X, Y, Z, Locality, Object, Guard, Experience, Mana, Morale, Luck,
				Gold, Resource, PrimarySkill, SecondarySkill, Artefact, Spell, Monster, HP, Address, Apply,
				Repeat
			};
			r_EventBoxRow.ItemArray = itemArray;
			base.Rows.Add(r_EventBoxRow);
			return r_EventBoxRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_EventBoxDataTable r_EventBoxDataTable = (R_EventBoxDataTable)base.Clone();
			r_EventBoxDataTable.InitVars();
			return r_EventBoxDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_EventBoxDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnGuard = base.Columns["Guard"];
			columnExperience = base.Columns["Experience"];
			columnMana = base.Columns["Mana"];
			columnMorale = base.Columns["Morale"];
			columnLuck = base.Columns["Luck"];
			columnGold = base.Columns["Gold"];
			columnResource = base.Columns["Resource"];
			columnPrimarySkill = base.Columns["PrimarySkill"];
			columnSecondarySkill = base.Columns["SecondarySkill"];
			columnArtefact = base.Columns["Artefact"];
			columnSpell = base.Columns["Spell"];
			columnMonster = base.Columns["Monster"];
			columnHP = base.Columns["HP"];
			columnAddress = base.Columns["Address"];
			columnApply = base.Columns["Apply"];
			columnRepeat = base.Columns["Repeat"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnExperience = new DataColumn("Experience", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnExperience);
			columnMana = new DataColumn("Mana", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnMana);
			columnMorale = new DataColumn("Morale", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMorale);
			columnLuck = new DataColumn("Luck", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLuck);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnPrimarySkill = new DataColumn("PrimarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPrimarySkill);
			columnSecondarySkill = new DataColumn("SecondarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSecondarySkill);
			columnArtefact = new DataColumn("Artefact", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArtefact);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnApply = new DataColumn("Apply", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnApply);
			columnRepeat = new DataColumn("Repeat", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnRepeat);
			columnMana.Caption = "DataColumn1";
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_EventBoxRow NewR_EventBoxRow()
		{
			return (R_EventBoxRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_EventBoxRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_EventBoxRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_EventBoxRowChanged != null)
			{
				this.R_EventBoxRowChanged(this, new R_EventBoxRowChangeEvent((R_EventBoxRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_EventBoxRowChanging != null)
			{
				this.R_EventBoxRowChanging(this, new R_EventBoxRowChangeEvent((R_EventBoxRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_EventBoxRowDeleted != null)
			{
				this.R_EventBoxRowDeleted(this, new R_EventBoxRowChangeEvent((R_EventBoxRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_EventBoxRowDeleting != null)
			{
				this.R_EventBoxRowDeleting(this, new R_EventBoxRowChangeEvent((R_EventBoxRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_EventBoxRow(R_EventBoxRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_EventBoxDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_ScholarDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnSpell;

		private DataColumn columnPrimarySkill;

		private DataColumn columnSecondarySkill;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SpellColumn => columnSpell;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PrimarySkillColumn => columnPrimarySkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SecondarySkillColumn => columnSecondarySkill;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ScholarRow this[int index] => (R_ScholarRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ScholarRowChangeEventHandler R_ScholarRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ScholarRowChangeEventHandler R_ScholarRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ScholarRowChangeEventHandler R_ScholarRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ScholarRowChangeEventHandler R_ScholarRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ScholarDataTable()
		{
			base.TableName = "R_Scholar";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ScholarDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_ScholarDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_ScholarRow(R_ScholarRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ScholarRow AddR_ScholarRow(int X, int Y, int Z, string Locality, string Spell, string PrimarySkill, string SecondarySkill)
		{
			R_ScholarRow r_ScholarRow = (R_ScholarRow)NewRow();
			object[] itemArray = new object[7] { X, Y, Z, Locality, Spell, PrimarySkill, SecondarySkill };
			r_ScholarRow.ItemArray = itemArray;
			base.Rows.Add(r_ScholarRow);
			return r_ScholarRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_ScholarDataTable r_ScholarDataTable = (R_ScholarDataTable)base.Clone();
			r_ScholarDataTable.InitVars();
			return r_ScholarDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_ScholarDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnSpell = base.Columns["Spell"];
			columnPrimarySkill = base.Columns["PrimarySkill"];
			columnSecondarySkill = base.Columns["SecondarySkill"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnPrimarySkill = new DataColumn("PrimarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPrimarySkill);
			columnSecondarySkill = new DataColumn("SecondarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSecondarySkill);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ScholarRow NewR_ScholarRow()
		{
			return (R_ScholarRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_ScholarRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_ScholarRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_ScholarRowChanged != null)
			{
				this.R_ScholarRowChanged(this, new R_ScholarRowChangeEvent((R_ScholarRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_ScholarRowChanging != null)
			{
				this.R_ScholarRowChanging(this, new R_ScholarRowChangeEvent((R_ScholarRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_ScholarRowDeleted != null)
			{
				this.R_ScholarRowDeleted(this, new R_ScholarRowChangeEvent((R_ScholarRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_ScholarRowDeleting != null)
			{
				this.R_ScholarRowDeleting(this, new R_ScholarRowChangeEvent((R_ScholarRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_ScholarRow(R_ScholarRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_ScholarDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_ResourceDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnResource;

		private DataColumn columnGold;

		private DataColumn columnGuard;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ResourceRow this[int index] => (R_ResourceRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ResourceRowChangeEventHandler R_ResourceRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ResourceRowChangeEventHandler R_ResourceRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ResourceRowChangeEventHandler R_ResourceRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ResourceRowChangeEventHandler R_ResourceRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ResourceDataTable()
		{
			base.TableName = "R_Resource";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ResourceDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_ResourceDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_ResourceRow(R_ResourceRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ResourceRow AddR_ResourceRow(int X, int Y, int Z, string Locality, string Object, string Resource, int Gold, string Guard, int Address, int HP)
		{
			R_ResourceRow r_ResourceRow = (R_ResourceRow)NewRow();
			object[] itemArray = new object[10] { X, Y, Z, Locality, Object, Resource, Gold, Guard, Address, HP };
			r_ResourceRow.ItemArray = itemArray;
			base.Rows.Add(r_ResourceRow);
			return r_ResourceRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_ResourceDataTable r_ResourceDataTable = (R_ResourceDataTable)base.Clone();
			r_ResourceDataTable.InitVars();
			return r_ResourceDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_ResourceDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnResource = base.Columns["Resource"];
			columnGold = base.Columns["Gold"];
			columnGuard = base.Columns["Guard"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ResourceRow NewR_ResourceRow()
		{
			return (R_ResourceRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_ResourceRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_ResourceRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_ResourceRowChanged != null)
			{
				this.R_ResourceRowChanged(this, new R_ResourceRowChangeEvent((R_ResourceRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_ResourceRowChanging != null)
			{
				this.R_ResourceRowChanging(this, new R_ResourceRowChangeEvent((R_ResourceRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_ResourceRowDeleted != null)
			{
				this.R_ResourceRowDeleted(this, new R_ResourceRowChangeEvent((R_ResourceRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_ResourceRowDeleting != null)
			{
				this.R_ResourceRowDeleting(this, new R_ResourceRowChangeEvent((R_ResourceRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_ResourceRow(R_ResourceRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_ResourceDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_SpellDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnSpell;

		private DataColumn columnGuard;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		private DataColumn columnLevel;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SpellColumn => columnSpell;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LevelColumn => columnLevel;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SpellRow this[int index] => (R_SpellRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SpellRowChangeEventHandler R_SpellRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SpellRowChangeEventHandler R_SpellRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SpellRowChangeEventHandler R_SpellRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SpellRowChangeEventHandler R_SpellRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SpellDataTable()
		{
			base.TableName = "R_Spell";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_SpellDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_SpellDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_SpellRow(R_SpellRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SpellRow AddR_SpellRow(int X, int Y, int Z, string Locality, string Object, string Spell, string Guard, int Address, int HP, int Level)
		{
			R_SpellRow r_SpellRow = (R_SpellRow)NewRow();
			object[] itemArray = new object[10] { X, Y, Z, Locality, Object, Spell, Guard, Address, HP, Level };
			r_SpellRow.ItemArray = itemArray;
			base.Rows.Add(r_SpellRow);
			return r_SpellRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_SpellDataTable r_SpellDataTable = (R_SpellDataTable)base.Clone();
			r_SpellDataTable.InitVars();
			return r_SpellDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_SpellDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnSpell = base.Columns["Spell"];
			columnGuard = base.Columns["Guard"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
			columnLevel = base.Columns["Level"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			columnLevel = new DataColumn("Level", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLevel);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SpellRow NewR_SpellRow()
		{
			return (R_SpellRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_SpellRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_SpellRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_SpellRowChanged != null)
			{
				this.R_SpellRowChanged(this, new R_SpellRowChangeEvent((R_SpellRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_SpellRowChanging != null)
			{
				this.R_SpellRowChanging(this, new R_SpellRowChangeEvent((R_SpellRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_SpellRowDeleted != null)
			{
				this.R_SpellRowDeleted(this, new R_SpellRowChangeEvent((R_SpellRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_SpellRowDeleting != null)
			{
				this.R_SpellRowDeleting(this, new R_SpellRowChangeEvent((R_SpellRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_SpellRow(R_SpellRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_SpellDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_ChestDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnArt;

		private DataColumn columnClass;

		private DataColumn _columnRelic_C;

		private DataColumn columnGold;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ArtColumn => columnArt;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ClassColumn => columnClass;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn _Relic_CColumn => _columnRelic_C;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ChestRow this[int index] => (R_ChestRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ChestRowChangeEventHandler R_ChestRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ChestRowChangeEventHandler R_ChestRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ChestRowChangeEventHandler R_ChestRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ChestRowChangeEventHandler R_ChestRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ChestDataTable()
		{
			base.TableName = "R_Chest";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ChestDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_ChestDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_ChestRow(R_ChestRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ChestRow AddR_ChestRow(int X, int Y, int Z, string Locality, string Object, string Art, string Class, string _Relic_C, int Gold, int Address, int HP)
		{
			R_ChestRow r_ChestRow = (R_ChestRow)NewRow();
			object[] itemArray = new object[11]
			{
				X, Y, Z, Locality, Object, Art, Class, _Relic_C, Gold, Address,
				HP
			};
			r_ChestRow.ItemArray = itemArray;
			base.Rows.Add(r_ChestRow);
			return r_ChestRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_ChestDataTable r_ChestDataTable = (R_ChestDataTable)base.Clone();
			r_ChestDataTable.InitVars();
			return r_ChestDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_ChestDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnArt = base.Columns["Art"];
			columnClass = base.Columns["Class"];
			_columnRelic_C = base.Columns["Relic-C"];
			columnGold = base.Columns["Gold"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnClass = new DataColumn("Class", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnClass);
			_columnRelic_C = new DataColumn("Relic-C", typeof(string), null, MappingType.Element);
			_columnRelic_C.ExtendedProperties.Add("Generator_ColumnVarNameInTable", "_columnRelic_C");
			_columnRelic_C.ExtendedProperties.Add("Generator_UserColumnName", "Relic-C");
			base.Columns.Add(_columnRelic_C);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ChestRow NewR_ChestRow()
		{
			return (R_ChestRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_ChestRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_ChestRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_ChestRowChanged != null)
			{
				this.R_ChestRowChanged(this, new R_ChestRowChangeEvent((R_ChestRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_ChestRowChanging != null)
			{
				this.R_ChestRowChanging(this, new R_ChestRowChangeEvent((R_ChestRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_ChestRowDeleted != null)
			{
				this.R_ChestRowDeleted(this, new R_ChestRowChangeEvent((R_ChestRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_ChestRowDeleting != null)
			{
				this.R_ChestRowDeleting(this, new R_ChestRowChangeEvent((R_ChestRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_ChestRow(R_ChestRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_ChestDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_SkillDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnAddress;

		private DataColumn columnSkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SkillColumn => columnSkill;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SkillRow this[int index] => (R_SkillRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SkillRowChangeEventHandler R_SkillRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SkillRowChangeEventHandler R_SkillRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SkillRowChangeEventHandler R_SkillRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SkillRowChangeEventHandler R_SkillRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SkillDataTable()
		{
			base.TableName = "R_Skill";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_SkillDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_SkillDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_SkillRow(R_SkillRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SkillRow AddR_SkillRow(int X, int Y, int Z, string Locality, string Object, int Address, string Skill)
		{
			R_SkillRow r_SkillRow = (R_SkillRow)NewRow();
			object[] itemArray = new object[7] { X, Y, Z, Locality, Object, Address, Skill };
			r_SkillRow.ItemArray = itemArray;
			base.Rows.Add(r_SkillRow);
			return r_SkillRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_SkillDataTable r_SkillDataTable = (R_SkillDataTable)base.Clone();
			r_SkillDataTable.InitVars();
			return r_SkillDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_SkillDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnAddress = base.Columns["Address"];
			columnSkill = base.Columns["Skill"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnSkill = new DataColumn("Skill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSkill);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SkillRow NewR_SkillRow()
		{
			return (R_SkillRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_SkillRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_SkillRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_SkillRowChanged != null)
			{
				this.R_SkillRowChanged(this, new R_SkillRowChangeEvent((R_SkillRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_SkillRowChanging != null)
			{
				this.R_SkillRowChanging(this, new R_SkillRowChangeEvent((R_SkillRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_SkillRowDeleted != null)
			{
				this.R_SkillRowDeleted(this, new R_SkillRowChangeEvent((R_SkillRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_SkillRowDeleting != null)
			{
				this.R_SkillRowDeleting(this, new R_SkillRowChangeEvent((R_SkillRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_SkillRow(R_SkillRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_SkillDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_CampDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnMonster;

		private DataColumn columnLevel;

		private DataColumn columnAddress;

		private DataColumn columnNumber;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MonsterColumn => columnMonster;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LevelColumn => columnLevel;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn NumberColumn => columnNumber;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_CampRow this[int index] => (R_CampRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_CampRowChangeEventHandler R_CampRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_CampRowChangeEventHandler R_CampRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_CampRowChangeEventHandler R_CampRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_CampRowChangeEventHandler R_CampRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_CampDataTable()
		{
			base.TableName = "R_Camp";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_CampDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_CampDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_CampRow(R_CampRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_CampRow AddR_CampRow(int X, int Y, int Z, string Locality, string Object, string Monster, int Level, int Address, int Number)
		{
			R_CampRow r_CampRow = (R_CampRow)NewRow();
			object[] itemArray = new object[9] { X, Y, Z, Locality, Object, Monster, Level, Address, Number };
			r_CampRow.ItemArray = itemArray;
			base.Rows.Add(r_CampRow);
			return r_CampRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_CampDataTable r_CampDataTable = (R_CampDataTable)base.Clone();
			r_CampDataTable.InitVars();
			return r_CampDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_CampDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnMonster = base.Columns["Monster"];
			columnLevel = base.Columns["Level"];
			columnAddress = base.Columns["Address"];
			columnNumber = base.Columns["Number"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnLevel = new DataColumn("Level", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLevel);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnNumber = new DataColumn("Number", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnNumber);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_CampRow NewR_CampRow()
		{
			return (R_CampRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_CampRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_CampRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_CampRowChanged != null)
			{
				this.R_CampRowChanged(this, new R_CampRowChangeEvent((R_CampRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_CampRowChanging != null)
			{
				this.R_CampRowChanging(this, new R_CampRowChangeEvent((R_CampRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_CampRowDeleted != null)
			{
				this.R_CampRowDeleted(this, new R_CampRowChangeEvent((R_CampRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_CampRowDeleting != null)
			{
				this.R_CampRowDeleting(this, new R_CampRowChangeEvent((R_CampRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_CampRow(R_CampRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_CampDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_MarketDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnSlot;

		private DataColumn columnArt;

		private DataColumn columnClass;

		private DataColumn _columnRelic_C;

		private DataColumn columnAddress;

		private DataColumn columnCost;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SlotColumn => columnSlot;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ArtColumn => columnArt;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ClassColumn => columnClass;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn _Relic_CColumn => _columnRelic_C;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn CostColumn => columnCost;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MarketRow this[int index] => (R_MarketRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MarketRowChangeEventHandler R_MarketRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MarketRowChangeEventHandler R_MarketRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MarketRowChangeEventHandler R_MarketRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MarketRowChangeEventHandler R_MarketRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MarketDataTable()
		{
			base.TableName = "R_Market";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_MarketDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_MarketDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_MarketRow(R_MarketRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MarketRow AddR_MarketRow(int X, int Y, int Z, string Locality, string Object, int Slot, string Art, string Class, string _Relic_C, int Address, int Cost)
		{
			R_MarketRow r_MarketRow = (R_MarketRow)NewRow();
			object[] itemArray = new object[11]
			{
				X, Y, Z, Locality, Object, Slot, Art, Class, _Relic_C, Address,
				Cost
			};
			r_MarketRow.ItemArray = itemArray;
			base.Rows.Add(r_MarketRow);
			return r_MarketRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_MarketDataTable r_MarketDataTable = (R_MarketDataTable)base.Clone();
			r_MarketDataTable.InitVars();
			return r_MarketDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_MarketDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnSlot = base.Columns["Slot"];
			columnArt = base.Columns["Art"];
			columnClass = base.Columns["Class"];
			_columnRelic_C = base.Columns["Relic-C"];
			columnAddress = base.Columns["Address"];
			columnCost = base.Columns["Cost"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnClass = new DataColumn("Class", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnClass);
			_columnRelic_C = new DataColumn("Relic-C", typeof(string), null, MappingType.Element);
			_columnRelic_C.ExtendedProperties.Add("Generator_ColumnVarNameInTable", "_columnRelic_C");
			_columnRelic_C.ExtendedProperties.Add("Generator_UserColumnName", "Relic-C");
			base.Columns.Add(_columnRelic_C);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnCost = new DataColumn("Cost", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnCost);
			columnArt.Caption = "Name";
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MarketRow NewR_MarketRow()
		{
			return (R_MarketRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_MarketRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_MarketRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_MarketRowChanged != null)
			{
				this.R_MarketRowChanged(this, new R_MarketRowChangeEvent((R_MarketRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_MarketRowChanging != null)
			{
				this.R_MarketRowChanging(this, new R_MarketRowChangeEvent((R_MarketRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_MarketRowDeleted != null)
			{
				this.R_MarketRowDeleted(this, new R_MarketRowChangeEvent((R_MarketRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_MarketRowDeleting != null)
			{
				this.R_MarketRowDeleting(this, new R_MarketRowChangeEvent((R_MarketRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_MarketRow(R_MarketRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_MarketDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_GarrisonDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnAntiMagic;

		private DataColumn columnGuard;

		private DataColumn columnColor;

		private DataColumn columnCanTake;

		private DataColumn columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AntiMagicColumn => columnAntiMagic;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn CanTakeColumn => columnCanTake;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HPColumn => columnHP;

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_GarrisonRow this[int index] => (R_GarrisonRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_GarrisonRowChangeEventHandler R_GarrisonRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_GarrisonRowChangeEventHandler R_GarrisonRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_GarrisonRowChangeEventHandler R_GarrisonRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_GarrisonRowChangeEventHandler R_GarrisonRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_GarrisonDataTable()
		{
			base.TableName = "R_Garrison";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_GarrisonDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_GarrisonDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_GarrisonRow(R_GarrisonRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_GarrisonRow AddR_GarrisonRow(int X, int Y, int Z, string Locality, string AntiMagic, string Guard, string Color, string CanTake, int HP)
		{
			R_GarrisonRow r_GarrisonRow = (R_GarrisonRow)NewRow();
			object[] itemArray = new object[9] { X, Y, Z, Locality, AntiMagic, Guard, Color, CanTake, HP };
			r_GarrisonRow.ItemArray = itemArray;
			base.Rows.Add(r_GarrisonRow);
			return r_GarrisonRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_GarrisonRow FindByXYZ(int X, int Y, int Z)
		{
			return (R_GarrisonRow)base.Rows.Find(new object[3] { X, Y, Z });
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_GarrisonDataTable r_GarrisonDataTable = (R_GarrisonDataTable)base.Clone();
			r_GarrisonDataTable.InitVars();
			return r_GarrisonDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_GarrisonDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnAntiMagic = base.Columns["AntiMagic"];
			columnGuard = base.Columns["Guard"];
			columnColor = base.Columns["Color"];
			columnCanTake = base.Columns["CanTake"];
			columnHP = base.Columns["HP"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnAntiMagic = new DataColumn("AntiMagic", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnAntiMagic);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnCanTake = new DataColumn("CanTake", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnCanTake);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			base.Constraints.Add(new UniqueConstraint("R_GarrisonPrKey", new DataColumn[3] { columnX, columnY, columnZ }, isPrimaryKey: true));
			columnX.AllowDBNull = false;
			columnY.AllowDBNull = false;
			columnZ.AllowDBNull = false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_GarrisonRow NewR_GarrisonRow()
		{
			return (R_GarrisonRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_GarrisonRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_GarrisonRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_GarrisonRowChanged != null)
			{
				this.R_GarrisonRowChanged(this, new R_GarrisonRowChangeEvent((R_GarrisonRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_GarrisonRowChanging != null)
			{
				this.R_GarrisonRowChanging(this, new R_GarrisonRowChangeEvent((R_GarrisonRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_GarrisonRowDeleted != null)
			{
				this.R_GarrisonRowDeleted(this, new R_GarrisonRowChangeEvent((R_GarrisonRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_GarrisonRowDeleting != null)
			{
				this.R_GarrisonRowDeleting(this, new R_GarrisonRowChangeEvent((R_GarrisonRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_GarrisonRow(R_GarrisonRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_GarrisonDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_SeerHutDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnMission;

		private DataColumn columnReward;

		private DataColumn columnDeadline;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MissionColumn => columnMission;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn RewardColumn => columnReward;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn DeadlineColumn => columnDeadline;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SeerHutRow this[int index] => (R_SeerHutRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SeerHutRowChangeEventHandler R_SeerHutRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SeerHutRowChangeEventHandler R_SeerHutRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SeerHutRowChangeEventHandler R_SeerHutRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_SeerHutRowChangeEventHandler R_SeerHutRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SeerHutDataTable()
		{
			base.TableName = "R_SeerHut";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_SeerHutDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_SeerHutDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_SeerHutRow(R_SeerHutRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SeerHutRow AddR_SeerHutRow(int X, int Y, int Z, string Locality, string Mission, string Reward, string Deadline)
		{
			R_SeerHutRow r_SeerHutRow = (R_SeerHutRow)NewRow();
			object[] itemArray = new object[7] { X, Y, Z, Locality, Mission, Reward, Deadline };
			r_SeerHutRow.ItemArray = itemArray;
			base.Rows.Add(r_SeerHutRow);
			return r_SeerHutRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SeerHutRow FindByXYZ(int X, int Y, int Z)
		{
			return (R_SeerHutRow)base.Rows.Find(new object[3] { X, Y, Z });
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_SeerHutDataTable r_SeerHutDataTable = (R_SeerHutDataTable)base.Clone();
			r_SeerHutDataTable.InitVars();
			return r_SeerHutDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_SeerHutDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnMission = base.Columns["Mission"];
			columnReward = base.Columns["Reward"];
			columnDeadline = base.Columns["Deadline"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnReward = new DataColumn("Reward", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnReward);
			columnDeadline = new DataColumn("Deadline", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnDeadline);
			base.Constraints.Add(new UniqueConstraint("R_SeerHutPrKey", new DataColumn[3] { columnX, columnY, columnZ }, isPrimaryKey: true));
			columnX.AllowDBNull = false;
			columnY.AllowDBNull = false;
			columnZ.AllowDBNull = false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SeerHutRow NewR_SeerHutRow()
		{
			return (R_SeerHutRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_SeerHutRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_SeerHutRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_SeerHutRowChanged != null)
			{
				this.R_SeerHutRowChanged(this, new R_SeerHutRowChangeEvent((R_SeerHutRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_SeerHutRowChanging != null)
			{
				this.R_SeerHutRowChanging(this, new R_SeerHutRowChangeEvent((R_SeerHutRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_SeerHutRowDeleted != null)
			{
				this.R_SeerHutRowDeleted(this, new R_SeerHutRowChangeEvent((R_SeerHutRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_SeerHutRowDeleting != null)
			{
				this.R_SeerHutRowDeleting(this, new R_SeerHutRowChangeEvent((R_SeerHutRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_SeerHutRow(R_SeerHutRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_SeerHutDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_PrisonDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnHero;

		private DataColumn columnLevel;

		private DataColumn columnPrimarySkill;

		private DataColumn columnSecondarySkill;

		private DataColumn columnArt;

		private DataColumn columnSpell;

		private DataColumn columnMonster;

		private DataColumn columnMachine;

		private DataColumn columnBook;

		private DataColumn columnMP;

		private DataColumn columnExperience;

		private DataColumn columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HeroColumn => columnHero;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LevelColumn => columnLevel;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PrimarySkillColumn => columnPrimarySkill;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SecondarySkillColumn => columnSecondarySkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ArtColumn => columnArt;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SpellColumn => columnSpell;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MonsterColumn => columnMonster;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MachineColumn => columnMachine;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn BookColumn => columnBook;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MPColumn => columnMP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ExperienceColumn => columnExperience;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HPColumn => columnHP;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PrisonRow this[int index] => (R_PrisonRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PrisonRowChangeEventHandler R_PrisonRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PrisonRowChangeEventHandler R_PrisonRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PrisonRowChangeEventHandler R_PrisonRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PrisonRowChangeEventHandler R_PrisonRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_PrisonDataTable()
		{
			base.TableName = "R_Prison";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_PrisonDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_PrisonDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_PrisonRow(R_PrisonRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_PrisonRow AddR_PrisonRow(int X, int Y, int Z, string Locality, string Hero, int Level, string PrimarySkill, string SecondarySkill, string Art, string Spell, string Monster, string Machine, string Book, int MP, int Experience, int HP)
		{
			R_PrisonRow r_PrisonRow = (R_PrisonRow)NewRow();
			object[] itemArray = new object[16]
			{
				X, Y, Z, Locality, Hero, Level, PrimarySkill, SecondarySkill, Art, Spell,
				Monster, Machine, Book, MP, Experience, HP
			};
			r_PrisonRow.ItemArray = itemArray;
			base.Rows.Add(r_PrisonRow);
			return r_PrisonRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_PrisonDataTable r_PrisonDataTable = (R_PrisonDataTable)base.Clone();
			r_PrisonDataTable.InitVars();
			return r_PrisonDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_PrisonDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnHero = base.Columns["Hero"];
			columnLevel = base.Columns["Level"];
			columnPrimarySkill = base.Columns["PrimarySkill"];
			columnSecondarySkill = base.Columns["SecondarySkill"];
			columnArt = base.Columns["Art"];
			columnSpell = base.Columns["Spell"];
			columnMonster = base.Columns["Monster"];
			columnMachine = base.Columns["Machine"];
			columnBook = base.Columns["Book"];
			columnMP = base.Columns["MP"];
			columnExperience = base.Columns["Experience"];
			columnHP = base.Columns["HP"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnHero = new DataColumn("Hero", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHero);
			columnLevel = new DataColumn("Level", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLevel);
			columnPrimarySkill = new DataColumn("PrimarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPrimarySkill);
			columnSecondarySkill = new DataColumn("SecondarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSecondarySkill);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnMachine = new DataColumn("Machine", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMachine);
			columnBook = new DataColumn("Book", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnBook);
			columnMP = new DataColumn("MP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnMP);
			columnExperience = new DataColumn("Experience", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnExperience);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PrisonRow NewR_PrisonRow()
		{
			return (R_PrisonRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_PrisonRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_PrisonRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_PrisonRowChanged != null)
			{
				this.R_PrisonRowChanged(this, new R_PrisonRowChangeEvent((R_PrisonRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_PrisonRowChanging != null)
			{
				this.R_PrisonRowChanging(this, new R_PrisonRowChangeEvent((R_PrisonRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_PrisonRowDeleted != null)
			{
				this.R_PrisonRowDeleted(this, new R_PrisonRowChangeEvent((R_PrisonRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_PrisonRowDeleting != null)
			{
				this.R_PrisonRowDeleting(this, new R_PrisonRowChangeEvent((R_PrisonRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_PrisonRow(R_PrisonRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_PrisonDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_ObjectDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnAddress;

		private DataColumn columnPayment;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn AddressColumn => columnAddress;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PaymentColumn => columnPayment;

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ObjectRow this[int index] => (R_ObjectRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ObjectRowChangeEventHandler R_ObjectRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ObjectRowChangeEventHandler R_ObjectRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ObjectRowChangeEventHandler R_ObjectRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_ObjectRowChangeEventHandler R_ObjectRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ObjectDataTable()
		{
			base.TableName = "R_Object";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_ObjectDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_ObjectDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_ObjectRow(R_ObjectRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ObjectRow AddR_ObjectRow(int X, int Y, int Z, string Locality, string Object, int Address, string Payment)
		{
			R_ObjectRow r_ObjectRow = (R_ObjectRow)NewRow();
			object[] itemArray = new object[7] { X, Y, Z, Locality, Object, Address, Payment };
			r_ObjectRow.ItemArray = itemArray;
			base.Rows.Add(r_ObjectRow);
			return r_ObjectRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_ObjectDataTable r_ObjectDataTable = (R_ObjectDataTable)base.Clone();
			r_ObjectDataTable.InitVars();
			return r_ObjectDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_ObjectDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnAddress = base.Columns["Address"];
			columnPayment = base.Columns["Payment"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnPayment = new DataColumn("Payment", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPayment);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ObjectRow NewR_ObjectRow()
		{
			return (R_ObjectRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_ObjectRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_ObjectRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_ObjectRowChanged != null)
			{
				this.R_ObjectRowChanged(this, new R_ObjectRowChangeEvent((R_ObjectRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_ObjectRowChanging != null)
			{
				this.R_ObjectRowChanging(this, new R_ObjectRowChangeEvent((R_ObjectRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_ObjectRowDeleted != null)
			{
				this.R_ObjectRowDeleted(this, new R_ObjectRowChangeEvent((R_ObjectRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_ObjectRowDeleting != null)
			{
				this.R_ObjectRowDeleting(this, new R_ObjectRowChangeEvent((R_ObjectRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_ObjectRow(R_ObjectRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_ObjectDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_PassGuardDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnMission;

		private DataColumn columnDeadline;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MissionColumn => columnMission;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn DeadlineColumn => columnDeadline;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_PassGuardRow this[int index] => (R_PassGuardRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PassGuardRowChangeEventHandler R_PassGuardRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PassGuardRowChangeEventHandler R_PassGuardRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PassGuardRowChangeEventHandler R_PassGuardRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_PassGuardRowChangeEventHandler R_PassGuardRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PassGuardDataTable()
		{
			base.TableName = "R_PassGuard";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_PassGuardDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_PassGuardDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_PassGuardRow(R_PassGuardRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_PassGuardRow AddR_PassGuardRow(int X, int Y, int Z, string Locality, string Mission, string Deadline)
		{
			R_PassGuardRow r_PassGuardRow = (R_PassGuardRow)NewRow();
			object[] itemArray = new object[6] { X, Y, Z, Locality, Mission, Deadline };
			r_PassGuardRow.ItemArray = itemArray;
			base.Rows.Add(r_PassGuardRow);
			return r_PassGuardRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_PassGuardRow FindByXYZ(int X, int Y, int Z)
		{
			return (R_PassGuardRow)base.Rows.Find(new object[3] { X, Y, Z });
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_PassGuardDataTable r_PassGuardDataTable = (R_PassGuardDataTable)base.Clone();
			r_PassGuardDataTable.InitVars();
			return r_PassGuardDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_PassGuardDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnMission = base.Columns["Mission"];
			columnDeadline = base.Columns["Deadline"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnDeadline = new DataColumn("Deadline", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnDeadline);
			base.Constraints.Add(new UniqueConstraint("R_PassGuardPrKey", new DataColumn[3] { columnX, columnY, columnZ }, isPrimaryKey: true));
			columnX.AllowDBNull = false;
			columnY.AllowDBNull = false;
			columnZ.AllowDBNull = false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PassGuardRow NewR_PassGuardRow()
		{
			return (R_PassGuardRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_PassGuardRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_PassGuardRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_PassGuardRowChanged != null)
			{
				this.R_PassGuardRowChanged(this, new R_PassGuardRowChangeEvent((R_PassGuardRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_PassGuardRowChanging != null)
			{
				this.R_PassGuardRowChanging(this, new R_PassGuardRowChangeEvent((R_PassGuardRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_PassGuardRowDeleted != null)
			{
				this.R_PassGuardRowDeleted(this, new R_PassGuardRowChangeEvent((R_PassGuardRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_PassGuardRowDeleting != null)
			{
				this.R_PassGuardRowDeleting(this, new R_PassGuardRowChangeEvent((R_PassGuardRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_PassGuardRow(R_PassGuardRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_PassGuardDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_HeroesDataTable : DataTable, IEnumerable
	{
		private DataColumn columnID;

		private DataColumn columnHero;

		private DataColumn columnPlace;

		private DataColumn columnColor;

		private DataColumn columnLevel;

		private DataColumn columnPrimarySkill;

		private DataColumn columnSecondarySkill;

		private DataColumn columnArt;

		private DataColumn columnSpell;

		private DataColumn columnMonster;

		private DataColumn columnMachine;

		private DataColumn columnBook;

		private DataColumn columnMP;

		private DataColumn columnExperience;

		private DataColumn columnAddress;

		private DataColumn columnHP;

		private DataColumn columnHire;

		private DataColumn columnIdeology;

		private DataColumn columnClass;

		private DataColumn columnClassID;

		private DataColumn columnTreeNumber;

		private DataColumn columnLastWisdom;

		private DataColumn columnLastMagic;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn IDColumn => columnID;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HeroColumn => columnHero;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn PlaceColumn => columnPlace;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LevelColumn => columnLevel;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn PrimarySkillColumn => columnPrimarySkill;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SecondarySkillColumn => columnSecondarySkill;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ArtColumn => columnArt;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SpellColumn => columnSpell;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MonsterColumn => columnMonster;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MachineColumn => columnMachine;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn BookColumn => columnBook;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MPColumn => columnMP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ExperienceColumn => columnExperience;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn AddressColumn => columnAddress;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HireColumn => columnHire;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn IdeologyColumn => columnIdeology;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ClassColumn => columnClass;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ClassIDColumn => columnClassID;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn TreeNumberColumn => columnTreeNumber;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LastWisdomColumn => columnLastWisdom;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LastMagicColumn => columnLastMagic;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_HeroesRow this[int index] => (R_HeroesRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_HeroesRowChangeEventHandler R_HeroesRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_HeroesRowChangeEventHandler R_HeroesRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_HeroesRowChangeEventHandler R_HeroesRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_HeroesRowChangeEventHandler R_HeroesRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_HeroesDataTable()
		{
			base.TableName = "R_Heroes";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_HeroesDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_HeroesDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_HeroesRow(R_HeroesRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_HeroesRow AddR_HeroesRow(int ID, string Hero, string Place, string Color, int Level, string PrimarySkill, string SecondarySkill, string Art, string Spell, string Monster, string Machine, string Book, int MP, int Experience, int Address, int HP, string Hire, string Ideology, string Class, int ClassID, int TreeNumber, int LastWisdom, int LastMagic)
		{
			R_HeroesRow r_HeroesRow = (R_HeroesRow)NewRow();
			object[] itemArray = new object[23]
			{
				ID, Hero, Place, Color, Level, PrimarySkill, SecondarySkill, Art, Spell, Monster,
				Machine, Book, MP, Experience, Address, HP, Hire, Ideology, Class, ClassID,
				TreeNumber, LastWisdom, LastMagic
			};
			r_HeroesRow.ItemArray = itemArray;
			base.Rows.Add(r_HeroesRow);
			return r_HeroesRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_HeroesDataTable r_HeroesDataTable = (R_HeroesDataTable)base.Clone();
			r_HeroesDataTable.InitVars();
			return r_HeroesDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_HeroesDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnID = base.Columns["ID"];
			columnHero = base.Columns["Hero"];
			columnPlace = base.Columns["Place"];
			columnColor = base.Columns["Color"];
			columnLevel = base.Columns["Level"];
			columnPrimarySkill = base.Columns["PrimarySkill"];
			columnSecondarySkill = base.Columns["SecondarySkill"];
			columnArt = base.Columns["Art"];
			columnSpell = base.Columns["Spell"];
			columnMonster = base.Columns["Monster"];
			columnMachine = base.Columns["Machine"];
			columnBook = base.Columns["Book"];
			columnMP = base.Columns["MP"];
			columnExperience = base.Columns["Experience"];
			columnAddress = base.Columns["Address"];
			columnHP = base.Columns["HP"];
			columnHire = base.Columns["Hire"];
			columnIdeology = base.Columns["Ideology"];
			columnClass = base.Columns["Class"];
			columnClassID = base.Columns["ClassID"];
			columnTreeNumber = base.Columns["TreeNumber"];
			columnLastWisdom = base.Columns["LastWisdom"];
			columnLastMagic = base.Columns["LastMagic"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnID = new DataColumn("ID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnID);
			columnHero = new DataColumn("Hero", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHero);
			columnPlace = new DataColumn("Place", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPlace);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnLevel = new DataColumn("Level", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLevel);
			columnPrimarySkill = new DataColumn("PrimarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPrimarySkill);
			columnSecondarySkill = new DataColumn("SecondarySkill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSecondarySkill);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnMachine = new DataColumn("Machine", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMachine);
			columnBook = new DataColumn("Book", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnBook);
			columnMP = new DataColumn("MP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnMP);
			columnExperience = new DataColumn("Experience", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnExperience);
			columnAddress = new DataColumn("Address", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnAddress);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			columnHire = new DataColumn("Hire", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHire);
			columnIdeology = new DataColumn("Ideology", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnIdeology);
			columnClass = new DataColumn("Class", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnClass);
			columnClassID = new DataColumn("ClassID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnClassID);
			columnTreeNumber = new DataColumn("TreeNumber", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnTreeNumber);
			columnLastWisdom = new DataColumn("LastWisdom", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLastWisdom);
			columnLastMagic = new DataColumn("LastMagic", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLastMagic);
			columnClass.Caption = "DataColumn1";
			columnClassID.Caption = "DataColumn1";
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_HeroesRow NewR_HeroesRow()
		{
			return (R_HeroesRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_HeroesRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_HeroesRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_HeroesRowChanged != null)
			{
				this.R_HeroesRowChanged(this, new R_HeroesRowChangeEvent((R_HeroesRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_HeroesRowChanging != null)
			{
				this.R_HeroesRowChanging(this, new R_HeroesRowChangeEvent((R_HeroesRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_HeroesRowDeleted != null)
			{
				this.R_HeroesRowDeleted(this, new R_HeroesRowChangeEvent((R_HeroesRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_HeroesRowDeleting != null)
			{
				this.R_HeroesRowDeleting(this, new R_HeroesRowChangeEvent((R_HeroesRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_HeroesRow(R_HeroesRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_HeroesDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_TownDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnName;

		private DataColumn columnType;

		private DataColumn columnSlot;

		private DataColumn columnSpell;

		private DataColumn columnColor;

		private DataColumn columnGarrison;

		private DataColumn columnCode;

		private DataColumn columnBuilt;

		private DataColumn columnHP;

		private DataColumn columnLibrary;

		private DataColumn columnID;

		private DataColumn columnMageTimer;

		private DataColumn columnLibTimer;

		private DataColumn columnAvailable;

		private DataColumn columnTimer;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn NameColumn => columnName;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn TypeColumn => columnType;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SlotColumn => columnSlot;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SpellColumn => columnSpell;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GarrisonColumn => columnGarrison;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn CodeColumn => columnCode;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn BuiltColumn => columnBuilt;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LibraryColumn => columnLibrary;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn IDColumn => columnID;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MageTimerColumn => columnMageTimer;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LibTimerColumn => columnLibTimer;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn AvailableColumn => columnAvailable;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn TimerColumn => columnTimer;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_TownRow this[int index] => (R_TownRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TownRowChangeEventHandler R_TownRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TownRowChangeEventHandler R_TownRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TownRowChangeEventHandler R_TownRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TownRowChangeEventHandler R_TownRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_TownDataTable()
		{
			base.TableName = "R_Town";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_TownDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_TownDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_TownRow(R_TownRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_TownRow AddR_TownRow(int X, int Y, int Z, string Name, string Type, int Slot, string Spell, string Color, string Garrison, int Code, string Built, int HP, string Library, int ID, int MageTimer, int LibTimer, string Available, string Timer)
		{
			R_TownRow r_TownRow = (R_TownRow)NewRow();
			object[] itemArray = new object[18]
			{
				X, Y, Z, Name, Type, Slot, Spell, Color, Garrison, Code,
				Built, HP, Library, ID, MageTimer, LibTimer, Available, Timer
			};
			r_TownRow.ItemArray = itemArray;
			base.Rows.Add(r_TownRow);
			return r_TownRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_TownDataTable r_TownDataTable = (R_TownDataTable)base.Clone();
			r_TownDataTable.InitVars();
			return r_TownDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_TownDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnName = base.Columns["Name"];
			columnType = base.Columns["Type"];
			columnSlot = base.Columns["Slot"];
			columnSpell = base.Columns["Spell"];
			columnColor = base.Columns["Color"];
			columnGarrison = base.Columns["Garrison"];
			columnCode = base.Columns["Code"];
			columnBuilt = base.Columns["Built"];
			columnHP = base.Columns["HP"];
			columnLibrary = base.Columns["Library"];
			columnID = base.Columns["ID"];
			columnMageTimer = base.Columns["MageTimer"];
			columnLibTimer = base.Columns["LibTimer"];
			columnAvailable = base.Columns["Available"];
			columnTimer = base.Columns["Timer"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnName = new DataColumn("Name", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnName);
			columnType = new DataColumn("Type", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnType);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnGarrison = new DataColumn("Garrison", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGarrison);
			columnCode = new DataColumn("Code", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnCode);
			columnBuilt = new DataColumn("Built", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnBuilt);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			columnLibrary = new DataColumn("Library", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLibrary);
			columnID = new DataColumn("ID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnID);
			columnMageTimer = new DataColumn("MageTimer", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnMageTimer);
			columnLibTimer = new DataColumn("LibTimer", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnLibTimer);
			columnAvailable = new DataColumn("Available", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnAvailable);
			columnTimer = new DataColumn("Timer", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnTimer);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TownRow NewR_TownRow()
		{
			return (R_TownRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_TownRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_TownRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_TownRowChanged != null)
			{
				this.R_TownRowChanged(this, new R_TownRowChangeEvent((R_TownRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_TownRowChanging != null)
			{
				this.R_TownRowChanging(this, new R_TownRowChangeEvent((R_TownRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_TownRowDeleted != null)
			{
				this.R_TownRowDeleted(this, new R_TownRowChangeEvent((R_TownRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_TownRowDeleting != null)
			{
				this.R_TownRowDeleting(this, new R_TownRowChangeEvent((R_TownRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_TownRow(R_TownRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_TownDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_AllArtsDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnArtefact;

		private DataColumn columnClass;

		private DataColumn _columnRelic_C;

		private DataColumn columnSlot;

		private DataColumn columnPlace;

		private DataColumn columnColor;

		private DataColumn columnHero;

		private DataColumn columnDoll;

		private DataColumn columnMonster;

		private DataColumn columnMission;

		private DataColumn columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ArtefactColumn => columnArtefact;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ClassColumn => columnClass;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn _Relic_CColumn => _columnRelic_C;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SlotColumn => columnSlot;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn PlaceColumn => columnPlace;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ColorColumn => columnColor;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HeroColumn => columnHero;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn DollColumn => columnDoll;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MonsterColumn => columnMonster;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MissionColumn => columnMission;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllArtsRow this[int index] => (R_AllArtsRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllArtsRowChangeEventHandler R_AllArtsRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllArtsRowChangeEventHandler R_AllArtsRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllArtsRowChangeEventHandler R_AllArtsRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllArtsRowChangeEventHandler R_AllArtsRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllArtsDataTable()
		{
			base.TableName = "R_AllArts";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_AllArtsDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_AllArtsDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_AllArtsRow(R_AllArtsRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllArtsRow AddR_AllArtsRow(int X, int Y, int Z, string Locality, string Object, string Artefact, string Class, string _Relic_C, int Slot, string Place, string Color, string Hero, string Doll, string Monster, string Mission, string Guard)
		{
			R_AllArtsRow r_AllArtsRow = (R_AllArtsRow)NewRow();
			object[] itemArray = new object[16]
			{
				X, Y, Z, Locality, Object, Artefact, Class, _Relic_C, Slot, Place,
				Color, Hero, Doll, Monster, Mission, Guard
			};
			r_AllArtsRow.ItemArray = itemArray;
			base.Rows.Add(r_AllArtsRow);
			return r_AllArtsRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_AllArtsDataTable r_AllArtsDataTable = (R_AllArtsDataTable)base.Clone();
			r_AllArtsDataTable.InitVars();
			return r_AllArtsDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_AllArtsDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnArtefact = base.Columns["Artefact"];
			columnClass = base.Columns["Class"];
			_columnRelic_C = base.Columns["Relic-C"];
			columnSlot = base.Columns["Slot"];
			columnPlace = base.Columns["Place"];
			columnColor = base.Columns["Color"];
			columnHero = base.Columns["Hero"];
			columnDoll = base.Columns["Doll"];
			columnMonster = base.Columns["Monster"];
			columnMission = base.Columns["Mission"];
			columnGuard = base.Columns["Guard"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnArtefact = new DataColumn("Artefact", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArtefact);
			columnClass = new DataColumn("Class", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnClass);
			_columnRelic_C = new DataColumn("Relic-C", typeof(string), null, MappingType.Element);
			_columnRelic_C.ExtendedProperties.Add("Generator_ColumnVarNameInTable", "_columnRelic_C");
			_columnRelic_C.ExtendedProperties.Add("Generator_UserColumnName", "Relic-C");
			base.Columns.Add(_columnRelic_C);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnPlace = new DataColumn("Place", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPlace);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnHero = new DataColumn("Hero", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHero);
			columnDoll = new DataColumn("Doll", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnDoll);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllArtsRow NewR_AllArtsRow()
		{
			return (R_AllArtsRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_AllArtsRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_AllArtsRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_AllArtsRowChanged != null)
			{
				this.R_AllArtsRowChanged(this, new R_AllArtsRowChangeEvent((R_AllArtsRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_AllArtsRowChanging != null)
			{
				this.R_AllArtsRowChanging(this, new R_AllArtsRowChangeEvent((R_AllArtsRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_AllArtsRowDeleted != null)
			{
				this.R_AllArtsRowDeleted(this, new R_AllArtsRowChangeEvent((R_AllArtsRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_AllArtsRowDeleting != null)
			{
				this.R_AllArtsRowDeleting(this, new R_AllArtsRowChangeEvent((R_AllArtsRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_AllArtsRow(R_AllArtsRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_AllArtsDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_TopologyDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnType;

		private DataColumn columnColor;

		private DataColumn columnPair;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn TypeColumn => columnType;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn PairColumn => columnPair;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyRow this[int index] => (R_TopologyRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TopologyRowChangeEventHandler R_TopologyRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TopologyRowChangeEventHandler R_TopologyRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TopologyRowChangeEventHandler R_TopologyRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_TopologyRowChangeEventHandler R_TopologyRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyDataTable()
		{
			base.TableName = "R_Topology";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_TopologyDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_TopologyDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_TopologyRow(R_TopologyRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyRow AddR_TopologyRow(int X, int Y, int Z, string Locality, string Object, string Type, string Color, int Pair)
		{
			R_TopologyRow r_TopologyRow = (R_TopologyRow)NewRow();
			object[] itemArray = new object[8] { X, Y, Z, Locality, Object, Type, Color, Pair };
			r_TopologyRow.ItemArray = itemArray;
			base.Rows.Add(r_TopologyRow);
			return r_TopologyRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_TopologyDataTable r_TopologyDataTable = (R_TopologyDataTable)base.Clone();
			r_TopologyDataTable.InitVars();
			return r_TopologyDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_TopologyDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnType = base.Columns["Type"];
			columnColor = base.Columns["Color"];
			columnPair = base.Columns["Pair"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnType = new DataColumn("Type", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnType);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnPair = new DataColumn("Pair", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnPair);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyRow NewR_TopologyRow()
		{
			return (R_TopologyRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_TopologyRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_TopologyRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_TopologyRowChanged != null)
			{
				this.R_TopologyRowChanged(this, new R_TopologyRowChangeEvent((R_TopologyRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_TopologyRowChanging != null)
			{
				this.R_TopologyRowChanging(this, new R_TopologyRowChangeEvent((R_TopologyRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_TopologyRowDeleted != null)
			{
				this.R_TopologyRowDeleted(this, new R_TopologyRowChangeEvent((R_TopologyRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_TopologyRowDeleting != null)
			{
				this.R_TopologyRowDeleting(this, new R_TopologyRowChangeEvent((R_TopologyRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_TopologyRow(R_TopologyRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_TopologyDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_AllSpellDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnObject;

		private DataColumn columnSpell;

		private DataColumn columnSlot;

		private DataColumn columnColor;

		private DataColumn columnName;

		private DataColumn columnBuilt;

		private DataColumn columnGarrison;

		private DataColumn columnID;

		private DataColumn columnHero;

		private DataColumn columnPlace;

		private DataColumn columnMission;

		private DataColumn columnGuard;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SpellColumn => columnSpell;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SlotColumn => columnSlot;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ColorColumn => columnColor;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn NameColumn => columnName;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn BuiltColumn => columnBuilt;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GarrisonColumn => columnGarrison;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn IDColumn => columnID;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HeroColumn => columnHero;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PlaceColumn => columnPlace;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MissionColumn => columnMission;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GuardColumn => columnGuard;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSpellRow this[int index] => (R_AllSpellRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSpellRowChangeEventHandler R_AllSpellRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSpellRowChangeEventHandler R_AllSpellRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSpellRowChangeEventHandler R_AllSpellRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSpellRowChangeEventHandler R_AllSpellRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSpellDataTable()
		{
			base.TableName = "R_AllSpell";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_AllSpellDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected R_AllSpellDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_AllSpellRow(R_AllSpellRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSpellRow AddR_AllSpellRow(int X, int Y, int Z, string Object, string Spell, int Slot, string Color, string Name, string Built, string Garrison, int ID, string Hero, string Place, string Mission, string Guard)
		{
			R_AllSpellRow r_AllSpellRow = (R_AllSpellRow)NewRow();
			object[] itemArray = new object[15]
			{
				X, Y, Z, Object, Spell, Slot, Color, Name, Built, Garrison,
				ID, Hero, Place, Mission, Guard
			};
			r_AllSpellRow.ItemArray = itemArray;
			base.Rows.Add(r_AllSpellRow);
			return r_AllSpellRow;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_AllSpellDataTable r_AllSpellDataTable = (R_AllSpellDataTable)base.Clone();
			r_AllSpellDataTable.InitVars();
			return r_AllSpellDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_AllSpellDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnObject = base.Columns["Object"];
			columnSpell = base.Columns["Spell"];
			columnSlot = base.Columns["Slot"];
			columnColor = base.Columns["Color"];
			columnName = base.Columns["Name"];
			columnBuilt = base.Columns["Built"];
			columnGarrison = base.Columns["Garrison"];
			columnID = base.Columns["ID"];
			columnHero = base.Columns["Hero"];
			columnPlace = base.Columns["Place"];
			columnMission = base.Columns["Mission"];
			columnGuard = base.Columns["Guard"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnName = new DataColumn("Name", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnName);
			columnBuilt = new DataColumn("Built", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnBuilt);
			columnGarrison = new DataColumn("Garrison", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGarrison);
			columnID = new DataColumn("ID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnID);
			columnHero = new DataColumn("Hero", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHero);
			columnPlace = new DataColumn("Place", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPlace);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSpellRow NewR_AllSpellRow()
		{
			return (R_AllSpellRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_AllSpellRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_AllSpellRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_AllSpellRowChanged != null)
			{
				this.R_AllSpellRowChanged(this, new R_AllSpellRowChangeEvent((R_AllSpellRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_AllSpellRowChanging != null)
			{
				this.R_AllSpellRowChanging(this, new R_AllSpellRowChangeEvent((R_AllSpellRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_AllSpellRowDeleted != null)
			{
				this.R_AllSpellRowDeleted(this, new R_AllSpellRowChangeEvent((R_AllSpellRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_AllSpellRowDeleting != null)
			{
				this.R_AllSpellRowDeleting(this, new R_AllSpellRowChangeEvent((R_AllSpellRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_AllSpellRow(R_AllSpellRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_AllSpellDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_AllTimerDataTable : DataTable, IEnumerable
	{
		private DataColumn columnObject;

		private DataColumn columnDay;

		private DataColumn columnRepeat;

		private DataColumn columnTown;

		private DataColumn columnType;

		private DataColumn columnPlace;

		private DataColumn columnGold;

		private DataColumn columnResource;

		private DataColumn columnBuilding;

		private DataColumn columnMonster;

		private DataColumn columnApply;

		private DataColumn columnID;

		private DataColumn columnColor;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn DayColumn => columnDay;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn RepeatColumn => columnRepeat;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn TownColumn => columnTown;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn TypeColumn => columnType;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn PlaceColumn => columnPlace;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GoldColumn => columnGold;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn BuildingColumn => columnBuilding;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MonsterColumn => columnMonster;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ApplyColumn => columnApply;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn IDColumn => columnID;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllTimerRow this[int index] => (R_AllTimerRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllTimerRowChangeEventHandler R_AllTimerRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllTimerRowChangeEventHandler R_AllTimerRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllTimerRowChangeEventHandler R_AllTimerRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllTimerRowChangeEventHandler R_AllTimerRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllTimerDataTable()
		{
			base.TableName = "R_AllTimer";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_AllTimerDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_AllTimerDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_AllTimerRow(R_AllTimerRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllTimerRow AddR_AllTimerRow(string Object, int Day, int Repeat, string Town, string Type, string Place, int Gold, string Resource, string Building, string Monster, string Apply, int ID, string Color)
		{
			R_AllTimerRow r_AllTimerRow = (R_AllTimerRow)NewRow();
			object[] itemArray = new object[13]
			{
				Object, Day, Repeat, Town, Type, Place, Gold, Resource, Building, Monster,
				Apply, ID, Color
			};
			r_AllTimerRow.ItemArray = itemArray;
			base.Rows.Add(r_AllTimerRow);
			return r_AllTimerRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_AllTimerDataTable r_AllTimerDataTable = (R_AllTimerDataTable)base.Clone();
			r_AllTimerDataTable.InitVars();
			return r_AllTimerDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_AllTimerDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnObject = base.Columns["Object"];
			columnDay = base.Columns["Day"];
			columnRepeat = base.Columns["Repeat"];
			columnTown = base.Columns["Town"];
			columnType = base.Columns["Type"];
			columnPlace = base.Columns["Place"];
			columnGold = base.Columns["Gold"];
			columnResource = base.Columns["Resource"];
			columnBuilding = base.Columns["Building"];
			columnMonster = base.Columns["Monster"];
			columnApply = base.Columns["Apply"];
			columnID = base.Columns["ID"];
			columnColor = base.Columns["Color"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnDay = new DataColumn("Day", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnDay);
			columnRepeat = new DataColumn("Repeat", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnRepeat);
			columnTown = new DataColumn("Town", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnTown);
			columnType = new DataColumn("Type", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnType);
			columnPlace = new DataColumn("Place", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnPlace);
			columnGold = new DataColumn("Gold", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnGold);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnBuilding = new DataColumn("Building", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnBuilding);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnApply = new DataColumn("Apply", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnApply);
			columnID = new DataColumn("ID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnID);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllTimerRow NewR_AllTimerRow()
		{
			return (R_AllTimerRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_AllTimerRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_AllTimerRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_AllTimerRowChanged != null)
			{
				this.R_AllTimerRowChanged(this, new R_AllTimerRowChangeEvent((R_AllTimerRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_AllTimerRowChanging != null)
			{
				this.R_AllTimerRowChanging(this, new R_AllTimerRowChangeEvent((R_AllTimerRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_AllTimerRowDeleted != null)
			{
				this.R_AllTimerRowDeleted(this, new R_AllTimerRowChangeEvent((R_AllTimerRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_AllTimerRowDeleting != null)
			{
				this.R_AllTimerRowDeleting(this, new R_AllTimerRowChangeEvent((R_AllTimerRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_AllTimerRow(R_AllTimerRow row)
		{
			base.Rows.Remove(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_AllTimerDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_AllSkillDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnSkill;

		private DataColumn columnMission;

		private DataColumn columnGuard;

		private DataColumn columnSlot;

		private DataColumn columnLevel;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn SkillColumn => columnSkill;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn MissionColumn => columnMission;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn GuardColumn => columnGuard;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SlotColumn => columnSlot;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LevelColumn => columnLevel;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSkillRow this[int index] => (R_AllSkillRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSkillRowChangeEventHandler R_AllSkillRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSkillRowChangeEventHandler R_AllSkillRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSkillRowChangeEventHandler R_AllSkillRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllSkillRowChangeEventHandler R_AllSkillRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSkillDataTable()
		{
			base.TableName = "R_AllSkill";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_AllSkillDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_AllSkillDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_AllSkillRow(R_AllSkillRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSkillRow AddR_AllSkillRow(int X, int Y, int Z, string Locality, string Object, string Skill, string Mission, string Guard, int Slot, string Level)
		{
			R_AllSkillRow r_AllSkillRow = (R_AllSkillRow)NewRow();
			object[] itemArray = new object[10] { X, Y, Z, Locality, Object, Skill, Mission, Guard, Slot, Level };
			r_AllSkillRow.ItemArray = itemArray;
			base.Rows.Add(r_AllSkillRow);
			return r_AllSkillRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_AllSkillDataTable r_AllSkillDataTable = (R_AllSkillDataTable)base.Clone();
			r_AllSkillDataTable.InitVars();
			return r_AllSkillDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_AllSkillDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnSkill = base.Columns["Skill"];
			columnMission = base.Columns["Mission"];
			columnGuard = base.Columns["Guard"];
			columnSlot = base.Columns["Slot"];
			columnLevel = base.Columns["Level"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnSkill = new DataColumn("Skill", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSkill);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnSlot = new DataColumn("Slot", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnSlot);
			columnLevel = new DataColumn("Level", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLevel);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSkillRow NewR_AllSkillRow()
		{
			return (R_AllSkillRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_AllSkillRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_AllSkillRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_AllSkillRowChanged != null)
			{
				this.R_AllSkillRowChanged(this, new R_AllSkillRowChangeEvent((R_AllSkillRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_AllSkillRowChanging != null)
			{
				this.R_AllSkillRowChanging(this, new R_AllSkillRowChangeEvent((R_AllSkillRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_AllSkillRowDeleted != null)
			{
				this.R_AllSkillRowDeleted(this, new R_AllSkillRowChangeEvent((R_AllSkillRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_AllSkillRowDeleting != null)
			{
				this.R_AllSkillRowDeleting(this, new R_AllSkillRowChangeEvent((R_AllSkillRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_AllSkillRow(R_AllSkillRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_AllSkillDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_AllExperienceDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		private DataColumn columnObject;

		private DataColumn columnTown;

		private DataColumn columnMonster;

		private DataColumn columnArt;

		private DataColumn columnResource;

		private DataColumn columnGuard;

		private DataColumn columnMission;

		private DataColumn columnSpell;

		private DataColumn columnID;

		private DataColumn columnHero;

		private DataColumn columnColor;

		private DataColumn columnExperience;

		private DataColumn columnHP;

		private DataColumn columnXP;

		private DataColumn columnXP5;

		private DataColumn columnXP10;

		private DataColumn columnXP15;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn LocalityColumn => columnLocality;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ObjectColumn => columnObject;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn TownColumn => columnTown;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MonsterColumn => columnMonster;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ArtColumn => columnArt;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ResourceColumn => columnResource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn GuardColumn => columnGuard;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn MissionColumn => columnMission;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn SpellColumn => columnSpell;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn IDColumn => columnID;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn HeroColumn => columnHero;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ColorColumn => columnColor;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn ExperienceColumn => columnExperience;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn HPColumn => columnHP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XPColumn => columnXP;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XP5Column => columnXP5;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XP10Column => columnXP10;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn XP15Column => columnXP15;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllExperienceRow this[int index] => (R_AllExperienceRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllExperienceRowChangeEventHandler R_AllExperienceRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllExperienceRowChangeEventHandler R_AllExperienceRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllExperienceRowChangeEventHandler R_AllExperienceRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_AllExperienceRowChangeEventHandler R_AllExperienceRowDeleted;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllExperienceDataTable()
		{
			base.TableName = "R_AllExperience";
			BeginInit();
			InitClass();
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_AllExperienceDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_AllExperienceDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void AddR_AllExperienceRow(R_AllExperienceRow row)
		{
			base.Rows.Add(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllExperienceRow AddR_AllExperienceRow(int X, int Y, int Z, string Locality, string Object, string Town, string Monster, string Art, string Resource, string Guard, string Mission, string Spell, int ID, string Hero, string Color, int Experience, int HP, int XP, int XP5, int XP10, int XP15)
		{
			R_AllExperienceRow r_AllExperienceRow = (R_AllExperienceRow)NewRow();
			object[] itemArray = new object[21]
			{
				X, Y, Z, Locality, Object, Town, Monster, Art, Resource, Guard,
				Mission, Spell, ID, Hero, Color, Experience, HP, XP, XP5, XP10,
				XP15
			};
			r_AllExperienceRow.ItemArray = itemArray;
			base.Rows.Add(r_AllExperienceRow);
			return r_AllExperienceRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override DataTable Clone()
		{
			R_AllExperienceDataTable r_AllExperienceDataTable = (R_AllExperienceDataTable)base.Clone();
			r_AllExperienceDataTable.InitVars();
			return r_AllExperienceDataTable;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataTable CreateInstance()
		{
			return new R_AllExperienceDataTable();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
			columnObject = base.Columns["Object"];
			columnTown = base.Columns["Town"];
			columnMonster = base.Columns["Monster"];
			columnArt = base.Columns["Art"];
			columnResource = base.Columns["Resource"];
			columnGuard = base.Columns["Guard"];
			columnMission = base.Columns["Mission"];
			columnSpell = base.Columns["Spell"];
			columnID = base.Columns["ID"];
			columnHero = base.Columns["Hero"];
			columnColor = base.Columns["Color"];
			columnExperience = base.Columns["Experience"];
			columnHP = base.Columns["HP"];
			columnXP = base.Columns["XP"];
			columnXP5 = base.Columns["XP5"];
			columnXP10 = base.Columns["XP10"];
			columnXP15 = base.Columns["XP15"];
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
			columnObject = new DataColumn("Object", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnObject);
			columnTown = new DataColumn("Town", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnTown);
			columnMonster = new DataColumn("Monster", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMonster);
			columnArt = new DataColumn("Art", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnArt);
			columnResource = new DataColumn("Resource", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnResource);
			columnGuard = new DataColumn("Guard", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnGuard);
			columnMission = new DataColumn("Mission", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnMission);
			columnSpell = new DataColumn("Spell", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnSpell);
			columnID = new DataColumn("ID", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnID);
			columnHero = new DataColumn("Hero", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnHero);
			columnColor = new DataColumn("Color", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnColor);
			columnExperience = new DataColumn("Experience", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnExperience);
			columnHP = new DataColumn("HP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnHP);
			columnXP = new DataColumn("XP", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnXP);
			columnXP5 = new DataColumn("XP5", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnXP5);
			columnXP10 = new DataColumn("XP10", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnXP10);
			columnXP15 = new DataColumn("XP15", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnXP15);
			columnMonster.Caption = "Name";
			columnHP.Caption = "∑ HP";
			columnXP.Caption = "∑ XP";
			columnXP5.ReadOnly = true;
			columnXP5.Caption = "∑ XP+5%";
			columnXP10.ReadOnly = true;
			columnXP10.Caption = "∑ XP+10%";
			columnXP15.ReadOnly = true;
			columnXP15.Caption = "∑ XP+15%";
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllExperienceRow NewR_AllExperienceRow()
		{
			return (R_AllExperienceRow)NewRow();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_AllExperienceRow(builder);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override Type GetRowType()
		{
			return typeof(R_AllExperienceRow);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_AllExperienceRowChanged != null)
			{
				this.R_AllExperienceRowChanged(this, new R_AllExperienceRowChangeEvent((R_AllExperienceRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_AllExperienceRowChanging != null)
			{
				this.R_AllExperienceRowChanging(this, new R_AllExperienceRowChangeEvent((R_AllExperienceRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_AllExperienceRowDeleted != null)
			{
				this.R_AllExperienceRowDeleted(this, new R_AllExperienceRowChangeEvent((R_AllExperienceRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_AllExperienceRowDeleting != null)
			{
				this.R_AllExperienceRowDeleting(this, new R_AllExperienceRowChangeEvent((R_AllExperienceRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void RemoveR_AllExperienceRow(R_AllExperienceRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_AllExperienceDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	[Serializable]
	[XmlSchemaProvider("GetTypedTableSchema")]
	public class R_MineDataTable : DataTable, IEnumerable
	{
		private DataColumn columnX;

		private DataColumn columnY;

		private DataColumn columnZ;

		private DataColumn columnLocality;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn XColumn => columnX;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataColumn YColumn => columnY;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn ZColumn => columnZ;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataColumn LocalityColumn => columnLocality;

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Count => base.Rows.Count;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MineRow this[int index] => (R_MineRow)base.Rows[index];

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MineRowChangeEventHandler R_MineRowChanging;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MineRowChangeEventHandler R_MineRowChanged;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MineRowChangeEventHandler R_MineRowDeleting;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public event R_MineRowChangeEventHandler R_MineRowDeleted;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MineDataTable()
		{
			base.TableName = "R_Mine";
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_MineDataTable(DataTable table)
		{
			base.TableName = table.TableName;
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected R_MineDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void AddR_MineRow(R_MineRow row)
		{
			base.Rows.Add(row);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MineRow AddR_MineRow(int X, int Y, int Z, string Locality)
		{
			R_MineRow r_MineRow = (R_MineRow)NewRow();
			object[] itemArray = new object[4] { X, Y, Z, Locality };
			r_MineRow.ItemArray = itemArray;
			base.Rows.Add(r_MineRow);
			return r_MineRow;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public virtual IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataTable Clone()
		{
			R_MineDataTable r_MineDataTable = (R_MineDataTable)base.Clone();
			r_MineDataTable.InitVars();
			return r_MineDataTable;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataTable CreateInstance()
		{
			return new R_MineDataTable();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			columnX = base.Columns["X"];
			columnY = base.Columns["Y"];
			columnZ = base.Columns["Z"];
			columnLocality = base.Columns["Locality"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			columnX = new DataColumn("X", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnX);
			columnY = new DataColumn("Y", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnY);
			columnZ = new DataColumn("Z", typeof(int), null, MappingType.Element);
			base.Columns.Add(columnZ);
			columnLocality = new DataColumn("Locality", typeof(string), null, MappingType.Element);
			base.Columns.Add(columnLocality);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MineRow NewR_MineRow()
		{
			return (R_MineRow)NewRow();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new R_MineRow(builder);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override Type GetRowType()
		{
			return typeof(R_MineRow);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.R_MineRowChanged != null)
			{
				this.R_MineRowChanged(this, new R_MineRowChangeEvent((R_MineRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.R_MineRowChanging != null)
			{
				this.R_MineRowChanging(this, new R_MineRowChangeEvent((R_MineRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.R_MineRowDeleted != null)
			{
				this.R_MineRowDeleted(this, new R_MineRowChangeEvent((R_MineRow)e.Row, e.Action));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.R_MineRowDeleting != null)
			{
				this.R_MineRowDeleting(this, new R_MineRowChangeEvent((R_MineRow)e.Row, e.Action));
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void RemoveR_MineRow(R_MineRow row)
		{
			base.Rows.Remove(row);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			DataSet2 dataSet = new DataSet2();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
			xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny2.MinOccurs = 1m;
			xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny2);
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "namespace";
			xmlSchemaAttribute.FixedValue = dataSet.Namespace;
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "tableTypeName";
			xmlSchemaAttribute2.FixedValue = "R_MineDataTable";
			xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					XmlSchema xmlSchema = null;
					schemaSerializable.Write(memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						xmlSchema = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}
	}

	public class R_MonstrRow : DataRow
	{
		private R_MonstrDataTable tableR_Monstr;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				return (int)base[tableR_Monstr.XColumn];
			}
			set
			{
				base[tableR_Monstr.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				return (int)base[tableR_Monstr.YColumn];
			}
			set
			{
				base[tableR_Monstr.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				return (int)base[tableR_Monstr.ZColumn];
			}
			set
			{
				base[tableR_Monstr.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Monstr.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Name
		{
			get
			{
				try
				{
					return (string)base[tableR_Monstr.NameColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Name' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.NameColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Number
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.NumberColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Number' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.NumberColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Mood
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.MoodColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mood' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.MoodColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Level
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.LevelColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_Monstr.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.ArtColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_Monstr.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.ResourceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Increase
		{
			get
			{
				try
				{
					return (string)base[tableR_Monstr.IncreaseColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Increase' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.IncreaseColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Monstr.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Monstr' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Monstr.HPColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_MonstrRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Monstr = (R_MonstrDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Monstr.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Monstr.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsNameNull()
		{
			return IsNull(tableR_Monstr.NameColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetNameNull()
		{
			base[tableR_Monstr.NameColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsNumberNull()
		{
			return IsNull(tableR_Monstr.NumberColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetNumberNull()
		{
			base[tableR_Monstr.NumberColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMoodNull()
		{
			return IsNull(tableR_Monstr.MoodColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMoodNull()
		{
			base[tableR_Monstr.MoodColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLevelNull()
		{
			return IsNull(tableR_Monstr.LevelColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLevelNull()
		{
			base[tableR_Monstr.LevelColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsArtNull()
		{
			return IsNull(tableR_Monstr.ArtColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtNull()
		{
			base[tableR_Monstr.ArtColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGoldNull()
		{
			return IsNull(tableR_Monstr.GoldColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGoldNull()
		{
			base[tableR_Monstr.GoldColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsResourceNull()
		{
			return IsNull(tableR_Monstr.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_Monstr.ResourceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsIncreaseNull()
		{
			return IsNull(tableR_Monstr.IncreaseColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetIncreaseNull()
		{
			base[tableR_Monstr.IncreaseColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Monstr.AddressColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAddressNull()
		{
			base[tableR_Monstr.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Monstr.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_Monstr.HPColumn] = Convert.DBNull;
		}
	}

	public class R_ArtRow : DataRow
	{
		private R_ArtDataTable tableR_Art;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.SlotColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Name
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.NameColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Name' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.NameColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Class
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.ClassColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Class' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.ClassColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string _Relic_C
		{
			get
			{
				try
				{
					return (string)base[tableR_Art._Relic_CColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Relic-C' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art._Relic_CColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.ResourceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_Art.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Art.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Art' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Art.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_ArtRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Art = (R_ArtDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Art.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_Art.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Art.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Art.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Art.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Art.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Art.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Art.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Art.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_Art.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSlotNull()
		{
			return IsNull(tableR_Art.SlotColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSlotNull()
		{
			base[tableR_Art.SlotColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsNameNull()
		{
			return IsNull(tableR_Art.NameColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetNameNull()
		{
			base[tableR_Art.NameColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsClassNull()
		{
			return IsNull(tableR_Art.ClassColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetClassNull()
		{
			base[tableR_Art.ClassColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool Is_Relic_CNull()
		{
			return IsNull(tableR_Art._Relic_CColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void Set_Relic_CNull()
		{
			base[tableR_Art._Relic_CColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGoldNull()
		{
			return IsNull(tableR_Art.GoldColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGoldNull()
		{
			base[tableR_Art.GoldColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsResourceNull()
		{
			return IsNull(tableR_Art.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_Art.ResourceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGuardNull()
		{
			return IsNull(tableR_Art.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_Art.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Art.AddressColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAddressNull()
		{
			base[tableR_Art.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Art.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_Art.HPColumn] = Convert.DBNull;
		}
	}

	public class R_BankRow : DataRow
	{
		private R_BankDataTable tableR_Bank;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Bank.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Name
		{
			get
			{
				try
				{
					return (string)base[tableR_Bank.NameColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Name' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.NameColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_Bank.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_Bank.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.MonsterColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_Bank.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.ResourceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Bank.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Bank' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Bank.HPColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_BankRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Bank = (R_BankDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Bank.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Bank.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Bank.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Bank.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Bank.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_Bank.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Bank.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Bank.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsNameNull()
		{
			return IsNull(tableR_Bank.NameColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetNameNull()
		{
			base[tableR_Bank.NameColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_Bank.GuardColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGuardNull()
		{
			base[tableR_Bank.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_Bank.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_Bank.MonsterColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGoldNull()
		{
			return IsNull(tableR_Bank.GoldColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGoldNull()
		{
			base[tableR_Bank.GoldColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsResourceNull()
		{
			return IsNull(tableR_Bank.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_Bank.ResourceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Bank.AddressColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAddressNull()
		{
			base[tableR_Bank.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Bank.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_Bank.HPColumn] = Convert.DBNull;
		}
	}

	public class R_EventBoxRow : DataRow
	{
		private R_EventBoxDataTable tableR_EventBox;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.GuardColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Experience
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.ExperienceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Experience' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ExperienceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Mana
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.ManaColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mana' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ManaColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Morale
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.MoraleColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Morale' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.MoraleColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Luck
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.LuckColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Luck' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.LuckColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ResourceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string PrimarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.PrimarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'PrimarySkill' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.PrimarySkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string SecondarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.SecondarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'SecondarySkill' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.SecondarySkillColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Artefact
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.ArtefactColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Artefact' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ArtefactColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.MonsterColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_EventBox.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Apply
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.ApplyColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Apply' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.ApplyColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Repeat
		{
			get
			{
				try
				{
					return (string)base[tableR_EventBox.RepeatColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Repeat' в таблице 'R_EventBox' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_EventBox.RepeatColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_EventBoxRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_EventBox = (R_EventBoxDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_EventBox.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_EventBox.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_EventBox.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_EventBox.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_EventBox.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_EventBox.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_EventBox.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_EventBox.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_EventBox.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_EventBox.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_EventBox.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_EventBox.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsExperienceNull()
		{
			return IsNull(tableR_EventBox.ExperienceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetExperienceNull()
		{
			base[tableR_EventBox.ExperienceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsManaNull()
		{
			return IsNull(tableR_EventBox.ManaColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetManaNull()
		{
			base[tableR_EventBox.ManaColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMoraleNull()
		{
			return IsNull(tableR_EventBox.MoraleColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMoraleNull()
		{
			base[tableR_EventBox.MoraleColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLuckNull()
		{
			return IsNull(tableR_EventBox.LuckColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLuckNull()
		{
			base[tableR_EventBox.LuckColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGoldNull()
		{
			return IsNull(tableR_EventBox.GoldColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGoldNull()
		{
			base[tableR_EventBox.GoldColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsResourceNull()
		{
			return IsNull(tableR_EventBox.ResourceColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetResourceNull()
		{
			base[tableR_EventBox.ResourceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPrimarySkillNull()
		{
			return IsNull(tableR_EventBox.PrimarySkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetPrimarySkillNull()
		{
			base[tableR_EventBox.PrimarySkillColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSecondarySkillNull()
		{
			return IsNull(tableR_EventBox.SecondarySkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSecondarySkillNull()
		{
			base[tableR_EventBox.SecondarySkillColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsArtefactNull()
		{
			return IsNull(tableR_EventBox.ArtefactColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtefactNull()
		{
			base[tableR_EventBox.ArtefactColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSpellNull()
		{
			return IsNull(tableR_EventBox.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_EventBox.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_EventBox.MonsterColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMonsterNull()
		{
			base[tableR_EventBox.MonsterColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHPNull()
		{
			return IsNull(tableR_EventBox.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_EventBox.HPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_EventBox.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_EventBox.AddressColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsApplyNull()
		{
			return IsNull(tableR_EventBox.ApplyColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetApplyNull()
		{
			base[tableR_EventBox.ApplyColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsRepeatNull()
		{
			return IsNull(tableR_EventBox.RepeatColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetRepeatNull()
		{
			base[tableR_EventBox.RepeatColumn] = Convert.DBNull;
		}
	}

	public class R_ScholarRow : DataRow
	{
		private R_ScholarDataTable tableR_Scholar;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Scholar.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Scholar.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Scholar.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Scholar.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_Scholar.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string PrimarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Scholar.PrimarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'PrimarySkill' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.PrimarySkillColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string SecondarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Scholar.SecondarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'SecondarySkill' в таблице 'R_Scholar' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Scholar.SecondarySkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ScholarRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Scholar = (R_ScholarDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Scholar.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Scholar.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Scholar.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Scholar.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Scholar.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_Scholar.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Scholar.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Scholar.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSpellNull()
		{
			return IsNull(tableR_Scholar.SpellColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSpellNull()
		{
			base[tableR_Scholar.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPrimarySkillNull()
		{
			return IsNull(tableR_Scholar.PrimarySkillColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPrimarySkillNull()
		{
			base[tableR_Scholar.PrimarySkillColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSecondarySkillNull()
		{
			return IsNull(tableR_Scholar.SecondarySkillColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSecondarySkillNull()
		{
			base[tableR_Scholar.SecondarySkillColumn] = Convert.DBNull;
		}
	}

	public class R_ResourceRow : DataRow
	{
		private R_ResourceDataTable tableR_Resource;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Resource.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Resource.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_Resource.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.ResourceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_Resource.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.GuardColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Resource.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Resource' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Resource.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_ResourceRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Resource = (R_ResourceDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Resource.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Resource.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Resource.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Resource.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Resource.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Resource.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Resource.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Resource.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Resource.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_Resource.ObjectColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsResourceNull()
		{
			return IsNull(tableR_Resource.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_Resource.ResourceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGoldNull()
		{
			return IsNull(tableR_Resource.GoldColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGoldNull()
		{
			base[tableR_Resource.GoldColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_Resource.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_Resource.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Resource.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Resource.AddressColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHPNull()
		{
			return IsNull(tableR_Resource.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Resource.HPColumn] = Convert.DBNull;
		}
	}

	public class R_SpellRow : DataRow
	{
		private R_SpellDataTable tableR_Spell;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Spell.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Spell.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_Spell.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_Spell.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.HPColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Level
		{
			get
			{
				try
				{
					return (int)base[tableR_Spell.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_Spell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Spell.LevelColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_SpellRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Spell = (R_SpellDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Spell.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Spell.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Spell.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Spell.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Spell.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_Spell.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Spell.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Spell.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Spell.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_Spell.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSpellNull()
		{
			return IsNull(tableR_Spell.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_Spell.SpellColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_Spell.GuardColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGuardNull()
		{
			base[tableR_Spell.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Spell.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Spell.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Spell.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Spell.HPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLevelNull()
		{
			return IsNull(tableR_Spell.LevelColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLevelNull()
		{
			base[tableR_Spell.LevelColumn] = Convert.DBNull;
		}
	}

	public class R_ChestRow : DataRow
	{
		private R_ChestDataTable tableR_Chest;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Chest.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Chest.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_Chest.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.ArtColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Class
		{
			get
			{
				try
				{
					return (string)base[tableR_Chest.ClassColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Class' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.ClassColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string _Relic_C
		{
			get
			{
				try
				{
					return (string)base[tableR_Chest._Relic_CColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Relic-C' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest._Relic_CColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.AddressColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Chest.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Chest' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Chest.HPColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_ChestRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Chest = (R_ChestDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Chest.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_Chest.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Chest.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Chest.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Chest.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Chest.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Chest.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Chest.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Chest.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_Chest.ObjectColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsArtNull()
		{
			return IsNull(tableR_Chest.ArtColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtNull()
		{
			base[tableR_Chest.ArtColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsClassNull()
		{
			return IsNull(tableR_Chest.ClassColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetClassNull()
		{
			base[tableR_Chest.ClassColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool Is_Relic_CNull()
		{
			return IsNull(tableR_Chest._Relic_CColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void Set_Relic_CNull()
		{
			base[tableR_Chest._Relic_CColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGoldNull()
		{
			return IsNull(tableR_Chest.GoldColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGoldNull()
		{
			base[tableR_Chest.GoldColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Chest.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Chest.AddressColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHPNull()
		{
			return IsNull(tableR_Chest.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Chest.HPColumn] = Convert.DBNull;
		}
	}

	public class R_SkillRow : DataRow
	{
		private R_SkillDataTable tableR_Skill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Skill.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Skill.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Skill.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Skill.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Skill.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Skill.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.AddressColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Skill
		{
			get
			{
				try
				{
					return (string)base[tableR_Skill.SkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Skill' в таблице 'R_Skill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Skill.SkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_SkillRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Skill = (R_SkillDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Skill.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Skill.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Skill.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Skill.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Skill.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Skill.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Skill.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Skill.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Skill.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_Skill.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Skill.AddressColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAddressNull()
		{
			base[tableR_Skill.AddressColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSkillNull()
		{
			return IsNull(tableR_Skill.SkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSkillNull()
		{
			base[tableR_Skill.SkillColumn] = Convert.DBNull;
		}
	}

	public class R_CampRow : DataRow
	{
		private R_CampDataTable tableR_Camp;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Camp.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Camp.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_Camp.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.MonsterColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Level
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.LevelColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Number
		{
			get
			{
				try
				{
					return (int)base[tableR_Camp.NumberColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Number' в таблице 'R_Camp' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Camp.NumberColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_CampRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Camp = (R_CampDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Camp.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Camp.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Camp.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Camp.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Camp.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Camp.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Camp.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Camp.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Camp.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_Camp.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_Camp.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_Camp.MonsterColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLevelNull()
		{
			return IsNull(tableR_Camp.LevelColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLevelNull()
		{
			base[tableR_Camp.LevelColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Camp.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Camp.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsNumberNull()
		{
			return IsNull(tableR_Camp.NumberColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetNumberNull()
		{
			base[tableR_Camp.NumberColumn] = Convert.DBNull;
		}
	}

	public class R_MarketRow : DataRow
	{
		private R_MarketDataTable tableR_Market;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Market.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Market.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.SlotColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_Market.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.ArtColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Class
		{
			get
			{
				try
				{
					return (string)base[tableR_Market.ClassColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Class' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.ClassColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string _Relic_C
		{
			get
			{
				try
				{
					return (string)base[tableR_Market._Relic_CColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Relic-C' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market._Relic_CColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Cost
		{
			get
			{
				try
				{
					return (int)base[tableR_Market.CostColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Cost' в таблице 'R_Market' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Market.CostColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_MarketRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Market = (R_MarketDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_Market.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Market.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Market.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Market.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Market.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Market.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Market.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Market.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Market.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_Market.ObjectColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSlotNull()
		{
			return IsNull(tableR_Market.SlotColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSlotNull()
		{
			base[tableR_Market.SlotColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsArtNull()
		{
			return IsNull(tableR_Market.ArtColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtNull()
		{
			base[tableR_Market.ArtColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsClassNull()
		{
			return IsNull(tableR_Market.ClassColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetClassNull()
		{
			base[tableR_Market.ClassColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool Is_Relic_CNull()
		{
			return IsNull(tableR_Market._Relic_CColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void Set_Relic_CNull()
		{
			base[tableR_Market._Relic_CColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Market.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Market.AddressColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsCostNull()
		{
			return IsNull(tableR_Market.CostColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetCostNull()
		{
			base[tableR_Market.CostColumn] = Convert.DBNull;
		}
	}

	public class R_GarrisonRow : DataRow
	{
		private R_GarrisonDataTable tableR_Garrison;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				return (int)base[tableR_Garrison.XColumn];
			}
			set
			{
				base[tableR_Garrison.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				return (int)base[tableR_Garrison.YColumn];
			}
			set
			{
				base[tableR_Garrison.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				return (int)base[tableR_Garrison.ZColumn];
			}
			set
			{
				base[tableR_Garrison.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Garrison.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string AntiMagic
		{
			get
			{
				try
				{
					return (string)base[tableR_Garrison.AntiMagicColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'AntiMagic' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.AntiMagicColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_Garrison.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_Garrison.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.ColorColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string CanTake
		{
			get
			{
				try
				{
					return (string)base[tableR_Garrison.CanTakeColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'CanTake' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.CanTakeColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Garrison.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Garrison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Garrison.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_GarrisonRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Garrison = (R_GarrisonDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Garrison.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Garrison.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAntiMagicNull()
		{
			return IsNull(tableR_Garrison.AntiMagicColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAntiMagicNull()
		{
			base[tableR_Garrison.AntiMagicColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_Garrison.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_Garrison.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsColorNull()
		{
			return IsNull(tableR_Garrison.ColorColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetColorNull()
		{
			base[tableR_Garrison.ColorColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsCanTakeNull()
		{
			return IsNull(tableR_Garrison.CanTakeColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetCanTakeNull()
		{
			base[tableR_Garrison.CanTakeColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Garrison.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Garrison.HPColumn] = Convert.DBNull;
		}
	}

	public class R_SeerHutRow : DataRow
	{
		private R_SeerHutDataTable tableR_SeerHut;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				return (int)base[tableR_SeerHut.XColumn];
			}
			set
			{
				base[tableR_SeerHut.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				return (int)base[tableR_SeerHut.YColumn];
			}
			set
			{
				base[tableR_SeerHut.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				return (int)base[tableR_SeerHut.ZColumn];
			}
			set
			{
				base[tableR_SeerHut.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_SeerHut.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_SeerHut' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_SeerHut.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_SeerHut.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_SeerHut' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_SeerHut.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Reward
		{
			get
			{
				try
				{
					return (string)base[tableR_SeerHut.RewardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Reward' в таблице 'R_SeerHut' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_SeerHut.RewardColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Deadline
		{
			get
			{
				try
				{
					return (string)base[tableR_SeerHut.DeadlineColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Deadline' в таблице 'R_SeerHut' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_SeerHut.DeadlineColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_SeerHutRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_SeerHut = (R_SeerHutDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_SeerHut.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_SeerHut.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMissionNull()
		{
			return IsNull(tableR_SeerHut.MissionColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMissionNull()
		{
			base[tableR_SeerHut.MissionColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsRewardNull()
		{
			return IsNull(tableR_SeerHut.RewardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetRewardNull()
		{
			base[tableR_SeerHut.RewardColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsDeadlineNull()
		{
			return IsNull(tableR_SeerHut.DeadlineColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetDeadlineNull()
		{
			base[tableR_SeerHut.DeadlineColumn] = Convert.DBNull;
		}
	}

	public class R_PrisonRow : DataRow
	{
		private R_PrisonDataTable tableR_Prison;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Hero
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.HeroColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hero' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.HeroColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Level
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.LevelColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string PrimarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.PrimarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'PrimarySkill' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.PrimarySkillColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string SecondarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.SecondarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'SecondarySkill' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.SecondarySkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.ArtColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.SpellColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.MonsterColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Machine
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.MachineColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Machine' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.MachineColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Book
		{
			get
			{
				try
				{
					return (string)base[tableR_Prison.BookColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Book' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.BookColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int MP
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.MPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'MP' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.MPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Experience
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.ExperienceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Experience' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.ExperienceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Prison.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Prison' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Prison.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_PrisonRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Prison = (R_PrisonDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_Prison.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_Prison.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Prison.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_Prison.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Prison.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Prison.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Prison.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Prison.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHeroNull()
		{
			return IsNull(tableR_Prison.HeroColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHeroNull()
		{
			base[tableR_Prison.HeroColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLevelNull()
		{
			return IsNull(tableR_Prison.LevelColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLevelNull()
		{
			base[tableR_Prison.LevelColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsPrimarySkillNull()
		{
			return IsNull(tableR_Prison.PrimarySkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetPrimarySkillNull()
		{
			base[tableR_Prison.PrimarySkillColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSecondarySkillNull()
		{
			return IsNull(tableR_Prison.SecondarySkillColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSecondarySkillNull()
		{
			base[tableR_Prison.SecondarySkillColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsArtNull()
		{
			return IsNull(tableR_Prison.ArtColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtNull()
		{
			base[tableR_Prison.ArtColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSpellNull()
		{
			return IsNull(tableR_Prison.SpellColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSpellNull()
		{
			base[tableR_Prison.SpellColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_Prison.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_Prison.MonsterColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMachineNull()
		{
			return IsNull(tableR_Prison.MachineColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMachineNull()
		{
			base[tableR_Prison.MachineColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsBookNull()
		{
			return IsNull(tableR_Prison.BookColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetBookNull()
		{
			base[tableR_Prison.BookColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMPNull()
		{
			return IsNull(tableR_Prison.MPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMPNull()
		{
			base[tableR_Prison.MPColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsExperienceNull()
		{
			return IsNull(tableR_Prison.ExperienceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetExperienceNull()
		{
			base[tableR_Prison.ExperienceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Prison.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Prison.HPColumn] = Convert.DBNull;
		}
	}

	public class R_ObjectRow : DataRow
	{
		private R_ObjectDataTable tableR_Object;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Object.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Object.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Object.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Object.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Object.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Object.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Payment
		{
			get
			{
				try
				{
					return (string)base[tableR_Object.PaymentColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Payment' в таблице 'R_Object' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Object.PaymentColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_ObjectRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Object = (R_ObjectDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Object.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_Object.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Object.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Object.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Object.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Object.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Object.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_Object.LocalityColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Object.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_Object.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Object.AddressColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetAddressNull()
		{
			base[tableR_Object.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPaymentNull()
		{
			return IsNull(tableR_Object.PaymentColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetPaymentNull()
		{
			base[tableR_Object.PaymentColumn] = Convert.DBNull;
		}
	}

	public class R_PassGuardRow : DataRow
	{
		private R_PassGuardDataTable tableR_PassGuard;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				return (int)base[tableR_PassGuard.XColumn];
			}
			set
			{
				base[tableR_PassGuard.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				return (int)base[tableR_PassGuard.YColumn];
			}
			set
			{
				base[tableR_PassGuard.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				return (int)base[tableR_PassGuard.ZColumn];
			}
			set
			{
				base[tableR_PassGuard.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_PassGuard.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_PassGuard' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_PassGuard.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_PassGuard.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_PassGuard' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_PassGuard.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Deadline
		{
			get
			{
				try
				{
					return (string)base[tableR_PassGuard.DeadlineColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Deadline' в таблице 'R_PassGuard' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_PassGuard.DeadlineColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_PassGuardRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_PassGuard = (R_PassGuardDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_PassGuard.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_PassGuard.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMissionNull()
		{
			return IsNull(tableR_PassGuard.MissionColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMissionNull()
		{
			base[tableR_PassGuard.MissionColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsDeadlineNull()
		{
			return IsNull(tableR_PassGuard.DeadlineColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetDeadlineNull()
		{
			base[tableR_PassGuard.DeadlineColumn] = Convert.DBNull;
		}
	}

	public class R_HeroesRow : DataRow
	{
		private R_HeroesDataTable tableR_Heroes;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int ID
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.IDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ID' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.IDColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Hero
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.HeroColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hero' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.HeroColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Place
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.PlaceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Place' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.PlaceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.ColorColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Level
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.LevelColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string PrimarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.PrimarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'PrimarySkill' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.PrimarySkillColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string SecondarySkill
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.SecondarySkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'SecondarySkill' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.SecondarySkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.ArtColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.MonsterColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Machine
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.MachineColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Machine' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.MachineColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Book
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.BookColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Book' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.BookColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int MP
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.MPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'MP' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.MPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Experience
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.ExperienceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Experience' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.ExperienceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Address
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.AddressColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Address' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.AddressColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Hire
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.HireColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hire' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.HireColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Ideology
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.IdeologyColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Ideology' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.IdeologyColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Class
		{
			get
			{
				try
				{
					return (string)base[tableR_Heroes.ClassColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Class' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.ClassColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int ClassID
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.ClassIDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ClassID' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.ClassIDColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int TreeNumber
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.TreeNumberColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'TreeNumber' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.TreeNumberColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int LastWisdom
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.LastWisdomColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'LastWisdom' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.LastWisdomColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int LastMagic
		{
			get
			{
				try
				{
					return (int)base[tableR_Heroes.LastMagicColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'LastMagic' в таблице 'R_Heroes' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Heroes.LastMagicColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_HeroesRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Heroes = (R_HeroesDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsIDNull()
		{
			return IsNull(tableR_Heroes.IDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIDNull()
		{
			base[tableR_Heroes.IDColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHeroNull()
		{
			return IsNull(tableR_Heroes.HeroColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHeroNull()
		{
			base[tableR_Heroes.HeroColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsPlaceNull()
		{
			return IsNull(tableR_Heroes.PlaceColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPlaceNull()
		{
			base[tableR_Heroes.PlaceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsColorNull()
		{
			return IsNull(tableR_Heroes.ColorColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetColorNull()
		{
			base[tableR_Heroes.ColorColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLevelNull()
		{
			return IsNull(tableR_Heroes.LevelColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLevelNull()
		{
			base[tableR_Heroes.LevelColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPrimarySkillNull()
		{
			return IsNull(tableR_Heroes.PrimarySkillColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPrimarySkillNull()
		{
			base[tableR_Heroes.PrimarySkillColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSecondarySkillNull()
		{
			return IsNull(tableR_Heroes.SecondarySkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSecondarySkillNull()
		{
			base[tableR_Heroes.SecondarySkillColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsArtNull()
		{
			return IsNull(tableR_Heroes.ArtColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetArtNull()
		{
			base[tableR_Heroes.ArtColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSpellNull()
		{
			return IsNull(tableR_Heroes.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_Heroes.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_Heroes.MonsterColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMonsterNull()
		{
			base[tableR_Heroes.MonsterColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMachineNull()
		{
			return IsNull(tableR_Heroes.MachineColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMachineNull()
		{
			base[tableR_Heroes.MachineColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsBookNull()
		{
			return IsNull(tableR_Heroes.BookColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetBookNull()
		{
			base[tableR_Heroes.BookColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMPNull()
		{
			return IsNull(tableR_Heroes.MPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMPNull()
		{
			base[tableR_Heroes.MPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsExperienceNull()
		{
			return IsNull(tableR_Heroes.ExperienceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetExperienceNull()
		{
			base[tableR_Heroes.ExperienceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAddressNull()
		{
			return IsNull(tableR_Heroes.AddressColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAddressNull()
		{
			base[tableR_Heroes.AddressColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Heroes.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_Heroes.HPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHireNull()
		{
			return IsNull(tableR_Heroes.HireColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHireNull()
		{
			base[tableR_Heroes.HireColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsIdeologyNull()
		{
			return IsNull(tableR_Heroes.IdeologyColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIdeologyNull()
		{
			base[tableR_Heroes.IdeologyColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsClassNull()
		{
			return IsNull(tableR_Heroes.ClassColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetClassNull()
		{
			base[tableR_Heroes.ClassColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsClassIDNull()
		{
			return IsNull(tableR_Heroes.ClassIDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetClassIDNull()
		{
			base[tableR_Heroes.ClassIDColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsTreeNumberNull()
		{
			return IsNull(tableR_Heroes.TreeNumberColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetTreeNumberNull()
		{
			base[tableR_Heroes.TreeNumberColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLastWisdomNull()
		{
			return IsNull(tableR_Heroes.LastWisdomColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLastWisdomNull()
		{
			base[tableR_Heroes.LastWisdomColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLastMagicNull()
		{
			return IsNull(tableR_Heroes.LastMagicColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLastMagicNull()
		{
			base[tableR_Heroes.LastMagicColumn] = Convert.DBNull;
		}
	}

	public class R_TownRow : DataRow
	{
		private R_TownDataTable tableR_Town;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Name
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.NameColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Name' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.NameColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Type
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.TypeColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Type' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.TypeColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.SlotColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.ColorColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Garrison
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.GarrisonColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Garrison' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.GarrisonColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Code
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.CodeColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Code' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.CodeColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Built
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.BuiltColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Built' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.BuiltColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.HPColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Library
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.LibraryColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Library' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.LibraryColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int ID
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.IDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ID' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.IDColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int MageTimer
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.MageTimerColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'MageTimer' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.MageTimerColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int LibTimer
		{
			get
			{
				try
				{
					return (int)base[tableR_Town.LibTimerColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'LibTimer' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.LibTimerColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Available
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.AvailableColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Available' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.AvailableColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Timer
		{
			get
			{
				try
				{
					return (string)base[tableR_Town.TimerColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Timer' в таблице 'R_Town' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Town.TimerColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_TownRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Town = (R_TownDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_Town.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Town.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Town.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Town.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_Town.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_Town.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsNameNull()
		{
			return IsNull(tableR_Town.NameColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetNameNull()
		{
			base[tableR_Town.NameColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsTypeNull()
		{
			return IsNull(tableR_Town.TypeColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetTypeNull()
		{
			base[tableR_Town.TypeColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSlotNull()
		{
			return IsNull(tableR_Town.SlotColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSlotNull()
		{
			base[tableR_Town.SlotColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSpellNull()
		{
			return IsNull(tableR_Town.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_Town.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsColorNull()
		{
			return IsNull(tableR_Town.ColorColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetColorNull()
		{
			base[tableR_Town.ColorColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGarrisonNull()
		{
			return IsNull(tableR_Town.GarrisonColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGarrisonNull()
		{
			base[tableR_Town.GarrisonColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsCodeNull()
		{
			return IsNull(tableR_Town.CodeColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetCodeNull()
		{
			base[tableR_Town.CodeColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsBuiltNull()
		{
			return IsNull(tableR_Town.BuiltColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetBuiltNull()
		{
			base[tableR_Town.BuiltColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_Town.HPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHPNull()
		{
			base[tableR_Town.HPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLibraryNull()
		{
			return IsNull(tableR_Town.LibraryColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLibraryNull()
		{
			base[tableR_Town.LibraryColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsIDNull()
		{
			return IsNull(tableR_Town.IDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIDNull()
		{
			base[tableR_Town.IDColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMageTimerNull()
		{
			return IsNull(tableR_Town.MageTimerColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMageTimerNull()
		{
			base[tableR_Town.MageTimerColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLibTimerNull()
		{
			return IsNull(tableR_Town.LibTimerColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLibTimerNull()
		{
			base[tableR_Town.LibTimerColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsAvailableNull()
		{
			return IsNull(tableR_Town.AvailableColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetAvailableNull()
		{
			base[tableR_Town.AvailableColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsTimerNull()
		{
			return IsNull(tableR_Town.TimerColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetTimerNull()
		{
			base[tableR_Town.TimerColumn] = Convert.DBNull;
		}
	}

	public class R_AllArtsRow : DataRow
	{
		private R_AllArtsDataTable tableR_AllArts;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_AllArts.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_AllArts.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_AllArts.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Artefact
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.ArtefactColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Artefact' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.ArtefactColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Class
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.ClassColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Class' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.ClassColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string _Relic_C
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts._Relic_CColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Relic-C' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts._Relic_CColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_AllArts.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.SlotColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Place
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.PlaceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Place' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.PlaceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.ColorColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Hero
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.HeroColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hero' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.HeroColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Doll
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.DollColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Doll' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.DollColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.MonsterColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_AllArts.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_AllArts' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllArts.GuardColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_AllArtsRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_AllArts = (R_AllArtsDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_AllArts.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_AllArts.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_AllArts.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_AllArts.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_AllArts.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_AllArts.ZColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_AllArts.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_AllArts.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_AllArts.ObjectColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetObjectNull()
		{
			base[tableR_AllArts.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsArtefactNull()
		{
			return IsNull(tableR_AllArts.ArtefactColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetArtefactNull()
		{
			base[tableR_AllArts.ArtefactColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsClassNull()
		{
			return IsNull(tableR_AllArts.ClassColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetClassNull()
		{
			base[tableR_AllArts.ClassColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool Is_Relic_CNull()
		{
			return IsNull(tableR_AllArts._Relic_CColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void Set_Relic_CNull()
		{
			base[tableR_AllArts._Relic_CColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSlotNull()
		{
			return IsNull(tableR_AllArts.SlotColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSlotNull()
		{
			base[tableR_AllArts.SlotColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsPlaceNull()
		{
			return IsNull(tableR_AllArts.PlaceColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPlaceNull()
		{
			base[tableR_AllArts.PlaceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsColorNull()
		{
			return IsNull(tableR_AllArts.ColorColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetColorNull()
		{
			base[tableR_AllArts.ColorColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHeroNull()
		{
			return IsNull(tableR_AllArts.HeroColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHeroNull()
		{
			base[tableR_AllArts.HeroColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsDollNull()
		{
			return IsNull(tableR_AllArts.DollColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetDollNull()
		{
			base[tableR_AllArts.DollColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_AllArts.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_AllArts.MonsterColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsMissionNull()
		{
			return IsNull(tableR_AllArts.MissionColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMissionNull()
		{
			base[tableR_AllArts.MissionColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_AllArts.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_AllArts.GuardColumn] = Convert.DBNull;
		}
	}

	public class R_TopologyRow : DataRow
	{
		private R_TopologyDataTable tableR_Topology;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Topology.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Topology.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Topology.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.ZColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Topology.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_Topology.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Type
		{
			get
			{
				try
				{
					return (string)base[tableR_Topology.TypeColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Type' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.TypeColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_Topology.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.ColorColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Pair
		{
			get
			{
				try
				{
					return (int)base[tableR_Topology.PairColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Pair' в таблице 'R_Topology' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Topology.PairColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_TopologyRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Topology = (R_TopologyDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_Topology.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Topology.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_Topology.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Topology.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Topology.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_Topology.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Topology.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Topology.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_Topology.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_Topology.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsTypeNull()
		{
			return IsNull(tableR_Topology.TypeColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetTypeNull()
		{
			base[tableR_Topology.TypeColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsColorNull()
		{
			return IsNull(tableR_Topology.ColorColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetColorNull()
		{
			base[tableR_Topology.ColorColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPairNull()
		{
			return IsNull(tableR_Topology.PairColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPairNull()
		{
			base[tableR_Topology.PairColumn] = Convert.DBNull;
		}
	}

	public class R_AllSpellRow : DataRow
	{
		private R_AllSpellDataTable tableR_AllSpell;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSpell.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSpell.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSpell.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSpell.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.SlotColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.ColorColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Name
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.NameColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Name' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.NameColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Built
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.BuiltColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Built' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.BuiltColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Garrison
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.GarrisonColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Garrison' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.GarrisonColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int ID
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSpell.IDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ID' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.IDColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Hero
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.HeroColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hero' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.HeroColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Place
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.PlaceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Place' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.PlaceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSpell.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_AllSpell' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSpell.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_AllSpellRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_AllSpell = (R_AllSpellDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_AllSpell.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_AllSpell.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_AllSpell.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_AllSpell.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_AllSpell.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_AllSpell.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsObjectNull()
		{
			return IsNull(tableR_AllSpell.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_AllSpell.ObjectColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSpellNull()
		{
			return IsNull(tableR_AllSpell.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_AllSpell.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsSlotNull()
		{
			return IsNull(tableR_AllSpell.SlotColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSlotNull()
		{
			base[tableR_AllSpell.SlotColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsColorNull()
		{
			return IsNull(tableR_AllSpell.ColorColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetColorNull()
		{
			base[tableR_AllSpell.ColorColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsNameNull()
		{
			return IsNull(tableR_AllSpell.NameColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetNameNull()
		{
			base[tableR_AllSpell.NameColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsBuiltNull()
		{
			return IsNull(tableR_AllSpell.BuiltColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetBuiltNull()
		{
			base[tableR_AllSpell.BuiltColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGarrisonNull()
		{
			return IsNull(tableR_AllSpell.GarrisonColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGarrisonNull()
		{
			base[tableR_AllSpell.GarrisonColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsIDNull()
		{
			return IsNull(tableR_AllSpell.IDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIDNull()
		{
			base[tableR_AllSpell.IDColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHeroNull()
		{
			return IsNull(tableR_AllSpell.HeroColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHeroNull()
		{
			base[tableR_AllSpell.HeroColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsPlaceNull()
		{
			return IsNull(tableR_AllSpell.PlaceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetPlaceNull()
		{
			base[tableR_AllSpell.PlaceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMissionNull()
		{
			return IsNull(tableR_AllSpell.MissionColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMissionNull()
		{
			base[tableR_AllSpell.MissionColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGuardNull()
		{
			return IsNull(tableR_AllSpell.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_AllSpell.GuardColumn] = Convert.DBNull;
		}
	}

	public class R_AllTimerRow : DataRow
	{
		private R_AllTimerDataTable tableR_AllTimer;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Day
		{
			get
			{
				try
				{
					return (int)base[tableR_AllTimer.DayColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Day' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.DayColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Repeat
		{
			get
			{
				try
				{
					return (int)base[tableR_AllTimer.RepeatColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Repeat' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.RepeatColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Town
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.TownColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Town' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.TownColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Type
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.TypeColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Type' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.TypeColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Place
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.PlaceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Place' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.PlaceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Gold
		{
			get
			{
				try
				{
					return (int)base[tableR_AllTimer.GoldColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Gold' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.GoldColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.ResourceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Building
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.BuildingColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Building' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.BuildingColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.MonsterColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Apply
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.ApplyColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Apply' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.ApplyColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int ID
		{
			get
			{
				try
				{
					return (int)base[tableR_AllTimer.IDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ID' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.IDColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_AllTimer.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_AllTimer' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllTimer.ColorColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_AllTimerRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_AllTimer = (R_AllTimerDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_AllTimer.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_AllTimer.ObjectColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsDayNull()
		{
			return IsNull(tableR_AllTimer.DayColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetDayNull()
		{
			base[tableR_AllTimer.DayColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsRepeatNull()
		{
			return IsNull(tableR_AllTimer.RepeatColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetRepeatNull()
		{
			base[tableR_AllTimer.RepeatColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsTownNull()
		{
			return IsNull(tableR_AllTimer.TownColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetTownNull()
		{
			base[tableR_AllTimer.TownColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsTypeNull()
		{
			return IsNull(tableR_AllTimer.TypeColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetTypeNull()
		{
			base[tableR_AllTimer.TypeColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsPlaceNull()
		{
			return IsNull(tableR_AllTimer.PlaceColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetPlaceNull()
		{
			base[tableR_AllTimer.PlaceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsGoldNull()
		{
			return IsNull(tableR_AllTimer.GoldColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetGoldNull()
		{
			base[tableR_AllTimer.GoldColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsResourceNull()
		{
			return IsNull(tableR_AllTimer.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_AllTimer.ResourceColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsBuildingNull()
		{
			return IsNull(tableR_AllTimer.BuildingColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetBuildingNull()
		{
			base[tableR_AllTimer.BuildingColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_AllTimer.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_AllTimer.MonsterColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsApplyNull()
		{
			return IsNull(tableR_AllTimer.ApplyColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetApplyNull()
		{
			base[tableR_AllTimer.ApplyColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsIDNull()
		{
			return IsNull(tableR_AllTimer.IDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIDNull()
		{
			base[tableR_AllTimer.IDColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsColorNull()
		{
			return IsNull(tableR_AllTimer.ColorColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetColorNull()
		{
			base[tableR_AllTimer.ColorColumn] = Convert.DBNull;
		}
	}

	public class R_AllSkillRow : DataRow
	{
		private R_AllSkillDataTable tableR_AllSkill;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSkill.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.XColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSkill.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSkill.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.ObjectColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Skill
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.SkillColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Skill' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.SkillColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.GuardColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Slot
		{
			get
			{
				try
				{
					return (int)base[tableR_AllSkill.SlotColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Slot' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.SlotColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Level
		{
			get
			{
				try
				{
					return (string)base[tableR_AllSkill.LevelColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Level' в таблице 'R_AllSkill' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllSkill.LevelColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		internal R_AllSkillRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_AllSkill = (R_AllSkillDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_AllSkill.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_AllSkill.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_AllSkill.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_AllSkill.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_AllSkill.ZColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetZNull()
		{
			base[tableR_AllSkill.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_AllSkill.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_AllSkill.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_AllSkill.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_AllSkill.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSkillNull()
		{
			return IsNull(tableR_AllSkill.SkillColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetSkillNull()
		{
			base[tableR_AllSkill.SkillColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMissionNull()
		{
			return IsNull(tableR_AllSkill.MissionColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetMissionNull()
		{
			base[tableR_AllSkill.MissionColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGuardNull()
		{
			return IsNull(tableR_AllSkill.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_AllSkill.GuardColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSlotNull()
		{
			return IsNull(tableR_AllSkill.SlotColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSlotNull()
		{
			base[tableR_AllSkill.SlotColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLevelNull()
		{
			return IsNull(tableR_AllSkill.LevelColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLevelNull()
		{
			base[tableR_AllSkill.LevelColumn] = Convert.DBNull;
		}
	}

	public class R_AllExperienceRow : DataRow
	{
		private R_AllExperienceDataTable tableR_AllExperience;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.YColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.LocalityColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Object
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.ObjectColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Object' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ObjectColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Town
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.TownColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Town' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.TownColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Monster
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.MonsterColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Monster' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.MonsterColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Art
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.ArtColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Art' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ArtColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Resource
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.ResourceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Resource' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ResourceColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Guard
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.GuardColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Guard' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.GuardColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Mission
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.MissionColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Mission' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.MissionColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Spell
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.SpellColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Spell' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.SpellColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int ID
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.IDColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'ID' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.IDColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Hero
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.HeroColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Hero' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.HeroColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public string Color
		{
			get
			{
				try
				{
					return (string)base[tableR_AllExperience.ColorColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Color' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ColorColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public int Experience
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.ExperienceColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Experience' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.ExperienceColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int HP
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.HPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'HP' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.HPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int XP
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.XPColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'XP' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.XPColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int XP5
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.XP5Column];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'XP5' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.XP5Column] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int XP10
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.XP10Column];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'XP10' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.XP10Column] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int XP15
		{
			get
			{
				try
				{
					return (int)base[tableR_AllExperience.XP15Column];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'XP15' в таблице 'R_AllExperience' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_AllExperience.XP15Column] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_AllExperienceRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_AllExperience = (R_AllExperienceDataTable)base.Table;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXNull()
		{
			return IsNull(tableR_AllExperience.XColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXNull()
		{
			base[tableR_AllExperience.XColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsYNull()
		{
			return IsNull(tableR_AllExperience.YColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetYNull()
		{
			base[tableR_AllExperience.YColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsZNull()
		{
			return IsNull(tableR_AllExperience.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_AllExperience.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_AllExperience.LocalityColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetLocalityNull()
		{
			base[tableR_AllExperience.LocalityColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsObjectNull()
		{
			return IsNull(tableR_AllExperience.ObjectColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetObjectNull()
		{
			base[tableR_AllExperience.ObjectColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsTownNull()
		{
			return IsNull(tableR_AllExperience.TownColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetTownNull()
		{
			base[tableR_AllExperience.TownColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMonsterNull()
		{
			return IsNull(tableR_AllExperience.MonsterColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMonsterNull()
		{
			base[tableR_AllExperience.MonsterColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsArtNull()
		{
			return IsNull(tableR_AllExperience.ArtColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetArtNull()
		{
			base[tableR_AllExperience.ArtColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsResourceNull()
		{
			return IsNull(tableR_AllExperience.ResourceColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetResourceNull()
		{
			base[tableR_AllExperience.ResourceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsGuardNull()
		{
			return IsNull(tableR_AllExperience.GuardColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetGuardNull()
		{
			base[tableR_AllExperience.GuardColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsMissionNull()
		{
			return IsNull(tableR_AllExperience.MissionColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetMissionNull()
		{
			base[tableR_AllExperience.MissionColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsSpellNull()
		{
			return IsNull(tableR_AllExperience.SpellColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetSpellNull()
		{
			base[tableR_AllExperience.SpellColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsIDNull()
		{
			return IsNull(tableR_AllExperience.IDColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetIDNull()
		{
			base[tableR_AllExperience.IDColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsHeroNull()
		{
			return IsNull(tableR_AllExperience.HeroColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetHeroNull()
		{
			base[tableR_AllExperience.HeroColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsColorNull()
		{
			return IsNull(tableR_AllExperience.ColorColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetColorNull()
		{
			base[tableR_AllExperience.ColorColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsExperienceNull()
		{
			return IsNull(tableR_AllExperience.ExperienceColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetExperienceNull()
		{
			base[tableR_AllExperience.ExperienceColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsHPNull()
		{
			return IsNull(tableR_AllExperience.HPColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetHPNull()
		{
			base[tableR_AllExperience.HPColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXPNull()
		{
			return IsNull(tableR_AllExperience.XPColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXPNull()
		{
			base[tableR_AllExperience.XPColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXP5Null()
		{
			return IsNull(tableR_AllExperience.XP5Column);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXP5Null()
		{
			base[tableR_AllExperience.XP5Column] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsXP10Null()
		{
			return IsNull(tableR_AllExperience.XP10Column);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetXP10Null()
		{
			base[tableR_AllExperience.XP10Column] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXP15Null()
		{
			return IsNull(tableR_AllExperience.XP15Column);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXP15Null()
		{
			base[tableR_AllExperience.XP15Column] = Convert.DBNull;
		}
	}

	public class R_MineRow : DataRow
	{
		private R_MineDataTable tableR_Mine;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int X
		{
			get
			{
				try
				{
					return (int)base[tableR_Mine.XColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'X' в таблице 'R_Mine' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Mine.XColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Y
		{
			get
			{
				try
				{
					return (int)base[tableR_Mine.YColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Y' в таблице 'R_Mine' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Mine.YColumn] = value;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public int Z
		{
			get
			{
				try
				{
					return (int)base[tableR_Mine.ZColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Z' в таблице 'R_Mine' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Mine.ZColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public string Locality
		{
			get
			{
				try
				{
					return (string)base[tableR_Mine.LocalityColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException("Значение для столбца 'Locality' в таблице 'R_Mine' равно DBNull.", innerException);
				}
			}
			set
			{
				base[tableR_Mine.LocalityColumn] = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal R_MineRow(DataRowBuilder rb)
			: base(rb)
		{
			tableR_Mine = (R_MineDataTable)base.Table;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsXNull()
		{
			return IsNull(tableR_Mine.XColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetXNull()
		{
			base[tableR_Mine.XColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsYNull()
		{
			return IsNull(tableR_Mine.YColumn);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void SetYNull()
		{
			base[tableR_Mine.YColumn] = Convert.DBNull;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public bool IsZNull()
		{
			return IsNull(tableR_Mine.ZColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetZNull()
		{
			base[tableR_Mine.ZColumn] = Convert.DBNull;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public bool IsLocalityNull()
		{
			return IsNull(tableR_Mine.LocalityColumn);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public void SetLocalityNull()
		{
			base[tableR_Mine.LocalityColumn] = Convert.DBNull;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_MonstrRowChangeEvent : EventArgs
	{
		private R_MonstrRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MonstrRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MonstrRowChangeEvent(R_MonstrRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_ArtRowChangeEvent : EventArgs
	{
		private R_ArtRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ArtRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ArtRowChangeEvent(R_ArtRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_BankRowChangeEvent : EventArgs
	{
		private R_BankRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_BankRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_BankRowChangeEvent(R_BankRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_EventBoxRowChangeEvent : EventArgs
	{
		private R_EventBoxRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_EventBoxRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_EventBoxRowChangeEvent(R_EventBoxRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_ScholarRowChangeEvent : EventArgs
	{
		private R_ScholarRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ScholarRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ScholarRowChangeEvent(R_ScholarRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_ResourceRowChangeEvent : EventArgs
	{
		private R_ResourceRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ResourceRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ResourceRowChangeEvent(R_ResourceRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_SpellRowChangeEvent : EventArgs
	{
		private R_SpellRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SpellRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SpellRowChangeEvent(R_SpellRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_ChestRowChangeEvent : EventArgs
	{
		private R_ChestRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ChestRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_ChestRowChangeEvent(R_ChestRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_SkillRowChangeEvent : EventArgs
	{
		private R_SkillRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SkillRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SkillRowChangeEvent(R_SkillRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_CampRowChangeEvent : EventArgs
	{
		private R_CampRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_CampRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_CampRowChangeEvent(R_CampRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_MarketRowChangeEvent : EventArgs
	{
		private R_MarketRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_MarketRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MarketRowChangeEvent(R_MarketRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_GarrisonRowChangeEvent : EventArgs
	{
		private R_GarrisonRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_GarrisonRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_GarrisonRowChangeEvent(R_GarrisonRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_SeerHutRowChangeEvent : EventArgs
	{
		private R_SeerHutRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_SeerHutRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_SeerHutRowChangeEvent(R_SeerHutRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_PrisonRowChangeEvent : EventArgs
	{
		private R_PrisonRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PrisonRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PrisonRowChangeEvent(R_PrisonRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_ObjectRowChangeEvent : EventArgs
	{
		private R_ObjectRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ObjectRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_ObjectRowChangeEvent(R_ObjectRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_PassGuardRowChangeEvent : EventArgs
	{
		private R_PassGuardRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PassGuardRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_PassGuardRowChangeEvent(R_PassGuardRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_HeroesRowChangeEvent : EventArgs
	{
		private R_HeroesRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_HeroesRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_HeroesRowChangeEvent(R_HeroesRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_TownRowChangeEvent : EventArgs
	{
		private R_TownRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_TownRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_TownRowChangeEvent(R_TownRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_AllArtsRowChangeEvent : EventArgs
	{
		private R_AllArtsRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllArtsRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllArtsRowChangeEvent(R_AllArtsRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_TopologyRowChangeEvent : EventArgs
	{
		private R_TopologyRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyRow Row => eventRow;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_TopologyRowChangeEvent(R_TopologyRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_AllSpellRowChangeEvent : EventArgs
	{
		private R_AllSpellRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSpellRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSpellRowChangeEvent(R_AllSpellRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_AllTimerRowChangeEvent : EventArgs
	{
		private R_AllTimerRow eventRow;

		private DataRowAction eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllTimerRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllTimerRowChangeEvent(R_AllTimerRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_AllSkillRowChangeEvent : EventArgs
	{
		private R_AllSkillRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllSkillRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public R_AllSkillRowChangeEvent(R_AllSkillRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_AllExperienceRowChangeEvent : EventArgs
	{
		private R_AllExperienceRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllExperienceRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_AllExperienceRowChangeEvent(R_AllExperienceRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public class R_MineRowChangeEvent : EventArgs
	{
		private R_MineRow eventRow;

		private DataRowAction eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MineRow Row => eventRow;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public DataRowAction Action => eventAction;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public R_MineRowChangeEvent(R_MineRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}

	private R_MonstrDataTable tableR_Monstr;

	private R_ArtDataTable tableR_Art;

	private R_BankDataTable tableR_Bank;

	private R_EventBoxDataTable tableR_EventBox;

	private R_ScholarDataTable tableR_Scholar;

	private R_ResourceDataTable tableR_Resource;

	private R_SpellDataTable tableR_Spell;

	private R_ChestDataTable tableR_Chest;

	private R_SkillDataTable tableR_Skill;

	private R_CampDataTable tableR_Camp;

	private R_MarketDataTable tableR_Market;

	private R_GarrisonDataTable tableR_Garrison;

	private R_SeerHutDataTable tableR_SeerHut;

	private R_PrisonDataTable tableR_Prison;

	private R_ObjectDataTable tableR_Object;

	private R_PassGuardDataTable tableR_PassGuard;

	private R_HeroesDataTable tableR_Heroes;

	private R_TownDataTable tableR_Town;

	private R_AllArtsDataTable tableR_AllArts;

	private R_TopologyDataTable tableR_Topology;

	private R_AllSpellDataTable tableR_AllSpell;

	private R_AllTimerDataTable tableR_AllTimer;

	private R_AllSkillDataTable tableR_AllSkill;

	private R_AllExperienceDataTable tableR_AllExperience;

	private R_MineDataTable tableR_Mine;

	private SchemaSerializationMode _schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

	[Browsable(false)]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_MonstrDataTable R_Monstr => tableR_Monstr;

	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_ArtDataTable R_Art => tableR_Art;

	[Browsable(false)]
	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_BankDataTable R_Bank => tableR_Bank;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_EventBoxDataTable R_EventBox => tableR_EventBox;

	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_ScholarDataTable R_Scholar => tableR_Scholar;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	public R_ResourceDataTable R_Resource => tableR_Resource;

	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_SpellDataTable R_Spell => tableR_Spell;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	public R_ChestDataTable R_Chest => tableR_Chest;

	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	public R_SkillDataTable R_Skill => tableR_Skill;

	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_CampDataTable R_Camp => tableR_Camp;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_MarketDataTable R_Market => tableR_Market;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_GarrisonDataTable R_Garrison => tableR_Garrison;

	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	public R_SeerHutDataTable R_SeerHut => tableR_SeerHut;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	[DebuggerNonUserCode]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_PrisonDataTable R_Prison => tableR_Prison;

	[DebuggerNonUserCode]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_ObjectDataTable R_Object => tableR_Object;

	[Browsable(false)]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public R_PassGuardDataTable R_PassGuard => tableR_PassGuard;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	[Browsable(false)]
	public R_HeroesDataTable R_Heroes => tableR_Heroes;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	[DebuggerNonUserCode]
	public R_TownDataTable R_Town => tableR_Town;

	[DebuggerNonUserCode]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public R_AllArtsDataTable R_AllArts => tableR_AllArts;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	public R_TopologyDataTable R_Topology => tableR_Topology;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	public R_AllSpellDataTable R_AllSpell => tableR_AllSpell;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	public R_AllTimerDataTable R_AllTimer => tableR_AllTimer;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DebuggerNonUserCode]
	public R_AllSkillDataTable R_AllSkill => tableR_AllSkill;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	[DebuggerNonUserCode]
	public R_AllExperienceDataTable R_AllExperience => tableR_AllExperience;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	[Browsable(false)]
	public R_MineDataTable R_Mine => tableR_Mine;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Browsable(true)]
	[DebuggerNonUserCode]
	public override SchemaSerializationMode SchemaSerializationMode
	{
		get
		{
			return _schemaSerializationMode;
		}
		set
		{
			_schemaSerializationMode = value;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new DataTableCollection Tables => base.Tables;

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DebuggerNonUserCode]
	public new DataRelationCollection Relations => base.Relations;

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	public DataSet2()
	{
		BeginInit();
		InitClass();
		CollectionChangeEventHandler value = SchemaChanged;
		base.Tables.CollectionChanged += value;
		base.Relations.CollectionChanged += value;
		EndInit();
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected DataSet2(SerializationInfo info, StreamingContext context)
		: base(info, context, ConstructSchema: false)
	{
		if (IsBinarySerialized(info, context))
		{
			InitVars(initTable: false);
			CollectionChangeEventHandler value = SchemaChanged;
			Tables.CollectionChanged += value;
			Relations.CollectionChanged += value;
			return;
		}
		string s = (string)info.GetValue("XmlSchema", typeof(string));
		if (DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
		{
			DataSet dataSet = new DataSet();
			dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
			if (dataSet.Tables["R_Monstr"] != null)
			{
				base.Tables.Add(new R_MonstrDataTable(dataSet.Tables["R_Monstr"]));
			}
			if (dataSet.Tables["R_Art"] != null)
			{
				base.Tables.Add(new R_ArtDataTable(dataSet.Tables["R_Art"]));
			}
			if (dataSet.Tables["R_Bank"] != null)
			{
				base.Tables.Add(new R_BankDataTable(dataSet.Tables["R_Bank"]));
			}
			if (dataSet.Tables["R_EventBox"] != null)
			{
				base.Tables.Add(new R_EventBoxDataTable(dataSet.Tables["R_EventBox"]));
			}
			if (dataSet.Tables["R_Scholar"] != null)
			{
				base.Tables.Add(new R_ScholarDataTable(dataSet.Tables["R_Scholar"]));
			}
			if (dataSet.Tables["R_Resource"] != null)
			{
				base.Tables.Add(new R_ResourceDataTable(dataSet.Tables["R_Resource"]));
			}
			if (dataSet.Tables["R_Spell"] != null)
			{
				base.Tables.Add(new R_SpellDataTable(dataSet.Tables["R_Spell"]));
			}
			if (dataSet.Tables["R_Chest"] != null)
			{
				base.Tables.Add(new R_ChestDataTable(dataSet.Tables["R_Chest"]));
			}
			if (dataSet.Tables["R_Skill"] != null)
			{
				base.Tables.Add(new R_SkillDataTable(dataSet.Tables["R_Skill"]));
			}
			if (dataSet.Tables["R_Camp"] != null)
			{
				base.Tables.Add(new R_CampDataTable(dataSet.Tables["R_Camp"]));
			}
			if (dataSet.Tables["R_Market"] != null)
			{
				base.Tables.Add(new R_MarketDataTable(dataSet.Tables["R_Market"]));
			}
			if (dataSet.Tables["R_Garrison"] != null)
			{
				base.Tables.Add(new R_GarrisonDataTable(dataSet.Tables["R_Garrison"]));
			}
			if (dataSet.Tables["R_SeerHut"] != null)
			{
				base.Tables.Add(new R_SeerHutDataTable(dataSet.Tables["R_SeerHut"]));
			}
			if (dataSet.Tables["R_Prison"] != null)
			{
				base.Tables.Add(new R_PrisonDataTable(dataSet.Tables["R_Prison"]));
			}
			if (dataSet.Tables["R_Object"] != null)
			{
				base.Tables.Add(new R_ObjectDataTable(dataSet.Tables["R_Object"]));
			}
			if (dataSet.Tables["R_PassGuard"] != null)
			{
				base.Tables.Add(new R_PassGuardDataTable(dataSet.Tables["R_PassGuard"]));
			}
			if (dataSet.Tables["R_Heroes"] != null)
			{
				base.Tables.Add(new R_HeroesDataTable(dataSet.Tables["R_Heroes"]));
			}
			if (dataSet.Tables["R_Town"] != null)
			{
				base.Tables.Add(new R_TownDataTable(dataSet.Tables["R_Town"]));
			}
			if (dataSet.Tables["R_AllArts"] != null)
			{
				base.Tables.Add(new R_AllArtsDataTable(dataSet.Tables["R_AllArts"]));
			}
			if (dataSet.Tables["R_Topology"] != null)
			{
				base.Tables.Add(new R_TopologyDataTable(dataSet.Tables["R_Topology"]));
			}
			if (dataSet.Tables["R_AllSpell"] != null)
			{
				base.Tables.Add(new R_AllSpellDataTable(dataSet.Tables["R_AllSpell"]));
			}
			if (dataSet.Tables["R_AllTimer"] != null)
			{
				base.Tables.Add(new R_AllTimerDataTable(dataSet.Tables["R_AllTimer"]));
			}
			if (dataSet.Tables["R_AllSkill"] != null)
			{
				base.Tables.Add(new R_AllSkillDataTable(dataSet.Tables["R_AllSkill"]));
			}
			if (dataSet.Tables["R_AllExperience"] != null)
			{
				base.Tables.Add(new R_AllExperienceDataTable(dataSet.Tables["R_AllExperience"]));
			}
			if (dataSet.Tables["R_Mine"] != null)
			{
				base.Tables.Add(new R_MineDataTable(dataSet.Tables["R_Mine"]));
			}
			base.DataSetName = dataSet.DataSetName;
			base.Prefix = dataSet.Prefix;
			base.Namespace = dataSet.Namespace;
			base.Locale = dataSet.Locale;
			base.CaseSensitive = dataSet.CaseSensitive;
			base.EnforceConstraints = dataSet.EnforceConstraints;
			Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
			InitVars();
		}
		else
		{
			ReadXmlSchema(new XmlTextReader(new StringReader(s)));
		}
		GetSerializationData(info, context);
		CollectionChangeEventHandler value2 = SchemaChanged;
		base.Tables.CollectionChanged += value2;
		Relations.CollectionChanged += value2;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected override void InitializeDerivedDataSet()
	{
		BeginInit();
		InitClass();
		EndInit();
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	public override DataSet Clone()
	{
		DataSet2 dataSet = (DataSet2)base.Clone();
		dataSet.InitVars();
		dataSet.SchemaSerializationMode = SchemaSerializationMode;
		return dataSet;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected override bool ShouldSerializeTables()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected override bool ShouldSerializeRelations()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected override void ReadXmlSerializable(XmlReader reader)
	{
		if (DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
		{
			Reset();
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(reader);
			if (dataSet.Tables["R_Monstr"] != null)
			{
				base.Tables.Add(new R_MonstrDataTable(dataSet.Tables["R_Monstr"]));
			}
			if (dataSet.Tables["R_Art"] != null)
			{
				base.Tables.Add(new R_ArtDataTable(dataSet.Tables["R_Art"]));
			}
			if (dataSet.Tables["R_Bank"] != null)
			{
				base.Tables.Add(new R_BankDataTable(dataSet.Tables["R_Bank"]));
			}
			if (dataSet.Tables["R_EventBox"] != null)
			{
				base.Tables.Add(new R_EventBoxDataTable(dataSet.Tables["R_EventBox"]));
			}
			if (dataSet.Tables["R_Scholar"] != null)
			{
				base.Tables.Add(new R_ScholarDataTable(dataSet.Tables["R_Scholar"]));
			}
			if (dataSet.Tables["R_Resource"] != null)
			{
				base.Tables.Add(new R_ResourceDataTable(dataSet.Tables["R_Resource"]));
			}
			if (dataSet.Tables["R_Spell"] != null)
			{
				base.Tables.Add(new R_SpellDataTable(dataSet.Tables["R_Spell"]));
			}
			if (dataSet.Tables["R_Chest"] != null)
			{
				base.Tables.Add(new R_ChestDataTable(dataSet.Tables["R_Chest"]));
			}
			if (dataSet.Tables["R_Skill"] != null)
			{
				base.Tables.Add(new R_SkillDataTable(dataSet.Tables["R_Skill"]));
			}
			if (dataSet.Tables["R_Camp"] != null)
			{
				base.Tables.Add(new R_CampDataTable(dataSet.Tables["R_Camp"]));
			}
			if (dataSet.Tables["R_Market"] != null)
			{
				base.Tables.Add(new R_MarketDataTable(dataSet.Tables["R_Market"]));
			}
			if (dataSet.Tables["R_Garrison"] != null)
			{
				base.Tables.Add(new R_GarrisonDataTable(dataSet.Tables["R_Garrison"]));
			}
			if (dataSet.Tables["R_SeerHut"] != null)
			{
				base.Tables.Add(new R_SeerHutDataTable(dataSet.Tables["R_SeerHut"]));
			}
			if (dataSet.Tables["R_Prison"] != null)
			{
				base.Tables.Add(new R_PrisonDataTable(dataSet.Tables["R_Prison"]));
			}
			if (dataSet.Tables["R_Object"] != null)
			{
				base.Tables.Add(new R_ObjectDataTable(dataSet.Tables["R_Object"]));
			}
			if (dataSet.Tables["R_PassGuard"] != null)
			{
				base.Tables.Add(new R_PassGuardDataTable(dataSet.Tables["R_PassGuard"]));
			}
			if (dataSet.Tables["R_Heroes"] != null)
			{
				base.Tables.Add(new R_HeroesDataTable(dataSet.Tables["R_Heroes"]));
			}
			if (dataSet.Tables["R_Town"] != null)
			{
				base.Tables.Add(new R_TownDataTable(dataSet.Tables["R_Town"]));
			}
			if (dataSet.Tables["R_AllArts"] != null)
			{
				base.Tables.Add(new R_AllArtsDataTable(dataSet.Tables["R_AllArts"]));
			}
			if (dataSet.Tables["R_Topology"] != null)
			{
				base.Tables.Add(new R_TopologyDataTable(dataSet.Tables["R_Topology"]));
			}
			if (dataSet.Tables["R_AllSpell"] != null)
			{
				base.Tables.Add(new R_AllSpellDataTable(dataSet.Tables["R_AllSpell"]));
			}
			if (dataSet.Tables["R_AllTimer"] != null)
			{
				base.Tables.Add(new R_AllTimerDataTable(dataSet.Tables["R_AllTimer"]));
			}
			if (dataSet.Tables["R_AllSkill"] != null)
			{
				base.Tables.Add(new R_AllSkillDataTable(dataSet.Tables["R_AllSkill"]));
			}
			if (dataSet.Tables["R_AllExperience"] != null)
			{
				base.Tables.Add(new R_AllExperienceDataTable(dataSet.Tables["R_AllExperience"]));
			}
			if (dataSet.Tables["R_Mine"] != null)
			{
				base.Tables.Add(new R_MineDataTable(dataSet.Tables["R_Mine"]));
			}
			base.DataSetName = dataSet.DataSetName;
			base.Prefix = dataSet.Prefix;
			base.Namespace = dataSet.Namespace;
			base.Locale = dataSet.Locale;
			base.CaseSensitive = dataSet.CaseSensitive;
			base.EnforceConstraints = dataSet.EnforceConstraints;
			Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
			InitVars();
		}
		else
		{
			ReadXml(reader);
			InitVars();
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	protected override XmlSchema GetSchemaSerializable()
	{
		MemoryStream memoryStream = new MemoryStream();
		WriteXmlSchema(new XmlTextWriter(memoryStream, null));
		memoryStream.Position = 0L;
		return XmlSchema.Read(new XmlTextReader(memoryStream), null);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	internal void InitVars()
	{
		InitVars(initTable: true);
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal void InitVars(bool initTable)
	{
		tableR_Monstr = (R_MonstrDataTable)base.Tables["R_Monstr"];
		if (initTable && tableR_Monstr != null)
		{
			tableR_Monstr.InitVars();
		}
		tableR_Art = (R_ArtDataTable)base.Tables["R_Art"];
		if (initTable && tableR_Art != null)
		{
			tableR_Art.InitVars();
		}
		tableR_Bank = (R_BankDataTable)base.Tables["R_Bank"];
		if (initTable && tableR_Bank != null)
		{
			tableR_Bank.InitVars();
		}
		tableR_EventBox = (R_EventBoxDataTable)base.Tables["R_EventBox"];
		if (initTable && tableR_EventBox != null)
		{
			tableR_EventBox.InitVars();
		}
		tableR_Scholar = (R_ScholarDataTable)base.Tables["R_Scholar"];
		if (initTable && tableR_Scholar != null)
		{
			tableR_Scholar.InitVars();
		}
		tableR_Resource = (R_ResourceDataTable)base.Tables["R_Resource"];
		if (initTable && tableR_Resource != null)
		{
			tableR_Resource.InitVars();
		}
		tableR_Spell = (R_SpellDataTable)base.Tables["R_Spell"];
		if (initTable && tableR_Spell != null)
		{
			tableR_Spell.InitVars();
		}
		tableR_Chest = (R_ChestDataTable)base.Tables["R_Chest"];
		if (initTable && tableR_Chest != null)
		{
			tableR_Chest.InitVars();
		}
		tableR_Skill = (R_SkillDataTable)base.Tables["R_Skill"];
		if (initTable && tableR_Skill != null)
		{
			tableR_Skill.InitVars();
		}
		tableR_Camp = (R_CampDataTable)base.Tables["R_Camp"];
		if (initTable && tableR_Camp != null)
		{
			tableR_Camp.InitVars();
		}
		tableR_Market = (R_MarketDataTable)base.Tables["R_Market"];
		if (initTable && tableR_Market != null)
		{
			tableR_Market.InitVars();
		}
		tableR_Garrison = (R_GarrisonDataTable)base.Tables["R_Garrison"];
		if (initTable && tableR_Garrison != null)
		{
			tableR_Garrison.InitVars();
		}
		tableR_SeerHut = (R_SeerHutDataTable)base.Tables["R_SeerHut"];
		if (initTable && tableR_SeerHut != null)
		{
			tableR_SeerHut.InitVars();
		}
		tableR_Prison = (R_PrisonDataTable)base.Tables["R_Prison"];
		if (initTable && tableR_Prison != null)
		{
			tableR_Prison.InitVars();
		}
		tableR_Object = (R_ObjectDataTable)base.Tables["R_Object"];
		if (initTable && tableR_Object != null)
		{
			tableR_Object.InitVars();
		}
		tableR_PassGuard = (R_PassGuardDataTable)base.Tables["R_PassGuard"];
		if (initTable && tableR_PassGuard != null)
		{
			tableR_PassGuard.InitVars();
		}
		tableR_Heroes = (R_HeroesDataTable)base.Tables["R_Heroes"];
		if (initTable && tableR_Heroes != null)
		{
			tableR_Heroes.InitVars();
		}
		tableR_Town = (R_TownDataTable)base.Tables["R_Town"];
		if (initTable && tableR_Town != null)
		{
			tableR_Town.InitVars();
		}
		tableR_AllArts = (R_AllArtsDataTable)base.Tables["R_AllArts"];
		if (initTable && tableR_AllArts != null)
		{
			tableR_AllArts.InitVars();
		}
		tableR_Topology = (R_TopologyDataTable)base.Tables["R_Topology"];
		if (initTable && tableR_Topology != null)
		{
			tableR_Topology.InitVars();
		}
		tableR_AllSpell = (R_AllSpellDataTable)base.Tables["R_AllSpell"];
		if (initTable && tableR_AllSpell != null)
		{
			tableR_AllSpell.InitVars();
		}
		tableR_AllTimer = (R_AllTimerDataTable)base.Tables["R_AllTimer"];
		if (initTable && tableR_AllTimer != null)
		{
			tableR_AllTimer.InitVars();
		}
		tableR_AllSkill = (R_AllSkillDataTable)base.Tables["R_AllSkill"];
		if (initTable && tableR_AllSkill != null)
		{
			tableR_AllSkill.InitVars();
		}
		tableR_AllExperience = (R_AllExperienceDataTable)base.Tables["R_AllExperience"];
		if (initTable && tableR_AllExperience != null)
		{
			tableR_AllExperience.InitVars();
		}
		tableR_Mine = (R_MineDataTable)base.Tables["R_Mine"];
		if (initTable && tableR_Mine != null)
		{
			tableR_Mine.InitVars();
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private void InitClass()
	{
		base.DataSetName = "DataSet2";
		base.Prefix = "";
		base.Namespace = "http://tempuri.org/DataSet2.xsd";
		base.EnforceConstraints = true;
		SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
		tableR_Monstr = new R_MonstrDataTable();
		base.Tables.Add(tableR_Monstr);
		tableR_Art = new R_ArtDataTable();
		base.Tables.Add(tableR_Art);
		tableR_Bank = new R_BankDataTable();
		base.Tables.Add(tableR_Bank);
		tableR_EventBox = new R_EventBoxDataTable();
		base.Tables.Add(tableR_EventBox);
		tableR_Scholar = new R_ScholarDataTable();
		base.Tables.Add(tableR_Scholar);
		tableR_Resource = new R_ResourceDataTable();
		base.Tables.Add(tableR_Resource);
		tableR_Spell = new R_SpellDataTable();
		base.Tables.Add(tableR_Spell);
		tableR_Chest = new R_ChestDataTable();
		base.Tables.Add(tableR_Chest);
		tableR_Skill = new R_SkillDataTable();
		base.Tables.Add(tableR_Skill);
		tableR_Camp = new R_CampDataTable();
		base.Tables.Add(tableR_Camp);
		tableR_Market = new R_MarketDataTable();
		base.Tables.Add(tableR_Market);
		tableR_Garrison = new R_GarrisonDataTable();
		base.Tables.Add(tableR_Garrison);
		tableR_SeerHut = new R_SeerHutDataTable();
		base.Tables.Add(tableR_SeerHut);
		tableR_Prison = new R_PrisonDataTable();
		base.Tables.Add(tableR_Prison);
		tableR_Object = new R_ObjectDataTable();
		base.Tables.Add(tableR_Object);
		tableR_PassGuard = new R_PassGuardDataTable();
		base.Tables.Add(tableR_PassGuard);
		tableR_Heroes = new R_HeroesDataTable();
		base.Tables.Add(tableR_Heroes);
		tableR_Town = new R_TownDataTable();
		base.Tables.Add(tableR_Town);
		tableR_AllArts = new R_AllArtsDataTable();
		base.Tables.Add(tableR_AllArts);
		tableR_Topology = new R_TopologyDataTable();
		base.Tables.Add(tableR_Topology);
		tableR_AllSpell = new R_AllSpellDataTable();
		base.Tables.Add(tableR_AllSpell);
		tableR_AllTimer = new R_AllTimerDataTable();
		base.Tables.Add(tableR_AllTimer);
		tableR_AllSkill = new R_AllSkillDataTable();
		base.Tables.Add(tableR_AllSkill);
		tableR_AllExperience = new R_AllExperienceDataTable();
		base.Tables.Add(tableR_AllExperience);
		tableR_Mine = new R_MineDataTable();
		base.Tables.Add(tableR_Mine);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Monstr()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Art()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Bank()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_EventBox()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Scholar()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Resource()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Spell()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Chest()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Skill()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Camp()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Market()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Garrison()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_SeerHut()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Prison()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Object()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_PassGuard()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Heroes()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_Town()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_AllArts()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Topology()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_AllSpell()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private bool ShouldSerializeR_AllTimer()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_AllSkill()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_AllExperience()
	{
		return false;
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	private bool ShouldSerializeR_Mine()
	{
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	private void SchemaChanged(object sender, CollectionChangeEventArgs e)
	{
		if (e.Action == CollectionChangeAction.Remove)
		{
			InitVars();
		}
	}

	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
	[DebuggerNonUserCode]
	public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
	{
		DataSet2 dataSet = new DataSet2();
		XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
		XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
		XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
		xmlSchemaAny.Namespace = dataSet.Namespace;
		xmlSchemaSequence.Items.Add(xmlSchemaAny);
		xmlSchemaComplexType.Particle = xmlSchemaSequence;
		XmlSchema schemaSerializable = dataSet.GetSchemaSerializable();
		if (xs.Contains(schemaSerializable.TargetNamespace))
		{
			MemoryStream memoryStream = new MemoryStream();
			MemoryStream memoryStream2 = new MemoryStream();
			try
			{
				XmlSchema xmlSchema = null;
				schemaSerializable.Write(memoryStream);
				IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
				while (enumerator.MoveNext())
				{
					xmlSchema = (XmlSchema)enumerator.Current;
					memoryStream2.SetLength(0L);
					xmlSchema.Write(memoryStream2);
					if (memoryStream.Length == memoryStream2.Length)
					{
						memoryStream.Position = 0L;
						memoryStream2.Position = 0L;
						while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
						{
						}
						if (memoryStream.Position == memoryStream.Length)
						{
							return xmlSchemaComplexType;
						}
					}
				}
			}
			finally
			{
				memoryStream?.Close();
				memoryStream2?.Close();
			}
		}
		xs.Add(schemaSerializable);
		return xmlSchemaComplexType;
	}
}
