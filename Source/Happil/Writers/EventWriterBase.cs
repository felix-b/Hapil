using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public abstract class EventWriterBase : MemberWriterBase
	{
		private readonly EventMember m_OwnerEvent;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected EventWriterBase(EventMember ownerEvent)
		{
			m_OwnerEvent = ownerEvent;
			ownerEvent.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new AttributeArgumentWriter<TAttribute>(values);
			m_OwnerEvent.EventBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventMember OwnerEvent
		{
			get
			{
				return m_OwnerEvent;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddAttributes(Func<EventMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				AttributeWriter.Include(attributeWriterFactory(m_OwnerEvent));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void SetCustomAttribute(CustomAttributeBuilder attribute)
		{
			m_OwnerEvent.EventBuilder.SetCustomAttribute(attribute);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IEventWriterAddOn
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IEventWriterRemoveOn
		{
		}
	}
}
