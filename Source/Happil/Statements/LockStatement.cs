using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Happil.Operands;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.Statements
{
	internal class LockStatement : StatementBase, IHappilLockSyntax
	{
		private readonly IOperand<object> m_SyncRoot;
		private readonly int m_MillisecondsTimeout;
		private readonly List<StatementBase> m_BodyBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LockStatement(IOperand<object> syncRoot, int millisecondsTimeout)
		{
			m_SyncRoot = syncRoot;
			m_MillisecondsTimeout = millisecondsTimeout;
			m_BodyBlock = new List<StatementBase>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilLockSyntax Members

		public void Do(Action body)
		{
			var timeoutExceptionMessage = string.Format("Lock could not be acquired within allotted timeout ({0} ms).", m_MillisecondsTimeout);

			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var m = scope.OwnerMethod.TransparentWriter;

				m.If(!Static.Func(Monitor.TryEnter, m_SyncRoot, m.Const(m_MillisecondsTimeout))).Then(() => 
					m.Throw<TimeoutException>(timeoutExceptionMessage)
				);

				m.Try(() => {
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

	public interface IHappilLockSyntax
	{
		void Do(Action body);
	}
}
