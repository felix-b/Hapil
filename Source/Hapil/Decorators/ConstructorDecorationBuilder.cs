using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Writers;

namespace Hapil.Decorators
{
	public class ConstructorDecorationBuilder : MethodDecorationBuilder
	{
		internal ConstructorDecorationBuilder(DecoratingConstructorWriter ownerWriter)
			: base(ownerWriter)
		{
		}
	}
}
