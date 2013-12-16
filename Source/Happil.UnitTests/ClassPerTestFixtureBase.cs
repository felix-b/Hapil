using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public abstract class ClassPerTestFixtureBase<TBase>
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private IHappilClassBody<TBase> m_ClassBody;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule(
				"Happil.UnitTests.EmittedBy" + this.GetType().Name,
				allowSave: true,
				saveDirectory: TestContext.CurrentContext.TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			var currentTestName = TestContext.CurrentContext.Test.Name;
			m_ClassBody = m_Module.DeriveClassFrom<TBase>(m_Module.SimpleName + ".TestCase" + currentTestName);
			m_Class = ((IHappilClassDefinitionInternals)m_ClassBody).HappilClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			m_Module.SaveAssembly();
		}
		
		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract IHappilClassDefinition DefineClass();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilModule Module
		{
			get
			{
				return m_Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilClass Class
		{
			get
			{
				return m_Class;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal IHappilClassBody<TBase> ClassBody
		{
			get
			{
				return m_ClassBody;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Factory : HappilFactoryBase
		{
			private readonly ClassPerTestFixtureBase<TBase> m_Owner;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Factory(ClassPerTestFixtureBase<TBase> owner)
				: base(owner.Module)
			{
				m_Owner = owner;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				var currentTestName = TestContext.CurrentContext.Test.Name;
				
				m_Owner.m_ClassBody = Module.DeriveClassFrom<TBase>(Module.SimpleName + ".TestCase" + currentTestName);
				m_Owner.m_Class = ((IHappilClassDefinitionInternals)m_Owner.m_ClassBody).HappilClass;

				return null;
			}
		}
	}
}
