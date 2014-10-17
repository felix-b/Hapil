using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Fluent;
using NUnit.Framework;

namespace Hapil.UnitTests.Demo
{
	[TestFixture, Ignore("This is only a demo")]
	public class EndToEndDemo
	{
		private HappilModule m_Module;
		private DemoFactory m_Factory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_Module = new HappilModule("UsingFactoryBase");
			m_Factory = new DemoFactory(m_Module);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByInterface()
		{
			IDemoInterface demoObj = m_Factory.CreateDemoObject<IDemoInterface>();

			demoObj.Number = 123;
			demoObj.Text = demoObj.Number.ToString() + "ABC";
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IDemoInterface
		{
			int Number { get; set; }
			string Text { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DemoFactory : HappilFactoryBase
		{
			public DemoFactory(HappilModule module)
				: base(module)
			{
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			/// <summary>
			/// Each concrete factory is free to define arbitrary public methods according to the need.
			/// </summary>
			/// <typeparam name="T">
			/// In this example, it is the primary interface that will be implemented by the dynamic type.
			/// </typeparam>
			/// <returns>
			/// A new instance of dynamic type implementing specified interface.
			/// </returns>
			public T CreateDemoObject<T>()
			{
				var key = new HappilTypeKey(baseType: typeof(object), primaryInterface: typeof(T));
				var type = GetOrBuildType(key);
				return type.CreateInstance<T>();
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key)
			{
				return Module.DefineClass(key, namespaceName: "EndToEndDemo")
					.ImplementInterface(key.PrimaryInterface)
					.AllProperties().ImplementAutomatic();
			}
		}
	}
}
