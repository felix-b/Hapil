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
        private readonly InterfaceImplementationKind m_ImplementationKind;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ImplementationClassWriter(
            ClassType ownerClass, 
            InterfaceImplementationKind implementationKind = 
            InterfaceImplementationKind.ImplicitNonVirtual)
            : this(ownerClass, typeof(TBase), implementationKind)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ImplementationClassWriter(
            ClassType ownerClass, 
            Type baseType, 
            InterfaceImplementationKind implementationKind = InterfaceImplementationKind.ImplicitNonVirtual)
			: base(ownerClass)
		{
			m_BaseType = TypeTemplate.Resolve(baseType);
			
			if ( m_BaseType.IsInterface )
			{
				ownerClass.AddInterface(m_BaseType);
			}

			m_Members = TypeMemberCache.Of(m_BaseType);
            m_ImplementationKind = implementationKind;

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

        public InterfaceImplementationKind ImplementationKind
        {
            get { return m_ImplementationKind; }
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
