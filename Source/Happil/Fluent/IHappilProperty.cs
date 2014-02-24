using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	internal interface IHappilProperty
	{
		HappilField<T> GetBackingFieldAs<T>();
		PropertyInfo Declaration { get; }
	}
}
