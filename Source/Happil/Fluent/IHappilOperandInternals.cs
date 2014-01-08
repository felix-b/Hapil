using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	internal interface IHappilOperandInternals
	{
		void EmitTarget(ILGenerator il);
		void EmitLoad(ILGenerator il);
		void EmitStore(ILGenerator il);
		void EmitAddress(ILGenerator il);
		HappilClass OwnerClass { get; }
		Type OperandType { get; }
	}
}












