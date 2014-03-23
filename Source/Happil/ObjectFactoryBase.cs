using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil
{
	public abstract class ObjectFactoryBase
	{
		private readonly DynamicModule m_Module;
		private readonly ConcurrentDictionary<TypeKey, TypeEntry> m_BuiltTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ObjectFactoryBase(DynamicModule module)
		{
			m_Module = module;
			m_BuiltTypes = new ConcurrentDictionary<TypeKey, TypeEntry>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DynamicModule Module
		{
			get { return m_Module; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected TypeEntry GetOrBuildType(TypeKey key)
		{
			return m_BuiltTypes.GetOrAdd(key, valueFactory: BuildNewTypeEntry);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract ClassType DefineNewClass(DynamicModule module, TypeKey key);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TypeEntry BuildNewTypeEntry(TypeKey key)
		{
			using ( key.CreateTypeTemplateScope() )
			{
				var classType = DefineNewClass(m_Module, key);
				return new TypeEntry(classType);
			}
		}
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected class TypeEntry
		{
			internal TypeEntry(ClassType classType)
			{
				this.DynamicType = classType.TypeBuilder.CreateType();
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

			public Type DynamicType { get; private set; }
			public Delegate[] FactoryMethods { get; private set; }
		}
	}
}
