using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Happil.Fluent
{
	internal class HappilClass
	{
		private readonly TypeBuilder m_TypeBuilder;
		private Type m_BuiltType = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> GetBody<TBase>()
		{
			return new HappilClassBody<TBase>(happilClass: this);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Inherit<TBase>(params Func<IHappilClassBody<TBase>, IHappilMember>[] members)
		//{
		//	throw new NotImplementedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Inherit(object baseType, params Func<IHappilClassBody<object>, IHappilMember>[] members)
		//{
		//	return this;
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ImplementInterface(Type interfaceType)
		{
			m_TypeBuilder.AddInterfaceImplementation(interfaceType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilMethod DefineMethod(MethodInfo declaration)
		{
			var implementation = m_TypeBuilder.DefineMethod(declaration.Name, 
				MethodAttributes.Final |
				MethodAttributes.HideBySig | 
				MethodAttributes.NewSlot |
				MethodAttributes.Public |
				MethodAttributes.Virtual);

			implementation.GetILGenerator().Emit(OpCodes.Ret);

			m_TypeBuilder.DefineMethodOverride(implementation, declaration);

			return new HappilMethod(this, implementation);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilMember>[] members)
		//{
		//	m_TypeBuilder.AddInterfaceImplementation(interfaceType);
		//	//TODO: add members to member list
		//	return this;
		//}

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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------


	}
}
