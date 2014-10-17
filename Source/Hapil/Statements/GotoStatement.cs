using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class GotoStatement : StatementBase, ILeaveStatement
	{
		private readonly LabelStatement m_LabelStatement;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public GotoStatement(LabelStatement labelStatement)
		{
			m_LabelStatement = labelStatement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			il.Emit(OpCodes.Br, m_LabelStatement.Label);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return m_LabelStatement.HomeScope;
			}
		}
	}
}
