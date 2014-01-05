using Happil.Fluent;
using Happil.Statements;

namespace Happil.Expressions
{
	internal interface IHappilExpression : IHappilOperandInternals
	{
		bool ShouldLeaveValueOnStack { get; set; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal abstract class HappilExpression<T> : HappilOperand<T>, IHappilExpression
	{
		internal HappilExpression(HappilMethod ownerMethod)
			: base(ownerMethod)
		{
			ShouldLeaveValueOnStack = true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilExpression Members

		public bool ShouldLeaveValueOnStack { get; set; }

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void EnsureOperandLeavesValueOnStack(IHappilExpression expressionOperand)
		{
			if ( expressionOperand != null )
			{
				expressionOperand.ShouldLeaveValueOnStack = true;
			}
		}
	}
}