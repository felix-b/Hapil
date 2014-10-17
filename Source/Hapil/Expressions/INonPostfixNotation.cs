using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Operands;

namespace Hapil.Expressions
{
	internal interface INonPostfixNotation
	{
		IOperand RightSide { set; }
	}
}
