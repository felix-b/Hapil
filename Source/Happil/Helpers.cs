using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	internal static class Helpers
	{
		public static void EmitCall(ILGenerator il, IHappilOperandInternals target, MethodBase method, params IHappilOperandInternals[] arguments)
		{
			if ( target != null )
			{
				target.EmitTarget(il);
				target.EmitLoad(il);
			}

			foreach ( var argument in arguments )
			{
				argument.EmitTarget(il);
				argument.EmitLoad(il);
			}

			var methodInfo = (method as MethodInfo);
			var constructorInfo = (method as ConstructorInfo);
			var callOpcode = (method.IsVirtual || method.DeclaringType.IsInterface ? OpCodes.Callvirt : OpCodes.Call);

			if ( methodInfo != null )
			{
				il.Emit(callOpcode, methodInfo);
			}
			else
			{
				il.Emit(callOpcode, constructorInfo);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperandInternals CreateConstant(Type type, object value)
		{
			var constantType = typeof(HappilConstant<>).MakeGenericType(type);
			return (IHappilOperandInternals)Activator.CreateInstance(constantType, new object[] { value });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperandInternals GetLambdaArgumentAsConstant(Expression argument)
		{
			var argumentLambda = Expression.Lambda<Func<object>>(argument);
			var argumentValueFunc = argumentLambda.Compile();
			var argumentValue = argumentValueFunc();

			return CreateConstant(argument.Type, argumentValue);
		}
	}
}
