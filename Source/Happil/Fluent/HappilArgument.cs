using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public class HappilArgument<T> : HappilAssignable<T>, ICanEmitAddress
	{
		private readonly byte m_Index;
		private readonly string m_Name;
		private readonly bool m_IsByRef;
		private readonly ParameterBuilder m_ParameterBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilArgument(HappilMethod ownerMethod, byte index)
			: base(ownerMethod)
		{
			var indexBase = (ownerMethod.IsStatic ? 0 : 1);

			if ( index < indexBase )
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Argument index must be {0}-based.", indexBase));
			}

			m_Index = index;
			m_Name = ownerMethod.GetArgumentNames()[index - indexBase];
			m_IsByRef = ownerMethod.GetArgumentTypes()[index - indexBase].IsByRef;
			m_ParameterBuilder = ownerMethod.DefineParameter(index - indexBase);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Arg<{0}>{{#{1}}}", typeof(T).Name, m_Index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilArgument<T> Attribute<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new HappilAttributeBuilder<TAttribute>(values);
			m_ParameterBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return m_IsByRef;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsByRef
		{
			get
			{
				return m_IsByRef;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int Index
		{
			get
			{
				return m_Index;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( m_IsByRef )
			{
				EmitLdarg(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			if ( !m_IsByRef )
			{
				EmitLdarg(il);
			}
			else if ( !OperandType.IsValueType )
			{
				il.Emit(OpCodes.Ldind_Ref);
			}
			else 
			{
				il.Emit(OpCodes.Ldobj, OperandType);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			if ( !m_IsByRef )
			{
				il.Emit(OpCodes.Starg_S, m_Index);
			}
			else if ( !OperandType.IsValueType )
			{
				il.Emit(OpCodes.Stind_Ref);
			}
			else
			{
				il.Emit(OpCodes.Stobj, OperandType);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			if ( m_IsByRef )
			{
				EmitLdarg(il);
			}
			else
			{
				il.Emit(OpCodes.Ldarga_S, m_Index);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void EmitLdarg(ILGenerator il)
		{
			switch ( m_Index )
			{
				case 1:
					il.Emit(OpCodes.Ldarg_1);
					break;
				case 2:
					il.Emit(OpCodes.Ldarg_2);
					break;
				case 3:
					il.Emit(OpCodes.Ldarg_3);
					break;
				default:
					il.Emit(OpCodes.Ldarg_S, m_Index);
					break;
			}
		}
	}
}
