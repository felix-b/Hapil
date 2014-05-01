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
	internal class BinaryExpressionOperand<TLeft, TRight, TExpr> : ExpressionOperand<TExpr>, IAcceptOperandVisitor
	{
		private readonly IBinaryOperator<TLeft, TRight> m_Operator;
		private IOperand<TLeft> m_Left;
		private IOperand<TRight> m_Right;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public BinaryExpressionOperand(
			IBinaryOperator<TLeft, TRight> @operator, 
			IOperand<TLeft> left, 
			IOperand<TRight> right)
		{
			m_Left = left;
			m_Right = right;
			m_Operator = @operator;

			var scope = StatementScope.Current;

			scope.Consume(left as IExpressionOperand);
			scope.Consume(right as IExpressionOperand);
			// since the unregister method only checks the last statement, the following line is 
			// required to remove dependency on the order of left and right registration:
			scope.Consume(left as IExpressionOperand);
			
			scope.RegisterExpressionStatement(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Left);
			visitor.VisitOperand(ref m_Right);
			visitor.VisitAcceptor(m_Operator as IAcceptOperandVisitor); // some binary operators may contain other operands
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("[{0} {1} {2}]", m_Left, m_Operator, m_Right);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.BinaryExpression;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			var dontLeaveValueOnStack = (m_Operator as IDontLeaveValueOnStack);

			if ( ShouldLeaveValueOnStack )
			{
				EnsureOperandLeavesValueOnStack(m_Left as IExpressionOperand);
				EnsureOperandLeavesValueOnStack(m_Right as IExpressionOperand);

				if ( dontLeaveValueOnStack != null )
				{
					dontLeaveValueOnStack.ForceLeaveFalueOnStack();
				}
			}

			m_Operator.Emit(il, m_Left, m_Right);

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

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class BinaryExpressionOperand<TOperand, TExpr> : BinaryExpressionOperand<TOperand, TOperand, TExpr>
	{
		public BinaryExpressionOperand(
			IBinaryOperator<TOperand, TOperand> @operator, 
			IOperand<TOperand> left, 
			IOperand<TOperand> right)
			: base(@operator, left, right)
		{
		}
	}
}
