using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class CollectionShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestCount()
		{
			//-- Arrange

			var input = new string[] { "A", "B", "C" };
			OutputCount = -1;

			DeriveClassFrom<CollectionTester>()
				.DefaultConstructor()
				.Method<ICollection<string>>(cls => cls.DoTest).Implement((m, inputCollection) => {
					Static.Prop(() => OutputCount).Assign(inputCollection.Count());
				});

			//-- Act

			var tester = CreateClassInstanceAs<CollectionTester>().UsingDefaultConstructor();
			tester.DoTest(input);

			//-- Assert

			Assert.That(OutputCount, Is.EqualTo(3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int OutputCount { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class CollectionTester
		{
			public abstract void DoTest(ICollection<string> inputCollection);
		}
	}
}
