using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;
using Happil.Statements;

namespace Happil
{
	public static class CollectionShortcuts
	{
		public static HappilOperand<int> IndexOf<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Insert<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<int> index, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void RemoveAt<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<int> index)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<T> ItemAt<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<int> index)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Add<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<T> item)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().Add, item));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Clear<T>(this IHappilOperand<ICollection<T>> collection)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Contains<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void CopyTo<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<T[]> array, IHappilOperand<int> arrayIndex)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> Count<T>(this IHappilOperand<ICollection<T>> collection)
		{
			return new PropertyAccessOperand<int>(collection, GetReflectionCache<T>().Count);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsReadOnly<T>(this IHappilOperand<ICollection<T>> collection)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Remove<T>(this IHappilOperand<ICollection<T>> collection, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<IEnumerator<T>> GetEnumerator<T>(this IHappilOperand<ICollection<T>> collection)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<T[]> ToArray<T>(this IHappilOperand<ICollection<T>> collection)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> Length<T>(this IHappilOperand<T[]> array)
		{
			return new PropertyAccessOperand<int>(array, s_ArrayLength);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, ReflectionCache> s_ReflectionCacheByItemType = 
			new ConcurrentDictionary<Type, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly PropertyInfo s_ArrayLength = 
			typeof(Array).GetProperty("Length");

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache<T>()
		{
			return s_ReflectionCacheByItemType.GetOrAdd(typeof(T), new ReflectionCache<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class ReflectionCache
		{
			public MethodInfo Add { get; protected set; }
			public PropertyInfo Count { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<T> : ReflectionCache
		{
			public ReflectionCache()
			{
				var type = typeof(ICollection<T>);
				
				Add = type.GetMethod("Add");
				Count = type.GetProperty("Count");
			}
		}
	}
}
