using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
			public MethodInfo Add { get; protected set; }
			public MethodInfo Clear { get; protected set; }
			public MethodInfo ContainsKey { get; protected set; }
			public MethodInfo ContainsValue { get; protected set; }
			public MethodInfo Remove { get; protected set; }
			public MethodInfo TryGetValue { get; protected set; }
			public PropertyInfo Item { get; protected set; }
			public PropertyInfo Count { get; protected set; }
			public PropertyInfo Keys { get; protected set; }
			public PropertyInfo Values { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<TKey, TValue> : ReflectionCache
		{
			public ReflectionCache()
			{
				Add = Helpers.GetMethodInfo<Expression<Action<IDictionary<TKey, TValue>>>>(x => x.Add(default(TKey), default(TValue)));
				Clear = Helpers.GetMethodInfo<Expression<Action<IDictionary<TKey, TValue>>>>(x => x.Clear());
				ContainsKey = Helpers.GetMethodInfo<Expression<Func<IDictionary<TKey, TValue>, bool>>>(x => x.ContainsKey(default(TKey)));
				ContainsValue = Helpers.GetMethodInfo<Expression<Func<Dictionary<TKey, TValue>, bool>>>(x => x.ContainsValue(default(TValue)));
				Remove = Helpers.GetMethodInfo<Expression<Func<Dictionary<TKey, TValue>, bool>>>(x => x.Remove(default(TKey)));
				TryGetValue = Helpers.GetMethodInfo<Expression<Func<Dictionary<TKey, TValue>, TValue, bool>>>((x, v) => x.TryGetValue(default(TKey), out v));

				Count = Helpers.GetPropertyInfo<Expression<Func<Dictionary<TKey, TValue>, int>>>(x => x.Count);
				Keys = Helpers.GetPropertyInfo<Expression<Func<Dictionary<TKey, TValue>, Dictionary<TKey, TValue>.KeyCollection>>>(x => x.Keys);
				Values = Helpers.GetPropertyInfo<Expression<Func<Dictionary<TKey, TValue>, Dictionary<TKey, TValue>.ValueCollection>>>(x => x.Values);
				Item = typeof(IDictionary<TKey, TValue>).GetProperty("Item");
			}
		}
	}
}
