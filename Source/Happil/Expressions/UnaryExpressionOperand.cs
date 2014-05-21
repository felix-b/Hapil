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
	internal class UnaryExpressionOperand<TOperand, TExpr> : ExpressionOperand<TExpr>, IAcceptOperandVisitor
	{
		private readonly UnaryOperatorPosition m_Position;
		private readonly IUnaryOperator<TOperand> m_Operator;
		private IOperand m_Operand;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public UnaryExpressionOperand(
			IUnaryOperator<TOperand> @operator, 
			IOperand<TOperand> operand, 
			UnaryOperatorPosition position = UnaryOperatorPosition.Prefix)
		{
			m_Operand = operand;
			m_Operator = @operator;
			m_Position = position;

			if ( StatementScope.Exists )
			{
				var scope = StatementScope.Current;

				scope.Consume(operand as IExpressionOperand);
				scope.RegisterExpressionStatement(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Operand);
			visitor.VisitAcceptor(m_Operator as IAcceptOperandVisitor);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			var isCallExpression = (m_Operator is UnaryOperators.OperatorCall<TOperand>);
			var formatString = (
				(m_Position == UnaryOperatorPosition.Prefix && !isCallExpression) ?
				"[{0}{1}]" :
				"[{1}{0}]");

			return string.Format(formatString, m_Operator, m_Operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.UnaryExpression;
			}
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

			m_Operator.Emit(il, (IOperand<TOperand>)m_Operand);

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
