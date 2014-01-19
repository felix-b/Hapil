using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.UnitTests
{
	public static class TargetRepository
	{
		public class TargetWithDefaultConstructor
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TargetWithNonDefaultConstructor
		{
			public TargetWithNonDefaultConstructor(int intValue, string stringValue)
			{
				this.IntValue = intValue;
				this.StringValue = stringValue;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int IntValue { get; protected set; }
			public string StringValue { get; protected set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TargetWithMultipleConstructors : TargetWithNonDefaultConstructor
		{
			public TargetWithMultipleConstructors() 
				: base(123, "ABC")
			{
			}
			public TargetWithMultipleConstructors(int intValue)
				: base(intValue, "ZZZ")
			{
			}
			public TargetWithMultipleConstructors(string stringValue)
				: base(999, stringValue)
			{
			}
		}
	}
}
