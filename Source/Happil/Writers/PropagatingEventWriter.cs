using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class PropagatingEventWriter : EventWriterBase
	{
		public PropagatingEventWriter(EventMember ownerEvent, IOperand target)
			: base(ownerEvent)
		{
			var addOn = new PropagatingMethodWriter(OwnerEvent.AddMethod, target);
			var removeOn = new PropagatingMethodWriter(OwnerEvent.RemoveMethod, target);
		}
	}
}
