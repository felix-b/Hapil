using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	public static class Attributes
	{
		public static IHappilAttributes Set<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null) 
			where TAttribute : Attribute
		{
			return new HappilAttributes().Set<TAttribute>(values);
		}
	}
}
