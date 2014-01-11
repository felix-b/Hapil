using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Selectors;
using Happil.Statements;

namespace Happil.Fluent
{
	public interface IHappilMethodBodyBase
	{
		IHappilIfBody If(IHappilOperand<bool> condition);
		IHappilWhileSyntax While(IHappilOperand<bool> condition);
		HappilOperand<TBase> This<TBase>();
		HappilLocal<T> Local<T>();
		HappilLocal<T> Local<T>(IHappilOperand<T> initialValue);
		HappilLocal<T> Local<T>(T initialValueConst);
		HappilConstant<T> Const<T>(T value);
		HappilConstant<T> Default<T>();
		HappilConstant<object> Default(Type type);
		void Throw<TException>(string message) where TException : Exception;
		void EmitFromLambda(Expression<Action> lambda);
		HappilArgument<T> Argument<T>(string name);
		HappilArgument<T> Argument<T>(int index);
		MethodInfo MethodInfo { get; }
		int ArgumentCount { get; }
		Type ReturnType { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilMethodBodyTemplate : IHappilMethodBodyBase
	{
		void Return(IHappilOperand<TypeTemplate> operand);
		void Return();
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilMethodBody<TReturn> : IHappilMethodBodyBase
	{
		void Return(IHappilOperand<TReturn> operand);
		void ReturnConst(TReturn constantValue);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IVoidHappilMethodBody : IHappilMethodBodyBase
	{
		void Return();
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilConstructorBody : IHappilMethodBodyBase
	{
		void Base();
		void Base<TArg1>(IHappilOperand<TArg1> arg1);
		void Base<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2);
		void Base<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3);
		void This();
		void This<TArg1>(IHappilOperand<TArg1> arg1);
		void This<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2);
		void This<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3);
	}
}
