using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	/// <summary>
	/// Base class for all different kinds of operands in Happil.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the operand.
	/// </typeparam>
	/// <remarks>
	/// In addition to <see cref="IHappilOperand{T}"/> interface, this class defines all possible kinds of operators 
	/// on Happil operands, for the fluent API.
	/// </remarks>
	public class HappilOperand<T> : IHappilOperand<T>
	{
		#region Overrides of Object

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TCast> CastTo<TCast>()
		{
			return new HappilBinaryExpression<T, Type, TCast>(
				@operator: new BinaryOperators.OperatorCastOrThrow<T>(), 
				left: this, 
				right: new HappilConstant<Type>(typeof(TCast)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TCast> As<TCast>()
		{
			return new HappilBinaryExpression<T, Type, TCast>(
				@operator: new BinaryOperators.OperatorTryCast<T>(),
				left: this,
				right: new HappilConstant<Type>(typeof(TCast)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator ==(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator !=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorGreaterThanOrEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				@operator: new BinaryOperators.OperatorLessThanOrEqual<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x, HappilOperand<T> y) 
		{
			return new HappilBinaryExpression<T, T>(
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator -(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator *(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				@operator: new BinaryOperators.OperatorMultiply<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator /(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				@operator: new BinaryOperators.OperatorDivide<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator %(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				@operator: new BinaryOperators.OperatorModulo<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator &(HappilOperand<T> x, HappilOperand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new HappilBinaryExpression<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalAnd(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseAnd<T>(),
					left: x,
					right: y);
			}

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator |(HappilOperand<T> x, HappilOperand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new HappilBinaryExpression<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalOr(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseOr<T>(),
					left: x,
					right: y);
			}

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool operator true(HappilOperand<T> x)
		{
			return false;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool operator false(HappilOperand<T> x)
		{
			return false;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator ^(HappilOperand<T> x, HappilOperand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new HappilBinaryExpression<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalXor(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseXor<T>(),
					left: x,
					right: y);
			}

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator <<(HappilOperand<T> x, int y)
		{
			return new HappilBinaryExpression<T, int, T>(
				@operator: new BinaryOperators.OperatorLeftShift<T>(),
				left: x,
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator >>(HappilOperand<T> x, int y)
		{
			return new HappilBinaryExpression<T, int, T>(
				@operator: new BinaryOperators.OperatorRightShift<T>(),
				left: x,
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator !(HappilOperand<T> x)
		{
			ValidateIsBooleanFor("!");

			object result = new HappilUnaryExpression<bool, bool>(
				@operator: new UnaryOperators.OperatorLogicalNot(),
				operand: (IHappilOperand<bool>)x);

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator ~(HappilOperand<T> x)
		{
			return new HappilUnaryExpression<T, T>(
				@operator: new UnaryOperators.OperatorBitwiseNot<T>(),
				operand: x);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		private static void ValidateIsBooleanFor(string operatorName)
		{
			if ( typeof(T) != typeof(bool) )
			{
				throw new ArgumentException(string.Format(
					"Operator {0} cannot be applied to operand of type {1}. It can only be applied to boolean operands.", 
					operatorName, typeof(T).FullName));
			}
		}
	}
}
