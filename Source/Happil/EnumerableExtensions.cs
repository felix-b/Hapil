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
	}
}
