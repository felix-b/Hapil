using System;
using System.Reflection.Emit;

namespace Happil.Fluent
{
	public class HappilClass
	{
		private TypeBuilder m_TypeBuilder = null;
		private Type m_BuiltType = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
        }


		public HappilClass Inherit<TBase>(params Func<HappilClassBody<TBase>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public HappilClass Inherit(object baseType, params Func<HappilClassBody<object>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public HappilClass Implement<TInterface>(params Func<HappilClassBody<TInterface>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public HappilClass Implement(Type interfaceType, params Func<HappilClassBody<object>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public Type CreateType()
		{
			m_BuiltType = m_TypeBuilder.CreateType();
			return m_BuiltType;
		}

		public Delegate[] GetFactoryMethods()
		{
			return new Delegate[0];
		}
	}
}
