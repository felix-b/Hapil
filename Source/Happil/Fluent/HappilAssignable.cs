using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public abstract class HappilAssignable<T> : HappilOperand<T>
	{
		internal HappilAssignable(HappilMethod ownerMethod)
			: base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> Assign(IHappilOperand<T> value)
		{
			if ( OperandType.IsValueType && value is IValueTypeInitializer && this is ICanEmitAddress )
			{
				((IValueTypeInitializer)value).Target = this;
				return (HappilOperand<T>)value;
			}
			else
			{
				return new HappilBinaryExpression<T, T>(
					base.OwnerMethod, 
					@operator: new BinaryOperators.OperatorAssign<T>(), 
					left: this, 
					right: value);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public HappilOperand<T> AssignConst(T constantValue)
		{
			return new HappilBinaryExpression<T, T>(
				base.OwnerMethod,
				@operator: new BinaryOperators.OperatorAssign<T>(),
				left: this,
				right: new HappilConstant<T>(constantValue));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> PostfixPlusPlus()
		{
			return new HappilUnaryExpression<T, T>(
				base.OwnerMethod,
				@operator: new UnaryOperators.OperatorPlusPlus<T>(),
				operand: this,
				position: UnaryOperatorPosition.Postfix);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> PostfixMinusMinus()
		{
			return new HappilUnaryExpression<T, T>(
				base.OwnerMethod,
				@operator: new UnaryOperators.OperatorMinusMinus<T>(),
				operand: this,
				position: UnaryOperatorPosition.Postfix);
		}
	}
}
