using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class CallStatement : IHappilStatement
	{
		private readonly IHappilOperandInternals m_Target;
		private readonly MethodBase m_Method;
		private readonly IHappilOperandInternals[] m_Arguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallStatement(IHappilOperand<object> target, MethodBase method, params IHappilOperand<object>[] arguments)
		{
			m_Target = target as IHappilOperandInternals;
			m_Method = method;
			m_Arguments = arguments.Cast<IHappilOperandInternals>().ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			if ( m_Target != null )
			{
				m_Target.EmitTarget(il);
				m_Target.EmitLoad(il);
			}

			foreach ( var argument in m_Arguments )
			{
				argument.EmitTarget(il);
				argument.EmitLoad(il);
			}

			var methodInfo = (m_Method as MethodInfo);
			var constructorInfo = (m_Method as ConstructorInfo);
			var opCode = (m_Method.IsVirtual || m_Method.DeclaringType.IsInterface ? OpCodes.Callvirt : OpCodes.Call);

			if ( methodInfo != null )
			{
				il.Emit(opCode, methodInfo);
			}
			else
			{
				il.Emit(opCode, constructorInfo);
			}
		}

		#endregion
	}
}
