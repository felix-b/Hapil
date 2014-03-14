using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Selectors;

namespace Happil.Fluent
{
	public interface IHappilClassBody<TBase> : IHappilClassDefinition
	{
		IHappilClassBody<T> AsBase<T>();
		IHappilClassBody<TBase> ImplementInterface(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members);
		IHappilClassBody<TInterface> ImplementInterface<TInterface>(params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members);

		IHappilClassBody<TBase> Attribute<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null) 
			where TAttribute : Attribute;

		HappilField<T> Field<T>(string name);
		IHappilClassBody<TBase> Field<T>(string name, out HappilField<T> field);
		IHappilClassBody<TBase> Field<T>(string name, IHappilAttributes attributes, out HappilField<T> field);

		HappilField<T> StaticField<T>(string name);
		IHappilClassBody<TBase> StaticField<T>(string name, out HappilField<T> field);
		IHappilClassBody<TBase> StaticField<T>(string name, IHappilAttributes attributes, out HappilField<T> field);

		IHappilClassBody<TBase> DefaultConstructor(IHappilAttributes attributes = null);
		IHappilClassBody<TBase> StaticConstructor(Action<IHappilConstructorBody> body, IHappilAttributes attributes = null);
		IHappilClassBody<TBase> Constructor(Action<IHappilConstructorBody> body, IHappilAttributes attributes = null);
		IHappilClassBody<TBase> Constructor<TArg1>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>> body, IHappilAttributes attributes = null);
		IHappilClassBody<TBase> Constructor<TArg1, TArg2>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body, IHappilAttributes attributes = null);
		IHappilClassBody<TBase> Constructor<TArg1, TArg2, TArg3>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body, IHappilAttributes attributes = null);

		MethodSelectors.Void<TBase> Method(Expression<Func<TBase, Action>> method);
		MethodSelectors.Void1Arg<TBase, TArg1> Method<TArg1>(Expression<Func<TBase, Action<TArg1>>> method);
		MethodSelectors.Void2Args<TBase, TArg1, TArg2> Method<TArg1, TArg2>(Expression<Func<TBase, Action<TArg1, TArg2>>> method);
		MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3> Method<TArg1, TArg2, TArg3>(Expression<Func<TBase, Action<TArg1, TArg2, TArg3>>> method);
		MethodSelectors.Functions<TBase, TReturn> Method<TReturn>(Expression<Func<TBase, Func<TReturn>>> function);
		MethodSelectors.Functions1Arg<TBase, TArg1, TReturn> Method<TArg1, TReturn>(Expression<Func<TBase, Func<TArg1, TReturn>>> function);
		MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn> Method<TArg1, TArg2, TReturn>(Expression<Func<TBase, Func<TArg1, TArg2, TReturn>>> function);
		MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn> Method<TArg1, TArg2, TArg3, TReturn>(Expression<Func<TBase, Func<TArg1, TArg2, TArg3, TReturn>>> function);
		MethodSelectors.Untyped<TBase> AllMethods(Func<MethodInfo, bool> where = null);
		MethodSelectors.Void<TBase> VoidMethods(Func<MethodInfo, bool> where = null);
		MethodSelectors.Void1Arg<TBase, TArg1> VoidMethods<TArg1>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Void2Args<TBase, TArg1, TArg2> VoidMethods<TArg1, TArg2>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3> VoidMethods<TArg1, TArg2, TArg3>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Functions<TBase, TReturn> NonVoidMethods<TReturn>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Functions1Arg<TBase, TArg1, TReturn> NonVoidMethods<TArg1, TReturn>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn> NonVoidMethods<TArg1, TArg2, TReturn>(Func<MethodInfo, bool> where = null);
		MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn> NonVoidMethods<TArg1, TArg2, TArg3, TReturn>(Func<MethodInfo, bool> where = null);

		PropertySelectors.Typed<TBase, TProperty> Property<TProperty>(Expression<Func<TBase, TProperty>> property);
		PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty> This<TIndex, TProperty>(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty> This<TIndex1, TIndex2, TProperty>(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Untyped<TBase> AllProperties(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Untyped<TBase> ReadOnlyProperties(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Untyped<TBase> ReadWriteProperties(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Typed<TBase, TProperty> Properties<TProperty>(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty> Properties<TIndex, TProperty>(Func<PropertyInfo, bool> where = null);
		PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty> Properties<TIndex1, TIndex2, TProperty>(Func<PropertyInfo, bool> where = null);

		EventSelectors.Untyped<TBase> AllEvents(Func<EventInfo, bool> where = null);
		//EventSelectors.Typed<TBase, TDelegate> Events<TDelegate>(Func<EventInfo, bool> where = null);

		IHappilClassBody<TBase> DecorateWith<TImplementor>() where TImplementor : IDecorationImplementor, new();
		IHappilClassBody<TBase> DecorateWith(IDecorationImplementor implementor);
		IHappilClassBody<TBase> DecorateWith(IEnumerable<IDecorationImplementor> implementors);
	}
}
