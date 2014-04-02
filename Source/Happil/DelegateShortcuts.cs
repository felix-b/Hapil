using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Operands;
using Happil.Members;
using Happil.Statements;

namespace Happil
{
	public static class DelegateShortcuts
	{
		//TODO:redesign
		//public static void Invoke(this IOperand<TypeTemplate.TEventHandler> eventDelegate, IOperand sender, IOperand eventArgs)
		//{
		//	StatementScope.Current.AddStatement(new CallStatement(
		//		eventDelegate, 
		//		GetReflectionCache(eventDelegate.OperandType).Invoke, 
		//		sender, eventArgs));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void Invoke(this IOperand<Action> action)
		//{
		//	StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action>().Invoke));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void Invoke<TArg>(this IOperand<Action<TArg>> action, IOperand<TArg> arg)
		//{
		//	StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg>>().Invoke, arg));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void Invoke<TArg1, TArg2>(
		//	this IOperand<Action<TArg1, TArg2>> action, 
		//	IOperand<TArg1> arg1, 
		//	IOperand<TArg2> arg2)
		//{
		//	StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg1, TArg2>>().Invoke, arg1, arg2));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void Invoke<TArg1, TArg2, TArg3>(
		//	this IOperand<Action<TArg1, TArg2, TArg3>> action, 
		//	IOperand<TArg1> arg1, 
		//	IOperand<TArg2> arg2,
		//	IOperand<TArg3> arg3)
		//{
		//	StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg1, TArg2, TArg3>>().Invoke, arg1, arg2, arg3));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilOperand<TResult> Invoke<TResult>(this IOperand<Func<TResult>> func)
		//{
		//	var @operator = new UnaryOperators.OperatorCall<Func<TResult>>(GetReflectionCache<Func<TResult>>().Invoke);
		//	return new HappilUnaryExpression<Func<TResult>, TResult>(null, @operator, func);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilOperand<TResult> Invoke<TArg, TResult>(
		//	this IOperand<Func<TArg, TResult>> func,
		//	IOperand<TArg> arg)
		//{
		//	var @operator = new UnaryOperators.OperatorCall<Func<TArg, TResult>>(GetReflectionCache<Func<TArg, TResult>>().Invoke, arg);
		//	return new HappilUnaryExpression<Func<TArg, TResult>, TResult>(null, @operator, func);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilOperand<TResult> Invoke<TArg1, TArg2, TResult>(
		//	this IOperand<Func<TArg1, TArg2, TResult>> func,
		//	IOperand<TArg1> arg1,
		//	IOperand<TArg2> arg2)
		//{
		//	var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TResult>>(
		//		GetReflectionCache<Func<TArg1, TArg2, TResult>>().Invoke, 
		//		arg1, arg2);

		//	return new HappilUnaryExpression<Func<TArg1, TArg2, TResult>, TResult>(null, @operator, func);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilOperand<TResult> Invoke<TArg1, TArg2, TArg3, TResult>(
		//	this IOperand<Func<TArg1, TArg2, TArg3, TResult>> func,
		//	IOperand<TArg1> arg1,
		//	IOperand<TArg2> arg2,
		//	IOperand<TArg3> arg3)
		//{
		//	var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TResult>>(
		//		GetReflectionCache<Func<TArg1, TArg2, TArg3, TResult>>().Invoke,
		//		arg1, arg2, arg3);

		//	return new HappilUnaryExpression<Func<TArg1, TArg2, TArg3, TResult>, TResult>(null, @operator, func);
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static ConstructorInfo GetDelegateConstructor(Type delegateType)
		{
			return GetReflectionCache(delegateType).Constructor;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, ReflectionCache> s_ReflectionCacheByDelegateType =
			new ConcurrentDictionary<Type, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache<TDelegate>()
		{
			return GetReflectionCache(typeof(TDelegate));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static ReflectionCache GetReflectionCache(Type delegateType)
		{
			return s_ReflectionCacheByDelegateType.GetOrAdd(delegateType, new ReflectionCache(delegateType));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReflectionCache
		{
			public ReflectionCache(Type delegateType)
			{
				Constructor = delegateType.GetConstructor(new[] { typeof(object), typeof(IntPtr) });
				Invoke = delegateType.GetMethod("Invoke");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ConstructorInfo Constructor { get; private set; }
			public MethodInfo Invoke { get; private set; }
		}
	}
}
