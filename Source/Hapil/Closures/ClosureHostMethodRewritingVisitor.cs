using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Closures
{
	internal class ClosureHostMethodRewritingVisitor : OperandVisitorBase
	{
		private readonly IAnonymousMethodIdentification m_Identification;
		private readonly Stack<ClosureDefinition> m_EffectiveClosures;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureHostMethodRewritingVisitor(IAnonymousMethodIdentification identification)
		{
			m_Identification = identification;
			m_EffectiveClosures = new Stack<ClosureDefinition>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void VisitStatementBlock(StatementBlock statementBlock)
		{
			var effectiveClosure = m_Identification.ClosuresOuterToInner.FirstOrDefault(c => c.HostScopeBlock == statementBlock);

			if ( effectiveClosure != null )
			{
				m_EffectiveClosures.Push(effectiveClosure);
			}

			try
			{
				base.VisitStatementBlock(statementBlock);
			}
			finally
			{
				if ( effectiveClosure != null )
				{
					m_EffectiveClosures.Pop();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool OnFilterOperand(IOperand operand)
		{
			return (operand is IScopedOperand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			var effectiveClosure = GetEffectiveClosure();

			if ( effectiveClosure != null )
			{
				effectiveClosure.RewriteOperandIfCaptured(ref operand);
			}
		}


	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ClosureDefinition GetEffectiveClosure()
		{
			if ( m_EffectiveClosures.Count > 0 )
			{
				return m_EffectiveClosures.Peek();
			}
			else
			{
				return null;
			}
		}
	}
}
