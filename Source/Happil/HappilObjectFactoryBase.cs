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
	/// When subclassing, one should implement the <see cref="BuildNewType"/> method. 
	/// In this method, a new type for the passed key should be created using Happil type factory instance provided by the <see cref="TypeFactory"/> property.
	/// The base class manages a read-through cache of built types. 
	/// The cache is backed by the <see cref="BuildNewType"/> method, which it falls back to if the type of the requested object was not already built.
	/// </remarks>
	public abstract class HappilObjectFactoryBase
	{
		private readonly HappilFactory m_TypeFactory;
		private readonly ConcurrentDictionary<HappilTypeKey, TypeEntry> m_BuiltTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilObjectFactoryBase(HappilFactory typeFactory)
		{
			m_TypeFactory = typeFactory;
			m_BuiltTypes = new ConcurrentDictionary<HappilTypeKey, TypeEntry>(concurrencyLevel: 2, capacity: 512);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected TypeEntry GetOrBuildType(HappilTypeKey key)
		{
			return m_BuiltTypes.GetOrAdd(key, valueFactory: BuildNewType);
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract TypeEntry BuildNewType(HappilTypeKey key);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilFactory TypeFactory
		{
			get
			{
				return m_TypeFactory;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected class TypeEntry
		{
			public TypeEntry(HappilClass classDefinition)
			{
				this.DynamicType = classDefinition.CreateType();
				this.FactoryMethods = classDefinition.GetFactoryMethods();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T>(int factoryMethodIndex = 0)
			{
				return ((Func<T>)FactoryMethods[factoryMethodIndex])();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1>(int factoryMethodIndex, TA1 arg1)
			{
				return ((Func<TA1, T>)FactoryMethods[factoryMethodIndex])(arg1);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2>(int factoryMethodIndex, TA1 arg1, TA2 arg2)
			{
				return ((Func<TA1, TA2, T>)FactoryMethods[factoryMethodIndex])(arg1, arg2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3)
			{
				return ((Func<TA1, TA2, TA3, T>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4)
			{
				return ((Func<TA1, TA2, TA3, TA4, T>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateInstance<T, TA1, TA2, TA3, TA4, TA5>(int factoryMethodIndex, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5)
			{
				return ((Func<TA1, TA2, TA3, TA4, TA5, T>)FactoryMethods[factoryMethodIndex])(arg1, arg2, arg3, arg4, arg5);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Type DynamicType { get; private set; }
			public Delegate[] FactoryMethods { get; private set; }
		}
	}
}
