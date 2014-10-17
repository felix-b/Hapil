using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;
using Hapil.Expressions;

namespace Hapil.Statements
{
	internal abstract class LoopStatementBase : StatementBase, ILoopBody
	{
		private readonly StatementScope m_HomeScope;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected LoopStatementBase()
		{
			m_HomeScope = StatementScope.Current;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Continue()
		{
			StatementScope.Current.AddStatement(new ContinueStatement(this));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Break()
		{
			StatementScope.Current.AddStatement(new BreakStatement(this));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected StatementScope HomeScope
		{
			get
			{
				return m_HomeScope;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		internal protected abstract Label LoopStartLabel { get; }
		internal protected abstract Label LoopEndLabel { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ContinueStatement : StatementBase, ILeaveStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ContinueStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region StatementBase Members

			public override void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br, m_OwnerLoop.LoopStartLabel);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void AcceptVisitor(OperandVisitorBase visitor)
			{
				// nothing
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ILeaveStatement Members

			public StatementScope HomeScope
			{
				get
				{
					return m_OwnerLoop.HomeScope;
				}
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BreakStatement : StatementBase, ILeaveStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public BreakStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region StatementBase Members

			public override void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br, m_OwnerLoop.LoopEndLabel);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void AcceptVisitor(OperandVisitorBase visitor)
			{
				// nothing
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ILeaveStatement Members

			public StatementScope HomeScope
			{
				get
				{
					return m_OwnerLoop.HomeScope;
				}
			}

			#endregion
		}
	}
}
