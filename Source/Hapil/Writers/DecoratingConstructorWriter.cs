using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Decorators;
using Hapil.Members;

namespace Hapil.Writers
{
	public class DecoratingConstructorWriter : DecoratingMethodWriter
	{
		public DecoratingConstructorWriter(MethodMember ownerMethod)
			: base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override MethodDecorationBuilder CreateDecorationBuilder()
		{
			return new ConstructorDecorationBuilder(this);
		}
	}
}
