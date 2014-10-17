using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Statements;

namespace Hapil.Operands
{
	internal interface IScopedOperand : IOperand
	{
		StatementBlock HomeStatementBlock { get; }
		string CaptureName { get; }
		bool ShouldInitializeHoistedField { get; }
	}
}
