using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Happil;

namespace Happil.UnitTests
{
	[TestFixture]
	public class SetShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestAdd()
		{
			//-- Arrange

			var input = new HashSet<int>();

			DeriveClassFrom<HashSetTester>()
				.DefaultConstructor()
				.Method<HashSet<int>>(cls => cls.DoTest).Implement((m, hashSet) => {
					hashSet.Add(m.Const(123));
				});

			//-- Act

			var tester = CreateClassInstanceAs<HashSetTester>().UsingDefaultConstructor();
			tester.DoTest(input);

			//-- Assert

			CollectionAssert.AreEqual(new[] { 123 }, input);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCount()
		{
			//-- Arrange

			var input = new HashSet<int> { 123, 456, 789 };

			DeriveClassFrom<HashSetTester>()
				.DefaultConstructor()
				.Method<HashSet<int>>(cls => cls.DoTest).Implement((m, hashSet) => {
					Static.Prop(() => OutputCount).Assign(hashSet.Count());
				});

			OutputCount = -1;

			//-- Act

			var tester = CreateClassInstanceAs<HashSetTester>().UsingDefaultConstructor();
			tester.DoTest(input);

			//-- Assert

			Assert.That(OutputCount, Is.EqualTo(3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int OutputCount { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class HashSetTester
		{
			public abstract void DoTest(HashSet<int> set);
		}
	}
}
