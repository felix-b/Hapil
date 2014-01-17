using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	public class ForStatement : IHappilStatement
	{
		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
