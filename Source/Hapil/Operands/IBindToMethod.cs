using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;

namespace Hapil.Operands
{
	internal interface IBindToMethod
	{
		void BindToMethod(MethodMember ownerMethod);
		void ResetBinding();
		bool IsBound { get; }
	}
}
