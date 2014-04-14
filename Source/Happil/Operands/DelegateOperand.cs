using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Members;

namespace Happil.Operands
{
	public class DelegateOperand<TDelegate> : Operand<TDelegate>, IDelegateOperand
	{
		private readonly IOperand m_Target;
		private readonly MethodInfo m_Method;
		private readonly ConstructorInfo m_Constructor;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DelegateOperand(IOperand target, MethodInfo method) 
		{
			m_Target = target;
			m_Method = method;
			m_Constructor = DelegateShortcuts.GetDelegateConstructor(TypeTemplate.Resolve<TDelegate>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			m_Target.EmitTarget(il);
			m_Target.EmitLoad(il);
			
			il.Emit(OpCodes.Ldftn, m_Method);
			il.Emit(OpCodes.Newobj, m_Constructor);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IDelegateOperand
	{
	}
}
