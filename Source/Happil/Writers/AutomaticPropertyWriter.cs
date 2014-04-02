using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public class AutomaticPropertyWriter : PropertyWriterBase
	{
		public AutomaticPropertyWriter(PropertyMember ownerProperty)
			: base(ownerProperty)
		{
			if ( ownerProperty.GetterMethod != null )
			{
				var getter = new TemplateMethodWriter(
					OwnerProperty.GetterMethod, 
					w => w.Return(ownerProperty.BackingField.AsOperand<TypeTemplate.TReturn>()));
			}

			if ( ownerProperty.SetterMethod != null )
			{
				var setter = new TemplateMethodWriter(
					OwnerProperty.SetterMethod,
					w => ownerProperty.BackingField.AsOperand<TypeTemplate.TArg1>().Assign(w.Arg1<TypeTemplate.TArg1>()));
			}
		}
	}
}
