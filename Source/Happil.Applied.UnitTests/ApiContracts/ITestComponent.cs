using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Applied.ApiContracts;

namespace Happil.Applied.UnitTests.ApiContracts
{
	public interface ITestComponent
	{
		void Echo([NotNull] string text);
	}
}
