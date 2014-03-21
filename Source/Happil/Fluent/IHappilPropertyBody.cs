using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public interface IHappilPropertyBody
	{
		PropertyInfo Declaration { get; }
		string Name { get; }
		Type Type { get; }
	}
	public interface IHappilPropertyBody<TProperty> : IHappilPropertyBody
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TProperty>> body);
		FieldAccessOperand<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyBody<TIndex, TProperty> : IHappilPropertyBody
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>, HappilArgument<TIndex>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex>, HappilArgument<TProperty>> body);
		FieldAccessOperand<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyBody<TIndex1, TIndex2, TProperty> : IHappilPropertyBody
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>, HappilArgument<TIndex1>, HappilArgument<TIndex2>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<TIndex2>, HappilArgument<TProperty>> body);
		FieldAccessOperand<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyGetter
	{
	}
	public interface IHappilPropertySetter
	{
	}
}
