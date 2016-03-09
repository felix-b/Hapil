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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void InheritedMembersAreOnyListedOnce()
        {
            //-- arrange & act

            var members = TypeMemberCache.Of<AbstractClassTwo>();

            //-- assert

            Assert.That(members.ImplementableMethods.Count(m => m.Name == "MethodOne"), Is.EqualTo(1));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanHandleMembersThatHaveCollectionTypes()
        {
            //-- arrange & act

            var members = TypeMemberCache.Of<AClassWithCollections>();

            //-- assert

            Assert.That(members.ImplementableProperties.Select(p => p.Name), Is.EquivalentTo(new[] { "IntArray", "IntList", "IntStringDictionary" }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanHandleCollectionTypes()
        {
            //-- act & assert

            TypeMemberCache.Of<int[]>();
            TypeMemberCache.Of<List<int>>();
            TypeMemberCache.Of<Dictionary<int, string>>();
            TypeMemberCache.Of<ICollection<int>>();
            TypeMemberCache.Of<IList<int>>();
            TypeMemberCache.Of<IDictionary<int, string>>();
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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class AClassWithCollections
        {
            public virtual int[] IntArray { get; set; }
            public virtual List<int> IntList { get; set; }
            public virtual Dictionary<int, string> IntStringDictionary { get; set; }
        }
    }
}
