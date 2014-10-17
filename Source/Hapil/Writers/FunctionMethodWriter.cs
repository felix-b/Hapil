
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Writers
{
	public class FunctionMethodWriter<TReturn> : MethodWriterBase
	{
		private readonly Action<FunctionMethodWriter<TReturn>> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FunctionMethodWriter(MethodMember ownerMethod, Action<FunctionMethodWriter<TReturn>> script)
			: this(ownerMethod, script, mode: MethodWriterModes.Normal, attachToOwner: true)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FunctionMethodWriter(MethodMember ownerMethod, Action<FunctionMethodWriter<TReturn>> script, MethodWriterModes mode, bool attachToOwner)
			: base(ownerMethod, mode, attachToOwner)
		{
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TReturn> operand)
		{
			AddReturnStatement(operand.OrNullConstant());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(TReturn constantValue)
		{
			AddReturnStatement(new Constant<TReturn>(constantValue));
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
