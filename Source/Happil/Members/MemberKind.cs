using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Members
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
