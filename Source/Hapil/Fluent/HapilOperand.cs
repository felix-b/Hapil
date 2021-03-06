﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Statements;

namespace Hapil.Fluent
{
	/// <summary>
	/// Base class for all different kinds of operands in Hapil.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the operand.
	/// </typeparam>
	/// <remarks>
	/// In addition to <see cref="IHappilOperand{T}"/> interface, this class defines all possible kinds of operators 
	/// on Hapil operands, for the fluent API.
	/// </remarks>
	public abstract class HappilOperand<T> : IHappilOperand<T>, IHappilOperandEmitter, IHappilOperandInternals
	{
		private readonly HappilMethod m_OwnerMethod;
		private readonly Type m_OperandType;
		private TypeMembers m_TypeMembers;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilOperand(HappilMethod ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
			m_OperandType = TypeTemplate.TryResolve<T>();
		}

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

		public void Void<TArg1>(Expression<Func<T, Action<TArg1>>> member, IHappilOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1>(MethodInfo method, IHappilOperand<TArg1> arg1)
		{
			ValidateMemberIsMethodOfType(method);
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1>(Func<MethodInfo, bool> methodSelector, IHappilOperand<TArg1> arg1)
		{
			var method = this.Members.SelectVoids<TArg1>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2>(Expression<Func<T, Action<TArg1, TArg2>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2>(Func<MethodInfo, bool> methodSelector, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = this.Members.SelectVoids<TArg1, TArg2>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2, TArg3>(
			Expression<Func<T, Action<TArg1, TArg2, TArg3>>> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2, 
			IHappilOperand<TArg3> arg3)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2, arg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Void<TArg1, TArg2, TArg3>(
			Func<MethodInfo, bool> methodSelector, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2, 
			IHappilOperand<TArg3> arg3)
		{
			var method = this.Members.SelectVoids<TArg1, TArg2>(methodSelector).Single();
			StatementScope.Current.AddStatement(new CallStatement(this, method, arg1, arg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TReturn>(Expression<Func<T, Func<TReturn>>> member)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TReturn>(Func<MethodInfo, bool> methodSelector)
		{
			var method = this.Members.SelectFuncs<TReturn>(methodSelector).Single();
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TReturn>(MethodInfo method)
		{
			ValidateMemberIsMethodOfType(method);
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TReturn>(Expression<Func<T, Func<TArg1, TReturn>>> member, IHappilOperand<TArg1> arg1)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1);
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, @operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TReturn>(Func<MethodInfo, bool> methodSelector, IHappilOperand<TArg1> arg1)
		{
			var method = this.Members.SelectFuncs<TArg1, TReturn>(methodSelector).Single();
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method, arg1), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TReturn>(MethodInfo method, IHappilOperand<TArg1> arg1)
		{
			ValidateMemberIsMethodOfType(method);
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method, arg1), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TArg2, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TReturn>>> member, IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1, arg2);

			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, @operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TArg2, TReturn>(
			Func<MethodInfo, bool> methodSelector, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2)
		{
			var method = this.Members.SelectFuncs<TArg1, TArg2, TReturn>(methodSelector).Single();
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method, arg1, arg2), this);
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Expression<Func<T, Func<TArg1, TArg2, TArg3, TReturn>>> member, 
			IHappilOperand<TArg1> arg1, 
			IHappilOperand<TArg2> arg2,
			IHappilOperand<TArg3> arg3)
		{
			var method = ValidateMemberIsMethodOfType(Helpers.ResolveMethodFromLambda(member));
			var @operator = new UnaryOperators.OperatorCall<T>(method, arg1, arg2, arg3);

			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, @operator, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TReturn> Func<TArg1, TArg2, TArg3, TReturn>(
			Func<MethodInfo, bool> methodSelector,
			IHappilOperand<TArg1> arg1,
			IHappilOperand<TArg2> arg2,
			IHappilOperand<TArg3> arg3)
		{
			var method = this.Members.SelectFuncs<TArg1, TArg2, TArg3, TReturn>(methodSelector).Single();
			return new HappilUnaryExpression<T, TReturn>(m_OwnerMethod, new UnaryOperators.OperatorCall<T>(method, arg1, arg2, arg3), this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> Prop<TProp>(Expression<Func<T, TProp>> property)
		{
			return new PropertyAccessOperand<TProp>(
				target: this, 
				property: Helpers.GetPropertyInfoArrayFromLambda(property).First());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> Prop<TProp>(Func<PropertyInfo, bool> propertySelector)
		{
			return new PropertyAccessOperand<TProp>(
				target: this,
				property: this.Members.SelectProps<TProp>(propertySelector).Single());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TProp> Prop<TProp>(PropertyInfo property)
		{
			return new PropertyAccessOperand<TProp>(this, property);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TField> Field<TField>(Expression<Func<T, TField>> field)
		{
			return new FieldAccessOperand<TField>(
				target: this,
				field: Helpers.ResolveFieldFromLambda(field));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TField> Field<TField>(Func<FieldInfo, bool> fieldSelector)
		{
			return new FieldAccessOperand<TField>(
				target: this,
				field: this.Members.SelectFields<TField>(fieldSelector).Single());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAssignable<TField> Field<TField>(FieldInfo field)
		{
			return new FieldAccessOperand<TField>(this, field);
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

		#region IHappilOperandEmitter Members

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
				right: new HappilConstant<Type>(TypeTemplate.Resolve<TCast>()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TCast> As<TCast>()
		{
			var castType = TypeTemplate.Resolve<TCast>();

			if ( castType.IsValueType && !castType.IsNullableValueType() )
			{
				throw new ArgumentException("The cast type must be a reference type or a nullable value type.");
			}
	
			return new HappilBinaryExpression<T, Type, TCast>(
				OwnerMethod,
				@operator: new BinaryOperators.OperatorTryCast<T>(),
				left: this,
				right: new HappilConstant<Type>(typeof(TCast)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> OrDefault(IHappilOperand<T> defaultValue)
		{
			return new HappilBinaryExpression<T, T>(
				OwnerMethod,
				@operator: new BinaryOperators.OperatorNullCoalesce<T>(),
				left: this,
				right: defaultValue);
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

		public virtual bool HasTarget
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeMembers Members
		{
			get
			{
				if ( m_TypeMembers == null )
				{
					m_TypeMembers = TypeMembers.Of(this.OperandType);
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
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator ==(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator !=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator !=(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorNotEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorGreaterThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorLessThan<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator >=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorGreaterThanOrEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<bool> operator <=(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, bool>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorLessThanOrEqual<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x, HappilOperand<T> y) 
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorAdd<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator +(HappilOperand<T> x)
		{
			return new HappilUnaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new UnaryOperators.OperatorPlus<T>(),
				operand: x.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator -(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator -(HappilOperand<T> x, HappilConstant<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorSubtract<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator -(HappilOperand<T> x)
		{
			return new HappilUnaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new UnaryOperators.OperatorNegation<T>(),
				operand: x.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator *(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorMultiply<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator /(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorDivide<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator %(HappilOperand<T> x, HappilOperand<T> y)
		{
			return new HappilBinaryExpression<T, T>(
				null,//x.OwnerMethod ?? y.OwnerMethod,
				@operator: new BinaryOperators.OperatorModulus<T>(),
				left: x.OrNullConstant<T>(),
				right: y.OrNullConstant<T>());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator &(HappilOperand<T> x, HappilOperand<T> y)
		{
			object result;

			if ( typeof(T) == typeof(bool) )
			{
				result = new HappilBinaryExpression<bool, bool>(
					null,//x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalAnd(),
					left: (IHappilOperand<bool>)x.OrNullConstant<T>(),
					right: (IHappilOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					null,//x.OwnerMethod ?? y.OwnerMethod,
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
					null,//x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalOr(),
					left: (IHappilOperand<bool>)x.OrNullConstant<T>(),
					right: (IHappilOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					null,//x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorBitwiseOr<T>(),
					left: x.OrNullConstant<T>(),
					right: y.OrNullConstant<T>());
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
					null,//x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorLogicalXor(),
					left: (IHappilOperand<bool>)x.OrNullConstant<T>(),
					right: (IHappilOperand<bool>)y.OrNullConstant<T>());
			}
			else
			{
				result = new HappilBinaryExpression<T, T>(
					null,//x.OwnerMethod ?? y.OwnerMethod,
					@operator: new BinaryOperators.OperatorBitwiseXor<T>(),
					left: x.OrNullConstant<T>(),
					right: y.OrNullConstant<T>());
			}

			return (HappilOperand<T>)result;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator <<(HappilOperand<T> x, int y)
		{
			return new HappilBinaryExpression<T, int, T>(
				null,//x.OwnerMethod
				@operator: new BinaryOperators.OperatorLeftShift<T>(),
				left: x.OrNullConstant<T>(),
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator >>(HappilOperand<T> x, int y)
		{
			return new HappilBinaryExpression<T, int, T>(
				null,//x.OwnerMethod
				@operator: new BinaryOperators.OperatorRightShift<T>(),
				left: x.OrNullConstant<T>(),
				right: new HappilConstant<int>(y));
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator !(HappilOperand<T> x)
		{
			if ( TypeTemplate.Resolve<T>() == typeof(bool) )
			{
				object result = new HappilUnaryExpression<bool, bool>(
					null,
					//x.OwnerMethod
					@operator: new UnaryOperators.OperatorLogicalNot(),
					operand: (IHappilOperand<bool>)x.OrNullConstant<T>());

				return (HappilOperand<T>)result;
			}
			else
			{
				throw new ArgumentException("Operator ! can only be applied to type Boolean.");
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilOperand<T> operator ~(HappilOperand<T> x)
		{
			var safeOperand = x.OrNullConstant<T>();
			var operandType = safeOperand.OperandType;

			if ( operandType == typeof(int) || operandType == typeof(uint) || operandType == typeof(long) || operandType == typeof(ulong) )
			{
				return new HappilUnaryExpression<T, T>(
					null,//x.OwnerMethod
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

			var allBaseTypes = typeof(T).GetTypeHierarchy();
			var resolvedBaseTypes = allBaseTypes.Select(TypeTemplate.Resolve).ToArray();

			if ( !resolvedBaseTypes.Contains(member.DeclaringType) )
			{
				throw new ArgumentException(string.Format(
					"Member {0} cannot be invoked because it is not a method of type {1}.",
					member.Name, typeof(T).FullName));
			}

			return method;
		}
	}
}
