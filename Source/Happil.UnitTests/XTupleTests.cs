using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class XTupleTests
	{
		private HappilModule m_Module;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule(
				"Happil.UnitTests.EmittedBy" + this.GetType().Name,
				allowSave: true,
				saveDirectory: TestContext.CurrentContext.TestDirectory);

			XTuple.FactoryInstance = new XTuple.Factory(m_Module);
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			m_Module.SaveAssembly();
			XTuple.FactoryInstance = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateTwoItemTuple()
		{
			//-- Act

			var tuple = XTuple.New<ITwoItemTuple>().Init(first: 123, second: "ABC");

			//-- Assert

			Assert.That(tuple, Is.Not.Null);
			Assert.That(tuple.First, Is.EqualTo(123));
			Assert.That(tuple.Second, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITwoItemTuple
		{
			ITwoItemTuple Init(int first, string second);
			int First { get; }
			string Second { get; }
		}
	}
}
