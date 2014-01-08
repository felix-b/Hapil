using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests.Fluent
{
	[TestFixture]
	public class InvocationTests
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private IHappilClassBody<AncestorRepository.BaseOne> m_ClassBody;

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
			m_ClassBody = m_Module.DeriveClassFrom<AncestorRepository.BaseOne>("_" + Guid.NewGuid().ToString("X"));
			m_Class = ((IHappilClassDefinitionInternals)m_ClassBody).HappilClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not yet implemented")]
		public void InvokeVoidMethodPassesCompilation()
		{
			//-- Arrange

			var field1 = m_ClassBody.Field<AncestorRepository.BaseOne>("m_Next");

			//-- Act

			field1.Method(x => x.VoidMethod);
			field1.Method(x => x.VoidMethodWithOneArg, new HappilConstant<int>(123));
			field1.Method(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<string>("ABC"));
			field1.Method(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<DateTime>(DateTime.Now));
			field1.Method(x => x.VoidMethodWithManyArgs, new HappilConstant<int>(123), new HappilConstant<string>("ABC"), new HappilConstant<DateTime>(DateTime.Now));
		}
	}
}
