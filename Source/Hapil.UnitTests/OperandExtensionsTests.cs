using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Operands;
using Hapil.Testing.NUnit;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class OperandExtensionsTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void TestIsNull()
        {
            //-- arrange

            using ( TT.CreateScope<TT.TValue>(typeof(List<string>)) )
            {
                Field<string> field1;
                Field<TT.TValue> field2;

                DeriveClassFrom<object>()
                    .PrimaryConstructor<string, TT.TValue>("F1", out field1, "F2", out field2)
                    .NewVirtualFunction<bool>("FirstOn").Implement(w => w.Return(field1.IsNotNull()))
                    .NewVirtualFunction<bool>("FirstOff").Implement(w => w.Return(field1.IsNull()))
                    .NewVirtualFunction<bool>("SecondOn").Implement(w => w.Return(field2.IsNotNull()))
                    .NewVirtualFunction<bool>("SecondOff").Implement(w => w.Return(field2.IsNull()));
            }

            //-- act

            dynamic obj1 = CreateClassInstanceAs<object>().UsingConstructor<string, List<string>>("ABC", null);
            dynamic obj2 = CreateClassInstanceAs<object>().UsingConstructor<string, List<string>>(null, new List<string> { "ABC" });

            bool[] result1 = { (bool)obj1.FirstOn(), (bool)obj1.FirstOff(), (bool)obj1.SecondOn(), (bool)obj1.SecondOff() };
            bool[] result2 = { (bool)obj2.FirstOn(), (bool)obj2.FirstOff(), (bool)obj2.SecondOn(), (bool)obj2.SecondOff() };

            //-- assert

            Assert.That(result1, Is.EqualTo(new bool[] { true, false, false, true }));
            Assert.That(result2, Is.EqualTo(new bool[] { false, true, true, false }));
        }
    }
}
