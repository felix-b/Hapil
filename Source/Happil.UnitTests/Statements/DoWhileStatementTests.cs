using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.UnitTests.Statements
{
	[TestFixture]
	public class DoWhileStatementTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void TestDoWhile()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsLeft = m.Local<int>(initialValue: input);
					var iterationsDone = m.Local<int>(initialValueConst: 0);

					m.Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);
						iterationsLeft.Assign(iterationsLeft - 1);
					}).While(iterationsLeft > 0);

					m.Return(iterationsDone);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(10);
			var result2 = tester.DoTest(20);

			//-- Assert

			Assert.That(result1, Is.EqualTo(10));
			Assert.That(result2, Is.EqualTo(20));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDoWhileWithBreak()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsDone = m.Local<int>(initialValueConst: 0);

					m.Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);

						m.If(iterationsDone == 5).Then(() => {
							loop.Break();
						});
					}).While(m.Const(true));

					m.Return(iterationsDone);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(result, Is.EqualTo(5));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDoWhileWithContinue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var iterationsDone = m.Local<int>(initialValueConst: 0);
					var counter = m.Local<int>(initialValueConst: 0);

					m.Do(loop => {
						iterationsDone.Assign(iterationsDone + 1);

						m.If(iterationsDone == 5).Then(() => {
							loop.Continue();
						});

						counter.Assign(counter + 1);
					}).While(iterationsDone < input);

					m.Return(counter);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result1 = tester.DoTest(4);
			var result2 = tester.DoTest(10);
			var result3 = tester.DoTest(100);

			//-- Assert

			Assert.That(result1, Is.EqualTo(4));
			Assert.That(result2, Is.EqualTo(9));
			Assert.That(result3, Is.EqualTo(99));
		}
	}
}
