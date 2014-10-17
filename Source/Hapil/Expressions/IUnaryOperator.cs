using System.Reflection.Emit;
using Hapil.Operands;

namespace Hapil.Expressions
{
	internal interface IUnaryOperator<T> : IOperator
	{
		void Emit(ILGenerator il, IOperand<T> operand);
	}
}
