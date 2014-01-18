using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Happil.Fluent;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.Statements
{
	internal class LockStatement : IHappilStatement, IHappilLockSyntax
	{
		private readonly IHappilOperand<object> m_SyncRoot;
		private readonly int m_MillisecondsTimeout;
		private readonly List<IHappilStatement> m_BodyBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LockStatement(IHappilOperand<object> syncRoot, int millisecondsTimeout)
		{
			m_SyncRoot = syncRoot;
			m_MillisecondsTimeout = millisecondsTimeout;
			m_BodyBlock = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
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
				var m = scope.OwnerMethod;

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
