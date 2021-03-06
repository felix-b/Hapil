﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Expressions;
using Hapil.Writers;

namespace Hapil.Statements
{
	internal class ForStatement : 
		LoopStatementBase, 
		IHapilForWhileSyntax, 
		IHapilForNextSyntax, 
		IHapilForDoSyntax
	{
		private readonly StatementBlock m_PreconditionBlock;
		private readonly StatementBlock m_NextBlock;
		private readonly StatementBlock m_BodyBlock;
		private IOperand<bool> m_Condition;
		private Label m_LoopNextLabel;
		private Label m_LoopEndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ForStatement(Action precondition)
		{
			m_PreconditionBlock = new StatementBlock();
			m_NextBlock = new StatementBlock();
			m_BodyBlock = new StatementBlock();

			using ( new StatementScope(m_PreconditionBlock) )
			{
				precondition();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			m_LoopNextLabel = il.DefineLabel();
			m_LoopEndLabel = il.DefineLabel();
			var conditionLabel = il.DefineLabel();

			foreach ( var statement in m_PreconditionBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			il.MarkLabel(conditionLabel);

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, m_LoopEndLabel);

			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			il.MarkLabel(m_LoopNextLabel);

			foreach ( var statement in m_NextBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			il.Emit(OpCodes.Br, conditionLabel);

			il.MarkLabel(m_LoopEndLabel);
			il.Emit(OpCodes.Nop);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitStatementBlock(m_PreconditionBlock);
			visitor.VisitStatementBlock(m_NextBlock);
			visitor.VisitStatementBlock(m_BodyBlock);
			visitor.VisitOperand(ref m_Condition);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForWhileSyntax Members

		public IHapilForNextSyntax While(IOperand<bool> condition)
		{
			m_Condition = condition;
			StatementScope.Current.Consume(condition);
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForNextSyntax Members

		public IHapilForDoSyntax Next(Action next)
		{
			using ( new StatementScope(m_NextBlock, loopStatement: this) )
			{
				next();
			}

			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForDoSyntax Members

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
			return string.Format(
				"FOR ({0} ; {1} ; {2}) {3}",
				m_PreconditionBlock.ToString(),
				m_Condition.ToString(),
				m_NextBlock.ToString(),
				m_BodyBlock.ToString());
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override Label LoopStartLabel
		{
			get
			{
				return m_LoopNextLabel; // in the FOR loop, we return a label at the beginning of the NEXT block
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override Label LoopEndLabel
		{
			get
			{
				return m_LoopEndLabel;
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilForWhileSyntax
	{
		IHapilForNextSyntax While(IOperand<bool> condition);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilForNextSyntax
	{
		IHapilForDoSyntax Next(Action next);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilForDoSyntax
	{
		void Do(Action<ILoopBody> body);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public class HapilForShortSyntax
	{
		private readonly MethodWriterBase m_Writer;
		private readonly Operand<int> m_From;
		private readonly Operand<int> m_To;
		private readonly int m_Increment;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal HapilForShortSyntax(MethodWriterBase writer, Operand<int> from, Operand<int> to, int increment)
		{
			m_Writer = writer;
			m_From = from;
			m_To = to;
			m_Increment = increment;

			StatementScope.Current.Consume(to);
			StatementScope.Current.Consume(from);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void Do(Action<ILoopBody, Local<int>> body)
		{
			var counter = m_Writer.Local<int>();
			ForStatement statement;

			if ( m_Increment > 0 )
			{
				statement = new ForStatement(() => counter.Assign(m_From));
				statement.While(counter < m_To);
			}
			else if ( m_Increment < 0 )
			{
				statement = new ForStatement(() => counter.Assign(m_From - 1));
				statement.While(counter >= m_To);
			}
			else
			{
				throw new ArgumentException("Increment cannot be zero.");
			}

			statement.Next(() => counter.Assign(counter + new Constant<int>(m_Increment)));
			statement.Do(loop => {
				body(loop, counter);
			});

			StatementScope.Current.AddStatement(statement);						
		}
	}
}
