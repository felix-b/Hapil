using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class PropagateCallStatement : StatementBase
	{
		private readonly MethodMember m_OwnerMethod;
		private IOperand m_Target;
		private IOperand m_ReturnValueLocal;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropagateCallStatement(MethodMember ownerMethod, IOperand target, IOperand returnValueLocal)
		{
			m_OwnerMethod = ownerMethod;
			m_Target = target;
			m_ReturnValueLocal = returnValueLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			var arguments = new IOperand[m_OwnerMethod.Signature.ArgumentCount];
			m_OwnerMethod.TransparentWriter.ForEachArgument((arg, index) => arguments[index] = arg);

			Helpers.EmitCall(il, m_Target, m_OwnerMethod.MethodDeclaration, arguments);

			if ( !m_OwnerMethod.IsVoid && !object.ReferenceEquals(m_ReturnValueLocal, null) )
			{
				m_ReturnValueLocal.EmitStore(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Target);
			visitor.VisitOperand(ref m_ReturnValueLocal);
		}
	}
}
