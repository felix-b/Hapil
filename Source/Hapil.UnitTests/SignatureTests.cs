using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Members;
using Hapil.Testing.NUnit;
using Hapil.Expressions;
using Hapil.Operands;
using Moq;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
	[TestFixture]
    public class SignatureTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void InterfaceMethods_OnyByOne()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(m => { })
				.Method<int>(intf => intf.Two).Implement((m, n) => { })
				.Method<int>(intf => intf.Three).Implement(m => m.Return(123))
				.Method<string, int>(intf => intf.Four).Implement((m, s) => m.Return(456))
				.Method<int, string>(intf => intf.Five).Implement((m, n) => m.Return("ABC"));

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			//-- Assert

			obj.One();
			obj.Two(0);
			
			Assert.That(obj.Three(), Is.EqualTo(123));
			Assert.That(obj.Four(null), Is.EqualTo(456));
			Assert.That(obj.Five(0), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_SelectAllVoidsAndNonVoids()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IMoreMethods>()
				.AllMethods(m => m.IsVoid()).Implement(m => { })
				.AllMethods(m => !m.IsVoid()).Implement(
					m => {
						m.Return(m.Default<TypeTemplate.TReturn>());
					});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();

			//-- Assert

			obj.One();
			obj.Three(0);
			obj.Seven(TimeSpan.Zero, null, 0);

			Assert.That(obj.Eleven(), Is.EqualTo(0));
			Assert.That(obj.Twelwe(), Is.Null);
			Assert.That(obj.Fifteen(TimeSpan.Zero, null), Is.EqualTo(0));
			Assert.That(obj.Sixteen(0, TimeSpan.Zero), Is.Null);
			Assert.That(obj.Eighteen(null, 0, TimeSpan.Zero), Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_SelectMultipleTimes_ImplementedOnlyOnce()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IMoreMethods>()
				.Method<string, int, TimeSpan>(intf => intf.Eight).Implement((m, s, n, t) => { })
				.AllMethods(m => m.IsVoid()).Implement(m => { })
				.AllMethods().Implement(   // this selects all methods, but only non-void methods will be implemented, 
					m => {                 // because void methods were already implemented earlier.
						m.Return(m.Default<TypeTemplate.TReturn>());
					});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();

			//-- Assert

			obj.One();
			Assert.That(obj.Eighteen(null, 0, TimeSpan.Zero), Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_SelectBySignature()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IMoreMethods>()
				.VoidMethods().Implement(m => { })
				.VoidMethods<int>().Implement((m, n) => { })
				.VoidMethods<string>().Implement((m, s) => { })
				.VoidMethods<int, string>().Implement((m, n, s) => { })
				.VoidMethods<string, int>().Implement((m, s, n) => { })
				.VoidMethods<TimeSpan, string, int>().Implement((m, t, s, n) => { })
				.VoidMethods<string, int, TimeSpan>().Implement((m, s, n, t) => { })
				.VoidMethods<int, int>().Implement((m, x, y) => {
					Assert.Fail("No such methods! this is a bug.");
				})
				.NonVoidMethods<int>().Implement(m => m.Return(123))
				.NonVoidMethods<string>().Implement(m => m.Return("ABC"))
				.NonVoidMethods<int, string>().Implement((m, n) => m.Return("DEF"))
				.NonVoidMethods<string, int>().Implement((m, s) => m.Return(456))
				.NonVoidMethods<TimeSpan, string, int>().Implement((m, t, s) => m.Return(789))
				.NonVoidMethods<int, TimeSpan, string>().Implement((m, n, t) => m.Return("GHI"))
				.NonVoidMethods<TimeSpan, string, int, object>().Implement((m, t, s, n) => m.Return(m.Const<object>(null)))
				.NonVoidMethods<string, int, TimeSpan, IEnumerable<int>>().Implement((m, s, n, t) => m.Return(m.Const<IEnumerable<int>>(null)));

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();

			//-- Assert

			obj.One();
			obj.Three(0);
			obj.Seven(TimeSpan.Zero, null, 0);

			Assert.That(obj.Eleven(), Is.EqualTo(123));
			Assert.That(obj.Twelwe(), Is.EqualTo("ABC"));
			Assert.That(obj.Fifteen(TimeSpan.Zero, null), Is.EqualTo(789));
			Assert.That(obj.Sixteen(0, TimeSpan.Zero), Is.EqualTo("GHI"));
			Assert.That(obj.Eighteen(null, 0, TimeSpan.Zero), Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_OnyByOne()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.Property(intf => intf.AnInt).Implement(
					p => p.Get(m => m.Return(123)),
					p => p.Set((m, value) => { }))
				.Property(intf => intf.AString).Implement(
					p => p.Get(m => m.Return("ABC")),
					p => p.Set((m, value) => { }))
				.Property(intf => intf.AnObject).Implement(
					p => p.Get(m => m.Return(m.Const<object>(null))),
					p => p.Set((m, value) => { }));

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(obj.AnObject, Is.Null);

			obj.AnInt = 0;
			obj.AString = null;
			obj.AnObject = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_SelectReadOnlyAndReadWrite()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IReadOnlyAndReadWriteProperties>()
				.ReadOnlyProperties().Implement(
					p => p.Get(m => m.Return(m.Default<TypeTemplate.TProperty>()))
				)
				.ReadWriteProperties().Implement(
					p => p.Get(m => m.Return(m.Default<TypeTemplate.TProperty>())),
					p => p.Set((m, value) => { })
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IReadOnlyAndReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(0));
			Assert.That(obj.AString, Is.Null);
			Assert.That(obj.AnObject, Is.Null);

			Assert.That(obj.AnotherInt, Is.EqualTo(0));
			Assert.That(obj.AnotherString, Is.Null);
			Assert.That(obj.AnotherObject, Is.Null);

			obj.AnotherInt = 0;
			obj.AnotherString = null;
			obj.AnotherObject = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_SelectMultipleTimes_ImplementedOnlyOnce()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IReadOnlyAndReadWriteProperties>()
				.ReadOnlyProperties().Implement(
					p => p.Get(m => m.Return(m.Default<TypeTemplate.TProperty>()))
				)
				.AllProperties().Implement(  // the Implement method only takes properties that were not implemented earlier
					p => p.Get(m => m.Return(m.Default<TypeTemplate.TProperty>())),
					p => p.Set((m, value) => { })
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IReadOnlyAndReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(0));
			Assert.That(obj.AString, Is.Null);
			Assert.That(obj.AnObject, Is.Null);

			Assert.That(obj.AnotherInt, Is.EqualTo(0));
			Assert.That(obj.AnotherString, Is.Null);
			Assert.That(obj.AnotherObject, Is.Null);

			obj.AnotherInt = 0;
			obj.AnotherString = null;
			obj.AnotherObject = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_SelectBySignature()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.Properties<int>().Implement(
					p => p.Get(m => m.Return(123)),
					p => p.Set((m, value) => { })
				)
				.Properties<string>().Implement(
					p => p.Get(m => m.Return("ABC")),
					p => p.Set((m, value) => { })
				)
				.Properties<object>().Implement(
					p => p.Get(m => m.Return(m.Const<object>(null))),
					p => p.Set((m, value) => { })
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(obj.AnObject, Is.Null);

			obj.AnInt = 0;
			obj.AString = null;
			obj.AnObject = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_SelectIndexers()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewPropertiesWithIndexers>()
				.AllProperties(where: p => !p.IsIndexer()).Implement(
					p => p.Get(m => m.Return(m.Default<TypeTemplate.TProperty>())),
					p => p.Set((m, value) => { })
				)
				.This<string, int>().Implement(
					p => p.Get((m, n) => m.Return(123)),
					p => p.Set((m, n, value) => { })
				)
				.This<int, string, string>().Implement(
					p => p.Get((m, n, s) => m.Return("ABC")),
					p => p.Set((m, n, s, value) => { })
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewPropertiesWithIndexers>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(0));
			Assert.That(obj.AString, Is.Null);
			Assert.That(obj.AnObject, Is.Null);

			Assert.That(obj["ZZZ"], Is.EqualTo(123));
			Assert.That(obj[999, "ZZZ"], Is.EqualTo("ABC"));

			obj.AnInt = 0;
			obj.AString = null;
			obj.AnObject = null;

			obj["ZZZ"] = 0;
			obj[999, "ZZZ"] = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_ImplementAutomatic()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			obj.AnInt = 321;
			obj.AString = "DEF";
			
			var anObjectValue = new object();
			obj.AnObject = anObjectValue;

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(321));
			Assert.That(obj.AString, Is.EqualTo("DEF"));
			Assert.That(obj.AnObject, Is.SameAs(anObjectValue));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceProperties_ImplementAutomatic_WithPrimaryConstructor()
		{
			//-- Arrange

			Field<int> theInt;
			Field<string> theString;
	
			DeriveClassFrom<object>()
				.PrimaryConstructor("TheInt", out theInt, "TheString", out theString)
				.ImplementInterface<AncestorRepository.IReadOnlyAndReadWriteProperties>()
				.Property(x => x.AnInt).ImplementAutomatic(theInt)
				.Property(x => x.AnotherString).ImplementAutomatic(theString)
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IReadOnlyAndReadWriteProperties>().UsingConstructor(123, "ABC");
			obj.AnotherInt = 456;

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AnotherString, Is.EqualTo("ABC"));

			Assert.That(obj.AString, Is.Null);
			Assert.That(obj.AnotherInt, Is.EqualTo(456));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BaseProperties_ImplementOneByOne()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.BaseTwo>()
				.DefaultConstructor()
				.Property(cls => cls.FirstValue).ImplementAutomatic()
				.Property(cls => cls.SecondValue).ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();

			obj.FirstValue = 123;
			obj.SecondValue = "ABC";

			//-- Assert

			Assert.That(obj.FirstValue, Is.EqualTo(123));
			Assert.That(obj.SecondValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BaseProperties_SelectAll()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.BaseTwo>()
				.DefaultConstructor()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();

			obj.FirstValue = 123;
			obj.SecondValue = "ABC";

			//-- Assert

			Assert.That(obj.FirstValue, Is.EqualTo(123));
			Assert.That(obj.SecondValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BaseMethods_ImplementOneByOne()
		{
			//-- Arrange

			Field<int> counterField;

			DeriveClassFrom<AncestorRepository.BaseThree>()
				.DefaultConstructor()
				.Field<int>("m_Counter", out counterField)
				.Method<int, int, int>(cls => cls.Add).Implement((m, x, y) => {
					m.Return(x + y);
				})
				.Method<int>(cls => cls.TakeNextCounter).Implement(m => {
					counterField.Assign(counterField + 1);
					m.Return(counterField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.BaseThree>().UsingDefaultConstructor();

			var sum = obj.Add(111, 222);
			var counterValue1 = obj.TakeNextCounter();
			var counterValue2 = obj.TakeNextCounter();

			//-- Assert

			Assert.That(sum, Is.EqualTo(333));
			Assert.That(counterValue1, Is.EqualTo(1));
			Assert.That(counterValue2, Is.EqualTo(2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void BaseMethods_SelectGroups()
		{
			//-- Arrange

			Field<int> counterField;

			DeriveClassFrom<AncestorRepository.BaseThree>()
				.DefaultConstructor()
				.Field<int>("m_Counter", out counterField)
				.NonVoidMethods<int, int, int>().Implement((m, x, y) => {
					m.Return(x + y);
				})
				.NonVoidMethods<int>(where: m => m.DeclaringType != typeof(object)).Implement(m => {
					counterField.Assign(counterField + 1);
					m.Return(counterField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.BaseThree>().UsingDefaultConstructor();

			var sum = obj.Add(111, 222);
			var counterValue1 = obj.TakeNextCounter();
			var counterValue2 = obj.TakeNextCounter();

			//-- Assert

			Assert.That(sum, Is.EqualTo(333));
			Assert.That(counterValue1, Is.EqualTo(1));
			Assert.That(counterValue2, Is.EqualTo(2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_OneByOne_ReferenceTypeArgsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethodsWithRefOutArgs>()
				.Method<string, string, string>(x => (s1, s2) => x.One(ref s1, out s2)).Implement((m, s1, s2) => {
					s2.Assign(s1 + s1);
					s1.Assign("Z");
					m.Return(s1 + s2);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethodsWithRefOutArgs>().UsingDefaultConstructor();

			string inputS1 = "A";
			string inputS2;
			string outputS = obj.One(ref inputS1, out inputS2);

			//-- Assert

			Assert.That(inputS1, Is.EqualTo("Z"));
			Assert.That(inputS2, Is.EqualTo("AA"));
			Assert.That(outputS, Is.EqualTo("ZAA"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_OneByOne_PrimitiveTypeArgsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethodsWithRefOutArgs>()
				.Method<int, int, int>(x => (n1, n2) => x.Two(ref n1, out n2)).Implement((m, n1, n2) => {
					n2.Assign(n1 + n1);
					n1.Assign(99);
					m.Return(n1 + n2);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethodsWithRefOutArgs>().UsingDefaultConstructor();

			int inputN1 = 1;
			int inputN2;
			int outputN = obj.Two(ref inputN1, out inputN2);

			//-- Assert

			Assert.That(inputN1, Is.EqualTo(99));
			Assert.That(inputN2, Is.EqualTo(2));
			Assert.That(outputN, Is.EqualTo(101));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_OneByOne_StructArgsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethodsWithRefOutArgs>()
				.Method<TimeSpan, TimeSpan, TimeSpan>(x => (t1, t2) => x.Three(ref t1, out t2)).Implement((m, t1, t2) => {
					t2.Assign(t1 + t1);
					t1.Assign(Static.Func(TimeSpan.FromHours, m.Const(9d)));
					m.Return(t1 + t2);
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethodsWithRefOutArgs>().UsingDefaultConstructor();

			TimeSpan inputT1 = TimeSpan.FromMinutes(1);
			TimeSpan inputT2;
			TimeSpan outputT = obj.Three(ref inputT1, out inputT2);

			//-- Assert

			Assert.That(inputT1, Is.EqualTo(TimeSpan.Parse("09:00:00")));
			Assert.That(inputT2, Is.EqualTo(TimeSpan.Parse("00:02:00")));
			Assert.That(outputT, Is.EqualTo(TimeSpan.Parse("09:02:00")));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceMethods_SelectAll_ArgsByRef()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethodsWithRefOutArgs>()
				.AllMethods().Implement(m => {
					var a1 = m.Argument<TypeTemplate.TReturn>(1);
					var a2 = m.Argument<TypeTemplate.TReturn>(2);
					a2.Assign(a1 + a1);
					a1.Assign(m.Default<TypeTemplate.TReturn>());
					m.Return(a2);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethodsWithRefOutArgs>().UsingDefaultConstructor();

			string inputS1 = "A";
			string inputS2;
			string outputS = obj.One(ref inputS1, out inputS2);

			int inputN1 = 1;
			int inputN2;
			int outputN = obj.Two(ref inputN1, out inputN2);

			TimeSpan inputT1 = TimeSpan.FromMinutes(1);
			TimeSpan inputT2;
			TimeSpan outputT = obj.Three(ref inputT1, out inputT2);

			//-- Assert

			Assert.That(inputS1, Is.Null);
			Assert.That(inputS2, Is.EqualTo("AA"));
			Assert.That(outputS, Is.EqualTo("AA"));

			Assert.That(inputN1, Is.EqualTo(0));
			Assert.That(inputN2, Is.EqualTo(2));
			Assert.That(outputN, Is.EqualTo(2));

			Assert.That(inputT1, Is.EqualTo(TimeSpan.Zero));
			Assert.That(inputT2, Is.EqualTo(TimeSpan.FromMinutes(2)));
			Assert.That(outputT, Is.EqualTo(TimeSpan.FromMinutes(2)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceEvents_ImplementAllAutomatic()
		{
			//-- Arrange

			var eventLog = new List<string>();

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewEvents>()
				.AllEvents().ImplementAutomatic()
				.Method(intf => intf.RaiseOne).Implement(m => {
					m.RaiseEvent("EventOne", Static.Prop(() => EventArgs.Empty));
				})
				.Method<string, string>(intf => intf.RaiseTwo).Implement((m, input) => {
					var eventArgs = m.Local(m.New<AncestorRepository.InOutEventArgs>());
					eventArgs.Prop(x => x.InputValue).Assign(input);
					m.RaiseEvent("EventTwo", eventArgs);
					m.Return(eventArgs.Prop(x => x.OutputValue));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewEvents>().UsingDefaultConstructor();

			obj.EventOne += (sender, args) => eventLog.Add("EventOne.A:" + args.ToString());
			obj.EventOne += (sender, args) => eventLog.Add("EventOne.B:" + args.ToString());

			obj.EventTwo += (sender, args) => {
				eventLog.Add("EventTwo.A:" + args.InputValue);
				args.OutputValue = "AAA";
			};
			obj.EventTwo += (sender, args) => {
				eventLog.Add("EventTwo.B:" + args.InputValue);
				args.OutputValue = "BBB";
			};

			obj.RaiseOne();
			var output = obj.RaiseTwo("INPUT");

			//-- Assert

			Assert.That(output, Is.EqualTo("BBB"));
			Assert.That(eventLog, Is.EqualTo(new[] {
				"EventOne.A:System.EventArgs", "EventOne.B:System.EventArgs",
				"EventTwo.A:INPUT", "EventTwo.B:INPUT"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InterfaceEvents_ImplementAllAutomatic_WithAllMethodsEmpty()
		{
			//-- Arrange

			var eventLog = new List<string>();

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewEvents>()
				.AllMethods().ImplementEmpty()
				.AllEvents().ImplementAutomatic();

			//-- Act & Assert

			// this must not fail:
			var obj = CreateClassInstanceAs<AncestorRepository.IFewEvents>().UsingDefaultConstructor();
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NewMethods_ImplementOneByOne()
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
        public void NewProperties_ImplementOneByOne()
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
        public void NewProperties_ImplementByTemaplte()
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
        public void NewCollectionProperties_ImplementByTemaplate()
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

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void ExplicitInterfaceMethods_ImplementOneByOne()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.LoggingBase>()
                .DefaultConstructor()
                .ImplementInterfaceExplicitly<AncestorRepository.IOneFuncLeft>()
                .Method<string>(intf => intf.One).Implement(m => {
                    m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOneFuncLeft.One"));
                    m.Return("AAA");
                })
                .ImplementInterfaceExplicitly<AncestorRepository.IOneFuncRight>()
                .Method<string>(intf => intf.One).Implement(m => {
                    m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOneFuncRight.One"));
                    m.Return("BBB");
                })
                .NewVirtualFunction<string>("One").Implement(m => {
                    m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("this.One"));
                    m.Return("CCC");
                });

            //-- Act

            object obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();
            AncestorRepository.IOneFuncLeft asLeft = (AncestorRepository.IOneFuncLeft)obj;
            AncestorRepository.IOneFuncRight asRight = (AncestorRepository.IOneFuncRight)obj;
            dynamic asObj = obj;

            var resultLeft = asLeft.One();
            var resultRight = asRight.One();
            var resultObj = asObj.One();

            //-- Assert

            Assert.That(asObj.TakeLog(), Is.EqualTo(new[] {
                "IOneFuncLeft.One",
                "IOneFuncRight.One",
                "this.One"
            }));

            Assert.That(resultLeft, Is.EqualTo("AAA"));
            Assert.That(resultRight, Is.EqualTo("BBB"));
            Assert.That(resultObj, Is.EqualTo("CCC"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void ExplicitInterfaceProperties_ImplementOneByOne()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.LoggingBase>()
                .DefaultConstructor()
                .ImplementInterfaceExplicitly<AncestorRepository.IOnePropertyLeft>()
                .Property<string>(intf => intf.One).Implement(
                    getter: p => p.Get(m => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOnePropertyLeft.One.Get()"));
                        m.Return(p.BackingField);
                    }),
                    setter: p => p.Set((m, value) => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOnePropertyLeft.One.Set(") + value + m.Const(")"));
                        p.BackingField.Assign(value);
                    }))
                .ImplementInterfaceExplicitly<AncestorRepository.IOnePropertyRight>()
                .Property<string>(intf => intf.One).Implement(
                    getter: p => p.Get(m => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOnePropertyRight.One.Get()"));
                        m.Return(p.BackingField);
                    }),
                    setter: p => p.Set((m, value) => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("IOnePropertyRight.One.Set(") + value + m.Const(")"));
                        p.BackingField.Assign(value);
                    }))
                .NewVirtualWritableProperty<string>("One").Implement(
                    getter: p => p.Get(m => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("this.One.Get()"));
                        m.Return(p.BackingField);
                    }),
                    setter: p => p.Set((m, value) => {
                        m.This<AncestorRepository.LoggingBase>().Void<string>(b => b.AddLog, m.Const("this.One.Set(") + value + m.Const(")"));
                        p.BackingField.Assign(value);
                    }));

            //-- Act

            object obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();
            AncestorRepository.IOnePropertyLeft asLeft = (AncestorRepository.IOnePropertyLeft)obj;
            AncestorRepository.IOnePropertyRight asRight = (AncestorRepository.IOnePropertyRight)obj;
            dynamic asObj = obj;

            asLeft.One = "LLL";
            asRight.One = "RRR";
            asObj.One = "OOO";

            var resultLeft = asLeft.One;
            var resultRight = asRight.One;
            var resultObj = asObj.One;

            //-- Assert

            Assert.That(asObj.TakeLog(), Is.EqualTo(new[] {
                "IOnePropertyLeft.One.Set(LLL)",
                "IOnePropertyRight.One.Set(RRR)",
                "this.One.Set(OOO)",
                "IOnePropertyLeft.One.Get()",
                "IOnePropertyRight.One.Get()",
                "this.One.Get()"
            }));

            Assert.That(resultLeft, Is.EqualTo("LLL"));
            Assert.That(resultRight, Is.EqualTo("RRR"));
            Assert.That(resultObj, Is.EqualTo("OOO"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void InterfaceProperties_ImplementAll_AsVirtualMembers()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterfaceVirtual<AncestorRepository.IFewReadWriteProperties>()
                .AllProperties().ImplementAutomatic();

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

            obj.AnInt = 321;
            obj.AString = "DEF";

            var anObjectValue = new object();
            obj.AnObject = anObjectValue;

            //-- Assert

            Assert.That(obj.AnInt, Is.EqualTo(321));
            Assert.That(obj.AString, Is.EqualTo("DEF"));
            Assert.That(obj.AnObject, Is.SameAs(anObjectValue));

            var propertyInfos = obj.GetType().GetProperties();

            Assert.That(propertyInfos.All(p => p.GetAccessors().All(m => m.IsVirtual)), "IsVirtual");
            Assert.That(propertyInfos.All(p => p.GetAccessors().All(m => !m.IsFinal)), "!IsFinal");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void InterfaceMethods_ImplementAll_AsVirtualMembers()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterfaceVirtual<AncestorRepository.IMoreMethods>()
                .AllMethods().ImplementEmpty();

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();

            //-- Assert

            obj.One();
            obj.Three(0);
            obj.Seven(TimeSpan.Zero, null, 0);

            Assert.That(obj.Eleven(), Is.EqualTo(0));
            Assert.That(obj.Twelwe(), Is.Null);
            Assert.That(obj.Fifteen(TimeSpan.Zero, null), Is.EqualTo(0));
            Assert.That(obj.Sixteen(0, TimeSpan.Zero), Is.Null);
            Assert.That(obj.Eighteen(null, 0, TimeSpan.Zero), Is.Null);

            var methodInfos = obj.GetType().GetMethods().Where(m => !m.IsStatic && m.DeclaringType != typeof(object)).ToArray();

            Assert.That(methodInfos.All(m => m.IsVirtual), "IsVirtual");
            Assert.That(methodInfos.All(m => !m.IsFinal), "!IsFinal");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NewStaticMethod_CallFromConstructor()
        {
            //-- Arrange

            MethodMember bytesToStreamFunc = null;

            DeriveClassFrom<AncestorRepository.BaseWithConstructorParameter>()
                .NewStaticFunction<byte[], Stream>("BytesToStream").Implement((m, bytes) => {
                    bytesToStreamFunc = m.OwnerMethod;
                    m.Return(m.New<MemoryStream>(bytes));
                })
                .Constructor<byte[]>((cw, bytes) => {
                    cw.Base(Static.Func<Stream>(bytesToStreamFunc, bytes));
                });

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.BaseWithConstructorParameter>().UsingConstructor(new byte[] { 11, 22, 33 });

            //-- Assert

            var memoryStream = (MemoryStream)obj.Stream;

            Assert.That(memoryStream.ToArray(), Is.EqualTo(new byte[] { 11, 22, 33 }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
	    public void MembersOfBaseTypeInheritedInMultiplePaths_ImplementedOnce()
	    {
            //-- Arrange
            
            DeriveClassFrom<AncestorRepository.DataRepoBase>()
                .ImplementInterface<AncestorRepository.IConcreteDataRepo1>()
                .Property(x => x.Objects1).Implement(getter: p => p.Get(m => m.Return(m.NewArray<object>(length: m.Const(0)))))
                .DefaultConstructor()
                .ImplementBase<AncestorRepository.DataRepoBase>()
                .Method<Type[]>(x => x.GetTypesInRepo).Implement(m => m.Return(m.NewArray<Type>(values: m.Const(typeof(object)))))
                .Property(x => x.IsAutoCommit).Implement(p => p.Get(m => m.Return(true)))
                .AllMethods(m => m.DeclaringType != typeof(object)).Throw<NotImplementedException>();

            //-- Act

            var obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();

            var typesInRepo1 = ((AncestorRepository.DataRepoBase)obj).GetTypesInRepo();
            var typesInRepo2 = ((AncestorRepository.IConcreteDataRepo1)obj).GetTypesInRepo();
            var typesInRepo3 = ((AncestorRepository.IDataRepo)obj).GetTypesInRepo();

            //-- Assert

            Assert.That(typesInRepo1, Is.EqualTo(new[] { typeof(object) }));
            Assert.That(typesInRepo2, Is.EqualTo(new[] { typeof(object) }));
            Assert.That(typesInRepo3, Is.EqualTo(new[] { typeof(object) }));
        }
    }
}
