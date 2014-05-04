using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Statements;

namespace Happil.Operands
{
	internal interface IScopedOperand
	{
		StatementBlock HomeStatementBlock { get; }
		string CaptureName { get; }
	}
}
