using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;
using Hapil.Expressions;

namespace Hapil.Statements
{
	internal class WhileStatement : LoopStatementBase, IHapilWhileSyntax
	{
		private readonly StatementBlock m_BodyBlock;
		private IOperand<bool> m_Condition;
		private Label m_LoopStartLabel;
		private Label m_LoopEndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public WhileStatement(IOperand<bool> condition)
		{
			m_Condition = condition;
			m_BodyBlock = new StatementBlock();

			StatementScope.Current.Consume(condition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Overrides

		public override void Emit(ILGenerator il)
		{
			m_LoopStartLabel = il.DefineLabel();
			m_LoopEndLabel = il.DefineLabel();

			il.MarkLabel(m_LoopStartLabel);

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, m_LoopEndLabel);

			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}

			il.Emit(OpCodes.Br, m_LoopStartLabel);
			il.MarkLabel(m_LoopEndLabel);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Condition);
			visitor.VisitStatementBlock(m_BodyBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilWhileSyntax Members

		public void Do(Action<ILoopBody> body)
		{
			using ( new StatementScope(m_BodyBlock, loopStatement: this) )
			{
				body(this);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return (
				"WHILE (" + m_Condition.ToString() + ") " +
				m_BodyBlock.ToString());
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopStartLabel
		{
			get
			{
				return m_LoopStartLabel;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopEndLabel
		{
			get
			{
				return m_LoopEndLabel;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilWhileSyntax
	{
		void Do(Action<ILoopBody> body);
	}
}
