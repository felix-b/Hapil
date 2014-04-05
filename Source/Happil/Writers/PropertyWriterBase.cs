using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public abstract class PropertyWriterBase : MemberWriterBase
	{
		private readonly PropertyMember m_OwnerProperty;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected PropertyWriterBase(PropertyMember ownerProperty)
		{
			m_OwnerProperty = ownerProperty;
			ownerProperty.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new AttributeArgumentWriter<TAttribute>(values);
			m_OwnerProperty.PropertyBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyMember OwnerProperty
		{
			get
			{
				return m_OwnerProperty;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddAttributes(Func<PropertyMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				AttributeWriter.Include(attributeWriterFactory(m_OwnerProperty));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void SetCustomAttribute(CustomAttributeBuilder attribute)
		{
			m_OwnerProperty.PropertyBuilder.SetCustomAttribute(attribute);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyWriterGetter
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyWriterSetter
		{
		}
	}
}
