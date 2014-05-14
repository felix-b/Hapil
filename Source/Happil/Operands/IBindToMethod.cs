using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Operands
{
	internal interface IBindToMethod
	{
		void BindToMethod(MethodMember ownerMethod);
		bool IsBound { get; }
	}
}
