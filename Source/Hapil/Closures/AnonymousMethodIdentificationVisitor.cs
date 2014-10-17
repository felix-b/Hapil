using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;

namespace Hapil.Closures
{
	internal class AnonymousMethodIdentificationVisitor : OperandVisitorBase, IAnonymousMethodIdentification
	{
		private readonly MethodMember m_HostMethod;
		private readonly HashSet<IOperand> m_MustCloseOverOperands = new HashSet<IOperand>();
		private readonly List<OperandCapture> m_Captures = new List<OperandCapture>();
		private readonly Dictionary<StatementBlock, ClosureDefinition> m_Closures = new Dictionary<StatementBlock, ClosureDefinition>();
		private readonly Dictionary<IAnonymousMethodOperand, AnonymousMethodScope> m_AnonymousMethods = new Dictionary<IAnonymousMethodOperand, AnonymousMethodScope>();
		private IAnonymousMethodOperand m_CurrentAnonymousMethod;
		private ClosureDefinition m_OutermostClosure = null;
		private ClosureDefinition m_InnermostClosure = null;
		private ClosureDefinition[] m_ClosuresFromOuterToInner = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousMethodIdentificationVisitor(MethodMember hostMethod)
		{
			m_HostMethod = hostMethod;
			m_CurrentAnonymousMethod = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of OperandVisitorBase

		public override void VisitAcceptor(IAcceptOperandVisitor acceptor)
		{
			if ( acceptor is IAnonymousMethodOperand && object.ReferenceEquals(m_CurrentAnonymousMethod, null) )
			{
				return;
			}

			base.VisitAcceptor(acceptor);
		}

		#endregion

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

		//public void Merge(IAnonymousMethodIdentification other)
		//{
		//	if ( this.ClosuresOuterToInner != null )
		//	{
		//		throw new InvalidOperationException("Cannot merge other identifications because current identification already has defined closures.");
		//	}

		//	if ( other.ClosuresOuterToInner != null )
		//	{
		//		throw new ArgumentException("Cannot merge specified identification because it already has defined closures.");
		//	}

		//	m_MustCloseOverOperands.UnionWith(other.MustCloseOverOperands);

		//	foreach ( var otherCapture in other.Captures )
		//	{
		//		var existingCapture = m_Captures.FirstOrDefault(c => c.SourceOperand == otherCapture.SourceOperand);

		//		if ( existingCapture != null )
		//		{
		//			existingCapture.Merge(otherCapture);
		//		}
		//		else
		//		{
		//			m_Captures.Add(otherCapture);
		//		}
		//	}

		//	foreach ( var anonymousMethod in other.AnonymousMethods )
		//	{
		//		AddOrUpdateAnonymousMethod(anonymousMethod, other.GetAnonymousMethodScope(anonymousMethod));
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousMethodScope GetAnonymousMethodScope(IAnonymousMethodOperand anonymousMethod)
		{
			return m_AnonymousMethods[anonymousMethod];
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand[] MustCloseOverOperands
		{
			get
			{
				return m_MustCloseOverOperands.ToArray();
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
				return m_AnonymousMethods.Keys.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool AnonymousMethodsFound
		{
			get
			{
				return (m_AnonymousMethods.Count > 0);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool ClosuresRequired
		{
			get
			{
				return (m_MustCloseOverOperands.Count > 0);
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
			var isTopLevelAnonymousMethod = (m_CurrentAnonymousMethod == null);

			try
			{
				if ( isTopLevelAnonymousMethod )
				{
					m_CurrentAnonymousMethod = anonymousMethodOperand;
					m_AnonymousMethods.Add(anonymousMethodOperand, AnonymousMethodScope.Static);
				}

				base.VisitStatementBlock(anonymousMethodOperand.Statements);
			}
			finally
			{
				if ( isTopLevelAnonymousMethod )
				{
					m_CurrentAnonymousMethod = null;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void CaptureOperandIfExternal(IScopedOperand operand)
		{
			if ( operand.HomeStatementBlock != null )
			{
				var mustCloseOver = (operand.HomeStatementBlock.OwnerMethod != null);

				if ( !mustCloseOver )
				{
					return;
				}

				m_MustCloseOverOperands.Add(operand);
			}
			else 
			{
				AddOrUpdateAnonymousMethod(m_CurrentAnonymousMethod, AnonymousMethodScope.Instance);
			}

			var existingCapture = m_Captures.FirstOrDefault(c => c.SourceOperand == operand);

			if ( existingCapture != null )
			{
				existingCapture.AddConsumer(m_CurrentAnonymousMethod);
			}
			else 
			{
				m_Captures.Add(new OperandCapture(operand, operand.HomeStatementBlock, consumerMethod: m_CurrentAnonymousMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void AddOrUpdateAnonymousMethod(IAnonymousMethodOperand anonymousMethod, AnonymousMethodScope scope)
		{
			AnonymousMethodScope existingScope;

			if ( !m_AnonymousMethods.TryGetValue(anonymousMethod, out existingScope) || scope > existingScope )
			{
				m_AnonymousMethods[anonymousMethod] = scope;
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
				return (m_CurrentAnonymousMethod != null);
			}
		}
	}
}
