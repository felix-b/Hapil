﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public class DefaultConstructorWriter : ConstructorWriter 
	{
		public DefaultConstructorWriter(MethodMember ownerMethod)
			: base(ownerMethod, script: null)
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
