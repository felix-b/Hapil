using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class ConstructorTests : ClassPerTestCaseFixtureBase
	{
		[Test, Ignore("Not yet completed")]
		public void CanCreateObjectUsingNonDefaultConstructor()
		{
			//-- Arrange

			HappilField<int> intField;
			HappilField<string> stringField;

			DeriveClassFrom<TestBaseOne>()
				.Field<int>("m_IntField", out intField)
				.Field<string>("m_StringField", out stringField)
				//.Constructor<int, string>((ctor, intValue, stringValue) => {
				//	intField.Assign(intValue);
				//	stringField.Assign(stringValue);
				//})
				.Implement<IMyFieldValues>()
				.Function<int>(x => x.GetIntFieldValue, f => {
					f.Return(intField);
				})
				.Function<string>(x => x.GetStringFieldValue, f => {
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
	}
}
