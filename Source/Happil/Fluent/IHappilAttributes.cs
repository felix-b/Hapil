﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilAttributes
	{
		IHappilAttributes Set<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null) 
			where TAttribute : Attribute;
	}
}
