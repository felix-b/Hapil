using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Operands;

namespace Happil.Expressions
{
	internal interface INonPostfixNotation
	{
		IOperand RightSide { set; }
	}
}
