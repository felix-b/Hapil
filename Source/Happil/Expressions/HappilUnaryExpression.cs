using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
			HappilMethod ownerMethod,
			IUnaryOperator<TOperand> @operator, 
			IHappilOperand<TOperand> operand, 
			UnaryOperatorPosition position = UnaryOperatorPosition.Prefix)
			: base(ownerMethod)
		{
			m_Operand = operand;
			m_Operator = @operator;
			m_Position = position;

			if ( ownerMethod != null )
			{
				ownerMethod.UnregisterExpressionStatement(operand as IHappilExpression);
				ownerMethod.RegisterExpressionStatement(this);
			}
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotImplementedException();
		}
	}
}
