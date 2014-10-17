using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hapil.Expressions
{
	public class TypeOperators
	{
		private readonly Type m_Type;
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private MethodInfo m_OpEquality;
		private MethodInfo m_OpInequality;
		private MethodInfo m_OpAddition;
		private MethodInfo m_OpSubtraction;
		private MethodInfo m_OpMultiply;
		private MethodInfo m_OpDivision;
		private MethodInfo m_OpModulus;
		private MethodInfo m_OpIncrement;
		private MethodInfo m_OpDecrement;
		private MethodInfo m_OpUnaryPlus;
		private MethodInfo m_OpNegation;
		private MethodInfo m_OpBitwiseNot;
		private MethodInfo m_OpBitwiseAnd;
		private MethodInfo m_OpBitwiseOr;
		private MethodInfo m_OpBitwiseXor;
		private MethodInfo m_OpLeftShift;
		private MethodInfo m_OpRightShift;
		private MethodInfo m_OpGreaterThan;
		private MethodInfo m_OpLessThan;
		private MethodInfo m_OpGreaterThanOrEqual;
		private MethodInfo m_OpLessThanOrEqual;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeOperators(Type type)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

			m_Type = type;
			ApplySpecialCases(type);

			m_OpEquality = m_OpEquality ?? type.GetMethod("op_Equality", flags);
			m_OpInequality = m_OpInequality ?? type.GetMethod("op_Inequality", flags);
			m_OpAddition = m_OpAddition ?? type.GetMethod("op_Addition", flags);
			m_OpSubtraction = m_OpSubtraction ?? type.GetMethod("op_Subtraction", flags);
			m_OpMultiply = m_OpMultiply ?? type.GetMethod("op_Multiply", flags);
			m_OpDivision = m_OpDivision ?? type.GetMethod("op_Division", flags);
			m_OpModulus = m_OpModulus ?? type.GetMethod("op_Modulus", flags);
			m_OpIncrement = m_OpIncrement ?? type.GetMethod("op_Increment", flags);
			m_OpDecrement = m_OpDecrement ?? type.GetMethod("op_Decrement", flags);
			m_OpUnaryPlus = m_OpNegation ?? type.GetMethod("op_UnaryPlus", flags);
			m_OpNegation = m_OpNegation ?? type.GetMethod("op_UnaryNegation", flags);

			m_OpBitwiseNot = m_OpBitwiseNot ?? type.GetMethod("op_OnesComplement", flags);
			m_OpBitwiseAnd = m_OpBitwiseAnd ?? type.GetMethod("op_BitwiseAnd", flags);
			m_OpBitwiseOr = m_OpBitwiseOr ?? type.GetMethod("op_BitwiseOr", flags);
			m_OpBitwiseXor = m_OpBitwiseXor ?? type.GetMethod("op_ExclusiveOr", flags);
			m_OpLeftShift = m_OpLeftShift ?? type.GetMethod("op_LeftShift", flags);
			m_OpRightShift = m_OpRightShift ?? type.GetMethod("op_RightShift", flags);
			m_OpGreaterThan = m_OpGreaterThan ?? type.GetMethod("op_GreaterThan", flags);
			m_OpGreaterThanOrEqual = m_OpGreaterThanOrEqual ?? type.GetMethod("op_GreaterThanOrEqual", flags);
			m_OpLessThan = m_OpLessThan ?? type.GetMethod("op_LessThan", flags);
			m_OpLessThanOrEqual = m_OpLessThanOrEqual ?? type.GetMethod("op_LessThanOrEqual", flags);
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

		public MethodInfo OpInequality
		{
			get { return m_OpInequality; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpAddition
		{
			get { return m_OpAddition; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpSubtraction
		{
			get { return m_OpSubtraction; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpMultiply
		{
			get { return m_OpMultiply; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpDivision
		{
			get { return m_OpDivision; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpModulus
		{
			get { return m_OpModulus; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpIncrement
		{
			get { return m_OpIncrement; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpDecrement
		{
			get { return m_OpDecrement; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpUnaryPlus
		{
			get { return m_OpUnaryPlus; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpNegation
		{
			get { return m_OpNegation; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpBitwiseNot
		{
			get { return m_OpBitwiseNot; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpBitwiseAnd
		{
			get { return m_OpBitwiseAnd; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpBitwiseOr
		{
			get { return m_OpBitwiseOr; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpBitwiseXor
		{
			get { return m_OpBitwiseXor; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpLeftShift
		{
			get { return m_OpLeftShift; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpRightShift
		{
			get { return m_OpRightShift; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpGreaterThan
		{
			get { return m_OpGreaterThan; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpGreaterThanOrEqual
		{
			get { return m_OpGreaterThanOrEqual; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpLessThan
		{
			get { return m_OpLessThan; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo OpLessThanOrEqual
		{
			get { return m_OpLessThanOrEqual; }
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
