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
			: base(ownerMethod, attachToOwner: false)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TypeTemplate.TReturn> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TypeTemplate.TReturn>(operand));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			StatementScope.Current.AddStatement(new ReturnStatement());
		}
	}
}
