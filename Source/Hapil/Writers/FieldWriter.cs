using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
{
	public class FieldWriter : MemberWriterBase
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
			AttributeWriter.Set<TAttribute>(values);
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

		protected internal void AddAttributes(Func<FieldMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				AttributeWriter.Include(attributeWriterFactory(m_OwnerField));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void SetCustomAttribute(CustomAttributeBuilder attribute)
		{
			m_OwnerField.FieldBuilder.SetCustomAttribute(attribute);
		}
	}
}
