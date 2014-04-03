using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;

namespace Happil.Expressions
{
	//TODO:redesign: rename and move to Operands
	public class FieldAccessOperand<T> : MutableOperand<T>
	{
		private readonly IOperand m_Target;
		private readonly FieldInfo m_Field;
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FieldAccessOperand(IOperand target, FieldInfo field)
		{
			m_Target = target;
			m_Field = field;
			m_Name = field.Name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Field{{{0}}}", m_Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( m_Target != null )
			{
				m_Target.EmitTarget(il);
				m_Target.EmitLoad(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(m_Field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, m_Field);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(m_Field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, m_Field);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(m_Field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, m_Field);
		}
	}
}
