using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;

namespace Happil.Statements
{
	internal class UsingStatement : StatementBase, IHappilUsingSyntax
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
			visitor.VisitOperand(ref m_Disposable);
			visitor.VisitStatementBlock(m_BodyBlock);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilUsingSyntax Members

		public void Do(Action body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var method = scope.OwnerMethod.TransparentWriter;
				var disposable = method.Local<IDisposable>(initialValue: m_Disposable.CastTo<IDisposable>());

				method.Try(() => {
					body();
				})
				.Finally(() => {
					method.If(disposable != method.Const<IDisposable>(null)).Then(() => {
						disposable.Void(x => x.Dispose);
					});
				});
			}
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilUsingSyntax
	{
		void Do(Action body);
	}
}
