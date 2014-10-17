using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Hapil.Expressions;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil
{
	public static class Static
	{
		public static void Void(Action member)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1>(Action<TArg1> member, IOperand<TArg1> arg1)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2>(Action<TArg1, TArg2> member, IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3>(
			Action<TArg1, TArg2, TArg3> member, 
			IOperand<TArg1> arg1, 
			IOperand<TArg2> arg2, 
			IOperand<TArg3> arg3)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3, TArg4>(
			Action<TArg1, TArg2, TArg3, TArg4> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3, arg4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3, TArg4, TArg5>(
			Action<TArg1, TArg2, TArg3, TArg4, TArg5> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3, arg4, arg5));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
			Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3, arg4, arg5, arg6));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
			Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6,
			IOperand<TArg7> arg7)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(
			Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6,
			IOperand<TArg7> arg7,
			IOperand<TArg8> arg8)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void(MethodInfo method, params IOperand[] arguments)
		{
			StatementScope.Current.AddStatement(new CallStatement(null, method, arguments));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TReturn>(Func<TReturn> member)
		{
			var method = GetValidStaticMethod(member);
			return new UnaryExpressionOperand<object, TReturn>(new UnaryOperators.OperatorCall<object>(method), null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TReturn>(Func<TArg1, TReturn> member, IOperand<TArg1> arg1)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1);
			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TReturn>(
			Func<TArg1, TArg2, TReturn> member, 
			IOperand<TArg1> arg1, 
			IOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method,arg1, arg2);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Func<TArg1, TArg2, TArg3, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TArg4, TReturn>(
			Func<TArg1, TArg2, TArg3, TArg4, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3, arg4);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TArg4, TArg5, TReturn>(
			Func<TArg1, TArg2, TArg3, TArg4, TArg5, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3, arg4, arg5);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(
			Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3, arg4, arg5, arg6);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>(
			Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6,
			IOperand<TArg7> arg7)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>(
			Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3,
			IOperand<TArg4> arg4,
			IOperand<TArg5> arg5,
			IOperand<TArg6> arg6,
			IOperand<TArg7> arg7,
			IOperand<TArg8> arg8)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> Func<TReturn>(MethodInfo method, params IOperand[] arguments)
		{
			var @operator = new UnaryOperators.OperatorCall<object>(method, arguments);
			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<TReturn> GenericFunc<TArg1, TArg2, TArg3, TReturn>(
			Expression<Func<TArg1, TArg2, TArg3, TReturn>> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = Helpers.ResolveMethodFromLambda(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3);

			return new UnaryExpressionOperand<object, TReturn>(@operator, operand: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MutableOperand<TProp> Prop<TProp>(Expression<Func<TProp>> propertyOrField)
		{
			var member = Helpers.GetMemberFromLambda(propertyOrField);

			if ( member is PropertyInfo )
			{
				var property = Helpers.ResolvePropertyFromLambda(propertyOrField);
				ValidateStaticMethod(property.GetGetMethod() ?? property.GetSetMethod(), "propertyLambda");

				return new Property<TProp>(
					target: null,
					property: property);
			}
			else if ( member is FieldInfo )
			{
				var field = Helpers.ResolveFieldFromLambda(propertyOrField);

				return new Field<TProp>(
					target: null,
					fieldInfo: field);
			}
			else
			{
				throw new ArgumentException("Specified lamba expression is not supported.");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo GetValidStaticMethod(Delegate memberDelegate)
		{
			return ValidateStaticMethod(memberDelegate.Method, parameterName: "member");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo ValidateStaticMethod(MethodInfo method, string parameterName)
		{
			if ( !method.IsStatic )
			{
				var typeNamePrefix = (method.DeclaringType != null ? method.DeclaringType.Name + "." : "");
				throw new ArgumentException(string.Format("Method is not static: {0}{1}", typeNamePrefix, method.Name), parameterName);
			}

			return method;
		}
	}
}
