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
	public class OperandTests
	{
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
	}
}
