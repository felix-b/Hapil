using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture]
	public class ImplementingMethodsDemo
	{
		public class DemoCalcObjectFactory : HappilObjectFactoryBase
		{
			public DemoCalcObjectFactory(HappilFactory typeFactory)
				: base(typeFactory)
			{
				TestFunc(() => Console.WriteLine("AAA"));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void TestFunc(Expression<Action> action)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override TypeEntry BuildNewType(HappilTypeKey key)
			{
				var classDefinition = TypeFactory.DefineClass("ImplementingMethodsDemo.Impl" + key.PrimaryInterface.Name).Implement(key.PrimaryInterface, 
					cls => cls.Methods(m => {
						var methodName = m.Local<string>("methodName");
						methodName.Assign(m.MethodInfo.Name);
						m.EmitByExample(() => Console.WriteLine(methodName));
					})
				);

				return null;
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IDemoPrint1
		{
			void Method1();
			void Method2();
			void Method3();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IDemoCalc
		{
			int Add(int x, int y);
			int Subtract(int x, int y);
			long Multiply(int x, int y);
			double Divide(int x, int y);
		}
	}
}
