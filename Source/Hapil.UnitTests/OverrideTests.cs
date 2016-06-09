using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil.Writers;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class OverrideTests : NUnitEmittedTypesTestBase
	{
        [Test]
	    public void OverrideBaseMember_VoidMethod()
	    {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method(x => x.VoidNoArgs).Implement(w => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:VoidNoArgs"));
                    w.Base();
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            obj.VoidNoArgs();

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] { "OVERRIDE:VoidNoArgs", "BASE:VoidNoArgs()" }));
        }
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_VoidMethodWithArguments()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string>(x => x.VoidWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:VoidWithArgs"));
                    w.Base(num, str);
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            obj.VoidWithArgs(num: 123, str: "ABC");

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:VoidWithArgs", 
                "BASE:VoidWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_Function()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<string>(x => x.FunctionNoArgs).Implement(w => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionNoArgs"));
                    w.Return(w.Base());
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionNoArgs();

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("ABC"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionNoArgs", 
                "BASE:FunctionNoArgs()"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_FunctionWithArguments()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.FunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs", 
                "BASE:FunctionWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_FunctionWithRefOutArguments()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => (num, str) => x.FunctionWithRefOutArgs(ref num, out str)).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithRefOutArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            int numArgument = 123;
            string strArgument;
            var returnValue = obj.FunctionWithRefOutArgs(ref numArgument, out strArgument);

            //-- Assert

            Assert.That(numArgument, Is.EqualTo(246));
            Assert.That(strArgument, Is.EqualTo("ABCDEF"));
            Assert.That(returnValue, Is.EqualTo("GHI"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithRefOutArgs", 
                "BASE:FunctionWithRefOutArgs(num=123)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_PropertyGetter()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Property(x => x.IntProperty).Implement(
                    p => p.Get(gw => {
                        gw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:IntProperty.GET"));
                        gw.Return(gw.Base());
                    }),
                    p => p.Set((sw, value) => {
                        sw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:IntProperty.SET"));
                        sw.Base(value);
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var propertyValue = obj.IntProperty;

            //-- Assert

            Assert.That(propertyValue, Is.EqualTo(123));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:IntProperty.GET", 
                "BASE:IntProperty.GET"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_PropertySetter()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Property(x => x.IntProperty).Implement(
                    p => p.Get(gw => {
                        gw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:IntProperty.GET"));
                        gw.Return(gw.Base());
                    }),
                    p => p.Set((sw, value) => {
                        sw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:IntProperty.SET"));
                        sw.Base(value);
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            obj.IntProperty = 456;

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:IntProperty.SET", 
                "BASE:IntProperty.SET(value=456)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_IndexerPropertyGetter()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .This<int, string>().Implement(
                    p => p.Get((gw, key) => {
                        gw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:this.GET"));
                        gw.Return(gw.Base(key));
                    }),
                    p => p.Set((sw, key, value) => {
                        sw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:this.SET"));
                        sw.Base(key, value);
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var indexerValue = obj[987];

            //-- Assert

            Assert.That(indexerValue, Is.EqualTo("XYZ"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:this.GET", 
                "BASE:this.GET(key=987)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_IndexerPropertySetter()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .This<int, string>().Implement(
                    p => p.Get((gw, key) => {
                        gw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:this.GET"));
                        gw.Return(gw.Base(key));
                    }),
                    p => p.Set((sw, key, value) => {
                        sw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:this.SET"));
                        sw.Base(key, value);
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            obj[987] = "ZZZ";

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:this.SET", 
                "BASE:this.SET(key=987,value=ZZZ)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void OverrideBaseMethod_TemplateImplementation()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .AllMethods().Implement(w => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:" + w.OwnerMethod.Name)); 
                    w.ProceedToBase();
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            obj.VoidNoArgs();
            obj.VoidWithArgs(num: 123, str: "UUU");
            var returnValue1 = obj.FunctionNoArgs();
            var returnValue2 = obj.FunctionWithArgs(num: 456, str: "VVV");

            var numArgument = 111;
            string strArgument;
            var returnValue3 = obj.FunctionWithRefOutArgs(ref numArgument, out strArgument);

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:VoidNoArgs", "BASE:VoidNoArgs()",
                "OVERRIDE:VoidWithArgs", "BASE:VoidWithArgs(num=123,str=UUU)",
                "OVERRIDE:FunctionNoArgs", "BASE:FunctionNoArgs()",
                "OVERRIDE:FunctionWithArgs", "BASE:FunctionWithArgs(num=456,str=VVV)",
                "OVERRIDE:FunctionWithRefOutArgs", "BASE:FunctionWithRefOutArgs(num=111)"
            }));

            Assert.That(returnValue1, Is.EqualTo("ABC"));
            Assert.That(returnValue2, Is.EqualTo("DEF"));
            Assert.That(returnValue3, Is.EqualTo("GHI"));
            Assert.That(numArgument, Is.EqualTo(222));
            Assert.That(strArgument, Is.EqualTo("ABCDEF"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseProperty_TemplateImplementation()
        {
            DeriveClassFrom<AncestorRepository.BaseFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .ReadWriteProperties().Implement(
                    p => p.Get(gw => {
                        gw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:" + p.OwnerProperty.Name + ".GET")); 
                        gw.ProceedToBase();
                    }),
                    p => p.Set((sw, value) => {
                        sw.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:" + p.OwnerProperty.Name + ".SET")); 
                        sw.ProceedToBase();
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var propertyValue1 = obj.IntProperty;
            obj.IntProperty = 456;

            var propertyValue2 = obj[987];
            obj[987] = "ZZZ";

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:IntProperty.GET", "BASE:IntProperty.GET",
                "OVERRIDE:IntProperty.SET", "BASE:IntProperty.SET(value=456)",
                "OVERRIDE:Item.GET", "BASE:this.GET(key=987)",
                "OVERRIDE:Item.SET", "BASE:this.SET(key=987,value=ZZZ)"
            }));

            Assert.That(propertyValue1, Is.EqualTo(123));
            Assert.That(propertyValue2, Is.EqualTo("XYZ"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_OverriddenVirtual_ThreeLevelInheritance()
        {
            DeriveClassFrom<AncestorRepository.ConcreteFour>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.FunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFour>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFour>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("CONCRETE-DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs", 
                "CONCRETE:FunctionWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_OverriddenAbstract_TwoLevelInheritance()
        {
            DeriveClassFrom<AncestorRepository.BaseFive>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.FunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFive>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseFive>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs", 
                "BASE:FunctionWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_OverriddenAbstract_ThreeLevelInheritance()
        {
            DeriveClassFrom<AncestorRepository.ConcreteFive>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.FunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFive>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.ConcreteFive>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("CONCRETE-DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs", 
                "CONCRETE:FunctionWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_OverriddenVirtual_IndirectBase()
        {
            DeriveClassFrom<AncestorRepository.ConcreteFive>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.AnotherFunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseFive>().Prop(x => x.Log).Add(w.Const("OVERRIDE:AnotherFunctionWithArgs"));
                    w.Return(w.Base(num, str));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.ConcreteFive>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.AnotherFunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("ANOTHER-DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:AnotherFunctionWithArgs", 
                "BASE:AnotherFunctionWithArgs(num=123,str=ABC)"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OverrideBaseMember_OverriddenAbstract_IndirectBase()
        {
            DeriveClassFrom<AncestorRepository.BaseSix>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .Method<int, string, string>(x => x.FunctionWithArgs).Implement((w, num, str) => {
                    w.This<AncestorRepository.BaseSix>().Prop(x => x.Log).Add(w.Const("OVERRIDE:FunctionWithArgs"));
                    w.Return(w.Const("OVERRIDE-DEF"));
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseSix>().UsingDefaultConstructor();

            //-- Act

            var returnValue = obj.FunctionWithArgs(123, "ABC");

            //-- Assert

            Assert.That(returnValue, Is.EqualTo("OVERRIDE-DEF"));

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs", 
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void Methods_TemplateImplementation_MixedLevelsAbstractsAndVirtuals()
        {
            DeriveClassFrom<AncestorRepository.BaseSix>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .AllMethods().Implement(w => {
                    w.This<AncestorRepository.BaseSix>().Prop(x => x.Log).Add(w.Const("OVERRIDE:" + w.OwnerMethod.Name));
                    w.ProceedToBaseIfAny();
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseSix>().UsingDefaultConstructor();

            //-- Act

            var returnValue1 = obj.FunctionWithArgs(num: 123, str: "ABC");
            var returnValue2 = obj.SecondFunctionWithArgs(num: 456, str: "DEF");
            var returnValue3 = obj.ThirdFunctionWithArgs(num: 789, str: "GHI");
            var returnValue4 = obj.FourthFunctionWithArgs(num: 987, str: "JKL");

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs",
                "OVERRIDE:SecondFunctionWithArgs", "BASEBASE:SecondFunctionWithArgs(num=456,str=DEF)",
                "OVERRIDE:ThirdFunctionWithArgs", "BASE:ThirdFunctionWithArgs(num=789,str=GHI)",
                "OVERRIDE:FourthFunctionWithArgs", "BASE:FourthFunctionWithArgs(num=987,str=JKL)",
            }));

            Assert.That(returnValue1, Is.Null);
            Assert.That(returnValue2, Is.EqualTo("BASEBASE-SECOND-RET"));
            Assert.That(returnValue3, Is.EqualTo("BASE-THIRD-RET"));
            Assert.That(returnValue4, Is.EqualTo("BASE-FOURTH-RET"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void Methods_TemplateImplementation_MixedLevelsAbstractsVirtualsAndInterfaces()
        {
            DeriveClassFrom<AncestorRepository.BaseSix>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .AllMethods().Implement(w => {
                    w.This<AncestorRepository.BaseSix>().Prop(x => x.Log).Add(w.Const("OVERRIDE:" + w.OwnerMethod.Name));
                    w.ProceedToBaseIfAny();
                })
                .ImplementInterfaceExplicitly<AncestorRepository.IBaseSix>()
                .AllMethods().Implement(w => {
                    w.This<AncestorRepository.BaseSix>().Prop(x => x.Log).Add(w.Const("INTERFACE:" + GetShortMethodName(w)));
                    w.ProceedToBaseIfAny();
                });

            var obj = CreateClassInstanceAs<AncestorRepository.BaseSix>().UsingDefaultConstructor();

            //-- Act

            var returnValue1 = obj.FunctionWithArgs(num: 123, str: "ABC");
            var returnValue2 = obj.SecondFunctionWithArgs(num: 456, str: "DEF");
            var returnValue3 = obj.ThirdFunctionWithArgs(num: 789, str: "GHI");
            var returnValue4 = obj.FourthFunctionWithArgs(num: 987, str: "JKL");
            var interfaceReturnValue1 = ((AncestorRepository.IBaseSix)obj).FunctionWithArgs(num: 123, str: "ABC");

            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:FunctionWithArgs",
                "OVERRIDE:SecondFunctionWithArgs", "BASEBASE:SecondFunctionWithArgs(num=456,str=DEF)",
                "OVERRIDE:ThirdFunctionWithArgs", "BASE:ThirdFunctionWithArgs(num=789,str=GHI)",
                "OVERRIDE:FourthFunctionWithArgs", "BASE:FourthFunctionWithArgs(num=987,str=JKL)",
                "INTERFACE:IBaseSix.FunctionWithArgs",
            }));

            Assert.That(returnValue1, Is.Null);
            Assert.That(returnValue2, Is.EqualTo("BASEBASE-SECOND-RET"));
            Assert.That(returnValue3, Is.EqualTo("BASE-THIRD-RET"));
            Assert.That(returnValue4, Is.EqualTo("BASE-FOURTH-RET"));
            Assert.That(interfaceReturnValue1, Is.Null);
        }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Ignore("There are still troubles here")]
        public void Properties_TemplateImplementation_MixedLevelsAbstractsVirtualsAndInterfaces()
        {
            DeriveClassFrom<AncestorRepository.BaseSeven>()
                .DefaultConstructor(cw => cw.Base(cw.New<List<string>>()))
                .AllProperties().Implement(
                    p => p.Get(gw => {
                        gw.This<AncestorRepository.BaseSeven>().Prop(x => x.Log).Add(gw.Const("OVERRIDE:" + gw.OwnerMethod.Name));
                        gw.ProceedToBaseIfAny();
                    }),
                    p => p.Set((sw, value) => {
                        sw.This<AncestorRepository.BaseSeven>().Prop(x => x.Log).Add(sw.Const("OVERRIDE:" + sw.OwnerMethod.Name));
                        sw.ProceedToBaseIfAny();
                    })
                )
                .ImplementInterface<AncestorRepository.IBaseSeven>()
                .ImplementInterfaceExplicitly<AncestorRepository.IBaseSevenEx>()
                .AllProperties().Implement(
                    p => p.Get(gw => {
                        gw.This<AncestorRepository.BaseSeven>().Prop(x => x.Log).Add(gw.Const("EXPLICIT-IMPL:" + gw.OwnerMethod.Name));
                        gw.ProceedToBaseIfAny();
                    }),
                    p => p.Set((sw, value) => {
                        sw.This<AncestorRepository.BaseSeven>().Prop(x => x.Log).Add(sw.Const("EXPLICIT-IMPL:" + sw.OwnerMethod.Name));
                        sw.ProceedToBaseIfAny();
                    })
                );

            var obj = CreateClassInstanceAs<AncestorRepository.BaseSeven>().UsingDefaultConstructor();

            //-- Act

            var property1 = obj.FirstProperty;
            var property2 = obj.SecondProperty;
            //var property3 = obj.ThirdProperty;
            var property4 = obj.FourthProperty;
            var property3implicit = ((AncestorRepository.IBaseSeven)obj).ThirdProperty;
            var property3explicit = ((AncestorRepository.IBaseSevenEx)obj).ThirdProperty;
            var property4explicit = ((AncestorRepository.IBaseSevenEx)obj).FourthProperty;

            obj.FirstProperty = "111";
            obj.SecondProperty = "222";
            //obj.ThirdProperty = "333";
            obj.FourthProperty = "444";
            ((AncestorRepository.IBaseSevenEx)obj).ThirdProperty = "X333";
            ((AncestorRepository.IBaseSevenEx)obj).FourthProperty = "X444";
            
            //-- Assert

            Assert.That(obj.Log, Is.EqualTo(new[] {
                "OVERRIDE:get_FirstProperty", 
                "OVERRIDE:get_SecondProperty", "BASEBASE:SecondProperty.GET",
                //"OVERRIDE:get_ThirdProperty", "BASE:ThirdProperty.GET",
                "OVERRIDE:get_FourthProperty", "BASE:FourthProperty.GET",
                "EXPLICIT-IMPL:IBaseSevenEx.get_ThirdProperty",
                "EXPLICIT-IMPL:IBaseSevenEx.get_FourthProperty",

                "OVERRIDE:set_FirstProperty", 
                "OVERRIDE:set_SecondProperty", "BASEBASE:SecondProperty.SET(value=222)",
                //"OVERRIDE:set_ThirdProperty", "BASE:ThirdProperty.SET(value=333)",
                "OVERRIDE:set_FourthProperty", "BASE:FourthProperty.SET(value=444)",
                "EXPLICIT-IMPL:IBaseSevenEx.set_ThirdProperty",
                "EXPLICIT-IMPL:IBaseSevenEx.set_FourthProperty",
            }));

            Assert.That(property1, Is.Null);
            Assert.That(property2, Is.EqualTo("BASEBASE-SECOND-PROPERTY"));
            //Assert.That(property3, Is.EqualTo("BASE-THIRD-PROPERTY"));
            Assert.That(property4, Is.EqualTo("BASE-FOURTH-PROPERTY"));
            Assert.That(property3implicit, Is.EqualTo("BASE-THIRD-PROPERTY"));
            Assert.That(property3explicit, Is.Null);
            Assert.That(property4explicit, Is.Null);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        private static string GetShortMethodName(TemplateMethodWriter w)
        {
            var methodNameParts = w.OwnerMethod.Name.Split('.', '+').ToArray();
            var shortMethodName = string.Join(".", methodNameParts.Skip(methodNameParts.Length - 2));
            return shortMethodName;
        }
    }
}
