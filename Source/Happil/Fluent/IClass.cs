using System;

namespace Happil.Fluent
{
	public interface IClass
	{
		IClass Inherit<TBase>(params Func<IHappilClassBody<TBase>, IMember>[] members);
		IClass Implement<TInterface>(params Func<IHappilClassBody<TInterface>, IMember>[] members);
		Type CreateType();
	}
}
