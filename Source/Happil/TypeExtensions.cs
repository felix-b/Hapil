using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;

namespace Happil
{
	public static class TypeExtensions
	{
		public static object GetDefaultValue(this Type type)
		{
			if ( type.IsValueType )
			{
				return Activator.CreateInstance(type);
			}
			else
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsNullableValueType(this Type type)
		{
			return (
				type.IsValueType &&
				type.IsGenericType && 
				!type.IsGenericTypeDefinition && 
				type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}
	}
}
