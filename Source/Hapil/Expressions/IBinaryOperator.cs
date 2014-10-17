using System.Reflection.Emit;
using Hapil.Operands;

namespace Hapil.Expressions
{
	internal interface IBinaryOperator<TLeft, TRight> : IOperator
	{
		void Emit(ILGenerator il, IOperand<TLeft> left, IOperand<TRight> right);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal interface IBinaryOperator<T> : IBinaryOperator<T, T>
	{
	}
}
