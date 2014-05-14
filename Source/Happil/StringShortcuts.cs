using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Operands;
using Happil.Statements;

namespace Happil
{
	public static class StringShortcuts
	{
		public static Operand<string> FuncToString<T>(this IOperand<T> obj)
		{
			var @operator = new UnaryOperators.OperatorCall<object>(s_ObjectToString);
			return new UnaryExpressionOperand<object, string>(@operator, obj.CastTo<object>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<char> CharAt(this IOperand<string> s, IOperand<int> index)
		{
			return new Property<char>(s, s_Item, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> Compare(this IOperand<string> strA, IOperand<string> strB, bool ignoreCase = false)
		{
			var comparisonValue = (ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.CurrentCulture);
			var comparisonOperand = new Constant<StringComparison>(comparisonValue);
			var @operator = new UnaryOperators.OperatorCall<string>(s_Compare, strA, strB, comparisonOperand);
			return new UnaryExpressionOperand<string, int>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Concat(this IOperand<string> strA, IOperand<string> strB)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Concat, strA, strB);
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Concat(this IOperand<string> str, params IOperand<string>[] values)
		{
			var writer = StatementScope.Current.Writer;
			var newArray = writer.Local(writer.NewArray<string>(new Constant<int>(values.Length + 1)));
			newArray.ElementAt(0).Assign(str);

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				newArray.ElementAt(i + 1).Assign(values[i]);
			}

			var @operator = new UnaryOperators.OperatorCall<string>(s_ConcatArray, newArray);
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Copy(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Copy, str);
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void CopyTo(
			this IOperand<string> str,
			IOperand<int> sourceIndex,
			IOperand<char[]> destination,
			IOperand<int> destinationIndex,
			IOperand<int> count)
		{
			StatementScope.Current.AddStatement(new CallStatement(str, s_CopyTo, sourceIndex, destination, destinationIndex, count));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<bool> EndsWith(this IOperand<string> str, IOperand<string> value, bool ignoreCase = false)
		{
			var comparisonValue = (ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.CurrentCulture);
			var comparisonOperand = new Constant<StringComparison>(comparisonValue);
			var @operator = new UnaryOperators.OperatorCall<string>(s_EndsWith, value, comparisonOperand);
			return new UnaryExpressionOperand<string, bool>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<bool> StringEquals(
			this IOperand<string> str,
			IOperand<string> value,
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_EqualsWithComparisonType,
				value, new Constant<StringComparison>(comparisonType));

			return new UnaryExpressionOperand<string, bool>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Format(
			this IOperand<string> format,
			params IOperand<object>[] args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Format, format, Helpers.BuildArrayLocal(values: args));
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Format(
			this IOperand<string> format,
			IOperand<object[]> args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Format, format, args);
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Format(
			this IOperand<string> format,
			IOperand<IFormatProvider> provider,
			params IOperand<object>[] args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_FormatWithProvider, provider, format, Helpers.BuildArrayLocal(values: args));
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Format(
			this IOperand<string> format,
			IOperand<IFormatProvider> provider,
			IOperand<object[]> args)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_FormatWithProvider, provider, format, args);
			return new UnaryExpressionOperand<string, string>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> StringGetHashCode(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_GetHashCode);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> IndexOf(this IOperand<string> str, IOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOf, value);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> IndexOf(
			this IOperand<string> str,
			IOperand<string> value,
			IOperand<int> startIndex,
			IOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfWithStartIndexAndCount, value, startIndex, count);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> IndexOfAny(this IOperand<string> str, IOperand<char[]> anyOf)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfAny, anyOf);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> IndexOfAny(
			this IOperand<string> str,
			IOperand<char[]> anyOf,
			IOperand<int> startIndex,
			IOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_IndexOfAnyWithStartIndexAndCount, anyOf, startIndex, count);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<string> Insert(this IOperand<string> str, IOperand<int> startIndex, IOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Insert, startIndex, value);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> LastIndexOf(this IOperand<string> str, IOperand<string> value)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOf, value);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> LastIndexOf(
			this IOperand<string> str,
			IOperand<string> value,
			IOperand<int> startIndex,
			IOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfWithStartIndexAndCount, value, startIndex, count);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> LastIndexOfAny(this IOperand<string> str, IOperand<char[]> anyOf)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfAny, anyOf);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> LastIndexOfAny(
			this IOperand<string> str,
			IOperand<char[]> anyOf,
			IOperand<int> startIndex,
			IOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_LastIndexOfAnyWithStartIndexAndCount, anyOf, startIndex, count);
			return new UnaryExpressionOperand<string, int>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<int> Length(this IOperand<string> str)
		{
			return new Property<int>(str, s_Length);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> PadLeft(this IOperand<string> str, IOperand<int> totalWidth)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_PadLeft, totalWidth);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> PadRight(this IOperand<string> str, IOperand<int> totalWidth)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_PadRight, totalWidth);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Remove(this IOperand<string> str, IOperand<int> index)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Remove, index);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Remove(this IOperand<string> str, IOperand<int> index, IOperand<int> count)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_RemoveWithCount, index, count);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Replace(this IOperand<string> str, IOperand<char> oldChar, IOperand<char> newChar)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ReplaceWithChars, oldChar, newChar);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Replace(this IOperand<string> str, IOperand<string> oldValue, IOperand<string> newValue)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Replace, oldValue, newValue);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string[]> Split(this IOperand<string> str, params char[] separator)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SplitWithCharArray, Helpers.BuildArrayLocal(separator));
			return new UnaryExpressionOperand<string, string[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string[]> Split(
			this IOperand<string> str,
			IOperand<char[]> separator,
			IOperand<int> count,
			IOperand<StringSplitOptions> options)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SplitWithCharArrayAndCount, separator, count, options);
			return new UnaryExpressionOperand<string, string[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string[]> Split(this IOperand<string> str, params string[] separators)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_SplitWithStringArray,
				Helpers.BuildArrayLocal(separators),
				new Constant<StringSplitOptions>(StringSplitOptions.None));

			return new UnaryExpressionOperand<string, string[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string[]> Split(
			this IOperand<string> str,
			IOperand<string[]> separator,
			IOperand<int> count,
			IOperand<StringSplitOptions> options)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(
				s_SplitWithStringArrayAndCount,
				separator,
				count,
				options);

			return new UnaryExpressionOperand<string, string[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> StartsWith(this IOperand<string> str, IOperand<string> value, bool ignoreCase = false)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_StartsWith, value);
			return new UnaryExpressionOperand<string, bool>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Substring(this IOperand<string> str, IOperand<int> startIndex)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Substring, startIndex);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Substring(this IOperand<string> str, IOperand<int> startIndex, IOperand<int> length)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_SubstringWithLength, startIndex, length);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<char[]> ToCharArray(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToCharArray);
			return new UnaryExpressionOperand<string, char[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<char[]> ToCharArray(
			this IOperand<string> str,
			IOperand<int> startIndex,
			IOperand<int> length)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToCharArrayWithIndexAndLength, startIndex, length);
			return new UnaryExpressionOperand<string, char[]>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> ToLower(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToLower);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> ToUpper(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_ToUpper);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Trim(this IOperand<string> str)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_Trim);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Trim(this IOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, Helpers.BuildArrayLocal(trimChars));
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Trim(this IOperand<string> str, params IOperand<char>[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, Helpers.BuildArrayLocal(trimChars));
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> Trim(this IOperand<string> str, IOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimWithChars, trimChars);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> TrimEnd(this IOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimEnd, Helpers.BuildArrayLocal(trimChars));
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> TrimEnd(this IOperand<string> str, IOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimEnd, trimChars);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> TrimStart(this IOperand<string> str, params char[] trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimStart, Helpers.BuildArrayLocal(trimChars));
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<string> TrimStart(this IOperand<string> str, IOperand<char[]> trimChars)
		{
			var @operator = new UnaryOperators.OperatorCall<string>(s_TrimStart, trimChars);
			return new UnaryExpressionOperand<string, string>(@operator, str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly MethodInfo s_ObjectToString;
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
			s_ObjectToString = GetMethodInfo<Expression<Func<object, string>>>(obj => obj.ToString());
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
