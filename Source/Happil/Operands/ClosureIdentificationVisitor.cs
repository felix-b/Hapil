﻿#if false

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
		private readonly List<IAnonymousMethodOperand> m_AnonymousMethodOperands = new List<IAnonymousMethodOperand>();
		private IAnonymousMethodOperand m_CurrentAnonymousMethodOperand;
		private ClosureDefinition m_OutermostClosure = null;
		private ClosureDefinition m_InnermostClosure = null;
		private ClosureDefinition[] m_ClosuresFromOuterToInner = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureIdentificationVisitor(MethodMember method)
		{
			m_Method = method;
			m_CurrentAnonymousMethodOperand = null;
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
				MapAnonymousMethodsToClosures();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Merge(IClosureIdentification other)
		{
			if ( this.ClosuresOuterToInner != null )
			{
				throw new InvalidOperationException("Cannot merge other identifications because current identification already has defined closures.");
			}

			if ( other.ClosuresOuterToInner != null )
			{
				throw new ArgumentException("Cannot merge specified identification because it already has defined closures.");
			}

			m_Externals.UnionWith(other.MustCloseOverOperands);

			foreach ( var otherCapture in other.Captures )
			{
				var existingCapture = m_Captures.FirstOrDefault(c => c.SourceOperand == otherCapture.SourceOperand);

				if ( existingCapture != null )
				{
					existingCapture.Merge(otherCapture);
				}
				else
				{
					m_Captures.Add(otherCapture);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] MustCloseOverOperands
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

		public IAnonymousMethodOperand[] AnonymousMethods
		{
			get
			{
				return m_AnonymousMethodOperands.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool AnonymousMethodsFound
		{
			get
			{
				return (m_AnonymousMethodOperands.Count > 0);
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
			return (operand is IAnonymousMethodOperand || operand is IScopedOperand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnVisitOperand(ref IOperand operand)
		{
			var anonymousMethodOperand = (operand as IAnonymousMethodOperand);

			if ( anonymousMethodOperand != null )
			{
				OnVisitAnonymousMethod(anonymousMethodOperand);
			}
			else if ( IsVisitorInsideAnonymousMethod )
			{
				CaptureOperandIfExternal((IScopedOperand)operand);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void OnVisitAnonymousMethod(IAnonymousMethodOperand anonymousMethodOperand)
		{
			var isTopLevelAnonymousMethod = (m_CurrentAnonymousMethodOperand == null);

			try
			{
				if ( isTopLevelAnonymousMethod )
				{
					m_CurrentAnonymousMethodOperand = anonymousMethodOperand;
					m_AnonymousMethodOperands.Add(anonymousMethodOperand);
				}

				base.VisitStatementBlock(anonymousMethodOperand.Statements);
			}
			finally
			{
				if ( isTopLevelAnonymousMethod )
				{
					m_CurrentAnonymousMethodOperand = null;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void CaptureOperandIfExternal(IScopedOperand operand)
		{
			if ( operand.HomeStatementBlock != null )
			{
				var isExternal = (operand.HomeStatementBlock.OwnerMethod == null);

				if ( !isExternal )
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
				m_Captures.Add(new OperandCapture(operand, operand.HomeStatementBlock, consumerMethod: m_CurrentAnonymousMethodOperand));
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
					closure = capture.SourceOperandHome.GetClosureDefinition();
					m_Closures.Add(closure.HostScopeBlock, closure);
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

		private void MapAnonymousMethodsToClosures()
		{
			var hoistedMethods = new HashSet<IAnonymousMethodOperand>();

			for ( var closure = m_InnermostClosure ; closure != null ; closure = closure.Parent )
			{
				foreach ( var anonymousMethod in closure.Captures.SelectMany(c => c.Consumers) )
				{
					if ( hoistedMethods.Add(anonymousMethod) )
					{
						closure.AddAnonymousMethod(anonymousMethod);
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ClosureDefinition TryFindParentClosure(ClosureDefinition closure)
		{
			var scopeBlock = closure.HostScopeBlock.ParentBlock;

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
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool IsVisitorInsideAnonymousMethod
		{
			get
			{
				return (m_CurrentAnonymousMethodOperand != null);
			}
		}
	}
}


#endif