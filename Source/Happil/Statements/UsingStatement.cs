using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class UsingStatement : IHappilStatement, IHappilUsingSyntax
	{
		private readonly IHappilOperand<IDisposable> m_Disposable;
		private readonly List<IHappilStatement> m_BodyBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public UsingStatement(IHappilOperand<IDisposable> disposable)
		{
			m_Disposable = disposable;
			m_BodyBlock = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilUsingSyntax Members

		public void Do(Action body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var method = scope.OwnerMethod;
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
