using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil;
using Happil.Members;
using Happil.Testing;
using Happil.Writers;
using NUnit.Framework;

namespace Hapil.Testing.NUnit
{
	[TestFixture]
	public abstract class NUnitEmittedTypesTestBase : EmittedTypesTestBase
	{
		[TestFixtureSetUp]
		public void BaseFixtureSetUp()
		{
			base.InitializeTestClass();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void BaseFixtureTearDown()
		{
			base.FinalizeTestClass();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void BaseSetUp()
		{
			base.InitializeTestCase();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void BaseTearDown()
		{
			base.FinalizeTestCase();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of EmittedTypeTestBase

		protected override void AssertStringContains(string s, string subString, string message)
		{
			StringAssert.Contains(subString, s, message);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void FailAssertion(string message)
		{
			Assert.Fail(message);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override string TestDirectory
		{
			get
			{
				return TestContext.CurrentContext.TestDirectory;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override string TestCaseName
		{
			get
			{
				return TestContext.CurrentContext.Test.Name;
			}
		}

		#endregion
	}
}
