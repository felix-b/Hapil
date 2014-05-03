using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Statements;

namespace Happil.Operands
{
	internal class MethodClosure
	{
		private readonly StatementBlock m_ScopeBlock;
		private readonly List<OperandCapture> m_Captures;
		private readonly List<MethodClosure> m_Children;
		private MethodClosure m_Parent;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodClosure(StatementBlock scopeBlock)
		{
			m_ScopeBlock = scopeBlock;
			m_Captures = new List<OperandCapture>();
			m_Parent = null;
			m_Children = new List<MethodClosure>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AttachToParent(MethodClosure parent)
		{
			if ( m_Parent != null )
			{
				throw new InvalidOperationException("Current closure already has a parent closure attached.");
			}

			m_Parent = parent;

			if ( parent != null )
			{
				parent.m_Children.Add(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AddCapture(OperandCapture capture)
		{
			m_Captures.Add(capture);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodClosure Parent
		{
			get
			{
				return m_Parent;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodClosure[] Children
		{
			get
			{
				return m_Children.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int ChildCount
		{
			get
			{
				return m_Children.Count;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock ScopeBlock
		{
			get
			{
				return m_ScopeBlock;
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
	}
}
