using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal abstract class StatementBase
	{
		public abstract void Emit(ILGenerator il);
	}
}
