using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Writers
{
	[TestFixture]
	public class MethodWriterTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void ImplementInterfaceMethods_AllEmpty()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementBase<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementEmpty();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			obj.One();
			var result3 = obj.Three();
			var result5 = obj.Five(123);

			//-- Assert

			Assert.That(result3, Is.EqualTo(0));
			Assert.That(result5, Is.Null);
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
	    public void CallBaseMember_VoidMethod()
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
        public void CallBaseMember_VoidMethodWithArguments()
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
        public void CallBaseMember_Function()
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
        public void CallBaseMember_FunctionWithArguments()
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
        public void CallBaseMember_FunctionWithRefOutArguments()
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
        public void CallBaseMember_PropertyGetter()
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
        public void CallBaseMember_PropertySetter()
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
        public void CallBaseMember_IndexerPropertyGetter()
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
        public void CallBaseMember_IndexerPropertySetter()
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
        public void CallBaseMethod_TemplateImplementation()
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
        public void CallBaseProperty_TemplateImplementation()
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
    }
}
