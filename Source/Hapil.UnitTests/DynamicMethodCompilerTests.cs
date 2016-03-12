using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Testing.NUnit;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

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

        [Test]
        public void CanCompileStaticFunctionForCustomDelegate()
        {
            //-- arrange

            var compiler = new DynamicMethodCompiler(base.Module);

            Mutation mutation = compiler.ForDelegate<Mutation>().CompileStaticFunction<KeyValuePair<int, string>, int, string, int>(
                "IntStringKeyValuePairMutation",
                (w, kvp, newKey, newValue) => {
                    var kvpFields = typeof(KeyValuePair<int, string>).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var kvpKeyField = kvpFields.Single(f => f.Name == "key");
                    var kvpValueField = kvpFields.Single(f => f.Name == "value");
                    var oldKey = w.Local(initialValue: kvp.Field<int>(kvpKeyField));
                    kvp.Field<int>(kvpKeyField).Assign(newKey);
                    kvp.Field<string>(kvpValueField).Assign(newValue);
                    w.Return(oldKey);
                }
            );

            //-- act

            var keyValuePair = new KeyValuePair<int, string>(123, "ABC");
            var originalKey = mutation(ref keyValuePair, 456, "DEF");

            //-- assert

            Assert.That(keyValuePair.Key, Is.EqualTo(456));
            Assert.That(keyValuePair.Value, Is.EqualTo("DEF"));
            Assert.That(originalKey, Is.EqualTo(123));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCompileStaticFunctionForCustomGenericDelegateUsingTemplateTypes()
        {
            //-- arrange

            var compiler = new DynamicMethodCompiler(base.Module);
            Delegate mutationDelegate;

            using (TT.CreateScope<TT.TKey, TT.TValue>(typeof(int), typeof(string)))
            {
                mutationDelegate = compiler
                    .ForTemplatedDelegate<Mutation<TT.TKey, TT.TValue>>()
                    .CompileStaticFunction<KeyValuePair<TT.TKey, TT.TValue>, TT.TKey, TT.TValue, TT.TKey>(
                        "IntStringKeyValuePairMutation",
                        (w, kvp, newKey, newValue) => {
                            var kvpFields = TT.Resolve<KeyValuePair<TT.TKey, TT.TValue>>().GetFields(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            var kvpKeyField = kvpFields.Single(f => f.Name == "key");
                            var kvpValueField = kvpFields.Single(f => f.Name == "value");
                            var oldKey = w.Local(initialValue: kvp.Field<TT.TKey>(kvpKeyField));
                            kvp.Field<TT.TKey>(kvpKeyField).Assign(newKey);
                            kvp.Field<TT.TValue>(kvpValueField).Assign(newValue);
                            w.Return(oldKey);
                        });
            }

            //-- act

            var intStringKvpMutation = (Mutation<int, string>)mutationDelegate;
            var keyValuePair = new KeyValuePair<int, string>(123, "ABC");
            var originalKey = intStringKvpMutation(ref keyValuePair, 456, "DEF");

            //-- assert

            Assert.That(keyValuePair.Key, Is.EqualTo(456));
            Assert.That(keyValuePair.Value, Is.EqualTo("DEF"));
            Assert.That(originalKey, Is.EqualTo(123));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static PropertyInfo s_IntValuePropertyInfo = typeof(TestTarget).GetProperty("IntValue");
        private static PropertyInfo s_StringValuePropertyInfo = typeof(TestTarget).GetProperty("StringValue");

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public delegate int Mutation(ref KeyValuePair<int, string> entry, int newKey, string newValue);
        public delegate K Mutation<K, V>(ref KeyValuePair<K, V> entry, K newKey, V newValue);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class TestTarget
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }
    }
}
