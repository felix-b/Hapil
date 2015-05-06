using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	/// <summary>
	/// Return statement for void methods
	/// </summary>
	internal class ReturnStatement : StatementBase, ILeaveStatement
	{
        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			il.Emit(OpCodes.Br, ownerMethod.GetMethodReturnLabel());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return "Return";
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return StatementScope.Current.Root;
			}
		}
    }

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Return statement for non-void methods
	/// </summary>
	/// <typeparam name="T">
	/// The type of the return value.
	/// </typeparam>
	internal class ReturnStatement<T> : StatementBase, ILeaveStatement
	{
		private IOperand<T> m_Operand;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ReturnStatement(IOperand<T> operand)
		{
			m_Operand = operand;
			StatementScope.Current.Consume(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
        {
            var returnValueLocal = ownerMethod.GetReturnValueLocal<T>();
            var returnLabel = ownerMethod.GetMethodReturnLabel();

			m_Operand.EmitTarget(il);
			m_Operand.EmitLoad(il);
            
            returnValueLocal.EmitStore(il);

			il.Emit(OpCodes.Br, returnLabel);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Return[{0}]", m_Operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return StatementScope.Current.Root;
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool WillLeaveMethod
        {
            get
            {
                return true;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Label? LeaveLabel { get; set; }
    }
}
