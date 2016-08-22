using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hapil.Members;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class ObjectFactoryBaseTests
	{
		private DynamicModule m_Module;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			m_Module = new DynamicModule(
				simpleName: "Hapil.UnitTests.EmittedByObjectFactoryBaseTests",
				allowSave: true,
				saveDirectory: TestContext.CurrentContext.TestDirectory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			m_Module.SaveAssembly();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateEmptyObject()
		{
			//-- Arrange

			var factory = new EmptyObjectFactory(m_Module);

			//-- Act

			var obj = factory.CreateEmptyObject();

			//-- Assert

			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.GetType(), Is.Not.SameAs(typeof(object)));
			Assert.That(obj.GetType().Assembly.IsDynamic, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanImplementMarkerInterface()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			AncestorRepository.IMakerInterfaceOne one = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();

			//-- Assert

			Assert.That(one, Is.Not.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FactoryAlwaysCreatesNewInstances()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			var one1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			var one2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();

			//-- Assert

			Assert.That(one1, Is.Not.SameAs(one2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DynamicTypesAreReusedPerSameKey()
		{
			//-- Arrange

			var factory = new MarkerInterfaceFactory(m_Module);

			//-- Act

			var one1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			var one2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
			
			var two1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();
			var two2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();

			//-- Assert

			Assert.That(one1.GetType(), Is.SameAs(one2.GetType()));
			Assert.That(two1.GetType(), Is.SameAs(two2.GetType()));
			
			Assert.That(one1.GetType(), Is.Not.SameAs(two1.GetType()));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void DynamicTypeIsCreatedOnlyOnceDespiteConcurrentFirstUseRequests()
        {
            //-- Arrange

            var ready = new ManualResetEvent(initialState: false);
            var done = new ManualResetEvent(initialState: false);
            var defineNewClassCount = 0;

            var factory = new MarkerInterfaceFactory(
                m_Module, 
                onDefinedNewClass: key => {
                    ready.Set();
                    done.WaitOne();
                    Interlocked.Increment(ref defineNewClassCount);
                });

            AncestorRepository.IMakerInterfaceOne obj1 = null;
            AncestorRepository.IMakerInterfaceOne obj2 = null;

            //-- Act

            // first request is in progress and will wait for DONE event
            var task1 = Task.Factory.StartNew(() => {
                obj1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
            });

            ready.WaitOne();

            // second request must wait until first one is completed, and use ready built type
            var task2 = Task.Factory.StartNew(() => {
                obj2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
            });
            
            Thread.Sleep(1000);
            done.Set();

            Task.WaitAll(task1, task2);

            //-- Assert

            Assert.That(defineNewClassCount, Is.EqualTo(1));

            Assert.That(obj1, Is.Not.SameAs(obj2));
            Assert.That(obj1.GetType(), Is.SameAs(obj2.GetType()));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void OnClassTypeCreatedIsCalledOncePerNewType()
        {
            //-- Arrange

            var log = new List<ObjectFactoryBase.TypeEntry>();
            var factory = new MarkerInterfaceFactory(m_Module, (k, e) => log.Add(e));

            //-- Act

            var one1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
            var logOne1 = log.ToArray();
            var one2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceOne>();
            var logOne2 = log.ToArray();

            log.Clear();

            var two1 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();
            var logTwo1 = log.ToArray();
            var two2 = factory.CreateMarkerObject<AncestorRepository.IMakerInterfaceTwo>();
            var logTwo2 = log.ToArray();

            //-- Assert

            Assert.That(logOne1.Length, Is.EqualTo(1));
            Assert.That(typeof(AncestorRepository.IMakerInterfaceOne).IsAssignableFrom(logOne1[0].DynamicType));
            Assert.That(logOne2, Is.EqualTo(logOne1));

            Assert.That(logTwo1.Length, Is.EqualTo(1));
            Assert.That(typeof(AncestorRepository.IMakerInterfaceTwo).IsAssignableFrom(logTwo1[0].DynamicType));
            Assert.That(logTwo2, Is.EqualTo(logTwo1));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class EmptyObjectFactory : ObjectFactoryBase
		{
			public EmptyObjectFactory(DynamicModule module)
				: base(module)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public object CreateEmptyObject()
			{
				return base.GetOrBuildType(new TypeKey()).CreateInstance<object>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override ClassType DefineNewClass(TypeKey key)
			{
				return DeriveClassFrom<object>(key).DefaultConstructor();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MarkerInterfaceFactory : ObjectFactoryBase
		{
		    private readonly Action<TypeKey, TypeEntry> m_OnClassTypeCreated;
		    private readonly Action<TypeKey> m_OnDefinedNewClass;

		    //-------------------------------------------------------------------------------------------------------------------------------------------------

		    public MarkerInterfaceFactory(DynamicModule module, Action<TypeKey, TypeEntry> onClassTypeCreated = null, Action<TypeKey> onDefinedNewClass = null)
				: base(module)
		    {
		        m_OnClassTypeCreated = onClassTypeCreated;
		        m_OnDefinedNewClass = onDefinedNewClass;
		    }

		    //-------------------------------------------------------------------------------------------------------------------------------------------------

			public TMarker CreateMarkerObject<TMarker>()
			{
				var typeEntry = base.GetOrBuildType(new TypeKey(primaryInterface: typeof(TMarker)));
				return typeEntry.CreateInstance<TMarker>();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override ClassType DefineNewClass(TypeKey key)
			{
				var newClass = DeriveClassFrom<object>(key);
                
                newClass.DefaultConstructor();
                newClass.ImplementBase<TypeTemplate.TPrimary>();

                if (m_OnDefinedNewClass != null)
                {
                    m_OnDefinedNewClass(key);
                }

                return newClass;
			}

            //-------------------------------------------------------------------------------------------------------------------------------------------------

		    protected override void OnClassTypeCreated(TypeKey key, TypeEntry type)
		    {
		        if ( m_OnClassTypeCreated != null )
		        {
		            m_OnClassTypeCreated(key, type);
		        }
		    }
		}
	}
}
