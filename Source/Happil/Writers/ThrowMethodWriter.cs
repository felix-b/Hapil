using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Statements;

namespace Happil.Writers
{
	public class ThrowMethodWriter : MethodWriterBase
	{
		private readonly string m_Message;
		private readonly Type m_ExceptionType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ThrowMethodWriter(MethodMember ownerMethod, Type exceptionType, string message)
			: base(ownerMethod)
		{
			m_Message = message;
			m_ExceptionType = exceptionType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			AddStatement(new ThrowStatement(m_ExceptionType, m_Message));
			base.Flush();
		}
	}
}
