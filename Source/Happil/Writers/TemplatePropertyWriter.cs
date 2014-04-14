using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class TemplatePropertyWriter : PropertyWriterBase
	{
		public TemplatePropertyWriter(
			PropertyMember ownerProperty,
			Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterGetter> getterScript,
			Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterSetter> setterScript)
			: base(ownerProperty)
		{
			if ( getterScript != null )
			{
				getterScript(this);
			}

			if ( setterScript != null )
			{
				setterScript(this);
			}
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<TypeTemplate.TProperty> BackingField
		{
			get
			{
				return OwnerProperty.BackingField.AsOperand<TypeTemplate.TProperty>();
			}
		}
	}
}
