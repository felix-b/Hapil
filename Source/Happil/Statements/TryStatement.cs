using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using Happil.Operands;

namespace Happil.Statements
{
	internal class TryStatement : StatementBase, IHappilCatchSyntax
	{
		private readonly List<StatementBase> m_TryBlock;
		private readonly List<CatchBlock> m_CatchBlocks;
		private readonly List<StatementBase> m_FinallyBlock;
		private readonly List<LeaveBlock> m_LeaveBlocks;
		private Label m_EndExceptionBlockLabel;
		private Label m_EndLeaveBlocksLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement(Action body)
		{
			m_TryBlock = new List<StatementBase>();
			m_CatchBlocks = new List<CatchBlock>();
			m_FinallyBlock = new List<StatementBase>();
			m_LeaveBlocks = new List<LeaveBlock>();

			using ( new StatementScope(m_TryBlock, this, ExceptionBlockType.Try) )
			{
				body();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			m_EndLeaveBlocksLabel = il.DefineLabel();
			m_EndExceptionBlockLabel = il.BeginExceptionBlock();

			foreach ( var statement in m_TryBlock )
			{
				statement.Emit(il);
			}

			foreach ( var catchBlock in m_CatchBlocks )
			{
				catchBlock.Emit(il);
			}

			if ( m_FinallyBlock.Count > 0 )
			{
				il.BeginFinallyBlock();

				foreach ( var statement in m_FinallyBlock )
				{
					statement.Emit(il);
				}
			}

			il.EndExceptionBlock();

			if ( m_LeaveBlocks.Count > 0 )
			{
				il.Emit(OpCodes.Br, m_EndLeaveBlocksLabel);

				foreach ( var leaveBlock in m_LeaveBlocks )
				{
					leaveBlock.Emit(il);
				}

				il.MarkLabel(m_EndLeaveBlocksLabel);
			}

			il.Emit(OpCodes.Nop);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilCatchSyntax Members

		public IHappilCatchSyntax Catch<TException>(Action<Operand<TException>> body) where TException : Exception
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
			public abstract void Emit(ILGenerator il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class CatchBlock<TException> : CatchBlock where TException : Exception
		{
			public CatchBlock(TryStatement ownerStatement, Action<Operand<TException>> body)
			{
				Statements = new List<StatementBase>();

				using ( var scope = new StatementScope(Statements, ownerStatement, ExceptionBlockType.Catch) )
				{
					ExceptionObject = scope.OwnerMethod.AddLocal<TException>();
					body(ExceptionObject);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void Emit(ILGenerator il)
			{
				il.BeginCatchBlock(typeof(TException));
				ExceptionObject.EmitStore(il);

				foreach ( var statement in Statements )
				{
					statement.Emit(il);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public List<StatementBase> Statements { get; private set; }
			public LocalOperand<TException> ExceptionObject { get; private set; }
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

			public void Emit(ILGenerator il)
			{
				il.MarkLabel(LeaveLabel);
				m_LeaveStatement.Emit(il);
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

			public override void Emit(ILGenerator il)
			{
				m_Destination.LeaveLabel = il.DefineLabel();
				il.Emit(OpCodes.Leave, m_Destination.LeaveLabel);
			}

			#endregion
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilCatchSyntax
	{
		IHappilCatchSyntax Catch<TException>(Action<Operand<TException>> body) where TException : Exception;
		void Finally(Action body);
	}
}
