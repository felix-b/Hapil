using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Statements;

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

		public Operand<TCast> As<TCast>()
		{
			var castType = TypeTemplate.Resolve<TCast>();

			if ( castType.IsValueType && !castType.IsNullableValueType() )
			{
				throw new ArgumentException("The cast type must be a reference type or a nullable value type.");
			}

			return new BinaryExpressionOperand<T, Type, TCast>(
				@operator: new BinaryOperators.OperatorTryCast<T>(),
				left: this,
				right: new ConstantOperand<Type>(typeof(TCast)));
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

		public void Void(Expression<Func<T, Action>> member)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void(Func<MethodInfo, bool> methodSelector)
		{
			var method = this.Members.SelectVoids(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void(MethodInfo method)
		{
			ValidateMemberIsMethodOfType(method);
			StatementScope.Current.AddStatement(new CallStatement(this, method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1>(Expression<Func<T, Action<TArg1>>> member, IOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1>(MethodInfo method, IOperand<TArg1> arg1)
		{
			ValidateMemberIsMethodOfType(method);
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1>(Func<MethodInfo, bool> methodSelector, IOperand<TArg1> arg1)
		{
			var method = this.Members.SelectVoids<TArg1>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2>(Expression<Func<T, Action<TArg1, TArg2>>> member, IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2>(Func<MethodInfo, bool> methodSelector, IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			var method = this.Members.SelectVoids<TArg1, TArg2>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2, TArg3>(
			Expression<Func<T, Action<TArg1, TArg2, TArg3>>> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2, arg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2, TArg3>(
			Func<MethodInfo, bool> methodSelector,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = this.Members.SelectVoids<TArg1, TArg2>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TReturn>(Expression<Func<T, Func<TReturn>>> member)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TReturn>(Func<MethodInfo, bool> methodSelector)
		{
			var method = this.Members.SelectFuncs<TReturn>(methodSelector).Single();
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TReturn>(MethodInfo method)
		{
			ValidateMemberIsMethodOfType(method);
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TReturn>(Expression<Func<T, Func<TArg1, TReturn>>> member, IOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1);
			return new UnaryExpressionOperand<T, TReturn>(@operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TReturn>(Func<MethodInfo, bool> methodSelector, IOperand<TArg1> arg1)
		{
			var method = this.Members.SelectFuncs<TArg1, TReturn>(methodSelector).Single();
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method, arg1), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TReturn>(MethodInfo method, IOperand<TArg1> arg1)
		{
			ValidateMemberIsMethodOfType(method);
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method, arg1), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TArg2, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TReturn>>> member, IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1, arg2);

			return new UnaryExpressionOperand<T, TReturn>(@operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TArg2, TReturn>(
			Func<MethodInfo, bool> methodSelector,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2)
		{
			var method = this.Members.SelectFuncs<TArg1, TArg2, TReturn>(methodSelector).Single();
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method, arg1, arg2), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TArg3, TReturn>>> member,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1, arg2, arg3);

			return new UnaryExpressionOperand<T, TReturn>(@operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Func<MethodInfo, bool> methodSelector,
			IOperand<TArg1> arg1,
			IOperand<TArg2> arg2,
			IOperand<TArg3> arg3)
		{
			var method = this.Members.SelectFuncs<TArg1, TArg2, TArg3, TReturn>(methodSelector).Single();
			return new UnaryExpressionOperand<T, TReturn>(new UnaryOperators.OperatorCall<T>(method, arg1, arg2, arg3), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TProp> Prop<TProp>(Expression<Func<T, TProp>> property)
		{
			return new PropertyAccessOperand<TProp>(
				target: this,
				property: Helpers.GetPropertyInfoArrayFromLambda(property).First());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TProp> Prop<TProp>(Func<PropertyInfo, bool> propertySelector)
		{
			return new PropertyAccessOperand<TProp>(
				target: this,
				property: this.Members.SelectProps<TProp>(propertySelector).Single());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TProp> Prop<TProp>(PropertyInfo property)
		{
			return new PropertyAccessOperand<TProp>(this, property);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TItem> Item<TIndex, TItem>(Operand<TIndex> indexArg1)
		{
			return Item<TIndex, TItem>((IOperand<TIndex>)indexArg1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TItem> Item<TIndex1, TIndex2, TItem>(Operand<TIndex1> indexArg1, Operand<TIndex2> indexArg2)
		{
			return Item<TIndex1, TIndex2, TItem>((IOperand<TIndex1>)indexArg1, (IOperand<TIndex2>)indexArg2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MutableOperand<TItem> Item<TIndex, TItem>(IOperand<TIndex> indexArg1)
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

		public MutableOperand<TItem> Item<TIndex1, TIndex2, TItem>(IOperand<TIndex1> indexArg1, IOperand<TIndex2> indexArg2)
		{
			var indexerProperty = OperandType.GetProperty("Item", typeof(TItem), new[] { typeof(TIndex1), typeof(TIndex2) });

			if ( indexerProperty == null )
			{
				throw new InvalidOperationException("Could not find indexer with specified types.");
			}

			return new PropertyAccessOperand<TItem>(
				target: this,
				property: indexerProperty,
				indexArguments: new IOperand[] { indexArg1, indexArg2 });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> OrDefault(IOperand<T> defaultValue)
		{
			return new BinaryExpressionOperand<T, T>(
				@operator: new BinaryOperators.OperatorNullCoalesce<T>(),
				left: this,
				right: defaultValue);
		}

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

			if ( method.DeclaringType != typeof(object) )
			{
				var allBaseTypes = typeof(T).GetTypeHierarchy();
				var resolvedBaseTypes = allBaseTypes.Select(TypeTemplate.Resolve).ToArray();

				if ( !resolvedBaseTypes.Contains(member.DeclaringType) )
				{
					throw new ArgumentException(string.Format(
						"Member {0} cannot be invoked because it is not a method of type {1}.",
						member.Name, typeof(T).FullName));
				}
			}

			return method;
		}
	}
}
