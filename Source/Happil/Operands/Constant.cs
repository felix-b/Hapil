using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Operands
{
	public class Constant<T> : Operand<T>
	{
		private readonly T m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Constant(T value)
		{
			m_Value = value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			bool isNull = object.ReferenceEquals(null, m_Value);

			if ( m_Value is Type )
			{
				return string.Format("Type[{0}]", isNull ? "null" : (m_Value as Type).FriendlyName());
			}
			else
			{
				return string.Format("Const[{0}]", isNull ? "null" : m_Value.ToString());
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public T Value
		{
			get
			{
				return m_Value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Constant;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// constants have no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			var actualValue = ResolveActualValue();

			if ( !TryEmitConvertibleValue(il, actualValue as IConvertible) )
			{
				if ( !TryEmitStaticDelegateValue(il, actualValue as Delegate) )
				{
					if ( !TryEmitNullValue(il, actualValue) )
					{
						throw Helpers.CreateConstantNotSupportedException(OperandType);
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException("Constants are not assignabble.");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException("Constants are not assignabble.");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private object ResolveActualValue()
		{
			return TypeTemplate.ResolveValue(m_Value);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool TryEmitConvertibleValue(ILGenerator il, IConvertible convertible)
		{
			if ( convertible != null )
			{
				Helpers.EmitConvertible(il, convertible);

				var valueType = convertible.GetType();

				if ( valueType.IsValueType && OperandType == typeof(object) )
				{
					il.Emit(OpCodes.Box, valueType);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool TryEmitStaticDelegateValue(ILGenerator il, Delegate @delegate)
		{
			if ( @delegate != null )
			{
				if ( !@delegate.Method.IsStatic )
				{
					throw new NotSupportedException("Constants of delegate types can only point to static methods.");
				}

				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ldftn, @delegate.Method);
				il.Emit(OpCodes.Newobj, DelegateShortcuts.GetDelegateConstructor(@delegate.GetType()));

				return true;
			}
			else
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool TryEmitNullValue(ILGenerator il, object value)
		{
			if ( object.ReferenceEquals(null, value) )
			{
				il.Emit(OpCodes.Ldnull);
				return true;
			}
			else
			{
				return false;
			}
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static implicit operator T(HappilConstant<T> operand)
		//{
		//	return operand.m_Value;
		//}
	}
}
