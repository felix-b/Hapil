using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	internal class HappilAttributes : IHappilAttributes
	{
		private readonly List<HappilAttributeBuilder> m_AttributeBuilders = new List<HappilAttributeBuilder>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilAttributes Members

		public IHappilAttributes Set<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null) where TAttribute : Attribute
		{
			m_AttributeBuilders.Add(new HappilAttributeBuilder<TAttribute>(values));
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CustomAttributeBuilder[] GetAttributes()
		{
			return m_AttributeBuilders.Select(builder => builder.GetAttributeBuilder()).ToArray();
		}
	}
}
