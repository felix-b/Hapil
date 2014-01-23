using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil
{
	public static class MemberInfoExtensions
	{
		public static bool IsVoid(this MethodInfo method)
		{
			return (method.ReturnType == null || method.ReturnType == typeof(void));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IEnumerable<MethodInfo> OfSignature(this IEnumerable<MethodInfo> methods, Type returnType, params Type[] parameterTypes)
		{
			return methods.Where(m => m.IsOfSignature(returnType, parameterTypes));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IEnumerable<PropertyInfo> OfSignature(this IEnumerable<PropertyInfo> properties, Type propertyType, params Type[] indexParameterTypes)
		{
			return properties.Where(p => p.IsOfSignature(propertyType, indexParameterTypes));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsOfSignature(this MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			if ( method.ReturnType != TypeTemplate.Resolve(returnType) )
			{
				return false;
			}

			var actualParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

			if ( actualParameterTypes.Length != parameterTypes.Length )
			{
				return false;
			}

			for ( int i = 0 ; i < parameterTypes.Length ; i++ )
			{
				if ( TypeTemplate.Resolve(parameterTypes[i]) != actualParameterTypes[i] )
				{
					return false;
				}
			}

			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsOfSignature(this PropertyInfo property, Type propertyType, params Type[] indexParameterTypes)
		{
			if ( property.PropertyType != TypeTemplate.Resolve(propertyType) )
			{
				return false;
			}

			var actualParameterTypes = property.GetIndexParameters().Select(p => p.ParameterType).ToArray();

			if ( actualParameterTypes.Length != indexParameterTypes.Length )
			{
				return false;
			}

			for ( int i = 0 ; i < indexParameterTypes.Length ; i++ )
			{
				if ( TypeTemplate.Resolve(indexParameterTypes[i]) != actualParameterTypes[i] )
				{
					return false;
				}
			}

			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsIndexer(this PropertyInfo property)
		{
			return (property.GetIndexParameters().Length > 0);
		}
	}
}
