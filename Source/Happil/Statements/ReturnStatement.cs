using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class ReturnStatement : Statement
	{
		public override void Emit(ILGenerator il)
		{
			il.Emit(OpCodes.Ret);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class ReturnStatement<T> : Statement
	{
		private IHappilOperand<T> m_Operand;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ReturnStatement(IHappilOperand<T> operand)
		{
			m_Operand = operand;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			//TODO: push operand onto stack
			il.Emit(OpCodes.Ret);
		}
	}
}
