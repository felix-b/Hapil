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
		private readonly StatementScope m_Previous;
		private readonly HappilClass m_OwnerClass;
		private readonly HappilMethod m_OwnerMethod;
		private readonly List<IHappilStatement> m_StatementList;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope(HappilClass ownerClass, HappilMethod ownerMethod, List<IHappilStatement> statementList)
		{
			if ( s_Current != null )
			{
				throw new InvalidOperationException("Root scope already exists.");
			}

			m_StatementList = statementList;
			m_OwnerMethod = ownerMethod;
			m_OwnerClass = ownerClass;

			m_Previous = null;
			s_Current = this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope()
		{
			m_Previous = s_Current;

			if ( m_Previous == null )
			{
				throw new InvalidOperationException("Parent scope is not present.");
			}

			m_StatementList = new List<IHappilStatement>();
			m_OwnerMethod = m_Previous.m_OwnerMethod;
			m_OwnerClass = m_Previous.m_OwnerClass;

			s_Current = this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IDisposable Members

		public void Dispose()
		{
			if ( s_Current != this )
			{
				throw new InvalidOperationException("Specified scope is not the current scope.");
			}

			s_Current = m_Previous;
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[ThreadStatic]
		private static StatementScope s_Current;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static StatementScope Current
		{
			get
			{
				var current = s_Current;

				if ( current == null )
				{
					throw new InvalidOperationException("There is no active scope at the moment.");
				}

				return current;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool Exists
		{
			get
			{
				return (s_Current != null);
			}
		}
	}
}
