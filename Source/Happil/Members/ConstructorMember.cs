using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Members
{
	public class ConstructorMember : MethodMember
	{
		internal ConstructorMember(ClassType ownerClass, ConstructorMethodFactory methodFactory)
			: base(ownerClass, methodFactory)
		{
		}
	}
}
