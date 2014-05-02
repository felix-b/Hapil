using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Expressions;

namespace Happil.Statements
{
	internal class StatementBlock : IEnumerable<StatementBase>
	{
		private readonly List<StatementBase> m_StatementList;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock()
		{
			m_StatementList = new List<StatementBase>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEnumerable<StatementBase> Members

		public IEnumerator<StatementBase> GetEnumerator()
		{
			return m_StatementList.GetEnumerator();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_StatementList.GetEnumerator();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Add(StatementBase statement)
		{
			m_StatementList.Add(statement);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RemoveExpressionStatement(IExpressionOperand expression)
		{
			for ( int index = m_StatementList.Count - 1 ; index >= 0 ; index-- )
			{
				var statement = (m_StatementList[index] as ExpressionStatement);

				if ( statement != null && ReferenceEquals(statement.Expression, expression) )
				{
					m_StatementList.RemoveAt(index);
					break;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int Count
		{
			get
			{
				return m_StatementList.Count;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBase this[int index]
		{
			get
			{
				return m_StatementList[index];
			}
		}
	}
}
