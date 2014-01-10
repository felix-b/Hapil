using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	internal interface IHappilOperandEmitter
	{
		void EmitTarget(ILGenerator il);
		void EmitLoad(ILGenerator il);
		void EmitStore(ILGenerator il);
		void EmitAddress(ILGenerator il);
	}
}
