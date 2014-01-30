﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
		private static readonly Dictionary<Type, OpCode> s_ValueTypeCastInstructions = new Dictionary<Type, OpCode>() {
				{ typeof(sbyte), OpCodes.Conv_I1 },
				{ typeof(short), OpCodes.Conv_I2 },
				{ typeof(int), OpCodes.Conv_I4 },
				{ typeof(long), OpCodes.Conv_I8 },
				{ typeof(byte), OpCodes.Conv_U1 },
				{ typeof(ushort), OpCodes.Conv_U2 },
				{ typeof(uint), OpCodes.Conv_U4 },
				{ typeof(ulong), OpCodes.Conv_U8 },
				{ typeof(float), OpCodes.Conv_R4 },
				{ typeof(double), OpCodes.Conv_R8 }
			};

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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
			MethodInfo methodDeclaration;

			if ( lambda.Body is UnaryExpression )
			{
				var createDelegateCall = (MethodCallExpression)(((UnaryExpression)lambda.Body).Operand);
				methodDeclaration = (MethodInfo)((ConstantExpression)createDelegateCall.Arguments[2]).Value;
			}
			else if ( lambda.Body is LambdaExpression )
			{
				/*
				((LambdaExpression)lambda.Body).Body as MethodCallExpression
				{x.One(s1, s2)}
					Arguments: Count = 2
					CanReduce: false
					DebugView: ".Call $x.One(\r\n    $s1,\r\n    $s2)"
					Method: {System.String One(System.String ByRef, System.String ByRef)}
					NodeType: Call
					Object: {x}
				*/

				methodDeclaration = ((MethodCallExpression)((LambdaExpression)lambda.Body).Body).Method;
			}
			else
			{
				throw new NotSupportedException("Specified lambda expression cannot be converted into method declaration.");
			}

			if ( TypeTemplate.IsTemplateType(methodDeclaration.DeclaringType) )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(methodDeclaration.DeclaringType);
				var resolvedReturnType = TypeTemplate.Resolve(methodDeclaration.ReturnType);
				var resolvedParameterTypes = methodDeclaration.GetParameters().Select(p => TypeTemplate.Resolve(p.ParameterType)).ToArray();
				var resolvedDeclaration = ImplementableMembers.Of(resolvedDeclaringType)
					.Methods.Where(m => m.Name == methodDeclaration.Name)
					.OfSignature(resolvedReturnType, resolvedParameterTypes)
					.Single();

				return new[] { resolvedDeclaration };
			}
			else
			{
				return new[] { methodDeclaration };
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static PropertyInfo[] GetPropertyInfoArrayFromLambda(LambdaExpression lambda)
		{
			var propertyInfo = (PropertyInfo)((MemberExpression)lambda.Body).Member;

			if ( TypeTemplate.IsTemplateType(propertyInfo.DeclaringType) )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(propertyInfo.DeclaringType);
				var resolvedPropertyType = TypeTemplate.Resolve(propertyInfo.PropertyType);
				var resolvedParameterTypes = propertyInfo.GetIndexParameters().Select(p => TypeTemplate.Resolve(p.ParameterType)).ToArray();
				var resolvedPropertyInfo = ImplementableMembers.Of(resolvedDeclaringType)
					.Properties.Where(p => p.Name == propertyInfo.Name)
					.OfSignature(resolvedPropertyType, resolvedParameterTypes)
					.Single();

				return new[] { resolvedPropertyInfo };
			}
			else
			{
				return new[] { propertyInfo };
			}
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void EmitConvertible(ILGenerator il, IConvertible convertible)
		{
			var formatProvider = CultureInfo.CurrentCulture;

			switch ( convertible.GetTypeCode() )
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
					il.Emit(OpCodes.Ldc_I4, convertible.ToInt32(formatProvider));
					break;
				case TypeCode.UInt32:
					il.Emit(OpCodes.Ldc_I4, convertible.ToUInt32(formatProvider));
					break;
				case TypeCode.Int64:
					il.Emit(OpCodes.Ldc_I8, convertible.ToInt64(formatProvider));
					break;
				case TypeCode.UInt64:
					il.Emit(OpCodes.Ldc_I8, convertible.ToUInt64(formatProvider));
					break;
				case TypeCode.Single:
					il.Emit(OpCodes.Ldc_R8, convertible.ToSingle(formatProvider));
					break;
				case TypeCode.Double:
					il.Emit(OpCodes.Ldc_R8, convertible.ToDouble(formatProvider));
					break;
				case TypeCode.String:
					il.Emit(OpCodes.Ldstr, convertible.ToString());
					break;
				default:
					throw CreateConstantNotSupportedException(convertible.GetType());
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static void EmitCast(ILGenerator il, Type fromType, Type toType)
		{

			if ( fromType.IsValueType )
			{
				EmitValueTypeConversion(il, fromType, toType);
			}
			else if ( !toType.IsValueType )
			{
				il.Emit(OpCodes.Castclass, toType);
			}
			else
			{
				throw NewConversionNotSupportedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Exception CreateConstantNotSupportedException(Type type)
		{
			return new NotSupportedException(string.Format(
				"Constants of type '{0}' are not supported.",
				type.FullName));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void EmitValueTypeConversion(ILGenerator il, Type fromType, Type toType)
		{
			if ( toType == typeof(object) )
			{
				il.Emit(OpCodes.Box, fromType);
				return;
			}

			if ( !toType.IsValueType )
			{
				throw NewConversionNotSupportedException();
			}

			var conversionType = (toType.IsEnum ? Enum.GetUnderlyingType(toType) : toType);

			if ( fromType != conversionType )
			{
				OpCode conversionInstruction;

				if ( !s_ValueTypeCastInstructions.TryGetValue(conversionType, out conversionInstruction) )
				{
					throw new NotSupportedException("Casting to specified value type is not supported.");
				}

				il.Emit(conversionInstruction);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static Exception NewConversionNotSupportedException()
		{
			return new NotSupportedException("Specified type conversion is not supported.");
		}
	}
}
