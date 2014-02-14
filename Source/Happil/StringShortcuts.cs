using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;
using Happil.Statements;

namespace Happil
{
	public static class StringShortcuts
	{
		public static IHappilOperand<char> CharAt(this IHappilOperand<string> s, IHappilOperand<int> index)
		{
			return new PropertyAccessOperand<char>(s, s_Item, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> Compare(this IHappilOperand<string> strA, IHappilOperand<string> strB, bool ignoreCase = false)
		{
			var comparisonValue = (ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.CurrentCulture);
			var comparisonOperand = new HappilConstant<StringComparison>(comparisonValue);
			var @operator = new UnaryOperators.OperatorCall<string>(s_Compare, strA, strB, comparisonOperand);
			return new HappilUnaryExpression<string, int>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Concat(this IHappilOperand<string> strA, IHappilOperand<string> strB)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Concat, strA, strB);
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Concat(this IHappilOperand<string> str, params IHappilOperand<string>[] values)
		{
			var method = StatementScope.Current.OwnerMethod;
			var newArray = method.Local(method.NewArray<string>(new HappilConstant<int>(values.Length + 1)));
			newArray.ElementAt(0).Assign(str);

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				newArray.ElementAt(i + 1).Assign(values[i]);
			}

			var @operator = new UnaryOperators.OperatorCall<string>(s_ConcatArray, newArray);
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Copy(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Copy, str);
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void CopyTo(
			this IHappilOperand<string> str, 
			IHappilOperand<int> sourceIndex, 
			IHappilOperand<char[]> destination, 
			IHappilOperand<int> destinationIndex, 
			IHappilOperand<int> count)
		{
			StatementScope.Current.AddStatement(new CallStatement(str, s_CopyTo, sourceIndex, destination, destinationIndex, count));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> EndsWith(this IHappilOperand<string> str, IHappilOperand<string> value, bool ignoreCase = false)
		{
			var comparisonValue = (ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.CurrentCulture);
			var comparisonOperand = new HappilConstant<StringComparison>(comparisonValue);
			var @operator = new UnaryOperators.OperatorCall<string>(s_EndsWith, value, comparisonOperand);
			return new HappilUnaryExpression<string, bool>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> StringEquals(
			this IHappilOperand<string> str, 
			IHappilOperand<string> value, 
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_EqualsWithComparisonType, 
				value, new HappilConstant<StringComparison>(comparisonType));

			return new HappilUnaryExpression<string, bool>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Format(
			this IHappilOperand<string> format,
			params IHappilOperand<object>[] args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Format, format, Helpers.BuildArrayLocal(values: args));
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Format(
			this IHappilOperand<string> format,
			IHappilOperand<object[]> args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Format, format, args);
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Format(
			this IHappilOperand<string> format,
			IHappilOperand<IFormatProvider> provider,
			params IHappilOperand<object>[] args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_FormatWithProvider, provider, format, Helpers.BuildArrayLocal(values: args));
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Format(
			this IHappilOperand<string> format,
			IHappilOperand<IFormatProvider> provider,
			IHappilOperand<object[]> args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_FormatWithProvider, provider, format, args);
			return new HappilUnaryExpression<string, string>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> StringGetHashCode(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_GetHashCode);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(this IHappilOperand<string> str, IHappilOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOf, value);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOf(
			this IHappilOperand<string> str, 
			IHappilOperand<string> value, 
			IHappilOperand<int> startIndex, 
			IHappilOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfWithStartIndexAndCount, value, startIndex, count);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOfAny(this IHappilOperand<string> str, IHappilOperand<char[]> anyOf)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfAny, anyOf);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> IndexOfAny(
			this IHappilOperand<string> str,
			IHappilOperand<char[]> anyOf,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfAnyWithStartIndexAndCount, anyOf, startIndex, count);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<string> Insert(this IHappilOperand<string> str, IHappilOperand<int> startIndex, IHappilOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Insert, startIndex, value);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(this IHappilOperand<string> str, IHappilOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOf, value);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOf(
			this IHappilOperand<string> str,
			IHappilOperand<string> value,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfWithStartIndexAndCount, value, startIndex, count);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOfAny(this IHappilOperand<string> str, IHappilOperand<char[]> anyOf)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfAny, anyOf);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> LastIndexOfAny(
			this IHappilOperand<string> str,
			IHappilOperand<char[]> anyOf,
			IHappilOperand<int> startIndex,
			IHappilOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfAnyWithStartIndexAndCount, anyOf, startIndex, count);
			return new HappilUnaryExpression<string, int>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<int> Length(this IHappilOperand<string> str)
		{
			return new PropertyAccessOperand<int>(str, s_Length);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> PadLeft(this IHappilOperand<string> str, IHappilOperand<int> totalWidth)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_PadLeft, totalWidth);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> PadRight(this IHappilOperand<string> str, IHappilOperand<int> totalWidth)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_PadRight, totalWidth);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Remove(this IHappilOperand<string> str, IHappilOperand<int> index)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Remove, index);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Remove(this IHappilOperand<string> str, IHappilOperand<int> index, IHappilOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_RemoveWithCount, index, count);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Replace(this IHappilOperand<string> str, IHappilOperand<char> oldChar, IHappilOperand<char> newChar)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ReplaceWithChars, oldChar, newChar);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Replace(this IHappilOperand<string> str, IHappilOperand<string> oldValue, IHappilOperand<string> newValue)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Replace, oldValue, newValue);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(this IHappilOperand<string> str, params char[] separator)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SplitWithCharArray, Helpers.BuildArrayLocal(separator));
			return new HappilUnaryExpression<string, string[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(
			this IHappilOperand<string> str, 
			IHappilOperand<char[]> separator, 
			IHappilOperand<int> count,
			IHappilOperand<StringSplitOptions> options)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SplitWithCharArrayAndCount, separator, count, options);
			return new HappilUnaryExpression<string, string[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(this IHappilOperand<string> str, params string[] separators)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_SplitWithStringArray, 
				Helpers.BuildArrayLocal(separators), 
				new HappilConstant<StringSplitOptions>(StringSplitOptions.None));
			
			return new HappilUnaryExpression<string, string[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string[]> Split(
			this IHappilOperand<string> str,
			IHappilOperand<string[]> separator,
			IHappilOperand<int> count,
			IHappilOperand<StringSplitOptions> options)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_SplitWithStringArrayAndCount,
				separator,
				count,
				options);

			return new HappilUnaryExpression<string, string[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> StartsWith(this IHappilOperand<string> str, IHappilOperand<string> value, bool ignoreCase = false)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_StartsWith, value);
			return new HappilUnaryExpression<string, bool>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Substring(this IHappilOperand<string> str, IHappilOperand<int> startIndex)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Substring, startIndex);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Substring(this IHappilOperand<string> str, IHappilOperand<int> startIndex, IHappilOperand<int> length)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SubstringWithLength, startIndex, length);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<char[]> ToCharArray(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToCharArray);
			return new HappilUnaryExpression<string, char[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<char[]> ToCharArray(
			this IHappilOperand<string> str, 
			IHappilOperand<int> startIndex, 
			IHappilOperand<int> length)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToCharArrayWithIndexAndLength, startIndex, length);
			return new HappilUnaryExpression<string, char[]>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToLower(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToLower);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> ToUpper(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToUpper);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Trim);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, Helpers.BuildArrayLocal(trimChars));
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, params IHappilOperand<char>[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, Helpers.BuildArrayLocal(trimChars));
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> Trim(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, trimChars);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimEnd(this IHappilOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimEnd, Helpers.BuildArrayLocal(trimChars));
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimEnd(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimEnd, trimChars);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimStart(this IHappilOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimStart, Helpers.BuildArrayLocal(trimChars));
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<string> TrimStart(this IHappilOperand<string> str, IHappilOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimStart, trimChars);
			return new HappilUnaryExpression<string, string>(null, @operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly PropertyInfo s_Item;
		private static readonly MethodInfo s_Compare;
		private static readonly MethodInfo s_Concat;
		private static readonly MethodInfo s_ConcatArray;
		private static readonly MethodInfo s_Copy;
		private static readonly MethodInfo s_CopyTo;
		private static readonly MethodInfo s_EndsWith;
		private static readonly MethodInfo s_EqualsWithComparisonType;
		private static readonly MethodInfo s_Format;
		private static readonly MethodInfo s_FormatWithProvider;
		private static readonly MethodInfo s_GetHashCode;
		private static readonly MethodInfo s_IndexOf;
		private static readonly MethodInfo s_IndexOfWithStartIndexAndCount;
		private static readonly MethodInfo s_IndexOfAny;
		private static readonly MethodInfo s_IndexOfAnyWithStartIndexAndCount;
		private static readonly MethodInfo s_Insert;
		private static readonly MethodInfo s_LastIndexOf;
		private static readonly MethodInfo s_LastIndexOfWithStartIndexAndCount;
		private static readonly MethodInfo s_LastIndexOfAny;
		private static readonly MethodInfo s_LastIndexOfAnyWithStartIndexAndCount;
		private static readonly PropertyInfo s_Length;
		private static readonly MethodInfo s_PadLeft;
		private static readonly MethodInfo s_PadRight;
		private static readonly MethodInfo s_Remove;
		private static readonly MethodInfo s_RemoveWithCount;
		private static readonly MethodInfo s_Replace;
		private static readonly MethodInfo s_ReplaceWithChars;
		private static readonly MethodInfo s_SplitWithCharArray;
		private static readonly MethodInfo s_SplitWithCharArrayAndCount;
		private static readonly MethodInfo s_SplitWithStringArray;
		private static readonly MethodInfo s_SplitWithStringArrayAndCount;
		private static readonly MethodInfo s_StartsWith;
		private static readonly MethodInfo s_Substring;
		private static readonly MethodInfo s_SubstringWithLength;
		private static readonly MethodInfo s_ToCharArray;
		private static readonly MethodInfo s_ToCharArrayWithIndexAndLength;
		private static readonly MethodInfo s_ToLower;
		private static readonly MethodInfo s_ToLowerInvariant;
		private static readonly MethodInfo s_ToUpper;
		private static readonly MethodInfo s_ToUpperInvariant;
		private static readonly MethodInfo s_Trim;
		private static readonly MethodInfo s_TrimWithChars;
		private static readonly MethodInfo s_TrimEnd;
		private static readonly MethodInfo s_TrimStart;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static StringShortcuts()
		{
			s_Item = typeof(string).GetProperty("Chars");
			s_Compare = GetMethodInfo<Expression<Func<string, int>>>(s => string.Compare(s, "s", StringComparison.InvariantCultureIgnoreCase));
			s_Concat = GetMethodInfo<Expression<Func<string, string>>>(s => string.Concat(s, s));
			s_ConcatArray = GetMethodInfo<Expression<Func<string, string>>>(s => string.Concat(new string[0]));
			s_Copy = GetMethodInfo<Expression<Func<string, string>>>(s => string.Copy(s));
			s_CopyTo = GetMethodInfo<Expression<Action<string>>>(s => s.CopyTo(0, new char[0], 0, 0));
			s_EndsWith = GetMethodInfo<Expression<Func<string, bool>>>(s => s.EndsWith("s", StringComparison.Ordinal));
			s_EqualsWithComparisonType = GetMethodInfo<Expression<Func<string, bool>>>(s => s.Equals("s", StringComparison.Ordinal));
			s_Format = GetMethodInfo<Expression<Func<string, string>>>(s => string.Format("s", new object[0]));
			s_FormatWithProvider = GetMethodInfo<Expression<Func<string, string>>>(s => string.Format(CultureInfo.CurrentCulture, s, new object[0]));
			s_GetHashCode = GetMethodInfo<Expression<Func<string, int>>>(s => s.GetHashCode());
			s_IndexOf = GetMethodInfo<Expression<Func<string, int>>>(s => s.IndexOf("s"));
			s_IndexOfWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, int>>>(s => s.IndexOf("s", 1, 2));
			s_IndexOfAny = GetMethodInfo<Expression<Func<string, int>>>(s => s.IndexOfAny(new char[0]));
			s_IndexOfAnyWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, int>>>(s => s.IndexOfAny(new char[0], 1, 2));
			s_Insert = GetMethodInfo<Expression<Func<string, string>>>(s => s.Insert(1, "s"));
			s_LastIndexOf = GetMethodInfo<Expression<Func<string, int>>>(s => s.LastIndexOf("s"));
			s_LastIndexOfWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, int>>>(s => s.LastIndexOf("s", 1, 2));
			s_LastIndexOfAny = GetMethodInfo<Expression<Func<string, int>>>(s => s.LastIndexOfAny(new char[0]));
			s_LastIndexOfAnyWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, int>>>(s => s.LastIndexOfAny(new char[0], 1, 2));
			s_Length = GetPropertyInfo<Expression<Func<string, int>>>(s => s.Length);
			s_PadLeft = GetMethodInfo<Expression<Func<string, string>>>(s => s.PadLeft(0));
			s_PadRight = GetMethodInfo<Expression<Func<string, string>>>(s => s.PadRight(0));
			s_Remove = GetMethodInfo<Expression<Func<string, string>>>(s => s.Remove(0));
			s_RemoveWithCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.Remove(0, 1));
			s_Replace = GetMethodInfo<Expression<Func<string, string>>>(s => s.Replace("s1", "s2"));
			s_ReplaceWithChars = GetMethodInfo<Expression<Func<string, string>>>(s => s.Replace('a', 'b'));
			s_SplitWithCharArray = GetMethodInfo<Expression<Func<string, string[]>>>(s => s.Split(new char[0]));
			s_SplitWithCharArrayAndCount = GetMethodInfo<Expression<Func<string, string[]>>>(s => s.Split(new char[0], 1, StringSplitOptions.None));
			s_SplitWithStringArray = GetMethodInfo<Expression<Func<string, string[]>>>(s => s.Split(new[] { "s" }, StringSplitOptions.None));
			s_SplitWithStringArrayAndCount = GetMethodInfo<Expression<Func<string, string[]>>>(s => s.Split(new[] { "s" }, 1, StringSplitOptions.None));
			s_StartsWith = GetMethodInfo<Expression<Func<string, bool>>>(s => s.StartsWith("s"));
			s_Substring = GetMethodInfo<Expression<Func<string, string>>>(s => s.Substring(1));
			s_SubstringWithLength = GetMethodInfo<Expression<Func<string, string>>>(s => s.Substring(1, 2));
			s_ToCharArray = GetMethodInfo<Expression<Func<string, char[]>>>(s => s.ToCharArray());
			s_ToCharArrayWithIndexAndLength = GetMethodInfo<Expression<Func<string, char[]>>>(s => s.ToCharArray(1, 2));
			s_ToLower = GetMethodInfo<Expression<Func<string, string>>>(s => s.ToLower());
			s_ToLowerInvariant = GetMethodInfo<Expression<Func<string, string>>>(s => s.ToLowerInvariant());
			s_ToUpper = GetMethodInfo<Expression<Func<string, string>>>(s => s.ToUpper());
			s_ToUpperInvariant = GetMethodInfo<Expression<Func<string, string>>>(s => s.ToUpperInvariant());
			s_Trim = GetMethodInfo<Expression<Func<string, string>>>(s => s.Trim());
			s_TrimWithChars = GetMethodInfo<Expression<Func<string, string>>>(s => s.Trim(new char[0]));
			s_TrimEnd = GetMethodInfo<Expression<Func<string, string>>>(s => s.TrimEnd(new char[0]));
			s_TrimStart = GetMethodInfo<Expression<Func<string, string>>>(s => s.TrimStart(new char[0]));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo GetMethodInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
		{
			return ((MethodCallExpression)lambda.Body).Method;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static PropertyInfo GetPropertyInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
		{
			return (PropertyInfo)((MemberExpression)lambda.Body).Member;
		}
	}
}
