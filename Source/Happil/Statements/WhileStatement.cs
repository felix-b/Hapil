using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Expressions;

namespace Happil.Statements
{
	internal class WhileStatement : LoopStatementBase, IHappilWhileSyntax
	{
		private readonly IOperand<bool> m_Condition;
		private readonly List<StatementBase> m_BodyBlock;
		private Label m_LoopStartLabel;
		private Label m_LoopEndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public WhileStatement(IOperand<bool> condition)
		{
			m_Condition = condition;
			m_BodyBlock = new List<StatementBase>();

			StatementScope.Current.Consume(condition);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public override void Emit(ILGenerator il)
		{
			m_LoopStartLabel = il.DefineLabel();
			m_LoopEndLabel = il.DefineLabel();

			il.MarkLabel(m_LoopStartLabel);

			m_Condition.EmitTarget(il);
			m_Condition.EmitLoad(il);

			il.Emit(OpCodes.Brfalse, m_LoopEndLabel);

			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}

			il.Emit(OpCodes.Br, m_LoopStartLabel);
			il.MarkLabel(m_LoopEndLabel);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilWhileSyntax Members

		public void Do(Action<ILoopBody> body)
		{
			using ( new StatementScope(m_BodyBlock) )
			{
				body(this);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopStartLabel
		{
			get
			{
				return m_LoopStartLabel;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopEndLabel
		{
			get
			{
				return m_LoopEndLabel;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilWhileSyntax
	{
		void Do(Action<ILoopBody> body);
	}
}
