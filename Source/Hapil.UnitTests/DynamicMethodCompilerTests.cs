using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class DynamicMethodCompilerTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanCreateStaticVoidMethodNotBelongingToType()
        {
            //-- arrange

            var compiler = new DynamicMethodCompiler(base.Module);

            Action<TestTarget, int> intPropertySetter = compiler.CompileStaticVoidMethod<TestTarget, int>(
                "IntValueSetter",
                (w, target, value) => {
                    target.Prop<int>(s_IntValuePropertyInfo).Assign(value);
                });

            Action<TestTarget, string> stringPropertySetter = compiler.CompileStaticVoidMethod<TestTarget, string>(
                "StringValueSetter",
                (w, target, value) => {
                    target.Prop<string>(s_StringValuePropertyInfo).Assign(value);
                });

            var targetInstance = new TestTarget();

            //-- act

            intPropertySetter(targetInstance, 123);
            stringPropertySetter(targetInstance, "ABC");

            //-- assert

            Assert.That(targetInstance.IntValue, Is.EqualTo(123));
            Assert.That(targetInstance.StringValue, Is.EqualTo("ABC"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCreateStaticFunctionNotBelongingToType()
        {
            //-- arrange

            var compiler = new DynamicMethodCompiler(base.Module);

            Func<TestTarget, int> intPropertyGetter = compiler.CompileStaticFunction<TestTarget, int>(
                "IntValueGetter",
                (w, target) => {
                    w.Return(target.Prop<int>(s_IntValuePropertyInfo));
                });

            Func<TestTarget, string> stringPropertyGetter = compiler.CompileStaticFunction<TestTarget, string>(
                "StringValueGetter",
                (w, target) => {
                    w.Return(target.Prop<string>(s_StringValuePropertyInfo));
                });

            var targetInstance = new TestTarget() {
                IntValue = 123,
                StringValue = "ABC"
            };

            //-- act

            var intValue = intPropertyGetter(targetInstance);
            var stringValue = stringPropertyGetter(targetInstance);

            //-- assert

            Assert.That(intValue, Is.EqualTo(123));
            Assert.That(stringValue, Is.EqualTo("ABC"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static PropertyInfo s_IntValuePropertyInfo = typeof(TestTarget).GetProperty("IntValue");
        private static PropertyInfo s_StringValuePropertyInfo = typeof(TestTarget).GetProperty("StringValue");

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class TestTarget
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }
    }
}
