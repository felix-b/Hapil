using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	public class HappilLocal<T> : HappilAssignable<T>
	{
		private readonly LocalBuilder m_LocalBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilLocal(HappilMethod ownerMethod)
			: base(ownerMethod)
		{
			m_LocalBuilder = ownerMethod.MethodBuilder.GetILGenerator().DeclareLocal(TypeTemplate.Resolve<T>());
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
