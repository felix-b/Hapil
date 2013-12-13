using System;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class CallOperand<TReturn> : HappilOperand<TReturn>
	{
		private readonly IHappilOperand<object> m_Target;
		private readonly MethodInfo m_Method;
		private readonly IHappilOperand<object>[] m_Arguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallOperand(HappilMethod ownerMethod, IHappilOperand<object> target, MethodInfo method, params IHappilOperand<object>[] arguments)
			: base(ownerMethod)
		{
			m_Target = target;
			m_Method = method;
			m_Arguments = arguments;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected override void OnEmitTarget(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotImplementedException();
		}
	}
}
