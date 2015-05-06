using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Writers;

namespace Hapil.Statements
{
	internal class ProceedStatement : StatementBase
	{
		private readonly MethodMember m_OwnerMethod;
		private readonly StatementBlock m_DecoratedStatements;
		private readonly LabelStatement m_LeaveLabel;
		private IMutableOperand m_ReturnValueLocal;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ProceedStatement(MethodMember ownerMethod, MethodWriterBase[] decoratedWriters, IMutableOperand returnValueLocal)
		{
			m_OwnerMethod = ownerMethod;
			m_DecoratedStatements = new StatementBlock();
			m_ReturnValueLocal = returnValueLocal;

			using ( var scope = new StatementScope(m_DecoratedStatements/*, exceptionStatement: null, blockType: ExceptionBlockType.None*/) )
			{
				m_LeaveLabel = scope.DefineLabel();

				foreach ( var writer in decoratedWriters )
				{
					writer.SetupDecoratedMode(returnValueLocal, m_LeaveLabel);
					writer.Flush();
				}

				m_LeaveLabel.MarkLabel();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			foreach ( var statement in m_DecoratedStatements )
			{
				statement.Emit(il, ownerMethod);
			}

			//il.MarkLabel(m_LeaveLabel);
			//il.Emit(OpCodes.Nop);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_ReturnValueLocal);
			visitor.VisitStatementBlock(m_DecoratedStatements);
		}
	}
}
