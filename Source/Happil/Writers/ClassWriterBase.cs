using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Decorators;
using Happil.Expressions;
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

		public FieldAccessOperand<T> Field<T>(string name)
		{
			var field = DefineField<T>(name, isStatic: false);
			return field.AsOperand<T>(); //TODO: check that T is compatible with field type
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> StaticField<T>(string name)
		{
			var fieldMember = DefineField<T>(name, isStatic: true);
			return fieldMember.AsOperand<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> DependencyField<T>(string name)
		{
			var field = OwnerClass.RegisterDependency<T>(() => DefineField<T>(name, isStatic: false));
			return field.AsOperand<T>();
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
		public ImplementationClassWriter<TBase> ImplementBase<TBase>()
		{
			return new ImplementationClassWriter<TBase>(m_OwnerClass);
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

		public ClassWriterBase DecorateWith<TDecorator>() where TDecorator : ClassDecoratorBase, new()
		{
			var decorator = new TDecorator();
			var decoratingWriter = new DecoratingClassWriter(m_OwnerClass, decorator);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassWriterBase DecorateWith(ClassDecoratorBase decorator)
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
