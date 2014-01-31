using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		public abstract class OverloadableBinaryOperatorBase<TLeft, TRight> : IBinaryOperator<TLeft, TRight>
		{
			private readonly OpCode m_Instruction;
			private readonly Func<TypeOperators, MethodInfo> m_OverloadSelector;
			private readonly string m_Symbol;
			private bool m_Overloaded;

			//-------------------------------------------------------------------------------------------------------------------------------------------------
	
			protected OverloadableBinaryOperatorBase(OpCode instruction, Func<TypeOperators, MethodInfo> overloadSelector, string symbol)
			{
				m_Instruction = instruction;
				m_OverloadSelector = overloadSelector;
				m_Symbol = symbol;
				m_Overloaded = false;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public virtual void Emit(ILGenerator il, IHappilOperand<TLeft> left, IHappilOperand<TRight> right)
			{
				var typeOverloads = TypeOperators.GetOperators(left.OperandType);
				var overload = m_OverloadSelector(typeOverloads);

				if ( overload != null )
				{
					m_Overloaded = true;
					Helpers.EmitCall(il, null, overload, left, right);
				}
				else
				{
					left.EmitTarget(il);
					left.EmitLoad(il);

					right.EmitTarget(il);
					right.EmitLoad(il);

					il.Emit(m_Instruction);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return m_Symbol;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected bool Overloaded
			{
				get
				{
					return m_Overloaded;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class OverloadableBinaryOperatorBase<T> : OverloadableBinaryOperatorBase<T, T>
		{
			protected OverloadableBinaryOperatorBase(OpCode instruction, Func<TypeOperators, MethodInfo> overloadSelector, string symbol)
				: base(instruction, overloadSelector, symbol)
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public sealed class OperatorAdd<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorAdd()
				: base(OpCodes.Add, overloads => overloads.OpAddition, "+")
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public sealed class OperatorSubtract<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorSubtract()
				: base(OpCodes.Sub, overloads => overloads.OpSubtraction, "-")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpSubtraction != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpSubtraction, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Sub);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "-";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public sealed class OperatorMultiply<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorMultiply()
				: base(OpCodes.Mul, overloads => overloads.OpMultiply, "*")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpMultiply != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpMultiply, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Mul);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "*";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorDivide<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorDivide()
				: base(OpCodes.Div, overloads => overloads.OpDivision, "/")
			{
			}
		} 
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpDivision != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpDivision, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Div);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "/";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorModulus<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorModulus()
				: base(OpCodes.Rem, overloads => overloads.OpModulus, "%")
			{
			}
		} 
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpModulus != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpModulus, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Rem);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "%";
		//	}
		//}

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
				var trueLabel = il.DefineLabel();
				var endLabel = il.DefineLabel();

				left.EmitTarget(il);
				left.EmitLoad(il);

				il.Emit(OpCodes.Brtrue, trueLabel);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Br, endLabel);

				il.MarkLabel(trueLabel);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Br, endLabel);

				il.MarkLabel(endLabel);
				il.Emit(OpCodes.Nop);
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
				left.EmitTarget(il);
				left.EmitLoad(il);

				right.EmitTarget(il);
				right.EmitLoad(il);

				il.Emit(OpCodes.Xor);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "^^";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseAnd<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorBitwiseAnd()
				: base(OpCodes.And, overloads => overloads.OpBitwiseAnd, "&")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "&";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseOr<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorBitwiseOr()
				: base(OpCodes.Or, overloads => overloads.OpBitwiseOr, "|")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "|";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseXor<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorBitwiseXor()
				: base(OpCodes.Xor, overloads => overloads.OpBitwiseXor, "^")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "^";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLeftShift<T> : OverloadableBinaryOperatorBase<T, int> 
		{
			public OperatorLeftShift()
				: base(OpCodes.Shl, overloads => overloads.OpLeftShift, "<<")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpLeftShift != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpLeftShift, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Shl);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "<<";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorRightShift<T> : OverloadableBinaryOperatorBase<T, int> 
		{
			public OperatorRightShift()
				: base(OpCodes.Shr, overloads => overloads.OpRightShift, ">>")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpRightShift != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpRightShift, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Shr);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return ">>";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorEqual<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorEqual()
				: base(OpCodes.Ceq, overloads => overloads.OpEquality, "==")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		var overloads = TypeOperators.GetOperators(left.OperandType);

		//		if ( overloads.OpEquality != null )
		//		{
		//			Helpers.EmitCall(il, null, overloads.OpEquality, left, right);
		//		}
		//		else
		//		{
		//			left.EmitTarget(il);
		//			left.EmitLoad(il);

		//			right.EmitTarget(il);
		//			right.EmitLoad(il);

		//			il.Emit(OpCodes.Ceq);
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "==";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNotEqual<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorNotEqual()
				: base(OpCodes.Ceq, overloads => overloads.OpInequality, "!=")
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				base.Emit(il, left, right);

				if ( !Overloaded )
				{
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThan<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorGreaterThan()
				: base(OpCodes.Cgt, overloads => overloads.OpGreaterThan, ">")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		left.EmitTarget(il);
		//		left.EmitLoad(il);

		//		right.EmitTarget(il);
		//		right.EmitLoad(il);

		//		il.Emit(OpCodes.Cgt);
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return ">";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThan<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorLessThan()
				: base(OpCodes.Clt, overloads => overloads.OpLessThan, "<")
			{
			}
		}
		//{
		//	public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
		//	{
		//		left.EmitTarget(il);
		//		left.EmitLoad(il);

		//		right.EmitTarget(il);
		//		right.EmitLoad(il);

		//		il.Emit(OpCodes.Clt);
		//	}

		//	//-------------------------------------------------------------------------------------------------------------------------------------------------

		//	public override string ToString()
		//	{
		//		return "<";
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThanOrEqual<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorGreaterThanOrEqual()
				: base(OpCodes.Clt, overloads => overloads.OpGreaterThanOrEqual, ">=")
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
	
			public override void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				base.Emit(il, left, right);
	
				if ( !Overloaded )
				{
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThanOrEqual<T> : OverloadableBinaryOperatorBase<T> 
		{
			public OperatorLessThanOrEqual()
				: base(OpCodes.Cgt, overloads => overloads.OpLessThanOrEqual, "<=")
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				base.Emit(il, left, right);

				if ( !Overloaded )
				{
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
				}
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
