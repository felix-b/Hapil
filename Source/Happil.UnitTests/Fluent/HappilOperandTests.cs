using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class HappilOperandTests
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private IHappilClassBody<TestBase> m_ClassBody;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule("HappilOperandTests");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_ClassBody = m_Module.DefineClass("_" + Guid.NewGuid().ToString("X")).Inherit<TestBase>();
			m_Class = ((IHappilClassDefinitionInternals)m_ClassBody).HappilClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ArithmeticalExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");

			//-- Act

			var expression = field1 + field2;

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<int, int>>());
			Assert.That(expression.ToString(), Is.EqualTo("Expr<Int32>{Field{f1} + Field{f2}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ComparisonExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");

			//-- Act

			var expression = (field1 < field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<int, bool>>());
			Assert.That(expression.ToString(), Is.EqualTo("Expr<Boolean>{Field{f1} < Field{f2}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalAndExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");
			var field3 = new HappilField<int>("f3");
			var const4 = new HappilConstant<int>(123);

			//-- Act

			var expression = ((field1 < field2) && (field3 > const4));

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<bool, bool>>());
			Assert.That(
				expression.ToString(), 
				Is.EqualTo("Expr<Boolean>{Expr<Boolean>{Field{f1} < Field{f2}} && Expr<Boolean>{Field{f3} > Const<Int32>{123}}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalOrExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");
			var field3 = new HappilField<int>("f3");
			var const4 = new HappilConstant<int>(123);

			//-- Act

			var expression = ((field1 < field2) || (field3 > const4));

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Boolean>{Expr<Boolean>{Field{f1} < Field{f2}} || Expr<Boolean>{Field{f3} > Const<Int32>{123}}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalXorExpression()
		{
			//-- Arrange

			var field1 = new HappilField<bool>("f1");
			var field2 = new HappilField<bool>("f2");

			//-- Act

			var expression = (field1 ^ field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Boolean>{Field{f1} ^^ Field{f2}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BinaryXorExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");

			//-- Act

			var expression = (field1 ^ field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int32>{Field{f1} ^ Field{f2}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalNotExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");

			//-- Act

			var expression = !(field1 < field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Boolean>{! Expr<Boolean>{Field{f1} < Field{f2}}}"));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ComplexLogicalExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<int>("f2");
			var field3 = new HappilField<int>("f3");
			var const4 = new HappilConstant<int>(123);

			//-- Act

			var expression = !(field1 < field2 && field2 < field3 || field3 > const4);

			//-- Assert

			const string expectedExpression = 
				"Expr<Boolean>{! " +
					"Expr<Boolean>{" +
						"Expr<Boolean>{" +
							"Expr<Boolean>{Field{f1} < Field{f2}}" +
							" && " +
							"Expr<Boolean>{Field{f2} < Field{f3}}" +
						"}" +
						" || " +
						"Expr<Boolean>{Field{f3} > Const<Int32>{123}}" +
					"}"	+
				"}";

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<bool, bool>>());
			Assert.That(expression.ToString(), Is.EqualTo(expectedExpression));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExplicitCastExpression()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");
			var field2 = new HappilField<long>("f2");

			//-- Act

			var expression = (field1.CastTo<long>() + field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<long, long>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int64>{Expr<Int64>{Field{f1} cast-to Const<Type>{System.Int64}} + Field{f2}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TryCastExpression()
		{
			//-- Arrange

			var field1 = new HappilField<string>("f1");
			var field2 = new HappilField<object>("f2");

			//-- Act

			var expression = (field1 + field2.As<string>());

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilBinaryExpression<string, string>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<String>{Field{f1} + Expr<String>{Field{f2} as Const<Type>{System.String}}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PostfixPlusPlusOperator()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");

			//-- Act

			var expression = field1.PostfixPlusPlus();

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int32>{Field{f1} ++}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PostfixMinusMinusOperator()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");

			//-- Act

			var expression = field1.PostfixMinusMinus();

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int32>{Field{f1} --}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PrefixPlusPlusOperator()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");

			//-- Act

			var expression = Prefix.PlusPlus(field1);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int32>{++ Field{f1}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PrefixMinusMinusOperator()
		{
			//-- Arrange

			var field1 = new HappilField<int>("f1");

			//-- Act

			var expression = Prefix.MinusMinus(field1);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<HappilUnaryExpression<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("Expr<Int32>{-- Field{f1}}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not yet implemented")]
		public void InvokeVoidMethodPassesCompilation()
		{
			//-- Arrange

			var field1 = m_ClassBody.Field<TestBase>("m_Next");

			//-- Act

			field1.Invoke(x => x.VoidMethod);
			field1.Invoke(x => x.VoidMethodWithOneArg, new HappilConstant<int>(123));
			field1.Invoke(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<string>("ABC"));
			field1.Invoke(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<DateTime>(DateTime.Now));
			field1.Invoke(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<string>("ABC"), new HappilConstant<DateTime>(DateTime.Now));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestBase
		{
			public virtual void TestMethod()
			{
			}
			public void VoidMethod()
			{
			}
			public void VoidMethodWithOneArg(int number)
			{
			}
			public void VoidMethodWithManyArgs(int number, string text)
			{
			}
			public void VoidMethodWithManyArgs(int number, DateTime date)
			{
			}
			public void VoidMethodWithManyArgs(int number, string text, DateTime date)
			{
			}
			public int FuncWithNoArgs()
			{
				return 123;
			}
			public int FuncWithOneArg(int number)
			{
				return 123;
			}
			public int FuncWithManyArgs(int number, string text)
			{
				return 123;
			}
			public int FuncWithManyArgs(int number, string text, DateTime date)
			{
				return 123;
			}
		}
	}
}
