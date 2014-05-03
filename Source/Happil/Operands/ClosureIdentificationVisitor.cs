using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Operands
{
	internal class ClosureIdentificationVisitor : OperandVisitorBase
	{
		private readonly MethodMember m_Method;
		private readonly HashSet<IOperand> m_Externals = new HashSet<IOperand>();
		private readonly HashSet<OperandCapture> m_Captures = new HashSet<OperandCapture>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureIdentificationVisitor(MethodMember method)
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

			if ( operandHome != null )
			{
				if ( operandHome.OwnerMethod != m_Method )
				{
					m_Externals.Add(operand);
					m_Captures.Add(new OperandCapture(operand, operandHome));
				}
			}
			else
			{
				m_Captures.Add(new OperandCapture(operand, sourceOperandHome: null));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] Externals
		{
			get
			{
				return m_Externals.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public OperandCapture[] Captures
		{
			get
			{
				return m_Captures.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsClosureRequired
		{
			get
			{
				return (m_Externals.Count > 0);
			}
		}
	}
}
