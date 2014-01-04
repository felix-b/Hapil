using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Selectors
{
	/// <summary>
	/// Serves as type parameter for non-typed member selectors.
	/// </summary>
	public class TypeTemplate
	{
		private readonly Type m_CastType;
		private readonly object m_CastValue;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeTemplate(Type castType, object castValue)
		{
			m_CastType = castType;
			m_CastValue = castValue;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type CastType
		{
			get
			{
				return m_CastType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public object CastValue
		{
			get
			{
				return m_CastValue;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeTemplate Cast<T>(T constantValue)
		{
			return new TypeTemplate(typeof(T), constantValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeTemplate DefaultValue
		{
			get
			{
				return new TypeTemplate();
			}
		}
	}
}
