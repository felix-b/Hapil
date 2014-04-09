using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Operands;

namespace Happil.Statements
{
	internal class PropagateCallStatement : StatementBase
	{
		private readonly MethodMember m_OwnerMethod;
		private readonly IOperand m_Target;
		private readonly IOperand m_ReturnValueLocal;

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
	}
}
