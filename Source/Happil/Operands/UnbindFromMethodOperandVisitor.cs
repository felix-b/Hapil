using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Operands
{
	internal class UnbindFromMethodOperandVisitor : OperandVisitorBase
	{
		protected override bool OnFilterOperand(IOperand operand)
		{
			return (operand is IBindToMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			var bindableOperand = (IBindToMethod)operand;

			if ( bindableOperand.IsBound )
			{
				bindableOperand.ResetBinding();
			}
		}
	}
}
