using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Conventions
{
	public interface IObjectFactoryConvention
	{
		void Apply(DynamicModule module, ref TypeKey typeKey, ref ClassType classType);
	}
}
