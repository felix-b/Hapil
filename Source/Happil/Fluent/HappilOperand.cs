using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Statements;

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
	public abstract class HappilOperand<T> : IHappilOperand<T>, IHappilOperandEmitter, IHappilOperandInternals
	{
		private HappilMethod m_OwnerMethod;
		private readonly Type m_OperandType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilOperand(HappilMethod ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
			m_OperandType = TypeTemplate.TryResolve<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Method(Expression<Func<T, Action>> member)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			StatementScope.Current.AddStatement(new CallStatement(this, method));
		}
		public void M(Expression<Func<T, Action>> member)
		{
			Method(member);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Method<TArg1>(Expression<Func<T, Action<TArg1>>> member, IHappilOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}
		public void M<TArg1>(Expression<Func<T, Action<TArg1>>> member, IHappilOperand<TArg1> arg1)
		{
			Method(member, arg1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Method<TArg1, TArg2>(Expression<Func<T, Action<TArg1, TArg2>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}
		public void M<TArg1, TArg2>(Expression<Func<T, Action<TArg1, TArg2>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			Method(member, arg1, arg2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Method<TArg1, TArg2, TArg3>(Expression<Func<T, Action<TArg1, TArg2, TArg3>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2, arg3));
		}
		public void M<TArg1, TArg2, TArg3>(Expression<Func<T, Action<TArg1, TArg2, TArg3>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			Method(member, arg1, arg2, arg3);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Method<TReturn>(Expression<Func<T, Func<TReturn>>> member)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method), this);
		}
		public HappilOperand<TReturn> M<TReturn>(Expression<Func<T, Func<TReturn>>> member)
		{
			return Method(member);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Method<TArg1, TReturn>(Expression<Func<T, Func<TArg1, TReturn>>> member, IHappilOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1);
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, @operator, this);
		}
		public HappilOperand<TReturn> M<TArg1, TReturn>(Expression<Func<T, Func<TArg1, TReturn>>> member, IHappilOperand<TArg1> arg1)
		{
			return Method(member, arg1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Method<TArg1, TArg2, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TReturn>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.GetMethodInfoFromLambda(member).First());
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1, arg2);

			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, @operator, this);
		}
		public HappilOperand<TReturn> M<TArg1, TArg2, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TReturn>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			return Method(member, arg1, arg2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> P<TProp>(Expression<Func<T, TProp>> property)
		{
			return Property<TProp>(property);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> Property<TProp>(Expression<Func<T, TProp>> property)
		{
			return new PropertyAccessOperand<TProp>(
				target: this, 
				property: Helpers.GetPropertyInfoArrayFromLambda(property).First());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TItem> Item<TIndex, TItem>(HappilConstant<TIndex> indexArg1)
		{
			return Item<TIndex, TItem>((IHappilOperand<TIndex>)indexArg1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TItem> Item<TIndex1, TIndex2, TItem>(HappilConstant<TIndex1> indexArg1, HappilConstant<TIndex2> indexArg2)
		{
			return Item<TIndex1, TIndex2, TItem>((IHappilOperand<TIndex1>)indexArg1, (IHappilOperand<TIndex2>)indexArg2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TItem> Item<TIndex, TItem>(IHappilOperand<TIndex> indexArg1)
		{
			var indexerProperty = OperandType.GetProperty("Item", typeof(TItem), new[] { typeof(TIndex) });

			if ( indexerProperty == null )
			{
				throw new InvalidOperationException("Could not find indexer with specified types.");
			}

			return new PropertyAccessOperand<TItem>(
				target: this,
				property: indexerProperty,
				indexArguments: indexArg1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TItem> Item<TIndex1, TIndex2, TItem>(IHappilOperand<TIndex1> indexArg1, IHappilOperand<TIndex2> indexArg2)
		{
			var indexerProperty = OperandType.GetProperty("Item", typeof(TItem), new[] { typeof(TIndex1), typeof(TIndex2) });

			if ( indexerProperty == null )
			{
				throw new InvalidOperationException("Could not find indexer with specified types.");
			}

			return new PropertyAccessOperand<TItem>(
				target: this,
				property: indexerProperty,
				indexArguments: new IHappilOperand[] { indexArg1, indexArg2 });
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

		void IHappilOperandEmitter.EmitTarget(ILGenerator il)
		{
			OnEmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandEmitter.EmitLoad(ILGenerator il)
		{
			OnEmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandEmitter.EmitStore(ILGenerator il)
		{
			OnEmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilOperandEmitter.EmitAddress(ILGenerator il)
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		HappilMethod IHappilOperandInternals.OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
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

		public virtual Type OperandType
		{
			get
			{
				if ( m_OperandType != null )
				{
					return m_OperandType;
				}
				else
				{
					//TODO: cover this case in unit tests!
					throw new NotSupportedException("The operand class must override OperandType property and provide a specialized implementation.");
				}
			}
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
					"Member {0} cannot be invoked because it is not a method.",
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
				"Member {0} cannot be invoked because it is not a method of type {1}.",
				member.Name, typeof(T).FullName));
		}
	}
}
