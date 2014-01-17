using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil;
using Happil.Fluent;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.UnitTests.Statements
{
	[TestFixture]
	public class ForStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestForCounterAutoIncrementAscending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(0, count).Do((loop, i) => {
						Static.Prop(() => OutputList).Add(i);
					});
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			OutputList = new List<int>();

			//-- Act

			tester.DoTest(5);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForCounterAutoIncrementDescending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(count, 0, increment: -1).Do((loop, i) => {
						Static.Prop(() => OutputList).Add(i);
					});
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			OutputList = new List<int>();

			//-- Act

			tester.DoTest(5);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] { 4, 3, 2, 1, 0 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<int> InputList { get; set; }
		public static List<int> OutputList { get; set; }
	}
}
