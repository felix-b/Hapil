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
		IHappilClassBody<T> AsBase<T>();
		IHappilClassBody<TBase> Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TInterface> Implement<TInterface>(params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TInterface> Implement<TInterface>(Type interfaceType, params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members);

		HappilField<T> Field<T>(string name);
		IHappilClassBody<TBase> Field<T>(string name, out HappilField<T> field);

		IHappilClassBody<TBase> DefaultConstructor();

		IHappilClassBody<TBase> Constructor(Action<IHappilConstructorBody> body);
		IHappilClassBody<TBase> Constructor<TArg1>(Action<IHappilConstructorBody, HappilArgument<TArg1>> body);
		IHappilClassBody<TBase> Constructor<TArg1, TArg2>(Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body);
		IHappilClassBody<TBase> Constructor<TArg1, TArg2, TArg3>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body);

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

		IHappilClassBody<TBase> VoidMethod(
			Expression<Func<TBase, Action>> method, 
			Action<IVoidHappilMethodBody> body);
		
		IHappilClassBody<TBase> VoidMethod<TArg1>(
			Expression<Func<TBase, Action<TArg1>>> method,
			Action<IVoidHappilMethodBody, HappilArgument<TArg1>> body);

		IHappilClassBody<TBase> VoidMethod<TArg1, TArg2>(
			Expression<Func<TBase, Action<TArg1, TArg2>>> method,
			Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body);

		IHappilClassBody<TBase> VoidMethod<TArg1, TArg2, TArg3>(
			Expression<Func<TBase, Action<TArg1, TArg2, TArg3>>> method,
			Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body);

		IHappilClassBody<TBase> Function<TReturn>(
			Expression<Func<TBase, Func<TReturn>>> method,
			Action<IHappilMethodBody<TReturn>> body);

		IHappilClassBody<TBase> Function<TArg1, TReturn>(
			Expression<Func<TBase, Func<TArg1, TReturn>>> method,
			Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>> body);

		IHappilClassBody<TBase> Function<TArg1, TArg2, TReturn>(
			Expression<Func<TBase, Func<TArg1, TArg2, TReturn>>> method,
			Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg1>> body);

		IHappilClassBody<TBase> Methods(
			Action<IVoidHappilMethodBody> body);

		IHappilClassBody<TBase> Methods<TReturn>(
			Action<IHappilMethodBody<TReturn>> body);

		IHappilClassBody<TBase> Methods<TReturn>(
			Func<MethodInfo, bool> where, 
			Action<IHappilMethodBody<TReturn>> body);
	}
}
