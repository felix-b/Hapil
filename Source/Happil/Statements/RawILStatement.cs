using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Operands;

namespace Happil.Statements
{
	internal class RawILStatement : StatementBase
	{
		private readonly Action<ILGenerator> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public RawILStatement(Action<ILGenerator> script)
		{
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			m_Script(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}
	}
}
