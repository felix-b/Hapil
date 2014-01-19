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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilArgument(HappilMethod ownerMethod, byte index)
			: base(ownerMethod)
		{
			if ( index < 1 )
			{
				throw new ArgumentOutOfRangeException("index", "Argument index must be 1-based.");
			}

			m_Index = index;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Arg<{0}>{{#{1}}}", typeof(T).Name, m_Index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// arguments have no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(OpCodes.Starg_S, m_Index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldarga_S, m_Index);
		}
	}
}
