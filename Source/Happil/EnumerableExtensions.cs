using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> SelectIf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			if ( predicate != null )
			{
				return source.Where(predicate);
			}
			else
			{
				return source;
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> source, bool condition, Func<T> valueFactory)
		{
			if ( condition )
			{
				return source.Concat(new[] { valueFactory() });
			}
			else
			{
				return source;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> source, T valueOrNull) where T : class
		{
			if ( valueOrNull != null )
			{
				return source.Concat(new[] { valueOrNull });
			}
			else
			{
				return source;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> source, bool condition, T value) 
		{
			if ( condition )
			{
				return source.Concat(new[] { value });
			}
			else
			{
				return source;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach ( var item in source )
			{
				action(item);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			var index = 0;

			foreach ( var item in source )
			{
				action(item, index++);
			}
		}
	}
}
