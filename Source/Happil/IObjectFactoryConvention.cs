using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil
{
	public interface IObjectFactoryConvention
	{
		bool ShouldApply(ObjectFactoryContext context);
		void Apply(ObjectFactoryContext context);
	}
}
