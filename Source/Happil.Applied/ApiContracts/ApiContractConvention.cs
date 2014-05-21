using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Applied.ApiContracts
{
	internal class ApiContractConvention : DecorationConvention
	{
		public ApiContractConvention()
			: base(Will.DecorateConstructors | Will.DecorateMethods | Will.DecorateProperties)
		{
		}
	}
}
