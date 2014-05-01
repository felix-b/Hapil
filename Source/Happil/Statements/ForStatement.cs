using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Expressions;

namespace Happil.Statements
{
	internal class ForStatement : 
		LoopStatementBase, 
		IHappilForWhileSyntax, 
		IHappilForNextSyntax, 
		IHappilForDoSyntax
	{
		private readonly List<StatementBase> m_PreconditionBlock;
		private readonly List<StatementBase> m_NextBlock;
		private readonly List<StatementBase> m_BodyBlock;
		private IOperand<bool> m_Condition;
		private Label m_LoopNextLabel;
		private Label m_LoopEndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ForStatement(Action precondition)
		{
			m_PreconditionBlock = new List<StatementBase>();
			m_NextBlock = new List<StatementBase>();
			m_BodyBlock = new List<StatementBase>();

			using ( new StatementScope(m_PreconditionBlock) )
			{
				precondition();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			m_LoopNextLabel = il.DefineLabel();
			m_LoopEndLabel = il.DefineLabel();
			var conditionLabel = il.DefineLabel();

			foreach ( var statement in m_PreconditionBlock )
			{
				statement.Emit(il);
			}

			il.MarkLabel(conditionLabel);

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, m_LoopEndLabel);

			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}

			il.MarkLabel(m_LoopNextLabel);

			foreach ( var statement in m_NextBlock )
			{
				statement.Emit(il);
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

		#region IHappilForWhileSyntax Members

		public IHappilForNextSyntax While(IOperand<bool> condition)
		{
			m_Condition = condition;
			StatementScope.Current.Consume(condition);
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForNextSyntax Members

		public IHappilForDoSyntax Next(Action next)
		{
			using ( new StatementScope(m_NextBlock) )
			{
				next();
			}

			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForDoSyntax Members

		public void Do(Action<ILoopBody> body)
		{
			using ( new StatementScope(m_BodyBlock) )
			{
				body(this);
			}
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

	public interface IHappilForWhileSyntax
	{
		IHappilForNextSyntax While(IOperand<bool> condition);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForNextSyntax
	{
		IHappilForDoSyntax Next(Action next);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForDoSyntax
	{
		void Do(Action<ILoopBody> body);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public class HappilForShortSyntax
	{
		private readonly MethodMember m_Method;
		private readonly Operand<int> m_From;
		private readonly Operand<int> m_To;
		private readonly int m_Increment;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilForShortSyntax(MethodMember method, Operand<int> from, Operand<int> to, int increment)
		{
			m_Method = method;
			m_From = from;
			m_To = to;
			m_Increment = increment;

			StatementScope.Current.Consume(to);
			StatementScope.Current.Consume(from);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void Do(Action<ILoopBody, Local<int>> body)
		{
			var counter = m_Method.AddLocal<int>();
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
