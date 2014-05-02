using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Expressions;

namespace Happil.Statements
{
	internal class ForeachStatement<TElement> :
		LoopStatementBase,
		IHappilForeachInSyntax<TElement>,
		IHappilForeachInDoSyntax<TElement>,
		IHappilForeachDoSyntax<TElement>
	{
		private readonly StatementBlock m_BodyBlock;
		private Local<TElement> m_Element;
		private IOperand<IEnumerable<TElement>> m_Collection;
		private WhileStatement m_InnerWhile;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ForeachStatement(Local<TElement> element)
		{
			m_Element = element;
			m_BodyBlock = new StatementBlock();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

		public override void Emit(ILGenerator il)
		{
			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Element);
			visitor.VisitOperand(ref m_Collection);
			visitor.VisitStatementBlock(m_BodyBlock);
			m_InnerWhile.AcceptVisitor(visitor);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForeachInSyntax<TItem> Members

		public IHappilForeachInDoSyntax<TElement> In(IOperand<IEnumerable<TElement>> collection)
		{
			m_Collection = collection;
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForeachInDoSyntax<TElement> Members

		public void Do(Action<ILoopBody> body)
		{
			Do((loop, element) => body(loop));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForeachDoSyntax<TItem> Members

		public void Do(Action<ILoopBody, Local<TElement>> body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var method = scope.OwnerMethod.TransparentWriter;
				var enumerator = method.Local<IEnumerator<TElement>>();

				enumerator.Assign(m_Collection.CastTo<IEnumerable<TElement>>().Func<IEnumerator<TElement>>(x => x.GetEnumerator));

				method.Using(enumerator).Do(() => {
					m_InnerWhile = (WhileStatement)method.While(enumerator.Func<bool>(e => e.MoveNext));
					m_InnerWhile.Do(loop => {
						m_Element.Assign(enumerator.Prop(e => e.Current));
						body(this, m_Element);
					});
				});
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopStartLabel
		{
			get
			{
				return m_InnerWhile.LoopStartLabel;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Label LoopEndLabel
		{
			get
			{
				return m_InnerWhile.LoopEndLabel;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForeachInSyntax<TItem>
	{
		IHappilForeachInDoSyntax<TItem> In(IOperand<IEnumerable<TItem>> collection);
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForeachInDoSyntax<TItem>
	{
		void Do(Action<ILoopBody> body);
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForeachDoSyntax<TItem>
	{
		void Do(Action<ILoopBody, Local<TItem>> body);
	}
}
