using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Operands
{
	internal class ArrayElementOperand<T> : MutableOperand<T>, IAcceptOperandVisitor
	{
		private IOperand m_Array;
		private IOperand m_Index;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ArrayElementOperand(IOperand array, IOperand index)
		{
			m_Array = array;
			m_Index = index;

			StatementScope.Current.Consume(index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Array);
			visitor.VisitOperand(ref m_Index);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format("{0}[{1}]", m_Array, m_Index);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return true;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.ArrayElement;
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
