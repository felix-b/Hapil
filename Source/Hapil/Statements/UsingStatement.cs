using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Statements
{
	internal class UsingStatement : StatementBase, IHapilUsingSyntax
	{
		private readonly StatementBlock m_BodyBlock;
		private IOperand<IDisposable> m_Disposable;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public UsingStatement(IOperand<IDisposable> disposable)
		{
			m_Disposable = disposable;
			m_BodyBlock = new StatementBlock();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region StatementBase Members

        public override void Emit(ILGenerator il, MethodMember ownerMethod)
		{
			foreach ( var statement in m_BodyBlock )
			{
				statement.Emit(il, ownerMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Disposable);
			visitor.VisitStatementBlock(m_BodyBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHapilUsingSyntax Members

		public void Do(Action body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var writer = scope.Writer;
				var disposable = writer.Local<IDisposable>(initialValue: m_Disposable.CastTo<IDisposable>());

				writer.Try(() => {
					body();
				})
				.Finally(() => {
					writer.If(disposable != writer.Const<IDisposable>(null)).Then(() => {
						disposable.Void(x => x.Dispose);
					});
				});
			}
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHapilUsingSyntax
	{
		void Do(Action body);
	}
}
