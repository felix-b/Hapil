using Happil.Fluent;

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
	}
}