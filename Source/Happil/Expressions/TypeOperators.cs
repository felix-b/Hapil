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

		private MethodInfo m_OpEquality;
		private MethodInfo m_OpAddition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeOperators(Type type)
		{
			m_Type = type;

			ApplySpecialCases(type);

			m_OpEquality = m_OpEquality ?? type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
			m_OpAddition = m_OpAddition ?? type.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
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

		public MethodInfo OpAddition
		{
			get { return m_OpAddition; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ApplySpecialCases(Type type)
		{
			SpecialCases specialCases;

			if ( s_SpecialCasesByType.TryGetValue(type, out specialCases) )
			{
				specialCases.Apply(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, TypeOperators> s_CachedOperatorsByType;
		private static readonly Dictionary<Type, SpecialCases> s_SpecialCasesByType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static TypeOperators()
		{
			s_CachedOperatorsByType = new ConcurrentDictionary<Type, TypeOperators>();
			
			s_SpecialCasesByType = new Dictionary<Type, SpecialCases>() {
				{ typeof(string), new StringSpecialCases() }
			};
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeOperators GetOperators(Type type)
		{
			return s_CachedOperatorsByType.GetOrAdd(type, key => new TypeOperators(key));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class SpecialCases
		{
			public abstract void Apply(TypeOperators operators);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class StringSpecialCases : SpecialCases
		{
			public override void Apply(TypeOperators operators)
			{
				var type = typeof(string);

				operators.m_OpAddition = type.GetMethod(
					"Concat", 
					BindingFlags.Static | BindingFlags.Public, 
					null, 
					new[] { typeof(string), typeof(string) }, 
					null);
			}
		}
	}
}
