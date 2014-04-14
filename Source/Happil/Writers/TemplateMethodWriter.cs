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
		private Local<TypeTemplate.TReturn> m_ReturnValueLocal;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TemplateMethodWriter(MethodMember ownerMethod, Action<TemplateMethodWriter> script)
			: this(ownerMethod, MethodWriterModes.Normal, script)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TemplateMethodWriter(MethodMember ownerMethod, MethodWriterModes mode, Action<TemplateMethodWriter> script)
			: base(ownerMethod, mode, attachToOwner: true)
		{
			m_Script = script;
			m_ReturnValueLocal = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TypeTemplate.TReturn> operand)
		{
			AddReturnStatement(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			AddReturnStatement();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Local<TypeTemplate.TReturn> ReturnValueLocal
		{
			get
			{
				return m_ReturnValueLocal;
			}
			set
			{
				m_ReturnValueLocal = value;
			}
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
