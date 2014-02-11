using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class EnumerableShortcutsTests : ClassPerTestCaseFixtureBase
	{
		//[Test]
		//public void TestAll()
		//{
		//	//-- Arrange

		//	DeriveClassFrom<AncestorRepository.EnumerableTester>()
		//		.DefaultConstructor()
		//		.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
		//			m.Return(source.All(s => s.StartsWith(m.Const("U"))));
		//		});

		//	//-- Act

		//	var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
		//	var result = tester.DoTest(new[] { "ABC", "UDE", "FGK", "UIJ", "LMN" }).ToArray();

		//	//-- Assert

		//	Assert.That(result, Is.EqualTo(new[] { "UDE", "UIJ" }));
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestWhere()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.Where(s => s.Length() > m.Const(2)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "1", "55555", "22", "4444", "333" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "55555", "4444", "333" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestOrderBy()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.OrderBy(s => s.Length()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "YY", "AAAA", "Z", "BBB" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "Z", "YY", "BBB", "AAAA" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestOrderByDescending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.OrderByDescending(s => s.Length()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AA", "BBBB", "Y", "ZZZ" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "BBBB", "ZZZ", "AA", "Y" }));
		}
	}
}
