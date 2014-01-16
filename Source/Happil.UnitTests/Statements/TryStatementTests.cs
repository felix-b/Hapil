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
	public class TryStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestTryCatchAllExceptions()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					m.Try(() => {
						m.If(input == 888).Then(() => m.Throw<ExceptionRepository.TestExceptionOne>("TEST888"));

						Static.Prop(() => OutputException).AssignConst(null);
						m.ReturnConst(111);
					})
					.Catch<Exception>(e => {
						Static.Prop(() => OutputException).Assign(e);
						m.ReturnConst(999);
					});

					m.ReturnConst(-999); // should never get here!
				});

			OutputException = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var result1 = tester.DoTest(123);
			var exception1 = OutputException;

			var result2 = tester.DoTest(888);
			var exception2 = OutputException;

			//-- Assert

			Assert.That(result1, Is.EqualTo(111));
			Assert.That(exception1, Is.Null);

			Assert.That(result2, Is.EqualTo(999));
			Assert.That(exception2, Is.InstanceOf<ExceptionRepository.TestExceptionOne>());
			Assert.That(exception2.Message, Is.EqualTo("TEST888"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTryCatchExceptionsByType()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					m.Try(() => {
						m.If(input == 55).Then(() => m.Throw<ExceptionRepository.TestExceptionOne>("TEST55"));
						m.If(input == 66).Then(() => m.Throw<ExceptionRepository.TestExceptionTwo>("TEST66"));

						Static.Prop(() => OutputException).AssignConst(null);
						m.ReturnConst(1111);
					})
					.Catch<ExceptionRepository.TestExceptionOne>(e => {
						Static.Prop(() => OutputException).Assign(e);
						m.ReturnConst(5555);
					})
					.Catch<Exception>(e => {
						Static.Prop(() => OutputException).Assign(e);
						m.ReturnConst(9999);
					});

					m.ReturnConst(-9999); // should never get here!
				});

			OutputException = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var result1 = tester.DoTest(123);
			var exception1 = OutputException;

			var result2 = tester.DoTest(55);
			var exception2 = OutputException;

			var result3 = tester.DoTest(66);
			var exception3 = OutputException;

			//-- Assert

			Assert.That(result1, Is.EqualTo(1111));
			Assert.That(exception1, Is.Null);

			Assert.That(result2, Is.EqualTo(5555));
			Assert.That(exception2, Is.InstanceOf<ExceptionRepository.TestExceptionOne>());

			Assert.That(result3, Is.EqualTo(9999));
			Assert.That(exception3, Is.InstanceOf<ExceptionRepository.TestExceptionTwo>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTryCatchFinally()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					m.Try(() => {
						m.If(input == 55).Then(() => m.Throw<ExceptionRepository.TestExceptionOne>("TEST55"));
						m.If(input == 66).Then(() => m.Throw<ExceptionRepository.TestExceptionTwo>("TEST66"));

						Static.Prop(() => OutputException).AssignConst(null);
						m.ReturnConst(1111);
					})
					.Catch<ExceptionRepository.TestExceptionOne>(e => {
						Static.Prop(() => OutputException).Assign(e);
						m.ReturnConst(5555);
					})
					.Finally(() => {
						Static.Prop(() => OutputString).AssignConst("FINALLY");
					});

					m.ReturnConst(-9999); // should never get here!
				});

			OutputException = null;
			OutputString = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var result1 = tester.DoTest(123);
			var exception1 = OutputException;
			var string1 = OutputString;

			var result2 = tester.DoTest(55);
			var exception2 = OutputException;
			var string2 = OutputString;

			try
			{
				tester.DoTest(66);
				Assert.Fail("Expected TestExceptionTwo");
			}
			catch ( ExceptionRepository.TestExceptionTwo )
			{
			}

			var string3 = OutputString;

			//-- Assert

			Assert.That(result1, Is.EqualTo(1111));
			Assert.That(exception1, Is.Null);
			Assert.That(string1, Is.EqualTo("FINALLY"));

			Assert.That(result2, Is.EqualTo(5555));
			Assert.That(exception2, Is.InstanceOf<ExceptionRepository.TestExceptionOne>());
			Assert.That(string2, Is.EqualTo("FINALLY"));

			Assert.That(string3, Is.EqualTo("FINALLY"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTryCatchFinallyNested()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));

					m.Try(() => {
						output.Add(m.Const("OUTER-TRY-START"));
						m.Try(() => {
							output.Add(m.Const("INNER-TRY"));
							m.If(input == 55).Then(() => m.Throw<ExceptionRepository.TestExceptionOne>("TEST55"));
						})
						.Finally(() => {
							output.Add(m.Const("INNER-FINALLY"));
						});
						output.Add(m.Const("OUTER-TRY-END"));
					})
					.Catch<ExceptionRepository.TestExceptionOne>(e => {
						output.Add(m.Const("OUTER-CATCH-E1"));
					})
					.Catch<Exception>(e => {
						output.Add(m.Const("OUTER-CATCH-*"));
					})
					.Finally(() => {
						output.Add(m.Const("OUTER-FINALLY"));
					});

					output.Add(m.Const("RETURN"));
					m.ReturnConst(0);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			OutputList = new List<string>();
			tester.DoTest(123);
			var output1 = OutputList;

			OutputList = new List<string>();
			tester.DoTest(55);
			var output2 = OutputList;

			//-- Assert

			Assert.That(output1, Is.EqualTo(new[] {
				"OUTER-TRY-START", "INNER-TRY", "INNER-FINALLY", "OUTER-TRY-END", "OUTER-FINALLY", "RETURN"
			}));

			Assert.That(output2, Is.EqualTo(new[] {
				"OUTER-TRY-START", "INNER-TRY", "INNER-FINALLY", "OUTER-CATCH-E1", "OUTER-FINALLY", "RETURN"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLoopBreakFromTry()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));

					m.While(input > 0).Do(loop => {
						output.Add(input.Func<string>(x => x.ToString));

						m.Try(() => {
							m.If(input == 11).Then(() => {
								output.Add(m.Const("BREAK"));
								loop.Break();
							});
						})
						.Finally(() => {
							output.Add(m.Const("FINALLY"));
						});

						input.Assign(input - 1);
					});

					m.ReturnConst(0);
				});

			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(13);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"13", "FINALLY", 
				"12", "FINALLY", 
				"11", "BREAK", "FINALLY"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLoopContinueFromTry()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));

					m.While(input > 10).Do(loop => {
						output.Add(input.Func<string>(x => x.ToString));

						m.Try(() => {
							m.If(input == 12).Then(() => {
								input.Assign(input - 1);
								output.Add(m.Const("CONTINUE"));
								loop.Continue();
							});
						})
						.Finally(() => {
							output.Add(m.Const("FINALLY"));
						});

						input.Assign(input - 1);
						output.Add(m.Const("END-LOOP"));
					});

					m.ReturnConst(0);
				});

			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(13);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"13", "FINALLY", "END-LOOP",
				"12", "CONTINUE", "FINALLY",
				"11", "FINALLY", "END-LOOP"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLoopBreakFromCatchWithFinally()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));

					m.While(input > 0).Do(loop => {
						output.Add(input.Func<string>(x => x.ToString));

						m.Try(() => {
							m.If(input == 11).Then(() => {
								output.Add(m.Const("THROW"));
								m.Throw<ExceptionRepository.TestExceptionOne>("11");
							});
						})
						.Catch<Exception>(e => {
							output.Add(m.Const("CATCH"));
							loop.Break();
						})
						.Finally(() => {
							output.Add(m.Const("FINALLY"));
						});

						input.Assign(input - 1);
						output.Add(m.Const("END-LOOP"));
					});

					m.ReturnConst(0);
				});

			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(13);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"13", "FINALLY", "END-LOOP",
				"12", "FINALLY", "END-LOOP",
				"11", "THROW", "CATCH", "FINALLY"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLoopContinueFromCatchWithFinally()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));

					m.While(input > 10).Do(loop => {
						output.Add(input.Func<string>(x => x.ToString));

						m.Try(() => {
							m.If(input == 12).Then(() => {
								output.Add(m.Const("THROW"));
								m.Throw<ExceptionRepository.TestExceptionOne>("12");
							});
						})
						.Catch<Exception>(e => {
							output.Add(m.Const("CATCH"));
							input.Assign(input - 1);
							loop.Continue();
						})
						.Finally(() => {
							output.Add(m.Const("FINALLY"));
						});

						input.Assign(input - 1);
						output.Add(m.Const("END-LOOP"));
					});

					m.ReturnConst(0);
				});

			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(13);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"13", "FINALLY", "END-LOOP",
				"12", "THROW", "CATCH", "FINALLY",
				"11", "FINALLY", "END-LOOP",
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Exception OutputException { get; set; }
		public static string OutputString { get; set; }
		public static List<string> OutputList { get; set; }
	}
}
