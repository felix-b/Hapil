using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal class HappilUnaryExpression<TOperand, TExpr> : HappilExpression<TExpr>
	{
		private readonly IUnaryOperator<TOperand> m_Operator;
		private readonly IHappilOperand<TOperand> m_Operand;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilUnaryExpression(IUnaryOperator<TOperand> @operator, IHappilOperand<TOperand> operand)
		{
			m_Operand = operand;
			m_Operator = @operator;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Expr<{0}>{{{1} {2}}}", typeof(TExpr).Name, m_Operator.ToString(), m_Operand.ToString());
		}
	}
}
