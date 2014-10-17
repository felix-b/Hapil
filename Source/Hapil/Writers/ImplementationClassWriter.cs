using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		private readonly Type m_BaseType;
		private readonly TypeMemberCache m_Members;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter(ClassType ownerClass)
			: this(ownerClass, typeof(TBase))
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter(ClassType ownerClass, Type baseType)
			: base(ownerClass)
		{
			m_BaseType = TypeTemplate.Resolve(baseType);
			
			if ( m_BaseType.IsInterface )
			{
				ownerClass.AddInterface(m_BaseType);
			}

			m_Members = TypeMemberCache.Of(m_BaseType);
			//TODO: validate base type
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var writer = new AttributeArgumentWriter<TAttribute>(values);
			OwnerClass.TypeBuilder.SetCustomAttribute(writer.GetAttributeBuilder());
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override void Flush()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class NA
		{
		}
	}
}
