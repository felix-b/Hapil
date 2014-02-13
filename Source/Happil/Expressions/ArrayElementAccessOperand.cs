using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal class ArrayElementAccessOperand<T> : HappilAssignable<T>
	{
		private readonly IHappilOperand m_Array;
		private readonly IHappilOperand m_Index;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ArrayElementAccessOperand(IHappilOperand array, IHappilOperand index)
			: base(ownerMethod: null)
		{
			m_Array = array;
			m_Index = index;
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
