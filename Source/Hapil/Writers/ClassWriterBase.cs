using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Decorators;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
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

		public Field<T> Field<T>(string name, bool isPublic = false)
		{
			var field = DefineField<T>(name, isStatic: false, isPublic: isPublic);
			return field.AsOperand<T>(); //TODO: check that T is compatible with field type
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<T> StaticField<T>(string name, bool isPublic = false)
		{
			var fieldMember = DefineField<T>(name, isStatic: true, isPublic: isPublic);
			return fieldMember.AsOperand<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<T> DependencyField<T>(string name)
		{
			var field = OwnerClass.RegisterDependency<T>(() => DefineField<T>(name, isStatic: false));
			return field.AsOperand<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<object> Implement(Type baseType)
		{
			return new ImplementationClassWriter<object>(m_OwnerClass, baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<T> AsBase<T>()
		{
			return new ImplementationClassWriter<T>(OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> ImplementBase<TBase>()
		{
			return new ImplementationClassWriter<TBase>(m_OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<T> ImplementInterface<T>()
		{
			return new ImplementationClassWriter<T>(OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<object> ImplementInterface(Type interfaceType)
		{
			return new ImplementationClassWriter<object>(OwnerClass, interfaceType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassWriterBase DecorateWith<TDecorator>() where TDecorator : DecorationConvention, new()
		{
			var decorator = new TDecorator();
			var decoratingWriter = new DecoratingClassWriter(m_OwnerClass, decorator);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassWriterBase DecorateWith(DecorationConvention decorator)
		{
			var decoratingWriter = new DecoratingClassWriter(m_OwnerClass, decorator);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get { return m_OwnerClass; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected FieldMember DefineField(string name, Type fieldType, bool isStatic, bool isPublic = false)
		{
			var field = new FieldMember(m_OwnerClass, name, fieldType, isStatic, isPublic);
			m_OwnerClass.AddMember(field);
			return field;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected FieldMember DefineField<T>(string name, bool isStatic, bool isPublic = false)
		{
			return DefineField(name, typeof(T), isStatic, isPublic);
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
