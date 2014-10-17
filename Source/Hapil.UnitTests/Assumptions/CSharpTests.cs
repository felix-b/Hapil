using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Hapil.UnitTests.Assumptions
{
	[TestFixture]
	public class CSharpTests
	{
		[Test]
		public void CanAssignDerivedToBase()
		{
			IOperand<string> stringOperand = (Constant<string>)"ABC";
			IOperand<object> objectOperand = (Constant<string>)"XYZ";
			IAssignableOperand<object> objectField = new Field<object>("m_Obj");
			objectField.Assign(stringOperand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CompilerEnsuresTypeSafety()
		{
			IAssignableOperand<string> stringField = new Field<string>("m_Str");
			IOperand<object> objectValue = new Constant<string>("It's a string");
			
			// The following will fail compilation, though it could (sometimes) work at run time:
			// stringField.Assign(objectValue); 
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanOverloadArithmeticalOperators()
		{
			Constant<int> x = 123;
			Constant<int> y = 456;

			Field<int> result = new Field<int>("m_Result");
			result.Assign(x + y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanAssignFieldALiteralValue()
		{
			Field<int> result = new Field<int>("m_Int");
			result.Assign(123);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanAssignValueTypeToObject()
		{
			Field<object> objectField = new Field<object>("m_Obj");
			Field<int> intField = new Field<int>("m_Int");
			
			objectField.Assign(intField);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCastOperandTypes()
		{
			Constant<long> x = 123L;
			Constant<int> y = 456;

			Field<long> result = new Field<long>("m_Result");
			result.Assign(x + y.CastTo<long>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanOverloadLogicalOperators()
		{
			Field<int> int1 = new Field<int>("int1");
			Field<int> int2 = new Field<int>("int2");
			Field<int> int3 = new Field<int>("int3");
		
			int1.Assign(123);
			int2.Assign(456);
			int3.Assign(789);

			Field<bool> result = new Field<bool>("result1");
			
			var expr1 = result.Assign(int1 > int2);
			var expr2 = result.Assign((int1 > int2) && (int2 > int3));
			var expr3 = result.Assign((int1 > int2) || (int2 > int3));

			Console.WriteLine(expr1.ToString());
			Console.WriteLine(expr2.ToString());
			Console.WriteLine(expr3.ToString());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanExtractCallFromLambdaWithIntLocal()
		{
			//-- Arrange

			int x = 1234;
			Expression<Action> lambda = () => Console.WriteLine(x.ToString("#,###"));

			//-- Act

			var call1 = (MethodCallExpression)lambda.Body;
			var method1 = call1.Method;
			var argument1 = call1.Arguments[0];
			var call2 = (MethodCallExpression)argument1;
			var method12 = call2.Method;
			var argument2 = call2.Arguments[0];
			var argument2Value = ((ConstantExpression)argument2).Value;

			//-- Assert

			Assert.That(method1.Name, Is.EqualTo("WriteLine"));
			Assert.That(method1.DeclaringType, Is.SameAs(typeof(Console)));
			Assert.That(argument2Value, Is.EqualTo("#,###"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanEvaluateArgumentInLambdaExpression()
		{
			//-- Arrange

			int x = 1234;
			Expression<Action> lambda = () => Console.WriteLine(x.ToString("#,###"));

			//-- Act

			var call1 = (MethodCallExpression)lambda.Body;
			var method1 = call1.Method;
			var argument1 = call1.Arguments[0];

			Expression<Func<object>> argument1Lambda = Expression.Lambda<Func<object>>(argument1);
			var argument1Delegate = argument1Lambda.Compile();
			var argument1Value = argument1Delegate();

			//-- Assert

			Assert.That(argument1Value, Is.EqualTo("1,234"));
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//[Test]
		//public void CanExtractCallFromLambdaExpressionWithMethodInfo()
		//{
		//	//-- Arrange

		//	MethodInfo m = (MethodInfo)MethodInfo.GetCurrentMethod();
		//	Expression<Action> lambda = () => Console.WriteLine(m.Name);

		//	//-- Act

		//	var call1 = (MethodCallExpression)lambda.Body;
		//	var method1 = call1.Method;
		//	var argument1 = call1.Arguments[0];
		//	var member1 = (MemberExpression)argument1;

		//	//-- Assert

		//	Assert.That(method1.Name, Is.EqualTo("WriteLine"));
		//	Assert.That(method1.DeclaringType, Is.SameAs(typeof(Console)));
		//	Assert.That(member1.Member.Name, Is.EqualTo("Name"));
		//	Assert.That(member1.Member.DeclaringType, Is.SameAs(typeof(MemberInfo)));
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanExtractCallFromLambdaWithMethodInfoArgument()
		{
			//-- Arrange

			var methodInfo = (MethodInfo)MethodInfo.GetCurrentMethod();

			//-- Act & Assert

			ExtractCallFromLambdaWithMethodInfo(() => Console.WriteLine(methodInfo.Name));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ExtractCallFromLambdaWithMethodInfo(Expression<Action> lambda)
		{
			//-- Act

			var call1 = (MethodCallExpression)lambda.Body;
			var method1 = call1.Method;
			var argument1 = call1.Arguments[0];
			var member1 = (MemberExpression)argument1;
			var member1TargetField = (FieldInfo)(((MemberExpression)member1.Expression).Member);
			var member1Target = ((ConstantExpression)(((MemberExpression)member1.Expression).Expression)).Value;
			var member1Value = member1TargetField.GetValue(member1Target);

			//-- Assert

			Assert.That(method1.Name, Is.EqualTo("WriteLine"));
			Assert.That(method1.DeclaringType, Is.SameAs(typeof(Console)));
			Assert.That(member1.Member.Name, Is.EqualTo("Name"));
			Assert.That(member1.Member.DeclaringType, Is.SameAs(typeof(MemberInfo)));
			Assert.That(member1Value, Is.Not.Null);
			Assert.That(member1Value, Is.SameAs(typeof(CSharpTests).GetMethod("CanExtractCallFromLambdaWithMethodInfoArgument")));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private interface IOperand<out T>
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class Operand<T> : IOperand<T>
		{
			public Operand<TCast> CastTo<TCast>()
			{
				return new Expression<T, T, TCast>(this, this, "cast");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<T> operator +(Operand<T> x, Operand<T> y)
			{
				return new Expression<T, T, T>(x, y, "+");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator |(Operand<T> x, Operand<T> y)
			{
				return new Expression<T, T, bool>(x, y, "|");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator &(Operand<T> x, Operand<T> y)
			{
				return new Expression<T, T, bool>(x, y, "&");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static bool operator true(Operand<T> x)
			{
				return false;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static bool operator false(Operand<T> x)
			{
				return false;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator >(Operand<T> x, Operand<T> y)
			{
				return new Expression<T, T, bool>(x, y, ">");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator <(Operand<T> x, Operand<T> y)
			{
				return new Expression<T, T, bool>(x, y, "<");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private interface IAssignableOperand<T> : IOperand<T>
		{
			IOperand<T> Assign(IOperand<T> value);
			IOperand<T> Assign(Constant<T> value);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class AssignableOperand<T> : Operand<T>, IAssignableOperand<T>
		{
			#region IAssignableOperand<T> Members

			public IOperand<T> Assign(IOperand<T> value)
			{
				return value;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IOperand<T> Assign(Constant<T> value)
			{
				return value;
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Constant<T> : Operand<T>, IOperand<T>
		{
			private readonly T m_Value;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Constant(T value)
			{
				m_Value = value;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static implicit operator Constant<T>(T value)
			{
				return new Constant<T>(value);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				var isNull = object.ReferenceEquals(null, m_Value);
				return string.Format("Const<{0}>{{{1}}}", typeof(T).Name, isNull ? "null" : m_Value.ToString());
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Field<T> : AssignableOperand<T>
		{
			private readonly string m_FieldName;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Field(string fieldName)
			{
				m_FieldName = fieldName;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return string.Format("Field<{0}>{{{1}}}", typeof(T).Name, m_FieldName);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Expression<TLeft, TRight, TExpr> : Operand<TExpr>, IOperand<TExpr>
		{
			private readonly IOperand<TLeft> m_Left;
			private readonly IOperand<TRight> m_Right;
			private readonly string m_Operation;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Expression(IOperand<TLeft> left, IOperand<TRight> right, string operation)
			{
				m_Left = left;
				m_Right = right;
				m_Operation = operation;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return string.Format(
					"Expr<{0}>{{({1}) {2} ({3})}}", 
					typeof(TExpr).Name,
					m_Left != null ? m_Left.ToString() : "null",
					m_Operation,
					m_Right != null ? m_Right.ToString() : "null");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IOperand<TLeft> Left
			{
				get { return m_Left; }
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public IOperand<TRight> Right
			{
				get { return m_Right; }
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public string Operation
			{
				get { return m_Operation; }
			}
		}
	}
}
