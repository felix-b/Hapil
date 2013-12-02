using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class HappilFactoryTests
	{
		private HappilFactory m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_FactoryUnderTest = new HappilFactory("Happil.Demo.Impl.dll");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateTypeAndInstanceOfSampleClass()
		{
			//-- Arrange

			const string typeFullName = "Happil.Demo.SampleClass";

			//-- Act

			Type type = m_FactoryUnderTest.DefineClass(typeFullName).CreateType();
			var instance = Assembly.GetAssembly(type).CreateInstance(type.FullName);

			//-- Assert

			Assert.That(type.FullName, Is.EqualTo(typeFullName));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance.GetType().FullName, Is.EqualTo(typeFullName));
		}
	}
}
