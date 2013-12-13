using Happil.Fluent;

namespace Happil.Expressions
{
	internal abstract class HappilExpression<T> : HappilOperand<T>
	{
		internal HappilExpression(HappilMethod ownerMethod)
			: base(ownerMethod)
		{
		}
	}
}
	