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
		private readonly Dictionary<StatementBlock, MethodClosure> m_Closures = new Dictionary<StatementBlock, MethodClosure>();
		private MethodClosure m_OutermostMethodClosure = null;
		private MethodClosure m_InnermostMethodClosure = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureIdentificationVisitor(MethodMember method)
		{
			m_Method = method;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void DefineClosures()
		{
			if ( !IsClosureRequired )
			{
				return;
			}

			foreach ( var capture in m_Captures.Where(c => c.SourceOperandHome != null) )
			{
				MethodClosure closure;
				
				if ( !m_Closures.TryGetValue(capture.SourceOperandHome, out closure) )
				{
					closure = new MethodClosure(capture.SourceOperandHome);
					m_Closures.Add(closure.ScopeBlock, closure);
				}

				closure.AddCapture(capture);
			}

			foreach ( var closure in m_Closures.Values )
			{
				FindClosureParent(closure);
			}

			Debug.Assert(m_OutermostMethodClosure != null, "Closure is required, but no closures were defined.");
			m_InnermostMethodClosure = m_Closures.Values.Single(closure => closure.ChildCount == 0);

			foreach ( var capture in m_Captures.Where(c => c.SourceOperandHome == null) )
			{
				m_OutermostMethodClosure.AddCapture(capture);
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

		public bool IsClosureRequired
		{
			get
			{
				return (m_Externals.Count > 0);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodClosure OutermostMethodClosure
		{
			get
			{
				return m_OutermostMethodClosure;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodClosure InnermostMethodClosure
		{
			get
			{
				return m_InnermostMethodClosure;
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

		private void FindClosureParent(MethodClosure closure)
		{
			var scopeBlock = closure.ScopeBlock.ParentBlock;

			while ( scopeBlock != null )
			{
				MethodClosure parentClosure;

				if ( m_Closures.TryGetValue(scopeBlock, out parentClosure) )
				{
					closure.AttachToParent(parentClosure);
					return;
				}

				scopeBlock = scopeBlock.ParentBlock;
			}

			Debug.Assert(m_OutermostMethodClosure == null, "Duplicate outermost closure identified.");
			m_OutermostMethodClosure = closure;
		}
	}
}
