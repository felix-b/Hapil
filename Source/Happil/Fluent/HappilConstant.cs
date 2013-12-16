using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

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

		public static implicit operator HappilConstant<T>(T value)
		{
			return new HappilConstant<T>(value);			
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator T(HappilConstant<T> operand)
		{
			return operand.m_Value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// constants have no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			//TODO: the following line only works for Int32; need to provide a general solution.
			il.Emit(OpCodes.Ldc_I4, (int)(object)m_Value);
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
	}
}
