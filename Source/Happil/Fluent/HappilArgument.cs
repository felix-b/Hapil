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
		private readonly bool m_IsByRef;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilArgument(HappilMethod ownerMethod, byte index)
			: base(ownerMethod)
		{
			if ( index < 1 )
			{
				throw new ArgumentOutOfRangeException("index", "Argument index must be 1-based.");
			}

			m_Index = index;
			m_IsByRef = ownerMethod.GetArgumentTypes()[index - 1].IsByRef;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Arg<{0}>{{#{1}}}", typeof(T).Name, m_Index);
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
