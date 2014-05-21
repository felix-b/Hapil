using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;
using Happil;

namespace Happil.UnitTests
{
	[TestFixture]
	public class SetShortcutsTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void TestAdd()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool, IEnumerable<string>>(cls => (input, result) => cls.DoOutBooleanTest(input, out result))
					.Implement((m, input, result) => {
						var hashSet = m.Local(m.New<HashSet<string>>(input));
						result.Assign(hashSet.Add(m.Const("ZZZ")));
						m.Return(hashSet);
					});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();

			bool added1;
			var result1 = tester.DoOutBooleanTest(new[] { "AAA", "BBB" }, out added1);

			bool added2;
			var result2 = tester.DoOutBooleanTest(new[] { "YYY", "ZZZ" }, out added2);

			//-- Assert

			CollectionAssert.AreEquivalent(new[] { "AAA", "BBB", "ZZZ" }, result1);
			Assert.That(added1, Is.True);

			CollectionAssert.AreEquivalent(new[] { "YYY", "ZZZ" }, result2);
			Assert.That(added2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestExceptWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					hashSet.ExceptWith(second);
					m.Return(hashSet);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "AAA", "BBB", "CCC", "DDD" }, new[] { "BBB", "CCC" });

			//-- Assert

			CollectionAssert.AreEquivalent(new[] { "AAA", "DDD" }, result);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIntersectWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					hashSet.IntersectWith(second);
					m.Return(hashSet);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "AAA", "BBB", "CCC", "DDD" }, new[] { "BBB", "CCC", "ZZZ" });

			//-- Assert

			CollectionAssert.AreEquivalent(new[] { "BBB", "CCC" }, result);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIsProperSubsetOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.IsProperSubsetOf(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB", "CCC" });
			var result3 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "BBB", "CCC" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
			Assert.That(result3, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIsProperSupersetOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.IsProperSupersetOf(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB", "CCC" }, new[] { "AAA", "BBB" });
			var result3 = tester.DoBooleanBinaryTest(new[] { "AAA" }, new[] { "AAA", "BBB" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
			Assert.That(result3, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIsSubsetOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.IsSubsetOf(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB", "CCC" });
			var result3 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "BBB", "CCC" });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.True);
			Assert.That(result3, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIsSupersetOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.IsSupersetOf(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB", "CCC" }, new[] { "AAA", "BBB" });
			var result3 = tester.DoBooleanBinaryTest(new[] { "AAA" }, new[] { "AAA", "BBB" });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.True);
			Assert.That(result3, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestOverlaps()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.Overlaps(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "CCC", "DDD" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "BBB", "CCC" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSetEquals()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, bool>(cls => cls.DoBooleanBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					m.Return(hashSet.SetEquals(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanBinaryTest(new[] { "AAA", "BBB" }, new[] { "AAA", "CCC" });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSymmetricExceptWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					hashSet.SymmetricExceptWith(second);
					m.Return(hashSet);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "AAA", "BBB", "CCC", "DDD" }, new[] { "BBB", "CCC", "ZZZ" });

			//-- Assert

			CollectionAssert.AreEquivalent(new[] { "AAA", "DDD", "ZZZ" }, result);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestUnionWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					var hashSet = m.Local(m.New<HashSet<string>>(first));
					hashSet.UnionWith(second);
					m.Return(hashSet);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "AAA", "BBB", "CCC", "DDD" }, new[] { "BBB", "CCC", "ZZZ" });

			//-- Assert

			CollectionAssert.AreEquivalent(new[] { "AAA", "BBB", "CCC", "DDD", "ZZZ" }, result);
		}
	}
}
