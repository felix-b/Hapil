using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class TryStatement : IHappilStatement, IHappilCatchSyntax
	{
		private readonly List<IHappilStatement> m_TryBlock;
		private readonly List<CatchBlock> m_CatchBlocks;
		private readonly List<IHappilStatement> m_FinallyBlock;
		private readonly List<LeaveBlock> m_LeaveBlocks;
		private Label m_EndExceptionBlockLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement(Action body)
		{
			m_TryBlock = new List<IHappilStatement>();
			m_CatchBlocks = new List<CatchBlock>();
			m_FinallyBlock = new List<IHappilStatement>();
			m_LeaveBlocks = new List<LeaveBlock>();

			using ( new StatementScope(m_TryBlock, this, ExceptionBlockType.Try) )
			{
				body();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
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

			foreach ( var leaveBlock in m_LeaveBlocks )
			{
				leaveBlock.Emit(il);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilCatchSyntax Members

		public IHappilCatchSyntax Catch<TException>(Action<HappilOperand<TException>> body) where TException : Exception
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

		public IHappilStatement WrapLeaveStatement(ILeaveStatement statement)
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
			public CatchBlock(TryStatement ownerStatement, Action<HappilOperand<TException>> body)
			{
				Statements = new List<IHappilStatement>();

				using ( var scope = new StatementScope(Statements, ownerStatement, ExceptionBlockType.Catch) )
				{
					ExceptionObject = scope.OwnerMethod.Local<TException>();
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
			
			public List<IHappilStatement> Statements { get; private set; }
			public HappilLocal<TException> ExceptionObject { get; private set; }
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

		private class LeaveStatement : IHappilStatement
		{
			private readonly LeaveBlock m_Destination;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public LeaveStatement(LeaveBlock destination)
			{
				m_Destination = destination;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilStatement Members

			public void Emit(ILGenerator il)
			{
				m_Destination.LeaveLabel = il.DefineLabel();
				il.Emit(OpCodes.Leave_S, m_Destination.LeaveLabel);
			}

			#endregion
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilCatchSyntax
	{
		IHappilCatchSyntax Catch<TException>(Action<HappilOperand<TException>> body) where TException : Exception;
		void Finally(Action body);
	}
}
