using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	internal class ClosureIdentificationVisitor : OperandVisitorBase
	{
		private readonly MethodMember m_Method;
		private readonly HashSet<IOperand> m_Externals = new HashSet<IOperand>();
		private readonly HashSet<OperandCapture> m_Captures = new HashSet<OperandCapture>();
		private readonly Dictionary<StatementBlock, ClosureDefinition> m_Closures = new Dictionary<StatementBlock, ClosureDefinition>();
		private ClosureDefinition m_OutermostClosure = null;
		private ClosureDefinition m_InnermostClosure = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureIdentificationVisitor(MethodMember method)
		{
			m_Method = method;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void DefineClosures()
		{
			if ( ClosuresRequired )
			{
				MapScopedCapturesToClosures();
				LinkParentChildClosures();
				FindInnermostClosure();
				MapUnscopedCapturesToOutermostClosure();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] Externals
		{
			get
			{
				return m_Externals.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public OperandCapture[] Captures
		{
			get
			{
				return m_Captures.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool ClosuresRequired
		{
			get
			{
				return (m_Externals.Count > 0);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition OutermostClosure
		{
			get
			{
				return m_OutermostClosure;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition InnermostClosure
		{
			get
			{
				return m_InnermostClosure;
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
			var operandHome = ((IScopedOperand)operand).HomeStatementBlock;

			if ( operandHome != null )
			{
				if ( operandHome.OwnerMethod != m_Method )
				{
					m_Externals.Add(operand);
					m_Captures.Add(new OperandCapture(operand, operandHome));
				}
			}
			else
			{
				m_Captures.Add(new OperandCapture(operand, sourceOperandHome: null));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void MapScopedCapturesToClosures()
		{
			foreach ( var capture in m_Captures.Where(c => c.SourceOperandHome != null) )
			{
				ClosureDefinition closure;

				if ( !m_Closures.TryGetValue(capture.SourceOperandHome, out closure) )
				{
					closure = new ClosureDefinition(capture.SourceOperandHome);
					m_Closures.Add(closure.ScopeBlock, closure);
				}

				closure.AddCapture(capture);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void LinkParentChildClosures()
		{
			foreach ( var closure in m_Closures.Values )
			{
				var parentClosure = TryFindParentClosure(closure);

				if ( parentClosure != null )
				{
					closure.AttachToParent(parentClosure);
				}
				else
				{
					Debug.Assert(m_OutermostClosure == null, "Duplicate outermost closure identified.");
					m_OutermostClosure = closure;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void FindInnermostClosure()
		{
			Debug.Assert(m_OutermostClosure != null, "Closures are required, but no closures were defined.");
			m_InnermostClosure = m_Closures.Values.Single(closure => closure.ChildCount == 0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void MapUnscopedCapturesToOutermostClosure()
		{
			foreach ( var capture in m_Captures.Where(c => c.SourceOperandHome == null) )
			{
				m_OutermostClosure.AddCapture(capture);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ClosureDefinition TryFindParentClosure(ClosureDefinition closure)
		{
			var scopeBlock = closure.ScopeBlock.ParentBlock;

			while ( scopeBlock != null )
			{
				ClosureDefinition parentClosure;

				if ( m_Closures.TryGetValue(scopeBlock, out parentClosure) )
				{
					return parentClosure;
				}

				scopeBlock = scopeBlock.ParentBlock;
			}

			return null;
		}
	}
}
