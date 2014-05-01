using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Statements;

namespace Happil.Operands
{
	public class Property<T> : MutableOperand<T>, INonPostfixNotation, IAcceptOperandVisitor
	{
		private readonly PropertyInfo m_Property;
		private readonly IOperand[] m_IndexArguments;
		private readonly MethodInfo m_Getter;
		private readonly MethodInfo m_Setter;
		private IOperand m_Target;
		private IOperand m_Value;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Property(IOperand target, PropertyInfo property, params IOperand[] indexArguments)
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

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Target);
			visitor.VisitOperandArray(m_IndexArguments);
			visitor.VisitOperand(ref m_Value);
		}

		#endregion

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

		#region Overrides of Object

		public override string ToString()
		{
			var isStatic = object.ReferenceEquals(m_Target, null);

			return string.Format(
				"{0}Field[{1}]",
				isStatic ? m_Property.DeclaringType.Name + "::" : m_Target.ToString() + ".",
				m_Property.Name);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool HasTarget
		{
			get
			{
				return (m_Target != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Property;
			}
		}

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
