using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class StatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestIfThen()
		{
			//-- Arrange

			DeriveClassFrom<StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValue: m.Const(999));
					
					m.If(input == m.Const(0)).Then(() => {
						result.AssignConst(111);
					});
					
					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester>()
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

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester>()
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

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester>()
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

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester2>()
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

			var tester = CreateClassInstanceAs<StatementTester2>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester2>()
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

			var tester = CreateClassInstanceAs<StatementTester2>().UsingDefaultConstructor();
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

			DeriveClassFrom<StatementTester2>()
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

			var tester = CreateClassInstanceAs<StatementTester2>().UsingDefaultConstructor();
			var result1 = tester.DoTest(50, 2);
			var result2 = tester.DoTest(1, 50);
			var result3 = tester.DoTest(1, 2);

			//-- Assert

			Assert.That(result1, Is.EqualTo(33));
			Assert.That(result2, Is.EqualTo(333));
			Assert.That(result3, Is.EqualTo(1333));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestWhile()
		{
			//-- Arrange

			DeriveClassFrom<StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsLeft = m.Local<int>(initialValue: input);
					var iterationsDone = m.Local<int>(initialValueConst: 0);

					m.While(iterationsLeft > 0).Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);
						iterationsLeft.Assign(iterationsLeft - 1);
					});

					m.Return(iterationsDone);
				});

			//-- Act

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(10);
			var result2 = tester.DoTest(20);

			//-- Assert

			Assert.That(result1, Is.EqualTo(10));
			Assert.That(result2, Is.EqualTo(20));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestWhileWithBreak()
		{
			//-- Arrange

			DeriveClassFrom<StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsDone = m.Local<int>(initialValueConst: 0);

					m.While(m.Const(true)).Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);

						m.If(iterationsDone == 5).Then(() => {
							loop.Break();
						});
					});

					m.Return(iterationsDone);
				});

			//-- Act

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(result, Is.EqualTo(5));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestWhileWithContinue()
		{
			//-- Arrange

			DeriveClassFrom<StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsDone = m.Local<int>(initialValueConst: 0);
					var counter = m.Local<int>(initialValueConst: 0);

					m.While(iterationsDone < input).Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);
						
						m.If(iterationsDone == 5).Then(() => {
							loop.Continue();
						});

						counter.Assign(counter + 1);
					});

					m.Return(counter);
				});

			//-- Act

			var tester = CreateClassInstanceAs<StatementTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(4);
			var result2 = tester.DoTest(10);
			var result3 = tester.DoTest(100);

			//-- Assert

			Assert.That(result1, Is.EqualTo(4));
			Assert.That(result2, Is.EqualTo(9));
			Assert.That(result3, Is.EqualTo(99));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester
		{
			public abstract int DoTest(int input);

			public virtual bool Predicate(int input)
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester2
		{
			public abstract int DoTest(int x, int y);
			
			public virtual bool Predicate(int x, int y)
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ExampleTester1 : StatementTester
		{
			public override int DoTest(int input)
			{
				if ( input == 0 )
				{
					return 111;
				}

				return 999;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int IfThenElseExample(int input)
			{
				var result = input;

				if ( result == 1 )
				{
					result = 11;
				}
				else if ( result == 11 )
				{
					result = 22;
				}
				else if ( result == 22 )
				{
					result = 33;
				}

				return result;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int WhileExample(int count)
			{
				var iterationsLeft = count;
				var iterationsDone = 0;

				while ( iterationsLeft > 0 )
				{
					iterationsDone++;
					iterationsLeft--;
				}

				return iterationsDone;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int CompareDifferentTypes()
			{
				string s1 = "A";
				string s2 = "B";

				if ( s1 == s2 )
				{
					double d1 = 123;
					double d2 = 456;

					if ( d1 != d2 )
					{
						DateTime dt1 = DateTime.Now;
						DateTime dt2 = DateTime.UtcNow;

						if ( dt1 == dt2 )
						{
							return 0;
						}
					}
				}

				return 1;
			}
		}
	}
}
