using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class LambdaDelegateTests : ClassPerTestCaseFixtureBase
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
		public void TestPassingDelegateReference()
		{
			//-- Arrange

			HappilField<Func<string, bool>> predicateField;

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

			HappilField<Func<string, bool>> predicateField;

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
					m.ReturnConst(null);
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

			HappilField<MyAccumulator> accumulatorField;

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
					m.ReturnConst(null);
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
