using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Operands;

namespace Happil.Expressions
{
	internal interface IValueTypeInitializer
	{
		IOperand Target { set; }
	}
}
