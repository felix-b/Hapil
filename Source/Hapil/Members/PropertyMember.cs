using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Writers;

namespace Hapil.Members
{
	public class PropertyMember : MemberBase
	{
        private readonly string m_PropertyName;
        private readonly Type m_PropertyType;
        private readonly Type[] m_IndexParameterTypes;
        private readonly PropertyInfo m_Declaration;
		private readonly PropertyBuilder m_PropertyBuilder;
		private readonly List<PropertyWriterBase> m_Writers;
		private readonly MethodMember m_GetterMethod;
		private readonly MethodMember m_SetterMethod;
		private FieldMember m_BackingField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyMember(ClassType ownerClass, PropertyInfo declaration, FieldMember backingField = null)
			: base(ownerClass, declaration.Name)
		{
            m_PropertyName = declaration.Name;
            m_PropertyType = declaration.PropertyType;
            m_IndexParameterTypes = declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray();
            
            m_Writers = new List<PropertyWriterBase>();
			m_Declaration = declaration;
			m_PropertyBuilder = ownerClass.TypeBuilder.DefineProperty(
				ownerClass.TakeMemberName(declaration.Name, mustUseThisName: true),
				declaration.Attributes,
				declaration.PropertyType,
                m_IndexParameterTypes);

			var getterDeclaration = declaration.GetGetMethod();

			if ( getterDeclaration != null )
			{
				m_GetterMethod = new MethodMember(ownerClass, new DeclaredMethodFactory(ownerClass, getterDeclaration));
				m_PropertyBuilder.SetGetMethod((MethodBuilder)m_GetterMethod.MethodFactory.Builder);
			}

			var setterDeclaration = declaration.GetSetMethod();

			if ( setterDeclaration != null )
			{
				m_SetterMethod = new MethodMember(ownerClass, new DeclaredMethodFactory(ownerClass, setterDeclaration));
				m_PropertyBuilder.SetSetMethod((MethodBuilder)m_SetterMethod.MethodFactory.Builder);
			}

			m_BackingField = backingField;
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public PropertyMember(ClassType ownerClass, string propertyName, Type propertyType, FieldMember backingField = null, Type[] indexerTypes = null)
            : base(ownerClass, propertyName)
        {
            m_PropertyName = propertyName;
            m_PropertyType = propertyType;
            m_IndexParameterTypes = (indexerTypes ?? Type.EmptyTypes);

            m_Writers = new List<PropertyWriterBase>();
            m_Declaration = null;
            m_PropertyBuilder = ownerClass.TypeBuilder.DefineProperty(
                ownerClass.TakeMemberName(propertyName, mustUseThisName: true),
                PropertyAttributes.None,
                propertyType,
                m_IndexParameterTypes);

            var getterSignature = new MethodSignature(isStatic: false, isPublic: true, returnType: propertyType);
            m_GetterMethod = new MethodMember(ownerClass, new NewMethodFactory(ownerClass, "get_" + propertyName, getterSignature));
            m_PropertyBuilder.SetGetMethod((MethodBuilder)m_GetterMethod.MethodFactory.Builder);

            var setterSignature = new MethodSignature(isStatic: false, isPublic: true, argumentTypes: new[] { propertyType });
            m_SetterMethod = new MethodMember(ownerClass, new NewMethodFactory(ownerClass, "set_" + propertyName, setterSignature));
            m_PropertyBuilder.SetSetMethod((MethodBuilder)m_SetterMethod.MethodFactory.Builder);

            m_BackingField = backingField;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: refactor to remove code duplication with same method in writer class
		internal void AddAttributes(Func<PropertyMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				var attributeWriter = attributeWriterFactory(this);

				if ( attributeWriter != null )
				{
					foreach ( var attribute in attributeWriter.GetAttributes() )
					{
						m_PropertyBuilder.SetCustomAttribute(attribute);
					}
				}
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

		public override MemberKind Kind
		{
			get
			{
				return MemberKind.InstanceProperty;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember BackingField
		{
			get
			{
				if ( m_BackingField == null )
				{
					m_BackingField = new FieldMember(OwnerClass, "m_" + m_PropertyName, m_PropertyType);
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

		public bool HasGetter
		{
			get
			{
				return (m_GetterMethod != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool HasSetter
		{
			get
			{
				return (m_SetterMethod != null);
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
				return m_PropertyType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override IDisposable CreateTypeTemplateScope()
		{
			switch ( m_IndexParameterTypes.Length )
			{
				case 0:
					return TypeTemplate.CreateScope<TypeTemplate.TProperty>(m_PropertyType);
				case 1:
					return TypeTemplate.CreateScope(
						typeof(TypeTemplate.TProperty), m_PropertyType,
						typeof(TypeTemplate.TIndex1), m_IndexParameterTypes[0]);
				case 2:
					return TypeTemplate.CreateScope(
						typeof(TypeTemplate.TProperty), m_PropertyType,
                        typeof(TypeTemplate.TIndex1), m_IndexParameterTypes[0],
                        typeof(TypeTemplate.TIndex2), m_IndexParameterTypes[1]);
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
