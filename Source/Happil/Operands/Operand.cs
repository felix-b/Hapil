using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;

namespace Happil.Operands
{
	public abstract class Operand<T> : IOperand<T>, IOperandEmitter
	{
		private readonly Type m_OperandType;
		private TypeMemberCache m_TypeMembers;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Operand()
		{
			m_OperandType = TypeTemplate.Resolve<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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

		#region IOperand Members

		public Operand<TCast> CastTo<TCast>()
		{
			return new BinaryExpressionOperand<T, Type, TCast>(
				@operator: new BinaryOperators.OperatorCastOrThrow<T>(),
				left: this,
				right: new ConstantOperand<Type>(TypeTemplate.Resolve<TCast>()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type OperandType
		{
			get
			{
				return m_OperandType;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IOperandEmitter Members

		void IOperandEmitter.EmitTarget(ILGenerator il)
		{
			OnEmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitLoad(ILGenerator il)
		{
			OnEmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitStore(ILGenerator il)
		{
			OnEmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitAddress(ILGenerator il)
		{
			OnEmitAddress(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool HasTarget
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool IsMutable
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool CanEmitAddress
		{
			get
			{
				return false;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeMemberCache Members
		{
			get
			{
				if ( m_TypeMembers == null )
				{
					m_TypeMembers = TypeMemberCache.Of(this.OperandType);
				}

				return m_TypeMembers;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void OnEmitTarget(ILGenerator il);
		protected abstract void OnEmitLoad(ILGenerator il);
		protected abstract void OnEmitStore(ILGenerator il);
		protected abstract void OnEmitAddress(ILGenerator il);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator Operand<T>(T value) 
		{
			if ( value is IOperand )
			{
				return (Operand<T>)value;
			}
			else
			{
				return new ConstantOperand<T>(value);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator ==(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator ==(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator !=(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator !=(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator >(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator >(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator <(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator <(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator >=(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorGreaterThanOrEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<bool> operator <=(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, bool>(
				@operator: new BinaryOperators.OperatorLessThanOrEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator +(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator +(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator +(Operand<T> x)
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorPlus<T>(),
				operand: x.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator -(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator -(Operand<T> x, ConstantOperand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator -(Operand<T> x)
		{
			return new UnaryExpressionOperand<T, T>(
				@operator: new UnaryOperators.OperatorNegation<T>(),
				operand: x.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator *(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorMultiply<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator /(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorDivide<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator %(Operand<T> x, Operand<T> y)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorModulus<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator &(Operand<T> x, Operand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new BinaryExpressionOperand<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalAnd(),
					left: (IOperand<bool>)x.OrNullConstant<T>(),
					right: (IOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new BinaryExpressionOperand<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseAnd<T>(),
					left: x,
					right: y);
			}

			return (Operand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator |(Operand<T> x, Operand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new BinaryExpressionOperand<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalOr(),
					left: (IOperand<bool>)x.OrNullConstant<T>(),
					right: (IOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new BinaryExpressionOperand<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseOr<T>(),
					left: x.OrNullConstant<T>(),
					right: y.OrNullConstant<T>());
			}

			return (Operand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool operator true(Operand<T> x)
		{
			return false;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool operator false(Operand<T> x)
		{
			return false;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator ^(Operand<T> x, Operand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new BinaryExpressionOperand<bool, bool>(
					@operator: new BinaryOperators.OperatorLogicalXor(),
					left: (IOperand<bool>)x.OrNullConstant<T>(),
					right: (IOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new BinaryExpressionOperand<T, T>(
					@operator: new BinaryOperators.OperatorBitwiseXor<T>(),
					left: x.OrNullConstant<T>(),
					right: y.OrNullConstant<T>());
			}

			return (Operand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator <<(Operand<T> x, int y)
		{
			return new BinaryExpressionOperand<T, int, T>(
				@operator: new BinaryOperators.OperatorLeftShift<T>(),
				left: x.OrNullConstant<T>(),
				right: new ConstantOperand<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator >>(Operand<T> x, int y)
		{
			return new BinaryExpressionOperand<T, int, T>(
				@operator: new BinaryOperators.OperatorRightShift<T>(),
				left: x.OrNullConstant<T>(),
				right: new ConstantOperand<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator !(Operand<T> x)
		{
			if ( TypeTemplate.Resolve<T>() == typeof(bool) )
			{
				object result = new UnaryExpressionOperand<bool, bool>(
					@operator: new UnaryOperators.OperatorLogicalNot(),
					operand: (IOperand<bool>)x.OrNullConstant<T>());

				return (Operand<T>)result;
			}
			else
			{
				throw new ArgumentException("Operator ! can only be applied to type Boolean.");
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static Operand<T> operator ~(Operand<T> x)
		{
			var safeOperand = x.OrNullConstant<T>();
			var operandType = safeOperand.OperandType;

			if ( operandType == typeof(int) || operandType == typeof(uint) || operandType == typeof(long) || operandType == typeof(ulong) )
			{
				return new UnaryExpressionOperand<T, T>(
					@operator: new UnaryOperators.OperatorBitwiseNot<T>(),
					operand: safeOperand);
			}
			else
			{
				throw new ArgumentException("Operator ~ is only supported on types int, uint, long, ulong.");
			}
		}
	}
}
