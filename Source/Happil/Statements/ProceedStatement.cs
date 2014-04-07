using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Writers;

namespace Happil.Statements
{
	internal class ProceedStatement : StatementBase
	{
		private readonly MethodMember m_OwnerMethod;
		private readonly List<StatementBase> m_DecoratedStatements;
		private readonly IMutableOperand m_ReturnValueLocal;
		private readonly LabelStatement m_LeaveLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ProceedStatement(MethodMember ownerMethod, MethodWriterBase[] decoratedWriters, IMutableOperand returnValueLocal)
		{
			m_OwnerMethod = ownerMethod;
			m_DecoratedStatements = new List<StatementBase>();
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

		public override void Emit(ILGenerator il)
		{
			foreach ( var statement in m_DecoratedStatements )
			{
				statement.Emit(il);
			}

			//il.MarkLabel(m_LeaveLabel);
			//il.Emit(OpCodes.Nop);
		}
	}
}
