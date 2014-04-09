using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Decorators
{
	public class EventDecorationBuilder
	{
		private readonly EventMember m_OwnerEvent;
		private MethodDecorationBuilder m_AddOn;
		private MethodDecorationBuilder m_RemoveOn;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal EventDecorationBuilder(EventMember ownerEvent)
		{
			m_OwnerEvent = ownerEvent;
			m_AddOn = null;
			m_RemoveOn = null;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public EventDecorationBuilder Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var attributes = new AttributeWriter();
			attributes.Set<TAttribute>(values);
			m_OwnerEvent.AddAttributes(p => attributes);
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder AddOn()
		{
			if ( m_AddOn == null )
			{
				m_AddOn = new DecoratingMethodWriter(m_OwnerEvent.AddMethod).DecorationBuilder;
			}

			return m_AddOn;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder RemoveOn()
		{
			if ( m_RemoveOn == null )
			{
				m_RemoveOn = new DecoratingMethodWriter(m_OwnerEvent.RemoveMethod).DecorationBuilder;
			}

			return m_RemoveOn;
		}
	}
}
