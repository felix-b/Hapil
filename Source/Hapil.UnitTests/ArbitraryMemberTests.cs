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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanAddNewPropertiesByTemaplte()
        {
            //-- Arrange

            var propertiesToAdd = new Dictionary<string, Type> {
                { "MyInt", typeof(int) },
                { "MyString", typeof(string) }
            };

            var classWriter = DeriveClassFrom<AncestorRepository.LoggingBase>();
            classWriter.DefaultConstructor();

            //-- Act

            propertiesToAdd.ForEach(kvp => {
                using ( TT.CreateScope<TT.TProperty>(actualType: kvp.Value) )
                {
                    classWriter.NewVirtualWritableProperty<TT.TProperty>(propertyName: kvp.Key).Implement(
                        getter: p => p.Get(m => {
                            m.This<AncestorRepository.LoggingBase>().Void<string>(_ => _.AddLog, m.Const(p.OwnerProperty.Name + ".Get()"));
                            m.Return(p.BackingField);
                        }),
                        setter: p => p.Set((m, value) => {
                            m.This<AncestorRepository.LoggingBase>().Void<string>(_ => _.AddLog,
                                m.Const(p.OwnerProperty.Name + ".Set(value=") +
                                value.FuncToString() +
                                m.Const(")"));
                            p.BackingField.Assign(value);
                        })
                    );
                }
            });
            
            dynamic obj = CreateClassInstanceAs<AncestorRepository.LoggingBase>().UsingDefaultConstructor();

            //-- Assert

            obj.MyInt = 123;
            obj.MyString = "ABC";

            var myIntValue = obj.MyInt;
            var myStringValue = obj.MyString;

            Assert.That(obj.TakeLog(), Is.EqualTo(new[] {
                "MyInt.Set(value=123)",
                "MyString.Set(value=ABC)", 
                "MyInt.Get()",
                "MyString.Get()"
            }));

            Assert.That(myIntValue, Is.EqualTo(123));
            Assert.That(myStringValue, Is.EqualTo("ABC"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanAddNewCollectionPropertiesByItemTemaplate()
        {
            //-- Arrange

            var propertiesToAdd = new Dictionary<string, Type> {
                { "MyIntList", typeof(int) },
                { "MyStringList", typeof(string) }
            };

            var classWriter = DeriveClassFrom<AncestorRepository.LoggingBase>();
            classWriter.DefaultConstructor();

            //-- Act

            propertiesToAdd.ForEach(kvp => {
                using ( TT.CreateScope<TT.TItem>(actualType: kvp.Value) )
                {
                    classWriter.NewVirtualWritableProperty<List<TT.TItem>>(propertyName: kvp.Key).Implement(
                        getter: p => p.Get(m => {
                            m.This<AncestorRepository.LoggingBase>().Void<string>(_ => _.AddLog, m.Const(p.OwnerProperty.Name + ".Get()"));
                            m.Return(p.BackingField);
                        }),
                        setter: p => p.Set((m, value) => {
                            m.This<AncestorRepository.LoggingBase>().Void<string>(_ => _.AddLog,
                                m.Const(p.OwnerProperty.Name + ".Set(value=") +
                                m.Iif(value != null, m.Const("[") + value.Count().FuncToString() + m.Const(" items]"), m.Const("null")) +
                                m.Const(")"));
                            p.BackingField.Assign(value);
                        })
                    );
                }
            });

            dynamic obj = CreateClassInstanceAs<AncestorRepository.LoggingBase>().UsingDefaultConstructor();

            //-- Assert

            obj.MyIntList = null;
            obj.MyIntList = new List<int> { 1, 3, 5 };
            obj.MyStringList = new List<string> { "A", "B" };

            var myIntList = obj.MyIntList;
            var myStringList = obj.MyStringList;

            Assert.That(obj.TakeLog(), Is.EqualTo(new[] {
                "MyIntList.Set(value=null)",
                "MyIntList.Set(value=[3 items])",
                "MyStringList.Set(value=[2 items])", 
                "MyIntList.Get()",
                "MyStringList.Get()"
            }));

            Assert.That(myIntList, Is.EqualTo(new[] { 1, 3, 5 }));
            Assert.That(myStringList, Is.EqualTo(new[] { "A", "B" }));
        }
    }
}

#if false

            //-- Arrange

            var propertiesToAdd = new Dictionary<string, Type> {
                { "MyInt", typeof(int), },
                { "MyString", typeof(string) }
            };

            var classWriter = DeriveClassFrom<AncestorRepository.LoggingBase>();
            classWriter.DefaultConstructor();

            var propertyFields = new Dictionary<string, Field<TT.TProperty>>();

            //-- Act

            propertiesToAdd.ForEach(kvp => {
                using ( TT.CreateScope<TT.TItem, TT.TProperty>(kvp.Value, typeof(List<>).MakeGenericType(kvp.Value)) )
                {
                    using ( TT.CreateScope<TT.TItem>(kvp.Value) )
                    {
                        classWriter.NewVirtualWritableProperty<TT.TProperty>()
                    }
                }
            });


#endif