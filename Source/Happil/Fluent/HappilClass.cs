using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
    public class HappilClass: IClass
    {
        private TypeBuilder m_TypeBuilder = null;

        public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
        }

        public IClass Inherits<TBase>(params Func<IHappilClassBody<TBase>, IMember>[] members)
        {
            throw new NotImplementedException();
        }

        public IClass Implements<TInterface>(params Func<IHappilClassBody<TInterface>, IMember>[] members)
        {
            throw new NotImplementedException();
        }

        public Type CreateType()
        {
            return m_TypeBuilder.CreateType();
        }

        public IClass Inherit<TBase>(params Func<IHappilClassBody<TBase>, IMember>[] members)
        {
            throw new NotImplementedException();
        }

        public IClass Implement<TInterface>(params Func<IHappilClassBody<TInterface>, IMember>[] members)
        {
            throw new NotImplementedException();
        }
    }
}
