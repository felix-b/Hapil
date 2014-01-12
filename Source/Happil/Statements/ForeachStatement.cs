using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class ForeachStatement<TElement> : 
		LoopStatementBase, 
		IHappilStatement, 
		IHappilForeachInSyntax<TElement>,
		IHappilForeachDoSyntax<TElement>
	{
		private readonly HappilLocal<TElement> m_Element;
		private readonly List<IHappilStatement> m_BodyBlock;
		private IHappilOperand<IEnumerable<TElement>> m_Collection;
		private WhileStatement m_InnerWhile;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ForeachStatement(HappilLocal<TElement> element)
		{
			m_Element = element;
			m_BodyBlock = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForeachInSyntax<TItem> Members

		public IHappilForeachDoSyntax<TElement> In(IHappilOperand<IEnumerable<TElement>> collection)
		{
			m_Collection = collection;
			return this;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilForeachDoSyntax<TItem> Members

		public void Do(Action<IHappilLoopBody, HappilLocal<TElement>> body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var method = scope.OwnerMethod;
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
		IHappilForeachDoSyntax<TItem> In(IHappilOperand<IEnumerable<TItem>> collection);
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilForeachDoSyntax<TItem>
	{
		void Do(Action<IHappilLoopBody, HappilLocal<TItem>> body);
	}
}
