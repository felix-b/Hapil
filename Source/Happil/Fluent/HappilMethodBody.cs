using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public class HappilMethodBody
	{
		public HappilLocal<T> Local<T>(string name)
		{
			throw new NotImplementedException();
		}

		public HappilLocal<T> Local<T>(string name, Operand initialValue)
		{
			throw new NotImplementedException();
		}

		public void Return(Operand operand)
		{
			throw new NotImplementedException();
		}

		public void Throw<TException>(string message) where TException : Exception
		{
			throw new NotImplementedException();
		}

		public void Emit(Expression<Action> action)
		{
			throw new NotImplementedException();
		}

		public Operand Argument(string name)
		{
			throw new NotImplementedException();
		}

		public Operand Argument(int index)
		{
			throw new NotImplementedException();
		}

		public MethodInfo MethodInfo
		{
			get
			{
				throw new NotImplementedException();
			}
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
