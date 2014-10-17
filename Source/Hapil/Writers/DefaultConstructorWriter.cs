using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Members;

namespace Hapil.Writers
{
	public class DefaultConstructorWriter : ConstructorWriter 
	{
		public DefaultConstructorWriter(ConstructorMember ownerConstructor)
			: base(ownerConstructor, script: null)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			this.Base();
			base.Flush();
		}
	}
}
