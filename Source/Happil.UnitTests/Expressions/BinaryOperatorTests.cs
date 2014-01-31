using System;
using System.Collections.Generic;
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
	}
}
