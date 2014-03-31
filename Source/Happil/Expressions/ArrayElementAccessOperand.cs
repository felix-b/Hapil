using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	//TODO:redesign:rename and move to Operands
	internal class ArrayElementAccessOperand<T> : MutableOperand<T>
	{
		private readonly IOperand m_Array;
		private readonly IOperand m_Index;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ArrayElementAccessOperand(IOperand array, IOperand index)
		{
			m_Array = array;
			m_Index = index;

			StatementScope.Current.Consume(index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return true;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			m_Array.EmitTarget(il);
			m_Array.EmitLoad(il);

			m_Index.EmitTarget(il);
			m_Index.EmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			if ( OperandType.IsValueType )
			{
				il.Emit(OpCodes.Ldelem, OperandType);
			}
			else
			{
				il.Emit(OpCodes.Ldelem_Ref);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			if ( OperandType.IsValueType )
			{
				il.Emit(OpCodes.Stelem, OperandType);
			}
			else
			{
				il.Emit(OpCodes.Stelem_Ref);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldelema, OperandType);
		}
	}
}
