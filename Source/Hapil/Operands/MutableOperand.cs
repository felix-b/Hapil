using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Expressions;

namespace Hapil.Operands
{
	public abstract class MutableOperand<T> : Operand<T>, IMutableOperand
	{
		internal MutableOperand()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IMutableOperand Members

		IOperand IMutableOperand.Assign(IOperand value)
		{
			if ( value is IOperand<T> )
			{
				return Assign((IOperand<T>)value);
			}
			else
			{
				return Assign(value.CastTo<T>());
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> Assign(T value)
		{
			return Assign(new Constant<T>(value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> Assign(IOperand<T> value)
		{
			if ( OperandType.IsValueType && value is IValueTypeInitializer && this is ICanEmitAddress )
			{
				((IValueTypeInitializer)value).Target = this;
				return (Operand<T>)value;
			}
			else
			{
				return new BinaryExpressionOperand<T, T>(
					@operator: new BinaryOperators.OperatorAssign<T>(),
					left: this,
					right: value.OrNullConstant());
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> PostfixPlusPlus()
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Postfix, positive: true),
				operand: this,
				position: UnaryOperatorPosition.Postfix);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> PostfixMinusMinus()
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorIncrement<T>(UnaryOperatorPosition.Postfix, positive: false),
				operand: this,
				position: UnaryOperatorPosition.Postfix);
		}
	}
}
