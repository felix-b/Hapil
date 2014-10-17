using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Operands
{
	internal interface IAcceptOperandVisitor
	{
		void AcceptVisitor(OperandVisitorBase visitor);
	}
}
