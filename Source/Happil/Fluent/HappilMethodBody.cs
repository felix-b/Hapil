using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilMethodBody
	{
		public void Return(HappilOperand operand)
		{
			throw new NotImplementedException();
		}

		public void Throw<TException>(string message) where TException : Exception
		{
			throw new NotImplementedException();
		}

		public HappilOperand Argument(string name)
		{
			throw new NotImplementedException();
		}

		public HappilOperand Argument(int index)
		{
			throw new NotImplementedException();
		}

		public int ArgumentCount
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Type ReturnValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
