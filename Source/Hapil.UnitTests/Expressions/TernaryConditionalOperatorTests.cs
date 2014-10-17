using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Expressions
{
	[TestFixture]
	public class TernaryConditionalOperatorTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void TestTernaryOperator()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					m.Return(m.Iif(input > 0, input + 10, input - 100));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(10);
			var result2 = tester.DoTest(-10);

			//-- Assert

			Assert.That(result1, Is.EqualTo(20));
			Assert.That(result2, Is.EqualTo(-110));
		}
	}
}
