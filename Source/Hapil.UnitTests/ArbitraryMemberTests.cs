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
    public class ArbitraryMemberTests : NUnitEmittedTypesTestBase
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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanAddArbitraryMethods()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.LoggingBase>()
                .DefaultConstructor()
                .NewVirtualVoidMethod("MyMethodOne").Implement(w => 
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, w.Const("MyMethodOne()"))
                )
                .NewVirtualVoidMethod<string>("MyMethodTwo", "myS1").Implement((w, s) => 
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, w.Const("MyMethodTwo(") + s + w.Const(")"))
                )
                .NewVirtualVoidMethod<int, string>("MyMethodThree", "num1", "str1").Implement((w, n, s) => 
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, 
                        w.Const("MyMethodThree(") + n.FuncToString() + w.Const(",") + s + w.Const(")"))
                )
                .NewVirtualFunction<int>("MyFuncOne").Implement(w => {
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, w.Const("MyFuncOne()"));
                    w.Return(123);
                })
                .NewVirtualFunction<string, int>("MyFuncTwo", "myS1").Implement((w, s) => {
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, w.Const("MyFuncTwo(") + s + w.Const(")"));
                    w.Return(456);
                })
                .NewVirtualFunction<int, string, int>("MyFuncThree", "num1", "str1").Implement((w, n, s) => {
                    w.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog,
                        w.Const("MyFuncThree(") + n.FuncToString() + w.Const(",") + s + w.Const(")"));
                    w.Return(789);
                });

            //-- Act

            dynamic obj = CreateClassInstanceAs<AncestorRepository.LoggingBase>().UsingDefaultConstructor();

            obj.MyMethodOne();
            obj.MyMethodTwo("ABC");
            obj.MyMethodThree(-1000, "DEF");

            var funcOneResult = obj.MyFuncOne();
            var funcTwoResult = obj.MyFuncTwo("ABC");
            var funcThreeResult = obj.MyFuncThree(-1000, "DEF");

            //-- Assert

            Assert.That(obj.TakeLog(), Is.EqualTo(new[] {
                "MyMethodOne()", "MyMethodTwo(ABC)", "MyMethodThree(-1000,DEF)",
                "MyFuncOne()", "MyFuncTwo(ABC)", "MyFuncThree(-1000,DEF)"
            }));

            Assert.That(funcOneResult, Is.EqualTo(123));
            Assert.That(funcTwoResult, Is.EqualTo(456));
            Assert.That(funcThreeResult, Is.EqualTo(789));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanAddArbitraryProperties()
        {
            //-- Arrange

            Field<string> myStringField;

            DeriveClassFrom<AncestorRepository.LoggingBase>()
                .DefaultConstructor()
                .Field<string>("m_MyString", out myStringField)
                .NewVirtualWritableProperty<int>("MyInt").ImplementAutomatic()
                .NewVirtualWritableProperty<string>("MyString").Implement(
                    p => p.Get(m => {
                        m.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, m.Const("MyString.Get()"));
                        m.Return(myStringField);
                    }),
                    p => p.Set((m, value) => {
                        m.This<AncestorRepository.LoggingBase>().Void(x => x.AddLog, m.Const("MyString.Set(") + value + m.Const(")"));
                        myStringField.Assign(value);
                    }));

            //-- Act

            dynamic obj = CreateClassInstanceAs<AncestorRepository.LoggingBase>().UsingDefaultConstructor();

            obj.MyInt = 123;
            obj.MyString = "ABC";

            var myIntValue = obj.MyInt;
            var myStringValue = obj.MyString;

            //-- Assert

            Assert.That(obj.TakeLog(), Is.EqualTo(new[] { "MyString.Set(ABC)", "MyString.Get()" }));

            Assert.That(myIntValue, Is.EqualTo(123));
            Assert.That(myStringValue, Is.EqualTo("ABC"));
        }
    }
}
