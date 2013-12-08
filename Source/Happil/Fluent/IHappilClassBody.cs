using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilClassBody<TBase> : IHappilClassDefinition
	{
		IHappilClassBody<TBase> Inherit(object baseType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TBase> Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TClass> Inherit<TClass>(params Func<IHappilClassBody<TClass>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TClass> Inherit<TClass>(object baseType, params Func<IHappilClassBody<TClass>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TInterface> Implement<TInterface>(params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TInterface> Implement<TInterface>(Type interfaceType, params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members);

		IHappilClassBody<TBase> DefaultConstructor();

		IHappilClassBody<TBase> Property<T>(
			Expression<Func<TBase, T>> selector,
			Func<HappilProperty<T>, HappilPropertyGetter> getter,
			Func<HappilProperty<T>, HappilPropertySetter> setter = null);

		IHappilClassBody<TBase> Properties<T>(
			Func<HappilProperty<T>, HappilPropertyGetter> getter,
			Func<HappilProperty<T>, HappilPropertySetter> setter = null);

		IHappilClassBody<TBase> Properties<T>(
			Func<PropertyInfo, bool> where,
			Func<HappilProperty<T>, HappilPropertyGetter> getter,
			Func<HappilProperty<T>, HappilPropertySetter> setter = null);

		IHappilClassBody<TBase> AutomaticProperties();

		IHappilClassBody<TBase> AutomaticProperties(Func<PropertyInfo, bool> where);

		IHappilClassBody<TBase> Method(
			Action<TBase> method, 
			Action<IVoidHappilMethodBody> body);
		IHappilClassBody<TBase> Method<T1>(
			Action<TBase, T1> method,
			Action<IVoidHappilMethodBody, HappilArgument<T1>> body);
		IHappilClassBody<TBase> Method<T1, T2>(
			Action<TBase, T1, T2> method,
			Action<IVoidHappilMethodBody, HappilArgument<T1>, HappilArgument<T2>> body);
		IHappilClassBody<TBase> Method<T1, T2, T3>(
			Action<TBase, T1, T2, T3> method,
			Action<IVoidHappilMethodBody, HappilArgument<T1>, HappilArgument<T2>, HappilArgument<T3>> body);
	
		IHappilClassBody<TBase> Method<TReturn>(
			Func<TBase, TReturn> method, 
			Action<IHappilMethodBody<TReturn>> body);

		IHappilClassBody<TBase> Methods(
			Action<IVoidHappilMethodBody> body);

		IHappilClassBody<TBase> Methods<TReturn>(
			Action<IHappilMethodBody<TReturn>> body);

		IHappilClassBody<TBase> Methods<TReturn>(
			Func<MethodInfo, bool> where, 
			Action<IHappilMethodBody<TReturn>> body);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal interface IHappilClassBody
	{
		HappilClass OwnerClass { get; }
	}
}
