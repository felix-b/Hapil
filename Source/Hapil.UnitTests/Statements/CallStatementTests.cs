using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Statements
{
	[TestFixture]
	public class CallStatementTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void CanCallVoidMethodOnReferenceTypeTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.ITargetObjectCaller>()
				.Method<object, object>(intf => intf.CallTheTarget).Implement((m, target) => {
					target.CastTo<TargetOne>().Void(x => x.CallMe);
					m.Return(null);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITargetObjectCaller>().UsingDefaultConstructor();
			var targetObj = new TargetOne();

			obj.CallTheTarget(targetObj);
			obj.CallTheTarget(targetObj);

			//-- Assert

			Assert.That(targetObj.TimesCalled, Is.EqualTo(2));
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallNonVoidMethodOnReferenceTypeTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.ITargetObjectCaller>()
				.Method<object, object>(intf => intf.CallTheTarget).Implement((m, target) => {
					m.Return(target.Func<string>(x => x.ToString).CastTo<object>());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITargetObjectCaller>().UsingDefaultConstructor();
			var returnValue = obj.CallTheTarget(123);

			//-- Assert

			Assert.That(returnValue, Is.EqualTo("123"));
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallMethodOnValueTypeTarget()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.ITargetValueTypeCaller>()
				.Method<int, object>(intf => intf.CallTheTarget).Implement((m, value) => {
					m.Return(value.Func<string>(x => x.ToString));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITargetValueTypeCaller>().UsingDefaultConstructor();
			var returnValue = obj.CallTheTarget(123);

			//-- Assert

			Assert.That(returnValue, Is.EqualTo("123"));
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallStaticVoidMethod()
		{
			//-- Arrange

			StaticTargetOne.ResetTimesCalled();

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.ITargetObjectCaller>()
				.Method<object, object>(intf => intf.CallTheTarget).Implement((m, value) => {
					Static.Void(StaticTargetOne.CallMe);
					m.Return(null);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITargetObjectCaller>().UsingDefaultConstructor();

			obj.CallTheTarget(null);
			obj.CallTheTarget(null);

			//-- Assert

			Assert.That(StaticTargetOne.TimesCalled, Is.EqualTo(2));
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallStaticNonVoidFunctionWithArguments()
		{
			//-- Arrange

			StaticTargetOne.ResetTimesCalled();

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.ITargetValueTypeCaller>()
				.Method<int, object>(intf => intf.CallTheTarget).Implement((m, value) => {
					m.Return(Static.Func(StaticTargetOne.IncrementMe, value).CastTo<object>());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITargetValueTypeCaller>().UsingDefaultConstructor();

			var value1 = (int)obj.CallTheTarget(111);
			var value2 = (int)obj.CallTheTarget(222);

			//-- Assert

			Assert.That(StaticTargetOne.TimesCalled, Is.EqualTo(333));
			Assert.That(value1, Is.EqualTo(0));
			Assert.That(value2, Is.EqualTo(111));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TargetOne
		{
			public void CallMe()
			{
				this.TimesCalled++;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int TimesCalled { get; private set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class StaticTargetOne
		{
			public static void CallMe()
			{
				TimesCalled++;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int IncrementMe(int delta)
			{
				var currentValue = TimesCalled;
				TimesCalled += delta;
				return currentValue;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static void ResetTimesCalled()
			{
				TimesCalled = 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int TimesCalled { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static string SetMe { get; set; }
		}
	}
}
