using System;
using System.Globalization;

namespace Neosmartpen.Net.Support
{
	public class NConvert
	{
		private static CultureInfo convertCulture = new CultureInfo("en-US");

		public static int ToInt32(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return 0;
			return Convert.ToInt32(str);
		}
		public static long ToInt64(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return 0;
			return Convert.ToInt64(str);
		}
		public static double ToDouble(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return 0;
			return Convert.ToDouble(str, convertCulture);
		}

		public static float ToSingle(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return 0;
			return Convert.ToSingle(str, convertCulture);
		}

		public static bool ToBoolean(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			return Convert.ToBoolean(str);
		}
	}
}
