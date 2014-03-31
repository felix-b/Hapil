using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class FieldWriter
	{
		private readonly FieldMember m_OwnerField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldWriter(FieldMember ownerField)
		{
			m_OwnerField = ownerField;
			ownerField.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new AttributeArgumentWriter<TAttribute>(values);
			m_OwnerField.FieldBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
			return m_OwnerField;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember OwnerField
		{
			get
			{
				return m_OwnerField;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void Flush()
		{
			// nothing
		}
	}
}
