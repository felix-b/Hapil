using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public class TemplateMethodWriter : MethodWriterBase
	{
		private readonly Action<TemplateMethodWriter> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TemplateMethodWriter(MethodMember ownerMethod, Action<TemplateMethodWriter> script)
			: base(ownerMethod)
		{
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TypeTemplate.TReturn> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TypeTemplate.TReturn>(operand));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			StatementScope.Current.AddStatement(new ReturnStatement());
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
