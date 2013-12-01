using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilClassBody<TBase>
	{
		IMember DefaultConstructor();
		IMemberGroup AutomaticProperties();
		IMemberGroup AutomaticProperties(Func<PropertyInfo, bool> where);
		IMemberGroup Property<T>(
			Expression<Func<TBase, T>> selector, 
			Func<IProperty, IPropertyGetter> getter, 
			Func<IProperty, IPropertySetter> setter = null);
		IMemberGroup Properties(
			Func<PropertyInfo, bool> where,
			Func<IProperty, IPropertyGetter> getter,
			Func<IProperty, IPropertySetter> setter = null);
	}
}
