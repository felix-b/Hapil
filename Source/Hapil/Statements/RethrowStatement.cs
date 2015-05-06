using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class RethrowStatement : StatementBase
	{
		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			il.Emit(OpCodes.Rethrow);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}

		#endregion
	}
}
