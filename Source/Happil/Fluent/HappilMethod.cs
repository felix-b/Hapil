using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	internal class HappilMethod : IHappilMember, IHappilMethodBody
	{
		#region IHappilMember Members

		public IHappilMember[] Flatten()
		{
			throw new NotImplementedException();
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IHappilMethodBody Members

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

		public void EmitByExample(System.Linq.Expressions.Expression<Action> action)
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

		public System.Reflection.MethodInfo MethodInfo
		{
			get { throw new NotImplementedException(); }
		}

		public int ArgumentCount
		{
			get { throw new NotImplementedException(); }
		}

		public Type ReturnValue
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
