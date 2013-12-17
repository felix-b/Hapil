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
		private readonly IHappilOperandInternals m_Operand;
		private readonly UnaryOperatorPosition m_Position;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilUnaryExpression(
			HappilMethod ownerMethod,
			IUnaryOperator<TOperand> @operator, 
			IHappilOperand<TOperand> operand, 
			UnaryOperatorPosition position = UnaryOperatorPosition.Prefix)
			: base(ownerMethod)
		{
			m_Operand = (IHappilOperandInternals)operand;
			m_Operator = @operator;
			m_Position = position;

			var scope = TryGetCurrrentScope();

			if ( scope != null )
			{
				scope.UnregisterExpressionStatement(operand as IHappilExpression);
				scope.RegisterExpressionStatement(this);
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

		#region Overrides of HappilOperand<TExpr>

		internal override HappilClass OwnerClass
		{
			get
			{
				return (base.OwnerClass ?? m_Operand.OwnerClass);
			}
		}

		#endregion

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
				EnsureOperandLeavesValueOnStack(m_Operand as IHappilExpression);

				if ( dontLeaveValueOnStack != null )
				{
					dontLeaveValueOnStack.ForceLeaveFalueOnStack();
				}
			}

			m_Operator.Emit(il, (IHappilOperand<TOperand>)m_Operand);

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
