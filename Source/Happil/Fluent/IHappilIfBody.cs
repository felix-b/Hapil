using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilIfBody
	{
		IHappilIfBodyThen Then(Action thenBodyDefinition);
	}
	public interface IHappilIfBodyThen
	{
		IHappilIfBody ElseIf(IHappilOperand<bool> condition);
		void Else(Action elseBodyDefinition);
	}
}
