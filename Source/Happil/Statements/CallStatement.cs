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
		private readonly MethodInfo m_Method;
		private readonly IHappilOperandInternals[] m_Arguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallStatement(IHappilOperand<object> target, MethodInfo method, params IHappilOperand<object>[] arguments)
		{
			m_Target = target as IHappilOperandInternals;
			m_Method = method;
			m_Arguments = arguments.Cast<IHappilOperandInternals>().ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
