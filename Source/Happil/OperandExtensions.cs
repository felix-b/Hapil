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
		public static bool IsDefined(this IOperand operand)
		{
			return !object.ReferenceEquals(operand, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitTarget(this IOperand operand, ILGenerator il)
		{
			if ( operand != null )
			{
				((IOperandEmitter)operand).EmitTarget(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void EmitLoad(this IOperand operand, ILGenerator il)
		{
			if ( operand != null )
			{
				((IOperandEmitter)operand).EmitLoad(il);
			}
			else
			{
				il.Emit(OpCodes.Ldnull);
			}
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

		//internal static HappilClass GetOwnerClass(this IOperand operand)
		//{
		//	return ((IOperandInternals)operand).OwnerClass;
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//internal static HappilMethod GetOwnerMethod(this IOperand operand)
		//{
		//	return ((IOperandInternals)operand).OwnerMethod;
		//}
	}
}
