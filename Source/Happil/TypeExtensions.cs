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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type[] GetTypeHierarchy(this Type type)
		{
			var baseTypes = new HashSet<Type>();
			GetAllBaseTypes(type, baseTypes);
			return baseTypes.ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void GetAllBaseTypes(Type currentType, HashSet<Type> baseTypes)
		{
			if ( baseTypes.Add(currentType) )
			{
				if ( currentType.IsClass && currentType.BaseType != null )
				{
					GetAllBaseTypes(currentType.BaseType, baseTypes);
				}
				
				foreach ( var baseInterface in currentType.GetInterfaces() )
				{
					GetAllBaseTypes(baseInterface, baseTypes);
				}
			}
		}
	}
}
