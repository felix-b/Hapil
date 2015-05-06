using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class TryStatement : StatementBase, IHapilCatchSyntax
	{
		private readonly StatementBlock m_TryBlock;
		private readonly List<CatchBlock> m_CatchBlocks;
		private readonly StatementBlock m_FinallyBlock;
		private readonly List<LeaveBlock> m_LeaveBlocks;
		private Label m_EndExceptionBlockLabel;
		private Label m_EndLeaveBlocksLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement(Action body)
		{
			m_TryBlock = new StatementBlock();
			m_CatchBlocks = new List<CatchBlock>();
			m_FinallyBlock = new StatementBlock();
			m_LeaveBlocks = new List<LeaveBlock>();

			using ( new StatementScope(m_TryBlock, this, ExceptionBlockType.Try) )
			{
				body();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			m_EndLeaveBlocksLabel = il.DefineLabel();
			m_EndExceptionBlockLabel = il.BeginExceptionBlock();

			foreach ( var statement in m_TryBlock )
			{
				statement.Emit(il, ownerMethod);
			}

			foreach ( var catchBlock in m_CatchBlocks )
			{
				catchBlock.Emit(il, ownerMethod);
			}

			if ( m_FinallyBlock.Count > 0 || m_CatchBlocks.Count == 0 )
			{
				il.BeginFinallyBlock();

				foreach ( var statement in m_FinallyBlock )
				{
                    statement.Emit(il, ownerMethod);
				}
			}

			il.EndExceptionBlock();

			if ( m_LeaveBlocks.Count > 0 )
			{
				il.Emit(OpCodes.Br, m_EndLeaveBlocksLabel);

				foreach ( var leaveBlock in m_LeaveBlocks )
				{
					leaveBlock.Emit(il, ownerMethod);
				}

				il.MarkLabel(m_EndLeaveBlocksLabel);
			}

			il.Emit(OpCodes.Nop);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitStatementBlock(m_TryBlock);

			foreach ( var catchBlock in m_CatchBlocks )
			{
				catchBlock.AcceptVisitor(visitor);
			}
			
			visitor.VisitStatementBlock(m_FinallyBlock);

			foreach ( var leaveBlock in m_LeaveBlocks )
			{
				leaveBlock.AcceptVisitor(visitor);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilCatchSyntax Members

		public IHapilCatchSyntax Catch<TException>(Action<Operand<TException>> body) where TException : Exception
		{
			m_CatchBlocks.Add(new CatchBlock<TException>(this, body));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Finally(Action body)
		{
			using ( new StatementScope(m_FinallyBlock, this, ExceptionBlockType.Finally) )
			{
				body();
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBase WrapLeaveStatement(ILeaveStatement statement)
		{
			var destination = new LeaveBlock(statement);
			m_LeaveBlocks.Add(destination);
			return new LeaveStatement(destination);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class CatchBlock
		{
            public abstract void Emit(ILGenerator il, MethodMember ownerMethod);
			public abstract void AcceptVisitor(OperandVisitorBase visitor);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class CatchBlock<TException> : CatchBlock where TException : Exception
		{
			private Local<TException> m_ExceptionObject;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public CatchBlock(TryStatement ownerStatement, Action<Operand<TException>> body)
			{
				Statements = new StatementBlock();

				using ( var scope = new StatementScope(Statements, ownerStatement, ExceptionBlockType.Catch) )
				{
					m_ExceptionObject = scope.AddLocal<TException>();
					body(ExceptionObject);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void Emit(ILGenerator il, MethodMember ownerMethod)
			{
				il.BeginCatchBlock(typeof(TException));
				m_ExceptionObject.EmitStore(il);

				foreach ( var statement in Statements )
				{
					statement.Emit(il, ownerMethod);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void AcceptVisitor(OperandVisitorBase visitor)
			{
				visitor.VisitOperand(ref m_ExceptionObject);
				visitor.VisitStatementBlock(Statements);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public StatementBlock Statements { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Local<TException> ExceptionObject
			{
				get
				{
					return m_ExceptionObject;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class LeaveBlock
		{
			private readonly ILeaveStatement m_LeaveStatement;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public LeaveBlock(ILeaveStatement statement)
			{
				m_LeaveStatement = statement;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Emit(ILGenerator il, MethodMember ownerMethod)
			{
				il.MarkLabel(LeaveLabel);
				m_LeaveStatement.Emit(il, ownerMethod);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void AcceptVisitor(OperandVisitorBase visitor)
			{
				visitor.VisitStatement((StatementBase)m_LeaveStatement);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Label LeaveLabel { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class LeaveStatement : StatementBase
		{
			private readonly LeaveBlock m_Destination;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public LeaveStatement(LeaveBlock destination)
			{
				m_Destination = destination;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region StatementBase Members

			public override void Emit(ILGenerator il, MethodMember ownerMethod)
			{
				m_Destination.LeaveLabel = il.DefineLabel();
				il.Emit(OpCodes.Leave, m_Destination.LeaveLabel);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void AcceptVisitor(OperandVisitorBase visitor)
			{
				// nothing
			}

			#endregion
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilCatchSyntax
	{
		IHapilCatchSyntax Catch<TException>(Action<Operand<TException>> body) where TException : Exception;
		void Finally(Action body);
	}
}
