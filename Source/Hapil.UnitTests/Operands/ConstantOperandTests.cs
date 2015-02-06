using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Operands
{
    [TestFixture]
    public class ConstantOperandTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanEmitTypeConstant()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveType>()
                .Property(x => x.TheType).Implement(p => p.Get(m => m.Return(m.Const(typeof(FileStream)))));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveType>().UsingDefaultConstructor();
            var returnedType = obj.TheType;

            //-- Assert

            Assert.That(returnedType, Is.SameAs(typeof(FileStream)));
        }
    }
}
