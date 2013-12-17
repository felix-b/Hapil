using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilMethodBodyBase
	{
		HappilLocal<T> Local<T>(string name);
		HappilLocal<T> Local<T>(string name, HappilOperand<T> initialValue);
		void Throw<TException>(string message) where TException : Exception;
		void EmitFromLambda(Expression<Action> lambda);
		HappilArgument<T> Argument<T>(string name);
		HappilArgument<T> Argument<T>(int index);
		MethodInfo MethodInfo { get; }
		int ArgumentCount { get; }
		Type ReturnType { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilMethodBody : IHappilMethodBodyBase
	{
		void Return(IHappilOperand<object> operand);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilMethodBody<TReturn> : IHappilMethodBodyBase
	{
		void Return(IHappilOperand<TReturn> operand);
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
