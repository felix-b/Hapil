using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Closures;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Statements
{
	internal class StatementBlock : IEnumerable<StatementBase>
	{
		private readonly List<StatementBase> m_StatementList;
		private MethodMember m_OwnerMethod;
		private StatementBlock m_ParentBlock;
		private int m_Depth;
		private ClosureDefinition m_ClosureDefinition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock()
		{
			m_StatementList = new List<StatementBase>();
			m_Depth = 0;
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

		public void BindToMethod(MethodMember ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
			m_ParentBlock = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Insert(int index, params StatementBase[] statements)
		{
			m_StatementList.InsertRange(index, statements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int RemoveExpressionStatement(IExpressionOperand expression)
		{
			for ( int index = m_StatementList.Count - 1 ; index >= 0 ; index-- )
			{
				var statement = (m_StatementList[index] as ExpressionStatement);

				if ( statement != null && ReferenceEquals(statement.Expression, expression) )
				{
					m_StatementList.RemoveAt(index);
					return index;
				}
			}

			return -1;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Attach(StatementScope scope)
		{
			m_OwnerMethod = scope.OwnerMethod; 
			m_ParentBlock = (scope.Previous != null ? scope.Previous.StatementBlock : null);
			m_Depth = (m_ParentBlock != null ? m_ParentBlock.Depth + 1 : 0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition GetClosureDefinition()
		{
			if ( m_ClosureDefinition == null )
			{
				m_ClosureDefinition = new ClosureDefinition(this);
			}

			return m_ClosureDefinition;
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

		public int Depth
		{
			get
			{
				return m_Depth;
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
