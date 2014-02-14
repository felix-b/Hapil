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
	public static class SetShortcuts
	{
		public static IHappilOperand<bool> Add<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<T> item)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().Add, item),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ExceptWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			StatementScope.Current.AddStatement(new CallStatement(set, GetReflectionCache<T>().ExceptWith, other));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void IntersectWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			StatementScope.Current.AddStatement(new CallStatement(set, GetReflectionCache<T>().IntersectWith, other));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsProperSubsetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().IsProperSubsetOf, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsProperSupersetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().IsProperSupersetOf, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsSubsetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().IsSubsetOf, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsSupersetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().IsSupersetOf, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Overlaps<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().Overlaps, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> SetEquals<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			return new HappilUnaryExpression<ISet<T>, bool>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorCall<ISet<T>>(GetReflectionCache<T>().SetEquals, other),
				operand: set);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void SymmetricExceptWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			StatementScope.Current.AddStatement(new CallStatement(set, GetReflectionCache<T>().SymmetricExceptWith, other));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void UnionWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			StatementScope.Current.AddStatement(new CallStatement(set, GetReflectionCache<T>().UnionWith, other));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, ReflectionCache> s_ReflectionCacheByItemType =
			new ConcurrentDictionary<Type, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache<T>()
		{
			return s_ReflectionCacheByItemType.GetOrAdd(typeof(T), new ReflectionCache<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class ReflectionCache
		{
			public MethodInfo Add { get; protected set; }
			public MethodInfo ExceptWith { get; protected set; }
			public MethodInfo IntersectWith { get; protected set; }
			public MethodInfo IsProperSubsetOf { get; protected set; }
			public MethodInfo IsProperSupersetOf { get; protected set; }
			public MethodInfo IsSubsetOf { get; protected set; }
			public MethodInfo IsSupersetOf { get; protected set; }
			public MethodInfo Overlaps { get; protected set; }
			public MethodInfo SetEquals { get; protected set; }
			public MethodInfo SymmetricExceptWith { get; protected set; }
			public MethodInfo UnionWith { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<T> : ReflectionCache
		{
			public ReflectionCache()
			{
				Add = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.Add(default(T)));
				ExceptWith = Helpers.GetMethodInfo<Expression<Action<ISet<T>>>>(x => x.ExceptWith(null));
				IntersectWith = Helpers.GetMethodInfo<Expression<Action<ISet<T>>>>(x => x.IntersectWith(null));
				IsProperSubsetOf = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.IsProperSubsetOf(null));
				IsProperSupersetOf = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.IsProperSupersetOf(null));
				IsSubsetOf = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.IsSubsetOf(null));
				IsSupersetOf = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.IsSupersetOf(null));
				Overlaps = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.Overlaps(null));
				SetEquals = Helpers.GetMethodInfo<Expression<Func<ISet<T>, bool>>>(x => x.SetEquals(null));
				SymmetricExceptWith = Helpers.GetMethodInfo<Expression<Action<ISet<T>>>>(x => x.SymmetricExceptWith(null));
				UnionWith = Helpers.GetMethodInfo<Expression<Action<ISet<T>>>>(x => x.UnionWith(null));
			}
		}
	}
}
