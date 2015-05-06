using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class ThrowStatement : StatementBase
	{
		private readonly Type m_ExceptionType;
		private readonly string m_Message;
		private readonly ConstructorInfo m_Constructor;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ThrowStatement(Type exceptionType, string message)
		{
			m_ExceptionType = exceptionType;
			m_Message = message;
			
			m_Constructor = (
				message != null ?
				exceptionType.GetConstructor(new[] { typeof(string) }) :
				exceptionType.GetConstructor(Type.EmptyTypes));

			if ( m_Constructor == null )
			{
				throw new ArgumentException("Could not find constructor on specified exception type.");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilStatement Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			if ( m_Message != null )
			{
				il.Emit(OpCodes.Ldstr, m_Message);
			}

			il.Emit(OpCodes.Newobj, m_Constructor);
			il.Emit(OpCodes.Throw);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}

		#endregion
	}
}
