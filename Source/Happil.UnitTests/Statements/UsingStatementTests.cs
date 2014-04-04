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
	public class UsingStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestUsing_NoException_DisposeCalled()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));
					output.Add(m.Const("BEFORE-USING"));

					m.Using(Static.Prop(() => InputDisposable)).Do(() => {
						output.Add(m.Const("INSIDE-USING"));
					});

					output.Add(m.Const("AFTER-USING"));
					m.Return(0);
				});

			InputDisposable = new TestDisposable();
			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(111);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"BEFORE-USING", "INSIDE-USING", "DISPOSE", "AFTER-USING",
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestUsing_ThrowException_DisposeCalled()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));
					output.Add(m.Const("BEFORE-USING"));

					m.Using(Static.Prop(() => InputDisposable)).Do(() => {
						output.Add(m.Const("INSIDE-USING"));
						m.Throw<ExceptionRepository.TestExceptionOne>("222");
					});

					output.Add(m.Const("AFTER-USING"));
					m.Return(0);
				});

			InputDisposable = new TestDisposable();
			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			try
			{
				tester.DoTest(111);
				Assert.Fail("Expected TestExceptionOne");
			}
			catch ( ExceptionRepository.TestExceptionOne )
			{
			}

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"BEFORE-USING", "INSIDE-USING", "DISPOSE"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestUsing_NullDisposable_DoNotThrow()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var output = m.Local(initialValue: Static.Prop(() => OutputList));
					output.Add(m.Const("BEFORE-USING"));

					m.Using(Static.Prop(() => InputDisposable)).Do(() => {
						output.Add(m.Const("INSIDE-USING"));
					});

					output.Add(m.Const("AFTER-USING"));
					m.Return(0);
				});

			InputDisposable = null;
			OutputList = new List<string>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(111);

			//-- Assert

			Assert.That(OutputList, Is.EqualTo(new[] {
				"BEFORE-USING", "INSIDE-USING", "AFTER-USING",
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IDisposable InputDisposable { get; set; }
		public static List<string> OutputList { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestDisposable : IDisposable
		{
			public void Dispose()
			{
				OutputList.Add("DISPOSE");
			}
		}
	}
}
