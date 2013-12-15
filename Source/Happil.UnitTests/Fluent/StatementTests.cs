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
			m_Module = new HappilModule("HappilOperandTests");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_ClassBody = m_Module.DeriveClassFrom<TestBaseOne>("_" + Guid.NewGuid().ToString("N"));
			m_Class = ((IHappilClassDefinitionInternals)m_ClassBody).HappilClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		[Test]
		public void AssignStatement()
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
	}
}
