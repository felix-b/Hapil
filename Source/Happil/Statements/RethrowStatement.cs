using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal class RethrowStatement : IHappilStatement
	{
		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			il.Emit(OpCodes.Rethrow);
		}

		#endregion
	}
}
