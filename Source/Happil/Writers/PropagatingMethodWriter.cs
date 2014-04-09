using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public class PropagatingMethodWriter : MethodWriterBase
	{
		private readonly IOperand m_Target;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropagatingMethodWriter(MethodMember ownerMethod, IOperand target)
			: base(ownerMethod, mode: MethodWriterModes.Normal, attachToOwner: true)
		{
			m_Target = target;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			LocalOperand<TypeTemplate.TReturn> returnValueLocal = null;

			if ( !OwnerMethod.IsVoid )
			{
				returnValueLocal = Local<TypeTemplate.TReturn>();
			}

			StatementScope.Current.AddStatement(new PropagateCallStatement(OwnerMethod, m_Target, returnValueLocal));

			if ( !OwnerMethod.IsVoid )
			{
				AddReturnStatement<TypeTemplate.TReturn>(returnValueLocal);
			}

			base.Flush();
		}
	}
}
