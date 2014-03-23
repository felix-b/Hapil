using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil
{
	public static class StringExtensions
	{
		public static string TrimPrefix(this string str, string prefix)
		{
			if ( str != null && prefix != null && str.StartsWith(prefix) )
			{
				return str.Substring(prefix.Length);
			}
			else
			{
				return str;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
	
		public static string TrimSuffix(this string str, string suffix)
		{
			if ( str != null && suffix != null && str.EndsWith(suffix) )
			{
				return str.Substring(0, str.Length - suffix.Length);
			}
			else
			{
				return str;
			}
		}
	}
}
