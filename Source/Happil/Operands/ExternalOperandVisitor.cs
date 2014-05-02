using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Operands
{
	internal class ExternalOperandVisitor : OperandVisitorBase
	{
		private readonly MethodMember m_Method;
		private readonly HashSet<IOperand> m_ExternalOperands = new HashSet<IOperand>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExternalOperandVisitor(MethodMember method)
		{
			m_Method = method;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool OnFilterOperand(IOperand operand)
		{
			return (operand is IScopedOperand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			var operandHome = ((IScopedOperand)operand).HomeStatementBlock;

			if ( operandHome.OwnerMethod != m_Method )
			{
				m_ExternalOperands.Add(operand);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] GetExternalOperands()
		{
			return m_ExternalOperands.ToArray();
		}
	}
}
