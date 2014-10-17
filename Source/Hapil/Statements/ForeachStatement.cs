using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Operands;
using Hapil.Expressions;

namespace Hapil.Statements
{
	internal class ForeachStatement<TElement> :
		LoopStatementBase,
		IHapilForeachInSyntax<TElement>,
		IHapilForeachInDoSyntax<TElement>,
		IHapilForeachDoSyntax<TElement>
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
			visitor.VisitStatement(m_InnerWhile);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForeachInSyntax<TItem> Members

		public IHapilForeachInDoSyntax<TElement> In(IOperand<IEnumerable<TElement>> collection)
		{
			m_Collection = collection;
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForeachInDoSyntax<TElement> Members

		public void Do(Action<ILoopBody> body)
		{
			Do((loop, element) => body(loop));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilForeachDoSyntax<TItem> Members

		public void Do(Action<ILoopBody, Local<TElement>> body)
		{
			using ( var scope = new StatementScope(m_BodyBlock, loopStatement: this) )
			{
				var writer = scope.Writer;
				var enumerator = writer.Local<IEnumerator<TElement>>();

				enumerator.Assign(m_Collection.CastTo<IEnumerable<TElement>>().Func<IEnumerator<TElement>>(x => x.GetEnumerator));

				writer.Using(enumerator).Do(() => {
					m_InnerWhile = (WhileStatement)writer.While(enumerator.Func<bool>(e => e.MoveNext));
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

	public interface IHapilForeachInSyntax<TItem>
	{
		IHapilForeachInDoSyntax<TItem> In(IOperand<IEnumerable<TItem>> collection);
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilForeachInDoSyntax<TItem>
	{
		void Do(Action<ILoopBody> body);
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilForeachDoSyntax<TItem>
	{
		void Do(Action<ILoopBody, Local<TItem>> body);
	}
}
