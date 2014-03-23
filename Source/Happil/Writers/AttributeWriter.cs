using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Writers
{
	public class AttributeWriter
	{
		private readonly Action<CustomAttributeBuilder> m_ApplyAction;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal AttributeWriter(Action<CustomAttributeBuilder> applyAction)
		{
			m_ApplyAction = applyAction;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeWriter Set<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> arguments = null) where TAttribute : Attribute
		{
			var argumentWriter = new AttributeArgumentWriter<TAttribute>();

			if ( arguments != null )
			{
				arguments(argumentWriter);
			}

			var attributeBuilder = argumentWriter.GetAttributeBuilder();
			m_ApplyAction(attributeBuilder);
			
			return this;
		}
	}
}
