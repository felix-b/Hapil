using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Operands;
using Happil.Statements;

namespace Happil
{
	public static class DictionaryShortcuts
	{
		public static void Add<TKey, TValue>(
			this IOperand<IDictionary<TKey, TValue>> dictionary, 
			IOperand<TKey> key, 
			IOperand<TValue> value)
		{
			StatementScope.Current.AddStatement(new CallStatement(dictionary, GetReflectionCache<TKey, TValue>().Add, key, value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Clear<TKey, TValue>(this IOperand<IDictionary<TKey, TValue>> dictionary)
		{
			StatementScope.Current.AddStatement(new CallStatement(dictionary, GetReflectionCache<TKey, TValue>().Clear));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> ContainsKey<TKey, TValue>(
			this IOperand<IDictionary<TKey, TValue>> dictionary, 
			IOperand<TKey> key)
		{
			var @operator = new UnaryOperators.OperatorCall<IDictionary<TKey, TValue>>(GetReflectionCache<TKey, TValue>().ContainsKey, key);
			return new UnaryExpressionOperand<IDictionary<TKey, TValue>, bool>(@operator, dictionary);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> ContainsValue<TKey, TValue>(
			this IOperand<IDictionary<TKey, TValue>> dictionary,
			IOperand<TValue> value)
		{
			var @operator = new UnaryOperators.OperatorCall<IDictionary<TKey, TValue>>(GetReflectionCache<TKey, TValue>().ContainsValue, value);
			return new UnaryExpressionOperand<IDictionary<TKey, TValue>, bool>(@operator, dictionary);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> Remove<TKey, TValue>(
			this IOperand<IDictionary<TKey, TValue>> dictionary,
			IOperand<TKey> key)
		{
			var @operator = new UnaryOperators.OperatorCall<IDictionary<TKey, TValue>>(GetReflectionCache<TKey, TValue>().Remove, key);
			return new UnaryExpressionOperand<IDictionary<TKey, TValue>, bool>(@operator, dictionary);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> TryGetValue<TKey, TValue>(
			this IOperand<IDictionary<TKey, TValue>> dictionary,
			IOperand<TKey> key,
			IOperand<TValue> value)
		{
			var @operator = new UnaryOperators.OperatorCall<IDictionary<TKey, TValue>>(GetReflectionCache<TKey, TValue>().TryGetValue, key, value);
			return new UnaryExpressionOperand<IDictionary<TKey, TValue>, bool>(@operator, dictionary);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<int> Count<TKey, TValue>(this IOperand<IDictionary<TKey, TValue>> dictionary)
		{
			return new Property<int>(dictionary, GetReflectionCache<TKey, TValue>().Count);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<ICollection<TKey>> Keys<TKey, TValue>(this IOperand<IDictionary<TKey, TValue>> dictionary)
		{
			return new Property<ICollection<TKey>>(dictionary, GetReflectionCache<TKey, TValue>().Keys);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<ICollection<TValue>> Values<TKey, TValue>(this IOperand<IDictionary<TKey, TValue>> dictionary)
		{
			return new Property<ICollection<TValue>>(dictionary, GetReflectionCache<TKey, TValue>().Values);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<TValue> Item<TKey, TValue>(this IOperand<IDictionary<TKey, TValue>> dictionary, IOperand<TKey> key)
		{
			return new Property<TValue>(
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
				Remove = Helpers.GetMethodInfo<Expression<Func<IDictionary<TKey, TValue>, bool>>>(x => x.Remove(default(TKey)));
				TryGetValue = Helpers.GetMethodInfo<Expression<Func<IDictionary<TKey, TValue>, TValue, bool>>>((x, v) => x.TryGetValue(default(TKey), out v));

				Count = Helpers.GetPropertyInfo<Expression<Func<IDictionary<TKey, TValue>, int>>>(x => x.Count);
				Keys = Helpers.GetPropertyInfo<Expression<Func<IDictionary<TKey, TValue>, ICollection<TKey>>>>(x => x.Keys);
				Values = Helpers.GetPropertyInfo<Expression<Func<IDictionary<TKey, TValue>, ICollection<TValue>>>>(x => x.Values);
				Item = typeof(IDictionary<TKey, TValue>).GetProperty("Item");
			}
		}
	}
}
