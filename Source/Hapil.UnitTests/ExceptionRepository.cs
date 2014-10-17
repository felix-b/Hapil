using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hapil.UnitTests
{
	public static class ExceptionRepository
	{
		public class TestExceptionOne : Exception
		{
			public TestExceptionOne(string message)
				: base(message)
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestExceptionTwo : Exception
		{
			public TestExceptionTwo(string message)
				: base(message)
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestExceptionThree : Exception
		{
			public TestExceptionThree(string message)
				: base(message)
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestExceptionDefaultCtor : Exception
		{
		}
	}
}
