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
	public class ArsenicTests : ClassPerTestCaseFixtureBase
	{
		private const string DynamicAssemblyName = "Happil.UnitTests.EmittedByArsenicTests";

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TextWriter m_SaveConsoleOut;
		private StringWriter m_TestConsoleOut;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
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
			//-- Arrange

			DeriveClassFrom<object>()
				.Implement<IArsenicTestInterface>()
				.DefaultConstructor()
				.Methods(m => {
					m.EmitFromLambda(() => Console.WriteLine(m.MethodInfo.Name));
				});

			//-- Act

			IArsenicTestInterface obj = base.CreateClassInstanceAs<IArsenicTestInterface>().UsingDefaultConstructor();

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

		[Test]
		public void CanEmitVoidMethodsThatPrintTheirNameToConsole()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.Implement<IArsenicTestInterface>()
				.DefaultConstructor()
				.Methods(m => {
					m.EmitFromLambda(() => Console.WriteLine(m.MethodInfo.Name));
				});

			//-- Act

			IArsenicTestInterface obj = base.CreateClassInstanceAs<IArsenicTestInterface>().UsingDefaultConstructor();

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
	}
}
