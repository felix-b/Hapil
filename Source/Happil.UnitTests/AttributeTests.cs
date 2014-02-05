using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class AttributeTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void AttributeOnClass_NoArguments()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Attribute<TestAttributeOne>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var attribute = obj.GetType().GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(attribute, Is.Not.Null);
			Assert.That(attribute.IntValue, Is.EqualTo(0));
			Assert.That(attribute.StringValue, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnClass_ConstructorArguments()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Attribute<TestAttributeOne>(a => a.Arg(123).Arg("ABC"))
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var attribute = obj.GetType().GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(attribute, Is.Not.Null);
			Assert.That(attribute.IntValue, Is.EqualTo(123));
			Assert.That(attribute.StringValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnClass_ConstructorArguments_AnotherOverload()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Attribute<TestAttributeOne>(a => a.Arg(987))
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var attribute = obj.GetType().GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(attribute, Is.Not.Null);
			Assert.That(attribute.IntValue, Is.EqualTo(987));
			Assert.That(attribute.StringValue, Is.EqualTo("987"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnClass_ConstructorArguments_Invalid()
		{
			//-- Arrange

			IHappilClassBody<object> classBody = DeriveClassFrom<object>().DefaultConstructor();
			ArgumentException caughtException;

			//-- Act

			try
			{
				ExpectException<ArgumentException>(
					() => {
						classBody.Attribute<TestAttributeOne>(a => a.Arg(TimeSpan.Zero));
					},
					out caughtException);
			}
			finally
			{
				CreateClassInstanceAs<object>().UsingDefaultConstructor();
			}

			//-- Assert

			Assert.That(caughtException, Is.Not.Null);
			StringAssert.Contains("signature", caughtException.Message);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnClass_NamedArguments()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Attribute<TestAttributeOne>(a => a.Named(x => x.IntValue, 123).Named(x => x.StringValue, "ABC"))
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var attribute = obj.GetType().GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(attribute, Is.Not.Null);
			Assert.That(attribute.IntValue, Is.EqualTo(123));
			Assert.That(attribute.StringValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnClass_MixedArguments()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Attribute<TestAttributeOne>(a => a.Arg(123).Named(x => x.StringValue, "XYZ"))
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var attribute = obj.GetType().GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(attribute, Is.Not.Null);
			Assert.That(attribute.IntValue, Is.EqualTo(123));
			Assert.That(attribute.StringValue, Is.EqualTo("XYZ"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceMethodImplementation()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg("999")), 
					m => { }
				)
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			//-- Assert

			var methodOne = obj.GetType().GetMethod("One");
			var attributeOne = methodOne.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			var methodTwo = obj.GetType().GetMethod("Two");
			var attributeTwo = methodTwo.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(attributeOne, Is.Not.Null);
			Assert.That(attributeOne.IntValue, Is.EqualTo(999));
			Assert.That(attributeOne.StringValue, Is.EqualTo("999"));

			Assert.That(attributeTwo, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfacePropertyImplementation()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.Property(intf => intf.AnInt).Implement(
					Attributes.Set<TestAttributeOne>(),
					p => p.Get(m => m.Throw<NotImplementedException>("")),
					p => p.Set((m, value) => m.Throw<NotImplementedException>(""))
				)
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var intProperty = obj.GetType().GetProperty("AnInt");
			var intAttribute = intProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();
			
			Assert.That(intAttribute, Is.Not.Null);

			var stringProperty = obj.GetType().GetProperty("AString");
			var stringAttribute = stringProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();
			
			Assert.That(stringAttribute, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestAttributeOne : Attribute
		{
			public TestAttributeOne()
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestAttributeOne(int num)
			{
				IntValue = num;
				StringValue = num.ToString();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestAttributeOne(string str)
			{
				StringValue = str;
				IntValue = Int32.Parse(str);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestAttributeOne(int num, string str)
			{
				IntValue = num;
				StringValue = str;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int IntValue;
			public string StringValue { get; set; }
		}
	}
}
