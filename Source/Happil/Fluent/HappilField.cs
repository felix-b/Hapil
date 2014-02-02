using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public class HappilField<T> : HappilAssignable<T>, IHappilMember, ICanEmitAddress
	{
		private readonly HappilClass m_HappilClass;
		private readonly FieldBuilder m_FieldBuilder;
		private readonly bool m_IsStatic;
		
		//TODO: remove this field
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: remove this constructor (update unit tests)
		internal HappilField(string name)
			: base(ownerMethod: null)
		{
			m_HappilClass = null;
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilField(HappilClass happilClass, string name, bool isStatic = false)
			: base(ownerMethod: null)
		{
			m_HappilClass = happilClass;
			m_Name = happilClass.TakeMemberName(name);
			m_IsStatic = isStatic;

			var actualType = TypeTemplate.Resolve<T>();
			var attributes = (isStatic ? FieldAttributes.Private | FieldAttributes.Static : FieldAttributes.Private);

			m_FieldBuilder = happilClass.TypeBuilder.DefineField(m_Name, actualType, attributes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return !m_IsStatic;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		void IHappilMember.EmitBody()
		{
			// nothing - a field does not have a body
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		IDisposable IHappilMember.CreateTypeTemplateScope()
		{
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IHappilMember.Name
		{
			get
			{
				return m_Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Field{{{0}}}", m_Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of HappilOperand<T>

		internal override HappilClass OwnerClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( !m_IsStatic )
			{
				il.Emit(OpCodes.Ldarg_0); // push 'this' reference onto stack
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(m_IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, m_FieldBuilder);  // push field value onto stack
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(m_IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, m_FieldBuilder);  // pop value from stack into field
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(m_IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, m_FieldBuilder);  // push field address onto stack
		}
	}
}
