using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal class HappilBinaryExpression<TLeft, TRight, TExpr> : HappilExpression<TExpr>
	{
		private readonly IBinaryOperator<TLeft, TRight> m_Operator;
		private readonly IHappilOperand<TLeft> m_Left;
		private readonly IHappilOperand<TRight> m_Right;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilBinaryExpression(
			HappilMethod ownerMethod, 
			IBinaryOperator<TLeft, TRight> @operator, 
			IHappilOperand<TLeft> left, 
			IHappilOperand<TRight> right)
			: base(ownerMethod)
		{
			m_Left = left;
			m_Right = right;
			m_Operator = @operator;

			if ( ownerMethod != null )
			{
				ownerMethod.UnregisterExpressionStatement(left as IHappilExpression);
				ownerMethod.UnregisterExpressionStatement(right as IHappilExpression);
				ownerMethod.UnregisterExpressionStatement(left as IHappilExpression); // do not depend on the order of left and right registration
				
				ownerMethod.RegisterExpressionStatement(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Expr<{0}>{{{1} {2} {3}}}", typeof(TExpr).Name, m_Left.ToString(), m_Operator.ToString(), m_Right.ToString());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			m_Operator.Emit(il, m_Left, m_Right);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class HappilBinaryExpression<TOperand, TExpr> : HappilBinaryExpression<TOperand, TOperand, TExpr>
	{
		public HappilBinaryExpression(
			HappilMethod ownerMethod,
			IBinaryOperator<TOperand, TOperand> @operator, 
			IHappilOperand<TOperand> left, 
			IHappilOperand<TOperand> right)
			: base(ownerMethod, @operator, left, right)
		{
		}
	}
}
