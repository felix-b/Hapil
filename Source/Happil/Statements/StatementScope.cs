using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class StatementScope : IDisposable
	{
		private readonly HappilClass m_OwnerClass;
		private readonly HappilMethod m_OwnerMethod;
		private readonly List<IHappilStatement> m_StatementList;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope(HappilClass ownerClass, HappilMethod ownerMethod, List<IHappilStatement> statementList)
		{
			m_StatementList = statementList;
			m_OwnerMethod = ownerMethod;
			m_OwnerClass = ownerClass;

			m_OwnerClass.PushScope(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IDisposable Members

		public void Dispose()
		{
			m_OwnerClass.PopScope(this);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AddStatement(IHappilStatement statement)
		{
			m_StatementList.Add(statement);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RegisterExpressionStatement(IHappilExpression expression)
		{
			if ( expression != null )
			{
				m_StatementList.Add(new ExpressionStatement(expression));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void UnregisterExpressionStatement(IHappilExpression expression)
		{
			if ( expression != null )
			{
				var lastExpressionStatement = (m_StatementList.LastOrDefault() as ExpressionStatement);

				if ( lastExpressionStatement != null && object.ReferenceEquals(lastExpressionStatement.Expression, expression) )
				{
					m_StatementList.RemoveAt(m_StatementList.Count - 1);
				}
			}
		}
	}
}
