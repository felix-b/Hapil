using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal interface IValueTypeInitializer
	{
		IHappilOperand Target { set; }
	}
}
