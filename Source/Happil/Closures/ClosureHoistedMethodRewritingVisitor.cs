using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Closures
{
	internal class ClosureHoistedMethodRewritingVisitor : OperandVisitorBase
	{
		private readonly MethodMember m_HoistedMethod;
		private readonly ClosureDefinition m_HoistingClosure;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureHoistedMethodRewritingVisitor(MethodMember hoistedMethod, ClosureDefinition hoistingClosure)
		{
			m_HoistedMethod = hoistedMethod;
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
