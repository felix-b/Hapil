using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				IntValue = 11,
				LongValue = 11,
				FloatValue = 11,
				DecimalValue = 22,
				DoubleValue = 33,
				TimeSpanValue = TimeSpan.FromDays(44)
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

		[Test, Ignore("Initializers are not yet implemented")]
		public void TestNewArrayWithInitializer()
		{
			////-- Arrange

			//DeriveClassFrom<AncestorRepository.StatementTester>()
			//	.DefaultConstructor()
			//	.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
			//		var arr = m.Local<int[]>();
			//		arr.Assign(m.NewArray<int>().Init(1, 2, 3, 4, 5));
			//		Static.Prop(() => OutputArray).Assign(arr);
			//		m.Return(arr.Length());
			//	});

			//OutputArray = null;

			////-- Act

			//var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			//var result = tester.DoTest(10);

			////-- Assert

			//Assert.That(result, Is.EqualTo(10));
			//Assert.That(OutputArray, Is.Not.Null);
			//Assert.That(OutputArray.Length, Is.EqualTo(10));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int[] OutputArray { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

	}
}
