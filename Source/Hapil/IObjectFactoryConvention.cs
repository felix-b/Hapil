using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;

namespace Hapil
{
	public interface IObjectFactoryConvention
	{
		bool ShouldApply(ObjectFactoryContext context);
		void Apply(ObjectFactoryContext context);
	}
}
