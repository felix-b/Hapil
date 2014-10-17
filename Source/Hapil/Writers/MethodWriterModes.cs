using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Writers
{
	[Flags]
	public enum MethodWriterModes
	{
		Normal = 0x00,
		Decorated = 0x01,
		Decorator = 0x02
	}
}
