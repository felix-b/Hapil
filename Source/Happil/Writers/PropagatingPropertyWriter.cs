using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
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
