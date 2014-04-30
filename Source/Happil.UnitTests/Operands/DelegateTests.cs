using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil;
using Happil.Operands;
using NUnit.Framework;

namespace Happil.UnitTests.Operands
{
	[TestFixture]
	public class DelegateTests : ClassPerTestCaseFixtureBase
	{
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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
		public void TestAnonymousDelegateFromStaticConstructor()
		{
			//-- Arrange

			Field<int[]> orderedArrayField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.StaticField<int[]>("s_OrderedArray", out orderedArrayField)
				.StaticConstructor(w => {
					var array = w.Local<int[]>();
					array.Assign(w.NewArray<int>(length: w.Const(5)));
					array.ElementAt(0).Assign(3);
					array.ElementAt(1).Assign(1);
					array.ElementAt(2).Assign(9);
					array.ElementAt(3).Assign(7);
					array.ElementAt(4).Assign(4);
					orderedArrayField.Assign(array.OrderBy(w.Delegate<int, int>((del, item) => del.Return(item))).ToArray());
				})
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					w.Return(
						/*w.NewArray<string>(length: w.Const(0)));*/
						orderedArrayField.Select(w.Delegate<int, string>((del, item) => del.Return(item.Func<string>(x => x.ToString))))
					);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			var result = tester.DoTest(null).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "1", "3", "4", "7", "9" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPassingDelegateReference()
		{
			//-- Arrange

			Field<Func<string, bool>> predicateField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.Field("m_Predicate", out predicateField)
				.Constructor<Func<string, bool>>((m, predicate) => {
					predicateField.Assign(predicate);
				})
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					m.Return(source.Where(predicateField));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingConstructor<Func<string, bool>>(s => s.StartsWith("W"));
			var result = tester.DoTest(new[] { "AAA", "WAA", "BBB", "WBB" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "WAA", "WBB" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestInvokingDelegateByReference()
		{
			//-- Arrange

			Field<Func<string, bool>> predicateField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.Field("m_Predicate", out predicateField)
				.Constructor<Func<string, bool>>((m, predicate) => {
					predicateField.Assign(predicate);
				})
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					var list = m.Local(initialValue: m.New<List<string>>());
					m.ForeachElementIn(source).Do((loop, item) => {
						m.If(predicateField.Invoke(item)).Then(() => list.Add(item));
					});
					m.Return(list);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingConstructor<Func<string, bool>>(s => s.StartsWith("W"));
			var result = tester.DoTest(new[] { "AAA", "WAA", "BBB", "WBB" }).ToArray();

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "WAA", "WBB" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDelegateToCompiledStaticMethod()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					var func = m.Local<Action<string>>(initialValueConst: StaticMethodToCall);
					m.ForeachElementIn(source).Do((loop, item) => {
						func.Invoke(item);
					});
					m.Return((string[])null);
				});

			Output = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingDefaultConstructor();
			tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(Output, Is.EqualTo(new[] { "AAA", "BBB", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDelegateToInstanceMethod()
		{
			//-- Arrange

			Field<MyAccumulator> accumulatorField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.Field<MyAccumulator>("m_Accumulator", out accumulatorField)
				.Constructor<MyAccumulator>((m, accumulator) => {
					accumulatorField.Assign(accumulator);
				})
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((m, source) => {
					var action = m.Local(initialValue: m.MakeDelegate<MyAccumulator, Action<string>>(accumulatorField, t => t.Accumulate));
					m.ForeachElementIn(source).Do((loop, item) => {
						action.Invoke(item);
					});
					m.Return((string[])null);
				});

			//-- Act

			var testAccumulator = new MyAccumulator();
			var tester = CreateClassInstanceAs<AncestorRepository.EnumerableTester>().UsingConstructor<MyAccumulator>(testAccumulator);
			tester.DoTest(new[] { "AAA", "BBB", "CCC" });

			//-- Assert

			Assert.That(testAccumulator.GetAccumulated(), Is.EqualTo(new[] { "AAA", "BBB", "CCC" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void StaticMethodToCall(string s)
		{
			Output.Add(s);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<string> Output { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MyAccumulator
		{
			private readonly List<string> m_Accumulated = new List<string>();

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Accumulate(string s)
			{
				m_Accumulated.Add(s);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string[] GetAccumulated()
			{
				return m_Accumulated.ToArray();
			}
		}

	}
}
