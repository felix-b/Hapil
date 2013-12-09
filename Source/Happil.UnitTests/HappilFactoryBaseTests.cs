using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests
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

				//TODO: should be able to replace the following line with 'return type.CreateInstance<T>();'
				//this depends on implementation of the factory methods.
				return (T)type.CreateInstance<object>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				return Module.DefineClass(
					"HappilFactoryBaseTests.Impl" + key.PrimaryInterface.Name)
					.Inherit(key.BaseType)
					.Implement(key.PrimaryInterface);
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
