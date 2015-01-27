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
        private readonly bool m_IsExplicitInterfaceImplementation;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ImplementationClassWriter(ClassType ownerClass, bool isExplicitInterfaceImplementation = false)
			: this(ownerClass, typeof(TBase), isExplicitInterfaceImplementation)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ImplementationClassWriter(ClassType ownerClass, Type baseType, bool isExplicitInterfaceImplementation = false)
			: base(ownerClass)
		{
			m_BaseType = TypeTemplate.Resolve(baseType);
			
			if ( m_BaseType.IsInterface )
			{
				ownerClass.AddInterface(m_BaseType);
			}

			m_Members = TypeMemberCache.Of(m_BaseType);
            m_IsExplicitInterfaceImplementation = isExplicitInterfaceImplementation;

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

        public bool IsExplicitInterfaceImplementation
        {
            get { return m_IsExplicitInterfaceImplementation; }
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
