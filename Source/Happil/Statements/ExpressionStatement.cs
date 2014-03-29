using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;

namespace Happil.Statements
{
	internal class ExpressionStatement : StatementBase
	{
		private readonly IExpressionOperand m_Expression;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExpressionStatement(IExpressionOperand expression)
		{
			m_Expression = expression;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			m_Expression.ShouldLeaveValueOnStack = false;
			m_Expression.EmitTarget(il);
			m_Expression.EmitLoad(il);
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
