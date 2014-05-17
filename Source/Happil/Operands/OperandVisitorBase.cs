using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Statements;

namespace Happil.Operands
{
	internal abstract class OperandVisitorBase
	{
		private readonly HashSet<OperandKind> m_OperandKindFilter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected OperandVisitorBase()
			: this(null)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected OperandVisitorBase(params OperandKind[] operandKindFilter)
		{
			m_OperandKindFilter = new HashSet<OperandKind>(operandKindFilter ?? new OperandKind[0]);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void VisitOperand(ref IOperand operand)
		{
			if ( operand != null )
			{
				var originalOperand = operand;

				if ( OnFilterOperand(operand) )
				{
					OnVisitOperand(ref operand);
				}

				if ( ReferenceEquals(operand, originalOperand) )
				{
					VisitAcceptor(operand as IAcceptOperandVisitor);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void VisitOperand<T>(ref IOperand<T> operand)
		{
			if ( operand != null )
			{
				IOperand replaceable = operand;

				if ( OnFilterOperand(operand) )
				{
					OnVisitOperand(ref replaceable);
				}

				if ( ReferenceEquals(operand, replaceable) )
				{
					VisitAcceptor(operand as IAcceptOperandVisitor);
				}
				else
				{
					operand = replaceable.CastTo<T>();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void VisitOperand<TOperand>(ref TOperand operand) where TOperand : class, IOperand
		{
			if ( operand != null )
			{
				IOperand replaceable = operand;

				if ( OnFilterOperand(operand) )
				{
					OnVisitOperand(ref replaceable);
				}

				if ( ReferenceEquals(operand, replaceable) )
				{
					VisitAcceptor(operand as IAcceptOperandVisitor);
				}
				else
				{
					operand = (TOperand)replaceable;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void VisitOperandArray(IOperand[] operandArray)
		{
			if ( operandArray != null )
			{
				for ( int i = 0 ; i < operandArray.Length ; i++ )
				{
					VisitOperand(ref operandArray[i]);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void VisitStatementBlock(StatementBlock statementBlock)
		{
			if ( statementBlock != null )
			{
				foreach ( var statement in statementBlock )
				{
					statement.AcceptVisitor(this);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void VisitAcceptor(IAcceptOperandVisitor acceptor)
		{
			if ( acceptor != null )
			{
				acceptor.AcceptVisitor(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected bool TestOperandFilter(IOperand operand)
		{
			return (operand != null && (m_OperandKindFilter.Count == 0 || m_OperandKindFilter.Contains(operand.Kind)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool OnFilterOperand(IOperand operand)
		{
			return TestOperandFilter(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void OnVisitOperand(ref IOperand operand);
	}
}
