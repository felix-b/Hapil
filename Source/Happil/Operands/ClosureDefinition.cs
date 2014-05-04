﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Operands
{
	internal class ClosureDefinition
	{
		private readonly StatementBlock m_ScopeBlock;
		private readonly ClassType m_OwnerClass;
		private readonly MethodMember m_OwnerMethod;
		private readonly List<OperandCapture> m_Captures;
		private readonly List<ClosureDefinition> m_Children;
		private ClosureDefinition m_Parent;
		private ClassType m_ClosureClass;
		private IOperand m_ClosureInstanceReference;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition(StatementBlock scopeBlock)
		{
			m_ScopeBlock = scopeBlock;
			m_OwnerMethod = scopeBlock.OwnerMethod;
			m_OwnerClass = scopeBlock.OwnerMethod.OwnerClass;
			m_Captures = new List<OperandCapture>();
			m_Parent = null;
			m_Children = new List<ClosureDefinition>();
			m_ClosureClass = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AttachToParent(ClosureDefinition parent)
		{
			ValidateMutability();

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
			ValidateMutability();
			m_Captures.Add(capture);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ImplementClosure()
		{
			if ( m_ClosureClass != null )
			{
				return;
			}

			m_ClosureClass = new NestedClassType(
				containingClass: m_OwnerClass,
				classFullName: m_OwnerMethod.Name + "Closure",
				baseType: typeof(object));

			var writer = new ImplementationClassWriter<object>(m_ClosureClass);

			foreach ( var capture in m_Captures )
			{
				//writer.DefineField(capture.)
			}

			writer.Flush();
			m_ClosureInstanceReference = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition Parent
		{
			get
			{
				return m_Parent;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClosureDefinition[] Children
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType ClosureClass
		{
			get
			{
				return m_ClosureClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand ClosureInstanceReference
		{
			get
			{
				return m_ClosureInstanceReference;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void ValidateMutability()
		{
			if ( m_ClosureClass != null )
			{
				throw new InvalidOperationException("Cannot change closure definition because closure class was already created.");
			}
		}
	}
}