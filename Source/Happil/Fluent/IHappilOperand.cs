using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Happil.Fluent
{
	/// <summary>
	/// This is the common denominator of all different kinds of operands in Happil.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the operand. 
	/// </typeparam>
	/// <remarks>
	/// Assigning operand a type allows protecting type safety of the generated code at compile time. 
	/// The covariance of <typeparamref name="T"/> allows passing operand of a more specific type wherever an operand
	/// of a more general type is expected, up the inheritance hierarchy.
	/// </remarks>
	public interface IHappilOperand<out T>
	{
		//void Invoke(Func<T, Action> member);

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//void Invoke<TArg1>(Func<T, Action<TArg1>> member, IHappilOperand<TArg1> arg1);

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//void Invoke<TArg1, TArg2>(Func<T, Action<TArg1, TArg2>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2);

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//void Invoke<TArg1, TArg2, TArg3>(Func<T, Action<TArg1, TArg2, TArg3>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3);
	}
}
