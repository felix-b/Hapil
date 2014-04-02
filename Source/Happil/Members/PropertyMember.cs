using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Writers;

namespace Happil.Members
{
	public class PropertyMember : MemberBase
	{
		private readonly PropertyInfo m_Declaration;
		private readonly PropertyBuilder m_PropertyBuilder;
		private readonly List<PropertyWriterBase> m_Writers;
		private readonly MethodMember m_GetterMethod;
		private readonly MethodMember m_SetterMethod;
		private FieldMember m_BackingField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyMember(ClassType ownerClass, PropertyInfo declaration)
			: base(ownerClass, declaration.Name)
		{
			m_Writers = new List<PropertyWriterBase>();
			m_Declaration = declaration;
			m_PropertyBuilder = ownerClass.TypeBuilder.DefineProperty(
				ownerClass.TakeMemberName(declaration.Name),
				declaration.Attributes,
				declaration.PropertyType,
				declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray());

			var getterDeclaration = declaration.GetGetMethod();

			if ( getterDeclaration != null )
			{
				m_GetterMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, getterDeclaration));
				m_PropertyBuilder.SetGetMethod((MethodBuilder)m_GetterMethod.MethodFactory.Builder);
				ownerClass.AddMember(m_GetterMethod);
			}

			var setterDeclaration = declaration.GetSetMethod();

			if ( setterDeclaration != null )
			{
				m_SetterMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, setterDeclaration));
				m_PropertyBuilder.SetSetMethod((MethodBuilder)m_SetterMethod.MethodFactory.Builder);
				ownerClass.AddMember(m_SetterMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberInfo MemberDeclaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyInfo PropertyDeclaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember BackingField
		{
			get
			{
				if ( m_BackingField == null )
				{
					m_BackingField = new FieldMember(OwnerClass, "m_" + m_Declaration.Name, m_Declaration.PropertyType);
					OwnerClass.AddMember(m_BackingField);
				}
				
				return m_BackingField;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember GetterMethod
		{
			get
			{
				return m_GetterMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember SetterMethod
		{
			get
			{
				return m_SetterMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyBuilder PropertyBuilder
		{
			get
			{
				return m_PropertyBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type PropertyType
		{
			get
			{
				return m_Declaration.PropertyType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(PropertyWriterBase writer)
		{
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Write()
		{
			foreach ( var writer in m_Writers )
			{
				writer.Flush();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			// nothing
		}
	}
}
