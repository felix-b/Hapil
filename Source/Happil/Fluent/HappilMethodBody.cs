﻿using System;
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

		public HappilLocal<T> Local<T>(string name, HappilOperand<T> initialValue)
		{
			throw new NotImplementedException();
		}

		public void Return<T>(HappilOperand<T> operand)
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

		public HappilOperand<object> Argument(string name)
		{
			throw new NotImplementedException();
		}

		public HappilOperand<object> Argument(int index)
		{
			throw new NotImplementedException();
		}

		public HappilArgument<T> Argument<T>(string name)
		{
			throw new NotImplementedException();
		}

		public HappilArgument<T> Argument<T>(int index)
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
