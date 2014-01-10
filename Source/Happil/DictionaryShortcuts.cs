using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil
{
	public static class DictionaryShortcuts
	{
		public static void Add<TKey, TValue>(
			this IHappilOperand<IDictionary<TKey, TValue>> dictionary, 
			IHappilOperand<TKey> key, 
			IHappilOperand<TValue> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Clear<TKey, TValue>(this IHappilOperand<IDictionary<TKey, TValue>> dictionary)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> ContainsKey<TKey, TValue>(
			this IHappilOperand<IDictionary<TKey, TValue>> dictionary, 
			IHappilOperand<TKey> key)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> ContainsValue<TKey, TValue>(
			this IHappilOperand<IDictionary<TKey, TValue>> dictionary,
			IHappilOperand<TValue> value)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Remove<TKey, TValue>(
			this IHappilOperand<IDictionary<TKey, TValue>> dictionary,
			IHappilOperand<TKey> key)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> TryGetValue<TKey, TValue>(
			this IHappilOperand<IDictionary<TKey, TValue>> dictionary,
			IHappilOperand<TKey> key,
			IHappilOperand<TValue> value) //TODO: support out parameters!
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<int> Count<TKey, TValue>(this IHappilOperand<IDictionary<TKey, TValue>> dictionary)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<ICollection<TKey>> Keys<TKey, TValue>(this IHappilOperand<IDictionary<TKey, TValue>> dictionary)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<ICollection<TValue>> Values<TKey, TValue>(this IHappilOperand<IDictionary<TKey, TValue>> dictionary)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<TValue> Item<TKey, TValue>(this IHappilOperand<IDictionary<TKey, TValue>> dictionary, IHappilOperand<TKey> key)
		{
			return new PropertyAccessOperand<TValue>(
				dictionary, 
				GetReflectionCache<TKey, TValue>().Item,
				key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, ReflectionCache> s_ReflectionCacheByKeyValueType =
			new ConcurrentDictionary<Tuple<Type,Type>, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache<TKey, TValue>()
		{
			var cacheKey = new Tuple<Type, Type>(typeof(TKey), typeof(TValue));
			return s_ReflectionCacheByKeyValueType.GetOrAdd(cacheKey, new ReflectionCache<TKey, TValue>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class ReflectionCache
		{
			public PropertyInfo Item { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<TKey, TValue> : ReflectionCache
		{
			public ReflectionCache()
			{
				var type = typeof(IDictionary<TKey, TValue>);
				base.Item = type.GetProperty("Item");
			}
		}
	}
}
