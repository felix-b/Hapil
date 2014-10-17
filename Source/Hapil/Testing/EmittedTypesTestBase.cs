using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil.Testing
{
	public abstract class EmittedTypesTestBase
	{
		private DynamicModule m_Module;
		private ClassType m_Class;
		private TestFactory m_Factory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void InitializeTestClass()
		{
			m_Module = new DynamicModule(
				"EmittedBy" + this.GetType().Name,
				allowSave: this.ShouldSaveAssembly,
				saveDirectory: TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void FinalizeTestClass()
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

		protected void InitializeTestCase()
		{
			m_Factory = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void FinalizeTestCase()
		{
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationClassWriter<TBase> DeriveClassFrom<TBase>(TypeKey key = null)
		{
			m_Class = m_Module.DefineClass(baseType: typeof(TBase), key: null, classFullName: TestCaseClassName);
			return new ImplementationClassWriter<TBase>(m_Class);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void OnDefineNewClass(Func<TypeKey, ClassWriterBase> callback)
		{
			m_Factory = CreateTestFactory(m_Module, classType: null, classDefinitionCallback: callback);
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

		protected void ExpectException<TException>(Action codeUnderTest, string messageContains) where TException : Exception
		{
			try
			{
				codeUnderTest();
				FailAssertion("Expected exception of type " + typeof(TException).Name);
			}
			catch ( TException e )
			{
				AssertStringContains(e.Message, messageContains, "Exception message was not as expected");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void ExpectException<TException>(Action codeUnderTest) where TException : Exception
		{
			try
			{
				codeUnderTest();
				FailAssertion("Expected exception of type " + typeof(TException).Name);
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

		protected virtual TestFactory CreateTestFactory(
			DynamicModule module,
			ClassType classType,
			Func<TypeKey, ClassWriterBase> classDefinitionCallback)
		{
			return new TestFactory(module, classType, classDefinitionCallback);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void AssertStringContains(string s, string subString, string message);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void FailAssertion(string message);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract string TestDirectory { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract string TestCaseName { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected ObjectFactoryBase.IConstructors<TBase> CreateClassInstanceAs<TBase>()
		{
			if ( m_Factory == null )
			{
				m_Factory = CreateTestFactory(m_Module, m_Class, classDefinitionCallback: null);
				m_Factory.DefineClass();
			}

			return new ObjectFactoryBase.Constructors<TBase>(m_Factory.ClassTypeEntry);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void DefineClassByKey(TypeKey key)
		{
			m_Factory.DefineClassByKey(key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected DynamicModule Module
		{
			get
			{
				return m_Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected ClassType Class
		{
			get
			{
				return m_Class;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected string TestCaseClassName
		{
			get
			{
				var currentTestName = TestCaseName;
				return (m_Module.SimpleName + ".TestCase" + currentTestName);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected class TestFactory : ObjectFactoryBase
		{
			private readonly ClassType m_ClassType;
			private readonly Func<TypeKey, ClassWriterBase> m_ClassDefinitionCallback;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestFactory(
				DynamicModule module, 
				ClassType classType, 
				Func<TypeKey, ClassWriterBase> classDefinitionCallback)
				: base(module)
			{
				m_ClassType = classType;
				m_ClassDefinitionCallback = classDefinitionCallback;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineClass()
			{
				this.ClassTypeEntry = base.GetOrBuildType(new TypeKey(m_ClassType.BaseType));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineClassByKey(TypeKey key)
			{
				this.ClassTypeEntry = base.GetOrBuildType(key);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TypeEntry ClassTypeEntry { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override ClassType DefineNewClass(TypeKey key)
			{
				if ( m_ClassType != null )
				{
					return m_ClassType;
				}
				else
				{
					return m_ClassDefinitionCallback(key).OwnerClass;
				}
			}
		}
	}
}
