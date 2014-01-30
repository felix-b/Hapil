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
				var overloads = TypeOperators.GetOperators(left.OperandType);

				if ( overloads.OpAddition != null )
				{
					Helpers.EmitCall(il, null, overloads.OpAddition, left, right);
				}
				else
				{
					left.EmitTarget(il);
					left.EmitLoad(il);
					
					right.EmitTarget(il);
					right.EmitLoad(il);
					
					il.Emit(OpCodes.Add);
				}
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
				var falseLabel = il.DefineLabel();
				var endLabel = il.DefineLabel();

				left.EmitTarget(il);
				left.EmitLoad(il);

				il.Emit(OpCodes.Brfalse, falseLabel);

				right.EmitTarget(il);
				right.EmitLoad(il);
				
				il.Emit(OpCodes.Br, endLabel);

				il.MarkLabel(falseLabel);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Br, endLabel);

				il.MarkLabel(endLabel);
				il.Emit(OpCodes.Nop);
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
				left.EmitTarget(il);
				left.EmitLoad(il);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Clt);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ceq);
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

				var castType = TypeTemplate.Resolve(typeConstant.Value);

				left.EmitTarget(il);
				left.EmitLoad(il);

				Helpers.EmitCast(il, left.OperandType, castType);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "cast-to";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorTryCast<T> : IBinaryOperator<T, Type>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<Type> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "as";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNullCoalesce<T> : IBinaryOperator<T>
		{
			#region IBinaryOperator<T,T> Members

			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "??";
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
