using System;

namespace Happil.Fluent
{
	public interface IClass
	{
		IClass Inherits<TBase>(params Func<IHappilClassBody<TBase>, IMember>[] members);
		IClass Implements<TInterface>(params Func<IHappilClassBody<TInterface>, IMember>[] members);
		Type CreateType();
	}
}
