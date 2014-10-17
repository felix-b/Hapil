using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Hapil.Statements
{
	internal interface ILeaveStatement
	{
		void Emit(ILGenerator il);
		StatementScope HomeScope { get; }
	}
}
