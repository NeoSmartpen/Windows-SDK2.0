using System;
using System.Globalization;

namespace Neosmartpen.Net.Support
{
	class FloatConverter
	{
		private static CultureInfo convertCulture = new CultureInfo("en-US");
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

	}
}
