using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class ExpressionStatement : IHappilStatement
	{
		private readonly IHappilExpression m_Expression;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExpressionStatement(IHappilExpression expression)
		{
			m_Expression = expression;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			m_Expression.EmitTarget(il);
			m_Expression.EmitLoad(il);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return m_Expression.ToString();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilExpression Expression
		{
			get
			{
				return m_Expression;
			}
		}
	}
}
