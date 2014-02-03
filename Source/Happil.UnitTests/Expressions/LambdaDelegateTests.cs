using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class LambdaDelegateTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestAnonymousDelegate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.Where(m.Delegate<string, bool>((del, s) => del.Return(s.Length() > del.Const(2)))));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "1", "55555", "22", "4444", "333" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "55555", "4444", "333" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDelegateReference()
		{

		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDelegateToExistingMethod()
		{

		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLambdaExpression()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.OrderBy(m.Lambda<string, int>(s => s.Length())));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "YY", "AAAA", "Z", "BBB" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "Z", "YY", "BBB", "AAAA" }));
		}
	}
}
