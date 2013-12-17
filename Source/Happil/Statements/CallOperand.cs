using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class CallOperand<TReturn> : HappilOperand<TReturn>
	{
		private readonly IHappilOperandInternals m_Target;
		private readonly MethodBase m_Method;
		private readonly IHappilOperandInternals[] m_Arguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallOperand(HappilMethod ownerMethod, IHappilOperand<object> target, MethodBase method, params IHappilOperand<object>[] arguments)
			: base(ownerMethod)
		{
			m_Target = (target as IHappilOperandInternals);
			m_Method = method;
			m_Arguments = arguments.Cast<IHappilOperandInternals>().ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( m_Target != null )
			{
				m_Target.EmitTarget(il);
				m_Target.EmitLoad(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			foreach ( var argument in m_Arguments )
			{
				argument.EmitTarget(il);
				argument.EmitLoad(il);
			}

			var methodInfo = (m_Method as MethodInfo);
			var constructorInfo = (m_Method as ConstructorInfo);
			var callOpcode = (m_Method.IsVirtual || m_Method.DeclaringType.IsInterface ? OpCodes.Callvirt : OpCodes.Call);

			if ( methodInfo != null )
			{
				il.Emit(callOpcode, methodInfo);
			}
			else
			{
				il.Emit(callOpcode, constructorInfo);
			}
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
}
