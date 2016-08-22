using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil
{
	public abstract class ObjectFactoryBase
	{
        private readonly DynamicModule m_Module;
		private readonly AtomicDictionary<TypeKey, TypeEntry> m_BuiltTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ObjectFactoryBase(DynamicModule module)
		{
			m_Module = module;
            m_BuiltTypes = new AtomicDictionary<TypeKey, TypeEntry>();
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Type FindDynamicType(Type contractType, params Type[] secondaryInterfaceTypes)
        {
            return FindDynamicType(CreateTypeKey(contractType, secondaryInterfaceTypes));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Type FindDynamicType(TypeKey key)
        {
            ClassType typeInProgress;
            var typesBeingBuilt = s_TypesBeingBuilt;

            if ( typesBeingBuilt != null && typesBeingBuilt.TryGetValue(key, out typeInProgress) )
            {
                return typeInProgress.TypeBuilder;
            }
            else
            {
                return GetOrBuildType(key).DynamicType;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public virtual TypeKey CreateTypeKey(Type contractType, params Type[] secondaryInterfaceTypes)
        {
            return new TypeKey(
                primaryInterface: contractType,
                secondaryInterfaces: secondaryInterfaceTypes);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DynamicModule Module
		{
			get { return m_Module; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationClassWriter<TBase> DeriveClassFrom<TBase>(TypeKey key)
		{
			var classType = m_Module.DefineClass(typeof(TBase), key, key.SuggestClassName(this));
			return new ImplementationClassWriter<TBase>(classType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationClassWriter<TBase> DeriveClassFrom<TBase>(TypeKey key, string classFullName)
		{
			var classType = m_Module.DefineClass(typeof(TBase), key, classFullName);
			return new ImplementationClassWriter<TBase>(classType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationClassWriter<object> DeriveClassFrom(Type baseType, TypeKey key)
		{
			var classType = m_Module.DefineClass(baseType, key, key.SuggestClassName(this));
			return new ImplementationClassWriter<object>(classType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationClassWriter<object> DeriveClassFrom(Type baseType, TypeKey key, string classFullName)
		{
			var classType = m_Module.DefineClass(baseType, key, classFullName);
			return new ImplementationClassWriter<object>(classType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract ClassType DefineNewClass(TypeKey key);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal TypeEntry GetOrBuildType(TypeKey key)
		{
            return m_BuiltTypes.GetOrAdd(key, valueFactory: BuildNewTypeEntry);
            
            //if (!Monitor.TryEnter(_s_globalSyncRoot, 10000))
            //{
            //    throw new TimeoutException("ObjectFacotryBase.GetOrBuildType timed out waiting for exclusive lock (10 sec).");
            //}

            //try
            //{
            //    return m_BuiltTypes.GetOrAdd(key, valueFactory: BuildNewTypeEntry);
            //}
            //finally
            //{
            //    Monitor.Exit(_s_globalSyncRoot);
            //}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected virtual void OnClassTypeCreated(TypeKey key, TypeEntry type)
	    {
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal void NotifyClassTypeBeingBuilt(ClassType classType)
        {
            var typesBeingBuilt = s_TypesBeingBuilt;

            if ( typesBeingBuilt != null )
            {
                typesBeingBuilt.Add(classType.Key, classType);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TypeEntry BuildNewTypeEntry(TypeKey key)
		{
		    var ownTypesBeingBuilt = (s_TypesBeingBuilt == null);

		    if ( ownTypesBeingBuilt )
		    {
		        s_TypesBeingBuilt = new Dictionary<TypeKey, ClassType>();
		    }

		    try
		    {
		        using ( key.CreateTypeTemplateScope() )
		        {
		            var classType = DefineNewClass(key);
		            var entry = new TypeEntry(classType);

                    OnClassTypeCreated(key, entry);
		            
                    return entry;
		        }
		    }
		    finally
		    {
		        if ( ownTypesBeingBuilt )
		        {
		            s_TypesBeingBuilt = null;
		        }
		        else
		        {
                    s_TypesBeingBuilt.Remove(key);
		        }
		    }
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [ThreadStatic]
	    private static Dictionary<TypeKey, ClassType> s_TypesBeingBuilt;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private static readonly object _s_globalSyncRoot = new object();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IConstructors<out TBase>
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

		public class TypeEntry
		{
			internal TypeEntry(ClassType classType)
			{
				classType.Compile();
				
				this.DynamicType = classType.CompiledType;
				this.FactoryMethods = classType.GetFactoryMethods();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T>(int factoryMethodIndex = 0)
			{
				return (T)((Func<object>)FactoryMethods[factoryMethodIndex])();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1>(int factoryMethodIndex, TA1 arg1)
			{
				return (T)((Func<TA1, object>)FactoryMethods[factoryMethodIndex])(arg1);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2>(int factoryMethodIndex, TA1 arg1, TA2 arg2)
			{
				return (T)((Func<TA1, TA2, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3)
			{
				return (T)((Func<TA1, TA2, TA3, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4)
			{
				return (T)((Func<TA1, TA2, TA3, TA4, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4, TA5>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5)
			{
				return (T)((Func<TA1, TA2, TA3, TA4, TA5, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4, arg5);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4, TA5, TA6>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6)
			{
				return (T)((Func<TA1, TA2, TA3, TA4, TA5, TA6, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4, arg5, arg6);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4, TA5, TA6, TA7>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7)
			{
				return (T)((Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8)
			{
				return (T)((Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, object>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Type DynamicType { get; private set; }
			public Delegate[] FactoryMethods { get; private set; }
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected class Constructors<TBase> : IConstructors<TBase>
		{
			private readonly TypeEntry m_TypeEntry;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Constructors(TypeEntry typeEntry)
			{
				m_TypeEntry = typeEntry;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ICreateObjectSyntax<TBase> Members

			public TBase UsingDefaultConstructor(int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase>(constructorIndex);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T>(T arg, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T>(constructorIndex, arg);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2>(T1 arg1, T2 arg2, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2>(constructorIndex, arg1, arg2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3>(constructorIndex, arg1, arg2, arg3);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3, T4>(constructorIndex, arg1, arg2, arg3, arg4);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5>(constructorIndex, arg1, arg2, arg3, arg4, arg5);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6, T7>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TBase UsingConstructor<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int constructorIndex = 0)
			{
				return m_TypeEntry.CreateInstance<TBase, T1, T2, T3, T4, T5, T6, T7, T8>(constructorIndex, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			}

			#endregion
		}
	}
}
