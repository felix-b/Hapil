using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.UnitTests
{
	public class TestBaseOne
	{
		public void VoidMethod()
		{
		}
		public void VoidMethodWithOneArg(int number)
		{
		}
		public void VoidMethodWithManyArgs(int number, string text)
		{
		}
		public void VoidMethodWithManyArgs(int number, DateTime date)
		{
		}
		public void VoidMethodWithManyArgs(int number, string text, DateTime date)
		{
		}
		public int FuncWithNoArgs()
		{
			return 123;
		}
		public int FuncWithOneArg(int number)
		{
			return 123;
		}
		public int FuncWithManyArgs(int number, string text)
		{
			return 123;
		}
		public int FuncWithManyArgs(int number, string text, DateTime date)
		{
			return 123;
		}
		public virtual void VoidVirtualMethod()
		{
		}
	}
}
