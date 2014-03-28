using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Operands
{
	internal interface IOperandEmitter
	{
		void EmitTarget(ILGenerator il);
		void EmitLoad(ILGenerator il);
		void EmitStore(ILGenerator il);
		void EmitAddress(ILGenerator il);
		bool HasTarget { get; }
		bool IsMutable { get; }
		bool CanEmitAddress { get; }
	}
}
