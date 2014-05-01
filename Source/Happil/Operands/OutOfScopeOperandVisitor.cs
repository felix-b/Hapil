using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Operands
{
	internal class OutOfScopeOperandVisitor : OperandVisitorBase
	{
		private readonly HashSet<IOperand> m_OutOfScopeOperands = new HashSet<IOperand>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			m_OutOfScopeOperands.Add(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] GetOutOfScopeOperands()
		{
			return m_OutOfScopeOperands.ToArray();
		}
	}
}
