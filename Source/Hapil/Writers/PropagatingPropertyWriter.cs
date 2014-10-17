using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
{
	public class PropagatingPropertyWriter : PropertyWriterBase
	{
		public PropagatingPropertyWriter(PropertyMember ownerProperty, IOperand target)
			: base(ownerProperty)
		{
			if ( OwnerProperty.GetterMethod != null )
			{
				var getter = new PropagatingMethodWriter(OwnerProperty.GetterMethod, target);
			}

			if ( OwnerProperty.SetterMethod != null )
			{
				var setter = new PropagatingMethodWriter(OwnerProperty.SetterMethod, target);
			}
		}
	}
}
