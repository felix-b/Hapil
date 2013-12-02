using System;
using System.Reflection.Emit;

namespace Happil.Fluent
{
	public class HappilClass
	{
		private TypeBuilder m_TypeBuilder = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
        }


		public HappilClass Inherit<TBase>(params Func<HappilClassBody<TBase>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public HappilClass Implement<TInterface>(params Func<HappilClassBody<TInterface>, IMember>[] members)
		{
			throw new NotImplementedException();
		}

		public Type CreateType()
		{
			return m_TypeBuilder.CreateType();
		}
	}
}
