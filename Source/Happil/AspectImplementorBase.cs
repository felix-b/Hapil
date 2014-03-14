using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	public abstract class AspectImplementorBase : IDecorationImplementor
	{
		#region IAspectImplementor Members

		void IDecorationImplementor.ImplementDecoration<TBase>(IHappilClassBody<TBase> classDefinition)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		
	}
}
