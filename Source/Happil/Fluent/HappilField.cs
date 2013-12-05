using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilField<T> : AssignableOperand<T>, IMember
	{
		#region IMember Members

		public IMember[] Ungroup()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
