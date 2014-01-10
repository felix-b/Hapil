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

		public static void Clear<T>(this IHappilOperand<HashSet<T>> set)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Contains<T>(this IHappilOperand<HashSet<T>> set, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static IHappilOperand<T[]> ToArray<T>(this IHappilOperand<ISet<T>> set)
		//{
		//	throw new NotImplementedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void CopyTo<T>(
		//	this IHappilOperand<ISet<T>> set, 
		//	IHappilOperand<T[]> array, 
		//	IHappilOperand<int> arrayIndex = null, 
		//	IHappilOperand<int> count = null)
		//{
		//	throw new NotImplementedException();
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ExceptWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void IntersectWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsProperSubsetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsProperSupersetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsSubsetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> IsSupersetOf<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Overlaps<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> Remove<T>(this IHappilOperand<HashSet<T>> set, IHappilOperand<T> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<bool> SetEquals<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void SymmetricExceptWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void UnionWith<T>(this IHappilOperand<ISet<T>> set, IHappilOperand<IEnumerable<T>> other)
		{
			throw new NotImplementedException();
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
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache<T> : ReflectionCache
		{
			public ReflectionCache()
			{
				var isetType = typeof(ISet<T>);
				var hashSetType = typeof(HashSet<T>);

				base.Add = isetType.GetMethod("Add");
			}
		}
	}
}
