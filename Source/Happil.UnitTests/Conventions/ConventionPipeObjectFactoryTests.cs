#if false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Happil.Conventions;
using Happil.Decorators;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests.Conventions
{
	[TestFixture]
	public class ConventionPipeObjectFactoryTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanCreateObjectByConvention()
		{
			//-- Arrange

			var factory = new ConventionPipeObjectFactory(
				base.Module, 
				new SingletonReflectionConventionResolver(), 
				new DtoConvention(),
				new DataContractConvention());

			//-- Act

			var obj = (AncestorRepository.IFewReadWriteProperties)factory.CreateInstance<AncestorRepository.IFewReadWriteProperties>();

			obj.AString = "ABC";
			obj.AnInt = 123;

			//-- Assert

			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.GetType().Assembly.IsDynamic);


			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(obj.AnInt, Is.EqualTo(123));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DtoConvention : ImplementationConventionBase
		{
			protected override void ImplementAnyBase(Type baseType, ImplementationClassWriter<TypeTemplate.TBase> writer)
			{
				writer.AllProperties().ImplementAutomatic();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DataContractConvention : DecorationConventionBase
		{
			public override void OnClass(ClassType classType, ClassWriterBase writer)
			{
				writer.Attribute<DataContractAttribute>(values => values.Named(a => a.Namespace, "http://mydto"));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
			{
				decorate().Attribute<DataMemberAttribute>();
			}
		}
	}
}


#endif