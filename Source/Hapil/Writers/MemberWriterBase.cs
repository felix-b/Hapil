using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;

namespace Hapil.Writers
{
	public abstract class MemberWriterBase
	{
		private readonly AttributeWriter m_AttributeWriter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MemberWriterBase()
		{
			m_AttributeWriter = new AttributeWriter();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void SetCustomAttribute(CustomAttributeBuilder attribute);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal virtual void Flush()
		{
			foreach ( var attribute in m_AttributeWriter.GetAttributes() )
			{
				SetCustomAttribute(attribute);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddAttributes(AttributeWriter attributeWriter)
		{
			if ( attributeWriter != null )
			{
				m_AttributeWriter.Include(attributeWriter);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected AttributeWriter AttributeWriter
		{
			get
			{
				return m_AttributeWriter;
			}
		}
	}
}
