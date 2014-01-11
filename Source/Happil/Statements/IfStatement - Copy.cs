using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class IfStatement : IHappilStatement, IHappilIfBody, IHappilIfBodyThen
	{
		private readonly List<BodyPart> m_BodyParts;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IfStatement(IHappilOperand<bool> condition)
		{
			m_BodyParts = new List<BodyPart>();
			m_BodyParts.Add(new BodyPart(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			foreach ( var part in m_BodyParts )
			{
				part.Emit(il);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilIfBody Members

		public IHappilIfBodyThen Then(Action thenBodyDefinition)
		{
			m_BodyParts[m_BodyParts.Count - 1].DefineBody(thenBodyDefinition);
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilIfBodyThen Members

		public IHappilIfBody ElseIf(IHappilOperand<bool> condition)
		{
			m_BodyParts.Add(new BodyPart(condition));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Else(Action elseBodyDefinition)
		{
			m_BodyParts.Add(new BodyPart(condition: null));
			Then(elseBodyDefinition);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BodyPart
		{
			private readonly IHappilOperand<bool> m_Condition;
			private readonly List<IHappilStatement> m_Statements;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public BodyPart(IHappilOperand<bool> condition)
			{
				m_Condition = condition;
				m_Statements = new List<IHappilStatement>();

				if ( condition != null )
				{
					StatementScope.Current.Consume(condition);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void DefineBody(Action definition)
			{
				using ( new StatementScope(m_Statements) )
				{
					definition();
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Emit(ILGenerator il)
			{
				var endIf = new Label();

				if ( m_Condition != null )
				{
					endIf = il.DefineLabel();
					
					m_Condition.EmitTarget(il);
					m_Condition.EmitLoad(il);

					il.Emit(OpCodes.Brfalse_S, endIf);
				}

				foreach ( var statement in m_Statements )
				{
					statement.Emit(il);
				}

				if ( m_Condition != null )
				{
					il.MarkLabel(endIf);
				}
			}
		}
	}
}
