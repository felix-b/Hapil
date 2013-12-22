using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilPropertyBody<TProperty>
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TProperty>> body);
		HappilField<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyBody<TProperty, TIndex>
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>, HappilArgument<TIndex>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex>, HappilArgument<TProperty>> body);
		HappilField<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyBody<TProperty, TIndex1, TIndex2>
	{
		IHappilPropertyGetter Get(Action<IHappilMethodBody<TProperty>, HappilArgument<TIndex1>, HappilArgument<TIndex2>> body);
		IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<TIndex2>, HappilArgument<TProperty>> body);
		HappilField<TProperty> BackingField { get; }
	}
	public interface IHappilPropertyGetter
	{
	}
	public interface IHappilPropertySetter
	{
	}
}
