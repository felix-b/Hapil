using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture]
	public class UsingObjectFactoryBase
	{


		private class DemoObjectFactory : HappilObjectFactoryBase
		{
			public DemoObjectFactory(HappilFactory typeFactory)
				: base(typeFactory)
			{
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateObject<T>()
			{
				var key = new HappilTypeKey(baseType: typeof(object), primaryInterface: typeof(T));
				var type = GetOrBuildType(key);
				return type.CreateInstance<T>(factoryMethodIndex: 0);
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			protected override TypeEntry BuildNewType(HappilTypeKey key)
			{
				var fullName = "Happil.Demos.UsingObjectFactory.Impl" + key.PrimaryInterface.Name;
				
				var @class = TypeFactory.DefineClass(fullName).Implement(key.PrimaryInterface, 
					cls => cls.AutomaticProperties()
				);

				return new TypeEntry(@class);
			}
		}
	}
}
