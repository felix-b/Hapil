using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public interface IProperty
	{
		IPropertyGetter Get(params Func<IMethodBody, IStatement>[] statements);
		IPropertySetter Set(params Func<IMethodBody, IStatement>[] statements);
		IField BackingField { get; }
	}
}
