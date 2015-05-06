using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Hapil.Members;
using Hapil.Operands;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Hapil.Statements
{
	internal class LockStatement : StatementBase, IHapilLockSyntax
	{
		private readonly int m_MillisecondsTimeout;
		private readonly StatementBlock m_BodyBlock;
		private IOperand<object> m_SyncRoot;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LockStatement(IOperand<object> syncRoot, int millisecondsTimeout)
		{
			m_SyncRoot = syncRoot;
			m_MillisecondsTimeout = millisecondsTimeout;
			m_BodyBlock = new StatementBlock();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il, ownerMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_SyncRoot);
			visitor.VisitStatementBlock(m_BodyBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilLockSyntax Members

		public void Do(Action body)
		{
			var timeoutExceptionMessage = string.Format("Lock could not be acquired within allotted timeout ({0} ms).", m_MillisecondsTimeout);

			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var w = scope.Writer;

				w.If(!Static.Func(Monitor.TryEnter, m_SyncRoot, w.Const(m_MillisecondsTimeout))).Then(() => 
					w.Throw<TimeoutException>(timeoutExceptionMessage)
				);

				w.Try(() => {
					body();
				})
				.Finally(() => {
					Static.Void(Monitor.Exit, m_SyncRoot);
				});
			}
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilLockSyntax
	{
		void Do(Action body);
	}
}
