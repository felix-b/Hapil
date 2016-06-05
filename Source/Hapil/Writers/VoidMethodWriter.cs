using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;
using TT = Hapil.TypeTemplate;

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

        public void Base(params IOperand[] arguments)
	    {
	        var baseMethod = GetValidBaseMethod(arguments);
	        
            using (TT.CreateScope<TT.TBase>(baseMethod.DeclaringType))
	        {
                AddStatement(new CallStatement(This<TT.TBase>(), baseMethod, disableVirtual: true, arguments: arguments));
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
