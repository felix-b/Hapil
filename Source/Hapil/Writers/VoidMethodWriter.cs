using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Writers
{
	public class VoidMethodWriter : MethodWriterBase
	{
		private readonly Action<VoidMethodWriter> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public VoidMethodWriter(MethodMember ownerMethod, Action<VoidMethodWriter> script)
			: base(ownerMethod)
		{
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			AddReturnStatement();
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
