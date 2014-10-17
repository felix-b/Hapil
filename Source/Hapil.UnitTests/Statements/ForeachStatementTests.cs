using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Hapil.UnitTests.Statements
{
	[TestFixture]
	public class ForeachStatementTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void TestForeachElementInNonEmptyCollection()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					m.ForeachElementIn(input).Do((loop, element) => {
						output.Add(element);
					});
				});

			var inputEnumerable = new int[] { 1, 2, 3, 4, 5 };
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList, Is.EqualTo(inputEnumerable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForeachElementInEmptyCollection()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					m.ForeachElementIn(input).Do((loop, element) => {
						output.Add(element);
					});
				});

			var inputEnumerable = new int[0];
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList.Count, Is.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForeachBreak()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					var num = m.Local<int>();

					m.Foreach(num).In(input).Do(loop => {
						output.Add(num);
						m.If(num == 10).Then(loop.Break);
					});
				});

			var inputEnumerable = new int[] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList, Is.EqualTo(new[] { 0, 2, 4, 6, 8, 10 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForeachContinue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					var num = m.Local<int>();

					m.Foreach(num).In(input).Do(loop => {
						m.If(num == 10).Then(loop.Continue);
						output.Add(num);
					});
				});

			var inputEnumerable = new int[] { 2, 5, 10, 15, 18 };
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList, Is.EqualTo(new[] { 2, 5, 15, 18 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForeachBreakFromTry()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					var num = m.Local<int>();

					m.Foreach(num).In(input).Do(loop => {
						m.Try(() => {
							m.If(num == 10).Then(loop.Break);
						})
						.Finally(() => {
							output.Add(num);
						});
					});
				});

			var inputEnumerable = new int[] { 0, 5, 10, 15, 20 };
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList, Is.EqualTo(new[] { 0, 5, 10 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestForeachContinueFromCatch()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester3>()
				.DefaultConstructor()
				.Method<IEnumerable<int>, IList<int>>(cls => cls.DoTest).Implement((m, input, output) => {
					var num = m.Local<int>();

					m.Foreach(num).In(input).Do(loop => {
						m.Try(() => {
							m.If(num == 10).Then(() => m.Throw<ExceptionRepository.TestExceptionOne>("10"));
						})
						.Catch<Exception>(e => {
							loop.Continue();
						});
						output.Add(num);
					});
				});

			var inputEnumerable = new int[] { 0, 5, 10, 15, 20 };
			var outputList = new List<int>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester3>().UsingDefaultConstructor();
			tester.DoTest(inputEnumerable, outputList);

			//-- Assert

			Assert.That(outputList, Is.EqualTo(new[] { 0, 5, 15, 20 }));
		}
	}
}
