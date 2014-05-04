using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	public class ThisOperand<T> : Operand<T>, IScopedOperand
	{
		private readonly ClassType m_OwnerClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ThisOperand(MethodMember ownerMethod)
		{
			m_OwnerClass = ownerMethod.OwnerClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ThisOperand(ClassType ownerClass)
		{
			m_OwnerClass = ownerClass;
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilField<TField> FindField<TField>(string name)
		//{
		//	return OwnerClass.FindMember<HappilField<TField>>(name);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilProperty<TProperty> FindProperty<TProperty>(string name)
		//{
		//	return OwnerClass.FindMember<HappilProperty<TProperty>>(name);
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
