using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Writers;

namespace Happil
{
	public static class Attributes
	{
		public static AttributeWriter Set<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null) 
			where TAttribute : Attribute
		{
			return new AttributeWriter().Set<TAttribute>(values);
		}
	}
}
