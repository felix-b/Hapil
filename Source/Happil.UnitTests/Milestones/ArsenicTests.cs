using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Milestones
{
	[TestFixture]
	public class ArsenicTests
	{
		private TextWriter m_SaveConsoleOut;
		private StringWriter m_TestConsoleOut;
		private HappilFactory m_TypeFactory;
		private ArsenicTestFactory m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_SaveConsoleOut = Console.Out;
			m_TestConsoleOut = new StringWriter();
			Console.SetOut(m_TestConsoleOut);

			m_TypeFactory = new HappilFactory("ArsenicTests");
			m_FactoryUnderTest = new ArsenicTestFactory(m_TypeFactory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			Console.SetOut(m_SaveConsoleOut);
			m_TestConsoleOut.Dispose();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not Yet Implemented")]
		public void CanImplementInterfaceWithVoidParameterlessMethods()
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

		private class ArsenicTestFactory : HappilObjectFactoryBase
		{
			public ArsenicTestFactory(HappilFactory typeFactory)
				: base(typeFactory)
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

			protected override TypeEntry BuildNewType(HappilTypeKey key)
			{
				var classDefinition = base.TypeFactory.DefineClass("ArsenicTests.Impl" + key.PrimaryInterface.Name);

				classDefinition.Implement(key.PrimaryInterface, 
					cls => cls.Methods(m => {
						m.EmitByExample(() => Console.WriteLine(m.MethodInfo.Name));
					})
				);

				return new TypeEntry(classDefinition);
			}
		}
	}
}
