using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;

namespace Hapil.Fluent
{
	public class HappilField : IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly FieldBuilder m_FieldBuilder;
		private readonly bool m_IsStatic;
		
		//TODO: remove this field
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: remove this constructor (update unit tests)
		internal HappilField(string name, Type fieldType)
		{
			m_HappilClass = null;
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilField(HappilClass happilClass, string name, Type fieldType, bool isStatic = false)
		{
			m_HappilClass = happilClass;
			m_Name = happilClass.TakeMemberName(name);
			m_IsStatic = isStatic;

			var actualType = TypeTemplate.Resolve(fieldType);
			var attributes = (isStatic ? FieldAttributes.Private | FieldAttributes.Static : FieldAttributes.Private);

			m_FieldBuilder = happilClass.TypeBuilder.DefineField(m_Name, actualType, attributes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private HappilField(HappilClass happilClass, string name, bool isStatic, FieldBuilder fieldBuilder)
		{
			m_HappilClass = happilClass;
			m_Name = name;
			m_IsStatic = isStatic;
			m_FieldBuilder = fieldBuilder;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		void IHappilMember.DefineBody()
		{
			// nothing - a field does not have a body
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilMember.EmitBody()
		{
			// nothing - a field does not have a body
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		IDisposable IHappilMember.CreateTypeTemplateScope()
		{
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		MemberInfo IHappilMember.Declaration
		{
			get
			{
				return m_FieldBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IHappilMember.Name
		{
			get
			{
				return m_Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> AsOperand<T>()
		{
			var targetOperand = (m_IsStatic ? null : new HappilThis<object>(ownerMethod: null));
			return new FieldAccessOperand<T>(targetOperand, m_FieldBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilField Set<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new HappilAttributeBuilder<TAttribute>(values);
			m_FieldBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void SetAttributes(HappilAttributes attributes)
		{
			if ( attributes != null )
			{
				foreach ( var attribute in attributes.GetAttributes() )
				{
					m_FieldBuilder.SetCustomAttribute(attribute);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilClass OwnerClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FieldBuilder FieldBuilder
		{
			get
			{
				return m_FieldBuilder;
			}
		}
	}
}
