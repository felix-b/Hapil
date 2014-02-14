using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class CollectionShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestIndexOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, int>(cls => cls.DoIntTest).Implement((m, input) => {
					var list = m.Local(m.New<List<string>>(input));
					m.Return(list.IndexOf(m.Const("OPQ")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoIntTest(new[] { "AAA", "BBB", "OPQ", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestInsert()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(m.New<List<string>>(input));
					list.Insert(m.Const(2), m.Const("OPQ"));
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "BBB", "OPQ", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRemoveAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(m.New<List<string>>(input));
					list.RemoveAt(m.Const(2));
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC", "DDD" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "BBB", "DDD" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestGetItemAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, input) => {
					var list = m.Local(m.New<List<string>>(input));
					m.Return(list.ItemAt(m.Const(1)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo("BBB"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSetItemAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(m.New<List<string>>(input));
					list.ItemAt(m.Const(1)).AssignConst("ZZZ");
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "ZZZ", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestGetElementAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, input) => {
					var array = m.Local(initialValue: input.ToArray());
					m.Return(array.ElementAt(m.Const(1)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo("BBB"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSetElementAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var array = m.Local(initialValue: input.ToArray());
					array.ElementAt(m.Const(1)).AssignConst("ZZZ");
					m.Return(array);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "ZZZ", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestAdd()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(initialValue: input.ToList());
					list.Add(m.Const("ZZZ"));
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "BBB", "CCC", "ZZZ" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestClear()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(initialValue: input.ToList());
					list.Clear();
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result.Any(), Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestContains()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool>(cls => cls.DoBooleanTest).Implement((m, input) => {
					var list = m.Local(initialValue: input.ToList());
					m.Return(list.Contains(m.Const("ZZZ")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new[] { "AAA", "BBB", "CCC" });
			var result2 = tester.DoBooleanTest(new[] { "AAA", "ZZZ", "CCC" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCopyTo()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var array = m.Local(m.NewArray<string>(input.Count() + 2));
					array.ElementAt(0).AssignConst("^^");
					array.ElementAt(array.Length() - 1).AssignConst("$$");	

					var list = m.Local(initialValue: input.ToList());
					list.CopyTo(array, arrayIndex: m.Const(1));
					m.Return(array);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "^^", "AAA", "BBB", "CCC", "$$" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIsReadOnly()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool>(cls => cls.DoBooleanTest).Implement((m, input) => {
					var collection = m.Local<ICollection<string>>();
					
					m.If(input.First() == "ZZZ").Then(() => {
						collection.Assign(m.New<ReadOnlyCollection<string>>(input.ToArray()));
					})
					.Else(() => {
						collection.Assign(m.New<List<string>>(input));
					});
					
					m.Return(collection.IsReadOnly());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new[] { "AAA", "BBB" });
			var result2 = tester.DoBooleanTest(new[] { "ZZZ", "WWW" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRemove()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, input) => {
					var list = m.Local(initialValue: input.ToList());
					list.Remove(m.Const("ZZZ"));
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "AAA", "BBB", "ZZZ", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "AAA", "BBB", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLength()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, int>(cls => cls.DoIntTest).Implement((m, input) => {
					var array = m.Local<string[]>(input.ToArray());
					m.Return(array.Length());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoIntTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo(3));
		}
	}
}
