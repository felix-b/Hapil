using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Expressions;
using Happil.Statements;

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

		public static void EmitCall(ILGenerator il, IOperand target, MethodBase method, params IOperand[] arguments)
		{
			if ( target != null )
			{
				EmitCallTarget(il, target);
			}

			var methodParameters = (method is MethodBuilder || method is ConstructorBuilder ? null : method.GetParameters());

			for ( int i = 0 ; i < arguments.Length ; i++ )
			{
				arguments[i].EmitTarget(il);

				if ( methodParameters != null && methodParameters[i].ParameterType.IsByRef )
				{
					arguments[i].EmitAddress(il);
				}
				else
				{
					arguments[i].EmitLoad(il);
				}
			}

			var methodInfo = (method as MethodInfo);
			var constructorInfo = (method as ConstructorInfo);
			var callOpcode = (
				method.IsVirtual || method.DeclaringType.IsInterface || (target != null && target.OperandType.IsValueType) ? 
				OpCodes.Callvirt : 
				OpCodes.Call);

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

		public static IOperand CreateConstant(Type type, object value)
		{
			var constantType = typeof(Constant<>).MakeGenericType(type);
			return (IOperand)Activator.CreateInstance(constantType, new object[] { value });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand GetLambdaArgumentAsConstant(Expression argument)
		{
			var argumentLambda = Expression.Lambda<Func<object>>(argument);
			var argumentValueFunc = argumentLambda.Compile();
			var argumentValue = argumentValueFunc();

			return CreateConstant(argument.Type, argumentValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodInfo ResolveMethodFromLambda(LambdaExpression lambda)
		{
			var originalDeclaration = ExtractMethodInfoFromLambda(lambda);
			var shouldResolveDeclaringType = TypeTemplate.IsTemplateType(originalDeclaration.DeclaringType);

			if ( !originalDeclaration.IsGenericMethod && !shouldResolveDeclaringType )
			{
				return originalDeclaration;
			}

			var resolvedDeclaration = originalDeclaration;

			if ( shouldResolveDeclaringType )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(originalDeclaration.DeclaringType);
				var resolvedReturnType = TypeTemplate.Resolve(originalDeclaration.ReturnType);
				var resolvedParameterTypes = originalDeclaration.GetParameters().Select(p => TypeTemplate.Resolve(p.ParameterType)).ToArray();

				resolvedDeclaration = TypeMemberCache.Of(resolvedDeclaringType)
					.Methods.Where(m => m.Name == originalDeclaration.Name)
					.OfSignature(resolvedReturnType, resolvedParameterTypes)
					.Single();
			}

			if ( resolvedDeclaration.IsGenericMethod )
			{
				var resolvedGenericArguments = originalDeclaration.GetGenericArguments().Select(TypeTemplate.Resolve).ToArray();
				resolvedDeclaration = resolvedDeclaration.GetGenericMethodDefinition().MakeGenericMethod(resolvedGenericArguments);
			}

			return resolvedDeclaration;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodInfo ExtractMethodInfoFromLambda(LambdaExpression lambda)
		{
			MethodInfo methodDeclaration;

			if ( lambda.Body is UnaryExpression )
			{
				var createDelegateCall = (MethodCallExpression)(((UnaryExpression)lambda.Body).Operand);

				if ( createDelegateCall.Object != null )
				{
					methodDeclaration = (MethodInfo)((ConstantExpression)(createDelegateCall.Object)).Value; // works for .NET 4.5
				}
				else
				{
					methodDeclaration = (MethodInfo)((ConstantExpression)createDelegateCall.Arguments[2]).Value; // works for .NET 4.0
				}
			}
			else if ( lambda.Body is LambdaExpression )
			{
				methodDeclaration = ((MethodCallExpression)((LambdaExpression)lambda.Body).Body).Method;
			}
			else if ( lambda.Body is MethodCallExpression )
			{
				methodDeclaration = ((MethodCallExpression)lambda.Body).Method;
			}
			else
			{
				throw new NotSupportedException("Specified lambda expression cannot be converted into method declaration.");
			}

			return methodDeclaration;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodInfo[] GetMethodInfoArrayFromLambda(LambdaExpression lambda)
		{
			return new[] { ResolveMethodFromLambda(lambda) };
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static PropertyInfo[] GetPropertyInfoArrayFromLambda(LambdaExpression lambda)
		{
			return new[] { ResolvePropertyFromLambda(lambda) };
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MemberInfo GetMemberFromLambda(LambdaExpression lambda)
		{
			return ((MemberExpression)lambda.Body).Member;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static FieldInfo ResolveFieldFromLambda(LambdaExpression lambda)
		{
			var originalFieldInfo = (FieldInfo)((MemberExpression)lambda.Body).Member;

			if ( TypeTemplate.IsTemplateType(originalFieldInfo.DeclaringType) )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(originalFieldInfo.DeclaringType);
				return TypeMemberCache.Of(resolvedDeclaringType).Fields.Single(m => m.Name == originalFieldInfo.Name);
			}
			else
			{
				return originalFieldInfo;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static PropertyInfo ResolvePropertyFromLambda(LambdaExpression lambda)
		{
			var propertyInfo = (PropertyInfo)((MemberExpression)lambda.Body).Member;

			if ( TypeTemplate.IsTemplateType(propertyInfo.DeclaringType) )
			{
				var resolvedDeclaringType = TypeTemplate.Resolve(propertyInfo.DeclaringType);
				var resolvedPropertyType = TypeTemplate.Resolve(propertyInfo.PropertyType);
				var resolvedParameterTypes = propertyInfo.GetIndexParameters().Select(p => TypeTemplate.Resolve(p.ParameterType)).ToArray();
				var resolvedPropertyInfo = TypeMemberCache.Of(resolvedDeclaringType)
					.Properties.Where(p => p.Name == propertyInfo.Name)
					.OfSignature(resolvedPropertyType, resolvedParameterTypes)
					.Single();

				return resolvedPropertyInfo;
			}
			else
			{
				return propertyInfo;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodInfo GetMethodInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
		{
			return ((MethodCallExpression)lambda.Body).Method;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static PropertyInfo GetPropertyInfo<TLambda>(TLambda lambda) where TLambda : LambdaExpression
		{
			return (PropertyInfo)((MemberExpression)lambda.Body).Member;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IOperand<T> OrNullConstant<T>(this IOperand<T> operand)
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

			return new Constant<T>(default(T));
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
			else if ( !toType.IsValueType || toType.IsNullableValueType() )
			{
				if ( !toType.IsAssignableFrom(fromType) )
				{
					il.Emit(OpCodes.Castclass, toType);
				}

				if ( toType.IsNullableValueType() )
				{
					il.Emit(OpCodes.Unbox_Any, toType);
				}
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

		public static Local<T[]> BuildArrayLocal<T>(params T[] constantValues)
		{
			var writer = StatementScope.Current.Writer;
			var arrayLocal = writer.Local<T[]>(initialValue: writer.NewArray<T>(new Constant<int>(constantValues.Length)));

			for ( int i = 0 ; i < constantValues.Length ; i++ )
			{
				arrayLocal.ElementAt(new Constant<int>(i)).Assign(new Constant<T>(constantValues[i]));
			}

			return arrayLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Local<T[]> BuildArrayLocal<T>(params IOperand<T>[] values)
		{
			var method = StatementScope.Current.Writer;
			var arrayLocal = method.Local<T[]>(initialValue: method.NewArray<T>(new Constant<int>(values.Length)));

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				arrayLocal.ElementAt(new Constant<int>(i)).Assign(values[i]);
			}

			return arrayLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void EmitCallTarget(ILGenerator il, IOperand target)
		{
			target.EmitTarget(il);

			if ( target.OperandType.IsValueType )
			{
				if ( target is ICanEmitAddress )
				{
					target.EmitAddress(il);
				}
				else
				{
					target.EmitLoad(il);

					var temp = il.DeclareLocal(target.OperandType);
					il.Emit(OpCodes.Stloc, temp);
					il.Emit(OpCodes.Ldloca, (short)temp.LocalIndex);
				}

				il.Emit(OpCodes.Constrained, target.OperandType);
			}
			else
			{
				target.EmitLoad(il);
			}
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
