using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public abstract class ClassPerTestCaseFixtureBase
	{
		private DynamicModule m_Module;
		private ClassType m_Class;
		private TestFactory m_Factory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void BaseFixtureSetUp()
		{
			m_Module = new DynamicModule(
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
				Assert.Fail("Expected exception of type " + typeof(TException).Name);
			}
			catch ( TException e )
			{
				StringAssert.Contains(messageContains, e.Message, "Exception message was not as expected");
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

		protected virtual TestFactory CreateTestFactory(
			DynamicModule module,
			ClassType classType,
			Func<TypeKey, ClassWriterBase> classDefinitionCallback)
		{
			return new TestFactory(module, classType, classDefinitionCallback);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal IConstructors<TBase> CreateClassInstanceAs<TBase>()
		{
			if ( m_Factory == null )
			{
				m_Factory = CreateTestFactory(m_Module, m_Class, classDefinitionCallback: null);
				m_Factory.DefineClass();
			}

			return new Constructors<TBase>(m_Factory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void DefineClassByKey(TypeKey key)
		{
			m_Factory.DefineClassByKey(key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal DynamicModule Module
		{
			get
			{
				return m_Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ClassType Class
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
			TBase UsingConstructor<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int constructorIndex = 0);
			TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int constructorIndex = 0);
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

			internal TypeEntry ClassTypeEntry { get; private set; }

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
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T>(constructorIndex, arg);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2>(constructorIndex, arg1, arg2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3>(constructorIndex, arg1, arg2, arg3);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3, T4>(constructorIndex, arg1, arg2, arg3, arg4);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5>(constructorIndex, arg1, arg2, arg3, arg4, arg5);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6, T7>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int constructorIndex = 0)
			{
				return m_Factory.ClassTypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6, T7, T8>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			}

			#endregion
		}
	}
}
