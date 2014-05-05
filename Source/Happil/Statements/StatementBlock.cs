using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Expressions;
using Happil.Members;

namespace Happil.Statements
{
	internal class StatementBlock : IEnumerable<StatementBase>
	{
		private readonly List<StatementBase> m_StatementList;
		private MethodMember m_OwnerMethod;
		private StatementBlock m_ParentBlock;
		private StatementBlock m_RootBlock;

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

		public void Insert(int index, params StatementBase[] statements)
		{
			m_StatementList.InsertRange(index, statements);
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

		public void Attach(StatementScope scope)
		{
			m_OwnerMethod = scope.OwnerMethod; 
			m_ParentBlock = (scope.Previous != null ? scope.Previous.StatementBlock : null);
			m_RootBlock = scope.Root.StatementBlock;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			var text = new StringBuilder();
			text.Append("{");

			foreach ( var statement in m_StatementList )
			{
				text.Append(statement.ToString() + ";");
			}

			text.Append("}");
			return text.ToString();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock ParentBlock
		{
			get
			{
				return m_ParentBlock;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock RootBlock
		{
			get
			{
				return m_RootBlock;
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
