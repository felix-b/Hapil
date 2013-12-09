﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture]
	public class ImplementingMethodsDemo
	{
		public class DemoCalcFactory : HappilFactoryBase
		{
			public DemoCalcFactory(HappilModule module)
				: base(module)
			{
				TestFunc(() => Console.WriteLine("AAA"));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void TestFunc(Expression<Action> action)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				return Module.DefineClass(key, namespaceName: "ImplementingMethodsDemo")
					.Implement(key.PrimaryInterface)
					.Methods(m => {
						var methodName = m.Local<string>("methodName");
						methodName.Assign(m.MethodInfo.Name);
						m.EmitByExample(() => Console.WriteLine(methodName));
					});
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
