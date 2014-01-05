using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	//TODO: this class only implements instance fields; should implement static fields as well
	public class HappilField<T> : HappilAssignable<T>, IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly FieldBuilder m_FieldBuilder;
		
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

		internal HappilField(HappilClass happilClass, string name)
			: base(ownerMethod: null)
		{
			m_HappilClass = happilClass;
			m_Name = happilClass.TakeMemberName(name);

			var actualType = TypeTemplate.ResolveActualType<T>();
			m_FieldBuilder = happilClass.TypeBuilder.DefineField(m_Name, actualType, FieldAttributes.Private);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IMember Members

		void IHappilMember.EmitBody()
		{
			// nothing - a field does not have a body
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
			il.Emit(OpCodes.Ldarg_0); // push 'this' reference onto stack
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldfld, m_FieldBuilder);  // push field value onto stack
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(OpCodes.Stfld, m_FieldBuilder);  // pop value from stack into field
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldflda, m_FieldBuilder);  // push field address onto stack
		}
	}
}
