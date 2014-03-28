using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	internal abstract class ExpressionOperand<T> : Operand<T>, IExpressionOperand
	{
		internal ExpressionOperand()
		{
			ShouldLeaveValueOnStack = true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilExpression Members

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