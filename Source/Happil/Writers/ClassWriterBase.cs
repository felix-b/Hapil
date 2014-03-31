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

		//TODO:redesign:review overloads
		public ImplementationClassWriter<TBase> ImplementBase<TBase>()
		{
			return new ImplementationClassWriter<TBase>(m_OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign:review overloads
		public ImplementationClassWriter<object> Implement(Type baseType)
		{
			return new ImplementationClassWriter<object>(m_OwnerClass, baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign:review overloads
		public ImplementationClassWriter<T> AsBase<T>()
		{
			return new ImplementationClassWriter<T>(OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign:review overloads
		public ImplementationClassWriter<T> ImplementInterface<T>()
		{
			return new ImplementationClassWriter<T>(OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign:review overloads
		public ImplementationClassWriter<object> ImplementInterface(Type interfaceType)
		{
			return new ImplementationClassWriter<object>(OwnerClass, interfaceType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get { return m_OwnerClass; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected FieldMember DefineField(string name, Type fieldType, bool isStatic)
		{
			var field = new FieldMember(m_OwnerClass, name, fieldType, isStatic);
			m_OwnerClass.AddMember(field);
			return field;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected FieldMember DefineField<T>(string name, bool isStatic)
		{
			return DefineField(name, typeof(T), isStatic);
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
