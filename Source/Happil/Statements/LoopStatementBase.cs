using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal abstract class LoopStatementBase : IHappilLoopBody
	{
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
		
		internal protected abstract Label LoopStartLabel { get; }
		internal protected abstract Label LoopEndLabel { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ContinueStatement : IHappilStatement, ILeaveStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;
			private readonly StatementScope m_HomeScope;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ContinueStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
				m_HomeScope = StatementScope.Current;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilStatement Members

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br_S, m_OwnerLoop.LoopStartLabel);
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ILeaveStatement Members

			public StatementScope HomeScope
			{
				get
				{
					return m_HomeScope;
				}
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BreakStatement : IHappilStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;
			private readonly StatementScope m_HomeScope;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public BreakStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
				m_HomeScope = StatementScope.Current;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilStatement Members

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br_S, m_OwnerLoop.LoopEndLabel);
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ILeaveStatement Members

			public StatementScope HomeScope
			{
				get
				{
					return m_HomeScope;
				}
			}

			#endregion
		}
	}
}
