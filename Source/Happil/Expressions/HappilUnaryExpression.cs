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
		private readonly UnaryOperatorPosition m_Position;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilUnaryExpression(
			IUnaryOperator<TOperand> @operator, 
			IHappilOperand<TOperand> operand, 
			UnaryOperatorPosition position = UnaryOperatorPosition.Prefix)
		{
			m_Operand = operand;
			m_Operator = @operator;
			m_Position = position;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			var formatString = (
				m_Position == UnaryOperatorPosition.Prefix ?
				"Expr<{0}>{{{1} {2}}}" :
				"Expr<{0}>{{{2} {1}}}");

			return string.Format(formatString, typeof(TExpr).Name, m_Operator.ToString(), m_Operand.ToString());
		}
	}
}
