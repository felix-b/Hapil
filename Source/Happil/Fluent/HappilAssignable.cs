using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public abstract class HappilAssignable<T> : HappilOperand<T>
	{
		public HappilOperand<T> Assign(HappilOperand<T> operand)
		{
			throw new NotImplementedException();
		}
		public HappilOperand<T> Assign(HappilConstant<T> operand)
		{
			throw new NotImplementedException();
		}
	}
}
