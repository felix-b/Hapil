using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	internal class NewStructExpression<TStruct> : ExpressionOperand<TStruct>, IValueTypeInitializer, IAcceptOperandVisitor
	{
		private readonly Type m_StructType;
		private readonly ConstructorInfo m_Constructor;
		private readonly IOperand[] m_ConstructorArguments;
		private readonly StatementScope m_StatementScope;
		private IOperand m_Target;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public NewStructExpression(IOperand[] constructorArguments = null)
		{
			m_StatementScope = StatementScope.Current;

			m_StructType = TypeTemplate.Resolve<TStruct>();
			m_ConstructorArguments = (constructorArguments ?? new IOperand[0]);

			if ( m_ConstructorArguments.Length > 0 )
			{
				var argumentTypes = m_ConstructorArguments.Select(arg => arg.OperandType).ToArray();
				m_Constructor = m_StructType.GetConstructor(argumentTypes);

				if ( m_Constructor == null )
				{
					throw new ArgumentException("Could not find constructor with specified argument types.");
				}

				foreach ( var argument in m_ConstructorArguments.Reverse() )
				{
					m_StatementScope.Consume(argument);
				}
			}

			m_StatementScope.RegisterExpressionStatement(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IValueTypeInitializer Members

		public IOperand Target
		{
			set
			{
				m_Target = value;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperandArray(m_ConstructorArguments);
			visitor.VisitOperand(ref m_Target);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format(
				"NewStruct[{0}]({1})",
				m_StructType.Name,
				string.Join(",", m_ConstructorArguments.Select(a => a.ToString())));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.NewStruct;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			Local<TStruct> tempLocal = (m_Target == null && m_Constructor == null ? m_StatementScope.AddLocal<TStruct>() : null);
			var effectiveTarget = (m_Target ?? tempLocal);

			if ( effectiveTarget != null )
			{
				if ( m_Constructor != null )
				{
					Helpers.EmitCall(il, effectiveTarget, m_Constructor, m_ConstructorArguments);
				}
				else
				{
					effectiveTarget.EmitTarget(il);
					effectiveTarget.EmitAddress(il);

					il.Emit(OpCodes.Initobj, m_StructType);
				}

				if ( ShouldLeaveValueOnStack )
				{
					effectiveTarget.EmitTarget(il);
					effectiveTarget.EmitLoad(il);
				}
			}
			else
			{
				foreach ( var argument in m_ConstructorArguments )
				{
					argument.EmitTarget(il);
					argument.EmitLoad(il);
				}

				il.Emit(OpCodes.Newobj, m_Constructor);

				if ( !ShouldLeaveValueOnStack )
				{
					il.Emit(OpCodes.Pop);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}
}
