using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Statements
{
	internal class StatementScope : IDisposable
	{
		private readonly StatementScope m_Previous;
		private readonly StatementScope m_Root;
		private readonly ClassType m_OwnerClass;
		private readonly MethodMember m_OwnerMethod;
		private readonly StatementBlock m_StatementBlock;
		private readonly int m_Depth;
		private readonly TryStatement m_InheritedExceptionStatement;
		private readonly ExceptionBlockType m_InheritedExceptionBlockType;
		private readonly TryStatement m_ThisExceptionStatement;
		private readonly ExceptionBlockType m_ThisExceptionBlockType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope(ClassType ownerClass, MethodMember ownerMethod, StatementBlock statementBlock)
		{
			if ( s_Current != null )
			{
				throw new InvalidOperationException("Root scope already exists.");
			}

			m_OwnerMethod = ownerMethod;
			m_OwnerClass = ownerClass;
			m_Depth = 0;

			m_ThisExceptionBlockType = ExceptionBlockType.None;
			m_ThisExceptionStatement = null;
			m_InheritedExceptionStatement = null;
			m_InheritedExceptionBlockType = ExceptionBlockType.None;

			m_Previous = null;
			m_Root = this;
			s_Current = this;
			
			m_StatementBlock = AttachStatementBlock(statementBlock);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope(StatementBlock statementBlock)
		{
			m_Previous = s_Current;
			m_Root = m_Previous.Root;

			if ( m_Previous == null )
			{
				throw new InvalidOperationException("Parent scope is not present.");
			}

			m_StatementBlock = statementBlock;
			m_OwnerMethod = m_Previous.m_OwnerMethod;
			m_OwnerClass = m_Previous.m_OwnerClass;
			m_Depth = m_Previous.Depth + 1;

			m_ThisExceptionBlockType = ExceptionBlockType.None;
			m_ThisExceptionStatement = null;
			m_InheritedExceptionStatement = m_Previous.InheritedExceptionStatement;
			m_InheritedExceptionBlockType = m_Previous.InheritedExceptionBlockType;

			m_StatementBlock = AttachStatementBlock(statementBlock);
			s_Current = this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope(StatementBlock statementBlock, TryStatement exceptionStatement, ExceptionBlockType blockType)
			: this(statementBlock)
		{
			m_ThisExceptionStatement = exceptionStatement;
			m_ThisExceptionBlockType = blockType;
			m_InheritedExceptionStatement = exceptionStatement;
			m_InheritedExceptionBlockType = blockType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IDisposable Members

		public void Dispose()
		{
			if ( s_Current != this )
			{
				throw new InvalidOperationException("Specified scope is not the current scope.");
			}

			s_Current = m_Previous;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AddStatement(StatementBase statement)
		{
			var effectiveStatementToAdd = statement;
			var leaveStatement = (statement as ILeaveStatement);

			if ( leaveStatement != null && m_InheritedExceptionBlockType != ExceptionBlockType.None )
			{
				var tryStatement = FindOutermostTryStatementWithin(leaveStatement.HomeScope);

				if ( tryStatement != null )
				{
					effectiveStatementToAdd = tryStatement.WrapLeaveStatement(leaveStatement);
				}
			}
			
			m_StatementBlock.Add(effectiveStatementToAdd);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RegisterExpressionStatement(IExpressionOperand expression)
		{
			if ( expression != null )
			{
				m_StatementBlock.Add(new ExpressionStatement(expression));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Consume(IOperand operand)
		{
			var expression = (operand as IExpressionOperand);

			if ( expression != null )
			{
				Consume(expression);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Consume(IExpressionOperand expression)
		{
			if ( expression != null )
			{
				m_StatementBlock.RemoveExpressionStatement(expression);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement FindOutermostTryStatementWithin(StatementScope homeScope)
		{
			TryStatement result = null;
			var scope = this;

			while ( !ReferenceEquals(scope, homeScope) )
			{
				if ( scope == null )
				{
					throw new Exception("Internal error: bad scope hierarchy.");
				}

				if ( scope.ThisExceptionBlockType == ExceptionBlockType.Finally )
				{
					throw new InvalidOperationException("Leaving from withing FINALLY block is not allowed.");
				}

				if ( scope.ThisExceptionBlockType != ExceptionBlockType.None )
				{
					result = scope.ThisExceptionStatement;
				}

				scope = scope.m_Previous;
			}

			return result;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DynamicModule OwnerModule
		{
			get
			{
				return m_OwnerClass.Module;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get
			{
				return m_OwnerClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock StatementBlock
		{
			get
			{
				return m_StatementBlock;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope Previous
		{
			get
			{
				return m_Previous;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope Root
		{
			get
			{
				return m_Root;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int Depth
		{
			get
			{
				return m_Depth;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement InheritedExceptionStatement
		{
			get
			{
				return m_InheritedExceptionStatement;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExceptionBlockType InheritedExceptionBlockType
		{
			get
			{
				return m_InheritedExceptionBlockType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TryStatement ThisExceptionStatement
		{
			get
			{
				return m_ThisExceptionStatement;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ExceptionBlockType ThisExceptionBlockType
		{
			get
			{
				return m_ThisExceptionBlockType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal LabelStatement DefineLabel()
		{
			return new LabelStatement(m_OwnerMethod, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private StatementBlock AttachStatementBlock(StatementBlock statementBlock)
		{
			statementBlock.Attach(this);
			return statementBlock;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[ThreadStatic]
		private static StatementScope s_Current;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void Cleanup()
		{
			s_Current = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static StatementScope Current
		{
			get
			{
				var current = s_Current;

				if ( current == null )
				{
					throw new InvalidOperationException("There is no active scope at the moment.");
				}

				return current;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool Exists
		{
			get
			{
				return (s_Current != null);
			}
		}
	}
}
