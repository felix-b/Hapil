using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.UnitTests.Statements
{
	[TestFixture]
	public class IfStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestIfThen()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValue: m.Const(999));

					m.If(input == m.Const(0)).Then(() => {
						result.AssignConst(111);
					});

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var outputValue1 = tester.DoTest(0);
			var outputValue2 = tester.DoTest(1);

			//-- Assert

			Assert.That(outputValue1, Is.EqualTo(111));
			Assert.That(outputValue2, Is.EqualTo(999));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfThenElse()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValue: m.Const(999));

					m.If(input == m.Const(0)).Then(() => {
						result.AssignConst(111);
					})
					.Else(() => {
						result.AssignConst(222);
					});

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var outputValue1 = tester.DoTest(0);
			var outputValue2 = tester.DoTest(1);

			//-- Assert

			Assert.That(outputValue1, Is.EqualTo(111));
			Assert.That(outputValue2, Is.EqualTo(222));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfThenElseIf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValue: input);

					m.If(result == m.Const(1)).Then(() => {
						result.AssignConst(11);
					})
					.ElseIf(result == m.Const(11)).Then(() => {
						result.AssignConst(22);
					})
					.ElseIf(result == m.Const(22)).Then(() => {
						result.AssignConst(33);
					});

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var outputValue1 = tester.DoTest(1);
			var outputValue2 = tester.DoTest(11);
			var outputValue3 = tester.DoTest(22);
			var outputValue4 = tester.DoTest(50);

			//-- Assert

			Assert.That(outputValue1, Is.EqualTo(11));
			Assert.That(outputValue2, Is.EqualTo(22));
			Assert.That(outputValue3, Is.EqualTo(33));
			Assert.That(outputValue4, Is.EqualTo(50));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfThenElseIfAndElse()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValue: m.Const(999));

					m.If(input == m.Const(1)).Then(() => {
						result.AssignConst(111);
					})
					.ElseIf(input == m.Const(2)).Then(() => {
						result.AssignConst(222);
					})
					.ElseIf(input == m.Const(3)).Then(() => {
						result.AssignConst(333);
					})
					.Else(() => {
						result.AssignConst(444);
					});

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var outputValue1 = tester.DoTest(1);
			var outputValue2 = tester.DoTest(2);
			var outputValue3 = tester.DoTest(3);
			var outputValue4 = tester.DoTest(123);

			//-- Assert

			Assert.That(outputValue1, Is.EqualTo(111));
			Assert.That(outputValue2, Is.EqualTo(222));
			Assert.That(outputValue3, Is.EqualTo(333));
			Assert.That(outputValue4, Is.EqualTo(444));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfNestedInsideIf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester2>()
				.DefaultConstructor()
				.Method<int, int, int>(cls => cls.DoTest).Implement((m, x, y) => {
					var half1 = m.Local<int>(initialValueConst: 11);
					var half2 = m.Local<int>(initialValueConst: 22);

					m.If(x == m.Const(1)).Then(() => {
						half1.AssignConst(111);
						half2.AssignConst(222);

						m.If(y == m.Const(2)).Then(() => {
							half1.AssignConst(1111);
							half2.AssignConst(2222);
						});
					});

					m.Return(half1 + half2);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester2>().UsingDefaultConstructor();
			var result1 = tester.DoTest(50, 2);
			var result2 = tester.DoTest(1, 50);
			var result3 = tester.DoTest(1, 2);

			//-- Assert

			Assert.That(result1, Is.EqualTo(33));
			Assert.That(result2, Is.EqualTo(333));
			Assert.That(result3, Is.EqualTo(3333));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfThenElseNestedInsideIf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester2>()
				.DefaultConstructor()
				.Method<int, int, int>(cls => cls.DoTest).Implement((m, x, y) => {
					var half1 = m.Local<int>(initialValueConst: 11);
					var half2 = m.Local<int>(initialValueConst: 22);

					m.If(x == m.Const(1)).Then(() => {
						half1.AssignConst(111);
						half2.AssignConst(222);

						m.If(y == m.Const(2)).Then(() => {
							half1.AssignConst(1111);
							half2.AssignConst(2222);
						})
						.Else(() => {
							half1.AssignConst(11111);
							half2.AssignConst(22222);
						});
					});

					m.Return(half1 + half2);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester2>().UsingDefaultConstructor();
			var result1 = tester.DoTest(50, 2);
			var result2 = tester.DoTest(1, 50);
			var result3 = tester.DoTest(1, 2);

			//-- Assert

			Assert.That(result1, Is.EqualTo(33));
			Assert.That(result2, Is.EqualTo(33333));
			Assert.That(result3, Is.EqualTo(3333));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIfNestedInsideIfMixedWithStatements()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester2>()
				.DefaultConstructor()
				.Method<int, int, int>(cls => cls.DoTest).Implement((m, x, y) => {
					var half1 = m.Local<int>(initialValueConst: 11);
					var half2 = m.Local<int>(initialValueConst: 22);

					m.If(x == m.Const(1)).Then(() => {
						half1.AssignConst(111);

						m.If(y == m.Const(2)).Then(() => {
							half1.AssignConst(1111);
							half2.AssignConst(2222);
						});

						half2.AssignConst(222);
					});

					m.Return(half1 + half2);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester2>().UsingDefaultConstructor();
			var result1 = tester.DoTest(50, 2);
			var result2 = tester.DoTest(1, 50);
			var result3 = tester.DoTest(1, 2);

			//-- Assert

			Assert.That(result1, Is.EqualTo(33));
			Assert.That(result2, Is.EqualTo(333));
			Assert.That(result3, Is.EqualTo(1333));
		}
	}
}
