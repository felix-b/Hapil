using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Operands;

namespace Happil.Expressions
{
	public interface IExpressionOperand : IOperand
	{
		bool ShouldLeaveValueOnStack { get; set; }
	}
}
