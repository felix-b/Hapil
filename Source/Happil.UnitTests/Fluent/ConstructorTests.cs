using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
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

			FieldAccessOperand<int> intField;
			FieldAccessOperand<string> stringField;

			DeriveClassFrom<AncestorRepository.BaseOne>()
				.Field<int>("m_IntField", out intField)
				.Field<string>("m_StringField", out stringField)
				.Constructor<int, string>((ctor, intValue, stringValue) => {
					ctor.Base();
					intField.Assign(intValue);
					stringField.Assign(stringValue);
				})
				.ImplementInterface<IMyFieldValues>()
				.Method<int>(intf => intf.GetIntFieldValue).Implement(f => {
					f.Return(intField);
				})
				.Method<string>(intf => intf.GetStringFieldValue).Implement(f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingConstructor<int, string>(123, "ABC");

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateStaticConstructor()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.StaticConstructor(m => {
					Static.Prop(() => OutputList).Add(m.Const(".CCTOR"));
				})
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOneProperty>()
				.AllProperties().ImplementAutomatic();

			OutputList = new List<string>();

			//-- Act

			var obj1 = CreateClassInstanceAs<AncestorRepository.IOneProperty>().UsingDefaultConstructor();
			var output1 = OutputList.ToArray();

			var obj2 = CreateClassInstanceAs<AncestorRepository.IOneProperty>().UsingDefaultConstructor();
			var output2 = OutputList.ToArray();

			//-- Assert

			Assert.That(output1, Is.EqualTo(new[] { ".CCTOR" }));
			Assert.That(output2, Is.EqualTo(new[] { ".CCTOR" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeStaticFields()
		{
			//-- Arrange

			FieldAccessOperand<int> intField;
			FieldAccessOperand<string> stringField;

			DeriveClassFrom<object>()
				.StaticField<int>("s_IntField", out intField)
				.StaticField<string>("s_StringField", out stringField)
				.StaticConstructor(m => {
					intField.AssignConst(123);
					stringField.AssignConst("ABC");
				})
				.DefaultConstructor()
				.ImplementInterface<IMyFieldValues>()
				.Method<int>(intf => intf.GetIntFieldValue).Implement(f => {
					f.Return(intField);
				})
				.Method<string>(intf => intf.GetStringFieldValue).Implement(f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<string> OutputList { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMyFieldValues
		{
			int GetIntFieldValue();
			string GetStringFieldValue();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MyFieldValuesExample : AncestorRepository.BaseOne, IMyFieldValues
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
