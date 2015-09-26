using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using NUnit.Framework;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class TypeMemberCacheTests
    {
        [Test]
        public void InheritedMembersAreOnyListedOnce()
        {
            //-- arrange & act

            var members = TypeMemberCache.Of<AbstractClassTwo>();

            //-- assert

            Assert.That(members.ImplementableMethods.Count(m => m.Name == "MethodOne"), Is.EqualTo(1));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IInterfaceOne
        {
            void MethodOne();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IInterfaceTwo : IInterfaceOne
        {
            void MethodTwo();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public abstract class AbstractClassOne : IInterfaceOne
        {
            #region Implementation of IInterfaceOne

            public abstract void MethodOne();

            #endregion
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public abstract class AbstractClassTwo : AbstractClassOne, IInterfaceTwo
        {
            #region Implementation of IInterfaceTwo

            public abstract void MethodTwo();

            #endregion
        }
    }
}
