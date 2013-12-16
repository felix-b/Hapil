using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class StatementTests
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private IHappilClassBody<TestBaseOne> m_ClassBody;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule(
				"HappilOperandTests", 
				allowSave: true, 
				saveDirectory: TestContext.CurrentContext.TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			var currentTestName = TestContext.CurrentContext.Test.Name;
			m_ClassBody = m_Module.DeriveClassFrom<TestBaseOne>("Fluent.StatementTests.TestCase" + currentTestName);
			m_Class = ((IHappilClassDefinitionInternals)m_ClassBody).HappilClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			m_Module.SaveAssembly();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		[Test]
		public void AssignFieldToField()
		{
			//-- Arrange

			var field1 = m_ClassBody.Field<int>("f1");
			var field2 = m_ClassBody.Field<int>("f2");

			//-- Act

			m_ClassBody.Method(cls => cls.VoidVirtualMethod, m => {
				field1.Assign(field2);
			});

			m_Class.CreateType();

			//-- Assert

			var method = m_Class.FindMember<HappilMethod>("VoidVirtualMethod");

			Assert.That(
				method.ToString(), 
				Is.EqualTo("{Expr<Int32>{Field{f1} = Field{f2}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignConstToField()
		{
			//-- Arrange

			var field1 = m_ClassBody.Field<int>("f1");
			var const1 = new HappilConstant<int>(123);

			//-- Act

			m_ClassBody.Method(cls => cls.VoidVirtualMethod, m => {
				field1.Assign(const1);
			});

			m_Class.CreateType();

			//-- Assert

			var method = m_Class.FindMember<HappilMethod>("VoidVirtualMethod");

			Assert.That(
				method.ToString(),
				Is.EqualTo("{Expr<Int32>{Field{f1} = Const<Int32>{123}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignExpressionToField()
		{
			//-- Arrange

			var field1 = m_ClassBody.Field<int>("f1");
			var field2 = m_ClassBody.Field<int>("f2");

			//-- Act

			m_ClassBody.Method(cls => cls.VoidVirtualMethod, m => {
				field1.Assign(field2 + 123);
			});

			m_Class.CreateType();

			//-- Assert

			var method = m_Class.FindMember<HappilMethod>("VoidVirtualMethod");

			Assert.That(
				method.ToString(),
				Is.EqualTo("{Expr<Int32>{Field{f1} = Expr<Int32>{Field{f2} + Const<Int32>{123}}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne1 : TestBaseOne
		{
			private int f1;

			#region Overrides of TestBaseOne

			public override void VoidVirtualMethod()
			{
				f1 = 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne2 : TestBaseOne
		{
			private int f1 = 123;
			private int f2 = 456;

			#region Overrides of TestBaseOne

			public override void VoidVirtualMethod()
			{
				f1 = f2 + 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
			public int F2
			{
				get
				{
					return f2;
				}
			}
		}
	}
}
