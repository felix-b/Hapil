﻿using System;
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
	public static class CollectionShortcuts
	{
		public static Operand<int> IndexOf<T>(this IOperand<ICollection<T>> collection, IOperand<T> item)
		{
			var @operator = new UnaryOperators.OperatorCall<ICollection<T>>(GetReflectionCache<T>().IndexOf, item);
			return new UnaryExpressionOperand<ICollection<T>, int>(@operator, collection);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Insert<T>(this IOperand<ICollection<T>> collection, IOperand<int> index, IOperand<T> item)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().Insert, index, item));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void RemoveAt<T>(this IOperand<IList<T>> collection, IOperand<int> index)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().RemoveAt, index));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<T> ItemAt<T>(this IOperand<IList<T>> collection, IOperand<int> index)
		{
			return new PropertyAccessOperand<T>(collection, GetReflectionCache<T>().Item, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<T> ItemAt<T>(this IOperand<IList<T>> collection, int index)
		{
			return ItemAt(collection, new ConstantOperand<int>(index));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<T> ElementAt<T>(this IOperand<T[]> array, IOperand<int> index)
		{
			return new ArrayElementAccessOperand<T>(array, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<T> ElementAt<T>(this IOperand<T[]> array, int index)
		{
			return ElementAt(array, new ConstantOperand<int>(index));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Add<T>(this IOperand<ICollection<T>> collection, IOperand<T> item)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().Add, item));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Clear<T>(this IOperand<ICollection<T>> collection)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().Clear));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<bool> Contains<T>(this IOperand<ICollection<T>> collection, IOperand<T> item)
		{
			var @operator = new UnaryOperators.OperatorCall<ICollection<T>>(GetReflectionCache<T>().Contains, item);
			return new UnaryExpressionOperand<ICollection<T>, bool>(@operator, collection);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void CopyTo<T>(this IOperand<ICollection<T>> collection, IOperand<T[]> array, IOperand<int> arrayIndex)
		{
			StatementScope.Current.AddStatement(new CallStatement(collection, GetReflectionCache<T>().CopyTo, array, arrayIndex));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<int> Count<T>(this IOperand<ICollection<T>> collection)
		{
			return new PropertyAccessOperand<int>(collection, GetReflectionCache<T>().Count);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<bool> IsReadOnly<T>(this IOperand<ICollection<T>> collection)
		{
			return new PropertyAccessOperand<bool>(collection, GetReflectionCache<T>().IsReadOnly);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<bool> Remove<T>(this IOperand<ICollection<T>> collection, IOperand<T> item)
		{
			var @operator = new UnaryOperators.OperatorCall<ICollection<T>>(GetReflectionCache<T>().Remove, item);
			return new UnaryExpressionOperand<ICollection<T>, bool>(@operator, collection);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<int> Length<T>(this IOperand<T[]> array)
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
			public MethodInfo IndexOf { get; protected set; }
			public MethodInfo Insert { get; protected set; }
			public MethodInfo RemoveAt { get; protected set; }
			public MethodInfo Clear { get; protected set; }
			public MethodInfo Contains { get; protected set; }
			public MethodInfo CopyTo { get; protected set; }
			public MethodInfo Remove { get; protected set; }
			public PropertyInfo Count { get; protected set; }
			public PropertyInfo IsReadOnly { get; protected set; }
			public PropertyInfo Item { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<T> : ReflectionCache
		{
			public ReflectionCache()
			{
				Add = Helpers.GetMethodInfo<Expression<Action<ICollection<T>>>>(x => x.Add(default(T)));
				IndexOf = Helpers.GetMethodInfo<Expression<Func<IList<T>, int>>>(x => x.IndexOf(default(T)));
				Insert = Helpers.GetMethodInfo<Expression<Action<IList<T>>>>(x => x.Insert(0, default(T)));
				RemoveAt = Helpers.GetMethodInfo<Expression<Action<IList<T>>>>(x => x.RemoveAt(0));
				Clear = Helpers.GetMethodInfo<Expression<Action<ICollection<T>>>>(x => x.Clear());
				Contains = Helpers.GetMethodInfo<Expression<Func<ICollection<T>, bool>>>(x => x.Contains(default(T)));
				CopyTo = Helpers.GetMethodInfo<Expression<Action<ICollection<T>>>>(x => x.CopyTo(new T[0], 0));
				Remove = Helpers.GetMethodInfo<Expression<Action<ICollection<T>>>>(x => x.Remove(default(T)));

				Count = Helpers.GetPropertyInfo<Expression<Func<ICollection<T>, int>>>(x => x.Count);
				IsReadOnly = Helpers.GetPropertyInfo<Expression<Func<ICollection<T>, bool>>>(x => x.IsReadOnly);
				Item = typeof(IList<T>).GetProperty("Item");
			}
		}
	}
}
