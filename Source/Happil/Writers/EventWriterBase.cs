using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public abstract class EventWriterBase
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

		protected internal virtual void Flush()
		{
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
