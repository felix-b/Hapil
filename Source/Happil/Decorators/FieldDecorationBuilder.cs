using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Decorators
{
	public class FieldDecorationBuilder
	{
		private readonly FieldMember m_OwnerField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FieldDecorationBuilder(FieldMember ownerField)
		{
			m_OwnerField = ownerField;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldDecorationBuilder Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var attributes = new AttributeWriter();
			attributes.Set<TAttribute>(values);
			m_OwnerField.AddAttributes(p => attributes);
			return this;
		}
	}
}