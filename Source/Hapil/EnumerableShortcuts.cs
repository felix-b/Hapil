using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Hapil.Expressions;
using Hapil.Operands;
using Hapil.Members;
using Hapil.Statements;

namespace Hapil
{
	public static class EnumerableShortcuts
	{
		public static Operand<bool> All<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.All, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, bool>(@operator, null);
		}
		public static Operand<bool> All<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return All<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> Any<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Any, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, bool>(@operator, null);
		}
		public static Operand<bool> Any<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.AnyWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, bool>(@operator, null);
		}
		public static Operand<bool> Any<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return Any<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<Double> Average(this IOperand<IEnumerable<Int32>> source)
		{
			var methods = GetReflectionCache<Int32>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<Int32>>(methods.Average, source);
			return new UnaryExpressionOperand<IEnumerable<Int32>, Double>(@operator, null);
		}
		public static Operand<Double> Average(this IOperand<IEnumerable<Int64>> source)
		{
			var methods = GetReflectionCache<Int64>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<Int64>>(methods.Average, source);
			return new UnaryExpressionOperand<IEnumerable<Int64>, Double>(@operator, null);
		}
		public static Operand<Single> Average(this IOperand<IEnumerable<Single>> source)
		{
			var methods = GetReflectionCache<Single>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<Single>>(methods.Average, source);
			return new UnaryExpressionOperand<IEnumerable<Single>, Single>(@operator, null);
		}
		public static Operand<Decimal> Average(this IOperand<IEnumerable<Decimal>> source)
		{
			var methods = GetReflectionCache<Decimal>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<Decimal>>(methods.Average, source);
			return new UnaryExpressionOperand<IEnumerable<Decimal>, Decimal>(@operator, null);
		}
		public static Operand<Double> Average(this IOperand<IEnumerable<Double>> source)
		{
			var methods = GetReflectionCache<Double>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<Double>>(methods.Average, source);
			return new UnaryExpressionOperand<IEnumerable<Double>, Double>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TResult>> Cast<TResult>(this IOperand<System.Collections.IEnumerable> source)
		{
			var castMethod = GetCastMethod<TResult>();
			var @operator = new UnaryOperators.OperatorCall<System.Collections.IEnumerable>(castMethod, source);
			return new UnaryExpressionOperand<System.Collections.IEnumerable, IEnumerable<TResult>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<IGrouping<TKey, TSource>>> GroupBy<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return GroupBy<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<IEnumerable<IGrouping<TKey, TSource>>> GroupBy<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.GroupBy<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<IGrouping<TKey, TSource>>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Min<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();

			if ( methods.Min == null )
			{
				throw new InvalidOperationException(string.Format("Method [Min] is not supported on enumerable of element type [{0}]", typeof(TSource).Name));
			}

			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Min, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Max<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();

			if ( methods.Max == null )
			{
				throw new InvalidOperationException(string.Format("Method [Max] is not supported on enumerable of element type [{0}]", typeof(TSource).Name));
			}

			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Max, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TResult>> OfType<TResult>(this IOperand<System.Collections.IEnumerable> source)
		{
			var ofTypeMethod = GetOfTypeMethod<TResult>();
			var @operator = new UnaryOperators.OperatorCall<System.Collections.IEnumerable>(ofTypeMethod, source);
			return new UnaryExpressionOperand<System.Collections.IEnumerable, IEnumerable<TResult>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return OrderBy<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<IOrderedEnumerable<TSource>> OrderBy<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderBy<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return OrderByDescending<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<IOrderedEnumerable<TSource>> OrderByDescending<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.OrderByDescending<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IOrderedEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TResult>> Select<TSource, TResult>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TResult>> resultSelectorLambda)
		{
			return Select<TSource, TResult>(source, StatementScope.Current.Writer.Lambda(resultSelectorLambda));
		}
		public static Operand<IEnumerable<TResult>> Select<TSource, TResult>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TResult>> resultSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Select<TResult>(), source, resultSelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TResult>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TResult>> SelectMany<TSource, TResult>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<IEnumerable<TResult>>> resultSelectorLambda)
		{
			return SelectMany<TSource, TResult>(source, StatementScope.Current.Writer.Lambda(resultSelectorLambda));
		}
		public static Operand<IEnumerable<TResult>> SelectMany<TSource, TResult>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, IEnumerable<TResult>>> resultSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SelectMany<TResult>(), source, resultSelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TResult>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Sum<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();

			if ( methods.Sum == null )
			{
				throw new InvalidOperationException(string.Format("Method [Sum] is not supported on enumerable of element type [{0}]", typeof(TSource).Name));
			}

			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Sum, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IOrderedEnumerable<TSource>> ThenBy<TSource, TKey>(
			this IOperand<IOrderedEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return ThenBy<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<IOrderedEnumerable<TSource>> ThenBy<TSource, TKey>(
			this IOperand<IOrderedEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IOrderedEnumerable<TSource>>(methods.ThenBy<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IOrderedEnumerable<TSource>> ThenByDescending<TSource, TKey>(
			this IOperand<IOrderedEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return ThenByDescending<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<IOrderedEnumerable<TSource>> ThenByDescending<TSource, TKey>(
			this IOperand<IOrderedEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IOrderedEnumerable<TSource>>(methods.ThenByDescending<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<Dictionary<TKey, TSource>> ToDictionary<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda)
		{
			return ToDictionary<TSource, TKey>(source, StatementScope.Current.Writer.Lambda(keySelectorLambda));
		}
		public static Operand<Dictionary<TKey, TSource>> ToDictionary<TSource, TKey>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToDictionary<TKey>(), source, keySelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, Dictionary<TKey, TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<Dictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<TKey>> keySelectorLambda,
			Func<Operand<TSource>, IOperand<TElement>> elementSelectorLambda)
		{
			return ToDictionary<TSource, TKey, TElement>(
				source,
				StatementScope.Current.Writer.Lambda(keySelectorLambda),
				StatementScope.Current.Writer.Lambda(elementSelectorLambda));
		}
		public static Operand<Dictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, TKey>> keySelector,
			IOperand<Func<TSource, TElement>> elementSelector)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToDictionary<TKey, TElement>(), source, keySelector, elementSelector);
			return new UnaryExpressionOperand<IEnumerable<TSource>, Dictionary<TKey, TElement>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Where<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Where, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> Where<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return Where<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Where<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, int, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.WhereWithIndex, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> Where<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, Operand<int>, IOperand<bool>> predicateLambda)
		{
			return Where<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Concat<TSource>(
			this IOperand<IEnumerable<TSource>> first,
			IOperand<IEnumerable<TSource>> second)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Concat, first, second);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> Contains<TSource>(
			this IOperand<IEnumerable<TSource>> source, 
			IOperand<TSource> value)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Contains, source, value);
			return new UnaryExpressionOperand<IEnumerable<TSource>, bool>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<int> Count<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Count, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, int>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<int> Count<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Where, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, int>(@operator, null);
		}
		public static Operand<int> Count<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return Count<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> DefaultIfEmpty<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.DefaultIfEmpty, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> DefaultIfEmpty<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<TSource> defaultValue)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.DefaultIfEmptyWithValue, source, defaultValue);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public static Operand<IEnumerable<TSource>> Distinct<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Distinct, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> ElementAt<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<int> index)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ElementAt, source, index);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> ElementAtOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<int> index)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ElementAtOrDefault, source, index);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Except<TSource>(
			this IOperand<IEnumerable<TSource>> first,
			IOperand<IEnumerable<TSource>> second)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Except, first, second);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> First<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.First, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> First<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.FirstWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> First<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return First<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> FirstOrDefault<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.FirstOrDefault, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> FirstOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.FirstOrDefaultWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> FirstOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return FirstOrDefault<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Intersect<TSource>(
			this IOperand<IEnumerable<TSource>> first,
			IOperand<IEnumerable<TSource>> second)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Intersect, first, second);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Last<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Last, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Last<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.LastWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> Last<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return Last<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> LastOrDefault<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.LastOrDefault, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> LastOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.LastOrDefaultWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> LastOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return LastOrDefault<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Reverse<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Reverse, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> SequenceEqual<TSource>(
			this IOperand<IEnumerable<TSource>> first,
			IOperand<IEnumerable<TSource>> second)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SequenceEqual, first, second);
			return new UnaryExpressionOperand<IEnumerable<TSource>, bool>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Single<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Single, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> Single<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SingleWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> Single<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return Single<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> SingleOrDefault<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SingleOrDefault, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource> SingleOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SingleOrDefaultWithPredicate, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource>(@operator, null);
		}
		public static Operand<TSource> SingleOrDefault<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return SingleOrDefault<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Skip<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<int> count)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Skip, source, count);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> SkipWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SkipWhile, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> SkipWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return SkipWhile<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> SkipWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, int, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.SkipWhileWithIndex, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> SkipWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, Operand<int>, IOperand<bool>> predicateLambda)
		{
			return SkipWhile<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Take<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<int> count)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Take, source, count);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> TakeWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.TakeWhile, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> TakeWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, IOperand<bool>> predicateLambda)
		{
			return TakeWhile<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> TakeWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			IOperand<Func<TSource, int, bool>> predicate)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.TakeWhileWithIndex, source, predicate);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
		}
		public static Operand<IEnumerable<TSource>> TakeWhile<TSource>(
			this IOperand<IEnumerable<TSource>> source,
			Func<Operand<TSource>, Operand<int>, IOperand<bool>> predicateLambda)
		{
			return TakeWhile<TSource>(source, StatementScope.Current.Writer.Lambda(predicateLambda));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TSource[]> ToArray<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToArray, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, TSource[]>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<List<TSource>> ToList<TSource>(this IOperand<IEnumerable<TSource>> source)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.ToList, source);
			return new UnaryExpressionOperand<IEnumerable<TSource>, List<TSource>>(@operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<IEnumerable<TSource>> Union<TSource>(
			this IOperand<IEnumerable<TSource>> first,
			IOperand<IEnumerable<TSource>> second)
		{
			var methods = GetReflectionCache<TSource>();
			var @operator = new UnaryOperators.OperatorCall<IEnumerable<TSource>>(methods.Union, first, second);
			return new UnaryExpressionOperand<IEnumerable<TSource>, IEnumerable<TSource>>(@operator, null);
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

		private static MethodInfo GetMethodInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
		{
			return ((MethodCallExpression)lambda.Body).Method;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo GetCastMethod<TResult>()
		{
			Expression<Func<System.Collections.IEnumerable, IEnumerable<TResult>>> lambda = (source => source.Cast<TResult>());
			return GetMethodInfo(lambda);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo GetOfTypeMethod<TResult>()
		{
			Expression<Func<System.Collections.IEnumerable, IEnumerable<TResult>>> lambda = (source => source.OfType<TResult>());
			return GetMethodInfo(lambda);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class ReflectionCache
		{
			public abstract MethodInfo GroupBy<TKey>();
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
			public MethodInfo Average { get; protected set; }
			public MethodInfo Min { get; protected set; }
			public MethodInfo Max { get; protected set; }
			public MethodInfo Sum { get; protected set; }
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

				InitializeTypeSpecificMethods();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

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

			private void InitializeTypeSpecificMethods()
			{
				if ( typeof(TSource) == typeof(Int32) )
				{
					Min = GetMethodInfo<Expression<Func<IEnumerable<Int32>, Int32>>>(source => source.Min());
					Max = GetMethodInfo<Expression<Func<IEnumerable<Int32>, Int32>>>(source => source.Max());
					Average = GetMethodInfo<Expression<Func<IEnumerable<Int32>,	Double>>>(source => source.Average());
					Sum = GetMethodInfo<Expression<Func<IEnumerable<Int32>, Int32>>>(source => source.Sum());
				}
				else if ( typeof(TSource) == typeof(Int64) )
				{
					Min = GetMethodInfo<Expression<Func<IEnumerable<Int64>, Int64>>>(source => source.Min());
					Max = GetMethodInfo<Expression<Func<IEnumerable<Int64>, Int64>>>(source => source.Max());
					Average = GetMethodInfo<Expression<Func<IEnumerable<Int64>, Double>>>(source => source.Average());
					Sum = GetMethodInfo<Expression<Func<IEnumerable<Int64>, Int64>>>(source => source.Sum());
				}
				else if ( typeof(TSource) == typeof(Single) )
				{
					Min = GetMethodInfo<Expression<Func<IEnumerable<Single>, Single>>>(source => source.Min());
					Max = GetMethodInfo<Expression<Func<IEnumerable<Single>, Single>>>(source => source.Max());
					Average = GetMethodInfo<Expression<Func<IEnumerable<Single>, Single>>>(source => source.Average());
					Sum = GetMethodInfo<Expression<Func<IEnumerable<Single>, Single>>>(source => source.Sum());
				}
				else if ( typeof(TSource) == typeof(Decimal) )
				{
					Min = GetMethodInfo<Expression<Func<IEnumerable<Decimal>, Decimal>>>(source => source.Min());
					Max = GetMethodInfo<Expression<Func<IEnumerable<Decimal>, Decimal>>>(source => source.Max());
					Average = GetMethodInfo<Expression<Func<IEnumerable<Decimal>, Decimal>>>(source => source.Average());
					Sum = GetMethodInfo<Expression<Func<IEnumerable<Decimal>, Decimal>>>(source => source.Sum());
				}
				else if ( typeof(TSource) == typeof(Double) )
				{
					Min = GetMethodInfo<Expression<Func<IEnumerable<Double>, Double>>>(source => source.Min());
					Max = GetMethodInfo<Expression<Func<IEnumerable<Double>, Double>>>(source => source.Max());
					Average = GetMethodInfo<Expression<Func<IEnumerable<Double>, Double>>>(source => source.Average());
					Sum = GetMethodInfo<Expression<Func<IEnumerable<Double>, Double>>>(source => source.Sum());
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private LambdaExpression GroupByMethod<TKey>()
			{
				Expression<Func<IEnumerable<TSource>, IEnumerable<IGrouping<TKey, TSource>>>> lambda = (source => source.GroupBy<TSource, TKey>(item => default(TKey)));
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
