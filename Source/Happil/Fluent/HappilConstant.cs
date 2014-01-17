using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Selectors;

namespace Happil.Fluent
{
	public class HappilConstant<T> : HappilOperand<T>
	{
		private readonly T m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstant(T value)
			: base(ownerMethod: null)
		{
			m_Value = value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			bool isNull = object.ReferenceEquals(null, m_Value);
			return string.Format("Const<{0}>{{{1}}}", typeof(T).Name, isNull ? "null" : m_Value.ToString());
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

		protected override void OnEmitTarget(ILGenerator il)
		{
			// constants have no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			var actualValue = ResolveActualValue();
			var convertible = actualValue as IConvertible;

			if ( convertible != null )
			{
				EmitConvertible(il, convertible);
			}
			else if ( object.ReferenceEquals(null, actualValue) )
			{
				EmitNull(il);
			}
			else
			{
				throw CreateTypeNotSupportedException();
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
			var template = (m_Value as TypeTemplate);

			if ( template != null )
			{
				return template.CastValue;
			}
			else
			{
				return m_Value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void EmitConvertible(ILGenerator il, IConvertible convertible)
		{
			var formatProvider = CultureInfo.CurrentCulture;

			switch ( convertible.GetTypeCode() )
			{
				case TypeCode.Int32:
				case TypeCode.Boolean:
					il.Emit(OpCodes.Ldc_I4, convertible.ToInt32(formatProvider));
					break;
				case TypeCode.Int64:
					il.Emit(OpCodes.Ldc_I8, convertible.ToInt64(formatProvider));
					break;
				case TypeCode.String:
					il.Emit(OpCodes.Ldstr, convertible.ToString());
					break;
				default:
					throw CreateTypeNotSupportedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void EmitNull(ILGenerator il)
		{
			il.Emit(OpCodes.Ldnull);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Exception CreateTypeNotSupportedException()
		{
			return new NotSupportedException(string.Format(
				"Constants of type '{0}' are not supported.",
				typeof(T).FullName));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator HappilConstant<T>(T value)
		{
			return new HappilConstant<T>(value);			
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static implicit operator T(HappilConstant<T> operand)
		//{
		//	return operand.m_Value;
		//}
	}
}
