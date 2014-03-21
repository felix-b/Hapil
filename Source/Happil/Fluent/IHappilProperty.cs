using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	internal interface IHappilProperty
	{
		PropertyInfo Declaration { get; }
		HappilField BackingField { get; }
	}
}
