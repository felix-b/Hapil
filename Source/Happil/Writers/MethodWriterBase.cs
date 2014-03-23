using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public abstract class MethodWriterBase
	{
		private readonly MethodMember m_OwnerMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------


	}
}
