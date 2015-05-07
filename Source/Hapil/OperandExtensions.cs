using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;

namespace Hapil
{
	public static class OperandExtensions
	{
		public static bool IsDefined(this IOperand operand)
		{
			return !object.ReferenceEquals(operand, null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static bool IsByRefArgument(this IOperand operand)
        {
            var argument = (operand as IArgument);
            return (argument != null && argument.IsByRef);
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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal static void EmitTargetAndAddress(this IOperand operand, ILGenerator il)
        {
            var emitter = (operand as IOperandEmitter);

            if ( emitter != null )
            {
                if ( !operand.IsByRefArgument() )
                {
                    emitter.EmitTarget(il);
                }

                emitter.EmitAddress(il);
            }
        }
	}
}
