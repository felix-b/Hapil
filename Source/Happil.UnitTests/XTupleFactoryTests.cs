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

		[Test]
		public void CanTestForEquality()
		{
			//-- Arrange

			var tuple1 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple2 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple3 = m_Factory.New<IThreeItemTuple>().Init(first: 456, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple4 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "DEF", third: TimeSpan.FromMinutes(5));
			var tuple5 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(10));

			//-- Act & Assert

			Assert.That(tuple1.Equals(tuple1), Is.True);
			
			Assert.That(tuple1.Equals(tuple2), Is.True);
			Assert.That(tuple2.GetHashCode(), Is.EqualTo(tuple1.GetHashCode()));
	
			Assert.That(tuple1.Equals(tuple3), Is.False);
			Assert.That(tuple1.Equals(tuple4), Is.False);
			Assert.That(tuple1.Equals(tuple5), Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCompare()
		{
			//-- Arrange

			var tuple1 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple2 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple3 = m_Factory.New<IThreeItemTuple>().Init(first: 456, second: "ABC", third: TimeSpan.FromMinutes(5));
			var tuple4 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "DEF", third: TimeSpan.FromMinutes(5));
			var tuple5 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(10));

			//-- Act & Assert

			Assert.That(tuple1.CompareTo(tuple1), Is.EqualTo(0));
			Assert.That(tuple1.CompareTo(tuple2), Is.EqualTo(0));

			Assert.That(tuple1.CompareTo(tuple3), Is.LessThan(0));
			Assert.That(tuple3.CompareTo(tuple1), Is.GreaterThan(0));

			Assert.That(tuple1.CompareTo(tuple4), Is.LessThan(0));
			Assert.That(tuple4.CompareTo(tuple1), Is.GreaterThan(0));

			Assert.That(tuple1.CompareTo(tuple5), Is.LessThan(0));
			Assert.That(tuple5.CompareTo(tuple1), Is.GreaterThan(0));

			Assert.That(tuple3.CompareTo(tuple4), Is.GreaterThan(0));
			Assert.That(tuple4.CompareTo(tuple3), Is.LessThan(0));
			
			Assert.That(tuple4.CompareTo(tuple5), Is.GreaterThan(0));
			Assert.That(tuple5.CompareTo(tuple4), Is.LessThan(0));

			Assert.That(tuple3.CompareTo(tuple5), Is.GreaterThan(0));
			Assert.That(tuple5.CompareTo(tuple3), Is.LessThan(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCompareWithIncompatibleObject()
		{
			//-- Arrange

			var tuple1 = m_Factory.New<ITwoItemTuple>().Init(first: 123, second: "ABC");
			var tuple2 = m_Factory.New<IThreeItemTuple>().Init(first: 123, second: "ABC", third: TimeSpan.FromMinutes(5));

			//-- Act 

			var result = tuple2.CompareTo(tuple1);

			//-- Assert

			Assert.That(result, Is.EqualTo(1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanUseAsKeyInDictionary()
		{
			//-- Arrange

			var dictionary = new Dictionary<ITwoItemTuple, string>() {
				{ m_Factory.New<ITwoItemTuple>().Init(123, "ABC"), "AAA" },
				{ m_Factory.New<ITwoItemTuple>().Init(456, "DEF"), "DDD" },
			};

			//-- Act

			var value1 = dictionary[m_Factory.New<ITwoItemTuple>().Init(123, "ABC")];
			var value2 = dictionary[m_Factory.New<ITwoItemTuple>().Init(456, "DEF")];
			var conatinsKey1 = dictionary.ContainsKey(m_Factory.New<ITwoItemTuple>().Init(123, "ABC"));
			var conatinsKey2 = dictionary.ContainsKey(m_Factory.New<ITwoItemTuple>().Init(789, "GHI"));

			//-- Assert

			Assert.That(value1, Is.EqualTo("AAA"));
			Assert.That(value2, Is.EqualTo("DDD"));
			Assert.That(conatinsKey1, Is.True);
			Assert.That(conatinsKey2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanSortItemsInList()
		{
			//-- Arrange

			var list = new List<ITwoItemTuple>() {
				{ m_Factory.New<ITwoItemTuple>().Init(10, "A") },
				{ m_Factory.New<ITwoItemTuple>().Init(5, "B") },
				{ m_Factory.New<ITwoItemTuple>().Init(5, "A") },
				{ m_Factory.New<ITwoItemTuple>().Init(10, "B") }
			};

			//-- Act

			list.Sort();

			//-- Assert

			Assert.That(list, Is.EqualTo(new[] {
				m_Factory.New<ITwoItemTuple>().Init(5, "A"),
				m_Factory.New<ITwoItemTuple>().Init(5, "B"),
				m_Factory.New<ITwoItemTuple>().Init(10, "A"),
				m_Factory.New<ITwoItemTuple>().Init(10, "B")
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITwoItemTuple
		{
			ITwoItemTuple Init(int first, string second);
			int First { get; }
			string Second { get; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IThreeItemTuple : IComparable
		{
			IThreeItemTuple Init(TimeSpan third, int first);
			IThreeItemTuple Init(int first, string second, TimeSpan third);
			int First { get; }
			string Second { get; }
			TimeSpan Third { get; }
		}
	}
}
