using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Expressions;
using Hapil.Operands;

namespace Hapil
{
	public static class Prefix
	{
		public static Operand<T> PlusPlus<T>(MutableOperand<T> mutable)
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Prefix, positive: true),
				operand: mutable,
				position: UnaryOperatorPosition.Prefix);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> MinusMinus<T>(MutableOperand<T> mutable)
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Prefix, positive: false),
				operand: mutable,
				position: UnaryOperatorPosition.Prefix);
		}
	}
}
