using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal abstract class StatementBase
	{
		public abstract void Emit(ILGenerator il);
		public abstract void AcceptVisitor(OperandVisitorBase visitor);
	}
}
