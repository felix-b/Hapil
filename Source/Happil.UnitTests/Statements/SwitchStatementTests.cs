using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.UnitTests.Statements
{
	[TestFixture]
	public class SwitchStatementTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestJumpTableNoAdjustment()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var result = m.Local<int>(initialValueConst: 9999);

					m.Switch(input)
						.Case(0).Do(() => result.AssignConst(1000))
						.Case(1).Do(() => result.AssignConst(1111))
						.Case(2).Do(() => result.AssignConst(2222));

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 0, 1, 2, 123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 1000, 1111, 2222, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestJumpTableWithDefaultNoAdjustment()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var result = m.Local<int>();
					
					m.Switch(input)
						.Case(0).Do(() => result.AssignConst(1000))
						.Case(1).Do(() => result.AssignConst(1111))
						.Case(2).Do(() => result.AssignConst(2222))
						.Default(() => result.AssignConst(9999));

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 0, 1, 2, 123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert
		
			Assert.That(outputs, Is.EqualTo(new[] { 1000, 1111, 2222, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestJumpTableWithAdjustment()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var result = m.Local<int>();

					m.Switch(input)
						.Case(10).Do(() => result.AssignConst(1000))
						.Case(11).Do(() => result.AssignConst(1111))
						.Case(12).Do(() => result.AssignConst(2222))
						.Default(() => result.AssignConst(9999));

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 10, 11, 12, 123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 1000, 1111, 2222, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestJumpTableWithNegativeAdjustment()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var result = m.Local<int>();

					m.Switch(input)
						.Case(-10).Do(() => result.AssignConst(1000))
						.Case(-11).Do(() => result.AssignConst(1111))
						.Case(-12).Do(() => result.AssignConst(2222))
						.Default(() => result.AssignConst(9999));

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { -10, -11, -12, -123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 1000, 1111, 2222, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestJumpTableWithAdjustmentAndGaps()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var result = m.Local<int>();

					m.Switch(input)
						.Case(10).Do(() => result.AssignConst(1000))
						.Case(12).Do(() => result.AssignConst(2222))
						.Case(14).Do(() => result.AssignConst(4444))
						.Default(() => result.AssignConst(9999));

					m.Return(result);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 9, 10, 11, 12, 13, 14, 15 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 9999, 1000, 9999, 2222, 9999, 4444, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestByteValue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var byteInput = m.Local<byte>(initialValue: input.CastTo<byte>());

					m.Switch(byteInput)
						.Case(10).Do(() => m.ReturnConst(1000))
						.Case(11).Do(() => m.ReturnConst(1111))
						.Case(12).Do(() => m.ReturnConst(2222));

					m.ReturnConst(9999);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 10, 11, 12, 123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 1000, 1111, 2222, 9999 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestEnumValue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, input) => {
					var enumInput = m.Local<DayOfWeek>(initialValue: input.CastTo<DayOfWeek>());

					m.Switch(enumInput)
						.Case(DayOfWeek.Monday)
							.Do(() => m.ReturnConst(111))
						.Case(DayOfWeek.Tuesday)
							.Do(() => m.ReturnConst(222))
						.Case(DayOfWeek.Thursday)
							.Do(() => m.ReturnConst(444));

					m.ReturnConst(999);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			var inputs = new[] { 0, 1, 2, 4, 3, 123 };
			var outputs = inputs.Select(tester.DoTest).ToArray();

			//-- Assert

			Assert.That(outputs, Is.EqualTo(new[] { 999, 111, 222, 444, 999, 999 }));
		}
	}
}
