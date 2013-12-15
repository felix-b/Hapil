using System.Reflection.Emit;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal interface IBinaryOperator<TLeft, TRight> : IOperator
	{
		void Emit(ILGenerator il, IHappilOperand<TLeft> left, IHappilOperand<TRight> right);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal interface IBinaryOperator<T> : IBinaryOperator<T, T>
	{
	}
}
