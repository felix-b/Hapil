using Happil.Fluent;

namespace Happil
{
	public interface IDecorationImplementor
	{
		void ImplementDecoration<TBase>(IHappilClassBody<TBase> classDefinition);
	}
}