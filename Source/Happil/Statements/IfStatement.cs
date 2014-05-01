using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;

namespace Happil.Statements
{
	internal class IfStatement : StatementBase, IHappilIfBody, IHappilIfBodyThen
	{
		private readonly bool m_ConditionIsAlwaysTrue;
		private readonly List<StatementBase> m_ThenBlock;
		private readonly List<StatementBase> m_ElseBlock;
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
			m_ThenBlock = new List<StatementBase>();
			m_ElseBlock = new List<StatementBase>();

			StatementScope.Current.Consume(condition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			if ( m_ConditionIsAlwaysTrue )
			{
				EmitThenBlock(il);
			}
			else
			{
				EmitFullStatement(il);
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

		#region IHappilIfBody Members

		public IHappilIfBodyThen Then(Action thenBodyDefinition)
		{
			using ( new StatementScope(m_ThenBlock) )
			{
				thenBodyDefinition();
			}

			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilIfBodyThen Members

		public void Else(Action elseBodyDefinition)
		{
			using ( new StatementScope(m_ElseBlock) )
			{
				elseBodyDefinition();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilIfBody ElseIf(IOperand<bool> condition)
		{
			var nestedIf = new IfStatement(condition);
			m_ElseBlock.Add(nestedIf);
			return nestedIf;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void EmitThenBlock(ILGenerator il)
		{
			foreach ( var statement in m_ThenBlock )
			{
				statement.Emit(il);
			}
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void EmitFullStatement(ILGenerator il)
		{
			var afterIfBlock = il.DefineLabel();
			var afterElseBlock = (m_ElseBlock.Count > 0 ? il.DefineLabel() : new Label());

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, afterIfBlock);

			foreach ( var statement in m_ThenBlock )
			{
				statement.Emit(il);
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
					statement.Emit(il);
				}

				il.MarkLabel(afterElseBlock);
			}

			il.Emit(OpCodes.Nop);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilIfBody
	{
		IHappilIfBodyThen Then(Action thenBodyDefinition);
	}
	
	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilIfBodyThen
	{
		IHappilIfBody ElseIf(IOperand<bool> condition);
		void Else(Action elseBodyDefinition);
	}
}
