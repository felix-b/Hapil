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
		
		protected abstract Label LoopStartLabel { get; }
		protected abstract Label LoopEndLabel { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ContinueStatement : IHappilStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ContinueStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilStatement Members

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br_S, m_OwnerLoop.LoopStartLabel);
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BreakStatement : IHappilStatement
		{
			private readonly LoopStatementBase m_OwnerLoop;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public BreakStatement(LoopStatementBase ownerLoop)
			{
				m_OwnerLoop = ownerLoop;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilStatement Members

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Br_S, m_OwnerLoop.LoopEndLabel);
			}

			#endregion
		}
	}
}
