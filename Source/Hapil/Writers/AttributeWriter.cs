using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;

namespace Hapil.Writers
{
	public class AttributeWriter
	{
		private readonly List<AttributeArgumentWriter> m_Attributes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal AttributeWriter()
		{
			m_Attributes = new List<AttributeArgumentWriter>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeWriter Set<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> arguments = null) where TAttribute : Attribute
		{
			var attribute = new AttributeArgumentWriter<TAttribute>(arguments);
			m_Attributes.Add(attribute);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal CustomAttributeBuilder[] GetAttributes()
		{
			return m_Attributes.Select(attr => attr.GetAttributeBuilder()).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void Include(AttributeWriter other)
		{
			m_Attributes.AddRange(other.m_Attributes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator Func<FieldMember, AttributeWriter>(AttributeWriter value)
		{
			return f => value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator Func<MethodMember, AttributeWriter>(AttributeWriter value)
		{
			return m => value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator Func<PropertyMember, AttributeWriter>(AttributeWriter value)
		{
			return p => value;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator Func<EventMember, AttributeWriter>(AttributeWriter value)
		{
			return e => value;
		}
	}
}
