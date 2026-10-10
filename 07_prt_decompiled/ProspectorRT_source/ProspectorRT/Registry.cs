using Microsoft.Win32;

namespace ProspectorRT;

public static class Registry
{
	public static string GetRegistryValue(string Key, string Name)
	{
		RegistryKey registryKey;
		try
		{
			registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CommonSetting.m_SoftwareKey + "\\" + Key);
		}
		catch
		{
			return "";
		}
		if (registryKey == null)
		{
			return "";
		}
		object value = registryKey.GetValue(Name);
		registryKey.Close();
		if (value == null)
		{
			return "";
		}
		return value.ToString();
	}

	public static void SetRegistryValue(string Key, string Name, string Value)
	{
		RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CommonSetting.m_SoftwareKey + "\\" + Key, writable: true);
		if (registryKey == null)
		{
			CreateKey(Key);
			registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CommonSetting.m_SoftwareKey + "\\" + Key, writable: true);
		}
		registryKey.SetValue(Name, Value);
		registryKey.Close();
		registryKey = null;
	}

	public static void CreateKey(string Key)
	{
		Microsoft.Win32.Registry.CurrentUser.CreateSubKey(CommonSetting.m_SoftwareKey + "\\" + Key);
	}

	public static void DeleteKey(string Key)
	{
		try
		{
			Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(CommonSetting.m_SoftwareKey + "\\" + Key);
		}
		catch
		{
		}
	}

	public static bool RegistryKeyExist(string Key)
	{
		RegistryKey registryKey;
		try
		{
			registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CommonSetting.m_SoftwareKey + "\\" + Key);
		}
		catch
		{
			return false;
		}
		if (registryKey == null)
		{
			return false;
		}
		registryKey.Close();
		return true;
	}
}
