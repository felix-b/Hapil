﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class IfStatement : StatementBase, IHapilIfBody, IHapilIfBodyThen
	{
		private readonly bool m_ConditionIsAlwaysTrue;
		private readonly StatementBlock m_ThenBlock;
		private readonly StatementBlock m_ElseBlock;
		private IOperand<bool> m_Condition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IfStatement(IOperand<bool> condition)
			: this(condition, conditionIsAlwaysTrue: false)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IfStatement(IOperand<bool> condition, bool conditionIsAlwaysTrue)
		{
			m_Condition = condition;
			m_ConditionIsAlwaysTrue = conditionIsAlwaysTrue;
			m_ThenBlock = new StatementBlock();
			m_ElseBlock = new StatementBlock();

			StatementScope.Current.Consume(condition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			if ( m_ConditionIsAlwaysTrue )
			{
				EmitThenBlock(il, ownerMethod);
			}
			else
			{
                EmitFullStatement(il, ownerMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Condition);
			visitor.VisitStatementBlock(m_ThenBlock);
			visitor.VisitStatementBlock(m_ElseBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilIfBody Members

		public IHapilIfBodyThen Then(Action thenBodyDefinition)
		{
			using ( new StatementScope(m_ThenBlock) )
			{
				thenBodyDefinition();
			}

			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilIfBodyThen Members

		public void Else(Action elseBodyDefinition)
		{
			using ( new StatementScope(m_ElseBlock) )
			{
				elseBodyDefinition();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilIfBody ElseIf(IOperand<bool> condition)
		{
			var nestedIf = new IfStatement(condition);
			m_ElseBlock.Add(nestedIf);
			return nestedIf;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			if ( m_ConditionIsAlwaysTrue )
			{
				return m_ThenBlock.ToString();
			}
			else
			{
				return (
					"IF (" + m_Condition.ToString() + ") " + 
					"THEN " + m_ThenBlock.ToString() +
					(m_ElseBlock.Count > 0 ? " ELSE " + m_ElseBlock.ToString() : ""));
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void EmitThenBlock(ILGenerator il, MethodMember ownerMethod)
		{
			foreach ( var statement in m_ThenBlock )
			{
				statement.Emit(il, ownerMethod);
			}
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void EmitFullStatement(ILGenerator il, MethodMember ownerMethod)
		{
			var afterIfBlock = il.DefineLabel();
			var afterElseBlock = (m_ElseBlock.Count > 0 ? il.DefineLabel() : new Label());

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, afterIfBlock);

			foreach ( var statement in m_ThenBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			if ( m_ElseBlock.Count > 0 )
			{
				il.Emit(OpCodes.Br, afterElseBlock);
			}

			il.MarkLabel(afterIfBlock);

			if ( m_ElseBlock.Count > 0 )
			{
				foreach ( var statement in m_ElseBlock )
				{
					statement.Emit(il, ownerMethod);
				}

				il.MarkLabel(afterElseBlock);
			}

			il.Emit(OpCodes.Nop);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilIfBody
	{
		IHapilIfBodyThen Then(Action thenBodyDefinition);
	}
	
	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilIfBodyThen
	{
		IHapilIfBody ElseIf(IOperand<bool> condition);
		void Else(Action elseBodyDefinition);
	}
}
