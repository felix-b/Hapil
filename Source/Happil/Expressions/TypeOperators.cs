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
		private readonly MethodInfo m_OpEquality;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeOperators(Type type)
		{
			m_Type = type;
			m_OpEquality = type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type Type
		{
			get { return m_Type; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpEquality
		{
			get { return m_OpEquality; }
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
