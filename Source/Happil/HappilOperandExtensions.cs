using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	internal static class HappilOperandExtensions
	{
		public static void EmitTarget(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void EmitLoad(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void EmitStore(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void EmitAddress(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitAddress(il);
		}
	}
}
