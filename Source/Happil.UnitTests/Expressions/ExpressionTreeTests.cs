using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Operands;
using Happil.Statements;
using NUnit.Framework;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class ExpressionTreeTests
	{
		private StatementScope m_StatementScope;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_StatementScope = new StatementScope(null, null, new StatementBlock());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			m_StatementScope.Dispose();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ArithmeticalExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");

			//-- Act

			var expression = field1 + field2;

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<int, int>>());
			Assert.That(expression.ToString(), Is.EqualTo("[Field[f1] + Field[f2]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ComparisonExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");

			//-- Act

			var expression = (field1 < field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<int, bool>>());
			Assert.That(expression.ToString(), Is.EqualTo("[Field[f1] < Field[f2]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalAndExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");
			var field3 = CreateField<int>("f3");
			var const4 = new Constant<int>(123);

			//-- Act

			var expression = ((field1 < field2) && (field3 > const4));

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<bool, bool>>());
			Assert.That(
				expression.ToString(), 
				Is.EqualTo("[[Field[f1] < Field[f2]] && [Field[f3] > Const[123]]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalOrExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");
			var field3 = CreateField<int>("f3");
			var const4 = new Constant<int>(123);

			//-- Act

			var expression = ((field1 < field2) || (field3 > const4));

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[[Field[f1] < Field[f2]] || [Field[f3] > Const[123]]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalXorExpression()
		{
			//-- Arrange

			var field1 = CreateField<bool>("f1");
			var field2 = CreateField<bool>("f2");

			//-- Act

			var expression = (field1 ^ field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[Field[f1] ^^ Field[f2]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BinaryXorExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");

			//-- Act

			var expression = (field1 ^ field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[Field[f1] ^ Field[f2]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LogicalNotExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");

			//-- Act

			var expression = !(field1 < field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<bool, bool>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[![Field[f1] < Field[f2]]]"));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ComplexLogicalExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<int>("f2");
			var field3 = CreateField<int>("f3");
			var const4 = new Constant<int>(123);

			//-- Act

			var expression = !(field1 < field2 && field2 < field3 || field3 > const4);

			//-- Assert

			const string expectedExpression = 
				"[!" +
					"[" +
						"[" +
							"[Field[f1] < Field[f2]]" +
							" && " +
							"[Field[f2] < Field[f3]]" +
						"]" +
						" || " +
						"[Field[f3] > Const[123]]" +
					"]"	+
				"]";

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<bool, bool>>());
			Assert.That(expression.ToString(), Is.EqualTo(expectedExpression));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExplicitCastExpression()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");
			var field2 = CreateField<long>("f2");

			//-- Act

			var expression = (field1.CastTo<long>() + field2);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<long, long>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[[Field[f1] cast-to Const[System.Int64]] + Field[f2]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TryCastExpression()
		{
			//-- Arrange

			var field1 = CreateField<string>("f1");
			var field2 = CreateField<object>("f2");

			//-- Act

			var expression = (field1 + field2.As<string>());

			//-- Assert

			Assert.That(expression, Is.InstanceOf<BinaryExpressionOperand<string, string>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[Field[f1] + [Field[f2] as Const[System.String]]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PostfixPlusPlusOperator()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");

			//-- Act

			var expression = field1.PostfixPlusPlus();

			//-- Assert

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[Field[f1]++]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PostfixMinusMinusOperator()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");

			//-- Act

			var expression = field1.PostfixMinusMinus();

			//-- Assert

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[Field[f1]--]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PrefixPlusPlusOperator()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");

			//-- Act

			var expression = Prefix.PlusPlus(field1);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[++Field[f1]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void PrefixMinusMinusOperator()
		{
			//-- Arrange

			var field1 = CreateField<int>("f1");

			//-- Act

			var expression = Prefix.MinusMinus(field1);

			//-- Assert

			Assert.That(expression, Is.InstanceOf<UnaryExpressionOperand<int, int>>());
			Assert.That(
				expression.ToString(),
				Is.EqualTo("[--Field[f1]]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TestOperand<T> CreateField<T>(string name)
		{
			return new TestOperand<T>(name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestOperand<T> : MutableOperand<T>
		{
			private readonly string m_Name;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestOperand(string name)
			{
				m_Name = name;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return string.Format("Field[{0}]", m_Name);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override OperandKind Kind
			{
				get
				{
					return OperandKind.Local;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
	
			protected override void OnEmitTarget(ILGenerator il)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnEmitLoad(ILGenerator il)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnEmitStore(ILGenerator il)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnEmitAddress(ILGenerator il)
			{
				throw new NotImplementedException();
			}
		}
	}
}
