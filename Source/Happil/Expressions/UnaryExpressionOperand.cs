using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	internal class HappilUnaryExpression<TOperand, TExpr> : ExpressionOperand<TExpr>
	{
		private readonly IUnaryOperator<TOperand> m_Operator;
		private readonly IOperand m_Operand;
		private readonly UnaryOperatorPosition m_Position;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilUnaryExpression(
			MethodMember ownerMethod,
			IUnaryOperator<TOperand> @operator, 
			IOperand<TOperand> operand, 
			UnaryOperatorPosition position = UnaryOperatorPosition.Prefix)
		{
			m_Operand = operand;
			m_Operator = @operator;
			m_Position = position;

			var scope = StatementScope.Current; 

			scope.Consume(operand as IExpressionOperand);
			scope.RegisterExpressionStatement(this);
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
			// expression has no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			var dontLeaveValueOnStack = (m_Operator as IDontLeaveValueOnStack);

			if ( ShouldLeaveValueOnStack )
			{
				EnsureOperandLeavesValueOnStack(m_Operand as IExpressionOperand);

				if ( dontLeaveValueOnStack != null )
				{
					dontLeaveValueOnStack.ForceLeaveFalueOnStack();
				}
			}

			m_Operator.Emit(il, (ExpressionOperand<TOperand>)m_Operand);

			if ( !ShouldLeaveValueOnStack && dontLeaveValueOnStack == null )
			{
				il.Emit(OpCodes.Pop);
			}
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
}
