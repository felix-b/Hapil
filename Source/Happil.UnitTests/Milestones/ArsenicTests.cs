using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Milestones
{
	[TestFixture]
	public class ArsenicTests
	{
		private const string DynamicAssemblyName = "Happil.EmittedTypes.ArsenicTests";

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TextWriter m_SaveConsoleOut;
		private StringWriter m_TestConsoleOut;
		private HappilModule m_Module;
		private ArsenicTestFactory m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			m_Module = new HappilModule(DynamicAssemblyName, allowSave: true);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			m_Module.SaveAssembly();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_FactoryUnderTest = new ArsenicTestFactory(m_Module);

			m_SaveConsoleOut = Console.Out;
			m_TestConsoleOut = new StringWriter();
			Console.SetOut(m_TestConsoleOut);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			Console.SetOut(m_SaveConsoleOut);
			m_TestConsoleOut.Dispose();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanImplementInterfaceWithVoidParameterlessMethods()
		{
			//-- Act

			IArsenicTestInterface obj = m_FactoryUnderTest.CreateObject<IArsenicTestInterface>();

			obj.First();
			obj.Second();
			obj.Third();

			//-- Assert

			Assert.That(obj.GetType().IsClass);
			Assert.That(obj.GetType().BaseType, Is.SameAs(typeof(object)));
			Assert.That(obj.GetType().Assembly.IsDynamic);
			Assert.That(obj.GetType().Assembly.GetName().Name, Is.EqualTo(DynamicAssemblyName));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not yet implemented")]
		public void CanEmitVoidMethodsThatPrintTheirNameToConsole()
		{
			//-- Act

			var obj = m_FactoryUnderTest.CreateObject<IArsenicTestInterface>();

			obj.First();
			obj.Second();
			obj.Third();

			//-- Assert

			string expectedOutput =
				"First" + Environment.NewLine +
				"Second" + Environment.NewLine +
				"Third" + Environment.NewLine;

			Assert.That(m_TestConsoleOut.ToString(), Is.EqualTo(expectedOutput));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IArsenicTestInterface
		{
			void First();
			void Second();
			void Third();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ArsenicTestFactory : HappilFactoryBase
		{
			public ArsenicTestFactory(HappilModule module)
				: base(module)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateObject<T>()
			{
				var key = new HappilTypeKey(primaryInterface: typeof(T));
				var type = base.GetOrBuildType(key);

				//TODO: replace the following line with 'return type.CreateInstance<T>()'
				return (T)type.CreateInstance<object>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				return Module.DefineClass("ArsenicTests.Impl" + key.PrimaryInterface.Name)
					.Implement(key.PrimaryInterface)
					.Methods(m => {
						m.EmitByExample(() => Console.WriteLine(m.MethodInfo.Name));
					});
			}
		}
	}
}
