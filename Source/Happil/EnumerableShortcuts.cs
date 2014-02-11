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
	public static class EnumerableShortcuts
	{
		public static HappilOperand<IEnumerable<TSource>> Where<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Where, source, predicate);
			return new HappilUnaryExpression<IEnumerable<TSource>, IEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IEnumerable<TSource>> Where<TSource>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<bool>> predicateLambda)
		{
			return Where<TSource>(source, StatementScope.Current.OwnerMethod.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderBy<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return OrderBy<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			IHappilOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderByDescending<TKey>(), source, keySelector);
			return new HappilUnaryExpression<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IHappilOperand<IEnumerable<TSource>> source,
			Func<HappilOperand<TSource>, IHappilOperand<TKey>> keySelectorLambda)
		{
			return OrderByDescending<TSource, TKey>(source, StatementScope.Current.OwnerMethod.Lambda(keySelectorLambda));
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
			public abstract MethodInfo OrderBy<TKey>();
			public abstract MethodInfo OrderByDescending<TKey>();
			public MethodInfo Where { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private sealed class ReflectionCache<TSource> : ReflectionCache
		{
			public ReflectionCache()
			{
				Where = GetMethodInfo(WhereMethod());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override MethodInfo OrderBy<TKey>()
			{
				return GetMethodInfo(OrderByMethod<TKey>());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override MethodInfo OrderByDescending<TKey>()
			{
				return GetMethodInfo(OrderByDescendingMethod<TKey>());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private MethodInfo GetMethodInfo(LambdaExpression lambda)
			{
				return ((MethodCallExpression)lambda.Body).Method;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private LambdaExpression WhereMethod()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<TSource>>> lambda = (source => source.Where(item => true));
				return lambda;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private LambdaExpression OrderByMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.OrderBy(item => default(TKey)));
				return lambda;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private LambdaExpression OrderByDescendingMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>>> lambda = (source => source.OrderByDescending(item => default(TKey)));
				return lambda;
			}
		}
	}
}
