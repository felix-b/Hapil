using System;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class FluentCSharpTests
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

			Field<bool> result1 = new Field<bool>("result1");
			Field<bool> result2 = new Field<bool>("result2");

			result1.Assign(int1 > int2);
			result2.Assign((int1 > int2) && (int2 > int3));
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
				return new Expression<TCast>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<T> operator +(Operand<T> x, Operand<T> y)
			{
				return new Expression<T>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator |(Operand<T> x, Operand<T> y)
			{
				return new Expression<bool>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator &(Operand<T> x, Operand<T> y)
			{
				return new Expression<bool>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static bool operator true(Operand<T> x)
			{
				return false;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static bool operator false(Operand<T> x)
			{
				return true;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator >(Operand<T> x, Operand<T> y)
			{
				return new Expression<bool>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Operand<bool> operator <(Operand<T> x, Operand<T> y)
			{
				return new Expression<bool>();
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
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Expression<T> : Operand<T>, IOperand<T>
		{
		}
	}
}
