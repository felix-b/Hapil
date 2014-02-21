using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;
using Happil.Selectors;

namespace Happil
{
	public static class HappilOperandExtensions
	{
		internal static void EmitTarget(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitLoad(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitStore(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitAddress(this IHappilOperand operand, ILGenerator il)
		{
			((IHappilOperandEmitter)operand).EmitAddress(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static HappilClass GetOwnerClass(this IHappilOperand operand)
		{
			return ((IHappilOperandInternals)operand).OwnerClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static HappilMethod GetOwnerMethod(this IHappilOperand operand)
		{
			return ((IHappilOperandInternals)operand).OwnerMethod;
		}
	}
}
