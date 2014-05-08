using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Operands
{
	interface ILocal : IMutableOperand
	{
		void Declare(ILGenerator il);
	}
}
