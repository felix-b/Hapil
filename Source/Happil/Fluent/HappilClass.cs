using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
    public class HappilClass: IClass
    {
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
            throw new NotImplementedException();
        }
    }
}
