using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil
{
	public class CompositeConvention : IObjectFactoryConvention
	{
		private readonly IObjectFactoryConvention[] m_ContainedConventions;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CompositeConvention(params IObjectFactoryConvention[] containedConventions)
		{
			m_ContainedConventions = containedConventions;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IObjectFactoryConvention Members

		public virtual bool ShouldApply(ObjectFactoryContext context)
		{
			return m_ContainedConventions.Any(c => c.ShouldApply(context));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void Apply(ObjectFactoryContext context)
		{
			foreach ( var convention in m_ContainedConventions )
			{
				if ( convention.ShouldApply(context) )
				{
					convention.Apply(context);
				}
			}
		}

		#endregion
	}
}
