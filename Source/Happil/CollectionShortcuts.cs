using System;
using System.Collections.Concurrent;
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

		public static HappilAssignable<T> ItemAt<T>(this IHappilOperand<IList<T>> collection, IHappilOperand<int> index)
		{
			return new PropertyAccessOperand<T>(collection, GetReflectionCache<T>().Item, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<T> ItemAt<T>(this IHappilOperand<IList<T>> collection, int index)
		{
			return ItemAt(collection, new HappilConstant<int>(index));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<T> ElementAt<T>(this IHappilOperand<T[]> array, IHappilOperand<int> index)
		{
			return new ArrayElementAccessOperand<T>(array, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<T> ElementAt<T>(this IHappilOperand<T[]> array, int index)
		{
			return ElementAt(array, new HappilConstant<int>(index));
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
			public PropertyInfo Item { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<T> : ReflectionCache
		{
			public ReflectionCache()
			{
				var collectionType = typeof(ICollection<T>);
				var listType = typeof(IList<T>);

				Add = Helpers.GetMethodInfo<Expression<Action<ICollection<T>>>>(x => x.Add(default(T)));
				Count = Helpers.GetPropertyInfo<Expression<Func<ICollection<T>, int>>>(x => x.Count);
				Item = listType.GetProperty("Item");
			}
		}
	}
}
