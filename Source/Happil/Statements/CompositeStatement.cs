using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal class CompositeStatement : Statement
	{
		private readonly List<Statement> m_ContainedStatements;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CompositeStatement()
		{
			m_ContainedStatements = new List<Statement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CompositeStatement(IEnumerable<Statement> containedStatements)
		{
			m_ContainedStatements = new List<Statement>(containedStatements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Add(Statement statement)
		{
			m_ContainedStatements.Add(statement);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void Emit(ILGenerator il)
		{
			foreach ( var statement in m_ContainedStatements )
			{
				statement.Emit(il);
			}
		}
	}
}
