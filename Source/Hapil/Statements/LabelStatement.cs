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
	internal class LabelStatement : StatementBase
	{
		private readonly MethodMember m_OwnerMethod;
		private readonly Label m_Label;
		private readonly StatementScope m_HomeScope;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LabelStatement(MethodMember ownerMethod, StatementScope homeScope)
		{
			m_OwnerMethod = ownerMethod;
			m_Label = ownerMethod.MethodFactory.GetILGenerator().DefineLabel();
			m_HomeScope = homeScope;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			il.MarkLabel(m_Label);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void MarkLabel()
		{
			m_HomeScope.AddStatement(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return m_HomeScope;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Label Label
		{
			get
			{
				return m_Label;
			}
		}
	}
}
