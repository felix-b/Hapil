using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Assumptions
{
    [TestFixture]
    public class EmittedTypeTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanUseGeneratedObjectThroughCSharpDynamic()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IFewMethods>()
                .AllMethods(where: m => m.IsVoid()).Implement(m => { })
                .Method<int>(x => x.Three).Implement(m => m.Return(333))
                .Method<string, int>(x => x.Four).Implement((m, s) => m.Return(Static.Func(Int32.Parse, s)))
                .Method<int, string>(x => x.Five).Implement((m, n) => m.Return(n.FuncToString()));

            var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

            //-- Act

            dynamic asDynamic = obj;
            var four = asDynamic.Four("123");

            //-- Assert

            Assert.That(four, Is.EqualTo(123));
        }
    }
}
