﻿using System;
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
	public static class EnumerableShortcuts
	{
		public static HappilOperand<bool> All<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.All, source, predicate);
			return new HappilUnaryExpression<IEnumerable<TSource>, bool>(null, @operator, null);
		}
		public static HappilOperand<bool> All<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<bool>> predicateLambda)
		{
			return All<TSource>(source, StatementScope.Current.OwnerMethod.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> Any<TSource>(this IHappilOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Any, source);
			return new HappilUnaryExpression<IEnumerable<TSource>, bool>(null, @operator, null);
		}
		public static HappilOperand<bool> Any<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.AnyWithPredicate, source, predicate);
			return new HappilUnaryExpression<IEnumerable<TSource>, bool>(null, @operator, null);
		}
		public static HappilOperand<bool> Any<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<bool>> predicateLambda)
		{
			return Any<TSource>(source, StatementScope.Current.OwnerMethod.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TResult>> Cast<TSource, TResult>(this IHappilOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Cast<TResult>(), source);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TResult>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<IGrouping<TKey, TSource>>> GroupBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return GroupBy<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<IEnumerable<IGrouping<TKey, TSource>>> GroupBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.GroupBy<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<IGrouping<TKey, TSource>>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TResult>> OfType<TSource, TResult>(this IHappilOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OfType<TResult>(), source);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TResult>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return OrderBy<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderBy<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return OrderByDescending<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderByDescending<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TResult>> Select<TSource, TResult>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TResult>> resultSelectorLambda)
		{
			return Select<TSource, TResult>(source, StatementScope.Current.OwnerMethod.Lambda(resultSelectorLambda));
		}
		public static HappilOperand<IEnumerable<TResult>> Select<TSource, TResult>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TResult>> resultSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Select<TResult>(), source, resultSelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TResult>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TResult>> SelectMany<TSource, TResult>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<IEnumerable<TResult>>> resultSelectorLambda)
		{
			return SelectMany<TSource, TResult>(source, StatementScope.Current.OwnerMethod.Lambda(resultSelectorLambda));
		}
		public static HappilOperand<IEnumerable<TResult>> SelectMany<TSource, TResult>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, IEnumerable<TResult>>> resultSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SelectMany<TResult>(), source, resultSelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TResult>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> ThenBy<TSource, TKey>(
			this IHappilOperand<IOrderedEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return ThenBy<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<IOrderedEnumerable<TSource>> ThenBy<TSource, TKey>(
			this IHappilOperand<IOrderedEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IOrderedEnumerable<TSource>>(methods.ThenBy<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> ThenByDescending<TSource, TKey>(
			this IHappilOperand<IOrderedEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return ThenByDescending<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<IOrderedEnumerable<TSource>> ThenByDescending<TSource, TKey>(
			this IHappilOperand<IOrderedEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IOrderedEnumerable<TSource>>(methods.ThenByDescending<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<Dictionary<TKey, TSource>> ToDictionary<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return ToDictionary<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}
		public static HappilOperand<Dictionary<TKey, TSource>> ToDictionary<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToDictionary<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, Dictionary<TKey, TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<Dictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda,
			Func<HappilOperand<TSource>, IHappilOperand<TElement>> elementSelectorLambda)
		{
			return ToDictionary<TSource, TKey, TElement>(
				source, 
				StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda),
				StatementScope.Current.OwnerMethod.Lambda(elementSelectorLambda));
		}
		public static HappilOperand<Dictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector,
			IHappilOperand<Func<TSource, TElement>> elementSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToDictionary<TKey, TElement>(), source, keySelector, elementSelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, Dictionary<TKey, TElement>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TSource>> Where<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Where, source, predicate);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TSource>>(null, @operator, null);
		}
		public static HappilOperand<IEnumerable<TSource>> Where<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<bool>> predicateLambda)
		{
			return Where<TSource>(source, StatementScope.Current.OwnerMethod.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, ReflectionCache> s_ReflectionCacheByItemType = 
			new ConcurrentDictionary<Type, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache<TSource>()
		{
			return s_ReflectionCacheByItemType.GetOrAdd(typeof(TSource), new ReflectionCache<TSource>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class ReflectionCache
		{
			public abstract MethodInfo Cast<TResult>();
			public abstract MethodInfo GroupBy<TKey>();
			public abstract MethodInfo OfType<TResult>();
			public abstract MethodInfo OrderBy<TKey>();
			public abstract MethodInfo OrderByDescending<TKey>();
			public abstract MethodInfo Select<TResult>();
			public abstract MethodInfo SelectMany<TResult>();
			public abstract MethodInfo ThenBy<TKey>();
			public abstract MethodInfo ThenByDescending<TKey>();
			public abstract MethodInfo ToDictionary<TKey>();
			public abstract MethodInfo ToDictionary<TKey, TElement>();
			public MethodInfo All { get; protected set; }
			public MethodInfo Any { get; protected set; }
			public MethodInfo AnyWithPredicate { get; protected set; }
			public MethodInfo Concat { get; protected set; }
			public MethodInfo Contains { get; protected set; }
			public MethodInfo Count { get; protected set; }
			public MethodInfo CountWithPredicate { get; protected set; }
			public MethodInfo DefaultIfEmpty { get; protected set; }
			public MethodInfo DefaultIfEmptyWithValue { get; protected set; }
			public MethodInfo Distinct { get; protected set; }
			public MethodInfo ElementAt { get; protected set; }
			public MethodInfo ElementAtOrDefault { get; protected set; }
			public MethodInfo Except { get; protected set; }
			public MethodInfo First { get; protected set; }
			public MethodInfo FirstWithPredicate { get; protected set; }
			public MethodInfo FirstOrDefault { get; protected set; }
			public MethodInfo FirstOrDefaultWithPredicate { get; protected set; }
			public MethodInfo Intersect { get; protected set; }
			public MethodInfo Last { get; protected set; }
			public MethodInfo LastWithPredicate { get; protected set; }
			public MethodInfo LastOrDefault { get; protected set; }
			public MethodInfo LastOrDefaultWithPredicate { get; protected set; }
			public MethodInfo Reverse { get; protected set; }
			public MethodInfo SequenceEqual { get; protected set; }
			public MethodInfo Single { get; protected set; }
			public MethodInfo SingleWithPredicate { get; protected set; }
			public MethodInfo SingleOrDefault { get; protected set; }
			public MethodInfo SingleOrDefaultWithPredicate { get; protected set; }
			public MethodInfo Skip { get; protected set; }
			public MethodInfo SkipWhile { get; protected set; }
			public MethodInfo SkipWhileWithIndex { get; protected set; }
			public MethodInfo Take { get; protected set; }
			public MethodInfo TakeWhile { get; protected set; }
			public MethodInfo TakeWhileWithIndex { get; protected set; }
			public MethodInfo ToArray { get; protected set; }
			public MethodInfo ToList { get; protected set; }
			public MethodInfo Union { get; protected set; }
			public MethodInfo Where { get; protected set; }
			public MethodInfo WhereWithIndex { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private sealed class ReflectionCache<TSource> : ReflectionCache
		{
			public ReflectionCache()
			{
				All = GetMethodInfo<Expression<Func<IEnumerable<TSource>, bool>>>(source => source.All(item => true));
				Any = GetMethodInfo<Expression<Func<IEnumerable<TSource>, bool>>>(source => source.Any());
				AnyWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, bool>>>(source => source.Any(item => true));
				Concat = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>, IEnumerable<TSource>>>>((left, right) => left.Concat(right));
				Contains = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource, bool>>>((source, value) => source.Contains(value));
				Count = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int>>>(source => source.Count());
				CountWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int>>>(source => source.Count(item => true));
				DefaultIfEmpty = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.DefaultIfEmpty());
				DefaultIfEmptyWithValue = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.DefaultIfEmpty(default(TSource)));
				Distinct = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.Distinct());
				ElementAt = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int, TSource>>>((source, index) => source.ElementAt(index));
				ElementAtOrDefault = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int, TSource>>>((source, index) => source.ElementAtOrDefault(index));
				Except = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>, IEnumerable<TSource>>>>((left, right) => left.Except(right));
				First = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.First());
				FirstWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.First(item => true));
				FirstOrDefault = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.FirstOrDefault());
				FirstOrDefaultWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.FirstOrDefault(item => true));
				Intersect = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>, IEnumerable<TSource>>>>((left, right) => left.Intersect(right));
				Last = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.Last());
				LastWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.Last(item => true));
				LastOrDefault = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.LastOrDefault());
				LastOrDefaultWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.LastOrDefault(item => true));
				Reverse = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.Reverse());
				SequenceEqual = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>, bool>>>((left, right) => left.SequenceEqual(right));
				Single = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.Single());
				SingleWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.Single(item => true));
				SingleOrDefault = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.SingleOrDefault());
				SingleOrDefaultWithPredicate = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource>>>(source => source.SingleOrDefault(item => true));
				Skip = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int, IEnumerable<TSource>>>>((source, count) => source.Skip(count));
				SkipWhile = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.SkipWhile(item => true));
				SkipWhileWithIndex = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.SkipWhile((item, index) => true));
				Take = GetMethodInfo<Expression<Func<IEnumerable<TSource>, int, IEnumerable<TSource>>>>((source, count) => source.Take(count));
				TakeWhile = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.TakeWhile(item => true));
				TakeWhileWithIndex = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.TakeWhile((item, index) => true));
				ToArray = GetMethodInfo<Expression<Func<IEnumerable<TSource>, TSource[]>>>(source => source.ToArray());
				ToList = GetMethodInfo<Expression<Func<IEnumerable<TSource>, List<TSource>>>>(source => source.ToList());
				Union = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>, IEnumerable<TSource>>>>((left, right) => left.Union(right));
				Where = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.Where(item => true));
				WhereWithIndex = GetMethodInfo<Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>>>(source => source.Where((item, index) => true));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override MethodInfo Cast<TResult>()
			{
				return GetMethodInfo(CastMethod<TResult>());
			}
			public override MethodInfo GroupBy<TKey>()
			{
				return GetMethodInfo(GroupByMethod<TKey>());
			}
			public override MethodInfo OrderBy<TKey>()
			{
				return GetMethodInfo(OrderByMethod<TKey>());
			}
			public override MethodInfo OrderByDescending<TKey>()
			{
				return GetMethodInfo(OrderByDescendingMethod<TKey>());
			}
			public override MethodInfo OfType<TResult>()
			{
				return GetMethodInfo(OfTypeMethod<TResult>());
			}
			public override MethodInfo Select<TResult>()
			{
				return GetMethodInfo(SelectMethod<TResult>());
			}
			public override MethodInfo SelectMany<TResult>()
			{
				return GetMethodInfo(SelectManyMethod<TResult>());
			}
			public override MethodInfo ThenBy<TKey>()
			{
				return GetMethodInfo(ThenByMethod<TKey>());
			}
			public override MethodInfo ThenByDescending<TKey>()
			{
				return GetMethodInfo(ThenByDescendingMethod<TKey>());
			}
			public override MethodInfo ToDictionary<TKey>()
			{
				return GetMethodInfo(ToDictionaryMethod<TKey>());
			}
			public override MethodInfo ToDictionary<TKey, TElement>()
			{
				return GetMethodInfo(ToDictionaryMethod<TKey, TElement>());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private MethodInfo GetMethodInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
			{
				return ((MethodCallExpression)lambda.Body).Method;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private LambdaExpression CastMethod<TResult>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<TResult>>> lambda = (source => source.Cast<TResult>());
				return lambda;
			}
			private LambdaExpression GroupByMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<IGrouping<TKey, TSource>>>> lambda = (source => source.GroupBy<TSource, TKey>(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression OfTypeMethod<TResult>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<TResult>>> lambda = (source => source.OfType<TResult>());
				return lambda;
			}
			private LambdaExpression OrderByMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.OrderBy(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression OrderByDescendingMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.OrderByDescending(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression SelectMethod<TResult>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<TResult>>> lambda = (source => source.Select(item => default(TResult)));
				return lambda;
			}
			private LambdaExpression SelectManyMethod<TResult>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<TResult>>> lambda = (source => source.SelectMany(item => new TResult[0]));
				return lambda;
			}
			private LambdaExpression ThenByMethod<TKey>()
			{
				Expression<Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.ThenBy(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression ThenByDescendingMethod<TKey>()
			{
				Expression<Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.ThenByDescending(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression ToDictionaryMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, Dictionary<TKey, TSource>>> lambda = (source => source.ToDictionary(item => default(TKey)));
				return lambda;
			}
			private LambdaExpression ToDictionaryMethod<TKey, TElement>()
			{
				Expression<Func<IEnumerable<TSource>, Dictionary<TKey, TElement>>> lambda = (source => source.ToDictionary(
					item => default(TKey),
					item => default(TElement)));
				return lambda;
			}
		}
	}
}
