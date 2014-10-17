using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil
{
	public class ObjectFactoryContext
	{
		private readonly ObjectFactoryBase m_Factory;
		private TypeKey m_TypeKey;
		private ClassType m_ClassType;
		private string m_ClassFullName;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ObjectFactoryContext(ObjectFactoryBase factory, TypeKey typeKey)
		{
			m_Factory = factory;
			m_TypeKey = typeKey;
			m_ClassType = null;
			m_ClassFullName = typeKey.SuggestClassName(factory);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> CreateImplementationWriter<TBase>()
		{
			EnsureClassTypeDefined();
			return new ImplementationClassWriter<TBase>(m_ClassType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> CreateImplementationWriter<TBase>(Type baseType)
		{
			EnsureClassTypeDefined();
			return new ImplementationClassWriter<TBase>(m_ClassType, baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DecoratingClassWriter CreateDecorationWriter(DecorationConvention decoration)
		{
			EnsureClassTypeDefined();
			return new DecoratingClassWriter(m_ClassType, decoration);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeKey TypeKey
		{
			get
			{
				return m_TypeKey;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DynamicModule Module
		{
			get
			{
				return m_Factory.Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ObjectFactoryBase Factory
		{
			get
			{
				return m_Factory;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type BaseType
		{
			get
			{
				return m_TypeKey.BaseType;
			}
			set
			{
				ValidateClassTypeNotYetDefined();
				m_TypeKey = new TypeKey(
					baseType: value,
					primaryInterface: m_TypeKey.PrimaryInterface,
					secondaryInterfaces: m_TypeKey.SecondaryInterfaces);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string ClassFullName
		{
			get
			{
				return m_ClassFullName;
			}
			set
			{
				ValidateClassTypeNotYetDefined();
				m_ClassFullName = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType ClassType
		{
			get
			{
				return m_ClassType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void EnsureClassTypeDefined()
		{
			if ( m_ClassType == null )
			{
				m_ClassType = m_Factory.Module.DefineClass(m_TypeKey, m_ClassFullName);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void ValidateClassTypeNotYetDefined()
		{
			if ( m_ClassType != null )
			{
				throw new InvalidOperationException("Cannot perform requested operation because class type was already defined.");
			}
		}
	}
}
