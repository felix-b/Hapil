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
	internal class ClosureIdentificationVisitor : OperandVisitorBase, IClosureIdentification
	{
		private readonly MethodMember m_Method;
		private readonly HashSet<IOperand> m_Externals = new HashSet<IOperand>();
		private readonly HashSet<OperandCapture> m_Captures = new HashSet<OperandCapture>();
		private readonly Dictionary<StatementBlock, ClosureDefinition> m_Closures = new Dictionary<StatementBlock, ClosureDefinition>();
		private ClosureDefinition m_OutermostClosure = null;
		private ClosureDefinition m_InnermostClosure = null;
		private ClosureDefinition[] m_ClosuresFromOuterToInner = null;

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
				ListClosuresInOuterToInnerOrder();
				PushOuterCapturesDownToInnerClosures();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember HostMethod
		{
			get
			{
				if ( m_OutermostClosure != null )
				{
					return m_OutermostClosure.ScopeBlock.OwnerMethod;
				}
				else
				{
					return null;
				}
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

		public ClosureDefinition[] ClosuresOuterToInner
		{
			get
			{
				return m_ClosuresFromOuterToInner;
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
			AddCapture((IScopedOperand)operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void AddCapture(IScopedOperand operand)
		{
			if ( operand.HomeStatementBlock != null )
			{
				if ( operand.HomeStatementBlock.OwnerMethod == m_Method )
				{
					return;
				}

				if ( !m_Externals.Add(operand) )
				{
					return;
				}
			}

			if ( !m_Captures.Any(c => c.SourceOperand == operand) )
			{
				m_Captures.Add(new OperandCapture(operand, operand.HomeStatementBlock));
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

		private void ListClosuresInOuterToInnerOrder()
		{
			var orderedClosureList = new List<ClosureDefinition>();

			for ( var closure = m_InnermostClosure ; closure != null ; closure = closure.Parent )
			{
				orderedClosureList.Add(closure);
			}

			orderedClosureList.Reverse();
			m_ClosuresFromOuterToInner = orderedClosureList.ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void PushOuterCapturesDownToInnerClosures()
		{
			foreach ( var closure in m_ClosuresFromOuterToInner )
			{
				closure.PullCapturesFromParent();
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
