using System.Reflection.Emit;
using Happil.Operands;

namespace Happil.Expressions
{
	internal interface IUnaryOperator<T> : IOperator
	{
		void Emit(ILGenerator il, IOperand<T> operand);
	}
}
