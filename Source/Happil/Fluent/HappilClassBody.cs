using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public class HappilClassBody<TBase>
	{
		public IMember DefaultConstructor()
		{
			throw new NotImplementedException();
		}

		public IMemberGroup AutomaticProperties()
		{
			throw new NotImplementedException();
		}

		public IMemberGroup AutomaticProperties(Func<PropertyInfo, bool> where)
		{
			throw new NotImplementedException();
		}

		public IMemberGroup Property<T>(
			Expression<Func<TBase, T>> selector, 
			Func<HappilProperty, HappilPropertyGetter> getter, 
			Func<HappilProperty, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IMemberGroup Properties(
			Func<PropertyInfo, bool> where,
			Func<HappilProperty, HappilPropertyGetter> getter,
			Func<HappilProperty, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IMemberGroup Methods(Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public IMemberGroup Methods(Func<MethodInfo, bool> where, Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}
	}
}
