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

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, int>(cls => cls.DoIntTest).Implement((m, input) => {
					var list = m.New<List<string>>(input);
					m.Return(list.Count());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoIntTest(new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(result, Is.EqualTo(3));
		}
	}
}
