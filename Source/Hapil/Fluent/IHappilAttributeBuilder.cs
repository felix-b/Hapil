using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hapil.Fluent
{
	public interface IHappilAttributeBuilder<TAttribute>
		where TAttribute : Attribute
	{
		IHappilAttributeBuilder<TAttribute> Arg<T>(T value);
		IHappilAttributeBuilder<TAttribute> Named<T>(Expression<Func<TAttribute, T>> fieldOrProperty, T value);
	}
}
