using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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
	public abstract class HappilOperand<T> : IHappilOperand<T>, IHappilOperandInternals
	{
		private HappilMethod m_OwnerMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilOperand(HappilMethod ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void InvokeVoid(Expression<Func<T, Action>> member)
		{
			var method = ValidateMemberIsMethodOfType(((MemberExpression)member.Body).Member);
			
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void InvokeVoid<TArg1>(Expression<Func<T, Action<TArg1>>> member, IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void InvokeVoid<TArg1, TArg2>(Expression<Func<T, Action<TArg1, TArg2>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void InvokeVoid<TArg1, TArg2, TArg3>(Expression<Func<T, Action<TArg1, TArg2, TArg3>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> InvokeFunc<TReturn>(Expression<Func<T, Func<TReturn>>> member)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> InvokeFunc<TArg1, TReturn>(Expression<Func<T, Func<TArg1, TReturn>>> member, IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> InvokeFunc<TArg1, TArg2, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TReturn>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> Property<TProp>(Expression<Func<T, TProp>> property)
		{
			return new PropertyAccessOperand<TProp>(
				target: this, 
				property: Helpers.GetPropertyInfoFromLambda(property).First());
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

		#region IHappilOperandInternals Members

		void IHappilOperandInternals.EmitTarget(ILGenerator il)
		{
			OnEmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandInternals.EmitLoad(ILGenerator il)
		{
			OnEmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandInternals.EmitStore(ILGenerator il)
		{
			OnEmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandInternals.EmitAddress(ILGenerator il)
		{
			OnEmitAddress(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		HappilClass IHappilOperandInternals.OwnerClass
		{
			get
			{
				return this.OwnerClass;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TCast> CastTo<TCast>()
		{
			return new HappilBinaryExpression<T, Type, TCast>(
				OwnerMethod,
				@operator: new BinaryOperators.OperatorCastOrThrow<T>(), 
				left: this, 
				right: new HappilConstant<Type>(typeof(TCast)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TCast> As<TCast>()
		{
			return new HappilBinaryExpression<T, Type, TCast>(
				OwnerMethod,
				@operator: new BinaryOperators.OperatorTryCast<T>(),
				left: this,
				right: new HappilConstant<Type>(typeof(TCast)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void OnEmitTarget(ILGenerator il);
		protected abstract void OnEmitLoad(ILGenerator il);
		protected abstract void OnEmitStore(ILGenerator il);
		protected abstract void OnEmitAddress(ILGenerator il);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilMethod OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal virtual HappilClass OwnerClass
		{
			get
			{
				if ( m_OwnerMethod != null )
				{
					return m_OwnerMethod.HappilClass;
				}
				else
				{
					return null;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator ==(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator !=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorGreaterThanOrEqual<T>(),
				left: x,
				right: y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorLessThanOrEqual<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x, HappilOperand<T> y) 
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator -(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator *(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorMultiply<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator /(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorDivide<T>(),
				left: x,
				right: y);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator %(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				x.OwnerMethod ?? y.OwnerMethod,
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
					x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalAnd(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					x.OwnerMethod ?? y.OwnerMethod,
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
					x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalOr(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					x.OwnerMethod ?? y.OwnerMethod,
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
					x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalXor(),
					left: (IHappilOperand<bool>)x,
					right: (IHappilOperand<bool>)y);
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					x.OwnerMethod ?? y.OwnerMethod,
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
				x.OwnerMethod,
				@operator: new BinaryOperators.OperatorLeftShift<T>(),
				left: x,
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator >>(HappilOperand<T> x, int y)
		{
			return new HappilBinaryExpression<T, int, T>(
				x.OwnerMethod,
				@operator: new BinaryOperators.OperatorRightShift<T>(),
				left: x,
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator !(HappilOperand<T> x)
		{
			ValidateIsBooleanFor("!");

			object result = new HappilUnaryExpression<bool, bool>(
				x.OwnerMethod,
				@operator: new UnaryOperators.OperatorLogicalNot(),
				operand: (IHappilOperand<bool>)x);

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator ~(HappilOperand<T> x)
		{
			return new HappilUnaryExpression<T, T>(
				x.OwnerMethod,
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

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodInfo ValidateMemberIsMethodOfType(MemberInfo member)
		{
			var method = (member as MethodInfo);

			if ( method == null )
			{
				throw new ArgumentException(string.Format(
					"Member {0} cannot be invoked because it is not a method. Invoke can only be applied to methods.",
					member.Name));
			}

			var declaringType = typeof(T);

			while ( declaringType != null )
			{
				if ( member.DeclaringType == declaringType )
				{
					return method;
				}

				declaringType = declaringType.BaseType;
			}

			throw new ArgumentException(string.Format(
				"Member {0} cannot be invoked because it is not a method of type {1}. Invoke can only be applied to the methods of the operand.",
				member.Name, typeof(T).FullName));
		}
	}
}
