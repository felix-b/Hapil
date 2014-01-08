using System;
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
	public static class Static
	{
		public static void Void(Action member)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1>(Action<TArg1> member, IHappilOperand<TArg1> arg1)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, (IHappilOperandInternals)arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2>(Action<TArg1, TArg2> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(
				null,
				method,
				(IHappilOperandInternals)arg1,
				(IHappilOperandInternals)arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3>(
			Action<TArg1, TArg2, TArg3> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2, 
			IHappilOperand<TArg3> arg3)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(
				null,
				method,
				(IHappilOperandInternals)arg1,
				(IHappilOperandInternals)arg2,
				(IHappilOperandInternals)arg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> Func<TReturn>(Func<TReturn> member)
		{
			var method = GetValidStaticMethod(member);
			return new HappilUnaryExpression<object, TReturn>(null, new UnaryOperators.OperatorCall<object>(method), null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> Func<TArg1, TReturn>(Func<TArg1, TReturn> member, IHappilOperand<TArg1> arg1)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, (IHappilOperandInternals)arg1);
			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> Func<TArg1, TArg2, TReturn>(
			Func<TArg1, TArg2, TReturn> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(
				method,
				(IHappilOperandInternals)arg1,
				(IHappilOperandInternals)arg2);

			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<TProp> Prop<TProp>(Expression<Func<TProp>> propertyLambda)
		{
			var property = Helpers.GetPropertyInfoArrayFromLambda(propertyLambda).First();
			ValidateStaticMethod(property.GetGetMethod() ?? property.GetSetMethod(), "propertyLambda");

			return new PropertyAccessOperand<TProp>(
				target: null,
				property: property);
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
