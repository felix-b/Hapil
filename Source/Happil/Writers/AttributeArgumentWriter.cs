using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Writers
{
	public abstract class AttributeArgumentWriter
	{
		public abstract CustomAttributeBuilder GetAttributeBuilder();
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public class AttributeArgumentWriter<TAttribute> : AttributeArgumentWriter
	{
		private readonly List<Type> m_ConstructorArgumentTypes = new List<Type>();
		private readonly List<object> m_ConstructorArgumentValues = new List<object>();
		private readonly List<PropertyInfo> m_NamedProperties = new List<PropertyInfo>();
		private readonly List<object> m_NamedPropertyValues = new List<object>();
		private readonly List<FieldInfo> m_NamedFields = new List<FieldInfo>();
		private readonly List<object> m_NamedFieldValues = new List<object>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeArgumentWriter(Action<AttributeArgumentWriter<TAttribute>> values)
		{
			if ( values != null )
			{
				values(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeArgumentWriter<TAttribute> Arg<T>(T value)
		{
			if ( m_NamedFields.Count > 0 || m_NamedProperties.Count > 0 )
			{
				throw new InvalidOperationException("Cannot mix Arg with Named in attribute values. All Arg's must come before Named.");
			}

			m_ConstructorArgumentTypes.Add(typeof(T));
			m_ConstructorArgumentValues.Add(value);

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeArgumentWriter<TAttribute> Named<T>(Expression<Func<TAttribute, T>> fieldOrProperty, T value)
		{
			var member = ((MemberExpression)fieldOrProperty.Body).Member;

			if ( member is PropertyInfo )
			{
				m_NamedProperties.Add((PropertyInfo)member);
				m_NamedPropertyValues.Add(value);
			}
			else if ( member is FieldInfo )
			{
				m_NamedFields.Add((FieldInfo)member);
				m_NamedFieldValues.Add(value);
			}
			else
			{
				throw new NotSupportedException("Specified expression cannot be parsed.");
			}

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override CustomAttributeBuilder GetAttributeBuilder()
		{
			var constructor = typeof(TAttribute).GetConstructor(m_ConstructorArgumentTypes.ToArray());

			if ( constructor == null )
			{
				throw new ArgumentException("Attribute has no constructor with matching signature.");
			}

			return new CustomAttributeBuilder(
				constructor,
				m_ConstructorArgumentValues.ToArray(),
				m_NamedProperties.ToArray(),
				m_NamedPropertyValues.ToArray(),
				m_NamedFields.ToArray(),
				m_NamedFieldValues.ToArray());
		}
	}
}
