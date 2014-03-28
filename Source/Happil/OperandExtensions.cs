using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;

namespace Happil
{
	public static class OperandExtensions
	{
		internal static void EmitTarget(this IOperand operand, ILGenerator il)
		{
			((IOperandEmitter)operand).EmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitLoad(this IOperand operand, ILGenerator il)
		{
			((IOperandEmitter)operand).EmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitStore(this IOperand operand, ILGenerator il)
		{
			((IOperandEmitter)operand).EmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitAddress(this IOperand operand, ILGenerator il)
		{
			((IOperandEmitter)operand).EmitAddress(il);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//internal static HappilClass GetOwnerClass(this IHappilOperand operand)
		//{
		//	return ((IHappilOperandInternals)operand).OwnerClass;
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//internal static HappilMethod GetOwnerMethod(this IHappilOperand operand)
		//{
		//	return ((IHappilOperandInternals)operand).OwnerMethod;
		//}
	}
}
