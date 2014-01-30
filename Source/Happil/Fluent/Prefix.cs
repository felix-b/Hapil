using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public static class Prefix
	{
		public static HappilOperand<T> PlusPlus<T>(HappilAssignable<T> assignable)
		{
			return new HappilUnaryExpression<T, T>(
				assignable.OwnerMethod,
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Prefix, positive: true),
				operand: assignable,
				position: UnaryOperatorPosition.Prefix);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> MinusMinus<T>(HappilAssignable<T> assignable)
		{
			return new HappilUnaryExpression<T, T>(
				assignable.OwnerMethod,
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Prefix, positive: false),
				operand: assignable,
				position: UnaryOperatorPosition.Prefix);
		}
	}
}
