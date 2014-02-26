using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class XTupleFactoryTests
	{
		private HappilModule m_Module;
		private XTupleFactory m_Factory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule(
				"Happil.UnitTests.EmittedBy" + this.GetType().Name,
				allowSave: true,
				saveDirectory: TestContext.CurrentContext.TestDirectory);

			m_Factory = new XTupleFactory(m_Module);
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			m_Module.SaveAssembly();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateTwoItemTuple()
		{
			//-- Act

			var tuple = m_Factory.New<ITwoItemTuple>().Init(first: 123, second: "ABC");

			//-- Assert

			Assert.That(tuple, Is.Not.Null);
			Assert.That(tuple.First, Is.EqualTo(123));
			Assert.That(tuple.Second, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateThreeItemTuple()
		{
			//-- Act

			var tuple = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));

			//-- Assert

			Assert.That(tuple, Is.Not.Null);
			Assert.That(tuple.First, Is.EqualTo(123));
			Assert.That(tuple.Second, Is.EqualTo("ABC"));
			Assert.That(tuple.Third, Is.EqualTo(TimeSpan.FromMinutes(5)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateThreeItemTupleWithOverloadedInit()
		{
			//-- Act

			var tuple = m_Factory.New<IThreeItemTuple>().Init(first: 123, third: TimeSpan.FromMinutes(5));

			//-- Assert

			Assert.That(tuple, Is.Not.Null);
			Assert.That(tuple.First, Is.EqualTo(123));
			Assert.That(tuple.Second, Is.Null);
			Assert.That(tuple.Third, Is.EqualTo(TimeSpan.FromMinutes(5)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanGetHashCode()
		{
			//-- Arrange

			var tuple = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));
			var dotNetFxTuple = new Tuple<int, string, TimeSpan>(123, "ABC", TimeSpan.FromMinutes(5));
			
			//-- Act

			var hashCode = tuple.GetHashCode();

			//-- Assert

			Assert.That(hashCode, Is.EqualTo(dotNetFxTuple.GetHashCode()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanGetHashCodeWithNullItem()
		{
			//-- Arrange

			var tuple = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: null, third: TimeSpan.FromMinutes(5));
			var dotNetFxTuple = new Tuple<int, string, TimeSpan>(123, null, TimeSpan.FromMinutes(5));

			//-- Act

			var hashCode = tuple.GetHashCode();

			//-- Assert

			Assert.That(hashCode, Is.EqualTo(dotNetFxTuple.GetHashCode()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITwoItemTuple
		{
			ITwoItemTuple Init(int first, string second);
			int First { get; }
			string Second { get; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IThreeItemTuple
		{
			IThreeItemTuple Init(TimeSpan third, int first);
			IThreeItemTuple Init(int first, string second, TimeSpan third);
			int First { get; }
			string Second { get; }
			TimeSpan Third { get; }
		}
	}
}
