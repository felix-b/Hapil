using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using NUnit.Framework;

namespace Hapil.UnitTests.Members
{
    [TestFixture]
    public class TypeMemberCacheTests
    {
        [Test]
        public void CanHideImplementedMethodsOfInterface()
        {
            //-- Arrange

            var members = TypeMemberCache.Of<AncestorRepository.DataRepoBase>();

            //-- Act

            var implementedInterfaceMethod = members.ImplementableMethods.Where(m => m.Name == "GetTypesInRepo");

            //-- Assert

            Assert.That(implementedInterfaceMethod.Count(), Is.EqualTo(1));
            Assert.That(implementedInterfaceMethod.Single().DeclaringType, Is.EqualTo(typeof(AncestorRepository.DataRepoBase)));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanHideImplementedPropertiesOfInterface()
        {
            //-- Arrange

            var members = TypeMemberCache.Of<AncestorRepository.DataRepoBase>();

            //-- Act

            var implementedInterfaceProperty = members.ImplementableProperties.Where(m => m.Name == "IsAutoCommit");

            //-- Assert

            Assert.That(implementedInterfaceProperty.Count(), Is.EqualTo(1));
            Assert.That(implementedInterfaceProperty.Single().DeclaringType, Is.EqualTo(typeof(AncestorRepository.DataRepoBase)));
        }
    }
}
