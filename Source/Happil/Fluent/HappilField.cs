using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilField<T> : HappilAssignableOperand<T>, IHappilMember
	{
		#region IMember Members

		public IHappilMember[] Flatten()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
