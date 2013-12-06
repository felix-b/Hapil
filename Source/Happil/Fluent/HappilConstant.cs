using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilConstant<T> : HappilOperand<T>
	{
		private readonly T m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstant(T value)
		{
			m_Value = value;
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
	}
}
