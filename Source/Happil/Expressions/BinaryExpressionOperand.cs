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
	internal class BinaryExpressionOperand<TLeft, TRight, TExpr> : ExpressionOperand<TExpr>
	{
		private readonly IBinaryOperator<TLeft, TRight> m_Operator;
		private readonly IOperand<TLeft> m_Left;
		private readonly IOperand<TRight> m_Right;

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

		public override string ToString()
		{
			return string.Format("Expr<{0}>{{{1} {2} {3}}}", typeof(TExpr).Name, m_Left.ToString(), m_Operator.ToString(), m_Right.ToString());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign?
		//#region Overrides of HappilOperand<TExpr>

		//internal override ClassType OwnerClass
		//{
		//	get
		//	{
		//		var ownerClass = base.OwnerClass;

		//		if ( ownerClass != null )
		//		{
		//			return ownerClass;
		//		}

		//		var leftOwnerClass = m_Left.GetOwnerClass();
		//		var rightOwnerClass = m_Right.GetOwnerClass();

		//		return (leftOwnerClass ?? rightOwnerClass);
		//	}
		//}

		//#endregion

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
