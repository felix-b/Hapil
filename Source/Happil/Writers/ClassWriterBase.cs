using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public abstract class ClassWriterBase
	{
		private readonly ClassType m_OwnerClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ClassWriterBase(ClassType ownerClass)
		{
			m_OwnerClass = ownerClass;
			ownerClass.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> ImplementBase<TBase>()
		{
			return new ImplementationClassWriter<TBase>(m_OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<object> Implement(Type baseType)
		{
			return new ImplementationClassWriter<object>(m_OwnerClass, baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get { return m_OwnerClass; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected abstract void Flush();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static implicit operator ClassType(ClassWriterBase writer)
		{
			return writer.OwnerClass;
		}
	}
}
