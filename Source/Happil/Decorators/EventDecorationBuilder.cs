using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Decorators
{
	public class EventDecorationBuilder
	{
		private readonly EventMember m_OwnerEvent;
		private MethodDecorationBuilder m_OnAdd;
		private MethodDecorationBuilder m_OnRemove;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal EventDecorationBuilder(EventMember ownerEvent)
		{
			m_OwnerEvent = ownerEvent;
			m_OnAdd = null;
			m_OnRemove = null;
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

		public MethodDecorationBuilder OnAdd()
		{
			if ( m_OnAdd == null )
			{
				m_OnAdd = new DecoratingMethodWriter(m_OwnerEvent.AddMethod).DecorationBuilder;
			}

			return m_OnAdd;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnRemove()
		{
			if ( m_OnRemove == null )
			{
				m_OnRemove = new DecoratingMethodWriter(m_OwnerEvent.RemoveMethod).DecorationBuilder;
			}

			return m_OnRemove;
		}

	}
}
