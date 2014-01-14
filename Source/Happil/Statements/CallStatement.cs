using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class CallStatement : IHappilStatement
	{
		private readonly IHappilOperand m_Target;
		private readonly MethodBase m_Method;
		private readonly IHappilOperand[] m_Arguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallStatement(IHappilOperand target, MethodBase method, params IHappilOperand[] arguments)
		{
			var scope = StatementScope.Current;
	
			scope.Consume(target as IHappilExpression);

			foreach ( var argument in arguments )
			{
				scope.Consume(argument as IHappilExpression);			
			}

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
