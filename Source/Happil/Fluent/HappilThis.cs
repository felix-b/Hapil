using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public class HappilThis<T> : HappilOperand<T>
	{
		internal HappilThis(HappilMethod ownerMethod)
			: base(ownerMethod)
		{
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

		public FieldAccessOperand<TProperty> BackingFieldOf<TProperty>(PropertyInfo declaration)
		{
			return OwnerClass.GetPropertyBackingField(declaration).AsOperand<TProperty>();
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
