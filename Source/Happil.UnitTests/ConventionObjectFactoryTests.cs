using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Happil.Decorators;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class ConventionObjectFactoryTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void CanImplementObjectByConventions()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module, 
				new DefaultConstructorConvention(),
				new AutomaticPropertyConvention());

			//-- Act

			var obj = factory.CreateInstanceOf<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			obj.AString = "ABC";
			obj.AnInt = 123;

			//-- Assert

			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.GetType().Assembly.IsDynamic);

			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(obj.AnInt, Is.EqualTo(123));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanChangeClassName()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module,
				new ClassNameConvention(TestCaseClassName),
				new DefaultConstructorConvention(),
				new AutomaticPropertyConvention());

			//-- Act

			var obj = factory.CreateInstanceOf<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.GetType().FullName, Is.EqualTo(base.TestCaseClassName));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanChangeBaseType()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module,
				new ClassNameConvention(TestCaseClassName),
				new BaseTypeConvention(typeof(AncestorRepository.BaseOne)),
				new DefaultConstructorConvention());

			//-- Act

			var obj = factory.CreateInstanceOf<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.GetType().BaseType, Is.EqualTo(typeof(AncestorRepository.BaseOne)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanDecorateObjectByConventions()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module,
				new ClassNameConvention(TestCaseClassName),
				new DefaultConstructorConvention(),
				new AutomaticPropertyConvention(),
				new DataContractConvention());

			//-- Act

			var obj = factory.CreateInstanceOf<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.GetType().IsDefined(typeof(DataContractAttribute), inherit: true));
			Assert.That(obj.GetType().GetProperties().All(p => p.IsDefined(typeof(DataMemberAttribute), inherit: true)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanFilterConventions()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module,
				new ClassNameConvention(TestCaseClassName),
				new DefaultConstructorConvention(),
				new AutomaticPropertyConvention(),
				new EmptyMethodConvention(),
				new HasPropertiesConvention());

			//-- Act

			var obj1 = factory.CreateInstanceOf<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();
			var obj2 = factory.CreateInstanceOf<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj1.GetType().IsDefined(typeof(HasPropertiesAttribute), inherit: true), Is.True);
			Assert.That(obj2.GetType().IsDefined(typeof(HasPropertiesAttribute), inherit: true), Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanUseTransientConventions()
		{
			//-- Arrange

			var factory = new ConventionObjectFactory(
				base.Module,
				transientConventionFactory: context => new IObjectFactoryConvention[] {
					new ClassNameConvention(TestCaseClassName),
					new DefaultConstructorConvention(),
					new AutomaticPropertyConvention()
				});

			//-- Act

			var obj = factory.CreateInstanceOf<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();
			obj.AString = "ABC";

			//-- Assert

			Assert.That(obj.GetType().FullName, Is.EqualTo(base.TestCaseClassName));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ClassNameConvention : ImplementationConvention
		{
			private readonly string m_ClassFullName;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ClassNameConvention(string classFullName)
				: base(Will.InspectDeclaration)
			{
				m_ClassFullName = classFullName;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnInspectDeclaration(ObjectFactoryContext context)
			{
				context.ClassFullName = m_ClassFullName;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BaseTypeConvention : ImplementationConvention
		{
			private readonly Type m_BaseType;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public BaseTypeConvention(Type baseType)
				: base(Will.InspectDeclaration)
			{
				m_BaseType = baseType;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnInspectDeclaration(ObjectFactoryContext context)
			{
				context.BaseType = m_BaseType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DefaultConstructorConvention : ImplementationConvention
		{
			public DefaultConstructorConvention()
				: base(Will.ImplementBaseClass)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
			
			protected override void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
			{
				writer.DefaultConstructor();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class AutomaticPropertyConvention : ImplementationConvention
		{
			public AutomaticPropertyConvention()
				: base(Will.ImplementPrimaryInterface)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
			{
				writer.AllProperties().ImplementAutomatic();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class EmptyMethodConvention : ImplementationConvention
		{
			public EmptyMethodConvention()
				: base(Will.ImplementPrimaryInterface)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
			{
				writer.AllMethods().ImplementEmpty();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DataContractConvention : DecorationConvention
		{
			public DataContractConvention()
				: base(Will.DecorateProperties)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter classWriter)
			{
				classWriter.Attribute<DataContractAttribute>(values => values.Named(a => a.Namespace, "http://mydto"));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
			{
				decorate().Attribute<DataMemberAttribute>();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class HasPropertiesConvention : DecorationConvention
		{
			public HasPropertiesConvention()
				: base(Will.DecorateClass)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override bool ShouldApply(ObjectFactoryContext context)
			{
				return (
					context.TypeKey.PrimaryInterface != null && 
					TypeMemberCache.Of(context.TypeKey.PrimaryInterface).ImplementableProperties.Any());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter classWriter)
			{
				classWriter.Attribute<HasPropertiesAttribute>();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class HasPropertiesAttribute : Attribute
		{
		}
	}
}
