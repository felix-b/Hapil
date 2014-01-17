using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal static class BinaryOperators
	{
		static BinaryOperators()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorAdd<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				left.EmitTarget(il);
				left.EmitLoad(il);
				right.EmitTarget(il);
				right.EmitLoad(il);
				il.Emit(OpCodes.Add);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "+";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorSubtract<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				left.EmitTarget(il);
				left.EmitLoad(il);
				right.EmitTarget(il);
				right.EmitLoad(il);
				il.Emit(OpCodes.Sub);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "-";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorMultiply<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "*";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorDivide<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "/";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorModulo<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "%";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalAnd : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "&&";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalOr : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "||";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalXor : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "^^";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseAnd<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "&";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseOr<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "|";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseXor<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "^";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLeftShift<T> : IBinaryOperator<T, int>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<<";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorRightShift<T> : IBinaryOperator<T, int>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">>";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				var overloads = TypeOperators.GetOperators(left.OperandType);

				if ( overloads.OpEquality != null )
				{
					Helpers.EmitCall(il, null, overloads.OpEquality, left, right);
				}
				else
				{
					left.EmitTarget(il);
					left.EmitLoad(il);

					right.EmitTarget(il);
					right.EmitLoad(il);

					il.Emit(OpCodes.Ceq);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "==";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNotEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				left.EmitTarget(il);
				left.EmitLoad(il);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ceq);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "!=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThan<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				left.EmitTarget(il);
				left.EmitLoad(il);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Cgt);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThan<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				left.EmitTarget(il);
				left.EmitLoad(il);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Clt);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThanOrEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThanOrEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorCastOrThrow<T> : IBinaryOperator<T, Type>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<Type> right)
			{
				var typeConstant = (right as HappilConstant<Type>);

				if ( object.ReferenceEquals(typeConstant, null) )
				{
					throw new NotSupportedException("Cast type must be a constant type known in advance.");
				}

				left.EmitTarget(il);
				left.EmitLoad(il);

				var castType = TypeTemplate.Resolve(typeConstant.Value);

				if ( left.OperandType.IsValueType )
				{
					EmitValueTypeConversion(il, left.OperandType, castType);
				}
				else if ( !castType.IsValueType )
				{
					il.Emit(OpCodes.Castclass, castType);
				}
				else
				{
					throw NewConversionNotSupportedException();
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "cast-to";
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			private void EmitValueTypeConversion(ILGenerator il, Type fromType, Type toType)
			{
				if ( toType == typeof(object) )
				{
					il.Emit(OpCodes.Box, fromType);
					return;
				}
				
				if ( !toType.IsValueType )
				{
					throw NewConversionNotSupportedException();
				}

				var conversionType = (toType.IsEnum ? Enum.GetUnderlyingType(toType) : toType);

				if ( fromType != conversionType )
				{
					OpCode conversionInstruction;

					if ( !s_ValueTypeCastInstructions.TryGetValue(conversionType, out conversionInstruction) )
					{
						throw new NotSupportedException("Casting to specified value type is not supported.");
					}

					il.Emit(conversionInstruction);
				}
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			private Exception NewConversionNotSupportedException()
			{
				return new NotSupportedException("Specified type conversion is not supported.");
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			private static readonly Dictionary<Type, OpCode> s_ValueTypeCastInstructions = new Dictionary<Type, OpCode>() {
				{ typeof(sbyte), OpCodes.Conv_I1 },
				{ typeof(short), OpCodes.Conv_I2 },
				{ typeof(int), OpCodes.Conv_I4 },
				{ typeof(long), OpCodes.Conv_I8 },
				{ typeof(byte), OpCodes.Conv_U1 },
				{ typeof(ushort), OpCodes.Conv_U2 },
				{ typeof(uint), OpCodes.Conv_U4 },
				{ typeof(ulong), OpCodes.Conv_U8 },
				{ typeof(float), OpCodes.Conv_R4 },
				{ typeof(double), OpCodes.Conv_R8 }
			};
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorTryCast<T> : IBinaryOperator<T, Type>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<Type> right)
			{
				throw new InvalidOperationException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "as";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorAssign<T> : IBinaryOperator<T>, IDontLeaveValueOnStack
		{
			private bool m_ForceLeaveFalueOnStack = false;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				var nonPostfix = (left as INonPostfixNotation);

				left.EmitTarget(il);

				if ( nonPostfix != null )
				{
					nonPostfix.RightSide = right;
				}
				else
				{
					right.EmitTarget(il);
					right.EmitLoad(il);
				}

				left.EmitStore(il);

				if ( m_ForceLeaveFalueOnStack )
				{
					left.EmitTarget(il);
					left.EmitLoad(il);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IDontLeaveValueOnStack Members

			public void ForceLeaveFalueOnStack()
			{
				m_ForceLeaveFalueOnStack = true;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "=";
			}
		}
	}
}
