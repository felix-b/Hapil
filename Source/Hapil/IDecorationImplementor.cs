using Hapil.Fluent;

namespace Hapil
{
	public interface IDecorationImplementor
	{
		void ImplementDecoration<TBase>(IHappilClassBody<TBase> classDefinition);
	}
}