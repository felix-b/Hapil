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
	public class HappilObjectFactoryBaseTests
	{
		private HappilFactory m_TypeFactory;
		private TestObjectFactory m_ObjectFactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_TypeFactory = new HappilFactory("HappilObjectFactoryBaseTests.dll");
			m_ObjectFactoryUnderTest = new TestObjectFactory(m_TypeFactory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByInterfaceType()
		{
			//-- Act

			ITestOne one = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByAnotherInterfaceType()
		{
			//-- Act

			ITestOne one = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CacheAndReuseGeneratedTypes()
		{
			//-- Act

			ITestOne one1 = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();
			ITestOne one2 = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one1.GetType(), Is.SameAs(one2.GetType()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ObjectInstancesAreAlwaysNew()
		{
			//-- Act

			ITestOne one1 = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();
			ITestOne one2 = m_ObjectFactoryUnderTest.CreateObject<ITestOne>();

			//-- Assert

			Assert.That(one1, Is.Not.SameAs(one2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestObjectFactory : HappilObjectFactoryBase
		{
			public TestObjectFactory(HappilFactory factory)
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
				return TypeFactory.DefineClass(
					"HappilObjectFactoryBaseTests.Impl" + key.PrimaryInterface.Name)
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
