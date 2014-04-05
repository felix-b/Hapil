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
				ownerClass.TakeMemberName(declaration.Name, mustUseThisName: true),
				declaration.Attributes,
				declaration.PropertyType,
				declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray());

			var getterDeclaration = declaration.GetGetMethod();

			if ( getterDeclaration != null )
			{
				m_GetterMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, getterDeclaration));
				m_PropertyBuilder.SetGetMethod((MethodBuilder)m_GetterMethod.MethodFactory.Builder);
			}

			var setterDeclaration = declaration.GetSetMethod();

			if ( setterDeclaration != null )
			{
				m_SetterMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, setterDeclaration));
				m_PropertyBuilder.SetSetMethod((MethodBuilder)m_SetterMethod.MethodFactory.Builder);
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

		public override string Name
		{
			get
			{
				return m_PropertyBuilder.Name;
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

		internal override IDisposable CreateTypeTemplateScope()
		{
			var indexParameters = m_Declaration.GetIndexParameters();

			switch ( indexParameters.Length )
			{
				case 0:
					return TypeTemplate.CreateScope<TypeTemplate.TProperty>(m_Declaration.PropertyType);
				case 1:
					return TypeTemplate.CreateScope(
						typeof(TypeTemplate.TProperty), m_Declaration.PropertyType,
						typeof(TypeTemplate.TIndex1), indexParameters[0].ParameterType);
				case 2:
					return TypeTemplate.CreateScope(
						typeof(TypeTemplate.TProperty), m_Declaration.PropertyType,
						typeof(TypeTemplate.TIndex1), indexParameters[0].ParameterType,
						typeof(TypeTemplate.TIndex2), indexParameters[1].ParameterType);
				default:
					throw new NotSupportedException("Properties with more than 2 indexer parameters are not supported.");
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

			if ( m_GetterMethod != null )
			{
				using ( m_GetterMethod.CreateTypeTemplateScope() )
				{
					m_GetterMethod.Write();
				}
			}

			if ( m_SetterMethod != null )
			{
				using ( m_SetterMethod.CreateTypeTemplateScope() )
				{
					m_SetterMethod.Write();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			if ( m_GetterMethod != null )
			{
				using ( m_GetterMethod.CreateTypeTemplateScope() )
				{
					m_GetterMethod.Compile();
				}
			}

			if ( m_SetterMethod != null )
			{
				using ( m_SetterMethod.CreateTypeTemplateScope() )
				{
					m_SetterMethod.Compile();
				}
			}
		}
	}
}
