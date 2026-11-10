namespace ProspectorRT
{
	internal static class TownNameValidator
	{
		public static bool IsPrintableTownChar(byte b)
		{
			if (b <= 0x08) return false;
			if (b == 0x0B) return false;
			if (b == 0x0C) return false;
			if (b >= 0x0E && b <= 0x1F) return false;
			if (b == 0x7F) return false;
			return true;
		}
	}
}
