using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal class CompositeStatement : IHappilStatement
	{
		private readonly List<IHappilStatement> m_ContainedStatements;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CompositeStatement()
		{
			m_ContainedStatements = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CompositeStatement(IEnumerable<IHappilStatement> containedStatements)
		{
			m_ContainedStatements = new List<IHappilStatement>(containedStatements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Add(IHappilStatement statement)
		{
			m_ContainedStatements.Add(statement);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Emit(ILGenerator il)
		{
			foreach ( var statement in m_ContainedStatements )
			{
				statement.Emit(il);
			}
		}
	}
}
