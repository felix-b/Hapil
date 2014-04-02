using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class TemplatePropertyWriter : PropertyWriterBase
	{
		public TemplatePropertyWriter(PropertyMember ownerProperty, Action<TemplatePropertyWriter> script)
			: base(ownerProperty)
		{
			script(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterGetter Get(Action<FunctionMethodWriter<TypeTemplate.TProperty>> script)
		{
			var writer = new FunctionMethodWriter<TypeTemplate.TProperty>(OwnerProperty.GetterMethod, script);
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterSetter Set(Action<VoidMethodWriter, Argument<TypeTemplate.TProperty>> script)
		{
			var indexArgumentCount = OwnerProperty.PropertyDeclaration.GetIndexParameters().Length;
			var writer = new VoidMethodWriter(
				OwnerProperty.SetterMethod,
				w => script(w, w.Argument<TypeTemplate.TProperty>(indexArgumentCount + 1)));
			
			return null;
		}
	}
}
