using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Expressions;

namespace Hapil.Statements
{
	internal class DoWhileStatement : LoopStatementBase, IHapilDoWhileSyntax
	{
		private readonly StatementBlock m_BodyBlock;
		private IOperand<bool> m_Condition = null;
		private Label m_LoopStartLabel;
		private Label m_LoopEndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DoWhileStatement(Action<ILoopBody> body)
		{
			m_BodyBlock = new StatementBlock();

			using ( new StatementScope(m_BodyBlock, loopStatement: this) )
			{
				body(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			m_LoopStartLabel = il.DefineLabel();
			m_LoopEndLabel = il.DefineLabel();
			
			var nextIterationLabel = il.DefineLabel();
			il.MarkLabel(nextIterationLabel);

			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			il.MarkLabel(m_LoopStartLabel);

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);
			il.Emit(OpCodes.Brtrue, nextIterationLabel);
			
			il.MarkLabel(m_LoopEndLabel);
			
			il.Emit(OpCodes.Nop);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Condition);
			visitor.VisitStatementBlock(m_BodyBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilDoWhileSyntax Members

		public void While(IOperand<bool> condition)
		{
			m_Condition = condition;
			StatementScope.Current.Consume(condition);
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

	public interface IHapilDoWhileSyntax
	{
		void While(IOperand<bool> condition);
	}
}
