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
	public class ThrowStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		[ExpectedException(typeof(ExceptionRepository.TestExceptionOne), ExpectedMessage = "TestThrowException")]
		public void TestThrowException()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					m.Throw<ExceptionRepository.TestExceptionOne>("TestThrowException");
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			tester.DoTest(0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Could not find constructor", MatchType = MessageMatch.Contains)]
		public void TestThrowExceptionConstructorNotFound()
		{
			//-- Act

			try
			{
				DeriveClassFrom<AncestorRepository.StatementTester>()
					.DefaultConstructor()
					.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
						m.Return(0);
						m.Throw<ExceptionRepository.TestExceptionDefaultCtor>("TestThrowExceptionConstructorNotFound");
					});
			}
			finally
			{
				CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestReThrowException()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					m.Try(() => {
						m.Throw<ExceptionRepository.TestExceptionOne>("TestThrowException");
					})
					.Catch<Exception>(e => {
						Static.Prop(() => OutputException).Assign(e);
						m.Throw();
					});
				});

			Exception caughtException = null;
			OutputException = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			try
			{
				tester.DoTest(0);
			}
			catch ( ExceptionRepository.TestExceptionOne exc )
			{
				caughtException = exc;
			}

			//-- Assert

			Assert.That(OutputException, Is.Not.Null);
			Assert.That(caughtException, Is.Not.Null);
			Assert.That(caughtException, Is.SameAs(OutputException));
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Exception OutputException { get; set; }
	}
}
