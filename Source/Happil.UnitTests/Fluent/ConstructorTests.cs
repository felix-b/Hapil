using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class ConstructorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanCreateObjectUsingNonDefaultConstructor()
		{
			//-- Arrange

			HappilField<int> intField;
			HappilField<string> stringField;

			DeriveClassFrom<TestBaseOne>()
				.Field<int>("m_IntField", out intField)
				.Field<string>("m_StringField", out stringField)
				.Constructor<int, string>((ctor, intValue, stringValue) => {
					ctor.Base();
					intField.Assign(intValue);
					stringField.Assign(stringValue);
				})
				.Implement<IMyFieldValues>()
				.Function<int>(intf => intf.GetIntFieldValue, f => {
					f.Return(intField);
				})
				.Function<string>(intf => intf.GetStringFieldValue, f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingConstructor<int, string>(123, "ABC");

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMyFieldValues
		{
			int GetIntFieldValue();
			string GetStringFieldValue();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MyFieldValuesExample : TestBaseOne, IMyFieldValues
		{
			private int m_IntField;
			private string m_StringField;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MyFieldValuesExample(int intValue, string stringValue)
			{
				m_IntField = intValue;
				m_StringField = stringValue;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int GetIntFieldValue()
			{
				return m_IntField;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string GetStringFieldValue()
			{
				return m_StringField;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static IMyFieldValues FactoryMethod1(int arg1, string arg2)
			{
				return new MyFieldValuesExample(arg1, arg2);
			}
		}
	}
}
