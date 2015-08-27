using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Writers;

namespace Hapil.Toolbox
{
    public class DefaultConstructorConvention : ImplementationConvention
    {
        public DefaultConstructorConvention()
            : base(Will.ImplementBaseClass)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Overrides of ImplementationConvention

        protected override void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
        {
            writer.DefaultConstructor();
        }

        #endregion
    }
}
