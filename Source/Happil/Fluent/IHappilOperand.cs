using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Happil.Fluent
{
	/// <summary>
	/// A type-agnostic common base for all kinds of operands.
	/// </summary>
	public interface IHappilOperand
	{
		/// <summary>
		/// Gets CLR type of the operand value.
		/// </summary>
		Type OperandType { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

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
	public interface IHappilOperand<out T> : IHappilOperand
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TCast"></typeparam>
		/// <returns></returns>
		HappilOperand<TCast> CastTo<TCast>();
	}
}
