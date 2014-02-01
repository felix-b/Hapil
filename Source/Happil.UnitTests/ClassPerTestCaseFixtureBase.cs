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
		public void BaseFixtureSetUp()
		{
			m_Module = new HappilModule(
				"Happil.UnitTests.EmittedBy" + this.GetType().Name,
				allowSave: this.ShouldSaveAssembly,
				saveDirectory: TestContext.CurrentContext.TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void BaseFixtureTearDown()
		{
			if ( ShouldSaveAssembly )
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
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void BaseSetUp()
		{
			m_Factory = null;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected IHappilClassBody<TBase> DeriveClassFrom<TBase>()
		{
			m_ClassDefinition = m_Module.DeriveClassFrom<TBase>(TestCaseClassName);
			m_Class = ((IHappilClassDefinitionInternals)m_ClassDefinition).HappilClass;
			
			return ((IHappilClassBody<TBase>)m_ClassDefinition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void OnDefineNewClass(Func<HappilTypeKey, IHappilClassDefinition> callback)
		{
			m_Factory = new TestFactory(m_Module, classDefinition: null, classDefinitionCallback: callback);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void ExpectException<TException>(Action codeUnderTest, out TException caughtException) where TException : Exception
		{
			try
			{
				codeUnderTest();
				caughtException = null;
			}
			catch ( TException e )
			{
				caughtException = e;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void ExpectException<TException>(Action codeUnderTest) where TException : Exception
		{
			try
			{
				codeUnderTest();
				Assert.Fail("Expected exception of type " + typeof(TException).Name);
			}
			catch ( TException )
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool ShouldSaveAssembly
		{
			get
			{
				return this.AllClassesAreCompleteTypes;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool AllClassesAreCompleteTypes
		{
			get
			{
				return true;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal IConstructors<TBase> CreateClassInstanceAs<TBase>()
		{
			if ( m_Factory == null )
			{
				m_Factory = new TestFactory(m_Module, m_ClassDefinition, classDefinitionCallback: null);
				m_Factory.DefineClass();
			}

			return new Constructors<TBase>(m_Factory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void DefineClassByKey(HappilTypeKey key)
		{
			m_Factory.DefineClassByKey(key);
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

		internal string TestCaseClassName
		{
			get
			{
				var currentTestName = TestContext.CurrentContext.Test.Name;
				return (m_Module.SimpleName + ".TestCase" + currentTestName);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal interface IConstructors<TBase>
		{
			TBase UsingDefaultConstructor(int constructorIndex = 0);
			TBase UsingConstructor<T>(T arg, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, int constructorIndex = 0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestFactory : HappilFactoryBase
		{
			private readonly IHappilClassDefinition m_ClassDefinition;
			private readonly Func<HappilTypeKey, IHappilClassDefinition> m_ClassDefinitionCallback;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestFactory(
				HappilModule module, 
				IHappilClassDefinition classDefinition, 
				Func<HappilTypeKey, IHappilClassDefinition> classDefinitionCallback)
				: base(module)
			{
				m_ClassDefinition = classDefinition;
				m_ClassDefinitionCallback = classDefinitionCallback;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineClass()
			{
				this.ClassTypeEntry = base.GetOrBuildType(new HappilTypeKey(m_ClassDefinition.BaseType));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineClassByKey(HappilTypeKey key)
			{
				this.ClassTypeEntry = base.GetOrBuildType(key);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TypeEntry ClassTypeEntry { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilTypeKey key)
			{
				if ( m_ClassDefinition != null )
				{
					return m_ClassDefinition;
				}
				else
				{
					return m_ClassDefinitionCallback(key);
				}
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

			public TBase UsingDefaultConstructor(int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase>(constructorIndex);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T>(T arg, int constructorIndex = 0)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2>(constructorIndex, arg1, arg2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, int constructorIndex = 0)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
