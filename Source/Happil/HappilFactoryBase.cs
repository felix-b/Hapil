using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	/// <summary>
	/// Base class for factories responsible for instantiation of objects of dynamically created types.
	/// </summary>
	/// <remarks>
	/// This base class should be subclassed for each different kind of responsibilities of dynamic types. 
	/// When subclassing, one should implement the <see cref="DefineNewClass"/> method. 
	/// In this method, a new type for the passed key should be created using Happil type factory instance provided by the <see cref="Module"/> property.
	/// The base class manages a read-through cache of built types. 
	/// The cache is backed by the <see cref="DefineNewClass"/> method, which it falls back to if the type of the requested object was not already built.
	/// </remarks>
	public abstract class HappilFactoryBase
	{
		private readonly HappilModule m_Module;
		private readonly ConcurrentDictionary<HappilTypeKey, TypeEntry> m_BuiltTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilFactoryBase(HappilModule module)
		{
			m_Module = module;
			m_BuiltTypes = new ConcurrentDictionary<HappilTypeKey, TypeEntry>(concurrencyLevel: 2, capacity: 512);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected TypeEntry GetOrBuildType(HappilTypeKey key)
		{
			return m_BuiltTypes.GetOrAdd(key, valueFactory: BuildNewTypeEntry);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilModule Module
		{
			get
			{
				return m_Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TypeEntry BuildNewTypeEntry(HappilTypeKey key)
		{
			using ( key.CreateTypeTemplateScope() )
			{
				var classDefinition = DefineNewClass(m_Module, key);
				return new TypeEntry((IHappilClassDefinitionInternals)classDefinition);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected class TypeEntry
		{
			internal TypeEntry(IHappilClassDefinitionInternals classDefinition)
			{
				this.DynamicType = classDefinition.HappilClass.CreateType();
				this.FactoryMethods = classDefinition.HappilClass.GetFactoryMethods();
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
