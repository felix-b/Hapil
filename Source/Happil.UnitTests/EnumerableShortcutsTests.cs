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
		[Test]
		public void TestAll()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool>(cls => cls.DoBooleanTest).Implement((m, source) => {
					m.Return(source.All(s => s.StartsWith(m.Const("U"))));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new[] { "UA", "UB", "XC" });
			var result2 = tester.DoBooleanTest(new[] { "UA", "UB", "UC" });

			//-- Assert

			Assert.That(result1, Is.False);
			Assert.That(result2, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestAny()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool>(cls => cls.DoBooleanTest).Implement((m, source) => {
					m.Return(source.Any(s => s.StartsWith(m.Const("U"))));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new[] { "UA", "UB", "UC" });
			var result2 = tester.DoBooleanTest(new[] { "XA", "UB", "XC" });
			var result3 = tester.DoBooleanTest(new[] { "XA", "XB", "XC" });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.True);
			Assert.That(result3, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestGroupBy()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<AncestorRepository.FirstLetterGroup>>(cls => cls.DoGroupingTest).Implement((m, source) => {
					var groups = m.Local<IEnumerable<IGrouping<string, string>>>();
					var results = m.Local<IEnumerable<AncestorRepository.FirstLetterGroup>>();
					
					groups.Assign(source.GroupBy(s => s.Substring(m.Const(0), m.Const(1))));
					results.Assign(groups.Select(m.Delegate<IGrouping<string, string>, AncestorRepository.FirstLetterGroup>((del, group) => {
						var firstLetterGroup = del.Local(del.New<AncestorRepository.FirstLetterGroup>());
						firstLetterGroup.Prop(x => x.FirstLetter).Assign(group.Prop(x => x.Key));
						firstLetterGroup.Prop(x => x.Values).Assign(group.ToArray());
						del.Return(firstLetterGroup);
					})));

					m.Return(results);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var input = new[] { "A1", "B1", "A2", "B2" };
			var output = tester.DoGroupingTest(input).ToArray();

			//-- Assert

			Assert.That(output.Length, Is.EqualTo(2));
	
			Assert.That(output[0].FirstLetter, Is.EqualTo("A"));
			Assert.That(output[0].Values, Is.EqualTo(new[] { "A1", "A2" }));

			Assert.That(output[1].FirstLetter, Is.EqualTo("B"));
			Assert.That(output[1].Values, Is.EqualTo(new[] { "B1", "B2" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestOfType()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<object>, IEnumerable<int>>(cls => cls.DoCastingTest).Implement((m, source) => {
					m.Return(source.OfType<int>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			int[] result = tester.DoCastingTest(new object[] { "AAA", 123, true, 456 }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { 123, 456 }));
		}

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
		public void TestCast()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<object>, IEnumerable<int>>(cls => cls.DoCastingTest).Implement((m, source) => {
					m.Return(source.Cast<int>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();

			var input = new object[] { 123, 456, 789 };
			int[] result = tester.DoCastingTest(input).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { 123, 456, 789 }));
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestThenBy()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.OrderBy(s => s.Length()).ThenBy(s => s.First()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "CCCC", "ZZZ", "BBB", "AAAA" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "BBB", "ZZZ", "AAAA", "CCCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestThenByDescending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.OrderBy(s => s.Length()).ThenByDescending(s => s.First()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "EEEE", "ZZZ", "FFFF", "BBB", "DDDD", "VVV" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ZZZ", "VVV", "BBB", "FFFF", "EEEE", "DDDD" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSelect()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<object>, IEnumerable<int>>(cls => cls.DoCastingTest).Implement((m, source) => {
					m.Return(source.Cast<string>().Select(s => s.Length()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoCastingTest(new[] { "AAA", "BB", "C", "DDDD" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { 3, 2, 1, 4 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSelectMany()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.SelectMany(s => s.Split(";")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "A1;A2;A3", "B1;B2", "C" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "A1", "A2", "A3", "B1", "B2", "C" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestToDictionary()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					Static.Prop(() => OutputDictionary).Assign(source.ToDictionary(s => s.Substring(m.Const(0), m.Const(1))));
					m.ReturnConst(null);
				});

			OutputDictionary = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			tester.DoTest(new[] { "A11", "B22", "C33" });

			//-- Assert

			Assert.That(OutputDictionary, Is.Not.Null);
			CollectionAssert.AreEquivalent(OutputDictionary.Keys, new[] { "A", "B", "C" });
			Assert.That(OutputDictionary["A"], Is.EqualTo("A11"));
			Assert.That(OutputDictionary["B"], Is.EqualTo("B22"));
			Assert.That(OutputDictionary["C"], Is.EqualTo("C33"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestConcat()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest)
					.Implement((m, first, second) => {
						m.Return(first.Concat(second));
					});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "111", "222", "333" }, new[] { "444", "555" });

			//-- Assert

			CollectionAssert.AreEquivalent(result, new[] { "111", "222", "333", "444", "555" });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestContains()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, bool>(cls => cls.DoBooleanTest).Implement((m, source) => {
					m.Return(source.Contains(m.Const("ABC")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new[] { "UUU", "VVV" });
			var result2 = tester.DoBooleanTest(new[] { "ABC", "DEF" });

			//-- Assert

			Assert.IsFalse(result1);
			Assert.IsTrue(result2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<int>>(cls => cls.DoCastingTest).Implement((m, source) => {
					m.Return(m.NewArray<int>(values: source.Count()));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoCastingTest(new[] { "A", "B" });
			var result2 = tester.DoCastingTest(new[] { "C", "D", "E", "F" });

			//-- Assert

			Assert.That(result1.Single(), Is.EqualTo(2));
			Assert.That(result2.Single(), Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDefaultIfEmpty()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.DefaultIfEmpty());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(new[] { "ABC", "DEF" });
			var result2 = tester.DoTest(new string[0]);

			//-- Assert

			Assert.That(result1, Is.EqualTo(new[] { "ABC", "DEF" }));
			Assert.That(result2, Is.EqualTo(new string[] { null }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDefaultIfEmptyWithDefaultValue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.DefaultIfEmpty(m.Const("ZZZ")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(new[] { "ABC", "DEF" });
			var result2 = tester.DoTest(new string[0]);

			//-- Assert

			Assert.That(result1, Is.EqualTo(new[] { "ABC", "DEF" }));
			Assert.That(result2, Is.EqualTo(new string[] { "ZZZ" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDistinct()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.Distinct());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(new[] { "A", "B", "C" });
			var result2 = tester.DoTest(new[] { "A", "B", "A", "C", "B" });

			//-- Assert

			Assert.That(result1, Is.EqualTo(new[] { "A", "B", "C" }));
			Assert.That(result2, Is.EqualTo(new[] { "A", "B", "C" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestElementAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.ElementAt(m.Const(2)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "A", "B", "C", "D" });

			//-- Assert

			Assert.That(result, Is.EqualTo("C"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestElementAtOrDefault()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.ElementAtOrDefault(m.Const(2)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new[] { "A", "B", "C", "D" });
			var result2 = tester.DoStringTest(new[] { "A", "B" });

			//-- Assert

			Assert.That(result1, Is.EqualTo("C"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestExcept()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					m.Return(first.Except(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "A", "B", "C", "D" }, new[] { "A", "D" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "B", "C" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFirst()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.First());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(result, Is.EqualTo("A"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFirstWithPredicate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.First(s => s.Length() > 1));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "A", "BB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo("BB"));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFirstOrDefault()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.FirstOrDefault());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new[] { "A", "B", "C" });
			var result2 = tester.DoStringTest(new string[0]);

			//-- Assert

			Assert.That(result1, Is.EqualTo("A"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFirstOrDefaultWithPredicate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.FirstOrDefault(s => s.Length() > 1));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new[] { "A", "BB", "CCC" });
			var result2 = tester.DoStringTest(new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(result1, Is.EqualTo("BB"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIntersect()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					m.Return(first.Intersect(second));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoBinaryTest(new[] { "A", "B", "C", "D" }, new[] { "A", "D" });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "A", "D" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLast()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.Last());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(result, Is.EqualTo("C"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastWithPredicate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.Last(s => s.Length() < 3));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(new[] { "A", "BB", "CCC" });

			//-- Assert

			Assert.That(result, Is.EqualTo("BB"));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastOrDefault()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.LastOrDefault());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new[] { "A", "B", "C" });
			var result2 = tester.DoStringTest(new string[0]);

			//-- Assert

			Assert.That(result1, Is.EqualTo("C"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastOrDefaultWithPredicate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, string>(cls => cls.DoStringTest).Implement((m, source) => {
					m.Return(source.LastOrDefault(s => s.Length() < 3));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new[] { "A", "BB", "CCC" });
			var result2 = tester.DoStringTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(result1, Is.EqualTo("BB"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestReverse()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.Reverse());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(new[] { "A", "B", "C" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "C", "B", "A" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSequenceEqual()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(cls => cls.DoBinaryTest).Implement((m, first, second) => {
					m.If(first.SequenceEqual(second)).Then(() => {
						m.Return(m.NewArray<string>(constantValues: "EQ"));		
					})
					.Else(() => {
						m.Return(m.NewArray<string>(constantValues: "NEQ"));
					});
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result1 = tester.DoBinaryTest(new[] { "A", "B", "C" }, new[] { "A", "B", "C" });
			var result2 = tester.DoBinaryTest(new[] { "A", "B", "C" }, new[] { "A", "B" });

			//-- Assert

			Assert.That(result1, Is.EqualTo(new[] { "EQ" }));
			Assert.That(result2, Is.EqualTo(new[] { "NEQ" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Dictionary<string, string> OutputDictionary { get; set; }
	}
}
