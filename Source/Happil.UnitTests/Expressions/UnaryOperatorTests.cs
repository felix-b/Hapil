using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Operands;
using NUnit.Framework;
using InOut = Happil.UnitTests.AncestorRepository.OperatorInputOutput;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class UnaryOperatorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestLogicalNot()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.BooleanValue).Assign(!input.Prop(x => x.BooleanValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();

			var result1 = tester.Unary(new InOut { BooleanValue = true });
			var result2 = tester.Unary(new InOut { BooleanValue = false });

			//-- Assert

			Assert.That(result1.BooleanValue, Is.False);
			Assert.That(result2.BooleanValue, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestBitwiseNot()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());
					output.Prop(x => x.IntValue).Assign(~input.Prop(x => x.IntValue));
					output.Prop(x => x.UIntValue).Assign(~input.Prop(x => x.UIntValue));
					output.Prop(x => x.LongValue).Assign(~input.Prop(x => x.LongValue));
					output.Prop(x => x.ULongValue).Assign(~input.Prop(x => x.ULongValue));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut {
				IntValue = -0x010203,
				UIntValue = 0x010203,
				LongValue = -0x01020304L,
				ULongValue = 0x01020304L
			});

			//-- Assert

			Assert.That(result.IntValue, Is.EqualTo(~(-0x010203)));
			Assert.That(result.UIntValue, Is.EqualTo(~(uint)0x010203));
			Assert.That(result.LongValue, Is.EqualTo(~(-0x01020304L)));
			Assert.That(result.ULongValue, Is.EqualTo(~(ulong)0x01020304L));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestBitwiseNotWithUnsupportedType()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());
					try
					{
						output.Prop(x => x.FloatValue).Assign(~input.Prop(x => x.FloatValue));
						Assert.Fail("ArgumentException was expected.");
					}
					catch ( ArgumentException )
					{
					}
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestUnaryPlus()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.ShortValue).Assign(+input.Prop(x => x.ShortValue));
					output.Prop(x => x.IntValue).Assign(+input.Prop(x => x.IntValue));
					output.Prop(x => x.LongValue).Assign(+input.Prop(x => x.LongValue));
					output.Prop(x => x.FloatValue).Assign(+input.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(+input.Prop(x => x.DecimalValue));
					output.Prop(x => x.DoubleValue).Assign(+input.Prop(x => x.DoubleValue));
					output.Prop(x => x.TimeSpanValue).Assign(+input.Prop(x => x.TimeSpanValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut {
				ShortValue = -11,
				IntValue = -11,
				LongValue = -11,
				FloatValue = -11,
				DecimalValue = -22,
				DoubleValue = -33,
				TimeSpanValue = TimeSpan.FromDays(-44)
			});

			//-- Assert

			Assert.That(result.ShortValue, Is.EqualTo(-11));
			Assert.That(result.IntValue, Is.EqualTo(-11));
			Assert.That(result.LongValue, Is.EqualTo(-11));
			Assert.That(result.FloatValue, Is.EqualTo(-11));
			Assert.That(result.DecimalValue, Is.EqualTo(-22));
			Assert.That(result.DoubleValue, Is.EqualTo(-33));
			Assert.That(result.TimeSpanValue, Is.EqualTo(TimeSpan.FromDays(-44)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestNegation()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.ShortValue).Assign(-input.Prop(x => x.ShortValue));
					output.Prop(x => x.IntValue).Assign(-input.Prop(x => x.IntValue));
					output.Prop(x => x.LongValue).Assign(-input.Prop(x => x.LongValue));
					output.Prop(x => x.FloatValue).Assign(-input.Prop(x => x.FloatValue));
					output.Prop(x => x.DecimalValue).Assign(-input.Prop(x => x.DecimalValue));
					output.Prop(x => x.DoubleValue).Assign(-input.Prop(x => x.DoubleValue));
					output.Prop(x => x.TimeSpanValue).Assign(-input.Prop(x => x.TimeSpanValue));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut {
				ShortValue = 11,
				IntValue = 22,
				LongValue = 33,
				FloatValue = 44,
				DecimalValue = 55,
				DoubleValue = 66,
				TimeSpanValue = TimeSpan.FromDays(77)
			});

			//-- Assert

			Assert.That(result.ShortValue, Is.EqualTo(-11));
			Assert.That(result.IntValue, Is.EqualTo(-22));
			Assert.That(result.LongValue, Is.EqualTo(-33));
			Assert.That(result.FloatValue, Is.EqualTo(-44));
			Assert.That(result.DecimalValue, Is.EqualTo(-55));
			Assert.That(result.DoubleValue, Is.EqualTo(-66));
			Assert.That(result.TimeSpanValue, Is.EqualTo(TimeSpan.FromDays(-77)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPostfixPlusPlus_OperandsWithoutTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var v1 = m.Local(initialValue: input.Prop(x => x.ShortValue));
					var v2 = m.Local(initialValue: input.Prop(x => x.IntValue));
					var v3 = m.Local(initialValue: input.Prop(x => x.LongValue));
					var v4 = m.Local(initialValue: input.Prop(x => x.FloatValue));
					var v5 = m.Local(initialValue: input.Prop(x => x.DecimalValue));
					var v6 = m.Local(initialValue: input.Prop(x => x.DoubleValue));

					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.ShortValue).Assign(v1.PostfixPlusPlus());
					output.Prop(x => x.IntValue).Assign(v2.PostfixPlusPlus());
					output.Prop(x => x.LongValue).Assign(v3.PostfixPlusPlus());
					output.Prop(x => x.FloatValue).Assign(v4.PostfixPlusPlus());
					output.Prop(x => x.DecimalValue).Assign(v5.PostfixPlusPlus());
					output.Prop(x => x.DoubleValue).Assign(v6.PostfixPlusPlus());

					input.Prop(x => x.ShortValue).Assign(v1);
					input.Prop(x => x.IntValue).Assign(v2);
					input.Prop(x => x.LongValue).Assign(v3);
					input.Prop(x => x.FloatValue).Assign(v4);
					input.Prop(x => x.DecimalValue).Assign(v5);
					input.Prop(x => x.DoubleValue).Assign(v6);

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var testInput = new InOut {
				ShortValue = 10,
				IntValue = 20,
				LongValue = 30,
				FloatValue = 40,
				DecimalValue = 50,
				DoubleValue = 60
			};
			var result = tester.Unary(testInput);

			//-- Assert

			Assert.That(testInput.ShortValue, Is.EqualTo(11));
			Assert.That(testInput.IntValue, Is.EqualTo(21));
			Assert.That(testInput.LongValue, Is.EqualTo(31));
			Assert.That(testInput.FloatValue, Is.EqualTo(41));
			Assert.That(testInput.DecimalValue, Is.EqualTo(51));
			Assert.That(testInput.DoubleValue, Is.EqualTo(61));

			Assert.That(result.ShortValue, Is.EqualTo(10));
			Assert.That(result.IntValue, Is.EqualTo(20));
			Assert.That(result.LongValue, Is.EqualTo(30));
			Assert.That(result.FloatValue, Is.EqualTo(40));
			Assert.That(result.DecimalValue, Is.EqualTo(50));
			Assert.That(result.DoubleValue, Is.EqualTo(60));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPostfixPlusPlus_OperandsWithTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.ShortValue).Assign(input.Prop(x => x.ShortValue).PostfixPlusPlus());
					output.Prop(x => x.IntValue).Assign(input.Prop(x => x.IntValue).PostfixPlusPlus());
					output.Prop(x => x.LongValue).Assign(input.Prop(x => x.LongValue).PostfixPlusPlus());
					output.Prop(x => x.FloatValue).Assign(input.Prop(x => x.FloatValue).PostfixPlusPlus());
					output.Prop(x => x.DecimalValue).Assign(input.Prop(x => x.DecimalValue).PostfixPlusPlus());
					output.Prop(x => x.DoubleValue).Assign(input.Prop(x => x.DoubleValue).PostfixPlusPlus());

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var testInput = new InOut {
				ShortValue = 10,
				IntValue = 20,
				LongValue = 30,
				FloatValue = 40,
				DecimalValue = 50,
				DoubleValue = 60
			};
			var result = tester.Unary(testInput);

			//-- Assert

			Assert.That(testInput.ShortValue, Is.EqualTo(11));
			Assert.That(testInput.IntValue, Is.EqualTo(21));
			Assert.That(testInput.LongValue, Is.EqualTo(31));
			Assert.That(testInput.FloatValue, Is.EqualTo(41));
			Assert.That(testInput.DecimalValue, Is.EqualTo(51));
			Assert.That(testInput.DoubleValue, Is.EqualTo(61));

			Assert.That(result.ShortValue, Is.EqualTo(10));
			Assert.That(result.IntValue, Is.EqualTo(20));
			Assert.That(result.LongValue, Is.EqualTo(30));
			Assert.That(result.FloatValue, Is.EqualTo(40));
			Assert.That(result.DecimalValue, Is.EqualTo(50));
			Assert.That(result.DoubleValue, Is.EqualTo(60));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPostfixPlusPlus_OperandsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<int, float, decimal>(intf => (x, y, z) => intf.NumbersByRef(ref x, ref y, ref z)).Implement((m, x, y, z) => {
					x.PostfixPlusPlus();
					y.PostfixPlusPlus();
					z.PostfixPlusPlus();
				})
				.AllMethods().Throw<NotImplementedException>();

			int inputX = 10;
			float inputY = 20;
			decimal inputZ = 30;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			tester.NumbersByRef(ref inputX, ref inputY, ref inputZ);

			//-- Assert

			Assert.That(inputX, Is.EqualTo(11));
			Assert.That(inputY, Is.EqualTo(21));
			Assert.That(inputZ, Is.EqualTo(31));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPrefixPlusPlus_OperandsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<int, float, decimal>(intf => (x, y, z) => intf.NumbersByRef(ref x, ref y, ref z)).Implement((m, x, y, z) => {
					Prefix.PlusPlus(x);
					Prefix.PlusPlus(y);
					Prefix.PlusPlus(z);
				})
				.AllMethods().Throw<NotImplementedException>();

			int inputX = 10;
			float inputY = 20;
			decimal inputZ = 30;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			tester.NumbersByRef(ref inputX, ref inputY, ref inputZ);

			//-- Assert

			Assert.That(inputX, Is.EqualTo(11));
			Assert.That(inputY, Is.EqualTo(21));
			Assert.That(inputZ, Is.EqualTo(31));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPrefixPlusPlus_OperandsWithTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(Prefix.PlusPlus(input.Prop(x => x.IntValue)));
					output.Prop(x => x.FloatValue).Assign(Prefix.PlusPlus(input.Prop(x => x.FloatValue)));
					output.Prop(x => x.DecimalValue).Assign(Prefix.PlusPlus(input.Prop(x => x.DecimalValue)));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var testInput = new InOut {
				IntValue = 20,
				FloatValue = 40,
				DecimalValue = 50
			};
			var result = tester.Unary(testInput);

			//-- Assert

			Assert.That(testInput.IntValue, Is.EqualTo(21));
			Assert.That(testInput.FloatValue, Is.EqualTo(41));
			Assert.That(testInput.DecimalValue, Is.EqualTo(51));

			Assert.That(result.IntValue, Is.EqualTo(21));
			Assert.That(result.FloatValue, Is.EqualTo(41));
			Assert.That(result.DecimalValue, Is.EqualTo(51));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPrefixPlusPlus_OperandsWithoutTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var v1 = m.Local(initialValue: input.Prop(x => x.IntValue));
					var v2 = m.Local(initialValue: input.Prop(x => x.FloatValue));
					var v3 = m.Local(initialValue: input.Prop(x => x.DecimalValue));

					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(Prefix.PlusPlus(v1));
					output.Prop(x => x.FloatValue).Assign(Prefix.PlusPlus(v2));
					output.Prop(x => x.DecimalValue).Assign(Prefix.PlusPlus(v3));

					input.Prop(x => x.IntValue).Assign(v1);
					input.Prop(x => x.FloatValue).Assign(v2);
					input.Prop(x => x.DecimalValue).Assign(v3);

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var testInput = new InOut {
				IntValue = 20,
				FloatValue = 40,
				DecimalValue = 50
			};
			var result = tester.Unary(testInput);

			//-- Assert

			Assert.That(testInput.IntValue, Is.EqualTo(21));
			Assert.That(testInput.FloatValue, Is.EqualTo(41));
			Assert.That(testInput.DecimalValue, Is.EqualTo(51));

			Assert.That(result.IntValue, Is.EqualTo(21));
			Assert.That(result.FloatValue, Is.EqualTo(41));
			Assert.That(result.DecimalValue, Is.EqualTo(51));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPrefixMinusMinus_OperandsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<int, float, decimal>(intf => (x, y, z) => intf.NumbersByRef(ref x, ref y, ref z)).Implement((m, x, y, z) => {
					Prefix.MinusMinus(x);
					Prefix.MinusMinus(y);
					Prefix.MinusMinus(z);
				})
				.AllMethods().Throw<NotImplementedException>();

			int inputX = 10;
			float inputY = 20;
			decimal inputZ = 30;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			tester.NumbersByRef(ref inputX, ref inputY, ref inputZ);

			//-- Assert

			Assert.That(inputX, Is.EqualTo(9));
			Assert.That(inputY, Is.EqualTo(19));
			Assert.That(inputZ, Is.EqualTo(29));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPostfixMinusMinus_OperandsWithTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(intf => intf.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.IntValue).Assign(input.Prop(x => x.IntValue).PostfixMinusMinus());
					output.Prop(x => x.FloatValue).Assign(input.Prop(x => x.FloatValue).PostfixMinusMinus());
					output.Prop(x => x.DecimalValue).Assign(input.Prop(x => x.DecimalValue).PostfixMinusMinus());

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var testInput = new InOut {
				IntValue = 20,
				FloatValue = 40,
				DecimalValue = 50
			};
			var result = tester.Unary(testInput);

			//-- Assert

			Assert.That(testInput.IntValue, Is.EqualTo(19));
			Assert.That(testInput.FloatValue, Is.EqualTo(39));
			Assert.That(testInput.DecimalValue, Is.EqualTo(49));

			Assert.That(result.IntValue, Is.EqualTo(20));
			Assert.That(result.FloatValue, Is.EqualTo(40));
			Assert.That(result.DecimalValue, Is.EqualTo(50));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestNewArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var arr = m.Local<int[]>();
					arr.Assign(m.NewArray<int>(input));
					Static.Prop(() => OutputArray).Assign(arr);
					m.Return(arr.Length());
				});

			OutputArray = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result = tester.DoTest(10);

			//-- Assert

			Assert.That(result, Is.EqualTo(10));
			Assert.That(OutputArray, Is.Not.Null);
			Assert.That(OutputArray.Length, Is.EqualTo(10));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestNewArrayWithInitializer()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					Static.Prop(() => OutputArray).Assign(m.NewArray<int>(123, 456, 789));
					m.Return(0);
				});

			OutputArray = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(OutputArray, Is.Not.Null);
			Assert.That(OutputArray, Is.EqualTo(new[] { 123, 456, 789 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCall()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(cls => cls.Unary).Implement((m, input) => {
					var target = m.Local<string>(initialValue: input.Prop<string>(x => x.StringValue));
					var output = m.Local(m.New<InOut>());
					
					output.Prop<string>(x => x.StringValue).Assign(target.Func<string, string, string>(x => x.Replace, m.Const("@"), m.Const("#")));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut() { StringValue = "A@B@C" });

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result.StringValue, Is.EqualTo("A#B#C"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCallChain()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(cls => cls.Unary).Implement((m, input) => {
					var target = m.Local<string>(initialValue: input.Prop<string>(x => x.StringValue));
					var output = m.Local(m.New<InOut>());

					output.Prop<string>(x => x.StringValue).Assign(target
						.Func<string, string, string>(x => x.Replace, m.Const("@"), m.Const("#"))
						.Func<int, int, string>(x => x.Substring, m.Const(1), m.Const(3))
						.Func<string>(x => x.ToLower)
						.Func<int, char, string>(x => x.PadLeft, m.Const(10), m.Const('Z')));
					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut() { StringValue = "A@B@C" });

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result.StringValue, Is.EqualTo("ZZZZZZZ#b#"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCallChainMixedWithProperties()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOperatorTester>()
				.Method<InOut, InOut>(cls => cls.Unary).Implement((m, input) => {
					var output = m.Local(m.New<InOut>());

					output.Prop(x => x.StringValue).Assign(input
						.Prop(x => x.StringValue)
						.Func<int, string>(x => x.PadRight, m.Const(15))
						.Prop(x => x.Length)
						.Func<string>(x => x.ToString));

					m.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.IOperatorTester>().UsingDefaultConstructor();
			var result = tester.Unary(new InOut() { StringValue = "ABC" });

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result.StringValue, Is.EqualTo("15"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int[] OutputArray { get; set; }
	}
}
