using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilField : AssignableHappilOperand, IMember
	{
		#region IMember Members

		public IMember[] Ungroup()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
