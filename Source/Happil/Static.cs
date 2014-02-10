﻿using System;
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
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2>(Action<TArg1, TArg2> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Void<TArg1, TArg2, TArg3>(
			Action<TArg1, TArg2, TArg3> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2, 
			IHappilOperand<TArg3> arg3)
		{
			var method = GetValidStaticMethod(member);
			StatementScope.Current.AddStatement(new CallStatement(null, method, arg1, arg2, arg3));
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
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1);
			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> Func<TArg1, TArg2, TReturn>(
			Func<TArg1, TArg2, TReturn> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method,arg1, arg2);

			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Func<TArg1, TArg2, TArg3, TReturn> member,
			IHappilOperand<TArg1> arg1,
			IHappilOperand<TArg2> arg2,
			IHappilOperand<TArg3> arg3)
		{
			var method = GetValidStaticMethod(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3);

			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<TReturn> GenericFunc<TArg1, TArg2, TArg3, TReturn>(
			Expression<Func<TArg1, TArg2, TArg3, TReturn>> member,
			IHappilOperand<TArg1> arg1,
			IHappilOperand<TArg2> arg2,
			IHappilOperand<TArg3> arg3)
		{
			var method = Helpers.ResolveMethodFromLambda(member);
			var @operator = new UnaryOperators.OperatorCall<object>(method, arg1, arg2, arg3);

			return new HappilUnaryExpression<object, TReturn>(null, @operator, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilAssignable<TProp> Prop<TProp>(Expression<Func<TProp>> propertyOrField)
		{
			var member = Helpers.GetMemberFromLambda(propertyOrField);

			if ( member is PropertyInfo )
			{
				var property = Helpers.ResolvePropertyFromLambda(propertyOrField);
				ValidateStaticMethod(property.GetGetMethod() ?? property.GetSetMethod(), "propertyLambda");

				return new PropertyAccessOperand<TProp>(
					target: null,
					property: property);
			}
			else if ( member is FieldInfo )
			{
				var field = Helpers.ResolveFieldFromLambda(propertyOrField);

				return new FieldAccessOperand<TProp>(
					target: null,
					field: field);
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
