using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IHappilClassDefinition
	{
		Type BaseType { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal interface IHappilClassDefinitionInternals : IHappilClassDefinition
	{
		HappilClass HappilClass { get; }
	}
}
