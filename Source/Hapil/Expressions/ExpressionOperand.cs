using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Expressions
{
	internal abstract class ExpressionOperand<T> : Operand<T>, IExpressionOperand
	{
		internal ExpressionOperand()
		{
			ShouldLeaveValueOnStack = true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilExpression Members

		public bool ShouldLeaveValueOnStack { get; set; }

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void EnsureOperandLeavesValueOnStack(IExpressionOperand expressionOperand)
		{
			if ( expressionOperand != null )
			{
				expressionOperand.ShouldLeaveValueOnStack = true;
			}
		}
	}
}