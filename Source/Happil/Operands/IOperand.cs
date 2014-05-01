using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Operands
{
	public interface IOperand
	{
		Operand<T> CastTo<T>();
		OperandKind Kind { get; }
		Type OperandType { get; }
		bool HasTarget { get; }
		bool IsMutable { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IOperand<out T> : IOperand
	{
	}
}
