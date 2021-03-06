﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Fluent;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class HappilFactoryTests
	{
		private HappilModule m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_FactoryUnderTest = new HappilModule("Hapil.Demo.Impl");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateTypeAndInstanceOfSampleClass()
		{
			//-- Arrange

			const string typeFullName = "Hapil.Demo.SampleClass";

			//-- Act

			var classBody = m_FactoryUnderTest.DefineClass(typeFullName);
			var type = ((IHappilClassDefinitionInternals)classBody).HappilClass.CreateType();
			var instance = Assembly.GetAssembly(type).CreateInstance(type.FullName);

			//-- Assert

			Assert.That(type.FullName, Is.EqualTo(typeFullName));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance.GetType().FullName, Is.EqualTo(typeFullName));
		}
	}
}
