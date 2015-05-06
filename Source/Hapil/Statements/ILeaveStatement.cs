using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;

namespace Hapil.Statements
{
	internal interface ILeaveStatement
	{
        void Emit(ILGenerator il, MethodMember ownerMethod);
		StatementScope HomeScope { get; }
	}
}
