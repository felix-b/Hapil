using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	internal interface IHappilOperandInternals
	{
		HappilClass OwnerClass { get; }
		HappilMethod OwnerMethod { get; }
	}
}












