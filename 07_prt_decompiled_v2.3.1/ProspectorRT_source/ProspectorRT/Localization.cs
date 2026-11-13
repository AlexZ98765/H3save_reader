using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProspectorRT
{
	public enum Lang { Ru, En }

	public static class Localization
	{
		private static Lang _current;
		private static bool _loaded;

		private static readonly Dictionary<string, string> En = new Dictionary<string, string>
		{
			{ "Главное меню", "Main Menu" },
			{ "        Файл       ", "        File       " },
			{ "Открыть", "Open" },
			{ "Очистить", "Clear" },
			{ "        Старт        ", "        Start        " },
			{ "        Стоп        ", "        Stop        " },
			{ "       Обновить       ", "       Refresh       " },
			{ "        Сервис        ", "        Service        " },
			{ "Экспорт в Excel", "Export to Excel" },
			{ "Оракул", "Oracle" },
			{ "Без запуска игры", "Without launching the game" },
			{ "Версия", "Version" },
			{ "Максимум уровней", "Max Levels" },
			{ "Настройка", "Settings" },
			{ "Кнопка  «Старт»", "«Start» Button" },
			{ "Последних открытых файлов", "Recent Files" },
			{ "Названия объектов", "Object Names" },
			{ "        Справка        ", "        Help        " },
			{ "Справка", "Help" },
			{ "О программе", "About" },
			{ "Артефакты", "Artifacts" },
			{ "Район", "Region" },
			{ "Объект", "Object" },
			{ "Слот", "Slot" },
			{ "Артефакт", "Artifact" },
			{ "Класс", "Class" },
			{ "Реликт-С", "Relic-C" },
			{ "Золото", "Gold" },
			{ "Ресурс", "Resource" },
			{ "Охрана", "Guard" },
			{ "Монстры", "Monsters" },
			{ "Монстр", "Monster" },
			{ "Количество", "Count" },
			{ "Настроение", "Mood" },
			{ "Уровень", "Level" },
			{ "Прирост", "Growth" },
			{ "Банки", "Banks" },
			{ "События и Ящики Пандоры", "Events and Pandora's Boxes" },
			{ "Опыт", "Experience" },
			{ "Мана", "Mana" },
			{ "Мораль", "Morale" },
			{ "Удача", "Luck" },
			{ "Первичный навык", "Primary Skill" },
			{ "Вторичный навык", "Secondary Skill" },
			{ "Заклинание", "Spell" },
			{ "Доступно", "Available" },
			{ "Повтор", "Repeat" },
			{ "Ученые", "Scholars" },
			{ "Ресурсы", "Resources" },
			{ "Сундуки", "Chests" },
			{ "Заклинания", "Spells" },
			{ "Навыки", "Skills" },
			{ "Навык", "Skill" },
			{ "Лагеря Беженцев", "Refugee Camps" },
			{ "Рынки", "Markets" },
			{ "Цена на Черном Рынке", "Black Market Price" },
			{ "Провидцы", "Seers" },
			{ "Задание", "Quest" },
			{ "Награда", "Reward" },
			{ "Срок", "Term" },
			{ "Стражи Прохода", "Border Guards" },
			{ "Гарнизоны", "Garrisons" },
			{ "Антимагический", "Anti-magic" },
			{ "Флаг", "Flag" },
			{ "Можно забрать", "Can Take" },
			{ "Тюрьмы", "Prisons" },
			{ "Герой", "Hero" },
			{ "Машина", "Machine" },
			{ "Книга заклинаний", "Spellbook" },
			{ "Объекты", "Objects" },
			{ "Плата", "Fee" },
			{ "Топология", "Topology" },
			{ "Тип", "Type" },
			{ "Цвет", "Color" },
			{ "Пара", "Pair" },
			{ "События-Таймеры", "Event Timers" },
			{ "День", "Day" },
			{ "Город", "Town" },
			{ "Местоположение", "Location" },
			{ "Постройки", "Buildings" },
			{ "Герои", "Heroes" },
			{ "Найм", "Hire" },
			{ "Идеология", "Alignment" },
			{ "Города", "Towns" },
			{ "Гарнизон", "Garrison" },
			{ "Построен", "Built" },
			{ "Библиотека", "Library" },
			{ "Доступен", "Available" },
			{ "Таймер", "Timer" },
			{ "Все Арты", "All Arts" },
			{ "Кукла", "Doll" },
			{ "Все Заклы", "All Spells" },
			{ "Все Навыки", "All Skills" },
			{ "ProspectorRT  -  HoMM 3  Стартовый сейв", "ProspectorRT  -  HoMM 3  Start Save" },
			{ "Фильтр с помечаемым списком", "Filter with Checked List" },
			{ "Удалить шаблон по умолчанию", "Remove Default Template" },
			{ "Создать шаблон по умолчанию", "Create Default Template" },
			{ "&ОК", "&OK" },
			{ "Автор:", "Author:" },
			{ "Сайт:", "Website:" },
			{ "Благодарности:", "Acknowledgements:" },
			{ "Обсуждение:", "Discussion:" },
			{ "Выберите таблицы для экспорта в Excel", "Select tables to export to Excel" },
			{ "Имя файла", "File name" },
			{ "Отмена", "Cancel" },
			{ "Очистить все", "Clear All" },
			{ "Выбрать все", "Select All" },
			{ "&Экспорт...", "&Export..." },
			{ "Сохранить", "Save" },
			{ "&Печать...", "&Print..." },
			{ "Па&раметры страницы...", "Pa&ge Setup..." },
			{ "Колонтитулы", "Header/Footer" },
			{ "Масштаб", "Zoom" },
			{ "Найти", "Find" },
			{ "Перемещать", "Move" },
			{ "Лупа", "Magnifier" },
			{ "Уменьшить", "Zoom Out" },
			{ "Изменить", "Edit" },
			{ "Увеличить", "Zoom In" },
			{ "Первая страница", "First Page" },
			{ "Предыдущая страница", "Previous Page" },
			{ "Следующая страница", "Next Page" },
			{ "Последняя страница", "Last Page" },
			{ "Несколько страниц", "Multiple Pages" },
			{ "&Подложка...", "&Background..." },
			{ "В&ыход", "E&xit" },
			{ "Строка статуса", "Status Bar" },
			{ "&Файл", "&File" },
			{ "&Вид", "&View" },
			{ "&Макет страницы", "&Page Layout" },
			{ "&Скрыть пробелы", "&Hide Spaces" },
			{ "&Показать пробелы", "&Show Spaces" },
			{ "Ф&он", "B&ackground" },
			{ "Просмотр", "Preview" },
			{ "Кодировка", "Encoding" },
			{ "Названия Объектов", "Object Names" },
			{ "Развернуть все", "Expand All" },
			{ "Свернуть все", "Collapse All" },
			{ "Язык", "Language" },
			{ "Не указан файл экспорта.", "No export file specified." },
			{ "Нет выбранных таблиц для экспорта.", "No tables selected for export." },
			{ "Библиотека LMOracle.SkillTreeAPI.dll не найдена.\r\nПостроение дерева прокачки невозможно.", "The LMOracle.SkillTreeAPI.dll library was not found.\r\nBuilding the skill tree is impossible." },
			{ "Ошибка загрузки LMOracle.SkillTreeAPI.dll", "LMOracle.SkillTreeAPI.dll load error" },
			{ "Процесс \"Heroes of Might and Magic 3\" не найден.\r\nЗапустите игру или выберите соответствующую версию.", "Process \"Heroes of Might and Magic 3\" not found.\r\nLaunch the game or select the appropriate version." },
			{ "Ошибка доступа к процессу", "Process access error" },
			{ "Выберите соответствующую версию или в \"Heroes of Might and Magic 3\" загрузите сейв \"", "Select the appropriate version or in \"Heroes of Might and Magic 3\" load the save \"" },
			{ "Ошибка построения дерева прокачки", "Skill tree build error" },
			{ "Не указан файл с названиями объектов", "No object names file specified" },
			{ "Начали", "Started" },
			{ "Стоп", "Stop" },
			{ "Старт", "Start" },
			{ "Готово", "Ready" },
			{ "Выберите файл", "Choose file" },
			{ "Файл", "File" },
			{ "Ошибка", "Error" },
			{ "не существует.\r\nУдалить из списка?", "does not exist.\r\nRemove from list?" },
			{ "ошибка открытия файла", "file open error" },
			{ "Недопустимая версия карты", "Invalid map version" },
			{ "Ошибка открытия", "Open error" },
			{ "не является шаблоном для этой Таблицы.", "is not a template for this Table." },
			{ "Экспорт  в  Excel  завершен.\r\nОткрыть  файл", "Export  to  Excel  complete.\r\nOpen  file" },
			{ " К сожалению, ошибка обработки сейва. \r\n Пожалуйста, вышлите его и карту на e-mail \r\n \r\n               prospectorrt@yandex.ru \r\n \r\n Будет возможность в ответном сообщении \r\n получить исправленный файл программы.", " Unfortunately, a save processing error occurred. \r\n Please send it and the map to e-mail \r\n \r\n               prospectorrt@yandex.ru \r\n \r\n You will have the opportunity in a reply \r\n to receive the fixed program file." },
			{ "ProspectorRT - Экспорт в Excel", "ProspectorRT - Export to Excel" },
			{ "Экспорт...", "Export..." },
			{ "Печать...", "Print..." },
			{ "Параметры страницы...", "Page Setup..." },
			{ "Подложка...", "Background..." },
			{ "Выход", "Exit" },
			{ "Из файла", "From file" },
			{ "По умолчанию", "By default" },
			{ "Дата - ", "Date - " },
			{ "Excel файлы|*.xlsx|Все файлы|*.*", "Excel files|*.xlsx|All files|*.*" },
			{ "с названиями объектов не существует.\r\nПроверьте пункт меню \"Сервис->Настройка->Названия объектов\"\r\nИспользованы значения по умолчанию.", "with object names does not exist.\r\nCheck the menu item \"Service->Settings->Object names\"\r\nDefault values were used." },
		};

		public static Lang Current
		{
			get
			{
				if (!_loaded)
				{
					_current = LoadFromRegistry();
					_loaded = true;
				}
				return _current;
			}
			set { _current = value; _loaded = true; }
		}

		public static string T(string ru)
		{
			if (ru == null || ru.Length == 0) return ru;
			if (Current == Lang.Ru) return ru;
			string en;
			return En.TryGetValue(ru, out en) ? en : ru;
		}

		public static void Save()
		{
			try
			{
				Registry.SetRegistryValue("Lang", "Lang", _current == Lang.En ? "en" : "ru");
			}
			catch { }
		}

		private static Lang LoadFromRegistry()
		{
			try
			{
				string v = Registry.GetRegistryValue("Lang", "Lang");
				if (v == "en") return Lang.En;
			}
			catch { }
			return Lang.Ru;
		}

		private static readonly string[] PropNames = new string[]
		{
			"Caption", "Text", "GroupPanelText", "HeaderText", "Title", "ToolTipTitle", "SuperTip"
		};

		public static void ApplyTo(object root)
		{
			if (root == null) return;
			try { ApplyObject(root); }
			catch { }
		}

		private static void ApplyObject(object obj)
		{
			if (obj == null) return;
			Type t = obj.GetType();
			// Translate own string properties.
			foreach (string pn in PropNames)
			{
				TryTranslateProperty(obj, t, pn);
			}
			// Walk child collections common to WinForms + DevExpress.
			WalkCollection(obj, t, "Controls");
			WalkCollection(obj, t, "Items");          // BarManager.Items, Menu items
			WalkCollection(obj, t, "Columns");        // GridView columns
			WalkCollection(obj, t, "TabPages");
			WalkCollection(obj, t, "MergedMenu");
			WalkArray(obj, t, "ViewCollection");     // GridControl views
			WalkArray(obj, t, "Views");
		}

		private static void TryTranslateProperty(object obj, Type t, string propName)
		{
			PropertyInfo p = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			if (p == null || !p.CanRead || !p.CanWrite) return;
			if (p.PropertyType != typeof(string)) return;
			try
			{
				string cur = (string)p.GetValue(obj, null);
				if (cur == null || cur.Length == 0) return;
				string tr = Translate(cur);
				if (tr != cur) p.SetValue(obj, tr, null);
			}
			catch { }
		}

		private static string Translate(string s)
		{
			if (Current == Lang.Ru) return s;
			string en;
			if (En.TryGetValue(s, out en)) return en;
			return s;
		}

		private static void WalkCollection(object obj, Type t, string propName)
		{
			PropertyInfo p = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			if (p == null) return;
			object col;
			try { col = p.GetValue(obj, null); } catch { return; }
			if (col == null) return;
			IEnumerable e = col as IEnumerable;
			if (e == null) return;
			var list = new List<object>();
			foreach (object o in e) list.Add(o);
			foreach (object o in list) ApplyObject(o);
		}

		private static void WalkArray(object obj, Type t, string propName)
		{
			PropertyInfo p = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			if (p == null) return;
			Array arr;
			try { arr = p.GetValue(obj, null) as Array; } catch { return; }
			if (arr == null) return;
			foreach (object o in arr) ApplyObject(o);
		}
	}
}
