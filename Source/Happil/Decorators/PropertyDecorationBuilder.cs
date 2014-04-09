using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Decorators
{
	public class PropertyDecorationBuilder
	{
		private readonly PropertyMember m_OwnerProperty;
		private MethodDecorationBuilder m_Getter;
		private MethodDecorationBuilder m_Setter;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal PropertyDecorationBuilder(PropertyMember ownerProperty)
		{
			m_OwnerProperty = ownerProperty;
			m_Getter = null;
			m_Setter = null;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyDecorationBuilder Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var attributes = new AttributeWriter();
			attributes.Set<TAttribute>(values);
			m_OwnerProperty.AddAttributes(p => attributes);
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder Getter()
		{
			if ( m_Getter == null )
			{
				var getterMethod = m_OwnerProperty.GetterMethod;
				m_Getter = new DecoratingMethodWriter(getterMethod).DecorationBuilder;
			}

			return m_Getter;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder Setter()
		{
			if ( m_Setter == null )
			{
				var setterMethod = m_OwnerProperty.SetterMethod;
				m_Setter = new DecoratingMethodWriter(setterMethod).DecorationBuilder;
			}

			return m_Setter;
		}
	}
}
