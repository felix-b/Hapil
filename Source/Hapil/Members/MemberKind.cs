using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Members
{
	public enum MemberKind
	{
		StaticField,
		StaticConstructor,
		StaticAnonymousMethod,
		InstanceField,
		InstanceConstructor,
		Destructor,
		VirtualMethod,
		InstanceAnonymousMethod,
		InstanceProperty,
		InstanceEvent
	}
}
