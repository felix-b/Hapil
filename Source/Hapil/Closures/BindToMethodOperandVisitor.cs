using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Closures
{
	internal class BindToMethodOperandVisitor : OperandVisitorBase
	{
		private readonly MethodMember m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public BindToMethodOperandVisitor(MethodMember method)
		{
			m_Method = method;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool OnFilterOperand(IOperand operand)
		{
			return (operand is IBindToMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			var bindableOperand = (IBindToMethod)operand;

			if ( !bindableOperand.IsBound )
			{
				bindableOperand.BindToMethod(m_Method);
			}
		}
	}
}
