using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Closures;
using Hapil.Operands;

namespace Hapil.UnitTests
{
	internal static class OperandExtensions
	{
		public static string[] ToStringArray(this IEnumerable<IOperand> operands)
		{
			return operands.Select(operand => operand.ToString()).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static string[] ToStringArray(this IEnumerable<OperandCapture> operands)
		{
			return operands.Select(capture => capture.ToString()).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static OperandCapture Find(this IEnumerable<OperandCapture> operands, string stringValue)
		{
			return operands.Single(capture => capture.ToString() == stringValue);
		}
	}
}
