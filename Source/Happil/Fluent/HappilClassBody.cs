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
		public IHappilMember DefaultConstructor()
		{
			throw new NotImplementedException();
		}

		public IHappilMember AutomaticProperties()
		{
			throw new NotImplementedException();
		}

		public IHappilMember AutomaticProperties(Func<PropertyInfo, bool> where)
		{
			throw new NotImplementedException();
		}

		public IHappilMember Property<T>(
			Expression<Func<TBase, T>> selector, 
			Func<HappilProperty<T>, HappilPropertyGetter> getter, 
			Func<HappilProperty<T>, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IHappilMember Properties(
			Func<PropertyInfo, bool> where,
			Func<HappilProperty<object>, HappilPropertyGetter> getter,
			Func<HappilProperty<object>, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IHappilMember Methods(Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public IHappilMember Methods(Func<MethodInfo, bool> where, Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}
	}
}
