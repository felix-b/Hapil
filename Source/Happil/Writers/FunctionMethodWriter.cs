
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public class FunctionMethodWriter<TReturn> : MethodWriterBase
	{
		public FunctionMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IOperand<TReturn> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(operand));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(TReturn constantValue)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(new ConstantOperand<TReturn>(constantValue)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
		}
	}
}
