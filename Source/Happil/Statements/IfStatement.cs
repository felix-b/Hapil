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
		private readonly IHappilOperand<bool> m_Condition;
		private readonly List<IHappilStatement> m_ThenBlock;
		private readonly List<IHappilStatement> m_ElseBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IfStatement(IHappilOperand<bool> condition)
		{
			m_Condition = condition;
			m_ThenBlock = new List<IHappilStatement>();
			m_ElseBlock = new List<IHappilStatement>();

			StatementScope.Current.Consume(condition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			var afterIfBlock = il.DefineLabel();
			var afterElseBlock = (m_ElseBlock.Count > 0 ? il.DefineLabel() : new Label());

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);
			
			il.Emit(OpCodes.Brfalse_S, afterIfBlock);

			foreach ( var statement in m_ThenBlock )
			{
				statement.Emit(il);
			}

			if ( m_ElseBlock.Count > 0 )
			{
				il.Emit(OpCodes.Br_S, afterElseBlock);
			}
			
			il.MarkLabel(afterIfBlock);

			if ( m_ElseBlock.Count > 0 )
			{
				foreach ( var statement in m_ElseBlock )
				{
					statement.Emit(il);
				}

				il.MarkLabel(afterElseBlock);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilIfBody Members

		public IHappilIfBodyThen Then(Action thenBodyDefinition)
		{
			using ( new StatementScope(m_ThenBlock) )
			{
				thenBodyDefinition();
			}

			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilIfBodyThen Members

		public void Else(Action elseBodyDefinition)
		{
			using ( new StatementScope(m_ElseBlock) )
			{
				elseBodyDefinition();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilIfBody ElseIf(IHappilOperand<bool> condition)
		{
			var nestedIf = new IfStatement(condition);
			m_ElseBlock.Add(nestedIf);
			return nestedIf;
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
