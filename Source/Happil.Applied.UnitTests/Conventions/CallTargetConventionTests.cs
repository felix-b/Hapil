using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Happil.Applied.Conventions;
using Happil.Testing;
using NUnit.Framework;

namespace Happil.Applied.UnitTests.Conventions
{
	[TestFixture]
	public class CallTargetConventionTests : NUnitEmittedTypesTestBase
	{
		private ConventionObjectFactory m_Factory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_Factory = new ConventionObjectFactory(
				module: base.Module,
				transientConventionsFactory: context => new IObjectFactoryConvention[] {
					new TestNameConvention(this), 
					new CallTargetConvention(), 
				});
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DoNotOverrideSystemObjectMethods()
		{
			//-- Arrange

			var target = new TestTarget();

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);
			var implementedMethodNames = wrapper.GetType()
				.GetMethods()
				.Where(m => m.DeclaringType == wrapper.GetType())
				.Select(m => m.Name)
				.ToArray();

			//-- Assert

			CollectionAssert.DoesNotContain(implementedMethodNames, "Equals");
			CollectionAssert.DoesNotContain(implementedMethodNames, "Finalize");
			CollectionAssert.DoesNotContain(implementedMethodNames, "GetHashCode");
			CollectionAssert.DoesNotContain(implementedMethodNames, "ToString");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallVoidMethodOnTarget()
		{
			//-- Arrange

			var target = new TestTarget();

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);
			wrapper.One("ABC");

			//-- Assert

			Assert.That(target.Log, Is.EqualTo(new[] { "One(ABC)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCallFunctionOnTarget()
		{
			//-- Arrange

			var target = new TestTarget();

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);
			var result = wrapper.Two(123, "ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo("123|ABC"));
			Assert.That(target.Log, Is.EqualTo(new[] { "Two(123,ABC)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanGetAndSetPropertyOnTarget()
		{
			//-- Arrange

			var target = new TestTarget();
			target.StringValue = "DEF";
			target.Log.Clear();

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);
			
			var result1 = wrapper.StringValue;
			wrapper.StringValue = "GHI";
			var result2 = wrapper.StringValue;

			//-- Assert

			Assert.That(result1, Is.EqualTo("DEF"));
			Assert.That(result2, Is.EqualTo("GHI"));
			Assert.That(target.Log, Is.EqualTo(new[] { "StringValue.Get", "StringValue.Set(GHI)", "StringValue.Get" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanGetAndSetIndexerOnTarget()
		{
			//-- Arrange

			var target = new TestTarget();
			target[123] = "ABC";
			target.Log.Clear();

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);

			var result1 = wrapper[123];
			wrapper[456] = "DEF";
			var result2 = wrapper[456];

			//-- Assert

			Assert.That(result1, Is.EqualTo("ABC"));
			Assert.That(result2, Is.EqualTo("DEF"));
			Assert.That(target.Log, Is.EqualTo(new[] { "This.Get[123]", "This.Set([456]=DEF)", "This.Get[456]" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanAddEventHandlerToTarget()
		{
			//-- Arrange

			var target = new TestTarget();
			var eventCount = 0;
			string eventValue = null;

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);
			wrapper.EventOne += (sender, args) => {
				eventCount++;
				eventValue = args.Value;
			};

			target.RaiseEventOne("ZZZ");

			//-- Assert

			Assert.That(eventCount, Is.EqualTo(1));
			Assert.That(eventValue, Is.EqualTo("ZZZ"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanRemoveEventHandlerFromTarget()
		{
			//-- Arrange

			var target = new TestTarget();
			var eventCount = 0;
			string eventValue = null;

			var eventHandler = new EventHandler<TestTargetEventArgs>((sender, args) => {
				eventCount++;
				eventValue = args.Value;
			});

			target.EventOne += eventHandler;

			//-- Act

			var wrapper = m_Factory.CreateInstanceOf<ITestTarget>().UsingConstructor(target);

			target.RaiseEventOne("YYY");
			wrapper.EventOne -= eventHandler;
			target.RaiseEventOne("ZZZ");

			//-- Assert

			Assert.That(eventCount, Is.EqualTo(1));
			Assert.That(eventValue, Is.EqualTo("YYY"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITestTarget
		{
			void One(string s);
			string Two(int n, string s);
			string StringValue { get; set; }
			string this[int number] { get; set; }
			event EventHandler<TestTargetEventArgs> EventOne;
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class TestTargetEventArgs : EventArgs
		{
			public string Value { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestTarget : ITestTarget
		{
			private string m_StringValue;
			private Dictionary<int, string> m_Index;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestTarget()
			{
				Log = new List<string>();
				m_Index = new Dictionary<int, string>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void One(string s)
			{
				Log.Add(string.Format("One({0})", s));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string Two(int n, string s)
			{
				Log.Add(string.Format("Two({0},{1})", n, s));
				return n.ToString() + "|" + s;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string StringValue
			{
				get
				{
					Log.Add("StringValue.Get");
					return m_StringValue;
				}
				set
				{
					Log.Add(string.Format("StringValue.Set({0})", value));
					m_StringValue = value;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string this[int number]
			{
				get
				{
					Log.Add(string.Format("This.Get[{0}]", number));
					return m_Index[number];
				}
				set
				{
					Log.Add(string.Format("This.Set([{0}]={1})", number, value));
					m_Index[number] = value;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void RaiseEventOne(string value)
			{
				if ( EventOne != null )
				{
					EventOne(this, new TestTargetEventArgs() { Value = value });
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public event EventHandler<TestTargetEventArgs> EventOne;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public List<string> Log { get; private set; }
		}
	}
}
