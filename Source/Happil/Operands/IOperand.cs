using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Operands
{
	public interface IOperand
	{
		IOperand<T> CastTo<T>();
		Type OperandType { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IOperand<out T> : IOperand
	{
	}
}
