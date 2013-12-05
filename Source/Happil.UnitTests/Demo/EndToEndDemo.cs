using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture, Ignore("This is only a demo")]
	public class EndToEndDemo
	{
		private HappilFactory m_TypeFactory;
		private DemoObjectFactory m_ObjectFactory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_TypeFactory = new HappilFactory("UsingObjectFactoryBase.dll");
			m_ObjectFactory = new DemoObjectFactory(m_TypeFactory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CreateObjectByInterface()
		{
			IDemoInterface demoObj = m_ObjectFactory.CreateDemoObject<IDemoInterface>();

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

		private class DemoObjectFactory : HappilObjectFactoryBase
		{
			public DemoObjectFactory(HappilFactory typeFactory)
				: base(typeFactory)
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

			protected override TypeEntry BuildNewType(HappilTypeKey key)
			{
				var fullName = "Happil.Demos.UsingObjectFactoryBase.Impl" + key.PrimaryInterface.Name;
				
				var @class = TypeFactory.DefineClass(fullName).Implement(key.PrimaryInterface, 
					cls => cls.AutomaticProperties()
				);

				return new TypeEntry(@class);
			}
		}
	}
}
