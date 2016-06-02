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

        public static void Invoke<TArg1, TArg2, TArg3, TArg4>(
            this IOperand<Action<TArg1, TArg2, TArg3, TArg4>> action,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4)
        {
            StatementScope.Current.AddStatement(new CallStatement(
                action, 
                GetReflectionCache<Action<TArg1, TArg2, TArg3, TArg4>>().Invoke, 
                arg1, 
                arg2, 
                arg3,
                arg4));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void Invoke<TArg1, TArg2, TArg3, TArg4, TArg5>(
            this IOperand<Action<TArg1, TArg2, TArg3, TArg4, TArg5>> action,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5)
        {
            StatementScope.Current.AddStatement(new CallStatement(
                action,
                GetReflectionCache<Action<TArg1, TArg2, TArg3, TArg4, TArg5>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
            this IOperand<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>> action,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6)
        {
            StatementScope.Current.AddStatement(new CallStatement(
                action,
                GetReflectionCache<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
            this IOperand<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>> action,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6,
            IOperand<TArg7> arg7)
        {
            StatementScope.Current.AddStatement(new CallStatement(
                action,
                GetReflectionCache<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6,
                arg7));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(
            this IOperand<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>> action,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6,
            IOperand<TArg7> arg7,
            IOperand<TArg8> arg8)
        {
            StatementScope.Current.AddStatement(new CallStatement(
                action,
                GetReflectionCache<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6,
                arg7,
                arg8));
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

        public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TArg4, TResult>(
            this IOperand<Func<TArg1, TArg2, TArg3, TArg4, TResult>> func,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4)
        {
            var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TArg4, TResult>>(
                GetReflectionCache<Func<TArg1, TArg2, TArg3, TArg4, TResult>>().Invoke,
                arg1, 
                arg2, 
                arg3,
                arg4);

            return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TArg4, TResult>, TResult>(@operator, func);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
            this IOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>> func,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5)
        {
            var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>>(
                GetReflectionCache<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5);

            return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>, TResult>(@operator, func);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
            this IOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>> func,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6)
        {
            var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>>(
                GetReflectionCache<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6);

            return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>, TResult>(@operator, func);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
            this IOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>> func,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6,
            IOperand<TArg7> arg7)
        {
            var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>>(
                GetReflectionCache<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6,
                arg7);

            return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>, TResult>(@operator, func);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Operand<TResult> Invoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
            this IOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>> func,
            IOperand<TArg1> arg1,
            IOperand<TArg2> arg2,
            IOperand<TArg3> arg3,
            IOperand<TArg4> arg4,
            IOperand<TArg5> arg5,
            IOperand<TArg6> arg6,
            IOperand<TArg7> arg7,
            IOperand<TArg8> arg8)
        {
            var @operator = new UnaryOperators.OperatorCall<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>>(
                GetReflectionCache<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>>().Invoke,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6,
                arg7,
                arg8);

            return new UnaryExpressionOperand<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>, TResult>(@operator, func);
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
