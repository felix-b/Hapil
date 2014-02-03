using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil
{
	public static class StringShortcuts
	{
		public static IHappilOperand<char> CharAt(this IHappilOperand<string> s, IHappilOperand<int> index)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> Compare(this IHappilOperand<string> strA, IHappilOperand<string> strB, bool ignoreCase = false)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Concat(this IHappilOperand<string> strA, IHappilOperand<string> strB)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Concat(this IHappilOperand<string> strA, IHappilOperand<string[]> values)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Concat(this IHappilOperand<string> strA, params IHappilOperand<string>[] values)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Copy(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void CopyTo(
			this IHappilOperand<string> str, 
			IHappilOperand<int> sourceIndex, 
			IHappilOperand<char[]> destination, 
			IHappilOperand<int> destinationIndex, 
			IHappilOperand<int> count)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> EndsWith(this IHappilOperand<string> str, IHappilOperand<string> value, bool ignoreCase = false)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Equals(
			this IHappilOperand<string> str, 
			IHappilOperand<string> value, 
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Format(
			this IHappilOperand<string> format,
			params IHappilOperand<object>[] args)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Format(
			this IHappilOperand<string> format,
			IHappilOperand<IFormatProvider> provider,
			params IHappilOperand<object>[] args)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> GetHashCode(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(this IHappilOperand<string> str, IHappilOperand<string> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(
			this IHappilOperand<string> str, 
			IHappilOperand<string> value, 
			IHappilOperand<int> startIndex, 
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(this IHappilOperand<string> str, IHappilOperand<char> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(
			this IHappilOperand<string> str,
			IHappilOperand<char> value,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOfAny(this IHappilOperand<string> str, IHappilOperand<char[]> anyOf)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOfAny(
			this IHappilOperand<string> str,
			IHappilOperand<char[]> anyOf,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Insert(this IHappilOperand<string> str, IHappilOperand<int> startIndex, IHappilOperand<string> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(this IHappilOperand<string> str, IHappilOperand<string> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(
			this IHappilOperand<string> str,
			IHappilOperand<string> value,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(this IHappilOperand<string> str, IHappilOperand<char> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(
			this IHappilOperand<string> str,
			IHappilOperand<char> value,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOfAny(this IHappilOperand<string> str, IHappilOperand<char[]> anyOf)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOfAny(
			this IHappilOperand<string> str,
			IHappilOperand<char[]> anyOf,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<int> Length(this IHappilOperand<string> str)
		{
			return new PropertyAccessOperand<int>(str, s_Length);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> PadLeft(this IHappilOperand<string> str, IHappilOperand<int> totalWidth, IHappilOperand<char> paddingChar = null)
		{
			throw new NotImplementedException();	
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> PadRight(this IHappilOperand<string> str, IHappilOperand<int> totalWidth, IHappilOperand<char> paddingChar = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Remove(this IHappilOperand<string> str, IHappilOperand<int> index, IHappilOperand<int> count = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Replace(this IHappilOperand<string> str, IHappilOperand<char> oldChar, IHappilOperand<char> newChar)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Replace(this IHappilOperand<string> str, IHappilOperand<string> oldValue, IHappilOperand<string> newValue)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(this IHappilOperand<string> str, params char[] separator)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(
			this IHappilOperand<string> str, 
			IHappilOperand<char[]> separator, 
			IHappilOperand<int> count = null,
			IHappilOperand<StringSplitOptions> options = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(this IHappilOperand<string> str, params string[] separator)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(
			this IHappilOperand<string> str,
			IHappilOperand<string[]> separator,
			IHappilOperand<int> count = null,
			IHappilOperand<StringSplitOptions> options = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> StartsWith(this IHappilOperand<string> str, IHappilOperand<string> value, bool ignoreCase = false)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Substring(this IHappilOperand<string> str, IHappilOperand<int> startIndex, IHappilOperand<int> length = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<char[]> ToCharArray(
			this IHappilOperand<string> str, 
			IHappilOperand<int> startIndex = null, 
			IHappilOperand<int> length = null)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToLower(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToLowerInvariant(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToUpper(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToUpperInvariant(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, params char[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, params IHappilOperand<char>[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimEnd(this IHappilOperand<string> str, params char[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimEnd(this IHappilOperand<string> str, params IHappilOperand<char>[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimEnd(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimStart(this IHappilOperand<string> str, params char[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimStart(this IHappilOperand<string> str, params IHappilOperand<char>[] trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimStart(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly PropertyInfo s_Length;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static StringShortcuts()
		{
			var type = typeof(string);

			s_Length = type.GetProperty("Length");
		}
	}
}
