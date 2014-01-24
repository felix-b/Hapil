using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil.Expressions
{
	public class TypeOperators
	{
		private readonly Type m_Type;
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public readonly MethodInfo OpEquality;
		public readonly MethodInfo OpAddition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeOperators(Type type)
		{
			m_Type = type;
			
			OpEquality = type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
			OpAddition = type.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type Type
		{
			get { return m_Type; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, TypeOperators> s_CachedOperatorsByType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static TypeOperators()
		{
			s_CachedOperatorsByType = new ConcurrentDictionary<Type, TypeOperators>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeOperators GetOperators(Type type)
		{
			return s_CachedOperatorsByType.GetOrAdd(type, key => new TypeOperators(key));
		}
	}
}
