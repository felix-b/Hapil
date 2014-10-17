using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Operands;

namespace Hapil.Expressions
{
	public interface IExpressionOperand : IOperand
	{
		bool ShouldLeaveValueOnStack { get; set; }
	}
}
