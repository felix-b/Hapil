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
	}
}
