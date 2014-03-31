﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public class TemplateMethodWriter : MethodWriterBase
	{
		public TemplateMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod)
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
		}
	}
}
