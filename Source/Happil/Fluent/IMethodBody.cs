using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IMethodBody
	{
		IStatement Return(IOperand operand);
		IOperand Argument(string name);
		IOperand Argument(int index);
		int ArgumentCount { get; }
		Type ReturnValue { get; }
	}
}
