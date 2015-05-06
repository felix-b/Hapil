using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class ExpressionStatement : StatementBase
	{
		private IExpressionOperand m_Expression;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExpressionStatement(IExpressionOperand expression)
		{
			m_Expression = expression;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			m_Expression.ShouldLeaveValueOnStack = false;
			m_Expression.EmitTarget(il);
			m_Expression.EmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Expression);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return m_Expression.ToString();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IExpressionOperand Expression
		{
			get
			{
				return m_Expression;
			}
		}
	}
}
