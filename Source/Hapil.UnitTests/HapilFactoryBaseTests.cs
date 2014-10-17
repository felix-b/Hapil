using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Fluent;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	/// <summary>
	/// Contains tests on HappilTypeCache class
	/// </summary>
	[TestFixture]
	public class HappilFactoryBaseTests
	{
		private HappilModule m_Module;
		private TestFactory m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_Module = new HappilModule("HappilFactoryBaseTests");
			m_FactoryUnderTest = new TestFactory(m_Module);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByInterfaceType()
		{
			//-- Act

			ITestOne one = m_FactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByAnotherInterfaceType()
		{
			//-- Act

			ITestOne one = m_FactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CacheAndReuseGeneratedTypes()
		{
			//-- Act

			ITestOne one1 = m_FactoryUnderTest.CreateObject<ITestOne>();
			ITestOne one2 = m_FactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one1.GetType(), Is.SameAs(one2.GetType()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ObjectInstancesAreAlwaysNew()
		{
			//-- Act

			ITestOne one1 = m_FactoryUnderTest.CreateObject<ITestOne>();
			ITestOne one2 = m_FactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one1, Is.Not.SameAs(one2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestFactory : HappilFactoryBase
		{
			public TestFactory(HappilModule factory)
				: base(factory)
			{
			}
			
			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateObject<T>()
			{
				var key = new HappilTypeKey(primaryInterface: typeof(T));
				var type = base.GetOrBuildType(key);

				return type.CreateInstance<T>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key)
			{
				return Module.DefineClass(
					"HappilFactoryBaseTests.Impl" + key.PrimaryInterface.Name, key.BaseType)
					.ImplementInterface(key.PrimaryInterface)
					.DefaultConstructor();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestBase
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITestOne
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITestTwo
		{
		}
	}
}
