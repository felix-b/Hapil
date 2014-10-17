using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Statements;

namespace Hapil.Operands
{
	public class ThisOperand<T> : Operand<T>, IScopedOperand, ITransformType
	{
		private readonly ClassType m_OwnerClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ThisOperand(ClassType ownerClass)
		{
			m_OwnerClass = ownerClass;
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HapilField<TField> FindField<TField>(string name)
		//{
		//	return OwnerClass.FindMember<HapilField<TField>>(name);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HapilProperty<TProperty> FindProperty<TProperty>(string name)
		//{
		//	return OwnerClass.FindMember<HapilProperty<TProperty>>(name);
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		#region IScopedOperand Members

		StatementBlock IScopedOperand.HomeStatementBlock
		{
			get
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IScopedOperand.CaptureName
		{
			get
			{
				return "This";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		bool IScopedOperand.ShouldInitializeHoistedField
		{
			get
			{
				return true;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ITransformType Members

		Operand<TCast> ITransformType.TransformToType<TCast>()
		{
			return new ThisOperand<TCast>(m_OwnerClass);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<TProperty> BackingFieldOf<TProperty>(PropertyInfo declaration)
		{
			return m_OwnerClass.GetPropertyBackingField(declaration).AsOperand<TProperty>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return "this";
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.This;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(System.Reflection.Emit.ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(System.Reflection.Emit.ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(System.Reflection.Emit.ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(System.Reflection.Emit.ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}
}
