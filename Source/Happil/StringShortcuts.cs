using System;
using System.CodeDom;
using System.Collections.Generic;
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
			IHappilOperand<int> count = null,
			IHappilOperand<StringSplitOptions> options = null)
		{
			throw new NotImplementedException();
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

		private static readonly PropertyInfo s_Item;
		private static readonly MethodInfo s_Compare;
		private static readonly MethodInfo s_Concat;
		private static readonly MethodInfo s_ConcatArray;
		private static readonly MethodInfo s_Copy;
		//private static readonly MethodInfo s_CopyTo;
		//private static readonly MethodInfo s_EndsWith;
		//private static readonly MethodInfo s_EqualsWithComparisonType;
		//private static readonly MethodInfo s_Format;
		//private static readonly MethodInfo s_FormatWithProvider;
		//private static readonly MethodInfo s_IndexOf;
		//private static readonly MethodInfo s_IndexOfWithStartIndexAndCount;
		//private static readonly MethodInfo s_IndexOfAny;
		//private static readonly MethodInfo s_IndexOfAnyWithStartIndexAndCount;
		//private static readonly MethodInfo s_Insert;
		//private static readonly MethodInfo s_LastIndexOf;
		//private static readonly MethodInfo s_LastIndexOfWithStartIndexAndCount;
		//private static readonly MethodInfo s_LastIndexOfAny;
		//private static readonly MethodInfo s_LastIndexOfAnyWithStartIndexAndCount;
		private static readonly PropertyInfo s_Length;
		//private static readonly MethodInfo s_PadLeft;
		//private static readonly MethodInfo s_PadRight;
		//private static readonly MethodInfo s_Remove;
		//private static readonly MethodInfo s_Replace;
		//private static readonly MethodInfo s_ReplaceWithChars;
		//private static readonly MethodInfo s_SplitWithCharArray;
		//private static readonly MethodInfo s_SplitWithCharArrayAndCount;
		private static readonly MethodInfo s_SplitWithStringArray;
		//private static readonly MethodInfo s_SplitWithStringArrayAndCount;
		//private static readonly MethodInfo s_SplitWithCount;
		private static readonly MethodInfo s_StartsWith;
		private static readonly MethodInfo s_Substring;
		private static readonly MethodInfo s_SubstringWithLength;
		//private static readonly MethodInfo s_ToCharArray;
		//private static readonly MethodInfo s_ToLower;
		//private static readonly MethodInfo s_ToLowerInvariant;
		//private static readonly MethodInfo s_ToUpper;
		//private static readonly MethodInfo s_ToUpperInvariant;
		//private static readonly MethodInfo s_Trim;
		//private static readonly MethodInfo s_TrimWithChars;
		//private static readonly MethodInfo s_TrimEnd;
		//private static readonly MethodInfo s_TrimStart;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static StringShortcuts()
		{
			s_Item = typeof(string).GetProperty("Chars");
			s_Compare = GetMethodInfo<Expression<Func<string, int>>>(s => string.Compare(s, "s", StringComparison.InvariantCultureIgnoreCase));
			s_Concat = GetMethodInfo<Expression<Func<string, string>>>(s => string.Concat(s, s));
			s_ConcatArray = GetMethodInfo<Expression<Func<string, string>>>(s => string.Concat(new string[0]));
			s_Copy = GetMethodInfo<Expression<Func<string, string>>>(s => string.Copy(s));
			
			//s_CopyTo = GetMethodInfo<Expression<Action<string>>>(s => s.CopyTo(0, new char[0], 0, 0));
			//s_EndsWith = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_EqualsWithComparisonType = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_Format = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_FormatWithProvider = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_IndexOf = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_IndexOfWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_IndexOfAny = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_IndexOfAnyWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_Insert = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_LastIndexOf = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_LastIndexOfWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_LastIndexOfAny = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_LastIndexOfAnyWithStartIndexAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));

			s_Length = GetPropertyInfo<Expression<Func<string, int>>>(s => s.Length);

			//s_PadLeft = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_PadRight = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_Remove = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_Replace = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_ReplaceWithChars = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_SplitWithCharArray = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_SplitWithCharArrayAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));

			s_SplitWithStringArray = GetMethodInfo<Expression<Func<string, string[]>>>(s => s.Split(new[] { "s" }, StringSplitOptions.None));

			//s_SplitWithStringArrayAndCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_SplitWithCount = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));

			s_StartsWith = GetMethodInfo<Expression<Func<string, bool>>>(s => s.StartsWith("s"));
			s_Substring = GetMethodInfo<Expression<Func<string, string>>>(s => s.Substring(1));
			s_SubstringWithLength = GetMethodInfo<Expression<Func<string, string>>>(s => s.Substring(1, 2));

			//s_ToCharArray = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_ToLower = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_ToLowerInvariant = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_ToUpper = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_ToUpperInvariant = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_Trim = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_TrimWithChars = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_TrimEnd = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
			//s_TrimStart = GetMethodInfo<Expression<Func<string, string>>>(s => s.StartsWith("s"));
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
