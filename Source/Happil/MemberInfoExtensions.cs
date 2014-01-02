using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil
{
	internal static class MemberInfoExtensions
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

		public static bool IsOfSignature(this MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			if ( method.ReturnType != returnType )
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
				if ( parameterTypes[i] != actualParameterTypes[i] )
				{
					return false;
				}
			}

			return true;
		}
	}
}
