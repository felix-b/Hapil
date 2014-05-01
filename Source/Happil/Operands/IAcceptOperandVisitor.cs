using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Operands
{
	internal interface IAcceptOperandVisitor
	{
		void AcceptVisitor(OperandVisitorBase visitor);
	}
}
