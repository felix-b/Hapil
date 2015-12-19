using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Operands;
using Hapil.Members;
using Hapil.Statements;

namespace Hapil
{
	public static class DelegateShortcuts
	{
		private static readonly ConcurrentDictionary<Type, ReflectionCache> s_ReflectionCacheByDelegateType =
			new ConcurrentDictionary<Type, ReflectionCache>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Invoke(this IOperand<TypeTemplate.TEventHandler> eventDelegate, IOperand sender, IOperand eventArgs)
		{
			StatementScope.Current.AddStatement(new CallStatement(
				eventDelegate,
				GetReflectionCache(eventDelegate.OperandType).Invoke,
				sender, eventArgs));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Invoke(this IOperand<Action> action)
		{
			StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action>().Invoke));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Invoke<TArg>(this IOperand<Action<TArg>> action, IOperand<TArg> arg)
		{
			StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg>>().Invoke, arg));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Invoke<TArg1, TArg2>(
			this IOperand<Action<TArg1, TArg2>> action,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2)
		{
			StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg1, TArg2>>().Invoke, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Invoke<TArg1, TArg2, TArg3>(
			this IOperand<Action<TArg1, TArg2, TArg3>> action,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			StatementScope.Current.AddStatement(new CallStatement(action, GetReflectionCache<Action<TArg1, TArg2, TArg3>>().Invoke, arg1, arg2, arg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TResult> Invoke<TResult>(this IOperand<Func<TResult>> func)
		{
			var @operator = new UnaryOperators.OperatorCall<Func<TResult>>(GetReflectionCache<Func<TResult>>().Invoke);
			return new UnaryExpressionOperand<Func<TResult>, TResult>(@operator, func);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TResult> Invoke<TArg, TResult>(
			this IOperand<Func<TArg, TResult>> func,
			IOperand<TArg> arg)
		{
			var @operator = new UnaryOperators.OperatorCall<Func<TArg, TResult>>(GetReflectionCache<Func<TArg, TResult>>().Invoke, arg);
			return new UnaryExpressionOperand<Func<TArg, TResult>, TResult>(@operator, func);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TResult> Invoke<TArg1, TArg2, TResult>(
			this IOperand<Func<TArg1, TArg2, TResult>> func,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2)
		{
			var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TResult>>(
				GetReflectionCache<Func<TArg1, TArg2, TResult>>().Invoke,
				arg1, arg2);

			return new UnaryExpressionOperand<Func<TArg1, TArg2, TResult>, TResult>(@operator, func);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TResult>(
			this IOperand<Func<TArg1, TArg2, TArg3, TResult>> func,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TResult>>(
				GetReflectionCache<Func<TArg1, TArg2, TArg3, TResult>>().Invoke,
				arg1, arg2, arg3);

			return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TResult>, TResult>(@operator, func);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static ConstructorInfo GetDelegateConstructor(Type delegateType)
		{
			return GetReflectionCache(delegateType).Constructor;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static ReflectionCache GetReflectionCache<TDelegate>()
		{
			return GetReflectionCache(typeof(TDelegate));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static ReflectionCache GetReflectionCache(Type delegateType)
		{
			return s_ReflectionCacheByDelegateType.GetOrAdd(delegateType, new ReflectionCache(delegateType));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal class ReflectionCache
		{
			public ReflectionCache(Type delegateType)
			{
				Constructor = delegateType.GetRuntimeOrBuilderConstructor(new[] { typeof(object), typeof(IntPtr) });

			    if ( delegateType.IsGenericType && delegateType.GetType().Name.Contains("Builder") )
			    {
			        var genericDefinition = delegateType.GetGenericTypeDefinition();
			        var genericDefinitionMethod = genericDefinition.GetMethod("Invoke");
			        Invoke = TypeBuilder.GetMethod(delegateType, genericDefinitionMethod);
			    }
			    else
			    {
			        Invoke = delegateType.GetMethod("Invoke");
			    }
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ConstructorInfo Constructor { get; private set; }
			public MethodInfo Invoke { get; private set; }
		}
	}
}
