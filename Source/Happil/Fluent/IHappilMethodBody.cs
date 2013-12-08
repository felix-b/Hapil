using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilMethodBody
	{
		HappilLocal<T> Local<T>(string name);
		HappilLocal<T> Local<T>(string name, HappilOperand<T> initialValue);
		void Return<T>(HappilOperand<T> operand);
		void Throw<TException>(string message) where TException : Exception;
		void EmitByExample(Expression<Action> action);
		HappilArgument<T> Argument<T>(string name);
		HappilArgument<T> Argument<T>(int index);
		MethodInfo MethodInfo { get; }
		int ArgumentCount { get; }
		Type ReturnValue { get; }
	}
}
