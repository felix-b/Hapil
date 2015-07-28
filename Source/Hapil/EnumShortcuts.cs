using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Expressions;
using Hapil.Operands;

namespace Hapil
{
    public static class EnumShortcuts
    {
        public static Operand<bool> EnumHasFlag<TEnum>(this IOperand<TEnum> value, TEnum flag)
            where TEnum : struct
        {
            if ( !typeof(TEnum).IsEnum )
            {
                throw new ArgumentException("Operands must be of enum type.");
            }

            return ((value.CastTo<int>() & new Constant<int>((int)(object)flag)) != 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Operand<bool> EnumHasFlag<TEnum>(this IOperand<TEnum> value, IOperand<TEnum> flag)
            where TEnum : struct
        {
            if ( !typeof(TEnum).IsEnum )
            {
                throw new ArgumentException("Operands must be of enum type.");
            }

            return ((value.CastTo<int>() & flag.CastTo<int>()) != 0);
        }
    }
}
