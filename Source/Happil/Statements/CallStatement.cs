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

		public CallStatement(IHappilOperandInternals target, MethodBase method, params IHappilOperandInternals[] arguments)
		{
			StatementScope.Current.Consume((IHappilOperand<object>)target);

			m_Target = target;
			m_Method = method;
			m_Arguments = arguments;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			Helpers.EmitCall(il, m_Target, m_Method, m_Arguments);
		}

		#endregion
	}
}
