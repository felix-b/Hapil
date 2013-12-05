using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class AssignableOperand<T> : Operand<T>
	{
		public Operand Assign(Operand<T> operand)
		{
			throw new NotImplementedException();
		}
		public Operand Assign(ConstantOperand<T> operand)
		{
			throw new NotImplementedException();
		}
	}
}
