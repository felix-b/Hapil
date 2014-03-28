using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class ObjectFactoryBaseTests
	{
		private DynamicModule m_Module;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			m_Module = new DynamicModule(
				simpleName: "Happil.UnitTests.EmittedByObjectFactoryBaseTests",
				allowSave: true,
				saveDirectory: TestContext.CurrentContext.TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			m_Module.SaveAssembly();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateEmptyObject()
		{
			//-- Arrange

			var factory = new EmptyObjectFactory(m_Module);

			//-- Act

			var obj = factory.CreateEmptyObject();

			//-- Assert

			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.GetType(), Is.Not.SameAs(typeof(object)));
			Assert.That(obj.GetType().Assembly.IsDynamic, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanImplementMarkerInterface()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			AncestorRepository.IMakerInterfaceOne one = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FactoryAlwaysCreatesNewInstances()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			var one1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			var one2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();

			//-- Assert

			Assert.That(one1, Is.Not.SameAs(one2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DynamicTypesAreReusedPerSameKey()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			var one1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			var one2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			
			var two1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();
			var two2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();

			//-- Assert

			Assert.That(one1.GetType(), Is.SameAs(one2.GetType()));
			Assert.That(two1.GetType(), Is.SameAs(two2.GetType()));
			
			Assert.That(one1.GetType(), Is.Not.SameAs(two1.GetType()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class EmptyObjectFactory : ObjectFactoryBase
		{
			public EmptyObjectFactory(DynamicModule module)
				: base(module)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public object CreateEmptyObject()
			{
				return base.GetOrBuildType(new TypeKey()).CreateInstance<object>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override ClassType DefineNewClass(TypeKey key)
			{
				return DeriveClassFrom<object>(key).DefaultConstructor();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MarkerInterfaceFactory : ObjectFactoryBase
		{
			public MarkerInterfaceFactory(DynamicModule module)
				: base(module)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TMarker CreateMarkerObject<TMarker>()
			{
				var typeEntry = base.GetOrBuildType(new TypeKey(primaryInterface: typeof(TMarker)));
				return typeEntry.CreateInstance<TMarker>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override ClassType DefineNewClass(TypeKey key)
			{
				return DeriveClassFrom<object>(key)
					.DefaultConstructor()
					.ImplementBase<TypeTemplate.TPrimary>();
			}
		}
	}
}
