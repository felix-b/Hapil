using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal class TryStatement : IHappilStatement, IHappilCatchSyntax
	{
		private readonly List<IHappilStatement> m_TryBlock;
		private readonly List<IHappilStatement> m_FinallyBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement(Action body)
		{
			m_TryBlock = new List<IHappilStatement>();
			m_FinallyBlock = new List<IHappilStatement>();

			using ( new StatementScope(m_TryBlock) )
			{
				body();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilCatchSyntax Members

		public IHappilCatchSyntax Catch<TException>(Action body) where TException : Exception
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Finally(Action body)
		{
			using ( new StatementScope(m_FinallyBlock) )
			{
				body();
			}
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilCatchSyntax
	{
		IHappilCatchSyntax Catch<TException>(Action body) where TException : Exception;
		void Finally(Action body);
	}
}
