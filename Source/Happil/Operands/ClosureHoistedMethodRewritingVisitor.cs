using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Statements;

namespace Happil.Operands
{
	internal class ClosureHoistedMethodRewritingVisitor : OperandVisitorBase
	{
		private readonly ClosureDefinition m_HoistingClosure;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureHoistedMethodRewritingVisitor(ClosureDefinition hoistingClosure)
		{
			m_HoistingClosure = hoistingClosure;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool OnFilterOperand(IOperand operand)
		{
			return (operand is IScopedOperand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			m_HoistingClosure.RewriteOperandIfCaptured(
				ref operand,
				closureInstanceReference: new ThisOperand<object>(m_HoistingClosure.ClosureClass));
		}
	}
}
