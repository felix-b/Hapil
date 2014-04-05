
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public class FunctionMethodWriter<TReturn> : MethodWriterBase
	{
		private readonly Action<FunctionMethodWriter<TReturn>> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FunctionMethodWriter(MethodMember ownerMethod, Action<FunctionMethodWriter<TReturn>> script)
			: base(ownerMethod)
		{
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TReturn> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(operand.OrNullConstant()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(TReturn constantValue)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(new ConstantOperand<TReturn>(constantValue)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_Script != null )
			{
				m_Script(this);
			}

			base.Flush();
		}
	}
}
