using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;

namespace Happil.Operands
{
	public class LocalOperand<T> : MutableOperand<T>, ICanEmitAddress
	{
		private readonly LocalBuilder m_LocalBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal LocalOperand(MethodMember ownerMethod)
		{
			m_LocalBuilder = ownerMethod.MethodFactory.GetILGenerator().DeclareLocal(TypeTemplate.Resolve<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Local<{0}>{{#{1}}}", m_LocalBuilder.LocalType.Name, m_LocalBuilder.LocalIndex);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc, m_LocalBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(OpCodes.Stloc, m_LocalBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldloca, (short)m_LocalBuilder.LocalIndex);
		}
	}
}
