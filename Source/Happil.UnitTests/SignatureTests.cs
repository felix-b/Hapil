using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using Happil.Selectors;
using Moq;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class SignatureTests : ClassPerTestCaseFixtureBase
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
				.Method<int>(intf => intf.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(intf => intf.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(intf => intf.Five).Implement((m, n) => m.ReturnConst("ABC"));

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
				.NonVoidMethods<int>().Implement(m => m.ReturnConst(123))
				.NonVoidMethods<string>().Implement(m => m.ReturnConst("ABC"))
				.NonVoidMethods<int, string>().Implement((m, n) => m.ReturnConst("DEF"))
				.NonVoidMethods<string, int>().Implement((m, s) => m.ReturnConst(456))
				.NonVoidMethods<TimeSpan, string, int>().Implement((m, t, s) => m.ReturnConst(789))
				.NonVoidMethods<int, TimeSpan, string>().Implement((m, n, t) => m.ReturnConst("GHI"))
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
					p => p.Get(m => m.ReturnConst(123)),
					p => p.Set((m, value) => { }))
				.Property(intf => intf.AString).Implement(
					p => p.Get(m => m.ReturnConst("ABC")),
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
					p => p.Get(m => m.ReturnConst(123)),
					p => p.Set((m, value) => { })
				)
				.Properties<string>().Implement(
					p => p.Get(m => m.ReturnConst("ABC")),
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
					p => p.Get((m, n) => m.ReturnConst(123)),
					p => p.Set((m, n, value) => { })
				)
				.This<int, string, string>().Implement(
					p => p.Get((m, n, s) => m.ReturnConst("ABC")),
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

			HappilField<int> counterField;

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

			HappilField<int> counterField;

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
					s1.AssignConst("Z");
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
					n1.AssignConst(99);
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
	}
}
