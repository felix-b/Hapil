using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Members;

namespace Happil.Writers
{
	public class DecoratingClassWriter : ClassWriterBase
	{
		private readonly DecorationConvention m_Decorator;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DecoratingClassWriter(ClassType ownerClass, DecorationConvention decorator)
			: base(ownerClass)
		{
			m_Decorator = decorator;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DecoratingClassWriter Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var writer = new AttributeArgumentWriter<TAttribute>(values);
			OwnerClass.TypeBuilder.SetCustomAttribute(writer.GetAttributeBuilder());
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			m_Decorator.VisitClass(OwnerClass, this);
		}
	}
}
