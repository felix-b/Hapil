using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class ConstantOperand<T> : Operand
	{
		private readonly T m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConstantOperand(T value)
		{
			m_Value = value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator ConstantOperand<T>(T value)
		{
			return new ConstantOperand<T>(value);			
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator T(ConstantOperand<T> operand)
		{
			return operand.m_Value;
		}
	}
}
