﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;

namespace Hapil.Operands
{
	interface ILocal : IMutableOperand
	{
		void Declare(ILGenerator il);
		LocalBuilder LocalBuilder { get; }
	}
}
