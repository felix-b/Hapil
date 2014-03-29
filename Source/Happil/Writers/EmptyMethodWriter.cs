using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Statements;

namespace Happil.Writers
{
	public class EmptyMethodWriter : MethodWriterBase
	{
		public EmptyMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( !OwnerMethod.Signature.IsVoid )
			{
				OwnerMethod.AddStatement(new ReturnStatement<TypeTemplate.TReturn>(Default<TypeTemplate.TReturn>()));
			}
		}
	}
}
