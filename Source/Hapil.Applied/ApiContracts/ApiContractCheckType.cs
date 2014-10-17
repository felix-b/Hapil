using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Applied.ApiContracts
{	
	public enum ApiContractCheckType
	{
		NotNull,
		NotEmpty,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		MinLength,
		MaxLength,
		Custom = 1000
	}
}
