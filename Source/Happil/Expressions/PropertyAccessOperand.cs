using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	public class PropertyAccessOperand<T> : MutableOperand<T>, INonPostfixNotation
	{
		private readonly IOperand m_Target;
		private readonly PropertyInfo m_Property;
		private readonly IOperand[] m_IndexArguments;
		private readonly MethodInfo m_Getter;
		private readonly MethodInfo m_Setter;
		private IOperand m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyAccessOperand(IOperand target, PropertyInfo property, params IOperand[] indexArguments)
		{
			m_Target = target;
			m_Property = property;
			m_IndexArguments = indexArguments;

			m_Getter = m_Property.GetGetMethod();
			m_Setter = m_Property.GetSetMethod();

			var scope = StatementScope.Current;
			scope.Consume(target);

			foreach ( var argument in m_IndexArguments )
			{
				scope.Consume(argument);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return (m_Target != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region INonPostfixNotation Members

		IOperand INonPostfixNotation.RightSide
		{
			set
			{
				m_Value = value;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( m_Target != null )
			{
				m_Target.EmitTarget(il);
				m_Target.EmitLoad(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			if ( m_Getter != null )
			{
				Helpers.EmitCall(il, target: null, method: m_Getter, arguments: m_IndexArguments);
			}
			else
			{
				throw new InvalidOperationException(string.Format("Property '{0}' does not define a getter.", m_Property.Name));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			if ( m_Setter != null )
			{
				var setterArguments = m_IndexArguments.ConcatIf(m_Value).ToArray();
				Helpers.EmitCall(il, target: null, method: m_Setter, arguments: setterArguments);
			}
			else
			{
				throw new InvalidOperationException(string.Format("Property '{0}' is read-only.", m_Property.Name));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException("Properties cannot be passed by reference.");
		}
	}
}
