using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using NUnit.Framework;
using InOut = Happil.UnitTests.AncestorRepository.OperatorInputOutput;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class BinaryOperatorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestAddition()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) + in2.Prop(x => x.IntValue));
					output.Prop(x => x.FloatValue).Assign(in1.Prop(x => x.FloatValue) + in2.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(in1.Prop(x => x.DecimalValue) + in2.Prop(x => x.DecimalValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var input1 = new InOut {
				IntValue = 20,
				FloatValue = 30,
				DecimalValue = 40,
			};
			var input2 = new InOut {
				IntValue = 7,
				FloatValue = 8,
				DecimalValue = 9,
			};
			var result = tester.Binary(input1, input2);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(27));
			Assert.That(result.FloatValue, Is.EqualTo(38));
			Assert.That(result.DecimalValue, Is.EqualTo(49));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSubtraction()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) - in2.Prop(x => x.IntValue));
					output.Prop(x => x.FloatValue).Assign(in1.Prop(x => x.FloatValue) - in2.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(in1.Prop(x => x.DecimalValue) - in2.Prop(x => x.DecimalValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var input1 = new InOut {
				IntValue = 20,
				FloatValue = 30,
				DecimalValue = 40,
			};
			var input2 = new InOut {
				IntValue = 7,
				FloatValue = 8,
				DecimalValue = 9,
			};
			var result = tester.Binary(input1, input2);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(13));
			Assert.That(result.FloatValue, Is.EqualTo(22));
			Assert.That(result.DecimalValue, Is.EqualTo(31));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestMulitiply()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) * in2.Prop(x => x.IntValue));
					output.Prop(x => x.FloatValue).Assign(in1.Prop(x => x.FloatValue) * in2.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(in1.Prop(x => x.DecimalValue) * in2.Prop(x => x.DecimalValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var input1 = new InOut {
				IntValue = 2,
				FloatValue = 3,
				DecimalValue = 4,
			};
			var input2 = new InOut {
				IntValue = 3,
				FloatValue = 4,
				DecimalValue = 5,
			};
			var result = tester.Binary(input1, input2);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(6));
			Assert.That(result.FloatValue, Is.EqualTo(12));
			Assert.That(result.DecimalValue, Is.EqualTo(20));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestDivide()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) / in2.Prop(x => x.IntValue));
					output.Prop(x => x.FloatValue).Assign(in1.Prop(x => x.FloatValue) / in2.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(in1.Prop(x => x.DecimalValue) / in2.Prop(x => x.DecimalValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var input1 = new InOut {
				IntValue = 10,
				FloatValue = 20,
				DecimalValue = 30,
			};
			var input2 = new InOut {
				IntValue = 3,
				FloatValue = 10,
				DecimalValue = 20,
			};
			var result = tester.Binary(input1, input2);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(3));
			Assert.That(result.FloatValue, Is.EqualTo(2));
			Assert.That(result.DecimalValue, Is.EqualTo(1.5));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestModulus()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) % in2.Prop(x => x.IntValue));
					output.Prop(x => x.FloatValue).Assign(in1.Prop(x => x.FloatValue) % in2.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(in1.Prop(x => x.DecimalValue) % in2.Prop(x => x.DecimalValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var input1 = new InOut {
				IntValue = 10,
				FloatValue = 20,
				DecimalValue = 30,
			};
			var input2 = new InOut {
				IntValue = 3,
				FloatValue = 10,
				DecimalValue = 20,
			};
			var result = tester.Binary(input1, input2);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(1));
			Assert.That(result.FloatValue, Is.EqualTo(0));
			Assert.That(result.DecimalValue, Is.EqualTo(10));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLogicalAnd()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.BooleanValue).Assign(in1.Prop(x => x.BooleanValue) && in2.Prop(x => x.BooleanValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();

			var result1 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = true });
			var result2 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = false });
			var result3 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = true });
			var result4 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = false });

			//-- Assert

			Assert.That(result1.BooleanValue, Is.True);
			Assert.That(result2.BooleanValue, Is.False);
			Assert.That(result3.BooleanValue, Is.False);
			Assert.That(result4.BooleanValue, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLogicalOr()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.BooleanValue).Assign(in1.Prop(x => x.BooleanValue) || in2.Prop(x => x.BooleanValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();

			var result1 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = true });
			var result2 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = false });
			var result3 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = true });
			var result4 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = false });

			//-- Assert

			Assert.That(result1.BooleanValue, Is.True);
			Assert.That(result2.BooleanValue, Is.True);
			Assert.That(result3.BooleanValue, Is.True);
			Assert.That(result4.BooleanValue, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLogicalXor()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.BooleanValue).Assign(in1.Prop(x => x.BooleanValue) ^ in2.Prop(x => x.BooleanValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();

			var result1 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = true });
			var result2 = tester.Binary(new InOut { BooleanValue = true }, new InOut { BooleanValue = false });
			var result3 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = true });
			var result4 = tester.Binary(new InOut { BooleanValue = false }, new InOut { BooleanValue = false });

			//-- Assert

			Assert.That(result1.BooleanValue, Is.False);
			Assert.That(result2.BooleanValue, Is.True);
			Assert.That(result3.BooleanValue, Is.True);
			Assert.That(result4.BooleanValue, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestBitwiseAnd()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) & in2.Prop(x => x.IntValue));
					output.Prop(x => x.SqlIntValue).Assign(in1.Prop(x => x.SqlIntValue) & in2.Prop(x => x.SqlIntValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Binary(
				new InOut { IntValue = 0x0123, SqlIntValue = new SqlInt32(0x0123) }, 
				new InOut { IntValue = 0x1020, SqlIntValue = new SqlInt32(0x1020) });

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(0x0020));
			Assert.That(result.SqlIntValue.Value, Is.EqualTo(0x0020));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestBitwiseOr()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) | in2.Prop(x => x.IntValue));
					output.Prop(x => x.SqlIntValue).Assign(in1.Prop(x => x.SqlIntValue) | in2.Prop(x => x.SqlIntValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Binary(
				new InOut { IntValue = 0x0123, SqlIntValue = new SqlInt32(0x0123) },
				new InOut { IntValue = 0x1020, SqlIntValue = new SqlInt32(0x1020) });

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(0x1123));
			Assert.That(result.SqlIntValue.Value, Is.EqualTo(0x1123));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestBitwiseXor()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) ^ in2.Prop(x => x.IntValue));
					output.Prop(x => x.SqlIntValue).Assign(in1.Prop(x => x.SqlIntValue) ^ in2.Prop(x => x.SqlIntValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Binary(
				new InOut { IntValue = 0x1014, SqlIntValue = new SqlInt32(0x1014) },
				new InOut { IntValue = 0x1104, SqlIntValue = new SqlInt32(0x1104) });

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(0x0110));
			Assert.That(result.SqlIntValue.Value, Is.EqualTo(0x0110));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLeftShift()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) << 3);
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Binary(new InOut { IntValue = 128 }, right: null);
			
			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(1024));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRightShift()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut, InOut>(intf => intf.Binary).Implement((m, in1, in2) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(in1.Prop(x => x.IntValue) >> 3);
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Binary(new InOut { IntValue = 1024 }, right: null);

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(128));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestEqual()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IComparisonTester>()
				.Method<int, int, bool>(intf => intf.CompareInt).Implement((m, x, y) => {
					m.Return(x == y);
				})
				.Method<float, float, bool>(intf => intf.CompareFloat).Implement((m, x, y) => {
					m.Return(x == y);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IComparisonTester>().UsingDefaultConstructor();
			var resultInt1 = tester.CompareInt(123, 123);
			var resultInt2 = tester.CompareInt(123, 321);
			var resultFloat1 = tester.CompareFloat(123.5f, 123.5f);
			var resultFloat2 = tester.CompareFloat(123.5f, 321.5f);

			//-- Assert

			Assert.That(resultInt1, Is.True);
			Assert.That(resultInt2, Is.False);
			Assert.That(resultFloat1, Is.True);
			Assert.That(resultFloat2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestNotEqual()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IComparisonTester>()
				.Method<int, int, bool>(intf => intf.CompareInt).Implement((m, x, y) => {
					m.Return(x != y);
				})
				.Method<float, float, bool>(intf => intf.CompareFloat).Implement((m, x, y) => {
					m.Return(x != y);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IComparisonTester>().UsingDefaultConstructor();
			var resultInt1 = tester.CompareInt(123, 123);
			var resultInt2 = tester.CompareInt(123, 321);
			var resultFloat1 = tester.CompareFloat(123.5f, 123.5f);
			var resultFloat2 = tester.CompareFloat(123.5f, 321.5f);

			//-- Assert

			Assert.That(resultInt1, Is.False);
			Assert.That(resultInt2, Is.True);
			Assert.That(resultFloat1, Is.False);
			Assert.That(resultFloat2, Is.True);
		}
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLessThan()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IComparisonTester>()
				.Method<int, int, bool>(intf => intf.CompareInt).Implement((m, x, y) => {
					m.Return(x < y);
				})
				.Method<float, float, bool>(intf => intf.CompareFloat).Implement((m, x, y) => {
					m.Return(x < y);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IComparisonTester>().UsingDefaultConstructor();
			var resultInt1 = tester.CompareInt(100, 200);
			var resultInt2 = tester.CompareInt(100, 100);
			var resultInt3 = tester.CompareInt(200, 100);
			var resultFloat1 = tester.CompareFloat(123.4f, 123.5f);
			var resultFloat2 = tester.CompareFloat(123.5f, 123.5f);
			var resultFloat3 = tester.CompareFloat(123.5f, 123.4f);

			//-- Assert

			Assert.That(resultInt1, Is.True);
			Assert.That(resultInt2, Is.False);
			Assert.That(resultInt3, Is.False);
			Assert.That(resultFloat1, Is.True);
			Assert.That(resultFloat2, Is.False);
			Assert.That(resultFloat3, Is.False);
		}
	}
}
