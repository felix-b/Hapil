using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public abstract class ClassPerTestCaseFixtureBase
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private IHappilClassDefinition m_ClassDefinition;
		private TestFactory m_Factory;

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

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			try
			{
				m_Module.SaveAssembly();
			}
			catch ( Exception e )
			{
				Console.WriteLine("FAILED TO SAVE EMITTED ASSEMBLY: {0}", e.Message);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			m_Factory = null;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected IHappilClassBody<TBase> DeriveClassFrom<TBase>()
		{
			var currentTestName = TestContext.CurrentContext.Test.Name;
			m_ClassDefinition = m_Module.DeriveClassFrom<TBase>(m_Module.SimpleName + ".TestCase" + currentTestName);
			
			m_Class = ((IHappilClassDefinitionInternals)m_ClassDefinition).HappilClass;
			return ((IHappilClassBody<TBase>)m_ClassDefinition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal IConstructors<TBase> CreateClassInstanceAs<TBase>()
		{
			if ( m_Factory == null )
			{
				m_Factory = new TestFactory(m_Module, m_ClassDefinition);
				m_Factory.DefineClass();
			}

			return new Constructors<TBase>(m_Factory);
		}

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

		internal interface IConstructors<TBase>
		{
			TBase UsingDefaultConstructor();
			TBase UsingConstructor<T>(T arg);
			TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2);
			TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestFactory : HappilFactoryBase
		{
			private readonly IHappilClassDefinition m_ClassDefinition;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestFactory(HappilModule module, IHappilClassDefinition classDefinition)
				: base(module)
			{
				m_ClassDefinition = classDefinition;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineClass()
			{
				this.ClassTypeEntry = base.GetOrBuildType(new HappilTypeKey(m_ClassDefinition.BaseType));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TypeEntry ClassTypeEntry { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				return m_ClassDefinition;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Constructors<TBase> : IConstructors<TBase>
		{
			private readonly TestFactory m_Factory;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Constructors(TestFactory factory)
			{
				m_Factory = factory;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ICreateObjectSyntax<TBase> Members

			public TBase UsingDefaultConstructor()
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T>(T arg)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
