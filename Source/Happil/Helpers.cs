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
		public static void EmitCall(ILGenerator il, IHappilOperand target, MethodBase method, params IHappilOperand[] arguments)
		{
			if ( target != null )
			{
				if ( target.OperandType.IsValueType )
				{
					target.EmitTarget(il);
					target.EmitAddress(il);
				}
				else
				{
					target.EmitTarget(il);
					target.EmitLoad(il);
				}
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

		public static IHappilOperand CreateConstant(Type type, object value)
		{
			var constantType = typeof(HappilConstant<>).MakeGenericType(type);
			return (IHappilOperand)Activator.CreateInstance(constantType, new object[] { value });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand GetLambdaArgumentAsConstant(Expression argument)
		{
			var argumentLambda = Expression.Lambda<Func<object>>(argument);
			var argumentValueFunc = argumentLambda.Compile();
			var argumentValue = argumentValueFunc();

			return CreateConstant(argument.Type, argumentValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodInfo[] GetMethodInfoFromLambda(LambdaExpression lambda)
		{
			var createDelegateCall = (MethodCallExpression)(((UnaryExpression)lambda.Body).Operand);
			var methodDeclaration = (MethodInfo)((ConstantExpression)createDelegateCall.Arguments[2]).Value;

			if ( TypeTemplate.IsTemplateType(methodDeclaration.DeclaringType) )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(methodDeclaration.DeclaringType);
				var resolvedReturnType = TypeTemplate.Resolve(methodDeclaration.ReturnType);
				var resolvedParameterTypes = methodDeclaration.GetParameters().Select(p => TypeTemplate.Resolve(p.ParameterType)).ToArray();
				var resolvedDeclaration = resolvedDeclaringType.GetMethods().OfSignature(resolvedReturnType, resolvedParameterTypes).Single();

				methodDeclaration = resolvedDeclaration;
			}

			return new[] { methodDeclaration };
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static PropertyInfo[] GetPropertyInfoArrayFromLambda(LambdaExpression lambda)
		{
			var propertyInfo = (PropertyInfo)((MemberExpression)lambda.Body).Member;
			return new[] { propertyInfo };
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IHappilOperand<T> OrNullConstant<T>(this IHappilOperand<T> operand)
		{
			if ( operand != null )
			{
				return operand;
			}

			var type = typeof(T);

			if ( type.IsValueType && !type.IsNullableValueType() )
			{
				throw new NotSupportedException(string.Format("Null is not a valid value for type '{0}'.", type.FullName));
			}

			return new HappilConstant<T>(default(T));
		}
	}
}
