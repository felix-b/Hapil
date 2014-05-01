using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Operands
{
	public class Field<T> : MutableOperand<T>, IAcceptOperandVisitor
	{
		private readonly FieldInfo m_FieldInfo;
		private readonly FieldMember m_FieldMember;
		private readonly string m_Name;
		private IOperand m_Target;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Field(IOperand target, FieldInfo fieldInfo)
		{
			m_Target = target;
			m_FieldInfo = fieldInfo;
			m_Name = fieldInfo.Name;
			m_FieldMember = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Field(IOperand target, FieldMember fieldMember)
		{
			m_Target = target;
			m_FieldMember = fieldMember;
			m_FieldInfo = m_FieldMember.FieldBuilder;
			m_Name = fieldMember.Name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Target);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format(
				"{0}Field[{1}]", 
				m_FieldInfo.IsStatic ? m_FieldInfo.DeclaringType.Name + "::" : m_Target.ToString() + ".",
				m_Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Field;
			}
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
			il.Emit(m_FieldInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, m_FieldInfo);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(m_FieldInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, m_FieldInfo);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(m_FieldInfo.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, m_FieldInfo);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator FieldMember(Field<T> fieldOperand)
		{
			if ( fieldOperand.m_FieldMember != null )
			{
				return fieldOperand.m_FieldMember;
			}
			else
			{
				throw new InvalidOperationException("Current operand is not associated with a field member of the type being built.");
			}
		}
	}
}
