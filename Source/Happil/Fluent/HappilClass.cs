using System;
using System.Reflection.Emit;

namespace Happil.Fluent
{
	public class HappilClass
	{
		private readonly TypeBuilder m_TypeBuilder;
		private Type m_BuiltType = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass Inherit<TBase>(params Func<HappilClassBody<TBase>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass Inherit(object baseType, params Func<HappilClassBody<object>, IMember>[] members)
		{
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass Implement<TInterface>(params Func<HappilClassBody<TInterface>, IMember>[] members)
		{
			m_TypeBuilder.AddInterfaceImplementation(typeof(TInterface));
			//TODO: add members to member list
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass Implement(Type interfaceType, params Func<HappilClassBody<object>, IMember>[] members)
		{
			m_TypeBuilder.AddInterfaceImplementation(interfaceType);
			//TODO: add members to member list
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Called by HappilObjectFactoryBase.TypeEntry constructor.
		/// </summary>
		internal Type CreateType()
		{
			m_BuiltType = m_TypeBuilder.CreateType();
			//TODO: add members to member list
			return m_BuiltType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Called by HappilObjectFactoryBase.TypeEntry constructor.
		/// </summary>
		internal Delegate[] GetFactoryMethods()
		{
			//TODO: this is a temporary implementation; the real implementation must be Reflection-free as explained here:
			//Factory method should be delegate to a static method in the generated type
			//The static method should invoke correct constructor and return the created instance.
			return new Delegate[] {
				new Func<object>(() => Activator.CreateInstance(m_BuiltType))
			};
		}
	}
}
