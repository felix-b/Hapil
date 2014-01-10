using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class StringShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestLength()
		{
			//-- Arrange

			DeriveClassFrom<StringTester>()
				.DefaultConstructor()
				.Method(cls => cls.DoTest).Implement(m => {
					Static.Prop(() => OutputLength).Assign(Static.Prop(() => InputString).Length());
				});

			InputString = "AAABBBC";

			//-- Act

			var obj = CreateClassInstanceAs<StringTester>().UsingDefaultConstructor();
			obj.DoTest();

			//-- Assert

			Assert.That(OutputLength, Is.EqualTo(7));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static string InputString { get; set; }
		public static int OutputLength { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StringTester
		{
			public abstract void DoTest();
		}
	}
}
