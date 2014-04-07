using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	internal class TransparentMethodWriter : MethodWriterBase
	{
		public TransparentMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod, MethodWriterModes.Normal, attachToOwner: false)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TypeTemplate.TReturn> operand)
		{
			AddReturnStatement(operand.OrNullConstant());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			AddReturnStatement();
		}
	}
}
