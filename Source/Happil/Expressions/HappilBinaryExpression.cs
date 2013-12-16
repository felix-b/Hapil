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

			var scope = TryGetCurrrentScope();

			if ( scope != null )
			{
				scope.UnregisterExpressionStatement(left as IHappilExpression);
				scope.UnregisterExpressionStatement(right as IHappilExpression);
				// since the unregister method only checks the last statement, the following line is 
				// required to remove dependency on the order of left and right registration:
				scope.UnregisterExpressionStatement(left as IHappilExpression);

				scope.RegisterExpressionStatement(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Expr<{0}>{{{1} {2} {3}}}", typeof(TExpr).Name, m_Left.ToString(), m_Operator.ToString(), m_Right.ToString());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of HappilOperand<TExpr>

		internal override HappilClass OwnerClass
		{
			get
			{
				var ownerClass = base.OwnerClass;

				if ( ownerClass != null )
				{
					return ownerClass;
				}

				var leftOwnerClass = ((IHappilOperandInternals)m_Left).OwnerClass;
				var rightOwnerClass = ((IHappilOperandInternals)m_Right).OwnerClass;

				return (leftOwnerClass ?? rightOwnerClass);
			}
		}

		#endregion

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
				EnsureOperandLeavesValueOnStack(m_Left as IHappilExpression);
				EnsureOperandLeavesValueOnStack(m_Right as IHappilExpression);

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
