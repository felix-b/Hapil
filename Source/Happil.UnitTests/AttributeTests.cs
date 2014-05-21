using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Testing.NUnit;
using Happil.Expressions;
using Happil.Operands;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class AttributeTests : NUnitEmittedTypesTestBase
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

			ImplementationClassWriter<object> classBody = DeriveClassFrom<object>().DefaultConstructor();
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
		public void AttributeOnConstructor_Default()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor(Attributes.Set<TestAttributeOne>());

			//-- Act

			var obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();

			//-- Assert

			AssertAttribute<TestAttributeOne>(obj.GetType().GetConstructor(Type.EmptyTypes));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnConstructor_NonDefault()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.Constructor<int>(
					attributes: Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "AAA")),
					body: (m, p1) => { })
				.Constructor<int, string>(
					attributes: Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "BBB")),
					body: (m, p1, p2) => { })
				.Constructor<int, string, TimeSpan>(
					attributes: Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "CCC")),
					body: (m, p1, p2, p3) => { });

			//-- Act

			var obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(
				AssertAttribute<TestAttributeOne>(obj.GetType().GetConstructor(new[] { typeof(int) })).StringValue, 
				Is.EqualTo("AAA"));

			Assert.That(
				AssertAttribute<TestAttributeOne>(obj.GetType().GetConstructor(new[] { typeof(int), typeof(string) })).StringValue,
				Is.EqualTo("BBB"));
			
			Assert.That(
				AssertAttribute<TestAttributeOne>(obj.GetType().GetConstructor(new[] { typeof(int), typeof(string), typeof(TimeSpan) })).StringValue,
				Is.EqualTo("CCC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceMethod_ImplementOneByOne()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IMoreMethods>()
				.Method(intf => intf.One).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(100)), 
					m => { })
				.Method<int>(intf => intf.Three).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(300)),
					(m, p1) => { })
				.Method<int, string>(intf => intf.Five).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(500)),
					(m, p1, p2) => { })
				.Method<TimeSpan, string, int>(intf => intf.Seven).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(700)),
					(m, p1, p2, p3) => { })
				.Method<int>(intf => intf.Eleven).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(1100)), 
					m => m.Return(0))
				.Method<string, int>(intf => intf.Thirteen).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(1300)),
					(m, p1) => m.Return(0))
				.Method<TimeSpan, string, int>(intf => intf.Fifteen).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(1500)),
					(m, p1, p2) => m.Return(0))
				.Method<TimeSpan, string, int, object>(intf => intf.Seventeen).Implement(
					Attributes.Set<TestAttributeOne>(a => a.Arg(1700)),
					(m, p1, p2, p3) => m.Return(null))
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();
			var type = obj.GetType();

			//-- Assert

			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("One")).IntValue, Is.EqualTo(100));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Three")).IntValue, Is.EqualTo(300));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Five")).IntValue, Is.EqualTo(500));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Seven")).IntValue, Is.EqualTo(700));

			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Eleven")).IntValue, Is.EqualTo(1100));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Thirteen")).IntValue, Is.EqualTo(1300));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Fifteen")).IntValue, Is.EqualTo(1500));
			Assert.That(AssertAttribute<TestAttributeOne>(type.GetMethod("Seventeen")).IntValue, Is.EqualTo(1700));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceMethod_ImplementTemplate()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(m => m.IsVoid()).Implement(
					m => Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "VOID_" + m.Name)),
					m => { }
				)
				.AllMethods().Implement(
					m => Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "NON_VOID_" + m.Name)),
					m => { m.Return(m.Default<TypeTemplate.TReturn>()); }
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			//-- Assert

			var methodOne = obj.GetType().GetMethod("One");
			var attributeOne = methodOne.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			var methodThree = obj.GetType().GetMethod("Three");
			var attributeThree = methodThree.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(attributeOne, Is.Not.Null);
			Assert.That(attributeOne.StringValue, Is.EqualTo("VOID_One"));

			Assert.That(attributeThree, Is.Not.Null);
			Assert.That(attributeThree.StringValue, Is.EqualTo("NON_VOID_Three"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnMethodParameters()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IMoreMethods>()
				.Method<string, int>(intf => intf.Six).Implement((m, str, num) => {
					str.Attribute<TestAttributeOne>(a => a.Named(x => x.StringValue, "PARAM_S"));
					num.Attribute<TestAttributeOne>(a => a.Named(x => x.StringValue, "PARAM_N"));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IMoreMethods>().UsingDefaultConstructor();

			//-- Assert

			var parameters = obj.GetType().GetMethod("Six").GetParameters();
			var attribute1 = parameters[0].GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();
			var attribute2 = parameters[1].GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(attribute1, Is.Not.Null);
			Assert.That(attribute1.StringValue, Is.EqualTo("PARAM_S"));

			Assert.That(attribute2, Is.Not.Null);
			Assert.That(attribute2.StringValue, Is.EqualTo("PARAM_N"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnMethodReturnParameter()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(m => !m.IsVoid()).Implement(
					m => {
						m.ReturnAttributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "NON_VOID_" + m.OwnerMethod.Name));
						m.Return(m.Default<TypeTemplate.TReturn>());
					}
				)
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			//-- Assert

			var method = obj.GetType().GetMethod("Three");
			var returnAttribute = method.ReturnParameter.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(returnAttribute, Is.Not.Null);
			Assert.That(returnAttribute.StringValue, Is.EqualTo("NON_VOID_Three"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceProperty_ImplementOneByOne()
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

		[Test]
		public void AttributeOnInterfaceProperty_ImplementAutomatic()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic(Attributes.Set<TestAttributeOne>());

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var intProperty = obj.GetType().GetProperty("AnInt");
			var intAttribute = intProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(intAttribute, Is.Not.Null);

			var stringProperty = obj.GetType().GetProperty("AString");
			var stringAttribute = stringProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(stringAttribute, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceProperty_ImplementAutomatic_UsePropertyNameInAttribute()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic(prop => Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, prop.Name)));

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var intProperty = obj.GetType().GetProperty("AnInt");
			var intAttribute = intProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(intAttribute, Is.Not.Null);
			Assert.That(intAttribute.StringValue, Is.EqualTo("AnInt"));

			var stringProperty = obj.GetType().GetProperty("AString");
			var stringAttribute = stringProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(stringAttribute, Is.Not.Null);
			Assert.That(stringAttribute.StringValue, Is.EqualTo("AString"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceProperty_ImplementTemplate()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IReadOnlyAndReadWriteProperties>()
				.ReadOnlyProperties().Implement(
					Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "R/O")), 
					prop => prop.Get(m => m.Return(m.Default<TypeTemplate.TProperty>()))
				)
				.AllProperties().Implement(
					Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "R/W")), 
					prop => prop.Get(m => m.Return(prop.BackingField)),
					prop => prop.Set((m, value) => prop.BackingField.Assign(value))
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IReadOnlyAndReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var readOnlyProperty = obj.GetType().GetProperty("AnInt");
			var readOnlyAttribute = readOnlyProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(readOnlyAttribute, Is.Not.Null);
			Assert.That(readOnlyAttribute.StringValue, Is.EqualTo("R/O"));

			var readWriteProperty = obj.GetType().GetProperty("AnotherInt");
			var readWriteAttribute = readWriteProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(readWriteAttribute, Is.Not.Null);
			Assert.That(readWriteAttribute.StringValue, Is.EqualTo("R/W"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceProperty_ImplementTemplate_UsePropertyNameInAttribute()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IReadOnlyAndReadWriteProperties>()
				.ReadOnlyProperties().Implement(
					prop => Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "R/O_" + prop.Name)),
					prop => prop.Get(m => m.Return(m.Default<TypeTemplate.TProperty>()))
				)
				.AllProperties().Implement(
					prop => Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "R/W_" + prop.Name)),
					prop => prop.Get(m => m.Return(prop.BackingField)),
					prop => prop.Set((m, value) => prop.BackingField.Assign(value))
				);

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IReadOnlyAndReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var readOnlyProperty = obj.GetType().GetProperty("AnInt");
			var readOnlyAttribute = readOnlyProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(readOnlyAttribute, Is.Not.Null);
			Assert.That(readOnlyAttribute.StringValue, Is.EqualTo("R/O_AnInt"));

			var readWriteProperty = obj.GetType().GetProperty("AnotherInt");
			var readWriteAttribute = readWriteProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(readWriteAttribute, Is.Not.Null);
			Assert.That(readWriteAttribute.StringValue, Is.EqualTo("R/W_AnotherInt"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnField_Instance()
		{
			//-- Arrange

			Field<int> intField;
			Field<string> stringField;

			DeriveClassFrom<object>()
				.Field<int>("m_AnInt", Attributes.Set<TestAttributeOne>(), out intField)
				.Field<string>("m_AString", out stringField)
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.Property(intf => intf.AnInt).Implement(
					p => p.Get(m => m.Return(intField)),
					p => p.Set((m, value) => intField.Assign(value))
				)
				.Property(intf => intf.AString).Implement(
					p => p.Get(m => m.Return(stringField)),
					p => p.Set((m, value) => stringField.Assign(value))
				)
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var intFieldInfo = obj.GetType().GetField("m_AnInt", BindingFlags.Instance | BindingFlags.NonPublic);
			var intAttribute = intFieldInfo.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(intAttribute, Is.Not.Null);

			var stringFieldInfo = obj.GetType().GetField("m_AString", BindingFlags.Instance | BindingFlags.NonPublic);
			var stringAttribute = stringFieldInfo.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(stringAttribute, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnField_Static()
		{
			//-- Arrange

			Field<int> intField;
			Field<string> stringField;

			DeriveClassFrom<object>()
				.StaticField<int>("s_AnInt", Attributes.Set<TestAttributeOne>(), out intField)
				.StaticField<string>("s_AString", out stringField)
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.Property(intf => intf.AnInt).Implement(
					p => p.Get(m => m.Return(intField)),
					p => p.Set((m, value) => intField.Assign(value))
				)
				.Property(intf => intf.AString).Implement(
					p => p.Get(m => m.Return(stringField)),
					p => p.Set((m, value) => stringField.Assign(value))
				)
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			var intFieldInfo = obj.GetType().GetField("s_AnInt", BindingFlags.Static | BindingFlags.NonPublic);
			var intAttribute = intFieldInfo.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(intAttribute, Is.Not.Null);

			var stringFieldInfo = obj.GetType().GetField("s_AString", BindingFlags.Static | BindingFlags.NonPublic);
			var stringAttribute = stringFieldInfo.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().FirstOrDefault();

			Assert.That(stringAttribute, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AttributeOnInterfaceProperty_Indexers()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewPropertiesWithIndexers>()
				.This<string, int>().Implement(
					Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "1ARG")),
					p => p.Get((m, s) => m.Throw<NotImplementedException>("")),
					p => p.Set((m, s, value) => m.Throw<NotImplementedException>(""))
				)
				.This<int, string, string>().Implement(
					Attributes.Set<TestAttributeOne>(a => a.Named(x => x.StringValue, "2ARGS")),
					p => p.Get((m, n, s) => m.Throw<NotImplementedException>("")),
					p => p.Set((m, n, s, value) => m.Throw<NotImplementedException>(""))
				)
				.AllProperties().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewPropertiesWithIndexers>().UsingDefaultConstructor();

			//-- Assert

			var indexer1ArgProperty = obj.GetType().GetProperty("Item", new Type[] { typeof(string) });
			var indexer1ArgAttribute = indexer1ArgProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(indexer1ArgAttribute, Is.Not.Null);
			Assert.That(indexer1ArgAttribute.StringValue, Is.EqualTo("1ARG"));

			var indexer2ArgsProperty = obj.GetType().GetProperty("Item", new Type[] { typeof(int), typeof(string) });
			var indexer2ArgsAttribute = indexer2ArgsProperty.GetCustomAttributes(inherit: false).OfType<TestAttributeOne>().Single();

			Assert.That(indexer2ArgsAttribute, Is.Not.Null);
			Assert.That(indexer2ArgsAttribute.StringValue, Is.EqualTo("2ARGS"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TAttribute AssertAttribute<TAttribute>(MemberInfo member, bool shouldExist = true) where TAttribute : Attribute
		{
			var attribute = member.GetCustomAttributes(inherit: false).OfType<TAttribute>().FirstOrDefault();

			if ( shouldExist )
			{
				Assert.IsNotNull(attribute, "Expected attribute of type " + typeof(TAttribute).Name);
			}
			else
			{
				Assert.IsNull(attribute, "Unexpected attribute of type " + typeof(TAttribute).Name);
			}

			return attribute;
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
