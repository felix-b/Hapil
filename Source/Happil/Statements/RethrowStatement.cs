using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal class RethrowStatement : StatementBase
	{
		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			il.Emit(OpCodes.Rethrow);
		}

		#endregion
	}
}
